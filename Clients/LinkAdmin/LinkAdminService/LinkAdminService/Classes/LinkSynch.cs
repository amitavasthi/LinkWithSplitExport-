using ApplicationUtilities;
using ApplicationUtilities.Classes;
using LinkAdminService.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Web;
using System.Xml;

namespace LinkAdminService.Classes
{
    public class LinkSynch
    {
        #region Properties

        public bool IsSynching { get; set; }

        public int Interval { get; set; }

        public Timer Timer { get; set; }

        public Dictionary<string, bool> ServersSynchStates { get; set; }

        public Dictionary<string, string> FileStates { get; set; }

        public Dictionary<string, DateTime> LatestFileSynchActions { get; set; }

        public Dictionary<string, DateTime> LatestDatabaseSynchActions { get; set; }

        public Dictionary<string, string> Files { get; set; }

        public bool SynchEnabled { get; set; }

        public LinkSynchLog SynchLog { get; set; }

        #endregion


        #region Constructor

        public LinkSynch(int interval)
        {
            this.LatestFileSynchActions = new Dictionary<string, DateTime>();
            this.LatestDatabaseSynchActions = new Dictionary<string, DateTime>();
            this.Interval = interval;
            this.SynchEnabled = bool.Parse(ConfigurationManager.AppSettings["SynchEnabled"]);

            this.Timer = new Timer((double)this.Interval);
            this.Timer.Elapsed += Timer_Elapsed;

            if (this.SynchEnabled)
                this.Timer.Start();
        }

        #endregion


        #region Methods

        public void Synch()
        {
            if (!this.SynchEnabled)
                return;

            this.SynchLog = new Classes.LinkSynchLog();

            this.IsSynching = true;

            try
            {
                this.ServersSynchStates = new Dictionary<string, bool>();

                // Get server state of self.
                bool serverState = GetServerSynchState(ConfigurationManager.AppSettings["ServerIP"]);

                if (!serverState)
                    return;

                //if (this.FileStates == null)
                    this.GetFileSynchStates();

                InstanceCollection instances = new InstanceCollection(
                    ConfigurationManager.AppSettings["InstanceRoot"]
                );

                SynchDatabases();
                SynchFiles(instances);

                CheckDeletedFiles();
            }
            catch (Exception ex)
            {
                Global.Log(ex.ToString(), LogType.Error);
                try
                {
                    // configuration values from the web.config file.
                    MailConfiguration mailConfiguration = new MailConfiguration(true);
                    // Create a new mail by the mail configuration.
                    Mail mail = new Mail(mailConfiguration, "_NONE_")
                    {
                        TemplatePath = Path.Combine(
                            ConfigurationManager.AppSettings["ApplicationPath"],
                            "App_Data",
                            "ErrorMailTemplate.html"
                        ),
                        Subject = "SYNCH ERROR"
                    };

                    mail.Placeholders.Add("Server", ConfigurationManager.AppSettings["ServerIP"]);
                    mail.Placeholders.Add("Exception", ex.ToString());

                    // Send the mail.
                    mail.Send(ConfigurationManager.AppSettings["ErrorMailReciepent"]);
                }
                catch (Exception e)
                {
                    Global.Log(e.ToString(), LogType.Error);
                }
            }

            StringBuilder log = new StringBuilder();

            if (this.SynchLog.Files != 0)
            {
                log.Append(string.Format(
                    "{0} files synched successfully. ",
                    this.SynchLog.Files
                ));
            }
            if (this.SynchLog.FilesFailed != 0)
            {
                log.Append(string.Format(
                    "{0} files failed to synch. ",
                    this.SynchLog.FilesFailed
                ));
            }
            if (this.SynchLog.Queries != 0)
            {
                log.Append(string.Format(
                    "{0} database queries synched successfully. ",
                    this.SynchLog.Queries
                ));
            }
            if (this.SynchLog.QueriesFailed != 0)
            {
                log.Append(string.Format(
                    "{0} database queries failed to synch. ",
                    this.SynchLog.QueriesFailed
                ));
            }

            if (log.Length != 0)
                Global.Log(log.ToString(), LogType.Information);

            log.Clear();

            this.IsSynching = false;
        }

