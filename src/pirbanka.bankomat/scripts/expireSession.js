var idleTimer;
var firstRun = true;

function expireSession() {
    clearTimeout(idleTimer);
    ClockNote.hide();
    ClockNote.css("transition", "visibility 0.5s, opacity 0.5s linear");

    //if (firstRun == false) {
        // Set expiration timeout
        window.expirateSec = 30;

        // Plan session termination
        idleTimer = setTimeout(function () {
            location.reload();
        }, 30000);
    //}

    firstRun = false;
}

$(document).on({
    'click': function (e) {
        expireSession();
    },
    //'scroll': function (e) {
    //    expireSession();
    //},
    'keydown': function (e) {
        keyDownHandler(e);
        expireSession();
    },
    'keypress': function (e) {
        expireSession();
    },
    'keyup': function (e) {
        expireSession();
    },
});
$('*').bind('mousemove', function (e) {
    if (window.lastX !== e.clientX || window.lastY !== e.clientY) {
        expireSession();
    }
    window.lastX = e.clientX;
    window.lastY = e.clientY;
});