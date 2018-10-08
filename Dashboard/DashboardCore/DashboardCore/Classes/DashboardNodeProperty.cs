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
    public class DashboardNodeProperty : DashboardNode
    {
        #region Properties

        public DashboardPropertyType Type { get; set; }

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
        public DashboardNodeProperty(Dashboard dashboard, XmlNode xmlNode)
            : base(dashboard, xmlNode)
        { }

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
        public DashboardNodeProperty(Dashboard dashboard, XmlNode xmlNode, DashboardNode parent)
            : base(dashboard, xmlNode, parent)
        { }

        #endregion


        #region Methods

        protected override void ParseNode()
        {
            this.Type = (DashboardPropertyType)Enum.Parse(
                typeof(DashboardPropertyType),
                this.XmlNode.Attributes["Type"].Value
            );
        }

        public override void Render(
            StringBuilder result,
            DashboardRenderContext context,
            Data filter,
            WeightingFilterCollection weight
        )
        {
            StringBuilder value = new StringBuilder();

            // Run through all child dashboard nodes.
            foreach (DashboardNode children in base.Children)
            {
                children.Render(
                    value,
                    context,
                    filter,
                    weight
                );
            }

            this.Dashboard.Properties[this.Type] = value.ToString().Trim();
        }

        #endregion
    }

    public enum DashboardPropertyType
    {
        Title,
        ExportTitle
    }
}
