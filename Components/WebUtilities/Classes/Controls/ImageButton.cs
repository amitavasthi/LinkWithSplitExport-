using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtilities.Controls
{
    public class ImageButton : System.Web.UI.WebControls.ImageButton
    {
        #region Constructor

        public ImageButton()
        {
            this.Load += ImageButton_Load;
        }

        #endregion


        #region Event Handlers

        protected void ImageButton_Load(object sender, EventArgs e)
        {
            // Check if the source has parameters.
            if (this.ImageUrl.Contains("?"))
                this.ImageUrl += "&";
            else
                this.ImageUrl += "?";

            // Add the version parameter to the source to
            // avoid caching across platform versions.
            this.ImageUrl += "Version=" + System.Web.HttpContext.Current.Session["Version"];
        }

        #endregion
    }
}
