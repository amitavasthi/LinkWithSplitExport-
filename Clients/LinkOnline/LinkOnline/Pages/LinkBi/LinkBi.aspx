<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="LinkBi.aspx.cs" Inherits="LinkOnline.Pages.LinkBi.LinkBi" %>

<%@ Register Src="~/Classes/Controls/EquationDefinition.ascx" TagPrefix="uc1" TagName="EquationDefinition" %>
<%@ Register Src="~/Classes/Controls/HierarchySelector.ascx" TagPrefix="uc1" TagName="HierarchySelector" %>
<%@ Register Src="~/Classes/Controls/ConnectPowerBI.ascx" TagPrefix="uc1" TagName="ConnectPowerBI" %>
<%@ Register Src="~/Classes/Controls/VariableSearch.ascx" TagPrefix="uc1" TagName="VariableSearch" %>





<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/LinkBi.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Pages/LinkBi.js"></wu:ScriptReference>

    
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
    
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Controls/CategorySearch.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Controls/CategorySearch.js"></wu:ScriptReference>

    <script type="text/javascript">
        scrollScores = false;

        function BlurTable2() {
            UnBlurTable();

            var content = document.body;

            var blur = document.createElement("div");
            blur.id = "BlurTable";
            blur.className = "BlurTable";

            blur.style.marginLeft = "0px";
            blur.style.marginTop = "0px";
            blur.style.width = "100%";
            blur.style.height = "100%";

            content.insertBefore(blur, content.childNodes.item(0));
        }

        loadFunctions.push(function () {
            var table = document.getElementById("TableLinkBiDefinition");

            var height = ContentHeight - 68;

            var tableCellDimensions = GetChildByAttribute(table, "class", "PanelDimensions", true);
            var tableCellMeasures = GetChildByAttribute(table, "class", "PanelMeasures", true);

            tableCellDimensions.style.height = height + "px";
            tableCellMeasures.style.height = height + "px";
        });

        function BuildCrosstable() {
            window.location = window.location;
        }
    </script>
    <style type="text/css">
        .TableSettings {
            font-weight: bold;
        }

        .Box .BoxContent .TableSettings td {
            padding-top: 10px;
            padding-bottom: 10px;
            padding-left: 40px;
            padding-right: 40px;
        }

        .TableCellHeadlineIcons {
            padding:0px !important;
            width:100px;
            max-width:100px;
        }

        .TableCellHeadlineIcons table {
            width:100px;
        }

        .TableCellHeadlineIcons td {
            padding:0px !important;
        }

        .TableLinkBiDefinition {
            table-layout:fixed;
        }

        .TableCellHeadlineMeasures {
            width:50%;
        }

        .TableCellDimension, .TableCellMeasure {
            background-color:#FFFFFF;
        }
    </style>
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphTitle" runat="server">
    <table>
        <tr>
            <td>
                <h1 class="Color1">
                    <wu:Label ID="lblPageTitle" runat="server" Name="LinkBiTitle"></wu:Label><span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>&nbsp;-&nbsp; 
                </h1>
            </td>
            <td>
                <asp:LinkButton ID="btnBack" runat="server" OnClick="btnBack_Click">
                    <div class="BackButton" onclick="window.location='SavedDefinitions.aspx'" style=""></div>
                </asp:LinkButton>
            </td>
            <td>
                <h1 class="Color1">
                    <asp:Label ID="lblDefinitionName" runat="server"></asp:Label>
                </h1>
            </td>
        </tr>
    </table>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <div id="pnlData"></div>
    <uc1:VariableSearch runat="server" id="VariableSearch" />

    <div id="pnlRightPanelContainer" style="position:absolute;right:0px;z-index: 1010;">
        <table cellspacing="0" cellpadding="0" style="height:602px;">
            <tr>
                <td align="right">
                    <asp:Image ID="imgRightPanelTrigger" CssClass="BackgroundColor6" runat="server" ImageUrl="/Images/Icons/LinkReporterSettings/Expander.png" style="cursor:pointer;" onclick="ShowRightPanel(this);"></asp:Image>
                </td>
                <td>
                    <div id="pnlRightPanel" style="display:none;overflow:hidden;width:0px;border:1px solid #444444;">
                        <asp:Panel ID="pnlRightPanelNew" runat="server" class="RightPanelItem" onclick="ClearLinkBiDefinition();" style="">
                            <img src="/Images/Icons/LinkReporterSettings/New.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelNew" runat="server" Name="New"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelHierarchy" runat="server" class="RightPanelItem" onclick="" style="">
                            <img src="/Images/Icons/LinkReporterSettings/Hierarchy.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelHierarchy" runat="server" Name="Hierarchy"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelSettings" runat="server" class="RightPanelItem" onclick="InitDragBox('boxSettingsControl');" style="">
                            <img src="/Images/Icons/LinkReporterSettings/Settings.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelSettings" runat="server" Name="Settings"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelConnectPowerBI" runat="server" class="RightPanelItem" onclick="" style="">
                            <img src="/Images/Icons/LinkReporterSettings/ConnectPowerBI.png" style="width:40px;" /><br />
                            <wu:Label ID="lblRightPanelConnectPowerBI" runat="server" Name="LinkReporterSettingsConnectPowerBI"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelSave" runat="server" class="RightPanelItem" onclick="SaveLinkBiDefinition();" style="">
                            <img src="/Images/Icons/LinkReporterSettings/Save.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelSave" runat="server" Name="Save"></wu:Label>
                        </asp:Panel>
                        <asp:Panel id="pnlGoButton" runat="server" class="GoButton2">
                            <wu:Label ID="lblGoButton" runat="server" Name="Go"></wu:Label>
                        </asp:Panel>
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <div style="margin:0px">
        <asp:Panel ID="pnlLinkBiDefinition" runat="server"></asp:Panel>
    </div>
    
    <wu:HoverBox ID="hbLeftPanel" runat="server" Display="RightPanel" IdParent="hbGoButton" IdTrigger="pnlLeftPanelTrigger" style="margin-right:115px;z-index: 1010;">
        <asp:Panel ID="pnlLinkBiTools" runat="server"></asp:Panel>
    </wu:HoverBox>
    
    <script type="text/javascript">
        var linkBiSettingsChanged = false;
    </script>
    <wu:Box ID="boxSettings" runat="server" Title="Settings" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true" OnClientClose="if(!linkBiSettingsChanged) return; window.location = window.location; linkBiSettingsChanged = false;">
        <table class="TableSettings" cellpadding="10" style="font-weight:bold;">
            <tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsExportPercentage" runat="server" Name="ExportPercentage"></wu:Label>
                </td>
                <td>
                    <wu:Checkbox ID="chkLeftPanelSettingsExportPercentage" runat="server" onclick="UpdateLinkBiSetting('ExportPercentage', this.checked);"></wu:Checkbox>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsDisplayUnweightedBase" runat="server" Name="DisplayUnweightedBase"></wu:Label>
                </td>
                <td>
                    <wu:CheckBox ID="chkLeftPanelSettingsDisplayUnweightedBase" runat="server" onclick="UpdateLinkBiSetting('DisplayUnweightedBase', this.checked);"></wu:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsDataCheckEnabled" runat="server" Name="DataCheckEnabled"></wu:Label>
                </td>
                <td>
                    <wu:CheckBox ID="chkLeftPanelSettingsDataCheckEnabled" runat="server" onclick="UpdateLinkBiSetting('DataCheckEnabled', this.checked);"></wu:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsMetadataLanguage" runat="server" Name="MetadataLanguage"></wu:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlLeftPanelSettingsMetadataLanguage" runat="server" onchange="UpdateLinkBiSetting('IdLanguage', this.value);linkBiSettingsChanged = true;">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <wu:TipGallery ID="tgSettings" runat="server" _TipItems="SettingsTip1, SettingsTip2" />
    </wu:Box>
    
    <wu:Box ID="boxFilterDefinition" runat="server" Title="Filters" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true" OnClientClose="">
        <asp:Panel ID="pnlFilterCategories" runat="server" style="width:800px;margin:2em;" CssClass="BoxFilter"></asp:Panel>
        <wu:TipGallery ID="tgFilterDefinition" runat="server" _TipItems="FilterDefinitionTip1, FilterDefinitionTip2" />
    </wu:Box>
    
    <wu:Box ID="boxFilterSearch" runat="server" Title="SearchFilter" TitleLanguageLabel="true" Position="Top" JavascriptTriggered="true" Dragable="true">
        <wu:CategorySearch ID="csFilterDefinition" runat="server" SelectionType="Multi" style="width:1000px;">
        </wu:CategorySearch>
        <div id="btnCancelFilterCategorySearch" class="CancelCircle" onclick="document.getElementById('cphContent_csFilterDefinition').Close(false);" style="position:absolute;">
        </div>
        <div id="btnConfirmFilterCategorySearch" class="OkCircle" onclick="document.getElementById('cphContent_csFilterDefinition').Close(true);" style="position:absolute;margin-left: 1006px;">
        </div>
    </wu:Box>

    <script type="text/javascript">
        loadFunctions.push(function () {
            var pnlRightPanelContainer = document.getElementById("pnlRightPanelContainer");

            pnlRightPanelContainer.style.top = ((window.innerHeight / 2) - (pnlRightPanelContainer.offsetHeight / 2)) + "px";
        });
    </script>
    
    <asp:Panel ID="pnlWorkflowContainer" runat="server">
        <div id="WorkflowBackground" class="BoxBackground" style="opacity:0;display:none;"></div>
        <div id="WorkflowContainer" class="WorkflowContainer">
            <div class="BtnWorkflowShowPreview" onclick="ShowWorkflowSelectionDetail(this.getElementsByTagName('img').item(0));">
                <img class="BackgroundColor6" src="/Images/Icons/Expand.png" alt="Expand" />
            </div>
            <asp:Panel ID="pnlWorkflow" runat="server" style=""></asp:Panel>
            <div style="clear:both"></div>
        </div>
    </asp:Panel>

    <div id="pnlDefaultWeighting" class="PanelDefaultWeighting BorderColor1 Color1" style="display:none;">
        <table cellspacing="0" cellpadding="0" style="height:100%;width:100%;">
            <tr>
                <td style="padding:5px;font-weight:bold;font-size:14pt;">
                    <wu:Label ID="lblDefaultWeighting" runat="server" Name="Weight"></wu:Label>
                </td>
                <td style="padding:5px;">
                    <wu:DropDownList ID="ddlDefaultWeighting" runat="server"></wu:DropDownList>
                </td>
                <td align="right">
                    <div class="BackgroundColor6" onclick="HideDefaultWeightingSelector();" style="cursor:pointer;background-image:url('/Images/Icons/ExpandH.png');background-repeat:no-repeat;background-position:center center;width:18px;height:100%;">

                    </div>
                </td>
            </tr>
        </table>
    </div>
    <uc1:EquationDefinition runat="server" ID="EquationDefinition" />
    <uc1:HierarchySelector runat="server" id="HierarchySelector" />
    <uc1:ConnectPowerBI runat="server" id="ConnectPowerBI" />
</asp:content>


<asp:content id="Content4" contentplaceholderid="cphFooter" runat="server">
</asp:content>

