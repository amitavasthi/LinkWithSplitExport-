var languageTexts = new Object();

function LoadLanguageText(name, onResponse) {
    var http = getHTTPObject();
    var params = 'Name=' + name;

    http.open("POST", window.location.protocol + "//" + window.location.host + "/Handlers/LanguageManager.ashx", true);

    http.onreadystatechange = function () {
        if (http.readyState == 4) {
            if (http.status == "200") {
                // Success
                if (onResponse != undefined)
                    onResponse(http.responseText);
            }
            else {
            }
        }
    }

    http.setRequestHeader("Content-Type",
        "application/x-www-form-urlencoded");

    http.send(params);
}

function LoadLanguageText(name) {
    var text;

    if (languageTexts[name] == undefined) {
        $.ajax({
            type: "POST",
            data: "Name=" + name,
            url: window.location.protocol + "//" + window.location.host + "/Handlers/LanguageManager.ashx",
            dataType: "html",
            async: false,
            success: function (data) {
                text = data;
            }
        });

        languageTexts[name] = text;
    }
    else {
        text = languageTexts[name];
    }

    return text;
}