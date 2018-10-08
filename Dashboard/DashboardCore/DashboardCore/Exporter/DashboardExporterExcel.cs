using ApplicationUtilities.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

namespace DashboardCore.Exporter
{
    public class DashboardExporterExcel : DashboardExporter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the color scheme split by css class names.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> ColorScheme { get; set; }

        #endregion


        #region Constructor

        public DashboardExporterExcel(Dashboard dashboard)
            : base(dashboard)
        {
            //base.Extension = ".xlsx";
            base.Extension = ".xls";
            base.MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        #endregion


        #region Methods

        public override string Export(string html, string fileName)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(html);

            ExcelWriter writer = new ExcelWriter();

            XmlNodeList xmlNodesTabs = document.DocumentElement.SelectNodes("//dashboardtab");

            XmlNodeList xmlNodesRows;
            XmlNodeList xmlNodesCells;
            int column = 0;
            int colSpan = 1;
            foreach (XmlNode xmlNodeTab in xmlNodesTabs)
            {
                writer.NewSheet(xmlNodeTab.Attributes["name"].Value);

                xmlNodesRows = xmlNodeTab.SelectNodes("table/tr");

                foreach (XmlNode xmlNodeRow in xmlNodesRows)
                {
                    column = 0;

                    xmlNodesCells = xmlNodeRow.SelectNodes("td");

                    foreach (XmlNode xmlNodeCell in xmlNodesCells)
                    {
                        colSpan = 1;

                        if (xmlNodeCell.Attributes["width"] != null)
                        {
                            writer.ActiveSheet.Cells[writer.Position, column].EntireColumn.ColumnWidth = (int.Parse(
                                xmlNodeCell.Attributes["width"].Value
                            ) / 10) + 2;
                        }

                        if (xmlNodeCell.Attributes["freeze"] != null)
                        {
                            if (bool.Parse(xmlNodeCell.Attributes["freeze"].Value))
                            {
                                writer.ActiveSheet.Cells[writer.Position, column].Select();
                                writer.ActiveSheet.WindowInfo.FreezePanes = true;
                            }
                        }

                        if (xmlNodeCell.Attributes["border"] != null)
                        {
                            writer.ActiveSheet.Cells[writer.Position, column].Borders.Color = GenerateRGB(
                                xmlNodeCell.Attributes["border"].Value
                            );
                            writer.ActiveSheet.Cells[writer.Position, column].Borders.LineStyle = SpreadsheetGear.LineStyle.Continous;
                            writer.ActiveSheet.Cells[writer.Position, column].Borders.Weight = SpreadsheetGear.BorderWeight.Thin;
                        }

                        writer.ActiveSheet.Cells[writer.Position, column].WrapText = true;
                        writer.ActiveSheet.Cells[writer.Position, column].VerticalAlignment = SpreadsheetGear.VAlign.Center;

                        if (xmlNodeCell.Attributes["fontBold"] != null)
                        {
                            if (bool.Parse(xmlNodeCell.Attributes["fontBold"].Value))
                                writer.ActiveSheet.Cells[writer.Position, column].Font.Bold = true;
                        }

                        if (xmlNodeCell.Attributes["fontUnderline"] != null)
                        {
                            if (bool.Parse(xmlNodeCell.Attributes["fontUnderline"].Value))
                                writer.ActiveSheet.Cells[writer.Position, column].Font.Underline = SpreadsheetGear.UnderlineStyle.Single;
                        }

                        if (xmlNodeCell.Attributes["colspan"] != null)
                            colSpan = int.Parse(xmlNodeCell.Attributes["colspan"].Value);

                        if (colSpan != 1)
                        {
                            writer.ActiveSheet.Cells[
                                writer.Position,
                                column,
                                writer.Position,
                                column + (colSpan - 1)
                            ].Merge();
                        }

                        if (xmlNodeCell.Attributes["align"].Value == "center")
                            writer.ActiveSheet.Cells[writer.Position, column].HorizontalAlignment = SpreadsheetGear.HAlign.Center;

                        writer.ActiveSheet.Cells[writer.Position, column].Interior.Color = GenerateRGB(xmlNodeCell.Attributes["background"].Value);
                        writer.ActiveSheet.Cells[writer.Position, column].Font.Color = GenerateRGB(xmlNodeCell.Attributes["color"].Value);

                        if (xmlNodeCell.Attributes["Format"] != null)
                        {
                            writer.ActiveSheet.Cells[writer.Position, column].NumberFormat = xmlNodeCell.Attributes["Format"].Value;
                        }
                        else
                        {
                            if (!xmlNodeCell.InnerXml.Trim().EndsWith("%"))
                            {
                                writer.ActiveSheet.Cells[writer.Position, column].NumberFormat = "0.0";
                            }
                            else
                            {
                                string format = "0";

                                if (xmlNodeCell.Attributes["decimals"] != null)
                                {
                                    format += ".";
                                    for (int i = 0; i < int.Parse(xmlNodeCell.Attributes["decimals"].Value); i++)
                                    {
                                        format += "0";
                                    }
                                }

                                format += "%";
                                writer.ActiveSheet.Cells[writer.Position, column].NumberFormat = format;
                            }
                        }

                        writer.Write(column, xmlNodeCell.InnerXml);

                        column += colSpan;
                    }

                    writer.NewLine();
                }
            }

            writer.Workbook.Worksheets[0].Delete();

            //writer.Save(fileName);
            writer.Workbook.SaveAs(fileName, SpreadsheetGear.FileFormat.Excel8);
            /*writer = new ExcelWriter(fileName);
            writer.Save(fileName);*/

            return fileName;
            
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            var f = Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(
                fileName,
                0,
                true,
                Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8,
                "",
                "",
                true,
                Microsoft.Office.Interop.Excel.XlPlatform.xlWindows,
                "\t",
                false,
                false,
                0,
                true,
                1,
                0
            );

            xlWorkBook.SaveAs(fileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);

            return fileName;
        }
        //public string _Export(string html, string fileName)
        //{
        //    fileName = fileName.Replace(".xlsx", ".xls");
        //    Stopwatch stopwatch = new Stopwatch();

