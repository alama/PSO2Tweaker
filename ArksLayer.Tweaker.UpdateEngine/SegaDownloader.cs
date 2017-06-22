using ArksLayer.Tweaker.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.UpdateEngine
{
    /// <summary>
    /// A class capable of interacting with SEGA patch server.
    /// </summary>
    internal class SegaDownloader
    {
        /// <summary>
        /// Patchlist URL for the game launcher executables.
        /// </summary>
        private const string LauncherListUrl = PatchBaseUrl + "launcherlist.txt";

        /// <summary>
        /// Base URL for the older patch server.
        /// </summary>
        private const string OldPatchBaseUrl = "http://download.pso2.jp/patch_prod/patches_old/";

        /// <summary>
        /// Patchlist URL from the older patch server.
        /// </summary>
        private const string OldPatchlistUrl = OldPatchBaseUrl + "patchlist.txt";

        /// <summary>
        /// Base URL for the standard patch server.
        /// </summary>
        private const string PatchBaseUrl = "http://download.pso2.jp/patch_prod/patches/";

        /// <summary>
        /// Patchlist URL from the standard patch server.
        /// </summary>
        private const string PatchlistUrl = PatchBaseUrl + "patchlist.txt";

        /// <summary>
        /// Construct an instance of SegaDownloader using the provided UI renderer.
        /// </summary>
        /// <param name="output"></param>
        public SegaDownloader(ITrigger output)
        {
            this.Output = output;
        }
        /// <summary>
        /// Dependency to UI renderer class.
        /// </summary>
        private ITrigger Output { get; set; }

        /// <summary>
        /// Lock object for manipulating the skip list singleton.
        /// </summary>
        private object SkipListLock = new object();

        /// <summary>
        /// Internal container to the list of files to be skipped.
        /// </summary>
        private HashSet<string> _SkipList;

        /// <summary>
        /// Using singleton pattern, fetch a list of file to be skipped.
        /// </summary>
        public HashSet<string> SkipList
        {
            get
            {
                if (_SkipList == null)
                {
                    lock (SkipListLock)
                    {
                        ReadSkipList();
                    }
                }

                return _SkipList;
            }
        }

        /// <summary>
        /// Attempt to read from skip.txt, if exist.
        /// </summary>
        private void ReadSkipList()
        {
            if (_SkipList == null)
            {
                if (File.Exists("skip.txt") == false)
                {
                    _SkipList = new HashSet<string>();
                }
                var ar = File.ReadLines("skip.txt").Select(Q => Q.Trim()).Where(Q => string.IsNullOrEmpty(Q) == false);
                _SkipList = new HashSet<string>(ar);
            }
        }

        /// <summary>
        /// Attempts to download a patch into a target directory.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="directory"></param>
        /// <param name="successLog">If provided, will write a line containing the patch file name then flush immediately into the log when a download is successful.</param>
        /// <param name="attempts">Causes the download to be retried as much as the number allows, with exponential backoff algorithm</param>
        /// <returns>True if download is successful, else false.</returns>
        public async Task<bool> DownloadGamePatch(PatchInfo target, string directory, StreamWriter successLog = null, int attempts = 4)
        {
            if (SkipList.Contains(target.File))
            {
                Output.AppendLog($"Skipping download: {target.File}");
                return true;
            }

            if (attempts < 1) attempts = 1;

            var file = Path.Combine(directory, target.File);
            var folder = new FileInfo(file).DirectoryName;
            //Directory.CreateDirectory(folder);

            var baseUrl = target.Old ? OldPatchBaseUrl : PatchBaseUrl;
            var url = baseUrl + target.File + ".pat";

            for (int i = 0; i < attempts; i++)
            {
                await ExponentialBackoff(i, url);

                var client = new AquaClient();
                var download = client.DownloadFileTaskAsync(url, file);
                Output.AppendLog($"Downloading a file from {url} to {file}");
                Output.OnDownloadStart(url, client);

                try
                {
                    await download;
                    if (new FileInfo(file).Length == 0) throw new Exception("Empty response!");

                    Output.OnDownloadFinish(url);
                    if (successLog != null)
                    {
                        lock (UpdateManager.DownloadSuccessLogLock)
                        {
                            successLog.WriteLine(target.File);
                            successLog.Flush();
                        }
                    }

                    return true;
                }
                catch (Exception Ex)
                {
                    Output.AppendLog($"Download failed {url} because: {Ex.Message}");
                }
            }

            Output.OnDownloadAborted(url);
            return false;
            //throw new Exception("Unable to download " + url);
        }

        /// <summary>
        /// Attempts do download an update patchlist from SEGA server.
        /// The value returned will be a list of patch information.
        /// </summary>
        /// <returns></returns>
        public async Task<List<PatchInfo>> FetchUpdatePatchlist()
        {
            Output.OnPatchlistFetchStart();

            var launcherList = DownloadAndParsePatchlist(LauncherListUrl);
            var patchlist = DownloadAndParsePatchlist(PatchlistUrl);
            var oldPatchlist = DownloadAndParsePatchlist(OldPatchlistUrl);

            await Task.WhenAll(launcherList, patchlist, oldPatchlist);

            var t = Stopwatch.StartNew();

            // Apparently, there are duplicates and the (new) patchlist takes priority before the old patchlist!
            var merge = new Dictionary<string, PatchInfo>();
            MergePatchlist(merge, await launcherList, false);
            MergePatchlist(merge, await patchlist, false);
            MergePatchlist(merge, await oldPatchlist, true);
            Output.Benchmark(t, "Patchlist merge");

            var blacklist = new string[] { "PSO2JP.ini", "GameGuard.des", "user_default.pso2", "user_intel.pso2", UpdateManager.CensorFile };
            var result = merge.Values.AsParallel().Where(Q => FilterPatch(blacklist, Q)).ToList();
            Output.Benchmark(t, "Patchlist blacklist");

            using (var file = File.CreateText("patchlist.json"))
            {
                await file.WriteAsync(JsonConvert.SerializeObject(result));
            }

            Output.Benchmark(t, "Patchlist write");
            Output.OnPatchlistFetchCompleted(result.Count);
            return result;
        }

        /// <summary>
        /// Attempts to download a patchlist from a given url then parse into a sequence of patch information.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<List<PatchInfo>> DownloadAndParsePatchlist(string url)
        {
            var response = await DownloadPatchlist(url);
            var result = await Task.Run(() => ParsePatchlist(response));
            return result;
        }

        /// <summary>
        /// Attempts to download a patchlist from a given url, then read as string and returned to caller.
        /// If download failed, will throw an exception.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="attempts">Causes the download to be retried as much as the number allows, with exponential backoff algorithm.</param>
        /// <returns></returns>
        private async Task<string> DownloadPatchlist(string url, int attempts = 4)
        {
            if (attempts < 1) attempts = 1;

            for (int i = 0; i < attempts; i++)
            {
                await ExponentialBackoff(i, url);

                var client = new AquaClient();
                var download = client.DownloadStringTaskAsync(url);
                Output.OnDownloadStart(url, client);

                try
                {
                    var response = await download;
                    if (string.IsNullOrEmpty(response)) throw new Exception("Empty response!");

                    Output.OnDownloadFinish(url);
                    return response;
                }
                catch (Exception Ex)
                {
                    Output.AppendLog($"Download failed {url} because: {Ex.Message}");
                }
            }

            Output.OnDownloadAborted(url);
            throw new Exception("Unable to download " + url);
        }

        /// <summary>
        /// Grabs a version string for the game client from remote SEGA server.
        /// </summary>
        /// <returns></returns>
        public Task<string> GetRemoteVersion()
        {
            var url = PatchBaseUrl + "version.ver";
            return DownloadPatchlist(url, 5);
        }

        /// <summary>
        /// Induce an artificial delay against file download as not to flood the SEGA server with download requests.
        /// Then notify the user that the download is being delayed.
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private Task ExponentialBackoff(int iteration, string url)
        {
            // 4^1 = 4 seconds
            // 4^2 = 16 seconds
            // 4^3 = 64 seconds (1 minute and 4 seconds) <-- 4th attempt
            // 4^4 = 256 seconds (4 minutes and 16 seconds)
            if (iteration > 0)
            {
                var backoff = Convert.ToInt32(Math.Pow(4, iteration));
                Output.OnDownloadRetry(url, backoff);
                return Task.Delay(backoff * 1000);
            }

            return Task.Delay(1);
        }

        /// <summary>
        /// Uses a passed file blacklist to determine whether a patch should be downloaded.
        /// </summary>
        /// <param name="blacklist"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        private bool FilterPatch(IEnumerable<string> blacklist, PatchInfo patch)
        {
            foreach (var black in blacklist)
            {
                if (patch.File.EndsWith(black, StringComparison.InvariantCultureIgnoreCase)) return false;
            }
            return true;
        }

        /// <summary>
        /// Attempts to merge multiple patches into a dictionary of filename to patch information.
        /// If a filename already exists in the dictionary, then the patch information will be disregarded. (First Come First Serve)
        /// </summary>
        /// <param name="merge"></param>
        /// <param name="patchlist"></param>
        /// <param name="old">Indicates whether the patch should be downloaded from the older patch server</param>
        private void MergePatchlist(Dictionary<string, PatchInfo> merge, List<PatchInfo> patchlist, bool old)
        {
            // Converted concurrent dictionary to standard dictionary because benchmark shows that it can be 60% faster.
            foreach (var p in patchlist)
            {
                if (merge.ContainsKey(p.File) == false)
                {
                    p.Old = old;
                    merge[p.File] = p;
                }
            }
        }
        /// <summary>
        /// Parse a raw downloaded patchlist content.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private List<PatchInfo> ParsePatchlist(string raw)
        {
            var t = Stopwatch.StartNew();

            // patchlist structure: [File]\t[Size]\t[Hash]\r\n

            var result = raw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .AsParallel()
                .Select(Q => ParsePatchlistRow(Q))
                .Where(Q => Q != null) // ParsePatchlistRow may be NULL if exception occured!
                .ToList();

            Output.Benchmark(t, "Parsing patchlist");

            return result;
        }

        /// <summary>
        /// Parse a line extracted from the raw patchlist content.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private PatchInfo ParsePatchlistRow(string row)
        {
            try
            {
                var columns = row.Split('\t');
                var filename = PatTrim(columns[0].Trim());
                var hash = columns[2].Trim();

                return new PatchInfo
                {
                    File = filename,
                    Hash = hash
                };
            }
            catch (Exception Ex)
            {
                Output.AppendLog($"Error parsing patchlist: {JsonConvert.SerializeObject(row)}");
                Debug.WriteLine(Ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Removes .pat extension from the file downloaded.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string PatTrim(string s)
        {
            if (s.EndsWith(".pat", StringComparison.InvariantCultureIgnoreCase))
            {
                return s.Remove(s.Length - ".pat".Length);
            }

            return s;
        }

    }
}
