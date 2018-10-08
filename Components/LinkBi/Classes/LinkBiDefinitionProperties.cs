using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LinkBi1.Classes
{
    public class LinkBiDefinitionProperties
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning LinkBi definition
        /// where the properties are part of.
        /// </summary>
        public LinkBiDefinition Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node that
        /// contains the property definitions.
        /// </summary>
        public XmlNode XmlNode { get; set; }


        /// <summary>
        /// Gets or sets a list of server connections.
        /// </summary>
        public Dictionary<Guid, LinkBiServerConnection> ServerConnections { get; set; }

        /// <summary>
        /// Gets or sets the name of the LinkBi report.
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
        /// Gets or sets the latest data update of the LinkBi report.
        /// </summary>
        public DateTime LatestUpdate
        {
            get
            {
                return DateTime.Parse(this.GetValue("LatestUpdate", (new DateTime()).ToString()));
            }
            set
            {
                this.SetValue("LatestUpdate", value.ToString());
            }
        }

        #endregion


        #region Constructor

        public LinkBiDefinitionProperties(LinkBiDefinition owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;
            this.ServerConnections = new Dictionary<Guid, LinkBiServerConnection>();

            if (xmlNode != null)
                InitServerConnections();
        }

        #endregion


        #region Methods

        private void InitServerConnections()
        {
            this.ServerConnections = new Dictionary<Guid, LinkBiServerConnection>();

            // Select all server connection xml nodes.
            XmlNodeList xmlNodes = this.XmlNode.SelectNodes("ServerConnections/ServerConnection");

            // Run through all server connection xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                LinkBiServerConnectionType connectionType = LinkBiServerConnectionType.FileSystem;

                // Get the xml node that contains the connection type definition.
                XmlNode xmlNodeConnectionType = xmlNode.SelectSingleNode("Type");

                Guid id = Guid.Parse(xmlNode.Attributes["Id"].Value);

                // Check if a connection type is set.
                if (xmlNodeConnectionType != null)
                {
                    connectionType = (LinkBiServerConnectionType)Enum.Parse(
                        typeof(LinkBiServerConnectionType),
                        xmlNodeConnectionType.InnerXml
                    );
                }

                LinkBiServerConnection connection = null;

                switch (connectionType)
                {
                    case LinkBiServerConnectionType.FileSystem:

                        connection = new LinkBiServerConnections.LinkBiServerConnectionFileSystem(
                            this.Owner,
                            xmlNode
                        );

                        break;
                    case LinkBiServerConnectionType.FTP:

                        connection = new LinkBiServerConnections.LinkBiServerConnectionFTP(
                            this.Owner,
                            xmlNode
                        );

                        break;
                        /*case LinkBiServerConnectionType.WebUpload:
                            break;*/
                }

                if (connection == null)
                    continue;

                // Add the server connection to the server connections.
                this.ServerConnections.Add(id, connection);
            }
        }


        public void SetValue(string name, string value)
        {
            XmlNode xmlNode = this.XmlNode.SelectSingleNode(name);

            if (xmlNode != null)
            {
                xmlNode.InnerXml = value;
            }
            else
            {
                this.XmlNode.InnerXml += string.Format(
                    "<{0}>{1}</{0}>",
                    name,
                    value
                );

                this.Owner.Save();
            }
        }

        public string GetValue(string name, string defaultValue = null)
        {
            XmlNode xmlNode = this.XmlNode.SelectSingleNode(name);

            if (xmlNode != null)
            {
                return xmlNode.InnerXml;
            }
            else if (defaultValue != null)
            {
                this.XmlNode.InnerXml += string.Format(
                    "<{0}>{1}</{0}>",
                    name,
                    defaultValue
                );

                this.Owner.Save();

                return GetValue(name);
            }

            return "";
        }

        #endregion
    }

    public enum LinkBiDefinitionType
    {
        Download,
        Server
    }
}
