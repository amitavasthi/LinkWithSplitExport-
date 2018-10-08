using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Homescreen1.Classes
{
    public class DashboardItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the
        /// dashboard item's definition file.
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that contains
        /// the dashboard item's definition.
        /// </summary>
        private XmlDocument Document { get; set; }

        /// <summary>
        /// Gets or sets the id of the dashboard item.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the dashboard item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the source to the dashboard item.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the id of the user
        /// that created the dashboard item.
        /// </summary>
        public Guid IdUser { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the dashboard item.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of the latest
        /// access times of the client's users.
        /// </summary>
        public Dictionary<Guid, DateTime> LatestUses { get; set; }

        /// <summary>
        /// Gets or sets the icon location.
        /// </summary>
        public string Icon { get; set; }

        #endregion


        #region Constructor

        public DashboardItem(string fileName)
        {
            this.FileName = fileName;

            // Parse the dashboard item's definition file.
            this.Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            this.LatestUses = new Dictionary<Guid, DateTime>();

            this.Id = Guid.Parse(new FileInfo(this.FileName).Name.Replace(".xml", ""));

            this.Document = new XmlDocument();
            this.Document.Load(this.FileName);

            this.Source = this.Document.DocumentElement.Attributes["Source"].Value;
            this.Name = this.Document.DocumentElement.Attributes["Name"].Value;
            this.IdUser = Guid.Parse(this.Document.DocumentElement.Attributes["IdUser"].Value);
            this.CreationDate = DateTime.Parse(this.Document.DocumentElement.Attributes["CreationDate"].Value);
            this.Icon = "/Images/Icons/Charts/customcharts.png";

            if (this.Document.DocumentElement.Attributes["Icon"] != null)
                this.Icon = this.Document.DocumentElement.Attributes["Icon"].Value;

            // Get all xml nodes that define a latest use.
            XmlNodeList xmlNodes = this.Document.SelectNodes("DashboardItem/LatestUses/LatestUse");

            foreach (XmlNode xmlNode in xmlNodes)
            {
                Guid idUser = Guid.Parse(xmlNode.Attributes["IdUser"].Value);
                DateTime latestUse = DateTime.Parse(xmlNode.Attributes["Timestamp"].Value);

                this.LatestUses.Add(idUser, latestUse);
            }
        }

        public void Save()
        {
            // Create a new string builder to
            // build the latest uses xml part.
            StringBuilder xmlBuilder = new StringBuilder();

            foreach (Guid idUser in this.LatestUses.Keys)
            {
                xmlBuilder.Append(string.Format(
                    "<LatestUse IdUser=\"{0}\" Timestamp=\"{1}\" />",
                    idUser,
                    this.LatestUses[idUser].ToString()
                ));
            }

            XmlNode xmlNode = this.Document.SelectSingleNode("DashboardItem/LatestUses");

            if (xmlNode == null)
            {
                xmlBuilder.Insert(0, "<LatestUses>");
                xmlBuilder.Append("</LatestUses>");

                this.Document.DocumentElement.InnerXml += xmlBuilder.ToString();
            }
            else
            {
                xmlNode.InnerXml = xmlBuilder.ToString();
            }

            this.Document.DocumentElement.Attributes["Name"].Value = this.Name;
            this.Document.DocumentElement.Attributes["Source"].Value = this.Source;

            this.Document.Save(this.FileName);
        }

        #endregion
    }
}
