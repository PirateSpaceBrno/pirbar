function startTime() {
    // Clock part
    var today = new Date();
    var h = today.getHours();
    var m = today.getMinutes();
    var s = today.getSeconds();
    h = checkTime(h);
    m = checkTime(m);
    s = checkTime(s);
    Clock.text(h + ":" + m + ":" + s);
    Clock.css("color", "var(--white)");

    // Session expiration
    if (window.expirateSec > 0) {
        if (window.expirateSec <= 6) {
            Clock.css("color", "var(--redLight)");
            ClockNote.css("color", "var(--redLight)");
            var showSec = window.expirateSec - 1;
            ClockNote.text("Relace bude ukončena za " + showSec + " vteřin.");
            ClockNote.show();
        }
        else if (window.expirateSec > 6 && window.expirateSec <= 16) {
            Clock.css("color", "var(--yellow)");
            ClockNote.css("color", "var(--yellow)");
            ClockNote.text("Relace bude za chvíli ukončena.")
            ClockNote.show();
        }

        window.expirateSec = window.expirateSec - 1;
        //console.log("Session expire in " + window.expirateSec + " sec.");
    }

    // Set trigger to next second
    var t = setTimeout(startTime, 1000);
}
function checkTime(i) {
    if (i < 10) {i = "0" + i};  // add zero in front of numbers < 10
    return i;
}
