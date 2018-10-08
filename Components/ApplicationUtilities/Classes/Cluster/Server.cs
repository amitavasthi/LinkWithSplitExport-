using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Xml;

namespace ApplicationUtilities.Cluster
{
    public class Server
    {
        #region Properties

        /// <summary>
        /// Gets or sets the xml node that
        /// contains the server definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValue("Name");
            }
            set
            {
                this.SetValue("Name", value);
            }
        }

        /// <summary>
        /// Gets or sets the role of the server.
        /// </summary>
        public ServerRole Role
        {
            get
            {
                return (ServerRole)Enum.Parse(
                    typeof(ServerRole),
                    this.GetValue("Role")
                );
            }
            set
            {
                this.SetValue("Role", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the role of the server.
        /// </summary>
        public ServerState State
        {
            get
            {
                return (ServerState)Enum.Parse(
                    typeof(ServerState),
                    this.GetValue("State")
                );
            }
            set
            {
                this.SetValue("State", value.ToString());
            }
        }

        public string[] Countries
        {
            get
            {
                return this.GetValue("Countries").Split(',');
            }
            set
            {
                this.SetValue("Countries", string.Join(",", value));
            }
        }

        public int Sessions { get; set; }

        /// <summary>
        /// Gets or sets the IP of the server.
        /// </summary>
        public string IP
        {
            get
            {
                return this.GetValue("IP");
            }
            set
            {
                this.SetValue("IP", value);
            }
        }

        /// <summary>
        /// Gets or sets the description of the server.
        /// </summary>
        public string Description
        {
            get
            {
                return this.GetValue("Description");
            }
            set
            {
                this.SetValue("Description", value);
            }
        }

        /// <summary>
        /// Gets or sets if the server
        /// is set up for load balancing.
        /// </summary>
        public bool IsLoadBalancing { get; set; }

        #endregion


        #region Constructor

        public Server(XmlNode xmlNode)
        {
            this.XmlNode = xmlNode;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Sets the value of a specified
        /// property of the server.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value to set.</param>
        private void SetValue(string name, string value)
        {
            this.XmlNode.Attributes[name].Value = value;
        }

        /// <summary>
        /// Returns the value of a specified
        /// property of the server.
        /// <param name="name">The name of the property.</param>
        private string GetValue(string name)
        {
            return this.XmlNode.Attributes[name].Value;
        }

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