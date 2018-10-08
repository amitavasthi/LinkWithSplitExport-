<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="AutoLinking.aspx.cs" Inherits="LinkOnline.Pages.LinkManager.AutoLinking" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>
<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js"></script>  
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">  
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
    <link href="../../Stylesheets/Pages/LinkModule.css" rel="stylesheet" />
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/Filters.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/VariableSelector.js"></wu:ScriptReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Modules/VariableSearch.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Pages/material-design-iconic-font.min.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSearch.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSelector2.css"></wu:StylesheetReference>    
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSelector.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Main.css"></wu:StylesheetReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/ColorSchemeTemp.css"></wu:StylesheetReference>

    <style type="text/css">
        .TaxonomyChapterTitle {
            border-top: 1px solid #FFFFFF;
            background: #444444;
            cursor: pointer;
        }

        .TaxonomyChapterTitleLabel {
            padding: 1em;
            font-weight: bold;
            color: #FFFFFF;
            font-size: 16pt;
        }

        .TaxonomyChapterVariables {
            overflow: hidden;
        }

        .TreeViewNodeLabel {
            cursor: pointer;
        }

        .LabelHierarchyPath {
            font-size: 18pt;
        }

        #boxVariableSearchControl .BtnBoxClose {
            display: none;
        }

        .ButtonLinking {
            height: 40px;
            width: auto;
            border: 1px solid #41719C;
            font-size: 11pt;
        }
    </style>
    <script type="text/javascript">

        var categories = [];
        var variableName = "";
        var taxonomyVariableMapping = [];
        var selectdTaxonomyVariableId = "";
        var existingMappings = [];
        var result = [];
        var multipleId = [];
        function HideTaxonomyChapter(sender, idTaxonomyChapter) {
            var control = document.getElementById("TaxonomyChapterVariables" + idTaxonomyChapter);

            control["Height"] = control.offsetHeight;

            DecreaseHeight(control, 0);

            sender.setAttribute("onclick", "ShowTaxonomyChapter(this, '" + idTaxonomyChapter + "')");
        }

        function ShowTaxonomyChapter(sender, idTaxonomyChapter) {
            var control = document.getElementById("TaxonomyChapterVariables" + idTaxonomyChapter);

            IncreaseHeight(control, control["Height"]);

            sender.setAttribute("onclick", "HideTaxonomyChapter(this, '" + idTaxonomyChapter + "')");
        }

        loadFunctions.push(function () {
            document.getElementById("pnlTaxonomyVariablesContainer").style.height = (ContentHeight - 66) + "px";
            document.getElementById("cphContent_pnlHierarchies").style.height = (ContentHeight - 66) + "px";
        });

        /*The below method is used for Laod the Taxonomy Variable First Time*/
        var selectedHierarchy = undefined;
        function LoadVariables(sender, idHierarchy) {
            var nodes = GetChildsByAttribute(document.getElementById("cphContent_pnlHierarchies"), "class", "TreeViewNode BackgroundColor1", true);

            for (var i = 0; i < nodes.length; i++) {
                nodes[i].className = "TreeViewNode BackgroundColor5";
            }

            sender.parentNode.className = "TreeViewNode BackgroundColor1";
            document.getElementById("lblHierarchyPath").innerHTML = sender.parentNode.getAttribute("Path");

            selectedHierarchy = {
                Id: idHierarchy,
                Name: sender.textContent,
                Path: sender.parentNode.getAttribute("Path")
            };

            ShowLoading(document.getElementById("pnlTaxonomyVariablesContainer"));

            _AjaxRequest("AutoLinking.aspx", "GetTaxonomyVariables", "IdHierarchy=" + idHierarchy, function (response) {
                var pnlTaxonomyVariables = document.getElementById("pnlTaxonomyVariables");

                pnlTaxonomyVariables.innerHTML = response;

                var scripts = pnlTaxonomyVariables.getElementsByTagName("script");

                for (var i = 0; i < scripts.length; i++) {
                    eval(scripts.item(i).innerHTML);
                }

                HideLoading();
            });
        }

        /*The below method is used for Search Taxonomy Variable*/

        function searchTaxonomyVariables() {
            ShowLoading(document.getElementById("pnlTaxonomyVariablesContainer"));
            $.post("AutoLinking.aspx", { Method: "GetTaxonomyVariablesSearch", IdHierarchy: $("#cphContent_selectedHierarchyId").val(), variableName: $("#txtSearchTaxonomyVariables").val() })
            .done(function (response) {
                var pnlTaxonomyVariables = document.getElementById("pnlTaxonomyVariables");
                pnlTaxonomyVariables.innerHTML = response;
                var scripts = pnlTaxonomyVariables.getElementsByTagName("script");
                for (var i = 0; i < scripts.length; i++) {
                    eval(scripts.item(i).innerHTML);
                }
                HideLoading();
            });
        }

        /*The below method is used for to get the Taxonomy Variable details when select a particular variable*/

        $(document).on("click", "#pnlTaxonomyVariables .VariableSelectorVariableLabel", function () {
            selectdTaxonomyVariableId = $(this).parent().parent().parent().parent().parent().parent().attr("idvariable");

            variableName = $(this).text();
            categories = [];
            $("#taxonomyvariable").innerHTML = "";
            var parameters = "selectdTaxonomyVariableId=" + selectdTaxonomyVariableId;
            var service = "/Handlers/AutoLink.ashx";
            _AjaxRequest(service, "GetTaxonomyCategories", parameters, function (response) {
                respData = JSON.parse(response);

                $('.categories,.variables,.nocategories').remove();
                $("#taxonomyVariableName").text(variableName);
                $("#taxonomyVariableName").attr("taxonomy_variable_id", selectdTaxonomyVariableId);

                $("#divTaxonomySelector").addClass("hidden");
                $("#divAutoLink").removeClass("hidden");
                $("#taxonomyvariable").innerHTML = "";
                for (var i = 0 ; i < respData.length; i++) {
                    categories.push(respData[i]);
                    $("#taxonomyvariable").append("<tr class='categories' taxonomy_category_id='" + respData[i].id + "' data-category_name='" + respData[i].name + "' > <td> " + respData[i].name + "</td> </tr>");
                }
                if ($("#taxonomyvariable tr.nocategories").length == 0) {
                    $("#taxonomyvariable").append("<tr class='nocategories'><td><b>" + LoadLanguageText('NonMatchingtaxonomyCategoryLabel').replace('{0}', name) + "</b></td></tr>");
                }
            });
            _AjaxRequest(service, "GetCategoryMappings", parameters, function (mappingData) {
                taxonomyVariableMapping = JSON.parse(mappingData);
            });
        });

        /*The below code used for Go Back to the Taxonomy Variable Selection View*/
        $(document).on("click", ".BackButton", function () {
            taxonomyVariableMapping = [];
            existingMappings = [];
            $("#divTaxonomySelector").toggleClass("hidden");
            $("#divAutoLink").toggleClass("hidden");
        });
        /*The below code is used for showing the Study Variable List*/
        $(document).on("click", "#AddSymbol", function () {
            // $("#variableModal").modal('show');
            //$("#cphContent_txtVariableSearch").val('');
            SearchVariables();
        });
        /* Code for search the study variable details*/
        function SearchVariables() {
            ShowLoading(document.getElementById("variableSearchResults"));
            // var existingVariables = $(".variables");

            /* The below code uses for getting all the varibale ids*/
            var existingVariables = $(".variables").map(function () {
                return this.id.replace('variable-', '');
            }).get();

            var variablesList = [];
            for (var j = 0; j < existingVariables.length; j++) {
                //variablesList.push("'" + $(existingVariables)[j] + "'");
                variablesList.push($(existingVariables)[j]);
            }

            // $.post("AutoLinking.aspx", { Method: "GetStudyVariables", IdHierarchy: $("#cphContent_selectedHierarchyId").val(), searchstring: $("#cphContent_txtVariableSearch").val(), existingVariables: variablesList.join() })
            $.post("/Handlers/AutoLink.ashx", { Method: "BindStudyVariables", IdHierarchy: $("#cphContent_selectedHierarchyId").val(), searchstring: $("#cphContent_txtVariableSearch").val(), existingVariables: variablesList.join() })
                       .done(function (response) {
                           var pnlTaxonomyVariables = document.getElementById("variableSearchResults");
                           pnlTaxonomyVariables.innerHTML = response;
                           var scripts = pnlTaxonomyVariables.getElementsByTagName("script");
                           for (var i = 0; i < scripts.length; i++) {
                               eval(scripts.item(i).innerHTML);
                           }
                           // $("#boxVariableSearch .VariableSelectorVariableLabel").prepend("<input type='checkbox' name='chk-variable' >");
                           InitBoxes();
                           HideLoading();
                       });
        }


        /*Creating the study variable details*/
        function AddVariableClick() {
            // var selectedVariable = $("#boxVariableSearch .VariableSelectorVariableLabel input:checked");
            var selectedVariable = $("#boxVariableSearch .VariableSelectorCheckBox input:checked");
            if (selectedVariable.length > 0) {

                for (var j = 0; j < selectedVariable.length; j++) {

                    var idVariable = ($(selectedVariable[j])).parent().parent().parent().parent().parent().parent().parent().attr("idvariable");

                    var variableName = ($(selectedVariable[j])).parent().attr("name");

                    var studyID = ($(selectedVariable[j])).parent().parent().parent().parent().parent().parent().parent().parent().attr('id').replace('TaxonomyChapterVariables', '');
                    var studyName = ($(selectedVariable[j])).parent().parent().parent().parent().parent().parent().parent().parent().parent().attr('name');

                    var isLinkedVariable = ($(selectedVariable[j])).parent().attr("id").split('_')[0];

                    var addbutton = $("#AddSymbol");
                    $("#AddSymbol").remove();

                    if (isLinkedVariable === "linked") {
                        $("#taxonomyvariable .taxonomyVariableLabel").append("<td class='variables' bgcolor='#61CF71' id='variable-" + idVariable + "' ><b><span style='font-size:10'>" + studyName + "</span></b> &nbsp; &nbsp; <button type='button' class='close variableRemove' onclick='removeVariables(\"" + idVariable + "\",\"" + studyID + "\");' >&times;</button><br/><b>" + variableName + "</b>&nbsp; &nbsp; <button type='button' class='close variableRemove' onclick='removeVariableLink(\"" + idVariable + "\",\"" + selectdTaxonomyVariableId + "\");' >unlink</button><br/></td>");
                    }
                    else {
                        $("#taxonomyvariable .taxonomyVariableLabel").append("<td class='variables' id='variable-" + idVariable + "' ><b><span style='font-size:10'>" + studyName + "</span></b> &nbsp; &nbsp; <button type='button' class='close variableRemove' onclick='removeVariables(\"" + idVariable + "\",\"" + studyID + "\");' >&times;</button><br/><b>" + variableName + "</b><br/></td>");
                    }
                    $("#taxonomyvariable .taxonomyVariableLabel").append(addbutton);


                    $(".categories").append("<td class='droppable' study-id='" + studyID + "'  variable_id='" + idVariable + "'></td>");
                    $(".nocategories").append("<td class='droppable' study_id='" + studyID + "' variable_id='" + idVariable + "' ></td>");

                    $(".droppable").droppable(
                            {
                                drop: function (event, ui) {
                                    variabledroped(event, ui, $(this), $(this).parent().attr("taxonomy_category_id"));
                                }
                            }
                        );

                    var parameters = "";
                    parameters += "idVariable=" + idVariable + "&";
                    parameters += "selectdTaxonomyVariableId=" + selectdTaxonomyVariableId + "&";
                    parameters += "idStudy=" + studyID;

                    _AjaxRequest("/Handlers/AutoLink.ashx", "GetCategoryLinks", parameters, function (cLResponse) {
                        clData = JSON.parse(cLResponse);
                        for (var i = 0; i < clData.length; i++) {
                            existingMappings.push(clData[i]);
                        }
                    });

                    _AjaxRequest("/Handlers/AutoLink.ashx", "GetCategories", parameters, function (response) {
                        respData = JSON.parse(response);
                        for (var i = 0; i < respData.length; i++) {
                            processCategory(respData[i].id, respData[i].name, respData[i].idStudy, respData[i].idVariable, respData[i].exist, respData[i].IdTaxonomyCategory, respData[i].savedNotes);
                        }

                    });

                }
            }
            else {
                console.log("no variables selected");
            }
        }
        function processCategory(id, categoryName, studyId, variableId, categoryLinkExists, IdTaxonomyCategory, savedNotes) {
            var matchingCategories = $(".categories[data-category_name^='" + categoryName + "']");

            if (categoryLinkExists) {
                if (savedNotes != "") {
                    $(".categories[taxonomy_category_id='" + IdTaxonomyCategory + "'] td[variable_id='" + variableId + "']").append("<div class='notesLink' id='category-" + id + "' taxonomy_category_id='" + IdTaxonomyCategory + "' category_id='" + id +
                        "' variable_id='" + variableId + "'oncontextmenu='ShowNoteMenu(this,\"" + variableId + "\",\"" + id + "\",\"" + IdTaxonomyCategory + "\");return false;'>" + categoryName + "  <button type='button' class='close variableRemove' onclick='Unlink(\"" + id + "\",\"" + variableId + "\",\"" +
                        IdTaxonomyCategory + "\",\"" + selectdTaxonomyVariableId + "\",\"" + categoryName + "\")' > X </button></div><div class='addNotes ui-draggable ui-draggable-handle' id='addNotes" + id + "-" + IdTaxonomyCategory + "' style='display:none;'><textarea id='txt" + id + "-" + IdTaxonomyCategory + "'>" + savedNotes + "</textarea><button type='button' class='closeButton' onclick='ClearNotes(\"" + id + "\",\"" + variableId + "\",\"" +
                        IdTaxonomyCategory + "\",\"" + selectdTaxonomyVariableId + "\",\"" + categoryName + "\")' ></button><button type='button' class='acceptButton' onclick='AddNotes(\"" + id + "\",\"" + variableId + "\",\"" +
                        IdTaxonomyCategory + "\",\"" + selectdTaxonomyVariableId + "\",\"" + categoryName + "\")' ></button></div>");

                    draggableContent();
                }
                else {
                    $(".categories[taxonomy_category_id='" + IdTaxonomyCategory + "'] td[variable_id='" + variableId + "']").append("<div class='manualLink' id='category-" + id + "' taxonomy_category_id='" + IdTaxonomyCategory + "' category_id='" + id +
                        "' variable_id='" + variableId + "'oncontextmenu='ShowNoteMenu(this,\"" + variableId + "\",\"" + id + "\",\"" + IdTaxonomyCategory + "\");return false;'>" + categoryName + "  <button type='button' class='close variableRemove' onclick='Unlink(\"" + id + "\",\"" + variableId + "\",\"" +
                        IdTaxonomyCategory + "\",\"" + selectdTaxonomyVariableId + "\",\"" + categoryName + "\")' > X </button></div><div class='addNotes ui-draggable ui-draggable-handle' id='addNotes" + id + "-" + IdTaxonomyCategory + "' style='display:none;'><textarea id='txt" + id + "-" + IdTaxonomyCategory + "'>" + savedNotes + "</textarea><button type='button' class='closeButton' onclick='ClearNotes(\"" + id + "\",\"" + variableId + "\",\"" +
                        IdTaxonomyCategory + "\",\"" + selectdTaxonomyVariableId + "\",\"" + categoryName + "\")' ></button><button type='button' class='acceptButton' onclick='AddNotes(\"" + id + "\",\"" + variableId + "\",\"" +
                        IdTaxonomyCategory + "\",\"" + selectdTaxonomyVariableId + "\",\"" + categoryName + "\")' ></button></div>");

                    draggableContent();
                }
            }
            else if (matchingCategories.length > 0) {
                $(".categories[data-category_name^='" + categoryName + "'] td[variable_id='" + variableId + "']").append("<div class='preferedLink' id='category-" + id + "' taxonomy_category_id='" + IdTaxonomyCategory + "' category_id='" + id + "' variable_id='" + variableId + "' >" + categoryName + "<button type='button' class='close variableHide' onclick='$(this).parent().remove();' > X </button></div>")//event, ui RemoveVariable(\"" + id + "\",\"" + variableId + "\")
                draggableContent();
            }
            else {
                $("tr.nocategories td[variable_id='" + variableId + "']").append("<div class='notlinked' id='category-" + id + "' taxonomy_category_id='" + IdTaxonomyCategory + "' category_id='" + id + "' variable_id='" + variableId + "' >" + categoryName + "<button type='button' class='close variableHide' onclick='$(this).parent().remove();' > X </button></div>");
                draggableContent();
            }

            //draggableContent();
        }

        function draggableContent() {
            $(".preferedLink").draggable(
            {
                revert: "invalid",
                helper: "clone",
                delay: 300,
                opacity: 0.65,
                axis: "y",
                cursor: "move",
                drop: function (event, ui) {
                    variabledroped(event, ui, $(this));
                }
            });
            $(".manualLink").draggable(
            {
                revert: "invalid",
                delay: 300,
                opacity: 0.65,
                axis: "y",
                cursor: "move",
                helper: "clone",
                drop: function (event, ui) {
                    variabledroped(event, ui, $(this));
                }
            });
            $(".notesLink").draggable(
           {
               revert: "invalid",
               helper: "clone",
               opacity: 0.65,
               delay: 300,
               axis: "y",
               cursor: "move",
               drop: function (event, ui) {
                   variabledroped(event, ui, $(this));
               }
           });
            $(".notlinked").draggable(
              {
                  revert: "invalid",
                  helper: "clone",
                  opacity: 0.65,
                  delay: 300,
                  axis: "y",
                  cursor: "move",
                  drop: function (event, ui) {
                      variabledroped(event, ui, $(this));
                  }
              });
        }
        function removeVariables(itemId, studyId) {
            $('#variable-' + itemId).remove();
            $("td[variable_id='" + itemId + "']").remove();
        }

        /* For removing a variable Link*/

        function removeVariableLink(variableId, selectdTaxonomyVariableId) {

            CreateConfirmBox(LoadLanguageText('UnlinkVariable').replace('{0}', name), function () {
                UnlinkVariables(variableId, selectdTaxonomyVariableId);
            });

            //$('#variable-' + variableId).remove();
            //$("td[variable_id='" + variableId + "']").remove();
        }

        /*Drop the Variable*/
        function variabledroped(event, ui, $this, taxonomyCategoryId) {
            var variableId = $(ui.draggable).attr("variable_id");

            var categoryId = $(ui.draggable).attr("category_id");
            var idtaxonomyCategory = $(ui.draggable).attr("taxonomy_category_id");
            var variableText = $(ui.draggable).html();

            variableCategoryText = $(ui.draggable).text().replace('X', "").trim();

            /* Adding the linked varibale to the right container*/
            if ($(ui.draggable).hasClass('manualLink')) {
                $this.append("<div class='preferedLink' id='category-" + categoryId + "'taxonomy_category_id='" + taxonomyCategoryId + "' category_id='" + categoryId + "' variable_id='" + variableId + "' >" + variableText + "</div>");
                $(".preferedLink[category_id='" + categoryId + "'][taxonomy_category_id='" + taxonomyCategoryId + "'][variable_id='" + variableId + "' ]").find("button").remove();
                $(".preferedLink[category_id='" + categoryId + "'][taxonomy_category_id='" + taxonomyCategoryId + "'][variable_id='" + variableId + "' ]").append("<button type='button' class='close variableHide' onclick='$(this).parent().remove();' > X </button></div>").prop("oncontextmenu", null);
            }
            else if ($(ui.draggable).hasClass('notesLink')) {
                /* Adding the linked varibale to the right container*/
                $this.append("<div class='preferedLink' id='category-" + categoryId + "'taxonomy_category_id='" + taxonomyCategoryId + "' category_id='" + categoryId + "' variable_id='" + variableId + "' >" + variableText + "</div>");
                $(".preferedLink[category_id='" + categoryId + "'][taxonomy_category_id='" + taxonomyCategoryId + "'][variable_id='" + variableId + "' ]").find("button").remove();
                $(".preferedLink[category_id='" + categoryId + "'][taxonomy_category_id='" + taxonomyCategoryId + "'][variable_id='" + variableId + "' ]").append("<button type='button' class='close variableHide' onclick='$(this).parent().remove();' > X </button></div>").prop("oncontextmenu", null);

            }
            else if ($(ui.draggable).hasClass('preferedLink')) {

                $this.append("<div class='preferedLink' id='category-" + categoryId + "'taxonomy_category_id='" + taxonomyCategoryId + "' category_id='" + categoryId + "' variable_id='" + variableId + "' >" + variableText + "</div>");
                draggableContent();

            }
            else if ($(ui.draggable).hasClass('notlinked')) {
                $this.append("<div class='preferedLink' id='category-" + categoryId + "'taxonomy_category_id='" + taxonomyCategoryId + "' category_id='" + categoryId + "' variable_id='" + variableId + "' >" + variableText + "</div>");
                draggableContent();

                //$(ui.draggable).remove();
            }
        }

        function RemoveVariable(categoryId, variableId) {
            $('#category-' + categoryId).remove();
        }

        function UnlinkVariables(VariableId, TaxonomyVariableId) {

            var parameters = "";
            parameters += "IdVariable=" + VariableId + "&";
            parameters += "IdTaxonomyVariable=" + TaxonomyVariableId;

            _AjaxRequest("/Handlers/AutoLink.ashx", "UnlinkVariables", parameters, function (response) {
                $('#variable-' + VariableId).remove();
                $("td[variable_id='" + VariableId + "']").remove();
            });
        }


        function UnlinkNotConfirmed(CategoryId, VariableId, TaxonomyCategoryId, TaxonomyVariableId, text) {

            $("#addNotes" + CategoryId).remove();

            var parameters = "";
            parameters += "IdTaxonomyCategory=" + TaxonomyCategoryId + "&";
            parameters += "IdCategory=" + CategoryId + "&";
            parameters += "IdVariable=" + VariableId + "&";
            parameters += "IdTaxonomyVariable=" + TaxonomyVariableId + "&";
            parameters += "Text=" + text;

            _AjaxRequest("/Handlers/AutoLink.ashx", "UnLink", parameters, function (response) {
                draggableContent();
            });
        }
        function Unlink(CategoryId, VariableId, TaxonomyCategoryId, TaxonomyVariableId, text) {
            CreateConfirmBox(LoadLanguageText('UnlinkVariable').replace('{0}', name), function () {
                UnlinkNotConfirmed(CategoryId, VariableId, TaxonomyCategoryId, TaxonomyVariableId, text);
                //$(".nocategories td[variable_id='" + VariableId + "']").append("<div class='notlinked' category_id='" + CategoryId + "' variable_id='" + VariableId + "' >" + text + " </div>");
                /*The below line is used to remove all the capabilities of the addnotes and the class*/
                $(".manualLink[category_id='" + CategoryId + "'][taxonomy_category_id='" + TaxonomyCategoryId + "'][variable_id='" + VariableId + "' ]").find("button").remove();
                $(".manualLink[category_id='" + CategoryId + "'][taxonomy_category_id='" + TaxonomyCategoryId + "'][variable_id='" + VariableId + "' ]").append("<button type='button' class='close variableHide' onclick='$(this).parent().remove();' > X </button></div>").removeClass("manualLink").addClass("preferedLink").prop("oncontextmenu", null);

                $(".notesLink[category_id='" + CategoryId + "'][taxonomy_category_id='" + TaxonomyCategoryId + "'][variable_id='" + VariableId + "' ]").find("button").remove();
                $(".notesLink[category_id='" + CategoryId + "'][taxonomy_category_id='" + TaxonomyCategoryId + "'][variable_id='" + VariableId + "' ]").append("<button type='button' class='close variableHide' onclick='$(this).parent().remove();' > X </button></div>").removeClass("manualLink").addClass("preferedLink").prop("oncontextmenu", null);

                //$(".notesLink[category_id='" + CategoryId + "'][taxonomy_category_id='" + TaxonomyCategoryId + "'][variable_id='" + VariableId + "' ]").removeClass("notesLink").addClass("preferedLink").prop("oncontextmenu", null).find("button").remove();
            });
        }

        $(document).on("click", "#AutoLink", function () {
            var categoryCount = $(".categories").length;
            if (categoryCount > 0) {
                var pCnt = 0;
                var prefferdLink = $(".preferedLink");
                var calls = [];
                /*This is for Linking Variables and Categories*/
                for (var i = 0; i < prefferdLink.length; i++) {
                    var variableId = $(prefferdLink[i]).attr("variable_id");
                    var categoryId = $(prefferdLink[i]).attr("category_id");
                    var TaxonomomyCategoryId = $(prefferdLink[i]).parent().parent().attr("taxonomy_category_id");
                    var variableText = $(prefferdLink[i]).text();

                    var parameters = "";
                    parameters += "IdTaxonomyCategory=" + TaxonomomyCategoryId + "&";
                    parameters += "IdCategory=" + categoryId + "&";
                    parameters += "IdVariable=" + variableId + "&";
                    parameters += "IdTaxonomyVariable=" + selectdTaxonomyVariableId;

                    _AjaxRequest("/Handlers/AutoLink.ashx", "Link", parameters, function (response) {
                    });

                    pCnt++;
                    // $(prefferdLink[i]).removeClass("preferedLink ui-draggable ui-draggable-handle").addClass("manualLink ui-draggable ui-draggable-handle").attr("oncontextmenu", "ShowNoteMenu(this,\"" + variableId + "\",\"" + categoryId + "\");return false;");
                    $(prefferdLink[i]).removeClass("preferedLink ui-draggable ui-draggable-handle").addClass("manualLink ui-draggable ui-draggable-handle");
                    $(prefferdLink[i]).append("<button type='button' class='close variableRemove' onclick='Unlink(\"" + categoryId + "\",\"" + variableId + "\",\"" +
                    TaxonomomyCategoryId + "\",\"" + selectdTaxonomyVariableId + "\",\"" + variableText + "\")' > X </button>");
                    //$(prefferdLink[i]).append("<div class='addNotes' id='addNotes" + categoryId + "' style='display:none;'><textarea id='txt" + categoryId + "'></textarea><button type='button' class='closeButton' onclick='ClearNotes(\"" + categoryId + "\",\"" +
                    //        variableId + "\")' ></button><button type='button' class='acceptButton' onclick='AddNotes(\"" + categoryId + "\",\"" + variableId + "\")' ></button></div>");

                }
                if (pCnt == prefferdLink.length) {
                    setTimeout(function () { window.location = "AutoLinking.aspx" }, 12000);
                }
            }
            else {
                /*This is for Linking Non Numeric & Text Variables*/
                var variablestoLink = $(".variables");
                var vCnt = variablestoLink.length
                for (var i = 0; i < variablestoLink.length; i++) {
                    var variableId = $(variablestoLink[i]).attr("id").replace('variable-', '');
                    var taxonomyVariableId = $("#taxonomyVariableName").attr("taxonomy_variable_id");

                    var parameters = "";
                    parameters += "IdVariable=" + variableId + "&";
                    parameters += "IdTaxonomyVariable=" + selectdTaxonomyVariableId;

                    _AjaxRequest("/Handlers/AutoLink.ashx", "LinkNumericOrMulti", parameters, function (response) {
                        //Do something here.
                    });
                }
                window.location = "AutoLinking.aspx";
            }
            //$.when.apply($, calls).done(function () {
            //    window.location = "LinkManager.aspx";
            //});
        });

        /* The below code for genereate add note context menu*/

        function ShowNoteMenu(sender, variableId, categoryId, taxonomyCategoryId) {

            var menu = InitMenu("menuVariable" + sender.id);
            var lnkAddNote = document.createElement("div");
            lnkAddNote.ImageUrl = "/Images/Icons/Menu/Rename.png";
            lnkAddNote.innerHTML = LoadLanguageText("AddNote");
            lnkAddNote.MenuItemClick = "ShowTextBox('" + variableId + "', '" + categoryId + "','" + taxonomyCategoryId + "');";
            menu.Items.push(lnkAddNote);
            menu.Render();
        }

        function ShowTextBox(variableId, categoryId, taxonomyCategoryId) {
            $("#addNotes" + categoryId + "-" + taxonomyCategoryId).css("display", "block");
        }
        /*Ends the code used for handle the textarea*/
        /*The below code used to call the AddNotes Method*/
        function AddNotes(CategoryId, VariableId, TaxonomyCategoryId, TaxonomyVariableId, text) {
            var notes = $("#txt" + CategoryId + "-" + TaxonomyCategoryId).val();
            if ($("#txt" + CategoryId + "-" + TaxonomyCategoryId).val() != "") {
                var parameters = "";
                parameters += "IdTaxonomyCategory=" + TaxonomyCategoryId + "&";
                parameters += "IdCategory=" + CategoryId + "&";
                parameters += "IdVariable=" + VariableId + "&";
                parameters += "IdTaxonomyVariable=" + TaxonomyVariableId + "&";
                parameters += "Notes=" + $("#txt" + CategoryId + "-" + TaxonomyCategoryId).val();

                _AjaxRequest("/Handlers/AutoLink.ashx", "AddNotes", parameters, function (response) {
                    $("#addNotes" + CategoryId + "-" + TaxonomyCategoryId).val = notes;
                    $("#addNotes" + CategoryId + "-" + TaxonomyCategoryId).css("display", "none");
                    $(".manualLink[category_id='" + CategoryId + "'][taxonomy_category_id='" + TaxonomyCategoryId + "'][variable_id='" + VariableId + "' ]").removeClass("manualLink ui-draggable ui-draggable-handle").addClass("notesLink ui-draggable ui-draggable-handle");
                });
            } else {
                $("#addNotes" + CategoryId + "-" + TaxonomyCategoryId).css("display", "none");
            }

        }
        /*The below code used to call the ClaerNotes Method*/
        function ClearNotes(CategoryId, VariableId, TaxonomyCategoryId, TaxonomyVariableId, text) {
            var notes = $("#txt" + CategoryId + "-" + TaxonomyCategoryId).val();
            if ($("#txt" + CategoryId + "-" + TaxonomyCategoryId).val() != "") {
                var parameters = "";
                parameters += "IdTaxonomyCategory=" + TaxonomyCategoryId + "&";
                parameters += "IdCategory=" + CategoryId + "&";
                parameters += "IdVariable=" + VariableId + "&";
                parameters += "IdTaxonomyVariable=" + TaxonomyVariableId + "&";
                parameters += "Notes=" + $("#txt" + CategoryId + "-" + TaxonomyCategoryId).val();

                _AjaxRequest("/Handlers/AutoLink.ashx", "ClearNotes", parameters, function (response) {
                    $("#addNotes" + CategoryId + "-" + TaxonomyCategoryId).html = "";
                    $("#txt" + CategoryId + "-" + TaxonomyCategoryId).val('');
                    $("#addNotes" + CategoryId + "-" + TaxonomyCategoryId).css("display", "none");
                    $(".notesLink[category_id='" + CategoryId + "'][taxonomy_category_id='" + TaxonomyCategoryId + "'][variable_id='" + VariableId + "' ]").removeClass("notesLink").addClass("manualLink");
                });
            } else {
                $("#addNotes" + CategoryId + "-" + TaxonomyCategoryId).css("display", "none");
            }
        }
    </script>
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitleTaxonomyManager" runat="server" Name="Linking"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
  <wu:ConfirmBox ID="cbReset" runat="server"></wu:ConfirmBox>
   
     <wu:Box ID="boxVariableSearch" Height="500" runat="server" Title="VariableSearch" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true" OnClientClose="if(!filtersChanged) return; window.setTimeout(PopulateCrosstable, 500); filtersChanged = false;">
          <div style="margin-top:-100px;margin-bottom:100px;">
            <div id="btnCancelVariableSearch" class="CancelCircle" onclick="CloseBox('boxVariableSearchControl', 'Bottom');" style="position:absolute;"></div>
            <div id="btnConfirmVariableSearch" class="OkCircle" onclick="CloseBox('boxVariableSearchControl', 'Bottom');AddVariableClick()" style="position:absolute;margin-left: 1035px;"></div>
        </div>
        <div class="PnlVariableSelectors">
            <div style="margin:1em;">
                <select id="ddlVariableSearchChapter" style="display:none;"></select>
                <asp:TextBox ID="txtVariableSearch" onkeyup="SearchVariables();" runat="server" type="text" style="width:99%;border-radius:5px;border-style:solid;font-size:16pt;" autocomplete="off" class="Color1 BorderColor1"  />
            </div>
            <div id="variableSearchResults" class="VariableSearchResults BorderColor1" style="width:1000px;margin:1em;overflow:auto;">

            </div>
             <%-- <div id="pnlSelectedVariableList">
                <input type="button" value="Add Variables" onclick="AddVariableClick()" />
            </div>--%>
        </div>
        <wu:TipGallery ID="tgVariableSearch" runat="server" _TipItems="StudyVariableSearchTip" />
    </wu:Box>
    <img id="imgVariableSearchCancelBin" src="/Images/Icons/Bin.png" style="display:none;position:absolute;right:50px;bottom:50px;width:100px;opacity:1.0;" />
  <div id="divTaxonomySelector" style="overflow:hidden;">  
    <input type="hidden" runat="server" id="selectedHierarchyId" value="" />    
    <table style="width:100%" cellspacing="0" cellpadding="0">
        <tr valign="top">
            <td class="BackgroundColor7" rowspan="2" style="width:250px;min-width:250px;">
                <div style="margin:1em">
                    <asp:Panel id="pnlHierarchies" runat="server" style="overflow:auto;"></asp:Panel>
                </div>
            </td>
            <td class="BackgroundColor7" style="height: 10px;">
                <div style="margin:5px;">                 
                    <div style="float:right;width:70%;">
                        <input type="text" style="margin:5px;width:99%;border-radius:5px;border-style:solid;font-size:16pt;" id="txtSearchTaxonomyVariables" onkeyup="searchTaxonomyVariables();" />
                    </div>
                    <div id="lblHierarchyPath" class="LabelHierarchyPath Color1" style="float:left;width:30%;">

                    </div>
                    <div style="clear:both"></div>
                </div>
            </td>
        </tr>
        <tr valign="top">
            <td style="width:100%;">
                <div id="pnlTaxonomyVariablesContainer" style="overflow:auto;">
                    <div style="margin:1em">
                        <div id="pnlTaxonomyVariables"></div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
 </div>
