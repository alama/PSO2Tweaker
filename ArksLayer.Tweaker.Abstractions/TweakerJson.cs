using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Abstractions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TweakerJson
    {
        public string Backup { set; get; }

        public string DeletedParserOnce { set; get; }

        public string ENPatchVersion { set; get; } = "Not Installed";

        public string EnableBeta { set; get; }

        public string GNFieldName { set; get; }

        public string JustPrepatched { set; get; }

        public string LargeFilesVersion { set; get; } = "Not Installed";

        public string LatestStoryBase { set; get; }

        public string Locale { set; get; }

        public string NewENVersion { set; get; }

        public string NewLargeFilesVersion { set; get; }

        public string OldX { set; get; }

        public string OldY { set; get; }

        public string Pastebin { set; get; }

        public string PluginsEnabled { set; get; }

        public string PreDownloadedRar { set; get; }

        public string ProxyJSONURL { set; get; }

        public string PSO2Dir { set; get; } = @"E:\PSO2\pso2_bin";

        public string PSO2PrecedeVersion { set; get; }

        public string PSO2RemoteVersion { set; get; }

        public string ShipStatus { set; get; }

        public string SteamMode { set; get; }

        public string StoryPatchVersion { set; get; } = "Not Installed";

        public string UseIcsHost { set; get; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
