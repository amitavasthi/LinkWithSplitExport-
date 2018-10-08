function ShowNavigationMenuSubItems(sender) {
    ShowRootMenuItems(sender);

    // Get all sub menu items of the menu item.
    var subMenuItems = GetChildsByAttribute(document.getElementById(sender.id + "pnlSubMenuItems"), "class", "NavigationMenuItem _BackgroundColor7");

    var table = document.getElementById(sender.getAttribute("IdTable"));

    var tableRows = table.getElementsByTagName("tr");

    for (var i = 0; i < tableRows.length; i++) {
        var tableCells = tableRows.item(i).getElementsByTagName("td");

        for (var c = 0; c < tableCells.length; c++) {
            try {
                GetChildByAttribute(tableCells.item(c), "class", "NavigationMenuItem _BackgroundColor7").style.visibility = "hidden";
            } catch (e)
            { }
        }
    }

    var row;
    var position;
    var tableCell;

    // Run through all sub menu items of the menu item.
    for (var i = 0; i < subMenuItems.length; i++) {
        var subMenuItem = subMenuItems[i].cloneNode(true);

        row = parseInt(subMenuItem.getAttribute("Row"));
        position = parseInt(subMenuItem.getAttribute("Position"));

        tableCell = tableRows.item(row).getElementsByTagName("td").item(position);

        if (tableCell == undefined)
            continue;

        tableCell.className = "TableCellNavigationMenuItem BorderColor1 Color1";

        subMenuItem.style.display = "";
        subMenuItem.style.visibility = "";
        subMenuItem.style.opacity = "0.0";

        //table.getElementsByTagName("tr").item(row).getElementsByTagName("td").item(position).style.borderColor = "#FF0000";

        try{
            GetChildByAttribute(tableCell, "class", "NavigationMenuItem _BackgroundColor7").style.display = "none";
        } catch (e) { }

        tableCell.appendChild(subMenuItem);

        IncreaseOpacity(subMenuItem);
    }

    row = parseInt(sender.getAttribute("Row"));
    position = parseInt(sender.getAttribute("Position"));

    tableCell = tableRows.item(row).getElementsByTagName("td").item(position);

    // Check if a sub menu was loaded at startup.
    if (tableCell.childNodes.length == 1) {
        tableCell.childNodes.item(0).style.display = "none";

        sender = sender.cloneNode(true);
        sender.style.display = "";
        sender.style.visibility = "";
        sender.style.opacity = "1.0";

        tableCell.appendChild(sender);
    }

    sender.parentNode.className = "TableCellNavigationMenuItem BorderColor1 BackgroundColor5I";
    sender.style.display = "";
    sender.style.visibility = "";

    if (sender.getAttribute("_onclick") == undefined)
        sender.setAttribute("_onclick", sender.getAttribute("onclick"));

    if (sender.getAttribute("IdParent") != undefined) {
        sender.setAttribute("onclick", "ShowRootMenuItems(this);ShowNavigationMenuSubItems(document.getElementById(this.getAttribute('IdParent')));");
    } else {
        sender.setAttribute("onclick", "ShowRootMenuItems(this);");
    }

    sender.onmouseover = function () {
        var img = sender.getElementsByTagName("img").item(0);

        if(this.getAttribute("_imgsrc") == undefined)
            this.setAttribute("_imgsrc", img.src);

        img.src = "/Images/Icons/Navigation/Back.png";

        var lbl = sender.getElementsByTagName("span").item(0);
        lbl.style.display = "none";
    };

    sender.onmouseout = function () {
        if (this.getAttribute("_imgsrc") == undefined)
            return;

        var img = sender.getElementsByTagName("img").item(0);

        img.src = this.getAttribute("_imgsrc");

        var lbl = sender.getElementsByTagName("span").item(0);
        lbl.style.display = "";
    };

    sender.onmouseover();

    if (sender.getAttribute("IdContentPanel") != undefined) {
        var contentPanel = document.getElementById(sender.getAttribute("IdContentPanel"));

        if (contentPanel != undefined) {
            contentPanel = contentPanel.cloneNode(true);
            
            while (contentPanel.innerHTML.search("id=\"c") != -1) {
                contentPanel.innerHTML = contentPanel.innerHTML.replace("id=\"c", "id=\"_c");
            }

            contentPanel.style.display = "";

            tableCell.appendChild(contentPanel);

            contentPanel.style.opacity = "0.0";

            IncreaseOpacity(contentPanel);

            InitBoxes();
        }
    }
}

function ShowRootMenuItems(sender) {
    if (sender.onmouseout != undefined)
        sender.onmouseout();

    sender.onmouseover = undefined;
    sender.onmouseout = undefined;

    var table = document.getElementById(sender.getAttribute("IdTable"));

    var tableRows = table.getElementsByTagName("tr");

    for (var i = 0; i < tableRows.length; i++) {
        var tableCells = tableRows.item(i).getElementsByTagName("td");
        for (var c = 0; c < tableCells.length; c++) {
            tableCells.item(c).className = "TableCellNavigationMenuItem Color1 BorderColor1";

            //var navigationItems = GetChildsByAttribute(tableCells.item(c), "class", "NavigationMenuItem");
            var navigationItems = tableCells.item(c).childNodes;

            var showRoot = true;

            for (var n = 1; n < navigationItems.length; n++) {
                if (navigationItems.item(n).id == sender.id) {
                    showRoot = false;
                    continue;
                }

                tableCells.item(c).removeChild(navigationItems.item(n));
            }

            if (navigationItems.length > 0 && showRoot) {

                if (navigationItems.item(0).getAttribute("IdParent") != sender.id) {
                    navigationItems.item(0).style.display = "";
                    navigationItems.item(0).style.visibility = "";
                    navigationItems.item(0).style.opacity = "0.0";

                    IncreaseOpacity(navigationItems.item(0));
                }
                else {
                    tableCells.item(c).removeChild(navigationItems.item(0));
                }
            }
        }
    }

    if (sender.getAttribute("_onclick") != undefined)
        sender.setAttribute("onclick", sender.getAttribute("_onclick"));
}