using System;

namespace PirBanka.Server.Models.Db
{
    public class ExchangeRates
    {
        public int id { get; set; }
        public int currency { get; set; }
        public DateTime valid_since { get; set; }
        public decimal rate { get; set; }
    }
}
