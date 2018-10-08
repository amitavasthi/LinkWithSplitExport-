var filtersChanged = false;

function EnableFilterCategorySearch(sender, fileName, path) {
    InitDragBox('boxFilterSearchControl', 'Top');

    var background = GetChildByAttribute(document.getElementById("boxFilterSearch"), "class", "BoxBackground");
    background.style.zIndex = 10000;
    document.getElementById("boxFilterSearchControl").style.zIndex = 10001;

    var categorySearch = document.getElementById("cphContent_csFilterDefinition");

    categorySearch.LastSearchValue = undefined;
    categorySearch.Search();

    if (document.getElementById("pnlFilterErrors") == null)
        document.getElementById("btnConfirmFilterCategorySearch").style.pointerEvents = "";

    //only for split and export or remove checkbox
    if (document.getElementById("cphContent_chkSplitnExportAllTabs") != null) {
        var span = document.getElementById("cphContent_chkSplitnExportAllTabs").parentNode;
        span.style.display = "none";
    }
    categorySearch.OnConfirm = function () {
        DisableFilterCategorySearch(fileName, path);
        document.querySelectorAll(".CategorySearchBox")[1].value = "";
    };
    categorySearch.OnCancel = function () {
        document.querySelectorAll(".CategorySearchBox")[1].value = "";
        CloseBox("boxFilterSearchControl", "Top");
    };
}

function DisableFilterCategorySearch(fileName, path) {

    var requests = 0;

    var taxonomyCategories = "";
    var categories = "";

    var categorySearch = document.getElementById("cphContent_csFilterDefinition");

    for (var i = 0; i < categorySearch.SelectedItems.length; i++) {
        if (categorySearch.SelectedItems[i].IsTaxonomy == "false") {
            categories += categorySearch.SelectedItems[i].Id + ",";
        }
        else {
            taxonomyCategories += categorySearch.SelectedItems[i].Id + ",";
        }
    }

    if (taxonomyCategories.length > 0) {
        AjaxRequest("AddFilterCategories", "FileName=" + fileName + "&XPath=" + path + "&Type=" + "ReportDefinitionTaxonomyCategory" + "&Categories=" + taxonomyCategories, function (response) {
            filtersChanged = true;

            UpdateFilterView(fileName);
        });
    }

    if (categories.length > 0) {
        AjaxRequest("AddFilterCategories", "FileName=" + fileName + "&XPath=" + path + "&Type=" + "ReportDefinitionCategory" + "&Categories=" + categories, function (response) {
            filtersChanged = true;

            UpdateFilterView(fileName);
        });
    }

    CloseBox("boxFilterSearchControl", "Top");
}

function SearchFilterCategories(sender) {
    var searchValue = "";

    if (sender != undefined)
        searchValue = sender.value;

    AjaxRequest("SearchFilterCategories", "Expression=" + searchValue, function (response) {
        var result = JSON.parse(response);

        var searchValue = "";

        if (sender != undefined)
            searchValue = sender.value;

        if (result.SearchExpression != searchValue)
            return;

        var filterSearchResults = document.getElementById("filterSearchResults");
        filterSearchResults.innerHTML = "";

        for (var i = 0; i < result.Results.length; i++) {
            var html = "";

            html += "<table cellspacing=\"0\" cellpadding=\"0\"><tr><td class=\"FilterSearchResultVariable BackgroundColor1\" colspan=\"2\">" + result.Results[i].Label + "</td></tr>";

            for (var c = 0; c < result.Results[i].Categories.length; c++) {
                html += "<tr><td class=\"BackgroundColor1\" style=\"width:20px\"><td><input IsTaxonomy=\"" + result.Results[i].IsTaxonomy + "\" IdCategory=\"" + result.Results[i].Categories[c].Id + "\" type=\"checkbox\" onclick=\"ToggleFilterSearchCategory(this);\" />" + result.Results[i].Categories[c].Label + "</td></tr>";
            }

            html += "</table>";

            filterSearchResults.innerHTML += html;
        }
    });
}

