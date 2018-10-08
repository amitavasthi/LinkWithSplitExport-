using Crosstables.Classes.HierarchyClasses;
using Crosstables.Classes.ReportDefinitionClasses;
using DatabaseCore.Items;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Xml;
using WebUtilities;
using WebUtilities.Controls;

namespace Crosstables.Classes
{
    public class Crosstable : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database core is used.
        /// </summary>
        public DatabaseCore.Core Core { get; set; }

        /// <summary>
        /// Gets or sets the full path to the report definition xml file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the report definition xml document.
        /// </summary>
        public ReportDefinition ReportDefinition { get; set; }

        /// <summary>
        /// Gets or sets the report calculator which is used to aggregate the data.
        /// </summary>
        public ReportCalculator ReportCalculator { get; set; }

        public TableCell TableCellFilter { get; set; }

        public Table Table { get; set; }

        public bool IsExport { get; set; }


        public int ContentWidth
        {
            get
            {
                if (HttpContext.Current.Session["ContentWidth"] == null)
                    HttpContext.Current.Session["ContentWidth"] = 0;

                return (int)HttpContext.Current.Session["ContentWidth"];
            }
        }

        public int ContentHeight
        {
            get
            {
                if (HttpContext.Current.Session["ContentHeight"] == null)
                    HttpContext.Current.Session["ContentHeight"] = 0;

                return (int)HttpContext.Current.Session["ContentHeight"];
            }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the crosstable control.
        /// </summary>
        /// <param name="fileName">The full path to the report definition file.</param>
        public Crosstable(DatabaseCore.Core core, string fileName)
        {
            this.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
            this.Core = core;
            this.FileName = fileName;

            if (!File.Exists(fileName))
                throw new Exception("Report definition not found.");

            this.ReportDefinition = new ReportDefinition(
                this.Core,
                fileName,
                ((HierarchyFilterCollection)HttpContext.Current.Session["HierarchyFilters"])[fileName]
            );

            this.Load += Crosstable_Load;
        }

        #endregion


        #region Methods

        private void ApplyCellHeight(TableCell tableCell)
        {
            if (tableCell.RowSpan == 0)
                tableCell.RowSpan = 1;

            int height = (int)((cellHeight * tableCell.RowSpan));

            tableCell.Style.Add("min-height", height + "px");
            tableCell.Style.Add("max-height", height + "px");
            tableCell.Style.Add("height", height + "px");
        }


        private void InitCalculator()
        {
            this.ReportCalculator = new ReportCalculator(
                this.ReportDefinition,
                this.Core,
                HttpContext.Current.Session
            );

            if (!this.IsExport)
            {
                this.ReportDefinition.Settings.AutoUpdate = false;
            }
            else
            {
                this.ReportDefinition.Settings.AutoUpdate = true;
            }


            if (this.ReportDefinition.HasData == false &&
                this.ReportDefinition.Settings.AutoUpdate)
                this.ReportCalculator.Aggregate((string)HttpContext.Current.Session["Version"]);
        }


        int leftVariableCount = 0;
        int topVariableCounter = 0;

        private void RenderTopVariableLabels(
            List<ReportDefinitionVariable> topVariables,
            int level,
            bool renderNested = true,
            string sigDiffIdentity = ""
        )
        {
            foreach (ReportDefinitionVariable topVariable in topVariables)
            {
                sigDiffIdentity += topVariable.IdVariable;

                if (leftVariableCount == 0 || topVariable.ParentVariable != null)
                {
                    // Create a new table cell for the top variable label.
                    TableCell tableCellVariable = new TableCell();
                    tableCellVariable.ColumnSpan = topVariable.ColumnSpan;
                    tableCellVariable.CssClass = "TableCellHeadline TableCellHeadlineTopVariable BackgroundColor7 ";
                    //tableCellVariable.Style.Add("width", (cellWidth * topVariable.ColumnSpan) + "px");

                    // Check if the top variable is on root level.
                    if (topVariable.ParentVariable == null)
                    {
                        tableCellVariable.CssClass += "TableCellRootTopVariable";

                        // Increase the variable label table cell's column span by the
                        // nested level of the left variable times two (variable label + category label),
                        // substracted by one (variable label should include the weighting information).
                        //tableCellVariable.ColumnSpan += (leftVariable.NestedLevels * 2) - 1;
                    }
                    else
                    {
                        tableCellVariable.CssClass += string.Format(
                            "GrayBackgroundColor{0} TableCellHeadlineNestedTopVariable",
                            /*(topVariableCounter++ % 2 == 0) ? "20" : "21"*/
                            "7"
                        );

                        tableCellVariable.Style.Add("height", "60px");
                    }

                    if (this.IsExport)
                    {
                        tableCellVariable.Text = topVariable.Label;
                    }
                    else
                    {
                        if (!topVariable.IsFake)
                        {
                            int topCategoryCellWidth = (cellWidth * tableCellVariable.ColumnSpan) - ((tableCellVariable.ColumnSpan - 1) * 2);

                            //tableCellVariable.CssClass += "BackgroundColor7 Color1";
                            tableCellVariable.Attributes.Add("onmouseover", string.Format(
                                "ShowToolTip(this, this.textContent, undefined, 'Top');"
                            ));

                            tableCellVariable.Text += string.Format(
                                "<div style=\"height:60px;max-height:60px;overflow:hidden;\"><div style=\"height:60px\" IdSelected=\"{4}\" DropArea=\"True\" DropAreaMessage=\"DropAreaMessageReplace\" Source=\"{1}\" Path=\"{3}\">" +
                                "<table cellspacing=\"0\" cellpadding=\"0\" style=\"width:100%;height:100%;\"><tr><td><div class=\"VariableLabel\" style=\"max-width:{5}px;width:{5}px;overflow:hidden;\">{0}</div></td></tr></table></div>" +
                                "<div style=\"height:60px;display:none;\" DropArea=\"True\" DropAreaMessage=\"DropAreaMessageNest\" Position=\"Top\" Source=\"{1}\" Path=\"{2}\"></div></div>",
                                topVariable.Label,
                                this.ReportDefinition.FileName.Replace("\\", "/"),
                                HttpUtility.UrlEncode(topVariable.XmlNode.GetXPath(true)),
                                HttpUtility.UrlEncode(topVariable.XmlNode.ParentNode.GetXPath(true)),
                                topVariable.IdVariable,
                                topCategoryCellWidth
                            );

                            tableCellVariable.Attributes.Add("oncontextmenu", string.Format(
                                "ShowVariableOptions2('{0}', '{1}'); return false;",
                                this.ReportDefinition.FileName.Replace("\\", "/"),
                                topVariable.XmlNode.GetXPath(true)
                            ));

                            /*int topCategoryCellWidth = (cellWidth * tableCellVariable.ColumnSpan) - ((tableCellVariable.ColumnSpan - 1) * 2);

                            VariableSelector1.Classes.VariableSelector variableSelector = new VariableSelector1.Classes.VariableSelector(
                                this.ReportDefinition.Settings.IdLanguage,
                                this.ReportDefinition.FileName,
                                topVariable.XmlNode.GetXPath(true),
                                false
                            );
                            variableSelector.OnCrosstable = true;
                            variableSelector.ControlWidth = topCategoryCellWidth;

                            variableSelector.Attributes.Add("onmouseover", string.Format(
                                "ShowToolTip(this, '{0}', undefined, 'Top');",
                                topVariable.Label
                            ));

                            if (this.AsynchRender)
                                variableSelector.Render();

                            tableCellVariable.Controls.Add(variableSelector);*/
                        }
                        else
                        {
                            tableCellVariable.Style.Add("width", cellWidth + "px");
                            tableCellVariable.RowSpan = 4;

                            tableCellVariable.Attributes.Add("DropArea", "True");
                            tableCellVariable.Attributes.Add("Source", this.ReportDefinition.FileName.Replace("\\", "/"));
                            tableCellVariable.Attributes.Add("Path", topVariable.XmlNode.ParentNode.GetXPath(true));
                            tableCellVariable.Attributes.Add("DropAreaMessage", "DropAreaMessageAdd");
                        }
                    }

                    tableRowValues.Cells.Add(tableCellVariable);
                }

                if (!topVariable.IsFake)
                {
                    int topCategoryCellWidth;

                    // Check if the unweighted base should be displayed.
                    if (this.ReportDefinition.Settings.DisplayUnweightedBase && topVariable.NestedVariables.Count == 0)
                    {
                        //// Create a new table cell for the base label.
                        //TableCell tableCellUnweightedBase = new TableCell();
                        //tableCellUnweightedBase.Text = base.LanguageManager.GetText("BaseUnweightedTop");
                        //tableCellUnweightedBase.CssClass = "TableCellHeadline TableCellHeadlineTopBase TableCellHeadlineTopBaseUnweighted TableCellHeadlineBase BackgroundColor9 ";
                        //tableCellUnweightedBase.RowSpan = 1;
                        //tableCellUnweightedBase.ColumnSpan = 1;

                        //topCategoryCellWidth = (cellWidth * tableCellUnweightedBase.ColumnSpan) - ((tableCellUnweightedBase.ColumnSpan - 1) * 2);

                        //tableCellUnweightedBase.Style.Add("min-width", topCategoryCellWidth + "px");
                        //tableCellUnweightedBase.Style.Add("max-width", topCategoryCellWidth + "px");
                        //tableCellUnweightedBase.Style.Add("width", topCategoryCellWidth + "px");

                        //tableCellUnweightedBase.RowSpan += (maxTopNestedLevel - topVariable.NestedLevelsBase) * 2;

                        //if (this.ReportDefinition.SignificanceTest && topVariable.NestedVariables.Count == 0)
                        //    tableCellUnweightedBase.RowSpan += 1;

                        //if (topVariable.NestedVariables.Count == 0)
                        //    tableCellUnweightedBase.Style.Add("border-bottom-width", "0px");

                        //tableRowSigDiff.Cells.Add(tableCellUnweightedBase);
                    }

                    // Check if the Effectiveted base should be displayed.
                    if (this.ReportDefinition.Settings.DisplayEffectiveBase && topVariable.NestedVariables.Count == 0)
                    {
                        // Create a new table cell for the base label.
                        //TableCell tableCellEffectiveBase = new TableCell();
                        //tableCellEffectiveBase.Text = base.LanguageManager.GetText("BaseEffectiveTop");
                        //tableCellEffectiveBase.CssClass = "TableCellHeadline TableCellHeadlineTopBase TableCellHeadlineTopBaseUnweighted TableCellHeadlineBase BackgroundColor9 ";
                        //tableCellEffectiveBase.RowSpan = 1;
                        //tableCellEffectiveBase.ColumnSpan = 1;

                        //topCategoryCellWidth = (cellWidth * tableCellEffectiveBase.ColumnSpan) - ((tableCellEffectiveBase.ColumnSpan - 1) * 2);

                        //tableCellEffectiveBase.Style.Add("min-width", topCategoryCellWidth + "px");
                        //tableCellEffectiveBase.Style.Add("max-width", topCategoryCellWidth + "px");
                        //tableCellEffectiveBase.Style.Add("width", topCategoryCellWidth + "px");

                        //tableCellEffectiveBase.RowSpan += (maxTopNestedLevel - topVariable.NestedLevelsBase) * 2;

                        //if (this.ReportDefinition.SignificanceTest && topVariable.NestedVariables.Count == 0)
                        //    tableCellEffectiveBase.RowSpan += 1;

                        //if (topVariable.NestedVariables.Count == 0)
                        //    tableCellEffectiveBase.Style.Add("border-bottom-width", "0px");

                        //tableRowSigDiff.Cells.Add(tableCellEffectiveBase);
                    }

                    // Create a new table cell for the base label.
                    TableCell tableCellBase = new TableCell();
                    tableCellBase.CssClass = "TableCellHeadline TableCellHeadlineBase BackgroundColor9 ";
                    tableCellBase.ColumnSpan = topVariable.ColumnSpanBase;
                    if (this.ReportDefinition.Settings.SignificanceTestType == 4)
                    {
                        tableCellBase.RowSpan = 0;
                    }
                    else
                    {
                        tableCellBase.RowSpan = 1;
                    }
                    tableCellBase.Text = base.LanguageManager.GetText("Base");

                    if (topVariable.NestedVariables.Count == 0)
                        tableCellBase.Style.Add("border-bottom-width", "0px");

                    /*tableCellBase.Text = string.Format(
                        "<table cellspacing=\"0\" cellpadding=\"0\" style=\"width:100%;height:100%;\"><tr><td><div class=\"VariableLabel\" style=\"max-width:{1}px;width:{1}px;overflow:hidden;\">{0}</div></td></tr></table>",
                        base.LanguageManager.GetText("Base"),
                        topCategoryCellWidth
                    );*/

                    if (!this.ReportDefinition.Settings.DisplayUnweightedBase)
                        tableCellBase.CssClass += " TableCellHeadlineTopBase";

                    if (!this.ReportDefinition.Settings.DisplayEffectiveBase)
                        tableCellBase.CssClass += " TableCellHeadlineTopBase";

                    topCategoryCellWidth = (cellWidth * tableCellBase.ColumnSpan) - ((tableCellBase.ColumnSpan - 1) * 2);

                    tableCellBase.Style.Add("min-width", topCategoryCellWidth + "px");
                    tableCellBase.Style.Add("max-width", topCategoryCellWidth + "px");
                    tableCellBase.Style.Add("width", topCategoryCellWidth + "px");

                    tableCellBase.RowSpan += (maxTopNestedLevel - topVariable.NestedLevelsBase) * 2;

                    if (this.ReportDefinition.SignificanceTest && topVariable.NestedVariables.Count == 0)
                        tableCellBase.RowSpan += 1;

                    tableRowSigDiff.Cells.Add(tableCellBase);
                }

                /*string cellHeight = CalculateCellHeight(topVariable).ToString(
                    new System.Globalization.CultureInfo("en-GB")
                ) + "px";*/
                string cellOverflowHeight = (CalculateCellHeight(topVariable) - 10).ToString(
                    new System.Globalization.CultureInfo("en-GB")
                ) + "px";

                if (topVariable.NestedVariables.Count == 0)
                {
                    tableRowSigDiff.Attributes.Add(
                        "IsTitle",
                        "True"
                    );

                    tableRowPercentage.Attributes.Add(
                        "IsTitle",
                        "True"
                    );
                }

                if (!topVariable.IsFake)
                {
                    if (topVariable.VariableType == VariableType.Numeric)
                    {
                        string category = "";

                        for (int i = 0; i < 4; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    category = "Mean";
                                    break;
                                case 1:
                                    category = "Min";
                                    break;
                                case 2:
                                    category = "Max";
                                    break;
                                case 3:
                                    category = "StdDev";
                                    break;
                            }

                            // Create a new table cell for the category label.
                            TableCell tableCellCategory = new TableCell();
                            tableCellCategory.CssClass = "TableCellHeadline TableCellHeadlineCategory BackgroundColor9 ";
                            tableCellCategory.Text = "<div style='overflow:hidden;max-height:" + cellOverflowHeight + ";'>" +
                                base.LanguageManager.GetText(category) + "</div>";

                            if (topVariable.NestedVariables.Count == 0)
                                tableCellCategory.Style.Add("border-bottom-width", "0px");

                            tableCellCategory.Style.Add("min-width", cellWidth + "px");
                            tableCellCategory.Style.Add("max-width", cellWidth + "px");
                            tableCellCategory.Style.Add("width", cellWidth + "px");

                            tableRowSigDiff.Cells.Add(tableCellCategory);

                            if (topVariable.NestedVariables.Count == 0 && this.ReportDefinition.SignificanceTest && topVariable.IsFake == false)
                            {
                                tableCellCategory.RowSpan = 2;
                            }
                            else
                            {
                                tableCellCategory.RowSpan = 1;
                            }

                            tableCellCategory.RowSpan += (maxTopNestedLevel - topVariable.NestedLevelsBase) * 2;
                        }
                    }
                    else
                    {
                        // Run through all categories of the top variable.
                        foreach (ReportDefinitionScore topScore in topVariable.Scores)
                        {
                            // Check if the scores is hidden.
                            if (topScore.Hidden || (topScore.HasValues == false && this.ReportDefinition.Settings.HideEmptyRowsAndColumns))
                                continue;

                            // Check if the score is available in the selected hierarchy.
                            if (topVariable.IsTaxonomy &&
                                topScore.Persistent &&
                                this.ReportDefinition.HierarchyFilter.
                                TaxonomyCategories.ContainsKey(topScore.Identity) == false)
                                continue;

                            // Create a new table cell for the category label.
                            TableCell tableCellCategory = new TableCell();
                            tableCellCategory.CssClass = "TableCellHeadline TableCellHeadlineCategory BackgroundColor9 ";
                            tableCellCategory.ColumnSpan = topScore.ColumnSpan;

                            if (topVariable.NestedVariables.Count == 0)
                                tableCellCategory.Style.Add("border-bottom-width", "0px");

                            int topCategoryCellWidth = (cellWidth * tableCellCategory.ColumnSpan) - ((tableCellCategory.ColumnSpan - 1) * 2);

                            tableCellCategory.Style.Add("min-width", topCategoryCellWidth + "px");
                            tableCellCategory.Style.Add("max-width", topCategoryCellWidth + "px");
                            tableCellCategory.Style.Add("width", topCategoryCellWidth + "px");

                            tableCellCategory.Text = "<div style='overflow:hidden;max-height:" + cellOverflowHeight + ";'>" + topScore.Label + "</div>";

                            tableCellCategory.Attributes.Add("oncontextmenu", string.Format(
                                "ShowScoreOptions2({1}, '{2}', '{0}'); return false;",
                                topScore.XmlNode.GetXPath(true),
                                topScore.Equation == null ? "false" : "true",
                                this.ReportDefinition.FileName.Replace("\\", "/")
                            ));

                            tableCellCategory.Attributes.Add(
                                "onmouseout",
                                "this.style.background='';this.style.cursor = '';this.style.color='';this.onclick = undefined;"
                            );

                            string tooltip = "this.textContent";

                            if (topScore.XmlNode.Name == "ScoreGroup")
                            {
                                ReportDefinitionScoreGroup scoreGroup = (ReportDefinitionScoreGroup)topScore;

                                if (scoreGroup.Equation != null)
                                {
                                    tooltip = "'" + scoreGroup.Equation + "'";
                                }
                                else
                                {
                                    tooltip = "'<b>' + this.textContent + '</b><br />";

                                    foreach (ReportDefinitionScore score in scoreGroup.Scores)
                                    {
                                        if (!tooltip.Contains("&nbsp;-&nbsp;" + score.Label.Replace("'", "") + "<br />"))
                                            tooltip += "&nbsp;-&nbsp;" + score.Label.Replace("'", "") + "<br />";
                                    }

                                    tooltip += "'";
                                }
                            }

                            tableCellCategory.Attributes.Add(
                                "onmouseover",
                                "ShowToolTip(this, " + tooltip + ", undefined, 'Top');CombineScores('" + topScore.Identity + "', '" + topScore.GetType().Name + "', '" + topScore.XmlNode.GetXPath(true) + "');"
                            );

                            tableCellCategory.Attributes.Add(
                                "Position",
                                "Top"
                            );

                            tableCellCategory.Attributes.Add(
                                "NestLevel",
                                topScore.Variable.NestedLevels.ToString()
                            );

                            tableCellCategory.Attributes.Add(
                                "ScoreType",
                                topScore.GetType().Name
                            );

                            tableCellCategory.Attributes.Add(
                                "XPath",
                                topScore.XmlNode.GetXPath(true)
                            );

                            tableCellCategory.Attributes.Add(
                                "IdCategory",
                                topScore.Identity.ToString()
                            );

                            tableCellCategory.Attributes.Add("IsTitle", "True");

                            if (!topVariable.IsFake)
                            {
                                tableCellCategory.Attributes.Add(
                                    "onmousedown",
                                    "CreateDragCategory(this)"
                                );
                            }

                            if (topVariable.NestedVariables.Count == 0)
                            {
                                tableCellCategory.Attributes.Add(
                                    "SignificanceLetter",
                                    topScore.SignificanceLetter
                                );

                                tableCellCategory.Attributes.Add(
                                    "IdCategory",
                                    topScore.Identity.ToString()
                                );

                                tableCellCategory.Attributes.Add(
                                    "IdVariable",
                                    topVariable.IdVariable.ToString()
                                );
                            }

                            tableRowSigDiff.Cells.Add(tableCellCategory);

                            tableCellCategory.RowSpan = 1;

                            tableCellCategory.RowSpan += (maxTopNestedLevel - topVariable.NestedLevelsBase) * 2;

                            /*if (this.ReportDefinition.SignificanceTest && topVariable.NestedVariables.Count == 0)
                                tableCellCategory.RowSpan += 2;*/
                        }
                    }
                }
                else
                {
                    tableRowValues.Cells.Add(new System.Web.UI.WebControls.TableCell());

                    // Add a empty cell to the sig diff row, so it gets rendered in the excel export.
                    // This is required in order to get proper alignments.
                    tableRowSigDiff.Cells.Add(new System.Web.UI.WebControls.TableCell());
                }
            }

            if (level == 0)
                InitNewRow(tableTopHeadline, true);

            bool renderRow = false;
            foreach (ReportDefinitionVariable topVariable in topVariables)
            {
                // Check if the top variable has nested variables.
                if (topVariable.NestedVariables.Count > 0 && renderNested)
                {
                    renderRow = true;
                    //InitNewRow(tableTopHeadline, true);

                    int baseValueCells = 1;

                    /*if (this.ReportDefinition.Settings.DisplayUnweightedBase)
                        baseValueCells += 1;*/

                    // Run through all categories of the top variable.
                    for (int i = 0; i < topVariable.ScoresCount + baseValueCells; i++)
                    {
                        string nestedSigDiffIdentitiy = sigDiffIdentity;

                        if ((i - 1) >= 0 && (i - 1) < topVariable.ScoresCount)
                            nestedSigDiffIdentitiy += topVariable.Scores[i - 1].Identity;

                        RenderTopVariableLabels(
                            topVariable.NestedVariables,
                            level + 1,
                            false,
                            nestedSigDiffIdentitiy
                        );

                        // Run through all nested top variables.
                        /*foreach (ReportDefinitionVariable nestedTopVariable in topVariable.NestedVariables)
                        {
                            string nestedSigDiffIdentitiy = sigDiffIdentity;

                            if ((i - 1) >= 0 && (i - 1) < topVariable.ScoresCount)
                                nestedSigDiffIdentitiy += topVariable.Scores[i - 1].Identity;

                            RenderTopVariableLabels(
                                nestedTopVariable,
                                false,
                                nestedSigDiffIdentitiy
                            );
                        }*/
                    }
                }
            }

            if (renderRow)
                InitNewRow(tableTopHeadline, true);

            renderRow = false;
            foreach (ReportDefinitionVariable topVariable in topVariables)
            {
                if (topVariable.NestedVariables.Count > 0 && renderNested)
                {
                    int baseValueCells = 1;

                    /*if (this.ReportDefinition.Settings.DisplayUnweightedBase)
                        baseValueCells += 1;*/

                    topVariableCounter = 0;

                    // Run through all categories of the top variable.
                    for (int i = 0; i < topVariable.ScoresCount + baseValueCells; i++)
                    {
                        // Run through all nested top variables.
                        foreach (ReportDefinitionVariable nestedTopVariable in topVariable.NestedVariables)
                        {
                            if (nestedTopVariable.NestedVariables.Count > 0)
                                renderRow = true;

                            // Run through all categories of the top variable.
                            for (int a = 0; a < nestedTopVariable.ScoresCount + baseValueCells; a++)
                            {
                                string nestedSigDiffIdentitiy = sigDiffIdentity;

                                if ((i - 1) >= 0 && (i - 1) < topVariable.ScoresCount)
                                    nestedSigDiffIdentitiy += topVariable.Scores[i - 1].Identity;

                                nestedSigDiffIdentitiy += nestedTopVariable.IdVariable;

                                if ((a - 1) >= 0 && (a - 1) < nestedTopVariable.ScoresCount)
                                    nestedSigDiffIdentitiy += nestedTopVariable.Scores[a - 1].Identity;

                                RenderTopVariableLabels(
                                    nestedTopVariable.NestedVariables,
                                    level + 2,
                                    false,
                                    nestedSigDiffIdentitiy
                                );

                                /*foreach (ReportDefinitionVariable nestedTopVariable2 in nestedTopVariable.NestedVariables)
                                {
                                    RenderTopVariableLabels(
                                        nestedTopVariable2,
                                        false,
                                        nestedSigDiffIdentitiy
                                    );
                                }*/
                            }
                        }
                    }

                    //InitNewRow(tableTopHeadline, true);
                }
            }

            if (renderRow)
                InitNewRow(tableTopHeadline, true);
        }

