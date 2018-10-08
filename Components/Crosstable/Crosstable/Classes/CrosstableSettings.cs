using Crosstables.Classes.ReportDefinitionClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Crosstables.Classes
{
    public class CrosstableSettings : BaseReportSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets if values should be display in the report.
        /// </summary>
        public bool ShowValues
        {
            get
            {
                return this.GetValue<bool>("ShowValues", bool.Parse, "True")
                ;
            }
            set
            {
                this.SetValue("ShowValues", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if percentages should be display in the report.
        /// </summary>
        public bool ShowPercentage
        {
            get
            {
                return this.GetValue<bool>("ShowPercentage", bool.Parse, "True");
            }
            set
            {
                this.SetValue("ShowPercentage", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the minimum width of the table cells.
        /// </summary>
        public int MinWidth
        {
            get
            {
                return this.GetValue<int>("MinWidth", int.Parse, "60");
            }
            set
            {
                this.SetValue("MinWidth", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the minimum height of the table cells.
        /// </summary>
        public int MinHeight
        {
            get
            {
                return this.GetValue<int>("MinHeight", int.Parse, "45");
            }
            set
            {
                this.SetValue("MinHeight", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets how many decimal places of values should be displayed.
        /// </summary>
        public int DecimalPlaces
        {
            get
            {
                return this.GetValue<int>("DecimalPlaces", int.Parse, "0");
            }
            set
            {
                this.SetValue("DecimalPlaces", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if the significance tests should use the effective base.
        /// </summary>
        public bool SigDiffEffectiveBase
        {
            get
            {
                return this.GetValue<bool>("SigDiffEffectiveBase", bool.Parse, "False");
            }
            set
            {
                this.SetValue("SigDiffEffectiveBase", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if significance tests should be display in the report.
        /// </summary>
        public bool SignificanceTest
        {
            get
            {
                return this.GetValue<bool>("SignificanceTest", bool.Parse, "True");
            }
            set
            {
                this.SetValue("SignificanceTest", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the significance test level.
        /// </summary>
        public int SignificanceTestLevel
        {
            get
            {
                return this.GetValue<int>("SignificanceTestLevel", int.Parse, "95");
            }
            set
            {
                this.SetValue("SignificanceTestLevel", value.ToString());
            }
        }
        /// <summary>
        /// Gets or sets the Significance Weight.
        /// </summary>
        public int SignificanceWeight
        {
            get
            {
                return this.GetValue<int>("SignificanceWeight", int.Parse, "2");
            }
            set
            {
                this.SetValue("SignificanceWeight", value.ToString());
            }
        }
        public int SignificanceTestType
        {
            get
            {               
                {
                    return this.GetValue<int>("SignificanceTestType", int.Parse, "0");
                }
               
            }
            set
            {
                this.SetValue("SignificanceTestType", value.ToString());
            }
        }
        /// <summary>
        /// Gets or sets the width for the value table cells.
        /// </summary>
        public int TableWidth
        {
            get
            {
                return this.GetValue<int>("TableWidth", int.Parse, "1000");
            }
            set
            {
                this.SetValue("TableWidth", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the height for the value table cells.
        /// </summary>
        public int TableHeight
        {
            get
            {
                return this.GetValue<int>("TableHeight", int.Parse, "500");
            }
            set
            {
                this.SetValue("TableHeight", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the display type of the report.
        /// </summary>
        public DisplayType DisplayType
        {
            get
            {
                return this.GetValue<DisplayType>("DisplayType", delegate (string value)
                {
                    return (DisplayType)Enum.Parse(
                        typeof(DisplayType),
                        value
                    );
                }, "Crosstable");
            }
            set
            {
                this.SetValue("DisplayType", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the percentage base of the report.
        /// </summary>
        public PercentageBase PercentageBase
        {
            get
            {
                return this.GetValue<PercentageBase>("PercentageBase", delegate (string value)
                {
                    return (PercentageBase)Enum.Parse(
                        typeof(PercentageBase),
                        value
                    );
                }, "Column");
            }
            set
            {
                this.SetValue("PercentageBase", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets what the PowerBI connector should render.
        /// </summary>
        public PowerBIValues PowerBIValues
        {
            get
            {
                return this.GetValue<PowerBIValues>("PowerBIValues", delegate (string value)
                {
                    return (PowerBIValues)Enum.Parse(
                        typeof(PowerBIValues),
                        value
                    );
                }, "Values");
            }
            set
            {
                this.SetValue("PowerBIValues", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if the report definition
        /// should auto-update data.
        /// </summary>
        public bool AutoUpdate
        {
            get
            {
                return this.GetValue<bool>("AutoUpdate", bool.Parse, "False");
            }
            set
            {
                this.SetValue("AutoUpdate", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if the table should scroll
        /// the variable and category labels.
        /// </summary>
        public bool ScrollLabels
        {
            get
            {
                return this.GetValue<bool>("ScrollLabels", bool.Parse, "True");
            }
            set
            {
                this.SetValue("ScrollLabels", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if the table should
        /// hide empty rows and columns.
        /// </summary>
        public bool HideEmptyRowsAndColumns
        {
            get
            {
                return this.GetValue<bool>("HideEmptyRowsAndColumns", bool.Parse, "False");
            }
            set
            {
                this.SetValue("HideEmptyRowsAndColumns", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if the aggregation should order
        /// left categories descending by their base.
        /// </summary>
        public bool RankLeft
        {
            get
            {
                return this.GetValue<bool>("RankLeft", bool.Parse, "False");
            }
            set
            {
                this.SetValue("RankLeft", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if the aggregation should order
        /// top categories descending by their base.
        /// </summary>
        public bool RankTop
        {
            get
            {
                return this.GetValue<bool>("RankTop", bool.Parse, "False");
            }
            set
            {
                this.SetValue("RankTop", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the category limit.
        /// </summary>
        public int CategoryLimit
        {
            get
            {
                return this.GetValue<int>("CategoryLimit", int.Parse, "500");
            }
            set
            {
                if (value <= 0)
                    value = 500;

                this.SetValue("CategoryLimit", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the base type.
        /// </summary>
        public BaseType BaseType
        {
            get
            {
                return (BaseType)this.GetValue<int>("BaseType", int.Parse, "0");
            }
            set
            {
                this.SetValue("BaseType", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Gets or sets the minimum base.
        /// </summary>
        public int LowBase
        {
            get
            {
                return this.GetValue<int>("LowBase", int.Parse, "0");
            }
            set
            {
                this.SetValue("LowBase", value.ToString());
            }
        }

        /// <summary>
        /// Low base on selected base consider value
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="xmlNode"></param>
        public int LowBaseConsider
        {
            get
            {
                return this.GetValue<int>("LowBaseConsider", int.Parse, "2");
            }
            set
            {
                this.SetValue("LowBaseConsider", value.ToString());
            }
        }

        #endregion


        #region Constructor

        public CrosstableSettings(BaseReportDefinition owner, XmlNode xmlNode)
            : base(owner, xmlNode)
        { }

        #endregion
    }

    public enum BaseType
    {
        AnsweringBase,
        TotalBase
    }

    public enum DisplayType
    {
        Crosstable,
        Column,
        Line,
        Bar,
        Scatter,
        SpiderWeb,
        WindRose,
        Pie,
        Area,
        WordCloud
    }

    public enum PercentageBase
    {
        Row,
        Column
    }

    public enum PowerBIValues
    {
        Values,
        Percentages
    }
}
