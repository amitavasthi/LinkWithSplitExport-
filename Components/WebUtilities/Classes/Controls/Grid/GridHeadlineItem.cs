using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using WebUtilities.Controls;

namespace WebUtilities.Classes.Controls.GridClasses
{
    public class GridHeadlineItem : BaseControl
    {
        #region Properties

        public string Text { get; set; }

        public GridHeadlineItemWidth Width { get; set; }

        public int Index { get; set; }

        public GridHeadline Owner { get; set; }

        public string ColumnName { get; private set; }

        public string SearchText 
        {
            get
            {
                if (HttpContext.Current.Session["GridSearch" + this.Owner.Owner.ID + "_" + this.ColumnName] == null)
                    HttpContext.Current.Session["GridSearch" + this.Owner.Owner.ID + "_" + this.ColumnName] = "";

                return HttpContext.Current.Session["GridSearch" + this.Owner.Owner.ID + "_" + this.ColumnName].ToString();
            }
            set
            {
                HttpContext.Current.Session["GridSearch" + this.Owner.Owner.ID + "_" + this.ColumnName] = value;
            }
        }

        public int StringMaxLength { get; set; }

        #endregion


        #region Constructor

        public GridHeadlineItem(GridHeadline owner, int index, string text, GridHeadlineItemWidth width, bool languageLabel = false)
            : base("div")
        {
            this.Width = width;
            this.Index = index;
            this.Owner = owner;

            if (languageLabel)
            {
                this.ColumnName = text;
                this.Text = base.LanguageManager.GetText(text);
            }
            else
            {
                this.ColumnName = text.Replace(" ", ""); ;
                this.Text = text;
            }

            this.Load += GridHeadlineItem_Load;
        }

        public GridHeadlineItem(GridHeadline owner, int index, string text, int width, bool languageLabel = false)
            : this(owner, index, text, new GridHeadlineItemWidth(width), languageLabel)
        { }

        #endregion


        #region Method

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

                if (base.UserHasPermission(permission.Id) == false)
                {
                    this.Visible = false;

                    return;
                }
            }

            int width = 0;

            if (HttpContext.Current.Request.Params[this.Owner.Owner.ClientID + this.ColumnName] != null)
            {
                width = (int)(double.Parse(HttpContext.Current.Request.Params[this.Owner.Owner.ClientID + this.ColumnName]));
            }

            if (HttpContext.Current.Request.Params[this.Owner.Owner.ClientName + "HdfGridSearch" + this.ColumnName] != null)
            {
                this.SearchText = HttpContext.Current.Request.Params[this.Owner.Owner.ClientName + "HdfGridSearch" + this.ColumnName];
            }

            this.CssClass = "GridHeadlineItem";

            this.Attributes.Add("ColumnName", this.ColumnName);
            this.Attributes.Add("Index", this.Index.ToString());

            //this.Controls.Add(new LiteralControl(this.Text));

            HtmlGenericControl label = new HtmlGenericControl("div");
            label.Attributes.Add("class", "GridHeadlineItemOverflow");
            label.InnerHtml = this.Text;

            this.ToolTip = this.Text;
            this.ToolTipMustOverflow = true;

            string script = "var obj = new Object();";
            script += "obj.IdGrid = '" + this.Owner.Owner.ClientID + "';";
            script += "obj.Index = '" + this.Index + "';";
            script += "obj.MaxGridHeight = '" + this.Owner.Owner.MaxHeight + "';";
            script += "obj.Width = '" + this.Width.Width;

            script += "%';gridPercentageWidths.push(obj);";

            if (Page != null)
            {
                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "GridSetColumnWidth" + this.Owner.Owner.ID + this.Text.Replace(" ", ""),
                    script,
                    true
                );
            }

            int w = this.Width.Width;

            this.Style.Add("width", (w) + "%");

            HiddenField hdfGridSearch = new HiddenField();
            hdfGridSearch.ID = this.Owner.Owner.ID + "HdfGridSearch" + this.ColumnName;
            hdfGridSearch.Value = this.SearchText;

            this.Controls.Add(hdfGridSearch);

            Image imgSearch = new Image();
            imgSearch.CssClass = "GridHeadlineItemSearch";
            imgSearch.ImageUrl = "/Images/Icons/Search.png";
            imgSearch.Attributes.Add("onmouseover", "this.src = '/Images/Icons/Search_Hover.png';");
            imgSearch.Attributes.Add("onmouseout", "this.src = '/Images/Icons/Search.png';");
            imgSearch.Attributes.Add("onclick",
                "InitGridSearch('" +
                this.Owner.Owner.ClientID + "','" +
                this.Index + "', this, '" +
                hdfGridSearch.ClientID + "');"
            );

            this.Controls.Add(imgSearch);

            this.Controls.Add(label);

            if (this.SearchText != "" && Page != null)
            {
                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "SearchSavedText" + this.Owner.Owner.ID + this.ColumnName,
                    "loadFunctions.push(function () { " +
                    "document.getElementById('" + imgSearch.ClientID + "').onclick();" +
                    "document.getElementById('" + this.Owner.Owner.ClientID + "Search" + this.Index + "').value = '" + this.SearchText + "';" +
                    "GridSearch('" + this.Owner.Owner.ClientID + "', '" + this.Index + "', '" + this.SearchText + "'); " +
                    "var filterIndex = GetFilter('" + this.Owner.Owner.ClientID + "', '" + this.Index + "');" +
                    "filters[filterIndex].value = '" + this.SearchText + "';" +
                    "});",
                    true
                );
            }
        }

        #endregion


        #region Event Handlers

        protected void GridHeadlineItem_Load(object sender, EventArgs e)
        {
            Build();
        }

        #endregion
    }
}
