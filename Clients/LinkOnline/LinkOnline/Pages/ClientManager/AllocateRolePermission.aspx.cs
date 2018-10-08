using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseCore.Items;

namespace LinkOnline.Pages.ClientManager
{
    public partial class AllocateRolePermission : WebUtilities.BasePage
    {
        #region Methods
        private void GenerateUserRoleDetails(Guid roleId)
        {
            if (Global.PermissionCore.Sections.Items != null)
            {
                foreach (PermissionCore.Classes.Section sec in Global.PermissionCore.Sections.Items)
                {
                    WebUtilities.Controls.Table tblSection = new WebUtilities.Controls.Table
                    {
                        ID = "tbl" + sec.Name,
                        CssClass = "Userroletable",
                        CellPadding = 0,
                        CellSpacing = 0
                    };

                    TableHeaderRow hrow = new TableHeaderRow();

                    TableCell hCell1 = new TableCell();
                    hCell1.ID = "hCellgroupassigned";
                    hCell1.Width = new Unit(150, UnitType.Pixel);
                    hCell1.CssClass = "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1";
                    hrow.Cells.Add(hCell1);


                    int cnt = 0;
                    foreach (PermissionCore.Classes.Permission permission in sec.Permissions)
                    {
                        TableCell tc = new TableCell
                        {
                            ID = "tc" + permission.Name.ToLower(),
                            Text = permission.Name.ToLower(),
                            Width = new Unit(150, UnitType.Pixel),
                            CssClass =
                                cnt % 2 == 0
                                    ? "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5"
                                    : "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1"
                        };
                        hrow.Cells.Add(tc);
                        cnt++;
                    }
                    tblSection.Rows.Add(hrow);

                    var role = Global.Core.Roles.GetSingle(roleId);
                    if (role != null)
                    {

                        TableRow tr = new WebUtilities.Controls.TableRow();
                        TableCell td1 = new TableCell
                        {
                            ID = "td_" + role.Name,
                            Width = new Unit(150, UnitType.Pixel),
                            Text = role.Name.ToLower()
                        };
                        td1.Font.Bold = true;
                        td1.CssClass =
                            "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5";
                        tr.Cells.Add(td1);

                        int count = 0;
                        foreach (PermissionCore.Classes.Permission detailPermission in sec.Permissions)
                        {
                            TableCell cell = new TableCell
                            {
                                ID = "td" + detailPermission.Name,
                                Width = new Unit(150, UnitType.Pixel),
                                Text = role.Name.ToLower(),
                                CssClass = count % 2 == 0
                                    ? "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor1"
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
                        tblSection.Rows.Add(tr);

                    }
                    pnlModuleDetails.Controls.Add(tblSection);
                }
            }
        }
        void SaveModules()
        {
            try
            {
                var role = Global.Core.Roles.GetSingle(Guid.Parse(hdnRoleId.Value));
                if (role != null)
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
                    Response.Redirect("ManageGroup.aspx" + "?msg=3", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("ManageGroup.aspx?msg=" + "4", false);
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            hdnRoleId.Value = "";
            Guid newGuid;
            if (Request.QueryString["mrp"] != null)
            {
                if (Guid.TryParse(Request.QueryString["mrp"], out newGuid))
                {
                    hdnRoleId.Value = newGuid.ToString();
                    GenerateUserRoleDetails(newGuid);
                }
            }
            else
            {
                base.ShowMessage(string.Format(
                Global.LanguageManager.GetText("RoleNotExist"),
                Global.LanguageManager.GetText("RoleNotExist")), WebUtilities.MessageType.Error);
            }
        }

        protected void btnAcceptChanges_OnClick(object sender, EventArgs e)
        {
            SaveModules();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            cbRemove.Visible = true;

            cbRemove.Text = string.Format(
                Global.LanguageManager.GetText("RemoveUserGroupConfirmMessage")
            );

            cbRemove.Confirm = delegate()
            {
                try
                {
                    int cnt = Global.Core.Roles.Get().Count();
                    if (cnt > 1)
                    {
                        var roles = Global.Core.Roles.GetSingle(Guid.Parse(hdnRoleId.Value));
                        if (roles != null)
                        {
                            if (Global.Core.UserRoles.Get("IdRole", hdnRoleId.Value).Count() == 0)
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
    }
}