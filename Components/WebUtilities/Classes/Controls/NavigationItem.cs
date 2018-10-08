using PermissionCore.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebUtilities.Controls
{
    public class NavigationItem : BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the depth level of the navigation item.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the target of the navigation item where it points to.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the url to the icon for the navigation item.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the url to the icon for the navigation item.
        /// </summary>
        public string IconActive { get; set; }

        /// <summary>
        /// Gets or sets the css class which is used for active items.
        /// </summary>
        public string CssClassActive { get; set; }

        /// <summary>
        /// Gets or sets the name as key for the language system.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets all sub navigation items.
        /// </summary>
        public List<NavigationItem> SubNavigationItems { get; set; }

        public string LeaveMessageScript { get; set; }

        #endregion


        #region Constructor

        public NavigationItem()
        {
            this.SubNavigationItems = new List<NavigationItem>();
            this.Load += NavigationItem_Load;
        }

        #endregion


        #region Methods

        private bool IsSelected()
        {
            if (HttpContext.Current.Request.Url.PathAndQuery == this.Target)
                return true;

            // Run through all sub navigation items.
            foreach (NavigationItem subNavigationItem in this.SubNavigationItems)
            {
                if (subNavigationItem.IsSelected())
                    return true;
            }

            return false;
        }

        public NavigationItem GetSelected()
        {
            if (HttpContext.Current.Request.Url.PathAndQuery == this.Target)
                return this;

            // Run through all sub navigation items.
            foreach (NavigationItem subNavigationItem in this.SubNavigationItems)
            {
                NavigationItem temp = subNavigationItem.GetSelected();

                if (temp != null)
                    return temp;
            }

            return null;
        }

        public bool HasPermission()
        {
            if (!string.IsNullOrEmpty(this.Target))
            {
                // Get the page permission for the navigation item's target page.
                PagePermission pagePermission = base.PermissionCore.PagePermissions[this.Target.Split('?')[0]];

                // Check if there is a restriction for the navigation item's target page.
                if (pagePermission != null)
                {
                    // Check if the current's session authenticated
                    // user has the permission for this page.
                    if (!base.UserHasPermission(pagePermission.Permission.Id))
                    {
                        return false;
                    }
                }
            }
            else
            {
                bool result = false;

                foreach (NavigationItem subNavigationItem in this.SubNavigationItems)
                {
                    if (subNavigationItem.HasPermission())
                        result = true;
                }

                return result;
            }

            return true;
        }

        #endregion


        #region Event Handlers

        protected void NavigationItem_Load(object sender, EventArgs e)
        {
            if (!this.HasPermission())
            {
                // Hide the navigation item.
                this.Visible = false;

                return;
            }

            // Check if a css class for active items is set.
            if (this.CssClassActive == null || this.CssClassActive == "")
            {
                if (!this.CssClass.EndsWith("_Active"))
                    this.CssClassActive = this.CssClass + "_Active";
            }

            Table table = new Table();
            TableRow tableRow = new TableRow();

            TableCell tableCellIcon = new TableCell();
            TableCell tableCellLabel = new TableCell();

            tableCellIcon.CssClass = "TableCellIcon";
            tableCellLabel.CssClass = "TableCellLabel";

            Image icon = null;

            if (!string.IsNullOrEmpty(this.Icon))
            {
                icon = new Image();
                icon.ID = "icn" + this.Name;
                icon.ImageUrl = this.Icon;

                tableCellIcon.Controls.Add(icon);
            }

            // Check if the current page is the navigation item's target page.
            if (this.IsSelected() && this.CssClassActive != null && this.CssClassActive != "")
            {
                // Assign the active css class to the navigation item.
                this.CssClass = this.CssClassActive;

                if (!string.IsNullOrEmpty(this.IconActive) && icon != null)
                    icon.ImageUrl = this.IconActive;
            }

            table.CssClass = this.CssClass;

            tableCellLabel.Text = base.LanguageManager.GetText(this.Name);

            tableRow.Cells.Add(tableCellIcon);
            tableRow.Cells.Add(tableCellLabel);
            table.Rows.Add(tableRow);

            // Add the link button to the navigation item's control.
            this.Controls.Add(table);

            // Check if a target is set.
            if (this.Target != null && this.Target != "")
            {
                if (!string.IsNullOrEmpty(this.LeaveMessageScript))
                {
                    table.Attributes.Add("onclick", string.Format(
                        this.LeaveMessageScript,
                        "window.location = '" + this.Target + "';"
                    ));
                }
                else
                {
                    // Add the target to the link button's href attribute.
                    table.Attributes.Add("onclick", "window.location = '" + this.Target + "';");
                }
            }

            // Check if there are any sub navigation items.
            if (this.SubNavigationItems.Count != 0)
            {
                TableCell tableCellArrow = new TableCell();
                tableCellArrow.CssClass = "TableCellArrow";
                tableCellArrow.Style.Add("width", "20px");

                tableRow.Cells.Add(tableCellArrow);

                // Create a new hover box for the sub navigation items.
                Panel pnlSubNavigation = new Panel();
                pnlSubNavigation.CssClass = "SubNavigation";
                pnlSubNavigation.ID = "pnlSubNavigation" + this.Name;

                foreach (NavigationItem subNavigationItem in this.SubNavigationItems)
                {
                    subNavigationItem.LeaveMessageScript = this.LeaveMessageScript;

                    pnlSubNavigation.Controls.Add(subNavigationItem);
                }

                //this.Parent.Parent.Controls.Add(hbSubNavigation);
                this.Controls.Add(pnlSubNavigation);

                table.Attributes.Add("CssClassActive", this.CssClassActive);
                table.Attributes.Add("CssClass", this.CssClass);

                if (!this.IsSelected())
                {
                    pnlSubNavigation.Style.Add("height", "0px");
                    pnlSubNavigation.Style.Add("display", "none");

                    table.Attributes.Add("onclick", string.Format(
                        "ShowSubNavigation(this, '{0}');",
                        pnlSubNavigation.ClientID
                    ));
                }
                else
                {
                    table.Attributes.Add("state", "Expanded");

                    table.Attributes.Add("onclick", string.Format(
                        "HideSubNavigation(this, '{0}');",
                        pnlSubNavigation.ClientID
                    ));
                }
            }

            if (string.IsNullOrEmpty(this.Icon) == false && string.IsNullOrEmpty(this.IconActive) == false && this.IsSelected() == false)
            {
                this.Attributes.Add("onmouseover", string.Format(
                    "document.getElementById('{0}').src = '{1}';",
                    icon.ClientID,
                    this.IconActive
                ));

                this.Attributes.Add("onmouseout", string.Format(
                    "document.getElementById('{0}').src = '{1}';",
                    icon.ClientID,
                    this.Icon
                ));
            }
        }

        #endregion
    }
}