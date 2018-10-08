using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class ReportDefinitionCategory : ReportDefinitionScore
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

        public Category Category
        {
            get
            {
                return this.Variable.Owner.Core.Categories.GetSingle(this.IdCategory);
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
                    return (string)this.Variable.Owner.Core.Categories.GetValue(
                        "Name",
                        "Id",
                        this.Identity
                    );
                }
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
                this.XmlNode.Attributes["IsTaxonomy"].Value = value.ToString();
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
                    if (this.Variable.Owner.Settings.SignificanceTest && this.Variable.ParentVariable == null)
                        result += 2;
                    else
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

                result = (double)this.Variable.Owner.Core.Categories.GetValue(
                    "Value",
                    "Id",
                    this.IdCategory
                );

                return result;
            }
        }

        /// <summary>
        /// Gets the category's label in the report
        /// definition's settings defined language.
        /// </summary>
        public override string _Label
        {
            get
            {
                string result = result = (string)this.Variable.Owner.Core.CategoryLabels.GetValue(
                    "Label",
                    new string[] { "IdCategory", "IdLanguage" },
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
                string result = result = (string)this.Variable.Owner.Core.CategoryLabels.GetValue(
                    "Label",
                    new string[] { "IdCategory", "IdLanguage" },
                    new object[] { this.IdCategory, this.Variable.Owner.Settings.IdLanguage }
                );

                if (result == null)
                    result = "";

                return result;
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

        public ReportDefinitionCategory(ReportDefinitionVariable variable, XmlNode xmlNode)
            : base(variable, xmlNode)
        {
        }

        #endregion


        #region Methods

        public override List<string> BuildCommandTexts()
        {
            List<string> result = new List<string>();

            result.Add(string.Format(
                "SELECT IdRespondent FROM [resp].[Var_{0}] WHERE IdCategory='{1}'",
                this.Variable.IdVariable,
                this.IdCategory
            ));

            return result;
        }

        #endregion
    }
}
