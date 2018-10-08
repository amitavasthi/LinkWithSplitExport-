using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionCore.Classes
{
    public class ButtonPermissionCollection
    {
        #region Properties

        public List<ButtonPermission> Items { get; set; }

        #endregion


        #region Constructor

        public ButtonPermissionCollection()
        {
            this.Items = new List<ButtonPermission>();
        }

        #endregion


        #region Methods

        public void Add(ButtonPermission item)
        {
            this.Items.Add(item);
        }

        #endregion


        #region Operators

        public ButtonPermission this[string idButton]
        {
            get
            {
                return this.Items.Find(x => x.IdButton == idButton);
            }
        }

        #endregion
    }
}
