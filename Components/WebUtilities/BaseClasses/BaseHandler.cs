using ApplicationUtilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace WebUtilities
{
    public abstract class BaseHandler : IHttpHandler, IRequiresSessionState
    {
        #region Properties

        public event EventHandler OnSessionError;

        /// <summary>
        /// Gets the id of the authenticated of the current session.
        /// </summary>
        public Guid? IdUser
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null)
                    return null;

                return (Guid)HttpContext.Current.Session["User"];
            }
        }

        /// <summary>
        /// Gets the database core of the web application's session.
        /// </summary>
        public DatabaseCore.Core Core
        {
            get
            {
                return (DatabaseCore.Core)HttpContext.Current.Session["Core"];
            }
        }

        /// <summary>
        /// Gets or sets the available methods of the generic handler.
        /// </summary>
        public Dictionary<string, WebMethod> Methods { get; set; }

        /// <summary>
        /// Gets or sets if the generic handler
        /// requires an authenticated session.
        /// </summary>
        public bool RequiresAuthentication { get; set; }

        /// <summary>
        /// Gets or sets if the generic handler
        /// requires an IP on the whitelist.
        /// </summary>
        public bool RequiresWhitelist { get; set; }

        /// <summary>
        /// Gets if the generic handler is re useable.
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion


        #region Constructor

        public BaseHandler(bool requiresAuthentication = true, bool requiresWhitelist = false)
        {
            this.RequiresAuthentication = requiresAuthentication;
            this.RequiresWhitelist = requiresWhitelist;
            this.Methods = new Dictionary<string, WebMethod>();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(HttpContext context)
        {
            PreProcessRequest(context);

            // Set the content type of the http response to plain text.
            context.Response.ContentType = "text/plain";

            // Check if the generic handler requires an authenticated session.
            if (this.RequiresAuthentication)
            {
                // Check if the current session has an authenticated user.
                if (HttpContext.Current.Session["User"] == null)
                {
                    if (this.OnSessionError != null)
                        this.OnSessionError(context, new EventArgs());
                    else
                        throw new Exception("Not authenticated.");

                    return;
                }
            }

            if (this.RequiresWhitelist)
            {
                Whitelist whitelist = new Whitelist();

                if(!whitelist.Valid(context.Request.ServerVariables["REMOTE_HOST"]))
                    throw new Exception("Access denied [" + context.Request.ServerVariables["REMOTE_HOST"] + "].");
            }

            // Get the requested method name from the http request.
            string method = context.Request.Params["Method"];

            // Check if the requested method exists.
            if (!this.Methods.ContainsKey(method))
                throw new NotImplementedException();

            // Invoke the requested method.
            this.Methods[method].Invoke(context);
        }

        public virtual void PreProcessRequest(HttpContext context)
        {

        }

        public string ToJson(string[] names, object[] values)
        {
            StringBuilder jsonBuilder = new StringBuilder();

            jsonBuilder.Append("{");

            for (int i = 0; i < names.Length; i++)
            {
                string value = "";

                if (values[i] == null)
                    value = "";
                else
                    value = values[i].ToString();

                if (value.StartsWith("[") || value.StartsWith("{"))
                {
                    value = values[i].ToString();
                }
                else
                {
                    value = "\"" + HttpUtility.HtmlEncode(values[i]) + "\"";
                }

                jsonBuilder.Append(string.Format(
                    "\"{0}\": {1},",
                    names[i],
                    value
                ));
            }

            string result = jsonBuilder.ToString();

            if (result.Length > 0)
                result = result.Remove(result.Length - 1, 1);

            result += "}";

            return result;
        }


        /// <summary>
        /// Writes a file to the response of a request.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file that is to be written.
        /// </param>
        /// <param name="displayName">
        /// The name that is to be displayed for the file.
        /// </param>
        public void WriteFileToResponse(string fileName, string displayName, string mimeType, bool deleteFile)
        {
            // Local variables.
            HttpContext context = null;


            // Replace the spaces in the 
            // file name with underscores.
            displayName = displayName.Replace(" ", "_");


            // Read the context.
            context = HttpContext.Current;

            // Configure the response and transfer the file.
            context.Response.Buffer = true;
            context.Response.Clear();
            context.Response.AppendHeader("content-disposition",
                string.Format(CultureInfo.InvariantCulture,
                "attachment; filename={0}", displayName));
            context.Response.ContentType = mimeType;

            byte[] buffer = File.ReadAllBytes(fileName);

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);

            if (deleteFile)
                File.Delete(fileName);

            context.Response.RedirectLocation = context.Request.Url.ToString();

            context.Response.End();
        }

        /// <summary>
        /// Writes a file to the response of a request.
        /// </summary>
        /// <param name="data">
        /// The data to transmit as byte array.
        /// </param>
        /// <param name="displayName">
        /// The name that is to be displayed for the file.
        /// </param>
        public void WriteFileToResponse(byte[] data, string displayName, string mimeType)
        {
            // Local variables.
            HttpContext context = null;


            // Replace the spaces in the 
            // file name with underscores.
            displayName = displayName.Replace(" ", "_");


            // Read the context.
            context = HttpContext.Current;

            // Configure the response and transfer the file.
            context.Response.Buffer = true;
            context.Response.Clear();
            context.Response.AppendHeader("content-disposition",
                string.Format(CultureInfo.InvariantCulture,
                "attachment; filename={0}", displayName));
            context.Response.ContentType = mimeType;
            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.End();
        }

        #endregion
    }

    public delegate void WebMethod(HttpContext context);
}
