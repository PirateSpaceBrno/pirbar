using JamesWright.SimpleHttp;
using Newtonsoft.Json;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Db;
using PirBanka.Server.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PirBanka.Server.Responses
{
    public class Post
    {
        public static KeyValuePair<Regex, Action<Request, Response>> Action(string endpoint)
        {
            var action = Actions.FirstOrDefault(x => x.Key == endpoint);

            if (!action.Equals(new KeyValuePair<string, Action<Request, Response>>()))
            {
                return new KeyValuePair<Regex, Action<Request, Response>>(new Regex(action.Key), action.Value);
            }

            return new KeyValuePair<Regex, Action<Request, Response>>();
        }

        private static Dictionary<string, Action<Request, Response>> Actions = new Dictionary<string, Action<Request, Response>>()
        {
            {
                @"^/authenticate$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var token = HttpAuth.CreateToken(Server.db, req.UserIdentity);

                    res.Content = JsonConvert.SerializeObject(token);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/create$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var request = await req.GetBodyAsync();

                    try
                    {
                        Server.db.BeginTransaction();

                        var identitiesCreate = JsonConvert.DeserializeObject<IdentitiesCreate>(request);
                        if(identitiesCreate != null)
                        {
                            Identity newIdentity = new Identity()
                            {
                                name = TextHelper.RemoveSpecialCharacters(identitiesCreate.name),
                                display_name = identitiesCreate.name,
                                admin = false,
                                created = DateTime.Now
                            };

                            Server.db.Insert(DatabaseHelper.Tables.identities, newIdentity);

                            newIdentity = Server.db.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{TextHelper.RemoveSpecialCharacters(newIdentity.name)}'");
                            Authentication authentication = new Authentication()
                            {
                                identity = newIdentity.id,
                                content = TextHelper.SHA512(identitiesCreate.password),
                                created = DateTime.Now
                            };

                            Account account = new Account()
                            {
                                currency_id = 1,
                                description = "GENERAL account",
                                identity = newIdentity.id,
                                market = false,
                                created = DateTime.Now
                            };

                            Server.db.Execute("SET FOREIGN_KEY_CHECKS=0;");
                            Server.db.Insert(DatabaseHelper.Tables.authentications, authentication);
                            Server.db.Insert(DatabaseHelper.Tables.accounts, account);
                            Server.db.Execute("SET FOREIGN_KEY_CHECKS=1;");
                            Server.db.CompleteTransaction();

                            res.Content = $"Identity {newIdentity.display_name} created. Login name is {newIdentity.name}.";
                            res.StatusCode = StatusCodes.Success.Created;
                        }
                        else
                        {
                            res.Content = $"Invalid data for Identity creation.";
                            res.StatusCode = StatusCodes.ClientError.BadRequest;
                        }
                    }
                    catch (Exception ex)
                    {
                        Server.db.AbortTransaction();

                        res.Content = $"Identity creation failed.";
                        res.StatusCode = StatusCodes.Redirection.NotModified;
                        Console.WriteLine($"WARN - {ex.Message}");
                    }

                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/authentications/create$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/create$")[1];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(Server.db, req.UserIdentity, HttpAuth.AccessLevel.Identity, id);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonConvert.DeserializeObject<IdentitiesAuthCreate>(request);

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

                                Server.db.Execute("SET FOREIGN_KEY_CHECKS=0;");
                                Server.db.Insert(DatabaseHelper.Tables.authentications, authentication);
                                Server.db.Execute("SET FOREIGN_KEY_CHECKS=1;");
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
                @"^/identities/(\d+)/authentications/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/(\d+)$")[1];
                    int authId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/(\d+)$")[2];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(Server.db, req.UserIdentity, HttpAuth.AccessLevel.Identity, identityId);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonConvert.DeserializeObject<IdentitiesAuthCreate>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                var authentication = Server.db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"id={authId}");
                                if(authentication != null)
                                {
                                    authentication.content = TextHelper.SHA512(requestContent.password);
                                    authentication.created = DateTime.Now;
                                    Server.db.Update(DatabaseHelper.Tables.authentications, authentication);
                                }

                                Server.db.CompleteTransaction();

                                res.Content = $"Authentication for Identity {identityId} updated.";
                                res.StatusCode = StatusCodes.Success.Ok;
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Authentication update for Identity {identityId} failed.";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Authentication update.";
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
                @"^/currencies/create$",
                new Action<Request, Response>( async (req, res) =>
                {
                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(Server.db, req.UserIdentity, HttpAuth.AccessLevel.Administrator);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();

                        try
                        {
                            Server.db.BeginTransaction();

                            var currencyCreate = JsonConvert.DeserializeObject<CurrenciesCreate>(request);
                            if (currencyCreate != null)
                            {
                                Currency newCurrency = new Currency()
                                {
                                    name = currencyCreate.name,
                                    shortname = currencyCreate.shortname
                                };

                                Server.db.Insert(DatabaseHelper.Tables.currencies, newCurrency);

                                newCurrency = Server.db.Get<Currency>(DatabaseHelper.Tables.currencies, $"name='{newCurrency.name}'");

                                // Create account for bank
                                Account bankOutsideWorldAccount = new Account()
                                {
                                    currency_id = newCurrency.id,
                                    identity = 1,
                                    market = false,
                                    description = $"OUTSIDEWORLD account for {newCurrency.name}",
                                    created = DateTime.Now
                                };

                                Server.db.Execute("SET FOREIGN_KEY_CHECKS=0;");
                                Server.db.Insert(DatabaseHelper.Tables.accounts, bankOutsideWorldAccount);
                                Server.db.Execute("SET FOREIGN_KEY_CHECKS=1;");

                                Server.db.CompleteTransaction();

                                res.Content = $"Currency {newCurrency.name} created.";
                                res.StatusCode = StatusCodes.Success.Created;
                            }
                            else
                            {
                                res.Content = $"Invalid data for Currency creation.";
                                res.StatusCode = StatusCodes.ClientError.BadRequest;
                            }

                        }
                        catch (Exception ex)
                        {
                            Server.db.AbortTransaction();

                            res.Content = $"Currency creation failed: {ex.Message}";
                            res.StatusCode = StatusCodes.Redirection.NotModified;
                            Console.WriteLine($"WARN - {ex.Message}");
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
