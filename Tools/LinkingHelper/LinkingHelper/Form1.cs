using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkingHelper
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public void LoadMetadata(Metadata taxonomy)
        {
            foreach (string variable in taxonomy.Variables.Keys)
            {
                int row = gridViewVariables.Rows.Add();

                gridViewVariables.Rows[row].Cells["Chapter"].Value = "Default";
                gridViewVariables.Rows[row].Cells["Type"].Value = "SINGLE";
                gridViewVariables.Rows[row].Cells["Name"].Value = variable;
                gridViewVariables.Rows[row].Cells["Label"].Value = taxonomy.Variables[variable];
            }
        }
    }
}
