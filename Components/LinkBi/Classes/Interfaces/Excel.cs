using ApplicationUtilities.Classes;
using DatabaseCore.Items;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VariableSelector1.Classes;

namespace LinkBi1.Classes.Interfaces
{
    public class Excel : LinkBiInterface
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
                return "application/msexcel";
            }
        }

        #endregion


        #region Constructor

        public Excel(DatabaseCore.Core core)
            : base(core)
        {
            this.StorageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Core,
                null
            );
        }

        public Excel(DatabaseCore.Core core, LinkBiDefinition definition)
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


            // Get a temp file path to save the document to.
            string fileName = Path.GetTempFileName() + ".xlsx";

            GetTotalToProcessCount();

            // Create a new excel writer to write the PowerBI document.
            ExcelWriter writer = new ExcelWriter();

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
                    filter
                );
            }

            writer.AutoFit();

            writer.Save(fileName);

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
                if (measure.VariableType == VariableType.Numeric)
                    totalToProcess *= 1;
                else
                    totalToProcess *= measure.Scores.Count;
            }
        }

        private void RenderHeadline(ExcelWriter writer)
        {
            int a = 0;
            foreach (LinkBiDefinitionDimension filter in this.Definition.Dimensions)
            {
                DefinitionObject _filter = new DefinitionObject(
                    this.Core, 
                    this.Definition.FileName, 
                    filter.XmlNode
                );

                //writer.Write(a++, 0, filter.Name);
                writer.Write(a++, 0, _filter.GetLabel(this.Definition.Settings.IdLanguage));
            }

            //foreach (LinkBiDefinitionDimension measure in this.Definition.Measures)
            //{
            writer.Write(a++, 0, "variablename");
            writer.Write(a++, 0, "response");
            //}

            if (this.DisplayUnweightedBase)
                writer.Write(a++, 0, "unweighted base");


            if (this.DisplayEffectiveBase)
                writer.Write(a++, 0, "Effective base");

            writer.Write(a++, 0, "base");
            writer.Write(a++, 0, "value");

            if (this.ExportPercentage)
                writer.Write(a++, 0, "percentage");
        }

        int i = 1;
        private void RenderDimension(LinkBiDefinitionDimension dimension, ExcelWriter writer, int offset, Data dataFilter = null)
        {
            Data scoreFilter;

            // Run through all filter scores of the filter.
            foreach (LinkBiDefinitionDimensionScore score in dimension.Scores)
            {
                if (score.Hidden)
                    continue;

                if(dimension.IsTaxonomy && score.Persistent && 
                    base.Definition.HierarchyFilter.TaxonomyCategories.ContainsKey(score.Identity) == false)
                {
                    continue;
                }

                int rowOffset = i;

                scoreFilter = score.GetRespondents(dataFilter, this.StorageMethod);

                if (this.Definition.Dimensions.Count > (offset + 1))
                {
                    RenderDimension(
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

                for (int a = rowOffset; a < i; a++)
                {
                    DefinitionObject _score = new DefinitionObject(
                        this.Core,
                        this.Definition.FileName,
                        score.XmlNode
                    );


                    /*writer.Write(
                        offset,
                        a,
                        score.Label
                    );*/
                    writer.Write(
                        offset,
                        a,
                        _score.GetLabel(this.Definition.Settings.IdLanguage)
                    );
                }

                //i++;
            }
        }

        private void RenderMeasure(LinkBiDefinitionDimension measure, ExcelWriter writer, int columnOffset, Data filter = null)
        {
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                base.Core,
                null
            );

            int rowOffset = i;

            if (measure.Type == VariableType.Numeric)
            {
                Data result = storageMethod.GetRespondentsNumeric(
                    measure.Identity,
                    measure.IsTaxonomy,
                    this.Core.CaseDataLocation,
                    filter,
                    this.Definition.WeightingFilters
                );

                double baseValue = result.Base;
                string uBaseValueStr = result.Responses.Count.ToString(new CultureInfo("en-GB"));
                string eBaseValueStr = result.Responses.Count.ToString(new CultureInfo("en-GB"));
                string baseValueStr = baseValue.ToString(new CultureInfo("en-GB"));

                writer.Write(
                    columnOffset + 2,
                    i,
                    baseValueStr
                );

                writer.Write(
                    columnOffset + 3,
                    i,
                    result.Responses.Values.Sum(x => x[0]).ToString(new CultureInfo("en-GB"))
                );

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
            else
            {

                filter = storageMethod.GetRespondents(
                    measure,
                    filter,
                    this.Definition.WeightingFilters
                );

                double baseValue = filter.Base;
                string uBaseValueStr = filter.Responses.Count.ToString(new CultureInfo("en-GB"));
                string eBaseValueStr = filter.Responses.Count.ToString(new CultureInfo("en-GB"));
                string baseValueStr = baseValue.ToString(new CultureInfo("en-GB"));

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
                        score.XmlNode
                    );

                    /*writer.Write(
                        columnOffset + 1,
                        i,
                        score.Label
                    );*/

                    writer.Write(
                        columnOffset + 1,
                        i,
                        _score.GetLabel(this.Definition.Settings.IdLanguage)
                    );

                    int columnOffset2 = 0;

                    if (this.DisplayUnweightedBase)
                    {
                        columnOffset2 = 1;

                        writer.Write(
                            columnOffset + 2,
                            i,
                            uBaseValueStr
                        );
                    }



                    if (this.DisplayEffectiveBase)
                    {
                        columnOffset2 = 1;

                        writer.Write(
                            columnOffset + 2,
                            i,
                            eBaseValueStr
                        );
                    }

                    writer.Write(
                        columnOffset + columnOffset2 + 2,
                        i,
                        baseValueStr
                    );

                    double value;

                    Data scoreFilter = score.GetRespondents(filter, this.StorageMethod);


                    value = scoreFilter.Value;

                    writer.Write(
                        columnOffset + columnOffset2 + 3,
                        i,
                        value.ToString(new CultureInfo("en-GB"))
                    );

                    if (this.ExportPercentage)
                    {
                        double percentage = (value * 100 / baseValue);

                        if (double.IsNaN(percentage))
                            percentage = 0.0;

                        writer.Write(
                            columnOffset + columnOffset2 + 4,
                            i,
                            percentage.ToString(new CultureInfo("en-GB"))
                        );
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
                }
            }

            for (int a = rowOffset; a < i; a++)
            {
                DefinitionObject _measure = new DefinitionObject(
                    this.Core, 
                    this.Definition.FileName, 
                    measure.XmlNode
                );

                /*writer.Write(
                    columnOffset,
                    a,
                    measure.Label
                );*/
                writer.Write(
                    columnOffset,
                    a,
                    _measure.GetLabel(this.Definition.Settings.IdLanguage)
                );
            }
        }

        #endregion
    }
}
