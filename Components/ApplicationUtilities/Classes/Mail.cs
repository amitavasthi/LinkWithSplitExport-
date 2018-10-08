using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace ApplicationUtilities
{
    public class Mail
    {
        #region Properties

        /// <summary>
        /// Gets or sets dictionary containing
        /// the mail's attachments.
        /// </summary>
        public Dictionary<string, byte[]> Attachments { get; set; }

        /// <summary>
        /// Gets or sets dictionary containing the
        /// mail's placeholders with values.
        /// </summary>
        public Dictionary<string, string> Placeholders { get; set; }

        /// <summary>
        /// Gets or sets the mail's subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the path to the mail template.
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// Gets or sets the configuration for the mail.
        /// </summary>
        public MailConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        public string ClientName { get; set; }

        public string ReplyTo { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the mail class.
        /// </summary>
        public Mail(string clientName)
        {
            this.ClientName = clientName;
            this.Attachments = new Dictionary<string, byte[]>();
            this.Placeholders = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates a new instance of the mail class.
        /// </summary>
        /// <param name="configuration">The configuration for the mail.</param>
        public Mail(MailConfiguration configuration, string clientName)
            : this(clientName)
        {
            this.Configuration = configuration;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Sends the mail to a given mail address.
        /// </summary>
        /// <param name="address">
        /// The recipients mail address as string.
        /// </param>
        public void Send(string address)
        {
            // The mail message.
            MailMessage mail = new MailMessage();

            // Set the from address.
            mail.From = new MailAddress(this.Configuration.MailFrom);

            if (!string.IsNullOrEmpty(this.ReplyTo))
                mail.ReplyToList.Add(new MailAddress(this.ReplyTo));

            // Set the recipient.
            mail.To.Add(address);

            // Check if a blind copy address is configured.
            if (this.Configuration.MailBCC != null &&
                this.Configuration.MailBCC != "")
            {
                // Add the blind copy address.
                mail.Bcc.Add(this.Configuration.MailBCC);
            }

            // Set the mail's subject.
            mail.Subject = this.Subject;

            // Set IsBodyHtml to true.
            mail.IsBodyHtml = true;

            // Build the full path to the client
            // specific e-mail template file.
            string fileName = Path.Combine(
                Path.GetDirectoryName(this.TemplatePath),
                this.ClientName,
                new FileInfo(this.TemplatePath).Name
            );

            // Check if the client has a
            // specific e-mail template file.
            if (!File.Exists(fileName))
                fileName = this.TemplatePath;

            // Read the template file contents.
            string content = File.ReadAllText(fileName);

            // Run through all placeholders.
            foreach (KeyValuePair<string, string> placeholder in this.Placeholders)
            {
                // Replace the placeholder with the placeholders value.
                content = content.Replace(string.Format(
                    "###{0}###",
                    placeholder.Key
                ), placeholder.Value);
            }

            // Set the mail message's body.
            mail.Body = content;

            // Get a temp dir.
            string tempDir = Path.GetTempPath();

            // Run through all attachments.
            foreach (KeyValuePair<string, byte[]> attachment in this.Attachments)
            {
                // Load the attachments buffer into a memory stream.
                MemoryStream memoryStream = new MemoryStream(attachment.Value);

                // Create a new mail attachment.
                Attachment a = new Attachment(memoryStream, attachment.Key,
                    "application/" + new FileInfo(attachment.Key).Extension.Remove(0, 1));

                // Add the attachment to the mail message.
                mail.Attachments.Add(a);
            }

            // Create a new smtp client.
            SmtpClient smtpClient = new SmtpClient();

            // Set the smtp host.
            smtpClient.Host = this.Configuration.MailSmtpHost;

            // Set enable ssl to configured value.
            smtpClient.EnableSsl = this.Configuration.MailSmtpSSL;

            // Check if smtp username is set.
            if (this.Configuration.MailSmtpUsername != "")
            {
                // Create the network credentials.
                NetworkCredential credentials = new NetworkCredential();
                credentials.UserName = this.Configuration.MailSmtpUsername;
                credentials.Password = this.Configuration.MailSmtpPassword;

                // Credential cache containing the network credentials.
                CredentialCache cache = new CredentialCache();

                // Add the network credentials to the cache.
                cache.Add(
                    smtpClient.Host, // Host.
                    smtpClient.Port, // Port.
                    "Basic",         // Authentication type.
                    credentials      // Login credentials.
                );

                // Set the smtp client's credentials.
                if (this.Configuration.MailUseCredentialCache)
                    smtpClient.Credentials = cache;
                else
                    smtpClient.Credentials = credentials;
            }

            // Send the mail message.

            bool succeeded = false;
            int index = 0;
            Exception ex = null;

            while (succeeded == false)
            {
                try
                {
                    smtpClient.Send(mail);

                    succeeded = true;
                }
                catch (Exception e)
                {
                    ex = e;

                    if (this.Configuration.MailSmtpHostFailover.Length >= index)
                        break;

                    // Set the smtp host.
                    smtpClient.Host = this.Configuration.MailSmtpHostFailover[index++];
                }
            }

            if (!succeeded)
            {
                if (ex != null)
                    throw ex;
                else
                    throw new Exception("Unknown error.");
            }

            smtpClient.Dispose();
        }

        #endregion
    }

    public class MailConfiguration
    {
        #region Properties

        public string MailSmtpHost { get; set; }

        public string[] MailSmtpHostFailover { get; set; }

        public bool MailSmtpSSL { get; set; }

        public string MailSmtpUsername { get; set; }

        public string MailSmtpPassword { get; set; }

        public string MailBCC { get; set; }

        public string MailFrom { get; set; }

        public bool MailUseCredentialCache { get; set; }

        #endregion


        #region Constructor

        public MailConfiguration(bool useConfigurationManager = false)
        {
            if (useConfigurationManager)
            {
                LoadFromConfigurationManager();
            }
        }

        #endregion


        #region Methods

        private void LoadFromConfigurationManager()
        {
            this.MailBCC = ConfigurationManager.AppSettings["MailBCC"];
            this.MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            this.MailSmtpHost = ConfigurationManager.AppSettings["MailSmtpHost"];
            this.MailSmtpSSL = bool.Parse(
                ConfigurationManager.AppSettings["MailSmtpSSL"]
            );
            this.MailSmtpUsername = ConfigurationManager.AppSettings["MailSmtpUsername"];
            this.MailSmtpPassword = ConfigurationManager.AppSettings["MailSmtpPassword"];
            this.MailUseCredentialCache = bool.Parse(
                ConfigurationManager.AppSettings["MailUseCredentialCache"]
            );

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MailSmtpHostFailover"]))
            {
                this.MailSmtpHostFailover = ConfigurationManager.AppSettings["MailSmtpHostFailover"].Split(',');
            }
        }

        #endregion


        #region Event Handlers

        #endregion
    }
}
