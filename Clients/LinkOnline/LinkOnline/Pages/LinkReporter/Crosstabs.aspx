<%@ Page
    Title=""
    Language="C#"
    AutoEventWireup="true"
    CodeBehind="Crosstabs.aspx.cs"
    Inherits="LinkOnline.Pages.LinkReporter.Crosstabs"
    EnableEventValidation="false"
    EnableViewState="false" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>
<%@ Register Src="~/Classes/Controls/EquationDefinition.ascx" TagPrefix="uc1" TagName="EquationDefinition" %>
<%@ Register Src="~/Classes/Controls/HierarchySelector.ascx" TagPrefix="uc2" TagName="HierarchySelector" %>
<%@ Register Src="~/Classes/Controls/ConnectPowerBI.ascx" TagPrefix="uc1" TagName="ConnectPowerBI" %>
<%@ Register Src="~/Classes/Controls/VariableSearch.ascx" TagPrefix="uc1" TagName="VariableSearch" %>




<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <meta name="format-detection" content="telephone=no"/>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/Crosstables.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/Workflow.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/Filter.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/VariableSelector.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/ReportDefinition.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/Workflow.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/Filters.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/Voice.js"></wu:ScriptReference>
    
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Includes/select2.min.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Includes/select2.full.js"></wu:ScriptReference>
    
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/VariableSearch.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSearch.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSelector2.css"></wu:StylesheetReference>
    
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSelector.css"></wu:StylesheetReference>
    
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/ReportTabs.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/ReportTabs.js"></wu:ScriptReference>
    
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/EquationWizard.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/EquationWizard.js"></wu:ScriptReference>
    
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/SpeechRecognition.css"></wu:StylesheetReference>
    <%--<wu:ScriptReference runat="server" Source="/Scripts/Modules/SpeechRecognition.js"></wu:ScriptReference>--%>
    
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Controls/CategorySearch.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Controls/CategorySearch.js"></wu:ScriptReference>

    <style type="text/css">
        .RendererV2 {
            border-collapse: collapse;
        }

            .RendererV2 td {
                border: 1px solid #8B8888;
                text-align: center;
                vertical-align: middle;
                font-family: SegoeUI;
                overflow: hidden;
                /*box-sizing: border-box;
        -moz-box-sizing: border-box;
        -ms-box-sizing: border-box;
        -webkit-box-sizing: border-box;*/
            }

        .selectedNode {
            letter-spacing: 1.8px;
            line-height: 25px;
            border-bottom: 2px dashed #FCC16B;
        }

        .RendererV2 .TableCellBase {
            background: #EEEEEE;
        }

        .RendererV2 .TableCellSigDiff {
            cursor: pointer;
        }
    </style>


    <style type="text/css">
        .CrosstabSelection {
            width: 100%;
        }


            .CrosstabSelection td {
                width: 25%;
            }

        .CrosstabSelectionItem {
            text-align: center;
            padding-top: 20px;
            padding-bottom: 20px;
            color: #FFFFFF;
            font-size: 16pt;
        }

        .Test td {
            border: 1px solid #444444;
        }


        .TableSettings {
            font-weight: bold;
        }

        .Box .BoxContent .TableSettings td {
            padding-top: 10px;
            padding-bottom: 10px;
            /*padding-left: 40px;*/
            padding-left: 20px;
            padding-right: 40px;
        }

        #WorkflowContainer .Customcheckbox {
            height: 12px;
            width: 12px;
        }

        .applyAllChks {
            position: absolute;
            right: 8px;
            top: 12px;
        }

        .Box .BoxContent .TableSettings td {
            position: relative;
        }

        .applyAllChks > .Customcheckbox {
            width: 15px;
            height: 15px;
        }


        .TableCellExportType:hover {
            font-weight: bold;
        }

        label[for=cphContent_chkSplitnExportAllTabs] {
            position: relative;
            left: -10px;
            top: -9px;
        }

        .btnSaveTabConfirm {
            margin-top: 20px !important;
        }

        #cphContent_TreeView1 {
            max-width: 500px;
        }
    </style>
    
    <!--<script src="/Scripts/JQuery/jquery-1.7.2.js" type="text/javascript"></script>-->
    <script src="/Scripts/JQuery/jquery-ui-1.7.2.custom.min.js"></script>
    <link rel="stylesheet" href="/Stylesheets/JQuery/jqueryui/ui-lightness/jquery-ui-1.7.2.custom.css" />

    <script src="https://code.highcharts.com/highcharts.js"></script>
    <script src="https://code.highcharts.com/highcharts-more.js"></script>
    <script src="https://code.highcharts.com/modules/exporting.js"></script>

    <script src="/Scripts/Jquery/AwesomeCloud/jquery.awesomeCloud-0.2.min.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $(".FileExplorerItem").click(function () {
                var elements = document.getElementsByClassName('selectedNode');
                while (elements.length > 0) {
                    elements[0].classList.remove('selectedNode');
                }
                this.classList.add('selectedNode');
            });
        });
        loadFunctions.push(function () {
            document.body.onkeypress = function (e) {
                //$(document).keydown(function (event) {
                if (event.keyCode != 26)
                    return;


                _AjaxRequest("/Handlers/GlobalHandler.ashx", "ReportDefinitionHistoryBack", "", function (response) {
                    //window.location = window.location;
                    PopulateCrosstable();
                });
                //});
            };
        });


        loadFunctions.push(function () {
            var reportContainer = document.getElementById("cphContent_pnl");

            // Check if the workflow is pinned.
            if ($(".WorkflowPinned").length != 0) {
                reportContainer.style.height = (ContentHeight - 300 - 65) + "px";
            }
            else {
                reportContainer.style.height = (ContentHeight - 65) + "px";
            }
        });

        loadFunctions.push(BindReportDisplayTypes);


        function ShowSigDiff(sender, color) {
            var font = "";

            if (color == undefined)
                color = "";
            else {
                font = "#FFFFFF";
                color = "#444444";
            }

            if (color != '')
                sender.setAttribute("onmouseout", "ShowSigDiff(this)");
            else
                sender.removeAttribute("onmouseout");

            var innerText = sender.innerText;

            while (innerText.indexOf('*') != -1) {
                innerText = innerText.replace('*', '');
            }

            var letters = innerText.split(',');
            letters.push(sender.getAttribute("SigDiffLetter"));

            if (letters.length == 1)
                return;

            var tableRowsHeadline = document.getElementById("pnlTopHeadline").getElementsByTagName("tr");

            var sigDiffContext = sender.getAttribute("SigDiffContext");

            var tableCellsSigDiff = sender.parentNode.getElementsByTagName("td");
            var tableCellsValue = sender.parentNode.previousSibling.previousSibling.getElementsByTagName("td");
            var tableCellsPercentage = sender.parentNode.previousSibling.getElementsByTagName("td");
            var tableCellsHeadline = tableRowsHeadline.item(tableRowsHeadline.length - 2).getElementsByTagName("td");

            var cells = new Object();

            GetSigDiffCells(tableCellsSigDiff, cells, sigDiffContext);
            GetSigDiffCells(tableCellsValue, cells, sigDiffContext);
            GetSigDiffCells(tableCellsPercentage, cells, sigDiffContext);
            //GetSigDiffCells(tableCellsHeadline, cells, sigDiffContext);

            for (var i = 0; i < tableCellsValue.length; i++) {
                var tableCell = tableCellsValue.item(i);

                if (tableCell.getAttribute("SigDiffContext") != sigDiffContext)
                    continue;

                if (cells[tableCell.getAttribute("SigDiffLetter")] == undefined)
                    cells[tableCell.getAttribute("SigDiffLetter")] = new Array();

                if (tableCellsHeadline.length > i)
                    cells[tableCell.getAttribute("SigDiffLetter")].push(tableCellsHeadline.item(i));
            }

            for (var i = 0; i < letters.length; i++) {
                var letter = letters[i].trim();

                if (cells[letter] == undefined)
                    continue;

                for (var c = 0; c < cells[letter].length; c++) {
                    cells[letter][c].style.background = color;
                    cells[letter][c].style.color = font;
                }
            }
        }

        function GetSigDiffCells(tableCells, cells, sigDiffContext) {
            for (var i = 0; i < tableCells.length; i++) {
                var tableCell = tableCells.item(i);

                if (tableCell.getAttribute("SigDiffContext") != sigDiffContext)
                    continue;

                if (cells[tableCell.getAttribute("SigDiffLetter")] == undefined)
                    cells[tableCell.getAttribute("SigDiffLetter")] = new Array();

                cells[tableCell.getAttribute("SigDiffLetter")].push(tableCell);
            }
        }
        function RenderValues() {

            UpdateSetting('AutoUpdate', 'true', true, true);
            //UpdateSetting('AutoUpdate', 'true', true, false);


            //settingsChanged = true;
            //var parameters = "Name=AutoUpdate&Value=true&ClearData=true";
            //settingsChanged = true;
            //AjaxRequest("UpdateSetting", parameters, function (response) {                
            //        PopulateCrosstable();
            //});


        }

    </script>
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><asp:Label ID="lblPageTitle" runat="server"></asp:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <script type="text/javascript">
        resizeFunctions.push(PopulateCrosstable);
    </script>
    <asp:Panel ID="pnlReportTabs" runat="server" CssClass="ReportTabContainer">
        <asp:HiddenField ID="cogSrchApi" runat="server" />
    </asp:Panel>
    <div id="pnlRightPanelContainer" style="position:absolute;right:0px;z-index: 1010;">
        <table cellspacing="0" cellpadding="0" style="height:712px;">
            <tr>
                <td align="right">
                    <asp:Image ID="imgRightPanelTrigger" CssClass="BackgroundColor6" runat="server" ImageUrl="/Images/Icons/LinkReporterSettings/Expander.png" style="cursor:pointer;" onclick="ShowRightPanel(this);"></asp:Image>
                </td>
                <td>
                    <div id="pnlRightPanel" class="BorderColor1" style="display:none;overflow:hidden;width:0px;border-width:1px;border-style:solid;">
                        <asp:Panel ID="pnlRightPanelNew" runat="server" class="RightPanelItem" onclick="ClearReportDefinition();" style="">
                            <img src="/Images/Icons/LinkReporterSettings/New.png" style="width:40px;" /><br />
                            <wu:Label ID="lblRightPanelNew" runat="server" Name="New"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelHierarchy" runat="server" class="RightPanelItem" onclick="" style="">
                            <img src="/Images/Icons/LinkReporterSettings/Hierarchy.png" style="width:40px;" /><br />
                            <wu:Label ID="lblRightPanelHierarchy" runat="server" Name="Hierarchy"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelSettings" runat="server" class="RightPanelItem" onclick="clearAllCheckBox();InitDragBox('boxSettingsControl');" style="">
                            <img src="/Images/Icons/LinkReporterSettings/Settings.png" style="width:40px;" /><br />
                            <wu:Label ID="lblRightPanelSettings" runat="server" Name="Settings"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelView" runat="server" class="RightPanelItem" style="">
                            <img src="/Images/Icons/LinkReporterSettings/View.png" style="width:40px;" /><br />
                            <wu:Label ID="lblRightPanelView" runat="server" Name="View"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelExport" runat="server" class="RightPanelItem" onclick="ExportReport();" style="">
                            <img src="/Images/Icons/LinkReporterSettings/Export.png" style="width:40px;" /><br />
                            <wu:Label ID="lblRightPanelExport" runat="server" Name="Export"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelConnectPowerBI" runat="server" class="RightPanelItem" onclick="ConnectPowerBI(window.location.search.search('SavedReport') != -1 ? window.location.search.split('SavedReport=')[1].split('&')[0] : undefined, 'Reporter');" style="">
                            <img src="/Images/Icons/LinkReporterSettings/ConnectPowerBI.png" style="width:40px;" /><br />
                            <wu:Label ID="lblRightPanelConnectPowerBI" runat="server" Name="LinkReporterSettingsConnectPowerBI"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelSave" runat="server" class="RightPanelItem" onclick="if(window.location.toString().search('SavedReport') != -1) { ShowSaveReportLocation() } else { InitDragBox('boxSaveControl', 'Center'); } document.getElementById('cphContent_txtSaveTabName').style.borderColor = '';" style="">
                            <img src="/Images/Icons/LinkReporterSettings/Save.png" style="width:40px;" /><br />
                            <wu:Label ID="lblRightPanelSave" runat="server" Name="Save"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlGoButton" runat="server" class="GoButton2" onclick="UpdateSetting('AutoUpdate', 'true', true, true);">
                            <wu:Label ID="lblGoButton" runat="server" Name="Go"></wu:Label>
                        </asp:Panel>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <wu:Box ID="boxSecretSettings" runat="server" Title="SecretSettings" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true" OnClientClose="window.setTimeout(PopulateCrosstable, 500);document.body.focus();">
        <table class="TableSettings" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <wu:Label ID="lblSecretSettingsPercentageBase" runat="server" Name="SecretSettingsPercentageBase"></wu:Label>
                </td>
                <td>
                    <wu:DropDownList ID="ddlSecretSettingsPercentageBase" runat="server" maxlength="1" onchange="UpdateSetting('PercentageBase', this.value);">
                        <asp:ListItem Value="Row">row</asp:ListItem>
                        <asp:ListItem Value="Column">column</asp:ListItem>
                    </wu:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblSecretSettingsPowerBIValues" runat="server" Name="SecretSettingsPowerBIValues"></wu:Label>
                </td>
                <td>
                    <wu:DropDownList ID="ddlSecretSettingsPowerBIValues" runat="server" maxlength="1" onchange="UpdateSetting('PowerBIValues', this.value);">
                        <asp:ListItem Value="Values">values</asp:ListItem>
                        <asp:ListItem Value="Percentages">percentages</asp:ListItem>
                    </wu:DropDownList>
                </td>
            </tr>
        </table>
    </wu:Box>
    <wu:Box ID="boxSettings" runat="server" Title="Settings" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true" OnClientClose="document.body.focus();if(document.getElementById('cphContent_chkLeftPanelSettingsHideLowBase').checked){UpdateHideLowBase();}if(!settingsChanged) return; window.setTimeout(PopulateCrosstable, 500);settingsChanged = false; ">
        <table class="TableSettings" cellpadding="0" cellspacing="0">
            <tr style="font-size:16pt;font-weight:normal">
                <td colspan="2">
                    <wu:Label ID="lblSettingsGeneral" runat="server" Name="General"></wu:Label>
                    <wu:Label ID="lblSettingsAllGeneral" runat="server" Name="AllGeneral" class="applyAllChks"></wu:Label>
                </td>
                <td colspan="2">
                    <wu:Label ID="lblSettingsSignificanceDifference" runat="server" Name="SignificanceDifference"></wu:Label>
                    <wu:Label ID="lblSettingsAllSignificanceDifference" runat="server" Name="AllGeneral"  class="applyAllChks"></wu:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsBaseType" runat="server" Name="BaseType"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid">
                    <asp:DropDownList ID="ddlLeftPanelSettingsBaseType" runat="server" onchange="UpdateSetting('BaseType', this.value, true); clearCheckBox('chkAllLeftPanelSettingsBaseType'); ">
                    </asp:DropDownList>
                <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsBaseType" runat="server" onclick="UpdateSettingAll('BaseType', document.getElementById('cphContent_ddlLeftPanelSettingsBaseType').value, true, true);"></wu:CheckBox></div>
                </td>
                  <td>
                    <wu:Label ID="lblLeftPanelSettingsSignificanceTest" runat="server" Name="SignificanceTest"></wu:Label>
                </td>
                <td>
                   <asp:DropDownList ID="ddlLeftPanelSettingsSignificanceTestType" runat="server" onchange="if(this.value==0){ UpdateSetting('SignificanceTest', false);}
                       else{UpdateSetting('SignificanceTest', true);UpdateSetting('SignificanceTestType',this.value,true);}clearCheckBox('chkAllLeftPanelSettingsSignificanceTestType');">
                        <asp:ListItem Text="No test selected " Value="0"></asp:ListItem>
                        <asp:ListItem Text="IBM based T - Test" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Dependent(Multi/Overlap) - Test" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Independent - Test " Value="3"></asp:ListItem>
                        <asp:ListItem Text="Test against Total " Value="4"></asp:ListItem>
                    </asp:DropDownList>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsSignificanceTestType" runat="server" onclick="if(document.getElementById('cphContent_ddlLeftPanelSettingsSignificanceTestType').value==0){ UpdateSettingAll('SignificanceTest', false, false, true);}
                       else{UpdateSettingAll('SignificanceTest', true, false, true);UpdateSettingAll('SignificanceTestType',document.getElementById('cphContent_ddlLeftPanelSettingsSignificanceTestType').value, true, true);}"></wu:CheckBox></div>
                    <wu:CheckBox style="display:none;" ID="chkLeftPanelSettingsSignificanceTest" runat="server" onclick="UpdateSetting('SignificanceTest', this.checked);"></wu:CheckBox>
                </td>
                <%--<td>
                    <wu:Label ID="lblLeftPanelSettingsSigDiffEffectiveBase" runat="server" Name="SigDiffEffectiveBase"></wu:Label>
                </td>
                <td>
                    <wu:CheckBox ID="chkLeftPanelSettingsSigDiffEffectiveBase" runat="server" onclick="UpdateSetting('SigDiffEffectiveBase', this.checked);"></wu:CheckBox>
                </td>--%>
            </tr>
            <tr>
                 <td>
                    <wu:Label ID="Label3" runat="server" Name="LowBaseConsider"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid"><br /><br />
                    <asp:DropDownList ID="ddlLeftPanelSettingsLowBaseConsider" runat="server" onchange="UpdateSetting('LowBaseConsider', this.value, true);clearCheckBox('chkAllLeftPanelSettingsLowBaseConsider');">
                         <asp:ListItem Text="unweighted" Value="1"></asp:ListItem>
                        <asp:ListItem Text="weighted" Value="2"></asp:ListItem> 
                         <asp:ListItem Text="effective" Value="3"></asp:ListItem>  
                       
                    </asp:DropDownList>
                     <div class="applyAllChks"><br /><br />    <wu:CheckBox ID="chkAllLeftPanelSettingsLowBaseConsider" runat="server" onclick="UpdateSettingAll('LowBaseConsider', document.getElementById('cphContent_ddlLeftPanelSettingsLowBaseConsider').value, true, true);"></wu:CheckBox></div>

                        <td><br />
                    <wu:Label ID="lblLeftPanelSettingsSignificanceTestLevel" runat="server" Name="SignificanceTestLevel"></wu:Label>
                </td>
                <td><br />
                    <asp:DropDownList ID="ddlLeftPanelSettingsSignificanceTestLevel" runat="server" onchange="UpdateSetting('SignificanceTestLevel', this.value, true);clearCheckBox('chkAllLeftPanelSettingsSignificanceTestLevel');">
                        <asp:ListItem Text="95%" Value="95"></asp:ListItem>
                        <asp:ListItem Text="90%" Value="90"></asp:ListItem>
                    </asp:DropDownList>
                     <div class="applyAllChks">  <br />  <wu:CheckBox ID="chkAllLeftPanelSettingsSignificanceTestLevel" runat="server" onclick="UpdateSettingAll('SignificanceTestLevel', document.getElementById('cphContent_ddlLeftPanelSettingsSignificanceTestLevel').value, true, true);"></wu:CheckBox></div>
                </td>
             
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsLowBase" runat="server" Name="LowBase"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid">
                    <wu:TextBox ID="txtLeftPanelSettingsLowBase" class="decimal" TextMode="Number" runat="server" onclick="this.onkeyup();" onkeypress="return isNumber(event)" onchange="document.getElementById('cphContent_chkLeftPanelSettingsHideLowBase').checked=false;document.getElementById('cphContent_chkLeftPanelSettingsHideLowBase').previousSibling.src='/Images/Icons/Boxes/checkbox/Inactive.png';clearCheckBox('chkAllLeftPanelSettingsLowBase');"  onkeyup="UpdateSetting('LowBase', parseInt(this.value) < 0 ? '0': this.value == '' ? '0' : (this.value), true);if(this.value.indexOf('.')>-1) this.value=this.value.substring(0,this.value.indexOf('.'));"></wu:TextBox>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsLowBase" runat="server" onclick="UpdateSettingAll('LowBase', parseInt(document.getElementById('cphContent_txtLeftPanelSettingsLowBase').value) < 0 ? '0': document.getElementById('cphContent_txtLeftPanelSettingsLowBase').value == '' ? '0' : (document.getElementById('cphContent_txtLeftPanelSettingsLowBase').value), true, true);"></wu:CheckBox></div>
                </td>
               <td>
                    <wu:Label ID="Label1" runat="server" Name="SignificanceWeight"></wu:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlLeftPanelSettingsSignificanceweight" runat="server" onchange="UpdateSetting('SignificanceWeight', this.value, true);clearCheckBox('chkAllLeftPanelSettingsSignificanceweight');">
                         <asp:ListItem Text="unweighted" Value="1"></asp:ListItem>
                        <asp:ListItem Text="weighted" Value="2"></asp:ListItem> 
                         <asp:ListItem Text="effective" Value="3"></asp:ListItem>  
                       
                    </asp:DropDownList>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsSignificanceweight" runat="server" onclick="UpdateSettingAll('SignificanceWeight', document.getElementById('cphContent_ddlLeftPanelSettingsSignificanceweight').value, true, true);"></wu:CheckBox></div>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsDecimalPlaces" runat="server" Name="DecimalPlaces"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid">
                    <wu:TextBox ID="txtLeftPanelSettingsDecimalPlaces" class="decimal" TextMode="Number" runat="server"  maxlength="1" onclick="this.onkeyup();" onkeypress="return isNumber(event)"  onkeyup="UpdateSetting('DecimalPlaces',parseInt(this.value) < 0 ? '0': this.value == '' ? '0' : (parseInt(this.value) > 9 ? '9' : this.value));if(parseInt(this.value) > 9) this.value=9;if(!validnum(this)) this.value='';clearCheckBox('chkAllLeftPanelSettingsDecimalPlaces');"></wu:TextBox>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsDecimalPlaces" runat="server" onclick="UpdateSettingAll('DecimalPlaces', parseInt(document.getElementById('cphContent_txtLeftPanelSettingsDecimalPlaces').value) < 0 ? '0': document.getElementById('cphContent_txtLeftPanelSettingsDecimalPlaces').value == '' ? '0' : (parseInt(document.getElementById('cphContent_txtLeftPanelSettingsDecimalPlaces').value) > 15 ? '15' : document.getElementById('cphContent_txtLeftPanelSettingsDecimalPlaces').value), false, true);"></wu:CheckBox></div>
                </td>
                

            
                </tr>
           
            <tr>                      
                 <td>
                    <wu:Label ID="lblLeftPanelSettingsDisplayUnweightedBase" runat="server" Name="DisplayUnweightedBase"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid">
                    <wu:CheckBox ID="chkLeftPanelSettingsDisplayUnweightedBase" runat="server" onclick="UpdateSetting('DisplayUnweightedBase', this.checked);clearCheckBox('chkAllLeftPanelSettingsDisplayUnweightedBase');"></wu:CheckBox>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsDisplayUnweightedBase" runat="server" onclick="UpdateSettingAll('DisplayUnweightedBase', document.getElementById('cphContent_chkLeftPanelSettingsDisplayUnweightedBase').checked, false, true);"></wu:CheckBox></div>
                </td>                
                 <td colspan="2" style="font-size:16pt;font-weight:normal">
                    <wu:Label ID="lblSettingsRenderer" runat="server" Name="SettingsRendererTitle"></wu:Label>
                </td>
             
            
            </tr>
            <tr>
                  <td>
                    <wu:Label ID="Label2" runat="server" Name="DisplayEffectiveBase"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid"> 
                    <wu:CheckBox ID="chkLeftPanelSettingsDisplayEffectiveBase" runat="server" onclick="UpdateSetting('DisplayEffectiveBase', this.checked);clearCheckBox('chkAllLeftPanelSettingsDisplayEffectiveBase');"></wu:CheckBox>
                     <div class="applyAllChks"> <wu:CheckBox ID="chkAllLeftPanelSettingsDisplayEffectiveBase" runat="server" onclick="UpdateSettingAll('DisplayEffectiveBase', document.getElementById('cphContent_chkLeftPanelSettingsDisplayEffectiveBase').checked, false, true);"></wu:CheckBox></div>
                </td>
                  <td>
                    <wu:Label ID="lblLeftPanelSettingsDisplay" runat="server" Name="SettingsDisplay"></wu:Label>
                </td> 
                <td class="BorderColor1" style="">
                    <wu:DropDownList ID="ddlLeftPanelSettingsDisplay" runat="server" maxlength="1" onchange="if (this.value == '0') { UpdateSetting('ShowValues', 'True'); UpdateSetting('ShowPercentage', 'True'); } else if (this.value == '1') { UpdateSetting('ShowValues', 'True'); UpdateSetting('ShowPercentage', 'False'); } else if (this.value == '2') { UpdateSetting('ShowValues', 'False'); UpdateSetting('ShowPercentage', 'True'); }clearCheckBox('chkAllLeftPanelSettingsDisplay');">
                    </wu:DropDownList>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsDisplay" runat="server" onclick="var value = document.getElementById('cphContent_ddlLeftPanelSettingsDisplay').value;if (value == '0') { UpdateSettingAll('ShowValues', 'True', false, true); UpdateSettingAll('ShowPercentage', 'True', false, true); } else if (value == '1') { UpdateSettingAll('ShowValues', 'True', false, true); UpdateSettingAll('ShowPercentage', 'False', false, true); } else if (value == '2') { UpdateSettingAll('ShowValues', 'False', false, true); UpdateSettingAll('ShowPercentage', 'True', false, true);} "></wu:CheckBox></div>
                </td>              
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsScrollLabels" runat="server" Name="ScrollLabels"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid">
                    <wu:CheckBox ID="chkLeftPanelSettingsScrollLabels" runat="server" onclick="UpdateSetting('ScrollLabels', this.checked);clearCheckBox('chkAllLeftPanelSettingsScrollLabels');"></wu:CheckBox>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsScrollLabels" runat="server" onclick="UpdateSettingAll('ScrollLabels', document.getElementById('cphContent_chkLeftPanelSettingsScrollLabels').checked, false, true);"></wu:CheckBox></div>
                </td>
                 <td>
                    <wu:Label ID="lblLeftPanelSettingsMinWidth" runat="server" Name="SettingsMinWidth"></wu:Label>
                </td>
                <td class="BorderColor1" style="">
                    <wu:TextBox ID="txtLeftPanelSettingsMinWidth" TextMode="Number" runat="server" maxlength="3" onkeypress="return isNumber(event)"   onkeyup="UpdateSetting('MinWidth', isNaN(parseInt(this.value)) == true ? '0' : parseInt(this.value));if(!validnum(this)) this.value='';clearCheckBox('chkAllLeftPanelSettingsMinWidth');"></wu:TextBox>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsMinWidth" runat="server" onclick="UpdateSettingAll('MinWidth', isNaN(parseInt(document.getElementById('cphContent_txtLeftPanelSettingsMinWidth').value)) == true ? '0' : parseInt(document.getElementById('cphContent_txtLeftPanelSettingsMinWidth').value), false, true);"></wu:CheckBox></div>
                </td>             
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsHideEmptyRowsAndColumns" runat="server" Name="HideEmptyRowsAndColumns"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid">
                    <wu:CheckBox ID="chkLeftPanelSettingsHideEmptyRowsAndColumns" runat="server" onclick="UpdateSetting('HideEmptyRowsAndColumns', this.checked, true);clearCheckBox('chkAllLeftPanelSettingsHideEmptyRowsAndColumns');"></wu:CheckBox>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsHideEmptyRowsAndColumns" runat="server" onclick="UpdateSettingAll('HideEmptyRowsAndColumns', document.getElementById('cphContent_chkLeftPanelSettingsHideEmptyRowsAndColumns').checked, true, true);"></wu:CheckBox></div>
                </td>
               
                      <td>
                    <wu:Label ID="lblLeftPanelSettingsMinHeight" runat="server" Name="SettingsMinHeight"></wu:Label>
                </td>
                <td class="BorderColor1" style="">
                    <wu:TextBox ID="txtLeftPanelSettingsMinHeight" class="decimalHeight" TextMode="Number" runat="server" onchange="rangeVaid(this)" maxlength="3" onkeypress="return isNumber(event)"  onkeyup="UpdateSetting('MinHeight', isNaN(parseInt(this.value)) == true ? '0' : parseInt(this.value));if(!validnum(this)) this.value='';clearCheckBox('chkAllLeftPanelSettingsMinHeight');"></wu:TextBox>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsMinHeight" runat="server" onclick="var value = document.getElementById('cphContent_txtLeftPanelSettingsMinHeight').value;UpdateSettingAll('MinHeight', isNaN(parseInt(value)) == true ? '0' : parseInt(value), false, true);"></wu:CheckBox></div>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsRankLeft" runat="server" Name="SettingsRankLeft"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid">
                    <wu:CheckBox ID="chkLeftPanelSettingsRankLeft" runat="server" onclick="UpdateSetting('RankLeft', this.checked, true);clearCheckBox('chkAllLeftPanelSettingsRankLeft');"></wu:CheckBox>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsRankLeft" runat="server" onclick="UpdateSettingAll('RankLeft', document.getElementById('cphContent_chkLeftPanelSettingsRankLeft').checked, true, true);"></wu:CheckBox></div>
                </td>
                  <td>
                    <wu:Label ID="lblLeftPanelSettingsCategoryLimit" runat="server" Name="CategoryLimit"></wu:Label>
                </td>
                <td class="BorderColor1" style="">
                    <wu:TextBox ID="txtLeftPanelSettingsCategoryLimit" class="decimal" TextMode="Number" runat="server" onclick="this.onkeyup();" onkeyup="UpdateSetting('CategoryLimit',parseInt(this.value) < 0 ? '0': this.value == '' ? '0' : (this.value), true);clearCheckBox('chkAllLeftPanelSettingsCategoryLimit');"></wu:TextBox>
                     <div class="applyAllChks">    <wu:CheckBox ID="chkAllLeftPanelSettingsCategoryLimit" runat="server" onclick="UpdateSettingAll('CategoryLimit', parseInt(document.getElementById('cphContent_txtLeftPanelSettingsCategoryLimit').value) < 0 ? '0': document.getElementById('cphContent_txtLeftPanelSettingsCategoryLimit').value == '' ? '0' : (document.getElementById('cphContent_txtLeftPanelSettingsCategoryLimit').value), true, true);"></wu:CheckBox></div>
                </td>  
        
            </tr>
             <tr style="display:none;">
                <td>
                   <wu:Label ID="lblLeftPanelSettingsHideLowbase" runat="server" Name="HideLowBase"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid">
                    <wu:CheckBox ID="chkLeftPanelSettingsHideLowBase" runat="server"></wu:CheckBox>
                </td>
            </tr>
            <!--<tr>
                <td>
                    <wu:Label ID="lblLeftPanelSettingsRankTop" runat="server" Name="SettingsRankTop"></wu:Label>
                </td>
                <td class="BorderColor1" style="border-right-width:1px;border-right-style:solid">
                    <wu:CheckBox ID="chkLeftPanelSettingsRankTop" runat="server" onclick="UpdateSetting('RankTop', this.checked, true);"></wu:CheckBox>
                </td>
            </tr>-->
        </table>
        <wu:TipGallery ID="tgSettings" runat="server" _TipItems="SettingsTip1, SettingsTip2" />
    </wu:Box>
    <div id="pnlExport" class="Color1" style="position: absolute; z-index: 9999; background: #FFFFFF; font-size: 12pt; box-shadow: 0px 0px 2px 0px #000000; padding: 2em; display: none;">
    <table style="width: 100%;text-align:center;" cellspacing="0" cellpadding="0">
        <tr>
            <td id="tdNormalExport" class="Color1 TableCellExportType" style="cursor: pointer; width: 150px; height: 100px;">
                <wu:Label ID="lblNormalExport" runat="server" Name="NormalExport"></wu:Label>
            </td>
            <td id="tdSplitExport" class="Color1 TableCellExportType" style="cursor: pointer; width: 150px; height: 100px;">
                <wu:Label ID="lblSplitExport" runat="server" Name="SplitExport"></wu:Label>
            </td>
        </tr>
    </table>
