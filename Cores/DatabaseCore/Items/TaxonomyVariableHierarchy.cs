using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class TaxonomyVariableHierarchy : BaseClasses.BaseItem<TaxonomyVariableHierarchy>
    {
        #region Properties

        public Guid IdTaxonomyVariable
        {
            get
            {
                return base.GetValue<Guid>("IdTaxonomyVariable");
            }
            set
            {
                base.SetValue("IdTaxonomyVariable", value);
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

        public TaxonomyVariableHierarchy(BaseClasses.BaseCollection<TaxonomyVariableHierarchy> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
