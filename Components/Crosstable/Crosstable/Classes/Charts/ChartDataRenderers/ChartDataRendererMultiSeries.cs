using Crosstables.Classes.HierarchyClasses;
using Crosstables.Classes.ReportDefinitionClasses;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using VariableSelector1.Classes;

namespace Crosstables.Classes.Charts.ChartDataRenderers
{
    public class ChartDataRendererMultiSeries : ChartDataRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the xPath to the measure to render.
        /// </summary>
        public string PathMeasure { get; set; }

        #endregion


        #region Constructor

        public ChartDataRendererMultiSeries(string source, string pathDimension, string pathMeasure)
            : base(source, pathDimension)
        {
            this.PathMeasure = pathMeasure;
        }

        #endregion


        #region Methods

        public override void Render(StringBuilder writer, HierarchyFilter hierarchyFilter)
        {
            string[] chartingColors = LoadChartingColors();
            //Random rnd = new Random();
            //List<string> usedColors = new List<string>();
            int i = 0;

            // Select the xml node that contains the chart's data.
            XmlNodeList xmlNodes = this.Document.SelectNodes(this.PathDimension + "/*[not(self::Variable)][not(self::MeanScore)]");


            // Open an array for the values.
            writer.Append("[");

            foreach (XmlNode xmlNodeCategory in xmlNodes)
            {

                if (xmlNodeCategory.Attributes["ShowInChart"] != null && bool.Parse(xmlNodeCategory.Attributes["ShowInChart"].Value) == false)
                    continue;

                // if (xmlNodeCategory.Name == "MeanScore") continue;
                if (xmlNodeCategory.ParentNode.Attributes["IsNestedBase"] != null && bool.Parse(xmlNodeCategory.ParentNode.Attributes["IsNestedBase"].Value) == true)
                    continue;

                DefinitionObject variable = new DefinitionObject(
                    base.Core,
                    this.Source,
                    string.Format(
                        "//Variable[@Id=\"{0}\"]",
                        xmlNodeCategory.ParentNode.Attributes["Id"].Value
                    )
                );
                DefinitionObject category = new DefinitionObject(
                    base.Core,
                    this.Source,
                    string.Format(
                        "//Variable[@Id=\"{2}\"]/{0}[@Id=\"{1}\"]",
                        xmlNodeCategory.Name,
                        xmlNodeCategory.Attributes["Id"].Value,
                        xmlNodeCategory.ParentNode.Attributes["Id"].Value
                    )
                );

                object enabled = category.GetValue("Enabled");
                object _hasValue = category.GetValue("HasValues");

                ReportDefinition reportDefinition = new ReportDefinition(
                    base.Core,
                    this.Source,
                    hierarchyFilter
                );

                //if (enabled != null && bool.Parse((string)enabled) == false) 
                //    continue;
                bool hasValue = true;

                bool.TryParse(_hasValue != null ? _hasValue.ToString() : "true", out hasValue);

                if ((enabled != null && bool.Parse((string)enabled) == false) || (hasValue == false && reportDefinition.Settings.HideEmptyRowsAndColumns))
                    continue;

                string variableName = (string)variable.GetValue("Name");
                string categoryName = (string)category.GetValue("Name");
                string categoryLabel = (string)category.GetLabel(base.IdLanguage);
                if (String.IsNullOrEmpty(categoryName))
                {
                    categoryName = categoryLabel;
                }
                writer.Append("{");

                writer.Append(string.Format(
                    "\"dimension\": \"{0}\", \"Label\": \"{1}\",",
                    variableName + "_" + categoryName,
                    categoryLabel
                ));

                //XmlNodeList xmlNodesMeasures = xmlNodeCategory.SelectNodes("Variable/TaxonomyCategory");
                XmlNodeList xmlNodesMeasures = xmlNodeCategory.SelectNodes(this.PathMeasure + "/*[not(self::Variable)]");
                
                foreach (XmlNode measure in xmlNodesMeasures)
                {

                    if (measure.Attributes["ShowInChart"] != null && bool.Parse(measure.Attributes["ShowInChart"].Value) == false)
                        continue;

                    if (measure.ParentNode.Attributes["IsNestedBase"] != null &&
                        bool.Parse(measure.ParentNode.Attributes["IsNestedBase"].Value) == true)
                        continue;

                    //usedColors.Add(measure.Attributes["Color"].Value);

                    DefinitionObject category2 = new DefinitionObject(
                        base.Core,
                        this.Source,
                        //xmlNodeCategory.GetXPath(true)
                        string.Format(
                            "//{2}[@Id=\"{3}\"]/{0}[@Id=\"{1}\"]",
                            measure.Name,
                            measure.Attributes["Id"].Value,
                            measure.ParentNode.Name,
                            measure.ParentNode.Attributes["Id"].Value
                        )
                    );

                    double value = 0.0;
                    double baseValue = 0;

                    // Nested on the left.
                    if ((category2.XmlNode.ParentNode.Name != "ScoreGroup" && category2.XmlNode.ParentNode.Attributes["Position"] != null && category2.XmlNode.ParentNode.Attributes["Position"].Value == "Left")
                        || (category2.XmlNode.ParentNode.Name == "ScoreGroup" && category2.XmlNode.ParentNode.ParentNode.Attributes["Position"] != null && category2.XmlNode.ParentNode.ParentNode.Attributes["Position"].Value == "Left"))
                    {
                        value = double.Parse(measure.ParentNode.ParentNode.Attributes["Base"].Value);
                        baseValue = double.Parse(measure.ParentNode.ParentNode.ParentNode.Attributes["Base"].Value);

                        writer.Append(string.Format(
                            "\"_{0}\": \"{1}\",",
                            "",
                            (value * 100 / baseValue).ToString(new CultureInfo(2057))
                        ));

                        if (measure.ChildNodes.Count > 0)
                        {
                            writer.Append(string.Format(
                                "\"XPATH_{0}\": \"{1}\", \"IsDimensionPath_{0}\": \"true\", \"Color_{0}\": \"{2}\"",
                                "",
                                HttpUtility.UrlEncode(measure.ParentNode.ParentNode.GetXPath(true) + "/Variable"),
                                measure.Attributes["Color"].Value
                            /*HttpUtility.UrlEncode(string.Format(
                                "Variable[@Id='{0}']/{2}[@Id='{1}']/Variable",
                                measure.ParentNode.Attributes["Id"].Value,
                                measure.Attributes["Id"].Value,
                                measure.Name
                            ))*/
                            ));
                        }

                        break;
                    }

                    enabled = category2.GetValue("Enabled");
                    bool _enabled;
                    _hasValue = category2.GetValue("HasValues", false);

                    hasValue = true;

                    bool.TryParse(_hasValue != null ? _hasValue.ToString() : "true", out hasValue);

                    if (enabled != null && bool.TryParse(enabled.ToString(), out _enabled))
                    {
                        if ((_enabled == false) || (hasValue == false && reportDefinition.Settings.HideEmptyRowsAndColumns))
                            continue;
                    }

                    //if (enabled != null && bool.TryParse(enabled.ToString(), out _enabled))
                    //{
                    //    if (_enabled == false)
                    //        continue;
                    //}
                    
                    string measureLabel = new DefinitionObject(
                        this.Core, 
                        category2.Source, 
                        category2.ParentPath
                    ).GetLabel(base.IdLanguage) +
                        "###SPLIT###" + category2.GetLabel(base.IdLanguage);

                    if (category2.TypeName != "NumericValue")
                    {
                        if (measure.Attributes["Base"] != null)
                            value = double.Parse(measure.Attributes["Base"].Value);
                        else if (measure.ParentNode.Attributes["Base"] != null)
                            value = double.Parse(measure.ParentNode.Attributes["Base"].Value);

                        if (measure.Attributes["Value"] != null)
                        {
                            baseValue = value;
                            value = double.Parse(measure.Attributes["Value"].Value);
                        }
                        else if (measure.ChildNodes.Count > 0)
                        {
                            if (measure.ChildNodes[0].Attributes["Base"] != null)
                                baseValue = double.Parse(measure.ChildNodes[0].Attributes["Base"].Value);
                        }
                        bool isFake = Guid.Parse(measure.Attributes["Id"].Value) == new Guid();

                        if (isFake)
                            baseValue = double.Parse(measure.ParentNode.Attributes["VariableBase"].Value);
                    }
                    else
                    {
                        value = double.Parse(measure.ParentNode.ParentNode.Attributes["Base"].Value);
                        baseValue = double.Parse(measure.ParentNode.ParentNode.ParentNode.Attributes["Base"].Value);
                    }

                    if (chartingColors.Length == i)
                        i = 0;

                    writer.Append(string.Format(
                        "\"_{0}\": \"{1}\", \"Color_{0}\": \"{2}\",",
                         measureLabel,
                        (value * 100 / baseValue).ToString(new CultureInfo(2057)),
                        chartingColors[i++]//measure.Attributes["Color"].Value
                    ));

                    if (measure.ChildNodes.Count > 0)
                    {
                        writer.Append(string.Format(
                            "\"XPATH_{0}\": \"{1}\",",
                            measureLabel,
                            HttpUtility.UrlEncode(string.Format(
                                "Variable[@Id='{0}']/{2}[@Id='{1}']/Variable",
                                measure.ParentNode.Attributes["Id"].Value,
                                measure.Attributes["Id"].Value,
                                measure.Name
                            ))
                        ));
                    }
                }

                writer = writer.Remove(writer.Length - 1, 1);

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
