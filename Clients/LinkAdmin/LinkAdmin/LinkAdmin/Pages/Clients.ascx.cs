using ApplicationUtilities.Cluster;
using LinkAdmin.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkAdmin.Pages
{
    public partial class Clients : Classes.Page
    {
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

            foreach (string instanceName in instances.Instances.Keys)
            {
                result.Append("<div class=\"Instance\"><h1>");
                result.Append(instanceName + " - " + instances.Instances[instanceName].Version);
                result.Append("</h1><table class=\"TableContent\" cellspacing=\"0\" cellpadding=\"0\">");

                result.Append(string.Format(
                    "<tr class=\"TableHeadline\"><td>{0}</td><td style=\"width:50%;\">{1}</td>"+
                    "<td></td><td style=\"width:100px;\">{2}</td><td style=\"width:100px;\">{4}</td><td style=\"width:100px;\">{3}</td></tr>",
                    Global.LanguageManager.GetText("PortalName"),
                    Global.LanguageManager.GetText("HostName"),
                    Global.LanguageManager.GetText("DataSize"),
                    Global.LanguageManager.GetText("InstanceName"),
                    Global.LanguageManager.GetText("Servers")
                ));

                foreach (string portalName in instances.Instances[instanceName].Portals.Keys)
                {
                    List<string> servers = new List<string>();

                    foreach (Server server in instances.Instances[instanceName].Portals[portalName].Servers)
                    {
                        if(server.State == ServerState.Offline)
                        {
                            servers.Add(string.Format(
                                "<span style=\"color:#CC0000;\">{0}</span>",
                                server.Description
                            ));
                        }
                        else
                        {
                            servers.Add(string.Format(
                                "{0}",
                                server.Description
                            ));
                        }
                    }

                    result.Append(string.Format(
                        "<tr><td>{0}</td><td style=\"width:50%;\">{1}</td>"+
                        "<td class=\"TdHoverOnly\" style=\"width:110px;\" onclick=\"GetUsers('{5}', '{0}', '{7}');\"><div class=\"Button\">{4}</div></td>" +
                        "<td style=\"width:100px;\">{2}</td>" +
                        "<td style=\"width:100px;\">{6}</td><td style=\"width:100px;\">" +
                        "<select onchange=\"MovePortal('{5}', this.value)\">{3}</select></td></tr>",
                        portalName,
                        instances.Instances[instanceName].Portals[portalName].Host,
                        Math.Round((test.Next(1024, 100024) / 1024.0), 2) + " GB",
                        instanceSelection.Replace("<option>" + instanceName + "</option>", "<option SELECTED=\"selected\">" + instanceName + "</option>"),
                        "show users",
                        instanceName,
                        string.Join(",", servers),
                        instances.Instances[instanceName].Portals[portalName].Servers[0].IP
                    ));
                }

                result.Append(string.Format(
                    "<tr><td class=\"TableCellNew\" colspan=\"6\" onclick=\"CreatePortal('{0}', '{1}')\">+</td></tr>",
                    instanceName,
                    instanceSelection
                ));

                result.Append("</table></div>");
            }

            this.Controls.Add(new LiteralControl(result.ToString()));
        }
    }
}