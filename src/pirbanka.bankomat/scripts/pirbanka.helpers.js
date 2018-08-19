// Hide/Show methods
function isElemVisible(elem) {
    if (elem.is(":hidden") || elem.css("visibility") == "hidden" || elem.css("opacity") == 0) {
        return 0;
    } else {
        return 1;
    }
}

function showHideVirtKeyboard() {
    if (isElemVisible($('#jqk-toggle-btn'))) {
        showVirtKeyboard();
    }
    else {
        hideVirtKeyboard();
    }
}

function showVirtKeyboard() {
    $('#jqk-toggle-btn').hide(); $('#jq-keyboard').addClass('show');
}

function hideVirtKeyboard() {
    $('#jqk-toggle-btn').show(); $('#jq-keyboard').removeClass('show');
}

function showElem(elem) {
    return elem.css('visibility', 'visible').css('opacity', '1');
}

function hideElem(elem) {
    elem.css("transition", "visibility 0.5s, opacity 0.5s linear");
    elem.css('opacity', '0').css('visibility', 'hidden').delay(500).queue(function () {
        elem.css("transition", "visibility 0s, opacity 0.5s linear");
        elem.dequeue();
    });
    return elem;
}

// Method to switch page
function switchPage(from, to, color) {
    showElem(LoadingScreen).delay(1000).queue(function () {
        //hideElem(from);
        from.hide();
        changeStatus(color);
        to.show();
        //showElem(to);
        //LoadingScreen.css("transition", "visibility 0.5s, opacity 0.5s linear");
        hideElem(LoadingScreen);
        LoadingScreen.dequeue();
    });//.css("transition", "visibility 0s, opacity 0.5s linear");
}

// 
function changeStatus(color, delay = 0) {
    var defaultColorHex = "var(--white)";
    var element = $("#magicEye .plane.main .circle");
    var colorCss = "var(--" + color + ")";

    var setVal = "0 0 15px " + colorCss + ", inset 0 0 15px " + colorCss;
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

function changeText(elem, text) {
    elem.fadeOut(function () {
        $(this).text(text).fadeIn(50);
    });
}



