using Crosstables.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LinkBi1.Classes
{
    public class LinkBiSettings : BaseReportSettings
    {
        #region Properties


        /// <summary>
        /// Gets or sets if the percentage value should be exported.
        /// </summary>
        public bool ExportPercentage
        {
            get
            {
                return this.GetValue<bool>("ExportPercentage", bool.Parse, true.ToString());
            }
            set
            {
                this.SetValue("ExportPercentage", value.ToString());
            }
        }

        #endregion


        #region Constructor

        public LinkBiSettings(BaseReportDefinition owner, XmlNode xmlNode)
            : base(owner, xmlNode)
        { }

        #endregion
    }
}
