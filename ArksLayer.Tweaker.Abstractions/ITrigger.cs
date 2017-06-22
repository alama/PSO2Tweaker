using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// This interface is what enables the UpdateEngine to communicate with your UI.
    /// See ArksLayer.Tweaker.Terminal.ConsoleTrigger for example on how best to implement this.
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// Called when the library wants to log something. 
        /// This method may run in a separated thread, implement with caution.
        /// </summary>
        /// <param name="s"></param>
        void AppendLog(string s);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if no update to the game client is needed.
        /// </summary>
        void IfUpdateNotNeeded();

        /// <summary>
        /// This method will be called once per UpdateEngine.Update when a backup restore is going to be performed.
        /// Ideal implementation should also log the backup files.
        /// </summary>
        /// <param name="backupFiles"></param>
        void OnBackupRestore(IEnumerable<string> backupFiles);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if previous client hash failed to be read.
        /// </summary>
        void OnClientHashReadFailed();

        /// <summary>
        /// Will be called when a fan translation patch process is started.
        /// </summary>
        /// <param name="name"></param>
        void OnFanPatching(string name);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if previous client hash successfully read.
        /// </summary>
        void OnClientHashReadSuccess();

        /// <summary>
        /// This method will be called by the UpdateEngine.Update for every downloads that completely failed after certain amount of retries.
        /// Ideal implementation should also destroy the progress bar for the download.
        /// </summary>
        /// <param name="url"></param>
        void OnDownloadAborted(string url);

        /// <summary>
        /// This method will be called when user attempted to configure Telepipe Proxy from a JSON file.
        /// </summary>
        void OnTelepipeProxyEnabling();

        /// <summary>
        /// This method will be called by the UpdateEngine.Update for every downloads that completely failed after certain amount of retries.
        /// Ideal implementation should also destroy the progress bar for the download.
        /// </summary>
        /// <param name="url"></param>
        void OnDownloadFinish(string url);

        /// <summary>
        /// This method will be called by the UpdateEngine.Update for every downloads that completely failed but will be retried.
        /// Ideal implementation should also destroy the progress bar for the download.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="delaySecond"></param>
        void OnDownloadRetry(string url, int delaySecond);


        /// <summary>
        /// This method will be called when user successfully configured Telepipe Proxy from a JSON file.
        /// </summary>
        void OnTelepipeProxyEnabled(string name);

        /// <summary>
        /// Will be called when a fan translation patch process is completed.
        /// </summary>
        void OnFanPatchSuccessful(string name);

        /// <summary>
        /// This method will be called by the UpdateEngine.Update for every downloads that was started.
        /// Ideal implementation should summon a progress bar for the download.
        /// Download progress can be tracked using the WebClient.OnDownloadProgressChanged event handler.
        /// Download can be cancelled using WebClient.CancelAsync.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="client"></param>
        void OnDownloadStart(string url, WebClient client);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if whole game client hashing finished.
        /// Ideal implementation should also destroy the hashing progress bar.
        /// </summary>
        void OnHashComplete();

        /// <summary>
        /// This method will be called every second during UpdateEngine.Update whole client hash progress.
        /// Ideal implementation should visualize the progress with preferrably a progress bar.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="total"></param>
        void OnHashProgress(int progress, int total);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update when starting a whole client hashing.
        /// Ideal implementation should also log the files to be hashed.
        /// </summary>
        /// <param name="files"></param>
        void OnHashStart(IEnumerable<string> files);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update when housekeeping is started.
        /// </summary>
        void OnHousekeeping();

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if legacy files are found and to be deleted.
        /// Ideal implementation should also log the legacy files.
        /// </summary>
        /// <param name="legacyFiles"></param>
        void OnLegacyFilesFound(IEnumerable<string> legacyFiles);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if legacy files are not found.
        /// </summary>
        void OnLegacyFilesNotFound();

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if a scan for legacy files is to be started.
        /// </summary>
        void OnLegacyFilesScanning();

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if there are missing or corrupted files in the game client.
        /// Ideal implementation should also log the missing files.
        /// </summary>
        /// <param name="missingFiles"></param>
        void OnMissingFilesDiscovery(IEnumerable<string> missingFiles);

        /// <summary>
        /// This method will be called when a fan patch file cannot be found from given URL / File Path.
        /// </summary>
        void OnFanPatchNotFound();

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if there are one or more game patch downloads that failed.
        /// </summary>
        /// <param name="failCount"></param>
        void OnPatchingFailed(int failCount);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if there are patch downloads to be resumed.
        /// Ideal implementation may also log the missing files.
        /// </summary>
        /// <param name="missingFiles"></param>
        void OnPatchingResume(IEnumerable<string> missingFiles);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if there are patch files to be downloaded.
        /// </summary>
        /// <param name="fileCount"></param>
        void OnPatchingStart(int fileCount);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update if all patches were successfully downloaded.
        /// </summary>
        void OnPatchingSuccess();

        /// <summary>
        /// This method will be called once per UpdateEngine.Update when patchlists are successfully downloaded and merged.
        /// </summary>
        /// <param name="count"></param>
        void OnPatchlistFetchCompleted(int count);

        /// <summary>
        /// This method will be called once per UpdateEngine.Update when patchlists are to be downloaded (and merged).
        /// </summary>
        void OnPatchlistFetchStart();

        /// <summary>
        /// This method will be called once per UpdateEngine.Update when it's done.
        /// </summary>
        void OnUpdateCompleted();

        /// <summary>
        /// This method will be called once per UpdateEngine.Update when it's starting.
        /// </summary>
        /// <param name="rehash"></param>
        void OnUpdateStart(bool rehash);
    }
}