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
    public class DashboardNodeDateTime : DashboardNode
    {
        #region Properties

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
        public DashboardNodeDateTime(Dashboard dashboard, XmlNode xmlNode)
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
        public DashboardNodeDateTime(Dashboard dashboard, XmlNode xmlNode, DashboardNode parent)
            : base(dashboard, xmlNode, parent)
        { }

        #endregion


        #region Methods

        protected override void ParseNode()
        {
        }

        public override void Render(
            StringBuilder result,
            DashboardRenderContext context,
            Data filter,
            WeightingFilterCollection weight
        )
        {
            DateTime value;

            if (this.XmlNode.Attributes["Value"].Value == "Now")
                value = DateTime.Now;
            else
                value = DateTime.Parse(this.XmlNode.Attributes["Value"].Value);

            if (this.XmlNode.Attributes["Format"] != null)
                result.Append(value.ToString(this.XmlNode.Attributes["Format"].Value));
            else
                result.Append(value.ToString());
        }

        #endregion
    }
}
