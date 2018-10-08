<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConnectPowerBI.ascx.cs" Inherits="LinkOnline.Classes.Controls.ConnectPowerBI" %>
<script type="text/javascript">
    var redirectUrl = undefined;
    function ConnectPowerBI(idSavedReport, type) {
        CloseConnectPowerBI();

        var box = document.getElementById("cpbiTarget");

        box.style.display = "";

        box.style.left = ((window.innerWidth / 2) - (box.offsetWidth / 2)) + "px";
        box.style.top = ((window.innerHeight / 2) - (box.offsetHeight / 2)) + "px";

        document.getElementById("tdConnectPowerBITargetPowerBI").onclick = function () {
            ConnectPowerBITargetConfirm(idSavedReport, "PowerBI");
        };
      
        if (window.location.href.indexOf("LinkBi") == -1) {
            document.getElementById("tdConnectPowerBITargetTableau").style.display = '';
            document.getElementById("tdConnectPowerBITargetTableau").onclick = function () {
                ConnectPowerBITargetConfirm(idSavedReport, "Tableau");
            };
        } else {
            document.getElementById("tdConnectPowerBITargetTableau").style.display = 'none';
        }
           
    if (window.location.href.indexOf("LinkBi") != -1) {
        document.getElementById("tdConnectPowerBITargetTable").style.display = '';
        document.getElementById("tdConnectPowerBITargetTable").onclick = function () {
            ConnectPowerBITargetConfirm(idSavedReport, "TABLE");
        };
    } else {
        document.getElementById("tdConnectPowerBITargetTable").style.display = 'none';
    }

        document.getElementById("cpbiBackground").style.display = "";
        box.style.display = "";

    }
    function ConnectPowerBITargetConfirm(idSavedReport, type) {
        document.getElementById("cpbiTarget").style.display = "none";

        if (idSavedReport == undefined) {
            var saveBox = document.getElementById("cpbiSaveReport");
            saveBox.style.display = "";

            saveBox.style.left = ((window.innerWidth / 2) - (saveBox.offsetWidth / 2)) + "px";
            saveBox.style.top = ((window.innerHeight / 2) - (saveBox.offsetHeight / 2)) + "px";

            document.getElementById("cphContent_ConnectPowerBI_lnkConnectPowerBISaveConfirm").onclick = function () {
                AjaxRequest("SaveAllTabs", "Name=" + document.getElementById("cphContent_ConnectPowerBI_txtConnectPowerBISaveName").value, function (response) {
                    redirectUrl = "Crosstabs.aspx?SavedReport=" + response;
                    ConnectPowerBITargetConfirm(response, type);
                });
            };

            return;
        }

        var box = document.getElementById("cpbiEnterCredentials");
        var box2 = document.getElementById("cpbiResult");

        box.style.display = "";

        box.style.left = ((window.innerWidth / 2) - (box.offsetWidth / 2)) + "px";
        box.style.top = ((window.innerHeight / 2) - (box.offsetHeight / 2)) + "px";

        document.getElementById("cphContent_ConnectPowerBI_lnkConnectPowerBIConfirm").onclick = function () {

            AjaxRequest("MD5Encrypt", "Value=" + encodeURIComponent(document.getElementById("cphContent_ConnectPowerBI_txtConnectPowerBIPassword").value), function (encryptedPassword) {
                var result = "";

                //if (type == "Reporter") {
                //    result += "ProcessSavedReport&ResponseType=PowerBI";
                //}
                //else if (type == "LinkBi") {
                //    result += "ProcessDefinedReport&ResponseType=TABLE";
                //}
                var image = document.getElementById("img" + type + "Sample");               
                var tabs = GetChildsByAttribute(document.getElementById("cphContent_pnlReportTabs"), "class", "ReportTabLabel", true);

                if (type == "Tableau" && tabs.length != 0) {
                    result += "<table>";
                    for (var i = 0; i < tabs.length; i++) {
                        result += "<tr><td style=\"font-size:10pt;font-weight:bold;\">" + tabs[i].textContent + "</td><td><span contenteditable=\"true\" onclick=\"SetCursorToEnd(this);\">" +
                            window.location.origin + "/Handlers/LinkBIExternal.ashx?Method=" +
                            "ProcessSavedReport&ResponseType=" + type +
                            "&IdReport=" + idSavedReport + "&Username=" + currentUserName + "&Password=" + encryptedPassword +
                            "&Tab=" + tabs[i].id.replace("cphContent_lblReportTabName", "").replace(".xml", "")
                            + "</span></td></tr>";
                    }
                    result += "</table>";
                }
                else {

                    if (((type == "TABLE") || (type == "PowerBI")) && (window.location.href.indexOf("LinkBi") != -1)) {

                        type = "TABLE";
                        result = window.location.origin + "/Handlers/LinkBIExternal.ashx?Method=";
                        result += "ProcessDefinedReport&ResponseType=" + type;

                        result += "&IdReport=" + idSavedReport + "&Username=" + currentUserName + "&Password=" + encryptedPassword;
                    } else {
                        {
                            result = window.location.origin + "/Handlers/LinkBIExternal.ashx?Method=";

                            result += "ProcessSavedReport&ResponseType=" + type;

                            result += "&IdReport=" + idSavedReport + "&Username=" + currentUserName + "&Password=" + encryptedPassword;
                        }
                    }

                }
                 

                var resultContainer = document.getElementById("cpbiResultLink");
                resultContainer.innerHTML = result;

                document.body.removeAttribute("onselectstart");
                document.body.removeAttribute("unselectable");

                if (type != "Tableau" || tabs.length == 0) {
                    resultContainer.setAttribute("contenteditable", "true");
                    resultContainer.focus();
                    SetCursorToEnd(resultContainer);
                }

                image.style.display = "";
                image.style.height = (window.innerHeight - 200) + "px";

                box.style.display = "none";
                box2.style.display = "";
                box2.style.left = ((window.innerWidth / 2) - (box2.offsetWidth / 2)) + "px";
                box2.style.top = ((window.innerHeight / 2) - (box2.offsetHeight / 2)) + "px";
            });
        };
    }

    function CloseConnectPowerBI() {
        document.getElementById("cpbiBackground").style.display = "none";
        document.getElementById("cpbiTarget").style.display = "none";
        document.getElementById("cpbiSaveReport").style.display = "none";
        document.getElementById("cpbiEnterCredentials").style.display = "none";
        document.getElementById("cpbiResult").style.display = "none";
        document.getElementById("imgPowerBISample").style.display = "none";
        document.getElementById("imgTableauSample").style.display = "none";
        document.getElementById("imgTABLESample").style.display = "none";

        if (redirectUrl != undefined)
            window.location = redirectUrl;
    }
