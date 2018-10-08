using Backup.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.IO.Compression;

namespace Backup
{
    public class Program
    {
        static string fileNameClientBackups = "";
        static string clientBackupEntryPath = "";
        static string databaseName = "";
        static string databaseProvider = "";
        static string connectionString = "";
        static string clientName = "";
        static List<Guid> clientUsers;

        static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(10000);
            try
            {
                // FOR TEST ONLY:
                if (false)
                {
                    args = new string[6];
                    args[0] = "Database=LinkManager";
                    args[1] = "ClientName=LinkManager";
                    args[2] = "DatabaseProvider=System.Data.SqlClient";
                    args[3] = "ConnectionString=Data%20Source%3D.%5CStandard%3BInitial%20Catalog%3DLinkManager%3BIntegrated%20Security%3DTrue";
                    args[4] = "FileName=C%3A%5CProjects%5CBlueocean%5CLink%5CLinkLibraries%5CLinkManager%5CClients%5CLinkOnline%5CLinkOnline%5CFileadmin%5CClientBackups%5Clinkmanager.xml";
                    args[5] = "BackupEntryPath=ClientBackups%2FClientBackup%5B%40Id%3D%227c384b95-8d92-4147-bd3c-4949bcceb4c7%22%5D";
                }

                // Run through all command line arguments.
                foreach (string arg in args)
                {
                    // Get the name of the argument.
                    string name = arg.Split('=')[0];

                    // Get the value of the argument.
                    string value = HttpUtility.UrlDecode(arg.Split('=')[1]);

                    switch (name)
                    {
                        case "FileName":
                            fileNameClientBackups = value;
                            break;
                        case "BackupEntryPath":
                            clientBackupEntryPath = value;
                            break;
                        case "Database":
                            databaseName = value;
                            break;
                        case "ClientName":
                            clientName = value;
                            break;
                        case "DatabaseProvider":
                            databaseProvider = HttpUtility.UrlDecode(value);
                            break;
                        case "ConnectionString":
                            connectionString = HttpUtility.UrlDecode(value);
                            break;
                    }
                }

                clientUsers = new List<Guid>();

                GetClientUsers();

                // Build the full path to the backup directory name.
                string directoryName = Path.Combine(
                    ConfigurationManager.AppSettings["BackupRoot"],
                    DateTime.Now.ToString("yyyy"),
                    DateTime.Now.ToString("MMM"),
                    clientName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                );

                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["Username"]))
                {
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    // Backup the client's definition files.
                    BackupFileSystem(directoryName);

                    // Backup the client's database.
                    BackupDatabase(Path.Combine(
                        directoryName,
                        clientName + ".bak"
                    ));
                }
                else
                {
                    string error = NetworkShare.ConnectToShare(
                        ConfigurationManager.AppSettings["BackupRoot"],
                        ConfigurationManager.AppSettings["Username"],
                        ConfigurationManager.AppSettings["Password"]
                    );
                    /*using (new NetworkConnection(ConfigurationManager.AppSettings["BackupRoot"], new System.Net.NetworkCredential(
                        ConfigurationManager.AppSettings["Username"],
                        ConfigurationManager.AppSettings["Password"]
                    )))*/
                    {
                        if (!Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        // Backup the client's definition files.
                        BackupFileSystem(directoryName);

                        string tempFile = Path.GetTempFileName() + ".bak";

                        // Backup the client's database.
                        BackupDatabase(tempFile);

                        File.Move(tempFile, Path.Combine(
                            directoryName,
                            clientName + ".bak"
                        ));
                    }

                    NetworkShare.DisconnectFromShare(
                        ConfigurationManager.AppSettings["BackupRoot"],
                        false
                    );
                }

                XmlDocument document = new XmlDocument();
                document.Load(fileNameClientBackups);

                XmlNode xmlNode = document.SelectSingleNode(clientBackupEntryPath);

                xmlNode.Attributes["Status"].Value = "Finished";
                xmlNode.Attributes["FinishDate"].Value = DateTime.Now.ToString();
                xmlNode.Attributes["BackupPath"].Value = directoryName;

                document.Save(fileNameClientBackups);

                string zipPath = Path.Combine(
                    ConfigurationManager.AppSettings["BackupRoot"],
                    "Archives",
                    DateTime.Now.ToString("yyyy"),
                    DateTime.Now.ToString("MMM")
                    );
                string zipArchiveName = Path.Combine(
                   zipPath,
                    clientName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip"
                );

                if (!Directory.Exists(zipPath))
                    Directory.CreateDirectory(zipPath);

                StreamWriter writer = new StreamWriter(zipArchiveName);

                ZipArchive zipArchive = new ZipArchive(writer.BaseStream, ZipArchiveMode.Create);

                CreateEntryFromDirectory(zipArchive, directoryName, false);

                zipArchive.Dispose();
            }
            catch (Exception ex)
            {
                XmlDocument document = new XmlDocument();
                document.Load(fileNameClientBackups);

                XmlNode xmlNode = document.SelectSingleNode(clientBackupEntryPath);

                xmlNode.Attributes["Status"].Value = "Failed";
                xmlNode.Attributes["FinishDate"].Value = DateTime.Now.ToString();
                xmlNode.Attributes["BackupPath"].Value = "";

                xmlNode.InnerXml = ex.ToString();

                document.Save(fileNameClientBackups);
            }
        }

        static void CreateEntryFromDirectory(ZipArchive zipArchive, string directoryPath, bool csRestriction)
        {
            string replaceStr = Path.Combine(
                     ConfigurationManager.AppSettings["BackupRoot"],
                     DateTime.Now.ToString("yyyy"),
                     DateTime.Now.ToString("MMM")
                 );

            string folderName = directoryPath.Replace(replaceStr, "");

            if (folderName != "")
                folderName += "\\";

            foreach (string file in Directory.GetFiles(directoryPath))
            {
                FileInfo fInfo = new FileInfo(file);

                ZipArchiveEntry entry = zipArchive.CreateEntryFromFile(file, folderName + fInfo.Name);
            }

            foreach (string directory in Directory.GetDirectories(directoryPath))
            {

                CreateEntryFromDirectory(zipArchive, directory, csRestriction);
            }
        }


        static void GetClientUsers()
        {
            // Get a new database provider factory.
            DbProviderFactory factory = DbProviderFactories.
                GetFactory(databaseProvider);

            // Create a new database connection.
            DbConnection connection = factory.CreateConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = connectionString;

            // Create a new database command.
            DbCommand command = factory.CreateCommand();

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = string.Format(
                "SELECT {0} FROM {1}",
                "Id",
                "Users"
            );

            connection.Open();

            DbDataReader reader = null;

            try
            {
                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    clientUsers.Add(reader.GetGuid(0));
                }
            }
            finally
            {
                connection.Close();

                command.Dispose();
                connection.Dispose();

                if (reader != null)
                    reader.Dispose();
            }
        }


        static void BackupFileSystem(string directoryName)
        {
            FileSystemBackupDefinitionCollection directories = new FileSystemBackupDefinitionCollection(directoryName);

            string backupRoot = ConfigurationManager.AppSettings["BackupRoot"];

            foreach (FileSystemBackupDefinition directory in directories)
            {
                if (directory.FormatTypes.Contains(FileSystemBackupDefinitionFormatType.User))
                {
                    // Run through all users of the client.
                    foreach (Guid idUser in clientUsers)
                    {
                        directory.Backup(backupRoot, directory.FormatTypes.Select(
                            x => x == FileSystemBackupDefinitionFormatType.Client ?
                            clientName :
                            idUser.ToString()
                        ).ToArray());
                    }
                }
                else
                {
                    directory.Backup(backupRoot, directory.FormatTypes.Select(
                        x => x == FileSystemBackupDefinitionFormatType.Client ?
                        clientName :
                        ""
                    ).ToArray());
                }
            }
        }

        static void BackupDatabase(string path)
        {
            // Get a new database provider factory.
            DbProviderFactory factory = DbProviderFactories.
                GetFactory(databaseProvider);

            // Create a new database connection.
            DbConnection connection = factory.CreateConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = connectionString;

            // Create a new database command.
            DbCommand command = factory.CreateCommand();

            // Set the command's connection.
            command.Connection = connection;

            // Set the command's command text.
            command.CommandText = string.Format(
                "BACKUP DATABASE [{0}] TO  DISK = '{1}'",
                databaseName,
                path
            );

            // Open the database connection.
            connection.Open();

            command.CommandTimeout = 86400;

            try
            {
                // Execute the command on the database.
                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();

                connection.Dispose();
                command.Dispose();
            }
        }
    }
}
