using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class Role : BaseClasses.BaseItem<Role>
    {
        #region Properties

        public Guid IdCompany
        {
            get
            {
                return base.GetValue<Guid>("IdCompany");
            }
            set
            {
                base.SetValue("IdCompany", value);
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

        public string Description
        {
            get
            {
                return base.GetValue<string>("Description");
            }
            set
            {
                base.SetValue("Description", value);
            }
        }

        public bool Hidden
        {
            get
            {
                return base.GetValue<bool>("Hidden");
            }
            set
            {
                base.SetValue("Hidden", value);
            }
        }

        #endregion


        #region Constructor

        public Role(BaseClasses.BaseCollection<Role> collection, DbDataReader reader = null)
            : base(collection, reader)
        {
            if (reader == null)
                this.CreationDate = DateTime.Now;
        }

        #endregion
    }
}
