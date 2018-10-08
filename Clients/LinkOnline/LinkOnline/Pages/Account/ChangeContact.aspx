<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ChangeContact.aspx.cs" Inherits="LinkOnline.Pages.Account.ChangeContact" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>
<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
 
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
        <wu:Label ID="lblPageTitle" runat="server" Name="ChangeContact"></wu:Label>
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
     <wu:Table ID="tblContent"  runat="server" Width="50%" HorizontalAlign="Center" CellPadding="0" CellSpacing="0" CssClass = "Userroletable">
           <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" ColumnSpan="3" CssClass="Color1"> 
                <wu:Label ID="lblMessage" runat="server" Name="ChangeContact" Font-Size="15pt" CssClass="Color1"></wu:Label>           
            </wu:TableCell>
        </wu:TableRow>
           <%--  <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" Width="20%">
                  <wu:Label ID="lblMEmail" runat="server" Name="Email" CssClass="Color1"></wu:Label>    
            </wu:TableCell>
            <wu:TableCell  Width="30%" HorizontalAlign="Left">
                    <wu:TextBox ID="txtMEmail" runat="server" MaxLength="350" Width="168px"></wu:TextBox>
            </wu:TableCell>
             <wu:TableCell Width="50%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMEmail" ErrorMessage="email is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"  
            ControlToValidate="txtMEmail"  ErrorMessage="*"  ForeColor ="Red">  
        </asp:RegularExpressionValidator>  
            </wu:TableCell>
        </wu:TableRow>--%>
      <wu:TableRow Height="45px">
          <wu:TableCell Width="20%" HorizontalAlign="Left">
                <wu:Label ID="lblMPhone" runat="server" Name="Phone" CssClass="Color1"></wu:Label>       
          </wu:TableCell>
        <wu:TableCell Width="30%" HorizontalAlign="Left">
                <wu:TextBox ID="txtMPhone" runat="server" TextMode="Phone" Width="176px" Height="24px" MaxLength="15"></wu:TextBox>
        </wu:TableCell>    
        <wu:TableCell Width="50%" HorizontalAlign="Left">
                 <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMPhone" ErrorMessage="phone is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator Display="Dynamic" ID="RegularExpressionValidator7" runat="server"  ValidationExpression="^\+?(?:[0-9]●?){6,14}[0-9]$"   
            ControlToValidate="txtMPhone"  ErrorMessage="please enter a valid phone"  ForeColor ="Red">  
        </asp:RegularExpressionValidator>
    </wu:TableCell>       
    </wu:TableRow>         
    <wu:TableRow Height="70px">
          <wu:TableCell  HorizontalAlign="Center" ColumnSpan="3">
              <wu:Button runat="server"  ID="btnAccept"  Name="AcceptChanges" CssClass="Button"  OnClientClick="return fnContact();" Align="Right" OnClick="btnAccept_Click"></wu:Button>
         </wu:TableCell>
    </wu:TableRow>    
</wu:Table>
     </div> 
</asp:content>
