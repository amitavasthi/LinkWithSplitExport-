using LinkBi1.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using WebUtilities.Controls;

namespace LinkBi1.Controls
{
    public class LinkBiDefinitionPropertiesControl : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the LinkBi definition of which
        /// the properties should be displayed.
        /// </summary>
        public LinkBiDefinition Definition { get; set; }

        #endregion


        #region Constructor

        public LinkBiDefinitionPropertiesControl(LinkBiDefinition definition)
        {
            this.Definition = definition;

            this.Load += LinkBiDefinitionPropertiesControl_Load;
        }

        #endregion


        #region Methods

        public void Render()
        {
            base.CssClass = "LinkBiDefinitionProperties";

            Table table = new Table();

            TableRow tableRowName = new TableRow();

            TableCell tableCellNameTitle = new TableCell();
            TableCell tableCellNameValue = new TableCell();
            TableCell tableCellButtons = new TableCell();

            tableCellButtons.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;

            System.Web.UI.WebControls.Label lblName = new System.Web.UI.WebControls.Label();
            lblName.Text = base.LanguageManager.GetText("Name");

            TextBox txtName = new TextBox();
            txtName.Text = this.Definition.Properties.Name;

            txtName.Attributes.Add(
                "onchange",
                "UpdateLinkBiSavedReportName('" + this.Definition.FileName.Replace("\\", "/") + "', this.value, '" + this.Definition.FileName.Replace("\\", "") + "')"
            );

            OptionSwipe swiper = new OptionSwipe();

            Option optionUpdate = new Option();
            Option optionDownload = new Option();
            Option optionEdit = new Option();

            optionUpdate.Text = base.LanguageManager.GetText("Update");
            optionDownload.Text = base.LanguageManager.GetText("Download");
            optionEdit.Text = base.LanguageManager.GetText("Edit");

            optionUpdate.Style.Add("background", "#61CF71");
            optionDownload.CssClass = "BackgroundColor1";
            optionEdit.Style.Add("background", "#FFA719");

            optionUpdate.OnClientClick = string.Format(
                "DeployLinkBiReport('{0}', [{1}]);return false;",
                this.Definition.FileName.Replace("\\", "/"),
                string.Join(",", this.Definition.Properties.ServerConnections.Values.Select(x => "'" + x.Identity + "'"))
            );

            optionDownload.OnClientClick = string.Format(
                "DownloadLinkBiReport('{0}');return false;",
                this.Definition.FileName.Replace("\\", "/")
            );

            optionEdit.OnClientClick = string.Format(
                "EditLinkBiReport('{0}');return false;",
                this.Definition.FileName.Replace("\\", "/")
            );

            if (this.Definition.Properties.ServerConnections.Count > 0)
                swiper.Options.Add(optionUpdate);

            swiper.Options.Add(optionDownload);
            swiper.Options.Add(optionEdit);

            optionUpdate.Render();
            optionDownload.Render();
            optionEdit.Render();

            swiper.Render();

            tableCellNameTitle.Controls.Add(lblName);
            tableCellNameValue.Controls.Add(txtName);
            tableCellButtons.Controls.Add(swiper);

            tableRowName.Cells.Add(tableCellNameTitle);
            tableRowName.Cells.Add(tableCellNameValue);
            tableRowName.Cells.Add(tableCellButtons);

            table.Rows.Add(tableRowName);

            TableRow tableRowServerConnections = new TableRow();
            TableCell tableCellServerConnections = new TableCell();
            tableCellServerConnections.ColumnSpan = 3;

            foreach (LinkBiServerConnection serverConnection in this.Definition.Properties.ServerConnections.Values)
            {
                LinkBiServerConnectionControl serverConnectionControl = new LinkBiServerConnectionControl(serverConnection);

                serverConnectionControl.Render();

                if (Page != null)
                {
                    Page.ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "CheckLinkBiServerConnection" + serverConnection.Identity,
                        "loadFunctions.push(function() { CheckLinkBiServerConnection('" + this.Definition.FileName.Replace("\\", "/") + "', '" + serverConnection.Identity + "') });",
                        true
                    );
                }

                tableCellServerConnections.Controls.Add(serverConnectionControl);
            }

            tableCellServerConnections.Controls.Add(new LiteralControl("<div style=\"clear:both\"></div>"));

            tableCellServerConnections.Controls.Add(new LiteralControl(string.Format(
                "<img onclick=\"AddServerConnection('{0}');\" style=\"cursor:pointer\" src=\"/Images/Icons/Cloud/NewDirectory.png\" " +
                "onmouseover=\"this.src='/Images/Icons/Cloud/NewDirectory_Hover.png';\" onmouseout=\"this.src='/Images/Icons/Cloud/NewDirectory.png';\" />",
                this.Definition.FileName.Replace("\\", "/")
            )));

            tableRowServerConnections.Cells.Add(tableCellServerConnections);

            table.Rows.Add(tableRowServerConnections);

            base.Controls.Add(table);
        }

        #endregion


        #region Event Handlers

        protected void LinkBiDefinitionPropertiesControl_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }
}
