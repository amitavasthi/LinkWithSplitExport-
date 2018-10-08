using DatabaseCore.Items;
using HtmlAgilityPack;
using LinkOnline.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;


namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for HomeScreenHandler
    /// </summary>
    public class HomePageHandler : IHttpHandler, IRequiresSessionState
    {

        //public void ProcessRequest(HttpContext context)
        //{
        //    context.Response.ContentType = "text/plain";
        //    context.Response.Write("Hello World");
        //}

        public delegate void Deleg1(HttpContext context);

        #region Properties

        /// <summary>
        /// Gets or sets the available methods of the generic handler.
        /// </summary>
        public Dictionary<string, Deleg1> Methods { get; set; }

        /// <summary>
        /// Gets if the generic handler is re useable.
        /// </summary>
        /// 

        public int MaxWidgets
        {
            get
            {
                int N = 20;

                try
                {
                    N = int.Parse(ConfigurationManager.AppSettings["MaxWidgets"]);
                }
                catch { }

                return N;

            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion


        public HomePageHandler()
        {
            this.Methods = new Dictionary<string, Deleg1>();

            this.Methods.Add("GetContent", GetContent);
            this.Methods.Add("GetChartClick", GetChartClick);
            this.Methods.Add("GetWidgetsData", GetWidgetsData);
            this.Methods.Add("SaveWidgetsOrder", SaveWidgetsOrder);
            this.Methods.Add("AddComponent", AddComponent);
            this.Methods.Add("RemoveComponent", RemoveComponent);
            this.Methods.Add("SavedReportClick", SavedReportClick);

        }

        #region Methods
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(HttpContext context)
        {
            // Check if the current session has an authenticated user.
            if (HttpContext.Current.Session["User"] == null)
                throw new Exception("Not authenticated.");

            // Get the requested method name from the http request.
            string method = context.Request.Params["Method"];

            // Check if the requested method exists.
            if (!this.Methods.ContainsKey(method))
                throw new NotImplementedException();

            //Invoke the requested method.
            this.Methods[method].Invoke(context);
        }

        public void GetContent(HttpContext context)
        {
            int id = int.Parse(context.Request.Params["idvar"]);

            if (id == 1)
            {
                Panel myPanel = GenerateNewsFromWebSite();

                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter textWriter = new HtmlTextWriter(sw))
                    {
                        myPanel.RenderControl(textWriter);
                    }
                }

                //return sb.ToString();

                context.Response.Write(sb.ToString());
            }
            else if (id == 2)
            {
                Panel myPanel = RenderSavedReports();

                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter textWriter = new HtmlTextWriter(sw))
                    {
                        myPanel.RenderControl(textWriter);
                    }
                }

                context.Response.Write(sb.ToString());
            }
            else if (id == 3)
            {
                //Panel myPanel = GetCharts();

                //StringBuilder sb = new StringBuilder();
                //using (StringWriter sw = new StringWriter(sb))
                //{
                //    using (HtmlTextWriter textWriter = new HtmlTextWriter(sw))
                //    {
                //        myPanel.RenderControl(textWriter);
                //    }
                //}

                //context.Response.Write(sb.ToString());    


                string boxchart = "<div id='boxChartMsg' class=''><div id='boxChartMsgControl' class='Box BackgroundColor5' style='position: absolute; left: 687px; top: 349px;'>"
               + "<div class='BoxTitle'><img id='boxChartMsgCloseme' src='/Images/Icons/BoxClose.png' class='' style='float:right' onmouseover=\"this.src='/Images/Icons/BoxClose_Hover.png'\" onmouseout=\"this.src = '/Images/Icons/BoxClose.png'\" >"
               + "</div><div class='BoxContent'>"
               + "<table id='cphContent_Table1'>"
               + "<tbody><tr>"
               + "<td style='align-content:center;text-align:center;font-style:normal'>"
               + "<span id='cphContent_no custom charts' class='noMsg'>no custom charts are defined</span>"
               + "</td>"
               + "</tr>"
               + "</tbody>"
               + "</table>"
               + "</div></div><div id='BoxBackground' class='BoxBackground'></div></div>";

                context.Response.Write(
                    "<div>"
                    + "<img class=\"chart-image\" id=\"imgChart\" src=\"../Images/Icons/Home/chart1.PNG\" onclick=\"chartClick();\">"
                    + "</div>"
                    + boxchart
                    );


            }



        }

        public void GetWidgetsData(HttpContext context)
        {
            if (!File.Exists(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml")))
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode rootNode = xmlDoc.CreateElement("HomeScreen");
                xmlDoc.AppendChild(rootNode);

                var pageNode = CreateHomeNode(xmlDoc);

                rootNode.AppendChild(pageNode);

                xmlDoc.Save(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml"));


            }


            StringBuilder sb = new StringBuilder();

            if (File.Exists(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml")))
            {
                XmlDocument xmldoc = new XmlDocument();

                bool flag = false;

                using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml")))
                {


                    xmldoc.Load(sr);

                    var node = xmldoc.SelectSingleNode("HomeScreen/HomePage[@ClientName='" + Global.Core.ClientName + "']");

                    if (node != null)
                    {
                        string xml = node.InnerXml;

                        xml = "<HomePage>" + xml + "</HomePage>";

                        context.Response.Write(HomeHelper.XmlToJSON(xml));
                    }
                    else
                    {
                        var pageNode = CreateHomeNode(xmldoc);

                        XmlNode screen = xmldoc.SelectSingleNode("HomeScreen");
                        screen.AppendChild(pageNode);

                        flag = true;

                        string xml = pageNode.InnerXml;

                        xml = "<HomePage>" + xml + "</HomePage>";

                        context.Response.Write(HomeHelper.XmlToJSON(xml));

                    }
                }

                if (flag)
                {
                    xmldoc.Save(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml"));
                }
            }

            context.Response.Write(string.Empty);

        }

        XmlNode CreateHomeNode(XmlDocument xmlDoc)
        {
            XmlNode pageNode = xmlDoc.CreateElement("HomePage");
            XmlAttribute attribute = xmlDoc.CreateAttribute("ClientName");
            attribute.Value = Global.Core.ClientName;
            pageNode.Attributes.Append(attribute);

            XmlNode compNode = xmlDoc.CreateElement("Component");
            attribute = xmlDoc.CreateAttribute("Rank");
            attribute.Value = "1";
            compNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("Divid");
            attribute.Value = "1";
            compNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("ContentType");
            attribute.Value = "1";
            compNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("Title");
            attribute.Value = "LiNK announcements";
            compNode.Attributes.Append(attribute);
            pageNode.AppendChild(compNode);

            compNode = xmlDoc.CreateElement("Component");
            attribute = xmlDoc.CreateAttribute("Rank");
            attribute.Value = "1";
            compNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("Divid");
            attribute.Value = "2";
            compNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("ContentType");
            attribute.Value = "2";
            compNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("Title");
            attribute.Value = "LiNK Overview";
            compNode.Attributes.Append(attribute);
            pageNode.AppendChild(compNode);

            compNode = xmlDoc.CreateElement("Component");
            attribute = xmlDoc.CreateAttribute("Rank");
            attribute.Value = "2";
            compNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("Divid");
            attribute.Value = "2";
            compNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("ContentType");
            attribute.Value = "3";
            compNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("Title");
            attribute.Value = "most recent used";
            compNode.Attributes.Append(attribute);
            pageNode.AppendChild(compNode);

            return pageNode;
        }

        public void AddComponent(HttpContext context)
        {
            string data = context.Request.Params["data"];

            string[] temp = data.Split(',');

            XmlDocument xmlDoc = new XmlDocument();

            using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml")))
            {

                xmlDoc.Load(sr);

                XmlNode pnode = xmlDoc.SelectSingleNode("HomeScreen/HomePage[@ClientName='" + Global.Core.ClientName + "']");

                List<int> ids = new List<int>();

                foreach (XmlNode n in pnode.ChildNodes)
                {
                    ids.Add(int.Parse(n.Attributes["ContentType"].Value.ToString()));
                }


                int ctid = 0;

                for (int i = 1; i <= MaxWidgets; i++)
                {
                    if (!ids.Contains(i))
                    {
                        ctid = i;
                        break;
                    }

                }

                if (ctid == 0) return;

                if (pnode != null)
                {
                    XmlNode compNode = xmlDoc.CreateElement("Component");
                    XmlAttribute attribute = xmlDoc.CreateAttribute("Rank");
                    attribute.Value = temp[0];
                    compNode.Attributes.Append(attribute);
                    attribute = xmlDoc.CreateAttribute("Divid");
                    attribute.Value = temp[1];
                    compNode.Attributes.Append(attribute);
                    attribute = xmlDoc.CreateAttribute("ContentType");
                    attribute.Value = ctid.ToString();
                    compNode.Attributes.Append(attribute);
                    attribute = xmlDoc.CreateAttribute("Title");
                    attribute.Value = temp[3].ToLower();
                    compNode.Attributes.Append(attribute);
                    pnode.AppendChild(compNode);
                }
            }

            xmlDoc.Save(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml"));


        }

        public void SaveWidgetsOrder(HttpContext context)
        {

            int maxwidgets = MaxWidgets;

            string order = context.Request.Params["order"];

            string[] strs = order.Split('^');

            string[] temp = strs[0].Split('-');

            var divDrop = int.Parse(temp[0]);
            var divDrag = int.Parse(temp[1]);
            var rankDrop = int.Parse(temp[2]);
            var rankDrag = int.Parse(temp[3]);
            var append = int.Parse(temp[4]);



            //===================================





            XmlDocument xmldoc = new XmlDocument();

            using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml")))
            {

                xmldoc.Load(sr);

                XmlNode anode = xmldoc.SelectSingleNode("HomeScreen/HomePage[@ClientName='" + Global.Core.ClientName + "']");

                if (anode != null)
                {



                    if (divDrop == divDrag)
                    {
                        if (append == 1)
                        {


                            if (rankDrop < rankDrag)
                            {
                                var nodedrag = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + rankDrag + "' and @Divid='" + divDrop + "']");



                                for (int i = rankDrag - 1, indx = rankDrag; i > rankDrop; i--)
                                {

                                    var node = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + i + "' and @Divid='" + divDrop + "']");

                                    if (node != null)
                                    {
                                        node.SetAttribute("Rank", indx.ToString());
                                        indx--;
                                    }

                                }

                                nodedrag.SetAttribute("Rank", (rankDrop + 1).ToString());
                            }

                            else if (rankDrag < rankDrop)
                            {

                                var nodedrag = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + rankDrag + "' and @Divid='" + divDrop + "']");

                                for (int i = rankDrag + 1, indx = rankDrag; i <= rankDrop; i++)
                                {

                                    var node = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + i + "' and @Divid='" + divDrop + "']");

                                    if (node != null)
                                    {
                                        if (i == rankDrop)
                                        {
                                            rankDrop = indx;
                                        }

                                        node.SetAttribute("Rank", indx.ToString());
                                        indx++;
                                    }

                                }

                                if (nodedrag != null)
                                {

                                    nodedrag.SetAttribute("Rank", (rankDrop + 1).ToString());
                                }


                            }

                        }
                        //prepend
                        else
                        {

                            if (rankDrop < rankDrag)
                            {
                                var nodedrag = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + rankDrag + "' and @Divid='" + divDrop + "']");

                                for (int i = rankDrag - 1, indx = rankDrag; i >= rankDrop; i--)
                                {

                                    var node = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + i + "' and @Divid='" + divDrop + "']");

                                    if (node != null)
                                    {

                                        node.SetAttribute("Rank", indx.ToString());
                                        indx--;
                                    }

                                }



                                nodedrag.SetAttribute("Rank", (rankDrop).ToString());
                            }

                            else if (rankDrag < rankDrop)
                            {

                                var nodedrag = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + rankDrag + "' and @Divid='" + divDrop + "']");

                                for (int i = rankDrag + 1, indx = rankDrag; i < rankDrop; i++)
                                {

                                    var node = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + i + "' and @Divid='" + divDrop + "']");

                                    if (node != null)
                                    {


                                        node.SetAttribute("Rank", indx.ToString());
                                        indx++;
                                    }

                                }

                                nodedrag.SetAttribute("Rank", (rankDrop - 1).ToString());

                            }

                        }


                    } //divdrop != dragdrop
                    else
                    {
                        if (append == 1)
                        {


                            for (int i = maxwidgets - 1, indx = maxwidgets; i > rankDrop; i--)
                            {

                                var node = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + i + "' and @Divid='" + divDrop + "']");

                                node.SetAttribute("Rank", indx.ToString());

                                indx--;
                            }

                            var nodeDrag = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + rankDrag + "' and @Divid='" + divDrag + "']");
                            if (nodeDrag != null)
                            {
                                nodeDrag.SetAttribute("Divid", divDrop.ToString());
                                nodeDrag.SetAttribute("Rank", (rankDrop + 1).ToString());
                            }

                        }
                        else //prepend
                        {

                            for (int i = maxwidgets - 1, indx = maxwidgets; i >= rankDrop; i--)
                            {

                                var node = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + i + "' and @Divid='" + divDrop + "']");
                                if (node != null)
                                {
                                    node.SetAttribute("Rank", indx.ToString());

                                }


                                indx--;

                            }



                            var nodeDrag = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + rankDrag + "' and @Divid='" + divDrag + "']");

                            if (nodeDrag != null)
                            {
                                nodeDrag.SetAttribute("Divid", divDrop.ToString());
                                nodeDrag.SetAttribute("Rank", (rankDrop).ToString());
                            }

                        }


                        for (int i = 1, indx = 1; i < maxwidgets; i++)
                        {
                            var node = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + i + "' and @Divid='" + divDrag + "']");

                            if (node != null)
                            {
                                node.SetAttribute("Rank", indx.ToString());
                                indx++;
                            }
                        }

                    }



                }

            }

            xmldoc.Save(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml"));

        }

        public void RemoveComponent(HttpContext context)
        {
            string data = context.Request.Params["data"];

            string[] temp = data.Split('-');

            XmlDocument xmldoc = new XmlDocument();

            using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml")))
            {

                xmldoc.Load(sr);

                XmlNode anode = xmldoc.SelectSingleNode("HomeScreen/HomePage[@ClientName='" + Global.Core.ClientName + "']");

                if (anode != null)
                {

                    var node = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + temp[0] + "' and @Divid='" + temp[1] + "']");

                    var attr = node.GetAttribute("ContentType");

                    if (attr == "1" || attr == "2" || attr == "3")
                    {
                        return;
                    }

                    anode.RemoveChild(node);

                    int start = int.Parse(temp[0]);

                    for (int i = start + 1, indx = start; i < MaxWidgets; i++)
                    {
                        var rnode = (XmlElement)anode.SelectSingleNode("Component[@Rank='" + i + "' and @Divid='" + temp[1] + "']");

                        if (rnode != null)
                        {
                            rnode.SetAttribute("Rank", indx.ToString());

                            indx++;
                        }
                    }

                }
            }

            xmldoc.Save(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml"));
        }

        public void GetChartClick(HttpContext context)
        {
            var hManager = new HomeManager();

            var xmlHomeDetails = hManager.GetChartDetails(Global.Core.ClientName);

            if (xmlHomeDetails != null)
            {
                if (xmlHomeDetails.Attributes != null)
                {
                    if (xmlHomeDetails.ChildNodes.Count > 1)
                    {
                        context.Response.Write("redirect:/Pages/CustomCharts/ChartsHome.aspx");
                    }
                    else
                    {
                        if (xmlHomeDetails.ChildNodes != null)
                        {
                            if (xmlHomeDetails.ChildNodes.Item(0) != null)
                            {
                                if (xmlHomeDetails.ChildNodes.Item(0).Attributes["url"] != null)
                                {
                                    if (!string.IsNullOrEmpty(xmlHomeDetails.ChildNodes.Item(0).Attributes["url"].Value))
                                    {
                                        //Response.Redirect("Default.aspx", false);
                                        // Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenWindow", "window.open('YourURL','_newtab');", true);

                                        context.Response.Write("redirect:Default.aspx?fw=HP");
                                    }
                                    else
                                    {
                                        context.Response.Write("makevisible:boxChartMsg");
                                    }
                                }
                                else
                                {
                                    context.Response.Write("makevisible:boxChartMsg");
                                }
                            }
                            else
                            {
                                hManager.Delete();
                                context.Response.Write("makevisible:boxChartMsg");
                            }
                           
                        }
                        else
                        {
                            context.Response.Write("makevisible:boxChartMsg");
                        }
                    }
                }
                else
                {
                    context.Response.Write("makevisible:boxChartMsg");
                }
            }
            else
            {
                context.Response.Write("makevisible:boxChartMsg");
            }
        }


        #endregion



        private Panel GetCharts()
        {
            using (Page page = new Page())
            {
                Panel myPanel = new Panel();

                UserControl userControl = (UserControl)page.LoadControl("~/Pages/Charts.ascx");

                myPanel.Controls.Add(userControl);

                return myPanel;


            }
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

        private bool IsBr(HtmlNode n)
        {
            return n != null && n.NodeType == HtmlNodeType.Element && n.Name == "br";
        }



        private Panel GenerateNewsFromWebSite()
        {
            Panel myPanel = new Panel();

            if (HttpContext.Current.Session["NewsDetails"] != null)
            {
                var tblNews = new WebUtilities.Controls.Table
                {
                    ID = "tblNews",
                    CssClass = "newsTable",
                    CellPadding = 0,
                    CellSpacing = 0
                };

                var newsXmlRow = new TableRow();
                var tdClientNews = new TableCell
                {
                    ID = "tdClientNewsFromXML"
                };
                if (GetNews() != null)
                {
                    tdClientNews.Controls.Add(GetNews());

                }
                else
                {
                    tdClientNews.CssClass = "noMsg Color2";
                    tdClientNews.Text = "no news available";
                }
                newsXmlRow.Cells.Add(tdClientNews);
                tblNews.Rows.Add(newsXmlRow);
                var emptyRow = new TableRow();
                var emptyCell = new TableCell
                {
                    ID = "emptyCell",
                    CssClass = "emptCell"
                };
                emptyRow.Cells.Add(emptyCell);
                tblNews.Rows.Add(emptyRow);

                var globalHeadingRow = new TableRow();
                var tdglobalHeading = new TableCell
                {
                    ID = "tdglobalHeading",
                    CssClass = "widgetTitle Color1",
                    Text = "news @ blueocean"
                };
                globalHeadingRow.Cells.Add(tdglobalHeading);
                tblNews.Rows.Add(globalHeadingRow);

                var emptyRow1 = new TableRow();
                var emptyCell1 = new TableCell
                {
                    ID = "emptyCell1",
                    CssClass = "emptCell"
                };
                emptyRow1.Cells.Add(emptyCell1);
                tblNews.Rows.Add(emptyRow1);
               
                var gloablNews = new TableRow();
                var tdgloablNews = new TableCell
                {
                    ID = "tdgloablNews",
                    CssClass="globalNews"
                };

                tdgloablNews.Controls.Add(HttpContext.Current.Session["NewsDetails"] as Table);
                gloablNews.Cells.Add(tdgloablNews);
                tblNews.Rows.Add(gloablNews);

                //pnlRssFeeds.Controls.Add(tblNews);
                myPanel.Controls.Add(tblNews);
                return myPanel;
            }
            else
            {

                return myPanel;
            }

        }

        private Table GetNews()
        {
            var newsManager = new NewsManager();

            var xmlNewsListBase = newsManager.GetNewsAll(Global.Core.ClientName);

            var innerTbl = new WebUtilities.Controls.Table
            {
                ID = "NewsFromXML",
                CellPadding = 2,
                CellSpacing = 2,
                CssClass = "tblNews"
                //BackColor = System.Drawing.ColorTranslator.FromHtml("#E2EFF9")
            };
            //innerTbl.Attributes.Add(
            //        "onmouseover",
            //        "this.bgColor='#E2EFF9'"
            //    );

            //innerTbl.Attributes.Add(
            //      "onmouseout",
            //      "this.bgColor='#FFFFFF'"
            //  );
            if (xmlNewsListBase != null)
            {
                /*The below code using for sort the XML based on an attribute*/
                var xmlNewsList = new List<XmlNode>(xmlNewsListBase.Cast<XmlNode>().OrderByDescending(p => p.Attributes["CreatedDate"].Value));
                if (xmlNewsList.Count != 0)
                {
                    int i = 1;
                    foreach (XmlNode xn in xmlNewsList)
                    {
                        var xmlElement = xn;
                        if (xmlElement != null)
                        {
                            var innerRow = new TableRow();
                            var innerCell = new TableCell
                            {
                                ID = "NewsXMLHeading" + i,
                                Text = xmlElement.Attributes["Heading"].Value,
                                CssClass = "newsText Color1"
                            };
                            innerCell.Font.Bold = true;
                            innerRow.Cells.Add(innerCell);
                            innerTbl.Controls.Add(innerRow);

                            var innerRow1 = new TableRow();
                            string datePart = null;
                            if (!string.IsNullOrEmpty(xmlElement.Attributes["CreatedDate"].Value))
                            {
                                datePart = xmlElement.Attributes["CreatedDate"].Value;
                            }
                            var innerCell1 = new TableCell
                            {
                                ID = "NewsXMLDesc" + i,
                                Text = xmlElement.Attributes["Description"].Value,
                                CssClass = "newsText Color1"
                            };
                            //innerCell1.Font.Bold = true;
                            //innerCell1.CssClass = "newsSubText Color2";
                            innerRow1.Cells.Add(innerCell1);
                            innerTbl.Controls.Add(innerRow1);

                            var innerRow2 = new TableRow();

                            string userName = "";
                            if (xmlElement.Attributes["UserId"] != null)
                            {
                                User user = null;
                                user = Global.Core.Users.GetSingle(Guid.Parse(xmlElement.Attributes["UserId"].Value.Trim()));
                                if (user != null)
                                {
                                    userName = user.FirstName + "&nbsp;" + user.LastName;
                                }

                            }
                            var innerCell2 = new TableCell
                            {
                                ID = "NewsCreatedUser" + i,
                                Text = Convert.ToDateTime(datePart).ToString("MMMM dd, yyyy") + " | &nbsp;" + userName,
                                CssClass = "newsSubText Color2"
                            };
                            innerCell2.Font.Bold = true;
                            innerRow2.Cells.Add(innerCell2);
                            innerTbl.Controls.Add(innerRow2);
                        }
                        i++;
                    }
                }
                else
                {
                    innerTbl = null;
                }
            }
            else
            {
                innerTbl = null;
            }

            return innerTbl;

        }


        /// <summary>
        /// Render Saved Reports
        /// </summary>

        private Panel RenderSavedReports()
        {
            // Build the full path to the current users's saved reports directory.
            string directory = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedReports",
                Global.Core.ClientName,
                Global.User.Id.ToString()
            );

            var tblReport = new WebUtilities.Controls.Table
            {
                ID = "tblReport",
                CssClass = "newsText Color1",
                CellPadding = 0,
                CellSpacing = 0
            };

            if (Directory.Exists(directory))
            {
                if (Directory.GetFiles(directory).Any())
                {
                    var row = new TableRow();
                    var cnt = 1;
                    // foreach (var file in Directory.GetFiles(directory).OrderByDescending(file => file).Take(2))
                    foreach (var file in Directory.GetFiles(directory).OrderByDescending(file => new FileInfo(file).LastWriteTime).Take(3))
                    {
                        var fInfo = new FileInfo(file);
                        // Check if the file is a saved report.
                        if (fInfo.Extension == ".lor")
                        {
                            var reportCell = new TableCell
                            {
                                ID = "reportCell" + cnt,
                                CssClass = "rptCell"
                            };

                            reportCell.Attributes.Add("Style", "margin-left:20px;");

                            var imgFile = new ImageButton
                            {
                                ID = "imgFile" + fInfo.Name.Split('.')[0],
                                CssClass = "BackgroundColor1",
                                //Width = 50,
                                //Height = 50
                            };
                            imgFile.Attributes.Add("Source", file);

                            string imgFileName = Path.Combine(
                                       HttpContext.Current.Request.PhysicalApplicationPath,
                                       "Images",
                                       "Icons",
                                       "Cloud",
                                       fInfo.Extension + ".png"
                                   );

                            string imageName = fInfo.Extension + ".png";

                            //imgFile.ImageUrl = cnt % 2 == 0 ? "/Images/Icons/Home/report1.png" : "/Images/Icons/Home/report2.png";
                            imgFile.ImageUrl = "/Images/Icons/Cloud/.lor.png";
                            imgFile.Attributes.Add(
                                                   "onmouseover",
                                                   "this.src = '/Images/Icons/Cloud/Run.png'"
                                               );
                            imgFile.Attributes.Add(
                                                   "onmouseout",
                                                   "this.src = '/Images/Icons/Cloud/" + imageName + "';document.forms[0].action = document.forms[0].action.split('?')[0];"
                                               );

                            imgFile.Attributes.Add(
                                                   "onclick",
                                                   "return savedReportsClick(this);"
                                               );


                            //LinkOnline.Pages.LandingPage page = new LinkOnline.Pages.LandingPage();

                            //imgFile.Click += page.SavedReport_Click;

                            reportCell.Controls.Add(imgFile);
                            row.Controls.Add(reportCell);

                            //var reportText = new Label
                            //{
                            //    ID = "lbl" + fInfo.Name.Split('.')[0],
                            //    CssClass = "reportText Color1",
                            //    Text = fInfo.Name.Split('.')[0].Trim()
                            //};
                            //reportCell.Controls.Add(reportText);


                            var reportCellText = new TableCell
                            {
                                ID = "reportCellText" + cnt,
                                CssClass = "reportText Color1",
                                Text = fInfo.Name.Split('.')[0],

                            };

                            row.Controls.Add(reportCellText);

                            tblReport.Rows.Add(row);
                        }
                        else
                        {
                            var cellReport = new TableCell
                            {
                                ID = "cellReport",
                                CssClass = "noMsg Color2",
                                ColumnSpan = 4,
                                Text = "no reports are saved"
                            };
                            row.Controls.Add(cellReport);
                            cellReport.Font.Size = 14;
                            tblReport.Rows.Add(row);
                        }
                        cnt++;
                    }
                }
                else
                {
                    var row = new TableRow();
                    var cellReport = new TableCell
                    {
                        ID = "cellReport",
                        CssClass = "noMsg Color2",
                        ColumnSpan = 4,
                        Text = "no reports are saved"
                    };

                    cellReport.Font.Size = 14;
                    row.Controls.Add(cellReport);
                    tblReport.Rows.Add(row);
                }
            }
            else
            {
                var row = new TableRow();
                var cellReport = new TableCell
                {
                    ID = "cellReport",
                    CssClass = "noMsg Color2",
                    ColumnSpan = 4,
                    Text = "no reports are saved"
                };

                cellReport.Font.Size = 14;
                row.Controls.Add(cellReport);
                tblReport.Rows.Add(row);
            }

            Panel mypanel = new Panel();

            mypanel.Controls.Add(tblReport);

            //pnlReportDetails.Controls.Add(tblReport);

            return mypanel;
        }

        public void SavedReportClick(HttpContext context)
        {
            string data = context.Request.Params["data"];
            // Cast the sending object as image button.
            //var imgDirectory = (ImageButton)sender;

            string reportDefinitionFileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "ReportDefinitions",
                Global.Core.ClientName,
                Global.User.Id + ".xml"
            );

            File.Copy(
                data,
                //imgDirectory.Attributes["Source"],
                reportDefinitionFileName,
                true
            );

            context.Response.Write("/Pages/LinkReporter/Crosstabs.aspx");
        }

    }
}



