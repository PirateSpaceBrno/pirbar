using Newtonsoft.Json;
using PirBanka.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PirBanka.Server.Controllers
{
    public class PirBankaConfig
    {
        private readonly ConfigJson _config;

        public PirBankaConfig()
        {
            var configJson = File.ReadAllText($"{Environment.CurrentDirectory}\\config.json");
            _config = JsonConvert.DeserializeObject<List<ConfigJson>>(configJson)[0];
        }

        public string ListenGateway => _config.listenGateway;
        public int ListenPort => _config.listenPort;
        public string Name => _config.instanceName;
        public string Description => _config.instanceDescription;
    }
}
