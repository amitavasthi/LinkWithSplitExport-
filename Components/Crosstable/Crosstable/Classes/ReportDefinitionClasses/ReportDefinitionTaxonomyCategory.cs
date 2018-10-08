using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class ReportDefinitionTaxonomyCategory : ReportDefinitionScore
    {
        #region Properties

        public override Guid Identity
        {
            get
            {
                return this.IdCategory;
            }
        }

        /// <summary>
        /// Gets or sets the id of the category.
        /// </summary>
        public Guid IdCategory
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
                    object name = this.Variable.Owner.Core.TaxonomyCategories.GetValue(
                        "Name",
                        "Id",
                        this.Identity
                    );

                    if (name != null)
                        return (string)name;

                    return "";
                }
            }
        }

        /// <summary>
        /// Gets the taxonomy category's label in the report
        /// definition's settings defined language.
        /// </summary>
        public override string _Label
        {
            get
            {
                string result = (string)this.Variable.Owner.Core.TaxonomyCategoryLabels.GetValue(
                    "Label",
                    new string[] { "IdTaxonomyCategory", "IdLanguage" },
                    new object[] { this.IdCategory, this.Variable.Owner.Settings.IdLanguage }
                );

                if (result == null)
                    result = "";

                return result;
            }
        }

        public override string _LabelValues
        {
            get
            {
                string result = (string)this.Variable.Owner.Core.TaxonomyCategoryLabels.GetValue(
                    "Label",
                    new string[] { "IdTaxonomyCategory", "IdLanguage" },
                    new object[] { this.IdCategory, this.Variable.Owner.Settings.IdLanguage }
                );

                if (result == null)
                    result = "";

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
                    /*if (this.Variable.Owner.SignificanceTest && this.Variable.ParentVariable == null)
                        result += 2;
                    else
                        result += 1;*/
                    // Check if sig diff is enabled.
                    if (this.Variable.Owner.SignificanceTest)
                        result += 1;

                    if (this.Variable.Owner.Settings.ShowValues)
                        result += 1;
                }

                return result;
            }
        }

        public override double Factor
        {
            get
            {
                double result = 0;

                if (this.IsFake)
                    return result;

                if (this.XmlNode.Attributes["Value"] != null)
                    return int.Parse(this.XmlNode.Attributes["Value"].Value);

                result = (double)this.Variable.Owner.Core.TaxonomyCategories.GetValue(
                    "Value",
                    "Id",
                    this.IdCategory
                );

                return result;
            }
        }

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

        public override bool ScoreBase
        {
            get
            {
                return true;
            }
        }

        public override string Equation
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        #endregion


        #region Constructor

        public ReportDefinitionTaxonomyCategory(ReportDefinitionVariable variable, XmlNode xmlNode)
            : base(variable, xmlNode)
        { }

        #endregion


        #region Methods

        public override List<string> BuildCommandTexts()
        {
            List<string> result = new List<string>();

            // Select all linked categories.
            List<object[]> categories;

            categories = this.Variable.Owner.Core.CategoryLinks.GetValues(
                new string[] { "IdCategory", "IdVariable" },
                new string[] { "IdTaxonomyCategory" },
                new object[] { this.IdCategory }
            );

            StringBuilder builder = new StringBuilder();

            foreach (object[] category in categories)
            {
                // Select the category's variable id.
                /*Guid idVariable = (Guid)this.Variable.Owner.Core.Categories.GetValue(
                    "IdVariable",
                    "Id",
                    (Guid)category[0]
                );*/

                builder.Append(string.Format(
                    "(SELECT IdRespondent FROM [resp].[Var_{0}] WHERE IdCategory='{1}') UNION ALL ",
                    (Guid)category[1],
                    (Guid)category[0]
                ));
            }

            if (categories.Count > 0)
                builder = builder.Remove(builder.Length - 11, 11);

            result.Add(builder.ToString());

            return result;
        }

        #endregion


        #region Event Handlers

        #endregion
    }
}
