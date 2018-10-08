using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace WebUtilities.Controls
{
    [DefaultProperty("Name")]
    [ToolboxData("<{0}:Label runat=server></{0}:Label>")]
    /// <summary>
    /// Language label. The 'LanguageManager' and 'Language' 
    /// property must been set in the current http session.
    /// </summary>
    public class Label : System.Web.UI.WebControls.Label
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]

        #region Properties

        public string Text { private get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tooltip
        /// is shown when mouse over.
        /// </summary>
        public string ToolTip { get; set; }

        public bool ToolTipMustOverflow { get; set; }

        #endregion


        #region Constructor

        public Label()
        {
            this.Load += new EventHandler(Label_Load);
            this.PreRender += Label_PreRender;
        }

        #endregion


        #region Methods

        public void Render()
        {
            if (this.ID == null)
            {
                this.ID = "LanguageLabel_" + this.Name + "_" + Guid.NewGuid();
            }

            string text = ((LanguageManager)HttpContext.Current.Session["LanguageManager"]).
                GetText(((Language)HttpContext.Current.Session["Language"]), this.Name);

            base.Text = text;
        }

        #endregion


        #region Event Handlers

        protected void Label_Load(object sender, EventArgs e)
        {
            Render();
        }

        protected void Label_PreRender(object sender, EventArgs e)
        {
            // Check if a tool tip is set.
            if (this.ToolTip != null && this.ToolTip != "")
            {
                // Check if element must overflow for showing tool tip.
                if (this.ToolTipMustOverflow)
                {
                    // Add the javascript onmouseover event for showing the tool tip.
                    this.Attributes.Add("onmouseover", "if(isOverflowed(this)) { ShowToolTip(this, \"" + this.ToolTip + "\"); }");
                }
                else
                {
                    // Add the javascript onmouseover event for showing the tool tip.
                    this.Attributes.Add("onmouseover", "ShowToolTip(this, \"" + this.ToolTip + "\");");
                }
            }
        }

        #endregion
    }
}
