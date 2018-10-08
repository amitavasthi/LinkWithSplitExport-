using ApplicationUtilities;
using ApplicationUtilities.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace LinkOnline.Classes
{
    public class SessionSynch
    {
        #region Properties

        public Dictionary<string, Dictionary<string, string>> SessionSynchLog { get; set; }

        public Dictionary<string, bool> ServersSynchStates { get; set; }

        public System.Timers.Timer SessionSynchTimer { get; set; }

        #endregion


        #region Constructor

        public SessionSynch()
        {
            SessionSynchLog = new Dictionary<string, Dictionary<string, string>>();

            int interval = 10000;

            if (ConfigurationManager.AppSettings["SynchInterval"] != null)
                int.Parse(ConfigurationManager.AppSettings["SynchInterval"]);

            SessionSynchTimer = new System.Timers.Timer(interval);
            SessionSynchTimer.Elapsed += SessionSynchTimer_Elapsed;
            SessionSynchTimer.Start();
        }

        #endregion


        #region Methods

        private bool GetServerSynchState(string ip)
        {
            if (this.ServersSynchStates.ContainsKey(ip))
                return this.ServersSynchStates[ip];

            bool result = false;

            try
            {
                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}/Handlers/Public.ashx",
                    ConfigurationManager.AppSettings["LinkAdminHostname"]
                ));

                // Get server state of target.
                string serverState = service.Request(service.Address +
                    "?Method=GetServerState" + (string.IsNullOrEmpty(ip) ? "" : ("&IP=" + ip)), new byte[0]);

                if (serverState == "Online")
                    result = true;
            }
            catch
            { }

            this.ServersSynchStates.Add(ip, result);

            return this.ServersSynchStates[ip];
        }

        #endregion


        #region Event Handlers


        private void SessionSynchTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SessionSynchTimer.Stop();

            try {
                this.ServersSynchStates = new Dictionary<string, bool>();

                // Get server state of self.
                bool serverState = GetServerSynchState(
                    ConfigurationManager.AppSettings["ServerIP"]
                );

                /*if (!serverState)
                {
                    SessionSynchTimer.Start();
                    return;
                }*/

                string physicalApplicationPath = HttpRuntime.AppDomainAppPath;

                ClientManager clientManager = new ClientManager();

                foreach (string clientName in Global.AllSessions.Keys)
                {
                    Client client = clientManager.GetSingle(clientName);

                    if (client == null)
                        continue;

                    if (client.SynchServers.Length == 0)
                        continue;

                    foreach (Guid idUser in Global.AllSessions[clientName].Keys)
                    {
                        string sessionId = Global.AllSessions[clientName][idUser].SessionID;

                        if (!this.SessionSynchLog.ContainsKey(sessionId))
                            this.SessionSynchLog.Add(sessionId, new Dictionary<string, string>());

                        StringBuilder xmlBuilder = new StringBuilder();

                        if (Global.AllSessions[clientName][idUser]["User"] == null)
                            continue;

                        foreach (string key in Global.AllSessions[clientName][idUser].Keys)
                        {
                            try
                            {
                                if (Global.AllSessions[clientName][idUser][key] == null)
                                    continue;

                                //byte[] bytes = HttpContext.Current.Session[key].ToByteArray();
                                string valueStr = Global.AllSessions[clientName][idUser][key].ToString();
                                string typeName = Global.AllSessions[clientName][idUser][key].GetType().Name;

                                switch (typeName)
                                {
                                    case "Guid":
                                    case "Language":
                                    case "Int16":
                                    case "Int32":
                                    case "Int64":
                                        break;
                                    case "String":

                                        if (File.Exists(valueStr))
                                        {
                                            typeName = "Path";

                                            valueStr = valueStr.Replace(
                                                physicalApplicationPath,
                                                ""
                                            );
                                        }

                                        break;
                                    default:
                                        continue;
                                }

                                if (!this.SessionSynchLog[sessionId].ContainsKey(key))
                                    this.SessionSynchLog[sessionId].Add(key, "");

                                if (this.SessionSynchLog[sessionId][key] == valueStr)
                                    continue;

                                xmlBuilder.Append(string.Format(
                                    "<Value Key=\"{0}\" Type=\"{2}\"><![CDATA[{1}]]></Value>",
                                    key,
                                    valueStr,//System.Text.Encoding.UTF8.GetString(bytes),
                                    typeName
                                ));

                                this.SessionSynchLog[sessionId][key] = valueStr;
                            }
                            catch
                            { }
                        }

                        if (xmlBuilder.Length == 0)
                            continue;

                        foreach (string server in client.SynchServers)
                        {
                            // Check if the server is online.
                            /*if (!GetServerSynchState(server))
                            {
                                this.SessionSynchLog[sessionId].Clear();
                                continue;
                            }*/

                            try
                            {
                                Dictionary<string, string> cookies = new Dictionary<string, string>();
                                cookies.Add("ASP.NET_SessionId", sessionId);

                                ServiceLink service = new ServiceLink(string.Format(
                                    "http://{0}/Handlers/Synch.ashx",
                                    server
                                ));
                                service.HostHeader = client.Host;

                                service.Request(new string[]
                                {
                                "Method=SetSessionUpdates"
                                }, System.Text.Encoding.UTF8.GetBytes(string.Format(
                                    "<Session Id=\"{0}\" Client=\"{2}\" IdUser=\"{3}\">{1}</Session>",
                                    sessionId,
                                    xmlBuilder.ToString(),
                                    clientName,
                                    idUser
                                )), cookies);
                            }
                            catch (Exception ex)
                            {
                                this.SessionSynchLog[sessionId].Clear();
                            }
                        }
                    }
                }
            }
            catch { }
            SessionSynchTimer.Start();
        }

        #endregion
    }
}