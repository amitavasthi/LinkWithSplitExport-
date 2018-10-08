<%@ Page 
    Title="" 
    Language="C#" 
    AutoEventWireup="true" 
    CodeBehind="Architect.aspx.cs" 
    Inherits="LinkOnline.Pages.Architect" 
    EnableViewState="false"
    EnableViewStateMac="false"
%>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        //loadFunctions.push(LoadSuggestions);
        function LoadSuggestions() {
            _AjaxRequest("Architect.aspx", "LoadSuggestions", "", function (response) {
                document.getElementById("pnlSuggestions").innerHTML = response;
            });
        }


    </script>
    <style type="text/css">
        .Variable {
            padding:10px;
            margin:5px;

            color:#FFFFFF;
        }
    </style>
</asp:Content>
<asp:content id="Content3" contentplaceholderid="cphTitle" runat="server">
    <%--<h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="ArchitectTitle"></wu:Label></h1>--%>
     <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="ArchitectTitle"></wu:Label>  
        <span class="Beta">
            <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
        </span></h1>
</asp:content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div id="pnlSuggestions"></div>
</asp:Content>
