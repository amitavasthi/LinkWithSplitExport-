using Crosstables.Classes.ReportDefinitionClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkOnline.Pages.LinkReporter
{
    public partial class Studio : WebUtilities.BasePage
    {
        #region Properties

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void BindReports()
        {
            string directoryName;

            if(this.Request.Params["SavedReport"] == null)
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
            }
            else
            {
                directoryName = "";
            }

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

            string selectedReport;

            if (info.ActiveReport.HasValue)
            {
                selectedReport = Path.Combine(
                    directoryName,
                    info.ActiveReport.Value + ".xml"
                );
            }
            else
            {
                selectedReport = reports[0];
            }

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
                    report.Replace("\\", "/")
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

            pnlReportTabs.Controls.Add(imgAddReportTab);
        }

        #endregion


        #region Event Handler

        protected void Page_Load(object sender, EventArgs e)
        {
            lblPageTitle.Text = Global.LanguageManager.GetText("Crosstabs");

            BindReports();
        }

        #endregion
    }
}