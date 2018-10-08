using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Backup.Classes
{
    public class FileSystemBackupDefinitionCollection : IEnumerable
    {
        #region Properties

        private List<FileSystemBackupDefinition> Items { get; set; }

        public string DirectoryName { get; set; }

        #endregion


        #region Constructor

        public FileSystemBackupDefinitionCollection(string directoryName)
        {
            this.DirectoryName = directoryName;
            this.Items = new List<FileSystemBackupDefinition>();

            this.Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            string fileNameDefinition = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                "FileSystem.xml"
            );

            if (!File.Exists(fileNameDefinition))
                return;

            XmlDocument document = new XmlDocument();
            document.Load(fileNameDefinition);

            foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("Directory"))
            {
                this.Items.Add(new FileSystemBackupDefinition(
                    this,
                    xmlNode
                ));
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.Items.GetEnumerator();
        }

        #endregion
    }

    public class FileSystemBackupDefinition
    {
        #region Properties

        public FileSystemBackupDefinitionCollection Owner { get; set; }


        public string Path { get; set; }

        public List<FileSystemBackupDefinitionFormatType> FormatTypes { get; set; }

        #endregion


        #region Constructor

        public FileSystemBackupDefinition(FileSystemBackupDefinitionCollection owner, XmlNode xmlNode)
        {
            this.Owner = owner;

            this.Path = xmlNode.Attributes["Path"].Value;
            this.FormatTypes = new List<FileSystemBackupDefinitionFormatType>();

            int i = 0;
            while (true)
            {
                if (xmlNode.Attributes["FormatValue" + i] == null)
                    break;

                this.FormatTypes.Add((FileSystemBackupDefinitionFormatType)Enum.Parse(
                    typeof(FileSystemBackupDefinitionFormatType),
                    xmlNode.Attributes["FormatValue" + i].Value
                ));
                i++;
            }
        }

        #endregion


        #region Methods

        public void Backup(string backupRoot, string[] formatValues)
        {
            string fileNameSource = string.Format(System.IO.Path.Combine(
                ConfigurationManager.AppSettings["ApplicationPath"],
                this.Path
            ), formatValues);

            string fileNameDestination = string.Format(System.IO.Path.Combine(
                this.Owner.DirectoryName,
                this.Path
            ), formatValues);

            if (new FileInfo(fileNameSource).Extension != "")
            {
                BackupFile(
                    fileNameSource,
                    fileNameDestination
                );
            }
            else
            {
                BackupDirectory(fileNameSource, fileNameDestination);
            }
        }

        private void BackupDirectory(string fileNameSource, string fileNameDestination)
        {
            if (!Directory.Exists(fileNameSource))
                return;

            if (!Directory.Exists(fileNameDestination))
                Directory.CreateDirectory(fileNameDestination);

            // Run through all files of the source directory.
            foreach (string fileName in Directory.GetFiles(fileNameSource))
            {
                BackupFile(fileName, System.IO.Path.Combine(
                    fileNameDestination,
                    new FileInfo(fileName).Name
                ));
            }

            // Run through all sub directories of the source directory.
            foreach (string directoryName in Directory.GetDirectories(fileNameSource))
            {
                BackupDirectory(directoryName, System.IO.Path.Combine(
                    fileNameDestination,
                    new DirectoryInfo(directoryName).Name
                ));
            }
        }

        private void BackupFile(string fileNameSource, string fileNameDestination)
        {
            if (!File.Exists(fileNameSource))
                return;

            if (!Directory.Exists(System.IO.Path.GetDirectoryName(fileNameDestination)))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileNameDestination));

            File.Copy(
                fileNameSource,
                fileNameDestination,
                true
            );
        }

        #endregion
    }

    public enum FileSystemBackupDefinitionFormatType
    {
        Client,
        User
    }
}
