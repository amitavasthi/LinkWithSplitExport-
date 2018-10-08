using DataAnalyser1.Classes;
using DatabaseCore;
using DatabaseCore.Items;
using DataInterface.BaseClasses;
using DataInterface.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using WebUtilities.Controls;

namespace LinkOnline.Pages.DataManagement
{
    public partial class Overview : WebUtilities.BasePage
    {
        #region Properties

        TreeView treeView;

        public DataUpload DataUpload
        {
            get
            {
                if (HttpContext.Current.Session["DataUpload"] == null)
                    this.DataUpload = new DataUpload();

                return (DataUpload)HttpContext.Current.Session["DataUpload"];
            }
            set
            {
                HttpContext.Current.Session["DataUpload"] = value;
            }
        }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void BindLanguages()
        {
            // Get all available languages.
            CultureInfo[] languages = Global.GetCultures();

            // Run through all languages.
            foreach (CultureInfo language in languages)
            {
                // Create a new list item for the language.
                System.Web.UI.WebControls.ListItem lItem = new System.Web.UI.WebControls.ListItem();
                lItem.Text = language.DisplayName;
                lItem.Value = language.LCID.ToString();

                ddlUploadStudyLanguage.Items.Add(lItem);
            }

            ddlUploadStudyLanguage.SelectedValue = "2057";
        }

        private void BindHierarchies()
        {
            // Get all hierarchies on root level.
            List<object[]> hierarchies = Global.Core.Hierarchies.GetValues(
                new string[] { "Id", "Name" },
                new string[] { "IdHierarchy" },
                new object[] { null }
            );

            treeView = new TreeView("tvHierarchies");

            // Run through all hierarchies on root level.
            foreach (object[] hierarchy in hierarchies)
            {
                TreeViewNode node = RenderHierarchyTreeViewNode(
                    treeView,
                    hierarchy,
                    ""
                );
                node.Attributes.Add("id", "tnHierarchy" + hierarchy[0]);

                treeView.Nodes.Add(node);

                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "LoadHierarchyVariables" + hierarchy[0],
                    "LoadStudies(document.getElementById('cphContent__tvnHierarchy" + hierarchy[0] + "'), '" + hierarchy[0] + "');",
                    true
                );
            }

            pnlHierarchies.Controls.Add(treeView);
        }

        private void BindWorkgroups()
        {
            // Get all workgroups assigned to the user.
            List<object[]> workgroups = Global.Core.Workgroups.ExecuteReader(
                "SELECT Id, Name FROM Workgroups WHERE Id IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser={0})",
                new object[] { Global.IdUser.Value }
            );

            // Run through all workgroups assigned to the user.
            foreach (object[] workgroup in workgroups)
            {
                ddlInsertHierarchyWorkgroup.Items.Add(new System.Web.UI.WebControls.ListItem(
                    (string)workgroup[1],
                    workgroup[0].ToString()
                ));
            }
        }


