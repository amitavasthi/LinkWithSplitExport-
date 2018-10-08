using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class TableauReportDefinitionRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the report definition
        /// of the report to render.
        /// </summary>
        public ReportDefinition ReportDefinition { get; set; }

        public bool RenderPercentage { get; set; }

        public Dictionary<string, XmlNode> DefinitionXmlNodeMapping { get; set; }

        #endregion


        #region Constructor

        public TableauReportDefinitionRenderer(ReportDefinition reportDefinition)
        {
            this.DefinitionXmlNodeMapping = new Dictionary<string, XmlNode>();
            this.ReportDefinition = reportDefinition;
        }

        #endregion


        #region Methods

        public string[] Render()
        {
            StringBuilder sbReturnData = new StringBuilder();

            StringBuilder sbFieldNames = new StringBuilder();
            StringBuilder sbFieldTypes = new StringBuilder();

            sbFieldNames.Append("[");
            sbFieldTypes.Append("[");

            foreach (ReportDefinitionVariable dimension in this.ReportDefinition.LeftVariables)
            {
                RenderDimension(
                    sbFieldNames,
                    sbFieldTypes,
                    dimension
                );
            }

            foreach (ReportDefinitionVariable measure in this.ReportDefinition.TopVariables)
            {
                RenderMeasure(
                    sbFieldNames,
                    sbFieldTypes,
                    measure
                );
            }

            if (this.ReportDefinition.LeftVariables.Count != 0)
            {
                sbFieldNames = sbFieldNames.Remove(sbFieldNames.Length - 1, 1);
                sbFieldTypes = sbFieldTypes.Remove(sbFieldTypes.Length - 1, 1);
            }

            sbFieldNames.Append("]");
            sbFieldTypes.Append("]");

            sbReturnData.Append("[");

            XmlNodeList xmlNodesCategories = this.ReportDefinition.XmlDocument.DocumentElement.SelectNodes("Results/Variable/*");

            foreach (XmlNode xmlNodeCategory in xmlNodesCategories)
            {
                if (xmlNodeCategory.Name == "Variable")
                    continue;

                string enabledStr = GetPropertyFromDefinition(xmlNodeCategory, "Enabled");

                if (!string.IsNullOrEmpty(enabledStr))
                {
                    if (!bool.Parse(enabledStr))
                        continue;
                }

                //sbReturnData.Append("[");

                RenderValue(
                    "[",
                    sbReturnData,
                    xmlNodeCategory
                );

                //sbReturnData = sbReturnData.Remove(sbReturnData.Length - 1, 1);
                //sbReturnData.Append("],");
            }

            if (xmlNodesCategories.Count != 0)
                sbReturnData = sbReturnData.Remove(sbReturnData.Length - 1, 1);

            sbReturnData.Append("]");

            return new string[]
            {
                sbFieldNames.ToString(),
                sbFieldTypes.ToString(),
                sbReturnData.ToString()
            };
        }

        public void RenderDimension(
            StringBuilder sbFieldNames,
            StringBuilder sbFieldTypes,
            ReportDefinitionVariable dimension
        )
        {
            sbFieldNames.Append("\"" + dimension.Label + "\",");
            sbFieldTypes.Append("\"string\",");

            foreach (ReportDefinitionVariable nestedDimension in dimension.NestedVariables)
            {
                RenderDimension(
                    sbFieldNames,
                    sbFieldTypes,
                    nestedDimension
                );
            }
        }

        public void RenderMeasure(
            StringBuilder sbFieldNames,
            StringBuilder sbFieldTypes,
            ReportDefinitionVariable measure
        )
        {
            foreach (ReportDefinitionScore category in measure.Scores)
            {
                if (category.Hidden)
                    continue;

                string categoryLabel = category.Label.Trim();

                if (categoryLabel == "")
                    categoryLabel = "Value";

                sbFieldNames.Append("\"" + categoryLabel + "\",");
                sbFieldTypes.Append("\"float\",");

                if (category.Variable.IsFake)
                {
                    this.RenderPercentage = true;

                    sbFieldNames.Append("\"Percentage\",");
                    sbFieldTypes.Append("\"float\",");
                }
            }
        }

        public void RenderValue(
            string prefix,
            StringBuilder result,
            XmlNode xmlNodeCategory
        )
        {
            string enabledStr = GetPropertyFromDefinition(xmlNodeCategory, "Enabled");

            if (!string.IsNullOrEmpty(enabledStr))
            {
                if (!bool.Parse(enabledStr))
                    return;
            }

            if (xmlNodeCategory.ParentNode.Attributes["Position"].Value == "Top")
            {
                double value = 0;
                if (xmlNodeCategory.Attributes["Value"] != null)
                    value = double.Parse(xmlNodeCategory.Attributes["Value"].Value);
                else
                    value = 0;

                if (this.RenderPercentage == false && this.ReportDefinition.Settings.PowerBIValues == Crosstables.Classes.PowerBIValues.Percentages)
                {
                    switch (this.ReportDefinition.Settings.PercentageBase)
                    {
                        case Crosstables.Classes.PercentageBase.Row:
                            value = value * 100 / double.Parse(xmlNodeCategory.Attributes["Base"].Value);
                            break;
                        case Crosstables.Classes.PercentageBase.Column:
                            value = value * 100 / double.Parse(xmlNodeCategory.ParentNode.Attributes["Base"].Value);
                            break;
                    }
                }

                if (double.IsNaN(value))
                    value = 0;

                //result.Append(prefix);
                result.Append(value);
                result.Append(",");

                if (this.RenderPercentage)
                {
                    /*switch (this.ReportDefinition.Settings.PercentageBase)
                    {
                        case Crosstables.Classes.PercentageBase.Row:
                            value = value * 100 / double.Parse(xmlNodeCategory.Attributes["Base"].Value);
                            break;
                        case Crosstables.Classes.PercentageBase.Column:
                            value = value * 100 / double.Parse(xmlNodeCategory.ParentNode.Attributes["Base"].Value);
                            break;
                    }*/
                    value = value * 100 / double.Parse(xmlNodeCategory.Attributes["Base"].Value);

                    if (double.IsNaN(value))
                        value = 0;

                    result.Append(value);
                    result.Append(",");
                }
            }
            else
            {
                string _prefix = prefix + "\"" + GetPropertyFromDefinition(xmlNodeCategory, "Label2057") + "\"" + ",";

                XmlNodeList xmlNodesCategories = xmlNodeCategory.SelectNodes("Variable[not(@IsNestedBase=\"True\")]/*");

                if (xmlNodesCategories.Count == 0)
                    return;

                if (xmlNodesCategories[0].ParentNode.Attributes["Position"].Value == "Top")
                {
                    result.Append(_prefix);
                }

                foreach (XmlNode xmlNodeNestedCategory in xmlNodesCategories)
                {
                    RenderValue(
                        _prefix,
                        result,
                        xmlNodeNestedCategory
                    );
                }
                if (xmlNodesCategories[0].ParentNode.Attributes["Position"].Value == "Top")
                {
                    result = result.Remove(result.Length - 1, 1);
                    result.Append("],");
                }
            }

        }

        private string GetPropertyFromDefinition(XmlNode xmlNode, string property)
        {
            if (!this.DefinitionXmlNodeMapping.ContainsKey(xmlNode.Attributes["Id"].Value))
            {
                string xPath = "//";

                if (xmlNode.Name != "Variable")
                {
                    if (xmlNode.ParentNode.Name == "Variable")
                    {
                        xPath += string.Format(
                            "{0}[@Id=\"{1}\" and @Position=\"{2}\"]/",
                            xmlNode.ParentNode.Name,
                            xmlNode.ParentNode.Attributes["Id"].Value,
                            xmlNode.ParentNode.Attributes["Position"].Value
                        );
                    }
                    else
                    {
                        xPath += string.Format(
                            "{0}[@Id=\"{1}\" and @Position=\"{2}\"]/",
                            xmlNode.ParentNode.ParentNode.Name,
                            xmlNode.ParentNode.ParentNode.Attributes["Id"].Value,
                            xmlNode.ParentNode.ParentNode.Attributes["Position"].Value
                        );
                    }
                }

                xPath += string.Format(
                    "{0}[@Id=\"{1}\"]",
                    xmlNode.Name,
                    xmlNode.Attributes["Id"].Value
                );

                // Needed cause of asynch tasks......
                try
                {
                    if (!this.DefinitionXmlNodeMapping.ContainsKey(xmlNode.Attributes["Id"].Value))
                        this.DefinitionXmlNodeMapping.Add(xmlNode.Attributes["Id"].Value, this.ReportDefinition.XmlDocument.SelectSingleNode(xPath));
                }
                catch
                { }
            }

            XmlNode xmlNodeDefinition = this.DefinitionXmlNodeMapping[xmlNode.Attributes["Id"].Value];

            if (xmlNodeDefinition == null || xmlNodeDefinition.Attributes[property] == null)
                return null;

            return xmlNodeDefinition.Attributes[property].Value;
        }

        #endregion
    }
}
