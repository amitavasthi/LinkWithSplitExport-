using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using WebUtilities.Controls;

namespace MasterPage.Classes
{
    public class Navigation : System.Web.UI.WebControls.WebControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the navigation definition file.
        /// </summary>
        public string FileName { get; set; }

        public bool RenderUser { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the navigation control.
        /// </summary>
        /// <param name="fileName">The full path to the navigation definition file.</param>
        public Navigation(string fileName, bool renderUser)
        {
            this.FileName = fileName;
            this.RenderUser = renderUser;

            this.Load += Navigation_Load;
        }

        #endregion


        #region Methods

        private NavigationItem[] ParseNavigationDefinition()
        {
            // Create a new xml document.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the content of the navigation
            // definition file into the xml document.
            xmlDocument.Load(this.FileName);

            // Get all navigation item xml nodes.
            XmlNodeList xmlNodesNavigationItems = xmlDocument.
                DocumentElement.SelectNodes("NavigationItem");

            NavigationItem[] result = new NavigationItem[xmlNodesNavigationItems.Count];

            // Run through all navigation item xml nodes on root level.
            for (int i = 0; i < result.Length; i++)
            {
                // Parse the navigation item by the xml node.
                result[i] = ParseNavigationItem(xmlNodesNavigationItems[i]);
            }

            return result;
        }

        private NavigationItem ParseNavigationItem(XmlNode xmlNode, int level = 1)
        {
            NavigationItem result = new NavigationItem();

            result.Level = level;
            result.Name = xmlNode.Attributes["Name"].Value;
            result.ID = result.Name;

            if (xmlNode.Attributes["Target"] != null)
                result.Target = xmlNode.Attributes["Target"].Value;

            if (xmlNode.Attributes["Icon"] != null)
                result.Icon = xmlNode.Attributes["Icon"].Value;

            if (xmlNode.Attributes["IconActive"] != null)
                result.IconActive = xmlNode.Attributes["IconActive"].Value;

            if (xmlNode.Attributes["CssClass"] != null)
                result.CssClass = xmlNode.Attributes["CssClass"].Value;

            if (xmlNode.Attributes["CssClassActive"] != null)
                result.CssClassActive = xmlNode.Attributes["CssClassActive"].Value;

            if (xmlNode.Attributes["LeaveMessage"] != null && bool.Parse(xmlNode.Attributes["LeaveMessage"].Value) == true)
            {
                NavigationLeaveMessage leaveMessage = new NavigationLeaveMessage(xmlNode);

                result.LeaveMessageScript = leaveMessage.RenderScript();
            }

            // Run through all sub navigation items.
            foreach (XmlNode xmlNodeSub in xmlNode.ChildNodes)
            {
                result.SubNavigationItems.Add(
                    ParseNavigationItem(xmlNodeSub, level + 1)
                );
            }

            return result;
        }

        private void UploadUserImage()
        {
            FileInfo fInfo = new FileInfo(HttpContext.Current.Request.Files[0].FileName);

            string directoryNameUserImage = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "UserImages"
            );

            if (!Directory.Exists(directoryNameUserImage))
                Directory.CreateDirectory(directoryNameUserImage);

            string fileNameUserImage = Path.Combine(
                directoryNameUserImage,
                HttpContext.Current.Session["User"] + ".png"
            );

            HttpContext.Current.Request.Files[0].SaveAs(fileNameUserImage);
        }

        #endregion


        #region Event Handlers

        protected void Navigation_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.Params["fuUserImage"] != null)
                UploadUserImage();

            // Parse the navigation definition file.
            NavigationItem[] navigationItems = ParseNavigationDefinition();

            string leaveMessageScript = "";
            // Run through all navigation items.
            foreach (NavigationItem navigationItem in navigationItems)
            {
                NavigationItem selected = navigationItem.GetSelected();

                if (selected != null)
                {
                    leaveMessageScript = selected.LeaveMessageScript;

                    break;
                }
            }

            if (this.RenderUser)
            {
                string userName = "";

                if (HttpContext.Current.Session["Core"] != null && HttpContext.Current.Session["User"] != null)
                {
                    object[] user = ((DatabaseCore.Core)HttpContext.Current.Session["Core"]).Users.GetValues(
                        new string[] { "FirstName", "LastName" },
                        new string[] {"Id" },
                        new object[] {HttpContext.Current.Session["User"] }
                    )[0];

                    userName = ((string)user[0]).ToLower() + " " + ((string)user[1]).ToLower();
                }

                Panel pnlUserInfo = new Panel();
                pnlUserInfo.CssClass = "NavigationUserInfo BackgroundColor1";

                Image imgUserInfo = new Image();
                imgUserInfo.CssClass = "ImgUserImage";
                imgUserInfo.ImageUrl = "/Images/Icons/Navigation/UserInfo.png";
                imgUserInfo.Attributes.Add("onclick", "SelectUserImage();");

                string directoryNameUserImage = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "UserImages"
                );

                if (!Directory.Exists(directoryNameUserImage))
                    Directory.CreateDirectory(directoryNameUserImage);

                string fileNameUserImage = Path.Combine(
                    directoryNameUserImage,
                    HttpContext.Current.Session["User"] + ".png"
                );

                if (File.Exists(fileNameUserImage))
                {
                    imgUserInfo.ImageUrl = string.Format(
                        "/Fileadmin/UserImages/{0}.png",
                        HttpContext.Current.Session["User"]
                    );
                }

                HtmlGenericControl lblUserInfo = new HtmlGenericControl("div");
                lblUserInfo.ID = "lblUser";
                lblUserInfo.Attributes.Add("class", "LabelUserName");
                /*lblUserInfo.Text = string.Format(
                    "<table><tr><td>{0}</td><td><img src=\"/Images/Icons/Navigation/Expand2.png\" /></td></tr></table>",
                    userName
                );*/
                lblUserInfo.InnerText = userName;

                HoverBox hoverBoxSettings = new HoverBox();
                hoverBoxSettings.ID = "hbSettings";
                hoverBoxSettings.CssClass = "BorderColor1";
                hoverBoxSettings.Display = HoverBoxDisplay.BottomCenter;
                hoverBoxSettings.TriggerMode = HoverBoxTriggerMode.Click;
                hoverBoxSettings.IdTrigger = "lblUser";
                hoverBoxSettings.IdParent = this.Parent.Parent.ID;
                hoverBoxSettings.ActivateTriggerImage = false;
                hoverBoxSettings.Level = 1;
                hoverBoxSettings.Style.Add("border-width", "1px");
                hoverBoxSettings.Style.Add("border-style", "solid");

                string fileName = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "App_Data",
                    "Settings.xml"
                );

                if (!File.Exists(fileName))
                    return;

                Navigation navigation = new Navigation(fileName, false);

                hoverBoxSettings.Controls.Add(navigation);

                pnlUserInfo.Controls.Add(imgUserInfo);
                pnlUserInfo.Controls.Add(new LiteralControl("<br />"));
                pnlUserInfo.Controls.Add(lblUserInfo);

                pnlUserInfo.Controls.Add(hoverBoxSettings);

                this.Controls.Add(pnlUserInfo);
            }

            // Run through all navigation items.
            foreach (NavigationItem navigationItem in navigationItems)
            {
                navigationItem.LeaveMessageScript = leaveMessageScript;

                // Add the navigation item to the child controls.
                this.Controls.Add(navigationItem);
            }
        }

        #endregion
    }
}
