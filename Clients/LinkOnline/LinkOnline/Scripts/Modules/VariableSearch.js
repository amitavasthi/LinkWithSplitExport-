function ShowFullScreenPanel(id, animate) {
    if (animate == undefined)
        animate = true;

    var container = document.getElementById(id);

    container.style.display = "";

    if (animate) {
        IncreaseWidth(container, window.innerWidth);
        IncreaseHeight(container, window.innerHeight);
    }
    else {
        container.style.width = window.innerWidth + "px";
        container.style.height = (window.innerHeight) + "px";
    }
}

function HideFullScreenPanel(id) {
    var container = document.getElementById(id);

    DecreaseHeight(container, 0);
    DecreaseWidth(container, 0, false, function () {
        container.style.display = "none";
    });
}

var lastSearchRequest = "";
var variableSearchSource = undefined;
var _enableDataCheck;
var activeVariableSearch = undefined;
function SearchVariablesWithAPI(source, idLanguage, enableDataCheck, apiSearchTxt) {    
    var searchValue = apiSearchTxt[0].apiText;
    if (searchValue == undefined)
        searchValue = "";

    lastSearchRequest = searchValue;

    var pnlVariables = document.getElementById("variableSearchResults");

    pnlVariables.style.height = (window.innerHeight - 300) + "px";

    if (document.getElementById("LoadingBlur") == undefined)
        ShowLoading(document.getElementById("variableSearchResults"));

    // var parameters = "EnableDataCheck=" + false + "&Source=" + source + "&Expression=" + txtSearch.value;
    var parameters = "EnableDataCheck=" + false + "&Source=" + source + "&Expression=" + JSON.stringify(apiSearchTxt);
    //  var parameters = "EnableDataCheck=" + false + "&Source=" + source + "&Expression=" + apiSearchTxt[0].apiText;
    if (idLanguage != undefined)
        parameters += "&IdLanguage=" + idLanguage;

    if (activeVariableSearch != undefined)
        activeVariableSearch.abort();
    parameters = encodeURI(parameters);
    activeVariableSearch = AjaxRequest("SearchVariables", parameters, function (response) {
        activeVariableSearch = undefined;
        var pnlVariables = document.getElementById("variableSearchResults");

        pnlVariables.style.height = (window.innerHeight - 300) + "px";

        var split = response.split('###SPLIT###');

        var searchExpression = split[0];

        if (searchExpression != lastSearchRequest)
            return;

        pnlVariables.innerHTML = split[1];

        var scripts = pnlVariables.getElementsByTagName("script");

        for (var i = 0; i < scripts.length; i++) {
            eval(scripts.item(i).innerHTML);
        }

        HideLoading();

        // Run through all variable select items.
        for (var i = 0; i < pnlVariables.childNodes.length; i++) {
            var pnlVariable = pnlVariables.childNodes.item(i);

            if (pnlVariable.className == undefined || pnlVariable.className.search("VariableSelectorControl") == -1)
                continue;

            DragVariableSelector(pnlVariable, false);
        }
    });
}
function SearchVariables(source, idLanguage, enableDataCheck) {

    //var apiSearchTxt = "";    
    var apiSearchTxt = [];

    _enableDataCheck = enableDataCheck;

    if (enableDataCheck == undefined)
        enableDataCheck = true;

    if (source == undefined && variableSearchSource != undefined)
        source = variableSearchSource;

    variableSearchSource = source;

    var txtSearch = document.getElementById("cphContent_VariableSearch_txtVariableSearch");

    //apiSearchTxt = txtSearch.value;
    apiSearchTxt.push({ key: 0, apiText: txtSearch.value });

    // API for spell check and synoniums
   document.getElementById("cphContent_cogSrchApi")
   if (document.getElementById("cphContent_cogSrchApi") !=undefined &&  document.getElementById("cphContent_cogSrchApi").value == "true") {
    $.ajax({
        url: 'http://159.65.94.148:8008/v1/end_point/',
        type: 'POST',
        accept: 'application/json',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        dataType: "json",
        'Accept-Encoding': 'gzip, deflate',
        data: { "text": apiSearchTxt[0].apiText },
        success: function (resp) {
            // apiSearchTxt = resp.text;
            apiSearchTxt.push({ key: 1, apiText: resp.text });            
            SearchVariablesWithAPI(source, idLanguage, enableDataCheck, apiSearchTxt);
        },
        error: function (error) {
            SearchVariablesWithAPI(source, idLanguage, enableDataCheck, apiSearchTxt);
        }
    });
    //
    } else {
        SearchVariablesWithAPI(source, idLanguage, enableDataCheck, apiSearchTxt);
    }

}
//function SearchVariables1(source, idLanguage, enableDataCheck) {
   
