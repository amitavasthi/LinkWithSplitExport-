using LinkOnline.Classes.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkOnline.Pages.DataManagement
{
    public partial class HierarchyFilter : WebUtilities.BasePage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the study.
        /// </summary>
        public Guid IdStudy { get; set; }

        /// <summary>
        /// Gets or sets the full path to the study's
        /// hierarchy filter definition file.
        /// </summary>
        public string FileName { get; set; }

        #endregion


        #region Constructor

        public HierarchyFilter()
        {

        }

        #endregion


        #region Methods

        private void InitFile()
        {
            // Build the full path to the study's
            // hierarchy filter definition file.
            this.FileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "HierarchyFilters",
                Global.Core.ClientName,
                this.IdStudy + ".xml"
            );

            if (!Directory.Exists(Path.GetDirectoryName(this.FileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(this.FileName));

            if (!File.Exists(this.FileName))
            {
                File.WriteAllText(this.FileName, "<HierarchyFilters></HierarchyFilters>");
            }
        }

        private void BindHierarchyFilters()
        {
            XmlDocument document = new XmlDocument();
            document.Load(this.FileName);

            int width = (int)(100 / (document.DocumentElement.ChildNodes.Count + 1));

            // Run through all hierarchy filters defined for the study.
            foreach (XmlNode xmlNode in document.DocumentElement.ChildNodes)
            {
                HierarchyFilterDefinition filterDefinition = new HierarchyFilterDefinition(xmlNode);
                filterDefinition.Style.Add("width", width + "%");

                pnlHierarchyFilters.Controls.Add(filterDefinition);
            }

            LiteralControl pnlAddHierarchyFilter = new LiteralControl(
                "<table cellpadding=\"0\" cellspacing=\"0\" style=\"width:" + (width - 1) + "%;height:100%\"><tr><td align=\"center\">" +
                "<img src=\"/Images/Icons/Add5.png\" /></td></tr></table>");

            pnlHierarchyFilters.Controls.Add(pnlAddHierarchyFilter);
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if a study id is applied.
            if (Request.Params["IdStudy"] == null)
                Response.Redirect("Overview.aspx");

            // Set the id of the study.
            this.IdStudy = Guid.Parse(Request.Params["IdStudy"]);

            InitFile();

            BindHierarchyFilters();
        }

        #endregion
    }
}