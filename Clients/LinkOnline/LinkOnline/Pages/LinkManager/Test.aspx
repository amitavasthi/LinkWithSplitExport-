<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="LinkOnline.Pages.LinkManager.Test" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Controls/CategorySearch.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Controls/CategorySearch.js"></wu:ScriptReference>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphNonScroll" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <!--<script type="text/javascript">
        function searchStudy(expression) {
            _AjaxRequest("Test.aspx", "LoadStudyVariables", "Expression=" + expression, function (response) {
                document.getElementById("pnlStudyVariables").innerHTML = response;
            });
        }
        function searchTax(expression) {
            _AjaxRequest("Test.aspx", "LoadTaxonomyVariables", "Expression=" + expression, function (response) {
                document.getElementById("pnlTaxonomyVariables").innerHTML = response;
            });
        }

        searchStudy("");
        searchTax("");
    </script>
    <table cellspacing="0" cellpadding="0" style="width:100%">
        <tr>
            <td>
                <input type="text" id="txtSearchStudyVariables" onkeyup="searchStudy(this.value)" />
            </td>
            <td>
                <input type="text" id="txtSearchTaxonomyVariables" onkeyup="searchTax(this.value)" />
            </td>
        </tr>
        <tr valign="top">
            <td style="width:50%">
                <div id="pnlStudyVariables"></div>
            </td>
            <td>
                <div id="pnlTaxonomyVariables"></div>
            </td>
        </tr>
    </table>-->
    <table cellspacing="0" cellpadding="0" style="width:100%">
        <tr valign="top">
            <td style="width:50%">
                <wu:CategorySearch ID="csStudyCategories" runat="server" />
            </td>
            <td>
                <wu:CategorySearch ID="csTaxonomyCategories" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
