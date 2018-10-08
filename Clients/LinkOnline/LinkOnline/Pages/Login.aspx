<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LinkOnline.Pages.Login" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="icon" type="image/png" href="/favicon.png">
    <title>Link | Reporting & Analytics</title>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Main.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Login.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/ColorSchemeTemp.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:ScriptReference runat="server" Source="/Scripts/Controls/Boxes.js" />
    <wu:ScriptReference runat="server" Source="/Scripts/Ajax.js" />
    <style>
        body{
               background: url(../images/loginPage/image.png) no-repeat;
               background-position: center;
               background-size: 75%;
        }
        .LoginBox{
            background-color:rgb(3,78,162);
        }
        #btnForgot{
                background: transparent;
        }
        #lblLoginText{
            text-transform: capitalize;
        }
        #btnForgot{
            text-transform: capitalize;
        }

         #btnForgot:focus{
            outline:none;
        }
         	.input_Txt{
		background: #FFF1D0;
		}
        .input_Txt:focus{
            outline:none;
        }

        #imgLogo {
         background: url(/Images/loginPage/linklogo.png) no-repeat;
    height: 70px;
    width: 195px;
    display: inline-block;
    background-size: cover;
        }

        #imgCopyright {
      background: url(/Images/loginPage/C5logo.png) no-repeat;
    height: 55px;
    width: 200px;
    display: inline-block;
    background-size: cover;
        }
        .Footer {
    text-align: inherit;
    margin-left: 50px;
}
        #txt_content{
     float: right;
    margin-right: 50px;
    text-decoration: none;
    color: #000;
    font-weight: bold;
    cursor: default;
    position: relative;
    top: 30px;
        }

input:-webkit-autofill,
input:-webkit-autofill:hover, 
input:-webkit-autofill:focus{
    -webkit-text-fill-color: #000;
    background-color:#FFF1D0;
    transition: background-color 5000s ease-in-out 0s;
}

#btnLogin{
    background:rgb(245,130,32);
}

    </style>

</head>
<body onload="LoadLogin();">
    <script type="text/javascript">
        function KeepAlive() {

        }

        function LoadLogin() {
            document.getElementById("hdfContentWidth").value = window.innerWidth;
            document.getElementById("hdfContentHeight").value = window.innerHeight;

            AjaxRequest("SetContentWidth", "Value=" + window.innerWidth, function (response) {
            });
            AjaxRequest("SetContentHeight", "Value=" + window.innerHeight, function (response) {
            });
        }
    </script>
    <asp:Panel ID="pnlMaintananceMessage" runat="server" Visible="false">
        <div style="position: absolute; left: 0px; top: 0px; width: 100%; height: 100%; background: #FF0000; opacity: 0.4;"></div>
        <div style="position: absolute; left: 50%; top: 50%; margin-left: -320px; margin-top: -140px; height: 300px; width: 650px; background: #FF0000; z-index: 1000;">
            <table style="width: 100%; height: 100%">
                <tr>
                    <td style="text-align: center;">
                        <h1 style="color: #FFFFFF;">
                            <wu:Label ID="lblMaintananceMessage" runat="server" Name="MaintananceMessage"></wu:Label>
                        </h1>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <form id="form1" runat="server">
        <input type="hidden" name="hdfContentWidth" id="hdfContentWidth" />
        <input type="hidden" name="hdfContentHeight" id="hdfContentHeight" />
        <table style="height: 100%; width: 100%;">
            <tr>
                <td>
                    <div class="Headline">
                       <a href="https://www.course5i.com/course5-link/" id="imgLogo" target="_blank" ></a>
                      <%--  <wu:Image ID="imgLogo" runat="server" ImageUrl="../Images/loginPage/link%20logo.png"></wu:Image>--%>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="height: 100%;" align="center">
                    <div class="LoginBox"> <%--BackgroundColor1--%>
                        <span class="LoginText">
                            <wu:Label ID="lblLoginText" runat="server" Name="LoginText"></wu:Label>
                        </span>
                        <br />
                        <span class="MsgLabel">
                            <wu:Label ID="lblMsg" runat="server" Name="" Visible="false"></wu:Label></span>
                        <table style="margin-left: 6em; margin-right: 6em; margin-top: 30px;">
                            <tr>
                                <td>
                                    <wu:TextBox ID="txtUsername" CssClass="input_Txt" runat="server" TabIndex="1" Button="btnLogin"></wu:TextBox>
                                </td>
                                <td rowspan="2">
                                    <wu:Button ID="btnLogin" runat="server" Name="Login" CssClass="LoginButton BackgroundColor2" OnClick="btnLogin_Click" TabIndex="3" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <wu:TextBox ID="txtPassword" CssClass="input_Txt" runat="server" TextMode="Password" TabIndex="2" Button="btnLogin"></wu:TextBox>
                                </td>
                            </tr>
                        </table>
                        <div class="ForgotPassword" style="margin-top: 15px; float: right; vertical-align: top; color: #FFFFFF;">
                            <wu:Button ID="btnForgot" runat="server" CssClass="ForgotButton" Name="forgot password" OnClick="btnForgot_Click" />
                        </div>
                    </div>
                </td>
            </tr>
            <% %>
            <tr>
                <td>
                    <div id="footer" class="Footer" style="display: block;">
                        <a href="https://www.course5i.com/" id="imgCopyright" target="_blank" ></a>
                        <a href="#" id="txt_content">&copy; Course5. All rights reserved.</a>

                      <%--  <wu:Image ID="imgCopyright" runat="server" ImageUrl="/Images/Logos/Blueocean.png"></wu:Image>--%>
                    </div>
                </td>
            </tr>
        </table>
        <script>           
            var row = document.getElementById("footer");
            if (window.location.href == "https://demo.linkmr.com/Pages/Login.aspx") {
                row.style.display = "none";
            }
            else if (window.location.href.split('?') == "https://demo.linkmr.com/Pages/Login.aspx") {
                row.style.display = "none";
            }
            else {
                row.style.display = "block";
            }
        </script>
    </form>
</body>
</html>
