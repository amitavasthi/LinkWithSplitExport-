<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ChartsHome.aspx.cs" Inherits="LinkOnline.Pages.CustomCharts.ChartsHome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
       <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/LinkCloud.css"></wu:StylesheetReference>
</asp:Content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="custom charts"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
     <%-- <table cellspacing="0" cellpadding="0">
        <tr valign="top">
            <td rowspan="2">
               <asp:Panel ID="pnlExplorer" runat="server" CssClass="FileExplorer BorderColor1"></asp:Panel>
            </td>
            <td style="height:100%;width:100%;">
                <asp:Panel ID="pnlExplorerNavigation" runat="server" CssClass="FileExplorerNavigation BackgroundColor5"></asp:Panel>
                <asp:Panel ID="pnlFiles" runat="server" CssClass="Files" ondrop="UploadFile(e);" ondragover="this.className='Files BackgroundColor9';" ondragend="this.className='Files';"></asp:Panel>
            </td>
        </tr>
    </table>--%>
     <div style="margin:1em;">
         <div class="Color1" style="margin: 3%;">
            <table>
                <tr>
                    <td>
                        <div class="BackButton" onclick="window.location='/Pages/Default.aspx'"></div>
                    </td>
                    <td style="padding-left:20px;">
                    <%--    <wu:ImageButton runat="server" ID="btnAdd" ImageUrl="/Images/Icons/Cloud/NewDirectory.png" OnClick="btnAdd_Click" />--%>
                     </td>
                </tr>
            </table>
        </div>
          <div style="margin-top:40px;">
            <wu:Table ID="tblAdmin" runat="server" HorizontalAlign="Center" CellPadding="2" CellSpacing="2">
                 <wu:TableRow>
                      <wu:TableCell ColumnSpan="2" HorizontalAlign="Center">
                            &nbsp;
                      </wu:TableCell>
                  </wu:TableRow>
                 <wu:TableRow>
                     <wu:TableCell HorizontalAlign="Left">
                         
                     </wu:TableCell>
                 </wu:TableRow>
                 <wu:TableRow>
                 <wu:TableCell>
                    <asp:Panel ID="pnlFiles" runat="server" CssClass="Files" ondrop="UploadFile(e);" ondragover="this.className='Files BackgroundColor9';" ondragend="this.className='Files';"></asp:Panel>
                 </wu:TableCell>
                     </wu:TableRow>
             </wu:Table>
              </div>
         </div>
</asp:Content>
