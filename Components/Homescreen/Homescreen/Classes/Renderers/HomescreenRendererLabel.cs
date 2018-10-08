using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using WebUtilities;

namespace Homescreen1.Classes.Renderers
{
    public class HomescreenRendererLabel : HomescreenRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the amount of sections in the line.
        /// </summary>
        public int SectionCount { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the label renderer.
        /// </summary>
        /// <param name="node">The node to render.</param>
        public HomescreenRendererLabel(HomescreenNode node)
            : base(node)
        { }

        #endregion


        #region Methods

        public override void RenderBegin(StringBuilder writer)
        {
            // Render the tag name.
            writer.Append(string.Format(
                "<{0}>",
                "span"
            ));

            writer.Append(
                ((LanguageManager)HttpContext.Current.Session["LanguageManager"]).GetText(this.Node.Name)
            );
        }

        public override void RenderEnd(StringBuilder writer)
        {
            // Render the closing tag.
            writer.Append(string.Format(
                "</{0}>",
                "span"
            ));
        }

        #endregion
    }
}
