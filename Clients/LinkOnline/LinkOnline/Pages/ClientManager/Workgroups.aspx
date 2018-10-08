<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Workgroups.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.Workgroups" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <style type="text/css">
        .WorkgroupItem {
            color: #FFFFFF;
            cursor: pointer;
            border-top: 1px solid #FFFFFF;
        }

        .WorkgroupItemName {
            padding-left: 1em;
            font-size: 16pt;
            width: 100%;
        }

        .WorkgroupItem .WorkgroupItemExpand {
            visibility: hidden;
            width: 60px;
        }

        .WorkgroupItem:hover .WorkgroupItemExpand {
            visibility: visible;
        }


        .WorkgroupHierachies {
            background: #FFFFFF;
        }

        .WorkgroupHierachies .TreeViewNodeLabel {
            color: #444444 !important;
            padding: 5px;
        }

        .WorkgroupHierachies .TreeViewNodeExpander {
            display:none;
        }
    </style>
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1">
        <wu:Label ID="lblPageTitle" runat="server" Name="ManageWorkgroupsTitle"></wu:Label>
            <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
        
    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <script type="text/javascript">
        function ShowWorkgroupHierarchies(idWorkgroup) {
            //document.getElementById("pnlBoxBackgroundWorkgroupHierarchies").style.display = "";
            var control = document.getElementById("cphContent_pnlWorkgroupContainer" + idWorkgroup);

            var background = document.createElement("div");
            background.id = "WorkgroupHierarchiesBackground" + idWorkgroup;
            background.className = "BoxBackground";

            document.body.appendChild(background);

            background.style.width = "0px";
            background.style.height = "0px";
            background.style.display = "";

            IncreaseWidth(background, window.innerWidth);
            IncreaseHeight(background, window.innerHeight, undefined, true);

            window.setTimeout(function () {
                var controlHeight = control.offsetHeight;

                control.style.zIndex = 10001;
                control.style.position = "absolute";
                control.style.width = (control.offsetWidth - 10) + "px";
                //control.style.top = "0px";

                var hierarchyContainer = document.getElementById("cphContent_pnlWorkgroupHierachies" + idWorkgroup);
                hierarchyContainer.style.height = "";
                //control.style.overflow = "hidden";
                control.style.overflowY = "auto";
                hierarchyContainer.style.visibility = "hidden";
                hierarchyContainer.style.display = "";
                control.style.height =  "500px";
                var target = control.offsetHeight;
                //control.style.top = ((window.innerHeight / 2) - (target / 2)) + "px";

                hierarchyContainer.style.visibility = "";
                //control.style.height = controlHeight + "px";
              
                var imgWorkgroupItemExpand = document.getElementById("imgWorkgroupItemExpand" + idWorkgroup);
                imgWorkgroupItemExpand.src = "/Images/Icons/VariableSelector/Up.png";
                imgWorkgroupItemExpand.parentNode.setAttribute("onclick", "HideWorkgroupHierarchies('" + idWorkgroup + "');");

                IncreaseHeight(control, target, undefined, true);
            }, 800);
        }

        function HideWorkgroupHierarchies(idWorkgroup) {
            //document.getElementById("pnlBoxBackgroundWorkgroupHierarchies").style.display = "none";
            var control = document.getElementById("cphContent_pnlWorkgroupContainer" + idWorkgroup);

            DecreaseHeight(control, 60, function () {
                var hierarchyContainer = document.getElementById("cphContent_pnlWorkgroupHierachies" + idWorkgroup);

                hierarchyContainer.style.display = "none";

                control.style.position = "";
                control.style.top = "";
                control.style.marginTop = "";
                control.style.width = "";
                control.style.height = "";

                var background = document.getElementById("WorkgroupHierarchiesBackground" + idWorkgroup);
                
                DecreaseWidth(background, 0);
                DecreaseHeight(background, 0, function () {
                    background.style.display = "";
                    background.parentNode.removeChild(background);
                }, true);

                var imgWorkgroupItemExpand = document.getElementById("imgWorkgroupItemExpand" + idWorkgroup);
                imgWorkgroupItemExpand.src = "/Images/Icons/VariableSelector/Down.png";
                imgWorkgroupItemExpand.parentNode.setAttribute("onclick", "ShowWorkgroupHierarchies('" + idWorkgroup + "');");
            }, true);
        }

        function ToggleWorkgroupHierarchyCheckbox(idWorkgroup, idHierarchy) {
            _AjaxRequest("Workgroups.aspx", "ToggleWorkgroupHierarchyCheckbox", "IdWorkgroup=" + idWorkgroup + "&IdHierarchy=" + idHierarchy);
        }

        function EditWorkgroup(idWorkgroup) {
            var menu = InitMenu("menuWorkgroup" + idWorkgroup);


            var lnkRename = document.createElement("div");

            lnkRename.ImageUrl = "/Images/Icons/Menu/Rename.png";
            lnkRename.innerHTML = LoadLanguageText("Rename");
            lnkRename.MenuItemClick = "RenameWorkgroup('" + idWorkgroup + "')";

            menu.Items.push(lnkRename);

            var lnkDelete = document.createElement("div");

            lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
            lnkDelete.innerHTML = LoadLanguageText("Delete");
            lnkDelete.MenuItemClick = "DeleteWorkgroup('" + idWorkgroup + "')";

            menu.Items.push(lnkDelete);


            menu.Render();
        }

        function RenameWorkgroup(idWorkgroup) {
            var label = document.getElementById("lblWorkgroupName" + idWorkgroup);

            var txtLabel = document.createElement("input");
            txtLabel.type = "text";
            txtLabel.value = label.innerHTML;

            txtLabel.onblur = function () {
                _AjaxRequest("Workgroups.aspx", "RenameWorkgroup", "IdWorkgroup=" + idWorkgroup + "&Name=" + encodeURIComponent(this.value));

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

        function DeleteWorkgroup(idWorkgroup) {
            CreateConfirmBox(LoadLanguageText("DeleteWorkgroupMessage").replace("{0}", document.getElementById("lblWorkgroupName" + idWorkgroup).innerHTML), function () {
                _AjaxRequest("Workgroups.aspx", "DeleteWorkgroup", "IdWorkgroup=" + idWorkgroup, function () {
                    window.location = window.location;
                });
            });
        }
    </script>
    <div style="margin:1em;">
        <div style="padding:10px;">
            <img style="cursor:pointer;" onclick="InitDragBox('boxAddWorkgroupControl');" src="/Images/Icons/Add.png" onmouseover="this.src='/Images/Icons/Add4.png'" onmouseout="this.src='/Images/Icons/Add.png'" />
        </div>
        <asp:Panel ID="pnlGridContainer" runat="server"></asp:Panel>
    </div>
    <wu:Box ID="boxAddWorkgroup" runat="server" Title="AddWorkgroup" TitleLanguageLabel="true" Dragable="true" JavascriptTriggered="true">
        <table>
            <tr>
                <td>
                    <wu:Label ID="lblAddWorkgroupName" runat="server" Name="Name"></wu:Label>
                </td>
                <td>
                    <wu:TextBox ID="txtAddWorkgroupName" runat="server"></wu:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <wu:Button ID="btnAddWorkgroupConfirm" runat="server" Name="Add" OnClick="btnAddWorkgroupConfirm_Click"></wu:Button>
                    <wu:Button ID="btnAddWorkgroupCancel" runat="server" Name="Cancel"></wu:Button>
                </td>
            </tr>
        </table>
    </wu:Box>
</asp:content>
