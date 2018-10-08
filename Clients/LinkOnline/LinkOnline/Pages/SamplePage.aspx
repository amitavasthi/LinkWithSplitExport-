<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="SamplePage.aspx.cs" Inherits="LinkOnline.Pages.SamplePage" %>

<asp:content id="ContentHead" contentplaceholderid="cphHead" runat="server">
    <wu:StylesheetReference Source="../Stylesheets/Overview.css" runat="server" />
     <wu:StylesheetReference Source="../Stylesheets/Main.css" runat="server" />
    <script src="http://code.highcharts.com/highcharts.js"></script>
    <script src="http://code.highcharts.com/highcharts-more.js"></script>
    
     <script>
         var hostname = window.location.host;
         var url = "http://" + hostname + "/Handlers/LinkOverview.ashx?Method=GetStudyResponse&ResponseType=JSON";
         /*This is for fetching the data from the JSON*/
         $(function () {
             $.getJSON(url, function (data) {
                 var jSonData = data.values;
                 var type = "bar";
                 loadBarChart(jSonData, type);
             });
         });

         function loadBarChart(data, chartType) {
             var processedstudy_json = new Array();
             for (i = 0; i < data.length; i++) {
                 processedstudy_json.push([data[i].study, data[i].responseCount]);
             }
             Highcharts.setOptions({
                 colors: ['#50B432', '#FCB040', '#6CAEE0', '#DDDF00', '#ED561B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#FFFFFF', '#6AF9C4'],
                 chart: {
                     style: {
                         fontFamily: 'SegoeUILight'
                     }
                 }
             });
             var chart = new Highcharts.Chart({
                 chart: {
                     renderTo: 'container',
                     type: chartType
                 },
                 credits: {
                     enabled: false
                 },
                 title: false,
                 //title: {
                 //    text: '<span style="color: #4572A7">INSIDE </span><span style="color:#6CAEE0"> L</span><span style="color:#FCB040">i</span><span style="color:#6CAEE0">NK</span>'
                 //},
                 xAxis: {
                     categories: [data.study],
                     title: {
                         text: '<b> Name of the Study </b>'
                     }
                 },
                 yAxis: {
                     min: 0,
                     title: {
                         text: '<b> Number of Respondents </b>'
                     }
                 },
                 tooltip: {
                     formatter: function () {
                         return '<b>' + this.point.name + '</b>: ' + this.y;
                     }
                 },
                 legend: {
                     layout: 'vertical',
                     align: 'right',
                     verticalAlign: 'middle',
                     borderWidth: 0

                 },
                 plotOptions: {
                     series: {
                         stacking: 'normal',
                         showInLegend: false
                     }
                 },
                 series: [{
                     data: processedstudy_json
                 }]
             });
         }
    </script>
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="overview"></wu:Label></h1>
</asp:content>
<asp:content id="ContentPart" contentplaceholderid="cphContent" runat="server">
     <div id="chartOuterContainer" style="margin:auto;clear:both;">
          <%-- <div style="margin-top:50px;height:auto;clear:both;"> 
                 <div id="container" style="height: auto;width:900px;margin-left:auto;margin-right:auto;"></div>
            </div>--%>
         <asp:Panel ID="pnl" runat="server"></asp:Panel>
     </div>
</asp:content>

