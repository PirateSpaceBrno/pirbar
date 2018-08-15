using System;

namespace PirBanka.Server.Models.Get
{
    internal class Token
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
        public int authentications_id { get; set; }
    }
}
