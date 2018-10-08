using DataCore.Classes;
using DataCore.Classes.StorageMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebUtilities.Controls;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class FilterCategory
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning filter category operator.
        /// </summary>
        public FilterCategoryOperator Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains
        /// the filter category definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the id of the filter category's category.
        /// </summary>
        public Guid IdCategory { get; set; }

        /// <summary>
        /// Gets or sets the type of the filter category.
        /// </summary>
        public FilterCategoryType Type { get; set; }

        /// <summary>
        /// Gets the label of the filter category.
        /// </summary>
        public string Label
        {
            get
            {
                string result = "";

                if (this.Type == FilterCategoryType.Category)
                {
                    result = (string)this.Owner.Owner.Core.CategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdCategory", "IdLanguage" },
                        new object[] { this.IdCategory, this.Owner.Owner.Owner.Settings.IdLanguage }
                    );
                }
                else
                {
                    result = (string)this.Owner.Owner.Core.TaxonomyCategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyCategory", "IdLanguage" },
                        new object[] { this.IdCategory, this.Owner.Owner.Owner.Settings.IdLanguage }
                    );
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the id of the category's variable.
        /// </summary>
        public Guid? IdVariable
        {
            get
            {
                object result;

                if (this.Type == FilterCategoryType.Category)
                {
                    result = this.Owner.Owner.Core.Categories.GetValue(
                        "IdVariable",
                        "Id",
                        this.IdCategory
                    );
                }
                else
                {
                    result = this.Owner.Owner.Core.TaxonomyCategories.GetValue(
                        "IdTaxonomyVariable",
                        "Id",
                        this.IdCategory
                    );
                }

                if (result == null)
                    return null;

                return (Guid)result;
            }
        }

        /// <summary>
        /// Gets the label of the category's variable.
        /// </summary>
        public string VariableLabel
        {
            get
            {
                string result = "";

                if (this.Type == FilterCategoryType.Category)
                {
                    object idVariable = this.Owner.Owner.Core.Categories.GetValue("IdVariable", "Id", this.IdCategory);

                    if (idVariable == null)
                        return result;

                    result = (string)this.Owner.Owner.Core.VariableLabels.GetValue(
                        "Label",
                        new string[] { "IdVariable", "IdLanguage" },
                        new object[] { idVariable, this.Owner.Owner.Owner.Settings.IdLanguage }
                    );
                }
                else
                {
                    object idVariable = this.Owner.Owner.Core.TaxonomyCategories.GetValue("IdTaxonomyVariable", "Id", this.IdCategory);

                    if (idVariable == null)
                        return result;

                    result = (string)this.Owner.Owner.Core.TaxonomyVariableLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyVariable", "IdLanguage" },
                        new object[] { idVariable, this.Owner.Owner.Owner.Settings.IdLanguage }
                    );
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the label of the category.
        /// </summary>
        public string CategoryLabel
        {
            get
            {
                string result = "";

                if (this.Type == FilterCategoryType.Category)
                {
                    result = (string)this.Owner.Owner.Core.CategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdCategory", "IdLanguage" },
                        new object[] { this.IdCategory, this.Owner.Owner.Owner.Settings.IdLanguage }
                    );
                }
                else
                {
                    result = (string)this.Owner.Owner.Core.TaxonomyCategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyCategory", "IdLanguage" },
                        new object[] { this.IdCategory, this.Owner.Owner.Owner.Settings.IdLanguage }
                    );
                }

                return result;
            }
        }

        #endregion


        #region Constructor

        public FilterCategory(FilterCategoryOperator owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;

            // Parse the filter category definition.
            Parse();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Parses the filter category definition from the xml node.
        /// </summary>
        private void Parse()
        {
            // Parse the id of the filter category's category.
            this.IdCategory = Guid.Parse(
                this.XmlNode.Attributes["Id"].Value
            );

            // Parse the type of the filter category.
            this.Type = this.XmlNode.Name == "Category" ?
                FilterCategoryType.Category :
                FilterCategoryType.TaxonomyCategory;
        }

        public override string ToString()
        {
            return string.Format(
                "[<b>{0}:</b> {1}]",
                this.VariableLabel,
                this.CategoryLabel
            );
        }

        public Panel RenderControl()
        {
            Panel result = new Panel();

            System.Web.UI.WebControls.Label lblVariable = new System.Web.UI.WebControls.Label();

            lblVariable.Text = this.VariableLabel + ": ";

            result.Controls.Add(lblVariable);

            System.Web.UI.WebControls.Label lblCategory = new System.Web.UI.WebControls.Label();

            lblCategory.Text = this.Label;

            result.Controls.Add(lblCategory);

            Image imgDeleteFilterCategory = new Image();
            imgDeleteFilterCategory.Style.Add("cursor", "pointer");
            imgDeleteFilterCategory.ID = this.Owner.Identity + this.IdCategory;
            imgDeleteFilterCategory.ImageUrl = "/Images/Icons/Delete2.png";
            //imgDeleteFilterCategory.Click += imgDeleteFilterCategory_Click;
            imgDeleteFilterCategory.Attributes.Add("onclick", string.Format(
                "DeleteFilterCategory('{0}', '{1}', '{2}')",
                this.Owner.FileName,
                this.Owner.XmlNode.GetXPath(),
                this.IdCategory
            ));

            result.Controls.Add(imgDeleteFilterCategory);

            return result;
        }

        public Data GetRespondents(Database storageMethod, Data filter)
        {
            if (this.IdVariable.HasValue == false)
                return filter;

            if (this.Type == FilterCategoryType.Category)
            {
                return storageMethod.GetRespondents(
                    this.IdCategory,
                    this.IdVariable.Value,
                    false,
                    this.Owner.Owner.Core.CaseDataLocation,
                    filter,
                    null,
                    null
                );
            }
            else
            {
                List<object[]> categories = storageMethod.Core.CategoryLinks.GetValues(
                    new string[] { "IdCategory" },
                    new string[] { "IdTaxonomyCategory" },
                    new object[] { this.IdCategory }
                );

                Data result = new Data();

                Dictionary<Guid, List<object[]>> c = storageMethod.Core.Categories.ExecuteReaderDict<Guid>(
                    "SELECT Id, IdVariable FROM Categories",
                    new object[] {  }
                );

                foreach (object[] idCategory in categories)
                {
                    // Get the id of the category's variable.
                    /*Guid idVariable = (Guid)storageMethod.Core.Categories.GetValue(
                        "IdVariable",
                        "Id",
                        (Guid)idCategory[0]
                    );*/
                    Guid idVariable = (Guid)c[(Guid)idCategory[0]][0][1];

                    Data categoryRespondents = storageMethod.GetRespondents(
                        (Guid)idCategory[0],
                        idVariable,
                        false,
                        this.Owner.Owner.Owner.Core.CaseDataLocation,
                        filter,
                        null,
                        null
                    );

                    // Run through all respondents of the category.
                    foreach (KeyValuePair<Guid, double[]> respondent in categoryRespondents.Responses)
                    {
                        if (result.Responses.ContainsKey(respondent.Key))
                            continue;

                        result.Responses.Add(respondent.Key, respondent.Value);
                    }
                }

                if (filter != null)
                {
                    List<Guid> removeRespondents = new List<Guid>();

                    // Run through all respondents of the result.
                    foreach (Guid idRespondent in result.Responses.Keys)
                    {
                        if (!filter.Responses.ContainsKey(idRespondent))
                            removeRespondents.Add(idRespondent);
                    }

                    foreach (Guid idRespondent in removeRespondents)
                    {
                        result.Responses.Remove(idRespondent);
                    }
                }

                return result;
            }
        }

        #endregion


        #region Event Handlers

        protected void imgDeleteFilterCategory_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.Owner.Delete(this);
        }

        #endregion
    }

    public enum FilterCategoryType
    {
        Category,
        TaxonomyCategory
    }
}
