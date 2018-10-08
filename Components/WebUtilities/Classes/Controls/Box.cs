using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace WebUtilities.Controls
{
    [ParseChildren(false)]
    public class Box : BaseControl
    {
        #region Properties

        public string Title { get; set; }

        public bool TitleLanguageLabel { get; set; }

        public bool Dragable { get; set; }

        public bool Valid { get; set; }

        /// <summary>
        /// Gets or sets if the close button should do a post back.
        /// </summary>
        public bool ClosePostBack { get; set; }

        public string OnClientClose { get; set; }

        public bool JavascriptTriggered { get; set; }

        public bool AsynchRender { get; set; }

        public BoxPosition Position { get; set; }

        #endregion


        #region Constructor

        public Box()
            : base("div")
        {
            this.Load += Box_Load;
            this.TitleLanguageLabel = true;

            if (!this.JavascriptTriggered)
                this.Visible = false;
            else
                this.Visible = true;
        }

        #endregion


        #region Methods

        public void Validate()
        {
            this.Valid = true;

            List<TextBox> textboxes = GetTextboxes(this);

            foreach (TextBox textbox in textboxes)
            {
                if (textbox.Required && textbox.Text.Trim() == "")
                {
                    textbox.BorderStyle = System.Web.UI.WebControls.BorderStyle.Solid;
                    textbox.BorderWidth = 1;
                    textbox.BorderColor = Color.Red;

                    this.Valid = false;
                }
            }

            if (!this.Valid)
            {
                ShowError("Error1");
            }
        }

        private List<TextBox> GetTextboxes(Control parent)
        {
            List<TextBox> result = new List<TextBox>();

            foreach (Control control in parent.Controls)
            {
                if (control.GetType().Name == "TextBox")
                {
                    result.Add((TextBox)control);
                }
                else
                {
                    result.AddRange(GetTextboxes(control));
                }
            }

            return result;
        }

        public void ShowError(string name)
        {
            Label lblError = new Label();
            lblError.Name = name;

            lblError.CssClass = "Error";

            this.Controls.AddAt(0, lblError);

            this.Visible = true;
        }

        public void ShowError2(string text)
        {
            System.Web.UI.WebControls.Label lblError =
                new System.Web.UI.WebControls.Label();
            lblError.Text = text;

            lblError.CssClass = "Error";

            this.Controls.AddAt(0, lblError);

            this.Visible = true;
        }


        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.Write("<div id='" + this.ID + "' class='" + this.CssClass + "'");

            if (this.JavascriptTriggered)
                writer.Write(" style='display:none' ");

            writer.Write(">");

            string title = this.Title;

            if (this.TitleLanguageLabel)
                title = base.LanguageManager.GetText(title);

            writer.Write("<div id='" + this.ID + "Control' class=\"Box Color1\">");

            writer.Write("<div class=\"BoxTitle\">" + title + "");

            if (this.Dragable)
            {
                string clickAction = "";

                if (this.ClosePostBack)
                {
                    //clickAction = "__doPostBack('','');";
                    clickAction = "CloseBox('" + this.ID + "Control', '"+ this.Position +"');ShowLoading(document.body);window.location = window.location;" + this.OnClientClose;
                }
                else
                {
                    clickAction = "CloseBox('" + this.ID + "Control', '" + this.Position + "');" + this.OnClientClose;
                }
                
                    // Add the javascript controled close button to the drag bar.
                    writer.Write("<img src=\"/Images/Icons/BoxClose.png\" class=\"BtnBoxClose\" style=\"float:right\" " +
                        "onmouseover=\"this.src='/Images/Icons/BoxClose_Hover.png';\" onmouseout=\"this.src = '/Images/Icons/BoxClose.png'\" " +
                        "onclick=\"" + clickAction + "\" />");
            }

            writer.Write("</div>");

            writer.Write("<div class=\"BoxContent\">");
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            writer.Write("</div>");
            writer.Write("</div>");
            writer.Write("</div>");

            if (this.Dragable)
            {
                string script = "";

                if (!this.JavascriptTriggered)
                {
                    if (this.AsynchRender)
                    {
                        //script = "window.setTimeout(function() { InitDragBox('" + this.ID + "Control'); }, 500);";
                        script = "InitDragBox('" + this.ID + "Control', '"+ this.Position+"');";
                    }
                    else
                    {
                        script = "loadFunctions.push(function() { InitDragBox('" + this.ID + "Control', '" + this.Position + "'); });";
                    }
                }

                writer.Write(
                    "<script type=\"text/javascript\">" +
                        script +
                    "</script>"
                );
            }
        }

        #endregion


        #region Event Handlers

        protected void Box_Load(object sender, EventArgs e)
        {
        }

        #endregion
    }

    public enum BoxPosition
    {
        Bottom,
        Top,
        Center
    }
}
