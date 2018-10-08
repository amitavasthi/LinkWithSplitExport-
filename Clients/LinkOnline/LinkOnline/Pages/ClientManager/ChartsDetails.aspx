<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ChartsDetails.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.ChartsDetails" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
       <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Login.css" />
     <wu:StylesheetReference runat="server" Source="/Stylesheets/UserRole.css" />   
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
   
    </asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="ChartDetails"></wu:Label> 
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
                        <div class="BackButton" onclick="window.location='HomeManagement.aspx'"></div>
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
                <wu:Label ID="lblMessage" runat="server" Name="AddDetails" Font-Size="15pt" CssClass="Color1"></wu:Label>           
            </wu:TableCell>          
        </wu:TableRow>
             <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" Width="20%">
                <wu:Label ID="lblChartHeading" runat="server" Name="ChartHeading" CssClass="Color1"></wu:Label>    
            </wu:TableCell>
            <wu:TableCell  Width="30%" HorizontalAlign="Left">
                <wu:TextBox ID="txtCheading" runat="server" MaxLength="200"></wu:TextBox>
            </wu:TableCell>
            <wu:TableCell Width="50%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCheading" ErrorMessage="chart heading is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
            </wu:TableCell>
        </wu:TableRow>
      <wu:TableRow Height="45px">
          <wu:TableCell Width="20%" HorizontalAlign="Left">
              <wu:Label ID="lblCustomCharts" runat="server" Name="ChartURL" CssClass="Color1"></wu:Label>         
          </wu:TableCell>
        <wu:TableCell Width="30%" HorizontalAlign="Left">
               <wu:TextBox ID="txtChartURL" runat="server" MaxLength="250"></wu:TextBox>
        </wu:TableCell>    
        <wu:TableCell Width="50%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtChartURL" ErrorMessage="chart url required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
    </wu:TableCell>       
      </wu:TableRow>   
          <wu:TableRow Height="45px">
         <wu:TableCell Width="20%" HorizontalAlign="Left">
                <wu:Label ID="lblMultiple" runat="server" Name="multiple charts" CssClass="Color1" ></wu:Label>
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
                <wu:Label ID="Label1" runat="server" Name="EditDetails" Font-Size="15pt" CssClass="Color1"></wu:Label>           
            </wu:TableCell>
        </wu:TableRow>
             <wu:TableRow Height="45px">
            <wu:TableCell HorizontalAlign="Left" Width="20%">
                <wu:Label ID="lblMCHeading" runat="server" Name="ChartHeading" CssClass="Color1"></wu:Label>    
            </wu:TableCell>
            <wu:TableCell  Width="30%" HorizontalAlign="Left">
              <wu:TextBox ID="txtMCHeading" runat="server" MaxLength="200"></wu:TextBox>
            </wu:TableCell>
             <wu:TableCell Width="50%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMCHeading" ErrorMessage="chart heading is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
             </wu:TableCell>
        </wu:TableRow>
      <wu:TableRow Height="45px">
          <wu:TableCell Width="20%" HorizontalAlign="Left">
           <wu:Label ID="lblMCustomCharts" runat="server" Name="ChartURL" CssClass="Color1"></wu:Label>      
          </wu:TableCell>
        <wu:TableCell Width="30%" HorizontalAlign="Left">
              <wu:TextBox ID="txtMChartURL" runat="server" MaxLength="250"></wu:TextBox>
        </wu:TableCell>    
        <wu:TableCell Width="50%" HorizontalAlign="Left">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMChartURL" ErrorMessage="chart url required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
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
