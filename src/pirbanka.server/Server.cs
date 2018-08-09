using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using JamesWright.SimpleHttp;
using Newtonsoft.Json;
using PirBanka.Server.Controllers;

namespace PirBanka.Server
{
    class Server
    {
        private static PirBankaConfig config = new PirBankaConfig();

        static void Main()
        {
            // Check DB connection and state
                // If not, close app (e.g. connections string does not exist, DB connection can't open, DB does not exist, etc.)
            // Check DB is filled with tables and data
                // If not, provide console wizard to fill it up

            SimpleHttpServer app = new SimpleHttpServer();

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
