using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class TaxonomyChapter : BaseClasses.BaseItem<TaxonomyChapter>
    {
        #region Properties

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

        public TaxonomyChapter(BaseClasses.BaseCollection<TaxonomyChapter> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
