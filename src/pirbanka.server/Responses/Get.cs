using JamesWright.SimpleHttp;
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

            return new KeyValuePair<Regex, Action<Request, Response>>(new Regex(endpoint), JamesWright.SimpleHttp.Actions.Error404);
        }

        public static Dictionary<string, Action<Request, Response>> Actions = new Dictionary<string, Action<Request, Response>>()
        {
            {
                @"^/currencies$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var result = Server.db.GetList<Currency>(DatabaseHelper.Tables.currencies);

                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/currencies/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/currencies/(\d+)$")[1];

                    var result = Server.db.Get<CurrencyView>(DatabaseHelper.Tables.currencies_view, $"id={id}");

                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var result = Server.db.GetList<Identity>(DatabaseHelper.Tables.identities);

                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint,  @"^/identities/(\d+)$")[1];

                    var result = Server.db.Get<Identity>(DatabaseHelper.Tables.identities, $"id={id}");

                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint,  @"^/identities/(\d+)/accounts$")[1];

                    var result = Server.db.GetList<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={id}");

                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint,  @"^/identities/(\d+)/accounts/(\d+)$")[1];
                    int account_id = TextHelper.GetUriIds(req.Endpoint,  @"^/identities/(\d+)/accounts/(\d+)$")[2];

                    var result = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={id} and id={account_id}");

                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)/transactions$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions$")[1];
                    int accId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions$")[2];

                    var account = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={identityId} and id={accId}");
                    if(account != null)
                    {
                        var result = Server.db.GetList<Transact>(DatabaseHelper.Tables.transactions, $"source_account={accId} or target_account={accId}");
                        res.Content = JsonHelper.SerializeObject(result);
                        res.ContentType = ContentTypes.Json;
                    }
                    else {
                        res.Content = JsonHelper.SerializeObject(null);
                        res.ContentType = ContentTypes.Json;
                    }

                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)/transactions/incoming$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions/incoming$")[1];
                    int accId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions/incoming$")[2];

                    var account = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={identityId} and id={accId}");
                    if(account != null)
                    {
                        var result = Server.db.GetList<Transact>(DatabaseHelper.Tables.transactions, $"target_account={accId}");
                        res.Content = JsonHelper.SerializeObject(result);
                        res.ContentType = ContentTypes.Json;
                    }
                    else {
                        res.Content = JsonHelper.SerializeObject(null);
                        res.ContentType = ContentTypes.Json;
                    }

                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/accounts/(\d+)/transactions/outgoing$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions/outgoing$")[1];
                    int accId = TextHelper.GetUriIds(req.Endpoint, @"^/identities/(\d+)/accounts/(\d+)/transactions/outgoing$")[2];

                    var account = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={identityId} and id={accId}");
                    if(account != null)
                    {
                        var result = Server.db.GetList<Transact>(DatabaseHelper.Tables.transactions, $"source_account={accId}");
                        res.Content = JsonHelper.SerializeObject(result);
                        res.ContentType = ContentTypes.Json;
                    }
                    else {
                        res.Content = JsonHelper.SerializeObject(null);
                        res.ContentType = ContentTypes.Json;
                    }
                    
                    await res.SendAsync();
                }
            )},
            {
                @"^/currencies/(\d+)/exchangeRates$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/currencies/(\d+)/exchangeRates$")[1];

                    var result = Server.db.GetList<ExchangeRates>(DatabaseHelper.Tables.exchange_rates, $"currency={id}");
                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/markets$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var result = Server.db.GetList<AccountView>(DatabaseHelper.Tables.accounts_view, $"market=1");

                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/markets/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int account_id = TextHelper.GetUriIds(req.Endpoint,  @"^/markets/(\d+)$")[1];

                    var result = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"market=1 and id={account_id}");

                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/markets$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint,  @"^/identities/(\d+)/markets$")[1];

                    var result = Server.db.GetList<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={id} AND market=1");

                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/identities/(\d+)/markets/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint,  @"^/identities/(\d+)/markets/(\d+)$")[1];
                    int account_id = TextHelper.GetUriIds(req.Endpoint,  @"^/identities/(\d+)/markets/(\d+)$")[2];

                    var result = Server.db.Get<AccountView>(DatabaseHelper.Tables.accounts_view, $"identity={id} and id={account_id} and market=1");

                    res.Content = JsonHelper.SerializeObject(result);
                    res.ContentType = ContentTypes.Json;
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
