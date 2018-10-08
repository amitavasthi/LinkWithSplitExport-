<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ModifyUser.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.ModifyUser" %>

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
                ShowLoading(document.getElementById("boxAddUserControl"));
            }
            else {
                return false;
            }
        }

        loadFunctions.push(InitBoxes);

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
   <%-- <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Login.css" />
    <wu:StylesheetReference runat="server" Source="/Stylesheets/UserRole.css" />  --%>
   
    </asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="ModifyUser"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>

<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
  <div class="container-fluid">
  <div class="row" style="margin:1em;">      
    <div class="col-xs-12 Color1" style="margin: 3%;">
            <table>
                <tr>
                    <td>
                        <div class="BackButton" onclick="window.location='ManageUsers.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
     </div>
     <div style="margin-top:-30px;width:1000px;margin:auto;">
         <table style="width:100%;">
             <tr valign="top">
                 <td style="width:50%">
                     <wu:Table ID="tblContent"  runat="server" Width="100%" HorizontalAlign="Center" CellPadding="0" CellSpacing="0">
           <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" ColumnSpan="3" CssClass="Color1">
                <wu:Label ID="lblMessage" runat="server" Name="ModifyUser" Font-Size="15pt" CssClass="Color1"></wu:Label>           
            </wu:TableCell>
          
        </wu:TableRow>
             <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" Width="20%">
                <wu:Label ID="lblUserName" runat="server" Name="UserName" CssClass="Color1"></wu:Label>           
            </wu:TableCell>
            <wu:TableCell  Width="30%" HorizontalAlign="Left">
                <wu:TextBox ID="txtName" runat="server" MaxLength="255" AutoCompleteType="Office" Width="170px" Height="34px"></wu:TextBox>
            </wu:TableCell>
             <wu:TableCell Width="50%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" ErrorMessage="password is required." ForeColor="Red"> *</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator  ControlToValidate="txtName" ID="RegularExpressionValidator3" ValidationExpression="^[a-zA-Z0-9@._-]{3,255}$" runat="server" ForeColor="Red" ErrorMessage="please verify the name you entered."></asp:RegularExpressionValidator>
            </wu:TableCell>
        </wu:TableRow>
      <wu:TableRow Height="45px">
          <wu:TableCell Width="20%" HorizontalAlign="Left">
                <wu:Label ID="lblFName" runat="server" Name="FirstName" CssClass="Color1"></wu:Label>             
          </wu:TableCell>
        <wu:TableCell Width="30%" HorizontalAlign="Left">
                <wu:TextBox ID="txtFirstName" runat="server" MaxLength="255" Width="170px" Height="34px"></wu:TextBox>
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
           <wu:TextBox ID="txtLastName" runat="server" MaxLength="255" Width="170px"  Height="34px"></wu:TextBox>
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
           <wu:TextBox ID="txtMail" runat="server" MaxLength="255" Width="170px"  Height="34px"></wu:TextBox>
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
           <wu:TextBox ID="txtPhone" runat="server" MaxLength="50" Width="170px"  Height="34px"></wu:TextBox>
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
                <wu:DropDownList runat="server" ID="ddlRole" Width="170px" Height="32px"/>
          </wu:TableCell>
         <wu:TableCell Width="50%" HorizontalAlign="Left">
         <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlRole" ErrorMessage="user group is required." ForeColor ="Red" InitialValue="select"> *</asp:RequiredFieldValidator> 
                 </wu:TableCell>    
    </wu:TableRow>
</wu:Table>
                 </td>
                 <td>
                     <wu:Label ID="lblUserWorkgroupsTitle" runat="server" Name="UserWorkgroupsTitle" Font-Size="15pt" CssClass="Color1"></wu:Label>           
                     <asp:Panel ID="pnlUserWorkgroups" runat="server" style="height: 280px;overflow:auto;">

                     </asp:Panel>
                 </td>
             </tr>
             <tr>
                 <td colspan="2" align="right">
                     <wu:Button runat="server"  ID="btnAcceptChanges"  Name="AcceptChanges" CssClass="Button"  OnClientClick="return fnCreateLoad();" Align="Left" OnClick="btnAcceptChanges_Click"></wu:Button>
                 </td>
             </tr>
         </table>
     
     </div>   
  </div>
        </div>
</asp:content>
