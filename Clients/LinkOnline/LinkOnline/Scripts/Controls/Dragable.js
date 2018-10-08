function MakeDragable(control, trigger, deleteControlAfter, onFinish, onDragStart, clone, onDrag) {
    if (deleteControlAfter == undefined)
        deleteControlAfter = false;

    var c;


    if (clone) {
        c = control.cloneNode(true);
    }
    else {
        c = control;
    }

    trigger.onmousedown = function () {

        if (clone)
            document.body.appendChild(c);

        if (onDragStart != undefined)
            onDragStart(c);

        mouseMoveFunctions.Add("Drag" + c.id, function () {
            if (onDrag != undefined)
                onDrag(c);

            c.style.position = "absolute";
            c.style.left = (tempX - (c.offsetWidth / 2)) + "px";
            c.style.top = (tempY - (c.offsetHeight / 2)) + "px";
        });
    }

    c.onmouseup = function () {
        this.style.position = "";
        mouseMoveFunctions.Delete("Drag" + this.id);

        if (deleteControlAfter) {
            this.parentNode.removeChild(this);
        }

        if (onFinish != undefined)
            onFinish(c);
    };
}