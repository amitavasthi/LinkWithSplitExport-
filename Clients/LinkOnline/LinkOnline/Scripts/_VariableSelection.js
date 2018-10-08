function ShowVariableSelection(sender, direction) {
    var container = document.getElementById("CrosstableContainer");
    var options = GetChildByAttribute(sender, "class", "VariableSelectionSelector BackgroundColor7");

    if (options == undefined)
        return;

    if (options.style.display == "block")
        return;

    LoadCurrentCrosstableIntoCache();

    options.style.display = "block";

    var height = container.offsetHeight - sender.parentNode.offsetHeight - 15;
    var width = (document.getElementById("Content").offsetWidth - 202);

    options.style.marginTop = sender.parentNode.offsetHeight + "px";
    //options.style.marginTop = "76px";

    //options.style.width = sender.offsetWidth + "px";
    options.style.left = "202px";
    options.style.height = "1px";
    options.style.width = "1px";

    IncreaseHeight(options, height);
    IncreaseWidth(options, width, false);

    var ddlChapter = GetChildByAttribute(sender, "class", "VariableSelectionSelectorChapter BorderColor1", true);
    var txtSearch = GetChildByAttribute(sender, "class", "VariableSelectionSelectorSearch BorderColor1", true);

    SearchVariables(
        ddlChapter,
        txtSearch,
        GetChildByAttribute(sender, "class", "VariableSelectionSelectorVariables BorderColor1", true)
    );

    txtSearch.focus();
}

function HideVariableSelection(sender, direction) {
    var container = document.getElementById("CrosstableContainer");
    var options = GetChildByAttribute(sender, "class", "VariableSelectionSelector BackgroundColor7");

    if (options == undefined)
        return;

    if (direction == "Side") {
        var width = container.offsetWidth - sender.offsetWidth - 9;

        options.style.marginLeft = sender.offsetWidth + "px";

        options.style.marginTop = "-" + (sender.parentNode.parentNode.offsetTop + 16) + "px";
        options.style.height = (sender.parentNode.parentNode.offsetHeight + 55) + "px";

        DecreaseWidth(options, 1, false, function () {

            LoadCrosstableFromCache();
        });
    }
    else {
        var height = container.offsetHeight - sender.offsetHeight - 9;
        var width = container.offsetWidth - sender.offsetWidth - 9;
        
        DecreaseWidth(options, 1, false);
        DecreaseHeight(options, 1, function () {
            options.style.display = "none";
            LoadCrosstableFromCache();
        });
    }
}

var filterCategoryDropTime;

function SearchVariables(ddlChapter, sender, pnlVariables) {
    AjaxRequest("SearchVariables", "EnableDataCheck=False&IdChapter=" + ddlChapter.value + "&Expression=" + sender.value, function (response) {
        var content = document.getElementById("Content");

        var variables;

        try {
            // Parse the response json script.
            variables = JSON.parse(response);
        }
        catch (e) {
            return;
        }

        var searchValue = sender.value;

        if (searchValue == undefined)
            searchValue = "";

        searchValue = searchValue.trim();

        if (variables.SearchExpression != searchValue)
            return;

        pnlVariables.innerHTML = "";

        // Run through all variables.
        for (var i = 0; i < variables.Variables.length; i++) {
            var variable = variables.Variables[i];

            var cssClass = "VariableSelectionSelectorVariable Color1 BackgroundColor4";

            if (variable.HasData == "False") {
                cssClass += " VariableSelectionItemNoData";
            }

            var style = "";
            var onClick = "";
            var status = "";

            if (variable.IsSelected == "True") {
                style += "background:rgb(97,207,113);color:#FFFFFF;";
                onClick += "DeSelectVariable(this, '" + variable.Id + "');";

                if (variable.Location == "Top") {
                    status = "<img src=\"/Images/Icons/VariableSelector/VariableTop.png\" onclick=\"SwitchVariableLocation(this, '" + variable.Id + "', 'Top');\" />";
                }
                else {
                    status = "<img src=\"/Images/Icons/VariableSelector/VariableTop.png\" style=\"transform:rotate(-90deg);\" onclick=\"SwitchVariableLocation(this, '" + variable.Id + "', 'Left');\" />";
                }
            }
            else {
                onClick += "SelectVariable(this, '" + variable.Id + "', '" + variable.IsTaxonomy + "');";
            }

            pnlVariables.innerHTML += "<div id='VariableSelectionItem" + variable.Id + "' IdVariable=\"" + variable.Id + "\" class=\"" + cssClass + "\" " +
                "IsTaxonomy=\"" + variable.IsTaxonomy + "\" style=\"" + style + "\" " +
                "onselectstart=\"return false;\"><table style=\"width:100%;\" cellspacing=\"0\" cellpadding=\"0\"><tr><td style=\"width:30px;\">" + status + "</td><td onclick=\"" + onClick + "\">" + variable.Label + "</td>" +
                "<td align=\"right\"><img onclick=\"LoadCategories(this, '" + variable.Id + "', '" + variable.IsTaxonomy + "');\" class=\"VariableSelectionSelectorVariableExpandCategories\" src=\"/Images/Icons/Swiper.png\" /></td></tr></table></div>";
        }
    });
}

