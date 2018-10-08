using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.ClientManager
{
    public partial class ManageUserGroupsHome :WebUtilities.BasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnManage_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("ManageGroup.aspx", false);
        }


        protected void btnCreate_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("CreateGroup.aspx",false);
        }
    }
}