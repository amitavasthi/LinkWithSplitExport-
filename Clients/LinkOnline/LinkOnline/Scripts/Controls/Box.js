var dragBox = undefined;
var dragOffsetLeft = 0;
var dragOffsetTop = 0;

function InitDragBox(id, position) {
    var box = document.getElementById(id);

    if (box == undefined)
        return;

    if (position == undefined)
        position = "Bottom"

    box.style.position = "absolute";
    box.parentNode.style.display = "";

    var left = (ContentWidth / 2) - (box.offsetWidth / 2);
    var top = (ContentHeight / 2) - (box.offsetHeight / 2);

    if (left < 0) left = 0;
    if (top < 0) top = 0;

    box.style.left = left + "px";
    
    switch (position) {
        case "Bottom":
            box.style.bottom = "-" + (box.offsetHeight + 15) + "px";
            break;
        case "Center":
            box.style.top = ((window.innerHeight / 2) - (box.offsetHeight / 2)) + "px";
            break;
        case "Top":
            box.style.top = "-" + (box.offsetHeight + 15) + "px";
            break;
    }

    var boxContent = GetChildByAttribute(box, "class", "BoxContent");

    boxContent.style.maxHeight = (window.innerHeight - 100) + "px";
    
  
    var id = $('.BoxContent').last().parent().prop('id');
   

    if (id == 'boxCombineScores') {
        boxContent.style.overflow = "hidden";
    } else {
        boxContent.style.overflowY = "auto";
    }

   
    //boxContent.style.overflow = "hidden";

    var background = GetChildByAttribute(box.parentNode, "id", "BoxBackground");

    if (background == undefined) {
        background = document.createElement("div");

        box.parentNode.appendChild(background);
    }

    background.id = "BoxBackground";
    background.className = "BoxBackground";

    background.style.opacity = "0.0";

    IncreaseOpacity(background, 20, 0.5);

    var headline = box.getElementsByTagName("div").item(0);

    headline.onmousedown = function (e) {
        dragBox = this.parentNode;

        getMouseXY(e);

        dragOffsetLeft = tempX - dragBox.offsetLeft;
        dragOffsetTop = tempY - dragBox.offsetTop;

        dragOffsetTop += 15;
        dragOffsetLeft += 15;
    };

    bodyMouseUp.push(function () {
        dragBox = undefined;

        dragOffsetLeft = 0;
        dragOffsetTop = 0;
    });

    switch (position) {
        case "Bottom":
            IncreaseBottom(box, 0);
            break;
        case "Center":
            box.style.top = ((window.innerHeight / 2) - (box.offsetHeight / 2)) + "px";
            box.style.opacity = "";
            break;
        case "Top":
            IncreaseTop(box, 15);
            break;
    }
}

function CloseBox(id, position) {
    var box = document.getElementById(id);

    if (box == undefined)
        return;

    if (position == undefined)
        position = "Bottom"

    var background = GetChildByAttribute(box.parentNode, "id", "BoxBackground");

    DecreaseOpacity(background, undefined, 100);

    switch (position) {
        case "Bottom":
            DecreaseBottom(box, -1 * (box.offsetHeight + 15), function () {
                box.parentNode.style.display = "none";
            });
            break;
        case "Center":
            DecreaseOpacity(box, function () {
                box.parentNode.style.display = "none";
            }, 50);
            break;
        case "Top":
            DecreaseTop(box, -1 * (box.offsetHeight + 15), function () {
                box.parentNode.style.display = "none";
            });
            break;
    }
}