using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PermissionCore.Classes
{
    public class GridColumnPermissionCollection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning permission core.
        /// </summary>
        public PermissionCore Owner { get; set; }

        public List<GridColumnPermission> Items { get; private set; }

        #endregion


        #region Constructor

        public GridColumnPermissionCollection(PermissionCore owner)
        {
            this.Items = new List<GridColumnPermission>();
            this.Owner = owner;
        }

        public GridColumnPermissionCollection(PermissionCore owner, XmlNode xmlNode)
            : this(owner)
        {
            // Get all GridColumnPermission xml nodes.
            XmlNodeList xmlNodesGridColumnPermissions = xmlNode.SelectNodes("GridColumn");

            // Run through all GridColumnPermission xml nodes.
            foreach (XmlNode xmlNodeGridColumnPermission in xmlNodesGridColumnPermissions)
            {
                // Create a new GridColumnPermission using the xml node.
                GridColumnPermission GridColumnPermission = new GridColumnPermission(this, xmlNodeGridColumnPermission);

                // Add the new GridColumnPermission to the collection's items.
                this.Items.Add(GridColumnPermission);
            }
        }

        #endregion


        #region Operators

        public GridColumnPermission this[string id]
        {
            get
            {
                return this.Items.Find(x => x.Id == id);
            }
        }

        #endregion
    }
}
