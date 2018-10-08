using ApplicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class ReportDefinitionRendererJSON
    {
        #region Properties

        /// <summary>
        /// Gets or sets the report definition to render.
        /// </summary>
        public ReportDefinition ReportDefinition { get; set; }

        private List<ReportDefinitionRenderJSONCombination> Combinations { get; set; }

        #endregion


        #region Constructor

        public ReportDefinitionRendererJSON(ReportDefinition reportDefinition)
        {
            this.ReportDefinition = reportDefinition;
            this.Combinations = new List<ReportDefinitionRenderJSONCombination>();
        }

        #endregion


        #region Methods

        public string Render()
        {
            // Create a new string builder that contains the result json string.
            StringBuilder result = new StringBuilder();

            foreach (ReportDefinitionVariable variable in this.ReportDefinition.LeftVariables)
            {
                LoadCombinations(variable, new List<string>(), "Report/Results");
            }

            TaskCollection tasks = new TaskCollection();

            foreach (ReportDefinitionRenderJSONCombination combination in this.Combinations)
            {
                tasks.Add(() => combination.LoadValue(this.ReportDefinition.XmlDocument));
            }

            tasks.WaitAll();

            result.Append("[");

            foreach (ReportDefinitionRenderJSONCombination combination in this.Combinations)
            {
                result.Append(combination.ToString());
                result.Append(",");
            }

            if (this.Combinations.Count != 0)
                result = result.Remove(result.Length - 1, 1);

            result.Append("]");

            // Return the contents of the
            // result string builder.
            return result.ToString();
        }

        private void LoadCombinations(ReportDefinitionVariable variable, List<string> filters, string path)
        {
            List<ReportDefinitionVariable> next = variable.NestedVariables;

            if(next.Count == 0 && variable.Position == "Left")
            {
                next = this.ReportDefinition.TopVariables;
            }

            if (next.Count > 0)
            {
                foreach (ReportDefinitionScore category in variable.Scores)
                {
                    if (category.Hidden)
                        continue;

                    List<string> _filters = new List<string>();
                    _filters.AddRange(filters);
                    _filters.Add(category.Label);

                    string _path = path + string.Format(
                        "/Variable[@Id=\"{0}\"]/*[@Id=\"{1}\"]",
                        variable.IdVariable,
                        category.Identity
                    );

                    foreach (ReportDefinitionVariable nestedVariable in next)
                    {
                        LoadCombinations(nestedVariable, _filters, _path);
                    }
                }
            }
            else
            {
                foreach (ReportDefinitionScore category in variable.Scores)
                {
                    if (category.Hidden)
                        continue;

                    ReportDefinitionRenderJSONCombination combination = new ReportDefinitionRenderJSONCombination();
                    combination.Filters = filters;
                    combination.Label = category.Label;
                    combination.Path = path + string.Format(
                        "/Variable[@Id=\"{0}\"]/*[@Id=\"{1}\"]",
                        variable.IdVariable,
                        category.Identity
                    );

                    this.Combinations.Add(combination);
                }
            }
        }

        #endregion
    }

    public class ReportDefinitionRenderJSONCombination
    {
        #region Properties

        public List<string> Filters { get; set; }

        public string Label { get; set; }

        public string Path { get; set; }

        public double Value { get; set; }

        public double Base { get; set; }

        #endregion


        #region Constructor

        public ReportDefinitionRenderJSONCombination()
        {
            this.Filters = new List<string>();
        }

        #endregion


        #region Methods

        public void LoadValue(XmlDocument document)
        {
            XmlNode xmlNode = document.SelectSingleNode(this.Path);

            if (xmlNode == null)
                return;

            this.Value = double.Parse(xmlNode.Attributes["Value"].Value);
            this.Base = double.Parse(xmlNode.Attributes["Base"].Value);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.Append("{");

            for (int i = 0; i < this.Filters.Count; i++)
            {
                result.Append(string.Format(
                    "\"Filter{0}\": \"{1}\",",
                    i + 1,
                    this.Filters[i]
                ));
            }

            result.Append(string.Format(
                "\"Label\": \"{0}\",\"Value\": \"{1}\",\"Base\": \"{2}\"",
                this.Label,
                this.Value,
                this.Base
            ));

            result.Append("}");

            return result.ToString();
        }

        #endregion
    }
}
