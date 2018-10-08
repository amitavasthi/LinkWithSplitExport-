<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.ManageUsers" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
       <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:StylesheetReference Source="/Stylesheets/OverView.css" runat="server" />
    <link href="/Stylesheets/Main.css" rel="stylesheet" />
     <script type="text/javascript">
         function fnCreateLoad() {
             if (Page_ClientValidate()) {
                 ShowLoading(document.getElementById("boxMangeUsers"));
             }
             else {
                 return false;
             }
         }
     </script>  
     <style>
         .styleclass {
             white-space: pre-wrap;
         }

         #cphContent_tblContent {
             clear: both;
         }

         .rowclass {
             margin-top: 10px;
             margin-bottom: 10px;
         }

         body {
             font-family: SegoeUILight;
         }
     </style> 
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="ManageUsers"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>

<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
       <div class="container-fluid">  
          <div class="row" style="height:100%;">   
              <div class="col-xs-12">
            <table style="margin-top:10px;">
                 <% DatabaseCore.Items.User user = this.Core.Users.GetSingle(this.IdUser.Value);
                     if (user.HasPermission(111) || user.HasPermission(111))
                     { %>  
                <tr class="rowclass">
                    <td>
                        <div class="BackButton" onclick="window.location='UsersHome.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <%  }
                    else
                    { %>
                <tr class="rowclass">
                    <td>
                        <div class="BackButton" onclick="window.location='../Overview.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <%
                    }
                %>
            </table>
           <div class="Color1" style="height: 20px;">
            <wu:ConfirmBox ID="cbRemove" runat="server"></wu:ConfirmBox>
        </div>
            <div style="margin-left:370px;margin-top:10px;">
                 <wu:Label ID="lblMsg" runat="server" Name="UserModifyMsg" Visible="false"></wu:Label>
                  <wu:Label ID="lblAppError" runat="server" Visible="false"></wu:Label>
          </div>
           <div style="margin:auto;height:auto;">
             <asp:Panel id="pnlUserManagement" runat="server"></asp:Panel>
        </div>
                <%--<asp:HyperLink ID="HyperLink1" runat="server" Text="Create New User" NavigateUrl="~/Pages/ClientManager/CreateUser.aspx" style="font-size:16px"> </asp:HyperLink>--%>
            <div style="height:25px;float:right;margin-top:10px;">
              <wu:Table runat="server" Visible="true" ID="tblClient" Width="40%"  HorizontalAlign="Right">
               <wu:TableRow>
                   <wu:TableCell>
                       <wu:Button runat="server"  ID="btnCreateUser"  Name="AddUser" CssClass="Button"  OnClick="btnCreateUser_OnClick" />                     
                   </wu:TableCell>
                   <wu:TableCell>
                     <wu:Button runat="server"  ID="btnModify"  Name="Modify" CssClass="Button"  OnClick="btnModify_OnClick" />
                   </wu:TableCell>
                  <wu:TableCell>
                     <wu:Button runat="server"  ID="btnDelete"  Name="Delete" CssClass="Button"   OnClick="btnDelete_OnClick" />
                   </wu:TableCell> 
                    <wu:TableCell>
                     <wu:Button runat="server"  ID="btnReminder"  Name="Reminder" CssClass="Button"   OnClick="btnReminder_Click" />
                   </wu:TableCell>           
               </wu:TableRow>
              </wu:Table>
                </div>
            
          </div>
              </div>
           </div>
</asp:content>
