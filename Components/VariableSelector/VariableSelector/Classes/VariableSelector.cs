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
    public class VariableSelector : WebUtilities.BaseControl
    {
        #region Properties

        public bool HasData { get; set; }

        /// <summary>
        /// Gets or sets the variable of the variable selector.
        /// </summary>
        public DefinitionObject Variable { get; set; }

        /// <summary>
        /// Gets or sets the settings for the variable selector.
        /// </summary>
        public VariableSelectorSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the alignment of the variable selector.
        /// </summary>
        public VariableSelectorAlignment Alignment { get; set; }

        public bool DisableVariableOptions { get; set; }

        public int ControlHeight { get; set; }
        public int ControlWidth { get; set; }

        public bool IsTaxonomy { get; set; }

        #endregion


        #region Constructor

        public VariableSelector(int idLanguage, DefinitionObject variable, bool editable = true)
        {
            this.IsTaxonomy = true;
            this.HasData = true;
            this.Variable = variable;
            this.Settings = new VariableSelectorSettings(variable.Source.Replace("\\", "/"), variable.Path);

            this.Load += VariableSelector_Load;
        }

        public VariableSelector(int idLanguage, string source, string path, bool editable = true)
        {
            this.IsTaxonomy = true;
            this.HasData = true;
            this.Variable = new DefinitionObject(this.Core, source, path);
            this.Settings = new VariableSelectorSettings(source.Replace("\\", "/"), path);

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
            this.Attributes.Add("IsTaxonomy", this.IsTaxonomy.ToString());

            Panel pnlContainer = new Panel();
            pnlContainer.ID = "VariableSelectorContainer" + this.ID;

            Panel pnlSelector = new Panel();
            pnlSelector.ID = "pnlContainer" + this.ID;
            pnlSelector.CssClass = "VariableSelector";

            pnlSelector.Controls.Add(RenderVariableLabel());

            pnlSelector.Attributes.Add("onclick", "return false;");

            //base.Controls.Add(pnlSelector);

            pnlSelector.Attributes.Add(
                "onmouseout",
                "this.style.background='';this.style.cursor = '';this.style.color='';this.onclick=undefined;"
            );

            pnlContainer.Controls.Add(pnlSelector);

            base.Controls.Add(pnlContainer);

            if (!this.DisableVariableOptions)
            {
                if (Page != null)
                {
                    Page.ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "InitVariableSelector" + this.ID,
                        "loadFunctions.push(function() { InitVariableSelector('" + this.ClientID + "', " + this.Settings.ToJson() + "); });",
                        true
                    );
                }
                else
                {
                    HtmlGenericControl script = new HtmlGenericControl("script");
                    script.Attributes.Add("type", "text/javascript");
                    script.InnerHtml = "InitVariableSelector('" + this.ClientID + "', " + this.Settings.ToJson() + ");";

                    base.Controls.Add(script);
                }
            }
        }

        public Table RenderVariableLabel()
        {
            Table result = new Table();
            result.ID = "lblSelectedItem" + this.ID;
            result.Style.Add("width", "100%");

            result.Attributes.Add("oncontextmenu",string.Format(
                "ShowVariableMenu(this, '{0}', '{1}', {2}); return false;",
                this.Variable.GetValue("Type"),
                this.Variable.GetValue("Name"),
                this.Settings.ToJson()
            ));

            VariableType variableType = ((VariableType)int.Parse(this.Variable.GetValue("Type").ToString()));

            TableCell tableCellVariableType = new TableCell();
            TableCell tableCellVariableLabel = new TableCell();
            TableCell tableCellVariableDropDown = new TableCell();

            tableCellVariableType.CssClass = "VariableSelectorTypeBackground" + variableType;
            tableCellVariableLabel.CssClass = "VariableSelectorVariableLabel";
            tableCellVariableDropDown.CssClass = "VariableSelectorDropDown";

            tableCellVariableType.Style.Add("width", "30px");
            tableCellVariableDropDown.Style.Add("width", "30px");

            Image imgVariableType = new Image();
            imgVariableType.CssClass = "VariableSelectorVariableType";
            imgVariableType.ImageUrl = string.Format(
                "/Images/Icons/VariableSelector/{0}.png",
                variableType
            );

            if (!this.DisableVariableOptions)
            {
                imgVariableType.Attributes.Add("onclick", string.Format(
                    "ShowVariableOptions(this, '{0}', '{1}');return false;",
                    this.Variable.Source.Replace("\\", "/"),
                    this.Variable.Path
                ));

                imgVariableType.Attributes.Add("onmouseover", "this.src=\"/Images/Icons/Swiper.png\"");
                imgVariableType.Attributes.Add("onmouseout", string.Format(
                    "this.src=\"/Images/Icons/VariableSelector/{0}.png\"",
                    ((VariableType)int.Parse(this.Variable.GetValue("Type").ToString()))
                ));
            }

            tableCellVariableLabel.Text = this.Variable.GetLabel(this.Settings.IdLanguage);

            tableCellVariableType.Controls.Add(imgVariableType);

            Image imgVariableDropDown = new Image();
            imgVariableDropDown.ID = "imgVariableDropDown" + this.ID;
            imgVariableDropDown.ImageUrl = "/Images/Icons/VariableSelector/Down.png";

            tableCellVariableDropDown.Controls.Add(imgVariableDropDown);

            if (this.Alignment == VariableSelectorAlignment.Horizontal)
            {
                TableRow tableRow = new TableRow();

                tableRow.Cells.Add(tableCellVariableType);

                if (!ChechHasData(Guid.Parse(this.Variable.GetValue("Id").ToString())))
                {
                    TableCell tableCellWarning = new TableCell();
                    tableCellWarning.CssClass = "VariableSelectorVariableWarning";
                    tableCellWarning.Text = "<img src=\"/Images/Icons/Warning2.png\" />";

                    tableRow.Cells.Add(tableCellWarning);
                }

                tableRow.Cells.Add(tableCellVariableLabel);

                result.Rows.Add(tableRow);

                tableRow.Cells.Add(tableCellVariableDropDown);
            }
            else
            {
                int heightS = 60;
                heightS += 60;

                if (this.Settings.Dragable)
                    heightS += 60;

                if (this.ControlHeight > 0)
                    tableCellVariableLabel.Style.Add("height", (this.ControlHeight - heightS) + "px");

                tableCellVariableType.Style.Add("height", "60px");

                TableRow tableRowVariableType = new TableRow();
                TableRow tableRowLabel = new TableRow();
                TableRow tableRowDropDown = new TableRow();

                tableRowVariableType.Cells.Add(tableCellVariableType);
                tableRowLabel.Cells.Add(tableCellVariableLabel);
                tableRowDropDown.Cells.Add(tableCellVariableDropDown);

                result.Rows.Add(tableRowVariableType);
                result.Rows.Add(tableRowLabel);

                result.Rows.Add(tableRowDropDown);
            }

            return result;
        }

        /// <summary>
        /// Checks if there is data for a category.
        /// </summary>
        /// <param name="idCategory">The id of the category to check data for.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public bool ChechHasData(Guid idVariable)
        {
            List<string> commandTexts = new List<string>();

           
                List<object[]> variableLinks;

                variableLinks = this.Core.VariableLinks.GetValues(
                    new string[] { "IdVariable" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { idVariable }
                );

                // Run through all linked variable ids.
                foreach (object[] variableLink in variableLinks)
                {
                    commandTexts.Add(string.Format(
                        "SELECT Count(*) FROM [resp].[Var_{0}]",
                        variableLink[0]
                    ));
                }
            


            foreach (string commandText in commandTexts)
            {
                int count = (int)this.Core.Respondents.ExecuteReader(commandText, typeof(int))[0][0];

                if (count > 0)
                    return true;
            }

            return false;
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

    public class VariableSelectorSettings
    {
        #region Properties

        public bool EnableRename { get; set; }
        public bool EnableDelete { get; set; }

        public bool FullScreen { get; set; }
        public bool SmoothEffect { get; set; }
        public bool EnableCombine { get; set; }
        public bool EnableCategorize { get; set; }
        public bool Dragable { get; set; }

        public int IdLanguage { get; set; }

        public string Source { get; set; }
        public string Path { get; set; }

        public VariableSelectorSettingsScoreOptions ScoreOptions { get; set; }

        #endregion


        #region Constructor

        public VariableSelectorSettings(string source, string path)
        {
            this.EnableRename = true;
            this.EnableDelete = true;
            this.FullScreen = true;
            this.SmoothEffect = true;
            this.EnableCombine = true;
            this.EnableCategorize = false;
            this.Dragable = false;

            this.IdLanguage = 2057;

            this.Source = source;
            this.Path = path;

            this.ScoreOptions = new VariableSelectorSettingsScoreOptions(this);
        }

        #endregion


        #region Methods

        public string ToJson()
        {
            StringBuilder result = new StringBuilder();
            result.Append("{");

            result.Append(this.ToJson("EnableRename", this.EnableRename) + ",");
            result.Append(this.ToJson("EnableDelete", this.EnableDelete) + ",");
            result.Append(this.ToJson("FullScreen", this.FullScreen) + ",");
            result.Append(this.ToJson("SmoothEffect", this.SmoothEffect) + ",");
            result.Append(this.ToJson("EnableCombine", this.EnableCombine) + ",");
            result.Append(this.ToJson("EnableCategorize", this.EnableCategorize) + ",");
            result.Append(this.ToJson("Dragable", this.Dragable) + ",");
            result.Append(this.ToJson("IdLanguage", this.IdLanguage) + ",");
            result.Append(this.ToJson("Source", this.Source) + ",");
            result.Append(this.ToJson("Path", this.Path) + ",");

            result.Append("\"ScoreOptions\": ");
            result.Append(this.ScoreOptions.ToJson());

            result.Append("}");

            return result.ToString();
        }

        public string ToJson(string name, object value)
        {
            string valueStr = "";

            switch (value.GetType().Name)
            {
                case "Boolean":
                    valueStr = value.ToString().ToLower();
                    break;
                case "Int16":
                case "Int32":
                case "Int64":
                    valueStr = value.ToString().ToLower();
                    break;
                case "Double":
                    valueStr = ((double)value).ToString(new CultureInfo(2057));
                    break;
                default:
                    valueStr = "\"" + value.ToString().Replace("\"", "\\\"") + "\"";
                    break;
            }

            return string.Format(
                "\"{0}\": {1}",
                name,
                valueStr
            );
        }

        #endregion
    }

    public class VariableSelectorSettingsScoreOptions
    {
        #region Properties

        public VariableSelectorSettings Owner { get; set; }

        public bool Factor { get; set; }
        public bool Name { get; set; }
        public bool Hide { get; set; }
        public bool Delete { get; set; }
        public bool Equation { get; set; }

        #endregion


        #region Constructor

        public VariableSelectorSettingsScoreOptions(VariableSelectorSettings owner)
        {
            this.Owner = owner;

            this.Factor = true;
            this.Name = false;
            this.Hide = true;
            this.Delete = false;
            this.Equation = true;
        }

        #endregion


        #region Methods

        public string ToJson()
        {
            StringBuilder result = new StringBuilder();
            result.Append("{");

            result.Append(this.Owner.ToJson("Factor", this.Factor) + ",");
            result.Append(this.Owner.ToJson("Name", this.Name) + ",");
            result.Append(this.Owner.ToJson("Hide", this.Hide) + ",");
            result.Append(this.Owner.ToJson("Delete", this.Delete) + ",");
            result.Append(this.Owner.ToJson("Equation", this.Equation));

            result.Append("}");

            return result.ToString();
        }

        #endregion
    }

    public enum VariableSelectorAlignment
    {
        Horizontal,
        Vertical
    }
}
