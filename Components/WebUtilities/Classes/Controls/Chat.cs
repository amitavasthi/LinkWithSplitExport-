using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using WebUtilities.Controls;

namespace WebUtilities.Controls
{
    public class Chat : WebUtilities.BaseControl
    {
        #region Properties

        public string IdChat { get; set; }

        /// <summary>
        /// Gets or sets the full path to the xml
        /// file that contains the chat log.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the available height of the chat control.
        /// </summary>
        public int ControlHeight { get; set; }

        #endregion


        #region Constructor

        public Chat(string fileName)
        {
            this.FileName = fileName;

            this.Load += Chat_Load;
        }

        #endregion


        #region Methods

        public void Render()
        {
            if (string.IsNullOrEmpty(this.IdChat))
                this.IdChat = "Global";

            this.CssClass = "ChatContainer";

            Panel pnlChat = new Panel();
            pnlChat.ID = this.ID + "_pnlChat";
            pnlChat.CssClass = "Chat";

            TextBox txtNewChat = new TextBox();
            txtNewChat.Attributes.Add("onkeypress", "if(event.keyCode != 13) return true; event.preventDefault(); SendChatMessage(this, document.getElementById(this.parentNode.id + '_pnlChat')); return false;");

            base.Controls.Add(pnlChat);
            base.Controls.Add(txtNewChat);
            base.Controls.Add(new LiteralControl(string.Format(
                "<script type=\"text/javascript\">InitChat('{0}');</script>",
                pnlChat.ClientID
            )));
        }

        #endregion


        #region Event Handlers

        protected void Chat_Load(object sender, EventArgs e)
        {
            this.Render();
        }

        #endregion
    }
}
