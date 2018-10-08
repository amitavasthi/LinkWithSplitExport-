using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Classes.Controls
{
    public partial class EquationDefinition : System.Web.UI.UserControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the report definition
        /// file where the equation definition is part of.
        /// </summary>
        public string Source { get; set; }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            boxEquationDefinitionCategorySearch.Visible = true;
            boxEquationInsertMethod.Visible = true;
            boxEquationDefinition.Visible = true;
            csEquationDefinition.Visible = true;

            csEquationDefinition.Source = this.Source;
        }

        #endregion
    }
}