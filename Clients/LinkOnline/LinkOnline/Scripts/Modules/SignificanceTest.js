var hightlightedCells = new Array();

function HighlightSigDiff(sender, idTopVariable, idLeftVariable, idCategory, letters) {
    hightlightedCells = new Array();

    if (letters.length == 0)
        return;

    var identity = sender.getAttribute("SignificanceIdentity");
    var leftIdentitiy = sender.getAttribute("LeftSignificanceIdentity");

    // Get all table rows.
    var tableRows = document.body.getElementsByTagName("tr");

    // Run through all table rows.
    for (var i = 0; i < tableRows.length; i++) {
        // Check if the table row belongs to the category to highlight.
        if (tableRows.item(i).getAttribute("IdCategory") != idCategory && tableRows.item(i).getAttribute("IsTitle") != "True")
            continue;

        /*if (tableRows.item(i).getAttribute("IdVariable") != idLeftVariable)
            continue;*/

        // Get all table cells of the table row.
        var tableCells = tableRows.item(i).getElementsByTagName("td");

        // Run through all table cells.
        for (var c = 0; c < tableCells.length; c++) {
            //if (tableCells.item(c).getAttribute("IdVariable") != idTopVariable)
            //continue;
            var isTitle = tableCells.item(c).getAttribute("IsTitle") == "True";
            var leftTitle = false;

            if (isTitle == false && tableCells.item(c).getAttribute("SignificanceIdentity") != identity)
                continue;

            if (tableCells.item(c).getAttribute("LeftSignificanceIdentity") != undefined) {
                if (tableCells.item(c).getAttribute("LeftSignificanceIdentity") != leftIdentitiy)
                    continue;
                else if(isTitle)
                    leftTitle = true;
            }

            var letter = tableCells.item(c).getAttribute("SignificanceLetter");

            // Check if the table cell got a significance letter.
            if ((letter == undefined || letter == "") && leftTitle == false)
                continue;

            // Check if the table cell should be highlighted.
            if (letters.search(letter + ",") != -1 || leftTitle) {
                tableCells.item(c).style.opacity = "1.0";
                tableCells.item(c).style.backgroundColor = "#B6D7EF";
                tableCells.item(c).setAttribute("SigDiffHighlight", "True");

                var test = tableCells.item(c).getElementsByTagName("td");

                for (var t = 0; t < test.length; t++) {
                    test.item(t).style.opacity = "1.0";
                }

                hightlightedCells.push(tableCells.item(c));
            }
        }
    }

    if (hightlightedCells.length <= 5)
        return;

    var style = document.createElement("style");
    style.id = "SignificanceTestHighlighting";
    style.type = "text/css";

    style.innerHTML = ".Crosstable td { opacity:0.4 }";

    document.body.appendChild(style);

    //HighlightSigDiffDimm(1.0);
}

var dimMode;

function HighlightSigDiffDimm(opacity) {
    dimMode = "Dimming";

    if (opacity <= 0.4) {
        dimMode == undefined;

        return;
    }

    var style = document.getElementById("SignificanceTestHighlighting");

    if (style == undefined)
        return;

    style.innerHTML = ".Crosstable td:not([SigDiffHighlight='True']) { opacity:" + opacity + "; background-color:#B6D7EF; }";

    window.setTimeout(function () {
        var factor = (1.0 - opacity) * 10 / 100;

        factor = parseInt(factor * 100) / 100;

        if (factor <= 0.05)
            factor = 0.05;

        HighlightSigDiffDimm(opacity - factor);
    }, 20);
}

function DeHighlightSigDiffDimm(opacity) {
    if (dimMode == "Dimming")
        return;

    hightlightedCells = new Array();

    dimMode = "Undimming";

    if (opacity >= 1.0) {
        var style = document.getElementById("SignificanceTestHighlighting");

        if (style != undefined) {
            style.parentNode.removeChild(style);
        }

        for (var i = 0; i < hightlightedCells.length; i++) {
            hightlightedCells[i].style.opacity = "";
            hightlightedCells[i].style.backgroundColor = "";
        }

        return;
    }

    var style = document.getElementById("SignificanceTestHighlighting");

    if (style == undefined)
        return;

    style.innerHTML = ".Crosstable td { opacity:" + opacity + " }";

    window.setTimeout(function () {
        var factor = (1.0 - opacity) * 10 / 100;

        factor = parseInt(factor * 100) / 100;

        if (factor <= 0.05)
            factor = 0.05;

        DeHighlightSigDiffDimm(opacity + factor);
    }, 50);
}

function DeHighlightSigDiff() {
    var style = document.getElementById("SignificanceTestHighlighting");

    if (style != undefined) {
        style.parentNode.removeChild(style);
    }

    for (var i = 0; i < hightlightedCells.length; i++) {
        hightlightedCells[i].style.opacity = "";
        hightlightedCells[i].style.backgroundColor = "";
        hightlightedCells[i].removeAttribute("SigDiffHighlight");

        var test = hightlightedCells[i].getElementsByTagName("td");

        for (var t = 0; t < test.length; t++) {
            test.item(t).style.opacity = "1.0";
        }
    }

    //DeHighlightSigDiffDimm(0.4);
}