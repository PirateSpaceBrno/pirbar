startTime();

// gt sderver status, if no konkšun, den resultonte to error mesidž a no authpage shown
var tries = 0;
function InitBankomat() {
    showElem(LoadingScreen);
    GetServerStatus()

    if (null == window.serverStatus) {
        if (tries < 3) {
            setTimeout(function () {
                tries++;
                InitBankomat();
            }, 2000);
        }
        else {
            ResultNote.text(":( PirBankomat je smutný");
            //hideElem(LoadingScreen);
            switchPage(LoadingClearing, LoadingError, "redLight");
        }
    }
    else {
        $("body").focus();
        changeText(IdentityName, "PirBankomat - " + window.serverStatus["name"]);
        switchPage(LoadingClearing, AuthScreen, "white");
        

        // Append virtual keyboard
        $(function () {
            "use strict";
            jqKeyboard.init({
                icon: "dark"
            });
        });
    }
}

try {
    InitBankomat();

}
catch (ex) {
    ResultNote.text(":( PirBankomat je smutný");
    switchPage(LoadingClearing, LoadingError, "redLight");
}
