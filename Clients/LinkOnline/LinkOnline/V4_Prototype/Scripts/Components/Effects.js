var Effects = {
    Rotate: function (element, deg) {
        element.style.transform = "rotate(" + deg + "deg)";
        element.style.transition = "transform 2s";
    },
    BlendIn: function (element) {
        element.style.opacity = "0";
        element.style.transition = "opacity .5s";

        window.setTimeout(function () {
            element.style.opacity = "1";
        }, 100);
    }
};