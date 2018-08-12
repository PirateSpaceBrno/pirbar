using System;

namespace PirBanka.Server.Models.Db
{
    internal class Identity
    {
        public int id { get; set; }
        public string name { get; set; }
        public string display_name { get; set; }
        public DateTime created { get; set; }
    }
}
