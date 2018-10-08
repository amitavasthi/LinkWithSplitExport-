using Crosstables.Classes.HierarchyClasses;
using Crosstables.Classes.ReportDefinitionClasses;
using DataCore.Classes;
using DataCore.Classes.StorageMethods;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using VariableSelector1.Classes;

namespace Crosstables.Classes.Charts.ChartDataRenderers
{
    public class ChartDataRendererWords : ChartDataRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the text
        /// variable to build the word cloud for.
        /// </summary>
        public Guid IdVariable { get; set; }

        public Data Filter { get; set; }

        public string Source { get; set; }

        public DatabaseCore.Core Core { get; set; }

        #endregion


        #region Constructor

        public ChartDataRendererWords(Guid idVariable, string source, DatabaseCore.Core core)
        {
            this.IdVariable = idVariable;
            this.Source = source;
            this.Core = core;
        }

        #endregion


        #region Methods

        private void InitFilter(string source, DatabaseCore.Core core, HierarchyFilter hierarchyFilter)
        {
            ReportDefinitionClasses.ReportDefinition definition = new ReportDefinitionClasses.ReportDefinition(
                core,
                source,
                hierarchyFilter
            );

            DataCore.Classes.ReportCalculator test = new ReportCalculator(
                definition,
                core,
                HttpContext.Current.Session
            );

            this.Filter = test.GetFilter();
        }

        public override void Render(StringBuilder writer, HierarchyFilter hierarchyFilter)
        {
            InitFilter(
                this.Source, 
                this.Core, 
                hierarchyFilter
            );

            // Open an array for the values.
            writer.Append("[");

            // FOR TEST ONLY:
            string variableName = (string)base.Core.TaxonomyVariables.GetValue("Name", "Id", this.IdVariable);

            Database storageMethod = new Database(
                base.Core,
                null,
                1
            );

            List<string> categories = storageMethod.GetTextAnswers(this.IdVariable, true, null, true);

            List<Task> tasks = new List<Task>();

            Dictionary<string, double> categoryValues = new Dictionary<string, double>();

            Dictionary<string, object> ignoreTexts = new Dictionary<string, object>();
            Dictionary<string, object> ignoreCharacters = new Dictionary<string, object>();

            LoadTextAggregationDefinition(ignoreTexts, ignoreCharacters);

            int value;
            foreach (string category in categories)
            {
                string text = category;

                foreach (string ignoreCharacter in ignoreCharacters.Keys)
                {
                    text = text.Replace(ignoreCharacter, "");
                }

                text = text.ToLower().Trim();

                if (text.Length <= 1)
                    continue;

                if (int.TryParse(text, out value))
                    continue;

                if (ignoreTexts.ContainsKey(text))
                    continue;

                if (!categoryValues.ContainsKey(text))
                    categoryValues.Add(text, 0);
            }


            Guid[] idVariables;
            
            idVariables = this.Core.VariableLinks.GetValues(
                new string[] { "IdVariable" },
                new string[] { "IdTaxonomyVariable" },
                new object[] { this.IdVariable }
            ).Select(x => (Guid)x[0]).ToArray();

            Dictionary<Guid, List<string>> data = storageMethod.LoadTextAnswers(
                this.IdVariable,
                true,
                null,
                true
            );

            foreach (string category in categoryValues.Keys.ToList())
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    categoryValues[category] = GetTextCategoryCount(
                        category,
                        storageMethod,
                        idVariables,
                        data
                    );
                }));
            }

            Task.WaitAll(tasks.ToArray());

            StringBuilder test = new StringBuilder();
            foreach (string category in categoryValues.Keys.OrderByDescending(x => categoryValues[x]))
            {
                test.Append(category);
                test.Append("\t");
                test.Append(categoryValues[category]);
                test.Append(Environment.NewLine);
            }

            // System.IO.File.WriteAllText(@"E:\LiNKApplication\LinkLibraries\LinkManager\Clients\LinkOnline\LinkOnline\Fileadmin\Temp\WriteText.txt", test.ToString());

            foreach (string category in categoryValues.Keys)
            {
                if (categoryValues[category] == 0)
                    continue;

                writer.Append("{");

                string text = category;

                text = HttpUtility.HtmlEncode(text);


                writer.Append(string.Format(
                    "\"text\": \"{0}\", \"value\": {1}",
                    text,
                    categoryValues[category].ToString(new CultureInfo(2057))
                ));

                writer.Append("},");
            }

            if (categories.Count > 0)
                writer = writer.Remove(writer.Length - 1, 1);

            // Close the values array.
            writer.Append("]");
        }

        private void LoadTextAggregationDefinition(Dictionary<string, object> ignoreTexts, Dictionary<string, object> ignoreCharacters)
        {
            string fileName = System.IO.Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "TextAggregation.xml"
            );

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            XmlNodeList xmlNodesIgnoreWords = document.DocumentElement.SelectNodes("IgnoreWords/IgnoreWord");

            foreach (XmlNode xmlNode in xmlNodesIgnoreWords)
            {
                if (ignoreTexts.ContainsKey(xmlNode.InnerText))
                    continue;

                ignoreTexts.Add(xmlNode.InnerText, null);
            }

            XmlNodeList xmlNodesIgnoreCharacter = document.DocumentElement.SelectNodes("IgnoreCharacters/IgnoreCharacter");

            foreach (XmlNode xmlNode in xmlNodesIgnoreCharacter)
            {
                if (ignoreCharacters.ContainsKey(xmlNode.InnerText))
                    continue;

                ignoreCharacters.Add(xmlNode.InnerText.Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t"), null);
            }
        }

        private double GetTextCategoryCount(string category, Database storageMethod, Guid[] idVariables, Dictionary<Guid, List<string>> data)
        {
            double value = 0;

            foreach (Guid idRespondent in data.Keys)
            {
                for (int i = 0; i < data[idRespondent].Count; i++)
                {
                    if (category.Trim() == data[idRespondent][i].Trim())
                    {
                        value++;
                    }
                }
            }

            return value;

            /*
            double value = storageMethod.GetRespondentsText(
                category,
                this.IdVariable,
                true,
                idVariables,
                this.Filter
            ).Base;

            return value;*/
        }

        #endregion
    }
}
