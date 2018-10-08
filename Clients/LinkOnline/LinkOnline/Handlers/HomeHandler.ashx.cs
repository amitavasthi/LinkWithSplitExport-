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
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for HomeHandler
    /// </summary>
    public class HomeHandler : IHttpHandler, IRequiresSessionState
    {

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
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion
        public HomeHandler()
        {
            this.Methods = new Dictionary<string, Deleg1>();
            this.Methods.Add("SavedReportClick", SavedReportClick);
            this.Methods.Add("SaveOrder", SaveOrder);
            this.Methods.Add("ChartTypeSave", ChartTypeSave);
            this.Methods.Add("GetChartType", GetChartType);
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

        /// <summary>
        /// Handles the OnClick event for the most recent used reports
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void SavedReportClick(HttpContext context)
        {
            var data = context.Request.Params["data"];

            var recentUsed = new RecentUsed();

            if (data.Split('_')[0].ToString() == "imgChart")
            {
                var id = data.Split('_');


                recentUsed.GuId = Guid.NewGuid().ToString();
                recentUsed.itemName = id[1];
                recentUsed.itemType = "CustomChart";
                recentUsed.UsedDate = DateTime.Now.ToString();
                recentUsed.CreateRecentUsed();

                context.Response.Write(
                    "/Pages/Dashboards.aspx?val=" + id[1]
                );
                // context.Response.Write("redirect:/Pages/Default.aspx?val=" + id[1]);


            }
            else if (data.Split('_')[0].ToString() == "imgReport")
            {
                recentUsed.GuId = Guid.NewGuid().ToString();
                recentUsed.itemName = data.Split('_')[1];//imgDirectory.ID.Split('_')[1];
                recentUsed.activeName = data.Split('_')[2];
                recentUsed.itemType = "LinkReporter";
                recentUsed.UsedDate = DateTime.Now.ToString();
                recentUsed.CreateRecentUsed();

              context.Response.Write("/Pages/LinkReporter/Crosstabs.aspx?SavedReport=" + Global.User.Id  + data.Split('_')[2]);
            }
            else
            {
                string fileName = Path.Combine(
                 context.Request.PhysicalApplicationPath,
                 "Fileadmin",
                 "SavedLinkBiDefinitions", Global.Core.ClientName,
                  data.Split('_')[1] + ".xml");

                HttpContext.Current.Session["LinkBiDefinition"] = fileName;

                recentUsed.GuId = Guid.NewGuid().ToString();
                recentUsed.itemName = data.Split('_')[1];
                recentUsed.itemType = "LinkBiReport";
                recentUsed.UsedDate = DateTime.Now.ToString();
                recentUsed.CreateRecentUsed();

                context.Response.Write("/Pages/LinkBi//LinkBi.aspx");
            }

        }

        /// <summary>
        /// Handles the SaveOrder of the homepage divs
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void SaveOrder(HttpContext context)
        {
            var order = context.Request.Params["order"];

            var splitOrder = order.Split(',');

            var order1Name = "NewsPart";
            var order2Name = "ChartPart";
            var order3Name = "MostRecentPart";

            var newOrder1 = splitOrder[0].Split('.')[0];
            var newOrder2 = splitOrder[1].Split('.')[0];
            var newOrder3 = splitOrder[2].Split('.')[0];


            XmlDocument xmldoc = new XmlDocument();

            //using (StreamReader streamReader = new StreamReader(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml")))
            using (StreamReader streamReader = new StreamReader(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "HomeScreen", Global.Core.ClientName, Global.User.Id + ".xml")))
            {
                xmldoc.Load(streamReader);

                XmlNode anode = xmldoc.SelectSingleNode("HomeScreen");

                if (anode != null)
                {
                    XmlNodeList listNode = anode.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes;
                    XmlNode firstLiNode = anode.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).SelectSingleNode("ul").ChildNodes.Item(0);
                    XmlNode secondLiNode = anode.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).SelectSingleNode("ul").ChildNodes.Item(1);
                    XmlNode thirdLiNode = anode.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).SelectSingleNode("ul").ChildNodes.Item(2);

                    string divId1 = "";// "upleft";
                    string divId2 = "";//"upright";
                    string divId3 = "";//"bottomleft";

                    if (newOrder1 == "order-1")
                    {
                        divId1 = "upleft";
                    }
                    else if (newOrder1 == "order-2")
                    {
                        divId2 = "upleft";
                    }
                    else
                    {
                        divId3 = "upleft";
                    }
                    if (newOrder2 == "order-1")
                    {
                        divId1 = "upright";
                    }
                    else if (newOrder2 == "order-2")
                    {
                        divId2 = "upright";
                    }
                    else
                    {
                        divId3 = "upright";
                    }

                    if (newOrder3 == "order-1")
                    {
                        divId1 = "bottomleft";
                    }
                    else if (newOrder3 == "order-2")
                    {
                        divId2 = "bottomleft";
                    }
                    else
                    {
                        divId3 = "bottomleft";
                    }

                    string firstNodeContent = "<div id='" + divId1 + "' class='col-sm-12 news'><img class='handle' src='../Images/drag-handle-dark-icon.png'>";
                    firstNodeContent = firstNodeContent + "<span style='color:#6CAEE0;margin-left:30px;' class='newsTitle'>L</span><span style='color:#FCB040' class='newsTitle'>i</span>";
                    firstNodeContent = firstNodeContent + "<span style='color:#6CAEE0' class='newsTitle'>NK </span><span style='color:#6CAEE0' class='newsTitle'> announcements</span>";
                    firstNodeContent = firstNodeContent + "<Module Name='News'>NewsPart</Module></img></div>";

                    string secondNodeContent = "<div id='" + divId2 + "' class='col-sm-12 news'><img class='handle' src='../Images/drag-handle-dark-icon.png'>";
                    secondNodeContent = secondNodeContent + "<span style='float:right;'><a href='LinkOverview.aspx'><img src='../Images/Icons/Home/edit.png' style='height:20px;margin-top:5px;'/></a></span>";
                    secondNodeContent = secondNodeContent + "<span style='color:#6CAEE0;margin-left:30px;' class='newsTitle'>L</span><span style='color:#FCB040' class='newsTitle'>i</span>";
                    secondNodeContent = secondNodeContent + "<span style='color:#6CAEE0' class='newsTitle'>NK </span><span style='color:#6CAEE0' class='newsTitle'> overview</span>";
                    secondNodeContent = secondNodeContent + "<Module Name='DashBoards'>ChartPart</Module></img></div>";

                    string thirdNodeContent = "<div id='" + divId3 + "' class='col-sm-12 news'><img class='handle' src='../Images/drag-handle-dark-icon.png'>";
                    thirdNodeContent = thirdNodeContent + "<span style='color:#6CAEE0;margin-left:30px;' class='newsTitle'>L</span><span style='color:#FCB040' class='newsTitle'>i</span>";
                    thirdNodeContent = thirdNodeContent + "<span style='color:#6CAEE0' class='newsTitle'>NK </span><span style='color:#6CAEE0' class='newsTitle'> most recent used</span>";
                    thirdNodeContent = thirdNodeContent + "<Module Name='MostRecent'>MostRecentPart</Module></img></div>";


                    if (newOrder1 == "order-1")
                    {
                        divId1 = "upleft";
                        firstLiNode.InnerXml = firstNodeContent;
                    }
                    else if (newOrder1 == "order-2")
                    {
                        divId2 = "upleft";
                        firstLiNode.InnerXml = secondNodeContent;
                    }
                    else
                    {
                        divId3 = "upleft";
                        firstLiNode.InnerXml = thirdNodeContent;
                    }

                    if (newOrder2 == "order-1")
                    {
                        divId1 = "upright";
                        secondLiNode.InnerXml = firstNodeContent;
                    }
                    else if (newOrder2 == "order-2")
                    {
                        divId2 = "upright";
                        secondLiNode.InnerXml = secondNodeContent;
                    }
                    else
                    {
                        divId3 = "upright";
                        secondLiNode.InnerXml = thirdNodeContent;
                    }

                    if (newOrder3 == "order-1")
                    {
                        divId1 = "bottomleft";
                        thirdLiNode.InnerXml = firstNodeContent;
                    }
                    else if (newOrder3 == "order-2")
                    {
                        divId2 = "bottomleft";
                        thirdLiNode.InnerXml = secondNodeContent;
                    }
                    else
                    {
                        divId3 = "bottomleft";
                        thirdLiNode.InnerXml = thirdNodeContent;
                    }

                    for (int i = 0; i < listNode.Count; i++)
                    {
                        var xmlElement = listNode.Item(i).ChildNodes.Item(0).ChildNodes.Item(0).LastChild;

                        if (xmlElement.InnerText.Trim() == order1Name)
                        {
                            if (newOrder1 != "order-1")
                            {
                                if (newOrder1 == "order-2")
                                {
                                    xmlElement.PreviousSibling.InnerText = "overview";
                                    xmlElement.InnerText = order2Name;
                                }
                                else if (newOrder1 == "order-3")
                                {
                                    xmlElement.PreviousSibling.InnerText = "most recent used";
                                    xmlElement.InnerText = order3Name;
                                }
                            }
                        }
                        else if (xmlElement.InnerText.Trim() == order2Name)
                        {
                            if (newOrder2 != "order-2")
                            {
                                if (newOrder2 == "order-1")
                                {
                                    xmlElement.PreviousSibling.InnerText = "announcements";
                                    xmlElement.InnerText = order1Name;
                                }
                                else if (newOrder2 == "order-3")
                                {
                                    xmlElement.PreviousSibling.InnerText = "most recent used";
                                    xmlElement.InnerText = order3Name;
                                }
                            }
                        }
                        else if (xmlElement.InnerText.Trim() == order3Name)
                        {
                            if (newOrder3 != "order-3")
                            {
                                if (newOrder3 == "order-1")
                                {
                                    xmlElement.PreviousSibling.InnerText = "announcements";
                                    xmlElement.InnerText = order1Name;
                                }
                                else if (newOrder3 == "order-2")
                                {
                                    xmlElement.PreviousSibling.InnerText = "overview";
                                    xmlElement.InnerText = order2Name;
                                }
                            }
                        }

                    }

                }

            }

            xmldoc.Save(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "HomeScreen", Global.Core.ClientName, Global.User.Id + ".xml"));

        }



        /// <summary>
        /// Handles the ChartTypeSave of the homepage divs
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ChartTypeSave(HttpContext context)
        {
            var chartType = context.Request.Params["data"];

            XmlDocument xmldoc = new XmlDocument();

            //using (StreamReader streamReader = new StreamReader(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml")))
            using (StreamReader streamReader = new StreamReader(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "HomeScreen", Global.Core.ClientName, Global.User.Id + ".xml")))
            {
                xmldoc.Load(streamReader);

                XmlNode anode = xmldoc.SelectSingleNode("HomeScreen").LastChild;

                if (anode != null)
                {
                    anode.InnerXml = chartType;
                }
            }

            xmldoc.Save(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "HomeScreen", Global.Core.ClientName, Global.User.Id + ".xml"));

        }

        public void GetChartType(HttpContext context)
        {
            string chartType = null;
            XmlDocument xmldoc = new XmlDocument();

            //using (StreamReader streamReader = new StreamReader(HostingEnvironment.MapPath("~/App_Data/HomeScreen/HomeScreen.xml")))
            using (StreamReader streamReader = new StreamReader(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "HomeScreen", Global.Core.ClientName, Global.User.Id + ".xml")))
            {
                xmldoc.Load(streamReader);

                XmlNode anode = xmldoc.SelectSingleNode("HomeScreen").LastChild;

                if (anode != null)
                {
                      chartType = anode.InnerXml.Trim();
                      context.Response.Write(new JavaScriptSerializer().Serialize(new { chartType = chartType }));
                }
                  context.Response.ContentType = "text/plain";
            }

         
        }
        #endregion
    }
}