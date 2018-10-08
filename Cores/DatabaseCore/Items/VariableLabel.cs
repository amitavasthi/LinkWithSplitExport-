using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class VariableLabel : BaseClasses.BaseItem<VariableLabel>
    {
        #region Properties

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

        public string ReportLabel
        {
            get
            {
                return base.GetValue<string>("ReportLabel");
            }
            set
            {
                base.SetValue("ReportLabel", value);
            }
        }


        public Variable Variable
        {
            get
            {
                return ((DatabaseCore.Core)this.Owner.Owner).Variables.GetSingle(this.IdVariable);
            }
        }

        #endregion


        #region Constructor

        public VariableLabel(BaseClasses.BaseCollection<VariableLabel> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
