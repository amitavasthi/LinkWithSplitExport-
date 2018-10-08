
function BindReportDisplayTypes() {
    AjaxRequest("BindReportDisplayTypes", "", function (response) {
        var pnlReporterViews = document.getElementById("cphContent_pnlReporterViews");
        pnlReporterViews.innerHTML = response;

        if (document.getElementsByClassName("RendererV2").length != 0 && document.getElementsByClassName("RendererV2")[0].getElementsByTagName("tr").length == 0)
            document.getElementById("cphContent_pnlGoButton").className = "GoButton2 GreenBackground2I"
        else
            document.getElementById("cphContent_pnlGoButton").className = "GoButton2"


    });
}


function ShowLabelTooltip(sender) {
    return;

    var message = document.getElementById("DropAreaMessage");

    if (message == undefined) {
        message = document.createElement("div");
        message.id = "DropAreaMessage";
        message.className = "DropAreaMessage Color1";

        document.body.appendChild(message);
    }

    message.innerHTML = sender.innerText
}

function HideLabelTooltip() {
    return;

    var message = document.getElementById("DropAreaMessage");

    if (message != undefined)
        message.parentNode.removeChild(message);
}


function ExportTable(variableName, categoryNames, categories, taxonomyCategories) {
    var parameters = "";
    if (categories != undefined)
        parameters = "Type=" + "ReportDefinitionCategory" + "&Categories=" + categories;
    if (taxonomyCategories != undefined)
        parameters = "Type=" + "ReportDefinitionTaxonomyCategory" + "&Categories=" + taxonomyCategories;
    if (variableName != undefined && categoryNames != undefined)
        parameters += "&VariableFilter=" + variableName + "&CategoryNames=" + categoryNames;
    ShowLoading(document.body);
    AjaxRequest("ExportTable", parameters, function (response) {
        if (parameters != "") {
            var files = response.split('###');
            for (var i = 0; i < files.length; i++) {
                if (files[i] != "") {
                    var filename = files[i].split("/");
                    if (document.getElementById("excelDownloadLink") != null)
                        document.body.removeChild(anchor);
                    var anchor = document.createElement("a");
                    anchor.id = "excelDownloadLink"
                    anchor.href = files[i];
                    anchor.setAttribute("download", filename[5]);
                    document.body.appendChild(anchor);
                    anchor.click();
                    document.body.removeChild(anchor);
                }
            }

            UpdateSetting('AutoUpdate', 'true', true, true);
        }
        else {
            var filename = response.split("/");
            if (document.getElementById("excelDownloadLink") != null)
                document.body.removeChild(anchor);
            var anchor = document.createElement("a");
            anchor.id = "excelDownloadLink"
            anchor.href = response;
            anchor.setAttribute("download", filename[5]);
            document.body.appendChild(anchor);
            anchor.click();
            document.body.removeChild(anchor);
        }
        HideLoading();
        document.getElementById("cphContent_pnlGoButton").className = "GoButton2"
    });
}

function ExportAllTabs(variableName, categoryNames, categories, taxonomyCategories) {
    var parameters = "";
    if (categories != undefined)
        parameters = "Type=" + "ReportDefinitionCategory" + "&Categories=" + categories;
    if (taxonomyCategories != undefined)
        parameters = "Type=" + "ReportDefinitionTaxonomyCategory" + "&Categories=" + taxonomyCategories;
    if (variableName != undefined && categoryNames != undefined)
        parameters += "&VariableFilter=" + variableName + "&CategoryNames=" + categoryNames;
    ShowLoading(document.body);
    AjaxRequest("ExportAllTabs", parameters, function (response) {
        if (parameters != "") {
            var files = response.split('###');

            for (var i = 0; i < files.length; i++) {
                if (files[i] != "") {
                    var filename = files[i].split("/");
                    var anchor = document.createElement("a");
                    anchor.id = "excelDownloadLink" + i;
                    document.body.appendChild(anchor);
                    anchor.href = files[i];
                    anchor.setAttribute("download", filename[5]);

                }
            }
            //export all categories
            var atags = document.querySelectorAll("[id*=excelDownloadLink]");
            for (var i = 0; i < atags.length; i++) {

                if (i != (atags.length - 1))
                    atags[i].click();
                if (i == (atags.length - 1)) {
                    setTimeout(
                    atags[i].click()
                        , 1000);

                }
            }
            for (var i = 0; i < atags.length; i++) {
                document.body.removeChild(atags[i]);
            }

            UpdateSetting('AutoUpdate', 'true', true, true);
        }
        else {
            var filename = response.split("/");
            if (document.getElementById("excelDownloadLink") != null)
                document.body.removeChild(anchor);
            var anchor = document.createElement("a");
            anchor.id = "excelDownloadLink"
            anchor.href = response;
            anchor.setAttribute("download", filename[5]);
            document.body.appendChild(anchor);
            anchor.click();
            document.body.removeChild(anchor);
        }
        HideLoading();
        document.getElementById("cphContent_pnlGoButton").className = "GoButton2"
    });
}

