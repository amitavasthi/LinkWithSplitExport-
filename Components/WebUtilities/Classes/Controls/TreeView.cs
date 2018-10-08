using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.HtmlControls;

namespace WebUtilities.Controls
{
    public class TreeView : BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the tree view.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets a collection of nodes at root level.
        /// </summary>
        public List<TreeViewNode> Nodes { get; private set; }

        #endregion


        #region Constructor

        public TreeView(string id)
        {
            this.ID = id;
            this.Load += TreeView_Load;

            HttpContext.Current.Session["TreeView" + id] = this;

            this.Nodes = new List<TreeViewNode>();
        }

        #endregion


        #region Methods

        public void AddNode(TreeViewNode node)
        {
            node.Level = 0;

            this.Nodes.Add(node);
        }

        public void Render()
        {
            // Run through all nodes of the tree view.
            foreach (TreeViewNode node in this.Nodes)
            {
                base.Controls.Add(node);
            }
        }

        public TreeViewNode Select(string path)
        {
            string[] parts = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (TreeViewNode node in this.Nodes)
            {
                if (node.ID == parts[0])
                {
                    if (parts.Length == 1)
                        return node;

                    List<string> childParts = parts.ToList();
                    childParts.RemoveAt(0);

                    string childPath = string.Join(
                        "/",
                        childParts
                    );

                    return node.Select(
                      childPath
                    );
                }
            }

            return null;
        }

        #endregion


        #region Event Handlers

        protected void TreeView_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }

    public class TreeViewNode : BaseControl
    {
        #region Properties

        public object ScopeObject { get; set; }

        /// <summary>
        /// Gets or sets the owning tree view of the node.
        /// </summary>
        public TreeView Owner { get; set; }

        /// <summary>
        /// Gets or sets the path to the node.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets if the tree view's children are visible.
        /// </summary>
        public bool Expanded
        {
            get
            {
                if (HttpContext.Current.Session["TreeViewNodeExpanded" + this.ID] == null)
                    this.Expanded = false;

                return (bool)HttpContext.Current.Session["TreeViewNodeExpanded" + this.ID];
            }
            set
            {
                HttpContext.Current.Session["TreeViewNodeExpanded" + this.ID] = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the tree view node.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the label of the tree view node.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a list of the child tree view nodes.
        /// </summary>
        public List<TreeViewNode> Nodes { get; private set; }

        internal int Level { get; set; }

        public TreeViewLoadChildren LoadChildren { get; set; }

        public List<System.Web.UI.Control> Buttons { get; set; }

        public string OnClientClick { get; set; }

        public string OnContextMenu { get; set; }

        #endregion


        #region Constructor

        public TreeViewNode(TreeView owner, string id, string parentPath)
        {
            this.Owner = owner;
            this.ID = id;
            this.Path = parentPath + "/" + id;

            this.Buttons = new List<System.Web.UI.Control>();
            this.Nodes = new List<TreeViewNode>();

            this.Load += TreeViewNode_Load;
        }

        #endregion


        #region Methods

        public TreeViewNode Select(string path)
        {
            string[] parts = path.Split('/');

            foreach (TreeViewNode node in this.Nodes)
            {
                if (node.ID == parts[0])
                {
                    if (parts.Length == 1)
                        return node;

                    List<string> childParts = parts.ToList();
                    childParts.RemoveAt(0);

                    string childPath = string.Join(
                        "/",
                        childParts
                    );

                    return node.Select(
                      childPath
                    );
                }
            }

            return null;
        }

        public void AddChild(TreeViewNode node)
        {
            node.Level = this.Level + 1;

            this.Nodes.Add(node);
        }

        public void Render(bool asynch = false)
        {
            this.CssClass = "TreeViewNode " + this.CssClass.Replace("TreeViewNode ", "");
            base.Controls.Clear();

            //base.Style.Add("margin-left", (50 * this.Level) + "px");
            if (this.Level != 0)
                base.Style.Add("margin-left", "40px");

            Panel pnlButtons = new Panel();
            pnlButtons.CssClass = "TreeViewNodeButtons";

            foreach (System.Web.UI.Control button in this.Buttons)
            {
                pnlButtons.Controls.Add(button);
            }

            HtmlGenericControl label = new HtmlGenericControl("div");
            label.ID = "_" + this.ID;
            //label.Attributes.Add("class", "");

            if (!string.IsNullOrEmpty(this.OnClientClick))
            {
                label.Attributes.Add("onclick", this.OnClientClick);
            }

            if (!string.IsNullOrEmpty(this.OnContextMenu))
            {
                label.Attributes.Add("oncontextmenu", this.OnContextMenu);
            }

            base.Controls.Add(pnlButtons);
            base.Controls.Add(label);

            Panel pnlChildNodes = this.RenderChildNodes(asynch);

            base.Controls.Add(pnlChildNodes);


            string imgExpander = "";

            if (this.Nodes.Count > 0 || this.LoadChildren != null)
            {
                if (!this.Expanded)
                {
                    imgExpander = "<img style=\"cursor:pointer;\" src=\"/Images/Icons/TreeViewNodeExpand.png\" onclick=\"TreeViewShowChildNodes(this, '" + pnlChildNodes.Attributes["id"] + "', '" + this.Owner.ID + "', '" + this.Path + "');\" />&nbsp;";
                }
                else
                {
                    imgExpander = "<img style=\"cursor:pointer;\" src=\"/Images/Icons/TreeViewNodeCollapse.png\" onclick=\"TreeViewHideChildNodes(this, '" + pnlChildNodes.Attributes["id"] + "', '" + this.Owner.ID + "', '" + this.Path + "');\" />&nbsp;";
                }
            }

            label.InnerHtml = "<table cellspacing=\"0\" cellpadding=\"0\"><tr><td class=\"TreeViewNodeExpander\">" + imgExpander + "</td><td class=\"TreeViewNodeLabel\">" + this.Label + "</td></tr></table>";
        }

        public Panel RenderChildNodes(bool asynch)
        {
            Panel pnlChildNodes = new Panel();
            //pnlChildNodes.Style.Add("display", "none");
            pnlChildNodes.Attributes.Add("id", "pnlChildNodes" + this.ID);

            // Check if the tree view node's child nodes are visible.
            if (this.Expanded && this.LoadChildren != null && this.Nodes.Count == 0)
            {
                // Load the child nodes.
                this.LoadChildren(this, this.ScopeObject);
            }

            // Run through all child nodes.
            foreach (TreeViewNode node in this.Nodes)
            {
                pnlChildNodes.Controls.Add(node);

                if (asynch)
                    node.Render();
            }

            return pnlChildNodes;
        }

        #endregion


        #region Event Handlers

        protected void TreeViewNode_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }

    public delegate void TreeViewLoadChildren(TreeViewNode sender, object scopeObject);
}
