namespace PirBanka.Server.Models.Post
{

    internal class MarketsSell : TransactBase
    {
        public string paymentFromAccountIdentifier { get; set; }
        public string paymentToAccountIdentifier { get; set; }
        public string sourceAccountIdentifier { get; set; }
    }
}
