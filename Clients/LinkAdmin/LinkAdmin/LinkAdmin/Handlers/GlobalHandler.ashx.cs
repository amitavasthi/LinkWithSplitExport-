using ApplicationUtilities;
using ApplicationUtilities.Classes;
using ApplicationUtilities.Cluster;
using LinkAdmin.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Xml;

namespace LinkAdmin.Handlers
{
    /// <summary>
    /// Summary description for GlobalHandler
    /// </summary>
    public class GlobalHandler : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public GlobalHandler()
        {
            base.OnSessionError += GlobalHandler_OnSessionError;

            base.Methods.Add("GetUsers", GetUsers);
            base.Methods.Add("GetDefaultUsers", GetDefaultUsers);
            base.Methods.Add("CreatePortalGetServers", CreatePortalGetServers);
            base.Methods.Add("GetServers", GetServers);
            base.Methods.Add("CreateClient", CreateClient);
            base.Methods.Add("DeployUpdate", DeployUpdate);
            base.Methods.Add("GetDeploymentDetails", GetDeploymentDetails);
            base.Methods.Add("GetCurrentDeploymentStep", GetCurrentDeploymentStep);

            base.Methods.Add("ServerBringOnline", ServerBringOnline);
            base.Methods.Add("ServerTakeOffline", ServerTakeOffline);
            base.Methods.Add("GetLatestSynchAction", GetLatestSynchAction);

            base.Methods.Add("CreateSingleSignOn", CreateSingleSignOn);
            base.Methods.Add("DeleteSingleSignOn", DeleteSingleSignOn);

            base.Methods.Add("Test", Test);
        }

        #endregion


        #region Web Methods

        private void GetUsers(HttpContext context)
        {
            // Get the instance name from
            // the http request's parameters.
            string instanceName = context.Request.Params["Instance"];

            // Get the client name from
            // the http request's parameters.
            string clientName = context.Request.Params["Client"];

            string server = context.Request.Params["Server"];

            ServiceLink service = new ServiceLink(string.Format(
                "http://{0}:8080/Handler.ashx",
                server
            ));

            string xmlString = service.Request(new string[]
            {
                "Method=GetUsers",
                "Client=" + clientName,
                "Instance=" + instanceName
            });

            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);

