var Render = {
    ContentBox: function (title) {
        var container = document.createElement("div");
        container.className = "ContentBox";

        var headline = document.createElement("div");
        headline.className = "ContentBoxHeadline";

        headline.innerHTML = title;

        var content = document.createElement("div");
        content.className = "ContentBoxContent";

        container.appendChild(headline);
        container.appendChild(content);

        container.Content = content;

        return container;
    },
    Table: function (columns, rows) {
        var table = document.createElement("table");
        table.cellPadding = 0;
        table.cellSpacing = 0;

        var tableRow = document.createElement("tr");
        var tableCell;

        for (var i = 0; i < columns.length; i++) {
            tableCell = document.createElement("td");

            if (columns[i].Class != undefined)
                tableCell.className = columns[i].Class;

            if (columns[i].ColumnSpan != undefined)
                tableCell.colSpan = columns[i].ColumnSpan;

            if (columns[i].RowSpan != undefined)
                tableCell.rowSpan = columns[i].RowSpan;

            tableCell.innerHTML = columns[i].Text;

            tableRow.appendChild(tableCell);
        }

        table.appendChild(tableRow);

        for (var r = 0; r < rows.length; r++) {
            tableRow = document.createElement("tr");

            for (var c = 0; c < rows[r].length; c++) {
                tableCell = document.createElement("td");

                if (rows[r][c].Class != undefined)
                    tableCell.className = rows[r][c].Class;

                if (rows[r][c].ColumnSpan != undefined)
                    tableCell.colSpan = rows[r][c].ColumnSpan;

                if (rows[r][c].RowSpan != undefined)
                    tableCell.rowSpan = rows[r][c].RowSpan;

                tableCell.innerHTML = rows[r][c].Text;

                tableRow.appendChild(tableCell);
            }

            table.appendChild(tableRow);
        }

        return table;
    },
    LineBreak: function () {
        var result = document.createElement("br");

        return result;
    }
};