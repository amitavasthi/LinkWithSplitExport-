using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class Company : BaseClasses.BaseItem<Company>
    {
        #region Properties

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
        
        #endregion


        #region Constructor

        public Company(BaseClasses.BaseCollection<Company> collection, DbDataReader reader = null)
            : base(collection, reader)
        {
            if (reader == null)
                this.CreationDate = DateTime.Now;
        }

        #endregion
    }
}
