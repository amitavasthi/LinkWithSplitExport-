var imageBasePath = "Includes/Boxes/";

$(document).ready(function () {
    InitBoxes();
});

function InitBoxes() {

    var elements = document.getElementsByTagName("input");

    for (var i = 0; i < elements.length; i++) {
        var element = elements.item(i);

        if (element["Image"] != undefined)
            continue;

        var type = element.getAttribute("type");

        if (type != "radio" && type != "checkbox")
            continue;

        var image = document.createElement("img");
        image["Element"] = element;

        if (element.checked == true)
            image.src = imageBasePath + "Images/" + type + "/Active.png";
        else
            image.src = imageBasePath + "Images/" + type + "/Inactive.png";

        image.onmouseover = function () {
            var type = this["Element"].getAttribute("type");

            if (this["Element"].checked == true)
                this.src = imageBasePath + "Images/" + type + "/ActiveHover.png";
            else
                this.src = imageBasePath + "Images/" + type + "/Hover.png";
        }

        image.onmouseout = function () {
            var type = this["Element"].getAttribute("type");

            if (this["Element"].checked == true)
                this.src = imageBasePath + "Images/" + type + "/Active.png";
            else
                this.src = imageBasePath + "Images/" + type + "/Inactive.png";
        }

        image.onclick = function () {
            var type = this["Element"].getAttribute("type");

            if (this["Element"].checked == false) {
                this.src = imageBasePath + "Images/" + type + "/Active.png";
                this["Element"].checked = true;
            }
            else {
                this.src = imageBasePath + "Images/" + type + "/Inactive.png";
                this["Element"].checked = false;
            }

            if (this["Element"].onchange != undefined)
                this["Element"].onchange();
            if (this["Element"].onclick != undefined)
                this["Element"].onclick();

            var boxes = document.getElementsByName(this["Element"].getAttribute('name'));

            for (var i = 0; i < boxes.length; i++) {
                boxes.item(i)["Image"]["Change"]();
            }
        }

        image["Change"] = function () {
            var type = this["Element"].getAttribute("type");

            if (this["Element"].checked == true)
                this.src = imageBasePath + "Images/" + type + "/Active.png";
            else
                this.src = imageBasePath + "Images/" + type + "/Inactive.png";
        }

        image.style.cursor = "pointer";

        element["Image"] = image;

        element.style.display = "none";
        element.parentNode.insertBefore(image, element);
    }

    InitLabels();
}

function InitLabels()
{
    var labels = document.getElementsByTagName("label");

    for (var i = 0; i < labels.length; i++) {
        var spans = labels.item(i).getElementsByTagName("span");

        for (var s = 0; s < spans.length; s++) {
            spans.item(s).setAttribute("onclick","CheckBoxes();");
        }
    }
}

function CheckBoxes() {
    var elements = document.getElementsByTagName("input");

    for (var i = 0; i < elements.length; i++) {
        var element = elements.item(i);

        var type = element.getAttribute("type");

        if (type != "radio" && type != "checkbox")
            continue;

        var image = element["Image"];

        image["Change"]();
    }
}