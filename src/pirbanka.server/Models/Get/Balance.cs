using PirBanka.Server.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirBanka.Server.Models.Get
{
    internal class Balance
    {
        public Currency currency { get; set; }
        public decimal chestBalance { get; set; }
        public decimal creditBalance { get; set; }
    }
}
