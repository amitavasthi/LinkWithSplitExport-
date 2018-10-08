<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="LinkManager.aspx.cs" Inherits="LinkOnline.Pages.LinkManager.LinkManager" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
     <script src="../../Scripts/D3JS/d3.min.js"></script>
    <script src="../../Scripts/D3JS/d3.tip.v0.6.3.js"></script>
    <script src="../../Scripts/Ajax.js"></script>
    <script src="../../Scripts/Main.js"></script>
    <script src="../../Scripts/PageResize.js"></script>
    <style>
        path.slice {
            stroke-width: 2px;
            cursor: pointer;
        }

        polyline {
            opacity: 1;
            stroke: #000;
            stroke-width: 1px;
            fill: none;
            display: block;
        }

        .circle .ToolTip {
            display: none;
        }

        .circle:hover .ToolTip {
            display: block;
        }

        .title {
            color: #000000;
            font-size: 16pt;
        }

        .DataLabelValue {
            fill: black;
            display: none;
        }

        .BtnPreviousData {
            height: 20px;
            cursor: pointer;
            margin-left: 30px;
            margin-top: 10px;
            float: left;
            position: absolute;
        }

        .exportBtn {
            font: 10px SegoeUIBold;
            padding: 3px;
            margin-left: 3px;
        }

        svg {
            background-color: #ffffff;
            height: 100%;
            overflow: visible;
        }

        #drop_down {
            position: relative;
            top: 0%;
            left: 90%;
        }

        .minheight {
            min-height: 500px;
        }

        .tooltip {
            line-height: 1;
            padding: 10px;
            background: rgba(0, 0, 0, 0.8);
            color: #fff;
            border-radius: 2px;
            position: absolute;
            text-align: left;
            text-anchor: start;
            text-overflow: ellipsis;
            display: none;
            top: 42%;
            left: 47%;
            z-index: 10;
        }

        .Content {
            overflow-y: hidden !important;
        }
        .ButtonLinking {
            height: 40px;
            width: auto;
            border: 1px solid #41719C;
            font-size: 11pt;
        }
        .BoxContent {
            min-width:150px;
        }
    </style>
    <script type="text/javascript">
        function fnCreateLoad() {
            if (Page_ClientValidate()) {
                ShowLoading(document.getElementById("boxUploadAugment"));
            }
            else {
                return false;
            }
        }

    </script> 
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <div style="margin:1em">
      <table style="width:98%;margin:auto;">
          <tr>
              <td style="float:left;">
                   <wu:Button ID="btnUploadAugment" runat="server" CssClass="ButtonLinking" Name="UploadAugment" OnClick="btnUploadAugment_Click" />
                    <wu:Button ID="btnDownloadAugment" runat="server"  CssClass="ButtonLinking" Name="DownloadAugment" OnClick="btnDownloadAugment_Click" />
                    <wu:Button ID="btnDownloadUnLinkedVariables"  CssClass="ButtonLinking" runat="server" Name="UnlinkedVaraiables" OnClick="btnDownloadUnLinkedVariables_Click" />                 
              </td>
              <td style="float:right;">
                  <wu:Button ID="btnDelete" runat="server"  CssClass="ButtonLinking" Name="DeleteLinking" OnClick="btnDelete_Click" />
                  <wu:Button ID="btnReset" runat="server"  CssClass="ButtonLinking" Name="ResetLinking" OnClick="btnReset_Click" />
                  <wu:ConfirmBox ID="cbReset" runat="server"></wu:ConfirmBox>
              </td>
          </tr>
         <tr>
             <td colspan="2">
                 <div>
                       <span class="Color1" style="font-size:18pt;">
                            <asp:Label ID="lblLinkedVariableCount" runat="server"></asp:Label>
                        </span>
                        <span class="Color2" style="font-size:15pt;">
                            linked variables
                        </span>
                        <br />
                        <span class="Color1" style="font-size:18pt;">
                            <asp:Label ID="lblUnlinkedVariableCount" runat="server"></asp:Label>
                        </span>
                        <span class="Color2" style="font-size:15pt;">
                            unlinked variables
                        </span>
                        <br />
                        <span class="Color1" style="font-size:18pt;">
                            <asp:Label ID="lblLinkedCategoryCount" runat="server"></asp:Label>
                        </span>
                        <span class="Color2" style="font-size:15pt;">
                            linked categories
                        </span>
                        <br />
                        <span class="Color1" style="font-size:18pt;">
                            <asp:Label ID="lblUnlinkedCategoryCount" runat="server"></asp:Label>
                        </span>
                        <span class="Color2" style="font-size:15pt;">
                            unlinked categories
                        </span>
                 </div>
             </td>
         </tr>
          <tr>
              <td colspan="2" align="center">
                 <%--  <wu:Button ID="btnManualLinking" runat="server" Name="ManualLinking" OnClick="btnManualLinking_Click"></wu:Button>--%>
                   <wu:Button ID="btnAutoLink" runat="server" Name="LinkVariables" CssClass="ButtonLinking" OnClick="btnAutoLink_Click"></wu:Button>
              </td>
          </tr>
            <tr>
              <td colspan="2" align="center">
                   <div id="chartContainer" class="minheight" style="margin:auto;">
                <div id="ChartTitle" class="ChartTitle" style="display:none">
                    Test
                </div>
                        <svg id="pieChartSVG" style="background-color:#ffffff;font-family:Segoe UI;">
                            <defs>
                                <filter id='pieChartInsetShadow'>
                                    <feOffset dx='0' dy='0' />
                                    <feGaussianBlur stdDeviation='3' result='offset-blur' />
                                    <feComposite operator='out' in='SourceGraphic' in2='offset-blur' result='inverse' />
                                    <feFlood flood-color='black' flood-opacity='1' result='color' />
                                    <feComposite operator='in' in='color' in2='inverse' result='shadow' />
                                    <feComposite operator='over' in='shadow' in2='SourceGraphic' />
                                </filter>
                                <filter id="pieChartDropShadow">
                                    <feGaussianBlur in="SourceAlpha" stdDeviation="3" result="blur" />
                                    <feOffset in="blur" dx="0" dy="3" result="offsetBlur" />
                                    <feMerge>
                                        <feMergeNode />
                                        <feMergeNode in="SourceGraphic" />
                                    </feMerge>
                                </filter>
                            </defs>
                        </svg>
                    </div>
                    <script type="text/javascript">

                        var svg;
                        var pie;
                        var arc;
                        var outerArc;
                        var key;
                        var color;
                        var width, height, radius;
                        var tip;
                        var sumValue = 0;
                        var pi = Math.PI;
                        var chartData = [];
                        var colors = [];

                        if ($(window).height() <= 665) {
                            var chartHeight = document.getElementById("chartContainer").offsetHeight - 100;
                        }
                        else {
                            var chartHeight = document.getElementById("chartContainer").offsetHeight + 100;
                        }
                        var chartWidth = document.getElementById("chartContainer").offsetWidth - 300;
                        var margin = { top: 10, right: 20, bottom: 30, left: 50 },
                                      width = chartWidth,
                                      height = chartHeight - margin.bottom;


                        var chartContainer = document.getElementById("chartContainer");
                        var svgheight = height;
                        svg = d3.select("#chartContainer")
                        .select('svg')
                           .attr("width", width)
                          .attr("height", height)
                        .append("g")
                            .attr("width", width)

                        svg.append("g")
                             .attr("transform", "translate(" + 0 + "," + 20 + ")")
                            .attr("class", "slices");
                        svg.append("g")
                            .attr("class", "labels")
                         .attr("transform", "translate(" + 0 + "," + 20 + ")");
                        svg.append("g")
                            .attr("class", "lines")
                         .attr("transform", "translate(" + 0 + "," + 20 + ")");


                        radius = Math.min(width * 0.8, height * 0.8) / 2;

                        chartContainer.style.height = (height) + "px";
                        chartContainer.style.width = (width - 20) + "px";

                        pie = d3.layout.pie()
                         .sort(null)
                         .value(function (d) {
                             return d.value;
                         });


                        arc = d3.svg.arc()
                            .outerRadius(radius * 0.8)
                            .innerRadius(radius * 0.4);
                        //var arc = d3.svg.arc()
                        //    .innerRadius(0)
                        //    .outerRadius(200);

                        if ($(window).width() <= 1207) {
                            outerArc = d3.svg.arc()
                            .innerRadius(radius * 0.8)
                            .outerRadius(radius * 0.8);
                        }
                        else {
                            outerArc = d3.svg.arc()
                                .innerRadius(radius * 0.785)
                                .outerRadius(radius * 0.785);
                        }


                        svg.attr("transform", "translate(" + width / 2 + "," + Math.min(width * 0.8, height * 0.8) / 2 + ")");




                        $.post("/Handlers/AutoLink.ashx", { Method: "GetChartDetails" })
                                  .done(function (response) {
                                      pData = JSON.parse(response);
                                      for (var i = 0; i < pData.length; i++) {
                                          chartData.push(pData[i]);
                                          colors.push(pData[i].color);
                                      }

                                      var tooltip = d3.select('#chartContainer')
                                                    .append('div')
                                                    .attr("dy", ".35em")
                                                    .style("text-anchor", "middle")
                                                    .attr('class', 'tooltip');
                                      tooltip.append('div')
                                           .attr('class', 'percent');

                                      var legendWidth = 0;
                                      var xSpacing = 30;
                                      var ySpacing = 20;
                                      for (var i = 0; i < chartData.length; i++) {
                                          if (chartData[i].value == 0)
                                              continue;

                                          legendWidth += chartData[i].label + " : " + chartData[i].value;
                                          legendWidth += xSpacing;
                                      }

                                      var lineCount = legendWidth / chartContainer.style.width;

                                      var legendX = 0;
                                      var legendY = 0;

                                      if ($(window).height() <= 665) {
                                          var legendXOffset = (width / -2.5) + 5;
                                      }
                                      else {
                                          var legendXOffset = (width / -4) + 5;
                                      }
                                      var legendYOffset = height / 2;
                                      legendYOffset -= 30;//ySpacing * (lineCount + 1);

                                      svg.append("g").attr("class", "legends").attr("transform", function () {
                                          return "translate(" + 0 + "," + legendYOffset + ")";
                                      });

                                      legendYOffset = 0;

                                      var legend = svg.select(".legends").selectAll(".legend")
                                                .data(chartData)
                                                .enter().append("g")
                                                .attr("class", "legend")
                                                .attr("transform", function (d, i) {
                                                    var x = legendX;
                                                    var y = legendY;

                                                    //legendX += chartData[i].dimension + " : " + chartData[i].value + xSpacing;
                                                    legendX += 200;

                                                    if (legendX > chartContainer.style.width) {
                                                        legendX = 0;
                                                        legendY += ySpacing;

                                                        x = legendX;
                                                        y = legendY;

                                                        legendX += (chartData[i].label + " : " + chartData[i].value) + xSpacing;
                                                    }

                                                    return "translate(" + (x + legendXOffset) + "," + (y + legendYOffset) + ")";
                                                });

                                      legend.append("circle")
                                          .attr("y", -5)
                                          .attr("r", 5)
                                          .attr("width", 12)
                                          .style("opacity", function (d, i) { if ((chartData[i].value != 0) && (chartData[i].length != 0)) { return 1 } else { return 0 } })
                                          .attr("height", 12)
                                          .style("fill", function (d, i) {
                                              if (d.value != 0) {
                                                  return chartData[i].color;
                                              } else { return ''; }
                                          });

                                      var pieColor;
                                      legend.append("text")
                                          .attr("x", 15)
                                          .attr("dy", ".30em")
                                          .style("text-anchor", "start")
                                          .attr("font-size", "10pt")
                                          .attr("text-anchor", "right")
                                          .text(function (d, i) {
                                              if (chartData[i].value != 0) {
                                                  if (chartData[i].label.length != 0) {
                                                      return htmlDecode(chartData[i].label + " : " + chartData[i].value);
                                                  }
                                              }
                                          })
                                          .on("mouseover", function (d, i) {
                                              pieColor = $("#slice" + i).css("fill");
                                              document.getElementById("slice" + i).style.fill = '#444444';
                                          })
                                          .on("mouseout", function (d, i) {
                                              document.getElementById("slice" + i).style.fill = pieColor;
                                          });



                                      var slice = svg.select(".slices").selectAll("path.slice")
                                                     .data(pie(chartData));
                                      slice.enter()
                                      .insert("path")
                                      .style("fill", function (d, i) {
                                          return chartData[i].color;
                                      })
                                      .on("mouseover", function (d, i) {
                                          this.style.fill = '#444444';
                                          document.getElementById("DataLabelValue" + i).style.display = "block";
                                      })
                                      .on('mouseout', function (d, i) {
                                          this.style.fill = chartData[i].color;
                                          document.getElementById("DataLabelValue" + i).style.display = ""
                                      })
                                      .attr("class", "slice")
                                      .attr("id", function (d, i) { return "slice" + i; })
                                      .attr('d', arc)
                                      .each(function (d) {
                                          this._current = { startAngle: 0, endAngle: 0 };
                                      })
                                      slice
                                          .transition().duration(1000)
                                          .attrTween("d", function (d) {
                                              var interpolate = d3.interpolate(this._current, d);
                                              this._current = interpolate(0);
                                              return function (t) {
                                                  if (d.value != 0) {
                                                      return arc(interpolate(t));
                                                  }
                                              };
                                          })

                                      slice.exit()
                                          .remove();

                                      var text = svg.select(".labels").selectAll("text")
                                                        .data(pie(chartData));

                                      text.enter()
                                          .append("g")
                                          .attr("dy", ".35em")
                                          .style("font", "12px Segoe UI")
                                          .attr("id", function (d, i) {
                                              return "DataLabelValue" + i;
                                          })
                                         .attr("class", "DataLabelValue")
                                          .append("text")
                                          .attr("fill", "#444444")
                                          .attr("font-size", "10pt")
                                          .text(function (d, i) {
                                              if (d != undefined && d.data.value != 0) {
                                                  return (htmlDecode(chartData[i].label) + "- ") + chartData[i].value;
                                              }
                                          });


                                      function midAngle(d) {
                                          return d.endAngle - d.startAngle < 0.2;
                                      }
                                      function leftAngle(d) {
                                          var angle = (d.startAngle + d.endAngle) / 2;
                                          return angle < Math.PI;
                                      }
                                      function centerAngle(d) {
                                          var angle = (d.startAngle + d.endAngle) / 2;

                                          if (angle < (Math.PI / 4) || (angle > (((Math.PI * 2) / 4) * 3))) {
                                              return -0.05;
                                          }
                                          else if ((angle > ((Math.PI / 4) * 2))) {
                                              return 0.05;
                                          }
                                          return 0;
                                      }

                                      text.transition().duration(1000)
                                            .attrTween("transform", function (d) {

                                                if (d.value == 0)
                                                    return;

                                                this._current = this._current || d;
                                                var interpolate = d3.interpolate(this._current, d);
                                                this._current = interpolate(0);
                                                return function (t) {
                                                    var d2 = interpolate(t);
                                                    var pos = outerArc.centroid(d2);

                                                    pos[0] += radius * (leftAngle(d2) ? 0.05 : -0.05);
                                                    pos[1] += radius * centerAngle(d2);
                                                    return "translate(" + pos + ")";
                                                };

                                            })
                                    .style("font-family", "Segoe UI")
                                    .style("font-size", "12px")
                                    .styleTween("text-anchor", function (d) {
                                        if (d.value == 0)
                                            return;
                                        this._current = this._current || d;
                                        var interpolate = d3.interpolate(this._current, d);
                                        this._current = interpolate(0);
                                        return function (t) {
                                            var d2 = interpolate(t);
                                            return leftAngle(d2) ? "start" : "end";
                                        };
                                    });

                                      text.exit()
                                          .remove();


                                  });

                                </script>
              </td>
          </tr>
      </table>
          <wu:Box ID="boxDeleteLinking" runat="server" Dragable="true">
            <table>
                <tr>
                    <td>
                        <wu:Label ID="lblDeleteLinkingStudy" runat="server" Name="Study"></wu:Label>
                    </td>
                    <td>
                        <wu:DropDownList ID="ddlDeleteLinkingStudy" runat="server"></wu:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <wu:Button ID="btnDeleteLinkingConfirm" runat="server" Name="Delete" OnClick="btnDeleteLinkingConfirm_Click"></wu:Button>
                        <wu:Button ID="btnDeleteLinkingCancel" runat="server" Name="Cancel" OnClick="btnDeleteLinkingCancel_Click"></wu:Button>
                    </td>
                </tr>
            </table>
        </wu:Box>
          <wu:Box ID="boxDownloadAugment" runat="server" Dragable="true">
            <table>
                <tr>
                    <td>
                        <wu:Label ID="lblDownloadAugmentStudy" runat="server" Name="Study"></wu:Label>
                    </td>
                    <td>
                        <wu:DropDownList ID="ddlDownloadAugmentStudy" runat="server"></wu:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <wu:Button ID="btnDownloadAugmentConfirm" runat="server" Name="Download" OnClick="btnDownloadAugmentConfirm_Click"></wu:Button>
                        <wu:Button ID="btnDownloadAugmentCancel" runat="server" Name="Cancel" OnClick="btnDownloadAugmentCancel_Click" ></wu:Button>
                    </td>
                </tr>
            </table>
        </wu:Box>
        <wu:Box ID="boxDownLoadUnlinked" runat="server" Dragable="true">
            <table>
                <tr>
                    <td>
                        <wu:Label ID="lblUnlinkedVariables" runat="server" Name="Study"></wu:Label>
                    </td>
                    <td>
                        <wu:DropDownList ID="ddlUnlinkedVariables" runat="server"></wu:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <wu:Button ID="btnDownloadUnLinkedVariablesConfirm" runat="server" Name="Download" OnClick="btnDownloadUnLinkedVariablesConfirm_Click"></wu:Button>
                        <wu:Button ID="btnDownloadUnLinkedVariablesCancel" runat="server" Name="Cancel" OnClick="btnDownloadUnLinkedVariablesCancel_Click"></wu:Button>
                        </
                </tr>
            </table>
        </wu:Box>
        <wu:Box ID="boxUploadAugment" runat="server" Dragable="true">
            <table>
                <tr>
                    <td>
                        <wu:Label ID="lblUploadAugmentStudy" runat="server" Name="Study"></wu:Label>
                    </td>
                    <td>
                        <wu:DropDownList ID="ddlUploadAugmentStudy" runat="server"></wu:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <wu:Label ID="lblUploadAugmentFile" runat="server" Name="SelectFile"></wu:Label>
                    </td>
                    <td>
                        <asp:FileUpload ID="fuUploadAugmentFile" runat="server"></asp:FileUpload>
                        <asp:RegularExpressionValidator ID="regularExpressionValidator1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.xls|.xlsx)$"
                            ControlToValidate="fuUploadAugmentFile" runat="server" ForeColor="Red"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <wu:Label ID="lblUploadAugmentOverwriteExistingLinks" runat="server" Name="OverwriteExistingLinks"></wu:Label>
                    </td>
                    <td>
                        <wu:CheckBox ID="ddlUploadAugmentOverwriteExistingLinks" runat="server" Checked="true"></wu:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <wu:Button ID="btnUploadAugmentConfirm" runat="server" OnClientClick="return fnCreateLoad();" Name="Upload" OnClick="btnUploadAugmentConfirm_Click"></wu:Button>
                        <wu:Button ID="btnUploadAugmentCancel" runat="server" Name="Cancel" OnClientClick="window.location.href='LinkManager.aspx';" OnClick="btnUploadAugmentCancel_Click"></wu:Button>
                    </td>
                </tr>
            </table>
        </wu:Box>
        <wu:Box ID="boxLinkReport" runat="server" Dragable="true">
            <table>
                <tr>
                    <td>
                        <wu:Label ID="lblLinkReportErrorCountLabel" runat="server" Name="ErrorCount"></wu:Label>
                    </td>
                    <td>
                        <asp:Label id="lblLinkReportErrorCountValue" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <wu:Label ID="lblLinkReportErrorLogFileTitle" runat="server" Name="ErrorLog"></wu:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblLinkReportErrorLogFileValue" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <wu:Label ID="lblLinkReportAugmentTitle" runat="server" Name="AugmentFile"></wu:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblLinkReportAugmentValue" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <wu:Button ID="btnLinkReportClose" runat="server" Name="Close"></wu:Button>
                    </td>
                </tr>
            </table>
        </wu:Box>
    </div>
   
</asp:content>
