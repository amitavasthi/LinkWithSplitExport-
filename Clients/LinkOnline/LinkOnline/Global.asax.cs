using ApplicationUtilities;
using ApplicationUtilities.Classes;
using Crosstables.Classes.HierarchyClasses;
using DatabaseCore.Items;
using LinkOnline.Classes;
using LinkOnline.Pages.AgencyManagementSystem;
using MasterPage.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.SessionState;
using WebUtilities;

namespace LinkOnline
{
    public class Global : System.Web.HttpApplication
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database core of the web application's session.
        /// </summary>
        public static DatabaseCore.Core Core
        {
            get
            {
                return (DatabaseCore.Core)HttpContext.Current.Session["Core"];
            }
            set
            {
                HttpContext.Current.Session["Core"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the permission core of the web application's session.
        /// </summary>
        public static PermissionCore.PermissionCore PermissionCore
        {
            get
            {
                return (PermissionCore.PermissionCore)HttpContext.Current.Session["PermissionCore"];
            }
            set
            {
                HttpContext.Current.Session["PermissionCore"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the language manager of the current session.
        /// </summary>
        public static LanguageManager LanguageManager
        {
            get
            {
                return (LanguageManager)HttpContext.Current.Session["LanguageManager"];
            }
            set
            {
                HttpContext.Current.Session["LanguageManager"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the language of the current session.
        /// </summary>
        public static Language Language
        {
            get
            {
                return (Language)HttpContext.Current.Session["Language"];
            }
            set
            {
                HttpContext.Current.Session["Language"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the client manager of the current session.
        /// </summary>
        public static ClientManager ClientManager
        {
            get
            {
                return (ClientManager)HttpContext.Current.Session["ClientManager"];
            }
            set
            {
                HttpContext.Current.Session["ClientManager"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the authenticated user of the current session.
        /// </summary>
        public static Guid? IdUser
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null)
                    return null;

                return (Guid)HttpContext.Current.Session["User"];
            }
            set
            {
                HttpContext.Current.Session["User"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the authenticated user of the current session.
        /// </summary>
        public static User User
        {
            get
            {
                if (IdUser == null)
                    return null;

                return Core.Users.GetSingle(IdUser.Value);
            }
            set
            {
                IdUser = value.Id;
            }
        }

        /// <summary>
        /// Gets or sets the running imports of the application.
        /// </summary>
        public static Dictionary<Guid, DataUpload> RunningImports { get; set; }

        /// <summary>
        /// Gets or sets the user defaults for
        /// the current session's logged on user.
        /// </summary>
        public static UserDefaults UserDefaults
        {
            get
            {
                return (UserDefaults)HttpContext.Current.Session["UserDefaults"];
            }
            set
            {
                HttpContext.Current.Session["UserDefaults"] = value;
            }
        }

        private static Dictionary<string, Dictionary<Guid, HttpSessionState>> allSessions;

        public static Dictionary<string, Dictionary<Guid, HttpSessionState>> AllSessions
        {
            get { return allSessions; }
            set { allSessions = value; }
        }
        

        /// <summary>
        /// Gets or sets the hierarchy filter collection for the current session.
        /// </summary>
        public static HierarchyFilterCollection HierarchyFilters
        {
            get
            {
                return (HierarchyFilterCollection)HttpContext.Current.Session["HierarchyFilters"];
            }
            set
            {
                HttpContext.Current.Session["HierarchyFilters"] = value;
            }
        }

        public static void ClearCaches()
        {
            ClearCaches(Global.Core);
        }
        public static void ClearCaches(DatabaseCore.Core core)
        {
            DataCore.Classes.StorageMethods.Database database = new DataCore.Classes.StorageMethods.Database(
                core,
                null
            );

            database.ClearCaseDataCache();

            core.ClearCache();
        }

        private static SessionSynch sessionSynch;

        public static SessionSynch SessionSynch
        {
            get { return sessionSynch; }
            set { sessionSynch = value; }
        }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        public static CultureInfo[] GetCultures()
        {
            List<CultureInfo> result = new List<CultureInfo>();

            CultureInfo[] cultureInfos = CultureInfo.GetCultures(
                CultureTypes.SpecificCultures |
                CultureTypes.NeutralCultures
            );

            foreach (CultureInfo cultureInfo in cultureInfos.OrderBy(x => x.DisplayName))
            {
                if (cultureInfo.Name.EndsWith("BTX") || cultureInfo.Name.EndsWith("DTX"))
                {
                    continue;
                }

                try
                {
                    new CultureInfo(cultureInfo.LCID);
                }
                catch
                {
                    continue;
                }

                try
                {
                    CultureInfo ciTest = new CultureInfo(cultureInfo.LCID);

                    result.Add(cultureInfo);
                }
                catch
                {

                }
            }

            return result.ToArray();
        }

        public static string GetNiceUsername(Guid idUser)
        {
            // Get the user's first name.
            string result = (string)Core.Users.GetValue(
                "FirstName",
                "Id",
                idUser
            );

            if (result == null)
                return "";

            return GetNiceUsername(idUser, result);
        }

        public static string GetNiceUsername(Guid idUser, string name)
        {
            string result = name;

            // Get the count of users with the first name in the company.
            int count = Core.Users.Count(
                new string[] { "FirstName" },
                new object[] { result }
            );

            // Check if there are more than one user with the first name.
            if (count > 1)
            {
                // Add the user's last name to the string.
                result += " " + (string)Core.Users.GetValue(
                    "LastName",
                    "Id",
                    idUser
                );
            }

            return result.ToLower();
        }


        private static void ClearSessionTemp()
        {
            if (HttpContext.Current == null)
                return;

            string[] tempSessionDirectories = new string[] {
                "VariableSelector",
                "ChartCache",
                "ReportDefinitionHistory",
                "OpenSavedReports"
            };

            foreach (string tempSessionDirectory in tempSessionDirectories)
            {
                string tempDirectoryName = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "Temp",
                    tempSessionDirectory,
                    HttpContext.Current.Session.SessionID
                );

                try
                {
                    if (Directory.Exists(tempDirectoryName))
                        Directory.Delete(tempDirectoryName, true);
                }
                catch { }
            }
        }

        #endregion


        #region Event Handlers

        protected void Application_Start(object sender, EventArgs e)
        {
            MasterPageVirtualPathProvider vpp = new MasterPageVirtualPathProvider();
            HostingEnvironment.RegisterVirtualPathProvider(vpp);

            RunningImports = new Dictionary<Guid, DataUpload>();
            AllSessions = new Dictionary<string, Dictionary<Guid, HttpSessionState>>();
            SessionSynch = new SessionSynch();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Initialize the session's language manager.
            LanguageManager = new LanguageManager("", HttpContext.Current.Request.PhysicalApplicationPath);
            Language = LanguageManager.DefaultLanguage;

            // Initialize the session's permission core.
            //PermissionCore = new PermissionCore.PermissionCore("LinkOnline");

            // Initialize the session's client manager.
            ClientManager = new ClientManager();

            try
            {
                HttpContext.Current.Session["Version"] = System.Reflection.Assembly.
                    GetExecutingAssembly().GetName().Version.ToString().Replace(".", "");
            }
            catch { }

            ClearSessionTemp();

            HierarchyFilters = new HierarchyFilterCollection();

            try
            {
                ServiceLink service = new ServiceLink(string.Format(
                    "http://{0}/Handlers/Public.ashx",
                    ConfigurationManager.AppSettings["LinkAdminHostname"]
                ));

                string server = service.Request(new string[] {
                    "Method=WhoAmI"
                });

                HttpContext.Current.Session["Server"] = server;
            }
            catch (Exception ex)
            {
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            bool found = false;
            foreach (string client in AllSessions.Keys)
            {
                foreach (KeyValuePair<Guid, HttpSessionState> s in AllSessions[client])
                {
                    if (s.Value.SessionID == Session.SessionID)
                    {
                        AllSessions[client].Remove(s.Key);

                        found = true;

                        break;
                    }
                }

                if (found)
                    break;
            }

            ClearSessionTemp();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        #endregion
    }
}