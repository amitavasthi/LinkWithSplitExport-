function ShowJavascriptBox(id, content, onConfirm, renderButtons, className) {
    if (renderButtons == undefined)
        renderButtons = true;

    if (className == undefined)
        className = "";

    var container = document.createElement("div");
    container.id = "_" + id;

    var box = document.createElement("div");
    box.id = id;
    box.className = "Box BackgroundColor5 " + className;

    var title = document.createElement("div");
    var contentContainer = document.createElement("div");
    var buttons = document.createElement("div");

    title.className = "BoxTitle";
    contentContainer.className = "BoxContent";
    buttons.className = "BoxButtons";

    var closeButton = document.createElement("img");
    closeButton.src = "/Images/Icons/BoxClose.png";
    closeButton.style.cssFloat = "right";
    closeButton.onmouseover = "this.src='/Images/Icons/BoxClose_Hover.png';";
    closeButton.onmouseout = "this.src = '/Images/Icons/BoxClose.png'";
    closeButton.onclick = function () {
        HideJavascriptBox(id);
    };

    title.appendChild(closeButton);
    contentContainer.innerHTML = content;

    box.appendChild(title);
    box.appendChild(contentContainer);

    if (renderButtons) {
        var submitButton = document.createElement("input");
        submitButton.type = "button";
        submitButton.value = LoadLanguageText("Confirm");
        submitButton.onclick = function ()
        {
            var txtCombineScoresName = document.getElementById( "txtCombineScoresName" );
            var textVal = txtCombineScoresName.value
            var trimVal = textVal.trim();
            if ( trimVal != "" )
            {
                document.getElementById( "txtCombineScoresName" ).style.borderColor = "#6CAEE0";
                if ( onConfirm != undefined )
                    onConfirm();
                HideJavascriptBox( id );
            } else
            {
                document.getElementById( "txtCombineScoresName" ).style.borderColor = "red";
            }
        };
        //submitButton.onclick = function () {
        //    if (onConfirm != undefined)
        //        onConfirm();

        //    HideJavascriptBox(id);
        //};

        var cancelButton = document.createElement("input");
        cancelButton.type = "button";
        cancelButton.value = LoadLanguageText("Cancel");

        cancelButton.onclick = function () {
            HideJavascriptBox(id);
        }

        buttons.appendChild(submitButton);
        buttons.appendChild(cancelButton);

        box.appendChild(buttons);

        box["SubmitButton"] = submitButton;
    }

    container.appendChild(box);

    document.body.appendChild(container);

    InitDragBox(id);
}

function HideJavascriptBox(id) {
    var container = document.getElementById(id);

    if (container != undefined)
        container.parentNode.parentNode.removeChild(container.parentNode);
}