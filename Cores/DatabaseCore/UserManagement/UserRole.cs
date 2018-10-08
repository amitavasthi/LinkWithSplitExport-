using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class UserRole : BaseClasses.BaseItem<UserRole>
    {
        #region Properties

        public Guid IdUser
        {
            get
            {
                return base.GetValue<Guid>("IdUser");
            }
            set
            {
                base.SetValue("IdUser", value);
            }
        }

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

        public UserRole(BaseClasses.BaseCollection<UserRole> collection, DbDataReader reader = null)
            : base(collection, reader)
        {
            if (reader == null)
                this.CreationDate = DateTime.Now;
        }

        #endregion
    }
}
