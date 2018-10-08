using ApplicationUtilities;
using DatabaseCore.Items;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace LinkBi1.Classes.Interfaces
{
    public class JSON : LinkBiInterface
    {
        #region Properties

        public DataCore.Classes.StorageMethods.Database StorageMethod { get; set; }

        public TimeSpan DataAggregationTime { get; set; }

        #endregion


        #region Constructor

        public JSON(DatabaseCore.Core core)
            : base(core)
        {
            this.StorageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Core,
                null
            );
        }

        public JSON(DatabaseCore.Core core, LinkBiDefinition definition)
            : base(core, definition)
        {
            this.StorageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Core,
                null
            );
        }

        #endregion


        #region Methods

        public override void Read(string source)
        {
            throw new NotImplementedException();
        }

        int totalToProcess = 0;
        public override string Render()
        {
            GetTotalToProcessCount();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Create a new string builder to build the result json script.
            StringBuilder writer = new StringBuilder();

            Data filter = base.InitFilter();

            this.Definition.WeightingFilters.LoadRespondents(filter);

            List<Task> tasks = new List<Task>();

            foreach (LinkBiDefinitionDimension dimension in this.Definition.Dimensions)
            {
                if (dimension.VariableType == VariableType.Numeric)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        this.StorageMethod.InitDataNumeric(
                            dimension.Identity,
                            dimension.IsTaxonomy,
                            this.StorageMethod.Core.CaseDataLocation
                        );
                    }));
                }
                else
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        this.StorageMethod.InitData(
                            dimension.Identity,
                            dimension.IsTaxonomy,
                            this.StorageMethod.Core.CaseDataLocation
                        );
                    }));
                }
            }
            foreach (LinkBiDefinitionDimension measure in this.Definition.Measures)
            {
                if (measure.VariableType == VariableType.Numeric)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        this.StorageMethod.InitDataNumeric(
                            measure.Identity,
                            measure.IsTaxonomy,
                            this.StorageMethod.Core.CaseDataLocation
                        );
                    }));
                }
                else
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        this.StorageMethod.InitData(
                            measure.Identity,
                            measure.IsTaxonomy,
                            this.StorageMethod.Core.CaseDataLocation
                        );
                    }));
                }
            }

            Task.WaitAll(tasks.ToArray());

            // Check if there are filter defined.
            if (this.Definition.Dimensions.Count > 0)
            {
                // Render the first defined filter.
                RenderDimensions(
                    this.Definition.Dimensions[0],
                    "",
                    writer,
                    0,
                    filter
                );
            }

            writer.RemoveLastComma();

            stopwatch.Stop();

            this.DataAggregationTime = stopwatch.Elapsed;

            base.Progress = 100;

            string result = string.Format(
                "[{0}]",
                writer.ToString()
            );

            writer.Clear();

            string fileName = Path.GetTempFileName() + ".txt";
            File.WriteAllText(fileName, result);
            base.Finalize(fileName);
            File.Delete(fileName);

            return result;
        }


        private void GetTotalToProcessCount()
        {
            if (this.Definition.Dimensions.Count == 0)
                return;

            totalToProcess = this.Definition.Dimensions[0].Scores.Count;

            for (int a = 1; a < this.Definition.Dimensions.Count; a++)
            {
                totalToProcess *= this.Definition.Dimensions[a].Scores.Count;
            }

            foreach (LinkBiDefinitionDimension measure in this.Definition.Measures)
            {
                totalToProcess *= measure.Scores.Count;
            }
        }

        int i = 1;
        private void RenderDimensions(
            LinkBiDefinitionDimension dimension,
            string _selection,
            StringBuilder writer,
            int offset,
            Data dataFilter = null
        )
        {
            Data scoreFilter;

            List<Task> tasks = new List<Task>();

            if (dimension.VariableType == VariableType.Numeric)
            {
                scoreFilter = this.StorageMethod.GetRespondentsNumeric(
                    dimension.Identity,
                    dimension.IsTaxonomy,
                    this.Core.CaseDataLocation,
                    dataFilter,
                    this.Definition.WeightingFilters
                );

                string label = "";

                if (dimension.XmlNode.Attributes["Label" + this.Definition.Settings.IdLanguage] == null)
                {
                    dimension.XmlNode.AddAttribute("Label" + this.Definition.Settings.IdLanguage, dimension.Label);
                }

                label = dimension.XmlNode.Attributes["Label" + this.Definition.Settings.IdLanguage].Value;

                string selection = _selection + string.Format(
                    "\"Filter{0}\": \"{1}\",",
                    offset + 1,
                    label
                );

                writer.Append("{");
                writer.Append(selection);

                writer.Append(string.Format(
                    "\"Label\": \"{0}\",",
                    label
                ));

                writer.Append(string.Format(
                    "\"Base\": \"{0}\",",
                    scoreFilter.Base.ToString(new CultureInfo("en-GB"))
                ));

                writer.Append(string.Format(
                    "\"Value\": \"{0}\"",
                    scoreFilter.Value.ToString(new CultureInfo("en-GB"))
                ));
                writer.Append("},");
            }
            else
            {
                // Run through all filter scores of the filter.
                foreach (LinkBiDefinitionDimensionScore score in dimension.Scores)
                {
                    string label = "";

                    if (score.XmlNode.Attributes["Label" + this.Definition.Settings.IdLanguage] == null)
                    {
                        score.XmlNode.AddAttribute("Label" + this.Definition.Settings.IdLanguage, score.Label);
                    }

                    label = score.XmlNode.Attributes["Label" + this.Definition.Settings.IdLanguage].Value;

                    /*tasks.Add(Task.Factory.StartNew(() => */
                    {
                        string selection = _selection + string.Format(
                            "\"Filter{0}\": \"{1}\",",
                            offset + 1,
                            label
                        );

                        if (dimension.IsTaxonomy && score.Persistent &&
                            base.Definition.HierarchyFilter.TaxonomyCategories.ContainsKey(score.Identity) == false)
                        {
                            continue;
                        }

                        int rowOffset = i;

                        scoreFilter = score.GetRespondents(dataFilter, this.StorageMethod);

                        if (this.Definition.Dimensions.Count > (offset + 1))
                        {
                            RenderDimensions(
                                this.Definition.Dimensions[offset + 1],
                                selection,
                                writer,
                                offset + 1,
                                scoreFilter
                            );
                        }
                        else
                        {
                            if (this.Definition.Measures.Count > 0)
                            {
                                foreach (LinkBiDefinitionDimension measure in this.Definition.Measures)
                                {
                                    RenderMeasure(
                                        measure,
                                        selection,
                                        writer,
                                        offset + 1,
                                        scoreFilter
                                    );
                                }
                            }
                            else
                            {
                                writer.Append("{");
                                writer.Append(selection);

                                writer.Append(string.Format(
                                    "\"Base\": \"{0}\",",
                                    scoreFilter.Base.ToString(new CultureInfo("en-GB"))
                                ));

                                writer.Append(string.Format(
                                    "\"Value\": \"{0}\"",
                                    scoreFilter.Value.ToString(new CultureInfo("en-GB"))
                                ));
                                writer.Append("},");
                            }
                        }
                    }/*));*/
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        private void RenderMeasure(LinkBiDefinitionDimension measure, string _selection, StringBuilder writer, int columnOffset, Data filter = null)
        {
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                base.Core,
                null
            );

            // Check if the measure is a numeric variable.
            if (measure.Type == VariableType.Numeric)
            {
                string label = "";

                if (measure.XmlNode.Attributes["Label" + this.Definition.Settings.IdLanguage] == null)
                {
                    if (measure.IsTaxonomy)
                    {
                        label = (string)this.Core.TaxonomyVariableLabels.GetValue(
                            "Label",
                            new string[] { "IdTaxonomyVariable", "IdLanguage" },
                            new object[] { measure.Identity, this.Definition.Settings.IdLanguage }
                        );
                    }
                    else
                    {
                        label = (string)this.Core.VariableLabels.GetValue(
                            "Label",
                            new string[] { "IdVariable", "IdLanguage" },
                            new object[] { measure.Identity, this.Definition.Settings.IdLanguage }
                        );
                    }

                    measure.XmlNode.AddAttribute("Label" + this.Definition.Settings.IdLanguage, label);
                }

                label = measure.XmlNode.Attributes["Label" + this.Definition.Settings.IdLanguage].Value;

                string selection = _selection + string.Format(
                    "\"Label\": \"{0}\",",
                    label
                );

                Data result = storageMethod.GetRespondentsNumeric(
                    measure.Identity,
                    measure.IsTaxonomy,
                    this.Core.CaseDataLocation,
                    filter,
                    this.Definition.WeightingFilters
                );

                writer.Append("{" + selection);

                writer.Append(string.Format(
                    "\"Base\": \"{0}\",",
                    result.Base.ToString(new CultureInfo("en-GB"))
                ));

                writer.Append(string.Format(
                    "\"Value\": \"{0}\",",
                    result.Value.ToString(new CultureInfo("en-GB"))
                ));
                writer.Append(string.Format(
                    "\"StdDev\": \"{0}\"",
                    result.GetStdDev(1, result.GetMean(1)).ToString(new CultureInfo("en-GB"))
                ));

                writer.Append("},");
            }
            else
            {
                // Create a new report calculator to calculate the
                // significant difference between categories.
                ReportCalculator calculator = new ReportCalculator(
                    null,
                    base.Core,
                    null
                );

                filter = storageMethod.GetRespondents(measure, filter);
                string baseValue = filter.Base.ToString(new CultureInfo("en-GB"));

                int rowOffset = i;

                Dictionary<Guid, double> categoryValues = new Dictionary<Guid, double>();

                TaskCollection tasks = new TaskCollection();
                tasks.Synchronously = true;

                // Run through all scores of the measure.
                /*foreach (LinkBiDefinitionDimensionScore score in measure.Scores)
                {
                    if (score.Equation != null)
                    {
                        tasks.Synchronously = true;
                        break;
                    }
                }*/

                // Run through all scores of the measure.
                foreach (LinkBiDefinitionDimensionScore score in measure.Scores)
                {
                    tasks.Add(() =>
                    {
                        Data scoreFilter = score.GetRespondents(filter, this.StorageMethod);

                        categoryValues.Add(score.Identity, scoreFilter.Value);
                    });
                }

                tasks.WaitAll();

                // Run through all scores of the measure.
                foreach (LinkBiDefinitionDimensionScore score in measure.Scores)
                {
                    if (measure.IsTaxonomy && score.Persistent &&
                        base.Definition.HierarchyFilter.TaxonomyCategories.ContainsKey(score.Identity) == false)
                    {
                        continue;
                    }

                    string label = "";

                    if (score.XmlNode.Attributes["Label" + this.Definition.Settings.IdLanguage] == null)
                    {
                        if (measure.IsTaxonomy)
                        {
                            label = (string)this.Core.TaxonomyCategoryLabels.GetValue(
                                "Label",
                                new string[] { "IdTaxonomyCategory", "IdLanguage" },
                                new object[] { score.Identity, this.Definition.Settings.IdLanguage }
                            );
                        }
                        else
                        {
                            label = (string)this.Core.CategoryLabels.GetValue(
                                "Label",
                                new string[] { "IdCategory", "IdLanguage" },
                                new object[] { score.Identity, this.Definition.Settings.IdLanguage }
                            );
                        }

                        score.XmlNode.AddAttribute("Label" + this.Definition.Settings.IdLanguage, label);
                    }

                    label = score.XmlNode.Attributes["Label" + this.Definition.Settings.IdLanguage].Value;


                    string selection = _selection + string.Format(
                        "\"Label\": \"{0}\",",
                        HttpUtility.HtmlEncode(label)
                    );

                    writer.Append("{" + selection);

                    writer.Append(string.Format(
                        "\"Base\": \"{0}\",",
                        baseValue
                    ));

                    writer.Append(string.Format(
                        "\"Value\": \"{0}\"",
                        categoryValues.ContainsKey(score.Identity) ? categoryValues[score.Identity] : 0
                    ));

                    writer.Append("},");

                    i++;

                    if (totalToProcess != 0)
                    {
                        int progress = i * 100 / totalToProcess;

                        if (progress == 100)
                            base.Progress = 99;
                        else
                            base.Progress = progress;
                    }
                }
            }
        }

        #endregion
    }
}
