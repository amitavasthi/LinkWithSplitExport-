// ##### EVAL #####
$(document).ready(function () {
    var elements;
    try {
        elements = $("Eval");

        for (var i = 0; i < elements.length; i++) {
            try{
                elements[i].innerHTML = eval(elements[i].innerText);
            } catch (e) {
                console.log(e);
            }
            elements[i].style.display = "initial";
        }
    }
    catch (e) { }
});

var StringBuilder = function () {
    var result = {
        Sequences:[]
    };

    result.Append = function (value) {
        this.Sequences.push(value);
    }

    result.ToString = function () {
        return this.Sequences.join("");
    };

    return result;
};