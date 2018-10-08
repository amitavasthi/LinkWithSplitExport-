using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch.Classes
{
    public class SwitchService
    {
        #region Properties

        public Dictionary<string, Method> Methods { get; set; }

        #endregion


        #region Constructor

        public SwitchService()
        {
            this.Methods = new Dictionary<string, Classes.Method>();

            this.Methods.Add("GetServers", GetServers);
            this.Methods.Add("SetServers", SetServers);
        }

        #endregion


        #region Methods

        public void GetServers(Request request)
        {
            Response response = new Response();

            response.Data = File.ReadAllBytes("Servers.xml");
            response.ContentType = "text/plain";

            response.Send(request.Handler);
        }

        public void SetServers(Request request)
        {
            string txt = System.Text.Encoding.UTF8.GetString(request.Data);

            int index = txt.IndexOf("\r\n\r\n") + 4;

            File.WriteAllBytes("Servers.xml", request.Data.ToList().
                GetRange(index, request.Data.Length - index).ToArray());

            Response response = new Response();

            response.Data = System.Text.Encoding.UTF8.GetBytes("True");
            response.ContentType = "text/plain";

            response.Send(request.Handler);
        }

        #endregion
    }

    public delegate void Method(Request request);
}
