using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace LinkBi1.Classes
{
    public class LinkBiDefinitionScoreGroup : LinkBiDefinitionDimensionScore
    {
        #region Properties

        /// <summary>
        /// Gets the id of the taxonomy category.
        /// </summary>
        public override Guid Identity
        {
            get
            {
                return Guid.Parse(base.GetValue("Id"));
            }
        }

        public override string Label
        {
            get
            {
                return base.GetValue("Name");
            }
        }

        public override string Name
        {
            get
            {
                return base.GetValue("Name");
            }
        }

        public override string Equation
        {
            get
            {
                if (this.XmlNode.Attributes["Equation"] == null)
                {
                    string equation = (string)base.Owner.Owner.Core.TaxonomyCategories.GetValue(
                        "Equation",
                        "Id",
                        this.Identity
                    );

                    if (equation == null)
                        equation = "";

                    this.XmlNode.AddAttribute("Equation", equation);

                    return this.Equation;
                }

                if (this.XmlNode.Attributes["Equation"].Value.Length == 0)
                    return null;

                return HttpUtility.HtmlDecode(this.XmlNode.Attributes["Equation"].Value);
            }
            set
            {
                if (this.XmlNode.Attributes["Equation"] == null)
                    this.XmlNode.AddAttribute("Equation", value);
                else
                    this.XmlNode.Attributes["Equation"].Value = value;
            }
        }

        public Equation CompiledEquation { get; set; }

        public List<LinkBiDefinitionDimensionScore> Scores { get; set; }

        #endregion


        #region Constructor

        public LinkBiDefinitionScoreGroup(LinkBiDefinitionDimension owner, XmlNode xmlNode)
            : base(owner, xmlNode)
        {
            this.Scores = new List<LinkBiDefinitionDimensionScore>();

            foreach (XmlNode xmlNodeScore in this.XmlNode.ChildNodes)
            {
                LinkBiDefinitionDimensionScore score = null;

                switch (xmlNodeScore.Name)
                {
                    case "TaxonomyCategory":
                        // Create a new taxonomy category as score by the xml node.
                        score = new LinkBiDefinitionTaxonomyCategory(this.Owner, xmlNodeScore);
                        break;
                    case "Category":
                        // Create a new category as score by the xml node.
                        score = new LinkBiDefinitionCategory(this.Owner, xmlNodeScore);
                        break;
                    case "ScoreGroup":
                        score = new LinkBiDefinitionScoreGroup(this.Owner, xmlNodeScore);
                        break;
                }

                // Check if it was possible to parse the score.
                if (score == null)
                    continue;

                // Add the filter score to the filter's filter scores.
                this.Scores.Add(score);
            }
        }

        #endregion


        #region Methods

        public override Data GetRespondents(Data filter, DataCore.Classes.StorageMethods.Database storageMethod)
        {
            if (this.Equation != null)
            {
                if (this.CompiledEquation == null)
                {
                    this.CompiledEquation = new DataCore.Classes.Equation(
                        this.Owner.Owner.Core,
                        this.Equation,
                        storageMethod.WeightMissingValue
                    );
                }

                return storageMethod.GetRespondents(
                    this.Identity,
                    this.Owner.Identity,
                    this.Owner.IsTaxonomy,
                    this.Owner.Owner.Core.CaseDataLocation,
                    filter,
                    this.Owner.Owner.WeightingFilters,
                    this.CompiledEquation
                );
            }

            Data result = new Data();

            // Run through all scores of the score group.
            foreach (LinkBiDefinitionDimensionScore score in this.Scores)
            {
                Data scoreRespondents = score.GetRespondents(filter, storageMethod);

                // Run through all respondent ids of the score result.
                foreach (Guid idRespondent in scoreRespondents.Responses.Keys)
                {
                    if (result.Responses.ContainsKey(idRespondent))
                    {
                        result.Responses[idRespondent][0] += scoreRespondents.Responses[idRespondent][0];
                    }
                    else
                    {
                        result.Responses.Add(idRespondent, scoreRespondents.Responses[idRespondent]);

                        result.Base += scoreRespondents.Responses[idRespondent][0];

                        if (score.Owner.VariableType == DatabaseCore.Items.VariableType.Multi)
                        {

                            result.Value += scoreRespondents.Responses[idRespondent][0];
                        }
                    }
                    if (score.Owner.VariableType != DatabaseCore.Items.VariableType.Multi)
                    {
                        result.Value += scoreRespondents.Responses[idRespondent][0];
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
