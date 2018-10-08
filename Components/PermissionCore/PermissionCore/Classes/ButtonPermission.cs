using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionCore.Classes
{
    public class ButtonPermission
    {
        #region Properties

        public string IdButton { get; set; }

        public Permission Permission { get; set; }

        #endregion


        #region Constructor

        public ButtonPermission()
        {

        }

        public ButtonPermission(string idButton, Permission permission)
        {
            this.IdButton = idButton;
            this.Permission = permission;
        }

        #endregion
    }
}
