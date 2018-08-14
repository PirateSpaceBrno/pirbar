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

            return new KeyValuePair<Regex, Action<Request, Response>>();
        }

        private static Dictionary<string, Action<Request, Response>> Actions = new Dictionary<string, Action<Request, Response>>()
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
                        newContent += "<b>GET endpoints:</b><br />";
                        newContent += "<pre>";
                        var listGetRoutes = Server.app.RouteRepository.Get.Keys;
                        newContent += string.Join("<br />", listGetRoutes.ToList()).Replace("^", "").Replace("$", "").Replace(@"(\d+)", "ID");
                        newContent += "</pre>";

                        newContent += "<b>POST endpoints:</b><br />";
                        newContent += "<pre>";
                        var listPostRoutes = Server.app.RouteRepository.Post.Keys;
                        newContent += string.Join("<br />", listPostRoutes.ToList()).Replace("^", "").Replace("$", "").Replace(@"(\d+)", "ID");
                        newContent += "</pre>";

                        newContent += "<b>PUT endpoints:</b><br />";
                        newContent += "<pre>";
                        var listPutRoutes = Server.app.RouteRepository.Put.Keys;
                        newContent += string.Join("<br />", listPutRoutes.ToList()).Replace("^", "").Replace("$", "").Replace(@"(\d+)", "ID");
                        newContent += "</pre>";

                        newContent += "<b>DELETE endpoints:</b><br />";
                        newContent += "<pre>";
                        var listDeleteRoutes = Server.app.RouteRepository.Delete.Keys;
                        newContent += string.Join("<br />", listDeleteRoutes.ToList()).Replace("^", "").Replace("$", "").Replace(@"(\d+)", "ID");
                        newContent += "</pre>";

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
    }
}
