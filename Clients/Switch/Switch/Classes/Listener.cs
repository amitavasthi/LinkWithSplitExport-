using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Switch.Classes
{
    public class Listener
    {
        #region Properties

        public Dictionary<Thread, DateTime> Threads = new Dictionary<Thread, DateTime>();

        // Thread signal.
        public ManualResetEvent allDone = new ManualResetEvent(false);

        public static Dictionary<int, string> Handles = new Dictionary<int, string>();

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

            IPEndPoint localEndPoint = new IPEndPoint(
                IPAddress.Parse(ConfigurationManager.AppSettings["ListenerAddress"]),
                int.Parse(ConfigurationManager.AppSettings["ListenerPort"])
            );

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
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener
                    );
                    /*Socket socket = listener.Accept();
                    Accept(socket);*/

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void _Start()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            IPEndPoint localEndPoint = new IPEndPoint(
                IPAddress.Parse(ConfigurationManager.AppSettings["ListenerAddress"]),
                int.Parse(ConfigurationManager.AppSettings["ListenerPort"])
            );

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(1000);

            ThreadPool.QueueUserWorkItem((o) =>
            {
                while (true)
                {
                    ThreadPool.QueueUserWorkItem((c) =>
                    {
                    }, listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener
                    ));
                }
            });
        }

        public void Accept(Socket handler)
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    // Create the state object.
                    StateObject state = new StateObject();
                    state.workSocket = handler;
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
                catch
                {
                }
            });

            thread.Start();

            lock (Threads)
            {
                Threads.Add(thread, DateTime.Now);
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            Thread thread = new Thread(() =>
            {
                try
                {
                    // Get the socket that handles the client request.
                    Socket listener = (Socket)ar.AsyncState;
                    Socket handler = listener.EndAccept(ar);

                    // Create the state object.
                    StateObject state = new StateObject();
                    state.workSocket = handler;
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
                catch
                {
                }
            });

            thread.Start();

            lock (Threads)
            {
                Threads.Add(thread, DateTime.Now);
            }
        }
        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            /*if (test.ContainsKey(handler.Handle.ToInt32()))
                state.sb.Append(test[handler.Handle.ToInt32()]);*/

            try
            {
                // Read data from the client socket. 
                int bytesRead = handler.EndReceive(ar);

                bool finished = false;

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    content = state.sb.ToString();

                    if (!content.StartsWith("GET"))
                    {
                        finished = false;

                        int index = content.IndexOf("Content-Length:");

                        if (index != -1)
                        {
                            int index2 = content.IndexOf("\r\n", index);

                            if (index2 != -1)
                            {
                                index += 15;

                                int contentLength = int.Parse(content.Substring(index, index2 - index).Trim());

                                if (Encoding.ASCII.GetBytes(content).Length == (contentLength + content.IndexOf("\r\n\r\n") + 4))
                                    finished = true;
                            }
                        }

                        if (!finished)
                        {
                            if (Handles.ContainsKey(handler.Handle.ToInt32()))
                                Handles[handler.Handle.ToInt32()] = state.sb.ToString();
                            else
                                Handles.Add(handler.Handle.ToInt32(), state.sb.ToString());
                        }
                    }
                    else if (content.EndsWith("\r\n\r\n"))
                    {
                        finished = true;
                    }

                    //result.AddRange(state.buffer.ToList().GetRange(0, bytesRead));
                    /*if (content.IndexOf("<EOF>") > -1)
                    {
                        // All the data has been read from the 
                        // client. Display it on the console.
                        Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                            content.Length, content);
                        // Echo the data back to the client.
                        Send(handler, content);
                    }
                    else*/

                    //if (!finished)
                    {
                        // Not all data received. Get more.
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                         new AsyncCallback(ReadCallback), state);
                        //return;
                    }
                }
                else
                {
                    finished = true;
                    // Not all data received. Get more.
                    /*handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                     new AsyncCallback(ReadCallback), state);
                    return;*/
                    /*System.Threading.Thread.CurrentThread.Abort();
                    lock (Threads)
                    {
                        Threads.Remove(System.Threading.Thread.CurrentThread);
                    }*/
                }

                if (finished)
                {
                    /*if (test.ContainsKey(handler.Handle.ToInt32()))
                        test.Remove(handler.Handle.ToInt32());*/
                    byte[] result = System.Text.Encoding.ASCII.GetBytes(state.sb.ToString());
                    state.sb.Clear();

                    if (this.OnRequest == null)
                        return;

                    Request request = new Request(result);
                    request.Handler = handler;

                    new Thread(() => this.OnRequest(
                        request,
                        new EventArgs()
                    )).Start();
                }
            }
            catch// (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                lock (Threads)
                {
                    Threads.Remove(System.Threading.Thread.CurrentThread);
                }
            }
        }

        #endregion


        #region Event Handlers

        #endregion
    }
}
