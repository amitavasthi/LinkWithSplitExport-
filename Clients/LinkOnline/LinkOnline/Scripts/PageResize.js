var lastWidth = window.innerWidth;
var lastHeight = window.innerHeight;

window.setTimeout(CheckResize, 1000);

function CheckResize() {
    if (lastWidth != window.innerWidth ||
        lastHeight != window.innerHeight) {

        lastWidth = window.innerWidth;
        lastHeight = window.innerHeight;

        PageResize();
    }

    window.setTimeout(CheckResize, 1000);
}