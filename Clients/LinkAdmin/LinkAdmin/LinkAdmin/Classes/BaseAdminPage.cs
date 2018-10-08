using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace LinkAdmin.Classes
{
    public class BaseAdminPage : System.Web.UI.Page
    {
        #region Properties

        public bool RequiresAuthentication { get; set; }

        public Dictionary<string, PageMethod> Methods { get; set; }

        #endregion


        #region Constructor

        public BaseAdminPage(bool requiresAuthentication = true)
        {
            this.RequiresAuthentication = requiresAuthentication;
            this.Methods = new Dictionary<string, PageMethod>();

            this.Load += BaseAdminPage_Load;
        }

        #endregion


        #region Methods

        private void BaseAdminPage_Load(object sender, EventArgs e)
        {
            if (this.RequiresAuthentication && Global.IdUser.HasValue == false)
            {
                Response.Redirect("/Pages/Login.aspx");
                return;
            }

            if (Request.Params["Method"] == null)
                return;

            Response.Clear();

            string method = Request.Params["Method"];

            if (!this.Methods.ContainsKey(method))
                throw new NotImplementedException();

            this.Methods[method]();

            Response.End();
        }

        #endregion
    }
    public abstract class Page : System.Web.UI.UserControl
    {
        #region Properties

        #endregion


        #region Constructor

        public Page(bool requiresAuthentication = true)
        {
            this.Load += Page_Load;
        }

        #endregion


        #region Methods

        public virtual void Render()
        {

        }

        /// <summary>
        /// Returns the report control as html string.
        /// </summary>
        public string ToHtml()
        {
            string result = "";

            StringBuilder sb;
            StringWriter stWriter;
            HtmlTextWriter htmlWriter;

            sb = new StringBuilder();
            stWriter = new StringWriter(sb);
            htmlWriter = new HtmlTextWriter(stWriter);

            this.RenderControl(htmlWriter);
            /*this.RenderBeginTag(htmlWriter);

            foreach (Control control in this.Controls)
            {
                control.RenderControl(htmlWriter);
            }

            this.RenderEndTag(htmlWriter);*/

            result = sb.ToString();

            return result;
        }

        #endregion


        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            this.Render();
        }

        #endregion
    }

    public delegate void PageMethod();
}