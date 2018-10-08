using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebUtilities.Controls;

namespace Crosstables.Classes.WorkflowClasses
{
    /// <summary>
    /// This will turn into the base class.
    /// </summary>
    public class WorkflowSelectionVariable : WorkflowSelectionSelector
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the variable of
        /// the workflow selection variable.
        /// </summary>
        public Guid IdVariable
        {
            get
            {
                return Guid.Parse(this.XmlNode.Attributes["IdVariable"].Value);
            }
            set
            {
                this.XmlNode.Attributes["IdVariable"].Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets if the variable is a taxonomy variable.
        /// </summary>
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

        #endregion


        #region Constructor

        public WorkflowSelectionVariable(WorkflowSelection owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;

            this.Load += WorkflowSelectionVariable_Load;

            if (this.XmlNode.Attributes["IdVariable"] == null &&
                this.XmlNode.Attributes["VariableName"] != null)
            {
                Guid? idVariable = null;

                if (!this.IsTaxonomy)
                {
                    object _idVariable = this.Owner.Owner.Core.Variables.GetValue(
                        "Id",
                        "Name",
                        this.XmlNode.Attributes["VariableName"].Value
                    );
                    if (_idVariable != null)
                        idVariable = (Guid)_idVariable;
                }
                else
                {

                    object _idVariable = this.Owner.Owner.Core.TaxonomyVariables.GetValue(
                        "Id",
                        "Name",
                        this.XmlNode.Attributes["VariableName"].Value
                    );

                    if (_idVariable != null)
                        idVariable = (Guid)_idVariable;
                }

                if (idVariable != null)
                    this.XmlNode.AddAttribute("IdVariable", idVariable);
                else
                    this.XmlNode.AddAttribute("IdVariable", new Guid());
            }

            Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            // Create a new workflow selector for the variable.
            this.Selector = new WorkflowSelector(this);

            List<object[]> categories;

            if (!this.IsTaxonomy)
            {
                categories = this.Owner.Owner.Core.Categories.GetValues(
                    new string[] { "Id" },
                    new string[] { "IdVariable" },
                    new object[] { this.IdVariable },
                    "Value"
                );
            }
            else
            {
                categories = this.Owner.Owner.Core.TaxonomyCategories.GetValues(
                    new string[] { "Id" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { this.IdVariable },
                    "Value"
                );
            }

            // Run through all categories of the variable.
            foreach (object[] category in categories)
            {
                string categoryLabel;

                if (!this.IsTaxonomy)
                {
                    categoryLabel = (string)this.Owner.Owner.Core.CategoryLabels.GetValue(
                        "Label",
                        "IdCategory",
                        category[0]
                    );
                }
                else
                {
                    if (!this.Owner.Owner.HierarchyFilter.TaxonomyCategories.ContainsKey((Guid)category[0]))
                        continue;

                    categoryLabel = (string)this.Owner.Owner.Core.TaxonomyCategoryLabels.GetValue(
                        "Label",
                        "IdTaxonomyCategory",
                        category[0]
                    );
                }

                // Create a new workflow selector item for the category.
                WorkflowSelectorItem selectorItem = new WorkflowSelectorItem(this.Selector);
                selectorItem.Id = (Guid)category[0];
                selectorItem.Text = categoryLabel;

                // Add the workflow selector item to the workflow selector's items.
                this.Selector.Items.Add(selectorItem);
            }

            // Run through all selected category xml nodes.
            foreach (XmlNode xmlNodeSelected in this.XmlNode.ChildNodes)
            {
                Guid idCategory = Guid.Parse(
                    xmlNodeSelected.Attributes["Id"].Value
                );

                if (this.IsTaxonomy && this.Owner.Owner.HierarchyFilter.TaxonomyCategories.ContainsKey(idCategory) == false)
                    continue;

                this.Selector.SelectedItems.Add(idCategory);
            }
        }

        #endregion


        #region Event Handlers

        protected void WorkflowSelectionVariable_Load(object sender, EventArgs e)
        {
            this.CssClass = "WorkflowSelectionVariable";

            base.Controls.Add(this.Selector);
        }

        #endregion
    }
}
