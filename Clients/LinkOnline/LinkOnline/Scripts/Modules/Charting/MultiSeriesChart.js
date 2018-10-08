chart.LegendNames = new Array();

// Override virtual void to initialize the chart.
chart.Init = function () {
    var chartContainer = document.getElementById("chartContainer");

    this.Transform.ChartWidth = chartContainer.parentNode.offsetWidth - 80;
    this.Transform.ChartHeight = chartContainer.parentNode.parentNode.offsetHeight - 100;

    if (this.Transform.ChartHeight <= 150) {
        this.Transform.ChartHeight = chartContainer.offsetHeight - 100;
    }
    if (window.location.toString().search("LinkReporter/Crosstabs.aspx") != -1)
        this.Transform.ChartHeight = window.innerHeight - 230;

    this.Transform.Margin = {
        top: 20,
        right: 50,
        bottom: 10,
        left: 80
    };

    this.Transform.Width = this.Transform.ChartWidth;
    this.Transform.Height = this.Transform.ChartHeight;

    chart.x = d3.scale.ordinal()
       .rangeRoundBands([0, this.Transform.Width - 100], .1);

    chart.x1 = d3.scale.ordinal();

    chart.y = d3.scale.linear()
    //  .range([height, 0]);

    this.Build(
        this.Source,
        this.PathDimensions,
        "Variable",
        true,
        true
    );
};

// Override virtual void to create the value tooltips.
chart.CreateTooltip = function () {
    tip = d3.tip()
        .attr('class', 'd3-tip')
        .offset([-1, 0])
            .html(function (d, i) {
                return $.trim(htmlDecode(d.name.slice(1, d.name.length))) + ": " + RoundValue(d.value) + "%";
            })
    chart.svg.call(tip);
}

// Override virtual void to build the chart.
chart.Build = function (source, pathDimensions, pathMeasures, logRequest, doAnimations, onFinish) {
    var formatPercent = d3.format(".0%");

    d3.select("svg").remove();

    if (logRequest == undefined)
        logRequest = true;

    if (logRequest) {
        this.DataRequestHistory.push({
            Source: source,
            PathDimensions: pathDimensions,
            PathMeasures: pathMeasures
        });
        this.DataRequestIndex++;
    }
    
    d3.json("/Handlers/ChartHandler.ashx?Method=GetData&NoCache=" + (new Date()).getTime() + "&Renderer=MultiSeries&Source=" + source + "&PathDimensions=" + pathDimensions + "&PathMeasures=" + pathMeasures, function (error, data) {
        
        chart.Data = data;

        chart.svg = d3.select("#chartContainer").append("svg")
            .style("font-family", "Segoe UI")
            .attr("width", chart.Transform.Width + chart.Transform.Margin.right + chart.Transform.Margin.left + 100)
            .attr("height", chart.Transform.Height + chart.Transform.Margin.top + chart.Transform.Margin.bottom + 100)
            .append("g")
            .attr("width", chart.Transform.Width)
            .attr("height", chart.Transform.Height + chart.Transform.Margin.top + chart.Transform.Margin.bottom)
            .attr("transform", "translate(" + chart.Transform.Margin.left + "," + chart.Transform.Margin.top + ")");

        chart.LegendNames = d3.keys(data[0]).filter(function (key) {
            return key !== "dimension" && key != "Label" && key.search("XPATH") == -1 && key.search("Color_") == -1 && key.search("IsDimensionPath") == -1;
        });
        chart.Labels = new Object();
        chart.Labels.Array = new Array();

        for (var i = 0; i < data.length; i++) {
            chart.Labels.Array.push(data[i].Label);
            chart.Labels[data[i].dimension] = data[i].Label;
        }
        
        // Build the chart legend.
        chart.CreateLegend();


        chart.CreateTooltip();

        data.forEach(function (d) {
            d.Values = chart.LegendNames.map(function (name) {
                return {
                    name: name,
                    value: +d[name],
                    path: d["XPATH" + name],
                    color: d["Color_" + name],
                    isDimensionPath: d["IsDimensionPath" + name],
                    dimension: d.dimension
                };
            });
        });

        /*
        var xAxis = d3.svg.axis()
            .scale(chart.x)
            .orient("bottom");

        var yAxis = d3.svg.axis()
            .scale(chart.y)
            .orient("left")
            .tickFormat(formatPercent);
        */
        var xAxis = d3.svg.axis()
            .scale(chart.x)
            .orient("bottom");

        var yAxis = d3.svg.axis()
            .scale(chart.y)
            .orient("left")
            .tickFormat(formatPercent);

        chart.x.domain(data.map(function (d) {
            return d.dimension;
        }));
        chart.x1.domain(chart.LegendNames).rangeRoundBands([0, chart.x.rangeBand()]);

        chart.y.domain([0, d3.max(data, function (d) {
            return d3.max(d.Values, function (d) {
                return RoundValue(d.value) / 100;
            });
        })]);

        var rotate = 0;

        while (true) {
            var result = true;
            for (var i = 0; i < data.length; i++) {
                var textDimensions = GetTextWidth(data[i].Label, rotate);

                if (textDimensions.width > chart.x.rangeBand()) {
                    result = false;
                    rotate -= 1;
                    break;
                }
            }

            if (result || rotate <= -90)
                break;
        }

        if (Math.abs(rotate) < 5)
            rotate = 0;

        labelsHeight = 0;
        var labelsWidth = 0;

        for (var i = 0; i < data.length; i++) {
            var textDimensions = GetTextWidth(data[i].Label, rotate);

            if (textDimensions.height > labelsHeight)
                labelsHeight = textDimensions.height;

            if (textDimensions.width > labelsWidth)
                labelsWidth = textDimensions.width;
        }

        labelsHeight += 10;

        if (labelsHeight < 40)
            labelsHeight = 40;

        chart.y.range([(legendYOffset - labelsHeight), 35]);

        /*This is for calculationg if values less than 5*/
        var maxYRange = d3.max(data, function (d) {
            return d3.max(d.Values, function (d) {
                return RoundValue(d.value) / 100;
            });
        })

        var nticks = 0;
        if (maxYRange > .07) { nticks = 10; } else { nticks = 5; }


        yAxis = d3.svg.axis()
           .scale(chart.y)
           .orient("left")
            .ticks(nticks)
           .tickFormat(formatPercent);
        /*Ends Here*/
        chart.svg.append("g")
            .attr("class", "x axis")
            .attr("id", "xAxis")
            .attr("transform", "translate(0," + (legendYOffset - labelsHeight + 2) + ")")
            .call(xAxis)
            .selectAll(".tick")
                .selectAll("text")
                .style("fill", "#000000")
                .style("font-size", "12px")
                .style("text-anchor", function () {
                    if (Math.abs(rotate) > 5)
                        return "end";
                    else
                        return "middle";
                })
                .attr("transform", function () {
                    return "rotate(" + rotate + ")";
                })
        .html(function (d) {
            return chart.Labels[d];
        });

        chart.svg.append("g")
            .attr("class", "y axis")
            .attr("id", "yAxis")
            .call(yAxis)
            .append("text")
            .attr("transform", "rotate(-90)")
            .attr("y", 6)
            .attr("dy", ".71em")
            .style("fill", "#000000")
            .style("font-size", "12pt")
            //.style("font-weight", "bold")
            .style("text-anchor", "end")
            .text("");

        var lablesets = 0;
        var gvalue = [];

        chart.CreateValue();

    });
};

