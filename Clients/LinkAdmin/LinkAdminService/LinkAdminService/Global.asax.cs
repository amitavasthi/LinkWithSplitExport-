using LinkAdminService.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace LinkAdminService
{
    public class Global : System.Web.HttpApplication
    {
        #region Properties

        public static LinkSynch Synch { get; set; }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        public static void Log(string message, LogType type)
        {
            string fileName = Path.Combine(
                ConfigurationManager.AppSettings["ApplicationPath"],
                "Logs",
                DateTime.Today.ToString("yyyyMMdd") + ".txt"
            );

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.AppendAllText(fileName,string.Format(
                "<LogEntry Timestamp=\"{0}\" Type=\"{1}\"><![CDATA[{2}]]></LogEntry>",
                DateTime.Now.ToString(),
                type,
                message
            ));
        }

        #endregion


        #region Event Handlers

        protected void Application_Start(object sender, EventArgs e)
        {
            Synch = new LinkSynch(int.Parse(
                ConfigurationManager.AppSettings["SynchInterval"]
            ));
            Synch.Synch();
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
            Log(Server.GetLastError().ToString(), LogType.Error);
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        #endregion
    }

    public enum LogType
    {
        Error, Information, Success
    }
}