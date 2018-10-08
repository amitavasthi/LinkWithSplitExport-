using ApplicationUtilities;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Handlers.Support
{
    /// <summary>
    /// Summary description for Support.
    /// </summary>
    public class Support : IHttpHandler, IRequiresSessionState
    {
        #region Properties

        string path;
        string browserDetails;
        string[] module;
        string url;
        string userAgent;
        string attachment;
        string imageData;
        public Classes.SupportClass supportClass = new Classes.SupportClass();
        #endregion

        #region Methods

        public void ProcessRequest(HttpContext context)
        {

            try
            {
                imageData = context.Request.Params["imageData"];

                if (imageData != "NO Image")
                {
                    UploadPic(imageData);
                }              
              
                    string iSupportId;
                    string description = context.Request.Params["description"];
                    url = context.Request.Params["pagedata"];
                    attachment = context.Request.Params["attachimage"];
                    userAgent = context.Request.Params["useragent"];
                    browserDetails = context.Request.Params["browserinfo"]; 
                    module = url.Split('/');
                    if (module[4] == null)
                    {
                        module[4] = module[3];
                    }
                    string[] parts = userAgent.Split(';');
                    string[] firstpart = parts[0].Split('(');
                    userAgent = firstpart[1];

                    iSupportId = supportClass.ReturnSupportId();

                    if (attachment == "YES")
                    {
                        Rename(iSupportId.ToString());
                    }

                    if (userAgent == "Windows NT 5.2") { userAgent = "Windows Server 2003/Windows XP x64 Edition"; }
                    else if (userAgent == "Windows NT 6.0") { userAgent = "Windows Vista"; }
                    else if (userAgent == "Windows NT 6.1") { userAgent = "Windows 7"; }
                    else if (userAgent == "Windows NT 6.1") { userAgent = "Windows RT"; }
                    else if (userAgent == "Windows NT 6.2") { userAgent = "Windows 8"; }
                    else if (userAgent == "Windows NT 6.3") { userAgent = "Windows 8.1"; }
                    else if (userAgent == "Windows NT 10.0") { userAgent = "Windows 10"; }

                    path = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "FileAdmin", "SupportSnapShots", "img_" + iSupportId + ".png");
                    // Saving in database.              
                    supportClass.InsertSupportDetails(module[4], attachment, path, userAgent, browserDetails, description, iSupportId);
                  
                    User user;
                    if (Global.User != null)
                    {
                        user = Global.User;
                        MailConfiguration mailConfiguration = new MailConfiguration(true);

                        Mail mail = new Mail(mailConfiguration, Global.Core.ClientName)
                        {
                            TemplatePath = Path.Combine(
                            context.Request.PhysicalApplicationPath,
                           "App_Data",
                           "MailTemplates",
                           Global.Language.ToString(),
                           "Feedback.html"
                           ),
                            Subject = "Bug Information From Support Module"
                        };
                        if (attachment == "YES")
                        {
                            mail.Attachments.Add("support.png", File.ReadAllBytes(path));
                        }
                        mail.Placeholders.Add("supportid", iSupportId.ToString());
                        mail.Placeholders.Add("fname", user.FirstName);
                        mail.Placeholders.Add("LastName", user.LastName);
                        mail.Placeholders.Add("Username", user.Name);
                        mail.Placeholders.Add("Email", user.Mail);
                        mail.Placeholders.Add("module", module[4]);
                        mail.Placeholders.Add("description", description);
                        mail.Placeholders.Add("browserinfo", browserDetails);

                        // Send the mail.
                        mail.Send(ConfigurationManager.AppSettings["SupportMail"].ToString());

                        supportClass = null;
                    }               
            }
            catch (Exception e)
            {
                supportClass = null;
                throw e;
                
            }
        }

        /// <summary>
        /// Upload the snapshot to the server 
        /// </summary>
        /// <param name="imageData">imageData as string</param>
        public static void UploadPic(string imageData)
        {
            string picPath = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "FileAdmin", "SupportSnapShots", "MyPicture.png");

            using (FileStream fs = new FileStream(picPath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(imageData);
                    bw.Write(data);
                    bw.Close();
                }
            }
        }

        /// <summary>
        /// To rename the snapshot for future refernces
        /// </summary>
        /// <param name="supportId">supportId</param>
        public bool Rename(string supportId)
        {
            string imgName;
            string imgPath = ConfigurationManager.AppSettings["SupportSnapShotPath"];
            imgName = "img_" + supportId + ".png";
            bool ret = false;
            System.IO.FileInfo fi = new System.IO.FileInfo(imgPath);
            if (!fi.Exists)
                return ret;

            string newFilePathName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(imgPath), imgName);
            System.IO.FileInfo f2 = new System.IO.FileInfo(newFilePathName);
            try
            {
                if (f2.Exists)
                {
                    f2.Attributes = System.IO.FileAttributes.Normal;
                    f2.Delete();
                }

                fi.CopyTo(newFilePathName);
                fi.Delete();
                ret = true;
            }
            catch
            {

            }

            return ret;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion
    }

}
