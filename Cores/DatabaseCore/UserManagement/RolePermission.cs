using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class RolePermission : BaseClasses.BaseItem<RolePermission>
    {
        #region Properties

        public Guid IdRole
        {
            get
            {
                return base.GetValue<Guid>("IdRole");
            }
            set
            {
                base.SetValue("IdRole", value);
            }
        }

        public int Permission
        {
            get
            {
                return base.GetValue<int>("Permission");
            }
            set
            {
                base.SetValue("Permission", value);
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

        public RolePermission(BaseClasses.BaseCollection<RolePermission> collection, DbDataReader reader = null)
            : base(collection, reader)
        {
            if (reader == null)
                this.CreationDate = DateTime.Now;
        }

        #endregion
    }
}
