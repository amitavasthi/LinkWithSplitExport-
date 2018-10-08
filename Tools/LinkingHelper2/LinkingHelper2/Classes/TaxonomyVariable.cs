using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkingHelper2.Classes
{
    public class TaxonomyVariable
    {
        #region Properties

        public Study Study { get; set; }

        /// <summary>
        /// Gets or sets the name of the taxonomy variable.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the label of the taxonomy variable.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a list of the taxonomy variable's categories.
        /// </summary>
        public Dictionary<string, TaxonomyCategory> Categories { get; set; }

        public bool Linked { get; set; }

        public string LinkedTaxonomyVariable { get; set; }

        #endregion


        #region Constructor

        public TaxonomyVariable()
        {
            this.Categories = new Dictionary<string, TaxonomyCategory>();
        }

        public TaxonomyVariable(string name, string label)
            : this()
        {
            this.Name = name;
            this.Label = label;
        }

        public TaxonomyVariable(string name, string label, Study study)
            : this(name, label)
        {
            this.Study = study;
        }

        #endregion


        #region Methods

        #endregion
    }
}
