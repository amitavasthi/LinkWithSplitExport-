using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataCore.Classes
{
    public class Filter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database core which is used.
        /// </summary>
        public DatabaseCore.Core Core { get; set; }

        /// <summary>
        /// Gets or sets the filter expressions of the filter definition.
        /// </summary>
        public List<FilterExpression> FilterExpressions { get; set; }

        #endregion


        #region Constructor

        public Filter(DatabaseCore.Core core)
        {
            this.Core = core;
        }

        public Filter(DatabaseCore.Core core, XmlNode xmlNode)
            : this(core)
        {
            // Parse all filter expressions of the filter definition.
            this.FilterExpressions = ParseFilterExpressions(xmlNode);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Parses all filter expressions from an xml node.
        /// </summary>
        /// <param name="xmlNode">The parent xml node containing the filter expressions.</param>
        internal List<FilterExpression> ParseFilterExpressions(XmlNode xmlNode)
        {
            List<FilterExpression> result = new List<FilterExpression>();

            // Select all children xml nodes.
            XmlNodeList xmlNodesChildren = xmlNode.ChildNodes;

            // Run through all children xml nodes.
            foreach (XmlNode xmlNodeChild in xmlNodesChildren)
            {
                // Check if the xml node is a child xml node.
                if (xmlNodeChild.Name == "Category")
                    continue;

                // Create a new filter expression by the child xml node.
                FilterExpression child = new FilterExpression(this, xmlNodeChild);

                // Add the filter expression to the child filter expressions.
                result.Add(child);
            }

            return result;
        }

        /// <summary>
        /// Builds the sql selection string of the filter definition.
        /// </summary>
        public string BuildSqlSelection()
        {
            // Create a new string builder containing the result sql selection.
            StringBuilder result = new StringBuilder();

            // Run through all filter expressions.
            foreach (FilterExpression filterExpression in this.FilterExpressions)
            {

            }

            // Return the contents of the result's string builder.
            return result.ToString();
        }

        #endregion
    }
}
