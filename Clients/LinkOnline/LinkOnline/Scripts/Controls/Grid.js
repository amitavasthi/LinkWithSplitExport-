var gridButtons = new Array();
var gridIds = new Array();

function SelectRow(gridId, id, autoPostBack, sender, idGrid, dependingGrid) {
    if (true) {
        for (var i = 0; i < sender.parentNode.childNodes.length; i++) {
            if (sender.parentNode.childNodes.item(i).className == "GridRow_Active")
                sender.parentNode.childNodes.item(i).className = "GridRow";
        }

        sender.className = "GridRow_Active";
    }

    var hiddenField = document.getElementById(gridId);

    if (hiddenField == undefined) {
        var hiddenField = document.createElement("input");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("id", gridId);
        hiddenField.setAttribute("name", gridId);

        document.forms[0].appendChild(hiddenField);
    }

    hiddenField.value = id;

    if (autoPostBack) {
        //DisablePage();

        if (dependingGrid != undefined) {
            AjaxUpdate(gridId, id, true, idGrid, dependingGrid);
        }
        else {
            document.forms[0].submit();
        }
    }
    else {
        CheckGridButtons();
    }
}

function AjustOverflows() {
    var sections = document.getElementsByTagName("gridsection");

    var changed = false;

    for (var i = 0; i < sections.length; i++) {
        var rows = GetChildsByAttribute(sections.item(i), "class", "GridRow");
        rows.push(GetChildByAttribute(sections.item(i), "class", "GridRow_Active"));
        rows.push(GetChildByAttribute(sections.item(i), "class", "GridHeadline BackgroundColor1"));

        for (var r = 0; r < rows.length; r++) {
            var row = rows[r];

            if (row == undefined)
                continue;

            var cells = GetChildsByAttribute(row, "class", "GridRowItem");

            if(cells.length == 0)
                cells = GetChildsByAttribute(row, "class", "GridHeadlineItem");

            if (cells.length == 1) {
                cells[0].style.maxWidth = (sections.item(i).offsetWidth - 10) + "px";
                cells[0].style.width = cells[0].style.maxWidth;

                continue;
            }

            for (var c = 0; c < cells.length; c++) {
                var cell = cells[c];

                if (cell == undefined)
                    continue;

                var overflow = GetChildByAttribute(cell, "class", "GridRowItemOverflow");

                if (overflow == undefined) {
                    overflow = GetChildByAttribute(cell, "class", "GridHeadlineItemOverflow");

                    if (overflow == undefined)
                        continue;
                }

                var newMaxWidth = cell.offsetWidth - 10;

                if (overflow.className == "GridHeadlineItemOverflow")
                    newMaxWidth -= 20;

                if (overflow.style.maxWidth != (newMaxWidth + "px"))
                    changed = true;

                overflow.style.maxWidth = newMaxWidth + "px";
            }
        }
    }

    if(changed) {
        window.setTimeout(AjustOverflows, 10);
    }
}

function AjaxUpdate(idGrid, idValue, restart, gridId, dependingGrid)
{
    var url = window.location.protocol + "//" + window.location.host + "/Handlers/GridHandler.ashx";
    var params = "Method=Update&" + idGrid + "=" + idValue + "&IdGrid=" + gridId + "&IdGrid2=" + (gridId.replace(idGrid, dependingGrid));

    var request = AjaxRequest(url, params);

    var grid = document.getElementById("cphContent_cphProjectsContent_gridTestRunSteps_Grid");

    var gridOverflow = GetChildByAttribute(grid, "class", "GridOverflow");

    if (gridOverflow == undefined)
        document.forms[0].submit();

    var gridSection = gridOverflow.getElementsByTagName("gridsection").item(0);
    var gridOverflow = gridSection.parentNode;

    gridOverflow.removeChild(gridSection);

    request.OnResponse = function (text) {
        if (request.Result == undefined ||
            request.Result == "undefined")
        {
            window.setTimeout(function () {
                AjaxUpdate(idGrid, idValue, true, gridId, dependingGrid);
            }, 1000);

            return;
        }

        gridOverflow.innerHTML = "<gridsection>"+ request.Result +"</gridsection>";

        //if(enable)
        //    window.setTimeout(EnablePage, 500)
        if (restart) {
            AjaxUpdate(idGrid, idValue, false, gridId, dependingGrid);
        }
        else {
            window.setTimeout(function () {
                UpdateClassNames(gridOverflow.getElementsByTagName("gridsection").item(0));
                CheckGridHeights();
            }, 10);
        }
    }

    request.Send();
}

