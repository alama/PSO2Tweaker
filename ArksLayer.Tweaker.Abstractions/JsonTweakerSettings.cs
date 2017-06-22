using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// A class for managing Tweaker settings using JSON file.
    /// </summary>
    public class JsonTweakerSettings : ITweakerSettings
    {
        /// <summary>
        /// Shared lock object for the configuration file.
        /// </summary>
        public static object ConfigurationLock = new object();

        private JObject Configuration;
        private FileSystemWatcher Watch;
        private readonly string FilePath;

        /// <summary>
        /// Returns a default Tweaker JSON configuration file path, which is located in %appdata%\PSO2 Tweaker\settings.json 
        /// </summary>
        public static string DefaultTweakerJsonFilePath
        {
            get
            {
                var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appdata, "PSO2 Tweaker", "settings.json");
            }
        }

        /// <summary>
        /// Sets or gets value of the directory path to the English Large Patch version.
        /// </summary>
        public string EnglishLargePatchVersion
        {
            get
            {
                return Configuration["LargeFilesVersion"].Value<string>();
            }

            set
            {
                Configuration["LargeFilesVersion"] = value;
                WriteConfiguration();
            }
        }

        /// <summary>
        /// Sets or gets value of the directory path to the English Patch version.
        /// </summary>
        public string EnglishPatchVersion
        {
            get
            {
                return Configuration["ENPatchVersion"].Value<string>();
            }

            set
            {
                Configuration["ENPatchVersion"] = value;
                WriteConfiguration();
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
                return Configuration["PSO2Dir"].Value<string>();
            }

            set
            {
                Configuration["PSO2Dir"] = value;
                WriteConfiguration();
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
        /// Sets or gets value of the directory path to the Story Patch version.
        /// </summary>
        public string StoryPatchVersion
        {
            get
            {
                return Configuration["StoryPatchVersion"].Value<string>();
            }

            set
            {
                Configuration["StoryPatchVersion"] = value;
                WriteConfiguration();
            }
        }

        /// <summary>
        /// Constructs a new setting against the specified file.
        /// </summary>
        /// <param name="file"></param>
        public JsonTweakerSettings(string file)
        {
            this.FilePath = file;
            InitializeWatcher();
            ReadConfiguration();
        }

        private void InitializeWatcher()
        {
            this.Watch = new FileSystemWatcher();
            Watch.Path = Path.GetDirectoryName(FilePath);
            Watch.Filter = Path.GetFileName(FilePath);
            Watch.Created += Watch_Changed;
            Watch.Changed += Watch_Changed;

            Watch.EnableRaisingEvents = true;
        }

        private void Watch_Changed(object sender, FileSystemEventArgs e)
        {
            ReadConfiguration();
            Debug.WriteLine(JsonConvert.SerializeObject(Configuration));
        }

        private void ReadConfiguration()
        {
            lock (ConfigurationLock)
            {
                if (File.Exists(FilePath) == false)
                {
                    this.Configuration = JObject.FromObject(new TweakerJson());
                }
                else
                {
                    using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sr = new StreamReader(fs))
                    {
                        var json = sr.ReadToEnd();
                        this.Configuration = JObject.Parse(json);
                    }
                }
            }
        }

        private void WriteConfiguration()
        {
            lock (ConfigurationLock)
            {
                var dir = Path.GetDirectoryName(FilePath);
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                var json = JsonConvert.SerializeObject(Configuration, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
        }
    }
}
