using ApplicationUtilities;
using Crosstables.Classes.ReportDefinitionClasses;
using Crosstables.Classes.ReportDefinitionClasses.Collections;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.SessionState;
using System.Xml;

namespace DataCore.Classes
{
    public class ReportCalculator
    {
        #region Properties

        public bool Synchron { get; set; }

        public Dictionary<Guid, Equation> CachedEquations { get; set; }

        /// <summary>
        /// Gets or sets the definition of the
        /// report to aggregate data for.
        /// </summary>
        public ReportDefinition Definition { get; set; }

        /// <summary>
        /// Gets or sets the database core which
        /// is used to access the database.
        /// </summary>
        public DatabaseCore.Core Core { get; set; }

        /// <summary>
        /// Gets or sets the current session.
        /// </summary>
        public HttpSessionState Session { get; set; }

        public int TotalAggregationSteps { get; set; }

        private int _processedAggregationSteps;
        public int ProcessedAggregationSteps
        {
            get
            {
                return _processedAggregationSteps;
            }
            set
            {
                _processedAggregationSteps = value;

                double percentage = (value * 100 / this.TotalAggregationSteps);

                if (percentage == 100)
                    percentage = 99;

                this.Session["DataAggregationProgress"] = percentage;
            }
        }

        /// <summary>
        /// Gets or sets the current step of data aggregation.
        /// </summary>
        public DataAggregationStatus Status
        {
            get
            {
                if (this.Session["DataAggregationStatus"] == null)
                    return DataAggregationStatus.InitializingAggregator;

                return (DataAggregationStatus)this.Session["DataAggregationStatus"];
            }
            set
            {
                this.Session["DataAggregationStatus"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the storage method which
        /// is used to access the case data.
        /// </summary>
        private StorageMethods.Database StorageMethod { get; set; }

        public WeightingFilterCollection WeightingDefinition { get; set; }

        public bool HasNesting { get; set; }

        public bool HasLeftNesting { get; set; }

        public Dictionary<Guid, object> PreloadedVariables { get; set; }

        public Data RankFilter { get; set; }

        public Data BaseFilter { get; set; }

        public Dictionary<string, XmlNode> NodeMap { get; set; }

        public Dictionary<Guid, string> EmptyLeftCategories { get; set; }

        public Dictionary<Guid, string> EmptyTopCategories { get; set; }

        #endregion


        #region Constructor

        public ReportCalculator(
            ReportDefinition definition,
            DatabaseCore.Core core,
            HttpSessionState session
        )
        {
            this.CachedEquations = new Dictionary<Guid, Equation>();
            this.NodeMap = new Dictionary<string, XmlNode>();
            this.Definition = definition;
            this.Core = core;
            this.Session = session;
            this.EmptyLeftCategories = new Dictionary<Guid, string>();
            this.EmptyTopCategories = new Dictionary<Guid, string>();

            this.InitSettings();
        }

        #endregion


        #region Public Methods

        public void Aggregate(string version)
        {
            Aggregate(version, true);
        }

        public void Aggregate(string version, bool save = true)
        {
            this.Synchron = false;

            this.Session["DataAggregationProgress"] = 0;

            this.StorageMethod = new StorageMethods.Database(
                this.Core,
                this,
                this.Definition.Settings.WeightMissingValue
            );

            this.Status = DataAggregationStatus.InitializingAggregator;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Run through all top variables of the report definition on root level.
            foreach (ReportDefinitionVariable topVariable in this.Definition.TopVariables)
            {
                // Assign the significance letters to the top variable.
                AssignSignificanceTestLetters(topVariable);
            }

            this.Status = DataAggregationStatus.InitializingFilters;

            this.HasNesting = false;
            this.HasLeftNesting = false;
            foreach (ReportDefinitionVariable leftVariable in this.Definition.LeftVariables)
            {
                if (leftVariable.NestedVariables.Count > 0)
                {
                    this.HasNesting = true;
                    this.HasLeftNesting = true;
                    break;
                }
            }
            if (!this.HasNesting)
            {
                foreach (ReportDefinitionVariable topVariable in this.Definition.TopVariables)
                {
                    if (topVariable.NestedVariables.Count > 0)
                    {
                        this.HasNesting = true;
                        break;
                    }
                }
            }

            Dictionary<Guid, PreloadDefinition> preload = new Dictionary<Guid, PreloadDefinition>();

            foreach (ReportDefinitionVariable leftVariable in this.Definition.LeftVariables)
            {
                InitData(leftVariable, preload);
                InitEquationData(leftVariable, preload);
            }
            foreach (ReportDefinitionVariable topVariable in this.Definition.TopVariables)
            {
                InitData(topVariable, preload);
                InitEquationData(topVariable, preload);
            }

            TaskCollection tasks = new TaskCollection();

            foreach (Guid idVariable in preload.Keys)
            {
                switch (preload[idVariable].VariableType)
                {
                    case VariableType.Text:
                        tasks.Add(() => this.StorageMethod.InitDataText(
                            idVariable,
                            preload[idVariable].IsTaxonomy,
                            this.Core.CaseDataLocation
                        ));
                        break;
                    case VariableType.Single:
                    case VariableType.Multi:
                        tasks.Add(() => this.StorageMethod.InitData(
                            idVariable,
                            preload[idVariable].IsTaxonomy,
                            this.Core.CaseDataLocation
                        ));
                        break;
                    case VariableType.Numeric:
                        tasks.Add(() => this.StorageMethod.InitDataNumeric(
                            idVariable,
                            preload[idVariable].IsTaxonomy,
                            this.Core.CaseDataLocation
                        ));
                        break;
                }
            }

            tasks.WaitAll();

            // Initialize the defined filters.
            Data filter = InitializeFilters();
            Data totalBase = filter;

            this.WeightingDefinition = this.Definition.WeightingFilters;

            // Check if a weighting is defined.
            if (this.WeightingDefinition == null)
                return;

            // Load the respondent's weight values for the defined weighting.
            this.WeightingDefinition.LoadRespondents(filter);

            tasks = new TaskCollection();

            this.RankFilter = filter;
            this.BaseFilter = filter;

            if (this.Definition.Settings.RankLeft)
            {
                Data rankFilter = this.RankFilter;

                if (this.Definition.TopVariables.Count != 0)
                    rankFilter = this.StorageMethod.GetRespondents(this.Definition.TopVariables[0], this.RankFilter);

                foreach (ReportDefinitionVariable leftVariable in this.Definition.LeftVariables)
                {
                    this.RankFilter = rankFilter;

                    Rank(leftVariable);
                }
            }
            else if (!this.Definition.Settings.RankLeft)
            {

                Data rankFilter = this.RankFilter;

                if (this.Definition.TopVariables.Count != 0)
                    rankFilter = this.StorageMethod.GetRespondents(this.Definition.TopVariables[0], this.RankFilter);

                foreach (ReportDefinitionVariable leftVariable in this.Definition.LeftVariables)
                {
                    this.RankFilter = rankFilter;

                    ReRank(leftVariable);
                }
            }

            if (this.Definition.Settings.RankTop)
            {
                this.RankFilter = this.StorageMethod.GetRespondents(this.Definition.TopVariables[0], this.RankFilter);

                foreach (ReportDefinitionVariable topVariable in this.Definition.TopVariables)
                {
                    Rank(topVariable);
                }
            }
            //else if (!this.Definition.Settings.RankTop)
            //{
            //    if (this.Definition.TopVariables.Count > 0)
            //    {

            //        this.RankFilter = this.StorageMethod.GetRespondents(this.Definition.TopVariables[0], this.RankFilter);

            //        foreach (ReportDefinitionVariable topVariable in this.Definition.TopVariables)
            //        {
            //            ReRank(topVariable);
            //        }
            //    }
            //}

            foreach (ReportDefinitionVariable leftVariable in this.Definition.LeftVariables)
            {
                CheckCategoryLimit(leftVariable);
            }
            foreach (ReportDefinitionVariable topVariable in this.Definition.TopVariables)
            {
                CheckCategoryLimit(topVariable);
            }

            // Prepare the definition's xml document for the data results.
            PrepareResultXml(version);

            //this.Synchron = false;

            this.Status = DataAggregationStatus.AggregatingData;

            stopwatch.Stop();

            this.Definition.DataPreperationTime = stopwatch.Elapsed;

            stopwatch.Reset();
            stopwatch.Start();


            // Run through all left variables of the definition.
            foreach (ReportDefinitionVariable leftVariable in this.Definition.LeftVariables)
            {
                // Run through all top variables of the definition.
                foreach (ReportDefinitionVariable topVariable in this.Definition.TopVariables)
                {
                    Aggregate(
                        "Report[@Name=\"" + this.Definition.XmlDocument.DocumentElement.Attributes["Name"].Value +
                        "\"]/Results",
                        leftVariable,
                        topVariable,
                        filter,
                        totalBase
                    );

                    foreach (var item in this.EmptyTopCategories)
                    {

                        foreach (ReportDefinitionScore topScore in topVariable.Scores)
                        {
                            if (topScore.Identity == item.Key)
                            {
                                topScore.HasValues = false;
                            }
                        }

                    }
                }

                foreach (var item in this.EmptyLeftCategories)
                {

                    foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                    {
                        if (leftScore.Identity == item.Key)
                        {
                            leftScore.HasValues = false;
                        }
                    }

                }
            }



            stopwatch.Stop();

            this.Definition.DataCalculationTime = stopwatch.Elapsed;

            // Check if the aggregation should be saved in the report definition file.
            if (save)
            {
                // Save the report definition.
                this.Definition.Save();
            }

            this.NodeMap.Clear();
            GC.Collect(GC.MaxGeneration);

            this.Status = DataAggregationStatus.RenderingTable;

            this.Session["DataAggregationProgress"] = 100;
            this.Session["DataAggregationTime"] = stopwatch.Elapsed;
        }

        public Data GetFilter()
        {
            // Forward call.
            return InitializeFilters();
        }


        public bool HasData(
            Guid idVariable,
            bool isTaxonomy,
            Data filter
        )
        {
            this.StorageMethod = new StorageMethods.Database(
                this.Core,
                this,
                1
            );

            // Check if the numeric variable has data.
            return this.StorageMethod.HasData(idVariable, isTaxonomy, filter);
        }

        #endregion


        #region Preperation Methods

        private void InitSettings()
        {
            if (this.Definition == null || this.Definition.Settings == null)
                return;

            object value;
            value = this.Definition.Settings.WeightMissingValue;
            value = this.Definition.Settings.SigDiffEffectiveBase;
            value = this.Definition.Settings.SignificanceTestLevel;
            value = this.Definition.Settings.SignificanceTestType;
            value = this.Definition.Settings.HideEmptyRowsAndColumns;
            value = this.Definition.Settings.RankLeft;
            value = this.Definition.Settings.RankTop;
            value = this.Definition.Settings.SignificanceWeight;

        }

        private void Rank(ReportDefinitionVariable variable)
        {
            if (variable.IsFake)
                return;

            variable.Scores.Items.Sort(Order);

            for (int i = 0; i < variable.Scores.Items.Count; i++)
            {
                if (variable.Scores.Items[i].XmlNode.Attributes["UnrankedOrder"] == null)
                {
                    variable.Scores.Items[i].XmlNode.AddAttribute("UnrankedOrder",
                        variable.Scores.Items[i].XmlNode.Attributes["Order"].Value);
                }

                if (variable.Scores.Items[i].XmlNode.Attributes["Order"] == null)
                    variable.Scores.Items[i].XmlNode.AddAttribute("Order", i.ToString());
                else
                    variable.Scores.Items[i].XmlNode.Attributes["Order"].Value = i.ToString();

              


            }

            this.RankFilter = this.StorageMethod.GetRespondents(variable, this.RankFilter);

            foreach (ReportDefinitionVariable nestedVariable in variable.NestedVariables)
            {
                Rank(nestedVariable);
            }
        }

        private void ReRank(ReportDefinitionVariable variable)
        {
            if (variable.IsFake)
                return;

            variable.Scores.Items.Sort(ReOrder);

            for (int i = 0; i < variable.Scores.Items.Count; i++)
            {
                if (variable.Scores.Items[i].XmlNode.Attributes["Order"] == null)
                    variable.Scores.Items[i].XmlNode.AddAttribute("Order", i.ToString());
                else
                    variable.Scores.Items[i].XmlNode.Attributes["Order"].Value = i.ToString();

                if (variable.Scores.Items[i].XmlNode.Attributes["UnrankedOrder"] == null)
                {
                    variable.Scores.Items[i].XmlNode.AddAttribute("UnrankedOrder",
                        variable.Scores.Items[i].XmlNode.Attributes["Order"].Value);
                }


            }

            this.RankFilter = this.StorageMethod.GetRespondents(variable, this.RankFilter);

            foreach (ReportDefinitionVariable nestedVariable in variable.NestedVariables)
            {
                ReRank(nestedVariable);
            }
        }

        private int Order(ReportDefinitionScore score1, ReportDefinitionScore score2)
        {
            Equation equation1 = null;
            Equation equation2 = null;

            if (score1.Equation != null)
                equation1 = new Equation(this.Core, score1.Equation, this.StorageMethod.WeightMissingValue);

            if (score2.Equation != null)
                equation2 = new Equation(this.Core, score2.Equation, this.StorageMethod.WeightMissingValue);

            double value1 = this.StorageMethod.GetRespondents(
                score1,
                score1.Variable,
                this.RankFilter,
                this.WeightingDefinition,
                equation1
            ).Value;

            double value2 = this.StorageMethod.GetRespondents(
                score2,
                score2.Variable,
                this.RankFilter,
                this.WeightingDefinition,
                equation2
            ).Value;

            return value2.CompareTo(value1);
        }
        private int ReOrder(ReportDefinitionScore score1, ReportDefinitionScore score2)
        {
            try
            {
                if ((score1.XmlNode.Attributes["Order"] != null) && (score2.XmlNode.Attributes["Order"] != null))
                    return int.Parse(score1.XmlNode.Attributes["Order"].Value).CompareTo(int.Parse(score2.XmlNode.Attributes["Order"].Value));
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        private void CheckCategoryLimit(ReportDefinitionVariable variable)
        {
            int limit = this.Definition.Settings.CategoryLimit;
            int count = variable.Scores.Items.Count;
            //for (int i = limit; i < variable.Scores.Items.Count; i++)

            for (int i = 0; i < variable.Scores.Items.Count; i++)
            {
                if (i >= variable.Scores.Items.Count)
                    break;

                if (variable.Scores.Items[i].ExceededCategoryLimit)
                {
                    variable.Scores.Items[i].Hidden = false;
                    variable.Scores.Items[i].ExceededCategoryLimit = false;
                }
            }

            while (variable.Scores.Items.Count > limit)
            {
                variable.Scores.Items[limit].ExceededCategoryLimit = true;
                variable.Scores.Items[limit].Hidden = true;
                variable.Scores.Items.Remove(variable.Scores.Items[limit]);
            }

            foreach (ReportDefinitionVariable nestedVariable in variable.NestedVariables)
            {
                CheckCategoryLimit(nestedVariable);
            }
        }

        private void PrepareResultXml(string version)
        {
            // Select the results xml node in the definition's xml document.
            XmlNode xmlNodeResults = this.Definition.XmlDocument.DocumentElement.SelectSingleNode("Results");

            // Check if the results xml node already exists.
            if (xmlNodeResults != null)
                xmlNodeResults.ParentNode.RemoveChild(xmlNodeResults);

            // Create a new results xml node.
            xmlNodeResults = this.Definition.XmlDocument.CreateElement("Results");
            xmlNodeResults.AddAttribute("Version", version);
            xmlNodeResults.AddAttribute("CaseDataVersion", this.Core.CaseDataVersion);

            // Add the results xml node to the definition's xml document.
            this.Definition.XmlDocument.DocumentElement.AppendChild(xmlNodeResults);

            // Run through all left variables of the definition.
            foreach (ReportDefinitionVariable leftVariable in this.Definition.LeftVariables)
            {
                // Run through all top variables of the definition.
                foreach (ReportDefinitionVariable topVariable in this.Definition.TopVariables)
                {
                    PrepareResultXml(xmlNodeResults, leftVariable, topVariable);
                }
            }
        }

        private void PrepareResultXml(
            XmlNode owner,
            ReportDefinitionVariable leftVariable,
            ReportDefinitionVariable topVariable,
            bool isNestedBase = false
        )
        {
            // Aggregating variable base step.
            this.TotalAggregationSteps++;

            XmlNode xmlNodeLeftVariable = PrepareResultXmlNode(
                owner,
                leftVariable.XmlNode
            );

            if (isNestedBase)
                xmlNodeLeftVariable.AddAttribute("IsNestedBase", "True");

            // Run through all nested variables of the left variable.
            foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
            {
                PrepareResultXml(
                    xmlNodeLeftVariable,
                    nestedLeftVariable,
                    topVariable,
                    true
                );
            }

            // Check if the left variable is a numeric variable.
            if (leftVariable.VariableType == VariableType.Numeric)
            {
                XmlNode xmlNodeLeftScore,
                    xmlNodeLeftScore2,
                    xmlNodeLeftScore3,
                    xmlNodeLeftScore4;

                if (xmlNodeLeftVariable.SelectSingleNode("Mean") == null)
                {
                    xmlNodeLeftScore = PrepareResultXmlNode(
                        xmlNodeLeftVariable,
                        "Mean",
                        Guid.NewGuid(),
                        "Left"
                    );
                    xmlNodeLeftScore2 = PrepareResultXmlNode(
                        xmlNodeLeftVariable,
                        "Min",
                        Guid.NewGuid(),
                        "Left"
                    );
                    xmlNodeLeftScore3 = PrepareResultXmlNode(
                        xmlNodeLeftVariable,
                        "Max",
                        Guid.NewGuid(),
                        "Left"
                    );
                    xmlNodeLeftScore4 = PrepareResultXmlNode(
                        xmlNodeLeftVariable,
                        "StdDev",
                        Guid.NewGuid(),
                        "Left"
                    );
                }
                else
                {
                    xmlNodeLeftScore = xmlNodeLeftVariable.SelectSingleNode("Mean");
                    xmlNodeLeftScore2 = xmlNodeLeftVariable.SelectSingleNode("Min");
                    xmlNodeLeftScore3 = xmlNodeLeftVariable.SelectSingleNode("Max");
                    xmlNodeLeftScore4 = xmlNodeLeftVariable.SelectSingleNode("StdDev");
                }

                // Aggregating row base step.
                this.TotalAggregationSteps++;

                PrepareResultXml(
                    xmlNodeLeftScore,
                    topVariable,
                    isNestedBase
                );

                xmlNodeLeftScore2.InnerXml = xmlNodeLeftScore.InnerXml;
                xmlNodeLeftScore3.InnerXml = xmlNodeLeftScore.InnerXml;
                xmlNodeLeftScore4.InnerXml = xmlNodeLeftScore.InnerXml;
            }
            else
            {
                // Run through all scores of the left variable.
                foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                {
                    XmlNode xmlNodeLeftScore = PrepareResultXmlNode(
                        xmlNodeLeftVariable,
                        leftScore.XmlNode
                    );

                    if (leftScore.XmlNode.Attributes["RenderPercentage"] != null)
                        xmlNodeLeftScore.AddAttribute("RenderPercentage", leftScore.XmlNode.Attributes["RenderPercentage"].Value);

                    if (leftScore.XmlNode.Attributes["ShowInChart"] != null)
                        xmlNodeLeftScore.AddAttribute("ShowInChart", leftScore.XmlNode.Attributes["ShowInChart"].Value);

                    // Aggregating row base step.
                    this.TotalAggregationSteps++;

                    if (leftVariable.NestedVariables.Count > 0)
                    {
                        // Run through all nested variables of the left variable.
                        foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
                        {
                            this.TotalAggregationSteps++;

                            PrepareResultXml(
                                xmlNodeLeftScore,
                                nestedLeftVariable,
                                topVariable,
                                isNestedBase
                            );
                        }
                    }
                    else
                    {
                        PrepareResultXml(
                            xmlNodeLeftScore,
                            topVariable,
                            isNestedBase
                        );
                    }
                }
            }
        }

        private void PrepareResultXml(
            XmlNode owner,
            ReportDefinitionVariable topVariable,
            bool isNestedBase = false
        )
        {
            XmlNode xmlNodeTopVariable = PrepareResultXmlNode(
                owner,
                topVariable.XmlNode
            );

            if (isNestedBase)
                xmlNodeTopVariable.AddAttribute("IsNestedBase", "True");

            // Run through all nested variables of the top variable.
            foreach (ReportDefinitionVariable nestedTopVariable in topVariable.NestedVariables)
            {
                PrepareResultXml(
                    xmlNodeTopVariable,
                    nestedTopVariable,
                    true
                );
            }

            // Check if the top variable is a numeric variable.
            if (topVariable.VariableType == VariableType.Numeric)
            {
                // Aggregating value step.
                this.TotalAggregationSteps++;

                XmlNode xmlNodeTopScore = PrepareResultXmlNode(
                    xmlNodeTopVariable,
                    "Mean",
                    Guid.NewGuid()
                );
                XmlNode xmlNodeTopScore3 = PrepareResultXmlNode(
                    xmlNodeTopVariable,
                    "Min",
                    Guid.NewGuid()
                );
                XmlNode xmlNodeTopScore2 = PrepareResultXmlNode(
                    xmlNodeTopVariable,
                    "Max",
                    Guid.NewGuid()
                );
                XmlNode xmlNodeTopScore4 = PrepareResultXmlNode(
                    xmlNodeTopVariable,
                    "StdDev",
                    Guid.NewGuid()
                );
            }
            else
            {
                // Run through all scores of the top variable.
                foreach (ReportDefinitionScore topScore in topVariable.Scores)
                {
                    // Aggregating value step.
                    this.TotalAggregationSteps++;

                    /*if (topScore.Equation != null)
                        this.TotalAggregationSteps++;*/

                    XmlNode xmlNodeTopScore = PrepareResultXmlNode(
                        xmlNodeTopVariable,
                        topScore.XmlNode
                    );

                    xmlNodeTopScore.AddAttribute("SigDiffLetter", topScore.SignificanceLetter);

                    if (topScore.XmlNode.Attributes["RenderPercentage"] != null)
                        xmlNodeTopScore.AddAttribute("RenderPercentage", topScore.XmlNode.Attributes["RenderPercentage"].Value);

                    if (topScore.XmlNode.Attributes["ShowInChart"] != null)
                        xmlNodeTopScore.AddAttribute("ShowInChart", topScore.XmlNode.Attributes["ShowInChart"].Value);

                    // Run through all nested variables of the top variable.
                    foreach (ReportDefinitionVariable nestedTopVariable in topVariable.NestedVariables)
                    {
                        PrepareResultXml(
                            xmlNodeTopScore,
                            nestedTopVariable
                        );
                    }
                }
            }
        }

        private XmlNode PrepareResultXmlNode(XmlNode owner, string name, Guid id, string position = null)
        {
            XmlNode result = owner.SelectSingleNode(string.Format(
                "{0}[@Id=\"{1}\"]",
                name,
                id
            ));

            if (result == null)
            {
                result = this.Definition.XmlDocument.CreateElement(name);
                result.AddAttribute("Id", id);

                if (position != null)
                    result.AddAttribute("Position", position);

                owner.AppendChild(result);
            }

            return result;
        }


        private XmlNode PrepareResultXmlNode(XmlNode owner, XmlNode copy)
        {
            string xPath = owner.GetXPath() + string.Format(
                "/{0}[@Id=\"{1}\"]",
                copy.Name,
                copy.Attributes["Id"].Value
            );

            XmlNode result;
            if (!this.NodeMap.ContainsKey(xPath))
            {
                result = this.Definition.XmlDocument.CreateElement(copy.Name);
                result.AddAttribute("Id", copy.Attributes["Id"].Value);

                if (copy.Attributes["Position"] != null)
                    result.AddAttribute("Position", copy.Attributes["Position"].Value);

                if (copy.Attributes["Enabled"] != null)
                    result.AddAttribute("Enabled", copy.Attributes["Enabled"].Value);

                owner.AppendChild(result);

                this.NodeMap.Add(xPath, result);
            }
            else
            {
                result = this.NodeMap[xPath];
            }

            return result;
        }


        int significanceLetterCount = 0;
        int significanceRountCount = 0;
        private void AssignSignificanceTestLetters(ReportDefinitionVariable topVariable)
        {
            if (topVariable.NestedVariables.Count > 0)
            {
                // Run through all nested variables of the top variable.
                foreach (ReportDefinitionVariable nestedTopVariable in topVariable.NestedVariables)
                {
                    // Assign the significance letters to the nested top variable.
                    AssignSignificanceTestLetters(nestedTopVariable);
                }

                return;
            }

            // Run through all scores of the top variable.
            foreach (ReportDefinitionScore topScore in topVariable.Scores)
            {
                // Check if the score is available in the selected hierarchy.
                if (topVariable.IsTaxonomy &&
                    topScore.Persistent &&
                    this.Definition.HierarchyFilter.
                    TaxonomyCategories.ContainsKey(topScore.Identity) == false)
                    continue;
                int idLetter;

                if (this.Definition.Settings.SignificanceTestType == 4)
                {
                    idLetter = significanceLetterCount + 66;
                }
                else
                {
                    idLetter = significanceLetterCount + 65;
                }

                string letter = System.Text.Encoding.UTF8.GetString(new byte[] { (byte)idLetter });

                topScore.SignificanceLetter = letter + (significanceRountCount > 0 ? significanceRountCount.ToString() : "");

                significanceLetterCount++;

                if (idLetter >= 90)
                {
                    significanceLetterCount = 0;
                    significanceRountCount++;
                }
            }
        }

        #endregion


        #region Aggregation Methods

        private void InitData(
            ReportDefinitionVariable variable,
            Dictionary<Guid, PreloadDefinition> result
        )
        {
            if (result.ContainsKey(variable.IdVariable) == false && variable.IsFake == false)
            {
                //variable.InitData(this.StorageMethod.VariableData, this.StorageMethod.CategoryData);
                PreloadDefinition definition = new PreloadDefinition();
                definition.IdVariable = variable.IdVariable;
                definition.IsTaxonomy = variable.IsTaxonomy;
                definition.VariableType = variable.VariableType;

                result.Add(variable.IdVariable, definition);
            }

            foreach (ReportDefinitionVariable nestedVariable in variable.NestedVariables)
            {
                InitData(nestedVariable, result);
            }
        }

        private void InitEquationData(
            ReportDefinitionVariable variable,
            Dictionary<Guid, PreloadDefinition> result
        )
        {
            foreach (ReportDefinitionScore score in variable.Scores)
            {
                if (score.Equation == null)
                    continue;

                //if(score.Equation.Contains("Include.R"))
                this.Synchron = true;

                Equation equation = new Equation(
                    this.Core,
                    score.Equation,
                    this.StorageMethod.WeightMissingValue
                );

                foreach (EquationPlaceHolder placeHolder in equation.Values.Values)
                {
                    /*this.StorageMethod.InitData(
                        placeHolder.IdVariable,
                        placeHolder.IsTaxonomy,
                        this.Core.CaseDataLocation
                    );*/

                    if (result.ContainsKey(placeHolder.IdVariable))
                        continue;

                    PreloadDefinition definition = new PreloadDefinition();
                    definition.IdVariable = placeHolder.IdVariable;
                    definition.IsTaxonomy = placeHolder.IsTaxonomy;
                    definition.VariableType = placeHolder.VariableType;

                    result.Add(placeHolder.IdVariable, definition);
                }
            }

            foreach (ReportDefinitionVariable nestedVariable in variable.NestedVariables)
            {
                InitEquationData(nestedVariable, result);
            }
        }

        private Data InitializeFilters()
        {
            Data workflowFilter = null;

            // Get the category ids of the applied workflow filter.
            workflowFilter = this.Definition.Workflow.GetWorkflowFilter(
                this.StorageMethod,
                true
            );

            StorageMethods.Database storageMethod = new StorageMethods.Database(
                this.Core,
                this,
                1
            );

            workflowFilter = this.Definition.GetHierarchyFilter(workflowFilter);
            Data filter = null;

            foreach (FilterCategoryOperator filterCategoryOperator in this.Definition.FilterCategories)
            {
                filter = filterCategoryOperator.GetRespondents(
                    storageMethod,
                    null
                );
            }

            if (filter != null && workflowFilter != null)
            {
                List<Guid> removalRespondents = new List<Guid>();
                foreach (Guid idRespondent in filter.Responses.Keys)
                {
                    if (!workflowFilter.Responses.ContainsKey(idRespondent))
                    {
                        removalRespondents.Add(idRespondent);
                    }
                }

                foreach (Guid idRespondent in removalRespondents)
                {
                    filter.Responses.Remove(idRespondent);
                }
            }
            else if (filter == null)
            {
                filter = workflowFilter;
            }

            return filter;
        }


        private void Aggregate(
            string path,
            ReportDefinitionVariable leftVariable,
            ReportDefinitionVariable topVariable,
            Data filter,
            Data totalBase,
            double factor = 0,
            Data scaleFilter = null,
            Dictionary<Guid, double> factors = null,
            Equation equation = null
        )
        {
            filter = this.StorageMethod.GetRespondents(
                leftVariable,
                filter,
                this.WeightingDefinition
            );

            //check top Variable exits.
            if (topVariable.Label == null)
            {
                totalBase = this.StorageMethod.GetRespondents(
                leftVariable,
                totalBase,
                this.WeightingDefinition
            );
            }

            XmlNode xmlNodeLeftVariable = this.Definition.XmlDocument.SelectSingleNode(path + string.Format(
                "/Variable[@Id=\"{0}\"]",
                leftVariable.IdVariable
            ));

            if (xmlNodeLeftVariable != null && filter != null)
            {
                Data baseValue = filter;

                if (equation != null)
                {
                    baseValue = this.StorageMethod.GetRespondents(
                        leftVariable,
                        filter,
                        this.WeightingDefinition,
                        equation
                    );
                }

                xmlNodeLeftVariable.AddAttribute("Base", baseValue.Base);
                xmlNodeLeftVariable.AddAttribute("EffectiveBase", baseValue.EffectiveBase);
                //xmlNodeLeftVariable.AddAttribute
            }

            this.ProcessedAggregationSteps++;

            TaskCollection tasks = new TaskCollection(this.Synchron);

            // Run through all nested left variables.
            foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
            {
                // Aggregate the data for the nested left variable.
                tasks.Add(() =>
                {
                    AsynchAggregateNestedLeft(
                        leftVariable,
                        nestedLeftVariable,
                        topVariable,
                        filter,
                        totalBase,
                        scaleFilter,
                        factor,
                        path + string.Format("/{0}[@Id=\"{1}\"]", leftVariable.XmlNode.Name, leftVariable.IdVariable)
                    );
                });
            }

            tasks.WaitAll();


            {
                string xPath = path + string.Format(
                    "/{0}[@Id=\"{1}\"]/",
                    leftVariable.XmlNode.Name,
                    leftVariable.IdVariable
                ) + "{0}";

                // Aggregate the row base for the left variable.
                AggregateLeftBase(
                    xPath,
                    leftVariable,
                    topVariable,
                    filter,
                    totalBase,
                    leftVariable.Scale ? 0 : factor,
                    null
                );
            }




            // Check if the left variable has nested variables.
            if (leftVariable.NestedVariables.Count > 0)
            {
                // Run through all scores of the nested left variable.
                foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                {
                    Equation equationString = equation;

                    if (equationString == null && leftScore.Equation != null)
                    {
                        if (!this.CachedEquations.ContainsKey(leftScore.Identity))
                        {
                            this.CachedEquations.Add(
                                leftScore.Identity,
                                new Equation(this.Core, leftScore.Equation, this.StorageMethod.WeightMissingValue)
                            );
                        }

                        equationString = this.CachedEquations[leftScore.Identity];
                    }
                    /*if (leftScore.Hidden)
                        continue;*/

                    Data nestedFilter = this.StorageMethod.GetRespondents(
                        leftScore,
                        leftVariable,
                        filter,
                        this.WeightingDefinition
                    );

                    this.ProcessedAggregationSteps++;

                    // Run through all nested left variables.
                    foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
                    {
                        string xPath = path + string.Format(
                            "/{0}[@Id=\"{1}\"]/{2}[@Id=\"{3}\"]",
                            leftVariable.XmlNode.Name,
                            leftVariable.IdVariable,
                            leftScore.XmlNode.Name,
                            leftScore.Identity
                        );

                        Aggregate(
                            xPath,
                            nestedLeftVariable,
                            topVariable,
                            nestedFilter,
                            totalBase,
                            leftVariable.Scale ? leftScore.Factor : factor,
                            leftVariable.Scale ? filter : scaleFilter,
                            null,
                            equationString
                        );
                    }
                }
            }
            else
            {
                string xPath = path + string.Format(
                    "/{0}[@Id=\"{1}\"]/",
                    leftVariable.XmlNode.Name,
                    leftVariable.IdVariable
                );

                // Aggregate the values for the top variable.
                AggregateTop(
                    xPath + "{0}",
                    leftVariable,
                    topVariable,
                    filter,
                    totalBase,
                    factor,
                    scaleFilter,
                    equation
                );
            }

            //{
            //    string xPath = path + string.Format(
            //        "/{0}[@Id=\"{1}\"]/",
            //        leftVariable.XmlNode.Name,
            //        leftVariable.IdVariable
            //    ) + "{0}";

            //    // Aggregate the row base for the left variable.
            //    AggregateLeftBase(
            //        xPath,
            //        leftVariable,
            //        topVariable,
            //        filter,
            //        totalBase,
            //        leftVariable.Scale ? 0 : factor,
            //        null
            //    );
            //}
        }

        private void AsynchAggregateNestedLeft(
            ReportDefinitionVariable leftVariable,
            ReportDefinitionVariable nestedLeftVariable,
            ReportDefinitionVariable topVariable,
            Data filter,
            Data totalBase,
            Data scaleFilter,
            double factor,
            string path
        )
        {
            Aggregate(
                path,
                nestedLeftVariable,
                topVariable,
                filter,
                totalBase,
                factor,
                scaleFilter
            );
        }

        private void AggregateTop(
            string path,
            ReportDefinitionVariable leftVariable,
            ReportDefinitionVariable topVariable,
            Data filter,
            Data totalBase,
            double factor = 0,
            Data scaleFilter = null,
            Equation equation = null
        )
        {
            string xPath = string.Format(
                path + "/{1}[@Id=\"{2}\"]",
                "*",
                topVariable.XmlNode.Name,
                topVariable.IdVariable
            );

            // Get the top-left base of the variables.
            filter = this.StorageMethod.GetRespondents(
                topVariable,
                filter,
                this.WeightingDefinition
            );
            totalBase = this.StorageMethod.GetRespondents(
                topVariable,
                totalBase,
                this.WeightingDefinition
            );

            //  TaskCollection tasks = new TaskCollection(this.Synchron);



            XmlNodeList xmlNodesTopVariable = this.Definition.XmlDocument.SelectNodes(xPath);

            Data variableBase = filter;
            Data totalVariableBase = totalBase;

            if (equation != null)
            {
                variableBase = this.StorageMethod.GetRespondents(
                    topVariable,
                    filter,
                    this.WeightingDefinition,
                    equation
                );
                totalVariableBase = this.StorageMethod.GetRespondents(
                    topVariable,
                    totalBase,
                    this.WeightingDefinition,
                    equation
                );
            }

            foreach (XmlNode xmlNodeTopVariable in xmlNodesTopVariable)
            {
                xmlNodeTopVariable.AddAttribute("VariableBase", variableBase.Base);
                xmlNodeTopVariable.AddAttribute("VariableCount", variableBase.UnweightedValue);
                xmlNodeTopVariable.AddAttribute("VariableEffectiveBase", variableBase.EffectiveValue);

                if (totalVariableBase == null)
                    totalVariableBase = variableBase;

                xmlNodeTopVariable.AddAttribute("TotalVariableBase", totalVariableBase.Base);
                xmlNodeTopVariable.AddAttribute("TotalVariableCount", totalVariableBase.UnweightedValue);
                xmlNodeTopVariable.AddAttribute("TotalVariableEffectiveBase", totalVariableBase.EffectiveValue);

                if (this.Definition.Settings.SignificanceTestType == 4)
                    xmlNodeTopVariable.AddAttribute("SigDiffLetter", "A");
            }

            // Check if the left variable is a numeric variable.
            if (leftVariable.VariableType == VariableType.Numeric)
            {
                NumericData data = this.StorageMethod.GetRespondentsNumeric(
                    leftVariable,
                    this.Core.CaseDataLocation,
                    filter,
                    this.WeightingDefinition
                );

                XmlNode xmlNodeMeanBase = this.Definition.XmlDocument.SelectSingleNode(xPath.Replace("*", "Mean"));
                XmlNode xmlNodeMinBase = this.Definition.XmlDocument.SelectSingleNode(xPath.Replace("*", "Min"));
                XmlNode xmlNodeMaxBase = this.Definition.XmlDocument.SelectSingleNode(xPath.Replace("*", "Max"));
                XmlNode xmlNodeStdDevBase = this.Definition.XmlDocument.SelectSingleNode(xPath.Replace("*", "StdDev"));

                foreach (XmlNode xmlNodeTopVariable in xmlNodesTopVariable)
                {
                    xmlNodeMeanBase.AddAttribute("Base", data.MeanValue);
                    xmlNodeMeanBase.AddAttribute("Count", data.UMeanValue);

                    xmlNodeMaxBase.AddAttribute("Base", data.MaxValue);
                    xmlNodeMaxBase.AddAttribute("Count", data.MaxValue);

                    xmlNodeMinBase.AddAttribute("Base", data.MinValue);
                    xmlNodeMinBase.AddAttribute("Count", data.MinValue);

                    xmlNodeStdDevBase.AddAttribute("Base", data.StdDev);
                    xmlNodeStdDevBase.AddAttribute("Count", data.UStdDev);
                }
            }

            if (topVariable.VariableType == VariableType.Numeric)
            {
                NumericData data = this.StorageMethod.GetRespondentsNumeric(
                    topVariable,
                    this.Core.CaseDataLocation,
                    filter,
                    this.WeightingDefinition
                );

                xPath = string.Format(
                    path + "/{1}[@Id=\"{2}\"]",
                    "*",
                    topVariable.XmlNode.Name,
                    topVariable.IdVariable
                );

                XmlNode xmlNodeValue = this.Definition.XmlDocument.SelectSingleNode(xPath);

                xmlNodeValue.AddAttribute("MeanValue", data.MeanValue);
                xmlNodeValue.AddAttribute("UMeanValue", data.UMeanValue);

                xmlNodeValue.AddAttribute("MaxValue", data.MaxValue);
                xmlNodeValue.AddAttribute("UMaxValue", data.MaxValue);

                xmlNodeValue.AddAttribute("MinValue", data.MinValue);
                xmlNodeValue.AddAttribute("UMinValue", data.MinValue);

                xmlNodeValue.AddAttribute("StdDevValue", data.StdDev);
                xmlNodeValue.AddAttribute("UStdDevValue", data.UStdDev);

                // Run through all scores of the left variable.
                foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                {
                    /*if (leftScore.Hidden)
                        continue;*/

                    // Aggregate the row base.
                    Data rowBase = this.StorageMethod.GetRespondents(
                        leftScore,
                        leftVariable,
                        filter,
                        this.WeightingDefinition
                    );

                    NumericData scoreData = this.StorageMethod.GetRespondentsNumeric(
                        topVariable,
                        this.Core.CaseDataLocation,
                        rowBase,
                        this.WeightingDefinition
                    );

                    xPath = string.Format(
                        path + "/{1}[@Id=\"{2}\"]/",
                        string.Format("{0}[@Id=\"{1}\"]", leftScore.XmlNode.Name, leftScore.Identity),
                        topVariable.XmlNode.Name,
                        topVariable.IdVariable
                    );

                    XmlNode xmlNodeMeanValue = this.Definition.XmlDocument.SelectSingleNode(xPath + "Mean");
                    XmlNode xmlNodeMaxValue = this.Definition.XmlDocument.SelectSingleNode(xPath + "Max");
                    XmlNode xmlNodeMinValue = this.Definition.XmlDocument.SelectSingleNode(xPath + "Min");
                    XmlNode xmlNodeStdDevValue = this.Definition.XmlDocument.SelectSingleNode(xPath + "StdDev");

                    xmlNodeMeanValue.AddAttribute("Value", scoreData.MeanValue);
                    xmlNodeMaxValue.AddAttribute("Value", scoreData.MaxValue);
                    xmlNodeMinValue.AddAttribute("Value", scoreData.MinValue);
                    xmlNodeStdDevValue.AddAttribute("Value", scoreData.StdDev);

                    /*xmlNodeMeanValue.AddAttribute("UValue", scoreData.UMeanValue);
                    xmlNodeMaxValue.AddAttribute("UValue", scoreData.MaxValue);
                    xmlNodeMinValue.AddAttribute("UValue", scoreData.MinValue);
                    xmlNodeStdDevValue.AddAttribute("UValue", scoreData.UStdDev);*/

                    xmlNodeMeanValue.AddAttribute("Base", data.MeanValue);
                    xmlNodeMeanValue.AddAttribute("Count", data.UMeanValue);
                    xmlNodeMaxValue.AddAttribute("Base", data.MaxValue);
                    xmlNodeMaxValue.AddAttribute("Count", data.MaxValue);
                    xmlNodeMinValue.AddAttribute("Base", data.MinValue);
                    xmlNodeMinValue.AddAttribute("Count", data.MinValue);
                    xmlNodeStdDevValue.AddAttribute("Base", data.StdDev);
                    xmlNodeStdDevValue.AddAttribute("Count", data.UStdDev);

                    xmlNodeValue.ParentNode.AddAttribute("Value", rowBase.Base);
                    xmlNodeValue.ParentNode.AddAttribute("UValue", rowBase.UnweightedValue);
                }
            }
            else
            {
                //Dictionary<ReportDefinitionScore, Dictionary<ReportDefinitionScore, Data>> test = new Dictionary<ReportDefinitionScore, Dictionary<ReportDefinitionScore, Data>>();
                Dictionary<Guid, double[]> leftBases = new Dictionary<Guid, double[]>();


                // Run through all nested variables of the top variable.
                foreach (ReportDefinitionVariable nestedTopVariable in topVariable.NestedVariables)
                {
                    AggregateTop(
                        //xPath.Replace("*", string.Format("{0}[@Id=\"{1}\"]", leftScore.XmlNode.Name, leftScore.Identity)),
                        xPath.Replace("*", "{0}"),
                        leftVariable,
                        nestedTopVariable,
                        filter,
                        totalBase,
                        factor,
                        null,
                        equation
                    );
                }



                //  tasks = new TaskCollection(this.Synchron);

                // Run through all scores of the top variable.
                foreach (ReportDefinitionScore topScore in topVariable.Scores)
                {
                    /*if (topScore.Hidden)
                        continue;*/

                    //   tasks.Add(() =>
                    AsynchAggregateTopScore(
                    topScore,
                    topVariable,
                    leftVariable,
                    filter,
                    totalBase,
                    scaleFilter,
                    path,
                    factor,
                    leftBases,
                    equation
                );
                    // );
                }

                //    tasks.WaitAll(); ;

                if (this.Definition.Settings.SignificanceTestType == 4)
                {

                    XmlNode xmlNode;
                    int sigLevel = 95;
                    sigLevel = this.Definition.Settings.SignificanceTestLevel;
                    double Base1 = 0, Base2 = 0, Value1 = 0, Value2 = 0, Base11 = 0, Base22 = 0;
                    foreach (string item in this.NodeMap.Keys)
                    {
                        List<string> significanceLetters = new List<string>();
                        xmlNode = this.NodeMap[item];

                        if (this.NodeMap[item].SelectSingleNode("Variable") == null)
                            continue;

                        //weighted AnsweringBase
                        if ((this.Definition.Settings.SignificanceWeight == 2) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.AnsweringBase))
                        {

                            if (this.NodeMap[item].SelectSingleNode("Variable").Attributes["VariableBase"] == null)
                                continue;

                            if (this.NodeMap[item].SelectSingleNode("Variable").Attributes["Base"] == null)
                                continue;

                            Base1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["VariableBase"].Value);
                            Base11 = Base1;
                            Value1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["Base"].Value);

                        }
                        else //weighted TotalBase
                        if ((this.Definition.Settings.SignificanceWeight == 2) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase))
                        {

                            if (this.NodeMap[item].SelectSingleNode("Variable").Attributes["TotalVariableBase"] == null)
                                continue;
                            if (this.NodeMap[item].SelectSingleNode("Variable").Attributes["Base"] == null)
                                continue;

                            Base1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["TotalVariableBase"].Value);
                            Base11 = Base1;
                            Value1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["Base"].Value);

                        }
                        else//Unweighted AnsweringBase
                         if ((this.Definition.Settings.SignificanceWeight == 1) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.AnsweringBase))
                        {

                            if (this.NodeMap[item].SelectSingleNode("Variable").Attributes["VariableBase"] == null)
                                continue;

                            if (this.NodeMap[item].SelectSingleNode("Variable").Attributes["Count"] == null)
                                continue;

                            Base1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["VariableCount"].Value);
                            Base11 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["VariableBase"].Value);                            
                            Value1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["Base"].Value);

                        }
                        else//Unweighted TotalBase
                        if ((this.Definition.Settings.SignificanceWeight == 1) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase))
                        {


                            if (this.NodeMap[item].SelectSingleNode("Variable").Attributes["TotalVariableCount"] == null)
                                continue;


                            if (this.NodeMap[item].SelectSingleNode("Variable").Attributes["Count"] == null)
                                continue;

                            Base1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["TotalVariableCount"].Value);
                           Base11= double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["VariableBase"].Value);
                            Value1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["Base"].Value);

                        }//Effective AnsweringBase
                        else if ((this.Definition.Settings.SignificanceWeight == 3) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.AnsweringBase))
                        {

                            if (this.NodeMap[item].SelectSingleNode("Variable").Attributes["VariableBase"] == null)
                                continue;

                            if (this.NodeMap[item].SelectSingleNode("Variable").Attributes["EffectiveBase"] == null)
                                continue;

                            Base1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["VariableEffectiveBase"].Value);
                            Base11 = Base1;
                            Value1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["Base"].Value);


                        }
                        else//Effective TotalBase
                       if ((this.Definition.Settings.SignificanceWeight == 3) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase) && this.NodeMap[item].SelectSingleNode("Variable").Attributes["EffectiveBase"] != null && this.NodeMap[item].SelectSingleNode("Variable").Attributes["TotalVariableEffectiveBase"] != null)
                        {
                            Base1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["TotalVariableEffectiveBase"].Value);
                            Base11 = Base1;
                            //Value1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["EffectiveBase"].Value);
                            Value1 = double.Parse(this.NodeMap[item].SelectSingleNode("Variable").Attributes["Base"].Value);

                        }

                        int i = 0;
                        foreach (XmlNode topScore in this.NodeMap[item].SelectSingleNode("Variable").SelectNodes("TaxonomyCategory"))
                        {
                            if (topScore.Attributes["Base"] == null)
                                continue;


                            //weighted AnsweringBase
                            if ((this.Definition.Settings.SignificanceWeight == 2) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.AnsweringBase))
                            {
                                Base2 = double.Parse(topScore.Attributes["Base"].Value);
                                Base22 = Base2;
                                Value2 = double.Parse(topScore.Attributes["Value"].Value);

                            }
                            else //weighted TotalBase
                            if ((this.Definition.Settings.SignificanceWeight == 2) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase))
                            {
                                Base2 = double.Parse(topScore.Attributes["TotalBase"].Value);
                                Base22 = Base2;
                                Value2 = double.Parse(topScore.Attributes["Value"].Value);
                            }
                            else//Unweighted AnsweringBase
                             if ((this.Definition.Settings.SignificanceWeight == 1) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.AnsweringBase))
                            {
                                Base2 = double.Parse(topScore.Attributes["Count"].Value);
                                Base22 = double.Parse(topScore.Attributes["Base"].Value);                               
                                Value2 = double.Parse(topScore.Attributes["Value"].Value);
                               
                            }
                            else//Unweighted TotalBase
                            if ((this.Definition.Settings.SignificanceWeight == 1) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase))
                            {
                                Base2 = double.Parse(topScore.Attributes["TotalCount"].Value);
                                Base22= double.Parse(topScore.Attributes["TotalBase"].Value);
                                Value2 = double.Parse(topScore.Attributes["Value"].Value);

                            }//Effective AnsweringBase
                            else if ((this.Definition.Settings.SignificanceWeight == 3) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.AnsweringBase))
                            {
                                Base2 = double.Parse(topScore.Attributes["EffectiveBase"].Value);
                                Base22 = Base2;
                                Value2 = double.Parse(topScore.Attributes["Value"].Value);

                            }
                            else//Effective TotalBase
                           if ((this.Definition.Settings.SignificanceWeight == 3) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase))
                            {
                                Base2 = double.Parse(topScore.Attributes["TotalEffectiveBase"].Value);
                                Base22 = Base2;
                                Value2 = double.Parse(topScore.Attributes["Value"].Value);
                            }

                            string isDiff = SignificanceCompareWithTatalCoumn(Base1, Base2, Value1, Value2, sigLevel,Base11,Base22);

                            if (isDiff == "false")
                            {
                                significanceLetters.Add(topScore.Attributes["SigDiffLetter"].Value);
                            }
                            else if (isDiff == "true")
                            {
                                //this.NodeMap[item].SelectSingleNode("Variable").SelectNodes("TaxonomyCategory")[topScore].AddAttribute("SigDiff", "A");
                                this.NodeMap[item].SelectSingleNode("Variable").SelectNodes("TaxonomyCategory")[i].AddAttribute("SigDiff", "A");

                            }
                            else
                            {
                                this.NodeMap[item].SelectSingleNode("Variable").SelectNodes("TaxonomyCategory")[i].AddAttribute("SigDiff", "");
                            }
                            i++;
                        }

                        i = 0;
                        foreach (XmlNode topScoreGroup in this.NodeMap[item].SelectSingleNode("Variable").SelectNodes("ScoreGroup"))
                        {
                            if (topScoreGroup.Attributes["Base"] == null)
                                continue;


                            //weighted AnsweringBase
                            if ((this.Definition.Settings.SignificanceWeight == 2) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.AnsweringBase))
                            {                               
                                Base2 = double.Parse(topScoreGroup.Attributes["Base"].Value);
                                Base22 = Base2;
                                Value2 = double.Parse(topScoreGroup.Attributes["Value"].Value);

                            }
                            else //weighted TotalBase
                            if ((this.Definition.Settings.SignificanceWeight == 2) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase))
                            {                         
                                Base2 = double.Parse(topScoreGroup.Attributes["TotalBase"].Value);
                                Base22 = Base2;
                                Value2 = double.Parse(topScoreGroup.Attributes["Value"].Value);
                            }
                            else//Unweighted AnsweringBase
                             if ((this.Definition.Settings.SignificanceWeight == 1) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.AnsweringBase))
                            {
                                
                                Base2 = double.Parse(topScoreGroup.Attributes["Count"].Value);
                                Base22 = double.Parse(topScoreGroup.Attributes["Base"].Value);
                                Value2 = double.Parse(topScoreGroup.Attributes["Value"].Value);
                            }
                            else//Unweighted TotalBase
                            if ((this.Definition.Settings.SignificanceWeight == 1) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase))
                            {
                                Base2 = double.Parse(topScoreGroup.Attributes["TotalCount"].Value);
                                Base22 = double.Parse(topScoreGroup.Attributes["TotalBase"].Value);
                                Value2 = double.Parse(topScoreGroup.Attributes["Value"].Value);

                            }//Effective AnsweringBase
                            else if ((this.Definition.Settings.SignificanceWeight == 3) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.AnsweringBase))
                            {
                                Base2 = double.Parse(topScoreGroup.Attributes["EffectiveBase"].Value);
                                Base22 = Base2;
                                Value2 = double.Parse(topScoreGroup.Attributes["Value"].Value);

                            }
                            else//Effective TotalBase
                           if ((this.Definition.Settings.SignificanceWeight == 3) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase))
                            {
                                Base2 = double.Parse(topScoreGroup.Attributes["TotalEffectiveBase"].Value);
                                Base22 = Base2;
                                Value2 = double.Parse(topScoreGroup.Attributes["Value"].Value);

                            }
                            string isDiff = SignificanceCompareWithTatalCoumn(Base1, Base2, Value1, Value2, sigLevel,Base11,Base22);
                            if (isDiff == "false")
                            {
                                significanceLetters.Add(topScoreGroup.Attributes["SigDiffLetter"].Value);
                            }
                            else if (isDiff == "true")
                            {
                                //this.NodeMap[item].SelectSingleNode("Variable").SelectNodes("TaxonomyCategory")[topScore].AddAttribute("SigDiff", "A");
                                this.NodeMap[item].SelectSingleNode("Variable").SelectNodes("ScoreGroup")[i].AddAttribute("SigDiff", "A");

                            }
                            else
                            {
                                this.NodeMap[item].SelectSingleNode("Variable").SelectNodes("ScoreGroup")[i].AddAttribute("SigDiff", "");
                            }
                            i++;
                        }


                        this.NodeMap[item].SelectSingleNode("Variable").AddAttribute("SigDiff", string.Join(",", significanceLetters));
                        //string.Join(",", significanceLetters);
                    }
                }
                else
                {
                    if (topVariable.NestedVariables.Count == 0 && leftVariable.NestedVariables.Count == 0)
                    {
                        // Run through all scores of the left variable.
                        foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                        {
                            /*if (leftScore.Hidden)
                                continue;*/

                            //   TaskCollection tasks1 = new TaskCollection();

                            // Run through all scores of the top variable.
                            foreach (ReportDefinitionScore topScore in topVariable.Scores)
                            {
                                /*if (topScore.Hidden)
                                    continue;*/

                                //   tasks1.Add(() => 
                                SignificanceCompare(
                                    topVariable,
                                    leftScore,
                                    topScore,
                                    path
                                );
                                //);
                            }


                            //  tasks1.WaitAll();
                        }
                    }
                }
            }
        }


        private string SignificanceCompareWithTatalCoumn(double base1, double base2, double value1, double value2, int sigLevel,double Base11,double Base22)
        {

            // TN - Total column sample size
            //WB1: Base of the subset
            //TnPct: Total column % age
            //WP1: Subset percentage
            string isCon = null;

            if (value2 <= 0 || value1 <= 0 || double.IsNaN(value1) || double.IsNaN(value1))
            {
                return isCon;
            }

            double TN = base1;
            double WB1 = base2;
            double TNPct = (value1) / Base11;
            double WP1 = (value2) / Base22;
            double WB2 = TN - WB1;
            double WP2 = ((TNPct * TN) - (WP1 * WB1)) / WB2;
            double z = (WP1 - WP2) / Math.Sqrt(((WP1 * (1 - WP1)) / WB1) + ((WP2 * (1 - WP2)) / WB2));
            double zScoreLevel = 0.0;

            switch (sigLevel)
            {
                case 95:
                    zScoreLevel = 1.96;
                    break;

                case 90:
                    zScoreLevel = 1.645;
                    break;
            }

            if (z >= zScoreLevel)
            {
                // The difference is significant .
                isCon = "true";
            }
            else if (z < -zScoreLevel)
            {
                // The difference is not significant.
                isCon = "false";
            }
            else if (double.IsNaN(z))
            {
                isCon = "null";
            }



            return isCon;
        }

        private void SignificanceCompare(
            ReportDefinitionVariable topVariable,
            ReportDefinitionScore leftScore,
            ReportDefinitionScore topScore,
            string path
        )
        {
            try
            {
                List<string> significanceLetters = new List<string>();

                string xPath = string.Format(
                    path + "/{1}[@Id=\"{2}\"]/{3}[@Id=\"{4}\"]",
                    string.Format("{0}[@Id=\"{1}\"]", leftScore.XmlNode.Name, leftScore.Identity),
                    topVariable.XmlNode.Name,
                    topVariable.IdVariable,
                    topScore.XmlNode.Name,
                    topScore.Identity
                );

                XmlNode xmlNode;

                if (this.NodeMap.ContainsKey(xPath))
                {
                    xmlNode = this.NodeMap[xPath];
                }
                else
                {
                    xmlNode = this.Definition.XmlDocument.SelectSingleNode(xPath);
                    this.NodeMap.Add(xPath, xmlNode);
                }

                if (xmlNode == null)
                    return;

                double value = 0.0;
                double _base = 0.0;

                if (xmlNode.Attributes["Value"] != null)
                    double.TryParse(xmlNode.Attributes["Value"].Value, out value);

                //if (this.Definition.Settings.SigDiffEffectiveBase && xmlNode.Attributes["EffectiveBase"] != null)
                if (this.Definition.Settings.SignificanceWeight == 3 && xmlNode.Attributes["EffectiveBase"] != null && xmlNode.Attributes["TotalEffectiveBase"] != null && this.Definition.WeightingFilters.DefaultWeighting != null)
                {
                    //double.TryParse(xmlNode.Attributes["EffectiveBase"].Value, out _base);
                    //value *= 100 / double.Parse(xmlNode.Attributes["Base"].Value);

                    if (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase)
                    {
                        double.TryParse(xmlNode.Attributes["TotalEffectiveBase"].Value, out _base);
                        value *= 100 / double.Parse(xmlNode.Attributes["TotalEffectiveBase"].Value);

                    }
                    else
                    {
                        double.TryParse(xmlNode.Attributes["EffectiveBase"].Value, out _base);
                        value *= 100 / double.Parse(xmlNode.Attributes["EffectiveBase"].Value);
                    }
                }
                else if ((xmlNode.Attributes["TotalBase"] != null) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase))
                {

                    if (this.Definition.Settings.SignificanceWeight == 2)
                        double.TryParse(xmlNode.Attributes["TotalBase"].Value, out _base);
                    else
                        double.TryParse(xmlNode.Attributes["TotalCount"].Value, out _base);

                    if (this.Definition.WeightingFilters.DefaultWeighting != null)
                        value *= 100 / Convert.ToDouble(xmlNode.Attributes["TotalBase"].Value);
                    else
                        value *= 100 / _base;
                }
                else if (xmlNode.Attributes["Base"] != null)
                {
                    if (this.Definition.Settings.SignificanceWeight == 2)
                        double.TryParse(xmlNode.Attributes["Base"].Value, out _base);
                    else
                        double.TryParse(xmlNode.Attributes["Count"].Value, out _base);

                    if (this.Definition.WeightingFilters.DefaultWeighting != null)
                        value *= 100 / Convert.ToDouble(xmlNode.Attributes["Base"].Value);
                    else
                        value *= 100 / _base;

                }

                //TaskCollection tasks2 = new TaskCollection();

                // Run through all scores of the top variable.
                //foreach (ReportDefinitionScore topScore2 in topVariable.Scores)
                if (_base != 0)
                {
                    foreach (XmlNode xmlNode2 in xmlNode.ParentNode.ChildNodes)
                    {
                        /*tasks2.Add(() => */
                        SignificanceCompareScores(
                           topVariable,
                           leftScore,
                           topScore,
                           xmlNode2,
                           significanceLetters,
                           path,
                           value,
                           _base
                        );/*);*/
                    }
                }

                //Task.WaitAll(tasks2.ToArray());
                //tasks2.WaitAll();

                // Create a new xml attribute to store the
                // signifiance test letters split by comma.
                xmlNode.AddAttribute("SigDiff", string.Join(",", significanceLetters));

                //xmlNode.AddAttribute("SigDiffLetter", topScore.SignificanceLetter);
            }
            catch { }
        }

        private void SignificanceCompareScores(
            ReportDefinitionVariable topVariable,
            ReportDefinitionScore leftScore,
            ReportDefinitionScore topScore,
            XmlNode xmlNode2,
            List<string> significanceLetters,
            string path,
            double value,
            double _base
        )
        {
            try
            {
                //if (xmlNode2.Attributes["Id"].Value == topScore.Identity.ToString())
                if (xmlNode2.Attributes["Id"].Value == topScore.XmlNode.Attributes["Id"].Value)
                    return;

                /*string xPath2 = string.Format(
                    path + "/{1}[@Id=\"{2}\"]/{3}[@Id=\"{4}\"]",
                    string.Format("{0}[@Id=\"{1}\"]", leftScore.XmlNode.Name, leftScore.Identity),
                    topVariable.XmlNode.Name,
                    topVariable.IdVariable,
                    topScore2.XmlNode.Name,
                    topScore2.Identity
                );*/

                //XmlNode xmlNode2 = this.Definition.XmlDocument.SelectSingleNode(xPath2);
                /*XmlNode xmlNode2 = xmlNode.ParentNode.SelectSingleNode(string.Format(
                    "{0}[@Id=\"{1}\"]",
                    topScore2.XmlNode.Name,
                    topScore2.Identity
                ));*/

                double value2 = 0.0;
                double _base2 = 0.0;

                if (xmlNode2.Attributes["Value"] != null)
                    double.TryParse(xmlNode2.Attributes["Value"].Value, out value2);

                //if (this.Definition.Settings.SigDiffEffectiveBase && xmlNode2.Attributes["EffectiveBase"] != null)
                if (this.Definition.Settings.SignificanceWeight == 3 && xmlNode2.Attributes["EffectiveBase"] != null && xmlNode2.Attributes["TotalEffectiveBase"] != null && this.Definition.WeightingFilters.DefaultWeighting != null)
                {

                    if (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase)
                    {
                        double.TryParse(xmlNode2.Attributes["TotalEffectiveBase"].Value, out _base2);
                        value2 *= 100 / _base2;
                       // value2 *= 100 / double.Parse(xmlNode2.Attributes["TotalBase"].Value);
                    }
                    else
                    {
                        double.TryParse(xmlNode2.Attributes["EffectiveBase"].Value, out _base2);
                        value2 *= 100 / _base2;
                        //value2 *= 100 / double.Parse(xmlNode2.Attributes["Base"].Value);
                    }
                }
                else if ((xmlNode2.Attributes["TotalBase"] != null) && (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase))
                {
                    if (this.Definition.Settings.SignificanceWeight == 2)
                        double.TryParse(xmlNode2.Attributes["TotalBase"].Value, out _base2);
                    else
                        double.TryParse(xmlNode2.Attributes["TotalCount"].Value, out _base2);

                    if (this.Definition.WeightingFilters.DefaultWeighting != null)
                        value2 *= 100 / Convert.ToDouble(xmlNode2.Attributes["TotalBase"].Value);
                    else
                        value2 *= 100 / _base2;
                }
                else if (xmlNode2.Attributes["Base"] != null)
                {
                    if (this.Definition.Settings.SignificanceWeight == 2)
                        double.TryParse(xmlNode2.Attributes["Base"].Value, out _base2);
                    else
                        double.TryParse(xmlNode2.Attributes["Count"].Value, out _base2);

                    if (this.Definition.WeightingFilters.DefaultWeighting != null)
                        value2 *= 100 / Convert.ToDouble(xmlNode2.Attributes["Base"].Value);
                    else
                        value2 *= 100 / _base2;
                }



                bool isSigDiff = IsSigDiff(
                    this.Definition.Settings.SignificanceTestLevel,
                    value,
                    _base,
                    value2,
                    _base2,
                    false,
                    this.Definition.Settings.SignificanceTestType
                );

                if (isSigDiff)
                {
                    //significanceLetters.Add(topScore2.SignificanceLetter);

                    if (xmlNode2.Attributes["EffectiveBase"] != null && this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase)
                        double.TryParse(xmlNode2.Attributes["TotalBase"].Value, out _base2);
                    double thresholdWeight = 0.0;
                    if (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase)
                    {
                        if (this.Definition.Settings.LowBaseConsider == 1 && xmlNode2.Attributes["TotalVariableCount"] != null)
                            thresholdWeight = double.Parse(xmlNode2.Attributes["TotalVariableCount"].Value);
                        else if (this.Definition.Settings.LowBaseConsider == 2 && xmlNode2.Attributes["TotalVariableBase"] != null)
                            thresholdWeight = double.Parse(xmlNode2.Attributes["TotalVariableBase"].Value);
                        else if (this.Definition.Settings.LowBaseConsider == 3 && xmlNode2.Attributes["TotalVariableEffectiveBase"] != null)
                            thresholdWeight = double.Parse(xmlNode2.Attributes["TotalVariableEffectiveBase"].Value);
                    }
                    else
                    {
                        if (this.Definition.Settings.LowBaseConsider == 1 && xmlNode2.Attributes["VariableCount"] != null)
                            thresholdWeight = double.Parse(xmlNode2.Attributes["VariableCount"].Value);
                        else if (this.Definition.Settings.LowBaseConsider == 2 && xmlNode2.Attributes["Base"] != null)
                            thresholdWeight = double.Parse(xmlNode2.Attributes["Base"].Value);
                        else if (this.Definition.Settings.LowBaseConsider == 3 && xmlNode2.Attributes["VariableEffectiveBase"] != null)
                            thresholdWeight = double.Parse(xmlNode2.Attributes["VariableEffectiveBase"].Value);
                    }
                    if (thresholdWeight != 0.0)
                        _base = thresholdWeight;
                    if (xmlNode2.Attributes["SigDiffLetter"].Value !="" && xmlNode2.Attributes["Enabled"].Value=="True")
                    {
                        significanceLetters.Add(
                       xmlNode2.Attributes["SigDiffLetter"].Value +
                       (Math.Round(_base, 4) <= this.Definition.Settings.LowBase ? "*" : "")
                   );
                    }
               
                }
            }
            catch { throw; }
        }

        private void AsynchAggregateTopScore(
            ReportDefinitionScore topScore,
            ReportDefinitionVariable topVariable,
            ReportDefinitionVariable leftVariable,
            Data filter,
            Data totalBase,
            Data scaleFilter,
            string path,
            double factor,
            Dictionary<Guid, double[]> leftBases,
            Equation equation = null
        )
        {
            try
            {
                string xPath;

                Equation equationString = equation;

                if (equationString == null && topScore.Equation != null)
                {
                    if (!this.CachedEquations.ContainsKey(topScore.Identity))
                    {
                        this.CachedEquations.Add(
                            topScore.Identity,
                            new Equation(this.Core, topScore.Equation, this.StorageMethod.WeightMissingValue)
                        );
                    }

                    equationString = this.CachedEquations[topScore.Identity];
                }

                // Aggregate the column base.
                Data columnBase = this.StorageMethod.GetRespondents(
                    topScore,
                    topVariable,
                    filter,
                    this.WeightingDefinition
                );
                Data totalColumnBase = this.StorageMethod.GetRespondents(
                    topScore,
                    topVariable,
                    totalBase,
                    this.WeightingDefinition
                );

                Data dataBase = columnBase;

                /*if (equationString != null)
                {
                    dataBase = this.StorageMethod.GetRespondents(
                        topScore,
                        topVariable,
                        filter,
                        this.WeightingDefinition,
                        equationString
                    );
                }*/

                // Check if the left variable is a numeric variable.
                if (leftVariable.VariableType == VariableType.Numeric)
                {
                    NumericData data = this.StorageMethod.GetRespondentsNumeric(
                        leftVariable,
                        this.Core.CaseDataLocation,
                        columnBase,
                        this.WeightingDefinition
                    );

                    xPath = string.Format(
                        path + "/{1}[@Id=\"{2}\"]/{3}[@Id=\"{4}\"]",
                        "{0}",
                        topVariable.XmlNode.Name,
                        topVariable.IdVariable,
                        topScore.XmlNode.Name,
                        topScore.Identity
                    );

                    XmlNode xmlNodeMeanValue = this.Definition.XmlDocument.SelectSingleNode(string.Format(xPath, "Mean"));
                    XmlNode xmlNodeMinValue = this.Definition.XmlDocument.SelectSingleNode(string.Format(xPath, "Min"));
                    XmlNode xmlNodeMaxValue = this.Definition.XmlDocument.SelectSingleNode(string.Format(xPath, "Max"));
                    XmlNode xmlNodeStdDevValue = this.Definition.XmlDocument.SelectSingleNode(string.Format(xPath, "StdDev"));

                    xmlNodeMeanValue.AddAttribute("Value", data.MeanValue);
                    xmlNodeMinValue.AddAttribute("Value", data.MinValue);
                    xmlNodeMaxValue.AddAttribute("Value", data.MaxValue);
                    xmlNodeStdDevValue.AddAttribute("Value", data.StdDev);

                    xmlNodeMeanValue.AddAttribute("UValue", data.UMeanValue);
                    xmlNodeMaxValue.AddAttribute("UValue", data.MaxValue);
                    xmlNodeMinValue.AddAttribute("UValue", data.MinValue);
                    xmlNodeStdDevValue.AddAttribute("UValue", data.UStdDev);

                    xmlNodeMeanValue.AddAttribute("Base", dataBase.Base);
                    xmlNodeMeanValue.AddAttribute("Count", dataBase.UnweightedValue);

                    xmlNodeMinValue.AddAttribute("Base", dataBase.Base);
                    xmlNodeMinValue.AddAttribute("Count", dataBase.UnweightedValue);

                    xmlNodeMaxValue.AddAttribute("Base", dataBase.Base);
                    xmlNodeMaxValue.AddAttribute("Count", dataBase.UnweightedValue);

                    xmlNodeStdDevValue.AddAttribute("Base", dataBase.Base);
                    xmlNodeStdDevValue.AddAttribute("Count", dataBase.UnweightedValue);

                    if (dataBase.Base == 0 && this.Definition.Settings.HideEmptyRowsAndColumns && this.HasNesting == false)
                    {
                        topScore.HasValues = false;
                    }
                    else
                    {
                        topScore.HasValues = true;
                    }
                }
                else
                {
                    double uMeanScore;
                    double meanScore;

                    XmlNode test = this.Definition.XmlDocument.SelectSingleNode(string.Format(
                        path + "/{1}[@Id=\"{2}\"]",
                        "*",
                        topVariable.XmlNode.Name,
                        topVariable.IdVariable
                    ));

                    // Run through all scores of the left variable.
                    foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
                    {
                        /*if (leftScore.Hidden)
                            continue;*/

                        Data data = columnBase;

                        //equationString = equation;
                        Equation leftScoreEquation = equationString;

                        if (leftScoreEquation == null && leftScore.Equation != null)
                        {
                            if (!this.CachedEquations.ContainsKey(leftScore.Identity))
                            {
                                this.CachedEquations.Add(
                                    leftScore.Identity,
                                    new Equation(this.Core, leftScore.Equation, this.StorageMethod.WeightMissingValue)
                                );
                            }

                            leftScoreEquation = this.CachedEquations[leftScore.Identity];
                        }

                        if (leftScoreEquation != null)
                        {
                            data = this.StorageMethod.GetRespondents(
                               leftScore,
                               leftVariable,
                               data,
                               this.WeightingDefinition
                           );
                        }

                        // Aggregate value.
                        data = this.StorageMethod.GetRespondents(
                            leftScore,
                            leftVariable,
                            data,
                            this.WeightingDefinition,
                            leftScoreEquation
                        );

                        Data baseValue = dataBase;
                        Data totalBaseValue = totalColumnBase;

                        if (leftScoreEquation != null)
                        {
                            baseValue = this.StorageMethod.GetRespondents(
                                leftScore,
                                leftVariable,
                                dataBase,
                                this.WeightingDefinition,
                                leftScoreEquation
                            );
                            totalBaseValue = this.StorageMethod.GetRespondents(
                                leftScore,
                                leftVariable,
                                totalBaseValue,
                                this.WeightingDefinition,
                                leftScoreEquation
                            );
                        }

                        Data _scaleFilter;

                        if (leftVariable.Scale)
                        {
                            factor = leftScore.Factor;
                        }

                        if (scaleFilter != null)
                        {
                            _scaleFilter = this.StorageMethod.GetRespondents(
                                leftScore,
                                leftVariable,
                                scaleFilter,
                                this.WeightingDefinition
                            );

                            _scaleFilter = this.StorageMethod.GetRespondents(
                                topScore,
                                topVariable,
                                _scaleFilter,
                                this.WeightingDefinition
                            );
                        }
                        else
                        {
                            _scaleFilter = columnBase;
                        }

                        this.ProcessedAggregationSteps++;

                        xPath = string.Format(
                            path + "/{1}[@Id=\"{2}\"]/{3}[@Id=\"{4}\"]",
                            string.Format("{0}[@Id=\"{1}\"]", leftScore.XmlNode.Name, leftScore.Identity),
                            topVariable.XmlNode.Name,
                            topVariable.IdVariable,
                            topScore.XmlNode.Name,
                            topScore.Identity
                        );

                        XmlNode xmlNodeValue;

                        if (this.NodeMap.ContainsKey(xPath))
                        {
                            xmlNodeValue = this.NodeMap[xPath];
                        }
                        else
                        {
                            xmlNodeValue = this.Definition.XmlDocument.SelectSingleNode(xPath);
                            this.NodeMap.Add(xPath, xmlNodeValue);
                        }

                        meanScore = ((double)data.Value * factor) / _scaleFilter.Base;

                        if (double.IsNaN(meanScore))
                            meanScore = 0;

                        xmlNodeValue.AddAttribute("MeanScore", meanScore);
                        xmlNodeValue.AddAttribute("Value", data.Value);
                        xmlNodeValue.AddAttribute("Base", baseValue.Base);
                        xmlNodeValue.AddAttribute("EffectiveBase", baseValue.EffectiveBase);
                        //xmlNodeValue.AddAttribute("Count", baseValue.UnweightedValue);
                        xmlNodeValue.AddAttribute("Count", baseValue.Responses.Count);

                        xmlNodeValue.AddAttribute("TotalBase", totalBaseValue.Base);
                        xmlNodeValue.AddAttribute("TotalCount", totalBaseValue.UnweightedValue);
                        xmlNodeValue.AddAttribute("TotalEffectiveBase", totalBaseValue.EffectiveBase);

                        if (topScore.Variable.ParentVariable == null)
                        {
                            if (this.Definition.Settings.HideEmptyRowsAndColumns && this.HasNesting == true)
                            {
                                if (dataBase.Base == 0)
                                {
                                    if (!this.EmptyTopCategories.ContainsKey(topScore.Identity))
                                        this.EmptyTopCategories.Add(topScore.Identity, topScore.Label);
                                }
                                else
                                {
                                    if (this.EmptyTopCategories.ContainsKey(topScore.Identity))
                                        this.EmptyTopCategories.Remove(topScore.Identity);
                                }
                            }

                            if (dataBase.Base == 0 && this.Definition.Settings.HideEmptyRowsAndColumns && this.HasNesting == false)
                            {
                                topScore.HasValues = false;
                            }
                            else
                            {
                                topScore.HasValues = true;
                            }
                        }

                        data.Factor = factor;

                        /*if (!test.ContainsKey(topScore))
                            test.Add(topScore, new Dictionary<ReportDefinitionScore, Data>());

                        test[topScore].Add(leftScore, data);*/
                    }
                }

                // Run through all nested variables of the top variable.
                foreach (ReportDefinitionVariable nestedTopVariable in topVariable.NestedVariables)
                {
                    xPath = path + string.Format(
                        "/{0}[@Id=\"{1}\"]/{2}[@Id=\"{3}\"]",
                        topVariable.XmlNode.Name,
                        topVariable.IdVariable,
                        topScore.XmlNode.Name,
                        topScore.Identity
                    );

                    AggregateTop(
                        xPath,
                        leftVariable,
                        nestedTopVariable,
                        columnBase,
                        totalColumnBase,
                        factor,
                        null,
                        equationString
                    );
                }
            }
            catch (Exception ex)
            {
                try
                {
                    System.IO.File.WriteAllText("C:\\Temp\\AsynchAggregationErrors\\" + topScore.Name + ".txt", ex.ToString());
                }
                catch { }
            }
        }

        /// <summary>
        /// Aggregates the row base values for a left variable.
        /// </summary>
        /// <param name="path">The base path.</param>
        /// <param name="leftVariable">The variable to aggregate the row base for.</param>
        /// <param name="filter">The current data filter.</param>
        private void AggregateLeftBase(
            string path,
            ReportDefinitionVariable leftVariable,
            ReportDefinitionVariable topVariable,
            Data filter,
            Data totalBase,
            double factor,
            Equation equation = null,
            ReportDefinitionScore topCategory = null
        )
        {
            if (leftVariable.IsFake)
                return;

            filter = this.StorageMethod.GetRespondents(
                topVariable,
                filter
            );
            totalBase = this.StorageMethod.GetRespondents(
                topVariable,
                totalBase
            );

            if (topVariable.NestedVariables.Count > 0)
            {
                foreach (ReportDefinitionVariable nestedTopVariable in topVariable.NestedVariables)
                {
                    AggregateLeftBase(
                        path + string.Format(
                            "/{0}[@Id=\"{1}\"]",
                            topVariable.XmlNode.Name,
                            topVariable.IdVariable
                        ),
                        leftVariable,
                        nestedTopVariable,
                        filter,
                        totalBase,
                        factor,
                        equation
                    );
                }

                foreach (ReportDefinitionScore topScore in topVariable.Scores)
                {
                    /*if (topScore.Hidden)
                        continue;*/

                    Equation topEquation = equation;

                    if (topEquation == null && topScore.Equation != null)
                    {
                        if (!this.CachedEquations.ContainsKey(topScore.Identity))
                        {
                            this.CachedEquations.Add(
                                topScore.Identity,
                                new Equation(this.Core, topScore.Equation, this.StorageMethod.WeightMissingValue)
                            );
                        }

                        topEquation = this.CachedEquations[topScore.Identity];
                    }

                    Data scoreFilter = this.StorageMethod.GetRespondents(
                        topScore,
                        topVariable,
                        filter
                    );
                    Data totalScoreFilter = this.StorageMethod.GetRespondents(
                        topScore,
                        topVariable,
                        totalBase
                    );

                    foreach (ReportDefinitionVariable nestedTopVariable in topVariable.NestedVariables)
                    {
                        AggregateLeftBase(
                            path + string.Format(
                                "/{0}[@Id=\"{1}\"]/{2}[@Id=\"{3}\"]",
                                topVariable.XmlNode.Name,
                                topVariable.IdVariable,
                                topScore.XmlNode.Name,
                                topScore.Identity
                            ),
                            leftVariable,
                            nestedTopVariable,
                            scoreFilter,
                            totalScoreFilter,
                            factor,
                            topEquation,
                            topScore
                        );
                    }
                }
            }

            // Run through all scores of the left variable.
            foreach (ReportDefinitionScore leftScore in leftVariable.Scores)
            {
                /*if (leftScore.Hidden)
                    continue;*/

                Equation equationString = equation;

                if (equationString == null && leftScore.Equation != null)
                {
                    if (!this.CachedEquations.ContainsKey(leftScore.Identity))
                    {
                        this.CachedEquations.Add(
                            leftScore.Identity,
                            new Equation(this.Core, leftScore.Equation, this.StorageMethod.WeightMissingValue)
                        );
                    }

                    equationString = this.CachedEquations[leftScore.Identity];
                }

                Data dataFilter = filter;

                if (equationString != null)
                {
                    dataFilter = this.StorageMethod.GetRespondents(
                        leftScore,
                        leftVariable,
                        filter,
                        this.WeightingDefinition
                    );
                }

                // Aggregate the base for the left score.
                Data data = this.StorageMethod.GetRespondents(
                    leftScore,
                    leftVariable,
                    dataFilter,
                    this.WeightingDefinition,
                    equationString
                );

                string xPath = string.Format(
                    path, string.Format(
                    "{0}[@Id=\"{1}\"]",
                    leftScore.XmlNode.Name,
                    leftScore.Identity
                ));

                XmlNode xmlNodeLeftScore = this.Definition.XmlDocument.SelectSingleNode(xPath);

                if (xmlNodeLeftScore != null)
                {
                    XmlNode xmlNodeTopVariable = xmlNodeLeftScore.SelectSingleNode(string.Format(
                        "{0}[@Id=\"{1}\"]",
                        topVariable.XmlNode.Name,
                        topVariable.IdVariable
                    ));

                    //xmlNodeTopVariable.AddAttribute("Base", data.Base);
                    xmlNodeTopVariable.AddAttribute("Base", data.Value);
                    xmlNodeTopVariable.AddAttribute("EffectiveBase", data.EffectiveBase);
                    xmlNodeTopVariable.AddAttribute("Count", data.UnweightedValue);

                    if (xmlNodeLeftScore.ParentNode.Attributes["IsNestedBase"] == null)
                    {
                        double baseValue = 0.0;

                        if (leftVariable.Scale)
                            factor = leftScore.Factor;

                        if (leftVariable.ParentVariable == null)
                        {
                            if (xmlNodeLeftScore.ParentNode.Attributes["Base"] != null)
                                baseValue = double.Parse(xmlNodeLeftScore.ParentNode.Attributes["Base"].Value);
                        }
                        else
                        {
                            string test123 = string.Format(
                                "{0}[@Id=\"{1}\"]/{2}[@Id=\"{3}\"]",
                                leftVariable.XmlNode.Name,
                                leftVariable.IdVariable,
                                leftScore.XmlNode.Name,
                                leftScore.Identity
                            );

                            XmlNode xmlNodeBase = xmlNodeLeftScore.ParentNode.ParentNode.ParentNode.SelectSingleNode(test123);

                            if (xmlNodeBase != null && xmlNodeBase.Attributes["Base"] != null)
                                baseValue = double.Parse(xmlNodeBase.Attributes["Base"].Value);
                        }

                        if (leftVariable.ParentVariable == null)
                        {
                            if (this.Definition.Settings.HideEmptyRowsAndColumns && this.HasLeftNesting == false)
                            {
                                if (topCategory == null)
                                {
                                    if (data.Base == 0)
                                    {
                                        if (!this.EmptyLeftCategories.ContainsKey(leftScore.Identity))
                                        {
                                            this.EmptyLeftCategories.Add(leftScore.Identity, leftScore.Label);
                                        }
                                    }
                                }

                            }

                            if (data.Base == 0 && this.Definition.Settings.HideEmptyRowsAndColumns && this.HasNesting == false)
                            {
                                leftScore.HasValues = false;
                            }
                            else
                            {
                                leftScore.HasValues = true;
                            }
                        }

                    }
                }

                /*foreach (ReportDefinitionVariable nestedLeftVariable in leftVariable.NestedVariables)
                {

                }*/
            }
        }

        #endregion


        #region Significance Test Methods

        /// <summary>
        /// Checks if a value is significantly different to a second value compared by the base.
        /// </summary>
        /// <param name="value">The first value to compare.</param>
        /// <param name="baseValue">The base of the first value.</param>
        /// <param name="valueCompare">The second value to compare.</param>
        /// <param name="baseCompare">The base of the second value.</param>
        /// <param name="simpleMode">Indicates if simple mode if significant calculation should be used.</param>
        /// <returns></returns>
        public bool IsSigDiff(
            int level,
            double value,
            double baseValue,
            double valueCompare,
            double baseCompare,
            bool simpleMode = false,
            int sgiestType = 1// 1 "DependentTest"  //3. IBMTTest, 2.IndependentTest ,DependentTest
        )
        {
            if (value == 0)
                return false;

            if (valueCompare == 0)
                return false;

            // Get the percentage of the first value.
            //value = value * 100 / baseValue;

            // Get the percentage of the second value.
            //valueCompare = valueCompare * 100 / baseCompare;

            if (value == 0.0 || valueCompare == 0.0)
                return false;

            // 95% = 1.96
            // 90% = 1.645
            double zScoreLevel = 0.0;

            switch (level)
            {
                case 95:
                    zScoreLevel = 1.96;
                    break;

                case 90:
                    zScoreLevel = 1.645;
                    break;
            }


            // Check if the simple mode should be used.
            if (simpleMode)
            {
                double sig = 0.05;
                if (value < (valueCompare - sig) || (value > (valueCompare + sig)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (sgiestType == 2)
            {
                // 95% = 1.96
                // 90% = 1.645

                //2.DependentTest

                double p = (value - valueCompare) / Math.Sqrt((valueCompare * (100 - valueCompare)) / baseValue);

                if (p >= zScoreLevel)
                {
                    // The difference is significant .
                    return true;
                }
                else
                {
                    // The difference is not significant.
                    return false;
                }
            }
            else if (sgiestType == 3)
            {
                //3.IndependentTest                

                double p = (value - valueCompare) / Math.Sqrt(((value * (100 - value)) / baseValue) + ((valueCompare * (100 - valueCompare)) / baseCompare));

                if (p >= zScoreLevel)
                {
                    // The difference is significant .
                    return true;
                }
                else
                {
                    // The difference is not significant.
                    return false;
                }
            }
            else
            {
                double p = (zScoreLevel * (Math.Sqrt(((value * (100 - value)) / baseValue) + (valueCompare * (100 - valueCompare)) / baseCompare))) / 100;

                if ((((value - valueCompare) / 100) > p))
                {
                    // The difference is significant .
                    return true;
                }
                else
                {
                    // The difference is not significant.
                    return false;
                }
            }
        }

        #endregion
    }

    public class PreloadDefinition
    {
        #region Properties

        public Guid IdVariable { get; set; }

        public bool IsTaxonomy { get; set; }

        public VariableType VariableType { get; set; }

        #endregion
    }

    public enum DataAggregationStatus
    {
        InitializingAggregator,
        InitializingFilters,
        AggregatingData,
        SignificanceTests,
        RenderingTable
    }
}
