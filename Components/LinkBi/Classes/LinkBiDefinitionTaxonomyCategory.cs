using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LinkBi1.Classes
{
    public class LinkBiDefinitionTaxonomyCategory : LinkBiDefinitionDimensionScore
    {
        #region Properties

        private Guid? identity;

        /// <summary>
        /// Gets the id of the taxonomy category.
        /// </summary>
        public override Guid Identity
        {
            get
            {
                if(!identity.HasValue)
                    identity = Guid.Parse(base.GetValue("Id"));

                return identity.Value;
            }
        }

        public override string Label
        {
            get
            {
                return (string)this.Owner.Owner.Core.TaxonomyCategoryLabels.GetValue(
                    "Label",
                    new string[] { "IdTaxonomyCategory", "IdLanguage" },
                    new object[] { this.Identity, this.Owner.Owner.Settings.IdLanguage }
                );
            }
        }

        public override string Name
        {
            get
            {
                return (string)this.Owner.Owner.Core.TaxonomyCategories.GetValue(
                    "Name",
                    "Id",
                    this.Identity
                );
            }
        }

        public override string Equation
        {
            get { return null; }
            set { }
        }

        #endregion


        #region Constructor

        public LinkBiDefinitionTaxonomyCategory(LinkBiDefinitionDimension owner, XmlNode xmlNode)
            : base(owner, xmlNode)
        { }

        #endregion


        #region Methods

        #endregion
    }
}
