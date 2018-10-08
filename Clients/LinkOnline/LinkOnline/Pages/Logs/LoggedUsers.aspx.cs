using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using ApplicationUtilities;
using DatabaseCore.Items;
using WebUtilities.Classes.Controls.GridClasses;
using WebUtilities.Controls;
using System.Web.Security;
using WebUtilities.Classes;
using ApplicationUtilities.Classes;

namespace LinkOnline.Pages.Logs
{
    public partial class LoggedUsers : WebUtilities.BasePage
    {
        #region Properties
        private Grid gridUsers;
        #endregion

        #region Methods
        private void GetUsers()
        {
            try
            {
                gridUsers = new Grid { ID = "gridUsers" };

                var headline = new GridHeadline(gridUsers);

                headline.Items.Add(new GridHeadlineItem(headline, 0, "user name", new GridHeadlineItemWidth(20)));
                headline.Items.Add(new GridHeadlineItem(headline, 1, "email id", new GridHeadlineItemWidth(20)));
                headline.Items.Add(new GridHeadlineItem(headline, 2, "client", new GridHeadlineItemWidth(20)));
                headline.Items.Add(new GridHeadlineItem(headline, 3, "date", new GridHeadlineItemWidth(20)));
                headline.Items.Add(new GridHeadlineItem(headline, 4, "roles", new GridHeadlineItemWidth(10)));
                headline.Items.Add(new GridHeadlineItem(headline, 5, "browser", new GridHeadlineItemWidth(10)));


                gridUsers.GridHeadline = headline;

                ApplicationUtilities.UsageLogger logger = new ApplicationUtilities.UsageLogger(
                        Global.Core.ClientName,
                        Global.User
                    );
                //object[] user = logger.SelectSingle("SELECT Id FROM Variables WHERE Name={0}", "Users");
                //object[] email = logger.SelectSingle("SELECT Id FROM Variables WHERE Name={0}", "EmailId");
                //object[] login = logger.SelectSingle("SELECT Id FROM Variables WHERE Name={0}", "Login");
                //object[] client = logger.SelectSingle("SELECT Id FROM Variables WHERE Name={0}", "Clients");
                //object[] roles = logger.SelectSingle("SELECT Id FROM Variables WHERE Name={0}", "Roles");
                //object[] browsers = logger.SelectSingle("SELECT Id FROM Variables WHERE Name={0}", "Browser");
                List<object[]> respondent;
                if (ConfigurationManager.AppSettings["AdminPortal"] != null)
                {
                    if (Global.Core.ClientName.ToLower() == ConfigurationManager.AppSettings["AdminPortal"])
                        respondent = logger.ExecuteReader(string.Format(
                                        @"SELECT  [Id] ,[UserName],[EmailId],[Client],CONVERT(varchar,[Login],103)+' '+CONVERT(char(8),[Login],108) AS [Date],[Role],[Browser] FROM UsageLog ORDER BY [Date]"));
                    else
                        respondent = logger.ExecuteReader(string.Format(
                                       @"SELECT  [Id] ,[UserName],[EmailId],[Client],CONVERT(varchar,[Login],103)+' '+CONVERT(char(8),[Login],108) AS [Date],[Role],[Browser] FROM UsageLog WHERE [Client]='{0}' ORDER BY [Date] ", Global.Core.ClientName));
                }
                else
                {
                    respondent = logger.ExecuteReader(string.Format(
                                      @"SELECT  [Id] ,[UserName],[EmailId],[Client],CONVERT(varchar,[Login],103)+' '+CONVERT(char(8),[Login],108) AS [Date],[Role],[Browser] FROM UsageLog WHERE [Client]='{0}' ORDER BY [Date] ", Global.Core.ClientName));
                }
                if (respondent != null)
                {
                    foreach (var useritem in respondent)
                    {
                        var row = new GridRow(gridUsers, useritem[0]);
                        var name = new GridRowItem(row, useritem[1].ToString());
                        var emailid = new GridRowItem(row, useritem[2].ToString());
                        var clientname = new GridRowItem(row, useritem[3].ToString());
                        var date = new GridRowItem(row, useritem[4].ToString());
                        var role = new GridRowItem(row, useritem[5].ToString());
                        var browser = new GridRowItem(row, useritem[6].ToString());

                        row.Items.Add(name);
                        row.Items.Add(emailid);
                        row.Items.Add(clientname);
                        row.Items.Add(date);
                        row.Items.Add(role);
                        row.Items.Add(browser);
                        gridUsers.Rows.Add(row);
                    }
                    pnlUserManagement.Controls.Add(gridUsers);
                }

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("/Pages/ErrorPage.aspx");
            }

        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            GetUsers();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = Path.Combine(
                   Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "Temp",
                   "Exports",
                   HttpContext.Current.Session.SessionID,
                    "Usage.xlsx"
               );

            // Create a new excel reader by the temporary saved file.
            ExcelWriter writer = new ExcelWriter();
            writer.Write(0, "user name");
            writer.Write(1, "email id");
            writer.Write(2, "client");
            writer.Write(3, "date");
            writer.Write(4, "roles");
            writer.Write(5, "browser");

            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 5].Interior.Color = SpreadsheetGear.Color.FromArgb(54, 94, 146);
            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 5].Font.Color = SpreadsheetGear.Color.FromArgb(255, 255, 255);

            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 1].EntireColumn.ColumnWidth = 50;
            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 1].EntireColumn.WrapText = true;

            writer.ActiveSheet.WindowInfo.FreezePanes = true;
            writer.ActiveSheet.Cells[0, 0, 0, 5].Select();

            writer.NewLine();

            foreach (var detail in gridUsers.Rows)
            {
                writer.Write(0, (string)detail.Items[0].ToString());
                writer.Write(1, detail.Items[1].ToString());
                writer.Write(2, detail.Items[2].ToString());
                writer.Write(3, detail.Items[3].ToString());
                writer.Write(4, detail.Items[4].ToString());
                writer.Write(5, detail.Items[5].ToString());
                writer.NewLine();
            }

            writer.Save(fileName);

            Page.ClientScript.RegisterStartupScript(this.GetType(), "DownloadAction", string.Format(
              "window.open('/Fileadmin/Temp/Exports/{0}/{1}');",
              HttpContext.Current.Session.SessionID,
              "Usage.xlsx"
          ), true);

        }
    }
}