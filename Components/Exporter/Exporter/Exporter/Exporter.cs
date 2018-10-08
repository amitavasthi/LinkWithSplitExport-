using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;
using System.IO;
using ApplicationUtilities.Classes;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using Crosstables.Classes;
using Crosstables.Classes.ReportDefinitionClasses;
using System.Xml;
using System.Drawing;



namespace LiNK_Excel_Export
{
    public class Exporter
    {
        #region Properties

        public LanguageManager LanguageManager
        {
            get
            {
                return (LanguageManager)HttpContext.Current.Session["LanguageManager"];
            }
        }

        /// <summary>
        /// Gets or sets the source table object to export.
        /// </summary>
        public Table Source { get; set; }

        public string Style { get; set; }

        /// <summary>
        /// Gets or sets the color scheme split by css class names.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> ColorScheme { get; set; }

        /// <summary>
        /// Gets or sets the report definition of the export.
        /// </summary>
        public ReportDefinition ReportDefinition { get; set; }

        public Dictionary<string, string> ScoreNameReferences { get; set; }
        public Dictionary<Guid, Dictionary<Guid, string>> ValueReferences { get; set; }

        #endregion


        #region Constructor

        public Exporter(Table table, string style, ReportDefinition reportDefinition)
        {
            this.Source = table;
            this.Style = style;
            this.ReportDefinition = reportDefinition;

            InitColorScheme();

            this.ScoreNameReferences = new Dictionary<string, string>();
            this.ValueReferences = new Dictionary<Guid, Dictionary<Guid, string>>();
        }

        #endregion


        #region Methods