        private void CheckDeletedFiles()
        {
            return;
            string fileName;
            foreach (string file in this.FileStates.Keys)
            {
                fileName = Path.Combine(
                    ConfigurationManager.AppSettings["InstanceRoot"],
                    file.Remove(0, 1)
                );

                InstanceCollection instances = new InstanceCollection(
                    ConfigurationManager.AppSettings["InstanceRoot"]
                );

                List<string> servers = new List<string>();

                foreach (Instance instance in instances.Instances.Values)
                {
                    foreach (Client client in instance.GetClients())
                    {
                        if (client.SynchServers.Length == 0)
                            continue;

                        foreach (string server in client.SynchServers)
                        {
                            // TEST:
                            //string server = "127.0.0.1";

                            if (servers.Contains(server))
                                continue;

                            servers.Add(server);
                        }
                    }
                }

                if (!File.Exists(fileName))
                {
                    if (long.Parse(DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")) - long.Parse(this.FileStates[file]) > 10000)
                    {
                        foreach (string server in servers)
                        {
                            //File.Delete(fileName);
                            ServiceLink service = new ServiceLink(string.Format(
                                "http://{0}:8080/Handler.ashx",
                                server
                            ));

                            string result = service.Request(
                                service.Address + "?Method=DeleteFile&Path=" + file.Remove(0, 1),
                                new byte[0]
                            );
                        }
                    }
                }
            }
        }

        private void SynchDatabases()
        {
            string directoryName = Path.Combine(ConfigurationManager.AppSettings["SynchQueuePath"]);

            foreach (string server in Directory.GetDirectories(directoryName))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(server);

                // TEST ONLY:
                /*if (dirInfo.Name != "127.0.0.1")
                    continue;*/

                /*if (!Directory.Exists(Path.Combine(server, "DATABASE")))
                    continue;*/

                foreach (string synchFile in Directory.GetFiles(server).OrderBy(x => new FileInfo(x).CreationTime))
                {
                    try
                    {
                        // Get server state of target.
                        bool serverState = GetServerSynchState(dirInfo.Name);

                        if (!serverState)
                            continue;

                        XmlDocument document = new XmlDocument();
                        document.Load(synchFile);

                        string databaseName = document.DocumentElement.SelectSingleNode("Database").InnerXml;

                        ServiceLink service = new ServiceLink(string.Format(
                            "http://{0}:8080/Handler.ashx",
                            dirInfo.Name
                        ));

                        try
                        {
                            string result = service.Request(
                                service.Address + "?Method=ExecuteSynch",
                                File.ReadAllBytes(synchFile)
                            );

                            if (bool.Parse(result) == true)
                            {
                                File.Delete(synchFile);

                                if (!this.LatestDatabaseSynchActions.ContainsKey(databaseName.ToLower()))
                                    this.LatestDatabaseSynchActions.Add(databaseName.ToLower(), DateTime.UtcNow);
                                else
                                    this.LatestDatabaseSynchActions[databaseName.ToLower()] = DateTime.UtcNow;

                                this.SynchLog.Queries++;
                            }
                            else
                            {
                                this.SynchLog.QueriesFailed++;
                            }
                        }
                        catch
                        {
                            this.SynchLog.QueriesFailed++;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.SynchLog.QueriesFailed++;
                    }
                }
            }
        }

        private void SynchFiles(InstanceCollection instances)
        {
            string instanceRoot = Path.GetDirectoryName(
                ConfigurationManager.AppSettings["InstanceRoot"]
            );

            XmlDocument document = new XmlDocument();
            document.Load(Path.Combine(
                HttpRuntime.AppDomainAppPath,
                "App_Data",
                "Filesystem.xml"
            ));

            XmlNodeList xmlNodesDirectories = document.DocumentElement.SelectNodes("Directory");
            XmlNodeList xmlNodesFiles = document.DocumentElement.SelectNodes("File");

            // Run through all listed file to check for deleted files.
            foreach (string relativePath in this.Files.Keys)
            {
                string fileName = Path.GetDirectoryName(
                    ConfigurationManager.AppSettings["InstanceRoot"]
                ) + relativePath;

                // Check if the file still exists.
                if (File.Exists(fileName))
                    continue;

                DeleteFile(relativePath, this.Files[relativePath]);
            }

            foreach (Instance instance in instances.Instances.Values)
            {
                foreach (Client client in instance.GetClients())
                {
                    if (client.SynchServers.Length == 0)
                        continue;

                    foreach (string server in client.SynchServers)
                    {
                        // Get server state of target server.
                        bool serverState = GetServerSynchState(server);

                        if (!serverState)
                            continue;

                        foreach (XmlNode xmlNode in xmlNodesDirectories)
                        {
                            string directory = Path.Combine(
                                instanceRoot,
                                instance.Name,
                                string.Format(xmlNode.Attributes["Path"].Value, client.Name)
                            );

                            if (!Directory.Exists(directory))
                                continue;

                            SynchDirectory(server, client.Name, directory);
                        }
                        foreach (XmlNode xmlNode in xmlNodesFiles)
                        {
                            SynchFile(server, client.Name, Path.Combine(instanceRoot, instance.Name, string.Format(
                                xmlNode.Attributes["Path"].Value,
                                client.Name
                            )));
                        }
                    }
                }
            }
        }

        private void SynchDirectory(string server, string client, string directory)
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                SynchFile(server, client, file);
            }
            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                SynchDirectory(server, client, subDirectory);
            }
        }

