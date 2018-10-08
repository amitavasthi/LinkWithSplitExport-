<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LinkOnline.Pages.Default" %>

<%@ Register Src="~/Classes/Controls/ConnectPowerBI.ascx" TagPrefix="uc1" TagName="ConnectPowerBI" %>


<asp:content id="ContentHead" contentplaceholderid="cphHead" runat="server">
    <style type="text/css">
        .Content {
            overflow:hidden !important;
        }
    </style>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Homescreen.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/SavedReports.js"></wu:ScriptReference>
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="Home"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:content id="Content" contentplaceholderid="cphContent" runat="server">
    <asp:Panel ID="pnlHomeContainer" runat="server" CssClass="Homescreen"></asp:Panel>
    <uc1:ConnectPowerBI runat="server" id="ConnectPowerBI" />
</asp:content>
