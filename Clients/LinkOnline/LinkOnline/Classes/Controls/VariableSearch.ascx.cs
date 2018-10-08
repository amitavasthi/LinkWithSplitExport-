using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Classes.Controls
{
    public partial class VariableSearch : System.Web.UI.UserControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to
        /// the current report definition.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the selected metadata language.
        /// </summary>
        public int IdLanguage { get; set; }

        /// <summary>
        /// Gets or sets the client id of the
        /// data check settings checkbox.
        /// </summary>
        public string DataCheckClientId { get; set; }

        #endregion


        #region Constructor

        public VariableSearch()
        {

        }

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            boxVariableSearch.Visible = true;

            txtVariableSearch.Attributes.Add("onkeyup", string.Format(
                "SearchVariables('{0}', '{1}')", //, document.getElementById('{2}').checked;
                this.Source,
                this.IdLanguage,
                this.DataCheckClientId
            ));
        }

        #endregion
    }
}