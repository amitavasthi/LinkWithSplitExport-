function ShowSubNavigation(sender, idPanel) {

    var elements = document.getElementById("pnlNavigation").getElementsByTagName("table");

    for (var i = 0; i < elements.length; i++) {
        var element = elements.item(i);

        if (element.getAttribute("state") == undefined ||
            element.getAttribute("state") != "Expanded")
            continue;

        if (IsNavigationParent(element, sender))
            continue;

        element.onclick();
    }

    sender.className = sender.getAttribute("CssClassActive");

    var pnlSubNavigation = document.getElementById(idPanel);

    pnlSubNavigation.style.visibility = "hidden";
    pnlSubNavigation.style.position = "absolute";
    pnlSubNavigation.style.height = "";
    pnlSubNavigation.style.display = "";

    ResizeNavigation(pnlSubNavigation);

    var height = pnlSubNavigation.offsetHeight;

    pnlSubNavigation.style.visibility = "";
    pnlSubNavigation.style.position = "";
    pnlSubNavigation.style.height = "0px";

    IncreaseHeight(pnlSubNavigation, height, function () {
        pnlSubNavigation.style.height = "";
    });

    sender.setAttribute("state", "Expanded");
    sender.setAttribute("onclick", "HideSubNavigation(this, '" + idPanel + "');");

    window.setTimeout(ResizeNavigation, 300);
}

function HideSubNavigation(sender, idPanel) {

    var pnlSubNavigation = document.getElementById(idPanel);

    DecreaseHeight(pnlSubNavigation, 0, function () {
        sender.className = sender.getAttribute("CssClass");
        pnlSubNavigation.style.display = "none";

        ResizeNavigation();
    });

    sender.setAttribute("state", "Collapsed");
    sender.setAttribute("onclick", "ShowSubNavigation(this, '" + idPanel + "');");
}

function IsNavigationParent(parent, element) {
    var items = parent.parentNode.getElementsByTagName("table");

    for (var i = 0; i < items.length; i++) {
        if (items.item(i) == element)
            return true;
    }

    return false;
}


function ResizeNavigation(pnlSubNavigation) {
    return;

    var pnlNavigation = document.getElementById("pnlNavigation");

    var top = GetOffsetTop(pnlNavigation);

    var height = window.innerHeight - 67;
    //height -= GetChildByAttribute(pnlNavigation, "class", "NavigationUserInfo BackgroundColor1", true).offsetHeight;
    height -= 152;

    var childs = pnlNavigation.getElementsByTagName("table");

    var count = 0;

    for (var i = 0; i < childs.length; i++) {
        if (!IsNavigationItemVisible(childs.item(i)))
            continue;

        if (childs.item(i).parentNode.parentNode.parentNode.id == "hbSettings")
            continue;

        count++;
    }

    height /= count;

    for (var i = 0; i < childs.length; i++) {
        if (pnlSubNavigation != undefined && childs.item(i).parentNode.parentNode == pnlSubNavigation) {
            childs.item(i).style.height = height + "px";
        }
        else {
            if (childs.item(i).offsetHeight > height) {
                DecreaseHeight(childs.item(i), height);
            }
            else {
                IncreaseHeight(childs.item(i), height);
            }
        }
    }

    // Temp:
    //window.setTimeout(ResizeNavigation, 500);
}

loadFunctions.push(ResizeNavigation);

function IsNavigationItemVisible(navigationItem) {
    var parent = navigationItem.parentNode;

    while (true) {
        if (parent.style.display == "none")
            return false;

        if (parent.id == "pnlNavigation")
            return true;

        parent = parent.parentNode;
    }
}

function ShowLeaveMessage(name, buttons, leaveActionPositive, leaveActionNegative) {
    CreateConfirmBox(LoadLanguageText(name), function () {
        eval(leaveActionPositive);
    }, function () {
        eval(leaveActionNegative);
    });
}


function SelectUserImage() {
    var fuUserImage = document.createElement("input");
    fuUserImage.type = "file";
    fuUserImage.style.height = "0px";
    fuUserImage.style.width = "0px";
    fuUserImage.name = "fuUserImage";
    fuUserImage.accept = "image/*";

    document.forms[0].appendChild(fuUserImage);

    fuUserImage.onchange = function () {
        if (fuUserImage.files.length === 0) {
            return;
        }

        var data = new FormData();
        data.append('SelectedFile', fuUserImage.files[0]);

        var request = new XMLHttpRequest();
        request.onreadystatechange = function () {
            if (request.readyState == 4) {
                try {
                    window.location = window.location;
                } catch (e) {
                }
                console.log(resp.status + ': ' + resp.data);
            }
        };

        var url = window.location.toString();

        if (url.search("\\?") == -1) {
            url += "?";
        } else {
            url += "&";
        }

        url += "fuUserImage=True";

        request.open("POST", url);
        request.send(data);
    }

    fuUserImage.click();
}