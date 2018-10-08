using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtilities.Controls
{
    public class Table : System.Web.UI.WebControls.Table
    {
        #region Constructor

        public Table()
        {
            this.CellSpacing = 0;
            this.CellPadding = 0;
        }

        #endregion
    }

    public class TableRow : System.Web.UI.WebControls.TableRow
    {
    }

    public class TableCell : System.Web.UI.WebControls.TableCell
    {
    }

    public class TableHeaderCell : System.Web.UI.WebControls.TableHeaderCell
    {

    }
}
