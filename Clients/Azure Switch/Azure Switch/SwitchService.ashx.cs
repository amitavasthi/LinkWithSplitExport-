using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Azure_Switch
{
    public class SwitchService : IHttpHandler
    {
        #region Properties

        /// <summary>
        /// Gets or sets the available methods of the generic handler.
        /// </summary>
        public Dictionary<string, WebMethod> Methods { get; set; }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion


        #region Constructor

        public SwitchService()
        {
            this.Methods = new Dictionary<string, WebMethod>();

            this.Methods.Add("GetServers", GetServers);
            this.Methods.Add("SetServers", SetServers);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(HttpContext context)
        {
            // Set the content type of the http response to plain text.
            context.Response.ContentType = "text/plain";

            /*if (this.RequiresWhitelist)
            {
                Whitelist whitelist = new Whitelist();

                if (!whitelist.Items.ContainsKey(context.Request.ServerVariables["REMOTE_HOST"]))
                    throw new Exception("Access denied.");
            }*/

            // Get the requested method name from the http request.
            string method = context.Request.Params["Method"];

            // Check if the requested method exists.
            if (!this.Methods.ContainsKey(method))
                throw new NotImplementedException();

            // Invoke the requested method.
            this.Methods[method].Invoke(context);
        }

        #endregion


        #region Web Methods

        private void GetServers(HttpContext context)
        {
            context.Response.Write(File.ReadAllText(Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            )));
        }

        private void SetServers(HttpContext context)
        {
            string fileName = System.IO.Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            );

            MemoryStream ms = new MemoryStream();
            context.Request.InputStream.CopyTo(ms);

            File.WriteAllBytes(fileName, ms.ToArray());
        }

        #endregion
    }

    public delegate void WebMethod(HttpContext context);
}