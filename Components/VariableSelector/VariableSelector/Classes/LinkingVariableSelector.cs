using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using WebUtilities.Controls;

namespace VariableSelector1.Classes
{
    public class LinkingVariableSelector : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the variable of the variable selector.
        /// </summary>
        public DefinitionObject Variable { get; set; }

        /// <summary>
        /// Gets or sets the alignment of the variable selector.
        /// </summary>
        public LinkingVariableSelectorAlignment Alignment { get; set; }

        public bool DisableVariableOptions { get; set; }

        public bool AlreadyLinkedVariable { get; set; }

        public bool IsTaxonomy { get; set; }
        public int IdLanguage { get; set; }

        #endregion

        #region Constructor

        public LinkingVariableSelector(int idLanguage, DefinitionObject variable, bool editable = true)
        {
            this.IsTaxonomy = true;
            this.Variable = variable;
            this.IdLanguage = idLanguage;
            this.Load += LinkingVariableSelector_Load;
        }

        #endregion

        public void Render()
        {
            if (this.Variable.GetValue("Id") == null)
                return;

            this.ID = this.ID + "VariableSelector" + this.Variable.GetValue("Id");
            this.CssClass = "VariableSelectorControl BackgroundColor6";

            this.Attributes.Add("IdVariable", this.Variable.GetValue("Id").ToString());
            this.Attributes.Add("IsTaxonomy", this.IsTaxonomy.ToString());

            Panel pnlContainer = new Panel();
            pnlContainer.ID = "VariableSelectorContainer" + this.ID;

            Panel pnlSelector = new Panel();
            pnlSelector.ID = "pnlContainer" + this.ID;

            if (AlreadyLinkedVariable)
            {
                pnlSelector.CssClass = "VariableSelector BackgroundColorLinked";
                pnlSelector.Controls.Add(RenderVariableLabel(true));
            }
            else
            {
                pnlSelector.CssClass = "VariableSelector";
                pnlSelector.Controls.Add(RenderVariableLabel(false));
            }

            pnlSelector.Attributes.Add("onclick", "return false;");

            pnlSelector.Attributes.Add(
                "onmouseout",
                "this.style.background='';this.style.cursor = '';this.style.color='';this.onclick=undefined;"
            );

            pnlContainer.Controls.Add(pnlSelector);

            base.Controls.Add(pnlContainer);
        }

        public Table RenderVariableLabel(bool linked)
        {
            Table result = new Table();
            result.ID = "lblSelectedItem" + this.ID;
            result.Style.Add("width", "100%");

            VariableType variableType = ((VariableType)int.Parse(this.Variable.GetValue("Type").ToString()));

            TableCell tableCellCheckBox = new TableCell();
            TableCell tableCellVariableType = new TableCell();
            TableCell tableCellVariableLabel = new TableCell();

            tableCellCheckBox.CssClass = "VariableSelectorCheckBox";
            if (linked)
            {
                tableCellCheckBox.ID = "linked_" + this.Variable.GetValue("Id").ToString();
                tableCellCheckBox.Attributes.Add("name", this.Variable.GetLabel(this.IdLanguage));
            }
            else
            {
                tableCellCheckBox.ID = "unlinked_" + this.Variable.GetValue("Id").ToString();
                tableCellCheckBox.Attributes.Add("name", this.Variable.GetLabel(this.IdLanguage));
            }
            tableCellVariableType.CssClass = "VariableSelectorTypeBackground" + variableType;
            tableCellVariableLabel.CssClass = "VariableSelectorVariableLabel";

            tableCellCheckBox.Style.Add("width", "20px");
            tableCellVariableType.Style.Add("width", "30px");

            CheckBox chkBoxVariable = new CheckBox();
            tableCellCheckBox.Controls.Add(chkBoxVariable);

            Image imgVariableType = new Image();
            imgVariableType.CssClass = "VariableSelectorVariableType";
            imgVariableType.ImageUrl = string.Format(
                "/Images/Icons/VariableSelector/{0}.png",
                variableType
            );
            tableCellVariableLabel.Text = this.Variable.GetLabel(this.IdLanguage);

            tableCellVariableType.Controls.Add(imgVariableType);

            if (this.Alignment == LinkingVariableSelectorAlignment.Horizontal)
            {
                TableRow tableRow = new TableRow();

                tableRow.Cells.Add(tableCellCheckBox);

                tableRow.Cells.Add(tableCellVariableType);

                tableRow.Cells.Add(tableCellVariableLabel);

                result.Rows.Add(tableRow);

            }

            return result;
        }

        protected void LinkingVariableSelector_Load(object sender, EventArgs e)
        {
            Render();
        }


        public enum LinkingVariableSelectorAlignment
        {
            Horizontal
        }

    }
}
