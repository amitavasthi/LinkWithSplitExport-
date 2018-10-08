using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Azure_Switch.Classes
{
    public class SwitchHandler
    {
        #region Properties

        public static Dictionary<string, byte[]> Cache { get; set; }

        public HttpRequest Request { get; set; }

        public HttpResponse Response { get; set; }

        public string[] CachedFileTypes = new string[] {
            ".css",
            ".png",
            ".js",
            ".ttf"
        };

        #endregion


        #region Constructor

        public SwitchHandler()
        {
            this.Request = HttpContext.Current.Request;
            this.Response = HttpContext.Current.Response;
        }

        #endregion


        #region Methods

        public bool Switch(string server, string url, string parameters)
        {
            if (Cache == null)
                Cache = new Dictionary<string, byte[]>();

            Response.Clear();

            if (Request.HttpMethod == "GET" && parameters.Length != 0)
            {
                if (url.Contains("?"))
                    url += "&";
                else
                    url += "?";

                url += parameters;

                parameters = "";
            }

            bool cacheable = false;

            foreach (string cachedFileType in this.CachedFileTypes)
            {
                if (url.Contains(cachedFileType))
                {
                    cacheable = true;
                    break;
                }
            }

            if (cacheable)
            {
                // Build the full path to the cached file.
                string fileName = Path.Combine(
                    Request.PhysicalApplicationPath
                );

                Uri uri = new Uri(url);

                foreach (string segment in uri.Segments)
                {
                    fileName = Path.Combine(
                        fileName,
                        HttpUtility.UrlDecode(segment.Replace("/", ""))
                    );
                }

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                // Check if the file has been cached before.
                if (!File.Exists(fileName))
                {
                    // Cache the file.
                    File.WriteAllBytes(
                        fileName,
                        Download(server, url, Request.ContentEncoding.GetBytes(parameters))
                    );
                }
                Response.BinaryWrite(File.ReadAllBytes(fileName));
                /*if (!Cache.ContainsKey(fileName))
                {
                    Cache.Add(fileName, Download(url, Request.ContentEncoding.GetBytes(parameters)));
                }
                Response.BinaryWrite(Cache[fileName]);*/

                Response.ContentType = GetMimeType(new FileInfo(fileName).Name);
            }
            else
            {
                byte[] data = Download(server, url, Request.ContentEncoding.GetBytes(parameters));

                if (data == null)
                    return false;

                Response.BinaryWrite(data);
            }

            return true;

            //Response.End();
        }


        private byte[] Download(string server, string url, byte[] data)
        {
            //return new byte[0];

            StringBuilder requestHeader = new StringBuilder();

            if (data.Length > 0)
                requestHeader.Append("POST");
            else
                requestHeader.Append("GET");

            requestHeader.Append(" " + url.Replace(server, Request.Url.Host) + " HTTP/1.0");
            requestHeader.Append("\r\nAccept: application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, */*");
            requestHeader.Append("\r\nAccept-Language: en-US");
            requestHeader.Append("\r\nUser-Agent: Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)");
            requestHeader.Append("\r\nAccept-Encoding: gzip, deflate");

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                requestHeader.Append("\r\nCookie: ");

                if (Request.Cookies["_et_coid"] != null)
                {
                    requestHeader.Append("_et_coid=" + Request.Cookies["_et_coid"].Value + "; ");
                }
                requestHeader.Append("ASP.NET_SessionId=" + Request.Cookies["ASP.NET_SessionId"].Value);
            }

            requestHeader.Append("\r\nConnection: keep-alive");

            if (data.Length > 0)
            {
                requestHeader.Append("\r\nContent-Type: application/x-www-form-urlencoded");
                requestHeader.Append("\r\nContent-Length: " + data.Length + "");
                requestHeader.Append("\r\nReferer: " + url.Replace(server, Request.Url.Host) + "");
                requestHeader.Append("\r\nOrigin: http://switch.tokyo.local");
            }

            requestHeader.Append("\r\nHost: " + Request.Url.Host);
            requestHeader.Append("\r\n\r\n");

            if (data.Length != 0)
            {
                requestHeader.Append(System.Text.Encoding.UTF8.GetString(data));
                requestHeader.Append("\r\n");
            }

            byte[] r;

            try {
                SocketLevelWebClient wc = new SocketLevelWebClient();

                r = wc.SendWebRequest(server, requestHeader.ToString());
            }
            catch
            {
                return null;
            }

            if (r.Length == 0)
                return new byte[0];

            string contentStr = System.Text.Encoding.ASCII.GetString(r);

            // Check if the response is a redirect.
            if (contentStr.Split('\n')[0].Contains("302 Found"))
            {
                Response.Redirect(contentStr.Split(new string[] { "<a href=\"" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('\"')[0]);

                return new byte[0];
            }
            else if (contentStr.Split('\n')[0].Contains("503 Service Unavailable"))
            {
                return null;
            }

                contentStr = contentStr.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
            int headersLength = contentStr.Length;
            int contentLength = r.Length - headersLength - 4;

            Dictionary<string, string> headers = ParseHeaders(contentStr);

            if (contentLength == 0)
                return new byte[0];

            //int contentLength = int.Parse(contentStr.Split(new string[] { "Content-Length:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('\n')[0].Trim());
            string responseType = "text/html";

            if (headers.ContainsKey("Content-Type"))
                responseType = headers["Content-Type"];

            Response.ContentType = responseType;

            byte[] result = new byte[contentLength];

            Array.Copy(r, r.Length - contentLength, result, 0, contentLength);

            if (headers.ContainsKey("Content-Encoding") && headers["Content-Encoding"] == "gzip")
            {
                result = Decompress(result);
            }

            if(headers.ContainsKey("Content-Disposition"))
            {
                Response.Headers.Add("Content-Disposition", headers["Content-Disposition"]);
            }

            return result;
        }

        private Dictionary<string, string> ParseHeaders(string headerStr)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            string[] headers = headerStr.Split('\n');

            foreach (string header in headers)
            {
                if (!header.Contains(":"))
                    continue;

                string key = header.Split(':')[0].Trim();

                if (!result.ContainsKey(key))
                    result.Add(key, "");

                result[key] = header.Split(':')[1].Trim();
            }

            return result;
        }

        public byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        private byte[] Download2(string url, byte[] data)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            if (data.Length != 0)
                url = url.Split('?')[0];

            // Create a new http web request.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                Uri.EscapeUriString(url)
            );
            request.Host = Request.Url.Host;
            //request.Host = "alphayt.linkmr.com";

            CookieContainer cookieContainer = new CookieContainer();

            foreach (string cookie in Request.Cookies)
            {
                cookieContainer.Add(new Cookie(
                    cookie,
                    Request.Cookies[cookie].Value,
                    Request.Cookies[cookie].Path,
                    Request.Url.Host//"alphayt.linkmr.com"//ConfigurationManager.AppSettings["Domain"]
                ));
            }

            int length = data.Length;

            foreach (HttpPostedFile file in Request.Files)
            {
                length += file.ContentLength;
            }

            // Set the content type of the web request.
            request.ContentType = HttpContext.Current.Request.ContentType;
            request.ContentLength = data.Length;
            request.CookieContainer = cookieContainer;

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
            request.UserAgent = HttpContext.Current.Request.UserAgent;

            // Check if there is data to send to the server.
            if (length > 0)
            {
                // Get the request stream from the request.
                Stream requestStream = request.GetRequestStream();

                if (data.Length > 0)
                {
                    // Write the data to the request stream.
                    requestStream.Write(data, 0, data.Length);
                }

                foreach (HttpPostedFile file in Request.Files)
                {
                    // Write the file to the request stream.
                    while (true)
                    {
                        byte[] buffer = new byte[1];

                        if (file.InputStream.Read(buffer, 0, 1) == 0)
                            break;

                        requestStream.WriteByte(buffer[0]);
                    }
                }

                // Close the request stream.
                requestStream.Close();
            }

            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            try
            {
                // Get the response from the server.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Get the response stream from the response.
                Stream responseStream = response.GetResponseStream();
                byte[] result;

                if (response.ContentLength == -1)
                {
                    MemoryStream test = new MemoryStream();
                    //byte[] result = new byte[0];

                    while (true)
                    {
                        byte[] buffer = new byte[1];

                        if (responseStream.Read(buffer, 0, 1) == 0)
                            break;

                        test.WriteByte(buffer[0]);
                    }

                    result = test.ToArray();
                }
                else
                {
                    result = new byte[response.ContentLength];

                    using (BinaryReader br = new BinaryReader(responseStream))
                    {
                        result = br.ReadBytes((int)response.ContentLength);
                    }
                }

                if (response.Headers["Content-Disposition"] != null)
                {
                    Response.Headers.Add("Content-Disposition", response.Headers["Content-Disposition"]);
                }

                Response.ContentType = response.ContentType;
                //Response.RedirectLocation = Request.Url.ToString().Replace(Request.Url.PathAndQuery, response.ResponseUri.PathAndQuery);
                Response.RedirectLocation = response.ResponseUri.ToString();


                if (response.Cookies["ASP.NET_SessionId"] != null)
                    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", response.Cookies["ASP.NET_SessionId"].Value));

                // Dispose the request stream.
                //requestStream.Dispose();

                // Dispose the response stream.
                responseStream.Dispose();

                if (response.ResponseUri.ToString() != Request.Url.ToString() && Request.Url.ToString().Contains("?404;") == false)
                {
                    Response.Redirect(response.ResponseUri.ToString());
                }

                response.Close();

                return result;
            }
            catch (Exception ex)
            {
                return System.Text.Encoding.UTF8.GetBytes(ex.ToString() + "<br /><br />" + url + "<br /><br />" +
                    System.Text.Encoding.UTF8.GetString(data));
            }
        }

        /// <summary>
        /// Returns true for accepting all certifications.
        /// </summary>
        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification,
            System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private string GetMimeType(string fileName)
        {
            if (fileName.Contains(".css"))
            {
                return "text/css";
            }
            else if (fileName.Contains(".js"))
            {
                return "text/javascript";
            }
            else if (fileName.Contains(".png"))
            {
                return "image/png";
            }
            else if (fileName.Contains(".ttf"))
            {
                return "application/octet-stream";
            }

            return "text/plain";
        }

        #endregion
    }
}