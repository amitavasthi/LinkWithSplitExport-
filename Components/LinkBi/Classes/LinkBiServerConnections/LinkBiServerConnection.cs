using LinkBi1.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using WebUtilities;
using WebUtilities.Controls;

namespace LinkBi1.Classes
{
    public abstract class LinkBiServerConnection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning LinkBi Definition.
        /// </summary>
        public LinkBiDefinition Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains
        /// the server connection properties.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        public Guid Identity
        {
            get
            {
                return Guid.Parse(this.XmlNode.Attributes["Id"].Value);
            }
        }


        /// <summary>
        /// Gets or sets the type of the server connection.
        /// </summary>
        public LinkBiServerConnectionType Type
        {
            get
            {
                return (LinkBiServerConnectionType)Enum.Parse(
                    typeof(LinkBiServerConnectionType),
                    this.GetValue("Type", LinkBiServerConnectionType.FileSystem.ToString())
                );
            }
            set
            {
                this.SetValue("Type", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the interface type of the server connection.
        /// </summary>
        public LinkBiInterfaceType InterfaceType
        {
            get
            {
                return (LinkBiInterfaceType)Enum.Parse(
                    typeof(LinkBiInterfaceType),
                    this.GetValue("InterfaceType", LinkBiInterfaceType.PowerBI.ToString())
                );
            }
            set
            {
                this.SetValue("InterfaceType", value.ToString());
            }
        }

        public DateTime LatestDeploy 
        {
            get
            {
                DateTime result = new DateTime();

                XmlNode xmlNode = this.XmlNode.SelectSingleNode("LatestDeploy");

                if (xmlNode != null)
                    result = DateTime.Parse(xmlNode.InnerXml);

                return result;
            }
            set
            {
                XmlNode xmlNode = this.XmlNode.SelectSingleNode("LatestDeploy");

                if (xmlNode == null)
                {
                    this.XmlNode.InnerXml += string.Format(
                        "<{0}>{1}</{0}>",
                        "LatestDeploy",
                        value.ToString()
                    );
                }
                else
                {
                    xmlNode.InnerXml = value.ToString();
                }
            }
        }

        public bool Outdated
        {
            get
            {
                XmlNode xmlNode = this.XmlNode.SelectSingleNode("Outdated");

                if (xmlNode != null)
                    return bool.Parse(xmlNode.InnerXml);

                return false;
            }
            set
            {
                XmlNode xmlNode = this.XmlNode.SelectSingleNode("Outdated");

                if (xmlNode == null)
                {
                    this.XmlNode.InnerXml += string.Format(
                        "<{0}>{1}</{0}>",
                        "Outdated",
                        value.ToString()
                    );
                }
                else
                {
                    xmlNode.InnerXml = value.ToString();
                }
            }
        }

        public LanguageManager LanguageManager
        {
            get
            {
                return (LanguageManager)HttpContext.Current.Session["LanguageManager"];
            }
        }

        #endregion


        #region Constructor

        public LinkBiServerConnection(LinkBiDefinition owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;
        }

        #endregion


        #region Methods

        protected void SetValue(string name, string value)
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

        protected string GetValue(string name, string defaultValue = null)
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


        public abstract TableRow[] Render();

        public abstract bool Deploy(string fileName);

        public abstract bool IsValid();

        #endregion
    }

    public enum LinkBiServerConnectionType
    {
        FileSystem,
        FTP/*,
        WebUpload*/
    }
}
