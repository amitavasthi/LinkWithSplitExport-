using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Switch2.Classes
{
    public class Listener
    {
        #region Properties

        public Dictionary<Thread, DateTime> Threads = new Dictionary<Thread, DateTime>();

        // Thread signal.
        public ManualResetEvent allDone = new ManualResetEvent(false);

        //public static Dictionary<int, string> Handles = new Dictionary<int, string>();

        public event EventHandler OnRequest;

        #endregion


        #region Constructor

        public Listener()
        {

        }

        #endregion


        #region Methods

        public void Start()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            //IPEndPoint localEndPoint = new IPEndPoint(new IPAddress(new byte[] {
            //    127,
            //    0,
            //    0,
            //    1
            //}), 90);
            IPEndPoint localEndPoint = new IPEndPoint(new IPAddress(new byte[] {
                10,
                0,
                0,
                4
            }), 80);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(1000);

                while (true)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(ProcessRequest));
                    thread.Start(listener.Accept());
                    //ProcessRequest(listener.Accept());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ProcessRequest(object sender)
        {
            Socket socket = (Socket)sender;

            byte[] buffer = new byte[1024];

            StringBuilder result = new StringBuilder();

            //Console.WriteLine("[START] Parsing request from browser");

            int total = 0;
            int contentLength = -1;
            int length;
            while (true)
            {
                try
                {
                    length = socket.Receive(buffer);
                    total += length;

                    result.Append(System.Text.Encoding.UTF8.GetString(buffer, 0, length));

                    if (total == contentLength)
                        break;

                    if (contentLength == -1 && result.ToString().Contains("Content-Length: ") && result.ToString().Contains("\r\n\r\n"))
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

                    if (contentLength == -1 && result.ToString().EndsWith("\r\n\r\n"))
                    {
                        break;
                    }
                }
                catch
                {
                    socket.Dispose();
                    return;
                }
            }

            //Console.WriteLine("[COMPLETED] Parsing request from browser");
            //Task<int> test = socket.ReceiveAsync();

            Request request = new Request(System.Text.Encoding.
                UTF8.GetBytes(result.ToString()/*.Replace(" HTTP/1.1\r\n", " HTTP/1.0\r\n")*/));
            request.Handler = socket;

            if (this.OnRequest != null)
            {
                try
                {
                    this.OnRequest(request, new EventArgs());
                }
                catch
                {
                    socket.Dispose();
                }
            }
        }

        //public void AcceptCallback(IAsyncResult ar)
        //{
        //    // Signal the main thread to continue.
        //    allDone.Set();

        //    Thread thread = new Thread(() =>
        //    {
        //        // Get the socket that handles the client request.
        //        Socket listener = (Socket)ar.AsyncState;
        //        Socket handler = listener.EndAccept(ar);

        //        // Create the state object.
        //        StateObject state = new StateObject();
        //        state.workSocket = handler;
        //        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
        //            new AsyncCallback(ReadCallback), state);
        //    });

        //    thread.Start();

        //    lock (Threads)
        //    {
        //        Threads.Add(thread, DateTime.Now);
        //    }
        //}

        //public void ReadCallback(IAsyncResult ar)
        //{
        //    String content = String.Empty;

        //    // Retrieve the state object and the handler socket
        //    // from the asynchronous state object.
        //    StateObject state = (StateObject)ar.AsyncState;
        //    Socket handler = state.workSocket;

        //    /*if (test.ContainsKey(handler.Handle.ToInt32()))
        //        state.sb.Append(test[handler.Handle.ToInt32()]);*/

        //    try
        //    {
        //        // Read data from the client socket. 
        //        int bytesRead = handler.EndReceive(ar);

        //        bool finished = false;

        //        if (bytesRead > 0)
        //        {
        //            // There  might be more data, so store the data received so far.
        //            state.sb.Append(Encoding.ASCII.GetString(
        //                state.buffer, 0, bytesRead));

        //            // Check for end-of-file tag. If it is not there, read 
        //            // more data.
        //            content = state.sb.ToString();

        //            if (!content.StartsWith("GET"))
        //            {
        //                finished = false;

        //                int index = content.IndexOf("Content-Length:");

        //                if (index != -1)
        //                {
        //                    int index2 = content.IndexOf("\r\n", index);

        //                    if (index2 != -1)
        //                    {
        //                        index += 15;

        //                        int contentLength = int.Parse(content.Substring(index, index2 - index).Trim());

        //                        if (Encoding.ASCII.GetBytes(content).Length == (contentLength + content.IndexOf("\r\n\r\n") + 4))
        //                            finished = true;
        //                    }
        //                }

        //                if (!finished)
        //                {
        //                    /*if (Handles.ContainsKey(handler.Handle.ToInt32()))
        //                        Handles[handler.Handle.ToInt32()] = state.sb.ToString();
        //                    else
        //                        Handles.Add(handler.Handle.ToInt32(), state.sb.ToString());*/
        //                }
        //            }
        //            else if (content.EndsWith("\r\n\r\n"))
        //            {
        //                finished = true;
        //            }

        //            //result.AddRange(state.buffer.ToList().GetRange(0, bytesRead));
        //            /*if (content.IndexOf("<EOF>") > -1)
        //            {
        //                // All the data has been read from the 
        //                // client. Display it on the console.
        //                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
        //                    content.Length, content);
        //                // Echo the data back to the client.
        //                Send(handler, content);
        //            }
        //            else*/
        //            //if(!finished)
        //            {
        //                // Not all data received. Get more.
        //                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
        //                 new AsyncCallback(ReadCallback), state);
        //            }
        //        }
        //        else
        //        {
        //            finished = true;
        //        }

        //        if (finished)
        //        {
        //            /*if (test.ContainsKey(handler.Handle.ToInt32()))
        //                test.Remove(handler.Handle.ToInt32());*/
        //            byte[] result = System.Text.Encoding.ASCII.GetBytes(state.sb.ToString());
        //            state.sb.Clear();

        //            if (this.OnRequest == null)
        //                return;

        //            Request request = new Request(result);
        //            request.Handler = handler;

        //            new Thread(() => this.OnRequest(
        //                request, 
        //                new EventArgs()
        //            )).Start();
        //        }
        //    }
        //    catch// (Exception ex)
        //    {
        //        //Console.WriteLine(ex.Message);
        //        lock (Threads)
        //        {
        //            Threads.Remove(System.Threading.Thread.CurrentThread);
        //        }
        //    }
        //}

        #endregion


        #region Event Handlers

        #endregion
    }
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
}
