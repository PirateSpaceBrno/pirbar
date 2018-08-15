using JamesWright.SimpleHttp;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Db;
using PirBanka.Server.Models.Post;
using PirBanka.Server.Models.Put;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PirBanka.Server.Responses
{
    public class AuthPut
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
                @"^/identities/(\d+)/authentications/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/(\d+)$")[1];
                    int authId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications/(\d+)$")[2];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Identity, identityId);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<IdentitiesAuthCreate>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                var authentication = Server.db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"id={authId} AND identity={identityId}");
                                if(authentication != null)
                                {
                                    authentication.content = TextHelper.SHA512(requestContent.password);
                                    authentication.created = DateTime.Now;
                                    Server.db.Update(DatabaseHelper.Tables.authentications, authentication);
                                }
                                else
                                {
                                    throw new Exception("Trying to update nonexisting Authentication.");
                                }

                                Server.db.CompleteTransaction();

                                res.Content = $"Authentication for Identity {identityId} updated.";
                                res.StatusCode = StatusCodes.Success.Created;
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
                @"^/identities/(\d+)/accounts/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)$")[1];
                    int accId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)$")[2];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Identity, identityId);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();
                        var requestContent = JsonHelper.DeserializeObject<AccountsUpdate>(request);

                        if (requestContent != null)
                        {
                            try
                            {
                                Server.db.BeginTransaction();

                                var account = Server.db.Get<Account>(DatabaseHelper.Tables.accounts, $"id={accId} AND identity={identityId}");
                                if
                                (
                                    account != null
                                    && !account.description.StartsWith("general", StringComparison.InvariantCultureIgnoreCase)
                                    && !account.description.StartsWith("outsideworld", StringComparison.InvariantCultureIgnoreCase)
                                )
                                {
                                    account.description = TextHelper.SHA512(requestContent.description);
                                    Server.db.Update(DatabaseHelper.Tables.authentications, account);

                                    Server.db.CompleteTransaction();

                                    res.Content = $"Account description updated.";
                                    res.StatusCode = StatusCodes.Success.Created;
                                }
                                else
                                {
                                    throw new Exception("General account cannot be updated.");
                                }
                            }
                            catch (Exception ex)
                            {
                                Server.db.AbortTransaction();

                                res.Content = $"Account description update failed.";
                                res.StatusCode = StatusCodes.Redirection.NotModified;
                                Console.WriteLine($"WARN - {ex.Message}");
                            }
                        }
                        else
                        {
                            res.Content = $"Invalid data for Account description update.";
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
            //)}
        };
    }
}
