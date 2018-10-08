<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LinkAdmin.Default" %>



<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>LiNK Admin</title>
<script src="/Scripts/Pages/ServerBenchmark/jquery.min.js"></script>
<script src="/Scripts/Pages/ServerBenchmark/d3.min.js" type="text/javascript"></script>
<script src="/Scripts/Pages/ServerBenchmark/dimple.v2.1.6.min.js"></script>

    <wu:ScriptReference runat="server" Source="/Scripts/Controls/Button2.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Components/Ajax.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Core/Global.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/JQuery/jquery-1.8.1.min.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Components/LanguageManager.js"></wu:ScriptReference>

    <wu:ScriptReference runat="server" Source="/Scripts/Pages/Clients.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Clients.css"></wu:StylesheetReference>

    <wu:ScriptReference runat="server" Source="/Scripts/Pages/Instances.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Instances.css"></wu:StylesheetReference>
    
    <wu:ScriptReference runat="server" Source="/Scripts/Pages/Servers.js"></wu:ScriptReference>
    <!--<wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Servers.css"></wu:StylesheetReference>-->
    
    <wu:ScriptReference runat="server" Source="/Scripts/Pages/Default.js"></wu:ScriptReference>
    <!--<wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Default.css"></wu:StylesheetReference>-->

    <wu:StylesheetReference runat="server" Source="/Stylesheets/Components/Loading.css"></wu:StylesheetReference>
    
    <wu:ScriptReference runat="server" Source="/Scripts/Controls/Box.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Controls/Box.css"></wu:StylesheetReference>

    <wu:StylesheetReference runat="server" Source="/Stylesheets/Core/Global.css"></wu:StylesheetReference>
</head>
<body onload="Body_Load();">
    <script type="text/javascript">
        loadFunctions.push(function () {
            Navigate(undefined, '~/Pages/Default.ascx');
        });
    </script>
    <script type="text/javascript">
        function ShowConsoleTab(sender, id) {
            var elements = $(".LogEntries");

            for (var i = 0; i < elements.length; i++) {
                elements[i].style.display = "none";
            }

            document.getElementById("pnlConsole" + id).style.display = "";

            elements = $(".ConsoleTab_Active");

            for (var i = 0; i < elements.length; i++) {
                elements[i].className = "ConsoleTab";
            }

            sender.className = "ConsoleTab ConsoleTab_Active";
        }
    </script>
    <form id="form1" runat="server">
        <table cellspacing="0" cellpadding="0" style="height:100%;width:100%;">
            <tr>
                <td colspan="2" class="Headline">
                    <img id="imgLogo" onclick="window.location = '/Default.aspx'" src="/Images/Link.png" style="cursor:pointer;">
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="Navigation">
                    <div id="navItemDefault" class="NavigationItem" onclick="Navigate(this, '~/Pages/Default.ascx');" style="background:#FFFFFF">
                        <wu:Label ID="lblNavigationDefault" runat="server" Name="NavigationDefault"></wu:Label>
                    </div>
                    <div id="navItemOverview" class="NavigationItem" onclick="Navigate(this, '~/Pages/Overview.ascx');">
                        <wu:Label ID="lblNavigationOverview" runat="server" Name="NavigationOverview"></wu:Label>
                    </div>
                    <div id="navItemConsole" class="NavigationItem" onclick="Navigate(this, '~/Pages/Console.ascx');">
                        <wu:Label ID="lblNavigationConsole" runat="server" Name="NavigationConsole"></wu:Label>
                    </div>
                    <div id="navItemClients" class="NavigationItem" onclick="Navigate(this, '~/Pages/Clients.ascx');">
                        <wu:Label ID="lblNavigationClients" runat="server" Name="NavigationClients"></wu:Label>
                    </div>
                    <div id="navItemPortals" class="NavigationItem" onclick="Navigate(this, '~/Pages/Instances.ascx');">
                        <wu:Label ID="lblNavigationInstances" runat="server" Name="NavigationInstances"></wu:Label>
                    </div>
                    <div id="navItemServers" class="NavigationItem" onclick="Navigate(this, '~/Pages/Servers.ascx');">
                        <wu:Label ID="lblNavigationServers" runat="server" Name="NavigationServers"></wu:Label>
                    </div>
                    <div id="navItemServerBenchmark" class="NavigationItem" onclick="Navigate(this, '~/Pages/ServerBenchmark.ascx');">
                        <wu:Label ID="lblNavigationServerBenchmark" runat="server" Name="NavigationServerBenchmark"></wu:Label>
                    </div>
                </td>
                <td class="Content">
                    <div id="ContentPanel" class="ContentPanel">
                    </div>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
