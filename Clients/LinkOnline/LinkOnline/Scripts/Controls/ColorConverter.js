function ConvertColor(to, element, mode, onFinish) {
    element["RedDone"] = false;
    element["GreenDone"] = false;
    element["BlueDone"] = false;

    var str = element.style.backgroundColor;

    if (str == "" && element.style.backgroundColor != "")
        str = element.style.backgroundColor;

    str = str.replace("rgb(", "").replace(")", "");

    element["ColorMode"] = mode;

    var splits = str.split(',');

    var from = [Red = splits[0], Green = splits[1], Blue = splits[2]];

    window.setTimeout(function () {
        IncreaseRed(from[0], to[0], from[0], element, mode, onFinish);
    }, 100);
    window.setTimeout(function () {
        IncreaseGreen(from[1], to[1], from[1], element, mode, onFinish);
    }, 100);
    window.setTimeout(function () {
        IncreaseBlue(from[2], to[2], from[2], element, mode, onFinish);
    }, 100);
}

function CheckDone(element, onFinish) {
    if(element["RedDone"] == true &&
        element["GreenDone"] == true &&
        element["BlueDone"] == true) {
        if (onFinish != undefined) {
            onFinish();
        }
    }
}


function IncreaseRed(from, to, current, element, mode, onFinish) {
    if (element["ColorMode"] != mode)
        return;

    element["Red"] = current;

    var factor = Math.abs(to - current) * 10 / 100;

    if (factor < 1)
        factor = 1;

    factor = parseInt(factor);

    current = parseInt(current);
    to = parseInt(to);

    if (current < to) {
        current += factor;
    }
    else if (current > to) {
        current -= factor;
    }
    else {
        element["RedDone"] = true;
        CheckDone(element, onFinish);
        return;
    }


    element["Red"] = current;

    element.style.backgroundColor = "rgb(" + element["Red"] + ", " + element["Green"] + ", " + element["Blue"] + ")";

    window.setTimeout(function () {
        IncreaseRed(parseInt(from), parseInt(to), parseInt(current), element, mode, onFinish);
    }, 10);

}

function IncreaseBlue(from, to, current, element, mode, onFinish) {
    if (element["ColorMode"] != mode)
        return;

    element["Blue"] = current;

    var factor = Math.abs(to - current) * 10 / 100;

    if (factor < 1)
        factor = 1;

    factor = parseInt(factor);

    current = parseInt(current);
    to = parseInt(to);

    if (current < to) {
        current += factor;
    }
    else if (current > to) {
        current -= factor;
    }
    else {
        element["BlueDone"] = true;
        CheckDone(element, onFinish);
        return;
    }

    element["Blue"] = current;

    element.style.backgroundColor = "rgb(" + element["Red"] + ", " + element["Green"] + ", " + element["Blue"] + ")";

    window.setTimeout(function () {
        IncreaseBlue(parseInt(from), parseInt(to), parseInt(current), element, mode, onFinish);
    }, 10);
}

function IncreaseGreen(from, to, current, element, mode, onFinish) {
    if (element["ColorMode"] != mode)
        return;

    element["Green"] = current;

    var factor = Math.abs(to - current) * 10 / 100;

    if (factor < 1)
        factor = 1;

    factor = parseInt(factor);

    current = parseInt(current);
    to = parseInt(to);

    if (current < to) {
        current += factor;
    }
    else if (current > to) {
        current -= factor;
    }
    else {
        element["GreenDone"] = true;
        CheckDone(element, onFinish);
        return;
    }

    element["Green"] = current;

    element.style.backgroundColor = "rgb(" + element["Red"] + ", " + element["Green"] + ", " + element["Blue"] + ")";

    window.setTimeout(function () {
        IncreaseGreen(parseInt(from), parseInt(to), parseInt(current), element, mode, onFinish);
    }, 10);
}


function ConvertFontColor(to, element, mode) {
    var str = element.style.color;

    if (str == "" && element.style.color != "")
        str = element.style.color;

    str = str.replace("rgb(", "").replace(")", "");

    element["FontColorMode"] = mode;

    var splits = str.split(',');

    var from = [Red = splits[0], Green = splits[1], Blue = splits[2]];

    window.setTimeout(function () {
        IncreaseFontRed(from[0], to[0], from[0], element, mode);
    }, 100);
    window.setTimeout(function () {
        IncreaseFontGreen(from[1], to[1], from[1], element, mode);
    }, 100);
    window.setTimeout(function () {
        IncreaseFontBlue(from[2], to[2], from[2], element, mode);
    }, 100);
}

function IncreaseFontRed(from, to, current, element, mode) {
    if (element["FontColorMode"] != mode)
        return;

    var factor = Math.abs(to - current) * 10 / 100;

    if (factor < 1)
        factor = 1;

    factor = parseInt(factor);

    current = parseInt(current);
    to = parseInt(to);

    if (current < to) {
        current += factor;
    }
    else if (current > to) {
        current -= factor;
    }
    else {
        return;
    }


    element["FontRed"] = current;

    element.style.color = "rgb(" + element["FontRed"] + ", " + element["FontGreen"] + ", " + element["FontBlue"] + ")";

    window.setTimeout(function () {
        IncreaseFontRed(parseInt(from), parseInt(to), parseInt(current), element, mode);
    }, 10);

}

function IncreaseFontBlue(from, to, current, element, mode) {
    if (element["FontColorMode"] != mode)
        return;

    var factor = Math.abs(to - current) * 10 / 100;

    if (factor < 1)
        factor = 1;

    factor = parseInt(factor);

    current = parseInt(current);
    to = parseInt(to);

    if (current < to) {
        current += factor;
    }
    else if (current > to) {
        current -= factor;
    }
    else {
        return;
    }

    element["FontBlue"] = current;

    element.style.color = "rgb(" + element["FontRed"] + ", " + element["FontGreen"] + ", " + element["FontBlue"] + ")";

    window.setTimeout(function () {
        IncreaseFontBlue(parseInt(from), parseInt(to), parseInt(current), element, mode);
    }, 10);
}

function IncreaseFontGreen(from, to, current, element, mode) {
    if (element["FontColorMode"] != mode)
        return;

    var factor = Math.abs(to - current) * 10 / 100;

    if (factor < 1)
        factor = 1;

    factor = parseInt(factor);

    current = parseInt(current);
    to = parseInt(to);

    if (current < to) {
        current += factor;
    }
    else if (current > to) {
        current -= factor;
    }
    else {
        return;
    }

    element["FontGreen"] = current;

    element.style.color = "rgb(" + element["FontRed"] + ", " + element["FontGreen"] + ", " + element["FontBlue"] + ")";

    window.setTimeout(function () {
        IncreaseFontGreen(parseInt(from), parseInt(to), parseInt(current), element, mode);
    }, 10);
}