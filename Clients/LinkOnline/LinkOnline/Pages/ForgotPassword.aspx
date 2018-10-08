<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="LinkOnline.Pages.ForgotPassword" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Main.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Login.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/ColorSchemeTemp.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Home.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js"></wu:ScriptReference>
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
                                    <wu:Label ID="lblMessage" runat="server" Name="ForgotMsg"></wu:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <wu:Label ID="lblMsg" runat="server" Name="" Visible="false"></wu:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <wu:Label ID="lblEmail" runat="server" Name="email address" ForeColor="#FFFFFF"></wu:Label>
                                </td>
                                <td style="float: left;">&nbsp;&nbsp;&nbsp;&nbsp;<wu:TextBox ID="txtMail" runat="server" TabIndex="1" Button="btnGo"></wu:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMail" ErrorMessage="email is required." ForeColor="Red"> *</asp:RequiredFieldValidator>
                                    <%--<asp:RegularExpressionValidator ID="RegularExpressionValidatorMail" runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                        ControlToValidate="txtMail" ErrorMessage="*" ForeColor="Red">  
                                    </asp:RegularExpressionValidator>--%>
                                    <wu:Button ID="btnGo" runat="server" Name="Login" CssClass="Button BackgroundColor2" Font-Bold="true" Height="40px" Width="70px" OnClick="btnGo_Click" TabIndex="3" />
                                </td>
                                <%-- <td>
                                    <wu:Button ID="btnGo" runat="server" Name="Login" CssClass="Button BackgroundColor2" Font-Bold="true" Height="40px" Width="70px" OnClick="btnGo_Click" TabIndex="3" />
                                </td>--%>
                            </tr>
                            <tr>
                                <td colspan="2" align="center">
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidatorMail" runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                        ControlToValidate="txtMail" ForeColor="Red">  
                                    </asp:RegularExpressionValidator>
                                </td>
                            </tr>
                        </table>
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
