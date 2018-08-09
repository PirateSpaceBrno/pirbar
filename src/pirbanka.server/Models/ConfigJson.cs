using Newtonsoft.Json;
using System.ComponentModel;

namespace PirBanka.Server.Models
{
    internal class ConfigJson
    {
        [DefaultValue("localhost")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string listenGateway { get; set; }

        [DefaultValue("8080")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int listenPort { get; set; }

        [DefaultValue("PirBanka")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string instanceName { get; set; }

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string instanceDescription { get; set; }

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string databaseServer { get; set; }

        [DefaultValue("3306")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string databaseServerPort { get; set; }

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string databaseName { get; set; }

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string databaseUser { get; set; }

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string databasePassword { get; set; }
    }
}