</div>
    <script type="text/javascript">
        function isNumberWithDot(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46) {
                return false;
            }
            return true;
        }
        function isNumber(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }
        function validnum(a) {
            if ((a.value < 0 || a.value > 9 && a.className == "decimal") || (a.value < 0) || a.value == "") {
                return false;
            }
            else
                return true;
        }
        function rangeVaid(id) {

            if (id.value < 45)
                id.value = 45
        }
        var isFolderSelectedToMove = false;
        function ConfirmManualMove() {
            debugger
            if (!isFolderSelectedToMove) {
                ShowMessage(LoadLanguageText("DestinationDirectorySaveError"), "Error");
                return false;
            }
            else {

                if (!isFolderSelectedToMove) {
                    ShowMessage(LoadLanguageText("DestinationDirectorySaveError"), "Error");
                    return false;
                }
                else {
                    isFolderSelectedToMove = false;
                    return true;
                }
            }

        }
        function validate() {

            var elements = document.getElementsByClassName('selectedNode');
            while (elements.length > 0) {
                elements[0].classList.remove('selectedNode');
            }

            if (ConfirmManualMove()) {
                var val = document.getElementById("<%=txtSaveTabName.ClientID %>").value;
                if (val.trim() == "") {
                    document.getElementById("cphContent_txtSaveTabName").style.borderColor = "red";
                    return false;
                }
                else {
                    document.getElementById("cphContent_txtSaveTabName").style.borderColor = "";
                    if (document.getElementById('chkSaveTabAllTabs').checked) {
                        SaveAllTabs(
                            document.getElementById('cphContent_txtSaveTabName').value,
                            document.getElementById('chkSaveAllowOverwrite').checked
                        );
                    }
                    else {
                        SaveTab(
                            document.getElementById('cphContent_txtSaveTabName').value,
                            document.getElementById('chkSaveAllowOverwrite').checked
                        );
                    }

                    CloseBox('boxSaveControl');
                    return false;
                }
            } else {
                return false;
            }
        }
        function ExportReport() {
            var box = document.getElementById("pnlExport");

            box.style.display = "";

            box.style.left = ((window.innerWidth / 2) - (box.offsetWidth / 2)) + "px";
            box.style.top = ((window.innerHeight / 2) - (box.offsetHeight / 2)) + "px";

            document.getElementById("tdNormalExport").onclick = function () {
                document.getElementById("pnlExport").style.display = "none";
                if (document.getElementById("cphContent_pnlReportTabs").getElementsByClassName("ReportTab").length > 1) {
                    CreateConfirmBox(
                        LoadLanguageText('ExportAllTabs').replace('{0}', name),
                        function () {
                            ExportAllTabs();
                        }, function () {
                            ExportTable();
                        });
                }
                else {
                    ExportTable();
                }
            };

            document.getElementById("tdSplitExport").onclick = function () {
                document.getElementById("pnlExport").style.display = "none";
                EnableFilterCategorySearchForExport();
            };
            /*if (document.getElementById("cphContent_pnlReportTabs").getElementsByClassName("ReportTab").length > 1) {
                CreateConfirmBox(
                    LoadLanguageText('ExportAllTabs').replace('{0}', name),
                    function () {
                        ExportAllTabs();
                    }, function () {
                        ExportTable();
                    });
            }
            else {
                ExportTable();
            }*/

        }

        loadFunctions.push(function () {
            var pnlRightPanelContainer = document.getElementById("pnlRightPanelContainer");

            pnlRightPanelContainer.style.top = ((window.innerHeight / 2) - (pnlRightPanelContainer.offsetHeight / 2)) + "px";
        });

        //$(".FileExplorerItemManual").click(function () {            
        //    var elements = document.getElementsByClassName('selectedNode');
        //    while (elements.length > 0) {
        //        elements[0].classList.remove('selectedNode');
        //    }            
        //    this.classList.add('selectedNode');
        //});
    </script>
    <script type="text/javascript">
        var FileExplorerItemManual = false;
        function ShowSaveReportLocation() {
            AjaxRequest("OverwriteAllowed", "", function (response) {

                if (response != "True") {
                    //document.getElementById("pnlSaveReportLocationSelector_Overwrite").style.display = "none";
                    ConfirmSaveReportLocation('New');
                    return;
                }

                var box = document.getElementById("pnlSaveReportLocation");

                box.style.display = "";

                var elements = box.getElementsByTagName("td");
                for (var i = 0; i < elements.length; i++) {
                    elements.item(i).className = "Color1";
                    elements.item(i).style.color = "";
                }

                box.style.left = ((window.innerWidth / 2) - (box.offsetWidth / 2)) + "px";
                box.style.top = ((window.innerHeight / 2) - (box.offsetHeight / 2)) + "px";
            });
        }
        function HideSaveReportLocation() {
            document.getElementById("pnlSaveReportLocationBackground").style.display = "none";
            document.getElementById("pnlSaveReportLocation").style.display = "none";
        }
        function ManualSaveReportFolderSelect(sender, path) {
            isFolderSelectedToMove = true;
            _AjaxRequest("/Handlers/GlobalHandler.ashx", "ManualSaveReportFolderSelect", "destination=" + encodeURI(path), function (response) {
            });
        }
        function ChangeSaveReportLocation(selection) {
            var selector = document.getElementById("pnlSaveReportLocationSelector_" + selection);

            var elements = selector.parentNode.getElementsByTagName("td");
            for (var i = 0; i < elements.length; i++) {
                elements.item(i).className = "Color1";
                elements.item(i).style.color = "";
            }

            selector.className = "BackgroundColor1";
            selector.style.color = "#FFFFFF";
        }

        function ConfirmSaveReportLocation(selection) {
            var selector = document.getElementById("pnlSaveReportLocationSelector_" + selection);

            var elements = selector.parentNode.getElementsByTagName("td");
            for (var i = 0; i < elements.length; i++) {
                elements.item(i).className = "Color1";
                elements.item(i).style.color = "";
            }

            selector.className = "BackgroundColor1";
            selector.style.color = "#FFFFFF";

            if (selection == "Overwrite") {
                SaveExisting();

                window.setTimeout(HideSaveReportLocation, 500);
            }
            else {
                HideSaveReportLocation();
                window.setTimeout(function () {
                    InitDragBox('boxSaveControl', 'Center');

                }, 500);
            }
        }

        function clearCheckBox(id) {
            document.getElementById('cphContent_' + id).checked = false;
            document.getElementById('cphContent_' + id).previousSibling.onmouseover();
        }
        function clearAllCheckBox() {
            var controls = document.querySelectorAll("[id*='cphContent_chkAll']");
            for (var i = 0; i < controls.length; i++) {
                if (document.getElementById(controls[i].id) != null) {
                    document.getElementById(controls[i].id).checked = false;
                    document.getElementById(controls[i].id).previousSibling.onmouseover();
                }
            }
        }
        var callCounts = 0;
        var responseCounts = 0;
        function EnableConfirm() {
            if (callCounts == responseCounts)
                document.getElementById("btnConfirmFilterCategorySearch").style.pointerEvents = "";
            else
                document.getElementById("btnConfirmFilterCategorySearch").style.pointerEvents = "none";
        }
    </script>
