using JamesWright.SimpleHttp;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PirBanka.Server.Responses
{
    public class AuthGet
    {
        public static KeyValuePair<Regex, Action<Request, Response>> Action(string endpoint)
        {
            var action = Actions.FirstOrDefault(x => x.Key == endpoint);

            if(!action.Equals(new KeyValuePair<string, Action<Request, Response>>()))
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
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications$")[1];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Identity, identityId);
                    if (auth != null)
                    {
                        List<Authentication> result = Server.db.GetList<Authentication>(DatabaseHelper.Tables.authentications, $"identity={identityId}");

                        res.Content = JsonHelper.SerializeObject(result);
                        res.ContentType = ContentTypes.Json;
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                        res.ContentType = ContentTypes.Html;
                    }


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
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Identity, identityId);
                    if (auth != null)
                    {
                        Authentication result = Server.db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"identity={identityId} AND id={authId}");

                        res.Content = JsonHelper.SerializeObject(result);
                        res.ContentType = ContentTypes.Json;
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                        res.ContentType = ContentTypes.Html;
                    }


                    await res.SendAsync();
                }
            )},
            {
                @"^/auth/token$",
                new Action<Request, Response>( async (req, res) =>
                {
                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Identity);
                    if (auth != null)
                    {
                        var token = HttpAuth.CreateToken(req.UserIdentity, auth);

                        res.Content = JsonHelper.SerializeObject(token);
                        res.ContentType = ContentTypes.Json;
                    }
                    else
                    {
                        res.Content = "Unauthorized";
                        res.StatusCode = StatusCodes.ClientError.Unauthorized;
                        res.ContentType = ContentTypes.Html;
                    }

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
