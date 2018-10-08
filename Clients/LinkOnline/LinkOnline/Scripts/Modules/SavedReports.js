function RenderSavedReportOptions(sender, directory, idSavedReport, isPrivate) {
    var menu = InitMenu("menuSavedReport" + idSavedReport);
    var lnkPinToHome = document.createElement("div");
    lnkPinToHome.ImageUrl = "/Images/Icons/Menu/Pin.png";
    lnkPinToHome.innerHTML = LoadLanguageText("PinToHomescreen");
    lnkPinToHome.MenuItemClick = "PinToHomescreen('" + idSavedReport + "')";

    menu.Items.push(lnkPinToHome);

    var path = directory + "/Info.xml";
    var lnkRename = document.createElement("div");
    lnkRename.ImageUrl = "/Images/Icons/Menu/Rename.png";
    lnkRename.innerHTML = LoadLanguageText("Rename");
    lnkRename.MenuItemClick = "RenameReport('" + sender.id + "', '" + path + "', true);";
    if (isPrivate != "NA") {
        menu.Items.push(lnkRename);
    }

    var lnkMove = document.createElement("div");
    lnkMove.ImageUrl = "/Images/Icons/Menu/move.png";
    lnkMove.innerHTML = LoadLanguageText("Move");
    //lnkMove.MenuItemClick = "MoveReport('" + sender.id + "', '" + path + "', true);";
    lnkMove.MenuItemClick = "document.getElementById('cphContent_ImageMove').setAttribute('source', '" + path + "');ShowTreeview('" + path + "');";
    var textElement = sender.parentNode.getElementsByTagName('div');
    textElement = textElement[textElement.length - 1];
    sender.parentNode.getElementsByTagName('div')[0].innerText;
    document.getElementById("reportNameDiv").innerHTML = textElement.innerText;  // "sdgsg"

    if (isPrivate != "NA") {
        menu.Items.push(lnkMove);
    }


    var lnkCopyLink = document.createElement("div");
    lnkCopyLink.ImageUrl = "/Images/Icons/Menu/CopyHyperlink.png";
    lnkCopyLink.innerHTML = LoadLanguageText("CopyHyperlink");
    lnkCopyLink.MenuItemClick = "CopyHyperlink('" + idSavedReport + "','" + directory + "')";

    menu.Items.push(lnkCopyLink);

    var lnkConnectPowerBI = document.createElement("div");
    lnkConnectPowerBI.ImageUrl = "/Images/Icons/Menu/ConnectPowerBI.png";
    lnkConnectPowerBI.innerHTML = LoadLanguageText("ConnectPowerBI");
    lnkConnectPowerBI.MenuItemClick = "ConnectPowerBI('" + idSavedReport + "', 'Reporter')";

    menu.Items.push(lnkConnectPowerBI);


    var lnkMakeReportPrivate = document.createElement("div");
    lnkMakeReportPrivate.ImageUrl = "/Images/Icons/Menu/private.png";
    lnkMakeReportPrivate.innerHTML = LoadLanguageText("MakePrivate");
    lnkMakeReportPrivate.MenuItemClick = "MakeReportPrivate('" + sender.id + "', '" + path + "', true)";
    lnkMakeReportPrivate.setAttribute("id", "divPrivate");
    if (isPrivate == "true") {
        lnkMakeReportPrivate.Display = "none";
        menu.Items.push(lnkMakeReportPrivate);
    }
    else if (isPrivate == "false") {
        lnkMakeReportPrivate.Display = "";
        menu.Items.push(lnkMakeReportPrivate);
    }



    var lnkMakeReportPublic = document.createElement("div");
    lnkMakeReportPublic.ImageUrl = "/Images/Icons/Menu/private.png";
    lnkMakeReportPublic.innerHTML = LoadLanguageText("MakePublic");
    lnkMakeReportPublic.MenuItemClick = "MakeReportPublic('" + sender.id + "', '" + path + "', true)";
    lnkMakeReportPublic.setAttribute("id", "divPublic");

    if (isPrivate == "true") {
        lnkMakeReportPublic.Display = "";
        menu.Items.push(lnkMakeReportPublic);
    } else {
        lnkMakeReportPublic.Display = "none";
        menu.Items.push(lnkMakeReportPublic);
    }


    if (typeof (DeleteSavedReport) != "undefined") {
        var lnkDelete = document.createElement("div");
        lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
        lnkDelete.innerHTML = LoadLanguageText("Delete");
        lnkDelete.MenuItemClick = "DeleteSavedReport(this, '" + directory.replace(/(\r\n|\n|\r)/gm, " ") + "', '" + sender.parentNode.getElementsByClassName("DirectoryName")[0].innerText.trim().replace(/(\r\n|\n|\r)/gm, " ") + "');";
        // console.log(directory.replace(/(\r\n|\n|\r)/gm, " ") + ' ' + sender.parentNode.innerText.trim().replace(/(\r\n|\n|\r)/gm, " "))

        if (isPrivate != "NA") {
            menu.Items.push(lnkDelete);
        }
    }

    menu.Render();
}

