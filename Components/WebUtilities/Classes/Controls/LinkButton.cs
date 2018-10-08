using PermissionCore.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace WebUtilities.Controls
{
    [DefaultProperty("Name")]
    [ToolboxData("<{0}:LinkButton runat=server></{0}:LinkButton>")]
    public class LinkButton : System.Web.UI.WebControls.LinkButton
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]

        #region Properties

        public string Text { private get; set; }

        public string Name { get; set; }

        public bool AutoPostBack { get; set; }

        /// <summary>
        /// Gets the page permission of the current page.
        /// </summary>
        public PagePermission PagePermission
        {
            get
            {
                return (PagePermission)HttpContext.Current.Session["PagePermission"];
            }
        }

        /// <summary>
        /// Gets the current permission core of the session.
        /// </summary>
        public PermissionCore.PermissionCore PermissionCore
        {
            get
            {
                return (PermissionCore.PermissionCore)HttpContext.Current.Session["PermissionCore"];
            }
        }

        public bool DontOverwriteHref { get; set; }

        #endregion


        #region Constructor

        public LinkButton()
        {
            this.AutoPostBack = true;

            this.Load += new EventHandler(LinkButton_Load);
            this.PreRender += LinkButton_PreRender;
        }

        #endregion


        #region Methods

        public void SetText(string text)
        {
            base.Text = text;
        }

        #endregion


        #region Event Handlers

        protected void LinkButton_Load(object sender, EventArgs e)
        {
            // Check if the current page has a permission restriction.
            if (this.PagePermission != null)
            {/*
                ButtonPermission buttonPermission = this.PagePermission.ButtonPermissions[this.ID];

                // Check if the link button has a permission restriction.
                if (buttonPermission != null)
                {
                    // Check if the current user has the permission to see/use the link button.
                    if (!this.User.HasPermission(buttonPermission.Permission.Id))
                    {
                        // Hide the link button.
                        this.Visible = false;

                        return;
                    }
                }*/
            }
            /*
            // Check if the link button has a permission restriction.
            if (this.PermissionCore.ButtonPermissions[this.ID] != null)
            {
                // Check if the current user has the permission to see/use the link button.
                if (!this.User.HasPermission(this.PermissionCore.ButtonPermissions[this.ID].Permission.Id))
                {
                    // Hide the link button.
                    this.Visible = false;

                    return;
                }
            }*/

            // Check if the link button has an id assigned.
            if (this.ID == null)
            {
                this.ID = "LanguageLinkButton_" + this.Name + "_" + Guid.NewGuid();
            }

            string text = ((LanguageManager)HttpContext.Current.Session["LanguageManager"]).
                GetText(((Language)HttpContext.Current.Session["Language"]), this.Name);

            base.Text = text;
        }

        protected void LinkButton_PreRender(object sender, EventArgs e)
        {
            if (!this.AutoPostBack && this.DontOverwriteHref == false)
            {
                this.Attributes.Add("href", "javascript:return false;");
            }
        }

        #endregion
    }
}