function GridScroll(idHiddenField, sender, idGrid) {
    var hiddenField = document.getElementById(idHiddenField);

    hiddenField.value = sender.scrollTop;

    if (sender.scrollTop == GetScrollTopMax(sender)) {
        for (var i = 0; i < 5; i++) {
            GetNextRow(idGrid);
        }
    }
}

function GetScrollTopMax(element) {
    if (element.scrollTopMax != undefined)
        return element.scrollTopMax;

    if (element.scrollHeight != undefined)
        return element.scrollHeight - element.offsetHeight;

    return element.childNodes.item(0).offsetHeight - element.offsetHeight;
}

function GetNextRow(idGrid) {
    return;

    var grid = document.getElementById(idGrid);

    if (grid["RowIndex"] == null)
        grid["RowIndex"] = GetRowCount(idGrid) - 1;

    grid["RowIndex"]++;

    if (parseInt(grid.getAttribute("ItemCount")) <= grid["RowIndex"])
        return;

    var url = window.location.protocol + "//" + window.location.host + "/Handlers/GridHandler.ashx";
    var params = "Method=GetRow&IdGrid="+ idGrid +"&Index=" + grid["RowIndex"];

    var request = AjaxRequest(url, params);

    var placeholder = document.createElement("div");
    placeholder.id = "Placeholder" + idGrid + "_" + grid["RowIndex"];
    placeholder.style.textAlign = "center";
    placeholder.innerHTML = "<img src='/Images/Wait.gif' height='20' />";

    var grid = document.getElementById(idGrid);

    var gridOverflow = GetChildByAttribute(grid, "class", "GridOverflow");

    var gridSection = gridOverflow.getElementsByTagName("gridsection").item(0);

    gridSection.appendChild(placeholder);

    gridOverflow.scrollTop = gridOverflow.scrollTop + ((GetScrollTopMax(gridOverflow) - gridOverflow.scrollTop) / 2);

    request.OnResponse = GetNextRowResponse;

    request.Send([idGrid, grid["RowIndex"]]);
}

function GetNextRowResponse(text, parameters) {
    var grid = document.getElementById(parameters[0]);

    var gridOverflow = GetChildByAttribute(grid, "class", "GridOverflow");

    var gridSection = gridOverflow.getElementsByTagName("gridsection").item(0);

    var placeholder = document.getElementById("Placeholder" + parameters[0] + "_" + parameters[1]);

    if (placeholder == undefined) {
        var tdjsflkjds = "";
    }

    placeholder.outerHTML = text;

    placeholder = gridSection.childNodes.item(gridSection.childNodes.length - 1);

    window.setTimeout(UpdateClassNames, 100);
}

function UpdateClassNames(gridSection) {
    var gridSections = document.getElementsByTagName("gridsection");

    for (var a = 0; a < gridSections.length; a+=2) {
        var gridSection = gridSections.item(a + 1);

        var items = GetChildsByAttribute(gridSection, "class", "GridRow2");

        for (var i = 0; i < items.length; i++) {
            items[i].className = "GridRow";
        }
    }
}

function GetRowCount(idGrid) {
    var grid = document.getElementById(idGrid);

    var gridOverflow = GetChildByAttribute(grid, "class", "GridOverflow");

    var gridSection = gridOverflow.getElementsByTagName("gridsection").item(0);

    var childs = GetChildsByAttribute(gridSection, "GridRow");
    var childs2 = GetChildsByAttribute(gridSection, "GridRow_Active");

    return childs.length + childs2.length;
}

