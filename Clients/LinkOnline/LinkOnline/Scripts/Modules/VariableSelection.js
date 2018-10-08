function EnabledVariableSearch(idHoverBox, _id, sender, idHbCategories, source, path) {
    sender.parentNode.parentNode.parentNode.style.opacity = "1.0";

    try {
        HideHoverBox(idHoverBox, sender.id, "", "onclick", undefined, true);
        HideHoverBox(idHbCategories, undefined, "", "onmouseover", undefined, true);

        activeHoverBoxes[idHoverBox] = undefined;
        activeHoverBoxes[idHbCategories] = undefined;
    }
    catch(e){}

    window.setTimeout(function () {
        if (activeHoverBoxes[idHoverBox] == undefined)
            return;

        var content = document.getElementById("Content");
        var pnlData = document.getElementById("pnlData");

        var scrollLeft = 0;

        scrollLeft += content.scrollLeft;

        if (pnlData != undefined)
            scrollLeft += pnlData.scrollLeft;

        var hoverBox = document.getElementById(idHoverBox);

        var marginLeft = ((hoverBox.parentNode.offsetWidth / 2) - (scrollLeft + 150));

        if ((marginLeft + hoverBox.offsetWidth + hoverBox.offsetLeft) > window.innerWidth) {
            marginLeft -= 150;
        }

        if ((marginLeft + hoverBox.offsetLeft) < 0) {
            marginLeft = 50;
        }

        hoverBox.style.marginLeft = marginLeft + "px";
        hoverBox.style.marginTop = "20px";

        BlurTable2();

        activeHoverBoxes[idHoverBox]["Locked"] = "True";

        var pnlSearch = document.createElement("div");

        var txtSearch = document.createElement("input");
        txtSearch.id = "txtSearch" + sender.id;
        txtSearch.type = "text";
        txtSearch.className = "BorderColor1";
        txtSearch.setAttribute("autocomplete", "off");

        var imgSearch = document.createElement("img");
        imgSearch.src = "/Images/Icons/Search2.png";

        pnlSearch.appendChild(txtSearch);
        pnlSearch.appendChild(imgSearch);

        sender.parentNode.parentNode.insertBefore(pnlSearch, sender.parentNode);

        if (sender["Searching"] == "True")
            return;

        var text = sender.innerHTML;

        sender["Searching"] = "True";

        //txtSearch.setAttribute("onblur", "DisableVariableSearch(this, document.getElementById('" + sender.id + "'), '" + idHoverBox + "', '" + idHbCategories + "', '" + sender.id + "');")
        var blur = document.getElementById("BlurTable");
        blur.setAttribute("onclick", "DisableVariableSearch(document.getElementById('" + txtSearch.id + "'), document.getElementById('" + sender.id + "'), '" + idHoverBox + "', '" + idHbCategories + "', '" + sender.id + "');")

        txtSearch.value = "";

        txtSearch.style.width = "258" + "px";

        pnlSearch.style.position = "absolute";
        pnlSearch.style.zIndex = "1001";
        pnlSearch.style.marginLeft = marginLeft + "px";
        pnlSearch.style.width = "305px";

        //if (sender.parentNode.parentNode.parentNode.className == "TableVariableSelector TableNestedVariableSelector") {
        pnlSearch.style.marginTop = (-15 - GetParentScrollTop(hoverBox)) + "px";
        if (((hoverBox.offsetTop + hoverBox.offsetHeight) - GetParentScrollTop(hoverBox)) > window.innerHeight) {
            pnlSearch.style.marginTop = ((-15 - GetParentScrollTop(hoverBox)) - ((hoverBox.offsetTop + hoverBox.offsetHeight) - GetParentScrollTop(hoverBox) - window.innerHeight + 20)) + "px";
        }

        hoverBox.style.marginTop = (20 - GetParentScrollTop(hoverBox)) + "px";

        if ((hoverBox.offsetTop + hoverBox.offsetHeight) > window.innerHeight) {
            var moveTop = ((20 - GetParentScrollTop(hoverBox)) - ((hoverBox.offsetTop + hoverBox.offsetHeight) - window.innerHeight + 20));

            hoverBox.setAttribute("MoveTop", ((hoverBox.offsetTop + hoverBox.offsetHeight) - window.innerHeight + 20));

            hoverBox.style.marginTop = moveTop + "px";
        }
        //}

        imgSearch.style.float = "right";


        txtSearch.onkeyup = function () {
            LoadVariables(idHoverBox, _id, this, idHbCategories, source, path);
        };

        LoadVariables(idHoverBox, _id, "", idHbCategories, source, path);

        txtSearch.focus();
    }, 50);
}

