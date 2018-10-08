using Crosstables.Classes.HierarchyClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Crosstables.Classes.Charts.ChartDataRenderers
{
    public abstract class ChartDataRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the source to
        /// the report definition file.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the xPath to
        /// the dimension to render.
        /// </summary>
        public string PathDimension { get; set; }

        public DatabaseCore.Core Core { get; set; }

        public int IdLanguage { get; set; }

        public XmlDocument Document { get; protected set; }

        #endregion


        #region Constructor

        public ChartDataRenderer()
        {

        }

        public ChartDataRenderer(string source, string pathDimension)
        {
            this.Source = source;
            this.PathDimension = pathDimension;

            // Create a new xml document that contains the report definition.
            this.Document = new XmlDocument();

            // Load the contents of the report definition xml file into the xml document.
            this.Document.Load(source);
        }

        #endregion


        #region Methods

        public abstract void Render(StringBuilder writer, HierarchyFilter hierarchyFilter);

        #endregion
    }
}
