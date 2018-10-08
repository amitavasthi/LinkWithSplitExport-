using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace ApplicationUtilities.Classes
{
    public class ClientManager
    {
        #region Properties

        /// <summary>
        /// Gets or sets the path to the client definition file.
        /// </summary>
        public string FileName { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the client manager.
        /// </summary>
        public ClientManager()
        {
            // Build the path to the client definition file.
            this.FileName = Path.Combine(
                HttpContext.Current != null ? HttpContext.Current.Request.PhysicalApplicationPath :
                HttpRuntime.AppDomainAppPath,
                "App_Data",
                "Clients.xml"
            );
        }

        /// <summary>
        /// Creates a new instance of the client manager.
        /// </summary>
        /// <param name="fileName">The full path to the client definition xml file.</param>
        public ClientManager(string fileName)
        {
            this.FileName = fileName;
        }

        #endregion


        #region Methods

        public void IncreaseCaseDataVersion(string clientName)
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the definition
            // file into the xml document.
            xmlDocument.Load(this.FileName);

            // Select the xml node that defines the client.
            XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Client[@Name=\"{0}\"]",
                clientName
            ));

            int caseDataVersion = 0;

            if (xmlNode.Attributes["CaseDataVersion"] == null)
            {
                xmlNode.AddAttribute("CaseDataVersion", 0);
            }

            caseDataVersion = int.Parse(xmlNode.Attributes["CaseDataVersion"].Value);

            caseDataVersion++;

            xmlNode.Attributes["CaseDataVersion"].Value = caseDataVersion.ToString();

            xmlDocument.Save(this.FileName);
        }

        /// <summary>
        /// Checks if a client exists.
        /// </summary>
        /// <param name="name">The name of the client.</param>
        public bool Exists(string name)
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the definition
            // file into the xml document.
            xmlDocument.Load(this.FileName);

            // Select the client's xml node.
            XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Client[@Name=\"{0}\"]",
                name
            ));

            // Check if the xml node exists.
            if (xmlNode == null)
                return false;

            return true;
        }

        /// <summary>
        /// Gets all clients.
        /// </summary>
        public Client[] Get()
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the definition
            // file into the xml document.
            xmlDocument.Load(this.FileName);

            Client[] result = new Client[xmlDocument.DocumentElement.ChildNodes.Count];

            // Run through all client xml nodes.
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Client(this, xmlDocument.DocumentElement.ChildNodes[i]);
            }

            return result;
        }

        /// <summary>
        /// Gets a single client by it's name.
        /// </summary>
        /// <param name="name">The name of the client.</param>
        public Client GetSingle(string name)
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the definition
            // file into the xml document.
            xmlDocument.Load(this.FileName);

            // Select the client's xml node.
            XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Client[@Name=\"{0}\"]",
                name
            ));

            // Check if the xml node exists.
            if (xmlNode == null)
                return null;

            return new Client(this, xmlNode);
        }

        /// <summary>
        /// Gets the name of a client's database.
        /// </summary>
        /// <param name="name">The name of the client.</param>
        public string GetDatabaseName(string name)
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the definition
            // file into the xml document.
            xmlDocument.Load(this.FileName);

            // Select the client's xml node.
            XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Client[@Name=\"{0}\"]",
                name
            ));

            // Check if the xml node exists.
            if (xmlNode == null)
                return null;

            // Return the content of xml node's database attribute.
            return xmlNode.Attributes["Database"].Value;
        }


        internal void Update(XmlNode xmlNode)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(this.FileName);


            XmlNode xmlNodeCurrent = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Client[@Name=\"{0}\"]",
                xmlNode.Attributes["Name"].Value
            ));

            if (xmlNodeCurrent != null)
            {
                foreach (XmlAttribute attribute in xmlNode.Attributes)
                {
                    xmlNodeCurrent.Attributes[attribute.Name].Value = attribute.Value;
                }
            }

            xmlDocument.Save(this.FileName);
        }

        public void Append(
            string name,
            string database,
            string host,
            string color1,
            string color2,
            string createdDate,
            string synch
        )
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(this.FileName);

            xmlDocument.DocumentElement.InnerXml += string.Format(
                "<Client Name=\"{0}\" Host=\"{1}\" Database=\"{2}\" Color1=\"{3}\" Color2=\"{4}\" CreatedDate=\"{5}\" {6}></Client>",
                name,
                host,
                database,
                color1,
                color2,
                createdDate,
                string.IsNullOrEmpty(synch) ? "" : "Synch=\"" + synch + "\""
            );

            xmlDocument.Save(this.FileName);
        }

        public void Remove(string clientName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(this.FileName);

            XmlNode xmlNodeClient = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Client[@Name=\"{0}\"]",
                clientName
            ));

            if (xmlNodeClient == null)
                return;

            xmlNodeClient.ParentNode.RemoveChild(xmlNodeClient);

            xmlDocument.Save(this.FileName);
        }


        #endregion
    }

    public class Client
    {
        #region Properties

        public XmlNode XmlNode { get; private set; }

        /// <summary>
        /// Gets or sets the owning client manager.
        /// </summary>
        public ClientManager Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValue("Name").ToString();
            }
            set
            {
                this.SetValue("Name", value);
            }
        }

        /// <summary>
        /// Gets or sets the host of the client.
        /// </summary>
        public string Host
        {
            get
            {
                return this.GetValue("Host").ToString();
            }
            set
            {
                this.SetValue("Host", value);
            }
        }

        /// <summary>
        /// Gets or sets the database of the client.
        /// </summary>
        public string Database
        {
            get
            {
                return this.GetValue("Database").ToString();
            }
            set
            {
                this.SetValue("Database", value);
            }
        }

        /// <summary>
        /// Gets or sets the first color of the client.
        /// </summary>
        public string Color1
        {
            get
            {
                return this.GetValue("Color1").ToString();
            }
            set
            {
                this.SetValue("Color1", value);
            }
        }

        /// <summary>
        /// Gets or sets the second color of the client.
        /// </summary>
        public string Color2
        {
            get
            {
                return this.GetValue("Color2").ToString();
            }
            set
            {
                this.SetValue("Color2", value);
            }
        }

        /// <summary>
        /// Gets or sets the third color of the client.
        /// </summary>
        public string Color3
        {
            get
            {
                return this.GetValue("Color3").ToString();
            }
            set
            {
                this.SetValue("Color3", value);
            }
        }

        /// <summary>
        /// Gets or sets the third color of the client.
        /// </summary>
        public string[] SynchServers
        {
            get
            {
                if (this.XmlNode.Attributes["Synch"] == null)
                    return new string[0];

                return this.GetValue("Synch").ToString().Split(',').Select(x => x.Trim()).ToArray();
            }
            set
            {
                this.SetValue("Synch", string.Join(",", value));
            }
        }

        public CaseDataLocation CaseDataLocation
        {
            get
            {
                if (this.XmlNode.Attributes["CaseDataLocation"] == null)
                    return CaseDataLocation.Sql;

                return (CaseDataLocation)Enum.Parse(
                    typeof(CaseDataLocation),
                    this.GetValue("CaseDataLocation").ToString()
                );
            }
            set
            {
                this.SetValue("CaseDataLocation", value);
            }
        }

        public int CaseDataVersion
        {
            get
            {
                if (this.XmlNode.Attributes["CaseDataVersion"] == null)
                    this.SetValue("CaseDataVersion", 0);

                return int.Parse(
                    (string)this.GetValue("CaseDataVersion")
                );
            }
            set
            {
                this.SetValue("CaseDataLocation", value);
            }
        }

        #endregion


        #region Constructor

        public Client(ClientManager owner, XmlNode xmlNode)
            : base()
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;

            if (this.XmlNode == null)
            {
                XmlDocument document = new XmlDocument();
                document.Load(this.Owner.FileName);

                this.XmlNode = document.CreateElement("Client");

                string[] attributes = new string[]{
                    "Name",
                    "Host",
                    "Database",
                    "Color1",
                    "Color2",
                    "Color3"
                };

                foreach (string attribute in attributes)
                {
                    XmlAttribute xmlAttribute = document.CreateAttribute(attribute);
                    this.XmlNode.Attributes.Append(xmlAttribute);
                }

                document.DocumentElement.AppendChild(this.XmlNode);

                document.Save(this.Owner.FileName);
            }
        }

        public Client(ClientManager owner, string name)
            : base()
        {
            this.Owner = owner;

            XmlDocument document = new XmlDocument();
            document.Load(this.Owner.FileName);

            this.XmlNode = document.CreateElement("Client");

            string[] attributes = new string[]{
                    "Name",
                    "Host",
                    "Database",
                    "Color1",
                    "Color2",
                    "Color3"
                };

            foreach (string attribute in attributes)
            {
                XmlAttribute xmlAttribute = document.CreateAttribute(attribute);
                this.XmlNode.Attributes.Append(xmlAttribute);
            }

            this.XmlNode.Attributes["Name"].Value = name;

            document.DocumentElement.AppendChild(this.XmlNode);

            document.Save(this.Owner.FileName);
        }

        #endregion


        #region Methods

        public object GetValue(string name)
        {
            return this.XmlNode.Attributes[name].Value;
        }

        public void SetValue(string name, object value)
        {
            if (this.XmlNode.Attributes[name] == null)
                this.XmlNode.AddAttribute(name, value.ToString());
            else
                this.XmlNode.Attributes[name].Value = value.ToString();
        }


        public void Save()
        {
            this.Owner.Update(this.XmlNode);
        }

        #endregion
    }

    public enum CaseDataLocation
    {
        Sql,
        File
    }
}