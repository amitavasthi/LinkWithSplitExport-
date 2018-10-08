using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace WebUtilities.Controls
{
    [ParseChildren(false)]
    public class HoverBox : BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the control that triggers the hover box.
        /// </summary>
        public string IdTrigger { get; set; }

        /// <summary>
        /// Gets or sets the id of the parent hover box, if exists, for locking it.
        /// </summary>
        public string IdParent { get; set; }

        /// <summary>
        /// Gets or sets the color scheme of the hover box.
        /// </summary>
        public string ColorScheme { get; set; }

        /// <summary>
        /// Gets or sets the display type of the hover box.
        /// </summary>
        public HoverBoxDisplay Display { get; set; }

        /// <summary>
        /// Gets or sets the animation type of the hover box.
        /// </summary>
        public HoverBoxAnimation Animation { get; set; }

        /// <summary>
        /// Gets or sets the trigger mode of the hover box.
        /// </summary>
        public HoverBoxTriggerMode TriggerMode { get; set; }

        /// <summary>
        /// Indicates if the trigger's image should be set to active.
        /// </summary>
        public bool ActivateTriggerImage { get; set; }

        /// <summary>
        /// Gets or sets the display level of the hover box.
        /// </summary>
        public int Level { get; set; }

        public bool AsynchRender { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the hover box.
        /// </summary>
        public HoverBox()
        {
            this.Load += HoverBox_Load;

            base.Style.Add("opacity", "0.0");
        }

        #endregion


        #region Methods

        public void Render()
        {
            this.AsynchRender = true;

            HoverBox_Load(this, new EventArgs());
        }


        private string GetButtonId(string idButton)
        {
            string result = this.ClientID;

            result = result.Replace(this.ID, idButton);

            return result;
        }

        #endregion


        #region Event Handlers

        protected void HoverBox_Load(object sender, EventArgs e)
        {
            base.CssClass = "HoverBox " + base.CssClass;
            string eventName = "";

            switch (this.TriggerMode)
            {
                case HoverBoxTriggerMode.Hover:
                    eventName = "onmouseover";
                    break;
                case HoverBoxTriggerMode.Click:
                    eventName = "onclick";
                    break;
            }

            if (this.IdTrigger == null)
                return;

            HtmlGenericControl script = new HtmlGenericControl("script");
            script.Attributes.Add("type", "text/javascript");

            string s = string.Format(
                " var trigger = document.getElementById(\"{0}\"); "+
                "trigger.setAttribute(\"{1}\",\"ShowHoverBox('{2}', this.id, '{3}','{1}', {4}, {5}, {6}, '{7}');\" + trigger.getAttribute(\"{1}\"));",
                    this.GetButtonId(this.IdTrigger),
                    eventName,
                    this.ClientID,
                    this.Display,
                    this.ActivateTriggerImage.ToString().ToLower(),
                    this.Level,
                    this.IdParent != null ? "'" + this.GetButtonId(this.IdParent) + "'" : "undefined",
                    this.Animation
            );

            if (!this.AsynchRender)
            {
                script.InnerHtml = "loadFunctions.push(function() { " + s + " });";
            }
            else
            {
                script.InnerHtml = s;
            }
            /*script.InnerHtml = string.Format(
                "loadFunctions.push(function() { document.getElementById(\"{0}\").setAttribute(\"{1}\",\""+
                "ShowHoverBox('{2}', this.id, '{3}', '{1}', {4}, {5});\"); } );",
                this.GetButtonId(),
                eventName,
                this.ClientID,
                this.ActivateTriggerImage.ToString().ToLower(),
                this.Level
            );*/

            this.Controls.Add(script);
        }

        #endregion
    }

    public enum HoverBoxDisplay
    {
        RightTop,
        RightMiddle,
        RightBottom,
        TopLeft,
        LeftTop,
        Workflow,
        RightPanel,
        LeftPanel,
        LeftPanelSection,
        SubNavigation,
        BottomCenter
    }

    public enum HoverBoxTriggerMode
    {
        Hover,
        Click
    }

    public enum HoverBoxAnimation
    {
        Opacity,
        Slide,
        SlideH,
        SlideWithTrigger
    }
}
