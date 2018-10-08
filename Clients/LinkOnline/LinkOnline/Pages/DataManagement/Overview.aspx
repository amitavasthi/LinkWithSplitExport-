<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Overview.aspx.cs" Inherits="LinkOnline.Pages.DataManagement.Overview" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <style type="text/css">
        .StudyControl {
            word-break: break-word;
            margin: 5px;
            padding: 10px;
            box-shadow: 0px 0px 2px 0px #cccccc;
            border: 1px solid #cccccc;
            overflow: hidden;
        }


        .TreeViewNodeLabel {
            cursor: pointer;
        }

        .LabelHierarchyPath {
            font-size: 18pt;
        }

        .StudyControlColumn {
            float: left;
            padding: 2px;
        }


        .MoveStudiesDescription {
            background: #FFFFFF;
            margin-top: 20px;
            position: absolute;
            z-index: 10000;
            font-size: 18pt;
        }


        .TreeViewNodeLabel .ImageInsertHierarchy {
            cursor: pointer;
        }

        .TreeViewNodeLabel .ImageInsertHierarchy img {
            margin-right: 10px;
        }

        .TreeViewNodeLabel .ImageInsertHierarchy {
            visibility: hidden;
        }

        .TreeViewNodeLabel:hover .ImageInsertHierarchy {
            visibility: visible;
        }
    </style>
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="DataManagementTitle"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>

<asp:content id="Content2" contentplaceholderid="cphNonScroll" runat="server">
</asp:content>

