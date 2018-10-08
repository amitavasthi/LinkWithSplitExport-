<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ManageNews.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.ManageNews" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
      <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <script type="text/javascript">
        function fnCreateLoad() {
            if (Page_ClientValidate()) {
                ShowLoading(document.getElementById("tblContent"));
            }
            else {
                return false;
            }
        }
        function fnModifyLoad() {
            if (Page_ClientValidate()) {
                ShowLoading(document.getElementById("tblContent"));
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
   </style>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Login.css" />
     <wu:StylesheetReference runat="server" Source="/Stylesheets/UserRole.css" />   
    </asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="NewsDetails"></wu:Label>
    <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
  <div style="margin:1em;">      
    <div class="Color1" style="margin: 3%;">
            <table>
                <tr>
                    <td>
                        <div class="BackButton" onclick="window.location='NewsDetails.aspx'"></div>
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
                <wu:Label ID="lblMessage" runat="server" Name="AddNews" Font-Size="15pt" CssClass="Color1"></wu:Label>           
            </wu:TableCell>
        </wu:TableRow>
             <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" Width="20%">
                 <wu:Label ID="lblnewsHeadline" runat="server" Name="newsHeading" CssClass="Color1"></wu:Label>    
            </wu:TableCell>
            <wu:TableCell  Width="30%" HorizontalAlign="Left">
                  <wu:TextBox ID="txtnewsHeading" runat="server" MaxLength="250" Width="173px"></wu:TextBox>
            </wu:TableCell>
             <wu:TableCell Width="50%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtnewsHeading" ErrorMessage="news heading is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
            </wu:TableCell>
        </wu:TableRow>
      <wu:TableRow Height="45px">
          <wu:TableCell Width="20%" HorizontalAlign="Left">
               <wu:Label ID="lblDescription" runat="server" Name="Description" CssClass="Color1"></wu:Label>         
          </wu:TableCell>
        <wu:TableCell Width="30%" HorizontalAlign="Left">
                <asp:TextBox ID="txtDescription" runat="server" MaxLength="1000" TextMode="MultiLine" Width="178px" Rows="5"></asp:TextBox>
        </wu:TableCell>    
        <wu:TableCell Width="50%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription" ErrorMessage="news description required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
    </wu:TableCell>       
      </wu:TableRow>   
          <wu:TableRow Height="45px">
         <wu:TableCell Width="20%" HorizontalAlign="Left">
                <wu:Label ID="lblMultiple" runat="server" Name="multiple news" CssClass="Color1" ></wu:Label>
          </wu:TableCell>
            <wu:TableCell Width="30%" HorizontalAlign="Left">
              <wu:CheckBox runat="server" ID="chkMultiple" />
          </wu:TableCell>
         <wu:TableCell Width="50%" HorizontalAlign="Left">      
                 </wu:TableCell>    
    </wu:TableRow>           
    <wu:TableRow Height="70px">
          <wu:TableCell  HorizontalAlign="Center" ColumnSpan="3">
          <wu:Button runat="server"  ID="btnSaveDetails"  Name="Save" CssClass="Button" Align="Right" OnClick="btnSaveDetails_Click" OnClientClick="return fnCreateLoad();"></wu:Button>
         </wu:TableCell>
    </wu:TableRow>    
</wu:Table>
     </div> 
       <div style="margin-top:-30px;margin-left:150px;" runat="server" id="divModify">
     <wu:Table ID="Table1"  runat="server" Width="50%" HorizontalAlign="Center" CellPadding="0" CellSpacing="0" CssClass = "Userroletable">
           <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" ColumnSpan="3" CssClass="Color1"> 
                <wu:Label ID="Label1" runat="server" Name="EditNews" Font-Size="15pt" CssClass="Color1"></wu:Label>           
            </wu:TableCell>          
        </wu:TableRow>
             <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" Width="20%">
                <wu:Label ID="lblMNHeading" runat="server" Name="newsHeading" CssClass="Color1"></wu:Label>    
            </wu:TableCell>
            <wu:TableCell  Width="30%" HorizontalAlign="Left">
             <wu:TextBox ID="txtMHeading" runat="server" MaxLength="250" Width="173px"></wu:TextBox>
            </wu:TableCell>
             <wu:TableCell Width="50%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMHeading" ErrorMessage="news heading is required." ForeColor ="Red"   display="Dynamic" > *</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator 
                       runat="server" ControlToValidate ="txtMHeading"
                       ErrorMessage="Must be a valid" ForeColor ="Red"   display="Dynamic"  ValidationExpression="^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]+$"
                       >
                </asp:RegularExpressionValidator>
            </wu:TableCell>
        </wu:TableRow>
      <wu:TableRow Height="45px">
          <wu:TableCell Width="20%" HorizontalAlign="Left">
          <wu:Label ID="lblMDesc" runat="server" Name="Description" CssClass="Color1"></wu:Label>      
          </wu:TableCell>
        <wu:TableCell Width="30%" HorizontalAlign="Left">
             <asp:TextBox ID="txtMDescription" runat="server" MaxLength="1000" TextMode="MultiLine" Rows="5" Width="178px"></asp:TextBox>
        </wu:TableCell>    
        <wu:TableCell Width="50%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMDescription" ErrorMessage="news description required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
    </wu:TableCell>       
      </wu:TableRow>          
    <wu:TableRow Height="70px">
          <wu:TableCell  HorizontalAlign="Center" ColumnSpan="3">
            <wu:Button runat="server"  ID="btnModify"  Name="Save" CssClass="Button"  Align="Right" OnClick="btnModify_Click"  OnClientClick="return fnModifyLoad();"></wu:Button>
          </wu:TableCell>
    </wu:TableRow>    
</wu:Table>
     </div>   
  </div>
</asp:content>