        TableRow tableRowSignficanceHeadline;
        private void RenderTopSignificanceHeadline(
            List<ReportDefinitionVariable> topVariables
        )
        {
            tableRowSignficanceHeadline.Attributes.Add("IsTitle", "True");

            // Run through all top variables.
            foreach (ReportDefinitionVariable topVariable in topVariables)
            {
                if (topVariable.IsFake)
                    continue;

                // Check if the top variables has nested variables.
                if (topVariable.NestedVariables.Count > 0)
                {
                    for (int i = 0; i < topVariable.ScoresCount + 1; i++)
                    {
                        // Render the significance headline for the top variable's nested variables.
                        RenderTopSignificanceHeadline(
                            topVariable.NestedVariables
                        );
                    }
                }
                else
                {

                    if (this.ReportDefinition.Settings.SignificanceTestType == 4)
                    {
                        // Create a new table cell for the category's sig diff letter.
                        TableCell tableCellSigDiffTotal = new TableCell();
                        tableCellSigDiffTotal.CssClass = "TableCellHeadline TableCellHeadlineSigDiff BackgroundColor9 ";
                        tableCellSigDiffTotal.ColumnSpan = 1;//totalbase
                        tableCellSigDiffTotal.Text = "A";

                        if (topVariable.NestedVariables.Count == 0)
                            tableCellSigDiffTotal.Style.Add("border-bottom-width", "0px");

                        if (tableCellSigDiffTotal.Text == "")
                            tableCellSigDiffTotal.Text = "&nbsp;";

                        tableCellSigDiffTotal.Attributes.Add(
                            "IsTitle",
                            "True"
                        );

                        tableCellSigDiffTotal.Attributes.Add(
                            "SignificanceLetter",
                           "A"
                        );

                        tableCellSigDiffTotal.Attributes.Add(
                            "IdCategory",
                            ""
                        );

                        tableCellSigDiffTotal.Attributes.Add(
                            "IdVariable",
                            topVariable.IdVariable.ToString()
                        );

                        tableRowSignficanceHeadline.Cells.Add(tableCellSigDiffTotal);
                    }


                    // Run through all scores of the top variables.
                    foreach (ReportDefinitionScore topScore in topVariable.Scores)
                    {
                        // Check if the score is available in the selected hierarchy.
                        if (topVariable.IsTaxonomy &&
                            topScore.Persistent &&
                            this.ReportDefinition.HierarchyFilter.
                            TaxonomyCategories.ContainsKey(topScore.Identity) == false)
                            continue;

                        if (topScore.Hidden || (topScore.HasValues == false && this.ReportDefinition.Settings.HideEmptyRowsAndColumns))
                            continue;

                        // Create a new table cell for the category's sig diff letter.
                        TableCell tableCellSigDiff = new TableCell();
                        tableCellSigDiff.CssClass = "TableCellHeadline TableCellHeadlineSigDiff BackgroundColor9 ";
                        tableCellSigDiff.ColumnSpan = topScore.ColumnSpan;
                        tableCellSigDiff.Text = topScore.SignificanceLetter;

                        if (topVariable.NestedVariables.Count == 0)
                            tableCellSigDiff.Style.Add("border-bottom-width", "0px");

                        if (tableCellSigDiff.Text == "")
                            tableCellSigDiff.Text = "&nbsp;";

                        tableCellSigDiff.Attributes.Add(
                            "IsTitle",
                            "True"
                        );

                        tableCellSigDiff.Attributes.Add(
                            "SignificanceLetter",
                            topScore.SignificanceLetter
                        );

                        tableCellSigDiff.Attributes.Add(
                            "IdCategory",
                            topScore.Identity.ToString()
                        );

                        tableCellSigDiff.Attributes.Add(
                            "IdVariable",
                            topVariable.IdVariable.ToString()
                        );

                        tableRowSignficanceHeadline.Cells.Add(tableCellSigDiff);
                        //tableTopHeadline.Rows[tableTopHeadline.Rows.Count - 1].Cells.Add(tableCellSigDiff);
                    }
                }
            }
        }

