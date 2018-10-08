using ApplicationUtilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Azure_Switch.Classes
{
    public class ServerCollection
    {
        #region Properties

        public Dictionary<string, Server> Items { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the server collection class.
        /// </summary>
        /// <param name="fileName">The full path to the server definition file.</param>
        public ServerCollection(string fileName)
        {
            this.Items = new Dictionary<string, Server>();

            // Create a new xml document that
            // contains the server definition.
            XmlDocument document = new XmlDocument();

            // Load the contentes of the server
            // definition file into the xml document.
            document.Load(fileName);

            // Run through all server definition xml nodes.
            foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("Server"))
            {
                // Create a new server by the xml node.
                Server server = new Server(xmlNode);

                // Add the server to the collection's items.
                this.Items.Add(server.IP, server);
            }
        }

        #endregion


        #region Methods

        public void Save()
        {
            // Create a new string builder that
            // contains the result xml string.
            StringBuilder result = new StringBuilder();

            result.Append("<Servers>");

            // Run through all defined servers.
            foreach (Server server in this.Items.Values)
            {
                result.Append(string.Format(
                    "<Server IP=\"{0}\" Description=\"{1}\" Role=\"{2}\" Countries=\"{3}\" State=\"{4}\"></Server>",
                    server.IP,
                    server.Description,
                    server.Role.ToString(),
                    string.Join(",", server.Countries),
                    server.State.ToString()
                ));
            }

            result.Append("</Servers>");

            System.IO.File.WriteAllText(System.IO.Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Servers.xml"
            ), result.ToString());
        }

        /// <summary>
        /// Returns the name of a server that is next to the user.
        /// </summary>
        /// <param name="IP">The ip of the user.</param>
        /// <returns></returns>
        public string GetServer(string IP, ServerRole role = ServerRole.Primary)
        {
            string country;
            try {
                country = GetCountry(IP);
            }
            catch
            {
                foreach (string server in this.Items.Keys)
                {
                    if(this.Items[server].Role == role && this.Items[server].State == ServerState.Online)
                    {
                        return server;
                    }
                }

                return null;
                //return this.Items.Where(x => x.Value.Role == role && x.Value.State == ServerState.Online).First().Key;
            }

            Dictionary<string, int> servers = new Dictionary<string, int>();

            // Run through all servers.
            foreach (string server in this.Items.Keys)
            {
                if (this.Items[server].State == ServerState.Offline)
                    continue;

                // Check if the country is assigned to the server.
                if (this.Items[server].Countries.Contains(country))
                {
                    servers.Add(server, this.Items[server].Sessions);
                }
            }

            // Check if servers where found for the country.
            if (servers.Count == 0)
            {
                KeyValuePair<string, Server>[] items = this.Items.Where(x => x.Value.Role == role && x.Value.State == ServerState.Online).ToArray();

                if (items.Length == 0)
                    return null;

                return items.First().Key;
            }

            // Return the server with least amount of sessions.
            return servers.OrderBy(x => x.Value).First().Key;
        }

        public string GetCountry(string IP)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "\"" + HttpContext.Current.Request.PhysicalApplicationPath + "bin\\curl.exe\"";
            p.StartInfo.Arguments = "ipinfo.io/" + IP + "/country";
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }

        #endregion
    }
}