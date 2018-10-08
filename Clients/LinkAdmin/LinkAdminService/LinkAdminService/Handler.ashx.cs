using ApplicationUtilities;
using ApplicationUtilities.Classes;
using LinkAdminService.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace LinkAdminService
{
    /// <summary>
    /// Summary description for Handler
    /// </summary>
    public class Handler : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public Handler()
            : base(false, true)
        {
            base.Methods.Add("Ping", Ping);
            base.Methods.Add("GetLog", GetLog);

            base.Methods.Add("ExecuteSynch", ExecuteSynch);
            base.Methods.Add("SynchEnabled", SynchEnabled);
            base.Methods.Add("GetFileSynchStates", GetFileSynchStates);
            base.Methods.Add("SynchFile", SynchFile);
            base.Methods.Add("DeleteFile", DeleteFile);
            base.Methods.Add("GetLatestSynchActions", GetLatestSynchActions);

            base.Methods.Add("GetInstances", GetInstances);
            base.Methods.Add("GetUsers", GetUsers);

            base.Methods.Add("MoveClient", MoveClient);
            base.Methods.Add("CreateClient", CreateClient);
            base.Methods.Add("CreateUser", CreateUser);

            base.Methods.Add("DeployUpdate", DeployUpdate);
            base.Methods.Add("GetAvailableUpdates", GetAvailableUpdates);
        }

        #endregion


        #region Web Methods

        private void Ping(HttpContext context)
        {

        }

        private void GetLog(HttpContext context)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<LogEntries>");

            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Logs",
                DateTime.Today.ToString("yyyyMMdd") + ".txt"
            );

            if (File.Exists(fileName))
                result.Append(File.ReadAllText(fileName));

            result.Append("</LogEntries>");
            context.Response.Write(result.ToString());
        }



        private void ExecuteSynch(HttpContext context)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                context.Request.InputStream.CopyTo(ms);

                XmlDocument document = new XmlDocument();
                document.LoadXml(System.Text.Encoding.UTF8.GetString(ms.ToArray()));

                string databaseName = document.DocumentElement.SelectSingleNode("Database").InnerXml;
                string commandText = document.DocumentElement.SelectSingleNode("Query").InnerText;

                XmlNodeList xmlNodesParameters = document.DocumentElement.SelectNodes("Parameters/Parameter");

                string[] names = new string[xmlNodesParameters.Count];
                object[] values = new object[xmlNodesParameters.Count];

                for (int i = 0; i < xmlNodesParameters.Count; i++)
                {
                    names[i] = xmlNodesParameters[i].Attributes["Name"].Value;

                    switch (xmlNodesParameters[i].Attributes["Type"].Value)
                    {
                        case "Guid":
                            values[i] = Guid.Parse(xmlNodesParameters[i].InnerText);
                            break;
                        case "Int16":
                            values[i] = Int16.Parse(xmlNodesParameters[i].InnerText);
                            break;
                        case "Int32":
                            values[i] = Int32.Parse(xmlNodesParameters[i].InnerText);
                            break;
                        case "Int64":
                            values[i] = Int64.Parse(xmlNodesParameters[i].InnerText);
                            break;
                        case "Double":
                            values[i] = Double.Parse(xmlNodesParameters[i].InnerText);
                            break;
                        case "float":
                            values[i] = float.Parse(xmlNodesParameters[i].InnerText);
                            break;
                        case "DateTime":
                            values[i] = DateTime.Parse(xmlNodesParameters[i].InnerText);
                            break;
                        case "Boolean":
                        case "bool":
                            values[i] = bool.Parse(xmlNodesParameters[i].InnerText);
                            break;
                        default:
                            values[i] = xmlNodesParameters[i].InnerText;
                            break;
                    }
                }

                ExecuteQuery(
                    databaseName,
                    commandText,
                    names,
                    values
                );

                context.Response.Write("true");
            }
            catch (Exception ex)
            {
                try
                {
                    string fileName = Path.Combine(
                        context.Request.PhysicalApplicationPath,
                        "Logs",
                        "Latest.txt"
                    );

                    if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                    File.WriteAllText(fileName, ex.ToString());
                }
                catch { }

                context.Response.Write("false");
            }
        }

        private void SynchEnabled(HttpContext context)
        {
            bool result = Global.Synch.Timer.Enabled;

            if (result == false && Global.Synch.IsSynching)
                result = true;

            context.Response.Write(result.ToString());
        }

        private void GetFileSynchStates(HttpContext context)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<Files>");

            InstanceCollection instances = new InstanceCollection(
                ConfigurationManager.AppSettings["InstanceRoot"]
            );

            XmlDocument document = new XmlDocument();
            document.Load(Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "Filesystem.xml"
            ));

            XmlNodeList xmlNodesDirectories = document.DocumentElement.SelectNodes("Directory");
            XmlNodeList xmlNodesFiles = document.DocumentElement.SelectNodes("File");

            foreach (Instance instance in instances.Instances.Values)
            {
                foreach (XmlNode xmlNode in xmlNodesDirectories)
                {
                    string directory = Path.Combine(
                        instance.Name,
                        string.Format(xmlNode.Attributes["Path"].Value, "")
                    );

                    if (!Directory.Exists(Path.Combine(instances.Source, directory)))
                        continue;

                    foreach (Client client in instance.GetClients())
                    {
                        if (client.SynchServers.Length == 0)
                            continue;

                        GetDirectorySynchState(instances.Source, Path.Combine(instances.Source, directory), result);
                    }
                }
                foreach (XmlNode xmlNode in xmlNodesFiles)
                {
                    foreach (Client client in instance.GetClients())
                    {
                        if (client.SynchServers.Length == 0)
                            continue;

                        string directory = Path.GetDirectoryName(Path.Combine(
                            instance.Name,
                            string.Format(xmlNode.Attributes["Path"].Value, client.Name)
                        ));

                        if (!Directory.Exists(Path.Combine(instances.Source, directory)))
                            continue;

                        GetDirectorySynchState(instances.Source, Path.Combine(instances.Source, directory), result);
                    }
                }

            }

            result.Append("</Files>");

            context.Response.Write(result.ToString());
        }

        private void SynchFile(HttpContext context)
        {
            string fileName = Path.GetDirectoryName(
                ConfigurationManager.AppSettings["InstanceRoot"]
            ) + context.Request.Params["Path"];

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            string relativePath = fileName.Replace(Path.GetDirectoryName(
                ConfigurationManager.AppSettings["InstanceRoot"]), "");

            MemoryStream ms = new MemoryStream();
            context.Request.InputStream.CopyTo(ms);

            File.WriteAllBytes(fileName, ms.ToArray());

            FileInfo fInfo = new FileInfo(fileName);

            if (!Global.Synch.FileStates.ContainsKey(relativePath))
                Global.Synch.FileStates.Add(relativePath, context.Request.Params["LastWriteTime"]);
            else
                Global.Synch.FileStates[relativePath] = context.Request.Params["LastWriteTime"];
        }

        private void DeleteFile(HttpContext context)
        {
            string fileName = Path.GetDirectoryName(
                ConfigurationManager.AppSettings["InstanceRoot"]
            ) + context.Request.Params["Path"];

            string relativePath = context.Request.Params["Path"];

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                return;

            if (!File.Exists(fileName))
                return;

            File.Delete(fileName);

            if (Global.Synch.FileStates.ContainsKey(relativePath))
                Global.Synch.FileStates.Remove(relativePath);

            if (Global.Synch.Files.ContainsKey(relativePath))
                Global.Synch.Files.Remove(relativePath);
        }

        private void GetLatestSynchActions(HttpContext context)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<LatestSynchs>");

            foreach (string client in Global.Synch.LatestFileSynchActions.Keys)
            {
                result.Append(string.Format(
                    "<Synch Client=\"{0}\" Filesystem=\"{1}\" Database=\"{2}\" />",
                    client,
                    Global.Synch.LatestFileSynchActions[client].ToString(new CultureInfo("en-GB")),
                    Global.Synch.LatestDatabaseSynchActions.ContainsKey(client.ToLower()) ?
                    Global.Synch.LatestDatabaseSynchActions[client.ToLower()].ToString(new CultureInfo("en-GB")) : ""
                ));
            }
            foreach (string client in Global.Synch.LatestDatabaseSynchActions.Keys)
            {
                if (Global.Synch.LatestFileSynchActions.ContainsKey(client))
                    continue;

                result.Append(string.Format(
                    "<Synch Client=\"{0}\" Filesystem=\"{1}\" Database=\"{2}\" />",
                    client,
                    "",
                    Global.Synch.LatestDatabaseSynchActions[client.ToLower()].ToString(new CultureInfo("en-GB"))
                ));
            }

            result.Append("</LatestSynchs>");

            context.Response.Write(result.ToString());
        }


        private void GetInstances(HttpContext context)
        {
            InstanceCollection instances = new InstanceCollection(
                ConfigurationManager.AppSettings["InstanceRoot"]
            );

            context.Response.Write(instances.ToXml());
        }

        private void GetUsers(HttpContext context)
        {
            // Get the instance name from
            // the http request's parameters.
            string instanceName = context.Request.Params["Instance"];

            // Get the client name from
            // the http request's parameters.
            string clientName = context.Request.Params["Client"];

            ClientManager clientManager = new ClientManager(Path.Combine(
                ConfigurationManager.AppSettings["InstanceRoot"],
                instanceName,
                "App_Data",
                "Clients.xml"
            ));

            string databaseName = clientManager.GetDatabaseName(clientName);

            List<object[]> users = ExecuteReader(
                databaseName,
                "SELECT Name, FirstName, LastName, Mail FROM [Users]"
            );

            StringBuilder result = new StringBuilder();
            result.Append("<Users>");

            foreach (object[] user in users)
            {
                result.Append(string.Format(
                    "<User Name=\"{0}\" FirstName=\"{1}\" LastName=\"{2}\" Mail=\"{3}\"></User>",
                    user[0],
                    user[1],
                    user[2],
                    user[3]
                ));
            }

            result.Append("</Users>");

            context.Response.Write(result.ToString());
        }


        private void MoveClient(HttpContext context)
        {
            // Get the client name from
            // the http request's parameters.
            string clientName = context.Request.Params["Client"];

            // Get the source instance name from
            // the http request's parameters.
            string instanceNameSource = context.Request.Params["SourceInstance"];

            // Get the target instance name from
            // the http request's parameters.
            string instanceNameTarget = context.Request.Params["TargetInstance"];

            // Move the website binding.
            MoveClientBinding(
                instanceNameSource,
                instanceNameTarget,
                clientName
            );

            // Move the client definition from the
            // instance's Clients.xml file.
            MoveClientDefinition(
                instanceNameSource,
                instanceNameTarget,
                clientName
            );

            // Move the client's files.
            MoveClientFileSystem(
                instanceNameSource,
                instanceNameTarget,
                clientName
            );
        }

        private void CreateClient(HttpContext context)
        {
            // Get the client name from
            // the http request's parameters.
            string clientName = context.Request.Params["Client"];

            // Get the source instance name from
            // the http request's parameters.
            string instanceName = context.Request.Params["Instance"];

            string hostname = string.Format(
                ConfigurationManager.AppSettings["Hostname"],
                clientName
            );

            InstanceCollection instances = new InstanceCollection(
                ConfigurationManager.AppSettings["InstanceRoot"]
            );

            if (!instances.Instances.ContainsKey(instanceName))
            {
                context.Response.Write("__ERROR__Instance does not exist on the server.");
                return;
            }

            Instance instance = instances.Instances[instanceName];

            ClientManager clientManager = new ClientManager(Path.Combine(
                ConfigurationManager.AppSettings["InstanceRoot"],
                instanceName,
                "App_Data",
                "Clients.xml"
            ));

            clientManager.Append(
                clientName.ToLower(),
                clientName,
                hostname,
                "#6CAEE0",
                "#FCB040",
                DateTime.Today.ToString("dd-MM-yyyy"),
                context.Request.Params["Servers"]
            );

            InstanceVersion version = new InstanceVersion(instance.Version);

            string databaseScriptsDirectory = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "ClientCreation",
                version.ToString(),
                "DATABASE"
            );

            while (!Directory.Exists(databaseScriptsDirectory))
            {
                version -= 1;

                if (version.ToString() == "0.0.0.0")
                    return;

                databaseScriptsDirectory = Path.Combine(
                    context.Request.PhysicalApplicationPath,
                    "App_Data",
                    "ClientCreation",
                    version.ToString(),
                    "DATABASE"
                );
            }

            string databaseRoot = ConfigurationManager.AppSettings["DatabaseRoot"];

            int i = 0;
            foreach (string scriptFile in Directory.GetFiles(databaseScriptsDirectory))
            {
                string script = File.ReadAllText(scriptFile);
                script = script.Replace("###CLIENTNAME###", clientName);
                script = script.Replace("###DATABASEROOT###", databaseRoot);

                ExecuteQuery(
                    i == 0 ? "master" : clientName,
                    script
                );

                i++;
            }

            string fileSystemRoot = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "ClientCreation",
                version.ToString(),
                "FILESYSTEM"
            );

            foreach (string directory in Directory.GetDirectories(fileSystemRoot))
            {
                string target = directory.Replace(
                    fileSystemRoot, Path.Combine(
                    ConfigurationManager.AppSettings["InstanceRoot"],
                    instanceName
                ));

                CopyDirectory(
                    directory,
                    target,
                    clientName
                );
            }

            try
            {
                IISBindings iisBindings = new IISBindings();
                iisBindings.AddBindings(instanceName, hostname);
            }
            catch { }

            string caseDataDirectory = Path.Combine(
                ConfigurationManager.AppSettings["CaseDataPath"],
                clientName
            );

            if (!Directory.Exists(caseDataDirectory))
                Directory.CreateDirectory(caseDataDirectory);

            ExecuteQuery(clientName,
                "INSERT INTO [Roles] (Id, CreationDate, Name, Description) " +
                "VALUES ({0}, {1}, {2}, {3})",
                Guid.Parse("00000000-0000-0000-0000-000000000000"),
                DateTime.Now,
                "Administrators",
                "Blueocean administrators"
            );

            XmlDocument document = new XmlDocument();
            document.Load(Path.Combine(
                ConfigurationManager.AppSettings["InstanceRoot"],
                instanceName,
                "App_Data",
                "Permissions.xml"
            ));

            foreach (XmlNode xmlNodePermission in document.SelectNodes("//Permission"))
            {
                ExecuteQuery(clientName,
                    "INSERT INTO [RolePermissions] (Id, IdRole, Permission, CreationDate) " +
                    "VALUES (NEWID(), {0}, {1}, {2})",
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    int.Parse(xmlNodePermission.Attributes["Id"].Value),
                    DateTime.Now
                );
            }
        }

        private void CreateUser(HttpContext context)
        {
            // Get the client name from
            // the http request's parameters.
            string clientName = context.Request.Params["Client"];

            // Get the source instance name from
            // the http request's parameters.
            string instanceName = context.Request.Params["Instance"];

            InstanceCollection instances = new InstanceCollection(
                ConfigurationManager.AppSettings["InstanceRoot"]
            );

            if (!instances.Instances.ContainsKey(instanceName))
            {
                context.Response.Write("__ERROR__Instance does not exist on the server.");
                return;
            }

            Instance instance = instances.Instances[instanceName];

            ClientManager clientManager = new ClientManager(Path.Combine(
                ConfigurationManager.AppSettings["InstanceRoot"],
                instanceName,
                "App_Data",
                "Clients.xml"
            ));

            Client client = clientManager.GetSingle(clientName);

            if (client == null)
            {
                context.Response.Write("__ERROR__Client does not exist on this instance.");
                return;
            }

            ExecuteQuery(client.Database,
                "INSERT INTO [Users] (Id, CreationDate, Name, FirstName, LastName, Mail, Password, Language) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
                context.Request.Params["Id"],
                DateTime.Now,
                context.Request.Params["Name"],
                context.Request.Params["FirstName"],
                context.Request.Params["LastName"],
                context.Request.Params["Mail"],
                context.Request.Params["Password"],
                "English"
            );

            ExecuteQuery(client.Database,
                "INSERT INTO [UserRoles] (Id, IdUser, IdRole, CreationDate) " +
                "VALUES (NEWID(), {0}, {1}, {2})",
                context.Request.Params["Id"],
                context.Request.Params["IdRole"],
                DateTime.Now
            );
        }


        private void DeployUpdate(HttpContext context)
        {
            context.Response.Write("<Errors>");

            // Get the source instance name from
            // the http request's parameters.
            string instanceName = context.Request.Params["Instance"];

            InstanceVersion toVersion = new InstanceVersion(context.Request.Params["Version"]);

            string instanceRoot = ConfigurationManager.AppSettings["InstanceRoot"];

            InstanceCollection instances = new InstanceCollection(instanceRoot);

            if (!instances.Instances.ContainsKey(instanceName))
                return;

            InstanceVersion version = new InstanceVersion(instances.Instances[instanceName].Version);
            version += 1;

            while (version.ToInt() <= toVersion.ToInt())
            {
                string directory = Path.Combine(
                    context.Request.PhysicalApplicationPath,
                    "App_Data",
                    "SoftwareUpdate",
                    version.ToString()
                );

                if (!Directory.Exists(directory))
                    continue;

                string fileArchive = Path.Combine(
                    directory,
                    "Files.zip"
                );

                if (File.Exists(fileArchive))
                {
                    try
                    {

                        string tempDirectory = Path.Combine(
                            Path.GetTempPath(),
                            Guid.NewGuid().ToString()
                        );

                        Directory.CreateDirectory(tempDirectory);

                        System.IO.Compression.ZipFile.ExtractToDirectory(
                            fileArchive,
                            tempDirectory
                        );

                        DeployDirectory(tempDirectory, Path.Combine(
                            instanceRoot,
                            instanceName
                        ), version.ToString());
                    }
                    catch (Exception ex)
                    {
                        context.Response.Write(string.Format(
                            "<Error Type=\"Software\" Version=\"{1}\">{0}</Error>",
                            ex.ToString(),
                            version.ToString()
                        ));
                    }
                }

                string databaseScripts = Path.Combine(
                    directory,
                    "DATABASE"
                );

                if (!Directory.Exists(databaseScripts))
                {
                    version += 1;
                    continue;
                }

                Client[] clients = instances.Instances[instanceName].GetClients();

                foreach (Client client in clients)
                {
                    foreach (string script in Directory.GetFiles(databaseScripts))
                    {
                        try
                        {
                            ExecuteQuery(client.Database, File.ReadAllText(script));
                        }
                        catch (Exception ex)
                        {
                            context.Response.Write(string.Format(
                                "<Error Type=\"Database\" Version=\"{1}\" Client=\"{2}\">{0}</Error>",
                                ex.ToString(),
                                version.ToString(),
                                client.Name
                            ));
                        }
                    }
                }

                version += 1;
            }

            context.Response.Write("</Errors>");
        }

        private void GetAvailableUpdates(HttpContext context)
        {
            string directory = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "SoftwareUpdate"
            );

            context.Response.Write("<Updates>");

            foreach (string update in Directory.GetDirectories(directory))
            {
                context.Response.Write(string.Format(
                    "<Update Version=\"{0}\"/>",
                    new DirectoryInfo(update).Name
                ));
            }

            context.Response.Write("</Updates>");
        }


        #endregion


        #region Methods

        private string DeployDirectory(string source, string destination, string version)
        {
            StringBuilder result = new StringBuilder();

            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            foreach (string directory in Directory.GetDirectories(source))
            {
                result.Append(DeployDirectory(directory, Path.Combine(
                    destination,
                    new DirectoryInfo(directory).Name
                ), version));
            }

            foreach (string file in Directory.GetFiles(source))
            {
                try
                {
                    File.Copy(file, Path.Combine(
                        destination,
                         new FileInfo(file).Name
                    ), true);
                }
                catch (Exception ex)
                {
                    result.Append(string.Format(
                        "<Error Type=\"Software\" Version=\"{1}\">{0}</Error>",
                        ex.ToString(),
                        version.ToString()
                    ));
                }
            }

            return result.ToString();
        }

        private void GetDirectorySynchState(string instanceRoot, string directory, StringBuilder result)
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                result.Append(string.Format(
                    "<File Path=\"{0}\" Value=\"{1}\" />",
                    file.Replace(instanceRoot, ""),
                    new FileInfo(file).LastWriteTimeUtc.ToString("yyyyMMddHHmmssfff")
                ));
            }
            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                GetDirectorySynchState(instanceRoot, subDirectory, result);
            }
        }

        private void CopyDirectory(string source, string target, string clientName)
        {
            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);

            foreach (string directory in Directory.GetDirectories(source))
            {
                CopyDirectory(directory, Path.Combine(
                    target,
                    new DirectoryInfo(directory).Name
                ), clientName);
            }

            foreach (string file in Directory.GetFiles(source))
            {
                File.Copy(
                    file, Path.Combine(
                    target,
                    new FileInfo(file).Name.Replace("###CLIENTNAME###", clientName)
                ), true);
            }
        }


        public List<object[]> ExecuteReader(string databaseName, string query, params object[] parameters)
        {
            // Get a new database provider factory.
            DbProviderFactory factory = DbProviderFactories.
                GetFactory(ConfigurationManager.AppSettings["DatabaseProvider"]);

            // Create a new database connection.
            DbConnection connection = factory.CreateConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = string.Format(
                ConfigurationManager.AppSettings["ConnectionString"],
                databaseName
            );

            // Create a new database command.
            DbCommand command = factory.CreateCommand();

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = query;

            for (int i = 0; i < parameters.Length; i++)
            {
                command.CommandText = command.CommandText.Replace("{" + i + "}", "@parameter" + i);

                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = "parameter" + i;
                parameter.Value = parameters[i];

                command.Parameters.Add(parameter);
            }

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

        public void ExecuteQuery(string databaseName, string query, params object[] parameters)
        {
            // Get a new database provider factory.
            DbProviderFactory factory = DbProviderFactories.
                GetFactory(ConfigurationManager.AppSettings["DatabaseProvider"]);

            // Create a new database connection.
            DbConnection connection = factory.CreateConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = string.Format(
                ConfigurationManager.AppSettings["ConnectionString"],
                databaseName
            );

            // Create a new database command.
            DbCommand command = factory.CreateCommand();

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = query;

            for (int i = 0; i < parameters.Length; i++)
            {
                command.CommandText = command.CommandText.Replace("{" + i + "}", "@parameter" + i);

                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = "parameter" + i;
                parameter.Value = parameters[i];

                command.Parameters.Add(parameter);
            }

            connection.Open();

            try
            {
                // Execute a database reader on the command.
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();

                command.Dispose();
                connection.Dispose();
            }
        }
        public void ExecuteQuery(string databaseName, string query, string[] names, object[] values)
        {
            // Get a new database provider factory.
            DbProviderFactory factory = DbProviderFactories.
                GetFactory(ConfigurationManager.AppSettings["DatabaseProvider"]);

            // Create a new database connection.
            DbConnection connection = factory.CreateConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = string.Format(
                ConfigurationManager.AppSettings["ConnectionString"],
                databaseName
            );

            // Create a new database command.
            DbCommand command = factory.CreateCommand();

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = query;

            for (int i = 0; i < names.Length; i++)
            {
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = names[i];
                parameter.Value = values[i];

                command.Parameters.Add(parameter);
            }

            connection.Open();

            try
            {
                // Execute a database reader on the command.
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();

                command.Dispose();
                connection.Dispose();
            }
        }

        private void MoveClientDefinition(
            string instanceNameSource,
            string instanceNameTarget,
            string clientName
        )
        {
            ClientManager clientManagerSource = new ClientManager(Path.Combine(
                ConfigurationManager.AppSettings["InstanceRoot"],
                instanceNameSource,
                "App_Data",
                "Clients.xml"
            ));

            ClientManager clientManagerTarget = new ClientManager(Path.Combine(
                ConfigurationManager.AppSettings["InstanceRoot"],
                instanceNameTarget,
                "App_Data",
                "Clients.xml"
            ));

            if (clientManagerTarget.GetSingle(clientName) != null)
            {
                clientManagerTarget.Remove(clientName);
            }

            Client client = clientManagerSource.GetSingle(clientName);
            clientManagerTarget.Append(
                client.Name,
                client.Database,
                client.Host,
                client.Color1,
                client.Color2,
                client.XmlNode.Attributes["CreatedDate"].Value,
                string.Join(",", client.SynchServers)
            );

            clientManagerSource.Remove(clientName);

        }

        private void MoveClientFileSystem(
            string instanceNameSource,
            string instanceNameTarget,
            string clientName
        )
        {
            List<PortalDirectory> directories = new List<PortalDirectory>();

            XmlDocument document = new XmlDocument();
            document.Load(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "PortalDirectories.xml"
            ));

            ClientManager clientManager = new ClientManager(Path.Combine(
                ConfigurationManager.AppSettings["InstanceRoot"],
                instanceNameSource,
                "App_Data",
                "Clients.xml"
            ));

            string databaseName = clientManager.GetDatabaseName(clientName);

            List<object[]> users = ExecuteReader(
                databaseName,
                "SELECT Name, FirstName, LastName, Mail FROM [Users]"
            );

            foreach (XmlNode xmlNodeDirectory in document.DocumentElement.SelectNodes("Directory"))
            {
                PortalDirectory directory = new PortalDirectory(xmlNodeDirectory);

                string path = directory.Path;

                if (directory.FormatTypes.Contains(PortalDirectoryFormatType.User))
                {
                    // Run through all users of the client.
                    foreach (object[] user in users)
                    {
                        path = string.Format(path, directory.FormatTypes.Select(x =>
                            x == PortalDirectoryFormatType.Client ?
                            clientName :
                            user[0].ToString()
                        ));
                    }
                }
                else
                {
                    path = string.Format(path, clientName);
                }

                Directory.Move(Path.Combine(
                    ConfigurationManager.AppSettings["InstanceRoot"],
                    instanceNameSource,
                    path
                ), Path.Combine(
                    ConfigurationManager.AppSettings["InstanceRoot"],
                    instanceNameTarget,
                    path
                ));
            }
        }

        private void MoveClientBinding(
            string instanceNameSource,
            string instanceNameTarget,
            string clientName
        )
        {
            IISBindings bindings = new IISBindings();

            ClientManager clientManager = new ClientManager(Path.Combine(
                ConfigurationManager.AppSettings["InstanceRoot"],
                instanceNameSource,
                "App_Data",
                "Clients.xml"
            ));

            Client client = clientManager.GetSingle(clientName);

            bindings.RemoveBindings(
                instanceNameSource,
                client.Host
            );

            bindings.AddBindings(
                instanceNameTarget,
                client.Host
            );
        }

        #endregion
    }
}