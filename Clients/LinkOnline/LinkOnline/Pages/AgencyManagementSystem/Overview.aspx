<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Overview.aspx.cs" Inherits="LinkOnline.Pages.AgencyManagementSystem.Overview" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .TableStudies {
            width:100%;
        }
        .TableStudies td {
            padding:5px;
        }

        .TableCellOptions {
            /*padding:5px !important;*/
        }

        .SetProjectHierachyButton {
            display:inline-block;
            color:#FFFFFF;
            cursor:pointer;
            padding:5px;
            margin-left:5px;
        }


        .TableStudies .TableRowHeadline {
            font-size:16pt;
            font-style:italic;
        }

        .StudyImportErrorDetail {
            background:#FF0000 !important;
        }
    </style>
    <script type="text/javascript">
        function SetProjectHierachy(idStudy) {
            _AjaxRequest("/Handlers/StudyTaxonomyStructure.ashx", "SetProjectHierachy", "IdStudy=" + idStudy, function (response) {
                window.location = "Hierarchy.aspx";
            });
        }

        function DownloadAssignment(idStudy) {
            window.location = "Overview.aspx?Action=DownloadAssignment&IdStudy=" + idStudy;
        }

        function ShowImportErrorDetail(errorMessage) {
            ShowJavascriptBox("boxStudyImportError", errorMessage, undefined, false, "StudyImportErrorDetail");
        }

        function RunDataAnalyser(idStudy) {
            document.getElementById("imgDataAnalyserExport").onclick = function () {
                CloseBox('boxDataAnalyserControl', 'Bottom');
                window.location = "Overview.aspx?Action=DownloadDataAnalyser&DataAnalyserMethod=Export&IdStudy=" + idStudy;
            };
            document.getElementById("imgDataAnalyserWeights").onclick = function () {
                var btnDownload = document.getElementById("btnDataAnalyserWeightsDownload");

                _AjaxRequest("/Handlers/GlobalHandler.ashx", "GetNumericVariables", "IdStudy=" + idStudy, function (response) {
                    var variables = JSON.parse(response);

                    var ddlDataAnalyserWeightsVariable = document.getElementById("cphContent_ddlDataAnalyserWeightsVariable");

                    for (var i = 0; i < variables.length; i++) {
                        ddlDataAnalyserWeightsVariable.innerHTML += "<option value=\""+ variables[i].Id +"\">"+ variables[i].Name +"</option>";
                    }

                    btnDownload.value = LoadLanguageText("Download");
                    btnDownload.onclick = function () {
                        CloseBox('boxDataAnalyserWeightsControl', 'Bottom');

                        window.location = "Overview.aspx?Action=DownloadDataAnalyser&DataAnalyserMethod=Weights&IdStudy=" + idStudy + "&IdVariable=" +
                            document.getElementById("cphContent_ddlDataAnalyserWeightsVariable").value;
                    };

                    CloseBox('boxDataAnalyserControl', 'Bottom');
                    InitDragBox("boxDataAnalyserWeightsControl");
                });
            };
            document.getElementById("imgDataAnalyserCompare").onclick = function () {
                CloseBox('boxDataAnalyserControl', 'Bottom');
                window.location = "Overview.aspx?Action=DownloadDataAnalyser&DataAnalyserMethod=Compare&IdStudy=" + idStudy;
            };

            InitDragBox("boxDataAnalyserControl");
            var btnCancel = document.getElementById("btnDataAnalyserWeightsCancel");
            btnCancel.value = LoadLanguageText("Cancel");
            btnCancel.onclick = function () {
                CloseBox('boxDataAnalyserWeightsControl', 'Bottom');
            }
        }

        function DownloadDataAnalyserWeights(idStudy) {
            window.location = "Overview.aspx?Action=DownloadDataAnalyserWeights&IdStudy=" + idStudy;
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphTitle" runat="server">
    <h1 class="Color1">
        <wu:Label ID="lblPageTitle" runat="server" Name="AgencyManagementSystemTitle"></wu:Label>
        <span class="Beta">
            <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
        </span>
    </h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script type="text/javascript">
        function DeleteStudy(idStudy, studyName) {
            CreateConfirmBox(LoadLanguageText("DeleteStudyConfirmMessage").replace("{0}", studyName), function () {
                _AjaxRequest("Overview.aspx", "", "DeleteStudy=" + idStudy, function (response) {
                    window.location = window.location;
                });
            })
        }

        function validateStudyName() {

            if (document.getElementById("txtUploadStudyName").value.trim() != "") {

                if (document.getElementById("validationUploadStudyName").innerHTML == "Enter Study name") {
                    document.getElementById("validationUploadStudyName").style.display = "none";
                }
            }
        }

        function UploadFile() {
            var fileExist = true;
            document.getElementById("validationUploadStudyName").style.display = "none";
            var _file = document.getElementById('_file');

            var studyName = document.getElementById("txtUploadStudyName").value;
            if (studyName != "") {

                var studyPnl = document.getElementById("cphContent_pnlStudies");
                var stydyTableTr = studyPnl.getElementsByTagName("table")[0].getElementsByTagName("tr");
                var studyNames = [];

                for (var i = 2; i < stydyTableTr.length; i++)
                {                   
                    studyNames.push(stydyTableTr[i].getElementsByTagName("td")[0].innerHTML)
                }
               
                for (var i = 0; i < studyNames.length; i++) {
                    if (studyName == studyNames[i]) {
                        fileExist = false;
                    }
                }
            }else{
                document.getElementById("validationUploadStudyName").innerHTML = "Enter Study name";
                document.getElementById("validationUploadStudyName").style.display = "block";
            }
            if (fileExist) {
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

                request.open("POST", "Overview.aspx?Action=UploadFile&StudyName=" + studyName + "&IdLanguage=" + idLanguage + "&RespondentVariable=" + respondentVariable);
                request.send(data);
            } else {
                document.getElementById("validationUploadStudyName").innerHTML = "Study name already exist.";
                document.getElementById("validationUploadStudyName").style.display = "block";
            }
           
        }
    </script>
    <div style="margin:1em">
        <wu:Box ID="boxDataAnalyser" runat="server" Title="DataAnalyser" Dragable="true" JavascriptTriggered="true">
            <table style="text-align:center;">
                <tr>
                    <td>
                        <img id="imgDataAnalyserExport" src="/Images/Icons/DataAnalyser/Export.png" style="height:200px;cursor:pointer;" />
                    </td>
                    <td>
                        <img id="imgDataAnalyserWeights" src="/Images/Icons/DataAnalyser/Weights.png" style="height:200px;cursor:pointer;" />
                    </td>
                    <td>
                        <img id="imgDataAnalyserCompare" src="/Images/Icons/DataAnalyser/Compare.png" style="height:200px;cursor:pointer;" />
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

        <wu:Box ID="boxUpload" runat="server" Title="Upload" Dragable="true">
            <table>
                <tr>
                    <td>
                        <wu:Label ID="lblUploadStudyName" runat="server" Name="StudyName"></wu:Label>
                    </td>
                    <td>
                        <input type="text" id="txtUploadStudyName" onmouseout="validateStudyName();" />
                        <label id="validationUploadStudyName" style="color:red;display:none;">Study name already exist.</label>
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
        <asp:Panel ID="pnlStudies" runat="server"></asp:Panel>
    </div>
</asp:Content>