</script>
<style type="text/css">
    .TableCellConnectPowerBITarget:hover {
        font-weight:bold;
    }
</style>
<div id="cpbiBackground" class="BoxBackground" style="display: none;"></div>
<div id="cpbiTarget" class="Color1" style="position: absolute; z-index: 9999; background: #FFFFFF; font-size: 12pt; box-shadow: 0px 0px 2px 0px #000000; padding: 2em; display: none;">
    <wu:Label ID="lblConnectPowerBIDescriptionTarget" runat="server" Name="ConnectPowerBIDescriptionTarget" Style="font-style: italic;"></wu:Label>
    <br />
    <br />
    <table style="width: 100%;text-align:center;" cellspacing="0" cellpadding="0">
        <tr>
            <td id="tdConnectPowerBITargetPowerBI" class="Color1 TableCellConnectPowerBITarget" style="cursor: pointer; width: 150px; height: 100px;">
                <wu:Label ID="lblConnectPowerBITargetPowerBI" runat="server" Name="ConnectPowerBITargetPowerBI"></wu:Label>
            </td>
            <td id="tdConnectPowerBITargetTableau" class="Color1 TableCellConnectPowerBITarget" style="cursor: pointer; width: 150px; height: 100px;">
                <wu:Label ID="lblConnectPowerBITargetTableau" runat="server" Name="ConnectPowerBITargetTableau"></wu:Label>
            </td>
            <td id="tdConnectPowerBITargetTable" class="Color1 TableCellConnectPowerBITarget" style="cursor: pointer; width: 150px; height: 100px;">
                <wu:Label ID="lblConnectPowerBITargetTable" runat="server" Name="ConnectPowerBITargetTable"></wu:Label>
            </td>
        </tr>
    </table>
