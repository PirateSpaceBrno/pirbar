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

            // Set endpoints
            #region GET endpoints returning HTML and misc
                app.Get(Responses.Misc.Action(@"^/$"));
                app.Get(Responses.Misc.Action(@"^/api-style.css$"));
                app.Get(Responses.Misc.Action(@"^/favicon.svg$"));
            #endregion

            #region GET listing endpoints
                app.Get(Responses.Get.Action(@"^/currencies$"));
                app.Get(Responses.Get.Action(@"^/currencies/(\d+)$"));
                app.Get(Responses.Get.Action(@"^/identities$"));
                app.Get(Responses.Get.Action(@"^/identities/(\d+)$"));
                app.Get(Responses.Get.Action(@"^/identities/(\d+)/accounts$"));
                app.Get(Responses.Get.Action(@"^/identities/(\d+)/accounts/(\d+)$"));
                app.Get(Responses.Get.Action(@"^/identities/(\d+)/accounts/(\d+)/transactions$"));
                app.Get(Responses.Get.Action(@"^/identities/(\d+)/accounts/(\d+)/transactions/incoming$"));
                app.Get(Responses.Get.Action(@"^/identities/(\d+)/accounts/(\d+)/transactions/outgoing$"));
                app.Get(Responses.Get.Action(@"^/identities/(\d+)/authentications$"));
            #endregion

            #region GET endpoints with documentation
                app.Get(Responses.GetDocs.Action(@"^/doc/authenticate$"));
                app.Get(Responses.GetDocs.Action(@"^/doc/identities/create$"));
                app.Get(Responses.GetDocs.Action(@"^/doc/identities/(\d+)/authentications/create$"));
                app.Get(Responses.GetDocs.Action(@"^/doc/identities/(\d+)/authentications/(\d+)$"));
                app.Get(Responses.GetDocs.Action(@"^/doc/currencies/create$"));
            #endregion

            #region POST endpoints
                app.Post(Responses.Post.Action(@"^/authenticate$"));
                app.Post(Responses.Post.Action(@"^/identities/create$"));
                app.Post(Responses.Post.Action(@"^/identities/(\d+)/authentications/create$"));
                app.Post(Responses.Post.Action(@"^/identities/(\d+)/authentications/(\d+)$"));
                app.Post(Responses.Post.Action(@"^/currencies/create$"));
            #endregion

            #region DELETE endpoints
                app.Delete(Responses.Delete.Action(@"^/identities/(\d+)/authentications/(\d+)$"));
            #endregion

            #region PUT endpoints
            #endregion

            // Start listener
            app.Start(config.ListenGateway, config.ListenPort);
        }
    }
}
