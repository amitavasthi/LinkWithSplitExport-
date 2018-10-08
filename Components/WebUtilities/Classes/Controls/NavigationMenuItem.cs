using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Xml;
using WebUtilities.Controls;

namespace WebUtilities
{
    public class NavigationMenuItem : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning navigation
        /// menu where the item is part of.
        /// </summary>
        public NavigationMenu Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node that
        /// contains the item's definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets in which row the menu item is displayed.
        /// </summary>
        public int Row
        {
            get
            {
                return (int)this.GetValue<int>("Row");
            }
        }

        /// <summary>
        /// Gets in which position in the row the menu item is displayed.
        /// </summary>
        public int Position
        {
            get
            {
                return (int)this.GetValue<int>("Position");
            }
        }

        /// <summary>
        /// Gets in which position in the row the menu item is displayed.
        /// </summary>
        public string Name
        {
            get
            {
                return (string)this.GetValue<string>("Name");
            }
        }

        /// <summary>
        /// Gets or sets the parent navigation menu item.
        /// </summary>
        public NavigationMenuItem Parent { get; set; }

        public bool HasPermission
        {
            get
            {
                // Check if a target page is defined for the menu item.
                if (this.XmlNode.Attributes["Target"] != null)
                {
                    if (!base.HasPagePermission(this.XmlNode.Attributes["Target"].Value))
                    {
                        return false;
                    }
                }
                else if (this.XmlNode.Attributes["OnClick"] == null && this.XmlNode.Attributes["IdContentPanel"] == null)
                {
                    int count = 0;

                    foreach (XmlNode xmlNode in this.XmlNode.ChildNodes)
                    {
                        // Create a new menu item by the definition xml node.
                        NavigationMenuItem item = new NavigationMenuItem(
                            this.Owner,
                            xmlNode,
                            this
                        );

                        if (item.HasPermission)
                            count++;
                    }

                    if (count == 0)
                        return false;
                }

                return true;
            }
        }

        #endregion


        #region Constructor

        public NavigationMenuItem(NavigationMenu owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;

            this.ID = owner.ID + this.Name;
        }

        public NavigationMenuItem(NavigationMenu owner, XmlNode xmlNode, NavigationMenuItem parent)
            : this(owner, xmlNode)
        {
            this.Parent = parent;
        }

        #endregion


        #region Methods

        public void Render()
        {
            if (!this.HasPermission)
                return;

            // Set the css class name of the menu item.
            base.CssClass = "NavigationMenuItem _BackgroundColor7";

            base.Attributes.Add("Row", this.Row.ToString());
            base.Attributes.Add("Position", this.Position.ToString());

            // Check if a icon for the menu item is defined.
            if (this.XmlNode.Attributes["Icon"] != null)
            {
                // Create a new image web control to
                // display the menu item's icon.
                Image imgIcon = new Image();

                imgIcon.Height = 60;

                // Set the image's source to the
                // defined source for the menu item.
                imgIcon.ImageUrl = this.XmlNode.Attributes["Icon"].Value;

                // Add the icon image to the menu
                // item control's child controls.
                base.Controls.Add(imgIcon);
            }

            // Add a line break between the image and the name.
            base.Controls.Add(new LiteralControl("<br />"));

            // Create a new label for the menu item's name.
            System.Web.UI.WebControls.Label lblName = new System.Web.UI.WebControls.Label();

            // Check if a name for the menu item is defined.
            if (this.XmlNode.Attributes["Name"] != null)
            {
                // Set the language label's language key
                // to the menu item's defined name.
                lblName.Text = base.LanguageManager.GetText(this.XmlNode.Attributes["Name"].Value);
            }

            // Create a new panel for the menu items sub menu items.
            Panel pnlSubMenuItems = new Panel();
            pnlSubMenuItems.ID = this.ID + "pnlSubMenuItems";

            // Select all menu item definition xml nodes
            // of the menu item's sub menu items.
            XmlNodeList xmlNodes = this.XmlNode.SelectNodes("MenuItem");

            // Run through all menu item definition xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                // Create a new menu item by the definition xml node.
                NavigationMenuItem item = new NavigationMenuItem(
                    this.Owner,
                    xmlNode,
                    this
                );

                // Hide the sub menu item.
                item.Style.Add("display", "none");

                // Add the menu item control to the
                // child controls panel controls.
                pnlSubMenuItems.Controls.Add(item);

                item.Render();
            }

            // Add the language label to the menu
            // item control's child controls.
            base.Controls.Add(lblName);

            base.Controls.Add(pnlSubMenuItems);

            // Check if a target page is defined for the menu item.
            if (this.XmlNode.Attributes["Target"] != null)
            {
                base.Attributes.Add("onclick", string.Format(
                    "window.location = '{0}';",
                    this.XmlNode.Attributes["Target"].Value
                ));

                if (this.Parent != null && HttpContext.Current.Request.Url.ToString().Contains(this.XmlNode.Attributes["Target"].Value))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowNavigationMenuSubItems", string.Format(
                        "ShowNavigationMenuSubItems(document.getElementById('{0}'));",
                        this.Parent.ClientID
                    ), true);
                }
            }
            else if (this.XmlNode.Attributes["OnClick"] != null)
            {
                base.Attributes.Add("onclick", this.XmlNode.Attributes["OnClick"].Value);
            }
            else
            {
                base.Attributes.Add("onclick", string.Format(
                    "ShowNavigationMenuSubItems(this);"
                ));
            }

            this.Attributes.Add("IdTable", this.Owner.ClientID + "Table");

            if (this.Parent != null)
            {
                // Set the id of the menu item as parent menu item.
                this.Attributes.Add("IdParent", this.Parent.ClientID);
            }

            if (this.XmlNode.Attributes["IdContentPanel"] != null)
            {
                this.Attributes.Add("IdContentPanel", this.XmlNode.Attributes["IdContentPanel"].Value);
            }
        }


        private object GetValue<T>(string name)
        {
            // Check if a value with this key exists.
            if (this.XmlNode.Attributes[name] == null)
                return null;

            // Get the value as string from the xml node's attributes.
            string resultStr = this.XmlNode.Attributes[name].Value;

            object result = resultStr;

            // Switch on the result type.
            switch (typeof(T).Name)
            {
                case "Int32":
                    result = int.Parse(resultStr);
                    break;
            }

            return result;
        }

        #endregion


        #region Event Handlers

        #endregion
    }
}
