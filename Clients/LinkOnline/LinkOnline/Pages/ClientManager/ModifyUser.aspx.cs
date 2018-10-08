using ApplicationUtilities;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUtilities.Classes;

namespace LinkOnline.Pages.ClientManager
{
    public partial class ModifyUser : WebUtilities.BasePage
    {

        private void RenderUserValidationFields()
        {
            UserValidation userValidation = new UserValidation(Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "UserValidation",
                Global.Core.ClientName + ".xml"
            ));

            Dictionary<string, List<object[]>> values = Global.Core.UserValidationFieldValues.ExecuteReaderDict<string>(
                "SELECT Field, Value FROM [UserValidationFieldValues] WHERE IdUser={0}",
                new object[] { Guid.Parse(Request.Params["val"]) }
            );

            TableRow tr;
            TableCell td;
            string value;
            foreach (string field in userValidation.Fields.Keys)
            {
                value = null;

                if (values.ContainsKey(field))
                    value = (string)values[field][0][1];

                tr = new TableRow();

                td = new TableCell();
                td.Text = "<span class=\"Color1\">" + Global.LanguageManager.GetText("UserValidation" + field) + "</span>";
                tr.Controls.Add(td);

                td = new TableCell();
                td.Controls.Add(userValidation.Fields[field].Render(value));
                tr.Controls.Add(td);

                //tblContent.Controls.AddAt(6, tr);
                tblContent.Controls.Add(tr);
            }
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

            pnlUserWorkgroups.Controls.Add(table);
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


        protected void Page_Load(object sender, EventArgs e)
        {
            RenderUserValidationFields();
            BindUserWorkgroups(Guid.Parse(Request.QueryString["val"]));

            if (!IsPostBack)
            {
                if (Request.QueryString["val"] != null)
                {
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

                    HttpContext.Current.Session["thisUser"] = Request.QueryString["val"];
                    HttpContext.Current.Session["Muser"] = Request.QueryString["val"];
                    var single = Global.Core.Users.GetSingle(Guid.Parse(Request.QueryString["val"].ToString()));
                    if (single != null)
                    {
                        txtName.Text = single.Name;
                        txtFirstName.Text = single.FirstName;
                        txtLastName.Text = single.LastName;
                        txtMail.Text = single.Mail;
                        txtPhone.Text = single.Phone;
                        var userRole = Global.Core.UserRoles.GetSingle("IdUser", Request.QueryString["val"].Trim());
                        ddlRole.SelectedValue = userRole != null ? userRole.IdRole.ToString() : "select";
                    }
                    else
                    {
                        Response.Redirect("ManageUsers.aspx?msg=2", false);
                    }
                }
            }

        }