<div id="pnlSaveReportLocationBackground" class="BoxBackground" style="display:none;"></div>
<div id="pnlSaveReportLocation" class="Color1" style="position:absolute;z-index: 9999;background:#FFFFFF;font-size:12pt;box-shadow:0px 0px 2px 0px #444444;display:none;">
    <table cellspacing="0" cellpadding="0">
        <tr style="vertical-align:middle;text-align:center;">
            <td id="pnlSaveReportLocationSelector_Overwrite" onclick="ConfirmSaveReportLocation('Overwrite');" class="Color1" style="cursor:pointer;width:150px;height:100px;">
                <wu:Label ID="lblSaveReportLocationOverwrite" runat="server" Name="SaveReportLocationOverwrite"></wu:Label>
            </td>
            <td id="pnlSaveReportLocationSelector_New" onclick="ConfirmSaveReportLocation('New');" class="Color1" style="cursor:pointer;width:150px;height:100px;">
                <wu:Label ID="lblSaveReportLocationNew" runat="server" Name="SaveReportLocationNew"></wu:Label>
            </td>
        </tr>
        <!--<tr>
            <td colspan="2" class="GreenBackground3" style="cursor:pointer;color:#FFFFFF;text-align:center;padding:5px;" onclick="ConfirmSaveReportLocation();">
                <wu:Label ID="lblSaveReportLocationConfirm" runat="server" Name="Save"></wu:Label>
            </td>
        </tr>-->
    </table>
