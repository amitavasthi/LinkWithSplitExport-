using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectHierarchy1
{
    public class ProjectHierarchyStudyValue
    {
        #region Properties

        /// <summary>
        /// Gets or sets the field which the study value is part of.
        /// </summary>
        public ProjectHierarchyField Field { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains the study value definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the index of the study value.
        /// </summary>
        public int Index 
        {
            get
            {
                return int.Parse(this.XmlNode.Attributes["Index"].Value);
            }
            set
            {
                this.XmlNode.Attributes["Index"].Value = value.ToString();
            } 
        }

        /// <summary>
        /// Gets or sets the value of the study value.
        /// </summary>
        public string Value
        {
            get
            {
                return this.XmlNode.InnerXml;
            }
            set
            {
                this.XmlNode.InnerXml = value;
            }
        }

        #endregion


        #region Constructor

        public ProjectHierarchyStudyValue(ProjectHierarchyField field, XmlNode xmlNode)
        {
            this.Field = field;
            this.XmlNode = xmlNode;
        }

        #endregion
    }
}
