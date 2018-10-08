using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class WorkgroupHierarchy : BaseClasses.BaseItem<WorkgroupHierarchy>
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
        /// Gets or sets the id of the hierarchy.
        /// </summary>
        public Guid IdHierarchy
        {
            get
            {
                return base.GetValue<Guid>("IdHierarchy");
            }
            set
            {
                base.SetValue("IdHierarchy", value);
            }
        }

        #endregion


        #region Constructor

        public WorkgroupHierarchy(BaseClasses.BaseCollection<WorkgroupHierarchy> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
