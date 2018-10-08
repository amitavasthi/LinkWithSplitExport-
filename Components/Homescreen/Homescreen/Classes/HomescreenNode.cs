using Homescreen1.Classes.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Homescreen1.Classes
{
    public class HomescreenNode
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning homescreen.
        /// </summary>
        public Homescreen Owner { get; set; }

        /// <summary>
        /// Gets the typename
        /// of the homescreen node.
        /// </summary>
        public string TypeName
        {
            get
            {
                return this.XmlNode.Name;
            }
        }

        public string Name
        {
            get
            {
                if (this.XmlNode.Attributes["Name"] != null)
                    return this.XmlNode.Attributes["Name"].Value;

                return null;
            }
        }

        public bool HasModule
        {
            get
            {
                if (this.ChildNodes.Find(x => x.TypeName == "Module") != null)
                    return true;

                foreach (HomescreenNode item in this.ChildNodes)
                {
                    if (item.HasModule)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the xml node that contains
        /// the homescreen node's definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets a list of the
        /// homescreen node's child nodes.
        /// </summary>
        public List<HomescreenNode> ChildNodes { get; set; }

        /// <summary>
        /// Gets or sets the parent node of the homescreen node.
        /// </summary>
        public HomescreenNode Parent { get; set; }

        public string Height { get; set; }
        public string Width { get; set; }

        #endregion


        #region Constructor

        public HomescreenNode(XmlNode xmlNode, Homescreen owner)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;

            // Parse the homescreen node definition.
            this.Parse();
        }

        public HomescreenNode(XmlNode xmlNode, Homescreen owner, HomescreenNode parent)
            : this(xmlNode, owner)
        {
            this.Parent = parent;
        }

        #endregion


        #region Methods

        private void Parse()
        {
            // Create a new list that contains the
            // homescreen node's child nodes.
            this.ChildNodes = new List<HomescreenNode>();

            // Run through all child nodes of the homescreen node's xml node.
            foreach (XmlNode xmlNode in this.XmlNode.ChildNodes)
            {
                // Create a new homescreen node by the xml node.
                HomescreenNode child = new HomescreenNode(xmlNode, this.Owner, this);

                // Add the child homescreen node to
                // the homescreen node's child nodes.
                this.ChildNodes.Add(child);
            }
        }

        public void Render(StringBuilder writer)
        {
            HomescreenRenderer renderer = null;

            bool renderChildNodes = true;

            string height = this.Height;
            string width = this.Width;

            // Switch on the homescreen node's type name.
            switch (this.TypeName)
            {
                case "Section":
                    // Create a new section renderer.
                    renderer = new HomescreenRendererSection(this);
                    height = ((HomescreenRendererSection)renderer).CalculateHeight();
                    width = ((HomescreenRendererSection)renderer).CalculateWidth();

                    this.Height = height;
                    this.Width = width;
                    break;
                case "Module":
                    // Create a new module renderer.
                    renderer = new HomescreenRendererModule(this);
                    renderChildNodes = false;
                    break;
                case "Label":
                    // Create a new label renderer.
                    renderer = new HomescreenRendererLabel(this);
                    break;
                default:
                    // Create a new html renderer.
                    renderer = new HomescreenRendererHtml(this);
                    break;
            }

            // Render the homescreen node's begin tag.
            renderer.RenderBegin(writer);

            if (renderChildNodes)
            {
                // Run through all child nodes of the homescreen node.
                foreach (HomescreenNode childNode in this.ChildNodes)
                {
                    childNode.Height = height;
                    childNode.Width = width;

                    // Render the homescreen node's child node.
                    childNode.Render(writer);
                }
            }

            // Render the homescreen node's end tag.
            renderer.RenderEnd(writer);
        }

        #endregion
    }
}
