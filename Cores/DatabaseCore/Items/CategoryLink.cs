using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class CategoryLink : BaseClasses.BaseItem<CategoryLink>
    {
        #region Properties

        public Guid IdVariable
        {
            get
            {
                return base.GetValue<Guid>("IdVariable");
            }
            set
            {
                base.SetValue("IdVariable", value);
            }
        }

        public Guid IdCategory
        {
            get
            {
                return base.GetValue<Guid>("IdCategory");
            }
            set
            {
                base.SetValue("IdCategory", value);
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

        public DateTime CreationDate
        {
            get
            {
                return base.GetValue<DateTime>("CreationDate");
            }
            set
            {
                base.SetValue("CreationDate", value);
            }
        }

        public int QA
        {
            get
            {
                return base.GetValue<int>("QA");
            }
            set
            {
                base.SetValue("QA", value);
            }
        }

        #endregion


        #region Constructor

        public CategoryLink(BaseClasses.BaseCollection<CategoryLink> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion


        #region Methods

        #endregion
    }
}
