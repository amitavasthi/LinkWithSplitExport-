function SetStudyValue(xPath, index, value, refreshPage) {
    if (value == undefined)
        return;

    if (refreshPage == true) {
        ShowLoading(document.getElementById("Content"));
    }

    _AjaxRequest("/Handlers/StudyTaxonomyStructure.ashx", "SetStudyValue", "XPath=" + xPath + "&Index=" + index + "&Value=" + value, function (response) {
        if (refreshPage == true)
            window.location = window.location;
    });
}

function LoadCategories(idDdlCategories, idVariable) {
    _AjaxRequest("/Handlers/StudyTaxonomyStructure.ashx", "LoadCategories", "IdVariable=" + idVariable, function (response) {
        var ddlCategories = document.getElementById(idDdlCategories);

        if (ddlCategories == undefined)
            return;

        ddlCategories.innerHTML = response;
    });
}