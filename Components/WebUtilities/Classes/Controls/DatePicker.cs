using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtilities.Controls
{
    public class DatePicker : TextBox
    {
        public DateTime Value
        {
            get
            {
                return DateTime.Parse(base.Text);
            }
            set
            {
                base.Text = value.ToString();
            }
        }
    }
}
