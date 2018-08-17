var pirbankaEndpoint = "http://api.pirbanka.psb";

$.fn.extend({
    qcss: function (css) {
        return $(this).queue(function (next) {
            $(this).css(css);
            next();
        });
    }
});

function changeStatus(color, delay = 0) {
    var colorHex = '';
    var defaultColorHex = "#ecf0f1";
    var element = $("#magicEye .plane.main .circle");

    switch (color) {
        case "red":
            colorHex = "#e74c3c";
            break;
        case "green":
            colorHex = "#2ecc71";
            break;
        case "yellow":
            colorHex = "#f1c40f";
            break;
        case "blue":
            colorHex = "#3498db";
            break;
        case "purple":
            colorHex = "#8e44ad";
            break;
        default:
            colorHex = defaultColorHex;
            break;
    }

    var setVal = "0 0 15px " + colorHex + ", inset 0 0 15px " + colorHex;
    var defVal = "0 0 15px " + defaultColorHex + ", inset 0 0 15px " + defaultColorHex;

    if (delay == 0) {
        element.css("box-shadow", setVal);
    }
    else {
        element.css("box-shadow", setVal).delay(delay).queue(function () {
            element.css("box-shadow", defVal);
            element.dequeue();
        }); 
    }
}


function identifyAuth(token) {
    var json = '';

    $.ajaxSetup({
        headers: {
            'Authorization': "TOKEN " + token,
        }
    });

    $.get(pirbankaEndpoint + "/auth/identify", function (data) {
        console.log(data);
    }, 'json');

    if (json === "") {
        console.log("err");
        changeStatus("red", 5000);
    }
    else {
        console.log("succ");
        changeStatus("green", 5000);
    }
}