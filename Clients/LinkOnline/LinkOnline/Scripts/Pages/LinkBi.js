function ClearLinkBiDefinition() {
    _AjaxRequest("/Handlers/LinkBi.ashx", "ClearLinkBiDefinition", "", function (response) {
        window.location = window.location;
    });
}

function SaveLinkBiDefinition() {
    ShowLoading(document.body);
    _AjaxRequest("/Handlers/LinkBi.ashx", "Save", "", function (response) {
        window.location = "SavedDefinitions.aspx";
    });
}

function SwitchLinkBiDefinition() {
    _AjaxRequest("/Handlers/LinkBi.ashx", "SwitchLinkBiDefinition", "", function (response) {
        window.location = window.location;
    });
}


function UpdateLinkBiWeightingVariable(idVariable) {
    _AjaxRequest("/Handlers/LinkBi.ashx", "UpdateLinkBiWeightingVariable", "IdVariable=" + idVariable, function (response) {

    });
}

function UpdateLinkBiSetting(name, value, reload) {
    _AjaxRequest("/Handlers/LinkBi.ashx", "UpdateLinkBiSetting", "Name=" + name + "&Value=" + value, function (response) {
        if (reload == true)
            window.location = window.location;
    });
}


function ToggleLinkBiSelectorCategory(sender, xPath, idCategory) {
    _AjaxRequest("/Handlers/LinkBi.ashx", "ToggleLinkBiSelectorCategory", "XPath=" + xPath + "&IdCategory=" + idCategory, function (response) {
        sender.className = "LinkBiSelectorCategory " + response;
    });
}


function LoadLinkBiDefinitionProperties(sender, fileName) {
    var pnl = document.getElementById("pnlDefinitionProperties");

    if (sender != undefined)
        ShowLoading(pnl.parentNode);

    _AjaxRequest("/Handlers/LinkBi.ashx", "LoadLinkBiDefinitionProperties", "FileName=" + fileName, function (response) {
        var pnl = document.getElementById("pnlDefinitionProperties");

        HideLoading();

        if (pnl == undefined)
            return;

        pnl.innerHTML = response;

        if (sender.className.search("LinkBiDefinitionSelector") != -1) {
            var elements = sender.parentNode.getElementsByTagName("div");
            for (var i = 0; i < elements.length; i++) {
                elements.item(i).className = "LinkBiDefinitionSelector";
            }

            sender.className = "LinkBiDefinitionSelector LinkBiDefinitionSelector_Active";
        }
    });
}


function UpdateLinkBiSavedReportName(fileName, value, identity) {
    _AjaxRequest("/Handlers/LinkBi.ashx", "UpdateLinkBiSavedReportName", "FileName=" + fileName + "&Value=" + value, function (response) {
        var label = document.getElementById("LinkBiDefinitionSelectorName" + identity);

        if (label == undefined)
            return;

        DecreaseOpacity(label, function () {
            label.innerHTML = value;

            IncreaseOpacity(label, 20);
        }, 20);
    });
}


function ChangeServerConnectionType(sender, fileName, xPath) {
    var pnl = document.getElementById("pnlDefinitionProperties");

    ShowLoading(pnl.parentNode);

    _AjaxRequest("/Handlers/LinkBi.ashx", "ChangeServerConnectionType", "FileName=" + fileName + "&XPath=" + xPath + "&Value=" + sender.value, function (response) {
        LoadLinkBiDefinitionProperties(undefined, fileName);
    });
}

function AddServerConnection(fileName) {
    var pnl = document.getElementById("pnlDefinitionProperties");

    ShowLoading(pnl.parentNode);

    _AjaxRequest("/Handlers/LinkBi.ashx", "AddServerConnection", "FileName=" + fileName, function (response) {
        LoadLinkBiDefinitionProperties(undefined, fileName);
    });
}

function DeleteServerConnection(sender, fileName, xPath) {
    sender.style.background = 'url(/Images/Icons/Cloud/Delete.png) no-repeat 50% 50% #FF0000';
    sender.style.color = 'transparent';
    sender.style.cursor = 'pointer';

    HideChildren(sender);

    sender.onclick = function () {
        var pnl = document.getElementById("pnlDefinitionProperties");

        ShowLoading(pnl.parentNode);

        _AjaxRequest("/Handlers/LinkBi.ashx", "DeleteServerConnection", "FileName=" + fileName + "&XPath=" + xPath, function (response) {
            LoadLinkBiDefinitionProperties(undefined, fileName);
        });
    };

    window.setTimeout(function () {
        sender.onmouseout = function () {
            this.style.background = '';
            this.style.cursor = '';
            this.style.color = '';
            this.onclick = undefined;
            this.onmouseout = undefined;

            ShowChildren(sender);
        }
    }, 100);
}

