using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtilities.Controls
{
    public class TipGallery : WebUtilities.BaseControl
    {
        #region Properties

        public string _TipItems { get; set; }

        /// <summary>
        /// Gets or sets an array of language
        /// keys which are displayed as tips.
        /// </summary>
        public string[] TipItems { get; set; }

        /// <summary>
        /// Gets or sets the interval of
        /// switching between the tips.
        /// </summary>
        public int Interval { get; set; }

        #endregion


        #region Constructor

        public TipGallery()
        {
            this.TipItems = new string[0];
            this.Load += TipGallery_Load;
        }

        public TipGallery(params string[] items)
            : this()
        {
            this.TipItems = items;
        }

        #endregion


        #region Methods

        public void Render()
        {
            // Set the css class of the tip gallery.
            this.CssClass = "BoxTip " + this.CssClass;

            // Create a new label to display the tips.
            System.Web.UI.WebControls.Label lbl = new System.Web.UI.WebControls.Label();

            // Check if there are tip labels defined.
            if (this.TipItems.Length > 0)
            {
                // Set the text of the first tip as default.
                lbl.Text = base.LanguageManager.GetText(this.TipItems[0].Trim());
            }

            base.Controls.Add(lbl);

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "InitTipGallery" + this.ID,
                string.Format(
                    "window.setTimeout(function() {3} TipGalleryNext('{0}', '{1}', [{2}], 0); {4}, {1});", 
                    this.ClientID, 
                    this.Interval,
                    string.Join(",", this.TipItems.Select(x=>"'" + x.Trim() + "'")),
                    "{",
                    "}"
                ),
                true
            );
        }

        #endregion


        #region Event Handlers

        protected void TipGallery_Load(object sender, EventArgs e)
        {
            if (this.TipItems.Length == 0 && this._TipItems != null)
            {
                this.TipItems = this._TipItems.Split(',');
            }

            if (this.Interval == 0)
                this.Interval = 5000;

            Render();
        }

        #endregion
    }
}
