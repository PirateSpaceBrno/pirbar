using System;

namespace PirBanka.Server.Models.Db
{
    internal class AccountView : Account
    {
        public string account_identifier { get; set; }
        public decimal balance { get; set; }
    }
}
