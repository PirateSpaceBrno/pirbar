namespace PirBanka.Server.Models.Post
{

    internal class MarketsBuy : TransactBase
    {
        public string paymentFromAccountIdentifier { get; set; }
        public string paymentToAccountIdentifier { get; set; }
        public string targetAccountIdentifier { get; set; }
        public string marketPassword { get; set; }
    }
}
