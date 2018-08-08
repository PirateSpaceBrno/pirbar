using Newtonsoft.Json;
using PirBar.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PirBar.Server.Controllers
{
    public class PirBarConfig
    {
        private readonly PirBarConfigJson _config;

        public PirBarConfig()
        {
            var configJson = File.ReadAllText($"{Environment.CurrentDirectory}\\config.json");
            _config = JsonConvert.DeserializeObject<List<PirBarConfigJson>>(configJson)[0];
        }

        public string ListenGateway => _config.listenGateway;
        public int ListenPort => _config.listenPort;
        public string Name => _config.instanceName;
        public string Description => _config.instanceDescription;
    }
}