function GridDoubleClick(idGrid) {
    var hdf = document.createElement("input");
    hdf.type = "hidden";
    hdf.name = "GridDoubleClick" + idGrid;
    hdf.Value = "true";

    document.forms[0].appendChild(hdf);

    document.forms[0].submit();
}

var gridPixelWidths = new Array();
var gridPercentageWidths = new Array();

loadFunctions.push(function () {
    window.setTimeout(SetWidths, 100);
    window.setTimeout(CheckGridHeights, 200);
});

function SetWidths() {
    var gridsections = document.getElementsByTagName("gridsection");

    for (var i = 0; i < gridsections.length; i += 2) {
        var headline = GetChildByAttribute(gridsections.item(i), "class", "GridHeadline BackgroundColor1");
        var content = gridsections.item(i + 1);

        //if (browser == "Chrome")
            content = content.parentNode;

        var width = content.offsetWidth;

        if (width == 0)
            continue;

        //if (browser != "InternetExplorer" && isOverflowed(content)) {
        if (isOverflowed(content)) {
            //width -= 17;
            
            var scrollBarThickness = 8;

            if (false)
            {
                scrollBarThickness = 16;
            }

            width -= scrollBarThickness;

            //width -= 10;
            headline.style.width = width + "px";
        }

        //content.parentNode.style.width = width + "px";
    }
}

resizeFunctions.push(function () {
    AjustOverflows();

    SetWidths();

    AjustOverflows();
});

submitFunctions.push(function () {
    var grids = new Array();

    for (var i = 0; i < gridPercentageWidths.length; i++) {
        if (grids.indexOf(gridPercentageWidths[i].IdGrid) == -1) {
            grids.push(gridPercentageWidths[i].IdGrid);
        }
    }

    for (var i = 0; i < grids.length; i++) {
        var grid = document.getElementById(grids[i]);

        var gridRows = grid.getElementsByTagName("gridsection").item(1).childNodes;
        var headCells = grid.getElementsByTagName("gridsection").item(0).childNodes.item(0).getElementsByTagName("div");

        var _headCells = new Array();

        for (var a = 0; a < headCells.length; a++) {
            if (headCells.item(a).className == "GridHeadlineItem")
                _headCells.push(headCells.item(a));
        }

        for (var index = 0; index < _headCells.length; index++) {
            var headCell = _headCells[index];

            var hiddenField = document.createElement("input");
            hiddenField.type = "hidden";
            hiddenField.name = grid.id + headCell.getAttribute("ColumnName");
            hiddenField.value = roundUp(parseInt(headCell.style.width) * 100 / grid.offsetWidth).toString();

            document.forms[0].appendChild(hiddenField);
        }

    }
});

function CheckGridHeights() {
    for (var o = 0; o < gridIds.length; o++) {

        var grid = document.getElementById(gridIds[o]);

        var gridHeadline = grid.getElementsByTagName("gridsection").item(0).getElementsByTagName("div").item(0);
        var gridRows = grid.getElementsByTagName("gridsection").item(1).childNodes;

        CheckGridHeight2(gridHeadline);

        for (var i = 0; i < gridRows.length; i++) {
            var gridRow = gridRows.item(i);

            CheckGridHeight(gridRow);
        }
    }

    RunSetHeightSmoothJobs();
}

function CheckGridHeight(row) {
    var cells = GetChildsByAttribute(row, "class", "GridRowItem");

    var maxHeight = 0;

    for (var i = 0; i < cells.length; i++) {
        var cell = GetChildByAttribute(cells[i], "class", "GridRowItemOverflow");

        if (cell.offsetHeight > maxHeight)
            maxHeight = cell.offsetHeight;
    }

    for (var i = 0; i < cells.length; i++) {
        if (maxHeight > cells[i].offsetHeight) {
            //window.setTimeout(function () {
                AppendSetHeightSmooth(cells[i], maxHeight);
            //});
        }
        else {
            cells[i].style.height = maxHeight + "px";
        }
    }

    //row.style.height = maxHeight + "px";
    AppendSetHeightSmooth(row, maxHeight);
}

