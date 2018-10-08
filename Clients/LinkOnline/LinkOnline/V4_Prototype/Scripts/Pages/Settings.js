Pages.Settings = {
    Load: function () {
        document.content.innerHTML = "";

        RequestAsynch("Service.ashx", "LoadUsers", "", function (users) {
            users = JSON.parse(users);

            Menu.HideSubMenu();

            /* #### Users ##### */
            document.content.appendChild(Render.LineBreak());
            container = Render.ContentBox("Users");

            document.content.appendChild(container);
            Effects.BlendIn(container);

            container = container.Content;

            var rows = [];

            for (var i = 0; i < users.length; i++) {
                rows.push([
                    { Text: users[i].Name, Class: "" },
                    { Text: users[i].FirstName, Class: "" },
                    { Text: users[i].LastName, Class: "" },
                    { Text: users[i].Mail, Class: "" }
                ]);
            }

            var table = Render.Table([
                { Text: "username", Class: "TableCellHeader BackgroundColor2" },
                { Text: "first name", Class: "TableCellHeader BackgroundColor2" },
                { Text: "last name", Class: "TableCellHeader BackgroundColor2" },
                { Text: "mail", Class: "TableCellHeader BackgroundColor2" }
            ], rows);

            table.className = "DataTable";

            container.appendChild(table);
        }, function () {
            ShowError("An unexpected error occurred while loading the users.<br />" +
                "Please contact the server administrator or try again later.");
        });

        /* #### E-Mail settings ##### */
        container = Render.ContentBox("E-Mail settings");

        document.content.appendChild(container);
        Effects.BlendIn(container);

        container = container.Content;

        container.appendChild(Render.Table([], [
            [{ Text: "server", Class: "TableCellTitle" }, { Text: "<input type=\"text\" value=\"smtpout.secureserver.net\" />", Class: "TableCellValue" }],
            [{ Text: "username", Class: "TableCellTitle" }, { Text: "<input type=\"text\" value=\"admin@linkmr.com\" />", Class: "TableCellValue" }],
            [{ Text: "password", Class: "TableCellTitle" }, { Text: "<input type=\"text\" value=\"test\" />", Class: "TableCellValue" }],
            [{ Text: "SSL", Class: "TableCellTitle" }, { Text: "<input type=\"checkbox\" checked=\"CHECKED\" />", Class: "TableCellValue" }],
            [{ Text: "Use credential cache", Class: "TableCellTitle" }, { Text: "<input type=\"checkbox\" />", Class: "TableCellValue" }]
        ]));
    }
};