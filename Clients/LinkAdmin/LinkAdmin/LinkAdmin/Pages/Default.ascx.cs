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
    public partial class Default : Classes.Page
    {
        #region Properties

        #endregion


        #region Constructor

        #endregion


        #region Methods

        public override void Render()
        {
            StringBuilder result = new StringBuilder();

            InstanceCollection instances = new InstanceCollection();
            StoredPortalCredentials credentials = new StoredPortalCredentials();

            foreach (string instance in instances.Instances.Keys)
            {
                result.Append("<div class=\"Instance\">");

                result.Append(string.Format(
                    "<h1>{0}</h1>",
                    instance
                ));

                foreach (string portal in instances.Instances[instance].Portals.Keys)
                {
                    string host = instances.Instances[instance].Portals[portal].Host;

                    if (credentials.Credentials.ContainsKey(host))
                    {
                        result.Append(string.Format(
                            "<div class=\"Portal\" onclick=\"if(event.target.nodeName == 'IMG')return;SingleSignOn('{0}', '{1}', '{2}')\"><table>",
                            host,
                            credentials.Credentials[host].Username,
                            credentials.Credentials[host].Password
                        ));
                    }
                    else
                    {
                        result.Append(string.Format(
                            "<div class=\"Portal\" onclick=\"CreateSingleSignOn('{0}')\"><table>",
                            host
                        ));
                    }

                    result.Append(string.Format(
                        "<tr><td colspan=\"2\"><b>{0}</b></td>",
                        host
                    ));

                    if (credentials.Credentials.ContainsKey(host))
                    {
                        result.Append(string.Format(
                            "<tr><td><b>{0}</b></td><td>{1}</td><td><img src=\"/Images/Icons/Delete.png\" onclick=\"DeleteSingleSignOn('{2}')\" /></td></tr>",
                            Global.LanguageManager.GetText("Username"),
                            credentials.Credentials[host].Username,
                            host
                        ));
                    }
                    else
                    {
                        result.Append(string.Format(
                            "<tr><td><td>{0}</td></tr>",
                            Global.LanguageManager.GetText("PortalCredentialsNotExist")
                        ));
                    }

                    result.Append("</table></div>");
                }

                result.Append("</div>");
            }

            this.Controls.Add(new LiteralControl(result.ToString()));
        }

        #endregion


        #region Event Handlers

        #endregion
    }
}