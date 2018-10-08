using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;

namespace Switch.Classes
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, Request> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, Request> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, Request> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            HttpListenerContext ctx = c as HttpListenerContext;
                            try
                            {
                                //string rstr = _responderMethod(ctx.Request);
                                //byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                Request request = _responderMethod(ctx.Request);
                                ctx.Response.ContentLength64 = request.Data.Length;

                                /*if (request.Values.ContainsKey("Content-Type"))
                                    ctx.Response.ContentType = request.Values["Content-Type"];*/

                                foreach (string key in request.Values.Keys)
                                {
                                    ctx.Response.AddHeader(key, request.Values[key]);
                                }

                                ctx.Response.OutputStream.Write(request.Data, 0, request.Data.Length);

                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}