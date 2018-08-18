var pirbankaEndpoint = "http://api.pirbanka.psb";

function identifyAuth(token) {
    try {
        var json = '';

        console.log(token);

        var myHeaders = new Headers();

        // jQuery
        json = $.ajax({
            url: pirbankaEndpoint + "/auth/identify",
            method: 'GET',
            beforeSend: function (req) {
                req.setRequestHeader('Authorization', 'TOKEN ' + token);
            },
            success: function (data) {
                return data.getResponseHeader;
            },
            error: function (data) {
                console.log("err - " + data);
            }
        });

        console.log(json);
    }
    catch (ex) {
        console.log(ex);
    }
}