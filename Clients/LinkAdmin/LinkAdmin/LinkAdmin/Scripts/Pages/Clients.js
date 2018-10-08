function CreatePortal(instanceName, instanceSelection) {
    var html = "<h1 style=\"margin-top:-0.5em;\">" + LanguageManager.GetLabel("CreatePortal") + "</h1>";

    html+= "<table class=\"TableCreatePortalDetails\">";

    html += "<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("CreatePortalName") + "</td>" +
        "<td class=\"TableCellValue\"><input id=\"txtCreateClientName\" type=\"text\" />" +
        "<span id=\"lblCreatePortalName_Error\" style=\"display:none\">" +
        LanguageManager.GetLabel("CreatePortalName_Error")+
        "</span></td></tr>" +
        "<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("CreatePortalInstance") + "</td>" +
        "<td class=\"TableCellValue\"><select id=\"ddlCreateClientInstance\" disabled=\"DISABLED\">" +
        instanceSelection.replace("<option>" + instanceName + "</option>",
        "<option SELECTED=\"selected\">" + instanceName + "</option>") + "</select></td></tr>";

        html += "<tr><td class=\"TableCellTitle\"><br />" +
            LanguageManager.GetLabel("CreatePortalServers") + "</td>" +
            "<td id=\"lblCreatePortalServers_Error\" style=\"display:none\">" +
            LanguageManager.GetLabel("CreatePortalServers_Error")
            +"</td></tr>";

        html += "<tr><td colspan=\"2\">" + Request(
            "/Handlers/GlobalHandler.ashx",
            "CreatePortalGetServers",
            "Instance=" + instanceName
        ) + "</td></tr>";

        html += "<tr><td class=\"TableCellTitle\"><br />" +
            LanguageManager.GetLabel("CreatePortalDefaultUsers") + "</td>" +
            "<td id=\"lblCreatePortalDefaultUsers_Error\" style=\"display:none\">" +
            LanguageManager.GetLabel("CreatePortalDefaultUsers_Error") + "</td></tr>";

        html += "<tr><td colspan=\"2\">" + Request(
            "/Handlers/GlobalHandler.ashx",
            "GetDefaultUsers",
            ""
        ) + "<br /><br /></td></tr>";

    html += "<tr><td colspan=\"2\" style=\"text-align:right;\" id=\"tdCreateClientConfirm\">" +
        CreateButton2("btnMovePortal", "Confirm", "CreateClientConfirm();", "").outerHTML + "</td></tr>";

    html += "</table>";

    CreateBox("boxCreatePortal", html);
}

function GetUsers(instanceName, clientName, server) {
    var result = Request(
        "/Handlers/GlobalHandler.ashx",
        "GetUsers",
        "Instance=" + instanceName +
        "&Client=" + clientName +
        "&Server=" + server
    );

    CreateBox("boxUsers", result);
}

function MovePortal(source, target) {
    var html = "<table class=\"TableMovePortalDetails\">";

    html += "<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("MovePortalSource") + "</td>" +
        "<td class=\"TableCellValue\">" + source + "</td></tr>" +
        "<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("MovePortalTarget") + "</td>" +
        "<td class=\"TableCellValue\">" + target + "</td></tr>";

    html+="<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("MovePortalMigrationRequired") + "</td>" +
        "<td class=\"TableCellValue\">" + LanguageManager.GetLabel("False") + "</td></tr>";

    html += "<tr><td colspan=\"2\" style=\"text-align:right;\">" +
        CreateButton2("btnMovePortal", "Confirm", "", "").outerHTML + "</td></tr>";

    html += "</table>";

    CreateBox("boxMovePortal", html);
}

function CreateClientConfirm() {
    document.getElementById("lblCreatePortalName_Error").style.display = "none";
    document.getElementById("lblCreatePortalDefaultUsers_Error").style.display = "none";
    document.getElementById("lblCreatePortalServers_Error").style.display = "none";

    var valid = true;

    var clientName = document.getElementById("txtCreateClientName").value;

    if (clientName.trim() == "") {
        document.getElementById("lblCreatePortalName_Error").style.display = "";

        document.getElementById("tdCreateClientConfirm").innerHTML = CreateButton2(
            "btnMovePortal",
            "Confirm",
            "CreateClientConfirm();",
            ""
        ).outerHTML;

        valid = false;
    }

    var parameters = "Client=" + clientName;
    parameters += "&Instance=" + document.getElementById("ddlCreateClientInstance").value;
    parameters += "&DefaultUsers=";

    var defaultUsers = "";

    var chkCreateClientDefaultUsers = document.getElementsByName("chkCreateClientDefaultUser");

    for (var i = 0; i < chkCreateClientDefaultUsers.length; i++) {
        if (chkCreateClientDefaultUsers.item(i).checked) {
            defaultUsers += chkCreateClientDefaultUsers.item(i).getAttribute("IdUser") + ",";
        }
    }

    if (defaultUsers.length != 0) {
        defaultUsers = defaultUsers.removeAt(defaultUsers.length - 1, 1);
    }

    if (defaultUsers == "") {
        document.getElementById("lblCreatePortalDefaultUsers_Error").style.display = "";

        document.getElementById("tdCreateClientConfirm").innerHTML = CreateButton2(
            "btnMovePortal",
            "Confirm",
            "CreateClientConfirm();",
            ""
        ).outerHTML;

        valid = false;
    }

    parameters += defaultUsers;

    parameters += "&Servers=";

    var servers = "";

    var chkCreateClientServers = document.getElementsByName("chkCreateClientServer");

    for (var i = 0; i < chkCreateClientServers.length; i++) {
        if (chkCreateClientServers.item(i).checked) {
            servers += chkCreateClientServers.item(i).getAttribute("server") + ",";
        }
    }

    if (servers == "") {
        document.getElementById("lblCreatePortalServers_Error").style.display = "";
        document.getElementById("tdCreateClientConfirm").innerHTML = CreateButton2(
            "btnMovePortal",
            "Confirm",
            "CreateClientConfirm();",
            ""
        ).outerHTML;

        valid = false;
    }

    if (!valid)
        return;

    parameters += servers;

    var result = Request(
        "/Handlers/GlobalHandler.ashx",
        "CreateClient",
        parameters
    );

    if (result != undefined && result.startsWith("__ERROR__")) {
        alert(result);
    }

    HideBox('boxCreatePortal');
}