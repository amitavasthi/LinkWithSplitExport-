using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseCore.Items;
using System.Configuration;
using System.IO;
using MasterPage.Classes;
using Crosstables.Classes.HierarchyClasses;
using WebUtilities;
using System.Xml;

namespace LinkOnline.Pages
{
    public partial class Login : WebUtilities.BasePage
    {
        #region Properties

        /// <summary>
        /// Gets the client name from the url.
        /// </summary>
        public string ClientName
        {
            get
            {
                return Request.Url.Host.Split('.')[0].Trim();
            }
        }

        #endregion


        #region Constructor

        public Login()
            : base(false, false)
        { }

        #endregion


        #region Methods

        private void RenderColorScheme()
        {
            ColorSchemeStylesheet stylesheet = new ColorSchemeStylesheet();

            this.Page.Header.Controls.Add(stylesheet);
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["Version"] == null)
            {
                HttpContext.Current.Session["Version"] = System.Reflection.Assembly.
                    GetExecutingAssembly().GetName().Version.ToString().Replace(".", "");
            }

            if (HttpContext.Current.Session["KickedOut"] != null)
            {
                // Initialize the session's language manager.
                Global.LanguageManager = new LanguageManager("", HttpContext.Current.Request.PhysicalApplicationPath);
                Global.Language = Global.LanguageManager.DefaultLanguage;

                // Initialize the session's permission core.
                Global.PermissionCore = new PermissionCore.PermissionCore(
                    Request.PhysicalApplicationPath,
                    "LinkOnline",
                    this.ClientName
                );

                // Initialize the session's client manager.
                Global.ClientManager = new ApplicationUtilities.Classes.ClientManager();

                Global.HierarchyFilters = new HierarchyFilterCollection();

                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "KickedOut",
                    "ShowMessage(\"" +
                    Global.LanguageManager.GetText("KickedOutMessage") + "\", \"Error\");",
                    true
                );

                HttpContext.Current.Session["KickedOut"] = null;
            }

            if (ConfigurationManager.AppSettings["Maintanance"] != null &&
                bool.Parse(ConfigurationManager.AppSettings["Maintanance"]) == true && Request.Params["IgnoreMaintanance"] == null)
            {
                pnlMaintananceMessage.Visible = true;
                btnLogin.Enabled = false;
                return;
            }

            if (Request.QueryString != null)
            {
                if (Request.QueryString["msg"] == "1")
                {
                    //base.ShowMessage("ResetMailSuccess", WebUtilities.MessageType.Success);
                    lblMsg.Visible = true;
                    lblMsg.Name = "ResetMailSuccess";
                    lblMsg.ForeColor = System.Drawing.Color.Green;
                }
                if (Request.QueryString["msg"] == "2")
                {
                    // base.ShowMessage("ResetSuccess", WebUtilities.MessageType.Success);
                    lblMsg.Visible = true;
                    lblMsg.Name = "ResetSuccess";
                    lblMsg.ForeColor = System.Drawing.Color.Green;
                }
                if (Request.QueryString["msg"] == "3")
                {
                    // base.ShowMessage("ResetSuccess", WebUtilities.MessageType.Success);
                    lblMsg.Visible = true;
                    lblMsg.Name = "LoginErrorMsg";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                }
            }
            if (Request.Params["Logout"] != null)
            {
                ApplicationUtilities.UsageLogger logger = new ApplicationUtilities.UsageLogger(
                        this.Core.ClientName,
                        this.Core.Users.GetSingle(IdUser.Value)
                    );
                //to call logout only once
                logger.Logout();
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();

                Response.Redirect(
                    "/Pages/Login.aspx"
                );
            }

            // Check if the session is corrupt (Google Chrome issue)
            if (Global.ClientManager == null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();

                Response.Redirect(
                    "/Pages/Login.aspx"
                );
            }

