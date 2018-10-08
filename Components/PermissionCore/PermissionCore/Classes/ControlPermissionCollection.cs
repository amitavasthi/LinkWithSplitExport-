using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionCore.Classes
{
    public class ControlPermissionCollection
    {
        #region Properties

        public List<ControlPermission> Items { get; set; }

        #endregion


        #region Constructor

        public ControlPermissionCollection()
        {
            this.Items = new List<ControlPermission>();
        }

        #endregion


        #region Methods

        public void Add(ControlPermission item)
        {
            this.Items.Add(item);
        }

        #endregion


        #region Operators

        public ControlPermission this[string idButton]
        {
            get
            {
                return this.Items.Find(x => x.IdButton == idButton);
            }
        }

        #endregion
    }
}
