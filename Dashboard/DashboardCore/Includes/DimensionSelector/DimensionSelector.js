
$(document).ready(function () {
    var elements = $(".DimensionSelector");

    for (var i = 0; i < elements.length; i++) {
        var selected = elements[i].getAttribute("selected").split(',');
        //var selected = [elements[i].getAttribute("selected")];

        for (var j = 0; j < selected.length; j++) {
            for (var o = 0; o < elements[i].options.length; o++) {
                if (elements[i].options[o].value.trim() == selected[j].trim()) {
                    elements[i].options[o].selected = true;
                    break;
                }
            }
        }

        elements[i].setAttribute("onchange", "DimensionSelector.UpdateValues();");
    }

    try{
        $(".DimensionSelector").select2({
            placeholder: "select...", allowClear: true
        });
    }
    catch(e){}
});

var DimensionSelector = {
    UpdateValues: function () {
        var parameters = "";

        var elements = $(".DimensionSelector");

        for (var i = 0; i < elements.length; i++) {
            var options = "";

            for (var j = 0; j < elements[i].selectedOptions.length; j++) {
                options += elements[i].selectedOptions[j].value + ",";
            }

            if (options.length != 0)
                options = options.slice(0, options.length - 1);

            parameters += "&" + elements[i].id + "=" + options;
        }

        parameters += "&BodyOnly=false";

        window.location = window.location.toString().split('&')[0] + parameters;
        /*RequestAsynch(window.location.toString().split('&')[0], "", parameters, function (response) {
            document.body.innerHTML = response;
        });*/
        return;

        var parameters = "";

        var elements = $(".DimensionSelector");

        for (var i = 0; i < elements.length; i++) {
            var options = "";

            for (var j = 0; j < elements[i].selectedOptions.length; j++) {
                options += elements[i].selectedOptions[j].value + ",";
            }

            if (options.length != 0)
                options = options.slice(0, options.length - 1);

            parameters += elements[i].id + "=" + options + "&";
        }

        parameters += "RenderMode=DataUpdate";

        RequestAsynch(window.location.toString(), "", parameters, function (response) {
            response = JSON.parse(response);

            var element;
            for (var i = 0; i < response.length; i++) {
                element = document.getElementById("r_" + response[i].Path);

                if (element == undefined)
                    continue;

                element.innerHTML = response[i].Value;
            }
        });
    }
};