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
    public class DashboardNodeDimension : DashboardNode
    {
        #region Properties

        public string Variable { get; set; }

        public Guid IdVariable { get; set; }

        public Dictionary<Guid, object> Categories { get; set; }

        public bool HideEmpty { get; set; }

        public string Order { get; set; }

        public int Count { get; set; }

        public bool Aggregate { get; set; }

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
        public DashboardNodeDimension(Dashboard dashboard, XmlNode xmlNode)
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
        public DashboardNodeDimension(Dashboard dashboard, XmlNode xmlNode, DashboardNode parent)
            : base(dashboard, xmlNode, parent)
        { }

        #endregion


        #region Methods

        protected override void ParseNode()
        {
            this.Order = null;
            this.Count = -1;
            this.Aggregate = true;

            if (this.XmlNode.Attributes["Order"] != null)
                this.Order = this.XmlNode.Attributes["Order"].Value;

            if (this.XmlNode.Attributes["Count"] != null)
                this.Count = int.Parse(this.XmlNode.Attributes["Count"].Value);

            if (this.XmlNode.Attributes["Aggregate"] != null)
                this.Aggregate = bool.Parse(this.XmlNode.Attributes["Aggregate"].Value);

            if (this.XmlNode.Attributes["IdSelector"] != null)
            {
                string idSelector = this.XmlNode.Attributes["IdSelector"].Value;

                if (!this.Dashboard.DimensionSelectors.ContainsKey(idSelector))
                {
                    throw new Exception(string.Format(
                        "Dimension selector with the id '{0}' wasn't found.",
                        idSelector
                    ));
                }

                this.Variable = this.Dashboard.DimensionSelectors[idSelector].Variable;

                this.XmlNode.AddAttribute("Categories", string.Join(",", this.Dashboard.DimensionSelectors[idSelector].Selected));
            }
            else
            {
                this.Variable = this.XmlNode.Attributes["Variable"].Value;
            }

            if (this.XmlNode.Attributes["HideEmpty"] != null)
                this.HideEmpty = bool.Parse(this.XmlNode.Attributes["HideEmpty"].Value);

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
                else if (this.Dashboard.Cache.CategoryLabels2[this.IdVariable].
                    ContainsKey(this.XmlNode.Attributes["Category"].Value))
                {
                    this.Categories.Add(
                        (Guid)this.Dashboard.Cache.CategoryLabels2[this.IdVariable]
                        [this.XmlNode.Attributes["Category"].Value][0][2],
                        null
                    );
                }
            }
            else if (this.XmlNode.Attributes["Categories"] != null)
            {
                foreach (string _category in this.XmlNode.Attributes["Categories"].Value.Split(','))
                {
                    string category = _category.Replace("&#44;", ",");
                    if (this.Dashboard.Cache.Categories[this.IdVariable].ContainsKey(category))
                    {
                        this.Categories.Add(
                            (Guid)this.Dashboard.Cache.Categories[this.IdVariable]
                            [category][0][2],
                            null
                        );
                    }
                    else if (this.Dashboard.Cache.CategoryLabels2[this.IdVariable].
                        ContainsKey(category))
                    {
                        this.Categories.Add(
                            (Guid)this.Dashboard.Cache.CategoryLabels2[this.IdVariable]
                            [category][0][2],
                            null
                        );
                    }
                }
            }
            else if (this.XmlNode.Attributes["CategoryIndex"] != null)
            {
                string[] indexes = this.XmlNode.Attributes["CategoryIndex"].Value.Split(',');

                foreach (string indexStr in indexes)
                {
                    int index = int.Parse(indexStr.Trim());

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
            else
            {
                List<string> categories = this.Dashboard.Cache.Categories[this.IdVariable].Keys.ToList();

                if (this.XmlNode.Attributes["Order"] != null && this.XmlNode.Attributes["Order"].Value == "Descending")
                {
                    categories.Reverse();
                }

                foreach (string category in categories)
                {
                    this.Categories.Add(
                        (Guid)this.Dashboard.Cache.Categories[this.IdVariable][category][0][2],
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
            if (!context.Categories.ContainsKey(this.Variable))
                context.Categories.Add(this.Variable, new Guid());

            DataCore.Classes.Data data = null;
            DataCore.Classes.Data datafilter = null;
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

            int count = 0;
            foreach (Guid idCategory in this.Categories.Keys)
            {
                //if (this.Aggregate)
                {
                    data = storageMethod.GetRespondents(
                        idCategory,
                        this.IdVariable,
                        true,
                        this.Dashboard.Core.CaseDataLocation,
                        filter,
                        weight
                    );
                }

                if (this.HideEmpty)
                {
                    /*bool exists = true;
                    Data hasValueFilter = null;
                    DashboardCacheHasValue hasValue = null;

                    if (!this.Dashboard.Cache.HasValue.ContainsKey(idCategory))
                    {
                        this.Dashboard.Cache.HasValue.Add(idCategory, new DashboardCacheHasValue());

                        if(this.Aggregate)
                            this.Dashboard.Cache.HasValue[idCategory].Filter = data;
                        else
                        {
                            this.Dashboard.Cache.HasValue[idCategory].Filter = storageMethod.GetRespondents(
                                idCategory,
                                this.IdVariable,
                                true,
                                this.Dashboard.Core.CaseDataLocation,
                                filter,
                                weight
                            );
                        }
                        hasValueFilter = filter;
                        exists = false;
                    }

                    hasValue = this.Dashboard.Cache.HasValue[idCategory];

                    foreach (string variable in context.Categories.Keys)
                    {
                        if (variable == this.Variable)
                            continue;

                        if (!hasValue.Values.ContainsKey(context.Categories[variable]))
                        {
                            hasValue.Values.Add(context.Categories[variable], new DashboardCore.DashboardCacheHasValue());

                            exists = false;
                            break;
                        }
                        else
                        {
                            hasValueFilter = hasValue.Filter;
                        }

                        hasValue = hasValue.Values[context.Categories[variable]];
                        hasValue.Filter = hasValueFilter;
                    }

                    if (!exists)
                    {
                        if (!this.Aggregate)
                        {
                            data = storageMethod.GetRespondents(
                                idCategory,
                                this.IdVariable,
                                true,
                                this.Dashboard.Core.CaseDataLocation,
                                hasValueFilter,
                                weight
                            );

                            hasValue.Filter = data;
                        }

                        hasValue.HasValue = data.Base != 0;
                    }

                    //if(data.Responses.Count == 0)
                    if (!hasValue.HasValue)
                        continue;*/
                    /*if (data.Base == 0)
                        continue;*/
                    if (data.Base == 0)
                        continue;
                }

                context.Categories[this.Variable] = idCategory;

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

                count++;

                if (this.Count != -1 && count == this.Count)
                    break;
            }

            context.Categories.Remove(this.Variable);
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

    public class DashboardRenderContext
    {
        #region Properties

        public Dictionary<string, Guid> Categories { get; set; }

        public Dictionary<Guid, string> LastRenderedLabel { get; set; }

        #endregion


        #region Constructor

        public DashboardRenderContext()
        {
            this.Categories = new Dictionary<string, Guid>();
            this.LastRenderedLabel = new Dictionary<Guid, string>();
        }

        #endregion
    }
}
