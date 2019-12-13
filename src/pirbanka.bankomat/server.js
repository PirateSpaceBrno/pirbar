var express = require("express");
var session = require("express-session");
var app = require("express")();
module.exports.app = app;

var i18n = require("i18n");
var bodyParser = require("body-parser");

var http = require("http").Server(app);

var router = require("./router.js");
var routerEje = require("./router.eje.js");
var helpers = require("./helpers.js");
var log = require('./log.js');

const { version } = require('./package.json');


// set the view engine to ejs
app.set('view engine', 'ejs');

// translate application
i18n.configure({
  defaultLocale: 'cs_CZ',
  directory: __dirname + '/locales'
});

// Middlewares to accept POST requests
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());
// default: using 'accept-language' header to guess language settings
app.use(i18n.init);
// Static endpoints
app.use(express.static('static'));
// Dynamic endpoints
app.use("/bower_components", express.static("bower_components"));
app.use("/", router);
app.use("/client", routerEje);
//The 404 Route (ALWAYS Keep this as the last route)
//app.get('*', auth.isAuthorized, function(req, res){
//  res.redirect("/eje/404");
//});


// Setup Express Listener
http.listen(helpers.getPort(), helpers.getListenIp(), function() {
  log.log("Starting PirBanka bankomat " + version);
  log.log("Listening on: " + helpers.getListenIp() + ":" + helpers.getPort());
});
