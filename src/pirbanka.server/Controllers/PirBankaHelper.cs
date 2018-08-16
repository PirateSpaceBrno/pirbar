using PirBanka.Server.Models.Db;
using System;
using System.Linq;

namespace PirBanka.Server.Controllers
{
    internal static class PirBankaHelper
    {
        public static Identity BankIdentity => Server.db.Get<Identity>(DatabaseHelper.Tables.identities, $"display_name='{Server.config.InstanceName}'");
        public static CurrencyView GeneralCurrency => Server.db.GetList<CurrencyView>(DatabaseHelper.Tables.currencies_view).FirstOrDefault();
        public static AccountView BankOutSideWorldAccount(Currency currency)
        {
            if(currency == null) { return null; }
            var result = Server.db.GetList<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={BankIdentity.id} AND currency_id={currency.id}");
            return result.FirstOrDefault(x => x.description.StartsWith("outsideworld", System.StringComparison.InvariantCultureIgnoreCase));
        }

        public static void SendTransaction(AccountView sourceAccount, AccountView targetAccount, decimal amount)
        {
            if (sourceAccount == null)
            {
                throw new Exception("Source account does not exists.");
            }
            if (sourceAccount.balance < amount && !sourceAccount.description.StartsWith("outsideworld", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("There are no enough funds on the source account.");
            }
            if (amount != Math.Abs(amount))
            {
                throw new Exception("Amount cannot be negative.");
            }

            if (targetAccount == null)
            {
                throw new Exception($"Target account with identifier {targetAccount.account_identifier} does not exists.");
            }

            if (sourceAccount.currency_id != targetAccount.currency_id)
            {
                throw new Exception("Source and target account must be in the same currency.");
            }

            Transact newTransact = new Transact()
            {
                amount = amount,
                created = DateTime.Now,
                source_account = sourceAccount.id,
                target_account = targetAccount.id
            };

            Server.db.Insert(DatabaseHelper.Tables.transactions, newTransact);
        }

        public static void SendMarketBuy(AccountView payFrom, AccountView market, AccountView marketPayTo, decimal amount, AccountView targetAccount)
        {
            if (payFrom == null)
            {
                throw new Exception("Payment account does not exists.");
            }
            if (market == null)
            {
                throw new Exception($"Market does not exists.");
            }
            if (marketPayTo == null)
            {
                throw new Exception($"Market account for payments does not exists.");
            }
            if (targetAccount == null)
            {
                throw new Exception("Target account does not exists.");
            }
            if (market.identity != marketPayTo.identity)
            {
                throw new Exception("Account to pay to must belong to the same identity as market.");
            }
            
            var marketCurrency = Server.db.Get<CurrencyView>(DatabaseHelper.Tables.currencies_view, $"id={market.currency_id}");
            var paymentCurrency = Server.db.Get<CurrencyView>(DatabaseHelper.Tables.currencies_view, $"id={payFrom.currency_id}");

            SendTransaction(payFrom, marketPayTo, (amount * marketCurrency.rate)/paymentCurrency.rate);
            SendTransaction(market, targetAccount, amount);
        }

        public static void SendMarketSell(AccountView marketPayFrom, AccountView market, AccountView payTo, decimal amount, AccountView sourceAccount)
        {
            if (marketPayFrom == null)
            {
                throw new Exception("Market account for payments does not exists.");
            }
            if (market == null)
            {
                throw new Exception($"Market does not exists.");
            }
            if (payTo == null)
            {
                throw new Exception($"Account to receive payment does not exists.");
            }
            if (sourceAccount == null)
            {
                throw new Exception("Source account does not exists.");
            }
            if (market.identity != marketPayFrom.identity)
            {
                throw new Exception("Payment account must belong to the same identity as market.");
            }

            var marketCurrency = Server.db.Get<CurrencyView>(DatabaseHelper.Tables.currencies_view, $"id={market.currency_id}");
            var paymentCurrency = Server.db.Get<CurrencyView>(DatabaseHelper.Tables.currencies_view, $"id={payTo.currency_id}");

            SendTransaction(marketPayFrom, payTo, (amount * marketCurrency.rate) / paymentCurrency.rate);
            SendTransaction(sourceAccount, market, amount);
        }
    }
}