function SwitchTable() {
    AjaxRequest("SwitchTable", "", function (response) {
        PopulateCrosstable();
    });
}

var filterCategoryDropTime;
var filterCategoryDropType;
var filterCategoryDropXPath;
var filterCategoryDrop;

function ShowFilterBox() {
    var pnlFilterBox = document.getElementById("pnlFilterBox");

    pnlFilterBox.style.height = "0px";
    pnlFilterBox.style.width = "0px";
    pnlFilterBox.style.display = "";
    pnlFilterBox.style.overflow = "hidden";

    IncreaseHeight(pnlFilterBox, window.innerHeight - 150, false);

    IncreaseWidth(pnlFilterBox, window.innerWidth, false, function () {
        pnlFilterBox.style.overflow = "auto";
    });
}

var tableScrollLocked = false;
var askedVariableScrollWarning = false;
function TableScroll(sender) {

    var leftHeadline = document.getElementById("pnlLeftHeadline");
    var topHeadline = document.getElementById("pnlTopHeadline");

    if (leftHeadline == undefined || topHeadline == undefined)
        return;

    if (tableScrollLocked) {
        sender.scrollLeft = topHeadline.scrollLeft;
        sender.scrollTop = leftHeadline.scrollTop;

        return false;
    }

    if (sender.scrollTop > (leftHeadline.scrollHeight - leftHeadline.offsetHeight))
        sender.scrollTop = (leftHeadline.scrollHeight - leftHeadline.offsetHeight);

    if (sender.scrollLeft > (topHeadline.scrollWidth - topHeadline.offsetWidth))
        sender.scrollLeft = (topHeadline.scrollWidth - topHeadline.offsetWidth);

    leftHeadline.scrollTop = sender.scrollTop;
    topHeadline.scrollLeft = sender.scrollLeft;

    //window.setTimeout(ScrollVariableLabel, 10);
    ScrollVariableLabel();
}


var leftScrollIndex;
var leftVariableSelectors;
var leftHeadline;
var leftScrollTopStart;
var leftScroll_top;
var leftScroll_top_h;
function ScrollVariableLabel() {

    /*var style = document.createElement("style");
    style.type = "text/css";
    style.innerHTML = ".TableCellHeadline { vertical-align:middle !important; }";
    document.body.appendChild(style);
    return;*/

    if (document.getElementById("cphContent_chkLeftPanelSettingsScrollLabels").checked == false) {
        var style = document.createElement("style");
        style.type = "text/css";
        style.innerHTML = ".TableCellHeadline { vertical-align:middle !important; }";
        document.body.appendChild(style);
        //SetLeftHeadlineLabelsHeight();
        return;
    }

    var startTime = new Date();

    SetLeftHeadlineLabelsHeight();

    leftHeadline = document.getElementById("pnlLeftHeadline");
    leftVariableSelectors = GetChildsByAttribute(leftHeadline, "ScrollLabel", "true", true);
    leftScrollTopStart = GetOffsetTop(leftHeadline);
    leftScroll_top = new Object();
    leftScroll_top_h = new Object();

    //window.setTimeout(ScrollLeftVariableLabel, 1);
    scrollLeftIndex = 0;
    ScrollLeftVariableLabel();
    ScrollTopVariableLabel();

    var endTime = new Date();

    if (askedVariableScrollWarning == false && (endTime - startTime) > 200) {
        CreateConfirmBox(LoadLanguageText("VariableScrollWarning"), function () {
            document.getElementById("cphContent_chkLeftPanelSettingsScrollLabels").checked = false;
            UpdateSetting("ScrollLabels", false, false, true);
        });
        askedVariableScrollWarning = true;
    }
}

function SetLeftHeadlineLabelsHeight() {

    var leftHeadline = document.getElementById("pnlLeftHeadline");

    var variableSelectors = GetChildsByAttribute(leftHeadline, "ScrollLabel", "true", true);

    for (var i = 0; i < variableSelectors.length; i++) {
        if (variableSelectors[i].getAttribute("NestLevel").search(".5") != -1)
            continue;

        if (variableSelectors[i].parentNode.offsetHeight < window.innerHeight) {
            variableSelectors[i].removeAttribute("ScrollLabel");
        }

        variableSelectors[i].style.height = (variableSelectors[i].parentNode.offsetHeight - 5) + "px";
    }
}