            // Check if the database core of the current session is initialized.
            if (Global.Core == null || Global.Core.ClientName != this.ClientName)
            {
                ApplicationUtilities.Classes.Client client = Global.ClientManager.GetSingle(this.ClientName);

                if (client == null)
                {
                    // TEST ONLY:
                    Response.Clear();
                    Response.Write(string.Format(
                        "Client with name '{0}' doesn't exist.",
                        this.ClientName != null ? this.ClientName : "null"
                    ));
                    Response.End();
                }
                                

                string connectionString = string.Format(
                    ConfigurationManager.AppSettings["ConnectionString"],
                    client.Database
                );

                // Create a new database core for the session.
                Global.Core = new DatabaseCore.Core(
                    ConfigurationManager.AppSettings["DatabaseProvider"],
                    connectionString,
                    ConfigurationManager.AppSettings["DatabaseProviderUserManagement"],
                    connectionString,
                    client.SynchServers
                );

                // Initialize the session's permission core.
                Global.PermissionCore = new PermissionCore.PermissionCore(
                    Request.PhysicalApplicationPath,
                    "LinkOnline",
                    this.ClientName
                );

                // Initialize the session's language manager.
                Global.LanguageManager = new LanguageManager(
                    this.ClientName,
                    Request.PhysicalApplicationPath
                );

                // Set the database core's file storage path.
                Global.Core.FileStorageRoot = string.Format(
                    ConfigurationManager.AppSettings["FileStorageRoot"],
                    this.ClientName
                );

                /*if (!Directory.Exists(Global.Core.FileStorageRoot))
                    Directory.CreateDirectory(Global.Core.FileStorageRoot);

                Global.Core.LogDirectory = ConfigurationManager.AppSettings["DatabaseChangeLogDirectory"];*/

                Global.Core.ClientName = this.ClientName;
                Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(this.ClientName).CaseDataVersion;
                Global.Core.CaseDataLocation = Global.ClientManager.GetSingle(this.ClientName).CaseDataLocation;
            }

            RenderColorScheme();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            SetPageDimensions();

            User user = null;
            if((ConfigurationManager.AppSettings["Overrides"] !=null) &&(this.ClientName.ToLower() == ConfigurationManager.AppSettings["Overrides"].ToString()))            
            {                              
                try
                {               
                XmlDocument document = new XmlDocument();
                document.Load(System.IO.Path.Combine(
                           System.Web.HttpContext.Current.Request.PhysicalApplicationPath,
                           "App_Data",
                           "Overrides",
                           this.ClientName + ".xml"
                       ));

                ApplicationUtilities.Classes.Client client = Global.ClientManager.GetSingle(Request.Url.Host.Split('.')[0].Trim());

                string Db= null;
                
                foreach (XmlNode item in document.DocumentElement.SelectNodes("User"))
                {
                    if ((txtUsername.Text.ToLower() == item.Attributes["Name"].Value.ToLower())) {
                        Db = item.Attributes["Db"].Value.ToLower();
                    }
                }

                    if (Db !=null)
                {

                  
                    string connectionString = string.Format(
                      ConfigurationManager.AppSettings["ConnectionString"],
                       Db
                   );

                    Global.Core = new DatabaseCore.Core(
                    ConfigurationManager.AppSettings["DatabaseProvider"],
                    connectionString,
                    ConfigurationManager.AppSettings["DatabaseProviderUserManagement"],
                    connectionString,
                    client.SynchServers
                );

                    // Create a new database core for the session.
                    Global.Core = new DatabaseCore.Core(
                        ConfigurationManager.AppSettings["DatabaseProvider"],
                        connectionString,
                        ConfigurationManager.AppSettings["DatabaseProviderUserManagement"],
                        connectionString,
                        client.SynchServers
                    );

                    // Initialize the session's permission core.
                    Global.PermissionCore = new PermissionCore.PermissionCore(
                        Request.PhysicalApplicationPath,
                        "LinkOnline",
                        this.ClientName
                    );

                    // Initialize the session's language manager.
                    Global.LanguageManager = new LanguageManager(
                        this.ClientName,
                        Request.PhysicalApplicationPath
                    );

                    // Set the database core's file storage path.
                    Global.Core.FileStorageRoot = string.Format(
                        ConfigurationManager.AppSettings["FileStorageRoot"],
                        this.ClientName
                    );

                    /*if (!Directory.Exists(Global.Core.FileStorageRoot))
                        Directory.CreateDirectory(Global.Core.FileStorageRoot);

                    Global.Core.LogDirectory = ConfigurationManager.AppSettings["DatabaseChangeLogDirectory"];*/

                    Global.Core.ClientName = this.ClientName;
                    Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(this.ClientName).CaseDataVersion;
                    Global.Core.CaseDataLocation = Global.ClientManager.GetSingle(this.ClientName).CaseDataLocation;

                }
                }
                catch (Exception)
                {
                   
                }
            }


