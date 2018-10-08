using DashboardCore;
using DashboardCore.Exporter;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using WebUtilities;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for Dashboards
    /// </summary>
    public class Dashboards : IHttpHandler, IRequiresSessionState
    {
        #region Properties

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /*public Dictionary<string, Dashboard> Cache
        {
            get
            {
                if (HttpContext.Current.Session["Dashboards"] == null)
                    this.Cache = new Dictionary<string, Dashboard>();

                return (Dictionary<string, Dashboard>)HttpContext.Current.Session["Dashboards"];
            }
            set
            {
                HttpContext.Current.Session["Dashboards"] = value;
            }
        }*/

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private bool Authenticate(HttpContext context)
        {
            if (context.Request.Params["Username"] == null ||
                context.Request.Params["Password"] == null)
                return false;

            string clientName = context.Request.Url.Host.Split('.')[0];

            string connectionString = string.Format(
                ConfigurationManager.AppSettings["ConnectionString"],
                Global.ClientManager.GetDatabaseName(clientName)
            );

            // Create a new database core for the session.
            Global.Core = new DatabaseCore.Core(
                ConfigurationManager.AppSettings["DatabaseProvider"],
                connectionString,
                ConfigurationManager.AppSettings["DatabaseProviderUserManagement"],
                connectionString,
                new string[0]
            );

            // Initialize the session's language manager.
            Global.LanguageManager = new WebUtilities.LanguageManager(
                clientName,
                context.Request.PhysicalApplicationPath
            );

            // Set the database core's file storage path.
            Global.Core.FileStorageRoot = string.Format(
                ConfigurationManager.AppSettings["FileStorageRoot"],
                clientName
            );

            /*if (!Directory.Exists(Global.Core.FileStorageRoot))
                Directory.CreateDirectory(Global.Core.FileStorageRoot);

            Global.Core.LogDirectory = ConfigurationManager.AppSettings["DatabaseChangeLogDirectory"];*/

            Global.Core.ClientName = clientName;
            Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(clientName).CaseDataVersion;
            Global.Core.CaseDataLocation = Global.ClientManager.GetSingle(clientName).CaseDataLocation;

            // Check if the entered login data is valid.
            User user = Global.Core.Users.Valid(
                context.Request.Params["Username"],
                context.Request.Params["Password"],
                false
            );

            if (user == null)
                return false;

            Global.IdUser = user.Id;

            Global.UserDefaults = new UserDefaults(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "UserDefaults",
                Global.Core.ClientName,
                Global.IdUser.Value + ".xml"
            ));

            Global.PermissionCore = new PermissionCore.PermissionCore(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "LinkOnline",
                Global.Core.ClientName
            );

            return true;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (!Global.IdUser.HasValue)
            {
                //throw new Exception("Not authenticated.");
                if (!Authenticate(context))
                {
                    context.Response.Write(File.ReadAllText(Path.Combine(
                        context.Request.PhysicalApplicationPath,
                        "App_Data",
                        "Templates",
                        "DashboardLogin.html"
                    )));

                    return;
                }
            }

            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Dashboards",
                Global.Core.ClientName,
                context.Request.Params["Dashboard"],
                "Definition.xml"
            );

            if (!File.Exists(fileName))
            {
                throw new Exception(string.Format(
                    "Dashboard with the name '{0}' does not exist.",
                    context.Request.Params["Dashboard"]
                ));
            }

            Dashboard dashboard;
            dashboard = new Dashboard(fileName, Global.Core);

            dashboard.Parse();

            if (context.Request.Params["Export"] == "True")
            {
                dashboard.Render(true);
                DashboardExporter exporter = null;
                DashboardExportFormat exportFormat = DashboardExportFormat.Excel;

                if (context.Request.Params["Format"] != null)
                {
                    exportFormat = (DashboardExportFormat)Enum.Parse(
                        typeof(DashboardExportFormat),
                        context.Request.Params["Format"]
                    );
                }
                else
                {
                    exportFormat = dashboard.Settings.ExportFormat;
                }

                switch (dashboard.Settings.ExportFormat)
                {
                    case DashboardExportFormat.Excel:
                        exporter = new DashboardExporterExcel(dashboard);
                        break;
                    case DashboardExportFormat.Pdf:
                        exporter = new DashboardExporterPdf(dashboard);
                        break;
                    default:
                        throw new Exception("Undefined export interface.");
                        break;
                }

                string test = exporter.Export(HttpUtility.UrlDecode(context.Request.Params["Html"]), Path.Combine(
                   HttpContext.Current.Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "Temp",
                   Guid.NewGuid() + exporter.Extension
               ));

                if (dashboard.Properties[DashboardCore.Classes.DashboardPropertyType.ExportTitle] == "")
                    dashboard.Properties[DashboardCore.Classes.DashboardPropertyType.ExportTitle] =
                        dashboard.Properties[DashboardCore.Classes.DashboardPropertyType.Title];

                WriteFileToResponse(
                    test,
                    dashboard.Properties[DashboardCore.Classes.DashboardPropertyType.ExportTitle].Replace(",", "") + exporter.Extension,//dashboard.Properties[DashboardCore.Classes.DashboardPropertyType.ExportTitle] + exporter.Extension,
                    exporter.MimeType,
                    true
                );
                return;
            }

            if (context.Request.Params["RenderMode"] == "DataUpdate")
            {
                context.Response.Write(dashboard.RenderDataUpdate());
                context.Response.ContentType = "text/html";
                return;
            }

            bool bodyOnly = false;

            if (context.Request.Params["BodyOnly"] != null)
                bodyOnly = bool.Parse(context.Request.Params["BodyOnly"]);

            context.Response.Write(dashboard.Render(bodyOnly));
            context.Response.ContentType = "text/html";
        }

        /// <summary>
        /// Writes a file to the response of a request.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file that is to be written.
        /// </param>
        /// <param name="displayName">
        /// The name that is to be displayed for the file.
        /// </param>
        public void WriteFileToResponse(
            string fileName,
            string displayName,
            string mimeType,
            bool deleteFile
        )
        {
            // Local variables.
            HttpContext context = null;


            // Replace the spaces in the 
            // file name with underscores.
            //displayName = displayName.Replace(" ", "_");


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

        #endregion
    }
}