function ScrollLeftVariableLabel() {
    /*if (leftScrollIndex == undefined)
        leftScrollIndex = 0;

    var i = leftScrollIndex;

    if (leftVariableSelectors.length == i)
        return;*/

    var heights = {};

    for (var i = 0; i < leftVariableSelectors.length; i++) {
        var variableSelector = leftVariableSelectors[i];

        if (variableSelector.TotalTop == undefined) {
            variableSelector.TotalTop = GetOffsetTop(variableSelector);
        }

        if (leftHeadline.scrollTop + window.innerHeight < variableSelector.TotalTop) {
            variableSelector.style.display = "none";
            return;
        }

        variableSelector.style.width = (variableSelector.parentNode.offsetWidth - 2) + "px";
        variableSelector.style.top = "";
        variableSelector.style.height = "";
        variableSelector.style.display = "";
        variableSelector.style.position = "absolute";
        variableSelector.style.overflow = "hidden";

        var nestLevel = parseFloat(variableSelector.getAttribute("NestLevel"));

        if (leftScroll_top[nestLevel] == undefined)
            leftScroll_top[nestLevel] = 0;

        if (leftScroll_top_h[nestLevel] == undefined)
            leftScroll_top_h[nestLevel] = 0;

        var top = leftScroll_top[nestLevel];

        var height = variableSelector.parentNode.offsetHeight;

        if ((leftScroll_top_h[nestLevel] + height - leftHeadline.scrollTop) > leftHeadline.offsetHeight) {
            height -= ((leftScroll_top_h[nestLevel] + height - leftHeadline.scrollTop) - leftHeadline.offsetHeight);
            //height -= height - leftHeadline.offsetHeight;
        }

        if (leftScroll_top_h[nestLevel] < leftHeadline.scrollTop)
            height -= (leftHeadline.scrollTop - leftScroll_top_h[nestLevel]);

        variableSelector.style.top = (top + leftScrollTopStart) + "px";
        variableSelector.style.height = height + "px";

        var tdVariableLabel = GetChildByAttribute(variableSelector, "class", "VariableSelectorVariableLabel", true);

        if (tdVariableLabel != undefined)
            tdVariableLabel.style.height = (height - 120) + "px";

        if (height <= 30) {
            variableSelector.style.display = "none";
        }
        else {
            leftScroll_top[nestLevel] += height;
        }

        leftScroll_top_h[nestLevel] += variableSelector.parentNode.offsetHeight;
    }

    leftScrollIndex++;

    //window.setTimeout(ScrollLeftVariableLabel, 1);
}

function ScrollTopVariableLabel() {

    /*if (browser.search("IE") != -1)
        return;*/

    var headline = document.getElementById("pnlTopHeadline");
    var headlineOffsetLeft = GetOffsetLeft(headline);

    //var labels = GetChildsByAttribute(headline.getElementsByTagName("tr").item(0), "class", "VariableLabel", true);
    var visibleLeft = new Object();

    var rows = headline.getElementsByTagName("tr");

    for (var r = 0; r < rows.length; r++) {
        visibleLeft[r] = 0;

        var labels = GetChildsByAttribute(rows.item(r), "class", "VariableLabel", true);

        for (var i = 0; i < labels.length; i++) {
            var label = labels[i];

            var tableCell = label.parentNode.parentNode.parentNode.parentNode;

            var width = tableCell.offsetWidth;

            if (width > headline.offsetWidth)
                width = headline.offsetWidth;

            var left = headline.scrollLeft;

            //left -= visibleLeft;
            //width -= visibleLeft;

            var parentLeft = GetOffsetLeft(label.parentNode) - headlineOffsetLeft;

            left -= parentLeft;

            //width -= parentLeft;

            if ((width + left) > tableCell.offsetWidth)
                width = tableCell.offsetWidth - left;

            if (visibleLeft[r] + width > headline.offsetWidth)
                width -= (visibleLeft[r] + width) - headline.offsetWidth;

            if (width > tableCell.offsetWidth)
                width = tableCell.offsetWidth;

            width -= 10;

            if (left < 0)
                left = 0;

            label.style.width = width + "px";
            label.style.display = "";

            label.style.marginLeft = left + "px";

            if (width <= 5) {
                label.style.display = "none";
            }
            else {
                visibleLeft[r] += width;
            }
        }
    }
}

