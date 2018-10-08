using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace WebUtilities.Controls
{
    [DefaultProperty("Name")]
    [ToolboxData("<{0}:TextBox runat=server></{0}:TextBox>")]
    /// <summary>
    /// TextBox containing additional functions, like default button.
    /// </summary>
    public class TextBox : System.Web.UI.WebControls.TextBox
    {
        #region Properties

        /// <summary>
        /// The id of the default button which will
        /// be invoked by pressing the enter key.
        /// </summary>
        public string Button { get; set; }

        /// <summary>
        /// Indicates if the textbox have to be filled.
        /// </summary>
        public bool Required { get; set; }

        #endregion


        #region Constructor

        public TextBox()
        {
            this.PreRender += TextBox_PreRender;
            this.Required = false;
        }

        #endregion


        #region Methods

        private string GetButtonId()
        {
            string result = this.ClientID;

            result = result.Replace(this.ID, this.Button);

            return result;
        }

        #endregion


        #region Event Handlers

        protected void TextBox_PreRender(object sender, EventArgs e)
        {
            this.Attributes.Add(
                "onkeypress",
                "if(event.keyCode == 13) " +
                "{" +
                //"__doPostBack('" + GetButtonId() + "', '');"+
                //"return false; "+
                    "event.preventDefault();document.getElementById('" + GetButtonId() + "').click();return false;" +
                "}"
            );
        }

        #endregion
    }
}
