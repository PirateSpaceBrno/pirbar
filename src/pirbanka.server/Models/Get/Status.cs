using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirBanka.Server.Models.Get
{
    internal class Status
    {
        public string name => PirBankaHelper.BankIdentity.display_name;
        public string version => PirBanka.Server.AssemblyInfo.Version;
        public int identitiesCount => Server.db.GetList<Identity>(DatabaseHelper.Tables.identities).Count;
        public int accountsCount => Server.db.GetList<Account>(DatabaseHelper.Tables.accounts).Count;
        public int marketsCount => Server.db.GetList<Account>(DatabaseHelper.Tables.accounts, $"market=1").Count;
    }
}
