using ApplicationUtilities.Cluster;
using LinkAdmin.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkAdmin.Pages
{
    public partial class Instances : Classes.Page
    {
        #region Properties

        #endregion


        #region Constructor

        #endregion


        #region Methods

        public override void Render()
        {
            string instanceSelection = "";
            InstanceCollection instances = new InstanceCollection();

            foreach (string instance in instances.Instances.Keys)
            {
                instanceSelection += "<option>" +
                    (instance) +
                    "</option>";
            }

            Random test = new Random();

            StringBuilder result = new StringBuilder();
            result.Append("<table class=\"TableContent\" cellspacing=\"0\" cellpadding=\"0\">");

            result.Append(string.Format(
                "<tr class=\"TableHeadline\"><td>{0}</td>" +
                "<td></td>"+
                "<td style=\"width:200px;\">{1}</td>" +
                "<td style=\"width:200px;\">{3}</td>" +
                "<td style=\"width:50%;\">{2}</td></tr>",
                Global.LanguageManager.GetText("InstanceName"),
                Global.LanguageManager.GetText("InstanceVersion"),
                Global.LanguageManager.GetText("InstanceServers"),
                Global.LanguageManager.GetText("InstanceClients")
            ));

            foreach (string instanceName in instances.Instances.Keys)
            {
                List<string> servers = new List<string>();

                foreach (string server in instances.Instances[instanceName].Servers)
                {
                    if (instances.Servers.Items[server].State == ServerState.Offline)
                    {
                        servers.Add(string.Format(
                            "<span style=\"color:#CC0000;\">{0}</span>",
                            instances.Servers.Items[server].Description
                        ));
                    }
                    else
                    {
                        servers.Add(string.Format(
                            "{0}",
                            instances.Servers.Items[server].Description
                        ));
                    }
                }

                result.Append(string.Format(
                    "<tr><td>{0}</td>" +
                    "<td class=\"TdHoverOnly\" style=\"width:110px;\" onclick=\"DeployUpdate('{0}');\"><div class=\"Button\">{3}</div></td>" +
                    "<td style=\"width:200px;\">{1}</td>" +
                    "<td style=\"width:200px;\">{4}</td>" +
                    "<td style=\"width:50%;\">{2}</td></tr>",
                    instanceName,
                    instances.Instances[instanceName].Version,
                    string.Join(",", servers),
                    Global.LanguageManager.GetText("InstanceDeployUpdate"),
                    instances.Instances[instanceName].Portals.Count
                ));
            }

            result.Append(string.Format(
                "<tr><td class=\"TableCellNew\" colspan=\"5\" onclick=\"\">+</td></tr>"
            ));

            result.Append("</table>");

            this.Controls.Add(new LiteralControl(result.ToString()));
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion
    }
}