using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkingHelper2.Classes
{
    public class TaxonomyCategory
    {
        #region Properties

        /// <summary>
        /// Gets or sets the taxonomy variable
        /// where the category is defined in.
        /// </summary>
        public TaxonomyVariable TaxonomyVariable { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy category's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy category's label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy category's value.
        /// </summary>
        public double Value { get; set; }

        #endregion


        #region Constructor

        public TaxonomyCategory(TaxonomyVariable taxonomyVariable, string name, string label)
        {
            this.TaxonomyVariable = taxonomyVariable;
            this.Name = name;
            this.Label = label;
        }

        #endregion


        #region Methods

        #endregion
    }
}
