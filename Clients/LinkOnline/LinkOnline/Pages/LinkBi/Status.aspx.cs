using ApplicationUtilities;
using LinkBi1.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.LinkBi
{
    public partial class Status : WebUtilities.BasePage
    {
        #region Properties

        #endregion


        #region Constructor

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Request.Params["Method"] == "Download")
            {
                if (HttpContext.Current.Session["CurrentLinkBiCreation"] != null)
                {
                    LinkBiInterface linkBiInterface = (LinkBiInterface)HttpContext.Current.Session["CurrentLinkBiCreation"];

                    base.WriteFileToResponse(
                        linkBiInterface.FileName,
                        "LinkBI" + (new FileInfo(linkBiInterface.FileName)).Extension,
                        linkBiInterface.MimeType,
                        true
                    );
                }

                return;
            }

            pnlProgress.Visible = false;
            pnlDownload.Visible = false;

            if (HttpContext.Current.Session["CurrentLinkBiCreation"] != null)
            {
                LinkBiInterface linkBiInterface = (LinkBiInterface)HttpContext.Current.Session["CurrentLinkBiCreation"];

                if (linkBiInterface.Progress < 100)
                {
                    pnlProgress.Visible = true;
                }
                else
                {
                    if (Request.Params["MailMe"] == "true")
                        SendMail(linkBiInterface);

                    pnlDownload.Visible = true;
                }
            }
        }

        private void SendMail(LinkBiInterface linkBiInterface)
        {
            // Create a new mail configuration and load the
            // configuration values from the web.config file.
            MailConfiguration mailConfiguration = new MailConfiguration(true);

            // Create a new mail by the mail configuration.
            Mail mail = new Mail(mailConfiguration, Global.Core.ClientName);

            // Set the full path to the mail's template file.
            mail.TemplatePath = Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "MailTemplates",
                Global.Language.ToString(),
                "LinkBiExportFinished.html"
            );

            // Add the placeholder value for the user's first name.
            mail.Placeholders.Add("FirstName", Global.User.FirstName);

            // Add the placeholder value for the user's last name.
            mail.Placeholders.Add("LastName", Global.User.LastName);

            mail.Placeholders.Add("imagepath", "http://" + Request.Url.Host.ToString() + "/Images/Logos/link.png");

            mail.Attachments.Add(
                "LinkBiExport.xls",
                File.ReadAllBytes(linkBiInterface.FileName)
            );

            try
            {
                // Send the mail.
                mail.Send(Global.User.Mail);
            }
            catch(Exception ex)
            {
                base.ShowMessage(
                    string.Format(Global.LanguageManager.GetText("MailSendError"), Global.User.Mail, ex.Message),
                    WebUtilities.MessageType.Error
                );
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            
        }

        #endregion
    }
}