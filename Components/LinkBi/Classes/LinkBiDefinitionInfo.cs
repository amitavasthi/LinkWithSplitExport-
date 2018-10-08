using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LinkBi1.Classes
{
    public class LinkBiDefinitionInfo
    {
        #region Properties

        /// <summary>
            /// Gets or sets the full path to the
            /// link bi definition xml file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that contains
        /// the link bi definition's properties.
        /// </summary>
        public XmlDocument XmlDocument { get; set; }

        /// <summary>
        /// Gets the name of the link bi report.
        /// </summary>
        public string ReportName
        {
            get
            {
                return this.GetProperty("Name");
            }
        }

        /// <summary>
        /// Gets if the link bi definition has
        /// one or more server connections.
        /// </summary>
        public bool HasServerConnection
        {
            get
            {
                return this.GetProperty("ServerConnections") != null;
            }
        }

        /// <summary>
        /// Gets the latest update date of the link bi
        /// definition on the defined servers.
        /// </summary>
        public DateTime LatestUpdate
        {
            get
            {
                if (this.GetProperty("LatestUpdate") == null)
                    return new DateTime();

                return DateTime.Parse(this.GetProperty("LatestUpdate"));
            }
        }

        public Dictionary<Guid, DateTime> LatestUses { get; set; }

        #endregion


        #region Constructor

        public LinkBiDefinitionInfo(string fileName)
        {
            this.FileName = fileName;

            // Parse the properties xml part
            // from the link bi definition file.
            this.Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            XmlDocument temp = new XmlDocument();
            temp.Load(this.FileName);

            this.XmlDocument = new XmlDocument();
            this.XmlDocument.LoadXml(temp.SelectSingleNode("Report/Properties").OuterXml);

            this.LatestUses = new Dictionary<Guid, DateTime>();

            XmlNodeList xmlNodesLatestUses = this.XmlDocument.SelectNodes("Properties/LatestUses/LatestUse");

            foreach (XmlNode xmlNodeLatestUse in xmlNodesLatestUses)
            {
                Guid idUser = Guid.Parse(xmlNodeLatestUse.Attributes["IdUser"].Value);
                DateTime latestUse = DateTime.Parse(xmlNodeLatestUse.InnerXml);

                if (this.LatestUses.ContainsKey(idUser))
                    continue;

                this.LatestUses.Add(idUser, latestUse);
            }
        }


        public string GetProperty(string name)
        {
            // Select the xml node that contains the property's value.
            XmlNode xmlNode = this.XmlDocument.SelectSingleNode(string.Format(
                "Properties/{0}",
                name
            ));

            // Check if a property with the value exists.
            if (xmlNode == null)
                return null;

            return xmlNode.InnerXml;
        }

        public void Save()
        {
            StringBuilder xmlString = new StringBuilder();

            foreach (Guid idUser in this.LatestUses.Keys)
            {
                xmlString.Append(string.Format(
                    "<LatestUse IdUser=\"{0}\">{1}</LatestUse>",
                    idUser,
                    this.LatestUses[idUser]
                ));
            }

            XmlDocument temp = new XmlDocument();
            temp.Load(this.FileName);

            XmlNode xmlNodeLatestUses = temp.SelectSingleNode("Report/Properties/LatestUses");

            if (xmlNodeLatestUses == null)
            {
                xmlString.Insert(0, "<LatestUses>");
                xmlString.Append("</LatestUses>");

                temp.SelectSingleNode("Report/Properties").InnerXml += xmlString.ToString();
            }
            else
            {
                xmlNodeLatestUses.InnerXml = xmlString.ToString();
            }

            temp.Save(this.FileName);
        }

        #endregion
    }
}