        private void SynchFile(string server, string client, string path)
        {
            //TEST:
            //server = "127.0.0.1";

            if (!File.Exists(path))
                return;

            FileInfo fInfo = new FileInfo(path);

            string relativePath = path.Replace(Path.GetDirectoryName(
                ConfigurationManager.AppSettings["InstanceRoot"]), "");

            if (this.FileStates.ContainsKey(relativePath) && long.Parse(this.FileStates[relativePath]) >= long.Parse(fInfo.LastWriteTimeUtc.ToString("yyyyMMddHHmmssfff")))
                return;

            try
            {
                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}:8080/Handler.ashx",
                    server
                ));

                string result = service.Request(
                    service.Address + "?Method=SynchFile&Path=" + relativePath + "&LastWriteTime=" +
                    fInfo.LastWriteTimeUtc.ToString("yyyyMMddHHmmssfff"),
                    File.ReadAllBytes(path)
                );

                if (!this.LatestFileSynchActions.ContainsKey(client))
                    this.LatestFileSynchActions.Add(client, DateTime.UtcNow);
                else
                    this.LatestFileSynchActions[client] = DateTime.UtcNow;


                if (!this.FileStates.ContainsKey(relativePath))
                    this.FileStates.Add(relativePath, fInfo.LastWriteTimeUtc.ToString("yyyyMMddHHmmssfff"));
                else
                    this.FileStates[relativePath] = fInfo.LastWriteTimeUtc.ToString("yyyyMMddHHmmssfff");

                this.SynchLog.Files++;
            }
            catch (Exception ex)
            {
                this.SynchLog.FilesFailed++;
            }
        }

        private void DeleteFile(string relativePath, string server)
        {
            try
            {
                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}:8080/Handler.ashx",
                    server
                ));

                string result = service.Request(
                    service.Address + "?Method=DeleteFile&Path=" + relativePath,
                    new byte[0]
                );

                if (this.FileStates.ContainsKey(relativePath))
                    this.FileStates.Remove(relativePath);
                if (this.Files.ContainsKey(relativePath))
                    this.Files.Remove(relativePath);
            }
            catch (Exception ex)
            {
            }
        }


        private bool GetServerSynchState(string ip)
        {

            if (this.ServersSynchStates.ContainsKey(ip))
                return this.ServersSynchStates[ip];

            bool result = false;

            try
            {
                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}/Handlers/Public.ashx",
                    ConfigurationManager.AppSettings["LinkAdminHostname"]
                ));

                // Get server state of target.
                string serverState = service.Request(service.Address +
                    "?Method=GetServerState" + (string.IsNullOrEmpty(ip) ? "" : ("&IP=" + ip)), new byte[0]);

                if (serverState == "Online")
                    result = true;
            }
            catch
            { }

            this.ServersSynchStates.Add(ip, result);

            return this.ServersSynchStates[ip];
        }

        private void GetFileSynchStates()
        {
            this.FileStates = new Dictionary<string, string>();
            this.Files = new Dictionary<string, string>();

            InstanceCollection instances = new InstanceCollection(
                ConfigurationManager.AppSettings["InstanceRoot"]
            );

            List<string> servers = new List<string>();

            foreach (Instance instance in instances.Instances.Values)
            {
                foreach (Client client in instance.GetClients())
                {
                    if (client.SynchServers.Length == 0)
                        continue;

                    foreach (string server in client.SynchServers)
                    {
                        // TEST:
                        //string server = "127.0.0.1";

                        if (servers.Contains(server))
                            continue;

                        servers.Add(server);
                    }
                }
            }

            foreach (string server in servers)
            {
                try
                {
                    ServiceLink service = new ServiceLink(string.Format(
                        "http://{0}:8080/Handler.ashx",
                        server
                    ));

                    string result = service.Request(
                        service.Address + "?Method=GetFileSynchStates",
                        new byte[0]
                    );

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(result);

                    foreach (XmlNode xmlNode in xmlDocument.DocumentElement.SelectNodes("File"))
                    {
                        string path = xmlNode.Attributes["Path"].Value;

                        if (this.FileStates.ContainsKey(path))
                            continue;

                        this.FileStates.Add(path, xmlNode.Attributes["Value"].Value);
                        this.Files.Add(path, server);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        #endregion


        #region Event Handlers

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Timer.Stop();

            this.Synch();

            this.Timer.Start();
        }

        #endregion
    }

    public class LinkSynchLog
    {
        public int Files { get; set; }
        public int FilesFailed { get; set; }

        public int Queries { get; set; }
        public int QueriesFailed { get; set; }
    }
}