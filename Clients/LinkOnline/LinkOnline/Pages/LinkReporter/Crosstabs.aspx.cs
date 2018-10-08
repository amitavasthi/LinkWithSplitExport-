using Crosstables.Classes;
using Crosstables.Classes.HierarchyClasses;
using Crosstables.Classes.ReportDefinitionClasses;
using Crosstables.Classes.ReportDefinitionClasses.Collections;
using Crosstables.Classes.WorkflowClasses;
using DatabaseCore.Items;
using DataCore.Classes;
using MasterPage.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkOnline.Pages.LinkReporter
{
    public partial class Crosstabs : WebUtilities.BasePage
    {
        #region Properties

        /*string[] WeightingVariables = new string[] {
            "MUSIC-GENC",
            "MAINGENCSAMPLE"
        };*/
        public string[] WeightingVariables
        {
            get
            {
                if (HttpContext.Current.Session["WeightingVariables"] == null)
                {
                    HttpContext.Current.Session["WeightingVariables"] = new string[0];
                }

                return (string[])HttpContext.Current.Session["WeightingVariables"];
            }
            set
            {
                HttpContext.Current.Session["WeightingVariables"] = value;
            }
        }

        public Guid? SelectedStudy
        {
            get
            {
                if (HttpContext.Current.Session["SelectedStudy"] == null)
                    return null;

                return (Guid)HttpContext.Current.Session["SelectedStudy"];
            }
            set
            {
                HttpContext.Current.Session["SelectedStudy"] = value;
            }
        }

        public string SelectedWeightingVariable
        {
            get
            {
                if (HttpContext.Current.Session["SelectedWeightingVariable"] == null)
                    return null;

                return (string)HttpContext.Current.Session["SelectedWeightingVariable"];
            }
            set
            {
                HttpContext.Current.Session["SelectedWeightingVariable"] = value;
            }
        }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void BindSettings(ReportDefinition reportDefinition)
        {
            ddlLeftPanelSettingsSignificanceweight.SelectedValue = reportDefinition.Settings.SignificanceWeight.ToString();
            ddlLeftPanelSettingsLowBaseConsider.SelectedValue = reportDefinition.Settings.LowBaseConsider.ToString();
            ddlLeftPanelSettingsSignificanceTestLevel.SelectedValue = reportDefinition.Settings.SignificanceTestLevel.ToString();
            chkLeftPanelSettingsDisplayUnweightedBase.Checked = reportDefinition.Settings.DisplayUnweightedBase;
            chkLeftPanelSettingsDisplayEffectiveBase.Checked = reportDefinition.Settings.DisplayEffectiveBase;
            chkLeftPanelSettingsSignificanceTest.Checked = reportDefinition.Settings.SignificanceTest;
            txtLeftPanelSettingsDecimalPlaces.Text = reportDefinition.Settings.DecimalPlaces.ToString();
            //chkLeftPanelSettingsDataCheckEnabled.Checked = reportDefinition.Settings.DataCheckEnabled;
            chkLeftPanelSettingsScrollLabels.Checked = reportDefinition.Settings.ScrollLabels;
            chkLeftPanelSettingsHideEmptyRowsAndColumns.Checked = reportDefinition.Settings.HideEmptyRowsAndColumns;
            chkLeftPanelSettingsRankLeft.Checked = reportDefinition.Settings.RankLeft;
            chkLeftPanelSettingsRankTop.Checked = reportDefinition.Settings.RankTop;
            //chkLeftPanelSettingsSigDiffEffectiveBase.Checked = reportDefinition.Settings.SigDiffEffectiveBase;
            txtLeftPanelSettingsCategoryLimit.Text = reportDefinition.Settings.CategoryLimit.ToString();
            txtLeftPanelSettingsLowBase.Text = reportDefinition.Settings.LowBase.ToString();
            ddlLeftPanelSettingsSignificanceTestType.SelectedValue = reportDefinition.Settings.SignificanceTestType.ToString();

            txtLeftPanelSettingsMinWidth.Text = reportDefinition.Settings.MinWidth.ToString();
            txtLeftPanelSettingsMinHeight.Text = reportDefinition.Settings.MinHeight.ToString();

            if (!reportDefinition.Settings.SignificanceTest)
            {
                reportDefinition.Settings.SignificanceTestType = 0;
                ddlLeftPanelSettingsSignificanceTestType.SelectedValue = "0";
            }


            //if (reportDefinition.Settings.SignificanceTest == true && reportDefinition.Settings.SignificanceTestType.ToString() == "0")
            //{
            //    reportDefinition.Settings.SignificanceTestType = 0;
            //    ddlLeftPanelSettingsSignificanceTestType.SelectedValue = "0";
            //}
            //ddlLeftPanelSettingsSignificanceTestType.SelectedValue = reportDefinition.Settings.SignificanceTestType.ToString();

            // Get all available translations of the taxonomy
            List<object[]> languages = Global.Core.TaxonomyVariableLabels.
                ExecuteReader("SELECT DISTINCT IdLanguage FROM TaxonomyVariableLabels");

            // Run through all available translations.
            /*foreach (object[] language in languages)
            {
                CultureInfo cultureInfo = new CultureInfo((int)language[0]);

                ListItem lItem = new ListItem();
                lItem.Text = cultureInfo.DisplayName;
                lItem.Value = cultureInfo.LCID.ToString();

                ddlLeftPanelSettingsMetadataLanguage.Items.Add(lItem);
            }

            ddlLeftPanelSettingsMetadataLanguage.SelectedValue = reportDefinition.Settings.IdLanguage.ToString();*/


            if (reportDefinition.HasData == false && (reportDefinition.LeftVariables.Count > 0 || reportDefinition.TopVariables.Count > 0))
                pnlGoButton.CssClass = "GoButton2 GreenBackground2I";

            VariableSearch.Source = reportDefinition.FileName.Replace("\\", "/");
            VariableSearch.IdLanguage = reportDefinition.Settings.IdLanguage;
            //VariableSearch.DataCheckClientId = chkLeftPanelSettingsDataCheckEnabled.ClientID;

            /*Added for hide and show the PowerBi option - Bug No 385*/

            if (Global.User.HasPermission(Global.PermissionCore.Permissions["PowerBi"]))
            {
                pnlRightPanelConnectPowerBI.Visible = true;
            }
            else
            {
                pnlRightPanelConnectPowerBI.Visible = false;
            }

            /*Ends here*/

            ddlLeftPanelSettingsBaseType.Items.Add(new ListItem(HttpUtility.HtmlDecode(Global.LanguageManager.GetText("SettingsBaseTypeAnsweringBase")), "0"));
            ddlLeftPanelSettingsBaseType.Items.Add(new ListItem(HttpUtility.HtmlDecode(Global.LanguageManager.GetText("SettingsBaseTypeTotalBase")), "1"));

            ddlLeftPanelSettingsBaseType.SelectedValue = "0";
            if (reportDefinition.Settings.BaseType == BaseType.TotalBase)
                ddlLeftPanelSettingsBaseType.SelectedValue = "1";

            ddlLeftPanelSettingsDisplay.Items.Add(new ListItem(HttpUtility.HtmlDecode(Global.LanguageManager.GetText("SettingsDisplayBoth")), "0"));
            ddlLeftPanelSettingsDisplay.Items.Add(new ListItem(HttpUtility.HtmlDecode(Global.LanguageManager.GetText("SettingsDisplayValues")), "1"));
            ddlLeftPanelSettingsDisplay.Items.Add(new ListItem(HttpUtility.HtmlDecode(Global.LanguageManager.GetText("SettingsDisplayPercentages")), "2"));

            if (reportDefinition.Settings.ShowValues && reportDefinition.Settings.ShowPercentage)
                ddlLeftPanelSettingsDisplay.SelectedValue = "0";
            else if (reportDefinition.Settings.ShowValues)
                ddlLeftPanelSettingsDisplay.SelectedValue = "1";
            else if (reportDefinition.Settings.ShowPercentage)
                ddlLeftPanelSettingsDisplay.SelectedValue = "2";



            if (Request.Params["SecretSettings"] != null)
            {
                ddlSecretSettingsPercentageBase.SelectedValue = reportDefinition.Settings.PercentageBase.ToString();
                ddlSecretSettingsPowerBIValues.SelectedValue = reportDefinition.Settings.PowerBIValues.ToString();

                boxSecretSettings.Visible = true;

                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "ShowSecretSettings",
                    "InitDragBox('boxSecretSettingsControl');",
                    true
                );
            }
        }

        private void BindFilteredCategories(ReportDefinition reportDefinition)
        {
            // Run through all applied filter categories.
            foreach (FilterCategoryOperator filterCategoryOperator in reportDefinition.FilterCategories)
            {
                filterCategoryOperator.OnChange += filterCategoryOperator_OnChange;

                pnlFilterCategories.Controls.Add(
                    filterCategoryOperator
                );
            }
        }

        private void BindWeightingDefinition(ReportDefinition reportDefinition)
        {
            ddlDefaultWeighting.Items.Add(new ListItem()
            {
                Text = Global.LanguageManager.GetText("None"),
                Value = (new Guid()).ToString()
            });

            HierarchyFilter hierarchyFilter = Global.HierarchyFilters[reportDefinition.FileName];

            // Get all as weighting variable defined variables.
            List<object[]> weightingVariables = Global.Core.TaxonomyVariables.GetValues(
                new string[] { "Id" },
                new string[] { "Weight" },
                new object[] { true }
            );

            // Run through all weighting variables.
            foreach (object[] weightingVariable in weightingVariables)
            {

                if (hierarchyFilter.TaxonomyVariables.ContainsKey(Guid.Parse(weightingVariable[0].ToString())))
                {
                    // Create a new list item for the weighting variable.
                    ddlDefaultWeighting.Items.Add(new ListItem()
                    {
                        Value = weightingVariable[0].ToString(),
                        Text = (string)Global.Core.TaxonomyVariableLabels.GetValue(
                            "Label",
                            new string[] { "IdTaxonomyVariable", "IdLanguage" },
                            new object[] { weightingVariable[0], reportDefinition.Settings.IdLanguage }
                        )
                    });

                }
            }

            if (reportDefinition.WeightingFilters.DefaultWeighting.HasValue)
            {
                ddlDefaultWeighting.SelectedValue = reportDefinition.WeightingFilters.DefaultWeighting.Value.ToString();

                if (ddlDefaultWeighting.SelectedValue != reportDefinition.WeightingFilters.DefaultWeighting.Value.ToString())
                {
                    string label = "";

                    if (reportDefinition.WeightingFilters.XmlNode.Attributes["IsTaxonomy"] != null &&
                        bool.Parse(reportDefinition.WeightingFilters.XmlNode.Attributes["IsTaxonomy"].Value) == false)
                    {
                        label = (string)Global.Core.VariableLabels.GetValue(
                            "Label",
                            new string[] { "IdVariable", "IdLanguage" },
                            new object[] { reportDefinition.WeightingFilters.DefaultWeighting.Value, reportDefinition.Settings.IdLanguage }
                        );
                    }
                    else
                    {
                        label = (string)Global.Core.TaxonomyVariableLabels.GetValue(
                            "Label",
                            new string[] { "IdTaxonomyVariable", "IdLanguage" },
                            new object[] { reportDefinition.WeightingFilters.DefaultWeighting.Value, reportDefinition.Settings.IdLanguage }
                        );
                    }

                    // Create a new list item for the weighting variable.
                    ddlDefaultWeighting.Items.Add(new ListItem()
                    {
                        Value = reportDefinition.WeightingFilters.DefaultWeighting.Value.ToString(),
                        Text = label
                    });

                    ddlDefaultWeighting.SelectedValue = reportDefinition.WeightingFilters.DefaultWeighting.Value.ToString();
                }
            }

            ddlDefaultWeighting.Attributes.Add(
                "onchange",
                "SetOverallWeightingVariable(this.value)"
            );
        }

        private void BindReports(List<string> reports)
        {
            string selectedReport = HttpContext.Current.Session["ReportDefinition"].ToString();

            pnlFilterDefinitionAllTabs.Style["display"] = "none";
            if (reports.Count >= 2)
            {
                pnlFilterDefinitionAllTabs.Style["display"] = "block";
            }

            // Run through all defined reports of the user.
            foreach (string report in reports)
            {
                FileInfo fInfo = new FileInfo(report);

                if (fInfo.Name == "Info.xml" || fInfo.Name == "VariableSearch.xml")
                    continue;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(report);

                string reportName = "";

                if (xmlDocument.DocumentElement.Attributes["Name"] != null)
                    reportName = xmlDocument.DocumentElement.Attributes["Name"].Value;

                Panel pnlReportTab = new Panel();
                pnlReportTab.CssClass = "ReportTab";

                if (report == selectedReport)
                    pnlReportTab.CssClass += " Color1I ReportTab_Active";
                else
                    pnlReportTab.CssClass += " BackgroundColor10";

                HtmlGenericControl lblReportTabName = new HtmlGenericControl("div");
                lblReportTabName.ID = "lblReportTabName" + fInfo.Name;
                lblReportTabName.Attributes.Add("class", "ReportTabLabel");
                lblReportTabName.InnerText = reportName;

                pnlReportTab.Attributes.Add("onclick", string.Format(
                    "ChangeReportTab(this, '{0}');",
                    fInfo.Name
                ));
                pnlReportTab.Attributes.Add("oncontextmenu", string.Format(
                    "ShowReportTabContextMenu(this, '{0}');return false;",
                    fInfo.Name
                ));


                pnlReportTab.Controls.Add(lblReportTabName);

                pnlReportTabs.Controls.Add(pnlReportTab);
            }

            Image imgAddReportTab = new Image();
            imgAddReportTab.CssClass = "BtnReportTabAdd BackgroundColor6";
            imgAddReportTab.ImageUrl = "/Images/Icons/ReporterAddTab.png";
            imgAddReportTab.Style.Add("cursor", "pointer");

            imgAddReportTab.Attributes.Add(
                "onclick",
                "CreateNewReportTab();"
            );

            imgAddReportTab.Attributes.Add(
                "onmouseover",
                string.Format("ShowToolTip(this, '{0}', false, 'Bottom');", Global.LanguageManager.GetText("New"))
            );

            pnlReportTabs.Controls.Add(imgAddReportTab);

            Image imgDuplicateReportTab = new Image();
            imgDuplicateReportTab.CssClass = "BtnReportTabAdd BackgroundColor6";
            imgDuplicateReportTab.ImageUrl = "/Images/Icons/Duplicate.png";
            imgDuplicateReportTab.Style.Add("cursor", "pointer");

            imgDuplicateReportTab.Attributes.Add(
                "onclick",
                "DuplicateReportTab();"
            );

            imgDuplicateReportTab.Attributes.Add(
            "onmouseover",
            string.Format("ShowToolTip(this, '{0}', false, 'Bottom');", Global.LanguageManager.GetText("Copy"))
        );

            pnlReportTabs.Controls.Add(imgDuplicateReportTab);

            // start code to implement Undo and redo
            Image imgUndoReportTab = new Image();
            imgUndoReportTab.CssClass = "BtnReportTabAdd BackgroundColor6";
            imgUndoReportTab.ImageUrl = "/Images/Icons/Undo.png";
            imgUndoReportTab.Style.Add("cursor", "pointer");
            imgUndoReportTab.Style.Add("padding", "2px");

            imgUndoReportTab.Attributes.Add(
                "onclick",
                "UndoReportTab();"
            );

            imgUndoReportTab.Attributes.Add(
            "onmouseover",
            string.Format("ShowToolTip(this, '{0}', false, 'Bottom');", Global.LanguageManager.GetText("Undo"))
        );

            //    Image imgRedoReportTab = new Image();
            //    imgRedoReportTab.CssClass = "BtnReportTabAdd BackgroundColor6";
            //    imgRedoReportTab.ImageUrl = "/Images/Icons/Duplicate.png";
            //    imgRedoReportTab.Style.Add("cursor", "pointer");

            //    imgRedoReportTab.Attributes.Add(
            //        "onclick",
            //        "RedoReportTab();"
            //    );

            //    imgRedoReportTab.Attributes.Add(
            //    "onmouseover",
            //    string.Format("ShowToolTip(this, '{0}', false, 'Bottom');", Global.LanguageManager.GetText("Redo"))
            //);

            pnlReportTabs.Controls.Add(imgUndoReportTab);
            //  pnlReportTabs.Controls.Add(imgRedoReportTab);

            //end code to implement Undo and redo

        }


        private void CreateReport()
        {
            ReportDefinition reportDefinition;
            string fileName;

            string directoryName;

            if (this.Request.Params["SavedReport"] == null)
            {
                // Get the full path to the directory
                // where the saved report is saved.
                directoryName = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "ReportDefinitions",
                    Global.Core.ClientName,
                    Global.User.Id.ToString()
                );

                lblPageTitle.Text = Global.LanguageManager.GetText("Crosstabs");

                if (HttpContext.Current.Session["ActiveSavedReport"] != null && Directory.Exists((string)HttpContext.Current.Session["ActiveSavedReport"]))
                {
                    Directory.Delete((string)HttpContext.Current.Session["ActiveSavedReport"], true);
                }

                HttpContext.Current.Session["ActiveSavedReport"] = null;

                if (HttpContext.Current.Session["ReportDefinition"] != null && (new FileInfo(HttpContext.Current.Session["ReportDefinition"].ToString())).DirectoryName != directoryName)
                    HttpContext.Current.Session["ReportDefinition"] = null;
            }
            else
            {
                Guid idUser = Guid.Parse(this.Request.Params["SavedReport"].Substring(0, 36));
                Guid idReport = Guid.Parse(this.Request.Params["SavedReport"].Substring(36, 36));

                directoryName = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "Temp",
                    "OpenSavedReports",
                    HttpContext.Current.Session.SessionID,
                    this.Request.Params["SavedReport"]
                );

                if (HttpContext.Current.Session["ActiveSavedReport"] == null || Directory.Exists(directoryName) == false)
                {
                    // Get the full path to the directory
                    // where the user's reports are saved.

                    string sourceDirectoryName = "";
                    if (HttpContext.Current.Session["ManualSaveReportFolderSelect"] != null)
                    {
                        sourceDirectoryName = HttpContext.Current.Session["ManualSaveReportFolderSelect"].ToString() + "/" + idReport.ToString();
                    }


                    if (sourceDirectoryName == "")
                    {
                        sourceDirectoryName = Path.Combine(
                      Request.PhysicalApplicationPath,
                      "Fileadmin",
                      "SavedReports",
                      Global.Core.ClientName,
                      idUser.ToString(),
                      idReport.ToString()
                  );

                    }

                    if (HttpContext.Current.Session["LinkCloudSelectedReportUrl"] == null && HttpContext.Current.Session["ManualSaveReportFolderSelect"] != null)
                    {
                        HttpContext.Current.Session["LinkCloudSelectedReportUrl"] = HttpContext.Current.Session["ManualSaveReportFolderSelect"];
                    }


                    if (HttpContext.Current.Session["LinkCloudSelectedReportUrl"] == null || !Directory.Exists(sourceDirectoryName))
                    {
                        string[] paths = Directory.GetDirectories(Path.Combine(Request.PhysicalApplicationPath,
                           "Fileadmin",
                           "SavedReports",
                           Global.Core.ClientName,
                           idUser.ToString()), "*", SearchOption.AllDirectories);

                        if (paths.Where(x => x.ToLower().IndexOf(idReport.ToString().ToLower()) > -1) != null)
                        {
                            HttpContext.Current.Session["LinkCloudSelectedReportUrl"] = paths.Where(x => x.ToLower().IndexOf(idReport.ToString().ToLower()) > -1).FirstOrDefault();
                        }

                    }


                    if (HttpContext.Current.Session["LinkCloudSelectedReportUrl"].ToString().IndexOf(idReport.ToString()) != -1)
                    {
                        sourceDirectoryName = HttpContext.Current.Session["LinkCloudSelectedReportUrl"].ToString();
                    }


                    if (Directory.Exists(directoryName))
                        Directory.Delete(directoryName, true);

                    Directory.CreateDirectory(directoryName);

                    foreach (string file in Directory.GetFiles(sourceDirectoryName).OrderBy(d => new FileInfo(d).CreationTime))
                    {
                        File.Copy(file, Path.Combine(
                            directoryName,
                            new FileInfo(file).Name
                        ));
                        File.SetCreationTime(Path.Combine(
                            directoryName,
                            new FileInfo(file).Name
                        ), File.GetCreationTime(file));
                    }


                    //foreach (string file in Directory.GetFiles(sourceDirectoryName))
                    //{
                    //    File.Copy(file, Path.Combine(
                    //        directoryName,
                    //        new FileInfo(file).Name
                    //    ));
                    //}
                }

                if (HttpContext.Current.Session["ActiveSavedReport"] == null)
                    HttpContext.Current.Session["ReportDefinition"] = null;

                HttpContext.Current.Session["ActiveSavedReport"] = directoryName;

                if (HttpContext.Current.Session["ReportDefinition"] != null)
                {
                    if (new FileInfo(HttpContext.Current.Session["ReportDefinition"].ToString()).DirectoryName != directoryName)
                        HttpContext.Current.Session["ReportDefinition"] = null;
                }

                ReportDefinitionInfo savedReportInfo = new ReportDefinitionInfo(Path.Combine(
                    directoryName,
                    "Info.xml"
                ));

                lblPageTitle.Text = "<table style=\"display:inline-block\"><tbody style=\"display:inline-block\"><tr style=\"display:inline-block\"><td style=\"display:inline-block\"><img src=\"/Images/Icons/Back.png\" onclick=\"window.location='/Pages/LinkCloud.aspx'\" " +
                    "onmouseover=\"this.src='/Images/Icons/Back_Hover.png'\" onmouseout=\"this.src='/Images/Icons/Back.png'\" style=\"cursor:pointer;\" />" +
                    "</td><tr style=\"margin-top:-20px;display:inline-block \"><td style=\"display:inline-block\">" + savedReportInfo.Name + "</td></tr></tr></tbody></table>";
            }


            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            ReportDefinitionInfo info = new ReportDefinitionInfo(Path.Combine(
                directoryName,
                "Info.xml"
            ));

            info.LatestAccess = DateTime.Now;

            info.Save();

            // Get all the reports of the user.
            List<string> reports = info.GetReports(
                Global.Core,
                Global.IdUser.Value
            );

            if (HttpContext.Current.Session["ReportDefinition"] != null)
            {
                if (!reports.Contains(HttpContext.Current.Session["ReportDefinition"]))
                    HttpContext.Current.Session["ReportDefinition"] = null;
            }

            if (HttpContext.Current.Session["ReportDefinition"] == null && info.ActiveReport.HasValue)
            {
                HttpContext.Current.Session["ReportDefinition"] = Path.Combine(
                    directoryName,
                    info.ActiveReport.Value + ".xml"
                );
            }

            // Check if for the current session a report is selected.
            if (HttpContext.Current.Session["ReportDefinition"] == null ||
                File.Exists(HttpContext.Current.Session["ReportDefinition"].ToString()) == false)
            {
                // Check if the user has reports defined.
                if (reports.Count == 0)
                {
                    fileName = Path.Combine(
                        directoryName,
                        Guid.NewGuid().ToString() + ".xml"
                    );

                    string fileNameWorkflow = Path.Combine(
                        Request.PhysicalApplicationPath,
                        "App_Data",
                        "ReportingWorkflows",
                        Global.Core.ClientName + ".xml"
                    );

                    string fileNameWeighting = Path.Combine(
                        Request.PhysicalApplicationPath,
                        "App_Data",
                        "WeightingDefaults",
                        Global.Core.ClientName + ".xml"
                    );

                    if (!File.Exists(fileNameWeighting))
                        fileNameWeighting = null;

                    reportDefinition = new ReportDefinition(
                        Global.Core,
                        fileName,
                        fileNameWorkflow,
                        fileNameWeighting,
                        Global.HierarchyFilters[fileName, false],
                        Global.UserDefaults["ReportDefinitionSettings"]
                    );

                    reportDefinition.XmlDocument.DocumentElement.SetAttribute("Name", GetUniqueReportName(string.Format(Global.LanguageManager.GetText("NewReport"), "").Trim(), directoryName));

                    reportDefinition.Save();

                    reports = info.GetReports(
                        Global.Core,
                        Global.IdUser.Value
                    );
                }
                else
                {
                    // Select the first report as the selected report.
                    fileName = reports[0];
                }

                HttpContext.Current.Session["ReportDefinition"] = fileName;
            }
            else
            {
                // Get the full path to the currently selected report's definition file.
                fileName = HttpContext.Current.Session["ReportDefinition"].ToString();
            }
            // Get the full path of the current session's report definition file.
            string directory = (new FileInfo(HttpContext.Current.Session["ReportDefinition"].ToString())).DirectoryName;
            // Run through all files of the directory.
            foreach (string file in Directory.GetFiles(directory).OrderBy(x => new FileInfo(x).CreationTime))
            {
                if (Path.GetFileName(file) == "Info.xml")
                    continue;
                csFilterDefinition.Source = file;
                EquationDefinition.Source = file;

                HierarchySelector.FileName = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "App_Data",
                    "HierarchySelectors",
                    Global.Core.ClientName + ".xml"
                );

                HierarchySelector.Source = file;

                // Create a new report definition by the report definition file.
                reportDefinition = new ReportDefinition(
                    Global.Core,
                    file,
                    Global.HierarchyFilters[file]
                );
                bool hasNewCategories1 = reportDefinition.CheckForNewCategories();

                if (hasNewCategories1)
                {
                    reportDefinition.Save();

                    reportDefinition = new ReportDefinition(
                        Global.Core,
                        file,
                        Global.HierarchyFilters[file]
                    );

                    ReportCalculator calculator = new ReportCalculator(
                        reportDefinition,
                        Global.Core,
                        HttpContext.Current.Session
                    );

                    calculator.Aggregate((string)HttpContext.Current.Session["Version"]);
                }
                if (HierarchySelector.Exists)
                {
                    HierarchySelector.Parse();
                }
                reportDefinition.Save();

            }

            csFilterDefinition.Source = fileName;
            EquationDefinition.Source = fileName;

            HierarchySelector.FileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "HierarchySelectors",
                Global.Core.ClientName + ".xml"
            );

            HierarchySelector.Source = fileName;

            // Create a new report definition by the report definition file.
            reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );
            bool hasNewCategories = reportDefinition.CheckForNewCategories();

            if (hasNewCategories)
            {
                reportDefinition.Save();

                reportDefinition = new ReportDefinition(
                    Global.Core,
                    fileName,
                    Global.HierarchyFilters[fileName]
                );

                ReportCalculator calculator = new ReportCalculator(
                    reportDefinition,
                    Global.Core,
                    HttpContext.Current.Session
                );

                calculator.Aggregate((string)HttpContext.Current.Session["Version"]);
            }

            if (HierarchySelector.Exists)
            {
                HierarchySelector.Parse();

                int optionCount = 0;

                foreach (Classes.Controls.HierarchySelectorSection section in HierarchySelector.Sections.Values)
                {
                    optionCount += section.OptionCount;
                }

                if (optionCount <= 1)
                {
                    pnlRightPanelHierarchy.Visible = false;
                }
                else
                {
                    pnlRightPanelHierarchy.Attributes.Add("onclick", string.Format(
                        "InitDragBox('boxHierarchySelectorControl');LoadHierarchySelectedItems('{0}');",
                        HttpUtility.UrlEncode(fileName.Replace("\\", "/"))
                    ));

                    if (reportDefinition.XmlDocument.DocumentElement.SelectSingleNode("HierarchyFilter") == null)
                    {
                        Page.ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "ShowHierarchySelector",
                            string.Format("InitDragBox('boxHierarchySelectorControl');LoadHierarchySelectedItems('{0}');",
                            HttpUtility.UrlEncode(fileName.Replace("\\", "/"))),
                            true
                        );
                    }
                }
            }
            else
            {
                pnlRightPanelHierarchy.Visible = false;
            }

            if (base.ContentWidth != 0)
                reportDefinition.Settings.TableWidth = base.ContentWidth;

            if (base.ContentHeight != 0)
                reportDefinition.Settings.TableHeight = base.ContentHeight;

            reportDefinition.Save();

            Crosstables.Classes.Crosstable crosstable = new Crosstables.Classes.Crosstable(
                 Global.Core,
                 fileName
             );

            //crosstable.FilterClickAction = "ctl00$cphContent$btnDisplayFilters";

            pnl.Controls.Add(crosstable);

            BindSettings(reportDefinition);
            BindFilteredCategories(reportDefinition);
            BindWeightingDefinition(reportDefinition);
            BindReports(reports);

            if (reportDefinition.Workflow.Selections.Count > 0)
            {
                if (bool.Parse(Global.UserDefaults["BottomBarPinned", "false"]))
                {
                    pnlWorkflowContainer.CssClass = "WorkflowPinned BorderColor1";
                }

                pnlWorkflow.Controls.Add(reportDefinition.Workflow);
            }
            else
                pnlWorkflowContainer.Visible = false;

            if ((fileName.IndexOf(IdUser.ToString()) == -1) && (Convert.ToBoolean(HttpContext.Current.Session["RenderValues"]) == true))
            {
                HttpContext.Current.Session["RenderValues"] = false;
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "Javascript", "RenderValues();", true);
            }
        }

        private string GetUniqueReportName(string name, string directory)
        {

            bool foundUniqueName = true;
            int i = 2;

            while (true)
            {
                // Run through all tabs.
                foreach (string file in Directory.GetFiles(directory))
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
        private void ShowError()
        {
            Response.Redirect("ReportDefinitionError.aspx");
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            this.EnableViewState = false;
            try
            {
                CreateReport();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["LastReportDefinitionError"] = ex;

                ShowError();
            }

            // for treeview on save report option
            string path = Path.Combine(
                       HttpContext.Current.Request.PhysicalApplicationPath,
                       "Fileadmin",
                       "SavedReports",
                       Global.Core.ClientName
                  //, IdUser.ToString()
                   );
            string userPath = path + "/" + IdUser.ToString();
            //if (!Directory.Exists(path + "/" + IdUser.ToString()))
            //{
            //    Directory.CreateDirectory(path + "/" + IdUser.ToString());
            //}

            DirectoryInfo rootInfo = new DirectoryInfo(path);
            TreeView1.Nodes.Clear();
            this.PopulateTreeView(rootInfo, null, userPath);

            boxSettings.Visible = true;
            boxFilterDefinition.Visible = true;
            boxFilterSearch.Visible = true;
            boxSave.Visible = true;
            boxCombineCategoires.Visible = true;

            cogSrchApi.Value = "false";
            if (ConfigurationManager.AppSettings["cogSrchApi"] != null)
            {
                ConfigurationManager.AppSettings["cogSrchApi"].ToString();
            }
                
        }

        private void PopulateTreeView(DirectoryInfo dirInfo, TreeNode treeNode,string userPath)
        {
            TreeView1.NodeStyle.CssClass = "FileExplorerItem Color1";
            TreeView1.SelectedNodeStyle.CssClass = "FileExplorerItem_Active Color2";



            if (!Directory.Exists(userPath))
            {
                TreeNode directoryNode = new TreeNode
                {
                    Text = "my saved reports",
                    Value = userPath
                };

                directoryNode.NavigateUrl = "javascript: ManualSaveReportFolderSelect(this,'" + directoryNode.Value.Replace("\\", "/") + "'); ";

                TreeView1.Nodes.Add(directoryNode);

            }
            else 
            {
                foreach (DirectoryInfo directory in dirInfo.GetDirectories())
                {

                    //if (directory.FullName.IndexOf(IdUser.ToString()) == -1)
                    //    continue;

                    Guid guidOutput = Guid.Empty;
                    if (Guid.TryParse(directory.Name, out guidOutput))
                    {
                        if (!(directory.Name == IdUser.ToString()))
                        {
                            continue;
                        }
                    }

                    string Text = directory.Name;
                    if (Text == IdUser.ToString())
                    {
                        Text = "my saved reports";
                    }
                    else
                    {
                        Text = directory.Name;
                    }

                    TreeNode directoryNode = new TreeNode
                    {
                        Text = Text,
                        Value = directory.FullName
                    };

                    directoryNode.NavigateUrl = "javascript: ManualSaveReportFolderSelect(this,'" + directoryNode.Value.Replace("\\", "/") + "'); ";

                    if (treeNode == null)
                    {
                        //If Root Node, add to TreeView.
                        TreeView1.Nodes.Add(directoryNode);
                    }
                    else
                    {
                        //If Child Node, add to Parent Node.
                        treeNode.ChildNodes.Add(directoryNode);
                    }

                    //Get all files in the Directory.
                    //foreach (FileInfo file in directory.GetFiles())
                    //{
                    //    //Add each file as Child Node.
                    //    TreeNode fileNode = new TreeNode
                    //    {
                    //        Text = file.Name,
                    //        Value = file.FullName,
                    //        Target = "_blank",
                    //        NavigateUrl = (new Uri(Server.MapPath("~/"))).MakeRelativeUri(new Uri(file.FullName)).ToString()
                    //    };
                    //    directoryNode.ChildNodes.Add(fileNode);
                    //}

                    PopulateTreeView(directory, directoryNode, userPath); 
                }
            }
        }




        protected void btnAddWeightingFilter_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void lnkLeftPanelSectionNew_Click(object sender, EventArgs e)
        {
            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the file.
            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Clear the report definition.
            reportDefinition.Clear();

            // Clear the data check entries.
            DataCheck dataCheck = new DataCheck(fileName);

            dataCheck.Clear();

            // Self redirect to display clear crosstable.
            Response.Redirect(
                Request.Url.ToString()
            );
        }

        protected void lnkLeftPanelSectionExport_Click(object sender, EventArgs e)
        {
            // Get the full path of the current session's report definition file.
            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            // Create a new report definition by the file.
            ReportDefinition reportDefinition = new ReportDefinition(
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
                Request.PhysicalApplicationPath,
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

            string fName = exporter.Export();

            base.WriteFileToResponse(fName, "Export.xlsx", "application/msexcel", true);
        }


        protected void filterCategoryOperator_OnChange(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "ShowFilterSelector",
                "ShowFullScreenPanel('pnlFilterDefinition', false);",
                true
            );
        }

        protected void btnNewScoreGroupConfirm_Click(object sender, EventArgs e)
        {
            object idWeightingVariable = Global.Core.TaxonomyVariables.GetValue(
                "Id",
                "Weight",
                true
            );

            if (idWeightingVariable == null)
                return;

            string fileName = HttpContext.Current.Session["ReportDefinition"].ToString();

            ReportDefinition reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            reportDefinition.WeightingFilters.XmlNode.InnerXml += string.Format(
                "<Operator Id=\"{0}\" Type=\"AND\" WeightingVariable=\"{1}\">",
                Guid.NewGuid(),
                idWeightingVariable
            );

            reportDefinition.Save();

            reportDefinition = new ReportDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            BindWeightingDefinition(reportDefinition);

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "ShowWeightingFilterSelector",
                "ShowFullScreenPanel('pnlWeightingDefinition', false);",
                true
            );
        }

        #endregion
    }
}
