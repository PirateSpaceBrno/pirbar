using Newtonsoft.Json;
using System.ComponentModel;

namespace PirBar.Server.Models
{
    internal class PirBarConfigJson
    {
        [DefaultValue("localhost")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string listenGateway { get; set; }

        [DefaultValue("8080")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int listenPort { get; set; }

        [DefaultValue("PirBar")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string instanceName { get; set; }

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string instanceDescription { get; set; }
    }
}
