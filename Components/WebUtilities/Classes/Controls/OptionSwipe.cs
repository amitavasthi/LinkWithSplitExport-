using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtilities.Controls
{
    public class OptionSwipe : BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets a list of options
        /// of the option swipe control.
        /// </summary>
        public List<Option> Options { get; set; }

        /// <summary>
        /// Gets or sets in which direction 
        /// the option swiper should swipe.
        /// </summary>
        public OptionSwiperDirection Direction { get; set; }

        #endregion


        #region Constructor

        public OptionSwipe()
        {
            this.Load += OptionSwipe_Load;

            this.Options = new List<Option>();
        }

        #endregion


        #region Methods

        public void Render()
        {
            this.CssClass = "OptionSwiper OptionSwiper" + this.Direction + " " + this.CssClass;

            Panel pnlOptions = new Panel();
            pnlOptions.CssClass = "Options";

            // Run through all options of the option swipe.
            foreach (Option option in this.Options)
            {
                pnlOptions.Controls.Add(option);
            }

            base.Controls.Add(pnlOptions);

            Image imgSwiper = new Image();
            imgSwiper.ImageUrl = "/Images/Icons/Swiper.png";
            imgSwiper.CssClass = "Swiper BackgroundColor1";

            /*imgSwiper.Attributes.Add(
                "onmouseover",
                "overOptions = true;"
            );

            imgSwiper.Attributes.Add(
                "onmouseout",
                "overOptions = false;"
            );*/

            imgSwiper.Attributes.Add(
                "onclick",
                "ShowOptions(this.parentNode, '" + this.Direction + "');"
            );

            this.Attributes.Add(
                "onmouseout",
                "HideOptions(this, '" + this.Direction + "');"
            );

            base.Controls.Add(imgSwiper);
        }

        #endregion


        #region Event Handlers

        protected void OptionSwipe_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }

    public enum OptionSwiperDirection
    {
        Left,
        Bottom,
        Right
    }

    public class Option : BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the text of the option control.
        /// </summary>
        public string Text { get; set; }

        public string OnClientClick { get; set; }

        public event EventHandler OnClick;

        public string CommandArgument { get; set; }

        #endregion


        #region Constructor

        public Option()
        {
            this.OnClientClick = null;

            this.Load += Option_Load;
        }

        #endregion


        #region Methods

        public void Render()
        {
            this.CssClass = "Option " + this.CssClass;

            if (this.OnClick == null)
            {
                System.Web.UI.WebControls.Label lbl = new System.Web.UI.WebControls.Label();
                lbl.Text = this.Text;

                if (this.OnClientClick != null)
                {
                    base.Attributes.Add(
                        "onclick",
                        this.OnClientClick
                    );
                }

                base.Controls.Add(lbl);
            }
            else
            {
                System.Web.UI.WebControls.LinkButton lnk = new System.Web.UI.WebControls.LinkButton();
                lnk.Click += this.OnClick;
                lnk.Text = this.Text;
                lnk.CommandArgument = this.CommandArgument;

                base.Controls.Add(lnk);
            }

            this.Attributes.Add(
                "onmouseover",
                "overOptions = true;"
            );

            this.Attributes.Add(
                "onmouseout",
                "overOptions = false;"
            );
        }

        #endregion


        #region Event Handlers

        protected void Option_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }
}
