using DatabaseCore.Items;
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
    [ToolboxData("<{0}:Button runat=server></{0}:Button>")]
    public class Button : System.Web.UI.WebControls.Button
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]

        #region Properties

        public string Text { private get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Gets the page permission of the current page.
        /// </summary>
        public PagePermission PagePermission
        {
            get
            {
                if (this.PermissionCore == null)
                    return null;

                if (HttpContext.Current == null)
                    return null;

                return this.PermissionCore.PagePermissions[HttpContext.Current.Request.Url.LocalPath];
            }
        }

        /// <summary>
        /// Gets the current user of the session.
        /// </summary>
        public User User
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null)
                    return null;

                return this.Core.Users.GetSingle((Guid)HttpContext.Current.Session["User"]);
            }
        }

        /// <summary>
        /// Gets the database core of the web application's session.
        /// </summary>
        public DatabaseCore.Core Core
        {
            get
            {
                return (DatabaseCore.Core)HttpContext.Current.Session["Core"];
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

        #endregion


        #region Constructor

        public Button()
        {
            this.Load += new EventHandler(Button_Load);
        }

        #endregion


        #region Methods

        public void Render()
        {
            if (this.PagePermission != null)
            {
                ButtonPermission buttonPermission = this.PagePermission.ButtonPermissions[this.ID];

                if (buttonPermission != null)
                {
                    if (!this.User.HasPermission(buttonPermission.Permission.Id))
                    {
                        this.Visible = false;

                        return;
                    }
                }
            }
            
            if (this.PermissionCore != null && this.PermissionCore.ButtonPermissions[this.ID] != null)
            {
                if (!this.User.HasPermission(this.PermissionCore.ButtonPermissions[this.ID].Permission.Id))
                {
                    this.Visible = false;

                    return;
                }
            }

            if (this.ID == null)
            {
                this.ID = "LanguageButton_" + this.Name + "_" + Guid.NewGuid();
            }

            string text = ((LanguageManager)HttpContext.Current.Session["LanguageManager"]).
                GetText(((Language)HttpContext.Current.Session["Language"]), this.Name);

            base.Text = text.Replace("\n","");
        }

        #endregion


        #region Event Handlers

        protected void Button_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }
}