function ScrollVariableLabel2() {
    var leftHeadline = document.getElementById("pnlLeftHeadline");

    var variableSelectors = GetChildsByAttribute(leftHeadline, "class", "TableVariableSelector", true);

    var top = (pnlLeftHeadline.offsetHeight / 2) + pnlLeftHeadline.parentNode.offsetTop + 75;

    for (var i = 0; i < variableSelectors.length; i++) {
        var variableSelector = variableSelectors[i];

        if ((leftHeadline.scrollTop + (leftHeadline.offsetHeight / 2)) >= variableSelector.parentNode.offsetTop &&
            variableSelector.parentNode.offsetTop + variableSelector.parentNode.offsetHeight + 10 >= leftHeadline.scrollTop) {

            var t = top;

            /*if ((leftHeadline.scrollTop + (window.innerHeight / 2)) < ((variableSelector.parentNode.offsetHeight / 2) + variableSelector.parentNode.offsetTop)) {
                //top -= variableSelector.parentNode.offsetHeight / 2;
            }*/
            if ((leftHeadline.scrollTop + leftHeadline.offsetHeight) > (variableSelector.parentNode.offsetTop + variableSelector.parentNode.offsetHeight + 10)) {

                var change = ((leftHeadline.offsetHeight / 2) - (variableSelector.parentNode.offsetHeight / 2) + leftHeadline.scrollTop);
                change -= variableSelector.parentNode.offsetTop;

                t -= change;

                //top += change;
            }

            variableSelector.style.position = "absolute";
            variableSelector.style.width = "200px";
            variableSelector.style.top = t + "px";

            if (t < (leftHeadline.parentNode.offsetTop + 83))
                variableSelector.style.visibility = "hidden";
            else
                variableSelector.style.visibility = "";
        }
        else {
            variableSelector.style.position = "";
            variableSelector.style.width = "";
            variableSelector.style.top = "";
            variableSelector.style.visibility = "";
        }
    }
}

loadFunctions.push(ScrollVariableLabel);


function BlurTable() {
    UnBlurTable();


    var style = document.createElement("style");
    style.id = "TableBlur";
    style.type = "text/css";

    style.innerHTML = ".Crosstable td { opacity:0.4; border-color:#f6f6f6 !important; } ";

    document.body.appendChild(style);
}

function BlurTable2() {
    UnBlurTable();

    var pnlFilter = document.getElementById("cphContent_pnlFilter");
    var content = document.getElementById("cphContent_pnl");

    if (pnlFilter == undefined)
        pnlFilter = document.getElementById("pnlFilter");

    /*if (pnlFilter != undefined) {
        pnlFilter.style.width = (pnlFilter.offsetWidth - 60) + "px";
        pnlFilter.style.position = "absolute";
        pnlFilter.style.boxShadow = "0px 0px 5px 0px #cccccc";
        pnlFilter.style.marginTop = "-" + GetParentScrollTop(pnlFilter) + "px"
    }*/

    var blur = document.createElement("div");
    blur.id = "BlurTable";
    blur.className = "BlurTable";

    if (pnlFilter != undefined) {
        var table = pnlFilter.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;

        //blur.style.width = table.offsetWidth + "px";
        //blur.style.height = table.offsetHeight + "px";
        blur.style.width = "100%";
        blur.style.height = "100%";

        //pnlFilter.parentNode.insertBefore(blur, pnlFilter);
        content.insertBefore(blur, content.childNodes.item(0));
    }
}

function UnBlurTable() {
    var blur = document.getElementById("TableBlur");

    if (blur != undefined)
        blur.parentNode.removeChild(blur);

    /*var pnlFilter = document.getElementById("cphContent_pnlFilter");

    if (pnlFilter == undefined)
        pnlFilter = document.getElementById("pnlFilter");

    if (pnlFilter != undefined) {
        pnlFilter.style.width = "";
        pnlFilter.style.position = "";
        pnlFilter.style.boxShadow = "";
        pnlFilter.style.marginTop = "";
    }*/

    var blur = document.getElementById("BlurTable");

    if (blur == undefined)
        return;


    blur.parentNode.removeChild(blur);
}

var settingsChanged = false;


