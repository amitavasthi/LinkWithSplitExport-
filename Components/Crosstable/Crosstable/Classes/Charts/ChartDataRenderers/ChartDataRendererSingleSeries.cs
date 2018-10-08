using Crosstables.Classes.HierarchyClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using VariableSelector1.Classes;

namespace Crosstables.Classes.Charts.ChartDataRenderers
{
    public class ChartDataRendererSingleSeries : ChartDataRenderer
    {
        #region Properties

        #endregion


        #region Constructor

        public ChartDataRendererSingleSeries(string source, string pathDimension)
            : base(source, pathDimension)
        { }

        #endregion


        #region Methods

        public override void Render(StringBuilder writer, HierarchyFilter hierarchyFilter)
        {
            string[] chartingColors = LoadChartingColors();
            int i = 0;
            // Select the xml node that contains the chart's data.
            XmlNodeList xmlNodes = this.Document.SelectNodes(this.PathDimension + "/*[not(self::Variable)][not(self::MeanScore)]");

            // Open an array for the values.
            writer.Append("[");

            // FOR TEST ONLY:
            foreach (XmlNode xmlNodeCategory in xmlNodes)
            {
                if (xmlNodeCategory.ParentNode.Attributes["IsNestedBase"] != null &&
                    bool.Parse(xmlNodeCategory.ParentNode.Attributes["IsNestedBase"].Value) == true)
                    continue;

                if (xmlNodeCategory.Attributes["ShowInChart"] != null && bool.Parse(xmlNodeCategory.Attributes["ShowInChart"].Value) == false)
                    continue;

                DefinitionObject test = new DefinitionObject(
                    base.Core,
                    this.Source,
                    string.Format(
                        "//Variable/{0}[@Id=\"{1}\"]",
                        xmlNodeCategory.Name,
                        xmlNodeCategory.Attributes["Id"].Value
                    )
                );

                object enabled = test.GetValue("Enabled");

                if (enabled != null && bool.Parse((string)enabled) == false)
                    continue;

                writer.Append("{");

                Guid idCategory = Guid.Parse(xmlNodeCategory.Attributes["Id"].Value);

                string categoryName = (string)test.GetValue("Name");
                string categoryLabel = (string)test.GetLabel(base.IdLanguage);

                XmlNode xmlNode = xmlNodeCategory.SelectSingleNode("Variable[@IsNestedBase=\"True\"]");

                if (xmlNode == null)
                    xmlNode = xmlNodeCategory.SelectSingleNode("Variable");

                if (xmlNode == null)
                    xmlNode = xmlNodeCategory;

                double value = 0.0;
                double baseValue = 0;
                if (xmlNode.Attributes["Value"] != null)
                {
                    if (xmlNode.Attributes["Value"] != null)
                        value = double.Parse(xmlNode.Attributes["Value"].Value);

                    if (xmlNode.ParentNode.Attributes["Position"].Value == "Top")
                    {
                        if (xmlNode.ParentNode.Attributes["Base"] != null)
                            baseValue = double.Parse(xmlNode.ParentNode.Attributes["Base"].Value);
                    }
                    else
                    {
                        if (xmlNode.Attributes["Base"] != null)
                            baseValue = double.Parse(xmlNode.Attributes["Base"].Value);
                    }
                }
                else
                {
                    if (xmlNode.Attributes["Base"] != null)
                        value = double.Parse(xmlNode.Attributes["Base"].Value);
                    if (xmlNode.Attributes["VariableBase"] != null)
                        baseValue = double.Parse(xmlNode.Attributes["VariableBase"].Value);
                    else if (xmlNodeCategory.ParentNode.Attributes["Base"] != null)
                        baseValue = double.Parse(xmlNodeCategory.ParentNode.Attributes["Base"].Value);
                }
                /*if (xmlNodeCategory.Attributes["Base"] != null)
                    value = double.Parse(xmlNodeCategory.Attributes["Base"].Value);
                else if (xmlNodeCategory.ParentNode.Attributes["Base"] != null)
                    value = double.Parse(xmlNodeCategory.ParentNode.Attributes["Base"].Value);

                if (xmlNode.Attributes["Value"] != null)
                {
                    baseValue = value;
                    value = double.Parse(xmlNode.Attributes["Value"].Value);
                }
                else
                {
                    if (xmlNodeCategory.ParentNode.Attributes["Base"] != null)
                        baseValue = double.Parse(xmlNodeCategory.ParentNode.Attributes["Base"].Value);
                }*/

                if (baseValue != 0)
                {
                    value = value * 100 / baseValue;
                }
                else
                {
                    value = 0;
                }

                if (chartingColors.Length == i)
                    i = 0;

                writer.Append(string.Format(
                    "\"dimension\": \"{0}\", \"value\": \"{1}\", \"Label\": \"{2}\",\"Color_{0}\": \"{3}\"",
                    categoryName,
                    value.ToString(new CultureInfo(2057)),
                    HttpUtility.HtmlEncode(categoryLabel),
                    chartingColors[i++]
                ));

                if (xmlNodeCategory.ChildNodes.Count > 0 && xmlNodeCategory.FirstChild.Attributes["Id"].Value != "00000000-0000-0000-0000-000000000000")
                {
                    writer.Append(string.Format(
                        ", \"XPATH\": \"{0}\"",
                        HttpUtility.UrlEncode(xmlNodeCategory.GetXPath(true) + "/Variable")
                    ));
                }

                writer.Append("},");
            }

            if (xmlNodes.Count > 0)
                writer = writer.Remove(writer.Length - 1, 1);

            // Close the values array.
            writer.Append("]");
        }

        private string[] LoadChartingColors()
        {
            return File.ReadAllText(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "ChartingColors.txt"
            )).Split(',').Select(x => x.Trim()).ToArray();
        }

        #endregion
    }
}
