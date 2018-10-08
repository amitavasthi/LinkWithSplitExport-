using ApplicationUtilities;
using Azure_Switch.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Azure_Switch
{
    public partial class Default : System.Web.UI.Page
    {
        #region Properties

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private string CheckSession()
        {
            string sessionId = "";

            try
            {
                ServerCollection servers = new ServerCollection(Path.Combine(
                    Request.PhysicalApplicationPath,
                    "App_Data",
                    "Servers.xml"
                ));

                string server;

                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    sessionId = Request.Cookies["ASP.NET_SessionId"].Value;

                    if (!Global.Sessions.ContainsKey(sessionId))
                    {
                        //sessionId = GenerateNewSessionId();
                        //Request.Cookies.Remove("ASP.NET_SessionId");
                        //Request.Cookies.Add(new HttpCookie("ASP.NET_SessionId", sessionId));

                        server = servers.GetServer(Request.ServerVariables["REMOTE_HOST"]);
                    }
                    else if (servers.Items.ContainsKey(Global.Sessions[sessionId]) == false ||
                        servers.Items[Global.Sessions[sessionId]].State == ServerState.Offline)
                    {
                        server = servers.GetServer(Request.ServerVariables["REMOTE_HOST"]);
                    }
                    else
                    {
                        server = Global.Sessions[sessionId];
                    }

                    if (server == null)
                    {
                        ShowOverloadMessage();
                        return null;
                    }
                }
                else
                {
                    server = servers.GetServer(Request.ServerVariables["REMOTE_HOST"]);

                    if(server == null)
                    {
                        ShowOverloadMessage();
                        return null;
                    }

                    ServiceLink service = new ServiceLink(string.Format(
                        "http://{0}/Handlers/GlobalHandler.ashx",
                        server
                    ));

                    /*Dictionary<string, string> cookies = new Dictionary<string, string>();

                    cookies.Add("ASP.NET_SessionId", GenerateNewSessionId());*/


                    sessionId = service.Request(new string[]
                    {
                        "Method=CreateSession"
                    });

                    Request.Cookies.Add(new HttpCookie("ASP.NET_SessionId", sessionId));
                }

                if (!Global.Sessions.ContainsKey(sessionId))
                {
                    Global.Sessions.Add(
                        sessionId,
                        server
                    );
                }
                else
                {
                    Global.Sessions[sessionId] = server;
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
                Response.End();
                return null;
            }

            return sessionId;
        }

        private string GenerateNewSessionId()
        {
            SessionIDManager manager = new SessionIDManager();
            string result = manager.CreateSessionID(HttpContext.Current);

            string client = Request.Url.Host.Split('.')[0];

            result = result.Substring(client.Length, result.Length - client.Length);

            result = client + result;

            return result;
        }

        private void ShowOverloadMessage()
        {
            Context.Response.Clear();
            Context.Response.Write(File.ReadAllText(Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "ClusterOverloadMessage.html"
            )));
            Context.Response.End();
        }

        private void DeleteCacheDirectory(string name)
        {
            string directoryName = Path.Combine(
                Request.PhysicalApplicationPath,
                name
            );

            if (Directory.Exists(directoryName))
                Directory.Delete(directoryName, true);
        }


        private void ProcessRequest()
        {
            try
            {
                Response.Clear();
                
                Response.Headers.Add("Arr-Disable-Session-Affinity", "True");

                // Load all parameter keys to ignore.
                string[] ignoreParams = File.ReadAllText(Path.Combine(
                    Request.PhysicalApplicationPath,
                    "App_Data",
                    "IgnoreParams.txt"
                )).Split(',');

                string parameters = "";

                // Run through all request parameters.
                foreach (string key in Request.Params.AllKeys)
                {
                    if (key == null)
                        continue;

                    if (key.StartsWith("404"))
                        continue;

                    if (ignoreParams.Contains(key))
                        continue;

                    //if (Request.HttpMethod != "GET")
                        parameters += key + "=" + HttpUtility.UrlEncode(Request.Params[key]) + "&";
                    /*else
                        parameters += key + "=" + (Request.Params[key]) + "&";*/
                }

                string url;

                if (Request.Params.ToString().Split('&')[0].StartsWith("404"))
                    url = ConfigurationManager.AppSettings["Protocol"] + "://" + "216.176.177.26" +
                        new Uri(HttpUtility.UrlDecode(Request.Params.ToString().Split('&')[0]).Split(';')[1]).PathAndQuery;
                else
                    url = ConfigurationManager.AppSettings["Protocol"] + "://" + "216.176.177.26" + Request.Params["Path_Info"];

                if (parameters.Length > 0)
                    parameters = parameters.Remove(parameters.Length - 1, 1);

                SwitchHandler switchHandler = new SwitchHandler();

                if (switchHandler.Switch(
                    "216.176.177.26",
                    url,
                    parameters
                ) == false)
                {
                    ServerCollection servers = new ServerCollection(Path.Combine(
                        Request.PhysicalApplicationPath,
                        "App_Data",
                        "Servers.xml"
                    ));
                }

                /*if (Response.Cookies["ASP.NET_SessionId"] == null)
                {
                    HttpCookie cookie = new HttpCookie("ASP.NET_SessionId", Request.Cookies["ASP.NET_SessionId"].Value);
                    cookie.Domain = Request.Url.Host;
                    cookie.Path = "/";

                    Response.Cookies.Add(cookie);
                }
                else if (Response.Cookies["ASP.NET_SessionId"].Value == "")
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = Request.Cookies["ASP.NET_SessionId"].Value;
                }*/
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Log.txt"
                ), ex.ToString());
            }
        }
        private void _ProcessRequest()
        {
            try
            {
                Response.Clear();

                string sessionId = CheckSession();

                if (sessionId == null)
                    return;

                if (Request.Params["SwitchDebugVar1"] != null)
                {
                    Response.Write(Global.Sessions[sessionId]);
                    Response.End();
                    return;
                }

                if (Request.Params["DeleteSwitchCache"] != null)
                {
                    DeleteCacheDirectory("Images");
                    DeleteCacheDirectory("Scripts");
                    DeleteCacheDirectory("Stylesheets");
                }

                if (Request.Url.ToString().EndsWith("favicon.ico"))
                {
                    Response.BinaryWrite(new byte[0]);
                    Response.End();
                }

                Response.Headers.Add("Arr-Disable-Session-Affinity", "True");

                // Load all parameter keys to ignore.
                string[] ignoreParams = File.ReadAllText(Path.Combine(
                    Request.PhysicalApplicationPath,
                    "App_Data",
                    "IgnoreParams.txt"
                )).Split(',');

                string parameters = "";

                // Run through all request parameters.
                foreach (string key in Request.Params.AllKeys)
                {
                    if (key == null)
                        continue;

                    if (key.StartsWith("404"))
                        continue;

                    if (ignoreParams.Contains(key))
                        continue;

                    if (Request.HttpMethod != "GET")
                        parameters += key + "=" + HttpUtility.UrlEncode(Request.Params[key]) + "&";
                    else
                        parameters += key + "=" + (Request.Params[key]) + "&";
                }

                string url;

                if (Request.Params.ToString().Split('&')[0].StartsWith("404"))
                    url = ConfigurationManager.AppSettings["Protocol"] + "://" + Global.Sessions[sessionId] +
                        new Uri(HttpUtility.UrlDecode(Request.Params.ToString().Split('&')[0]).Split(';')[1]).PathAndQuery;
                else
                    url = ConfigurationManager.AppSettings["Protocol"] + "://" + Global.Sessions[sessionId] + Request.Params["Path_Info"];

                if (parameters.Length > 0)
                    parameters = parameters.Remove(parameters.Length - 1, 1);

                SwitchHandler switchHandler = new SwitchHandler();

                if (switchHandler.Switch(
                    Global.Sessions[sessionId],
                    url,
                    parameters
                ) == false)
                {
                    ServerCollection servers = new ServerCollection(Path.Combine(
                        Request.PhysicalApplicationPath,
                        "App_Data",
                        "Servers.xml"
                    ));

                    if (servers.Items.ContainsKey(Global.Sessions[sessionId]))
                    {
                        try
                        {
                            SendServerOfflineNotification(servers.Items[Global.Sessions[sessionId]]);
                        }
                        catch { }

                        servers.Items[Global.Sessions[sessionId]].State = ServerState.Offline;
                        servers.Save();

                        ProcessRequest();
                        return;
                    }
                }

                Response.AddHeader("LiNK_Server", Global.Sessions[sessionId]);

                /*if (Response.Cookies["ASP.NET_SessionId"] == null)
                {
                    HttpCookie cookie = new HttpCookie("ASP.NET_SessionId", Request.Cookies["ASP.NET_SessionId"].Value);
                    cookie.Domain = Request.Url.Host;
                    cookie.Path = "/";

                    Response.Cookies.Add(cookie);
                }
                else if (Response.Cookies["ASP.NET_SessionId"].Value == "")
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = Request.Cookies["ASP.NET_SessionId"].Value;
                }*/
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Log.txt"
                ), ex.ToString());
            }
        }

        private void SendServerOfflineNotification(Server server)
        {
            // configuration values from the web.config file.
            MailConfiguration mailConfiguration = new MailConfiguration(true);
            // Create a new mail by the mail configuration.
            Mail mail = new Mail(mailConfiguration, "_NONE_")
            {
                TemplatePath = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "App_Data",
                    "ServerOfflineNotification.html"
                ),
                Subject = "SERVER OFFLINE"
            };

            mail.Placeholders.Add("ServerIp", server.IP);
            mail.Placeholders.Add("Server", server.Description);

            // Send the mail.
            mail.Send(ConfigurationManager.AppSettings["ServerOfflineNotificationReciepent"]);
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            ProcessRequest();
        }

        #endregion
    }
}