function UpdateHideLowBase() {
    var lowbaseTd = document.getElementsByClassName("RendererV2")[0].getElementsByTagName("tr")[0].getElementsByTagName("td");
    var lowbaseColumns = [];
    for (var i = 1; i < lowbaseTd.length; i++) {
        if ((lowbaseTd[i].className).indexOf("TableCellLowBase") != -1) {
            lowbaseColumns.push(i);
        }
    }
    var lowbaseHeadTr = document.getElementsByClassName("Crosstable TopHeadline")[0].getElementsByTagName("tr");
    for (var i = 0; i < lowbaseHeadTr.length - 1; i++) {
        var baseinitial = 1;

        var lowbaseHeadTd = lowbaseHeadTr[i].getElementsByTagName("td");
        if (((lowbaseHeadTd[0].innerHTML.indexOf("Total") != -1) || ((lowbaseHeadTd[0].innerHTML.indexOf("Base")))) && (lowbaseHeadTr[i].getAttribute("istitle") == "True")) {
            if (lowbaseHeadTd[0].innerHTML.indexOf("unweighted") != -1) { baseinitial = 2; }
            for (var j = baseinitial; j < lowbaseHeadTd.length; j++) {
                if (lowbaseColumns.indexOf(j) != -1) {
                    if (lowbaseHeadTd[j].getAttribute("xPath") != null) {
                        RemoveScore(lowbaseHeadTd[j], lowbaseHeadTd[j].getAttribute("xPath"));
                    }
                }
            }
        }

    }
}

function UpdateSetting(name, value, clearData, populate) {

    if (clearData == undefined)
        clearData = false;

    var parameters = "Name=" + name + "&Value=" + value + "&ClearData=" + clearData;

    settingsChanged = true;
    AjaxRequest("UpdateSetting", parameters, function (response) {
        if (populate)
            PopulateCrosstable(undefined, true);
    });
}

function UpdateSettingAll(name, value, clearData, applyAll) {
    if (clearData == undefined)
        clearData = false;

    var parameters = "Name=" + name + "&Value=" + value + "&ClearData=" + clearData + "&ApplyAll=" + applyAll;

    settingsChanged = true;
    AjaxRequest("UpdateSetting", parameters, function (response) {

    });
}

function PropagateFilterDefinition(onFinish) {
    AjaxRequest("PropagateFilterDefinition", "", function (response) {
        if (onFinish != undefined)
            onFinish();
    });
}

function PopulateCrosstable(sender, load) {

    if (load == undefined) {
        load = false;
    }

    if (sender != undefined)
        sender.style.display = "none";
    if (load) {
        ShowLoadingGo(document.body);
    }

    //ShowLoading(document.body);

    AjaxRequest("PopulateCrosstable", "", function (response) {
        GetDataAggregationProgress(load);
    });
}

function PopulateCrosstableDelayed(timeout, onFinish) {

    AjaxRequest("PopulateCrosstable", "", function (response) {
        //window.setTimeout(CacheReloadCrosstable, 50);
        window.setTimeout(function () {
            GetDataAggregationProgress(true);

            if (onFinish != undefined)
                onFinish();

        }, timeout);
    });
}

function GetDataAggregationProgress(load) {
    AjaxRequest("GetDataAggregationProgress", "", function (response) {
        var status = response.split('|')[0];
        var progress = parseInt(response.split('|')[1]);

        if (load == undefined) {
            load = false;
        }

        //for hiding Layout except go btn click
        if (load) {
            var loading = document.getElementById("LoadingText");

            if (loading == undefined) {
                // ShowLoadingGo(document.body);
                loading = document.getElementById("LoadingText");
            }
            if (loading != undefined) {

                if (progress != 100)
                    loading.innerHTML = progress + " %<br /><br />" + status;
                else
                    loading.innerHTML = status;

            }
        }
        if (progress < 100)
            window.setTimeout(GetDataAggregationProgress(true), 100);
        else {
            BuildCrosstable();

            if (window.location.toString().search("LinkReporter/Crosstabs.aspx") != -1)
                BindReportDisplayTypes();
        }
    });
}

function BuildCrosstable() {
    AjaxRequest("BuildCrosstable", "", function (response) {
        try {
            var container = document.getElementById("cphContent_pnl");

            container.innerHTML = response;

            EvaluateScripts(container);

            InitBoxes();
            ScrollVariableLabel();
            HideLoading();

            if (document.getElementsByClassName("RendererV2").length != 0 && document.getElementsByClassName("RendererV2")[0].getElementsByTagName("tr").length == 0)
                document.getElementById("cphContent_pnlGoButton").className = "GoButton2 GreenBackground2I"
            else
                document.getElementById("cphContent_pnlGoButton").className = "GoButton2"

        }
        catch (e) {
            window.location = window.location;
        }
    });
}


function ClearReportDefinition(refresh) {
    if (refresh == undefined)
        refresh = true;

    AjaxRequest("ClearReportDefinition", "", function (response) {
        if (refresh == true)
            window.location = window.location.toString().split('?')[0];
    });
}


