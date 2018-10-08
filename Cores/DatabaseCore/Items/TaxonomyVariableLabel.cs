using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class TaxonomyVariableLabel : BaseClasses.BaseItem<TaxonomyVariableLabel>
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

        public TaxonomyVariableLabel(BaseClasses.BaseCollection<TaxonomyVariableLabel> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
