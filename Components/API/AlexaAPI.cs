using Crosstables.Classes.ReportDefinitionClasses.Collections;
using DatabaseCore.Items;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using WebUtilities;

namespace API
{
    public class AlexaAPI : IHttpHandler, IRequiresSessionState
    {
        #region Properties

        /// <summary>
        /// Gets or sets the available methods of the generic handler.
        /// </summary>
        public Dictionary<string, Meth> Methods { get; set; }

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

        public AlexaAPI()
        {
            this.Methods = new Dictionary<string, Meth>();

            this.Methods.Add("Authenticate", Authenticate);
            this.Methods.Add("HighestValue", HighestValue);
            this.Methods.Add("MeanScore", MeanScore);
            this.Methods.Add("GetValue", GetValue);
        }

        #endregion


        #region Methods

        private void LogRequest(HttpContext context)
        {
            StringBuilder result = new StringBuilder();
            result.Append(string.Format(
                "<Request Timestamp=\"{0}\">",
                DateTime.Now.ToString("yyyyMMddHHmm")
            ));
            result.Append(Environment.NewLine);

            string[] ignoreParameters = new string[]
            {
                "_et_coid","_ga","ASP.NET_SessionId","ALL_HTTP","ALL_RAW","APPL_MD_PATH","APPL_PHYSICAL_PATH","AUTH_TYPE","AUTH_USER","AUTH_PASSWORD","LOGON_USER","REMOTE_USER","CERT_COOKIE","CERT_FLAGS","CERT_ISSUER","CERT_KEYSIZE","CERT_SECRETKEYSIZE","CERT_SERIALNUMBER","CERT_SERVER_ISSUER","CERT_SERVER_SUBJECT","CERT_SUBJECT","CONTENT_LENGTH","CONTENT_TYPE","GATEWAY_INTERFACE","HTTPS","HTTPS_KEYSIZE","HTTPS_SECRETKEYSIZE","HTTPS_SERVER_ISSUER","HTTPS_SERVER_SUBJECT","INSTANCE_ID","INSTANCE_META_PATH","LOCAL_ADDR","PATH_INFO","PATH_TRANSLATED","QUERY_STRING","REMOTE_ADDR","REMOTE_HOST","REMOTE_PORT","REQUEST_METHOD","SCRIPT_NAME","SERVER_NAME","SERVER_PORT","SERVER_PORT_SECURE","SERVER_PROTOCOL","SERVER_SOFTWARE","URL","HTTP_CACHE_CONTROL","HTTP_CONNECTION","HTTP_ACCEPT","HTTP_ACCEPT_ENCODING","HTTP_ACCEPT_LANGUAGE","HTTP_COOKIE","HTTP_HOST","HTTP_USER_AGENT","HTTP_UPGRADE_INSECURE_REQUESTS"
            };

            foreach (string key in context.Request.Params.Keys)
            {
                if (ignoreParameters.Contains(key))
                    continue;

                result.Append(string.Format(
                    "<Parameter Key=\"{0}\"><![CDATA[{1}]]></Paramter>",
                    key,
                    context.Request.Params[key]
                ));
                result.Append(Environment.NewLine);
            }

            result.Append("</Request>");
            result.Append(Environment.NewLine);

            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Logs",
                Global.Core.ClientName,
                DateTime.Now.ToString("yyyyMMdd") + ".xml"
            );

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.AppendAllText(fileName, result.ToString());
            result.Clear();
        }


