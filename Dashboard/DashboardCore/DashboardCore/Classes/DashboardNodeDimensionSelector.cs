using Crosstables.Classes.ReportDefinitionClasses.Collections;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace DashboardCore.Classes
{
    public class DashboardNodeDimensionSelector : DashboardNode
    {
        #region Properties

        public string Id { get; set; }

        public string Variable { get; set; }

        public Guid IdVariable { get; set; }

        private Dictionary<Guid, object> Categories { get; set; }

        public bool HideEmpty { get; set; }

        public List<string> Selected { get; set; }

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
        public DashboardNodeDimensionSelector(Dashboard dashboard, XmlNode xmlNode)
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
        public DashboardNodeDimensionSelector(Dashboard dashboard, XmlNode xmlNode, DashboardNode parent)
            : base(dashboard, xmlNode, parent)
        { }

        #endregion


        #region Methods

        public override void PreParse()
        {
            this.Selected = new List<string>();

            if (this.XmlNode.Attributes["Id"] == null)
                throw new Exception("Dimension selectors require Id fields.");

            this.Id = this.XmlNode.Attributes["Id"].Value;

            if (this.XmlNode.Attributes["Selected"] != null)
                this.Selected = this.XmlNode.Attributes["Selected"].Value.Split(',').ToList();

            if (HttpContext.Current.Request.Params[this.Id] != null)
            {
                this.Selected = HttpContext.Current.Request.Params[this.Id].Split(',').ToList();
                /*this.Selected = new List<string>();
                this.Selected.Add(HttpContext.Current.Request.Params[this.Id]);*/
                this.XmlNode.AddAttribute("Selected", string.Join(",", this.Selected));
            }

            this.Variable = this.XmlNode.Attributes["Variable"].Value;

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

            this.Dashboard.DimensionSelectors.Add(this.Id, this);

            if (this.Selected.Count == 0)
            {
                this.Selected.Add(this.Dashboard.Cache.Categories[this.IdVariable].First().Key);
            }
        }

        protected override void ParseNode()
        { }

        public override void Render(
            StringBuilder result,
            DashboardRenderContext context,
            Data filter,
            WeightingFilterCollection weight
        )
        {
            result.Append("<select");

            if (this.XmlNode.Attributes["class"] != null)
                this.XmlNode.Attributes["class"].Value += " DimensionSelector";
            else
                this.XmlNode.AddAttribute("class", "DimensionSelector");

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

            if(this.XmlNode.Attributes["selected"] == null)
            {
                // Render the html attribute.
                result.Append(string.Format(
                    " {0}=\"{1}\"",
                    "selected",
                    string.Join(",", this.Selected)
                ));
            }

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

            foreach (Guid idCategory in this.Categories.Keys)
            {
                if (this.HideEmpty)
                {
                    data = storageMethod.GetRespondents(
                        idCategory,
                        this.IdVariable,
                        true,
                        this.Dashboard.Core.CaseDataLocation,
                        null,//filter,
                        weight
                    );

                    //if(data.Responses.Count == 0)
                    if (data.Base == 0)
                        continue;
                }
                result.Append(string.Format(
                    "<option value=\"{0}\">{1}</option>",
                    this.Dashboard.Cache.Categories2[idCategory][0][1],
                    this.Dashboard.Cache.CategoryLabels[idCategory][0][1]
                ));
            }

            result.Append("</select>");
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
