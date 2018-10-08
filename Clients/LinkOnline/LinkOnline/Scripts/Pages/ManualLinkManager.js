SelectionStack = [];
IdCategorySelectionStack = [];
IdTaxonomyCategorySelectionStack = [];

Categories="CATEGORIES";
TaxonomyCategories = "TAXONOMYCATEGORIES";
logUndo = true;

$(document).on("click", "#btn_variable_undo,#btn_Category_Undo", function () {

    CreateConfirmBox($("#hdnUndoMessage").val(), function () {  
        if (SelectionStack[SelectionStack.length - 1] == Categories) {
            if (IdCategorySelectionStack.length > 0) {
                UnselectCategories("cphContent_csStudyCategories", IdCategorySelectionStack.pop());
            }
        }
        if (SelectionStack[SelectionStack.length - 1] == TaxonomyCategories) {
            if (IdTaxonomyCategorySelectionStack.length > 0) {
                UnselectCategories("cphContent_csTaxonomyCategories", IdTaxonomyCategorySelectionStack.pop());                
            }
        }
       SelectionStack.pop();
       
    });

});



$(document).on("click", "td > img", function () {        
    if ($(this).siblings().attr("istaxonomy") == "false") {
        IdCategorySelectionStack.push($(this).siblings().attr("idcategory"));
    } else if ($(this).siblings().attr("istaxonomy") == "true") {
        IdTaxonomyCategorySelectionStack.push($(this).siblings().attr("idcategory"));
    }    
});

function UnselectCategories(ObjectName,categoryId) {
    logUndo = false;        
    document.getElementById(ObjectName).Select(categoryId);
    logUndo = true;
}

$(document).on("click", "#btn-Link", function (e) {
    if (validateStudyCategories() && validateTaxonomyCategories()) {
        var RequestObject = GetRequestObject();
        $.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: "ManualLinkManager.aspx/Link",
            data: JSON.stringify(RequestObject),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $("#p-LinkSuccessMessage").html("Link Process Successfully Completed!")

                var selectedCategories = document.getElementById("cphContent_csStudyCategories").SelectedItems;
                var selectedTaxonomyCategories = document.getElementById("cphContent_csTaxonomyCategories").SelectedItems;
                
                var StudyMessageListHtml = "";
                var taxonomyCategoryListHtml = "";

                for (i = 0; i < selectedCategories.length; i++) {
                    StudyMessageListHtml = StudyMessageListHtml + "<li>" + (selectedCategories[i]).Name.replace(/_/g, " ") + "</li>";
                }

                $("#ulCategoriesList").html(StudyMessageListHtml);

                
                for (j = 0; j < selectedTaxonomyCategories.length; j++) {
                    taxonomyCategoryListHtml = taxonomyCategoryListHtml + "<li>" + (selectedTaxonomyCategories[j]).Name.replace(/_/g, " ") + "</li>";
                }
                $("#ulTaxonomyCategoriesList").html(taxonomyCategoryListHtml);
                                
                $('#myModal').modal('show');
                               
            },
            failure: function (response) {

            }
        });
    }

});

