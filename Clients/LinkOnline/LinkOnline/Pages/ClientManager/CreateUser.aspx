﻿<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.CreateUser" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>

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
                ShowLoading(document.getElementById("boxUploadAugment"));
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
  <%--  <wu:StylesheetReference runat="server" Source="../../Stylesheets/Pages/Login.css" />
     <wu:StylesheetReference runat="server" Source="../../Stylesheets/UserRole.css" />  --%> 
    </asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="AddUser"></wu:Label>
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
                </table>
                <wu:Table ID="tblContent"  runat="server" style="margin-top:auto;margin-bottom:auto;margin-left:auto;margin-right:auto;">
                 <wu:TableRow Height="45px">
                <wu:TableCell HorizontalAlign="Left" ColumnSpan="3" CssClass="Color1"> 
                    <wu:Label ID="lblMessage" runat="server" Name="AddUser" Font-Size="15pt" CssClass="Color1"></wu:Label>           
                </wu:TableCell>          
            </wu:TableRow>
                 <wu:TableRow Height="45px">
                <wu:TableCell HorizontalAlign="Left" Width="20%">
                    <wu:Label ID="lblUserName" runat="server" Name="UserName" CssClass="Color1"></wu:Label>           
                </wu:TableCell>
                <wu:TableCell  Width="30%" HorizontalAlign="Left">
                    <wu:TextBox ID="txtName" runat="server" MaxLength="255" AutoCompleteType="Office" Width="170px" Height="30px"></wu:TextBox>
                </wu:TableCell>
                 <wu:TableCell Width="50%" HorizontalAlign="Left">
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" ErrorMessage="password is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator  ControlToValidate="txtName" ID="RegularExpressionValidator3" ValidationExpression="^[a-zA-Z0-9@._-]{3,255}$" runat="server" ForeColor="Red" ErrorMessage="please verify the name you entered."></asp:RegularExpressionValidator>
                </wu:TableCell>
            </wu:TableRow>
                 <wu:TableRow Height="45px">
                  <wu:TableCell Width="20%" HorizontalAlign="Left">
                        <wu:Label ID="lblFName" runat="server" Name="FirstName" CssClass="Color1"></wu:Label>             
                  </wu:TableCell>
                <wu:TableCell Width="30%" HorizontalAlign="Left">
                        <wu:TextBox ID="txtFirstName" runat="server" MaxLength="255" Width="170px" Height="30px"></wu:TextBox>
                </wu:TableCell>    
                <wu:TableCell Width="50%" HorizontalAlign="Left">
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName" ErrorMessage="first name is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator ControlToValidate="txtFirstName" ID="RegularExpressionValidator1" ValidationExpression="^[a-zA-Z''-'\s]{1,40}$" runat="server" ForeColor="Red" ErrorMessage="please verify the first name you entered."></asp:RegularExpressionValidator>
                </wu:TableCell>       
              </wu:TableRow>
                 <wu:TableRow Height="45px">
                 <wu:TableCell Width="20%" HorizontalAlign="Left">
                         <wu:Label ID="lblLastName" runat="server" Name="LastName" CssClass="Color1" ></wu:Label>
                  </wu:TableCell>
                    <wu:TableCell Width="30%" HorizontalAlign="Left">
                   <wu:TextBox ID="txtLastName" runat="server" MaxLength="255" Width="170px" Height="30px"></wu:TextBox>
                  </wu:TableCell>
                 <wu:TableCell Width="50%">
                   <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName" ErrorMessage="last name is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                     <asp:RegularExpressionValidator ControlToValidate="txtLastName" ID="RegularExpressionValidator2" ValidationExpression="^[a-zA-Z''-'\s]{1,40}$" runat="server" ForeColor="Red" ErrorMessage="please verify the last name you entered."></asp:RegularExpressionValidator>
                  </wu:TableCell>    
            </wu:TableRow> 
                 <wu:TableRow Height="45px">
                 <wu:TableCell Width="20%" HorizontalAlign="Left">
                         <wu:Label ID="lblMail" runat="server" Name="Email" CssClass="Color1"></wu:Label>
                  </wu:TableCell>
                    <wu:TableCell Width="30%" HorizontalAlign="Left">
                   <wu:TextBox ID="txtMail" runat="server" MaxLength="255" Width="170px" Height="30px"></wu:TextBox>
                  </wu:TableCell>
                 <wu:TableCell Width="50%" HorizontalAlign="Left">
                   <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMail" ErrorMessage="email is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                                 <asp:RegularExpressionValidator  ID="RegularExpressionValidatorMail" runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"  
                    ControlToValidate="txtMail"  ErrorMessage="please enter a valid email"  ForeColor ="Red">  
                </asp:RegularExpressionValidator>  
                  </wu:TableCell>    
            </wu:TableRow>            

                      <wu:TableRow Height="45px">
                 <wu:TableCell Width="20%" HorizontalAlign="Left">
                         <wu:Label ID="lblPhone" runat="server" Name="Phone" CssClass="Color1"></wu:Label>
                  </wu:TableCell>
                    <wu:TableCell Width="30%" HorizontalAlign="Left">
                   <wu:TextBox ID="txtPhone" runat="server" MaxLength="50" Width="170px" Height="30px"></wu:TextBox>
                  </wu:TableCell>
                 <wu:TableCell Width="50%" HorizontalAlign="Left">
                 <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPhone" ErrorMessage="phone is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator  ID="RegularExpressionValidator4" runat="server" ValidationExpression="^\+?(?:[0-9]●?){6,14}[0-9]$"  
                    ControlToValidate="txtPhone"  ErrorMessage="please enter a valid phone"  ForeColor ="Red">  
                </asp:RegularExpressionValidator> 
                  </wu:TableCell>    
            </wu:TableRow> 
                 <wu:TableRow Height="45px">
             <wu:TableCell Width="20%" HorizontalAlign="Left">
                    <wu:Label ID="lblRole" runat="server" Name="Role" CssClass="Color1" ></wu:Label>
              </wu:TableCell>
                <wu:TableCell Width="30%" HorizontalAlign="Left">
                    <wu:DropDownList runat="server" ID="ddlRole" Width="170px" Height="30px"/>
              </wu:TableCell>
             <wu:TableCell Width="50%" HorizontalAlign="Left">
             <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlRole" ErrorMessage="user group is required." ForeColor ="Red" InitialValue="select"> *</asp:RequiredFieldValidator> 
                     </wu:TableCell>    
        </wu:TableRow>
                          <wu:TableRow Height="45px">
          <wu:TableCell Width="20%" HorizontalAlign="Left">
                     <wu:Label ID="lblUserWorkgroupsTitle" runat="server" Name="UserWorkgroupsTitle"  CssClass="Color1"></wu:Label>  
              
              </wu:TableCell>
               <wu:TableCell Width="30%" HorizontalAlign="Left">
                <wu:Panel ID="pnlUserWorkgroups" runat="server" BorderWidth="1px" BorderColor="#A9A9A9">
                     </wu:Panel>
              </wu:TableCell>           
        </wu:TableRow>  
                 <wu:TableRow Height="45px">
             <wu:TableCell Width="20%" HorizontalAlign="Left">
                    <wu:Label ID="lblMultiple" runat="server" Name="multiple users" CssClass="Color1" ></wu:Label>
              </wu:TableCell>
                <wu:TableCell Width="30%" HorizontalAlign="Left">
                  <wu:CheckBox runat="server" ID="chkMultiple" />
              </wu:TableCell>
             <wu:TableCell Width="50%" HorizontalAlign="Left">      
                     </wu:TableCell>    
        </wu:TableRow>    
                    
                 <wu:TableRow Height="70px">
                          <wu:TableCell  HorizontalAlign="Center" ColumnSpan="3">
                         <wu:Button runat="server"  ID="btnCreate"  Name="Create" CssClass="Button"  OnClientClick="return fnCreateLoad();" Align="Left" OnClick="btnCreate_Click"></wu:Button>
                         </wu:TableCell>
                    </wu:TableRow>    
                </wu:Table>
             </div>  
         </div> 
  </div>
</asp:content>
