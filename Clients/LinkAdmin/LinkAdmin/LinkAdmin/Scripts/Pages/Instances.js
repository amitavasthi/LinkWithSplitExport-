function DeployUpdate(instance) {
    var html = "<h1 style=\"margin-top:-0.5em;\">" + LanguageManager.GetLabel("DeployUpdate") + "</h1>";

    html += "<table class=\"TableDeployUpdate\">";

    var deploymentDetails = JSON.parse(Request(
        "/Handlers/GlobalHandler.ashx",
        "GetDeploymentDetails",
        "Instance=" + instance
    ));

    if (deploymentDetails.Errors != undefined && deploymentDetails.Errors.length != 0) {
        html = "";

        for (var i = 0; i < deploymentDetails.Errors.length; i++) {
            html += "<div class=\"DeployUpdateError\">" + deploymentDetails.Errors[i] + "</div>";
        }

        CreateBox("boxDeployUpdate", html);

        return;
    }

    if (deploymentDetails.AvailableUpdates.length == 0) {
        CreateBox("boxDeployUpdate", LanguageManager.GetLabel("NoUpdateAvailable"));

        return;
    }

    html+="<tr><td colspan=\"2\">"+ LanguageManager.GetLabel("DeployUpdateWarning") +"</td></tr>";

    html += "<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("DeployUpdateInstance") + "</td><td id=\"lblDeployUpdateInstance\">" + instance + "</td></tr>";
    html += "<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("DeployUpdateFromVersion") + "</td><td>" + deploymentDetails.FromVersion + "</td></tr>";
    html += "<tr><td class=\"TableCellTitle\">" + LanguageManager.GetLabel("DeployUpdateToVersion") + "</td><td><select id=\"ddlDeployUpdateToVersion\">";

    for (var i = 0; i < deploymentDetails.AvailableUpdates.length; i++) {
        if (i == (deploymentDetails.AvailableUpdates.length - 1)) {
            html += "<option selected=\"SELECTED\">" + deploymentDetails.AvailableUpdates[i] + "</option>";
        } else {
            html += "<option>" + deploymentDetails.AvailableUpdates[i] + "</option>";
        }
    }

    html += "</select></td></tr>";

    html += "<tr><td colspan=\"2\" style=\"text-align:right;\" id=\"tdCreateClientConfirm\">" +
        CreateButton2("btnMovePortal", "Confirm", "DeployUpdateConfirm();", "").outerHTML + "</td></tr>";

    html += "</table>";

    CreateBox("boxDeployUpdate", html);
}

function DeployUpdateConfirm() {
    var instance = document.getElementById("lblDeployUpdateInstance").innerText;
    var version = document.getElementById("ddlDeployUpdateToVersion").value;

    HideBox('boxDeployUpdate');

    CreateBox("boxDeploymentStatus", Request(
        "/Handlers/GlobalHandler.ashx",
        "DeployUpdate",
        "Instance=" + instance +
        "&Version=" + version
    ));

    UpdateDeploymentStep();
}

function UpdateDeploymentStep() {
    var step = Request(
        "/Handlers/GlobalHandler.ashx",
        "GetCurrentDeploymentStep"
    );

    for (var i = 0; i < step; i++) {
        document.getElementById("deploymentStep" + i).className = "DeploymentStep DeploymentStepDone";
    }

    var label = document.getElementById("deploymentStep" + step);

    if (label == undefined) {
        HideBox('boxDeploymentStatus');
        return;
    }

    label.className = "DeploymentStep DeploymentStepActive";

    window.setTimeout(UpdateDeploymentStep, 1000);
}