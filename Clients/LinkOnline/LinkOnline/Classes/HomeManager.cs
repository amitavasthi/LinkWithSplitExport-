using DatabaseCore.Items;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

using System.Linq;

using System.Web;

using System.Web.UI;
using System.Xml;

using WebUtilities.Controls;

namespace LinkOnline.Classes
{
    public class HomeManager
    {
        #region Properties

        /// <summary>
        /// Gets or sets the path to the news definition file.
        /// </summary>
        public string FileName { get; set; }
        public string GuId { get; set; }
        public string RssFeedUrl { get; set; }
        public string ClientName { get; set; }
        public string NewsFile { get; set; }
        public string CreatedDate { get; set; }
        public string SavedReports { get; set; }
        public string ChartsURL { get; set; }
        public string UserId { get; set; }
        public string ChartHeading { get; set; }
        #endregion
        #region Constructor

        /// <summary>
        /// Creates a new instance of the news manager.
        /// </summary>
        public HomeManager()
        {
            // Build the path to the client definition file.
            //this.FileName = Path.Combine(
            //    HttpContext.Current.Request.PhysicalApplicationPath,
            //    "App_Data",
            //    "HomePage.xml"
            //);
            if (Directory.Exists(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home")))
            {
                if (!File.Exists(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", HttpContext.Current.Request.Url.Host.Split('.')[0] + "HomePage.xml")))
                {
                    string srcFileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data", "HomePage.xml");
                    File.Copy(srcFileName, Path.Combine(
                       HttpContext.Current.Request.PhysicalApplicationPath,
                       "Fileadmin",
                       "Home",
                       HttpContext.Current.Request.Url.Host.Split('.')[0] + "HomePage.xml"), false);
                }
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(
                   HttpContext.Current.Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "Home"));
                string srcFileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data", "HomePage.xml");
                File.Copy(srcFileName, Path.Combine(
                   HttpContext.Current.Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "Home",
                   HttpContext.Current.Request.Url.Host.Split('.')[0] + "HomePage.xml"), false);
            }
            this.FileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", HttpContext.Current.Request.Url.Host.Split('.')[0] + "HomePage.xml");
        }

        #endregion
        #region Methods

        /// <summary>
        /// Checks if a client  exists.
        /// </summary>
        /// <param name="name">The name of the client.</param>
        public bool Exists(string clientName)
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the definition
            // file into the xml document.
            try
            {
                xmlDocument.Load(this.FileName);
            }
            catch (Exception)
            {
                XmlNode rootNode = xmlDocument.CreateElement("HomePage");
                xmlDocument.AppendChild(rootNode);
                xmlDocument.Save(FileName);
            }

            // Select the client's xml node.
            if (xmlDocument.DocumentElement != null)
            {
                XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(string.Format("ClientDetails[@Client=\"{0}\"]", clientName.Trim()));

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
        public XmlNodeList GetClientDetails(string clientName)
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

            //XmlNodeList xmlNodeList = xmlDocument.SelectNodes(string.Format(
            //    "NewsList[@Client=\"{0}\"]",
            //    name
            //    ));
            XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("ClientDetails[@Client=\"{0}\"]", clientName));
            if (root != null)
            {
                XmlNodeList xmlNodeList = root.ChildNodes;
                // Return the content of xml node's database attribute.
                return xmlNodeList;
            }
            return null;
        }

        /// <summary>
        /// Modify specific client home page details
        /// </summary>
        /// <param name="newsId">The clientName</param>
        public void Modify()
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
                XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("ClientDetails[@Client=\"{0}\"]", ClientName));
                if (root != null)
                {
                    XmlNodeList xmlNodeList = root.ChildNodes;

                    XmlNode xmlRssFeedNode = root.SelectSingleNode("RSSFeed");
                    if (xmlRssFeedNode != null)
                    {
                        if (xmlRssFeedNode.Attributes["url"] != null)
                        {
                            xmlRssFeedNode.Attributes["url"].Value = RssFeedUrl;
                        }
                        else
                        {
                            xmlRssFeedNode.AddAttribute("url", RssFeedUrl);
                        }
                    }
                    else
                    {
                        XmlNode rssNode = xmlDocument.CreateElement("RSSFeed");
                        XmlAttribute rssAttribute = xmlDocument.CreateAttribute("url");
                        rssAttribute.Value = RssFeedUrl;
                        rssNode.Attributes.Append(rssAttribute);
                        root.AppendChild(rssNode);
                    }
                    XmlNode xmlNewsNode = root.SelectSingleNode("News");
                    if (xmlNewsNode != null)
                    {
                        if (xmlNewsNode.Attributes["details"] != null)
                        {
                            xmlNewsNode.Attributes["details"].Value = NewsFile;
                        }
                        else
                        {
                            xmlNewsNode.AddAttribute("details", NewsFile);
                        }
                    }
                    else
                    {
                        XmlNode newsNode = xmlDocument.CreateElement("News");
                        XmlAttribute newsAttribute = xmlDocument.CreateAttribute("details");
                        newsAttribute.Value = NewsFile;
                        newsNode.Attributes.Append(newsAttribute);
                        root.AppendChild(newsNode);
                    }
                    XmlNode xmlReportsNode = root.SelectSingleNode("SavedReports");
                    if (xmlReportsNode != null)
                    {
                        if (xmlReportsNode.Attributes["details"] != null)
                        {
                            xmlReportsNode.Attributes["details"].Value = SavedReports;
                        }
                        else
                        {
                            xmlReportsNode.AddAttribute("details", SavedReports);
                        }
                    }
                    else
                    {
                        XmlNode reportNode = xmlDocument.CreateElement("SavedReports");
                        XmlAttribute reportAttribute = xmlDocument.CreateAttribute("details");
                        reportAttribute.Value = SavedReports;
                        reportNode.Attributes.Append(reportAttribute);
                        root.AppendChild(reportNode);
                    }
                    XmlNode xmlChartsNode = root.SelectSingleNode("CustomCharts");
                    if (xmlChartsNode != null)
                    {
                        if (xmlChartsNode.Attributes["id"] != null)
                        {
                            xmlChartsNode.Attributes["id"].Value = GuId;
                        }
                        else
                        {
                            xmlChartsNode.AddAttribute("id", GuId);
                        }
                        if (xmlChartsNode.Attributes["heading"] != null)
                        {
                            xmlChartsNode.Attributes["heading"].Value = ChartHeading;
                        }
                        else
                        {
                            xmlChartsNode.AddAttribute("heading", ChartHeading);
                        }
                        if (xmlChartsNode.Attributes["url"] != null)
                        {
                            xmlChartsNode.Attributes["url"].Value = ChartsURL;
                        }
                        else
                        {
                            xmlChartsNode.AddAttribute("url", ChartsURL);
                        }
                    }
                    else
                    {
                        XmlNode chartsNode = xmlDocument.CreateElement("CustomCharts");
                        XmlAttribute idAttribute = xmlDocument.CreateAttribute("id");
                        idAttribute.Value = GuId;
                        chartsNode.Attributes.Append(idAttribute);

                        XmlAttribute chartHeadingAttribute = xmlDocument.CreateAttribute("heading");
                        chartHeadingAttribute.Value = ChartHeading;
                        chartsNode.Attributes.Append(chartHeadingAttribute);

                        XmlAttribute chartAttribute = xmlDocument.CreateAttribute("url");
                        chartAttribute.Value = ChartsURL;
                        chartsNode.Attributes.Append(chartAttribute);
                        root.AppendChild(chartsNode);

                    }
                    XmlNode xmlDateNode = root.SelectSingleNode("CreatedDate");
                    if (xmlDateNode != null)
                    {
                        if (xmlDateNode.Attributes["date"] != null)
                        {
                            xmlDateNode.Attributes["date"].Value = CreatedDate;
                        }
                        else
                        {
                            xmlDateNode.AddAttribute("date", CreatedDate);
                        }

                    }
                    else
                    {
                        XmlNode dateNode = xmlDocument.CreateElement("CreatedDate");
                        XmlAttribute dateAttribute = xmlDocument.CreateAttribute("date");
                        dateAttribute.Value = CreatedDate;
                        dateNode.Attributes.Append(dateAttribute);
                        root.AppendChild(dateNode);
                    }

                    XmlNode xmlUserNode = root.SelectSingleNode("CreatedUser");
                    if (xmlUserNode != null)
                    {
                        if (xmlUserNode.Attributes["id"] != null)
                        {
                            xmlUserNode.Attributes["id"].Value = UserId;
                        }
                        else
                        {
                            xmlUserNode.AddAttribute("id", UserId);
                        }
                    }
                    else
                    {
                        XmlNode userNode = xmlDocument.CreateElement("CreatedUser");
                        XmlAttribute userAttribute = xmlDocument.CreateAttribute("id");
                        userAttribute.Value = UserId;
                        userNode.Attributes.Append(userAttribute);
                        root.AppendChild(userNode);
                    }
                }
            }
            xmlDocument.Save(FileName);

        }

