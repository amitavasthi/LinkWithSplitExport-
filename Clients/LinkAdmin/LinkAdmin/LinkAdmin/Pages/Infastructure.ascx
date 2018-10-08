<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Infastructure.ascx.cs" Inherits="LinkAdmin.Pages.Infastructure" %>
<style type="text/css">
    .Server {
        cursor:pointer;
        display:inline-block;
        padding:10px;
        margin:10px;
        
        width:200px;

        background:#2189d7;
        color:#FFFFFF;

        box-shadow: 0px 0px 2px 0px #444444;
        border-radius: 2px;
    }

    .Server:hover {
        box-shadow: 0px 0px 10px 0px #2189d7;
    }
</style>
<script type="text/javascript">
    var container = document.getElementById("pnlOverview");
    container.style.height = (document.getElementById("ContentPanel").offsetHeight - 40) + "px";

    var servers = JSON.parse(Request(
        "/Handlers/GlobalHandler.ashx",
        "GetServers",
        ""
    ));

    var container = document.getElementById("pnlServers");
    var pnlSwitch = document.getElementById("pnlSwitch");

    for (var i = 0; i < servers.length; i++) {
        var server = document.createElement("div");
        server.className = "Server";
        server.id = "pnlServer" + servers[i].IP;
        server.setAttribute("onclick", "document.getElementById('navItemServers').onclick();");

        server.innerHTML = servers[i].Description;

        container.appendChild(server);
    }

    for (var i = 0; i < servers.length; i++) {
        var server = document.getElementById("pnlServer" + servers[i].IP);

        var line = document.createElement("canvas");
        line.style.position = "absolute";

        container.appendChild(line);

        var context = line.getContext("2d");

        var start = {
            x: GetOffsetLeft(pnlSwitch) + (pnlSwitch.offsetWidth / 2),
            y: GetOffsetTop(pnlSwitch) + pnlSwitch.offsetHeight
        };
        var end = {
            x: GetOffsetLeft(server) + (server.offsetWidth / 2),
            y: GetOffsetTop(server)
        };

        var left;
        var top;

        if (start.x < end.x)
            left = start.x;
        else {
            left = end.x;
        }

        if (start.y < end.y)
            top = start.y;
        else
            top = end.y;

        left -= 5;
        top -= 5;

        line.style.left = left + "px";
        line.style.top = top + "px";
        line.width = Math.abs(end.x - start.x) + 10;
        line.height = Math.abs(end.y - start.y) + 10;

        context.beginPath();
        context.moveTo(
            start.x - left,
            start.y - top
        );
        context.lineTo(
            end.x - left,
            end.y - top
        );

        if (servers[i].Role == "Production") {
            context.setLineDash([5]);
        }

        if (servers[i].State == "Online") {
            context.strokeStyle = "#00AA00";
        }
        else if (servers[i].State == "Offline") {
            context.strokeStyle = "#FF0000";
        }

        context.lineWidth = 2;

        context.stroke();
    }

    DrawUserLine();

    function DrawUserLine() {
        var pnlUser = document.getElementById("pnlUser");
        var pnlSwitch = document.getElementById("pnlSwitch");

        var line = document.createElement("canvas");
        line.style.position = "absolute";

        container.appendChild(line);

        var context = line.getContext("2d");

        var start = {
            x: GetOffsetLeft(pnlUser) + (pnlUser.offsetWidth / 2),
            y: GetOffsetTop(pnlUser) + pnlUser.offsetHeight
        };
        var end = {
            x: GetOffsetLeft(pnlSwitch) + (pnlSwitch.offsetWidth / 2),
            y: GetOffsetTop(pnlSwitch)
        };

        var left;
        var top;

        if (start.x < end.x)
            left = start.x;
        else {
            left = end.x;
        }

        if (start.y < end.y)
            top = start.y;
        else
            top = end.y;

        left -= 5;
        top -= 5;

        line.style.left = left + "px";
        line.style.top = top + "px";
        line.width = Math.abs(end.x - start.x) + 10;
        line.height = Math.abs(end.y - start.y) + 10;

        context.beginPath();
        context.moveTo(
            start.x - left,
            start.y - top
        );
        context.lineTo(
            end.x - left,
            end.y - top
        );

        context.strokeStyle = "#444444";
        context.lineWidth = 2;

        context.stroke();
    }
</script>
<div id="pnlOverview" style="text-align:center">
    <div id="pnlUser" style="margin-bottom:100px;"><img height="70" src="/Images/Overview/User.png" /></div>
    <div id="pnlSwitch" onclick="document.getElementById('navItemServers').onclick();" class="Server">Switch</div>
    <div id="pnlServers" style="margin-top:200px;"></div>
</div>