using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkingHelper
{
    public class WordMatch
    {
        #region Properties

        public string File1 { get; set; }

        public string File2 { get; set; }


        public string Variable1 { get; set; }

        public string Variable2 { get; set; }

        public string Label1 { get; set; }

        public string Label2 { get; set; }

        public double Percentage { get; set; }

        #endregion


        #region Constructor

        public WordMatch(
            string file1,
            string file2,
            string variable1, 
            string variable2, 
            string label1, 
            string label2, 
            double percentage
        )
        {
            this.File1 = file1;
            this.File2 = file2;
            this.Variable1 = variable1;
            this.Variable2 = variable2;
            this.Label1 = label1;
            this.Label2 = label2;
            this.Percentage = percentage;
        }

        #endregion


        #region Methods

        #endregion
    }
}
