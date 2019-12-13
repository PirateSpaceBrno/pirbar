var express = require('express');
var router = express.Router();
var helpers = require("./helpers.js");
var i18n = require("i18n");
var server = require("./server.js");

// import library for czech names formatting
var osloveni = require("./cs_CZ-name_formatting/osloveni.js");

// Protect all area with authentication
//router.use("*", auth.isAuthorized);

// Dynamic endpoints
router.get("/", function(req, res) {
    res.render("home");
});

module.exports = router;