<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="LinkOnline.Pages.ErrorPage" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"> <wu:Label ID="Label1" runat="server" Name="application error"></wu:Label></h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
     <div style="margin:1em;">
           <%--  <div class="Color1">
   <wu:Label ID="lblPageTitle" runat="server" Name="application error" CssClass="TitleText"></wu:Label>
             </div> --%>     
          <div>
              <wu:Table runat="server"  ID="tblMsgTable" HorizontalAlign="Center">                
                    <wu:TableRow>
                     <wu:TableCell ColumnSpan="3" HorizontalAlign="Center" Height="30px">                 
                     <wu:Label ID="lblAppError" runat="server" Name="ApplicationError"  Visible="true" Font-Bold="true" ForeColor="Red"></wu:Label>
                    </wu:TableCell>
        </wu:TableRow>
                    <wu:TableRow>
                     <wu:TableCell ColumnSpan="3" HorizontalAlign="Center" Height="30px">                 
                     <wu:Label ID="lblError" runat="server" Name="ApplicationError"  Visible="true"></wu:Label>
                    </wu:TableCell>
        </wu:TableRow>
    </wu:Table>
   </div>
         </div>
</asp:content>

