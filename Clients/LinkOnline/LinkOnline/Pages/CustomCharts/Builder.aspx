<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Builder.aspx.cs" Inherits="LinkOnline.Pages.CustomCharts.Builder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .Content {
            background:#E4E4E4;
        }

        .CustomChartPage {
            background:#FFFFFF;
            box-shadow:0px 0px 20px 0px #444444;
            border-radius:5px;

            margin:50px;
            overflow:hidden;
        }

        .Section {
            float:left;
            border:1px dotted #E6E6E6;
        }

        .Section:hover {
            background:#e4f3ff;
        }



        .wheelMenu {
	max-width:200px;
	width: 100%;
    position:absolute;
}
    </style>
    
    <wu:ScriptReference runat="server" Source="/Scripts/D3JS/wheelMenu.js"></wu:ScriptReference>
    <wu:StylesheetReference runat="server" Source="/Stylesheets/Controls/wheelStyles.css"></wu:StylesheetReference>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script type="text/javascript">
        loadFunctions.push(function () {
            var customChartPage = document.getElementById("customChartPage");

            customChartPage.style.height = (ContentHeight - 100) + "px";
        });
    </script>
    <script type="text/javascript">
        function ShowSectionOptions(sender) {
            var wheel = document.getElementById("donut");
            wheel.innerHTML = "";

            window.menu = new wheelMenu('#donut', 1, '', ['\uf001', '\uf015', '\uf0fa', '\uf072', '\uf07a', '\uf0f5']);
            var circle = wheel.getElementsByTagName("circle").item(0);

            circle.onclick = function () {
                wheel.innerHTML = "";
            }

            wheel.style.left = (tempX - 100) + "px";
            wheel.style.top = (tempY - 100) + "px";

            window.setTimeout(function () {
                sender.style.background = "#e4f3ff";
            }, 1);
        }
</script>
    <div id='donut' class='wheelMenu'>
	</div>
    <div id="customChartPage" class="CustomChartPage">
        <div class="Section" onclick="ShowSectionOptions(this);" onmouseout="this.style.background='';" style="width:1228px;height:577px;"></div>
        <div class="Section" onclick="ShowSectionOptions(this);" onmouseout="this.style.background='';" style="width:1228px;height:577px;"></div>
        <div class="Section" onclick="ShowSectionOptions(this);" onmouseout="this.style.background='';" style="width:1228px;height:577px;"></div>
        <div class="Section" onclick="ShowSectionOptions(this);" onmouseout="this.style.background='';" style="width:1228px;height:577px;"></div>
    </div>
</asp:Content>
