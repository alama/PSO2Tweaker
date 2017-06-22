using ArksLayer.Tweaker.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.UpdateEngine
{
    /// <summary>
    /// A class capable of performing update, hash and cleanup tasks against the game client files.
    /// </summary>
    public class UpdateManager
    {
        /// <summary>
        /// Lock object for download success log file.
        /// </summary>
        internal static object DownloadSuccessLogLock = new object();

        /// <summary>
        /// File name for storing client hash information.
        /// </summary>
        private const string ClientHashesJson = "client.json";

        /// <summary>
        /// File name for logging files that were successfully downloaded.
        /// </summary>
        private const string DownloadSuccessLog = "done.log";

        /// <summary>
        /// File name for storing missing files information.
        /// </summary>
        private const string MissingFilesJson = "missing.json";

        /// <summary>
        /// File name for storing patch information.
        /// </summary>
        private const string PatchlistJson = "patchlist.json";

        /// <summary>
        /// This file contains thousands of curse words, made by SEGA.
        /// </summary>
        public const string CensorFile = "ffbff2ac5b7a7948961212cefd4d402c";

        /// <summary>
        /// Constructs an instance of UpdateManager requiring a tweaker settings class and a UI renderer class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="output"></param>
        public UpdateManager(ITweakerSettings settings, ITrigger output)
        {
            this.Settings = settings;
            this.Output = output;

            // At the moment, the downloader is tightly coupled against this class.
            // In the future, downloader can be decoupled away in the very case where we have PSO2 private server running a patch server.
            this.Downloader = new SegaDownloader(output);
        }

        /// <summary>
        /// Dependency to patch downloader class.
        /// </summary>
        private SegaDownloader Downloader { get; set; }

        /// <summary>
        /// Dependency to UI renderer class.
        /// </summary>
        private ITrigger Output { get; set; }

        /// <summary>
        /// Dependency to Tweaker Settings class.
        /// </summary>
        private ITweakerSettings Settings { get; set; }

        /// <summary>
        /// Deletes the file that must not exist for the next patching operation and then uncensor the game if not uncensored already.
        /// </summary>
        public Task Housekeeping()
        {
            Output.OnHousekeeping();
            return Task.Run(() =>
            {
                if (File.Exists(PatchlistJson)) File.Delete(PatchlistJson);
                if (File.Exists(MissingFilesJson)) File.Delete(MissingFilesJson);
                if (File.Exists(DownloadSuccessLog)) File.Delete(DownloadSuccessLog);

                //Remove Censor
                var censorPath = Path.Combine(Settings.DataWin32Directory(), CensorFile);
                if (File.Exists(censorPath))
                {
                    Output.AppendLog($"Removing chat censor file: {censorPath}");
                    File.Delete(censorPath);
                }
            });
        }

        /// <summary>
        /// Restore all backups in the backup directory to the assets folder, indiscriminately.
        /// </summary>
        public async Task RestoreBackupFiles()
        {
            if (Directory.Exists(Settings.BackupDirectory()) == false) return;

            Settings.EnglishPatchVersion = "Not Installed";
            Settings.EnglishLargePatchVersion = "Not Installed";
            Settings.StoryPatchVersion = "Not Installed";

            var backupFiles = await Task.Run(() => Directory.EnumerateFiles(Settings.BackupDirectory(), "*.*", SearchOption.AllDirectories));
            if (backupFiles.Any() == false)
            {
                return;
            }

            Output.OnBackupRestore(backupFiles);
            await Task.Run(() =>
            {
                foreach (var file in backupFiles)
                {
                    var fileName = Path.GetFileName(file);
                    var target = Path.Combine(Settings.DataWin32Directory(), fileName);

                    if (File.Exists(target)) File.Delete(target);
                    File.Move(file, target);
                }
            });
        }

        /// <summary>
        /// Execute a game client update operation.
        /// Supports download resume in case of interruptions or due to past failure of downloading certain files.
        /// Uncensor the client chat automatically, regardless of the Tweaker settings.
        /// </summary>
        /// <param name="rehash">Can be set true if desiring a full client rehash and hence rebuilding the game client hash file.
        /// Else will attempt to read from past client hashes, which in turn gets compiled from the latest game client update patchlist.
        /// (A much more elegant solution instead of maintaining your own hashes!)</param>
        /// <param name="cleanLegacy">Can be set true if desiring legacy files purging.</param>
        /// <returns>True if patching is successful. False otherwise.</returns>
        public async Task Update(bool rehash, bool cleanLegacy)
        {
            var remoteVersion = Downloader.GetRemoteVersion();
            Output.OnUpdateStart(rehash);

            List<PatchInfo> patchlist;
            List<PatchInfo> missingFiles;

            if (File.Exists(MissingFilesJson))
            {
                missingFiles = await ResumePatching();
                patchlist = await ReadPatchlistFromJson();
            }
            else
            {
                var patchlistDownload = Downloader.FetchUpdatePatchlist();

                await RestoreBackupFiles();
                var gameFiles = await Task.Run(() => ListGameFiles());
                patchlist = await patchlistDownload;

                if (cleanLegacy)
                {
                    await CleanLegacyFiles(patchlist, gameFiles);
                }
                var gameHashes = await GetClientHash(rehash);

                missingFiles = await DiscoverMissingPatches(gameHashes, patchlist);
                if (File.Exists(DownloadSuccessLog))
                {
                    File.Delete(DownloadSuccessLog);
                }
            }

            if (missingFiles.Count > 0)
            {
                int failCount = await TryUpdateGame(missingFiles);
                if (failCount > 0)
                {
                    // Uh-oh, one or more download failed
                    Output.OnPatchingFailed(failCount);
                    return;
                }

                Output.AppendLog("Saving the latest client hashes...");
                await OverwriteLatestClientJson(patchlist.ToDictionary(Q => Q.File, Q => Q.Hash));
                Output.OnPatchingSuccess();
                await Housekeeping();
            }
            else
            {
                Output.IfUpdateNotNeeded();
                await Housekeeping();
            }

            Settings.GameVersion = await remoteVersion;
            Output.OnUpdateCompleted();
        }

        /// <summary>
        /// Checks if game client version equals to the remote update version.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsGameUpToDate()
        {
            return await Downloader.GetRemoteVersion() == Settings.GameVersion;
        }

        /// <summary>
        /// Apparently, there are many files that are not listed in the patchlist.
        /// I assume these are legacy files that are not needed anymore and can be safely deleted!
        /// My game client was updated ever since Episode 2: using this method weeds out 1300+ files (1GB).
        /// </summary>
        /// <returns></returns>
        private async Task CleanLegacyFiles(List<PatchInfo> patchlist, List<string> gameFiles)
        {
            Output.OnLegacyFilesScanning();

            var t = Stopwatch.StartNew();

            var requiredFiles = patchlist.AsParallel().Select(Q =>
            {
                var shortpath = Q.File.Replace('/', '\\');
                return Path.Combine(Settings.GameDirectory, shortpath);
            });

            // Only delete files in /data/win32 for safety. Prevents deleting weird stuffs like GameGuard or Tweaker stuffs.
            // EnumerateGameFiles is only sending out files in /data/win32 and several whitelisted files in pso2_bin.

            var legacyFiles = gameFiles
                .AsParallel()
                .Except(requiredFiles)
                .ToList();

            Output.Benchmark(t, "Scan legacy files");

            if (legacyFiles.Any() == false)
            {
                Output.OnLegacyFilesNotFound();
                return;
            }

            Output.OnLegacyFilesFound(legacyFiles);
            await Task.Run(() =>
            {
                foreach (var file in legacyFiles)
                {
                    File.Delete(file);
                }
            });

            Output.Benchmark(t, "Destroy legacy files");
        }
        /// <summary>
        /// Compares the patchlist to the game client hashes and discover whether there are missing files that require downloading.
        /// </summary>
        /// <param name="gameFiles"></param>
        /// <param name="patchlist"></param>
        /// <returns></returns>
        private async Task<List<PatchInfo>> DiscoverMissingPatches(Dictionary<string, string> gameFiles, List<PatchInfo> patchlist)
        {
            var t = Stopwatch.StartNew();
            var missing = new ConcurrentBag<PatchInfo>();

            Parallel.ForEach(patchlist, patch =>
            {
                string hash;

                var isFound = gameFiles.TryGetValue(patch.File, out hash);
                if (isFound == false)
                {
                    missing.Add(patch);
                    Output.AppendLog($"File {patch.File} is missing.");
                    return;
                }

                if (string.IsNullOrEmpty(hash))
                {
                    missing.Add(patch);
                    Output.AppendLog($"File {patch.File} has null or empty hash?!?");
                    return;
                }

                var isEqual = hash.Equals(patch.Hash, StringComparison.InvariantCultureIgnoreCase);
                if (isEqual == false)
                {
                    missing.Add(patch);
                    Output.AppendLog($"File {patch.File} hash mismatched; expected: {patch.Hash}, you have: {hash}.");
                    return;
                }
            });

            var result = missing.ToList();
            Output.OnMissingFilesDiscovery(result.Select(Q => Q.File));
            await WriteMissingFilesToJson(result);

            Output.Benchmark(t, "Discover missing files");
            return result;
        }

        /// <summary>
        /// Returns a list of all game files.
        /// </summary>
        /// <returns></returns>
        private List<string> ListGameFiles()
        {
            var t = Stopwatch.StartNew();

            var gameFiles = new List<string>(70000);

            var dataFiles = Directory.EnumerateFiles(Settings.DataWin32Directory(), "*.*", SearchOption.TopDirectoryOnly).ToList();
            gameFiles.AddRange(dataFiles);

            var licenseFiles = Directory.EnumerateFiles(Settings.LicenseDirectory(), "*.*", SearchOption.TopDirectoryOnly);
            gameFiles.AddRange(licenseFiles);

            var whitelist = new[] { "pso2.exe", "pso2launcher.exe", "pso2updater.exe",
                "pso2download.exe", "pso2predownload.exe", "gameversion.ver", "edition.txt",
                "D3dx9d_43.dll", "D3dx9d_42.dll", "cudart32_30_9.dll",
                "sdkencryptedappticket.dll", "sdkencryptedappticket64.dll"
            };

            foreach (var file in whitelist)
            {
                var path = Path.Combine(Settings.GameDirectory, file);
                if (File.Exists(path))
                {
                    gameFiles.Add(path);
                }
            }

            Output.Benchmark(t, "Enumerate game files");
            return gameFiles;
        }

        /// <summary>
        /// Calculate a hard disk buffer size. Buffer size block gets increased by multiply of 2, starting from 4KB and up to 4MB.
        /// </summary>
        /// <param name="length">File size in bytes.</param>
        /// <returns></returns>
        private int GetBufferSize(long length)
        {
            var maxbuffer = 4 * 1024 * 1024;

            // Default stream buffer size is 4KB
            if (length < 4096) return 4096;

            if (length < maxbuffer)
            {
                // 3000 KB file = 3072000 bytes
                // pow = 22
                var pow = Math.Ceiling(Math.Log(length, 2));

                // 2^16 = 4194304 bytes = 4096 KB
                return (int)Math.Pow(2, pow);
            }

            // Most hard drives have 16MB or 32MB buffer size.
            // We'll take 4MB off that for hashing larger files!
            return maxbuffer;
        }

        /// <summary>
        /// Attempts to obtain client hashes, depending on required operation and whether the past hashes are available for read.
        /// </summary>
        /// <param name="rehash"></param>
        /// <returns></returns>
        private async Task<Dictionary<string, string>> GetClientHash(bool rehash)
        {
            if (rehash) return await RehashWholeClient();

            try
            {
                return await ReadLatestClientHash();
            }
            catch
            {
                Output.OnClientHashReadFailed();
                return await RehashWholeClient();
            }
        }

        /// <summary>
        /// Create a named JSON file containing file-MD5 hashes dictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        private async Task OverwriteLatestClientJson(Dictionary<string, string> dictionary)
        {
            using (var file = File.CreateText(ClientHashesJson))
            {
                await file.WriteAsync(JsonConvert.SerializeObject(dictionary));
            }
        }

        /// <summary>
        /// Returns a sequence of all files that were successfully downloaded in prior game client update attempt.
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> ReadDownloadedFilesFromLog()
        {
            if (File.Exists(DownloadSuccessLog) == false) return new List<string>();

            using (var file = File.OpenText(DownloadSuccessLog))
            {
                return (await file.ReadToEndAsync()).Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        /// <summary>
        /// Read latest game client hash dictionary.
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<string, string>> ReadLatestClientHash()
        {
            using (var file = File.OpenText(ClientHashesJson))
            {
                var client = await file.ReadToEndAsync();
                var hashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(client);

                Output.OnClientHashReadSuccess();
                return hashes;
            }
        }

        /// <summary>
        /// Read the sequence of missing files from the named JSON file.
        /// </summary>
        /// <returns></returns>
        private async Task<List<PatchInfo>> ReadMissingFilesFromJson()
        {
            using (var file = File.OpenText(MissingFilesJson))
            {
                var json = await file.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<PatchInfo>>(json);
            }
        }

        /// <summary>
        /// Read the sequence of patch information from the named JSON file.
        /// </summary>
        /// <returns></returns>
        private async Task<List<PatchInfo>> ReadPatchlistFromJson()
        {
            //Output.WriteLine("Reading last downloaded patchlist...");
            using (var file = File.OpenText(PatchlistJson))
            {
                var json = await file.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<PatchInfo>>(json);
            }
        }

        /// <summary>
        /// Perform a (fast) whole client rehash.
        /// When done, save the hashes into the named JSON file and return the result.
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<string, string>> RehashWholeClient()
        {
            var game = await Task.Run(() =>
            {
                // Random shuffle because YOLO.
                return ListGameFiles().OrderBy(Q => Guid.NewGuid()).Select(Q => new HashModel
                {
                    FileName = Q
                }).ToList();
            });

            var total = game.Count;
            Output.OnHashStart(game.Select(Q => Q.FileName));

            // Perform the long-running operation in the background thread instead of the UI thread.
            var progress = 0;
            var streamJob = Task.Run(() =>
            {
                var t = Stopwatch.StartNew();

                // Read the file sequentially, storing the content in-memory.
                // Then pass the content to a background process, where the hashes will be computed.
                // Because reference to the binary array is not being stored, GC will release the memory as needed.
                // Much more efficient because delays caused by hash workloads are offloaded to background process,
                // While the files are being read non-stop!
                foreach (var file in game)
                {
                    var binary = File.ReadAllBytes(file.FileName);
                    file.ComputeHash(binary, Settings.GameDirectory);

                    progress++;
                }

                Output.Benchmark(t, "Read files to hash");
            });

            var lastProgress = 0;
            do
            {
                await Task.Delay(500);
                if (progress > lastProgress)
                {
                    lastProgress = progress;
                    Output.OnHashProgress(lastProgress, total);
                }
            } while (lastProgress < total);

            await streamJob;
            var computeJobs = game.Select(Q => Q.ComputeTask);
            await Task.WhenAll(computeJobs);

            var hashes = game.ToDictionary(Q => Q.Key, Q => Q.Hash);
            Output.OnHashComplete();

            await OverwriteLatestClientJson(hashes);
            return hashes;
        }

        /// <summary>
        /// Read missing files again prior to update interruptions / failure and then read the files that were successfully downloaded previously.
        /// Update the missing files using this information and then return it.
        /// </summary>
        /// <returns></returns>
        private async Task<List<PatchInfo>> ResumePatching()
        {
            var missingFiles = await ReadMissingFilesFromJson();
            var downloaded = new HashSet<string>(await ReadDownloadedFilesFromLog());

            if (downloaded.Any())
            {
                missingFiles = missingFiles.Where(Q => downloaded.Contains(Q.File) == false).ToList();
            }

            Output.OnPatchingResume(missingFiles.Select(Q => Q.File));
            return missingFiles;
        }

        /// <summary>
        /// Attempts to recover the missing files from remote update server or local update repository.
        /// </summary>
        /// <param name="missingFiles">List of missing files</param>
        /// <returns>How many files failed to download</returns>
        private async Task<int> TryUpdateGame(List<PatchInfo> missingFiles)
        {
            using (var doneLog = File.AppendText(DownloadSuccessLog))
            {
                // TODO: implement sideloading prepatch files here.
                Output.OnPatchingStart(missingFiles.Count);

                var downloads = missingFiles.Select(patch => Downloader.DownloadGamePatch(patch, Settings.GameDirectory, doneLog)).ToList();
                return (await Task.WhenAll(downloads)).Count(Q => Q == false);
            }
        }
        /// <summary>
        /// Write the sequence of missing files to the named JSON file.
        /// </summary>
        /// <param name="missingFiles"></param>
        /// <returns></returns>
        private async Task WriteMissingFilesToJson(List<PatchInfo> missingFiles)
        {
            if (missingFiles.Any())
            {
                using (var file = File.CreateText(MissingFilesJson))
                {
                    await file.WriteAsync(JsonConvert.SerializeObject(missingFiles));
                }
            }
        }
    }
}
