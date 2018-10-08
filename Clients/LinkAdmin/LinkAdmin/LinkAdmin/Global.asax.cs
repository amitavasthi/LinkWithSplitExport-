using LinkAdmin.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using WebUtilities;

namespace LinkAdmin
{
    public class Global : System.Web.HttpApplication
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database core of the web application's session.
        /// </summary>
        public static LinkAdminCore Core
        {
            get
            {
                return (LinkAdminCore)HttpContext.Current.Session["Core"];
            }
            set
            {
                HttpContext.Current.Session["Core"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the permission core of the web application's session.
        /// </summary>
        public static PermissionCore.PermissionCore PermissionCore
        {
            get
            {
                return (PermissionCore.PermissionCore)HttpContext.Current.Session["PermissionCore"];
            }
            set
            {
                HttpContext.Current.Session["PermissionCore"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the language manager of the current session.
        /// </summary>
        public static LanguageManager LanguageManager
        {
            get
            {
                return (LanguageManager)HttpContext.Current.Session["LanguageManager"];
            }
            set
            {
                HttpContext.Current.Session["LanguageManager"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the language of the current session.
        /// </summary>
        public static Language Language
        {
            get
            {
                return (Language)HttpContext.Current.Session["Language"];
            }
            set
            {
                HttpContext.Current.Session["Language"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the authenticated user of the current session.
        /// </summary>
        public static Guid? IdUser
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null)
                    return null;

                return (Guid)HttpContext.Current.Session["User"];
            }
            set
            {
                HttpContext.Current.Session["User"] = value;
            }
        }


        #endregion


        #region Constructor

        #endregion


        #region Methods

        #endregion


        #region Event Handlers


        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Core = new LinkAdminCore(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "DATA"
            ));

            // Initialize the session's language manager.
            LanguageManager = new LanguageManager("", HttpContext.Current.Request.PhysicalApplicationPath);
            Language = LanguageManager.DefaultLanguage;

            // Initialize the session's permission core.
            PermissionCore = new PermissionCore.PermissionCore(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "LinkAdmin", 
                ""
            );
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            string fileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Logs",
                DateTime.Today.ToString("yyyyMMdd") + ".txt"
            );

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.AppendAllText(
                fileName,
                Server.GetLastError().ToString() + Environment.NewLine + Environment.NewLine
            );
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        #endregion
    }
}