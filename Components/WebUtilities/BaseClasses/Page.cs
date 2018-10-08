using DatabaseCore.Items;
using MasterPage.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using WebUtilities.Classes;

namespace WebUtilities
{
    public class BasePage : System.Web.UI.Page
    {
        #region Properties

        public int ContentWidth
        {
            get
            {
                if (HttpContext.Current.Session["ContentWidth"] == null)
                    HttpContext.Current.Session["ContentWidth"] = 0;

                return (int)HttpContext.Current.Session["ContentWidth"];
            }
            set
            {
                HttpContext.Current.Session["ContentWidth"] = value;
            }
        }

        public int ContentHeight
        {
            get
            {
                if (HttpContext.Current.Session["ContentHeight"] == null)
                    HttpContext.Current.Session["ContentHeight"] = 0;

                return (int)HttpContext.Current.Session["ContentHeight"];
            }
            set
            {
                HttpContext.Current.Session["ContentHeight"] = value;
            }
        }

        /// <summary>
        /// Gets the database core of the web application's session.
        /// </summary>
        public DatabaseCore.Core Core
        {
            get
            {
                return (DatabaseCore.Core)HttpContext.Current.Session["Core"];
            }
        }

        /// <summary>
        /// Gets or sets if a authenticated user
        /// is required to view the page.
        /// </summary>
        public bool CheckUser { get; set; }

        /// <summary>
        /// Gets or sets if the global
        /// master page should be used.
        /// </summary>
        public bool UseMasterPage { get; set; }

        /// <summary>
        /// Gets the id of the authenticated of the current session.
        /// </summary>
        public Guid? IdUser
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null)
                    return null;

