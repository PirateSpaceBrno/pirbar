using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using PirBar.Server.Controllers;
using log4net;

namespace PirBar.Server
{
    class Database
    {
        private PirBarConfig config = new PirBarConfig();
        private MySqlConnection _conn;
        private ILog log = LogManager.GetLogger("pirbar-server.database");

        /// <summary>
        /// Creates new connection to MySQL database
        /// </summary>
        /// <param name="connectionString"></param>
        public Database(string connectionString)
        {
            _conn = new MySqlConnection();
            _conn.ConnectionString = connectionString;
            Connect();
        }

        /// <summary>
        /// Returns current connection status (opened, closed)
        /// </summary>
        public ConnectionState Status => _conn.State;

        private async void Connect()
        {
            try
            {
                if (Status != ConnectionState.Open)
                {
                    await _conn.OpenAsync();
                }
            }
            catch (Exception ex)
            {
                log.Fatal($"{ex.Message} | {ex.StackTrace}");
            }
        }

        private async void Close()
        {
            try
            {
                if (Status == ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                log.Fatal($"{ex.Message} | {ex.StackTrace}");
            }
        }

        private async Task<MySqlTransaction> StartTransaction()
        {
            Connect();
            return await _conn.BeginTransactionAsync();
        }

    }
}
