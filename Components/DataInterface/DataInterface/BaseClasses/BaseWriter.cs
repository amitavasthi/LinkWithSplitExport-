using DatabaseCore;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInterface.BaseClasses
{
    public abstract class BaseWriter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the language of the export.
        /// </summary>
        public CultureInfo Language { get; set; }

        /// <summary>
        /// Gets or sets the writer's database core.
        /// </summary>
        public Core Core { get; set; }

        /// <summary>
        /// Gets or sets the current export progress of the metadata.
        /// </summary>
        public double MetadataProgress { get; protected set; }

        /// <summary>
        /// Gets or sets the current export progress of the case data.
        /// </summary>
        public double CaseDataProgress { get; protected set; }

        /// <summary>
        /// Gets or sets the list of respondents for the export.
        /// </summary>
        public List<Guid> Respondents { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the base writer.
        /// </summary>
        /// <param name="language">The language of the export.</param>
        public BaseWriter(Core core, CultureInfo language)
        {
            this.Core = core;
            this.Language = language;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Exports a project.
        /// </summary>
        /// <param name="variables">The variables to export.</param>
        public abstract string Export(TaxonomyVariable[] variables);

        #endregion
    }
}
