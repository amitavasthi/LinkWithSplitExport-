using Crosstables.Classes.ReportDefinitionClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtilities.Controls;

namespace Crosstables.Classes
{
    public class CategorySelector : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the current report definition.
        /// </summary>
        public ReportDefinition ReportDefinition { get; set; }

        public string FileName { get; set; }

        public string XPath { get; set; }

        public event EventHandler SelectedIndexChanged;

        public System.Web.UI.WebControls.TextBox TxtSearch { get; set; }

        #endregion


        #region Constructor

        public CategorySelector(ReportDefinition reportDefinition, string fileName)
            : base("div")
        {
            this.FileName = fileName.Replace("\\", "/");
            this.ReportDefinition = reportDefinition;
            this.Load += CategorySelector_Load;
            this.PreRender += CategorySelector_PreRender;
        }

        #endregion


        #region Methods

        public void Render()
        {
            base.Controls.Clear();

            base.CssClass = "CategorySelector " + this.CssClass;

            Panel pnlSearch = new Panel();
            Panel pnlCategories = new Panel();

            pnlSearch.CssClass = "CategorySelectorSearch";
            pnlCategories.CssClass = "CategorySelectorCategories";

            this.TxtSearch = new System.Web.UI.WebControls.TextBox();
            this.TxtSearch.ID = "txtSearch" + this.ID;
            this.TxtSearch.CssClass = "CategorySelectorSearch_Inactive";
            this.TxtSearch.Text = base.LanguageManager.GetText("SearchFilterCategory");

            this.TxtSearch.Attributes.Add(
                "autocomplete",
                "off"
            );

            pnlSearch.Controls.Add(this.TxtSearch);

            HoverBox hbCategories = new HoverBox();
            hbCategories.ID = "hbCategories" + this.ID;
            hbCategories.Display = HoverBoxDisplay.TopLeft;
            hbCategories.IdTrigger = TxtSearch.ID;
            hbCategories.TriggerMode = HoverBoxTriggerMode.Click;
            hbCategories.AsynchRender = true;

            pnlCategories.Controls.Add(hbCategories);

            base.Controls.Add(pnlSearch);
            base.Controls.Add(pnlCategories);

            this.TxtSearch.Attributes.Add(
                "onmouseup",
                "EnableFilterCategorySearch(this, '"+ this.FileName +"', '" + hbCategories.ClientID + "', '" + this.XPath + "')"
            );

            hbCategories.Render();
        }

        #endregion


        #region Event Handlers

        protected void CategorySelector_Load(object sender, EventArgs e)
        {
            Render();
        }

        protected void CategorySelector_PreRender(object sender, EventArgs e)
        {
            this.TxtSearch.Text = base.LanguageManager.GetText("SearchFilterCategory");
        }

        #endregion
    }
}
