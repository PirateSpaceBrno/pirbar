using PirBanka.Server.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirBanka.Server.Models.Get
{
    class Identify
    {
        public Identity identity { get; set; }
        public AccountView account { get; set; }
    }
}
