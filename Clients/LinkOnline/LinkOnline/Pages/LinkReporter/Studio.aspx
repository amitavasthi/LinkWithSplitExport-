<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Studio.aspx.cs" Inherits="LinkOnline.Pages.LinkReporter.Studio" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .Content {
            background-color: #E4E4E4;
        }

        .ReportTabContainer {
            height: 28px;
            margin-top: 10px;
        }

        .ReportTab {
            height: 28px;
            width: 140px;
            
            float: left;
            overflow: hidden;
            white-space: nowrap;
            
            padding-left: 30px;
            padding-right: 30px;
            
            color: #FFFFFF;
            font-weight: bold;
            cursor: pointer;
            text-align: center;
            border-right: 1px solid #FFFFFF;
        }

        .ReportTab_Active {
            background-color: #FFFFFF;
        }

        .ReportTabLabel {
            margin-top: 5px;
        }
    </style>
    <script type="text/javascript">
        loadFunctions.push(function () {
            document.getElementById("reportFrame").style.height = (ContentHeight - 60) + "px";
        });
    </script>
</asp:Content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><asp:Label ID="lblPageTitle" runat="server"></asp:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script type="text/javascript">
        function ChangeReportTab(sender, source) {
            document.getElementById("reportFrame").src = "http://youtube.tokyo.local/Fileadmin/CustomCharts/StudioV2/index.html?" + source;

            var elements = $("#" + sender.parentNode.id + " .ReportTab_Active");

            for (var i = 0; i < elements.length; i++) {
                elements[i].className = "ReportTab BackgroundColor10";
            }

            sender.className = "ReportTab Color1I ReportTab_Active";
        }
    </script>
    <div style="margin:20px;">
        <asp:Panel ID="pnlReportTabs" runat="server" CssClass="ReportTabContainer">
        </asp:Panel>
        <div style="padding:10px;background:#FFFFFF">
            <iframe id="reportFrame" frameborder="0" style="width:100%;height:100%;" src="http://youtube.tokyo.local/Fileadmin/CustomCharts/StudioV2/index.html"></iframe>
        </div>
    </div>
</asp:Content>
