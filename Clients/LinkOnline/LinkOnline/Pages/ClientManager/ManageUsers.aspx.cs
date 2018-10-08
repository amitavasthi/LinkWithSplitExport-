using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using ApplicationUtilities;
using DatabaseCore.Items;
using WebUtilities.Classes.Controls.GridClasses;
using WebUtilities.Controls;
using System.Web.Security;
using WebUtilities.Classes;

namespace LinkOnline.Pages.ClientManager
{
    public partial class ManageUsers : WebUtilities.BasePage
    {
        #region Properties
        private Grid gridUsers;
        #endregion


        #region Methods
        private string PrepareRoleName(string name)
        {
            string result = name;

            int c = 0;
            for (int i = 0; i < name.Length; i++)
            {
                if (i != 0 && i % 15 == 0)
                {
                    result = result.Insert(i + c, "<br />");

                    c += 6;
                }
            }

            return result;
        }

        private void CreateGridUsers()
        {
            try
            {
                gridUsers = new Grid { ID = "gridUsers" };

                var headline = new GridHeadline(gridUsers);
                headline.Items.Add(new GridHeadlineItem(headline, 0, "name", new GridHeadlineItemWidth(15)));
                headline.Items.Add(new GridHeadlineItem(headline, 1, "first name", new GridHeadlineItemWidth(20)));
                headline.Items.Add(new GridHeadlineItem(headline, 2, "last name", new GridHeadlineItemWidth(20)));
                headline.Items.Add(new GridHeadlineItem(headline, 3, "email", new GridHeadlineItemWidth(20)));
                headline.Items.Add(new GridHeadlineItem(headline, 4, "phone", new GridHeadlineItemWidth(15)));
                headline.Items.Add(new GridHeadlineItem(headline, 5, "user group", new GridHeadlineItemWidth(10)));
                gridUsers.GridHeadline = headline;

                var users = Global.Core.Users.Get();

                if (users != null)
                {
                    foreach (var useritem in users)
                    {
                        var userRole = Global.Core.UserRoles.GetSingle("IdUser", useritem.Id);
                        var role = Global.Core.Roles.GetSingle("Id", userRole.IdRole);

                        object idRoleUser = Global.Core.UserRoles.GetValue("IdRole", "IdUser", Global.IdUser.Value);

                        if (idRoleUser == null)
                            idRoleUser = new Guid();

                        if (role.Hidden && (Guid)idRoleUser != role.Id)
                            continue;

                        var row = new GridRow(gridUsers, useritem.Id);
                        var name = new GridRowItem(row, useritem.Name);
                        var fName = new GridRowItem(row, useritem.FirstName);
                        var lName = new GridRowItem(row, useritem.LastName);
                        var eMail = new GridRowItem(row, useritem.Mail);
                        var phone = new GridRowItem(row, useritem.Phone);
                        row.Items.Add(name);
                        row.Items.Add(fName);
                        row.Items.Add(lName);
                        row.Items.Add(eMail);
                        row.Items.Add(phone);

                        if (userRole != null)
                        {
                            var itemrole = new GridRowItem(row, role.Name);
                            row.Items.Add(itemrole);
                        }
                        else
                        {
                            var itemrole = new GridRowItem(row, "not assigned");
                            row.Items.Add(itemrole);
                        }

                        gridUsers.Rows.Add(row);
                    }
                    pnlUserManagement.Controls.Add(gridUsers);
                }

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("/Pages/ErrorPage.aspx");
            }

        }

        private void ClearFeilds()
        {
            lblAppError.Name = "";
            lblMsg.Name = "";
        }
        #endregion