        private void RenderLeftVariableLabels(
            ReportDefinitionVariable leftVariable,
            string sigDiffIdentity,
            bool isMeanRow = false
        )
        {
            sigDiffIdentity += leftVariable.IdVariable;

            if (leftVariable.ParentVariable != null)
            {
                // Create a new table cell for the left variable label.
                TableCell tableCellVariable = new TableCell();
                tableCellVariable.CssClass = "TableCellHeadline BackgroundColor7 TableCellHeadlineNestedLeftVariable BorderColor7";
                tableCellVariable.RowSpan = leftVariable.RowSpan;
                tableCellVariable.ColumnSpan = (maxLeftNestedLevel - leftVariable.NestedLevelsBase) + 1;

                if (isMeanRow)
                    tableCellVariable.RowSpan = leftVariable.RowSpanMean;

                /*tableCellVariable.Attributes.Add(
                    "oncontextmenu",
                    "PrepareDelete(this, '" + leftVariable.XmlNode.GetXPath(true) + "');return false;"
                );*/

                if (this.IsExport)
                {
                    tableCellVariable.Text = leftVariable.Label;
                }
                else
                {
                    /*tableCellVariable.Text = string.Format(
                        "<table ScrollLabel=\"true\" NestLevel=\"{1}\"><tr><td>{0}</td></tr></table>",
                        leftVariable.Label,
                        leftVariable.NestedLevels
                    );*/

                    tableCellVariable.Attributes.Add("onmouseover", string.Format(
                        "ShowToolTip(this, this.textContent, undefined, 'Left');"
                    ));

                    tableCellVariable.Text += string.Format(
                        "<div ScrollLabel=\"true\" NestLevel=\"{1}\"><div style=\"width:60px;margin-left:60px;display:none;\" DropArea=\"True\" Position=\"Left\" DropAreaMessage=\"DropAreaMessageNest\" Source=\"{2}\" Path=\"{3}\"></div>" +
                        "<table style=\"height:100%;\" IdSelected=\"{5}\" DropArea=\"True\" DropAreaMessage=\"DropAreaMessageReplace\" Source=\"{2}\" Path=\"{4}\"><tr><td><div style=\"width:60px;overflow:hidden\">{0}</div></td></tr></table></div>",
                        leftVariable.Label,
                        leftVariable.NestedLevels,
                        this.ReportDefinition.FileName.Replace("\\", "/"),
                        HttpUtility.UrlEncode(leftVariable.XmlNode.GetXPath(true)),
                        HttpUtility.UrlEncode(leftVariable.XmlNode.ParentNode.GetXPath(true)),
                        leftVariable.IdVariable
                    );

                    tableCellVariable.Attributes.Add("oncontextmenu", string.Format(
                        "ShowVariableOptions2('{0}', '{1}'); return false;",
                        this.ReportDefinition.FileName.Replace("\\", "/"),
                        leftVariable.XmlNode.GetXPath(true)
                    ));
                }

                int nestedLevel = leftVariable.NestedLevels;

                if (!nestedLeftVariableCounter.ContainsKey(nestedLevel))
                    nestedLeftVariableCounter.Add(nestedLevel, 0);

                //tableRowValues.Cells.Add(tableCellVariable);
                tableRowLeftHeadline.Cells.Add(tableCellVariable);
            }
            else
            {
                // Create a new table cell for the left variable label.
                TableCell tableCellVariable = new TableCell();
                tableCellVariable.CssClass = "TableCellHeadline TableCellHeadlineLeftVariable ";
                tableCellVariable.RowSpan = leftVariable.RowSpan;
                tableCellVariable.ColumnSpan = (maxLeftNestedLevel - leftVariable.NestedLevelsBase) + 1;

                if (isMeanRow)
                    tableCellVariable.RowSpan = leftVariable.RowSpanMean;

                if (leftVariable.IsFake)
                {
                    tableCellVariable.RowSpan = 1;
                }

                tableCellVariable.Attributes.Add("oncontextmenu", string.Format(
                    "ShowVariableOptions2('{0}', '{1}'); return false;",
                    this.ReportDefinition.FileName.Replace("\\", "/"),
                    leftVariable.XmlNode.GetXPath(true)
                ));

                if (this.IsExport)
                {
                    tableCellVariable.CssClass += " BackgroundColor7";
                    tableCellVariable.Text = leftVariable.Label;
                }
                else
                {
                    if (!leftVariable.IsFake)
                    {
                        tableCellVariable.CssClass += "BackgroundColor7";

                        /*tableCellVariable.Text = string.Format(
                            "",
                            leftVariable.Label,
                            leftVariable.NestedLevels
                        );*/
                        tableCellVariable.Attributes.Add("onmouseover", string.Format(
                            "ShowToolTip(this, this.textContent, undefined, 'Left');"
                        ));

                        tableCellVariable.Text += string.Format(
                            "<div ScrollLabel=\"true\" NestLevel=\"{1}\" style=\"max-height:{6}px;\"><div style=\"width:60px;margin-left:60px;display:none;\" DropArea=\"True\" Position=\"Left\" DropAreaMessage=\"DropAreaMessageNest\" Source=\"{2}\" Path=\"{3}\"></div>" +
                            "<table style=\"height:100%;\" IdSelected=\"{5}\" DropArea=\"True\" DropAreaMessage=\"DropAreaMessageReplace\" Source=\"{2}\" Path=\"{4}\"><tr><td><div style=\"width:60px;overflow:hidden\">{0}</div></td></tr></table></div>",
                            leftVariable.Label,
                            leftVariable.NestedLevels,
                            this.ReportDefinition.FileName.Replace("\\", "/"),
                            HttpUtility.UrlEncode(leftVariable.XmlNode.GetXPath(true)),
                            HttpUtility.UrlEncode(leftVariable.XmlNode.ParentNode.GetXPath(true)),
                            leftVariable.IdVariable,
                            (leftVariable.RowSpan * cellHeight) - 5
                        );
                    }
                    else
                    {
                        tableCellVariable.CssClass += "BackgroundColor7";
                        tableCellVariable.Style.Add("height", cellHeight + "px");
                        tableCellVariable.ColumnSpan = 2;

                        tableCellVariable.Attributes.Add("DropArea", "True");
                        tableCellVariable.Attributes.Add("Source", this.ReportDefinition.FileName.Replace("\\", "/"));
                        tableCellVariable.Attributes.Add("Path", leftVariable.XmlNode.ParentNode.GetXPath(true));
                        tableCellVariable.Attributes.Add("DropAreaMessage", "DropAreaMessageAdd");
                    }
                }

                tableRowLeftHeadline.Cells.Add(tableCellVariable);
            }

            if (!leftVariable.IsFake)
            {
                TableCell tableCellSpacer;

                // Check if the unweighted base should be displayed.
                if (this.ReportDefinition.Settings.DisplayUnweightedBase && leftVariable.NestedVariables.Count == 0)
                {
                    // Create a new table cell for the base label.
                    TableCell tableCellLeftBaseUnweighted = new TableCell();
                    tableCellLeftBaseUnweighted.CssClass = "TableCellHeadline TableCellHeadlineLeftCategory TableCellHeadlineLeftBase BackgroundColor9";
                    tableCellLeftBaseUnweighted.RowSpan = leftVariable.RowSpanBase;
                    //tableCellLeftBaseUnweighted.ColumnSpan = 1;
                    tableCellLeftBaseUnweighted.ColumnSpan = (maxLeftNestedLevel - leftVariable.NestedLevelsBase) + 1;
                    tableCellLeftBaseUnweighted.Style.Add("max-width", (70 * tableCellLeftBaseUnweighted.ColumnSpan) + "px");

                    tableCellLeftBaseUnweighted.Text = string.Format(
                        "<table style=\"max-height:{0}px;overflow:hidden;width:100%;\" {3} NestLevel=\"{2}.5\"><tr><td style=\"min-width:64px; max-width:64px; width:64px;border-bottom-width:0px\">{1}</td></tr></table>",
                        (cellHeight * tableCellLeftBaseUnweighted.RowSpan) - 5,
                        base.LanguageManager.GetText("BaseUnweightedLeft"),
                        leftVariable.NestedLevels,
                        leftVariable.NestedVariables.Count != 0 ? "ScrollLabel=\"true\"" : ""
                    );

                    ApplyCellHeight(tableCellLeftBaseUnweighted);

                    tableRowLeftHeadline.Cells.Add(tableCellLeftBaseUnweighted);

                    if (leftVariable.NestedVariables.Count == 0)
                    {
                        tableCellSpacer = new TableCell() { CssClass = "TableCellValue TableCellLeftHeadlineSpacer", Text = "&nbsp;" };
                        ApplyCellHeight(tableCellSpacer);
                        tableRowLeftHeadline.Cells.Add(tableCellSpacer);

                        if (this.IsExport)
                            tableCellSpacer.Attributes.Add("ExportWidth", "0");
                    }

                    tableLeftHeadline.Rows.Add(tableRowLeftHeadline);

                    // Run through all nested variables of the left variable.
                    foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
                    {
                        RenderLeftVariableLabels(
                            nestedLeftVariable,
                            ""
                        );
                    }


                    tableRowLeftHeadline = new TableRow();
                    tableRowLeftHeadline.Attributes.Add("IsTitle", "True");

                    for (int i = 0; i < tableCellLeftBaseUnweighted.RowSpan - 1; i++)
                    {
                        tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                        tableRowLeftHeadline = new TableRow();
                        tableRowLeftHeadline.Attributes.Add("IsTitle", "True");
                    }

                    //tableCellLeftBaseUnweighted.ColumnSpan += (maxLeftNestedLevel - leftVariable.NestedLevels) * 2;

                    InitNewRow(tableData);
                }

                // Check if the Effective base should be displayed.
                if (this.ReportDefinition.Settings.DisplayEffectiveBase && leftVariable.NestedVariables.Count == 0)
                {
                    // Create a new table cell for the base label.
                    TableCell tableCellLeftBaseEffective = new TableCell();
                    tableCellLeftBaseEffective.CssClass = "TableCellHeadline TableCellHeadlineLeftCategory TableCellHeadlineLeftBase BackgroundColor9";
                    tableCellLeftBaseEffective.RowSpan = leftVariable.RowSpanBase;
                    //tableCellLeftBaseEffective.ColumnSpan = 1;
                    tableCellLeftBaseEffective.ColumnSpan = (maxLeftNestedLevel - leftVariable.NestedLevelsBase) + 1;
                    tableCellLeftBaseEffective.Style.Add("max-width", (70 * tableCellLeftBaseEffective.ColumnSpan) + "px");

                    tableCellLeftBaseEffective.Text = string.Format(
                        "<table style=\"max-height:{0}px;overflow:hidden;width:100%;\" {3} NestLevel=\"{2}.5\"><tr><td style=\"min-width:64px; max-width:64px; width:64px;border-bottom-width:0px\">{1}</td></tr></table>",
                        (cellHeight * tableCellLeftBaseEffective.RowSpan) - 5,
                        base.LanguageManager.GetText("BaseEffectiveLeft"),
                        leftVariable.NestedLevels,
                        leftVariable.NestedVariables.Count != 0 ? "ScrollLabel=\"true\"" : ""
                    );

                    ApplyCellHeight(tableCellLeftBaseEffective);

                    tableRowLeftHeadline.Cells.Add(tableCellLeftBaseEffective);

                    if (leftVariable.NestedVariables.Count == 0)
                    {
                        tableCellSpacer = new TableCell() { CssClass = "TableCellValue TableCellLeftHeadlineSpacer", Text = "&nbsp;" };
                        ApplyCellHeight(tableCellSpacer);
                        tableRowLeftHeadline.Cells.Add(tableCellSpacer);

                        if (this.IsExport)
                            tableCellSpacer.Attributes.Add("ExportWidth", "0");
                    }

                    tableLeftHeadline.Rows.Add(tableRowLeftHeadline);

                    // Run through all nested variables of the left variable.
                    foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
                    {
                        RenderLeftVariableLabels(
                            nestedLeftVariable,
                            ""
                        );
                    }


                    tableRowLeftHeadline = new TableRow();
                    tableRowLeftHeadline.Attributes.Add("IsTitle", "True");

                    for (int i = 0; i < tableCellLeftBaseEffective.RowSpan - 1; i++)
                    {
                        tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                        tableRowLeftHeadline = new TableRow();
                        tableRowLeftHeadline.Attributes.Add("IsTitle", "True");
                    }

                    //tableCellLeftBaseEffective.ColumnSpan += (maxLeftNestedLevel - leftVariable.NestedLevels) * 2;

                    InitNewRow(tableData);
                }

                // Create a new table cell for the base label.
                TableCell tableCellLeftBase = new TableCell();
                tableCellLeftBase.CssClass = "TableCellHeadline TableCellHeadlineLeftCategory BackgroundColor9 ";
                tableCellLeftBase.RowSpan = leftVariable.RowSpanBase;
                //tableCellLeftBase.ColumnSpan = 1;
                tableCellLeftBase.ColumnSpan = (maxLeftNestedLevel - leftVariable.NestedLevelsBase) + 1;
                tableCellLeftBase.Style.Add("max-width", (70 * tableCellLeftBase.ColumnSpan) + "px");

                tableCellLeftBase.Text = string.Format(
                    "<table style=\"max-height:{0}px;overflow:hidden;width:100%;\" {3} NestLevel=\"{2}.5\"><tr><td>{1}</td></tr></table>",
                    (cellHeight * tableCellLeftBase.RowSpan) - 5,
                    this.ReportDefinition.Settings.BaseType == BaseType.TotalBase ? base.LanguageManager.GetText("BaseTotalLeft") : base.LanguageManager.GetText("BaseAnsweringLeft"),
                    leftVariable.NestedLevels,
                    leftVariable.NestedVariables.Count != 0 ? "ScrollLabel=\"true\"" : ""
                );

                if (!this.ReportDefinition.Settings.DisplayUnweightedBase)
                    tableCellLeftBase.CssClass += "TableCellHeadlineLeftBase ";

                if (leftVariable.NestedVariables.Count == 0)
                    ApplyCellHeight(tableCellLeftBase);

                tableRowLeftHeadline.Cells.Add(tableCellLeftBase);

                if (leftVariable.NestedVariables.Count == 0)
                {
                    tableCellSpacer = new TableCell() { CssClass = "TableCellValue TableCellLeftHeadlineSpacer", Text = "&nbsp;" };
                    ApplyCellHeight(tableCellSpacer);
                    tableRowLeftHeadline.Cells.Add(tableCellSpacer);

                    if (this.IsExport)
                        tableCellSpacer.Attributes.Add("ExportWidth", "0");
                }

                // Run through all nested variables of the left variable.
                foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
                {
                    RenderLeftVariableLabels(
                        nestedLeftVariable,
                        ""
                    );
                }

                if (leftVariable.NestedVariables.Count == 0)
                {
                    tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                    tableRowLeftHeadline = new TableRow();
                    tableRowLeftHeadline.Attributes.Add("IsTitle", "True");

                    for (int i = 0; i < tableCellLeftBase.RowSpan - 1; i++)
                    {
                        tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                        tableRowLeftHeadline = new TableRow();
                        tableRowLeftHeadline.Attributes.Add("IsTitle", "True");
                    }
                }

                //tableCellLeftBase.ColumnSpan += (maxLeftNestedLevel - leftVariable.NestedLevels) * 2;

                if (leftVariable.VariableType == VariableType.Numeric)
                {
                    string[] categories = { "Mean", "Min", "Max", "StdDev" };

                    foreach (string category in categories)
                    {
                        // Create a new table cell for the category's label.
                        TableCell tableCellCategory = new TableCell();
                        tableCellCategory.CssClass = "TableCellHeadline TableCellHeadlineLeftCategory BackgroundColor9 ";
                        tableCellCategory.Text = base.LanguageManager.GetText(category);
                        //tableCellCategory.ColumnSpan = 1;
                        tableCellCategory.ColumnSpan = (maxLeftNestedLevel - leftVariable.NestedLevelsBase) + 1;

                        tableCellCategory.Attributes.Add(
                            "IdCategory",
                            (new Guid()).ToString()
                        );

                        if (leftVariable.NestedVariables.Count == 0)
                        {
                            tableCellCategory.Attributes.Add(
                                "IsTitle",
                                "True"
                            );
                        }

                        tableCellCategory.RowSpan = leftVariable.RowSpanBase;

                        tableCellCategory.Style.Add(
                            "width",
                            CalculateCellWidth(leftVariable) + "px"
                        );

                        if (leftVariable.NestedVariables.Count == 0)
                            ApplyCellHeight(tableCellCategory);

                        tableRowLeftHeadline.Cells.Add(tableCellCategory);

                        tableCellSpacer = new TableCell() { CssClass = "TableCellValue TableCellLeftHeadlineSpacer", Text = "&nbsp;" };
                        ApplyCellHeight(tableCellSpacer);
                        tableRowLeftHeadline.Cells.Add(tableCellSpacer);

                        if (this.IsExport)
                            tableCellSpacer.Attributes.Add("ExportWidth", "0");

                        tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                        tableRowLeftHeadline = new TableRow();
                        tableRowLeftHeadline.Attributes.Add("IsTitle", "True");

                        for (int i = 0; i < tableCellCategory.RowSpan - 1; i++)
                        {
                            tableCellSpacer = new TableCell() { CssClass = "TableCellValue TableCellLeftHeadlineSpacer", Text = "&nbsp;" };
                            ApplyCellHeight(tableCellSpacer);
                            tableRowLeftHeadline.Cells.Add(tableCellSpacer);

                            if (this.IsExport)
                                tableCellSpacer.Attributes.Add("ExportWidth", "0");

                            tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                            tableRowLeftHeadline = new TableRow();
                            tableRowLeftHeadline.Attributes.Add("IsTitle", "True");
                        }

                        //tableCellCategory.ColumnSpan += (maxLeftNestedLevel - leftVariable.NestedLevels) * 2;
                    }
                }
                else
                {
                    // Run through all scores of the left variable.
                    foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                    {
                        // Check if the score is hidden.
                        if (leftScore.Hidden || (leftScore.HasValues == false && this.ReportDefinition.Settings.HideEmptyRowsAndColumns))
                            continue;

                        // Check if the score is available in the selected hierarchy.
                        if (leftVariable.IsTaxonomy &&
                            leftScore.Persistent &&
                            this.ReportDefinition.HierarchyFilter.
                            TaxonomyCategories.ContainsKey(leftScore.Identity) == false)
                            continue;

                        // Create a new table cell for the category's label.
                        TableCell tableCellCategory = new TableCell();
                        tableCellCategory.CssClass = "TableCellHeadline TableCellHeadlineLeftCategory BackgroundColor9 ";
                        tableCellCategory.RowSpan = leftScore.RowSpan;
                        //tableCellCategory.ColumnSpan = 1;
                        tableCellCategory.ColumnSpan = (maxLeftNestedLevel - leftVariable.NestedLevelsBase) + 1;

                        if (isMeanRow)
                            tableCellCategory.RowSpan = leftScore.RowSpanMean;

                        if (leftVariable.IsFake)
                        {
                            tableCellCategory.RowSpan = 1;
                        }

                        tableCellCategory.Attributes.Add("oncontextmenu", string.Format(
                            "ShowScoreOptions2({1}, '{2}', '{0}'); return false;",
                            leftScore.XmlNode.GetXPath(true),
                            leftScore.Equation == null ? "false" : "true",
                            this.ReportDefinition.FileName.Replace("\\", "/")
                        ));

                        tableCellCategory.Attributes.Add(
                            "onmouseout",
                            "this.style.background='';this.style.cursor='';this.style.color='';this.onclick = undefined;"
                        );

                        tableCellCategory.Attributes.Add(
                            "Position",
                            "Left"
                        );

                        tableCellCategory.Attributes.Add(
                            "NestLevel",
                            leftScore.Variable.NestedLevels.ToString()
                        );

                        tableCellCategory.Attributes.Add(
                            "ScoreType",
                            leftScore.GetType().Name
                        );

                        tableCellCategory.Attributes.Add(
                            "XPath",
                            leftScore.XmlNode.GetXPath(true)
                        );

                        tableCellCategory.Attributes.Add(
                            "IdCategory",
                            leftScore.Identity.ToString()
                        );

                        if (!leftVariable.IsFake)
                        {
                            tableCellCategory.Attributes.Add(
                                "onmousedown",
                                "CreateDragCategory(this)"
                            );
                        }

                        /*tableCellCategory.Style.Add(
                            "width",
                            CalculateCellWidth(leftScore.Variable) + "px"
                        );*/

                        if (leftScore.Variable.NestedVariables.Count == 0)
                        {
                            tableCellCategory.Attributes.Add(
                                "IsTitle",
                                "True"
                            );

                            tableCellCategory.Attributes.Add(
                                "LeftSignificanceIdentity",
                                sigDiffIdentity + leftScore.Identity
                            );

                            tableRowValues.Attributes.Add(
                                "IdVariable",
                                leftScore.Variable.IdVariable.ToString()
                            );

                            tableRowSigDiff.Attributes.Add(
                                "IdVariable",
                                leftScore.Variable.IdVariable.ToString()
                            );

                            tableRowPercentage.Attributes.Add(
                                "IdVariable",
                                leftScore.Variable.IdVariable.ToString()
                            );
                        }

                        if (tableCellCategory.RowSpan == 0)
                            tableCellCategory.RowSpan = 1;

                        /*if (leftVariable.NestedVariables.Count == 0)
                            ApplyCellHeight(tableCellCategory);*/

                        tableCellCategory.Text = string.Format(
                            "<table style=\"width:100%;\" {3} NestLevel=\"{2}.5\"><tr><td><div style=\"max-height:{0}px;overflow:hidden;width:56px;\">{1}</div></td></tr></table>",
                            (cellHeight * tableCellCategory.RowSpan) - 5,
                            leftScore.Label,
                            leftVariable.NestedLevels,
                            leftVariable.NestedVariables.Count != 0 ? "ScrollLabel=\"true\"" : ""
                        );

                        //string tooltip = leftScore.Label;
                        string tooltip = "this.textContent";

                        if (leftScore.XmlNode.Name == "ScoreGroup")
                        {

                            ReportDefinitionScoreGroup scoreGroup = (ReportDefinitionScoreGroup)leftScore;


                            if (scoreGroup.Equation != null)
                            {
                                tooltip = "'" + scoreGroup.Equation + "'";
                            }
                            else
                            {
                                tooltip = "'<b>' + this.textContent + '</b><br />";

                                foreach (ReportDefinitionScore score in scoreGroup.Scores)
                                {
                                    if (!tooltip.Contains("&nbsp;-&nbsp;" + score.Label.Replace("'", "") + "<br />"))
                                        tooltip += "&nbsp;-&nbsp;" + score.Label.Replace("'", "") + "<br />";
                                }

                                tooltip += "'";
                            }
                        }

                        tableCellCategory.Attributes.Add("onmouseover", string.Format(
                            "ShowToolTip(this, {0}, undefined, 'Left');CombineScores('" + leftScore.Identity + "', '" + leftScore.GetType().Name + "', '" + leftScore.XmlNode.GetXPath() + "');",
                            tooltip
                        ));

                        tableRowLeftHeadline.Cells.Add(tableCellCategory);

                        //tableCellCategory.ColumnSpan += (maxLeftNestedLevel - leftVariable.NestedLevels) * 2;

                        // Run through all nested variables of the left variable.
                        foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
                        {
                            RenderLeftVariableLabels(
                                nestedLeftVariable,
                                sigDiffIdentity + leftScore.Identity
                            );
                        }

                        if (leftVariable.NestedVariables.Count == 0)
                        {
                            tableCellSpacer = new TableCell() { CssClass = "TableCellValue TableCellLeftHeadlineSpacer", Text = "&nbsp;" };
                            ApplyCellHeight(tableCellSpacer);
                            tableRowLeftHeadline.Cells.Add(tableCellSpacer);

                            if (this.IsExport)
                                tableCellSpacer.Attributes.Add("ExportWidth", "0");

                            tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                            tableRowLeftHeadline = new TableRow();
                            tableRowLeftHeadline.Attributes.Add("IsTitle", "True");

                            for (int i = 0; i < tableCellCategory.RowSpan - 1; i++)
                            {
                                tableCellSpacer = new TableCell() { CssClass = "TableCellValue TableCellLeftHeadlineSpacer", Text = "&nbsp;" };
                                ApplyCellHeight(tableCellSpacer);
                                tableRowLeftHeadline.Cells.Add(tableCellSpacer);

                                if (this.IsExport)
                                    tableCellSpacer.Attributes.Add("ExportWidth", "0");

                                tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                                tableRowLeftHeadline = new TableRow();
                                tableRowLeftHeadline.Attributes.Add("IsTitle", "True");
                            }
                        }


                    }

                    if (leftVariable.Scale)
                    {
                        // Create a new table cell for the category's label.
                        TableCell tableCellCategory = new TableCell();
                        tableCellCategory.CssClass = "TableCellHeadline TableCellHeadlineLeftCategory BackgroundColor9 ";
                        tableCellCategory.Text = base.LanguageManager.GetText("Mean");
                        tableCellCategory.ColumnSpan = 1;

                        //tableCellCategory.ColumnSpan += (maxLeftNestedLevel - leftVariable.NestedLevels) * 2;

                        tableCellCategory.Attributes.Add(
                            "IdCategory",
                            (new Guid()).ToString()
                        );

                        if (leftVariable.NestedVariables.Count == 0)
                        {
                            tableCellCategory.Attributes.Add(
                                "IsTitle",
                                "True"
                            );
                        }

                        tableCellCategory.RowSpan = leftVariable.RowSpanMeanScore;

                        tableCellCategory.Style.Add(
                            "width",
                            CalculateCellWidth(leftVariable) + "px"
                        );

                        tableRowLeftHeadline.Cells.Add(tableCellCategory);

                        // Run through all nested variables of the left variable.
                        foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
                        {
                            RenderLeftVariableLabels(
                                nestedLeftVariable,
                                "",
                                true
                            );
                        }

                        if (leftVariable.NestedVariables.Count == 0)
                        {
                            tableCellSpacer = new TableCell() { CssClass = "TableCellValue TableCellLeftHeadlineSpacer", Text = "&nbsp;" };
                            ApplyCellHeight(tableCellSpacer);
                            tableRowLeftHeadline.Cells.Add(tableCellSpacer);

                            if (this.IsExport)
                                tableCellSpacer.Attributes.Add("ExportWidth", "0");

                            tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                            tableRowLeftHeadline = new TableRow();
                            tableRowLeftHeadline.Attributes.Add("IsTitle", "True");

                            for (int i = 0; i < tableCellCategory.RowSpan - 1; i++)
                            {
                                tableCellSpacer = new TableCell() { CssClass = "TableCellValue TableCellLeftHeadlineSpacer", Text = "&nbsp;" };
                                ApplyCellHeight(tableCellSpacer);
                                tableRowLeftHeadline.Cells.Add(tableCellSpacer);

                                if (this.IsExport)
                                    tableCellSpacer.Attributes.Add("ExportWidth", "0");

                                tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                                tableRowLeftHeadline = new TableRow();
                                tableRowLeftHeadline.Attributes.Add("IsTitle", "True");
                            }
                        }
                    }
                }
            }
            else
            {
                tableLeftHeadline.Rows.Add(tableRowLeftHeadline);
                tableRowLeftHeadline = new TableRow();
                tableRowLeftHeadline.Attributes.Add("IsTitle", "True");
            }
        }


