using JamesWright.SimpleHttp;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Db;
using PirBanka.Server.Models.Get;
using PirBanka.Server.Models.Post;
using PirBanka.Server.Models.Put;
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

            return new KeyValuePair<Regex, Action<Request, Response>>(new Regex(endpoint), JamesWright.SimpleHttp.Actions.Error404);
        }

        public static Dictionary<string, Action<Request, Response>> Actions = new Dictionary<string, Action<Request, Response>>()
        {
            {
                @"^/doc/auth/token$",
                new Action<Request, Response>( async (req, res) =>
                {
                    string newContent = "<div>";
                    newContent += "<h2>Authorization - get Token</h2>";
                    newContent += "<p>You can create token with 15 minutes expiration for future operations by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: GET\n";
                    newContent += "endpoint: /auth/token\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "response: application/json\n";
                    newContent += $"response content: {JsonHelper.SerializeObject(new Token() { token = "token value", authentications_id = 0, expiration = DateTime.Now })}\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities$",
                new Action<Request, Response>( async (req, res) =>
                {
                    string newContent = "<div>";
                    newContent += "<h2>Create new identity</h2>";
                    newContent += "<p>You can create new identity by following request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += "endpoint: /identities\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new IdentitiesCreate() { name = "Your unique username", password = "Your password" })}\n";
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
                @"^/doc/identities/(\d+)/authentications$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/authentications$")[1];

                    string newContent = "<div>";
                    newContent += $"<h2>Create Authentication for Identity {id}</h2>";
                    newContent += "<p>You can create new Identity Authentication by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += $"endpoint: /identities/{id}/authentications\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new IdentitiesAuthCreate() { password = "Your new password" })}\n";
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
                    newContent += "request type: PUT\n";
                    newContent += $"endpoint: /identities/{identityId}/authentications/{authId}\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new IdentitiesAuthCreate() { password = "Your new password" })}\n";
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
                @"^/doc/currencies$",
                new Action<Request, Response>( async (req, res) =>
                {
                    string newContent = "<div>";
                    newContent += $"<h2>Create new Currency</h2>";
                    newContent += "<p>You can create new currency by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += "endpoint: /currencies\n";
                    newContent += "header: Authorization with Administrator access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new CurrenciesCreate() { name = "Currency display name", shortname = "Curr" })}\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/auth$",
                new Action<Request, Response>( async (req, res) =>
                {
                    string newContent = "<div>";
                    newContent += "<h2>Authorization</h2>";
                    newContent += "<p>Some requests needs authorization. You can authorize your request by sending Authorization header.</p>";
                    newContent += "<p>";
                    newContent += "<b>Authorization with Username and Password:</b>";
                    newContent += "<pre>";
                    newContent += "header[Authorization] = \"BASIC Username:Password\"\n";
                    newContent += "String Username:Password is encoded with BASE64";
                    newContent += "</pre>";
                    newContent += "<b>Authorization with Token:</b>";
                    newContent += "<pre>";
                    newContent += "header[Authorization] = \"TOKEN TokenValue\"\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)/accounts$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/accounts$")[1];

                    string newContent = "<div>";
                    newContent += $"<h2>Create Account for Identity {id}</h2>";
                    newContent += "<p>You can create new Account by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += $"endpoint: /identities/{id}/accounts\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new AccountsCreate() { currencyId = 0, description = "My account" })}\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)/accounts/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/accounts/(\d+)$")[1];
                    int accId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/accounts/(\d+)$")[2];

                    string newContent = "<div>";
                    newContent += $"<h2>Update Account description</h2>";
                    newContent += "<p>You can update Account description by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: PUT\n";
                    newContent += $"endpoint: /identities/{id}/accounts/{accId}\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new AccountsUpdate() { description = "My account" })}\n";
                    newContent += "</pre>";
                    newContent += "NOTE! General account description cannot be changed.";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)/accounts/(\d+)/transactions$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/accounts/(\d+)/transactions$")[1];
                    int accId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/accounts/(\d+)/transactions$")[2];

                    string newContent = "<div>";
                    newContent += $"<h2>Send transaction</h2>";
                    newContent += "<p>You can send transaction from this account by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += $"endpoint: /identities/{id}/accounts/{accId}/transactions\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new TransactCreate() { targetAccountIdentifier = "1/420", amount = 200 })}\n";
                    newContent += "</pre>";
                    newContent += "Target account must be in the same currency as source account.<br />";
                    newContent += "Transaction amount cannot be bigger than source account balance.";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)/accounts/(\d+)/deposit$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/accounts/(\d+)/deposit$")[1];
                    int accId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/accounts/(\d+)/deposit$")[2];

                    string newContent = "<div>";
                    newContent += $"<h2>Deposit</h2>";
                    newContent += "<p>You can deposit funds to this account by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += $"endpoint: /identities/{id}/accounts/{accId}/deposit\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new TransactBase() { amount = 200 })}\n";
                    newContent += "</pre>";
                    newContent += "With the deposit submitted, put equivalent funds into the bank chest.";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)/accounts/(\d+)/withdrawal$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/accounts/(\d+)/withdrawal$")[1];
                    int accId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/accounts/(\d+)/withdrawal$")[2];

                    string newContent = "<div>";
                    newContent += $"<h2>Withdrawal</h2>";
                    newContent += "<p>You can withdrawal funds from this account by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += $"endpoint: /identities/{id}/accounts/{accId}/withdrawal\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new TransactBase() { amount = 200 })}\n";
                    newContent += "</pre>";
                    newContent += "With the withdrawal submitted, take equivalent funds from the bank chest.";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/currencies/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/currencies/(\d+)$")[1];

                    string newContent = "<div>";
                    newContent += $"<h2>Update Currency {id}</h2>";
                    newContent += "<p>You can update currency by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: PUT\n";
                    newContent += $"endpoint: /currencies/{id}\n";
                    newContent += "header: Authorization with Administrator access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += "<b>Update only name</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new CurrenciesUpdate() { name = "Currency new fancy name", shortname = "" })}\n";
                    newContent += "<b>Update only shortname</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new CurrenciesUpdate() { name = "", shortname = "Currency new shortname" })}\n";
                    newContent += "<b>Update both name and shortname</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new CurrenciesUpdate() { name = "Currency new fancy name", shortname = "Currency new shortname" })}\n";
                    newContent += "<b>Update only exchange rate</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new CurrenciesUpdate() { name = "", shortname = "", exchangeRate = 2 })}\n";
                    newContent += "<b>Update all</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new CurrenciesUpdate() { name = "Currency new fancy name", shortname = "Currency new shortname", exchangeRate = 2 })}\n";
                    newContent += "</pre>";
                    newContent += "NOTE! Name and shortname will be updated if their value is not empty or the same as actual.<br />";
                    newContent += "Exchange rate will be updated if its value is not null or negative.\n";
                    newContent += "Exchange rate cannot be updated for general currency.";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)/markets$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/markets$")[1];

                    string newContent = "<div>";
                    newContent += $"<h2>Create Market for Identity {id}</h2>";
                    newContent += "<p>You can create new Market by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += $"endpoint: /identities/{id}/markets\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new MarketsCreate() { currencyId = 0, description = "My market", password = "NFC tag, barcode, etc." })}\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)/markets/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/markets/(\d+)$")[1];
                    int accId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/markets/(\d+)$")[2];

                    string newContent = "<div>";
                    newContent += $"<h2>Update Market description</h2>";
                    newContent += "<p>You can update Market description by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: PUT\n";
                    newContent += $"endpoint: /identities/{id}/markets/{accId}\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new AccountsUpdate() { description = "My market" })}\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/markets/(\d+)/buy$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/markets/(\d+)/buy$")[1];

                    string newContent = "<div>";
                    newContent += $"<h2>Buy from Market {id}</h2>";
                    newContent += "<p>You can buy stuff from Market by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += $"endpoint: /markets/{id}/buy\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += "<b>Buy with withdrawal:</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new MarketsBuy() { amount = 1, paymentFromAccountIdentifier = "Identifier of your account", paymentToAccountIdentifier = "Identifier of market account in the same currency as payment" })}\n";
                    newContent += "<b>Buy with transfer to account:</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new MarketsBuy() { amount = 1, paymentFromAccountIdentifier = "Identifier of your account", paymentToAccountIdentifier = "Identifier of market account in the same currency as payment", targetAccountIdentifier = "Identifier of your account in market currency" })}\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/markets/buy$",
                new Action<Request, Response>( async (req, res) =>
                {
                    string newContent = "<div>";
                    newContent += $"<h2>Buy from Market</h2>";
                    newContent += "<p>You can buy stuff from Market by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += $"endpoint: /markets/buy\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += "<b>Buy with withdrawal:</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new MarketsBuy() { amount = 1, paymentFromAccountIdentifier = "Identifier of your account", paymentToAccountIdentifier = "Identifier of market account in the same currency as payment", marketPassword = "Stored market account password" })}\n";
                    newContent += "<b>Buy with transfer to account:</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new MarketsBuy() { amount = 1, paymentFromAccountIdentifier = "Identifier of your account", paymentToAccountIdentifier = "Identifier of market account in the same currency as payment", targetAccountIdentifier = "Identifier of your account in market currency", marketPassword = "Stored market account password" })}\n";
                    newContent += "</pre>";
                    newContent += "This option is used to identify market account via its stored authentication - e.g. barcode, nfc tag, etc.";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)/markets/(\d+)/authentications$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint,@"^/doc/identities/(\d+)/markets/(\d+)/authentications$")[1];
                    int marketId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/markets/(\d+)/authentications/(\d+)$")[2];

                    string newContent = "<div>";
                    newContent += $"<h2>Create Authentication for Market {id}</h2>";
                    newContent += "<p>You can create new Market Authentication by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += $"endpoint: /identities/{id}/markets/{marketId}/authentications\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new IdentitiesAuthCreate() { password = "Your new password" })}\n";
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
                @"^/doc/identities/(\d+)/markets/(\d+)/authentications/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int identityId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/markets/(\d+)/authentications/(\d+)$")[1];
                    int marketId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/markets/(\d+)/authentications/(\d+)$")[2];
                    int authId = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)/markets/(\d+)/authentications/(\d+)$")[3];

                    string newContent = "<div>";
                    newContent += $"<h2>Update Authentication</h2>";
                    newContent += "<p>You can update this Market Authentication by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: PUT\n";
                    newContent += $"endpoint: /identities/{identityId}/markets/{marketId}/authentications/{authId}\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new IdentitiesAuthCreate() { password = "Your new password" })}\n";
                    newContent += "</pre>";
                    newContent += "Password is stored as SHA512 hash.";
                    newContent += "</p>";

                    newContent += $"<h2>Delete Authentication</h2>";
                    newContent += "<p>You can delete this Identity Authentication by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: DELETE\n";
                    newContent += $"endpoint: /identities/{identityId}/markets/{marketId}/authentications/{authId}\n";
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
                @"^/doc/markets/(\d+)/sell$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/markets/(\d+)/sell$")[1];

                    string newContent = "<div>";
                    newContent += $"<h2>Sell to Market {id}</h2>";
                    newContent += "<p>You can sell stuff to Market by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: POST\n";
                    newContent += $"endpoint: /markets/{id}/sell\n";
                    newContent += "header: Authorization with Identity access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += "<b>Sell with deposit:</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new MarketsSell() { amount = 1, paymentFromAccountIdentifier = "Identifier of market account to pay", paymentToAccountIdentifier = "Identifier of your account in the same currency as payment" })}\n";
                    newContent += "<b>Sell with transfer from account:</b>\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new MarketsSell() { amount = 1, paymentFromAccountIdentifier = "Identifier of market account to pay", paymentToAccountIdentifier = "Identifier of your account in the same currency as payment", sourceAccountIdentifier = "Identifier of your selling account in market currency" })}\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/identities/(\d+)$",
                new Action<Request, Response>( async (req, res) =>
                {
                    int id = TextHelper.GetUriIds(req.Endpoint, @"^/doc/identities/(\d+)$")[1];

                    string newContent = "<div>";
                    newContent += $"<h2>Update Identity {id}</h2>";
                    newContent += "<p>You can update Identity by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: PUT\n";
                    newContent += $"endpoint: /identities/{id}\n";
                    newContent += "header: Authorization with Administrator access\n";
                    newContent += "content type: application/json; charset=utf8\n";
                    newContent += $"content: {JsonHelper.SerializeObject(new IdentitiesUpdate() { admin = false, displayName = "Display name", name = "Login" })}\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/auth/identify$",
                new Action<Request, Response>( async (req, res) =>
                {
                    string newContent = "<div>";
                    newContent += "<h2>Get Authorization identity/account</h2>";
                    newContent += "<p>You can see which Account or Identity is identified by provided Authentication by following authorized request:";
                    newContent += "<pre>";
                    newContent += "request type: GET\n";
                    newContent += "endpoint: /auth/identify\n";
                    newContent += "header: Authorization with Account access\n";
                    newContent += "response: application/json\n";
                    newContent += "<b>Authentication is for Account:</b>\n";
                    newContent += $"response content: {JsonHelper.SerializeObject(new AccountView() { account_identifier = "10001/1", balance = 2, currency_id = 1, id = 1, identity = 1})}\n";
                    newContent += "<b>Authentication is for Identity:</b>\n";
                    newContent += $"response content: {JsonHelper.SerializeObject(new Identity() { name = "Identity", display_name="Display name", id = 1})}\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/models$",
                new Action<Request, Response>( async (req, res) =>
                {
                    string newContent = "<div>";
                    newContent += "<h2>Models returned in GET requests</h2>";
                    newContent += "Get sample model by GET request on /doc/models/modelName.";
                    newContent += "<pre>";
                    newContent += "response: application/json\n";
                    newContent += "</pre>";
                    newContent += "</p>";
                    newContent += "</div>";

                    res.Content = TextHelper.GetApiPage(newContent);
                    res.ContentType = ContentTypes.Html;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/models/identities$",
                new Action<Request, Response>( async (req, res) =>
                {
                    res.Content = JsonHelper.SerializeObject(new Identity() { name = "Identity", display_name="Display name", id = 1});
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/models/accounts$",
                new Action<Request, Response>( async (req, res) =>
                {
                    res.Content = JsonHelper.SerializeObject(new AccountView() { account_identifier = "10001/1", balance = 2, currency_id = 1, id = 1, identity = 1});
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/models/currencies$",
                new Action<Request, Response>( async (req, res) =>
                {
                    res.Content = JsonHelper.SerializeObject(new CurrencyView() { id=1, name = "Currency name", shortname = "TEST", rate = 0});
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/models/transactions$",
                new Action<Request, Response>( async (req, res) =>
                {
                    res.Content = JsonHelper.SerializeObject(new Transact() { id=1, amount = 100, source_account = 1, target_account = 2});
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/models/exchangeRates$",
                new Action<Request, Response>( async (req, res) =>
                {
                    res.Content = JsonHelper.SerializeObject(new ExchangeRates() {id=1, rate=0, currency=1});
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/models/authentications$",
                new Action<Request, Response>( async (req, res) =>
                {
                    res.Content = JsonHelper.SerializeObject(new Authentication() {id=1, content = "SHA512 HASH", identity=1});
                    res.ContentType = ContentTypes.Json;
                    await res.SendAsync();
                }
            )},
            {
                @"^/doc/models/token$",
                new Action<Request, Response>( async (req, res) =>
                {
                    res.Content = JsonHelper.SerializeObject(new Token() {authentications_id=1,token="TOKEN value"});
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
