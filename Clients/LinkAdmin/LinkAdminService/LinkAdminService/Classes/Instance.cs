using ApplicationUtilities.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace LinkAdminService.Classes
{
    public class Instance
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path
        /// to the instance's directory.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the name of the instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the software
        /// version number of the instance.
        /// </summary>
        public string Version { get; set; }

        #endregion


        #region Constructor

        public Instance(string path)
        {
            this.Source = path;

            this.Name = new DirectoryInfo(this.Source).Name;

            this.Version = FileVersionInfo.GetVersionInfo(Path.Combine(
                this.Source,
                "bin",
                "LinkOnline.dll"
            )).ProductVersion;
        }

        #endregion


        #region Methods

        public Client[] GetClients()
        {
            ClientManager manager = new ClientManager(Path.Combine(
                this.Source,
                "App_Data",
                "Clients.xml"
            ));

            return manager.Get();
        }

        public string ToXml()
        {
            StringBuilder result = new StringBuilder();

            result.Append(string.Format(
                "<Instance Name=\"{0}\" Version=\"{1}\">",
                this.Name,
                this.Version
            ));

            foreach (Client client in this.GetClients())
            {
                result.Append(client.XmlNode.OuterXml);
            }

            result.Append("</Instance>");

            return result.ToString();
        }

        #endregion
    }
}