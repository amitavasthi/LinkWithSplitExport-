<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ClientManagerHome.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.ClientManagerHome" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>
<asp:content id="Content1" contentplaceholderid="cphHead" runat="server"> 
      <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:StylesheetReference Source="/Stylesheets/OverView.css" runat="server" />
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
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="ClientManager"></wu:Label>
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
                <tr>
                    <td>
                        <div class="BackButton" onclick="window.location='/Pages/Default.aspx'"></div>
                    </td>
                    <td>
                     &nbsp;
                    </td>
                </tr>
            </table>  
            <table style="margin-top:auto;margin-bottom:auto;margin-left:auto;margin-right:auto;">
                  <tr>
                      <td>
                 <% DatabaseCore.Items.User user = this.Core.Users.GetSingle(this.IdUser.Value);
                     if (user.HasPermission(111) || user.HasPermission(111))
                     { %>  
                           <div class="btnStyle">
                                <asp:LinkButton ID="btnManage" runat="server"  Name="ManageUsers" BorderStyle="None" Height="180px" Width="180px" CssClass="btnStyle" OnClick="btnManage_OnClick">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Icons/ClientManager/Manage Users.png" Height="180px" Width="180px" CssClass="BackgroundColor9 pic BorderClass"/>
                                <br />
                                <span class="styleclass Color1">users</span>
                            </asp:LinkButton>
                        </div>
                          <%
                             } %>
                      </td>
                        <td>&nbsp;</td>
                      <td>
                            <% if (user.HasPermission(113))
                               { %>  
                          <div class="btnStyle">
                                <asp:LinkButton ID="btnUserGroups" runat="server"  Name="ManageUsers" BorderStyle="None" Height="180px" Width="180px" CssClass="btnStyle" OnClick="btnUserGroups_OnClick">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/Icons/ClientManager/Manage User Groups.png" Height="180px" Width="180px" CssClass="BackgroundColor9 pic BorderClass"/>
                                <br />
                                <span class="styleclass Color1">user groups</span>
                            </asp:LinkButton>
                          </div> <%
                               } %>
                      </td>
                        <td>&nbsp;</td>
                      <td>
                           <% if (user.HasPermission(115))
                              { %>
                          <div class="btnStyle">
                                <asp:LinkButton ID="btnPortalSettings" runat="server"  Name="ManageUsers" BorderStyle="None" Height="180px" Width="180px" CssClass="btnStyle" OnClick="btnPortalSettings_OnClick">
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/Icons/ClientManager/Portal settings.png" Height="180px" Width="180px" CssClass="BackgroundColor9 pic BorderClass"/>
                                <br />
                                <span class="styleclass Color1">portal settings</span>
                            </asp:LinkButton>
                           </div><%
                              } %>
                      </td>
                        <td>&nbsp;</td>
                      <td>
                          <% if (user.HasPermission(114))
                             { %>
                          <div class="btnStyle">
                                <asp:LinkButton ID="btnHomeMgmt" runat="server" Name="ManageUsers" BorderStyle="None" Height="180px" Width="180px" CssClass="btnStyle" OnClick="btnHomeMgmt_Click">
                                <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/Icons/ClientManager/Home Management.png" Height="180px" Width="180px" CssClass="BackgroundColor9 pic BorderClass" />
                                <br />
                                <span class="styleclass Color1">home management</span>
                            </asp:LinkButton>
                          </div><%
                             } %>
                      </td>
                  </tr>
              </table>  
        </div>  
    </div>
</div>
</asp:content>


