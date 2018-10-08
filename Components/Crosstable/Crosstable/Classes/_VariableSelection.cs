using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtilities.Controls;

namespace Crosstables.Classes
{
    public class _VariableSelection : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the direction of the variable selection.
        /// </summary>
        public VariableSelectionDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the language id to display the variable labels.
        /// </summary>
        public int IdLanguage { get; set; }

        /// <summary>
        /// Gets or sets the id of the currently selected variable.
        /// </summary>
        public Guid IdSelectedVariable { get; set; }

        /// <summary>
        /// Gets or sets if the currently selected
        /// variable is a taxonomy variable.
        /// </summary>
        public bool IsTaxonomy { get; set; }

        public bool RenderAsynch { get; set; }

        public event EventHandler SelectedVariableChanged;

        #endregion


        #region Constructor

        public _VariableSelection()
        {
            this.Load += VariableSelection_Load;
        }

        #endregion


        #region Methods

        public void Render()
        {
            this.CssClass = "VariableSelection BackgroundColor7" + this.CssClass;

            Panel pnlButton = new Panel();
            pnlButton.CssClass = "VariableSelectionButton";

            pnlButton.Attributes.Add("onclick", string.Format(
                "ShowVariableSelection(this.parentNode, '{0}')",
                this.Direction.ToString()
            ));

            // Check if there is variable selected.
            if (this.IdSelectedVariable != null)
            {
                System.Web.UI.WebControls.Label lblVariableLabel = new System.Web.UI.WebControls.Label();

                // Check if the selected variable is a taxonomy variable.
                if (this.IsTaxonomy)
                {
                    // Get the selected taxonomy variable's label in the selected language.
                    lblVariableLabel.Text = (string)base.Core.TaxonomyVariableLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyVariable", "IdLanguage" },
                        new object[] { this.IdSelectedVariable, this.IdLanguage }
                    );
                }
                else
                {
                    // Get the selected variable's label in the selected language.
                    lblVariableLabel.Text = (string)base.Core.VariableLabels.GetValue(
                        "Label",
                        new string[] { "IdVariable", "IdLanguage" },
                        new object[] { this.IdSelectedVariable, this.IdLanguage }
                    );
                }

                pnlButton.Controls.Add(lblVariableLabel);
            }

            Panel pnlSelector = new Panel();
            pnlSelector.CssClass = "VariableSelectionSelector BackgroundColor7";

            Table table = new Table();
            table.CssClass = "VariableSelectionSelectorTable";

            TableRow tableRowSearch = new TableRow();
            TableCell tableCellSearch = new TableCell();

            tableCellSearch.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;

            RenderSearch(tableCellSearch);
            
            tableRowSearch.Cells.Add(tableCellSearch);

            table.Rows.Add(tableRowSearch);

            TableRow tableRowVariables = new TableRow();
            TableCell tableCellVariables = new TableCell();

            tableCellVariables.Style.Add("height", "100%");

            Panel pnlVariables = new Panel();

            pnlVariables.CssClass = "VariableSelectionSelectorVariables BorderColor1";

            tableCellVariables.Controls.Add(pnlVariables);

            tableRowVariables.Cells.Add(tableCellVariables);
            table.Rows.Add(tableRowVariables);

            TableRow tableRowClose = new TableRow();
            TableCell tableCellClose = new TableCell();

            tableCellClose.CssClass = "VariableSelectionSelectorClose BackgroundColor7";

            tableCellClose.Attributes.Add("onclick", string.Format(
                "HideVariableSelection(this.parentNode.parentNode.parentNode.parentNode.parentNode, '{0}');",
                this.Direction.ToString()
            ));

            tableCellClose.Attributes.Add(
                "onmouseover",
                "this.className = 'VariableSelectionSelectorClose BackgroundColor9';"
            );
            tableCellClose.Attributes.Add(
                "onmouseout",
                "this.className = 'VariableSelectionSelectorClose BackgroundColor7';"
            );

            tableRowClose.Cells.Add(tableCellClose);
            table.Rows.Add(tableRowClose);

            pnlSelector.Controls.Add(table);

