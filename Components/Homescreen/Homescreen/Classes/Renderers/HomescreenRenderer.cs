using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homescreen1.Classes.Renderers
{
    public abstract class HomescreenRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the homescreen node to render.
        /// </summary>
        public HomescreenNode Node { get; set; }

        #endregion


        #region Constructor

        public HomescreenRenderer(HomescreenNode node)
        {
            this.Node = node;
        }

        #endregion


        #region Methods
        
        /// <summary>
        /// Renders the homescreen node.
        /// </summary>
        /// <param name="writer">
        /// The string builder to write
        /// the rendered html to.
        /// </param>
        public abstract void RenderBegin(StringBuilder writer);
        public abstract void RenderEnd(StringBuilder writer);

        #endregion
    }
}
