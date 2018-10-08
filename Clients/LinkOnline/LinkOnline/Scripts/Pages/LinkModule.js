var categories = [];
var variablenaame = "";
var taxonomyVariableMapping = [];
var idselectedTaxonomyvariable = "";
var existingMappings = [];

$(document).on("click", "#pnlMainTaxonomyVariableSelect .VariableSelectorVariableLabel", function () {
    idselectedTaxonomyvariable = $(this).parent().parent().parent().parent().parent().parent().attr("idvariable");
    variablenaame = $(this).text();
    categories = [];
    $("#taxonomyvariable").innerHTML = "";
    $.ajax({
        type: "POST",
        cache: false,
        async: false,
        url: "LinkingModule.aspx/GetTaxonomyCategories",
        data: JSON.stringify({ IdTaxonomyVariable: idselectedTaxonomyvariable }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            respData = JSON.parse(response.d);
            $('.categories,.variables,.nocategories').remove();
            $("#taxonomyVariableName").text(variablenaame);
            $("#taxonomyVariableName").attr("taxonomy_variable_id", idselectedTaxonomyvariable);

            $("#taxonomySelectionDiv").addClass("hidden");
            $("#LinkingDiv").removeClass("hidden");
            $("#taxonomyvariable").innerHTML = "";
            for (var i = 0 ; i < respData.length; i++) {
                categories.push(respData[i]);
                $("#taxonomyvariable").append("<tr class='categories' taxonomy_category_id='" + respData[i].id + "' data-category_name='" + respData[i].name + "' > <td> " + respData[i].name + "</td> </tr>");
            }
            if ($("#taxonomyvariable tr.nocategories").length == 0) {
                $("#taxonomyvariable").append("<tr class='nocategories'><td></td></tr>");
            }
        }
    });
    $.ajax({
        type: "POST",
        cache: false,
        async: false,
        url: "LinkingModule.aspx/getCategoryMappings",
        data: JSON.stringify({ idTaxonomyVariables: idselectedTaxonomyvariable }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (mapping) {
            taxonomyVariableMapping = JSON.parse(mapping.d);
        }
    });
    

});

$(document).on("click", ".BackButton", function () {
    $("#taxonomySelectionDiv").toggleClass("hidden");
    $("#LinkingDiv").toggleClass("hidden");
});

$(function () {
    searchVariables();
});

function LoadVariables(sender, idHierarchy) {
    var nodes = GetChildsByAttribute(document.getElementById("cphContent_pnlHierarchies"), "class", "TreeViewNode BackgroundColor1", true);

    for (var i = 0; i < nodes.length; i++) {
        nodes[i].className = "TreeViewNode BackgroundColor5";
    }

    sender.parentNode.className = "TreeViewNode BackgroundColor1";
    document.getElementById("lblHierarchyPath").innerHTML = sender.parentNode.getAttribute("Path");

    $("#selectedHierarchyId").val(idHierarchy);
   
    selectedHierarchy = {
        Id: idHierarchy,
        Name: sender.textContent,
        Path: sender.parentNode.getAttribute("Path")
    };

    ShowLoading(document.getElementById("pnlTaxonomyVariablesContainer"));

    _AjaxRequest("LinkingModule.aspx", "GetTaxonomyVariables", "IdHierarchy=" + idHierarchy, function (response) {
        var pnlTaxonomyVariables = document.getElementById("pnlMainTaxonomyVariableSelect");
        pnlTaxonomyVariables.innerHTML = response;
        var scripts = pnlTaxonomyVariables.getElementsByTagName("script");
        for (var i = 0; i < scripts.length; i++) {
            eval(scripts.item(i).innerHTML);
        }

        HideLoading();
    });
}

function searchTaxonomyVariables() {
    
    ShowLoading(document.getElementById("pnlTaxonomyVariablesContainer"));

    $.post("LinkingModule.aspx", { Method: "GetTaxonomyVariables", IdHierarchy: $("#selectedHierarchyId").val(), searchstring: $("#txtSearchTaxonomyVariables").val() })
    .done(function (response) {
        var pnlTaxonomyVariables = document.getElementById("pnlMainTaxonomyVariableSelect");
        pnlTaxonomyVariables.innerHTML = response;
        var scripts = pnlTaxonomyVariables.getElementsByTagName("script");
        for (var i = 0; i < scripts.length; i++) {
            eval(scripts.item(i).innerHTML);
        }

        HideLoading();
    });

}
function searchVariables() {

    var existingVariables = $('.variables');
    var variablesList = [];
    for (var j = 0; j < existingVariables.length; j++) {
        variablesList.push($(existingVariables).attr("id").replace('variable-', ''));
    }

    $.post("LinkingModule.aspx", { Method: "GetVariables", IdHierarchy: $("#selectedHierarchyId").val(), searchstring: $("#txtsearchVariables").val(), existingVariables: variablesList.join() })
   .done(function (response) {
        var pnlTaxonomyVariables = document.getElementById("pnlVariableSelect");
        pnlTaxonomyVariables.innerHTML = response;
        var scripts = pnlTaxonomyVariables.getElementsByTagName("script");
        for (var i = 0; i < scripts.length; i++) {
            eval(scripts.item(i).innerHTML);
        }
        $("#variableModal .VariableSelectorVariableLabel").prepend("<input type='checkbox' name='chk-variable' >");
        InitBoxes();
    });

}
$(document).on("keyup", "#txtsearchVariables", function ()  {   
    searchVariables();
});

function removeVariables(itemId, studyId) {

    $('#variable-' + itemId).remove();
    $("td[variable_id='" + itemId + "']").remove();
      
}



function AddVariableClick() {

    var selectedVariable = $("#variableModal .VariableSelectorVariableLabel input:checked");

    if (selectedVariable.length > 0) {

        for (var j = 0; j < selectedVariable.length; j++) {
                     

            var idvariable = ($(selectedVariable[j])).parent().parent().parent().parent().parent().parent().parent().attr("idvariable");;
            var variablenaame = ($(selectedVariable[j])).parent().text();

            var studyID = ($(selectedVariable[j])).parent().parent().parent().parent().parent().parent().parent().attr('id').replace('TaxonomyChapterVariables', '');

            var addbutton = $("#AddSymbol");
            $("#AddSymbol").remove();

            $("#taxonomyvariable .taxonomyVariableLabel").append("<td class='variables' id='variable-" + idvariable + "' >" + variablenaame + " &nbsp &nbsp <button type='button' class='close variableRemove' onclick='removeVariables(\"" + idvariable + "\",\"" + studyID + "\");' >&times;</button></td>");
            $("#taxonomyvariable .taxonomyVariableLabel").append(addbutton);
            $('#variableModal').modal('hide');

            $("#txtsearchVariables").val('');
            searchVariables();

            $(".categories").append("<td class='droppable' study-id='" + studyID + "'  variable_id='" + idvariable + "'></td>");
            $(".nocategories").append("<td study_id='" + studyID + "' variable_id='" + idvariable + "' ></td>");

            $(".droppable").droppable(
                    {
                        drop: function (event, ui) {                            
                            variabledroped(event, ui, $(this), $(this).parent().attr("taxonomy_category_id"));
                        }
                    }
                );

            $.ajax({
                type: "POST",
                cache: false,
                async: false,
                url: "LinkingModule.aspx/getCategoryLinks",
                data: JSON.stringify({ idVariables: idvariable, idTaxonomyVariables: idselectedTaxonomyvariable }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    respData = JSON.parse(response.d);

                    for (var i = 0; i < respData.length; i++) {
                        existingMappings.push(respData[i]);
                    }

                }
            });

            $.ajax({
                type: "POST",
                cache: false,
                async: false,
                url: "LinkingModule.aspx/GetCategories",
                data: JSON.stringify({ idVariable: idvariable }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    respData = JSON.parse(response.d);
                    for (var i = 0 ; i < respData.length; i++) {
                        processCategory(respData[i].id, respData[i].name, studyID, idvariable);
                    }
                }
            });
        }



    }
    else {
        console.log("no variables selected");
    }

}


function variabledroped(event, ui,$this,taxonomyCategoryId) {
    
    var variableId = $(ui.draggable).attr("variable_id");

    var categoryId = $(ui.draggable).attr("category_id");
    var variableText = $(ui.draggable).html();

    if ($(ui.draggable).hasClass('manualLink'))
    {
        CreateConfirmBox("Do you want to break the existing link ?", function () {
            UnlinkNotConfirmed(categoryId, variableId, taxonomyCategoryId, idselectedTaxonomyvariable, variableText);
            $(ui.draggable).remove();
        }
        );
    }
    else {
        $(ui.draggable).remove();
    }

    if ($(ui.draggable).hasClass('manualLink')) {
        $this.append("<div class='manualLink' category_id='" + categoryId + "' variable_id='" + variableId + "' >" + variableText + " </div>");
    }
    else
    {
        $this.append("<div class='manualLink' category_id='" + categoryId + "' variable_id='" + variableId + "' >" + variableText + "  <button type='button' class='close variableRemove' onclick='Unlink(\"" + categoryId + "\",\"" + variableId + "\",\"" + taxonomyCategoryId + "\",\"" + idselectedTaxonomyvariable + "\",\"" + variableText + "\")' > X </button> </div>");
    }

    
    var requestObject = {IdTaxonomyCategory: taxonomyCategoryId, IdCategory: categoryId, IdVariable: variableId, IdTaxonomyVariable: idselectedTaxonomyvariable };

    $.ajax({
        type: "POST",
        cache: false,
        async: false,
        url: "LinkingModule.aspx/Link",
        data: JSON.stringify(requestObject),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            draggableContent();
        }
    });


}

