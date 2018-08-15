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
    public class AdminPost
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
                @"^/currencies$",
                new Action<Request, Response>( async (req, res) =>
                {
                    // Authorized request
                    var auth = HttpAuth.AuthenticateHttpRequest(req.UserIdentity, HttpAuth.AccessLevel.Administrator);
                    if (auth != null)
                    {
                        var request = await req.GetBodyAsync();

                        try
                        {
                            Server.db.BeginTransaction();

                            var currencyCreate = JsonHelper.DeserializeObject<CurrenciesCreate>(request);
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
