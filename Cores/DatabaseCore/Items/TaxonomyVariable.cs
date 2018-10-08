using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class TaxonomyVariable : BaseClasses.BaseItem<TaxonomyVariable>
    {
        #region Properties

        public Guid IdStudy
        {
            get
            {
                return base.GetValue<Guid>("IdStudy");
            }
            set
            {
                base.SetValue("IdStudy", value);
            }
        }

        public Guid IdTaxonomyChapter
        {
            get
            {
                return base.GetValue<Guid>("IdTaxonomyChapter");
            }
            set
            {
                base.SetValue("IdTaxonomyChapter", value);
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

        public VariableType Type
        {
            get
            {
                return base.GetValue<VariableType>("Type");
            }
            set
            {
                base.SetValue("Type", value);
            }
        }

        public bool Scale
        {
            get
            {
                return base.GetValue<bool>("Scale");
            }
            set
            {
                base.SetValue("Scale", value);
            }
        }

        public bool Weight
        {
            get
            {
                return base.GetValue<bool>("Weight");
            }
            set
            {
                base.SetValue("Weight", value);
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


        public TaxonomyCategory[] Categories
        {
            get
            {
                return ((DatabaseCore.Core)this.Owner.Owner).TaxonomyCategories.Get("IdTaxonomyVariable", this.Id);
            }
        }

        #endregion


        #region Constructor

        public TaxonomyVariable(BaseClasses.BaseCollection<TaxonomyVariable> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
