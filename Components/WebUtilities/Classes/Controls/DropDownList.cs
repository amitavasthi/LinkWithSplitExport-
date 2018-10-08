using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtilities.Controls
{
    public class DropDownList : System.Web.UI.WebControls.DropDownList
    {
        #region Properties

        #endregion


        #region Constructor

        public DropDownList()
        {
        }

        #endregion


        #region Methods

        public void BindEnum(Type enumType)
        {
            this.Items.Clear();

            foreach (string value in Enum.GetNames(enumType))
            {
                this.Items.Add(value);
            }
        }

        #endregion
    }
}
