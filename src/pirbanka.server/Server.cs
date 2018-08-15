using JamesWright.SimpleHttp;
using PirBanka.Server.Controllers;
using System;

namespace PirBanka.Server
{
    class Server
    {
        internal static PirBankaConfig config;
        internal static DatabaseHelper db;
        internal static SimpleHttpServer app;

        static void Main()
        {
            Console.WriteLine("Starting PirBanka...");

            // Initialize variables
            config = new PirBankaConfig();
            db = new DatabaseHelper(config.DbConnectionString);
            app = new SimpleHttpServer();

            // GET endpoints returning HTML and misc
            foreach (var x in Responses.Misc.Actions) app.Get(x.Key, x.Value);
            foreach (var x in Responses.GetDocs.Actions) app.Get(x.Key, x.Value);

            // endpoints
            foreach (var x in Responses.Get.Actions) app.Get(x.Key, x.Value);
            foreach (var x in Responses.Post.Actions) app.Post(x.Key, x.Value);
            foreach (var x in Responses.Put.Actions) app.Put(x.Key, x.Value);
            foreach (var x in Responses.Delete.Actions) app.Delete(x.Key, x.Value);

            // AUTH endpoints
            foreach (var x in Responses.AuthGet.Actions) app.Get(x.Key, x.Value);
            foreach (var x in Responses.AuthPost.Actions) app.Post(x.Key, x.Value);
            foreach (var x in Responses.AuthPut.Actions) app.Put(x.Key, x.Value);
            foreach (var x in Responses.AuthDelete.Actions) app.Delete(x.Key, x.Value);

            // GET endpoints with documentation
            foreach (var x in Responses.AdminGet.Actions) app.Get(x.Key, x.Value);
            foreach (var x in Responses.AdminPost.Actions) app.Post(x.Key, x.Value);
            foreach (var x in Responses.AdminPut.Actions) app.Put(x.Key, x.Value);
            foreach (var x in Responses.AdminDelete.Actions) app.Delete(x.Key, x.Value);

            // Start listener
            app.Start(config.ListenGateway, config.ListenPort);
        }
    }
}
