var reAggregateOnEquationClose;
var equationEditorActive = false;

function EditEquation(source, path, reaggregate) {
    if (reaggregate != undefined) {
        reAggregateOnEquationClose = reaggregate;
    }
    else {
        reAggregateOnEquationClose = false;
    }

    equationScoreSource = source;
    equationScorePath = path;

    _AjaxRequest("/Handlers/VariableSelector.ashx", "GetEquation", "Source=" + source + "&Path=" + path, function (response) {
        InitDragBox("boxEquationDefinitionControl");
        window.editor.setValue(htmlDecode(response));
        equationEditorActive = true;

        //window.setTimeout(ValidateEquation, 1000);
        ValidateEquation();
    });
}


function SetEquation(source, path, equation, populateCrosstable) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "SetEquation", "Source=" + source + "&Path=" + path + "&Equation=" + encodeURIComponent(equation), function (response) {
        if (populateCrosstable || reAggregateOnEquationClose) {
            equationEditorActive = false;
            //window.setTimeout(PopulateCrosstable, 500);
            UpdateSetting('AutoUpdate', 'true', true, true);
        }
    });
}

function EquationInsertCategory() {
    InitDragBox("boxEquationDefinitionCategorySearchControl", "Top");

    var categorySearch = document.getElementById("cphContent_EquationDefinition_csEquationDefinition");
    //only for split and export or remove checkbox
    if (document.getElementById("cphContent_EquationDefinition_chkSplitnExportAllTabs") != null) {
        var span = document.getElementById("cphContent_EquationDefinition_chkSplitnExportAllTabs").parentNode;
        span.style.display = "none";
    }
    categorySearch.Search();

    categorySearch.OnConfirm = function (control) {
        CloseBox("boxEquationDefinitionCategorySearchControl", "Top");

        if (control.SelectedItems.length == 0)
            return;

        var placeHolder = "";

        for (var i = 0; i < control.SelectedItems.length; i++) {
            placeHolder += "[";
            
            if (control.SelectedItems[i].IsTaxonomy == "false") {
                placeHolder += "/" + control.SelectedItems[i].IdStudy + "\\";
            }

            if (control.SelectedItems[i].Name == "" || control.SelectedItems[i].Name == undefined) {
                placeHolder += control.SelectedItems[i].VariableName + "] + ";
            }
            else {
                placeHolder += control.SelectedItems[i].VariableName + "." + control.SelectedItems[i].Name + "] + ";
            }
        }

        placeHolder = placeHolder.slice(0, placeHolder.length - 3);

        EquationInsert(placeHolder);
    };

    categorySearch.OnCancel = function (control) {
        CloseBox("boxEquationDefinitionCategorySearchControl", "Top");
    };
}


var equationMethods;
function EquationInsertMethod() {
    InitDragBox("boxEquationInsertMethodControl", "Top");

    document.getElementById("pnlEquationInsertMethodResults").style.height = (ContentHeight - 300) + "px";

    AjaxRequest("SearchEquationMethods", "Expression=" + encodeURIComponent(""), function (response) {
        equationMethods = new Object();

        var methods = JSON.parse(response);

        var pnlResults = document.getElementById("pnlEquationInsertMethodResults");
        pnlResults.removeChild = "";
        var control = document.createElement("table");
        control.className = "EquationMethod";
        control.setAttribute("cellspacing", "0");
        control.setAttribute("cellpadding", "0");

        var html;

        html = "<tr><td></td>";

        html += "<td>" + LoadLanguageText("Name") + "</td>";
        html += "<td>" + LoadLanguageText("Description") + "</td>";
        html += "<td>" + LoadLanguageText("Parameters") + "</td>";

        html += "</tr><tr>";
        control.innerHTML += html;

        for (var i = 0; i < methods.length; i++) {

            equationMethods[methods[i].Name] = methods[i];

            html = "<tr>";

            html += "<td class=\"TableCellEquationMethodCheckbox\"><input type=\"radio\" name=\"chkEquationInsertMethod\" Method=\"" + methods[i].Name + "\" /></td>";
            html += "<td class=\"TableCellEquationMethodName\">" + methods[i].Name + "</td>";
            html += "<td class=\"TableCellEquationMethodDescription\">" + LoadLanguageText("EquationMethodDescription_" + methods[i].Name) + "</td>";
            html += "<td><table class=\"EquationMethodParameters\">";

            for (var p = 0; p < methods[i].Parameters.length; p++) {
                html += "<tr>";

                html += "<td>" + methods[i].Parameters[p].Name + "</td>";
                html += "<td>" + methods[i].Parameters[p].Type + "</td>";

                html += "</tr>";
            }

            html += "</table></td></tr>";

            control.innerHTML += html;
        }

        pnlResults.appendChild(control);

        InitBoxes();
    });
}

function DisableEquationMethodInsert(confirm) {
    CloseBox('boxEquationInsertMethodControl', 'Top');

    if (!confirm)
        return;

    var method = undefined;

    var boxes = document.getElementsByName("chkEquationInsertMethod");

    for (var i = 0; i < boxes.length; i++) {
        if (!boxes.item(i).checked)
            continue;

        method = equationMethods[boxes.item(i).getAttribute("Method")];
        break;
    }

    if (method == undefined)
        return;

    var methodStr = method.Name + "(";

    for (var i = 0; i < method.Parameters.length; i++) {
        methodStr += method.Parameters[i].Name + ", ";
    }

    if (method.Parameters.length != 0)
        methodStr = methodStr.slice(0, methodStr.length - 2);

    methodStr += ")";

    EquationInsert(methodStr);
}


function EquationInsertCondition() {
    EquationInsert("CONDITION ? TRUE : FALSE");
}

function EquationInsert(insert) {

    var line = window.editor.getCursor().line;

    var equation = window.editor.getLine(line);

    equation = equation.splice(window.editor.getCursor().ch, 0, insert);

    window.editor.setLine(line, equation);
}


function ValidateEquation() {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "ValidateEquation", "Equation=" + encodeURIComponent(window.editor.getValue()), function (response) {
        var errors = JSON.parse(response);
        if (window.editor.getValue() == "") {
            document.getElementById("pnlEquationDefinitionErrors").innerHTML = "";
        }
        if (errors.length == 0) {

            document.getElementById("pnlEquationDefinitionErrors").innerHTML = "<font color=\"#00AA00\">" +
                LoadLanguageText("EquationValidationNoErrorsFound") + "</font>";
            /*ShowJavascriptBox(
                "boxEquationErrors",
                LoadLanguageText("EquationValidationNoErrorsFound"),
                undefined,
                false,
                "EquationErrors EquationErrorsSuccess"
            );*/

            return;
        }

        var html = "<font color=\"#FF0000\">";

        for (var i = 0; i < errors.length; i++) {
            html += (i+1) + ": " + errors[i] + "<br />";
        }

        html += "</font>";
        if (window.editor.getValue() != "") {
            document.getElementById("pnlEquationDefinitionErrors").innerHTML = html;
        }
        /*ShowJavascriptBox(
            "boxEquationErrors",
            html,
            undefined,
            false,
            "EquationErrors EquationErrorsFailed"
        );*/
    });

    if (equationEditorActive)
        window.setTimeout(ValidateEquation, 5000);
}


function ExpandEquationEditor() {
    var container = window.editor.display.inputDiv.parentNode;

    if (container.style.position == "fixed") {
        container.style.position = "";
        container.style.left = "";
        container.style.width = "";
    }
    else {
        container.style.position = "fixed";
        container.style.left = "0px";
        container.style.width = "100%";
    }
}