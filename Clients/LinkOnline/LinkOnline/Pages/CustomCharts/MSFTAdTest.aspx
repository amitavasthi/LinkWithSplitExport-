<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="MSFTAdTest.aspx.cs" Inherits="LinkOnline.Pages.CustomCharts.MSFTAdTest" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <link rel="stylesheet" href="http://netdna.bootstrapcdn.com/bootstrap/3.3.0/css/bootstrap.min.css">
    <link rel="stylesheet" href="../../Stylesheets/BootStrap/bootstrap.vertical-tabs.min.css">
    <script src="../../Scripts/DataTable/jquery-2.1.1.min.js"></script>
    <script src="http://netdna.bootstrapcdn.com/bootstrap/3.3.0/js/bootstrap.min.js"></script>
    <link href="../../Stylesheets/BootStrap/jquery.dataTables.css" rel="stylesheet">
    <link href="../../Stylesheets/BootStrap/dataTables.fixedColumns.css" rel="stylesheet">
    <link href="../../Stylesheets/MSAdTracker/AdTracker.css" rel="stylesheet" />
    <link href="../../Stylesheets/MSAdTracker/sumoselect.css" rel="stylesheet" />
    <script src="../../Scripts/DataTable/jquery.dataTables.min.js"></script>
    <script src="../../Scripts/DataTable/dataTables.fixedColumns.js"></script>
    <script src="../../Scripts/DataTable/Ajax.js" type="text/javascript"></script>
    <script src="../../Scripts/DataTable/UnderScore.js" type="text/javascript"></script>
    <script src="../../Scripts/DataTable/jquery.sumoselect.min.js" type="text/javascript"></script>
    <script src="../../Scripts/DataTable/dataTables.tableTools.min.js" type="text/javascript"></script>
    <script src="../../Scripts/DataTable/d3.min.js" type="text/javascript"></script>

    <style>
        
    </style>
    <script>

        $(function () {
           
            var listvalues = [];
            var jSonPath = "";
            var processedData = "";
            var selCountry = JSON.parse(localStorage.getItem("country"));
            var csvdata = "";

            $("#imgCountry").attr("src", "../../Images/CountryImages/" + selCountry + ".jpeg");
            $("#imgCountryP").attr("src", "../../Images/CountryImages/" + selCountry + ".jpeg");          

            d3.csv("../../MSADTracker/csv/Slide1csvv3.txt", function (d) {
                
                processedData = _.filter(d, function (obj) {
                    if (obj.Country == selCountry && obj.base != 0) return true;
                });

                var ADNames = [];
                $.each(processedData, function (index, value) {
                    if ($.inArray(value.ADName, ADNames) === -1) {
                        ADNames.push(value.ADName);
                    }
                });

                $.each(ADNames, function (i) {
                    var optionhtml = '<option value="' +
                   processedData[i].ADName + '">' + processedData[i].ADName + '</option>';
                    $("#adnames").append(optionhtml);
                });
                //This is for multiselect
                $('.SlectBox').SumoSelect({ selectAll: true });
                $('select.SlectBox')[0].sumo.selectAll();
                processM(processedData);
            });

            $("ul.nav-tabs > li > a").on("shown.bs.tab", function (e) {
                var id = $(e.target).attr("href").substr(1);
                if (id == "Methodology") {
                    if ($.fn.dataTable.isDataTable("#tableP")) {
                        listvalues = [];
                        $("#pTable").remove();
                        $("#divTableP").append("<div id='pTable'></div>");
                    }
                   
                }
                else if (id == "placemat") {
                   
                    PlaceMate(selCountry);
                }

            });

        });

        //For the second slide
        function PlaceMate(selCountry) {

            d3.csv("../../MSADTracker/csv/Slide2csvV3.txt", function (d) {
                var listvalues = [];
                var processedData = _.filter(d, function (obj) {
                    if (obj.Country == selCountry && obj.base != 0) return true;
                });

                if ($.fn.dataTable.isDataTable("#tableP")) {
                    listvalues = [];
                    $("#pTable").remove();
                    $("#divTableP").append("<div id='pTable'></div>");
                }

                var selected = $("#adnames option:selected");
                selected.each(function () {
                    listvalues.push($(this).text());
                });

                var adNamesToString = listvalues.join();
                var s = adNamesToString.split(',');
                var placemateData = _.filter(processedData, function (obj) {
                    if (obj.ADName == s[0] || obj.ADName == s[1] || obj.ADName == s[2] || obj.ADName == s[3]
                        || obj.ADName == s[4] || obj.ADName == s[5] || obj.ADName == s[6] || obj.ADName == s[7]
                        || obj.ADName == s[8] || obj.ADName == s[9] || obj.ADName == s[10]
                        || obj.ADName == s[11] || obj.ADName == s[12] || obj.ADName == s[13] || obj.ADName == s[14]
                        || obj.ADName == s[15] || obj.ADName == s[16] || obj.ADName == s[17] || obj.ADName == s[18] || obj.ADName == s[19] || obj.ADName == s[20]
                        || obj.ADName == s[21] || obj.ADName == s[22] || obj.ADName == s[23] || obj.ADName == s[24] || obj.ADName == s[25] || obj.ADName == s[26]
                        || obj.ADName == s[27] || obj.ADName == s[28] || obj.ADName == s[29] || obj.ADName == s[30] || obj.ADName == s[31] || obj.ADName == s[32]
                        || obj.ADName == s[33] || obj.ADName == s[34] || obj.ADName == s[35] || obj.ADName == s[36] || obj.ADName == s[37] || obj.ADName == s[38] || obj.ADName == s[39] || obj.ADName == s[40])
                        return true;
                });

                ProcessPlaceMateTable(placemateData);

            });
        }

        function ProcessPlaceMateTable(placemateData) {
            if (!jQuery.isEmptyObject(placemateData)) {
                var uniqearry = [];
                var attentionarry = [];
                var Memorablearry = [];
                var adrecalluniq = parseFloat(0);
                var adrecallattent = 0;
                var adrecallMemor = 0;
                var avarage = 0;
                var s = 0;
                $("#pTable").append("<table id='tableP'  class='table compact' border='0' width='100%'></table>")
                $("#tableP").append("<thead><tr id='thP'></tr></thead>");
                $("#tableP").append("<tbody id='bodyP'></tbody>");

                $("#thP").append("<th class='th1'></th>");
                $("#thP").append("<th class='th2'></th>");
                $("#thP").append("<th class='th3'>4SEG Average</th>");
                for (var j = 0; j < placemateData.length; j++) {
                    if (placemateData[j].response === "Unique") {
                        $("#thP").append("<th class=" + "th2" + ">" + placemateData[j].ADName + "</th>");
                    }
                }
                var adSegAvg = (parseFloat(placemateData[0].fSeg) + parseFloat(placemateData[1].fSeg) + parseFloat(placemateData[2].fSeg))/3;
                $("#bodyP").append("<tr id ='adrp'><td  class='tdClassHead'>Ad Recall Potential</td><td></td><td id='avg' class='tdClass'>" + Math.round(adSegAvg) + "</td></tr>");
                $("#bodyP").append("<tr id='uniq'><td></td><td class='tdClassHead'>% Unique</td><td id='uSeg'class='tdClass'>" + Math.round(placemateData[0].fSeg) + "</td></tr>");
                $("#bodyP").append("<tr id='Attention'><td></td><td class='tdClassHead'>% Attention-grabbing</td><td id='auSeg' class='tdClass'> " +Math.round(placemateData[1].fSeg) + "</td></tr>");
                $("#bodyP").append("<tr id='Memorable'><td></td><td class='tdClassHead'>% Memorable</td><td id='mSeg' class='tdClass'> " + Math.round(placemateData[2].fSeg) + "</td></tr>");
              
                $("#bodyP").append("<tr id ='SI'><td class='tdClassHead'>Interest Trace</td><td class='tdClassHead'>Seconds to 60% Interest</td><td class='tdClass' id='sSeg'></td></tr>");
                $("#bodyP").append("<tr id ='PI'><td></td><td class='tdClassHead'>Peak Interest</td><td class='tdClass' id='pISeg'>" + placemateData[2].fSeg + "</td></tr>");

               // $("#bodyP").append("<tr id ='pIntent'><td></td><td class='tdClassHead'>Purchase Intent</td><td class='tdClass' id='pISeg'>" + placemateData[2].fSeg + "</td></tr>");


                for (var z = 0; z < placemateData.length; z++) {

                    if (placemateData[z].response === "Unique") {
                        $("#uniq").append("<td class='tdClass'>" + Math.round(placemateData[z].value) + "</td>");
                        adrecalluniq = placemateData[z].value;
                    }

                    if (placemateData[z].response === "Attention-grabbing") {
                        $("#Attention").append("<td class='tdClass'>" + Math.round(placemateData[z].value) + "</td>");
                        adrecallattent = placemateData[z].value;
                    }
                    if (placemateData[z].response === "Memorable") {
                        $("#Memorable").append("<td class='tdClass'>" + Math.round(placemateData[z].value) + "</td>");
                        adrecallMemor = placemateData[z].value;
                    }
                    averag = Math.round((parseFloat(adrecalluniq) + parseFloat(adrecallattent) + parseFloat(adrecallMemor)));
                    if (s == 2) {
                        if (placemateData[z].response === "Memorable") {
                            $("#adrp").append("<td class='tdClass'>" + Math.round(parseFloat(averag) / 3) + "</td>");
                        }
                        s = 0;
                        avarage = 0;
                    } else { s++; }


                    if (placemateData[z].variablename == "Interest Trace") {
                        if (placemateData[z].response == "Seconds to 60% Interest") {
                            $("#SI").append("<td class='tdClass'>" + placemateData[z].value + "</td>");
                            $("#sSeg").html(Math.round(placemateData[z].fSeg));
                        }
                        if (placemateData[z].response == "Peak Interest") {
                            $("#PI").append("<td class='tdClass'>" + placemateData[z].value + "</td>");                          
                            $("#pISeg").html(Math.round(placemateData[z].fSeg));
                        }
                    }
                    //if (placemateData[z].variablename == "Purchase Intent") {
                    //    alert(placemateData[z].response);
                    //    //if (placemateData[z].response == "Seconds to 60% Interest") {
                    //    //    $("#pIntent").append("<td class='tdClass'>" + placemateData[z].value + "</td>");
                    //    //}
                        
                    //}
                }



                var ptable = $('#tableP').DataTable({
                    scrollY: '440px',
                    scrollX: true,
                    scrollCollapse: false,
                    paging: false,
                    ordering: false,
                    destroy: true,
                    bFilter: false,
                    info: false,
                    "dom": '<"top"lf><"bottom"tip><"clear">'
                });
                new $.fn.dataTable.FixedColumns(ptable, {
                    leftColumns: 2,
                });
            }
        }


        function processM(processedData) {
            ProcessMTable(processedData);
            $("#adnames").change(function () {
                changeMReport(processedData);
            });
        }

        function changeMReport(processedData) {
            /*Condition for checking whether the dataTable exist*/
            if ($.fn.dataTable.isDataTable("#example")) {
                listvalues = [];
                $("#dTable").remove();
                $("#divTable").append("<div id='dTable'></div>");
            }

            var selected = $("#adnames option:selected");
            selected.each(function () {
                listvalues.push($(this).text());
            });

            var adNamesToString = listvalues.join();
            var s = adNamesToString.split(',');

            var processedAdData = _.filter(processedData, function (obj) {
                if (obj.ADName == s[0] || obj.ADName == s[1] || obj.ADName == s[2] || obj.ADName == s[3]
                    || obj.ADName == s[4] || obj.ADName == s[5] || obj.ADName == s[6] || obj.ADName == s[7]
                    || obj.ADName == s[8] || obj.ADName == s[9] || obj.ADName == s[10]
                    || obj.ADName == s[11] || obj.ADName == s[12] || obj.ADName == s[13] || obj.ADName == s[14]
                    || obj.ADName == s[15] || obj.ADName == s[16] || obj.ADName == s[17] || obj.ADName == s[18] || obj.ADName == s[19] || obj.ADName == s[20]
                    || obj.ADName == s[21] || obj.ADName == s[22] || obj.ADName == s[23] || obj.ADName == s[24] || obj.ADName == s[25] || obj.ADName == s[26]
                    || obj.ADName == s[27] || obj.ADName == s[28] || obj.ADName == s[29] || obj.ADName == s[30] || obj.ADName == s[31] || obj.ADName == s[32]
                    || obj.ADName == s[33] || obj.ADName == s[34] || obj.ADName == s[35] || obj.ADName == s[36] || obj.ADName == s[37] || obj.ADName == s[38] || obj.ADName == s[39] || obj.ADName == s[40])
                    return true;
            });

            ProcessMTable(processedAdData);
        }
        function ProcessMTable(jSonData) {
            if (!jQuery.isEmptyObject(jSonData)) {
                var result = jSonData;
                // $("#dTable").append("<table id='example' border='0' width='100%'></table>")
                $("#dTable").append("<table id='example' class='table compact' cellspacing='0' width='100%'></table>")
                $("#dTable").append("<table id='example' border='0' width='100%'></table>")
                $("#example").append("<thead><tr id='tblth' class='trClass'></tr></thead>");
                $("#example").append("<tbody id='tBody'><tr id = 'nSize' class='trClass'></tr></tbody>");

                $("#tBody").append("<tr id = 'MoE' class='trClass'></tr>");

                $("#tblth").append("<th class='th1'></th>");
                if (result != " ") {
                    for (var j = 0; j < result.length; j++) {
                        if (result[j].variablename === "N Size") {
                            $("#tblth").append("<th class=" + "th2" + ">" + result[j].ADName + "</th>");
                        }
                    }

                    $("#nSize").append("<td class='tdClassHead'>" + result[0].variablename + "</td>");
                    for (var base = 0; base < result.length; base++) {
                        if (result[base].variablename === "N Size") {
                            $("#nSize").append("<td class='tdClass'>" + Math.round(result[base].base) + "</td>");
                        }
                    }
                }
                $("#MoE").append("<td class='tdClassHead'>MoE at the 90% confidence level</td>");
                for (var base = 0; base < result.length; base++) {
                    if (result[base].variablename === "MoE") {
                        $("#MoE").append("<td class='tdClass'>±" + result[base].percentage + "%</td>");
                    }
                }

                //var table = $('#example').DataTable({
                //    scrollY: "340px",
                //    scrollX: true,
                //    scrollCollapse: false,
                //    paging: true,

                //    "iDisplayLength": 50,
                //    "sPaginationType": "full_numbers",
                //    // "dom": 'T<"clear">rfltip',
                //    "oLanguage": {
                //        "sSearch": "Filter records:"
                //    },

                //    "dom": '<"top"lf><"bottom"tip><"clear">',
                //    // "tableTools": {

                //    //     "sSwfPath": "/swf/copy_csv_xls_pdf.swf"
                //    // },
                //    columnDefs: [{
                //        // "visible": false,
                //        width: 300,
                //        "targets": [2]
                //    }
                //    ]
                //});

                var tbl = $('#example').DataTable({
                    scrollY: '340px',
                    scrollX: true,
                    scrollCollapse: false,
                    paging: false,
                    ordering: false,
                    bFilter: false,
                    info: false,
                    destroy: true,
                    "dom": '<"top"lf><"bottom"tip><"clear">'
                });

                new $.fn.dataTable.FixedColumns(tbl, {
                    leftColumns: 1
                });
            }
        }

    </script>

