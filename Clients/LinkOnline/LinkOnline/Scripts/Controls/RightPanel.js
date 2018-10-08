

function ShowRightPanel(sender) {
    var pnlRightPanel = document.getElementById("pnlRightPanel");

    if (pnlRightPanel.style.display == "none") {
        pnlRightPanel.style.visibility = "hidden";
        pnlRightPanel.style.display = "";

        pnlRightPanel.style.width = "";

        var width = pnlRightPanel.offsetWidth;

        pnlRightPanel.style.width = "0px";
        pnlRightPanel.style.visibility = "";

        IncreaseWidth(pnlRightPanel, width, false, function () {
            sender.src = "/Images/Icons/LinkReporterSettings/Collapser.png";
        });
    }
    else {
        DecreaseWidth(pnlRightPanel, 0, false, function () {
            pnlRightPanel.style.display = "none";
            sender.src = "/Images/Icons/LinkReporterSettings/Expander.png";
        });

    }
}
