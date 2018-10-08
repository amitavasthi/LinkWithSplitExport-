using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace WebUtilities.Controls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ConfirmBox runat=server></{0}:ConfirmBox>")]
    [ParseChildren(false)]
    public class ConfirmBox : Box
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]

        #region Properties

        public ConfirmMethod Confirm
        {
            get
            {
                return (ConfirmMethod)HttpContext.Current.Session["ConfirmMethod"];
            }
            set
            {
                HttpContext.Current.Session["ConfirmMethod"] = value;
            }
        }

        public string Text
        {
            get
            {
                return this.Label.Text;
            }
            set
            {
                this.Label.Text = value;
            }
        }

        private System.Web.UI.WebControls.Label Label { get; set; }

        #endregion


        #region Constructor

        public ConfirmBox()
        {
            this.Visible = false;
            this.Dragable = true;

            this.Label = new System.Web.UI.WebControls.Label();

            this.Load += new EventHandler(ConfirmBox_Load);
        }

        #endregion


        #region Methods

        public delegate void ConfirmMethod();

        #endregion


        #region Event Handlers

        protected void ConfirmBox_Load(object sender, EventArgs e)
        {
            Button buttonYes = new Button();
            Button buttonNo = new Button();

            buttonYes.ID = this.ID + "_btnWarningBoxConfirm";
            buttonNo.ID = this.ID + "_btnWarningBoxCancel";

            buttonYes.Style.Add("float", "right");
            buttonNo.Style.Add("float", "right");

            buttonYes.Name = "Yes";
            buttonNo.Name = "No";

            buttonYes.Style.Add("margin-right", "0.5em");

            buttonYes.Click += new EventHandler(buttonYes_Click);
            buttonNo.Click += buttonNo_Click;

            this.Label.Text = this.Text;

            HtmlGenericControl pnlSpacer = new HtmlGenericControl("div");
            pnlSpacer.InnerHtml = "";

            this.Controls.Add(this.Label);
            this.Controls.Add(pnlSpacer);
            this.Controls.Add(buttonNo);
            this.Controls.Add(buttonYes);

            this.Controls.Add(new LiteralControl("<br /><br />"));
        }

        protected void buttonNo_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        protected void buttonYes_Click(object sender, EventArgs e)
        {
            if (this.Confirm != null)
                this.Confirm();

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "SelfRedirect",
                "__doPostBack('','');",
                true
            );
        }

        #endregion
    }
}
