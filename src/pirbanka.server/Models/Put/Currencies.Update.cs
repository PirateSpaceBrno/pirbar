using PirBanka.Server.Models.Post;

namespace PirBanka.Server.Models.Put
{
    internal class CurrenciesUpdate : CurrenciesCreate
    {
        public decimal? exchangeRate { get; set; }
    }
}
