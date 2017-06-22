using ArksLayer.Tweaker.Abstractions;
using Newtonsoft.Json;
using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.UpdateEngine
{
    /// <summary>
    /// Contains methods for downloading and applying English fan patches.
    /// </summary>
    public class FanPatcher
    {
        private readonly ITweakerSettings Settings;
        private readonly ITrigger Triggers;

        /// <summary>
        /// Provides supported patch to be downloaded.
        /// </summary>
        public enum FanPatchType
        {
            /// <summary>
            /// Causes patched files to be backed up to English Patch backup folder and version number updated accordingly.
            /// </summary>
            EnglishPatch,

            /// <summary>
            /// Causes patched files to be backed up to Large Files backup folder and version number updated accordingly.
            /// </summary>
            EnglishLargePatch
        }

        /// <summary>
        /// Constructs an instance of FanPatcher using given Tweaker settings and output triggers.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="triggers"></param>
        public FanPatcher(ITweakerSettings settings, ITrigger triggers)
        {
            this.Settings = settings;
            this.Triggers = triggers;
        }

        /// <summary>
        /// Windows NT DNS resolution file path.
        /// </summary>
        public string HostPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts");
            }
        }

        /// <summary>
        /// Enable Telepipe Proxy using JSON file downloaded from a configuration URL and an option to update Windows Host while at it.
        /// </summary>
        /// <param name="jsonUrl"></param>
        /// <param name="updateWindowsHost"></param>
        /// <returns></returns>
        public async Task EnableTelepipeProxy(string jsonUrl, bool updateWindowsHost = false)
        {
            Triggers.OnTelepipeProxyEnabling();

            var telepipe = await DownloadTelepipeJson(jsonUrl);
            var blob = await DownloadPatch(telepipe.PublicKeyUrl);

            await Task.Run(() =>
            {
                var blobPath = Path.Combine(Settings.GameDirectory, "publickey.blob");
                if (File.Exists(blobPath))
                {
                    File.SetAttributes(blobPath, FileAttributes.Normal);
                }
                File.WriteAllBytes(blobPath, blob);
                File.SetAttributes(blobPath, FileAttributes.ReadOnly);

                var proxyPath = Path.Combine(Settings.GameDirectory, "proxy.txt");
                if (File.Exists(proxyPath))
                {
                    File.SetAttributes(proxyPath, FileAttributes.Normal);
                }
                File.WriteAllText(proxyPath, telepipe.Host);
                File.SetAttributes(proxyPath, FileAttributes.ReadOnly);
            });

            if (updateWindowsHost)
            {
                await Task.Run(() =>
                {
                    RemoveWindowsHostShipEntries();
                    AddWindowsHostShipEntries(telepipe.Host);
                });
            }

            Triggers.OnTelepipeProxyEnabled(telepipe.Name);
        }

        /// <summary>
        /// Resolves PSO2 Server DNS to Telepipe Proxy in Windows Host file.
        /// </summary>
        /// <param name="proxyUrl"></param>
        private void AddWindowsHostShipEntries(string proxyUrl)
        {
            var ships = GetShipLists();

            using (StreamWriter append = File.AppendText(HostPath))
            {
                foreach (var ship in ships)
                {
                    append.WriteLine($"{proxyUrl} {ship.Value} #{ship.Key}");
                }
            }
        }

        /// <summary>
        /// Remove PSO2 Server DNS from local Windows Host File.
        /// </summary>
        public void RemoveWindowsHostShipEntries()
        {
            var servers = new HashSet<string>(GetShipLists().Select(Q => Q.Value));
            var current = File.ReadAllLines(HostPath).ToList();

            var commit = new List<string>();

            foreach (var entry in current)
            {
                var exclude = false;
                foreach (var server in servers)
                {
                    if (entry.Contains(server))
                    {
                        exclude = true;
                        break;
                    }
                }
                if (!exclude)
                {
                    commit.Add(entry);
                }
            }

            File.WriteAllLines(HostPath, commit);
        }

        /// <summary>
        /// Returns a list of PSO2 Servers (Ships) and their URLs.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetShipLists()
        {
            var ships = new Dictionary<string, string>();
            ships.Add("Ship 1", "gs001.pso2gs.net");
            ships.Add("Ship 2", "gs016.pso2gs.net");
            ships.Add("Ship 3", "gs031.pso2gs.net");
            ships.Add("Ship 4", "gs046.pso2gs.net");
            ships.Add("Ship 5", "gs061.pso2gs.net");
            ships.Add("Ship 6", "gs076.pso2gs.net");
            ships.Add("Ship 7", "gs091.pso2gs.net");
            ships.Add("Ship 8", "gs106.pso2gs.net");
            ships.Add("Ship 9", "gs121.pso2gs.net");
            ships.Add("Ship 10", "gs136.pso2gs.net");

            return ships;
        }

        /// <summary>
        /// Attempts to apply a patch of given type from a given URL or File Path via simple zip extraction.
        /// </summary>
        /// <param name="patchType"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task ApplyPatch(FanPatchType patchType, string path)
        {
            var patchName = GetPatchTypeName(patchType);

            Stream buffer = await TryObtainFile(path);
            if (buffer == null)
            {
                Triggers.OnFanPatchNotFound();
                return;
            }

            Triggers.OnFanPatching(patchType.ToString());

            await Task.Run(() =>
            {
                var backupFolder = Path.Combine(Settings.BackupDirectory(), patchName);
                if (Directory.Exists(backupFolder) == false)
                {
                    Directory.CreateDirectory(backupFolder);
                }

                using (var archive = ArchiveFactory.Open(buffer))
                {
                    var reader = archive.ExtractAllEntries();
                    while (reader.MoveToNextEntry())
                    {
                        if (reader.Entry.IsDirectory) continue;

                        var name = Path.GetFileName(reader.Entry.Key);
                        var target = Path.Combine(Settings.DataWin32Directory(), name);
                        if (File.Exists(target))
                        {
                            var backup = Path.Combine(backupFolder, name);
                            File.Move(target, backup);
                        }

                        using (var file = new FileStream(target, FileMode.Create))
                        {
                            reader.WriteEntryTo(file);
                        }
                    }
                }
                buffer.Dispose();
            });

            if (patchType == FanPatchType.EnglishPatch)
            {
                Settings.EnglishPatchVersion = Path.GetFileNameWithoutExtension(path);
            }
            if (patchType == FanPatchType.EnglishLargePatch)
            {
                Settings.EnglishLargePatchVersion = Path.GetFileNameWithoutExtension(path);
            }

            Triggers.OnFanPatchSuccessful(patchName);
        }

        /// <summary>
        /// Returns appropriate backup folder / patch name for the given patch type, based on existing folder names.
        /// </summary>
        /// <param name="patchType"></param>
        /// <returns></returns>
        private string GetPatchTypeName(FanPatchType patchType)
        {
            switch (patchType)
            {
                case FanPatchType.EnglishPatch:
                    return "English Patch";
                case FanPatchType.EnglishLargePatch:
                    return "Large Files";
                default:
                    throw new NotImplementedException("Fan patch type not supported.");
            }
        }

        /// <summary>
        /// Either obtain a fan patch file from a URL or a File System.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task<Stream> TryObtainFile(string path)
        {
            Uri uriResult;
            bool isUrl = Uri.TryCreate(path, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (isUrl)
            {
                var binary = await DownloadPatch(uriResult.AbsoluteUri);
                return new MemoryStream(binary);
            }
            if (File.Exists(path))
            {
                return new FileStream(path, FileMode.Open);
            }

            return null;
        }

        /// <summary>
        /// Attempts to download a binary from URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<byte[]> DownloadPatch(string url)
        {
            var client = new WebClient();
            Triggers.OnDownloadStart(url, client);
            var binary = await client.DownloadDataTaskAsync(url);
            Triggers.OnDownloadFinish(url);

            return binary;
        }

        /// <summary>
        /// Attempts to download a binary from URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<TelepipeSettings> DownloadTelepipeJson(string url)
        {
            var client = new WebClient();
            Triggers.OnDownloadStart(url, client);
            var json = await client.DownloadStringTaskAsync(url);
            Triggers.OnDownloadFinish(url);

            return JsonConvert.DeserializeObject<TelepipeSettings>(json);
        }
    }
}
