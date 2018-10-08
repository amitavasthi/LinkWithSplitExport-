using ClientBackup1.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WebUtilities.Classes.Controls.GridClasses;
using WebUtilities.Controls;

namespace LinkOnline.Pages
{
    public partial class Backup : WebUtilities.BasePage
    {
        #region Properties

        public Grid gridBackups;

        #endregion


        #region Constructor

        public Backup()
        {

        }

        #endregion


        #region Methods

        private void BindBackups()
        {
            // Build the full path to the client's client backups definition file.
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "ClientBackups",
                Global.Core.ClientName + ".xml"
            );

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, string.Format(
                    "<ClientBackups Client=\"{0}\"></ClientBackups>",
                    Global.Core.ClientName
                ));
            }

            // Create a new client backup collection which
            // contains information of the client's backups.
            ClientBackupCollection clientBackups = new ClientBackupCollection(
                Global.Core.ClientName,
                fileName
            );

            gridBackups = new Grid();
            gridBackups.ID = "gridBackups";
            gridBackups.MaxHeight = base.ContentHeight - 200;

            GridHeadline headline = new GridHeadline(gridBackups);

            headline.Items.Add(new GridHeadlineItem(headline, 0, Global.LanguageManager.GetText("User"), new GridHeadlineItemWidth(40)));
            headline.Items.Add(new GridHeadlineItem(headline, 1, Global.LanguageManager.GetText("CreationDate"), new GridHeadlineItemWidth(40)));
            headline.Items.Add(new GridHeadlineItem(headline, 1, Global.LanguageManager.GetText("Status"), new GridHeadlineItemWidth(20)));

            gridBackups.GridHeadline = headline;

            Dictionary<Guid, string> studies = new Dictionary<Guid, string>();

            // Run through all client backups.
            foreach (ClientBackup clientBackup in clientBackups)
            {
                GridRow row = new GridRow(gridBackups, clientBackup.Id);

                string statusStr = "";

                if (clientBackup.Status == ClientBackupStatus.Running)
                {
                    statusStr = "<span class=\"Color2\" style=\"font-weight:bold;\">" + Global.LanguageManager.GetText(clientBackup.Status.ToString()) + "</span>";
                }
                else if (clientBackup.Status == ClientBackupStatus.Failed)
                {
                    statusStr = "<span onclick=\"ShowJavascriptBox('boxClientBackupError', '"+ HttpUtility.HtmlEncode(clientBackup.XmlNode.InnerXml).Replace(Environment.NewLine, "") +
                        "', undefined, false, 'ClientBackupErrorDetail');\" style=\"color:#FF0000;font-weight:bold;text-decoration:underline;cursor:pointer;\">" + Global.LanguageManager.GetText(clientBackup.Status.ToString()) + "</span>";
                }
                else
                {
                    statusStr = clientBackup.Status.ToString();
                }

                row.Items.Add(new GridRowItem(row, Global.GetNiceUsername(clientBackup.IdUser), false));
                row.Items.Add(new GridRowItem(row, clientBackup.CreationDate.ToString(
                    Global.LanguageManager.GetText("DateFormat") + " " +
                    Global.LanguageManager.GetText("TimeFormat")
                ), false));
                row.Items.Add(new GridRowItem(row, statusStr, true));

                gridBackups.Rows.Add(row);
            }

            pnlClientBackups.Controls.Add(gridBackups);
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            // Bind all the client's backups.
            BindBackups();
        }


        protected void btnBackup_Click(object sender, EventArgs e)
        {
            // Build the full path to the client's client backups definition file.
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "ClientBackups",
                Global.Core.ClientName + ".xml"
            );

            Guid idBackup = Guid.NewGuid();

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            document.DocumentElement.InnerXml += string.Format(
                "<ClientBackup Id=\"{0}\" CreationDate=\"{1}\" IdUser=\"{2}\" BackupPath=\"\" Status=\"Running\" FinishDate=\"\"></ClientBackup>",
                idBackup,
                DateTime.Now.ToString(),
                Global.IdUser.Value
            );

            document.Save(fileName);

            Process app = new Process();
            app.StartInfo.FileName = ConfigurationManager.AppSettings["BackupApplication"];

            app.StartInfo.Arguments = string.Format(
                "Database={0} ClientName={1} DatabaseProvider={2} ConnectionString={3} FileName={4} BackupEntryPath={5}",
                HttpUtility.UrlEncode(Global.ClientManager.GetDatabaseName(Global.Core.ClientName)),
                HttpUtility.UrlEncode(Global.Core.ClientName),
                HttpUtility.UrlEncode(Global.Core.DatabaseProvider),
                HttpUtility.UrlEncode(Global.Core.ConnectionString),
                HttpUtility.UrlEncode(fileName),
                HttpUtility.UrlEncode(string.Format("ClientBackups/ClientBackup[@Id=\"{0}\"]", idBackup))
            );

            app.Start();

            Response.Redirect(Request.Url.ToString());
        }

        #endregion
    }
}