function DisableVariableSearch(sender, label, idHoverBox, idHbCategories, idTrigger) {
    UnBlurTable();

    sender.parentNode.parentNode.removeChild(sender.parentNode);

    label["Searching"] = undefined;

    activeHoverBoxes[idHoverBox]["Locked"] = undefined;

    //sender.value = text;
    //window.setTimeout(function () {
        HideHoverBox(idHoverBox, idTrigger, "", "onclick", undefined, true);
        HideHoverBox(idHbCategories, undefined, "", "onmouseover", undefined, true);

        activeHoverBoxes[idHoverBox] = undefined;
        activeHoverBoxes[idHbCategories] = undefined;
    //}, 200);
}

function LoadVariables(idHoverBox, _id, searchBox, idHbCategories, source, path) {
    //if (searchValue.length < 1)
    //    return;

    var hoverBox = document.getElementById(idHoverBox);

    if (activeHoverBoxes[idHbCategories] != undefined) {
        activeHoverBoxes[idHbCategories]["Hide"](undefined, true);
    }

    hoverBox.innerHTML = "";
    ShowLoading(hoverBox);

    var searchValue = searchBox.value;

    if (searchValue == undefined)
        searchValue = "";

    AjaxRequest("SearchVariables", "EnableDataCheck=False&Expression=" + searchValue, function (response) {
        var hoverBox = document.getElementById(idHoverBox);

        var content = document.getElementById("Content");

        var variables;

        try {
            // Parse the response json script.
            variables = JSON.parse(response);
        }
        catch (e) {
            return;
        }

        var searchValue = searchBox.value;

        if (searchValue == undefined)
            searchValue = "";

        searchValue = searchValue.trim();

        if (variables.SearchExpression != searchValue)
            return;

        hoverBox.innerHTML = "";

        var idSelected = undefined;

        if (document.getElementById(_id) != undefined)
            idSelected = document.getElementById(_id).getAttribute("IdSelected");

        // Run through all variables.
        for (var i = 0; i < variables.Variables.length; i++) {
            var variable = variables.Variables[i];

            var cssClass = "VariableSelectionItem Color1 BackgroundColor4";

            if (variable.HasData == "False") {
                cssClass += " VariableSelectionItemNoData";
            }

            var control = document.createElement("div");
            control.id = "VariableSelectionItem" + idHoverBox + variable.Id;
            control.className = cssClass;
            control.setAttribute("_id", _id);
            control.setAttribute("IsTaxonomy", variable.IsTaxonomy);
            control.setAttribute("onmouseover", "LoadCategories('" + variable.Id + "', '2057', '" + idHbCategories + "', this.id);");
            control.setAttribute("onselectstart", "return false;");
            control.setAttribute("onclick", "SelectVariable('" + variable.Id + "', '" + variable.IsTaxonomy + "', '" + source + "', '" + path + "', '" + idSelected + "');");

            control.innerHTML = variable.Label;

            hoverBox.appendChild(control);

            /*hoverBox.innerHTML += "<div id='VariableSelectionItem" + idHoverBox + variable.Id + "' class=\"" + cssClass + "\" _id=\"" + _id + "\" " +
                "IsTaxonomy=\"" + variable.IsTaxonomy + "\" onmouseover=\"LoadCategories('" + variable.Id + "', '2057', '" + idHbCategories + "', this.id);\" " +
                "onselectstart=\"return false;\" onclick=\"SelectVariable('" + variable.Id + "', '"+ variable.IsTaxonomy +"', '"+ path +"');\">" + variable.Label + "</div>";*/
        }
    });
}

function LoadCategories(idVariable, idLanguage, idHoverBox, idTrigger) {
    var hoverBox = document.getElementById(idHoverBox);
    var trigger = document.getElementById(idTrigger);

    var marginTop = 21 + trigger.offsetTop - trigger.parentNode.scrollTop - GetParentScrollTop(hoverBox);
    //hoverBox.innerHTML = "<img src='/Images/Icons/Loading.gif' />";
    hoverBox.innerHTML = "";

    if (trigger.parentNode.getAttribute("MoveTop") != undefined) {
        marginTop -= parseInt(trigger.parentNode.getAttribute("MoveTop"));
    }

    hoverBox.style.marginTop = marginTop + "px";

    var method = "GetCategories";

    AjaxRequest(method, "IdVariable=" + idVariable + "&IdLanguage=" + idLanguage + "&IsTaxonomy=" + trigger.getAttribute("IsTaxonomy"), function (response) {
        var hoverBox = document.getElementById(idHoverBox);

        hoverBox.innerHTML = "";

        try {
            var categories = JSON.parse(response);
        }
        catch (e) {
            return;
        }

        for (var i = 0; i < categories.Categories.length; i++) {
            var cssClass = "VariableSelectionCategory BackgroundColor1";

            if (categories.Categories[i].HasData == "False") {
                cssClass += " VariableSelectionCategoryNoData";
            }

            hoverBox.innerHTML += "<div onselectstart=\"return false;\" id=\"DragableCategory" + categories.Categories[i].Id +
                "\" IdCategory=\"" + categories.Categories[i].Id +
                "\" ScoreType=\"" + categories.Categories[i].ScoreType + "\" class=\"" + cssClass + "\">" + categories.Categories[i].Label + "</div>";
        }

        for (var i = 0; i < categories.Categories.length; i++) {
            var category = document.getElementById("DragableCategory" + categories.Categories[i].Id);

            if (category == undefined)
                continue;

            category.setAttribute("onmousedown", "CreateDragCategory(this)");
        }

        //window.setTimeout(function () {
        ShowHoverBox(idHoverBox, idTrigger, "VariableSelectionCategories", "onmouseover");
        //}, 1);
    });
}

