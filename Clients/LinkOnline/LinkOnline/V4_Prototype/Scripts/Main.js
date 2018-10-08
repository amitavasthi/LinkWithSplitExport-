

var Pages = {

};

$(document).ready(function () {
    document.content = document.getElementById("pnlMain");

    window.onresize();

    Menu.Init();

    Pages.Home.Load();
});

window.onresize = function () {
    document.content.style.overflow = "auto";
    document.content.style.height = window.innerHeight + "px";
};

function ShowError() {

}

function Logout() {
    var pnlMainContainer = document.getElementById("pnlMainContainer");

    pnlMainContainer.style.transition = "opacity .5s";
    pnlMainContainer.style.opacity = "1";

    var message = document.createElement("div");
    message.innerHTML = "goodbye";
    message.className = "LogoutMessage";
    message.style.opacity = "0";

    document.body.appendChild(message);
    
    RequestAsynch("Service.ashx", "Logout", "");
    window.setTimeout(function () {
        pnlMainContainer.style.opacity = "0";
        window.setTimeout(function () {
            message.style.opacity = "1";

            window.setTimeout(function () {
                message.style.opacity = "0";

                window.setTimeout(function () {
                    window.location = "Default.html";
                }, 500);
            }, 1000);
        }, 250);
    }, 10);
}

function htmlEncode(value) {
    //create a in-memory div, set it's inner text(which jQuery automatically encodes)
    //then grab the encoded contents back out.  The div never exists on the page.
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}