function ShowToolTip(owner, text, isFixed, direction) {
    if (text == "")
        return;

    if (isFixed == undefined)
        isFixed = false;

    if (direction == undefined)
        direction = "Left";

    var control = document.getElementById("ToolTip");

    if (control != undefined) {
        control.parentNode.removeChild(control);
    }
    control = document.createElement("div");

    control.id = "ToolTip";
    control.className = "ToolTip ToolTip" + direction + " Color1 BorderColor1";

    control.innerHTML = "<div class=\"ToolTipText\">" + text + "</div>";

    if (isFixed == false)
        document.body.appendChild(control);
    else
        owner.parentNode.appendChild(control);

    if (isFixed == false) {
        var left;
        var top;

        if (direction == "Top") {
            left = tempX - (control.offsetWidth / 2);
            top = tempY + 20;
        }
        else if (direction == "Bottom") {
            left = tempX - (control.offsetWidth / 2);
            top = tempY - 5 - (control.offsetHeight);
        }
        else {
            left = tempX + 10;
            top = tempY - (control.offsetHeight / 2);
        }

        if ((left + control.offsetWidth) > document.body.offsetWidth)
            left = document.body.offsetWidth - control.offsetWidth;

        if ((top + control.offsetHeight) > document.body.offsetHeight)
            top = document.body.offsetHeight - control.offsetHeight;

        control.style.left = left + "px";
        control.style.top = top + "px";

        owner.onmousemove = function () {
            var left;
            var top;

            if (direction == "Top") {
                left = tempX - (control.offsetWidth / 2);
                top = tempY + 20;
            }
            else if (direction == "Bottom") {
                left = tempX - (control.offsetWidth / 2);
                top = tempY - 5 - (control.offsetHeight);
            }
            else {
                left = tempX + 10;
                top = tempY - (control.offsetHeight / 2);
            }

            if ((left + control.offsetWidth) > document.body.offsetWidth)
                left = document.body.offsetWidth - control.offsetWidth;

            if ((top + control.offsetHeight) > document.body.offsetHeight)
                top = document.body.offsetHeight - control.offsetHeight;

            control.style.left = left + "px";
            control.style.top = top + "px";
        }
    }
    else {
        control.style.width = owner.offsetWidth + "px";
    }

    if (isFixed) {
        var onmouseout = "";

        if (owner.parentNode.getAttribute("onmouseout") != undefined)
            onmouseout = owner.parentNode.getAttribute("onmouseout");

        onmouseout = onmouseout.replace("var control = document.getElementById('ToolTip');control.parentNode.removeChild(control);", "");

        owner.parentNode.setAttribute("onmouseout", onmouseout + "var control = document.getElementById('ToolTip');control.parentNode.removeChild(control);");
    }
    else {
        var onmouseout = "";

        if (owner.getAttribute("onmouseout") != undefined)
            onmouseout = owner.getAttribute("onmouseout");

        onmouseout = onmouseout.replace("var control = document.getElementById('ToolTip');control.parentNode.removeChild(control);", "");

        owner.setAttribute("onmouseout", onmouseout + "var control = document.getElementById('ToolTip');control.parentNode.removeChild(control);");
    }
}

function ShowFiltersToolTip(owner, text, isFixed, direction) {

    var text = document.getElementById("cphContent_pnlFilterCategories");
    var count = (text.innerHTML.match(/<span/g) || []).length;

    if (count > 0) {
        var toolTipText = "<div id='filterTop'>Filters </br>" + text.innerHTML.replace(/<img[^>]*>/gi, "</br></br>") + "</div>";
    } else {

        var toolTipText = "<div id='filterTop'>Filters </br></div>";
    }


    ShowToolTip(this, toolTipText, false, undefined)
}

function HideFiltersToolTip() {
    document.getElementById("filterTop").parentElement.remove();
}