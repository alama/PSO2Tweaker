using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// Extension method for common values obtainable from ITweakerSettings.
    /// </summary>
    public static class TweakerSettingsExtensions
    {
        /// <summary>
        /// Gets the known, usual Tweaker backup directory.
        /// </summary>
        public static string BackupDirectory(this ITweakerSettings settings)
        {
            return Path.Combine(settings.DataWin32Directory(), "backup");
        }

        /// <summary>
        /// Gets the game file directory where the game asset files usually located in.
        /// </summary>
        public static string DataWin32Directory(this ITweakerSettings settings)
        {
            return Path.Combine(settings.GameDirectory, @"data\win32");
        }

        /// <summary>
        /// Gets the game file directory where the game asset files usually located in.
        /// </summary>
        public static string LicenseDirectory(this ITweakerSettings settings)
        {
            return Path.Combine(settings.GameDirectory, @"data\license");
        }

#pragma warning disable RECS0154 // Parameter is never used
        /// <summary>
        /// Gets the value of the PSO2 user profile folder.
        /// </summary>
        public static string GetUserFolderPath(this ITweakerSettings settings)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documents, @"SEGA\PHANTASYSTARONLINE2");
        }
#pragma warning restore RECS0154 // Parameter is never used

        /// <summary>
        /// Gets the path of the game version file.
        /// </summary>
        public static string GetGameVersionFilePath(this ITweakerSettings settings)
        {
            return Path.Combine(settings.GetUserFolderPath(), "version.ver");
        }

        /// <summary>
        /// A lock for thread-safe game version file IO.
        /// </summary>
        private static object GameVersionFileLock = new object();

        /// <summary>
        /// Gets the value of the game client version.
        /// </summary>
        public static string GetGameVersion(this ITweakerSettings settings)
        {
            lock (GameVersionFileLock)
            {
                var path = settings.GetGameVersionFilePath();
                if (!File.Exists(path))
                {
                    return null;
                }
                return File.ReadAllText(path);
            }
        }

        /// <summary>
        /// Sets the value of the game client version.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="version"></param>
        public static void SetGameVersion(this ITweakerSettings settings, string version)
        {
            lock (GameVersionFileLock)
            {
                var path = settings.GetGameVersionFilePath();
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(path, version);
            }
        }
    }
}
