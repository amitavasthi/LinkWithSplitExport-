<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="UsersHome.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.UsersHome" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:StylesheetReference Source="/Stylesheets/Main.css" runat="server" />
    <link href="/Stylesheets/Pages/ClientManager.css" rel="stylesheet" />
       <script type="text/javascript">
           function fnCreateLoad() {
               if (Page_ClientValidate()) {
                   ShowLoading(document.getElementById("boxAddUserControl"));
               }
               else {
                   return false;
               }
           }
    </script>   
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
          <div class="row">   
              <div class="col-xs-12">
                       <table style="margin-top:10px;">
                    <tr class="rowclass">
                       <td>
                           <div class="BackButton" onclick="window.location='ClientManagerHome.aspx'"></div>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                </table>                 
                  <table id="tblOuter" runat="server" style="margin-top:auto;margin-bottom:auto;margin-left:auto;margin-right:auto;">
                      <tr class="rowclass">                        
                      <td>     
                          <% DatabaseCore.Items.User user = this.Core.Users.GetSingle(this.IdUser.Value);
                              if (user.HasPermission(111))
                              { %>                 
                           <div class="btnStyle Color1">
                                <asp:LinkButton ID="btnAdd" runat="server"  Name="CreateUser" BorderStyle="None" Height="180px" Width="180px" CssClass="btnStyle" OnClick="btnAdd_OnClick">
                                      <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Icons/ClientManager/Add New User.png" Height="180px" Width="180px" CssClass="BackgroundColor9 pic BorderClass" />
                                     <br />
                                      <span class="styleclass Color1">create new user</span>
                             </asp:LinkButton>
                           </div>
                          <%} %>
                      </td> 
                          <td>&nbsp;</td>
                      <td>
                           <% DatabaseCore.Items.User user = this.Core.Users.GetSingle(this.IdUser.Value);
                               if (user.HasPermission(112))
                               { %> 
                       <div class="btnStyle Color1">
                                <asp:LinkButton ID="btnManage" runat="server"  Name="CreateUser" BorderStyle="None" Height="180px" Width="180px" CssClass="btnStyle" OnClick="btnManage_OnClick">
                                      <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/Icons/ClientManager/Manage Users.png" Height="180px" Width="180px" CssClass="BackgroundColor9 pic BorderClass" />
                                     <br />
                                      <span class="styleclass Color1">manage users</span>
                             </asp:LinkButton>
                           </div>  
                          <% } %>
                      </td>                   
                  </tr>
                  </table>
                </div>
            </div>
        </div>
</asp:content>
