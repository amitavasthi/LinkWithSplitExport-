using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebUtilities.Controls;

namespace WebUtilities
{
    public class NavigationMenu : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the xml document that
        /// contains the menu definition.
        /// </summary>
        public XmlDocument XmlDocument { get; set; }

        #endregion


        #region Constructor

        public NavigationMenu(string id, XmlDocument xmlDocument)
        {
            this.Load += NavigationMenu_Load;

            this.ID = id;
            this.XmlDocument = xmlDocument;
        }

        public NavigationMenu(string id, string fileName)
        {
            this.Load += NavigationMenu_Load;

            this.ID = id;

            this.XmlDocument = new XmlDocument();
            this.XmlDocument.Load(fileName);
        }

        #endregion


        #region Methods

        public void Render()
        {
            base.CssClass = "NavigationMenu";

            // Create a new html table that contains the menu items.
            Table table = new Table();
            table.ID = this.ID + "Table";

            // Add the html table containing the menu items
            // to the navigation control's child controls.
            base.Controls.Add(table);

            // Select all menu item definition xml nodes on the root level.
            XmlNodeList xmlNodes = this.XmlDocument.DocumentElement.SelectNodes("MenuItem");

            var maxRowIndex = 0;
            var maxCellIndex = 0;

            // Run through all menu item definition xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                // Create a new menu item by the definition xml node.
                NavigationMenuItem item = new NavigationMenuItem(
                    this,
                    xmlNode
                );

                if (item.Row >= table.Rows.Count)
                {
                    // Add a new table row to the table.
                    table.Rows.Add(new TableRow());
                }

                while (item.Position >= table.Rows[item.Row].Cells.Count)
                {
                    // Add a new table cell to the table row.
                    table.Rows[item.Row].Cells.Add(new TableCell()
                    {
                        CssClass = "TableCellNavigationMenuItem BorderColor1 Color1"
                    });
                }

                // Add the menu item control to the
                // defined position in the table
                table.Rows[item.Row].Cells[item.Position].Controls.Add(item);

                if (maxRowIndex < item.Row)
                    maxRowIndex = item.Row;

                if (maxCellIndex < item.Position)
                    maxCellIndex = item.Position;

                item.Render();
            }

            for (int i = table.Rows[maxRowIndex].Cells.Count; i < (maxCellIndex + 1); i++)
            {
                // Add a new table cell to the table row.
                table.Rows[maxRowIndex].Cells.Add(new TableCell()
                {
                    CssClass = "TableCellNavigationMenuItem BorderColor1 Color1"
                });
            }
        }

        #endregion


        #region Event Handlers

        protected void NavigationMenu_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }
}
