<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VariableSearch.ascx.cs" Inherits="LinkOnline.Classes.Controls.VariableSearch" %>
<wu:Box ID="boxVariableSearch" Height="500" runat="server" Title="VariableSearch" TitleLanguageLabel="true" JavascriptTriggered="true" Dragable="true" OnClientClose="document.body.focus();if(!filtersChanged) return; window.setTimeout(PopulateCrosstable, 500); filtersChanged = false;">
    <style>
 .qs {                                                               
        font-weight: bold;                                                                   
        position: relative; 
        right:15px;
        text-align: center;
         width: 30px; }
.qs .popover { background-color: rgba(0, 0, 0, 0.85); border-radius: 5px; bottom: 42px; box-shadow: 0 0 5px rgba(0, 0, 0, 0.4); color: #fff; display: none; font-size: 12px; font-family: 'Helvetica',sans-serif; left: -150px; padding: 7px 10px; position: absolute; width: 150px; z-index: 4; } .qs .popover:before { border-top: 7px solid rgba(0, 0, 0, 0.85); border-right: 7px solid transparent; border-left: 7px solid transparent; bottom: -7px; content: ''; display: block; left: 95%; margin-left: -7px; position: absolute; } .qs:hover .popover { display: block; -webkit-animation: fade-in .3s linear 1, move-up .3s linear 1; -moz-animation: fade-in .3s linear 1, move-up .3s linear 1; -ms-animation: fade-in .3s linear 1, move-up .3s linear 1; } @-webkit-keyframes fade-in { from { opacity: 0; } to { opacity: 1; } } @-moz-keyframes fade-in { from { opacity: 0; } to { opacity: 1; } } @-ms-keyframes fade-in { from { opacity: 0; } to { opacity: 1; } } @-webkit-keyframes move-up { from { bottom: 30px; } to { bottom: 42px; } } @-moz-keyframes move-up { from { bottom: 30px; } to { bottom: 42px; } } @-ms-keyframes move-up { from { bottom: 30px; } to { bottom: 42px; } } 
    </style>
    <div class="PnlVariableSelectors">
        <asp:Panel class="qs" ID="pnlVariableSearchExportVariables"  style="float:right; ">
           <span class="popover left">Download Taxonomy</span>
            <img src="/Images/Icons/ExportVariables.png" style="margin:10px;cursor:pointer;" class="BackgroundColor6" onclick="window.open('/Handlers/GlobalHandler.ashx?Method=ExportVariables');" />
        </asp:Panel>
        <div style="margin: 1em;">
            <select id="ddlVariableSearchChapter" style="display: none;"></select>
            <asp:TextBox ID="txtVariableSearch" autocomplete="off" runat="server" type="text" Style="width: 99%; border-radius: 5px; border-style: solid; font-size: 16pt;" class="Color1 BorderColor1" />
        </div>
        <div id="variableSearchResults" class="VariableSearchResults BorderColor1" style="width: 1000px; margin: 1em; overflow: auto;">
        </div>
    </div>
    <wu:TipGallery ID="tgVariableSearch" runat="server" _TipItems="VariableSearchTip1, VariableSearchTip2" />
</wu:Box>
<img id="imgVariableSearchCancelBin" src="/Images/Icons/Bin.png" style="display: none; position: absolute; right: 50px; bottom: 50px; width: 100px; opacity: 1.0;" />
