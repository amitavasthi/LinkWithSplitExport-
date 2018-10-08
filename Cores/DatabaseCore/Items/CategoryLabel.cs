using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class CategoryLabel : BaseClasses.BaseItem<CategoryLabel>
    {
        #region Properties

        public Guid IdCategory
        {
            get
            {
                return base.GetValue<Guid>("IdCategory");
            }
            set
            {
                base.SetValue("IdCategory", value);
            }
        }

        public int IdLanguage
        {
            get
            {
                return base.GetValue<int>("IdLanguage");
            }
            set
            {
                base.SetValue("IdLanguage", value);
            }
        }

        public string Label
        {
            get
            {
                return base.GetValue<string>("Label");
            }
            set
            {
                base.SetValue("Label", value);
            }
        }


        public Category Category
        {
            get
            {
                return ((DatabaseCore.Core)this.Owner.Owner).Categories.GetSingle(this.IdCategory);
            }
        }

        #endregion


        #region Constructor

        public CategoryLabel(BaseClasses.BaseCollection<CategoryLabel> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
