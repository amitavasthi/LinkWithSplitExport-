var workflowSelectionHasChanged = false;

var workflowSelectionRequestsPending = 0;
function SelectAllWorkflowItems(sender) {
    var activeItems = GetChildsByAttribute(sender, "class", "WorkflowSelectorItem WorkflowSelectorItem_Active BackgroundColor1", true);
    var inactiveItems = GetChildsByAttribute(sender, "class", "WorkflowSelectorItem BackgroundColor5H", true);

    if (activeItems.length == 0) {
        for (var i = 0; i < inactiveItems.length; i++) {
            inactiveItems[i].onclick();
        }
    }
    else {
        for (var i = 0; i < activeItems.length; i++) {
            activeItems[i].onclick();
        }
    }
}

function SelectWorkflowItem(selectorItem, source, idWorkflowSelection, workflowSelection, workflowSelectionVariable, service, idItem) {
    var parameters = "Source=" + source + "&WorkflowSelection=" + workflowSelection + "&WorkflowSelectionVariable=" +
        workflowSelectionVariable + "&IdItem=" + idItem + "&Action=";

    if (selectorItem.getAttribute("State") == "Selected") {
        parameters += "DeSelect";

        selectorItem.setAttribute("State", "none");
        selectorItem.className = "WorkflowSelectorItem BackgroundColor5H";
    } else {
        parameters += "Select";

        selectorItem.setAttribute("State", "Selected");
        selectorItem.className = "WorkflowSelectorItem WorkflowSelectorItem_Active BackgroundColor1";
    }

    workflowSelectionHasChanged = true;

    workflowSelectionRequestsPending++;
    _AjaxRequest(service, "SelectWorkflowSelectorItem", parameters, function (response) {
        workflowSelectionRequestsPending--;

        if ($(".WorkflowPinned").length != 0) {           
            if (window.location.toString().search("/LinkReporter/Crosstabs.aspx") != -1)
                ReloadCrosstable();
        }       
    });    
}

var workflowSelectionDetailVisible = false;
function ShowWorkflowSelectionDetail(sender) {
    
    var workflowContainer = GetChildByAttribute(document.getElementById("cphContent_pnlWorkflow"), "class", "Workflow Color1");
    var workflowBackground = document.getElementById("WorkflowBackground");

    if (workflowContainer.offsetHeight > 0) {
        if (workflowSelectionRequestsPending > 0) {
            ShowLoading(workflowContainer);
            window.setTimeout(function () { ShowWorkflowSelectionDetail(sender) }, 500);
            return;
        }

        HideLoading();

        DecreaseOpacity(workflowBackground, function () {
            workflowBackground.style.display = "none";

            if (workflowSelectionHasChanged) {
                workflowSelectionHasChanged = false;

                if (window.location.toString().search("/LinkReporter/Crosstabs.aspx") != -1)
                    ReloadCrosstable();
               
                UpdateSetting('AutoUpdate', 'true', true, true);
            }
        });

        DecreaseHeight(workflowContainer, 0, function () {
            if (sender != undefined)
                sender.src = "/Images/Icons/Expand.png";
        });
        if (document.getElementById("chkWorkflowAllTabs").checked) {
            AjaxRequest("ApplyWorkflowFilterToAllTabs", "", function (response) {
                ShowLoading(document.body);
                window.location = window.location;
            });
            document.getElementById("WorkflowContainer").getElementsByClassName("Customcheckbox")[0].src = "/Images/Icons/Boxes/checkbox/Inactive.png";
            document.getElementById("chkWorkflowAllTabs").checked = false;
        }
        var workflowSelections = GetChildsByAttribute(workflowContainer, "class", "WorkflowSelection BorderColor1");

        document.getElementsByClassName("WorkflowSelectorSection")[0].style.display = "none";

       
    }
    else {
       
        document.getElementsByClassName("WorkflowSelectorSection")[0].style.display = "inline-block";
        workflowSelectionHasChanged = false;

        workflowBackground.style.opacity = "0.0";
        workflowBackground.style.display = "";

        IncreaseOpacity(workflowBackground, undefined, 0.5);

        var height = parseInt(window.innerHeight / 3);
        IncreaseHeight(workflowContainer, height, function () {
            if (sender != undefined)
                sender.src = "/Images/Icons/Collapse.png";
        });

        var workflowSelections = GetChildsByAttribute(workflowContainer, "class", "WorkflowSelection BorderColor1");

        var width = parseInt(window.innerWidth / workflowSelections.length) - 2;

        // Run through all workflow selections.
        for (var i = 0; i < workflowSelections.length; i++) {
            workflowSelections[i].style.width = width + "px";

            var selector = GetChildByAttribute(workflowSelections[i], "class", "WorkflowSelector", true);

            selector.style.height = (height - 59) + "px";
            selector.style.maxHeight = (height - 59) + "px";
        }


    }
   
}