        Dictionary<int, int> nestedLeftVariableCounter = new Dictionary<int, int>();

        TableRow tableRowLeftHeadline = new TableRow();

        private Table RenderTopLeftButtons(int topHeadlineHeight)
        {
            Table table = new Table();
            table.CssClass = "Crosstable TableTopLeftButtons";
            table.Style.Add("height", (topHeadlineHeight - 1) + "px");

            TableRow tableRow1 = new TableRow();
            TableRow tableRow2 = new TableRow();

            TableCell tableCellSelectVariable = new TableCell();
            TableCell tableCellWeighting = new TableCell();
            TableCell tableCellFilter = new TableCell();
            TableCell tableCellSwitch = new TableCell();

            tableCellWeighting.Attributes.Add("DropArea", "True");
            tableCellWeighting.Attributes.Add("DropAreaMessage", "DropAreaMessageWeight");
            tableCellWeighting.Attributes.Add("Source", this.ReportDefinition.FileName.Replace("\\", "/"));
            tableCellWeighting.Attributes.Add("Path", "WEIGHT");

            tableCellSelectVariable.Attributes.Add("onmouseover", string.Format(
                "ShowToolTip(this, '{0}', false, 'Bottom');",
                base.LanguageManager.GetText("ToolTipSelectVariable")
            ));

            tableCellWeighting.Attributes.Add("onmouseover", string.Format(
                "ShowToolTip(this, '{0}', false, 'Bottom');",
                base.LanguageManager.GetText("ToolTipWeighting")
            ));

            tableCellSwitch.Attributes.Add("onmouseover", string.Format(
                "ShowToolTip(this, '{0}', false, 'Bottom');",
                base.LanguageManager.GetText("ToolTipSwitch")
            ));

            tableCellFilter.ID = "pnlFilter";

            //tableCellFilter.Attributes.Add("onmouseover", string.Format(
            //    "ShowToolTip(this, '{0}', false, 'Bottom');OverFilter();",
            //    base.LanguageManager.GetText("ToolTipFilter")
            //));

            tableCellFilter.Attributes.Add("onmouseover", string.Format(
                // "ShowToolTip(this, '{0}', false, 'Bottom');OverFilter();",
                "OverFilter();ShowFiltersToolTip(this, '{0}', false, 'Bottom');",
              base.LanguageManager.GetText("ToolTipFilter")
          ));

            tableCellFilter.Attributes.Add("onmouseout", string.Format(
                 "HideFiltersToolTip()"
                 ));

            tableCellSelectVariable.CssClass = "BackgroundColor7H1";
            tableCellWeighting.CssClass = "BackgroundColor7H1";
            tableCellFilter.CssClass = "BackgroundColor7H1";
            tableCellSwitch.CssClass = "BackgroundColor7H1";

            tableCellSelectVariable.Style.Add("background-image", "url('/Images/Icons/SelectVariable.png')");
            tableCellWeighting.Style.Add("background-image", "url('/Images/Icons/Weighting.png')");
            tableCellFilter.Style.Add("background-image", "url('/Images/Icons/Filter.png')");
            tableCellSwitch.Style.Add("background-image", "url('/Images/Icons/Switch.png')");

            if (this.ReportDefinition.FilterCategories.Count > 0 && this.ReportDefinition.FilterCategories[0].FiltersApplied)
            {
                tableCellFilter.Style["background-image"] = "url('/Images/Icons/Filter_Active.png')";
                tableCellFilter.CssClass = "GreenBackground3";
            }

            if (this.ReportDefinition.WeightingFilters.DefaultWeighting.HasValue || this.ReportDefinition.WeightingFilters.Length > 0)
            {
                tableCellWeighting.Style["background-image"] = "url('/Images/Icons/Weighting_Active.png')";
                tableCellWeighting.CssClass = "GreenBackground3";
            }

            tableCellSelectVariable.Style.Add("background-size", "50px");
            tableCellWeighting.Style.Add("background-size", "50px");
            tableCellFilter.Style.Add("background-size", "50px");
            tableCellSwitch.Style.Add("background-size", "50px");

            tableCellSelectVariable.Attributes.Add("onclick", "document.getElementById('variableSearchResults').style.height=(window.innerHeight - 300) + 'px';InitDragBox('boxVariableSearchControl');window.setTimeout(function() { SearchVariables('" + this.ReportDefinition.FileName.Replace("\\", "/") + "', '" + this.ReportDefinition.Settings.IdLanguage + "', " + this.ReportDefinition.Settings.DataCheckEnabled.ToString().ToLower() + "); }, 500);document.getElementById('cphContent_VariableSearch_txtVariableSearch').focus();");
            tableCellWeighting.Attributes.Add("onclick", "ShowDefaultWeightingSelector(this);");
            tableCellFilter.Attributes.Add("onclick", "InitDragBox('boxFilterDefinitionControl');");
            tableCellSwitch.Attributes.Add("onclick", "SwitchTable();");

            tableRow1.Cells.Add(tableCellSelectVariable);
            tableRow1.Cells.Add(tableCellWeighting);

            tableRow2.Cells.Add(tableCellFilter);
            tableRow2.Cells.Add(tableCellSwitch);

            table.Rows.Add(tableRow1);
            table.Rows.Add(tableRow2);

            return table;
        }

