using ApplicationUtilities;
using Crosstables.Classes.ReportDefinitionClasses;
using DatabaseCore.Items;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LinkBi1.Classes.Interfaces
{
    public abstract class LinkBiInterface
    {
        #region Properties

        /// <summary>
        /// Gets or sets the LinkBi definition of the interface.
        /// </summary>
        public LinkBiDefinition Definition { get; set; }

        /// <summary>
        /// Gets or sets the full path to the temp file
        /// where the interface has rendered to.
        /// </summary>
        public string FileName { get; set; }

        public DatabaseCore.Core Core { get; set; }

        public int Progress { get; set; }

        public virtual string MimeType
        {
            get
            {
                return "text/plain";
            }
        }

        public bool SendMail { get; set; }
        public HttpRequest Request { get; set; }
        public string Language { get; set; }
        public User User { get; set; }

        #endregion


        #region Constructor

        public LinkBiInterface(DatabaseCore.Core core)
        {
            this.Core = core;
        }

        public LinkBiInterface(DatabaseCore.Core core, LinkBiDefinition definition)
            : this(core)
        {
            this.Definition = definition;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Reads the LinkBi definition from the interface's source document.
        /// </summary>
        /// <param name="source"></param>
        public abstract void Read(string source);

        /// <summary>
        /// Renders the LinkBi definition to the interface's document format.
        /// </summary>
        /// <returns></returns>
        public abstract string Render();

        public Data InitFilter()
        {
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Core, 
                null
            );

            Data filter = null;
            Data workflowFilter = null;

            if(this.Definition.Workflow != null)
                workflowFilter = this.Definition.Workflow.GetWorkflowFilter(storageMethod, true);

            workflowFilter = this.Definition.GetHierarchyFilter(workflowFilter);

            foreach (FilterCategoryOperator filterCategoryOperator in this.Definition.FilterCategories)
            {
                filter = filterCategoryOperator.GetRespondents(
                    storageMethod,
                    null
                );
            }

            if (filter != null && workflowFilter != null)
            {
                List<Guid> removalRespondents = new List<Guid>();
                foreach (Guid idRespondent in filter.Responses.Keys)
                {
                    if (!workflowFilter.Responses.ContainsKey(idRespondent))
                    {
                        removalRespondents.Add(idRespondent);
                    }
                }

                foreach (Guid idRespondent in removalRespondents)
                {
                    filter.Responses.Remove(idRespondent);
                }
            }
            else if (filter == null)
            {
                filter = workflowFilter;
            }

            return filter;
        }

        protected void Finalize(string fileName)
        {
            if (!this.SendMail)
                return;

            // Create a new mail configuration and load the
            // configuration values from the web.config file.
            MailConfiguration mailConfiguration = new MailConfiguration(true);

            // Create a new mail by the mail configuration.
            Mail mail = new Mail(mailConfiguration, this.Core.ClientName);

            // Set the full path to the mail's template file.
            mail.TemplatePath = Path.Combine(
                this.Request.PhysicalApplicationPath,
                "App_Data",
                "MailTemplates",
                this.Language,
                "LinkBiExportFinished.html"
            );

            // Add the placeholder value for the user's first name.
            mail.Placeholders.Add("FirstName", this.User.FirstName);

            // Add the placeholder value for the user's last name.
            mail.Placeholders.Add("LastName", this.User.LastName);

            mail.Placeholders.Add("imagepath", "http://" + Request.Url.Host.ToString() + "/Images/Logos/link.png");

            mail.Attachments.Add(
                "LinkBiExport.xls",
                File.ReadAllBytes(fileName)
            );

            try
            {
                // Send the mail.
                mail.Send(this.User.Mail);
            }
            catch (Exception ex)
            {
                /*base.ShowMessage(
                    string.Format(Global.LanguageManager.GetText("MailSendError"), Global.User.Mail, ex.Message),
                    WebUtilities.MessageType.Error
                );*/
            }
        }

        #endregion
    }

    public enum LinkBiInterfaceType
    {
        PowerBI,
        CustomCharts
    }
}
