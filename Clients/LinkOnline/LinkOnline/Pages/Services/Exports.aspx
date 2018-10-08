<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Exports.aspx.cs" Inherits="LinkOnline.Pages.Services.Exports" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/Crosstables.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/Workflow.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/Filter.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/VariableSelection.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/ReportDefinition.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/Workflow.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/Filters.js"></wu:ScriptReference>
    
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSelector2.css"></wu:StylesheetReference>
    
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/VariableSelector.js"></wu:ScriptReference>
    
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/VariableSearch.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSearch.css"></wu:StylesheetReference>

    <style type="text/css">
        .TableCellHeadline {
            padding: 20px;
            color: #FFFFFF;
            font-weight: bold;
            font-size: 16pt;
            text-align: center;
        }
    </style>
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="Exports"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <table cellspacing="0" cellpadding="0" style="width:99%;">
        <tr>
            <td class="TableCellHeadline DarkGrayBackground" style="border-right:1px solid #FFFFFF;width:50%;">
                <wu:Label ID="lblAvailableVariables" runat="server" Name="AvailableVariables"></wu:Label>
            </td>
            <td class="TableCellHeadline DarkGrayBackground">
                <wu:Label ID="lblSelectedVariables" runat="server" Name="SelectedVariables"></wu:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlAvailableVariables" runat="server" CssClass="BackgroundColor7" DropArea="True" DropAreaMessage="DeSelect" style="height:500px;border-right:1px solid #FFFFFF;overflow:auto;"></asp:Panel>
            </td>
            <td>
                <asp:Panel ID="pnlSelectedVariables" runat="server" CssClass="BackgroundColor7" DropArea="True" DropAreaMessage="Select" style="height:500px;overflow:auto;"></asp:Panel>
            </td>
        </tr>
    </table>
    <img id="imgVariableSearchCancelBin" src="/Images/Icons/Bin.png" style="display:none;position:absolute;right:50px;bottom:50px;width:100px;opacity:1.0;" />
    <script type="text/javascript">

        function ClearExportDefinition() {
            _AjaxRequest("Exports.aspx", "ClearExportDefinition", "", function (response) {
                window.location = window.location;
            });
        }

        var selectedVariables = new Array();

        loadFunctions.push(function () {
            var pnlAvailableVariables = document.getElementById("cphContent_pnlAvailableVariables");
            var pnlSelectedVariables = document.getElementById("cphContent_pnlSelectedVariables");

            pnlAvailableVariables.style.height = (ContentHeight - 70) + "px";
            pnlSelectedVariables.style.height = (ContentHeight - 70) + "px";

            var pnlRightPanelContainer = document.getElementById("pnlRightPanelContainer");

            pnlRightPanelContainer.style.top = ((window.innerHeight / 2) - (pnlRightPanelContainer.offsetHeight / 2)) + "px";

            // Run through all variable select items.
            for (var i = 0; i < pnlAvailableVariables.childNodes.length; i++) {
                var pnlVariable = pnlAvailableVariables.childNodes.item(i);

                if (pnlVariable.className == undefined || pnlVariable.className.search("VariableSelectorControl") == -1)
                    continue;

                DragVariableSelector(pnlVariable, false);
            }
        });

        function Export() {
            var parameters = "SelectedVariables=";
            var pnlSelectedVariables = document.getElementById("cphContent_pnlSelectedVariables");
            for (var i = 1; i < pnlSelectedVariables.childNodes.length - 1; i++) {
                if (selectedVariables.length == 0) {
                    selectedVariables.push(pnlSelectedVariables.childNodes[i].getAttribute("idvariable"));
                }
            }

            if (selectedVariables.length > 0) {
                for (var i = 0; i < selectedVariables.length; i++) {
                    parameters += selectedVariables[i] + ",";
                }
                ShowLoading(document.body);

                _AjaxRequest("Exports.aspx", "Export", parameters, function (response) {
                    GetExportProgress();
                });
            }
        }

        function GetExportProgress() {
            _AjaxRequest("Exports.aspx", "GetExportProgress", "", function (response) {
                if (response == "100") {
                    GetExportFileName();

                    HideLoading();

                    return;
                }

                document.getElementById("LoadingText").innerHTML = response + "&nbsp;%";

                window.setTimeout(GetExportProgress, 200);
            });
        }

        function GetExportFileName() {
            window.location = "Exports.aspx?Method=DownloadExport";
            //_AjaxRequest("Exports.aspx", "GetExportFileName", "", function (response) {
            //    window.location = response;
            //});
        }

        function DeSelectAll() {
        }

        function SelectAll() {
            var pnlAvailableVariables = document.getElementById("cphContent_pnlAvailableVariables");

            for (var i = 0; i < pnlAvailableVariables.childNodes.length; i++) {

            }
        }

        function SelectVariable(sender, source, path, idVariable, isTaxonomy, idSelected) {
            _AjaxRequest("Exports.aspx", "SelectVariable", "Id=" + idVariable, function (response) {
                selectedVariables.push(idVariable);

                var pnlAvailableVariables = document.getElementById("cphContent_pnlAvailableVariables");
                var pnlSelectedVariables = document.getElementById("cphContent_pnlSelectedVariables");

                pnlAvailableVariables.style.background = "";
                pnlSelectedVariables.style.background = "";

                var sourceNode = GetChildByAttribute(pnlAvailableVariables, "id", sender.id);

                var clone = sender.cloneNode(true);

                clone.style.position = "";
                clone.style.textAlign = "left";
                clone.style.width = "100%";

                pnlSelectedVariables.appendChild(clone);

                sourceNode.parentNode.removeChild(sourceNode);
                sender.parentNode.removeChild(sender);
            });
        }
    </script>

    <div id="pnlRightPanelContainer" style="position:absolute;right:0px;z-index: 1010;">
        <table cellspacing="0" cellpadding="0" style="height:602px;">
            <tr>
                <td align="right">
                    <asp:Image ID="imgRightPanelTrigger" CssClass="BackgroundColor6" runat="server" ImageUrl="/Images/Icons/LinkReporterSettings/Expander.png" style="cursor:pointer;" onclick="ShowRightPanel(this);"></asp:Image>
                </td>
                <td>
                    <div id="pnlRightPanel" style="display:none;overflow:hidden;width:0px;border:1px solid #444444;">
                        <asp:Panel ID="pnlRightPanelNew" runat="server" class="RightPanelItem" onclick="ClearExportDefinition();" style="">
                            <img src="/Images/Icons/LinkReporterSettings/New.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelNew" runat="server" Name="New"></wu:Label>
                        </asp:Panel>
                        <!--<asp:Panel ID="pnlRightPanelSettings" runat="server" class="RightPanelItem DarkGrayBackground" onclick="InitDragBox('boxSettingsControl');" style="padding:12px 25px 12px 25px;color:#FFFFFF;text-align:center;">
                            <img src="/Images/Icons/LinkReporterSettings/Settings.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelSettings" runat="server" Name="Settings"></wu:Label>
                        </asp:Panel>-->
                        <asp:Panel id="pnlGoButton" runat="server" class="GoButton2" onclick="Export();">
                            <wu:Label ID="lblGoButton" runat="server" Name="Go"></wu:Label>
                        </asp:Panel>
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <div id="boxVariableSearch"></div>
    
    <div id="WorkflowBackground" class="BoxBackground" style="opacity:0;display:none;"></div>
    <div id="WorkflowContainer" class="WorkflowContainer">
        <div class="BtnWorkflowShowPreview" onclick="ShowWorkflowSelectionDetail(this.getElementsByTagName('img').item(0));">
            <img class="BackgroundColor6" src="/Images/Icons/Expand.png" alt="Expand" />
        </div>
        <asp:Panel ID="pnlWorkflow" runat="server" style=""></asp:Panel>
        <div style="clear:both"></div>
    </div>
</asp:content>
