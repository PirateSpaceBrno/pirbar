using JamesWright.SimpleHttp;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PirBanka.Server.Responses
{
    public class AuthDelete
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
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account, identityId);
                    if (auth != null)
                    {
                        try
                        {
                            Server.db.BeginTransaction();

                            var authentication = Server.db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"id={authId}");
                            if (authentication != null)
                            {
                                Server.db.Delete(DatabaseHelper.Tables.authentications, authentication);
                            }

                            Server.db.CompleteTransaction();

                            res.Content = $"Authentication {authId} for Identity {identityId} deleted.";
                            res.StatusCode = StatusCodes.Success.NoContent;
                        }
                        catch (Exception ex)
                        {
                            Server.db.AbortTransaction();

                            res.Content = $"Authentication delete for Identity {identityId} failed: {ex.Message}";
                            res.StatusCode = StatusCodes.Redirection.NotModified;
                            Console.WriteLine($"WARN - {ex.Message}");
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = "text/html; charset=utf-8";
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/markets/(\d+)/authentications/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/markets/(\d+)/authentications/(\d+)$")[1];
                    int accId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/markets/(\d+)/authentications/(\d+)$")[2];
                    int authId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/markets/(\d+)/authentications/(\d+)$")[3];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Account, identityId);
                    if (auth != null)
                    {
                        try
                        {
                            Server.db.BeginTransaction();

                            var authentication = Server.db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"id={authId} AND identity={identityId} AND account={accId}");
                            if (authentication != null)
                            {
                                Server.db.Delete(DatabaseHelper.Tables.authentications, authentication);
                            }

                            Server.db.CompleteTransaction();

                            res.Content = $"Authentication {authId} for Market {accId} deleted.";
                            res.StatusCode = StatusCodes.Success.NoContent;
                        }
                        catch (Exception ex)
                        {
                            Server.db.AbortTransaction();

                            res.Content = $"Authentication delete for Market {accId} failed: {ex.Message}";
                            res.StatusCode = StatusCodes.Redirection.NotModified;
                            Console.WriteLine($"WARN - {ex.Message}");
                        }
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                    }

                    res.ContentType = "text/html; charset=utf-8";
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
