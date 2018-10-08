using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace ApplicationUtilities
{
    public class ServiceLink
    {
        #region Properties

        /// <summary>
        /// Gets or sets the web address to
        /// the link admin service to call.
        /// </summary>
        public string Address { get; set; }

        public string HostHeader { get; set; }

        #endregion


        #region Constructor

        public ServiceLink(string address)
        {
            this.Address = address;

            this.HostHeader = new Uri(this.Address).Host;
        }

        #endregion


        #region Methods

        public string Request(string[] parameters, byte[] data = null, Dictionary<string, string> cookies = null)
        {
            if (data == null)
                data = new byte[0];

            return Request(this.Address + "?" + string.Join("&", parameters), data, cookies);
        }

        public string Request(string[] parameters, Dictionary<string, string> cookies)
        {
            return Request(this.Address + "?" + string.Join("&", parameters), new byte[0], cookies);
        }

        public string Request(string url, byte[] data, Dictionary<string, string> cookies = null)
        {
            if (cookies == null)
                cookies = new Dictionary<string, string>();

            // Create a new http web request.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                Uri.EscapeUriString(url)
            );

            request.Host = this.HostHeader;

            // Set the content type of the web request.
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
                request.ContentType = HttpContext.Current.Request.ContentType;
            request.ContentLength = data.Length;

            // Check if there is data to send to the server.
            if (data.Length == 0)
            {
                // Set the request method to GET.
                request.Method = "GET";
            }
            else
            {
                // Set the request method to POST.
                request.Method = "POST";
            }

            // Set the user agent of the request.
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
                request.UserAgent = HttpContext.Current.Request.UserAgent;

            request.CookieContainer = new CookieContainer();

            foreach (string key in cookies.Keys)
            {
                Cookie cookie = new Cookie(key, cookies[key]);
                cookie.Path = "/";
                cookie.Domain = request.Host;

                request.CookieContainer.Add(cookie);
            }

            // Check if there is data to send to the server.
            if (data.Length > 0)
            {
                // Get the request stream from the request.
                Stream requestStream = request.GetRequestStream();

                // Write the data to the request stream.
                requestStream.Write(data, 0, data.Length);

                // Close the request stream.
                requestStream.Close();
            }

            //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            request.Timeout = 1200000;

            // Get the response from the server.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the response stream from the response.
            Stream responseStream = response.GetResponseStream();

            // Create a new stream reader by the response.
            StreamReader streamReader = new StreamReader(
                responseStream,
                System.Text.Encoding.UTF8
            );

            // Read the contents of the response's stream reader.
            string result = streamReader.ReadToEnd();

            // Dispose the response stream.
            responseStream.Dispose();

            // Dispose the stream reader.
            streamReader.Dispose();

            return result;
        }

        #endregion
    }
}