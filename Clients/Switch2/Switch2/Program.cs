using Switch2.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Switch2
{
    public class Program
    {
        #region Properties

        public static Listener Listener { get; set; }

        public static Dictionary<string, byte[]> Cache = new Dictionary<string, byte[]>();

        #endregion


        #region Methods

        public static int Main(String[] args)
        {
            /*timer = new System.Timers.Timer(60000);

            timer.Elapsed += Timer_Elapsed;

            timer.Start();*/

            Console.WriteLine("Starting listener...");

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
                    Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
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
                    /*request.Handler.BeginSend(Cache[cachePath], 0, Cache[cachePath].Length, 0,
                        new AsyncCallback(SendCallback), request.Handler);*/
                    request.Handler.Send(Cache[cachePath]);
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
                    /*request.Handler.BeginSend(temp, 0, temp.Length, 0,
                        new AsyncCallback(SendCallback), request.Handler);*/
                    request.Handler.Send(temp);
                }
                catch { }

                lock (Listener.Threads)
                {
                    Listener.Threads.Remove(System.Threading.Thread.CurrentThread);
                }
            }
            else
            {
                byte[] temp = Request(request.Data);

                if (!Directory.Exists(Path.GetDirectoryName(cachePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(cachePath));

                File.WriteAllBytes(cachePath, temp);
                Cache.Add(cachePath, temp);

                try
                {
                    /*request.Handler.BeginSend(temp, 0, temp.Length, 0,
                        new AsyncCallback(SendCallback), request.Handler);*/
                    request.Handler.Send(temp);
                }
                catch { }

                lock (Listener.Threads)
                {
                    Listener.Threads.Remove(System.Threading.Thread.CurrentThread);
                }
            }

            request.Handler.Dispose();

            return true;
        }

        private static byte[] Request(byte[] data)
        {
            TcpClient serverConnection = new TcpClient();
            Task task = serverConnection.ConnectAsync("216.176.177.26", 80);

            task.Wait();

            NetworkStream writer = serverConnection.GetStream();

            writer.Write(data, 0, data.Length);
            writer.Flush();

            return ReadFromStream(writer);
        }


        static byte[] ReadFromStream(NetworkStream stream)
        {
            //Console.WriteLine("[START] Parsing request from server");

            StringBuilder result = new StringBuilder();

            // 1 KB
            int chunkSize = 1024;

            byte[] buffer = new byte[chunkSize];
            List<byte> data = new List<byte>();

            int contentLength = -1;
            int length = 0;
            int total = 0;

            int continueCount = 0;
            while (true)
            {
                if (stream.DataAvailable == false)
                {
                    if (continueCount++ >= 10000000)
                        break;

                    continue;
                }

                length = stream.Read(buffer, 0, chunkSize);
                total += length;

                if (contentLength == -1)
                    result.Append(System.Text.Encoding.UTF8.GetString(buffer, 0, length));

                data.AddRange(buffer.ToList().GetRange(0, length));

                if (total == contentLength)
                    break;

                if (contentLength == -1 && result.ToString().EndsWith("\r\n\r\n"))
                {
                    break;
                }

                if (contentLength == -1 &&
                    result.ToString().Contains("Content-Length: ") &&
                    result.ToString().Contains("\r\n\r\n"))
                {
                    contentLength = int.Parse(result.ToString().Split(new string[]
                    {
                            "Content-Length: "
                    }, StringSplitOptions.None)[1].Split('\r')[0]) + result.ToString().Split(new string[] {
                            "\r\n\r\n"
                    }, StringSplitOptions.None)[0].Length + 4;
                }

                if (total == contentLength)
                    break;
            }

            //Console.WriteLine("[COMPLETED] Parsing request from server");

            //return System.Text.Encoding.UTF8.GetBytes(result.ToString());
            return data.ToArray();
        }

        #endregion


        #region Event Handlers

        private static void Listener_OnRequest(object sender, EventArgs e)
        {
            Request request = (Request)sender;

            if (request.SessionId != null)
            {

            }

            // Check if the request if for a cacheable file.
            if (CheckCache(request))
                return;

            // Forward the request to the server.
            byte[] response = Request(request.Data);

            // Check if there is a response.
            if (response.Length == 0)
                return;

            try
            {
                StateObject state = new StateObject();
                state.workSocket = request.Handler;

                /*request.Handler.BeginSend(
                    response,
                    0,
                    response.Length,
                    SocketFlags.None,
                    new AsyncCallback(ReadSendCallback),
                    request.Handler
                );*/
                request.Handler.Send(response);
                request.Handler.Dispose();
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

        //private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    int stuck = 0;

        //    lock (Listener.Threads)
        //    {
        //        foreach (Thread thread in Listener.Threads.Keys)
        //        {
        //            if ((Listener.Threads[thread] - DateTime.Now).TotalMinutes >= 5)
        //            {
        //                thread.Abort();
        //                stuck++;
        //            }
        //        }

        //        foreach (Thread thread in Listener.Threads.Keys.ToArray())
        //        {
        //            if (thread.ThreadState == ThreadState.Running)
        //                continue;

        //            Listener.Threads.Remove(thread);
        //            stuck++;
        //        }
        //    }

        //    if (stuck != 0)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine(string.Format(
        //            "{0} threads were aborted.",
        //            stuck
        //        ));

        //        Console.ResetColor();
        //    }
        //}

        #endregion
    }
}
