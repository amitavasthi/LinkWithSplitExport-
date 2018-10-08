<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="LinkOverview.aspx.cs" Inherits="LinkOnline.Pages.LinkOverview" %>

<asp:content id="ContentHead" contentplaceholderid="cphHead" runat="server">
    <wu:StylesheetReference Source="../Stylesheets/Overview.css" runat="server" />
     <wu:StylesheetReference Source="../Stylesheets/Main.css" runat="server" />
    <script src="http://code.highcharts.com/highcharts.js"></script>
    <script src="http://code.highcharts.com/highcharts-more.js"></script>
       <script  src="../Scripts/Overview.js"></script>
     <script>
         var hostname = window.location.host;
         var urlChart = "http://" + hostname + "/Handlers/HomeHandler.ashx?Method=GetChartType";//"http://localhost:54580/Handlers/LinkOverview.ashx?Method=ProcessReport&ResponseType=JSON";
         var typeofChart = "donut";
         /*This is for fetching the data from the JSON*/
         $(function () {
             $.getJSON(urlChart, function (chartType) {
                 typeofChart = chartType.chartType;
             });
         });

         function loadChart(data, chartType) {

             if (chartType == "donut") {

                 Highcharts.setOptions({
                     colors: ['#FCB040', '#6CAEE0', '#DDDF00', '#50B432', '#ED561B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#FFFFFF', '#6AF9C4'],
                     chart: {
                         style: {
                             fontFamily: 'SegoeUILight'
                         }
                     }
                 });

                 Highcharts.setOptions({
                     colors: ['#FCB040', '#6CAEE0', '#DDDF00', '#50B432', '#ED561B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#FFFFFF', '#6AF9C4']
                 });

                 var chart = new Highcharts.Chart({
                     chart: {
                         renderTo: 'container',
                         type: 'pie',
                         borderRadius: 1
                     },
                     credits: {
                         enabled: false
                     },
                     title: false,
                     plotOptions: {
                         pie: {
                             innerSize: '50%',
                             allowPointSelect: true,
                             cursor: 'pointer',
                             dataLabels: {
                                 enabled: true
                             },
                             showInLegend: true
                         }
                     },
                     tooltip: {
                         formatter: function () {
                             return '<b>' + this.point.name + '</b>: ' + this.y + " " + this.point.name;
                         }
                     },
                     series: [{
                         data: [['Studies', data.studies],
                                 ['Linked Variables', data.linkedVariables],
                                 ['UnLinked Variables', data.unLinkedVariables],
                                 ['Linked Categories', data.linkedCategories],
                                 ['UnLinked Categories', data.unLinkedCategories]

                         ]
                     }]
                 },

                 function (chart) { // on complete

                     var xpos = '47%';
                     var ypos = '49%';

                     var circleradius = 100;
                     // Render the text 
                     //chart.renderer.text('<span style="color: #4572A7">inside </span><span style="color:#6CAEE0"> L</span><span style="color:#FCB040">i</span><span style="color:#6CAEE0">NK</span>', 405, 190).css({
                     //    width: circleradius * 2,
                     //    color: '#4572A7',
                     //    fontSize: '16px',
                     //    textAlign: 'center'
                     //}).attr({
                     //    // why doesn't zIndex get the text in front of the chart?
                     //    zIndex: 999
                     //}).add();
                    // chart.renderer.image('http://' + hostname + '/Images/Logos/Blueoceansmall.png', 410, 170, 90, 30)
                    //.add();
                 });
             }
             else if (chartType == "pie") {
                 $('#container').highcharts({
                     chart: {
                         plotBackgroundColor: null,
                         plotBorderWidth: null,
                         plotShadow: true
                     },
                     credits: false,
                     title: false,
                     tooltip: {
                         pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                     },
                     plotOptions: {
                         pie: {
                             allowPointSelect: true,
                             cursor: 'pointer',
                             dataLabels: {
                                 enabled: true,
                                 format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                                 style: {
                                     color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                                 }
                             },
                             connectorColor: 'silver',
                             showInLegend: true
                         }
                     },
                     series: [{
                         type: 'pie',
                         name: '<span style="color: #4572A7">INSIDE </span><span style="color:#6CAEE0"> L</span><span style="color:#FCB040">i</span><span style="color:#6CAEE0">NK</span>',
                         data: [
                             ['Studies', data.studies],
                                 {
                                     name: 'Linked Variables',
                                     y: data.linkedVariables,
                                     sliced: true,
                                     selected: true
                                 },
                                 ['UnLinked Variables', data.unLinkedVariables],
                                 ['Linked Categories', data.linkedCategories],
                                 ['UnLinked Categories', data.unLinkedCategories]
                         ]
                     }]
                 });
             }
             else {

                 Highcharts.setOptions({
                     colors: ['#FCB040', '#6CAEE0', '#DDDF00', '#50B432', '#ED561B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#FFFFFF', '#6AF9C4'],
                     chart: {
                         style: {
                             fontFamily: 'SegoeUILight'
                         }
                     }
                 });

                 Highcharts.setOptions({
                     colors: ['#6CAEE0', '#FCB040', '#DDDF00', '#50B432', '#ED561B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#FFFFFF', '#6AF9C4']
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
                         categories: ['Studies', 'Linked Variables', 'UnLinked Variables', 'Linked Categories', 'UnLinked Categories']
                     },
                     yAxis: {
                         min: 0,
                         title: {
                             text: ''
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
                         data: [['Studies', data.studies],
                               ['Linked Variables', data.linkedVariables],
                               ['UnLinked Variables', data.unLinkedVariables],
                               ['Linked Categories', data.linkedCategories],
                               ['UnLinked Categories', data.unLinkedCategories]

                         ]
                     }]
                 },
                 function (chart) { // on complete

                     //chart.legend.allItems[0].update({ name: 'Inside <span style="color:#6CAEE0"> L</span><span style="color:#FCB040">i</span><span style="color:#6CAEE0">NK</span>' });
                 });
             }
         }
         var hostname = window.location.host;
         var url = "http://" + hostname + "/Handlers/LinkOverview.ashx?Method=ProcessReport&ResponseType=JSON";//"http://localhost:54580/Handlers/LinkOverview.ashx?Method=ProcessReport&ResponseType=JSON";
         /*This is for fetching the data from the JSON*/
         $(function () {
             $.getJSON(url, function (data) {
                 var jSonData = data.values[0];
                 //var type = "donut";
                 var type = typeofChart;
                 loadChart(jSonData, type);
             });
         });


         $(document).ready(function () {
             $("#divLine a").click(function () {
                 $.getJSON(url, function (data) {
                     saveChartType("line");
                     var jSonData = data.values[0];
                     loadChart(jSonData, "line");
                 });
             });
             $("#divBar a").click(function () {
                 $.getJSON(url, function (data) {
                     saveChartType("bar");
                     var jSonData = data.values[0];
                     loadChart(jSonData, "bar");
                 });
             });
             $("#divDonut a").click(function () {
                 $.getJSON(url, function (data) {
                     saveChartType("donut");
                     var jSonData = data.values[0];
                     loadChart(jSonData, "donut");
                 });
             });
             $("#divColumn a").click(function () {
                 $.getJSON(url, function (data) {
                     saveChartType("column");
                     var jSonData = data.values[0];
                     loadChart(jSonData, "column");
                 });
             });
             $("#divPie a").click(function () {
                 $.getJSON(url, function (data) {
                     saveChartType("pie");
                     var jSonData = data.values[0];
                     loadChart(jSonData, "pie");
                 });
             });
         });

    </script>
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="overview"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:content id="ContentPart" contentplaceholderid="cphContent" runat="server">
     <div id="chartOuterContainer" style="margin:auto;clear:both;">
         <div style="margin-top:30px;height:auto;clear:both;">
              <div  style="float:left;margin-left:50px;"  class="BackButton" onclick="window.location='/Pages/Default.aspx'"></div>
           <div style="margin-left:auto;margin-right:auto;float:left;padding-left:100px;" id="divLine">
              <a href="#" id="lineChart"><img id="imgLine" src="../Images/Icons/Home/linechart.jpg" height="50" width="60" class="imgLine"/></a>
           </div>
           <div style="margin-left:auto;margin-right:auto;float:left;padding-left:100px;" id="divBar">
              <a href="#" id="barChart"><img id="imgBar" src="../Images/Icons/Home/baricon.png" height="50" width="60" class="imgBar"/></a>
           </div>
           <div style="margin-left:auto;margin-right:auto;float:left;padding-left:100px;" id="divDonut">
              <a href="#" id="donutChart"><img id="imgDonut" src="../Images/Icons/Home/donuticon.png" height="50" width="60" class="imgDonut"/></a>
           </div>
            <div style="margin-left:auto;margin-right:auto;float:left;padding-left:100px;" id="divColumn">
              <a href="#" id="columnChart"><img id="imgColumn" src="../Images/Icons/Home/columnicon.jpg" height="50" width="60" class="imgColumn"/></a>
           </div>
           <div style="margin-left:auto;margin-right:auto;float:left;padding-left:100px;" id="divPie">
              <a href="#" id="pieChart"><img id="imgPie" src="../Images/Icons/Home/pieicon.jpg" height="50" width="60" class="imgPie"/></a>
           </div>
         </div>
           <div style="margin-top:50px;height:auto;clear:both;"> 
                 <div id="container" style="height: auto;width:900px;margin-left:auto;margin-right:auto;"></div>
            </div>
     </div>
</asp:content>
