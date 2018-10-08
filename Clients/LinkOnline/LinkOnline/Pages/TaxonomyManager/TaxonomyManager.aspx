<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="TaxonomyManager.aspx.cs" Inherits="LinkOnline.Pages.TaxonomyManager.TaxonomyManager" %>
<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/VariableSelector.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSelector2.css"></wu:StylesheetReference>

    <style type="text/css">
        .TaxonomyChapterTitle {
            border-top:1px solid #FFFFFF;
            background:#444444;
            cursor:pointer;
        }

        .TaxonomyChapterTitleLabel {
            padding:1em;
            font-weight:bold;
            color:#FFFFFF;
            font-size:16pt;
        }

        .TaxonomyChapterVariables {
            overflow:hidden;
        }

        .TreeViewNodeLabel {
            cursor:pointer;
        }

        .LabelHierarchyPath {
            font-size:18pt;
        }
    </style>
    <script type="text/javascript">
        function HideTaxonomyChapter(sender, idTaxonomyChapter) {
            var control = document.getElementById("TaxonomyChapterVariables" + idTaxonomyChapter);

            control["Height"] = control.offsetHeight;

            DecreaseHeight(control, 0);

            sender.setAttribute("onclick", "ShowTaxonomyChapter(this, '"+ idTaxonomyChapter +"')");
        }

        function ShowTaxonomyChapter(sender, idTaxonomyChapter) {
            var control = document.getElementById("TaxonomyChapterVariables" + idTaxonomyChapter);

            IncreaseHeight(control, control["Height"]);

            sender.setAttribute("onclick", "HideTaxonomyChapter(this, '" + idTaxonomyChapter + "')");
        }

        loadFunctions.push(function () {
            document.getElementById("pnlTaxonomyVariablesContainer").style.height = (ContentHeight - 66) + "px";
            document.getElementById("cphContent_pnlHierarchies").style.height = (ContentHeight - 66) + "px";
        });


        var selectedHierarchy = undefined;
        function LoadVariables(sender, idHierarchy) {
            var nodes = GetChildsByAttribute(document.getElementById("cphContent_pnlHierarchies"), "class", "TreeViewNode BackgroundColor1", true);

            for (var i = 0; i < nodes.length; i++) {
                nodes[i].className = "TreeViewNode BackgroundColor5";
            }

            sender.parentNode.className = "TreeViewNode BackgroundColor1";
            document.getElementById("lblHierarchyPath").innerHTML = sender.parentNode.getAttribute("Path");

            selectedHierarchy = {
                Id: idHierarchy,
                Name: sender.textContent,
                Path: sender.parentNode.getAttribute("Path")
            };

            ShowLoading(document.getElementById("pnlTaxonomyVariablesContainer"));

            _AjaxRequest("TaxonomyManager.aspx", "GetVariables", "IdHierarchy=" + idHierarchy, function (response) {
                var pnlTaxonomyVariables = document.getElementById("pnlTaxonomyVariables");
                
                pnlTaxonomyVariables.innerHTML = response;

                var scripts = pnlTaxonomyVariables.getElementsByTagName("script");

                for (var i = 0; i < scripts.length; i++) {
                    eval(scripts.item(i).innerHTML);
                }

                HideLoading();
            });
        }

        function UploadTaxonomyFile() {
            document.getElementById("lblBoxUploadPath").innerHTML = selectedHierarchy.Path;
            document.getElementById("hdfUploadHierarchy").value = selectedHierarchy.Id;

            InitDragBox('boxUploadControl', 'Bottom');
            document.getElementsByClassName('BoxContent')[0].style.overflowY = "";
        }

        function ResetHierarchyTaxonomy() {
            CreateConfirmBox(LoadLanguageText("ResetHierarchyTaxonomyMessage").replace("{0}", selectedHierarchy.Name), function () {
                _AjaxRequest("TaxonomyManager.aspx", "ResetHierarchyTaxonomy", "IdHierarchy=" + selectedHierarchy.Id, function (response) {
                    LoadVariables(document.getElementById("cphContent__tvnHierarchy" + selectedHierarchy.Id), selectedHierarchy.Id);
                });
            });
        }
    </script>
</asp:Content>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitleTaxonomyManager" runat="server" Name="TaxonomyManagerTitle"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphNonScroll" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <wu:ConfirmBox ID="cbReset" runat="server"></wu:ConfirmBox>
    
    <table style="width:100%" cellspacing="0" cellpadding="0">
        <tr valign="top">
            <td class="BackgroundColor7" rowspan="2" style="width:250px;min-width:250px;">
                <div style="margin:1em">
                    <asp:Panel id="pnlHierarchies" runat="server" style="overflow:auto;"></asp:Panel>
                </div>
            </td>
            <td class="BackgroundColor7" style="height: 50px;">
                <div style="margin:10px;">
                    <div style="float:right">
                        <wu:Button ID="btnDownload" runat="server" Name="DownloadTaxonomy" OnClick="btnDownload_Click" />
                        <input type="button" ID="btnUpload" name="UploadTaxonomy" runat="server" onclick="UploadTaxonomyFile();" />
                        <wu:Button ID="btnReset" runat="server" Name="Reset" OnClick="btnReset_Click" />
                        <wu:Button ID="btnReset2" runat="server" Name="ResetHierarchyTaxonomy" OnClientClick="ResetHierarchyTaxonomy();return false;" />
                    </div>
                    <div id="lblHierarchyPath" class="LabelHierarchyPath Color1">

                    </div>
                    <div style="clear:both"></div>
                </div>
            </td>
        </tr>
        <tr valign="top">
            <td style="width:100%;">
                <div id="pnlTaxonomyVariablesContainer" style="overflow:auto;">
                    <div style="margin:1em">
                        <div id="pnlTaxonomyVariables"></div>
                    </div>
                </div>
            </td>
        </tr>
    </table>

    <wu:Box ID="boxUpload" runat="server" Title="<div id='lblBoxUploadPath' style='float:left;'></div>" TitleLanguageLabel="false" Dragable="true" JavascriptTriggered="true">
        <div style="clear:both"></div>
        <input type="hidden" id="hdfUploadHierarchy" name="hdfUploadHierarchy" />
        <table style="margin-top:10px;">
            <tr>
                <td>
                    <wu:Label ID="lblUploadFile" runat="server" Name="SelectTaxonomyFile"></wu:Label>
                </td>
                <td>
                    <asp:FileUpload ID="fuUploadFile" runat="server"></asp:FileUpload>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblUploadLanguage" runat="server" Name="Language"></wu:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlUploadLanguage" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblUploadHierarchySpecificUpload" runat="server" Name="HierarchySpecificUpload"></wu:Label>
                </td>
                <td>
                    <asp:Checkbox ID="chkUploadHierarchySpecificUpload" runat="server" Checked="True">
                    </asp:Checkbox>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <wu:Button ID="btnUploadConfirm" runat="server" Name="Upload" OnClick="btnUploadConfirm_Click"></wu:Button>
                    <wu:Button ID="btnUploadCancel" runat="server" Name="Cancel"></wu:Button>
                </td>
            </tr>
        </table>
    </wu:Box>
</asp:Content>
