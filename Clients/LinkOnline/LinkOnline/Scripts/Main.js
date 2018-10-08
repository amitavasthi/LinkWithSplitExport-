(function (i, s, o, g, r, a, m) {
    i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
        (i[r].q = i[r].q || []).push(arguments)
    }, i[r].l = 1 * new Date(); a = s.createElement(o),
    m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
})(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

ga('create', 'UA-71649221-1', 'auto');
ga('send', 'pageview');


var loadFunctions = new Array();
var resizeFunctions = new Array();
var submitFunctions = new Array();
var bodyMouseUp = new Array();
var mouseMoveFunctions = new Object();

var showSubmitLoading = true;
var showLoadLoading = true;
var holdingShift = false;

var ContentHeight = -1;
var ContentWidth = -1;


mouseMoveFunctions["Keys"] = new Array();

mouseMoveFunctions.Add = function (key, method) {
    mouseMoveFunctions["Keys"].push(key);

    mouseMoveFunctions[key] = method;
}

mouseMoveFunctions.Delete = function (key) {
    var keyIndex = -1;

    for (var i = 0; i < mouseMoveFunctions["Keys"].length; i++) {
        if (mouseMoveFunctions["Keys"][i] == key) {
            keyIndex = i;
            break;
        }
    }

    if (keyIndex == -1)
        return;

    mouseMoveFunctions["Keys"].splice(keyIndex, 1);

    mouseMoveFunctions[key] = undefined;
}

function SetPageDimensions(sendDimensions) {
    if (sendDimensions == undefined)
        sendDimensions = true;

    var main = document.getElementById("Main");
    var headline = document.getElementById("Headline");
    var nonScroll = document.getElementById("NonScroll");
    var content = document.getElementById("Content");
    var footer = document.getElementById("Footer");

    ContentHeight = window.innerHeight - (headline.offsetHeight + footer.offsetHeight + nonScroll.offsetHeight);
    ContentWidth = main.offsetWidth;

    content.style.height = ContentHeight + "px";

    if (sendDimensions == true) {
        try {
            AjaxRequest("SetContentWidth", "Value=" + ContentWidth, function (response) {
            });
            AjaxRequest("SetContentHeight", "Value=" + window.innerHeight, function (response) {
            });
        } catch (e) { }
    }

    var elements = GetChildsByAttribute(document.body, "HeightScript", undefined, true);

    for (var i = 0; i < elements.length; i++) {
        elements[i].style.height = eval(elements[i].getAttribute("HeightScript")) + "px";
    }

    elements = GetChildsByAttribute(document.body, "WidthScript", undefined, true);

    for (var i = 0; i < elements.length; i++) {
        elements[i].style.width = eval(elements[i].getAttribute("WidthScript")) + "px";
    }
}

// Set the body's onload function.
function Load() {

    //refresh crosstable if page load first..
    if (document.getElementsByClassName("TableCellHeadlineLeftVariable").length > 0 || document.getElementsByClassName("TableCellHeadlineTopVariable").length > 0)
        if (document.getElementsByClassName("RendererV2").length != 0 && document.getElementsByClassName("RendererV2")[0].getElementsByTagName("tr").length == 0) {
            UpdateSetting('AutoUpdate', 'true', true, true);
        }

    // Set the page's dimensions.
    SetPageDimensions();

    // Run through all load functions.
    for (var i = 0; i < loadFunctions.length; i++) {

        try {
            // Invoke the load function.
            loadFunctions[i]();
        }
        catch (e) { }
    }

    // Enable the page.
    document.forms[0].style.visibility = "visible";

    HideLoading();

    document.body.onresize = function () {

        /*SetPageDimensions(false);

        try {
            AjaxRequest("SetContentWidth", "Value=" + ContentWidth, function (response) {
                AjaxRequest("SetContentHeight", "Value=" + window.innerHeight, function (response) {
                    ExecuteResizeFunctions();
                });
            });
        } catch (e) {
            ExecuteResizeFunctions();
        }*/
        /*rtime = new Date();
        if (timeout === false) {
            timeout = true;
            setTimeout(resizeend, delta);
        }*/
    };

    document.forms[0].onsubmit = function () {
        // Run through all submit functions.
        for (var i = 0; i < submitFunctions.length; i++) {
            try {
                // Invoke the submit funtion.
                submitFunctions[i]();
            }
            catch (e) { }
        }

        if (showSubmitLoading)
            ShowLoading(document.body);
    };

    document.body.onmouseup = function () {
        // Run through all mouse up functions.
        for (var i = 0; i < bodyMouseUp.length; i++) {
            try {
                // Invoke the mouse up funtion.
                bodyMouseUp[i]();
            }
            catch (e) { }
        }
    };

    document.body.onmousemove = function (e) {
        getMouseXY(e);

        // Run through all mouse move functions.
        for (var i = 0; i < mouseMoveFunctions["Keys"].length; i++) {
            try {
                var key = mouseMoveFunctions["Keys"][i];

                // Invoke the mouse move funtion.
                mouseMoveFunctions[key](e);
            }
            catch (e) { }
        }
    };

    document.body.onkeydown = function (e) {
        if (e.keyCode == 16)
            holdingShift = true;
    };

    document.body.onkeyup = function (e) {
        if (e.keyCode == 16)
            holdingShift = false;
    };
}

function PageResize() {
    SetPageDimensions(false);

    try {
        AjaxRequest("SetContentWidth", "Value=" + ContentWidth, function (response) {
            AjaxRequest("SetContentHeight", "Value=" + window.innerHeight, function (response) {
                ExecuteResizeFunctions();
            });
        });
    } catch (e) {
        ExecuteResizeFunctions();
    }
}

function Logout() {

    DecreaseOpacity(document.body, function () {
        window.location = "/Pages/Login.aspx?Logout=True";
    });;
}


function RenderControl(assembly, name, arguments, onFinish) {
    _AjaxRequest("/Handlers/WebUtilitiesHandler.ashx", "RenderControl", "Assembly=" + assembly + "&Name=" + name + "&" + arguments, function (response) {

        if (onFinish != undefined)
            onFinish(response);
    });
}


function ShowMessage(message, type, hide) {
    if (hide == undefined)
        hide = true;

    var messageBox = document.createElement("div");
    messageBox.id = "MessageBox";
    messageBox.className = "MessageBox MessageBox" + type;
    messageBox.innerHTML = "<table><tr><td><img height=\"30\" src=\"/Images/Icons/" + type + ".png\" /></td><td>" + message + "</td></tr></table>";

    document.body.appendChild(messageBox);

    messageBox.style.left = ((window.innerWidth / 2) - (messageBox.offsetWidth / 2)) + "px";

    if (hide) {
        window.setTimeout(function () {
            DecreaseOpacity(document.getElementById("MessageBox"), function () {
                var messageBox = document.getElementById("MessageBox");

                messageBox.parentNode.removeChild(messageBox);
            })
        }, 3000);
    }
}

// For test only:
/*loadFunctions.push(function () {
    ShowMaintenanceWarning(60 * 5);
});*/

function ShowMaintenanceWarning(eta) {
    ShowMessage(
        LoadLanguageText("MaintenanceWarning"),
        "Warning",
        false
    );

    document.getElementById("MaintenanceWarningMinutes");
}


function IncreaseOpacity(control, speed, target) {
    if (control["OpacityMode"] == "Decreasing")
        return;

    if (target == undefined)
        target = 1.0;

    if (speed == undefined)
        speed = 50;

    control["OpacityMode"] = "Increasing";

    var opacity = parseFloat(control.style.opacity);

    if (opacity < target) {
        /*var factor = (1.0 - opacity) * 20 / 100;
    
        if (factor < 0.05)
            factor = 0.05;*/

        factor = 0.1;

        opacity += factor;

        opacity = parseInt(opacity * 100) / 100;

        control.style.opacity = opacity;

        window.setTimeout(function () { IncreaseOpacity(control, speed, target) }, speed);
    }
    else {
        control["OpacityMode"] = "none";
    }
}

function DecreaseOpacity(control, onFinish, speed) {
    control["OpacityMode"] = "Decreasing";

    if (speed == undefined)
        speed = 100;

    var opacity = parseFloat(control.style.opacity);

    if (isNaN(opacity))
        opacity = 1.0;

    if (opacity > 0.0) {
        /*var factor = (1.0 - opacity) * 20 / 100;

        if (factor < 0.05)
            factor = 0.05;*/

        factor = 0.1;

        opacity -= factor;

        opacity = parseInt(opacity * 100) / 100;

        control.style.opacity = opacity;

        window.setTimeout(function () { DecreaseOpacity(control, onFinish, speed) }, speed);
    }
    else {
        control["OpacityMode"] = "none";

        if (onFinish != undefined)
            onFinish(control);
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

function DecreaseHeight(control, target, onFinish, applyMarginTop) {
    control["ScaleMode"] = "Decreasing";

    var height = NaN;

    if (control.style != undefined)
        height = parseInt(control.style.height);

    if (isNaN(height))
        height = control.offsetHeight;

    if (control["DecreaseHeightStart"] == undefined)
        control["DecreaseHeightStart"] = height;

    if (height > target) {
        var factor = (Math.abs((control["DecreaseHeightStart"] / 2) - height));
        factor = Math.abs((control["DecreaseHeightStart"] / 2) - factor);

        factor = factor * 30 / 100;

        if (factor < 1 || isNaN(factor))
            factor = 1;

        if (factor < 1)
            factor = 1;

        control.style.height = (height - factor) + "px";

        if (applyMarginTop) {
            control.style.marginTop = ((control["DecreaseHeightStart"] - parseInt(control.style.height)) / 2) + "px";
        }

        window.setTimeout(function () {
            DecreaseHeight(control, parseInt(target), onFinish, applyMarginTop)
        }, 20);
    }
    else {
        control["ScaleMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}


function IncreaseLeft(control, target, onFinish) {
    if (control["PositionLeftMode"] == "Decreasing")
        return;

    control["PositionLeftMode"] = "Increasing";

    var left = NaN;

    if (control.style != undefined)
        left = parseInt(control.style.left);

    if (isNaN(left))
        left = control.offsetLeft;

    if (left < target) {
        var factor = (Math.abs((target / 2) - left));
        factor = Math.abs((target / 2) - factor);

        factor = factor * 30 / 100;

        /*var factor = (target - height) * 30 / 100;

        factor = parseInt(factor);*/

        if (factor < 1)
            factor = 1;

        control.style.left = (left + factor) + "px";

        window.setTimeout(function () {
            IncreaseLeft(control, parseInt(target), onFinish)
        }, 20);
    }
    else {
        control.style.left = target + "px";
        control["PositionLeftMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}

function DecreaseLeft(control, target, onFinish) {
    control["PositionLeftMode"] = "Decreasing";

    var left = NaN;

    if (control.style != undefined)
        left = parseInt(control.style.left);

    if (isNaN(left))
        left = control.offsetLeft;

    if (control["DecreaseLeftStart"] == undefined)
        control["DecreaseLeftStart"] = left;

    if (left > target) {
        /*var factor = (Math.abs((control["DecreaseLeftStart"] / 2) - left));
        factor = Math.abs((control["DecreaseLeftStart"] / 2) - factor);

        factor = factor * 30 / 100;

        if (factor < 1 || isNaN(factor))
            factor = 1;*/
        var factor = (Math.abs((target / 2) - left));
        factor = Math.abs((target / 2) - factor);

        factor = factor * 30 / 100;

        if (factor < 1)
            factor = 1;

        /*var factor = (height - target) * 30 / 100;

        factor = parseInt(factor);*/

        if (factor < 1)
            factor = 1;

        left -= factor;

        if (left < target)
            left = target;

        control.style.left = left + "px";

        window.setTimeout(function () {
            DecreaseLeft(control, parseInt(target), onFinish)
        }, 20);
    }
    else {
        control["PositionLeftMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}

function IncreaseRight(control, target, onFinish) {
    if (control["PositionRightMode"] == "Decreasing")
        return;

    control["PositionRightMode"] = "Increasing";

    var right = NaN;

    if (control.style != undefined)
        right = parseInt(control.style.right);

    if (isNaN(right))
        right = control.offsetRight;

    if (right < target) {
        var factor = (Math.abs((target / 2) - right));
        factor = Math.abs((target / 2) - factor);

        factor = factor * 30 / 100;

        /*var factor = (target - height) * 30 / 100;

        factor = parseInt(factor);*/

        if (factor < 1)
            factor = 1;

        control.style.right = (right + factor) + "px";

        window.setTimeout(function () {
            IncreaseRight(control, parseInt(target), onFinish)
        }, 20);
    }
    else {
        control.style.right = target + "px";
        control["PositionRightMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}

function DecreaseRight(control, target, onFinish) {
    control["PositionRightMode"] = "Decreasing";

    var right = NaN;

    if (control.style != undefined)
        right = parseInt(control.style.right);

    if (isNaN(right))
        right = control.offsetRight;

    if (control["DecreaseRightStart"] == undefined)
        control["DecreaseRightStart"] = right;

    if (right > target) {
        /*var factor = (Math.abs((control["DecreaseRightStart"] / 2) - right));
        factor = Math.abs((control["DecreaseRightStart"] / 2) - factor);

        factor = factor * 30 / 100;

        if (factor < 1 || isNaN(factor))
            factor = 1;*/
        var factor = (Math.abs((target / 2) - right));
        factor = Math.abs((target / 2) - factor);

        factor = factor * 30 / 100;

        if (factor < 1)
            factor = 1;

        /*var factor = (height - target) * 30 / 100;

        factor = parseInt(factor);*/

        if (factor < 1)
            factor = 1;

        right -= factor;

        if (right < target)
            right = target;

        control.style.right = right + "px";

        window.setTimeout(function () {
            DecreaseRight(control, parseInt(target), onFinish)
        }, 20);
    }
    else {
        control["PositionRightMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}


function IncreaseTop(control, target, onFinish) {
    if (control["PositionTopMode"] == "Decreasing")
        return;

    control["PositionTopMode"] = "Increasing";

    var top = NaN;

    if (control.style != undefined)
        top = parseInt(control.style.top);

    if (isNaN(top))
        top = control.offsetTop;

    if (top < target) {
        var factor = (Math.abs((target / 2) - top));
        factor = Math.abs((target / 2) - factor);

        factor = factor * 30 / 100;

        /*var factor = (target - height) * 30 / 100;

        factor = parseInt(factor);*/

        if (factor < 1)
            factor = 1;

        control.style.top = (top + factor) + "px";

        window.setTimeout(function () {
            IncreaseTop(control, parseInt(target), onFinish)
        }, 20);
    }
    else {
        control.style.top = target + "px";
        control["PositionTopMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}

function DecreaseTop(control, target, onFinish) {
    control["PositionTopMode"] = "Decreasing";

    var top = NaN;

    if (control.style != undefined)
        top = parseInt(control.style.top);

    if (isNaN(top))
        top = control.offsetTop;

    if (control["DecreaseTopStart"] == undefined)
        control["DecreaseTopStart"] = top;

    if (top > target) {
        /*var factor = (Math.abs((control["DecreaseTopStart"] / 2) - top));
        factor = Math.abs((control["DecreaseTopStart"] / 2) - factor);

        factor = factor * 30 / 100;*/
        var factor = (Math.abs((target / 2) - top));
        factor = Math.abs((target / 2) - factor);

        factor = factor * 30 / 100;

        if (factor < 1 || isNaN(factor))
            factor = 1;

        /*var factor = (height - target) * 30 / 100;

        factor = parseInt(factor);*/

        if (factor < 1)
            factor = 1;

        top -= factor;

        if (top < target)
            top = target;

        control.style.top = top + "px";

        window.setTimeout(function () {
            DecreaseTop(control, parseInt(target), onFinish)
        }, 20);
    }
    else {
        control["PositionTopMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}

function IncreaseBottom(control, target, onFinish) {
    if (control["PositionBottomMode"] == "Decreasing")
        return;

    control["PositionBottomMode"] = "Increasing";

    var bottom = NaN;

    if (control.style != undefined)
        bottom = parseInt(control.style.bottom);

    if (bottom < target) {
        var factor = (Math.abs((target / 2) - bottom));
        factor = Math.abs((target / 2) - factor);

        factor = factor * 30 / 100;

        /*var factor = (target - height) * 30 / 100;

        factor = parseInt(factor);*/

        if (factor < 1)
            factor = 1;

        control.style.bottom = (bottom + factor) + "px";

        window.setTimeout(function () {
            IncreaseBottom(control, parseInt(target), onFinish)
        }, 20);
    }
    else {
        control.style.bottom = target + "px";
        control["PositionBottomMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}

function DecreaseBottom(control, target, onFinish) {
    control["PositionBottomMode"] = "Decreasing";

    var bottom = NaN;

    if (control.style != undefined)
        bottom = parseInt(control.style.bottom);

    if (control["DecreaseBottomStart"] == undefined)
        control["DecreaseBottomStart"] = bottom;

    if (bottom > target) {
        var factor = (Math.abs((control["DecreaseBottomStart"] / 2) - bottom));
        factor = Math.abs((control["DecreaseBottomStart"] / 2) - factor);

        factor = factor * 30 / 100;

        if (factor < 1 || isNaN(factor))
            factor = 1;

        /*var factor = (height - target) * 30 / 100;

        factor = parseInt(factor);*/

        if (factor < 1)
            factor = 1;

        bottom -= factor;

        if (bottom < target)
            bottom = target;

        control.style.bottom = bottom + "px";

        window.setTimeout(function () {
            DecreaseBottom(control, parseInt(target), onFinish)
        }, 20);
    }
    else {
        control["PositionBottomMode"] = "none";

        if (onFinish != undefined)
            onFinish();
    }
}


function IncreaseWidth(control, target, applyMarginLeft, onFinish) {
    if (control["WidthMode"] == "Decreasing")
        return;

    control["WidthMode"] = "Increasing";

    var width = parseFloat(control.style.width);

    if (isNaN(width))
        width = control.offsetWidth;

    if (width < target) {
        var factor = (Math.abs((target / 2) - width));
        factor = Math.abs((target / 2) - factor);

        factor = factor * 30 / 100;

        if (factor < 1)
            factor = 1;

        //factor = 1;

        width += factor;

        width = parseInt(width);

        control.style.width = width + "px";

        if (applyMarginLeft)
            control.style.marginLeft = "-" + width + "px";

        window.setTimeout(function () { IncreaseWidth(control, parseInt(target), applyMarginLeft, onFinish) }, 20);
    }
    else {
        control["WidthMode"] = "none";

        if (onFinish != undefined)
            onFinish(control);
    }
}

function DecreaseWidth(control, target, applyMarginLeft, onFinish) {
    control["WidthMode"] = "Decreasing";

    var width = parseFloat(control.style.width);

    if (isNaN(width))
        width = control.offsetWidth;

    if (control["DecreaseWidthStart"] == undefined)
        control["DecreaseWidthStart"] = width;

    if (width > target) {
        //var factor = (width - target) * 20 / 100;
        var factor = (Math.abs((control["DecreaseWidthStart"] / 2) - width));
        factor = Math.abs((control["DecreaseWidthStart"] / 2) - factor);

        factor = factor * 30 / 100;

        if (factor < 1 || isNaN(factor))
            factor = 1;

        //factor = 1;

        width -= factor;

        width = parseInt(width);

        if (width < target)
            width = target;

        control.style.width = width + "px";

        if (applyMarginLeft)
            control.style.marginLeft = "-" + width + "px";

        window.setTimeout(function () { DecreaseWidth(control, parseInt(target), applyMarginLeft, onFinish) }, 20);
    }
    else {
        control["WidthMode"] = "none";
        control["DecreaseWidthStart"] = undefined;

        if (onFinish != undefined) {
            onFinish();
        }
    }
}


function IncreaseScrollTop(control, target) {
    if (control["ScaleMode"] == "Decreasing")
        return;

    control["ScaleMode"] = "Increasing";

    var height = NaN;

    height = control.scrollTop;

    height = parseInt(height) + 1;

    if (height != target) {
        var factor = (Math.abs((target / 2) - height));
        factor = Math.abs((target / 2) - factor);

        factor = factor * 30 / 100;

        factor = 5;
        /*var factor = (target - height) * 30 / 100;

        factor = parseInt(factor);*/

        factor = parseInt(factor);

        if (factor < 1)
            factor = 1;

        if (height < target)
            height += factor;
        else
            height -= factor;

        height = parseInt(height);

        control.scrollTop = height;

        if (parseInt(control.scrollTop) != height) {
            control["ScaleMode"] = "none";
            return;
        }

        window.setTimeout(function () {
            IncreaseScrollTop(control, parseInt(target))
        }, 100);
    }
    else {
        control["ScaleMode"] = "none";
    }
}


function Rotate(control, target) {
    var rotation = 0;

    if (control.style.transform != undefined && control.style.transform != "")
        rotation = parseInt(control.style.transform.split('rotate(')[1].split('deg)')[0]);

    if (rotation != target) {
        var factor = (Math.abs((target / 2) - rotation));
        factor = Math.abs((target / 2) - factor);

        factor = parseInt(factor * 20 / 100);

        if (factor < 1)
            factor = 1;

        if (rotation > target) {

            if ((rotation - factor) < target)
                rotation = target;
            else
                rotation -= factor;
        }
        else {

            if ((rotation + factor) > target)
                rotation = target;
            else
                rotation += factor;
        }

        control.style.transform = "rotate(" + rotation + "deg)";

        window.setTimeout(function () {
            Rotate(control, target);
        }, 20);
    }
    else {

    }
}



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

function isOverflowed(element) {
    return element.scrollHeight > element.parentNode.clientHeight ||
        element.scrollWidth > element.parentNode.clientWidth;
}

function GetParentScrollTop(element) {
    var result = 0;

    if (element.scrollTop > 0)
        result = element.scrollTop;

    if (element.parentNode != undefined)
        result += (GetParentScrollTop(element.parentNode));

    return result;
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


function GetScrollLeft(element) {
    var result = element.scrollLeft;

    if (isNaN(result))
        result = 0;

    if (element.parentNode != undefined)
        result += GetScrollLeft(element.parentNode);

    return result;
}


var tempX;
var tempY;

// Detect if the browser is IE or not.
// If it is not IE, we assume that the browser is NS.
var IE = document.all ? true : false

// If NS -- that is, !IE -- then set up for mouse capture
if (!IE) document.captureEvents(Event.MOUSEMOVE)

// Sets the current mouse position
function getMouseXY(e) {
    try {
        if (IE) { // grab the x-y pos.s if browser is IE
            tempX = event.clientX + document.body.scrollLeft;
            tempY = event.clientY + document.body.scrollTop;
        }
        else {  // grab the x-y pos.s if browser is NS
            tempX = e.pageX;
            tempY = e.pageY;
        }
        //tempX = e.pageX
        //tempY = e.pageY

        if (tempX < 0) { tempX = 0 }
        if (tempY < 0) { tempY = 0 }

        document.getElementById("test2").innerHTML = tempY;
    } catch (e) {

    }

    return true
}

//for hiding Layout except go btn click
function ShowLoadingGo(parent, absolute, zoom, background) {
    if (parent == undefined)
        return;

    if (zoom == undefined)
        zoom = 1.0;

    if (absolute == undefined)
        absolute = true;

    if (background == undefined)
        background = "BackgroundColor1";

    var loadingBlur = document.createElement("div");
    loadingBlur.id = "LoadingBlur";
    loadingBlur.className = "LoadingBackground BackgroundColor1";

    if (absolute == false)
        loadingBlur.className = "LoadingBackground2";

    //loadingBlur.style.marginTop = "-" + (GetParentScrollTop(parent) / 2) + "px";
    loadingBlur.style.width = parent.offsetWidth + "px";
    loadingBlur.style.height = parent.offsetHeight + "px";

    var className = "Loading";

    if (absolute == false)
        className = "Loading2";

    var style = "";

    if (absolute)
        style = "margin-left:" + ((parent.offsetWidth / 2) - 25) + "px;margin-top:" + ((parent.offsetHeight / 2) - 25) + "px;";

    loadingBlur.innerHTML = "<div class=\"" + className + "\" style=\"zoom:" + zoom + ";" + style + "\">" +
        "<div class=\"cube1 " + background + "\">" +
        "</div>" +
        "<div class=\"cube2 " + background + "\">" +
        "</div>" +
    "<div id=\"LoadingText\" class=\"LoadingText Color1\"></div></div>";

    parent.insertBefore(loadingBlur, parent.childNodes.item(0));
}

function ShowLoading(parent, absolute, zoom, background) {
    if (parent == undefined)
        return;

    if (zoom == undefined)
        zoom = 1.0;

    if (absolute == undefined)
        absolute = true;

    if (background == undefined)
        background = "BackgroundColor1";

    var loadingBlur = document.createElement("div");
    loadingBlur.id = "LoadingBlur";
    loadingBlur.className = "LoadingBackground BackgroundColor1";

    if (absolute == false)
        loadingBlur.className = "LoadingBackground2";

    //loadingBlur.style.marginTop = "-" + (GetParentScrollTop(parent) / 2) + "px";
    loadingBlur.style.width = parent.offsetWidth + "px";
    loadingBlur.style.height = parent.offsetHeight + "px";

    var className = "Loading";

    if (absolute == false)
        className = "Loading2";

    var style = "";

    if (absolute)
        style = "margin-left:" + ((parent.offsetWidth / 2) - 25) + "px;margin-top:" + ((parent.offsetHeight / 2) - 25) + "px;";

    loadingBlur.innerHTML = "<div class=\"" + className + "\" style=\"zoom:" + zoom + ";" + style + "\">" +
        "<div class=\"cube1 " + background + "\">" +
        "</div>" +
        "<div class=\"cube2 " + background + "\">" +
        "</div>" +
    "<div id=\"LoadingText\" class=\"LoadingText Color1\"></div></div>";

    parent.insertBefore(loadingBlur, parent.childNodes.item(0));
}

function ShowLoading_Old(parent, absolute, zoom, background) {
    if (parent == undefined)
        return;

    if (zoom == undefined)
        zoom = 1.0;

    if (absolute == undefined)
        absolute = true;

    if (background == undefined)
        background = "BackgroundColor1";

    var loadingBlur = document.createElement("div");
    loadingBlur.id = "LoadingBlur";
    loadingBlur.className = "LoadingBackground";

    if (absolute == false)
        loadingBlur.className = "LoadingBackground2";

    //loadingBlur.style.marginTop = "-" + (GetParentScrollTop(parent) / 2) + "px";
    loadingBlur.style.width = parent.offsetWidth + "px";
    loadingBlur.style.height = parent.offsetHeight + "px";

    var className = "Loading";

    if (absolute == false)
        className = "Loading2";

    var style = "";

    if (absolute)
        style = "margin-left:" + ((parent.offsetWidth / 2) - 25) + "px;margin-top:" + ((parent.offsetHeight / 2) - 25) + "px;";

    loadingBlur.innerHTML = "<div class=\"" + className + "\" style=\"zoom:" + zoom + ";" + style + "\"><div id=\"circularG\">" +
        "<div id=\"circularG_1\" class=\"circularG " + background + "\">" +
        "</div>" +
        "<div id=\"circularG_2\" class=\"circularG " + background + "\">" +
        "</div>" +
        "<div id=\"circularG_3\" class=\"circularG " + background + "\">" +
        "</div>" +
        "<div id=\"circularG_4\" class=\"circularG " + background + "\">" +
        "</div>" +
        "<div id=\"circularG_5\" class=\"circularG " + background + "\">" +
        "</div>" +
        "<div id=\"circularG_6\" class=\"circularG " + background + "\">" +
        "</div>" +
        "<div id=\"circularG_7\" class=\"circularG " + background + "\">" +
        "</div>" +
        "<div id=\"circularG_8\" class=\"circularG " + background + "\">" +
        "</div>" +
    "</div><div id=\"LoadingText\" class=\"LoadingText Color1\"></div></div>";

    parent.insertBefore(loadingBlur, parent.childNodes.item(0));
}

function HideLoading() {
    var loadingBlur = document.getElementById("LoadingBlur");

    if (loadingBlur == undefined)
        return;

    loadingBlur.parentNode.removeChild(loadingBlur);

    HideLoading();
}

function HideChildren(parent) {
    for (var i = 0; i < parent.childNodes.length; i++) {
        if (parent.childNodes.item(i).style != undefined)
            parent.childNodes.item(i).style.visibility = "hidden";
    }
}

function ShowChildren(parent) {
    for (var i = 0; i < parent.childNodes.length; i++) {
        if (parent.childNodes.item(i).style != undefined)
            parent.childNodes.item(i).style.visibility = "";
    }
}


function ChildNodeIndex(collection, item) {

    for (var i = 0; i < collection.length; i++) {
        if (collection.item(i) == item)
            return i;
    }

    return -1;
}
function ArrayContains(array, item) {

    for (var i = 0; i < array.length; i++) {
        if (array[i] == item)
            return true;
    }

    return false;
}


function CreateConfirmBox(text, onConfirm, onCancel) {
    var background = document.createElement("div");
    var box = document.createElement("div");
    var lbl = document.createElement("div");

    background.className = "ConfirmBoxBackground";
    box.className = "ConfirmBox";
    lbl.className = "ConfirmBoxText";

    lbl.innerHTML = text;

    var btnConfirm = document.createElement("input");
    btnConfirm.type = "button";
    btnConfirm.value = LoadLanguageText("Yes");
    btnConfirm.className = "ConfirmBoxBtnConfirm";

    btnConfirm.onclick = function () {
        if (onConfirm != undefined)
            onConfirm();

        this["Box"].Close();
    }

    var btnCancel = document.createElement("input");
    btnCancel.type = "button";
    btnCancel.value = LoadLanguageText("No");
    btnCancel.className = "ConfirmBoxBtnCancel";

    btnCancel.onclick = function () {
        if (onCancel != undefined)
            onCancel();

        this["Box"].Close();
    };

    box.appendChild(lbl);
    box.appendChild(btnConfirm);
    box.appendChild(btnCancel);

    btnCancel["Box"] = box;
    btnConfirm["Box"] = box;

    box["Close"] = function () {
        box.parentNode.removeChild(box);
        background.parentNode.removeChild(background);
    }

    document.body.appendChild(background);
    document.body.appendChild(box);

    box.style.left = ((window.innerWidth / 2) - (box.offsetWidth / 2)) + "px";
    box.style.top = ((window.innerHeight / 2) - (box.offsetHeight / 2)) + "px";
}


var rtime = new Date(1, 1, 2000, 12, 00, 00);
var timeout = false;
var delta = 200;

function resizeend() {
    if (new Date() - rtime < delta) {
        setTimeout(resizeend, delta);
    } else {
        timeout = false;
        SetPageDimensions(false);

        try {
            AjaxRequest("SetContentWidth", "Value=" + ContentWidth, function (response) {
                AjaxRequest("SetContentHeight", "Value=" + window.innerHeight, function (response) {
                    ExecuteResizeFunctions();
                });
            });
        } catch (e) {
            ExecuteResizeFunctions();
        }
    }
}

function ExecuteResizeFunctions() {
    // Run through all resize functions.
    for (var i = 0; i < resizeFunctions.length; i++) {
        try {
            // Invoke the resize funtion.
            resizeFunctions[i]();
        }
        catch (e) { }
    }
}

//resizeFunctions.push(SetPageDimensions);

function SelectElementContents(el) {
    var range = document.createRange();
    range.selectNodeContents(el);
    var sel = window.getSelection();
    sel.removeAllRanges();
    sel.addRange(range);
}
function SetCursorToEnd(div) {
    window.setTimeout(function () {
        var sel, range;
        if (window.getSelection && document.createRange) {
            range = document.createRange();
            range.selectNodeContents(div);
            //range.collapse(true);
            sel = window.getSelection();
            sel.removeAllRanges();
            sel.addRange(range);
        } else if (document.body.createTextRange) {
            range = document.body.createTextRange();
            range.moveToElementText(div);
            range.collapse(true);
            range.select();
        }
    }, 1);
}


function copyToClipboard(text) {
    if (window.clipboardData) { // Internet Explorer
        var result = window.clipboardData.setData("Text", text);

        if (result === null) {
            return false; //break out of the function early
        }
        else if (result == undefined)
            throw "";
        else if (result != "") {
            return true;
        }
        //if (result == false)
        //    throw "";
    } else {
        var result = window.prompt("Copy to clipboard: Ctrl+C, Enter", text);
        if (result === null) {
            return false; //break out of the function early
        }
        else if (result == undefined)
            throw "";
        else if (result != "") {
            return true;
        }
    }
}

function htmlEncode(value) {
    //create a in-memory div, set it's inner text(which jQuery automatically encodes)
    //then grab the encoded contents back out.  The div never exists on the page.
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

/*for calling assistance module*/

function callFeedback() {
    $.feedback({
        html2canvasURL: '/Scripts/Support/html2canvas.js'
    });
}

var browser = (function () {
    var ua = navigator.userAgent, tem,
    M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
    if (/trident/i.test(M[1])) {
        tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
        return 'IE ' + (tem[1] || '');
    }
    if (M[1] === 'Chrome') {
        tem = ua.match(/\b(OPR|Edge)\/(\d+)/);
        if (tem != null) return tem.slice(1).join(' ').replace('OPR', 'Opera');
    }
    M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
    if ((tem = ua.match(/version\/(\d+)/i)) != null) M.splice(1, 1, tem[1]);
    return M.join(' ');
})();


String.prototype.splice = function (idx, rem, s) {
    return (this.slice(0, idx) + s + this.slice(idx + Math.abs(rem)));
};

