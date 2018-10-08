<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="MSFTHome.aspx.cs" Inherits="LinkOnline.Pages.CustomCharts.MSFTHome" %>

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
    <script src="../../Scripts/DataTable/jquery.dataTables.min.js"></script>
    <script src="../../Scripts/DataTable/dataTables.fixedColumns.js"></script>
    <script src="../../Scripts/DataTable/Ajax.js" type="text/javascript"></script>
    <script src="../../Scripts/DataTable/UnderScore.js" type="text/javascript"></script>

    <script>

        $(function () {

            $.getJSON('../../MSADTracker/NSize.txt', function (data) {

                var processdData = _.filter(data.aaData, function (obj) {
                    // return true for every valid entry!
                    if (obj.base != 0) return true;
                });

                var country = _.chain(processdData)
                     .groupBy("Country")
                     .map(function (value, key) {
                         return {
                             Country: key,
                         }
                     })
                     .value();

                $.each(country, function (i) {
                    var optionhtml = '<option value="' +
                   country[i].Country + '">' + country[i].Country + '</option>';
                    $("#country").append(optionhtml);
                });
                // This has been change based on the new studies
                //$.each(category, function (i) {
                //    var optionhtml = "<option value='Microsoft Surface'>Microsoft Surface</option>";
                //    $("#category").append(optionhtml);
                //});

                var result = _.filter(data.aaData, function (obj) {
                    // return true for every valid entry!
                    if (obj.Country == "US" && obj.base != 0) return true;
                });

                $.each(result, function (i) {
                    var optionhtml = '<option value="' +
                   result[i].ADName + '">' + result[i].ADName + '</option>';
                    $("#adnames").append(optionhtml);
                });
                $('#adnames :first-child').attr('selected', true);

                $("#country").change(function () {

                    $('#adnames')
                     .find('option')
                     .remove()
                     .end()

                    result = _.filter(data.aaData, function (obj) {
                        // return true for every valid entry!
                        if (obj.Country == $("#country option:selected").text() && obj.base != 0) return true;
                    });
                    //$.each(result, function (i) {
                    //    var optionhtml = '<option value="' +
                    //   result[i].ADName + '">' + result[i].ADName + '</option>';
                    //    $("#adnames").append(optionhtml);
                    //});
                    //$('#adnames :first-child').attr('selected', true);
                });
            });
        });


        //function getValues() {
        //    var listvalues = [];
        //    var x = document.getElementById("adnames");
        //    for (var i = 0; i < x.options.length; i++) {
        //        if (x.options[i].selected == true) {
        //            if (listvalues != null) {
        //                listvalues.push(x.options[i].value);
        //            }
        //        }
        //    }
        //    localStorage.setItem('lists', JSON.stringify(listvalues));
        //}

        $(function () {

            $('form').on('submit', function (e) {
                localStorage.clear();
                //var listvalues = [];
                var countryName = [];
                //var x = document.getElementById("adnames");
                //for (var i = 0; i < x.options.length; i++) {
                //    if (x.options[i].selected == true) {
                //        if (listvalues != null) {
                //            listvalues.push(x.options[i].value);
                //        }
                //    }
                //}
                //localStorage.setItem('countryAdName', JSON.stringify(listvalues));

                var y = document.getElementById("country");
                for (var i = 0; i < y.options.length; i++) {
                    if (y.options[i].selected == true) {
                        if (countryName != null) {
                            countryName.push(y.options[i].value);
                        }
                    }
                }
                localStorage.setItem('country', JSON.stringify(countryName));


                e.preventDefault();

                $.ajax({
                    type: 'post',
                    url: 'MSFTAdTest.aspx',
                    data: countryName,
                    success: function () {
                        window.location.href = "MSFTAdTest.aspx";
                    }
                });

            });

        });


    </script>
    <style>
        .noMargin {
            margin-left: 0px !important;
            margin-right: 0px !important;
        }
    </style>
</head>
<body>
    <form id="frmSubmit">
        <div class="container" style="width: 100%;">
            <div class="row  noMargin" style="padding-left: 0px; padding-right: 0px;">
                <div class="col-md-12">
                    <div class="col-md-11">
                        <div class="tab-pane active" id="Methodology" style="margin-top: 10px;">
                            <div style="width: 100%; height: 30px;" class="AdTracker">
                                <div class="Section MainHeading Color1" style="width: 100%; margin-left: 300px;">
                                   <%-- One Consumer TV Ad Pre-Testing: [GEO]--%>
                                </div>
                            </div>
                            <div style="height: auto; width: 100%;" class="AdTracker">
                                <div style="width: 50%;" class="Section">
                                    <h1 class="Color1">country selection</h1>
                                    <div style="float: left; margin-left: 10px;">
                                        <span class="lblName">select country</span>
                                        <select id="country" class="ddlFont"></select>
                                    </div>
                                </div>
                                <div style="width: 50%;" class="Section">
                                    <h1 class="Color1">ad category</h1>
                                    <div style="float: left; margin-left: 10px;">
                                        <span class="lblName">select category</span>
                                        <select id="category" class="ddlFont">
                                            <option value="Microsoft Surface">Microsoft Surface</option>
                                        </select>
                                    </div>
                                </div>
                              <%--  <div style="width: 34%;" class="Section">
                                    <h1 class="Color1">ad names</h1>
                                    <select id="adnames" multiple="multiple" class="multiSelect" style="width: 300px; height: 350px;"></select>
                                </div>--%>

                            </div>

                        </div>
                    </div>
                    <div class="col-md-1">
                        <%--  <input type="button" value="Get Value" onclick="getValues()" />--%>
                        <input name="submit" type="submit" value="Submit" style="margin-top: 120px;">
                    </div>

                    <div class="clearfix"></div>
                </div>



            </div>
            <!-- /row -->
        </div>
    </form>
</body>
</html>