        /// <summary>
        /// Processes the current request.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(HttpContext context)
        {
            string method = "";

            ErrorResponse errorResponse = ErrorResponse.Exception;

            if (context.Request.Params["ErrorResponse"] != null)
            {
                errorResponse = (ErrorResponse)Enum.Parse(
                    typeof(ErrorResponse),
                    context.Request.Params["ErrorResponse"]
                );
            }

            //try
            {
                method = context.Request.Params["Method"];

                // Check if the current session is authenticated.
                if (Global.IdUser.HasValue == false)
                {
                    Authenticate(context);
                }

                // Check if the requested method exists.
                if (!this.Methods.ContainsKey(method))
                    throw new NotImplementedException();

                if (Global.PermissionCore == null)
                {
                    string clientName = context.Request.Url.Host.Split('.')[0];
                    Global.PermissionCore = new PermissionCore.PermissionCore(
                        HttpContext.Current.Request.PhysicalApplicationPath,
                        "LinkOnline",
                        clientName
                    );
                }

                // Check if the requested method is restricted by a permission.
                if (Global.IdUser.HasValue && Global.PermissionCore.Permissions[method] != null)
                {
                    // Check if the authenticated user has the
                    // permission to run the requested method.
                    if (!Global.Core.Users.HasPermission(Global.IdUser.Value, Global.PermissionCore.Permissions[method].Id))
                        throw new Exception("AccessDenied");
                }

                // Invoke the requested method.
                this.Methods[method].Invoke(context);
            }
            /*catch (Exception ex)
            {
                if (errorResponse == ErrorResponse.Exception)
                    throw ex;

                context.Response.Clear();
                context.Response.Write("{");
                context.Response.Write(string.Format(
                    "\"Error\": \"" + ex.Message + "\""
                ));
                context.Response.Write("}");
                context.Response.End();
            }*/
        }
        private void Authenticate(HttpContext context)
        {
            AmazonUsers users = new AmazonUsers();

            if (!users.Items.ContainsKey(context.Request.Params["User"]))
                throw new Exception("AccessDenied.");

            AmazonUser amazonUser = users.Items[context.Request.Params["User"]];

            string clientName = amazonUser.Client;

            Global.ClientManager = new ApplicationUtilities.Classes.ClientManager(Path.Combine(
                ConfigurationManager.AppSettings["ApplicationPath"],
                "App_Data",
                "Clients.xml"
            ));
            Global.LanguageManager = new LanguageManager(amazonUser.Client, context.Request.PhysicalApplicationPath);
            HttpContext.Current.Session["Language"] = Language.English;

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
            Global.LanguageManager = new LanguageManager(
                clientName,
                context.Request.PhysicalApplicationPath
            );

            /*if (!Directory.Exists(Global.Core.FileStorageRoot))
                Directory.CreateDirectory(Global.Core.FileStorageRoot);

            Global.Core.LogDirectory = ConfigurationManager.AppSettings["DatabaseChangeLogDirectory"];*/

            Global.Core.ClientName = clientName;
            Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(clientName).CaseDataVersion;
            Global.Core.CaseDataLocation = Global.ClientManager.GetSingle(clientName).CaseDataLocation;

            Global.IdUser = amazonUser.Id;

            Global.UserDefaults = new UserDefaults(Path.Combine(
                ConfigurationManager.AppSettings["ApplicationPath"],
                "Fileadmin",
                "UserDefaults",
                Global.Core.ClientName,
                Global.IdUser.Value + ".xml"
            ));

            Global.PermissionCore = new PermissionCore.PermissionCore(
                ConfigurationManager.AppSettings["ApplicationPath"],
                "LinkOnline",
                Global.Core.ClientName
            );
        }

        private Guid? FindVariable(string value)
        {
            Dictionary<string, List<object[]>> variableLabels = Global.Core.TaxonomyVariableLabels.ExecuteReaderDict<string>(
                "SELECT Label, IdTaxonomyVariable FROM TaxonomyVariableLabels",
                new object[] { }
            );

            double highestMatch = 0;
            Guid? result = null;

            double match;
            foreach (string label in variableLabels.Keys)
            {
                match = MatchLabels(value, label);

                if (match > highestMatch)
                {
                    highestMatch = match;
                    result = (Guid)variableLabels[label][0][1];
                }
            }

            return result;
        }