            StringBuilder result = new StringBuilder();
            result.Append("<table class=\"TableContent\">");
            result.Append("<tr class=\"TableHeadline\">");
            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("Name")
            ));
            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("FirstName")
            ));
            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("LastName")
            ));
            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("Mail")
            ));
            result.Append("</tr>");

            foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("User"))
            {
                result.Append("<tr>");
                result.Append(string.Format(
                    "<td>{0}</td>",
                    xmlNode.Attributes["Name"].Value
                ));
                result.Append(string.Format(
                    "<td>{0}</td>",
                    xmlNode.Attributes["FirstName"].Value
                ));
                result.Append(string.Format(
                    "<td>{0}</td>",
                    xmlNode.Attributes["LastName"].Value
                ));
                result.Append(string.Format(
                    "<td>{0}</td>",
                    xmlNode.Attributes["Mail"].Value
                ));
                result.Append("</tr>");
            }

            result.Append("</table>");

            context.Response.Write(result.ToString());
        }

        private void GetDefaultUsers(HttpContext context)
        {
            XmlDocument document = new XmlDocument();
            document.Load(Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "DefaultUsers.xml"
            ));

            StringBuilder result = new StringBuilder();
            result.Append("<table class=\"TableContent\" cellspacing=\"0\" cellpadding=\"0\">");

            foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("User"))
            {
                result.Append("<tr>");

                result.Append(string.Format(
                    "<td><input type=\"checkbox\" name=\"chkCreateClientDefaultUser\" IdUser=\"{0}\" checked=\"CHECKED\" /></td>",
                    xmlNode.Attributes["Id"].Value
                ));
                result.Append(string.Format("<td>{0}</td>", xmlNode.Attributes["Name"].Value));
                result.Append(string.Format("<td>{0}</td>", xmlNode.Attributes["FirstName"].Value));
                result.Append(string.Format("<td>{0}</td>", xmlNode.Attributes["LastName"].Value));
                result.Append(string.Format("<td>{0}</td>", xmlNode.Attributes["Mail"].Value));

                result.Append("</tr>");
            }

            result.Append("</table>");

            context.Response.Write(result.ToString());
        }

        private void CreatePortalGetServers(HttpContext context)
        {
            // Get the name of the instance from
            // the http request's parameters.
            string instance = context.Request.Params["Instance"];

            // Create a new string builder that
            // holds the result html string.
            StringBuilder result = new StringBuilder();

            result.Append("<table class=\"TableContent\" cellspacing=\"0\" cellpadding=\"0\">");

            ServerCollection servers = new ServerCollection(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            ));
            InstanceCollection instances = new InstanceCollection(servers);

            if (instances.Instances.ContainsKey(instance))
            {
                // Run through all defined servers.
                foreach (string ip in instances.Instances[instance].Servers)
                {
                    if (!servers.Items.ContainsKey(ip))
                        continue;

                    Server server = servers.Items[ip];

                    if (server.State == ServerState.Offline)
                        continue;

                    result.Append("<tr>");

                    result.Append(string.Format(
                        "<td><input type=\"checkbox\" name=\"chkCreateClientServer\" Server=\"{0}\" checked=\"CHECKED\" /></td>",
                        server.IP
                    ));

                    result.Append(string.Format(
                        "<td>{0}</td>",
                        server.Description
                    ));

                    result.Append(string.Format(
                        "<td>{0}</td>",
                        Global.LanguageManager.GetText("ServerRole" + server.Role)
                    ));

                    result.Append("</tr>");
                }
            }

            result.Append("</table>");

            context.Response.Write(result.ToString());
        }

        private void GetServers(HttpContext context)
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

            result.Append("[");

            // Run through all defined servers.
            foreach (string ip in servers.Items.Keys)
            {
                Server server = servers.Items[ip];

                result.Append("{");

                result.Append(string.Format(
                    "\"IP\": \"{0}\", ",
                    server.IP
                ));

                result.Append(string.Format(
                    "\"Description\": \"{0}\", ",
                    server.Description
                ));

                result.Append(string.Format(
                    "\"Role\": \"{0}\", ",
                    server.Role
                ));

                result.Append(string.Format(
                    "\"State\": \"{0}\"",
                    server.State
                ));

                result.Append("},");
            }

            if (servers.Items.Count != 0)
                result = result.Remove(result.Length - 1, 1);

            result.Append("]");

            context.Response.Write(result.ToString());
        }

        private void CreateClient(HttpContext context)
        {
            // Get the client name from
            // the http request's parameters.
            string clientName = context.Request.Params["Client"];

            // Get the source instance name from
            // the http request's parameters.
            string instanceName = context.Request.Params["Instance"];

            Guid[] defaultUsers = context.Request.Params["DefaultUsers"].Split(',').
                Select(x => Guid.Parse(x)).ToArray();

            List<string> _servers = context.Request.Params["Servers"].Split(
                new string[] { "," },
                StringSplitOptions.RemoveEmptyEntries
            ).ToList();

            // Create on one server only, synch will do the rest.
            //string server = servers[0];

            foreach (string server in _servers)
            {
                List<string> servers = new List<string>();
                servers.AddRange(_servers.ToArray());

                for (int i = 0; i < servers.Count; i++)
                {
                    if (servers[i] == server)
                    {
                        servers.RemoveAt(i);
                        break;
                    }
                }

                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}:8080/Handler.ashx",
                    server
                ));

                string result = service.Request(new string[]
                {
                    "Method=CreateClient",
                    "Instance=" + instanceName,
                    "Client=" + clientName,
                    "Servers=" + string.Join(",", servers)
                });

                if (result.StartsWith("__ERROR__"))
                {
                    context.Response.Write(result);

                    return;
                }

                XmlDocument document = new XmlDocument();
                document.Load(Path.Combine(
                    context.Request.PhysicalApplicationPath,
                    "App_Data",
                    "DefaultUsers.xml"
                ));

                foreach (Guid idDefaultUser in defaultUsers)
                {
                    XmlNode xmlNodeDefaultUser = document.DocumentElement.SelectSingleNode(string.Format(
                        "User[@Id=\"{0}\"]",
                        idDefaultUser
                    ));

                    if (xmlNodeDefaultUser == null)
                        continue;

                    service.Request(new string[]
                    {
                        "Method=CreateUser",
                        "Instance=" + instanceName,
                        "Client=" + clientName,
                        "Id=" + idDefaultUser,
                        "Name=" + xmlNodeDefaultUser.Attributes["Name"].Value,
                        "FirstName=" + xmlNodeDefaultUser.Attributes["FirstName"].Value,
                        "LastName=" + xmlNodeDefaultUser.Attributes["LastName"].Value,
                        "Mail=" + xmlNodeDefaultUser.Attributes["Mail"].Value,
                        "Password=" + xmlNodeDefaultUser.Attributes["Password"].Value,
                        "IdRole=" + "00000000-0000-0000-0000-000000000000"
                    });
                }
            }
        }

        private void DeployUpdate(HttpContext context)
        {
            // Get the name of the instance where to deploy to.
            string instance = context.Request.Params["Instance"];

            // Get the software version of the deployment.
            string version = context.Request.Params["Version"];

            InstanceCollection instances = new InstanceCollection();

            if (!instances.Instances.ContainsKey(instance))
                return;

            HttpContext.Current.Session["DeploymentStep"] = 0;

            context.Response.Write(GetDeploymentSteps(
                instances.Instances[instance],
                version
            ));

            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(DeployUpdateAsynch);

            Thread thread = new Thread(threadStart);

            thread.Start(new object[]
            {
                instances.Instances[instance],
                HttpContext.Current.Session,
                version,
                context.Request.PhysicalApplicationPath
            });
        }

        private void GetDeploymentDetails(HttpContext context)
        {
            // Get the name of the instance where to deploy to.
            string instance = context.Request.Params["Instance"];

            InstanceCollection instances = new InstanceCollection();

            if (!instances.Instances.ContainsKey(instance))
                return;

            InstanceVersion instanceVerion = new InstanceVersion(
                instances.Instances[instance].Version
            );

            // Create a new string builder that
            // contains the result JSON string.
            StringBuilder result = new StringBuilder();
            result.Append("{");

            result.Append(string.Format(
                "\"FromVersion\": \"{0}\",",
                instances.Instances[instance].Version
            ));

            result.Append(string.Format(
                "\"Errors\": [",
                instances.Instances[instance].Version
            ));

            bool hasErrors = false;
            foreach (string server in instances.Instances[instance].Servers)
            {
                if (instances.Servers.Items.ContainsKey(server) == false ||
                    instances.Servers.Items[server].State == ServerState.Offline)
                {
                    hasErrors = true;

                    result.Append(string.Format("\"{0}\",", string.Format(Global.LanguageManager.GetText(
                        "SoftwareUpdateError_ServerOffline"),
                        instances.Servers.Items.ContainsKey(server) ?
                        instances.Servers.Items[server].Description :
                        server
                    )));
                }
            }

            if (hasErrors)
            {
                result = result.Remove(result.Length - 1, 1);

                result.Append("]}");

                context.Response.Write(result.ToString());

                return;
            }

            result.Append("],");

            result.Append("\"AvailableUpdates\": [");

            ServiceLink service = new ServiceLink(string.Format(
                "http://{0}:8080/Handler.ashx",
                instances.Instances[instance].Servers[0]
            ));

            XmlDocument document = new XmlDocument();

            document.LoadXml(service.Request(new string[]{
                "Method=GetAvailableUpdates"
            }));

            foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("Update"))
            {
                InstanceVersion version = new InstanceVersion(
                    xmlNode.Attributes["Version"].Value
                );

                if (version.ToInt() <= instanceVerion.ToInt())
                    continue;

                result.Append(string.Format(
                    "\"{0}\",",
                    (version).ToString()
                ));
            }

            if (result.ToString().EndsWith(","))
                result = result.Remove(result.Length - 1, 1);

            result.Append("]");

            result.Append("}");

            context.Response.Write(result.ToString());
        }

        private void GetCurrentDeploymentStep(HttpContext context)
        {
            if (HttpContext.Current.Session["DeploymentStep"] == null)
            {
                context.Response.Write("-1");
                return;
            }

            context.Response.Write(HttpContext.Current.Session["DeploymentStep"]);
        }


        private void ServerBringOnline(HttpContext context)
        {
            // Get the IP of the server to bring online
            // from the http request's parameters.
            string IP = context.Request.Params["IP"];

            // Get all defined servers.
            ServerCollection servers = new ServerCollection(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            ));

            if (!servers.Items.ContainsKey(IP))
                return;

            servers.Items[IP].State = ServerState.Online;

            servers.Save();
        }

        private void ServerTakeOffline(HttpContext context)
        {
            // Get the IP of the server to take offline
            // from the http request's parameters.
            string IP = context.Request.Params["IP"];

            // Get all defined servers.
            ServerCollection servers = new ServerCollection(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            ));

            if (!servers.Items.ContainsKey(IP))
                return;

            servers.Items[IP].State = ServerState.Offline;

            servers.Save();
        }

        private void GetLatestSynchAction(HttpContext context)
        {
            ServiceLink service = new ServiceLink(string.Format(
                "http://{0}:8080/Handler.ashx",
                context.Request.Params["Server"]
            ));

            string xmlString = service.Request(new string[] {
                "Method=GetLatestSynchActions"
            });

            if (string.IsNullOrEmpty(xmlString))
            {
                context.Response.Write("N/A");
                return;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString);

            StringBuilder result = new StringBuilder();
            result.Append("<table class=\"TableContent\">");
            result.Append("<tr class=\"TableHeadline\">");
            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("Client")
            ));
            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("LatestSynchActionFilesystem")
            ));
            result.Append(string.Format(
                "<td>{0}</td>",
                Global.LanguageManager.GetText("LatestSynchActionDatabase")
            ));
            result.Append("</tr>");

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                string filesystem = GetLatestSynchActionStr(xmlNode.Attributes["Filesystem"].Value);
                string database = GetLatestSynchActionStr(xmlNode.Attributes["Database"].Value);

                if (filesystem == null && database == null)
                    continue;

                result.Append("<tr>");
                result.Append(string.Format(
                    "<td>{0}</td>",
                    xmlNode.Attributes["Client"].Value
                ));
                result.Append(string.Format(
                    "<td>{0}</td>",
                    filesystem != null ? filesystem : "-"
                ));
                result.Append(string.Format(
                    "<td>{0}</td>",
                    database != null ? database : "-"
                ));
                result.Append("</tr>");
            }

            result.Append("</table>");

            context.Response.Write(result.ToString());
        }


        private void CreateSingleSignOn(HttpContext context)
        {
            string host = context.Request.Params["Host"];
            string username = context.Request.Params["Username"];
            string password = context.Request.Params["Password"];

            StoredPortalCredentials credentials = new StoredPortalCredentials();
            
            if(credentials.Credentials.ContainsKey(host))
            {
                credentials.Credentials[host].Username = username;
                credentials.Credentials[host].Password = GetMD5Hash(password);
            }
            else
            {
                credentials.Credentials.Add(host, new PortalCredentials(
                    credentials, 
                    host, 
                    username,
                    GetMD5Hash(password)
                ));
            }

            credentials.Save();
        }

        private void DeleteSingleSignOn(HttpContext context)
        {
            string host = context.Request.Params["Host"];

            StoredPortalCredentials credentials = new StoredPortalCredentials();

            if (credentials.Credentials.ContainsKey(host))
                credentials.Credentials.Remove(host);

            credentials.Save();
        }


        private void Test(HttpContext context)
        {
            context.Response.Write(DateTime.UtcNow.ToString(new CultureInfo("en-GB")));
        }

        #endregion


        #region Methods

        /// <summary>
        /// Returns a MD5 hash as string.
        /// </summary>
        /// <param name="input">String to encrypt.</param>
        /// <returns>Hash as string.</returns>
        public string GetMD5Hash(string input)
        {
            // Check if there is an string to encrypt.
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // The MD5 encryption provider.
            MD5 md5 = new MD5CryptoServiceProvider();

            // Convert string to byte array.
            byte[] textToHash = Encoding.Default.GetBytes(input);

            // Calculate MD5 Hash. 
            byte[] result = md5.ComputeHash(textToHash);

            // Convert byte array to string.
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }

            return sb.ToString();
        }

        private void DeployUpdateAsynch(object _parameters)
        {
            object[] paramters = (object[])_parameters;

            Instance instance = (Instance)paramters[0];
            HttpSessionState session = (HttpSessionState)paramters[1];
            InstanceVersion version = new InstanceVersion((string)paramters[2]);
            string physicalApplicationPath = (string)paramters[3];

            StringBuilder result = new StringBuilder();
            result.Append("<Servers>");

            int step = 0;
            foreach (string server in instance.Servers)
            {
                session["DeploymentStep"] = step++;

                // Take the server offline for the deployment.
                instance.Owner.Servers.Items[server].State = ServerState.Offline;
                instance.Owner.Servers.Save();

                session["DeploymentStep"] = step++;

                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}:8080/Handler.ashx",
                    server
                ));

                string xmlString = service.Request(new string[]
                {
                    "Method=DeployUpdate",
                    "Instance=" + instance.Name,
                    "Version=" + version.ToString()
                });

                result.Append(string.Format(
                    "<Server IP=\"{0}\">{1}</Server>",
                    server,
                    xmlString
                ));

                session["DeploymentStep"] = step++;

                // Bring the server online.
                instance.Owner.Servers.Items[server].State = ServerState.Online;
                instance.Owner.Servers.Save();
            }

            result.Append("</Servers>");

            string fileName = Path.Combine(
                physicalApplicationPath,
                "Logs",
                "Deployment_" + DateTime.Now.ToString("yyyyMMddHHmmss")
            );

            File.WriteAllText(
                fileName,
                result.ToString()
            );

            session["DeploymentResult"] = fileName;
            session["DeploymentStep"] = null;
        }


        private string GetDeploymentSteps(Instance instance, string version)
        {
            // Create a new string builder that
            // contains the result html string.
            StringBuilder result = new StringBuilder();

            InstanceVersion toVersion = new InstanceVersion(version);

            int i = 0;
            foreach (string server in instance.Servers)
            {
                result.Append(string.Format(
                    Global.LanguageManager.GetText("DeploymentStep1"),
                    i++,
                    instance.Owner.Servers.Items[server].Description,
                    instance.Name
                ));

                result.Append(string.Format(
                    Global.LanguageManager.GetText("DeploymentStep2"),
                    i++,
                    instance.Owner.Servers.Items[server].Description,
                    instance.Name,
                    toVersion
                ));

                result.Append(string.Format(
                    Global.LanguageManager.GetText("DeploymentStep3"),
                    i++,
                    instance.Owner.Servers.Items[server].Description,
                    instance.Name
                ));
            }

            return result.ToString();
        }

        private string GetLatestSynchActionStr(string str)
        {
            DateTime value;

            if (DateTime.TryParse(str, new CultureInfo("en-GB"), DateTimeStyles.None, out value))
            {
                TimeSpan timespan = DateTime.UtcNow - value;

                string valueStr = "";

                if ((int)timespan.TotalHours != 0)
                {
                    valueStr += string.Format(
                        Global.LanguageManager.GetText("SynchTimestampHours"),
                        (int)timespan.TotalHours
                    ) + " ";
                }
                if (timespan.Minutes != 0)
                {
                    valueStr += string.Format(
                        Global.LanguageManager.GetText("SynchTimestampMinutes"),
                        timespan.Minutes
                    ) + " ";
                }
                if (timespan.Seconds != 0)
                {
                    valueStr += string.Format(
                        Global.LanguageManager.GetText("SynchTimestampSeconds"),
                        timespan.Seconds
                    ) + " ";
                }

                if (valueStr == "")
                {
                    valueStr = Global.LanguageManager.GetText("SynchTimestampMilliseconds");
                }
                else
                {
                    valueStr = string.Format(
                        Global.LanguageManager.GetText("SynchTimestamp"),
                        valueStr
                    );
                }

                return valueStr;
            }

            return null;
        }

        #endregion


        #region Event Handlers

        private void GlobalHandler_OnSessionError(object sender, EventArgs e)
        {
            HttpContext context = (HttpContext)sender;

            context.Response.Write("ERROR_SESSION");
        }

        #endregion
    }
}