var heightSmoothJobs = new Array();

function AppendSetHeightSmooth(item, height) {
    heightSmoothJobs.push({
        Item: item,
        Height: height
    });
}

function RunSetHeightSmoothJobs() {
    RunSetHeightSmoothJob(heightSmoothJobs.length - 1);
}

function RunSetHeightSmoothJob(index) {
    if (heightSmoothJobs.length == 0)
        return;

    if (heightSmoothJobs.length < 50) {
        SetHeightSmooth(
            heightSmoothJobs[index].Item,
            heightSmoothJobs[index].Height
        );
    }
    else {
        heightSmoothJobs[index].Item.style.height = heightSmoothJobs[index].Height + "px";
    }

    index--;

    if (index >= 0) {
        RunSetHeightSmoothJob(index);
    }
}

function SetHeightSmooth(item, height, onFinish) {
    var h = parseInt(item.style.height);

    if (isNaN(h))
        h = item.offsetHeight;

    if (h < height) {
        var factor = Math.abs(height - h) * 20 / 100;

        factor = parseInt(factor);

        if (factor == 0)
            factor = 1;

        item.style.height = (h + factor) + "px";

        window.setTimeout(function () {
            SetHeightSmooth(item, parseInt(height), onFinish)
        }, 1);
    }
    else if (h > height) {
        var factor = Math.abs(height - h) * 20 / 100;

        factor = parseInt(factor);

        if (factor == 0)
            factor = 1;

        item.style.height = (h - factor) + "px";

        window.setTimeout(function () {
            SetHeightSmooth(item, parseInt(height), onFinish)
        }, 1);
    }
    else {
        if (onFinish != undefined) {
            onFinish();
        }
    }
}

function CheckGridHeight2(row) {
    var cells = GetChildsByAttribute(row, "class", "GridHeadlineItem");

    var maxHeight = 0;

    for (var i = 0; i < cells.length; i++) {
        var cell = GetChildByAttribute(cells[i], "class", "GridHeadlineItemOverflow");

        if (cell.offsetHeight > maxHeight)
            maxHeight = cell.offsetHeight;
    }

    for (var i = 0; i < cells.length; i++) {
        cells[i].style.height = maxHeight + "px";
    }

    row.style.height = maxHeight + "px";
}

var resizeActivated = false;
var resizeIdGrid;
var resizeColumnName;

function ActivateResize(idGrid, columnName) {
    resizeIdGrid = idGrid;
    resizeColumnName = columnName;

    var grid = document.getElementById(idGrid);

    resizeActivated = true;
}