        private void InitNewRow(
            Table table,
            bool forceSigDiffRow = false,
            bool forcePercRow = false,
            bool excludeSigDiffRow = false
        )
        {
            if (tableRowValues.Cells.Count == 0 && forceSigDiffRow == false)
                return;

            if (tableRowValues.Cells.Count > 0)
                table.Rows.Add(tableRowValues);

            if (tableRowPercentage.Cells.Count > 0 || forcePercRow)
                table.Rows.Add(tableRowPercentage);

            if (forceSigDiffRow)
                table.Rows.Add(tableRowSigDiff);

            if (this.ReportDefinition.SignificanceTest && forceSigDiffRow == false)
            {
                if (!excludeSigDiffRow)
                    table.Rows.Add(tableRowSigDiff);
            }

            tableRowValues = new TableRow();
            tableRowSigDiff = new TableRow();
            tableRowPercentage = new TableRow();
        }


        private int GetTotalCategoryCount(ReportDefinitionVariable variable, bool timesRowSpan = false)
        {
            int result = 0;

            int baseValueCells = 1;

            //if (this.ReportDefinition.Settings.DisplayUnweightedBase)
            //    baseValueCells = 2;

            //if (this.ReportDefinition.Settings.DisplayEffectiveBase)
            //    baseValueCells = 2;

            if (variable.NestedVariables.Count == 0)
            {
                int count = 0;

                if (variable.VariableType == VariableType.Numeric)
                    count += 4;
                else
                {
                    count += variable.ScoresCount;

                    if (timesRowSpan)
                    {
                        int rows = 0;
                        if (this.ReportDefinition.SignificanceTest)
                            rows += 1;

                        if (this.ReportDefinition.Settings.ShowValues)
                            rows += 1;

                        if (this.ReportDefinition.Settings.ShowPercentage)
                            rows += 1;

                        count *= rows;
                    }

                    if (timesRowSpan && variable.Scale)
                    {
                        if (this.ReportDefinition.Settings.SignificanceTest && variable.ParentVariable == null)
                            count += 2;
                        else
                            count += 1;
                    }
                }

                count += baseValueCells;

                result += count;
            }
            else
            {
                int count = variable.ScoresCount + 1;

                /*if (timesRowSpan && variable.Scale && this.ReportDefinition.SignificanceTest)
                    count++;*/

                for (int i = 0; i < count; i++)
                {
                    // Run through all nested variables.
                    foreach (ReportDefinitionVariable nestedVariable in variable.NestedVariables)
                    {
                        result += GetTotalCategoryCount(nestedVariable, timesRowSpan);
                    }
                }

                if (timesRowSpan && variable.Scale)
                {
                    // Run through all nested variables.
                    foreach (ReportDefinitionVariable nestedVariable in variable.NestedVariables)
                    {
                        result += GetTotalCategoryCount(nestedVariable, false);
                    }
                }
            }

            return result;
        }

