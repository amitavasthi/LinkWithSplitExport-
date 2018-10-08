using API;
using LinkOnline.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Xml;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for LinkOverview
    /// </summary>
    public class LinkOverview : IHttpHandler, IRequiresSessionState
    {
        #region Properties
        /// <summary>
        /// Gets or sets the available methods of the generic handler.
        /// </summary>
        public Dictionary<string, Meth> Methods { get; set; }
        /// <summary>
        /// Gets or sets the available xml methods of the generic handler.
        /// </summary>
        public Dictionary<string, ExternalMethod> XmlMethods { get; set; }

        /// <summary>
        /// Gets if the generic handler is re useable.
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Constructor

        public LinkOverview()
        {

            this.Methods = new Dictionary<string, Meth>();

            this.Methods.Add("ProcessReport", ProcessReport);
            this.Methods.Add("GetStudyResponse", GetStudyResponse);
        }

        #endregion
        #region Methods

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(HttpContext context)
        {
            string method;

            // Check if the current request is a xml request.
            if (context.Request.Params["Method"] == null)
            {
                // Create a new xml document that contains the request definition.
                XmlDocument xmlDocument = new XmlDocument();

                Stream test = context.Request.GetBufferedInputStream();

                byte[] buffer = new byte[test.Length];

                test.Read(buffer, 0, (int)test.Length);

                string requestString = System.Text.Encoding.UTF8.GetString(buffer);

                // Load the content of the request stream into the xml document.
                //xmlDocument.Load(context.Request.GetBufferedInputStream());
                xmlDocument.LoadXml(HttpUtility.HtmlDecode(requestString).Trim());

                InitDatabaseConnection(context);

                // Check the authentication key.
                if (!CheckAuthentication(context, xmlDocument))
                    throw new Exception("Not authenticated.");

                // Get the name of the requested method from the xml document node's attributes.
                method = xmlDocument.DocumentElement.Attributes["Method"].Value;

                // Check if the requested method exists.
                if (!this.XmlMethods.ContainsKey(method))
                    throw new NotImplementedException();

                // Invoke the requested method.
                this.XmlMethods[method].Invoke(context, xmlDocument);
            }
            else
            {
                method = context.Request.Params["Method"];

                // Check if the current session is authenticated.
                if (Global.User == null)
                    throw new Exception("Not authenticated.");

                // Check if the requested method exists.
                if (!this.Methods.ContainsKey(method))
                    throw new NotImplementedException();

                // Invoke the requested method.
                this.Methods[method].Invoke(context);
            }
        }

        private bool CheckAuthentication(HttpContext context, XmlDocument xmlDocument)
        {
            string key = xmlDocument.DocumentElement.SelectSingleNode("Authentication").InnerText;

            DateTime requestTime = DateTime.Parse(xmlDocument.DocumentElement.Attributes["Timestamp"].Value);

            string compareKey = Global.Core.Users.GetMD5Hash(
                GetClientName(context) + requestTime.ToString("yyyyMMddHHmm")
            );

            if (key.ToUpper() == compareKey.ToUpper())
                return true;

            return false;
        }

        private void InitDatabaseConnection(HttpContext context)
        {
            string clientName = GetClientName(context);

            // Check if the database core of the current session is initialized.
            if (Global.Core == null || Global.Core.ClientName != clientName)
            {
                if (Global.ClientManager == null)
                    Global.ClientManager = new ApplicationUtilities.Classes.ClientManager();

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

                // Set the database core's file storage path.
                Global.Core.FileStorageRoot = string.Format(
                    ConfigurationManager.AppSettings["FileStorageRoot"],
                    clientName
                );

                // Initialize the session's language manager.
                Global.LanguageManager = new WebUtilities.LanguageManager(
                    clientName,
                    context.Request.PhysicalApplicationPath
                );

                Global.Core.LogDirectory = ConfigurationManager.AppSettings["DatabaseChangeLogDirectory"];

                Global.Core.ClientName = clientName;
                Global.Core.CaseDataVersion = Global.ClientManager.
                    GetSingle(clientName).CaseDataVersion;
                Global.Core.CaseDataLocation = Global.ClientManager.
                    GetSingle(clientName).CaseDataLocation;
            }
        }

        /// <summary>
        /// Gets the client name from the request url.
        /// </summary>
        private string GetClientName(HttpContext context)
        {
            return context.Request.Url.Host.Split('.')[0];
        }

        #endregion
        #region Web Methods

        private void ProcessReport(HttpContext context)
        {
            LinkOverviewResponseType responseType = LinkOverviewResponseType.XML;

            if (context.Request.Params["ResponseType"] != null)
            {
                // Get the response type from the request xml document.
                responseType = (LinkOverviewResponseType)Enum.Parse(
                    typeof(LinkOverviewResponseType),
                    context.Request.Params["ResponseType"]
                );
            }

            int idLanguage = 2057;

            if (context.Request.Params["IdLanguage"] != null)
            {
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);
            }
            int linked = Global.Core.VariableLinks.GetCount();
            int variables = Global.Core.Variables.GetCount();
            int unLinkedVariableCount = variables - linked;

            int linkedC = Global.Core.CategoryLinks.GetCount();
            int variablesC = Global.Core.Categories.GetCount();
            int unLinkedCCount = variablesC - linkedC;

            var study = Global.Core.Studies.GetCount();



            if (responseType == LinkOverviewResponseType.XML)
            {
                // Create a new string builder that build the xml string for the dimension definition.
                StringBuilder xmlBuilder = new StringBuilder();

                xmlBuilder.Append("<Response>");

                xmlBuilder.Append(string.Format(
               "<LinkedVariable linkedCount=\"{0}\" UnLinkedCount=\"{1}\" Total=\"{2}\">",
               linked, unLinkedVariableCount, variables));

                xmlBuilder.Append("</LinkedVariable>");

                xmlBuilder.Append(string.Format(
                "<LinkedCategory linkedCount=\"{0}\" UnLinkedCount=\"{1}\" Total=\"{2}\">",
                linkedC, unLinkedCCount, variablesC));

                xmlBuilder.Append("</LinkedCategory>");

                xmlBuilder.Append(string.Format(
              "<Studies Total=\"{0}\" >", study));

                xmlBuilder.Append("</Studies>");

                xmlBuilder.Append("</Response>");
                context.Response.Write(xmlBuilder);
            }
            else
            {
                //var linkOverView = new LinkOverviewHelper[]
                //{
                //   new LinkOverviewHelper(){
                //       linkedVariables = linked,
                //       unLinkedVariables=unLinkedVariableCount,
                //       linkedCategories = linkedC,
                //       unLinkedCategories=unLinkedVariableCount,
                //       studies=study
                //   }
                //};

                var objectToSerialize = new RootObject();
                objectToSerialize.values = new List<LinkOverviewHelper> 
                          {
                             new LinkOverviewHelper { studies=study ,linkedVariables = linked, unLinkedVariables = unLinkedVariableCount,linkedCategories=linkedC,unLinkedCategories=unLinkedVariableCount},

                          };
                context.Response.Write(new JavaScriptSerializer().Serialize(objectToSerialize));
            }

            // Set the content type of the http response to plain text.
            context.Response.ContentType = "text/plain";
        }


        private void GetStudyResponse(HttpContext context)
        {
            LinkOverviewResponseType responseType = LinkOverviewResponseType.XML;

            if (context.Request.Params["ResponseType"] != null)
            {
                // Get the response type from the request xml document.
                responseType = (LinkOverviewResponseType)Enum.Parse(
                    typeof(LinkOverviewResponseType),
                    context.Request.Params["ResponseType"]
                );
            }

            int idLanguage = 2057;

            if (context.Request.Params["IdLanguage"] != null)
            {
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);
            }
            int linked = Global.Core.VariableLinks.GetCount();
            int variables = Global.Core.Variables.GetCount();
            int unLinkedVariableCount = variables - linked;

            int linkedC = Global.Core.CategoryLinks.GetCount();
            int variablesC = Global.Core.Categories.GetCount();
            int unLinkedCCount = variablesC - linkedC;

            var study = Global.Core.Studies.GetCount();

            var result = Global.Core.Studies.Get();



            if (responseType == LinkOverviewResponseType.XML)
            {
                // Create a new string builder that build the xml string for the dimension definition.
                StringBuilder xmlBuilder = new StringBuilder();

                xmlBuilder.Append("<Response>");

                for (int i = 0; i < result.Count(); i++)
                {
                    var sName = result[i].Name;
                    var responseCount = Global.Core.Respondents.GetCount("IdStudy", result[i].Id);
                    xmlBuilder.Append(string.Format(
                        "<StudyDetails studyName=\"{0}\" responseCount=\"{1}\">", sName, responseCount));
                    xmlBuilder.Append("</StudyDetails>");
                }
                xmlBuilder.Append("</Response>");
                context.Response.Write(xmlBuilder);
            }
            else
            {
                //StudyRepondents[]  studyResponse = null;
                //for (int i = 0; i < result.Count(); i++)
                //{
                //    var responseCount = Global.Core.Respondents.GetCount("IdStudy", result[i].Id);
                //     studyResponse = new StudyRepondents[]{
                //    new StudyRepondents(){
                //        study =  result[i].Name,
                //        responseCount = responseCount
                //        }
                //    };
                //}
                var objectToSerialize = new RootRespondents();

                List<StudyRepondents> resultList = new List<StudyRepondents>();

                for (int i = 0; i < result.Count(); i++)
                {
                    var responseCount = Global.Core.Respondents.GetCount("IdStudy", result[i].Id);

                    var studyRespondents = new StudyRepondents
                                                  {
                                                      study = result[i].Name,
                                                      responseCount = responseCount
                                                  };

                    resultList.Add(studyRespondents);

                }

                objectToSerialize.values = resultList;
                context.Response.Write(new JavaScriptSerializer().Serialize(objectToSerialize));

            }


            // Set the content type of the http response to plain text.
            context.Response.ContentType = "text/plain";

        }

        #endregion

    }
    public enum LinkOverviewResponseType
    {
        XML,
        JSON
    }
}