var activeHoverBoxes = new Object();

activeHoverBoxes["Keys"] = new Array();

activeHoverBoxes["Add"] = function (control) {
    activeHoverBoxes.Keys.push(control.id);

    activeHoverBoxes[control.id] = control;
}

activeHoverBoxes["Delete"] = function (key) {
    var keyIndex = -1;

    for (var i = 0; i < activeHoverBoxes["Keys"].length; i++) {
        if (activeHoverBoxes["Keys"][i] == key) {
            keyIndex = i;
            break;
        }
    }

    if (keyIndex == -1)
        return;

    activeHoverBoxes["Keys"].splice(keyIndex, 1);

    activeHoverBoxes[key] = undefined;
}

function ShowHoverBox(idHoverBox, idTrigger, mode, triggerMode, activateTriggerImage, level, idParent, animation) {
    if (activateTriggerImage == undefined)
        activateTriggerImage = false;

    if (level == undefined)
        level = 0;

    if (animation == undefined)
        animation = "Opacity";

    var control = document.getElementById(idHoverBox);
    var trigger = document.getElementById(idTrigger);

    for (var i = 0; i < activeHoverBoxes.Keys.length; i++) {
        var key = activeHoverBoxes.Keys[i];

        if (key == idHoverBox)
            continue;

        if (activeHoverBoxes[key] == undefined)
            continue;

        if (activeHoverBoxes[key].Level == level) {
            if (mouseMoveFunctions["HideHoverBox" + activeHoverBoxes[key].id] != undefined)
                mouseMoveFunctions["HideHoverBox" + activeHoverBoxes[key].id](undefined, true);
        }
    }

    if (activeHoverBoxes[idHoverBox] != undefined)
        return;

    if (activateTriggerImage && trigger.tagName.toLowerCase() == "img") {
        if (trigger.src.search("_Active") == -1)
            trigger.src = trigger.src.replace(".png", "_Active.png");
    }

    // Check if the hover box exists.
    if (control == undefined)
        return;

    if (control.style.display == "block")
        return;

    control["ChildBoxes"] = new Array();

    if (idParent != undefined && activeHoverBoxes[idParent] == undefined)
        return;

    if (idParent != undefined && activeHoverBoxes[idParent] != undefined) {
        activeHoverBoxes[idParent]["Locked"] = "True";
        activeHoverBoxes[idParent].ChildBoxes.push(control);
        control["IdParent"] = idParent;
    }

    if (animation == "Opacity")
        control.style.opacity = "0.0";
    else
        control.style.visibility = "hidden";

    // Show the control.
    control.style.display = "block";

    control["Mode"] = mode;

    // Check which show mode is set.
    if (mode == "RightBottom") {
        //control.style.marginLeft = "-543px";
        //control.style.marginTop = "-5px";
        control.style.marginLeft = "-" + (control.offsetWidth - trigger.offsetWidth) + "px";
    }
    else if (mode == "LeftTop") {
        control.style.marginTop = "-" + (control.offsetHeight + trigger.offsetHeight) + "px";
    }
    else if (mode == "TopLeft") {
        //control.style.marginTop = trigger.offsetTop + "px";
        control.style.marginLeft = trigger.offsetLeft + "px";
    }
    else if (mode == "LeftMiddle") {
        //control.style.marginTop = (((control.offsetHeight / 2) - trigger.offsetTop) * -1) + "px";
        control.style.marginTop = trigger.offsetTop + "px";
        control.style.marginLeft = trigger.parentNode.offsetWidth + "px";
    }
    else if (mode == "RightMiddle") {
        control.style.marginLeft = "-" + (control.offsetWidth + control.offsetLeft) + "px";
        control.style.marginTop = "-" + (control.offsetHeight / 2) + "px";
    }
    else if (mode == "SubNavigation") {
        control.style.marginLeft = "-" + (control.offsetWidth + control.offsetLeft) + "px";
        //control.style.marginTop = "-" + ((control.offsetHeight / 2) + 11) + "px";
        control.style.marginTop = "-32px";
    }
    else if (mode == "RightTop") {
        control.style.marginLeft = "-" + (control.offsetWidth + control.offsetLeft) + "px";
        //control.style.marginTop = "-" + parseInt(trigger.style.marginTop) + "px";
    }
    else if (mode == "VariableSelectionCategories") {
        var left = parseInt(trigger.parentNode.style.marginLeft);

        if (isNaN(left))
            left = trigger.parentNode.offsetLeft;

        var marginTop = trigger.offsetTop - trigger.parentNode.scrollTop;

        if (trigger.parentNode.getAttribute("MoveTop") != undefined) {
            marginTop = parseInt(trigger.parentNode.getAttribute("MoveTop")) + marginTop;
        }

        //control.style.marginTop = marginTop + "px";
        control.style.marginLeft = (left - 199) + "px";

        // Check if the hover box is out of screen.
        if (control.offsetLeft < 0) {
            //control.style.marginLeft = (trigger.offsetWidth - 35) + "px"
            control.style.marginLeft = (trigger.offsetWidth + left + 17) + "px"
        }
    }
    else if (mode == "Workflow") {
        control.style.marginTop = "-" + (control.offsetHeight + trigger.offsetHeight - 1) + "px";

        control.style.width = (trigger.offsetWidth + 2.5) + "px";
    }
    else if (mode == "LeftPanel") {
        control.style.left = "0px";
        //control.style.marginTop = (trigger.offsetHeight / 2) + "px";
        control.style.top = ((window.innerHeight / 2) - (control.offsetHeight / 2)) + "px";
    }
    else if (mode == "RightPanel") {
        control.style.right = "0px";
        //control.style.marginTop = (trigger.offsetHeight / 2) + "px";
        control.style.top = ((window.innerHeight / 2) - (control.offsetHeight / 2)) + "px";
    }
    else if (mode == "LeftPanelSection") {
        control.style.marginTop = trigger.offsetTop + "px";
    }

    control.setAttribute("Mode", mode);

    control["Hide"] = function (e, immediate) {
        HideHoverBox(idHoverBox, idTrigger, mode, triggerMode, activateTriggerImage, immediate, animation);
    };

    if (parseInt(level) == 0) {
        window.setTimeout(function () {
            // Set the hide method.
            mouseMoveFunctions.Add("HideHoverBox" + idHoverBox, function (e, immediate) {
                HideHoverBox(idHoverBox, idTrigger, mode, triggerMode, activateTriggerImage, immediate, animation);
            });
        }, 1000);
    }
    else {

        // Set the hide method.
        mouseMoveFunctions.Add("HideHoverBox" + idHoverBox, function (e, immediate) {
            HideHoverBox(idHoverBox, idTrigger, mode, triggerMode, activateTriggerImage, immediate, animation);
        });
    }

    if (animation == "Opacity") {
        // Blur in the control.
        IncreaseOpacity(control);
    }
    else if (animation == "Slide") {
        var width = control.offsetWidth;

        control.style.overflow = "hidden";
        control.style.width = "0px";

        control.style.visibility = "";

        IncreaseWidth(control, width);
    }
    else if (animation == "SlideH") {
        var height = control.offsetHeight;

        control.style.overflow = "hidden";
        control.style.height = "0px";

        control.style.visibility = "";

        IncreaseHeight(control, height, function () {
            control.style.height = "";
        });
    }
    else if (animation == "SlideWithTrigger") {
        var width = control.offsetWidth;

        control.style.overflow = "hidden";
        control.style.width = "0px";

        control.style.visibility = "";

        IncreaseRight(trigger, width);
        IncreaseWidth(control, width);
    }

    control["Trigger"] = trigger;
    control["Level"] = level;

    activeHoverBoxes.Add(control);
}

