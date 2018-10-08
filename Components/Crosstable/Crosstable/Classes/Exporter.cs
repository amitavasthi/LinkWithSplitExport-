using ApplicationUtilities.Classes;
using Crosstables.Classes.ReportDefinitionClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using WebUtilities;

namespace Crosstables.Classes
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
            this.ColorScheme["BackgroundColor1"].Add("BorderColor", "#000000");

            this.ColorScheme.Add("BackgroundColor5", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor5"].Add("Background", "#98C6E9");
            //this.ColorScheme["BackgroundColor5"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor5"].Add("BorderColor", "#000000");

            this.ColorScheme.Add("BackgroundColor7", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor7"].Add("Background", "#E2EFF9");

            this.ColorScheme.Add("BackgroundColor9", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor9"].Add("Background", "#B7D8F0");
            //this.ColorScheme["BackgroundColor9"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor9"].Add("BorderColor", "#000000");


            this.ColorScheme.Add("BackgroundColor2", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor2"].Add("Background", "#FCB040");
            this.ColorScheme["BackgroundColor2"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor2"].Add("BorderColor", "#000000");

            this.ColorScheme.Add("BackgroundColor20", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor20"].Add("Background", "#FFCA49");
            this.ColorScheme["BackgroundColor20"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor20"].Add("BorderColor", "#000000");

            this.ColorScheme.Add("BackgroundColor21", new Dictionary<string, string>());
            this.ColorScheme["BackgroundColor21"].Add("Background", "#FFD34C");
            this.ColorScheme["BackgroundColor21"].Add("Color", "#FFFFFF");
            this.ColorScheme["BackgroundColor21"].Add("BorderColor", "#000000");

            this.ColorScheme.Add("Color1", new Dictionary<string, string>());
            this.ColorScheme["Color1"].Add("Color", "#6CAEE0");

            this.ColorScheme.Add("TableCellValue", new Dictionary<string, string>());
            this.ColorScheme["TableCellValue"].Add("BorderColor", "#000000");

            this.ColorScheme.Add("TableCellHeadline", new Dictionary<string, string>());
            this.ColorScheme["TableCellHeadline"].Add("BorderColor", "#000000");

            this.ColorScheme.Add("TableCellLowBase", new Dictionary<string, string>());
            this.ColorScheme["TableCellLowBase"].Add("Background", "#ffbcbc");

            /* this.ColorScheme.Add("TableCellExportSettingsOverviewValue", new Dictionary<string, string>());
             this.ColorScheme["TableCellExportSettingsOverviewValue"].Add("BorderColor", "#000000");*/
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

            /*if (tableRow.Cells.Count == 0)
                return false;*/

            bool didTable = false;

            if (tableRow.Cells.Count == 0)
            {
                tableRow.Cells.Add(new TableCell());
            }

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
                html = HttpUtility.HtmlDecode(tableCell.Text);


            html = Regex.Replace(html, "<.*?>", string.Empty);
            html = html.Replace("&nbsp;", "");
            html = html.Trim();

            string[] classNames = tableCell.CssClass.Split(' ');

            if (classNames.Contains("TableCellHeadlineCategory"))
                html = "'" + html;

            if (html.EndsWith("%"))
            {
                string format = "###,##";

                if (this.ReportDefinition.Settings.DecimalPlaces > 0)
                    format += ".";

                for (int i = 0; i < this.ReportDefinition.Settings.DecimalPlaces; i++)
                    format += "#";

                format += "%";

                writer.ActiveSheet.Cells[writer.Position, columnOffset].NumberFormat = format;

                if (html.StartsWith("'"))
                    html = html.Remove(0, 1);

                html = html.Replace("%", "");
                html = html.Trim();
                html = (double.Parse(html) / 100).ToString();
            }
            /*if (html.EndsWith("-"))
            {
                writer.ActiveSheet.Cells[writer.Position, columnOffset].NumberFormat = "0 \"%\"";

                html = html.Replace("-", "0");
                html = html.Trim();
            }*/

            if (this.ReportDefinition.Settings.ShowValues)
            {
                if (html.ToString() == "-")
                {
                    html = html.Replace("-", "");
                    html = html.Trim();
                }
            }

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
                            GetBorder(tableCell, writer, newColumnOffset, color);
                            break;

                        case "BorderColor":

                            color = System.Drawing.ColorTranslator.FromHtml(this.ColorScheme[className][attribute.Key]);
                            GetBorder(tableCell, writer, newColumnOffset, color);
                            break;

                        case "Color":

                            color = System.Drawing.ColorTranslator.FromHtml(this.ColorScheme[className][attribute.Key]);
                            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].Font.Color = SpreadsheetGear.Color.FromArgb(color.A, color.R, color.G, color.B);
                            break;
                    }
                }
            }

            if (tableCell.CssClass.Contains("TableCellLowBase"))
            {
                foreach (KeyValuePair<string, string> attribute in this.ColorScheme["TableCellLowBase"])
                {
                    Color color;

                    switch (attribute.Key)
                    {
                        case "Background":

                            color = System.Drawing.ColorTranslator.FromHtml(this.ColorScheme["TableCellLowBase"][attribute.Key]);
                            writer.ActiveSheet.Cells[writer.Position, newColumnOffset].Interior.Color = SpreadsheetGear.Color.FromArgb(color.A, color.R, color.G, color.B);
                            GetBorder(tableCell, writer, newColumnOffset, color);
                            break;
                    }
                }
            }


            if ((tableCell.Attributes["Wrap"] == null || bool.Parse(tableCell.Attributes["Wrap"]) != false))
            {
                if (tableCell.Attributes["class"] != null || tableCell.Attributes["style"] != null)
                {
                    writer.ActiveSheet.Cells[writer.Position, newColumnOffset].Borders.Weight = SpreadsheetGear.BorderWeight.Thin;
                }
                else if (tableCell.Text.Contains("<td><div style="))
                {
                    writer.ActiveSheet.Cells[writer.Position, newColumnOffset].Borders.Weight = SpreadsheetGear.BorderWeight.Thin;
                }

                writer.ActiveSheet.Cells[writer.Position, newColumnOffset].HorizontalAlignment = SpreadsheetGear.HAlign.Center;
                writer.ActiveSheet.Cells[writer.Position, newColumnOffset].VerticalAlignment = SpreadsheetGear.VAlign.Center;
                writer.ActiveSheet.Cells[writer.Position, newColumnOffset].WrapText = true;
            }

            if (tableCell.Attributes["height"] != null)
            {
                writer.ActiveSheet.Cells[writer.Position, newColumnOffset].RowHeight = int.Parse(tableCell.Attributes["height"]);
            }


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

            if (tableCell.Attributes["IdCategory"] != null && tableCell.Attributes["IdLeftScore"] != null)
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

                    //rowOffset++;

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

                writer.ActiveSheet.Cells[writer.Position, newColumnOffset, rowOffset2, columnOffset2].Borders.Weight = SpreadsheetGear.BorderWeight.Thin;

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
                    string result = ExportChildNodes(control, writer);

                    return result;
                }
                else if (controlType == "Label")
                {
                    return ((Label)control).Text;
                }
                else if (controlType == "HtmlGenericControl")
                {
                    string html = ((HtmlGenericControl)control).InnerHtml;

                    if (html.StartsWith("<table"))
                    {
                        return ExportChildNodes(ConvertHtmlTableStringToTable(html), writer);
                    }
                    else
                    {
                        return html;
                    }
                }
                else if (controlType == "LiteralControl")
                {
                    return ((LiteralControl)control).Text;
                }
            }

            return "";
        }

        public Panel ConvertHtmlTableStringToTable(string html)
        {
            XmlDocument document = new XmlDocument();

            if (html.IndexOf("<script>") > 0)
            {
                html = html.Remove(html.IndexOf("<script>"), html.Length - html.IndexOf("<script>"));
            }
            // html = html.Remove(html.IndexOf("<script>"), html.Length - html.IndexOf("<script>"));

            document.LoadXml(html);

            Panel result = new Panel();

            Control control = ConvertHtmlToControl(document.DocumentElement);

            if (control != null)
                result.Controls.Add(control);

            return result;
        }

        public Control ConvertHtmlToControl(XmlNode control)
        {
            WebControl result;

            bool copyAttributes = true;

            switch (control.Name)
            {
                case "table":
                    result = new Table();
                    break;
                case "tr":
                    if (control.ChildNodes.Count == 0)
                        return null;
                    result = new TableRow();
                    break;
                case "td":
                    result = new TableCell();

                    if (control.Attributes["rowspan"] != null)
                    {
                        ((TableCell)result).RowSpan = int.Parse(control.Attributes["rowspan"].Value);
                    }
                    if (control.Attributes["colspan"] != null)
                    {
                        ((TableCell)result).ColumnSpan = int.Parse(control.Attributes["colspan"].Value);
                    }
                    if (control.Attributes["class"] != null)
                    {
                        ((TableCell)result).CssClass = control.Attributes["class"].Value;
                    }
                    break;
                default:
                    copyAttributes = false;
                    result = new Panel();
                    ((Panel)result).Controls.Add(new LiteralControl(control.InnerText));
                    break;
            }

            if (copyAttributes)
            {
                foreach (XmlAttribute attribute in control.Attributes)
                {
                    result.Attributes.Add(attribute.Name, attribute.Value);
                }

                foreach (XmlNode xmlNodeChild in control.ChildNodes)
                {
                    Control c = ConvertHtmlToControl(xmlNodeChild);

                    if (c != null)
                        result.Controls.Add(c);
                }
            }

            return result;
        }

        public string Export(string fileName = null)
        {
            ExcelWriter writer = new ExcelWriter(fileName);

            string name = "";

            if (this.ReportDefinition.XmlDocument.DocumentElement.Attributes["Name"] != null)
                name = this.ReportDefinition.XmlDocument.DocumentElement.Attributes["Name"].Value;

            if (name.Length > 30)
                name = name.Substring(0, 30);

            if (fileName != null)
                writer.NewSheet(name);
            else
                writer.ActiveSheet.Name = name;

            ExportDefinition(writer);

            rowOffsetStart = writer.Position;

            Table tableNote = new Table();
            TableRow tableRowNote = new TableRow();
            TableCell tableCellNote = new TableCell();
            tableCellNote.Text = this.LanguageManager.GetText("ExportNote");
            tableCellNote.ColumnSpan = 13;
            tableCellNote.Attributes.Add("Wrap", "False");

            tableRowNote.Cells.Add(tableCellNote);
            tableNote.Rows.Add(tableRowNote);

            tableCellNote.Style.Add("font-weight", "bold");
            ExportTable(tableNote, writer);

            rowOffsetStart = writer.Position + 2;

            ExportTable((Table)this.Source, writer);

            if ((this.ReportDefinition.Settings.DisplayType != DisplayType.Crosstable)&&(this.ReportDefinition.Settings.DisplayType !=DisplayType.WordCloud))
            {
                BuildCharts(writer);
            }

            string result;

            if (string.IsNullOrEmpty(fileName) || File.Exists(fileName) == false)
                result = Path.GetTempFileName() + ".xlsx";
            else
                result = fileName;

            writer.Save(result);

            return result;
        }


        private void BuildCharts(ExcelWriter writer)
        {
            //writer.NewSheet("Charts");
            int chartCount = 0;

            int left = 0;
            int top = writer.Position * 15;
            if (this.ReportDefinition.IdHierarchies.Count > 0)
                if (writer.Position < 100)
                    top += (this.ReportDefinition.IdHierarchies.Count) * 15;
                else
                    top = writer.Position * 14;
            // Run through all left variables.
            foreach (ReportDefinitionVariable leftVariable in this.ReportDefinition.LeftVariables)
            {
                if (leftVariable.NestedVariables.Count > 0)
                    continue;

                if (this.ReportDefinition.Settings.DisplayType == DisplayType.Pie)
                {
                    SpreadsheetGear.Charts.IChart chart = writer.ActiveSheet.Shapes.AddChart(left, top, 500, 300).Chart;
                    chartCount++;
                    chart.ChartType = SpreadsheetGear.Charts.ChartType.Pie;
                    //SpreadsheetGear.IRange cells = writer.ActiveSheet.Cells["D14,D17,D20,D23,D26,D29,D32,D35"];

                    //chart.SetSourceData(cells, SpreadsheetGear.Charts.RowCol.Rows);

                    // Create a new chart series for the left score.
                    SpreadsheetGear.Charts.ISeries series = chart.SeriesCollection.Add();
                    //series.Values = "='new report'!$D$14,'new report'!$D$17,'new report'!$D$20,'new report'!$D$23,'new report'!$D$26,'new report'!$D$29,'new report'!$D$32,'new report'!$D$35";
                    //series.XValues = "='new report'!$B$13,'new report'!$B$16,'new report'!$B$19,'new report'!$B$22,'new report'!$B$25,'new report'!$B$28,'new report'!$B$31,'new report'!$B$34";

                    StringBuilder valuesBuilder = new StringBuilder();
                    StringBuilder xValuesBuilder = new StringBuilder();

                    // Run through all scores of the left variable.
                    foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                    {
                        string xPath = leftScore.XmlNode.GetXPath(true);

                        if (!this.ScoreNameReferences.ContainsKey(xPath))
                            continue;

                        xValuesBuilder.Append(this.ScoreNameReferences[xPath]);
                        xValuesBuilder.Append(",");

                        try
                        {
                            string reference = this.ScoreNameReferences[xPath].Split('!')[1];

                            //reference[0] = System.Text.Encoding.UTF8.GetString(new byte[] { (byte)((int)(reference[0]) + 1) })[0];
                            //reference = reference.Remove(0, 1).Insert(0, System.Text.Encoding.UTF8.GetString(new byte[] { (byte)((int)(reference[0]) + 2) }));
                            //if ((this.ReportDefinition.Settings.ShowValues) && (this.ReportDefinition.Settings.ShowPercentage))
                            //{
                            //    reference = reference.Remove(1, reference.Length - 1).Insert(1, (int.Parse(reference.Substring(1, reference.Length - 1)) + 1).ToString());
                            //}


                            if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true) && (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                            {
                                reference = reference.Remove(0, 1).Insert(0, System.Text.Encoding.UTF8.GetString(new byte[] { (byte)((int)(reference[0]) + 4) }));

                            }
                            else if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true) || (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                            {
                                reference = reference.Remove(0, 1).Insert(0, System.Text.Encoding.UTF8.GetString(new byte[] { (byte)((int)(reference[0]) + 3) }));
                            }
                            else
                            {
                                reference = reference.Remove(0, 1).Insert(0, System.Text.Encoding.UTF8.GetString(new byte[] { (byte)((int)(reference[0]) + 2) }));
                            }


                            //    if (this.ReportDefinition.Settings.DisplayUnweightedBase == true)
                            //{
                            //    reference = reference.Remove(0, 1).Insert(0, System.Text.Encoding.UTF8.GetString(new byte[] { (byte)((int)(reference[0]) + 3) }));
                            //}
                            //else
                            //{
                            //    reference = reference.Remove(0, 1).Insert(0, System.Text.Encoding.UTF8.GetString(new byte[] { (byte)((int)(reference[0]) + 2) }));
                            //}
                            //if (this.ReportDefinition.Settings.DisplayEffectiveBase == true)
                            //{
                            //    reference = reference.Remove(0, 1).Insert(0, System.Text.Encoding.UTF8.GetString(new byte[] { (byte)((int)(reference[0]) + 3) }));
                            //}
                            //else
                            //{
                            //    reference = reference.Remove(0, 1).Insert(0, System.Text.Encoding.UTF8.GetString(new byte[] { (byte)((int)(reference[0]) + 2) }));
                            //}
                            if ((this.ReportDefinition.Settings.ShowValues) && (this.ReportDefinition.Settings.ShowPercentage))
                            {
                                reference = reference.Remove(1, reference.Length - 1).Insert(1, (int.Parse(reference.Substring(1, reference.Length - 1)) + 1).ToString());

                            }
                            else if (this.ReportDefinition.Settings.ShowValues || this.ReportDefinition.Settings.ShowPercentage)
                            {
                                reference = reference.Remove(1, reference.Length - 1).Insert(1, (int.Parse(reference.Substring(1, reference.Length - 1))).ToString());
                            }

                            valuesBuilder.Append(reference);
                            valuesBuilder.Append(",");
                        }
                        catch
                        {

                        }
                    }

                    if (xValuesBuilder.Length > 0)
                        xValuesBuilder.Remove(xValuesBuilder.Length - 1, 1);

                    if (valuesBuilder.Length > 0)
                        valuesBuilder.Remove(valuesBuilder.Length - 1, 1);

                    series.Values = "=" + valuesBuilder.ToString();
                    series.XValues = "=" + xValuesBuilder.ToString();
                    series.HasDataLabels = true;
                    //if (this.ReportDefinition.Settings.ShowPercentage)
                    //{
                    //    series.DataLabels.NumberFormat = "0 \"%\"";
                    //}
                    //else
                    //{
                    //    series.DataLabels.NumberFormat = "0";
                    //}

                    chart.HasTitle = true;


                    chart.ChartTitle.Text = leftVariable.Label;

                    Color _color = System.Drawing.ColorTranslator.FromHtml(this.ColorScheme["Color1"]["Color"]);
                    SpreadsheetGear.Color color = SpreadsheetGear.Color.FromArgb(_color.A, _color.R, _color.G, _color.B);
                    chart.ChartTitle.Font.Color = color;
                    left += 500;

                }
                else
                {
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
                                chart.Axes[0].ReversePlotOrder = true;
                                break;
                            case DisplayType.Area:
                                chart.ChartType = SpreadsheetGear.Charts.ChartType.Area;
                                break;
                            case DisplayType.Pie:
                                chart.ChartType = SpreadsheetGear.Charts.ChartType.Pie;
                                break;                       
                            default:
                                chart.ChartType = SpreadsheetGear.Charts.ChartType.ColumnClustered;
                                break;
                        }


                        chart.Axes[0].MaximumScale = 1.0;

                        chart.HasTitle = true;
                        chart.Axes[SpreadsheetGear.Charts.AxisType.Category].HasTitle = true;
                        //chart.Axes[SpreadsheetGear.Charts.AxisType.Value].HasTitle = true;
                       // chart.Axes[SpreadsheetGear.Charts.AxisType.Value].TickLabels.NumberFormat = "0";

                        chart.ChartTitle.Text = leftVariable.Label;
                        chart.Axes[SpreadsheetGear.Charts.AxisType.Category].AxisTitle.Text = topVariable.Label;

                        Color _color = System.Drawing.ColorTranslator.FromHtml(this.ColorScheme["Color1"]["Color"]);
                        SpreadsheetGear.Color color = SpreadsheetGear.Color.FromArgb(_color.A, _color.R, _color.G, _color.B);
                        chart.ChartTitle.Font.Color = color;

                        if (chart.Axes[SpreadsheetGear.Charts.AxisType.Category].AxisTitle != null)
                            chart.Axes[SpreadsheetGear.Charts.AxisType.Category].AxisTitle.Font.Color = color;

                        string axisFormula = "=";



                        // Run through all scores of the left variable.
                        foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                        {
                            string path = leftScore.XmlNode.GetXPath(true);

                            if (this.ScoreNameReferences.ContainsKey(path))
                                axisFormula += this.ScoreNameReferences[path].Split('!')[1] + ",";
                        }

                        if (leftVariable.ScoresCount > 0)
                            axisFormula = axisFormula.Remove(axisFormula.Length - 1, 1);

                        if (topVariable.IsFake)
                        {
                            string valuesFormula = "=";

                            string xPath = topVariable.Scores[0].XmlNode.GetXPath(true);

                            /*if (!this.ScoreNameReferences.ContainsKey(xPath))
                                continue;*/

                            // Create a new chart series for the left score.
                            SpreadsheetGear.Charts.ISeries series = chart.SeriesCollection.Add();
                            //series.Name = "=Sheet1!B6";
                            if (this.ScoreNameReferences.ContainsKey(xPath))
                                series.Name = "=" + this.ScoreNameReferences[xPath];


                            // Run through all scores of the left variable.
                            foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                            {
                                string path = leftScore.XmlNode.GetXPath(true);

                                if (this.ScoreNameReferences.ContainsKey(path))
                                    valuesFormula += this.ScoreNameReferences[path].Split('!')[1].Replace("B", "D") + ",";

                                /*if (this.ValueReferences.ContainsKey(leftScore.Identity) && this.ValueReferences[leftScore.Identity].ContainsKey(topVariable.Scores[0].Identity))
                                    valuesFormula += this.ValueReferences[leftScore.Identity][topVariable.Scores[0].Identity].Split('!')[1] + ",";*/
                            }

                            /* fix for bug number 371 in bug tracker */
                            int tempVal = 0;
                            string tempString = "";

                            if ((valuesFormula.Length == 1) && (valuesFormula == "="))
                            {
                                string tempValuesFormula = axisFormula.Remove(0, 1);
                                if (axisFormula.Length > 0)
                                {
                                    int val = tempValuesFormula.IndexOf(",");
                                    if (val < 0)
                                    {
                                        val = tempValuesFormula.Length;
                                    }

                                    tempString = tempValuesFormula.Substring(1, val - 1);
                                    //tempVal = Convert.ToInt32(tempString) + 1;
                                    if ((this.ReportDefinition.Settings.ShowValues) && (this.ReportDefinition.Settings.ShowPercentage))
                                    {
                                        tempVal = Convert.ToInt32(tempString) + 1;
                                    }
                                    else if (this.ReportDefinition.Settings.ShowValues || this.ReportDefinition.Settings.ShowPercentage)
                                    {
                                        tempVal = Convert.ToInt32(tempString);
                                    }
                                    foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                                    {
                                        if (leftScore.Hidden)
                                            continue;
                                        /*Bug fix for 438 & 455*/
                                        //if (this.ReportDefinition.Settings.DisplayUnweightedBase == true)
                                        //{
                                        //    valuesFormula += "E" + tempVal + ",";
                                        //    tempVal += 2;
                                        //}
                                        //else
                                        //{
                                        //    valuesFormula += "D" + tempVal + ",";
                                        //    tempVal += 2;
                                        //}
                                        if ((this.ReportDefinition.Settings.ShowValues) && (this.ReportDefinition.Settings.ShowPercentage) && (!this.ReportDefinition.Settings.SignificanceTest))
                                        {
                                            if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true) &&(this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                            {
                                                valuesFormula += "F" + tempVal + ",";
                                                tempVal += 2;
                                            }
                                            if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true) || (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                            {
                                                valuesFormula += "E" + tempVal + ",";
                                                tempVal += 2;
                                            }                                            
                                            else
                                            {
                                                valuesFormula += "D" + tempVal + ",";
                                                tempVal += 2;
                                            }

                                        }
                                        else if ((this.ReportDefinition.Settings.ShowValues || this.ReportDefinition.Settings.ShowPercentage) && (!this.ReportDefinition.Settings.SignificanceTest))
                                        {
                                            if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true)&& (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                            {
                                                valuesFormula += "F" + tempVal + ",";
                                                tempVal += 1;
                                            }
                                            else
                                            if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true) || (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                            {
                                                valuesFormula += "E" + tempVal + ",";
                                                tempVal += 1;
                                            }
                                            else
                                            {
                                                valuesFormula += "D" + tempVal + ",";
                                                tempVal += 1;
                                            }
                                        }
                                        if ((this.ReportDefinition.Settings.ShowValues) && (this.ReportDefinition.Settings.ShowPercentage) && (this.ReportDefinition.Settings.SignificanceTest))
                                        {
                                            if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true)&& (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                            {
                                                valuesFormula += "F" + tempVal + ",";
                                                tempVal += 3;
                                            }
                                            else
                                           if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true) || (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                            {
                                                valuesFormula += "E" + tempVal + ",";
                                                tempVal += 3;
                                            }
                                            else
                                            {
                                                valuesFormula += "D" + tempVal + ",";
                                                tempVal += 3;
                                            }

                                        }
                                        /*ends 438 & 455*/
                                    }
                                    valuesFormula = valuesFormula.Remove(valuesFormula.Length - 1);

                                }
                            }
                            /*ends 371*/

                            if (leftVariable.ScoresCount > 0)
                            {
                                // valuesFormula = valuesFormula.Remove(valuesFormula.Length - 1, 1);
                                if (valuesFormula.Substring(valuesFormula.Length - 1) == ",")
                                {
                                    valuesFormula = valuesFormula.Remove(valuesFormula.Length - 1, 1);
                                }
                            }
                            try
                            {
                                series.Values = valuesFormula;
                            }
                            catch { }

                            for (int i = 0; i < leftVariable.ScoresCount; i++)
                            {
                                if (series.Points.Count <= i)
                                    break;

                                series.Points[i].HasDataLabel = true;
                                //series.Points[i].DataLabel.NumberFormat = "0 \"%\"";
                                //if (this.ReportDefinition.Settings.ShowPercentage)
                                //{
                                //    series.Points[i].DataLabel.NumberFormat = "0 \"%\"";
                                //}
                                //else
                                //{
                                //    series.Points[i].DataLabel.NumberFormat = "0";
                                //}

                                //if (this.ValueReferences.ContainsKey(leftVariable.Scores[i].Identity) && this.ValueReferences[leftVariable.Scores[i].Identity].ContainsKey(topScore.Identity))
                                //series.Points[i].DataLabel.Text = "=" + this.ValueReferences[leftVariable.Scores[i].Identity][topScore.Identity];

                            }

                            if (axisFormula != "=")
                                series.XValues = axisFormula;
                        }
                        else
                        {
                            // Run through all scores of the left variable.
                            foreach (ReportDefinitionScore topScore in topVariable.Scores)
                            {
                                if (topScore.Hidden)
                                    continue;

                                if (topScore.HasValues == false && this.ReportDefinition.Settings.HideEmptyRowsAndColumns)
                                    continue;
                                if (topScore.LabelValues == null)
                                    continue;


                                // (topScore  == false && this.ReportDefinition.Settings.HideEmptyRowsAndColumns)
                                //    continue;

                                string valuesFormula = "=";

                                string xPath = topScore.XmlNode.GetXPath(true);

                                /*if (!this.ScoreNameReferences.ContainsKey(xPath))
                                    continue;*/

                                // Create a new chart series for the left score.
                                SpreadsheetGear.Charts.ISeries series = chart.SeriesCollection.Add();
                                //series.Name = "=Sheet1!B6";
                                if (this.ScoreNameReferences.ContainsKey(xPath))
                                    series.Name = "=" + this.ScoreNameReferences[xPath];


                                // Run through all scores of the left variable.
                                foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                                {
                                    if (this.ValueReferences.ContainsKey(leftScore.Identity) && this.ValueReferences[leftScore.Identity].ContainsKey(topScore.Identity))
                                        valuesFormula += this.ValueReferences[leftScore.Identity][topScore.Identity].Split('!')[1] + ",";
                                }

                                /* fix for bug number 371 in bug tracker */
                                int tempVal = 0;
                                string tempString = "";

                                if ((valuesFormula.Length == 1) && (valuesFormula == "="))
                                {
                                    string tempValuesFormula = axisFormula.Remove(0, 1);
                                    if (axisFormula.Length > 0)
                                    {
                                        int val = tempValuesFormula.IndexOf(",");
                                        if (val < 0)
                                        {
                                            val = tempValuesFormula.Length;
                                        }

                                        tempString = tempValuesFormula.Substring(1, val - 1);
                                        //tempVal = Convert.ToInt32(tempString) + 1;
                                        if ((this.ReportDefinition.Settings.ShowValues) && (this.ReportDefinition.Settings.ShowPercentage))
                                        {
                                            tempVal = Convert.ToInt32(tempString) + 1;
                                        }
                                        else if (this.ReportDefinition.Settings.ShowValues || this.ReportDefinition.Settings.ShowPercentage)
                                        {
                                            tempVal = Convert.ToInt32(tempString);
                                        }
                                        foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                                        {
                                            if (leftScore.Hidden)
                                                continue;
                                            /*Bug fix for 438 & 455*/
                                            //if (this.ReportDefinition.Settings.DisplayUnweightedBase == true)
                                            //{
                                            //    valuesFormula += "E" + tempVal + ",";
                                            //    tempVal += 2;
                                            //}
                                            //else
                                            //{
                                            //    valuesFormula += "D" + tempVal + ",";
                                            //    tempVal += 2;
                                            //}
                                            if ((this.ReportDefinition.Settings.ShowValues) && (this.ReportDefinition.Settings.ShowPercentage) && (!this.ReportDefinition.Settings.SignificanceTest))
                                            {
                                                if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true)&& (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                                {
                                                    valuesFormula += "F" + tempVal + ",";
                                                    tempVal += 2;
                                                }
                                                else
                                              if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true) || (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                                {
                                                    valuesFormula += "E" + tempVal + ",";
                                                    tempVal += 2;
                                                }
                                                else
                                                {
                                                    valuesFormula += "D" + tempVal + ",";
                                                    tempVal += 2;
                                                }

                                            }
                                            else if ((this.ReportDefinition.Settings.ShowValues || this.ReportDefinition.Settings.ShowPercentage) && (!this.ReportDefinition.Settings.SignificanceTest))
                                            {
                                                if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true)&& (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                                {
                                                    valuesFormula += "F" + tempVal + ",";
                                                    tempVal += 1;
                                                }
                                                else
                                                if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true) || (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                                {
                                                    valuesFormula += "E" + tempVal + ",";
                                                    tempVal += 1;
                                                }
                                                else
                                                {
                                                    valuesFormula += "D" + tempVal + ",";
                                                    tempVal += 1;
                                                }
                                            }
                                            if ((this.ReportDefinition.Settings.ShowValues) && (this.ReportDefinition.Settings.ShowPercentage) && (this.ReportDefinition.Settings.SignificanceTest))
                                            {
                                                if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true)&& (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                                {
                                                    valuesFormula += "F" + tempVal + ",";
                                                    tempVal += 3;
                                                }
                                                else
                                              if ((this.ReportDefinition.Settings.DisplayUnweightedBase == true) || (this.ReportDefinition.Settings.DisplayEffectiveBase == true))
                                                {
                                                    valuesFormula += "E" + tempVal + ",";
                                                    tempVal += 3;
                                                }
                                                else
                                                {
                                                    valuesFormula += "D" + tempVal + ",";
                                                    tempVal += 3;
                                                }

                                            }
                                            /*ends 438 & 455*/
                                        }
                                        valuesFormula = valuesFormula.Remove(valuesFormula.Length - 1);

                                    }
                                }
                                /*ends 371*/

                                if (leftVariable.ScoresCount > 0)
                                {
                                    // valuesFormula = valuesFormula.Remove(valuesFormula.Length - 1, 1);
                                    if (valuesFormula.Substring(valuesFormula.Length - 1) == ",")
                                    {
                                        valuesFormula = valuesFormula.Remove(valuesFormula.Length - 1, 1);
                                    }
                                }
                                try
                                {
                                    series.Values = valuesFormula;
                                }
                                catch { }

                                for (int i = 0; i < leftVariable.ScoresCount; i++)
                                {
                                    if (series.Points.Count <= i)
                                        break;

                                    series.Points[i].HasDataLabel = true;
                                    //series.Points[i].DataLabel.NumberFormat = "0 \"%\"";
                                    //if (this.ReportDefinition.Settings.ShowPercentage)
                                    //{
                                    //    series.Points[i].DataLabel.NumberFormat = "0 \"%\"";
                                    //}
                                    //else
                                    //{
                                    //    series.Points[i].DataLabel.NumberFormat = "0";
                                    //}

                                    //if (this.ValueReferences.ContainsKey(leftVariable.Scores[i].Identity) && this.ValueReferences[leftVariable.Scores[i].Identity].ContainsKey(topScore.Identity))
                                    //series.Points[i].DataLabel.Text = "=" + this.ValueReferences[leftVariable.Scores[i].Identity][topScore.Identity];

                                }

                                if (axisFormula != "=")
                                    series.XValues = axisFormula;
                            }
                        }

                        //chart.Axes[SpreadsheetGear.Charts.AxisType.Value].AxisTitle.Text = axisFormula;
                        //chart.SetSourceData(writer.ActiveSheet.Cells[axisFormula], SpreadsheetGear.Charts.RowCol.Columns);

                        left += 500;
                    }
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
            table.CssClass = "TableExportSettingsOverview";

            TableRow tableRowName = new TableRow();
            TableCell tableCellNameTitle = new TableCell();
            TableCell tableCellNameValue = new TableCell();

            tableCellNameTitle.Text = this.LanguageManager.GetText("ReportName");

            if (this.ReportDefinition.XmlDocument.DocumentElement.Attributes["Name"] != null)
                tableCellNameValue.Text = this.ReportDefinition.XmlDocument.DocumentElement.Attributes["Name"].Value;

            tableRowName.Cells.Add(tableCellNameTitle);
            tableRowName.Cells.Add(tableCellNameValue);

            table.Rows.Add(tableRowName);

            TableRow tableRowFilter = new TableRow();
            TableCell tableCellFilterTitle = new TableCell();
            TableCell tableCellFilterValue = new TableCell();

            tableCellFilterTitle.Text = this.LanguageManager.GetText("ReportFilter");
            tableCellFilterValue.Text = this.RenderFilterString();

            tableRowFilter.Cells.Add(tableCellFilterTitle);
            tableRowFilter.Cells.Add(tableCellFilterValue);

            table.Rows.Add(tableRowFilter);

            TableRow tableRowBaseType = new TableRow();
            TableCell tableCellBaseTypeTitle = new TableCell();
            TableCell tableCellBaseTypeValue = new TableCell();

            tableCellBaseTypeTitle.Text = this.LanguageManager.GetText("ReportBaseType");
            tableCellBaseTypeValue.Text = this.ReportDefinition.Settings.BaseType == BaseType.AnsweringBase ? this.LanguageManager.GetText("SettingsBaseTypeAnsweringBase")
                                            : this.LanguageManager.GetText("SettingsBaseTypeTotalBase");
            tableRowBaseType.Cells.Add(tableCellBaseTypeTitle);
            tableRowBaseType.Cells.Add(tableCellBaseTypeValue);

            table.Rows.Add(tableRowBaseType);

            if (this.ReportDefinition.Settings.LowBaseConsider > 0)
            {
                TableRow tableRowLowBaseConsider = new TableRow();
                TableCell tableCellLowBaseConsiderTitle = new TableCell();
                TableCell tableCellLowBaseConsiderValue = new TableCell();

                tableCellLowBaseConsiderTitle.Text = this.LanguageManager.GetText("LowBaseConsiderExport");
                tableCellLowBaseConsiderValue.Text = "" + GetSigweightedType(this.ReportDefinition.Settings.LowBaseConsider);
                tableRowLowBaseConsider.Cells.Add(tableCellLowBaseConsiderTitle);
                tableRowLowBaseConsider.Cells.Add(tableCellLowBaseConsiderValue);

                table.Rows.Add(tableRowLowBaseConsider);

                tableCellLowBaseConsiderTitle.CssClass = "TableCellExportSettingsOverviewTitle";
                tableCellLowBaseConsiderValue.CssClass = "TableCellExportSettingsOverviewValue";

                tableCellLowBaseConsiderTitle.ColumnSpan = 3;
                tableCellLowBaseConsiderValue.ColumnSpan = 10;

                tableCellLowBaseConsiderValue.Attributes.Add("Wrap", "False");

            }

            if (this.ReportDefinition.Settings.LowBase > 0)
            {
                TableRow tableRowLowBase = new TableRow();
                TableCell tableCellLowBaseTitle = new TableCell();
                TableCell tableCellLowBaseValue = new TableCell();

                tableCellLowBaseTitle.Text = this.LanguageManager.GetText("LowBase");
                tableCellLowBaseValue.Text = this.ReportDefinition.Settings.LowBase.ToString();
                tableRowLowBase.Cells.Add(tableCellLowBaseTitle);
                tableRowLowBase.Cells.Add(tableCellLowBaseValue);

                table.Rows.Add(tableRowLowBase);

                tableCellLowBaseTitle.CssClass = "TableCellExportSettingsOverviewTitle";
                tableCellLowBaseValue.CssClass = "TableCellExportSettingsOverviewValue";

                tableCellLowBaseTitle.ColumnSpan = 3;
                tableCellLowBaseValue.ColumnSpan = 10;

                tableCellLowBaseValue.Attributes.Add("Wrap", "False");

            }

            TableRow tableRowWeighting = new TableRow();
            TableCell tableCellWeightingTitle = new TableCell();
            TableCell tableCellWeightingValue = new TableCell();

            tableCellWeightingTitle.Text = this.LanguageManager.GetText("ReportWeighting");

            if (this.ReportDefinition.WeightingFilters.DefaultWeighting.HasValue)
            {
                object label = this.ReportDefinition.Core.TaxonomyVariableLabels.GetValue(
                    "Label",
                    new string[] { "IdTaxonomyVariable", "IdLanguage" },
                    new object[] { this.ReportDefinition.WeightingFilters.DefaultWeighting.Value, this.ReportDefinition.Settings.IdLanguage }
                );

                if (label != null)
                    tableCellWeightingValue.Text = (string)label;
                else
                    tableCellWeightingValue.Text = "unknown";
            }
            else
            {
                tableCellWeightingValue.Text = this.LanguageManager.GetText("None");
            }

            tableRowWeighting.Cells.Add(tableCellWeightingTitle);
            tableRowWeighting.Cells.Add(tableCellWeightingValue);

            table.Rows.Add(tableRowWeighting);

            if (this.ReportDefinition.Settings.SignificanceTest)
            {
                TableRow tableRowSigDiff = new TableRow();
                TableCell tableCellSigDiffTitle = new TableCell();
                TableCell tableCellSigDiffValue = new TableCell();

                tableCellSigDiffTitle.Text = this.LanguageManager.GetText("ReportSigDiff");
                tableCellSigDiffValue.Text = "'" + this.ReportDefinition.Settings.SignificanceTestLevel + "%";

                tableRowSigDiff.Cells.Add(tableCellSigDiffTitle);
                tableRowSigDiff.Cells.Add(tableCellSigDiffValue);

                table.Rows.Add(tableRowSigDiff);

                tableCellSigDiffTitle.CssClass = "TableCellExportSettingsOverviewTitle";
                tableCellSigDiffValue.CssClass = "TableCellExportSettingsOverviewValue";

                tableCellSigDiffTitle.ColumnSpan = 3;
                tableCellSigDiffValue.ColumnSpan = 10;

                tableCellSigDiffValue.Attributes.Add("Wrap", "False");
            }

            if (this.ReportDefinition.Settings.SignificanceTestType!=9)
            {
                TableRow tableRowSigDiff = new TableRow();
                TableCell tableCellSigDiffTitle = new TableCell();
                TableCell tableCellSigDiffValue = new TableCell();

                tableCellSigDiffTitle.Text = this.LanguageManager.GetText("ReportSigDiffType");
                tableCellSigDiffValue.Text = "'" + GetSigDiffType(this.ReportDefinition.Settings.SignificanceTestType, this.ReportDefinition.Settings.SignificanceTest);

                tableRowSigDiff.Cells.Add(tableCellSigDiffTitle);
                tableRowSigDiff.Cells.Add(tableCellSigDiffValue);

                table.Rows.Add(tableRowSigDiff);

                tableCellSigDiffTitle.CssClass = "TableCellExportSettingsOverviewTitle";
                tableCellSigDiffValue.CssClass = "TableCellExportSettingsOverviewValue";

                tableCellSigDiffTitle.ColumnSpan = 3;
                tableCellSigDiffValue.ColumnSpan = 10;

                tableCellSigDiffValue.Attributes.Add("Wrap", "False");
            }
            //
            if (this.ReportDefinition.Settings.SignificanceWeight >0)
            {
                TableRow tableRowSigWeight = new TableRow();
                TableCell tableCellSigWeightTitle = new TableCell();
                TableCell tableCellSigWeightValue = new TableCell();

                tableCellSigWeightTitle.Text = this.LanguageManager.GetText("SignificanceWeightLabel");
                tableCellSigWeightValue.Text = "'" + GetSigweightedType(this.ReportDefinition.Settings.SignificanceWeight);

                tableRowSigWeight.Cells.Add(tableCellSigWeightTitle);
                tableRowSigWeight.Cells.Add(tableCellSigWeightValue);

                table.Rows.Add(tableRowSigWeight);

                tableCellSigWeightTitle.CssClass = "TableCellExportSettingsOverviewTitle";
                tableCellSigWeightValue.CssClass = "TableCellExportSettingsOverviewValue";

                tableCellSigWeightTitle.ColumnSpan = 3;
                tableCellSigWeightValue.ColumnSpan = 10;

                tableCellSigWeightValue.Attributes.Add("Wrap", "False");
            }
            //

            if (this.ReportDefinition.IdHierarchies != null && this.ReportDefinition.IdHierarchies.Count > 0)
            {
                TableRow tableRowHierarchy = new TableRow();
                TableCell tableCellHierarchyTitle = new TableCell();
                TableCell tableCellHierarchyValue = new TableCell();

                tableCellHierarchyTitle.ColumnSpan = 3;
                tableCellHierarchyValue.ColumnSpan = 10;

                tableCellHierarchyValue.Attributes.Add("height", (15 * this.ReportDefinition.IdHierarchies.Count).ToString());

                tableCellHierarchyTitle.Text = this.LanguageManager.GetText("ReportHierarchy");
                tableCellHierarchyValue.Text = this.RenderHierarchyString();

                tableRowHierarchy.Cells.Add(tableCellHierarchyTitle);
                tableRowHierarchy.Cells.Add(tableCellHierarchyValue);

                table.Rows.Add(tableRowHierarchy);
            }


            tableCellNameTitle.CssClass = "TableCellExportSettingsOverviewTitle";
            tableCellBaseTypeTitle.CssClass = "TableCellExportSettingsOverviewTitle";
            tableCellFilterTitle.CssClass = "TableCellExportSettingsOverviewTitle";
            tableCellWeightingTitle.CssClass = "TableCellExportSettingsOverviewTitle";

            tableCellNameValue.CssClass = "TableCellExportSettingsOverviewValue";
            tableCellBaseTypeValue.CssClass = "TableCellExportSettingsOverviewValue";
            tableCellFilterValue.CssClass = "TableCellExportSettingsOverviewValue";
            tableCellWeightingValue.CssClass = "TableCellExportSettingsOverviewValue";

            tableCellNameTitle.ColumnSpan = 3;
            tableCellBaseTypeTitle.ColumnSpan = 3;
            tableCellFilterTitle.ColumnSpan = 3;
            tableCellWeightingTitle.ColumnSpan = 3;

            tableCellNameValue.ColumnSpan = 10;
            tableCellBaseTypeValue.ColumnSpan = 10;
            tableCellFilterValue.ColumnSpan = 10;
            tableCellWeightingValue.ColumnSpan = 10;

            tableCellNameTitle.Style.Add("font-weight", "bold");
            tableCellBaseTypeTitle.Style.Add("font-weight", "bold");
            tableCellFilterTitle.Style.Add("font-weight", "bold");
            tableCellWeightingTitle.Style.Add("font-weight", "bold");

            tableCellNameValue.Attributes.Add("Wrap", "False");
            tableCellBaseTypeValue.Attributes.Add("Wrap", "False");
            tableCellFilterValue.Attributes.Add("Wrap", "False");
            tableCellWeightingValue.Attributes.Add("Wrap", "False");

            //TableRow tableRowEmpty = new TableRow();

            //for (int i = 0; i < 13; i++)
            //    tableRowEmpty.Cells.Add(new TableCell());

            // table.Rows.Add(tableRowEmpty);

            ExportTable(table, writer);

            writer.Position += 3;
        }

        private string RenderHierarchyString()
        {
            StringBuilder result = new StringBuilder();

            foreach (Guid idHierarchy in this.ReportDefinition.IdHierarchies)
            {
                result.Append(GetHierarchyLabel(idHierarchy));

                result.Append(Environment.NewLine);
            }

            return result.ToString().Trim();
        }

        private string GetHierarchyLabel(Guid idHierarchy)
        {
            string result = (string)this.ReportDefinition.Core.Hierarchies.GetValue(
                "Name",
                "Id",
                idHierarchy
            );

            object idParentHierarchy = this.ReportDefinition.Core.Hierarchies.GetValue(
                "IdHierarchy",
                "Id",
                idHierarchy
            );

            if (idParentHierarchy != null)
                result = GetHierarchyLabel((Guid)idParentHierarchy) + " -> " + result;

            return result;
        }

        private string RenderFilterString()
        {
            // Create a new string builder that
            // contains the result filter string.
            StringBuilder result = new StringBuilder();

            int i = 0;
            // Run through all workflow selections.
            foreach (WorkflowClasses.WorkflowSelection workflowSelection in this.ReportDefinition.Workflow.Selections.Values)
            {
                // Run through all workflow selectors of the workflow selection.
                foreach (WorkflowClasses.WorkflowSelectionSelector workflowSelector in workflowSelection.SelectionVariables.Values)
                {
                    result.Append("(");

                    // Run through all workflow selector items of the workflow selector.
                    foreach (WorkflowClasses.WorkflowSelectorItem item in workflowSelector.Selector.Items)
                    {
                        if (!workflowSelector.Selector.SelectedItems.Contains(item.Id))
                            continue;

                        result.Append(string.Format(
                            "[{0} - {1}] OR ",
                            (workflowSelection.Name.Contains("_varFilter") ? workflowSelection.Name.Replace("_varFilter", "") : workflowSelection.Name),
                            item.Text
                        ));
                    }

                    if (workflowSelector.Selector.SelectedItems.Count > 0)
                    {
                        result = result.Remove(result.Length - 4, 4);

                        result.Append(") AND ");
                    }
                    else
                    {
                        result = result.Remove(result.Length - 1, 1);
                    }

                    i++;
                }
            }

            // Run through all filter operators on root level.
            foreach (FilterCategoryOperator filterOperator in this.ReportDefinition.FilterCategories)
            {
                if (filterOperator.FilterCategories.Count == 0 && filterOperator.FilterCategoryOperators.Count == 0)
                {
                    if (result.Length > 4)
                        result = result.Remove(result.Length - 4, 4);
                    break;
                }

                result.Append("(");

                // Render the filter operator.
                result.Append(filterOperator.ToString());

                result.Append(")");
            }

            // Return the contents of the result's string builder.
            return result.ToString();
        }

        private void GetBorder(TableCell tableCell, ExcelWriter writer, int newColumnOffset, Color color)
        {
            if (tableCell.RowSpan > 1 || tableCell.ColumnSpan > 1)
            {
                if (tableCell.CssClass == null)
                {
                    if (tableCell.Attributes["class"] == null && tableCell.Attributes["style"] == null)
                        return;
                }
                int rowOffset2 = writer.Position;

                if (tableCell.RowSpan > 1)
                    rowOffset2 += tableCell.RowSpan - 1;

                int columnOffset2 = newColumnOffset;

                if (tableCell.ColumnSpan > 1)
                    columnOffset2 += tableCell.ColumnSpan - 1;

                writer.ActiveSheet.Cells[writer.Position, newColumnOffset, rowOffset2, columnOffset2].Borders.Color = SpreadsheetGear.Color.FromArgb(color.A, color.R, color.G, color.B);
                writer.ActiveSheet.Cells[writer.Position, newColumnOffset, rowOffset2, columnOffset2].Borders.Weight = SpreadsheetGear.BorderWeight.Thin;

            }
        }

        private string GetSigDiffType(int value,bool sigTest) {

            if (!sigTest)
                value = 0;


            switch (value)
            {
                case 0: return "No test selected ";
                    break;
                case 1:
                    return "IBM based T - Test";
                    break;
                case 2:
                    return "Dependent(Multi/Overlap) - Test";
                    break;
                case 3:
                    return "Independent - Test ";
                    break;
                         case 4:
                    return "Test against Total";
                    break;
                default: return "No test selected ";
                    break;
            }
        }

        private string GetSigweightedType(int value)
        {

            switch (value)
            {
                case 2:
                    return "weighted";
                    break;
                case 1:
                    return "unweighted";
                    break;
               
                default:
                    return "effective";
                    break;
            }
        }
        #endregion
    }
}
