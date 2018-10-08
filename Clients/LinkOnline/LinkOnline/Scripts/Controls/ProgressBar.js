function LoadProgress(idControl, service) {
    _AjaxRequest(service, "", "", function (response) {
        var progress = parseInt(response);

        document.getElementById(idControl).style.width = progress + "%";

        if (progress != 100) {
            window.setTimeout(function () {
                LoadProgress(idControl, service);
            }, 100);
        }
        else {
            document.forms[0].submit();
        }
    });
}