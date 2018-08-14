using JamesWright.SimpleHttp;
using Newtonsoft.Json;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PirBanka.Server.Responses
{
    public class Get
    {
        public static KeyValuePair<Regex, Action<Request, Response>> Action(string endpoint)
        {
            var action = Actions.FirstOrDefault(x => x.Key == endpoint);

            if(!action.Equals(new KeyValuePair<string, Action<Request, Response>>()))
            {
                return new KeyValuePair<Regex, Action<Request, Response>>(new Regex(action.Key), action.Value);
            }

            return new KeyValuePair<Regex, Action<Request, Response>>();
        }

        public static Dictionary<string, Action<Request, Response>> Actions = new Dictionary<string, Action<Request, Response>>()
        {
            {
                @"^/currencies$",
                new Action<Request, Response>( async (req, res) =>
                {
                    List<Currency> result = Server.db.GetList<Currency>(DatabaseHelper.Tables.currencies);

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/currencies/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var match = Regex.Match(req.Endpoint, @"^/currencies/(\d+)$", RegexOptions.IgnoreCase);
                    int id = Convert.ToInt32(match.Groups[1].Value);

                    Currency result = Server.db.Get<CurrencyView>(DatabaseHelper.Tables.currencies_view, $"id={id}");

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities$",
                new Action<Request, Response>( async (req, res) =>
                {
                    List<Identity> result = Server.db.GetList<Identity>(DatabaseHelper.Tables.identities);

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)$", RegexOptions.IgnoreCase);
                    int id = Convert.ToInt32(match.Groups[1].Value);

                    Identity result = Server.db.Get<Identity>(DatabaseHelper.Tables.identities, $"id={id}");

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)/accounts$", RegexOptions.IgnoreCase);
                    var id = match.Groups[1].Value;

                    List<Account> result = Server.db.GetList<Account>(DatabaseHelper.Tables.accounts, $"identity={id}");

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)$", RegexOptions.IgnoreCase);
                    var id = match.Groups[1].Value;
                    var account_id = match.Groups[2].Value;

                    Account result = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={id} and id={account_id}");

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)/transactions$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions$", RegexOptions.IgnoreCase);
                    var account_id = match.Groups[2].Value;

                    List<Transact> result = Server.db.GetList<Transact>(DatabaseHelper.Tables.transactions, $"source_account={account_id} or target_account={account_id}");

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)/transactions/incoming$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions/incoming$", RegexOptions.IgnoreCase);
                    var account_id = match.Groups[2].Value;

                    List<Transact> result = Server.db.GetList<Transact>(DatabaseHelper.Tables.transactions, $"target_account={account_id}");

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)/transactions/outgoing$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var match = Regex.Match(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions/outgoing$", RegexOptions.IgnoreCase);
                    var account_id = match.Groups[2].Value;

                    List<Transact> result = Server.db.GetList<Transact>(DatabaseHelper.Tables.transactions, $"source_account={account_id}");

                    res.Content = JsonConvert.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/authentications$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/authentications$")[1];

                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(Server.db, req.UserIdentity, HttpAuth.AccessLevel.Identity, id);
                    if (auth != null)
                    {
                        List<Authentication> result = Server.db.GetList<Authentication>(DatabaseHelper.Tables.authentications, $"identity={id}");

                        res.Content = JsonConvert.SerializeObject(result);
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
