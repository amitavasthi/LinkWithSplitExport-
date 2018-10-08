using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataCore.Classes
{
    public class FilterExpression
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning filter object.
        /// </summary>
        public Filter Owner { get; set; }

        /// <summary>
        /// Gets or sets the type of the filter expression.
        /// </summary>
        public FilterExpressionType Type { get; set; }

        /// <summary>
        /// Gets or sets the child filter expressions.
        /// </summary>
        public List<FilterExpression> FilterExpressions { get; set; }

        /// <summary>
        /// Gets or sets all filter categories of the filter expression.
        /// </summary>
        public List<Category> FilterCategories { get; set; }

        #endregion


        #region Constructor

        public FilterExpression(Filter owner)
        {
            // Set the type to AND by default.
            this.Type = FilterExpressionType.And;

            // Create a new collection of child filter expressions.
            this.FilterExpressions = new List<FilterExpression>();

            // Create a new collection of filter categories.
            this.FilterCategories = new List<Category>();
        }

        public FilterExpression(Filter owner, XmlNode xmlNode)
            : this(owner)
        {
            // Parse the type of the filter
            // expression by the xml node's name.
            this.Type = (FilterExpressionType)Enum.Parse(
                typeof(FilterExpressionType),
                xmlNode.Name
            );

            // Parse all children.
            this.FilterExpressions = this.Owner.ParseFilterExpressions(xmlNode);

            // Parse the filter categories.
            ParseFilterCategories(xmlNode);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Parses all filter categories of the filter expression.
        /// </summary>
        /// <param name="xmlNode">The xml node containing the categories definition.</param>
        private void ParseFilterCategories(XmlNode xmlNode)
        {
            // Select all category xml nodes.
            XmlNodeList xmlNodesCategories = xmlNode.SelectNodes("Category");

            // Run through all category xml nodes.
            foreach (XmlNode xmlNodeCategory in xmlNodesCategories)
            {
                // Parse the id of the category.
                Guid idCategory = Guid.Parse(
                    xmlNodeCategory.Attributes["Id"].Value
                );

                // Get the category by the if from the database.
                Category category = this.Owner.Core.Categories.GetSingle(idCategory);

                // Add the category to the filter categories.
                this.FilterCategories.Add(category);
            }
        }

        #endregion
    }

    public enum FilterExpressionType
    {
        And,
        Or
    }
}
