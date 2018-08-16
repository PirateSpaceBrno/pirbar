using JamesWright.SimpleHttp;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Db;
using PirBanka.Server.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PirBanka.Server.Responses
{
    public class AuthPost
    {
        public static KeyValuePair<Regex, Action<Request, Response>> Action(string endpoint)
        {
            var action = Actions.FirstOrDefault(x => x.Key == endpoint);

            if (!action.Equals(new KeyValuePair<string, Action<Request, Response>>()))
            {
                return new KeyValuePair<Regex, Action<Request, Response>>(new Regex(action.Key), action.Value);
            }

            return new KeyValuePair<Regex, Action<Request, Response>>(new Regex(endpoint), JamesWright.SimpleHttp.Actions.Error404);
        }

        public static Dictionary<string, Action<Request, Response>> Actions = new Dictionary<string, Action<Request, Response>>()
        {
            {
                @"^/identities/(\d+)/authentications$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications$")[1];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account, id);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<IdentitiesAuthCreate>(request);

                        if (requestContent != null && !string.IsNullOrEmpty(requestContent.password))
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                Authentication authentication = new Authentication()
                                {
                                    identity = id,
                                    content = TextHelper.SHA512(requestContent.password),
                                    created = DateTime.Now
                                };

                                Server.db.Insert(DatabaseHelper.Tables.authentications, authentication);
                                Server.db.CompleteTransaction();

                                res.Content = $"Authentication for Identity {id} created.";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Authentication creation for Identity {id} failed.";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Authentication creation.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts$")[1];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account, id);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<AccountsCreate>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                Account account = new Account()
                                {
                                    currency_id = requestContent.currencyId,
                                    market = false,
                                    identity = id,
                                    description = requestContent.description,
                                    created = DateTime.Now
                                };

                                Server.db.Insert(DatabaseHelper.Tables.accounts, account);
                                Server.db.CompleteTransaction();

                                res.Content = $"Account for Identity {id} created.";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Account creation for Identity {id} failed.";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Account creation.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)/transactions$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions$")[1];
                    int accountId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions$")[2];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account, identityId);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<TransactCreate>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                var sourceAccount = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={identityId} and id={accountId}");
                                var targetAccount = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"account_identifier='{requestContent.targetAccountIdentifier}'");

                                PirBankaHelper.SendTransaction(sourceAccount, targetAccount, requestContent.amount);
                                Server.db.CompleteTransaction();

                                res.Content = $"Transaction completed.";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Transaction failed: {ex.Message}";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Transaction.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)/deposit$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/deposit$")[1];
                    int accountId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/deposit$")[2];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account, identityId);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<TransactBase>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                var targetAccount = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={identityId} and id={accountId}");
                                var sourceAccount = PirBankaHelper.BankOutSideWorldAccount(Server.db.Get<Currency>(DatabaseHelper.Tables.currencies_view, $"id={targetAccount.currency_id}"));

                                PirBankaHelper.SendTransaction(sourceAccount, targetAccount, requestContent.amount);
                                Server.db.CompleteTransaction();

                                res.Content = $"Deposit completed.";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Deposit failed: {ex.Message}";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Deposit.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)/withdrawal$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/withdrawal$")[1];
                    int accountId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/withdrawal$")[2];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account, identityId);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<TransactBase>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                var sourceAccount = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={identityId} and id={accountId}");
                                var targetAccount = PirBankaHelper.BankOutSideWorldAccount(Server.db.Get<Currency>(DatabaseHelper.Tables.currencies_view, $"id={sourceAccount.currency_id}"));

                                PirBankaHelper.SendTransaction(sourceAccount, targetAccount, requestContent.amount);
                                Server.db.CompleteTransaction();

                                res.Content = $"Withdrawal completed.";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Withdrawal failed: {ex.Message}";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Withdrawal.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/markets$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/markets$")[1];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account, id);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<MarketsCreate>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                Account account = new Account()
                                {
                                    currency_id = requestContent.currencyId,
                                    market = true,
                                    identity = id,
                                    description = requestContent.description,
                                    created = DateTime.Now
                                };
                                
                                Server.db.Insert(DatabaseHelper.Tables.accounts, account);
                                account = Server.db.Get<Account>(DatabaseHelper.Tables.accounts, $"identity={id} AND market=1 AND currency_id={requestContent.currencyId}");

                                if(!string.IsNullOrEmpty(requestContent.password))
                                {
                                    Authentication authentication = new Authentication()
                                    {
                                        identity = id,
                                        account = account.id,
                                        content = TextHelper.SHA512(requestContent.password),
                                        created = DateTime.Now
                                    };

                                    Server.db.Execute("SET FOREIGN_KEY_CHECKS=0;");
                                    Server.db.Insert(DatabaseHelper.Tables.authentications, authentication);
                                    Server.db.Execute("SET FOREIGN_KEY_CHECKS=1;");
                                }
                                
                                Server.db.CompleteTransaction();

                                res.Content = $"Market for Identity {id} created.";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Market creation for Identity {id} failed.";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Market creation.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/markets/(\d+)/buy$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int accountId = TextHelper.GetUriIds(req.Endpoint, @"^/markets/(\d+)/buy$")[1];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<MarketsBuy>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                var paymentFrom = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={auth.identity} and account_identifier='{requestContent.paymentFromAccountIdentifier}'");
                                var paymentTo = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"account_identifier='{requestContent.paymentToAccountIdentifier}'");
                                var market = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"id={accountId} AND market=1");

                                AccountView targetAccount = null;
                                bool withdrawal = false;
                                var marketCurrency = Server.db.Get<CurrencyView>(DatabaseHelper.Tables.currencies_view, $"id={market.currency_id}");

                                if(!string.IsNullOrEmpty(requestContent.targetAccountIdentifier))
                                {
                                    targetAccount = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={auth.identity} and account_identifier='{requestContent.targetAccountIdentifier}'");
                                }
                                else
                                {
                                    targetAccount = PirBankaHelper.BankOutSideWorldAccount(marketCurrency);
                                    withdrawal = true;
                                }

                                PirBankaHelper.SendMarketBuy(paymentFrom, market, paymentTo, requestContent.amount, targetAccount);
                                Server.db.CompleteTransaction();

                                res.Content = withdrawal ? $"{requestContent.amount} {marketCurrency.name} transferred to {targetAccount.account_identifier} account" : $"Withdraw {requestContent.amount} {marketCurrency.name}";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Transaction failed: {ex.Message}";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Transaction.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/markets/buy$",
                new Action<Request, Response>( async (req, res) =>
                {
                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<MarketsBuy>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                var paymentFrom = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={auth.identity} and account_identifier='{requestContent.paymentFromAccountIdentifier}'");
                                var paymentTo = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"account_identifier='{requestContent.paymentToAccountIdentifier}' AND market=1");

                                AccountView market = null;
                                if (!string.IsNullOrEmpty(requestContent.marketPassword))
                                {
                                    var marketAuth = Server.db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"content='{TextHelper.SHA512(requestContent.marketPassword)}'");
                                    if (marketAuth != null)
                                    {
                                        market = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"id={marketAuth.identity} AND market=1");
                                    }
                                }

                                AccountView targetAccount = null;
                                bool withdrawal = false;
                                var marketCurrency = Server.db.Get<CurrencyView>(DatabaseHelper.Tables.currencies_view, $"id={market.currency_id}");

                                if(!string.IsNullOrEmpty(requestContent.targetAccountIdentifier))
                                {
                                    targetAccount = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={auth.identity} and account_identifier='{requestContent.targetAccountIdentifier}'");
                                }
                                else
                                {
                                    targetAccount = PirBankaHelper.BankOutSideWorldAccount(marketCurrency);
                                    withdrawal = true;
                                }

                                PirBankaHelper.SendMarketBuy(paymentFrom, market, paymentTo, requestContent.amount, targetAccount);
                                Server.db.CompleteTransaction();

                                res.Content = withdrawal ? $"{requestContent.amount} {marketCurrency.name} transferred to {targetAccount.account_identifier} account" : $"Withdraw {requestContent.amount} {marketCurrency.name}";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Transaction failed: {ex.Message}";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Transaction.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/markets/(\d+)/authentications$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/markets/(\d+)/authentications$")[1];
                    int marketId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/markets/(\d+)/authentications$")[2];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account, id);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<IdentitiesAuthCreate>(request);

                        if (requestContent != null && !string.IsNullOrEmpty(requestContent.password))
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                var market = Server.db.Get<Account>(DatabaseHelper.Tables.accounts, $"identity={id} AND id={marketId} AND market=1");
                                if (market == null)
                                {
                                    throw new Exception("Market not found.");
                                }

                                Authentication authentication = new Authentication()
                                {
                                    identity = id,
                                    account = marketId,
                                    content = TextHelper.SHA512(requestContent.password),
                                    created = DateTime.Now
                                };

                                Server.db.Insert(DatabaseHelper.Tables.authentications, authentication);
                                Server.db.CompleteTransaction();

                                res.Content = $"Authentication for Market {marketId} created.";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Authentication creation for Market {marketId} failed.";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Authentication creation.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/markets/(\d+)/sell$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int accountId = TextHelper.GetUriIds(req.Endpoint, @"^/markets/(\d+)/sell$")[1];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<MarketsSell>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                var paymentFrom = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"account_identifier='{requestContent.paymentFromAccountIdentifier}' AND market=1");
                                var paymentTo = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={auth.identity} and account_identifier='{requestContent.paymentToAccountIdentifier}'");
                                var market = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"id={accountId} AND market=1");

                                AccountView sourceAccount = null;
                                bool deposit = false;
                                var marketCurrency = Server.db.Get<CurrencyView>(DatabaseHelper.Tables.currencies_view, $"id={market.currency_id}");

                                if(!string.IsNullOrEmpty(requestContent.sourceAccountIdentifier))
                                {
                                    sourceAccount = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={auth.identity} and account_identifier='{requestContent.sourceAccountIdentifier}'");
                                }
                                else
                                {
                                    sourceAccount = PirBankaHelper.BankOutSideWorldAccount(marketCurrency);
                                    deposit = true;
                                }

                                PirBankaHelper.SendMarketSell(paymentFrom, market, paymentTo, requestContent.amount, sourceAccount);
                                Server.db.CompleteTransaction();

                                res.Content = deposit ? $"{requestContent.amount} {marketCurrency.name} transferred from {sourceAccount.account_identifier} account" : $"Deposit of {requestContent.amount} {marketCurrency.name}";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Transaction failed: {ex.Message}";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Transaction.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            //{
            //    @"",
            //    new Action<Request, Response>( async (req, res) =>
            //    {

        //    }
        //)},
    };
    }

}
