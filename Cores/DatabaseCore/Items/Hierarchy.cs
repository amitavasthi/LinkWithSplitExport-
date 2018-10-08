using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class Hierarchy : BaseClasses.BaseItem<Hierarchy>
    {
        #region Properties

        public Guid? IdHierarchy
        {
            get
            {
                return base.GetValue<Guid?>("IdHierarchy");
            }
            set
            {
                base.SetValue("IdHierarchy", value);
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

        #endregion


        #region Constructor

        public Hierarchy(BaseClasses.BaseCollection<Hierarchy> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
