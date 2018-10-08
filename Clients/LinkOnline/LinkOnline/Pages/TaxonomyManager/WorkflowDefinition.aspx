<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="WorkflowDefinition.aspx.cs" Inherits="LinkOnline.Pages.TaxonomyManager.WorkflowDefinition" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSelector2.css"></wu:StylesheetReference>
    <style type="text/css">
        .TreeViewNodeLabel {
            cursor: pointer;
        }


        .WorkflowFilterSelection {
            display: inline-block;
            padding: 10px;
            margin: 10px;
            color: #FFFFFF;
            box-shadow: 0px 0px 2px 0px #cccccc;
        }

        .WorkflowFilterSelectionTitle {
            font-size: 20pt;
            font-weight: bold;
            margin-bottom: 10px;
            cursor: pointer;
        }

        .WorkflowFilterSelectionCategories {
            width: 211px;
            height: 136px;
            overflow: hidden;
        }


        .HierarchyWorkflowNotDefinedMessage {
            font-size: 14pt;
        }

        .HierarchyWorkflowInherited .WorkflowFilterSelection {
            background: #aaaaaa !important;
        }

        .ButtonWorkflowFilterSelectionAdd {
            display: inline-block;
        }

            .ButtonWorkflowFilterSelectionAdd img {
                margin-bottom: 40px;
                cursor: pointer;
            }


        #boxVariableSearchControl .BtnBoxClose {
            display: none;
        }


        .VariableSearchResultItem {
            color: #FFFFFF;
        }

        .VariableSearchResults {
            border-width: 2px;
            border-style: solid;
            border-radius: 5px;
        }


        .LabelHierarchyPath {
            font-size: 14pt;
        }

        .ButtonWorkflowAdd img {
            cursor: pointer;
        }

        .ButtonDeleteHierarchyWorkflow {
            background: #FF0000;
            cursor: pointer;
            height: 50px;
        }

            .ButtonDeleteHierarchyWorkflow:hover {
                background: #ff3b3b;
            }
    </style>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <script type="text/javascript">
        var idSelectedHierarchy = undefined;
        var pathSelectedHierarchy = undefined;
        function LoadWorkflow(sender, idHierarchy, path) {
            idSelectedHierarchy = idHierarchy;
            pathSelectedHierarchy = path;

            var nodes = GetChildsByAttribute(document.getElementById("cphContent_pnlHierarchies"), "class", "TreeViewNode BackgroundColor1", true);

            for (var i = 0; i < nodes.length; i++) {
                nodes[i].className = "TreeViewNode BackgroundColor5";
            }

            sender.parentNode.className = "TreeViewNode BackgroundColor1";
            document.getElementById("lblHierarchyPath").innerHTML = sender.parentNode.getAttribute("Path");


            _AjaxRequest("WorkflowDefinition.aspx", "RenderWorkflowDefinition", "IdHierarchy=" + idHierarchy, function (response) {
                document.getElementById("pnlWorkflowDefinition").innerHTML = response;
                if (document.getElementsByClassName("HierarchyWorkflowInherited").length > 0)
                    if (document.getElementsByClassName("ButtonWorkflowFilterSelectionAdd").length > 0)
                        document.getElementsByClassName("HierarchyWorkflowInherited")[0].appendChild(document.getElementsByClassName("ButtonWorkflowFilterSelectionAdd")[0]);
                    else
                    { document.getElementsByClassName("HierarchyWorkflowInherited")[0].appendChild(document.getElementsByClassName("ButtonWorkflowAdd")[0]); }

            });
        }

        function EditWorkflowFilterSelectionTitle(sender, path) {
            sender.setAttribute("contenteditable", "true");
            sender.focus();
            //SelectElementContents(sender);
            sender.onkeypress = function () {
                if (event.keyCode != 13)
                    return true;

                this.blur();

                return false;
            }
            sender.onblur = function () {
                _AjaxRequest(
                    "WorkflowDefinition.aspx",
                    "SetWorkflowFilterSelectionTitle",
                    "Value=" + sender.innerHTML + "&Path=" + path,
                    function (response) {
                    });
            }

            //alert(decodeURIComponent(path));
        }

        function SearchVariables() {
            var searchValue = document.getElementById("cphContent_txtVariableSearch").value;

            _AjaxRequest("WorkflowDefinition.aspx", "SearchVariables", "EnableDataCheck=False&Expression=" + searchValue, function (response) {
                document.getElementById("variableSearchResults").innerHTML = response;

                InitBoxes();
            });
        }

        idVariableSearchSelectedItem = undefined;
        function SelectVariableSelectorItem(sender) {
            var selectedItems = GetChildsByAttribute(sender.parentNode, "class", "VariableSearchResultItem VariableSelector GreenBackground");

            for (var i = 0; i < selectedItems.length; i++) {
                selectedItems[i].className = "VariableSearchResultItem VariableSelector BackgroundColor1";
            }

            sender.className = "VariableSearchResultItem VariableSelector GreenBackground";

            idVariableSearchSelectedItem = sender.getAttribute("IdVariable");
        }

        function AddWorkflowFilterSelection(idVariable) {
            if (idVariable == undefined)
                return;

            _AjaxRequest("WorkflowDefinition.aspx", "AddWorkflowFilterSelection", "IdVariable=" + idVariable + "&Path=" + pathSelectedHierarchy, function (response) {
                LoadWorkflow(
                    document.getElementById("cphContent__tvnHierarchy" + idSelectedHierarchy),
                    idSelectedHierarchy,
                    pathSelectedHierarchy
                );
            });
        }

        var test;
        function WorkflowFilterSelection_ContextMenu(sender, path) {
            test = GetChildByAttribute(sender, "class", "WorkflowFilterSelectionTitle");

            var menu = InitMenu("menuWorkflowFilter");


            var lnkRename = document.createElement("div");

            lnkRename.ImageUrl = "/Images/Icons/Menu/Rename.png";
            lnkRename.innerHTML = LoadLanguageText("Rename");
            lnkRename.MenuItemClick = "EditWorkflowFilterSelectionTitle(test, '" + path + "')";

            menu.Items.push(lnkRename);

            var lnkDelete = document.createElement("div");

            lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
            lnkDelete.innerHTML = LoadLanguageText("Delete");
            lnkDelete.MenuItemClick = "DeleteWorkflowFilter('" + path + "')";

            menu.Items.push(lnkDelete);


            menu.Render();
        }

        function DeleteWorkflowFilter(path) {
            _AjaxRequest("WorkflowDefinition.aspx", "DeleteWorkflowFilterSelection", "Path=" + path, function (response) {
                LoadWorkflow(
                    document.getElementById("cphContent__tvnHierarchy" + idSelectedHierarchy),
                    idSelectedHierarchy,
                    pathSelectedHierarchy
                );
            });
        }

        function DeleteWorkflow() {
            CreateConfirmBox(LoadLanguageText("DeleteWorkflow"), function () {
                _AjaxRequest("WorkflowDefinition.aspx", "DeleteWorkflow", "Path=" + pathSelectedHierarchy, function (response) {
                    LoadWorkflow(
                        document.getElementById("cphContent__tvnHierarchy" + idSelectedHierarchy),
                        idSelectedHierarchy,
                        pathSelectedHierarchy
                    );
                });
            });
        }

        function ButtonWorkflowAdd_Click() {
            _AjaxRequest("WorkflowDefinition.aspx", "AddWorkflow", "IdHierarchy=" + idSelectedHierarchy, function (response) {
                LoadWorkflow(
                    document.getElementById("cphContent__tvnHierarchy" + idSelectedHierarchy),
                    idSelectedHierarchy,
                    pathSelectedHierarchy
                );
            });
        }
    </script>
    <table style="width:100%;height:100%;">
        <tr valign="top">
            <td class="BackgroundColor7" rowspan="2" style="width:250px;min-width:250px;">
                <asp:Panel ID="pnlHierarchies" runat="server"></asp:Panel>
            </td>
            <td class="BackgroundColor7" style="height: 50px;" id="tdStudiesHeadline">
                <div style="float:right;">
                    <img src="/Images/Icons/Delete3.png" class="ButtonDeleteHierarchyWorkflow" onclick="DeleteWorkflow();" />
                </div>
                <div style="margin:10px;">
                    <table><tr><td><div id="lblHierarchyPath" class="LabelHierarchyPath Color1"></div></td></tr></table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div id="pnlWorkflowDefinition" style="text-align: center;"></div>
            </td>
        </tr>
    </table>

    <wu:Box ID="boxVariableSearch" Height="500" runat="server" Title="VariableSearch" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true" OnClientClose="if(!filtersChanged) return; window.setTimeout(PopulateCrosstable, 500); filtersChanged = false;" HideCloseButton="True">
        <div style="margin-top:-100px;margin-bottom:100px;">
            <div id="btnCancelFilterCategorySearch" class="CancelCircle" onclick="CloseBox('boxVariableSearchControl', 'Bottom');" style="position:absolute;"></div>
            <div id="btnConfirmFilterCategorySearch" class="OkCircle" onclick="CloseBox('boxVariableSearchControl', 'Bottom');AddWorkflowFilterSelection(idVariableSearchSelectedItem)" style="position:absolute;margin-left: 1035px;"></div>
        </div>
        <div class="PnlVariableSelectors">
            <div style="margin:1em;">
                <select id="ddlVariableSearchChapter" style="display:none;"></select>
                <asp:TextBox ID="txtVariableSearch" runat="server" type="text" onkeyup="SearchVariables();" style="width:99%;border-radius:5px;border-style:solid;font-size:16pt;" class="Color1 BorderColor1"  />
            </div>
            <div id="variableSearchResults" class="VariableSearchResults BorderColor1" style="width:1000px;height:400px;margin:1em;overflow:auto;">

            </div>
        </div>
        <wu:TipGallery ID="tgVariableSearch" runat="server" _TipItems="VariableSearchTip1, VariableSearchTip2" />
    </wu:Box>
</asp:content>
