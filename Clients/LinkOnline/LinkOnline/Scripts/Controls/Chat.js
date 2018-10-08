var chatMessageLimit = 10;

function InitChat(idPnlChat) {

    var pnlChat = document.getElementById(idPnlChat);

    // FOR TEST ONLY:
    var height = pnlChat.parentNode.parentNode.parentNode.parentNode.offsetHeight;

    pnlChat.style.height = (height - 100) + "px";

    GetLatestChatMessages(pnlChat, true);
}

function SendChatMessage(sender, pnlChat) {
    _AjaxRequest("/Handlers/WebUtilitiesHandler.ashx", "SendChatMessage", "Limit=" + chatMessageLimit + "&IdChat=Global&Message=" + sender.value, function (response) {
        sender.value = "";
    });
}

var latestUpdate = new Date(2015, 05, 19, 12, 0, 0, 0);
var chatMessageCounter = 0;
function GetLatestChatMessages(pnlChat, firstRun) {
    _AjaxRequest("/Handlers/WebUtilitiesHandler.ashx", "GetLatestChatMessages", "Limit=" + chatMessageLimit + "&LatestUpdate=" + latestUpdate.format("yyyy/MM/dd HH:mm:ss") + "&IdChat=Global", function (response) {
        if (response == "") {
            window.setTimeout(function () {
                GetLatestChatMessages(pnlChat, false)
            }, 1000);

            return;
        }

        var messages = JSON.parse(response);

        for (var i = 0; i < messages.length; i++) {
            var sent = new Date(messages[i].Sent);

            if (sent > latestUpdate)
                latestUpdate = sent;

            RenderChatMessage(messages[i], pnlChat, firstRun);
        }

        if (messages.length > 0)
            pnlChat.scrollTop = pnlChat.scrollHeight;

        window.setTimeout(function () {
            GetLatestChatMessages(pnlChat, false)
        }, 500);
    });
}

function GetChatMessages(pnlChat) {
    _AjaxRequest("/Handlers/WebUtilitiesHandler.ashx", "GetChatMessages", "Limit=" + chatMessageLimit + "&IdChat=Global", function (response) {
        pnlChat.innerHTML += response;
    });
}

function RenderChatMessage(message, pnlChat, firstRun) {
    var messageControl = document.createElement("div");
    messageControl.className = "ChatMessage ChatMessage" + (chatMessageCounter++ % 2);

    var html = "<table cellspacing=\"0\" cellpadding=\"0\" class=\"ChatMessageHeadline\"><tr>";
    html += "<td><img src=\"/Handlers/GlobalHandler.ashx?Method=GetUserImage&IdUser=" + message.IdUser + "\" /></td>";
    html += "<td><b>" + message.UserName + "</b><br />" + message.Sent + "</td>";
    html += "</tr></table>";

    html += "<div class=\"ChatMessageContent\">" + message.Message + "</div>";

    messageControl.innerHTML = html;

    pnlChat.appendChild(messageControl);

    /*if (firstRun != true) {
        var height = messageControl.offsetHeight;
        messageControl.style.height = "0px";

        IncreaseHeight(messageControl, height, function () {
            messageControl.style.height = "";
        });
    }*/
}