<asp:content id="Content3" contentplaceholderid="cphContent" runat="server">

    <script type="text/javascript">

        var StudyDisplayType = {
            Grid: 0,
            Items: 1
        };

        var displayType = StudyDisplayType.Grid;

        loadFunctions.push(function () {
            document.getElementById("pnlStudiesContainer").style.height = (ContentHeight - 66) + "px";
            document.getElementById("cphContent_pnlHierarchies").style.height = (ContentHeight - 66) + "px";
        });

        var selectedHierarchy = undefined;
        function LoadStudies(sender, idHierarchy) {
            if (moveActionActive) {
                var idStudies = "";
                var selectedStudies = GetSelectedStudies();

                for (var i = 0; i < selectedStudies.length; i++) {
                    idStudies += selectedStudies[i].Id + ",";
                }

                if (selectedStudies.length > 0)
                    idStudies = idStudies.slice(0, idStudies.length - 1);

                _AjaxRequest("Overview.aspx", "MoveStudies", "IdHierarchy=" + idHierarchy + "&IdStudies=" + idStudies, function (response) {
                    LoadStudies(sender, idHierarchy);
                });

                moveActionActive = false;

                document.getElementById("Headline").style.opacity = "";
                document.getElementById("pnlStudiesContainer").style.opacity = "";
                document.getElementById("tdStudiesHeadline").style.opacity = "";
                document.getElementById("pnlMoveStudiesDescription").style.display = "none";

                return;
            }

            var nodes = GetChildsByAttribute(document.getElementById("cphContent_pnlHierarchies"), "class", "TreeViewNode BackgroundColor1", true);

            for (var i = 0; i < nodes.length; i++) {
                nodes[i].className = "TreeViewNode BackgroundColor5";
            }

            sender.parentNode.className = "TreeViewNode BackgroundColor1";
            document.getElementById("lblHierarchyPath").innerHTML = sender.parentNode.getAttribute("Path");

            selectedHierarchy = {
                Id: idHierarchy,
                Path: sender.parentNode.getAttribute("Path")
            };

            ShowLoading(document.getElementById("pnlTaxonomyVariablesContainer"));

            _AjaxRequest("Overview.aspx", "GetStudies", "IdHierarchy=" + idHierarchy, function (response) {
                var result = JSON.parse(response);

                var container = document.getElementById("pnlStudies");
                container.innerHTML = "";

                for (var i = 0; i < result.length; i++) {
                    var control = RenderStudyControl(result[i]);

                    if (displayType == StudyDisplayType.Items) {
                        control.style.width = "200px";
                        control.style.height = "100px";
                        control.style.float = "left";
                    }

                    container.appendChild(control);
                }

                InitBoxes();
                HideLoading();
                ToggleStudySelector();
            });
        }

        function RenderStudyControl(study) {
            var control = document.createElement("div");
            control.id = "pnlStudyControl" + study.Id;
            control.className = "StudyControl";
            control.setAttribute("onclick", "var image = document.getElementById(\"chkStudy" + study.Id + "\")[\"Image\"];if(event.target==image) return;image.onclick();");

            var status = "";

            if (study.Additional != "") {
                var split = study.Additional.split('|');

                var step = split[0];
                var progress = split[1];
                var eta = "";

                if (split.length > 2) {
                    eta = " " + LoadLanguageText("DataImportETA").replace("{0}", split[2]);
                }

                control.style.backgroundImage = "url('/Images/Icons/DataManagement/ProgressBackground.png')";
                control.style.backgroundSize = progress + "% 100%";
                control.style.backgroundRepeat = "no-repeat";

                status = "<div id=\"studyStatus" + study.Id + "\" class=\"StudyControlColumn\" style=\"float:right\"><table><tr>" +
                    "<td style=\"width:200px;text-align:right;\" class=\"Color1\">" + LoadLanguageText(step) + eta + "</td></tr></table></div>";

                window.setTimeout(function () { UpdateStudyUploadStatus(study) }, 1000);
            }
            else {
                status = "<div class=\"StudyControlColumn\" style=\"float:right\"><table><tr><td>" +
                    "<img height=\"20\" src=\"/Images/Icons/DataManagement/Status/" + study.Status + ".png\" /><td>" +
                    "<td style=\"width:50px;\">" + LoadLanguageText(study.Status) + "</td></tr></table></div>";
            }

            control.innerHTML += status;
            control.innerHTML += "<div class=\"StudyControlColumn\"><input onclick=\"ToggleStudySelector(this);\" " +
                "type=\"checkbox\" id=\"chkStudy" + study.Id + "\" name=\"chkStudy\" IdStudy=\"" + study.Id + "\" StudyName=\"" + study.Name + "\" /></div>";
            control.innerHTML += "<div class=\"StudyControlColumn\" style=\"padding: 5px;\">" + study.Name + "</div>";

            return control;
        }

        function UpdateStudyUploadStatus(study) {
            var pnlStatus = document.getElementById("studyStatus" + study.Id);

            if (pnlStatus == undefined)
                return;

            _AjaxRequest("Overview.aspx", "GetStudyUploadStatus", "IdStudy=" + study.Id, function (response) {

                var pnlStudy = document.getElementById("pnlStudyControl" + study.Id);

                if (response == "") {
                    pnlStatus.innerHTML = "<table><tr><td>" +
                    "<img height=\"20\" src=\"/Images/Icons/DataManagement/Status/" + status + ".png\" /><td>" +
                    "<td style=\"width:50px;\">" + LoadLanguageText("StudyStatusUnlinked") + "</td></tr></table>";

                    pnlStudy.style.backgroundImage = "";
                    pnlStudy.style.backgroundSize = "";
                    pnlStudy.style.backgroundRepeat = "";

                    return;
                }

                var split = response.split('|');

                var step = split[0];
                var progress = split[1];
                var eta = "";

                if (split.length > 2) {
                    eta = " " + LoadLanguageText("DataImportETA").replace("{0}", split[2]);
                }

                pnlStudy.style.backgroundSize = progress + "% 100%";

                pnlStatus.innerHTML = "<table><tr>" +
                    "<td style=\"width:200px;text-align:right;\" class=\"Color1\">" + LoadLanguageText(step) + eta + "</td></tr></table>";

                window.setTimeout(function () { UpdateStudyUploadStatus(study) }, 1000);
            });
        }

        function GetSelectedStudies() {
            var checkboxes = document.getElementsByName("chkStudy");

            var result = new Array();

            for (var i = 0; i < checkboxes.length; i++) {
                if (!checkboxes.item(i).checked)
                    continue;

                result.push({
                    Id: checkboxes.item(i).getAttribute("IdStudy"),
                    Name: checkboxes.item(i).getAttribute("StudyName")
                });
            }

            return result;
        }

        function ToggleStudySelector(sender) {
            var studies = GetSelectedStudies();

            if (studies.length == 0) {
                document.getElementById("cphContent_pnlRightPanelMove").style.opacity = "0.5";
                document.getElementById("cphContent_pnlRightPanelDataAnalyzer").style.opacity = "0.5";
                document.getElementById("cphContent_pnlRightPanelSetHierarchyFilter").style.opacity = "0.5";
                document.getElementById("cphContent_pnlRightPanelModifyVariables").style.opacity = "0.5";
                document.getElementById("cphContent_pnlRightPanelDownloadAssignment").style.opacity = "0.5";
                document.getElementById("cphContent_pnlRightPanelDelete").style.opacity = "0.5";
            }
            else if (studies.length > 1) {
                document.getElementById("cphContent_pnlRightPanelMove").style.opacity = "";
                document.getElementById("cphContent_pnlRightPanelDataAnalyzer").style.opacity = "0.5";
                document.getElementById("cphContent_pnlRightPanelSetHierarchyFilter").style.opacity = "0.5";
                document.getElementById("cphContent_pnlRightPanelModifyVariables").style.opacity = "0.5";
                document.getElementById("cphContent_pnlRightPanelDownloadAssignment").style.opacity = "";
                document.getElementById("cphContent_pnlRightPanelDelete").style.opacity = "";
            }
            else {
                document.getElementById("cphContent_pnlRightPanelMove").style.opacity = "";
                document.getElementById("cphContent_pnlRightPanelDataAnalyzer").style.opacity = "";
                document.getElementById("cphContent_pnlRightPanelSetHierarchyFilter").style.opacity = "";
                document.getElementById("cphContent_pnlRightPanelModifyVariables").style.opacity = "";
                document.getElementById("cphContent_pnlRightPanelDownloadAssignment").style.opacity = "";
                document.getElementById("cphContent_pnlRightPanelDelete").style.opacity = "";
            }
        }

        var changeDisplayTypeControls;
        function ChangeDisplayType(sender, newDisplayType) {

            if (newDisplayType == StudyDisplayType.Items) {
                sender.src = "/Images/Icons/DataManagement/ViewGrid.png";
                sender.setAttribute("onclick", "ChangeDisplayType(this, StudyDisplayType.Grid)");
            } else {
                sender.src = "/Images/Icons/DataManagement/ViewItems.png";
                sender.setAttribute("onclick", "ChangeDisplayType(this, StudyDisplayType.Items)");
            }

            displayType = newDisplayType;

            changeDisplayTypeControls = GetChildsByAttribute(document.getElementById("pnlStudies"), "class", "StudyControl");

            for (var i = 0; i < changeDisplayTypeControls.length; i++) {
                var control = changeDisplayTypeControls[i];

                if (displayType == StudyDisplayType.Items) {
                    control.style.width = (control.parentNode.offsetWidth - 30) + "px";
                    //control.style.height = "100px";
                    control.style.height = (control.offsetHeight - 10) + "px";
                    control.style.float = "left";

                    //DecreaseWidth(control, 200);
                    window.setTimeout("DecreaseWidth(changeDisplayTypeControls[" + i + "], 200);", i * 10);

                    IncreaseHeight(control, 100);
                }
                else {
                    /*control.style.width = "";
                    control.style.height = "";*/
                    //control.style.float = "";
                    control.style.height = "";

                    window.setTimeout("IncreaseWidth(changeDisplayTypeControls[" + i + "], " + (control.parentNode.offsetWidth - 30) + ", false, function(control) { control.style.float=''; control.style.width=''; });", i * 10);
                }
            }
        }

        var moveActionActive = false;
        function MoveSelectedStudies() {
            document.getElementById("Headline").style.opacity = "0.5";
            document.getElementById("pnlStudiesContainer").style.opacity = "0.5";
            document.getElementById("tdStudiesHeadline").style.opacity = "0.5";

            document.getElementById("pnlMoveStudiesDescription").style.display = "";

            moveActionActive = true;
        }


        function DownloadAssignment(idStudy) {
            window.location = "Overview.aspx?Action=DownloadAssignment&IdStudy=" + idStudy;
        }

        function RunDataAnalyser() {
            var selectedStudies = GetSelectedStudies();

            if (selectedStudies.length != 1)
                return;

            var idStudy = selectedStudies[0].Id;

            document.getElementById("imgDataAnalyserExport").onclick = function () {
                CloseBox('boxDataAnalyserControl', 'Bottom');
                window.location = "Overview.aspx?Method=DownloadDataAnalyser&DataAnalyserMethod=Export&IdStudy=" + idStudy;
            };
            document.getElementById("imgDataAnalyserWeights").onclick = function () {
                var btnDownload = document.getElementById("btnDataAnalyserWeightsDownload");

               

                _AjaxRequest("/Handlers/GlobalHandler.ashx", "GetNumericVariables", "IdStudy=" + idStudy, function (response) {
                    var variables = JSON.parse(response);

                    var ddlDataAnalyserWeightsVariable = document.getElementById("cphContent_ddlDataAnalyserWeightsVariable");
                    ddlDataAnalyserWeightsVariable.innerHTML = "";

                    for (var i = 0; i < variables.length; i++) {
                        ddlDataAnalyserWeightsVariable.innerHTML += "<option value=\"" + variables[i].Id + "\">" + variables[i].Name + "</option>";
                    }

                    btnDownload.value = LoadLanguageText("Download");
                    btnDownload.onclick = function () {
                        CloseBox('boxDataAnalyserWeightsControl', 'Bottom');

                        window.location = "Overview.aspx?Method=DownloadDataAnalyser&DataAnalyserMethod=Weights&IdStudy=" + idStudy + "&IdVariable=" +
                            document.getElementById("cphContent_ddlDataAnalyserWeightsVariable").value;
                    };

                    CloseBox('boxDataAnalyserControl', 'Bottom');
                    InitDragBox("boxDataAnalyserWeightsControl");
                });
            };
            document.getElementById("imgDataAnalyserCompare").onclick = function () {
                CloseBox('boxDataAnalyserControl', 'Bottom');
                window.location = "Overview.aspx?Method=DownloadDataAnalyser&DataAnalyserMethod=Compare&IdStudy=" + idStudy;
            };

            InitDragBox("boxDataAnalyserControl");

            var btnCancel = document.getElementById("btnDataAnalyserWeightsCancel");
            btnCancel.value = LoadLanguageText("Cancel");
            btnCancel.onclick = function () {
                CloseBox('boxDataAnalyserWeightsControl', 'Bottom');
            }
        }

        function DeleteStudies() {
            CreateConfirmBox(LoadLanguageText("DeleteStudiesConfirmMessage"), function () {
                var idStudies = "";
                var selectedStudies = GetSelectedStudies();

                for (var i = 0; i < selectedStudies.length; i++) {
                    idStudies += selectedStudies[i].Id + ",";
                }

                if (selectedStudies.length > 0)
                    idStudies = idStudies.slice(0, idStudies.length - 1);

                _AjaxRequest("Overview.aspx", "DeleteStudies", "IdStudies=" + idStudies, function (response) {
                    LoadStudies(document.getElementById("cphContent__tvnHierarchy" + selectedHierarchy.Id), selectedHierarchy.Id);
                });
            });
        }


        var insertHierarchyIdParent;
        function InsertHierarchy() {
            var parameters = "";
            parameters += "IdHierarchy=" + insertHierarchyIdParent + "&";
            parameters += "IdWorkgroup=" + document.getElementById("cphContent_ddlInsertHierarchyWorkgroup").value + "&";
            parameters += "Name=" + document.getElementById("txtInsertHierarchyName").value;

            _AjaxRequest("Overview.aspx", "InsertHierarchy", parameters, function (response) {
                window.location = window.location;
            });
        }


        function UploadFile() {
            var _file = document.getElementById('_file');

            var studyName = document.getElementById("txtUploadStudyName").value;
            var respondentVariable = document.getElementById("txtUploadRespondentVariable").value;
            var idLanguage = document.getElementById("cphContent_ddlUploadStudyLanguage").value;

            if (_file.files.length === 0) {
                return;
            }

            var data = new FormData();
            data.append('SelectedFile', _file.files[0]);

            var request = new XMLHttpRequest();
            request.onreadystatechange = function () {
                if (request.readyState == 4) {
                    try {
                        window.location = window.location;
                    } catch (e) {
                        var resp = {
                            status: 'error',
                            data: 'Unknown error occurred: [' + request.responseText + ']'
                        };
                    }
                    console.log(resp.status + ': ' + resp.data);
                }
            };

            request.upload.addEventListener('progress', function (e) {
                var progress = Math.ceil((e.loaded * 100) / e.total);
                document.getElementById("LoadingText").innerHTML = progress + '%';
            }, false);

            ShowLoading(document.getElementById("boxUploadControl"));

            request.open("POST", "Overview.aspx?Method=UploadFile&StudyName=" + studyName + "&IdLanguage=" + idLanguage + "&IdHierarchy=" + selectedHierarchy.Id + "&RespondentVariable=" + respondentVariable);
            request.send(data);
        }


        function EditHierarchy(idHierarchy) {
            var menu = InitMenu("menuHierarchy" + idHierarchy);


            var lnkRename = document.createElement("div");

            lnkRename.ImageUrl = "/Images/Icons/Menu/Rename.png";
            lnkRename.innerHTML = LoadLanguageText("Rename");
            lnkRename.MenuItemClick = "RenameHierarchy('" + idHierarchy + "')";

            menu.Items.push(lnkRename);

            var lnkDelete = document.createElement("div");

            lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
            lnkDelete.innerHTML = LoadLanguageText("Delete");
            lnkDelete.MenuItemClick = "DeleteHierarchy('" + idHierarchy + "')";

            menu.Items.push(lnkDelete);


            menu.Render();
        }


        function RenameHierarchy(idHierarchy) {
            var label = document.getElementById("lblHierarchyName" + idHierarchy);

            var txtLabel = document.createElement("input");
            txtLabel.type = "text";
            txtLabel.value = label.innerHTML;
            //txtLabel.style.width = (label.offsetWidth-20) + "px";

            txtLabel.onblur = function () {
                _AjaxRequest("Overview.aspx", "RenameHierarchy", "IdHierarchy=" + idHierarchy + "&Name=" + encodeURIComponent(this.value));

                this.parentNode.innerHTML = this.value;
            };
            txtLabel.onkeydown = function (event) {
                if (event.keyCode != 13) return;

                this.onblur(event);
            };

            label.innerHTML = "";
            label.appendChild(txtLabel);

            txtLabel.focus();
        }

        function DeleteHierarchy(idHierarchy) {
            CreateConfirmBox(LoadLanguageText("DeleteHierarchyMessage").replace("{0}", document.getElementById("lblHierarchyName" + idHierarchy).innerHTML), function () {
                _AjaxRequest("Overview.aspx", "DeleteHierarchy", "IdHierarchy=" + idHierarchy, function (response) {
                    if (response != "") {
                        ShowMessage(response, "Error");
                        return;
                    }
                    window.location = window.location;
                });
            });
        }


    </script>
    <div id="pnlRightPanelContainer" style="position:absolute;right:0px;z-index: 1010;">
        <table cellspacing="0" cellpadding="0" style="height:712px;">
            <tr>
                <td align="right">
                    <asp:Image ID="imgRightPanelTrigger" CssClass="BackgroundColor6" runat="server" ImageUrl="/Images/Icons/LinkReporterSettings/Expander.png" style="cursor:pointer;" onclick="ShowRightPanel(this);"></asp:Image>
                </td>
                <td>
                    <div id="pnlRightPanel" class="BorderColor1" style="display:none;overflow:hidden;width:0px;border-width:1px;border-style:solid;background: #FFFFFF;">
                        <asp:Panel ID="pnlRightPanelMove" runat="server" class="RightPanelItem" onclick="MoveSelectedStudies();" style="">
                            <img src="/Images/Icons/DataManagement/Move.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelMove" runat="server" Name="Move"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelDataAnalyzer" runat="server" class="RightPanelItem" onclick="RunDataAnalyser()" style="">
                            <img src="/Images/Icons/DataManagement/DataAnalyzer.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelDataAnalyzer" runat="server" Name="DataAnalyzer"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelSetHierarchyFilter" runat="server" class="RightPanelItem" onclick="window.location = 'HierarchyFilter.aspx?IdStudy=' + GetSelectedStudies()[0].Id;" style="">
                            <img src="/Images/Icons/DataManagement/SetHierarchyFilter.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelSetHierarchyFilter" runat="server" Name="SetHierarchyFilter"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelModifyVariables" runat="server" class="RightPanelItem" onclick="" style="">
                            <img src="/Images/Icons/DataManagement/ModifyVariables.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelModifyVariables" runat="server" Name="ModifyVariables"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelDownloadAssignment" runat="server" class="RightPanelItem" onclick="" style="">
                            <img src="/Images/Icons/DataManagement/DownloadAssignment.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelDownloadAssignment" runat="server" Name="DownloadAssignment"></wu:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlRightPanelDelete" runat="server" class="RightPanelItem RedBackground" onclick="DeleteStudies();" style="">
                            <img src="/Images/Icons/DataManagement/Delete.png" style="width:60px;" /><br />
                            <wu:Label ID="lblRightPanelDelete" runat="server" Name="Delete"></wu:Label>
                        </asp:Panel>
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <table style="width:100%" cellspacing="0" cellpadding="0">
        <tr valign="top">
            <td class="BackgroundColor7" rowspan="2" style="width:250px;min-width:250px;">
                <div style="margin:1em">
                    <asp:Panel id="pnlHierarchies" runat="server" style="overflow:auto;"></asp:Panel>
                </div>
            </td>
            <td class="BackgroundColor7" style="height: 50px;" id="tdStudiesHeadline">
                <div style="margin:10px;">
                    <div style="float:right">
                        <!--<input type="button" ID="btnUpload" runat="server" onclick="UploadStudy();" />-->
                        <table>
                            <tr>
                                <td>
                                    <img src="/Images/Icons/DataManagement/UploadStudy.png" style="cursor:pointer;" height="20" onclick="InitDragBox('boxUploadControl');" />
                                    &nbsp;
                                </td>
                                <td>
                                    <img src="/Images/Icons/DataManagement/ViewItems.png" style="cursor:pointer;" height="20" onclick="ChangeDisplayType(this, StudyDisplayType.Items);" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="lblHierarchyPath" class="LabelHierarchyPath Color1"></div>
                    <div style="clear:both"></div>
                </div>
            </td>
        </tr>
        <tr valign="top">
            <td style="width:100%;">
                    <div id="pnlMoveStudiesDescription" class="MoveStudiesDescription Color1" style="display:none;">
                        <table>
                            <tr>
                                <td>
                                    <img src="/Images/Icons/ArrowLeft.png" />
                                </td>
                                <td>
                                    <wu:Label ID="lblMoveStudiesDescription" runat="server" Name="MoveStudiesDescription"></wu:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                <div id="pnlStudiesContainer" style="overflow:auto;">
                    <div style="margin:1em">
                        <div id="pnlStudies"></div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    
    <wu:Box ID="boxInsertHierarchy" runat="server" Title="InsertHierarchy" Dragable="true" JavascriptTriggered="true">
        <table>
            <tr>
                <td>
                    <wu:Label ID="lblInsertHierarchyName" runat="server" Name="Name"></wu:Label>
                </td>
                <td>
                    <input type="text" id="txtInsertHierarchyName" />
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblInsertHierarchyWorkgroup" runat="server" Name="InsertHierarchyWorkgroup"></wu:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlInsertHierarchyWorkgroup" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <wu:Button ID="btnInsertHierarchyConfirm" runat="server" Name="Confirm" OnClientClick="InsertHierarchy();return false;"></wu:Button>
                    <wu:Button ID="btnInsertHierarchyCancel" runat="server" Name="Cancel"></wu:Button>
                </td>
            </tr>
        </table>
    </wu:Box>

    <wu:Box ID="boxDataAnalyser" runat="server" Title="DataAnalyser" Dragable="true" JavascriptTriggered="true">
        <table style="text-align:center;" cellspacing="20">
            <tr>
                <td>
                    <img id="imgDataAnalyserExport" src="/Images/Icons/DataManagement/DataAnalyser/Export.png" style="height:150px;cursor:pointer;" />
                </td>
                <td>
                    <img id="imgDataAnalyserWeights" src="/Images/Icons/DataManagement/DataAnalyser/Weights.png" style="height:150px;cursor:pointer;" />
                </td>
                <td>
                    <img id="imgDataAnalyserCompare" src="/Images/Icons/DataManagement/DataAnalyser/Compare.png" style="height:150px;cursor:pointer;" />
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblDataAnalyserExport" runat="server" Name="DataAnalyserExport"></wu:Label>
                </td>
                <td>
                    <wu:Label ID="lblDataAnalyserWeights" runat="server" Name="DataAnalyserWeights"></wu:Label>
                </td>
                <td>
                    <wu:Label ID="lblDataAnalyserCompare" runat="server" Name="DataAnalyserCompare"></wu:Label>
                </td>
            </tr>
        </table>
    </wu:Box>
    <wu:Box ID="boxDataAnalyserWeights" runat="server" Title="DataAnalyserWeights" Dragable="true" JavascriptTriggered="true">
        <table>
            <tr>
                <td>
                    <wu:Label ID="lblDataAnalyserWeightsVariable" runat="server" Name="WeightingVariable"></wu:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlDataAnalyserWeightsVariable" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <input type="button" id="btnDataAnalyserWeightsDownload" />
                    <input type="button" id="btnDataAnalyserWeightsCancel" />
                    <%--<wu:Button ID="btnDataAnalyserWeightsCancel" runat="server" Name="Cancel"></wu:Button>--%>
                </td>
            </tr>
        </table>
    </wu:Box>
    <wu:Box ID="boxUpload" runat="server" Title="Upload" TitleLanguageLabel="true" Dragable="true" JavascriptTriggered="true">
        <table>
            <tr>
                <td>
                    <wu:Label ID="lblUploadStudyName" runat="server" Name="StudyName"></wu:Label>
                </td>
                <td>
                    <input type="text" id="txtUploadStudyName" />
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblUploadRespondentVariable" runat="server" Name="RespondentVariable"></wu:Label>
                </td>
                <td>
                    <input type="text" id="txtUploadRespondentVariable" />
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblUploadStudyLanguage" runat="server" Name="StudyLanguage"></wu:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlUploadStudyLanguage" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblUploadFile" runat="server" Name="StudyFile"></wu:Label>
                </td>
                <td>
                    <input type='file' id='_file'>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <i><wu:Label ID="lblStep2FileSubTitle" runat="server" Name="DataUploadFileSubTitle" CssClass="SubTitle"></wu:Label></i>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <wu:Button ID="btnUploadConfirm" runat="server" Name="Upload" OnClientClick="UploadFile();return false;"></wu:Button>
                        
                    <wu:Button ID="btnUploadCancel" runat="server" Name="Cancel"></wu:Button>
                </td>
            </tr>
        </table>
        <wu:TipGallery ID="tgStudyUpload" runat="server" _TipItems="StudyUploadTip1, StudyUploadTip2" />
    </wu:Box>
</asp:content>

<asp:content id="Content4" contentplaceholderid="cphFooter" runat="server">
</asp:content>
