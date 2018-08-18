using JamesWright.SimpleHttp;
using PirBanka.Server.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PirBanka.Server.Responses
{
    public class Misc
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
                @"^/$",
                new Action<Request, Response>(
                     async (req, res) =>
                    {
                        string newContent = "<div>";
                        newContent += "<p>" +
                            "Welcome to API service for PirBanka. We provides endpoints to manage your PirBanka account (get data, send transactions, pay for goods, etc.) so you can create your own PirBanka client.<br />" +
                            "All GET listing requests returns JSON object.<br />" +
                            "Documentation for POST/PUT/DELETE request type endpoints are reachable via GET request on the route /doc/{endpoint}, which will return HTML page." +
                            "</p>";

                        newContent += SortJoinRoutes(Responses.GetDocs.Actions.Keys, "Documentation", true);
                        newContent += SortJoinRoutes(Responses.Get.Actions.Keys, "GET");
                        newContent += SortJoinRoutes(Responses.Post.Actions.Keys, "POST");
                        newContent += SortJoinRoutes(Responses.Put.Actions.Keys, "PUT");
                        newContent += SortJoinRoutes(Responses.Delete.Actions.Keys, "DELETE");
                        newContent += SortJoinRoutes(Responses.AuthGet.Actions.Keys, "AUTH GET");
                        newContent += SortJoinRoutes(Responses.AuthPost.Actions.Keys, "AUTH POST");
                        newContent += SortJoinRoutes(Responses.AuthPut.Actions.Keys, "AUTH PUT");
                        newContent += SortJoinRoutes(Responses.AuthDelete.Actions.Keys, "AUTH DELETE");
                        newContent += SortJoinRoutes(Responses.AdminGet.Actions.Keys, "ADMIN GET");
                        newContent += SortJoinRoutes(Responses.AdminPost.Actions.Keys, "ADMIN POST");
                        newContent += SortJoinRoutes(Responses.AdminPut.Actions.Keys, "ADMIN PUT");
                        newContent += SortJoinRoutes(Responses.AdminDelete.Actions.Keys, "ADMIN DELETE");

                        newContent += "</div>";



                        res.Content = TextHelper.GetApiPage(newContent);
                        res.ContentType = ContentTypes.Html;
                        await res.SendAsync();
                    })
            },
            {
                @"^/api-style.css$",
                async (req, res) =>
                {
                    res.Content = File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}api-docs{Path.DirectorySeparatorChar}api-style.css");
                    res.ContentType = ContentTypes.Css;
                    await res.SendAsync();
                }
            },
            {
                @"^/favicon.svg$",
                async (req, res) =>
                {
                    res.Content = File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}api-docs{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pirbanka_icon.svg");
                    res.ContentType = ContentTypes.Svg;
                    await res.SendAsync();
                }
            }
        };

        private static string SortJoinRoutes(Dictionary<string, Action<Request, Response>>.KeyCollection keys, string groupName, bool makeLinks = false)
        {
            if (keys.Count > 0)
            {
                string newContent = $"<b>{groupName} endpoints:</b>";
                newContent += "<pre>";

                if (makeLinks)
                {
                    var keysArr = keys.Select(
                        x => $"<a href=\"" +
                        $"{x.ToString().Replace("^", "").Replace("$", "").Replace(@"(\d+)", "0")}" +
                        $"\">" +
                        $"{x.ToString().Replace("^", "").Replace("$", "").Replace(@"(\d+)", "ID")}" +
                        $"</a>")
                        .ToArray();
                    Array.Sort(keysArr);

                    newContent += string.Join("<br />", keysArr);
                }
                else
                {
                    var keysArr = keys.Select(
                        x => x.ToString()
                        .Replace("^", "")
                        .Replace("$", "")
                        .Replace(@"(\d+)", "ID"))
                        .ToArray();
                    Array.Sort(keysArr);

                    newContent += string.Join("<br />", keysArr);
                }

                newContent += "</pre>";
                return newContent;
            }
            return "";
        }
    }
}
