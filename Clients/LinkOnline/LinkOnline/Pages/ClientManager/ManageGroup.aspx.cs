using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.ClientManager
{
    public partial class ManageGroup : WebUtilities.BasePage
    {
        #region Properties
        public bool flag = false;
        #endregion

        #region Methods
        private string PrepareRoleName(string name)
        {
            string result = name;

            int c = 0;
            for (int i = 0; i < name.Length; i++)
            {
                if (i != 0 && i % 18 == 0)
                {
                    result = result.Insert(i + c, "<br />");

                    c += 6;
                }
            }

            return result;
        }

        private void GenereateUserGroups(string selectedRole)
        {
            pnlModuleDetails.Controls.Clear();

            if (selectedRole != null)
            {
                if (Global.PermissionCore.Sections.Items != null)
                {
                    var results = Global.Core.Roles.Get().OrderBy(s => s.Name);
                    var roles = results.Where(x => x.Name == selectedRole);
                    // var results = listData.Where((item, index) => listFilter[index] == 1);
                    if (roles != null)
                    {
                        object idRoleUser = Global.Core.UserRoles.GetValue("IdRole", "IdUser", Global.IdUser.Value);

                        if (idRoleUser == null)
                            idRoleUser = new Guid();

                        foreach (Role role in roles)
                        {
                            if (role.Hidden && role.Id != (Guid)idRoleUser)
                                continue;

                            WebUtilities.Controls.Table tblSection = new WebUtilities.Controls.Table();

                            tblSection.ID = "tblRole";//"tbl" + role.Name;
                            tblSection.CssClass = "Userroletable";
                            tblSection.CellPadding = 0;
                            tblSection.CellSpacing = 0;
                            //TableHeaderRow hrow = new TableHeaderRow();

                            //TableCell hCell1 = new TableCell();
                            //hCell1.ID = "hCell_" + role.Name;
                            //hCell1.Width = new Unit(150, UnitType.Pixel);
                            //hCell1.CssClass = "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor8";
                            //hCell1.Text = PrepareRoleName(role.Name.ToLower());
                            //hrow.Cells.Add(hCell1);
                            //tblSection.Rows.Add(hrow);

                            foreach (PermissionCore.Classes.Section sec in Global.PermissionCore.Sections.Items)
                            {
                                TableRow trSec = new WebUtilities.Controls.TableRow();
                                TableCell secEmpty = new TableCell();
                                secEmpty.ID = "secEmpty_" + role.Name + sec.Name;
                                secEmpty.Width = new Unit(150, UnitType.Pixel);
                                secEmpty.CssClass = "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor8 TableCellPercentage";
                                secEmpty.Text = PrepareRoleName(Global.LanguageManager.GetText(sec.Name).ToLower());
                                trSec.Cells.Add(secEmpty);

                                int cnt = 0;
                                foreach (PermissionCore.Classes.Permission permission in sec.Permissions)
                                {

                                    if (!Global.User.HasPermission(permission.Id) && Global.User.HasPermission(1001) == false)
                                        continue;
                                    if (!(permission.Id == 1001) && !(permission.Id == 1000))
                                    {
                                        TableCell tc = new TableCell
                                        {
                                            ID = "tc" + permission.Name.ToLower() + "_" + role.Name,
                                            Text = PrepareRoleName(Global.LanguageManager.GetText(permission.Name).ToLower()),
                                            Width = new Unit(150, UnitType.Pixel),
                                            CssClass =
                                                cnt % 2 == 0
                                                    ? "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5"
                                                    : "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor9"
                                        };
                                        trSec.Cells.Add(tc);
                                        cnt++;
                                    }
                                    tblSection.Rows.Add(trSec);
                                }


                                //  var roles = Global.Core.Roles.Get().OrderBy(s => s.Name);


                                //RolePermission rolePermission = Global.Core.RolePermissions.GetSingle("IdRole", role.Id);
                                //TableRow tr = new WebUtilities.Controls.TableRow();
                                //TableCell td1 = new TableCell
                                //{
                                //    ID = "td_" + sec.Name + "_" + role.Name,
                                //    Width = new Unit(150, UnitType.Pixel),
                                //    Text = PrepareRoleName(sec.Name)
                                //};
                                //td1.Font.Bold = true;
                                //td1.CssClass =
                                //    "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor8";
                                //tr.Cells.Add(td1);
                                TableRow tr = new WebUtilities.Controls.TableRow();
                                TableCell tdSec = new TableCell();
                                tdSec.ID = "tdSec_" + sec.Name + role.Name;
                                tdSec.Width = new Unit(150, UnitType.Pixel);
                                tdSec.CssClass = "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor7 TableCellPercentage";
                                tr.Cells.Add(tdSec);

                                int count = 0;
                                foreach (PermissionCore.Classes.Permission detailPermission in sec.Permissions)
                                {

                                    if (!Global.User.HasPermission(detailPermission.Id) && Global.User.HasPermission(1001) == false)
                                        continue;

                                    if (!(detailPermission.Id == 1001) && !(detailPermission.Id == 1000))
                                    {
                                        TableCell cell = new TableCell
                                        {
                                            ID = "td" + detailPermission.Name + role.Id,
                                            Width = new Unit(150, UnitType.Pixel),
                                            Text = PrepareRoleName(role.Name.ToLower()),
                                            CssClass = count % 2 == 0
                                                ? "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor9"
                                                : "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5"
                                        };

                                        WebUtilities.Controls.CheckBox checkBox = new WebUtilities.Controls.CheckBox
                                        {
                                            ID = "chk_" + role.Id + "_" + detailPermission.Id
                                        };

                                        RolePermission dbPermission =
                                            Global.Core.RolePermissions.GetSingle(new string[] { "IdRole", "Permission" },
                                                new object[] { role.Id, detailPermission.Id });

                                        checkBox.Checked = dbPermission != null;

                                        cell.Controls.Add(checkBox);
                                        tr.Cells.Add(cell);
                                        count++;
                                    }
                                }
                                tblSection.Rows.Add(tr);



                                pnlModuleDetails.Controls.Add(tblSection);
                            }
                        }
                    }
                }
            }
        }


        private void GenerateUserRoleDetails()
        {
            if (Global.PermissionCore.Sections.Items != null)
            {
                foreach (PermissionCore.Classes.Section sec in Global.PermissionCore.Sections.Items)
                {
                    WebUtilities.Controls.Table tblSection = new WebUtilities.Controls.Table();

                    tblSection.ID = "tbl" + sec.Name;
                    tblSection.CssClass = "Userroletable";
                    tblSection.CellPadding = 0;
                    tblSection.CellSpacing = 0;
                    TableHeaderRow hrow = new TableHeaderRow();

                    TableCell hCell1 = new TableCell();
                    hCell1.ID = "hCell_" + sec.Name;
                    hCell1.Width = new Unit(150, UnitType.Pixel);
                    hCell1.CssClass = "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor9 TableCellPercentage";
                    hCell1.Text = PrepareRoleName(Global.LanguageManager.GetText(sec.Name));
                    hrow.Cells.Add(hCell1);


                    int cnt = 0;
                    foreach (PermissionCore.Classes.Permission permission in sec.Permissions)
                    {
                        if (!Global.User.HasPermission(permission.Id) && Global.User.HasPermission(1001) == false)
                            continue;
                        if (!(permission.Id == 1001) && !(permission.Id == 1000))
                        {
                            TableCell tc = new TableCell
                            {
                                ID = "tc" + permission.Name.ToLower(),
                                Text = PrepareRoleName(Global.LanguageManager.GetText(permission.Name).ToLower()),
                                Width = new Unit(150, UnitType.Pixel),
                                CssClass =
                                    cnt % 2 == 0
                                        ? "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5"
                                        : "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor9"
                            };
                            hrow.Cells.Add(tc);
                            cnt++;
                        }
                    }
                    tblSection.Rows.Add(hrow);

                    var roles = Global.Core.Roles.Get().OrderBy(s => s.Name);
                    if (roles != null)
                    {
                        foreach (Role role in roles)
                        {
                            RolePermission rolePermission = Global.Core.RolePermissions.GetSingle("IdRole", role.Id);
                            TableRow tr = new WebUtilities.Controls.TableRow();
                            TableCell td1 = new TableCell
                            {
                                ID = "td_" + sec.Name + "_" + role.Name,
                                Width = new Unit(150, UnitType.Pixel),
                                Text = PrepareRoleName(role.Name.ToLower())
                            };
                            td1.Font.Bold = true;
                            td1.CssClass =
                                "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor8";
                            tr.Cells.Add(td1);

                            int count = 0;
                            foreach (PermissionCore.Classes.Permission detailPermission in sec.Permissions)
                            {
                                if (!Global.User.HasPermission(detailPermission.Id) && Global.User.HasPermission(1001) == false)
                                    continue;

                                if (!(detailPermission.Id == 1001) && !(detailPermission.Id == 1000))
                                {
                                    TableCell cell = new TableCell
                                    {
                                        ID = "td" + detailPermission.Name + role.Id,
                                        Width = new Unit(150, UnitType.Pixel),
                                        Text = PrepareRoleName(role.Name.ToLower()),
                                        CssClass = count % 2 == 0
                                            ? "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor9"
                                            : "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5"
                                    };

                                    WebUtilities.Controls.CheckBox checkBox = new WebUtilities.Controls.CheckBox
                                    {
                                        ID = "chk_" + role.Id + "_" + detailPermission.Id
                                    };

                                    RolePermission dbPermission =
                                        Global.Core.RolePermissions.GetSingle(new string[] { "IdRole", "Permission" },
                                            new object[] { role.Id, detailPermission.Id });

                                    checkBox.Checked = dbPermission != null;

                                    cell.Controls.Add(checkBox);
                                    tr.Cells.Add(cell);
                                    count++;
                                }
                            }

                            tblSection.Rows.Add(tr);
                        }
                    }
                    pnlModuleDetails.Controls.Add(tblSection);
                }
            }
        }

        private void SaveAllUserGroups()
        {
            try
            {
                var roles = Global.Core.Roles.Get();
                if (roles != null)
                {
                    foreach (Role role in roles)
                    {
                        foreach (PermissionCore.Classes.Section sec in Global.PermissionCore.Sections.Items)
                        {
                            foreach (PermissionCore.Classes.Permission permission in sec.Permissions)
                            {
                                var cntrl = pnlModuleDetails.FindControl("tbl" + sec.Name)
                                        .FindControl("chk_" + role.Id + "_" + permission.Id);

                                var tblCntrl = cntrl is WebUtilities.Controls.CheckBox
                                    ? (WebUtilities.Controls.CheckBox)cntrl
                                    : null;

                                if (tblCntrl != null)
                                {
                                    RolePermission rolepermission =
                                        Global.Core.RolePermissions.GetSingle(new string[] { "IdRole", "Permission" },
                                            new object[] { role.Id, permission.Id });

                                    if (rolepermission == null && tblCntrl.Checked)
                                    {
                                        rolepermission = new RolePermission(Global.Core.RolePermissions);
                                        rolepermission.Permission = permission.Id;
                                        rolepermission.IdRole = role.Id;

                                        rolepermission.Insert();
                                    }
                                    else if (rolepermission != null && tblCntrl.Checked == false)
                                    {
                                        Global.Core.RolePermissions.Delete(rolepermission.Id);
                                    }
                                }
                            }
                        }
                    }
                    Response.Redirect("ManageGroup.aspx?msg=" + "3", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("ManageGroup.aspx?msg=" + "4", false);
            }
        }

        //This functon is used for ajax call
        [System.Web.Services.WebMethod]
        public static string RemoveRole(string roleId)
        {
            string returnUrl = null;
            try
            {
                var roles = Global.Core.Roles.GetSingle(Guid.Parse(roleId));
                if (roles != null)
                {
                    RolePermission rolePermission = Global.Core.RolePermissions.GetSingle("IdRole", roles.Id);

                    if (rolePermission != null)
                    {
                        Global.Core.RolePermissions.Delete(rolePermission.Id);
                    }
                    var userRole = Global.Core.UserRoles.GetSingle("IdRole", roles.Id);
                    if (userRole != null)
                    {
                        Global.Core.UserRoles.Delete(userRole.Id);
                        Global.Core.Users.Delete(userRole.IdUser);
                    }
                    Global.Core.Roles.Delete(roles.Id);
                    returnUrl = "ManageGroup.aspx?msg=1";
                }
            }
            catch (Exception exception)
            {
                HttpContext.Current.Session["Error"] = exception.Message;
                returnUrl = "ManageGroup.aspx?msg=4";
                return returnUrl;
            }
            return returnUrl.Trim();
        }


        #endregion

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
                            Global.LanguageManager.GetText("RoleDeleteMsg"),
                            Global.LanguageManager.GetText("RoleDeleteMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "2":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString().Trim()),
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString().Trim())), WebUtilities.MessageType.Error);
                            break;
                        case "3":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("RoleModifyMsg"),
                            Global.LanguageManager.GetText("RoleModifyMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "4":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString().Trim()),
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString().Trim())), WebUtilities.MessageType.Error);
                            break;
                        case "5":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("AdminRoleMsg"),
                            Global.LanguageManager.GetText("AdminRoleMsg")), WebUtilities.MessageType.Error);
                            break;
                        case "6":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("RoleDeleteMsg"),
                            Global.LanguageManager.GetText("RoleDeleteMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "7":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("UserExistMsg"),
                            Global.LanguageManager.GetText("UserExistMsg")), WebUtilities.MessageType.Error);
                            break;
                    }
                }
                //ddlRole.Items.Add(new ListItem("select", "select"));
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
            // GenerateUserRoleDetails();
            GenereateUserGroups(ddlRole.SelectedItem.Text);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            //Button btnDelete = (Button)sender;
            //var roleID = btnDelete.ID.Split('_')[1];
            var roleID = ddlRole.SelectedItem.Value;

            var roles = Global.Core.Roles.GetSingle(Guid.Parse(roleID));
            cbRemove.Visible = true;

            cbRemove.Text = string.Format(
                Global.LanguageManager.GetText("RemoveUserGroupConfirmMessage") + " " + roles.Name + "?"
            );

            cbRemove.Confirm = delegate()
            {
                try
                {
                    int cnt = Global.Core.Roles.Get().Count();
                    if (cnt > 1)
                    {
                        if (roles != null)
                        {
                            if (Global.Core.UserRoles.Get("IdRole", roleID).Count() == 0)
                            {
                                RolePermission rolePermission = Global.Core.RolePermissions.GetSingle("IdRole", roles.Id);

                                if (rolePermission != null)
                                {
                                    Global.Core.RolePermissions.Delete(rolePermission.Id);
                                }
                                var userRoles = Global.Core.UserRoles.Get("IdRole", roles.Id);
                                //var userRole = Global.Core.UserRoles.GetSingle("IdRole", roles.Id);
                                if (userRoles != null)
                                {
                                    //Need to add foreach loop
                                    foreach (var userRole in userRoles)
                                    {
                                        Global.Core.UserRoles.Delete(userRole.Id);
                                    }
                                }
                                Global.Core.Roles.Delete(roles.Id);
                                HttpContext.Current.Response.Redirect("ManageGroup.aspx?msg=6", false);
                            }
                            else
                            {
                                HttpContext.Current.Response.Redirect("ManageGroup.aspx?msg=7", false);

                            }
                        }
                    }
                    else
                    {
                        HttpContext.Current.Response.Redirect("ManageGroup.aspx?msg=5", false);
                    }
                }
                catch (Exception exception)
                {
                    HttpContext.Current.Session["Error"] = exception.Message;
                    HttpContext.Current.Response.Redirect("ManageGroup.aspx?msg=4", false);
                }
            };
        }

        protected void btnModify_Click(object sender, EventArgs e)
        {
            try
            {
                //Button btnName = (Button)sender;
                //var roleId = btnName.ID.Split('_')[1];
                var roleId = ddlRole.SelectedItem.Value;
                var role = Global.Core.Roles.GetSingle(Guid.Parse(roleId));
                if (role != null)
                {
                    foreach (PermissionCore.Classes.Section sec in Global.PermissionCore.Sections.Items)
                    {
                        foreach (PermissionCore.Classes.Permission permission in sec.Permissions)
                        {
                            //var cntrl = pnlModuleDetails.FindControl("tbl" + role.Name)
                            //        .FindControl("chk_" + role.Id + "_" + permission.Id);

                            var cntrl = pnlModuleDetails.FindControl("tblRole")
                                    .FindControl("chk_" + role.Id + "_" + permission.Id);
                            var tblCntrl = cntrl is WebUtilities.Controls.CheckBox
                                ? (WebUtilities.Controls.CheckBox)cntrl
                                : null;

                            if (tblCntrl != null)
                            {
                                RolePermission rolepermission =
                                    Global.Core.RolePermissions.GetSingle(new string[] { "IdRole", "Permission" },
                                        new object[] { role.Id, permission.Id });

                                if (rolepermission == null && tblCntrl.Checked)
                                {
                                    rolepermission = new RolePermission(Global.Core.RolePermissions);
                                    rolepermission.Permission = permission.Id;
                                    rolepermission.IdRole = role.Id;

                                    rolepermission.Insert();
                                }
                                else if (rolepermission != null && tblCntrl.Checked == false)
                                {
                                    Global.Core.RolePermissions.Delete(rolepermission.Id);
                                }
                            }
                        }
                    }
                    Response.Redirect("ManageGroup.aspx" + "?msg=3", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("ManageGroup.aspx?msg=" + "4", false);
            }
        }

        protected void backButton_Click(object sender, EventArgs e)
        {
            try
            {
                var roles = Global.Core.Roles.Get();
                if (roles != null)
                {
                    foreach (Role role in roles)
                    {
                        foreach (PermissionCore.Classes.Section sec in Global.PermissionCore.Sections.Items)
                        {
                            foreach (PermissionCore.Classes.Permission permission in sec.Permissions)
                            {
                                //var cntrl = pnlModuleDetails.FindControl("tbl" + sec.Name)
                                //        .FindControl("chk_" + role.Id + "_" + permission.Id);
                                var cntrl = pnlModuleDetails.FindControl("tblRole")
                                   .FindControl("chk_" + role.Id + "_" + permission.Id);

                                var tblCntrl = cntrl is WebUtilities.Controls.CheckBox
                                    ? (WebUtilities.Controls.CheckBox)cntrl
                                    : null;

                                if (tblCntrl != null)
                                {
                                    RolePermission rolepermission =
                                        Global.Core.RolePermissions.GetSingle(new string[] { "IdRole", "Permission" },
                                            new object[] { role.Id, permission.Id });

                                    if (rolepermission == null && tblCntrl.Checked)
                                    {
                                        rolepermission = new RolePermission(Global.Core.RolePermissions);
                                        rolepermission.Permission = permission.Id;
                                        rolepermission.IdRole = role.Id;

                                        rolepermission.Insert();
                                    }
                                    else if (rolepermission != null && tblCntrl.Checked == false)
                                    {
                                        Global.Core.RolePermissions.Delete(rolepermission.Id);
                                    }
                                }
                            }
                        }
                    }
                    Response.Redirect("ManageUserGroupsHome.aspx", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("ManageGroup.aspx?msg=" + "4", false);
            }

        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GenereateUserGroups(ddlRole.SelectedItem.Text);
            }
        }

    }
        #endregion
}