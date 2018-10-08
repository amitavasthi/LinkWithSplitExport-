using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class TaxonomyCategoryLabel : BaseClasses.BaseItem<TaxonomyCategoryLabel>
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

        public int IdLanguage
        {
            get
            {
                return base.GetValue<int>("IdLanguage");
            }
            set
            {
                base.SetValue("IdLanguage", value);
            }
        }
        
        public string Label
        {
            get
            {
                return base.GetValue<string>("Label");
            }
            set
            {
                base.SetValue("Label", value);
            }
        }


        #endregion


        #region Constructor

        public TaxonomyCategoryLabel(BaseClasses.BaseCollection<TaxonomyCategoryLabel> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
