using Azure_Switch.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Azure_Switch
{
    public class Global : System.Web.HttpApplication
    {
        #region Properties

        public static Dictionary<string, string> Sessions { get; set; }

        #endregion


        #region Event Handlers

        protected void Application_Start(object sender, EventArgs e)
        {
            Sessions = new Dictionary<string, string>();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        #endregion


        #region Methods

        #endregion
    }
}