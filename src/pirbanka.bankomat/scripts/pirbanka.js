var pirbankaEndpoint = "https://api.pirbanka.psb";
var json = '';

// Get models
var AuthToken;
var Balances;
var Identities;

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
var triesTimer;
clearTimeout(triesTimer);
function IdentifyAuth() {
    showElem(LoadingScreen);
    clearTimeout(triesTimer);
    var token = CatchTokenTo.val();

    AuthToken = GetJson("/auth/identify", token);

    if (!AuthToken.hasOwnProperty("identity")) {
        if (tries < 10) {
            AuthToken = GetJson("/auth/identify", token);

            triesTimer = setTimeout(function () {
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
            switchPage(KioskScreen, "greenLight");
        }
        else {
            changeText(IdentityName, "Přihlášen: " + AuthToken["identity"]["display_name"], "blueLight");
            switchPage(KioskScreen, "blueLight");
        }
        tries = 0;
        clearTimeout(triesTimer);
    }
}

function GetServerStatus() {
    return GetJson("/status");
}

function GetIdentities() {
    return GetJson("/identities");
}

function GetBalances() {
    return GetJson("/bank/balances");
}


function ShowStatsScreen() {
    showElem(LoadingScreen);
    clearTimeout(triesTimer);

    try {
        Balances = GetBalances();

        if (Balances[0].hasOwnProperty("currency")) {
            StatsVersion.html("Verze serveru: <span class='bold'>" + serverStatus["version"] + "</span>");
            StatsIdentitiesCount.html("Počet registrovaných identit: <span class='bold'>" + serverStatus["identitiesCount"] + "</span>");
            StatsAccountsCount.html("Počet registrovaných účtů: <span class='bold'>" + serverStatus["accountsCount"] + "</span>");
            StatsMarketsCount.html("Počet registrovaných marketů: <span class='bold'>" + serverStatus["marketsCount"] + "</span>");

            StatsBalances.empty();
            var i;
            for (i = 0; i < Balances.length;) {
                var balance = Balances[i];
                var newElem = $("<div></div>").html("<span class='bold'>" + balance["currency"]["name"] + "</span> | Na účtech: <span class='bold'>" + balance["creditBalance"] + " " + balance["currency"]["shortname"] + "</span> | V bance: <span class='bold'>" + balance["chestBalance"] + " " + balance["currency"]["shortname"] + "</span>");
                StatsBalances.append(newElem);
                i++;
            }
            StatsStatic.append(StatsBalances);

            tries = 0;
            clearTimeout(triesTimer);
            switchPage(Stats, 'purpleLight');
        }
        else {
            throw "undefined";
        }
    }
    catch (ex) {
        if (tries < 10) {
            Balances = GetBalances();

            triesTimer = setTimeout(function () {
                tries++;
                ShowStatsScreen();
            }, 500);
        }
    }
}


function ShowUserMgmScreen() {
    showElem(LoadingScreen);
    clearTimeout(triesTimer);

    try {
        Identities = GetIdentities();

        if (Identities[0].hasOwnProperty("display_name")) {
            console.log(Identities);

            StatsIdentities.empty();

            var i;
            for (i = 0; i < Identities.length;) {
                var identity = Identities[i];
                var newElem = $("<div></div>").html("<span class='bold'>" + identity["display_name"] + "</span>");

                if (i == 0) {
                    newElem.addClass("isBank").append(" (BANKA)");
                }

                if (identity["admin"] == 1) {
                    newElem.addClass("isAdmin").append(" (ADMIN)");
                }

                StatsIdentities.append(newElem);
                i++;
            }

            tries = 0;
            clearTimeout(triesTimer);
            switchPage(UserMgmScreen, 'purpleLight');
        }
        else {
            throw "undefined";
        }
    }
    catch (ex) {
        if (tries < 10) {
            Identities = GetIdentities();

            triesTimer = setTimeout(function () {
                tries++;
                ShowUserMgmScreen();
            }, 500);
        }
    }
}

function ShowKioskScreen() {
    changeText(IdentityName, IdentityName.text(), 'purpleLight');
    window.CatchToken = false;
    window.CatchTokenTo = null;
    window.CatchTokenFunc = function () { };
    switchPage(KioskScreen, 'purpleLight');
}

function CreateNewIdentity() {
    var nameInput = $("#userMgmScreen div.pass #addUserName");
    var tokenInput = $("#userMgmScreen div.pass #addUserToken");

    if (nameInput.val() != '' && tokenInput.val() != '') {
        $.post(pirbankaEndpoint + "/identities", "{ \"name\": \"" + nameInput.val() + "\", \"password\": \"" + tokenInput.val() + "\" }")
            .done(function (data) {
                alert(data);
            })
            .fail(function () {
                alert("Nastala chyba při vytváření identity.");
            });

        nameInput.val('');
        tokenInput.val('');
    }
    else {
        alert("Musíš vyplnit obě pole.");
    }
}