using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace LinkAdmin.Classes
{
    public class StoredPortalCredentials
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the xml file that
        /// contains the user's stored portal credentials.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that
        /// contains the user's stored portal credentials.
        /// </summary>
        public XmlDocument Document { get; set; }

        public Dictionary<string, PortalCredentials> Credentials { get; set; }

        #endregion


        #region Constructor

        public StoredPortalCredentials()
        {
            this.Credentials = new Dictionary<string, PortalCredentials>();

            this.FileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "StoredPortalCredentials",
                Global.IdUser.Value + ".xml"
            );

            if (!Directory.Exists(Path.GetDirectoryName(this.FileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(this.FileName));

            if (!File.Exists(this.FileName))
                this.Save();

            this.Document = new XmlDocument();
            this.Document.Load(this.FileName);

            foreach (XmlNode xmlNode in this.Document.DocumentElement.SelectNodes("Credentials"))
            {
                PortalCredentials credentials = new PortalCredentials(this, xmlNode);

                if(this.Credentials.ContainsKey(credentials.Host))
                {
                    xmlNode.ParentNode.RemoveChild(xmlNode);
                    continue;
                }

                this.Credentials.Add(credentials.Host, credentials);
            }
        }

        #endregion


        #region Methods

        public void Save()
        {
            StringBuilder result = new StringBuilder();
            result.Append("<PortalCredentials>");

            foreach (string host in this.Credentials.Keys)
            {
                result.Append(string.Format(
                    "<Credentials Host=\"{0}\" Username=\"{1}\" Password=\"{2}\" />",
                    host,
                    this.Credentials[host].Username,
                    this.Credentials[host].Password
                ));
            }

            result.Append("</PortalCredentials>");

            File.WriteAllText(this.FileName, result.ToString());
            result.Clear();
        }

        #endregion
    }

    public class PortalCredentials
    {
        #region Properties

        public StoredPortalCredentials Owner { get; set; }

        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        #endregion


        #region Constructor

        public PortalCredentials(StoredPortalCredentials owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.Host = xmlNode.Attributes["Host"].Value;
            this.Username = xmlNode.Attributes["Username"].Value;
            this.Password = xmlNode.Attributes["Password"].Value;
        }

        public PortalCredentials(StoredPortalCredentials owner, string host, string username, string password)
        {
            this.Owner = owner;
            this.Host = host;
            this.Username = username;
            this.Password = password;

            this.Owner.Document.DocumentElement.InnerXml += string.Format(
                "<Credentials Host=\"{0}\" Username=\"{1}\" Password=\"{2}\" />",
                host,
                username,
                password
            );

            this.Owner.Save();
        }

        #endregion


        #region Methods

        #endregion
    }
}