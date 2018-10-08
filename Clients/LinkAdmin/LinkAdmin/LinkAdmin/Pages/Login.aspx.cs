using DatabaseCore.Items;
using LinkAdmin.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUtilities;

namespace LinkAdmin.Pages
{
    public partial class Login : Classes.BaseAdminPage
    {
        #region Properties

        #endregion


        #region Constructor

        public Login()
            : base(false)
        {
            base.Methods.Add("btnLogin_Click", btnLogin_Click);
            this.Load += Login_Load;
        }

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        private void Login_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["LanguageManager"] == null)
            {
                Global.Core = new LinkAdminCore(Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "App_Data",
                    "DATA"
                ));

                // Initialize the session's language manager.
                Global.LanguageManager = new LanguageManager("", HttpContext.Current.Request.PhysicalApplicationPath);
                Global.Language = Global.LanguageManager.DefaultLanguage;

                // Initialize the session's permission core.
                Global.PermissionCore = new PermissionCore.PermissionCore(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "LinkAdmin",
                    ""
                );
            }
        }


        private void btnLogin_Click()
        {
            User user = Global.Core.Users.Valid(
                Request.Params["txtUsername"],
                Request.Params["txtPassword"]
            );

            if(user == null)
            {
                Response.Write("__ERROR__");

                return;
            }

            Global.IdUser = user.Id;

            Response.Write("/Default.aspx");
        }

        #endregion
    }
}