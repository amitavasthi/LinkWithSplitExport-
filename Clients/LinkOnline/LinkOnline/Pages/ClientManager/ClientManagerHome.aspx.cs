using ApplicationUtilities;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.ClientManager
{
    public partial class ClientManagerHome : WebUtilities.BasePage
    {
        #region Properties
        public string password;
        #endregion
        #region Method


        #endregion
        #region EventHandlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["msg"] != null)
                {
                    switch (Request.QueryString["msg"].Trim())
                    {
                        case "1":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("UserAddMsg"),
                            Global.LanguageManager.GetText("UserAddMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "2":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString()),
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString())), WebUtilities.MessageType.Error);
                            break;
                        case "3":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("DuplicateError"),
                            Global.LanguageManager.GetText("DuplicateError")), WebUtilities.MessageType.Error);
                            break;

                    }

                }
            }
        }

        protected void btnManage_OnClick(object sender, EventArgs e)
        {
            if (Global.User.HasPermission(111) && Global.User.HasPermission(112))
            {
                Response.Redirect("UsersHome.aspx", false);
            }
            else
            {
                if (Global.User.HasPermission(111))
                {
                    Response.Redirect("CreateUser.aspx", false);
                }
                else if (Global.User.HasPermission(112))
                {
                    Response.Redirect("ManageUsers.aspx", false);
                }
                
            }
        }
        protected void btnPortalSettings_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("ClientSettings.aspx", false);
        }

        protected void btnUserGroups_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("ManageUserGroupsHome.aspx", false);
        }

        protected void btnNews_Click(object sender, EventArgs e)
        {
            Response.Redirect("NewsDetails.aspx", false);
        }

        protected void btnHomeMgmt_Click(object sender, EventArgs e)
        {
            Response.Redirect("HomeScreenManagement.aspx", false);
        }

        #endregion



    }
}