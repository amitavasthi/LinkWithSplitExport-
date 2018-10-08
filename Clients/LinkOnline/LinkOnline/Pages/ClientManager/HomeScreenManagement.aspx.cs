using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.ClientManager
{
    public partial class HomeScreenManagement : WebUtilities.BasePage
    {
        #region EventHandlers
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            //Response.Redirect("HomeManagement.aspx", false);
            Response.Redirect("DashboardItems.aspx", false);
        }

        protected void btnNews_Click(object sender, EventArgs e)
        {
            Response.Redirect("NewsDetails.aspx", false);
        }
        protected void btnDesigner_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Pages/HomeScreen.aspx", false);
        }
        #endregion

      
    }
}