            // Check if the entered login data is valid.
            user = Global.Core.Users.Valid(
                txtUsername.Text,
                txtPassword.Text
            );
            if (user != null)
            {
                Global.User = user;

                if (!Global.AllSessions.ContainsKey(this.ClientName))
                {
                    Global.AllSessions.Add(this.ClientName, new Dictionary<Guid, System.Web.SessionState.HttpSessionState>());
                }

                if (Global.AllSessions[this.ClientName].ContainsKey(user.Id))
                {
                    if (Global.AllSessions[this.ClientName][user.Id] != null)
                        Global.AllSessions[this.ClientName][user.Id].RemoveAll();

                    try
                    {
                        Global.AllSessions[this.ClientName][user.Id]["KickedOut"] = true;
                        Global.AllSessions[this.ClientName].Remove(user.Id);
                    }
                    catch { }
                }

                if (!Global.IdUser.HasValue)
                {
                    Response.Redirect(Request.Url.ToString());
                    return;
                }

                if (Global.AllSessions[this.ClientName] != null)
                {
                    Global.AllSessions[this.ClientName].Add(user.Id, HttpContext.Current.Session);

                    /*The below GridLines is used for PasswordAssistance Module*/

                    Session["UserDetails"] = Global.User.Name + "," + Global.User.FirstName + "," + Global.User.LastName + "," + Global.User.Mail;

                    ApplicationUtilities.UsageLogger logger = new ApplicationUtilities.UsageLogger(
                        this.Core.ClientName,
                        this.Core.Users.GetSingle(IdUser.Value)
                    );

                    logger.Log(
                        ApplicationUtilities.UsageLogVariable.Login,
                        DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                    );
                    logger.Log(
                        ApplicationUtilities.UsageLogVariable.Browser,
                        (Request.UserAgent.IndexOf("Edge") > -1) ? "Microsoft Edge" : Request.Browser.Browser
                    );
                    logger.Log(
                      ApplicationUtilities.UsageLogVariable.EmailId,
                      Global.User.Mail
                  );

                    //to call initlog only once
                    logger.InitLog(
                    (Request.UserAgent.IndexOf("Edge") > -1) ? "Microsoft Edge" : Request.Browser.Browser
                  );

                    Global.UserDefaults = new UserDefaults(Path.Combine(
                        HttpContext.Current.Request.PhysicalApplicationPath,
                        "Fileadmin",
                        "UserDefaults",
                        this.ClientName,
                        Global.IdUser.Value + ".xml"
                    ));

                    if (Request.Params["RedirectUrl"] == null)
                        Response.Redirect("/Pages/Default.aspx");
                    else
                        Response.Redirect(HttpUtility.UrlDecode(Request.Params["RedirectUrl"]));
                }
            }
            else
            {
                Response.Redirect("/Pages/Login.aspx?msg=3");
            }
        }

        private void SetPageDimensions()
        {
            int width;

            if (int.TryParse(Request.Params["hdfContentWidth"], out width))
            {
                HttpContext.Current.Session["ContentWidth"] = width;
            }
            int height;

            if (int.TryParse(Request.Params["hdfContentHeight"], out height))
            {
                HttpContext.Current.Session["ContentHeight"] = height;
            }
        }

        #endregion

        protected void btnForgot_Click(object sender, EventArgs e)
        {
            Response.Redirect("ForgotPassword.aspx", false);
        }
    }
}