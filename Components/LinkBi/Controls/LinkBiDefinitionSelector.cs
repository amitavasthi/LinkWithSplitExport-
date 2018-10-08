using Crosstables.Classes.HierarchyClasses;
using LinkBi1.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtilities.Controls;

namespace LinkBi1.Controls
{
    public class LinkBiDefinitionSelector : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the LinkBi definition the selector is for.
        /// </summary>
        public LinkBiDefinition Definition { get; set; }

        public bool UpToDate { get; set; }

        #endregion


        #region Constructor

        public LinkBiDefinitionSelector(string fileName, HierarchyFilter hierarchyFilter)
        {
            this.Definition = new LinkBiDefinition(
                base.Core, 
                fileName,
                hierarchyFilter
            );

            this.UpToDate = this.Definition.IsUpToDate();

            this.Load += LinkBiDefinitionSelector_Load;
        }

        #endregion


        #region Methods


        public void Render()
        {
            base.CssClass = "LinkBiDefinitionSelector " + base.CssClass;

            base.Attributes.Add(
                "onclick",
                "LoadLinkBiDefinitionProperties(this, '" + this.Definition.FileName.Replace("\\", "/") + "');"
            );

            if (this.Definition != null)
                RenderSelector();
            else
                RenderError();
        }


        private void RenderSelector()
        {
            Table table = new Table();

            TableRow tableRowName = new TableRow();
            TableRow tableRowStatus = new TableRow();
            TableRow tableRowLatestUpdate = new TableRow();

            TableCell tableCellIcon = new TableCell();
            tableCellIcon.RowSpan = 3;

            TableCell tableCellName = new TableCell();
            TableCell tableCellStatus = new TableCell();
            TableCell tableCellLatestUpdate = new TableCell();

            tableCellName.CssClass = "TableCellName";
            tableCellStatus.CssClass = "TableCellStatus";
            tableCellLatestUpdate.CssClass = "TableCellLatestUpdate";

            tableCellStatus.Attributes.Add("id", "TableCellStatus" + this.Definition.FileName.Replace("\\", "/"));

            tableCellIcon.Text = string.Format(
                "<img src=\"/Images/Icons/LinkBi/{0}.png\" />",
                this.HasConnection() ? "Server" : "Download"
            );

            tableCellName.Attributes.Add(
                "id",
                "LinkBiDefinitionSelectorName" + this.Definition.FileName.Replace("\\", "")
            );

            tableCellName.Text = this.Definition.Properties.Name;

            tableCellLatestUpdate.Text = this.Definition.Properties.LatestUpdate.ToString(
                base.LanguageManager.GetText("DateFormat") + " " +
                base.LanguageManager.GetText("TimeFormat")
            );

            if (this.UpToDate)
                tableCellStatus.Text = base.LanguageManager.GetText("LinkBiDefinitionUpToDate");
            else
                tableCellStatus.Text = base.LanguageManager.GetText("LinkBiDefinitionOutdated");

            tableRowName.Cells.Add(tableCellIcon);
            tableRowName.Cells.Add(tableCellName);
            tableRowStatus.Cells.Add(tableCellStatus);
            tableRowLatestUpdate.Cells.Add(tableCellLatestUpdate);

            table.Rows.Add(tableRowName);
            table.Rows.Add(tableRowStatus);
            table.Rows.Add(tableRowLatestUpdate);

            base.Controls.Add(table);

            table.Attributes.Add("oncontextmenu", string.Format(
                "DeleteSavedReport(this, '{0}');return false;",
                this.Definition.FileName.Replace("\\", "/")
            ));
        }

        private void RenderError()
        {

        }

        private bool HasConnection()
        {
            return this.Definition.Properties.ServerConnections.Count > 0;
        }

        #endregion


        #region Event Handlers

        protected void LinkBiDefinitionSelector_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }
}
