using DatabaseCore.Items;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VariableSelector1.Classes;

namespace LinkBi1.Classes.Interfaces
{
    public class CSV : LinkBiInterface
    {
        #region Properties

        public DataCore.Classes.StorageMethods.Database StorageMethod { get; set; }

        public bool ExportPercentage { get; set; }

        public bool DisplayUnweightedBase { get; set; }

        public bool DisplayEffectiveBase { get; set; }

        public override string MimeType
        {
            get
            {
                return "text/comma-separated-values";
            }
        }

        #endregion


        #region Constructor

        public CSV(DatabaseCore.Core core)
            : base(core)
        {
            this.StorageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Core,
                null
            );
        }

        public CSV(DatabaseCore.Core core, LinkBiDefinition definition)
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

            if (((LinkBiSettings)this.Definition.Settings).ExportPercentage)
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

            // Get a temp file path to save the document to.
            string fileName = Path.GetTempFileName() + ".csv";

            GetTotalToProcessCount();

            // Create a new string builder to build the result .csv file.
            StringBuilder writer = new StringBuilder();

            // Render the headline.
            RenderHeadline(writer);

            Data filter = base.InitFilter();

            this.Definition.WeightingFilters.LoadRespondents(filter);

            // Check if there are filter defined.
            if (this.Definition.Dimensions.Count > 0)
            {
                // Render the first defined filter.
                RenderDimension(
                    this.Definition.Dimensions[0],
                    writer,
                    0,
                    "",
                    filter
                );
            }

            File.WriteAllText(
                fileName,
                writer.ToString()
            );

            base.Progress = 100;

            base.Finalize(fileName);

            return fileName;
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
                writer.Append(filter.Name.Replace(",", "&#44;") + ",");
            }

            writer.Append("measure,");
            writer.Append("category,");

            if (this.DisplayUnweightedBase)
                writer.Append("unweighted base,");

            if (this.DisplayEffectiveBase)
                writer.Append("Effective base,");

            writer.Append("base,");
            writer.Append("score");

            if (this.ExportPercentage)
            {
                writer.Append(",");
                writer.Append("percentage");
            }

            writer.Append(Environment.NewLine);
        }

        int i = 1;
        private void RenderDimension(LinkBiDefinitionDimension dimension, StringBuilder writer, int offset, string selection, Data dataFilter = null)
        {
            Data scoreFilter;

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

                DefinitionObject _score = new DefinitionObject(
                    this.Core,
                    this.Definition.FileName,
                    score.XmlNode.GetXPath()
                );

                string _selection = selection + _score.GetLabel(this.Definition.Settings.IdLanguage).Replace(",", "&#44;") + ",";

                int rowOffset = i;

                /*scoreFilter = storageMethod.GetRespondents(
                    score.Identity,
                    filter.Identity,
                    filter.IsTaxonomy,
                    dataFilter,
                    this.Definition.WeightingFilters
                );*/
                scoreFilter = score.GetRespondents(dataFilter, this.StorageMethod);

                if (this.Definition.Dimensions.Count > (offset + 1))
                {
                    RenderDimension(
                        this.Definition.Dimensions[offset + 1],
                        writer,
                        offset + 1,
                        _selection,
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
                            _selection,
                            scoreFilter
                        );
                    }

                    //selection = "";
                }

                /*for (int a = rowOffset; a < i; a++)
                {
                    //writer.Append(score.Label + ",");
                    selection += score.Label + ",";
                }*/

                //i++;
            }
        }

        private void RenderMeasure(LinkBiDefinitionDimension measure, StringBuilder writer, int columnOffset, string selection, Data filter = null)
        {
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                base.Core,
                null
            );

            VariableType variableType = measure.VariableType;

            // Check if the measure's variable is a numeric variable.
            if (variableType == VariableType.Numeric)
            {
                Data data = storageMethod.GetRespondentsNumeric(
                    measure.Identity,
                    measure.IsTaxonomy,
                    this.Core.CaseDataLocation,
                    filter,
                    this.Definition.WeightingFilters
                );
                
                selection += HttpUtility.HtmlEncode(measure.Label).Replace(",", "&#44;") + ",";

                writer.Append(selection);
                writer.Append(filter.Base.ToString(new CultureInfo(2057)));
                writer.Append(",");
                writer.Append(data.Value.ToString(new CultureInfo(2057)));
                writer.Append(Environment.NewLine);
            }
            else
            {
                filter = storageMethod.GetRespondents(measure, filter, this.Definition.WeightingFilters);
                string baseValue = filter.Base.ToString(new CultureInfo("en-GB"));
                string uBaseValue = filter.Responses.Count.ToString(new CultureInfo("en-GB"));
                string eBaseValue = filter.Responses.Count.ToString(new CultureInfo("en-GB"));


                int rowOffset = i;

                selection += HttpUtility.HtmlEncode(measure.Label).Replace(",", "&#44;") + ",";

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

                    DefinitionObject _score = new DefinitionObject(
                        this.Core,
                        this.Definition.FileName,
                        score.XmlNode.GetXPath()
                    );

                    writer.Append(selection);
                    writer.Append(_score.GetLabel(this.Definition.Settings.IdLanguage).Replace(",", "&#44;") + ",");

                    /*Data scoreFilter = storageMethod.GetRespondents(
                        score.Identity,
                        measure.Identity,
                        measure.IsTaxonomy,
                        filter,
                        this.Definition.WeightingFilters
                    );*/
                    Data scoreFilter = score.GetRespondents(filter, this.StorageMethod);

                    if (this.DisplayUnweightedBase)
                        writer.Append(uBaseValue + ",");

                    if (this.DisplayEffectiveBase)
                        writer.Append(eBaseValue + ",");

                    writer.Append(baseValue + ",");
                    writer.Append(scoreFilter.Value);

                    if (this.ExportPercentage)
                    {
                        writer.Append(",");

                        if (filter.Base != 0)
                            writer.Append((scoreFilter.Value * 100 / filter.Base).ToString(new CultureInfo("en-GB")));
                        else
                            writer.Append("0");
                    }

                    writer.Append(Environment.NewLine);

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
