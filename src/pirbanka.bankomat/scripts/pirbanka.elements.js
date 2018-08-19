// Screens elements
var LoadingScreen = $("#loading");
var AuthScreen = $("#authPage");
var KioskScreen = $("#kioskScreen");
var MarketScreen = $("#marketScreen");
var IdentityScreen = $("#identityScreen");
var Token = $("#token");
var Clock = $("#clock");
var ClockNote = $("#clockNote");
var ResultNote = $("#resultNote");
var LoadingClearing = $("#loadingClearing");
var LoadingError = $("#loadingError");
var IdentityName = $("#identityName");

var CatchToken = true;


// Set loading screen to AJAX
//$(document)
//    .ajaxStart(function () {
//    	showElem(LoadingScreen);
//    	//LoadingScreen.css("transition", "visibility 0s, opacity 0.5s linear");
//    })
//    .ajaxStop(function () {
//    	//LoadingScreen.css("transition", "visibility 0.5s, opacity 0.5s linear");
//    	hideElem(LoadingScreen);
//    });