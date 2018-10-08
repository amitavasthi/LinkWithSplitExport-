using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LinkBi1.Classes
{
    public class LinkBiDefinitionCategory : LinkBiDefinitionDimensionScore
    {
        #region Properties

        /// <summary>
        /// Gets the id of the taxonomy category.
        /// </summary>
        public override Guid Identity
        {
            get
            {
                return Guid.Parse(base.GetValue("Id"));
            }
        }

        public override string Label
        {
            get
            {
                return (string)this.Owner.Owner.Core.CategoryLabels.GetValue(
                    "Label",
                    new string[] { "IdCategory", "IdLanguage" },
                    new object[] { this.Identity, this.Owner.Owner.Settings.IdLanguage }
                );
            }
        }

        public override string Name
        {
            get
            {
                return (string)this.Owner.Owner.Core.Categories.GetValue(
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

        public LinkBiDefinitionCategory(LinkBiDefinitionDimension owner, XmlNode xmlNode)
            : base(owner, xmlNode)
        { }

        #endregion


        #region Methods

        #endregion
    }
}