        private TreeViewNode RenderHierarchyTreeViewNode(TreeView treeView, object[] hierarchy, string path)
        {
            path += "/" + (string)hierarchy[1];

            // Create a new tree view node for
            // the hierarchy on root level.
            TreeViewNode node = new TreeViewNode(treeView, "tvnHierarchy" + hierarchy[0].ToString(), "");
            node.CssClass = "BackgroundColor5";
            node.Label = string.Format(
                "<div id=\"lblHierarchyName{1}\">{0}</div>",
                (string)hierarchy[1],
                hierarchy[0]
            );
            node.Buttons.Add(new LiteralControl(string.Format(
                "<div class=\"ImageInsertHierarchy\"><img src=\"/Images/Icons/Add2.png\" onclick=\"insertHierarchyIdParent='{0}';InitDragBox('boxInsertHierarchyControl');\" /></div>",
                hierarchy[0]
            )));

            node.Attributes.Add(
                "Path",
                path
            );
            node.OnClientClick = string.Format(
                "LoadStudies(this, '{0}');",
                hierarchy[0]
            );
            node.OnContextMenu = string.Format(
                "EditHierarchy('{0}');return false;",
                hierarchy[0]
            );

            // Get all hierarchies of the workgroups where the user
            // is assigned to where the hierarchy is the parent.
            List<object[]> childHierarchies = Global.Core.Hierarchies.ExecuteReader(string.Format(
                "SELECT Id, Name FROM [Hierarchies] WHERE IdHierarchy='{1}' AND Id IN (SELECT IdHierarchy FROM WorkgroupHierarchies " +
                "WHERE IdWorkgroup IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser='{0}')) ORDER BY Name",
                Global.IdUser.Value,
                hierarchy[0]
            ));

            // Run through all available child hierarchies of the hierarchy.
            foreach (object[] childHierarchy in childHierarchies)
            {
                node.AddChild(RenderHierarchyTreeViewNode(
                    treeView,
                    childHierarchy,
                    path
                ));
            }

            /*TreeViewNode nodeAdd = new TreeViewNode(treeView, "add_hierarchy" + hierarchy[0], "");
            nodeAdd.Label = "<img src=\"/Images/Icons/Add2.png\" />";

            node.AddChild(nodeAdd);*/

            return node;
        }


        private void BindStudies()
        {
            // Create a new string builder that
            // stores the result JSON script.
            StringBuilder result = new StringBuilder();

            // Open the array that stores the studies.
            result.Append("[");

            // Parse the id of the hierarchy, where to load the
            // studies of, from the http request's parameters.
            Guid idHierarchy = Guid.Parse(Request.Params["IdHierarchy"]);

            // Get all studies assigned to this hierarchy.
            List<object[]> studies = Global.Core.Studies.GetValues(
                new string[] { "Id", "Name", "Status", "CreationDate" },
                new string[] { "IdHierarchy" },
                new object[] { idHierarchy }
            );

            // Run through all studies assigned to this hierarchy.
            foreach (object[] study in studies)
            {
                string fileNameStatus = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "RunningImports",
                    study[0].ToString()
                );

                string status;
                string additional = "";

                if (File.Exists(fileNameStatus))
                {
                    status = "StudyStatusImporting";

                    string[] runningImport = File.ReadAllText(fileNameStatus).Split('|');
                    double progress = 0;

                    DateTime responseInsertStarted;

                    if (runningImport.Length > 1)
                        progress = double.Parse(runningImport[1]);

                    additional = "DataUploaderStatus" + runningImport[0] + "|";
                    additional += (int)progress;

                    if (runningImport[0] == "Step6" && progress != 0)
                    {
                        if (runningImport.Length > 2)
                        {
                            if (!DateTime.TryParse(runningImport[2], out responseInsertStarted))
                                responseInsertStarted = (DateTime)study[3];
                        }
                        else
                            responseInsertStarted = (DateTime)study[3];

                        TimeSpan duration = DateTime.Now - responseInsertStarted;

                        long etaSeconds = ((long)(duration.TotalSeconds / progress)) * 100;
                        etaSeconds -= (long)duration.TotalSeconds;

                        TimeSpan eta = DateTime.Now.AddSeconds(etaSeconds) - DateTime.Now;

                        string etaStr = "";

                        if (eta.Days != 0)
                            etaStr += eta.Days + "d ";

                        if (eta.Hours != 0)
                            etaStr += eta.Hours + "h ";

                        if (eta.Minutes != 0)
                            etaStr += eta.Minutes + "m ";

                        if (etaStr == "" || eta.TotalSeconds <= 0)
                            etaStr = "<1m";

                        additional += "|" + etaStr;
                    }
                }
                else if ((int)Global.Core.CategoryLinks.ExecuteReader(
                    "SELECT Count(*) FROM CategoryLinks WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy={0})",
                    new object[] { study[0] }
                )[0][0] == 0)
                {
                    status = "StudyStatusUnlinked";
                }
                else if ((int)Global.Core.CategoryLinks.ExecuteReader(
                    "SELECT Count(*) FROM CategoryLinks WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy={0}) AND QA=0",
                    new object[] { study[0] }
                )[0][0] != 0)
                {
                    status = "StudyStatusQA";
                }
                else
                {
                    status = "StudyStatusOnline";
                }

                result.Append(base.ToJson(
                    new string[] { "Id", "Name", "Status", "Additional" },
                    new string[] { study[0].ToString(), (string)study[1], status, additional }
                ));

                result.Append(",");
            }

