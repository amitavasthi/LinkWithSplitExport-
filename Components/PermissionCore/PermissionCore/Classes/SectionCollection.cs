using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PermissionCore.Classes
{
    public class SectionCollection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning permission core.
        /// </summary>
        public PermissionCore Owner { get; set; }

        public List<Section> Items { get; private set; }

        #endregion


        #region Constructor

        public SectionCollection(PermissionCore owner)
        {
            this.Items = new List<Section>();
            this.Owner = owner;
        }

        #endregion


        #region Methods

        public void Parse(XmlNode xmlNode)
        {
            // Get all section xml nodes.
            XmlNodeList xmlNodesSections = xmlNode.SelectNodes("Section");

            // Run through all section xml nodes.
            foreach (XmlNode xmlNodeSection in xmlNodesSections)
            {
                // Create a new section using the xml node.
                Section section = new Section(this, xmlNodeSection);

                // Add the new section to the collection's items.
                this.Items.Add(section);
            }
        }

        #endregion


        #region Operators

        public Section this[int id]
        {
            get
            {
                return this.Items.Find(x => x.Id == id);
            }
        }

        public Section this[string name]
        {
            get
            {
                return this.Items.Find(x => x.Name == name);
            }
        }

        #endregion
    }
}
