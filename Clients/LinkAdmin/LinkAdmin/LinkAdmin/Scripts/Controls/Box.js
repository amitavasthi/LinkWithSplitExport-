function CreateBox(id, content) {
    var container = document.createElement("div");
    container.id = id;

    var background = document.createElement("div");
    var box = document.createElement("div");

    background.className = "BoxBackground";
    box.className = "Box";

    box.innerHTML = content;

    container.appendChild(background);
    container.appendChild(box);

    document.body.appendChild(container);

    //window.setTimeout(function () {
        box.style.maxHeight = (window.innerHeight - 100) + "px";
        box.style.maxWidth = (window.innerWidth - 100) + "px";

        box.style.marginLeft = (box.offsetWidth / -2) + "px";
        box.style.marginTop = (box.offsetHeight / -2) + "px";
    //}, 1);

    background.setAttribute("onclick", "HideBox('"+ id +"');")

    box.Close = function () {
        HideBox(id);
    };

    return box;
}

function HideBox(id) {
    var container = document.getElementById(id);

    if (container == undefined)
        return;

    //DecreaseZoom(container, 0.1, function () {
        container.parentNode.removeChild(container);
    //});
}