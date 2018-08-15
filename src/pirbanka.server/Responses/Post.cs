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
    public class Post
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
                @"^/identities$",
                new Action<Request, Response>( async (req, res) =>
                {
                    var request = await req.GetBodyAsync();

                    try
                    {
                        Server.db.BeginTransaction();

                        var identitiesCreate = JsonHelper.DeserializeObject<IdentitiesCreate>(request);
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
            //{
            //    @"",
            //    new Action<Request, Response>( async (req, res) =>
            //    {

            //    }
            //)},
        };
    }
}
