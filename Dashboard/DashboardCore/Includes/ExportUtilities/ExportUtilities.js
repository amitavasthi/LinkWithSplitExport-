var ExportUtilities = {
    Export: function (showLoading) {
        if (showLoading == undefined)
            showLoading = true;

        var container = undefined;
        var count = 0;

        if (showLoading) {
            container = document.createElement("div");
            var background = document.createElement("div");
            var progressContainer = document.createElement("div");
            var progress = document.createElement("div");

            container.id = "exportUtilities_Loading_Container";

            container.className = "ExportUtilities_Loading_Container";
            background.className = "ExportUtilities_Loading_Background";
            progressContainer.className = "ExportUtilities_Loading_ProgressContainer";
            progress.className = "ExportUtilities_Loading_spinner";

            progress.innerHTML = "<div class=\"ExportUtilities_Loading_bounce1\"></div><div class=\"ExportUtilities_Loading_bounce2\"></div><div class=\"ExportUtilities_Loading_bounce3\"></div>"

            progressContainer.appendChild(progress);

            container.appendChild(background);
            container.appendChild(progressContainer);

            document.body.appendChild(container);

            window.setTimeout(ExportUtilities.ExportAsynch, 100);
        }
        else {
            ExportUtilities.ExportAsynch();
        }
    },
    ExportAsynch: function () {
        var count = 0;
        var htmlBuilder = StringBuilder();

        htmlBuilder.Append("<html><head></head><body>");

        var dashboardTabs = $("dashboardtab");

        if (dashboardTabs.length == 0) {
            dashboardTabs = [document.body];
        }

        var tables;
        var tableRows;
        var tableCells;
        var selects;
        var styleValue;
        var hasColspan;
        var display;
        var frozen;
        var dashboardTabName;
        for (var i = 0; i < dashboardTabs.length; i++) {
            dashboardTabName = dashboardTabs[i].getAttribute("name");

            if (dashboardTabName == "" || dashboardTabName == undefined)
                dashboardTabName = "Report";

            htmlBuilder.Append("<dashboardtab name=\"" + dashboardTabName + "\">");

            display = dashboardTabs[i].style.display;
            dashboardTabs[i].style.display = "block";

            tables = dashboardTabs[i].getElementsByTagName("table");

            for (var t = 0; t < tables.length; t++) {
                htmlBuilder.Append("<table>");

                frozen = false;

                tableRows = tables.item(t).getElementsByTagName("tr");

                for (var tr = 0; tr < tableRows.length; tr++) {
                    htmlBuilder.Append("<tr>");

                    hasColspan = false;

                    tableCells = tableRows.item(tr).getElementsByTagName("td");

                    for (var td = 0; td < tableCells.length; td++) {
                        htmlBuilder.Append("<td");

                        if (frozen == false && tableCells.item(td).className.indexOf("TableCellValue") != -1) {
                            htmlBuilder.Append(" freeze=\"true\"");
                            frozen = true;
                        }

                        if (tableCells.item(td).className.indexOf("TableCellValue") != -1 ||
                            tableCells.item(td).className.indexOf("TableCellLeftHeadline") != -1 ||
                            tableCells.item(td).className.indexOf("TableCellWeeklyData") != -1) {
                            htmlBuilder.Append(" border=\"rgb(0,0,0)\"");
                        }

                        if (tableCells.item(td).colSpan <= 1 && hasColspan == false) {
                            htmlBuilder.Append(" width=\"" + tableCells.item(td).offsetWidth + "\"");
                        }

                        if (tableCells.item(td).colSpan > 1) {
                            hasColspan = true;
                            htmlBuilder.Append(" colspan=\"" + tableCells.item(td).colSpan + "\"");
                        }

                        if (tableCells.item(td).getAttribute("Decimals") != undefined) {
                            htmlBuilder.Append(" decimals=\"" + tableCells.item(td).getAttribute("Decimals") + "\"");
                        }

                        if (tableCells.item(td).getAttribute("ExcelFormat") != undefined) {
                            htmlBuilder.Append(" Format=\"" + tableCells.item(td).getAttribute("ExcelFormat") + "\"");
                        }
                        else if (tableRows.item(tr).className.indexOf("Headline") != -1) {
                            htmlBuilder.Append(" Format=\"@\"");
                        }

                        styleValue = $(tableCells.item(td)).css("font-weight");

                        if (styleValue == "bold")
                            htmlBuilder.Append(" fontBold=\"true\"");

                        styleValue = $(tableCells.item(td)).css("text-decoration");

                        if (styleValue == "underline")
                            htmlBuilder.Append(" fontUnderline=\"true\"");

                        htmlBuilder.Append(" align=\"" + $(tableCells.item(td)).css("text-align") + "\"");
                        htmlBuilder.Append(" color=\"" + $(tableCells.item(td)).css("color") + "\"");
                        htmlBuilder.Append(" background=\"" + $(tableCells.item(td)).css("background-color") + "\"");

                        htmlBuilder.Append(">");

                        selects = tableCells.item(td).getElementsByTagName("select");

                        for (var s = 0; s < selects.length; s++) {
                            if (selects.item(s).selectedIndex == -1)
                                continue;

                            htmlBuilder.Append(selects.item(s).options[selects.item(s).selectedIndex].innerHTML);
                        }



                        styleValue = tableCells.item(td).getAttribute("ExcelExport");
                        if (styleValue != undefined) {
                            if (styleValue == "NumberOnly") {
                                htmlBuilder.Append(parseFloat(ExportUtilities.GetInnerText(tableCells.item(td).innerText)));
                            }
                        }
                        else if (tableCells.item(td).getAttribute("ExportValue") != undefined) {
                            htmlBuilder.Append(tableCells.item(td).getAttribute("ExportValue").trim());
                        }
                        else {
                            htmlBuilder.Append(ExportUtilities.GetInnerText(tableCells.item(td).innerText));
                        }

                        htmlBuilder.Append("</td>");
                    }

                    htmlBuilder.Append("</tr>");
                }

                htmlBuilder.Append("</table>");
            }

            dashboardTabs[i].style.display = display;
            htmlBuilder.Append("</dashboardtab>");
        }
        htmlBuilder.Append("</body></html>");

        AjaxDownload(
            window.location + "&Export=True",
            encodeURIComponent(htmlBuilder.ToString()), "html"
        );

        var container = document.getElementById("exportUtilities_Loading_Container");

        if (container != undefined) {
            container.parentNode.removeChild(container);
        }
    },
    GetInnerText: function (text) {
        if (text == undefined)
            text = "";

        while (text.search("& ") != -1) {
            text = text.replace("& ", "&amp; ");
        }

        return text.trim();
    },
    Export2: function () {
        var html = "<html>";
        html += "<head>";

        var elements = $("head style");

        for (var i = 0; i < elements.length; i++) {
            html += elements[i].outerHTML;
        }

        html += "</head>";

        elements = $("*[ExportHidden=True]");

        for (var i = 0; i < elements.length; i++) {
            elements[i].style.display = "none";
        }

        elements = $("*[DynamicExportHeight]");

        for (var i = 0; i < elements.length; i++) {
            elements[i].style.height = (elements[i].offsetWidth * parseFloat(elements[i].getAttribute("DynamicExportHeight")) / 100) + "px";
        }

        elements = $("*[ExportOnly=True]");

        for (var i = 0; i < elements.length; i++) {
            if (elements[i].getAttribute("Display") != undefined)
                elements[i].style.display = elements[i].getAttribute("Display");
            else
                elements[i].style.display = "initial";
        }

        html += document.body.outerHTML;

        html += "</html>";

        AjaxDownload(
            window.location + "&Export=True",
            encodeURIComponent(html), "html"
        );

        elements = $("*[DynamicExportHeight]");

        for (var i = 0; i < elements.length; i++) {
            elements[i].style.height = "";
        }

        elements = $("*[ExportHidden=True]");

        for (var i = 0; i < elements.length; i++) {
            elements[i].style.display = "";
        }
        elements = $("*[ExportOnly=True]");

        for (var i = 0; i < elements.length; i++) {
            elements[i].style.display = "";
        }
    },
    CheckElement: function (element) {
        if (element.style != undefined) {

            if (element.style.display == "none") {
                element.parentNode.removeChild(element);
                return;
            }
        }

        if (element.childNodes) {
            for (var i = 0; i < element.childNodes.length; i++) {
                ExportUtilities.CheckElement(element.childNodes[i]);
            }
        }
    }
};