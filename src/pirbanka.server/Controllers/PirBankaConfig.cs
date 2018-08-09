using Newtonsoft.Json;
using PirBanka.Server.Models;
using PirBanka.Server.Controllers;
using System;
using System.Collections.Generic;
using System.IO;

namespace PirBanka.Server.Controllers
{
    public class PirBankaConfig
    {
        private readonly ConfigJson _config;
        private readonly string configPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}config.json";

        public PirBankaConfig()
        {
            if (!File.Exists(configPath))
            {
                CreateConfiguration();
            }

            var configJson = File.ReadAllText(configPath);
            _config = JsonConvert.DeserializeObject<List<ConfigJson>>(configJson)[0];
        }

        public string ListenGateway => _config.listenGateway;
        public int ListenPort => _config.listenPort;
        public string InstanceName => _config.instanceName;
        public string InstanceDescription => _config.instanceDescription;
        public string DatabaseServer => _config.databaseServer;
        public string DatabaseServerPort => _config.databaseServerPort;
        public string DatabaseName => _config.databaseName;
        public string DatabaseUser => _config.databaseUser;
        public string DatabasePassword => _config.databasePassword;

        /// <summary>
        /// Creates config.json
        /// </summary>
        private void CreateConfiguration()
        {
            ConfigJson newConfig = new ConfigJson();

            Console.WriteLine("PirBanka detected first run. Please go through wizard.");
            Console.WriteLine("Set your instance name (default PirBanka):");
            var name = Console.ReadLine();
            if (String.IsNullOrEmpty(name)) name = "PirBanka";
            newConfig.instanceName = name;

            Console.WriteLine("Set your instance description:");
            newConfig.instanceDescription = Console.ReadLine();

            Console.WriteLine("Set IP to listen for HTTP requests (default localhost):");
            var gateway = Console.ReadLine();
            if (String.IsNullOrEmpty(gateway)) gateway = "localhost";
            newConfig.listenGateway = gateway;

            Console.WriteLine("Set port to listen for HTTP requests (default 8080):");
            var port = Console.ReadLine();
            if (String.IsNullOrEmpty(port)) port = "8080";
            newConfig.listenPort = Convert.ToInt32(port);

            Console.WriteLine();
            Console.WriteLine("Now set the database connection. PirBanka can't work without MySQL 5.8+");
            Console.WriteLine("What is your database server IP or hostname?");
            var dbIp = Console.ReadLine();
            if (String.IsNullOrEmpty(dbIp)) throw new ArgumentNullException();
            newConfig.databaseServer = dbIp;

            Console.WriteLine("What is your database port? (default is 3306)");
            var dbPort = Console.ReadLine();
            if (String.IsNullOrEmpty(port)) dbPort = "3306";
            newConfig.databaseServerPort = dbPort;

            Console.WriteLine("What is your database name?");
            var dbName = Console.ReadLine();
            if (String.IsNullOrEmpty(dbName)) throw new ArgumentNullException();
            newConfig.databaseName = dbName;

            Console.WriteLine("What is your database user?");
            var dbUser = Console.ReadLine();
            if (String.IsNullOrEmpty(dbUser)) throw new ArgumentNullException();
            newConfig.databaseUser = dbUser;

            Console.WriteLine("What is your database password?");
            var dbPass = Console.ReadLine();
            if (String.IsNullOrEmpty(dbPass)) throw new ArgumentNullException();
            newConfig.databasePassword = dbPass;

            Console.WriteLine();
            Console.WriteLine("Thanks, all needed informations were collected. Please wait while DB connection is checked and tables are created.");

            // Connect to DB
            MySqlConnectionString mySqlConnectionString = new MySqlConnectionString()
            {
                dbName = newConfig.databaseName,
                dbPassword = newConfig.databasePassword,
                dbServerIp = newConfig.databaseServer,
                dbServerPort = newConfig.databaseServerPort,
                dbUser = newConfig.databaseUser
            };
            Database database = new Database(mySqlConnectionString);

            // Fill DB
            bool dbFilled = database.InitializeDb();
            database.Dispose();

            // After DB is filled correctly, save configuration to config.json
            if(dbFilled)
            {
                Console.WriteLine("PirBanka is configured. Now we will save all informations to config.json: ");

                try
                {
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(newConfig));
                    Console.WriteLine("Configuration successfully saved.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Configuration can't be saved because of {ex.Message}. You can create the config.json by yourself and then try to solve the issue.");
                    Console.WriteLine("Content of config.json:");
                    Console.Write(JsonConvert.SerializeObject(newConfig));
                }
            }
            else
            {
                Console.WriteLine("DB was not filled correctly. Exiting...");
                Environment.Exit(1000);
            }
        }
    }
}
