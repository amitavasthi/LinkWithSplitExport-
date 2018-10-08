<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="HomeScreenManagement.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.HomeScreenManagement" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
     <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <link href="/Stylesheets/Main.css" rel="stylesheet" />
    <link href="/Stylesheets/Pages/ClientManager.css" rel="stylesheet" />
</asp:content>

<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="HomeManagement"></wu:Label>
    <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphContent" runat="server">
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
                       <div class="btnStyle">
                                <asp:LinkButton ID="btnHome" runat="server"  Name="CustomChartMgmt" BorderStyle="None" Height="180px" Width="180px" CssClass="btnStyle" OnClick="btnHome_Click">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/Icons/ClientManager/Managedashboards.png" Height="180px" Width="180px" CssClass="pic BorderClass BackgroundColor9" />
                                <br />
                                <span class="styleclass Color1">manage dashboards</span>
                            </asp:LinkButton>
                          </div>
                      </td> 
                          <td>&nbsp;</td>
                      <td>
                        <div class="btnStyle">
                                <asp:LinkButton ID="btnNews" runat="server"  Name="CustomChartMgmt" BorderStyle="None" Height="180px" Width="180px" CssClass="btnStyle" OnClick="btnNews_Click">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Icons/ClientManager/Managenews.png" Height="180px" Width="180px" CssClass="pic BorderClass BackgroundColor9" />
                                <br />
                                <span class="styleclass Color1">manage news</span>
                            </asp:LinkButton>
                          </div>
                      </td>                   
                  </tr>
                  </table>
                </div>
            </div>
        </div>
</asp:content>



