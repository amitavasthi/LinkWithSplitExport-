<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="NewsDetails.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.NewsDetails" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
      <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
     <wu:StylesheetReference runat="server" Source="/Stylesheets/UserRole.css" />
       <script type="text/javascript">
           function fnCreateLoad() {
               if (Page_ClientValidate()) {
                   ShowLoading(document.getElementById("boxAddNewsControl"));
               }
               else {
                   return false;
               }
           }
           function fnModifyLoad() {
               if (Page_ClientValidate()) {
                   ShowLoading(document.getElementById("boxModifyControl"));
               }
               else {
                   return false;
               }
           }

    </script>
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
       <div class="Color1" style="height:1px;">
             </div>
         <div class="Color1" style="margin-top: 2%;margin-left:3%;">
            <table>
                <tr>
                    <td>
                        <div class="BackButton" onclick="window.location='HomeScreenManagement.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
           <div class="Color1" style="height: 20px;">
            <wu:ConfirmBox ID="cbRemove" runat="server"></wu:ConfirmBox>
        </div>
         <div style="margin-left:70px;margin-top:15px;">
             <div style="margin-left:450px;margin-top:15px;">
                   <wu:Label ID="lblError" runat="server" Name="" Visible="false"></wu:Label>
             </div>
             <asp:Panel id="pnlnewsManagement" runat="server"></asp:Panel>
        </div>
        <div style="height:25px;float:right;margin-top:10px;">
      <wu:Table runat="server" Visible="true" ID="tblClient" Width="40%"  HorizontalAlign="Right">
       <wu:TableRow>
           <wu:TableCell>
             <wu:Button runat="server"  ID="btnModify"  Name="Modify" CssClass="Button"  OnClick="btnModify_Click" />
           </wu:TableCell>
           <wu:TableCell>
             <wu:Button runat="server"  ID="btnAdd"  Name="Create" CssClass="Button"   OnClick="btnAdd_OnClick" />
           </wu:TableCell>
            <wu:TableCell>
             <wu:Button runat="server"  ID="btnDelete"  Name="Delete" CssClass="Button"   OnClick="btnDelete_OnClick" />
           </wu:TableCell>           
       </wu:TableRow>
          </wu:Table>
            </div>
          <div id="divAdd">
        <wu:Box ID="boxAddNews" runat="server"  Title="AddNews" TitleLanguageLabel="true" Dragable="true" >
            <table id="tblAdd" runat="server">
                 <tr>
                    <td colspan="3" style="align-content:center;text-align:center;font-style:normal">                       
                        <wu:Label ID="lblDuplicateErr" runat="server" Name="DuplicateError" Visible="false"></wu:Label>
                    </td>                   
                </tr>
                 <tr>
                    <td>
                          <wu:Label ID="lblnewsHeadline" runat="server" Name="newsHeading" ForeColor="#000000"></wu:Label>
                    </td>
                    <td>
                      <wu:TextBox ID="txtnewsHeading" runat="server" MaxLength="250" AutoCompleteType="Office" Width="173px"></wu:TextBox>
                      </td>
                        <td>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtnewsHeading" ErrorMessage="heading is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                    </td>
                </tr>  
                  <tr>
                    <td>
                          <wu:Label ID="lblDescription" runat="server" Name="Description" ForeColor="#000000" autocomplete="off"></wu:Label>
                    </td>
                    <td>
                      <wu:TextBox ID="txtDescription" runat="server" MaxLength="1000" AutoCompleteType="Office" TextMode="MultiLine" Width="178px" Rows="5"></wu:TextBox>
                      </td>
                        <td>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription" ErrorMessage="description is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                    </td>
                </tr>  
                                
                <tr style="height:10px;">
                    <td style="align-items:center;" colspan="3" ></td>
                </tr>
                 <tr>
                    <td>
                           &nbsp;
                      </td>
                    <td style="float:right;">
                          <wu:Button runat="server"  ID="btnCreate"  Name="Create" CssClass="Button"  OnUserClick="return fnCreateLoad();" Align="Right" OnClick="btnCreate_OnClick"></wu:Button>
                    </td>
                  </tr>
            </table>
            </wu:Box>
                 <wu:Box ID="boxModify" runat="server"  Title="EditNews" TitleLanguageLabel="true" Dragable="true" >
            <table id="tblModify" runat="server">
                
                 <tr>
                    <td>
                          <wu:Label ID="lblMNHeading" runat="server" Name="newsHeading" ForeColor="#000000"></wu:Label>
                    </td>
                    <td>
                      <wu:TextBox ID="txtMHeading" runat="server" MaxLength="250" AutoCompleteType="Office" Width="173px"></wu:TextBox>
                      </td>
                        <td>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMHeading" ErrorMessage="heading is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                    </td>
                </tr>  
                  <tr>
                    <td>
                          <wu:Label ID="lblMDesc" runat="server" Name="Description" ForeColor="#000000" autocomplete="off"></wu:Label>
                    </td>
                    <td>
                      <wu:TextBox ID="txtMDescription" runat="server" MaxLength="1000" TextMode="MultiLine" AutoCompleteType="Office"  Width="178px" Rows="5"></wu:TextBox>
                      </td>
                        <td>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMDescription" ErrorMessage="description is required." ForeColor ="Red"> *</asp:RequiredFieldValidator>
                    </td>
                </tr>  
                                
                <tr style="height:10px;">
                    <td style="align-items:center;" colspan="3" ></td>
                </tr>
                 <tr>
                    <td>
                           &nbsp;
                      </td>
                    <td style="float:right;">
                          <wu:Button runat="server"  ID="btnSave"  Name="Save" CssClass="Button"  OnUserClick="return fnModifyLoad();" Align="Right" OnClick="btnSave_OnClick"></wu:Button>
                    </td>
                  </tr>
            </table>
            </wu:Box>
    </div>
    </div>
</asp:content>
