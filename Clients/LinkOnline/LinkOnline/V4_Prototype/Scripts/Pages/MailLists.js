Pages.MailLists = {
    Load: function () {
        Menu.ClearSubMenu();
        RequestAsynch("Service.ashx", "LoadMailLists", "", function (mailLists) {
            mailLists = JSON.parse(decodeURI(mailLists));

            var items = [];

            for (var i = 0; i < mailLists.length; i++) {
                items.push({
                    Id: "pnlSubMenuItem" + mailLists[i].Id,
                    Label: mailLists[i].Name,
                    Click: "Pages.MailLists.LoadMailList('" + mailLists[i].Id + "');"
                });
            }

            Menu.ShowSubMenu(items, true);

        }, function () {
            ShowError("An unexpected error occurred while loading the e-mail lists.<br />" +
                "Please contact the server administrator or try again later.");
        });
    },
    LoadMailList: function (id) {
        RequestAsynch("Service.ashx", "LoadMailList", "Id=" + id, function (mailList) {
            mailList = JSON.parse(mailList);
            document.content.innerHTML = "";

            /* #### Recipients ##### */
            document.content.appendChild(Render.LineBreak());
            container = Render.ContentBox("Recipients (" + mailList.Recipients.length + ")");

            document.content.appendChild(container);
            Effects.BlendIn(container);

            container = container.Content;

            var rows = [];

            for (var i = 0; i < mailList.Recipients.length; i++) {
                rows.push([
                    { Text: mailList.Recipients[i].FirstName, Class: "" },
                    { Text: mailList.Recipients[i].LastName, Class: "" },
                    { Text: mailList.Recipients[i].Mail, Class: "" }
                ]);
            }

            var table = Render.Table([
                { Text: "first name", Class: "TableCellHeader BackgroundColor2" },
                { Text: "last name", Class: "TableCellHeader BackgroundColor2" },
                { Text: "mail", Class: "TableCellHeader BackgroundColor2" }
            ], rows);

            table.className = "DataTable";

            container.appendChild(table);
        }, function () {
            ShowError("An unexpected error occurred while loading the e-mail lists.<br />" +
                "Please contact the server administrator or try again later.");
        });
    }
};