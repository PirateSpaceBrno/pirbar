using MySql.Data.MySqlClient;
using PirBanka.Server.Models;
using PirBanka.Server.Models.Db;
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
            _config = JsonHelper.DeserializeObject<List<ConfigJson>>(configJson)[0];
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
        public string DbConnectionString => _config.dbConnectionString;

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
            Console.WriteLine("What is your database server IP or hostname (default localhost)?");
            var dbIp = Console.ReadLine();
            if (String.IsNullOrEmpty(dbIp)) dbIp = "localhost";
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
            Console.WriteLine("Now please specify some additional information about your instance.");
            Console.WriteLine("What will be the default currency? Please type its name:");
            var currencyName = Console.ReadLine();
            if (String.IsNullOrEmpty(currencyName)) throw new ArgumentNullException();
            Console.WriteLine("Now please type its shortname (e.g. currency symbol):");
            var currencyShortName = Console.ReadLine();
            if (String.IsNullOrEmpty(currencyShortName)) throw new ArgumentNullException();


            Console.WriteLine();
            Console.WriteLine("Thanks, all needed informations were collected. Please wait while DB connection is checked and tables are created.");

            bool blockCheck = false;
            Console.Write(".");
            try
            {
                // Connect to DB
                MySqlConnection db = new MySqlConnection(newConfig.dbConnectionString);
                Console.Write(".");
                db.Open();
                Console.Write(".");

                // Fill DB
                MySqlCommand command = new MySqlCommand(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}pirbanka.sql"), db);
                command.ExecuteNonQuery();
                Console.Write(".");
                db.Dispose();

                blockCheck = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                blockCheck = false;
            }

            // Fill db with default bank identity, currency and account
            if(blockCheck)
            {
                blockCheck = false;
                Console.Write(".");

                using (DatabaseHelper database = new DatabaseHelper(newConfig.dbConnectionString))
                {
                    Console.Write(".");
                    try
                    {
                        database.BeginTransaction();
                        Console.Write(".");

                        Identity bankIdentity = new Identity()
                        {
                            name = TextHelper.RemoveSpecialCharacters(newConfig.instanceName),
                            display_name = newConfig.instanceName,
                            created = DateTime.Now
                        };
                        database.Insert(DatabaseHelper.Tables.identities, bankIdentity);
                        Console.Write(".");

                        bankIdentity = database.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{bankIdentity.name}'");
                        Console.Write(".");

                        Currency generalCurrency = new Currency()
                        {
                            name = currencyName,
                            shortname = currencyShortName
                        };
                        database.Insert(DatabaseHelper.Tables.currencies, generalCurrency);
                        Console.Write(".");

                        generalCurrency = database.Get<Currency>(DatabaseHelper.Tables.currencies, $"name='{generalCurrency.name}'");
                        Console.Write(".");

                        database.Execute("SET FOREIGN_KEY_CHECKS=0;");
                        Console.Write(".");

                        Account bankOutsideWorldAccount = new Account()
                        {
                            currency_id = generalCurrency.id,
                            identity = bankIdentity.id,
                            market = false,
                            description = $"OUTSIDEWORLD account for {generalCurrency.name}",
                            created = DateTime.Now
                        };
                        database.Insert(DatabaseHelper.Tables.accounts, bankOutsideWorldAccount);
                        Console.Write(".");

                        database.Execute("SET FOREIGN_KEY_CHECKS=1;");
                        Console.Write(".");

                        database.CompleteTransaction();
                        Console.Write(".");

                        blockCheck = true;
                        Console.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        database.AbortTransaction();
                        Console.WriteLine(ex.Message);
                        blockCheck = false;
                    }
                }
            }
            
            if (blockCheck)
            {
                blockCheck = false;
                try
                {
                    // After DB is filled correctly, save configuration to config.json
                    Console.WriteLine("PirBanka is configured. Now we will save all informations to config.json: ");

                    var configToSerialize = new List<ConfigJson>();
                    configToSerialize.Add(newConfig);
                    File.WriteAllText(configPath, JsonHelper.SerializeObject(configToSerialize));
                    Console.WriteLine("Configuration successfully saved.");

                    // Empty line
                    Console.WriteLine();
                    blockCheck = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Configuration can't be saved because of {ex.Message}. You can create the config.json by yourself and then try to solve the issue.");
                    Console.WriteLine("Content of config.json:");
                    Console.Write(JsonHelper.SerializeObject(newConfig));
                    blockCheck = false;
                }
            }

            if (!blockCheck) Environment.Exit(1);
        }
    }
}
