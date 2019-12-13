var express = require('express');
var router = express.Router();
var helpers = require("./helpers.js");
var i18n = require("i18n");
var ejs = require('ejs');
var fs = require('fs');
var log = require('./log.js');

// Dynamic endpoints
router.get("/", function(req, res) {
    res.render("home", {
        Identity: {
            Name: "PirBankomat"
        }
    });
    //res.redirect("/home");
});

router.post("/auth", function(req, res) {
    res.redirect("/");
});

module.exports = router;