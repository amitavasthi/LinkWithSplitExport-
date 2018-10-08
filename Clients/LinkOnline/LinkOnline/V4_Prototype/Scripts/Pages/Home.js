Pages.Home = {
    Load: function () {
        Menu.HideSubMenu();

        document.content.innerHTML = "";

        return;

        /* #### Log ##### */
        document.content.appendChild(Render.LineBreak());
        container = Render.ContentBox("Log");

        document.content.appendChild(container);
        Effects.BlendIn(container);

        container = container.Content;

        container.appendChild(Render.Table([], [
            [{ Text: "11/16/2016 10:46am", Class: "TableCellLogDate" }, { Class: "TableCellLog_Successful", Text: "Report has been created successfully and has been sent out to 21 recipients." }],
            [{ Text: "11/16/2016 10:41am", Class: "TableCellLogDate" }, { Text: "Report creation process has started." }],
            [{ Text: "11/16/2016 10:41am", Class: "TableCellLogDate" }, { Class: "TableCellLog_Successful", Text: "Data update process has finished successfully. Data for 452 radio stations has been updated." }],
            [{ Text: "11/16/2016 07:03am", Class: "TableCellLogDate" }, { Class: "TableCellLog_Neutral", Text: "New data is available. Data update process has started." }],
            [{ Text: "11/16/2016 07:00am", Class: "TableCellLogDate" }, { Text: "Checking if new data is available." }],
            [{ Text: "11/16/2016 06:03am", Class: "TableCellLogDate" }, { Text: "No new data available yet. Postponing data update process by 1 hour." }],
            [{ Text: "11/16/2016 06:00am", Class: "TableCellLogDate" }, { Text: "Checking if new data is available." }],
            [{ Text: "11/16/2016 05:59am", Class: "TableCellLogDate" }, { Class: "TableCellLog_Error", Text: "An unexpected error occurred while starting the data update app. The administrator has been informed." }]
        ]));

        /* #### Progress ##### */
        document.content.appendChild(Render.LineBreak());
        container = Render.ContentBox("Data update progress");

        document.content.appendChild(container);
        Effects.BlendIn(container);

        container = container.Content;

        var label = document.createElement("div");
        label.id = "lblDataUpdateProgress";
        label.style.position = "absolute";
        label.style.marginLeft = "115px";
        label.style.marginTop = "62px";
        label.style.width = "80px";
        label.style.textAlign = "center";
        label.style.fontSize = "16pt";
        
        container.appendChild(label);


        var canvas = document.createElement("canvas");
        canvas.id = "cvDataUpdateProgress"
        container.appendChild(canvas);

        UpdateProgress(Math.floor((Math.random() * 100) + 1));
        //UpdateProgress(100);
    }
};

function UpdateProgress(progress) {
    var label = document.getElementById("lblDataUpdateProgress");
    var canvas = document.getElementById("cvDataUpdateProgress");

    label.innerHTML = progress + "%";

    var angle = 2 * Math.PI / (100 / progress);

    var ctx = canvas.getContext("2d");
    ctx.fillStyle = "#FFFFFF";
    ctx.fillRect(0, 0, 200, 200);
    ctx.beginPath();
    ctx.strokeStyle = "#45a4c8";
    ctx.lineWidth = 10;
    ctx.arc(150, 75, 50, (-0.5 * Math.PI), angle - (.5 * Math.PI));
    ctx.stroke();
}