function RemoveFilterCategory(idCategory, idTableRow) {
    AjaxRequest("RemoveFilterCategory", "IdCategory=" + idCategory, function (response) {
        var pnlFilter = document.getElementById("cphContent_pnlFilter");

        if (pnlFilter == undefined)
            pnlFilter = document.getElementById("pnlFilter");

        pnlFilter.innerHTML = response;

        var tr = document.getElementById(idTableRow);

        if (tr == undefined)
            tr = document.getElementById("cphContent_" + idTableRow);

        DecreaseOpacity(tr, function () {
            tr.style.display = "none";
        });
    });
}

function SelectVariable2(idControl, idVariable) {
    var id = "Content";

    ShowLoading(document.body);

    var hdf = document.createElement("input");
    hdf.type = "hidden";
    hdf.name = idControl;
    hdf.value = idVariable;

    document.forms[0].appendChild(hdf);

    document.forms[0].submit();
}

function SelectVariable(idVariable, isTaxonomy, source, path, idSelected) {
    AjaxRequest("SelectVariable", "IdVariable=" + idVariable + "&IsTaxonomy=" + isTaxonomy + "&Source=" + source + "&Path=" + path + "&IdSelected=" + idSelected, function (response) {
        crosstableIsCaching = true;

        try {
            // Load the data for the new selection in the background.
            /*AjaxRequest("PopulateCrosstable", "", function (response) {
                window.setTimeout(CacheReloadCrosstable, 50);
            });*/
            PopulateCrosstable();
        }
        catch (e) {
            window.location = window.location;
        }
    });
}


function InitNestedLeftVariableSelectors() {
    var container = document.getElementById("cphContent_pnl");
    var pnlLeftHeadline = document.getElementById("pnlLeftHeadline");

    var tableCells = container.getElementsByTagName("td");

    // Run through all table cells.
    for (var i = 0; i < tableCells.length; i++) {

        if (tableCells.item(i).className.search("TableCellHeadlineLeftVariable") == -1 &&
            tableCells.item(i).className.search("TableCellHeadlineNestedLeftVariable") == -1)
            continue;

        var height = tableCells.item(i).offsetHeight;
        //height -= 35;

        if (height > pnlLeftHeadline.offsetHeight)
            height = pnlLeftHeadline.offsetHeight;

        var marginLeft = (height / -2) + tableCells.item(i).offsetWidth - 13;

        var tags = tableCells.item(i).getElementsByTagName("div");

        var marginTop = 0;

        for (var d = 0; d < tags.length; d++) {
            if (tags.item(d).className.search("TableVariableSelector") != -1 && marginTop == 0) {
                tags.item(d).style.height = tags.item(d).getElementsByTagName("div").item(0).offsetHeight + "px";
            }

            if (tags.item(d).className.search("VariableSelection") != -1 && marginTop == 0) {
                marginTop = tags.item(d).offsetHeight / -2;
                marginTop -= 10;
            }

            if (tags.item(d).className.search(" NestedVariableSelector") == -1)
                continue;

            var tags2 = tags.item(d).getElementsByTagName("div").item(0).getElementsByTagName("div");

            for (var d2 = 0; d2 < tags2.length; d2++) {
                if (tags2.item(d2).className.search("VariableLabel") == -1)
                    continue;

                var tag = tags2.item(d2);

                tag.style.width = height + "px";
                tag.style.marginLeft = marginLeft + "px";
                tag.style.marginTop = marginTop + "px";

                tag.style.borderTopWidth = "2px";
                tag.style.borderTopStyle = "solid";
                tag.className += " BorderColor7 BackgroundColor2";
            }
        }
    }
}

loadFunctions.push(InitNestedLeftVariableSelectors);
