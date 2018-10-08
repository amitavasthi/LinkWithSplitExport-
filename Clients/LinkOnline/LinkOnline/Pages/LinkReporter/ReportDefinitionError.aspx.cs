using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.LinkReporter
{
    public partial class ReportDefinitionError : WebUtilities.BasePage
    {
        #region Properties

        #endregion


        #region Constructor

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["LastReportDefinitionError"] != null)
                lblReportDefinitionErrorMessage.Text = ((Exception)HttpContext.Current.Session["LastReportDefinitionError"]).ToString();
        }

        protected void btnTryAgain_Click(object sender, EventArgs e)
        {
            Response.Redirect("Crosstabs.aspx");
        }

        protected void btnResetReportDefinition_Click(object sender, EventArgs e)
        {
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            if (File.Exists(fileName))
                File.Delete(fileName);

            Response.Redirect("Crosstabs.aspx");
        }

        #endregion
    }
}