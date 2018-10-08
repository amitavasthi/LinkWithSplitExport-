function SingleSignOn(host, username, password) {
    window.open("http://" + host + "/Handlers/LinkBiExternal.ashx?Method=Authenticate&IsEncrypted=True&Username=" + username + "&Password=" + password + 
        "&RedirectUrl=" + "http://" + host + "/Pages/Default.aspx");
}

function DeleteSingleSignOn(host) {
    Request("/Handlers/GlobalHandler.ashx", "DeleteSingleSignOn", "Host=" + host);

    Navigate(document.getElementById("navItemDefault"), '~/Pages/Default.ascx');
}

function CreateSingleSignOn(host) {
    var html = "<h1 style=\"margin-top:-0.5em;\">" + LanguageManager.GetLabel("CreateSingleSignOn") + "</h1>";

    html += "<table>";

    html += "<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("CreateSingleSignOnHost") + "</td>" +
        "<td id=\"tdCreateSignleSignOnHost\" class=\"TableCellValue\">" + host + "</td></tr>" +
        "<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("CreateSingleSignOnUsername") + "</td>" +
        "<td class=\"TableCellValue\"><input id=\"txtCreateSignleSignOnUsername\" type=\"text\" /></td></tr>" +
    "<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("CreateSingleSignOnPassword") + "</td>" +
    "<td class=\"TableCellValue\"><input id=\"txtCreateSignleSignOnPassword\" type=\"password\" /></td></tr>";

    html += "<tr><td colspan=\"2\" style=\"text-align:right;\" id=\"tdCreateSingleSignOnConfirm\">" +
        CreateButton2("btnCreateSingleSignOnConfirm", "Confirm", "CreateCreateSingleSignOnConfirm();", "").outerHTML + "</td></tr>";

    html += "</table>";

    CreateBox("boxCreateSingleSignOn", html);
}

function CreateCreateSingleSignOnConfirm() {
    var host = document.getElementById("tdCreateSignleSignOnHost").innerText;
    var username = document.getElementById("txtCreateSignleSignOnUsername").value;
    var password = document.getElementById("txtCreateSignleSignOnPassword").value;

    Request("/Handlers/GlobalHandler.ashx", "CreateSingleSignOn", "Host=" + host + "&Username=" + username + "&Password=" + password);

    HideBox('boxCreateSingleSignOn');

    Navigate(document.getElementById("navItemDefault"), '~/Pages/Default.ascx');
}