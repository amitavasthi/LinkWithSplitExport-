using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homescreen1.Classes.Modules
{
    public abstract class HomescreenModule
    {
        #region Properties

        /// <summary>
        /// Gets or sets the available width
        /// for the module.
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Gets or sets the available height
        /// for the module.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the module's homescreen node.
        /// </summary>
        public HomescreenNode Node { get; set; }

        #endregion


        #region Constructor

        public HomescreenModule(HomescreenNode node)
        {
            this.Node = node;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Renders the module.
        /// </summary>
        /// <param name="writer">
        /// The string builder to write
        /// the rendered html to.
        /// </param>
        public abstract void Render(StringBuilder writer);

        #endregion
    }
}
