using ApplicationUtilities.Cluster;
using LinkAdmin.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkAdmin.Pages
{
    public partial class Servers : Classes.Page
    {
        #region Properties

        #endregion


        #region Constructor

        public Servers()
        {

        }

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

            // Create a new string builder that
            // holds the result html string.
            StringBuilder result = new StringBuilder();

            result.Append("<table class=\"TableContent\" cellspacing=\"0\" cellpadding=\"0\">");

            result.Append("<tr class=\"TableHeadline\">");

            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("ServerDescription")
            ));

            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("ServerIP")
            ));

            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("ServerRole")
            ));

            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("ServerCountries")
            ));

            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("ServerSessions")
            ));

            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("ServerConnectionQuality")
            ));

            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("ServerSynchEnabled")
            ));

            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("ServerState")
            ));

            result.Append("</tr>");

            Random test = new Random();

            // Run through all defined servers.
            foreach (string ip in servers.Items.Keys)
            {
                Server server = servers.Items[ip];

                result.Append("<tr>");

                result.Append(string.Format(
                    "<td>{0}</td>",
                    server.Description
                ));

                result.Append(string.Format(
                    "<td>{0}</td>",
                    server.IP
                ));

                result.Append(string.Format(
                    "<td>{0}</td>",
                    Global.LanguageManager.GetText("ServerRole" + server.Role)
                ));

                result.Append(string.Format(
                    "<td>{0}</td>",
                    string.Join(",", server.Countries)
                ));

                result.Append(string.Format(
                    "<td>{0}</td>",
                    server.State == ServerState.Online ? test.Next(0, 20) : 0
                ));

                if (server.State == ServerState.Online)
                {
                    ConnectionQuality connectionQuality = server.GetConnectionQuality();
                    result.Append(string.Format(
                        "<td>{0}</td>",
                        server.State == ServerState.Online ?
                        string.Format(Global.LanguageManager.GetText("ServerConnectionQuality" +
                        connectionQuality.Rank), connectionQuality.Ping) : " - "
                    ));
                }
                else
                {
                    result.Append("<td>-</td>");
                }

                result.Append(string.Format(
                    "<td onclick=\"GetLatestSynchAction('{1}')\" style=\"cursor:pointer;\">{0}</td>",
                    Global.LanguageManager.GetText("ServerSynchEnabled" + server.SynchEnabled()),
                    server.IP
                ));

                result.Append(string.Format(
                    "<td onclick=\"{1}(this, '{2}');\" style=\"cursor:pointer;\">{0}</td>",
                    Global.LanguageManager.GetText("ServerState" + server.State),
                    server.State == ServerState.Online ? "TakeOffline" : "BringOnline",
                    server.IP
                ));

                result.Append("</tr>");
            }

            result.Append("</table>");

            this.Controls.Add(new LiteralControl(result.ToString()));
        }

        #endregion
    }
}