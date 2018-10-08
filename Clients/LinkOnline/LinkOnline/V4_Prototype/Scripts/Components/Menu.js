var Menu = {
    Init: function() {
        var items = $(".MenuItem");

        for (var i = 0; i < items.length; i++) {
            if (items[i].getAttribute("onclick") != undefined)
                continue;

            items[i].setAttribute("onclick", "Menu.ItemClick(this);");
        }
    },
    ItemClick: function (sender) {
        var items = $(".MenuItem_Active");

        for (var i = 0; i < items.length; i++) {
            items[i].className = "MenuItem";
        }

        sender.className = "MenuItem MenuItem_Active";

        var colorScheme = "#DE0030";

        if (sender.getAttribute("ColorScheme") != undefined) {
            colorScheme = sender.getAttribute("ColorScheme");
        }

        document.getElementById("pnlMenu").style.backgroundColor = colorScheme;
        
        if (sender.getAttribute("Action") != undefined)
            eval(sender.getAttribute("Action"));
    },
    ToggleView: function () {
        var pnlMenu = document.getElementById("pnlMenu");

        if (pnlMenu.style.width != "200px") {
            pnlMenu.style.width = "200px";
            pnlMenu.style.minWidth = "200px";
            pnlMenu.style.maxWidth = "200px";
        }
        else {
            pnlMenu.style.width = "";
            pnlMenu.style.minWidth = "";
            pnlMenu.style.maxWidth = "";
        }
    },
    ClearSubMenu: function() {
        var pnlSubMenu = document.getElementById("pnlSubMenu");
        pnlSubMenu.innerHTML = "";
    },
    ShowSubMenu: function (items, showAddButton, addAction) {
        if (showAddButton == undefined)
            showAddButton = false;

        var pnlSubMenu = document.getElementById("pnlSubMenu");

        pnlSubMenu.style.width = "200px";
        pnlSubMenu.style.minWidth = "200px";
        pnlSubMenu.style.maxWidth = "200px";

        pnlSubMenu.innerHTML = "";

        var pnlSubMenuItem;
        for (var i = 0; i < items.length; i++) {
            pnlSubMenuItem = document.createElement("div");
            pnlSubMenuItem.className = "SubMenuItem";

            if (items[i].Id != undefined)
                pnlSubMenuItem.id = items[i].Id;

            if (items[i].Label != undefined)
                pnlSubMenuItem.innerHTML = items[i].Label;

            pnlSubMenuItem.Action = items[i].Click;

            pnlSubMenuItem.setAttribute("onclick", "Menu.SubMenuItemClick(this);");

            pnlSubMenu.appendChild(pnlSubMenuItem);
        }

        if (showAddButton) {
            pnlSubMenuItem = document.createElement("div");
            pnlSubMenuItem.className = "SubMenuItem";
            pnlSubMenuItem.innerHTML = "+";
            pnlSubMenuItem.style.textAlign = "center";
            pnlSubMenuItem.setAttribute("onclick", addAction);

            pnlSubMenu.appendChild(pnlSubMenuItem);
        }
    },
    HideSubMenu: function () {
        var pnlSubMenu = document.getElementById("pnlSubMenu");
        pnlSubMenu.innerHTML = "";
        pnlSubMenu.style.width = "0px";
        pnlSubMenu.style.minWidth = "0px";
        pnlSubMenu.style.maxWidth = "0px";
    },
    SubMenuItemClick: function (sender) {
        var items = $(".SubMenuItem_Active");

        for (var i = 0; i < items.length; i++) {
            items[i].className = "SubMenuItem";
        }

        sender.className = "SubMenuItem SubMenuItem_Active";

        if (sender.Action != undefined) {
            eval(sender.Action);
        }
    }
};