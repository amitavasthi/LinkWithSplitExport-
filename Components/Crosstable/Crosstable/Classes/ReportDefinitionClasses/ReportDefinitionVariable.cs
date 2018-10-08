using ApplicationUtilities.Classes;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using WebUtilities.Controls;

namespace Crosstables.Classes.ReportDefinitionClasses
{

    public class ReportDefinitionVariable
    {
        #region Properties

        //public Dictionary<Guid, object> Data { get; set; }
        /*public VariableData Data { get; set; }*/

        /// <summary>
        /// Gets or sets the owning report definition.
        /// </summary>
        public ReportDefinition Owner { get; set; }

        /// <summary>
        /// Gets or sets the id of the variable.
        /// </summary>
        public Guid IdVariable
        {
            get
            {
                return Guid.Parse(this.XmlNode.Attributes["Id"].Value);
            }
            set
            {
                this.XmlNode.Attributes["Id"].Value = value.ToString();
            }
        }

        public bool IsTaxonomy
        {
            get
            {
                if (this.XmlNode.Attributes["IsTaxonomy"] == null)
                    return false;

                return bool.Parse(this.XmlNode.Attributes["IsTaxonomy"].Value);
            }
            set
            {
                if (this.XmlNode.Attributes["IsTaxonomy"] == null)
                    this.XmlNode.AddAttribute("IsTaxonomy", value.ToString());
                else
                    this.XmlNode.Attributes["IsTaxonomy"].Value = value.ToString();
            }
        }

        public bool Scale
        {
            get
            {
                return false;

                if (this.IsFake)
                    return false;

                return bool.Parse(this.XmlNode.Attributes["Scale"].Value);
            }
            set
            {
                return;
                this.XmlNode.Attributes["Scale"].Value = value.ToString();
            }
        }

        public string Position
        {
            get
            {
                if (this.XmlNode.Attributes["Position"] == null)
                {
                    XmlNode parent = this.XmlNode.ParentNode;

                    while (parent.Name != "Variables")
                    {
                        if (parent.ParentNode == null)
                            break;

                        parent = parent.ParentNode;
                    }

                    this.XmlNode.AddAttribute("Position", parent.Attributes["Position"].Value);
                }

                return this.XmlNode.Attributes["Position"].Value;
            }
            set
            {
                if (this.XmlNode.Attributes["Position"] == null)
                    this.XmlNode.AddAttribute("Position", value);
                else
                    this.XmlNode.Attributes["Position"].Value = value;
            }
        }

