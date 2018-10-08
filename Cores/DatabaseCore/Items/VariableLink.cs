using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class VariableLink : BaseClasses.BaseItem<VariableLink>
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

        #endregion


        #region Constructor

        public VariableLink(BaseClasses.BaseCollection<VariableLink> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion


        #region Methods

        #endregion
    }
}
