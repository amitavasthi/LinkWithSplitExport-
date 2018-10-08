function InitMenu(id) {
    var menus = GetChildsByAttribute(document.body, "class", "Menu");

    for (var i = 0; i < menus.length; i++) {
        menus[i].parentNode.removeChild(menus[i]);
    }

    container = document.createElement("div");
    container.className = "Menu";
    container.id = id;

    container["Render"] = function () {
        this.style.left = (tempX - 20) + "px";
        this.style.top = (tempY - 20) + "px";

        var table = document.createElement("table");
        table.setAttribute("cellspacing", "0");
        table.setAttribute("cellpadding", "0");

        // Run through all items of the menu.
        for (var i = 0; i < this.Items.length; i++) {
            var tableRow = document.createElement("tr");

            if (this.Items[i].MenuItemClick != undefined) {
                while (this.Items[i].MenuItemClick.indexOf("\n") != -1) {
                    this.Items[i].MenuItemClick = this.Items[i].MenuItemClick.replace("\n", "");
                }

                tableRow.setAttribute("onclick", this.Items[i].MenuItemClick);               
            }

            if (this.Items[i].Display != undefined) {
                tableRow.style.display = this.Items[i].Display;
            }

            var tableCellImage = document.createElement("td");
            var tableCellLabel = document.createElement("td");

            tableCellImage.setAttribute("class", "TableCellMenuItemImage");
            tableCellLabel.setAttribute("class", "TableCellMenuItemLabel");

            tableCellImage.style.width = "35px";

            this.Items[i].className = "MenuItem " + this.Items[i].className;

            if (this.Items[i].ImageUrl != undefined) {
                tableCellImage.innerHTML = "<img src=\"" + this.Items[i].ImageUrl + "\" height=\"20\" />";
            }

            tableCellLabel.appendChild(this.Items[i]);

            tableRow.appendChild(tableCellImage);
            tableRow.appendChild(tableCellLabel);

            table.appendChild(tableRow);
        }

        this.appendChild(table);

        document.body.appendChild(this);
    };

    container["Items"] = new Array();

    container.onmouseout = function (e) {
        //if (e != undefined && (e.target.className.search("MenuItem") != -1 || e.target.parentNode.className.search("MenuItem") != -1))
        if (e != undefined && e.target.className != "Menu")
            return;

        this.parentNode.removeChild(this);
    };

    container.onclick = function (e) {
        var container = this;
        window.setTimeout(function () {
            if (container != undefined && container.parentNode != undefined)
                container.parentNode.removeChild(container);
        }, 200);
    };

    return container;
}