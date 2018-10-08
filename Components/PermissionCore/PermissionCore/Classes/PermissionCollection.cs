using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PermissionCore.Classes
{
    public class PermissionCollection
    {
        #region Properties

        public List<Permission> Items { get; private set; }

        public PermissionCore Owner { get; set; }

        #endregion


        #region Constructor

        public PermissionCollection(PermissionCore owner)
        {
            this.Items = new List<Permission>();
            this.Owner = owner;
        }

        #endregion


        #region Methods

        public void Parse(XmlNode xmlNode)
        {
            XmlNodeList xmlNodesPermissions = xmlNode.SelectNodes("Permission");

            foreach (XmlNode xmlNodePermission in xmlNodesPermissions)
            {
                Permission permission = new Permission(xmlNodePermission, this.Owner);

                this.Items.Add(permission);
            }
        }

        #endregion


        #region Operators

        public Permission this[string name]
        {
            get
            {
                return this.Items.Find(x => x.Name == name);
            }
        }

        public Permission this[int id]
        {
            get
            {
                return this.Items.Find(x => x.Id == id);
            }
        }

        #endregion
    }
}