function RemoveScore(sender, xPath) {
    AjaxRequest("RemoveScore", "XPath=" + xPath, function (response) {
        ReloadCrosstable();
    });
}

function RemoveVariable(path) {
    AjaxRequest("RemoveVariable", "XPath=" + path, function (response) {
        ReloadCrosstable();
    });
}

function CombineScores(idCategory, type, xPath) {
    if (filterCategoryDropTime == undefined)
        return;

    if ((new Date()) - filterCategoryDropTime > 200) {
        filterCategoryDropTime = undefined;

        return;
    }

    filterCategoryDropTime = undefined;

    var boxContent = "<table><tr><td>" + LoadLanguageText("Name") + "</td><td><input id=\"txtCombineScoresName\" type=\"text\" onkeydown=\"if(event.keyCode != 13) return;this.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode['SubmitButton'].onclick();\" /></td></tr></table>";

    if (filterCategoryDrop == idCategory)
        return;

    if (type == "ReportDefinitionScoreGroup") {
        var parameters = "Name=" + "&XPath=" + xPath +
                "&IdCategory=" + idCategory + "&IdCategory2=" + filterCategoryDrop;

        AjaxRequest("CombineScores", parameters, function (response) {
            ReloadCrosstable();
        });
    }
    else {
        var Varxpath1Index = filterCategoryDropXPath.indexOf('Variable[@Id="');
        var Varxpath1Id = filterCategoryDropXPath.substr(Varxpath1Index + 15, 35);

        var Varxpath2Index = xPath.indexOf('Variable[@Id="');
        var Varxpath2Id = xPath.substr(Varxpath2Index + 15, 35);
        if (Varxpath1Id == Varxpath2Id) {

            ShowJavascriptBox("boxCombineScores", boxContent, function () {
                var txtCombineScoresName = document.getElementById("txtCombineScoresName");
                var parameters = "Name=" + encodeURIComponent(txtCombineScoresName.value) + "&XPath=" + xPath +
                    "&IdCategory=" + idCategory + "&IdCategory2=" + filterCategoryDrop;
                AjaxRequest("CombineScores", parameters, function (response) {
                    ReloadCrosstable();
                    document.getElementById("cphContent_pnlGoButton").className = "GoButton2 GreenBackground2I"
                });

            });
        }
    }
}


function SaveExisting() {
    AjaxRequest("SaveExisting", "", function (response) {
        ShowMessage(LoadLanguageText("SaveReportSuccess").replace("{0}", "/SavedReports/" + name), "Success");
    });
}

function SaveTab(name, allowOverwrite) {
    AjaxRequest("SaveTab", "AllowOverwrite=" + allowOverwrite + "&Name=" + name, function (response) {
        if (response != "") {
            ShowMessage(LoadLanguageText("SaveReportSuccess").replace("{0}", "/SavedReports/" + name), "Success");
            window.location = "Crosstabs.aspx?SavedReport=" + response;
        }
        else {
            ShowMessage(name + " already exists", "Error");
        }
    });
}

function SaveAllTabs(name, allowOverwrite) {
    AjaxRequest("SaveAllTabs", "AllowOverwrite=" + allowOverwrite + "&Name=" + name, function (response) {
        if (response != "") {
            ShowMessage(LoadLanguageText("SaveReportSuccess").replace("{0}", "/SavedReports/" + name), "Success");
            window.location = "Crosstabs.aspx?SavedReport=" + response;
        }
        else {
            ShowMessage(name + " already exists", "Error");
        }
    });
}


