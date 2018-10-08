Pages.Dashboards = {
    Load: function () {
        Menu.ClearSubMenu();
        
        var items = [];

        items.push({ Label: "west palm beach", Click: "Pages.Dashboards.LoadDashboard('/Handlers/Dashboards.ashx?Dashboard=WestPalmBeach')" });
        items.push({ Label: "weekly meter share", Click: "Pages.Dashboards.LoadDashboard('/Handlers/Dashboards.ashx?Dashboard=WeeklyMeterShare')" });
        items.push({ Label: "msadtest", Click: "Pages.Dashboards.LoadDashboard('https://charts.linkmr.com/msadtest2/index.html')" });

        Menu.ShowSubMenu(items);
    },
    LoadDashboard: function (url) {
        document.content.innerHTML = "<iframe style=\"margin:40px;width:" + (document.content.offsetWidth - 80) + "px;height:" + (document.content.offsetHeight - 85) + "px;\" frameborder=\"0\" src=\""+ url +"\"></iframe>";
    }
};