            base.Controls.Add(pnlSelector);
            base.Controls.Add(pnlButton);
        }


        private void RenderSearch(TableCell tableCellSearch)
        {
            Table table = new Table();
            table.CssClass = "VariableSelectionSelectorSearchTable";

            TableRow tableRow = new TableRow();

            TableCell tableCellChapter = new TableCell();
            TableCell tableCellSettings = new TableCell();
            TableCell tableCell = new TableCell();

            tableCell.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left;

            tableCellChapter.CssClass = "VariableSelectionSelectorTableCellChapter";
            tableCellSettings.CssClass = "VariableSelectionSelectorTableCellSettings";

            DropDownList ddlChapter = new DropDownList();
            ddlChapter.CssClass = "VariableSelectionSelectorChapter BorderColor1";

            BindChapters(ddlChapter);

            OptionSwipe settings = new OptionSwipe();
            settings.Direction = OptionSwiperDirection.Bottom;

            Option optionDisplayNoDataVariables = new Option();
            optionDisplayNoDataVariables.Text = base.LanguageManager.GetText("DisplayNoDataVariables") + "<input type=\"checkbox\" checked=\"true\" />";

            Option optionDataCheckEnabled = new Option();
            optionDataCheckEnabled.Text = base.LanguageManager.GetText("EnableDataCheck") + "<input type=\"checkbox\" checked=\"true\" />";

            optionDisplayNoDataVariables.Style.Add("background", "#61CF71");
            optionDataCheckEnabled.CssClass = "BackgroundColor2";

            settings.Options.Add(optionDisplayNoDataVariables);
            settings.Options.Add(optionDataCheckEnabled);

            if (this.RenderAsynch)
            {
                optionDisplayNoDataVariables.Render();
                optionDataCheckEnabled.Render();
                settings.Render();
            }

            TextBox txtSearch = new TextBox();
            txtSearch.CssClass = "VariableSelectionSelectorSearch BorderColor1";

            ddlChapter.Attributes.Add("onchange", string.Format(
                "SearchVariables(this, GetChildByAttribute(this.parentNode.parentNode, 'class', 'VariableSelectionSelectorSearch BorderColor1', true), "+
                "GetChildByAttribute(this.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode, 'class', '{0}', true))",
                "VariableSelectionSelectorVariables BorderColor1"
            ));

            txtSearch.Attributes.Add("onkeyup", string.Format(
                "SearchVariables(GetChildByAttribute(this.parentNode.parentNode, 'class', 'VariableSelectionSelectorSearch BorderColor1', true), this, "+
                "GetChildByAttribute(this.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode, 'class', '{0}', true))",
                "VariableSelectionSelectorVariables BorderColor1"
            ));


            tableCellChapter.Controls.Add(ddlChapter);
            tableCellSettings.Controls.Add(settings);
            tableCell.Controls.Add(txtSearch);

            tableRow.Cells.Add(tableCellChapter);
            tableRow.Cells.Add(tableCellSettings);
            tableRow.Cells.Add(tableCell);

            table.Rows.Add(tableRow);

            tableCellSearch.Controls.Add(table);
        }

        private void BindChapters(DropDownList ddl)
        {
            // Get all chapters of the client.
            List<object[]> chapters = base.Core.TaxonomyChapters.GetValues(
                new string[] { "Id" }
            );

            ddl.Items.Add("");

            // Run through all chapters of the client.
            foreach (object[] chapter in chapters)
            {
                // Get the label of the chapter in the currently selected language.
                string label = (string)base.Core.TaxonomyChapterLabels.GetValue(
                    "Label",
                    new string[] { "IdTaxonomyChapter", "IdLanguage" },
                    new object[] { (Guid)chapter[0], this.IdLanguage }
                );

                // Create a new list item for the chapter.
                System.Web.UI.WebControls.ListItem lItem = new System.Web.UI.WebControls.ListItem();
                lItem.Text = label;
                lItem.Value = ((Guid)chapter[0]).ToString();

                ddl.Items.Add(lItem);
            }
        }

        #endregion


        #region Event Handlers

        protected void VariableSelection_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }

    public enum VariableSelectionDirection
    {
        Side,
        Top
    }
}