function ResizeColumn(e) {
    if (!resizeActivated)
        return;

    var grid = document.getElementById(resizeIdGrid);

    var headCells = grid.getElementsByTagName("gridsection").item(0).childNodes.item(0).getElementsByTagName("div");
    var bodyCells = grid.getElementsByTagName("gridsection").item(1).childNodes.item(0).getElementsByTagName("div");
    var gridRows = grid.getElementsByTagName("gridsection").item(1).childNodes;

    var headCell;
    var bodyCells = new Array();

    for (var i = 0; i < headCells.length; i++) {
        if (headCells.item(i).className == "GridHeadlineItem" &&
            headCells.item(i).getAttribute("ColumnName") == resizeColumnName) {
            headCell = headCells.item(i);
            break;
        }
    }

    for (var i = 0; i < gridRows.length; i++) {
        for (var a = 0; a < gridRows.item(i).childNodes.length; a++) {

            if (gridRows.item(i).childNodes.item(a).className == "GridRowItem" &&
                gridRows.item(i).childNodes.item(a).getAttribute("ColumnName") == headCell.getAttribute("ColumnName"))
                bodyCells.push(gridRows.item(i).childNodes.item(a));
        }
    }

    e = getEvent(e);
    target = getTarget(e);

    var minus = 5;

    if (target.tagName.toLowerCase() == "span") {
        target = target.parentNode;
        calc = true;

        minus += 2;
    }

    var calc = false;

    if (target.className == "GridHeadlineItemSearch") {
        if (browser != "Firefox")
            minus -= target.offsetLeft;

        target = target.parentNode;
        calc = true;
    }

    if (target.className != "GridHeadlineItem")
        return;

    var offsetLeft;

    if (e.offsetX != undefined) {
        offsetLeft = e.offsetX;
    }
    else {
        if (browser != "Firefox") {
            offsetLeft = e.layerX;
        }
        else {
            offsetLeft = e.layerX - grid.offsetLeft + 2;

            /*
            if (calc) {
                for (var i = 0; i < parseInt(target.getAttribute("Index")) ; i++) {
                    var cell = GetChildByAttribute(target.parentNode, "Index", i.toString());

                    if (cell != undefined)
                        offsetLeft -= cell.offsetWidth + 2;
                }
            }
            */
        }
    }

    offsetLeft = parseInt(offsetLeft) - minus;

    var rightSpace = target.parentNode.offsetWidth;

    var rowItems = GetChildsByAttribute(target.parentNode, "class", "GridHeadlineItem");

    for (var i = 0; i < rowItems.length; i++) {
        rightSpace -= rowItems[i].offsetWidth;
    }

    rightSpace -= 15;

    if (grid["IsScroll"])
        rightSpace -= 20;

    if (target.getAttribute("ColumnName") != resizeColumnName) {
        offsetLeft += 2;

        target.style.width = (parseInt(target.style.width) + (offsetLeft) + rightSpace) + "px";

        var targetBodyCells = new Array();

        for (var i = 0; i < gridRows.length; i++) {
            for (var a = 0; a < gridRows.item(i).childNodes.length; a++) {

                if (gridRows.item(i).childNodes.item(a).className == "GridRowItem" &&
                    gridRows.item(i).childNodes.item(a).getAttribute("ColumnName") == target.getAttribute("ColumnName"))
                    targetBodyCells.push(gridRows.item(i).childNodes.item(a));
            }
        }

        for (var i = 0; i < targetBodyCells.length; i++) {
            var targetBodyCell = targetBodyCells[i];

            targetBodyCell.style.width = target.style.width;
        }

        if (headCell != undefined)
            offsetLeft += headCell.offsetWidth;
    }
    else {
        var index = parseInt(target.getAttribute("Index")) + 1;

        var nextCell = GetChildByAttribute(target.parentNode, "index", index.toString());

        var nextCellWidth = parseInt(headCell.style.width) - offsetLeft;

        nextCellWidth += rightSpace;

        nextCell.style.width = (parseInt(nextCell.style.width) + nextCellWidth) + "px";

        var nextBodyCells = new Array();

        for (var i = 0; i < gridRows.length; i++) {
            var nextBodyCell = GetChildByAttribute(gridRows.item(i), "index", index.toString());

            if (nextBodyCell != undefined)
                nextBodyCell.style.width = nextCell.style.width;
        }
    }

    if (headCell != undefined)
        headCell.style.width = offsetLeft + "px";

    for (var i = 0; i < bodyCells.length; i++) {
        var bodyCell = bodyCells[i];

        if (bodyCell != undefined) {
            bodyCell.style.width = headCell.style.width;
        }

        AjustOverflowItems(resizeColumnName, offsetLeft - 2);
    }

    //CheckGridWidth(grid, grid["IsScroll"]);
}

function AjustOverflowItems(columnName, width) {
    var controls = document.getElementsByClassName("GridRowItemOverflow");

    for (var i = 0; i < controls.length; i++) {
        var control = controls.item(i);

        if (control.parentNode.getAttribute("ColumnName") != columnName)
            continue;

        control.style.width = width + "px";
    }
}

