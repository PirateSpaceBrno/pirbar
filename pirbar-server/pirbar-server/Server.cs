using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using JamesWright.SimpleHttp;
using Newtonsoft.Json;
using PirBar.Server.Controllers;

namespace PirBar.Server
{
    class Server
    {
        static void Main()
        {
            // Read config.json and initialize variables
            var config = new PirBarConfig();
            var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


            App app = new App();

            app.Get("/", async (req, res) =>
            {
                res.Content = "<p>You did a GET.</p>";
                res.ContentType = "text/html";
                await res.SendAsync();
            });

            app.Post("/", async (req, res) =>
            {
                res.Content = "<p>You did a POST: " + await req.GetBodyAsync() + "</p>";
                res.ContentType = "text/html";
                await res.SendAsync();
            });

            app.Put("/", async (req, res) =>
            {
                res.Content = "<p>You did a PUT: " + await req.GetBodyAsync() + "</p>";
                res.ContentType = "text/html";
                await res.SendAsync();
            });

            app.Delete("/", async (req, res) =>
            {
                res.Content = "<p>You did a DELETE: " + await req.GetBodyAsync() + "</p>";
                res.ContentType = "text/html";
                await res.SendAsync();
            });

            app.Start(config.ListenGateway, config.ListenPort);
        }
    }
}
