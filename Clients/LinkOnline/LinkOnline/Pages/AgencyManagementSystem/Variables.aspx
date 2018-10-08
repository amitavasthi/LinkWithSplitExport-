<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Variables.aspx.cs" Inherits="LinkOnline.Pages.AgencyManagementSystem.Variables" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/VariableSelector.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSelector2.css"></wu:StylesheetReference>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphTitle" runat="server">
    <h1 class="Color1">
        <asp:Label ID="lblPageTitle" runat="server"></asp:Label>
        <span class="Beta">
            <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
        </span>
    </h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div style="margin:1em">
        <asp:Panel ID="pnlVariables" runat="server" CssClass="PnlVariableSelectors"></asp:Panel>
    </div>
    <wu:Box ID="boxCategorizeTextVariable" runat="server" Title="CategorizeTextVariable" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true">
        <h2 class="Color1">
            <wu:Label ID="lblCategorizeTextVariableStep1" runat="server" Name="CategorizeTextVariableStep1"></wu:Label>
        </h2>
        <a id="lnkCategorizeTextVariableDownloadAssignment" target="_blank">
            <wu:Label ID="lblCategorizeTextVariableDownloadAssignment" runat="server" Name="DownloadTextAssignment"></wu:Label>
        </a>
        <br />
        <h2 class="Color1">
            <wu:Label ID="lblCategorizeTextVariableStep2" runat="server" Name="CategorizeTextVariableStep2"></wu:Label>
        </h2>
        <wu:Label ID="lblCategorizeTextVariableStep2Description" runat="server" Name="CategorizeTextVariableStep2Description"></wu:Label>
        <br />
        <h2 class="Color1">
            <wu:Label ID="lblCategorizeTextVariableStep3" runat="server" Name="CategorizeTextVariableStep3"></wu:Label>
        </h2>
        <table>
            <tr>
                <td>
                    <wu:Label ID="lblCategorizeTextVariableStep3VariableName" runat="server" Name="CategorizeTextVariableStep3VariableName"></wu:Label>
                </td>
                <td>
                    <input type="text" id="txtCategorizeTextVariableStep3VariableName" />
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblCategorizeTextVariableStep3UploadFile" runat="server" Name="CategorizeTextVariableStep3UploadFile"></wu:Label>
                </td>
                <td>
                    <input type="file" id="fuCategorizeTextVariableStep3File" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <input type="button" id="btnCategorizeTextVariableStep3FileUpload" />
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlCategorizeTextVariableStep4" runat="server" Visible="false">
            <h2 class="Color1">
                <wu:Label ID="lblCategorizeTextVariableStep4" runat="server" Name="CategorizeTextVariableStep4"></wu:Label>
            </h2>
            <a id="lnkCategorizeTextVariableDownloadLinkAssignment">
                <wu:Label ID="lnkCategorizeTextVariableDownloadLinkAssignment" runat="server" Name="DownloadAssignment"></wu:Label>
            </a>
        </asp:Panel>
    </wu:Box>
</asp:Content>
