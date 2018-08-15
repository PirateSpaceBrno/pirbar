PirBanka
---------
PirBanka is set of tools for micropayments inside community. Project consists of database, backend and frontend part.
Backend is HTTP server which provides REST API service to manage PirBank or connect frontend applications so only the server is talking to DB directly.

License: GNU GPL v3

Dependencies:
---------
  - MySQL 5.8+
     - account with disabled only_full_group_by
  - .NET framework 4.6.1+ (on Windows) or
  - Mono framework 5.14.0+ (all platforms) https://www.mono-project.com

Installation
---------
Before you run the application, all dependencies must be installed!

Server application can be started by running pirbanka-server.exe in commandline.

The initial run will offer configuration wizard where all needed informations will be collected:
  - DB connection string
  - instance name
  - general currency
  - listening gateway and port (use * as gateway to listen for all requests)
After all required data are collected, tables in db are created and filled.
Configuration is then stored to config.json file, so next run will not trigger the wizard.

The best approach to run the server on production environment is to configure it as a service.
This is OS dependent so we don't offer any guide to this.

Managing
---------
Once the PirBanka server is running, it offers REST API service on selected gateway and port.

Then you can use one of our prepared clients or create your own. Open http://{YOURSERVER}:{PORT}/ to see the usage documentation.

Clients
---------
TODO