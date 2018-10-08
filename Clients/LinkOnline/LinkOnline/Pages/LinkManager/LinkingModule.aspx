<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LinkingModule.aspx.cs" Inherits="LinkOnline.Pages.LinkManager.LinkingModule" %>


<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js"></script>
  
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
  
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
    <link rel="stylesheet" href="/resources/demos/style.css">
 
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Controls/CategorySearch.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Controls/CategorySearch.js"></wu:ScriptReference>   
    <script src="../../Scripts/Pages/LinkModule.js"></script>

    <wu:ScriptReference runat="server" Source="/Scripts/Modules/VariableSelector.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Modules/VariableSelector2.css"></wu:StylesheetReference>
<link href="../../Stylesheets/Pages/LinkModule.css" rel="stylesheet" />
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><label>Linking</label> </h1>
</asp:content>

<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <input type="hidden"  id="selectedHierarchyId" value="" />

    <div class="modal fade" id="variableModal" role="dialog">
    <div class="modal-dialog">
    
      <!-- Modal content-->
      <div class="modal-content">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal">&times;</button>
          <h4 class="modal-title">Add Variable</h4>
        </div>
        <div class="modal-body">
            <input type="text" id="txtsearchVariables" />
            <div id="pnlVariableSelect" > 

            </div>
            <div id="pnlSelectedVariableList">
                <input type="button" value="Add Variable" onclick="AddVariableClick()" />
            </div>

        </div>
      </div>
    </div>
  </div>
  

  <div id="taxonomySelectionDiv" class="">
     <table style="width:100%" cellspacing="0" cellpadding="0">
         <tr valign="top">
            <td class="BackgroundColor7" rowspan="2" style="width:250px;min-width:250px;">
                <div style="margin:1em">
                    <asp:Panel id="pnlHierarchies" runat="server" style="overflow:auto;"></asp:Panel>
                </div>
            </td>
            <td class="BackgroundColor7" style="height: 50px;">
                <div style="margin:10px;">
                    <div id="lblHierarchyPath" class="LabelHierarchyPath Color1">

                    </div>
                    <div style="clear:both"></div>
                </div>
            </td>
        </tr>

        <tr valign="top">
            <td style="width:100%;">
                <div><input type="text" style="margin:10px;" id="txtSearchTaxonomyVariables" onkeyup="searchTaxonomyVariables();" /></div>
                <div id="pnlTaxonomyVariablesContainer" style="overflow:auto;">
                    <div style="margin:1em">
                        <div id="pnlMainTaxonomyVariableSelect"></div>
                    </div>
                </div>
            </td>
        </tr>
     </table>
  </div>
  <div id="LinkingDiv" class="hidden">
      <div>
          <span  class="BackButton"></span>
          <div class="divtaxonomyvariable">
             <table  id="taxonomyvariable">
                <tr class="taxonomyVariableLabel">
                    <td id="taxonomyVariableName"></td>
                    <td id="AddSymbol"   class="glyphicon glyphicon-plus"> </td>
                </tr>
            
            </table>
          </div>
          <div>
              <input type="button" id="AutoLink" value="Auto Link" />
          </div>
      </div>

  </div>



</asp:content>

