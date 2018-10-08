using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Data.Common;

namespace Folder_structure_migration
{
    class Program
    {
        static void Main(string[] args)
        {
            Log("Press any key to start the conversion.", LogType.Information);

            Console.ReadLine();

            Start();

            Console.WriteLine(Environment.NewLine + "Done.");
            Console.ReadLine();
        }

        static void Start()
        {
            string physicalApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string connectionString = GetConnectionString();

            if (connectionString == null)
            {
                Log("Configuration invalid", LogType.Error);
                return;
            }

            ClientCollection clients = new ClientCollection(Path.Combine(
                physicalApplicationPath,
                "App_Data",
                "Clients.xml"
            ));

            if (clients.Items == null)
            {
                Log("Client definition file not found. Please place this file in the root folder of the web application.", LogType.Error);
                return;
            }

            foreach (string client in clients.Items.Keys)
            {
                Log(string.Format(
                    "Starting conversion of client '{0}'.",
                    client
                ), LogType.Information);

                List<object[]> users;

                try
                {
                    users = ExecuteReader(string.Format(
                        connectionString,
                        clients.Items[client].Database
                    ), string.Format(
                        "SELECT Id FROM Users"
                    ));
                }
                catch
                {
                    Log(string.Format(
                        "Conversion of client '{0}' failed.",
                        client
                    ), LogType.Error);

                    continue;
                }

                Convert(
                    Path.Combine(physicalApplicationPath, "Fileadmin", "ReportDefinitions"),
                    client,
                    users,
                    ConversionType.Directory
                );
                Convert(
                    Path.Combine(physicalApplicationPath, "Fileadmin", "LinkBiDefinitions"),
                    client,
                    users,
                    ConversionType.File,
                    ".xml"
                );
                Convert(
                    Path.Combine(physicalApplicationPath, "Fileadmin", "SavedReports"),
                    client,
                    users,
                    ConversionType.Directory
                );
                Convert(
                    Path.Combine(physicalApplicationPath, "Fileadmin", "HomeDefinitions"),
                    client,
                    users,
                    ConversionType.File,
                    ".xml"
                );

                Log(string.Format(
                    "Conversion of client '{0}' completed successfully.",
                    client
                ), LogType.Success);

                Console.WriteLine();
            }
        }

        static void Convert(
            string directory,
            string clientName,
            List<object[]> users,
            ConversionType type,
            string extension = null
        )
        {
            Log("Converting " + (new DirectoryInfo(directory)).Name, LogType.Information);


            if (!Directory.Exists(Path.Combine(
                directory,
                clientName
            )))
            {
                Directory.CreateDirectory(Path.Combine(
                    directory,
                    clientName
                ));
            }

            // Run through all users of the client.
            foreach (object[] user in users)
            {
                string source = Path.Combine(
                    directory,
                    ((Guid)user[0]).ToString() +
                    (extension != null ? extension : "")
                );

                string destination = Path.Combine(
                    directory,
                    clientName,
                    ((Guid)user[0]).ToString() +
                    (extension != null ? extension : "")
                );

                switch (type)
                {
                    case ConversionType.Directory:
                        if (Directory.Exists(source))
                            Directory.Move(source, destination);
                        break;
                    case ConversionType.File:
                        if (File.Exists(source))
                            File.Move(source, destination);
                        break;
                }
            }
        }

        static void Log(string message, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Information:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }

            Console.WriteLine(string.Format(
                "[{0}]: {1}",
                type,
                message
            ));

            Console.ResetColor();
        }

        static string GetConnectionString()
        {
            string fileName = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "Web.config"
            );

            if (!File.Exists(fileName))
                return null;

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            XmlNode xmlNode = document.SelectSingleNode("configuration/appSettings/add[@key=\"ConnectionString\"]");

            if (xmlNode == null)
                return null;

            return xmlNode.Attributes["value"].Value;
        }

        static List<object[]> ExecuteReader(string connectionString, string query)
        {
            // Get a new database provider factory.
            DbProviderFactory factory = DbProviderFactories.
                GetFactory("System.Data.SqlClient");

            // Create a new database connection.
            DbConnection connection = factory.CreateConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = connectionString;

            // Create a new database command.
            DbCommand command = factory.CreateCommand();

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = query;

            connection.Open();

            DbDataReader reader = null;

            List<object[]> result = new List<object[]>();

            try
            {
                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    object[] value = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        value[i] = reader.GetValue(i);

                        if (value[i] == DBNull.Value)
                            value[i] = null;
                    }

                    result.Add(value);
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

            return result;
        }
    }

    public enum ConversionType
    {
        Directory,
        File
    }

    public class ClientCollection
    {
        #region Properties

        public Dictionary<string, Client> Items { get; private set; }

        #endregion


        #region Constructor

        public ClientCollection(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            this.Items = new Dictionary<string, Client>();

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            foreach (XmlNode xmlNode in document.DocumentElement.ChildNodes)
            {
                Client client = new Client(xmlNode);

                this.Items.Add(client.Name, client);
            }
        }

        #endregion
    }

    public class Client
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the client's database.
        /// </summary>
        public string Database { get; set; }

        #endregion


        #region Constructor

        public Client(XmlNode xmlNode)
        {
            this.Name = xmlNode.Attributes["Name"].Value;
            this.Database = xmlNode.Attributes["Database"].Value;
        }

        #endregion
    }

    public enum LogType
    {
        Error,
        Information,
        Success
    }
}
