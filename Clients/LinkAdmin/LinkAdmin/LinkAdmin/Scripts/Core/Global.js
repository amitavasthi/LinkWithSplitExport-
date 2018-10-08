var loadFunctions = new Array();

function Body_Load() {
    document.getElementById("ContentPanel").style.height = (window.innerHeight - 106) + "px";

    for (var i = 0; i < loadFunctions.length; i++) {
        loadFunctions[i]();
    }
};

function GetChildByAttribute(parent, attributeName, attributeValue, recursive) {
    if (parent == undefined)
        return undefined;

    for (var i = 0; i < parent.childNodes.length; i++) {
        var child = parent.childNodes.item(i);

        if (child.getAttribute && child.getAttribute(attributeName) != undefined &&
            child.getAttribute(attributeName) == attributeValue) {
            return child;
        }

        if (recursive) {
            var result = GetChildByAttribute(
                child,
                attributeName,
                attributeValue,
                true
            );

            if (result != undefined)
                return result;
        }
    }

    return undefined;
}

function GetChildsByAttribute(parent, attributeName, attributeValue, recursive) {
    var result = new Array();

    if (parent == undefined || parent.childNodes == undefined)
        return result;

    for (var i = 0; i < parent.childNodes.length; i++) {
        var child = parent.childNodes.item(i);

        if (child.getAttribute && child.getAttribute(attributeName) != undefined &&
            attributeValue == undefined) {
            result.push(child);
        }
        else if (child.getAttribute && child.getAttribute(attributeName) != undefined &&
            child.getAttribute(attributeName) == attributeValue) {
            result.push(child);
        }
        else {
            if (recursive) {
                var cResult = GetChildsByAttribute(
                    child,
                    attributeName,
                    attributeValue,
                    true
                );

                for (var a = 0; a < cResult.length; a++) {
                    result.push(cResult[a]);
                }
            }
        }
    }

    return result;
}

function Navigate(sender, url) {
    document.getElementById("ContentPanel").innerHTML = "<table style=\"width:100%;height:100%;\"><tr><td style=\"text-align:center;\"><div class=\"spinner2\" style=\"display:inline-block;width:20px;height:20px;\">" +
        "<div class=\"double-bounce1\"></div>" +
        "<div class=\"double-bounce2\"></div>" +
      "</div></td></tr></table>";

    if (sender != undefined) {
        var navigationItems = GetChildsByAttribute(sender.parentNode, "class", "NavigationItem");

        for (var i = 0; i < navigationItems.length; i++) {
            navigationItems[i].style.background = "";
        }

        sender.style.background = "#FFFFFF";
    }

    var result = "<div style=\"margin:20px\">"
    result += Request("/Navigation.ashx?Page=" + url, "", "");
    result += "</div>";

    document.getElementById("ContentPanel").innerHTML = result;

    var scripts = document.getElementById("ContentPanel").getElementsByTagName("script");

    for (var i = 0; i < scripts.length; i++) {
        eval(scripts.item(i).innerHTML);
    }
}

function IncreaseHeight(control, target, onFinish, applyMarginTop) {
    if (control["ScaleMode"] == "Decreasing")
        return;

    control["ScaleMode"] = "Increasing";

    var height = NaN;

    if (control.style != undefined)
        height = parseInt(control.style.height);

    if (isNaN(height))
        height = control.offsetHeight;

    if (height < target) {
        var factor = (Math.abs((target / 2) - height));
        factor = Math.abs((target / 2) - factor);

        factor = factor * 30 / 100;

        /*var factor = (target - height) * 30 / 100;

        factor = parseInt(factor);*/

        if (factor < 1)
            factor = 1;

        control.style.height = (height + factor) + "px";

        if (applyMarginTop) {
            control.style.marginTop = ((target - parseInt(control.style.height)) / 2) + "px";
        }

        window.setTimeout(function () {
            IncreaseHeight(control, parseInt(target), onFinish, applyMarginTop)
        }, 20);
    }
    else {
        control.style.height = target + "px";
        control["ScaleMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}

function DecreaseZoom(control, target, onFinish) {
    control["ZoomMode"] = "Decreasing";

    var zoom = NaN;

    if (control.style != undefined)
        zoom = parseFloat(control.style.zoom);

    if (isNaN(zoom))
        zoom = 1.0;

    if (control["DecreaseZoomStart"] == undefined)
        control["DecreaseZoomStart"] = zoom;

    if (zoom > target) {
        var factor = (Math.abs((control["DecreaseZoomStart"] / 2) - zoom));
        factor = Math.abs((control["DecreaseZoomStart"] / 2) - factor);

        factor = factor * 30 / 100;

        if (factor == 0 || isNaN(factor))
            factor = 0.01;

        control.style.zoom = (zoom - factor);

        window.setTimeout(function () {
            DecreaseZoom(control, parseFloat(target), onFinish)
        }, 20);
    }
    else {
        control["ZoomMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}


function GetOffsetLeft(element) {
    var result = element.offsetLeft;

    if (isNaN(result))
        result = 0;

    if (element.offsetParent != undefined)
        result += GetOffsetLeft(element.offsetParent);

    return result;
}

function GetOffsetTop(element, addAbsolutes) {
    if (addAbsolutes == undefined)
        addAbsolutes = false;

    if (element == undefined)
        return 0;

    var result = 0;

    if (addAbsolutes || element.style.position != "absolute")
        result += element.offsetTop;

    if (isNaN(result))
        result = 0;

    if (element.offsetParent != undefined)
        result += GetOffsetTop(element.offsetParent, addAbsolutes);

    return result;
}

String.prototype.removeAt = function (index, charcount) {
    return this.substr(0, index) + this.substr(index + charcount);
}
