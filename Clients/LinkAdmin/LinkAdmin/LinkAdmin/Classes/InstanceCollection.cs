using ApplicationUtilities;
using ApplicationUtilities.Cluster;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace LinkAdmin.Classes
{
    public class InstanceCollection
    {
        #region Properties

        public ServerCollection Servers { get; set; }

        public Dictionary<string, Instance> Instances { get; set; }

        #endregion


        #region Constructor

        public InstanceCollection(ServerCollection servers = null)
        {
            this.Servers = servers;
            this.Instances = new Dictionary<string, Instance>();

            if (this.Servers == null)
            {
                // Get all defined servers.
                this.Servers = new ServerCollection(Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "App_Data",
                    "Servers.xml"
                ));
            }

            foreach (string ip in this.Servers.Items.Keys)
            {
                Server server = this.Servers.Items[ip];

                if (server.State == ServerState.Offline)
                    continue;

                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}:8080/Handler.ashx",
                    server.IP
                ));

                string xmlString;

                try
                {
                    xmlString = service.Request(new string[]
                    {
                        "Method=GetInstances"
                    });
                }
                catch (Exception ex)
                {
                    continue;
                }

                XmlDocument document = new XmlDocument();
                document.LoadXml(xmlString);

                foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("Instance"))
                {
                    string instanceName = xmlNode.Attributes["Name"].Value;

                    if (!this.Instances.ContainsKey(instanceName))
                        this.Instances.Add(instanceName, new Instance(this, xmlNode));

                    this.Instances[instanceName].BindPortals(xmlNode, server);
                }
            }
        }

        #endregion


        #region Methods

        #endregion
    }

    public class Instance
    {
        #region Properties

        public InstanceCollection Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of the instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the software version of the instance.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the portals of the instance.
        /// </summary>
        public Dictionary<string, Portal> Portals { get; set; }

        public List<string> Servers { get; set; }

        #endregion


        #region Constructor

        public Instance(InstanceCollection owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.Servers = new List<string>();
            this.Portals = new Dictionary<string, Portal>();
            this.Name = xmlNode.Attributes["Name"].Value;
            this.Version = xmlNode.Attributes["Version"].Value;
        }

        public void BindPortals(XmlNode xmlNode, Server server)
        {
            if (!this.Servers.Contains(server.IP))
                this.Servers.Add(server.IP);

            foreach (XmlNode xmlNodePortal in xmlNode.SelectNodes("Client"))
            {
                string portalName = xmlNodePortal.Attributes["Name"].Value;

                if (!this.Portals.ContainsKey(portalName))
                    this.Portals.Add(portalName, new Portal(this, xmlNodePortal));

                if (this.Portals[portalName].Servers.Find(x => x.IP == server.IP) != null)
                    continue;

                this.Portals[portalName].Servers.Add(server);
            }
        }

        #endregion
    }

    public class Portal
    {
        #region Properties

        public Instance Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of the portal.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the hostname of the portal.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the data size of the portal.
        /// </summary>
        public long DataSize { get; set; }

        /// <summary>
        /// Gets or sets the servers
        /// where the portal is on.
        /// </summary>
        public List<Server> Servers { get; set; }

        #endregion


        #region Constructor

        public Portal(Instance owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.Servers = new List<Server>();

            this.Name = xmlNode.Attributes["Name"].Value;
            this.Host = xmlNode.Attributes["Host"].Value;

            if (xmlNode.Attributes["Synch"] != null)
            {
                foreach (string server in xmlNode.Attributes["Synch"].Value.Split(','))
                {
                    if (!this.Owner.Owner.Servers.Items.ContainsKey(server.Trim()))
                        continue;

                    this.Servers.Add(this.Owner.Owner.Servers.Items[server.Trim()]);
                }
            }
        }

        #endregion
    }
}