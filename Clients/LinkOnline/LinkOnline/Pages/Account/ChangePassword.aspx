<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="LinkOnline.Pages.ChangePassword" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>
<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
       <link href="/Stylesheets/Main.css" rel="stylesheet" />  
    <script type="text/javascript">
        function fnChangePwd() {
            if (Page_ClientValidate()) {
                ShowLoading(document.getElementById("tblContent"));
            }
            else {
                return false;
            }
        }
        function fnContact() {
            if (Page_ClientValidate()) {
                ShowLoading(document.getElementById("tblContent"));
            }
            else {
                return false;
            }
        }
        function fnEmail() {
            if (Page_ClientValidate()) {
                ShowLoading(document.getElementById("tblContent"));
            }
            else {
                return false;
            }
        }
    </script> 
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1">
        <wu:Label ID="lblPageTitle" runat="server" Name="ChangePassword"></wu:Label>        
        <span class="Beta">
            <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
        </span>
    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <div class="Color1" style="margin:3%;">
            <table>
                <tr>
                    <td>
                        <div class="BackButton" onclick="window.location='/Pages/Account/Home.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
     <div style="margin-top:-30px;margin-left:150px;" runat="server" id="divCreate">
     <wu:Table ID="tblContent"  runat="server" Width="50%" HorizontalAlign="Center" CellPadding="0" CellSpacing="0">
           <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" ColumnSpan="3" CssClass="Color1"> 
                <wu:Label ID="lblMessage" runat="server" Name="ChangePassword" Font-Size="15pt" CssClass="Color1"></wu:Label>           
            </wu:TableCell>
        </wu:TableRow>
           <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" Width="20%">
                 <wu:Label runat="server" ID="lblOldPassword" Name="OldPassword" CssClass="Color1"></wu:Label>    
            </wu:TableCell>
            <wu:TableCell  Width="25%" HorizontalAlign="Left">
                   <wu:TextBox runat="server" ID="txtOldPassword" TextMode="Password" MaxLength="32" ></wu:TextBox>
            </wu:TableCell>
             <wu:TableCell Width="55%" HorizontalAlign="Left">
                 <asp:RequiredFieldValidator runat="server" ControlToValidate="txtOldPassword" ErrorMessage="old password required." ForeColor="Red"> *</asp:RequiredFieldValidator>
                       <%-- <asp:RegularExpressionValidator Display = "Dynamic" ControlToValidate = "txtOldPassword" ID="RegularExpressionValidator4" ValidationExpression = "^[\s\S]{8,32}$" runat="server" ForeColor="Red" ErrorMessage="minimum 8 & maximum 32 characters required."></asp:RegularExpressionValidator>--%>
            </wu:TableCell>
        </wu:TableRow>
      <wu:TableRow Height="45px">
          <wu:TableCell Width="20%" HorizontalAlign="Left">
                <wu:Label runat="server" ID="lblPassword" Name="NewPassword" CssClass="Color1"></wu:Label>         
          </wu:TableCell>
        <wu:TableCell Width="25%" HorizontalAlign="Left">
                <wu:TextBox runat="server" ID="txtPassword" TextMode="Password" MaxLength="32"></wu:TextBox>
        </wu:TableCell>    
        <wu:TableCell Width="55%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" ErrorMessage="new password required." ForeColor="Red"> *</asp:RequiredFieldValidator>
                           <asp:RegularExpressionValidator Display = "Dynamic" ControlToValidate = "txtPassword" ID="PasswordRegularExpressionValidator" ValidationExpression = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[*@$-+?_&=!%{}/])[A-Za-z\d*@$-+?_&=!%{}/]{8,32}" runat="server" ForeColor="Red"></asp:RegularExpressionValidator>
    </wu:TableCell>       
      </wu:TableRow>   
          <wu:TableRow Height="45px">
         <wu:TableCell Width="20%" HorizontalAlign="Left">
                 <wu:Label runat="server" ID="lblCPassword" Name="ConfirmPassword" CssClass="Color1" ></wu:Label>
          </wu:TableCell>
            <wu:TableCell Width="25%" HorizontalAlign="Left">
               <wu:TextBox runat="server" ID="txtCPassword" TextMode="Password" MaxLength="32"></wu:TextBox>
          </wu:TableCell>
         <wu:TableCell Width="55%" HorizontalAlign="Left">   
               <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCPassword" ErrorMessage="confirm password required." ForeColor="Red"> *</asp:RequiredFieldValidator>
                           <asp:RegularExpressionValidator Display = "Dynamic" ControlToValidate = "txtCPassword" ID="CPasswordRegularExpressionValidator" ValidationExpression = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[*@$-+?_&=!%{}/])[A-Za-z\d*@$-+?_&=!%{}/]{8,32}" runat="server" ForeColor="Red"></asp:RegularExpressionValidator>   
                 </wu:TableCell>    
    </wu:TableRow> 
          <wu:TableRow Height="45px">
         <wu:TableCell HorizontalAlign="Left" ColumnSpan="3">
                <wu:Label runat="server" ID="Label1" Name="PasswordNote" CssClass="Color1" ></wu:Label>
          </wu:TableCell>              
    </wu:TableRow>           
    <wu:TableRow Height="70px">
          <wu:TableCell  HorizontalAlign="Center" ColumnSpan="3">
            <wu:Button ID="btnChange" runat="server" CssClass="Button" Name="Change" OnClientClick="return fnChangePwd();" OnClick="btnChange_Click"/>
         </wu:TableCell>
    </wu:TableRow>    
</wu:Table>
     </div> 
</asp:content>
