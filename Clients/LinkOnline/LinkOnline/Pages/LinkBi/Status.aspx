<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="LinkOnline.Pages.LinkBi.Status" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <style type="text/css">
        .MailMeButton {
            cursor: pointer;
            padding: 0.5em;
            color: #FFFFFF;
            box-shadow: 0px 0px 5px 0px #444444;
        }
    </style>
</asp:content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="LinkBiTitle"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:Content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <script type="text/javascript">
        function ChangeMailMeSelection(button) {
            if (button.getAttribute("selected") == "false") {
                ConvertColor([0, 200, 0], button, 'Increase');
                button.setAttribute("selected", "true");
                document.getElementById("MailMe").value = "true";
            }
            else {
                ConvertColor([108, 174, 224], button, 'Increase');
                button.setAttribute("selected", "false");
                document.getElementById("MailMe").value = "false";
            }
        }

        function Download() {
            window.open(window.location + "?Method=Download", "_blank");

            window.location = "LinkBi.aspx";
        }
    </script>
    <div class="BackButton" onclick="window.location='LinkBi.aspx'" style="margin:2em;"></div>
    <asp:Panel ID="pnlProgress" runat="server" Visible="False" style="margin:auto;width:400px;margin-top:20%;text-align:center;">
        <h2 class="Color1"><wu:Label ID="lblProgress" runat="server" Name="LinkBiProgressLabel"></wu:Label></h2>
        <br />
        <wu:ProgressBar ID="progressBar" runat="server" Service="/Handlers/LinkBi.ashx?Method=GetProgress"></wu:ProgressBar>
        <br /><br />
        <!--<div style="background-color:rgb(108,174,224);" class="MailMeButton" selected="false" onclick="ChangeMailMeSelection(this)"></div>-->
        <input type="hidden" id="MailMe" name="MailMe" value="false" />
    </asp:Panel>
    <asp:Panel ID="pnlDownload" runat="server" Visible="False" style="margin:auto;width:400px;margin-top:20%;text-align:center;">
        <h2 class="Color1"><wu:Label ID="lblDownload" runat="server" Name="LinkBiExportFinished"></wu:Label><br /><br /></h2>
        <wu:Button ID="btnDownload" runat="server" Name="LinkBiDownloadExport" OnClientClick="Download();return false;" onmousedown="showSubmitLoading = false;" />
    </asp:Panel>
</asp:content>
