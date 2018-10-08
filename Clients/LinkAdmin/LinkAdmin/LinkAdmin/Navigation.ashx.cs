using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace LinkAdmin
{
    /// <summary>
    /// Summary description for Navigation
    /// </summary>
    public class Navigation : IHttpHandler, IRequiresSessionState
    {
        #region Properties

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        public void ProcessRequest(HttpContext context)
        {
            if (!Global.IdUser.HasValue)
            {
                context.Response.Write("ERROR_SESSION");
                return;
            }

            System.Web.UI.Page pageHolder = new System.Web.UI.Page();
            System.Web.UI.UserControl viewControl = (System.Web.UI.UserControl)pageHolder.LoadControl(context.Request.Params["Page"]);

            pageHolder.Controls.Add(viewControl);
            StringWriter result = new StringWriter();
            HttpContext.Current.Server.Execute(pageHolder, result, false);
            context.Response.Write(result.ToString());
        }

        #endregion
    }
    public class FormlessPage : System.Web.UI.Page
    {
        public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
        {
        }
    }
}