$(document).on("click", "#Un_link_categories", function (e) {
    if (ValidateCategories()) {
        var RequestObject = GetRequestObject();

        $.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: "ManualLinkManager.aspx/UnLink",
            data: JSON.stringify(RequestObject),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var TaxonomyVariableName = GetDataArrayFromCheckBox($("input[type=radio][name=chk-taxonomy_variables]:checked"), 'taxonomy_variable_label');
                var TaxonomyCategoriesName = GetDataArrayFromCheckBox($("input[type=radio][name=chk-taxonomy_categories]:checked"), 'taxonomy_category_label');

                ArrIdsStudyVariables = GetDataArrayFromCheckBox($("input[type=checkbox][name=chk-study_variables]:checked"), 'study_variable_label');
                ArrIdsStudyCategories = GetDataArrayFromCheckBox($("input[type=checkbox][name=chk-study_categories]:checked"), 'category_label');

                var StudyVariablesListHtml = "";
                var StudyMessageListHtml = "";
                $("#p-LinkSuccessMessage").html("Un Link Process Successfully Completed!")

                for (i = 0; i < ArrIdsStudyVariables.length; i++) {
                    StudyVariablesListHtml = StudyVariablesListHtml + "<li>" + ArrIdsStudyVariables[i] + "</li>";
                }

                for (i = 0; i < ArrIdsStudyCategories.length; i++) {
                    StudyMessageListHtml = StudyMessageListHtml + "<li>" + ArrIdsStudyCategories[i] + "</li>";
                }

                $("#ulVariablesList").html(StudyVariablesListHtml);
                $("#ulCategoriesList").html(StudyMessageListHtml);

                $("#spn-TaxonomyVariableName").html(TaxonomyVariableName[0]);
                $("#spn-TaxonomyCategoryName").html(TaxonomyCategoriesName[0]);

                $('#myModal').modal('show');
            },
            failure: function (response) {

            }
        });

    }

});


$(document).on("click", "#categoriesBackButton", function (e) {
    CreateConfirmBox($("#hdnBackButtonMessage").val(), function () {
        $("#div-variables").removeClass("hidden-div");
        $("#div-categories").addClass("hidden-div");
    });

});


function GetRequestObject() {

    var selectedCategoryIds = [];
    var selectedTaxonomyCategoryIds = [];

    var RequestObject = new Object();

    var selectedCategories = document.getElementById("cphContent_csStudyCategories").SelectedItems;
    var selectedTaxonomyCategories = document.getElementById("cphContent_csTaxonomyCategories").SelectedItems;
    
    for (var i = 0; i < selectedCategories.length; i++) {
        selectedCategoryIds.push(selectedCategories[i].Id);
    }

    for (var i = 0; i < selectedTaxonomyCategories.length; i++) {
        selectedTaxonomyCategoryIds.push(selectedTaxonomyCategories[i].Id);
    }

    RequestObject.IdsStudyCategories = selectedCategoryIds.join();
    RequestObject.IdsTaxonomyCategories = selectedTaxonomyCategoryIds.join();

    return RequestObject;
}


function validateStudyCategories() {
    var RequestObject = GetRequestObject();

    if (RequestObject.IdsStudyCategories == "") {
        $("#divStudyCategoriesMandatoryMessage").removeClass("hidden-div");
        $("#DivValidationMessage").removeClass("hidden-div");
        return false;
    }
    else {
        $("#divStudyCategoriesMandatoryMessage").addClass("hidden-div");
        $("#DivValidationMessage").addClass("hidden-div");
    }
    return true;

}

function validateTaxonomyCategories() {
    var RequestObject = GetRequestObject();

    if (RequestObject.IdsTaxonomyCategories == "") {
        $("#divTaxonomyCategoriesMandatoryMessage").removeClass("hidden-div");
        $("#DivValidationMessage").removeClass("hidden-div");
        return false;
    }
    else {
        $("#divTaxonomyCategoriesMandatoryMessage").addClass("hidden-div");
        $("#DivValidationMessage").addClass("hidden-div");
    }

    return true;
}


$(document).ready(function () {
    $("#btn_variable_undo,#btn_Category_Undo").css("background-color", "#FEDE31");
    $("#btn-Link,#btn_variable_link").css("background-color", "#61CF71");
    $("#Un_link_variables,#Un_link_categories").css("background-color", "#FF8251");
    BindLoaderEvents();
});

function BindLoaderEvents() {

    document.getElementById("cphContent_csStudyCategories").SelectionChanged = function () {
        if (logUndo == true) {
            SelectionStack.push(Categories);
        }
    }
    document.getElementById("cphContent_csTaxonomyCategories").SelectionChanged = function () {
        if (logUndo == true) {
            SelectionStack.push(TaxonomyCategories);
        }
    }
}
function RefreshPage() {
    var url = window.location.href = window.location.href.split('?')[0];
    window.location.href = url;
}