        private void InitColorScheme()
        {
            this.ColorScheme = new Dictionary<string, Dictionary<string, string>>();

            this.ColorScheme.Add("BackgroundColor1", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor1"].Add("Background", "#6CAEE0");
            this.ColorScheme["BackgroundColor1"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor1"].Add("BorderColor", "#FFFFFF");

            this.ColorScheme.Add("BackgroundColor5", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor5"].Add("Background", "#98C6E9");
            //this.ColorScheme["BackgroundColor5"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor5"].Add("BorderColor", "#FFFFFF");

            this.ColorScheme.Add("BackgroundColor7", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor7"].Add("Background", "#E2EFF9");

            this.ColorScheme.Add("BackgroundColor9", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor9"].Add("Background", "#B7D8F0");
            //this.ColorScheme["BackgroundColor9"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor9"].Add("BorderColor", "#FFFFFF");


            this.ColorScheme.Add("BackgroundColor2", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor2"].Add("Background", "#FCB040");
            this.ColorScheme["BackgroundColor2"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor2"].Add("BorderColor", "#FFFFFF");

            this.ColorScheme.Add("BackgroundColor20", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor20"].Add("Background", "#FFCA49");
            this.ColorScheme["BackgroundColor20"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor20"].Add("BorderColor", "#FFFFFF");

            this.ColorScheme.Add("BackgroundColor21", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor21"].Add("Background", "#FFD34C");
            this.ColorScheme["BackgroundColor21"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor21"].Add("BorderColor", "#FFFFFF");

            this.ColorScheme.Add("Color1", new Dictionary<string, string>());
            this.ColorScheme["Color1"].Add("Color", "#6CAEE0");


            this.ColorScheme.Add("TableCellHeadline", new Dictionary<string, string>());
            this.ColorScheme["TableCellHeadline"].Add("Color", "#FFFFFF");
        }


        private void ExportTable(Table table, ExcelWriter writer)
        {
            writer.Position = rowOffsetStart;

            // Run through all table rows of the table.
            foreach (TableRow tableRow in table.Rows)
            {
                bool didTable = ExportTableRow(tableRow, writer);

                if (didTable)
                {
                    rowOffsetStart = writer.Position;
                    columnOffsetStart = 0;
                }
            }
        }

        private bool ExportTableRow(TableRow tableRow, ExcelWriter writer)
        {
            columnOffset = columnOffsetStart;

            bool didTable = false;

            // Run through all table cells of the table row.
            foreach (TableCell tableCell in tableRow.Cells)
            {
                bool result = ExportTableCell(tableCell, writer);

                if (result)
                    didTable = true;
            }

            if (!didTable)
                writer.NewLine();

            return didTable;
        }

        int rowOffsetStart = 0;
        int columnOffsetStart = 0;
        int columnOffset = 0;
        Guid idLeftScore = new Guid();
        private bool ExportTableCell(TableCell tableCell, ExcelWriter writer)
        {
            string html = "";

            string text = ExportChildNodes(tableCell, writer);

            if (text == null) return true;

            html += text;

            if (tableCell.Text != "")
                html = tableCell.Text;

            html = Regex.Replace(html, "<.*?>", string.Empty);
            html = html.Replace("&nbsp;", "");
            html = html.Trim();

            string[] classNames = tableCell.CssClass.Split(' ');

            if (classNames.Contains("TableCellHeadlineCategory"))
                html = "'" + html;

            int newColumnOffset = writer.Write(columnOffset, html);

            foreach (string className in classNames)
            {
                if (!this.ColorScheme.ContainsKey(className))
                    continue;

                foreach (KeyValuePair<string, string> attribute in this.ColorScheme[className])
                {
                    Color color;

                    switch (attribute.Key)
                    {
                        case "Background":

                            color = System.Drawing.ColorTranslator.FromHtml(this.ColorScheme[className][attribute.Key]);

                            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].Interior.Color = SpreadsheetGear.Color.FromArgb(color.A, color.R, color.G, color.B);

                            break;

                        case "BorderColor":

                            color = System.Drawing.ColorTranslator.FromHtml(this.ColorScheme[className][attribute.Key]);

                            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].Borders.Color = SpreadsheetGear.Color.FromArgb(color.A, color.R, color.G, color.B);
                            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].Borders.Weight = SpreadsheetGear.BorderWeight.Medium;

                            break;

                        case "Color":

                            color = System.Drawing.ColorTranslator.FromHtml(this.ColorScheme[className][attribute.Key]);

                            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].Font.Color = SpreadsheetGear.Color.FromArgb(color.A, color.R, color.G, color.B);

                            break;
                    }
                }
            }

            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].Borders.Color = SpreadsheetGear.Color.FromArgb(255, 255, 255);
            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].Borders.Weight = SpreadsheetGear.BorderWeight.Medium;

            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].HorizontalAlignment = SpreadsheetGear.HAlign.Center;
            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].VerticalAlignment = SpreadsheetGear.VAlign.Center;
            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].WrapText = true;

            if (tableCell.Attributes["XPath"] != null)
            {
                if (!this.ScoreNameReferences.ContainsKey(tableCell.Attributes["XPath"]))
                {
                    this.ScoreNameReferences.Add(
                        tableCell.Attributes["XPath"],
                        writer.ActiveSheet.Cells[writer.Position, newColumnOffset].GetAddress(
                            false,
                            false,
                            SpreadsheetGear.ReferenceStyle.A1,
                            true,
                            writer.ActiveSheet.Cells[writer.Position, newColumnOffset]
                        )
                    );
                }

                if (tableCell.CssClass.Contains("TableCellHeadlineLeftCategory") && tableCell.Attributes["IdCategory"] != null)
                {
                    idLeftScore = Guid.Parse(
                        tableCell.Attributes["IdCategory"]
                    );

                    if (!this.ValueReferences.ContainsKey(idLeftScore))
                    {
                        this.ValueReferences.Add(
                            idLeftScore,
                            new Dictionary<Guid, string>()
                        );
                    }
                }
            }

            if (tableCell.CssClass.Contains("TableCellValue") && tableCell.Attributes["IdCategory"] != null && tableCell.Attributes["IdLeftScore"] != null)
            {
                idLeftScore = Guid.Parse(
                    tableCell.Attributes["IdLeftScore"]
                );

                Guid idTopScore = Guid.Parse(
                    tableCell.Attributes["IdCategory"]
                );

                if (this.ValueReferences.ContainsKey(idLeftScore))
                {
                    int rowOffset = writer.Position;

                    rowOffset++;

                    /*if (this.ReportDefinition.Settings.SignificanceTest)
                        rowOffset++;*/

                    if (!this.ValueReferences[idLeftScore].ContainsKey(idTopScore))
                    {
                        this.ValueReferences[idLeftScore].Add(idTopScore, writer.ActiveSheet.Cells[rowOffset, newColumnOffset].GetAddress(
                            false,
                            false,
                            SpreadsheetGear.ReferenceStyle.A1,
                            true,
                            writer.ActiveSheet.Cells[writer.Position, newColumnOffset]
                        ));
                    }
                }
            }

            if (tableCell.Attributes["ExportWidth"] != null)
                writer.ActiveSheet.Cells[writer.Position, newColumnOffset].ColumnWidth = int.Parse(tableCell.Attributes["ExportWidth"]);

            if (tableCell.RowSpan > 1 || tableCell.ColumnSpan > 1)
            {
                int rowOffset2 = writer.Position;

                if (tableCell.RowSpan > 1)
                    rowOffset2 += tableCell.RowSpan - 1;

                int columnOffset2 = newColumnOffset;

                if (tableCell.ColumnSpan > 1)
                    columnOffset2 += tableCell.ColumnSpan - 1;

                writer.Merge(
                    writer.Position,
                    newColumnOffset,
                    rowOffset2,
                    columnOffset2
                );
            }

            columnOffset = newColumnOffset;

            columnOffset++;

            return false;
        }

        private string ExportChildNodes(Control container, ExcelWriter writer)
        {
            // Run through all controls of the table cell.
            foreach (Control control in container.Controls)
            {
                string controlType = control.GetType().Name;

                // Check if the control is a table.
                //if (control.GetType() == typeof(Table))
                if (controlType == "Table")
                {
                    ExportTable((Table)control, writer);
                    columnOffsetStart = columnOffset;

                    return null;
                }
                else if (controlType == "Panel")
                {
                    ExportChildNodes(control, writer);

                    return null;
                }
                else if (controlType == "Label")
                {
                    return ((Label)control).Text;
                }
                else if (controlType == "HtmlGenericControl")
                {
                    return ((HtmlGenericControl)control).InnerHtml;
                }
            }

            return "";
        }

        public string Export()
        {
            ExcelWriter writer = new ExcelWriter();

            ExportDefinition(writer);

            ExportTable((Table)this.Source, writer);

            if (this.ReportDefinition.Settings.DisplayType != DisplayType.Crosstable)
            {
                BuildCharts(writer);
            }

            string fileName = Path.GetTempFileName() + ".xlsx";

            writer.Save(fileName);

            return fileName;
        }


        private void BuildCharts(ExcelWriter writer)
        {
            writer.NewSheet("Charts");
            int chartCount = 0;

            int left = 0;
            int top = 0;

            // Run through all left variables.
            foreach (ReportDefinitionVariable leftVariable in this.ReportDefinition.LeftVariables)
            {
                if (leftVariable.NestedVariables.Count > 0)
                    continue;

                // Run through all top variables.
                foreach (ReportDefinitionVariable topVariable in this.ReportDefinition.TopVariables)
                {
                    if (topVariable.NestedVariables.Count > 0)
                        continue;

                    SpreadsheetGear.Charts.IChart chart = writer.ActiveSheet.Shapes.AddChart(left, top, 500, 300).Chart;
                    chartCount++;

                    // Set the chart type.

                    switch (this.ReportDefinition.Settings.DisplayType)
                    {
                        case DisplayType.Column:
                            chart.ChartType = SpreadsheetGear.Charts.ChartType.ColumnClustered;
                            break;
                        case DisplayType.Line:
                            chart.ChartType = SpreadsheetGear.Charts.ChartType.Line;
                            break;
                        case DisplayType.Bar:
                            chart.ChartType = SpreadsheetGear.Charts.ChartType.BarClustered;
                            break;
                        case DisplayType.Area:
                            chart.ChartType = SpreadsheetGear.Charts.ChartType.Area;
                            break;
                        default:
                            chart.ChartType = SpreadsheetGear.Charts.ChartType.ColumnClustered;
                            break;
                    }

                    chart.Axes[0].MaximumScale = 1.0;

                    chart.HasTitle = true;
                    chart.Axes[SpreadsheetGear.Charts.AxisType.Category].HasTitle = true;
                    //chart.Axes[SpreadsheetGear.Charts.AxisType.Value].HasTitle = true;

                    chart.ChartTitle.Text = leftVariable.Label;
                    chart.Axes[SpreadsheetGear.Charts.AxisType.Category].AxisTitle.Text = topVariable.Label;


                    Color _color = System.Drawing.ColorTranslator.FromHtml(this.ColorScheme["Color1"]["Color"]);
                    SpreadsheetGear.Color color = SpreadsheetGear.Color.FromArgb(_color.A, _color.R, _color.G, _color.B);
                    chart.ChartTitle.Font.Color = color;
                    chart.Axes[SpreadsheetGear.Charts.AxisType.Category].AxisTitle.Font.Color = color;

                    string axisFormula = "=Sheet1!";

                    if (topVariable.Scores.Count > 0)
                    {
                        axisFormula += this.ScoreNameReferences[topVariable.Scores[0].XmlNode.GetXPath(true)].Split('!')[1];
                    }

                    if (topVariable.Scores.Count > 1)
                    {
                        axisFormula += ":" + this.ScoreNameReferences[topVariable.Scores[topVariable.Scores.Count - 1].XmlNode.GetXPath(true)].Split('!')[1];
                    }

                    // Run through all scores of the left variable.
                    foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                    {
                        string valuesFormula = "=Sheet1!";

                        // Create a new chart series for the left score.
                        SpreadsheetGear.Charts.ISeries series = chart.SeriesCollection.Add();
                        //series.Name = "=Sheet1!B6";
                        series.Name = "=" + this.ScoreNameReferences[leftScore.XmlNode.GetXPath(true)];

                        if (topVariable.Scores.Count > 0)
                        {
                            Guid idTopScore = topVariable.Scores[0].Identity;

                            if (this.ValueReferences[leftScore.Identity].ContainsKey(idTopScore))
                                valuesFormula += this.ValueReferences[leftScore.Identity][idTopScore].Split('!')[1];
                        }

                        if (topVariable.Scores.Count > 1)
                        {
                            Guid idTopScore = topVariable.Scores[topVariable.Scores.Count - 1].Identity;

                            if (this.ValueReferences[leftScore.Identity].ContainsKey(idTopScore))
                                valuesFormula += ":" + this.ValueReferences[leftScore.Identity][idTopScore].Split('!')[1];
                        }

                        try
                        {
                            series.Values = valuesFormula;
                        }
                        catch { }

                        for (int i = 0; i < topVariable.Scores.Count; i++)
                        {
                            series.Points[i].HasDataLabel = true;
                            series.Points[i].DataLabel.Text = "=" + this.ValueReferences[leftScore.Identity][topVariable.Scores[i].Identity];
                        }

                        series.XValues = axisFormula;
                    }

                    //chart.Axes[SpreadsheetGear.Charts.AxisType.Value].AxisTitle.Text = axisFormula;
                    //chart.SetSourceData(writer.ActiveSheet.Cells[axisFormula], SpreadsheetGear.Charts.RowCol.Columns);

                    left += 500;
                }

                left = 0;
                top += 300;
            }

            if (chartCount == 0)
            {
                writer.Workbook.Worksheets[0].Select();
            }
        }


        private void ExportDefinition(ExcelWriter writer)
        {
            Table table = new Table();

            TableRow tableRowName = new TableRow();
            TableCell tableCellNameTitle = new TableCell();
            TableCell tableCellNameValue = new TableCell();

            
        }

        #endregion
    }
}
