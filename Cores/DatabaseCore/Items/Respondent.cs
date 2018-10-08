using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class Respondent : BaseClasses.BaseItem<Respondent>
    {
        #region Properties

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

        public string OriginalRespondentID
        {
            get
            {
                return base.GetValue<string>("OriginalRespondentID");
            }
            set
            {
                base.SetValue("OriginalRespondentID", value);
            }
        }

        public string Origin
        {
            get
            {
                return base.GetValue<string>("Origin");
            }
            set
            {
                base.SetValue("Origin", value);
            }
        }

        public int IdCountryCode
        {
            get
            {
                return base.GetValue<int>("IdCountryCode");
            }
            set
            {
                base.SetValue("IdCountryCode", value);
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

        public string Weight1
        {
            get
            {
                return base.GetValue<string>("Weight1");
            }
            set
            {
                base.SetValue("Weight1", value);
            }
        }

        public string Weight2
        {
            get
            {
                return base.GetValue<string>("Weight2");
            }
            set
            {
                base.SetValue("Weight2", value);
            }
        }

        public string SoftwareVersionAtImport
        {
            get
            {
                return base.GetValue<string>("SoftwareVersionAtImport");
            }
            set
            {
                base.SetValue("SoftwareVersionAtImport", value);
            }
        }

        #endregion


        #region Constructor

        public Respondent(BaseClasses.BaseCollection<Respondent> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
