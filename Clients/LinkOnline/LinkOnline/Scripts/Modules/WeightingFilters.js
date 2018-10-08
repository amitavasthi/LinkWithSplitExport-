function ShowWeightingFilters(idBox) {
    var control = document.getElementById(idBox);

    if (control == undefined)
        return;

    control.style.display = "block";
}

function RemoveWeightingVariable(idCategory, sender) {
    AjaxRequest("RemoveWeightingVariable", "IdCategory=" + idCategory, function (response) {
        sender.parentNode.parentNode.parentNode.removeChild(
            sender.parentNode.parentNode
        );
    });
}

function CloseWeightingFilters() {
    ReloadCrosstable();
}

function SetOverallWeightingVariable(weightingVariable) {
    AjaxRequest("SetOverallWeightingVariable", "WeightingVariable=" + weightingVariable, function (response) {
        //ReloadCrosstable();
        defaultWeightingHasChanged = true;
    });
}

var defaultWeightingHasChanged = false;
function ShowDefaultWeightingSelector(sender) {
    var container = document.getElementById("pnlDefaultWeighting");

    if (container.style.display != "none") {
        HideDefaultWeightingSelector();
        return;
    }

    defaultWeightingHasChanged = false;

    var container = document.getElementById("pnlDefaultWeighting");

    container.style.visibility = "hidden";
    container.style.display = "";
    container.style.width = "";

    var width = container.offsetWidth;

    container.style.width = "0px";
    container.style.visibility = "";

    var left = GetOffsetLeft(sender);
    var top = GetOffsetTop(sender);

    left += sender.offsetWidth;

    container.style.left = left + "px";
    container.style.top = top + "px";

    IncreaseWidth(container, width);
}
//start of Bug no 253 
function HideDefaultWeightingSelector() {
    var container = document.getElementById("pnlDefaultWeighting");

    DecreaseWidth(container, 0, undefined, function () {
        container.style.display = "none";
        container.style.width = "0px";

        if (defaultWeightingHasChanged) {
            if (window.location.toString().search("/LinkReporter/Crosstabs.aspx") != -1)
                ReloadCrosstable();
        }
        else {
            if ($('#cphContent_ddlDefaultWeighting :selected').text() != "None") {

                if ($("#cphContent_pnlWeighting").attr("class") === "BackgroundColor7H1") {
                    $("#cphContent_pnlWeighting").removeClass("BackgroundColor7H1");
                    $("#cphContent_pnlWeighting").addClass("GreenBackground3");
                    $("#cphContent_pnlWeighting").css("background-image", "url('/Images/Icons/Weighting_Active.png')");
                }

            }
            else {
                $("#cphContent_pnlWeighting").removeClass("GreenBackground3");
                $("#cphContent_pnlWeighting").addClass("BackgroundColor7H1");
                $("#cphContent_pnlWeighting").css("background-image", "url('/Images/Icons/Weighting.png')");
            }

        }
    });
}

//end of Bug no 253

//function HideDefaultWeightingSelector() {
//    var container = document.getElementById("pnlDefaultWeighting");

//    DecreaseWidth(container, 0, undefined, function () {
//        container.style.display = "none";
//        container.style.width = "0px";

//        if (defaultWeightingHasChanged) {
//            if (window.location.toString().search("/LinkReporter/Crosstabs.aspx") != -1)
//                ReloadCrosstable();
//        }
//    });
//}