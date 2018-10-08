using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkAdmin.Handlers
{
    /// <summary>
    /// Summary description for LanguageManager
    /// </summary>
    public class LanguageManager : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public LanguageManager()
        {
            base.Methods.Add("GetLabel", GetLabel);
        }

        #endregion


        #region Web Methods

        private void GetLabel(HttpContext context)
        {
            context.Response.Write(Global.LanguageManager.GetText(
                context.Request.Params["Key"]
            ));
        }

        #endregion
    }
}