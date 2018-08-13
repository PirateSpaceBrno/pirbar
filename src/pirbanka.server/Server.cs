using HtmlAgilityPack;
using JamesWright.SimpleHttp;
using MMLib.Extensions;
using Newtonsoft.Json;
using PetaPoco;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Get;
using PirBanka.Server.Models.Db;
using PirBanka.Server.Models.Post;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace PirBanka.Server
{
    class Server
    {
        private static PirBankaConfig config;
        private static DatabaseHelper db;

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
            SimpleHttpServer app = new SimpleHttpServer();

            #region GET endpoints returning HTML and misc
            app.Get(@"^/$", async (req, res) =>
            {
                string newContent = "<div>";
                newContent += "<p>" +
                    "Welcome to API service for PirBanka. We provides endpoints to manage your PirBanka account (get data, send transactions, pay for goods, etc.) so you can create your own PirBanka client.<br />" +
                    "All GET listing requests returns JSON object.<br />" +
                    "Documentation for POST/PUT/DELETE request type endpoints are reachable via GET request on the same endpoint - this returns HTML page." +
                    "</p>";
                newContent += "<b>GET endpoints:</b><br />";
                newContent += "<pre>";
                var listGetRoutes = app.RouteRepository.Get.Keys;
                newContent += string.Join("<br />", listGetRoutes.ToList()).Replace("^", "").Replace("$","").Replace(@"(\d+)", "ID");
                newContent += "</pre>";

                newContent += "<b>POST endpoints:</b><br />";
                newContent += "<pre>";
                var listPostRoutes = app.RouteRepository.Post.Keys;
                newContent += string.Join("<br />", listPostRoutes.ToList()).Replace("^", "").Replace("$", "").Replace(@"(\d+)", "ID");
                newContent += "</pre>";

                newContent += "<b>PUT endpoints:</b><br />";
                newContent += "<pre>";
                var listPutRoutes = app.RouteRepository.Put.Keys;
                newContent += string.Join("<br />", listPutRoutes.ToList()).Replace("^", "").Replace("$", "").Replace(@"(\d+)", "ID");
                newContent += "</pre>";

                newContent += "<b>DELETE endpoints:</b><br />";
                newContent += "<pre>";
                var listDeleteRoutes = app.RouteRepository.Delete.Keys;
                newContent += string.Join("<br />", listDeleteRoutes.ToList()).Replace("^", "").Replace("$", "").Replace(@"(\d+)", "ID");
                newContent += "</pre>";

                newContent += "</div>";

                res.Content = GetApiPage(newContent);
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/api-style.css$", async (req, res) =>
            {
                res.Content = File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}api-docs{Path.DirectorySeparatorChar}api-style.css");
                res.ContentType = "text/css; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/favicon.svg$", async (req, res) =>
            {
                res.Content = File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}api-docs{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pirbanka_icon.svg");
                res.ContentType = "image/svg+xml; charset=utf-8";
                await res.SendAsync();
            });
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
                int id = GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications$")[1];

                // Authorized request
                var auth = AuthenticateHttpRequest(req.UserIdentity, AccessLevel.Identity, id);
                if (auth != null)
                {
                    List<Authentication> result = db.GetList<Authentication>(DatabaseHelper.Tables.authentications, $"identity={id}");

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = "application/json; charset=utf-8";
                }
                else
                {
                    res.Content = "Unauthorized";
                    res.ContentType = "text/html; charset=utf-8";
                }

                
                await res.SendAsync();
            });

            #endregion

            #region GET endpoints with documentation
            app.Get(@"^/authenticate$", async (req, res) =>
            {
                string newContent = "<div>";
                newContent += "<h2>Authenticate</h2>";
                newContent += "<p>You can authenticate and create token with 15 minutes expiration for future operations.<br />";
                newContent += "Token can be created by following request:";
                newContent += "<pre>";
                newContent += "request type: POST\n";
                newContent += "endpoint: /authenticate\n";
                newContent += "header: Authorize\n";
                newContent += "response: application/json\n";
                newContent += $"response content: {JsonConvert.SerializeObject(new Token() { token = "token value" })}";
                newContent += "</pre>";
                newContent += "Token can be requested just for Identities access, so UserName has to be filled and Password must correspond to Authentication record for Identity.";
                newContent += "</p>";
                newContent += "</div>";

                res.Content = GetApiPage(newContent);
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/identities/create$", async (req, res) =>
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

                res.Content = GetApiPage(newContent);
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/identities/(\d+)/authentications/create$", async (req, res) =>
            {
                int id = GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/create$")[1];

                string newContent = "<div>";
                newContent += $"<h2>Create Authentication for Identity {id}</h2>";
                newContent += "<p>You can create new Identity Authentication by following authorized request:";
                newContent += "<pre>";
                newContent += "request type: POST\n";
                newContent += $"endpoint: /identities/{id}/authentications/create\n";
                newContent += "header: Authorize with Identity access\n";
                newContent += "content type: application/json; charset=utf8\n";
                newContent += $"content: {JsonConvert.SerializeObject(new IdentitiesAuthCreate() { password = "Your new password" })}";
                newContent += "</pre>";
                newContent += "Password is stored as SHA512 hash.";
                newContent += "</p>";
                newContent += "</div>";

                res.Content = GetApiPage(newContent);
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });
            #endregion

            #region POST endpoints
            app.Post(@"^/authenticate$", async (req, res) =>
            {
                var token = CreateToken(req.UserIdentity);

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

                    IdentitiesCreate identitiesCreate = JsonConvert.DeserializeObject<IdentitiesCreate>(request);
                    Identity newIdentity = new Identity()
                    {
                        name = RemoveSpecialCharacters(identitiesCreate.name),
                        display_name = identitiesCreate.name,
                        admin = false,
                        created = DateTime.Now
                    };

                    db.Insert(DatabaseHelper.Tables.identities, newIdentity);

                    newIdentity = db.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{RemoveSpecialCharacters(newIdentity.name)}'");
                    Authentication authentication = new Authentication()
                    {
                        identity = newIdentity.id,
                        content = SHA512(identitiesCreate.password),
                        created = DateTime.Now
                    };

                    Account account = new Account()
                    {
                        currency_id = 1,
                        description = "General account",
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
                }
                catch (Exception ex)
                {
                    db.AbortTransaction();

                    res.Content = $"Identity creation failed: {ex.Message}";
                    Console.WriteLine($"WARN - {ex.Message}");
                }

                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Post(@"^/identities/(\d+)/authentications/create$", async (req, res) =>
            {
                int id = GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/create$")[1];

                // Authorized request
                var auth = AuthenticateHttpRequest(req.UserIdentity, AccessLevel.Identity, id);
                if(auth != null)
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
                                content = SHA512(requestContent.password),
                                created = DateTime.Now
                            };

                            db.Execute("SET FOREIGN_KEY_CHECKS=0;");
                            db.Insert(DatabaseHelper.Tables.authentications, authentication);
                            db.Execute("SET FOREIGN_KEY_CHECKS=1;");
                            db.CompleteTransaction();

                            res.Content = $"Authentication for Identity {id} created.";
                        }
                        catch (Exception ex)
                        {
                            db.AbortTransaction();

                            res.Content = $"Authentication creation for Identity {id} failed: {ex.Message}";
                            Console.WriteLine($"WARN - {ex.Message}");
                        }
                    }
                    else
                    {
                        res.Content = $"Invalid data for Authentication creation.";
                    }
                }
                else
                {
                    res.Content = "Unauthorized";
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

        /// <summary>
        /// Returns HTML Api page with header, css, etc.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string GetApiPage(string content)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}api-docs{Path.DirectorySeparatorChar}index.html");
            HtmlNode newNode = HtmlNode.CreateNode(content);
            doc.DocumentNode.SelectSingleNode("//body").AppendChild(newNode);
            return doc.DocumentNode.InnerHtml.Replace("PirBanka", config.InstanceName);
        }

        internal static string RemoveSpecialCharacters(string str)
        {
            str = str.RemoveDiacritics();
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        internal static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        /// <summary>
        /// Authenticates HTTP request.
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="accessLevel"></param>
        /// <returns></returns>
        internal static Authentication AuthenticateHttpRequest(NetworkCredential auth, AccessLevel accessLevel, int identityId = 0, int accountId = 0)
        {
            if (auth != null)
            {
                var authentication = db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"content='{SHA512(auth.Password)}'");

                // Check authentication expiration
                if(authentication != null && authentication.expiration != null)
                {
                    var expiration = (DateTime)authentication.expiration;
                    if(expiration.Subtract(DateTime.Now).TotalSeconds <= 0)
                    {
                        db.Delete(DatabaseHelper.Tables.authentications, authentication);
                        return null;
                    }
                }

                if (authentication != null)
                {
                    switch (auth.Domain)
                    {
                        // Credentials can be for Identity or Account (in Account case - username is empty)
                        case "creds":

                            switch (accessLevel)
                            {
                                case AccessLevel.Identity:
                                    if(authentication.account == null)
                                    {
                                        var identity = db.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{auth.UserName}'");
                                        if (identity != null && authentication.identity == identity.id)
                                        {
                                            if (identityId == 0 || (identityId != 0 && identityId == identity.id))
                                            {
                                                return authentication;
                                            }
                                        }
                                    }
                                    break;
                                case AccessLevel.Account:
                                    if(authentication.account != null)
                                    {
                                        var account = db.Get<Account>(DatabaseHelper.Tables.accounts, $"id={authentication.account}");
                                        if (account != null)
                                        {
                                            if (accountId == 0 || (accountId != 0 && accountId == account.id))
                                            {
                                                return authentication;
                                            }
                                        }
                                    }
                                    break;
                                case AccessLevel.Administrator:
                                    if (authentication.account == null)
                                    {
                                        var identity = db.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{auth.UserName}'");
                                        if (identity != null && authentication.identity == identity.id && identity.admin)
                                        {
                                            return authentication;
                                        }
                                    }
                                    break;
                            }
                            break;

                        // Token is for identity only
                        case "token":
                            switch (accessLevel)
                            {
                                case AccessLevel.Account:
                                    break;
                                case AccessLevel.Identity:
                                    if (identityId == 0 || (identityId != 0 && identityId == authentication.identity))
                                    {
                                        return authentication;
                                    }
                                    break;
                                case AccessLevel.Administrator:
                                    var identity = db.Get<Identity>(DatabaseHelper.Tables.identities, $"id='{authentication.identity}'");
                                    if (identity != null && identity.admin)
                                    {
                                        return authentication;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }

            return null;
        }

        internal static Token CreateToken(NetworkCredential auth)
        {
            if(auth != null && auth.Domain == "creds" && !string.IsNullOrEmpty(auth.UserName))
            {
                var authentication = AuthenticateHttpRequest(auth, AccessLevel.Identity);

                if(authentication != null)
                {
                    Identity ident = db.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{auth.UserName}'");

                    try
                    {
                        db.BeginTransaction();

                        var token = Guid.NewGuid().ToString();
                        var now = DateTime.Now;
                        var expiration = now.AddMinutes(15);

                        // create new token with 15 minutes expiration
                        Authentication newToken = new Authentication()
                        {
                            identity = ident.id,
                            content = SHA512(token),
                            expiration = expiration,
                            created = now
                        };
                        db.Insert(DatabaseHelper.Tables.authentications, newToken);

                        db.CompleteTransaction();

                        return new Token() { token = token, expiration = expiration };
                    }
                    catch (Exception ex)
                    {
                        db.AbortTransaction();
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return null;
        }

        internal static List<int> GetUriIds(string request, string regex)
        {
            var match = Regex.Match(request, regex, RegexOptions.IgnoreCase);
            var result = new List<int>();

            foreach(Group x in match.Groups)
            {
                try
                {
                    result.Add(Convert.ToInt32(x.Value));
                }
                catch
                {
                    result.Add(0);
                }
            }

            return result;
        }

        internal enum AccessLevel
        {
            Identity,
            Account,
            Administrator
        }
    }
}
