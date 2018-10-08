using ApplicationUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseCore.Items;

namespace LinkOnline.Pages.Account
{
    public partial class Home : WebUtilities.BasePage
    {
        #region EventHandlers
        protected void Page_Load(object sender, EventArgs e)
        {
            boxPassword.Visible = false;
            boxMangeUsers.Visible = false;
            boxEmail.Visible = false;
            if (!IsPostBack)
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
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("Mismatch"),
                            Global.LanguageManager.GetText("Mismatch")), WebUtilities.MessageType.Error);
                            break;
                        case "3":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("WrongPassword"),
                            Global.LanguageManager.GetText("WrongPassword")), WebUtilities.MessageType.Error);
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
                    }
                }
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChangePassword.aspx", false);
        }

        protected void btnChangeContact_Click(object sender, EventArgs e)
        {
            if (Global.User != null)
            {
                Response.Redirect("ChangeContact.aspx", false);
            }
        }

        protected void btnChangeEmail_Click(object sender, EventArgs e)
        {
            boxEmail.Visible = true;
        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            User user = null;
            if (Global.User != null)
            {
                if (txtPassword.Text.Trim() == txtCPassword.Text.Trim())
                {
                    if (Global.Core.Users.GetMD5Hash(txtOldPassword.Text.Trim()) == Global.User.Password)
                    {
                        user = Global.User;
                        user.Password = Global.Core.Users.GetMD5Hash(txtPassword.Text.Trim());
                        user.Browser = Request.Browser.Browser;
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
                            Subject = "mail from Link online team"
                        };
                        // Set the full path to the mail's template file.

                        // Add the placeholder value for the user's first name.

                        mail.Placeholders.Add("imagepath", "http://" + Request.Url.Host.ToString() + "/Images/Logos/link.png");
                        mail.Placeholders.Add("FirstName", user.FirstName);
                        mail.Placeholders.Add("LastName", user.LastName);
                        mail.Placeholders.Add("UserName", user.Name);
                        mail.Placeholders.Add("Password", txtPassword.Text.Trim());
                        mail.Placeholders.Add("clientsubdomain", "http://" + Request.Url.Host.ToString());

                        // Send the mail.
                        mail.Send(user.Mail);

                        Response.Redirect("Home.aspx?msg=1", false);
                    }
                    else
                    {
                        Response.Redirect("Home.aspx?msg=3", false);
                    }
                }
                else
                {
                    Response.Redirect("Home.aspx?msg=2", false);
                }

            }
        }
        protected void btnAccept_Click(object sender, EventArgs e)
        {
            User user = null;
            if (Global.User != null)
            {
                user = Global.User;
                //user.FirstName = txtMFName.Text.Trim();
                //user.LastName = txtMLName.Text.Trim();
                user.Mail = txtMEmail.Text.Trim();
                user.Phone = txtMPhone.Text.Trim();
                user.Browser = Request.Browser.Browser;
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
                        "ContactChange.html"
                        ),
                    Subject = "mail from Link online team"
                };
                // Set the full path to the mail's template file.

                // Add the placeholder value for the user's first name.

                mail.Placeholders.Add("imagepath", "http://" + Request.Url.Host.ToString() + "/Images/Logos/link.png");
                mail.Placeholders.Add("Message", "Your contact details has been changed for Link Online");
                mail.Placeholders.Add("FirstName", user.FirstName);
                mail.Placeholders.Add("LastName", user.LastName);
                mail.Placeholders.Add("Email", user.Mail);
                mail.Placeholders.Add("Phone", user.Phone);
                mail.Placeholders.Add("clientsubdomain", "http://" + Request.Url.Host.ToString());

                // Send the mail.
                mail.Send(user.Mail);

                Response.Redirect("Home.aspx?msg=4", false);
            }
        }

        protected void btnEmailAccept_Click(object sender, EventArgs e)
        {
            User user = null;
            if (Global.User != null)
            {
                if (txtOldEmail.Text.Trim() == Global.User.Mail.Trim())
                {
                    user = Global.User;

                    user.Mail = txtOldEmail.Text.Trim();
                    user.Browser = Request.Browser.Browser;
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
                            "ContactChange.html"
                            ),
                        Subject = "mail from Link online team"
                    };
                    // Set the full path to the mail's template file.

                    // Add the placeholder value for the user's first name.

                    mail.Placeholders.Add("imagepath", "http://" + Request.Url.Host.ToString() + "/Images/Logos/link.png");
                    mail.Placeholders.Add("Message", "Your email has been changed for Link Online");
                    mail.Placeholders.Add("FirstName", user.FirstName);
                    mail.Placeholders.Add("LastName", user.LastName);
                    mail.Placeholders.Add("Email", user.Mail);
                    mail.Placeholders.Add("Phone", user.Phone);
                    mail.Placeholders.Add("clientsubdomain", "http://" + Request.Url.Host.ToString());

                    // Send the mail.
                    mail.Send(user.Mail);

                    Response.Redirect("Home.aspx?msg=5", false);
                }
                else
                {
                    Response.Redirect("Home.aspx?msg=6", false);
                }
            }
        }

    }
        #endregion
}