        protected void btnAcceptChanges_Click(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["Muser"] != null)
            {
                try
                {
                    var oId = HttpContext.Current.Session["thisUser"];

                    AssignWorkgroups(Guid.Parse(oId.ToString()));

                    if (oId != null)
                    {
                        if (ddlRole.SelectedValue.Trim() != "select" && txtMail.Text != "")
                        {
                            UserValidation userValidation = new UserValidation(Path.Combine(
                                Request.PhysicalApplicationPath,
                                "App_Data",
                                "UserValidation",
                                Global.Core.ClientName + ".xml"
                            ));

                            Dictionary<string, List<object[]>> values = Global.Core.UserValidationFieldValues.ExecuteReaderDict<string>(
                                "SELECT Field, Id FROM [UserValidationFieldValues] WHERE IdUser={0}",
                                new object[] { Guid.Parse(oId.ToString()) }
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
                                    value.IdUser = Guid.Parse(oId.ToString());
                                    value.Field = field;
                                    value.Value = Request.Params["ctl00$cphContent$ctlUserValidationField" + field];

                                    value.Insert();
                                }
                            }


                            var oldValues = Global.Core.Users.GetSingle(Guid.Parse(oId.ToString()));
                            var roleValue = "";
                            if (oldValues.Role != null)
                            {
                                roleValue = oldValues.Role.Name.Trim();
                            }
                            if ((oldValues.Name == txtName.Text.Trim()) && (oldValues.FirstName == txtFirstName.Text.Trim()) && (oldValues.LastName == txtLastName.Text.Trim()) && (oldValues.Mail == txtMail.Text.Trim()) && (oldValues.Phone == txtPhone.Text.Trim()) && (roleValue == ddlRole.SelectedItem.Text))
                            {
                                //Response.Redirect("ManageUsers.aspx?msg=5", false);
                                Response.Redirect("ManageUsers.aspx", false);
                            }
                            else
                            {
                                var single = Global.Core.Users.GetSingle(Guid.Parse(oId.ToString()));
                                if (single != null)
                                {
                                    // Adding User Details to the Table
                                    single.Name = txtName.Text.Trim();
                                    single.FirstName = txtFirstName.Text.Trim();
                                    single.LastName = txtLastName.Text.Trim();
                                    single.Mail = txtMail.Text.Trim();
                                    single.Phone = txtPhone.Text.Trim();
                                    single.Browser = Request.Browser.Browser;
                                    single.Save();

                                    var userRole = Global.Core.UserRoles.GetSingle("IdUser", HttpContext.Current.Session["Muser"].ToString());
                                    if (userRole != null)
                                    {
                                        userRole.IdUser = single.Id;
                                        userRole.IdRole = Guid.Parse(ddlRole.SelectedValue.Trim());
                                        userRole.Save();
                                    }
                                    else
                                    {
                                        userRole = new UserRole(Global.Core.UserRoles)
                                        {
                                            IdUser = single.Id,
                                            IdRole = Guid.Parse(ddlRole.SelectedValue.Trim())

                                        };
                                        userRole.Insert();
                                    }
                                    //SendMail();

                                    //// configuration values from the web.config file.
                                    //MailConfiguration mailConfiguration = new MailConfiguration(true);
                                    //// Create a new mail by the mail configuration.
                                    //Mail mail = new Mail(mailConfiguration, Global.Core.ClientName)
                                    //{
                                    //    TemplatePath = Path.Combine(
                                    //        Request.PhysicalApplicationPath,
                                    //        "App_Data",
                                    //        "MailTemplates",
                                    //        Global.Language.ToString(),
                                    //         "ModifyOwnerTemplate.html"
                                    //        ),
                                    //    Subject = "mail from Link online team"
                                    //};
                                    //// Set the full path to the mail's template file.

                                    //// Add the placeholder value for the user's first name.
                                    //mail.Placeholders.Add("imagepath", "http://" + Request.Url.ToString().Split('/')[2] + "/Images/Logos/link.png");
                                    //mail.Placeholders.Add("FirstName", txtFirstName.Text.Trim());
                                    //mail.Placeholders.Add("LastName", txtLastName.Text.Trim());

                                    //if ((oldValues.FirstName != txtFirstName.Text.Trim()) && (oldValues.LastName != txtLastName.Text.Trim()))
                                    //{
                                    //    mail.Placeholders.Add("Message", "your first name and last name has been changed from");
                                    //    mail.Placeholders.Add("OldValue", oldValues.FirstName + " , " + oldValues.LastName);
                                    //    mail.Placeholders.Add("NewValue", txtFirstName.Text.Trim() + " , " + txtLastName.Text.Trim() + "  respectively");
                                    //}
                                    //else if ((oldValues.Mail != txtMail.Text.Trim()) && (oldValues.Phone != txtPhone.Text.Trim()))
                                    //{
                                    //    mail.Placeholders.Add("Message", "your phone number and email has been changed from");
                                    //    mail.Placeholders.Add("OldValue", oldValues.Phone + " , " + oldValues.Mail);
                                    //    mail.Placeholders.Add("NewValue", txtPhone.Text.Trim() + " , " + txtMail.Text.Trim() + "  respectively");
                                    //}
                                    //else if (oldValues.Name != txtName.Text.Trim())
                                    //{
                                    //    mail.Placeholders.Add("Message", "your user name has been changed from");
                                    //    mail.Placeholders.Add("OldValue", oldValues.Name);
                                    //    mail.Placeholders.Add("NewValue", txtName.Text.Trim());
                                    //}
                                    //else if (oldValues.FirstName != txtFirstName.Text.Trim())
                                    //{
                                    //    mail.Placeholders.Add("Message", "your first name has been changed from");
                                    //    mail.Placeholders.Add("OldValue", oldValues.FirstName);
                                    //    mail.Placeholders.Add("NewValue", txtFirstName.Text.Trim());
                                    //}
                                    //else if (oldValues.LastName != txtLastName.Text.Trim())
                                    //{
                                    //    mail.Placeholders.Add("Message", "your last name has been changed from");
                                    //    mail.Placeholders.Add("OldValue", oldValues.LastName);
                                    //    mail.Placeholders.Add("NewValue", txtLastName.Text.Trim());
                                    //}
                                    //else if (oldValues.Mail != txtMail.Text.Trim())
                                    //{
                                    //    mail.Placeholders.Add("Message", "your email has been changed from");
                                    //    mail.Placeholders.Add("OldValue", oldValues.Mail);
                                    //    mail.Placeholders.Add("NewValue", txtMail.Text.Trim());
                                    //}
                                    //else if (oldValues.Phone != txtPhone.Text.Trim())
                                    //{
                                    //    mail.Placeholders.Add("Message", "your phone number has been changed from");
                                    //    mail.Placeholders.Add("OldValue", oldValues.Phone);
                                    //    mail.Placeholders.Add("NewValue", txtPhone.Text.Trim());
                                    //}
                                    //else if (roleValue != ddlRole.SelectedItem.Text.Trim())
                                    //{
                                    //    mail.Placeholders.Add("Message", "your role has been changed from");
                                    //    mail.Placeholders.Add("OldValue", roleValue);
                                    //    mail.Placeholders.Add("NewValue", ddlRole.SelectedItem.Text.Trim());
                                    //}


                                    //mail.Placeholders.Add("clientsubdomain", "http://" + Request.Url.ToString().Split('/')[2].ToString());

                                    //// Send the mail.
                                    //mail.Send(txtMail.Text.Trim());

                                    Response.Redirect("ManageUsers.aspx?msg=1", false);
                                }
                            }
                        }
                        else
                        {
                            HttpContext.Current.Session["Error"] = "select the user role";
                            Response.Redirect("ManageUsers.aspx?msg=2", false);
                        }
                    }
                    else
                    {
                        Response.Redirect("Default.aspx", false);
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Session["Error"] = ex.Message;
                    Response.Redirect("ManageUsers.aspx?msg=2", false);
                }
            }
            else
            {
                Response.Redirect("/Pages/Login.aspx");
            }
        }
    }
}