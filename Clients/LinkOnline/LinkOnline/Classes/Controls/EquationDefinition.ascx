<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EquationDefinition.ascx.cs" Inherits="LinkOnline.Classes.Controls.EquationDefinition" %>

    
<wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/EquationDefinition.css"></wu:StylesheetReference>
<wu:ScriptReference runat="server" Source="/Scripts/Modules/EquationDefinition.js"></wu:ScriptReference>

<script type="text/javascript" src="/Scripts/Jquery/CodeMirror/codemirror.js"></script>
<script type="text/javascript" src="/Scripts/Jquery/CodeMirror/clike.js"></script>
<link rel="stylesheet" href="/Scripts/Jquery/CodeMirror/codemirror.css" />
<link rel="stylesheet" href="/Scripts/Jquery/CodeMirror/csharp.css" />
    
<script type="text/javascript" src="/Scripts/Jquery/CodeMirror/autocomplete/variables-hint.js"></script>
<script type="text/javascript" src="/Scripts/Jquery/CodeMirror/autocomplete/show-hint.js"></script>
<link rel="stylesheet" href="/Scripts/Jquery/CodeMirror/autocomplete/show-hint.css" />

<script type="text/javascript">
    loadFunctions.push(function () {
        window.editor = CodeMirror.fromTextArea(document.getElementById("txtEquationDefinitionEquation"), {
            mode: 'text/x-csharp',
            indentWithTabs: false,
            smartIndent: true,
            lineNumbers: true,
            matchBrackets: true,
            autofocus: true,
            extraKeys: { "Ctrl-Space": "autocomplete" }
        });
    });
</script>

<wu:Box ID="boxEquationDefinition" runat="server" style="" Title="Equation" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true" OnClientClose="SetEquation(equationScoreSource, equationScorePath, window.editor.getValue(), false);">
    <div class="EquationToolBar">
        <div class="EquationToolBarItem" style="float:right;" onclick="ExpandEquationEditor();" onmouseover="ShowToolTip(this, LoadLanguageText('EquationToolTipExpandEditor'), false, 'Bottom')">
            <img style="height:15px;" src="/Images/Icons/Equation/ExpandEditor.png" />
        </div>
        <div class="EquationToolBarItem" onclick="EquationInsertCategory();" onmouseover="ShowToolTip(this, LoadLanguageText('EquationToolTipInsertPlaceHolder'), false, 'Bottom')">
            <img style="height:15px;" src="/Images/Icons/Equation/InsertPlaceHolder.png" />
        </div>
        <div class="EquationToolBarItem" onclick="EquationInsertMethod();" onmouseover="ShowToolTip(this, LoadLanguageText('EquationToolTipInsertMethod'), false, 'Bottom')">
            <img style="height:15px;" src="/Images/Icons/Equation/InsertMethod.png" />
        </div>
        <div class="EquationToolBarItem" onclick="EquationInsertCondition();" onmouseover="ShowToolTip(this, LoadLanguageText('EquationToolTipInsertCondition'), false, 'Bottom')">
            <img style="height:15px;" src="/Images/Icons/Equation/InsertCondition.png" />
        </div>
        <!--<div class="EquationToolBarItem" onclick="ValidateEquation();" onmouseover="ShowToolTip(this, LoadLanguageText('EquationToolTipValidate'), false, 'Bottom')">
            <img style="height:15px;" src="/Images/Icons/Equation/Validate.png" onclick="" />
        </div>-->
    </div>
    <div id="pnlEquationDefinition" style="font-size:18pt;width:1000px;height:400px;border:1px solid #444444;">
        <textarea id="txtEquationDefinitionEquation"></textarea>
        <div id="pnlEquationDefinitionErrors" style="height:90px;background:#F6F6F6;border-top:1px solid #444444;border-bottom:1px solid #444444;font-size:10pt;padding:5px;overflow-y:auto">

        </div>
    </div>
    <wu:TipGallery ID="tgEquationDefinition" runat="server" _TipItems="EquationDefinitionTip1, EquationDefinitionTip2" />
</wu:Box>
<wu:Box ID="boxEquationDefinitionCategorySearch" runat="server" Title="SearchPlaceHolder" TitleLanguageLabel="true" Position="Top" JavascriptTriggered="true" Dragable="true">
    <div style="width:1000px">
        <wu:CategorySearch ID="csEquationDefinition" runat="server" SelectionType="Multi" EnableNonCategorical="true">

        </wu:CategorySearch>
    </div>
    <div id="btnCancelEquationPlaceHolderInsert" class="CancelCircle" onclick="document.getElementById('cphContent_EquationDefinition_csEquationDefinition').Close(false);" style="position:absolute;">
    </div>
    <div id="btnConfirmEquationPlaceHolderInsert" class="OkCircle" onclick="document.getElementById('cphContent_EquationDefinition_csEquationDefinition').Close(true);" style="position:absolute;margin-left: 1006px;">
    </div>
</wu:Box>
    
<wu:Box ID="boxEquationInsertMethod" runat="server" Title="InsertMethod" TitleLanguageLabel="true" Position="Top" JavascriptTriggered="true" Dragable="true">
    <div>
        <div id="pnlEquationInsertMethodResults" style="width:1000px;overflow:auto;"></div>
    </div>
    <div id="btnCancelEquationMethodInsert" class="CancelCircle" onclick="DisableEquationMethodInsert(false);" style="position:absolute;">
    </div>
    <div id="btnConfirmEquationMethodInsert" class="OkCircle" onclick="DisableEquationMethodInsert(true);" style="position:absolute;margin-left: 1000px;">
    </div>
</wu:Box>