var pirbankaEndpoint = "http://api.pirbanka.psb";
var json = '';

// Get models
var AuthToken;
var Balances;

function GetJson(location, token = '') {
    var path = pirbankaEndpoint + location;
    console.log(path);

    if (token == '') {
        $.getJSON(path, function (obj) {
            json = obj;
        });
    }
    else {
        $.ajax({
            url: path,
            type: 'GET',
            dataType: 'json',
            success: function (obj) {
                json = obj;
            },
            error: function (obj) {
                console.log(obj);
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Token ' + token);
            }
        });
    }

    console.log(json);
    return json;
}

var tries = 0;
function IdentifyAuth(token) {
    showElem(LoadingScreen);

    AuthToken = GetJson("/auth/identify", token);

    if (AuthToken["identity"] == null || AuthToken["identity"] == '') {
        if (tries < 10) {
            AuthToken = GetJson("/auth/identify", token);

            setTimeout(function () {
                tries++;
                IdentifyAuth(token);
            }, 500);
        }
        else {
            ResultNote.text("Přístupový klíč neexistuje.");
            switchPage(LoadingError, "redLight");
        }
    }
    else {
        if (AuthToken["account"] != null && AuthToken["account"]["market"] == 1) {
            changeText(IdentityName, "Market: " + AuthToken["account"]["currency"]["name"] + " (" + AuthToken["identity"]["display_name"] + ")", "greenLight");
            switchPage(KioskScreen, "greenLight", 1);
        }
        else {
            changeText(IdentityName, "Přihlášen: " + AuthToken["identity"]["display_name"], "blueLight");
            switchPage(KioskScreen, "blueLight", 1);
        }
        tries = 0;
    }
}

function GetServerStatus() {
    return GetJson("/status");
}

function GetIdentities() {
    return GetJson("/identities");
}

var timer;
clearTimeout(timer);
function GetBalances() {
    Balances = GetJson("/bank/balances");

    if (Balances[0] == null || Balances[0] == '') {
        if (tries < 10) {
            Balances = GetJson("/bank/balances");

            timer = setTimeout(function () {
                tries++;
                GetBalances();
            }, 500);
        }
        else {
            clearTimeout(timer);
        }

    }
    else {
        tries = 0;
        clearTimeout(timer);
        return Balances;
    }
}


function ShowStatsScreen() {
    showElem(LoadingScreen);
    StatsVersion.html("Verze serveru: <span class='bold'>" + serverStatus["version"] + "</span>");
    StatsIdentitiesCount.html("Počet registrovaných identit: <span class='bold'>" + serverStatus["identitiesCount"] + "</span>");
    StatsAccountsCount.html("Počet registrovaných účtů: <span class='bold'>" + serverStatus["accountsCount"] + "</span>");
    StatsMarketsCount.html("Počet registrovaných marketů: <span class='bold'>" + serverStatus["marketsCount"] + "</span>");

    GetBalances();
    console.log(Balances);


    var i;
    for (i = 0; i < Balances.length;) {
        var balance = Balances[i];
        var newElem = $("<div></div>").html("Měna: " + balance["currency"]["name"] + " | Na účtech: " + balance["creditBalance"] + " " + balance["currency"]["shortname"] + " | V bance: " + balance["chestBalance"] + " " + balance["currency"]["shortname"]);
        Stats.append(newElem);
        i++;
    }

    switchPage(Stats, 'purpleLight', 1);
}