using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionCore.Classes
{
    public class PagePermission
    {
        #region Properties

        public PermissionCore Owner { get; set; }

        public string PageName { get; set; }

        public Permission Permission { get; set; }

        public ButtonPermissionCollection ButtonPermissions { get; set; }

        public ControlPermissionCollection ControlPermissions { get; set; }

        public GridColumnPermissionCollection GridColumnPermissions { get; set; }

        #endregion


        #region Constructor

        public PagePermission(PermissionCore owner)
        {
            this.Owner = owner;

            this.ButtonPermissions = new ButtonPermissionCollection();
            this.ControlPermissions = new ControlPermissionCollection();
            this.GridColumnPermissions = new GridColumnPermissionCollection(this.Owner);
        }

        public PagePermission(PermissionCore owner, string pageName, Permission permission)
            :this(owner)
        {
            this.PageName = pageName;
            this.Permission = permission;
        }

        #endregion
    }
}
