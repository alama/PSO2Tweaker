using Newtonsoft.Json;

namespace ArksLayer.Tweaker.UpdateEngine
{
    internal class TelepipeSettings
    {
        [JsonProperty(PropertyName = "host")]
        public string Host { set; get; }

        [JsonProperty(PropertyName = "version")]
        public string Version { set; get; }

        [JsonProperty(PropertyName = "name")]
        public string Name { set; get; }

        [JsonProperty(PropertyName = "publickeyurl")]
        public string PublicKeyUrl { set; get; }
    }
}