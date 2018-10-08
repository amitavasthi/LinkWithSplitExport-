var hostname = window.location.host;
var urlChart = "http://" + hostname + "/Handlers/HomeHandler.ashx?Method=GetChartType";//"http://localhost:54580/Handlers/LinkOverview.ashx?Method=ProcessReport&ResponseType=JSON";
var typeofChart = "donut";
/*This is for fetching the data from the JSON for multiple chart types*/
/*Multiple charts type details starts from here*/
//$(function () {
//    $.getJSON(urlChart, function (chartType) {
//        typeofChart = chartType.chartType;
//    });
//});
//function loadChart(data, chartType) {

//    if (chartType == "donut") {

//        Highcharts.setOptions({
//            colors: ['#FCB040', '#6CAEE0', '#DDDF00', '#50B432', '#ED561B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#FFFFFF', '#6AF9C4'],
//            chart: {
//                style: {
//                    fontFamily: 'SegoeUILight'
//                }
//            }
//        });
//        Highcharts.setOptions({
//            colors: ['#FCB040', '#6CAEE0', '#DDDF00', '#50B432', '#ED561B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#FFFFFF', '#6AF9C4']
//        });
//        var chart = new Highcharts.Chart({
//            chart: {
//                renderTo: 'container',
//                type: 'pie',
//                borderRadius: 1
//            },
//            credits: {
//                enabled: false
//            },
//            title: false,
//            plotOptions: {
//                pie: {
//                    innerSize: '50%',
//                    allowPointSelect: true,
//                    cursor: 'pointer',
//                    dataLabels: {
//                        enabled: true
//                    },
//                    showInLegend: true
//                }
//            },
//            tooltip: {
//                formatter: function () {
//                    return '<b>' + this.point.name + '</b>: ' + this.y + " " + this.point.name;
//                }
//            },
//            series: [{
//                data: [['Studies', data.studies],
//                        ['Linked Variables', data.linkedVariables],
//                        ['UnLinked Variables', data.unLinkedVariables],
//                        ['Linked Categories', data.linkedCategories],
//                        ['UnLinked Categories', data.unLinkedCategories]

//                ]
//            }]
//        },

//        function (chart) { // on complete

//            var xpos = '47%';
//            var ypos = '49%';

//            var circleradius = 100;
//            // Render the text 
//            //chart.renderer.text('<span style="color: #4572A7;font-size:12pt;">inside </span><span style="color:#6CAEE0;font-size:12pt;"> L</span><span style="color:#FCB040;font-size:12pt;">i</span><span style="color:#6CAEE0;font-size:12pt;">NK</span>', 210, 120).css({
//            //    width: circleradius * 2,
//            //    color: '#4572A7',
//            //    fontSize: '16px',
//            //    textAlign: 'center'
//            //}).attr({
//            //    // why doesn't zIndex get the text in front of the chart?
//            //    zIndex: 999
//            //}).add();

//            //chart.renderer.image('http://' + hostname + '/Images/Logos/Blueoceansmall.png', 205, 100, 70, 30)
//            //.add();
//        });
//    }
//    else if (chartType == "pie") {

//        Highcharts.setOptions({
//            colors: ['#FCB040', '#6CAEE0', '#DDDF00', '#50B432', '#ED561B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#FFFFFF', '#6AF9C4']
//        });
//        var chart = new Highcharts.Chart({
//            chart: {
//                renderTo: 'container',
//                type: 'pie',
//                borderRadius: 1,
//                plotBackgroundColor: null,
//                plotBorderWidth: null,
//                plotShadow: true
//            },
//            credits: {
//                enabled: false
//            },
//            title: false,
//            plotOptions: {
//                pie: {
//                    allowPointSelect: true,
//                    cursor: 'pointer',
//                    dataLabels: {
//                        enabled: true,
//                        format: '<b>{point.name}</b>: {point.percentage:.1f} %',
//                        style: {
//                            color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
//                        }
//                    },
//                    connectorColor: 'silver',
//                    showInLegend: true
//                }
//            },
//            tooltip: {
//                pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
//            },
//            series: [{
//                type: 'pie',
//                name: '<span style="color: #4572A7">INSIDE </span><span style="color:#6CAEE0"> L</span><span style="color:#FCB040">i</span><span style="color:#6CAEE0">NK</span>',
//                data: [
//                    ['Studies', data.studies],
//                        {
//                            name: 'Linked Variables',
//                            y: data.linkedVariables,
//                            sliced: true,
//                            selected: true
//                        },
//                        ['UnLinked Variables', data.unLinkedVariables],
//                        ['Linked Categories', data.linkedCategories],
//                        ['UnLinked Categories', data.unLinkedCategories]
//                ]
//            }]
//        });
//    }
//    else {

//        Highcharts.setOptions({
//            colors: ['#FCB040', '#6CAEE0', '#DDDF00', '#50B432', '#ED561B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#FFFFFF', '#6AF9C4'],
//            chart: {
//                style: {
//                    fontFamily: 'SegoeUILight'
//                }
//            }
//        });

//        Highcharts.setOptions({
//            colors: ['#6CAEE0', '#FCB040', '#DDDF00', '#50B432', '#ED561B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#FFFFFF', '#6AF9C4']
//        });

//        var chart = new Highcharts.Chart({
//            chart: {
//                renderTo: 'container',
//                type: chartType
//            },
//            credits: {
//                enabled: false
//            },
//            title: false,
//            //title: {
//            //    text: '<span style="color: #4572A7">INSIDE </span><span style="color:#6CAEE0"> L</span><span style="color:#FCB040">i</span><span style="color:#6CAEE0">NK</span>'
//            //},
//            xAxis: {
//                categories: ['Studies', 'Linked Variables', 'UnLinked Variables', 'Linked Categories', 'UnLinked Categories']
//            },
//            yAxis: {
//                min: 0,
//                title: {
//                    text: ''
//                }
//            },
//            tooltip: {
//                formatter: function () {
//                    return '<b>' + this.point.name + '</b>: ' + this.y;
//                }
//            },
//            legend: {
//                layout: 'vertical',
//                align: 'right',
//                verticalAlign: 'middle',
//                borderWidth: 0

