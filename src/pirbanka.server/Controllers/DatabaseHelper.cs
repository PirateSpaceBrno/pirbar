using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PirBanka.Server.Controllers
{
    internal class DatabaseHelper : IDisposable
    {
        private static Database db;

        public DatabaseHelper(string connectionString)
        {
            db = new Database(connectionString, new MySqlDatabaseProvider());
        }

        public void BeginTransaction()
        {
            db.BeginTransaction();
        }

        public void CompleteTransaction()
        {
            db.CompleteTransaction();
        }

        public void AbortTransaction()
        {
            db.AbortTransaction();
        }

        public void Execute(string query)
        {
            db.Execute(query);
        }

        public void Insert(Tables tableName, object objToInsert)
        {
            db.Insert($"{tableName}", objToInsert);
        }

        public void Update(Tables tableName, object objToUpdate)
        {
            db.Update($"{tableName}", "id", objToUpdate);
        }

        public void Delete(Tables tableName, object objToDelete)
        {
            db.Delete($"{tableName}", "id", objToDelete);
        }

        public T Get<T>(Tables tableName, string where = "") where T : class, new()
        {
            string query = $"SELECT * FROM {tableName}";
            if (!string.IsNullOrEmpty(where)) query += $" WHERE {where}";
            query += ";";

            return db.SingleOrDefault<T>(query);
        }

        public List<T> GetList<T>(Tables tableName, string where = "") where T : class, new()
        {
            string query = $"SELECT * FROM {tableName}";
            if (!string.IsNullOrEmpty(where)) query += $" WHERE {where}";
            query += ";";

            return db.Query<T>(query).ToList();
        }

        public enum Tables
        {
            currencies,
            currencies_view,
            identities,
            accounts,
            accounts_view,
            transactions,
            authentications,
            exchange_rates
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
