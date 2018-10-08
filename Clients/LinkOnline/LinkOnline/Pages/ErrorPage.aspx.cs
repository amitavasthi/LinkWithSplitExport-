using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages
{
    public partial class ErrorPage : WebUtilities.BasePage 
    {
        public ErrorPage() :
            base(true, true)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["Error"] != null)
            {
                string msg = HttpContext.Current.Session["Error"].ToString();
                lblError.Text = msg;
                lblError.Name = msg;
                lblError.ForeColor = System.Drawing.Color.Red;

            }
        }
    }
}