function CreateDragCategory(category, e) {
    if (category.getElementsByTagName("div").item(0).getAttribute("contenteditable") != undefined)
        return true;

    var rightclick;
    if (!e) var e = window.event;
    if (e.which) rightclick = (e.which == 3);
    else if (e.button) rightclick = (e.button == 2);

    if (rightclick)
        return;

    if (category.onclick != undefined)
        return;

    window.setTimeout(BlurTable, 50);

    var pnlFilter = document.getElementById("cphContent_pnlFilter");

    if (pnlFilter == undefined)
        pnlFilter = document.getElementById("pnlFilter");

    pnlFilter.style.opacity = "1.0";

    var tableCells;

    if (category.getAttribute("Position") == "Left") {
        tableCells = category.parentNode.parentNode.getElementsByTagName("td");
    }
    else {
        tableCells = category.parentNode.getElementsByTagName("td");
    }

    // Run through all table cells of the parent table row.
    for (var i = 0; i < tableCells.length; i++) {
        var tableCell = tableCells.item(i);

        if ((tableCell.className.search("TableCellHeadlineCategory") != -1 || tableCell.className.search("TableCellHeadlineLeftCategory") != -1) &&
                tableCell.getAttribute("Position") == category.getAttribute("Position") &&
                tableCell.getAttribute("NestLevel") == category.getAttribute("NestLevel") &&
            tableCell.getAttribute("IdCategory") != category.getAttribute("IdCategory")) {
            tableCell.style.opacity = "1.0";

            var test = tableCell.getElementsByTagName("td");

            for (var a = 0; a < test.length; a++) {
                test.item(a).style.opacity = "1.0";
            }
        }
    }

    var duplicate = document.createElement("div");
    duplicate.className = "DragableCategory " + category.className;
    duplicate.id = "_" + category.id;
    duplicate.innerHTML = category.innerHTML;
    duplicate.setAttribute("IdCategory", category.getAttribute("IdCategory"));
    duplicate.setAttribute("ScoreType", category.getAttribute("ScoreType"));
    duplicate.setAttribute("XPath", category.getAttribute("XPath"));
    duplicate.style.width = category.offsetWidth + "px";
    duplicate.style.textAlign = "center";

    document.body.appendChild(duplicate);

    MakeDragable(duplicate, duplicate, true, function () {
        filterCategoryDropTime = new Date();
        filterCategoryDrop = duplicate.getAttribute("IdCategory");
        filterCategoryDropType = duplicate.getAttribute("ScoreType");
        filterCategoryDropXPath = duplicate.getAttribute("XPath");

        UnBlurTable();

        var content = document.getElementById("cphContent_pnl");

        if (content == undefined)
            content = document.getElementById("pnl");

        var tableCells = content.getElementsByTagName("td");

        // Run through all table cells of the parent table row.
        for (var i = 0; i < tableCells.length; i++) {
            var tableCell = tableCells.item(i);

            if ((tableCell.className.search("TableCellHeadlineCategory") != -1 || tableCell.className.search("TableCellHeadlineLeftCategory") != -1)) {
                tableCell.style.opacity = "";

                var test = tableCell.getElementsByTagName("td");

                for (var a = 0; a < test.length; a++) {
                    test.item(a).style.opacity = "";
                }
            }
        }
    }, undefined, false, function () {
        var pnlFilter = document.getElementById("cphContent_pnlFilter");

        if (pnlFilter == undefined)
            pnlFilter = document.getElementById("pnlFilter");

        var offsetLeft = GetOffsetLeft(pnlFilter);
        var offsetTop = GetOffsetTop(pnlFilter);

        if (tempX > offsetLeft && tempX < (offsetLeft + pnlFilter.offsetWidth)) {
            if (tempY > offsetTop && tempY < (offsetTop + pnlFilter.offsetHeight)) {
                pnlFilter.style.backgroundColor = "#61CF71";
                return;
            }
        }

        pnlFilter.style.backgroundColor = "";
    });

    duplicate.onmousedown();
}

function OverFilter() {
    if (filterCategoryDropTime == undefined)
        return;

    if ((new Date()) - filterCategoryDropTime > 500) {
        filterCategoryDropTime = undefined;

        return;
    }

    filterCategoryDropTime = undefined;

    var method = "AddFilterCategory";
    var parameters = "IdCategory=" + filterCategoryDrop + "&Type=" + filterCategoryDropType;

    if (filterCategoryDropType == "ReportDefinitionScoreGroup") {
        method = "AddFilterScoreGroup";
        parameters = "XPath=" + filterCategoryDropXPath;
    }

    AjaxRequest(method, parameters, function (response) {
        //ReloadCrosstable();
        PopulateCrosstableDelayed(1, function () {
            window.location = window.location;
        });
    });
}


function ShowVariableOptions2(source, path) {
    var menu = document.getElementById("menuReportVariable");

    if (menu != undefined)
        menu.onmouseout();

    menu = InitMenu("menuReportVariable");

    var lnkDelete = document.createElement("div");

    lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
    lnkDelete.innerHTML = LoadLanguageText("Delete");
    lnkDelete.MenuItemClick = "RemoveVariable('" + path + "');";

    menu.Items.push(lnkDelete);

    menu.Render();
}

