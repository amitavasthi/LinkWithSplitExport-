using Crosstables.Classes.ReportDefinitionClasses.Collections;
using DataCore.Classes;
using DataCore.Classes.StorageMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using WebUtilities;
using WebUtilities.Controls;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class FilterCategoryOperator : BaseControl
    {
        #region Properties

        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the owning report calculator.
        /// </summary>
        public WeightingFilterCollection Owner { get; set; }

        /// <summary>
        /// Gets or sets the parent filter category operator.
        /// </summary>
        public FilterCategoryOperator Parent { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains
        /// the filter category operator definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the id of the filter category operator.
        /// </summary>
        public Guid Id
        {
            get
            {
                return Guid.Parse(
                    this.XmlNode.Attributes["Id"].Value
                );
            }
            set
            {
                this.XmlNode.Attributes["Id"].Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the type of the filter category operator.
        /// </summary>
        public FilterCategoryOperatorType Type
        {
            get
            {
                return (FilterCategoryOperatorType)Enum.Parse(
                    typeof(FilterCategoryOperatorType),
                    this.XmlNode.Attributes["Type"].Value
                );
            }
            set
            {
                this.XmlNode.Attributes["Type"].Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the list of the child
        /// filter category operators.
        /// </summary>
        public List<FilterCategoryOperator> FilterCategoryOperators { get; set; }

        /// <summary>
        /// Gets or sets the list of filter categories
        /// of the filter category operator.
        /// </summary>
        public List<FilterCategory> FilterCategories { get; set; }

        /// <summary>
        /// Gets or sets the depth level of the filter category operator.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the unique identity
        /// of the filter category operator.
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// Gets if filters are applied in the filter category operator.
        /// </summary>
        public bool FiltersApplied
        {
            get
            {
                if (this.FilterCategories.Count > 0)
                    return true;

                foreach (FilterCategoryOperator filterCategoryOperator in this.FilterCategoryOperators)
                {
                    if (filterCategoryOperator.FiltersApplied)
                        return true;
                }

                return false;
            }
        }

        public event EventHandler OnChange;

        #endregion


        #region Constructor

        public FilterCategoryOperator(WeightingFilterCollection owner, XmlNode xmlNode, int level, string fileName)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;
            this.Level = level;
            this.FileName = fileName.Replace("\\", "/");
            this.Load += FilterCategoryOperator_Load;

            base.EnableViewState = false;

            Parse();
        }

        public FilterCategoryOperator(WeightingFilterCollection owner, XmlNode xmlNode, int level, string fileName, FilterCategoryOperator parent)
            : this(owner, xmlNode, level, fileName)
        {
            this.Parent = parent;
        }

        #endregion


        #region Methods

        public void Parse()
        {
            this.FilterCategories = new List<FilterCategory>();
            this.FilterCategoryOperators = new List<FilterCategoryOperator>();

            // Build the identity of the filter category operator.
            this.Identity = this.XmlNode.GetXPath().Replace("[", "").
                Replace("]", "").Replace("\"", "").Replace("/", "").
                Replace("=", "").Replace("@", "") + this.XmlNode.
                ParentNode.ChildNodes.IndexOf(this.XmlNode);

            // Run through all filter category xml nodes of the xml node.
            foreach (XmlNode xmlNode in this.XmlNode.SelectNodes("Category|TaxonomyCategory"))
            {
                // Create a new filter category by the xml node and add
                // it to the filter category operator's filter categories.
                this.FilterCategories.Add(
                    new FilterCategory(this, xmlNode)
                );
            }

            // Run through all filter category operator xml nodes of the xml node.
            foreach (XmlNode xmlNode in this.XmlNode.SelectNodes("Operator"))
            {
                // Create a new filter category operator by the xml node and
                // add it to the filter category operator's filter category operators.
                this.FilterCategoryOperators.Add(
                    new FilterCategoryOperator(this.Owner, xmlNode, this.Level + 1, this.FileName, this)
                );
            }
        }


        public Data GetRespondents(
            Database storageMethod,
            Data filter = null
        )
        {
            Data result = filter;

            // Run through all filter categories of the filter category operator.
            foreach (FilterCategory filterCategory in this.FilterCategories)
            {
                // Get the respondents for the filter category's category.
                Data categoryRespondents = filterCategory.GetRespondents(
                    storageMethod,
                    filter
                );

                if (result == null)
                {
                    result = categoryRespondents;
                    continue;
                }

                if (this.Type == FilterCategoryOperatorType.AND)
                {
                    List<Guid> removalRespondents = new List<Guid>();

                    // Run through all respondents of the result.
                    foreach (Guid idRespondent in result.Responses.Keys)
                    {
                        // Check if the respondent is present in category's respondents.
                        if (!categoryRespondents.Responses.ContainsKey(idRespondent))
                        {
                            removalRespondents.Add(idRespondent);
                        }
                    }

                    foreach (Guid idRespondent in removalRespondents)
                    {
                        result.Responses.Remove(idRespondent);
                    }
                }
                else if (this.Type == FilterCategoryOperatorType.OR && categoryRespondents != null)
                {
                    foreach (KeyValuePair<Guid, double[]> respondent in categoryRespondents.Responses)
                    {
                        if (result.Responses.ContainsKey(respondent.Key))
                            continue;

                        result.Responses.Add(
                            respondent.Key,
                            respondent.Value
                        );
                    }
                }
            }

            // Run through all child filter category operators.
            foreach (FilterCategoryOperator filterCategoryOperator in this.FilterCategoryOperators)
            {
                Data filterCategoryOperatorRespondents = filterCategoryOperator.GetRespondents(
                    storageMethod,
                    filter
                );

                if (filterCategoryOperatorRespondents == null)
                    continue;

                if (result == null)
                {
                    result = filterCategoryOperatorRespondents;
                    continue;
                }

                if (this.Type == FilterCategoryOperatorType.AND)
                {
                    List<Guid> removalRespondents = new List<Guid>();

                    // Run through all respondents of the result.
                    foreach (Guid idRespondent in result.Responses.Keys)
                    {
                        // Check if the respondent is present in category's respondents.
                        if (!filterCategoryOperatorRespondents.Responses.ContainsKey(idRespondent))
                        {
                            removalRespondents.Add(idRespondent);
                        }
                    }

                    foreach (Guid idRespondent in removalRespondents)
                    {
                        result.Responses.Remove(idRespondent);
                    }
                }
                else if (this.Type == FilterCategoryOperatorType.OR)
                {
                    foreach (KeyValuePair<Guid, double[]> respondent in filterCategoryOperatorRespondents.Responses)
                    {
                        if (result.Responses.ContainsKey(respondent.Key))
                            continue;

                        result.Responses.Add(
                            respondent.Key,
                            respondent.Value
                        );
                    }
                }
            }

            return result;
        }

        public void Render()
        {
            base.Controls.Clear();

            // Check if a post back was made from this filter category operator.
            if (HttpContext.Current.Request.Params["__EVENTARGUMENT"] == this.XmlNode.GetXPath())
            {
                Guid idCategory = Guid.Parse(
                    HttpContext.Current.Request.Params["__EVENTTARGET"].Replace("CategorySelectorCategory", "")
                );

                if (this.FilterCategories.Find(x => x.IdCategory == idCategory) == null)
                {
                    this.XmlNode.InnerXml += string.Format(
                        "<Category Id=\"{0}\"></Category>",
                        idCategory
                    );

                    this.Owner.Owner.ClearData();
                    this.Owner.Owner.Save();

                    Parse();
                }
            }

            if (HttpContext.Current.Request.Params["__EVENTTARGET"] != null &&
                HttpContext.Current.Request.Params["__EVENTTARGET"].EndsWith("lnkNewOperator" + this.Identity))
            {
                this.XmlNode.InnerXml += string.Format(
                    "<Operator Id=\"{0}\" Type=\"AND\"></Operator>",
                    Guid.NewGuid()
                );

                this.Owner.Owner.Save();

                Parse();
            }

            // Create a new panel for the result.
            Panel result = new Panel();
            result.ID = "pnlFilterCategoryOperator" + this.Identity;

            result.PreRender += result_PreRender;

            Table table = new Table();
            TableRow tableRow = new TableRow();

            TableCell tableCellSelector = new TableCell();
            TableCell tableCellElements = new TableCell();

            tableCellSelector.Style.Add("width", "1px");

            // Set the css class of the result panel.
            result.CssClass = "FilterOperator FilterOperator" + this.Type;

            if (this.Level % 2 == 1)
            {
                result.CssClass += " BackgroundColor9";
            }
            else
            {
                result.CssClass += " BackgroundColor1";
            }

            if (this.Parent != null)
            {
                Image imgDeleteFilterCategoryOperator = new Image();
                imgDeleteFilterCategoryOperator.Style.Add("cursor", "pointer");
                imgDeleteFilterCategoryOperator.ID = "imgDeleteFilterCategoryOperator" + this.Identity;
                imgDeleteFilterCategoryOperator.CssClass = "DeleteFilterCategoryOperator";
                imgDeleteFilterCategoryOperator.ImageUrl = "/Images/Icons/Delete2.png";
                //imgDeleteFilterCategoryOperator.Click += imgDeleteFilterCategoryOperator_Click;
                imgDeleteFilterCategoryOperator.Attributes.Add("onclick", string.Format(
                    "DeleteFilterCategoryOperator('{0}', '{1}')",
                    this.FileName,
                    this.XmlNode.GetXPath()
                ));

                tableCellElements.Controls.Add(imgDeleteFilterCategoryOperator);
            }

            // Create a new drop down list for the filter operator's type.
            DropDownList ddlType = new DropDownList();
            ddlType.ID = "DropDownListType" + this.Identity;
            ddlType.CssClass = "DropDownListType";
            ddlType.BindEnum(typeof(FilterCategoryOperatorType));
            ddlType.SelectedValue = this.Type.ToString();
            ddlType.AutoPostBack = false;

            ddlType.Attributes.Add(
                "onchange",
                string.Format(
                    "ChangeFilterCategoryOperator('{0}', '{1}',this.value);",
                    this.FileName,
                    this.XmlNode.GetXPath()
                )
            );

            if (this.FilterCategories.Count > 0)
            {
                ddlType.CssClass = "DropDownListCategoriesType";
                tableCellElements.Controls.Add(ddlType);
            }
            else
            {
                // Add the type drop down list to the result panel's controls.
                tableCellSelector.Controls.Add(ddlType);
            }

            // Run through all filter categories of the filter category operator.
            foreach (FilterCategory filterCategory in this.FilterCategories)
            {
                // Render the filter category and
                // add it to the result panel's controls.
                tableCellElements.Controls.Add(
                    filterCategory.RenderControl()
                );
            }

            int i = 0;
            // Run through all child filter category operators.
            foreach (FilterCategoryOperator filterCategoryOperator in this.FilterCategoryOperators)
            {
                // Set the child filter category operator's on change event handler.
                filterCategoryOperator.OnChange = this.OnChange;

                // Render the child filter category operator and
                // add it to the result panel's controls.
                tableCellElements.Controls.Add(
                    filterCategoryOperator
                );

                filterCategoryOperator.Render();
            }

            if (this.FilterCategoryOperators.Count == 0 && this.Parent != null)
            {
                /*CategorySelector selector = new CategorySelector(this.Owner.Owner, this.FileName);
                selector.ID = "FilterCategorySelector" + this.Identity;
                selector.XPath = this.XmlNode.GetXPath();

                tableCellElements.Controls.Add(selector);

                selector.Render();*/
                Image imgNewCategory = new Image();
                imgNewCategory.ImageUrl = "/Images/Icons/Add2.png";
                imgNewCategory.Style.Add("cursor", "pointer");

                imgNewCategory.Attributes.Add(
                    "onclick",
                    "EnableFilterCategorySearch(this, '" + this.FileName + "', '" + this.XmlNode.GetXPath() + "')"
                );

                tableCellElements.Controls.Add(imgNewCategory);
            }

            if (this.FilterCategories.Count == 0)
            {
                System.Web.UI.WebControls.LinkButton lnkNewOperator = new System.Web.UI.WebControls.LinkButton();
                lnkNewOperator.ID = "lnkNewOperator" + this.Identity;
                lnkNewOperator.Text = (this.FilterCategoryOperators.Count == 0 && this.Parent != null ? "<br />" : "") + base.LanguageManager.GetText("AddFilterOperator");
                lnkNewOperator.CssClass = "NewFilterOperator";
                //lnkNewOperator.Click += lnkNewOperator_Click;

                lnkNewOperator.Attributes.Add("href", string.Format(
                    "javascript:AddFilterCategoryOperator('{0}', '{1}');",
                    this.FileName,
                    this.XmlNode.GetXPath()
                ));

                tableCellElements.Controls.Add(lnkNewOperator);
            }

            tableRow.Cells.Add(tableCellSelector);
            tableRow.Cells.Add(tableCellElements);

            table.Rows.Add(tableRow);

            result.Controls.Add(table);

            base.Controls.Add(result);
        }

        public override string ToString()
        {
            // Create a new string builder that
            // contains the result filter string.
            StringBuilder result = new StringBuilder();

            // Run through all filter operators.
            foreach (FilterCategoryOperator filterOperator in this.FilterCategoryOperators)
            {
                result.Append("(");

                // Render the filter operator.
                result.Append(filterOperator.ToString());

                result.Append(") ");

                result.Append(this.Type.ToString());
                result.Append(" ");
            }

            // Run through all filter categories.
            foreach (FilterCategory filter in this.FilterCategories)
            {
                // Render the filter operator.
                result.Append(filter.ToString());

                result.Append(" ");
                result.Append(this.Type.ToString());
                result.Append(" ");
            }

            if (this.FilterCategoryOperators.Count > 0 || this.FilterCategories.Count > 0)
                result = result.Remove(result.Length - (this.Type.ToString().Length + 2), (this.Type.ToString().Length + 2));

            // Return the contents of the result's string builder.
            return result.ToString();
        }


        public bool Add(Guid idCategory, bool isTaxonomy = false)
        {
            if (this.FilterCategoryOperators.Count > 0)
            {
                foreach (FilterCategoryOperator filterCategoryOperator in this.FilterCategoryOperators)
                {
                    if (filterCategoryOperator.Add(idCategory, isTaxonomy))
                        return true;
                }
            }
            else if (this.FilterCategories.Find(x => x.IdCategory == idCategory) == null)
            {
                string tag = "Category";

                if (isTaxonomy)
                    tag = "TaxonomyCategory";

                this.XmlNode.InnerXml += string.Format(
                    "<{0} Id=\"{1}\"></{0}>",
                    tag,
                    idCategory
                );

                this.Owner.Owner.ClearData();
                this.Owner.Owner.Save();

                return true;
            }

            return false;
        }


        public void Delete(FilterCategory filterCategory)
        {
            this.XmlNode.RemoveChild(filterCategory.XmlNode);

            this.FilterCategories.Remove(filterCategory);

            this.Owner.Owner.ClearData();
            this.Owner.Owner.Save();

            base.Controls.Clear();
            Render();

            if (this.OnChange != null)
            {
                this.OnChange(this, new EventArgs());
            }
        }

        public void Delete(FilterCategoryOperator filterCategoryOperator)
        {
            this.XmlNode.RemoveChild(filterCategoryOperator.XmlNode);

            this.FilterCategoryOperators.Remove(filterCategoryOperator);

            this.Owner.Owner.ClearData();
            this.Owner.Owner.Save();

            base.Controls.Clear();
            Render();

            if (this.OnChange != null)
            {
                this.OnChange(this, new EventArgs());
            }
        }

        #endregion


        #region Event Handlers

        protected void FilterCategoryOperator_Load(object sender, EventArgs e)
        {
            Render();
        }


        protected void result_PreRender(object sender, EventArgs e)
        {
            // Check if a post back was made from this filter category operator.
            if (HttpContext.Current.Request.Params["__EVENTARGUMENT"] == this.XmlNode.GetXPath())
            {
                if (this.OnChange != null)
                {
                    this.OnChange(this, new EventArgs());
                }
            }
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlType = (DropDownList)sender;

            this.Type = (FilterCategoryOperatorType)Enum.Parse(
                typeof(FilterCategoryOperatorType),
                ddlType.SelectedValue
            );

            this.Owner.Owner.ClearData();
            this.Owner.Owner.Save();

            if (this.OnChange != null)
            {
                this.OnChange(this, e);
            }
        }

        protected void lnkNewOperator_Click(object sender, EventArgs e)
        {

        }

        protected void imgDeleteFilterCategoryOperator_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.Parent == null)
                return;

            this.Parent.Delete(this);
        }

        #endregion
    }

    public enum FilterCategoryOperatorType
    {
        AND,
        OR
    }
}
