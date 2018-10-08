function AjaxRequest(method, parameters, onFinish) {
    return _AjaxRequest("/Handlers/GlobalHandler.ashx", method, parameters, onFinish);
}

function _AjaxRequest(url, method, parameters, onFinish, scopeObj) {
    var http = getHTTPObject();

    if (method != "") {
        if (parameters != "")
            parameters = "Method=" + method + "&" + parameters;
        else
            parameters = "Method=" + method;
    }

    http.open("POST", url, true);

    http.onreadystatechange = function () {
        if (http.readyState == 4) {
            if (http.status == "200") {
                if (onFinish != undefined) {
                    onFinish(http.responseText, scopeObj);
                }
            }
            else {
                var errorText = "The server returned an error: ";

                try {
                    errorText += http.response.split("<title>")[1].split("</title>")[0];
                }
                catch (e) {
                    errorText += http.statusText;
                }

                if (errorText == "The server returned an error: ")
                    return;

                if (errorText.search("Not authenticated") != -1) {
                    window.location = "/Pages/Login.aspx?RedirectUrl=" + window.location.toString();
                }
                else {
                    // Weitere Anweisungen bei missglücktem Öffnen
                    ShowMessage(errorText, "Error");
                }
            }
        }
    }

    http.setRequestHeader("Content-Type",
        "application/x-www-form-urlencoded");

    http.send(parameters);

    return http;
}

function AjaxRequestBuffer(method, fileName, buffer, onFinish) {
    var http = getHTTPObject();

    http.open("POST", "/Handlers/GlobalHandler.ashx?Method=" + method + "&FileName=" + fileName, true);

    http.onreadystatechange = function () {
        if (http.readyState == 4) {
            if (http.status == "200") {
                if (onFinish != undefined)
                    onFinish(http.responseText);
            }
            else {
                // Weitere Anweisungen bei missglücktem Öffnen
            }
        }
    }

    http.setRequestHeader("Content-Type",
        "application/x-www-form-urlencoded");

    http.send(buffer);
}

function RequestSynch(url, method, parameters) {
    var result;

    var requestData = parameters;

    if (method != undefined && method != "") {
        requestData += "&Method=" + method;
    }

    $.ajax({
        type: "POST",
        data: requestData,
        url: url,
        dataType: "html",
        async: false,
        success: function (data) {
            result = data;
        },
        error: function (msg) {
            if (msg.responseText.search("<i>Not authenticated.</i>") != -1) {
                window.location = "Login.html";
            }
        }
    });

    return result;
}

function getHTTPObject() {
    var httpObject = false;
    if (window.XMLHttpRequest) {
        httpObject = new XMLHttpRequest();
    } else if (window.ActiveXObject) {
        httpObject = new ActiveXObject("Microsoft.XMLHTTP");
    } else {
        // If not supported
        httpObject = false;
    }

    return httpObject;
}