        /// <summary>
        /// Modify specific client charts
        /// </summary>
        /// <param name="chartId">The newsId of the client.</param>
        public void Modify(string chartId)
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
                XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("ClientDetails[@Client=\"{0}\"]", Global.Core.ClientName));
                if (root != null)
                {
                    XmlNode subRoot = root.SelectSingleNode("CustomCharts");

                    XmlNode xmlNode = subRoot.SelectSingleNode(string.Format("CustomChart[@id=\"{0}\"]", chartId));

                    if (xmlNode != null)
                    {
                        if (xmlNode.Attributes != null)
                        {
                            xmlNode.Attributes["heading"].Value = ChartHeading;
                            xmlNode.Attributes["url"].Value = ChartsURL;
                            xmlNode.Attributes["UserId"].Value = UserId;
                            xmlNode.Attributes["CreatedDate"].Value = CreatedDate;
                        }
                    }
                }
            }
            xmlDocument.Save(FileName);

        }

        ///<summary>
        /// Save the clients home page details
        /// </summary>
        public void Save()
        {
            if (!string.IsNullOrEmpty(ClientName))
            {
                XmlDocument xmlDoc = new XmlDocument();

                try
                {
                    xmlDoc.Load(this.FileName);
                }
                catch (XmlException ex)
                {
                    XmlNode rootNode = xmlDoc.CreateElement("HomePage");
                    xmlDoc.AppendChild(rootNode);
                    xmlDoc.Save(FileName);
                }
                if (xmlDoc.DocumentElement != null)
                {
                    XmlNode root = xmlDoc.DocumentElement.SelectSingleNode(string.Format(
                        "ClientDetails[@Client=\"{0}\"]",
                        ClientName
                        ));

                    if (root == null)
                    {
                        XmlNode clientDetailsNode = xmlDoc.CreateElement("ClientDetails");
                        XmlAttribute attribute = xmlDoc.CreateAttribute("Client");
                        attribute.Value = ClientName;
                        clientDetailsNode.Attributes.Append(attribute);
                        xmlDoc.DocumentElement.AppendChild(clientDetailsNode);

                        XmlNode rssfeedNode = xmlDoc.CreateElement("RSSFeed");
                        XmlAttribute rssAttribute = xmlDoc.CreateAttribute("url");
                        rssAttribute.Value = RssFeedUrl;
                        rssfeedNode.Attributes.Append(rssAttribute);
                        clientDetailsNode.AppendChild(rssfeedNode);

                        XmlNode newsNode = xmlDoc.CreateElement("News");
                        XmlAttribute newsAttribute = xmlDoc.CreateAttribute("details");
                        newsAttribute.Value = NewsFile;
                        newsNode.Attributes.Append(newsAttribute);
                        clientDetailsNode.AppendChild(newsNode);

                        XmlNode reportsNode = xmlDoc.CreateElement("SavedReports");
                        XmlAttribute reportAttribute = xmlDoc.CreateAttribute("details");
                        reportAttribute.Value = SavedReports;
                        reportsNode.Attributes.Append(reportAttribute);
                        clientDetailsNode.AppendChild(reportsNode);

                        XmlNode chartParentNode = xmlDoc.CreateElement("CustomCharts");
                        clientDetailsNode.AppendChild(chartParentNode);

                        XmlNode chartsNode = xmlDoc.CreateElement("CustomChart");
                        XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
                        idAttribute.Value = GuId;
                        chartsNode.Attributes.Append(idAttribute);

                        XmlAttribute chartHeadingAttribute = xmlDoc.CreateAttribute("heading");
                        chartHeadingAttribute.Value = ChartHeading;
                        chartsNode.Attributes.Append(chartHeadingAttribute);

                        XmlAttribute chartAttribute = xmlDoc.CreateAttribute("url");
                        chartAttribute.Value = ChartsURL;
                        chartsNode.Attributes.Append(chartAttribute);

                        XmlAttribute createdDateAttribute = xmlDoc.CreateAttribute("CreatedDate");
                        createdDateAttribute.Value = CreatedDate;
                        chartsNode.Attributes.Append(createdDateAttribute);

                        XmlAttribute createdUserAttribute = xmlDoc.CreateAttribute("UserId");
                        createdUserAttribute.Value = UserId;
                        chartsNode.Attributes.Append(createdUserAttribute);

                        chartParentNode.AppendChild(chartsNode);

                        XmlNode dateNode = xmlDoc.CreateElement("CreatedDate");
                        XmlAttribute dateAttribute = xmlDoc.CreateAttribute("date");
                        dateAttribute.Value = CreatedDate;
                        dateNode.Attributes.Append(dateAttribute);
                        clientDetailsNode.AppendChild(dateNode);

                        XmlNode userNode = xmlDoc.CreateElement("CreatedUser");
                        XmlAttribute userAttribute = xmlDoc.CreateAttribute("id");
                        userAttribute.Value = UserId;
                        userNode.Attributes.Append(userAttribute);
                        clientDetailsNode.AppendChild(userNode);
                    }
                    else
                    {
                        XmlNode node = root.SelectSingleNode("CustomCharts");
                        if (node == null)
                        {
                            XmlNode chartParentNode = xmlDoc.CreateElement("CustomCharts");
                        
                            XmlNode chartsNode = xmlDoc.CreateElement("CustomChart");

                            XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
                            idAttribute.Value = GuId;
                            chartsNode.Attributes.Append(idAttribute);

                            XmlAttribute chartHeadingAttribute = xmlDoc.CreateAttribute("heading");
                            chartHeadingAttribute.Value = ChartHeading;
                            chartsNode.Attributes.Append(chartHeadingAttribute);

                            XmlAttribute chartAttribute = xmlDoc.CreateAttribute("url");
                            chartAttribute.Value = ChartsURL;
                            chartsNode.Attributes.Append(chartAttribute);

                            XmlAttribute createdDateAttribute = xmlDoc.CreateAttribute("CreatedDate");
                            createdDateAttribute.Value = CreatedDate;
                            chartsNode.Attributes.Append(createdDateAttribute);

                            XmlAttribute createdUserAttribute = xmlDoc.CreateAttribute("UserId");
                            createdUserAttribute.Value = UserId;
                            chartsNode.Attributes.Append(createdUserAttribute);
                            chartParentNode.AppendChild(chartsNode);
                            root.AppendChild(chartParentNode);
                        }
                        else
                        {
                            XmlNode chartsNode = xmlDoc.CreateElement("CustomChart");

                            XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
                            idAttribute.Value = GuId;
                            chartsNode.Attributes.Append(idAttribute);

                            XmlAttribute chartHeadingAttribute = xmlDoc.CreateAttribute("heading");
                            chartHeadingAttribute.Value = ChartHeading;
                            chartsNode.Attributes.Append(chartHeadingAttribute);

                            XmlAttribute chartAttribute = xmlDoc.CreateAttribute("url");
                            chartAttribute.Value = ChartsURL;
                            chartsNode.Attributes.Append(chartAttribute);

                            XmlAttribute createdDateAttribute = xmlDoc.CreateAttribute("CreatedDate");
                            createdDateAttribute.Value = CreatedDate;
                            chartsNode.Attributes.Append(createdDateAttribute);

                            XmlAttribute createdUserAttribute = xmlDoc.CreateAttribute("UserId");
                            createdUserAttribute.Value = UserId;
                            chartsNode.Attributes.Append(createdUserAttribute);

                            node.AppendChild(chartsNode);
                        }
                    }
                }
                xmlDoc.Save(FileName);
            }
        }

        ///<summary>
        /// Delete the  specific client home page details
        /// <param name="newsId">The newsId of the client.</param>
        /// </summary>
        public void Delete()
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
                XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("ClientDetails[@Client=\"{0}\"]", Global.Core.ClientName));
                if (root != null)
                {
                    XmlNode parentNode = root.ParentNode;
                    XmlNode xmlChartsNode = root.SelectSingleNode("CustomCharts");
                    if (xmlChartsNode != null)
                    {
                        //xmlChartsNode.ParentNode.RemoveAll();
                        XmlNode subNode = xmlChartsNode.ParentNode;
                        if (subNode != null) subNode.RemoveChild(xmlChartsNode);
                    }

                }
            }
            xmlDocument.Save(FileName);
        }

        /// <summary>
        /// Modify specific client charts
        /// </summary>
        /// <param name="chartId">The newsId of the client.</param>
        public void Delete(string chartId)
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
                XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("ClientDetails[@Client=\"{0}\"]", Global.Core.ClientName));
                if (root != null)
                {
                    XmlNode subRoot = root.SelectSingleNode("CustomCharts");

                    XmlNode xmlNode = subRoot.SelectSingleNode(string.Format("CustomChart[@id=\"{0}\"]", chartId));

                    if (xmlNode != null)
                    {
                        XmlNode parentNode = xmlNode.ParentNode;
                        if (parentNode != null) parentNode.RemoveChild(xmlNode);
                    }
                }
            }
            xmlDocument.Save(FileName);

        }


        /// <summary>
        /// Get Home page Details
        /// </summary>
        public XmlNode GetChartDetails(string clientName)
        {
            //string fileName = Path.Combine(
            //  HttpContext.Current.Request.PhysicalApplicationPath,
            //  "App_Data",
            //  "HomePage.xml"    );
            string fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", Global.Core.ClientName + "HomePage.xml");


            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                // Load the contents of the definition file into the xml document.
                xmlDocument.Load(fileName);
            }
            catch (XmlException ex)
            {
                return null;
            }
            // Select the client's xml node.
            if (xmlDocument.DocumentElement == null) return null;

            XmlNode root = xmlDocument.DocumentElement.SelectSingleNode(string.Format("ClientDetails[@Client=\"{0}\"]", clientName));
            if (root != null)
            {
                XmlNodeList xmlNodeList = root.ChildNodes;

                XmlNode xmlNode = root.SelectSingleNode("CustomCharts");
                // Return the content of xml node's database attribute.
                return xmlNode;
            }
            return null;
        }

        /// <summary>
        /// Gets selected client chart details
        /// </summary>
        /// <param name="newsId">The newsId of the client.</param>
        public XmlNode ChartDetails(string chartId)
        {
            string fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", Global.Core.ClientName + "HomePage.xml");
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                // Load the contents of the definition file into the xml document.
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
                XmlNode subRoot = root.SelectSingleNode("CustomCharts");
                if (subRoot != null)
                {
                    XmlNode xmlNode = subRoot.SelectSingleNode(string.Format("CustomChart[@id=\"{0}\"]", chartId));
                    // Return the content of xml node's database attribute.
                    return xmlNode;
                }
            }
            return null;
        }


        //Getting the news from the WebSite

        public Table GenerateNewsFromWebSite()
        {
            // loading html into HtmlDocument
            var doc = new HtmlWeb().Load("http://blueoceanmi.com/newsroom");
            // walking through all nodes of interest
            var tblSection = new WebUtilities.Controls.Table
            {
                ID = "tblNews",
                CssClass = "newsTable",
                CellPadding = 0,
                CellSpacing = 0
            };

            var htmlNodes =
                doc.DocumentNode.SelectNodes(
                    "//div[@class='col-xs-12 col-sm-12 col-md-12 col-lg-12 shadowleft lead-news shadowbottom']");

            int cnt = 1;

            foreach (var htmlNode in
                doc.DocumentNode.SelectNodes(
                    "//div[@class='col-xs-12 col-sm-12 col-md-12 col-lg-12 shadowleft lead-news shadowbottom']").Take(6)
                )
            {
                HtmlNode subNode = htmlNode.ChildNodes["div"];
                if (subNode != null)
                {
                    var row = new TableRow();
                    var cell = new TableCell
                    {
                        ID = "tdParentNews" + cnt,
                    };
                    var innerTbl = new WebUtilities.Controls.Table
                    {
                        ID = "innerNews" + cnt,
                        CellPadding = 2,
                        CellSpacing = 2
                    };
                    var innerRow = new TableRow();

                    if (subNode.ChildNodes["a"].HasAttributes)
                    {
                        var innerCell = new TableCell
                        {
                            ID = "cellNews" + cnt,
                            Text = subNode.InnerText.Trim(),
                            CssClass = "newsText Color1"
                        };

                        string navURL = "";
                        if (subNode.ChildNodes["a"].Attributes["href"].Value.Contains("news-details"))
                        {
                            navURL = "http://blueoceanmi.com/" + subNode.ChildNodes["a"].Attributes["href"].Value;
                        }
                        else
                        {
                            navURL = subNode.ChildNodes["a"].Attributes["href"].Value;
                        }

                        var hl = new System.Web.UI.WebControls.HyperLink()
                        {
                            Text = subNode.ChildNodes["a"].InnerHtml,
                            NavigateUrl = navURL,//subNode.ChildNodes["a"].Attributes["href"].Value,
                            ToolTip = subNode.ChildNodes["a"].InnerHtml,
                            Target = "_blank",
                            CssClass = "newsLink"
                        };
                        innerCell.Controls.Add(hl);
                        innerCell.Font.Bold = true;
                        innerRow.Cells.Add(innerCell);
                        innerTbl.Controls.Add(innerRow);

                        var innerRow1 = new TableRow();

                        var dateNode = ReadTillTwoBr(subNode);

                        var innerCell1 = new TableCell
                        {
                            ID = "cellNewsURLDate" + cnt,
                            Text = ReadTillTwoBr(subNode).FirstOrDefault()
                        };
                        innerCell1.Font.Bold = true;
                        innerCell1.CssClass = "newsSubText Color2";
                        innerRow1.Cells.Add(innerCell1);
                        innerTbl.Controls.Add(innerRow1);
                    }
                    cell.Controls.Add(innerTbl);
                    row.Cells.Add(cell);
                    tblSection.Rows.Add(row);
                }
                cnt++;
            }

            if (htmlNodes.Count < 4)
            {
                var prevDoc = new HtmlWeb().Load("http://www.blueoceanmi.com/newsroom?t=latest-news&yr=" + DateTime.Now.AddYears(-1).Year);
                var prevhtmlNodes = prevDoc.DocumentNode.SelectNodes(
                    "//div[@class='col-xs-12 col-sm-12 col-md-12 col-lg-12 shadowleft lead-news shadowbottom']");

                int innrCnt = cnt;

                foreach (var htmlNode in prevhtmlNodes.Take(6 - htmlNodes.Count))
                {
                    HtmlNode subNode = htmlNode.ChildNodes["div"];
                    if (subNode != null)
                    {
                        var row = new TableRow();
                        var cell = new TableCell
                        {
                            ID = "tdParentNews" + innrCnt,
                        };
                        var innerTbl = new WebUtilities.Controls.Table
                        {
                            ID = "innerNews" + innrCnt,
                            CellPadding = 2,
                            CellSpacing = 2
                        };
                        var innerRow = new TableRow();

                        if (subNode.ChildNodes["a"].HasAttributes)
                        {
                            var innerCell = new TableCell
                            {
                                ID = "cellNews" + innrCnt,
                                Text = subNode.InnerText.Trim(),
                                CssClass = "newsText Color1"
                            };

                            string navURL = "";
                            if (subNode.ChildNodes["a"].Attributes["href"].Value.Contains("news-details"))
                            {
                                navURL = "http://blueoceanmi.com/" + subNode.ChildNodes["a"].Attributes["href"].Value;
                            }
                            else
                            {
                                navURL = subNode.ChildNodes["a"].Attributes["href"].Value;
                            }

                            var hl = new System.Web.UI.WebControls.HyperLink()
                            {
                                Text = subNode.ChildNodes["a"].InnerHtml,
                                NavigateUrl = navURL,//subNode.ChildNodes["a"].Attributes["href"].Value,
                                ToolTip = subNode.ChildNodes["a"].InnerHtml,
                                Target = "_blank",
                                CssClass = "newsLink"
                            };
                            innerCell.Controls.Add(hl);
                            innerCell.Font.Bold = true;
                            innerRow.Cells.Add(innerCell);
                            innerTbl.Controls.Add(innerRow);

                            var innerRow1 = new TableRow();

                            var dateNode = ReadTillTwoBr(subNode);

                            var innerCell1 = new TableCell
                            {
                                ID = "cellNewsURLDate" + innrCnt,
                                Text = ReadTillTwoBr(subNode).FirstOrDefault()
                            };
                            innerCell1.Font.Bold = true;
                            innerCell1.CssClass = "newsSubText Color2";
                            innerRow1.Cells.Add(innerCell1);
                            innerTbl.Controls.Add(innerRow1);
                        }
                        cell.Controls.Add(innerTbl);
                        row.Cells.Add(cell);
                        tblSection.Rows.Add(row);
                    }
                    innrCnt++;
                }


            }

            return tblSection;
        }
        private IEnumerable<string> ReadTillTwoBr(HtmlNode node)
        {
            var nonEmptyNodes =
                node.ChildNodes.Except(
                    node.ChildNodes.Where(f => f.Name == "#text" && String.IsNullOrWhiteSpace(f.InnerHtml)))
                    .ToList();

            return from n in nonEmptyNodes.TakeWhile(n => !IsBr(n) || !IsBr(n.NextSibling))
                   where n.Name == "#text"
                   select n.InnerText.Trim();
        }

        private static bool IsBr(HtmlNode n)
        {
            return n != null && n.NodeType == HtmlNodeType.Element && n.Name == "br";
        }

      

        #endregion
    }
}