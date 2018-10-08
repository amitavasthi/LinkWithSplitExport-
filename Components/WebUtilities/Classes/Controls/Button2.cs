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
    [ParseChildren(false)]
    [DefaultProperty("Name")]
    [ToolboxData("<{0}:Button2 runat=server></{0}:Button2>")]
    public class Button2 : System.Web.UI.WebControls.Panel
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]

        #region Properties

        public string Name { get; set; }

        public string Method { get; set; }

        public string PostFields { get; set; }

        #endregion


        #region Constructor

        public Button2()
        {
            this.Load += Button2_Load;
        }

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        private void Button2_Load(object sender, EventArgs e)
        {
            this.Controls.Add(new LiteralControl(string.Format(
                "<div id=\"_{3}\" class=\"Button\" onclick=\"Button2_Click(this, '{1}', [{2}]);\">{0}</div>",
                ((LanguageManager)HttpContext.Current.Session["LanguageManager"]).GetText(this.Name),
                this.Method,
                string.Join(",", this.PostFields.Split(',').Select(x => "'" + x.Trim() + "'")),
                this.ID
            )));
        }

        #endregion
    }
}
