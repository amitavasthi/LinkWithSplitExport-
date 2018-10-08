using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class TaxonomyCategoryLink : BaseClasses.BaseItem<TaxonomyCategoryLink>
    {
        #region Properties

        public Guid IdScoreGroup
        {
            get
            {
                return base.GetValue<Guid>("IdScoreGroup");
            }
            set
            {
                base.SetValue("IdScoreGroup", value);
            }
        }

        public Guid IdTaxonomyCategory
        {
            get
            {
                return base.GetValue<Guid>("IdTaxonomyCategory");
            }
            set
            {
                base.SetValue("IdTaxonomyCategory", value);
            }
        }

        #endregion


        #region Constructor

        public TaxonomyCategoryLink(BaseClasses.BaseCollection<TaxonomyCategoryLink> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
