function ShowOptions(sender, direction) {
    var options = GetChildByAttribute(sender, "class", "Options");

    if (options == undefined)
        return;

    if (options.style.display == "block")
        return;

    options.style.visibility = "hidden";
    options.style.display = "block";

    var top = GetParentScrollTop(options);

    window.setTimeout(function () {
        if (direction == "Left") {
            options.style.marginTop = "-" + top + "px";
            var width = options.offsetWidth;
            //var width = 250;

            options.style.width = "1px";
            options.style.visibility = "";

            IncreaseWidth(options, width, true);
        }
        else if (direction == "Right") {
            options.style.marginTop = "-" + top + "px";
            var width = options.offsetWidth;
            //var width = 250;

            options.style.marginLeft = sender.offsetWidth + "px";
            options.style.width = "1px";
            options.style.visibility = "";

            IncreaseWidth(options, width, false);
        }
        else if (direction == "Bottom") {
            options.style.marginTop = ((top * -1) + sender.offsetHeight) + "px";
            var height = options.offsetHeight;
            //var width = 250;

            options.style.height = "1px";
            options.style.visibility = "";

            IncreaseHeight(options, height, true);
        }
    }, 1);
}

var overOptions = false;

function HideOptions(sender, direction) {
    window.setTimeout(function () {
        if (overOptions)
            return;

        var options = GetChildByAttribute(sender, "class", "Options");

        if (options == undefined)
            return;

        if (direction == "Left") {
            DecreaseWidth(options, 0, true, function () {
                options.style.display = "none";
                options.style.width = "";
            });
        }
        else if (direction == "Right") {
            DecreaseWidth(options, 0, false, function () {
                options.style.display = "none";
                options.style.width = "";
            });
        }
        else if (direction == "Bottom") {
            DecreaseWidth(options, 0, true, function () {
                options.style.display = "none";
                options.style.width = "";
            });
        }
    }, 100);
}