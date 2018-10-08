using ApplicationUtilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using System.Xml;

namespace LinkAdmin.Classes
{
    public class ServerCollection
    {
        #region Properties

        public Dictionary<string, Server> Items { get; set; }
        public int Length
        {
            get
            {
                return this.Items.Count;
            }
        }

        #endregion


        #region Constructor

        public ServerCollection()
        {
            // Create a new list for the result server list.
            this.Items = new Dictionary<string, Server>();

            // Create a new xml document that
            // contains the server configuration.
            XmlDocument document = new XmlDocument();

            // Build the full path to the server
            // configuration file and load it's
            // contents to the xml document.
            /*document.Load(Path.Combine(
                Global.SwitchRoot,
                "App_Data",
                "Servers.xml"
            ));*/

            ServiceLink service = new ServiceLink(
                ConfigurationManager.AppSettings["LinkSwitchServiceUrl"]
            );

            document.LoadXml(service.Request(new string[]
            {
                "Method=GetServers"
            }));

            // Run through all xml nodes that define a server.
            foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("Server"))
            {
                Server server = new Server(xmlNode);

                if (this.Items.ContainsKey(server.IP))
                    continue;

                // Parse the server definition.
                this.Items.Add(server.IP, server);
            }
        }

        #endregion


        #region Methods

        public void Save()
        {
            // Create a new string builder that
            // contains the result xml string.
            StringBuilder result = new StringBuilder();

            result.Append("<Servers>");

            // Run through all defined servers.
            foreach (Server server in this.Items.Values)
            {
                result.Append(string.Format(
                    "<Server IP=\"{0}\" Description=\"{1}\" Role=\"{2}\" Countries=\"{3}\" State=\"{4}\"></Server>",
                    server.IP,
                    server.Description,
                    server.Role.ToString(),
                    string.Join(",", server.Countries),
                    server.State.ToString()
                ));
            }

            result.Append("</Servers>");

            ServiceLink service = new ServiceLink(
                ConfigurationManager.AppSettings["LinkSwitchServiceUrl"]
            );

            service.Request(new string[]
            {
                "Method=SetServers"
            }, System.Text.Encoding.UTF8.GetBytes(result.ToString()));
        }

        #endregion
    }

    public class Server
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ip of the server.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the description of the server.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the role of the server.
        /// </summary>
        public ServerRole Role { get; set; }

        /// <summary>
        /// Gets or sets the state of the server.
        /// </summary>
        public ServerState State { get; set; }

        /// <summary>
        /// Gets or sets an array of countries
        /// that are assigned to the server.
        /// </summary>
        public string[] Countries { get; set; }

        #endregion


        #region Constructor

        public Server(XmlNode xmlNode)
        {
            this.IP = xmlNode.Attributes["IP"].Value;

            this.Description = xmlNode.Attributes["Description"].Value;

            this.Role = (ServerRole)Enum.Parse(
                typeof(ServerRole),
                xmlNode.Attributes["Role"].Value
            );

            this.State = (ServerState)Enum.Parse(
                typeof(ServerState),
                xmlNode.Attributes["State"].Value
            );

            this.Countries = xmlNode.Attributes["Countries"].Value.Split(',');
        }

        #endregion


        #region Methods

        public ConnectionQuality GetConnectionQuality()
        {
            try
            {
                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}:8080/Handler.ashx",
                    this.IP
                ));

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                bool result = false;

                try
                {
                    service.Request(new string[]
                    {
                        "Method=Ping"
                    });

                    result = true;
                }
                catch { }

                stopwatch.Stop();

                return new ConnectionQuality(result, stopwatch.ElapsedMilliseconds);
            }
            catch
            {
                return new ConnectionQuality();
            }
        }

        public bool SynchEnabled()
        {
            if (this.State == ServerState.Offline)
                return false;

            ServiceLink service = new ServiceLink(string.Format(
                "http://{0}:8080/Handler.ashx",
                this.IP
            ));

            try
            {
                return bool.Parse(service.Request(new string[] { "Method=SynchEnabled" }));
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    public class ConnectionQuality
    {
        #region Properties

        public long Ping { get; set; }

        public ConnectionQualityRank Rank { get; set; }

        #endregion


        #region Constructor

        public ConnectionQuality(PingReply pingReply)
        {
            this.Ping = pingReply.RoundtripTime;

            if (pingReply.Status != IPStatus.Success)
            {
                this.Rank = ConnectionQualityRank.NA;
                return;
            }

            this.SetRank();
        }

        public ConnectionQuality(bool succeeded, long elapsed)
        {
            this.Ping = elapsed;

            if (!succeeded)
            {
                this.Rank = ConnectionQualityRank.NA;
                return;
            }

            this.SetRank();
        }

        public ConnectionQuality()
        {
            this.Ping = 0;
            this.Rank = ConnectionQualityRank.NA;
        }

        #endregion


        #region Methods

        private void SetRank()
        {
            if (this.Ping < 50)
            {
                this.Rank = ConnectionQualityRank.VeryGood;
            }
            else if (this.Ping < 100)
            {
                this.Rank = ConnectionQualityRank.Good;
            }
            else if (this.Ping < 150)
            {
                this.Rank = ConnectionQualityRank.Normal;
            }
            else if (this.Ping < 200)
            {
                this.Rank = ConnectionQualityRank.Bad;
            }
            else if (this.Ping >= 200)
            {
                this.Rank = ConnectionQualityRank.VeryBad;
            }
        }

        #endregion
    }

    public enum ConnectionQualityRank
    {
        VeryGood,
        Good,
        Normal,
        Bad,
        VeryBad,
        NA
    }

    public enum ServerRole
    {
        Primary,
        Failover,
        Production
    }

    public enum ServerState
    {
        Online,
        Offline
    }
}