function ChangeReportTab(sender, idReport) {
    var reportTabs = sender.parentNode.childNodes;

    for (var i = 0; i < reportTabs.length; i++) {
        if (reportTabs.item(i).tagName == "IMG" || reportTabs.item(i).tagName == "img")
            continue;

        if (reportTabs.item(i).className)
            reportTabs.item(i).className = "ReportTab BackgroundColor10";
    }

    sender.className = "ReportTab Color1I ReportTab_Active";

    //below line commented since user is unable to identify why the page is loading twice and added loader to documnet itself.
    //ShowLoading(document.getElementById("CrosstableContainer"));

    ShowLoading(document.body);
    AjaxRequest("ChangeReportTab", "FileName=" + idReport, function (response) {
        if (response.indexOf("Crosstable") > -1)
            window.location = window.location;
        else
        {
            UpdateSetting('AutoUpdate', 'true', true, true);
        }

    });
}

var getQueryString = function (field, url) {
    var href = url ? url : window.location.href;
    var reg = new RegExp('[?&]' + field + '=([^&#]*)', 'i');
    var string = reg.exec(href);
    return string ? string[1] : null;
};

function CreateNewReportTab() {
    AjaxRequest("CreateNewReportTab", "", function (response) {
        AjaxRequest("ChangeReportTab", "FileName=" + response, function (response) {
            ShowLoading(document.body);
            window.location = window.location;
        });
    });
}
function DuplicateReportTab() {
    AjaxRequest("DuplicateReportTab", "", function (response) {
        AjaxRequest("ChangeReportTab", "FileName=" + response, function (response) {
            ShowLoading(document.body);
            window.location = window.location;
        });
    });
}

function UndoReportTab() {

    _AjaxRequest("/Handlers/GlobalHandler.ashx", "ReportDefinitionHistoryBack", "", function (response) {
        window.location = window.location;
        PopulateCrosstable();
    });
}
function RedoReportTab() {
    AjaxRequest("RedoReportTab", "", function (response) {
    });
}


function ShowReportTabContextMenu(sender, idReport) {
    var menu = InitMenu("menuReportTab" + idReport);

    //if (browser.search("IE") == -1) {
    var lnkRename = document.createElement("div");

    lnkRename.ImageUrl = "/Images/Icons/Menu/Rename.png";
    lnkRename.innerHTML = LoadLanguageText("Rename");
    lnkRename.MenuItemClick = "RenameReportTab(this, '" + idReport + "');";

    menu.Items.push(lnkRename);
    //}

    var lnkDelete = document.createElement("div");

    lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
    lnkDelete.innerHTML = LoadLanguageText("Delete");
    lnkDelete.MenuItemClick = "DeleteReportTab(this, '" + idReport + "', '" + sender.innerText.trim() + "');";

    menu.Items.push(lnkDelete);

    menu.Render();
}

//function RenameReportTab(sender, idReport) {
//    var lblReportTabName = document.getElementById("cphContent_lblReportTabName" + idReport);

//    if (lblReportTabName == undefined)
//        return;

//    document.body.removeAttribute("onselectstart");
//    document.body.removeAttribute("unselectable");

//    lblReportTabName.setAttribute("contenteditable", "true");
//    lblReportTabName.focus();
//    lblReportTabName.style.background = "#FFFFFF";
//    lblReportTabName.className = "ReportTabLabel Color1";

//    //SelectElementContents(lblReportTabName);
//    SetCursorToEnd(lblReportTabName);

//    lblReportTabName.onblur = function () {
//        AjaxRequest("ChangeReportTabName", "FileName=" + idReport + "&Name=" + lblReportTabName.innerHTML.trim(), function (response) {
//            lblReportTabName.innerHTML = response;

//            lblReportTabName.style.background = "";
//            lblReportTabName.className = "ReportTabLabel";
//            lblReportTabName.removeAttribute("contenteditable");

//            document.body.setAttribute("onselectstart", "return false;");
//            document.body.setAttribute("unselectable", "on");
//        });
//    };

//    lblReportTabName.onkeydown = function (event) {
//        if (event.keyCode != 13) return;

//        this.blur();

//        return false;
//    }
//}

// start of bug no 192
function RenameReportTab(sender, idReport) {
    var lblReportTabName = document.getElementById("cphContent_lblReportTabName" + idReport);
    if (lblReportTabName == undefined)
        return;
    document.body.removeAttribute("onselectstart");
    document.body.removeAttribute("unselectable");
    lblReportTabName.setAttribute("contenteditable", "true");
    lblReportTabName.focus();
    lblReportTabName.style.background = "#FFFFFF";
    lblReportTabName.className = "ReportTabLabel Color1";
    SetCursorToEnd(lblReportTabName);
    var oldName = lblReportTabName.innerHTML.trim();
    lblReportTabName.onblur = function () {
        var newName = lblReportTabName.innerHTML.trim();
        var dupName;
        var repeatdupName = 0;
        $.each($(".ReportTabLabel"),
            function () {
                dupName == "";
                if (newName.trim() == $(this).text().trim()) {
                    dupName = "yes"; repeatdupName++;
                }
            });

        if ((dupName == "yes") && (repeatdupName == "2")) {
            ShowMessage(LoadLanguageText("ErrorTabAlreadyExists"), "Error");
            lblReportTabName.innerHTML = oldName;
        }
        else {
            lblReportTabName.innerHTML = newName;
        }
        AjaxRequest("ChangeReportTabName", "FileName=" + idReport + "&Name=" + lblReportTabName.innerHTML.trim(), function (response) {
            /*lblReportTabName.innerHTML = response;
            lblReportTabName.style.background = "";
            lblReportTabName.className = "ReportTabLabel";
            lblReportTabName.removeAttribute("contenteditable");
            document.body.setAttribute("onselectstart", "return false;");
            document.body.setAttribute("unselectable", "on");*/
            ShowLoading(document.body);
            window.location = window.location;
        });
    };
    lblReportTabName.onkeydown = function (event) {
        if (event.keyCode != 13) return;
        this.blur();
        return false;
    }
}
// end of bug no 192

function DeleteReportTab(sender, idReport, reportName) {
    CreateConfirmBox(LoadLanguageText("DeleteReportTab").replace("{0}", reportName), function () {
        AjaxRequest("DeleteReportTab", "FileName=" + idReport, function (response) {
            ShowLoading(document.body);
            window.location = window.location;
        });
    });
}