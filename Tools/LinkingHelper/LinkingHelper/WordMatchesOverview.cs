using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkingHelper
{
    public partial class WordMatchesOverview : Form
    {
        public WordMatchesOverview()
        {
            InitializeComponent();
        }

        private void WordMatchesOverview_Load(object sender, EventArgs e)
        {

        }

        public void LoadMatches(List<WordMatch> matches)
        {
            foreach (WordMatch match in matches.OrderByDescending(x => x.Percentage))
            {
                int row = gridView.Rows.Add();

                gridView.Rows[row].Cells["Name1"].Value = match.Variable1;
                gridView.Rows[row].Cells["Name2"].Value = match.Variable2;
                gridView.Rows[row].Cells["Label1"].Value = match.Label1;
                gridView.Rows[row].Cells["Label2"].Value = match.Label2;
                gridView.Rows[row].Cells["Match"].Value = Math.Round(match.Percentage, 2);
            }
        }
    }
}
