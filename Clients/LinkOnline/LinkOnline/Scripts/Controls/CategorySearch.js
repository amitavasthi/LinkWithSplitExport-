function InitCategorySearch(idControl, selectionType, source) {
    var control = document.getElementById(idControl);

    control.Source = source;

    control.SearchBox = GetChildByAttribute(control, "type", "text", true);
    control.SearchResults = GetChildByAttribute(control, "class", "CategorySearchResults", true);

    control.SearchBox.Control = control;
    control.SearchResults.Control = control;

    control.SearchBox.setAttribute("onkeyup", "document.getElementById('" + control.id + "').Search();");

    control.SearchResults.style.height = (ContentHeight - 300) + "px";

    control.SelectionType = selectionType;

    control.LastSearchValue = undefined;
    control.ActiveSearchRequests = new Array();
    control.Search = function () {
        var searchValue = "";

        searchValue = control.SearchBox.value;

        if (control.LastSearchValue == searchValue)
            return;

        for (var i = 0; i < control.ActiveSearchRequests.length; i++) {
            control.ActiveSearchRequests[i].abort();
        }

        control.ActiveSearchRequests = new Array();

        control.LastSearchValue = searchValue;

        ShowLoading(control.SearchResults);

        var searchRequest = _AjaxRequest("/Handlers/GlobalHandler.ashx", "SearchCategories", "Id=" + idControl + "&Expression=" + searchValue + "&Source=" + encodeURIComponent(control.Source), function (response, control) {

            var index = control.ActiveSearchRequests.indexOf(searchRequest);
            if (index > -1) {
                control.ActiveSearchRequests.splice(index, 1);
            }

            control.SelectedItems = new Array();
            var searchValue = control.SearchBox.value;

            var split = response.split("##################SPLIT##################");

            if (split[0] != searchValue.trim())
                return;

            control.SearchResults.innerHTML = split[1];

            InitBoxes();
        }, control);
        control.ActiveSearchRequests.push(searchRequest);

        if (document.getElementById("pnlFilterErrors") != null) {
            document.getElementById("pnlFilterErrors").innerHTML = "";
        }
    };

    control.SelectedItems = new Array();

    control.SelectionChanged = undefined;

    control.UpdateSelectedItems = function (confirmed) {
        control.SelectedItems = new Array();

        var inputs = control.SearchResults.getElementsByTagName("input");

        for (var i = 0; i < inputs.length; i++) {
            if (!inputs.item(i).checked)
                continue;

            control.SelectedItems.push({
                Id: inputs.item(i).getAttribute("IdCategory"),
                VariableName: inputs.item(i).getAttribute("VariableName"),
                Name: inputs.item(i).getAttribute("CategoryName"),
                IsTaxonomy: inputs.item(i).getAttribute("IsTaxonomy"),
                IdStudy: inputs.item(i).getAttribute("IdStudy")
            });
        }

        if (control.SelectionChanged != undefined)
            control.SelectionChanged();
    };

    control.Select = function (idCategory) {
        var inputs = control.SearchResults.getElementsByTagName("input");

        for (var i = 0; i < inputs.length; i++) {
            if (inputs.item(i).getAttribute("IdCategory") != idCategory.toString())
                continue;

            inputs.item(i)["Image"].onclick();
        }
    };

    control.Close = function (confirmed) {
        if (confirmed == true && control.OnConfirm != undefined)
            control.OnConfirm(control);
        else if (confirmed == false && control.OnCancel != undefined)
            control.OnCancel();
    };

    control.Search();
}

function ToggleSearchCategoryItem(sender) {
    sender.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.UpdateSelectedItems();

    if (sender.checked) {
        if (document.getElementById("pnlFilterErrors") != null) {
            var parameters = "";
            if (sender.getAttribute("istaxonomy") == "false")
                parameters = "Type=" + "ReportDefinitionCategory" + "&Category=" + sender.getAttribute("idcategory");
            if (sender.getAttribute("istaxonomy") == "true")
                parameters = "Type=" + "ReportDefinitionTaxonomyCategory" + "&Category=" + sender.getAttribute("idcategory");
            if (sender.getAttribute("variablename") != undefined && sender.getAttribute("categoryname") != undefined)
                parameters += "&VariableFilter=" + sender.getAttribute("variablename") + "&CategoryName=" + sender.getAttribute("categoryname");

            if (document.getElementById("cphContent_chkSplitnExportAllTabs") != null && document.getElementById("cphContent_chkSplitnExportAllTabs").checked)
                parameters += "&AllTabs=true";
            else
                parameters += "&AllTabs=false";

            document.getElementById("btnConfirmFilterCategorySearch").style.pointerEvents = "none";
            callCounts++;
            AjaxRequest("ValidateCategory", parameters, function (response) {
                if (response != "") {
                    document.getElementById("pnlFilterErrors").innerHTML += response;
                }
                else {
                    responseCounts++;
                }
                EnableConfirm();
            });
                
        }
        sender.parentNode.className = "";
        sender.parentNode.style.color = "";
    }
    else {
        callCounts--;
        if (document.getElementById("pnlFilterErrors") != null) {
            //remove element if uncheck category
            if (document.getElementById("pnlFilterErrors").innerHTML.indexOf(sender.parentNode.nextSibling.innerHTML) > -1) {
                var spans = document.getElementById("pnlFilterErrors").getElementsByTagName("span");
                var removeElements = [];
                for (var i = 0; i < spans.length; i++) {
                    if (spans[i].innerHTML.indexOf('"' + sender.parentNode.nextSibling.innerHTML + '"') > -1) {
                        removeElements.push(spans[i]);
                    }
                }
                for (var i = 0; i < removeElements.length; i++) {
                    document.getElementById("pnlFilterErrors").removeChild(removeElements[i]);
                }
            }
        }
        sender.parentNode.className = "";
        sender.parentNode.style.color = "";
    }
}

function ExportApplyAllTabs() {
    var inputs = document.querySelectorAll("input[variablename]");
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].checked) {
            inputs[i].checked = false;
            inputs[i].previousSibling.onmouseover();
        }
    }
    document.getElementById("pnlFilterErrors").innerHTML = "";
}