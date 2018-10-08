<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Dashboards.aspx.cs" Inherits="LinkOnline.Pages.Dashboards" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <style type="text/css">
        .Content {
            overflow:hidden !important;
        }
    </style>        
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphTitle" runat="server">
    <table>
        <tr>
            <td>
                <div class="BackButton" onclick="window.location='/Pages/CustomCharts/ChartsHome.aspx'" style=""></div>
            </td>
            <td>
                <h1 class="Color1">
                    <asp:Label ID="lblDefinitionName" runat="server"></asp:Label>
                </h1>
            </td>
        </tr>
    </table>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">    
    <script type="text/javascript">
        /*loadFunctions.push(function () {
            var frame = document.getElementById("cphContent_frame");

            frame.style.height = (ContentHeight) + "px";
        });*/
    </script>
    
    <iframe id="frame" frameborder="0" src="" style="width:100%;height:100%;" runat="server"></iframe>
</asp:content>
