using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class ReportDefinitionScoreGroup : ReportDefinitionScore
    {
        #region Properties

        public override string _Label
        {
            get
            {
                int idLanguage = this.Variable.Owner.Settings.IdLanguage;

                if (this.XmlNode.Attributes["Label" + idLanguage] == null)
                {
                    string result = (string)this.Variable.Owner.Core.TaxonomyCategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyCategory", "IdLanguage" },
                        new object[] { this.Identity, this.Variable.Owner.Settings.IdLanguage }
                    );

                    if (string.IsNullOrEmpty(result))
                    {
                        if (this.XmlNode.Attributes["Name"] != null)
                            return this.XmlNode.Attributes["Name"].Value;
                        else
                            return "";
                    }

                    return result;
                }
                else
                    return this.XmlNode.Attributes["Label" + idLanguage].Value;
            }
        }

        public override string _LabelValues
        {
            get
            {
                int idLanguage = this.Variable.Owner.Settings.IdLanguage;

                if (this.XmlNode.Attributes["Label" + idLanguage] == null)
                {
                   
                    return null;
                }
                else
                    return this.XmlNode.Attributes["Label" + idLanguage].Value;
            }
        }

        public override Guid Identity
        {
            get
            {
                return Guid.Parse(this.XmlNode.Attributes["Id"].Value);
            }
        }

        /// <summary>
        /// Gets all the scores of the score group.
        /// </summary>
        public List<ReportDefinitionScore> Scores { get; set; }

        public override string Name
        {
            get
            {
                if (this.XmlNode.Attributes["Name"] != null)
                {
                    return this.XmlNode.Attributes["Name"].Value;
                }
                else
                {
                    return (string)this.Variable.Owner.Core.TaxonomyCategories.GetValue(
                        "Name",
                        "Id",
                        this.Identity
                    );
                }
            }
        }

        public override double Factor
        {
            get
            {
                double result = 0;

                // Run through all child scores.
                foreach (ReportDefinitionScore score in this.Scores)
                {
                    result += score.Factor;
                }

                return result;
            }
        }

        public override int ColumnSpan
        {
            get
            {
                int result = 0;

                if (this.Variable.NestedVariables.Count > 0)
                {
                    // Run through all nested variables.
                    foreach (ReportDefinitionVariable nestedVariable in this.Variable.NestedVariables)
                    {
                        result += nestedVariable.ColumnSpan;
                    }
                }
                else
                {
                    result += 1;
                }

                return result;
            }
        }

        public override int RowSpan
        {
            get
            {
                int result = 0;

                if (this.Variable.NestedVariables.Count > 0)
                {
                    // Run through all nested variables.
                    foreach (ReportDefinitionVariable nestedVariable in this.Variable.NestedVariables)
                    {
                        result += nestedVariable.RowSpan;
                    }
                }
                else
                {
                    // Check if sig diff is enabled.
                    /*if (this.Variable.Owner.SignificanceTest)
                        result += 3;
                    else
                        result += 2;*/
                    if (this.Variable.Owner.SignificanceTest)
                        result += 1;

                    if (this.Variable.Owner.Settings.ShowValues)
                        result += 1;

                    if (this.XmlNode.Attributes["RenderPercentage"] != null && bool.Parse(this.XmlNode.Attributes["RenderPercentage"].Value) == false)
                    {

                    }
                    else
                    {
                        if (this.Variable.Owner.Settings.ShowPercentage)
                            result += 1;
                    }
                }

                return result;
            }
        }

        public override int RowSpanMean
        {
            get
            {
                int result = 0;

                if (this.Variable.NestedVariables.Count > 0)
                {
                    // Run through all nested variables.
                    foreach (ReportDefinitionVariable nestedVariable in this.Variable.NestedVariables)
                    {
                        result += nestedVariable.RowSpanMean;
                    }
                }
                else
                {
                    // Check if sig diff is enabled.
                    if (this.Variable.Owner.SignificanceTest && this.Variable.ParentVariable == null)
                        result += 2;
                    else
                        result += 1;
                }

                return result;
            }
        }

        public override bool ScoreBase
        {
            get
            {
                return false;
            }
        }

        public override string Equation
        {
            get
            {
                if (this.XmlNode.Attributes["Equation"] == null)
                {
                    string equation = (string)base.Variable.Owner.Core.TaxonomyCategories.GetValue(
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

        #endregion


        #region Constructor

        public ReportDefinitionScoreGroup(ReportDefinitionVariable variable, XmlNode xmlNode)
            : base(variable, xmlNode)
        {
            this.Scores = new List<ReportDefinitionScore>();

            // Run through all child nodes of the xml node.
            foreach (XmlNode xmlNodeScore in this.XmlNode.ChildNodes)
            {
                ReportDefinitionScore score = null;

                switch (xmlNodeScore.Name)
                {
                    case "Category":
                        score = new ReportDefinitionCategory(this.Variable, xmlNodeScore);
                        break;
                    case "TaxonomyCategory":
                        score = new ReportDefinitionTaxonomyCategory(this.Variable, xmlNodeScore);
                        break;
                    case "ScoreGroup":
                        score = new ReportDefinitionScoreGroup(this.Variable, xmlNodeScore);
                        break;
                }

                if (score != null)
                    this.Scores.Add(score);
            }
        }

        #endregion


        #region Methods

        public override List<string> BuildCommandTexts()
        {
            List<string> result = new List<string>();

            // Run through all child scores.
            foreach (ReportDefinitionScore score in this.Scores)
            {
                result.AddRange(score.BuildCommandTexts());
            }

            return result;
        }

        #endregion
    }
}
