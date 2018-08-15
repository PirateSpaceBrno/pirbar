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
            var result = Server.db.GetList<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={BankIdentity.id} AND currency={currency.id}");
            return result.FirstOrDefault(x => x.description.StartsWith("outsideworld", System.StringComparison.InvariantCultureIgnoreCase));
        }

        public static void SendTransaction(AccountView sourceAccount, AccountView targetAccount, decimal amount)
        {
            if (sourceAccount == null)
            {
                throw new Exception("Source account does not exists.");
            }
            if (sourceAccount.balance <= amount && !sourceAccount.description.StartsWith("outsideworld", StringComparison.InvariantCultureIgnoreCase))
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
    }
}
