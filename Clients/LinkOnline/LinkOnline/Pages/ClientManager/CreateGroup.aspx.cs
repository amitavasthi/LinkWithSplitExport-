using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseCore.Items;

namespace LinkOnline.Pages.ClientManager
{
    public partial class CreateGroup : WebUtilities.BasePage
    {
        #region Methods
        private string PrepareRoleName(string name)
        {
            string result = name;

            int c = 0;
            for (int i = 0; i < name.Length; i++)
            {
                if (i != 0 && i % 19 == 0)
                {
                    result = result.Insert(i + c, "<br />");

                    c += 6;
                }
            }

            return result;
        }
        private void GenerateModules()
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
                    tblSection.Width = new Unit(100, UnitType.Percentage);

                    TableHeaderRow hrow = new TableHeaderRow();

                    TableCell hCell1 = new TableCell();
                    hCell1.ID = "hCell" + sec.Name;
                    hCell1.Width = new Unit(100, UnitType.Pixel);
                    hCell1.CssClass = "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor8 TableCellPercentage";
                    hCell1.Text = Global.LanguageManager.GetText(sec.Name).ToLower();
                    hrow.Cells.Add(hCell1);


                    int cnt = 0;
                    foreach (PermissionCore.Classes.Permission permission in sec.Permissions)
                    {
                        if (!Global.User.HasPermission(permission.Id) && Global.User.HasPermission(1001) == false)
                            continue;

                        if (!(permission.Id == 1001) && !(permission.Id == 1000))
                        {
                            TableCell tc = new TableCell();
                            tc.ID = "tc" + permission.Name.ToLower();
                            tc.Text = PrepareRoleName(Global.LanguageManager.GetText(permission.Name).ToLower());
                            tc.Width = new Unit(150, UnitType.Pixel);
                            tc.CssClass = cnt % 2 == 0 ? "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor5" : "TableCellHeadline TableCellHeadlineCategory BorderColor9 BackgroundColor9";
                            hrow.Cells.Add(tc);
                            cnt++;
                        }

                      

                    }
                    tblSection.Rows.Add(hrow);

                    TableRow tr = new WebUtilities.Controls.TableRow();
                    TableCell td1 = new TableCell
                    {
                        ID = "td_" + sec.Name,
                        Width = new Unit(150, UnitType.Pixel)
                       
                    };
                    td1.CssClass =
                        "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor7";
                    tr.Cells.Add(td1);

                    int count = 0;
                    foreach (PermissionCore.Classes.Permission detailPermission in sec.Permissions)
                    {
                       

                        if (!Global.User.HasPermission(detailPermission.Id) && Global.User.HasPermission(1001) == false)
                            continue;
                        if (!(detailPermission.Id == 1001) && !(detailPermission.Id == 1000))
                        {
                            TableCell cell = new TableCell();
                            cell.ID = "td" + detailPermission.Name;
                            cell.Width = new Unit(150, UnitType.Pixel);
                            cell.CssClass = count % 2 == 0 ? "TableCellHeadline TableCellHeadlineCategory BorderColor7 BackgroundColor9" : "TableCellHeadline TableCellHeadlineCategory BorderColor9 BackgroundColor5";
                            WebUtilities.Controls.CheckBox checkBox = new WebUtilities.Controls.CheckBox();
                            checkBox.ID = "chk_" + detailPermission.Id;

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
        void SaveModules()
        {
            if (!string.IsNullOrEmpty(txtGroupName.Text.Trim()))
            {
                try
                {
                    Role role = Global.Core.Roles.GetSingle("Name", txtGroupName.Text.Trim());
                    if (role != null)
                    {
                        Response.Redirect("CreateGroup.aspx" + "?error=3", false);
                    }
                    else
                    {

                        Role newrole = new Role(Global.Core.Roles)
                         {
                             Id = Guid.NewGuid(),
                             Name = txtGroupName.Text.Trim(),
                             Description = txtGroupName.Text.Trim()
                         };
                        newrole.Insert();

                        foreach (PermissionCore.Classes.Section sec in Global.PermissionCore.Sections.Items)
                        {
                            foreach (PermissionCore.Classes.Permission permission in sec.Permissions)
                            {
                                var cntrl =
                                    pnlModuleDetails.FindControl("tbl" + sec.Name)
                                        .FindControl("chk_" + permission.Id);

                                var tblCntrl = cntrl is WebUtilities.Controls.CheckBox
                                    ? (WebUtilities.Controls.CheckBox)cntrl
                                    : null;

                                if (tblCntrl != null)
                                {
                                    RolePermission rolepermission =
                                        Global.Core.RolePermissions.GetSingle(new string[] { "IdRole", "Permission" },
                                            new object[] { newrole.Id, permission.Id });

                                    if (rolepermission == null && tblCntrl.Checked)
                                    {
                                        rolepermission = new RolePermission(Global.Core.RolePermissions);
                                        rolepermission.Permission = permission.Id;
                                        rolepermission.IdRole = newrole.Id;

                                        rolepermission.Insert();
                                    }
                                    else if (rolepermission != null && tblCntrl.Checked == false)
                                    {
                                        Global.Core.RolePermissions.Delete(rolepermission.Id);

                                    }
                                }
                            }
                        }
                        Response.Redirect("CreateGroup.aspx" + "?error=1", false);
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Session["Error"] = ex.Message;
                    Response.Redirect("CreateGroup.aspx" + "?error=2", false);
                }
            }
        }
        #endregion
        #region EventHandlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["error"] != null)
                {
                    switch (Request.QueryString["error"].Trim())
                    {
                        case "1":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("UserGroupAddMsg"),
                            Global.LanguageManager.GetText("UserGroupAddMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "2":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString().Trim()),
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString().Trim())), WebUtilities.MessageType.Error);
                            break;
                        case "3":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("DuplicateGroup"),
                            Global.LanguageManager.GetText("DuplicateGroup")), WebUtilities.MessageType.Error);
                            break;
                    }
                }
            }
            GenerateModules();
        }
        protected void btnCreateGroup_OnClick(object sender, EventArgs e)
        {
            SaveModules();
        }
        #endregion
    }
}