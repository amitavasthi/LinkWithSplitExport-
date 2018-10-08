using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class UserWorkgroup : BaseClasses.BaseItem<UserWorkgroup>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the workgroup.
        /// </summary>
        public Guid IdWorkgroup
        {
            get
            {
                return base.GetValue<Guid>("IdWorkgroup");
            }
            set
            {
                base.SetValue("IdWorkgroup", value);
            }
        }

        /// <summary>
        /// Gets or sets the id of the user.
        /// </summary>
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

        #endregion


        #region Constructor

        public UserWorkgroup(BaseClasses.BaseCollection<UserWorkgroup> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }

    public enum UserWorkgroupStatus
    {
        None = 0,
        ImportFailed = 1,
        Deleting = 2,
        DeletionFailed = 3
    }
}