//    _enableDataCheck = enableDataCheck;

//    if (enableDataCheck == undefined)
//        enableDataCheck = true;

//    if (source == undefined && variableSearchSource != undefined)
//        source = variableSearchSource;

//    variableSearchSource = source;

//    var txtSearch = document.getElementById("cphContent_VariableSearch_txtVariableSearch");

//    var searchValue = txtSearch.value;

//    if (searchValue == undefined)
//        searchValue = "";

//    lastSearchRequest = searchValue;

//    var pnlVariables = document.getElementById("variableSearchResults");

//    pnlVariables.style.height = (window.innerHeight - 300) + "px";

//    if (document.getElementById("LoadingBlur") == undefined)
//        ShowLoading(document.getElementById("variableSearchResults"));

//    var parameters = "EnableDataCheck=" + false + "&Source=" + source + "&Expression=" + txtSearch.value;

//    if (idLanguage != undefined)
//        parameters += "&IdLanguage=" + idLanguage;

//    if (activeVariableSearch != undefined)
//        activeVariableSearch.abort();
//    parameters = encodeURI(parameters);
//    activeVariableSearch = AjaxRequest("SearchVariables", parameters, function (response) {
//        activeVariableSearch = undefined;
//        var pnlVariables = document.getElementById("variableSearchResults");

//        pnlVariables.style.height = (window.innerHeight - 300) + "px";

//        var split = response.split('###SPLIT###');

//        var searchExpression = split[0];

//        if (searchExpression != lastSearchRequest)
//            return;

//        pnlVariables.innerHTML = split[1];

//        var scripts = pnlVariables.getElementsByTagName("script");

//        for (var i = 0; i < scripts.length; i++) {
//            eval(scripts.item(i).innerHTML);
//        }

//        HideLoading();

//        // Run through all variable select items.
//        for (var i = 0; i < pnlVariables.childNodes.length; i++) {
//            var pnlVariable = pnlVariables.childNodes.item(i);

//            if (pnlVariable.className == undefined || pnlVariable.className.search("VariableSelectorControl") == -1)
//                continue;

//            DragVariableSelector(pnlVariable, false);
//        }
//    });
//}

var resultDiff;
var resultDropAreaCoordinate;

