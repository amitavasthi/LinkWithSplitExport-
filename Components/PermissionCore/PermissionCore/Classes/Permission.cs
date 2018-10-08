using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace PermissionCore.Classes
{
    public class Permission
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the permission.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the permission.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the section.
        /// </summary>
        public int? IdSection { get; set; }

        /// <summary>
        /// Gets or sets the owning permission core.
        /// </summary>
        public PermissionCore Owner { get; set; }


        // Gets the section of the permission.
        public Section Section
        {
            get
            {
                if (!this.IdSection.HasValue)
                    return null;

                return this.Owner.Sections[this.IdSection.Value];
            }
        }

        #endregion


        #region Constructor

        public Permission(PermissionCore owner)
        {
            this.Owner = owner;
        }

        public Permission(XmlNode xmlNode, PermissionCore owner)
            : this(owner)
        {
            this.Name = xmlNode.Attributes["Name"].Value;
            this.Id = int.Parse(xmlNode.Attributes["Id"].Value);

            if (xmlNode.Attributes["Section"] != null)
                this.IdSection = int.Parse(xmlNode.Attributes["Section"].Value);
            else
                this.IdSection = null;
        }

        public Permission(string name, PermissionCore owner)
            : this(owner)
        {
            ReadFromXml("Name", name);
        }

        public Permission(int id, PermissionCore owner)
            : this(owner)
        {
            ReadFromXml("Id", id);
        }

        #endregion


        #region Methods

        private void ReadFromXml(string attribute, object attributeValue)
        {
            XmlNode xmlNode = null;

            xmlNode = this.Owner.XmlDocument.DocumentElement.SelectSingleNode(
                "Permission[@" + attribute + "='"+ attributeValue  +"']"
            );

            if(xmlNode == null)
            {
                xmlNode = this.Owner.XmlDocument.DocumentElement.SelectSingleNode(
                    "Products/Product[@Name='" + this.Owner.ProductName + "']/Permissions/Permission[@" + attribute + "='" + attributeValue + "']"
                );
            }

            if(xmlNode == null ||
                xmlNode.Attributes["Name"] == null ||
                xmlNode.Attributes["Id"] == null)
            {
                throw new Exception(
                    "Permission with " +
                    attribute.ToLower() + " '" +
                    attributeValue.ToString() +
                    "' is not defined."
                );
            }

            this.Name = xmlNode.Attributes["Name"].Value;
            this.Id = int.Parse(xmlNode.Attributes["Id"].Value);
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion


        #region Operators

        /*
        public static explicit operator Permission(string name)
        {
            Permission permission = new Permission(name);

            return permission;
        }

        public static explicit operator Permission(int id)
        {
            Permission permission = new Permission(id);

            return permission;
        }
        */
        
        public static implicit operator int(Permission p)  // implicit digit to byte conversion operator
        {
            return p.Id;
        }

        #endregion
    }
}
