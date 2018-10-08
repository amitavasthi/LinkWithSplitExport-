var Box = {
    Create: function (id, title) {
        var container = document.createElement("div");

        var box = document.createElement("div");
        var boxBackground = document.createElement("div");

        var title = document.createElement("div");
        var content = document.createElement("div");

        box.id = id;

        container.className = "BoxContainer";
        box.className = "Box";
        boxBackground.className = "BoxBackground";
        title.className = "BoxTitle";
        content.className = "BoxContent";

        box.appendChild(title);
        box.appendChild(content);

        container.appendChild(box);
        container.appendChild(boxBackground);

        document.body.appendChild(container);

        content["Position"] = function () {
            Box.Position(this.parentNode);
        };

        return content;
    },
    Position: function (box) {
        box.style.marginLeft = "-" + (box.offsetWidth / 2) + "px";
        box.style.marginTop = "-" + (box.offsetHeight / 2) + "px";
    }
}