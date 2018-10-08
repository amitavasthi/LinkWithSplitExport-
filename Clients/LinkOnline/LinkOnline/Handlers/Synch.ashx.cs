using ApplicationUtilities;
using ApplicationUtilities.Classes;
using Crosstables.Classes.HierarchyClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Xml;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for Synch
    /// </summary>
    public class Synch : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public Synch()
            : base(false, true)
        {
            base.Methods.Add("SetSessionUpdates", SetSessionUpdates);
            base.Methods.Add("CreateSession", CreateSession);
        }

        #endregion


        #region Methods

        private void SessionInit(Guid idUser,string physicalApplicationPath , string clientName, string sessionId)
        {
            /*ServiceLink service = new ServiceLink(string.Format(
                HttpContext.Current.Request.Url.ToString().Split('?')[0]
            ));

            Dictionary<string, string> cookies = new Dictionary<string, string>();
            cookies.Add("ASP.NET_SessionId", sessionId);

            service.Request(new string[] {
                "Method=CreateSession"
            }, cookies);*/

            /*var state = (HttpSessionState)System.Runtime.Serialization
                    .FormatterServices.GetUninitializedObject(typeof(HttpSessionState));

            var containerFld = typeof(HttpSessionState).GetField(
                "_container", BindingFlags.Instance | BindingFlags.NonPublic);

            ClientManager clientManager = new ClientManager();

            Client client = clientManager.GetSingle(clientName);

            string connectionString = string.Format(
                ConfigurationManager.AppSettings["ConnectionString"],
                client.Database
            );

            var itemCollection = new SessionStateItemCollection();
            itemCollection["User"] = idUser;
            itemCollection["ClientManager"] = clientManager;
            itemCollection["Core"] = new DatabaseCore.Core(
                ConfigurationManager.AppSettings["DatabaseProvider"],
                connectionString,
                ConfigurationManager.AppSettings["DatabaseProviderUserManagement"],
                connectionString,
                client.SynchServers
            );
            itemCollection["LanguageManager"] = new LanguageManager();

            // Initialize the session's permission core.
            itemCollection["PermissionCore"] = new PermissionCore.PermissionCore("LinkOnline");

            // Set the database core's file storage path.
            ((DatabaseCore.Core)itemCollection["Core"]).FileStorageRoot = string.Format(
                ConfigurationManager.AppSettings["FileStorageRoot"],
                clientName
            );

            ((DatabaseCore.Core)itemCollection["Core"]).ClientName = clientName;
            ((DatabaseCore.Core)itemCollection["Core"]).CaseDataLocation = client.CaseDataLocation;

            containerFld.SetValue(
                state,
                new HttpSessionStateContainer(
                    sessionId,
                    itemCollection,
                    new HttpStaticObjectsCollection(),
                    900,
                    true,
                    HttpCookieMode.UseCookies,
                    SessionStateMode.InProc,
                    false
                )
            );

            Global.AllSessions[clientName].Add(idUser, state);*/
            ClientManager clientManager = new ClientManager();

            Client client = clientManager.GetSingle(clientName);

            string connectionString = string.Format(
                ConfigurationManager.AppSettings["ConnectionString"],
                client.Database
            );

            HttpContext.Current.Session["User"] = idUser;

            HttpContext.Current.Session["Core"] = new DatabaseCore.Core(
                ConfigurationManager.AppSettings["DatabaseProvider"],
                connectionString,
                ConfigurationManager.AppSettings["DatabaseProviderUserManagement"],
                connectionString,
                client.SynchServers
            );
            // Set the database core's file storage path.
            ((DatabaseCore.Core)HttpContext.Current.Session["Core"]).FileStorageRoot = string.Format(
                ConfigurationManager.AppSettings["FileStorageRoot"],
                clientName
            );

            ((DatabaseCore.Core)HttpContext.Current.Session["Core"]).ClientName = clientName;
            ((DatabaseCore.Core)HttpContext.Current.Session["Core"]).CaseDataLocation = client.CaseDataLocation;

            HttpContext.Current.Session["HierarchyFilters"] = new HierarchyFilterCollection();

            HttpContext.Current.Session["PermissionCore"] = new PermissionCore.PermissionCore(
                physicalApplicationPath,
                "LinkOnline",
                clientName
            );

            Global.AllSessions[clientName].Add(idUser, HttpContext.Current.Session);
        }

        public IEnumerable<SessionStateItemCollection> GetActiveSessions()
        {
            object obj;
            object[] obj2 = new object[0];
            try
            {
                obj = typeof(HttpRuntime).GetProperty("CacheInternal", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);
                obj2 = (object[])obj.GetType().GetField("_caches", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
            }
            catch { }
            for (int i = 0; i < obj2.Length; i++)
            {
                Hashtable c2 = (Hashtable)obj2[i].GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj2[i]);
                foreach (DictionaryEntry entry in c2)
                {
                    object o1 = entry.Value.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(entry.Value, null);
                    if (o1.GetType().ToString() == "System.Web.SessionState.InProcSessionState")
                    {
                        SessionStateItemCollection sess = (SessionStateItemCollection)o1.GetType().GetField("_sessionItems", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(o1);
                        if (sess != null)
                        {
                            yield return sess;
                        }
                    }
                }
            }
        }

        #endregion


        #region Web methods

        private void SetSessionUpdates(HttpContext context)
        {
            MemoryStream ms = new MemoryStream();
            context.Request.InputStream.CopyTo(ms);

            XmlDocument document = new XmlDocument();
            document.LoadXml(System.Text.Encoding.UTF8.GetString(ms.ToArray()));

            //foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("Session"))
            XmlNode xmlNode = document.DocumentElement;

            string sessionId = xmlNode.Attributes["Id"].Value;
            string client = xmlNode.Attributes["Client"].Value;
            Guid idUser = Guid.Parse(xmlNode.Attributes["IdUser"].Value);

            if (!Global.AllSessions.ContainsKey(client))
            {
                Global.AllSessions.Add(client, new Dictionary<Guid, HttpSessionState>());
            }

            if (!Global.AllSessions[client].ContainsKey(idUser))
            {
                SessionInit(idUser, context.Request.PhysicalApplicationPath, client, sessionId);
            }

            foreach (XmlNode xmlNodeValue in xmlNode.SelectNodes("Value"))
            {
                object value = null;

                switch (xmlNodeValue.Attributes["Type"].Value)
                {
                    case "Language":
                        value = (WebUtilities.Language)Enum.Parse(
                            typeof(WebUtilities.Language),
                            xmlNodeValue.InnerText
                        );
                        break;
                    case "String":
                        value = xmlNodeValue.InnerText;
                        break;
                    case "Guid":
                        value = Guid.Parse(xmlNodeValue.InnerText);
                        break;
                    case "Int16":
                        value = Int16.Parse(xmlNodeValue.InnerText);
                        break;
                    case "Int32":
                        value = Int32.Parse(xmlNodeValue.InnerText);
                        break;
                    case "Int64":
                        value = Int64.Parse(xmlNodeValue.InnerText);
                        break;
                }

                if (value == null)
                    continue;

                Global.AllSessions[client][idUser][xmlNodeValue.Attributes["Key"].Value] = value;
            }

        }

        private void CreateSession(HttpContext context)
        {
            // Parse the id of the user from
            // the http request's parameters.
            Guid idUser = Guid.Parse(context.Request.Params["IdUser"]);

            // Parse the name of the client from
            // the http request's parameters.
            string client = context.Request.Params["Client"];

            if (!Global.AllSessions.ContainsKey(client))
                Global.AllSessions.Add(client, new Dictionary<Guid, HttpSessionState>());

            if (!Global.AllSessions[client].ContainsKey(idUser))
                Global.AllSessions[client].Add(new Guid(), context.Session);
        }

        #endregion
    }
}