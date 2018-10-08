function Request(url, method, parameters) {
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
            if (data == "NotAuthenticated")
            {
                window.location = "Login.html";
                return;
            }
            result = data;
        },
        error: function (msg) {
        }
    });

    return result;
}

function RequestAsynch(url, method, parameters, onFinish, onError) {
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
        async: true,
        success: function (data) {
            if (data == "NotAuthenticated") {
                window.location = "Login.html";
                return;
            }

            if (onFinish != undefined)
                onFinish(data);
        },
        error: function (msg) {

            if (onError)
                onError();

            if (msg.responseText.search("<i>Not authenticated.</i>") != -1) {
                window.location = "Login.html";
            }
        }
    });
}

function AjaxDownload(url, data, input_name) {
    var $iframe,
        iframe_doc,
        iframe_html;

    if (($iframe = $('#download_iframe')).length === 0) {
        $iframe = $("<iframe id='download_iframe'" +
                    " style='display: none' src='about:blank'></iframe>"
                   ).appendTo("body");
    }

    iframe_doc = $iframe[0].contentWindow || $iframe[0].contentDocument;
    if (iframe_doc.document) {
        iframe_doc = iframe_doc.document;
    }

    iframe_html = "<html><head></head><body><form method='POST' action='" +
                  url + "'>" +
                  "<textarea name='" + input_name + "'>" +
                  data + "</textarea></form>" +
                  "</body></html>";

    iframe_doc.open();
    iframe_doc.write(iframe_html);
    $(iframe_doc).find('form').submit();
}