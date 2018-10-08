using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace Azure_Switch.Classes
{
    public class SocketLevelWebClient
    {
        public byte[] SendWebRequest(string url, string request)
        {
            using (TcpClient tc = new TcpClient())
            {
                tc.Connect(url, 80);

                using (NetworkStream ns = tc.GetStream())
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(ns))
                    {
                        sw.Write(request);
                        sw.Flush();

                        int contentLength = -1;
                        int headerLength = -1;

                        List<byte> result = new List<byte>();

                        // 1 KB
                        int chunkSize = 1024;

                        byte[] buffer = new byte[chunkSize];

                        int responseLength = 0;
                        int bytesRead = 0;

                        while (true)
                        {
                            bytesRead = ns.Read(buffer, 0, chunkSize);
                            responseLength += bytesRead;

                            result.AddRange(buffer.ToList().GetRange(0, bytesRead));

                            buffer = new byte[chunkSize];

                            if (contentLength == -1 && headerLength != -1)
                            {
                                if (ns.DataAvailable == false && tc.Available == 0 && bytesRead == 0)
                                    break;

                                // 100 MB
                            }
                            else if (contentLength == -1)
                            {
                                string text = System.Text.Encoding.ASCII.GetString(result.ToArray());

                                if (text.Contains("Content-Length:"))
                                {
                                    try
                                    {
                                        contentLength = int.Parse(text.Split(
                                            new string[] { "Content-Length:" },
                                            StringSplitOptions.RemoveEmptyEntries
                                        )[1].Split('\n')[0].Trim());
                                    }
                                    catch { }
                                }

                                if (text.Contains("\r\n\r\n"))
                                {
                                    try
                                    {
                                        headerLength = text.Split(
                                            new string[] { "\r\n\r\n" },
                                            StringSplitOptions.RemoveEmptyEntries
                                        )[0].Length + 4;
                                    }
                                    catch { }
                                }
                            }
                            else if (headerLength == -1)
                            {
                                string text = System.Text.Encoding.ASCII.GetString(result.ToArray());

                                if (text.Contains("\r\n\r\n"))
                                {
                                    try
                                    {
                                        headerLength = text.Split(
                                            new string[] { "\r\n\r\n" },
                                            StringSplitOptions.RemoveEmptyEntries
                                        )[0].Length + 4;
                                    }
                                    catch { }
                                }
                            }

                            if (contentLength != -1 && headerLength != -1 && result.Count >= (contentLength + headerLength))
                            {
                                break;
                            }
                        }

                        tc.Close();

                        return result.ToArray();
                    }
                }
            }
        }

        private void Load(List<byte> result, byte[] buffer, int responseLength, int chunkSize, NetworkStream ns, TcpClient tc)
        {

            if (responseLength > tc.ReceiveBufferSize || responseLength == 0)
            {
                System.Threading.Thread.Sleep(100);

                Load(
                    result,
                    buffer,
                    responseLength,
                    chunkSize,
                    ns,
                    tc
                );
            }
        }
    }
}