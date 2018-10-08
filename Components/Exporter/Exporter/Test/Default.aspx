<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Test.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style id="style" type="text/css" runat="server">
        /* ##### Color definitions ##### */


        .Color1, .Color1 a {
            color: #6CAEE0;
        }

        .Color2, .Color2 a {
            color: #FCB040;
        }

        .BorderColor1 {
            border-color: #6CAEE0;
        }

        .BorderColor7 {
            border-color: #E1EEF8;
        }

        /* Main color */
        .BackgroundColor1, .BackgroundColor3, .BackgroundColor6 {
            background-color: #6CAEE0;
        }

        .BackgroundColor_1 {
            background-color: rgb(151,175,176);
        }

        /* Standard version of the secondary color. */
        .BackgroundColor2 {
            background: #FCB040;
        }

        /* Light version of the secondary color. */
        .BackgroundColor20 {
            background: #FFCA49;
        }

        /* Very light version of the secondary color. */
        .BackgroundColor21 {
            background: #FFD34C;
        }

        /* Main color with mouse over of secondary color. */
        .BackgroundColor3:hover {
            background: #FCB040;
        }

        /* Mouse over with main color. */
        .BackgroundColor4:hover {
            background: #6CAEE0;
            color: #FFFFFF;
        }

        /* Light version of main color. */
        .BackgroundColor5, .BackgroundColor6:hover {
            background: #97C5E8;
        }

        /* Very very light version of the main color. */
        .BackgroundColor7 {
            background: #E1EEF8;
        }

        /* Gray-ish version for non selected workflow */
        .BackgroundColor8 {
            background: #97AFB0;
        }

        /* Very light version of the main color. */
        .BackgroundColor9 {
            background: #B6D7EF;
        }


        body {
            scrollbar-face-color: #FCB040;
            scrollbar-shadow-color: #FCB040;
            scrollbar-highlight-color: #FCB040;
            scrollbar-3dlight-color: #FCB040;
            scrollbar-darkshadow-color: #FFFFFF;
            scrollbar-track-color: #FFFFFF;
            scrollbar-arrow-color: #6CAEE0;
        }


        .GridHeadlineItem {
            color: #6CAEE0;
        }

        .GridHeadlineItemSearch {
            background: #6CAEE0;
        }

        .GridRow_Active .GridRowItem {
            background: #FCB040;
        }


        input[type=button], input[type=submit] {
            background: #6CAEE0;
        }

            input[type=button]:hover, input[type=submit]:hover {
                background: #FCB040;
            }

        .NavigationItem:hover {
            background: #97C5E8;
        }

        .CrosstableContainer {
            min-width: 800px;
            min-height: 400px;
        }

            .CrosstableContainer td {
                padding: 0px;
            }

        .TableCellHeadlineCategory {
            padding: 2px !important;
        }

        .Crosstable {
            border-right: 4px solid #FFFFFF;
            border-bottom: 4px solid #FFFFFF;
        }

            .Crosstable td {
                padding: 5px;
            }

            .Crosstable .TableCellRootTopVariable {
                height: 71px;
            }

            .Crosstable .TableCellHeadlineTopVariable {
                padding-left: 0px;
                padding-right: 0px;
                padding-bottom: 0px;
            }

            .Crosstable .TableCellHeadlineNestedTopVariable {
                color: #FFFFFF;
                border-left-width: 2px;
                border-left-style: solid;
                border-right-width: 2px;
                border-right-style: solid;
            }

            .Crosstable .TableCellHeadlineNestedLeftVariable {
                min-width: 84px;
                max-width: 84px;
                width: 84px;
                color: #FFFFFF;
                border-top-width: 2px;
                border-top-style: solid;
                border-bottom-width: 2px;
                border-bottom-style: solid;
            }

        .NestedVariableSelector {
            color: #FFFFFF;
            border-top-width: 2px;
            border-top-style: solid;
        }

        .TableCellFilter {
            font-size: 10pt;
            height: 76px;
            padding: 0px !important;
        }

        .TableCellHeadlineLeftVariable,
        .TableCellFilter {
            width: 200px !important;
            min-width: 200px !important;
            max-width: 200px !important;
        }

        .PanelFilter {
            text-align: center;
            /*box-shadow:0px 0px 5px 0px #000000;*/
            color: #FFFFFF;
            padding-top: 20px;
            padding-bottom: 20px;
            padding-left: 30px;
            padding-right: 30px;
            cursor: pointer;
        }

        .TableCellHeadline {
            font-size: 10pt;
            text-align: center;
        }

        .Crosstable .TableCellHeadlineBase, .Crosstable .TableCellHeadlineSigDiff {
            border-style: solid;
            border-width: 2px;
            width: 100px;
            color: #FFFFFF;
            font-weight: bold;
        }

        .Crosstable .TableCellHeadlineCategory {
            border-style: solid;
            border-width: 2px;
            /*border-bottom-width:3px;*/
            min-width: 100px;
            max-width: 100px;
            width: 100px;
            height: 50px;
            min-height: 50px;
            max-height: 50px;
            overflow: hidden;
            color: #FFFFFF;
            font-weight: bold;
        }

        .TableCellHeadlineWeightingInformation {
            border-width: 1px;
            border-style: solid;
            color: #FFFFFF;
            cursor: pointer;
        }

        .Crosstable .TableCellHeadline:hover .ImageDeleteVariable {
            visibility: visible;
        }

        .Crosstable .TableCellHeadlineSelectNew {
            width: 200px;
        }

        .Crosstable .ImageDeleteVariable {
            visibility: hidden;
            float: right;
            cursor: pointer;
        }

        .Crosstable .TableCellValue {
            color: #000000;
            text-align: center;
            font-weight: bold;
            font-size: 9pt;
            border-style: solid;
            border-width: 2px;
        }

        .Crosstable .TableCellPercentage, .Crosstable .TableCellSigDiff {
            color: #000000;
            text-align: center;
            font-weight: bold;
            font-size: 9pt;
            border-style: solid;
            border-width: 2px;
        }

        .Crosstable .TableCellHeadline {
        }

        .Crosstable .TableCellVariableSpacerTop {
            border-left-color: #FFFFFF;
            border-left-width: 4px;
            border-left-style: solid;
        }

        .Crosstable .TableCellVariableSpacerLeft {
            border-top-color: #FFFFFF;
            border-top-width: 4px;
            border-top-style: solid;
        }

        .Crosstable select {
            width: 200px;
            border: none;
        }

        .CrosstableContainer input[type=text],
        .CrosstableContainer input[type=text]:active,
        .CrosstableContainer input[type=text]:focus {
            padding: 6px;
            border-top-left-radius: 3px;
            border-bottom-left-radius: 3px;
            border-width: 2px;
            border-style: solid;
            outline: 0;
        }


        .VariableLabel, .TableCellHeadlineCategory, .TableCellHeadlineWeightingInformation {
            text-align: center;
        }

        .VariableSelection {
            width: 100%;
        }

        .VariableSelectionLeft {
            width: 200px !important;
        }

        .HoverBoxVariableSelection {
            border-radius: 3px;
            background: #FFFFFF;
            max-width: 300px;
            width: 300px;
            border-style: solid;
            border-width: 2px;
        }

        .VariableSelectionCategory {
            padding: 10px;
            border-bottom: 1px solid #FFFFFF;
        }

        .HoverBoxVariableSelectionCategories {
            color: #FFFFFF;
            min-width: 200px;
            max-width: 200px;
            width: 200px;
            cursor: pointer;
            text-align: center;
        }

        .VariableSelectionItem {
            padding-left: 20px;
            padding-right: 20px;
            padding-top: 10px;
            padding-bottom: 10px;
            cursor: pointer;
            border-bottom: 1px solid #cccccc;
        }

        .VariableSelectionCategoryNoData,
        .VariableSelectionItemNoData:hover {
            background: #FF0000 !important;
        }

        .LeftPanelTrigger {
            position: absolute;
            width: 50px;
            height: 25%;
        }

        .LeftPanel {
            color: #FFFFFF;
            font-size: 16pt;
        }

            .LeftPanel a {
                color: #FFFFFF;
                text-decoration: none;
            }

        .LeftPanelSection {
            cursor: pointer;
            padding-top: 5px;
            padding-bottom: 5px;
            padding-left: 10px;
            padding-right: 10px;
            border-top: 1px solid #FFFFFF;
        }

        .AdvancedFilters td {
        }

        .AdvancedFilters a, .WeightingFilters a {
            color: #FFFFFF;
            font-weight: bold;
            text-decoration: none;
            cursor: pointer;
        }

        .AdvancedFilters, .WeightingFilters {
            position: absolute;
            padding: 1em;
            color: #FFFFFF;
        }

        .AdvancedFilters {
            width: 570px;
        }

            .AdvancedFilters table, .WeightingFilters table {
                width: 100%;
            }

        .AdvancedFiltersTableCellLabel {
            background: #FFFFFF;
            color: #000000;
            padding: 5px;
            width: 300px;
            font-weight: bold;
        }

        .DragableCategory {
            color: #FFFFFF;
            padding-top: 5px;
            padding-bottom: 5px;
            padding-left: 10px;
            padding-right: 10px;
        }

        .TableCellDescription {
            height: 50px;
        }

        .TableCellDescriptionLeft {
            width: 50px;
        }

        .BlurTable {
            position: absolute;
            background: #FFFFFF;
            opacity: 0.7;
            margin-top: -5px;
            margin-left: -5px;
        }

        .TextBoxVariableSelector {
            width: 95%;
            text-align: center;
            cursor: pointer;
        }


        .TableCellHeadlineNewTopVariable {
            padding-top: 27px;
        }


        .TableVariableSelector {
            height: 100%;
            width: 100%;
        }

            .TableVariableSelector td {
                padding: 0px;
            }
    </style>


