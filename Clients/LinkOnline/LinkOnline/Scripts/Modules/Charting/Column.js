// Override virtual function to create axis.
chart.CreateAxis = function () {
};

// Override virtual function to create axis value.
chart.CreateValue = function (animate) {
    var xtip = [];
    var ztip = [];
    var tipvalue = [];
    var barWidth = chart.x1.rangeBand();

    barSpace = parseInt(barWidth * 10 / 100);

    barWidth -= barSpace;

    if (barWidth <= 0)
        barWidth = 1;

    state = chart.svg.selectAll(".dimension")
    .data(chart.Data)
    .enter().append("g")
    .attr("class", "g")
    .attr("transform", function (d) {
        //lablesets++;
        //gvalue.push(chart.x(d.dimension));
        return "translate(" + chart.x(d.dimension) + ",1)";
    });

    state.selectAll("rect")
    .data(function (d) { return d.Values; })
    .enter()
    .append("g")
    .attr("transform", function (d, i) { return "translate(0,0)" })
    .append("rect")
    //.attr("class", "bar tool Rectbar")
    .attr("class", function (d, i) {
        return " bar tool Rectbar bars" + i;
    })
    .attr("width", barWidth)
    .attr("x", function (d, i) {
        if (!isNaN(chart.x1(d.name) + (barSpace / 2))) {
            xtip.push(chart.x1(d.name) + (barSpace / 2));
            return chart.x1(d.name) + (barSpace / 2);
        } else { xtip.push(null); }
    })
    .attr("y", function (d) {
        if (!animate) {
            if (!isNaN(d.value)) {
                var val = RoundValue(d.value) / 100;
                ztip.push(chart.y(d.value / 100));
                tipvalue.push(RoundValue(d.value) + "%");
                return RoundValue(chart.y(val));
            } else {
                ztip.push(null);
                tipvalue.push(null);
            }
        }

        return RoundValue((legendYOffset - labelsHeight));
    })
    .attr("height", function (d) {
        if (!animate) {
            if (!isNaN(legendYOffset - chart.y(d.value / 100) - labelsHeight)) {
                return RoundValue((legendYOffset - RoundValue(chart.y(RoundValue(d.value) / 100)) - labelsHeight));
            } else { return }
        }

        return 0;
    })
    .style("fill", function (d, i) {
        return chart.Data[0]["Color" + d.name];
    })
    .on('mouseover', tip.show)
    .on('mouseout', tip.hide)
    .on('click', function (d) {
        Nest(d)
    })
    .transition().delay(function (d, i) {
        if (animate == false)
            return 0;

        return i * 10;
    })
    .attr("y", function (d, i) {
        if (!isNaN(d.value)) {
            var val = RoundValue(d.value) / 100;
            ztip.push(chart.y(d.value / 100));
            tipvalue.push(RoundValue(d.value) + "%");
            return RoundValue(chart.y(val));
        } else {
            ztip.push(null);
            tipvalue.push(null);
        }
    })
    .attr("height", function (d) {
        if (!isNaN(legendYOffset - y(d.value / 100) - labelsHeight)) {
            return RoundValue((legendYOffset - RoundValue(y(RoundValue(d.value) / 100)) - labelsHeight));
        } else { return }
    })
};