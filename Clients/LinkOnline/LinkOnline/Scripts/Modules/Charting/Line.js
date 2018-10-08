var realvalues = [];

// Override virtual function to create axis value.
chart.CreateValue = function (animate) {
    var circleresultcx = new Array();
    var circleresultcy = new Array();
    var j = 0;

    var line = d3.svg.line()
        .interpolate("linear")
        .x(function (d, i) {
            circleresultcx[j] = (chart.x(d.dimension) + (chart.x.rangeBand() / 2));
            return chart.x(d.dimension) + (chart.x.rangeBand() / 2);
        })
        .y(function (d, i) {
            circleresultcy[j] = RoundValue(chart.y(RoundValue(d.value) / 100)); j++;
            return RoundValue(chart.y(RoundValue(d.value) / 100));
        });

    var results = new Array();
    var city = chart.svg.selectAll(".city")
        .data(chart.LegendNames)
        .enter().append("g")
        .attr("class", "city");

    var realcolor = [];
    var vartemp = 0;

    city.append("path")
       .attr("class", "line")
       .attr("id", function (d, i) {
           return "line" + i;
       })
       .attr("d", function (d, i) {
           //return line(d);
           if (!animate) {
               var result = new Array();
               for (var i = 0; i < chart.Data.length; i++) {
                   if (!isNaN(chart.Data[i][d])) {
                       result.push({
                           dimension: chart.Data[i].dimension,
                           value: chart.Data[i][d]
                       });
                       realvalues.push(RoundValue(chart.Data[i][d]));
                   }
               }
               return line(result);


               var result = new Array();
               for (var i = 0; i < chart.Data.length; i++) {
                   if (!isNaN(chart.Data[i][d])) {
                       result.push({
                           dimension: chart.Data[i].dimension,
                           value: chart.Data[i][d]
                       });
                       results[i] = chart.Data[i][d];
                       fetchcountries.push(d);
                   }
               }
               return _line(result);
           }
       })
        .style("stroke", function (d, i) {
            for (var i = 0; i < chart.Data.length; i++) {
                if (!isNaN(chart.Data[i][d])) {
                    realcolor.push(chart.Data[i]["Color" + d]);
                    return chart.Data[i]["Color" + d];
                    //realcolor.push(color(d));
                    //return (color(d));
                }
            }
        })
        .transition().delay(200).duration(500).attr("d", function (d, i) {
            if (animate == false)
                return 0;
            var result = new Array();
            for (var i = 0; i < chart.Data.length; i++) {
                if (!isNaN(chart.Data[i][d])) {
                    result.push({
                        dimension: chart.Data[i].dimension,
                        value: chart.Data[i][d]
                    });
                    realvalues.push(RoundValue(chart.Data[i][d]));
                }
            }

            /* For removing lines which having all the values zero  */

            $.each(result, function (i, d) {
                if (d.value == 0) {
                    if (vartemp == chart.Data.length - 1) {
                        if (i == chart.Data.length - 1) {
                            result = "";
                        }
                    }
                    else {
                        if (d.value == 0) {
                            vartemp++;
                        }
                        else {
                            vartemp = 0;
                        }
                    }
                }
            });
            vartemp = 0;
            return line(result);
        });

    var finalcirclearray = [];
    for (i = 0; i < circleresultcy.length; i++) {
        finalcirclearray.push("{cx:" + circleresultcx[i] + ",cy :" + circleresultcy[i] + "}");

    }

    /*this is to get the circle color as line color*/
    var finalcolor = [];
    var start = 0;
    for (var i = start; i < realcolor.length; i++) {
        for (var j = 1; j <= chart.Labels.Array.length; j++) {
            finalcolor.push(realcolor[i]);
        }
    }


    /*this is to append points to the line charts*/
    chart.svg.selectAll(".circle")
        .data(finalcirclearray)
        .enter()
        .append("svg:circle")
        .attr("class", "circle")
        .attr("id", function (d, i) { return i; })
        .attr("cx", function (d, i) { return circleresultcx[i] })
        .attr("cy", function (d, i) { return circleresultcy[i] })
        .attr("r", function (d, i) {
            if (!isNaN(circleresultcy[i])) {
                if (!isNaN(circleresultcx[i])) { return 3 }
            } else { return 0 }
        })
        .style("fill", function (d, i) {
            if (!isNaN(circleresultcy[i])) {
                return (finalcolor[i]);
            }
        })

    .on('mouseover', tip.show)
    .on('mouseout', tip.hide)
}

// Override virtual void to create the value tooltips.
chart.CreateTooltip = function () {
    var countlegend = 0; //legend count
    var arrylegend = [];

    tip = d3.tip()
        .attr('class', 'd3-tip')
        .offset([-1, 0])
        .html(function (d, i) {

            // for showing large tip value into multiple lines
            if (arrylegend[i] == undefined) { arrylegend[i] = ""; }

            if ((arrylegend[i] + ": " + RoundValue(realvalues[i]) + "%").length > 30) {
                var str = arrylegend[i] + ": " + RoundValue(realvalues[i]) + "%";
                var index = (str.length) / 2;
                index = (str.substr(0, index)).lastIndexOf(" "); // index += (str.length) / 2;
                return str.substr(0, index) + '<br/>' + str.substr(index);
            }
            else {
                return arrylegend[i] + ": " + RoundValue(realvalues[i]) + "%";
            }
        });

    chart.svg.call(tip);
}