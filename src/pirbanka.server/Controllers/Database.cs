using MySql.Data.MySqlClient;
using PirBanka.Server.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PirBanka.Server.Controllers
{
    class Database : IDisposable
    {
        private MySqlConnection _conn;

        /// <summary>
        /// Creates new connection to MySQL database
        /// </summary>
        /// <param name="connectionString"></param>
        internal Database(MySqlConnectionString dbConnInfo)
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
            connectionString += "charset=utf8;";

            return connectionString;
        }


        /// <summary>
        /// Returns current connection status (opened, closed)
        /// </summary>
        internal ConnectionState Status => _conn.State;

        private void Connect()
        {
            try
            {
                if (Status != ConnectionState.Open)
                {
                    Console.WriteLine("Opening DB connection");
                    _conn.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL - {ex.Message} | {ex.StackTrace}");
            }
        }

        public void Dispose()
        {
            try
            {
                if (Status == ConnectionState.Open)
                {
                    Console.WriteLine("Closing DB connection");
                    _conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL - {ex.Message} | {ex.StackTrace}");
            }
        }

        /// <summary>
        /// When first run of PirBanka, creates tables in DB.
        /// </summary>
        /// <returns></returns>
        internal void InitializeDb()
        {
            string query = File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}pirbanka.sql");
            new MySqlCommand(query, _conn).ExecuteNonQuery();
        }

        /// <summary>
        /// Generic method to get content of MySQL table and return list of provided related objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal List<T> Get<T>(MySqlCommand command, Tables tableName) where T : class, new()
        {
            string query = $"SELECT * FROM {tableName};";
            command.CommandText = query;
            Console.WriteLine($"Querying: {query}");
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            adapter.Fill(table);

            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Use to start mysql transaction. Must be terminated with comman.Transaction.Commit or Rollback method.
        /// </summary>
        /// <returns></returns>
        internal MySqlCommand StartTransaction()
        {
            Connect();
            var trans = _conn.BeginTransaction();
            var command = _conn.CreateCommand();
            command.Connection = _conn;
            command.Transaction = trans;

            return command;
        }

        internal MySqlCommand GetCommand()
        {
            Connect();
            return new MySqlCommand("", _conn);
        }

        internal enum Tables
        {
            currencies_view,
            identities
        }

    }
}
