using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClientBackup1.Classes
{
    public class ClientBackupCollection : IEnumerable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the full path to the xml file that
        /// contains the client backup definitions.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a collection that contains the client backup items.
        /// </summary>
        public List<ClientBackup> Items { get; set; }

        #endregion


        #region Constructor

        public ClientBackupCollection(string clientName, string fileName)
        {
            this.ClientName = clientName;
            this.FileName = fileName;

            this.Items = new List<ClientBackup>();

            this.Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            // Create a new xml document that contains the contents
            // of the client backup definitions xml file.
            XmlDocument document = new XmlDocument();

            // Load the contents o the client backup
            // definitions xml file into the xml document.
            document.Load(this.FileName);

            // Run through all xml nodes that
            // contain a client backup definition.
            foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("ClientBackup"))
            {
                this.Items.Add(new ClientBackup(
                    this,
                    xmlNode
                ));
            }

            this.Items = this.Items.OrderByDescending(x => x.CreationDate).ToList();
        }


        public ClientBackup CreateBackup()
        {
            return new ClientBackup(this, null);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.Items.GetEnumerator();
        }

        #endregion
    }
}
