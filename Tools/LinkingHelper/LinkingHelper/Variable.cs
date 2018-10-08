using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkingHelper
{
    public class Variable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the variable's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the variable's label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets if the variable was matched.
        /// </summary>
        public bool Matched { get; set; }

        #endregion


        #region Constructor

        public Variable(string name, string label)
        {
            this.Name = name;
            this.Label = label;
        }

        #endregion


        #region Methods

        #endregion
    }
}
