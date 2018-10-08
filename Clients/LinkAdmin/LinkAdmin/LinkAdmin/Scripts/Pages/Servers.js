function BringOnline(sender, IP) {

    var html = LanguageManager.GetLabel("ServerBringOnline");

    var box = CreateBox("boxServerBringOnline", html);
    box.style.cursor = "pointer";
    box.style.background = "#00AA00";
    box.style.color = "#FFFFFF";
    box.style.padding = "40px";
    box.style.textAlign = "center";

    box.onclick = function () {
        box.style.width = (box.offsetWidth - 80) + "px";

        box.innerHTML = "<div style=\"display:inline-block;\"><div class=\"spinner\" style=\"width:20px;height:20px;\">" +
            "<div class=\"double-bounce1\"></div>" +
            "<div class=\"double-bounce2\"></div>" +
          "</div></div>";

        Request(
            "/Handlers/GlobalHandler.ashx",
            "ServerBringOnline",
            "IP=" + IP
        );

        window.setTimeout(function () {
            Navigate(document.getElementById("navItemServers"), "~/Pages/Servers.ascx");
        }, 100);

        window.setTimeout(function () {
            sender.setAttribute("onclick", "TakeOffline(this, '" + IP + "');");
            sender.innerHTML = LanguageManager.GetLabel("ServerStateOnline");
            box.Close();
        }, 1000);
    };
}

function TakeOffline(sender, IP) {

    var html = LanguageManager.GetLabel("ServerTakeOffline");

    var box = CreateBox("boxServerTakeOffline", html);
    box.style.cursor = "pointer";
    box.style.background = "#FF0000";
    box.style.color = "#FFFFFF";
    box.style.padding = "40px";
    box.style.textAlign = "center";

    box.onclick = function () {
        box.style.width = (box.offsetWidth - 80) + "px";

        box.innerHTML = "<div style=\"display:inline-block;\"><div class=\"spinner\" style=\"width:20px;height:20px;\">" +
            "<div class=\"double-bounce1\"></div>" +
            "<div class=\"double-bounce2\"></div>" +
          "</div></div>";

        Request(
            "/Handlers/GlobalHandler.ashx",
            "ServerTakeOffline",
            "IP=" + IP
        );

        window.setTimeout(function () {
            sender.setAttribute("onclick", "BringOnline(this, '" + IP + "');");
            sender.innerHTML = LanguageManager.GetLabel("ServerStateOffline");
            box.Close();
        }, 1000);
    };
}

function GetLatestSynchAction(server) {
    var result = Request(
        "/Handlers/GlobalHandler.ashx",
        "GetLatestSynchAction",
        "Server=" + server
    );

    CreateBox("boxUsers", result);
}