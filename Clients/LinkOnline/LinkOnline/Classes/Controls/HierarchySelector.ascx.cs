using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkOnline.Classes.Controls
{
    public partial class HierarchySelector : System.Web.UI.UserControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the report definition xml file.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets the full path to the client's
        /// hierarchy selector definition file.
        /// </summary>
        public string FileName
        {
            get
            {
                if (HttpContext.Current.Session["HierarchySelector_FileName"] == null)
                    return null;

                return (string)HttpContext.Current.Session["HierarchySelector_FileName"];
            }
            set
            {
                HttpContext.Current.Session["HierarchySelector_FileName"] = value;
            }
        }

        /// <summary>
        /// Gets if the client has a hierarchy selector defined.
        /// </summary>
        public bool Exists
        {
            get
            {
                return File.Exists(this.FileName);
            }
        }

        /// <summary>
        /// Gets or sets a list of hierarchy selector sections.
        /// </summary>
        public Dictionary<string, HierarchySelectorSection> Sections { get; set; }

        public Dictionary<string, List<Guid>> SelectedItems
        {
            get
            {
                if (HttpContext.Current.Session["HierarchySelector_SelectedItems"] == null)
                    this.SelectedItems = new Dictionary<string, List<Guid>>();

                return (Dictionary<string, List<Guid>>)HttpContext.Current.Session["HierarchySelector_SelectedItems"];
            }
            set
            {
                HttpContext.Current.Session["HierarchySelector_SelectedItems"] = value;
            }
        }

        public string Stylesheet { get; set; }

        #endregion


        #region Constructor

        public HierarchySelector()
        {
        }

        #endregion


        #region Methods

        public void Parse()
        {
            this.Sections = new Dictionary<string, HierarchySelectorSection>();

            // Create a new xml document that contains the
            // client's hierarchy selector definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the client's hierarchy
            // selector definition xml file into the xml document.
            document.Load(this.FileName);

            if (document.DocumentElement.Attributes["AutoGenerate"] != null &&
                bool.Parse(document.DocumentElement.Attributes["AutoGenerate"].Value) == true)
            {
                this.AutoGenerate();

                // Load the contents of the auto generated hierarchy
                // selector definition xml file into the xml document.
                document.Load(this.FileName);
            }

            // Run through all xml nodes that
            // define a hierarchy selector section.
            foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("Section"))
            {
                // Create a new hierarchy selector section by the xml node.
                this.Sections.Add(xmlNode.Attributes["Id"].Value, new HierarchySelectorSection(
                    this,
                    xmlNode
                ));
            }

            XmlNode xmlNodeStylesheet = document.DocumentElement.SelectSingleNode("Stylesheet");

            if (xmlNodeStylesheet != null)
            {
                this.Stylesheet = xmlNodeStylesheet.InnerText;
            }
        }

        private void AutoGenerate()
        {
            //this.FileName = Path.GetTempFileName() + ".xml";
            this.FileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                HttpContext.Current.Session.SessionID,
                Guid.NewGuid() + ".xml"
            );

            if (!Directory.Exists(Path.GetDirectoryName(this.FileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(this.FileName));

            // Create a new string builder that stores
            // the auto generated xml structure.
            StringBuilder xmlBuilder = new StringBuilder();

            xmlBuilder.Append("<HierarchySelector>");

            xmlBuilder.Append("<Stylesheet>");

            xmlBuilder.Append(File.ReadAllText(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "HierarchySelectorDefault.css"
            )));

            xmlBuilder.Append("</Stylesheet>");

            // Get all hierarchies on root level.
            List<object[]> hierarchies = Global.Core.Hierarchies.GetValues(
                new string[] { "Id", "Name" },
                new string[] { "IdHierarchy" },
                new object[] { null }
            );

            // Run through all hierarchies on root level.
            foreach (object[] childHierarchy in hierarchies)
            {
                // Render the xml structure for the hierarchy.
                AutoGenerate(childHierarchy, Global.Core.Hierarchies.GetValues(
                    new string[] { "Id", "Name" },
                    new string[] { "IdHierarchy" },
                    new object[] { childHierarchy[0] },
                    "Name"
                ), xmlBuilder, false);
            }

            // Render the xml structure for all hierarchies on root level.
            //AutoGenerate(null, , xmlBuilder);

            xmlBuilder.Append("</HierarchySelector>");

            File.WriteAllText(
                this.FileName,
                xmlBuilder.ToString()
            );
        }

        private void AutoGenerate(object[] hierarchy, List<object[]> childHierarchies, StringBuilder xmlBuilder, bool filter = true)
        {
            if (childHierarchies.Count == 0)
                return;

            xmlBuilder.Append(string.Format(
                "<Section Id=\"{2}\" Name=\"{0}\"{1}>",
                hierarchy == null ? "Root" : HttpUtility.HtmlEncode(hierarchy[1]),
                " SelectionMode=\"Multi\"",
                hierarchy == null ? new Guid() : hierarchy[0]
            ));

            // Render the xml structure for the hierarchy.
            AutoGenerateHierarchy(hierarchy, childHierarchies, xmlBuilder, filter);

            xmlBuilder.Append("</Section>");

            // Run through all hierarchies on root level.
            foreach (object[] childHierarchy in childHierarchies)
            {
                // Render the xml structure for the hierarchy.
                AutoGenerate(childHierarchy, Global.Core.Hierarchies.GetValues(
                    new string[] { "Id", "Name" },
                    new string[] { "IdHierarchy" },
                    new object[] { childHierarchy[0] }
                ), xmlBuilder);
            }

            //xmlBuilder.Append("</Section>");
        }

        private void AutoGenerateHierarchy(object[] hierarchy, List<object[]> childHierarchies, StringBuilder xmlBuilder, bool filter = true)
        {
            if (filter)
            {
                xmlBuilder.Append(string.Format(
                    "<HierarchyFilter IdHierarchy=\"{0}\">",
                    hierarchy[0]
                ));
            }

            // Run through all hierarchies on root level.
            foreach (object[] childHierarchy in childHierarchies)
            {
                xmlBuilder.Append(string.Format(
                    "<Hierarchy Id=\"{0}\"><table style=\"height: 100%; width: 100% \" cellspacing=\"0\" cellpadding=\"0\">" +
                    "<tbody><tr><td>{1}</td></tr></tbody></table></Hierarchy>",
                    childHierarchy[0],
                    HttpUtility.HtmlEncode(childHierarchy[1])
                ));
            }

            if (filter)
            {
                xmlBuilder.Append("</HierarchyFilter>");
            }
        }

        public string Render()
        {
            StringBuilder writer = new StringBuilder();

            bool allSelected = true;

            // Run through all hierarchy selector sections.
            foreach (HierarchySelectorSection section in this.Sections.Values)
            {
                section.Render(writer);

                if (!section.SelectionValid())
                    allSelected = false;
            }

            if (allSelected)
            {
                writer.Append("<div class=\"HierarchySelectorSection\" style=\"background: #2f6cd0;color: #fff;float: left;width: 48%;padding: 4px;\"><table style=\"margin: 0 170px;\"><tr><td><input type =\"checkbox\" id=\"chkHierarchyAllTabs\" ></td><td>Apply all tabs</td></tr></table></div>");
                writer.Append(string.Format(
                   "<div class=\"HierarchyConfirmButton GreenBackground3\" onclick=\"ConfirmHierarchySelection('chkHierarchyAllTabs');\" style=\"width: 50%;float: right;padding: 6px;\">{0}</div>",
                   Global.LanguageManager.GetText("Go")
               ));
            }

            //pnlHierarchySections.Controls.Add(new LiteralControl(writer.ToString()));
            //return writer.ToString();
            /*Response.Clear();
            Response.Write(writer.ToString());
            Response.End();*/

            return writer.ToString();
        }

        public bool HasSelected(Guid idHierarchy)
        {
            foreach (List<Guid> items in this.SelectedItems.Values)
            {
                if (items.Contains(idHierarchy))
                    return true;
            }

            return false;
        }

        public void CheckForSelection()
        {
            foreach (HierarchySelectorSection section in this.Sections.Values)
            {
                section.CheckForSelection();
            }
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.FileName == null)
            {
                this.FileName = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "App_Data",
                    "HierarchySelectors",
                    Global.Core.ClientName + ".xml"
                );
            }

            // Check if the client has a hierarchy selector defined.
            if (!this.Exists)
            {
                this.Visible = false;
                return;
            }

            // Parse the client's hierarchy selector definition.
            this.Parse();

            // Render the client's hierarchy selector.
            //this.Render();

            if (!string.IsNullOrEmpty(this.Stylesheet))
            {
                boxHierarchySelector.Controls.Add(new LiteralControl(string.Format(
                    "<style type=\"text/css\">{0}</style>",
                    this.Stylesheet
                )));
            }

            boxHierarchySelector.Visible = true;

            HttpContext.Current.Session["HierarchySelector"] = this;
        }

        #endregion
    }

    public abstract class HierarchySelectorContext
    {
        #region Properties

        public int OptionCount
        {
            get
            {
                int result = 0;

                foreach (HierarchySelectorContext item in this.Items)
                {
                    if (item.GetType() == typeof(HierarchySelectorItem))
                    {
                        if (((HierarchySelectorItem)item).HasPermission)
                            result++;
                    }
                    else
                    {
                        result += item.OptionCount;
                    }
                }

                return result;
            }
        }

        public HierarchyItemSelectionMode SelectionMode { get; set; }

        /// <summary>
        /// Gets or sets the owning selector of
        /// the hierarchy selector section.
        /// </summary>
        public HierarchySelector Selector { get; set; }

        /// <summary>
        /// Gets or sets the parent hierarchy selector element.
        /// </summary>
        public HierarchySelectorContext Parent { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains
        /// the hierarchy selector element's definition.
        /// </summary>
        public XmlNode XmlNode { get; private set; }

        /// <summary>
        /// Gets or sets a list of child hierarchy selector elements.
        /// </summary>
        public List<HierarchySelectorContext> Items { get; set; }

        public int ItemsCount
        {
            get
            {
                int result = 0;

                foreach (HierarchySelectorContext item in this.Items)
                {
                    if (((HierarchySelectorItem)item).HasPermission)
                        result++;
                }

                return result;
            }
        }

        public HierarchySelectorSection Section
        {
            get
            {
                HierarchySelectorContext result = this.Parent;

                while (result != null)
                {
                    if (result.GetType() == typeof(HierarchySelectorSection))
                        return (HierarchySelectorSection)result;

                    result = result.Parent;
                }

                return null;
            }
        }

        #endregion


        #region Constructor

        public HierarchySelectorContext(HierarchySelector selector, XmlNode xmlNode, HierarchySelectorContext parent = null)
        {
            this.Selector = selector;
            this.XmlNode = xmlNode;
            this.Parent = parent;
            this.Items = new List<HierarchySelectorContext>();

            this.Parse();
        }

        #endregion


        #region Methods

        public void Parse()
        {
            if (this.XmlNode.Attributes["SelectionMode"] != null)
            {
                this.SelectionMode = (HierarchyItemSelectionMode)Enum.Parse(
                    typeof(HierarchyItemSelectionMode),
                    this.XmlNode.Attributes["SelectionMode"].Value
                );
            }
        }

        public virtual void Render(StringBuilder writer)
        {
            // Render the elements's begin tag.
            writer.Append(string.Format(
                "<div class=\"{0} {1}\">",
                this.GetType().Name,
                this.XmlNode.Attributes["CssClass"] != null ? this.XmlNode.Attributes["CssClass"].Value : ""
            ));

            // Run through all child elements of the element.
            foreach (HierarchySelectorContext item in this.Items)
            {
                // Render the child element.
                item.Render(writer);
            }

            // Render the element's end tag.
            writer.Append("</div>");
        }

        public virtual bool SelectionValid()
        {
            bool result = true;

            foreach (HierarchySelectorContext item in this.Items)
            {
                if (!item.SelectionValid())
                    result = false;
            }

            return result;
        }

        protected List<HierarchySelectorContext> ParseChildren()
        {
            List<HierarchySelectorContext> result = new List<HierarchySelectorContext>();

            // Run through all child nodes of the xml node.
            foreach (XmlNode xmlNode in this.XmlNode.ChildNodes)
            {
                switch (xmlNode.Name)
                {
                    case "Hierarchy":
                        result.Add(new HierarchySelectorItem(
                            this.Selector,
                            xmlNode,
                            this
                        ));
                        break;
                    case "HierarchyFilter":
                        result.Add(new HierarchySelectorFilter(
                            this.Selector,
                            xmlNode,
                            this
                        ));
                        break;
                    case "Section":
                        result.Add(new HierarchySelectorSection(
                            this.Selector,
                            xmlNode,
                            this
                        ));
                        break;
                }
            }

            return result;
        }

        #endregion
    }

    public class HierarchySelectorSection : HierarchySelectorContext
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the hierarchy selector section.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the hierarchy selector section.
        /// </summary>
        public string Name { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the
        /// hierarchy selector section object.
        /// </summary>
        /// <param name="xmlNode">
        /// The xml node that contains the hierarchy
        /// selector section definition.
        /// </param>
        public HierarchySelectorSection(
            HierarchySelector selector, 
            XmlNode xmlNode, 
            HierarchySelectorContext parent = null
        )
            : base(selector, xmlNode, parent)
        {
            // Parse the hierarchy selection section definition.
            this.Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            this.Name = this.XmlNode.Attributes["Name"].Value;
            this.Id = Guid.Parse(this.XmlNode.Attributes["Id"].Value);

            if (!this.Selector.SelectedItems.ContainsKey(this.Name))
                this.Selector.SelectedItems.Add(this.Name, new List<Guid>());

            this.Items = base.ParseChildren();
        }

        public void CheckForSelection()
        {
            if (!this.Selector.SelectedItems.ContainsKey(this.Name))
                this.Selector.SelectedItems.Add(this.Name, new List<Guid>());

            if (HttpContext.Current.Request.Params["HierarchySelectionSection"] == this.Id.ToString())
            {
                if (this.SelectionMode == HierarchyItemSelectionMode.Single)
                    this.Selector.SelectedItems[this.Name] = new List<Guid>();

                Guid[] selectedItems = HttpContext.Current.Request.Params["HierarchySelection"].Split(',').Select(x => Guid.Parse(x)).ToArray();

                foreach (Guid selectedItem in selectedItems)
                {
                    if (!this.Selector.SelectedItems[this.Name].Contains(selectedItem))
                        this.Selector.SelectedItems[this.Name].Add(selectedItem);
                    else
                        this.Selector.SelectedItems[this.Name].Remove(selectedItem);
                }
            }
        }

        #endregion
    }

    public class HierarchySelectorFilter : HierarchySelectorContext
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the hierarchy
        /// of which the filter applies to.
        /// </summary>
        public Guid IdHierarchy { get; set; }

        public bool IsVisible { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the hierarchy selector filter object.
        /// </summary>
        /// <param name="section">The hierarchy selector section of which the filter is part of.</param>
        /// <param name="xmlNode">The xml node that contains the hierarchy selector filter definition.</param>
        public HierarchySelectorFilter(HierarchySelector selector, XmlNode xmlNode, HierarchySelectorContext parent = null)
            : base(selector, xmlNode, parent)
        {
            this.Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            // Parse the id of the hierarchy
            // of which the filter applies to.
            this.IdHierarchy = Guid.Parse(
                this.XmlNode.Attributes["IdHierarchy"].Value
            );

            if (this.Selector.HasSelected(this.IdHierarchy))
                this.IsVisible = true;
            else
                this.IsVisible = false;

            this.Items = base.ParseChildren();

            if (!this.IsVisible)
                ValidateSelections(this);
        }

        private void ValidateSelections(HierarchySelectorContext parent)
        {
            foreach (HierarchySelectorContext item in parent.Items)
            {
                if (item.GetType() == typeof(HierarchySelectorItem))
                {
                    Guid idHierarchy = ((HierarchySelectorItem)item).IdHierarchy;

                    if (this.Selector.SelectedItems[this.Section.Name].Contains(idHierarchy))
                        this.Selector.SelectedItems[this.Section.Name].Remove(idHierarchy);
                }
                else
                {
                    foreach (HierarchySelectorContext child in item.Items)
                    {
                        ValidateSelections(child);
                    }
                }
            }
        }


        public override void Render(StringBuilder writer)
        {
            if (this.IsVisible)
                base.Render(writer);
        }

        public override bool SelectionValid()
        {
            if (!this.IsVisible)
                return true;

            if (this.Selector.SelectedItems[this.Section.Name].Count > 0)
                return true;

            return false;
        }

        #endregion
    }

    public class HierarchySelectorItem : HierarchySelectorContext
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the hierarchy
        /// of which the selector is for.
        /// </summary>
        public Guid IdHierarchy { get; set; }

        /// <summary>
        /// Gets or sets the html content that
        /// displays for the hierarchy selector item.
        /// </summary>
        public string HtmlContent { get; set; }

        public bool HasPermission { get; set; }

        #endregion


        #region Constructor

        public HierarchySelectorItem(HierarchySelector selector, XmlNode xmlNode, HierarchySelectorContext parent = null)
            : base(selector, xmlNode, parent)
        {
            // Parse the hierarchy selector item definition.
            this.Parse();
        }

        #endregion


        #region Methods

        public override void Render(StringBuilder writer)
        {
            if (!this.HasPermission)
                return;

            bool selected = this.Selector.SelectedItems[this.Section.Name].Contains(this.IdHierarchy);

            if (selected)
            {
                XmlNodeList xmlNodesImages = this.XmlNode.SelectNodes("//img");

                foreach (XmlNode xmlNodeImage in xmlNodesImages)
                {
                    if (xmlNodeImage.Attributes["SrcActive"] == null)
                        continue;

                    if (xmlNodeImage.Attributes["src"] == null)
                        xmlNodeImage.AddAttribute("src", "");

                    xmlNodeImage.Attributes["src"].Value = xmlNodeImage.Attributes["SrcActive"].Value;
                }

                this.HtmlContent = this.XmlNode.InnerXml;
            }

            // Render the elements's begin tag.
            writer.Append(string.Format(
                "<div class=\"{0} {2}\" onclick=\"SelectHierarchySelectorItem('{4}', '{1}')\" style=\"width:{3}%\"><div class=\"{0}_Spacer\">",
                "HierarchySelectorItem",
                this.IdHierarchy,
                selected ? "BackgroundColor5 HierarchySelectorItem_Active" : "",
                100.0 / this.Parent.ItemsCount,
                this.Section.Id
            ));

            // Render the hierarchy selector item's html content.
            writer.Append(this.HtmlContent);

            // Render the element's end tag.
            writer.Append("</div></div>");
        }

        private void Parse()
        {
            // Parse the id of the hierarchy of
            // which the selector is for.
            this.IdHierarchy = Guid.Parse(
                this.XmlNode.Attributes["Id"].Value
            );

            // Set the html content that displays for
            // the hierarchy selector item.
            this.HtmlContent = this.XmlNode.InnerXml;

            if ((int)Global.Core.WorkgroupHierarchies.ExecuteReader(
                "SELECT Count(*) FROM WorkgroupHierarchies WHERE IdWorkgroup IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser={0}) AND IdHierarchy={1}",
                new object[] { Global.IdUser.Value, this.IdHierarchy }
            )[0][0] == 0)
            {
                this.HasPermission = false;
            }
            else
            {
                this.HasPermission = true;
            }
        }

        public override bool SelectionValid()
        {
            if (this.Selector.SelectedItems[this.Section.Name].Count > 0)
                return true;

            return false;
        }

        #endregion
    }

    public enum HierarchyItemSelectionMode
    {
        Single,
        Multi
    }
}