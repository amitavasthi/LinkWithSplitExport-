function DeleteField(sender, xPath) {
    if (deleteValue == true)
        return;

    //sender.style.background = 'url(/Images/Icons/Cloud/Delete.png) no-repeat 50% 50% #FF0000';
    sender.style.color = 'transparent';
    sender.style.cursor = 'pointer';
    sender.style.boxShadow = "0px 0px 2px 0px #FF0000";

    var to = [255, 0, 0];

    sender.style.backgroundColor = "rgb(108,174,224)";
    ConvertColor(to, sender, "Increase");

    window.setTimeout(function () {
        if (sender["ColorMode"] != "None")
            sender.style.background = 'url(/Images/Icons/Cloud/Delete.png) no-repeat 50% 50% #FF0000';
    }, 370);

    HideChildren(sender);

    sender.onclick = function () {
        /*var pnl = document.getElementById("Content");

        ShowLoading(pnl);*/

        _AjaxRequest("/Handlers/ProjectHierarchyManager.ashx", "DeleteField", "XPath=" + xPath, function (response) {
            DecreaseOpacity(sender, function () {
                sender.parentNode.removeChild(sender);
            }, 50);
        });
    };

    window.setTimeout(function () {
        sender.onmouseout = function () {
            this["ColorMode"] = "None";
            this.style.background = '';
            this.style.backgroundColor = '';
            this.style.cursor = '';
            this.style.color = '';
            this.onclick = undefined;
            this.onmouseout = undefined;
            this.style.boxShadow = "";

            ShowChildren(sender);
        };
    }, 100);
}

var deleteValue = false;
function DeleteFieldValue(sender, xPath) {
    deleteValue = true;
    //sender.style.background = 'url(/Images/Icons/Cloud/Delete.png) no-repeat 50% 50% #FF0000';
    sender.style.color = 'transparent';
    sender.style.cursor = 'pointer';
    sender.style.boxShadow = "0px 0px 2px 0px #FF0000";

    //var from = [0, 0, 0];
    var to = [255, 0, 0];

    sender.style.backgroundColor = "rgb(108,174,224)";
    ConvertColor(to, sender, "Increase");

    window.setTimeout(function () {
        if (sender["ColorMode"] != "None")
            sender.style.background = 'url(/Images/Icons/Cloud/Delete.png) no-repeat 50% 50% #FF0000';
    }, 370);

    HideChildren(sender);

    sender.onclick = function () {
        //var pnl = document.getElementById("Content");

        //ShowLoading(pnl);

        _AjaxRequest("/Handlers/ProjectHierarchyManager.ashx", "DeleteFieldValue", "XPath=" + xPath, function (response) {
            //window.location = window.location;
            DecreaseOpacity(sender, function () {
                sender.parentNode.removeChild(sender);
            }, 50);
        });
    };

    window.setTimeout(function () {
        sender.onmouseout = function () {
            this["ColorMode"] = "None";
            deleteValue = false;

            this.style.background = '';
            this.style.backgroundColor = '';
            this.style.cursor = '';
            this.style.color = '';
            this.onclick = undefined;
            this.onmouseout = undefined;
            this.style.boxShadow = "";

            ShowChildren(sender);
        }
    }, 100);
}

function UpdateField(id, xPath) {
    var pnlField = document.getElementById("pnlField" + id);

    if (pnlField != undefined)
        ShowLoading(pnlField);

    _AjaxRequest("/Handlers/ProjectHierarchyManager.ashx", "UpdateField", "XPath=" + xPath, function (response) {
        var pnlField = document.getElementById("pnlField" + id);

        if (pnlField != undefined) {
            pnlField.outerHTML = response;
        }

        var pnlField = document.getElementById("pnlField" + id);

        pnlField.style.opacity = "1";
    });
}

function GetFields(parentId, xPath) {
    var container = document.getElementById(parentId);

    if (container != undefined)
        ShowLoading(container);

    _AjaxRequest("/Handlers/ProjectHierarchyManager.ashx", "GetFields", "XPath=" + xPath, function (response) {
        var container = document.getElementById(parentId);

        if (container != undefined) {
            container.innerHTML = response;

            // Run through all child nodes of the container.
            for (var i = 0; i < container.childNodes.length; i++) {
                IncreaseOpacity(container.childNodes.item(i));
            }
        }
    });
}

function SetFieldLabel(xPath, value) {
    _AjaxRequest("/Handlers/ProjectHierarchyManager.ashx", "SetFieldLabel", "XPath=" + xPath + "&Value=" + value, function (response) {
    });
}

function SetFieldValueLabel(xPath, value) {
    _AjaxRequest("/Handlers/ProjectHierarchyManager.ashx", "SetFieldValueLabel", "XPath=" + xPath + "&Value=" + value, function (response) {
    });
}

function SetFieldType(id, xPath, value) {
    _AjaxRequest("/Handlers/ProjectHierarchyManager.ashx", "SetFieldType", "XPath=" + xPath + "&Value=" + value, function (response) {
        UpdateField(id, xPath);
    });
}

function AddField(idParent, xPath) {
    _AjaxRequest("/Handlers/ProjectHierarchyManager.ashx", "AddField", "XPath=" + xPath, function (response) {
        GetFields(idParent, xPath);
    });
}

function AddFieldValue(id, xPath) {
    _AjaxRequest("/Handlers/ProjectHierarchyManager.ashx", "AddFieldValue", "XPath=" + xPath, function (response) {
        UpdateField(id, xPath);
    });
}