using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Switch.Classes
{

    class Response
    {
        #region Properties

        public string ContentType { get; set; }

        public byte[] Data { get; set; }

        #endregion


        #region Constructor

        public Response()
        {

        }

        #endregion


        #region Methods

        public void Send(Socket socket)
        {
            StringBuilder header = new StringBuilder();
            header.Append("HTTP/1.0 200 OK\r\n");
            header.Append(string.Format(
                "Content-Type:{0}\r\n",
                this.ContentType
            ));
            header.Append("Accept-Ranges: bytes\r\n");
            header.Append("Server: LiNK-Switch/1.0\r\n");
            header.Append("Access-Control-Allow-Origin: *\r\n");
            //header.Append("ETag:\"dfac44545378d01: 0\"\r\n");
            //header.Append("Connection: keep-alive");
            header.Append(string.Format(
                "Content-Length:{0}\r\n",
                this.Data.Length
            ));
            header.Append("\r\n");

            List<byte> data = new List<byte>();
            data.AddRange(Encoding.UTF8.GetBytes(header.ToString()));

            header.Clear();

            data.AddRange(this.Data);

            socket.Send(data.ToArray(), SocketFlags.None);
        }

        #endregion
    }
}