function CopyHyperlink(idSavedReport, directory) {
    try {
        if (window.location.href.indexOf("Default.aspx") > -1) {
            //copyToClipboard(window.location.href.replace("Default.aspx", "LinkReporter/Crosstabs.aspx?SavedReport=") + idSavedReport);
            if (copyToClipboard(window.location.href.replace("Default.aspx", "LinkReporter/Crosstabs.aspx?SavedReport=") + idSavedReport)) {
                _AjaxRequest("LinkCloud.aspx", "CopyHyperlinkSuccess", "Value=" + encodeURI(idSavedReport) + "&Path=" + encodeURI(directory), function (response) {
                    ShowMessage(LoadLanguageText("CopyHyperlinkSuccess"), "Success");
                });
            }
        }
        else {
            if (copyToClipboard(window.location.href.replace("LinkCloud.aspx", "LinkReporter/Crosstabs.aspx?SavedReport=") + idSavedReport)) {
                _AjaxRequest("LinkCloud.aspx", "CopyHyperlinkSuccess", "Value=" + encodeURI(idSavedReport) + "&Path=" + encodeURI(directory), function (response) {
                    ShowMessage(LoadLanguageText("CopyHyperlinkSuccess"), "Success");
                });

            }

        }

    }
    catch (e) {
        ShowMessage(LoadLanguageText("CopyHyperlinkError"), "Error");
    }
}

function PinToHomescreen(idSavedReport) {
    AjaxRequest("PinToHomescreen", "IdSavedReport=" + idSavedReport, function (response) {
        window.location = "/Pages/Default.aspx";
    });
}

function MakeReportPrivate(idSender, path, isFile) {
    var sender = document.getElementById(idSender);
    document.getElementById("divPrivate").parentElement.parentElement.style.display = 'none'
    document.getElementById("divPublic").parentElement.parentElement.style.display = ''
    _AjaxRequest("LinkCloud.aspx", "MakeReportPrivate", "IsFile=" + isFile + "&Path=" + path, function (response) {
        window.location = window.location;
    });

}
function MakeReportPublic(idSender, path, isFile) {
    var sender = document.getElementById(idSender);
    document.getElementById("divPrivate").parentElement.parentElement.style.display = ''
    document.getElementById("divPublic").parentElement.parentElement.style.display = 'none'

    _AjaxRequest("LinkCloud.aspx", "MakeReportPublic", "IsFile=" + isFile + "&Path=" + path, function (response) {
        window.location = window.location;
    });

}

function RenameReport(idSender, path, isFile) {
    var sender = document.getElementById(idSender);
    var label = GetChildByAttribute(sender.parentNode, "class", "DirectoryName");
    if (label == undefined)
        return;
    label.setAttribute("contenteditable", "true");
    label.contenteditable = true;
    label.focus();
    placeCaretAtEnd(label);
    label.onblur = function () {
        _AjaxRequest("LinkCloud.aspx", "RenameReport", "IsFile=" + isFile + "&Name=" + encodeURI(label.textContent) + "&Path=" + path, function (response) {
            window.location = window.location;
        });
        this.onblur = undefined;
    };
}

function MoveReport(idSender, path, isFile) {
    var sender = document.getElementById(idSender);
    sessionStorage.fileSelectedForMove = sender.getAttribute("source");
}
function ShowTreeview(path) {
    InitDragBox('box1Control');
    _AjaxRequest("LinkCloud.aspx", "ShowTreeview", "source=" + path, function (response) {
        document.getElementById("box1").display = "block";
    });
}

function MoveReportConfirm(source) {

    _AjaxRequest("LinkCloud.aspx", "MoveReport", "Source=" + sessionStorage.getItem("fileSelectedForMove") + "&destination=" + encodeURI(source), function (response) {
        sessionStorage.setItem("fileSelectedForMove", "");
        window.location = window.location;
    });


}

function ManualSelect(sender, path) {
    isFolderSelectedToMove = true;
    _AjaxRequest("LinkCloud.aspx", "updateSelectedtreeNode", "destination=" + encodeURI(path), function (response) {
    });
}

function GotoLibrary(user) {
    _AjaxRequest("Default.aspx", "GotoLibrary", "Value=" + encodeURI(user), function (response) {
        window.location = "/Pages/LinkCloud.aspx";
    })
}