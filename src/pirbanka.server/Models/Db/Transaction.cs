using System;

namespace PirBanka.Server.Models.Db
{
    internal class Transaction
    {
        public int id { get; set; }
        public DateTime created { get; set; }
        public decimal amount { get; set; }
        public int source_account { get; set; }
        public int target_account { get; set; }
    }
}
