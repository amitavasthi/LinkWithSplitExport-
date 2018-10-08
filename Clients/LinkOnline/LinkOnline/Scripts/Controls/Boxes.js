var imageBasePath = "/Images/Icons/Boxes";

loadFunctions.push(InitBoxes);

function InitBoxes() {
    var elements = document.getElementsByTagName("input");

    for (var i = 0; i < elements.length; i++) {
        var element = elements.item(i);

        var type = element.getAttribute("type");

        if (type != "radio" && type != "checkbox")
            continue;

        if (element["Image"] != undefined)
            continue;

        var image = document.createElement("img");
        image["Element"] = element;
        image.className = "Custom" + type;

        if (element.checked == true) {
            image.src = imageBasePath + "/" + type + "/Active.png";
            image.className += "";
        }
        else {
            image.src = imageBasePath + "/" + type + "/Inactive.png";
            image.className += "";
        }

        image.onmouseover = function () {
            var type = this["Element"].getAttribute("type");

            if (this["Element"].checked == true) {
                this.src = imageBasePath + "/" + type + "/ActiveHover.png";
                this.className += "";
            }
            else {
                this.src = imageBasePath + "/" + type + "/Hover.png";
                this.className += "";
            }
        }

        image.onmouseout = function () {
            var type = this["Element"].getAttribute("type");

            if (this["Element"].checked == true) {
                this.src = imageBasePath + "/" + type + "/Active.png";
                this.className += "";
            }
            else {
                this.src = imageBasePath + "/" + type + "/Inactive.png";
                this.className += "";
            }
        }

        image.onclick = function () {
            var type = this["Element"].getAttribute("type");

            if (this["Element"].checked == false) {
                this.src = imageBasePath + "/" + type + "/Active.png";
                this.className += "";
                this["Element"].checked = true;
            }
            else {
                this.src = imageBasePath + "/" + type + "/Inactive.png";
                this.className += "";
                this["Element"].checked = false;
            }

            var boxes = document.getElementsByName(this["Element"].getAttribute('name'));

            for (var i = 0; i < boxes.length; i++) {
                boxes.item(i)["Image"]["Change"]();
            }

            if (this["Element"].onclick != undefined)
                this["Element"].onclick();

            if (this["Element"].onchange != undefined)
                this["Element"].onchange();
        }

        image["Change"] = function () {
            var type = this["Element"].getAttribute("type");

            if (this["Element"].checked == true) {
                this.src = imageBasePath + "/" + type + "/Active.png";
                this.className += "";
            }
            else {
                this.src = imageBasePath + "/" + type + "/Inactive.png";
                this.className += "";
            }
        }

        image.style.cursor = "pointer";

        element["Image"] = image;

        if (element.parentNode.getElementsByTagName("img").length > 0 && element.style.display == "none") {
            element.parentNode.removeChild(element.parentNode.getElementsByTagName("img").item(0));
        }

        element.style.display = "none";

        element.parentNode.insertBefore(image, element);
    }
}