<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PasswordAssistance.aspx.cs" Inherits="LinkOnline.Pages.PasswordAssistance" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Main.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Login.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/ColorSchemeTemp.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:ScriptReference runat="server" Source="/Scripts/Boxes.js" />
</head>
<body>
    <form id="form1" runat="server">
        <table style="height: 100%; width: 100%;">
            <tr>
                <td>
                    <div class="Headline">
                        <wu:Image ID="imgLogo" runat="server" ImageUrl="/Images/Logos/Link.png"></wu:Image>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="height: 100%;" align="center">
                    <div class="LoginBox BackgroundColor1">
                        <span class="LoginText">
                            <wu:Label ID="lblLoginText" runat="server" Name="ForgotHeading"></wu:Label>
                        </span>
                        <table style="margin-left: 6em; margin-right: 6em; margin-top: 10px;">
                            <tr>
                                <td colspan="2">
                                    <wu:Label ID="lblMessage" runat="server" Name="" Font-Bold="true" ForeColor="#FFFFFF" Visible="false"></wu:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <wu:Label ID="lblMsg" runat="server" Name="" Visible="false" ForeColor="Red"></wu:Label>
                                </td>
                            </tr>
                           
                        </table>
                        <div class="ForgotPassword" style="margin-top: 15px; float: right; vertical-align: top; color: #FFFFFF;">
                            <wu:Button ID="btnForgot"  runat="server" CssClass="LoginBtn BackgroundColor1" Name="login" OnClick="btnLogin_Click" />
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="Footer">
                        <wu:Image ID="imgCopyright" runat="server" ImageUrl="/Images/Logos/Blueocean.png"></wu:Image>
                    </div>
                </td>
            </tr>
        </table>

    </form>
</body>
</html>

