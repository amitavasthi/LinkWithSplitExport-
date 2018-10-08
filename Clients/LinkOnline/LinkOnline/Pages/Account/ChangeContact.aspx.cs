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
    public partial class ChangeContact : WebUtilities.BasePage
    {
        #region EventHnadlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["msg"] != null)
                {
                    switch (Request.QueryString["msg"].Trim())
                    {
                        case "1":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("ChangeMsgWarning"),
                            Global.LanguageManager.GetText("ChangeMsgWarning")), WebUtilities.MessageType.Warning);
                            break;
                    }
                }
                if (Global.User != null)
                {
                    //txtMEmail.Text = Global.User.Mail;
                    txtMPhone.Text = Global.User.Phone;
                }
            }
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            User user = null;
            if (Global.User != null)
            {
                user = Global.User;
                if (user.Phone != txtMPhone.Text.Trim())
                {
                    //user.Mail = txtMEmail.Text.Trim();
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

                    mail.Placeholders.Add("imagepath", "http://" + Request.Url.ToString().Split('/')[2].ToString() + "/Images/Logos/link.png");
                    mail.Placeholders.Add("Message", "Your contact details has been changed for Link Online");
                    mail.Placeholders.Add("FirstName", user.FirstName);
                    mail.Placeholders.Add("LastName", user.LastName);
                    mail.Placeholders.Add("Email", user.Mail);
                    mail.Placeholders.Add("Phone", user.Phone);
                    mail.Placeholders.Add("clientsubdomain", "http://" + Request.Url.ToString().Split('/')[2].ToString());

                    // Send the mail.
                    mail.Send(user.Mail);

                    Response.Redirect("Home.aspx?msg=4", false);
                }
                else
                {
                    Response.Redirect("ChangeContact.aspx?msg=1", false);
                }
            }
        }
        #endregion
    }
}