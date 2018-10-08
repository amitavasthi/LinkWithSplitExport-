IncludeScript("/Scripts/D3JS/d3.min.js");
IncludeScript("/Scripts/D3JS/d3.tip.v0.6.3.js");
IncludeScript("/Scripts/Ajax.js");
IncludeScript("/Scripts/Main.js");
IncludeScript("/Scripts/PageResize.js");
IncludeScript("/Scripts/JQuery/jquery-1.8.1.min.js");

// Chart object.
var chart = new Object();

chart.Transform = new Object();
chart.DataRequestHistory = new Array();
chart.DataRequestIndex = -1;

// Virtual function to initialize the chart.
chart.Init = function () {
};

// Virtual function to build the chart.
chart.Build = function (source, pathDimensions, pathMeasures, logRequest, doAnimations, onFinish) {
};

// Virtual function to create values.
chart.CreateValue = function () {
};

// Virtual function to create axis.
chart.CreateAxis = function () {
};

// Virtual void to create the value tooltips.
chart.CreateTooltip = function () {
}

// Virtual function to create legend.
chart.CreateLegend = function () {
};

function IncludeScript(src) {
    var script = document.createElement("script");
    script.src = src;

    document.head.appendChild(script);
}


function GetTextWidth(text, rotate) {
    var test = document.createElement("div");
    test.style.display = "inline-block";
    test.style.fontFamily = "Segoe UI";
    test.style.fontSize = "10pt";
    test.innerHTML = text;

    document.body.appendChild(test)
    var result = test.offsetWidth;

    if (rotate != undefined) {
        if (rotate != 0) {
            // convert degrees to radians.
            var r = rotate * 0.0174533;

            result = new Object();

            // This one doesn't work:
            //result = Math.abs(test.offsetWidth * Math.sin(r)) + Math.abs(test.offsetHeight * Math.cos(r));
            result.width = 0 + (test.offsetWidth - 0) * Math.cos(r) + (test.offsetHeight - 0) * Math.sin(r);
            result.height = 0 - (test.offsetWidth - 0) * Math.sin(r) + (test.offsetHeight - 0) * Math.cos(r);
        }
        else {
            result = { width: test.offsetWidth, height: test.offsetHeight };
        }
    }

    document.body.removeChild(test);

    return result;
}

function RoundUp(value) {
    if (value - parseInt(value) > 0.0) {
        return parseInt(value) + 1;
    }
    else {
        return parseInt(value);
    }
}
function RoundValue(value) {
    var v = 1;

    for (var i = 0; i < chart.DecimalPlaces; i++) {
        v *= 10;
    }

    return Math.round(value * v) / v;
}