using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using DataCore.Classes.StorageMethods;

namespace LinkOnline.Classes
{
    public class NewsManager
    {
        #region Properties

        /// <summary>
        /// Gets or sets the path to the news definition file.
        /// </summary>
        public string FileName { get; set; }
        public string Guid { get; set; }
        public string Heading { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public string Client { get; set; }
        public string UserId { get; set; }
        #endregion
        #region Constructor

        /// <summary>
        /// Creates a new instance of the news manager.
        /// </summary>
        public NewsManager()
        {
            if (Directory.Exists(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "News")))
            {
                if (!File.Exists(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "News", Global.Core.ClientName + ".xml")))
                {
                    string srcFileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data", "News", "News.xml");
                    File.Copy(srcFileName, Path.Combine(
                       HttpContext.Current.Request.PhysicalApplicationPath,
                       "Fileadmin",
                       "News",
                       Global.Core.ClientName + ".xml"), false);
                }
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(
                   HttpContext.Current.Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "News"));

                string srcFileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data", "News", "News.xml");
                File.Copy(srcFileName, Path.Combine(
                   HttpContext.Current.Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "News",
                   Global.Core.ClientName + ".xml"),false);

                //File.Create(Path.Combine(
                //   HttpContext.Current.Request.PhysicalApplicationPath,
                //   "Fileadmin",
                //   "News",
                //   Global.Core.ClientName + ".xml")).Close();
            }
            this.FileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "News", Global.Core.ClientName + ".xml");
        }

        #endregion
        #region Methods

        /// <summary>
        /// Checks if a client news exists.
        /// </summary>
        /// <param name="name">The name of the client.</param>
        public bool Exists(string name)
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the definition
            // file into the xml document.
            try
            {
                xmlDocument.Load(this.FileName);
            }
            catch
            {
                XmlNode rootNode = xmlDocument.CreateElement("ClientNews");
                xmlDocument.AppendChild(rootNode);
                xmlDocument.Save(FileName);
            }

            // Select the client's xml node.
            if (xmlDocument.DocumentElement != null)
            {
                XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                    "ClientNews/NewsList[@Client=\"{0}\"]",
                    name
                    ));

                // Check if the xml node exists.
                if (xmlNode == null)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets all the client news
        /// </summary>
        /// <param name="name">The name of the client.</param>
        public XmlNodeList GetNewsAll(string name)
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                // Load the contents of the definition file into the xml document.
                xmlDocument.Load(this.FileName);
            }
            catch (Exception ex)
            {
              
            }
            // Select the client's xml node.
            if (xmlDocument.DocumentElement == null) return null;

            //XmlNodeList xmlNodeList = xmlDocument.SelectNodes(string.Format(
            //    "NewsList[@Client=\"{0}\"]",
            //    name
            //    ));
            XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("NewsList[@Client=\"{0}\"]", name));
            if (root != null)
            {
                XmlNodeList xmlNodeList = root.ChildNodes;
                // Return the content of xml node's database attribute.
                return xmlNodeList;
            }
            return null;
        }


        /// <summary>
        /// Gets selected client news
        /// </summary>
        /// <param name="newsId">The newsId of the client.</param>
        public XmlNode GetNews(string newsId)
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                // Load the contents of the definition file into the xml document.
                xmlDocument.Load(this.FileName);
            }
            catch (XmlException ex)
            {
                return null;
            }
            // Select the client's xml node.
            if (xmlDocument.DocumentElement == null) return null;

            XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("NewsList[@Client=\"{0}\"]", Client));
            if (root != null)
            {

                XmlNodeList xmlNodeList = root.ChildNodes;

                XmlNode xmlNode = root.SelectSingleNode(string.Format("News[@Id=\"{0}\"]", newsId));
                // Return the content of xml node's database attribute.
                return xmlNode;
            }
            return null;
        }

        /// <summary>
        /// Modify specific client news
        /// </summary>
        /// <param name="newsId">The newsId of the client.</param>
        public void Modify(string newsId)
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                // Load the contents of the definition file into the xml document.
                xmlDocument.Load(this.FileName);
            }
            catch (XmlException ex)
            {

            }
            // Select the client's xml node.
            if (xmlDocument.DocumentElement != null)
            {
                XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("NewsList[@Client=\"{0}\"]", Client));
                if (root != null)
                {
                    XmlNode xmlNode = root.SelectSingleNode(string.Format("News[@Id=\"{0}\"]", newsId));
                    if (xmlNode != null)
                    {
                        if (xmlNode.Attributes != null)
                        {
                            xmlNode.Attributes["Heading"].Value = Heading;
                            xmlNode.Attributes["Description"].Value = Description;
                            xmlNode.Attributes["UserId"].Value = UserId;
                            xmlNode.Attributes["CreatedDate"].Value = CreatedDate;
                        }
                    }
                }
            }
            xmlDocument.Save(FileName);

        }
        ///<summary>
        /// Save the clients news
        /// </summary>
        public void Save()
        {
            if (!string.IsNullOrEmpty(Client))
            {
                XmlDocument xmlDoc = new XmlDocument();

                try
                {
                    xmlDoc.Load(this.FileName);
                }
                catch (XmlException ex)
                {
                    XmlNode rootNode = xmlDoc.CreateElement("ClientNews");
                    xmlDoc.AppendChild(rootNode);
                    xmlDoc.Save(FileName);
                }
                if (xmlDoc.DocumentElement != null)
                {
                    XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(string.Format(
                        "NewsList[@Client=\"{0}\"]",
                        Client
                        ));
                    if (node == null)
                    {

                        //XmlNode rootNode = xmlDoc.CreateElement("ClientNews");
                        //xmlDoc.AppendChild(rootNode);

                        XmlNode newsListNode = xmlDoc.CreateElement("NewsList");
                        XmlAttribute attribute = xmlDoc.CreateAttribute("Client");
                        attribute.Value = Client;
                        newsListNode.Attributes.Append(attribute);
                        xmlDoc.DocumentElement.AppendChild(newsListNode);

                        XmlNode newsNode = xmlDoc.CreateElement("News");

                        XmlAttribute attributeId = xmlDoc.CreateAttribute("Id");
                        attributeId.Value = Guid;
                        newsNode.Attributes.Append(attributeId);

                        XmlAttribute attributeHeading = xmlDoc.CreateAttribute("Heading");
                        attributeHeading.Value = Heading;
                        newsNode.Attributes.Append(attributeHeading);

                        XmlAttribute attributeDesc = xmlDoc.CreateAttribute("Description");
                        attributeDesc.Value = Description;
                        newsNode.Attributes.Append(attributeDesc);

                        XmlAttribute attributeUserId = xmlDoc.CreateAttribute("UserId");
                        attributeUserId.Value = UserId;
                        newsNode.Attributes.Append(attributeUserId);

                        XmlAttribute attributeDate = xmlDoc.CreateAttribute("CreatedDate");
                        attributeDate.Value = CreatedDate;
                        newsNode.Attributes.Append(attributeDate);

                        newsListNode.AppendChild(newsNode);
                    }
                    else
                    {
                        XmlNode newsNode = xmlDoc.CreateElement("News");

                        XmlAttribute attributeId = xmlDoc.CreateAttribute("Id");
                        attributeId.Value = Guid;
                        newsNode.Attributes.Append(attributeId);

                        XmlAttribute attributeHeading = xmlDoc.CreateAttribute("Heading");
                        attributeHeading.Value = Heading;
                        newsNode.Attributes.Append(attributeHeading);

                        XmlAttribute attributeDesc = xmlDoc.CreateAttribute("Description");
                        attributeDesc.Value = Description;
                        newsNode.Attributes.Append(attributeDesc);

                        XmlAttribute attributeUserId = xmlDoc.CreateAttribute("UserId");
                        attributeUserId.Value = UserId;
                        newsNode.Attributes.Append(attributeUserId);

                        XmlAttribute attributeDate = xmlDoc.CreateAttribute("CreatedDate");
                        attributeDate.Value = CreatedDate;
                        newsNode.Attributes.Append(attributeDate);

                        node.AppendChild(newsNode);
                    }
                }
                xmlDoc.Save(FileName);
            }
        }

        ///<summary>
        /// Delete the  specific client news
        /// <param name="newsId">The newsId of the client.</param>
        /// </summary>
        public void Delete(string newsId)
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                // Load the contents of the definition file into the xml document.
                xmlDocument.Load(this.FileName);
            }
            catch (XmlException ex)
            {

            }
            // Select the client's xml node.
            if (xmlDocument.DocumentElement != null)
            {
                XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("NewsList[@Client=\"{0}\"]", Client));
                if (root != null)
                {
                    XmlNode xmlNode = root.SelectSingleNode(string.Format("News[@Id=\"{0}\"]", newsId));
                    if (xmlNode != null)
                    {
                        XmlNode parentNode = xmlNode.ParentNode;
                        if (parentNode != null) parentNode.RemoveChild(xmlNode);
                    }
                }
            }
            xmlDocument.Save(FileName);
        }

        #endregion
    }

}