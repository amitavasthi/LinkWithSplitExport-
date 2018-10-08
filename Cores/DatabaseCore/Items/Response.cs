using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DatabaseCore.Items
{
    public class Response : BaseClasses.BaseItem<Response>
    {
        #region Properties

        public Guid IdRespondent
        {
            get
            {
                return base.GetValue<Guid>("IdRespondent");
            }
            set
            {
                base.SetValue("IdRespondent", value);
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

        /*public Guid IdHLevelSlice
        {
            get
            {
                return base.GetValue<Guid>("IdHLevelSlice");
            }
            set
            {
                base.SetValue("IdHLevelSlice", value);
            }
        }

        public Guid IdVariable
        {
            get
            {
                return base.GetValue<Guid>("IdVariable");
            }
            set
            {
                base.SetValue("IdVariable", value);
            }
        }*/

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

        public decimal? NumericAnswer
        {
            get
            {
                //return base.GetValue<decimal?>("NumericAnswer");
                return decimal.Parse(base.GetValue<string>("NumericAnswer"));
            }
            set
            {
                base.SetValue("NumericAnswer", value);
            }
        }

        public string TextAnswer
        {
            get
            {
                return base.GetValue<string>("TextAnswer");
            }
            set
            {
                base.SetValue("TextAnswer", value);
            }
        }


        public Respondent Respondent
        {
            get
            {
                return ((DatabaseCore.Core)this.Owner.Owner).Respondents.GetSingle(this.IdRespondent);
            }
        }

        #endregion


        #region Constructor

        public Response(BaseClasses.BaseCollection<Response> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        public Response(BaseClasses.BaseCollection<Response> collection, XmlNode source)
            : base(collection, source)
        { }

        #endregion
    }
}