        private double CalculateCellHeight(ReportDefinitionVariable topVariable)
        {
            ReportDefinitionVariable rootVariable = topVariable;

            while (rootVariable.ParentVariable != null)
            {
                rootVariable = rootVariable.ParentVariable;
            }

            int maxNestedLevels = 0;

            // Run through all top variables at root level.
            foreach (ReportDefinitionVariable variable in this.ReportDefinition.TopVariables)
            {
                // Get the top variable's nesting level.
                int nestingLevel = variable.NestedLevels;

                // Check if the top variable's nesting levels is higher
                // than the maxNestedLevels variable's value.
                if (nestingLevel > maxNestedLevels)
                    maxNestedLevels = nestingLevel;
            }

            int difference = maxNestedLevels / rootVariable.NestedLevels;

            double factor = 50.0;

            if (topVariable.VariableType == VariableType.Numeric && this.ReportDefinition.SignificanceTest)
                factor = 79.0;

            double result = difference * factor;

            //result += 66 * (difference - 1);
            result += 82 * (difference - 1);

            return result;
        }

        private double CalculateCellWidth(ReportDefinitionVariable leftVariable, bool category = true)
        {
            ReportDefinitionVariable rootVariable = leftVariable;

            while (rootVariable.ParentVariable != null)
            {
                rootVariable = rootVariable.ParentVariable;
            }

            int maxNestedLevels = 0;

            // Run through all left variables at root level.
            foreach (ReportDefinitionVariable variable in this.ReportDefinition.LeftVariables)
            {
                // Get the top variable's nesting level.
                int nestingLevel = variable.NestedLevels;

                // Check if the top variable's nesting levels is higher
                // than the maxNestedLevels variable's value.
                if (nestingLevel > maxNestedLevels)
                    maxNestedLevels = nestingLevel;
            }

            int difference = maxNestedLevels;

            if (category)
            {
                difference = maxNestedLevels / rootVariable.NestedLevels;
            }

            double result = difference * 100;

            result += 100 * (difference - 1);

            return result;
        }

        Panel pnlFilter;
        TableRow tableRowTopLeft;


