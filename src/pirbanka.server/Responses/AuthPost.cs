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
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Identity, id);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<IdentitiesAuthCreate>(request);

                        if (requestContent != null)
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
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Identity, id);
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
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Identity, identityId);
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
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Identity, identityId);
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
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Identity, identityId);
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
            //{
            //    @"",
            //    new Action<Request, Response>( async (req, res) =>
            //    {

            //    }
            //)},
        };
    }

}
