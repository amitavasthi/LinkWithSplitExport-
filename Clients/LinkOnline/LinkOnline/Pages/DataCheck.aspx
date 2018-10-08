<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="DataCheck.aspx.cs" Inherits="LinkOnline.Pages.DataCheck" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table>
        <tr>
            <td>
                Study
            </td>
            <td>
                <asp:DropDownList ID="ddlStudy" runat="server"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                V2 Database Server
            </td>
            <td>
                <asp:TextBox ID="txtDatabaseServer" runat="server" Text="."></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                V2 Database Name
            </td>
            <td>
                <asp:TextBox ID="txtDatabaseName" runat="server" Text="Microsoft1"></asp:TextBox>
            </td>
        </tr>
    </table>

    <wu:Button ID="btnCheck" runat="server" Name="Check" OnClick="btnCheck_Click" />

    <asp:Panel ID="pnlResult" runat="server"></asp:Panel>
</asp:Content>
