using ApplicationUtilities;
using ApplicationUtilities.Cluster;
using LinkAdmin.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkAdmin.Pages
{
    public partial class Console : Classes.Page
    {
        #region Properties

        #endregion


        #region Constructor

        #endregion


        #region Methods

        public override void Render()
        {
            // Get all defined servers.
            ServerCollection servers = new ServerCollection(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            ));

            int count = 0;
            // Run through all defined servers.
            foreach (string ip in servers.Items.Keys)
            {
                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}:8080/Handler.ashx",
                    ip
                ));

                XmlDocument document = new XmlDocument();

                try
                {
                    document.LoadXml(service.Request(new string[]
                    {
                        "Method=GetLog"
                    }));
                }
                catch
                {
                    continue;
                }

                pnlServers.Controls.Add(new LiteralControl(string.Format(
                    "<div class=\"ConsoleTab {2}\" onclick=\"ShowConsoleTab(this, '{1}')\">{0}</div>",
                    ip,
                    ip.Replace(".", "_"),
                    count == 0 ? "ConsoleTab_Active" : ""
                )));

                StringBuilder htmlString = new StringBuilder();
                htmlString.Append(string.Format(
                    "<div id=\"pnlConsole{0}\" style=\"{1}\" class=\"LogEntries\">",
                    ip.Replace(".", "_"),
                    count != 0 ? "display:none;" : ""
                ));

                foreach (XmlNode xmlNode in document.DocumentElement.ChildNodes)
                {
                    htmlString.Append(string.Format(
                        "<div class=\"LogEntry LogType_{2}\">[{1}] {3}</div>",
                        ip,
                        xmlNode.Attributes["Timestamp"].Value,
                        xmlNode.Attributes["Type"].Value,
                        xmlNode.InnerText
                    ));
                }

                htmlString.Append("</div>");

                pnlConsole.Controls.Add(new LiteralControl(htmlString.ToString()));

                count++;
            }
        }

        #endregion
    }
}