function DragVariableSelector(pnlVariable, onCrosstable) {
    var dropAreaCoordinates = new Array();

    if (onCrosstable)
        pnlVariable.style.height = pnlVariable.offsetHeight + "px";

    var dragHandle = GetChildByAttribute(pnlVariable, "class", "VariableSelectorVariableLabel", true);

    MakeDragable(
        pnlVariable,
        dragHandle,
        false,
        function (control) {
            var found = false;
            var overBin = false;

            var message = document.getElementById("DropAreaMessage");

            if (message != undefined)
                message.parentNode.removeChild(message);

            control.style.position = "absolute";
            UnBlurTable();
            var cancelBin = document.getElementById("imgVariableSearchCancelBin");

            var left = parseInt(control.style.left) + (control.offsetWidth / 2);
            var top = parseInt(control.style.top);

            if ((left > cancelBin.offsetLeft || (left + control.offsetWidth) > cancelBin.offsetLeft) && left < (cancelBin.offsetLeft + cancelBin.offsetWidth)) {

                if ((top > cancelBin.offsetTop || (top + control.offsetHeight) > cancelBin.offsetTop) && top < (cancelBin.offsetTop + cancelBin.offsetHeight)) {
                    cancelBin.src = "/Images/Icons/Bin_Hover.png";
                    found = true;
                    overBin = true;
                } else {
                    cancelBin.src = "/Images/Icons/Bin.png";
                }
            }
            else {
                cancelBin.src = "/Images/Icons/Bin.png";
            }

            if (document.getElementById("pnlData")) {
                top += document.getElementById("pnlData").scrollTop;
            }

            cancelBin.style.display = "none";
            tableScrollLocked = false;

            if (resultDropAreaCoordinate != undefined && overBin == false) {
                control.style.textAlign = "center";

                if (left > (resultDropAreaCoordinate.Left - 60))
                    DecreaseLeft(control, resultDropAreaCoordinate.Left - 60);
                else
                    IncreaseLeft(control, resultDropAreaCoordinate.Left - 60);

                DecreaseTop(control, resultDropAreaCoordinate.Top);

                if (control.offsetWidth < (resultDropAreaCoordinate.Width + 120))
                    IncreaseWidth(control, resultDropAreaCoordinate.Width + 120, false);
                else
                    DecreaseWidth(control, resultDropAreaCoordinate.Width + 120, false);

                found = true;

                SelectVariable(
                    control,
                    resultDropAreaCoordinate.DropArea.getAttribute("Source"),
                    resultDropAreaCoordinate.DropArea.getAttribute("Path"),
                    control.getAttribute("IdVariable"),
                    control.getAttribute("IsTaxonomy"),
                    resultDropAreaCoordinate.DropArea.getAttribute("IdSelected")
                );
            }

            if (found == false || overBin == true) {
                control.style.position = "";
                control.parentNode.removeChild(control);
            }

            var dropAreas = GetChildsByAttribute(document.body, "DropArea", "True", true);

            for (var i = 0; i < dropAreas.length; i++) {
                dropAreas[i].style.opacity = "1.0";

                if (resultDropAreaCoordinate == undefined || dropAreas[i] != resultDropAreaCoordinate.DropArea) {
                    if (dropAreas[i].getAttribute("DropAreaMessage") != "DropAreaMessageWeight")
                        dropAreas[i].style.background = "";
                    else
                        dropAreas[i].style.backgroundColor = "";
                }

                if (dropAreas[i].getAttribute("DropAreaMessage") == "DropAreaMessageNest") {

                    if (dropAreas[i].getAttribute("Position") == "Left") {
                        dropAreas[i].style.height = dropAreas[i].parentNode.offsetHeight + "px";
                        dropAreas[i].style.display = "none";
                    }
                    else {
                        dropAreas[i].style.width = dropAreas[i].offsetWidth + "px";
                        dropAreas[i].style.position = "absolute";
                        dropAreas[i].style.display = "none";
                    }
                }

                dropAreaCoordinates.push({
                    Left: GetOffsetLeft(dropAreas[i]) - GetScrollLeft(dropAreas[i]),
                    Top: GetOffsetTop(dropAreas[i], true),
                    Width: dropAreas[i].offsetWidth,
                    Height: dropAreas[i].offsetHeight,
                    DropArea: dropAreas[i]
                });

                try {
                    dropAreas[i].parentNode.style.opacity = "1.0";
                    dropAreas[i].parentNode.parentNode.style.opacity = "1.0";
                }
                catch (e) { }
            }
        },
        function (control) {
            var variableSearch = document.getElementById("boxVariableSearch");
            var cancelBin = document.getElementById("imgVariableSearchCancelBin");

            var offsetTop = 0;
            var offsetLeft = 0;
            if (document.getElementById("pnlData")) {
                offsetTop = document.getElementById("pnlData").scrollTop * -1;
                offsetLeft = document.getElementById("pnlData").scrollLeft * -1;
            }

            tableScrollLocked = true;

            cancelBin.style.display = "";

            variableSearch.style.display = "none";
            variableSearch.style.height = "0px";
            variableSearch.style.width = "0px";

            BlurTable();

            var dropAreas = GetChildsByAttribute(document.body, "DropArea", "True", true);

            for (var i = 0; i < dropAreas.length; i++) {
                if (dropAreas[i].getAttribute("DropAreaMessage") == "DropAreaMessageWeight") {
                    if (control.getAttribute("VariableType") != "Numeric")
                        continue;
                }
                else if (dropAreas[i].getAttribute("DropAreaMessage") == "DropAreaMessageNest") {

                    if (dropAreas[i].getAttribute("Position") == "Left") {
                        dropAreas[i].style.height = dropAreas[i].parentNode.offsetHeight + "px";
                        dropAreas[i].style.position = "absolute";
                        dropAreas[i].style.display = "";
                    }
                    else {
                        dropAreas[i].style.width = dropAreas[i].parentNode.offsetWidth + "px";
                        dropAreas[i].style.position = "absolute";
                        dropAreas[i].style.display = "";
                    }
                }
                dropAreas[i].style.opacity = "1.0";

                dropAreaCoordinates.push({
                    Left: GetOffsetLeft(dropAreas[i]) - GetScrollLeft(dropAreas[i]),
                    Top: GetOffsetTop(dropAreas[i], true),
                    Width: dropAreas[i].offsetWidth,
                    Height: dropAreas[i].offsetHeight,
                    DropArea: dropAreas[i]
                });

                if (dropAreas[i].getAttribute("DropAreaMessage") == "DropAreaMessageNest") {
                    if (dropAreas[i].getAttribute("Position") == "Left") {
                        //dropAreas[i].style.marginTop = offsetTop + "px";
                    }
                    else {
                        //dropAreas[i].style.marginLeft = offsetLeft + "px";
                    }
                }

                try {
                    dropAreas[i].parentNode.style.opacity = "1.0";
                    dropAreas[i].parentNode.parentNode.style.opacity = "1.0";
                }
                catch (e) { }
            }

            for (var i = 0; i < dropAreas.length; i++) {
                if (dropAreas[i].offsetHeight < 10) {
                    dropAreas[i].style.height = "10px";
                }

                if (dropAreas[i].offsetWidth < 10) {
                    dropAreas[i].style.width = "10px";
                }
            }
        },
        true,
        function (control) {
            var pnlData = document.getElementById("pnlData");
            var found = false;

            var message = document.getElementById("DropAreaMessage");

            if (message == undefined) {
                message = document.createElement("div");
                message.id = "DropAreaMessage";
                message.className = "DropAreaMessage Color1";

                document.body.appendChild(message);
            }

            var cancelBin = document.getElementById("imgVariableSearchCancelBin");

            var left = parseInt(control.style.left) + (control.offsetWidth / 2);
            var top = parseInt(control.style.top);

            message.innerHTML = "";

            if ((left > cancelBin.offsetLeft || (left + control.offsetWidth) > cancelBin.offsetLeft) && left < (cancelBin.offsetLeft + cancelBin.offsetWidth)) {

                if ((top > cancelBin.offsetTop || (top + control.offsetHeight) > cancelBin.offsetTop) && top < (cancelBin.offsetTop + cancelBin.offsetHeight)) {
                    cancelBin.src = "/Images/Icons/Bin_Hover.png";
                    found = true;

                    if (control.getAttribute("OnCrosstable") == "True") {
                        message.innerHTML = LoadLanguageText("DropAreaMessageDelete");
                    } else {
                        message.innerHTML = LoadLanguageText("DropAreaMessageCancel");
                    }
                } else {
                    cancelBin.src = "/Images/Icons/Bin.png";
                }
            }
            else {
                cancelBin.src = "/Images/Icons/Bin.png";
            }

            resultDropAreaCoordinate = undefined;
            resultDiff = undefined;

            //left = parseInt(control.style.left) - (control.offsetWidth / 2);
            //left = parseInt(control.style.left) + (control.offsetWidth / 2);
            left = parseInt(control.style.left);
            top = parseInt(control.style.top);
            if (pnlData) {
                //top = parseInt(control.style.top) + document.getElementById("pnlData").scrollTop;
                //top = parseInt(control.style.top) + (document.getElementById("pnlData").scrollTop * -1);
                //top = document.getElementById("pnlData").scrollTop - parseInt(control.style.top);
            }

            // Run through the coordinates of all available drop areas.
            for (var i = 0; i < dropAreaCoordinates.length; i++) {
                var dropAreaCoordinate = dropAreaCoordinates[i];

                if (dropAreaCoordinate.DropArea.getAttribute("DropAreaMessage") == "DropAreaMessageNest") {
                    if (pnlData) {

                        if (dropAreaCoordinate.DropArea.getAttribute("Position") == "Left") {
                            //dropAreaCoordinate.DropArea.style.marginTop = "-" + pnlData.scrollTop + "px";
                        } else {
                            dropAreaCoordinate.DropArea.style.marginLeft = "-" + pnlData.scrollLeft + "px";
                        }
                    }
                }

                //var xDiff = Math.abs((left) - (dropAreaCoordinate.Left + (dropAreaCoordinate.DropArea.offsetWidth / 2)));
                var xDiff = Math.abs((left) - (dropAreaCoordinate.Left));
                var yDiff = Math.abs((top + (control.offsetHeight / 2)) - (dropAreaCoordinate.Top + (dropAreaCoordinate.DropArea.offsetHeight / 2)));

                if (xDiff < 0 || yDiff < 0) {
                    if (dropAreaCoordinate.DropArea.getAttribute("DropAreaMessage") != "DropAreaMessageWeight")
                        dropAreaCoordinate.DropArea.style.background = "";
                    else
                        dropAreaCoordinate.DropArea.style.backgroundColor = "";
                    continue;
                }

                var diff = xDiff + yDiff;

                if (resultDiff == undefined || diff < resultDiff) {
                    resultDropAreaCoordinate = dropAreaCoordinate;
                    resultDiff = diff;

                    if (dropAreaCoordinate.DropArea.getAttribute("DropAreaMessage") != undefined && cancelBin.src.indexOf("Bin_Hover.png")==-1)
                        message.innerHTML = LoadLanguageText(dropAreaCoordinate.DropArea.getAttribute("DropAreaMessage"));
                }

                if (dropAreaCoordinate.DropArea.getAttribute("DropAreaMessage") != "DropAreaMessageWeight")
                    dropAreaCoordinate.DropArea.style.background = "";
                else
                    dropAreaCoordinate.DropArea.style.backgroundColor = "";

                //safari studio bug no. 590 appears when uncomment below line.
                dropAreaCoordinate.DropArea.parentNode.style.overflow = "hidden";
            }

            if (found)
                return;

            if (resultDropAreaCoordinate != undefined) {
                if (resultDropAreaCoordinate.DropArea.getAttribute("DropAreaTypeRestriction") != undefined) {
                    var types = resultDropAreaCoordinate.DropArea.getAttribute("DropAreaTypeRestriction").split(",");

                    if (ArrayContains(types, control.getAttribute("VariableType")) == false) {
                        resultDropAreaCoordinate.DropArea.style.background = "#FF0000";
                        resultDropAreaCoordinate.DropArea.parentNode.style.overflow = "visible";

                        resultDropAreaCoordinate = undefined;

                        return;
                    }
                }

                if (resultDropAreaCoordinate.DropArea.getAttribute("DropAreaMessage") != "DropAreaMessageWeight")
                    resultDropAreaCoordinate.DropArea.style.background = "#61CF71";
                else
                    resultDropAreaCoordinate.DropArea.style.backgroundColor = "#61CF71";

                resultDropAreaCoordinate.DropArea.parentNode.style.overflow = "visible";
            }
        }
    );
}

