<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="SavedDefinitions.aspx.cs" Inherits="LinkOnline.Pages.LinkBi.SavedDefinitions" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/LinkBi.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Pages/LinkBi.js"></wu:ScriptReference>

    <style type="text/css">
        .LinkBiDefinitionMessage {
            margin:1em;
            padding:0.5em;

            padding-left:30px;
            cursor:pointer;
        }
        .LinkBiDefinitionMessage:hover {
            box-shadow:0px 0px 5px 0px #444444;
        }

        .LinkBiDefinitionMessageError {
            background: url('/Images/Icons/Error.png') no-repeat center left #FF0000;
            background-size:20px;
            background-position-x:5px;
            color:#FFFFFF;

            box-shadow:0px 0px 2px 0px #FF0000;
        }
    </style>
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="LinkBiTitle"></wu:Label>
    <span class="Beta">
        <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>

    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <script type="text/javascript">
        loadFunctions.push(function () {
            var pnlDefinitions = document.getElementById("cphContent_pnlDefinitions");

            pnlDefinitions.style.height = (ContentHeight - 100) + "px";
        });
    </script>
    <table style="height:100%;margin-top:-2px;">
        <tr valign="top">
            <td style="border-right-width:1px;border-right-style:solid;padding-top:0.5em;padding-left:0px;min-width:250px;" class="BorderColor1 BackgroundColor7">
                <asp:Panel ID="pnlDefinitions" runat="server" style="overflow:hidden;overflow-y:auto;"></asp:Panel>
                <img onclick="window.location = 'LinkBi.aspx'" style="cursor:pointer" src="/Images/Icons/Cloud/NewDirectory.png" onmouseover="this.src='/Images/Icons/Cloud/NewDirectory_Hover.png';" onmouseout="this.src='/Images/Icons/Cloud/NewDirectory.png';" />
            </td>
            <td style="width:100%;">
                <div id="pnlDefinitionProperties" style="margin:1em;"></div>
                <asp:Panel ID="pnlDefinitionMessages" runat="server"></asp:Panel>
            </td>
        </tr>
    </table>
</asp:content>
