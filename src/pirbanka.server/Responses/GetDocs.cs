using JamesWright.SimpleHttp;
using Newtonsoft.Json;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Get;
using PirBanka.Server.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PirBanka.Server.Responses
{
    public class GetDocs
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
                @"^/doc/authenticate$",
                new Action<Request, Response>( async (req, res) =>
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
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/create$",
                new Action<Request, Response>( async (req, res) =>
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
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)/authentications/create$",
                new Action<Request, Response>( async (req, res) =>
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
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)/authentications/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
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
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/currencies/create$",
                new Action<Request, Response>( async (req, res) =>
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
