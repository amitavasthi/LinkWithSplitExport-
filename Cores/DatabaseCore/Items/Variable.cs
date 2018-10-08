using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class Variable : BaseClasses.BaseItem<Variable>
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

        public Guid IdChapter
        {
            get
            {
                return base.GetValue<Guid>("IdChapter");
            }
            set
            {
                base.SetValue("IdChapter", value);
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
        }

        public VariableType Type
        {
            get
            {
                int number = base.GetValue<int>("Type");

                if (number == 1)
                    number = 6;

                return (VariableType)number;
            }
            set
            {
                base.SetValue("Type", value);
            }
        }

        public string RangeExpression
        {
            get
            {
                return base.GetValue<string>("RangeExpression");
            }
            set
            {
                base.SetValue("RangeExpression", value);
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

        public string AdditionalInfo
        {
            get
            {
                return base.GetValue<string>("AdditionalInfo");
            }
            set
            {
                base.SetValue("AdditionalInfo", value);
            }
        }

        public string Option1
        {
            get
            {
                return base.GetValue<string>("Option1");
            }
            set
            {
                base.SetValue("Option1", value);
            }
        }

        public string Option2
        {
            get
            {
                return base.GetValue<string>("Option2");
            }
            set
            {
                base.SetValue("Option2", value);
            }
        }

        public string Option3
        {
            get
            {
                return base.GetValue<string>("Option3");
            }
            set
            {
                base.SetValue("Option3", value);
            }
        }

        public string Option4
        {
            get
            {
                return base.GetValue<string>("Option4");
            }
            set
            {
                base.SetValue("Option4", value);
            }
        }

        public string Option5
        {
            get
            {
                return base.GetValue<string>("Option5");
            }
            set
            {
                base.SetValue("Option5", value);
            }
        }

        public string ReportFilter
        {
            get
            {
                return base.GetValue<string>("ReportFilter");
            }
            set
            {
                base.SetValue("ReportFilter", value);
            }
        }

        public string ReportVariable
        {
            get
            {
                return base.GetValue<string>("ReportVariable");
            }
            set
            {
                base.SetValue("ReportVariable", value);
            }
        }

        public int ScaleType
        {
            get
            {
                return base.GetValue<int>("ScaleType");
            }
            set
            {
                base.SetValue("ScaleType", value);
            }
        }

        public int ChapterOrder
        {
            get
            {
                return base.GetValue<int>("ChapterOrder");
            }
            set
            {
                base.SetValue("ChapterOrder", value);
            }
        }

        public int VariableOrderInChapter
        {
            get
            {
                return base.GetValue<int>("VariableOrderInChapter");
            }
            set
            {
                base.SetValue("VariableOrderInChapter", value);
            }
        }

        public string IdMeasure
        {
            get
            {
                return base.GetValue<string>("IdMeasure");
            }
            set
            {
                base.SetValue("IdMeasure", value);
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

        public bool Scale
        {
            get
            {
                return base.GetValue<bool>("Scale");
            }
            set
            {
                base.SetValue("Scale", value);
            }
        }


        public Category[] Categories
        {
            get
            {
                return ((DatabaseCore.Core)this.Owner.Owner).Categories.Get("IdVariable", this.Id);
            }
        }

        #endregion


        #region Constructor

        public Variable(BaseClasses.BaseCollection<Variable> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion


        #region Methods

        public string GetLabelText(int language)
        {
            string labelText = (string)((DatabaseCore.Core)this.Owner.Owner).VariableLabels.GetValue(
                "Label",
                new string[] { "IdVariable", "IdLanguage" },
                new object[] { this.Id, language }
            );

            return labelText;
        }

        #endregion

        public void SetLabelText(string text, int idLanguage)
        {
            VariableLabel label = ((DatabaseCore.Core)this.Owner.Owner).VariableLabels.GetSingle(
                new string[] { "IdVariable", "IdLanguage" },
                new object[] { this.Id, idLanguage}
            );

            if (label == null)
                return;

            label.Label = text;

            label.Save();
        }
    }

    public enum VariableType
    {
        Text = 2,
        Single = 3,
        Multi = 4,
        Numeric = 6
    }
}
