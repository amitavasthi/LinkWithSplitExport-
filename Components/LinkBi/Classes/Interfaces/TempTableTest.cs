using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LinkBi1.Classes.Interfaces
{
    public class TempTableTest : LinkBiInterface
    {
        #region Properties

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

        public TempTableTest(DatabaseCore.Core core)
            : base(core)
        { }

        public TempTableTest(DatabaseCore.Core core, LinkBiDefinition definition)
            : base(core, definition)
        { }

        #endregion


        #region Methods

        public override void Read(string source)
        {
            throw new NotImplementedException();
        }

        StringBuilder test;

        int totalToProcess = 0;
        public override string Render()
        {
            test = new StringBuilder();

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

            //Data filter = base.InitFilter();

            //this.Definition.WeightingFilters.LoadRespondents(filter);

            // Check if there are filter defined.
            if (this.Definition.Dimensions.Count > 0)
            {
                // Render the first defined filter.
                RenderDimensions(
                    this.Definition.Dimensions[0],
                    writer,
                    0,
                    ""
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

        private void RenderHeadline(StringBuilder writer)
        {
            int a = 0;
            foreach (LinkBiDefinitionDimension filter in this.Definition.Dimensions)
            {
                writer.Append(filter.Name + ",");
            }

            writer.Append("question,");
            writer.Append("category,");

            writer.Append("base,");
            writer.Append("score");

            writer.Append(Environment.NewLine);
        }

        int i = 1;
        private void RenderDimensions(LinkBiDefinitionDimension dimension, StringBuilder writer, int offset, string dataFilter)
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
                    attribute.Value
                ));
            }

            writer.Append(">");

            // Run through all filter scores of the filter.
            foreach (LinkBiDefinitionDimensionScore score in dimension.Scores)
            {
                if (score.Hidden)
                    continue;

                int rowOffset = i;

                //scoreFilter = score.GetRespondents(dataFilter);
                StringBuilder tempTableBuilder = new StringBuilder();

                string tempTableName = string.Format(
                    "TempTable_{0}_{1}",
                    dataFilter,
                    score.Identity.ToString().Replace("-", "")
                );

                tempTableBuilder.Append(string.Format(
                    "CREATE TABLE {0} (IdRespondent uniqueidentifier)" + Environment.NewLine,
                    tempTableName
                ));

                List<object[]> categoryLinks = this.Core.CategoryLinks.GetValues(
                    new string[] { "IdVariable", "IdCategory" },
                    new string[] { "IdTaxonomyCategory" },
                    new object[] { score.Identity }
                );

                foreach (object[] categoryLink in categoryLinks)
                {
                    tempTableBuilder.Append(string.Format(
                        "INSERT INTO {0} SELECT IdRespondent FROM resp.[Var_{1}] WHERE IdCategory='{2}' {3}" + Environment.NewLine,
                        tempTableName,
                        categoryLink[0],
                        categoryLink[1],
                        string.IsNullOrEmpty(dataFilter) == false ? "AND IdRespondent IN (SELECT IdRespondent FROM " + dataFilter + ")" : ""
                    ));
                }

                test.Append(tempTableBuilder.ToString());
                this.Core.Studies.ExecuteQuery(tempTableBuilder.ToString());

                writer.Append(string.Format(
                    "<{0}",
                    score.XmlNode.Name
                ));

                foreach (XmlAttribute attribute in score.XmlNode.Attributes)
                {
                    writer.Append(string.Format(
                        " {0}=\"{1}\"",
                        attribute.Name,
                        attribute.Value
                    ));
                }

                writer.Append(">");

                if (this.Definition.Dimensions.Count > (offset + 1))
                {
                    RenderDimensions(
                        this.Definition.Dimensions[offset + 1],
                        writer,
                        offset + 1,
                        tempTableName
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
                            tempTableName
                        );
                    }
                }

                test.Append(string.Format(
                    "DROP TABLE {0}" + Environment.NewLine,
                    tempTableName
                ));
                this.Core.Studies.ExecuteQuery(string.Format(
                    "DROP TABLE {0}",
                    tempTableName
                ));

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

        private void RenderMeasure(LinkBiDefinitionDimension measure, StringBuilder writer, int columnOffset, string filter)
        {
            /*DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                bool.Parse(this.Definition.Settings["AggregateNonQAData"]),
                base.Core,
                null
            );

            filter = storageMethod.GetRespondents(
                measure.Identity, 
                measure.IsTaxonomy, 
                filter, 
                this.Definition.WeightingFilters
            );*/

            string tempTableName = string.Format(
                "{0}_{1}",
                filter,
                measure.Identity.ToString().Replace("-", "")
            );

            StringBuilder tempTableBuilder = new StringBuilder();

            tempTableBuilder.Append(string.Format(
                "CREATE TABLE {0} (IdRespondent uniqueidentifier)" + Environment.NewLine,
                tempTableName
            ));

            List<object[]> variableLinks = this.Core.VariableLinks.GetValues(
                new string[] { "IdVariable" },
                new string[] { "IdTaxonomyVariable" },
                new object[] { measure.Identity }
            );

            foreach (object[] variableLink in variableLinks)
            {
                tempTableBuilder.Append(string.Format(
                    "INSERT INTO {0} SELECT IdRespondent FROM resp.[Var_{1}] WHERE IdRespondent IN (SELECT IdRespondent FROM {2})" + Environment.NewLine,
                    tempTableName,
                    variableLink[0],
                    filter
                ));
            }

            test.Append(tempTableBuilder.ToString());
            this.Core.Studies.ExecuteQuery(tempTableBuilder.ToString());
            //string baseValue = filter.Base.ToString(new CultureInfo("en-GB"));
            //string uBaseValue = filter.Responses.Count.ToString(new CultureInfo("en-GB"));
            double baseValue = (int)this.Core.Studies.ExecuteReader(string.Format(
                "SELECT Count(DISTINCT IdRespondent) FROM {0} WHERE IdRespondent IN (SELECT IdRespondent FROM {1})",
                tempTableName,
                filter
            ), typeof(int))[0][0];

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
                    attribute.Value
                ));
            }

            writer.Append(">");

            // Run through all scores of the measure.
            foreach (LinkBiDefinitionDimensionScore score in measure.Scores)
            {
                if (score.Hidden)
                    continue;

                writer.Append(string.Format(
                    "<{0}",
                    score.XmlNode.Name
                ));

                string tempTableName2 = string.Format(
                    "{0}_{1}",
                    filter,
                    score.Identity.ToString().Replace("-", "")
                );

                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.Append(string.Format(
                    "CREATE TABLE {0} (IdRespondent uniqueidentifier)" + Environment.NewLine,
                    tempTableName2
                ));

                List<object[]> categoryLinks = this.Core.CategoryLinks.GetValues(
                    new string[] { "IdVariable", "IdCategory" },
                    new string[] { "IdTaxonomyCategory" },
                    new object[] { score.Identity }
                );

                foreach (object[] categoryLink in categoryLinks)
                {
                    queryBuilder.Append(string.Format(
                        "INSERT INTO {0} SELECT IdRespondent FROM resp.[Var_{1}] WHERE IdCategory='{2}' AND IdRespondent IN (SELECT IdRespondent FROM {3})" + Environment.NewLine,
                        tempTableName2,
                        categoryLink[0],
                        categoryLink[1],
                        tempTableName
                    ));
                }

                test.Append(queryBuilder.ToString());
                this.Core.Studies.ExecuteQuery(queryBuilder.ToString());

                double value = (int)this.Core.Studies.ExecuteReader(string.Format(
                    "SELECT Count(DISTINCT IdRespondent) FROM {0}",
                    tempTableName2
                ), typeof(int))[0][0];

                test.Append(string.Format(
                    "DROP TABLE {0}" + Environment.NewLine,
                    tempTableName2
                ));
                this.Core.Studies.ExecuteQuery(string.Format(
                    "DROP TABLE {0}",
                    tempTableName2
                ));

                foreach (XmlAttribute attribute in score.XmlNode.Attributes)
                {
                    if (attribute.Name == "Base" || attribute.Name == "Value")
                        continue;

                    writer.Append(string.Format(
                        " {0}=\"{1}\"",
                        attribute.Name,
                        attribute.Value
                    ));
                }

                /*Data scoreFilter = storageMethod.GetRespondents(
                    score.Identity,
                    measure.Identity,
                    measure.IsTaxonomy,
                    filter,
                    this.Definition.WeightingFilters
                );*/
                //Data scoreFilter = score.GetRespondents(filter);

                if (this.DisplayUnweightedBase)
                {
                    writer.Append(string.Format(
                        " UBase=\"{0}\"",
                        baseValue.ToString(new CultureInfo("en-GB"))
                    ));
                }

                if (this.DisplayEffectiveBase)
                {
                    writer.Append(string.Format(
                        " EBase=\"{0}\"",
                        baseValue.ToString(new CultureInfo("en-GB"))
                    ));
                }

                writer.Append(string.Format(
                    " Base=\"{0}\"",
                    baseValue.ToString(new CultureInfo("en-GB"))
                ));

                //double value = scoreFilter.Responses.Values.Sum(x => x[0]);

                writer.Append(string.Format(
                    " Value=\"{0}\"",
                    value.ToString(new CultureInfo("en-GB"))
                ));

                if (this.ExportPercentage)
                {
                    writer.Append(string.Format(
                        " Percentage=\"{0}\"",
                        (value * 100 / baseValue).ToString(new CultureInfo("en-GB"))
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

            test.Append(string.Format(
                "DROP TABLE {0}" + Environment.NewLine,
                tempTableName
            ));
            this.Core.Studies.ExecuteQuery(string.Format(
                "DROP TABLE {0}",
                tempTableName
            ));

            writer.Append(string.Format(
                "</{0}>",
                measure.XmlNode.Name
            ));
        }

        #endregion
    }
}