                return (Guid)HttpContext.Current.Session["User"];
            }
        }

        /// <summary>
        /// Gets the permission core of the web application's session.
        /// </summary>
        public PermissionCore.PermissionCore PermissionCore
        {
            get
            {
                return (PermissionCore.PermissionCore)HttpContext.Current.Session["PermissionCore"];
            }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the base page.
        /// </summary>
        /// <param name="checkUser">
        /// Defines if a authenticated user
        /// is required to view the page.
        /// </param>
        public BasePage(bool checkUser = true, bool useMasterPage = true)
        {
            this.CheckUser = checkUser;
            this.UseMasterPage = useMasterPage;

            this.Load += BasePage_Load;
            this.PreInit += BasePage_PreInit;
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

        public string GetMimeType(string extension)
        {
            switch (extension)
            {
                case "js":
                    return "application/x-javascript";
                case "json":
                    return "application/json";
                case "jpg":
                case "jpeg":
                case "jpe":
                    return "image/jpg";
                case "png":
                case "gif":
                case "bmp":
                case "tiff":
                    return "image/tiff";
                case "css":
                    return "text/css";
                case "xml":
                    return "application/xml";
                case "doc":
                case "docx":
                    return "application/msword";
                case "xls":
                case "xlt":
                case "xlm":
                case "xld":
                case "xla":
                case "xlc":
                case "xlw":
                case "xll":
                    return "application/vnd.ms-excel";
                case "ppt":
                case "pps":
                    return "application/vnd.ms-powerpoint";
                case "rtf":
                    return "application/rtf";
                case "pdf":
                    return "application/pdf";
                case "html":
                case "htm":
                case "php":
                    return "text/html";
                case "txt":
                    return "text/plain";
                case "mpeg":
                case "mpg":
                case "mpe":
                    return "video/mpeg";
                case "mp3":
                    return "audio/mpeg3";
                case "wav":
                    return "audio/wav";
                case "aiff":
                case "aif":
                    return "audio/aiff";
                case "avi":
                    return "video/msvideo";
                case "wmv":
                    return "video/x-ms-wmv";
                case "mov":
                    return "video/quicktime";
                case "zip":
                    return "application/zip";
                case "tar":
                    return "application/x-tar";
                case "swf":
                    return "application/x-shockwave-flash";
            }

            return "text/plain";
        }

        /// <summary>
        /// Binds values of an enum type to a drop down list.
        /// </summary>
        /// <param name="control">The drop down list to bind the values as list items to.</param>
        /// <param name="enumType">The type of the enum containing the values.</param>
        public void BindEnum(DropDownList control, Type enumType)
        {
            foreach (Enum value in
                Enum.GetValues(enumType))
            {
                ListItem lItem = new ListItem();

                lItem.Text = value.ToString();
                lItem.Value = value.ToString();

                control.Items.Add(lItem);
            }
        }

        public void BindValues(DropDownList control, params string[] values)
        {
            foreach (string value in values)
            {
                control.Items.Add(value);
            }
        }


        /// <summary>
        /// Displays a message on the screen.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void ShowMessage(string message, MessageType type)
        {
            // For test only.
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "Message" + Guid.NewGuid(),
                "loadFunctions.push(function() { ShowMessage(\"" + message + "\", \"" + type.ToString() + "\"); });",
                true
            );
        }

        public bool HasPagePermission(string url)
        {
            if (this.PermissionCore != null && this.PermissionCore.PagePermissions != null)
            {
                if (this.PermissionCore.PagePermissions[url] != null)
                {
                    if ((int)this.Core.RolePermissions.ExecuteReader(string.Format(
                        "SELECT Count(*) FROM RolePermissions WHERE Permission='{0}' AND IdRole IN (SELECT IdRole FROM UserRoles WHERE IdUser='{1}')",
                        this.PermissionCore.PagePermissions[url].Permission.Id,
                        this.IdUser.Value
                    ), typeof(int))[0][0] == 0)
                    {
                        return false;
                    }

                    /*if (!this.User.HasPermission(this.PermissionCore.PagePermissions[url].Permission.Id))
                    {
                        return false;
                    }*/
                }
            }

            return true;
        }

        public string ToJson(string[] names, object[] values)
        {
            StringBuilder jsonBuilder = new StringBuilder();

            jsonBuilder.Append("{");

            for (int i = 0; i < names.Length; i++)
            {
                string value = "";

                if (values[i] == null)
                    value = "";
                else
                    value = values[i].ToString();

                if (value.StartsWith("[") || value.StartsWith("{"))
                {
                    value = values[i].ToString();
                }
                else
                {
                    value = "\"" + HttpUtility.HtmlEncode(values[i]) + "\"";
                }

                jsonBuilder.Append(string.Format(
                    "\"{0}\": {1},",
                    names[i],
                    value
                ));
            }

            string result = jsonBuilder.ToString();

            if (result.Length > 0)
                result = result.Remove(result.Length - 1, 1);

            result += "}";

            return result;
        }

        #endregion


        #region Event Handlers

        protected void BasePage_PreInit(object sender, EventArgs e)
        {
            // Check if the global master page should be used.
            if (this.UseMasterPage)
            {
                // Set the master page file of the page to the virtual path.
                base.MasterPageFile = MasterPageVirtualPathProvider.MasterPageFileLocation;
            }
        }

        /// <summary>
        /// Handles the load event of the base page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BasePage_Load(object sender, EventArgs e)
        {
            User user = null;

            if (this.IdUser.HasValue)
                user = this.Core.Users.GetSingle(IdUser.Value);

            if (Request.UrlReferrer == null || Request.UrlReferrer.ToString() != Request.Url.ToString())
            {
                if (this.IdUser.HasValue)
                {
                    ApplicationUtilities.UsageLogger logger = new ApplicationUtilities.UsageLogger(
                        this.Core.ClientName,
                        user
                    );

                    logger.Log(ApplicationUtilities.UsageLogVariable.AccessedPage, Request.Url.PathAndQuery.Split('?')[0]);
                    //to call usagelogdetails only once
                    logger.UsageLogDetails(
                        Request.Url.PathAndQuery.Split('?')[0]
                  );
                }
            }

            if (this.IdUser.HasValue && user.Validated == false)
            {
                UserValidation userValidation = new UserValidation(Path.Combine(
                    Request.PhysicalApplicationPath,
                    "App_Data",
                    "UserValidation",
                    this.Core.ClientName + ".xml"
                ));

                if (userValidation.Exists)
                {
                    if (Request.Url.ToString().Contains("Pages/Default.aspx") == false)
                    {
                        Response.Redirect("/Pages/Default.aspx");
                        return;
                    }

                    Page.Controls[0].Controls.Add(userValidation.Render());
                }
            }

            // Check if the page requires an authenticated user.
            if (this.CheckUser)
            {
                // Check if the current session has an authenticated user.
                if (this.IdUser == null)
                {
                    // Redirect to the login page.
                    Response.Redirect("/Pages/Login.aspx?RedirectUrl=" + HttpUtility.UrlEncode(Request.Url.ToString()));

                    return;
                }
            }

            if (!HasPagePermission(Request.Url.LocalPath))
                Response.Redirect("/Default.aspx");

            if (Request.Params["ContentWidth"] != null)
            {
                int contentWidth;

                if (int.TryParse(Request.Params["ContentWidth"].ToString(), out contentWidth))
                    this.ContentWidth = contentWidth;
            }

            if (this.IdUser != null)
            {
                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "SetCurrentUsername",
                    "var currentUserName='" + this.Core.Users.GetValue("Name", "Id", this.IdUser.Value) + "';",
                    true
                );
            }
        }

        #endregion
    }

    public enum MessageType
    {
        Error,
        Success,
        Warning
    }
}
