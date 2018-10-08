
function GetContentByID(idvar) {


    var content;

    $.ajax({
        type: "POST",
        async: false,
        url: "/Handlers/HomePageHandler.ashx/GetContent",
        data: "Method=GetContent&idvar=" + idvar,
        contentType: "application/x-www-form-urlencoded",
        success: function (r) {

            content = r;
        }



    });

    return content;


}



function GetChartClick() {
    

    var content;

    $.ajax({
        type: "POST",
        async: false,
        url: "/Handlers/HomePageHandler.ashx/GetChartClick",
        data: "Method=GetChartClick",  
        contentType: "application/x-www-form-urlencoded", 
        success: function (r) {

            content = r;
        }



    });

    
    return content;


}

function getWidgetsData()
{
    var content;

    $.ajax({
        type: "POST",
        async: false,
        url: "/Handlers/HomePageHandler.ashx/GetWidgetsData",
        data: "Method=GetWidgetsData",
        contentType: "application/x-www-form-urlencoded",
        success: function (r) {

            content = r;
        }



    });


    return content;
}

function saveWidgetsOrder(order) {

    $.ajax({
        type: "POST",
        async: false,
        url: "/Handlers/HomePageHandler.ashx/SaveWidgetsOrder",
        data: "Method=SaveWidgetsOrder&order=" + order,
        contentType: "application/x-www-form-urlencoded"

    });

}

function addComponent(data)
{
    $.ajax({
        type: "POST",
        async: false,
        url: "/Handlers/HomePageHandler.ashx/AddComponent",
        data: "Method=AddComponent&data=" + data,
        contentType: "application/x-www-form-urlencoded"

    });
}

function removeComponent(data) {
    $.ajax({
        type: "POST",
        async: false,
        url: "/Handlers/HomePageHandler.ashx/removeComponent",
        data: "Method=RemoveComponent&data=" + data,
        contentType: "application/x-www-form-urlencoded"

    });

    

}


function savedReportsClickJS(elem) {
   
    var src = $(elem).attr("Source");
      
    var loc;
   
    $.ajax({
        type: "POST",
        async: false,
        url: "/Handlers/HomePageHandler.ashx/SavedReportClick",
        data: "Method=SavedReportClick&data=" + src,
        contentType: "application/x-www-form-urlencoded",
        success: function (r) {

            loc = r;
        }



    });
    

    return loc;
}

    