            if (studies.Count > 0)
                result = result.Remove(result.Length - 1, 1);

            // Open the array that stores the studies.
            result.Append("]");

            Response.Write(result.ToString());
        }

        private void GetStudyUploadStatus()
        {
            Guid idStudy = Guid.Parse(Request.Params["IdStudy"]);

            string fileNameStatus = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "RunningImports",
                    idStudy.ToString()
                );

            string status;
            string additional = "";

            if (!File.Exists(fileNameStatus))
                return;

            status = "StudyStatusImporting";

            string[] runningImport = File.ReadAllText(fileNameStatus).Split('|');
            double progress = 0;

            DateTime responseInsertStarted;

            if (runningImport.Length > 1)
                progress = double.Parse(runningImport[1]);

            additional = "DataUploaderStatus" + runningImport[0] + "|";
            additional += (int)progress;

            if (runningImport[0] == "Step6" && progress != 0)
            {
                DateTime creationDate = (DateTime)Global.Core.Studies.GetValue("CreationDate", "Id", idStudy);

                if (runningImport.Length > 2)
                {
                    if (!DateTime.TryParse(runningImport[2], out responseInsertStarted))
                        responseInsertStarted = creationDate;
                }
                else
                    responseInsertStarted = creationDate;

                TimeSpan duration = DateTime.Now - responseInsertStarted;

                long etaSeconds = ((long)(duration.TotalSeconds / progress)) * 100;
                etaSeconds -= (long)duration.TotalSeconds;

                TimeSpan eta = DateTime.Now.AddSeconds(etaSeconds) - DateTime.Now;

                string etaStr = "";

                if (eta.Days != 0)
                    etaStr += eta.Days + "d ";

                if (eta.Hours != 0)
                    etaStr += eta.Hours + "h ";

                if (eta.Minutes != 0)
                    etaStr += eta.Minutes + "m ";

                if (etaStr == "" || eta.TotalSeconds <= 0)
                    etaStr = "<1m";

                additional += "|" + etaStr;
            }

            Response.Write(additional);
        }

        private void MoveStudies()
        {
            // Get the ids of all studies that should be
            // moved from the http request's parameters.
            List<Guid> idStudies = Request.Params["IdStudies"].
                Split(',').Select(x => Guid.Parse(x)).ToList();

            // Get the id of the hierarchy where
            // the studies should be assigned to.
            Guid idHierarchy = Guid.Parse(Request.Params["IdHierarchy"]);

            // Run through all studies that should be moved.
            foreach (Guid idStudy in idStudies)
            {
                // Set the hierarchy for the study.
                Global.Core.Studies.SetValue("Id=" + idStudy, "IdHierarchy", idHierarchy);
            }
        }

        private void DeleteStudies()
        {
            // Get the ids of all studies that should be
            // deleted, from the http request's parameters.
            List<Guid> idStudies = Request.Params["IdStudies"].
                Split(',').Select(x => Guid.Parse(x)).ToList();

            // Run through all study ids of the studies to delete.
            foreach (Guid idStudy in idStudies)
            {
                Study study = Global.Core.Studies.GetSingle(idStudy);

                if (study.Status == StudyStatus.Deleting)
                    return;

                study.Status = StudyStatus.Deleting;
                study.Save();
            }

            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(DeleteStudyAsynch);

            Thread thread = new Thread(threadStart);

            thread.Start(new object[] {
                idStudies,
                Global.Core,
                Request.PhysicalApplicationPath
            });

            Global.ClientManager.IncreaseCaseDataVersion(Global.Core.ClientName);
            Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(Global.Core.ClientName).CaseDataVersion;
        }

        private void InsertHierarchy()
        {
            // Parse the id of the parent hierarchy
            // from the http request's parameters.
            Guid idHierarchy = Guid.Parse(Request.Params["IdHierarchy"]);

            // Create a new hierarchy object.
            Hierarchy hierarchy = new Hierarchy(Global.Core.Hierarchies);
            hierarchy.IdHierarchy = idHierarchy;
            hierarchy.Name = Request.Params["Name"];
            hierarchy.SetValue("CreationDate", DateTime.Now);

            hierarchy.Insert();

            // Parse the id of the workgroup where to assign the
            // hierarchy to from the http request's parameters.
            Guid idWorkgroup;

            if (!Guid.TryParse(Request.Params["IdWorkgroup"], out idWorkgroup))
            {
                Workgroup workgroup = new Workgroup(Global.Core.Workgroups);
                workgroup.CreationDate = DateTime.Now;
                workgroup.Name = "Default";

                workgroup.Insert();

                idWorkgroup = workgroup.Id;

                UserWorkgroup userWorkgroup = new UserWorkgroup(Global.Core.UserWorkgroups);
                userWorkgroup.IdWorkgroup = idWorkgroup;
                userWorkgroup.IdUser = Global.IdUser.Value;

                userWorkgroup.Insert();
            }

            WorkgroupHierarchy workgroupHierarchy = new WorkgroupHierarchy(Global.Core.WorkgroupHierarchies);
            workgroupHierarchy.IdHierarchy = hierarchy.Id;
            workgroupHierarchy.IdWorkgroup = idWorkgroup;

            workgroupHierarchy.Insert();
        }

        private void UploadFile()
        {
            FileInfo fInfo = new FileInfo(Request.Files[0].FileName);

            string fileName = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid() + fInfo.Extension
            );

            Request.Files[0].SaveAs(fileName);

            this.DataUpload.IdLanguage = int.Parse(Request.Params["IdLanguage"]);
            this.DataUpload.FileName = fileName;
            this.DataUpload.StudyName = Request.Params["StudyName"];
            this.DataUpload.RespondentVariable = Request.Params["RespondentVariable"];
            this.DataUpload.IdHierarchy = Guid.Parse(Request.Params["IdHierarchy"]);

            this.DataUpload.StartImport();
        }


        private void DownloadDataAnalyser(Guid idStudy)
        {
            DataAnalyser analyser = new DataAnalyser(
                Global.Core,
                idStudy
            );

            List<DataAnalyserLog> logs;

            switch (this.Request.Params["DataAnalyserMethod"])
            {
                case "Weights":
                    logs = analyser.CheckWeights(Global.Core.Variables.GetSingle(
                        "Id",
                        Guid.Parse(this.Request.Params["IdVariable"])
                    ));
                    break;

                case "Export":

                    base.WriteFileToResponse(analyser.ExportValues(), "Export.xlsx", "application/msexcel");

                    return;
                    break;

                case "Compare":
                    //logs = analyser.CheckTaxonomizedData();
                    base.WriteFileToResponse(analyser.CheckTaxonomizedData(), "Compare.xlsx", "application/msexcel");

                    return;
                    break;

                default:
                    return;
                    break;
            }

            StringBuilder result = new StringBuilder();

            // Run through all data analyser logs.
            foreach (DataAnalyserLog log in logs)
            {
                result.Append(log.Message);
                result.Append(Environment.NewLine);
            }

            base.WriteFileToResponse(System.Text.Encoding.UTF8.GetBytes(result.ToString()), idStudy + ".txt", "text/plain");
        }


        private void RenameHierarchy()
        {
            // Parse the id of the Hierarchy to rename
            // from the http request's paramters.
            Guid idHierarchy = Guid.Parse(Request.Params["IdHierarchy"]);

            // Get the new name for the Hierarchy
            // from the http request's parameters.
            string name = Request.Params["Name"];

            // Set the new value for the name in the database.
            Global.Core.Hierarchies.SetValue("Id=" + idHierarchy, "Name", name);
        }

        private void DeleteHierarchy()
        {
            // Parse the id of the hierarchy to delete
            // from the http request's paramters.
            Guid idHierarchy = Guid.Parse(Request.Params["IdHierarchy"]);

            if (Global.Core.Studies.Count("IdHierarchy", idHierarchy) != 0)
            {
                Response.Write(Global.LanguageManager.GetText("HierarchyDeletionErrorStudies"));
                return;
            }

            if (Global.Core.TaxonomyVariableHierarchies.Count("IdHierarchy", idHierarchy) != 0)
            {
                Response.Write(Global.LanguageManager.GetText("HierarchyDeletionErrorTaxonomy"));
                return;
            }

            // Delete all workgroup hierarchies of the hierarchy.
            Global.Core.WorkgroupHierarchies.ExecuteQuery(string.Format(
                "DELETE FROM WorkgroupHierarchies WHERE IdHierarchy='{0}'",
                idHierarchy
            ));

            // Delete the Hierarchy in the database.
            Global.Core.Hierarchies.Delete(idHierarchy);
        }



        private void DeleteStudyAsynch(object _params)
        {
            object[] parameters = (object[])_params;

            List<Guid> studies = (List<Guid>)parameters[0];
            DatabaseCore.Core core = (DatabaseCore.Core)parameters[1];
            string applicationPath = (string)parameters[2];

            foreach (Guid idStudy in studies)
            {
                try
                {
                    core.Respondents.ExecuteQuery(string.Format(
                        "Delete FROM [QALogs] WHERE IdStudy='{0}'",
                        idStudy
                    ));

                    core.Respondents.ExecuteQuery(string.Format(
                        "Delete FROM [CategoryLabels] WHERE IdCategory IN (SELECT Id FROM Categories WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{0}'))",
                        idStudy
                    ));

                    core.Respondents.ExecuteQuery(string.Format(
                        "Delete FROM [CategoryLinks] WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{0}')",
                        idStudy
                    ));

                    core.Respondents.ExecuteQuery(string.Format(
                        "Delete FROM [Categories] WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{0}')",
                        idStudy
                    ));


                    core.Respondents.ExecuteQuery(string.Format(
                        "Delete FROM [VariableLinks] WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{0}')",
                        idStudy
                    ));

                    core.Respondents.ExecuteQuery(string.Format(
                        "Delete FROM [VariableLabels] WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{0}')",
                        idStudy
                    ));

                    foreach (object[] variable in core.Variables.GetValues(new string[] { "Id" }, new string[] { "IdStudy" }, new object[] { idStudy }))
                    {
                        try
                        {
                            core.Variables.ExecuteQuery("DROP TABLE resp.[Var_" + variable[0].ToString() + "]");
                        }
                        catch { }
                    }
                    core.Respondents.ExecuteQuery(string.Format(
                        "Delete FROM [Respondents] WHERE IdStudy='{0}'",
                        idStudy
                    ));

                    core.Respondents.ExecuteQuery(string.Format(
                        "Delete FROM Variables WHERE IdStudy='{0}'",
                        idStudy
                    ));

                    core.Studies.Delete(idStudy);
                }
                catch (Exception ex)
                {
                    Study study = core.Studies.GetSingle(idStudy);

                    if (study != null)
                    {
                        study.Status = StudyStatus.DeletionFailed;
                        study.Save();
                    }

                    string fileName = Path.Combine(
                        applicationPath,
                        "Fileadmin",
                        "StudyDeletionErrors"
                    );

                    if (!Directory.Exists(fileName))
                        Directory.CreateDirectory(fileName);

                    fileName = Path.Combine(
                        fileName,
                        idStudy + ".log"
                    );

                    File.WriteAllText(fileName, ex.ToString());
                }
            }

            Global.ClearCaches();
            Global.ClientManager.IncreaseCaseDataVersion(Global.Core.ClientName);
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            boxDataAnalyser.Visible = true;
            boxDataAnalyserWeights.Visible = true;
            boxInsertHierarchy.Visible = true;
            boxUpload.Visible = true;

            if (Request.Params["Method"] != null)
            {

                switch (Request.Params["Method"])
                {
                    case "GetStudies":
                        Response.Clear();
                        BindStudies();
                        Response.End();
                        break;
                    case "MoveStudies":
                        Response.Clear();
                        MoveStudies();
                        Response.End();
                        break;
                    case "DeleteStudies":
                        Response.Clear();
                        DeleteStudies();
                        Response.End();
                        break;
                    case "InsertHierarchy":
                        Response.Clear();
                        InsertHierarchy();
                        Response.End();
                        break;
                    case "GetStudyUploadStatus":
                        Response.Clear();
                        GetStudyUploadStatus();
                        Response.End();
                        break;
                    case "UploadFile":
                        Response.Clear();
                        UploadFile();
                        Response.End();
                        break;
                    case "RenameHierarchy":
                        Response.Clear();
                        RenameHierarchy();
                        Response.End();
                        break;
                    case "DeleteHierarchy":
                        Response.Clear();
                        DeleteHierarchy();
                        Response.End();
                        break;
                    case "DownloadDataAnalyser":
                        DownloadDataAnalyser(Guid.Parse(
                            Request.Params["IdStudy"]
                        ));
                        break;
                }


                return;
            }

            if (!IsPostBack)
            {
                BindLanguages();
                BindWorkgroups();
            }

            BindHierarchies();
        }

        #endregion
    }

    public class DataUpload
    {
        #region Properties

        public string StudyName { get; set; }

        public string RespondentVariable { get; set; }

        public string FileName { get; set; }

        public int IdLanguage { get; set; }

        public DataUploadProvider Provider { get; set; }

        public Guid IdStudy { get; set; }

        public Guid IdHierarchy { get; set; }


        public BaseReader Reader { get; set; }

        public string StatusFileName { get; set; }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        public void InitFile()
        {
            FileInfo fInfo = new FileInfo(this.FileName);

            string createResponses = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "DataStorage",
                "CreateResponses.sql"
            );

            switch (fInfo.Extension.ToLower())
            {
                case ".zip":
                    Unzip(fInfo);
                    InitFile();
                    return;
                case ".sav":
                case ".pkd":
                case ".mdd":
                case ".ddf":
                    this.Provider = DataUploadProvider.SPSS;
                    break;
                case ".xls":
                case ".xlsx":
                    this.Provider = DataUploadProvider.Excel;
                    break;
                case ".csv":
                    this.Provider = DataUploadProvider.CSV;
                    break;
                case ".rar":
                    Log("File format '.rar' not supported. Use '.zip' instead.");
                    break;
                default:
                    Log(string.Format(
                        "File format '{0}' not supported.",
                        fInfo.Extension
                    ));
                    break;
            }

            switch (this.Provider)
            {
                case DataUploadProvider.SPSS:
                    this.Reader = new SpssReader(this.FileName, Global.Core, null, this.RespondentVariable, createResponses, this.IdLanguage);
                    break;
                case DataUploadProvider.CSV:
                case DataUploadProvider.Excel:
                    this.Reader = new ExcelDataReader(this.FileName, Global.Core, null, this.RespondentVariable, createResponses, this.IdLanguage);
                    break;
            }

            this.Reader.ApplicationPath = HttpContext.Current.Request.PhysicalApplicationPath;
            this.Reader.Language = Global.Language.ToString();
        }

        public void Log(string message)
        {
            try
            {
                File.Delete(this.StatusFileName);
            }
            catch { }

            string fileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "StudyUploadErrors",
                this.IdStudy.ToString() + ".log"
            );

            FileInfo fInfo1 = new FileInfo(fileName);

            if (!Directory.Exists(fInfo1.Directory.FullName))
                Directory.CreateDirectory(fInfo1.Directory.FullName);

            File.WriteAllText(
                fileName,
                message
            );

            throw new Exception(message);
        }

        public List<string> ValidateFile()
        {
            return this.Reader.Validate();
        }

        public void StartImport()
        {
            // Create a new study.
            Study study = new Study(Global.Core.Studies);

            study.Status = StudyStatus.ImportFailed;

            // Set the name of the study.
            study.Name = this.StudyName;

            // Set the creation date of the study.
            study.CreationDate = DateTime.Now;

            study.IdUser = Global.User.Id;

            study.IdHierarchy = this.IdHierarchy;

            study.Insert();

            this.IdStudy = study.Id;

            this.StatusFileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "RunningImports",
                study.Id.ToString()
            );

            File.WriteAllText(this.StatusFileName, "Step2|0");

            InitFile();

            File.WriteAllText(this.StatusFileName, "Step3|0");

            this.Reader.Study = study;

            /*ParameterizedThreadStart threadStart = new ParameterizedThreadStart(StartImportAsync);

            Thread thread = new Thread(threadStart);

            Global.RunningImports.Add(study.Id, this);

            thread.Start(new object[] {
                HttpContext.Current.Session,
                study.Id
            });*/

            try
            {
                Process app = new Process();
                app.StartInfo.FileName = ConfigurationManager.AppSettings["DataImportApplication"];

                app.StartInfo.Arguments = string.Format(
                    "IdStudy={0} Provider={1} DatabaseProvider={2} ConnectionString={3} FileName={4} RespondentVariable={5} ApplicationPath={6} Language={7} IdLanguage={8} Debug=true",
                    study.Id,
                    this.Provider,
                    HttpUtility.UrlEncode(Global.Core.DatabaseProvider),
                    HttpUtility.UrlEncode(Global.Core.ConnectionString),
                    HttpUtility.UrlEncode(this.FileName),
                    this.RespondentVariable,
                    HttpUtility.UrlEncode(HttpContext.Current.Request.PhysicalApplicationPath),
                    Global.Language,
                    this.IdLanguage
                );

                app.Start();
            }
            catch (Exception ex)
            {
                Log(string.Format(
                    "An error occurred while starting the import app. {0}",
                    ex.Message
                ));
            }
        }


        private void StartImportAsync(object param)
        {
            object[] parameters = (object[])param;

            HttpSessionState session = (HttpSessionState)parameters[0];
            Guid idStudy = (Guid)parameters[1];

            Core core = (Core)session["Core"];

            List<string> fileErrors = this.Reader.Validate();

            if (fileErrors.Count > 0)
            {
                LogError(string.Join(
                    Environment.NewLine,
                    fileErrors
                ));
            }
            else
            {
                try
                {
                    this.Reader.Read();

                    Study study = this.Reader.Core.Studies.GetSingle(this.IdStudy);

                    study.Status = StudyStatus.None;

                    study.Save();
                }
                catch (Exception e)
                {
                    LogError(e.ToString());
                }
            }

            Global.RunningImports.Remove(this.IdStudy);
        }

        private void LogError(string message)
        {
            string fileName = Path.Combine(
                    this.Reader.ApplicationPath,
                    "Fileadmin",
                    "StudyUploadErrors",
                    this.IdStudy.ToString() + ".log"
                );

            FileInfo fInfo = new FileInfo(fileName);

            if (!Directory.Exists(fInfo.Directory.FullName))
                Directory.CreateDirectory(fInfo.Directory.FullName);

            File.WriteAllText(
                fileName,
                message
            );
        }

        private void Unzip(FileInfo fInfo)
        {
            File.WriteAllText(this.StatusFileName, "Step2_1|0");

            try
            {

                Guid id = Guid.NewGuid();

                string fileName = Path.Combine(
                    Path.GetTempPath(),
                    id.ToString()
                );

                System.IO.Compression.ZipFile.ExtractToDirectory(
                    this.FileName,
                    fileName
                );

                fileName = Directory.GetFiles(fileName)[0];

                this.FileName = fileName;
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        #endregion
    }
}