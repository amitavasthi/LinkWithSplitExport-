using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace WebUtilities.Classes.Controls.GridClasses
{
    public class GridRowItem : BaseControl
    {
        #region Properties

        public string Text { get; set; }

        public GridRow Owner { get; set; }

        public string ColumnName { get; set; }

        public int Index { get; set; }

        #endregion


        #region Constructor

        public GridRowItem(GridRow owner, string text, bool languageLabel = false)
            : base("div")
        {
            this.Owner = owner;


            if (languageLabel)
            {
                this.Text = base.LanguageManager.GetText(text);
            }
            else
            {
                this.Text = text;
            }

            this.Load += GridRowItem_Load;
        }

        #endregion


        #region Methods

        public void Build()
        {
            if (base.PermissionCore != null &&
                base.PermissionCore.PagePermissions[HttpContext.Current.Request.Url.LocalPath] != null &&
                base.PermissionCore.PagePermissions[HttpContext.Current.Request.Url.LocalPath].
                GridColumnPermissions[this.ColumnName] != null &&
                base.IdUser != null)
            {
                PermissionCore.Classes.Permission permission = base.PermissionCore
                    .PagePermissions[HttpContext.Current.Request.Url.LocalPath].
                    GridColumnPermissions[this.ColumnName].Permission;

                //if (base.User.HasPermission(permission.Id) == false)
                if (!base.UserHasPermission(permission.Id))
                    {
                    this.Visible = false;

                    return;
                }
            }

            this.CssClass = "GridRowItem";

            if (this.Owner.Owner.GridHeadline.Items.Count == 1)
            {
                this.CssClass = "GridRowItem GridRowItemSingle";
            }

            this.Attributes.Add("ColumnName", this.ColumnName);
            this.Attributes.Add("Index", this.Index.ToString());

            int w = this.Owner.Owner.GridHeadline.
                Items[this.Index].Width.Width;

            this.Style.Add("width", (w) + "%");

            BaseControl overflowControl = new BaseControl("div");
            overflowControl.Attributes.Add("class", "GridRowItemOverflow");

            overflowControl.ToolTipMustOverflow = true;

            if (this.Text != null)
                overflowControl.ToolTip = this.Text.Replace("\"", "'");

            overflowControl.Controls.Add(new LiteralControl(this.Text));

            this.Controls.Add(overflowControl);
        }

        #endregion


        #region Event Handlers

        protected void GridRowItem_Load(object sender, EventArgs e)
        {
            Build();
        }

        #endregion
    }
}