function SelectVariable(sender, source, path, idVariable, isTaxonomy, idSelected) {
    var parameters = "Source=" + source + "&Path=" + path + "&IdVariable=" + idVariable + "&IsTaxonomy=" + isTaxonomy;

    if (idSelected != undefined)
        parameters += "&IdSelected=" + idSelected;

    var service = "/Handlers/GlobalHandler.ashx";

    if (source.search("/App_Data/ReportingWorkflows/") != -1)
        service = "/Handlers/WorkflowDefinitionHandler.ashx";

    _AjaxRequest(service, "SelectVariable", parameters, function (response) {

        if (source.search("/App_Data/ReportingWorkflows/") != -1) {
            window.location = window.location;
            return;
        }

        if (window.location.toString().search("/LinkReporter/Crosstabs.aspx") == -1) {
            window.setTimeout(function () { window.location = window.location; }, 500);
            return;
        }

        crosstableIsCaching = true;

        if (path == "WEIGHT") {
            window.location = window.location;
            return;
        }

        try {
            PopulateCrosstableDelayed(500, function () {
                sender.parentNode.removeChild(sender);

                var hasData = response == "True";

                var pnlGoButton = document.getElementById("cphContent_pnlGoButton");

                if (hasData) {
                    pnlGoButton.className = "GoButton2";
                }
                else {
                    pnlGoButton.className = "GoButton2 GreenBackground2I";
                }
            });
        }
        catch (e) {
            window.location = window.location;
        }
    });
}