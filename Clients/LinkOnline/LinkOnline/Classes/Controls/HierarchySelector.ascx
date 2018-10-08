<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HierarchySelector.ascx.cs" Inherits="LinkOnline.Classes.Controls.HierarchySelector" %>

<style type="text/css">
    .HierarchySelector {
        white-space: nowrap;
    }

    .HierarchySelectorSection {
        white-space: initial;
        text-align: center;
        background: #FFFFFF;
        width: 1000px;
    }

    .HierarchySelectorItem {
        display: inline-block;
        cursor: pointer;
    }

    .HierarchySelectorItem_Spacer {
        margin: 10px;
    }

    .HierarchySelectorItem_Active {
        color: #FFFFFF;
    }


    .HierarchyConfirmButton {
        padding: 5px;
        text-align: center;
        font-size: 16pt;
        color: #FFFFFF;
        cursor: pointer;
    }


    #boxHierarchySelectorControl {
        background: transparent !important;
    }

        #boxHierarchySelectorControl .BoxTitle {
            display: none;
        }
</style>

<script type="text/javascript">
    function LoadHierarchySelectedItems(fileName) {
        AjaxRequest("LoadHierarchySelectedItems", "FileName=" + fileName, function (response) {
            LoadHierarchySelector();
        });
    }

    function LoadHierarchySelector() {
        AjaxRequest("HierarchySelectorRender", "", function (response) {
            document.getElementById("cphContent_HierarchySelector_pnlHierarchySections").innerHTML = response;

            //window.setTimeout(RepositionHierarchySelector, 100);
            RepositionHierarchySelector();
            InitBoxes();
        });
    }

    function SelectHierarchySelectorItem(section, idHierarchy) {
        var chkHierarchy;
        if (section != "00000000-0000-0000-0000-000000000000")
            if (document.getElementById("chkHierarchyAllTabs") != null)
                chkHierarchy = document.getElementById("chkHierarchyAllTabs").checked;
            else
                chkHierarchy = false;
            else
            chkHierarchy = false;
        AjaxRequest("HierarchySelectorRender", "HierarchySelectionSection=" + section + "&HierarchySelection=" + idHierarchy, function (response) {
            document.getElementById("cphContent_HierarchySelector_pnlHierarchySections").innerHTML = response;

            window.setTimeout(RepositionHierarchySelector, 100);

             document.getElementById("chkHierarchyAllTabs").checked = chkHierarchy;
            InitBoxes();
        });

    }

    function ConfirmHierarchySelection(idchkHierarchyAllTabs) {
        AjaxRequest("HierarchySelectorConfirm", "AllHierarchy=" + document.getElementById("chkHierarchyAllTabs").checked, function (response) {
            ShowLoading(document.body);
            window.location = window.location;
        });

    }

    function RepositionHierarchySelector() {
        /*if (flow == undefined)
            flow = true;*/

        var box = document.getElementById("boxHierarchySelectorControl");

        var left = ((window.innerWidth / 2) - (box.offsetWidth / 2));
        var top = ((window.innerHeight / 2) - (box.offsetHeight / 2));

        box.style.left = left + "px";

        var offsetTop = box.offsetTop;

        if (parseInt(top) != offsetTop) {
            if (top < offsetTop) {
                DecreaseTop(box, top);
            }
            else {
                IncreaseTop(box, top);
            }
        }
    }
</script>

<wu:Box ID="boxHierarchySelector" Position="Center" runat="server" Style="" Title="HierarchySelectorTitle" JavascriptTriggered="true" TitleLanguageLabel="true" Dragable="true" OnClientClose="">
    <asp:Panel ID="pnlHierarchySections" runat="server" CssClass="HierarchySelector"></asp:Panel>
    <div style="clear: both"></div>
</wu:Box>
