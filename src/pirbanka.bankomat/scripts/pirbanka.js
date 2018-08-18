var pirbankaEndpoint = "http://api.pirbanka.psb";

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