function ShowScoreOptions2(equation, source, path) {
    var menu = InitMenu("menuReportScore");

    var lnkDelete = document.createElement("div");

    lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
    lnkDelete.innerHTML = LoadLanguageText("Delete");
    lnkDelete.MenuItemClick = "RemoveScore('', '" + path + "');";

    menu.Items.push(lnkDelete);

    var lnkRename = document.createElement("div");

    lnkRename.ImageUrl = "/Images/Icons/Menu/Rename.png";
    lnkRename.innerHTML = LoadLanguageText("Rename");
    lnkRename.MenuItemClick = "RenameScore('" + source + "','" + path + "');";

    menu.Items.push(lnkRename);

    var lnkCombine = document.createElement("div");

    lnkCombine.ImageUrl = "/Images/Icons/Menu/combination.png";
    lnkCombine.innerHTML = LoadLanguageText("Combine");
    lnkCombine.MenuItemClick = "CombineScore('" + source + "','" + path + "');";

    menu.Items.push(lnkCombine);

    if (equation) {
        var lnkEquation = document.createElement("div");

        lnkEquation.ImageUrl = "/Images/Icons/Menu/Equation.png";
        lnkEquation.innerHTML = LoadLanguageText("DefineEquation");

        lnkEquation.MenuItemClick = "EditEquation('" + source + "', '" + path + "', true);";

        menu.Items.push(lnkEquation);
    }

    menu.Render();
}

function CombineScore(source, path) {
    //console.log(source + " == " + path)
    AjaxRequest("GetCombiningCategories", "XPath=" + encodeURIComponent(path) + "&Source=" + encodeURIComponent(source), function (response) {
        response = JSON.parse(response);
        var tableHtml = "";
        tableHtml = "<div style=\"max-height:450px;overflow-y:auto;margin-bottom:50px;min-width:350px;\"><table>";
        var isGroup = false;
        for (var i = 0; i < response.Categories.length; i++) {
            if (response.Categories[i].IsChecked == "True")
                isGroup = true;
            tableHtml += "<tr><td><input id='chkCategory_" + response.Categories[i].Id + "' type=\"checkbox\" " + (response.Categories[i].IsChecked == "True" ? "checked='checked'" : "'checked='false''") + " /></td>";
            tableHtml += "<td>" + response.Categories[i].Label + "</td></tr>";
        }
        tableHtml += "</table></div>";
        tableHtml += "<div style=\"position:absolute;bottom:0px;width:80%;\" >" +
        "<div style=" + (isGroup ? "display:none;" : "") + "><span style=\"display:inline-block;padding-left:10px\">New Category</span><div style=\"display:inline-block;margin-left: 20px;\"><input id=\"txtGroupName\" type=\"text\" /></div></div><div style=\"text-align:right;\"> <input id=\"btnCombine\" type=\"button\" value=\"" + (isGroup ? "save" : "combine") + "\" onclick=\"CombineConfirm('" + encodeURIComponent(source) + "','" + encodeURIComponent(path) + "','" + isGroup + "');\" /></div></div>";
        var boxcontent = document.getElementById("boxCombineCategoiresControl").getElementsByClassName("BoxContent")[0];
        boxcontent.innerHTML = tableHtml;
        InitDragBox('boxCombineCategoiresControl', 'Center');
    });
}

function CombineConfirm(source, path, group) {

    var condition = false;
    if (group == "true") {
        condition = true;
    }
    else {
        (document.getElementById('txtGroupName').value != "" ? condition = true : condition = false);
    }

    if (condition == true) {
        var chkCategories = document.querySelectorAll("[id*='chkCategory_']");
        var categories = [];
        for (var i = 0; i < chkCategories.length; i++) {
            if (chkCategories[i].checked)
                categories.push(chkCategories[i].id.replace("chkCategory_", ""));
        }
        if (categories.length > 0) {
            AjaxRequest("CombineMultipleCategories", "Name=" + encodeURIComponent(document.getElementById('txtGroupName').value) + "&XPath=" + (path) + "&Source=" + (source) + "&Categories=" + encodeURIComponent(categories), function (response) {
                ReloadCrosstable();
                CloseBox('boxCombineCategoiresControl');
            });
        }
        else
            ShowMessage("please select atleast one category", "Warning");
    }
    else
        document.getElementById('txtGroupName').style.border = "1px solid red";
}

function RenameScore(source, path) {
    var element = GetChildByAttribute(document.getElementById("CrosstableContainer"), "xPath", path, true);

    if (element == undefined)
        return;

    element = element.getElementsByTagName("div").item(0);

    element.setAttribute("contenteditable", "true");
    element.contenteditable = true;
    element.focus();
    element.style.background = "#FFFFFF";

    //return;
    element.onblur = function () {
        document.body.focus();

        element.removeAttribute("contenteditable");
        element.style.background = "";

        AjaxRequest("RenameScore", "XPath=" + encodeURIComponent(path) + "&Value=" + encodeURIComponent(element.innerText), function (response) {
        });
    };
}