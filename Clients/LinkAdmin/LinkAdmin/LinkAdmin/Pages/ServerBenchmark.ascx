<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerBenchmark.ascx.cs" Inherits="LinkAdmin.Pages.ServerBenchmark" %>



<style>
    .bubbleValue {
        display: none;
    }

    .dimple-axis text {
        font-size:12px !important;
    }


    .ServerBenchmarkDescription {
        margin:10px;
    }
</style>
<div class="ServerBenchmarkDescription">
    <wu:Label ID="lblServerBenchmarkDescription" runat="server" Name="ServerBenchmarkDescription"></wu:Label>
</div>
<div id="chartContainer">
    <svg id="chart"></svg>
</div>

<script>
    var url;
    $(document).ready(function () {

        var url = "/Scripts/Pages/ServerBenchmark/Data.json";

        d3.json(url, function (error, data) {
            if (error) throw error;
            var height = $("#ContentPanel").height() - 100;
            var width = $("#ContentPanel").width() - 50;

            var leftgap = width - 100;//(88 * width) / 100;
            var rightgap = 50;//(5 * width) / 100;
            var topgap = 50; //(15 * height) / 100;
            var bottomgap = height - 100;//(100 * height) / 100;

            var svg = d3.select("#chartContainer").select("svg").attr("height", height).attr("width", width);

            svg.append("rect")
             .attr("x", "4px")
            .attr("y", "4px")
            .attr("width", "100%")
            .attr("height", "100%")
             .style("fill", "#ffffff");

            //  alert(JSON.stringify(data))
            var chart = new dimple.chart(svg, data);
            chart.setBounds(rightgap, topgap + 10, leftgap - 20, bottomgap);
            //chart.setBounds(60, 20, width - 150, 180);

            // Add your x axis - nothing unusual here
            var x = chart.addCategoryAxis("x", "Filter1");
            // First y axis is the combination axis for revenue and profit
            var y1 = chart.addMeasureAxis("y", "Value");
            y1.showGridlines = false;
            y1.ticks = 10;
            // Use a simple line by metric for the other measures
            var lines = chart.addSeries("Label", dimple.plot.line, [x, y1]);
            var circles = chart.addSeries("Label", dimple.plot.bubble, [x, y1]);
            lines.addOrderRule(["Label"]);
            //// Do a bit of styling to make it look nicer
            lines.lineMarkers = false;
            // lines.lineWeight = 1.5;

            chart.defaultColors =
                      [new dimple.color("#98df8a"),
                        new dimple.color("#ff9896"),
                        new dimple.color("#1f77b4"),
                        new dimple.color("#ff7f0e"),
                        new dimple.color("#ffbb78"),
                        new dimple.color("#2ca02c"),
                        new dimple.color("#085897"),
                        new dimple.color("#EA5D18"),
                        new dimple.color("#E22319"),
                        new dimple.color("#655755"),
                        new dimple.color("#0E4B9C")];

            x.addOrderRule("Filter1");

            chart.addLegend(90 * width / 100, 100, (5 * width) / 100, height, "Right");
            chart.ease = "linear";
            chart.staggerDraw = true;

            x.fontFamily = "SegoeUIBold";
            y1.fontFamily = "SegoeUIBold";

            circles.afterDraw = function (shp, d) {
                var shape = d3.select(shp);
                shape.attr("r", 4);
                svg.append("text")
                    .attr("x", parseFloat(shape.attr("cx")))
                    .attr("y", parseFloat(shape.attr("cy")) - 5)
                     .attr("class", "bubbleValue")
                    .style("text-anchor", "middle")
                    .style("font-size", "10px")
                    .style("font-family", "SegoeUIBold")
                    .style("opacity", 0.6)
                    .text(Math.round(d.yValue) + '%');
            };
            chart.draw(300);
            // Add a title with some d3
            svg.append("text")
               .attr("x", chart._xPixels() + chart._widthPixels() / 2)
               .attr("y", chart._yPixels() - 5)
               .style("text-anchor", "middle")
                 .attr("class", "chartTitle fltlabel")
               .style("font-family", "SegoeUILight")
               .style("font-weight", "bold")
               .style("font-size", "18px")
               .text("LiNK server benchmark index")
        });
    });
</script>
