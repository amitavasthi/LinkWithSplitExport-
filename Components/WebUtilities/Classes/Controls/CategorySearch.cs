using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace WebUtilities.Controls
{
    public class CategorySearch : BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the report definition
        /// file where the category search is part of.
        /// </summary>
        public string Source { get; set; }

        public int Limit { get; set; }

        public CategorySearchSelectionType SelectionType { get; set; }

        public CategorySearchCheckDisplay CheckDisplayMethod { get; set; }

        public CategorySearchMode SearchMode { get; set; }

        public bool EnableNonCategorical { get; set; }

        #endregion


        #region Constructor

        public CategorySearch()
        {
            /*this.Title = "SearchFilter";
            this.TitleLanguageLabel = true;
            this.Position = BoxPosition.Top;
            this.JavascriptTriggered = true;
            this.Dragable = true;*/

            if (this.Limit == 0)
                this.Limit = 50;

            this.Load += CategorySearch_Load;
        }

        #endregion


        #region Methods

        public void Render()
        {
            Panel pnlContainer = new Panel();
            pnlContainer.CssClass = "CategorySearch";

            Panel pnlSearchBox = new Panel();
            pnlSearchBox.Style.Add("margin", "1em");

            TextBox txtSearchBox = new TextBox();
            txtSearchBox.CssClass = "CategorySearchBox";
            txtSearchBox.Attributes.Add("onkeyup", "SearchCategories(this);");

            CheckBox chkApplyAllTabs = new CheckBox();
            chkApplyAllTabs.ID = "chkSplitnExportAllTabs";
            chkApplyAllTabs.TextAlign = System.Web.UI.WebControls.TextAlign.Left;
            chkApplyAllTabs.Attributes.Add("onclick", "ExportApplyAllTabs();");
            chkApplyAllTabs.Text = base.LanguageManager.GetText("ExportAllTabs");
            chkApplyAllTabs.Style.Add("float", "right");

            Panel pnlSearchResults = new Panel();
            pnlSearchResults.CssClass = "CategorySearchResults";

            pnlSearchBox.Controls.Add(txtSearchBox);
            pnlSearchBox.Controls.Add(chkApplyAllTabs);

            pnlContainer.Controls.Add(pnlSearchBox);
            pnlContainer.Controls.Add(pnlSearchResults);

            /*Panel btnCancel = new Panel();
            btnCancel.CssClass = "CancelCircle";

            Panel btnConfirm = new Panel();
            btnConfirm.CssClass = "OkCircle";

            btnCancel.Attributes.Add("onclick", string.Format(
                "document.getElementById('{0}').Close(false);",
                this.ClientID
            ));
            btnConfirm.Attributes.Add("onclick", string.Format(
                "document.getElementById('{0}').Close(true);",
                this.ClientID
            ));

            pnlContainer.Controls.Add(btnCancel);
            pnlContainer.Controls.Add(btnConfirm);*/

            base.Controls.Add(pnlContainer);

            this.Visible = true;

            if (Page != null)
            {
                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "InitCategorySearch" + this.ID,
                    "loadFunctions.push(function() { InitCategorySearch('" + this.ClientID + "', undefined, '" + (this.Source != null ? this.Source.Replace("\\", "/") : "") + "'); });",
                    true
                );
            }
            else
            {
                base.Controls.Add(new LiteralControl(string.Format(
                    "<script type=\"text/javascript>InitCategorySearch('{0}', undefined, '{1}');</script>",
                    this.ClientID,
                    this.Source != null ? this.Source.Replace("\\", "/") : ""
                )));
            }
        }

        #endregion


        #region Event Handlers

        protected void CategorySearch_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CategorySearch" + this.ClientID] = this;

            // Render the category search control.
            this.Render();
        }

        #endregion
    }

    public enum CategorySearchSelectionType
    {
        Multi,
        Single
    }

    public enum CategorySearchMode
    {
        All,
        Taxonomy,
        Study
    }

    public delegate bool CategorySearchCheckDisplay(Guid idCategory, bool isTaxonomy);
}
