function Request(url, method, parameters) {
    var result;

    var data = "Method=" + method;

    if(parameters != "")
        data+="&" + parameters;

    $.ajax({
        type: "POST",
        data: data,
        url: url,
        dataType: "html",
        async: false,
        success: function (data) {
            result = data;
        }
    });

    if (result == "ERROR_SESSION") {
        window.location = "/Pages/Login.aspx";
        return;
    }

    return result;
}