        int maxTopNestedLevel;
        int maxLeftNestedLevel;
        private void BuildCrosstable()
        {
            HtmlGenericControl style = new HtmlGenericControl("style");
            style.Attributes.Add("type", "text/css");
            style.InnerHtml = ".Content { overflow: hidden; }";

            base.Controls.Add(style);

            int totalTopCategoryCount = 0;
            int totalLeftCategoryCount = 0;

            maxTopNestedLevel = 0;
            maxLeftNestedLevel = 0;

            foreach (ReportDefinitionVariable topVariable in this.ReportDefinition.TopVariables)
            {
                totalTopCategoryCount += GetTotalCategoryCount(topVariable);

                if (topVariable.NestedLevels > maxTopNestedLevel)
                    maxTopNestedLevel = topVariable.NestedLevels;
            }

            foreach (ReportDefinitionVariable leftVariable in this.ReportDefinition.LeftVariables)
            {
                totalLeftCategoryCount += GetTotalCategoryCount(leftVariable, true);

                if (leftVariable.NestedLevels > maxLeftNestedLevel)
                    maxLeftNestedLevel = leftVariable.NestedLevels;
            }

            //cellWidth = this.ReportDefinition.Settings.TableWidth;
            //cellHeight = this.ReportDefinition.Settings.TableHeight;
            cellWidth = this.ContentWidth - 40;
            cellHeight = this.ContentHeight - 175;

            if (bool.Parse(((UserDefaults)HttpContext.Current.Session["UserDefaults"])["BottomBarPinned", "false"]))
            {
                cellHeight -= 300;
            }

            int topHeadlineHeight = (103 + ((maxTopNestedLevel - 1) * 101) + (this.ReportDefinition.SignificanceTest ? 20 : 0));
            int leftHeadlineWidth = maxLeftNestedLevel * 120;

            // Left headline.
            cellWidth -= leftHeadlineWidth;

            // Top headline.
            //cellHeight -= (maxTopNestedLevel - 1) * (this.ReportDefinition.SignificanceTest ? 123 : 102);
            cellHeight -= topHeadlineHeight;

            int scrollBarThickness = 8;

            if (HttpContext.Current != null &&
                (HttpContext.Current.Request.Browser.Browser.Contains("InternetExplorer") ||
                HttpContext.Current.Request.Browser.Browser.Contains("IE")))
            {
                scrollBarThickness = 16;
            }

            // Scroll bar on right hand side.
            cellWidth -= scrollBarThickness;

            int maxWidth = cellWidth + 5;
            int maxHeight = cellHeight + scrollBarThickness - 65;

            // Border on the right
            cellWidth -= 10;
            cellHeight -= 5;

            // Devide the cell width by the ammount of columns to
            // get the width for each individual column.
            if (totalTopCategoryCount != 0)
                cellWidth /= totalTopCategoryCount;

            // Devide the cell height by the ammount of rows to
            // get the height for each individual row.
            if (totalLeftCategoryCount != 0)
                cellHeight /= totalLeftCategoryCount;

            // Check if the calculated cell width is smaller than
            // the maximum width. If yes a scroll bar appears.
            if (cellWidth < this.ReportDefinition.Settings.MinWidth)
                cellWidth = this.ReportDefinition.Settings.MinWidth;

            // Check if the calculated cell height is smaller than
            // the maximum height. If yes a scroll bar appears.
            if (cellHeight < this.ReportDefinition.Settings.MinHeight)
                cellHeight = this.ReportDefinition.Settings.MinHeight;

            leftVariableCount = 0;

            tableData = new Table();
            tableData.CssClass = "Crosstable";

            tableTopHeadline = new Table();
            tableTopHeadline.CssClass = "Crosstable TopHeadline";

            tableLeftHeadline = new Table();
            tableLeftHeadline.CssClass = "Crosstable BackgroundColor7 LeftHeadline";

            tableRowValues = new TableRow();
            tableRowSigDiff = new TableRow();
            tableRowPercentage = new TableRow();

            if (this.ReportDefinition.TopVariables.Count != 0 && this.ReportDefinition.TopVariables[0].IsFake)
            {
                cellWidth = 100;
            }

            if (this.ReportDefinition.LeftVariables.Count != 0 && this.ReportDefinition.LeftVariables[0].IsFake)
            {
                cellHeight = 100;
            }

            // Render the top variable's labels.
            RenderTopVariableLabels(
                this.ReportDefinition.TopVariables,
                0
            );

            if (this.ReportDefinition.SignificanceTest)
            {
                tableRowSignficanceHeadline = new TableRow();

                // Render the significance letters headline.
                RenderTopSignificanceHeadline(this.ReportDefinition.TopVariables);

                tableTopHeadline.Rows.Add(tableRowSignficanceHeadline);
            }

            tableRowLeftHeadline.Attributes.Add("IsTitle", "True");

            // Run through all left variables.
            foreach (ReportDefinitionVariable leftVariable in this.ReportDefinition.LeftVariables)
            {
                // Render the left variable's labels.
                RenderLeftVariableLabels(
                    leftVariable,
                    ""
                );
            }
            /*
            // Run through all left variables on root level.
            foreach (ReportDefinitionVariable leftVariable in this.ReportDefinition.LeftVariables)
            {
                tableRowValues = new TableRow();
                tableRowSigDiff = new TableRow();
                tableRowPercentage = new TableRow();

                RenderLeftVariable(
                    leftVariable,
                    "Results/",
                    "Results/"
                );

                leftVariableCount++;
            }*/

            Table tableContainer = new Table();
            tableContainer.Attributes.Add("id", "CrosstableContainer");
            tableContainer.CssClass = "CrosstableContainer";

            TableRow tableRowTop = new TableRow();
            tableRowTop.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;

            TableCell tableCellLeftTop = new TableCell();
            TableCell tableCellTopHeadline = new TableCell();

            if (maxTopNestedLevel == 0)
                topHeadlineHeight = 110;

            if (!this.IsExport)
            {
                tableCellLeftTop.CssClass = "BackgroundColor7";

                //tableCellLeftTop.Style.Add("border-right","1px solid #8B8888");
                //tableCellLeftTop.Style.Add("border-bottom", " 1px solid #8B8888");
                tableCellLeftTop.Controls.Add(RenderTopLeftButtons(topHeadlineHeight));
            }

            Panel pnlTopHeadline = new Panel();
            pnlTopHeadline.Attributes.Add("id", "pnlTopHeadline");
            pnlTopHeadline.CssClass = "PanelTopHeadlineScroll";
            pnlTopHeadline.Style.Add("max-width", (maxWidth - scrollBarThickness) + "px");

            if (this.ReportDefinition.TopVariables.Count > 0)
                pnlTopHeadline.Style.Add("width", (maxWidth - scrollBarThickness) + "px");
            else
                pnlTopHeadline.Style.Add("width", "110px");

            tableTopHeadline.Style.Add("height", topHeadlineHeight + "px");

            pnlTopHeadline.Controls.Add(tableTopHeadline);
            tableCellTopHeadline.Controls.Add(pnlTopHeadline);

            if (!this.IsExport)
            {
                tableRowTop.Cells.Add(tableCellLeftTop);
            }
            else
            {
                TableCell exportSpacer = new TableCell();
                exportSpacer.CssClass = "BackgroundColor7";

                exportSpacer.RowSpan = 4;
                exportSpacer.ColumnSpan = (maxLeftNestedLevel * 2) + 1;
                exportSpacer.RowSpan = (maxTopNestedLevel * 2);

                if (this.ReportDefinition.SignificanceTest)
                    exportSpacer.RowSpan += 1;

                if (this.ReportDefinition.LeftVariables.Count == 1 && this.ReportDefinition.LeftVariables[0].IsFake)
                {
                    exportSpacer.RowSpan++;

                    if (this.ReportDefinition.Settings.DisplayUnweightedBase)
                        exportSpacer.RowSpan++;

                    if (this.ReportDefinition.Settings.DisplayEffectiveBase)
                        exportSpacer.RowSpan++;

                    //tableLeftHeadline.Rows[0].Cells.RemoveAt(tableLeftHeadline.Rows[0].Cells.Count - 1);
                    tableLeftHeadline.Rows.Clear();
                }

                tableTopHeadline.Rows[0].Cells.AddAt(0, exportSpacer);
            }

            tableRowTop.Cells.Add(tableCellTopHeadline);

            tableContainer.Rows.Add(tableRowTop);

            TableRow tableRowData = new TableRow();
            tableRowData.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;

            TableCell tableCellLeftHeadline = new TableCell();
            TableCell tableCellData = new TableCell();

            Panel pnlLeftHeadline = new Panel();
            pnlLeftHeadline.Attributes.Add("id", "pnlLeftHeadline");
            pnlLeftHeadline.CssClass = "PanelLeftHeadlineScroll";
            pnlLeftHeadline.Style.Add("max-height", (maxHeight - scrollBarThickness) + "px");

            if (this.ReportDefinition.LeftVariables.Count > 0)
                pnlLeftHeadline.Style.Add("height", (maxHeight - scrollBarThickness) + "px");
            else
                pnlLeftHeadline.Style.Add("height", "110px");

            if (leftHeadlineWidth == 0)
                leftHeadlineWidth = 110;

            pnlLeftHeadline.Style.Add("width", leftHeadlineWidth + "px");
            pnlLeftHeadline.Style.Add("max-width", leftHeadlineWidth + "px");
            tableCellLeftHeadline.Style.Add("width", leftHeadlineWidth + "px");
            tableCellLeftHeadline.Style.Add("max-width", leftHeadlineWidth + "px");
            tableCellLeftTop.Style.Add("width", leftHeadlineWidth + "px");
            tableCellLeftTop.Style.Add("max-width", leftHeadlineWidth + "px");

            pnlLeftHeadline.Controls.Add(tableLeftHeadline);
            tableCellLeftHeadline.Controls.Add(pnlLeftHeadline);

            Panel pnlData = new Panel();
            pnlData.Attributes.Add("id", "pnlData");
            pnlData.CssClass = "PanelTableScroll";
            pnlData.Style.Add("max-height", maxHeight + "px");
            pnlData.Style.Add("height", maxHeight + "px");
            pnlData.Style.Add("max-width", maxWidth + "px");
            pnlData.Style.Add("width", maxWidth + "px");

            pnlData.Attributes.Add("onscroll", "TableScroll(this);");

            //pnlData.Controls.Add(tableData);
            ReportDefinitionRenderer renderer = new ReportDefinitionRenderer(this.ReportDefinition, this.IsExport);
            HtmlGenericControl test = new HtmlGenericControl("div");
            test.InnerHtml = renderer.Render(cellWidth, cellHeight);

            if (this.IsExport)
            {
                //test.InnerHtml = test.InnerHtml.Replace("<tr></tr>", "<tr><td></td></tr>");
                //test.InnerHtml = test.InnerHtml.Replace("<tr></tr>", "");
                if (this.ReportDefinition.LeftVariables.Count > 0 &&
                    (this.ReportDefinition.TopVariables.Count == 0 || this.ReportDefinition.TopVariables[0].IsFake))
                {
                    test.InnerHtml = test.InnerHtml.Replace("<tr></tr>", "");
                }
                else if (this.ReportDefinition.TopVariables.Count > 0 && this.ReportDefinition.LeftVariables.Count > 0 &&
                this.ReportDefinition.TopVariables[0].VariableType == VariableType.Numeric && this.ReportDefinition.LeftVariables[0].VariableType == VariableType.Numeric)
                {
                    test.InnerHtml = test.InnerHtml.Replace("<tr></tr>", "");
                }
                else if (this.ReportDefinition.TopVariables.Count > 0 &&
                    this.ReportDefinition.TopVariables[0].VariableType == VariableType.Numeric)
                {
                    // I hate to do this, but we need a quick fix right now...
                    int lineCount = 0;

                    if (this.ReportDefinition.Settings.SignificanceTest)
                        lineCount++;
                    if (this.ReportDefinition.Settings.ShowValues)
                        lineCount++;
                    if (this.ReportDefinition.Settings.ShowPercentage)
                        lineCount++;

                    if (lineCount == 2)
                    {
                        test.InnerHtml = test.InnerHtml.Replace("<tr></tr><tr></tr>", "<tr><td></td></tr>");
                    }
                    else if (lineCount == 1)
                    {
                        test.InnerHtml = test.InnerHtml.Replace("<tr></tr>", "");
                    }
                    else if (lineCount == 3)
                    {
                        test.InnerHtml = test.InnerHtml.Replace("<tr></tr>", "<tr><td></td></tr>");
                    }
                }
            }

            pnlData.Controls.Add(test);
            //pnlData.Controls.Add(new LiteralControl(renderer.Render(cellWidth, cellHeight)));

            tableCellData.Controls.Add(pnlData);

            tableRowData.Cells.Add(tableCellLeftHeadline);
            tableRowData.Cells.Add(tableCellData);

            tableContainer.Rows.Add(tableRowData);

            RenderNewVariableSelectors(
                tableContainer,
                maxWidth,
                maxHeight,
                topHeadlineHeight
            );

            base.Controls.Add(tableContainer);

            this.Table = tableContainer;

            TableRow tableRowDescription = new TableRow();

            tableRowDescription.Cells.Add(
                new TableCell()
            );

            if (pnlFilter != null)
            {
                pnlFilter.Attributes.Add(
                    "onclick",
                    //string.Format("__doPostBack('{0}', '');", this.FilterClickAction)
                    "document.getElementById(\"pnlFilterBox\").style.visibility=\"\";UpdateFilterView('" + this.ReportDefinition.FileName.Replace("\\", "/") + "');"
                );
            }
        }

