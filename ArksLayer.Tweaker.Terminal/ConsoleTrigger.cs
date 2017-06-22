using System;
using System.Collections.Generic;
using System.Net;
using ArksLayer.Tweaker.Abstractions;
using System.Linq;

namespace ArksLayer.Tweaker.Terminal
{
    internal class ConsoleTrigger : ITrigger
    {
        public void AppendLog(string s)
        {
            Console.WriteLine(s);
        }

        public void IfUpdateNotNeeded()
        {
            Console.WriteLine("Your game is already up-to-date!");
        }

        public void OnBackupRestore(IEnumerable<string> backupFiles)
        {
            Console.WriteLine($"Found {backupFiles.Count()} backup files. Restoring them...");
        }

        public void OnClientHashReadFailed()
        {
            Console.WriteLine("Failed to fetch latest client game hashes.");
        }

        public void OnClientHashReadSuccess()
        {
            Console.WriteLine("Obtained latest client game hashes.");
        }

        public void OnDownloadAborted(string url)
        {
            Console.WriteLine($"FAILED {url}");
        }

        public void OnDownloadFinish(string url)
        {
            Console.WriteLine($"DOWNLOADED {url}");
        }

        public void OnDownloadRetry(string url, int delaySecond)
        {
            Console.WriteLine($"Retrying {url} in {delaySecond} seconds...");
        }

        public void OnDownloadStart(string url, WebClient client)
        {
            Console.WriteLine($"GET {url}");

            client.DownloadProgressChanged += (o, e) =>
            {
                Console.WriteLine($"DOWNLOADING {url} {e.ProgressPercentage}% of {Math.Floor(e.TotalBytesToReceive / 1024.0)}KB");
            };
        }

        public void OnFanPatching(string name)
        {
            Console.Write("Installing " + name);
        }

        public void OnFanPatchNotFound()
        {
            Console.Write("Cannot find patch zip at given path / URL.");
        }

        public void OnFanPatchSuccessful(string name)
        {
            Console.WriteLine("Successfully installed " + name);
        }

        public void OnHashComplete()
        {
            Console.WriteLine("Game hashing successful!");
        }

        public void OnHashProgress(int progress, int total)
        {
            Console.WriteLine($"Hashed {progress} files out of {total}");
        }

        public void OnHashStart(IEnumerable<string> files)
        {
            Console.WriteLine($"Generating MD5 checksum for {files.Count()} files...");
        }

        public void OnHousekeeping()
        {
            Console.WriteLine("A little housekeeping...");
        }

        public void OnLegacyFilesFound(IEnumerable<string> legacyFiles)
        {
            Console.WriteLine($"Destroying {legacyFiles.Count()} legacy files...");
            foreach (var file in legacyFiles)
            {
                Console.WriteLine("LEGACY :" + file);
            }
        }

        public void OnLegacyFilesNotFound()
        {
            Console.WriteLine("No legacy files found.");
        }

        public void OnLegacyFilesScanning()
        {
            Console.WriteLine("Searching for legacy files...");
        }

        public void OnMissingFilesDiscovery(IEnumerable<string> missingFiles)
        {
            Console.WriteLine($"Discovered {missingFiles.Count()} missing or changed files.");
        }

        public void OnPatchingFailed(int failCount)
        {
            Console.WriteLine($"Failed to download {failCount} files!");
        }

        public void OnPatchingResume(IEnumerable<string> missingFiles)
        {
            Console.WriteLine($"Resuming patching {missingFiles.Count()} files!");
        }

        public void OnPatchingStart(int fileCount)
        {
            Console.WriteLine($"Downloading {fileCount} files.");
        }

        public void OnPatchingSuccess()
        {
            Console.WriteLine("Successfully downloaded all files!");
        }

        public void OnPatchlistFetchCompleted(int count)
        {
            Console.WriteLine($"Patchlist contains {count} entries.");
        }

        public void OnPatchlistFetchStart()
        {
            Console.WriteLine("Downloading patchlist...");
        }

        public void OnTelepipeProxyEnabled(string name)
        {
            Console.WriteLine($"You can now connect to {name}!");
        }

        public void OnTelepipeProxyEnabling()
        {
            Console.WriteLine("Downloading and applying PSO2 Proxy...");
        }

        public void OnUpdateCompleted()
        {
            Console.WriteLine("Update finished.");
        }

        public void OnUpdateStart(bool rehash)
        {
            Console.WriteLine("Update started.");
        }
    }
}