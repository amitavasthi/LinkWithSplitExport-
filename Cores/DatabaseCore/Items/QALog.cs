using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class QALog : BaseClasses.BaseItem<QALog>
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

        public Guid Identity
        {
            get
            {
                return base.GetValue<Guid>("Identity");
            }
            set
            {
                base.SetValue("Identity", value);
            }
        }

        public string Source
        {
            get
            {
                return base.GetValue<string>("Source");
            }
            set
            {
                base.SetValue("Source", value);
            }
        }

        public QALogStatus Status
        {
            get
            {
                return base.GetValue<QALogStatus>("Status");
            }
            set
            {
                base.SetValue("Status", value);
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

        public QALog(BaseClasses.BaseCollection<QALog> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }

    public enum QALogStatus
    {
        Active,
        Withdrawn
    }
}
