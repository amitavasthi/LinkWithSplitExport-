$(document).ready(function () {
    var indexOf = function (element) {
        for (var i = 0; i < element.parentNode.childNodes.length; i++) {
            if (element.parentNode.childNodes.item(i) == element)
                return i;
        }

        return -1;
    }

    var columns = $("*[ColumnFilter=True]");

    var cell;
    for (var i = 0; i < columns.length; i++) {
        var rows = columns[i].parentNode.parentNode.getElementsByTagName("tr");

        columns[i].ColumnIndex = indexOf(columns[i]);
        columns[i].className += " ColumnFilterHeader";

        var filterElements = {
            _Keys: []
        };

        columns[i].SelectedFilterElements = [];

        for (var r = 0; r < rows.length; r++) {
            try {
                if (rows.item(r).getAttribute("CellFilterIgnore") == "True")
                    continue;

                cell = rows.item(r).getElementsByTagName("td").item(columns[i].ColumnIndex);

                if (cell == columns[i])
                    continue;

                if (cell == undefined)
                    continue;

                if (filterElements[cell.innerText.trim()] == undefined) {
                    filterElements._Keys.push(cell.innerText.trim());
                    filterElements[cell.innerText.trim()] = [];
                    columns[i].SelectedFilterElements.push(cell.innerText.trim());
                }

                filterElements[cell.innerText.trim()].push(rows.item(r));


                columns[i].Update = ColumnFilter.Update;
            }
            catch (e) { }
        }

        columns[i].FilterElements = filterElements;

        columns[i].onclick = ColumnFilter.Click;
    }
});

var ColumnFilter = {
    Click: function () {
        if (event.IS_TOGGLE)
            return;

        if (event.target.className.trim() != "ColumnFilterHeader")
            return;

        var sender = this;

        var container = undefined;

        for (var i = 0; i < sender.childNodes.length; i++) {
            if (sender.childNodes.item(i).className == undefined)
                continue;

            if (sender.childNodes.item(i).className.indexOf("ColumnFilterContainer") != -1) {
                container = sender.childNodes.item(i);
                break;
            }
        }

        if (container != undefined) {
            container.parentNode.removeChild(container)
            return;
        }

        container = document.createElement("div")
        container.className = "ColumnFilterContainer";

        container.style.width = (sender.offsetWidth - 21) + "px";
        container.style.marginTop = (sender.offsetHeight - 19) + "px";

        var name = "chkColumnFilterSelectAll" + parseInt(Math.random() * 100);

        var pnlFilterElement;
        var input;
        var label;
        var table = document.createElement("table");
        var tableRow;
        var tableCell;


        tableRow = document.createElement("tr");

        table.appendChild(tableRow);

        pnlFilterElement = document.createElement("div");

        tableCell = document.createElement("td");

        input = document.createElement("input");
        input["Column"] = sender;
        input.type = "checkbox";

        if (sender.SelectedFilterElements.indexOf(sender.FilterElements._Keys[i]) != -1)
            input.checked = true;

        input.setAttribute("onclick", "ColumnFilter.SelectAll('" + name + "'); ");

        tableCell.appendChild(input);
        tableRow.appendChild(tableCell);

        tableCell = document.createElement("td");

        label = document.createElement("label");
        label.innerHTML = "select all";

        tableCell.appendChild(label);
        tableRow.appendChild(tableCell);

        for (var i = 0; i < sender.FilterElements._Keys.length; i++) {
            tableRow = document.createElement("tr");

            table.appendChild(tableRow);

            pnlFilterElement = document.createElement("div");

            tableCell = document.createElement("td");

            input = document.createElement("input");
            input.type = "checkbox";
            input.name = name;

            if (sender.SelectedFilterElements.indexOf(sender.FilterElements._Keys[i]) != -1)
                input.checked = true;

            input["Column"] = sender;
            input["Text"] = sender.FilterElements._Keys[i];
            input.onclick = ColumnFilter.ToggleItem;

            tableCell.appendChild(input);
            tableRow.appendChild(tableCell);

            tableCell = document.createElement("td");

            label = document.createElement("label");
            label.innerHTML = sender.FilterElements._Keys[i];

            tableCell.appendChild(label);
            tableRow.appendChild(tableCell);
        }
        container.appendChild(table);

        if (sender.childNodes.length != 0)
            sender.insertBefore(container, sender.childNodes.item(0));
        else
            sender.appendChild(container);

        if (InitBoxes)
            InitBoxes();
    },
    SelectAll: function (name) {
        var elements = document.getElementsByName(name);

        var index;
        for (var i = 0; i < elements.length; i++) {
            elements.item(i).checked = this.checked;

            index = elements.item(i).Column.SelectedFilterElements.indexOf(elements.item(i).Text);

            if (index != -1)
                elements.item(i).Column.SelectedFilterElements.splice(elements.item(i).Column.SelectedFilterElements.indexOf(elements.item(i).Text), 1);
            else
                elements.item(i).Column.SelectedFilterElements.push(elements.item(i).Text);
        }

        if (elements.length != 0)
            elements.item(0).Column.Update();
    },
    ToggleItem: function () {
        event.IS_TOGGLE = true;
        var index = this.Column.SelectedFilterElements.indexOf(this.Text);

        if (index != -1)
            this.Column.SelectedFilterElements.splice(this.Column.SelectedFilterElements.indexOf(this.Text), 1);
        else
            this.Column.SelectedFilterElements.push(this.Text);

        this.Column.Update();
    },
    Update: function () {
        var rows = this.parentNode.parentNode.getElementsByTagName("tr");

        var indexes;
        for (var i = 0; i < this.FilterElements._Keys.length; i++) {
            indexes = this.FilterElements[this.FilterElements._Keys[i]];


            for (var index = 0; index < indexes.length; index++) {
                indexes[index].style.display = "none";
            }

            if (this.SelectedFilterElements.indexOf(this.FilterElements._Keys[i]) != -1) {
                for (var index = 0; index < indexes.length; index++) {
                    indexes[index].style.display = "";
                }
            }
        }
    }
};