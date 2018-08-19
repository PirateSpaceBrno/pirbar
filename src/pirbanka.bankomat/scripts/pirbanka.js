var pirbankaEndpoint = "http://api.pirbanka.psb";
var json = '';

function IdentifyAuth(token) {
    try {
        var json = '';

        //console.log(token);

        // jQuery
        $.ajax({
            type: "GET",
            url: pirbankaEndpoint + "/auth/identify",
            accept: "application/json",
            beforeSend: function (xhr) {
                //xhr.setRequestHeader("Authorization", "Basic " + btoa("xxxxxxx:yyyyyyyy"));
                xhr.setRequestHeader('Authorization', 'TOKEN ' + token);
            },
            contentType: "application/json",
            dataType: "jsonp",
            success: function (data, textStatus, jqXHR) {
                console.log("succ");
            },
            error: function (error) {
                console.log("fail");
            }
        });

        console.log(json);
    }
    catch (ex) {
        console.log(ex);
    }
}

function GetServerStatus() {
    var path = pirbankaEndpoint + "/status";

    console.log(path);

    $.getJSON(path, function (obj) {
        window.serverStatus = obj;
    });
}


function GetIdentities() {
    var path = pirbankaEndpoint + "/identities";

    console.log(path);

    $.get(path, function (resp, stat) {
        json = resp;
    });

    console.log(json);
}