loadFunctions.push(function () {
    bodyMouseMove.push(ResizeColumn);

    AjustOverflows();
});

var filters = new Array();

function InitGridSearch(idGrid, columnIndex, sender, idHiddenField) {
    var existingBox = document.getElementById(idGrid + "Search" + columnIndex);

    if (existingBox != undefined) {
        existingBox.value = "";

        existingBox.Hide();

        GridSearch(idGrid, existingBox["ColumnIndex"], "");

        return;
    }

    for (var i = 0; i < filters.length; i++) {
        var filter = filters[i];

        var box = document.getElementById(filter.IdGrid + "Search" + filter.ColumnIndex);

        if (box != undefined) {
            box.value = "";

            box.Hide();

            GridSearch(idGrid, box["ColumnIndex"], "");
        }
    }

    var searchBoxContainer = document.createElement("div");
    searchBoxContainer.className = "GridSearch"

    var searchBox = document.createElement("input");
    searchBox.id = idGrid + "Search" + columnIndex;
    searchBox.type = "text";
    searchBox["IdGrid"] = idGrid;
    searchBox["ColumnIndex"] = columnIndex;
    searchBox["IdHiddenField"] = idHiddenField;

    searchBox.onkeyup = function () {
        window.setTimeout(function () {
            GridSearch(idGrid, columnIndex, searchBox.value);

            var filterIndex = GetFilter(idGrid, columnIndex);

            filters[filterIndex].value = searchBox.value;

            var hiddenField = document.getElementById(idHiddenField);
            hiddenField.value = searchBox.value;
        }, 100);
    };

    var _filter = new Object();
    _filter.IdGrid = idGrid;
    _filter.ColumnIndex = columnIndex;
    _filter.value = "";

    filters.push(_filter);
    

    searchBox["Hide"] = function () {
        if (this.value != "")
            return;

        this.parentNode.parentNode.removeChild(this.parentNode);

        HighlightSearchColumn(this["IdGrid"], this["ColumnIndex"], true);

        var filterIndex = GetFilter(this["IdGrid"], this["ColumnIndex"]);

        filters.splice(filterIndex, 1);

        var hiddenField = document.getElementById(this["IdHiddenField"]);
        hiddenField.value = "";
    };

    searchBox.onblur = searchBox["Hide"];

    var filterIndex = GetFilter(idGrid, columnIndex);

    var filter = undefined;

    if (filterIndex != undefined) {
        filter = filters[filterIndex];

        searchBox.value = filter.value;
    }

    searchBoxContainer.appendChild(searchBox);

    sender.parentNode.appendChild(searchBoxContainer, sender);

    HighlightSearchColumn(idGrid, columnIndex);

    searchBox.focus();
}

function GetFilter(idGrid, columnIndex) {
    var index = undefined;

    for (var i = 0; i < filters.length; i++) {
        if (filters[i].IdGrid == idGrid &&
            filters[i].ColumnIndex == columnIndex) {
            index = i;

            break;
        }
    }

    return index;
}

