using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PermissionCore.Classes
{
    public class Section
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning section collection.
        /// </summary>
        public SectionCollection Owner { get; set; }

        /// <summary>
        /// Gets or sets the id of the section.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Gets the permission of this section.
        /// </summary>
        public Permission[] Permissions
        {
            get
            {
                return this.Owner.Owner.Permissions.Items.FindAll(
                    x => x.IdSection == this.Id
                ).ToArray();
            }
        }

        #endregion


        #region Constructor

        public Section(SectionCollection owner, XmlNode xmlNode)
        {
            // Set the owner.
            this.Owner = owner;

            // Parse the id of the section.
            this.Id = int.Parse(xmlNode.Attributes["Id"].Value);

            // Get the name of the section.
            this.Name = xmlNode.Attributes["Name"].Value;
        }

        #endregion
    }
}