function HideHoverBox(idHoverBox, idTrigger, mode, triggerMode, activateTriggerImage, force, animation) {
    if (force == undefined)
        force = false;

    var control = document.getElementById(idHoverBox);

    if (control == undefined)
        return;

    if (control["Locked"] == "True")
        return;

    var trigger = document.getElementById(idTrigger);

    var marginX = 0;
    var marginY = 0;

    if (trigger != undefined) {
        marginX = trigger.offsetWidth;
        marginY = trigger.offsetHeight;

        if (mode == "Workflow") {
            marginX = 10;
            marginY + trigger.offsetHeight;
        }
        else if (mode == "LeftPanel") {
            marginX = trigger.offsetWidth;
            marginY = trigger.offsetHeight;
        }
        else if (mode == "LeftPanelSection") {
            //marginX = control.offsetWidth;
            //marginY = control.offsetHeight;
        }
        else if (mode == "RightMiddle") {
            //marginX += control.offsetWidth;
            //marginY += control.offsetHeight;
        }
    }

    if (force == false) {
        var offsetLeft = control.offsetLeft;
        var offsetTop = control.offsetTop;

        if (control.offsetParent.tagName != "body") {
            offsetLeft += control.offsetParent.offsetLeft;
            offsetTop += control.offsetParent.offsetTop;
        }

        if (tempX > (offsetLeft - marginX) && tempX < (offsetLeft + control.offsetWidth + marginX)) {
            if (tempY > (offsetTop - marginY) && tempY < (offsetTop + control.offsetHeight + marginY)) {
                return;
            }
        }

        mouseMoveFunctions.Delete("HideHoverBox" + idHoverBox);

        activeHoverBoxes.Delete(control.id);

        if (control["IdParent"] != undefined) {
            activeHoverBoxes[control["IdParent"]]["Locked"] = undefined;
        }

        //DecreaseOpacity(control, function () {
        if (animation == "Opacity") {
            control.style.opacity = "0.0";
            control.style.display = "none";
        }
        else if (animation == "Slide") {
            DecreaseWidth(control, 0, false, function () {
                control.style.display = "none";
                control.style.width = "";
                control.style.marginLeft = "";
                control.style.marginTop = "";
            });
        }
        else if (animation == "SlideH") {
            DecreaseHeight(control, 0, function () {
                control.style.display = "none";
                control.style.height = "";
                control.style.marginLeft = "";
                control.style.marginTop = "";
            }, false);
        }
        else if (animation == "SlideWithTrigger") {
            DecreaseRight(trigger, 0);

            DecreaseWidth(control, 0, false, function () {
                control.style.display = "none";
                control.style.width = "";
                control.style.marginLeft = "";
                control.style.marginTop = "";
            });
        }

        if (trigger != undefined) {
            if (trigger.getAttribute(triggerMode) == undefined || trigger.getAttribute(triggerMode) == "" || trigger.getAttribute(triggerMode).substring(0, 12) == "ShowHoverBox")
                trigger.setAttribute(triggerMode, "ShowHoverBox('" + idHoverBox + "','" + idTrigger + "','" + mode + "', '" + triggerMode + "', " + activateTriggerImage + ", " + control["Level"] + ", " + (control["IdParent"] != undefined ? "'" + control["IdParent"] + "'" : "undefined") + ", '"+ animation +"');");

            if (activateTriggerImage && trigger.tagName.toLowerCase() == "img") {
                trigger.src = trigger.src.replace("_Active.png", ".png");
            }
        }

        for (var i = 0; i < control["ChildBoxes"].length; i++) {
            HideHoverBox(control["ChildBoxes"][i].id, control["ChildBoxes"][i]["Trigger"].id, control["ChildBoxes"][i]["Mode"]);
        }
        //});
    }
    else {
        mouseMoveFunctions.Delete("HideHoverBox" + idHoverBox);

        activeHoverBoxes.Delete(control.id);

        if (control["IdParent"] != undefined && activeHoverBoxes[control["IdParent"]] != undefined) {
            activeHoverBoxes[control["IdParent"]]["Locked"] = undefined;
        }

        control.style.opacity = "0.0";
        control.style.display = "none";

        control.style.marginLeft = "";
        control.style.marginTop = "";

        if (trigger != undefined) {
            trigger.setAttribute(triggerMode, "ShowHoverBox('" + idHoverBox + "','" + idTrigger + "','" + mode + "', '" + triggerMode + "', " + activateTriggerImage + ", " + control["Level"] + ", " + (control["IdParent"] != undefined ? "'" + control["IdParent"] + "'" : "undefined") + ");");

            if (activateTriggerImage && trigger.tagName.toLowerCase() == "img") {
                trigger.src = trigger.src.replace("_Active.png", ".png");
            }
        }
    }
}