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
    public class DashboardNodeFilter : DashboardNode
    {
        #region Properties

        public string Variable { get; set; }

        public Guid IdVariable { get; set; }

        public Dictionary<Guid, object> Categories { get; set; }

        public DashboardFilterOperator Operator { get; set; }

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
        public DashboardNodeFilter(Dashboard dashboard, XmlNode xmlNode)
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
        public DashboardNodeFilter(Dashboard dashboard, XmlNode xmlNode, DashboardNode parent)
            : base(dashboard, xmlNode, parent)
        { }

        #endregion


        #region Methods

        protected override void ParseNode()
        {
            if (this.XmlNode.Attributes["Operator"] != null)
            {
                this.Operator = (DashboardFilterOperator)Enum.Parse(
                    typeof(DashboardFilterOperator),
                    this.XmlNode.Attributes["Operator"].Value
                );
            }

            this.Variable = this.XmlNode.Attributes["Variable"].Value;

            if (!this.Dashboard.Cache.Variables.ContainsKey(this.Variable))
            {
                throw new Exception(string.Format(
                    "Variable with the name '{0}' doesn't exist.",
                    this.Variable
                ));
            }

            this.IdVariable = (Guid)this.Dashboard.Cache.Variables[this.Variable][0][1];

            this.Categories = new Dictionary<Guid, object>();

            if (this.XmlNode.Attributes["Category"] != null)
            {
                if (this.Dashboard.Cache.Categories[this.IdVariable].ContainsKey(
                    this.XmlNode.Attributes["Category"].Value))
                {
                    this.Categories.Add(
                        (Guid)this.Dashboard.Cache.Categories[this.IdVariable]
                        [this.XmlNode.Attributes["Category"].Value][0][2],
                        null
                    );
                }
            }
            else if (this.XmlNode.Attributes["Categories"] != null)
            {
                foreach (string category in this.XmlNode.Attributes["Categories"].Value.Split(','))
                {
                    if (this.Dashboard.Cache.Categories[this.IdVariable].ContainsKey(category))
                    {
                        this.Categories.Add(
                            (Guid)this.Dashboard.Cache.Categories[this.IdVariable]
                            [category][0][2],
                            null
                        );
                    }
                }
            }
            else if (this.XmlNode.Attributes["CategoryIndex"] != null)
            {
                int index = int.Parse(this.XmlNode.Attributes["CategoryIndex"].Value);

                if (index < 0)
                {
                    index = this.Dashboard.Cache.Categories[this.IdVariable].Count + index;
                }

                if (index >= 0 && this.Dashboard.Cache.Categories[this.IdVariable].Count > index)
                {
                    this.Categories.Add(
                        (Guid)this.Dashboard.Cache.Categories[this.IdVariable].ElementAt(index).Value[0][2],
                        null
                    );
                }
            }
        }

        public override void Render(
            StringBuilder result,
            DashboardRenderContext context,
            Data filter,
            WeightingFilterCollection weight
        )
        {
            DataCore.Classes.Data data = filter;
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Dashboard.Core,
                null,
                this.Dashboard.Settings.ReportSettings.WeightMissingValue
            );

            if (this.Categories.Count == 0)
            {
                data = storageMethod.GetRespondents(
                    this.IdVariable,
                    true,
                    base.Dashboard.Core.CaseDataLocation,
                    data,
                    weight
                );
            }
            else
            {
                if (this.Operator == DashboardFilterOperator.AND)
                {
                    foreach (Guid idCategory in this.Categories.Keys)
                    {
                        data = storageMethod.GetRespondents(
                            idCategory,
                            this.IdVariable,
                            true,
                            base.Dashboard.Core.CaseDataLocation,
                            data,
                            weight
                        );
                    }
                }
                else
                {
                    if (data == null)
                        data = new Data();

                    foreach (Guid idCategory in this.Categories.Keys)
                    {
                        Data d = storageMethod.GetRespondents(
                            idCategory,
                            this.IdVariable,
                            true,
                            base.Dashboard.Core.CaseDataLocation,
                            data,
                            weight
                        );

                        foreach (Guid idRespondent in d.Responses.Keys)
                        {
                            if (!data.Responses.ContainsKey(idRespondent))
                                data.Responses.Add(idRespondent, d.Responses[idRespondent]);
                        }
                    }
                }
            }

            // Run through all child dashboard nodes.
            foreach (DashboardNode children in base.Children)
            {
                children.Render(
                    result,
                    context,
                    data,
                    weight
                );
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
            if (!context.Categories.ContainsKey(this.Variable))
                context.Categories.Add(this.Variable, new Guid());

            DataCore.Classes.Data data = null;
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Dashboard.Core,
                null,
                this.Dashboard.Settings.ReportSettings.WeightMissingValue
            );

            if (this.XmlNode.Attributes["DefaultWeighting"] != null)
            {
                if (this.Dashboard.Cache.Variables.ContainsKey(this.XmlNode.Attributes["DefaultWeighting"].Value))
                {
                    XmlNode xmlNode = this.XmlNode.Clone();
                    xmlNode.InnerXml = "";

                    xmlNode.Attributes["DefaultWeighting"].Value =
                        ((Guid)this.Dashboard.Cache.Variables
                        [this.XmlNode.Attributes["DefaultWeighting"].Value][0][1]).ToString();

                    weight = new WeightingFilterCollection(
                        null,
                        this.Dashboard.Core,
                        xmlNode
                    );
                    weight.LoadRespondents(filter);
                }
            }

            foreach (Guid idCategory in this.Categories.Keys)
            {
                data = storageMethod.GetRespondents(
                    idCategory,
                    this.IdVariable,
                    true,
                    this.Dashboard.Core.CaseDataLocation,
                    filter,
                    weight
                );

                context.Categories[this.Variable] = idCategory;

                foreach (DashboardNode node in this.Children)
                {
                    node.RenderDataUpdate(
                        result,
                        context,
                        data,
                        weight,
                        path
                    );
                }
            }
        }

        #endregion
    }

    public enum DashboardFilterOperator
    {
        AND,
        OR
    }
}
