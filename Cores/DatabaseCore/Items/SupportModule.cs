using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class SupportModule : BaseClasses.BaseItem<SupportModule>
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

        public string SupportId
        {
            get
            {
                return base.GetValue<string>("SupportId");
            }
            set
            {
                base.SetValue("SupportId", value);
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

        public string OSDetails
        {
            get
            {
                return base.GetValue<string>("OSDetails");
            }
            set
            {
                base.SetValue("OSDetails", value);
            }
        }
        public string BrowserDetails
        {
            get
            {
                return base.GetValue<string>("BrowserDetails");
            }
            set
            {
                base.SetValue("BrowserDetails", value);
            }
        }
        public string Module
        {
            get
            {
                return base.GetValue<string>("Module");
            }
            set
            {
                base.SetValue("Module", value);
            }
        }
        public string SnapshotPath
        {
            get
            {
                return base.GetValue<string>("SnapshotPath");
            }
            set
            {
                base.SetValue("SnapshotPath", value);
            }
        }

        public string Status
        {
            get
            {
                return base.GetValue<string>("Status");
            }
            set
            {
                base.SetValue("Status", value);
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return base.GetValue<DateTime>("CreatedDate");
            }
            set
            {
                base.SetValue("CreatedDate", value);
            }
        }
        public DateTime ModifiedDate
        {
            get
            {
                return base.GetValue<DateTime>("ModifiedDate");
            }
            set
            {
                base.SetValue("ModifiedDate", value);
            }
        }

        #endregion

          #region Constructor

        public SupportModule(BaseClasses.BaseCollection<SupportModule> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
