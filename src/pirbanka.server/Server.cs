using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using JamesWright.SimpleHttp;
using Newtonsoft.Json;
using PirBanka.Server.Controllers;
using PirBanka.Server.Models.Db;
using System.Text.RegularExpressions;

namespace PirBanka.Server
{
    class Server
    {
        private static PirBankaConfig config;

        static void Main()
        {
            Console.WriteLine("Starting PirBanka...");

            // Check DB connection and state
            // If not, close app (e.g. connections string does not exist, DB connection can't open, DB does not exist, etc.)
            // Check DB is filled with tables and data
            // If not, provide console wizard to fill it up

            config = new PirBankaConfig();
            Database db = new Database(config.DbConnectionString);
            SimpleHttpServer app = new SimpleHttpServer();


            app.Get(@"^/$", async (req, res) =>
            {
                res.Content = File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}api-docs{Path.DirectorySeparatorChar}index.html");
                res.ContentType = "text/html; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/api-style.css$", async (req, res) =>
            {
                res.Content = File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}api-docs{Path.DirectorySeparatorChar}api-style.css");
                res.ContentType = "text/css; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/favicon.svg$", async (req, res) =>
            {
                res.Content = File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}api-docs{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pirbanka_icon.svg");
                res.ContentType = "image/svg+xml; charset=utf-8";
                await res.SendAsync();
            });

            //app.Post("/", async (req, res) =>
            //{
            //    res.Content = "<p>You did a POST: " + await req.GetBodyAsync() + "</p>";
            //    res.ContentType = "text/html";
            //    await res.SendAsync();
            //});

            //app.Put("/", async (req, res) =>
            //{
            //    res.Content = "<p>You did a PUT: " + await req.GetBodyAsync() + "</p>";
            //    res.ContentType = "text/html";
            //    await res.SendAsync();
            //});

            //app.Delete("/", async (req, res) =>
            //{
            //    res.Content = "<p>You did a DELETE: " + await req.GetBodyAsync() + "</p>";
            //    res.ContentType = "text/html";
            //    await res.SendAsync();
            //});

            // List of all currencies
            app.Get(@"^/currencies$", async (req, res) =>
            {
                List<Currency> result = db.Get<Currency>(db.GetCommand(), Database.Tables.currencies_view);
                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/currencies/(\d+)$", async (req, res) =>
            {
                int id = Convert.ToInt32(Regex.Match(req.Endpoint, @"^/currencies/(\d+)$", RegexOptions.IgnoreCase).Groups[1].Value);
                Currency result = db.Get<Currency>(db.GetCommand(), Database.Tables.currencies_view).FirstOrDefault(x => x.id == id);
                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });


            // Historical list of exchange rates for specific currency
            app.Get(@"^/currencies/(\d+)/exchange-rates-history$", async (req, res) =>
            {

            });

            // List of all identities
            app.Get(@"^/identities$", async (req, res) =>
            {
                List<Identity> result = db.Get<Identity>(db.GetCommand(), Database.Tables.identities);
                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/identities/(\d+)$", async (req, res) =>
            {
                int id = Convert.ToInt32(Regex.Match(req.Endpoint, @"^/identities/(\d+)$", RegexOptions.IgnoreCase).Groups[1].Value);
                Identity result = db.Get<Identity>(db.GetCommand(), Database.Tables.identities).FirstOrDefault(x => x.id == id);
                res.Content = JsonConvert.SerializeObject(result);
                res.ContentType = "application/json; charset=utf-8";
                await res.SendAsync();
            });

            app.Get(@"^/identities/(\d+)/accounts$", async (req, res) =>
            {

            });

            app.Get(@"^/identities/(\d+)/accounts/(\d+)$", async (req, res) =>
            {

            });


            app.Get(@"^/identities/(\d+)/accounts/(\d+)/transactions$", async (req, res) =>
            {

            });


            // Start listener
            app.Start(config.ListenGateway, config.ListenPort);
        }
    }
}