        //    //string result = System.IO.Path.GetTempFileName() + ".xls";
        //    /*string fileName = Path.Combine(
        //        //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
        //        HttpContext.Current.Request.PhysicalApplicationPath,
        //        "Temp",
        //        Guid.NewGuid() + ".xls"
        //    );*/

        //    if (!Directory.Exists(Path.GetDirectoryName(fileName)))
        //        Directory.CreateDirectory(Path.GetDirectoryName(fileName));

        //    //System.IO.File.WriteAllText(result, this.Dashboard.Render());

        //    Dictionary<string, string> tabs = new Dictionary<string, string>();

        //    if (html.Contains("<dashboardtab"))
        //    {
        //        string header = html.Split(new string[]
        //        {
        //            "<head>"
        //        }, StringSplitOptions.None)[1].Split(new string[]
        //        {
        //            "</head>"
        //        }, StringSplitOptions.None)[0];

        //        string[] split = html.Split(new string[]
        //        {
        //            "<dashboardtab"
        //        }, StringSplitOptions.None);

        //        foreach (string s in split)
        //        {
        //            if (!s.Contains("</dashboardtab>"))
        //                continue;

        //            int index = s.IndexOf('>');
        //            string name = s.Split(new string[]
        //            {
        //                "name=\""
        //            }, StringSplitOptions.None)[1].Split('"')[0];

        //            tabs.Add(name, "<html><head>" + header + "</head><body>" + s.Substring(index + 1, s.Length - (index + 1)).Split(new string[]
        //            {
        //                "</dashboardtab>"
        //            }, StringSplitOptions.None)[0] + "</body></html>");
        //        }
        //    }
        //    else
        //    {
        //        tabs.Add("Sheet1", html);
        //    }

        //    ExcelWriter writer = new ExcelWriter();

        //    for (int i = 0; i < tabs.Count; i++)
        //    {
        //        string name = tabs.ElementAt(i).Key;
        //        string tab = tabs.ElementAt(i).Value;