        public ReportDefinitionVariable ScaleVariable
        {
            get
            {
                ReportDefinitionVariable result = this;

                while (result.Scale == false)
                {
                    if (result.ParentVariable == null)
                        break;

                    result = result.ParentVariable;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the variable.
        /// </summary>
        public Variable Variable
        {
            get
            {
                return this.Owner.Core.Variables.GetSingle(this.IdVariable);
            }
        }

        /// <summary>
        /// Gets the variable's label in the report
        /// definition's settings defined language.
        /// </summary>
        public string Label
        {
            get
            {
                /*string result = "";

                if (this.IsTaxonomy)
                {
                    result = (string)this.Owner.Core.TaxonomyVariableLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyVariable", "IdLanguage" },
                        new object[] { this.IdVariable, this.Owner.Settings.IdLanguage }
                    );
                }
                else
                {
                    result = (string)this.Owner.Core.VariableLabels.GetValue(
                        "Label",
                        new string[] { "IdVariable", "IdLanguage" },
                        new object[] { this.IdVariable, this.Owner.Settings.IdLanguage }
                    );
                }

                return result;*/
                /*VariableSelector1.Classes.DefinitionObject variable = new VariableSelector1.Classes.DefinitionObject(
                   this.Owner.Core,
                   this.Owner.FileName,
                   this.XmlNode.GetXPath(true)
                );*/
                VariableSelector1.Classes.DefinitionObject variable = new VariableSelector1.Classes.DefinitionObject(
                   this.Owner.Core,
                   this.Owner.FileName,
                   this.XmlNode
                );

                return variable.GetLabel(this.Owner.Settings.IdLanguage);
            }
        }

        /// <summary>
        /// Gets the variable's type.
        /// </summary>
        public VariableType VariableType { get; set; }

        public bool IsFake
        {
            get
            {
                if (this.XmlNode.Attributes["IsFake"] == null)
                    return false;

                return bool.Parse(this.XmlNode.Attributes["IsFake"].Value);
            }
            set
            {
                if (this.XmlNode.Attributes["IsFake"] == null)
                    this.XmlNode.AddAttribute("IsFake", value.ToString());
                else
                    this.XmlNode.Attributes["IsFake"].Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets a collection of the variable's categories.
        /// </summary>
        public ReportDefinitionScoreCollection Scores { get; set; }

        public int ScoresCount
        {
            get
            {
                int result = 0;

                foreach (ReportDefinitionScore score in this.Scores.Items)
                {
                    // Check if the score is available in the selected hierarchy.
                    if (this.IsTaxonomy &&
                        score.Persistent &&
                        this.Owner.HierarchyFilter.
                        TaxonomyCategories.ContainsKey(score.Identity) == false)
                        continue;

                    if (this.Owner.Settings.HideEmptyRowsAndColumns)
                    {
                        if (score.Hidden == false && score.HasValues == true)
                            result++;
                    }
                    else
                    {
                        if (score.Hidden == false)
                            result++;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the xml node that
        /// contains the variable definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets a collection of the nested variables of the report variable.
        /// </summary>
        public List<ReportDefinitionVariable> NestedVariables { get; set; }

        /// <summary>
        /// Gets or sets the parent variable in case the variable is a nested variable.
        /// </summary>
        public ReportDefinitionVariable ParentVariable { get; set; }

        public bool ParentScale
        {
            get
            {
                ReportDefinitionVariable parent = this;

                while (parent != null)
                {
                    if (parent.Scale)
                        return true;

                    parent = parent.ParentVariable;
                }

                return false;
            }
        }


        public int ColumnSpan
        {
            get
            {
                int result = 0;

                if (this.VariableType == VariableType.Numeric)
                {
                    result += 5;
                }
                else
                {
                    // Run for the length of the categories plus the base value row.
                    for (int i = 0; i < this.ScoresCount + 1; i++)
                    {
                        if (this.NestedVariables.Count > 0)
                        {
                            // Run through all nested variables.
                            foreach (ReportDefinitionVariable nestedVariable in this.NestedVariables)
                            {
                                result += nestedVariable.ColumnSpan;
                            }
                        }
                        else
                        {
                            result += 1;
                        }
                    }
                }


                //if (this.Owner.Settings.DisplayUnweightedBase && this.NestedVariables.Count == 0)
                //{
                //    result += 1;
                //}

                //if (this.Owner.Settings.DisplayEffectiveBase && this.NestedVariables.Count == 0)
                //{
                //    result += 1;
                //}

                return result;
            }
        }

        public int ColumnSpanBase
        {
            get
            {
                int result = 0;

                // Run for the length of the categories plus the base value row.
                for (int i = 0; i < 1; i++)
                {
                    if (this.NestedVariables.Count > 0)
                    {
                        // Run through all nested variables.
                        foreach (ReportDefinitionVariable nestedVariable in this.NestedVariables)
                        {
                            result += nestedVariable.ColumnSpan;
                        }
                    }
                    else
                    {
                        result += 1;
                    }
                }

                return result;
            }
        }


        public int RowSpan
        {
            get
            {
                int result = 0;

                if (this.VariableType == VariableType.Numeric)
                {
                    // Min mean max and stddev
                    result += 4;

                    // Base
                    result += 1;
                }
                else
                {
                    // Base value row.
                    int additional = 1;

                    int count = this.ScoresCount;

                    // Run for the length of the categories plus the additional rows.
                    for (int i = 0; i < count; i++)
                    {
                        if (this.NestedVariables.Count > 0)
                        {
                            // Run through all nested variables.
                            foreach (ReportDefinitionVariable nestedVariable in this.NestedVariables)
                            {
                                result += nestedVariable.RowSpan;
                            }
                        }
                        else
                        {
                            // Check if sig diff is enabled.
                            if (this.Owner.SignificanceTest)
                                result += 1;

                            if (this.Owner.Settings.ShowValues)
                                result += 1;

                            if (this.Owner.Settings.ShowPercentage)
                                result += 1;
                        }
                    }

                    for (int i = 0; i < additional; i++)
                    {
                        if (this.NestedVariables.Count > 0)
                        {
                            // Run through all nested variables.
                            foreach (ReportDefinitionVariable nestedVariable in this.NestedVariables)
                            {
                                result += nestedVariable.RowSpan;
                            }
                        }
                        else
                        {
                            result += 1;
                        }
                    }

                    // Check if there is an addition mean value row.
                    if (this.Scale)
                    {
                        if (this.NestedVariables.Count > 0)
                        {
                            // Run through all nested variables.
                            foreach (ReportDefinitionVariable nestedVariable in this.NestedVariables)
                            {
                                result += nestedVariable.RowSpanMean;
                            }
                        }
                        else
                        {
                            // Check if sig diff is enabled.
                            if (this.Owner.SignificanceTest && this.ParentVariable == null)
                                result += 2;
                            else
                                result += 2;
                        }
                    }
                }


                if (this.Owner.Settings.DisplayUnweightedBase && this.NestedVariables.Count == 0)
                    result++;

                if (this.Owner.Settings.DisplayEffectiveBase && this.NestedVariables.Count == 0)
                    result++;

                return result;
            }
        }

        public int RowSpanBase
        {
            get
            {
                int result = 0;

                // Run for the length of the categories plus the base value row.
                for (int i = 0; i < 1; i++)
                {
                    if (this.NestedVariables.Count > 0)
                    {
                        // Run through all nested variables.
                        foreach (ReportDefinitionVariable nestedVariable in this.NestedVariables)
                        {
                            result += nestedVariable.RowSpan;
                        }
                    }
                    else
                    {
                        // Check if sig diff is enabled.
                        /*if (this.Owner.SignificanceTest)
                            result += 3;
                        else
                            result += 2;*/
                        result += 1;
                    }
                }

                return result;
            }
        }

        public int RowSpanMean
        {
            get
            {
                int result = 0;

                if (this.NestedVariables.Count > 0)
                {
                    // Run through all nested variables.
                    foreach (ReportDefinitionVariable nestedVariable in this.NestedVariables)
                    {
                        int baseValueRows = 1;

                        if (this.Owner.Settings.DisplayUnweightedBase)
                            baseValueRows++;

                        if (this.Owner.Settings.DisplayEffectiveBase)
                            baseValueRows++;

                        // Run for the length of the categories plus the base value row.
                        for (int i = 0; i < nestedVariable.ScoresCount + baseValueRows; i++)
                        {
                            result += nestedVariable.RowSpanMean;
                        }
                    }
                }
                else
                {
                    result += 1;

                    if (this.Owner.Settings.DisplayUnweightedBase)
                        result++;

                    if (this.Owner.Settings.DisplayEffectiveBase)
                        result++;

                    for (int i = 0; i < this.ScoresCount; i++)
                    {
                        // Check if sig diff is enabled.
                        if (this.Owner.SignificanceTest && this.ParentVariable == null)
                            result += 2;
                        else
                            result += 1;
                    }
                }

                return result;
            }
        }

        public int RowSpanMeanScore
        {
            get
            {
                int result = 0;

                if (this.NestedVariables.Count > 0)
                {
                    // Run through all nested variables.
                    foreach (ReportDefinitionVariable nestedVariable in this.NestedVariables)
                    {
                        int baseValueRows = 1;

                        if (this.Owner.Settings.DisplayUnweightedBase)
                            baseValueRows++;

                        if (this.Owner.Settings.DisplayEffectiveBase)
                            baseValueRows++;

                        // Run for the length of the categories plus the base value row.
                        for (int i = 0; i < nestedVariable.ScoresCount + baseValueRows; i++)
                        {
                            result += nestedVariable.RowSpanMeanScore;
                        }
                    }
                }
                else
                {
                    // Check if sig diff is enabled.
                    if (this.Owner.SignificanceTest && this.ParentVariable == null)
                        result += 2;
                    else
                        result += 1;
                }

                return result;
            }
        }

        public int NestedLevels
        {
            get
            {
                int result = 1;

                int nestedLevel = 0;
                foreach (ReportDefinitionVariable nestedVariable in this.NestedVariables)
                {
                    if (nestedVariable.NestedLevels > nestedLevel)
                        nestedLevel = nestedVariable.NestedLevels;
                }

                result += nestedLevel;

                return result;
            }
        }

        public int NestedLevelsBase
        {
            get
            {
                ReportDefinitionVariable parent = this;

                while (parent.ParentVariable != null)
                {
                    parent = parent.ParentVariable;
                }

                return parent.NestedLevels;
            }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the report definition variable.
        /// </summary>
        /// <param name="owner">The owning report definition.</param>
        /// <param name="variable">The variable to create a report variable from.</param>
        public ReportDefinitionVariable(
            ReportDefinition owner,
            TaxonomyVariable variable,
            ReportDefinitionVariable parent
        )
        {
            this.Owner = owner;
            this.NestedVariables = new List<ReportDefinitionVariable>();
            this.Scores = new ReportDefinitionScoreCollection();

            this.XmlNode = this.Owner.XmlDocument.CreateElement("Variable");

            this.XmlNode.AddAttribute("Id", variable.Id.ToString());
            this.XmlNode.AddAttribute("Scale", variable.Scale.ToString());
            this.XmlNode.AddAttribute("IsTaxonomy", "True");

            if (parent != null)
                parent.XmlNode.AppendChild(this.XmlNode);

            this.VariableType = GetVariableType();

            RenderCategories();
            ParseCategories();
        }

        /// <summary>
        /// Creates a new instance of the report definition variable.
        /// </summary>
        /// <param name="owner">The owning report definition.</param>
        /// <param name="variable">The variable to create a report variable from.</param>
        public ReportDefinitionVariable(
            ReportDefinition owner,
            Variable variable,
            ReportDefinitionVariable parent
        )
        {
            this.Owner = owner;
            this.NestedVariables = new List<ReportDefinitionVariable>();
            this.Scores = new ReportDefinitionScoreCollection();

            this.XmlNode = this.Owner.XmlDocument.CreateElement("Variable");

            this.XmlNode.AddAttribute("Id", variable.Id.ToString());
            this.XmlNode.AddAttribute("Scale", variable.Scale.ToString());

            if (parent != null)
                parent.XmlNode.AppendChild(this.XmlNode);

            this.VariableType = GetVariableType();

            RenderCategories();
            ParseCategories();
        }

        /// <summary>
        /// Creates a new instance of the report definition variable.
        /// </summary>
        /// <param name="owner">The owning report definition.</param>
        /// <param name="xmlNode">The xml node that contains the variable definition.</param>
        public ReportDefinitionVariable(
            ReportDefinition owner,
            XmlNode xmlNode
        )
        {
            this.Owner = owner;
            this.NestedVariables = new List<ReportDefinitionVariable>();
            this.Scores = new ReportDefinitionScoreCollection();
            this.XmlNode = xmlNode;

            //this.IdVariable = this.IdVariable;

            this.VariableType = GetVariableType();

            ParseCategories();
            ParseNestedVariables();
        }

        /// <summary>
        /// Creates a new instance of the report definition variable.
        /// </summary>
        /// <param name="owner">The owning report definition.</param>
        /// <param name="xmlNode">The xml node that contains the variable definition.</param>
        /// <param name="parent">The parent report definition variable.</param>
        public ReportDefinitionVariable(
            ReportDefinition owner,
            XmlNode xmlNode,
            ReportDefinitionVariable parent
        )
            : this(owner, xmlNode)
        {
            this.ParentVariable = parent;
        }

        #endregion


        #region Methods
        public VariableType GetVariableType()
        {

            if (this.IsFake)
                return DatabaseCore.Items.VariableType.Single;

            VariableType result;

            if (this.XmlNode.Attributes["VariableType"] == null)
            {
                if (!this.IsTaxonomy)
                {
                    result = (VariableType)this.Owner.Core.Variables.GetValue(
                        "Type",
                        "Id",
                        this.IdVariable
                    );
                }
                else
                {
                    result = (VariableType)this.Owner.Core.TaxonomyVariables.GetValue(
                        "Type",
                        "Id",
                        this.IdVariable
                    );
                }

                this.XmlNode.AddAttribute("VariableType", result);
            }
            else
            {
                return (VariableType)Enum.Parse(
                    typeof(VariableType),
                    this.XmlNode.Attributes["VariableType"].Value
                );
            }

            return result;
        }

        public void RenderCategories()
        {
            if (this.IsTaxonomy)
            {
                RenderTaxonomyCategories();
                return;
            }

            this.XmlNode.InnerXml = "";

            Category[] categories = this.Owner.Core.Categories.Get("IdVariable", this.IdVariable);

            // Run through all categories of the variable.
            foreach (Category category in categories.OrderBy(x => x.Order))
            {
                this.XmlNode.InnerXml += string.Format(
                    "<Category Id=\"{0}\" Order=\"{1}\" Name=\"{2}\"></Category>",
                    category.Id,
                    category.Order,
                    category.Name
                );
            }
        }

        public void RenderTaxonomyCategories()
        {
            this.XmlNode.InnerXml = "";

            TaxonomyCategory[] categories = this.Owner.Core.
                TaxonomyCategories.Get("IdTaxonomyVariable", this.IdVariable);

            // Run through all categories of the variable.
            foreach (TaxonomyCategory category in categories.OrderBy(x => x.Value))
            {
                this.XmlNode.InnerXml += string.Format(
                    "<TaxonomyCategory Id=\"{0}\" Hidden=\"{1}\" Name=\"{2}\"></TaxonomyCategory>",
                    category.Id,
                    !category.Enabled,
                    category.Name
                );
            }
        }

        public void ParseCategories()
        {
            if (this.XmlNode.Attributes["Position"] != null)
            {
                if (this.XmlNode.Attributes["Position"].Value == "Left")
                {// Run through all child nodes of the variable xml node.
                    foreach (XmlNode xmlNodeScore in this.XmlNode.ChildNodes.OrderByNumeric("Order"))
                    {
                        ParseCategoryValue(xmlNodeScore);
                    }
                }
                else if (this.XmlNode.Attributes["Position"].Value == "Top")
                {
                    foreach (XmlNode xmlNodeScore in this.XmlNode.ChildNodes)
                    {
                        ParseCategoryValue(xmlNodeScore);
                    }
                }
            }
            else
            {
                foreach (XmlNode xmlNodeScore in this.XmlNode.ChildNodes.OrderByNumeric("Order"))
                {
                    ParseCategoryValue(xmlNodeScore);
                }
            }
        }

        private void ParseCategoryValue(XmlNode xmlNodeScore)
        {
            ReportDefinitionScore score = null;

            switch (xmlNodeScore.Name)
            {
                case "Category":
                    score = new ReportDefinitionCategory(this, xmlNodeScore);
                    break;
                case "TaxonomyCategory":
                    score = new ReportDefinitionTaxonomyCategory(this, xmlNodeScore);
                    break;
                case "ScoreGroup":
                    score = new ReportDefinitionScoreGroup(this, xmlNodeScore);
                    break;
            }

            if (score != null)
            {
                this.Scores.Add(score);
            }
        }

        public void ParseNestedVariables()
        {
            // Run through all child variable xml nodes.
            foreach (XmlNode xmlNodeVariable in this.XmlNode.SelectNodes("Variable"))
            {
                ReportDefinitionVariable variable = new ReportDefinitionVariable(this.Owner, xmlNodeVariable, this);

                this.NestedVariables.Add(variable);

                string position = variable.Position;
            }
        }


        /*public void InitData(
            Dictionary<Guid, Dictionary<Guid, object[]>> variableData,
            Dictionary<Guid, Dictionary<Guid, Dictionary<Guid, int>>> categoryData
        )
        {
            this.Data = new VariableData();

            KeyValuePair<List<string>, Dictionary<Guid, Guid>> commandTexts = BuildCommandTexts();

            foreach (string _commandText in commandTexts.Key)
            {
                List<object[]> values;

                string commandText = _commandText;

                if (commandText.Contains(" WHERE IdCategory IN ()"))
                    commandText = commandText.Replace(" WHERE IdCategory IN ()", "");

                if (this.Owner.Core.CaseDataLocation == CaseDataLocation.File &&
                    this.VariableType != VariableType.Text)
                {
                    CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                    // Konstantin's database system.
                    values = dataLink.Select2(
                        commandText,
                        this.Owner.Core.ClientName
                    );
                }
                else
                {
                    // MS SQL Server.
                    values = this.Owner.Core.Responses[this.IdVariable].ExecuteReader(commandText);
                }

                if(this.Owner.HierarchyFilter != null && this.Owner.HierarchyFilter.IsLoaded == false)
                {
                    this.Owner.HierarchyFilter.Load();
                }

                foreach (object[] value in values)
                {
                    if (this.VariableType == VariableType.Numeric || 
                        this.VariableType == VariableType.Text)
                    {
                        double numericAnswer = 0;
                        string textAnswer = "";

                        try {
                            if (this.VariableType == VariableType.Numeric)
                                numericAnswer = (double)value[1];
                            else if (this.VariableType == VariableType.Text)
                                textAnswer = (string)value[1];

                            this.Data.Add((Guid)value[0], textAnswer, numericAnswer);
                        }
                        catch { }

                        continue;
                    }

                    Guid idCategory = (Guid)value[1];

                    if (commandTexts.Value.ContainsKey(idCategory))
                        if (this.Owner.HierarchyFilter != null && this.Owner.HierarchyFilter.TaxonomyCategories.ContainsKey(commandTexts.Value[idCategory]) == false)
                            continue;

                    if (commandTexts.Value.ContainsKey(idCategory))
                        idCategory = commandTexts.Value[idCategory];

                    this.Data.Add(idCategory, (Guid)value[0]);
                }
            }

            if (!categoryData.ContainsKey(this.IdVariable))
                categoryData.Add(this.IdVariable, this.Data.CategoryData);

            if (!variableData.ContainsKey(this.IdVariable))
                variableData.Add(this.IdVariable, this.Data.VariableRespondents);
        }*/

        private KeyValuePair<List<string>, Dictionary<Guid, Guid>> BuildCommandTexts()
        {
            Dictionary<Guid, Guid> categoryLinks = new Dictionary<Guid, Guid>();

            List<string> commandTexts = new List<string>();

            if (this.IsTaxonomy)
            {
                List<object[]> linkedCategories;

                if (this.VariableType == VariableType.Numeric ||
                    this.VariableType == VariableType.Text)
                {
                    List<object[]> linkedVariables;

                    StringBuilder result = new StringBuilder();

                    linkedVariables = this.Owner.Core.VariableLinks.GetValues(
                        new string[] { "IdVariable" },
                        new string[] { "IdTaxonomyVariable" },
                        new object[] { this.IdVariable }
                    );

                    foreach (object[] linkedVariable in linkedVariables)
                    {
                        if (this.VariableType == VariableType.Single || this.VariableType == VariableType.Multi)
                        {
                            /*result.Append("SELECT [IdRespondent], NULL FROM [resp].[Var_" + ((Guid)linkedVariable[0]) +
                                "] WHERE IdCategory NOT IN (SELECT Id FROM Categories WHERE IdVariable='" + ((Guid)linkedVariable[0]) + "' AND Name='SystemMissing') UNION ALL ");*/
                            result.Append(string.Format(
                                "SELECT [IdRespondent], NULL FROM [resp].[Var_" + ((Guid)linkedVariable[0]) +
                                "] WHERE IdCategory IN ({1}) UNION ALL ",
                                linkedVariable[0],
                                string.Join(",", this.Owner.Core.Categories.ExecuteReader(
                                    "SELECT Id FROM Categories WHERE IdVariable='" + ((Guid)linkedVariable[0]) +
                                    "' AND Name <> 'SystemMissing'").Select(x => "'" + x[0] + "'"
                                ))
                            ));
                        }
                        else
                        {
                            if (this.VariableType == VariableType.Numeric)
                                result.Append("SELECT [IdRespondent], [NumericAnswer] FROM [resp].[Var_" + ((Guid)linkedVariable[0]) + "] UNION ALL ");
                            else if (this.VariableType == VariableType.Text)
                                result.Append("SELECT [IdRespondent], [TextAnswer] FROM [resp].[Var_" + ((Guid)linkedVariable[0]) + "] UNION ALL ");
                        }
                    }

                    if (linkedVariables.Count > 0)
                    {
                        result = result.Remove(result.Length - 11, 11);

                        commandTexts.Add(result.ToString());
                    }
                }
                else
                {
                    linkedCategories = this.Owner.Core.CategoryLinks.GetValues(
                        new string[] { "IdVariable", "IdCategory", "IdTaxonomyCategory" },
                        new string[] { "IdTaxonomyVariable" },
                        new object[] { this.IdVariable }
                    );

                    Dictionary<Guid, List<string>> variables = new Dictionary<Guid, List<string>>();


                    foreach (object[] link in linkedCategories)
                    {
                        Guid idVariable = (Guid)link[0];

                        if (!variables.ContainsKey(idVariable))
                            variables.Add(idVariable, new List<string>());

                        variables[idVariable].Add("'" + link[1] + "'");

                        if (!categoryLinks.ContainsKey((Guid)link[1]))
                            categoryLinks.Add((Guid)link[1], (Guid)link[2]);
                    }

                    foreach (KeyValuePair<Guid, List<string>> v in variables)
                    {
                        commandTexts.Add(string.Format(
                            "SELECT [IdRespondent], [IdCategory] FROM [resp].[Var_{0}] WHERE [IdCategory] IN ({1})",
                            v.Key,
                            string.Join(",", v.Value)
                        ));
                    }
                }

            }
            else
            {
                if (this.VariableType == VariableType.Single || this.VariableType == VariableType.Multi)
                {
                    commandTexts.Add(string.Format(
                        "SELECT [IdRespondent], [IdCategory] FROM [resp].[Var_{0}] WHERE IdCategory IN ({1})",
                        this.IdVariable,
                        string.Join(",", this.Owner.Core.Categories.ExecuteReader(
                            "SELECT Id FROM Categories WHERE IdVariable={0} AND ExcludeBase=0",
                            new object[] { this.IdVariable }
                        ).Select(x => "'" + x[0] + "'"))
                    ));
                }
                else if (this.VariableType == VariableType.Numeric)
                {
                    commandTexts.Add(string.Format(
                        "SELECT [IdRespondent], [NumericAnswer] FROM [resp].[Var_{0}]",
                        this.IdVariable
                    ));
                }
                else if (this.VariableType == VariableType.Text)
                {
                    commandTexts.Add(string.Format(
                        "SELECT [IdRespondent], [TextAnswer] FROM [resp].[Var_{0}]",
                        this.IdVariable
                    ));
                }
            }

            return new KeyValuePair<List<string>, Dictionary<Guid, Guid>>(commandTexts, categoryLinks);
        }


        public bool CheckForNewCategories()
        {
            bool result = false;

            //was giving error for IsScoreGroup and Equation for text varibale
            if (this.VariableType != VariableType.Numeric && this.VariableType != VariableType.Text)
            {
                Dictionary<Guid, List<object[]>> categories;

                string tablePrefix;

                if (this.IsTaxonomy)
                {
                    tablePrefix = "Taxonomy";

                    categories = this.Owner.Core.TaxonomyCategories.ExecuteReaderDict<Guid>(
                        "SELECT IdTaxonomyVariable, Id FROM TaxonomyCategories ORDER BY [Order]",
                        new object[] { }
                    );
                }
                else
                {
                    tablePrefix = "";

                    categories = this.Owner.Core.TaxonomyCategories.ExecuteReaderDict<Guid>(
                        "SELECT IdVariable, Id FROM Categories",
                        new object[] { }
                    );
                }

                if (categories.ContainsKey(this.IdVariable))
                {
                    foreach (object[] idCategory in categories[this.IdVariable])
                    {
                        if (!this.Scores.Mapping.ContainsKey((Guid)idCategory[1]))
                        {
                            result = true;

                            object[] category = this.Owner.Core.TaxonomyCategories.ExecuteReaderDict<Guid>(
                                "SELECT Id, IsScoreGroup, Name, [Order], [Value], [Enabled], [Equation] FROM [" + tablePrefix + "Categories] WHERE Id={0}",
                                new object[] { idCategory[1] }
                            ).First().Value.First();

                            string label = (string)this.Owner.Core.TaxonomyCategoryLabels.ExecuteReaderDict<Guid>(
                                "SELECT Id" + tablePrefix + "Category, Label FROM " + tablePrefix + "CategoryLabels",
                                new object[] { }
                            )[(Guid)idCategory[1]].First()[1];

                            if ((bool)category[1] == true)
                            {
                                StringBuilder xmlBuilder = new StringBuilder();

                                xmlBuilder.Append(string.Format(
                                    "<{0} Id=\"{1}\" Order=\"{2}\" Value=\"{3}\" Enabled=\"{4}\" Text=\"{5}\" Label2057=\"{5}\" Equation=\"{6}\">",
                                    "ScoreGroup",
                                    category[0],
                                    category[3],
                                    category[4],
                                    category[5],
                                    HttpUtility.HtmlEncode(label),
                                    HttpUtility.HtmlEncode(category[6])
                                ));

                                xmlBuilder.Append("</ScoreGroup>");

                                this.XmlNode.InnerXml += xmlBuilder.ToString();
                                xmlBuilder.Clear();
                            }
                            else
                            {
                                this.XmlNode.InnerXml += (string.Format(
                                    "<{0} Id=\"{1}\" Order=\"{2}\" Value=\"{3}\" Enabled=\"{4}\" Text=\"{5}\" Label2057=\"{5}\"></{0}>",
                                    tablePrefix + "Category",
                                    category[0],
                                    category[3],
                                    category[4],
                                    category[5],
                                    HttpUtility.HtmlEncode(label)
                                ));
                            }
                        }
                    }
                }
            }

            foreach (ReportDefinitionVariable variable in this.NestedVariables)
            {
                if (variable.CheckForNewCategories())
                    result = true;
            }

            return result;
        }

        #endregion
    }

    public enum ReportDefinitionVariablePosition
    {
        Left,
        Top
    }

    public class VariableData
    {
        #region Properties

        public Dictionary<Guid, KeyValuePair<string, double>> Respondents { get; set; }

        public Dictionary<Guid, Dictionary<Guid, int>> CategoryData { get; set; }

        public Dictionary<Guid, object[]> VariableRespondents { get; set; }

        public Dictionary<Guid, Dictionary<long, int>> CategoryData2 { get; set; }

        public Dictionary<long, int> VariableRespondents2 { get; set; }

        #endregion


        #region Constructor

        public VariableData()
        {
            this.Respondents = new Dictionary<Guid, KeyValuePair<string, double>>();
            this.CategoryData = new Dictionary<Guid, Dictionary<Guid, int>>();
            this.VariableRespondents = new Dictionary<Guid, object[]>();

            this.CategoryData2 = new Dictionary<Guid, Dictionary<long, int>>();
            this.VariableRespondents2 = new Dictionary<long, int>();
        }

        #endregion


        #region Methods

        public void Add(Guid idCategory, Guid idRespondent)
        {
            if (!this.CategoryData.ContainsKey(idCategory))
                this.CategoryData.Add(idCategory, new Dictionary<Guid, int>());

            if (!this.CategoryData[idCategory].ContainsKey(idRespondent))
                this.CategoryData[idCategory].Add(idRespondent, 0);

            if (!this.VariableRespondents.ContainsKey(idRespondent))
                this.VariableRespondents.Add(idRespondent, new object[0]);

            //this.CategoryData[idCategory][idRespondent]++;
            //this.VariableRespondents[idRespondent]++;
        }
        public void Add(Guid idCategory, long idRespondent)
        {
            if (!this.CategoryData2.ContainsKey(idCategory))
                this.CategoryData2.Add(idCategory, new Dictionary<long, int>());

            if (!this.CategoryData2[idCategory].ContainsKey(idRespondent))
                this.CategoryData2[idCategory].Add(idRespondent, 0);

            if (!this.VariableRespondents2.ContainsKey(idRespondent))
                this.VariableRespondents2.Add(idRespondent, 0);

            this.CategoryData2[idCategory][idRespondent]++;
            this.VariableRespondents2[idRespondent]++;
        }

        public void Add(Guid idRespondent, string textAnswer, double numericAnswer)
        {
            if (!this.Respondents.ContainsKey(idRespondent))
                this.Respondents.Add(idRespondent, new KeyValuePair<string, double>(textAnswer, numericAnswer));

            if (!this.VariableRespondents.ContainsKey(idRespondent))
                this.VariableRespondents.Add(idRespondent, new object[2]);

            //this.VariableRespondents[idRespondent]++;
            this.VariableRespondents[idRespondent][0] = numericAnswer;
            this.VariableRespondents[idRespondent][1] = textAnswer;
        }

        public void Clear()
        {
            this.Respondents.Clear();
            this.CategoryData.Clear();
        }

        #endregion


        #region Operators


        public Dictionary<Guid, int> this[Guid idCategory]
        {
            get
            {
                if (!this.CategoryData.ContainsKey(idCategory))
                    return new Dictionary<Guid, int>();

                return this.CategoryData[idCategory];
            }
        }

        #endregion
    }
}
