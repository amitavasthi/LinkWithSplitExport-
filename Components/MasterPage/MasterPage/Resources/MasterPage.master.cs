using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DatabaseCore.Items;
using System.IO;
using MasterPage.Classes;
using System.Web.Configuration;

namespace MasterPage1
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        #region Properties

        /// <summary>
        /// Gets the authenticated user of the session.
        /// </summary>
        public User User
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null)
                    return null;

                return this.Core.Users.GetSingle((Guid)HttpContext.Current.Session["User"]);
            }
        }

        /// <summary>
        /// Gets the database core of the web application's session.
        /// </summary>
        public DatabaseCore.Core Core
        {
            get
            {
                return (DatabaseCore.Core)HttpContext.Current.Session["Core"];
            }
        }

        /// <summary>
        /// Gets the database core of the web application's session.
        /// </summary>
        public string Server
        {
            get
            {
                return (string)HttpContext.Current.Session["Server"];
            }
        }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void BindNavigation()
        {
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "Navigation.xml"
            );

            if (!File.Exists(fileName))
                return;

            Navigation navigation = new Navigation(fileName, true);

            pnlNavigation.Controls.Add(navigation);
        }

        private void BindSettingsNavigation()
        {
            
        }

        private void GetSessionDetails()
        {
            //Session["Reset"] = true;
            //Configuration config = WebConfigurationManager.OpenWebConfiguration("~/Web.Config");
            //SessionStateSection section = (SessionStateSection)config.GetSection("system.web/sessionState");
            //int timeout = (int)section.Timeout.TotalMinutes * 1000 * 60;
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "SessionAlert", "SessionExpireAlert(" + timeout + ");", true);
        }

        private void RenderColorScheme()
        {
            ColorSchemeStylesheet stylesheet = new ColorSchemeStylesheet();

            //this.Page.Header.Controls.AddAt(2, stylesheet);
            //this.Page.Header.Controls.Add(stylesheet);
            phColorScheme.Controls.Add(stylesheet);
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Server != null)
            {
                Response.AddHeader("LiNK-Server", this.Server);
            }

            BindSettingsNavigation();
            BindNavigation();
            RenderColorScheme();
            GetSessionDetails();

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "InitTimeoutWarning",
                "InitTimeoutWarning(" + ((Session.Timeout * 60000) - 60000) + ");",
                true
            );
        }

        #endregion
    }
}