<div id="divAutoLink" class="hidden">
      <div style="margin-top:5px;margin-left:15px;">
          <div>
            <div class="BackButton" style="float:left;"></div>
            <div style="float:right;margin-right:50px;vertical-align:middle;">               
             <i class='md-label' style='color:#61CF71;font-size:12pt;'></i>&nbsp;<span style='font-size:11pt;'><%= LinkOnline.Global.LanguageManager.GetText("ExistingLinks") %></span> &nbsp;
             <i class='md-label' style='color:#05E9FF;font-size:12pt;'></i>&nbsp;<span style='font-size:11pt;'><%= LinkOnline.Global.LanguageManager.GetText("PrefferedText") %></span> &nbsp; 
             <i class='md-label' style='color:#b8860b;font-size:12pt;'></i>&nbsp;<span style='font-size:11pt;'><%= LinkOnline.Global.LanguageManager.GetText("LinkedNotesText") %></span> &nbsp;   
             <i class='md-label' style='color:#ff0000;font-size:12pt;'></i>&nbsp;<span style='font-size:11pt;'><%= LinkOnline.Global.LanguageManager.GetText("NonMatchText") %></span>
            </div>
          </div>
          <div class="divtaxonomyvariable">
             <table  id="taxonomyvariable">
                <tr class="taxonomyVariableLabel">
                    <td id="taxonomyVariableName"></td>
                    <td id="AddSymbol"><img src="/Images/Icons/Add2.png" id="AddStudy" onclick="document.getElementById('variableSearchResults').style.height=(window.innerHeight - 350) + 'px';InitDragBox('boxVariableSearchControl');SearchVariables();"></td>
                </tr>
            </table>
          </div>
          <div style="margin-top:5px;margin-left:15px;">
              <input type="button" id="AutoLink" value="link" class="ButtonLinking" />
          </div>
      </div>
  </div>
   
</asp:content>