function ToggleFilterSearchCategory(sender) {
    if (sender.checked) {
        sender.parentNode.className = "GreenBackground";
        sender.parentNode.style.color = "#FFFFFF";
    }
    else {
        sender.parentNode.className = "";
        sender.parentNode.style.color = "";
    }
}

function UpdateFilterView(fileName) {
    AjaxRequest("UpdateFilterView", "FileName=" + fileName, function (response) {
        var control = document.getElementById("cphContent_pnlFilterCategories");

        if (control == undefined)
            alert("Wrong ID");

        control.innerHTML = response;

        EvaluateScripts(control);

        HideLoading();
    });
}

function ChangeFilterCategoryOperator(fileName, xPath, type) {
    var pnl = document.getElementById("boxFilterControl");

    if (pnl != undefined)
        ShowLoading(pnl);

    filtersChanged = true;

    AjaxRequest("ChangeFilterCategoryOperator", "Type=" + type + "&FileName=" + fileName + "&XPath=" + xPath, function (response) {
        UpdateFilterView(fileName);
    });
}

function AddFilterCategory(fileName, xPath, idCategory, type) {
    var pnl = document.getElementById("boxFilterControl");

    if (pnl != undefined)
        ShowLoading(pnl);

    filtersChanged = true;

    AjaxRequest("AddFilterCategory", "IdCategory=" + idCategory + "&FileName=" + fileName + "&XPath=" + xPath + "&Type=" + type, function (response) {
        UpdateFilterView(fileName);
    });
}

function DeleteFilterCategory(fileName, xPath, idCategory) {
    var pnl = document.getElementById("boxFilterControl");

    if (pnl != undefined)
        ShowLoading(pnl);

    filtersChanged = true;

    AjaxRequest("DeleteFilterCategory", "IdCategory=" + idCategory + "&FileName=" + fileName + "&XPath=" + xPath, function (response) {
        UpdateFilterView(fileName);
    });
}

function AddFilterCategoryOperator(fileName, xPath) {
    var pnl = document.getElementById("boxFilterControl");

    if (pnl != undefined)
        ShowLoading(pnl);

    AjaxRequest("AddFilterCategoryOperator", "XPath=" + xPath + "&FileName=" + fileName, function (response) {
        UpdateFilterView(fileName);
    });
}

function DeleteFilterCategoryOperator(fileName, xPath) {
    var pnl = document.getElementById("boxFilterControl");

    if (pnl != undefined)
        ShowLoading(pnl);

    filtersChanged = true;

    AjaxRequest("DeleteFilterCategoryOperator", "XPath=" + xPath + "&FileName=" + fileName, function (response) {
        UpdateFilterView(fileName);
    });
}


