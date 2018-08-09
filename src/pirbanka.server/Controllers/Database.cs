using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;
using PirBanka.Server.Models;
using log4net;

namespace PirBanka.Server.Controllers
{
    class Database : IDisposable
    {
        private MySqlConnection _conn;
        private ILog log = LogManager.GetLogger("pirbanka.server.db");

        /// <summary>
        /// Creates new connection to MySQL database
        /// </summary>
        /// <param name="connectionString"></param>
        public Database(MySqlConnectionString dbConnInfo)
        {
            string connectionString = ToConnectionString(dbConnInfo);
            _conn = new MySqlConnection();
            _conn.ConnectionString = connectionString;
            Connect();
        }
        
        private string ToConnectionString(MySqlConnectionString db)
        {
            string connectionString = "";

            connectionString += "server=" + db.dbServerIp + ";";
            connectionString += "user=" + db.dbUser + ";";
            connectionString += "database=" + db.dbName + ";";
            connectionString += "port=" + db.dbServerPort + ";";
            connectionString += "password=" + db.dbPassword + ";";
            connectionString += "Character Set=utf8;";

            return connectionString;
        }


        /// <summary>
        /// Returns current connection status (opened, closed)
        /// </summary>
        public ConnectionState Status => _conn.State;

        private void Connect()
        {
            try
            {
                if (Status != ConnectionState.Open)
                {
                    _conn.Open();
                }
            }
            catch (Exception ex)
            {
                log.Fatal($"{ex.Message} | {ex.StackTrace}");
            }
        }

        public void Dispose()
        {
            try
            {
                if (Status == ConnectionState.Open)
                {
                    _conn.Close();
                }
            }
            catch (Exception ex)
            {
                log.Fatal($"{ex.Message} | {ex.StackTrace}");
            }
        }

        /// <summary>
        /// When first run of PirBanka, creates tables in DB.
        /// </summary>
        /// <returns></returns>
        internal bool InitializeDb()
        {
            string query = File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}pirbanka.sql");

            return (new MySqlCommand(query, _conn).ExecuteNonQuery() > 0);
        }

        private MySqlTransaction StartTransaction()
        {
            Connect();
            return _conn.BeginTransaction();
        }

    }
}