        //        if (i == 0)
        //        {
        //            if (name.Length > 31)
        //                name = name.Substring(0, 31);

        //            writer.ActiveSheet.Name = name;
        //        }
        //        else
        //        {
        //            writer.NewSheet(name);
        //        }

        //        //System.IO.File.WriteAllText(fileName, tab);

        //        string fName = "";

        //        if (!name.StartsWith("Counts"))
        //        {
        //            fName = name + " Avg Meter Share 01-15-2017";
        //        }

        //        System.IO.File.WriteAllText(Path.Combine(
        //            HttpContext.Current.Request.PhysicalApplicationPath,
        //            "Fileadmin",
        //            "Temp",
        //            fName + ".xls"
        //        ), tab);

        //        continue;

        //        Microsoft.Office.Interop.Excel.Application xlApp;
        //        Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
        //        Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
        //        object misValue = System.Reflection.Missing.Value;



        //        xlApp = new Microsoft.Office.Interop.Excel.Application();
        //        xlWorkBook = xlApp.Workbooks.Open(
        //            fileName,
        //            0,
        //            true,
        //            5,
        //            "",
        //            "",
        //            true,
        //            Microsoft.Office.Interop.Excel.XlPlatform.xlWindows,
        //            "\t",
        //            false,
        //            false,
        //            0,
        //            true,
        //            1,
        //            0
        //        );

        //        xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

        //        int rows = xlWorkSheet.UsedRange.Rows.Count;
        //        int columns = xlWorkSheet.UsedRange.Columns.Count;

        //        Microsoft.Office.Interop.Excel.Range cell;
        //        SpreadsheetGear.Color color;
        //        for (int row = 0; row < rows; row++)
        //        {
        //            for (int column = 0; column < columns; column++)
        //            {
        //                cell = xlWorkSheet.UsedRange.Cells[row + 1, column + 1];
        //                color = GenerateRGB((int)cell.Interior.Color);

        //                //writer.ActiveSheet.Cells[row - 1, column].MergeArea = cell.MergeArea;
        //                Microsoft.Office.Interop.Excel.Range merge = cell.MergeArea;

        //                //writer.ActiveSheet.Cells[row, column].HorizontalAlignment = cell.HorizontalAlignment;
        //                //writer.ActiveSheet.Cells[row, column].VerticalAlignment = cell.VerticalAlignment;

        //                Microsoft.Office.Interop.Excel.XlHAlign textAlign = (Microsoft.Office.Interop.Excel.XlHAlign)cell.HorizontalAlignment;

        //                writer.ActiveSheet.Cells[row, column].HorizontalAlignment = (SpreadsheetGear.HAlign)Enum.Parse(
        //                    typeof(SpreadsheetGear.HAlign),
        //                    textAlign.ToString().Replace("xlHAlign", "")
        //                );

        //                if (cell.MergeCells)
        //                    writer.ActiveSheet.Cells[cell.MergeArea.Address].Merge();

        //                if (color.A != 255 || color.G != 255 || color.B != 255)
        //                    writer.ActiveSheet.Cells[row, column].Interior.Color = color;

        //                try
        //                {
        //                    writer.ActiveSheet.Cells[row, column].Font.Color = GenerateRGB((int)cell.Font.Color);
        //                }
        //                catch { }

        //                writer.Write(column, cell.Text);
        //            }
        //            writer.NewLine();
        //        }

        //        for (int column = 0; column < columns; column++)
        //        {
        //            try
        //            {
        //                writer.ActiveSheet.Cells[0, column].EntireColumn.AutoFit();
        //                writer.ActiveSheet.Cells[0, column].EntireColumn.ColumnWidth += 2;
        //            }
        //            catch { }
        //        }

        //        for (int row = 0; row < rows; row++)
        //        {
        //            writer.ActiveSheet.Cells[row, 0].EntireRow.RowHeight += 2;
        //        }

        //        //xlWorkBook.Close(true, misValue, misValue);
        //        xlApp.Quit();
        //    }

