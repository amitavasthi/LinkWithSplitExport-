using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages
{
    public partial class PasswordAssistance : WebUtilities.BasePage
    {
        #region Constructor

        public PasswordAssistance()
            : base(false, false)
        { }

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["msg"] != null)
            {
                switch (Request.QueryString["msg"].Trim())
                {
                    case "1":
                        lblMsg.Visible = true;
                        lblMessage.Visible = true;
                        lblMessage.Name = "checkMail";
                        lblMsg.Name = "ResetMailSuccess";
                        lblMsg.ForeColor = System.Drawing.Color.Green;
                        break;
                    case "2":
                        lblMessage.Visible = true;
                        lblMessage.Name = "";
                        lblMsg.Visible = true;
                        lblMsg.Name = "ResetSuccess";
                        lblMsg.ForeColor = System.Drawing.Color.Green;
                        break;
                    case "3":
                        lblMessage.Visible = true;
                        lblMessage.Name = "linkExpire";
                        lblMsg.Name = "linkExpireMsg";
                        lblMsg.Visible = true;
                        break;
                   
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}