using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class TaxonomyCategory : BaseClasses.BaseItem<TaxonomyCategory>
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

        public string Name
        {
            get
            {
                return base.GetValue<string>("Name");
            }
            set
            {
                base.SetValue("Name", value);
            }
        }

        public int Order
        {
            get
            {
                return base.GetValue<int>("Order");
            }
            set
            {
                base.SetValue("Order", value);
            }
        }

        public double Value
        {
            get
            {
                return base.GetValue<double>("Value");
            }
            set
            {
                base.SetValue("Value", value);
            }
        }

        public string Color
        {
            get
            {
                return base.GetValue<string>("Color");
            }
            set
            {
                base.SetValue("Color", value);
            }
        }

        public bool IsScoreGroup
        {
            get
            {
                return base.GetValue<bool>("IsScoreGroup");
            }
            set
            {
                base.SetValue("IsScoreGroup", value);
            }
        }

        public bool Enabled
        {
            get
            {
                return base.GetValue<bool>("Enabled");
            }
            set
            {
                base.SetValue("Enabled", value);
            }
        }
        
        
        public TaxonomyVariable Variable
        {
            get
            {
                return ((DatabaseCore.Core)this.Owner.Owner).TaxonomyVariables.GetSingle(this.IdTaxonomyVariable);
            }
        }

        #endregion


        #region Constructor

        public TaxonomyCategory(BaseClasses.BaseCollection<TaxonomyCategory> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