</div>
    <wu:Box ID="boxSave" Position="Center" runat="server" Title="Save" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true">
        <table>
            <tr>
                <td>
                    <wu:Label ID="lblSaveTabName" runat="server" Name="name"></wu:Label>
                </td>
                <td>
                    <wu:TextBox ID="txtSaveTabName" runat="server"></wu:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblSaveTabAllTabs" runat="server" Name="SaveAllTabs"></wu:Label>
                </td>
                <td>
                    <input type="checkbox" id="chkSaveTabAllTabs" checked="checked" />
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblSaveAllowOverwrite" runat="server" Name="SaveAllowOverwrite"></wu:Label>
                </td>
                <td>
                    <input type="checkbox" id="chkSaveAllowOverwrite" />
                </td>
            </tr>
              <tr>
                    <td class="folderContents" colspan="2">
                        <div id="folderTitle" style="max-height:70px;max-width:350px;overflow: hidden; text-overflow: ellipsis;" ;="" class="DirectoryName" title="folder">Save to folder</div>
                             <asp:TreeView runat="server" ID="TreeView1">    
                     </asp:TreeView>
                        </td>
            </tr>
            <tr>
                <td colspan="2" align="right" >
                    <wu:Button ID="btnSaveTabConfirm" class="btnSaveTabConfirm" runat="server" Name="Save" OnClientClick=" return validate(); " />
                </td>
            </tr>
        </table>
    </wu:Box>


    <wu:Box ID="boxCombineCategoires" Position="Center" runat="server" Title="CombineCategoires" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true">
        
    </wu:Box>

    <wu:Box ID="boxFilterDefinition" runat="server" Title="Filters" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true" OnClientClose="document.body.focus();if(!filtersChanged) return; if(document.getElementById('chkFilterDefinitionAllTabs').checked) { PropagateFilterDefinition(PopulateCrosstable); } else {  window.setTimeout(PopulateCrosstable, 500); } filtersChanged = false;">
        <asp:Panel ID="pnlFilterCategories" runat="server" style="width:800px;margin:2em;" CssClass="BoxFilter"></asp:Panel>
        <table id="pnlFilterDefinitionAllTabs" runat="server" style="">
            <tr>
                <td>
                    <input type="checkbox" id="chkFilterDefinitionAllTabs" onclick="filtersChanged = true;" />
                </td>
                <td>
                    <wu:Label ID="lblFilterDefinitionAllTabs" runat="server" Name="FilterDefinitionAllTabs"></wu:Label>
                </td>
            </tr>
        </table>
        <wu:TipGallery ID="tgFilterDefinition" runat="server" _TipItems="FilterDefinitionTip1, FilterDefinitionTip2" />
    </wu:Box>
    
    <uc1:EquationDefinition runat="server" id="EquationDefinition" />
    
    <wu:Box ID="boxFilterSearch" runat="server" Title="SearchFilter" TitleLanguageLabel="true" Position="Top" JavascriptTriggered="true" Dragable="true" OnClientClose="document.body.focus();">
        <wu:CategorySearch ID="csFilterDefinition" runat="server" SelectionType="Multi" style="width:1000px;">
        </wu:CategorySearch>
        <div id="btnCancelFilterCategorySearch" class="CancelCircle" onclick="document.getElementById('cphContent_csFilterDefinition').Close(false);" style="position:absolute;">
        </div>
        <div id="btnConfirmFilterCategorySearch" class="OkCircle" onclick="document.getElementById('cphContent_csFilterDefinition').Close(true);" style="position:absolute;margin-left: 1006px;">
        </div>
    </wu:Box>

    <div id="pnlWeightingDefinition" style="width:0px;height:0px;" class="VariableSearch PnlVariableSelectors BackgroundColor7">
        <asp:Panel ID="pnlWeightingDefinitionContainer" runat="server" style="margin:2em;"></asp:Panel>
        <img src="/Images/Icons/HideVariableSelection.png" style="float:right;cursor:pointer;" onclick="PopulateCrosstableDelayed(500);HideFullScreenPanel('pnlWeightingDefinition');" />
    </div>
    
    <uc1:VariableSearch runat="server" id="VariableSearch" />
    <wu:HoverBox ID="hbReporterViews" runat="server" Display="RightPanel" TriggerMode="Click" IdTrigger="pnlRightPanelView" style="margin-right:110px;z-index: 1010;">
        <asp:Panel ID="pnlReporterViews" runat="server"></asp:Panel>
    </wu:HoverBox>
        
    <wu:Box ID="boxNewScoreGroup" runat="server" Dragable="true">
        <table>
            <tr>
                <td>
                    <wu:Label ID="lblNewScoreGroupName" runat="server" Name="Name"></wu:Label>
                </td>
                <td>
                    <wu:TextBox ID="txtNewScoreGroupName" runat="server"></wu:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <wu:Button ID="btnNewScoreGroupConfirm" runat="server" Name="Confirm" OnClick="btnNewScoreGroupConfirm_Click"></wu:Button>
                    <wu:Button ID="btnNewScoreGroupCancel" runat="server" Name="Cancel"></wu:Button>
                </td>
            </tr>
        </table>
    </wu:Box>
    
    <asp:Panel ID="pnl" runat="server" CssClass="ReportContainer"></asp:Panel>

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

    <asp:Panel ID="pnlWorkflowContainer" runat="server">
        <div id="WorkflowBackground" class="BoxBackground" style="opacity:0;display:none;"></div>
        <div id="WorkflowContainer" class="WorkflowContainer">
            <img class="BtnWorkflowPin BackgroundColor6" src="/Images/Icons/Pin.png" onclick="AjaxRequest('PinBottomBar', '', function (response) {window.location=window.location});" />
            <div class="WorkflowSelectorSection" style="position:absolute;display:none; background: #2f6cd0;color: #fff;float: right;width: 110px;height:20px"><table><tr><td><input type ="checkbox" id="chkWorkflowAllTabs" ></td><td>Apply all tabs</td></tr></table></div>
            <div class="BtnWorkflowShowPreview" onclick="ShowWorkflowSelectionDetail(this.getElementsByTagName('img').item(0));">
                <img class="BackgroundColor6" src="/Images/Icons/Expand.png" alt="Expand" onmouseover="ShowWorkflowOnHover(this);" onmouseout="ShowWorkflowOnOut(this)" />
                
            </div>
            <asp:Panel ID="pnlWorkflow" runat="server" style=""></asp:Panel>
            <div style="clear:both"></div>
        </div>
    </asp:Panel>

    <uc2:HierarchySelector runat="server" id="HierarchySelector" />
    <uc1:ConnectPowerBI runat="server" id="ConnectPowerBI" />
</asp:content>

<asp:content id="Content3" contentplaceholderid="cphFooter" runat="server">
</asp:content>
