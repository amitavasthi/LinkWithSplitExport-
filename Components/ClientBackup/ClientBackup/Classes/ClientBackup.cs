using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClientBackup1.Classes
{
    public class ClientBackup
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning client backup collection.
        /// </summary>
        public ClientBackupCollection Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node that
        /// contains the client backup definitions.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the unique id of the client backup.
        /// </summary>
        public Guid Id
        {
            get
            {
                return Guid.Parse(this.GetValue("Id"));
            }
            set
            {
                this.SetValue("Id", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the unique id of the
        /// user who executed the backup job.
        /// </summary>
        public Guid IdUser
        {
            get
            {
                return Guid.Parse(this.GetValue("IdUser"));
            }
            set
            {
                this.SetValue("IdUser", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the status of the client backup.
        /// </summary>
        public ClientBackupStatus Status
        {
            get
            {
                return (ClientBackupStatus)Enum.Parse(
                    typeof(ClientBackupStatus),
                    this.GetValue("Status")
                );
            }
            set
            {
                this.SetValue("Status", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the full path where the backup is stored.
        /// </summary>
        public string BackupPath
        {
            get
            {
                return this.GetValue("BackupPath");
            }
            set
            {
                this.SetValue("BackupPath", value);
            }
        }

        /// <summary>
        /// Gets or sets the creation date of the backup.
        /// </summary>
        public DateTime CreationDate
        {
            get
            {
                return DateTime.Parse(this.GetValue("CreationDate"));
            }
            set
            {
                this.SetValue("CreationDate", value.ToString());
            }
        }

        #endregion


        #region Constructor

        public ClientBackup(ClientBackupCollection owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;
        }

        #endregion


        #region Methods

        private string GetValue(string name)
        {
            if (this.XmlNode.Attributes[name] == null)
                return "";

            return this.XmlNode.Attributes[name].Value;
        }

        private void SetValue(string name, string value)
        {
            if (this.XmlNode.Attributes[name] == null)
                this.XmlNode.AddAttribute(name, value);
            else
                this.XmlNode.Attributes[name].Value = value;
        }


        public void Restore()
        {

        }

        #endregion
    }

    public enum ClientBackupStatus
    {
        Running,
        Finished,
        Failed
    }
}
