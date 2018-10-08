using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Switch2.Classes
{
    public class Request
    {
        #region Properties

        public string SessionId { get; set; }

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
            this.SessionId = null;
            this.Data = data;

            if (parseAll)
                Parse(data);
            else
                ParseLittle(data);
        }

        #endregion


        #region Methods

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

                this.SessionId = value.Split(new string[]
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

            string[] values = Encoding.UTF8.GetString(data).Split('\n');

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
        }

        #endregion
    }
}