function ShowWorkflowOnHover(sender) {
    if (sender.src.indexOf("Expand") == -1)
        return;
    var workflowContainer = GetChildByAttribute(document.getElementById("cphContent_pnlWorkflow"), "class", "Workflow Color1");
    var height = parseInt(window.innerHeight / 3) + 115;
    var workflowSelections = GetChildsByAttribute(workflowContainer, "class", "WorkflowSelection BorderColor1");
    var workflowContainer = document.createElement("div");
    workflowContainer.id = "workflowOnHover";
    workflowContainer.className = "Workflow Color1";
    workflowContainer.style.position = "relative";
    workflowContainer.style.bottom = (30 + height) + "px";
    workflowContainer.style.height = height + "px";
    workflowContainer.style.width = "100%";
    workflowContainer.style.textAlign = "center";
    workflowContainer.style.border = "2px solid #6CAEE0";
    document.body.appendChild(workflowContainer);

    var width = parseInt(window.innerWidth / workflowSelections.length) - 2;

    // Run through all workflow selections.
    for (var i = 0; i < workflowSelections.length; i++) {
        workflowSelections[i].style.width = width + "px";
        var selector = GetChildByAttribute(workflowSelections[i], "class", "WorkflowSelector", true);
        workflowContainer.innerHTML += "<div class=\"WorkflowSelection BorderColor1\" style=\"width:" + width + "px;\">" + workflowSelections[i].innerHTML + "</div>";
    }
    //var Variables = document.getElementById("workflowOnHover").getElementsByClassName("WorkflowSelector");
    //for (var i = 0; i < Variables.length; i++) {
    //    Variables[i].style.height = (height - 59) + "px";
    //    Variables[i].style.maxHeight = (height - 59) + "px";
    //}
}
function ShowWorkflowOnOut(sender) {
  //  setTimeout(function () {
        if (document.getElementById("workflowOnHover") != null) {
            document.body.removeChild(document.getElementById("workflowOnHover"));
        }
    //}, 500)

}

function ReloadCrosstable() {
    PopulateCrosstable();
    return;


    var id = "cphContent_pnl";

    //ShowLoading(document.getElementById("Content"));
    ShowLoading(document.body);

    //document.getElementById(id).innerHTML = "<table style='height:100%;width:100%;'><tr><td><img src='/Images/Icons/Loading.gif' /></td></tr></table>"

    AjaxRequest("BuildCrosstable", "", function (response) {
        var container = document.getElementById(id);
        
        container.innerHTML = response;

        EvaluateScripts(container);

        InitBoxes();
        HideLoading();
        InitNestedLeftVariableSelectors();
        ScrollVariableLabel();
    });
}

function EvaluateScripts(container) {
    var scripts = container.getElementsByTagName("script");

    for (var i = 0; i < scripts.length; i++) {
        var script = scripts.item(i);

        eval(script.innerHTML);
    }
}