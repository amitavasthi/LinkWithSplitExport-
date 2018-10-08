using Crosstables.Classes.ReportDefinitionClasses;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LinkBi1.Classes
{
    public abstract class LinkBiDefinitionDimension
    {
        #region Properties

        public LinkBiDefinition Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains the definitions for the filter.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets a list of the filter's filter scores.
        /// </summary>
        public List<LinkBiDefinitionDimensionScore> Scores { get; set; }

        public VariableType Type
        {
            get
            {
                return (VariableType)int.Parse(this.XmlNode.Attributes["Type"].Value);
            }
        }

        public abstract Guid Identity { get; }

        public abstract string Label { get; }

        public abstract VariableType VariableType { get; }

        public abstract string Name { get; }

        public abstract bool IsTaxonomy { get; }

        public bool New
        {
            get
            {
                if (this.XmlNode.Attributes["New"] == null)
                    return false;

                return true;
            }
            set
            {
                if (value == false && this.XmlNode.Attributes["New"] != null)
                {
                    this.XmlNode.Attributes.Remove(this.XmlNode.Attributes["New"]);

                    return;
                }

                if (this.XmlNode.Attributes["New"] == null)
                    this.XmlNode.AddAttribute("New", true.ToString());
                else
                    this.XmlNode.Attributes["New"].Value = value.ToString();
            }
        }


        /*public VariableData Data { get; set; }*/

        #endregion


        #region Constructor

        public LinkBiDefinitionDimension(LinkBiDefinition owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;
            this.Scores = new List<LinkBiDefinitionDimensionScore>();

            Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            // Run through all score xml nodes of the filter xml node.
            foreach (XmlNode xmlNodeScore in this.XmlNode.ChildNodes.OrderByNumeric("Order"))
            {
                LinkBiDefinitionDimensionScore score = null;

                switch (xmlNodeScore.Name)
                {
                    case "TaxonomyCategory":
                        // Create a new taxonomy category as score by the xml node.
                        score = new LinkBiDefinitionTaxonomyCategory(this, xmlNodeScore);
                        break;
                    case "Category":
                        // Create a new taxonomy category as score by the xml node.
                        score = new LinkBiDefinitionCategory(this, xmlNodeScore);
                        break;
                    case "ScoreGroup":
                        score = new LinkBiDefinitionScoreGroup(this, xmlNodeScore);
                        break;
                }

                // Check if it was possible to parse the score.
                if (score == null)
                    continue;

                if (score.Hidden)
                    continue;

                // Add the filter score to the filter's filter scores.
                this.Scores.Add(score);
            }
        }

        protected string GetValue(string field)
        {
            // Check if the field is present in the xml node's attributes.
            if (this.XmlNode.Attributes[field] == null)
                return null;

            // Return the value of the field stored
            // in the xml node's xml attributes.
            return this.XmlNode.Attributes[field].Value;
        }


        /*public void InitData(
            Dictionary<Guid, Dictionary<Guid, object[]>> variableData = null,
            Dictionary<Guid, Dictionary<Guid, Dictionary<Guid, int>>> categoryData = null
        )
        {
            this.Data = new VariableData();

            if (variableData != null && variableData.ContainsKey(this.Identity))
            {
                this.Data.VariableRespondents = variableData[this.Identity];
                this.Data.CategoryData = categoryData[this.Identity];

                return;
            }

            KeyValuePair<List<string>, Dictionary<Guid, Guid>> commandTexts = BuildCommandTexts();

            foreach (string commandText in commandTexts.Key)
            {
                List<object[]> values;

                if (this.Owner.Core.CaseDataLocation == ApplicationUtilities.Classes.CaseDataLocation.Sql)
                {
                    values = this.Owner.Core.Responses[this.Identity].ExecuteReader(commandText);
                }
                else
                {
                    CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();
                    values = dataLink.Select2(commandText, this.Owner.Core.ClientName);
                }

                if (this.VariableType == VariableType.Numeric)
                {
                    foreach (object[] value in values)
                    {
                        this.Data.Add((Guid)value[0], "", (double)value[1]);
                    }
                }
                else if (this.VariableType == VariableType.Text)
                {
                    foreach (object[] value in values)
                    {
                        this.Data.Add((Guid)value[0], (string)value[1], 0);
                    }
                }
                else
                {
                    foreach (object[] value in values)
                    {
                        Guid idCategory = (Guid)value[1];

                        if (commandTexts.Value.ContainsKey(idCategory))
                            idCategory = commandTexts.Value[idCategory];

                        this.Data.Add(idCategory, (Guid)value[0]);
                    }
                }
            }
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
                           new object[] { this.Identity }
                        );

                    foreach (object[] linkedVariable in linkedVariables)
                    {
                        result.Append(string.Format(
                            "SELECT [IdRespondent], [{1}Answer] FROM [resp].[Var_{0}] UNION ALL ",
                            ((Guid)linkedVariable[0]),
                            this.VariableType == VariableType.Numeric ? "Numeric" : "Text"
                        ));
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
                            new object[] { this.Identity }
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
                        this.Identity,
                        string.Join(",", this.Owner.Core.Categories.GetValues(
                            new string[] { "Id" },
                            new string[] { "IdVariable", "ExcludeBase" },
                            new object[] { this.Identity, false }
                        ).Select(x => "'" + x[0] + "'"))
                    ));
                }
                else
                {
                    commandTexts.Add(string.Format(
                        "SELECT [IdRespondent], [IdCategory] FROM [resp].[Var_{0}]",
                        this.Identity
                    ));
                }
            }

            return new KeyValuePair<List<string>, Dictionary<Guid, Guid>>(commandTexts, categoryLinks);
        }

        #endregion
    }
}
