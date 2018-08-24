startTime();

try {
    InitBankomat();
    window.CatchToken = true;
    window.CatchTokenTo = Token;
    window.CatchTokenFunc = IdentifyAuth;
    $('input').val('');

    // Append virtual keyboard
    $(function () {
        "use strict";
        jqKeyboard.init({
            icon: "dark"
        });
    });
}
catch (ex) {
    ResultNote.text(":( PirBankomat je smutný");
    changeText(IdentityName, "PirBankomat - chyba");
    switchPage(LoadingError, "redLight");
}
