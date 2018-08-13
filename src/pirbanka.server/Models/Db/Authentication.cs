using System;

namespace PirBanka.Server.Models.Db
{
    internal class Authentication
    {
        public int id { get; set; }
        public int identity { get; set; }
        public int? account { get; set; }
        public string content { get; set; }
        public DateTime created { get; set; }
        public DateTime? expiration { get; set; }
    }
}
