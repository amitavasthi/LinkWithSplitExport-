using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class TaxonomyCategoryHierarchy : BaseClasses.BaseItem<TaxonomyCategoryHierarchy>
    {
        #region Properties

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

        public Guid IdHierarchy
        {
            get
            {
                return base.GetValue<Guid>("IdHierarchy");
            }
            set
            {
                base.SetValue("IdHierarchy", value);
            }
        }


        #endregion


        #region Constructor

        public TaxonomyCategoryHierarchy(BaseClasses.BaseCollection<TaxonomyCategoryHierarchy> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
