using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class ScoreGroupLink : BaseClasses.BaseItem<ScoreGroupLink>
    {
        #region Properties

        public Guid IdScoreGroup
        {
            get
            {
                return base.GetValue<Guid>("IdScoreGroup");
            }
            set
            {
                base.SetValue("IdScoreGroup", value);
            }
        }

        public Guid IdTaxonomyCategory
        {
            get
            {
                return base.GetValue<Guid>("IdTaxonomyCategory");
            }
            set
            {
                base.SetValue("IdTaxonomyCategory", value);
            }
        }
        

        #endregion


        #region Constructor

        public ScoreGroupLink(BaseClasses.BaseCollection<ScoreGroupLink> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
