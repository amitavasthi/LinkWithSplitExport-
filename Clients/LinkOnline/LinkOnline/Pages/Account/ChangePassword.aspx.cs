using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseCore.Items;
using ApplicationUtilities;
using System.IO;
using System.Text.RegularExpressions;

namespace LinkOnline.Pages
{
    public partial class ChangePassword : WebUtilities.BasePage
    {
        #region EventHandlers
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
                            Global.LanguageManager.GetText("OldPasswordMatchesNewPassword"),
                            Global.LanguageManager.GetText("OldPasswordMatchesNewPassword")), WebUtilities.MessageType.Error);
                            break;
                        case "5":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("PasswordPolicy"),
                            Global.LanguageManager.GetText("PasswordPolicy")), WebUtilities.MessageType.Error);
                            break;
                    }
                }

                PasswordRegularExpressionValidator.ErrorMessage = Global.LanguageManager.GetText("PasswordPolicy");//Global.LanguageManager.GetText("PasswordError");
                CPasswordRegularExpressionValidator.ErrorMessage = Global.LanguageManager.GetText("PasswordPolicy");
            }
        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            User user = null;
            if (Global.User != null)
            {
                if (CheckRegEx(txtCPassword.Text.Trim()) || CheckRegEx(txtPassword.Text.Trim()))
                {
                    if (txtPassword.Text.Trim() == txtCPassword.Text.Trim())
                    {
                        if (Global.Core.Users.GetMD5Hash(txtOldPassword.Text.Trim()) == Global.User.Password)
                        {
                            if (Global.Core.Users.GetMD5Hash(txtOldPassword.Text.Trim()) == Global.Core.Users.GetMD5Hash(txtPassword.Text.Trim()))
                            {
                                Response.Redirect("ChangePassword.aspx?msg=4", false);
                            }
                            else
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
                                    TemplatePath = Path.Combine(Request.PhysicalApplicationPath,
                                        "App_Data", "MailTemplates", Global.Language.ToString(), "PasswordChange.html"),
                                    Subject = "mail from Link online team"
                                };
                                mail.Placeholders.Add("imagepath", "http://" + Request.Url.ToString().Split('/')[2].ToString() + "/Images/Logos/link.png");
                                mail.Placeholders.Add("FirstName", user.FirstName);
                                mail.Placeholders.Add("LastName", user.LastName);
                                mail.Placeholders.Add("UserName", user.Name);
                                mail.Placeholders.Add("Password", txtPassword.Text.Trim());
                                mail.Placeholders.Add("clientsubdomain", "http://" + Request.Url.ToString().Split('/')[2].ToString());
                                mail.Send(user.Mail);
                                Response.Redirect("Home.aspx?msg=1", false);
                            }
                        }
                        else
                        {
                            Response.Redirect("ChangePassword.aspx?msg=3", false);
                        }
                    }
                    else
                    {
                        Response.Redirect("ChangePassword.aspx?msg=2", false);
                    }
                }
                else
                {
                    Response.Redirect("ChangePassword.aspx?msg=5", false);
                }

            }
        }
        #endregion

        #region Method
        private bool CheckRegEx(string password)
        {
            string regex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[*@$-+?_&=!%{}/])[A-Za-z\d*@$-+?_&=!%{}/]{8,32}"; ;
            var match = Regex.Match(password, regex);
            return match.Success;
        }
        #endregion
    }
}