//            },
//            plotOptions: {
//                series: {
//                    stacking: 'normal',
//                    showInLegend: false
//                }
//            },
//            series: [{
//                data: [['Studies', data.studies],
//                      ['Linked Variables', data.linkedVariables],
//                      ['UnLinked Variables', data.unLinkedVariables],
//                      ['Linked Categories', data.linkedCategories],
//                      ['UnLinked Categories', data.unLinkedCategories]

//                ]
//            }]
//        },
//        function (chart) { // on complete

//           // chart.legend.allItems[0].update({ name: 'Inside <span style="color:#6CAEE0"> L</span><span style="color:#FCB040">i</span><span style="color:#6CAEE0">NK</span>' });
//        });
//    }
//}
//var hostname = window.location.host;
//var url = "http://" + hostname + "/Handlers/LinkOverview.ashx?Method=ProcessReport&ResponseType=JSON";//"http://localhost:54580/Handlers/LinkOverview.ashx?Method=ProcessReport&ResponseType=JSON";
///*This is for fetching the data from the JSON*/
//$(function () {
//    $.getJSON(url, function (data) {
//        var jSonData = data.values[0];
//        var type = typeofChart;
//        loadChart(jSonData, type);
//    });
//});

//$(document).ready(function () {
//    $("#divDonut a").click(function () {
//        $.getJSON(url, function (data) {
//            var jSonData = data.values[0];
//            loadChart(jSonData, "donut");
//        });
//    });
//});
/*Multiple chart type ends here*/

/* The below code used for rendering study vs respondents details as per Mike's request on 26th May 2015*/
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

    var newHeight = 400;
    //var PointWidth = 20;
    if (data.length <= 5) {
        newHeight = 260;
    }
    else if ((data.length > 5) && (data.length <= 14)) {
        newHeight = 350;
    }
    else {
        newHeight = data.length * 20 + 120
    }


    var processedstudy_json = new Array();
    for (i = 0; i < data.length; i++) {
        processedstudy_json.push([data[i].study, data[i].responseCount]);
    }
    Highcharts.setOptions({
        chart: {
            style: {
                fontFamily: 'SegoeUILight'
            }
        }
    });
    var chart = new Highcharts.Chart({
        chart: {
            renderTo: 'container',
            type: chartType,
            height: newHeight,
            spacingBottom: 10,
            spacingRight: 0//data.length * 20 + 120,
        },
        credits: {
            enabled: false
        },
        exporting: {
            enabled: true
        },
        title: false,
        //title: {
        //    text: '<span style="color: #4572A7">INSIDE </span><span style="color:#6CAEE0"> L</span><span style="color:#FCB040">i</span><span style="color:#6CAEE0">NK</span>'
        //},
        xAxis: {
            categories: [data.study],
            title: {
                text: LoadLanguageText("NameofStudy")
            }
        },
        yAxis: {
            min: 0,
            gridLineWidth: 0,
            minorGridLineWidth: 0,
            showLastLabel: false,
            reversed: false,
            title: {
                text: LoadLanguageText("RespondentsNumber")
            },
            align: 'high'
        },
        tooltip: {
            //backgroundColor: '#FCFFC5',
            borderColor: '#FCB040',
            //borderRadius: 10,
            //borderWidth: 3,
            formatter: function () {
                return '<b>' + LoadLanguageText("TextToolTip") + '  ' + this.point.name + '</b> : ' + this.y;
            }
        },
        legend: {
            layout: 'horizontal',
            align: 'center',
            verticalAlign: 'bottom',
            floating: false,
            borderWidth: 1,
            backgroundColor: '#FFFFFF',
            shadow: true,
            labelFormatter: function () {
                return '<div class="' + this.name + '-arrow"></div><span style="font-family: \'Advent Pro\', SegoeUILight; font-size:12px">' + this.name + '</span><br/>';
            }

        },
        scrollbar: {
            enabled: true
        },
        plotOptions: {
            series: {
                stacking: 'normal',
                showInLegend: false,
                colorByPoint: true,
                shadow: true
                // pointWidth: PointWidth
            },
            bar: {
                dataLabels: {
                    enabled: false
                }
            }
        },
        series: [{
            data: processedstudy_json
        }]
    });
}



/*This is using for save the order of the home page*/
function saveDivOrder(order) {
    $.ajax({
        type: "POST",
        async: false,
        url: "/Handlers/HomeHandler.ashx/SaveOrder",
        data: "Method=SaveOrder&order=" + order,
        contentType: "application/x-www-form-urlencoded"
    });

}

/*This is handling the Click event for the mostrecent charts*/
function savedReportsClickJS(elem) {
    var src = $(elem).attr("Source");

    var sourceId = $(elem).attr("id")
    var loc;

    $.ajax({
        type: "POST",
        async: false,
        url: "/Handlers/HomeHandler.ashx/SavedReportClick",
        data: "Method=SavedReportClick&data=" + sourceId,
        contentType: "application/x-www-form-urlencoded",
        success: function (r) {

            loc = r;
        }



    });


    return loc;
}
function saveChartType(charType) {
    $.ajax({
        type: "POST",
        async: false,
        url: "/Handlers/HomeHandler.ashx/ChartTypeSave",
        data: "Method=ChartTypeSave&data=" + charType,
        contentType: "application/x-www-form-urlencoded"

    });
}


