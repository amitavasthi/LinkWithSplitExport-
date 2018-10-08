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
    public class CustomCharts : LinkBiInterface
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

        public CustomCharts(DatabaseCore.Core core)
            : base(core)
        {
            this.StorageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Core,
                null
            );
        }

        public CustomCharts(DatabaseCore.Core core, LinkBiDefinition definition)
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

        double totalToProcess = 0;
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
                RenderFilter(
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

            double toProcess = this.Definition.Dimensions[0].Scores.Count;

            for (int a = 1; a < this.Definition.Dimensions.Count; a++)
            {
                if (this.Definition.Dimensions[a].Scores.Count != 0)
                    toProcess *= this.Definition.Dimensions[a].Scores.Count;
            }

            double result = 0;

            foreach (LinkBiDefinitionDimension measure in this.Definition.Measures)
            {
                result += (measure.Scores.Count > 0 ? measure.Scores.Count : 1) * toProcess;
            }

            totalToProcess = result;
        }

        private void RenderHeadline(StringBuilder writer)
        {
            foreach (LinkBiDefinitionDimension filter in this.Definition.Dimensions)
            {
                writer.Append(filter.Name.Replace(",", "&#44;") + ",");
            }

            foreach (LinkBiDefinitionDimension measure in this.Definition.Measures)
            {
                writer.Append(measure.Name.Replace(",", "&#44;") + ",");
            }

            if (writer.Length > 0)
                writer = writer.Remove(writer.Length - 1, 1);

            writer.Append(Environment.NewLine);
        }

        int i = 1;
        private void RenderFilter(LinkBiDefinitionDimension filter, StringBuilder writer, int offset, string selection, Data dataFilter = null)
        {
            Data scoreFilter;

            // Run through all filter scores of the filter.
            foreach (LinkBiDefinitionDimensionScore score in filter.Scores)
            {
                if (score.Hidden)
                    continue;

                if (filter.IsTaxonomy && score.Persistent &&
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

                scoreFilter = score.GetRespondents(dataFilter, this.StorageMethod);

                if (this.Definition.Dimensions.Count > (offset + 1))
                {
                    RenderFilter(
                        this.Definition.Dimensions[offset + 1],
                        writer,
                        offset + 1,
                        _selection,
                        scoreFilter
                    );
                }
                else
                {

                    writer.Append(_selection);

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

                    writer = writer.Remove(writer.Length - 1, 1);

                    writer.Append(Environment.NewLine);
                }
            }
        }

        private void RenderMeasure(LinkBiDefinitionDimension measure, StringBuilder writer, int columnOffset, string selection, Data filter = null)
        {
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                base.Core,
                null
            );

            // Check if the measure's variable is a numeric variable.
            if (measure.Type == VariableType.Numeric)
            {
                Data data = storageMethod.GetRespondentsNumeric(
                    measure.Identity,
                    measure.IsTaxonomy,
                    this.Core.CaseDataLocation,
                    filter,
                    this.Definition.WeightingFilters
                );

                writer.Append(data.Value.ToString(new CultureInfo("en-GB")));
                writer.Append(",");

                i++;

                if (totalToProcess != 0)
                {
                    int progress = (int)(i * 100 / totalToProcess);

                    if (progress == 100)
                        base.Progress = 99;
                    else
                        base.Progress = progress;
                }
            }
            else
            {
                filter = storageMethod.GetRespondents(measure, filter, this.Definition.WeightingFilters);
                string baseValue = filter.Base.ToString(new CultureInfo("en-GB"));
                string uBaseValue = filter.Responses.Count.ToString(new CultureInfo("en-GB"));

                int rowOffset = i;

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

                    Data scoreFilter = score.GetRespondents(filter, this.StorageMethod);

                    for (int r = 0; r < scoreFilter.Responses.Count; r++)
                    {
                        //writer.Append(selection);

                        writer.Append(_score.GetLabel(this.Definition.Settings.IdLanguage).Replace(",", "&#44;") + ",");

                        //writer.Append(Environment.NewLine);

                        break;
                    }


                    i++;

                    if (totalToProcess != 0)
                    {
                        int progress = (int)(i * 100 / totalToProcess);

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
