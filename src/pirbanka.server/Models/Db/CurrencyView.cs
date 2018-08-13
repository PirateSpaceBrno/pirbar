using System;

namespace PirBanka.Server.Models.Db
{
    internal class CurrencyView : Currency
    {
        public DateTime valid_since { get; set; }
        public decimal rate { get; set; }
    }
}
