

var DashboardTabs = {
    Init: function () {
        var elements = $("DashboardTab");

        var container = document.createElement("div");
        container.className = "DashboardTabSelection";

        $("DashboardTabSelection")[0].appendChild(container);

        var name;
        var pnlTab;
        var id;
        for (var i = 0; i < elements.length; i++) {
            name = elements[i].getAttribute("name");

            pnlTab = document.createElement("div");
            pnlTab.className = "DashboardTabSelector";
            pnlTab.innerHTML = name;
            pnlTab.Name = name;
            pnlTab.setAttribute("onclick", "DashboardTabs.Select(this);");

            container.appendChild(pnlTab);

            if (elements[i].getAttribute("Selected") == "True") {
                elements[i].style.display = "block";
                pnlTab.className = "DashboardTabSelector DashboardTabSelector_Active";
            }

            id = "DashboardTab" + name;

            while (id.indexOf(" ") != -1) {
                id = id.replace(" ", "_");
            }
            while (id.indexOf("-") != -1) {
                id = id.replace("-", "_");
            }
            while (id.indexOf(".") != -1) {
                id = id.replace(".", "_");
            }
            while (id.indexOf(",") != -1) {
                id = id.replace(",", "_");
            }

            elements[i].id = id;
            pnlTab.IdContainer = id;
        }
    },
    Select: function (sender) {
        var elements = $(".DashboardTabSelector_Active");

        for (var i = 0; i < elements.length; i++) {
            elements[i].className = "DashboardTabSelector";
        }

        sender.className = "DashboardTabSelector DashboardTabSelector_Active";

        elements = $("DashboardTab");

        for (var i = 0; i < elements.length; i++) {
            elements[i].style.display = "";
        }

        document.getElementById(sender.IdContainer).style.display = "block";
    }
}
try {
    $(document).ready(DashboardTabs.Init);
}
catch (e) { }