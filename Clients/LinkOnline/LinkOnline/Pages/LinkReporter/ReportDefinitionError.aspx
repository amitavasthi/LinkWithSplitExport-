<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ReportDefinitionError.aspx.cs" Inherits="LinkOnline.Pages.LinkReporter.ReportDefinitionError" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .Content {
            background:#880000;
        }

        .Error {
            margin:2em;
        }

        .Error, h1 {
            color:#FFFFFF;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div class="Error">
        <wu:Label ID="lblReportDefinitionError" runat="server" Name="ReportDefinitionError"></wu:Label>
        <br />
        <wu:Button ID="btnTryAgain" runat="server" Name="TryAgain" OnClick="btnTryAgain_Click" />
        <wu:Button ID="btnResetReportDefinition" runat="server" Name="ResetReportDefinition" OnClick="btnResetReportDefinition_Click" />

        <br /><br /><br />
        <asp:Label ID="lblReportDefinitionErrorMessage" runat="server" style="color:#FFFFFF;"></asp:Label>
    </div>
</asp:Content>
