using ApplicationUtilities;
using ApplicationUtilities.Classes;
using Crosstables.Classes;
using Crosstables.Classes.HierarchyClasses;
using Crosstables.Classes.ReportDefinitionClasses;
using Crosstables.Classes.WorkflowClasses;
using DatabaseCore.Items;
using DataCore.Classes;
using LinkOnline.Classes;
using LinkOnline.Classes.Controls;
using MasterPage.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using WebUtilities.Controls;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für GlobalHandler
    /// </summary>
    public class GlobalHandler : IHttpHandler, IRequiresSessionState
    {
        #region Properties

        /// <summary>
        /// Gets or sets the available methods of the generic handler.
        /// </summary>
        public Dictionary<string, Meth> Methods { get; set; }

        /// <summary>
        /// Gets if the generic handler is re useable.
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public Thread ActiveCategorySearch
        {
            get
            {
                if (HttpContext.Current.Session["ActiveCategorySearches"] == null)
                    return null;

                return (Thread)HttpContext.Current.Session["ActiveCategorySearches"];
            }
            set
            {
                HttpContext.Current.Session["ActiveCategorySearches"] = value;
            }
        }

        #endregion


        #region Constructor

        public GlobalHandler()
            : base()
        {
            this.Methods = new Dictionary<string, Meth>();

            this.Methods.Add("CreateSession", CreateSession);

            this.Methods.Add("ReportDefinitionHistoryBack", ReportDefinitionHistoryBack);
            this.Methods.Add("ManualSaveReportFolderSelect", ManualSaveReportFolderSelect);

            this.Methods.Add("KeepSessionAlive", KeepSessionAlive);
            this.Methods.Add("MD5Encrypt", MD5Encrypt);

            this.Methods.Add("HasData", HasData);
            this.Methods.Add("SwitchTable", SwitchTable);
            this.Methods.Add("BindReportDisplayTypes", BindReportDisplayTypes);
            this.Methods.Add("PinToHomescreen", PinToHomescreen);

            this.Methods.Add("GetUserImage", GetUserImage);

            this.Methods.Add("ChangeReportTab", ChangeReportTab);
            this.Methods.Add("CreateNewReportTab", CreateNewReportTab);
            this.Methods.Add("DuplicateReportTab", DuplicateReportTab);
            this.Methods.Add("DeleteReportTab", DeleteReportTab);
            this.Methods.Add("ChangeReportTabName", ChangeReportTabName);

            this.Methods.Add("SetContentWidth", SetContentWidth);
            this.Methods.Add("SetContentHeight", SetContentHeight);

            this.Methods.Add("SwitchVariableLocation", SwitchVariableLocation);
            this.Methods.Add("SelectVariable", SelectVariable);
            this.Methods.Add("DeSelectVariable", DeSelectVariable);
            this.Methods.Add("SearchVariables", SearchVariables);
            this.Methods.Add("SearchTaxonomyVariables", SearchTaxonomyVariables);
            this.Methods.Add("GetVariables", GetVariables);
            this.Methods.Add("GetCategories", GetCategories);
            this.Methods.Add("GetTaxonomyCategories", GetTaxonomyCategories);
            this.Methods.Add("GetDataAggregationProgress", GetDataAggregationProgress);
            this.Methods.Add("RemoveVariable", RemoveVariable);
            this.Methods.Add("RemoveScore", RemoveScore);
            this.Methods.Add("RenameScore", RenameScore);
            this.Methods.Add("CombineScores", CombineScores);

            this.Methods.Add("SearchEquationMethods", SearchEquationMethods);

            this.Methods.Add("GetSelectedWorkflowItems", GetSelectedWorkflowItems);
            this.Methods.Add("SelectWorkflowSelectorItem", SelectWorkflowSelectorItem);

            this.Methods.Add("AddFilterScoreGroup", AddFilterScoreGroup);
            this.Methods.Add("AddFilterCategory", AddFilterCategory);
            this.Methods.Add("AddFilterCategories", AddFilterCategories);
            this.Methods.Add("DeleteFilterCategory", DeleteFilterCategory);
            this.Methods.Add("AddFilterCategoryOperator", AddFilterCategoryOperator);
            this.Methods.Add("DeleteFilterCategoryOperator", DeleteFilterCategoryOperator);
            this.Methods.Add("RemoveFilterCategory", RemoveFilterCategory);
            this.Methods.Add("UpdateFilterView", UpdateFilterView);
            this.Methods.Add("ChangeFilterCategoryOperator", ChangeFilterCategoryOperator);

            this.Methods.Add("ClearReportDefinition", ClearReportDefinition);
            this.Methods.Add("ClearTabDefinition", ClearTabDefinition);
            this.Methods.Add("PopulateCrosstable", PopulateCrosstable);
            this.Methods.Add("BuildCrosstable", BuildCrosstable);
            this.Methods.Add("PropagateFilterDefinition", PropagateFilterDefinition);

            this.Methods.Add("UpdateSetting", UpdateSetting);
            this.Methods.Add("PinBottomBar", PinBottomBar);

            // Weighting.
            this.Methods.Add("RemoveWeightingVariable", RemoveWeightingVariable);
            this.Methods.Add("SetOverallWeightingVariable", SetOverallWeightingVariable);

            // link cloud.
            this.Methods.Add("UploadFile", UploadFile);

            this.Methods.Add("ValidateCategory", ValidateCategory);
            this.Methods.Add("ExportTable", ExportTable);
            this.Methods.Add("ExportAllTabs", ExportAllTabs);

            this.Methods.Add("ExportVariables", ExportVariables);


            this.Methods.Add("SaveExisting", SaveExisting);
            this.Methods.Add("SaveTab", SaveTab);
            this.Methods.Add("SaveAllTabs", SaveAllTabs);
            this.Methods.Add("OverwriteAllowed", OverwriteAllowed);

            // AMS
            this.Methods.Add("GetNumericVariables", GetNumericVariables);


            this.Methods.Add("HierarchySelectorRender", HierarchySelectorRender);
            this.Methods.Add("HierarchySelectorConfirm", HierarchySelectorConfirm);
            this.Methods.Add("LoadHierarchySelectedItems", LoadHierarchySelectedItems);

            this.Methods.Add("SearchCategories", SearchCategories);

            this.Methods.Add("GetCombiningCategories", GetCombiningCategories);
            this.Methods.Add("CombineMultipleCategories", CombineMultipleCategories);
            this.Methods.Add("ApplyWorkflowFilterToAllTabs", ApplyWorkflowFilterToAllTabs);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Writes a file to the response of a request.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file that is to be written.
        /// </param>
        /// <param name="displayName">
        /// The name that is to be displayed for the file.
        /// </param>
        public void WriteFileToResponse(string fileName, string displayName, string mimeType, bool deleteFile)
        {
            // Local variables.
            HttpContext context = null;


            // Replace the spaces in the 
            // file name with underscores.
            displayName = displayName.Replace(" ", "_");


            // Read the context.
            context = HttpContext.Current;

            // Configure the response and transfer the file.
            context.Response.Buffer = true;
            context.Response.Clear();
            context.Response.AppendHeader("content-disposition",
                string.Format(CultureInfo.InvariantCulture,
                "attachment; filename={0}", displayName));
            context.Response.ContentType = mimeType;

            byte[] buffer = File.ReadAllBytes(fileName);

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);

            if (deleteFile)
                File.Delete(fileName);

            context.Response.RedirectLocation = context.Request.Url.ToString();

            context.Response.End();
        }

        /// <summary>
        /// Writes a file to the response of a request.
        /// </summary>
        /// <param name="data">
        /// The data to transmit as byte array.
        /// </param>
        /// <param name="displayName">
        /// The name that is to be displayed for the file.
        /// </param>
        public void WriteFileToResponse(byte[] data, string displayName, string mimeType)
        {
            // Local variables.
            HttpContext context = null;


            // Replace the spaces in the 
            // file name with underscores.
            displayName = displayName.Replace(" ", "_");


            // Read the context.
            context = HttpContext.Current;

            // Configure the response and transfer the file.
            context.Response.Buffer = true;
            context.Response.Clear();
            context.Response.AppendHeader("content-disposition",
                string.Format(CultureInfo.InvariantCulture,
                "attachment; filename={0}", displayName));
            context.Response.ContentType = mimeType;
            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.End();
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(HttpContext context)
        {
            // Get the requested method name from the http request.
            string method = context.Request.Params["Method"];

            if (method != "SetContentWidth" && method != "SetContentHeight" && method != "CreateSession")
            {
                // Check if the current session has an authenticated user.
                if (HttpContext.Current.Session["User"] == null)
                    throw new Exception("Not authenticated.");
            }

            // Check if the requested method exists.
            if (!this.Methods.ContainsKey(method))
                throw new NotImplementedException();

            // Invoke the requested method.
            this.Methods[method].Invoke(context);
        }


        private void PopulateCrosstableAsynch(object _parameters)
        {
            HttpSessionState session = (HttpSessionState)_parameters;

            DatabaseCore.Core core = (DatabaseCore.Core)session["Core"];

            if (session["ReportDefinition"] == null)
            {
                session["DataAggregationProgress"] = 100;
                return;
            }

            string fileName = session["ReportDefinition"].ToString();

            ReportDefinition reportDefinition = new ReportDefinition(
                core,
                fileName,
                ((HierarchyFilterCollection)session["HierarchyFilters"])[fileName]
            );

            if (reportDefinition.XmlDocument.DocumentElement.SelectSingleNode("Results") != null || reportDefinition.Settings.AutoUpdate == false)
            {
                session["DataAggregationProgress"] = 100;
                return;
            }

            ReportCalculator calculator = new ReportCalculator(
                reportDefinition,
                core,
                session
            );

            calculator.Aggregate((string)session["Version"]);

            //reportDefinition.Settings.AutoUpdate = true;
            //reportDefinition.Save();
        }

        public string ToJson(string[] names, object[] values)
        {
            StringBuilder jsonBuilder = new StringBuilder();

            jsonBuilder.Append("{");

            for (int i = 0; i < names.Length; i++)
            {
                jsonBuilder.Append(string.Format(
                    "\"{0}\": \"{1}\",",
                    names[i],
                    HttpUtility.HtmlEncode(values[i])
                ));
            }

            string result = jsonBuilder.ToString();

            if (result.Length > 0)
                result = result.Remove(result.Length - 1, 1);

            result += "}";

            return result;
        }

        private string PrepareLabel(string label)
        {
            if (label == null)
                return "";

            string result = label;

            result = result.Replace("↵", "");
            result = result.Replace("\n", "");
            result = result.Replace("\r", "");
            result = result.Replace("\t", "");
            result = result.Trim();

            return result;
        }


        private void LogReportDefinitionHistory(string fileName, bool deleteReport = false)
        {
            string key = "ReportDefinitionHistory" + HttpContext.Current.Session["ReportDefinition"];

            if (deleteReport)
            {
                key = "deleteReport";
                HttpContext.Current.Session["isDeteted"] = true;
            }
            else
            {
                HttpContext.Current.Session["isDeteted"] = false;
            }

            if (HttpContext.Current.Session[key] == null)
                HttpContext.Current.Session[key] = new List<string>();

            List<string> historyItems = (List<string>)HttpContext.Current.Session[key];

            string historyFileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                "ReportDefinitionHistory",
                HttpContext.Current.Session.SessionID,
                Guid.NewGuid() + ".xml"
            );

            if (!Directory.Exists(Path.GetDirectoryName(historyFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(historyFileName));

            File.Copy(fileName, historyFileName, true);

            historyItems.Add(historyFileName);

            HttpContext.Current.Session[key] = historyItems;
        }

        private void RestoreUnkrankedOrder(ReportDefinitionVariable variable)
        {
            foreach (ReportDefinitionScore score in variable.Scores)
            {
                if (score.XmlNode.Attributes["UnrankedOrder"] == null)
                    continue;

                score.XmlNode.Attributes["Order"].Value = score.XmlNode.Attributes["UnrankedOrder"].Value;
                score.XmlNode.Attributes.RemoveNamedItem("UnrankedOrder");
            }

            foreach (ReportDefinitionVariable nestedVariable in variable.NestedVariables)
            {
                RestoreUnkrankedOrder(nestedVariable);
            }
        }

        #endregion

        #region Web Methods

        private void CreateSession(HttpContext context)
        {
            context.Response.Write(context.Session.SessionID);
        }
        private void ManualSaveReportFolderSelect(HttpContext context)
        {
            string destination = context.Request.Params["destination"];
            HttpContext.Current.Session["ManualSaveReportFolderSelect"] = HttpUtility.UrlDecode(context.Request.Params["destination"]).ToString();
        }

        private void ReportDefinitionHistoryBack(HttpContext context)
        {
            try
            {
                string key = "ReportDefinitionHistory" + HttpContext.Current.Session["ReportDefinition"];

                if (HttpContext.Current.Session["isDeteted"].ToString() == "True")
                {
                    if (HttpContext.Current.Session["deleteReport"] != null)
                        key = "deleteReport";
                }


                if (HttpContext.Current.Session[key] == null)
                    return;

                List<string> historyItems = (List<string>)HttpContext.Current.Session[key];


                if (historyItems.Count == 0)
                    return;

                int count = 0;
                if (HttpContext.Current.Session["deleteReport"] != null)
                {
                    List<string> deleteItems = (List<string>)HttpContext.Current.Session["deleteReport"];
                    count = deleteItems.Count;
                }

                if (count <= 0)
                {
                    File.Copy(historyItems[historyItems.Count - 1], (string)HttpContext.Current.Session["ReportDefinition"], true);
                }
                else
                {
                    File.Copy(historyItems[historyItems.Count - 1], Path.Combine(Path.GetDirectoryName((string)HttpContext.Current.Session["ReportDefinition"]), Path.GetFileName(historyItems[historyItems.Count - 1])));
                    HttpContext.Current.Session["deleteReport"] = null;
                    HttpContext.Current.Session.Remove("deleteReport");
                    HttpContext.Current.Session["isDeteted"] = false;
                }

                //     File.Copy(historyItems[historyItems.Count - 1], (string)HttpContext.Current.Session["ReportDefinition"], true);

                historyItems.RemoveAt(historyItems.Count - 1);

                HttpContext.Current.Session[key] = historyItems;

            }
            catch (Exception)
            {

            }

        }


        private void MD5Encrypt(HttpContext context)
        {
            context.Response.Write(Global.Core.Users.GetMD5Hash(
                context.Request.Params["Value"]
            ));
        }


        private void HierarchySelectorRender(HttpContext context)
        {
            if (HttpContext.Current.Session["HierarchySelector"] == null)
                return;

            HierarchySelector selector = (HierarchySelector)HttpContext.Current.Session["HierarchySelector"];

            selector.CheckForSelection();
            selector.Parse();

            context.Response.Write(selector.Render());
        }

        private void HierarchySelectorConfirm(HttpContext context)
        {
            if (HttpContext.Current.Session["HierarchySelector"] == null)
                return;

            HierarchySelector selector = (HierarchySelector)HttpContext.Current.Session["HierarchySelector"];

            XmlDocument reportDocument = new XmlDocument();
            reportDocument.Load(selector.Source);

            XmlNode xmlNode = reportDocument.DocumentElement.SelectSingleNode("HierarchyFilter");

            if (xmlNode == null)
            {
                xmlNode = reportDocument.CreateElement("HierarchyFilter");

                reportDocument.DocumentElement.AppendChild(xmlNode);
            }

            xmlNode.InnerXml = "";

            string hierarchyFilterKey = selector.Source.Replace("\\", "/");
            Global.HierarchyFilters[hierarchyFilterKey].Clear();

            foreach (string section in selector.SelectedItems.Keys)
            {
                foreach (Guid idHierarchy in selector.SelectedItems[section])
                {
                    xmlNode.InnerXml += string.Format(
                        "<Hierarchy Id=\"{0}\" Section=\"{1}\" />",
                        idHierarchy,
                        section
                    );

                    Global.HierarchyFilters[hierarchyFilterKey].Hierarchies.Add(idHierarchy);
                }
            }

            XmlNode xmlNodeResults = reportDocument.DocumentElement.SelectSingleNode("Results");

            if (xmlNodeResults != null)
                reportDocument.DocumentElement.RemoveChild(xmlNodeResults);

            reportDocument.Save(selector.Source);

            Global.HierarchyFilters[selector.Source, false].Clear();
            if (context.Request.Params["AllHierarchy"] != null)
            {
                if (context.Request.Params["AllHierarchy"] == "true")
                {
                    string path = selector.Source.Substring(0, selector.Source.LastIndexOf("\\"));

                    foreach (var file in Directory.GetFiles(path))
                    {
                        XmlDocument reportDocument1 = new XmlDocument();
                        reportDocument.Load(file);
                        if (file == selector.Source)
                            continue;
                        if (Path.GetFileName(file) == "Info.xml")
                            continue;

                        XmlNode xmlNode1 = reportDocument.DocumentElement.SelectSingleNode("HierarchyFilter");

                        if (xmlNode1 != null)
                        {
                            xmlNode1.InnerXml = "";

                        }

                        string hierarchyFilterKey1 = selector.Source.Replace("\\", "/");
                        Global.HierarchyFilters[hierarchyFilterKey1].Clear();

                        foreach (string section in selector.SelectedItems.Keys)
                        {
                            foreach (Guid idHierarchy in selector.SelectedItems[section])
                            {
                                xmlNode1.InnerXml += string.Format(
                                    "<Hierarchy Id=\"{0}\" Section=\"{1}\" />",
                                    idHierarchy,
                                    section
                                );

                                Global.HierarchyFilters[hierarchyFilterKey1].Hierarchies.Add(idHierarchy);
                            }
                        }
                        XmlNode xmlNodeResults1 = reportDocument.DocumentElement.SelectSingleNode("Results");

                        if (xmlNodeResults1 != null)
                            reportDocument.DocumentElement.RemoveChild(xmlNodeResults1);

                        reportDocument.Save(file);

                        Global.HierarchyFilters[file, false].Clear();
                    }
                }
            }
        }

        private void LoadHierarchySelectedItems(HttpContext context)
        {
            if (HttpContext.Current.Session["HierarchySelector"] == null)
                return;

            HierarchySelector selector = (HierarchySelector)HttpContext.Current.Session["HierarchySelector"];

            XmlDocument reportDocument = new XmlDocument();
            reportDocument.Load(selector.Source);

            XmlNodeList xmlNodes = reportDocument.DocumentElement.SelectNodes("HierarchyFilter/Hierarchy");

            selector.SelectedItems = new Dictionary<string, List<Guid>>();

            foreach (XmlNode xmlNode in xmlNodes)
            {
                string section = xmlNode.Attributes["Section"].Value;

                if (!selector.SelectedItems.ContainsKey(section))
                    selector.SelectedItems.Add(section, new List<Guid>());

                selector.SelectedItems[section].Add(Guid.Parse(
                    xmlNode.Attributes["Id"].Value
                ));
            }
        }


        private void KeepSessionAlive(HttpContext context)
        {

        }


        private void HasData(HttpContext context)
        {
            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new xml document to
            // store the report definition.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the report
            // definition into the xml document.
            xmlDocument.Load(fileName);

            bool result = true;

            if (xmlDocument.DocumentElement.SelectSingleNode("Results") == null)
            {
                if (xmlDocument.DocumentElement.SelectNodes("Variables[@Position=\"Left\"]/*").Count > 0 ||
                    xmlDocument.DocumentElement.SelectNodes("Variables[@Position=\"Top\"]/*").Count > 0)
                {
                    result = false;
                }
            }

            context.Response.Write(result);
        }

        private void SwitchTable(HttpContext context)
        {
            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            LogReportDefinitionHistory(fileName);

            // Create a new xml document to
            // store the report definition.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the report
            // definition into the xml document.
            xmlDocument.Load(fileName);

            XmlNode xmlNodeResults = xmlDocument.DocumentElement.SelectSingleNode("Results");

            if (xmlNodeResults != null)
                xmlNodeResults.ParentNode.RemoveChild(xmlNodeResults);

            XmlNode xmlNodeVariablesTop = xmlDocument.DocumentElement.SelectSingleNode("Variables[@Position=\"Top\"]");
            XmlNode xmlNodeVariablesLeft = xmlDocument.DocumentElement.SelectSingleNode("Variables[@Position=\"Left\"]");

            xmlNodeVariablesTop.Attributes["Position"].Value = "Left";
            xmlNodeVariablesLeft.Attributes["Position"].Value = "Top";

            XmlNode[] xmlNodesTop = xmlNodeVariablesTop.SelectNodes("//Variable[@Position=\"Top\"]").ToArray();
            XmlNode[] xmlNodesLeft = xmlNodeVariablesLeft.SelectNodes("//Variable[@Position=\"Left\"]").ToArray();

            foreach (XmlNode xmlNodeVariableTop in xmlNodesTop)
            {
                xmlNodeVariableTop.Attributes["Position"].Value = "Left";
            }

            foreach (XmlNode xmlNodeVariableLeft in xmlNodesLeft)
            {
                xmlNodeVariableLeft.Attributes["Position"].Value = "Top";
            }

            xmlDocument.Save(fileName);
        }

        private void BindReportDisplayTypes(HttpContext context)
        {
            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            string fileNameViews = null;

            if (reportDefinition.LeftVariables.Find(x => x.VariableType == VariableType.Numeric) != null ||
                reportDefinition.TopVariables.Find(x => x.VariableType == VariableType.Numeric) != null)
            {
                fileNameViews = Path.Combine(
                   context.Request.PhysicalApplicationPath,
                   "App_Data",
                   "Views",
                   "Crosstable.xml"
               );
            }
            else if (reportDefinition.LeftVariables.Find(x => x.VariableType == VariableType.Text) != null ||
                reportDefinition.TopVariables.Find(x => x.VariableType == VariableType.Text) != null)
            {
                if (reportDefinition.LeftVariables.Count == 1 && reportDefinition.LeftVariables[0].VariableType == VariableType.Text)
                {
                    fileNameViews = Path.Combine(
                       context.Request.PhysicalApplicationPath,
                       "App_Data",
                       "Views",
                       "ViewsText.xml"
                   );
                }
                else
                {
                    fileNameViews = Path.Combine(
                       context.Request.PhysicalApplicationPath,
                       "App_Data",
                       "Views",
                       "Crosstable.xml"
                   );
                }
            }
            else if (reportDefinition.LeftVariables.Count > 0 && reportDefinition.LeftVariables[0].IsFake == false)
            {
                if (reportDefinition.LeftVariables[0].NestedLevels == 1)
                {
                    fileNameViews = Path.Combine(
                       context.Request.PhysicalApplicationPath,
                       "App_Data",
                       "Views",
                       "Views.xml"
                   );
                }
                else
                {
                    fileNameViews = Path.Combine(
                       context.Request.PhysicalApplicationPath,
                       "App_Data",
                       "Views",
                       "Views2.xml"
                   );
                }
            }
            else
            {
                fileNameViews = Path.Combine(
                   context.Request.PhysicalApplicationPath,
                   "App_Data",
                   "Views",
                   "Crosstable.xml"
               );
            }

            if (fileNameViews == null)
                return;

            WebUtilities.NavigationMenu navReporterViews = new WebUtilities.NavigationMenu(
                "navReporterViews",
                fileNameViews
            );

            navReporterViews.Render();

            context.Response.Write(navReporterViews.ToHtml());
        }

        private void PinToHomescreen(HttpContext context)
        {
            Guid idUser = Guid.Parse(context.Request.Params["IdSavedReport"].Substring(0, 36));
            Guid idReport = Guid.Parse(context.Request.Params["IdSavedReport"].Substring(36, 36));

            // Get the full path to the directory
            // where the user's reports are saved.
            string directoryName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedReports",
                Global.Core.ClientName,
                idUser.ToString(),
                idReport.ToString()
            );

            if (!Directory.Exists(directoryName))
            {
                string[] paths = Directory.GetDirectories(Path.Combine(context.Request.PhysicalApplicationPath,
                           "Fileadmin",
                           "SavedReports",
                           Global.Core.ClientName,
                           idUser.ToString()), "*", SearchOption.AllDirectories);

                if (paths.Where(x => x.ToLower().IndexOf(idReport.ToString().ToLower()) > -1) != null)
                {
                    directoryName = paths.Where(x => x.ToLower().IndexOf(idReport.ToString().ToLower()) > -1).FirstOrDefault();
                }
            }

            // Get the report definition information for the saved report.
            ReportDefinitionInfo savedReportInfo = new ReportDefinitionInfo(Path.Combine(
                directoryName,
                "Info.xml"
            ));

            string fileName = null;

            if (savedReportInfo.ActiveReport.HasValue)
            {
                fileName = Path.Combine(
                    directoryName,
                    savedReportInfo.ActiveReport.Value + ".xml"
                );

                if (!File.Exists(fileName))
                {
                    string[] files = Directory.GetFiles(directoryName).Where(x => new FileInfo(x).Name != "Info.xml").ToArray();

                    if (files.Length > 0)
                        fileName = files[0];
                    else
                        fileName = null;
                }
            }
            else
            {
                string[] files = Directory.GetFiles(directoryName).Where(x => new FileInfo(x).Name != "Info.xml").ToArray();

                if (files.Length > 0)
                    fileName = files[0];
                else
                    fileName = null;
            }

            if (fileName == null)
                return;


            // Build the path to the user's home screen definition file.
            string fileNameHomescreen = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "HomeDefinitions",
                Global.Core.ClientName,
                Global.IdUser.Value + ".xml"
            );

            // Check if the user has a homescreen defined.
            if (!File.Exists(fileName))
            {
                // Build the path to the client's home screen definition file.
                string fileNameClient = Path.Combine(
                    context.Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "HomeDefinitions",
                    Global.Core.ClientName + ".xml"
                );

                // Check if the client has a homescreen defined.
                if (!File.Exists(fileNameClient))
                {
                    // Build the full path to the default homescreen definition.
                    string fileNameDefault = Path.Combine(
                        context.Request.PhysicalApplicationPath,
                        "App_Data",
                        "Homescreen.xml"
                    );

                    File.Copy(
                        fileNameDefault,
                        fileNameClient
                    );
                }

                File.Copy(
                    fileNameClient,
                    fileNameHomescreen
                );
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileNameHomescreen);

            XmlNode xmlNodeChartModule = xmlDocument.SelectSingleNode("//Module[@Name='Chart']");

            if (xmlNodeChartModule == null)
                return;

            if (xmlNodeChartModule.Attributes["Source"] != null)
                xmlNodeChartModule.Attributes["Source"].Value = fileName;
            else
                xmlNodeChartModule.AddAttribute("Source", fileName);

            xmlDocument.Save(fileNameHomescreen);
        }


        private void GetUserImage(HttpContext context)
        {
            Guid idUser = Guid.Parse(context.Request.Params["IdUser"]);

            string result = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Images",
                "Icons",
                "Navigation",
                "UserInfo.png"
            );

            string directoryNameUserImage = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "UserImages"
            );

            if (!Directory.Exists(directoryNameUserImage))
                Directory.CreateDirectory(directoryNameUserImage);

            string fileNameUserImage = Path.Combine(
                directoryNameUserImage,
                idUser + ".png"
            );

            if (File.Exists(fileNameUserImage))
            {
                result = fileNameUserImage;
            }

            Bitmap bitmap = new Bitmap(result);

            MemoryStream memoryStream = new MemoryStream();

            bitmap.Save(memoryStream, ImageFormat.Png);
            context.Response.BinaryWrite(memoryStream.GetBuffer());

            context.Response.Write(memoryStream.ToArray());
        }


        private void ChangeReportTab(HttpContext context)
        {
            // Get the file name of the report
            // from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            string directoryName;

            if (HttpContext.Current.Session["ActiveSavedReport"] == null)
            {
                // Get the full path to the directory
                // where the user's reports are saved.
                directoryName = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "ReportDefinitions",
                    Global.Core.ClientName,
                    Global.User.Id.ToString()
                );
            }
            else
            {
                // Get the full path to the directory
                // where the saved report is stored.
                directoryName = HttpContext.Current.Session["ActiveSavedReport"].ToString();
            }

            HttpContext.Current.Session["ReportDefinition"] = Path.Combine(
                directoryName,
                fileName
            );

            ReportDefinitionInfo info = new ReportDefinitionInfo(Path.Combine(
                directoryName,
                "Info.xml"
            ));

            info.ActiveReport = Guid.Parse(fileName.Replace(".xml", ""));

            info.Save();

            XmlDocument document = new XmlDocument();
            document.Load(HttpContext.Current.Session["ReportDefinition"].ToString());

            string displayType = document.SelectSingleNode("/Report/Settings/Setting[@Name='DisplayType']").InnerText;
            context.Response.Write(displayType);
        }

        private void CreateNewReportTab(HttpContext context)
        {
            string directoryName;

            if (HttpContext.Current.Session["ActiveSavedReport"] == null)
            {
                // Get the full path to the directory
                // where the user's reports are saved.
                directoryName = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "ReportDefinitions",
                    Global.Core.ClientName,
                    Global.User.Id.ToString()
                );
            }
            else
            {
                // Get the full path to the directory
                // where the saved report is stored.
                directoryName = HttpContext.Current.Session["ActiveSavedReport"].ToString();
            }

            string fileName = Path.Combine(
                directoryName,
                Guid.NewGuid().ToString() + ".xml"
            );

            string fileNameWorkflow = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "ReportingWorkflows",
                Global.Core.ClientName + ".xml"
            );

            string fileNameWeighting = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "WeightingDefaults",
                Global.Core.ClientName + ".xml"
            );

            if (!File.Exists(fileNameWeighting))
                fileNameWeighting = null;

            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                fileNameWorkflow,
                fileNameWeighting,
                Global.HierarchyFilters[fileName, false],
                Global.UserDefaults["ReportDefinitionSettings"]
            );

            string name = string.Format(
                Global.LanguageManager.GetText("NewReport"),
                ""
            ).Trim();
            bool foundUniqueName = true;
            int i = 2;

            while (true)
            {
                // Run through all tabs.
                foreach (string file in Directory.GetFiles(directoryName))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(file);

                    if (document.DocumentElement.Attributes["Name"] == null)
                        continue;

                    if (document.DocumentElement.Attributes["Name"].Value == name)
                    {
                        name = string.Format(
                            Global.LanguageManager.GetText("NewReport"),
                            i++
                        ).Trim();

                        foundUniqueName = false;
                        break;
                    }
                }

                if (foundUniqueName)
                    break;

                foundUniqueName = true;
            }

            reportDefinition.XmlDocument.DocumentElement.AddAttribute("Name", name);

            reportDefinition.Save();

            context.Response.Write((new FileInfo(fileName)).Name);
        }

        private void DuplicateReportTab(HttpContext context)
        {
            string fileName = (string)HttpContext.Current.Session["ReportDefinition"];
            string directoryName = Path.GetDirectoryName(fileName);

            string newFileName = Path.Combine(
                directoryName,
                Guid.NewGuid() + ".xml"
            );

            File.Copy(fileName, newFileName);

            XmlDocument document = new XmlDocument();
            document.Load(newFileName);

            if (document.DocumentElement.Attributes["Name"].Value.Contains(" - Copy"))
            {
                int number;

                string[] split = document.DocumentElement.Attributes["Name"].Value.Split(new string[] {
                    " - Copy"
                }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 1 || int.TryParse(split[1], out number) == false)
                    number = 1;

                document.DocumentElement.Attributes["Name"].Value =
                    split[0] + " - Copy " + (number + 1);
            }
            else
            {
                document.DocumentElement.Attributes["Name"].Value += " - Copy";
            }

            document.DocumentElement.Attributes["Name"].Value = GetUniqueReportName(document.DocumentElement.Attributes["Name"].Value);

            document.Save(newFileName);

            context.Response.Write((new FileInfo(newFileName)).Name);
        }

        private string GetUniqueReportName(string name)
        {
            string fileName = (string)HttpContext.Current.Session["ReportDefinition"];
            string directoryName = Path.GetDirectoryName(fileName);


            bool foundUniqueName = true;
            int i = 2;

            while (true)
            {
                // Run through all tabs.
                foreach (string file in Directory.GetFiles(directoryName))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(file);

                    if (document.DocumentElement.Attributes["Name"] == null)
                        continue;

                    if (document.DocumentElement.Attributes["Name"].Value == name)
                    {

                        if (Char.IsNumber(Convert.ToChar(name.Substring(name.Length - 1))))
                        {
                            name = name.Remove(name.Length - 1) + (Int32.Parse(name.Substring(name.Length - 1)) + 1);
                        }
                        else
                        {
                            name = name + " 1";
                        }
                        foundUniqueName = false;
                        break;
                    }
                }

                if (foundUniqueName)
                    break;

                foundUniqueName = true;
            }
            return name;
        }

        private void DeleteReportTab(HttpContext context)
        {

            // Get the full path to the current report's definition file.
            string fileName1 = HttpContext.Current.Session["ReportDefinition"].ToString();

            LogReportDefinitionHistory(fileName1, true);


            string directoryName;

            if (HttpContext.Current.Session["ActiveSavedReport"] == null)
            {
                // Get the full path to the directory
                // where the user's reports are saved.
                directoryName = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "ReportDefinitions",
                    Global.Core.ClientName,
                    Global.User.Id.ToString()
                );
            }
            else
            {
                // Get the full path to the directory
                // where the saved report is stored.
                directoryName = HttpContext.Current.Session["ActiveSavedReport"].ToString();
            }

            // Get the file name of the report
            // from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            fileName = Path.Combine(
                directoryName,
                fileName
            );

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        private void ChangeReportTabName(HttpContext context)
        {
            string directoryName;

            if (HttpContext.Current.Session["ActiveSavedReport"] == null)
            {
                // Get the full path to the directory
                // where the user's reports are saved.
                directoryName = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "ReportDefinitions",
                    Global.Core.ClientName,
                    Global.User.Id.ToString()
                );
            }
            else
            {
                // Get the full path to the directory
                // where the saved report is stored.
                directoryName = HttpContext.Current.Session["ActiveSavedReport"].ToString();
            }

            // Get the file name of the report
            // from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            fileName = Path.Combine(
                directoryName,
                fileName
            );

            // Get the new name for the report tab
            // from the http request's parameters.
            string name = context.Request.Params["Name"];

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);

            if (xmlDocument.DocumentElement.Attributes["Name"] == null)
            {
                xmlDocument.DocumentElement.AddAttribute("Name", name);
            }
            else
            {
                xmlDocument.DocumentElement.Attributes["Name"].Value = name;
            }

            xmlDocument.Save(fileName);

            context.Response.Write(name);
        }


        private void SetContentWidth(HttpContext context)
        {
            int value;

            if (int.TryParse(context.Request.Params["Value"], out value))
            {
                HttpContext.Current.Session["ContentWidth"] = value;
            }
        }

        private void SetContentHeight(HttpContext context)
        {
            int value;

            if (int.TryParse(context.Request.Params["Value"], out value))
            {
                HttpContext.Current.Session["ContentHeight"] = value;
            }
        }


        private void UploadFile(HttpContext context)
        {
            Stream stream = context.Request.GetBufferedInputStream();


            byte[] buffer = new byte[stream.Length];

            while (stream.Read(buffer, (int)stream.Position, 1) != -1) ;

            string fileName = Path.Combine(
                HttpContext.Current.Session["LinkCloudSelectedDirectory"].ToString(),
                context.Request.Params["FileName"]
            );

            File.WriteAllBytes(
                fileName,
                buffer
            );
        }


        private void SetOverallWeightingVariable(HttpContext context)
        {
            // Get the weighting variable from the http request's parameters.
            Guid idWeightingVariable = Guid.Parse(
                context.Request.Params["WeightingVariable"]
            );

            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Check if none is selected as overall weighting variable.
            if (idWeightingVariable == new Guid())
                reportDefinition.WeightingFilters.DefaultWeighting = null;
            else
                reportDefinition.WeightingFilters.DefaultWeighting = idWeightingVariable;

            reportDefinition.ClearData();

            // Save the report definition.
            reportDefinition.Save();
        }

        private void RemoveWeightingVariable(HttpContext context)
        {
            // Get the id of the weighting filter's category.
            Guid idCategory = Guid.Parse(
                context.Request.Params["IdCategory"]
            );

            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            reportDefinition.WeightingFilters.Delete(idCategory);

            reportDefinition.ClearData();

            reportDefinition.Save();
        }


        private void SearchTaxonomyVariables(HttpContext context)
        {
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            /*if (!reportDefinition.DataCheck.HasEntries)
                reportDefinition.DataCheck.Create();*/

            // Get the search expression from the http request's parameters.
            string searchExpression = context.Request.Params["Expression"];

            int limit = 10;

            searchExpression = searchExpression.Trim();

            List<TaxonomyVariableLabel> taxonomyVariableLabels = new List<TaxonomyVariableLabel>();

            // Get all variable labels that matches the search expression.

            if (searchExpression.Trim() != "")
            {
                taxonomyVariableLabels.AddRange(Global.Core.TaxonomyVariableLabels.Search("Label", searchExpression));

                if (taxonomyVariableLabels.Count < limit)
                    taxonomyVariableLabels.AddRange(Global.Core.TaxonomyVariableLabels.Search("Label", searchExpression + " %"));

                if (taxonomyVariableLabels.Count < limit)
                    taxonomyVariableLabels.AddRange(Global.Core.TaxonomyVariableLabels.Search("Label", "% " + searchExpression));
            }

            if (taxonomyVariableLabels.Count < limit)
                taxonomyVariableLabels.AddRange(Global.Core.TaxonomyVariableLabels.Search("Label", "%" + searchExpression + "%"));

            List<Guid> processedTaxonomyVariables = new List<Guid>();

            // Create a new string builder for the result json script.
            StringBuilder result = new StringBuilder();

            result.Append("{ \"SearchExpression\": \"" + searchExpression + "\",\"Variables\": [");

            int c = 0;
            // Run through all variable labels found.
            foreach (TaxonomyVariableLabel taxonomyVariableLabel in taxonomyVariableLabels)
            {
                if (processedTaxonomyVariables.Contains(taxonomyVariableLabel.IdTaxonomyVariable))
                    continue;

                // Get the variable type variable label's variable.
                //VariableType variableType = (VariableType)Global.Core.Variables.GetValue("Type", "Id", taxonomyVariableLabel.IdTaxonomyVariable);

                //if (variableType == VariableType.Text)
                //continue;

                string[] names = new string[] { "Id", "Label", "HasData", "IsTaxonomy" };
                object[] values = new object[] { taxonomyVariableLabel.IdTaxonomyVariable, taxonomyVariableLabel.Label, true, true };

                // Render the variable as json script and
                // add it to the result's string builder.
                result.Append(taxonomyVariableLabel.ToJson(
                    names,
                    values
                ) + ",");

                processedTaxonomyVariables.Add(taxonomyVariableLabel.IdTaxonomyVariable);

                if (c++ > limit)
                    break;
            }

            string resultString = result.ToString();

            if (c > 0)
                resultString = resultString.Remove(resultString.Length - 1, 1);

            resultString += "] }";

            // Write the content of the result's
            // string builder to the http response.
            context.Response.Write(resultString);

            // Set the content type of the http response to plain text.
            context.Response.ContentType = "text/plain";
        }

        public string LatestSearchExpression
        {
            get
            {
                if (HttpContext.Current.Session["LatestSearchExpression"] == null)
                    HttpContext.Current.Session["LatestSearchExpression"] = "";

                return (string)HttpContext.Current.Session["LatestSearchExpression"];
            }
            set
            {
                HttpContext.Current.Session["LatestSearchExpression"] = value;
            }
        }

        private void SearchVariables(HttpContext context)
        {

            string Expression ="";         
            if (!File.Exists(context.Request.Params["Source"]))
                return;

            DataCheck dataCheck = new DataCheck(context.Request.Params["Source"]);

            HierarchyFilter hierarchyFilter = Global.HierarchyFilters[context.Request.Params["Source"]];

            if (!hierarchyFilter.IsLoaded)
            {
                hierarchyFilter.Load();
            }

            bool dataCheckEnabled = false;

            /*if (context.Request.Params["EnableDataCheck"] != null)
                dataCheckEnabled = bool.Parse(context.Request.Params["EnableDataCheck"]);*/

            int idLanguage = 2057;

            if (context.Request.Params["IdLanguage"] != null)
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);


            StringBuilder taxVarLbls = new StringBuilder();
            StringBuilder taxVarTempLbls = new StringBuilder();
            dynamic dynJson = JsonConvert.DeserializeObject(context.Request.Params["Expression"]);
            //var expressionArray = new object[dynJson.Count * 4];
            int idx = 0;
            foreach (var item in dynJson)
            {
                if (idx == 0) { Expression = Convert.ToString(item.apiText); }
                taxVarLbls.Append(" Label like '" + item.apiText + "' OR");
                taxVarTempLbls.Append(" Label like '" + Convert.ToString(item.apiText) + "%" + "' OR");
                taxVarTempLbls.Append(" Label like '" + "%" + Convert.ToString(item.apiText) + "' OR");
                taxVarTempLbls.Append(" Label like '" + "%" + Convert.ToString(item.apiText) + "%" + "' OR");
                idx++;
            }


            // Get the search expression from the http request's parameters.
            string searchExpression = Expression;
            this.LatestSearchExpression = searchExpression;
            int limit = 50;
            searchExpression = searchExpression.Trim();

            Guid? idChapter = null;
            Guid _idChapter;

            if (Guid.TryParse(context.Request.Params["IdChapter"], out _idChapter))
            {
                idChapter = _idChapter;
            }                      

            taxVarTempLbls.Remove(taxVarTempLbls.Length - 2, 2);
            taxVarLbls.Remove(taxVarLbls.Length - 2, 2);

            // Create a new string builder for the result json script.
            StringBuilder result = new StringBuilder();

            result.Append(searchExpression + "###SPLIT###");

            Dictionary<Guid, List<object[]>> taxonomyVariableLabels = new Dictionary<Guid, List<object[]>>();
            List<object[]> variableLabels = new List<object[]>();

            taxonomyVariableLabels = Global.Core.TaxonomyVariableLabels.ExecuteReaderDict<Guid>(            
                "SELECT DISTINCT IdTaxonomyVariable, Label FROM TaxonomyVariableLabels WHERE "+ taxVarLbls.ToString()+" ORDER BY Label",
                new object[] { searchExpression, searchExpression + "%", "%" + searchExpression, "%" + searchExpression + "%" }
            );
            Dictionary<Guid, List<object[]>> taxonomyVariableLabelsTemp = Global.Core.TaxonomyVariableLabels.ExecuteReaderDict<Guid>(               
                "SELECT DISTINCT IdTaxonomyVariable, Label FROM TaxonomyVariableLabels WHERE "+ taxVarTempLbls + " ORDER BY Label",
                new object[] { searchExpression, searchExpression + "%", "%" + searchExpression, "%" + searchExpression + "%" }
            );
            foreach (Guid idTaxonomyVariable in taxonomyVariableLabelsTemp.Keys)
            {
                if (taxonomyVariableLabels.ContainsKey(idTaxonomyVariable))
                    continue;

                taxonomyVariableLabels.Add(idTaxonomyVariable, taxonomyVariableLabelsTemp[idTaxonomyVariable]);
            }

            List<Guid> processedTaxonomyVariables = new List<Guid>();

            string tempDirectoryName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                "VariableSelector",
                HttpContext.Current.Session.SessionID
            );

            if (!Directory.Exists(tempDirectoryName))
                Directory.CreateDirectory(tempDirectoryName);

            // Check if the data check is enabled.
            if (dataCheckEnabled)
            {
                List<Task> tasks = new List<Task>();

                Data filter = dataCheck.Filter;

                // Run through all variable labels found.
                foreach (Guid idTaxonomyVariable in taxonomyVariableLabels.Keys)
                {
                    // Check if the variable has data with the current filter selection.
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            dataCheck.HasData(idTaxonomyVariable, filter);
                        }
                        catch { }
                    }));
                }

                Task.WaitAll(tasks.ToArray());
            }

            int c = 0;
            // Run through all variable labels found.
            foreach (Guid idVariable in taxonomyVariableLabels.Keys)
            {
                object[] taxonomyVariableLabel = taxonomyVariableLabels[idVariable][0];

                if (!hierarchyFilter.TaxonomyVariables.ContainsKey(idVariable))
                    continue;

                if (idChapter.HasValue && taxonomyVariableLabel.Length == 2)
                {
                    Guid idTaxonomyChapter = (Guid)Global.Core.TaxonomyVariables.GetValue(
                        "IdTaxonomyChapter",
                        "Id",
                        (Guid)taxonomyVariableLabel[0]
                    );

                    if (idChapter.Value != idTaxonomyChapter)
                        continue;
                }

                if (this.LatestSearchExpression != searchExpression)
                    return;

                if (processedTaxonomyVariables.Contains(idVariable))
                    continue;

                bool hasData = true;

                // Check if the data check is enabled.
                if (dataCheckEnabled)
                {
                    try
                    {
                        // Check if the variable has data with the current filter selection.
                        hasData = dataCheck.HasData((Guid)taxonomyVariableLabel[0]);
                    }
                    catch { }
                }

                string tempFileName = Path.Combine(
                    tempDirectoryName,
                    idVariable + ".xml"
                );

                Dictionary<Guid, List<object[]>> variableTypes = Global.Core.TaxonomyVariables.ExecuteReaderDict<Guid>(
                    "SELECT [Id], [Type] FROM TaxonomyVariables",
                    new object[] { }
                );

                VariableType variableType = (VariableType)variableTypes[idVariable][0][1];

                /*VariableType variableType = (VariableType)(int)Global.Core.TaxonomyVariables.GetValue(
                    "Type",
                    "Id",
                    idVariable
                );*/

                if (!File.Exists(tempFileName))
                {
                    WriteVariableSelectorTempFile(
                        tempFileName,
                        idVariable,
                        variableType,
                        idLanguage,
                        dataCheck
                    );
                }

                VariableSelector1.Classes.VariableSelector variableSelector = new VariableSelector1.Classes.VariableSelector(
                    idLanguage,
                    tempFileName,
                    "Variable",
                    false
                );
                variableSelector.Settings.Dragable = true;
                variableSelector.HasData = hasData;
                variableSelector.Attributes.Add("VariableType", variableType.ToString());

                variableSelector.Render();

                /*if(variableSelector.ChangesMade)
                    variableSelector.XmlNode.OwnerDocument.Save(variableSelector.Source);*/

                result.Append(variableSelector.ToHtml());

                processedTaxonomyVariables.Add(idVariable);

                if (c++ > limit)
                    break;
            }

            if (processedTaxonomyVariables.Count == 0)
            {
                // Get all variable labels that matches the search expression.
                variableLabels = Global.Core.VariableLabels.ExecuteReader(string.Format(
                    "SELECT DISTINCT IdVariable, Label FROM VariableLabels WHERE (Label like '{0}' OR Label like '{0}%' OR Label like '%{0}' OR Label like '%{0}%') ORDER BY Label",
                    searchExpression.Trim(),
                    Global.IdUser.Value
                ));

                List<Guid> processedVariables = new List<Guid>();

                // Run through all variable labels found.
                foreach (object[] variableLabel in variableLabels)
                {
                    Guid idVariable = (Guid)variableLabel[0];

                    if (!hierarchyFilter.Variables.ContainsKey(idVariable))
                        continue;

                    if (processedVariables.Contains(idVariable))
                        continue;

                    // Get the variable type variable label's variable.
                    VariableType variableType = (VariableType)Global.Core.Variables.GetValue("Type", "Id", idVariable);

                    //if (variable.Type != VariableType.Single && variable.Type != VariableType.Multi)
                    /*if (variableType == VariableType.Text)
                        continue;*/

                    string tempFileName = Path.Combine(
                        tempDirectoryName,
                        idVariable + ".xml"
                    );

                    if (!File.Exists(tempFileName))
                    {
                        // Create a new string builder that contains the
                        // xml string for the variable to add.
                        StringBuilder xmlBuilder = new StringBuilder();

                        // Add the variable opening tag to the xml builder.
                        xmlBuilder.Append(string.Format(
                            "<{0} Id=\"{1}\" Scale=\"{2}\" IsTaxonomy=\"{3}\" Type=\"{4}\" Name=\"{5}\" Label{6}=\"{7}\" IsFake=\"False\">",
                            "Variable",
                            idVariable,
                            false,
                            false,
                            (int)variableType,
                            HttpUtility.HtmlEncode(Global.Core.Variables.GetValue("Name", "Id", idVariable)),
                            idLanguage,
                            HttpUtility.HtmlEncode(Global.Core.VariableLabels.GetValue(
                                "Label",
                                new string[] { "IdVariable", "IdLanguage" },
                                new object[] { idVariable, idLanguage }
                            ))
                        ));

                        if (variableType == VariableType.Text)
                        {

                            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                                Global.Core,
                                null
                            );

                            List<string> categories = storageMethod.GetTextAnswers(
                                idVariable,
                                false,
                                dataCheck.Filter,
                                false
                            );

                            int i = 0;
                            // Run through all categories.
                            foreach (string category in categories)
                            {
                                // Add the xml tag for the category to the xml builder.
                                xmlBuilder.Append(string.Format(
                                    "<{0} Id=\"{1}\" Order=\"{2}\" Value=\"{3}\" Enabled=\"{4}\" Text=\"{5}\" Label2057=\"{5}\"></{0}>",
                                    "Category",
                                    Guid.NewGuid(),
                                    i,
                                    i++,
                                    true,
                                    HttpUtility.HtmlEncode(category)
                                ));
                            }
                        }
                        else
                        {
                            // Stores the category id's from the variable to add.
                            List<object[]> categories;

                            // Get the ids of the taxonomy variable's categories.
                            categories = Global.Core.Categories.GetValues(
                                new string[] { "Id", "Order", "Value", "Enabled" },
                                new string[] { "IdVariable" },
                                new object[] { idVariable },
                                "Order"
                            );

                            // Run through all category ids of the variable.
                            foreach (object[] category in categories)
                            {
                                // Add the xml tag for the category to the xml builder.
                                xmlBuilder.Append(string.Format(
                                    "<{0} Id=\"{1}\" Order=\"{2}\" Value=\"{3}\" Enabled=\"{4}\"></{0}>",
                                    "Category",
                                    ((Guid)category[0]).ToString(),
                                    category[1],
                                    category[2],
                                    category[3]
                                ));
                            }
                        }

                        // Close the variable tag.
                        xmlBuilder.Append(string.Format("</{0}>",
                            "Variable"
                        ));

                        File.WriteAllText(
                            tempFileName,
                            xmlBuilder.ToString()
                        );
                    }

                    VariableSelector1.Classes.VariableSelector variableSelector = new VariableSelector1.Classes.VariableSelector(
                        idLanguage,
                        tempFileName,
                        "Variable",
                        false
                    );
                    variableSelector.Settings.Dragable = true;
                    variableSelector.IsTaxonomy = false;
                    variableSelector.Attributes.Add("VariableType", variableType.ToString());

                    variableSelector.Render();

                    result.Append(variableSelector.ToHtml());

                    processedVariables.Add(idVariable);

                    if (c++ > limit)
                        break;
                }
            }

            dataCheck.Save();

            string resultString = result.ToString();

            /*if (c > 0)
                resultString = resultString.Remove(resultString.Length - 1, 1);

            resultString += "] }";*/

            // Write the content of the result's
            // string builder to the http response.
            context.Response.Write(resultString);

            // Set the content type of the http response to plain text.
            context.Response.ContentType = "text/plain";
        }

        private void WriteVariableSelectorTempFile(
            string tempFileName,
            Guid idVariable,
            VariableType variableType,
            int idLanguage,
            DataCheck dataCheck
        )
        {
            // Defines if the variable is a scale variable.
            bool isScale = false;

            // Check if the taxonomy variable is a scale variable.
            isScale = (bool)Global.Core.TaxonomyVariables.GetValue(
                "Scale",
                "Id",
                idVariable
            );

            string variableName = (string)Global.Core.TaxonomyVariables.GetValue("Name", "Id", idVariable);

            // Create a new string builder that contains the
            // xml string for the variable to add.
            StringBuilder xmlBuilder = new StringBuilder();

            // Add the variable opening tag to the xml builder.
            xmlBuilder.Append(string.Format(
                "<{0} Id=\"{1}\" Scale=\"{2}\" IsTaxonomy=\"{3}\" Type=\"{4}\" Name=\"{5}\" Label{6}=\"{7}\" IsFake=\"False\">",
                "Variable",
                idVariable,
                false,
                true,
                (int)variableType,
                HttpUtility.HtmlEncode(variableName),
                idLanguage,
                HttpUtility.HtmlEncode(Global.Core.TaxonomyVariableLabels.GetValue(
                    "Label",
                    new string[] { "IdTaxonomyVariable", "IdLanguage" },
                    new object[] { idVariable, idLanguage }
                ))
            ));

            if (variableType == VariableType.Text)
            {
                DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                    Global.Core,
                    null
                );

                List<string> categories = storageMethod.GetTextAnswers(
                    idVariable,
                    true,
                    dataCheck.Filter,
                    false
                );

                int i = 0;
                // Run through all categories.
                foreach (string category in categories)
                {
                    if (category.Trim() == "")
                        continue;

                    // Add the xml tag for the category to the xml builder.
                    xmlBuilder.Append(string.Format(
                        "<{0} Id=\"{1}\" Order=\"{2}\" Value=\"{3}\" Enabled=\"{4}\" Text=\"{5}\" Label2057=\"{5}\"></{0}>",
                        "TaxonomyCategory",
                        Guid.NewGuid(),
                        i,
                        i++,
                        true,
                        HttpUtility.HtmlEncode(category)
                    ));
                }
            }
            else
            {
                // Stores the category id's from the variable to add.
                List<object[]> categories;

                // Get the ids of the taxonomy variable's categories.
                categories = Global.Core.TaxonomyCategories.GetValues(
                    new string[] { "Id", "Order", "Value", "Enabled", "IsScoreGroup", "Name", "Equation"/*,
                                "(SELECT Label FROM TaxonomyCategoryLabels WHERE TaxonomyCategoryLabels.IdTaxonomyCategory=TaxonomyCategories.Id AND IdLanguage='" + idLanguage +"')" */
                                                                                                                                                                                },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { idVariable },
                    "Order"
                );

                // Run through all category ids of the variable.
                foreach (object[] category in categories)
                {
                    // Check if the category is a score group.
                    if ((bool)category[4] == true)
                    {
                        // Add the xml tag for the score group to the xml builder.
                        xmlBuilder.Append(string.Format(
                            "<{0} Id=\"{1}\" Persistent=\"True\" Order=\"{2}\" Value=\"{3}\" Enabled=\"{4}\" Equation=\"{5}\">",
                            "ScoreGroup",
                            ((Guid)category[0]).ToString(),
                            category[1],
                            category[2],
                            ((bool)category[3]),
                            category[6] != null ? HttpUtility.HtmlEncode((string)category[6]) : ""
                        ));

                        // Get all categories that are part of the score group.
                        List<object[]> scoreGroupCategories = Global.Core.TaxonomyCategoryLinks.GetValues(
                            new string[] { "IdTaxonomyCategory" },
                            new string[] { "IdScoreGroup" },
                            new object[] { category[0] }
                        );

                        // Run through all categories of the score group.
                        foreach (object[] scoreGroupLink in scoreGroupCategories)
                        {
                            object[] scoreGroupCategory = Global.Core.TaxonomyCategories.GetValues(
                                new string[] { "Id", "Order", "Value", "Enabled", "IsScoreGroup" },
                                new string[] { "Id" },
                                new object[] { scoreGroupLink[0] }
                            )[0];

                            // Add the xml tag for the category to the xml builder.
                            xmlBuilder.Append(string.Format(
                                "<{0} Id=\"{1}\" Persistent=\"True\" Order=\"{2}\" Value=\"{3}\" Enabled=\"{4}\"></{0}>",
                                "TaxonomyCategory",
                                ((Guid)scoreGroupCategory[0]).ToString(),
                                scoreGroupCategory[1],
                                scoreGroupCategory[2],
                                ((bool)scoreGroupCategory[3])
                            ));
                        }

                        xmlBuilder.Append("</ScoreGroup>");
                    }
                    else
                    {
                        // Add the xml tag for the category to the xml builder.
                        xmlBuilder.Append(string.Format(
                            "<{0} Id=\"{1}\" Persistent=\"True\" Order=\"{2}\" Value=\"{3}\" Enabled=\"{4}\"></{0}>",
                            "TaxonomyCategory",
                            ((Guid)category[0]).ToString(),
                            category[1],
                            category[2],
                            ((bool)category[3])
                        ));
                    }
                }
            }

            // Close the variable tag.
            xmlBuilder.Append(string.Format("</{0}>",
                "Variable"
            ));

            if (!Directory.Exists(Path.GetDirectoryName(tempFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(tempFileName));

            File.WriteAllText(
                tempFileName,
                xmlBuilder.ToString()
            );
        }

        private void SwitchVariableLocation(HttpContext context)
        {
            // Get the id of the variable to switch
            // from the http request's parameters.
            Guid idVariable = Guid.Parse(
                context.Request.Params["IdVariable"]
            );

            // Get the current location of the variable.
            string location = context.Request.Params["Location"];

            // Get the new location of the variable.
            string newLocation = context.Request.Params["NewLocation"];

            // Get the full path to the current report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new xml document for the report definition.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the report definition file into the xml document.
            xmlDocument.LoadXml(File.ReadAllText(fileName));

            // Select the xml node of the variable.
            XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Variables[@Position=\"{0}\"]/Variable[@Id=\"{1}\"]",
                location,
                idVariable
            ));

            // Check if the variable definition exists.
            if (xmlNode == null)
                return;

            XmlNode xmlNodeVariables = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Variables[@Position=\"{0}\"]",
                newLocation
            ));

            xmlNodeVariables.InnerXml += xmlNode.OuterXml;

            xmlNode.ParentNode.RemoveChild(xmlNode);

            xmlDocument.Save(fileName);
        }

        private void SelectVariable(HttpContext context)
        {
            // Get the id of the variable to select
            // from the http request's parameters.
            Guid idVariable = Guid.Parse(
                context.Request.Params["IdVariable"]
            );

            // Get the full path to the current report definition file.
            //string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();
            string fileName;

            if (context.Request.Params["Source"] != null)
                fileName = context.Request.Params["Source"];
            else
                fileName = context.Session["ReportDefinition"].ToString();

            HierarchyFilter hierarchyFilter = Global.HierarchyFilters[fileName];

            LogReportDefinitionHistory(fileName);

            string path = context.Request.Params["Path"];

            // Create a new xml document for the report definition.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the report definition file into the xml document.
            xmlDocument.LoadXml(File.ReadAllText(fileName));

            // Select the xml node that contains the variables.
            XmlNode xmlNode;
            XmlNode xmlNodeResults;

            UsageLogger logger = new UsageLogger(
                Global.Core.ClientName,
                Global.User
            );

            string variableName = "";

            variableName = (string)Global.Core.TaxonomyVariables.GetValue("Name", "Id", idVariable);

            if (variableName == null)
                variableName = (string)Global.Core.Variables.GetValue("Name", "Id", idVariable);

            logger.Log(
                UsageLogVariable.VariableUsed,
                variableName
            );

            /*logger.Log(
                UsageLogVariable.VariableUsedSide,
                path.Contains("\"WEIGHT\"") ? "Weight" : path.Contains("\"Top\"") ? "Top" : "Left"
            );*/

            if (path == "WEIGHT")
            {
                xmlNode = xmlDocument.DocumentElement.SelectSingleNode("WeightingVariables");

                if (xmlNode != null)
                {
                    if (xmlNode.Attributes["DefaultWeighting"] == null)
                        xmlNode.AddAttribute("DefaultWeighting", idVariable.ToString());
                    else
                        xmlNode.Attributes["DefaultWeighting"].Value = idVariable.ToString();

                    if (xmlNode.Attributes["IsTaxonomy"] == null)
                        xmlNode.AddAttribute("IsTaxonomy", bool.Parse(context.Request.Params["IsTaxonomy"]));
                    else
                        xmlNode.Attributes["IsTaxonomy"].Value = bool.Parse(context.Request.Params["IsTaxonomy"]).ToString();
                }
                else
                {
                    xmlDocument.DocumentElement.InnerXml += string.Format(
                        "<WeightingVariables DefaultWeighting=\"{0}\" IsTaxonomy=\"{1}\"></WeightingVariables>",
                        idVariable.ToString(),
                        bool.Parse(context.Request.Params["IsTaxonomy"])
                    );
                }

                // Select the results xml node.
                xmlNodeResults = xmlDocument.DocumentElement.SelectSingleNode("Results");

                // Check if results are loaded for the report definition.
                if (xmlNodeResults != null)
                {
                    xmlNodeResults.ParentNode.RemoveChild(xmlNodeResults);
                }

                xmlDocument.Save(fileName);

                return;
            }
            else if (path != null)
            {
                xmlNode = xmlDocument.SelectSingleNode(context.Request.Params["Path"]);
            }
            else
            {
                xmlNode = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                    "Variables[@Position=\"{0}\"]",
                    "Top"
                ));
            }

            if (xmlNode == null)
                return;

            if (xmlNode.ChildNodes.Count == 1 && xmlNode.FirstChild.Attributes["IsFake"] != null && bool.Parse(xmlNode.FirstChild.Attributes["IsFake"].Value) == true)
            {
                xmlNode.RemoveChild(xmlNode.FirstChild);
            }

            DataCheck dataCheck = new DataCheck(context.Request.Params["Source"]);

            string variableFileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                "VariableSelector",
                HttpContext.Current.Session.SessionID,
                idVariable + ".xml"
            );

            if (!File.Exists(variableFileName))
            {
                WriteVariableSelectorTempFile(
                    variableFileName,
                    idVariable,
                    (VariableType)Global.Core.TaxonomyVariables.GetValue("Type", "Id", idVariable),
                    2057,
                    dataCheck
                );
            }

            Guid idSelected;

            if (context.Request.Params["IdSelected"] != null && Guid.TryParse(context.Request.Params["IdSelected"], out idSelected))
            {
                XmlNode xmlNodeSelected = xmlNode.SelectSingleNode(string.Format(
                    "*[@Id=\"{0}\"]",
                    idSelected
                ));

                if (xmlNodeSelected != null)
                {
                    //xmlNodeSelected.ParentNode.RemoveChild(xmlNodeSelected);
                    xmlNode.InnerXml = xmlNode.InnerXml.Replace(xmlNodeSelected.OuterXml, File.ReadAllText(variableFileName));
                }
            }
            else
            {
                VariableType type = VariableType.Single;

                if (xmlNode.Attributes["Type"] != null)
                {
                    type = (VariableType)Enum.Parse(
                        typeof(VariableType),
                        xmlNode.Attributes["Type"].Value
                    );
                }

                if (type == VariableType.Numeric)
                    return;

                if (xmlNode.SelectSingleNode("Variable[@Id=\"" + idVariable + "\"]") != null)
                    return;

                // Add the contents of the xml builder to the xml node.
                xmlNode.InnerXml += File.ReadAllText(variableFileName);
            }


            // Select the results xml node.
            xmlNodeResults = xmlDocument.DocumentElement.SelectSingleNode("Results");

            // Check if results are loaded for the report definition.
            if (xmlNodeResults != null)
            {
                xmlNodeResults.ParentNode.RemoveChild(xmlNodeResults);
            }

            // Save the report definition.
            xmlDocument.Save(fileName);
        }

        private void DeSelectVariable(HttpContext context)
        {
            // Get the id of the variable to select
            // from the http request's parameters.
            Guid idVariable = Guid.Parse(
                context.Request.Params["IdVariable"]
            );

            // Get the full path to the current report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new xml document for the report definition.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the report definition file into the xml document.
            xmlDocument.LoadXml(File.ReadAllText(fileName));

            // Select the xml node that contains the variables.
            XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Variables[@Position=\"{0}\"]/Variable[@Id=\"{1}\"]",
                "Top",
                idVariable
            ));

            if (xmlNode != null)
            {
                xmlNode.ParentNode.RemoveChild(xmlNode);
            }

            // Save the report definition.
            xmlDocument.Save(fileName);
        }


        private void GetDataAggregationProgress(HttpContext context)
        {
            string result = "";

            if (HttpContext.Current.Session["DataAggregationStatus"] != null)
                result += Global.LanguageManager.GetText(((DataAggregationStatus)HttpContext.Current.Session["DataAggregationStatus"]).ToString());
            else
                result += Global.LanguageManager.GetText((DataAggregationStatus.RenderingTable).ToString());

            result += "|";

            if (HttpContext.Current.Session["DataAggregationProgress"] != null)
                result += HttpContext.Current.Session["DataAggregationProgress"].ToString();
            else
                result += "0";

            context.Response.Write(result);
            context.Response.ContentType = "text/plain";
        }

        private void CombineScores(HttpContext context)
        {
            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            LogReportDefinitionHistory(fileName);

            // Create a new report definition by the file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Get the id of the first category to
            // combine from the http request's paramters.
            Guid idCategory = Guid.Parse(
                context.Request.Params["IdCategory"]
            );

            // Get the id of the second category to
            // combine from the http request's paramters.
            Guid idCategory2 = Guid.Parse(
                context.Request.Params["IdCategory2"]
            );

            // Get the type of the first category.
            string categoryType = Global.Core.Categories.GetValue("Id", "Id", idCategory) == null ? "TaxonomyCategory" : "Category";

            // Get the type of the second category to
            // combine from the http request's paramters.
            string categoryType2 = Global.Core.Categories.GetValue("Id", "Id", idCategory2) == null ? "TaxonomyCategory" : "Category";

            // Get the name of the scale group
            // from the http request's paramters.
            string name = context.Request.Params["Name"];

            // Get the xPath to the first category's definition xml node.
            string xPath = context.Request.Params["XPath"];

            XmlNode xmlNodeCategory = reportDefinition.XmlDocument.SelectSingleNode(xPath);
            XmlNode xmlNodeCategory2 = xmlNodeCategory.ParentNode.SelectSingleNode("*[@Id=\"" + idCategory2 + "\"]");

            if (xmlNodeCategory2 == null)
                return;

            if (xmlNodeCategory.Name == "ScoreGroup")
            {
                /*xmlNodeCategory.InnerXml += string.Format(
                    "<{0} Id=\"{1}\"></{0}>",
                    categoryType2,
                    idCategory2
                );*/
                xmlNodeCategory.InnerXml += xmlNodeCategory2.OuterXml;
            }
            else
            {
                XmlNode xmlNode = reportDefinition.XmlDocument.CreateElement("ScoreGroup");
                xmlNode.AddAttribute("Id", Guid.NewGuid());
                xmlNode.AddAttribute("Name", name);

                /*xmlNode.InnerXml = string.Format(
                    "<{0} Id=\"{1}\"></{0}>",
                    categoryType,
                    idCategory
                ) + string.Format(
                    "<{0} Id=\"{1}\"></{0}>",
                    categoryType2,
                    idCategory2
                );*/
                xmlNode.InnerXml = xmlNodeCategory.OuterXml + xmlNodeCategory2.OuterXml;

                xmlNodeCategory.ParentNode.InsertBefore(xmlNode, xmlNodeCategory);
            }

            reportDefinition.ClearData();

            reportDefinition.Save();
        }

        private void RemoveVariable(HttpContext context)
        {
            // Get the xPath to the variable to delete
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            LogReportDefinitionHistory(fileName);

            // Create a new report definition by the file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Select the xml node of the score to delete.
            XmlNode xmlNode = reportDefinition.XmlDocument.SelectSingleNode(xPath);

            // Delete the variable's definition xml node.
            xmlNode.ParentNode.RemoveChild(xmlNode);

            reportDefinition.ClearData();

            // Save the report definition's xml document.
            reportDefinition.Save();
        }

        private void RemoveScore(HttpContext context)
        {
            // Get the xPath to the score to delete
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            LogReportDefinitionHistory(fileName);

            // Create a new report definition by the file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Select the xml node of the score to delete.
            XmlNode xmlNode = reportDefinition.XmlDocument.SelectSingleNode(xPath);

            // Check if the score scores the base value.
            /*if (xmlNode.Name == "ScoreGroup")
            {
                // Delete the score's definition xml node.
                xmlNode.ParentNode.RemoveChild(xmlNode);
            }
            else*/
            {
                if (xmlNode.Attributes["Enabled"] == null)
                    xmlNode.AddAttribute("Enabled", "False");
                else
                    xmlNode.Attributes["Enabled"].Value = "False";
            }

            //reportDefinition.ClearData();

            // Save the report definition's xml document.
            reportDefinition.Save();
        }

        private void RenameScore(HttpContext context)
        {
            // Get the xPath to the score to delete
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            LogReportDefinitionHistory(fileName);

            // Create a new report definition by the file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Select the xml node of the score to delete.
            XmlNode xmlNode = reportDefinition.XmlDocument.SelectSingleNode(xPath);

            string attribute = "Label" + reportDefinition.Settings.IdLanguage;

            // Edge:
            string value = context.Request.Params["Value"].Replace("\r\n\r\n", "<br />");
            // Chrome:
            value = value.Replace("\n", "<br />");

            if (xmlNode.Attributes[attribute] == null)
                xmlNode.AddAttribute(attribute, value);
            else
                xmlNode.Attributes[attribute].Value = value;

            //reportDefinition.ClearData();

            // Save the report definition's xml document.
            reportDefinition.Save();
        }

        /// <summary>
        /// Gets combining categories
        /// </summary>
        /// <param name="context"></param>
        private void GetCombiningCategories(HttpContext context)
        {
            // Get the xPath to the score to delete
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            StringBuilder sbResult = new StringBuilder();

            // Create a new report definition by the file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Select the xml node of the score to get categories.
            XmlNode xmlNode = reportDefinition.XmlDocument.SelectSingleNode(xPath.Substring(0, xPath.LastIndexOf("/")));
            XmlNode xmlScoreGroup = reportDefinition.XmlDocument.SelectSingleNode(xPath);

            XmlNodeList xmlGroupCategoires = null;
            if (xmlScoreGroup.Name == "ScoreGroup")
            {
                xmlGroupCategoires = xmlScoreGroup.SelectNodes("TaxonomyCategory");
            }

            XmlNodeList xmlNodeCategories = xmlNode.SelectNodes("TaxonomyCategory");
            sbResult.Append("{ \"Categories\": [");
            foreach (XmlNode xmlCategory in xmlNodeCategories)
            {
                bool isCategoryExist = false;
                string sourceCategory = xPath.Substring(xPath.LastIndexOf("/")).Split('"')[1];
                if (sourceCategory == xmlCategory.Attributes["Id"].Value)
                    continue;
                if (xmlGroupCategoires != null)
                    foreach (XmlNode xmlGroupCategory in xmlGroupCategoires)
                    {
                        if (xmlGroupCategory.Attributes["Id"].Value == xmlCategory.Attributes["Id"].Value)
                        {
                            isCategoryExist = true;
                            break;
                        }
                    }

                if (bool.Parse(xmlCategory.Attributes["HasValues"].Value))
                {
                    string[] names = new string[] { "Id", "Label", "HasData", "IsChecked" };
                    object[] values = new object[] { Guid.Parse(xmlCategory.Attributes["Id"].Value), xmlCategory.Attributes["Label2057"].Value, true, isCategoryExist };

                    sbResult.Append(this.ToJson(
                       names,
                       values
                   ) + ",");
                }
            }
            string resultString = sbResult.ToString();

            if (xmlNodeCategories.Count > 0)
                resultString = resultString.Remove(resultString.Length - 1, 1);

            resultString += "] }";

            context.Response.Write(resultString);
            context.Response.ContentType = "application/json";
        }

        /// <summary>
        /// Combine Multiple Categories
        /// </summary>
        /// <param name="context"></param>
        private void CombineMultipleCategories(HttpContext context)
        {
            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            LogReportDefinitionHistory(fileName);

            // Create a new report definition by the file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Get the name of the scale group
            // from the http request's paramters.
            string name = context.Request.Params["Name"];

            // Get the xPath to the first category's definition xml node.
            string xPath = context.Request.Params["XPath"];

            XmlNode xmlNodeCategory = reportDefinition.XmlDocument.SelectSingleNode(xPath);
            object[] categories = context.Request.Params["Categories"].Split(',');
            if (xmlNodeCategory.Name == "ScoreGroup")
            {
                string category = "";
                for (int i = 0; i < categories.Length; i++)
                {
                    category += xmlNodeCategory.ParentNode.SelectSingleNode("*[@Id=\"" + categories[i] + "\"]").OuterXml;
                }
                xmlNodeCategory.InnerXml = category;
            }
            else
            {
                XmlNode xmlNode = reportDefinition.XmlDocument.CreateElement("ScoreGroup");
                xmlNode.AddAttribute("Id", Guid.NewGuid());
                xmlNode.AddAttribute("Name", name);
                if (categories.Length > 0)
                    xmlNode.InnerXml = xmlNodeCategory.OuterXml;
                for (int i = 0; i < categories.Length; i++)
                {
                    xmlNode.InnerXml += xmlNodeCategory.ParentNode.SelectSingleNode("*[@Id=\"" + categories[i] + "\"]").OuterXml;
                }

                xmlNodeCategory.ParentNode.InnerXml += xmlNode.OuterXml;
            }
            reportDefinition.ClearData();

            reportDefinition.Save();
        }

        /// <summary>
        /// Gets the variables of a study.
        /// </summary>
        /// <param name="context">The current http context.</param>
        private void GetVariables(HttpContext context)
        {
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            ReportCalculator reportCalculator = new ReportCalculator(
                reportDefinition,
                Global.Core,
                HttpContext.Current.Session
            );

            // Get all variables of the study.
            //Variable[] variables = Global.Core.Variables.Get("IdStudy", idStudy);
            Variable[] variables = Global.Core.Variables.Get();

            // Create a new string builder for the result json script.
            StringBuilder result = new StringBuilder();

            result.Append("{ \"Variables\": [");

            // Run through all variables of the study.
            foreach (Variable variable in variables)
            {
                // Get the variable label for the variable.
                VariableLabel variableLabel = Global.Core.VariableLabels.GetSingle(
                    new string[] { "IdVariable", "IdLanguage" },
                    new object[] { variable.Id, reportDefinition.Settings.IdLanguage }
                );

                string[] names = new string[] { "Id", "Label", "HasData" };
                object[] values = new object[] { variable.Id, variableLabel.Label, true };

                // Check if the variable has data.
                if (!reportCalculator.HasData(variable.Id, false, reportCalculator.GetFilter()))
                {
                    values[2] = false;
                }

                // Render the variable as json script and
                // add it to the result's string builder.
                result.Append(variable.ToJson(
                    names,
                    values
                ) + ",");
            }

            string resultString = result.ToString();

            if (variables.Length > 0)
                resultString = resultString.Remove(resultString.Length - 1, 1);

            resultString += "] }";

            // Write the content of the result's
            // string builder to the http response.
            context.Response.Write(result.ToString());

            // Set the content type of the http response to plain text.
            context.Response.ContentType = "text/plain";
        }

        /// <summary>
        /// Gets the categories of a variable.
        /// </summary>
        /// <param name="context">The current http context.</param>
        private void GetCategories(HttpContext context)
        {
            // Create a new string builder containing the result json script.
            StringBuilder result = new StringBuilder();

            // Parse the variable's id from
            // the http request's parameters.
            Guid idVariable = Guid.Parse(
                context.Request.Params["IdVariable"]
            );

            // Parse the language id from the http request's parameters.
            int idLanguage = int.Parse(
                context.Request.Params["IdLanguage"]
            );

            bool isTaxonomy = bool.Parse(
                context.Request.Params["IsTaxonomy"]
            );

            // Get all categories by the variable's id.
            //Category[] categories = Global.Core.Categories.Get("IdVariable", idVariable).OrderBy(x => x.Order).ToArray();
            List<object[]> categories;

            if (!isTaxonomy)
            {
                categories = Global.Core.Categories.GetValues(
                    new string[] { "Id" },
                    new string[] { "IdVariable" },
                    new object[] { idVariable },
                    "Value"
                );
            }
            else
            {
                categories = Global.Core.TaxonomyCategories.GetValues(
                    new string[] { "Id" },
                    new string[] { "IdTaxonomyVariable", "Enabled" },
                    new object[] { idVariable, true },
                    "Value"
                );
            }

            result.Append("{ \"Categories\": [");

            // Run through all categories.
            foreach (object[] category in categories)
            {
                string labelText;

                if (!isTaxonomy)
                {
                    labelText = (string)Global.Core.CategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdCategory", "IdLanguage" },
                        new object[] { (Guid)category[0], idLanguage }
                    );
                }
                else
                {
                    labelText = (string)Global.Core.TaxonomyCategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyCategory", "IdLanguage" },
                        new object[] { (Guid)category[0], idLanguage }
                    );
                }

                labelText = PrepareLabel(labelText);

                string scoreType = "ReportDefinitionCategory";

                if (isTaxonomy)
                    scoreType = "ReportDefinitionTaxonomyCategory";

                string[] names = new string[] { "Id", "Label", "HasData", "ScoreType" };
                object[] values = new object[] { (Guid)category[0], labelText, true, scoreType };

                result.Append(this.ToJson(
                    names,
                    values
                ) + ",");
            }

            string resultString = result.ToString();

            if (categories.Count > 0)
                resultString = resultString.Remove(resultString.Length - 1, 1);

            resultString += "] }";

            context.Response.Write(resultString);
            context.Response.ContentType = "application/json";
        }

        private void GetTaxonomyCategories(HttpContext context)
        {
            // Create a new string builder containing the result json script.
            StringBuilder result = new StringBuilder();

            // Parse the variable's id from
            // the http request's parameters.
            Guid idVariable = Guid.Parse(
                context.Request.Params["IdVariable"]
            );

            // Parse the language id from the http request's parameters.
            int idLanguage = int.Parse(
                context.Request.Params["IdLanguage"]
            );

            // Get all categories by the variable's id.
            //Category[] categories = Global.Core.Categories.Get("IdVariable", idVariable).OrderBy(x => x.Order).ToArray();
            List<object[]> categories = Global.Core.TaxonomyCategories.GetValues(
                new string[] { "Id" },
                new string[] { "IdTaxonomyVariable" },
                new object[] { idVariable },
                "Value"
            );

            result.Append("{ \"Categories\": [");

            // Run through all categories.
            foreach (object[] category in categories)
            {
                string labelText = (string)Global.Core.TaxonomyCategoryLabels.GetValue(
                    "Label",
                    new string[] { "IdTaxonomyCategory", "IdLanguage" },
                    new object[] { (Guid)category[0], idLanguage }
                );

                string[] names = new string[] { "Id", "Label", "HasData" };
                object[] values = new object[] { (Guid)category[0], labelText, true };

                result.Append(this.ToJson(
                    names,
                    values
                ) + ",");
            }

            string resultString = result.ToString();

            if (categories.Count > 0)
                resultString = resultString.Remove(resultString.Length - 1, 1);

            resultString += "] }";

            context.Response.Write(resultString);
            context.Response.ContentType = "application/json";
        }


        private void GetSelectedWorkflowItems(HttpContext context)
        {
            string workflowSelection = context.Request.Params["WorkflowSelection"];
            string workflowSelectionSelector = context.Request.Params["WorkflowSelectionSelector"];

            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            List<Guid> selectedItems = reportDefinition.Workflow.Selections[workflowSelection].SelectionVariables[workflowSelectionSelector].Selector.SelectedItems;

            context.Response.Write(string.Join(",", selectedItems.ToArray()));
        }

        private void SelectWorkflowSelectorItem(HttpContext context)
        {
            // Get the full path to the current report's definition file.
            //string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();
            string fileName = context.Request.Params["Source"];

            string workflowSelection = context.Request.Params["WorkflowSelection"];
            string workflowSelectionVariable = context.Request.Params["WorkflowSelectionVariable"];
            string action = context.Request.Params["Action"];

            Guid idItem = Guid.Parse(
                context.Request.Params["IdItem"]
            );

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            //ReportDefinition reportDefinition = new ReportDefinition(Global.Core, fileName);
            Workflow workflow = new Workflow(
                Global.Core,
                fileName,
                document.DocumentElement.SelectSingleNode("Workflow"),
                "/Handlers/GlobalHandler.ashx",
                Global.HierarchyFilters[fileName]
            );

            bool isDefault = workflow.Selections[workflowSelection].SelectionVariables[workflowSelectionVariable].IsDefaultSelection;

            WorkflowSelectionSelector selector = workflow.Selections[workflowSelection].SelectionVariables[workflowSelectionVariable];

            if (action == "Select")
            {
                selector.Select(idItem);
            }
            else if (action == "DeSelect")
            {
                selector.DeSelect(idItem);
            }

            if (selector.GetType() == typeof(WorkflowSelectionHierarchy))
            {
                Global.HierarchyFilters[fileName].Hierarchies = selector.Selector.SelectedItems;
                Global.HierarchyFilters[fileName].IsLoaded = false;
            }
            else
            {
                //reportDefinition.ClearData();
                XmlNode xmlNodeResult = document.DocumentElement.SelectSingleNode("Results");

                if (xmlNodeResult != null)
                    xmlNodeResult.ParentNode.RemoveChild(xmlNodeResult);

                DataCheck dataCheck = new DataCheck(fileName);

                dataCheck.Clear();
            }

            //reportDefinition.Save();
            document.Save(fileName);
        }


        private void AddFilterScoreGroup(HttpContext context)
        {
            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Get the xPath of the filter score group
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Get the xml node of the score group.
            XmlNode xmlNodeScoreGroup = reportDefinition.XmlDocument.SelectSingleNode(xPath);

            // Check if the score group exists.
            if (xmlNodeScoreGroup == null)
                return;

            reportDefinition.FilterCategories[0].XmlNode.InnerXml += string.Format(
                "<Operator Id=\"{0}\" Type=\"OR\"></Operator>",
                Guid.NewGuid()
            );

            reportDefinition.FilterCategories[0].Parse();

            // Run through all category xml nodes of the score group.
            foreach (XmlNode xmlNodeCategory in xmlNodeScoreGroup.ChildNodes)
            {
                // Get the id of the category of the category's xml node attributes.
                Guid idCategory = Guid.Parse(
                    xmlNodeCategory.Attributes["Id"].Value
                );

                bool isTaxonomy = xmlNodeCategory.Name == "TaxonomyCategory" ? true : false;

                reportDefinition.FilterCategories[0].FilterCategoryOperators.Last().Add(idCategory, isTaxonomy);
            }

            reportDefinition.Save();
        }

        private void AddFilterCategory(HttpContext context)
        {
            // Get the full path of the current definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            if (string.IsNullOrEmpty(fileName))
                fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Get the id of the new filter category
            // from the http request's parameters.
            Guid idCategory = Guid.Parse(
                context.Request.Params["IdCategory"]
            );

            // Get the type of the category from
            // the http request's parameters.
            string type = context.Request.Params["Type"];

            // Get the xPath to the filter category operator
            // where the filter category should be added to.
            string xPath = context.Request.Params["XPath"];

            // Check if the category is a taxonomy category.
            bool isTaxonomy = type == "ReportDefinitionTaxonomyCategory";

            // Check if a filter category operator was set.
            if (xPath == null)
            {
                if (reportDefinition.FilterCategories[0].FilterCategoryOperators.Count == 0 &&
                    reportDefinition.FilterCategories[0].FilterCategories.Count == 0)
                {
                    reportDefinition.FilterCategories[0].XmlNode.InnerXml += string.Format(
                        "<Operator Id=\"{0}\" Type=\"OR\"></Operator>",
                        Guid.NewGuid()
                    );

                    reportDefinition.FilterCategories[0].Parse();
                }

                // Run through all filter category operators on root level.
                foreach (FilterCategoryOperator filterCategoryOperator in reportDefinition.FilterCategories)
                {
                    if (filterCategoryOperator.Add(idCategory, isTaxonomy))
                    {
                        reportDefinition.ClearData();
                        reportDefinition.Save();

                        return;
                    }
                }
            }

            FilterCategoryOperator _filterCategoryOperator = new FilterCategoryOperator(
                reportDefinition.WeightingFilters,
                reportDefinition.XmlDocument.SelectSingleNode(xPath),
                0,
                fileName
            );

            // Add the filter category to the filter
            // category operator's filter categories.
            _filterCategoryOperator.Add(idCategory, isTaxonomy);

            // Clear the report's data.
            reportDefinition.ClearData();

            // Clear the data check entries.
            DataCheck dataCheck = new DataCheck(fileName);

            dataCheck.Clear();

            // Save the report definition.
            reportDefinition.Save();

            context.Response.Write(Global.LanguageManager.GetText("FiltersApplied"));
            context.Response.ContentType = "text/plain";
        }

        private void AddFilterCategories(HttpContext context)
        {
            // Get the full path of the current definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            if (string.IsNullOrEmpty(fileName))
                fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Get the type of the category from
            // the http request's parameters.
            string type = context.Request.Params["Type"];

            // Get the xPath to the filter category operator
            // where the filter category should be added to.
            string xPath = context.Request.Params["XPath"];

            // Check if the category is a taxonomy category.
            bool isTaxonomy = type == "ReportDefinitionTaxonomyCategory";

            FilterCategoryOperator _filterCategoryOperator = new FilterCategoryOperator(
                reportDefinition.WeightingFilters,
                reportDefinition.XmlDocument.SelectSingleNode(xPath),
                0,
                fileName
            );

            foreach (string category in context.Request.Params["Categories"].Split(','))
            {
                if (category == "")
                    continue;

                // Add the filter category to the filter
                // category operator's filter categories.
                _filterCategoryOperator.Add(Guid.Parse(category), isTaxonomy);
            }

            // Clear the report's data.
            reportDefinition.ClearData();

            // Clear the data check entries.
            DataCheck dataCheck = new DataCheck(fileName);

            dataCheck.Clear();

            // Save the report definition.
            reportDefinition.Save();

            context.Response.Write(Global.LanguageManager.GetText("FiltersApplied"));
            context.Response.ContentType = "text/plain";
        }

        private void DeleteFilterCategory(HttpContext context)
        {
            // Get the id of the new filter category
            // from the http request's parameters.
            Guid idCategory = Guid.Parse(
                context.Request.Params["IdCategory"]
            );

            // Get the xPath to the filter category operator
            // where the filter category should be added to.
            string xPath = context.Request.Params["XPath"];

            // Get the full path of the current definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            FilterCategoryOperator filterCategoryOperator = new FilterCategoryOperator(
                reportDefinition.WeightingFilters,
                reportDefinition.XmlDocument.SelectSingleNode(xPath),
                0,
                fileName
            );

            // Add the filter category to the filter
            // category operator's filter categories.
            filterCategoryOperator.Delete(filterCategoryOperator.
                FilterCategories.Find(x => x.IdCategory == idCategory));

            // Clear the report's data.
            reportDefinition.ClearData();

            // Clear the data check entries.
            DataCheck dataCheck = new DataCheck(fileName);

            dataCheck.Clear();

            // Save the report definition.
            reportDefinition.Save();

            context.Response.Write(Global.LanguageManager.GetText("FiltersApplied"));
            context.Response.ContentType = "text/plain";
        }

        private void AddFilterCategoryOperator(HttpContext context)
        {
            // Get the xPath to the filter category operator
            // where the filter category should be added to.
            string xPath = context.Request.Params["XPath"];

            // Get the full path of the current session's report definition file.
            string fileName = context.Request.Params["FileName"];

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            FilterCategoryOperator filterCategoryOperator = new FilterCategoryOperator(
                reportDefinition.WeightingFilters,
                reportDefinition.XmlDocument.SelectSingleNode(xPath),
                0,
                fileName
            );

            // Add a new filter category operator
            // under the filter category operator.
            filterCategoryOperator.XmlNode.InnerXml += string.Format(
                "<Operator Id=\"{0}\" Type=\"AND\"></Operator>",
                Guid.NewGuid()
            );

            // Save the report definition.
            reportDefinition.Save();
        }

        private void DeleteFilterCategoryOperator(HttpContext context)
        {
            // Get the xPath to the filter category operator
            // where the filter category should be added to.
            string xPath = context.Request.Params["XPath"];

            // Get the full path of the current definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            XmlNode xmlNode = reportDefinition.XmlDocument.SelectSingleNode(xPath);

            FilterCategoryOperator filterCategoryOperatorParent = new FilterCategoryOperator(
                reportDefinition.WeightingFilters,
                xmlNode.ParentNode,
                0,
                fileName
            );

            FilterCategoryOperator filterCategoryOperator = new FilterCategoryOperator(
                reportDefinition.WeightingFilters,
                xmlNode,
                0,
                fileName
            );

            filterCategoryOperatorParent.Delete(filterCategoryOperator);

            // Clear the report's data.
            reportDefinition.ClearData();

            // Save the report definition.
            reportDefinition.Save();
        }


        private void SearchEquationMethods(HttpContext context)
        {
            // Create a new string builder that
            // builds the result JSON string.
            StringBuilder result = new StringBuilder();

            // Open the array that contains the methods.
            result.Append("[");

            // Get the search expression from
            // the http request's parameters.
            string expression = context.Request.Params["Expression"];

            // Create a new xml document that
            // contains the equation method definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the equation method
            // definition file into the xml document.
            document.Load(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "bin",
                "Resources",
                "EquationValidatorConfiguration.xml"
            ));

            // Run through all xml nodes that define a method.
            foreach (XmlNode xmlNode in document.DocumentElement.ChildNodes)
            {
                if (xmlNode.Attributes["Visible"] != null && bool.Parse(xmlNode.Attributes["Visible"].Value) == false)
                    continue;

                result.Append("{");

                result.Append(string.Format(
                    "\"Name\": \"{0}\", \"Parameters\": [",
                    xmlNode.InnerText
                ));

                // Check if there are parameters for the method defined.
                if (xmlNode.Attributes["Parameters"] != null)
                {
                    string[] parameters = xmlNode.Attributes["Parameters"].Value.Split(',');

                    // Run through all parameters of the method.
                    foreach (string parameter in parameters)
                    {
                        result.Append("{");

                        // Render the name and type of the
                        // parameter to the result JSON string.
                        result.Append(string.Format(
                            "\"Name\": \"{0}\", \"Type\": \"{1}\"",
                            parameter.Split(':')[0],
                            parameter.Split(':')[1]
                        ));

                        result.Append("},");
                    }

                    // Remove the last comma from the result JSON string.
                    if (parameters.Length != 0)
                        result = result.Remove(result.Length - 1, 1);
                }

                result.Append("]},");
            }

            // Remove the last comma from the result JSON string.
            if (document.DocumentElement.ChildNodes.Count != 0)
                result = result.Remove(result.Length - 1, 1);

            // Close the array that contains the methods.
            result.Append("]");

            // Write the contents of the result JSON
            // string builder to the http response.
            context.Response.Write(result.ToString());
        }

        private void RemoveFilterCategory(HttpContext context)
        {
            Guid idCategory = Guid.Parse(
                context.Request.Params["IdCategory"]
            );

            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            reportDefinition.RemoveFilterCategory(idCategory);

            reportDefinition.ClearData();

            // Clear the data check entries.
            DataCheck dataCheck = new DataCheck(fileName);

            dataCheck.Clear();

            reportDefinition.Save();

            string result = Global.LanguageManager.GetText("NoFilterApplied");

            if (reportDefinition.FilterCategories.Count > 0)
                result = Global.LanguageManager.GetText("FiltersApplied");

            context.Response.Write(result);
            context.Response.ContentType = "text/plain";
        }

        private void UpdateFilterView(HttpContext context)
        {
            // Get the full path of the current definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Create a new panel which contains the
            // filter category operators on root level.
            Panel pnlFilterCategories = new Panel();

            // Run through all applied filter categories.
            foreach (FilterCategoryOperator filterCategoryOperator in reportDefinition.FilterCategories)
            {
                // Add the filter category operator to the panel.
                pnlFilterCategories.Controls.Add(
                    filterCategoryOperator
                );

                // Render the filter category operator control.
                filterCategoryOperator.Render();
            }

            // Write the panel's html code the the http response.
            context.Response.Write(pnlFilterCategories.ToHtml());

            // Set the http response's content type to plain text.
            context.Response.ContentType = "text/plain";
        }

        private void ChangeFilterCategoryOperator(HttpContext context)
        {
            // Get the full path of the current definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Get the xPath to the filter category
            // operator from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Get the new type of the filter category
            // operator from the http request's parameters.
            FilterCategoryOperatorType type = (FilterCategoryOperatorType)Enum.Parse(
                typeof(FilterCategoryOperatorType),
                context.Request.Params["Type"]
            );

            FilterCategoryOperator filterCategoryOperator = new FilterCategoryOperator(
                reportDefinition.WeightingFilters,
                reportDefinition.XmlDocument.SelectSingleNode(xPath),
                0,
                fileName
            );

            filterCategoryOperator.Type = type;

            // Clear the report's data.
            reportDefinition.ClearData();

            reportDefinition.Save();
        }


        private void ClearReportDefinition(HttpContext context)
        {
            // Get the full path of the current session's report definition file.
            //string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();
            string directoryName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "ReportDefinitions",
                Global.Core.ClientName,
                Global.IdUser.Value.ToString()
            );
            string directoryNameDataCheck = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "DataChecks",
                "ReportDefinitions",
                Global.Core.ClientName,
                Global.IdUser.Value.ToString()
            );

            if (Directory.Exists(directoryName))
                Directory.Delete(directoryName, true);

            if (Directory.Exists(directoryNameDataCheck))
            {
                // Clear the data check entries.
                Directory.Delete(directoryNameDataCheck, true);
            }
        }
        private void ClearTabDefinition(HttpContext context)
        {
            // Get the full path of the current session's report definition file.
            //string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();
            string fileName;

            if (context.Request.Params["Source"] != null)
                fileName = context.Request.Params["Source"];
            else
                fileName = context.Session["ReportDefinition"].ToString();

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            XmlNode xmlNode;

            xmlNode = document.DocumentElement.SelectSingleNode("Variables[@Position=\"Left\"]");

            if (xmlNode != null)
                xmlNode.InnerXml = "";

            xmlNode = document.DocumentElement.SelectSingleNode("Variables[@Position=\"Top\"]");

            if (xmlNode != null)
                xmlNode.InnerXml = "";

            xmlNode = document.DocumentElement.SelectSingleNode("Filters/Operator");

            if (xmlNode != null)
                xmlNode.InnerXml = "";

            xmlNode = document.DocumentElement.SelectSingleNode("Results");

            if (xmlNode != null)
                xmlNode.ParentNode.RemoveChild(xmlNode);

            document.Save(fileName);
        }

        private void PopulateCrosstable(HttpContext context)
        {
            HttpContext.Current.Session["DataAggregationProgress"] = 0;

            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(PopulateCrosstableAsynch);

            Thread thread = new Thread(threadStart);

            thread.Start(HttpContext.Current.Session);
        }

        private void BuildCrosstable(HttpContext context)
        {
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            ReportCalculator calculator;

            calculator = new ReportCalculator(
                reportDefinition,
                Global.Core,
                HttpContext.Current.Session
            );

            //calculator.Calculate();

            reportDefinition.Save();

            Crosstables.Classes.Crosstable crosstable = new Crosstables.Classes.Crosstable(
                Global.Core,
                fileName
            );

            //crosstable.FilterClickAction = "ctl00$cphContent$btnDisplayFilters";

            //crosstable.AsynchRender = true;

            crosstable.Render();

            context.Response.Write(crosstable.ToHtml());
        }

        private void ApplyWorkflowFilterToAllTabs(HttpContext context)
        {
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();
            XmlDocument reportDocument = new XmlDocument();
            reportDocument.Load(fileName);

            string path = fileName.Substring(0, fileName.LastIndexOf("\\"));

            foreach (var file in Directory.GetFiles(path))
            {
                XmlDocument reportDocument1 = new XmlDocument();
                reportDocument1.Load(file);
                if (file == fileName)
                    continue;
                if (Path.GetFileName(file) == "Info.xml")
                    continue;
                HttpContext.Current.Session["ReportDefinition"] = file;

                XmlNode xmlNode1 = reportDocument.DocumentElement.SelectSingleNode("Workflow");

                if (xmlNode1 != null)
                {

                    reportDocument1.DocumentElement.SelectSingleNode("Workflow").InnerXml = xmlNode1.InnerXml;
                    XmlNode xmlNodeResults = reportDocument1.DocumentElement.SelectSingleNode("Results");

                    if (xmlNodeResults != null)
                        reportDocument1.DocumentElement.RemoveChild(xmlNodeResults);

                    reportDocument1.Save(file);

                    // Create a new report definition by the report definition file.
                    ReportDefinition reportDefinition = new ReportDefinition(
                        Global.Core,
                        file,
                        Global.HierarchyFilters[file]
                    );

                    ReportCalculator calculator;

                    calculator = new ReportCalculator(
                        reportDefinition,
                        Global.Core,
                        HttpContext.Current.Session
                    );

                    //calculator.Calculate();

                    reportDefinition.Save();

                    Crosstables.Classes.Crosstable crosstable = new Crosstables.Classes.Crosstable(
                        Global.Core,
                        file
                    );

                    //crosstable.FilterClickAction = "ctl00$cphContent$btnDisplayFilters";

                    //crosstable.AsynchRender = true;

                    crosstable.Render();


                    string tempFileName = Path.Combine(
                 HttpContext.Current.Request.PhysicalApplicationPath,
                 "Fileadmin",
                 "Temp",
                 "Exports",
                 HttpContext.Current.Session.SessionID
             );

                    if (Directory.Exists(tempFileName))
                        Directory.Delete(tempFileName, true);


                }

            }
            HttpContext.Current.Session["ReportDefinition"] = fileName;
        }

        private void PropagateFilterDefinition(HttpContext context)
        {
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            XmlNode xmlNode = document.DocumentElement.SelectSingleNode("Filters");

            foreach (string file in Directory.GetFiles(new FileInfo(fileName).DirectoryName))
            {
                if (file.EndsWith("Info.xml") || file == fileName)
                    continue;

                XmlDocument tab = new XmlDocument();
                tab.Load(file);

                tab.DocumentElement.SelectSingleNode("Filters").InnerXml = xmlNode.InnerXml;

                XmlNode xmlNodeResults = tab.DocumentElement.SelectSingleNode("Results");

                if (xmlNodeResults != null)
                    xmlNodeResults.ParentNode.RemoveChild(xmlNodeResults);

                tab.Save(file);
            }
        }


        private void UpdateSetting(HttpContext context)
        {
            // Get the name of the setting to change
            // from the http request's parameters.
            string name = context.Request.Params["Name"];

            // Get the value of the setting to change
            // from the http request's parameters.
            string value = context.Request.Params["Value"];

            // Get from the http request's parameters if the
            // setting update should clear the report data.
            bool clearData = bool.Parse(
                context.Request.Params["ClearData"]
            );

            bool applyAll;
            // Get from the http request's parameters if the
            // setting update should apply on all reports.
            if (context.Request.Params["ApplyAll"] != null)
                applyAll = bool.Parse(
                context.Request.Params["ApplyAll"]
            );
            else
                applyAll = false;

            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();
            if (applyAll)
            {
                string[] files = Directory.GetFiles(Path.GetDirectoryName(fileName));

                foreach (string file in files)
                {
                    if (file == fileName)
                        continue;
                    if (file.Contains("Info.xml"))
                        continue;

                    // Create a new report definition by the report definition file.
                    ReportDefinition reportDefinition1 = new ReportDefinition(
                        Global.Core,
                        file,
                        Global.HierarchyFilters[file]
                    );

                    // Update the setting.
                    reportDefinition1.Settings.SetValue(name, value);

                    if (name == "RankLeft" && bool.Parse(value) == false)
                    {
                        foreach (ReportDefinitionVariable variable in reportDefinition1.LeftVariables)
                        {
                            RestoreUnkrankedOrder(variable);
                        }
                    }

                    // Check if the setting update should clear the report data.
                    if (clearData)
                    {
                        // Clear the report data.
                        reportDefinition1.ClearData();
                    }

                    // Save the report definition.
                    reportDefinition1.Save();

                }
            }

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Update the setting.
            reportDefinition.Settings.SetValue(name, value);

            if (name == "RankLeft" && bool.Parse(value) == false)
            {
                foreach (ReportDefinitionVariable variable in reportDefinition.LeftVariables)
                {
                    RestoreUnkrankedOrder(variable);
                }
            }

            // Check if the setting update should clear the report data.
            if (clearData)
            {
                // Clear the report data.
                reportDefinition.ClearData();
            }

            // Save the report definition.
            reportDefinition.Save();

            Global.UserDefaults["ReportDefinitionSettings"] = reportDefinition.Settings.XmlNode.InnerXml;

        }

        private void PinBottomBar(HttpContext context)
        {
            bool result = !bool.Parse(Global.UserDefaults["BottomBarPinned", "false"]);

            Global.UserDefaults["BottomBarPinned", "false"] = result.ToString();

            Global.UserDefaults.Save();
        }

        private void ValidateCategory(HttpContext context)
        {
            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the report definition file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );
            string name = "";
            StringBuilder sbFileNames = new StringBuilder();
            string variableName = context.Request.Params["VariableFilter"];

            object idVariable = null;

            List<object[]> variable = Global.Core.TaxonomyVariables.ExecuteReader(string.Format("SELECT TV.Id,TVL.Label FROM TaxonomyVariables TV INNER JOIN TaxonomyVariableLabels TVL ON TV.Id = TVL.IdTaxonomyVariable " +
                                                            " WHERE TV.Name = '{0}'", variableName), new object[] { });



            bool isAlltabs = bool.Parse(context.Request.Params["AllTabs"]);
            if (isAlltabs)
            {
                string path = Path.GetDirectoryName(fileName);
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    if (Path.GetFileName(file) == "Info.xml")
                        continue;

                    IsExist(context, file, variable[0][1], variable[0][0]);
                }
            }
            else
                IsExist(context, fileName, variable[0][1], variable[0][0]);
        }

        private static void IsExist(HttpContext context, string fileName, object variableName, object idVariable)
        {
            // Create a new xml document that contains
            // the client's workflow definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the client's workflow
            // definition file into the xml document.
            document.Load(fileName);
            if (context.Request.Params["Category"] != null)
            {
                XmlNodeList xmlCategories = document.SelectNodes("/Report/Workflow/Selection[@Name='" + context.Request.Params["VariableFilter"] + "']/VariableFilter[@IdVariable='" + idVariable + "']/Category");

                bool isCategoryMatch = false;
                StringBuilder sbSelectedCategories = new StringBuilder();
                foreach (XmlNode xmlCategory in xmlCategories)
                {
                    //check weather category selected in bottom bar definition or no ?. if selected in split n export.
                    if (xmlCategory.Attributes["Id"] != null && xmlCategory.Attributes["Id"].Value.ToLower() == context.Request.Params["Category"].ToLower())
                    {
                        isCategoryMatch = true;
                    }
                }
                if (!isCategoryMatch && xmlCategories.Count > 0)
                {
                    sbSelectedCategories.Append(context.Request.Params["Category"]);
                }

                //reset value to false to check for comman filter
                isCategoryMatch = true;
                // Create a new report definition by the report definition file.
                ReportDefinition reportDefinition = new ReportDefinition(
                    Global.Core,
                    fileName,
                    Global.HierarchyFilters[fileName]
                );

                // Run through all filter category operators on root level.
                foreach (FilterCategoryOperator filterCategoryOperator in reportDefinition.FilterCategories)
                {
                    foreach (FilterCategoryOperator _filterCategoryOperator in filterCategoryOperator.FilterCategoryOperators)
                    {
                        FilterCategory category = _filterCategoryOperator.FilterCategories.Find(x => x.IdVariable == Guid.Parse(idVariable.ToString()));
                        if (category != null)
                        {
                            isCategoryMatch = false;
                            category = _filterCategoryOperator.FilterCategories.Find(x => x.IdCategory == Guid.Parse(context.Request.Params["Category"].ToString()));
                            if (category != null && category.IdCategory == Guid.Parse(context.Request.Params["Category"]))
                            {
                                isCategoryMatch = true;
                                break;
                            }
                        }
                    }
                    if (isCategoryMatch)
                        break;
                }

                if (!isCategoryMatch && sbSelectedCategories.ToString().IndexOf(context.Request.Params["Category"].ToString()) == -1)
                {
                    sbSelectedCategories.Append(context.Request.Params["Category"]);
                }

                List<object[]> categoryLabels = new List<object[]>();
                if (sbSelectedCategories.ToString() != "")
                {
                    //sbSelectedCategories.Length = sbSelectedCategories.Length - 1;

                    categoryLabels = Global.Core.TaxonomyCategoryLabels.ExecuteReader(string.Format("SELECT Label FROM TaxonomyCategoryLabels WHERE IdTaxonomyCategory = '{0}'"
                        , sbSelectedCategories.ToString()), new object[] { });
                }

                if (categoryLabels.Count > 0)
                {
                    sbSelectedCategories.Clear();
                    sbSelectedCategories.Append(string.Format(Global.LanguageManager.GetText("TableExportFilterRestrict"),
                        variableName,
                        string.Join(",", categoryLabels.Select(x => "\"" + x[0] + "\"")),
                        document.SelectSingleNode("/Report").Attributes["Name"].Value));

                    context.Response.Write(sbSelectedCategories.ToString());
                    return;
                }
            }
        }

        private void ExportTable(HttpContext context)
        {
            UsageLogger logger = new UsageLogger(
                Global.Core.ClientName,
                Global.User
            );

            logger.Log(UsageLogVariable.Studio, "ExcelExport");

            //Get user saved report folder
            string userSavedReportPath = Path.Combine(
                   HttpContext.Current.Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "SavedReports",
                   Global.Core.ClientName,
                  // "localhost",
                  Global.IdUser.Value.ToString()
                );
            Guid newGuid;


            string[] subdirEntries = Directory.GetDirectories(userSavedReportPath, "*", SearchOption.AllDirectories);
            foreach (string subdir in subdirEntries)
            {
                if (!Guid.TryParse(subdir, out newGuid))
                {
                    HttpContext.Current.Session["ReportDefinition"] = subdir;

                    // Get the full path of the current session's report definition file.
                    string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

                    // Create a new report definition by the report definition file.
                    ReportDefinition reportDefinition = new ReportDefinition(
                        Global.Core,
                        fileName,
                        Global.HierarchyFilters[fileName]
                    );
                    string name = "";
                    StringBuilder sbFileNames = new StringBuilder();
                    string variableName = context.Request.Params["VariableFilter"];
                    // Create a new xml document that contains
                    // the client's workflow definition.
                    XmlDocument document = new XmlDocument();

                    // Load the contents of the client's workflow
                    // definition file into the xml document.
                    document.Load(fileName);

                    XmlNode xmlNode = document.SelectSingleNode("/Report/Workflow");
                    Guid elementGuid = Guid.NewGuid();

                    //split and export
                    if (context.Request.Params["Categories"] != null)
                    {
                        object idVariable = Global.Core.TaxonomyVariables.GetValue(
                             "Id",
                             "Name",
                             variableName
                         );

                        variableName = Global.Core.TaxonomyVariableLabels.GetValue(
                            "Label",
                            "IdTaxonomyVariable",
                            idVariable
                            ).ToString();

                        int i = 0;
                        try
                        {
                            foreach (string category in context.Request.Params["Categories"].Split(','))
                            {
                                if (category == "")
                                    continue;

                                // Check if the workflow selector exists.
                                if (xmlNode == null)
                                    continue;



                                if (xmlNode.SelectSingleNode("Selection[@Id='" + elementGuid + "']") == null)
                                {
                                    xmlNode.InnerXml += string.Format(
                                     "<Selection Name=\"{0}\" Id=\"{1}\">" +
                                     "<VariableFilter Name=\"{2}\" VariableName=\"{0}\" IsTaxonomy=\"True\" Mode=\"Multi\" IdVariable=\"{3}\"></VariableFilter>" +
                                     "</Selection>",
                                     (string)variableName + "_varFilter",
                                     elementGuid,
                                     (string)variableName + " varFilter",
                                     idVariable
                                 );

                                    // Save the changes made to the workflow definition file.
                                    document.Save(fileName);
                                }
                                Workflow workflow = new Workflow(
                            Global.Core,
                            fileName,
                            document.DocumentElement.SelectSingleNode("Workflow"),
                            "/Handlers/GlobalHandler.ashx",
                            Global.HierarchyFilters[fileName]
                        );
                                if (!reportDefinition.Workflow.Selections.ContainsKey(variableName))
                                    reportDefinition.Workflow.Selections.Add(variableName, new WorkflowSelection(workflow, document.SelectSingleNode("/Report/Workflow/Selection[@Id='" + elementGuid + "']")));

                                WorkflowSelection workflowSelection = new WorkflowSelection(workflow, document.SelectSingleNode("/Report/Workflow/Selection[@Id='" + elementGuid + "']"));
                                WorkflowSelectionSelector selector = null;

                                selector = new WorkflowSelectionVariable(workflowSelection, xmlNode.SelectSingleNode("Selection[@Id='" + elementGuid + "']/VariableFilter"));
                                if (!reportDefinition.Workflow.Selections[variableName].SelectionVariables.ContainsKey(variableName + "_varFilter"))
                                    reportDefinition.Workflow.Selections[variableName].SelectionVariables.Add(variableName + "_varFilter", selector);

                                selector = reportDefinition.Workflow.Selections[variableName].SelectionVariables[variableName + "_varFilter"];

                                selector.Select(Guid.Parse(category));


                                if (selector.GetType() == typeof(WorkflowSelectionHierarchy))
                                {
                                    Global.HierarchyFilters[fileName].Hierarchies = selector.Selector.SelectedItems;
                                    Global.HierarchyFilters[fileName].IsLoaded = false;
                                }
                                else
                                {
                                    //reportDefinition.ClearData();
                                    XmlNode xmlNodeResult = document.DocumentElement.SelectSingleNode("Results");

                                    if (xmlNodeResult != null)
                                        xmlNodeResult.ParentNode.RemoveChild(xmlNodeResult);

                                    DataCheck dataCheck = new DataCheck(fileName);

                                    dataCheck.Clear();
                                }

                                // Save the changes made to the workflow definition file.
                                document.Save(fileName);

                                name = RenderTableExport(context, fileName, reportDefinition, i);

                                sbFileNames.Append(string.Format(
                            "/Fileadmin/Temp/Exports/{0}/{1}.xlsx###",
                            HttpContext.Current.Session.SessionID,
                            name
                        ));

                                selector.DeSelect(Guid.Parse(category));

                                // Save the changes made to the workflow definition file.
                                document.Save(fileName);
                                i++;
                            }
                            if (sbFileNames.ToString() != "")
                            {
                                reportDefinition.Workflow.Selections.Remove(variableName);


                                // Clear the report's data.
                                reportDefinition.ClearData();

                                // Clear the data check entries.
                                DataCheck dataCheck = new DataCheck(fileName);

                                dataCheck.Clear();

                                // Save the report definition.
                                reportDefinition.Save();

                                xmlNode = xmlNode.SelectSingleNode("Selection[@Id='" + elementGuid + "']");
                                if (xmlNode != null)
                                {
                                    XmlNode parent = xmlNode.ParentNode;
                                    parent.RemoveChild(document.SelectSingleNode("/Report/Workflow/Selection[@Id='" + elementGuid + "']"));
                                    document.Save(fileName);
                                }

                                sbFileNames.Length = sbFileNames.Length - 3;
                                context.Response.Write(sbFileNames.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            xmlNode = xmlNode.SelectSingleNode("Selection[@Id='" + elementGuid + "']");
                            if (xmlNode != null)
                            {
                                XmlNode parent = xmlNode.ParentNode;
                                parent.RemoveChild(document.SelectSingleNode("/Report/Workflow/Selection[@Id='" + elementGuid + "']"));
                                document.Save(fileName);
                            }
                        }
                    }
                    else
                    {
                        name = RenderTableExport(context, fileName, reportDefinition);
                        context.Response.Write(string.Format(
                        "/Fileadmin/Temp/Exports/{0}/{1}.xlsx",
                        HttpContext.Current.Session.SessionID,
                        name
                    ));
                    }
                }
            }
          

        }

        private static string RenderTableExport(HttpContext context, string fileName, ReportDefinition reportDefinition, int? i = null)
        {
            if (i != null)
            {
                ReportCalculator calculator;

                calculator = new ReportCalculator(
                    reportDefinition,
                    Global.Core,
                    HttpContext.Current.Session
                );
            }

            reportDefinition = new ReportDefinition(
               Global.Core,
               fileName,
               Global.HierarchyFilters[fileName]
           );



            DisplayType displayType = reportDefinition.Settings.DisplayType;
            reportDefinition.Settings.DisplayType = DisplayType.Crosstable;
            reportDefinition.Save();


            Crosstables.Classes.Crosstable crosstable = new Crosstables.Classes.Crosstable(
                Global.Core,
                fileName
            );

            crosstable.IsExport = true;
            //crosstable.AsynchRender = true;

            crosstable.Render();

            ColorSchemeStylesheet colorScheme = new ColorSchemeStylesheet();
            colorScheme.Render();

            StringBuilder style = new StringBuilder();
            style.Append("<style type=\"text/css\">");
            style.Append(colorScheme.InnerHtml.Split(new string[] { "##### Scroll bar styles #####" }, StringSplitOptions.None)[0]);

            style.Append(File.ReadAllText(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Stylesheets",
                "Modules",
                "Crosstables.css"
            )));

            style.Append("</style>");

            Crosstables.Classes.Exporter exporter = new Crosstables.Classes.Exporter(
                crosstable.Table,
                style.ToString(),
                reportDefinition
            );


            reportDefinition.Settings.DisplayType = displayType;
            reportDefinition.Save();

            string categoryName = "";
            if (i != null)
                categoryName = Global.Core.TaxonomyCategoryLabels.GetValue(
                    "Label",
                    "IdTaxonomyCategory",
                    context.Request.Params["Categories"].Split(',')[(int)i]).ToString();


            string fName = exporter.Export();

            string tempFileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                "Exports",
                HttpContext.Current.Session.SessionID
            );

            string name = (new ReportDefinitionInfo(Path.Combine((
                new FileInfo(fileName)).DirectoryName,
                "Info.xml"
            ))).Name;

            if (string.IsNullOrEmpty(name))
                name = Global.LanguageManager.GetText("ReporterExport");

            if ((name.Length + tempFileName.Length) > 248)
                name = name.Substring(0, (248 - tempFileName.Length));

            tempFileName = Path.Combine(
                tempFileName,
                name + (categoryName != "" ? "-" + categoryName : categoryName) + ".xlsx"
            );

            FileInfo fInfo = new FileInfo(tempFileName);

            if (!Directory.Exists(fInfo.DirectoryName))
                Directory.CreateDirectory(fInfo.DirectoryName);

            File.Copy(fName, tempFileName, true);

            return name + (categoryName != "" ? "-" + categoryName : categoryName);
        }

        private void ExportAllTabs(HttpContext context)
        {
            UsageLogger logger = new UsageLogger(
                Global.Core.ClientName,
                Global.User
            );

            logger.Log(UsageLogVariable.Studio, "ExcelExport");
            string variableName = context.Request.Params["VariableFilter"];
            StringBuilder sbFileNames = new StringBuilder();
            string name = "";

            string userSavedReportPath = Path.Combine(
                  HttpContext.Current.Request.PhysicalApplicationPath,
                  "Fileadmin",
                  "SavedReports",
                  Global.Core.ClientName,               
                 Global.IdUser.Value.ToString()
               );
            Guid newGuid;
            string[] subdirEntries = Directory.GetDirectories(userSavedReportPath, "*", SearchOption.AllDirectories);

            foreach (string subdir in subdirEntries)
            {
                if (!Guid.TryParse(subdir, out newGuid))
                {
                    string parentFolder = Directory.GetParent(subdir).Name;

                    bool isValid = Guid.TryParse(parentFolder, out newGuid);

                    if (isValid)
                        parentFolder = "My Saved Reports";



                    HttpContext.Current.Session["ReportDefinition"] = subdir;

                    // Get the full path of the current session's report definition file.
                    string directory = subdir;//(new FileInfo(HttpContext.Current.Session["ReportDefinition"].ToString())).DirectoryName;
                    List<Guid> guidIds = new List<Guid>();

                    // for split and export
                    if (context.Request.Params["Categories"] != null)
                    {
                        object idVariable = Global.Core.TaxonomyVariables.GetValue(
                                          "Id",
                                          "Name",
                                          variableName
                                      );
                        try
                        {
                            int j = 0;

                            foreach (string category in context.Request.Params["Categories"].Split(','))
                            {
                                if (category == "")
                                    continue;
                                name = RenderTableExportAllTabs(context, directory, idVariable, category, j, guidIds, parentFolder);
                                sbFileNames.Append(string.Format(
                                    "/Fileadmin/Temp/Exports/{2}/{0}/{1}.xlsx",
                            HttpContext.Current.Session.SessionID,
                            name,
                            parentFolder
                                    ));
                                j++;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        name = RenderTableExportAllTabs(context, directory);

                        context.Response.Write(string.Format(
                            "/Fileadmin/Temp/Exports/{0}/{1}.xlsx",
                            HttpContext.Current.Session.SessionID,
                            name
                        ));
                    }

                    foreach (string fileName in Directory.GetFiles(directory).OrderBy(x => new FileInfo(x).CreationTime))
                    {
                        if (new FileInfo(fileName).Name == "Info.xml")
                            continue;

                        // Create a new xml document that contains
                        // the client's workflow definition.
                        XmlDocument document = new XmlDocument();

                        // Load the contents of the client's workflow
                        // definition file into the xml document.
                        document.Load(fileName);

                        foreach (Guid id in guidIds)
                        {
                            if (sbFileNames.ToString() != "")
                            {
                                ReportDefinition reportDefinition = new ReportDefinition(
                         Global.Core,
                         fileName,
                         Global.HierarchyFilters[fileName]
                     );
                                reportDefinition.Workflow.Selections.Remove(variableName);


                                // Clear the report's data.
                                reportDefinition.ClearData();

                                // Clear the data check entries.
                                DataCheck dataCheck = new DataCheck(fileName);

                                dataCheck.Clear();

                                // Save the report definition.
                                reportDefinition.Save();
                                XmlNode xmlNode = document.SelectSingleNode("/Report/Workflow");

                                xmlNode = xmlNode.SelectSingleNode("Selection[@Id='" + id + "']");
                                if (xmlNode != null)
                                {
                                    XmlNode parent = xmlNode.ParentNode;
                                    parent.RemoveChild(document.SelectSingleNode("/Report/Workflow/Selection[@Id='" + id + "']"));
                                    document.Save(fileName);
                                }

                            }
                        }
                    }

                    if (sbFileNames.ToString() != "")
                    {
                        sbFileNames.Length = sbFileNames.Length - 3;
                        context.Response.Write(sbFileNames.ToString());
                    }
                }
            }
        }

        private static string RenderTableExportAllTabs(HttpContext context, string directory, object idVariable = null, string category = "", int? j = null, List<Guid> guidIds = null, string parentFolder = null)
        {
            string fName = null;

            string categoryName = "";
            if (j != null)
                categoryName = Global.Core.TaxonomyCategoryLabels.GetValue(
                    "Label",
                    "IdTaxonomyCategory",
                    context.Request.Params["Categories"].Split(',')[(int)j]).ToString();

            int activeIndex = 0;
            int i = 0;
            // Run through all files of the directory.
            foreach (string fileName in Directory.GetFiles(directory).OrderBy(x => new FileInfo(x).CreationTime))
            {
                if (new FileInfo(fileName).Name == "Info.xml")
                    continue;

                // Create a new report definition by the report definition file.
                ReportDefinition reportDefinition = new ReportDefinition(
                    Global.Core,
                    fileName,
                    Global.HierarchyFilters[fileName]
                );

                StringBuilder sbFileNames = new StringBuilder();
                string variableName = context.Request.Params["VariableFilter"];
                // Create a new xml document that contains
                // the client's workflow definition.
                XmlDocument document = new XmlDocument();

                // Load the contents of the client's workflow
                // definition file into the xml document.
                document.Load(fileName);
                if (variableName != "undefined" && idVariable != null)
                    variableName = Global.Core.TaxonomyVariableLabels.GetValue(
                "Label",
                "IdTaxonomyVariable",
                idVariable
                ).ToString();

                WorkflowSelectionSelector selector = null;
                selector = ExportFilter(idVariable, category, fileName, reportDefinition, variableName, document, selector, guidIds);
                if (j != null)
                {
                    ReportCalculator calculator;

                    calculator = new ReportCalculator(
                        reportDefinition,
                        Global.Core,
                        HttpContext.Current.Session
                    );
                }
                reportDefinition = new ReportDefinition(
                   Global.Core,
                   fileName,
                   Global.HierarchyFilters[fileName]
               );



                DisplayType displayType = reportDefinition.Settings.DisplayType;
                reportDefinition.Settings.DisplayType = DisplayType.Crosstable;
                reportDefinition.Save();

                Crosstables.Classes.Crosstable crosstable = new Crosstables.Classes.Crosstable(
                    Global.Core,
                    fileName
                );

                crosstable.IsExport = true;
                //crosstable.AsynchRender = true;

                crosstable.Render();

                ColorSchemeStylesheet colorScheme = new ColorSchemeStylesheet();
                colorScheme.Render();

                StringBuilder style = new StringBuilder();
                style.Append("<style type=\"text/css\">");
                style.Append(colorScheme.InnerHtml.Split(new string[] { "##### Scroll bar styles #####" }, StringSplitOptions.None)[0]);

                style.Append(File.ReadAllText(Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "Stylesheets",
                    "Modules",
                    "Crosstables.css"
                )));

                style.Append("</style>");

                Crosstables.Classes.Exporter exporter = new Crosstables.Classes.Exporter(
                    crosstable.Table,
                    style.ToString(),
                    reportDefinition
                );

                reportDefinition.Settings.DisplayType = displayType;
                reportDefinition.Save();

                fName = exporter.Export(fName);

                if (fileName == HttpContext.Current.Session["ReportDefinition"].ToString())
                    activeIndex = i;

                if (idVariable != null)
                {
                    selector.DeSelect(Guid.Parse(category));

                    // Save the changes made to the workflow definition file.
                    document.Save(fileName);
                }

                i++;
            }

            if (fName != null)
            {
                ExcelWriter writer = new ExcelWriter(fName);

                writer.Workbook.Worksheets[activeIndex].Select();

                writer.Save(fName);
            }

            string name = (new ReportDefinitionInfo(Path.Combine(
                directory,
                "Info.xml"
            ))).Name;

            if (string.IsNullOrEmpty(name))
                name = Global.LanguageManager.GetText("ReporterExport");

            string tempFileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                "Exports",               
                HttpContext.Current.Session.SessionID,
                parentFolder,
                name + (categoryName != "" ? "-" + categoryName : categoryName) + ".xlsx"
            );

            FileInfo fInfo = new FileInfo(tempFileName);

            if (!Directory.Exists(fInfo.DirectoryName))
                Directory.CreateDirectory(fInfo.DirectoryName);

            File.Copy(fName, tempFileName, true);
            return name + (categoryName != "" ? "-" + categoryName : categoryName);
        }

        private static WorkflowSelectionSelector ExportFilter(object idVariable, string category, string fileName, ReportDefinition reportDefinition, string variableName, XmlDocument document, WorkflowSelectionSelector selector, List<Guid> guidIds = null)
        {
            Guid elementGuid = default(Guid);
            XmlNode xmlNode = document.SelectSingleNode("/Report/Workflow");
            try
            {
                if (idVariable != null)
                {
                    if (xmlNode.SelectSingleNode("Selection[@Name='" + variableName + "_varFilter']") == null)
                    {

                        elementGuid = Guid.NewGuid();

                        xmlNode.InnerXml += string.Format(
                         "<Selection Name=\"{0}\" Id=\"{1}\">" +
                         "<VariableFilter Name=\"{2}\" VariableName=\"{0}\" IsTaxonomy=\"True\" Mode=\"Multi\" IdVariable=\"{3}\"></VariableFilter>" +
                         "</Selection>",
                         (string)variableName + "_varFilter",
                         elementGuid,
                         (string)variableName + " varFilter",
                         idVariable
                     );
                        if (!guidIds.Contains(elementGuid))
                            guidIds.Add(elementGuid);

                        // Save the changes made to the workflow definition file.
                        document.Save(fileName);
                    }
                    else
                    {
                        XmlNode xmlFilter = xmlNode.SelectSingleNode("Selection[@Name='" + variableName + "_varFilter']");

                        elementGuid = Guid.Parse(xmlFilter.Attributes["Id"].Value);
                        if (!guidIds.Contains(Guid.Parse(xmlFilter.Attributes["Id"].Value)))
                            guidIds.Add(elementGuid);
                    }
                    Workflow workflow = new Workflow(
                Global.Core,
                fileName,
                document.DocumentElement.SelectSingleNode("Workflow"),
                "/Handlers/GlobalHandler.ashx",
                Global.HierarchyFilters[fileName]
            );
                    if (!reportDefinition.Workflow.Selections.ContainsKey(variableName))
                        reportDefinition.Workflow.Selections.Add(variableName, new WorkflowSelection(workflow, document.SelectSingleNode("/Report/Workflow/Selection[@Id='" + elementGuid + "']")));

                    WorkflowSelection workflowSelection = new WorkflowSelection(workflow, document.SelectSingleNode("/Report/Workflow/Selection[@Id='" + elementGuid + "']"));

                    selector = new WorkflowSelectionVariable(workflowSelection, xmlNode.SelectSingleNode("Selection[@Id='" + elementGuid + "']/VariableFilter"));
                    if (!reportDefinition.Workflow.Selections[variableName].SelectionVariables.ContainsKey(variableName + "_varFilter"))
                        reportDefinition.Workflow.Selections[variableName].SelectionVariables.Add(variableName + "_varFilter", selector);

                    selector = reportDefinition.Workflow.Selections[variableName].SelectionVariables[variableName + "_varFilter"];

                    selector.Select(Guid.Parse(category));


                    if (selector.GetType() == typeof(WorkflowSelectionHierarchy))
                    {
                        Global.HierarchyFilters[fileName].Hierarchies = selector.Selector.SelectedItems;
                        Global.HierarchyFilters[fileName].IsLoaded = false;
                    }
                    else
                    {
                        //reportDefinition.ClearData();
                        XmlNode xmlNodeResult = document.DocumentElement.SelectSingleNode("Results");

                        if (xmlNodeResult != null)
                            xmlNodeResult.ParentNode.RemoveChild(xmlNodeResult);

                        DataCheck dataCheck = new DataCheck(fileName);

                        dataCheck.Clear();
                    }

                    // Save the changes made to the workflow definition file.
                    document.Save(fileName);

                }
            }
            catch (Exception ex)
            {
                xmlNode = xmlNode.SelectSingleNode("Selection[@Id='" + elementGuid + "']");
                if (xmlNode != null)
                {
                    XmlNode parent = xmlNode.ParentNode;
                    parent.RemoveChild(document.SelectSingleNode("/Report/Workflow/Selection[@Id='" + elementGuid + "']"));
                    document.Save(fileName);
                }
            }
            return selector;
        }

        private void ExportVariables(HttpContext context)
        {
            MetadataExport export = new MetadataExport(new MetadataVariableColumn[]
            {
                MetadataVariableColumn.Chapter,
                MetadataVariableColumn.Type,
                MetadataVariableColumn.Name,
                MetadataVariableColumn.Label,
                MetadataVariableColumn.Hierarchy
            }, new MetadataCategoryColumn[]
            {
                MetadataCategoryColumn.VariableLabel,
                MetadataCategoryColumn.Value,
                MetadataCategoryColumn.Name,
                MetadataCategoryColumn.Label,
                MetadataCategoryColumn.Hierarchy
            });

            string fileName = export.Render();

            WriteFileToResponse(
                fileName,
                "Variables.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                true
            );
        }


        private void SaveExisting(HttpContext context)
        {
            if (HttpContext.Current.Session["ActiveSavedReport"] == null)
                return;

            // Get the full path of the current session's report definition file.
            string directoryName = HttpContext.Current.Session["ActiveSavedReport"].ToString();

            if (!Directory.Exists(directoryName))
                return;

            string destinationDirectory = directoryName.Replace(
                "Temp\\OpenSavedReports\\" + HttpContext.Current.Session.SessionID,
                "SavedReports\\" + Global.Core.ClientName
            );

            Guid idUser = Guid.Parse(new DirectoryInfo(directoryName).Name.Substring(0, 36));
            Guid idReport = Guid.Parse(new DirectoryInfo(directoryName).Name.Substring(36, 36));

            if (idUser != Global.IdUser.Value)
            {
                ReportDefinitionInfo info = new ReportDefinitionInfo(Path.Combine(
                    directoryName,
                    "Info.xml"
                ));

                if (!info.AllowOverwrite)
                    return;
            }

            destinationDirectory = destinationDirectory.Replace(
                idUser.ToString() + idReport.ToString(),
                idUser + "\\" + idReport
            );

            if (HttpContext.Current.Session["LinkCloudSelectedReportUrl"] == null)
            {
                string LinkCloudSelectedReportUrl = Path.Combine(
                            HttpContext.Current.Request.PhysicalApplicationPath,
                            "Fileadmin",
                            "SavedReports",
                            Global.Core.ClientName,
                            idUser.ToString(),
                            idReport.ToString()
                        );
                HttpContext.Current.Session["LinkCloudSelectedReportUrl"] = LinkCloudSelectedReportUrl;
            }


            if (HttpContext.Current.Session["LinkCloudSelectedReportUrl"].ToString().IndexOf(idReport.ToString()) != -1)
            {
                destinationDirectory = HttpContext.Current.Session["LinkCloudSelectedReportUrl"].ToString();
            }

            if (Directory.Exists(destinationDirectory))
                Directory.Delete(destinationDirectory, true);

            Directory.CreateDirectory(destinationDirectory);

            foreach (string file in Directory.GetFiles(directoryName).OrderBy(d => new FileInfo(d).CreationTime))
            {
                File.Copy(file, Path.Combine(
                    destinationDirectory,
                    new FileInfo(file).Name
                ));
            }

            context.Response.Write(Global.IdUser.Value.ToString() + idReport.ToString());
        }

        private void SaveTab(HttpContext context)
        {
            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            Guid idReport = Guid.NewGuid();

            string destinationDirectory = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedReports",
                Global.Core.ClientName,
                Global.IdUser.Value.ToString(),
                idReport.ToString()
            );

            Guid idFile = Guid.NewGuid();

            Directory.CreateDirectory(destinationDirectory);

            File.WriteAllText(Path.Combine(
                destinationDirectory,
                "Info.xml"
            ), string.Format(
                "<Info><ActiveReport>{0}</ActiveReport><Name>{1}</Name></Info>",
                idFile,
                context.Request.Params["Name"]
            ));

            File.Copy(fileName, Path.Combine(
                destinationDirectory,
                idFile + ".xml"
            ));

            ReportDefinitionInfo info = new ReportDefinitionInfo(Path.Combine(
                destinationDirectory,
                "Info.xml"
            ));

            bool allowOverwrite = true;

            if (context.Request.Params["AllowOverwrite"] != null)
                allowOverwrite = bool.Parse(context.Request.Params["AllowOverwrite"]);

            info.AllowOverwrite = allowOverwrite;
            info.Save();

            HttpContext.Current.Session["LinkCloudSelectedDirectory"] = Directory.GetParent(destinationDirectory);

            context.Response.Write(Global.IdUser.Value.ToString() + idReport.ToString());
        }

        private void OverwriteAllowed(HttpContext context)
        {
            // Get the full path of the current session's report definition file.
            string directoryName = HttpContext.Current.Session["ActiveSavedReport"].ToString();

            Guid idUser = Guid.Parse(new DirectoryInfo(directoryName).Name.Substring(0, 36));

            if (idUser == Global.IdUser.Value)
            {
                context.Response.Write(true);
                return;
            }

            ReportDefinitionInfo info = new ReportDefinitionInfo(Path.Combine(
                Path.GetDirectoryName((string)HttpContext.Current.Session["ReportDefinition"]),
                "Info.xml"
            ));

            context.Response.Write(info.AllowOverwrite);
        }

        private void SaveAllTabs(HttpContext context)
        {
            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            Guid idSavedReport = Guid.NewGuid();
            string destinationDirectory = "";
            if (HttpContext.Current.Session["ManualSaveReportFolderSelect"] != null)
                destinationDirectory = HttpContext.Current.Session["ManualSaveReportFolderSelect"].ToString();
            if (destinationDirectory == "" || destinationDirectory == null)
            {
                destinationDirectory = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedReports",
                Global.Core.ClientName,
                Global.IdUser.Value.ToString(),
                idSavedReport.ToString()
            );
            }
            else
            {
                destinationDirectory += "/" + idSavedReport.ToString();
            }


            Directory.CreateDirectory(destinationDirectory);

            FileInfo fInfo = new FileInfo(fileName);

            // Run through all files in the report definitions directory.
            foreach (FileInfo file in fInfo.Directory.GetFiles().OrderBy(x => x.CreationTime).ToArray())
            {
                DateTime creationtime = file.CreationTime;

                File.Copy(file.FullName, Path.Combine(
                    destinationDirectory,
                    file.Name
                ));

                File.SetCreationTime(Path.Combine(
                    destinationDirectory,
                    file.Name
                ), creationtime);

            }

            ReportDefinitionInfo info = new ReportDefinitionInfo(Path.Combine(
                destinationDirectory,
                "Info.xml"
            ));

            bool allowOverwrite = true;

            if (context.Request.Params["AllowOverwrite"] != null)
                allowOverwrite = bool.Parse(context.Request.Params["AllowOverwrite"]);

            info.AllowOverwrite = allowOverwrite;
            info.Name = context.Request.Params["Name"];

            info.Save();
            HttpContext.Current.Session["LinkCloudSelectedDirectory"] = Directory.GetParent(destinationDirectory);
            context.Response.Write(string.Format(
                "{0}{1}",
                Global.IdUser.Value,
                idSavedReport
            ));
        }


        private void GetNumericVariables(HttpContext context)
        {
            // Create a new string builder that
            // contains the result JSON string.
            StringBuilder result = new StringBuilder();

            // Open the array that contains the result variables.
            result.Append("[");

            // Parse the id of the study to get the variables
            // of from the http request's parameters.
            Guid idStudy = Guid.Parse(context.Request.Params["IdStudy"]);

            // Get all variables of the study.
            List<object[]> variables = Global.Core.Variables.GetValues(
                new string[] { "Id", "Name" },
                new string[] { "IdStudy", "Type" },
                new object[] { idStudy, (int)VariableType.Numeric }
            );

            // Run through all variables of the study.
            foreach (object[] variable in variables)
            {
                result.Append(this.ToJson(new string[]{
                    "Id",
                    "Name"
                }, new object[]{
                    variable[0],
                    variable[1]
                }));

                result.Append(",");
            }

            if (variables.Count > 0)
                result = result.Remove(result.Length - 1, 1);

            // Close the array that contains the result variables.
            result.Append("]");

            // Write the contents of the result
            // string builder to the http response.
            context.Response.Write(result.ToString());
        }


        private void UsageLog(HttpContext context)
        {
            UsageLogVariable variable = (UsageLogVariable)Enum.Parse(
                typeof(UsageLogVariable),
                context.Request.Params["Variable"]
            );

            string category = context.Request.Params["Category"];

            UsageLogger logger = new UsageLogger(
                Global.Core.ClientName,
                Global.User
            );

            logger.Log(variable, category);
        }

        #endregion


        #region Category search methods

        public void SearchCategories(HttpContext context)
        {
            if (this.ActiveCategorySearch != null)
                this.ActiveCategorySearch.Abort();

            Thread thread = null;
            Task task = Task.Factory.StartNew(() =>
            {
                thread = Thread.CurrentThread;
                _SearchCategories(context);
            });

            this.ActiveCategorySearch = thread;

            task.Wait();

            this.ActiveCategorySearch = null;
        }

        private void _SearchCategories(HttpContext context)
        {
            HttpContext.Current = context;

            HierarchyFilter hierarchyFilter = null;

            if (!string.IsNullOrEmpty(context.Request.Params["Source"]))
            {
                hierarchyFilter = Global.HierarchyFilters[context.Request.Params["Source"]];

                if (!hierarchyFilter.IsLoaded)
                {
                    hierarchyFilter.Load();
                }
            }

            CategorySearch categorySearch = (CategorySearch)context.Session["CategorySearch" + context.Request.Params["Id"]];

            int idLanguage = 2057;

            if (context.Request.Params["IdLanguage"] != null)
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);

            // Get the search expression from the http request's parameters.
            string searchExpression = context.Request.Params["Expression"].Trim();

            int limit = categorySearch.Limit;

            StringBuilder result = new StringBuilder();

            result.Append(searchExpression);
            result.Append("##################SPLIT##################");

            List<object[]> searchResults = new List<object[]>();

            if (categorySearch.SearchMode == CategorySearchMode.All || categorySearch.SearchMode == CategorySearchMode.Taxonomy)
            {
                searchResults.AddRange(Global.Core.TaxonomyVariables.ExecuteReader(
                    "SELECT TaxonomyVariables.Id, TaxonomyVariables.Type, TaxonomyVariables.Name, TaxonomyVariableLabels.Label, TaxonomyCategories.Id, TaxonomyCategories.Name, TaxonomyCategoryLabels.Label, 1 FROM TaxonomyCategories " +
                    "LEFT JOIN TaxonomyCategoryLabels ON TaxonomyCategories.Id = TaxonomyCategoryLabels.IdTaxonomyCategory " +
                    "LEFT JOIN TaxonomyVariables ON TaxonomyCategories.IdTaxonomyVariable = TaxonomyVariables.Id " +
                    "LEFT JOIN TaxonomyVariableLabels ON TaxonomyVariableLabels.IdTaxonomyVariable = TaxonomyVariables.Id " +
                    "WHERE (TaxonomyCategoryLabels.IdLanguage={0} AND (TaxonomyCategoryLabels.Label like {1} OR TaxonomyCategoryLabels.Label like {2} OR TaxonomyCategoryLabels.Label like {3} OR TaxonomyCategoryLabels.Label like {4})) " +
                    "OR (TaxonomyVariableLabels.IdLanguage={0} AND (TaxonomyVariableLabels.Label like {1} OR TaxonomyVariableLabels.Label like {2} OR TaxonomyVariableLabels.Label like {3} OR TaxonomyVariableLabels.Label like {4})) ORDER BY TaxonomyVariableLabels.Label, TaxonomyCategories.[Order]",
                    new object[] { idLanguage, searchExpression, searchExpression + "%", "%" + searchExpression, "%" + searchExpression + "%" }
                ));
            }

            if (categorySearch.EnableNonCategorical && (categorySearch.SearchMode == CategorySearchMode.All || categorySearch.SearchMode == CategorySearchMode.Taxonomy))
            {
                searchResults.AddRange(Global.Core.TaxonomyVariables.ExecuteReader(
                    "SELECT TaxonomyVariables.Id, TaxonomyVariables.Type, TaxonomyVariables.Name, TaxonomyVariableLabels.Label, NULL, NULL, NULL, 1 FROM TaxonomyVariables " +
                    "LEFT JOIN TaxonomyVariableLabels ON TaxonomyVariableLabels.IdTaxonomyVariable = TaxonomyVariables.Id " +
                    "WHERE (TaxonomyVariableLabels.IdLanguage={0} AND (TaxonomyVariableLabels.Label like {1} OR TaxonomyVariableLabels.Label like {2} OR TaxonomyVariableLabels.Label like {3} OR TaxonomyVariableLabels.Label like {4}))",
                    new object[] { idLanguage, searchExpression, searchExpression + "%", "%" + searchExpression, "%" + searchExpression + "%" }
                ));
            }

            // Check if any taxonomy categories where found.
            if ((categorySearch.SearchMode == CategorySearchMode.All && searchResults.Count == 0) || categorySearch.SearchMode == CategorySearchMode.Study)
            {
                if (!string.IsNullOrEmpty(searchExpression))
                {
                    searchResults.AddRange(Global.Core.TaxonomyVariables.ExecuteReader(
                        "SELECT Variables.Id, Variables.Type, Variables.Name, VariableLabels.Label, Categories.Id, Categories.Name, CategoryLabels.Label, 0, (SELECT Name FROM Studies WHERE Id=Variables.IdStudy), Variables.IdStudy FROM Categories " +
                        "LEFT JOIN CategoryLabels ON Categories.Id = CategoryLabels.IdCategory " +
                        "LEFT JOIN Variables ON Categories.IdVariable = Variables.Id " +
                        "LEFT JOIN VariableLabels ON VariableLabels.IdVariable = Variables.Id " +
                        "WHERE (VariableLabels.IdLanguage={0} AND (VariableLabels.Label like {1} OR VariableLabels.Label like {2} OR VariableLabels.Label like {3} OR VariableLabels.Label like {4})) " +
                        "ORDER BY Variables.Id",
                        new object[] { idLanguage, searchExpression, searchExpression + "%", "%" + searchExpression, "%" + searchExpression + "%" }
                    ));

                    searchResults.AddRange(Global.Core.TaxonomyVariables.ExecuteReader(
                        "SELECT Variables.Id, Variables.Type, Variables.Name, VariableLabels.Label, Categories.Id, Categories.Name, CategoryLabels.Label, 0, (SELECT Name FROM Studies WHERE Id=Variables.IdStudy), Variables.IdStudy FROM Categories " +
                        "LEFT JOIN CategoryLabels ON Categories.Id = CategoryLabels.IdCategory " +
                        "LEFT JOIN Variables ON Categories.IdVariable = Variables.Id " +
                        "LEFT JOIN VariableLabels ON VariableLabels.IdVariable = Variables.Id " +
                        "WHERE " +
                        "(CategoryLabels.IdLanguage={0} AND (CategoryLabels.Label like {1} OR CategoryLabels.Label like {2} OR CategoryLabels.Label like {3} OR CategoryLabels.Label like {4})) ORDER BY Variables.Id",
                        new object[] { idLanguage, searchExpression, searchExpression + "%", "%" + searchExpression, "%" + searchExpression + "%" }
                    ));
                }
                else
                {
                    searchResults.AddRange(Global.Core.TaxonomyVariables.ExecuteReader(
                        "SELECT Variables.Id, Variables.Type, Variables.Name, VariableLabels.Label, Categories.Id, Categories.Name, CategoryLabels.Label, 0, (SELECT Name FROM Studies WHERE Id=Variables.IdStudy), Variables.IdStudy FROM Categories " +
                        "LEFT JOIN CategoryLabels ON Categories.Id = CategoryLabels.IdCategory " +
                        "LEFT JOIN Variables ON Categories.IdVariable = Variables.Id " +
                        "LEFT JOIN VariableLabels ON VariableLabels.IdVariable = Variables.Id " +
                        "ORDER BY Variables.Id"
                    ));
                }
            }

            Dictionary<Guid, List<object[]>> variables = new Dictionary<Guid, List<object[]>>();

            int i = 0;
            Guid? idLastProcessedVariable = null;
            // Run through all search results.
            foreach (object[] searchResult in searchResults)
            {
                if (idLastProcessedVariable.HasValue && idLastProcessedVariable.Value != (Guid)searchResult[0])
                {
                    break;
                }

                if (searchResult[0] == null)
                    continue;

                if (hierarchyFilter != null)
                {
                    if ((int)searchResult[7] == 1)
                    {
                        if (!hierarchyFilter.TaxonomyVariables.ContainsKey((Guid)searchResult[0]))
                            continue;

                        if (searchResult[4] != null && hierarchyFilter.TaxonomyCategories.ContainsKey((Guid)searchResult[4]) == false)
                            continue;
                    }
                    else
                    {
                        if (!hierarchyFilter.Variables.ContainsKey((Guid)searchResult[0]))
                            continue;
                    }
                }

                if (categorySearch.CheckDisplayMethod != null && categorySearch.CheckDisplayMethod((Guid)searchResult[4], true) == false)
                {
                    continue;
                }

                if (!variables.ContainsKey((Guid)searchResult[0]))
                    variables.Add((Guid)searchResult[0], new List<object[]>());

                variables[(Guid)searchResult[0]].Add(searchResult);

                if (i++ >= limit)
                {
                    idLastProcessedVariable = (Guid)searchResult[0];
                }
            }

            string inputType = "checkbox";
            string name = "";

            if (categorySearch.SelectionType == CategorySearchSelectionType.Single)
            {
                inputType = "radio";
                name = string.Format(
                    "name=\"chkCategorySearchResult{0}\"",
                    context.Request.Params["Id"]
                );
            }

            // Run through all variables in the search results.
            foreach (Guid idVariable in variables.Keys)
            {
                result.Append(string.Format(
                    "<table cellspacing=\"0\" cellpadding=\"0\"><tr><td style=\"padding:0px;\"><img style=\"height:30px;\" src=\"/Images/Icons/VariableSelector/{0}.png\" />" +
                    "</td><td class=\"CategorySearchResultVariable BackgroundColor1\" colspan=\"3\">{2}{1}</td></tr>",
                    (VariableType)variables[idVariable][0][1],
                    (string)variables[idVariable][0][3],
                    (((int)variables[idVariable][0][7]) == 0) ? variables[idVariable][0][8] + " - " : ""
                ));

                foreach (object[] searchResult in variables[idVariable])
                {
                    result.Append(string.Format(
                        "<tr><td class=\"BackgroundColor1\" style=\"width:20px\"><td style=\"width:30px;padding:2px;\">" +
                        "<input VariableName=\"{0}\" CategoryName=\"{1}\" IsTaxonomy=\"{2}\" IdStudy=\"{7}\" IdCategory=\"{3}\" {4} " +
                        "type=\"{5}\" onclick=\"ToggleSearchCategoryItem(this);\" />" + "</td><td>{6}</td></tr>",
                        searchResult[2],
                        searchResult[5],
                        (((int)searchResult[7]) == 1).ToString().ToLower(),
                        searchResult[4],
                        name,
                        inputType,
                        searchResult[6],
                        (int)searchResult[7] == 1 == true ? "" : (searchResult[9].ToString())
                    ));
                }

                result.Append("</table>");
            }

            context.Response.Write(result.ToString());
        }

        private Dictionary<Guid, Dictionary<Guid, string[]>> FilterSearchTaxonomyCategories(string searchExpression)
        {
            List<object[]> taxonomyCategoryLabels = new List<object[]>();
            List<object[]> taxonomyVariableLabels = new List<object[]>();

            int limit = 20;

            if (!string.IsNullOrEmpty(searchExpression))
            {
                taxonomyCategoryLabels = Global.Core.TaxonomyCategoryLabels.ExecuteReader(
                    "SELECT Id, IdTaxonomyVariable, Name, (SELECT Label FROM TaxonomyCategoryLabels WHERE IdTaxonomyCategory=TaxonomyCategories.Id) as Label " +
                    "FROM TaxonomyCategories WHERE Id IN (SELECT IdTaxonomyCategory FROM TaxonomyCategoryLabels WHERE " +
                    "(Label like {0} OR Label like {1} OR Label like {2} OR Label like {3})) ORDER BY Label",
                    new object[] { searchExpression, searchExpression + "%", "%" + searchExpression, "%" + searchExpression + "%" }
                );
                taxonomyVariableLabels = Global.Core.TaxonomyCategoryLabels.ExecuteReader(
                    "SELECT DISTINCT IdTaxonomyVariable, Label FROM TaxonomyVariableLabels WHERE (Label like {0} OR Label like {1} OR Label like {2} OR Label like {3}) ORDER BY Label",
                    new object[] { searchExpression, searchExpression + "%", "%" + searchExpression, "%" + searchExpression + "%" }
                );
            }
            else
            {
                taxonomyVariableLabels.AddRange(Global.Core.TaxonomyVariableLabels.GetValues(
                    new string[] { "IdTaxonomyVariable", "Label" },
                    new string[] { },
                    new object[] { },
                    "Label"
                ));
            }

            Dictionary<Guid, Dictionary<Guid, string[]>> taxonomyCategories = new Dictionary<Guid, Dictionary<Guid, string[]>>();

            // Run through all categories found.
            foreach (object[] taxonomyCategory in taxonomyCategoryLabels)
            {
                Guid idTaxonomyVariable = (Guid)taxonomyCategory[1];

                //categories.Add(new KeyValuePair<Guid, string>((Guid)category[0], variableLabel));
                if (!taxonomyCategories.ContainsKey(idTaxonomyVariable))
                    taxonomyCategories.Add(idTaxonomyVariable, new Dictionary<Guid, string[]>());

                if (!taxonomyCategories[idTaxonomyVariable].ContainsKey((Guid)taxonomyCategory[0]))
                {
                    taxonomyCategories[idTaxonomyVariable].Add((Guid)taxonomyCategory[0], new string[] {
                        (string)taxonomyCategory[3],
                        (string)taxonomyCategory[2]
                    });
                }
            }

            int i = 0;
            // Run through all variables found.
            foreach (object[] taxonomyVariable in taxonomyVariableLabels)
            {
                /*List<object[]> taxonomyVariableCategories = base.Core.TaxonomyCategories.GetValues(
                    new string[] { "Id", "Name", "(SELECT Label FROM TaxonomyCategoryLabels WHERE IdTaxonomyCategory=TaxonomyCategories.Id)" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { (Guid)taxonomyVariable[0] },
                    "Order"
                );*/
                List<object[]> taxonomyVariableCategories = Global.Core.TaxonomyCategories.ExecuteReader(
                    "SELECT Id, Name, (SELECT Label FROM TaxonomyCategoryLabels WHERE IdTaxonomyCategory=TaxonomyCategories.Id) FROM TaxonomyCategories WHERE IdTaxonomyVariable={0}",
                    new object[] { taxonomyVariable[0] }
                );

                if (!taxonomyCategories.ContainsKey((Guid)taxonomyVariable[0]))
                    taxonomyCategories.Add((Guid)taxonomyVariable[0], new Dictionary<Guid, string[]>());

                // Run through all categories of the variable.
                foreach (object[] category in taxonomyVariableCategories)
                {
                    if (!taxonomyCategories[(Guid)taxonomyVariable[0]].ContainsKey((Guid)category[0]))
                    {
                        taxonomyCategories[(Guid)taxonomyVariable[0]].Add((Guid)category[0], new string[] {
                            (string)category[1],
                            (string)category[2]
                        });
                    }

                    i++;
                }

                if (i >= limit)
                    break;
            }

            return taxonomyCategories;
        }

        private Dictionary<Guid, Dictionary<Guid, string[]>> FilterSearchCategories(string searchExpression)
        {
            List<object[]> categoryLabels = new List<object[]>();
            List<object[]> variableLabels = new List<object[]>();

            int limit = 20;

            if (!string.IsNullOrEmpty(searchExpression))
            {
                categoryLabels = Global.Core.TaxonomyCategoryLabels.ExecuteReader(
                    "SELECT Id, IdVariable, Name, (SELECT Label FROM CategoryLabels WHERE IdCategory=Categories.Id) as Label FROM Categories WHERE Id IN (SELECT IdCategory FROM CategoryLabels WHERE (Label like '{0}' OR Label like '{0}%' OR Label like '%{0}' OR Label like '%{0}%')) ORDER BY Label",
                    new object[] { searchExpression }
                );
                variableLabels = Global.Core.TaxonomyCategoryLabels.ExecuteReader(
                    "SELECT DISTINCT IdVariable, Label FROM VariableLabels WHERE (Label like '{0}' OR Label like '{0}%' OR Label like '%{0}' OR Label like '%{0}%') ORDER BY Label",
                    new object[] { searchExpression }
                );
            }
            else
            {
                variableLabels.AddRange(Global.Core.VariableLabels.GetValues(
                    new string[] { "IdVariable", "Label" },
                    new string[] { },
                    new object[] { },
                    "Label"
                ));
            }

            Dictionary<Guid, Dictionary<Guid, string[]>> categories = new Dictionary<Guid, Dictionary<Guid, string[]>>();

            // Run through all categories found.
            foreach (object[] category in categoryLabels)
            {
                Guid idVariable = (Guid)category[1];

                //categories.Add(new KeyValuePair<Guid, string>((Guid)category[0], variableLabel));
                if (!categories.ContainsKey(idVariable))
                    categories.Add(idVariable, new Dictionary<Guid, string[]>());

                if (!categories[idVariable].ContainsKey((Guid)category[0]))
                {
                    categories[idVariable].Add((Guid)category[0], new string[] {
                        (string)category[2],
                        (string)category[3]
                    });
                }
            }

            int i = 0;
            // Run through all variables found.
            foreach (object[] variable in variableLabels)
            {
                /*List<object[]> variableCategories = base.Core.Categories.GetValues(
                    new string[] { "Id" },
                    new string[] { "IdVariable" },
                    new object[] { (Guid)variable[0] },
                    "Order"
                );*/
                List<object[]> variableCategories = Global.Core.Categories.ExecuteReader(string.Format(
                    "SELECT Id, Name, (SELECT Label FROM CategoryLabels WHERE CategoryLabels.IdCategory=Categories.Id) FROM Categories WHERE IdVariable='{0}'",
                    variable[0]
                ));

                if (!categories.ContainsKey((Guid)variable[0]))
                    categories.Add((Guid)variable[0], new Dictionary<Guid, string[]>());

                // Run through all categories of the variable.
                foreach (object[] category in variableCategories)
                {
                    if (!categories[(Guid)variable[0]].ContainsKey((Guid)category[0]))
                    {
                        categories[(Guid)variable[0]].Add((Guid)category[0], new string[] {
                            (string)category[1],
                            (string)category[2]
                        });
                    }

                    i++;
                }

                if (i >= limit)
                    break;
            }

            return categories;
        }

        #endregion
    }

    public delegate void Meth(HttpContext context);
}