// Override virtual void to create legend.
chart.CreateLegend = function () {
    var legendWidth = 0;
    var xSpacing = 20;
    var ySpacing = 20;

    for (var i = 0; i < chart.LegendNames.length; i++) {
        legendWidth += GetTextWidth(chart.LegendNames[i]);
        legendWidth += xSpacing;
    }

    var lineCount = RoundUp(legendWidth / window.innerWidth);

    var legendX = 0;
    var legendY = 0;

    var legendXOffset = 0;
    legendYOffset = this.Transform.Height + 50;
    legendYOffset -= ySpacing * (lineCount);

    chart.svg.append("g").attr("class", "legends").attr("transform", function(){
        return "translate(" + 0 + "," + legendYOffset + ")";
    });

    var legend = chart.svg.select(".legends").selectAll(".legend")
        .data(chart.LegendNames.slice())
            .enter().append("g")
            .attr("class", "legend")
            .style("font-size", "12pt")
            .attr("transform", function (d, i) {
                var x = legendX;
                var y = legendY;

                legendX += GetTextWidth(d) + xSpacing;

                if(legendX > (window.innerWidth - 100)){
                    legendX = 0;
                    legendY += ySpacing;

                    x = legendX;
                    y = legendY;

                    legendX += GetTextWidth(d) + xSpacing;
                }

                return "translate(" + (x + legendXOffset) + "," + (y) + ")";
            });

    legend.append("circle")
        .attr("r", "5")
        .attr("width", 12)
        .attr("height", 12)
        .style("fill", function (d, i) {
            return chart.Data[0]["Color" + d];
        })
        //.style("Opacity", function (d, i) { if (($.inArray(htmlDecode(d.slice(1, d.length)), arryResult) > -1)) { if (htmlDecode(d.slice(1, d.length).length) == 0) { return 0; } else { return 1; } } else { return 0; } })
        .style("display", function (d) {
            return d == "" ? "none" : "";
        });

    var barColor;
    legend.append("text")
    .attr("x", 10)
    .attr("dy", ".35em")
    .attr("text-anchor", "start")
    .attr("class", "legendtext")
    .style("font-size", "10pt")
    .text(function (d) {
        if (htmlDecode(d.slice(1, d.length).length) == 0) {
            return;
        }
        else {
            return $.trim(htmlDecode(d.slice(1, d.length)));
        }
        /*if (($.inArray(htmlDecode(d.slice(1, d.length)), arryResult) > -1)) { 
            if (htmlDecode(d.slice(1, d.length).length) == 0) { 
                return; 
            }
            else { 
                return $.trim(htmlDecode(d.slice(1, d.length)));
            }
        }
        else { 
            return; 
        }*/
    })
    .on('mouseover', function (d, i) {
        barColor = $(".bars" + i).css("fill");
        $.each($(".bars" + i), function (a, f) {

            f.style.fill = "#444444";
            f.style.fillOpacity = "";

        })
    })
    .on('mouseout', function (d, i) {

        $.each($(".bars" + i), function (a, f) {
            f.style.fill = barColor;

        })
    });
};