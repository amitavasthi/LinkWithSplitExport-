<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="LinkOnline.Pages.Account.Home" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>
<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
     <style>
          .styleclass {
              white-space: pre-wrap;
          }
      </style>
      <script type="text/javascript">
          function fnChangePwd() {
              if (Page_ClientValidate()) {
                  ShowLoading(document.getElementById("boxPassword"));
              }
              else {
                  return false;
              }
          }
          function fnContact() {
              if (Page_ClientValidate()) {
                  ShowLoading(document.getElementById("boxMangeUsers"));
              }
              else {
                  return false;
              }
          }
          function fnEmail() {
              if (Page_ClientValidate()) {
                  ShowLoading(document.getElementById("boxEmail"));
              }
              else {
                  return false;
              }
          }
    </script>   
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="navAccountManagement"></wu:Label>  
       <%--<wu:Label CssClass ="Color1" ID="lblPageTitle" runat="server" Name="navAccountManagement"></wu:Label>--%>
        <span class="Beta">
            <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
        </span></h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <div style="margin:1em;">   
         <div class="Color1" style="margin:3%;">
            <table>
                <tr>
                    <td>
                        <div class="BackButton" onclick="window.location='/Pages/Default.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>    
         <div style="margin-top:100px;">
             <wu:Table ID="tblAdmin" runat="server" HorizontalAlign="Center" CellPadding="2" CellSpacing="2">
                 <wu:TableRow>
                      <wu:TableCell ColumnSpan="3" HorizontalAlign="Center">
                              <wu:Label ID="lblMsg" runat="server" Name="" Visible="false"></wu:Label>
                      </wu:TableCell>
                  </wu:TableRow>
                 <wu:TableRow>
                     <wu:TableCell HorizontalAlign="Left">
                         <wu:Button runat="server"  ID="btnChangePassword"  Name="ChangePassword" CssClass="styleclass" BorderStyle="None"  Height="180px" Width="180px" Font-Size="14pt"  OnClick="btnChangePassword_Click" />
                     </wu:TableCell>
                       <wu:TableCell  HorizontalAlign="Left">
                          <wu:Button runat="server"  ID="btnChangeContact"  Name="ChangeContact" CssClass="styleclass" BorderStyle="None" Height="180px" Width="180px" Font-Size="14pt" OnClick="btnChangeContact_Click" />
                     </wu:TableCell>
                     <%-- <wu:TableCell HorizontalAlign="Left">
                         <wu:Button runat="server"  ID="btnChangeEmail" CssClass="styleclass" Name="ChangeEmail" BorderStyle="None"  Height="180px" Width="180px" Font-Size="14pt"  OnClick="btnChangeEmail_Click" />
                     </wu:TableCell>    --%>                 
                 </wu:TableRow>
       </wu:Table>
      </div>
        
        <div class="LoginBox">
            <wu:Box ID="boxPassword" runat="server" Dragable="true" Visible="false" Title="ChangePassword">           
              <table style="margin-top: 0px;">
                  <tr>
                      <td>
                          <wu:Label runat="server" ID="lblOldPassword" Name="OldPassword"></wu:Label>
                      </td>
                       <td>
                          <wu:TextBox runat="server" ID="txtOldPassword" TextMode="Password" MaxLength="32" ></wu:TextBox>
                      </td>
                       <td>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtOldPassword" ErrorMessage="old password required." ForeColor="Red"> *</asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator Display = "Dynamic" ControlToValidate = "txtOldPassword" ID="RegularExpressionValidator4" ValidationExpression = "^[\s\S]{8,32}$" runat="server" ForeColor="Red" ErrorMessage="minimum 8 & maximum 32 characters required."></asp:RegularExpressionValidator>
                      </td>
                  </tr>
                  <tr>
                      <td>
                          <wu:Label runat="server" ID="lblPassword" Name="NewPassword"></wu:Label>
                      </td>
                       <td>
                          <wu:TextBox runat="server" ID="txtPassword" TextMode="Password" MaxLength="32"></wu:TextBox>
                      </td>
                       <td>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" ErrorMessage="old password required." ForeColor="Red"> *</asp:RequiredFieldValidator>
                           <asp:RegularExpressionValidator Display = "Dynamic" ControlToValidate = "txtPassword" ID="RegularExpressionValidator5" ValidationExpression = "^[\s\S]{8,32}$" runat="server" ForeColor="Red" ErrorMessage="minimum 8 & maximum 32 characters required."></asp:RegularExpressionValidator>
                      </td>
                  </tr>
                  <tr>
                      <td>
                          <wu:Label runat="server" ID="lblCPassword" Name="ConfirmPassword"></wu:Label>
                      </td>
                       <td>
                          <wu:TextBox runat="server" ID="txtCPassword" TextMode="Password" MaxLength="32"></wu:TextBox>
                      </td>
                       <td>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCPassword" ErrorMessage="old password required." ForeColor="Red"> *</asp:RequiredFieldValidator>
                           <asp:RegularExpressionValidator Display = "Dynamic" ControlToValidate = "txtCPassword" ID="RegularExpressionValidator6" ValidationExpression = "^[\s\S]{8,32}$" runat="server" ForeColor="Red" ErrorMessage="minimum 8 & maximum 32 characters required."></asp:RegularExpressionValidator>
                      </td>
                  </tr>
                  
                  <tr style="height:10px;">
                    <td style="align-items:center;" colspan="3" ></td>
                </tr>
                  <tr>
                       <td>
                           &nbsp;
                      </td>
                      <td colspan="2" style="float:right;">
                          <wu:Button ID="btnChange" runat="server" CssClass="Button" Name="Change" OnClientClick="return fnChangePwd();" OnClick="btnChange_Click"/>
                </td>
                  </tr>                 
              </table>
            </wu:Box>
          </div>
           <div id="divMangeUsers" class="LoginBox">
        <wu:Box ID="boxMangeUsers" runat="server"  Title="ChangeContact" TitleLanguageLabel="true" Dragable="true" >
            <table id="Table1" runat="server">                           
                   <%-- <tr>
                    <td>
                          <wu:Label ID="lblMFName" runat="server" Name="FirstName"></wu:Label>
                    </td>
                    <td>
                         <wu:TextBox ID="txtMFName" runat="server" MaxLength="250" Width="168px"></wu:TextBox>
                    </td>
                      <td>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMFName" ErrorMessage="first name is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                    </td>
                </tr>
                  <tr>
                    <td>
                          <wu:Label ID="lblMLName" runat="server" Name="LastName"></wu:Label>
                    </td>
                    <td>
                         <wu:TextBox ID="txtMLName" runat="server" MaxLength="250" Width="168px"></wu:TextBox>
                    </td>
                        <td>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMLName" ErrorMessage="last name is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                    </td>
                </tr>--%>
                <tr>
                    <td>
                          <wu:Label ID="lblMEmail" runat="server" Name="Email"></wu:Label>
                    </td>
                    <td>
                         <wu:TextBox ID="txtMEmail" runat="server" MaxLength="350" Width="168px"></wu:TextBox>
                    </td>
                     <td>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMEmail" ErrorMessage="email is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"  
            ControlToValidate="txtMEmail"  ErrorMessage="*"  ForeColor ="Red">  
        </asp:RegularExpressionValidator>  
                    </td>
                </tr>
                   <tr>
                    <td>
                          <wu:Label ID="lblMPhone" runat="server" Name="Phone"></wu:Label>
                    </td>
                    <td>
                        <wu:TextBox ID="txtMPhone" runat="server" TextMode="Phone" Width="176px" Height="24px" MaxLength="15"></wu:TextBox>
                      </td>
                        <td>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMPhone" ErrorMessage="phone is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator Display="Dynamic" ID="RegularExpressionValidator7" runat="server" ValidationExpression="^(?!411|911|0)\d{10}$"  
            ControlToValidate="txtMPhone"  ErrorMessage="please enter a valid phone"  ForeColor ="Red">  
        </asp:RegularExpressionValidator>
                    </td>
                </tr> 
                               
                <tr style="height:10px;">
                    <td style="align-items:center;" colspan="3" ></td>
                </tr>
                 <tr>                    
                    <td>
                           &nbsp;
                      </td>
                    <td style="float:right;" colspan="2">
                          <wu:Button runat="server"  ID="btnAccept"  Name="AcceptChanges" CssClass="Button"  OnClientClick="return fnContact();" Align="Right" OnClick="btnAccept_Click"></wu:Button>
                    </td>
                  </tr>
            </table>
            </wu:Box>
    </div>
          <div id="divEmail" class="LoginBox">
        <wu:Box ID="boxEmail" runat="server"  Title="ChangeEmail" TitleLanguageLabel="true" Dragable="true" >
            <table id="Table2" runat="server">  
                  <tr>
                    <td>
                          <wu:Label ID="lblOldEmail" runat="server" Name="oldEmail"></wu:Label>
                    </td>
                    <td>
                         <wu:TextBox ID="txtOldEmail" runat="server" MaxLength="250" Width="168px"></wu:TextBox>
                    </td>
                        <td>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtOldEmail" ErrorMessage="email is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                              <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"  
            ControlToValidate="txtOldEmail"  ErrorMessage="*"  ForeColor ="Red"> </asp:RegularExpressionValidator>  
                    </td>
                </tr>
                <tr>
                    <td>
                          <wu:Label ID="lblNEmail" runat="server" Name="newEmail"></wu:Label>
                    </td>
                    <td>
                         <wu:TextBox ID="txtNewEmail" runat="server" MaxLength="350" Width="168px"></wu:TextBox>
                    </td>
                     <td>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNewEmail" ErrorMessage="email is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"  
            ControlToValidate="txtNewEmail"  ErrorMessage="*"  ForeColor ="Red">  
        </asp:RegularExpressionValidator>  
                    </td>
                </tr>                                            
                <tr style="height:10px;">
                    <td style="align-items:center;" colspan="3" ></td>
                </tr>
                 <tr>                    
                    <td>
                           &nbsp;
                      </td>
                    <td style="float:right;" colspan="2">
                          <wu:Button runat="server"  ID="btnEmailAccept"  Name="AcceptChanges" CssClass="Button"  OnClientClick="return fnEmail();" Align="Right" OnClick="btnEmailAccept_Click"></wu:Button>
                    </td>
                  </tr>
            </table>
            </wu:Box>
    </div>
        </div>
</asp:content>
