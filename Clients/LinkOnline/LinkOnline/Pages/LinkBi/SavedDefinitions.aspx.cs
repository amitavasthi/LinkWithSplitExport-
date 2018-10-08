using LinkBi1.Classes;
using LinkBi1.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.LinkBi
{
    public partial class SavedDefinitions : WebUtilities.BasePage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the currently
        /// selected LinkBi definition file.
        /// </summary>
        public string SelectedDefinition
        {
            get
            {
                if (HttpContext.Current.Session["LinkBiSelectedReport"] == null)
                    return null;

                return (string)HttpContext.Current.Session["LinkBiSelectedReport"];
            }
            set
            {
                HttpContext.Current.Session["LinkBiSelectedReport"] = value;
            }
        }

        public List<LinkBiDefinition> OutdatedDefinitions { get; set; }

        #endregion


        #region Constructor

        public SavedDefinitions()
        {
            this.OutdatedDefinitions = new List<LinkBiDefinition>();
        }

        #endregion


        #region Methods

        private void BindDefinitionList()
        {
            string directory = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedLinkBiDefinitions",
                Global.Core.ClientName
            );

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Get all saved LinkBi definitions of the client.
            foreach (string fileName in Directory.GetFiles(directory))
            {
                LinkBiDefinitionSelector selector = new LinkBiDefinitionSelector(
                    fileName,
                    Global.HierarchyFilters[fileName]
                );

                if (this.SelectedDefinition == fileName)
                    selector.CssClass = "LinkBiDefinitionSelector_Active";

                if (!selector.UpToDate)
                    this.OutdatedDefinitions.Add(selector.Definition);

                pnlDefinitions.Controls.Add(selector);
            }
        }

        private void BindMessages()
        {
            // Run through all definitions that have outdated data.
            foreach (LinkBiDefinition definition in this.OutdatedDefinitions)
            {
                Panel pnlMessage = new Panel();
                pnlMessage.CssClass = "LinkBiDefinitionMessage LinkBiDefinitionMessageError";

                Label lblMessage = new Label();
                lblMessage.Text = string.Format(
                    Global.LanguageManager.GetText("LinkBiDefinitionMessageOudatedData"),
                    definition.Properties.Name
                );

                pnlMessage.Attributes.Add("onclick", string.Format(
                    "LoadLinkBiDefinitionProperties(this, '{0}');",
                    definition.FileName.Replace("\\", "/")
                ));

                pnlMessage.Controls.Add(lblMessage);

                pnlDefinitionMessages.Controls.Add(pnlMessage);
            }
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            BindDefinitionList();

            if (this.SelectedDefinition != null && File.Exists(this.SelectedDefinition) == false)
            {
                this.SelectedDefinition = null;
            }

            if (this.SelectedDefinition != null)
            {
                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "LoadProperties",
                    "loadFunctions.push(function() { LoadLinkBiDefinitionProperties(document.body, '" + this.SelectedDefinition.Replace("\\", "/") + "') });",
                    true
                );
            }
            else
            {
                BindMessages();
            }

            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "LinkBiDefinitions",
                Global.Core.ClientName,
                Global.User.Id + ".xml"
            );

            HttpContext.Current.Session["LinkBiDefinition"] = fileName;
        }

        #endregion
    }
}