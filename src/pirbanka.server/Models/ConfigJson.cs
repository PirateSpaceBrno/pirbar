using Newtonsoft.Json;

namespace PirBanka.Server.Models
{
    internal class ConfigJson
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string listenGateway { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int listenPort { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string instanceName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string instanceDescription { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string databaseServer { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string databaseServerPort { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string databaseName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string databaseUser { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string databasePassword { get; set; }
    }
}
