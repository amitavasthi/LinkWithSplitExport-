using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using WebUtilities.Controls;

namespace WebUtilities.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für WebUtilities
    /// </summary>
    public class WebUtilitiesHandler : BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public WebUtilitiesHandler()
        {
            base.Methods.Add("TreeViewExpandNode", TreeViewExpandNode);
            base.Methods.Add("TreeViewCollapseNode", TreeViewCollapseNode);

            base.Methods.Add("SendChatMessage", SendChatMessage);
            base.Methods.Add("GetLatestChatMessages", GetLatestChatMessages);
            base.Methods.Add("GetChatMessages", GetChatMessages);

            base.Methods.Add("RenderControl", RenderControl);
        }

        #endregion


        #region Methods

        public void RenderControl(HttpContext context)
        {
            // Get the assembly name of the control to render
            // from the http request's parameters.
            string assembly = context.Request.Params["Assembly"];

            // Get the name of the control to render
            // from the http request's parameters.
            string name = context.Request.Params["Name"];

            List<object> arguments = new List<object>();

            int i = 0;
            while (true)
            {
                if (context.Request.Params["Argument" + i] == null)
                    break;

                string type = context.Request.Params["Argument" + i].Split('|')[0];
                string valueStr = context.Request.Params["Argument" + i].Split('|')[1];

                object value;

                switch (type)
                {
                    case "Guid":
                        value = Guid.Parse(valueStr);
                        break;
                    case "Int32":
                        value = int.Parse(valueStr);
                        break;
                    case "Int64":
                        value = long.Parse(valueStr);
                        break;
                    case "double":
                        value = double.Parse(valueStr);
                        break;
                    case "bool":
                        value = bool.Parse(valueStr);
                        break;
                    default:
                        value = valueStr;
                        break;
                }

                arguments.Add(value);

                i++;
            }

            Type t = (Assembly.Load(assembly)).GetType(name);

            BaseControl control = (BaseControl)((object)Activator.CreateInstance(
                t,
                arguments.ToArray()
            ));

            MethodInfo mInfo = t.GetMethod("Render");

            if (mInfo != null)
                mInfo.Invoke(control, new object[0]);

            context.Response.Write(control.ToHtml());
        }

        private string RenderChatMessageJson(Guid idUser, XmlNode xmlNode, DateTime sent)
        {
            StringBuilder result = new StringBuilder();

            result.Append("{");

            object[] user = base.Core.Users.GetValues(
                new string[] { "FirstName", "LastName" },
                new string[] { "Id" },
                new object[] { idUser }
            )[0];

            result.Append(string.Format(
                "\"IdUser\": \"{0}\", \"UserName\": \"{1}\", \"Sent\": \"{2}\", \"Message\": \"{3}\"",
                Guid.Parse(xmlNode.Attributes["IdUser"].Value),
                ((string)user[0] + " " + (string)user[1]).ToLower(),
                sent.ToFormattedString(),
                xmlNode.InnerText
            ));

            result.Append("}");

            return result.ToString();
        }

        #endregion


        #region TreeView methods

        public void TreeViewExpandNode(HttpContext context)
        {
            // Get the path to the tree view node
            // from the http request's parameters.
            string path = context.Request.Params["Path"];

            // Get the id of the tree view
            // from the http request's paramters.
            string idTreeView = context.Request.Params["IdTreeView"];

            if (HttpContext.Current.Session["TreeView" + idTreeView] == null)
                return;

            TreeView treeView = (TreeView)HttpContext.Current.Session["TreeView" + idTreeView];

            TreeViewNode node = treeView.Select(path);

            if (node == null)
                return;

            node.Expanded = true;

            Panel pnlChildNodes = node.RenderChildNodes(true);

            context.Response.Write(pnlChildNodes.ToHtml());
        }

        public void TreeViewCollapseNode(HttpContext context)
        {
            // Get the path to the tree view node
            // from the http request's parameters.
            string path = context.Request.Params["Path"];

            // Get the id of the tree view
            // from the http request's paramters.
            string idTreeView = context.Request.Params["IdTreeView"];

            if (HttpContext.Current.Session["TreeView" + idTreeView] == null)
                return;

            TreeView treeView = (TreeView)HttpContext.Current.Session["TreeView" + idTreeView];

            TreeViewNode node = treeView.Select(path);

            if (node == null)
                return;

            node.Expanded = false;
        }

        #endregion


        #region Chat methods

        public void SendChatMessage(HttpContext context)
        {
            // Get the id of the chat from the http request's parameters.
            string idChat = context.Request.Params["IdChat"];

            // Get the text message to add from the http request's parameters.
            string message = context.Request.Params["Message"];

            // Build the full path to the chat's log file.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Chat",
                base.Core.ClientName,
                idChat + ".xml"
            );

            FileInfo fInfo = new FileInfo(fileName);

            if (!Directory.Exists(fInfo.DirectoryName))
                Directory.CreateDirectory(fInfo.DirectoryName);

            // Check if the chat log file exists.
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, string.Format(
                    "<ChatLog></ChatLog>"
                ));
            }

            // Create a new xml document that contains the chat log.
            XmlDocument document = new XmlDocument();

            // Load the contents of the chat log file into the xml document.
            document.Load(fileName);

            // Add a new chat log item.
            document.DocumentElement.InnerXml += string.Format(
                "<Message IdUser=\"{0}\" Sent=\"{1}\"><![CDATA[{2}]]></Message>",
                base.IdUser.Value,
                DateTime.Now.ToString(),
                message
            );

            document.Save(fileName);
        }

        public void GetLatestChatMessages(HttpContext context)
        {
            // Parse the latest update time from the http request's parameters.
            DateTime latestUpdate = DateTime.Parse(
                context.Request.Params["LatestUpdate"]
            );

            int limit = 10;

            if (context.Request.Params["Limit"] != null)
                limit = int.Parse(context.Request.Params["Limit"]);

            // Get the id of the chat from the http request's parameters.
            string idChat = context.Request.Params["IdChat"];

            // Build the full path to the chat's log file.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Chat",
                base.Core.ClientName,
                idChat + ".xml"
            );

            // Check if the chat exists.
            if (!File.Exists(fileName))
                return;

            // Create a new xml document that contains the chat log.
            XmlDocument document = new XmlDocument();

            // Load the contents of the chat log file into the xml document.
            document.Load(fileName);

            int i = document.DocumentElement.ChildNodes.Count - 1;
            // Create a new dictionary that contains the
            // result json string seperated by the sent date.
            Dictionary<DateTime, string> result = new Dictionary<DateTime, string>();

            if (document.DocumentElement.ChildNodes.Count == 0)
                return;

            int c = 0;
            while (true)
            {
                if (c++ == limit)
                    break;

                if (i == -1)
                    break;

                XmlNode xmlNode = document.DocumentElement.ChildNodes[i];

                DateTime sent = DateTime.Parse(xmlNode.Attributes["Sent"].Value);

                if (sent <= latestUpdate)
                    break;

                result.Add(sent, RenderChatMessageJson(
                    Guid.Parse(xmlNode.Attributes["IdUser"].Value),
                    xmlNode,
                    sent
                ));

                i--;
            }

            context.Response.Write("[" + string.Join(
                ",",
                result.OrderBy(x => x.Key).Select(x => x.Value)
            ) + "]");
        }

        public void GetChatMessages(HttpContext context)
        {
            int limit = int.Parse(context.Request.Params["Limit"]);

            // Get the id of the chat from the http request's parameters.
            string idChat = context.Request.Params["IdChat"];

            // Build the full path to the chat's log file.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Chat",
                base.Core.ClientName,
                idChat + ".xml"
            );

            // Check if the chat exists.
            if (!File.Exists(fileName))
                return;

            // Create a new xml document that contains the chat log.
            XmlDocument document = new XmlDocument();

            // Load the contents of the chat log file into the xml document.
            document.Load(fileName);

            int i = document.DocumentElement.ChildNodes.Count - 1;
            // Create a new dictionary that contains the
            // result json string seperated by the sent date.
            Dictionary<DateTime, string> result = new Dictionary<DateTime, string>();

            while (true)
            {
                if (i == document.DocumentElement.ChildNodes.Count - limit)
                    break;

                XmlNode xmlNode = document.DocumentElement.ChildNodes[i];

                DateTime sent = DateTime.Parse(xmlNode.Attributes["Sent"].Value);

                result.Add(sent, RenderChatMessageJson(
                    Guid.Parse(xmlNode.Attributes["IdUser"].Value),
                    xmlNode,
                    sent
                ));

                i--;
            }

            context.Response.Write(string.Join(
                ",",
                result.OrderByDescending(x => x.Key).Select(x => x.Value)
            ));
        }

        #endregion
    }
}