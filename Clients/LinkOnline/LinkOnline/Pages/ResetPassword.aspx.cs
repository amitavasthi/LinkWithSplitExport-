using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseCore.Items;
using ApplicationUtilities;
using System.IO;
using System.Configuration;

namespace LinkOnline.Pages
{
    public partial class ResetPassword : WebUtilities.BasePage
    {
        #region Properties
        Guid resetGuid;
        #endregion
        #region Constructor

        public ResetPassword()
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
                        base.ShowMessage(string.Format(
                        Global.LanguageManager.GetText("ChangeMessage"),
                        Global.LanguageManager.GetText("ChangeMessage")), WebUtilities.MessageType.Success);
                        break;
                    case "2":
                        lblMsg.Name = "Mismatch";
                        lblMsg.Visible = true;
                        break;
                    case "3":
                        lblMsg.Name = Global.LanguageManager.GetText("PasswordRequestExpired");
                        lblMsg.Visible = true;
                        btnLogin.Enabled = false;
                        break;
                    case "4":
                        base.ShowMessage(string.Format(
                        Global.LanguageManager.GetText("ContactSuccess"),
                        Global.LanguageManager.GetText("ContactSuccess")), WebUtilities.MessageType.Success);
                        break;
                    case "5":
                        base.ShowMessage(string.Format(
                        Global.LanguageManager.GetText("MailSuccess"),
                        Global.LanguageManager.GetText("MailSuccess")), WebUtilities.MessageType.Success);
                        break;
                    case "6":
                        base.ShowMessage(string.Format(
                        Global.LanguageManager.GetText("MailError"),
                        Global.LanguageManager.GetText("MailError")), WebUtilities.MessageType.Error);
                        break;
                    case "7":
                        lblMsg.Visible = true;
                        lblMsg.Name = Global.LanguageManager.GetText("OldPasswordMatchesNewPassword");
                        break;
                }
            }

            PasswordRegularExpressionValidator.ErrorMessage = Global.LanguageManager.GetText("PasswordPolicy");//Global.LanguageManager.GetText("PasswordError");
            CPasswordRegularExpressionValidator.ErrorMessage = Global.LanguageManager.GetText("PasswordPolicy"); //Global.LanguageManager.GetText("PasswordError");
            // Check if the session is corrupt (Google Chrome issue)
            if (Global.ClientManager == null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();

                Response.Redirect(
                    "/Pages/Login.aspx"
                );
            }

            // Check if the database core of the current session is initialized.
            if (Global.Core == null || Global.Core.ClientName != Request.Url.Host.Split('.')[0])
            {
                string connectionString = string.Format(
                    ConfigurationManager.AppSettings["ConnectionString"],
                    Global.ClientManager.GetDatabaseName(Request.Url.Host.Split('.')[0])
                );

                // Create a new database core for the session.
                Global.Core = new DatabaseCore.Core(
                    ConfigurationManager.AppSettings["DatabaseProvider"],
                    connectionString,
                    ConfigurationManager.AppSettings["DatabaseProviderUserManagement"],
                    connectionString,
                    new string[0]
                );

                // Set the database core's file storage path.
                Global.Core.FileStorageRoot = string.Format(
                    ConfigurationManager.AppSettings["FileStorageRoot"],
                    Request.Url.Host.Split('.')[0]
                );

                // Initialize the session's language manager.
                Global.LanguageManager = new WebUtilities.LanguageManager(
                    Request.Url.Host.Split('.')[0],
                    Request.PhysicalApplicationPath
                );

                if (!Directory.Exists(Global.Core.FileStorageRoot))
                    Directory.CreateDirectory(Global.Core.FileStorageRoot);

                Global.Core.LogDirectory = ConfigurationManager.AppSettings["DatabaseChangeLogDirectory"];

                Global.Core.ClientName = Request.Url.Host.Split('.')[0];
                Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(Global.Core.ClientName).CaseDataVersion;
                Global.Core.CaseDataLocation = Global.ClientManager.GetSingle(Global.Core.ClientName).CaseDataLocation;
            }

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["arb"] != null)
            {
                resetGuid = Guid.Parse(Request.QueryString["arb"]);
                User user = null;

                if (Global.Core != null)
                {
                    user = Global.Core.Users.GetSingle("PasswordReset", resetGuid);
                    if (user != null)
                    {
                        if (user.GetValue("PwdCreated") != null)
                        {
                            if (DateTime.Now > DateTime.Parse(user.GetValue("PwdCreated").ToString()).AddHours(24))
                            {
                                //lblMsg.Visible = true;
                                //btnLogin.Enabled = false;
                                //lblMsg.Name = "this request has been either expired or not found, please make a new request or contact administrator";
                                Response.Redirect("ResetPassword.aspx?msg=3&arb=" + resetGuid, false);
                            }
                            else
                            {
                                if (txtPassword.Text.Trim() == txtCPassword.Text.Trim())
                                {
                                    if (user.Password == Global.Core.Users.GetMD5Hash(txtPassword.Text.Trim()))
                                    {
                                        Response.Redirect("ResetPassword.aspx?msg=7&arb=" + resetGuid, false);
                                    }
                                    else
                                    {
                                        user.Password = Global.Core.Users.GetMD5Hash(txtPassword.Text.Trim());
                                        user.SetValue("PasswordReset", DBNull.Value);
                                        user.SetValue("PwdCreated", DBNull.Value);
                                        user.Save();
                                        // configuration values from the web.config file.
                                        MailConfiguration mailConfiguration = new MailConfiguration(true);
                                        // Create a new mail by the mail configuration.
                                        Mail mail = new Mail(mailConfiguration, Global.Core.ClientName)
                                        {
                                            TemplatePath = Path.Combine(
                                                Request.PhysicalApplicationPath,
                                                "App_Data",
                                                "MailTemplates",
                                                Global.Language.ToString(),
                                                "PasswordChange.html"
                                                ),
                                            Subject = Global.LanguageManager.GetText("ForgotSubject")
                                        };

                                        // Add the placeholder value for the reset link.
                                        mail.Placeholders.Add("FirstName", user.FirstName);
                                        mail.Placeholders.Add("LastName", user.LastName);
                                        mail.Placeholders.Add("imagepath", "http://" + Request.Url.ToString().Split('/')[2] + "/Images/Logos/link.png");
                                        mail.Placeholders.Add("clientsubdomain", "http://" + Request.Url.ToString().Split('/')[2]);
                                        // Send the mail.
                                        mail.Send(user.Mail);
                                        Response.Redirect("PasswordAssistance.aspx?msg=2", false);
                                    }
                                }
                                else
                                {
                                    Response.Redirect("ResetPassword.aspx?msg=2&arb=" + resetGuid, false);
                                }
                            }
                        }
                        else
                        {
                            Response.Redirect("PasswordAssistance.aspx?msg=3", false);
                        }
                    }
                    else
                    {
                        Response.Redirect("PasswordAssistance.aspx?msg=3", false);
                    }
                }
                else
                {
                    Response.Redirect("PasswordAssistance.aspx?msg=3", false);
                }
            }

        }


    }
}