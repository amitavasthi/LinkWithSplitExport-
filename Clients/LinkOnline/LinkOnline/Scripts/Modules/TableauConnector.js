
var tableauConnector = {
    "Username": undefined,
    "Password": undefined,
    "SelectedVariables": []
};

tableauConnector.CheckAuthentication = function (onFinish) {
    _AjaxRequest("/Handlers/LinkBiExternal.ashx", "GetVariables", "", function () {
        if (onFinish != undefined)
            onFinish();
    }, function () {
        document.getElementById("LoginForm").style.display = "";
        document.getElementById("content").style.display = "none";
    });
};

tableauConnector.Authenticate = function (username, password) {

    _AjaxRequest("/Handlers/LinkBiExternal.ashx", "Authenticate", "Username=" + username + "&Password=" + password, function () {

        document.getElementById("LoginForm").style.display = "none";
        document.getElementById("content").style.display = "";

        tableauConnector.LoadVariables();

    }, function () {
        document.getElementById("lblLoginError").style.display = "";
    });
};

tableauConnector.LoadVariables = function () {
    _AjaxRequest("/Handlers/LinkBiExternal.ashx", "GetVariables", "", function (response) {
        tableauConnector.AvailableVariables = JSON.parse(response);

        var container = document.getElementById("availableVariables");

        for (var i = 0; i < tableauConnector.AvailableVariables.length; i++) {
            var pnlVariable = document.createElement("div");
            pnlVariable.id = "pnlAvailableVariable" + tableauConnector.AvailableVariables[i].Id;
            pnlVariable.className = "TableauConnectorVariable";

            if (i % 2 == 0) {
                //pnlVariable.style.background = "#F6F6F6";
            }

            pnlVariable.innerHTML = tableauConnector.AvailableVariables[i].Label;
            pnlVariable.setAttribute("onclick", "tableauConnector.SelectVariable('" + tableauConnector.AvailableVariables[i].Id + "','" + tableauConnector.AvailableVariables[i].Name + "')");

            container.appendChild(pnlVariable);
        }
    });
};

tableauConnector.Search = function (value) {
    var container = document.getElementById("availableVariables");

    for (var i = 0; i < container.childNodes.length; i++) {
        if (container.childNodes.item(i).textContent.toLowerCase().search(value.toLowerCase()) == -1) {
            container.childNodes.item(i).style.display = "none";
        } else {
            container.childNodes.item(i).style.display = "";
        }
    }
};

tableauConnector.SelectVariable = function (id, name) {
    var pnlVariable = document.getElementById("pnlAvailableVariable" + id);

    if (pnlVariable == undefined)
        return;

    pnlVariable.style.background = "";

    if (document.getElementById("selectedVariables").childNodes.length % 2 == 1) {
        //pnlVariable.style.background = "#F6F6F6";
    }

    document.getElementById("selectedVariables").appendChild(pnlVariable);

    tableauConnector["SelectedVariables"].push(name);
};


var data;
var FieldNames = [];
var FieldTypes = [];
var ReturnData = [];

tableauConnector.Load = function() {
    var url = "http://youtube.tokyo.local/Handlers/LinkBiExternal.ashx?Method=ProcessReport&";

    for (var i = 0; i < tableauConnector.SelectedVariables.length; i++) {
        url += "Dimension" + (i + 1) + "=" + tableauConnector.SelectedVariables[i] + "&";
    }

    url += "ResponseType=JSON";

    document.body.style.background = "#355C80";
    document.forms[0].style.display = "none";
    document.body.innerHTML += "<div class=\"spinner\"><div class=\"double-bounce1\"></div><div class=\"double-bounce2\"></div></div>";

    _AjaxRequest(url, "", "", function (response) {
        FieldNames = [];
        FieldTypes = [];
        ReturnData = [];

        data = JSON.parse(response);

        for (var i = 0; i < tableauConnector.SelectedVariables.length; i++) {
            FieldNames.push(tableauConnector.SelectedVariables[i]);
            FieldTypes.push("string");
        }

        FieldNames.push("Value");
        FieldTypes.push("float");

        var jsonScript = "";
        for (var i = 0; i < data.length; i++) {
            jsonScript = "[";
            for (var v = 0; v < tableauConnector.SelectedVariables.length; v++) {
                jsonScript += "\"" + data[i]["Filter" + (v + 1)] + "\", ";
            }
            jsonScript += data[i].Value + "]";

            ReturnData.push(JSON.parse(jsonScript));
        }
        
        document.body.innerHTML = "<form method=\"POST\" style=\"display:none;\" action=\"" + window.location + "\">" +
            "<input type=\"hidden\" name=\"Method\" value=\"Load\" />" +
            "<textarea name=\"FieldNames\">" + (JSON.stringify(FieldNames)) + "</textarea>" +
            "<textarea name=\"FieldTypes\">" + (JSON.stringify(FieldTypes)) + "</textarea>" +
            "<textarea name=\"ReturnData\">" + (JSON.stringify(ReturnData)) + "</textarea>" +
            "<div class=\"spinner\"><div class=\"double-bounce1\"></div><div class=\"double-bounce2\"></div></div>"
            "</form>";

        document.forms[0].submit();
        //tableau.submit();
        //window.location = "TableauConnectionEstablisher.aspx?FieldNames=" + encodeURIComponent(JSON.stringify(FieldNames)) + "&FieldTypes=" + encodeURIComponent(JSON.stringify(FieldTypes)) + "&ReturnData=" + encodeURIComponent(JSON.stringify(ReturnData));
    });
}