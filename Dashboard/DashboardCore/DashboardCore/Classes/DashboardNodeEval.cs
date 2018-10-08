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
    public class DashboardNodeEval : DashboardNode
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
        public DashboardNodeEval(Dashboard dashboard, XmlNode xmlNode)
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
        public DashboardNodeEval(Dashboard dashboard, XmlNode xmlNode, DashboardNode parent)
            : base(dashboard, xmlNode, parent)
        { }

        #endregion


        #region Methods

        protected override void ParseNode()
        { }

        public override void Render(
            StringBuilder result,
            DashboardRenderContext context,
            Data filter,
            WeightingFilterCollection weight
        )
        {
            if (this.XmlNode.NodeType == XmlNodeType.Text)
            {
                string text = ReplacePlaceholder(this.XmlNode.InnerText, context);

                result.Append(text);
                return;
            }

            // Render the opening tag of the html node.
            result.Append("<");
            result.Append(this.XmlNode.Name);

            // Run through all attributes of the html node.
            foreach (XmlAttribute xmlAttribute in this.XmlNode.Attributes)
            {
                // Render the html attribute.
                result.Append(string.Format(
                    " {0}=\"{1}\"",
                    xmlAttribute.Name,
                    ReplacePlaceholder(xmlAttribute.Value, context)
                ));
            }

            result.Append(">");

            // Run through all child dashboard nodes.
            foreach (DashboardNode children in base.Children)
            {
                children.Render(result, context, filter, weight);
            }

            // Render the closing tag of the html node.
            result.Append(string.Format(
                "</{0}>",
                this.XmlNode.Name
            ));
        }

        public override void RenderDataUpdate(
            StringBuilder result, 
            DashboardRenderContext context, 
            Data filter, 
            WeightingFilterCollection weight, 
            string path
        )
        {
            if (this.XmlNode.NodeType == XmlNodeType.Text)
            {
                base.RenderDataUpdate(result, context, filter, weight, path);
                return;
            }

            path += this.XmlNode.Name + string.Format(
                ":nth-child({0}) ",
                this.XmlNode.ParentNode.ChildNodes.IndexOf(this.XmlNode)
            );

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

        private string ReplacePlaceholder(string source, DashboardRenderContext context)
        {
            while (source.Contains("###CATEGORYNAME_"))
            {
                string variableName = source.Split(new string[]
                {
                        "###CATEGORYNAME_"
                }, StringSplitOptions.None)[1].Split('#')[0];

                if (context.Categories.ContainsKey(variableName))
                {
                    source = source.Replace(string.Format(
                        "###CATEGORYNAME_{0}###", variableName
                    ), (string)this.Dashboard.Cache.CategoryLabels[context.Categories[variableName]][0][1]);
                }
                else
                {
                    source = source.Replace(string.Format(
                        "###CATEGORYNAME_{0}###", variableName
                    ), "undefined");
                }
            }

            return source;
        }

        #endregion
    }
}