</head>
<body>

    <form id="form1" runat="server">


        <div id="tableDiv">
            <asp:Table runat="server" ID="table" runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="CrosstableContainer">
                 
                    <asp:TableRow>
                        <asp:TableCell></asp:TableCell>
                        <asp:TableCell colspan="3" class="TableCellDescription">
                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="width: 100%">
                                 
                                    <asp:TableRow>
                                        <asp:TableCell align="center" style="width: 200px;">
                                            <h2 class="Color1">filter</h2>
                                        </asp:TableCell>
                                        <asp:TableCell align="center">
                                            <h2 class="Color1">top</h2>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                 
                            </asp:Table>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow valign="top">
                        <asp:TableCell valign="middle" rowspan="2" class="TableCellDescriptionLeft">
                            <h2 class="Color1">side</h2>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="Crosstable">
                                 
                                    <asp:TableRow>
                                        <asp:TableCell valign="top" class="TableCellFilter BackgroundColor7">
                                            <div style="display: none;" class="AdvancedFilters BackgroundColor2" id="cphContent_pnlFilters">
                                                <asp:Table runat="server" cellspacing="5" cellpadding="0">
                                                     
                                                        <asp:TableRow>
                                                            <asp:TableCell align="right" colspan="4"><a onclick="ReloadCrosstable();">Done</a></asp:TableCell>
                                                        </asp:TableRow>
                                                     
                                                </asp:Table>
                                            </div>
                                            <div onclick="document.getElementById('cphContent_pnlFilters').style.display = '';" onmouseover="OverFilter();" class="PanelFilter BackgroundColor2" id="cphContent_pnlFilter">
                                                <span>
                                                    <div style="margin-top: 9px; margin-bottom: 9px;">no filters applied</div>
                                                </span>
                                            </div>
                                        </asp:TableCell>
                                        <asp:TableCell colspan="18" class="TableCellHeadline TableCellHeadlineTopVariable TableCellRootTopVariable Color1 BackgroundColor7">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]" idselected="86c921a6-62b8-4f79-b262-a576eeb2f1e5" class="VariableSelection " id="cphContent_ddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51' , 'ddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51', this, 'cphContent_hbCategoriesddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51" onclick="ShowHoverBox('cphContent_hbDropDownddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51', this.id, 'TopLeft','onclick', false, 0, undefined);null">DET Identifier Variable</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51','cphContent_hbDropDownddlTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7 BackgroundColor2">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51' , 'ddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51', this, 'cphContent_hbCategoriesddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51','cphContent_hbDropDownddlNewNestedTopVariable86c921a6-62b8-4f79-b262-a576eeb2f1e51');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell rowspan="5" class="BackgroundColor7"></asp:TableCell>
                                        <asp:TableCell style="width: 95.5px; min-width: 95.5px;" rowspan="5" colspan="3" onclick="ShowWeightingFilters();" class="TableCellHeadline TableCellHeadlineWeightingInformation BorderColor7 BackgroundColor1">no weighting<br>
                                            applied</asp:TableCell>
                                        <asp:TableCell rowspan="2" colspan="3" class="TableCellHeadline TableCellHeadlineBase BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="height: 50px;" rowspan="2" colspan="3" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">
                                            <div style="overflow: hidden; max-height: 40px;">Cloud</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" rowspan="2" colspan="3" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">
                                            <div style="overflow: hidden; max-height: 40px;">Core</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" rowspan="2" colspan="3" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">
                                            <div style="overflow: hidden; max-height: 40px;">Office</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" rowspan="2" colspan="3" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">
                                            <div style="overflow: hidden; max-height: 40px;">Windows Apps</div>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell style="height: 50px;" colspan="3" class="TableCellHeadline TableCellHeadlineTopVariable BackgroundColor20 BorderColor7 TableCellHeadlineNestedTopVariable">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]');" class="ImageDeleteVariable" /></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]" idselected="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" class="VariableSelection " id="cphContent_ddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51' , 'ddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51', this, 'cphContent_hbCategoriesddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51" onclick="ShowHoverBox('cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51', this.id, 'TopLeft','onclick', false, 0, undefined);null">Core Qualification - AW</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51','cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51' , 'ddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51', this, 'cphContent_hbCategoriesddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51','cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e51');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="3" class="TableCellHeadline TableCellHeadlineTopVariable BackgroundColor21 BorderColor7 TableCellHeadlineNestedTopVariable">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]" idselected="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" class="VariableSelection " id="cphContent_ddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52' , 'ddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52', this, 'cphContent_hbCategoriesddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52" onclick="ShowHoverBox('cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52', this.id, 'TopLeft','onclick', false, 0, undefined);null">Core Qualification - AW</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52','cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52' , 'ddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52', this, 'cphContent_hbCategoriesddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52','cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e52');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="3" class="TableCellHeadline TableCellHeadlineTopVariable BackgroundColor20 BorderColor7 TableCellHeadlineNestedTopVariable">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]" idselected="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" class="VariableSelection " id="cphContent_ddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53' , 'ddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53', this, 'cphContent_hbCategoriesddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53" onclick="ShowHoverBox('cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53', this.id, 'TopLeft','onclick', false, 0, undefined);null">Core Qualification - AW</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53','cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53' , 'ddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53', this, 'cphContent_hbCategoriesddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53','cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e53');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="3" class="TableCellHeadline TableCellHeadlineTopVariable BackgroundColor21 BorderColor7 TableCellHeadlineNestedTopVariable">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]" idselected="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" class="VariableSelection " id="cphContent_ddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54' , 'ddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54', this, 'cphContent_hbCategoriesddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54" onclick="ShowHoverBox('cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54', this.id, 'TopLeft','onclick', false, 0, undefined);null">Core Qualification - AW</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54','cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54' , 'ddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54', this, 'cphContent_hbCategoriesddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54','cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e54');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="3" class="TableCellHeadline TableCellHeadlineTopVariable BackgroundColor20 BorderColor7 TableCellHeadlineNestedTopVariable">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]" idselected="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" class="VariableSelection " id="cphContent_ddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55' , 'ddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55', this, 'cphContent_hbCategoriesddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55" onclick="ShowHoverBox('cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55', this.id, 'TopLeft','onclick', false, 0, undefined);null">Core Qualification - AW</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55','cphContent_hbDropDownddlTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;86c921a6-62b8-4f79-b262-a576eeb2f1e5&quot;]/Variable[@Id=&quot;0ddbb568-b6bc-4b54-b273-f00307b1b5a7&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55' , 'ddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55', this, 'cphContent_hbCategoriesddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55','cphContent_hbDropDownddlNewNestedTopVariable0ddbb568-b6bc-4b54-b273-f00307b1b5a786c921a6-62b8-4f79-b262-a576eeb2f1e55');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idvariable="89acb66e-223f-4161-a760-1b60299ce500" istitle="True">
                                        <asp:TableCell rowspan="2" colspan="1" class="TableCellHeadline TableCellHeadlineBase BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">
                                            <div style="overflow: hidden; max-height: 40px;">No</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">
                                            <div style="overflow: hidden; max-height: 40px;">Yes</div>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="2" colspan="1" class="TableCellHeadline TableCellHeadlineBase BorderColor7 BackgroundColor1">Base</asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">
                                            <div style="overflow: hidden; max-height: 40px;">No</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">
                                            <div style="overflow: hidden; max-height: 40px;">Yes</div>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="2" colspan="1" class="TableCellHeadline TableCellHeadlineBase BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">
                                            <div style="overflow: hidden; max-height: 40px;">No</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">
                                            <div style="overflow: hidden; max-height: 40px;">Yes</div>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="2" colspan="1" class="TableCellHeadline TableCellHeadlineBase BorderColor7 BackgroundColor1">Base</asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">
                                            <div style="overflow: hidden; max-height: 40px;">No</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">
                                            <div style="overflow: hidden; max-height: 40px;">Yes</div>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="2" colspan="1" class="TableCellHeadline TableCellHeadlineBase BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">
                                            <div style="overflow: hidden; max-height: 40px;">No</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 50px;" colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">
                                            <div style="overflow: hidden; max-height: 40px;">Yes</div>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idvariable="89acb66e-223f-4161-a760-1b60299ce500" istitle="True">
                                        <asp:TableCell colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor1">A</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor5">B</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor5">A</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor1">B</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor1">A</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor5">B</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor5">A</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor1">B</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor1">A</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor5">B</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053" istitle="True">
                                        <asp:TableCell rowspan="90" class="TableCellHeadline TableCellHeadlineLeftVariable Color1 BackgroundColor7">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]" idselected="89acb66e-223f-4161-a760-1b60299ce500" class="VariableSelection " id="cphContent_ddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001' , 'ddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001', this, 'cphContent_hbCategoriesddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001" onclick="ShowHoverBox('cphContent_hbDropDownddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001', this.id, 'TopLeft','onclick', false, 0, undefined);null">Respondents Gender</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001','cphContent_hbDropDownddlLeftVariable89acb66e-223f-4161-a760-1b60299ce5001');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7 BackgroundColor2">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001' , 'ddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001', this, 'cphContent_hbCategoriesddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001','cphContent_hbDropDownddlNewNestedTopVariable89acb66e-223f-4161-a760-1b60299ce5001');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="30" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell rowspan="30" class="TableCellHeadline BackgroundColor20 TableCellHeadlineNestedLeftVariable BorderColor7">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" idselected="e0e50456-9f7f-4038-8402-24e05e1ab053" class="VariableSelection " id="cphContent_ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001' , 'ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001', this, 'cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001" onclick="ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001', this.id, 'TopLeft','onclick', false, 0, undefined);null">How well each of the following attributes describes BRAND Innovative:  Oracle</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001','cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001' , 'ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001', this, 'cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001','cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5001');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="3" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">11108</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">2841</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">8267</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor5">850</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">255</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">595</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">9202</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">2307</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">6895</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor5">184</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">184</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">872</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">279</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">593</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">2</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">68</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">22</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">46</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">8</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">6</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">54</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">18</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">36</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">156</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">36</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">120</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">6</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">136</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">32</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">104</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">7</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">4</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">378</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">99</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">279</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">22</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">13</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">9</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">324</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">78</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">246</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">30</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">8</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">22</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', 'B,A');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">B</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', 'A,B');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">A</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">5&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">4&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1136</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">283</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">853</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">66</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">21</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">45</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">965</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">236</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">729</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">20</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">20</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">85</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">26</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">59</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">8&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">8&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">8&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">11&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">11&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">11&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">9&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">6</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1715</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">454</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1261</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">101</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">33</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">68</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1473</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">377</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1096</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">20</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">20</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">121</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">44</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">77</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">15&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">15&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">12&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">13&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">11&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">11&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">11&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">14&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">13&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2745</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">700</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">2045</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">189</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">64</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">125</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2297</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">572</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1725</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">42</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">42</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">217</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">64</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">153</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">22&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">21&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">23&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">23&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">23&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">26&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">8</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3013</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">761</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">2252</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">260</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">69</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">191</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2427</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">599</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1828</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">62</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">62</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">264</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">93</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">171</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">31&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">32&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">34&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">34&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">30&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">33&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">29&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Describes Completely</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1840</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">476</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1364</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">193</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">52</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">141</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1477</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">386</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1091</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">32</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">32</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">138</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">38</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">100</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">23&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">20&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">24&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">14&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">17&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">Does not describe at all</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">57</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">10</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">47</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">4</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">4</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">49</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">9</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">40</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">4</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053" istitle="True">
                                        <asp:TableCell style="width: 100px;" rowspan="30" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">Female</asp:TableCell>
                                        <asp:TableCell rowspan="30" class="TableCellHeadline BackgroundColor21 TableCellHeadlineNestedLeftVariable BorderColor7">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" idselected="e0e50456-9f7f-4038-8402-24e05e1ab053" class="VariableSelection " id="cphContent_ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002' , 'ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002', this, 'cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002" onclick="ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002', this.id, 'TopLeft','onclick', false, 0, undefined);null">How well each of the following attributes describes BRAND Innovative:  Oracle</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002','cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002' , 'ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002', this, 'cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002','cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5002');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="3" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">2781</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">738</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">2043</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor5">243</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">72</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">171</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">2249</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">591</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1658</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor5">57</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">57</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">232</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">75</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">157</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">2</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">18</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">13</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">13</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">4</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">9</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">32</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">11</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">21</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">27</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">10</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">17</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">4</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">89</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">30</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">59</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">4</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">72</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">24</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">48</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">7</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">4&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">309</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">74</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">235</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">24</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">6</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">18</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">258</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">62</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">196</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">22</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">6</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">16</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">11&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">12&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">8&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">11&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">11&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">12&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">9&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">9&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">9&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">8&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">6</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">447</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">127</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">320</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">34</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">10</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">24</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">368</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">97</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">271</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">8</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">8</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">37</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">20</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">17</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', 'B,A');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">B</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', 'A,B');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">A</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">14&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">14&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">14&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">14&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">14&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">11&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">651</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">172</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">479</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">52</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">19</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">33</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">534</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">143</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">391</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">11</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">11</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">54</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">10</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">44</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', 'B,A');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">B</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', 'A,B');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">A</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">23&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">23&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">23&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">21&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">19&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">24&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">24&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">24&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">19&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">19&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">23&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">13&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">28&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">8</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">713</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">189</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">524</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">66</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">20</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">46</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">569</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">148</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">421</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">18</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">18</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">60</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">21</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">39</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">28&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">32&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">32&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">28&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Describes Completely</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">511</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">127</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">384</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">58</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">14</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">44</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">398</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">101</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">297</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">12</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">12</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">43</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">12</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">31</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">18&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">19&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">24&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">19&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">18&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">18&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">21&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">21&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">19&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">20&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">Does not describe at all</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">11</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">8</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">8</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053" istitle="True">
                                        <asp:TableCell style="width: 100px;" rowspan="30" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Male</asp:TableCell>
                                        <asp:TableCell rowspan="30" class="TableCellHeadline BackgroundColor20 TableCellHeadlineNestedLeftVariable BorderColor7">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" idselected="e0e50456-9f7f-4038-8402-24e05e1ab053" class="VariableSelection " id="cphContent_ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003' , 'ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003', this, 'cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003" onclick="ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003', this.id, 'TopLeft','onclick', false, 0, undefined);null">How well each of the following attributes describes BRAND Innovative:  Oracle</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003','cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003' , 'ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003', this, 'cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003','cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5003');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="3" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">8327</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">2103</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">6224</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor5">607</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">183</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">424</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">6953</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1716</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">5237</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor5">127</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">127</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">640</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">204</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">436</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">2</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">50</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">17</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">33</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">41</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">14</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">27</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">124</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">25</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">99</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">6</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">109</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">22</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">87</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">7</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">5</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">4</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">289</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">69</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">220</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">15</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">10</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">252</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">54</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">198</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">20</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">15</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', 'B,A');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">B</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', 'A,B');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">A</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">5&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">827</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">209</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">618</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">42</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">15</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">27</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">707</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">174</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">533</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">15</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">15</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">63</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">20</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">43</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">7&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">8&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">6&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">12&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">12&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">6</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1268</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">327</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">941</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">67</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">23</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">44</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1105</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">280</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">825</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">12</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">12</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">84</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">24</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">60</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">15&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">15&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">11&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">13&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">9&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">9&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">13&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">12&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">14&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2094</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">528</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1566</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">137</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">45</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">92</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1763</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">429</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1334</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">31</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">31</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">163</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">54</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">109</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">23&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">22&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">24&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">24&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">8</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2300</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">572</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1728</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">194</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">49</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">145</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1858</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">451</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1407</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">44</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">44</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">204</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">72</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">132</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">28&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">28&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">32&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">34&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">27&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">35&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">35&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">32&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">35&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">30&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Describes Completely</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1329</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">349</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">980</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">135</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">38</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">97</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1079</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">285</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">794</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">20</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">20</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">95</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">26</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">69</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">22&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">21&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">23&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">15&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">15&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">13&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">16&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="86c921a6-62b8-4f79-b262-a576eeb2f1e5" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">Does not describe at all</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">46</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">39</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">4</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">4</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">39</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">32</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="ded15056-e696-4c75-bdad-f81a3270cf23" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" idcategory="dbabd20d-d946-48ab-9eeb-a4e8ad694507" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('0ddbb568-b6bc-4b54-b273-f00307b1b5a7', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor5"></asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">0&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="A" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="0ddbb568-b6bc-4b54-b273-f00307b1b5a7" significanceletter="B" class="TableCellValue BorderColor7 BackgroundColor9">1&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                 
                            </asp:Table>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="Crosstable">
                                 
                                    <asp:TableRow>
                                        <asp:TableCell colspan="8" class="TableCellHeadline TableCellHeadlineTopVariable TableCellRootTopVariable Color1 BackgroundColor7">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;30d5a6cb-abd2-4d2e-bd21-93200b87768e&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;30d5a6cb-abd2-4d2e-bd21-93200b87768e&quot;]" idselected="30d5a6cb-abd2-4d2e-bd21-93200b87768e" class="VariableSelection " id="cphContent_ddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1' , 'ddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1', this, 'cphContent_hbCategoriesddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1" onclick="ShowHoverBox('cphContent_hbDropDownddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1', this.id, 'TopLeft','onclick', false, 0, undefined);null">When developing apps for smartphones, tablets, or PCs at work have you evaluated or used Monogame </span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1','cphContent_hbDropDownddlTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7 BackgroundColor2">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Top&quot;]/Variable[@Id=&quot;30d5a6cb-abd2-4d2e-bd21-93200b87768e&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1' , 'ddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1', this, 'cphContent_hbCategoriesddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1','cphContent_hbDropDownddlNewNestedTopVariable30d5a6cb-abd2-4d2e-bd21-93200b87768e1');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idvariable="89acb66e-223f-4161-a760-1b60299ce500" istitle="True">
                                        <asp:TableCell style="width: 95.5px; min-width: 95.5px;" rowspan="2" colspan="3" onclick="ShowWeightingFilters();" class="TableCellHeadline TableCellHeadlineWeightingInformation BorderColor7 BackgroundColor1">no weighting<br>
                                            applied</asp:TableCell>
                                        <asp:TableCell rowspan="2" colspan="1" class="TableCellHeadline TableCellHeadlineBase BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="height: 166px;" colspan="1" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">
                                            <div style="overflow: hidden; max-height: 156px;">Evaluated but not used</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 166px;" colspan="1" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">
                                            <div style="overflow: hidden; max-height: 156px;">Not evaluated</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 166px;" colspan="1" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">
                                            <div style="overflow: hidden; max-height: 156px;">Using across more than one OS</div>
                                        </asp:TableCell>
                                        <asp:TableCell style="height: 166px;" colspan="1" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">
                                            <div style="overflow: hidden; max-height: 156px;">Using on a single OS</div>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idvariable="89acb66e-223f-4161-a760-1b60299ce500" istitle="True">
                                        <asp:TableCell colspan="1" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor1">C</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor5">D</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor1">E</asp:TableCell>
                                        <asp:TableCell colspan="1" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellHeadline TableCellHeadlineSigDiff BorderColor7 BackgroundColor5">F</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053" istitle="True">
                                        <asp:TableCell rowspan="30" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell rowspan="30" class="TableCellHeadline BackgroundColor21 TableCellHeadlineNestedLeftVariable BorderColor7">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" idselected="e0e50456-9f7f-4038-8402-24e05e1ab053" class="VariableSelection " id="cphContent_ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004' , 'ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004', this, 'cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004" onclick="ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004', this.id, 'TopLeft','onclick', false, 0, undefined);null">How well each of the following attributes describes BRAND Innovative:  Oracle</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004','cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004' , 'ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004', this, 'cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004','cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5004');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="3" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">49</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">19</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">9</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">18</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">2</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">4</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">5&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">7</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">14&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">16&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">22&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">11&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">6</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">4&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">5&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">6&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">11</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">4</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">22&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">21&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">67&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">22&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">17&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">8</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">16</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">6</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">9</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">33&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">32&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">33&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">50&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Describes Completely</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">12</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">4</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', 'F,E');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">F</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', 'E,F');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">E</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">24&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">21&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">56&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">17&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">Does not describe at all</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053" istitle="True">
                                        <asp:TableCell style="width: 100px;" rowspan="30" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">Female</asp:TableCell>
                                        <asp:TableCell rowspan="30" class="TableCellHeadline BackgroundColor20 TableCellHeadlineNestedLeftVariable BorderColor7">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" idselected="e0e50456-9f7f-4038-8402-24e05e1ab053" class="VariableSelection " id="cphContent_ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005' , 'ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005', this, 'cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005" onclick="ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005', this.id, 'TopLeft','onclick', false, 0, undefined);null">How well each of the following attributes describes BRAND Innovative:  Oracle</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005','cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005' , 'ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005', this, 'cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005','cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5005');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="3" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">10</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">4</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">2</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">4</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">33&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">20&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">33&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">6</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">30&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">67&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">33&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">8</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Describes Completely</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">20&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">33&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">25&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">Does not describe at all</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053" istitle="True">
                                        <asp:TableCell style="width: 100px;" rowspan="30" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Male</asp:TableCell>
                                        <asp:TableCell rowspan="30" class="TableCellHeadline BackgroundColor21 TableCellHeadlineNestedLeftVariable BorderColor7">
                                            <asp:Table runat="server" cellspacing="0" cellpadding="0" style="border-collapse: collapse;" class="TableVariableSelector">
                                                 
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <img src="/Images/Icons/Delete2.png?Version=00122" onclick="RemoveVariable('Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]');" class="ImageDeleteVariable"></asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell style="height: 100%;">
                                                            <div xpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" idselected="e0e50456-9f7f-4038-8402-24e05e1ab053" class="VariableSelection " id="cphContent_ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006">
                                                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006">
                                                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                </div>
                                                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006">
                                                                </div>
                                                                <div class="VariableLabel" id="cphContent_pnlSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006">
                                                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006' , 'ddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006', this, 'cphContent_hbCategoriesddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006" onclick="ShowHoverBox('cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006', this.id, 'TopLeft','onclick', false, 0, undefined);null">How well each of the following attributes describes BRAND Innovative:  Oracle</span>
                                                                </div>
                                                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006','cphContent_hbDropDownddlLeftVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006');</script>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <div class="NestedVariableSelector BorderColor7">
                                                                <div xpath="" parentxpath="Report[@DataRequest=&quot;11/20/2014 3:11:47 AM&quot;]/Variables[@Position=&quot;Left&quot;]/Variable[@Id=&quot;89acb66e-223f-4161-a760-1b60299ce500&quot;]/Variable[@Id=&quot;e0e50456-9f7f-4038-8402-24e05e1ab053&quot;]" class="VariableSelection " id="cphContent_ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006">
                                                                    <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006">
                                                                        <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                                                    </div>
                                                                    <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006">
                                                                    </div>
                                                                    <div class="VariableLabel" id="cphContent_pnlSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006">
                                                                        <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006' , 'ddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006', this, 'cphContent_hbCategoriesddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006" onclick="ShowHoverBox('cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006', this.id, 'TopLeft','onclick', false, 0, undefined);null">add nested</span>
                                                                    </div>
                                                                    <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006','cphContent_hbDropDownddlNewNestedTopVariablee0e50456-9f7f-4038-8402-24e05e1ab05389acb66e-223f-4161-a760-1b60299ce5006');</script>
                                                                </div>
                                                            </div>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                 
                                            </asp:Table>
                                        </asp:TableCell>
                                        <asp:TableCell rowspan="3" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Base</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" class="TableCellValue BorderColor7 BackgroundColor9">39</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">16</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">6</asp:TableCell>
                                        <asp:TableCell style="width: 150px;" rowspan="3" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">14</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">2</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'b51e854c-09e0-4ae8-8121-8b843f22d01c', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="b51e854c-09e0-4ae8-8121-8b843f22d01c" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'c4b9d31e-b6d6-4edd-b934-91fc25d1b04b', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="c4b9d31e-b6d6-4edd-b934-91fc25d1b04b" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">4</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'a5351e70-b035-406c-88f0-b8fe67d2cc50', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="a5351e70-b035-406c-88f0-b8fe67d2cc50" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">5</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">5</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">3</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f4b59e6d-e005-4e4e-a52e-67fee5f26970', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f4b59e6d-e005-4e4e-a52e-67fee5f26970" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">13&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">19&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">7&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">6</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'bdc8bc35-e124-46ff-8f83-c5f456d61dbe', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="bdc8bc35-e124-46ff-8f83-c5f456d61dbe" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">3&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">6&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">7</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">8</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">2</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">3</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'ad62c08f-d344-44e4-a624-217924ea1574', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="ad62c08f-d344-44e4-a624-217924ea1574" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">21&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">13&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">67&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">17&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">21&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">8</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">15</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">6</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">1</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">8</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'e507d53e-720e-4991-a815-3a18ae73eed1', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="e507d53e-720e-4991-a815-3a18ae73eed1" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">38&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">38&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">33&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">57&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5">Describes Completely</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">10</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">4</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">4</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">2</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', 'F,E');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">F</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', 'f2bc787b-a4dc-498d-b83d-c7d8a44b0faf', 'E,F');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">E</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="f2bc787b-a4dc-498d-b83d-c7d8a44b0faf" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">26&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">25&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">67&nbsp;%</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">14&nbsp;%</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell style="width: 100px;" rowspan="3" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" istitle="True" class="TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1">Does not describe at all</asp:TableCell>
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="78303997-2435-4a6d-98f4-178cf43caf65" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="fddea6a9-7cee-45e0-876f-2707f510de93" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="d5752f1f-6b57-4e4e-b3d7-ad5bc11099eb" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">-</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" idcategory="eb2860e4-3f06-4f7a-ae94-0678d1f72ed2" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">-</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell onmouseout="DeHighlightSigDiff();" onmouseover="HighlightSigDiff('30d5a6cb-abd2-4d2e-bd21-93200b87768e', 'e0e50456-9f7f-4038-8402-24e05e1ab053', '691d7279-bc9b-48e5-b137-0740dd303c95', '');" idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow idcategory="691d7279-bc9b-48e5-b137-0740dd303c95" idvariable="e0e50456-9f7f-4038-8402-24e05e1ab053">
                                        <asp:TableCell class="TableCellValue BorderColor7 BackgroundColor9"></asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="C" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="D" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="E" class="TableCellValue BorderColor7 BackgroundColor5">&nbsp;</asp:TableCell>
                                        <asp:TableCell idvariable="30d5a6cb-abd2-4d2e-bd21-93200b87768e" significanceletter="F" class="TableCellValue BorderColor7 BackgroundColor9">&nbsp;</asp:TableCell>
                                    </asp:TableRow>
                                 
                            </asp:Table>
                        </asp:TableCell>
                        <asp:TableCell style="padding: 0px; width: 200px; max-width: 200px;" class="Color1">
                            <div style="height: 50px;" xpath="" class="VariableSelection TableCellHeadline TableCellHeadlineNewTopVariable BackgroundColor7" id="cphContent_ddlNewTopVariable2">
                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewTopVariable2">
                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewTopVariable2"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewTopVariable2', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                </div>
                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewTopVariable2">
                                </div>
                                <div class="VariableLabel" id="cphContent_pnlSelectorddlNewTopVariable2">
                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewTopVariable2' , 'ddlNewTopVariable2', this, 'cphContent_hbCategoriesddlNewTopVariable2');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewTopVariable2" onclick="ShowHoverBox('cphContent_hbDropDownddlNewTopVariable2', this.id, 'TopLeft','onclick', false, 0, undefined);null">Search variable...</span>
                                </div>
                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewTopVariable2','cphContent_hbDropDownddlNewTopVariable2');</script>
                            </div>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell style="padding: 0px;" class="Color1">
                            <div style="width: 210px;" xpath="" class="VariableSelection TableCellHeadline BackgroundColor7" id="cphContent_ddlNewLeftVariable1">
                                <div style="opacity: 0.0; height: 300px; overflow: auto;" class="HoverBox HoverBoxVariableSelection BorderColor1" id="cphContent_hbDropDownddlNewLeftVariable1">
                                    <script type="text/javascript">loadFunctions.push(function() {  var trigger = document.getElementById("cphContent_txtSelectorddlNewLeftVariable1"); trigger.setAttribute("onclick","ShowHoverBox('cphContent_hbDropDownddlNewLeftVariable1', this.id, 'TopLeft','onclick', false, 0, undefined);" + trigger.getAttribute("onclick")); });</script>
                                </div>
                                <div style="opacity: 0.0; overflow: auto;" class="HoverBox HoverBoxVariableSelectionCategories" id="cphContent_hbCategoriesddlNewLeftVariable1">
                                </div>
                                <div class="VariableLabel" id="cphContent_pnlSelectorddlNewLeftVariable1">
                                    <span onmouseup="EnabledVariableSearch('c3d85c52-aeca-44a1-9bc4-f7d96ffe88ab', 'cphContent_hbDropDownddlNewLeftVariable1' , 'ddlNewLeftVariable1', this, 'cphContent_hbCategoriesddlNewLeftVariable1');" class="TextBoxVariableSelector" id="cphContent_txtSelectorddlNewLeftVariable1" onclick="ShowHoverBox('cphContent_hbDropDownddlNewLeftVariable1', this.id, 'TopLeft','onclick', false, 0, undefined);null">Search variable...</span>
                                </div>
                                <script type="text/javascript">InitSearchBox('cphContent_txtSelectorddlNewLeftVariable1','cphContent_hbDropDownddlNewLeftVariable1');</script>
                            </div>
                        </asp:TableCell>
                    </asp:TableRow>
                 
            </asp:Table>
        </div>
    </form>
</body>
</html>
