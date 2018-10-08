using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class Workgroup : BaseClasses.BaseItem<Workgroup>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the workgroup.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the creation date of the workgroup.
        /// </summary>
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

        public Workgroup(BaseClasses.BaseCollection<Workgroup> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }

    public enum WorkgroupStatus
    {
        None = 0,
        ImportFailed = 1,
        Deleting = 2,
        DeletionFailed = 3
    }
}
