using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionCore.Classes
{
    public class PagePermissionCollection
    {
        #region Properties

        public List<PagePermission> Items { get; set; }

        #endregion


        #region Constructor

        public PagePermissionCollection()
        {
            this.Items = new List<PagePermission>();
        }
        
        #endregion


        #region Methods

        public void Add(PagePermission item)
        {
            this.Items.Add(item);
        }

        #endregion


        #region Operators

        public PagePermission this[string pageName]
        {
            get
            {
                return this.Items.Find(x => x.PageName == pageName);
            }
        }

        #endregion
    }
}
