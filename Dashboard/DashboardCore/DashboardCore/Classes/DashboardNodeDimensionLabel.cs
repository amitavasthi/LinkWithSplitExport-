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
    public class DashboardNodeDimensionLabel : DashboardNode
    {
        #region Properties

        public Guid Identity { get; set; }

        public string Variable { get; set; }

        public bool Repeat { get; set; }

        public bool RenderContainer { get; set; }

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
        public DashboardNodeDimensionLabel(Dashboard dashboard, XmlNode xmlNode)
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
        public DashboardNodeDimensionLabel(Dashboard dashboard, XmlNode xmlNode, DashboardNode parent)
            : base(dashboard, xmlNode, parent)
        { }

        #endregion


        #region Methods

        protected override void ParseNode()
        {
            this.Identity = Guid.NewGuid();
            this.Variable = this.XmlNode.Attributes["Variable"].Value;
            this.RenderContainer = true;

            if (this.XmlNode.Attributes["RenderContainer"] != null)
                this.RenderContainer = bool.Parse(this.XmlNode.Attributes["RenderContainer"].Value);

            if (!this.Dashboard.Cache.Variables.ContainsKey(this.Variable))
            {
                throw new Exception(string.Format(
                    "Variable with the name '{0}' doesn't exist.",
                    this.Variable
                ));
            }

            this.Repeat = true;

            if (this.XmlNode.Attributes["Repeat"] != null)
                this.Repeat = bool.Parse(this.XmlNode.Attributes["Repeat"].Value);
        }

        public override void Render(
            StringBuilder result,
            DashboardRenderContext context,
            Data filter,
            WeightingFilterCollection weight
        )
        {
            string label = "";

            if (context.Categories.ContainsKey(this.Variable))
            {
                label = (string)this.Dashboard.Cache.CategoryLabels[context.Categories[this.Variable]][0][1];
            }

            if (this.Repeat == false && context.LastRenderedLabel.ContainsKey(this.Identity))
            {
                if (context.LastRenderedLabel[this.Identity] == label)
                    return;
            }

            if (!context.LastRenderedLabel.ContainsKey(this.Identity))
                context.LastRenderedLabel.Add(this.Identity, "");

            context.LastRenderedLabel[this.Identity] = label;

            if (this.RenderContainer)
            {
                result.Append(string.Format(
                    "<span id=\"r_{0}\">",
                    this.Dashboard.IdCounter++
                ));
            }

            result.Append(label);

            if (this.RenderContainer)
            {
                result.Append("</span>");
            }
        }

        public override void RenderDataUpdate(
            StringBuilder result,
            DashboardRenderContext context,
            Data filter,
            WeightingFilterCollection weight,
            string path
        )
        {
            result.Append("{ \"Path\": \"");
            //result.Append(path);
            result.Append(this.Dashboard.IdCounter++);
            result.Append("\", \"Value\": \"");

            string label = "";

            if (context.Categories.ContainsKey(this.Variable))
            {
                label = (string)this.Dashboard.Cache.CategoryLabels[context.Categories[this.Variable]][0][1];
            }

            result.Append(label);

            result.Append("\" },");
        }

        #endregion
    }
}
