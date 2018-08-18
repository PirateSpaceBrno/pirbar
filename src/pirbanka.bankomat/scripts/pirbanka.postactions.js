// Set loading screen to AJAX
LoadingScreen
    .ajaxStart(function () {
        showElem($(this));
    })
    .ajaxStop(function () {
        hideElem($(this));
    });



$("body").focus();


// Append virtual keyboard
$(function () {
    "use strict";
    jqKeyboard.init({
        icon: "dark"
    });
});
