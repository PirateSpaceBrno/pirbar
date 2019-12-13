const dotenv = require('dotenv');
dotenv.config();

var defaultPort = 8080;
var defaultListenIp = "0.0.0.0";
var fs = require('fs');
var dateFormat = require('dateformat');
var i18n = require("i18n");
var ejs = require("ejs");

module.exports.getPort = function getPort() {
    return process.env.PORT || defaultPort;
}

module.exports.getListenIp = function getListenIp() {
    return process.env.LISTEN_IP || defaultListenIp;
}
  
module.exports.createdDate = function createdDate (file) {  
    const { birthtime } = fs.statSync(file);
    return dateFormat(birthtime, "dd.mm.yyyy" );
}

module.exports.simpleReadFileSync = function simpleReadFileSync(filePath)
{
    var options = {encoding:'utf-8', flag:'r'};
    return fs.readFileSync(filePath, options);
}