        //    return "";

        //    //string result2 = System.IO.Path.GetTempFileName() + ".xlsx";
        //    /*string result2 = Path.Combine(
        //        //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
        //        HttpContext.Current.Request.PhysicalApplicationPath,
        //        "Temp",
        //        Guid.NewGuid() + ".xls"
        //    );*/
        //    string result2 = Path.Combine(
        //        Path.GetDirectoryName(fileName),
        //        Guid.NewGuid() + ".xlsx"
        //    );
        //    writer.Save(result2);

        //    System.IO.File.Delete(fileName);

        //    stopwatch.Stop();

        //    this.Dashboard.Document.DocumentElement.AddAttribute(
        //        "ElapsedExporting",
        //        stopwatch.ElapsedMilliseconds
        //    );

        //    return result2;

        //    /*string result = System.IO.Path.GetTempFileName() + ".xlsx";
        //    System.IO.File.WriteAllText(result, this.Dashboard.Render());

        //    // Open the source workbook file.
        //    SpreadsheetGear.IWorkbook test = SpreadsheetGear.Factory.GetWorkbook(result);

        //    string result2 = System.IO.Path.GetTempFileName() + ".xlsx";

        //    test.SaveAs(result2, SpreadsheetGear.FileFormat.OpenXMLWorkbook);

        //    System.IO.File.Delete(result);

        //    return result2;*/

        //    /*StringBuilder style = new StringBuilder();

        //    foreach (string stylesheet in this.Dashboard.Settings.Includes)
        //    {
        //        style.Append(stylesheet);
        //    }

        //    this.ColorScheme = new Dictionary<string, Dictionary<string, string>>();

        //    string result = "";

        //    Panel table = ConvertHtmlTableStringToTable(this.Dashboard.Render());

        //    ExcelWriter writer = new ExcelWriter();

        //    ExportTable((Table)table.Controls[0], writer);

        //    result = System.IO.Path.GetTempFileName() + ".xlsx";

        //    writer.Save(result);

        //    return result;*/
        //}

        private SpreadsheetGear.Color GenerateRGB(int value)
        {
            SpreadsheetGear.Color color;

            Color c = ColorTranslator.FromOle(value);

            color = SpreadsheetGear.Color.FromArgb(c.R, c.G, c.B);

            return color;
        }
        private SpreadsheetGear.Color GenerateRGB(string value)
        {
            try
            {
                SpreadsheetGear.Color color;

                string[] split = value.Split('(')[1].Split(')')[0].Split(',');

                if (split.Length < 3)
                    return SpreadsheetGear.Color.FromArgb(0, 0, 0);

                if (split.Length == 4)
                {
                    color = SpreadsheetGear.Color.FromArgb(
                        int.Parse(split[0].Trim()),
                        int.Parse(split[1].Trim()),
                        int.Parse(split[2].Trim()),
                        int.Parse(split[3].Trim())
                    );
                }
                else
                {
                    color = SpreadsheetGear.Color.FromArgb(
                        int.Parse(split[0].Trim()),
                        int.Parse(split[1].Trim()),
                        int.Parse(split[2].Trim())
                    );
                }

                return color;
            }
            catch
            {
                return SpreadsheetGear.Color.FromArgb(0, 0, 0);
            }
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
                string format = "0";

                format += " \"%\"";

                //writer.ActiveSheet.Cells[writer.Position, columnOffset].NumberFormat = "0 \"%\"";
                writer.ActiveSheet.Cells[writer.Position, columnOffset].NumberFormat = format;

                if (html.StartsWith("'"))
                    html = html.Remove(0, 1);

                html = html.Replace("%", "");
                html = html.Trim();
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


        /// <summary>
        /// Convert HSV to RGB
        /// h is from 0-360
        /// s,v values are 0-1
        /// r,g,b values are 0-255
        /// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
        /// </summary>
        void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
        {
            // ######################################################################
            // T. Nathan Mundhenk
            // mundhenk@usc.edu
            // C/C++ Macro HSV to RGB

            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        #endregion
    }
}