        private object[] FindCategory(string value, Guid? idTaxonomyVariable = null)
        {
            Dictionary<string, List<object[]>> categories = Global.Core.TaxonomyVariableLabels.ExecuteReaderDict<string>(
                "SELECT TaxonomyCategoryLabels.Label, TaxonomyCategories.Id, TaxonomyCategories.IdTaxonomyVariable FROM TaxonomyCategories INNER JOIN TaxonomyCategoryLabels ON TaxonomyCategoryLabels.IdTaxonomyCategory=TaxonomyCategories.Id",
                new object[] { }
            );

            double highestMatch = 0;
            object[] result = null;

            double match;
            foreach (string label in categories.Keys)
            {
                /*if (idTaxonomyVariable.HasValue && (Guid)categories
                    [label][0][2] != idTaxonomyVariable.Value)
                    continue;*/

                match = MatchLabels(value, label);

                if (match > highestMatch)
                {
                    highestMatch = match;
                    result = categories[label][0];
                }
            }

            return result;
        }


        char[] separators = new char[]
        {
                ' ', '=', '<', '>', '.', ',', '!', '?', '-', ':'
        };
        private double MatchLabels(string label1, string label2)
        {
            int count = 0;

            label1 = label1.ToLower();
            label2 = label2.ToLower();

            string[] words1 = label1.Split(separators);
            string[] words2 = label2.Split(separators);

            int index = 0;
            foreach (string word1 in words1)
            {
                index += word1.Length + 1;

                if (words2.Contains(word1))
                    count++;
                else if (word1.EndsWith("s")) // Plural
                {
                    if (words2.Contains(word1.Remove(word1.Length - 1, 1)))
                    {
                        count++;
                    }
                }
            }

            index = 0;
            foreach (string word2 in words2)
            {
                index += word2.Length + 1;

                if (words1.Contains(word2))
                    count++;
                else if (word2.EndsWith("s")) // Plural
                {
                    if (words1.Contains(word2.Remove(word2.Length - 1, 1)))
                    {
                        count++;
                    }
                }
            }

            return count * 100.0 / (words1.Length + words2.Length);
        }

        #endregion


        #region API Methods

        private void HighestValue(HttpContext context)
        {
            LogRequest(context);

            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                Global.Core,
                null
            );

            Guid? idVariable = FindVariable(context.Request.Params["Variable"]);

            if (!idVariable.HasValue)
            {
                context.Response.Write(Global.LanguageManager.GetText("Error_VariableNotExists"));
                return;
            }

            Dictionary<Guid, List<object[]>> categories = Global.Core.TaxonomyCategories.ExecuteReaderDict<Guid>(
                "SELECT IdTaxonomyVariable, Id FROM TaxonomyCategories",
                new object[] { }
            );

            Data filter = null;

            if (context.Request.Params["Filter"] != null)
            {
                object[] category = FindCategory(context.Request.Params["Filter"]);

                if (category != null)
                {
                    filter = storageMethod.GetRespondents(
                        (Guid)category[1],
                        (Guid)category[2],
                        true,
                        Global.Core.CaseDataLocation,
                        filter
                    );
                }
            }

            if (context.Request.Params["Filter2"] != null)
            {
                object[] category = FindCategory(context.Request.Params["Filter2"]);

                if (category != null)
                {
                    filter = storageMethod.GetRespondents(
                        (Guid)category[1],
                        (Guid)category[2],
                        true,
                        Global.Core.CaseDataLocation,
                        filter
                    );
                }
            }

            if (context.Request.Params["Filter3"] != null)
            {
                object[] category = FindCategory(context.Request.Params["Filter3"]);

                if (category != null)
                {
                    filter = storageMethod.GetRespondents(
                        (Guid)category[1],
                        (Guid)category[2],
                        true,
                        Global.Core.CaseDataLocation,
                        filter
                    );
                }
            }

            WeightingFilterCollection weighting = null;

            Guid? idWeightingVariable = null;

            if (context.Request.Params["WeightingVariable"] != null)
            {
                idWeightingVariable = FindVariable(context.Request.Params["WeightingVariable"]);

                if (!idWeightingVariable.HasValue)
                {
                    context.Response.Write(
                        Global.LanguageManager.GetText("Error_VariableNotExists"));
                    return;
                }

                weighting = new WeightingFilterCollection(null, Global.Core, null);
                weighting.DefaultWeighting = idWeightingVariable;
                weighting.LoadRespondents(null);
            }

