using ApplicationUtilities.Classes;
using DataAnalyser1.Classes;
using DatabaseCore;
using DatabaseCore.Items;
using DataInterface.BaseClasses;
using DataInterface.Classes;
using ProjectHierarchy1;
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

namespace LinkOnline.Pages.AgencyManagementSystem
{
    public partial class Overview : WebUtilities.BasePage
    {
        #region Properties

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

        private void Test()
        {
            foreach (string file in Directory.GetFiles(@"C:\Projects\Blueocean\Link\LinkManager\Tools\SurveyWriter to LiNK Survey File\SurveyWriter to LiNK Survey File\SurveyWriter to LiNK Survey File\bin\Debug\Output"))
            {
                DataUpload dataUpload = new DataUpload();
                dataUpload.IdLanguage = 2057;
                dataUpload.FileName = file;
                dataUpload.StudyName = new FileInfo(file).Name.Split('.')[0];
                dataUpload.RespondentVariable = "";

                dataUpload.StartImport();
            }
        }

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

        private void DeleteStudyAsynch(object _params)
        {
            object[] parameters = (object[])_params;

            Guid idStudy = (Guid)parameters[0];
            DatabaseCore.Core core = (DatabaseCore.Core)parameters[1];
            string applicationPath = (string)parameters[2];

            try
            {
                core.Respondents.ExecuteQuery(string.Format(
                    "Delete FROM [QALogs] WHERE IdStudy='{0}'",
                    idStudy
                ));

                core.Respondents.ExecuteQuery(string.Format(
                    "DELETE FROM [CATEGORYLABELS] WHERE IDCATEGORY IN (SELECT CATEGORIES.ID FROM CATEGORIES,VARIABLES WHERE CATEGORIES.IDVARIABLE=VARIABLES.ID AND VARIABLES.IDSTUDY='{0}')",
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

                StringBuilder dropRespData = new StringBuilder();
                dropRespData.Append("drop table ");

                foreach (object[] variable in core.Variables.GetValues(new string[] { "Id" }, new string[] { "IdStudy" }, new object[] { idStudy }))
                {
                    try
                    {
                        dropRespData.Append("resp.[Var_" + variable[0].ToString()+"],");
                        // core.Variables.ExecuteQuery("DROP TABLE resp.[Var_" + variable[0].ToString() + "]");
                    }
                    catch { }
                }

                core.Variables.ExecuteQuery(dropRespData.ToString().Remove(dropRespData.ToString().Length-1));

                core.Respondents.ExecuteQuery(string.Format(
                    "Delete FROM [Respondents] WHERE IdStudy='{0}'",
                    idStudy
                ));

                core.Respondents.ExecuteQuery(string.Format(
                    "Delete FROM Variables WHERE IdStudy='{0}'",
                    idStudy
                ));

                core.Studies.Delete(idStudy);

                Global.ClearCaches(core);
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

        private void DeleteStudy(Guid idStudy)
        {
            Study study = Global.Core.Studies.GetSingle(idStudy);

            if (study.Status == StudyStatus.Deleting)
                return;

            study.Status = StudyStatus.Deleting;
            study.Save();

            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(DeleteStudyAsynch);

            Thread thread = new Thread(threadStart);

            thread.Start(new object[] {
                idStudy,
                Global.Core,
                Request.PhysicalApplicationPath
            });

            Global.ClientManager.IncreaseCaseDataVersion(Global.Core.ClientName);
            Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(Global.Core.ClientName).CaseDataVersion;
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

        private void BindStudies()
        {
            Table table = new Table();
            table.CssClass = "TableStudies";

            TableRow tableRowHeadline = new TableRow();
            tableRowHeadline.CssClass = "TableRowHeadline Color1";

            TableCell tableCellHeadlineStudyName = new TableCell();
            TableCell tableCellHeadlineCreationDate = new TableCell();
            TableCell tableCellHeadlineTaxonomyStructureStatus = new TableCell();

            tableCellHeadlineStudyName.Text = Global.LanguageManager.GetText("StudyName");
            tableCellHeadlineCreationDate.Text = Global.LanguageManager.GetText("CreationDate");
            tableCellHeadlineTaxonomyStructureStatus.Text = Global.LanguageManager.GetText("StudyStatus");

            tableRowHeadline.Cells.Add(tableCellHeadlineStudyName);
            tableRowHeadline.Cells.Add(tableCellHeadlineCreationDate);
            tableRowHeadline.Cells.Add(tableCellHeadlineTaxonomyStructureStatus);

            table.Rows.Add(tableRowHeadline);

            if (Global.User.HasPermission(Global.PermissionCore.Permissions["UploadStudy"].Id))
            {
                TableRow tableRowNew = new TableRow();
                TableCell tableCellNew = new TableCell();

                ImageButton btnNew = new ImageButton();
                btnNew.ID = "btnNew";
                btnNew.ImageUrl = "/Images/Icons/Add.png";
                btnNew.Click += btnNew_Click;
                btnNew.Attributes.Add("onmousedown", "showSubmitLoading = false;");

                tableCellNew.Controls.Add(btnNew);
                tableRowNew.Cells.Add(tableCellNew);
                table.Rows.Add(tableRowNew);
            }

            // Run through all studies of the client.
            foreach (Study study in Global.Core.Studies.Get().OrderByDescending(x => x.CreationDate))
            {
                TableRow tableRow = new TableRow();

                TableCell tableCellStudyName = new TableCell();
                TableCell tableCellCreationDate = new TableCell();
                TableCell tableCellTaxonomyStructureStatus = new TableCell();
                TableCell tableCellOptions = new TableCell();

                tableCellOptions.CssClass = "TableCellOptions";
                tableCellOptions.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;

                tableCellStudyName.Text = study.Name;
                tableCellCreationDate.Text = study.CreationDate.ToString(
                    Global.LanguageManager.GetText("DateFormat") + " " +
                    Global.LanguageManager.GetText("TimeFormat")
                );

                // Build the path to the study's taxonomy structure file.
                string fileName = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "StudyTaxonomyStructures",
                    Global.Core.ClientName,
                    study.Id + ".xml"
                );

                bool showDelete = true;
                OptionSwipe options = new OptionSwipe();

                Option optionDownloadAssignment = new Option();
                optionDownloadAssignment.Text = Global.LanguageManager.GetText("DownloadAssignment");
                optionDownloadAssignment.CssClass = "DarkGrayBackground";
                optionDownloadAssignment.OnClientClick = string.Format(
                    "DownloadAssignment('{0}')",
                    study.Id
                );

                if (study.Status == StudyStatus.None)
                {
                    /*Option optionSetHierarchy = new Option();
                    optionSetHierarchy.Text = Global.LanguageManager.GetText("SetProjectHierarchy");
                    optionSetHierarchy.CssClass = "BackgroundColor1";
                    optionSetHierarchy.OnClientClick = string.Format(
                        "SetProjectHierachy('{0}')",
                        study.Id
                    );*/

                    Option optionDataAnalyser = new Option();
                    optionDataAnalyser.Text = Global.LanguageManager.GetText("RunDataAnalyser");
                    optionDataAnalyser.Style.Add("background", "#000000");
                    optionDataAnalyser.OnClientClick = string.Format(
                        "RunDataAnalyser('{0}');",
                        study.Id
                    );

                    Option optionModifyVariables = new Option();
                    optionModifyVariables.Text = Global.LanguageManager.GetText("ModifyVariables");
                    optionModifyVariables.CssClass = "BackgroundColor2";
                    optionModifyVariables.OnClientClick = string.Format(
                        "window.location='Variables.aspx?IdStudy={0}';",
                        study.Id
                    );

                    /*if (Global.User.HasPermission(Global.PermissionCore.Permissions["SetStudyHiarachy"].Id))
                        options.Options.Add(optionSetHierarchy);*/

                    if (Global.User.HasPermission(Global.PermissionCore.Permissions["DownloadAssignment"].Id))
                        options.Options.Add(optionDownloadAssignment);

                    if (Global.User.HasPermission(Global.PermissionCore.Permissions["ModifyVariables"].Id))
                        options.Options.Add(optionModifyVariables);

                    if (Global.User.HasPermission(Global.PermissionCore.Permissions["DataAnalyser"].Id))
                        options.Options.Add(optionDataAnalyser);

                    // Check if a taxonomy structure for the study is set.
                    if (File.Exists(fileName))
                    {
                        ProjectHierarchy hierarchy = new ProjectHierarchy(fileName);

                        if (hierarchy.IsValid())
                        {
                            tableCellTaxonomyStructureStatus.Text = Global.LanguageManager.GetText("ProjectHierarchySet");
                        }
                        else
                        {
                            tableCellTaxonomyStructureStatus.Text = Global.LanguageManager.GetText("ProjectHierarchyIncomplete");
                        }
                    }
                    else
                    {
                        tableCellTaxonomyStructureStatus.Text = Global.LanguageManager.GetText("NoProjectHierarchySet");
                    }
                }
                else
                {
                    string fileNameStatus = Path.Combine(
                        Request.PhysicalApplicationPath,
                        "Fileadmin",
                        "RunningImports",
                        study.Id.ToString()
                    );

                    if (File.Exists(fileNameStatus))
                    {
                        showDelete = false;

                        string[] runningImport = File.ReadAllText(fileNameStatus).Split('|');
                        double progress = 0;

                        DateTime responseInsertStarted;

                        if (runningImport.Length > 1)
                            progress = double.Parse(runningImport[1]);

                        if (runningImport[0] == "Step6")
                        {
                            if (Global.User.HasPermission(Global.PermissionCore.Permissions["DownloadAssignment"].Id))
                                options.Options.Add(optionDownloadAssignment);
                        }

                        tableCellTaxonomyStructureStatus.CssClass = "Color1";
                        tableCellTaxonomyStructureStatus.Text = Global.LanguageManager.GetText("DataUploaderStatus" + runningImport[0]);
                        tableCellTaxonomyStructureStatus.Text += "&nbsp;";
                        tableCellTaxonomyStructureStatus.Text += "<span class=\"Color2\">" + Math.Round(progress, 2) + "%</span>";

                        if (runningImport[0] == "Step6" && progress != 0)
                        {

                            if (runningImport.Length > 2)
                            {
                                if (!DateTime.TryParse(runningImport[2], out responseInsertStarted))
                                    responseInsertStarted = study.CreationDate;
                            }
                            else
                                responseInsertStarted = study.CreationDate;

                            TimeSpan duration = DateTime.Now - responseInsertStarted;

                            long etaSeconds = ((long)(duration.TotalSeconds / progress)) * 100;
                            etaSeconds -= (long)duration.TotalSeconds;

                            TimeSpan eta;
                            try
                            {
                                eta = DateTime.Now.AddSeconds(etaSeconds) - DateTime.Now;
                            }
                            catch
                            {
                                eta = DateTime.Now - DateTime.Now;
                            }

                            string etaStr = "";

                            if (eta.Days != 0)
                                etaStr += eta.Days + "d ";

                            if (eta.Hours != 0)
                                etaStr += eta.Hours + "h ";

                            if (eta.Minutes != 0)
                                etaStr += eta.Minutes + "m ";

                            if (etaStr == "" || eta.TotalSeconds <= 0)
                                etaStr = "<1m";

                            tableCellTaxonomyStructureStatus.Text += "&nbsp;" + string.Format(
                                Global.LanguageManager.GetText("DataImportETA"),
                                etaStr
                            );
                        }
                    }
                    else
                    {
                        string fileNameError = Path.Combine(
                            Request.PhysicalApplicationPath,
                            "Fileadmin",
                            study.Status == StudyStatus.ImportFailed ? "StudyUploadErrors" : "StudyDeletionErrors",
                            study.Id.ToString() + ".log"
                        );

                        string errorMessage = "";

                        if (File.Exists(fileNameError))
                            errorMessage = File.ReadAllText(fileNameError);

                        errorMessage = HttpUtility.HtmlEncode(errorMessage.Split('\n')[0].Trim());

                        if (study.Status == StudyStatus.ImportFailed || study.Status == StudyStatus.DeletionFailed)
                        {
                            tableCellTaxonomyStructureStatus.Text = string.Format(
                                "<a style=\"cursor:pointer;color:#FF0000;font-weight:bold;text-decoration:underline;\" ErrorMessage=\"{1}\" onclick=\"ShowImportErrorDetail(this.getAttribute('ErrorMessage'));\">{0}</a>",
                                Global.LanguageManager.GetText("Study" + study.Status),
                                errorMessage
                            );
                        }
                        else
                        {
                            tableCellTaxonomyStructureStatus.Text = string.Format(
                                "{0}",
                                Global.LanguageManager.GetText("Study" + study.Status)
                            );
                        }

                    }
                }

                if (study.Status != StudyStatus.Deleting && showDelete)
                {
                    Option optionDeleteStudy = new Option();
                    optionDeleteStudy.Text = Global.LanguageManager.GetText("DeleteStudy");
                    optionDeleteStudy.CssClass = "RedBackground";
                    optionDeleteStudy.OnClientClick = string.Format(
                        "DeleteStudy('{0}', '{1}');",
                       HttpUtility.HtmlAttributeEncode(study.Id.ToString()),
                        HttpUtility.HtmlAttributeEncode(study.Name)
                    );

                    if (Global.User.HasPermission(Global.PermissionCore.Permissions["DeleteStudy"].Id))
                        options.Options.Add(optionDeleteStudy);
                }

                tableCellOptions.Controls.Add(options);

                tableRow.Cells.Add(tableCellStudyName);
                tableRow.Cells.Add(tableCellCreationDate);
                tableRow.Cells.Add(tableCellTaxonomyStructureStatus);
                tableRow.Cells.Add(tableCellOptions);

                table.Rows.Add(tableRow);
            }

            pnlStudies.Controls.Add(table);
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

            this.DataUpload.StartImport();
        }

        private void DownloadAssignment(Guid idStudy)
        {
            // Create a new excel writer to write the
            // result assignment excel file.
            ExcelWriter writer = new ExcelWriter();

            bool autoLink = false;

            // Write the headline of the assignment file.
            writer.Write(0, "TaxonomyVariable");
            writer.Write(1, "StudyVariable");
            writer.Write(2, "TaxonomyCategory");
            writer.Write(3, "StudyCategory");

            writer.ActiveSheet.Cells[0, 0, 0, 3].Font.Bold = true;
            writer.ActiveSheet.Cells[0, 0, 0, 3].Interior.Color = SpreadsheetGear.Color.FromArgb(255, 255, 204);
            writer.ActiveSheet.Cells[0, 0, 0, 3].Borders.Color = SpreadsheetGear.Color.FromArgb(178, 178, 178);
            writer.ActiveSheet.Cells[0, 0, 0, 3].Borders.LineStyle = SpreadsheetGear.LineStyle.Continous;

            writer.ActiveSheet.WindowInfo.FreezePanes = true;
            writer.ActiveSheet.Cells[0, 0, 0, 3].Select();

            writer.ActiveSheet.Cells[0, 0].ColumnWidth = 40;
            writer.ActiveSheet.Cells[0, 1].ColumnWidth = 40;
            writer.ActiveSheet.Cells[0, 2].ColumnWidth = 40;
            writer.ActiveSheet.Cells[0, 3].ColumnWidth = 40;

            writer.NewLine();

            // Get all variables of the study.
            List<object[]> variables = Global.Core.Variables.GetValues(
                new string[] { "Id", "Name" },
                new string[] { "IdStudy" },
                new object[] { idStudy },
                "Order"
            );

            Dictionary<string, string> variableLinks = new Dictionary<string, string>();

            Dictionary<Guid, List<object[]>> categories = Global.Core.Categories.ExecuteReaderDict<Guid>(
                "SELECT IdVariable, Name FROM Categories ORDER BY [order]",
                new object[] { }
            );

            Guid idVariable;
            // Run through all variables of the study.
            foreach (object[] variable in variables)
            {
                idVariable = (Guid)variable[0];
                // Get all categories of the variable.
                /*List<object[]> categories = Global.Core.Categories.GetValues(
                    new string[] { "Name" },
                    new string[] { "IdVariable" },
                    new object[] { variable[0] },
                    "Order"
                );*/

                // Check if the variable has categories.
                //if (categories.Count == 0)
                if (!categories.ContainsKey(idVariable))
                {
                    // Write the name of the study variable into
                    // the second column of the assignment file.
                    writer.Write(1, (string)variable[1]);

                    writer.NewLine();
                }
                else
                {
                    // Run through all categories of the study.
                    foreach (object[] category in categories[idVariable])
                    {
                        // Write the name of the study variable into
                        // the second column of the assignment file.
                        writer.Write(1, (string)variable[1]);

                        // Write the name of the study category into
                        // the fourth column of the assignment file.
                        writer.Write(3, (string)category[1]);

                        // Check if auto linking is enabled.
                        if (autoLink)
                        {
                            try
                            {
                                // Get all the taxonomy categories where categories, with
                                // the same name in the existing database, are linked to.
                                List<object[]> taxonomyCategories = Global.Core.TaxonomyCategories.ExecuteReader(
                                    "SELECT Name, (SELECT Name FROM TaxonomyVariables WHERE TaxonomyVariables.Id=TaxonomyCategories.IdTaxonomyVariable)" +
                                    " FROM TaxonomyCategories WHERE Id IN (SELECT IdTaxonomyCategory " +
                                    "FROM CategoryLinks WHERE IdCategory IN (SELECT Id FROM Categories WHERE Name='{0}'))"
                                , new object[] { category[1] });

                                Dictionary<string, string> taxonomyVariables = new Dictionary<string, string>();
                                Dictionary<string, int> linkFrequency = new Dictionary<string, int>();

                                // Run through all taxonomy categories.
                                foreach (object[] taxonomyCategory in taxonomyCategories)
                                {
                                    if (!linkFrequency.ContainsKey((string)taxonomyCategory[0]))
                                        linkFrequency.Add((string)taxonomyCategory[0], 0);

                                    linkFrequency[(string)taxonomyCategory[0]]++;

                                    if (!taxonomyVariables.ContainsKey((string)taxonomyCategory[0]))
                                        taxonomyVariables.Add((string)taxonomyCategory[0], (string)taxonomyCategory[1]);
                                }

                                if (linkFrequency.Count > 0)
                                {
                                    string taxonomyCategory = linkFrequency.OrderBy(x => x.Value).First().Key;

                                    // Write the name of the taxonomy category with the highest link
                                    // frequency into the third column of the assignment file.
                                    writer.Write(2, taxonomyCategory);

                                    if (!variableLinks.ContainsKey((string)variable[1]))
                                        variableLinks.Add((string)variable[1], taxonomyVariables[taxonomyCategory]);

                                    // Write the name of the taxonomy category's variable with the highest link
                                    // frequency into the third column of the assignment file.
                                    writer.Write(0, variableLinks[(string)variable[1]]);
                                }
                            }
                            catch { }
                        }

                        writer.NewLine();
                    }
                }
            }

            writer.ActiveSheet.Cells[0, 0].EntireColumn.Locked = false;
            writer.ActiveSheet.Cells[0, 1].EntireColumn.Locked = true;
            writer.ActiveSheet.Cells[0, 2].EntireColumn.Locked = false;
            writer.ActiveSheet.Cells[0, 3].EntireColumn.Locked = true;

            writer.ActiveSheet.ProtectContents = true;

            // Save the assignment file to the memory.
            byte[] data = writer.Save();

            // Get the name of the study.
            string studyName = (string)Global.Core.Studies.GetValue("Name", "Id", idStudy);
            if (studyName.Contains(","))
            {
                studyName = String.Format("\"{0}\"", studyName);
            }

            // Write the assignment file to the response.
            base.WriteFileToResponse(
                data,
                studyName + ".xlsx",
                "application/msexcel"
            );
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["Test"] == "True")
                Test();

            boxUpload.Visible = false;

            if (Request.Params["Action"] == "UploadFile")
            {
                UploadFile();

                return;
            }

            if (Request.Params["Action"] == "DownloadAssignment")
            {
                DownloadAssignment(Guid.Parse(
                    Request.Params["IdStudy"]
                ));

                return;
            }

            if (Request.Params["DeleteStudy"] != null)
            {
                DeleteStudy(Guid.Parse(
                    Request.Params["DeleteStudy"]
                ));

                return;
            }

            if (Request.Params["Action"] == "DownloadDataAnalyser")
            {
                DownloadDataAnalyser(Guid.Parse(
                    Request.Params["IdStudy"]
                ));

                return;
            }

            // Build the path to the directory where the
            // taxonomy strucures are defined.
            string directory = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "StudyTaxonomyStructures",
                Global.Core.ClientName
            );

            // Check if the directory exists.
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            BindStudies();

            boxDataAnalyser.Visible = true;
            boxDataAnalyserWeights.Visible = true;

            if (!IsPostBack)
                BindLanguages();
        }


        protected void btnNew_Click(object sender, ImageClickEventArgs e)
        {
            boxUpload.Visible = true;

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "DisableLoadLoading",
                "showLoadLoading = false;",
                true
            );
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
                case ".ldf":
                    this.Provider = DataUploadProvider.LDF;
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
                case DataUploadProvider.LDF:
                    this.Reader = new LdfReader(this.FileName, Global.Core, null, createResponses, this.IdLanguage);
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

            study.IdHierarchy = (Guid)Global.Core.Hierarchies.GetValue(
                "Id",
                "IdHierarchy",
                null
            );

            study.Insert();

            this.IdStudy = study.Id;

            this.StatusFileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "RunningImports",
                study.Id.ToString()
            );

            if (!Directory.Exists(Path.GetDirectoryName(this.StatusFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(this.StatusFileName));

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
                    "IdStudy={0} Provider={1} DatabaseProvider={2} ConnectionString={3} FileName={4} RespondentVariable={5} ApplicationPath={6} Language={7} IdLanguage={8} ClientName={9} CaseDataLocation={10} Debug=true",
                    study.Id,
                    this.Provider,
                    HttpUtility.UrlEncode(Global.Core.DatabaseProvider),
                    HttpUtility.UrlEncode(Global.Core.ConnectionString),
                    HttpUtility.UrlEncode(this.FileName),
                    this.RespondentVariable,
                    HttpUtility.UrlEncode(HttpContext.Current.Request.PhysicalApplicationPath),
                    Global.Language,
                    this.IdLanguage,
                    Global.Core.ClientName,
                    Global.Core.CaseDataLocation
                );

                app.Start();

                Global.HierarchyFilters.Default.Clear();
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