function HighlightSearchColumn(idGrid, columnIndex, isReset) {
    return;

    var grid = document.getElementById(idGrid);

    if (grid == undefined)
        return;

    var gridSectionItems = GetChildByAttribute(grid, "class", "GridOverflow");

    if (gridSectionItems == undefined)
        return;

    gridSectionItems = gridSectionItems.childNodes.item(0);

    var gridRows = GetChildsByAttribute(gridSectionItems, "class", "GridRow");
    gridRows.push(GetChildByAttribute(gridSectionItems, "class", "GridRow_Active"));

    // Run through all rows.
    for (var i = 0; i < gridRows.length; i++) {
        var gridRow = gridRows[i];

        var columns = GetChildsByAttribute(gridRow, "class", "GridRowItem");

        var column = GetChildByAttribute(gridRow, "Index", columnIndex);

        if(gridRow.className != "GridRow_Active" || isReset == true)
        {
            for (var a = 0; a < columns.length; a++) {
                //columns[a].style.backgroundColor = "rgb(255,255,255)";
                columns[a].style.backgroundColor = "";

                var to = [230,230,230];

                //columns[a].style.background = "#E6E6E6";
                //if (isReset != true && columns[a].getAttribute("Index") != columnIndex.toString())
                    //ConvertColor(to, columns[a], "Decrease");


                columns[a].style.boxShadow = "";
                columns[a].style.borderTop = "";
                columns[a].style.borderBottom = "";
                columns[a].style.color = "";

                if (isReset != undefined && isReset == true) {
                    //if (columns[a].getAttribute("Index") != columnIndex.toString())
                    //    columns[a].style.backgroundColor = "rgb(230,230,230)";

                    //ConvertColor([255, 255, 255], columns[a], "Increase");
                    columns[a].style.backgroundColor = "";

                    continue;
                }
            }
        }

        if (column != undefined && isReset != true) {
            column["ColorMode"] = undefined;

            column.style.backgroundColor = "rgb(255,255,255)";
            column.style.color = "#000000";

            //var to = [255, 255, 255];

            //ConvertColor(to, column, "Increase");

            //column.style.boxShadow = "-10px 10px 10px 1px #555555";
            column.style.boxShadow = "0px 0px 10px 5px #FFFFFF";
            //column.style.borderTop = "1px solid #cccccc";
            column.style.borderBottom = "1px solid #aaaaaa";
        }
    }
}

function GridSearch(idGrid, columnIndex, text) {
    var grid = document.getElementById(idGrid);

    if (grid == undefined)
        return;

    var gridSectionItems = GetChildByAttribute(grid, "class", "GridOverflow");

    if (gridSectionItems == undefined)
        return;

    gridSectionItems = gridSectionItems.childNodes.item(0);

    var gridRows = GetChildsByAttribute(gridSectionItems, "class", "GridRow");

    // Run through all rows.
    for (var i = 0; i < gridRows.length; i++) {
        var gridRow = gridRows[i];

        var column = GetChildByAttribute(gridRow, "Index", columnIndex);

        var columnText = GetChildByAttribute(column, "class", "GridRowItemOverflow").innerHTML;

        if (columnText == undefined || columnText.toLowerCase().search(text.toLowerCase()) == -1) {
            gridRow.style.display = "none";
        }
        else {
            gridRow.style.display = "";
        }
    }
}

function CheckGridButtons() {
    for (var i = 0; i < gridButtons.length; i++) {
        var idGrid = gridButtons[i].IdGrid;

        var gridSelected = GridSelected(idGrid);

        for (var a = 0; a < gridButtons[i].Buttons.length; a++) {
            var button = document.getElementById(gridButtons[i].Buttons[a]);

            if (button == undefined)
                continue;

            var mode = button.getAttribute("Mode");

            if (mode == "Always")
                continue;

            if(gridSelected == false) {
                button.style.display = "none";
            }
            else {
                button.style.display = "";
            }
        }
    }
}

loadFunctions.push(function () {
    loadFunctions.push(CheckGridButtons);
});

function GridSelected(idGrid) {
    var result = false;

    try
    {
        var grid = document.getElementById(idGrid);

        var gridSectionItems = GetChildByAttribute(grid, "class", "GridOverflow");

        if (gridSectionItems == undefined)
            return;

        gridSectionItems = gridSectionItems.childNodes.item(0);

        var gridRow = GetChildByAttribute(gridSectionItems, "class", "GridRow_Active");

        if (gridRow == undefined)
            result = false;
        else
            result = true;
    }
    catch(e)
    {}

    return result;
}

function getEvent(e) {
    if (window.event != undefined)
        return window.event;

    return e;
}

function getTarget(obj) {
    var targ;
    var e = obj;
    if (e.srcElement != undefined) targ = e.srcElement;
    else if (e.target != undefined) targ = e.target;

    if (targ.nodeType == 3) // defeat Safari bug
        targ = targ.parentNode;
    return targ;
}