</head>
<body>

    <div class="container" style="width: 100%;">
        <div class="row  noMargin" style="padding-left: 0px; padding-right: 0px;">
            <div class="col-md-12">
                <div class="col-md-2" style="float: left;">
                    <!-- required for floating -->
                    <!-- Nav tabs -->
                    <ul class="nav nav-tabs tabs-left">
                        <li class="active"><a href="#Methodology" data-toggle="tab">Methodology</a></li>
                        <li><a href="#placemat" data-toggle="tab">Placemat Summary</a></li>
                        <li><a href="#messages" data-toggle="tab">Messages</a></li>
                        <li><a href="#settings" data-toggle="tab">Settings</a></li>
                    </ul>
                </div>

                <div class="col-md-10">
                    <!-- Tab panes -->
                    <div class="tab-content">
                        <div class="tab-pane active" id="Methodology" style="margin-top: 10px;">
                            <div style="width: 100%; height: 30px;" class="AdTracker">
                                <div class="Section Heading" style="width: 48%;">
                                    Methodology
                                </div>
                                <div class="Section" style="margin-right: 10px; width: 48%;">
                                    <div style="float: right; margin-right: 50px;">
                                        <img id="imgCountry" height="50px" width="100px"><br />
                                        <span class="labelClass">4SEG</span>
                                    </div>
                                </div>
                            </div>
                            <div style="height: 250px; width: 100%;" class="AdTracker">
                                <div style="width: 50%;" class="Section">
                                    <h1 class="Color1">Sample Criteria</h1>
                                    <div class="CriteriaText">
                                        18 years old +, 4SEG Audience<br />
                                        4SEG – Windows Segments<br />
                                        <ul class="ulClass">
                                            <li>Tech-setter </li>
                                            <li>Cultural Curator</li>
                                            <li>Productive Practical </li>
                                            <li>Mainstreamer </li>
                                        </ul>
                                        Representative mix of age/gender<br />
                                        Cannot reject Windows Phone
                                    </div>
                                </div>
                                <div style="width: 50%;" class="Section">
                                    <h1 class="Color1">Survey Structure </h1>
                                    <%-- <div class="CriteriaText" id="surveystructure">Respondents evaluated 1 of<span id="adCount"></span><span id="adCategory" style="font-weight: bold;"></span> ad concepts</div>
                                    <div class="SectionContent">
                                        <ul id="ssAd" class="ulClass"></ul>
                                    </div>--%>
                                    <div style="margin-bottom: 10px;">
                                        <select id="adnames" multiple="multiple" placeholder="" class="SlectBox" style="width: 300px;"></select>
                                    </div>

                                </div>
                                <%-- <div style="width: 20%;" class="Section">
                                    <h1 class="Color1">&nbsp;</h1>
                                    <input type="button" id="view" value="view report" width="100px;" />
                                </div>--%>
                            </div>
                            <div id="divTable">
                                <div id="dTable"></div>
                            </div>
                        </div>
                        <div class="tab-pane" id="placemat" style="margin-top: 10px;">
                            <div style="width: 100%; height: 30px;" class="AdTracker">
                                <div class="Section Heading" style="width: 48%;">
                                    Placemat Summary
                                </div>
                                <div class="Section" style="margin-right: 10px; width: 48%;">
                                    <div style="float: right; margin-right: 50px;">
                                        <img id="imgCountryP" height="50px" width="100px"><br />
                                        <span class="labelClass">4SEG</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div style="width: 100%; margin-bottom: 20px;" class="AdTracker">
                        </div>
                        <div id="divTableP" style="margin-top: 100px;">
                            <div id="pTable"></div>
                        </div>
                        <div class="tab-pane" id="messages">Messages Tab.</div>
                        <div class="tab-pane" id="settings">Settings Tab.</div>
                    </div>
                </div>

                <div class="clearfix"></div>

            </div>



        </div>
        <!-- /row -->
    </div>
</body>
</html>
