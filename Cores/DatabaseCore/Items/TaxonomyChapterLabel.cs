using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class TaxonomyChapterLabel : BaseClasses.BaseItem<TaxonomyChapterLabel>
    {
        #region Properties

        public Guid IdTaxonomyChapter
        {
            get
            {
                return base.GetValue<Guid>("IdTaxonomyChapter");
            }
            set
            {
                base.SetValue("IdTaxonomyChapter", value);
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

        #endregion


        #region Constructor

        public TaxonomyChapterLabel(BaseClasses.BaseCollection<TaxonomyChapterLabel> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
