using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace LinkOnline.Classes.Controls
{
    public class HierarchyFilterDefinition : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the xml node that contains 
        /// the hierarchy filter definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        #endregion


        #region Constructor

        public HierarchyFilterDefinition(XmlNode xmlNode)
        {
            this.XmlNode = xmlNode;

            this.Load += HierarchyFilterDefinition_Load;
        }

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        private void HierarchyFilterDefinition_Load(object sender, EventArgs e)
        {
            this.CssClass = "HierarchyFilter BackgroundColor7";
        }

        #endregion
    }
}