        private void RenderNewVariableSelectors(
            Table tableContainer,
            int maxWidth,
            int maxHeight,
            int headlineHeight
        )
        {
            TableRow tableRowNewLeftVariable = new TableRow();

            TableCell tableCellNewLeftVariable = new TableCell();
            tableCellNewLeftVariable.CssClass = "Color1";
            tableCellNewLeftVariable.Style.Add("padding", "0px");
            tableCellNewLeftVariable.ColumnSpan = 2;

            tableCellNewLeftVariable.Attributes.Add("DropArea", "True");
            tableCellNewLeftVariable.Attributes.Add("Source", this.ReportDefinition.FileName.Replace("\\", "/"));
            tableCellNewLeftVariable.Attributes.Add("Path", "Report/Variables[@Position=\"Left\"]");
            tableCellNewLeftVariable.Attributes.Add("DropAreaMessage", "DropAreaMessageAdd");

            tableRowNewLeftVariable.Cells.Add(tableCellNewLeftVariable);

            TableCell tableCellNewTopVariable = new TableCell();
            tableCellNewTopVariable.CssClass = "TableCellNewTopVariable Color1";
            tableCellNewTopVariable.Style.Add("padding", "0px");
            tableCellNewTopVariable.RowSpan = 2;

            tableCellNewTopVariable.Attributes.Add("DropArea", "True");
            tableCellNewTopVariable.Attributes.Add("Source", this.ReportDefinition.FileName.Replace("\\", "/"));
            tableCellNewTopVariable.Attributes.Add("Path", "Report/Variables[@Position=\"Top\"]");
            tableCellNewTopVariable.Attributes.Add("DropAreaMessage", "DropAreaMessageAdd");


            if (this.ReportDefinition.LeftVariables.Count == 0 &&
                this.ReportDefinition.TopVariables.Count == 0)
            {
                tableCellNewTopVariable.CssClass = "BackgroundColor7";

                tableData.Style.Add("height", "100%");
                tableData.Rows.Add(new System.Web.UI.WebControls.TableRow());

                TableCell tableCellEmpty = new TableCell();
                tableCellEmpty.CssClass = "";

                //tableCellEmpty.Style.Add("width", maxWidth + "px");
                tableCellEmpty.Style.Add("width", 110 + "px");
                tableCellEmpty.Style.Add("height", maxHeight + "px");

                tableData.Rows[0].Cells.Add(tableCellEmpty);

                //tableCellNewTopVariable.Style.Add("width", maxWidth + "px");
                tableCellNewTopVariable.Style.Add("width", 110 + "px");
                tableCellNewLeftVariable.Style.Add("height", maxHeight + "px");
            }
            else
            {
                tableCellNewTopVariable.Style.Add("width", "20px");
                tableCellNewLeftVariable.Style.Add("height", "20px");
            }

            if (!this.IsExport)
            {
                if (this.ReportDefinition.TopVariables.Count == 0 &&
                    this.ReportDefinition.LeftVariables.Count == 0)
                {
                    tableCellNewTopVariable.RowSpan = 4;

                    tableTopHeadline.Rows[0].Cells.Add(tableCellNewTopVariable);
                    tableLeftHeadline.Rows.Add(tableRowNewLeftVariable);
                }
                else
                {
                    tableContainer.Rows[0].Cells.Add(tableCellNewTopVariable);
                    tableContainer.Rows.Add(tableRowNewLeftVariable);
                }
            }
        }

        private void BuildCharts()
        {
            if (this.ReportDefinition.LeftVariables.Count == 1 && this.ReportDefinition.LeftVariables[0].VariableType == VariableType.Text)
            {
                // Run through all left variables.
                foreach (ReportDefinitionVariable leftVariable in this.ReportDefinition.LeftVariables)
                {
                    Charts.Chart chart = new Charts.Chart();
                    chart.DisplayType = this.ReportDefinition.Settings.DisplayType;
                    chart.Source = this.ReportDefinition.FileName;
                    chart.Title = leftVariable.Label;
                    chart.IdVariable = leftVariable.IdVariable;
                    chart.DecimalPlaces = this.ReportDefinition.Settings.DecimalPlaces;

                    chart.Render();

                    this.Style.Add("height", "100%");
                    this.Style.Add("width", "100%");
                    base.Controls.Add(chart);
                }
            }
            else
            {
                Charts.Chart chart = new Charts.Chart();
                chart.DisplayType = this.ReportDefinition.Settings.DisplayType;
                chart.DecimalPlaces = this.ReportDefinition.Settings.DecimalPlaces;
                chart.Source = this.ReportDefinition.FileName;
                if (this.ReportDefinition.XmlDocument.DocumentElement.Attributes["Name"].Value.Contains("new report"))
                {
                    foreach (ReportDefinitionVariable leftVariable in this.ReportDefinition.LeftVariables)
                    {
                        if (leftVariable.NestedVariables.Count > 0)
                        {
                            foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
                            {
                                if (nestedLeftVariable.NestedVariables.Count > 0)
                                {
                                    foreach (ReportDefinitionVariable nestedLeftVariable2 in nestedLeftVariable.NestedVariables)
                                    {
                                        chart.Title = string.Join(" / ", this.ReportDefinition.TopVariables.Select(x => x.Label)) + " - " + nestedLeftVariable.Label + " - " + nestedLeftVariable2.Label;
                                    }
                                }
                                else
                                {
                                    chart.Title = string.Join(" / ", this.ReportDefinition.LeftVariables.Select(x => x.Label)) + " - " + nestedLeftVariable.Label;
                                }
                            }
                        }
                        else
                        {
                            chart.Title = string.Join(" / ", this.ReportDefinition.LeftVariables.Select(x => x.Label));
                        }
                    }
                }
                else
                {
                    foreach (ReportDefinitionVariable leftVariable in this.ReportDefinition.LeftVariables)
                    {
                        if (leftVariable.NestedVariables.Count > 0)
                        {
                            foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
                            {
                                if (nestedLeftVariable.NestedVariables.Count > 0)
                                {
                                    foreach (ReportDefinitionVariable nestedLeftVariable2 in nestedLeftVariable.NestedVariables)
                                    {
                                        chart.Title = string.Join(" / ", this.ReportDefinition.TopVariables.Select(x => x.Label)) + " - " + nestedLeftVariable.Label + " - " + nestedLeftVariable2.Label;
                                    }
                                }
                                else
                                {
                                    chart.Title = string.Join(" / ", this.ReportDefinition.LeftVariables.Select(x => x.Label)) + " - " + nestedLeftVariable.Label;
                                }
                            }
                        }
                        else
                        {
                            chart.Title = this.ReportDefinition.XmlDocument.DocumentElement.Attributes["Name"].Value + "-" + string.Join(" / ", this.ReportDefinition.LeftVariables.Select(x => x.Label)); ;
                        }
                    }
                }
                chart.Render();

                this.Style.Add("height", "100%");
                this.Style.Add("width", "100%");
                base.Controls.Add(chart);
            }
        }


        public void Render()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                XmlNode xmlNodeResults = this.ReportDefinition.XmlDocument.DocumentElement.SelectSingleNode("Results");

                if (xmlNodeResults != null)
                {
                    if (xmlNodeResults.Attributes["Version"] == null || xmlNodeResults.Attributes["Version"].Value != (string)HttpContext.Current.Session["Version"])
                    {
                        xmlNodeResults.ParentNode.RemoveChild(xmlNodeResults);
                    }
                    else if (xmlNodeResults.Attributes["CaseDataVersion"] == null || xmlNodeResults.Attributes["CaseDataVersion"].Value != this.Core.CaseDataVersion.ToString())
                    {
                        xmlNodeResults.ParentNode.RemoveChild(xmlNodeResults);
                    }
                }

                if (this.ReportDefinition.LeftVariables.Count == 0 &&
                    this.ReportDefinition.TopVariables.Count != 0)
                {
                    XmlNode xmlNode = this.ReportDefinition.XmlDocument.DocumentElement.SelectSingleNode("Variables[@Position=\"Left\"]");
                    xmlNode.InnerXml += "<Variable Id=\"00000000-0000-0000-0000-000000000000\" Scale=\"False\" IsFake=\"True\" Type=\"3\"><TaxonomyCategory Id=\"00000000-0000-0000-0000-000000000000\" IsFake=\"True\"></TaxonomyCategory></Variable>";

                    this.ReportDefinition.ClearData();

                    this.ReportDefinition.Save();
                    this.ReportDefinition.Parse();
                }
                else if (this.ReportDefinition.LeftVariables.Count != 0 &&
                   this.ReportDefinition.TopVariables.Count == 0)
                {
                    XmlNode xmlNode = this.ReportDefinition.XmlDocument.DocumentElement.SelectSingleNode("Variables[@Position=\"Top\"]");
                    xmlNode.InnerXml += "<Variable Id=\"00000000-0000-0000-0000-000000000000\" Scale=\"False\" IsFake=\"True\" Type=\"3\"><TaxonomyCategory Id=\"00000000-0000-0000-0000-000000000000\" IsFake=\"True\"></TaxonomyCategory></Variable>";

                    this.ReportDefinition.ClearData();

                    this.ReportDefinition.Save();
                    this.ReportDefinition.Parse();
                }

                this.ReportDefinition.ValidateChanges();

                stopwatch.Stop();
                InitCalculator();
                stopwatch.Start();

                switch (this.ReportDefinition.Settings.DisplayType)
                {
                    case DisplayType.Crosstable:
                        BuildCrosstable();

                        break;
                    default:
                        BuildCharts();
                        break;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["LastReportDefinitionError"] = ex;

                HttpContext.Current.Response.Redirect("/Pages/LinkReporter/ReportDefinitionError.aspx");
            }

            stopwatch.Stop();

            if (!this.IsExport)
            {
                this.ReportDefinition.DataRenderTime = stopwatch.Elapsed;
            }

            this.ReportDefinition.Save();
        }

        #endregion


        #region Event Handlers

        int cellWidth;
        int cellHeight = 0;

        Table tableTopHeadline;
        Table tableLeftHeadline;
        Table tableData;

        TableRow tableRowValues;
        TableRow tableRowSigDiff;
        TableRow tableRowPercentage;

        protected void Crosstable_Load(object sender, EventArgs e)
        {
            // Render the control.
            this.Render();
        }

        #endregion
    }
}
