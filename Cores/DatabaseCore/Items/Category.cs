using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class Category : BaseClasses.BaseItem<Category>
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

        public string Formula
        {
            get
            {
                return base.GetValue<string>("Formula");
            }
            set
            {
                base.SetValue("Formula", value);
            }
        }

        public double? Factor
        {
            get
            {
                return base.GetValue<double?>("Factor");
            }
            set
            {
                base.SetValue("Factor", value);
            }
        }

        public string ClearText
        {
            get
            {
                return base.GetValue<string>("ClearText");
            }
            set
            {
                base.SetValue("ClearText", value);
            }
        }

        public double Value
        {
            get
            {
                return base.GetValue<double>("Value");
            }
            set
            {
                base.SetValue("Value", value);
            }
        }

        public int Order
        {
            get
            {
                return base.GetValue<int>("Order");
            }
            set
            {
                base.SetValue("Order", value);
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

        public Category(BaseClasses.BaseCollection<Category> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion


        #region Methods

        public string GetLabelText(int language)
        {
            CategoryLabel label = ((DatabaseCore.Core)this.Owner.Owner).CategoryLabels.GetSingle(
                new string[] { "IdCategory", "IdLanguage" },
                new object[] { this.Id, language }
            );

            if (label == null)
                return "";

            return label.Label;
        }

        #endregion
    }
}
