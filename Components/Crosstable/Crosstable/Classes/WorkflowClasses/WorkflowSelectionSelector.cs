using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Crosstables.Classes.WorkflowClasses
{
    public abstract class WorkflowSelectionSelector : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning workflow selection.
        /// </summary>
        public WorkflowSelection Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node which contains the
        /// definition of the variable selection.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the name of the worflow selection variable.
        /// </summary>
        public string Name
        {
            get
            {
                if (this.XmlNode.Attributes["Name"] == null)
                    return "";

                return this.XmlNode.Attributes["Name"].Value;
            }
            set
            {
                this.XmlNode.Attributes["Name"].Value = value;
            }
        }

        public bool IsDefaultSelection
        {
            get
            {
                if (this.XmlNode.Attributes["IsDefaultSelection"] == null)
                    return false;

                return bool.Parse(this.XmlNode.Attributes["IsDefaultSelection"].Value);
            }
            set
            {
                if (this.XmlNode.Attributes["IsDefaultSelection"] == null)
                    this.XmlNode.AddAttribute("IsDefaultSelection", value.ToString());
                else
                    this.XmlNode.Attributes["IsDefaultSelection"].Value = value.ToString();
            }
        }

        public WorkflowSelector Selector { get; set; }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        public void Select(Guid idItem)
        {
            this.Selector.SelectedItems.Add(idItem);

            this.XmlNode.InnerXml += string.Format(
                "<Category Id=\"{0}\"></Category>",
                idItem
            );
        }

        public void DeSelect(Guid idItem)
        {
            this.Selector.SelectedItems.Remove(idItem);

            XmlNode xmlNodeItem = this.XmlNode.SelectSingleNode(string.Format(
                "Category[@Id=\"{0}\"]",
                idItem
            ));

            if (xmlNodeItem != null)
                this.XmlNode.RemoveChild(xmlNodeItem);
        }

        #endregion
    }
}
