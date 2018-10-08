<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="CreateGroup.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.CreateGroup" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
     <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:StylesheetReference runat="server" Source="/Stylesheets/UserRole.css" />
    <link href="/Stylesheets/Main.css" rel="stylesheet" />
    <wu:StylesheetReference Source="/Stylesheets/OverView.css" runat="server" />
     <script type="text/javascript">
         function fnCreateLoad() {
             if (Page_ClientValidate()) {
                 return true;
             }
             else {
                 return false;
             }
         }

    </script>   
    <style>
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
    <h1 class="Color1"><wu:Label ID="Label1" runat="server" Name="CreateGroup"></wu:Label></h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
     <div class="container-fluid">
      <div class="row">   
          <div class="col-xs-12">
               <table style="margin-top:10px;position:absolute;">
                    <tr class="rowclass">
                       <td>
                           <div class="BackButton" onclick="window.location='ManageUserGroupsHome.aspx'"></div>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                </table> 
               <table style="margin-left:auto;margin-right:auto;margin-top:30px;" id="tblUserGroup">
                     <tr class="rowclass">
                      <td colspan="2" style="float:right;">
                           <wu:Label runat="server" Name="GroupName" CssClass="Color1"></wu:Label>
                      </td>
                       <td>
                          &nbsp;<wu:TextBox runat="server" ID="txtGroupName" MaxLength="40"/>
                             <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGroupName" ErrorMessage="group name is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtGroupName" ID="RegularExpressionValidator1" ValidationExpression="^[a-zA-Z''-'\s]{1,40}$" runat="server" ForeColor="Red" ErrorMessage="please verify the usergroup name you entered."></asp:RegularExpressionValidator>
                      </td>                     
                      </tr>
                      <tr class="rowclass">
                          <td>&nbsp;</td>
                      </tr>
                      <tr class="rowclass">
                          <td colspan="3">
                                <wu:Label runat="server" Name="Selectthemodulestoassigntothegroup" CssClass="Color1"></wu:Label>
                            </td>
                      </tr>
                </table>
              <table id="tblOuter" runat="server" style="margin-top:10px;margin-left:auto;margin-right:auto;margin-bottom:20px;">               
                <tr class="rowclass">
                      <td>&nbsp;</td>
                  </tr>
                  <tr class="rowclass">
                      <td colspan="2" align="center">
                          <asp:Panel id="pnlModuleDetails" runat="server" Visible="True"></asp:Panel>
                      </td>
                                
                  </tr>
                    <tr class="rowclass">
                          <td colspan="2" align="center">
                             <wu:Button runat="server" ID="btnCreateGroup" CssClass="Button" Name="Create"  OnClientClick="return fnCreateLoad();"  OnClick="btnCreateGroup_OnClick" Visible="True"/> 
                          </td>
                      </tr>
              </table>
        </div>
    </div>
    </div>
    
</asp:content>
