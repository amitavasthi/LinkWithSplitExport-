<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ClientSettings.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.ClientSettings" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
   <script type="text/javascript" src="/Scripts/jquery.js"></script>
    <script type="text/javascript" src="/Scripts/Controls/colorpicker.js"></script>
   <wu:StylesheetReference runat="server" Source="/Stylesheets/Controls/colorpicker.css" />
    <wu:StylesheetReference runat="server" Source="/Stylesheets/layout.css" />
      <wu:StylesheetReference Source="/Stylesheets/Main.css" runat="server" />
    <script>
        function fnModifyLoad() {
            if (Page_ClientValidate("g1")) {
                ShowLoading(document.getElementById("tblContent"));
            }
            else {
                return false;
            }
        }
    </script>
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="PortalSettings"></wu:Label>
         <span class="Beta">
            <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
        </span>
    </h1>
</asp:content>

<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
  <div style="margin:1em;">      
 <div class="Color1" style="margin-top:10px;">
            <table>
                <tr>
                    <td>
                        <div class="BackButton" onclick="window.location='ClientManagerHome.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
         <wu:Box Visible="false" ID="boxLogo" runat="server"  Title="ChangeClientLogo" Dragable="true">
          <wu:Table ID="tblChangeLogo"   runat="server" Width="300px">
       <wu:TableRow>
     <wu:TableCell Width="50%" >
            <wu:Label Name="SelectImage" runat="server"/>
     </wu:TableCell>
      <wu:TableCell HorizontalAlign="Left" Width="50%" Font-Size="X-Small" ColumnSpan="2">
          <asp:FileUpload ID="fileUploadImage" runat="server" Width="200px"></asp:FileUpload>
          <asp:RegularExpressionValidator  ControlToValidate ="fileUploadImage"  ValidationGroup="g1" runat="server" ErrorMessage="Must be of PNG|JPG|GIF" ForeColor ="Red" ValidationExpression="([a-zA-Z0-9\s_\\.\-:()])+(.png|.jpg|.gif)$"></asp:RegularExpressionValidator>
     </wu:TableCell>
 </wu:TableRow>
               <wu:TableRow>
     <wu:TableCell ColumnSpan="2" HorizontalAlign="Right">
         <div style="margin-top:0px;margin-bottom:0px;">
         <wu:Button ID="btnUpdateImage" Name="Save" runat="server" CssClass="Button" Width="80px" BorderStyle="None"  OnClick="btnUpdateImage_Click"  ValidationGroup="g1" OnClientClick="return fnModifyLoad();"/>
             </div>
     </wu:TableCell>      
 </wu:TableRow>
          </wu:Table>
    </wu:Box>
  <div style="margin-top:-10px;">
<wu:Table ID="tblContent"  runat="server" Width="40%" HorizontalAlign="Center">
     <wu:TableRow Height="60px">
        <wu:TableCell HorizontalAlign="Center" ColumnSpan="2">
            <wu:Label ID="lblMsg" runat="server" Visible="False" Name="SuccessMsg"/>             
          </wu:TableCell>
    </wu:TableRow>
    <wu:TableRow Height="60px">
        <wu:TableCell HorizontalAlign="Left">
            <wu:Label Name="uploadclientlogo" ID="lblClientlogo" CssClass="Color1" runat="server" />             
          </wu:TableCell>
          <wu:TableCell>
                          <wu:ImageButton ID="imgbtnChangeLogo" CssClass="ImageButton" runat="server" OnClick="btnChangeLogo_Click" />
                        </wu:TableCell>
    </wu:TableRow>
      <wu:TableRow Height="60px">
          <wu:TableCell Width="50%" HorizontalAlign="Left">
                <wu:Label Name="MainColor" ID="lblColor1" CssClass="Color1"   runat="server" />
          </wu:TableCell>
            <wu:TableCell Width="50%">
                 <div id="colorSelector"><div style="background-color: #6CAEE0" runat="server" id="divMainColor"></div></div>      
              <asp:HiddenField ID="colorpickerField1" runat="server" value="" />
                  <script type="text/javascript">
                      $('#colorSelector').ColorPicker({
                          color: '#0000ff',
                          onShow: function (colpkr) {
                              $(colpkr).fadeIn(500);
                              return false;
                          },
                          onHide: function (colpkr) {
                              $(colpkr).fadeOut(500);
                              return false;
                          },
                          onChange: function (hsb, hex, rgb) {
                              $('#colorSelector div').css('backgroundColor', '#' + hex);
                              $('#cphContent_colorpickerField1').val(hex);
                          },
                          onSubmit: function (hsb, hex, rgb, el) {
                              $(el).val(hex);
                              $(el).ColorPickerHide();
                              $('#cphContent_colorpickerField1').val(hex);
                              $('#colorSelector div').css('backgroundColor', '#' + hex);
                          },
                      });
                </script>
          </wu:TableCell>         
      </wu:TableRow>
    <wu:TableRow Height="60px">
         <wu:TableCell Width="50%" HorizontalAlign="Left">
                <wu:Label Name="SecondaryColor" ID="lblColor2" CssClass="Color1"  runat="server" />
          </wu:TableCell>
            <wu:TableCell Width="50%">
                 <div id="colorSelectorFirst"><div style="background-color: #6CAEE0"  runat="server" id="div2ndColor"></div></div> 
              <asp:HiddenField ID="colorpickerField2" runat="server" value="" />
                                  <script type="text/javascript">
                                      $('#colorSelectorFirst').ColorPicker({
                                          color: '#0000ff',
                                          onShow: function (colpkr) {
                                              $(colpkr).fadeIn(500);
                                              return false;
                                          },
                                          onHide: function (colpkr) {
                                              $(colpkr).fadeOut(500);
                                              return false;
                                          },
                                          onChange: function (hsb, hex, rgb) {
                                              $('#colorSelectorFirst div').css('backgroundColor', '#' + hex);
                                              $('#cphContent_colorpickerField2').val(hex);
                                          },
                                          onSubmit: function (hsb, hex, rgb, el) {
                                              $(el).val(hex);
                                              $(el).ColorPickerHide();
                                              $('#cphContent_colorpickerField2').val(hex);
                                              $('#colorSelectorFirst div').css('backgroundColor', '#' + hex);
                                          },
                                      });
                </script>
          </wu:TableCell>
    </wu:TableRow>
    <%--<wu:TableRow Height="60px">
         <wu:TableCell Width="50%" HorizontalAlign="Center">
                <wu:Label Name="EnableCustomCharting" ID="lblCustomCharting"   runat="server" />
          </wu:TableCell>
        <wu:TableCell Width="50%">
                <wu:CheckBox runat="server" ID="chkCustomCharting" Checked="true" />
          </wu:TableCell>
    </wu:TableRow>--%>
    <wu:TableRow Height="70px">
          <wu:TableCell  HorizontalAlign="Center" ColumnSpan="2">
         <wu:Button ID="btnSave" Name="Acceptchanges" runat="server" CssClass="Button" OnClick="btnSave_Click"></wu:Button>     
             <wu:Button ID="btnCancel" Name="Reset" runat="server" CssClass="Button"  OnClick="btnCancel_Click"> </wu:Button>
         </wu:TableCell>
    </wu:TableRow>    
</wu:Table>
          </div>
 
        </div>
</asp:content>
