using Homescreen1.Classes.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homescreen1.Classes.Renderers
{
    public class HomescreenRendererModule : HomescreenRenderer
    {
        #region Properties

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the module renderer.
        /// </summary>
        /// <param name="node">The node to render.</param>
        public HomescreenRendererModule(HomescreenNode node)
            : base(node)
        { }

        #endregion


        #region Methods

        public override void RenderBegin(StringBuilder writer)
        {
            // Open the module's div tag.
            writer.Append(string.Format(
                "<div class=\"Module Module{0}\" HeightScript=\"{1}\" WidthScript=\"{2}\">",
                this.Node.Name,
                this.Node.Height,
                this.Node.Width
            ));

            HomescreenModule module = null;

            // Switch on the module name.
            switch (this.Node.Name)
            {
                case "RecentUsed":
                    // Create a new recent used module.
                    module = new HomescreenModuleRecentUsed(this.Node);
                    break;
                case "Chat":
                    // Create a new chat module.
                    module = new HomescreenModuleChat(this.Node);
                    break;
                case "Chart":
                    // Create a new chart module.
                    module = new HomescreenModuleChart(this.Node);
                    break;
                case "News":
                    // Create a new news module.
                    module = new HomescreenModuleNews(this.Node);
                    break;
            }

            // Check if the module exists.
            if (module == null)
                return;

            // Render the module.
            module.Render(writer);
        }

        public override void RenderEnd(StringBuilder writer)
        {
            // Close the module's div tag.
            writer.Append("</div>");
        }

        #endregion
    }
}
