using ApplicationUtilities;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WebUtilities.Classes;

namespace LinkOnline.Pages.ClientManager
{
    public partial class CreateUser : WebUtilities.BasePage
    {
        #region Properties
        public string password;
        protected string FileName { get; set; }
        #endregion
        #region Method

        private void RenderUserValidationFields()
        {
            UserValidation userValidation = new UserValidation(Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "UserValidation",
                Global.Core.ClientName + ".xml"
            ));

            TableRow tr;
            TableCell td;
            foreach (string field in userValidation.Fields.Keys)
            {
                tr = new TableRow();

                td = new TableCell();
                td.Text = "<span class=\"Color1\">" + Global.LanguageManager.GetText("UserValidation" + field) + "</span>";
                tr.Controls.Add(td);

                td = new TableCell();
                td.Controls.Add(userValidation.Fields[field].Render());
                tr.Controls.Add(td);

                tblContent.Controls.Add(tr);
            }
        }


        private bool SendMail()
        {
            bool send = false;
            string clientsubdomainName = "";
            string userRoleName = "";
            string templateFileName = "CMUserTemplate.html";
            string validatCode = "false";
            try
            {
                clientsubdomainName = (Request.Url.ToString().Split('/')[2]).Split('.')[0];
                userRoleName = ddlRole.SelectedItem.Text;
                FileName = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "UserValidation", clientsubdomainName + ".xml");

                if (File.Exists(FileName))
                {

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(FileName);

                    XmlElement root = xmlDocument.DocumentElement;

                    if (root.HasAttribute("EnforcePasswordChange"))
                    {
                        validatCode = root.GetAttribute("EnforcePasswordChange").ToString();
                    }

                    if ((validatCode).ToLower() == "true")
                    {
                        templateFileName = "CMUserTemplate_UserValidation.html";
                    }
                    else {
                        templateFileName = "CMUserTemplate.html";
                    }

                }
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
                        templateFileName
                        ),
                    //Subject = "account details for " + "https://" + Request.Url.ToString().Split('/')[2]
                    Subject = Global.LanguageManager.GetText("MailSubjectUserCreation") + Request.Url.ToString().Split('/')[0] + "//" + Request.Url.ToString().Split('/')[2]
                };
                // Set the full path to the mail's template file.

                // Add the placeholder value for the user's first name.

                //mail.Placeholders.Add("imagepath", "https://" + Request.Url.ToString().Split('/')[2] + "/Images/Logos/link.png");
                mail.Placeholders.Add("imagepath", Request.Url.ToString().Split('/')[0] + "//" + Request.Url.ToString().Split('/')[2] + "/Images/Logos/link.png");
                mail.Placeholders.Add("FirstName", txtFirstName.Text.Trim());
                mail.Placeholders.Add("LastName", txtLastName.Text.Trim());
                mail.Placeholders.Add("UserName", txtName.Text.Trim());
                mail.Placeholders.Add("Password", password);
                mail.Placeholders.Add("clientsubdomain", Request.Url.ToString().Split('/')[0] + "//" + Request.Url.ToString().Split('/')[2]);
                mail.Placeholders.Add("clientsubdomainName", clientsubdomainName.ToUpper());
                mail.Placeholders.Add("userRoleName", userRoleName);
                // Send the mail.               
                mail.Send(txtMail.Text.Trim());
                send = true;


            }
            catch (Exception ex)
            {
                send = false;
            }
            return send;
        }

        private bool SendConfirmMail()
        {
            bool send = false;
            try
            {
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
                        "MailSendConfirmation.html"
                    ),
                    Subject = "user creation for " + "http://" + Request.Url.ToString().Split('/')[2]
                };
                // Set the full path to the mail's template file.

                // Add the placeholder value for the user's first name.

                mail.Placeholders.Add("imagepath", "http://" + Request.Url.ToString().Split('/')[2] + "/Images/Logos/link.png");
                mail.Placeholders.Add("User", Global.User.Name);
                mail.Placeholders.Add("UserName", txtFirstName.Text.Trim());
                mail.Placeholders.Add("clientsubdomain", "http://" + Request.Url.ToString().Split('/')[2]);

                // Send the mail.
                mail.Send(Global.User.Mail.Trim());
                send = true;
            }
            catch (Exception ex)
            {
                send = false;
            }
            return send;
        }

        private void ClearFeilds()
        {
            txtName.Text = "";
            txtLastName.Text = "";
            txtFirstName.Text = "";
            txtMail.Text = "";
            txtPhone.Text = "";
            ddlRole.SelectedValue = "select";
        }

        private string GeneratePassword()
        {
            string randPassword = "";
            string allowedNames = "";
            allowedNames = "Apricot,Banana,Blackcurrant,Blackcurrant,Boysenberry,Cherry,Coconut,Cranberry,Gooseberry,Grape,Guava,Huckleberry,Honeydew,Lemon,Lychee,Jackfruit,Mango,Melon,Mulberry,Olive,Orange,Papaya,Passionfruit,Peach,Pineapple,Raspberry,Strawberry,Sapote,Tamarind,Vanilla,Wineberry,";
            allowedNames += "Avocado,Clementine,Aubergine,Beetroot,Pumpkin,Quince,Raisin,Satsuma,Endive,Watermelon,Savoy,Kiwifruit,Choko,Nactarine,Peach,Dragonfruit,Pomegranate,Rambutan,Redcurrant,Durian,Pumpkin,WhiteMulberry";
            char[] sep = { ',' };
            string[] arr = allowedNames.Split(sep);
            Random rnd = new Random();
            int randNum = rnd.Next(100, 999);
            Random rand = new Random();
            char[] chars = "*@$-+?_&=!%{}/".ToCharArray();
            randPassword = arr[rand.Next(0, arr.Length)] + chars[rand.Next(0, chars.Length)].ToString() + randNum;
            return randPassword;
        }

        private void BindUserWorkgroups(Guid idUser)
        {
            // Get all workgroups of the client.
            List<object[]> workgroups = Global.Core.Workgroups.GetValues(
                new string[] { "Id", "Name" },
                new string[] { },
                new object[] { }
            );

            // Create a new table for the workgroups.
            Table table = new Table();
            table.Style.Add("width", "100%");
            table.Style.Add("border", "2px;");

            // Run through all workgroups of the client.
            foreach (object[] workgroup in workgroups)
            {
                // Create a new table row for the workgroup.
                TableRow tableRow = new TableRow();

                TableCell tableCellCheckbox = new TableCell();
                TableCell tableCellName = new TableCell();

                tableCellCheckbox.Style.Add("padding", "5px");
                tableCellName.Style.Add("width", "100%");

                WebUtilities.Controls.CheckBox chkWorkgroup = new WebUtilities.Controls.CheckBox();
                chkWorkgroup.ID = "chkUserWorkgroup" + workgroup[0];
                chkWorkgroup.Checked = Global.Core.UserWorkgroups.GetCount(
                    new string[] { "IdUser", "IdWorkgroup" },
                    new object[] { idUser, workgroup[0] }
                ) > 0;

                tableCellCheckbox.Controls.Add(chkWorkgroup);
                tableCellName.Text = (string)workgroup[1];

                tableRow.Cells.Add(tableCellCheckbox);
                tableRow.Cells.Add(tableCellName);

                table.Rows.Add(tableRow);
            }
            if (workgroups.Count == 0)
            {
                pnlUserWorkgroups.Controls.Add(new LiteralControl(Global.LanguageManager.GetText("NoWorkGroups")));
            }
            else
            {
                pnlUserWorkgroups.Controls.Add(table);
            }


        }
        private void AssignWorkgroups(Guid idUser)
        {
            // Get all workgroups of the client.
            List<object[]> workgroups = Global.Core.Workgroups.GetValues(
                new string[] { "Id" },
                new string[] { },
                new object[] { }
            );
            //ctl00$cphContent$chkUserWorkgroup

            // Run through all workgroups of the client.
            foreach (object[] workgroup in workgroups)
            {
                if (Request.Params["ctl00$cphContent$chkUserWorkgroup" + workgroup[0]] == null)
                {
                    if (Global.Core.UserWorkgroups.GetCount(
                        new string[] { "IdWorkgroup", "IdUser" },
                        new object[] { workgroup[0], idUser }
                    ) > 0)
                    {
                        Global.Core.UserWorkgroups.Delete((Guid)Global.Core.UserWorkgroups.GetValue(
                            "Id",
                            new string[] { "IdWorkgroup", "IdUser" },
                            new object[] { workgroup[0], idUser }
                        ));
                    }

                    continue;
                }


                // Check if the user is asigned to the workgroup.
                if (Global.Core.UserWorkgroups.GetCount(
                    new string[] { "IdWorkgroup", "IdUser" },
                    new object[] { workgroup[0], idUser }
                ) == 0)
                {
                    UserWorkgroup workgroupHierarchy = new UserWorkgroup(Global.Core.UserWorkgroups);
                    workgroupHierarchy.IdWorkgroup = (Guid)workgroup[0];
                    workgroupHierarchy.IdUser = idUser;

                    workgroupHierarchy.Insert();
                }
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            RenderUserValidationFields();

            if (!IsPostBack)
            {
                User user = new User(Global.Core.Users);
                BindUserWorkgroups(user.Id);
                if (Request.QueryString["msg"] != null)
                {
                    switch (Request.QueryString["msg"].Trim())
                    {
                        case "1":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("UserAddMsg"),
                            Global.LanguageManager.GetText("UserAddMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "2":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString()),
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString())), WebUtilities.MessageType.Error);
                            break;
                        case "3":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("DuplicateError"),
                            Global.LanguageManager.GetText("DuplicateError")), WebUtilities.MessageType.Error);
                            break;
                        case "4":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("please verify the email"),
                             Global.LanguageManager.GetText("please verify the email")), WebUtilities.MessageType.Error);
                            break;
                    }
                }
                ddlRole.Items.Add(new ListItem("select", "select"));

                object idRoleUser = Global.Core.UserRoles.GetValue("IdRole", "IdUser", Global.IdUser.Value);

                if (idRoleUser == null)
                    idRoleUser = new Guid();

                foreach (Role role in Global.Core.Roles.Get())
                {
                    if (role.Hidden && role.Id != (Guid)idRoleUser)
                        continue;

                    ddlRole.Items.Add(new ListItem(role.Name, role.Id.ToString()));
                }
            }

        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                bool mailFlag = false;
                if (!(string.IsNullOrEmpty(txtName.Text) && (string.IsNullOrEmpty(txtMail.Text))))
                {
                    if (Regex.IsMatch(txtMail.Text.Trim(), @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$"))
                    {
                        var duplicateEmail = Global.Core.Users.GetSingle("Mail", txtMail.Text.Trim());

                        if (duplicateEmail != null)
                        {
                            Response.Redirect("CreateUser.aspx" + "?msg=3", false);
                        }
                        else
                        {

                            //var single = Global.Core.Users.GetSingle("Name", txtName.Text.Trim());
                            //if (single != null)
                            //{
                            //    Response.Redirect("CreateUser.aspx" + "?msg=3", false);
                            //}
                            //else
                            //{
                            //password = Membership.GeneratePassword(8, 2);
                            password = GeneratePassword();
                            // Adding User Details to the Table
                            User user = new User(Global.Core.Users)
                            {
                                Name = txtName.Text.Trim(),
                                FirstName = txtFirstName.Text.Trim(),
                                LastName = txtLastName.Text.Trim(),
                                Mail = txtMail.Text.Trim(),
                                Password = Global.Core.Users.GetMD5Hash(password),
                                //Global.Core.Users.GetMD5Hash(txtUserPassword.Text.Trim()),
                                Phone = txtPhone.Text,
                                Language = Global.Language.ToString(),
                                Browser = Request.Browser.Browser
                            };
                            user.Insert();

                            var userRole = new UserRole(Global.Core.UserRoles)
                            {
                                IdUser = user.Id,
                                IdRole = Guid.Parse(ddlRole.SelectedValue.Trim())

                            };
                            userRole.Insert();
                            AssignWorkgroups(user.Id);

                            mailFlag = SendMail();
                            if (mailFlag)
                            {
                                SendConfirmMail();
                            }

                            UserValidation userValidation = new UserValidation(Path.Combine(
                                Request.PhysicalApplicationPath,
                                "App_Data",
                                "UserValidation",
                                Global.Core.ClientName + ".xml"
                            ));

                            Dictionary<string, List<object[]>> values = Global.Core.UserValidationFieldValues.ExecuteReaderDict<string>(
                                "SELECT Field, Id FROM [UserValidationFieldValues] WHERE IdUser={0}",
                                new object[] { user.Id }
                            );
                            foreach (string field in userValidation.Fields.Keys)
                            {
                                if (Request.Params["ctl00$cphContent$ctlUserValidationField" + field] == null)
                                    continue;

                                if (values.ContainsKey(field))
                                {
                                    Global.Core.UserValidationFieldValues.SetValue(
                                        "Id=" + values[field][0][1],
                                        "Value",
                                        Request.Params["ctl00$cphContent$ctlUserValidationField" + field]
                                    );
                                }
                                else
                                {
                                    UserValidationFieldValue value = new UserValidationFieldValue(Global.Core.UserValidationFieldValues);
                                    value.IdUser = user.Id;
                                    value.Field = field;
                                    value.Value = Request.Params["ctl00$cphContent$ctlUserValidationField" + field];

                                    value.Insert();
                                }
                            }


                            ClearFeilds();
                            if (chkMultiple.Checked)
                            {
                                Response.Redirect("CreateUser.aspx" + "?msg=1", false);
                            }
                            else
                            {
                                Response.Redirect("UsersHome.aspx" + "?msg=1", false);
                            }
                            //}
                        }
                    }
                    else
                    {
                        Response.Redirect("CreateUser.aspx" + "?msg=4", false);
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message.Trim();
                Response.Redirect("CreateUser.aspx" + "?msg=2", false);
            }
        }
    }
}