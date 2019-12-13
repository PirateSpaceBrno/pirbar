var ResultNote = $("#resultNote");


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

function addUserHideShowInputs() {
    if ($('#userMgmScreen div.pass').css('top') == '0px') {
        $('#userMgmScreen div.close').css('margin-top', '20px');
        $('#userMgmScreen div.pass').css('top', '-80px');
    }
    else {
        $('#userMgmScreen div.close').css('margin-top', '100px');
        $('#userMgmScreen div.pass').css('top', '0px');
    }
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

function changeText(elem, text, color='') {
    elem.delay(200).fadeOut(function () {
        if (color != '') {
            $(this).css("color", "var(--" + color + ")");
        }
        else {
            $(this).css("color", "var(--white)");
        }
        $(this).text(text).fadeIn(200);
    });
}

function authShowPasswordInput() {
    $('#authPage div.pass').css('top', '150px');
    $('#token').val('');
    showVirtKeyboard();
    $('#token').focus();
}

function authHidePasswordInput() {
    $('#authPage div.pass').css('top', '110px');
    $('#token').val('');
    $('body').focus();
    hideVirtKeyboard();
}

var TokenListenScreen = $("#tokenListenScreen");

function StartListenForToken(tokenInput) {
    window.CatchToken = true;
    window.CatchTokenTo = tokenInput;
    window.CatchTokenTo.val('');
    window.CatchTokenFunc = StopListenForToken;
    showElem(TokenListenScreen);
}

function StopListenForToken(canc = false) {
    window.CatchToken = false;
    if (canc == true) {
        window.CatchTokenTo.val('');
    }
    window.CatchTokenTo = null;
    window.CatchTokenFunc = function () { };
    hideElem(TokenListenScreen);
}