using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using WebUtilities;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für LanguageManager
    /// </summary>
    public class LanguageManager : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            // Get the label key from the http request's parameters.
            string name = context.Request.Params["Name"];

            Language language;

            // Check if a language is defined in the parameters.
            if (context.Request.Params["Language"] != null)
            {
                // Get the language from the http request's parameters.
                language = (Language)Enum.Parse(
                    typeof(Language),
                    context.Request.Params["Language"]
                );
            }
            else
            {
                // Get the current's session selected language.
                language = (Language)HttpContext.Current.Session["Language"];
            }

            // Get the text for the language label key in the selected language.
            string text = Global.LanguageManager.GetText(language, name);

            // Set the content type of the http response to plain text.
            context.Response.ContentType = "text/plain";

            // Write the text of the language label to the http response.
            context.Response.Write(text);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}