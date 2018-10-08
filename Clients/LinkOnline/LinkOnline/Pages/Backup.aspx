<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Backup.aspx.cs" Inherits="LinkOnline.Pages.Backup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .ClientBackupErrorDetail {
            background:#FF0000 !important;
            color:#FFFFFF;
        }
    </style>
</asp:Content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <%--<h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="BackupTitle"></wu:Label></h1>--%>
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="BackupTitle"></wu:Label>  
        <span class="Beta">
            <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
        </span></h1>
</asp:content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div style="margin:1em">
        <asp:Panel ID="pnlClientBackups" runat="server"></asp:Panel>
        <div style="float:right">
            <wu:Button ID="btnBackup" runat="server" Name="Backup" OnClick="btnBackup_Click" />
        </div>
    </div>
</asp:Content>