            string highestIdentifier = "highest";

            if (context.Request.Params["HighestIdentifier"] != null)
            {
                highestIdentifier = context.Request.Params["HighestIdentifier"];

                if (highestIdentifier == "undefined")
                    highestIdentifier = "most";
            }

            if (categories.ContainsKey(idVariable.Value))
            {
                double highestShare = 0;
                Guid? idCategoryHighestShare = null;

                Data data = storageMethod.GetRespondents(
                    idVariable.Value,
                    true,
                    Global.Core.CaseDataLocation,
                    filter,
                    weighting
                );

                double baseValue = data.Value;

                foreach (object[] category in categories[idVariable.Value])
                {
                    data = storageMethod.GetRespondents(
                        (Guid)category[1],
                        idVariable.Value,
                        true,
                        Global.Core.CaseDataLocation,
                        filter,
                        weighting
                    );

                    data.Value = data.Value * 100 / baseValue;

                    if (data.Value > highestShare)
                    {
                        highestShare = data.Value;
                        idCategoryHighestShare = (Guid)category[1];
                    }
                }

                Dictionary<Guid, List<object[]>> categoryLabels = Global.Core.TaxonomyCategoryLabels.ExecuteReaderDict<Guid>(
                    "SELECT IdTaxonomyCategory, Label FROM TaxonomyCategoryLabels",
                    new object[] { }
                );

                if (idCategoryHighestShare.HasValue)
                {
                    context.Response.Write(string.Format(
                        Global.LanguageManager.GetText("Output_HighestValue"),
                        context.Request.Params["Variable"],
                        categoryLabels[idCategoryHighestShare.Value][0][1],
                        Math.Round(highestShare, 0),
                        highestIdentifier,
                        context.Request.Params["WeightingVariable"] == null ? "value" :
                        context.Request.Params["WeightingVariable"]
                    ));
                }
                else
                {
                    context.Response.Write(string.Format(
                        Global.LanguageManager.GetText("Error_ValueNotFound")
                    ));
                }
            }
            else
            {
                Data data = storageMethod.GetRespondents(
                    idVariable.Value,
                    true,
                    Global.Core.CaseDataLocation,
                    filter,
                    weighting
                );

                context.Response.Write(string.Format(
                    Global.LanguageManager.GetText("Output_Value"),
                    context.Request.Params["Variable"],
                    data.Value
                ));
            }
        }

        private void MeanScore(HttpContext context)
        {
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                Global.Core,
                null
            );

            Guid? idVariable = FindVariable(context.Request.Params["Variable"]);

            if (!idVariable.HasValue)
            {
                context.Response.Write(
                    Global.LanguageManager.GetText("Error_VariableNotExists"));
                return;
            }

            Dictionary<Guid, List<object[]>> categories = Global.Core.TaxonomyCategories.ExecuteReaderDict<Guid>(
                "SELECT IdTaxonomyVariable, Id, [Value] FROM TaxonomyCategories",
                new object[] { }
            );

            if (categories.ContainsKey(idVariable.Value))
            {
                double meanScore = 0;
                double baseValue = 0;

                Data data = storageMethod.GetRespondents(
                    idVariable.Value,
                    true,
                    Global.Core.CaseDataLocation
                );

                baseValue = data.Value;

                foreach (object[] category in categories[idVariable.Value])
                {
                    data = storageMethod.GetRespondents(
                        (Guid)category[1],
                        idVariable.Value,
                        true,
                        Global.Core.CaseDataLocation
                    );

                    data.Value = data.Value * (int)category[2] / baseValue;

                    meanScore += data.Value;
                }

                context.Response.Write(string.Format(
                    Global.LanguageManager.GetText("Output_Value2"),
                    context.Request.Params["Variable"],
                    Math.Round(meanScore, 2)
                ));
            }
            else
            {
                Data data = storageMethod.GetRespondents(
                    idVariable.Value,
                    true,
                    Global.Core.CaseDataLocation
                );

                context.Response.Write(string.Format(
                    Global.LanguageManager.GetText("Output_Value"),
                    context.Request.Params["Variable"],
                    data.Value
                ));
            }
        }

        private void GetValue(HttpContext context)
        {
            LogRequest(context);

            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                Global.Core,
                null
            );

            Calculation calculation = Calculation.Total;

            if (context.Request.Params["Calculation"] != null)
            {
                if (!Enum.TryParse<Calculation>(
                    context.Request.Params["Calculation"],
                    true,
                    out calculation
                ))
                {
                    if (context.Request.Params["Category"] != null)
                        calculation = Calculation.Difference;
                }
            }
            else
            {
                if (context.Request.Params["Category"] != null)
                    calculation = Calculation.Difference;
            }

            Data filter = null;

            /*filter = storageMethod.GetRespondentsNumeric(
                idVariable.Value,
                true,
                Global.Core.CaseDataLocation,
                filter
            );*/

            WeightingFilterCollection weighting = null;

            Guid? idWeightingVariable = null;

            if (context.Request.Params["WeightingVariable"] != null)
            {
                idWeightingVariable = FindVariable(context.Request.Params["WeightingVariable"]);

                if (!idWeightingVariable.HasValue)
                {
                    context.Response.Write(
                        Global.LanguageManager.GetText("Error_VariableNotExists"));
                    return;
                }

                weighting = new WeightingFilterCollection(null, Global.Core, null);
                weighting.DefaultWeighting = idWeightingVariable;
                weighting.LoadRespondents(filter);
            }

            if (context.Request.Params["Filter"] != null)
            {
                object[] category = FindCategory(context.Request.Params["Filter"]);

                if (category != null)
                {
                    filter = storageMethod.GetRespondents(
                        (Guid)category[1],
                        (Guid)category[2],
                        true,
                        Global.Core.CaseDataLocation,
                        filter,
                        weighting
                    );
                }
            }

            if (context.Request.Params["Filter2"] != null)
            {
                object[] category = FindCategory(context.Request.Params["Filter2"]);

                if (category != null)
                {
                    filter = storageMethod.GetRespondents(
                        (Guid)category[1],
                        (Guid)category[2],
                        true,
                        Global.Core.CaseDataLocation,
                        filter,
                        weighting
                    );
                }
            }

            if (context.Request.Params["Filter3"] != null)
            {
                object[] category = FindCategory(context.Request.Params["Filter3"]);

                if (category != null)
                {
                    filter = storageMethod.GetRespondents(
                        (Guid)category[1],
                        (Guid)category[2],
                        true,
                        Global.Core.CaseDataLocation,
                        filter,
                        weighting
                    );
                }
            }

            double result = 0.0;

            if (calculation != Calculation.Difference && filter == null)
            {
                if (idWeightingVariable.HasValue)
                {
                    filter = storageMethod.GetRespondentsNumeric(
                        idWeightingVariable.Value,
                        true,
                        Global.Core.CaseDataLocation,
                        filter,
                        weighting
                    );
                }
                else
                {
                    filter = new Data();
                }
            }

            switch (calculation)
            {
                case Calculation.Average:
                case Calculation.Mean:
                    result = filter.Value / filter.Responses.Count;

                    result = Math.Round(result, 2);

                    if (result > 1000)
                        result = Math.Round(result, 0);

                    context.Response.Write(string.Format(
                        Global.LanguageManager.GetText("Output_Value"),
                        calculation,
                        result
                    ));
                    break;
                case Calculation.Total:
                    result = filter.Value;

                    result = Math.Round(result, 2);

                    if (result > 1000)
                        result = Math.Round(result, 0);

                    context.Response.Write(string.Format(
                        Global.LanguageManager.GetText("Output_Value"),
                        calculation,
                        result,
                        idWeightingVariable.HasValue ? context.Request.Params["WeightingVariable"] : ""
                    ));
                    break;
                case Calculation.Difference:
                case Calculation.Up:
                case Calculation.Increased:
                    Data data = filter;
                    object[] category = FindCategory(context.Request.Params["Category"]);

                    if (category != null)
                    {
                        data = storageMethod.GetRespondents(
                            (Guid)category[1],
                            (Guid)category[2],
                            true,
                            Global.Core.CaseDataLocation,
                            filter,
                            weighting
                        );
                    }

                    double value1 = data.Value;
                    double baseValue = data.Value;

                    category = FindCategory(context.Request.Params["CategoryCompare"]);

                    data = storageMethod.GetRespondents(
                        (Guid)category[1],
                        (Guid)category[2],
                        true,
                        Global.Core.CaseDataLocation,
                        filter,
                        weighting
                    );

                    value1 = data.Value - value1;

                    result = Math.Round(result, 2);

                    ValueChange valueChange = ValueChange.Increase;

                    if (value1 < 10)
                    {
                        valueChange = ValueChange.Decrease;
                    }

                    if (value1 == 0)
                    {
                        context.Response.Write(string.Format(
                            Global.LanguageManager.GetText("Output_Difference_NoChange"),
                            context.Request.Params["WeightingVariable"],
                            context.Request.Params["CategoryCompare"]
                        ));
                    }
                    else
                    {
                        value1 = Math.Round(Math.Abs(value1 * 100 / baseValue), 0);

                        if (value1 == 0)
                            value1 = Math.Round(Math.Abs(value1 * 100 / baseValue), 2);

                        string outputType = "Percent";
                        if (value1 > 200)
                        {
                            value1 = Math.Round(value1 / 100);

                            outputType = "Times";
                            calculation = Calculation.Difference;
                        }

                        if (calculation == Calculation.Difference)
                        {
                            context.Response.Write(string.Format(
                                Global.LanguageManager.GetText("Output_Difference"),
                                context.Request.Params["WeightingVariable"],
                                Global.LanguageManager.GetText("Type_ValueChange_" + valueChange),
                                value1,
                                context.Request.Params["CategoryCompare"],
                                outputType
                            ));
                        }
                        else
                        {
                            context.Response.Write(string.Format(
                                Global.LanguageManager.GetText("Output_Difference2"),
                                context.Request.Params["WeightingVariable"],
                                Global.LanguageManager.GetText("Type_ValueChange_" + calculation + "_" + valueChange),
                                value1,
                                context.Request.Params["Category"]
                            ));
                        }
                    }

                    break;
            }
        }

        #endregion
    }
    public class AmazonUsers
    {
        #region Properties

        public XmlDocument Document { get; set; }

        public Dictionary<string, AmazonUser> Items { get; set; }

        #endregion


        #region Constructor

        public AmazonUsers()
        {
            this.Document = new XmlDocument();
            this.Document.Load(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Users.xml"
            ));

            this.Load();
        }

        #endregion


        #region Methods

        private void Load()
        {
            this.Items = new Dictionary<string, AmazonUser>();

            XmlNodeList xmlNodes = this.Document.DocumentElement.SelectNodes("User");

            AmazonUser user;
            foreach (XmlNode xmlNode in xmlNodes)
            {
                user = new AmazonUser();
                user.Id = Guid.Parse(xmlNode.Attributes["IdUser"].Value);
                user.Client = xmlNode.Attributes["Client"].Value;
                user.AmazonId = xmlNode.Attributes["AmazonId"].Value;

                this.Items.Add(user.AmazonId, user);
            }
        }

        #endregion


        #region Event Handlers

        #endregion
    }

    public struct AmazonUser
    {
        public Guid Id;
        public string Client;
        public string AmazonId;
    }

    public enum Calculation
    {
        Average,
        Mean,
        Total,
        Difference,
        Up,
        Increased
    }

    public enum ValueChange
    {
        Increase, Decrease
    }
}
