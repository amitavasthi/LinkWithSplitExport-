using ApplicationUtilities;
using BenchmarkTest1;
using Crosstables.Classes.ReportDefinitionClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkOnline.Pages
{
    public partial class Debug : WebUtilities.BasePage
    {
        #region Properties

        public BenchmarkTest BenchmarkTest
        {
            get
            {
                if (HttpContext.Current.Session["BenchmarkTest"] == null)
                    return null;

                return (BenchmarkTest)HttpContext.Current.Session["BenchmarkTest"];
            }
            set
            {
                HttpContext.Current.Session["BenchmarkTest"] = value;
            }
        }

        public Dictionary<int, long> BenchmarkTestResults
        {
            get
            {
                if (HttpContext.Current.Session["BenchmarkTestResults"] == null)
                    return null;

                return (Dictionary<int, long>)HttpContext.Current.Session["BenchmarkTestResults"];
            }
            set
            {
                HttpContext.Current.Session["BenchmarkTestResults"] = value;
            }
        }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        public IEnumerable<SessionStateItemCollection> GetActiveSessions()
        {
            object obj;
            object[] obj2 = new object[0];
            try
            {
                obj = typeof(HttpRuntime).GetProperty("CacheInternal", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);
                obj2 = (object[])obj.GetType().GetField("_caches", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
            }
            catch { }
            for (int i = 0; i < obj2.Length; i++)
            {
                Hashtable c2 = (Hashtable)obj2[i].GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj2[i]);
                foreach (DictionaryEntry entry in c2)
                {
                    object o1 = entry.Value.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(entry.Value, null);
                    if (o1.GetType().ToString() == "System.Web.SessionState.InProcSessionState")
                    {
                        SessionStateItemCollection sess = (SessionStateItemCollection)o1.GetType().GetField("_sessionItems", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(o1);
                        if (sess != null)
                        {
                            yield return sess;
                        }
                    }
                }
            }
        }


        private void BindActiveSessions()
        {
            Table table = new Table();
            table.CellSpacing = 0;
            table.CellPadding = 0;

            /*IEnumerable<SessionStateItemCollection> activeSessions = GetActiveSessions();

            foreach (SessionStateItemCollection session in activeSessions)*/
            foreach (string clientName in Global.AllSessions.Keys)
            {
                foreach (Guid idUser in Global.AllSessions[clientName].Keys)
                {

                    string username = "Unknown";

                    if (Global.AllSessions[clientName][idUser]["Core"] != null)
                    {
                        try
                        {
                            DatabaseCore.Core core = (DatabaseCore.Core)Global.AllSessions[clientName][idUser]["Core"];

                            DatabaseCore.Items.User user = core.Users.GetSingle(idUser);

                            username = user.Name;
                        }
                        catch { }
                    }

                    TableRow tableRow = new TableRow();

                    TableCell tableCellClient = new TableCell();
                    TableCell tableCellUser = new TableCell();
                    TableCell tableCellId = new TableCell();

                    tableCellClient.Text = clientName;
                    tableCellUser.Text = username;
                    tableCellId.Text = Global.AllSessions[clientName][idUser].SessionID;

                    tableRow.Cells.Add(tableCellClient);
                    tableRow.Cells.Add(tableCellUser);
                    tableRow.Cells.Add(tableCellId);

                    table.Rows.Add(tableRow);
                }
            }

            pnlActiveSessions.Controls.Add(table);
        }

        private void BindDataAggregationTimes()
        {
            string directoryName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "ReportDefinitions",
                Global.Core.ClientName,
                Global.User.Id.ToString()
            );

            if (!Directory.Exists(directoryName))
                return;

            Table table = new Table();
            table.CellPadding = 5;

            TableRow tableRowHeadline = new TableRow();
            tableRowHeadline.Style.Add("font-weight", "bold");

            TableCell tableCellHeadlineName = new TableCell();
            TableCell tableCellHeadlineValue = new TableCell();
            TableCell tableCellHeadlineValue2 = new TableCell();
            TableCell tableCellHeadlineValue3 = new TableCell();
            TableCell tableCellHeadlineTotal = new TableCell();

            tableCellHeadlineName.Text = "tab";
            tableCellHeadlineValue.Text = "data preperation time";
            tableCellHeadlineValue2.Text = "data aggregation time";
            tableCellHeadlineValue3.Text = "render time";
            tableCellHeadlineTotal.Text = "total";

            tableRowHeadline.Cells.Add(tableCellHeadlineName);
            tableRowHeadline.Cells.Add(tableCellHeadlineValue);
            tableRowHeadline.Cells.Add(tableCellHeadlineValue2);
            tableRowHeadline.Cells.Add(tableCellHeadlineValue3);
            tableRowHeadline.Cells.Add(tableCellHeadlineTotal);

            table.Rows.Add(tableRowHeadline);

            foreach (string fileName in Directory.GetFiles(directoryName))
            {
                FileInfo fInfo = new FileInfo(fileName);

                if (fInfo.Name == "Info.xml")
                    continue;

                TableRow tableRow = new TableRow();

                TableCell tableCellName = new TableCell();
                TableCell tableCellValue = new TableCell();
                TableCell tableCellValue2 = new TableCell();
                TableCell tableCellValue3 = new TableCell();
                TableCell tableCellTotal = new TableCell();

                tableCellTotal.Style.Add("font-weight", "bold");

                try
                {
                    if (File.Exists(fileName))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(fileName);

                        if (xmlDocument.DocumentElement.Attributes["Name"] != null)
                            tableCellName.Text = "<b>" + xmlDocument.DocumentElement.Attributes["Name"].Value + "</b>";

                        if (xmlDocument.DocumentElement.Attributes["DataPreperationTime"] != null)
                            tableCellValue.Text = TimeSpan.Parse(xmlDocument.DocumentElement.Attributes["DataPreperationTime"].Value).ToString("mm':'ss'.'fff");

                        if (xmlDocument.DocumentElement.Attributes["DataCalculationTime"] != null)
                            tableCellValue2.Text = TimeSpan.Parse(xmlDocument.DocumentElement.Attributes["DataCalculationTime"].Value).ToString("mm':'ss'.'fff");

                        if (xmlDocument.DocumentElement.Attributes["DataRenderTime"] != null)
                            tableCellValue3.Text = TimeSpan.Parse(xmlDocument.DocumentElement.Attributes["DataRenderTime"].Value).ToString("mm':'ss'.'fff");

                        if (xmlDocument.DocumentElement.Attributes["DataPreperationTime"] != null &&
                            xmlDocument.DocumentElement.Attributes["DataCalculationTime"] != null &&
                            xmlDocument.DocumentElement.Attributes["DataRenderTime"] != null)
                        {
                            tableCellTotal.Text = (TimeSpan.Parse(xmlDocument.DocumentElement.Attributes["DataCalculationTime"].Value) +
                                TimeSpan.Parse(xmlDocument.DocumentElement.Attributes["DataPreperationTime"].Value) +
                                TimeSpan.Parse(xmlDocument.DocumentElement.Attributes["DataRenderTime"].Value)).ToString("mm':'ss'.'fff");
                        }
                    }
                    else
                    {
                        tableCellValue.Text = "Not available";
                    }
                }
                catch
                {
                    tableCellValue.Text = "Not available";
                }

                tableRow.Cells.Add(tableCellName);
                tableRow.Cells.Add(tableCellValue);
                tableRow.Cells.Add(tableCellValue2);
                tableRow.Cells.Add(tableCellValue3);
                tableRow.Cells.Add(tableCellTotal);

                table.Rows.Add(tableRow);
            }

            pnlDataAggregationTimes.Controls.Add(table);
        }


        private void BindBenchmarkTests()
        {
            if (this.BenchmarkTest == null)
            {
                pnlBenchmarkTestStartNew.Visible = true;
                pnlBenchmarkTestRunning.Visible = false;
                pnlBenchmarkTestResult.Visible = false;
            }
            else if (this.BenchmarkTest.Step < this.BenchmarkTest.Steps)
            {
                pnlBenchmarkTestStartNew.Visible = false;
                pnlBenchmarkTestRunning.Visible = true;
                pnlBenchmarkTestResult.Visible = false;

                if (this.BenchmarkTest.Finished)
                {
                    if (this.BenchmarkTestResults == null)
                        this.BenchmarkTestResults = new Dictionary<int, long>();

                    this.BenchmarkTestResults.Add(
                        this.BenchmarkTest.Step,
                        this.BenchmarkTest.AverageAggregationTime
                    );

                    StartBenchmarkTest(
                        this.BenchmarkTest.Step + 1,
                        this.BenchmarkTest.Steps,
                        this.BenchmarkTest.TotalTests
                    );
                }
            }
            else
            {
                if (this.BenchmarkTestResults == null)
                    this.BenchmarkTestResults = new Dictionary<int, long>();

                if (!this.BenchmarkTestResults.ContainsKey(this.BenchmarkTest.Step))
                {
                    this.BenchmarkTestResults.Add(
                        this.BenchmarkTest.Step,
                        this.BenchmarkTest.AverageAggregationTime
                    );
                }

                pnlBenchmarkTestStartNew.Visible = false;
                pnlBenchmarkTestRunning.Visible = false;
                pnlBenchmarkTestResult.Visible = true;

                // Create chart.


                Table table = new Table();

                foreach (int step in this.BenchmarkTestResults.Keys)
                {
                    TableRow tableRow = new TableRow();

                    TableCell tableCellCount = new TableCell();
                    TableCell tableCellAverage = new TableCell();

                    tableCellCount.Text = ((this.BenchmarkTest.TotalTests / this.BenchmarkTest.Steps) * step).ToString();
                    tableCellAverage.Text = (new TimeSpan(0, 0, 0, 0, (int)this.BenchmarkTestResults[step])).ToString();

                    tableRow.Cells.Add(tableCellCount);
                    tableRow.Cells.Add(tableCellAverage);

                    table.Rows.Add(tableRow);
                }

                pnlBenchmarkTestResult.Controls.Add(table);
            }
        }

        private void StartBenchmarkTest(int step, int steps, int totalTests)
        {
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "ReportDefinitions",
                Global.Core.ClientName,
                Global.User.Id.ToString()
            );

            fileName = Directory.GetFiles(fileName)[0];

            this.BenchmarkTest = new BenchmarkTest(
                totalTests,
                steps,
                step,
                fileName,
                Global.Core
            );

            this.BenchmarkTest.StartTest();
        }


        private void BindHardwareSensors()
        {
            OpenHardwareMonitor.Hardware.Computer computer = new OpenHardwareMonitor.Hardware.Computer();
            computer.CPUEnabled = true;
            computer.RAMEnabled = true;
            computer.MainboardEnabled = true;
            computer.FanControllerEnabled = true;
            computer.HDDEnabled = true;
            computer.Open();

            Table table = new Table();
            table.CellPadding = 0;
            table.CellSpacing = 0;

            TableRow tableRowHeadline = new TableRow();
            tableRowHeadline.CssClass = "HardwareSensorsHeadline";

            TableCell tableCellTitleHeadline = new TableCell();
            tableCellTitleHeadline.Text = "Sensor";

            TableCell tableCellLoadHeadline = new TableCell();
            tableCellLoadHeadline.Text = "Load";

            TableCell tableCellTempHeadline = new TableCell();
            tableCellTempHeadline.Text = "Temp";

            tableRowHeadline.Cells.Add(tableCellTitleHeadline);
            tableRowHeadline.Cells.Add(tableCellLoadHeadline);
            tableRowHeadline.Cells.Add(tableCellTempHeadline);

            table.Rows.Add(tableRowHeadline);

            foreach (OpenHardwareMonitor.Hardware.IHardware hardware in computer.Hardware)
            {
                if (hardware.HardwareType != OpenHardwareMonitor.Hardware.HardwareType.CPU)
                    continue;

                Dictionary<string, int[]> cores = new Dictionary<string, int[]>();

                foreach (OpenHardwareMonitor.Hardware.ISensor sensor in hardware.Sensors)
                {
                    if (!cores.ContainsKey(sensor.Name))
                        cores.Add(sensor.Name, new int[2]);

                    if (sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Load)
                    {
                        try
                        {
                            cores[sensor.Name][0] = (int)sensor.Value.Value;
                        }
                        catch { }
                    }
                    else if (sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Temperature)
                    {
                        try
                        {
                            cores[sensor.Name][1] = (int)sensor.Value.Value;

                            foreach (OpenHardwareMonitor.Hardware.SensorValue v in sensor.Values)
                            {
                                cores[sensor.Name][1] = (int)v.Value;
                            }
                        }
                        catch { }
                    }
                }

                foreach (string core in cores.Keys)
                {
                    TableRow tableRow = new TableRow();

                    TableCell tableCellTitle = new TableCell();
                    tableCellTitle.Text = core;

                    TableCell tableCellLoad = new TableCell();
                    tableCellLoad.Text = cores[core][0].ToString() + "%";

                    TableCell tableCellTemp = new TableCell();
                    tableCellTemp.Text = cores[core][1] != 0 ? cores[core][1].ToString() + "°" : "N/A";

                    tableRow.Cells.Add(tableCellTitle);
                    tableRow.Cells.Add(tableCellLoad);
                    tableRow.Cells.Add(tableCellTemp);

                    table.Rows.Add(tableRow);
                }
            }

            foreach (OpenHardwareMonitor.Hardware.IHardware hardware in computer.Hardware)
            {
                if (hardware.HardwareType == OpenHardwareMonitor.Hardware.HardwareType.CPU)
                    continue;

                Dictionary<string, int[]> cores = new Dictionary<string, int[]>();

                foreach (OpenHardwareMonitor.Hardware.ISensor sensor in hardware.Sensors)
                {
                    TableRow tableRow = new TableRow();

                    TableCell tableCellTitle = new TableCell();
                    tableCellTitle.Text = hardware.Name + " - " + sensor.Name;

                    TableCell tableCellLoad = new TableCell();
                    tableCellLoad.Text = sensor.Value != null ? sensor.Value.ToString() : "N/A";

                    tableRow.Cells.Add(tableCellTitle);
                    tableRow.Cells.Add(tableCellLoad);

                    table.Rows.Add(tableRow);
                }
            }

            pnlHardwareSensors.Controls.Add(table);

            computer.Close();
        }

        private void BindServerInfo()
        {
            try
            {
                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}/Handlers/Public.ashx",
                    ConfigurationManager.AppSettings["LinkAdminHostname"]
                ));

                string server = service.Request(new string[] {
                    "Method=WhoAmI"
                });

                lblServer.Text = server;

                HttpContext.Current.Session["Server"] = server;
            }
            catch (Exception ex)
            {
            }

            try
            {
                lblInstance.Text = new DirectoryInfo(Path.GetDirectoryName(
                    Request.PhysicalApplicationPath
                )).Name;
            }
            catch
            {

            }
        }


        private void BindCaseDataCache()
        {
            StringBuilder result = new StringBuilder();
            result.Append("<table cellspacing=\"0\" cellpadding=\"0\">");

            lock (DataCore.Classes.StorageMethods.Database.Cache)
            {
                foreach (string client in DataCore.Classes.StorageMethods.Database.Cache.Keys)
                {
                    result.Append("<tr><td>");
                    result.Append(client);
                    result.Append("</td><td>");

                    long size = DataCore.Classes.StorageMethods.Database.Cache[client].GetSize();

                    string sizeStr = GetSizeStr(size);

                    result.Append("~ " + sizeStr);

                    result.Append("</td></tr>");
                }
            }

            result.Append("</table>");

            pnlCaseDataCache.Controls.Add(new LiteralControl(result.ToString()));
        }

        private string GetSizeStr(long size, int index = 0)
        {
            string[] suffixes = new string[]
            {
                "B",
                "KB",
                "MB",
                "GB",
                "TB"
            };

            if (index >= suffixes.Length)
                return size.ToString();

            if (size > 1024)
            {
                return GetSizeStr(size / 1024, index + 1);
            }

            return size + " " + suffixes[index];
        }


        private void DeleteDirectory(string directory)
        {
            foreach (string dir in Directory.GetDirectories(directory))
            {
                DeleteDirectory(dir);
            }

            foreach (string file in Directory.GetFiles(directory))
            {
                File.Delete(file);
            }

            Directory.Delete(directory);
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblVersion.Text = System.Reflection.Assembly.
                    GetExecutingAssembly().GetName().Version.ToString();

                lblVersionDataImporter.Text = AssemblyName.GetAssemblyName(
                    ConfigurationManager.AppSettings["DataImportApplication"]
                ).Version.ToString();

                lblVersionAPI.Text = Assembly.GetAssembly(typeof(API.APIHandler))
                    .GetName().Version.ToString();
            }
            catch { }

            try
            {
                lblGenerateResponsesRespondentCount.Text = Global.Core.Respondents.GetCount().ToString();
            }
            catch { }

            try
            {
                BindHardwareSensors();
            }
            catch
            {
                try
                {
                    BindHardwareSensors();
                }
                catch
                { }
            }

            BindServerInfo();
            BindDataAggregationTimes();
            BindActiveSessions();
            BindBenchmarkTests();

            try
            {
                BindCaseDataCache();
            }
            catch { }
        }


        protected void btnCreateResponseIndexes_Click(object sender, EventArgs e)
        {
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "DataStorage",
                "CreateResponseIndex.sql"
            );

            // Select all variable ids from the client's database.
            List<object[]> variables = Global.Core.Variables.GetValues(new string[] { "Id" });

            // Run through all variable ids of the client.
            foreach (object[] variable in variables)
            {
                string script = string.Format(
                    File.ReadAllText(fileName),
                    (Guid)variable[0]
                );

                try
                {
                    Global.Core.Variables.ExecuteQuery(script);
                }
                catch { }
            }
        }

        protected void btnRebuildResponseIndexes_Click(object sender, EventArgs e)
        {
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "DataStorage",
                "RebuildResponseIndex.sql"
            );

            // Select all variable ids from the client's database.
            List<object[]> variables = Global.Core.Variables.GetValues(new string[] { "Id" });

            // Run through all variable ids of the client.
            foreach (object[] variable in variables)
            {
                string script = string.Format(
                    File.ReadAllText(fileName),
                    (Guid)variable[0]
                );

                try
                {
                    Global.Core.Variables.ExecuteQuery(script);
                }
                catch { }
            }
        }


        protected void btnBenchmarkTestStart_Click(object sender, EventArgs e)
        {
            StartBenchmarkTest(
                1,
                int.Parse(txtBenchmarkTestSteps.Text),
                int.Parse(txtBenchmarkTestCount.Text)
            );
        }


        protected void btnGenerateResponsesConfirm_Click(object sender, EventArgs e)
        {
            int difference = int.Parse(txtGenerateResponsesCount.Text) - Global.Core.Respondents.GetCount();

            if (difference <= 0)
                return;

            Random random = new Random();

            Guid[] idStudies = Global.Core.Studies.GetValues(new string[] { "Id" }).Select(x => (Guid)x[0]).ToArray();

            Dictionary<Guid, List<Guid>> respondents = new Dictionary<Guid, List<Guid>>();

            StringBuilder scriptBuilder = new StringBuilder();

            for (int i = 0; i < difference; i++)
            {
                Guid idRespondent = Guid.NewGuid();
                Guid idStudy = idStudies[random.Next(0, idStudies.Length - 1)];

                scriptBuilder.Append(string.Format(
                    "INSERT INTO [Respondents] (Id, IdStudy, OriginalRespondentID) VALUES ('{0}','{1}','{2}');" + Environment.NewLine,
                    idRespondent,
                    idStudy,
                    "LiNK Auto Generated Respondent"
                ));

                if (!respondents.ContainsKey(idStudy))
                    respondents.Add(idStudy, new List<Guid>());

                respondents[idStudy].Add(idRespondent);
            }

            Global.Core.Respondents.ExecuteQuery(scriptBuilder.ToString());

            scriptBuilder.Clear();

            // Run through all studies.
            foreach (Guid idStudy in respondents.Keys)
            {
                // Get all variables from the study.
                List<object[]> variables = Global.Core.Variables.GetValues(
                    new string[] { "Id", "Type" },
                    new string[] { "IdStudy" },
                    new object[] { idStudy }
                );

                // Run through all variables of the study.
                foreach (object[] variable in variables)
                {
                    switch ((DatabaseCore.Items.VariableType)variable[1])
                    {
                        case DatabaseCore.Items.VariableType.Text:
                        case DatabaseCore.Items.VariableType.Numeric:
                            continue;
                    }

                    Guid[] idCategories = Global.Core.Categories.GetValues(
                        new string[] { "Id" },
                        new string[] { "IdVariable" },
                        new object[] { variable[0] }
                    ).Select(x => (Guid)x[0]).ToArray();

                    foreach (Guid idRespondent in respondents[idStudy])
                    {
                        scriptBuilder.Append(string.Format(
                            "INSERT INTO [resp].[Var_{0}] (Id, IdRespondent, IdStudy, IdCategory) VALUES (NEWID(), '{1}','{2}','{3}');" + Environment.NewLine,
                            variable[0],
                            idRespondent,
                            idStudy,
                            idCategories[random.Next(0, idCategories.Length - 1)]
                        ));
                    }

                    Global.Core.Respondents.ExecuteQuery(scriptBuilder.ToString());
                    scriptBuilder.Clear();
                }
            }
        }

        protected void btnDuplicateResponses_Click(object sender, EventArgs e)
        {
            Dictionary<Guid, Dictionary<Guid, Guid>> respondents = new Dictionary<Guid, Dictionary<Guid, Guid>>();

            List<object[]> r = Global.Core.Respondents.GetValues(
                new string[] { "Id", "IdStudy" }
            );

            StringBuilder insertBuilder = new StringBuilder();

            foreach (object[] resondent in r)
            {
                if (!respondents.ContainsKey((Guid)resondent[1]))
                    respondents.Add((Guid)resondent[1], new Dictionary<Guid, Guid>());

                Guid idRespondent = Guid.NewGuid();

                respondents[(Guid)resondent[1]].Add((Guid)resondent[0], idRespondent);

                insertBuilder.Append(string.Format(
                    "INSERT INTO Respondents (Id, IdStudy) VALUES ('{1}', '{0}');",
                    (Guid)resondent[1],
                    idRespondent
                ) + Environment.NewLine);
            }

            Global.Core.Respondents.ExecuteQuery(insertBuilder.ToString());
            insertBuilder.Clear();

            foreach (Guid idStudy in respondents.Keys)
            {
                List<object[]> variables = Global.Core.Variables.GetValues(
                    new string[] { "Id" },
                    new string[] { "IdStudy" },
                    new object[] { idStudy }
                );

                foreach (object[] variable in variables)
                {
                    List<object[]> responses = Global.Core.Responses[(Guid)variable[0]].GetValues(
                        new string[] { "IdRespondent", "IdCategory", "NumericAnswer", "TextAnswer" }
                    );

                    foreach (object[] response in responses)
                    {
                        insertBuilder.Append(string.Format(
                            "INSERT INTO [resp].[Var_{0}] (Id, IdRespondent, IdStudy, IdCategory, NumericAnswer, TextAnswer) " +
                            "VALUES (NEWID(), '{1}', '{2}', {3}, {4}, {5})",
                            variable[0],
                            respondents[idStudy][(Guid)response[0]],
                            idStudy,
                            response[1] != null ? "'" + response[1] + "'" : "NULL",
                            response[2] != null ? "'" + response[2] + "'" : "NULL",
                            response[3] != null ? "'" + response[3] + "'" : "NULL"
                        ) + Environment.NewLine);
                    }

                    Global.Core.Responses[(Guid)variable[0]].ExecuteQuery(insertBuilder.ToString());
                    insertBuilder.Clear();
                }
            }
        }


        protected void btnClearCaseDataCache_Click(object sender, EventArgs e)
        {
            DataCore.Classes.StorageMethods.Database database = new DataCore.Classes.StorageMethods.Database(
                Global.Core,
                null
            );

            database.ClearCaseDataCache();

            Global.Core.ClearCache();
        }

        protected void btnClearVariableCache_Click(object sender, EventArgs e)
        {

            string tempDirectoryName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                "VariableSelector",
                HttpContext.Current.Session.SessionID
            );

            if (!Directory.Exists(tempDirectoryName))
                return;

            DeleteDirectory(tempDirectoryName);
        }

        #endregion
    }
}