function EnableFilterCategorySearchForExport() {
    InitDragBox('boxFilterSearchControl', 'Top');

    if (document.getElementById("cphContent_csFilterDefinition") != null && document.getElementById("pnlFilterErrors") != null)
        document.getElementById("cphContent_csFilterDefinition").removeChild(document.getElementById("pnlFilterErrors"));

    if (document.getElementById("cphContent_pnlReportTabs").getElementsByClassName("ReportTab").length > 1) {
        if (document.getElementById("cphContent_chkSplitnExportAllTabs") != null) {
            var span = document.getElementById("cphContent_chkSplitnExportAllTabs").parentNode;
            span.style.display = "";

            document.getElementById("cphContent_chkSplitnExportAllTabs").checked = true;
            document.getElementById("cphContent_chkSplitnExportAllTabs").previousSibling.onmouseover();
        }
    }
    else {
        if (document.getElementById("cphContent_chkSplitnExportAllTabs") != null) {
            document.getElementById("cphContent_chkSplitnExportAllTabs").checked = false;
            document.getElementById("cphContent_chkSplitnExportAllTabs").previousSibling.onmouseover();

            var span = document.getElementById("cphContent_chkSplitnExportAllTabs").parentNode;
            span.style.display = "none";
        }
    }

    var background = GetChildByAttribute(document.getElementById("boxFilterSearch"), "class", "BoxBackground");
    background.style.zIndex = 10000;
    document.getElementById("boxFilterSearchControl").style.zIndex = 10001;

    //create dom element to show error msg.
    var pnlFilterErrors = document.createElement("div");
    pnlFilterErrors.id = "pnlFilterErrors";
    pnlFilterErrors.style.height = "90px";
    pnlFilterErrors.style.background = "#F6F6F6";
    //pnlFilterErrors.style.border = "1px solid #444444";
    pnlFilterErrors.style.fontSize = "10pt";
    pnlFilterErrors.style.padding = "5px";
    pnlFilterErrors.style.overflowY = "auto";
    pnlFilterErrors.style.margin = "1em";

    //assign text as search variable
    var prevText = document.getElementById("boxFilterSearchControl").getElementsByClassName("BoxTitle")[0].innerText;
    document.getElementById("boxFilterSearchControl").getElementsByClassName("BoxTitle")[0].innerText = "search variable";

    document.getElementById("cphContent_csFilterDefinition").appendChild(pnlFilterErrors);


    var categorySearch = document.getElementById("cphContent_csFilterDefinition");

    categorySearch.LastSearchValue = undefined;
    categorySearch.Search();

    categorySearch.OnConfirm = function () {
        if (document.getElementById("pnlFilterErrors") != null && document.getElementById("pnlFilterErrors").innerHTML != "") {
            //dont do anything if true
        }
        else {
            DisableFilterCategorySearchForExport();
            document.querySelectorAll(".CategorySearchBox")[1].value = "";
        }
    };
    categorySearch.OnCancel = function () {
        document.querySelectorAll(".CategorySearchBox")[1].value = "";
        CloseBox("boxFilterSearchControl", "Top");
        document.getElementById("boxFilterSearchControl").getElementsByClassName("BoxTitle")[0].innerText = prevText;
        document.getElementById("cphContent_csFilterDefinition").removeChild(pnlFilterErrors);
        //document.getElementsByClassName("CategorySearch")[0].getElementsByTagName("div")[0].removeChild(applyAllTabs);
    };
}

function DisableFilterCategorySearchForExport() {
    var requests = 0;

    var taxonomyCategories = "";
    var categories = "";
    var variableName = "";
    var categoryNames = "";
    var categorySearch = document.getElementById("cphContent_csFilterDefinition");

    var prevVarName = "";
    for (var i = 0; i < categorySearch.SelectedItems.length; i++) {

        if (prevVarName != "" && categorySearch.SelectedItems[i].VariableName != prevVarName) {
            ShowMessage(LoadLanguageText("TableExportRestrict"), "Warning");
            return false;
        }
        else if (prevVarName == "" || categorySearch.SelectedItems[i].VariableName == prevVarName) {
            prevVarName = categorySearch.SelectedItems[i].VariableName;
        }
    }


    for (var i = 0; i < categorySearch.SelectedItems.length; i++) {
        if (categorySearch.SelectedItems[i].IsTaxonomy == "false") {
            categories += categorySearch.SelectedItems[i].Id + ",";
            categoryNames += categorySearch.SelectedItems[i].Name + ",";
        }
        else {
            taxonomyCategories += categorySearch.SelectedItems[i].Id + ",";
            categoryNames += categorySearch.SelectedItems[i].Name + ",";
        }
    }

    variableName = categorySearch.SelectedItems[0].VariableName;

    CloseBox("boxFilterSearchControl", "Top");
    if (document.getElementById("cphContent_pnlReportTabs").getElementsByClassName("ReportTab").length > 1) {
        if (document.getElementById("cphContent_chkSplitnExportAllTabs").checked) {
            ExportAllTabs(variableName, categoryNames, categories, taxonomyCategories);
        }
        else
            ExportTable(variableName, categoryNames, categories, taxonomyCategories);
    }
    else {
        ExportTable(variableName, categoryNames, categories, taxonomyCategories);
    }
}