<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="HierarchyFilter.aspx.cs" Inherits="LinkOnline.Pages.DataManagement.HierarchyFilter" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .HierarchyFilter {
            float:left;
            margin:10px;

            width:200px;
            height:100%;
            
            box-shadow:0px 0px 2px 0px #444444;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script type="text/javascript">
        loadFunctions.push(function () {
            document.getElementById("cphContent_pnlHierarchyFilters").style.height = (ContentHeight - 20) + "px";
        });
    </script>
    <asp:Panel ID="pnlHierarchyFilters" runat="server"></asp:Panel>
</asp:Content>
