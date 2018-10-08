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
    public class DashboardNodeMeasureLabel : DashboardNode
    {
        #region Properties

        public string Variable { get; set; }

        public Guid? IdVariable { get; set; }

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
        public DashboardNodeMeasureLabel(Dashboard dashboard, XmlNode xmlNode)
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
        public DashboardNodeMeasureLabel(Dashboard dashboard, XmlNode xmlNode, DashboardNode parent)
            : base(dashboard, xmlNode, parent)
        { }

        #endregion


        #region Methods

        protected override void ParseNode()
        {
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
        }

        public override void Render(
            StringBuilder result,
            DashboardRenderContext context,
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

            double highestValue = double.MinValue;
            Guid idCategory = new Guid();
            foreach (string category in this.Dashboard.Cache.Categories[this.IdVariable.Value].Keys)
            {
                Data value = storageMethod.GetRespondents(
                    (Guid)this.Dashboard.Cache.Categories[this.IdVariable.Value][category][0][2],
                    this.IdVariable.Value,
                    true,
                    this.Dashboard.Core.CaseDataLocation,
                    data,
                    weight
                );

                if (value.Value > highestValue)
                {
                    highestValue = value.Value;
                    idCategory = (Guid)this.Dashboard.Cache.
                        Categories[this.IdVariable.Value][category][0][2];
                }
            }

            if (this.Dashboard.Cache.CategoryLabels.ContainsKey(idCategory))
                result.Append((string)this.Dashboard.Cache.CategoryLabels[idCategory][0][1]);
        }

        #endregion
    }
}
