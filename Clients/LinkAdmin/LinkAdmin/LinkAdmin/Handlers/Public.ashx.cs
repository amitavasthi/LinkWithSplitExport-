using ApplicationUtilities.Cluster;
using LinkAdmin.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LinkAdmin.Handlers
{
    /// <summary>
    /// Summary description for Public
    /// </summary>
    public class Public : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public Public()
            : base(false, true)
        {
            base.Methods.Add("GetServerState", GetServerState);
            base.Methods.Add("WhoAmI", WhoAmI);
            base.Methods.Add("GetServers", GetServers);
            base.Methods.Add("SetServers", SetServers);
        }

        #endregion


        #region Web Methods

        private void GetServerState(HttpContext context)
        {
            string ip;

            if (context.Request.Params["IP"] != null)
                ip = context.Request.Params["IP"];
            else
                ip = context.Request.ServerVariables["REMOTE_HOST"];

            ServerCollection servers = new ServerCollection(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            ));

            foreach (string server in servers.Items.Keys)
            {
                if (server != ip)
                    continue;

                context.Response.Write(servers.Items[server].State.ToString());

                break;
            }
        }

        private void WhoAmI(HttpContext context)
        {
            string ip;

            if (context.Request.Params["IP"] != null)
                ip = context.Request.Params["IP"];
            else
                ip = context.Request.ServerVariables["REMOTE_HOST"];

            ServerCollection servers = new ServerCollection(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            ));

            foreach (string server in servers.Items.Keys)
            {
                if (server != ip)
                    continue;

                context.Response.Write(servers.Items[server].Description);

                break;
            }
        }

        private void GetServers(HttpContext context)
        {
            context.Response.Write(File.ReadAllText(Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            )));
        }

        private void SetServers(HttpContext context)
        {
            File.WriteAllText(Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            ), HttpUtility.UrlDecode(context.Request.Params["Data"]));
        }


        #endregion
    }
}