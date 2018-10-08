function CreateButton2(id, name, onclick, onError) {
    var button = document.createElement("div");
    button.id = id;

    var click = "Button2_Click(this, undefined, undefined, " +
        (onclick != undefined ? "'" + onclick + "'" : "undefined") + ");";

    button.innerHTML = "<div id=\"_"+ id +"\" class=\"Button\" onclick=\"" + click + "\">" +
        LanguageManager.GetLabel(name)+ "</div>";

    return button;
}

function Button2_Click(sender, method, postFields, onClientClick) {
    sender.style.width = (sender.offsetWidth - 20) + "px";
    sender.style.height = (sender.offsetHeight - 10) + "px";

    var exText = sender.innerHTML;

    sender.innerHTML = "<div style=\"display:inline-block;\"><div class=\"spinner\" style=\"width:20px;height:20px;\">" +
        "<div class=\"double-bounce1\"></div>"+
        "<div class=\"double-bounce2\"></div>"+
      "</div></div>";

    if (onClientClick != undefined)
        eval(onClientClick);

    if (method == undefined)
        return;

    window.setTimeout(function () {
        var parameters = "";

        for (var i = 0; i < postFields.length; i++) {
            var postField = document.getElementById(postFields[i]);

            if (postField == undefined)
                continue;

            parameters += postFields[i] + "=" + postField.value + "&";
        }

        var result = Request(window.location.toString().split('?')[0], method, parameters);


        if (result != undefined && result != "") {
            if (result == "__ERROR__") {

                if (sender.getAttribute("onError") != undefined)
                    eval(sender.getAttribute("onError"));

                sender.innerHTML = exText;
                sender.style.width = "";
                sender.style.height = "";
                return;
            }

            window.location = result;
        }
        else {

            sender.innerHTML = exText;
            sender.style.width = "";
            sender.style.height = "";
        }
    }, 1);
}