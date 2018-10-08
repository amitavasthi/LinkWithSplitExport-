using Crosstables.Classes.ReportDefinitionClasses.Collections;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DashboardCore.Classes
{
    public abstract class DashboardNode
    {
        #region Properties

        /// <summary>
        /// Gets or sets the parent dashboard node.
        /// </summary>
        public DashboardNode Parent { get; set; }

        /// <summary>
        /// Gets or sets a list of all child dashboard nodes.
        /// </summary>
        public List<DashboardNode> Children { get; set; }

        /// <summary>
        /// Gets or sets the owning dashboard.
        /// </summary>
        public Dashboard Dashboard { get; set; }

        /// <summary>
        /// Gets or sets the xml node that
        /// contains the dashboard node definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the dashboard node.
        /// </summary>
        /// <param name="dashboard">
        /// The owning dashboard.
        /// </param>
        /// <param name="xmlNode">
        /// The xml node that contains the
        /// dashboard node definition.
        /// </param>
        public DashboardNode(Dashboard dashboard, XmlNode xmlNode)
        {
            this.Children = new List<DashboardNode>();
            this.Dashboard = dashboard;
            this.XmlNode = xmlNode;
        }

        /// <summary>
        /// Creates a new instance of the dashboard node.
        /// </summary>
        /// <param name="dashboard">
        /// The owning dashboard.
        /// </param>
        /// <param name="xmlNode">
        /// The xml node that contains the
        /// dashboard node definition.
        /// </param>
        /// <param name="parent">
        /// The parent dashboard node.
        /// </param>
        public DashboardNode(Dashboard dashboard, XmlNode xmlNode, DashboardNode parent)
            : this(dashboard, xmlNode)
        {
            this.Parent = parent;
        }

        #endregion


        #region Methods

        public virtual void ParseChildren()
        {
            // Run through all child xml nodes.
            foreach (XmlNode xmlNode in this.XmlNode.ChildNodes)
            {
                // Parse the dashboard node and add
                // it to the dashboard node's children.
                this.Children.Add(this.Dashboard.ParseDashboardNode(
                    xmlNode,
                    this
                ));

                this.Children[this.Children.Count - 1].ParseChildren();
            }
        }

        public virtual void PreParse()
        {
            foreach (DashboardNode node in this.Children)
            {
                node.PreParse();
            }
        }

        public void Parse()
        {
            this.ParseNode();
            foreach (DashboardNode node in this.Children)
            {
                node.Parse();
            }
        }

        /// <summary>
        /// Parses the dashboard node from the definition xml node.
        /// </summary>
        protected abstract void ParseNode();

        /// <summary>
        /// Renders the dashboard node to the result html string.
        /// </summary>
        /// <param name="result">
        /// The string builder that holds the result html string.
        /// </param>
        public abstract void Render(
            StringBuilder result,
            DashboardRenderContext context,
            Data filter,
            WeightingFilterCollection weight
        );

        public virtual void RenderDataUpdate(
            StringBuilder result,
            DashboardRenderContext context,
            Data filter,
            WeightingFilterCollection weight,
            string path
        )
        {
            foreach (DashboardNode node in this.Children)
            {
                node.RenderDataUpdate(
                    result,
                    context,
                    filter,
                    weight,
                    path
                );
            }
        }

        #endregion
    }
}
