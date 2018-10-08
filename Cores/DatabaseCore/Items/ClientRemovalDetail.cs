using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class ClientRemovalDetail : BaseClasses.BaseItem<ClientRemovalDetail>
    {
        #region Properties

        public Guid UserId
        {
            get
            {
                return base.GetValue<Guid>("UserId");
            }
            set
            {
                base.SetValue("UserId", value);
            }
        }

        public string ClientName
        {
            get
            {
                return base.GetValue<string>("ClientName");
            }
            set
            {
                base.SetValue("ClientName", value);
            }
        }

        public Guid ActivationCode
        {
            get
            {
                return base.GetValue<Guid>("ActivationCode");
            }
            set
            {
                base.SetValue("ActivationCode", value);
            }
        }

        public string RemovalReason
        {
            get
            {
                return base.GetValue<string>("RemovalReason");
            }
            set
            {
                base.SetValue("RemovalReason", value);
            }
        }

        public bool IsApproved
        {
            get
            {
                return base.GetValue<bool>("IsApproved");
            }
            set
            {
                base.SetValue("IsApproved", value);
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

        public ClientRemovalDetail(BaseClasses.BaseCollection<ClientRemovalDetail> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