function SwitchVariableLocation(sender, idVariable, location) {
    var newLocation = "";

    if (location == "Top") {
        newLocation = "Left";
        Rotate(sender, -90);
    }
    else {
        newLocation = "Top";
        Rotate(sender, 0);
    }

    sender.setAttribute("onclick", "SwitchVariableLocation(this, '" + idVariable + "', '" + newLocation + "');");
    
    AjaxRequest("SwitchVariableLocation", "IdVariable=" + idVariable + "&Location=" + location + "&NewLocation=" + newLocation, function (response) {
        // Load the data for the new selection in the background.
        AjaxRequest("PopulateCrosstable", "", function (response) {
            window.setTimeout(CacheReloadCrosstable, 50);
        });
    });
}

function LoadCategories(sender, idVariable, isTaxonomy) {
    AjaxRequest("GetCategories", "IdVariable=" + idVariable + "&IsTaxonomy=" + isTaxonomy + "&IdLanguage=" + 2057, function (response) {
        var categories;

        try {
            // Parse the response json script.
            categories = JSON.parse(response);
        }
        catch (e) {
            return;
        }

        var pnlCategories = document.createElement("div");
        pnlCategories.id = "VariableSelectionSelectorCategories" + idVariable;
        pnlCategories.className = "VariableSelectionSelectorCategories";
        pnlCategories.style.height = "1px";

        // Run through all categories.
        for (var i = 0; i < categories.Categories.length; i++) {
            var category = categories.Categories[i];

            pnlCategories.innerHTML += "<div class=\"VariableSelectionSelectorCategory BackgroundColor2\">" + category.Label + "</div>";
        }

        var pnlVariable = document.getElementById("VariableSelectionItem" + idVariable);

        if (pnlVariable == undefined)
            return;

        var index = ChildNodeIndex(pnlVariable.parentNode.childNodes, pnlVariable);

        if (index == -1)
            return;

        index++;

        if (index < pnlVariable.parentNode.childNodes.length)
            pnlVariable.parentNode.insertBefore(pnlCategories, pnlVariable.parentNode.childNodes.item(index));
        else
            pnlVariable.parentNode.appendChild(pnlCategories);

        sender.setAttribute("onclick", "HideCategories(this, '" + idVariable + "', '" + isTaxonomy + "');");

        IncreaseHeight(pnlCategories, 30 * categories.Categories.length);
    });
}

function HideCategories(sender, idVariable, isTaxonomy) {
    var pnlCategories = document.getElementById("VariableSelectionSelectorCategories" + idVariable);

    DecreaseHeight(pnlCategories, 0, function () {
        pnlCategories.parentNode.removeChild(pnlCategories);

        sender.setAttribute("onclick", "LoadCategories(this, '" + idVariable + "', '" + isTaxonomy + "');");
    });
}

function SelectVariable(sender, idVariable, isTaxonomy) {
    var control = document.getElementById("VariableSelectionItem" + idVariable);

    if (control == undefined)
        return;

    var to = [97, 207, 113];

    if (control.className.search("VariableSelectionItemNoData") != -1) {
        control.style.background = "rgb(255,0,0)";
    }
    else {
        control.style.background = "rgb(108,174,224)";
    }

    ConvertColor(to, control, "Increase");

    control.style.color = "#FFFFFF";

    sender.setAttribute("onclick", "DeSelectVariable(this, '" + idVariable + "');");

    AjaxRequest("SelectVariable", "IdVariable=" + idVariable + "&IsTaxonomy=" + isTaxonomy, function (response) {
        crosstableIsCaching = true;

        try {
            // Load the data for the new selection in the background.
            AjaxRequest("PopulateCrosstable", "", function (response) {
                window.setTimeout(CacheReloadCrosstable, 50);
            });
        }
        catch (e) {
            window.location = window.location;
        }
    });
}

function DeSelectVariable(sender, idVariable) {
    var control = document.getElementById("VariableSelectionItem" + idVariable);

    if (control == undefined)
        return;

    var to = [108, 174, 224];

    if (control.className.search("VariableSelectionItemNoData") != -1) {
        to = [255, 0, 0];
    }

    control.style.background = "rgb(97,207,113)";

    ConvertColor(to, control, "Decrease", function () {
        control.style.background = "";
        control.style.color = "";
    });

    sender.setAttribute("onclick", "SelectVariable(this, '" + idVariable + "');");

    AjaxRequest("DeSelectVariable", "IdVariable=" + idVariable, function (response) {
        window.setTimeout(CacheReloadCrosstable, 50);
    });
}

var crosstableCache;
var crosstableIsCaching = false;
var showingLoadingScreen = false;

function CacheReloadCrosstable() {
    crosstableIsCaching = true;

    AjaxRequest("BuildCrosstable", "", function (response) {
        crosstableCache = response;
        crosstableIsCaching = false;
    });
}

function LoadCurrentCrosstableIntoCache() {
    crosstableIsCaching = true;

    var id = "cphContent_pnl";

    var container = document.getElementById(id);

    crosstableCache = container.innerHTML;

    crosstableIsCaching = false;
}

function LoadCrosstableFromCache() {
    if (crosstableIsCaching) {
        if (showingLoadingScreen == false) {
            showingLoadingScreen = true;
            ShowLoading(document.getElementById("Content"));
        }

        window.setTimeout(LoadCrosstableFromCache, 100);

        return;
    }

    var id = "cphContent_pnl";

    var container = document.getElementById(id);

    HideLoading();
    showingLoadingScreen = false;

    var id = "cphContent_pnl";

    var container = document.getElementById(id);

    container.innerHTML = crosstableCache;

    EvaluateScripts(container);

    InitBoxes();
    HideLoading();
    InitNestedLeftVariableSelectors();
}