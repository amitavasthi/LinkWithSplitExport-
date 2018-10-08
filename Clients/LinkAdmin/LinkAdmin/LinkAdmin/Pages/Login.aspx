<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LinkAdmin.Pages.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>LiNK admin</title>

    <wu:ScriptReference runat="server" Source="/Scripts/Controls/Button2.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Components/Loading.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Components/Ajax.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/JQuery/jquery-1.8.1.min.js"></wu:ScriptReference>

    <style type="text/css">
        body {
            font-family: Calibri;
            background-size: 100%;
            background-repeat: no-repeat;
        }

        .LoginBox {
            background: #444444;
            position: absolute;
            left: 10%;
            top: 0px;
            height: 100%;
            width: 300px;
            opacity: 0.7;
            box-shadow: 0px 0px 5px 0px #444444;
        }

        .LoginForm {
            font-size: 16pt;
            color: #FFFFFF;
            position: absolute;
            left: 10%;
            top: 50%;
            margin-left: 20px;
            margin-top: -50px;
        }

        input[type=text],
        input[type=password] {
            background: #FAFAFA;
            padding: 5px;
            border-radius: 5px;
            border: none;
        }

        input[type=button],
        .Button {
            font-size: 14pt;
            display: inline-block;
            border: none;
            padding: 5px 10px 5px 10px;
            color: #FFFFFF;
            background: #2189d7;
            cursor: pointer;
            text-align: center;
        }

        .Button:hover {
            background: #2495e9;
            /*box-shadow:0px 0px 2px 0px #FFFFFF;*/
        }
    </style>
</head>
<body>
    <script type="text/javascript">
        var max = 6;

        var imgId = 0;

        while (imgId < 1 || imgId > max) {
            imgId = parseInt(Math.random() * 10);
        }

        document.body.style.backgroundImage = "url('/Images/Login/Backgrounds/" + imgId + ".png')";
    </script>
    <form id="form1" runat="server">
        <div>
            <div class="LoginBox">
            </div>
            <div class="LoginForm">
                <table>
                    <tr>
                        <td>
                            <wu:Label ID="lblUsername" runat="server" Name="Username"></wu:Label>
                        </td>
                        <td>
                            <wu:TextBox ID="txtUsername" runat="server" Button="_btnLogin"></wu:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <wu:Label ID="lblPassword" runat="server" Name="Password"></wu:Label>
                        </td>
                        <td>
                            <wu:TextBox ID="txtPassword" runat="server" TextMode="Password" Button="_btnLogin"></wu:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align: right; padding-top: 5px">
                            <wu:Button2 ID="btnLogin" runat="server" Name="Login" Method="btnLogin_Click" PostFields="txtUsername, txtPassword"></wu:Button2>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
