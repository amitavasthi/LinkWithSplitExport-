using ApplicationUtilities;
using ApplicationUtilities.Cluster;
using Switch.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Switch
{
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class Program
    {
        #region Properties

        public static string PreferedServer { get; set; }

        public static string PhysicalApplicationPath { get; set; }

        public static ServerCollection Servers { get; set; }

        public static Dictionary<string, string> Sessions { get; set; }

        public static Listener Listener { get; set; }

        public static Dictionary<string, byte[]> Cache = new Dictionary<string, byte[]>();

        static System.Timers.Timer timer;

        #endregion


        #region Methods

        public static Request SendResponse(HttpListenerRequest request)
        {
            Request response;
            TcpClient serverConnection = new TcpClient("127.0.0.1", 80);

            string req = string.Format(
                "{0} {3} HTTP/1.0" + Environment.NewLine,
                request.HttpMethod,
                request.Url.Scheme,
                "127.0.0.1",
                request.RawUrl,
                request.ProtocolVersion
            );


            byte[] data = CheckCache2(request);

            if (data.Length != 0)
            {
                response = new Switch.Request(data, true);

                //int contentLength = ParseContentLength2(System.Text.Encoding.UTF8.GetString(response.Data));

                response.Data = response.Data.ToList().GetRange(
                    response.Data.Length - response.ContentLength,
                    response.ContentLength
                ).ToArray();

                return response;
            }

            req += request.Headers.ToString();

            data = System.Text.Encoding.UTF8.GetBytes(req);

            NetworkStream writer = serverConnection.GetStream();

            writer.Write(data, 0, data.Length);
            request.InputStream.CopyTo(writer);
            writer.Flush();

            data = ReadFromStream(writer);

            response = new Switch.Request(data, true);

            //int contentLength = ParseContentLength2(System.Text.Encoding.UTF8.GetString(response.Data));

            response.Data = response.Data.ToList().GetRange(
                response.Data.Length - response.ContentLength,
                response.ContentLength
            ).ToArray();

            return response;

            //return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
        }

        public static int Main(String[] args)
        {
            PhysicalApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            /*Servers = new ServerCollection("Servers.xml");
            Sessions = new Dictionary<string, string>();
            Sessions.Add("", "127.0.0.1");

            WebServer ws = new WebServer(SendResponse, "http://127.0.0.1:90/");
            ws.Run();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            ws.Stop();
            return 1;*/

            //Servers = new ServerCollection("Servers.xml");
            PreferedServer = ConfigurationManager.AppSettings["PreferedServer"];
            Servers = new ServerCollection();
            Sessions = new Dictionary<string, string>();

            timer = new System.Timers.Timer(60000);

            timer.Elapsed += Timer_Elapsed;

            timer.Start();

            Listener = new Listener();
            Listener.OnRequest += Listener_OnRequest;
            Listener.Start();

            Console.ReadLine();

            return 0;
        }

        private static bool CheckCache(Request request)
        {
            string cachePath = "";

            try
            {
                cachePath = Path.Combine(
                    PhysicalApplicationPath,
                    "Cache",
                    request.Path.Split('?')[0].Replace("/", "\\")
                );
            }
            catch { }

            string[] extensions = new string[]
            {
                ".png",
                ".css",
                ".ttf",
                ".woff",
                ".woff2",
                ".js"
            };

            bool cacheable = false;

            foreach (string extension in extensions)
            {
                if (cachePath.EndsWith(extension))
                {
                    cacheable = true;
                    break;
                }
            }

            if (!cacheable)
                return false;

            if (Cache.ContainsKey(cachePath))
            {
                try
                {
                    request.Handler.BeginSend(Cache[cachePath], 0, Cache[cachePath].Length, 0,
                        new AsyncCallback(SendCallback), request.Handler);
                }
                catch { }

                lock (Listener.Threads)
                {
                    Listener.Threads.Remove(System.Threading.Thread.CurrentThread);
                }
            }
            else if (File.Exists(cachePath))
            {
                byte[] temp = File.ReadAllBytes(cachePath);

                lock (Cache)
                {
                    if (!Cache.ContainsKey(cachePath))
                        Cache.Add(cachePath, temp);
                }

                try
                {
                    request.Handler.BeginSend(temp, 0, temp.Length, 0,
                        new AsyncCallback(SendCallback), request.Handler);
                }
                catch { }

                lock (Listener.Threads)
                {
                    Listener.Threads.Remove(System.Threading.Thread.CurrentThread);
                }
            }
            else
            {
                byte[] temp = Request(request.Session, request.Data);

                if (!Directory.Exists(Path.GetDirectoryName(cachePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(cachePath));

                File.WriteAllBytes(cachePath, temp);
                Cache.Add(cachePath, temp);

                try
                {
                    request.Handler.BeginSend(temp, 0, temp.Length, 0,
                        new AsyncCallback(SendCallback), request.Handler);
                }
                catch { }

                lock (Listener.Threads)
                {
                    Listener.Threads.Remove(System.Threading.Thread.CurrentThread);
                }
            }

            return true;
        }
        private static byte[] CheckCache2(HttpListenerRequest request)
        {
            string cachePath = "";

            try
            {
                cachePath = Path.Combine(
                    PhysicalApplicationPath,
                    "Cache",
                    request.RawUrl.Split('?')[0].Replace("/", "\\").Remove(0, 1)
                );
            }
            catch { }

            string[] extensions = new string[]
            {
                ".png",
                ".css",
                ".ttf",
                ".woff",
                ".woff2",
                ".js"
            };

            bool cacheable = false;

            foreach (string extension in extensions)
            {
                if (cachePath.EndsWith(extension))
                {
                    cacheable = true;
                    break;
                }
            }

            if (!cacheable)
                return new byte[0];

            if (Cache.ContainsKey(cachePath))
            {
                return Cache[cachePath];
            }
            else if (File.Exists(cachePath))
            {
                byte[] temp = File.ReadAllBytes(cachePath);

                lock (Cache)
                {
                    if (!Cache.ContainsKey(cachePath))
                        Cache.Add(cachePath, temp);
                }

                return Cache[cachePath];
            }
            else
            {
                TcpClient serverConnection = new TcpClient("127.0.0.1", 80);

                string req = string.Format(
                    "{0} {3} HTTP/1.0" + Environment.NewLine,
                    request.HttpMethod,
                    request.Url.Scheme,
                    "127.0.0.1",
                    request.RawUrl,
                    request.ProtocolVersion
                );

                req += request.Headers.ToString();

                byte[] temp = System.Text.Encoding.UTF8.GetBytes(req);

                NetworkStream writer = serverConnection.GetStream();

                writer.Write(temp, 0, temp.Length);
                //request.InputStream.CopyTo(writer);
                writer.Flush();

                temp = ReadFromStream(writer);

                if (!Directory.Exists(Path.GetDirectoryName(cachePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(cachePath));

                File.WriteAllBytes(cachePath, temp);
                Cache.Add(cachePath, temp);

                return Cache[cachePath];
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static byte[] Request(string session, byte[] data)
        {
            if (Sessions[session] == null)
            {
                Sessions[session] = FindServer();

                if (Sessions[session] == null)
                {
                    return File.ReadAllBytes("ClusterOverloadMessage.html");
                }
            }

            string server = Sessions[session];

            if (Servers.Items[server].State == ServerState.Offline)
            {
                SetServerOffline(server, false);

                return Request(session, data);
            }

            try
            {
                TcpClient serverConnection = new TcpClient(Sessions[session], 80);
                serverConnection.ReceiveTimeout = 20000;
                serverConnection.SendTimeout = 20000;

                NetworkStream writer = serverConnection.GetStream();

                writer.Write(data, 0, data.Length);
                writer.Flush();

                return ReadFromStream(writer);
            }
            catch (SocketException ex)
            {
                SetServerOffline(server, true);

                return Request(session, data);
            }
        }

        static void SetServerOffline(string server, bool sendMail)
        {
            lock (Servers)
            {
                //Servers = new ServerCollection("Servers.xml");
                try
                {
                    ServerCollection servers = new ServerCollection();
                    Servers = servers;
                }
                catch
                {

                }

                if (Servers.Items.ContainsKey(server))
                {
                    if (Servers.Items[server].State == ServerState.Offline)
                        return;

                    Servers.Items[server].State = ServerState.Offline;
                    Servers.Save();

                    if (sendMail)
                        SendServerOfflineNotification(Servers.Items[server]);
                }

                lock (Sessions)
                {
                    foreach (string s in Sessions.Keys.ToArray())
                    {
                        if (Sessions[s] == server)
                            Sessions[s] = null;
                    }
                }
            }
        }

        private static void SendServerOfflineNotification(Server server)
        {
            // configuration values from the web.config file.
            MailConfiguration mailConfiguration = new MailConfiguration(true);
            // Create a new mail by the mail configuration.
            Mail mail = new Mail(mailConfiguration, "_NONE_")
            {
                TemplatePath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "ServerOfflineNotification.html"
                ),
                Subject = "SERVER OFFLINE"
            };

            mail.Placeholders.Add("ServerIp", server.IP);
            mail.Placeholders.Add("Server", server.Description);

            // Send the mail.
            mail.Send(ConfigurationManager.AppSettings["ServerOfflineNotificationReciepent"]);
        }

        private static string FindServer()
        {
            string result = null;

            lock (Servers)
            {
                //Servers = new ServerCollection("Servers.xml");
                Servers = new ServerCollection();

                if (Servers.Items.ContainsKey(PreferedServer) && 
                    Servers.Items[PreferedServer].State == ServerState.Online)
                {
                    result = PreferedServer;
                }
                else
                {
                    foreach (string server in Servers.Items.Keys.ToArray())
                    {
                        if (Servers.Items[server].State == ServerState.Offline)
                            continue;

                        if (Servers.Items[server].Role != ServerRole.Primary)
                            continue;

                        result = Servers.Items[server].IP;
                        break;
                    }
                }

                if (result == null)
                {
                    foreach (string server in Servers.Items.Keys)
                    {
                        if (Servers.Items[server].State == ServerState.Offline)
                            continue;

                        if (Servers.Items[server].Role != ServerRole.Failover)
                            continue;

                        result = Servers.Items[server].IP;
                        break;
                    }
                }
            }

            return result;
        }


        static byte[] ReadFromStream(NetworkStream stream)
        {
            StringBuilder content = new StringBuilder();
            List<byte> result = new List<byte>();

            // 1 KB
            int chunkSize = 1024;

            byte[] buffer = new byte[chunkSize];

            int contentLength = -1;
            int bytesRead = 0;

            while (true)
            {
                try
                {
                    bytesRead = stream.Read(buffer, 0, chunkSize);
                }
                catch
                {
                    return result.ToArray();
                }

                if (contentLength == -1)
                    content.Append(Encoding.ASCII.GetString(
                    buffer, 0, bytesRead));

                result.AddRange(buffer.ToList().GetRange(0, bytesRead));

                buffer = new byte[chunkSize];

                if (contentLength == -1)
                    contentLength = ParseContentLength(content.ToString());

                if (contentLength != -1 && result.Count >= contentLength)
                    break;
            }

            return result.ToArray();
        }

        private static int ParseContentLength(string content)
        {
            int contentLength = -1;
            int index = content.ToString().IndexOf("\r\n\r\n");

            if (index != -1)
            {
                string header = content.ToString().Split(new string[]
                {
                            "\r\n\r\n"
                }, StringSplitOptions.None)[0] + "\r\n";

                index = header.IndexOf("Content-Length:");

                if (index == -1)
                {
                    string chunks = content.ToString().Split(new string[]
                    {
                        "\r\n\r\n"
                    }, StringSplitOptions.None)[1];

                    index = chunks.IndexOf("\r\n");

                    if (index == -1)
                        return contentLength;

                    chunks = chunks.Substring(0, index);

                    try
                    {
                        contentLength = (Convert.ToInt32(chunks, 16));
                    }
                    catch
                    {
                        contentLength = 0;
                    }

                    contentLength += header.Length + chunks.Length + 4 + 20;

                    //contentLength = -2;
                    return contentLength;
                }

                int index2 = header.IndexOf("\r\n", index);

                if (index2 == -1)
                    return contentLength;

                index += 15;

                contentLength = int.Parse(header.Substring(index, index2 - index).Trim()) + header.Length + 2;
            }

            return contentLength;
        }
        private static int ParseContentLength2(string content)
        {
            int contentLength = -1;
            int index = content.ToString().IndexOf("\r\n\r\n");

            if (index != -1)
            {
                string header = content.ToString().Split(new string[]
                {
                            "\r\n\r\n"
                }, StringSplitOptions.None)[0] + "\r\n";

                index = header.IndexOf("Content-Length:");

                if (index == -1)
                {
                    string chunks = content.ToString().Split(new string[]
                    {
                        "\r\n\r\n"
                    }, StringSplitOptions.None)[1];

                    index = chunks.IndexOf("\r\n");

                    if (index == -1)
                        return contentLength;

                    chunks = chunks.Substring(0, index);

                    try
                    {
                        contentLength = (Convert.ToInt32(chunks, 16));
                    }
                    catch
                    {
                        contentLength = 0;
                    }

                    //contentLength += header.Length + chunks.Length + 4 + 20;

                    //contentLength = -2;
                    return contentLength;
                }

                int index2 = header.IndexOf("\r\n", index);

                if (index2 == -1)
                    return contentLength;

                index += 15;

                contentLength = int.Parse(header.Substring(index, index2 - index).Trim());// + header.Length + 2;
            }

            return contentLength;
        }

        public static void ReadSendCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            client.EndSend(ar);
        }


        #endregion


        #region Event Handlers

        private static void Listener_OnRequest(object sender, EventArgs e)
        {
            Request request = (Request)sender;

            if (request.Session != null)
            {
                lock (Sessions)
                {
                    if (!Sessions.ContainsKey(request.Session))
                    {
                        Sessions.Add(request.Session, FindServer());
                    }
                }
            }

            // Check if the request if for a cacheable file.
            if (CheckCache(request))
                return;

            if (request.Path != null && request.Path.StartsWith("SwitchService.ashx?"))
            {
                string method = request.Path.Split(new string[]
                {
                    "Method="
                }, StringSplitOptions.None)[1].Split('&')[0];

                SwitchService service = new Classes.SwitchService();

                if (service.Methods.ContainsKey(method))
                {
                    service.Methods[method].Invoke(request);
                }
                else
                {
                    Response r = new Response();
                    r.ContentType = "text/html";
                    r.Data = System.Text.Encoding.UTF8.GetBytes("NotImplemented");

                    r.Send(request.Handler);
                }

                return;
            }

            // Forward the request to the server.
            byte[] response = new byte[0];
            
            if(request.Data.Length != 0)
                response = Request(request.Session, request.Data);

            // Check if there is a response.
            /*if (response.Length == 0)
                return;*/

            try
            {
                StateObject state = new StateObject();
                state.workSocket = request.Handler;

                request.Handler.BeginSend(
                    response,
                    0,
                    response.Length,
                    SocketFlags.None,
                    new AsyncCallback(ReadSendCallback),
                    request.Handler
                );
            }
            catch (Exception ex)
            {
                //socket.Disconnect(false);
                //socket.Dispose();
            }

            lock (Listener.Threads)
            {
                Listener.Threads.Remove(System.Threading.Thread.CurrentThread);
            }
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Servers = new ServerCollection();
            
            int stuck = 0;

            lock (Listener.Threads)
            {
                foreach (Thread thread in Listener.Threads.Keys)
                {
                    if ((Listener.Threads[thread] - DateTime.Now).TotalMinutes >= 5)
                    {
                        thread.Abort();
                        stuck++;
                    }
                }

                foreach (Thread thread in Listener.Threads.Keys.ToArray())
                {
                    if (thread.ThreadState == ThreadState.Running)
                        continue;

                    Listener.Threads.Remove(thread);
                    stuck++;
                }
            }

            if (stuck != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format(
                    "{0} threads were aborted.",
                    stuck
                ));

                Console.ResetColor();
            }
        }

        #endregion
    }

    public class Request
    {
        #region Properties
        public int ContentLength { get; set; }

        public string Session { get; set; }

        public byte[] Data { get; set; }

        public Socket Handler { get; set; }

        public string Method { get; set; }

        public string Path { get; set; }

        public string HttpVersion { get; set; }

        public Dictionary<string, string> Values { get; set; }

        #endregion


        #region Constructor

        public Request(byte[] data, bool parseAll = false)
        {
            this.Session = "unknown";

            data = SetHttpVersion(data);

            this.Data = data;

            if (parseAll)
                Parse(data);
            else
                ParseLittle(data);
        }

        #endregion


        #region Methods

        private byte[] SetHttpVersion(byte[] data)
        {
            byte[] temp = System.Text.Encoding.UTF8.GetBytes(
                System.Text.Encoding.UTF8.GetString(data).Split('\r')[0].Replace(" HTTP/1.1", " HTTP/1.0")
            );

            for (int i = 0; i < temp.Length; i++)
            {
                data[i] = temp[i];
            }

            return data;
        }

        private void ParseLittle(byte[] data)
        {
            this.Values = new Dictionary<string, string>();

            string[] values = Encoding.UTF8.GetString(data).Split('\n');

            if (values.Length == 1 && values[0] == "")
                return;

            string[] parts = values[0].Split(' ');

            if (parts[0] == "GET" || parts[0] == "POST")
            {
                this.Path = parts[1];
            }
            else
            {
                this.Method = "GET";
                this.Path = parts[0];

                int index = this.Path.IndexOf("HTTP/1.");

                if (index != -1)
                {
                    this.Path = this.Path.Substring(0, index);
                }
            }

            foreach (string value in values)
            {
                if (!value.StartsWith("Cookie:"))
                    continue;

                if (!value.Contains("ASP.NET_SessionId="))
                    continue;

                this.Session = value.Split(new string[]
                {
                    "ASP.NET_SessionId="
                }, StringSplitOptions.None)[1].Split(';')[0];
            }


            if (this.Path.StartsWith("/"))
                this.Path = this.Path.Remove(0, 1);
        }

        private void Parse(byte[] data)
        {
            this.Values = new Dictionary<string, string>();

            string[] values = Encoding.UTF8.GetString(data).Split(new string[] {
                "\r\n\r\n" }, StringSplitOptions.None)[0].Split('\n');

            if (values.Length == 1 && values[0] == "")
                return;

            string[] parts = values[0].Split(' ');

            if (parts[0] == "GET" || parts[0] == "POST")
            {
                this.Method = parts[0];
                this.Path = parts[1];
                this.HttpVersion = parts[2];
            }
            else
            {
                this.Method = "GET";
                this.Path = parts[0];

                if (parts.Length == 2)
                    this.HttpVersion = parts[1];

                int index = this.Path.IndexOf("HTTP/1.");

                if (index != -1)
                {
                    this.Path = this.Path.Substring(0, index);
                }
            }


            if (this.Path.StartsWith("/"))
                this.Path = this.Path.Remove(0, 1);

            for (int i = 1; i < values.Length; i++)
            {
                parts = values[i].Split(':');

                if (parts.Length < 2)
                    continue;

                if (this.Values.ContainsKey(parts[0].Trim()))
                    continue;

                this.Values.Add(parts[0].Trim(), parts[1].Trim());
            }

            if (this.Values.ContainsKey("Content-Length"))
                this.ContentLength = int.Parse(this.Values["Content-Length"]);
        }

        #endregion
    }
}