function DeleteSavedReport(sender, fileName) {
    sender.style.background = 'url(/Images/Icons/Cloud/Delete.png) no-repeat 50% 50% #FF0000';
    sender.style.color = 'transparent';
    sender.style.cursor = 'pointer';

    HideChildren(sender);

    sender.onclick = function () {
        var pnl = document.getElementById("Content");

        ShowLoading(pnl);

        _AjaxRequest("/Handlers/LinkBi.ashx", "DeleteSavedReport", "FileName=" + fileName, function (response) {
            window.location = window.location;
        });
    };

    window.setTimeout(function () {
        sender.onmouseout = function () {
            this.style.background = '';
            this.style.cursor = '';
            this.style.color = '';
            this.onclick = undefined;
            this.onmouseout = undefined;

            ShowChildren(sender);
        }
    }, 100);
}

function UpdateServerConnectionProperty(idServerConnection, fileName, xPath, name, value) {
    _AjaxRequest("/Handlers/LinkBi.ashx", "UpdateServerConnectionProperty", "FileName=" + fileName + "&XPath=" + xPath + "&Name=" + name + "&Value=" + value, function (response) {
        CheckLinkBiServerConnection(fileName, idServerConnection);
    });
}


function DeployLinkBiReport(fileName, serverConnections) {
    var pnl = document.getElementById("pnlDefinitionProperties");

    if (serverConnections == undefined) {
        ShowLoading(pnl.parentNode);

        _AjaxRequest("/Handlers/LinkBi.ashx", "DeployLinkBiReport", "FileName=" + fileName, function (response) {
            HideLoading();
            CheckLinkBiReportStatus(fileName);
        });
    }
    else {
        for (var i = 0; i < serverConnections.length; i++) {
            var idServerConnection = serverConnections[i];

            var container = document.getElementById("LinkBiServerConnectionValid" + idServerConnection);
            container.className = "";

            container.innerHTML = "";
            ShowLoading(container, false, 0.3, "WhiteBackground");

            _AjaxRequest("/Handlers/LinkBi.ashx", "DeployLinkBiReport", "FileName=" + fileName + "&IdServerConnection=" + idServerConnection, function (response) {
                var idServerConnection = response.split('|')[0];
                response = response.split('|')[1];

                var container = document.getElementById("LinkBiServerConnectionValid" + idServerConnection);

                var html;
                html = "<table style=\"\"><tr>";

                if (response != "false") {
                    html += "<td><img src=\"/Images/Icons/Success.png\" height=\"30\" /></td>";
                    html += "<td>" + LoadLanguageText("LinkBiServerConnectionDeploySuccess") + "</td>";

                    container.className = "LinkBiServerConnectionMessage GreenBackground";
                }
                else {
                    html += "<td><img src=\"/Images/Icons/Error.png\" height=\"30\" /></td>";
                    html += "<td>" + LoadLanguageText("LinkBiServerConnectionDeployFailed") + "</td>";

                    container.className = "LinkBiServerConnectionMessage RedBackground";
                }

                html += "</tr></table>";

                container.innerHTML = html;
                CheckLinkBiReportStatus(fileName);
            });
        }
    }
}

var checkLinkBiServerConnectionPending;
function CheckLinkBiServerConnection(fileName, idServerConnection) {

    var container = document.getElementById("LinkBiServerConnectionValid" + idServerConnection);

    container.innerHTML = "";
    container.className = "";
    ShowLoading(container, false, 0.3, "WhiteBackground");

    checkLinkBiServerConnectionPending = true;

    _AjaxRequest("/Handlers/LinkBi.ashx", "CheckLinkBiServerConnection", "FileName=" + fileName + "&IdServerConnection=" + idServerConnection, function (response) {
        if (checkLinkBiServerConnectionPending == false)
            return;

        checkLinkBiServerConnectionPending = false;

        var container = document.getElementById("LinkBiServerConnectionValid" + idServerConnection);

        if (response != "false") {
            container.innerHTML = "";
            HideLoading();
            return;
        }

        var html = "<table style=\"\"><tr>";

        html += "<td><img src=\"/Images/Icons/Warning.png\" height=\"30\" /></td>";
        html += "<td>"+ LoadLanguageText("LinkBiServerConnectionInvalid") +"</td>";

        html += "</tr></table>";

        container.innerHTML = html;
        container.className = "LinkBiServerConnectionMessage BackgroundColor2";

        HideLoading();
    });
}

function CheckLinkBiReportStatus(fileName) {
    _AjaxRequest("/Handlers/LinkBi.ashx", "IsOutdated", "FileName=" + fileName, function (response) {
        var tableCell = document.getElementById("TableCellStatus" + fileName);

        if (tableCell == undefined)
            alert("not found.");

        if (response == "false") {
            tableCell.innerHTML = LoadLanguageText("LinkBiDefinitionUpToDate");
        }
        else {
            tableCell.innerHTML = LoadLanguageText("LinkBiDefinitionOutdated");
        }
    });
}


function EditLinkBiReport(fileName) {
    _AjaxRequest("/Handlers/LinkBi.ashx", "SetActiveLinkBiDefinition", "FileName=" + fileName, function (response) {
        window.location = "LinkBi.aspx";
    });
}

function DownloadLinkBiReport(fileName) {
    _AjaxRequest("/Handlers/LinkBi.ashx", "SetActiveLinkBiDefinition", "FileName=" + fileName, function (response) {
        window.location = "SelectInterface.aspx";
    });
}


var idLanguage;