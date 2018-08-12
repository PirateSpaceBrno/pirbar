using System;

namespace PirBanka.Server.Models.Db
{
    internal class Currency
    {
        public int id { get; set; }
        public string name { get; set; }
        public string shortname { get; set; }
        public DateTime valid_since { get; set; }
        public decimal rate { get; set; }
    }
}
