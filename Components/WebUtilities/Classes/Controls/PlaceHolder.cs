using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUtilities.Controls
{
    [ToolboxData("<{0}:PlaceHolder runat=server></{0}:PlaceHolder>")]
    [ParseChildren(false)]
    public class PlaceHolder : BaseControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]


        #region Properties

        public List<WebControl> Controls { get; set; }

        #endregion


        #region Constructor

        public PlaceHolder()
        {
            this.Controls = new List<WebControl>();
        }

        #endregion


        #region Methods



        #endregion


        #region Event Handlers

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            foreach (WebControl control in this.Controls)
                control.RenderControl(writer);
        }

        #endregion
    }
}
