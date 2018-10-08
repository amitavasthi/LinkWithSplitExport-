using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseCore.Items;
using ApplicationUtilities;
using System.IO;
using System.Web.Security;

namespace LinkOnline.Pages
{
    public partial class ForgotPassword : System.Web.UI.Page
    {

        #region Methods

        #endregion
        #region Event Handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString != null)
            {
                if (Request.QueryString["msg"] == "1")
                {
                    lblMsg.Visible = true;
                    lblMsg.Name = "ResetSuccess";
                    lblMsg.ForeColor = System.Drawing.Color.Green;
                }
                if (Request.QueryString["msg"] == "2")
                {
                    lblMsg.Visible = true;
                    lblMsg.Name = "NotFound";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                }
            }
            RegularExpressionValidatorMail.ErrorMessage = Global.LanguageManager.GetText("ValidEmail");
        }
        private string GeneratePassword()
        {
            string randPassword = "";
            string allowedNames = "";
            allowedNames = "Apricot,Banana,Blackcurrant,BlackBerry,Blueberry,Boysenberry,Cherry,Coconut,Cranberry,Gooseberry,Grape,Guava,Huckleberry,Honeydew,Lemon,Lychee,Jackfruit,Mango,Melon,Mulberry,Olive,Orange,Papaya,Passionfruit,Peach,Pineapple,Raspberry,Strawberry,Sapote,Tamarind,Vanilla,Wineberry,";
            allowedNames += "Avocado,Clementine,Date,Pear,Pumpkin,Quince,Raisin,Watermelon,Fig,Kiwifruit,Lime,Nactarine,Peach,Dragonfruit,Pomegranate,Rambutan,Redcurrant,Durian,Pumpkin,WhiteMulberry";
            char[] sep = { ',' };
            string[] arr = allowedNames.Split(sep);
            Random rnd = new Random();
            int randNum = rnd.Next(0, 999);
            Random rand = new Random();
            randPassword = arr[rand.Next(0, arr.Length)] + randNum;
            return randPassword;
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMail.Text))
            {
                User user = null;
                if (Global.Core != null)
                {
                    // Check if the entered email is valid.
                    user = Global.Core.Users.GetSingle("Mail", txtMail.Text.Trim());
                    if (user != null)
                    {
                        //Global.User = user;
                        var pwdResetId = Guid.NewGuid();
                        user.SetValue("PasswordReset", pwdResetId);
                        user.SetValue("PwdCreated", DateTime.Now);
                        user.Save();

                        //string password = GeneratePassword(); //Membership.GeneratePassword(8, 2);

                        //user.Password = Global.Core.Users.GetMD5Hash(password);
                        //user.Browser = Request.Browser.Browser;
                        //user.Save();

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
                                "ForgotPassword.html"
                                ),
                            Subject = Global.LanguageManager.GetText("ForgotSubject")
                        };


                        string resetLink = "http://" + Request.Url.ToString().Split('/')[2] + "/Pages/ResetPassword.aspx?arb=" + pwdResetId;

                        // Add the placeholder value for the reset link.
                        mail.Placeholders.Add("imagepath", "http://" + Request.Url.ToString().Split('/')[2] + "/Images/Logos/link.png");
                        mail.Placeholders.Add("RequestURL", resetLink);
                        // Send the mail.
                        mail.Send(txtMail.Text.Trim());

                        Response.Redirect("PasswordAssistance.aspx?msg=1", false);
                    }
                    else
                    {
                        Response.Redirect("ForgotPassword.aspx?msg=2", false);
                    }
                }
            }
        }
        #endregion
    }
}