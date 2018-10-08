using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using WebUtilities.Controls;

namespace VariableSelector.Classes
{
    public class VariableSelector : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the variable of the variable selector.
        /// </summary>
        public DefinitionObject Variable { get; set; }

        public bool Editable { get; set; }

        public bool OnCrosstable { get; set; }

        #endregion


        #region Constructor

        public VariableSelector(DefinitionObject variable, bool editable = true)
        {
            this.Editable = editable;
            this.Variable = variable;

            this.Load += VariableSelector_Load;
        }

        public VariableSelector(string source, string path, bool editable = true)
        {
            this.Editable = editable;
            this.Variable = new DefinitionObject(this.Core, source, path);

            this.Load += VariableSelector_Load;
            this.PreRender += VariableSelector_PreRender;
        }

        #endregion


        #region Methods

        public void Render()
        {
            if (this.Variable.GetValue("Id") == null)
                return;

            this.ID = this.ID + "VariableSelector" + this.Variable.GetValue("Id");
            this.CssClass = "VariableSelectorControl BackgroundColor6";

            this.Attributes.Add("IdVariable", this.Variable.GetValue("Id").ToString());
            this.Attributes.Add("IsTaxonomy", true.ToString());

            Panel pnlContainer = new Panel();
            pnlContainer.ID = "VariableSelectorContainer" + this.ID;

            Panel pnlSelector = new Panel();
            pnlSelector.ID = "pnlContainer" + this.ID;
            pnlSelector.CssClass = "VariableSelector";

            pnlSelector.Controls.Add(RenderVariableLabel());

            pnlSelector.Attributes.Add("ondblclick", "EditVariableLabel(this, '" + this.Variable.Source.Replace("\\","/") + "', '" + this.Variable.Path + "');");
            pnlSelector.Attributes.Add("onclick", "return false;");

            //base.Controls.Add(pnlSelector);

            pnlSelector.Attributes.Add(
                "onmouseout",
                "this.style.background='';this.style.cursor = '';this.style.color='';this.onclick=undefined;"
            );

            pnlContainer.Controls.Add(pnlSelector);

            base.Controls.Add(pnlContainer);

            if (this.OnCrosstable)
            {
                pnlSelector.Style.Add("height", "100%");
                pnlContainer.Style.Add("height", "100%");
                base.Style.Add("height", "100%");
            }

            if (Page != null)
            {
                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "InitVariableSelector" + this.ID,
                    "loadFunctions.push(function() { InitVariableSelector('" + this.ClientID + "'); });",
                    true
                );
            }
            else
            {
                HtmlGenericControl script = new HtmlGenericControl("script");
                script.Attributes.Add("type", "text/javascript");
                script.InnerHtml = "InitVariableSelector('" + this.ClientID + "');";

                base.Controls.Add(script);
            }
        }

        public Table RenderVariableLabel()
        {
            Table result = new Table();
            result.ID = "lblSelectedItem" + this.ID;
            result.Style.Add("width", "100%");

            if (this.OnCrosstable)
            {
                result.Style.Add("height", "100%");
            }

            result.Attributes.Add(
                "oncontextmenu",
                "DeleteVariable" + "(this, '" +
                this.Variable.Source.Replace("\\", "/") + "', '" +
                this.Variable.Path + "'); return false;"
            );

            VariableType variableType = ((VariableType)int.Parse(this.Variable.GetValue("Type").ToString()));

            TableRow tableRow = new TableRow();

            TableCell tableCellVariableType = new TableCell();
            TableCell tableCellVariableLabel = new TableCell();
            TableCell tableCellVariableDropDown = new TableCell();

            if (this.OnCrosstable)
            {
                tableCellVariableLabel.Attributes.Add("DropArea", "True");
                tableCellVariableLabel.Attributes.Add("IdSelected", this.Variable.GetValue("Id").ToString());
                tableCellVariableLabel.Attributes.Add("Source", this.Variable.Source);
                tableCellVariableLabel.Attributes.Add("Path", this.Variable.ParentPath);
            }

            tableCellVariableType.CssClass = "VariableSelectorTypeBackground" + variableType;
            tableCellVariableLabel.CssClass = "VariableSelectorVariableLabel";
            tableCellVariableDropDown.CssClass = "VariableSelectorDropDown";

            tableCellVariableType.Style.Add("width", "60px");
            tableCellVariableDropDown.Style.Add("width", "60px");

            Image imgVariableType = new Image();
            imgVariableType.CssClass = "VariableSelectorVariableType";
            imgVariableType.ImageUrl = string.Format(
                "/Images/Icons/VariableSelector/{0}.png",
                variableType
            );

            imgVariableType.Attributes.Add("onclick", string.Format(
                "ShowVariableOptions(this, '{0}', '{1}', {2});return false;",
                this.Variable.Source.Replace("\\", "/"),
                this.Variable.Path,
                this.Editable.ToString().ToLower()
            ));

            imgVariableType.Attributes.Add("onmouseover", "this.src=\"/Images/Icons/Swiper.png\"");
            imgVariableType.Attributes.Add("onmouseout", string.Format(
                "this.src=\"/Images/Icons/VariableSelector/{0}.png\"",
                ((VariableType)int.Parse(this.Variable.GetValue("Type").ToString()))
            ));

            tableCellVariableLabel.Text = this.Variable.GetLabel(1031);

            tableCellVariableType.Controls.Add(imgVariableType);

            Image imgVariableDropDown = new Image();
            imgVariableDropDown.ID = "imgVariableDropDown" + this.ID;
            imgVariableDropDown.ImageUrl = "/Images/Icons/VariableSelector/Down.png";

            imgVariableDropDown.Attributes.Add("onclick", string.Format(
                "ShowScores(this, '{0}', '{1}', true, undefined, {2}, {3})",
                this.Variable.Source.Replace("\\", "/"),
                this.Variable.Path,
                this.Editable.ToString().ToLower(),
                this.OnCrosstable.ToString().ToLower()
            ));

            tableCellVariableDropDown.Controls.Add(imgVariableDropDown);

            tableRow.Cells.Add(tableCellVariableType);
            tableRow.Cells.Add(tableCellVariableLabel);
            tableRow.Cells.Add(tableCellVariableDropDown);

            result.Rows.Add(tableRow);

            if (this.OnCrosstable)
            {
                tableCellVariableType.RowSpan = 2;
                tableCellVariableDropDown.RowSpan = 2;

                TableRow tableRowDropAreaNested = new TableRow();

                TableCell tableCellDropAreaNested = new TableCell();
                tableCellDropAreaNested.Attributes.Add("DropArea", "True");
                tableCellDropAreaNested.Attributes.Add("Source", this.Variable.Source.Replace("\\", "/"));
                tableCellDropAreaNested.Attributes.Add("Path", this.Variable.Path);

                tableRowDropAreaNested.Cells.Add(tableCellDropAreaNested);

                result.Rows.Add(tableRowDropAreaNested);
            }

            return result;
        }

        #endregion


        #region Event Handlers

        protected void VariableSelector_Load(object sender, EventArgs e)
        {
            Render();
        }

        protected void VariableSelector_PreRender(object sender, EventArgs e)
        {
        }

        #endregion
    }
}
