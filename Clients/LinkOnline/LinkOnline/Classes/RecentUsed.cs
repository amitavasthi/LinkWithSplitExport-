using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace LinkOnline.Classes
{
    public class RecentUsed
    {
        #region Properties
        /// <summary>
        /// Gets or sets the path to the news definition file.
        /// </summary>
        public string FileName { get; set; }
        public string GuId { get; set; }
        public string ClientName { get; set; }
        public string UserId { get; set; }
        public string itemName { get; set; }
        public string UsedDate { get; set; }
        public string itemType { get; set; }
        public string activeName { get; set; }
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public RecentUsed()
        {
            if (Directory.Exists(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home")))
            {
                if (!File.Exists(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", HttpContext.Current.Request.Url.Host.Split('.')[0] + "RecentUsedDetails.xml")))
                {
                    string srcFileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data", "RecentUsedDetails.xml");
                    File.Copy(srcFileName, Path.Combine(
                       HttpContext.Current.Request.PhysicalApplicationPath,
                       "Fileadmin",
                       "Home",
                       HttpContext.Current.Request.Url.Host.Split('.')[0] + "RecentUsedDetails.xml"), false);
                }
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(
                   HttpContext.Current.Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "Home"));
                string srcFileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data", "RecentUsedDetails.xml");
                File.Copy(srcFileName, Path.Combine(
                   HttpContext.Current.Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "Home",
                   HttpContext.Current.Request.Url.Host.Split('.')[0] + "RecentUsedDetails.xml"), false);
            }
            this.FileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", HttpContext.Current.Request.Url.Host.Split('.')[0] + "RecentUsedDetails.xml");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all the recent used item deatils for a user order by last used
        /// </summary>
        /// <param name="name">The name of the client.</param>
        public IOrderedEnumerable<XmlNode> GetItemDetails()
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

            //var sortedItems = xmlDocument.GetElementsByTagName("RecentItem").OfType<XmlElement>().
            //    OrderBy(item => DateTime.Parse(item.GetAttribute("useddate")));


            // Select the client's xml node.
            if (xmlDocument.DocumentElement == null) return null;

            XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("UserDetails[@User=\"{0}\"]", Global.User.Id.ToString()));
            if (root != null)
            {
                XmlNode xmlRootNodes = root.SelectSingleNode("RecentItems");

                //var itemList = xmlRootNodes.SelectNodes(string.Format("RecentItem[@type=\"{0}\"]", type));

                // Return the xmlnode with descending order.
                IOrderedEnumerable<XmlNode> itemList = xmlRootNodes.Cast<XmlNode>()
                    .OrderByDescending(node => node.Attributes["useddate"].Value);

                return itemList;
            }
            return null;
        }


        /// <summary>
        /// Gets all the used item deatils for a user
        /// </summary>
        /// <param name="name">The name of the client.</param>
        public IOrderedEnumerable<XmlNode> GetItemDetails(string type)
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

            //var sortedItems = xmlDocument.GetElementsByTagName("RecentItem").OfType<XmlElement>().
            //    OrderBy(item => DateTime.Parse(item.GetAttribute("useddate")));


            // Select the client's xml node.
            if (xmlDocument.DocumentElement == null) return null;

            XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("UserDetails[@User=\"{0}\"]", Global.User.Id.ToString()));
            if (root != null)
            {
                XmlNode xmlRootNodes = root.SelectSingleNode("RecentItems");

                //var itemList = xmlRootNodes.SelectNodes(string.Format("RecentItem[@type=\"{0}\"]", type));

                // Return the xmlnode with descending order.
                IOrderedEnumerable<XmlNode> itemList = xmlRootNodes.SelectNodes(string.Format("RecentItem[@type=\"{0}\"]", type))
                    .Cast<XmlNode>()
                    .OrderByDescending(node => node.Attributes["useddate"].Value);

                return itemList;
            }
            return null;
        }

        /// <summary>
        /// Get Chart Deatils by Name Details
        /// </summary>
        public XmlNode GetChartDetailsbyNameFromActual(string chartName)
        {
            string fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", Global.Core.ClientName + "HomePage.xml");

            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(fileName);
            }
            catch (XmlException ex)
            {
                return null;
            }
            // Select the client's xml node.
            if (xmlDocument.DocumentElement == null) return null;

            XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("ClientDetails[@Client=\"{0}\"]", Global.Core.ClientName));
            if (root != null)
            {
                XmlNodeList xmlNodeList = root.ChildNodes;
                XmlNode xmlNode = root.SelectSingleNode("CustomCharts");
                var chartDetailsByName = xmlNode.SelectSingleNode(string.Format("CustomChart[@id=\"{0}\"]", chartName));
                return chartDetailsByName;
            }
            return null;
        }


        /// <summary>
        /// Get Chart Details by name
        /// </summary>
        public bool CheckDetailsExistsRecent(string name, string type)
        {
            bool exist = true;

            string fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", Global.Core.ClientName + "RecentUsedDetails.xml");

            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(fileName);

            XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("UserDetails[@User=\"{0}\"]", Global.User.Id));

            if (root != null)
            {
                XmlNodeList xmlNodeList = root.ChildNodes;
                XmlNode xmlNode = root.SelectSingleNode("RecentItems");
                XmlNode typeXMLNode = xmlNode.SelectSingleNode(string.Format("RecentItem[@ name=\"{0}\"]", name));
                if (typeXMLNode == null)
                {
                    exist = false;
                }
            }
            return exist;
        }
        /// <summary>
        /// Get Chart Details by name
        /// </summary>
        public bool CheckReportsExistsRecent(string activereport, string type)
        {
            bool exist = true;

            string fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", Global.Core.ClientName + "RecentUsedDetails.xml");

            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(fileName);

            XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("UserDetails[@User=\"{0}\"]", Global.User.Id));

            if (root != null)
            {
                XmlNodeList xmlNodeList = root.ChildNodes;
                XmlNode xmlNode = root.SelectSingleNode("RecentItems");
                XmlNode typeXMLNode = xmlNode.SelectSingleNode(string.Format("RecentItem[@ activename=\"{0}\"]", activereport));
                if (typeXMLNode == null)
                {
                    exist = false;
                }
            }
            return exist;
        }


        ///<summary>
        ///Save All the available reports and Dashborads in the firsttime
        ///</summary>
        ///<summary>
        /// Creates the clients report details XML first time
        /// </summary>
        public void InitialCreation()
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(this.FileName);
            }
            catch (XmlException ex)
            {
                XmlNode rootNode = xmlDoc.CreateElement("RecentUsedItems");
                xmlDoc.AppendChild(rootNode);
                xmlDoc.Save(FileName);
            }
            if (xmlDoc.DocumentElement != null)
            {
                XmlNode root = xmlDoc.DocumentElement.SelectSingleNode(string.Format(
                    "UserDetails[@User=\"{0}\"]",
                    Global.User.Id
                    ));
                if (root == null)
                {
                    XmlNode UserDetailsNode = xmlDoc.CreateElement("UserDetails");
                    XmlAttribute attribute = xmlDoc.CreateAttribute("User");
                    attribute.Value = Global.User.Id.ToString();
                    UserDetailsNode.Attributes.Append(attribute);
                    xmlDoc.DocumentElement.AppendChild(UserDetailsNode);


                    XmlNode recentItemParentNode = xmlDoc.CreateElement("RecentItems");
                    UserDetailsNode.AppendChild(recentItemParentNode);

                    XmlNode recentNode = xmlDoc.CreateElement("RecentItem");
                    XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
                    idAttribute.Value = GuId;
                    recentNode.Attributes.Append(idAttribute);

                    XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
                    nameAttribute.Value = itemName;
                    recentNode.Attributes.Append(nameAttribute);

                    if (itemType == "LinkReporter")
                    {
                        XmlAttribute activeNameAttribute = xmlDoc.CreateAttribute("activename");
                        activeNameAttribute.Value = activeName;
                        recentNode.Attributes.Append(activeNameAttribute);
                    }


                    XmlAttribute typeAttribute = xmlDoc.CreateAttribute("type");
                    typeAttribute.Value = itemType;
                    recentNode.Attributes.Append(typeAttribute);

                    XmlAttribute dateAttribute = xmlDoc.CreateAttribute("useddate");
                    dateAttribute.Value = UsedDate;
                    recentNode.Attributes.Append(dateAttribute);

                    recentItemParentNode.AppendChild(recentNode);
                    UserDetailsNode.AppendChild(recentItemParentNode);
                }
                else
                {
                    XmlNode node = root.SelectSingleNode("RecentItems");
                    if (node == null)
                    {
                        XmlNode recentItemParentNode = xmlDoc.CreateElement("RecentItems");

                        XmlNode recentNode = xmlDoc.CreateElement("RecentItem");

                        XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
                        idAttribute.Value = GuId;
                        recentNode.Attributes.Append(idAttribute);

                        XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
                        nameAttribute.Value = itemName;
                        recentNode.Attributes.Append(nameAttribute);

                        if (itemType == "LinkReporter")
                        {
                            XmlAttribute activeNameAttribute = xmlDoc.CreateAttribute("activename");
                            activeNameAttribute.Value = activeName;
                            recentNode.Attributes.Append(activeNameAttribute);
                        }


                        XmlAttribute typeAttribute = xmlDoc.CreateAttribute("type");
                        typeAttribute.Value = itemType;
                        recentNode.Attributes.Append(typeAttribute);


                        XmlAttribute dateAttribute = xmlDoc.CreateAttribute("useddate");
                        dateAttribute.Value = UsedDate;
                        recentNode.Attributes.Append(dateAttribute);

                        recentItemParentNode.AppendChild(recentNode);
                        root.AppendChild(recentItemParentNode);
                    }
                    else
                    {
                        XmlNode recentNode = xmlDoc.CreateElement("RecentItem");
                        XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
                        idAttribute.Value = GuId;
                        recentNode.Attributes.Append(idAttribute);

                        XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
                        nameAttribute.Value = itemName;
                        recentNode.Attributes.Append(nameAttribute);

                        if (itemType == "LinkReporter")
                        {
                            XmlAttribute activeNameAttribute = xmlDoc.CreateAttribute("activename");
                            activeNameAttribute.Value = activeName;
                            recentNode.Attributes.Append(activeNameAttribute);
                        }


                        XmlAttribute typeAttribute = xmlDoc.CreateAttribute("type");
                        typeAttribute.Value = itemType;
                        recentNode.Attributes.Append(typeAttribute);

                        XmlAttribute dateAttribute = xmlDoc.CreateAttribute("useddate");
                        dateAttribute.Value = UsedDate;
                        recentNode.Attributes.Append(dateAttribute);
                        node.AppendChild(recentNode);
                    }
                }
            }
            xmlDoc.Save(FileName);
        }

        ///<summary>
        ///Remove a non existing Item from Recent Used 
        ///</summary>
        public void RevomeItem(string itemName, string type)
        {
            if (!string.IsNullOrEmpty(Global.Core.ClientName))
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(this.FileName);
                }
                catch (XmlException ex)
                {
                    XmlNode rootNode = xmlDoc.CreateElement("RecentUsedItems");
                    xmlDoc.AppendChild(rootNode);
                    xmlDoc.Save(FileName);
                }
                if (xmlDoc.DocumentElement != null)
                {
                    XmlNode root = xmlDoc.DocumentElement.SelectSingleNode(string.Format(
                        "UserDetails[@User=\"{0}\"]",
                        Global.User.Id
                        ));

                    if (root != null)
                    {
                        XmlNode xmlRootNodes = root.SelectSingleNode("RecentItems");
                        if (xmlRootNodes != null)
                        {
                            for (int i = 0; i < xmlRootNodes.ChildNodes.Count; i++)
                            {
                                if (xmlRootNodes.ChildNodes.Item(i).Attributes["type"].Value == type)
                                {
                                    if (type == "LinkReporter")
                                    {
                                        if (xmlRootNodes.ChildNodes.Item(i).Attributes["activename"].Value == itemName)
                                        {
                                            xmlRootNodes.RemoveChild(xmlRootNodes.ChildNodes.Item(i));
                                        }
                                    }
                                    else
                                    {
                                        if (xmlRootNodes.ChildNodes.Item(i).Attributes["name"].Value == itemName)
                                        {
                                            xmlRootNodes.RemoveChild(xmlRootNodes.ChildNodes.Item(i));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                xmlDoc.Save(FileName);
            }
        }


        ///<summary>
        /// Modify and Save the clients report details XML
        /// </summary>
        public void CreateRecentUsed()
        {
            if (!string.IsNullOrEmpty(Global.Core.ClientName))
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(this.FileName);
                }
                catch (XmlException ex)
                {
                    XmlNode rootNode = xmlDoc.CreateElement("RecentUsedItems");
                    xmlDoc.AppendChild(rootNode);
                    xmlDoc.Save(FileName);
                }
                if (xmlDoc.DocumentElement != null)
                {
                    XmlNode root = xmlDoc.DocumentElement.SelectSingleNode(string.Format(
                        "UserDetails[@User=\"{0}\"]",
                        Global.User.Id
                        ));

                    if (root == null)
                    {
                        XmlNode UserDetailsNode = xmlDoc.CreateElement("UserDetails");
                        XmlAttribute attribute = xmlDoc.CreateAttribute("User");
                        attribute.Value = Global.User.Id.ToString();
                        UserDetailsNode.Attributes.Append(attribute);
                        xmlDoc.DocumentElement.AppendChild(UserDetailsNode);


                        XmlNode recentItemParentNode = xmlDoc.CreateElement("RecentItems");
                        UserDetailsNode.AppendChild(recentItemParentNode);

                        XmlNode recentNode = xmlDoc.CreateElement("RecentItem");
                        XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
                        idAttribute.Value = GuId;
                        recentNode.Attributes.Append(idAttribute);

                        XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
                        nameAttribute.Value = itemName;
                        recentNode.Attributes.Append(nameAttribute);

                        if (itemType == "LinkReporter")
                        {
                            XmlAttribute activeNameAttribute = xmlDoc.CreateAttribute("activename");
                            activeNameAttribute.Value = activeName;
                            recentNode.Attributes.Append(activeNameAttribute);
                        }


                        XmlAttribute typeAttribute = xmlDoc.CreateAttribute("type");
                        typeAttribute.Value = itemType;
                        recentNode.Attributes.Append(typeAttribute);

                        XmlAttribute dateAttribute = xmlDoc.CreateAttribute("useddate");
                        dateAttribute.Value = UsedDate;
                        recentNode.Attributes.Append(dateAttribute);

                        recentItemParentNode.AppendChild(recentNode);
                        UserDetailsNode.AppendChild(recentItemParentNode);

                    }
                    else
                    {
                        XmlNode node = root.SelectSingleNode("RecentItems");
                        if (node == null)
                        {
                            XmlNode recentItemParentNode = xmlDoc.CreateElement("RecentItems");

                            XmlNode recentNode = xmlDoc.CreateElement("RecentItem");

                            XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
                            idAttribute.Value = GuId;
                            recentNode.Attributes.Append(idAttribute);

                            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
                            nameAttribute.Value = itemName;
                            recentNode.Attributes.Append(nameAttribute);

                            if (itemType == "LinkReporter")
                            {
                                XmlAttribute activeNameAttribute = xmlDoc.CreateAttribute("activename");
                                activeNameAttribute.Value = activeName;
                                recentNode.Attributes.Append(activeNameAttribute);
                            }


                            XmlAttribute typeAttribute = xmlDoc.CreateAttribute("type");
                            typeAttribute.Value = itemType;
                            recentNode.Attributes.Append(typeAttribute);


                            XmlAttribute dateAttribute = xmlDoc.CreateAttribute("useddate");
                            dateAttribute.Value = UsedDate;
                            recentNode.Attributes.Append(dateAttribute);

                            recentItemParentNode.AppendChild(recentNode);
                            root.AppendChild(recentItemParentNode);
                        }
                        else
                        {
                            XmlNode xmlRootNodes = root.SelectSingleNode("RecentItems");
                            if (xmlRootNodes != null)
                            {
                                for (int i = 0; i < xmlRootNodes.ChildNodes.Count; i++)
                                {
                                    if (xmlRootNodes.ChildNodes.Item(i).Attributes["type"].Value == "LinkReporter")
                                    {
                                        if (xmlRootNodes.ChildNodes.Item(i).Attributes["activename"].Value == activeName)
                                        {
                                            xmlRootNodes.RemoveChild(xmlRootNodes.ChildNodes.Item(i));
                                        }
                                    }
                                    else if (xmlRootNodes.ChildNodes.Item(i).Attributes["type"].Value == "CustomChart")
                                    {
                                        if (xmlRootNodes.ChildNodes.Item(i).Attributes["name"].Value == itemName)
                                        {
                                            xmlRootNodes.RemoveChild(xmlRootNodes.ChildNodes.Item(i));
                                        }
                                    }//the Bi parts comes here
                                    else if (xmlRootNodes.ChildNodes.Item(i).Attributes["type"].Value == "LinkBiReport")
                                    {
                                        if (xmlRootNodes.ChildNodes.Item(i).Attributes["name"].Value == itemName)
                                        {
                                            xmlRootNodes.RemoveChild(xmlRootNodes.ChildNodes.Item(i));
                                        }
                                    }
                                }
                            }

                            XmlNode recentNode = xmlDoc.CreateElement("RecentItem");

                            XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
                            idAttribute.Value = GuId;
                            recentNode.Attributes.Append(idAttribute);

                            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
                            nameAttribute.Value = itemName;
                            recentNode.Attributes.Append(nameAttribute);

                            if (itemType == "LinkReporter")
                            {
                                XmlAttribute activeNameAttribute = xmlDoc.CreateAttribute("activename");
                                activeNameAttribute.Value = activeName;
                                recentNode.Attributes.Append(activeNameAttribute);
                            }


                            XmlAttribute typeAttribute = xmlDoc.CreateAttribute("type");
                            typeAttribute.Value = itemType;
                            recentNode.Attributes.Append(typeAttribute);


                            XmlAttribute dateAttribute = xmlDoc.CreateAttribute("useddate");
                            dateAttribute.Value = UsedDate;
                            recentNode.Attributes.Append(dateAttribute);
                            node.AppendChild(recentNode);
                        }
                    }
                }
                xmlDoc.Save(FileName);
            }
        }

        #endregion

    }
}