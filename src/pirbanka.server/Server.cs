using JamesWright.SimpleHttp;
using Newtonsoft.Json;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Db;
using PirBanka.Server.Models.Get;
using PirBanka.Server.Models.Post;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PirBanka.Server
{
    class Server
    {
        internal static PirBankaConfig config;
        internal static DatabaseHelper db;
        internal static SimpleHttpServer app;

        static void Main()
        {
            Console.WriteLine("Starting PirBanka...");

            // Check DB connection and state
            // If not, close app (e.g. connections string does not exist, DB connection can't open, DB does not exist, etc.)
            // Check DB is filled with tables and data
            // If not, provide console wizard to fill it up

            config = new PirBankaConfig();
            //Database db = new Database(config.DbConnectionString);
            db = new DatabaseHelper(config.DbConnectionString);
            app = new SimpleHttpServer();

            #region GET endpoints returning HTML and misc
            app.Get(@"^/$", Responses.Get.Action(@"^/$"));
            app.Get(@"^/api-style.css$", Responses.Get.Action(@"^/api-style.css$"));
            app.Get(@"^/favicon.svg$", Responses.Get.Action(@"^/favicon.svg$"));
            #endregion

            #region GET listing endpoints
            // List of all currencies
            app.Get(@"^/currencies$", async (req, res) =>
            {
                List<Currency> result = db.GetList<Currency>(DatabaseHelper.Tables.currencies);

                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/currencies/(\d+)$", async (req, res) =>
            {
                var match = Regex.Match(req.Endpoint, @"^/currencies/(\d+)$", RegexOptions.IgnoreCase);
                int id = Convert.ToInt32(match.Groups[1].Value);

                Currency result = db.Get<CurrencyView>(DatabaseHelper.Tables.currencies_view, $"id={id}");

                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            // List of all identities
            app.Get(@"^/identities$", async (req, res) =>
            {
                List<Identity> result = db.GetList<Identity>(DatabaseHelper.Tables.identities);

                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/identities/(\d+)$", async (req, res) =>
            {
                var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)$", RegexOptions.IgnoreCase);
                int id = Convert.ToInt32(match.Groups[1].Value);

                Identity result = db.Get<Identity>(DatabaseHelper.Tables.identities, $"id={id}");

                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/identities/(\d+)/accounts$", async (req, res) =>
            {
                var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)/accounts$", RegexOptions.IgnoreCase);
                var id = match.Groups[1].Value;

                List<Account> result = db.GetList<Account>(DatabaseHelper.Tables.accounts, $"identity={id}");

                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/identities/(\d+)/accounts/(\d+)$", async (req, res) =>
            {
                var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)$", RegexOptions.IgnoreCase);
                var id = match.Groups[1].Value;
                var account_id = match.Groups[2].Value;

                Account result = db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={id} and id={account_id}");

                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });


            app.Get(@"^/identities/(\d+)/accounts/(\d+)/transactions$", async (req, res) =>
            {
                var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions$", RegexOptions.IgnoreCase);
                var account_id = match.Groups[2].Value;

                List<Transact> result = db.GetList<Transact>(DatabaseHelper.Tables.transactions, $"source_account={account_id} or target_account={account_id}");

                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/identities/(\d+)/accounts/(\d+)/transactions/incoming$", async (req, res) =>
            {
                var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions/incoming$", RegexOptions.IgnoreCase);
                var account_id = match.Groups[2].Value;

                List<Transact> result = db.GetList<Transact>(DatabaseHelper.Tables.transactions, $"target_account={account_id}");

                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/identities/(\d+)/accounts/(\d+)/transactions/outgoing$", async (req, res) =>
            {
                var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions/outgoing$", RegexOptions.IgnoreCase);
                var account_id = match.Groups[2].Value;

                List<Transact> result = db.GetList<Transact>(DatabaseHelper.Tables.transactions, $"source_account={account_id}");

                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/identities/(\d+)/authentications$", async (req, res) =>
            {
                int id = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications$")[1];

                // Authorized request
                var auth = HttpAuth.AuthenticateHttpRequest(db, req.UserIdentity, HttpAuth.AccessLevel.Identity, id);
                if (auth != null)
                {
                    List<Authentication> result = db.GetList<Authentication>(DatabaseHelper.Tables.authentications, $"identity={id}");

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = "application/json; charset=utf-8";
                }
                else
                {
                    res.Content = "Unauthorized";
                    res.StatusCode = 401;
                    res.ContentType = "text/html; charset=utf-8";
                }


                await res.SendAsync();
            });

            #endregion

            #region GET endpoints with documentation
            app.Get(@"^/doc/authenticate$", async (req, res) =>
            {
                string newContent = "<div>";
                newContent += "<h2>Authenticate</h2>";
                newContent += "<p>You can authenticate and create token with 15 minutes expiration for future operations.<br />";
                newContent += "Token can be created by following authorized request:";
                newContent += "<pre>";
                newContent += "request type: POST\n";
                newContent += "endpoint: /authenticate\n";
                newContent += "header: Authorization with Identity access\n";
                newContent += "response: application/json\n";
                newContent += $"response content: {JsonConvert.SerializeObject(new Token() { token = "token value" })}";
                newContent += "</pre>";
                newContent += "Token can be requested just for Identities access, so UserName has to be filled and Password must correspond to Authentication record for Identity.";
                newContent += "</p>";
                newContent += "</div>";

                res.Content = TextHelper.GetApiPage(newContent);
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/doc/identities/create$", async (req, res) =>
            {
                string newContent = "<div>";
                newContent += "<h2>Create new identity</h2>";
                newContent += "<p>You can create new identity by following request:";
                newContent += "<pre>";
                newContent += "request type: POST\n";
                newContent += "endpoint: /identities/create\n";
                newContent += "content type: application/json; charset=utf8\n";
                newContent += $"content: {JsonConvert.SerializeObject(new IdentitiesCreate() { name = "Your unique username", password = "Your password" })}";
                newContent += "</pre>";
                newContent += "Passwords are stored as SHA512 hash.";
                newContent += "</p>";
                newContent += "</div>";

                res.Content = TextHelper.GetApiPage(newContent);
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/doc/identities/(\d+)/authentications/create$", async (req, res) =>
            {
                int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/authentications/create$")[1];

                string newContent = "<div>";
                newContent += $"<h2>Create Authentication for Identity {id}</h2>";
                newContent += "<p>You can create new Identity Authentication by following authorized request:";
                newContent += "<pre>";
                newContent += "request type: POST\n";
                newContent += $"endpoint: /identities/{id}/authentications/create\n";
                newContent += "header: Authorization with Identity access\n";
                newContent += "content type: application/json; charset=utf8\n";
                newContent += $"content: {JsonConvert.SerializeObject(new IdentitiesAuthCreate() { password = "Your new password" })}";
                newContent += "</pre>";
                newContent += "Password is stored as SHA512 hash.";
                newContent += "</p>";
                newContent += "</div>";

                res.Content = TextHelper.GetApiPage(newContent);
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/doc/identities/(\d+)/authentications/(\d+)$", async (req, res) =>
            {
                int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/authentications/(\d+)$")[1];
                int authId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/authentications/(\d+)$")[2];

                string newContent = "<div>";
                newContent += $"<h2>Update Authentication</h2>";
                newContent += "<p>You can update this Identity Authentication by following authorized request:";
                newContent += "<pre>";
                newContent += "request type: POST\n";
                newContent += $"endpoint: /identities/{identityId}/authentications/{authId}\n";
                newContent += "header: Authorization with Identity access\n";
                newContent += "content type: application/json; charset=utf8\n";
                newContent += $"content: {JsonConvert.SerializeObject(new IdentitiesAuthCreate() { password = "Your new password" })}";
                newContent += "</pre>";
                newContent += "Password is stored as SHA512 hash.";
                newContent += "</p>";

                newContent += $"<h2>Delete Authentication</h2>";
                newContent += "<p>You can delete this Identity Authentication by following authorized request:";
                newContent += "<pre>";
                newContent += "request type: DELETE\n";
                newContent += $"endpoint: /identities/{identityId}/authentications/{authId}\n";
                newContent += "header: Authorization with Identity access\n";
                newContent += "</pre>";
                newContent += "WARNING! If you delete all your Identity Authentications, you will loose access to your account.";
                newContent += "</p>";

                newContent += "</div>";

                res.Content = TextHelper.GetApiPage(newContent);
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/doc/currencies/create$", async (req, res) =>
            {
                string newContent = "<div>";
                newContent += $"<h2>Create new Currency</h2>";
                newContent += "<p>You can create new currency by following authorized request:";
                newContent += "<pre>";
                newContent += "request type: POST\n";
                newContent += "endpoint: /currencies/create\n";
                newContent += "header: Authorization with Administrator access\n";
                newContent += "content type: application/json; charset=utf8\n";
                newContent += $"content: {JsonConvert.SerializeObject(new CurrenciesCreate() { name = "Currency display name", shortname = "Curr" })}";
                newContent += "</pre>";
                newContent += "</p>";
                newContent += "</div>";

                res.Content = TextHelper.GetApiPage(newContent);
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });
            #endregion

            #region POST endpoints
            app.Post(@"^/authenticate$", async (req, res) =>
            {
                var token = HttpAuth.CreateToken(db, req.UserIdentity);

                res.Content = JsonConvert.SerializeObject(token);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Post(@"^/identities/create$", async (req, res) =>
            {
                var request = await req.GetBodyAsync();

                try
                {
                    db.BeginTransaction();

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

                        db.Insert(DatabaseHelper.Tables.identities, newIdentity);

                        newIdentity = db.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{TextHelper.RemoveSpecialCharacters(newIdentity.name)}'");
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

                        db.Execute("SET FOREIGN_KEY_CHECKS=0;");
                        db.Insert(DatabaseHelper.Tables.authentications, authentication);
                        db.Insert(DatabaseHelper.Tables.accounts, account);
                        db.Execute("SET FOREIGN_KEY_CHECKS=1;");
                        db.CompleteTransaction();

                        res.Content = $"Identity {newIdentity.display_name} created. Login name is {newIdentity.name}.";
                        res.StatusCode = 201;
                    }
                    else
                    {
                        res.Content = $"Invalid data for Identity creation.";
                        res.StatusCode = 400;
                    }
                }
                catch (Exception ex)
                {
                    db.AbortTransaction();

                    res.Content = $"Identity creation failed.";
                    res.StatusCode = 400;
                    Console.WriteLine($"WARN - {ex.Message}");
                }

                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Post(@"^/identities/(\d+)/authentications/create$", async (req, res) =>
            {
                int id = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/create$")[1];

                // Authorized request
                var auth = HttpAuth.AuthenticateHttpRequest(db, req.UserIdentity, HttpAuth.AccessLevel.Identity, id);
                if (auth != null)
                {
                    var request = await req.GetBodyAsync();
                    var requestContent = JsonConvert.DeserializeObject<IdentitiesAuthCreate>(request);

                    if (requestContent != null)
                    {
                        try
                        {
                            db.BeginTransaction();

                            Authentication authentication = new Authentication()
                            {
                                identity = id,
                                content = TextHelper.SHA512(requestContent.password),
                                created = DateTime.Now
                            };

                            db.Execute("SET FOREIGN_KEY_CHECKS=0;");
                            db.Insert(DatabaseHelper.Tables.authentications, authentication);
                            db.Execute("SET FOREIGN_KEY_CHECKS=1;");
                            db.CompleteTransaction();

                            res.Content = $"Authentication for Identity {id} created.";
                            res.StatusCode = 201;
                        }
                        catch (Exception ex)
                        {
                            db.AbortTransaction();

                            res.Content = $"Authentication creation for Identity {id} failed.";
                            res.StatusCode = 400;
                            Console.WriteLine($"WARN - {ex.Message}");
                        }
                    }
                    else
                    {
                        res.Content = $"Invalid data for Authentication creation.";
                        res.StatusCode = 400;
                    }
                }
                else
                {
                    res.Content = "Unauthorized";
                    res.StatusCode = 401;
                }

                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Post(@"^/identities/(\d+)/authentications/(\d+)$", async (req, res) =>
            {
                int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/(\d+)$")[1];
                int authId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/(\d+)$")[2];

                // Authorized request
                var auth = HttpAuth.AuthenticateHttpRequest(db, req.UserIdentity, HttpAuth.AccessLevel.Identity, identityId);
                if (auth != null)
                {
                    var request = await req.GetBodyAsync();
                    var requestContent = JsonConvert.DeserializeObject<IdentitiesAuthCreate>(request);

                    if (requestContent != null)
                    {
                        try
                        {
                            db.BeginTransaction();

                            var authentication = db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"id={authId}");
                            if(authentication != null)
                            {
                                authentication.content = TextHelper.SHA512(requestContent.password);
                                authentication.created = DateTime.Now;
                                db.Update(DatabaseHelper.Tables.authentications, authentication);
                            }

                            db.CompleteTransaction();

                            res.Content = $"Authentication for Identity {identityId} updated.";
                            res.StatusCode = 200;
                        }
                        catch (Exception ex)
                        {
                            db.AbortTransaction();

                            res.Content = $"Authentication update for Identity {identityId} failed.";
                            res.StatusCode = 304;
                            Console.WriteLine($"WARN - {ex.Message}");
                        }
                    }
                    else
                    {
                        res.Content = $"Invalid data for Authentication update.";
                        res.StatusCode = 304;
                    }
                }
                else
                {
                    res.Content = "Unauthorized";
                    res.StatusCode = 401;
                }

                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Post(@"^/currencies/create$", async (req, res) =>
            {
                // Authorized request
                var auth = HttpAuth.AuthenticateHttpRequest(db, req.UserIdentity, HttpAuth.AccessLevel.Administrator);
                if (auth != null)
                {
                    var request = await req.GetBodyAsync();

                    try
                    {
                        db.BeginTransaction();

                        var currencyCreate = JsonConvert.DeserializeObject<CurrenciesCreate>(request);
                        if (currencyCreate != null)
                        {
                            Currency newCurrency = new Currency()
                            {
                                name = currencyCreate.name,
                                shortname = currencyCreate.shortname
                            };

                            db.Insert(DatabaseHelper.Tables.currencies, newCurrency);

                            newCurrency = db.Get<Currency>(DatabaseHelper.Tables.currencies, $"name='{newCurrency.name}'");

                            // Create account for bank
                            Account bankOutsideWorldAccount = new Account()
                            {
                                currency_id = newCurrency.id,
                                identity = 1,
                                market = false,
                                description = $"OUTSIDEWORLD account for {newCurrency.name}",
                                created = DateTime.Now
                            };

                            db.Execute("SET FOREIGN_KEY_CHECKS=0;");
                            db.Insert(DatabaseHelper.Tables.accounts, bankOutsideWorldAccount);
                            db.Execute("SET FOREIGN_KEY_CHECKS=1;");

                            db.CompleteTransaction();

                            res.Content = $"Currency {newCurrency.name} created.";
                            res.StatusCode = 201;
                        }
                        else
                        {
                            res.Content = $"Invalid data for Currency creation.";
                            res.StatusCode = 400;
                        }

                    }
                    catch (Exception ex)
                    {
                        db.AbortTransaction();

                        res.Content = $"Currency creation failed: {ex.Message}";
                        res.StatusCode = 400;
                        Console.WriteLine($"WARN - {ex.Message}");
                    }
                }
                else
                {
                    res.Content = "Unauthorized";
                    res.StatusCode = 401;
                }

                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });
            #endregion

            #region DELETE endpoints

            app.Delete(@"^/identities/(\d+)/authentications/(\d+)$", async (req, res) =>
            {
                int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/(\d+)$")[1];
                int authId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/(\d+)$")[2];

                // Authorized request
                var auth = HttpAuth.AuthenticateHttpRequest(db, req.UserIdentity, HttpAuth.AccessLevel.Identity, identityId);
                if (auth != null)
                {
                    try
                    {
                        db.BeginTransaction();

                        var authentication = db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"id={authId}");
                        if (authentication != null)
                        {
                            db.Delete(DatabaseHelper.Tables.authentications, authentication);
                        }

                        db.CompleteTransaction();

                        res.Content = $"Authentication {authId} for Identity {identityId} deleted.";
                        res.StatusCode = 204;
                    }
                    catch (Exception ex)
                    {
                        db.AbortTransaction();

                        res.Content = $"Authentication delete for Identity {identityId} failed: {ex.Message}";
                        res.StatusCode = 304;
                        Console.WriteLine($"WARN - {ex.Message}");
                    }
                }
                else
                {
                    res.Content = "Unauthorized";
                    res.StatusCode = 401;
                }

                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });


            #endregion

            app.Get(@"^/test$", async (req, res) =>
            {
                res.Content = "<p>You did a GET</p>";
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Post(@"^/test$", async (req, res) =>
            {
                res.Content = "<p>You did a POST: <pre>" + await req.GetBodyAsync() + "</pre></p>";
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Put(@"^/test$", async (req, res) =>
            {
                res.Content = "<p>You did a PUT: <pre>" + await req.GetBodyAsync() + "</pre></p>";
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Delete(@"^/test$", async (req, res) =>
            {
                res.Content = "<p>You did a DELETE: <pre>" + await req.GetBodyAsync() + "</pre></p>";
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });


            // Start listener
            app.Start(config.ListenGateway, config.ListenPort);
        }
    }
}
