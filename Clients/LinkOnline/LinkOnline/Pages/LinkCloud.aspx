<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="LinkCloud.aspx.cs" Inherits="LinkOnline.Pages.LinkCloud" EnableEventValidation="false" %>

<%@ Register Src="~/Classes/Controls/ConnectPowerBI.ascx" TagPrefix="uc1" TagName="ConnectPowerBI" %>


<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/LinkCloud.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/SavedReports.js"></wu:ScriptReference>
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="LibraryTitle"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <table cellspacing="0" cellpadding="0" class="LinkCloud">
        <tr valign="top">
            <td rowspan="2">
                <asp:Panel ID="pnlExplorer" runat="server" CssClass="FileExplorer BorderColor1"></asp:Panel>
            </td>
            <td style="height:100%;width:100%;">
                <asp:Panel ID="pnlExplorerNavigation" runat="server" CssClass="FileExplorerNavigation BackgroundColor5">
                    <div style="float:right">
                        <wu:TextBox ID="txtSearch" runat="server"  onkeyup="Search(this.value);" AutoComplete="off" Text=""></wu:TextBox>
                        <img src="/Images/Icons/Search.png" alt="search" onclick="document.getElementById('cphContent_txtSearch').focus();" 
                            onmouseover="this.src = '/Images/Icons/Search_Hover.png';" onmouseout="this.src = '/Images/Icons/Search.png';" />
                    <img id="imgSwitchView" src="/Images/Icons/Menu/SwitchViewItems.png" style="float:right;margin:4px;cursor:pointer" height="20" onclick="SwitchView(this);" />
                    <div style="clear:both"></div>
                        </div>
                </asp:Panel>
                <div id="pnlGridViewHeadline" class="LibraryGridHeadline CloudItemGrid" style="display:none;">
                    <div id="pnlGridViewHeadlineItemCreationDate" runat="server" class="LibraryGridHeadlineItem GridViewDetail" style="width:130px;" onclick="SetLibraryOrderField('CreationDate');">
                        <wu:Label ID="lblGridViewHeadlineCreationDate" runat="server" Name="LibraryHeadlineCreationDate"></wu:Label>
                    </div>
                    <div id="pnlGridViewHeadlineItemModificationDate" runat="server" class="LibraryGridHeadlineItem GridViewDetail" style="width:130px;" onclick="SetLibraryOrderField('ModificationDate');">
                        <wu:Label ID="lblGridViewHeadlineModificationDate" runat="server" Name="LibraryHeadlineModifiedDate"></wu:Label>
                    </div>
                    <div id="pnlGridViewHeadlineItemSize" runat="server" class="LibraryGridHeadlineItem GridViewDetail" style="width:60px;" onclick="SetLibraryOrderField('Size');">
                        <wu:Label ID="lblGridViewHeadlineSize" runat="server" Name="LibraryHeadlineSize"></wu:Label>
                    </div>
                    <div id="pnlGridViewHeadlineItemType" runat="server" class="LibraryGridHeadlineItem GridViewDetail" style="width:150px" onclick="SetLibraryOrderField('Type');">
                        <wu:Label ID="lblGridViewHeadlineType" runat="server" Name="LibraryHeadlineType"></wu:Label>
                    </div>
                    <div id="pnlGridViewHeadlineItemAuthor" runat="server" class="LibraryGridHeadlineItem GridViewDetail" style="width:150px" onclick="SetLibraryOrderField('Author');">
                        <wu:Label ID="lblGridViewHeadlineAuthor" runat="server" Name="LibraryHeadlineAuthor"></wu:Label>
                    </div>
                    <div style="float:left;width:32px;height:32px;background:#FFFFFF;">

                    </div>
                    <div class="LibraryGridHeadlineItem" style="display:block;padding-left:60px;" onclick="SetLibraryOrderField('Name');">
                        <wu:Label ID="lblGridViewHeadlineName" runat="server" Name="LibraryHeadlineName"></wu:Label>
                    </div>
                </div>
                <asp:Panel ID="pnlFiles" runat="server" CssClass="Files" ondrop="UploadFile(e);" style="padding-bottom: 110px;" ScrollBars="Auto" ondragover="this.className='Files BackgroundColor9';" ondragend="this.className='Files';"></asp:Panel>
            </td>
        </tr>
    </table>
    <wu:Box ID="boxChooseNewAction" runat="server" Title="ChooseNewAction" Dragable="true">
        <table class="TableChooseNewAction">
            <tr>
                <td>
                    <asp:ImageButton ID="btnUpload" runat="server" ImageUrl="/Images/Icons/Cloud/Upload.png" OnClick="btnUpload_Click"></asp:ImageButton>
                </td>
                <td>
                    <asp:ImageButton ID="btnCreateDirectory" runat="server" CssClass="BackgroundColor2" ImageUrl="/Images/Icons/Cloud/Directory.png" OnClick="btnCreateDirectory_Click"></asp:ImageButton>
                </td>
            </tr>
            <tr>
                <td>
                    <wu:Label ID="lblUpload" runat="server" Name="UploadFile"></wu:Label>
                </td>
                <td>
                    <wu:Label ID="lblCreateDirectory" runat="server" Name="CreateDirectory"></wu:Label>
                </td>
            </tr>
        </table>
    </wu:Box>
    <wu:Box ID="boxUploadFile" runat="server" Dragable="true" Title="UploadFile">
        <table>
            <tr>
                <td>
                    <asp:FileUpload ID="fuUploadFile" runat="server"></asp:FileUpload>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <wu:Button ID="btnUploadFileConfirm" runat="server" Name="UploadFile" OnClick="btnUploadFileConfirm_Click"></wu:Button>
                    <wu:Button ID="btnUploadFileCancel" runat="server" Name="Cancel"></wu:Button>
                </td>
            </tr>
        </table>
    </wu:Box>
    <wu:Box ID="boxCreateDirectory" runat="server" Dragable="true" Title="CreateDirectory">
        <table>
            <tr>
                <td>
                    <wu:Label ID="lblCreateDirectoryName" runat="server" Name="Name"></wu:Label>
                </td>
                <td>
                    <wu:TextBox ID="txtCreateDirectoryName" runat="server" Button="btnCreateDirectoryConfirm"></wu:TextBox>
                </td>
            </tr>
            <tr>
                <td  align="centre">
                    <wu:Button ID="btnCreateDirectoryConfirm" runat="server" Name="CreateDirectory" OnClick="btnCreateDirectoryConfirm_Click"></wu:Button>
                    <wu:Button ID="btnCreateDirectoryCancel" runat="server" Name="Cancel"></wu:Button>
                </td>
            </tr>
        </table>
    </wu:Box>    
    <wu:Box ID="boxCreateFolder" runat="server" Dragable="true" Title="CreateDirectory">
        <table>
            <tr>
                <td>
                    <wu:Label ID="lblCreateFolderName" runat="server" Name="Name"></wu:Label>
                </td>
                <td>
                    <wu:TextBox ID="txtCreateFolderName" runat="server" Button="boxCreateFolderConfirm"></wu:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <wu:Button ID="btnCreateFolderConfirm" runat="server" Name="CreateDirectory" OnClick="boxCreateFolderConfirm_Click"></wu:Button>
                    <wu:Button ID="btnCreateFolderCancel" runat="server" Name="Cancel"></wu:Button>
                </td>
                
            </tr>
        </table>
    </wu:Box>
    <wu:Box ID="box1" runat="server" JavascriptTriggered="true" Dragable="true" Title="MovingReport">
        <table  class="TableSettings moveFolderTable" style="width:400px;">
            <tr>
                    <td>
                        <div class="MoveBox">
                             <input id="reportNameImg" class="BackgroundColor1"  type="image" src="/Images/Icons/Cloud/.lor.png">
                            <div id="reportNameDiv" style="max-height:70px;max-width:350px;overflow: hidden; text-overflow: ellipsis;" ;="" class="DirectoryName" title="move"></div>
                        </div>                           
                        </td>
            </tr>
            <tr>
                    <td class="folderContents">
                        <div id="folderTitle" style="max-height:70px;max-width:350px;overflow: hidden; text-overflow: ellipsis;" ;="" class="DirectoryName" title="folder">Move to folder</div>
                             <asp:TreeView runat="server" ID="TreeView1">    
                     </asp:TreeView>
                        </td>
            </tr>
             <tr>
                    <td colspan="2" align="right">                          
                        <wu:Button ID="ImageMove" runat="server" Name="Move" OnClientClick="return ConfirmMove();" OnClick="ImageMove_Click"></wu:Button>
                         <wu:Button ID="Button1" runat="server" Name="Cancel"></wu:Button>
                        </td>
            </tr>
        </table>
    </wu:Box>     
    <input id="hdfLibraryViewState" name="hdfLibraryViewState" runat="server" />
    <style type="text/css">
        .Box .BoxContent .TableSettings td {
            padding-top: 10px;
            padding-bottom: 10px;
            /*padding-left: 40px;*/
            padding-left: 20px;
            /*padding-right: 40px;*/
        }

        /*.selectedNode {
            background-color: orange;
            color: orange;
        }*/


        .Box .BoxContent .TableSettings td {
            position: relative;
        }
    </style>
    <script type="text/javascript">
        sessionStorage.fileSourceForMove = "";

        function SetLibraryOrderField(orderField) {
            _AjaxRequest("LinkCloud.aspx", "SetLibraryOrderField", "Value=" + orderField, function (response) {
                window.location = window.location;
            });
        }


        var libraryViewState;

        loadFunctions.push(function () {
            libraryViewState = document.getElementById("cphContent_hdfLibraryViewState").value;

            if (libraryViewState != "Items") {
                libraryViewState = "Items";
                SwitchView(document.getElementById("imgSwitchView"));
                libraryViewState = "Grid";
            }
        });

        function SwitchView(sender) {

            var cloudItems = GetChildsByAttribute(document.body, "class", "DirectoryName", true);

            if (libraryViewState == "Items") {
                document.getElementById("pnlGridViewHeadline").style.display = "";

                sender.src = "/Images/Icons/Menu/SwitchViewGrid.png";

                for (var i = 0; i < cloudItems.length; i++) {
                    cloudItems[i].parentNode.className = cloudItems[i].parentNode.className.replace("CloudItem", "CloudItemGrid");

                    while (cloudItems[i].innerHTML.search("<br") != -1) {
                        cloudItems[i].innerHTML = cloudItems[i].innerHTML.replace("<br>", "<span style=\"display:none;\"></span>");
                    }
                }

                libraryViewState = "Grid";
            }
            else {
                document.getElementById("pnlGridViewHeadline").style.display = "none";

                sender.src = "/Images/Icons/Menu/SwitchViewItems.png";

                for (var i = 0; i < cloudItems.length; i++) {
                    cloudItems[i].parentNode.className = cloudItems[i].parentNode.className.replace("CloudItemGrid", "CloudItem");

                    while (cloudItems[i].innerHTML.search("<span style=\"display:none;\"></span>") != -1) {
                        cloudItems[i].innerHTML = cloudItems[i].innerHTML.replace("<span style=\"display:none;\"></span>", "<br>");
                    }
                }

                libraryViewState = "Items";
            }

            _AjaxRequest("LinkCloud.aspx", "SetLibraryViewState", "Value=" + libraryViewState, function (response) {

            });
        }

        function UploadFile(e) {
            e.preventDefault();

            var file = e.dataTransfer.files[0];

            //alert(file);

            var reader = new FileReader();

            reader.onload = function () {
                var buffer = reader.result;

                AjaxRequestBuffer("UploadFile", file.name, buffer, function (response) {
                    alert("Uploaded!");
                });
            };

            reader.readAsArrayBuffer(file);

            return false;
        }

        addEventHandler(window, 'load', function () {
        });

        function addEventHandler(obj, evt, handler) {
            if (obj.addEventListener) {
                // W3C method
                obj.addEventListener(evt, handler, false);
            } else if (obj.attachEvent) {
                // IE method.
                obj.attachEvent('on' + evt, handler);
            } else {
                // Old school method.
                obj['on' + evt] = handler;
            }
        }

        function RenderDirectoryOptions(sender, directory, isDirectoryPrivate) {

            if (isDirectoryPrivate != "NA") {

                if (sender.id == "cphContent_pnlFiles" && (document.getElementById("menuDirectory") != null || document.querySelectorAll("[id*='menuSavedReport']").length > 0))
                    return false;

                sender = sender.getElementsByTagName("input").item(0);
                //if (document.getElementById("menuDirectory") != null && sender.id == "cphContent_imgCreateDirectory" )
                //    return false;

                var menu = InitMenu("menuDirectory");
                if (sender.id != "cphContent_imgCreateDirectory") {
                    var lnkRename = document.createElement("div");

                    lnkRename.ImageUrl = "/Images/Icons/Menu/Rename.png";
                    lnkRename.innerHTML = LoadLanguageText("Rename");
                    lnkRename.MenuItemClick = "RenameDirectory('" + sender.id + "', '" + directory + "', false);";

                    menu.Items.push(lnkRename);

                    var lnkDelete = document.createElement("div");

                    lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
                    lnkDelete.innerHTML = LoadLanguageText("Delete");
                    lnkDelete.MenuItemClick = "DeleteDirectory(this, '" + directory + "', '" + GetChildByAttribute(sender.parentNode, "class", "DirectoryName").innerText.trim() + "');";

                    menu.Items.push(lnkDelete);
                }
                if (sessionStorage.getItem("fileSelectedForMove") != null && sessionStorage.getItem("fileSelectedForMove") != "") {
                    var lnkMoveHere = document.createElement("div");
                    lnkMoveHere.ImageUrl = "/Images/Icons/Menu/Delete.png";
                    lnkMoveHere.innerHTML = LoadLanguageText("MoveHere");
                    lnkMoveHere.MenuItemClick = "MoveReportConfirm('" + encodeURI(sender.getAttribute("source")) + "');";

                    menu.Items.push(lnkMoveHere);
                }

                menu.Render();
            }
        }

        function RenderFileOptions(sender, file, name) {
            if (sender.id == "cphContent_pnlFiles" && (document.getElementById("menuDirectory") != null || document.querySelectorAll("[id*='menuSavedReport']").length > 0))
                return false;
            sender = sender.getElementsByTagName("input").item(0);

            var menu = InitMenu("menuFile");

            var lnkRename = document.createElement("div");

            lnkRename.ImageUrl = "/Images/Icons/Menu/Rename.png";
            lnkRename.innerHTML = LoadLanguageText("Rename");
            lnkRename.MenuItemClick = "RenameDirectory('" + sender.id + "', '" + file + "', true);";

            menu.Items.push(lnkRename);

            var lnkDelete = document.createElement("div");

            lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
            lnkDelete.innerHTML = LoadLanguageText("Delete");
            lnkDelete.MenuItemClick = "DeleteFile(this, '" + file + "', '" + name + "');";

            menu.Items.push(lnkDelete);

            var lnkDuplicate = document.createElement("div");

            lnkDuplicate.ImageUrl = "/Images/Icons/Menu/Duplicate.png";
            lnkDuplicate.innerHTML = LoadLanguageText("DuplicateFile");
            lnkDuplicate.MenuItemClick = "DuplicateFile(this, '" + file + "', '" + name + "');";

            menu.Items.push(lnkDuplicate);

            menu.Render();
        }

        function RenameDirectory(idSender, path, isFile) {
            var sender = document.getElementById(idSender);

            var label = GetChildByAttribute(sender.parentNode, "class", "DirectoryName");

            if (label == undefined)
                return;

            var normalWidth = label.offsetWidth;
            if (document.getElementById("imgSwitchView").src.indexOf("SwitchViewItems") == -1)
                label.style.width = "200px";

            label.setAttribute("contenteditable", "true");
            label.contenteditable = true;
            var str = label.textContent.substring(label.textContent.lastIndexOf("."), label.textContent.length);
            var str2 = label.textContent.substring(0, label.textContent.lastIndexOf("."));
            //label.textContent = str2;
            if (isFile)
                label.textContent = str2;
            else
                label.textContent = str;

            label.focus();
            placeCaretAtEnd(label);


            label.onblur = function () {
                if (label.textContent.length > 248) {
                    ShowMessage(LoadLanguageText("CreateDirectoryErrorPathMaxLength"), "Error");
                    if (isFile)
                        label.textContent = str2 + str;
                    else {
                        label.textContent = str;
                    }
                    return;
                }
                var pathlength = (path.length - (str2 + str).length) + label.textContent.length;
                if (pathlength > 260) {
                    ShowMessage(LoadLanguageText("CreateDirectoryErrorDirectoryFullMaxLength"), "Error");
                    if (isFile)
                        label.textContent = str2 + str;
                    else {
                        label.textContent = str;
                    }
                    return;
                }
                if (/^[a-zA-Z0-9\s]+$/.test(label.textContent)) {
                    if (isFile)
                        label.textContent += str;
                    if (!isFile && label.textContent != str)
                        _AjaxRequest("LinkCloud.aspx", "RenameDirectory", "IsFile=" + isFile + "&Name=" + encodeURI(label.textContent) + "&Path=" + path, function (response) {
                            window.location = window.location;
                        });
                    else if (isFile)
                        _AjaxRequest("LinkCloud.aspx", "RenameDirectory", "IsFile=" + isFile + "&Name=" + encodeURI(label.textContent) + "&Path=" + path, function (response) {
                            window.location = window.location;
                        });

                    this.onblur = undefined;
                }
                else {
                    ShowMessage(LoadLanguageText("CreateDirectorySpecialCharacter"), "Error");
                    if (isFile)
                        label.textContent = str2 + str;
                    else {
                        label.textContent = str;
                    }
                }
                if (!isFile) {
                    label.setAttribute("contenteditable", false);
                    label.style.width = "";
                }
            };

        }



        function DuplicateFile(idSender, path, name) {
            _AjaxRequest("LinkCloud.aspx", "DuplicateFile", "Path=" + path, function (response) {
                window.location = window.location;
            });
        }

        function DeleteDirectory(sender, directory, name) {
            CreateConfirmBox(LoadLanguageText("DeleteCloudDirectory").replace("{0}", name), function () {
                window.location = document.forms[0].action + "?DeleteDirectory=" + directory;
            });
        }

        function DeleteSavedReport(sender, directory, name) {
            CreateConfirmBox(LoadLanguageText("DeleteSavedReport").replace("{0}", name), function () {
                window.location = document.forms[0].action + "?DeleteDirectory=" + directory;
            });
        }

        function DeleteFile(sender, file, name) {
            CreateConfirmBox(LoadLanguageText("DeleteCloudFile").replace("{0}", name), function () {
                window.location = document.forms[0].action + "?DeleteFile=" + file;
            });
        }

        var lastSearchRequest = "";
        var activeVariableSearch = undefined;
        function Search(value) {

            if (value == undefined)
                value = "";

            lastSearchRequest = value;

            if (document.getElementById("LoadingBlur") == undefined)
                ShowLoading(document.getElementById("cphContent_pnlFiles"));

            if (activeVariableSearch != undefined)
                activeVariableSearch.abort();

            activeVariableSearch = _AjaxRequest("LinkCloud.aspx", "Search", "Value=" + encodeURI(value), function (response) {
                activeVariableSearch = undefined;


                var split = response.split('###SPLIT###');

                var searchExpression = split[0];

                if (searchExpression != lastSearchRequest)
                    return;


                document.getElementById("cphContent_pnlFiles").outerHTML = split[1];

                //var scripts = document.getElementById("cphContent_pnlFiles").getElementsByTagName("script");

                //for (var i = 0; i < scripts.length; i++) {
                //    eval(scripts.item(i).innerHTML);
                //}


                if (libraryViewState == "Grid")
                    libraryViewState = "Items";
                else
                    libraryViewState = "Grid";
                

                
                var imgDirs = document.querySelectorAll("[id*='cphContent_imgDirectory'");
                for (var i = 0; i < imgDirs.length; i++) {
                    imgDirs[i].onclick = function () {
                        _AjaxRequest("LinkCloud.aspx", "SearchedFile", "Path=" + encodeURI(this.getAttribute('source')), function (response1) {
                            if (response1.indexOf("Crosstabs") > -1)
                                window.location = response1;
                        });
                    }
                }

                SwitchView(document.getElementById("imgSwitchView"));
                HideLoading();
            });
        }


        function placeCaretAtEnd(el) {
            el.focus();
            if (typeof window.getSelection != "undefined"
                    && typeof document.createRange != "undefined") {
                var range = document.createRange();
                range.selectNodeContents(el);
                range.collapse(false);
                var sel = window.getSelection();
                sel.removeAllRanges();
                sel.addRange(range);
            } else if (typeof document.body.createTextRange != "undefined") {
                var textRange = document.body.createTextRange();
                textRange.moveToElementText(el);
                textRange.collapse(false);
                textRange.select();
            }
        }

        $(".FileExplorerItem").click(function () {
            var elements = document.getElementsByClassName('selectedNode');
            while (elements.length > 0) {
                elements[0].classList.remove('selectedNode');
            }
            console.log(this.classList);
            this.classList.add('selectedNode');
        });

        var FileExplorerItem = document.getElementsByClassName("FileExplorerItem");
        FileExplorerItem.clic
        var isFolderSelectedToMove = false;
        function ConfirmMove() {
            if (!isFolderSelectedToMove) {
                ShowMessage(LoadLanguageText("DestinationDirectoryError"), "Error");
                return false;
            }
            else {

                if (!isFolderSelectedToMove) {
                    ShowMessage(LoadLanguageText("DestinationDirectoryError"), "Error");
                    return false;
                }
                else {
                    isFolderSelectedToMove = false;
                    return true;
                }
            }

        }
    </script>
    <uc1:ConnectPowerBI runat="server" id="ConnectPowerBI" />
</asp:content>
