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
    public class DashboardNodeMeasure : DashboardNode
    {
        #region Properties

        public string Variable { get; set; }

        public Guid? IdVariable { get; set; }

        public bool RenderContainer { get; set; }

        public DashboardMeasureType Type { get; set; }

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
        public DashboardNodeMeasure(Dashboard dashboard, XmlNode xmlNode)
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
        public DashboardNodeMeasure(Dashboard dashboard, XmlNode xmlNode, DashboardNode parent)
            : base(dashboard, xmlNode, parent)
        { }

        #endregion


        #region Methods

        protected override void ParseNode()
        {
            this.RenderContainer = true;

            if (this.XmlNode.Attributes["RenderContainer"] != null)
                this.RenderContainer = bool.Parse(this.XmlNode.Attributes["RenderContainer"].Value);

            if (this.XmlNode.Attributes["Type"] != null)
            {
                this.Type = (DashboardMeasureType)Enum.Parse(
                    typeof(DashboardMeasureType),
                    this.XmlNode.Attributes["Type"].Value
                );
            }

            if (this.XmlNode.Attributes["Variable"] != null)
            {
                this.Variable = this.XmlNode.Attributes["Variable"].Value;

                if (!this.Dashboard.Cache.Variables.ContainsKey(this.Variable))
                {
                    throw new Exception(string.Format(
                        "Variable with the name '{0}' doesn't exist.",
                        this.Variable
                    ));
                }

                this.IdVariable = (Guid)this.Dashboard.Cache.Variables[this.Variable][0][1];
            }

            //this.XmlNode.ParentNode.AddAttribute("RenderId", this.Dashboard.IdCounter++);
        }

        private string GetValue(
            Data filter,
            WeightingFilterCollection weight
        )
        {
            Data data = filter;
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Dashboard.Core,
                null,
                this.Dashboard.Settings.ReportSettings.WeightMissingValue
            );

            if (this.IdVariable.HasValue)
            {
                data = storageMethod.GetRespondentsNumeric(
                    this.IdVariable.Value,
                    true,
                    this.Dashboard.Core.CaseDataLocation,
                    data,
                    weight
                );
            }

            if (this.XmlNode.Attributes["Decimals"] != null)
            {
                data.Value = Math.Round(data.Value, int.Parse(
                    this.XmlNode.Attributes["Decimals"].Value
                ));
            }

            switch (this.Type)
            {
                case DashboardMeasureType.Sum:
                    break;
                case DashboardMeasureType.Average:
                    data.Value = data.Value / data.Responses.Count;
                    break;
            }

            if (this.XmlNode.Attributes["Format"] != null)
            {
                return (string.Format(
                    this.XmlNode.Attributes["Format"].Value,
                    data.Value
                ));
            }
            else
            {
                return data.Value.ToString();
            }
        }

        public override void Render(
            StringBuilder result,
            DashboardRenderContext context,
            Data filter,
            WeightingFilterCollection weight
        )
        {
            if (this.RenderContainer)
            {
                result.Append(string.Format(
                    "<span id=\"r_{0}\">",
                    this.Dashboard.IdCounter++
                ));
            }

            result.Append(this.GetValue(filter, weight));

            if (this.RenderContainer)
                result.Append("</span>");
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

            result.Append(this.GetValue(filter, weight));

            result.Append("\" },");
        }

        #endregion
    }

    public enum DashboardMeasureType
    {
        Sum,
        Average
    }
}
