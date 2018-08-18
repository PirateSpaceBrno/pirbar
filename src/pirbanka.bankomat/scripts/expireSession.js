var idleTimer;
var idleWarn1;
var idleWarn0;
var firstRun = true;

function expireSession() {
    clearTimeout(idleTimer);
    ClockNote.hide();
    ClockNote.css("transition", "visibility 0.5s, opacity 0.5s linear");

    if (firstRun == false) {
        // Set expiration timeout
        window.expirateSec = 30;

        // Plan session termination
        idleTimer = setTimeout(function () {
            location.reload();
        }, 30000);
    }

    firstRun = false;
}

$('*').bind('keyup scroll', expireSession());
$('*').bind('mousemove', function (e) {
    if (window.lastX !== e.clientX || window.lastY !== e.clientY) {
        expireSession();
    }
    window.lastX = e.clientX;
    window.lastY = e.clientY;
});