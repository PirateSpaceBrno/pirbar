namespace PirBanka.Server.Models.Db
{
    internal class Account
    {
        public int id { get; set; }
        public int identity { get; set; }
        public string description { get; set; }
        public bool market { get; set; }
        public int currency { get; set; }
        public decimal balance { get; set; }
    }
}
