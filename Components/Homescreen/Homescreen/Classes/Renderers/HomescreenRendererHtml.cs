using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Homescreen1.Classes.Renderers
{
    public class HomescreenRendererHtml : HomescreenRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the amount of sections in the line.
        /// </summary>
        public int SectionCount { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the section renderer.
        /// </summary>
        /// <param name="node">The node to render.</param>
        public HomescreenRendererHtml(HomescreenNode node)
            : base(node)
        { }

        #endregion


        #region Methods

        public override void RenderBegin(StringBuilder writer)
        {
            if (this.Node.XmlNode.NodeType == XmlNodeType.Text ||
                this.Node.XmlNode.NodeType == XmlNodeType.CDATA)
            {
                writer.Append(this.Node.XmlNode.InnerText);
                return;
            }

            // Render the tag name.
            writer.Append(string.Format(
                "<{0}",
                this.Node.TypeName
            ));

            // Run through all attributes of the homescreen node's xml node.
            foreach (XmlAttribute attribute in this.Node.XmlNode.Attributes)
            {
                // Render the attribute.
                writer.Append(string.Format(
                    " {0}=\"{1}\"",
                    attribute.Name,
                    attribute.Value
                ));
            }

            writer.Append(">");
        }

        public override void RenderEnd(StringBuilder writer)
        {
            if (this.Node.XmlNode.NodeType == XmlNodeType.Text ||
                this.Node.XmlNode.NodeType == XmlNodeType.CDATA)
            {
                return;
            }

            // Render the closing tag.
            writer.Append(string.Format(
                "</{0}>",
                this.Node.TypeName
            ));
        }

        #endregion
    }
}