        #region EventHandlers
        protected void Page_Load(object sender, EventArgs e)
        {
            CreateGridUsers();

            if (!IsPostBack)
            {
                if (Request.QueryString["msg"] != null)
                {
                    switch (Request.QueryString["msg"].Trim())
                    {
                        case "1":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("UserModifyMsg"),
                            Global.LanguageManager.GetText("UserModifyMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "2":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString().Trim()),
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString().Trim())), WebUtilities.MessageType.Error);
                            break;
                        case "3":
                            base.ShowMessage(string.Format(
                           Global.LanguageManager.GetText("UserDeleteErrMsg"),
                           Global.LanguageManager.GetText("UserDeleteErrMsg")), WebUtilities.MessageType.Error);
                            break;
                        case "4":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("UserDeleteMsg"),
                            Global.LanguageManager.GetText("UserDeleteMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "5":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("NoChangeMsg"),
                            Global.LanguageManager.GetText("NoChangeMsg")), WebUtilities.MessageType.Error);
                            break;
                        case "6":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("UserModifyErrMsg"),
                            Global.LanguageManager.GetText("UserModifyErrMsg")), WebUtilities.MessageType.Error);
                            break;
                        case "7":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("ReminderMsg"),
                            Global.LanguageManager.GetText("ReminderMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "8":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("DuplicateError"),
                            Global.LanguageManager.GetText("DuplicateError")), WebUtilities.MessageType.Error);
                            break;
                    }
                }

            }

        }

        protected void btnCreateUser_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("CreateUser.aspx");
        }

        protected void btnModify_OnClick(object sender, EventArgs e)
        {
            if (gridUsers.SelectedItem != null)
            {
                Response.Redirect("ModifyUser.aspx?val=" + gridUsers.SelectedItem, false);
            }
            else
            {
                ClearFeilds();
                HttpContext.Current.Session["Error"] = "page load error, please try later.";
                Response.Redirect("ManageUsers.aspx?msg=2", false);
            }
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {

            if (gridUsers.SelectedItem != null)
            {
                /*prevent the logged in User and the delete User are same*/
                if (gridUsers.SelectedItem.ToString() != Global.User.Id.ToString())
                {
                    Guid userId = Guid.Parse(gridUsers.SelectedItem.ToString());
                    cbRemove.Visible = true;
                    cbRemove.Text = string.Format(
                        Global.LanguageManager.GetText("RemoveUserConfirmMessage")
                    );

                    var userRole = Global.Core.UserRoles.Get("IdUser", userId);
                    if (userRole != null)
                    {
                        Guid userRoleId = userRole[0].Id;

                        cbRemove.Confirm = delegate()
                        {
                            Global.Core.QALogs.ExecuteQuery(string.Format(
                               "DELETE FROM QALogs WHERE IdUser='{0}';",
                               userId
                           ));
                            Global.Core.UserWorkgroups.ExecuteQuery(string.Format(
                                "DELETE FROM UserWorkgroups WHERE IdUser='{0}';",
                                userId
                            ));

                            Global.Core.Users.Delete(userId);
                            Global.Core.UserRoles.Delete(userRoleId);
                            if (gridUsers != null)
                            {
                                gridUsers.SelectedItem = gridUsers.Rows[0].Identity;
                            }
                            HttpContext.Current.Response.Redirect("ManageUsers.aspx?msg=4", false);
                        };
                    }
                }
                else
                {
                    HttpContext.Current.Response.Redirect("ManageUsers.aspx?msg=3", false);
                }
            }

        }


        protected void btnReminder_Click(object sender, EventArgs e)
        {
            if (gridUsers.SelectedItem != null)
            {
                var userDetails = Global.Core.Users.Get("id", gridUsers.SelectedItem.ToString());
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
                         "ReminderMail.html"
                        ),
                    Subject = Global.LanguageManager.GetText("MailSubject")
                };

                mail.Placeholders.Add("imagepath", "http://" + Request.Url.ToString().Split('/')[2] + "/Images/Logos/link.png");
                mail.Placeholders.Add("FirstName", userDetails[0].FirstName);
                mail.Placeholders.Add("LastName", userDetails[0].LastName);
                mail.Placeholders.Add("URL", "http://" + Request.Url.ToString().Split('/')[2].Trim());
                mail.Placeholders.Add("Username", userDetails[0].Name.Trim());

                // Send the mail.
                mail.Send(userDetails[0].Mail.Trim());

                Response.Redirect("ManageUsers.aspx?msg=7", false);
            }
        }

        #endregion
    }
}