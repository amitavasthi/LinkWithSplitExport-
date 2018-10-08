<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TableauConnector.aspx.cs" Inherits="LinkOnline.Pages.Services.TableauConnector" %>

<%@ Register Src="~/Classes/Controls/HierarchySelector.ascx" TagPrefix="uc1" TagName="HierarchySelector" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Cache-Control" content="no-store" />
    <title>Stock Quote Connector - Advanced</title>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Main.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/Login.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/ColorSchemeTemp.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:ScriptReference runat="server" Source="/Scripts/Controls/Boxes.js" />
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/TableauConnector.js" />


    <style type="text/css">
        body {
        }

        .LoginForm {
            background: #355C80 !important;
            color: #FFFFFF;
        }

        .TableauConnectorVariable {
            cursor: pointer;
            padding: 5px;
            border-right: 1px solid #444444;
            border-top: 1px solid #444444;
        }

            .TableauConnectorVariable:hover {
                background: #355C80 !important;
                color: #FFFFFF;
            }

        .HeadlineCell {
            background: #355C80;
            color: #FFFFFF;
            font-size: 16pt;
            padding: 5px;
        }

        .spinner {
            width: 40px;
            height: 40px;
            position: relative;
            margin: 25% auto;
        }

        .double-bounce1, .double-bounce2 {
            width: 100%;
            height: 100%;
            border-radius: 50%;
            background-color: #FFF;
            opacity: 0.6;
            position: absolute;
            top: 0;
            left: 0;
            -webkit-animation: sk-bounce 2.0s infinite ease-in-out;
            animation: sk-bounce 2.0s infinite ease-in-out;
        }

        .double-bounce2 {
            -webkit-animation-delay: -1.0s;
            animation-delay: -1.0s;
        }

        @-webkit-keyframes sk-bounce {
            0%, 100% {
                -webkit-transform: scale(0.0);
            }

            50% {
                -webkit-transform: scale(1.0);
            }
        }

        @keyframes sk-bounce {
            0%, 100% {
                transform: scale(0.0);
                -webkit-transform: scale(0.0);
            }

            50% {
                transform: scale(1.0);
                -webkit-transform: scale(1.0);
            }
        }
    </style>
</head>
<body onresize="Resize();">
    <form id="form1" runat="server">
        <div id="LoginForm" class="LoginForm" style="display: none">
            <table style="margin: auto; height: 100%;">
                <tr>
                    <td>
                        <wu:Label ID="lblLoginError" runat="server" Name="LoginErrorMsg" style="display:none"></wu:Label>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <wu:Label ID="lblUsername" runat="server" Name="Username"></wu:Label>
                                </td>
                                <td style="padding:5px;">
                                    <input tabindex="1" type="text" id="txtUsername" />
                                </td>
                                <td rowspan="2">
                                    <input tabindex="3" class="BackgroundColor2" style="border:none;height:95px;width:95px;" type="button" id="btnLogin" onclick="tableauConnector.Authenticate(document.getElementById('txtUsername').value, document.getElementById('txtPassword').value);" value="login" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <wu:Label ID="lblPassword" runat="server" Name="Password"></wu:Label>
                                </td>
                                <td style="padding:5px;">
                                    <input tabindex="2" type="password" id="txtPassword" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="content">
            <table id="tableVariableSelection" cellspacing="0" cellpadding="0" style="border-top: 1px solid #FFFFFF; width: 100%;">
                <tr valign="top">
                    <td class="HeadlineCell" style="border-right: 1px solid #355C80;">
                        <!--<wu:Label ID="lblAvailableVariables" runat="server" Name="AvailableVariables"></wu:Label>-->
                        <input type="text" id="txtSearchVariables" style="font-size: 10pt; padding: 2px;" onkeyup="tableauConnector.Search(this.value);" />
                    </td>
                    <td class="HeadlineCell">
                        <wu:Label ID="lblSelectedVariables" runat="server" Name="SelectedVariables"></wu:Label>
                    </td>
                </tr>
                <tr valign="top">
                    <td style="width: 50%; border-right: 1px solid #355C80;">
                        <div id="availableVariables" style="overflow: auto;"></div>
                    </td>
                    <td style="width: 50%;">
                        <div id="selectedVariables" style="overflow: auto;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="HeadlineCell" colspan="2" style="text-align: center; cursor: pointer;" onclick="tableauConnector.Load();">load
                    </td>
                </tr>
            </table>
        </div>
        <script type="text/javascript">


            function Resize() {
                document.getElementById("availableVariables").style.height = (window.innerHeight - 76) + "px";
                document.getElementById("selectedVariables").style.height = (window.innerHeight - 76) + "px";
                document.getElementById("LoginForm").style.height = (window.innerHeight) + "px";
            }

            //tableauConnector.InitConnector();
            tableauConnector.CheckAuthentication(function () {
                tableauConnector.LoadVariables();
            });

            Resize();


            function _AjaxRequest(url, method, parameters, onFinish, onError) {
                var http = getHTTPObject();

                if (method != "") {
                    if (parameters != "")
                        parameters = "Method=" + method + "&" + parameters;
                    else
                        parameters = "Method=" + method;
                }

                http.open("POST", url, true);

                http.onreadystatechange = function () {
                    if (http.readyState == 4) {
                        if (http.status == "200") {
                            if (onFinish != undefined) {
                                onFinish(http.responseText);
                            }
                        }
                        else {
                            if (onError != undefined)
                                onError();
                        }
                    }
                }

                http.setRequestHeader("Content-Type",
                    "application/x-www-form-urlencoded");

                http.send(parameters);

                return http;
            }


            function getHTTPObject() {
                var httpObject = false;
                if (window.XMLHttpRequest) {
                    httpObject = new XMLHttpRequest();
                } else if (window.ActiveXObject) {
                    httpObject = new ActiveXObject("Microsoft.XMLHTTP");
                } else {
                    // If not supported
                    httpObject = false;
                }

                return httpObject;
            }

            //});
        </script>
    </form>
</body>
</html>
