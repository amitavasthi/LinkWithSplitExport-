using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtilities.Controls
{
    public class ProgressBar : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full url string to the
        /// service method that returns the progress.
        /// </summary>
        public string Service { get; set; }

        #endregion


        #region Constructor

        public ProgressBar()
            : base("div")
        {
            this.Load += ProgressBar_Load;
        }

        public ProgressBar(string service)
            : this()
        {
            this.Service = service;
        }

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        protected void ProgressBar_Load(object sender, EventArgs e)
        {
            base.CssClass = "ProgressContainer BorderColor1";

            Panel pnlProgress = new Panel();
            pnlProgress.ID = "pnlProgress" + this.ID;
            pnlProgress.CssClass = "ProgressBar BackgroundColor1";

            base.Controls.Add(pnlProgress);

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "InitProgressBar" + this.ID,
                "LoadProgress('" + pnlProgress.ClientID + "', '" + this.Service + "');",
                true
            );
        }

        #endregion
    }
}
