using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace WebUtilities.Controls
{
    public class StylesheetReference : WebControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the url to the source file.
        /// </summary>
        public string Source { get; set; }

        #endregion


        #region Constructor

        public StylesheetReference()
            : base("link")
        {
            this.Load += StylesheetReference_Load;
        }

        #endregion


        #region Event Handlers

        protected void StylesheetReference_Load(object sender, EventArgs e)
        {
            // Check if the source has parameters.
            if (this.Source.Contains("?"))
                this.Source += "&";
            else
                this.Source += "?";

            // Add the version parameter to the source to
            // avoid caching across platform versions.
            this.Source += "Version=" + System.Web.HttpContext.Current.Session["Version"];

            // Set the content type of the target source file.
            this.Attributes.Add("type", "text/css");
            this.Attributes.Add("rel", "stylesheet");

            // Set the href attribute.
            this.Attributes.Add("href", this.Source);
        }

        #endregion
    }
}