function processCategory(id, categoryName,studyId,variableId) {
    var matchingCategories = $(".categories[data-category_name^='" + categoryName + "']");
    
    filteredExistingMappings = existingMappings.filter(function (e) {
        if (e.IdVariable == variableId && e.IdCategory == id) {
            return true;
        }
        else {
            return false;
        }
    });

    if (filteredExistingMappings.length > 0) {
        $(".categories[taxonomy_category_id='" + (filteredExistingMappings[0]).IdTaxonomyCategory + "'] td[variable_id='" + variableId + "']").append("<div class='manualLink' category_id='" + id + "' variable_id='" + variableId + "' >" + categoryName + "  <button type='button' class='close variableRemove' onclick='Unlink(\"" + id + "\",\"" + variableId + "\",\"" + (filteredExistingMappings[0]).IdTaxonomyCategory + "\",\"" + idselectedTaxonomyvariable + "\",\"" + categoryName + "\")' > X </button> </div>");
       
    }
    else if (matchingCategories.length > 0) {
        $(".categories[data-category_name^='" + categoryName + "'] td[variable_id='" + variableId + "']").append("<div class='preferedLink' category_id='" + id + "' variable_id='" + variableId + "' >" + categoryName + "</div>")
    }
    else {
        $("tr.nocategories td[variable_id='" + variableId + "']").append("<div class='notlinked' category_id='" + id + "' variable_id='" + variableId + "' >" + categoryName + "</div>");
    }

    draggableContent();
}

