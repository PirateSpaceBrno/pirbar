startTime();

// Append virtual keyboard
$(function () {
    "use strict";
    jqKeyboard.init({
        icon: "dark"
    });
});

window.CatchToken = true;
window.CatchTokenTo = Token;
window.CatchTokenFunc = IdentifyAuth;
$('input').val('');