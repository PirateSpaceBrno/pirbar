namespace PirBanka.Server.Models.Post
{
    internal class TransactCreate : TransactBase
    {
        public string targetAccountIdentifier { get; set; }
    }
}