</div>
<div id="cpbiSaveReport" class="Color1" style="position: absolute; z-index: 9999; background: #FFFFFF; font-size: 12pt; box-shadow: 0px 0px 2px 0px #000000; padding: 2em; display: none;">
    <wu:Label ID="lblConnectPowerBIDescriptionSave" runat="server" Name="ConnectPowerBIDescriptionSave" Style="font-style: italic;"></wu:Label>
    <br />
    <br />
    <table style="width: 100%;">
        <tr>
            <td>
                <wu:Label ID="lblConnectPowerBISaveName" runat="server" Name="Name"></wu:Label>
            </td>
            <td>
                <wu:TextBox ID="txtConnectPowerBISaveName" runat="server"></wu:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="right">
                <wu:LinkButton ID="lnkConnectPowerBISaveConfirm" runat="server" AutoPostBack="false" Name="Save"></wu:LinkButton>
                <wu:LinkButton ID="lnkConnectPowerBISaveCancel" runat="server" AutoPostBack="false" Name="Cancel" OnClientClick="CloseConnectPowerBI();"></wu:LinkButton>
            </td>
        </tr>
    </table>
</div>
<div id="cpbiEnterCredentials" class="Color1" style="position: absolute; z-index: 9999; background: #FFFFFF; font-size: 12pt; box-shadow: 0px 0px 2px 0px #000000; padding: 2em; display: none;">
    <wu:Label ID="lblConnectPowerBIDescription1" runat="server" Name="ConnectPowerBIDescription1" Style="font-style: italic;"></wu:Label>
    <br />
    <br />
    <table style="width: 100%;">
        <tr>
            <td>
                <wu:Label ID="lblConnectPowerBIPassword" runat="server" Name="Password"></wu:Label>
            </td>
            <td>
                <wu:TextBox ID="txtConnectPowerBIPassword" runat="server" TextMode="Password"></wu:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="right">
                <wu:LinkButton ID="lnkConnectPowerBIConfirm" runat="server" AutoPostBack="false" Name="CreateLink"></wu:LinkButton>
                <wu:LinkButton ID="lnkConnectPowerBICancel" runat="server" AutoPostBack="false" Name="Cancel" OnClientClick="CloseConnectPowerBI();"></wu:LinkButton>
            </td>
        </tr>
    </table>
</div>
<div id="cpbiResult" class="Color1" style="width: 1060px; position: absolute; z-index: 9999; background: #FFFFFF; font-size: 12pt; box-shadow: 0px 0px 2px 0px #000000; padding: 2em; display: none;">
    <img src="/Images/Icons/BoxClose.png" class="BtnBoxClose" style="cursor: pointer; position: absolute; margin-left: 1066px; margin-top: -50px;" onmouseover="this.src='/Images/Icons/BoxClose_Hover.png';" onmouseout="this.src = '/Images/Icons/BoxClose.png'" onclick="CloseConnectPowerBI();">
    <wu:Label ID="lblConnectPowerBIDescription2" runat="server" Name="ConnectPowerBIDescription2" Style="font-style: italic;"></wu:Label>
    <br />
    <span id="cpbiResultLink" style="font-size: 7pt;"></span>
    <br />
    <br />
    <wu:Label ID="lblConnectPowerBIDescription3" runat="server" Name="ConnectPowerBIDescription3" Style="font-style: italic;"></wu:Label>
    <br />
    <div style="text-align: center;">
        <img id="imgPowerBISample" style="max-height: 700px;display:none;" src="/Images/Samples/ConnectPowerBI.png" />
        <img id="imgTableauSample" style="max-height: 700px;display:none;" src="/Images/Samples/ConnectTableau.png" />
        <img id="imgTABLESample" style="max-height: 700px;display:none;" src="/Images/Samples/ConnectTable.png" />
    </div>
</div>
