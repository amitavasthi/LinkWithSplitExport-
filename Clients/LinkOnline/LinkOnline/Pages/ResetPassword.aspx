<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="LinkOnline.Pages.ResetPassword" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Main.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Login.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/ColorSchemeTemp.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Boxes.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Box.js"></wu:ScriptReference>
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
                        <div style="text-align: center;">
                            <wu:Label ID="lblPolicy" runat="server" Name="PasswordPolicyMsg" Font-Bold="false" Font-Size="10" ForeColor="#FFFFFF"></wu:Label>
                        </div>
                        <table style="margin-left: 0; margin-right: 0; margin-top: 10px;">
                            <tr>
                                <td colspan="3">
                                    <wu:Label ID="lblMessage" runat="server" Name="CreateMsg" Font-Bold="true" ForeColor="#FFFFFF"></wu:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <wu:Label ID="lblMsg" runat="server" Name="" Visible="false" ForeColor="Red"></wu:Label>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <wu:Label ID="lblNewPassword" runat="server" Name="NewPassword" ForeColor="#FFFFFF"></wu:Label>
                                </td>
                                <td style="float: left;">
                                    <wu:TextBox runat="server" ID="txtPassword" TextMode="Password" MaxLength="32"></wu:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" ErrorMessage="password is required." ForeColor="Red"> *</asp:RequiredFieldValidator>
                                </td>
                                <td rowspan="4" style="text-align: right; vertical-align: middle;">
                                    <wu:Button ID="btnLogin" runat="server" Name="Login" CssClass="LoginButton BackgroundColor2" OnClick="btnLogin_Click" TabIndex="3" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="text-align: left;">
                                    <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtPassword" ID="PasswordRegularExpressionValidator" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[*@$-+?_&=!%{}/])[A-Za-z\d*@$-+?_&=!%{}/]{8,32}" runat="server" ForeColor="Red" Font-Size="10" Font-Bold="true"></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <wu:Label runat="server" ID="lblCPassword" Name="ConfirmPassword" ForeColor="#FFFFFF"></wu:Label>
                                </td>
                                <td style="float: left;">
                                    <wu:TextBox runat="server" ID="txtCPassword" TextMode="Password" MaxLength="32"></wu:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCPassword" ErrorMessage="password is required." ForeColor="Red"> *</asp:RequiredFieldValidator>

                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="text-align: left">
                                    <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtCPassword" ID="CPasswordRegularExpressionValidator" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[*@$-+?_&=!%{}/])[A-Za-z\d*@$-+?_&=!%{}/]{8,32}" runat="server" ForeColor="Red" Font-Size="10" Font-Bold="true"></asp:RegularExpressionValidator>
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
