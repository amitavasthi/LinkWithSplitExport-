<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Dummy.Master" AutoEventWireup="true" CodeBehind="AllocateRolePermission.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.AllocateRolePermission" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
      <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:StylesheetReference runat="server" Source="/Stylesheets/UserRole.css" />
  
</asp:Content>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="cphTitle" runat="server">
    <%--<h1 class="Color1">
        <wu:Label ID="lblPageTitle" runat="server" Name="ManageGroups"></wu:Label></h1>--%>
     <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="ManageGroups"></wu:Label>  
        <span class="Beta">
            <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
        </span></h1>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div style="margin: 1em;">
        <div class="Color1" style="margin-top: 3%;margin-left:3%;">
            <table>
                <tr>
                    <td>
                        <div class="BackButton" onclick="window.location='ManageGroup.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <div class="Color1" style="height: 20px;">
            <wu:ConfirmBox ID="cbRemove" runat="server"></wu:ConfirmBox>
        </div>
        <div style="margin-top: 10px; margin-left: 350px;">
            <table cellpadding="1" cellspacing="3" runat="server">
                <tr>
                    <td colspan="2" align="Center">
                        <wu:Label ID="lblMsg" runat="server" Name="RoleDeleteMsg" Visible="false"></wu:Label>
                        <wu:Label ID="lblAppError" runat="server" Visible="false"></wu:Label>
                    </td>
                    <td colspan="2" align="Center">
                        <asp:HiddenField runat="server" ID="hdnRoleId" Value="" />
                    </td>
                </tr>
            </table>
        </div>
        <div style="margin-top: 10px; margin-left: 50px">
            <asp:Panel ID="pnlModuleDetails" runat="server" Visible="True"></asp:Panel>
        </div>
        <div style="float: right; margin-top: 10px;" runat="server">
            <wu:Button runat="server" ID="btnDelete" CssClass="Button" Name="Remove" OnClick="btnDelete_Click" Visible="True" />
            <wu:Button runat="server" ID="btnAcceptChanges" CssClass="Button" Name="AcceptChanges" OnClick="btnAcceptChanges_OnClick" Visible="True" />
        </div>
    </div>
</asp:Content>
