<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManualLinkManager.aspx.cs" Inherits="LinkOnline.Pages.LinkManager.ManualLinkManager" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
     <meta name="viewport" content="width=device-width, initial-scale=1">

     <link href="../../Stylesheets/BootStrap/bootstrap.min.css" rel="stylesheet" />
        
     <script src="../../Scripts/jquery-1.10.2.js"></script>
    
     <script src="../../Scripts/BootStrap/bootstrap.min.js"></script>

     <link href="../../Stylesheets/JQuery/jqueryui/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet" />

    <script src="../../Scripts/JQuery/jquery-ui-1.7.2.custom.min.js"></script>

    <wu:StylesheetReference runat="server" Source="/Stylesheets/Controls/CategorySearch.css"></wu:StylesheetReference>
    <wu:ScriptReference runat="server" Source="/Scripts/Controls/CategorySearch.js"></wu:ScriptReference>
    <script src="../../Scripts/Pages/ManualLinkManager.js"></script>
    <link href="../../Stylesheets/Pages/ManualLinkManager.css" rel="stylesheet" />  
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="lblManualLink"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
   
    <div class="modal fade" id="myModal" role="dialog">
    <div class="modal-dialog">
    
      <!-- Modal content-->
      <div class="modal-content">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal">&times;</button>           
           <h1 class="Color1"><%= LinkOnline.Global.LanguageManager.GetText("lblManualLink") %></h1>
        </div>
        <div class="modal-body">
            <p class="text-success" id="p-LinkSuccessMessage">Link Process Successfully Completed!</p>
            <h1 class="Color1"> Mapping Details</h1>
            <p><span class="Color1"><%= LinkOnline.Global.LanguageManager.GetText("lblStudyCategories") %> </span></p>
            <ul id="ulCategoriesList">

            </ul>
            <p><span class="Color1"> <%= LinkOnline.Global.LanguageManager.GetText("lblTaxanomyCategories") %>  :</span>   </p>
            <ul id="ulTaxonomyCategoriesList">

            </ul>
        </div>
        <div class="modal-footer">             
             <input type="button"  data-dismiss="modal" value="Close" />
        </div>
       </div>      
      </div>
    </div>
    
    <input type="hidden" id="hdnUndoMessage" value="<%= LinkOnline.Global.LanguageManager.GetText("UndoConfirmationMessage") %>"/>
    <input type="hidden" id="hdnBackButtonMessage" value="<%= LinkOnline.Global.LanguageManager.GetText("BackButtonConfirmationMessage") %>"/>
    <div class="alert alert-danger hidden-div" role="alert" id="DivValidationMessage">
            <div class="hidden-div" id="divStudyCategoriesMandatoryMessage" ><%= LinkOnline.Global.LanguageManager.GetText("StudyCategoriesMandatoryMessage") %></div>
            <div class="hidden-div" id="divTaxonomyCategoriesMandatoryMessage" ><%= LinkOnline.Global.LanguageManager.GetText("TaxonomyCategoriesMandatoryMessage") %></div>
    </div>
    <div class="container-fluid container-padding" onmouseover="BindLoaderEvents()" id="div-variables">
        <div class="row">
            <div class="col-lg-6">
                 <wu:Label ID="Label3" runat="server" Name="lblStudyCategories" CssClass="Color1"></wu:Label>
            </div>
             <div class="col-lg-6">
                  <wu:Label ID="Label4" runat="server" Name="lblTaxanomyCategories" CssClass="Color1"></wu:Label>
             </div>
        </div>       
        <div class="row">
            <div class="col-lg-6 " id="divStudyVariablesList">                               
                 <wu:CategorySearch ID="csStudyCategories" runat="server" />
            </div>            
            <div class="col-lg-6">
                 <wu:CategorySearch ID="csTaxonomyCategories" runat="server"  />
            </div>            
        </div>
        <div class="row">
            <div class="col-lg-12">
            <div class="footer-panel">
                <input type="button" class="btn-Undo" id="btn_Category_Undo" value="Undo"  />
                <input type="button" class="btn-Link" id="btn-Link" value="LiNK" />                                  
     <%--  <input type="button" class="btn-UnLink" id="Un_link_categories" value="Unlink"  />--%>
                </div>
            </div>
        </div>
    </div>



</asp:content>
