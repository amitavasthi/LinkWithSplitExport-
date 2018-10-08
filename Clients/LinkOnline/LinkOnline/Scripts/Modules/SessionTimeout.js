var latestAction = new Date();

mouseMoveFunctions.Add("SessionTimeoutWarning", function () {
    latestAction = new Date();
});

function InitTimeoutWarning(timeout) {
    window.setTimeout(function () {
        ShowTimeoutWarning(timeout);
    }, 2000);
}

function ShowTimeoutWarning(timeout) {
    if ((new Date() - latestAction) < timeout) {
        //KeepSessionAlive(timeout);

        window.setTimeout(function () {
            ShowTimeoutWarning(timeout);
        }, 2000);
    }
    else {
        var background = document.createElement("div");
        var box = document.createElement("div");

        background.id = "TimeoutWarningBackground";
        box.id = "TimeoutWarning";
        background.className = "BoxBackground";
        box.className = "TimeoutWarning Color1";

        //box.innerHTML = "<table style=\"width:100%;height:100%;\"><tr><td>" + LoadLanguageText("SessionTimeoutWarning").replace("{0}", "45") + "</td></tr></table>";

        document.body.appendChild(background);
        document.body.appendChild(box);

        CountDownSessionTimeout(timeout);
    }
}
var  myVar;
function CountDownSessionTimeout(timeout) {
    var box = document.getElementById("TimeoutWarning");

    if ((new Date() - latestAction) < timeout) {
        KeepSessionAlive(timeout);

        var background = document.getElementById("TimeoutWarningBackground");

        box.parentNode.removeChild(box);
        background.parentNode.removeChild(background);

        return;
    }
    if (parseInt(60 - parseInt((1000 - (new Date() - latestAction) + timeout) / -1000)) == 0) {
        clearTimeout(myVar);
        box.innerHTML = "<table style=\"width:100%;height:100%;\"><tr><td>" + LoadLanguageText("SessionTimeoutWarning").replace("{0}", "<span style=\"color:#FF0000\">" + 0 + "</span>") + "</td></tr></table>";
        window.location = "/Pages/Login.aspx";
    }
    else {
        var result = 60 - parseInt((1000 - (new Date() - latestAction) + timeout) / -1000);

        if (parseInt(result) <= 0) {
            window.location = "/Pages/Login.aspx";
        }

        if (parseInt(result) < 20) {
            result = "<span style=\"color:#FF0000\">" + result + "</span>";
        }

    box.innerHTML = "<table style=\"width:100%;height:100%;\"><tr><td>" + LoadLanguageText("SessionTimeoutWarning").replace("{0}", result) + "</td></tr></table>";

        myVar = window.setTimeout(function () {
            CountDownSessionTimeout(timeout);
        }, 1000);
    }
}

function KeepSessionAlive(timeout) {
    AjaxRequest("KeepSessionAlive", "", function (response) {
    });

    window.setTimeout(function () {
        ShowTimeoutWarning(timeout);
    }, 500);
}