var equationWizards = undefined;

function InitWizardSelector() {
    _AjaxRequest("/Handlers/EquationWizard.ashx", "GetEquationWizards", "", function (response) {
        equationWizards = JSON.parse(response);

        var container = document.createElement("div");
        var background = document.createElement("div");
        var pnlWizard = document.createElement("div");

        container.id = "pnlEquationWizardSelectionContainer";
        background.setAttribute("onclick",
            "var container = document.getElementById('pnlEquationWizardSelectionContainer');" +
            "var pnlEquationWizardSelection = document.getElementById('pnlEquationWizardSelection');" +
            "pnlEquationWizardSelection.style.transform = 'scale(0)';window.setTimeout(function() { " +
            "container.parentNode.removeChild(container); },300);"
        );

        background.className = "BoxBackground";
        pnlWizard.id = "pnlEquationWizardSelection";
        pnlWizard.className = "EquationWizardSelection";

        var sections = document.createElement("div");
        var pnlWizardSelection = document.createElement("div");
        pnlWizardSelection.id = "pnlWizardSelection";

        sections.className = "EquationWizardSelectionSections BackgroundColor1";
        pnlWizardSelection.className = "EquationWizardSelectionItems";

        var sectionSelector;
        for (var i = 0; i < equationWizards.Sections.length; i++) {
            sectionSelector = document.createElement("div");
            sectionSelector.className = "EquationWizardSelectionSection BackgroundColor3";
            sectionSelector.innerHTML = equationWizards.Sections[i].Name;
            sectionSelector.setAttribute("onclick",
                "LoadEquationWizards(equationWizards.Sections[" + i + "]);");
            sections.appendChild(sectionSelector);
        }

        pnlWizard.appendChild(sections);
        pnlWizard.appendChild(pnlWizardSelection);

        container.appendChild(background);
        container.appendChild(pnlWizard);

        document.body.appendChild(container);

        LoadEquationWizards(equationWizards.Sections[0]);

        window.setTimeout(function () {
            pnlWizard.style.transform = "scale(1)";
        }, 50);
    });
}

function LoadEquationWizards(section) {
    var pnlWizardSelection = document.getElementById("pnlWizardSelection");

    if (pnlWizardSelection == undefined)
        return;

    pnlWizardSelection.innerHTML = "";

    var pnlWizard;
    for (var i = 0; i < section.Wizards.length; i++) {
        pnlWizard = document.createElement("div");

        pnlWizard.innerHTML = "<table><tr><td rowspan=\"2\"><img src=\"/Images/Icons/EquationWizard/" +
            section.Wizards[i].Name + ".png\" /></td><td class=\"EquationWizardSelectionItemName\">" + section.Wizards[i].Name +
            "</td></tr><tr><td class=\"EquationWizardSelectionItemDescription\">" + section.Wizards[i].Description + "</td></tr></table>";

        pnlWizard.setAttribute("onclick", "InitWizard('"+ section.Wizards[i].Source +"');");

        pnlWizardSelection.appendChild(pnlWizard);
    }
}

function InitWizard(source) {
    _AjaxRequest("/Handlers/EquationWizard.ashx", "GetEquationWizard", "Source=" + source, function (response) {
        response = JSON.parse(response);

        var container = document.getElementById("pnlEquationWizardSelection");
        container.innerHTML = "";

        var table = document.createElement("table");
        table.className = "EquationWizardConfiguration";

        var tableRow;
        var tableCellName;
        var tableCellValue;
        for (var i = 0; i < response.PlaceHolders.length; i++) {
            tableRow = document.createElement("tr");
            tableCellName = document.createElement("td");
            tableCellValue = document.createElement("td");

            tableCellName.innerHTML = response.PlaceHolders[i].Name;

            switch (response.PlaceHolders[i].Type) {
                case "Variable":

                    EquationWizard_LoadVariables(tableCellValue);

                    break;
            }

            tableRow.appendChild(tableCellName);
            tableRow.appendChild(tableCellValue);
            table.appendChild(tableRow);
        }

        tableRow = document.createElement("tr");
        tableCellName = document.createElement("td");
        tableCellName.colSpan = 2;
        tableCellName.align = "right";

        tableCellName.innerHTML = "<input type=\"button\" value=\"confirm\" />"+
            "<input type=\"button\" value=\"cancel\" />";

        tableRow.appendChild(tableCellName);
        table.appendChild(tableRow);

        container.appendChild(table);

        $(".EquationWizardSelection select").select2({
            placeholder: "select...", allowClear: true
        });
    });
}

function EquationWizard_LoadVariables(container) {
    var variables = JSON.parse(RequestSynch(
        "/Handlers/LinkBiExternal.ashx",
        "GetVariables",
        ""
    ));

    var control = document.createElement("select");

    var option;
    for (var i = 0; i < variables.length; i++) {
        option = document.createElement("option");

        option.innerHTML = variables[i].Label;
        option.value = variables[i].Id;

        control.appendChild(option);
    }

    control.value = undefined;

    container.appendChild(control);
}