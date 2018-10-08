using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectHierarchy1
{
    public class ProjectHierarchyFieldValue
    {
        #region Properties

        /// <summary>
        /// Gets or sets the field of which the value is part of.
        /// </summary>
        public ProjectHierarchyField Field { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains
        /// the value definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the id of the value.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the labels of the value
        /// seperated by the language id.
        /// </summary>
        public Dictionary<int, string> Labels { get; set; }

        #endregion


        #region Constructor

        public ProjectHierarchyFieldValue(ProjectHierarchyField field, XmlNode xmlNode)
        {
            this.Field = field;
            this.XmlNode = xmlNode;

            Init();
        }

        #endregion


        #region Methods

        private void Init()
        {
            // Parse the value's id.
            this.Id = Guid.Parse(this.XmlNode.Attributes["Id"].Value);

            // Parse the value's labels.
            ParseLabels();
        }

        private void ParseLabels()
        {
            // Create a new dicitionary that contains the
            // value's labels seperated by the language id.
            this.Labels = new Dictionary<int, string>();

            // Run through all label xml nodes of the field's child nodes.
            foreach (XmlNode xmlNodeLabel in this.XmlNode.SelectNodes("Label"))
            {
                // Parse the language id of the label.
                int idLanguage = int.Parse(xmlNodeLabel.Attributes["IdLanguage"].Value);

                // Get the label text.
                string label = xmlNodeLabel.InnerXml;

                // Add the label to the value's labels.
                this.Labels.Add(idLanguage, label);
            }
        }

        #endregion
    }
}
