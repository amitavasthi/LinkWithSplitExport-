using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtilities.Controls
{
    public class NumericBox : TextBox
    {
        public int Value 
        {
            get
            {
                return int.Parse(base.Text);
            }
            set
            {
                base.Text = value.ToString();
            } 
        }
    }
}