function draggableContent() {
    $(".preferedLink,.notlinked").draggable(
    {
        revert: "invalid",
        drop: function (event, ui) {
            variabledroped(event, ui, $(this));
        }
    });
    $(".manualLink").draggable(
    {
        revert: "invalid",
        helper: "clone",
        drop: function (event, ui) {
            variabledroped(event, ui, $(this));
        }
    }
   );
}



$(document).on("click", "#AutoLink", function () {
    
    var prefferdLink = $(".preferedLink");
    
    for (var i = 0; i < prefferdLink.length; i++) {

        var variableId = $(prefferdLink[i]).attr("variable_id");
        var categoryId = $(prefferdLink[i]).attr("category_id");
        var TaxonomomyCategoryId = $(prefferdLink[i]).parent().parent().attr("taxonomy_category_id");
    
        var requestObject = {
            IdTaxonomyCategory: TaxonomomyCategoryId,
            IdCategory: categoryId,
            IdVariable: variableId,
            IdTaxonomyVariable: idselectedTaxonomyvariable 
        };
    
        $.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: "LinkingModule.aspx/Link",
            data: JSON.stringify(requestObject),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $(prefferdLink[i]).removeClass("preferedLink").addClass("manualLink");
            }
        });


    }
 
});

$(document).on("click", "#AddSymbol", function () {
   
    $("#variableModal").modal('show');
    $("#txtsearchVariables").val('');
    searchVariables();

   
});


$(document).on("click", ".unlinkbutton", function (e) {

    var CategoryId = $(this).parent().attr("category_id");
    var VariableId = $(this).parent().attr("variable_id");
    $(this).parent().parent().attr("taxonomy_category_id");


});


function Unlink(CategoryId, VariableId, TaxonomyCategoryId, TaxonomyVariableId,text) {
    CreateConfirmBox("Are you sure you want to unlink ?", function () {
        UnlinkNotConfirmed(CategoryId, VariableId, TaxonomyCategoryId, TaxonomyVariableId, text);
        $(".nocategories td[variable_id='" + VariableId + "']").append("<div class='notlinked' category_id='" + CategoryId + "' variable_id='" + VariableId + "' >" + text + " </div>");
        $(".manualLink[category_id='" + CategoryId + "'][variable_id='" + VariableId + "' ]").remove();
    });    
}

function UnlinkNotConfirmed(CategoryId, VariableId, TaxonomyCategoryId, TaxonomyVariableId, text) {

    var requestObject = {
        IdTaxonomyCategory: TaxonomyCategoryId,
        IdCategory: CategoryId,
        IdVariable: VariableId,
        IdTaxonomyVariable: TaxonomyVariableId
    };
    
    $.ajax({
        type: "POST",
        cache: false,
        async: false,
        url: "LinkingModule.aspx/UnLink",
        data: JSON.stringify(requestObject),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            draggableContent();
        }
    });

}