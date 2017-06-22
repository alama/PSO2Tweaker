using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// Implementation of Tweaker Settings interface against Windows Registry.
    /// </summary>
    public class RegistryTweakerSettings : ITweakerSettings
    {
        /// <summary>
        /// Registry key for value containing the version of English Large Patch.
        /// </summary>
        private const string EnglishLargePatchVersionKey = "LargeFilesVersion";

        /// <summary>
        /// Registry key for value containing the version of English Patch.
        /// </summary>
        private const string EnglishPatchVersionKey = "ENPatchVersion";

        /// <summary>
        /// Registry key for value containing directory path to pso2_bin.
        /// </summary>
        private const string GameDirectoryKey = "PSO2Dir";

        /// <summary>
        /// Registry key for value containing the version of Story Patch.
        /// </summary>
        private const string StoryPatchVersionKey = "StoryPatchVersion";

        /// <summary>
        /// Constructs an instance of the class, pointing to parameter registry path for HKEY_CURRENT_USER.
        /// </summary>
        /// <param name="path"></param>
        public RegistryTweakerSettings(string path = @"Software\AIDA")
        {
            this.Root = Registry.CurrentUser.CreateSubKey(path);
        }
        /// <summary>
        /// Sets or gets value of the directory path to the English Large Patch version.
        /// </summary>
        public string EnglishLargePatchVersion
        {
            get
            {
                return (string)Root.GetValue(EnglishLargePatchVersionKey);
            }
            set
            {
                Root.SetValue(EnglishLargePatchVersionKey, value.Trim());
            }
        }

        /// <summary>
        /// Sets or gets value of the directory path to the English Patch version.
        /// </summary>
        public string EnglishPatchVersion
        {
            get
            {
                return (string)Root.GetValue(EnglishPatchVersionKey);
            }
            set
            {
                Root.SetValue(EnglishPatchVersionKey, value.Trim());
            }
        }

        /// <summary>
        /// Sets or gets value of the directory path to pso2_bin.
        /// Expected result example: D:\PSO2\pso2_bin
        /// </summary>
        public string GameDirectory
        {
            get
            {
                var value = (string)Root.GetValue(GameDirectoryKey);
                return value.TrimEnd('\\');
            }
            set
            {
                Root.SetValue(GameDirectoryKey, value.TrimEnd('\\'));
            }
        }

        /// <summary>
        /// Sets or gets the latest game client version.
        /// </summary>
        public string GameVersion
        {
            get
            {
                return this.GetGameVersion();
            }

            set
            {
                this.SetGameVersion(value);
            }
        }

        /// <summary>
        /// Using this property, you can manipulate Windows Registry for Tweaker directly.
        /// </summary>
        public RegistryKey Root { get; private set; }

        /// <summary>
        /// Sets or gets value of the directory path to the Story Patch version.
        /// </summary>
        public string StoryPatchVersion
        {
            get
            {
                return (string)Root.GetValue(StoryPatchVersionKey);
            }
            set
            {
                Root.SetValue(StoryPatchVersionKey, value.Trim());
            }
        }
    }
}
