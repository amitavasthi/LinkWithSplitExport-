using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PermissionCore.Classes
{
    public class GridColumnPermission
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning grid column permission collection.
        /// </summary>
        public GridColumnPermissionCollection Owner { get; set; }

        /// <summary>
        /// Gets or sets the id of the grid column.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the permission id of the grid column.
        /// </summary>
        public int IdPermission { get; set; }


        /// <summary>
        /// Gets the permission of the grid column.
        /// </summary>
        public Permission Permission 
        {
            get
            {
                return this.Owner.Owner.Permissions[this.IdPermission];
            }
        }

        #endregion


        #region Constructor

        public GridColumnPermission(GridColumnPermissionCollection owner, XmlNode xmlNode)
        {
            // Set the owner.
            this.Owner = owner;

            // Get the id of the grid column.
            this.Id = xmlNode.Attributes["Id"].Value;

            // Parse the permission id of the grid column.
            this.IdPermission = int.Parse(xmlNode.Attributes["Permission"].Value);
        }

        #endregion
    }
}
