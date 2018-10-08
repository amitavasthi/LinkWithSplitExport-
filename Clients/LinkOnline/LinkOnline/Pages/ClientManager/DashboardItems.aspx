<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="DashboardItems.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.DashboardItems" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
      <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
  <script>
        function fnModifyLoad() {
            if (Page_ClientValidate("g1")) {
                ShowLoading(document.getElementById("tblContent"));
            }
            else {
                return false;
            }
        }
        $(document).ready(function () {
            $("[id*=btnDashboardItemCancel]").on('click', function () {
                setTimeout(function () {  document.getElementById("LoadingBlur").remove(); }, 1000);
                CloseBox('boxDashboardItemControl', 'Bottom');
            });
        });
     
    </script>
     <table>
        <tr>
            <td>
              
            </td>
            <td>
                 <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="DashboardItems"></wu:Label>
                    <span class="Beta">
                    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
                    </span>
                 </h1>
            </td>
        </tr>
    </table>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <div style="margin:1em">
          <div class="Color1" style="margin-top: 0;margin-left:5px;margin-bottom:10px;">
            <table>
                <tr>
                    <td>
                        <div class="BackButton" onclick="window.location='HomeScreenManagement.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="pnlGrid" runat="server"></asp:Panel>
        <div style="float:right">
            <wu:Button ID="btnAdd" runat="server" Name="Add" OnClick="btnAdd_Click" />
            <wu:Button ID="btnEdit" runat="server" Name="Edit" OnClick="btnEdit_Click" />
            <wu:Button ID="btnDelete" runat="server" Name="Delete" OnClick="btnDelete_Click" />
        </div>
    </div>

    <wu:Box ID="boxDashboardItem" runat="server" Title="" Dragable="true">
        <table>            
            <tr>
                <td>
                    <wu:Label ID="lblDashboardItemTitle" runat="server" Name="DashboardItemTitle"></wu:Label>
                </td>
                <td>
                    <wu:TextBox ID="txtDashboardItemTitle" runat="server"></wu:TextBox><br />
                     <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDashboardItemTitle" ErrorMessage="Dashboard title required" ForeColor ="Red"  display="Dynamic" ValidationGroup="g1"> *</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator 
                           runat="server" ControlToValidate ="txtDashboardItemTitle" 
                           ErrorMessage="Must be a valid title" ForeColor ="Red" ValidationExpression="^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]+$" ValidationGroup="g1"
                           >
                </asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblDashboardItemSource" runat="server" Name="DashboardItemSource"></wu:Label>
                </td>
                <td>
                    <wu:TextBox ID="txtDashboardItemSource" runat="server"></wu:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <wu:Button ID="btnDashboardItemAdd" runat="server" Name="Add" OnClick="btnDashboardItemAdd_Click" ValidationGroup="g1"  OnClientClick="return fnModifyLoad();"></wu:Button>
                    <wu:Button ID="btnDashboardItemSave" runat="server" Name="Save" OnClick="btnDashboardItemSave_Click" ValidationGroup="g1" OnClientClick="return fnModifyLoad();"></wu:Button>
                    <wu:Button ID="btnDashboardItemCancel" runat="server" Name="Cancel"></wu:Button>
                </td>
            </tr>
        </table>
    </wu:Box>
    <wu:ConfirmBox ID="cbDelete" runat="server" Visible="false"></wu:ConfirmBox>
</asp:content>
