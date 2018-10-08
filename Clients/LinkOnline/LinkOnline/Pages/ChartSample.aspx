<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChartSample.aspx.cs" Inherits="LinkOnline.Pages.ChartSample" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <title>Link :: Custom Charts</title>
    <style type="text/css">
        .lblName {
            font-family: "Lucida Grande", "Lucida Sans Unicode", Arial, Helvetica, sans-serif;
            font-size: 13px;
            color: #6CAEE0;
        }

        .ddlFont {
            font-family: "Lucida Grande", "Lucida Sans Unicode", Arial, Helvetica, sans-serif;
            font-size: 14px;
        }
    </style>
    <script src="../Scripts/jquery.js" type="text/javascript"></script>
    <script src="../Scripts/HighCharts/jquery.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="../Scripts/Ajax.js" type="text/javascript"></script>
    <script src="../Scripts/UnderScore.js" type="text/javascript"></script>

    <script src="../Scripts/HighCharts/highcharts.js" type="text/javascript"></script>
    <script src="../Scripts/HighCharts/exporting.js" type="text/javascript"></script>

    <script type="text/javascript">
        function sum(numbers) {
            return _.reduce(numbers, function (result, current) {
                return result + parseFloat(current);
            }, 0);
        }


        function loadData(dataSB) {
            var tot2000 = $.grep(dataSB, function (n, i) {
                return n.Label === 'Count of servers with Windows Server 2000';
            });

            /*server results grouping*/
            var result2000 = _.chain(tot2000)
                 .groupBy("Filter1")
                 .map(function (value, key) {
                     return {
                         Filter: key,
                         tot2000Base: sum(_.pluck(value, "Base")),
                         tot2000Val: sum(_.pluck(value, "Value"))
                     }
                 })
                 .value();

            var tot2003 = $.grep(dataSB, function (n, i) {
                return n.Label === 'Count of servers with Windows Server 2003';
            })

            /*server results grouping*/
            var result2003 = _.chain(tot2003)
                 .groupBy("Filter1")
                 .map(function (value, key) {
                     return {
                         Filter: key,
                         tot2003Base: sum(_.pluck(value, "Base")),
                         tot2003Val: sum(_.pluck(value, "Value"))
                     }
                 })
                 .value();

            var tot2008 = $.grep(dataSB, function (n, i) {
                return n.Label === 'Count of servers with Windows Server 2008';
            })

            /*server results grouping*/
            var result2008 = _.chain(tot2008)
                 .groupBy("Filter1")
                 .map(function (value, key) {
                     return {
                         Filter: key,
                         tot2008Base: sum(_.pluck(value, "Base")),
                         tot2008Val: sum(_.pluck(value, "Value"))
                     }
                 })
                 .value();

            var tot2012 = $.grep(dataSB, function (n, i) {
                return n.Label === 'Count of servers with Windows Server 2012';
            })

            /*server results grouping*/
            var result2012 = _.chain(tot2012)
                 .groupBy("Filter1")
                 .map(function (value, key) {
                     return {
                         Filter: key,
                         tot2012Base: sum(_.pluck(value, "Base")),
                         tot2012Val: sum(_.pluck(value, "Value"))
                     }
                 })
                 .value();


            var totLinux = $.grep(dataSB, function (n, i) {
                return n.Label === 'Count of servers with Red Hat Linux';
            })

            /*server results grouping*/
            var resultLinux = _.chain(totLinux)
                 .groupBy("Filter1")
                 .map(function (value, key) {
                     return {
                         Filter: key,
                         linuxBase: sum(_.pluck(value, "Base")),
                         linuxVal: sum(_.pluck(value, "Value"))
                     }
                 })
                 .value();

            var processed2000Json = new Array();
            var processed2003Json = new Array();
            var processed2008Json = new Array();
            var processed2012Json = new Array();
            var processedLinuxJson = new Array();
            var length = result2000.length;

            for (var i = length - 1; i >= 0; i--) {

                var calcFilter = result2000[i].Filter;
                var val2000 = result2000[i].tot2000Val;
                var val2003 = result2003[i].tot2003Val;
                var val2008 = result2008[i].tot2008Val;
                var val2012 = result2012[i].tot2012Val;
                var valLinux = resultLinux[i].linuxVal;

                var totSum = 0;
                totSum = parseFloat(val2000) + parseFloat(val2003) + parseFloat(val2008) + parseFloat(val2012) + parseFloat(valLinux);
                var avg2000 = 0;
                var avg2003 = 0;
                var avg2008 = 0;
                var avg2012 = 0;
                var avgLinux = 0;

                if (totSum != 0) {
                    avg2000 = parseFloat(((parseFloat(val2000) / parseFloat(totSum)) * 100).toFixed(2));
                    avg2003 = parseFloat(((parseFloat(val2003) / parseFloat(totSum)) * 100).toFixed(2));
                    avg2008 = parseFloat(((parseFloat(val2008) / parseFloat(totSum)) * 100).toFixed(2));
                    avg2012 = parseFloat(((parseFloat(val2012) / parseFloat(totSum)) * 100).toFixed(2));
                    avgLinux = parseFloat(((parseFloat(valLinux) / parseFloat(totSum)) * 100).toFixed(2));
                }
                var arr2000 = new Array();
                var arr2003 = new Array();
                var arr2008 = new Array();
                var arr2012 = new Array();
                var arrLinux = new Array();
                arr2000.push({ 'Filter': calcFilter, 'Value': avg2000 });
                arr2003.push({ 'Filter': calcFilter, 'Value': avg2003 });
                arr2008.push({ 'Filter': calcFilter, 'Value': avg2008 });
                arr2012.push({ 'Filter': calcFilter, 'Value': avg2012 });
                arrLinux.push({ 'Filter': calcFilter, 'Value': avgLinux });

                $.map(arr2000, function (obj, i) {
                    processed2000Json.push([obj.Filter, parseFloat(obj.Value)]);
                });
                $.map(arr2003, function (obj, i) {
                    processed2003Json.push([obj.Filter, parseFloat(obj.Value)]);
                });

                $.map(arr2008, function (obj, i) {
                    processed2008Json.push([obj.Filter, parseFloat(obj.Value)]);
                });

                $.map(arr2012, function (obj, i) {
                    processed2012Json.push([obj.Filter, parseFloat(obj.Value)]);
                });
                $.map(arrLinux, function (obj, i) {
                    processedLinuxJson.push([obj.Filter, parseFloat(obj.Value)]);
                });
                alert(processedLinuxJson);
            };
            var chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'container',
                    type: 'line',
                },
                //loading: {
                //    labelStyle: {
                //        backgroundImage: 'url("http://jsfiddle.net/img/logo.png")',
                //        display: 'block',
                //        width: '136px',
                //        height: '26px',
                //        backgroundColor: '#000'
                //    }
                //},
                title: {
                    text: $("#segment option:selected").text() + '    OS Share Trend',
                    x: -20 //center
                },
                subtitle: {
                    text: 'Source:MS Telemetry',
                    x: -20
                },
                xAxis: {
                    type: "category"
                },
                yAxis: {
                    title: {
                        text: 'OS Share'
                    },
                    plotLines: [processed2000Json, processed2003Json, processed2008Json, processed2012Json, processedLinuxJson]
                },
                tooltip: {
                    formatter: function () {
                        return this.point.name + '<br/>' + this.series.name + ' : ' + '<b>' + '<b> ' + this.y + '</b>' + ' %';
                    },
                    //valueSuffix: ' %'
                },
                legend: {
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'middle',
                    borderWidth: 0
                },
                series: [{
                    name: "WS 2000",
                    data: processed2000Json
                }
                , {
                    name: "WS 2003",
                    data: processed2003Json
                },
                {
                    name: "WS 2008",
                    data: processed2008Json
                },
                {
                    name: "WS 2012",
                    data: processed2012Json
                },
                 {
                     name: "Linux",
                     data: processedLinuxJson
                 }
                ]
            });
        }
        //url : http://localhost:54580/Handlers/LinkBiExternal.ashx?Method=ProcessReport&Dimension1=Month&Dimension2=OrgSizeSegment&Measure1=CountofServerswithWindowsServer2000&Measure2=CountofServerswithWindowsServer2003&Measure3=CountofServerswithWindowsServer2008&Measure4=CountofServerswithWindowsServer2012&Measure5=CountofServerswithRedHatLinux&ResponseType=JSON
        $(function () {

            $.getJSON('../JSON/telemetry.txt', function (data) {
                /*filtering the JSON records with individaual servers */



                var segment = _.chain(data)
                     .groupBy("Filter2")
                     .map(function (value, key) {
                         return {
                             Filter: key,
                         }
                     })
                     .value();

                //alert(JSON.stringify(segment));
                var optionhtml1 = '<option value="' + "All" + '">' + "All" + '</option>';
                $("#segment").append(optionhtml1);

                $.each(segment, function (i) {
                    var optionhtml = '<option value="' +
                   segment[i].Filter + '">' + segment[i].Filter + '</option>';
                    $("#segment").append(optionhtml);
                });


                data = _.sortBy(data, function (obj) { return obj.Filter1; });

                data = _.sortBy(data, function (obj) { return obj.Filter1.substring(obj.Filter1.length - 4, obj.Filter1.length - 7); });


                if ($("#segment option:selected").text() === "All") {
                    loadData(data);
                }


                $("#segment").change(function () {

                    if ($("#segment option:selected").text() === "All") {

                        loadData(data);
                    }
                    else {
                        dataSB = $.grep(data, function (n, i) {
                            return n.Filter2 === $("#segment option:selected").text();
                        });
                        loadData(dataSB);
                    };

                    //dropdown change
                });
            });
        });

    </script>
</head>
<body>
    <%--<div id="results" style="height: 500px; min-width: 500px"></div>--%>
    <div style="margin-left: 20px;">
        <span class="lblName">select segment</span>
        <select id="segment" class="ddlFont"></select>
    </div>
    <div id="container" style="height: 500px; min-width: 500px"></div>
</body>
</html>
