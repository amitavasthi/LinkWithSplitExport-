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
    public class XML : LinkBiInterface
    {
        #region Properties

        public DataCore.Classes.StorageMethods.Database StorageMethod { get; set; }

        public bool ExportPercentage { get; set; }

        public bool DisplayUnweightedBase { get; set; }

        public bool DisplayEffectiveBase { get; set; }

        public TimeSpan DataAggregationTime { get; set; }

        public override string MimeType
        {
            get
            {
                return "application/xml";
            }
        }

        #endregion


        #region Constructor

        public XML(DatabaseCore.Core core)
            : base(core)
        {
            this.StorageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Core,
                null
            );
        }

        public XML(DatabaseCore.Core core, LinkBiDefinition definition)
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
            this.ExportPercentage = false;

            if (this.Definition.Settings.ExportPercentage)
            {
                this.ExportPercentage = true;
            }

            this.DisplayUnweightedBase = false;

            if (this.Definition.Settings.DisplayUnweightedBase)
            {
                this.DisplayUnweightedBase = true;
            }

            this.DisplayEffectiveBase = false;

            if (this.Definition.Settings.DisplayEffectiveBase)
            {
                this.DisplayEffectiveBase = true;
            }

            GetTotalToProcessCount();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Create a new string builder to build the result .xml file.
            StringBuilder writer = new StringBuilder();

            Data filter = base.InitFilter();

            this.Definition.WeightingFilters.LoadRespondents(filter);

            // Check if there are filter defined.
            if (this.Definition.Dimensions.Count > 0)
            {
                // Render the first defined filter.
                RenderDimensions(
                    this.Definition.Dimensions[0],
                    writer,
                    0,
                    filter
                );
            }

            stopwatch.Stop();

            this.DataAggregationTime = stopwatch.Elapsed;

            base.Progress = 100;

            string result = string.Format(
                "<Result DataAggregationTime=\"{0}\">{1}</Result>",
                this.DataAggregationTime,
                writer.ToString()
            );

            string tempFileName = Path.GetTempFileName() + ".xml";

            File.WriteAllText(tempFileName, result);

            base.Finalize(tempFileName);

            return tempFileName;
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
        private void RenderDimensions(LinkBiDefinitionDimension dimension, StringBuilder writer, int offset, Data dataFilter = null)
        {
            Data scoreFilter;

            writer.Append(string.Format(
                "<{0}",
                dimension.XmlNode.Name
            ));

            foreach (XmlAttribute attribute in dimension.XmlNode.Attributes)
            {
                writer.Append(string.Format(
                    " {0}=\"{1}\"",
                    attribute.Name,
                   HttpUtility.HtmlEncode(attribute.Value)
                ));
            }

            writer.Append(">");

            // Run through all filter scores of the filter.
            foreach (LinkBiDefinitionDimensionScore score in dimension.Scores)
            {
                if (score.Hidden)
                    continue;

                if (dimension.IsTaxonomy && score.Persistent &&
                    base.Definition.HierarchyFilter.TaxonomyCategories.ContainsKey(score.Identity) == false)
                {
                    continue;
                }

                int rowOffset = i;

                scoreFilter = score.GetRespondents(dataFilter, this.StorageMethod);

                writer.Append(string.Format(
                    "<{0}",
                    score.XmlNode.Name
                ));

                foreach (XmlAttribute attribute in score.XmlNode.Attributes)
                {
                    writer.Append(string.Format(
                        " {0}=\"{1}\"",
                        attribute.Name,
                        HttpUtility.HtmlEncode(attribute.Value)
                    ));
                }

                writer.Append(">");

                if (this.Definition.Dimensions.Count > (offset + 1))
                {
                    RenderDimensions(
                        this.Definition.Dimensions[offset + 1],
                        writer,
                        offset + 1,
                        scoreFilter
                    );
                }
                else
                {
                    foreach (LinkBiDefinitionDimension measure in this.Definition.Measures)
                    {
                        RenderMeasure(
                            measure,
                            writer,
                            offset + 1,
                            scoreFilter
                        );
                    }
                }

                writer.Append(string.Format(
                    "</{0}>",
                    score.XmlNode.Name
                ));
            }

            writer.Append(string.Format(
                "</{0}>",
                dimension.XmlNode.Name
            ));
        }

        private void RenderMeasure(LinkBiDefinitionDimension measure, StringBuilder writer, int columnOffset, Data filter = null)
        {
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                base.Core,
                null
            );

            string baseValue = filter.Base.ToString(new CultureInfo("en-GB"));
            string uBaseValue = filter.Responses.Count.ToString(new CultureInfo("en-GB"));
            string eBaseValue = filter.Responses.Count.ToString(new CultureInfo("en-GB"));

            int rowOffset = i;

            writer.Append(string.Format(
                "<{0}",
                measure.XmlNode.Name
            ));

            foreach (XmlAttribute attribute in measure.XmlNode.Attributes)
            {
                writer.Append(string.Format(
                    " {0}=\"{1}\"",
                    attribute.Name,
                    HttpUtility.HtmlEncode(attribute.Value)
                ));
            }

            writer.Append(">");

            if (measure.Type == DatabaseCore.Items.VariableType.Numeric)
            {
                Data result = storageMethod.GetRespondentsNumeric(
                    measure.Identity,
                    measure.IsTaxonomy,
                    this.Core.CaseDataLocation,
                    filter,
                    this.Definition.WeightingFilters
                );

                writer.Append(string.Format(
                    "<NumericValue Id=\"00000000-0000-0000-0000-000000000000\" Value=\"{0}\" Base=\"{1}\" />",
                    result.Value,
                    filter.Base
                ));

                i++;
            }
            else
            {
                filter = storageMethod.GetRespondents(
                    measure,
                    filter,
                    this.Definition.WeightingFilters
                );
                baseValue = filter.Base.ToString(new CultureInfo("en-GB"));
                uBaseValue = filter.Responses.Count.ToString(new CultureInfo("en-GB"));
                eBaseValue = filter.Responses.Count.ToString(new CultureInfo("en-GB"));
                // Run through all scores of the measure.
                foreach (LinkBiDefinitionDimensionScore score in measure.Scores)
                {
                    if (score.Hidden)
                        continue;

                    if (measure.IsTaxonomy && score.Persistent &&
                        base.Definition.HierarchyFilter.TaxonomyCategories.ContainsKey(score.Identity) == false)
                    {
                        continue;
                    }

                    writer.Append(string.Format(
                        "<{0}",
                        score.XmlNode.Name
                    ));

                    foreach (XmlAttribute attribute in score.XmlNode.Attributes)
                    {
                        if (attribute.Name == "Base" || attribute.Name == "Value")
                            continue;

                        writer.Append(string.Format(
                            " {0}=\"{1}\"",
                            attribute.Name,
                            HttpUtility.HtmlEncode(attribute.Value)
                        ));
                    }

                    /*Data scoreFilter = storageMethod.GetRespondents(
                        score.Identity,
                        measure.Identity,
                        measure.IsTaxonomy,
                        filter,
                        this.Definition.WeightingFilters
                    );*/
                    Data scoreFilter = score.GetRespondents(filter, this.StorageMethod);

                    if (this.DisplayUnweightedBase)
                    {
                        writer.Append(string.Format(
                            " UBase=\"{0}\"",
                            uBaseValue
                        ));
                    }
                    if (this.DisplayEffectiveBase)
                    {
                        writer.Append(string.Format(
                            " eBase=\"{0}\"",
                            uBaseValue
                        ));
                    }

                    writer.Append(string.Format(
                        " Base=\"{0}\"",
                        baseValue
                    ));

                    //double value = scoreFilter.Responses.Values.Sum(x => x[0]);
                    double value = scoreFilter.Value;

                    writer.Append(string.Format(
                        " Value=\"{0}\"",
                        value.ToString(new CultureInfo("en-GB"))
                    ));

                    if (this.ExportPercentage)
                    {
                        writer.Append(string.Format(
                            " Percentage=\"{0}\"",
                            (value * 100 / filter.Base).ToString(new CultureInfo("en-GB"))
                        ));
                    }

                    i++;

                    if (totalToProcess != 0)
                    {
                        int progress = i * 100 / totalToProcess;

                        if (progress == 100)
                            base.Progress = 99;
                        else
                            base.Progress = progress;
                    }

                    writer.Append(string.Format(
                        "></{0}>",
                        score.XmlNode.Name
                    ));
                }
            }

            writer.Append(string.Format(
                "</{0}>",
                measure.XmlNode.Name
            ));
        }

        #endregion
    }
}
