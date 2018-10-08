using ApplicationUtilities;
using ApplicationUtilities.Classes;
using Crosstables.Classes.HierarchyClasses;
using Crosstables.Classes.ReportDefinitionClasses;
using DatabaseCore.Items;
using DataCore.Classes;
using LinkBi1.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Xml;
using WebUtilities;

namespace API
{
    public class APIHandler : IHttpHandler, IRequiresSessionState
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

        public APIHandler()
        {
            this.XmlMethods = new Dictionary<string, ExternalMethod>();
            this.Methods = new Dictionary<string, Meth>();

            this.XmlMethods.Add("GetVariables", GetVariables);
            this.XmlMethods.Add("ProcessReport", ProcessReport);

            this.Methods.Add("Authenticate", Authenticate);
            this.Methods.Add("IsAuthenticated", IsAuthenticated);

            this.Methods.Add("HasPermission", HasPermission);
            this.Methods.Add("ForgotPassword", ForgotPassword);
            this.Methods.Add("GetStudies", GetStudies);
            this.Methods.Add("GetVariable", GetVariable);
            this.Methods.Add("GetVariables", GetVariables);
            this.Methods.Add("GetCategories", GetCategories);
            this.Methods.Add("GetVariableLabels", GetVariableLabels);
            this.Methods.Add("GetVariableLinks", GetVariableLinks);
            this.Methods.Add("CreateStudy", CreateStudy);
            this.Methods.Add("CreateVariable", CreateVariable);
            this.Methods.Add("CreateTaxonomyVariable", CreateTaxonomyVariable);
            this.Methods.Add("CreateCategory", CreateCategory);
            this.Methods.Add("LinkVariable", LinkVariable);
            this.Methods.Add("LinkCategory", LinkCategory);
            this.Methods.Add("ClearResponses", ClearResponses);
            this.Methods.Add("InsertResponses", InsertResponses);
            this.Methods.Add("TransferResponses", TransferResponses);
            this.Methods.Add("GetRespondents", GetRespondents);
            this.Methods.Add("GetResponses", GetResponses);
            this.Methods.Add("GenerateGuid", GenerateGuid);
            this.Methods.Add("ClearCaseDataCache", ClearCaseDataCache);
            this.Methods.Add("GetNews", GetNews);
            this.Methods.Add("SaveNews", SaveNews);
            this.Methods.Add("DeleteNews", DeleteNews);
            this.Methods.Add("ProcessReport", ProcessReport);
            this.Methods.Add("ProcessDefinedReport", ProcessDefinedReport);
            this.Methods.Add("ProcessSavedReport", ProcessSavedReport);
            this.Methods.Add("SignificanceTest", SignificanceTest);
        }

        #endregion


        #region Methods

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

            try
            {
                // Check if the current request is a xml request.
                if (context.Request.Params["Method"] == null)
                {
                    Stream test = context.Request.InputStream;

                    byte[] buffer = new byte[test.Length];

                    test.Read(buffer, 0, (int)test.Length);

                    string requestString = System.Text.Encoding.UTF8.GetString(buffer);

                    // Create a new xml document that contains the request definition.
                    XmlDocument xmlDocument = new XmlDocument();

                    // Check if the request is a JSON request.
                    if (!requestString.Contains("<Request"))
                    {
                        // Convert the JSON request to a XML request.
                        requestString = JsonToXml(requestString);

                        requestString = requestString.Replace("<Children>", "");
                        requestString = requestString.Replace("</Children>", "");

                        requestString = requestString.Replace("<Categories>", "");
                        requestString = requestString.Replace("</Categories>", "");
                    }

                    // Load the content of the request stream into the xml document.
                    xmlDocument.LoadXml(HttpUtility.HtmlDecode(requestString).Trim());

                    // Get the name of the requested method from the xml document node's attributes.
                    method = xmlDocument.DocumentElement.Attributes["Method"].Value;

                    // Check if the current session is authenticated.
                    if (Global.IdUser.HasValue == false && method != "Authenticate" && method != "IsAuthenticated" && method != "ForgotPassword")
                    {
                        if (context.Request.Params["Username"] == null)
                            throw new Exception("Not authenticated.");
                        else
                            Authenticate(context);
                    }

                    // Check if the requested method is restricted by a permission.
                    if (Global.PermissionCore.Permissions[method] != null)
                    {
                        // Check if the authenticated user has the
                        // permission to run the requested method.
                        if (!Global.Core.Users.HasPermission(Global.IdUser.Value, Global.PermissionCore.Permissions[method].Id))
                            throw new Exception("AccessDenied");
                    }

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
                    if (Global.IdUser.HasValue == false && method != "Authenticate" && method != "IsAuthenticated" && method != "ForgotPassword")
                    {
                        if (context.Request.Params["Username"] == null)
                            throw new Exception("Not authenticated.");
                        else
                            Authenticate(context);
                    }

                    // Check if the requested method exists.
                    if (!this.Methods.ContainsKey(method))
                        throw new NotImplementedException();

                    if (Global.PermissionCore == null)
                    {
                        string clientName = context.Request.Url.Host.Split('.')[0];
                        Global.PermissionCore = new PermissionCore.PermissionCore(
                            context.Request.PhysicalApplicationPath,
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
            }
            catch (Exception ex)
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
            }
        }


        private void RenderCategories(
            Guid idVariable,
            int idLanguage,
            StringBuilder result
        )
        {
            // Get all taxonomy categories of the taxonomy variable.
            List<object[]> categories = Global.Core.Categories.GetValues(
                new string[] { "Id", "Name" },
                new string[] { "IdVariable" },
                new object[] { idVariable },
                "Order"
            );

            Dictionary<Guid, List<object[]>> categoryLabels = Global.Core.CategoryLabels.ExecuteReaderDict<Guid>(
                "SELECT IdCategory, Label FROM CategoryLabels WHERE IdLanguage={0}",
                new object[] { idLanguage }
            );
            Dictionary<Guid, List<object[]>> categoryLinks = Global.Core.CategoryLinks.ExecuteReaderDict<Guid>(
                "SELECT IdCategory, IdTaxonomyCategory, IdTaxonomyVariable FROM CategoryLinks",
                new object[] { }
            );

            string label;
            // Run through all taxonomy categories of the taxonomy variable.
            foreach (object[] category in categories)
            {
                label = "";

                if (categoryLabels.ContainsKey((Guid)category[0]))
                    label = (string)categoryLabels[(Guid)category[0]][0][1];

                string[] names = new string[] {
                    "Id",
                    "IdVariable",
                    "Name",
                    "Label",
                    "IdTaxonomyVariable",
                    "IdTaxonomyCategory"
                };
                object[] values = new object[] {
                    category[0],
                    idVariable,
                    category[1],
                    label,
                    categoryLinks.ContainsKey((Guid)category[0]) ? categoryLinks[(Guid)category[0]][0][2]: null,
                    categoryLinks.ContainsKey((Guid)category[0]) ? categoryLinks[(Guid)category[0]][0][1]: null
                };

                result.Append(this.ToJson(
                    names,
                    values
                ));

                result.Append(",");
            }
        }

        private void RenderTaxonomyCategories(
            Guid idVariable,
            int idLanguage,
            StringBuilder result,
            Dictionary<Guid, List<object[]>> taxonomyCategoryLabels
        )
        {
            // Get all taxonomy categories of the taxonomy variable.
            List<object[]> taxonomyCategories = Global.Core.TaxonomyCategories.GetValues(
                new string[] { "Id", "Name", "IsScoreGroup" },
                new string[] { "IdTaxonomyVariable", "Enabled" },
                new object[] { idVariable, true },
                "Order"
            );

            if (taxonomyCategoryLabels == null)
            {
                taxonomyCategoryLabels = Global.Core.TaxonomyCategoryLabels.ExecuteReaderDict<Guid>(
                    "SELECT IdTaxonomyCategory, Label FROM TaxonomyCategoryLabels WHERE IdLanguage={0}",
                    new object[] { idLanguage }
                );
            }

            string label;
            // Run through all taxonomy categories of the taxonomy variable.
            foreach (object[] taxonomyCategory in taxonomyCategories)
            {
                label = "";

                if (taxonomyCategoryLabels.ContainsKey((Guid)taxonomyCategory[0]))
                    label = (string)taxonomyCategoryLabels[(Guid)taxonomyCategory[0]][0][1];

                string[] names = new string[]{
                        "Id",
                        "IdTaxonomyVariable",
                        "Name",
                        "Label",
                        "IsScoreGroup"
                    };
                object[] values = new object[]{
                        taxonomyCategory[0],
                        idVariable,
                        taxonomyCategory[1],
                        label,
                        taxonomyCategory[2]
                    };

                result.Append(this.ToJson(
                    names,
                    values
                ));

                result.Append(",");
            }
        }



        private string ToJson(string[] names, object[] values)
        {
            StringBuilder jsonBuilder = new StringBuilder();

            jsonBuilder.Append("{");

            for (int i = 0; i < names.Length; i++)
            {
                if (values[i] == null)
                    continue;

                jsonBuilder.Append(string.Format(
                    "\"{0}\": \"{1}\",",
                    names[i],
                    HttpUtility.HtmlEncode(values[i]).Replace("\r", "").Replace("\n", "").Replace("\t", "")
                ));
            }

            string result = jsonBuilder.ToString();

            if (result.Length > 0)
                result = result.Remove(result.Length - 1, 1);

            result += "}";

            return result;
        }


        private void Authenticate(HttpContext context, bool isEncrypted)
        {
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
                isEncrypted
            );

            if (user == null)
                throw new Exception("InvalidCredentials");

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
        }

        private void MoveNestedNodes(XmlNode xmlNodeContainer, XmlNode xmlNode)
        {
            XmlNodeList xmlNodes = xmlNode.SelectNodes("Variable/Variable");

            foreach (XmlNode x in xmlNodes)
            {
                xmlNodeContainer.AppendChild(x);

                MoveNestedNodes(xmlNodeContainer, x);
            }
        }

        private void CreateResponsesTable(Guid idVariable)
        {
            // Read the script from the script template file.
            string script = File.ReadAllText(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "DataStorage",
                "CreateResponses.sql"
            ));

            // Format the script with the variable's id.
            script = string.Format(
                script,
                idVariable
            );

            // Lock the create table action.
            lock (this)
            {
                // Execute the script on the database.
                Global.Core.Responses[idVariable].ExecuteQuery(script);
            }
        }

        #endregion


        #region Xml Methods

        private void GetVariables(HttpContext context, XmlDocument xmlDocument)
        {
            LinkBiExternalResponseType responseType = LinkBiExternalResponseType.XML;

            if (xmlDocument.DocumentElement.SelectSingleNode("ResponseType") != null)
            {
                // Get the response type from the request xml document.
                responseType = (LinkBiExternalResponseType)Enum.Parse(
                    typeof(LinkBiExternalResponseType),
                    xmlDocument.DocumentElement.SelectSingleNode("ResponseType").InnerText
                );
            }

            // Get the id of the language for the labels
            // from the request xml document.
            int idLanguage = 2057;

            if (xmlDocument.DocumentElement.SelectSingleNode("IdLanguage") != null)
                int.Parse(xmlDocument.DocumentElement.SelectSingleNode("IdLanguage").InnerText);

            // Create a new xml builder that contains the xml result string.
            StringBuilder xmlBuilder = new StringBuilder();

            xmlBuilder.Append("<Response>");

            xmlBuilder.Append("<TaxonomyVariables>");

            // Get all taxonomy variables of the client.
            List<object[]> taxonomyVariables = Global.Core.TaxonomyVariables.GetValues(
                new string[] { "Id", "Name" }
            );

            // Run through all taxonomy variables of the client.
            foreach (object[] taxonomyVariable in taxonomyVariables)
            {
                // Get the label of the taxonomy variable in the requested language.
                string variableLabel = (string)Global.Core.TaxonomyVariableLabels.GetValue(
                    "Label",
                    new string[] { "IdTaxonomyVariable", "IdLanguage" },
                    new object[] { (Guid)taxonomyVariable[0], idLanguage }
                );

                // Add the taxonomy variable to the xml builder.
                xmlBuilder.Append(string.Format(
                    "<TaxonomyVariable Id=\"{0}\" Name=\"{1}\" Label=\"{2}\">",
                    (Guid)taxonomyVariable[0],
                    HttpUtility.HtmlEncode((string)taxonomyVariable[1]),
                    HttpUtility.HtmlEncode(variableLabel)
                ));

                // Get all categories of the taxonomy variable.
                List<object[]> categories = Global.Core.TaxonomyCategories.GetValues(
                    new string[] { "Id", "Name" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { (Guid)taxonomyVariable[0] }
                );

                xmlBuilder.Append("<TaxonomyCategories>");

                // Run through all categories of the taxonomy variable.
                foreach (object[] category in categories)
                {
                    // Get the label of the taxonomy variable in the requested language.
                    string categoryLabel = (string)Global.Core.TaxonomyCategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyCategory", "IdLanguage" },
                        new object[] { (Guid)category[0], idLanguage }
                    );

                    // Add the category to the xml builder.
                    xmlBuilder.Append(string.Format(
                        "<TaxonomyCategory Id=\"{0}\" Name=\"{1}\" Label=\"{2}\"></TaxonomyCategory>",
                        (Guid)category[0],
                        HttpUtility.HtmlEncode((string)category[1]),
                        HttpUtility.HtmlEncode(categoryLabel)
                    ));
                }

                xmlBuilder.Append("</TaxonomyCategories>");

                xmlBuilder.Append("</TaxonomyVariable>");
            }

            xmlBuilder.Append("</TaxonomyVariables>");

            xmlBuilder.Append("<Variables>");
            /*
            // Get all variables of the client.
            List<object[]> variables = Global.Core.Variables.GetValues(
                new string[] { "Id", "Name", "IdStudy" }
            );

            // Run through all variables of the client.
            foreach (object[] variable in variables)
            {
                // Get the label of the variable in the requested language.
                string label = (string)Global.Core.VariableLabels.GetValue(
                    "Label",
                    new string[] { "IdVariable", "IdLanguage" },
                    new object[] { (Guid)variable[0], idLanguage }
                );

                string studyName = (string)Global.Core.Studies.GetValue(
                    "Name",
                    "Id",
                    (Guid)variable[2]
                );

                // Add the variable to the xml builder.
                xmlBuilder.Append(string.Format(
                    "<Variable Id=\"{0}\" Name=\"{1}\" Label=\"{2}\" Study=\"{3}\">",
                    (Guid)variable[0],
                    HttpUtility.HtmlEncode((string)variable[1]),
                    HttpUtility.HtmlEncode(label),
                    HttpUtility.HtmlEncode(studyName)
                ));

                // Get all categories of the variable.
                List<object[]> categories = Global.Core.Categories.GetValues(
                    new string[] { "Id", "Name" },
                    new string[] { "IdVariable" },
                    new object[] { (Guid)variable[0] }
                );

                // Run through all categories of the variable.
                foreach (object[] category in categories)
                {
                    // Get the label of the taxonomy variable in the requested language.
                    string categoryLabel = (string)Global.Core.CategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdCategory", "IdLanguage" },
                        new object[] { (Guid)category[0], idLanguage }
                    );

                    // Add the category to the xml builder.
                    xmlBuilder.Append(string.Format(
                        "<Category Id=\"{0}\" Name=\"{1}\" Label=\"{2}\"></Category>",
                        (Guid)category[0],
                        HttpUtility.HtmlEncode((string)category[1]),
                        HttpUtility.HtmlEncode(categoryLabel)
                    ));
                }

                xmlBuilder.Append("</Variable>");
            }
            */
            xmlBuilder.Append("</Variables>");

            xmlBuilder.Append("</Response>");

            string responseString;

            // Check if the response type is JSON.
            if (responseType == LinkBiExternalResponseType.JSON)
            {
                XmlDocument result = new XmlDocument();
                result.LoadXml(xmlBuilder.ToString());

                /*responseString = "{" + result.DocumentElement.SelectSingleNode("TaxonomyVariables").ToJson() + "," + 
                    result.DocumentElement.SelectSingleNode("Variables").ToJson() + "}";*/
                responseString = "{" + result.DocumentElement.SelectSingleNode("TaxonomyVariables").ToJson() + "}";
            }
            else
            {
                responseString = xmlBuilder.ToString();
            }

            context.Response.Write(responseString);
        }

        public void ProcessReport(HttpContext context, XmlDocument xmlDocument)
        {
            //string tempFileName = Path.GetTempFileName() + ".xml";
            string tempFileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                context.Session.SessionID,
                Guid.NewGuid() + ".xml"
            );

            if (!Directory.Exists(Path.GetDirectoryName(tempFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(tempFileName));

            LinkBiExternalResponseType responseType = LinkBiExternalResponseType.XML;

            if (xmlDocument.DocumentElement.SelectSingleNode("ResponseType") != null)
            {
                // Get the response type from the request xml document.
                responseType = (LinkBiExternalResponseType)Enum.Parse(
                    typeof(LinkBiExternalResponseType),
                    xmlDocument.DocumentElement.SelectSingleNode("ResponseType").InnerText
                );
            }
            else if (xmlDocument.DocumentElement.Attributes["ResponseType"] != null)
            {
                // Get the response type from the request xml document.
                responseType = (LinkBiExternalResponseType)Enum.Parse(
                    typeof(LinkBiExternalResponseType),
                    xmlDocument.DocumentElement.Attributes["ResponseType"].Value
                );
            }

            // Select all taxonomy variable xml nodes from the request xml document.
            XmlNodeList xmlNodes = xmlDocument.SelectNodes("//Variable");

            // Run through all taxonomy variable xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                Guid id;

                // Check if a id is set for the taxonomy variable.
                if (xmlNode.Attributes["Id"] == null)
                {
                    object _id = Global.Core.TaxonomyVariables.GetValue(
                        "Id",
                        "Name",
                        xmlNode.Attributes["Name"].Value
                    );

                    if (_id == null)
                    {
                        throw new Exception(string.Format(
                            "A taxonomy variable with the name '{0}' does not exist.",
                            xmlNode.Attributes["Name"].Value
                        ));
                    }

                    id = (Guid)_id;

                    xmlNode.AddAttribute("Id", id.ToString());
                }
                else
                {
                    id = Guid.Parse(xmlNode.Attributes["Id"].Value);
                }

                if (xmlNode.Attributes["Name"] == null)
                {
                    string name = (string)Global.Core.TaxonomyVariables.GetValue(
                        "Name",
                        new string[] { "Id" },
                        new object[] { id }
                    );

                    xmlNode.AddAttribute("Name", name);
                }

                if (xmlNode.Attributes["Type"] == null)
                {
                    object type = Global.Core.TaxonomyVariables.GetValue(
                        "Type",
                        new string[] { "Id" },
                        new object[] { id }
                    );

                    if (type == null)
                    {
                        type = Global.Core.Variables.GetValue(
                            "Type",
                            new string[] { "Id" },
                            new object[] { id }
                        );
                    }

                    xmlNode.AddAttribute("Type", (int)type);
                }
            }

            // Select all taxonomy category xml nodes.
            xmlNodes = xmlDocument.SelectNodes("//TaxonomyCategory");

            int order = 0;
            // Run through all taxonomy category xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                if (xmlNode.Attributes["Order"] == null)
                {
                    xmlNode.AddAttribute("Order", order);
                }
                order++;

                // Check if a id is set for the taxonomy category.
                if (xmlNode.Attributes["Id"] == null)
                {
                    string variableName = "";

                    if (xmlNode.Attributes["VariableName"] != null)
                    {
                        variableName = xmlNode.Attributes["VariableName"].Value;
                    }
                    else
                    {
                        variableName = xmlNode.ParentNode.Attributes["Name"].Value;
                    }

                    object _idVariable = Global.Core.TaxonomyVariables.GetValue(
                        "Id",
                        "Name",
                        variableName
                    );

                    if (_idVariable == null)
                    {
                        throw new Exception(string.Format(
                            "A taxonomy variable with the name '{0}' does not exist.",
                            variableName
                        ));
                    }

                    Guid idVariable = (Guid)_idVariable;

                    object _idCategory = Global.Core.TaxonomyCategories.GetValue(
                        "Id",
                        new string[] { "IdTaxonomyVariable", "Name" },
                        new object[] { idVariable, xmlNode.Attributes["Name"].Value }
                    );

                    if (_idCategory == null)
                    {
                        throw new Exception(string.Format(
                            "A taxonomy category with the name '{0}' does not exist in the taxonomy variable '{1}'.",
                            xmlNode.Attributes["Name"].Value,
                            variableName
                        ));
                    }

                    xmlNode.AddAttribute("Id", ((Guid)_idCategory).ToString());
                }
            }

            xmlDocument.Save(tempFileName);

            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                xmlDocument,
                Global.HierarchyFilters[tempFileName]
            );

            string result = "";

            switch (responseType)
            {
                case LinkBiExternalResponseType.XML:

                    LinkBi1.Classes.Interfaces.XML xml = new LinkBi1.Classes.Interfaces.XML(Global.Core, definition);

                    string temp = xml.Render();
                    result = File.ReadAllText(temp);

                    File.Delete(temp);

                    break;
                case LinkBiExternalResponseType.JSON:

                    LinkBi1.Classes.Interfaces.JSON json = new LinkBi1.Classes.Interfaces.JSON(Global.Core, definition);
                    result = json.Render();

                    break;
                case LinkBiExternalResponseType.CSV:

                    LinkBi1.Classes.Interfaces.CustomCharts csv = new LinkBi1.Classes.Interfaces.CustomCharts(Global.Core, definition);

                    string tempFileName2 = csv.Render();
                    result = File.ReadAllText(tempFileName2);

                    File.Delete(tempFileName2);

                    break;
                default:
                    break;
            }

            File.Delete(tempFileName);

            context.Response.Write(result);
        }

        #endregion


        #region JSON Methods

        private string JsonToXml(string json)
        {
            Dictionary<string, object> values = (Dictionary<string, object>)
                new JavaScriptSerializer().Deserialize(json, typeof(object));

            StringBuilder result = new StringBuilder();

            JsonToXml(values, result);

            return result.ToString();
        }

        private void JsonToXml(object value, StringBuilder writer)
        {
            switch (value.GetType().Name)
            {
                case "String":
                    writer.Append((string)value);
                    break;
                case "Dictionary`2":
                    Dictionary<string, object> values = (Dictionary<string, object>)value;

                    foreach (string key in values.Keys)
                    {
                        object v = values[key];

                        writer.Append(string.Format(
                            "<{0}",
                            key
                        ));

                        v = WriteAttributes(v, writer);

                        writer.Append(">");

                        JsonToXml(v, writer);

                        writer.Append(string.Format(
                            "</{0}>",
                            key
                        ));

                        /*result.Append(string.Format(
                            "<{0}>{1}</{0}>",
                            key,
                            JsonToXml(values[key])
                        ));*/
                    }
                    break;
                case "Object[]":

                    object[] values2 = (object[])value;

                    foreach (object item in values2)
                    {
                        JsonToXml(item, writer);
                    }

                    break;
            }
        }

        private object WriteAttributes(object value, StringBuilder writer)
        {
            if (value.GetType().Name != "Dictionary`2")
                return value;

            Dictionary<string, object> values = (Dictionary<string, object>)value;

            List<string> attributes = new List<string>();

            foreach (string key in values.Keys)
            {
                if (values[key].GetType().Name != "String")
                    continue;

                writer.Append(string.Format(
                    " {0}=\"{1}\"",
                    key,
                    (string)values[key]
                ));

                attributes.Add(key);
            }

            foreach (string attribute in attributes)
            {
                values.Remove(attribute);
            }

            return values;
        }

        #endregion


        #region Web Methods

        private void Authenticate(HttpContext context)
        {
            bool isEncrypted = true;

            if (context.Request.Params["IsEncrypted"] != null)
                isEncrypted = bool.Parse(context.Request.Params["IsEncrypted"]);

            Authenticate(context, isEncrypted);

            if (context.Request.Params["RedirectUrl"] != null)
            {
                context.Response.Redirect(context.Request.Params["RedirectUrl"]);
            }
        }

        private void IsAuthenticated(HttpContext context)
        {
            context.Response.Write(Global.IdUser.HasValue);
        }

        private void HasPermission(HttpContext context)
        {
            string permission = context.Request.Params["Permission"];

            // Check if the permission exists.
            if (Global.PermissionCore.Permissions[permission] != null)
            {
                // Check if the user has access
                // granted to the comment section.
                if (Global.Core.Users.HasPermission(
                    Global.IdUser.Value,
                    Global.PermissionCore.Permissions[permission].Id
                ) == false)
                {
                    context.Response.Write("0");
                    return;
                }
            }

            context.Response.Write("1");
        }

        private void ForgotPassword(HttpContext context)
        {
            if (Global.Core == null)
            {
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

                /*if (!Directory.Exists(Global.Core.FileStorageRoot))
                    Directory.CreateDirectory(Global.Core.FileStorageRoot);

                Global.Core.LogDirectory = ConfigurationManager.AppSettings["DatabaseChangeLogDirectory"];*/

                Global.Core.ClientName = clientName;
                Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(clientName).CaseDataVersion;
                Global.Core.CaseDataLocation = Global.ClientManager.GetSingle(clientName).CaseDataLocation;
            }

            User user = Global.Core.Users.GetSingle("Mail", context.Request.Params["Mail"]);
            if (user != null)
            {
                //Global.User = user;
                var pwdResetId = Guid.NewGuid();
                user.SetValue("PasswordReset", pwdResetId);
                user.SetValue("PwdCreated", DateTime.Now);
                user.Save();

                //string password = GeneratePassword(); //Membership.GeneratePassword(8, 2);

                //user.Password = Global.Core.Users.GetMD5Hash(password);
                //user.Browser = Request.Browser.Browser;
                //user.Save();

                // configuration values from the web.config file.
                MailConfiguration mailConfiguration = new MailConfiguration(true);
                // Create a new mail by the mail configuration.
                Mail mail = new Mail(mailConfiguration, Global.Core.ClientName)
                {
                    TemplatePath = Path.Combine(
                        context.Request.PhysicalApplicationPath,
                        "App_Data",
                        "MailTemplates",
                        Global.Language.ToString(),
                        "ForgotPassword.html"
                    ),
                    Subject = Global.LanguageManager.GetText("ForgotSubject")
                };


                string resetLink = "http://" + context.Request.Url.ToString().Split('/')[2] + "/Pages/ResetPassword.aspx?arb=" + pwdResetId;

                // Add the placeholder value for the reset link.
                mail.Placeholders.Add("imagepath", "http://" + context.Request.Url.ToString().Split('/')[2] + "/Images/Logos/link.png");
                mail.Placeholders.Add("RequestURL", resetLink);
                // Send the mail.
                mail.Send(context.Request.Params["Mail"]);

                context.Response.Write("True");
            }
            else
            {
                context.Response.Write("False");
            }
        }

        private void GetStudies(HttpContext context)
        {
            Guid? idHierarchy = null;

            // Parse the id of the hierarchy
            // from the http request's parameters.
            if (context.Request.Params["IdHierarchy"] != null)
            {
                idHierarchy = Guid.Parse(context.Request.Params["IdHierarchy"]);
            }

            List<object[]> studies;

            if (idHierarchy.HasValue)
            {
                studies = Global.Core.Studies.GetValues(
                    new string[] { "Id", "Name" },
                    new string[] { "IdHierarchy" },
                    new object[] { idHierarchy.Value }
                );
            }
            else
            {
                studies = Global.Core.Studies.GetValues(
                    new string[] { "Id", "Name" },
                    new string[] { },
                    new object[] { }
                );
            }

            // Create a new string builder that
            // contains the result JSON string.
            StringBuilder result = new StringBuilder();
            result.Append("[");

            foreach (object[] study in studies)
            {
                result.Append("{");

                result.Append(string.Format(
                    "\"Id\": \"{0}\", \"Name\": \"{1}\"",
                    study[0],
                    study[1]
                ));

                result.Append("},");
            }

            if (studies.Count != 0)
                result = result.Remove(result.Length - 1, 1);

            result.Append("]");

            context.Response.Write(result.ToString());

            result.Clear();
        }

        private void GetVariables(HttpContext context)
        {
            // Create a new string builder that stores the result JSON string.
            StringBuilder result = new StringBuilder();

            int idLanguage = 2057;

            // Check if a language id is applied.
            if (context.Request.Params["IdLanguage"] != null)
            {
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);
            }

            result.Append("[");

            if (context.Request.Params["IdStudy"] != null)
            {
                // Get all taxonomy variables of the chapter.
                List<object[]> variables = Global.Core.Variables.GetValues(
                    new string[] { "Id", "Name", "Type" },
                    new string[] { "IdStudy" },
                    new object[] { Guid.Parse(context.Request.Params["IdStudy"]) },
                    "Order"
                );

                Dictionary<Guid, List<object[]>> variableLabels = Global.Core.TaxonomyVariables.ExecuteReaderDict<Guid>(
                    "SELECT IdVariable, Label FROM VariableLabels WHERE IdLanguage={0}",
                    new object[] { idLanguage }
                );

                Dictionary<Guid, List<object[]>> variableLinks = Global.Core.TaxonomyVariables.ExecuteReaderDict<Guid>(
                    "SELECT IdVariable, IdTaxonomyVariable FROM VariableLinks",
                    new object[] { idLanguage }
                );

                Dictionary<Guid, List<object[]>> taxonomyVariables = Global.Core.TaxonomyVariables.ExecuteReaderDict<Guid>(
                    "SELECT Id, Name FROM TaxonomyVariables",
                    new object[] { }
                );

                object idTaxonomyVariable;
                foreach (object[] variable in variables)
                {
                    string label = string.Empty;

                    if (variableLabels.ContainsKey((Guid)variable[0]))
                        label = (string)variableLabels[(Guid)variable[0]][0][1];

                    /*idTaxonomyVariable = Global.Core.VariableLinks.GetValue(
                        "IdTaxonomyVariable",
                        "IdVariable",
                        variable[0]
                    );*/
                    idTaxonomyVariable = null;

                    if (variableLinks.ContainsKey((Guid)variable[0]))
                        idTaxonomyVariable = variableLinks[(Guid)variable[0]][0][1];

                    string[] names = new string[]{
                        "Id",
                        "Name",
                        "Label",
                        "Type",
                        "TaxonomyVariable"
                    };
                    object[] values = new object[]{
                        variable[0],
                        variable[1],
                        label,
                        (VariableType)((int)variable[2]),
                        idTaxonomyVariable != null &&
                        taxonomyVariables.ContainsKey((Guid)idTaxonomyVariable) ?
                        taxonomyVariables[(Guid)idTaxonomyVariable][0][1]: null
                    };

                    result.Append(this.ToJson(
                        names,
                        values
                    ));

                    result.Append(",");
                }
            }
            else
            {
                // Get all taxonomy chapters.
                List<object[]> taxonomyChapters = Global.Core.TaxonomyChapters.GetValues(
                    new string[] { "Id", "Name" },
                    new string[] { },
                    new object[] { }
                );

                Dictionary<Guid, List<object[]>> taxonomyVariableLabels = Global.Core.TaxonomyVariables.ExecuteReaderDict<Guid>(
                    "SELECT IdTaxonomyVariable, Label FROM TaxonomyVariableLabels WHERE IdLanguage={0}",
                    new object[] { idLanguage }
                );

                Dictionary<Guid, List<object[]>> taxonomyVariableHierarchies = Global.Core.TaxonomyVariables.ExecuteReaderDict<Guid>(
                    "SELECT IdTaxonomyVariable, IdHierarchy FROM TaxonomyVariableHierarchies",
                    new object[] { }
                );

                // Run through all taxonomy chapters.
                foreach (object[] taxonomyChapter in taxonomyChapters)
                {
                    // Get all taxonomy variables of the chapter.
                    List<object[]> taxonomyVariables = Global.Core.TaxonomyVariables.GetValues(
                        new string[] { "Id", "Name", "Type", "Scale", "Weight" },
                        new string[] { "IdTaxonomyChapter" },
                        new object[] { taxonomyChapter[0] },
                        "Order"
                    );

                    // Run through all taxonomy variables of the chapter.
                    foreach (object[] taxonomyVariable in taxonomyVariables)
                    {
                        string label = string.Empty;

                        if (taxonomyVariableLabels.ContainsKey((Guid)taxonomyVariable[0]))
                        {
                            label = (string)taxonomyVariableLabels[(Guid)taxonomyVariable[0]][0][1];
                        }

                        string[] names = new string[] {
                            "Id",
                            "Name",
                            "Label",
                            "Chapter",
                            "Type",
                            "Scale",
                            "Weight",
                            "IdHierarchy"
                        };
                        object[] values = new object[] {
                            taxonomyVariable[0],
                            taxonomyVariable[1],
                            label,
                            taxonomyChapter[1],
                            (VariableType)((int)taxonomyVariable[2]),
                            taxonomyVariable[3],
                            taxonomyVariable[4],
                            taxonomyVariableHierarchies.ContainsKey((Guid)taxonomyVariable[0]) ?
                            taxonomyVariableHierarchies[(Guid)taxonomyVariable[0]][0][1].ToString() : "undefined"
                        };

                        result.Append(this.ToJson(
                            names,
                            values
                        ));

                        result.Append(",");
                    }
                }
            }

            result.RemoveLastComma();

            result.Append("]");

            context.Response.Write(result);
        }

        private void GetVariable(HttpContext context)
        {
            // Create a new string builder that stores the result JSON string.
            StringBuilder result = new StringBuilder();

            object idVariable;

            if (context.Request.Params["IdVariable"] != null)
            {
                idVariable = Guid.Parse(context.Request.Params["IdVariable"]);
            }
            else
            {
                if (context.Request.Params["IdStudy"] != null)
                {
                    idVariable = Global.Core.Variables.GetValue(
                        "Id",
                        new string[] { "IdStudy", "Name" },
                        new object[] {
                            Guid.Parse(context.Request.Params["IdStudy"]),
                            context.Request.Params["VariableName"]
                        }
                    );
                }
                else
                {
                    idVariable = Global.Core.Variables.GetValue(
                        "Id",
                        new string[] { "Name", "IdStudy" },
                        new object[] { context.Request.Params["VariableName"],
                            Guid.Parse(context.Request.Params["IdStudy"]) }
                    );
                }
            }

            if (idVariable == null)
            {
                context.Response.Write("{}");
                return;
            }

            int idLanguage = 2057;

            // Check if a language id is applied.
            if (context.Request.Params["IdLanguage"] != null)
            {
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);
            }

            if (context.Request.Params["IdStudy"] != null)
            {
                // Get all taxonomy variables of the chapter.
                List<object[]> variables = Global.Core.Variables.GetValues(
                    new string[] { "Id", "Name", "Type" },
                    new string[] { "Id" },
                    new object[] { idVariable },
                    "Order"
                );

                foreach (object[] variable in variables)
                {
                    string label = (string)Global.Core.VariableLabels.GetValue(
                        "Label",
                        new string[] { "IdVariable", "IdLanguage" },
                        new object[] { variable[0], idLanguage }
                    );

                    string[] names = new string[]{
                        "Id",
                        "Name",
                        "Label",
                        "Type"
                    };
                    object[] values = new object[]{
                        variable[0],
                        variable[1],
                        label,
                        (VariableType)((int)variable[2])
                    };

                    result.Append(this.ToJson(
                        names,
                        values
                    ));
                }
            }
            else
            {
                // Get all taxonomy variables of the chapter.
                List<object[]> taxonomyVariables = Global.Core.TaxonomyVariables.GetValues(
                    new string[] { "Id", "Name", "Type", "Scale", "Weight", "IdTaxonomyChapter" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { idVariable },
                    "Order"
                );

                // Run through all taxonomy variables of the chapter.
                foreach (object[] taxonomyVariable in taxonomyVariables)
                {
                    string label = (string)Global.Core.TaxonomyVariableLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyVariable", "IdLanguage" },
                        new object[] { taxonomyVariable[0], idLanguage }
                    );

                    string[] names = new string[]{
                        "Id",
                        "Name",
                        "Label",
                        "Chapter",
                        "Type",
                        "Scale",
                        "Weight"
                    };
                    object[] values = new object[]{
                        taxonomyVariable[0],
                        taxonomyVariable[1],
                        label,
                        taxonomyVariable[5],
                        (VariableType)((int)taxonomyVariable[2]),
                        taxonomyVariable[3],
                        taxonomyVariable[4]
                    };

                    result.Append(this.ToJson(
                        names,
                        values
                    ));
                }
            }

            context.Response.Write(result);
        }

        private void GetCategories(HttpContext context)
        {
            Guid idVariable;

            // Set default language to en-GB.
            int idLanguage = 2057;

            // Check if a language id is applied.
            if (context.Request.Params["IdLanguage"] != null)
            {
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);
            }

            // Create a new string builder that contains the result JSON string.
            StringBuilder result = new StringBuilder();

            if (context.Request.Params["Variables"] != null)
            {
                result.Append("{");

                string[] variables = context.Request.Params["Variables"].Split(',');

                Dictionary<string, List<object[]>> taxonomyVariables = null;
                Dictionary<Guid, List<object[]>> taxonomyCategoryLabels = null;


                if (context.Request.Params["IdStudy"] == null)
                {
                    taxonomyVariables = Global.Core.TaxonomyVariables.ExecuteReaderDict<string>(
                        "SELECT Name, Id FROM TaxonomyVariables",
                        new object[] { }
                    );
                    taxonomyCategoryLabels = Global.Core.TaxonomyCategoryLabels.ExecuteReaderDict<Guid>(
                        "SELECT IdTaxonomyCategory, Label FROM TaxonomyCategoryLabels WHERE IdLanguage={0}",
                        new object[] { idLanguage }
                    );
                }

                foreach (string variable in variables)
                {
                    if (context.Request.Params["IdStudy"] == null)
                    {
                        /*idVariable = (Guid)Global.Core.TaxonomyVariables.GetValue(
                            "Id",
                            "Name",
                            variable
                        );*/
                        if (!taxonomyVariables.ContainsKey(variable))
                            continue;

                        idVariable = (Guid)taxonomyVariables[variable][0][1];
                    }
                    else
                    {
                        idVariable = (Guid)Global.Core.Variables.GetValue(
                            "Id",
                            new string[] { "IdStudy", "Name" },
                            new object[] { Guid.Parse(context.Request.Params["IdStudy"]),
                            variable }
                        );
                    }

                    result.Append("\"" + variable + "\": ");

                    result.Append("[");

                    if (context.Request.Params["IdStudy"] != null)
                    {
                        RenderCategories(
                            idVariable,
                            idLanguage,
                            result
                        );
                    }
                    else
                    {
                        RenderTaxonomyCategories(
                            idVariable,
                            idLanguage,
                            result,
                            taxonomyCategoryLabels
                        );
                    }

                    result.RemoveLastComma();

                    result.Append("],");
                }

                result.RemoveLastComma();

                result.Append("}");
            }
            else
            {
                if (context.Request.Params["IdVariable"] != null)
                {
                    // Parse the id of the variables to get the
                    // categories for from the http request's parameters.
                    idVariable = Guid.Parse(
                        context.Request.Params["IdVariable"]
                    );
                }
                else
                {
                    if (context.Request.Params["IdStudy"] == null)
                    {
                        idVariable = (Guid)Global.Core.TaxonomyVariables.GetValue(
                            "Id",
                            "Name",
                            context.Request.Params["VariableName"]
                        );
                    }
                    else
                    {
                        idVariable = (Guid)Global.Core.Variables.GetValue(
                            "Id",
                            new string[] { "IdStudy", "Name" },
                            new object[] { Guid.Parse(context.Request.Params["IdStudy"]),
                            context.Request.Params["VariableName"] }
                        );
                    }
                }

                result.Append("[");

                if (context.Request.Params["IdStudy"] != null)
                {
                    RenderCategories(
                        idVariable,
                        idLanguage,
                        result
                    );
                }
                else
                {
                    RenderTaxonomyCategories(
                        idVariable,
                        idLanguage,
                        result,
                        null
                    );
                }

                result.RemoveLastComma();

                result.Append("]");
            }

            context.Response.Write(result.ToString());
        }

        private void GetVariableLabels(HttpContext context)
        {
            // Get the names of the variables
            // from the http request's parameters.
            string[] variables = context.Request.Params["Variables"].Split(',');

            if (variables.Length == 0)
            {
                context.Response.Write("{}");
                return;
            }

            int idLanguage = 2057;

            if (context.Request.Params["IdLanguage"] != null)
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);

            // Create a new string builder that
            // contains the result JSON string.
            StringBuilder result = new StringBuilder();

            result.Append("{");

            string parameters = "";

            for (int i = 0; i < variables.Length; i++)
            {
                parameters += "{" + i + "},";
            }

            parameters = parameters.Remove(parameters.Length - 1, 1);

            Dictionary<string, List<object[]>> labels = Global.Core.TaxonomyVariableLabels.ExecuteReaderDict<string>(
                "SELECT Name, Label FROM TaxonomyVariableLabels INNER JOIN TaxonomyVariables ON IdTaxonomyVariable=TaxonomyVariables.Id WHERE Name IN (" + parameters + ") AND IdLanguage=" + idLanguage,
                variables
            );

            // Run through all variables.
            foreach (string variable in labels.Keys)
            {
                result.Append(string.Format(
                    "\"{0}\": \"{1}\",",
                    variable,
                    labels[variable].First()[1]
                ));
            }

            result.RemoveLastComma();
            result.Append("}");

            context.Response.Write(result.ToString());
            result.Clear();
        }

        private void GetVariableLinks(HttpContext context)
        {
            Dictionary<Guid, List<object[]>> items;

            if (context.Request.Params["IdStudy"] != null)
            {
                items = Global.Core.VariableLinks.ExecuteReaderDict<Guid>(
                    "SELECT IdTaxonomyVariable, IdVariable FROM VariableLinks WHERE IdVariable IN " +
                    "(SELECT Id FROM Variables WHERE IdStudy={0})",
                    new object[] { Guid.Parse(context.Request.Params["IdStudy"]) }
                );
            }
            else
            {
                items = Global.Core.VariableLinks.ExecuteReaderDict<Guid>(
                    "SELECT IdTaxonomyVariable, IdVariable FROM VariableLinks",
                    new object[] { }
                );
            }

            StringBuilder result = new StringBuilder();

            result.Append("{ \"Keys\": [");

            foreach (Guid idTaxonomyVariable in items.Keys)
                result.Append(string.Format("\"{0}\",", idTaxonomyVariable));

            result.RemoveLastComma();

            result.Append("],");

            foreach (Guid idTaxonomyVariable in items.Keys)
            {
                result.Append(string.Format(
                    "\"{0}\": [",
                    idTaxonomyVariable
                ));

                foreach (object[] variable in items[idTaxonomyVariable])
                {
                    result.Append(string.Format(
                        "\"{0}\",",
                        variable[1]
                    ));
                }

                result.RemoveLastComma();
                result.Append("],");
            }

            result.RemoveLastComma();
            result.Append("}");

            context.Response.Write(result.ToString());
            result.Clear();
        }


        private void CreateStudy(HttpContext context)
        {
            // Get the name of the study to create
            // from the http request's parameters.
            string name = context.Request.Params["Name"];

            // Parse the id of the hierarchy
            // from the http request's parameters.
            Guid idHierarchy = Guid.Parse(context.Request.Params["IdHierarchy"]);

            Study study = new Study(Global.Core.Studies);
            study.Name = name;
            study.CreationDate = DateTime.Now;
            study.IdHierarchy = idHierarchy;
            study.IdUser = Global.IdUser.Value;

            study.Insert();

            context.Response.Write(study.Id.ToString());
        }
        private void CreateVariable(HttpContext context)
        {
            // Parse the id of the study from
            // the http request's parameters.
            Guid idStudy = Guid.Parse(context.Request.Params["IdStudy"]);

            // Get the name of the variable to create
            // from the http request's parameters.
            string name = context.Request.Params["Name"];

            // Get the label of the variable to create
            // from the http request's parameters.
            string label = context.Request.Params["Label"];

            if (string.IsNullOrEmpty(label))
                label = name;

            // Parse the type of the variable to create
            // from the http request's parameters.
            VariableType variableType = (VariableType)Enum.Parse(
                typeof(VariableType),
                context.Request.Params["Type"]
            );

            Variable variable = new Variable(Global.Core.Variables);
            variable.IdStudy = idStudy;
            variable.Name = name;
            variable.Type = variableType;

            variable.Insert();

            VariableLabel variableLabel = new VariableLabel(Global.Core.VariableLabels);
            variableLabel.IdVariable = variable.Id;
            variableLabel.IdLanguage = 2057;
            variableLabel.Label = label;

            variableLabel.Insert();

            CreateResponsesTable(variable.Id);

            context.Response.Write(variable.Id.ToString());
        }
        private void CreateTaxonomyVariable(HttpContext context)
        {
            // Get the name of the taxonomyVariable to create
            // from the http request's parameters.
            string name = context.Request.Params["Name"];

            object id = Global.Core.TaxonomyVariables.GetValue("Id", "Name", name);
            if (id != null)
            {
                context.Response.Write(id.ToString());
                return;
            }

            // Get the label of the taxonomyVariable to create
            // from the http request's parameters.
            string label = context.Request.Params["Label"];

            if (string.IsNullOrEmpty(label))
                label = name;

            // Parse the type of the taxonomyVariable to create
            // from the http request's parameters.
            VariableType taxonomyVariableType = (VariableType)Enum.Parse(
                typeof(VariableType),
                context.Request.Params["Type"]
            );

            TaxonomyVariable taxonomyVariable = new TaxonomyVariable(Global.Core.TaxonomyVariables);
            taxonomyVariable.Name = name;
            taxonomyVariable.Type = taxonomyVariableType;
            taxonomyVariable.SetValue("CreationDate", DateTime.Now);

            taxonomyVariable.Insert();

            TaxonomyVariableLabel taxonomyVariableLabel = new TaxonomyVariableLabel(Global.Core.TaxonomyVariableLabels);
            taxonomyVariableLabel.IdTaxonomyVariable = taxonomyVariable.Id;
            taxonomyVariableLabel.IdLanguage = 2057;
            taxonomyVariableLabel.Label = label;
            taxonomyVariableLabel.SetValue("CreationDate", DateTime.Now);

            taxonomyVariableLabel.Insert();

            if (context.Request.Params["IdHierarchy"] != null)
            {
                TaxonomyVariableHierarchy taxonomyVariableHierarchy = new TaxonomyVariableHierarchy(Global.Core.TaxonomyVariableHierarchies);
                taxonomyVariableHierarchy.IdTaxonomyVariable = taxonomyVariable.Id;
                taxonomyVariableHierarchy.IdHierarchy = Guid.Parse(context.Request.Params["IdHierarchy"]);

                taxonomyVariableHierarchy.Insert();
            }

            context.Response.Write(taxonomyVariable.Id.ToString());
        }
        private void CreateCategory(HttpContext context)
        {
            // Parse the id of the variable from
            // the http request's parameters.
            Guid idVariable = Guid.Parse(context.Request.Params["IdVariable"]);

            bool isTaxonomy = bool.Parse(context.Request.Params["IsTaxonomy"]);

            Guid? idHierarchy = null;

            if (context.Request.Params["IdHierarchy"] != null)
            {
                idHierarchy = Guid.Parse(context.Request.Params["IdHierarchy"]);
            }

            // Get the name of the category to create
            // from the http request's parameters.
            string name = context.Request.Params["Name"];

            // Get the label of the category to create
            // from the http request's parameters.
            string label = context.Request.Params["Label"];

            if (string.IsNullOrEmpty(label))
                label = name;

            if (isTaxonomy)
            {
                TaxonomyCategory taxonomyCategory = new TaxonomyCategory(Global.Core.TaxonomyCategories);
                taxonomyCategory.IdTaxonomyVariable = idVariable;
                taxonomyCategory.Name = name;

                taxonomyCategory.Insert();

                TaxonomyCategoryLabel taxonomyCategoryLabel = new TaxonomyCategoryLabel(Global.Core.TaxonomyCategoryLabels);
                taxonomyCategoryLabel.IdTaxonomyCategory = taxonomyCategory.Id;
                taxonomyCategoryLabel.IdLanguage = 2057;
                taxonomyCategoryLabel.Label = label;

                taxonomyCategoryLabel.Insert();

                if (idHierarchy.HasValue)
                {
                    TaxonomyCategoryHierarchy taxonomyCategoryHierarchy = new TaxonomyCategoryHierarchy(Global.Core.TaxonomyCategoryHierarchies);
                    taxonomyCategoryHierarchy.IdHierarchy = idHierarchy.Value;
                    taxonomyCategoryHierarchy.IdTaxonomyCategory = taxonomyCategory.Id;

                    taxonomyCategoryHierarchy.Insert();
                }

                context.Response.Write(taxonomyCategory.Id.ToString());
            }
            else
            {
                Category category = new Category(Global.Core.Categories);
                category.IdVariable = idVariable;
                category.Name = name;

                category.Insert();

                CategoryLabel categoryLabel = new CategoryLabel(Global.Core.CategoryLabels);
                categoryLabel.IdCategory = category.Id;
                categoryLabel.IdLanguage = 2057;
                categoryLabel.Label = label;

                categoryLabel.Insert();

                context.Response.Write(category.Id.ToString());
            }
        }
        private void LinkVariable(HttpContext context)
        {
            // Parse the id of the study from
            // the http request's parameters.
            Guid idStudy = Guid.Parse(context.Request.Params["IdStudy"]);

            // Get the name of the study variable
            // from the http request's parameters.
            string variableName = context.Request.Params["StudyVariable"];

            // Get the name of the taxonomy variable
            // from the http request's parameters.
            string taxonomyVariableName = context.Request.Params["TaxonomyVariable"];

            Guid idVariable = (Guid)Global.Core.Variables.GetValue(
                "Id",
                new string[] { "Name", "IdStudy" },
                new object[] { variableName, idStudy }
            );

            Guid idTaxonomyVariable = (Guid)Global.Core.TaxonomyVariables.GetValue("Id", "Name", taxonomyVariableName);

            Global.Core.VariableLinks.ExecuteQuery(string.Format(
                "DELETE FROM [VariableLinks] WHERE [IdVariable]='{0}';",
                idVariable
            ));

            VariableLink variableLink = new VariableLink(Global.Core.VariableLinks);
            variableLink.IdVariable = idVariable;
            variableLink.IdTaxonomyVariable = idTaxonomyVariable;
            variableLink.SetValue("QA", "1");
            variableLink.CreationDate = DateTime.Now;

            variableLink.Insert();
        }
        private void LinkCategory(HttpContext context)
        {
            // Parse the id of the study from
            // the http request's parameters.
            Guid idVariable = Guid.Parse(context.Request.Params["IdVariable"]);

            Guid idCategory;

            if (context.Request.Params["StudyCategory"] != null)
            {
                idCategory = Guid.Parse(context.Request.Params["StudyCategory"]);
            }
            else
            {
                idCategory = (Guid)Global.Core.Categories.GetValue(
                    "Id",
                    new string[] { "IdVariable", "Name" },
                    new object[] { idVariable, context.Request.Params["StudyCategoryName"] }
                );
            }

            // Get the name of the taxonomy category
            // from the http request's parameters.
            Guid idTaxonomyCategory;

            if (context.Request.Params["TaxonomyCategoryName"] != null)
            {
                if (context.Request.Params["TaxonomyVariable"] != null)
                {
                    idTaxonomyCategory = (Guid)Global.Core.TaxonomyCategories.GetValue(
                        "Id",
                        new string[] { "Name", "IdTaxonomyVariable" },
                        new object[] {
                            context.Request.Params["TaxonomyCategoryName"],
                            Guid.Parse(context.Request.Params["TaxonomyVariable"])
                        }
                    );
                }
                else if (context.Request.Params["TaxonomyVariableName"] != null)
                {
                    idTaxonomyCategory = (Guid)Global.Core.TaxonomyCategories.GetValue(
                        "Id",
                        new string[] { "Name", "IdTaxonomyVariable" },
                        new object[] {
                            context.Request.Params["TaxonomyCategoryName"],
                            (Guid)Global.Core.TaxonomyVariables.GetValue(
                                "Id",
                                "Name",
                                context.Request.Params["TaxonomyVariableName"]
                            )
                        }
                    );
                }
                else
                {
                    idTaxonomyCategory = (Guid)Global.Core.TaxonomyCategories.GetValue(
                        "Id",
                        "Name",
                        context.Request.Params["TaxonomyCategoryName"]
                    );
                }
            }
            else
            {
                idTaxonomyCategory = Guid.Parse(context.Request.Params["TaxonomyCategory"]);
            }

            Guid idTaxonomyVariable = (Guid)Global.Core.TaxonomyCategories.GetValue(
                "IdTaxonomyVariable",
                "Id",
                idTaxonomyCategory
            );

            Global.Core.CategoryLinks.ExecuteQuery(string.Format(
                "DELETE FROM [CategoryLinks] WHERE [IdCategory]='{0}';",
                idCategory
            ));

            CategoryLink categoryLink = new CategoryLink(Global.Core.CategoryLinks);
            categoryLink.IdCategory = idCategory;
            categoryLink.IdTaxonomyCategory = idTaxonomyCategory;
            categoryLink.IdVariable = idVariable;
            categoryLink.IdTaxonomyVariable = idTaxonomyVariable;
            categoryLink.SetValue("QA", "1");
            categoryLink.CreationDate = DateTime.Now;

            categoryLink.Insert();
        }
        private void ClearResponses(HttpContext context)
        {
            // Parse the id of the variable to clear the
            // data of from the http request's parameters.
            Guid idVariable = Guid.Parse(context.Request.Params["IdVariable"]);

            Global.Core.Responses[idVariable].ExecuteQuery(string.Format(
                "DELETE FROM [resp].[Var_{0}]",
                idVariable
            ));
        }
        private void InsertResponses(HttpContext context)
        {
            // Parse the id of the variable to insert the
            // data of from the http request's parameters.
            Guid idVariable = Guid.Parse(context.Request.Params["IdVariable"]);

            Guid idStudy = (Guid)Global.Core.Variables.GetValue("IdStudy", "Id", idVariable);

            object[] responses = (object[])new JavaScriptSerializer().
                Deserialize(context.Request.Params["Responses"], typeof(object));

            foreach (Dictionary<string, object> response in responses)
            {
                Response r = new Response(Global.Core.Responses[idVariable]);

                if (response.ContainsKey("IdRespondent"))
                {
                    r.IdRespondent = Guid.Parse((string)response["IdRespondent"]);

                    if (Global.Core.Respondents.GetValue("Id", "Id", r.IdRespondent) == null)
                    {
                        Respondent respondent = new Respondent(Global.Core.Respondents);
                        respondent.Id = r.IdRespondent;
                        respondent.IdStudy = idStudy;
                        respondent.OriginalRespondentID = respondent.Id.ToString();

                        respondent.Insert();
                    }
                }
                else
                {
                    Respondent respondent = new Respondent(Global.Core.Respondents);
                    respondent.IdStudy = idStudy;
                    respondent.OriginalRespondentID = respondent.Id.ToString();

                    respondent.Insert();

                    r.IdRespondent = respondent.Id;
                }

                r.IdStudy = idStudy;

                if (response.ContainsKey("IdCategory"))
                    r.IdCategory = Guid.Parse((string)response["IdCategory"]);
                else if (response.ContainsKey("NumericAnswer"))
                {
                    switch (response["NumericAnswer"].GetType().Name)
                    {
                        case "Int32":
                            r.NumericAnswer = (decimal)(int)response["NumericAnswer"];
                            break;
                        case "Double":
                            r.NumericAnswer = (decimal)(double)response["NumericAnswer"];
                            break;
                        case "Decimal":
                            r.NumericAnswer = (decimal)response["NumericAnswer"];
                            break;
                        default:
                            r.NumericAnswer = (decimal)double.Parse(response["NumericAnswer"].ToString());
                            break;
                    }
                }
                else if (response.ContainsKey("TextAnswer"))
                {
                    r.TextAnswer = response["TextAnswer"].ToString();
                }

                r.Insert();
            }
        }
        private void TransferResponses(HttpContext context)
        {
            // Parse the id of the variable to insert the
            // data of from the http request's parameters.
            Guid idVariable = Guid.Parse(context.Request.Params["IdVariable"]);

            Guid idStudy = (Guid)Global.Core.Variables.GetValue("IdStudy", "Id", idVariable);

            Dictionary<Guid, List<object[]>> respondents;

            if (context.Request.Params["FromIdVariable"] == null)
            {
                respondents = Global.Core.Respondents.GetValuesDict(
                    new string[] { "Id" },
                    new string[] { "IdStudy" },
                    new object[] { idStudy }
                );
            }
            else if (context.Request.Params["FromIdCategory"] == null)
            {
                respondents = Global.Core.Responses[Guid.Parse(context.Request.Params["FromIdVariable"])].GetValuesDict(
                    new string[] { "IdRespondent" },
                    new string[] { },
                    new object[] { }
                );
            }
            else
            {
                respondents = Global.Core.Responses[Guid.Parse(context.Request.Params["FromIdVariable"])].GetValuesDict(
                    new string[] { "IdRespondent" },
                    new string[] { "IdCategory" },
                    new object[] { Guid.Parse(context.Request.Params["FromIdCategory"]) }
                );
            }

            foreach (Guid idRespondent in respondents.Keys)
            {
                Response r = new Response(Global.Core.Responses[idVariable]);

                r.IdRespondent = idRespondent;

                r.IdStudy = idStudy;

                if (context.Request.Params["IdCategory"] != null)
                    r.IdCategory = Guid.Parse(context.Request.Params["IdCategory"]);
                else if (context.Request.Params["NumericAnswer"] != null)
                {
                    r.NumericAnswer = decimal.Parse(context.Request.Params["NumericAnswer"]);
                }
                else if (context.Request.Params["TextAnswer"] != null)
                {
                    r.TextAnswer = context.Request.Params["TextAnswer"];
                }

                r.Insert();
            }
        }
        private void GetRespondents(HttpContext context)
        {
            // Parse the id of the study from
            // the http request's parameters.
            Guid idStudy = Guid.Parse(context.Request.Params["IdStudy"]);

            Dictionary<Guid, List<object[]>> respondents;

            if (context.Request.Params["IdVariable"] == null)
            {
                respondents = Global.Core.Respondents.GetValuesDict(
                    new string[] { "Id" },
                    new string[] { "IdStudy" },
                    new object[] { idStudy }
                );
            }
            else if (context.Request.Params["IdCategory"] == null)
            {
                respondents = Global.Core.Responses[Guid.Parse(context.Request.Params["IdVariable"])].GetValuesDict(
                    new string[] { "IdRespondent" },
                    new string[] { },
                    new object[] { }
                );
            }
            else
            {
                respondents = Global.Core.Responses[Guid.Parse(context.Request.Params["IdVariable"])].GetValuesDict(
                    new string[] { "IdRespondent" },
                    new string[] { "IdCategory" },
                    new object[] { Guid.Parse(context.Request.Params["IdCategory"]) }
                );
            }

            StringBuilder result = new StringBuilder();
            result.Append("[");

            foreach (Guid idRespondent in respondents.Keys)
            {
                result.Append("{");
                result.Append(string.Format(
                    "\"IdRespondent\": \"{0}\"",
                    idRespondent
                ));
                result.Append("},");
            }

            result.RemoveLastComma();
            result.Append("]");
            context.Response.Write(result.ToString());
        }
        private void GetResponses(HttpContext context)
        {
            // Parse the id of the study from
            // the http request's parameters.
            Guid idStudy = Guid.Parse(context.Request.Params["IdStudy"]);

            Guid idVariable;

            if (context.Request.Params["IdVariable"] != null)
            {
                idVariable = Guid.Parse(context.Request.Params["IdVariable"]);
            }
            else
            {
                idVariable = (Guid)Global.Core.Variables.GetValue(
                    "Id",
                    new string[] { "Name", "IdStudy" },
                    new object[] { context.Request.Params["VariableName"], idStudy }
                );
            }

            Dictionary<Guid, List<object[]>> respondents;

            if (context.Request.Params["IdCategory"] == null)
            {
                respondents = Global.Core.Responses[idVariable].GetValuesDict(
                    new string[] { "IdRespondent", "IdCategory", "NumericAnswer", "TextAnswer" },
                    new string[] { },
                    new object[] { }
                );
            }
            else
            {
                respondents = Global.Core.Responses[idVariable].GetValuesDict(
                    new string[] { "IdRespondent", "IdCategory", "NumericAnswer", "TextAnswer" },
                    new string[] { "IdCategory" },
                    new object[] { Guid.Parse(context.Request.Params["IdCategory"]) }
                );
            }

            StringBuilder result = new StringBuilder();
            result.Append("[");

            foreach (Guid idRespondent in respondents.Keys)
            {
                result.Append("{");
                result.Append(string.Format(
                    "\"IdRespondent\": \"{0}\"",
                    idRespondent
                ));

                if (respondents[idRespondent][0][1] != null)
                {
                    result.Append(string.Format(
                        ",\"IdCategory\": \"{0}\"",
                        respondents[idRespondent][0][1]
                    ));
                }

                if (respondents[idRespondent][0][2] != null)
                {
                    result.Append(string.Format(
                        ",\"NumericAnswer\": {0}",
                        respondents[idRespondent][0][2]
                    ));
                }

                if (respondents[idRespondent][0][3] != null)
                {
                    result.Append(string.Format(
                        ",\"TextAnswer\": \"{0}\"",
                        respondents[idRespondent][0][3]
                    ));
                }

                result.Append("},");
            }

            result.RemoveLastComma();
            result.Append("]");
            context.Response.Write(result.ToString());
        }
        private void GenerateGuid(HttpContext context)
        {
            if (context.Request.Params["Count"] == null)
            {
                context.Response.Write(Guid.NewGuid().ToString());
                return;
            }

            StringBuilder result = new StringBuilder();
            result.Append("[");

            for (int i = 0; i < int.Parse(context.Request.Params["Count"]); i++)
            {
                result.Append(string.Format(
                    "\"{0}\",",
                    Guid.NewGuid()
                ));
            }

            result.RemoveLastComma();

            result.Append("]");

            context.Response.Write(result.ToString());
            result.Clear();
        }
        private void ClearCaseDataCache(HttpContext context)
        {
            DataCore.Classes.StorageMethods.Database database = new DataCore.Classes.StorageMethods.Database(
                Global.Core,
                null,
                1
            );

            database.ClearCaseDataCache();

            Global.Core.ClearCache();
        }

        private void GetNews(HttpContext context)
        {
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "News",
                Global.Core.ClientName + ".xml"
            );

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            StringBuilder result = new StringBuilder();

            result.Append("[");

            XmlNodeList xmlNodes = document.DocumentElement.SelectNodes("NewsList/News");

            foreach (XmlNode xmlNode in xmlNodes)
            {
                result.Append(this.ToJson(new string[] {
                    "Id",
                    "Heading",
                    "Description"
                }, new object[] {
                    xmlNode.Attributes["Id"].Value,
                    xmlNode.Attributes["Heading"].Value,
                    HttpUtility.HtmlDecode(xmlNode.Attributes["Description"].Value)
                }));

                result.Append(",");
            }

            result.RemoveLastComma();

            result.Append("]");

            context.Response.Write(result.ToString());
            result.Clear();
        }
        private void SaveNews(HttpContext context)
        {
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "News",
                Global.Core.ClientName + ".xml"
            );

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            XmlNode xmlNode = null;

            if (context.Request.Params["Id"] != null)
            {
                xmlNode = document.DocumentElement.SelectSingleNode(string.Format(
                    "NewsList/News[@Id=\"{0}\"]",
                    context.Request.Params["Id"]
                ));

                xmlNode.Attributes["Heading"].Value = context.Request.Params["Heading"];
                xmlNode.Attributes["Description"].Value = HttpUtility.HtmlEncode(context.Request.Params["Description"]);

                context.Response.Write(context.Request.Params["Id"]);
            }
            else
            {
                Guid id = Guid.NewGuid();

                document.DocumentElement.SelectSingleNode("NewsList").InnerXml += string.Format(
                    "<News Id=\"{0}\" Heading=\"{1}\" Description=\"{2}\" UserId=\"{3}\" CreatedDate=\"{4}\" />",
                    id,
                    context.Request.Params["Heading"],
                    HttpUtility.HtmlEncode(context.Request.Params["Description"]),
                    Global.IdUser.Value,
                    DateTime.Now.ToString()
                );

                context.Response.Write(id.ToString());
            }

            document.Save(fileName);
        }
        private void DeleteNews(HttpContext context)
        {
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "News",
                Global.Core.ClientName + ".xml"
            );

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            XmlNode xmlNode = document.DocumentElement.SelectSingleNode(string.Format(
                "NewsList/News[@Id=\"{0}\"]",
                context.Request.Params["Id"]
            ));

            xmlNode.ParentNode.RemoveChild(xmlNode);

            document.Save(fileName);
        }


        private void ProcessReport(HttpContext context)
        {
            LinkBiExternalResponseType responseType = LinkBiExternalResponseType.XML;

            if (context.Request.Params["ResponseType"] != null)
            {
                // Get the response type from the request xml document.
                responseType = (LinkBiExternalResponseType)Enum.Parse(
                    typeof(LinkBiExternalResponseType),
                    context.Request.Params["ResponseType"]
                );
            }

            int idLanguage = 2057;

            if (context.Request.Params["IdLanguage"] != null)
            {
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);
            }

            // Build the full path to the LinkBi definition template file.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "LinkBiDefinition.xml"
            );

            // Create a new xml document to build the LinkBi definition.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the LinkBi definition
            // template file into the xml document.
            xmlDocument.LoadXml(File.ReadAllText(fileName));

            if (context.Request.Params["Weight"] != null)
            {
                XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("WeightingVariables");

                if (xmlNode != null)
                {
                    object idWeight = Global.Core.TaxonomyVariables.GetValue(
                        "Id",
                        "Name",
                        context.Request.Params["Weight"]
                    );

                    // Check if the variable with the name exists.
                    if (idWeight == null)
                    {
                        throw new Exception(string.Format(
                            "A taxonomy variable with the name '{0}' doesn't exist.",
                            context.Request.Params["Weight"]
                        ));
                    }

                    if (xmlNode.Attributes["DefaultWeighting"] == null)
                        xmlNode.AddAttribute("DefaultWeighting", idWeight.ToString());
                    else
                        xmlNode.Attributes["DefaultWeighting"].Value = idWeight.ToString();
                }
            }

            if (context.Request.Params["Filter"] != null)
            {
                XmlNode xmlNodeFilter = xmlDocument.DocumentElement.SelectSingleNode("Filters/Operator");

                string[] filters = context.Request.Params["Filter"].Split(',');

                foreach (string filter in filters)
                {
                    Guid idCategory;

                    if (!Guid.TryParse(filter, out idCategory))
                        continue;

                    xmlNodeFilter.InnerXml += string.Format(
                        "<TaxonomyCategory Id=\"{0}\"></TaxonomyCategory>",
                        idCategory
                    );
                }
            }

            string[] fields = new string[] {
                "Measure",
                "Dimension"
            };

            // Run through all fields.
            foreach (string field in fields)
            {
                // Select the xml node that contains the dimensions.
                XmlNode xmlNodeField = xmlDocument.DocumentElement.SelectSingleNode(field + "s");

                // The counter that counts through the definied dimensions and measures.
                int i = 1;

                while (true)
                {
                    // Get the name of the variable from the http request's paramters.
                    string variableName = context.Request.Params[field + i++];

                    // Check if the dimension is defined.
                    if (variableName == null)
                        break;

                    // Get the id of the variable by the name.
                    object _idVariable = Global.Core.TaxonomyVariables.GetValue(
                        "Id",
                        "Name",
                        variableName
                    );

                    // Check if the variable with the name exists.
                    if (_idVariable == null)
                    {
                        throw new Exception(string.Format(
                            "A taxonomy variable with the name '{0}' doesn't exist.",
                            variableName
                        ));
                    }

                    Guid idVariable = (Guid)_idVariable;

                    string variableLabel = (string)Global.Core.TaxonomyVariableLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyVariable", "IdLanguage" },
                        new object[] { idVariable, idLanguage }
                    );

                    // Create a new string builder that build the xml string for the dimension definition.
                    StringBuilder xmlBuilder = new StringBuilder();

                    int variableType = (int)Global.Core.TaxonomyVariables.GetValue(
                        "Type",
                        "Id",
                        idVariable
                    );

                    // Add the dimension definition to the LinkBi definition xml document.
                    xmlBuilder.Append(string.Format(
                        "<Variable Id=\"{0}\" Name=\"{1}\" Label{4}=\"{2}\" Type=\"{3}\" Enabled=\"True\">",
                        idVariable,
                        variableName,
                        HttpUtility.HtmlEncode(variableLabel),
                        variableType.ToString(),
                        idLanguage
                    ));

                    if (variableType == 2)
                    {
                        DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                            Global.Core,
                            null,
                            1
                        );

                        List<string> categories = storageMethod.GetTextAnswers(
                            idVariable,
                            true,
                            null,
                            false
                        );

                        int j = 0;
                        // Run through all categories.
                        foreach (string category in categories)
                        {
                            if (category.Trim() == "")
                                continue;

                            // Add the xml tag for the category to the xml builder.
                            xmlBuilder.Append(string.Format(
                                "<{0} Id=\"{1}\" Order=\"{2}\" Value=\"{3}\" Enabled=\"{4}\" Text=\"{5}\" Label2057=\"{5}\"></{0}>",
                                "TaxonomyCategory",
                                Guid.NewGuid(),
                                j,
                                j++,
                                true,
                                HttpUtility.HtmlEncode(category)
                            ));
                        }
                    }
                    else
                    {
                        // Get all categories of the variable.
                        List<object[]> categories = Global.Core.TaxonomyCategories.GetValues(
                            new string[] { "Id", "Name", "IsScoreGroup", "Equation" },
                            new string[] { "IdTaxonomyVariable" },
                            new object[] { idVariable },
                            "Order"
                        );

                        int order = 0;
                        // Run through all categories of the variable.
                        foreach (object[] category in categories)
                        {
                            string categoryLabel = (string)Global.Core.TaxonomyCategoryLabels.GetValue(
                                "Label",
                                new string[] { "IdTaxonomyCategory", "IdLanguage" },
                                new object[] { (Guid)category[0], idLanguage }
                            );

                            if ((bool)category[2] == true)
                            {
                                xmlBuilder.Append(string.Format(
                                    "<ScoreGroup Id=\"{0}\" Persistent=\"True\" Name=\"{1}\" " +
                                    "Label{4}=\"{2}\" Order=\"{3}\" Equation=\"{5}\"></ScoreGroup>",
                                    (Guid)category[0],
                                    HttpUtility.HtmlEncode((string)category[1]),
                                    HttpUtility.HtmlEncode(categoryLabel),
                                    order++,
                                    idLanguage,
                                    category[3] != null ? HttpUtility.HtmlEncode((string)category[3]) : ""
                                ));
                            }
                            else
                            {
                                xmlBuilder.Append(string.Format(
                                    "<TaxonomyCategory Id=\"{0}\" Persistent=\"True\" Name=\"{1}\" " +
                                    "Label{4}=\"{2}\" Order=\"{3}\"></TaxonomyCategory>",
                                    (Guid)category[0],
                                    HttpUtility.HtmlEncode((string)category[1]),
                                    HttpUtility.HtmlEncode(categoryLabel),
                                    order++,
                                    idLanguage
                                ));
                            }
                        }
                    }

                    xmlBuilder.Append("</Variable>");

                    xmlNodeField.InnerXml += xmlBuilder.ToString();

                    xmlBuilder.Clear();
                }
            }

            string tempFile = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                Guid.NewGuid() + ".xml"
            );

            xmlDocument.Save(tempFile);

            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                tempFile,
                Global.HierarchyFilters.Default
            );

            string result = "";

            context.Response.ContentType = "text/plain";

            switch (responseType)
            {
                case LinkBiExternalResponseType.PowerBI:
                    string xmlString = File.ReadAllText(tempFile);
                    xmlString = xmlString.Replace("<Dimensions>", "<Variables Position=\"Left\">");
                    xmlString = xmlString.Replace("<Measures>", "<Variables Position=\"Top\">");
                    xmlString = xmlString.Replace("</Dimensions>", "</Variables>");
                    xmlString = xmlString.Replace("</Measures>", "</Variables>");
                    xmlString = xmlString.Replace("<Variable ", "<Variable IsTaxonomy=\"True\" ");

                    File.WriteAllText(tempFile, xmlString);

                    // Create a new report definition by the report definition file.
                    ReportDefinition reportDefinition = new ReportDefinition(
                        Global.Core,
                        tempFile,
                        Global.HierarchyFilters.Default
                    );

                    ReportCalculator aggregator = new ReportCalculator(
                        reportDefinition,
                        Global.Core,
                        HttpContext.Current.Session
                    );

                    aggregator.Aggregate("");

                    ReportDefinitionRenderer renderer = new ReportDefinitionRenderer(reportDefinition);

                    ReportDefinitionRenderConfiguration renderConfiguration;

                    if (reportDefinition.Settings.PowerBIValues == Crosstables.Classes.PowerBIValues.Values)
                        renderConfiguration = new ReportDefinitionRenderConfiguration(true, false, false, false);
                    else
                        renderConfiguration = new ReportDefinitionRenderConfiguration(false, true, false, false);

                    renderConfiguration.PowerBIExport = true;

                    context.Response.Write("<div id=\"Table" + (new FileInfo(fileName)).Name.Replace(".xml", "") + "\">");

                    context.Response.Write(renderer.Render(
                        100,
                        45,
                        renderConfiguration
                    ).Replace("<tr></tr>", "").Replace(">-</td>", ">0</td>").Replace("></td>", ">0</td>").Replace(" %", ""));

                    context.Response.Write("</div>");

                    context.Response.ContentType = "text/html";

                    return;

                    break;
                case LinkBiExternalResponseType.XML:

                    LinkBi1.Classes.Interfaces.XML xml = new LinkBi1.Classes.Interfaces.XML(Global.Core, definition);

                    string tempFileName = xml.Render();
                    result = File.ReadAllText(tempFileName);

                    File.Delete(tempFileName);

                    break;
                case LinkBiExternalResponseType.TABLE:

                    LinkBi1.Classes.Interfaces.XML xml2 = new LinkBi1.Classes.Interfaces.XML(Global.Core, definition);

                    string tempFileName2 = xml2.Render();

                    //result = RenderTable(tempFileName2);

                    //context.Response.ContentType = "text/html";
                    string[] renderedArrays = RenderTable(tempFileName2, tempFile);

                    result = File.ReadAllText(Path.Combine(
                        context.Request.PhysicalApplicationPath,
                        "App_Data",
                        "Templates",
                        "TableauConnector",
                        "LoadData.html"
                    ));

                    result = result.Replace("###FIELDNAMES###", renderedArrays[0]);
                    result = result.Replace("###FIELDTYPES###", renderedArrays[1]);
                    result = result.Replace("###RETURNDATA###", renderedArrays[2]);
                    result = result.Replace("###REPORTNAME###", definition.XmlDocument.DocumentElement.Attributes["Name"].Value);

                    Guid idTempFile = Guid.NewGuid();
                    //context.Response.Write(result);

                    break;
                case LinkBiExternalResponseType.JSON:

                    LinkBi1.Classes.Interfaces.JSON json = new LinkBi1.Classes.Interfaces.JSON(Global.Core, definition);
                    result = json.Render();

                    context.Response.ContentType = "application/json";

                    break;
                case LinkBiExternalResponseType.EXCEL:

                    LinkBi1.Classes.Interfaces.Excel excel = new LinkBi1.Classes.Interfaces.Excel(Global.Core, definition);
                    result = excel.Render();

                    WriteFileToResponse(
                        result,
                        "export.xlsx",
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        true
                    );

                    return;

                    break;
                case LinkBiExternalResponseType.CSV:

                    LinkBi1.Classes.Interfaces.CustomCharts csv = new LinkBi1.Classes.Interfaces.CustomCharts(Global.Core, definition);

                    string tempFileName3 = csv.Render();
                    result = File.ReadAllText(tempFileName3);

                    File.Delete(tempFileName3);

                    break;
                default:
                    break;
            }

            context.Response.Write(result);

            // Set the content type of the http response to plain text.

            File.Delete(tempFile);
        }

        private void ProcessDefinedReport(HttpContext context)
        {
            if (context.Request.Params["Username"] != null)
                Authenticate(context, true);

            LinkBiExternalResponseType responseType = LinkBiExternalResponseType.XML;

            if (context.Request.Params["ResponseType"] != null)
            {
                // Get the response type from the request xml document.
                responseType = (LinkBiExternalResponseType)Enum.Parse(
                    typeof(LinkBiExternalResponseType),
                    context.Request.Params["ResponseType"]
                );
            }

            int idLanguage = 2057;

            if (context.Request.Params["IdLanguage"] != null)
            {
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);
            }

            // Build the full path to the LinkBi definition template file.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedLinkBiDefinitions",
                Global.Core.ClientName,
                context.Request.Params["IdReport"] + ".xml"
            );

            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            string result = "";

            XmlNodeList xmlNodesTaxonomyCategories = definition.XmlDocument.SelectNodes(string.Format(
                "//TaxonomyCategory[not(@Label{0})]",
                idLanguage
            ));

            try
            {
                foreach (XmlNode xmlNodeCategory in xmlNodesTaxonomyCategories)
                {
                    xmlNodeCategory.AddAttribute("Label" + idLanguage, (string)Global.Core.TaxonomyCategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyCategory", "IdLanguage" },
                        new object[] { xmlNodeCategory.Attributes["Id"].Value, idLanguage }
                    ));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            XmlNodeList xmlNodesCategories = definition.XmlDocument.SelectNodes(string.Format(
                "//Category[not(@Label{0})]",
                idLanguage
            ));

            try
            {
                foreach (XmlNode xmlNodeCategory in xmlNodesCategories)
                {
                    xmlNodeCategory.AddAttribute("Label" + idLanguage, (string)Global.Core.CategoryLabels.GetValue(
                        "Label",
                        new string[] { "IdCategory", "IdLanguage" },
                        new object[] { xmlNodeCategory.Attributes["Id"].Value, idLanguage }
                    ));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            if (xmlNodesTaxonomyCategories.Count != 0 || xmlNodesCategories.Count != 0)
                definition.Save();

            // Set the content type of the http response to plain text.
            context.Response.ContentType = "text/plain";

            try
            {
                switch (responseType)
                {
                    case LinkBiExternalResponseType.XML:

                        LinkBi1.Classes.Interfaces.XML xml = new LinkBi1.Classes.Interfaces.XML(Global.Core, definition);

                        string tempFileName = xml.Render();
                        result = File.ReadAllText(tempFileName);

                        File.Delete(tempFileName);

                        break;
                    case LinkBiExternalResponseType.TABLE:
                        context.Response.ContentType = "text/html";
                        LinkBi1.Classes.Interfaces.HTML response = new LinkBi1.Classes.Interfaces.HTML(Global.Core, definition);





                        string[] renderedArrays = response.RenderTable();

                        result = File.ReadAllText(Path.Combine(
                        context.Request.PhysicalApplicationPath,
                        "App_Data",
                        "Templates",
                        "TableauConnector",
                        "LoadData.html"
                    ));

                        result = result.Replace("###FIELDNAMES###", renderedArrays[0]);
                        result = result.Replace("###FIELDTYPES###", renderedArrays[1]);
                        result = result.Replace("###RETURNDATA###", "[" + renderedArrays[2] + "]");
                        result = result.Replace("###REPORTNAME###", definition.XmlDocument.DocumentElement.SelectSingleNode("Properties/Name").InnerXml);

                        Guid idTempFile = Guid.NewGuid();
                        //context.Response.Write(result);

                        break;
                    case LinkBiExternalResponseType.JSON:

                        LinkBi1.Classes.Interfaces.JSON json = new LinkBi1.Classes.Interfaces.JSON(Global.Core, definition);
                        result = json.Render();

                        break;
                    case LinkBiExternalResponseType.CSV:

                        LinkBi1.Classes.Interfaces.CustomCharts csv = new LinkBi1.Classes.Interfaces.CustomCharts(Global.Core, definition);

                        string tempFileName2 = csv.Render();
                        result = File.ReadAllText(tempFileName2);

                        File.Delete(tempFileName2);

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            context.Response.Write(result);
        }

        private void ProcessSavedReport(HttpContext context)
        {
            BIToolResponseType responseType = BIToolResponseType.PowerBI;

            if (context.Request.Params["ResponseType"] != null)
            {
                responseType = (BIToolResponseType)Enum.Parse(
                    typeof(BIToolResponseType),
                    context.Request.Params["ResponseType"]
                );
            }

            Guid idUser = Guid.Parse(context.Request.Params["IdReport"].Substring(0, 36));
            Guid idSavedReport = Guid.Parse(context.Request.Params["IdReport"].Substring(36, 36));

            string savedReportDirectoryName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedReports",
                Global.Core.ClientName,
                idUser.ToString(),
                idSavedReport.ToString()
            );
            if (!Directory.Exists(savedReportDirectoryName))
            {
                string[] paths = Directory.GetDirectories(Path.Combine(context.Request.PhysicalApplicationPath,
                           "Fileadmin",
                           "SavedReports",
                           Global.Core.ClientName,
                           idUser.ToString()), "*", SearchOption.AllDirectories);

                if (paths.Where(x => x.ToLower().IndexOf(idSavedReport.ToString().ToLower()) > -1) != null)
                {
                    savedReportDirectoryName = paths.Where(x => x.ToLower().IndexOf(idSavedReport.ToString().ToLower()) > -1).FirstOrDefault();
                }
            }

            string[] files = Directory.GetFiles(savedReportDirectoryName);

            if (context.Request.Params["Tab"] != null)
            {
                files = new string[]
                {
                    Path.Combine(savedReportDirectoryName, context.Request.Params["Tab"] + ".xml")
                };
            }

            foreach (string fileName in files)
            {
                if (fileName.EndsWith("Info.xml"))
                    continue;

                if (responseType == BIToolResponseType.PowerBI)
                {
                    ReportDefinition definition = new ReportDefinition(
                        Global.Core,
                        fileName,
                        Global.HierarchyFilters[fileName]
                    );

                    if (!definition.HasData)
                    {
                        ReportCalculator calculator = new ReportCalculator(
                            definition,
                            Global.Core,
                            HttpContext.Current.Session
                        );

                        calculator.Aggregate((string)HttpContext.Current.Session["Version"]);
                    }

                    ReportDefinitionRenderer renderer = new ReportDefinitionRenderer(definition);

                    ReportDefinitionRenderConfiguration renderConfiguration;

                    if (definition.Settings.PowerBIValues == Crosstables.Classes.PowerBIValues.Values)
                        renderConfiguration = new ReportDefinitionRenderConfiguration(true, false, false, false);
                    else
                        renderConfiguration = new ReportDefinitionRenderConfiguration(false, true, false, false);

                    renderConfiguration.PowerBIExport = true;

                    context.Response.Write("<div id=\"Table" + (new FileInfo(fileName)).Name.Replace(".xml", "") + "\">");

                    try
                    {
                        context.Response.Write(renderer.Render(
                            100,
                            45,
                            renderConfiguration
                        ).Replace("<tr></tr>", "").Replace(">-</td>", ">0</td>").Replace("></td>", ">0</td>").Replace(" %", ""));
                    }
                    catch { }

                    context.Response.Write("</div>");
                }
                else if (responseType == BIToolResponseType.Tableau)
                {
                    ReportDefinition definition = new ReportDefinition(
                        Global.Core,
                        fileName,
                        Global.HierarchyFilters[fileName]
                    );

                    TableauReportDefinitionRenderer renderer = new TableauReportDefinitionRenderer(definition);

                    string[] renderedArrays = renderer.Render();

                    string result = File.ReadAllText(Path.Combine(
                        context.Request.PhysicalApplicationPath,
                        "App_Data",
                        "Templates",
                        "TableauConnector",
                        "LoadData.html"
                    ));

                    result = result.Replace("###FIELDNAMES###", renderedArrays[0]);
                    result = result.Replace("###FIELDTYPES###", renderedArrays[1]);
                    result = result.Replace("###RETURNDATA###", renderedArrays[2]);
                    result = result.Replace("###REPORTNAME###", definition.XmlDocument.DocumentElement.Attributes["Name"].Value);

                    Guid idTempFile = Guid.NewGuid();
                    context.Response.Write(result);
                    break;
                }
                else if (responseType == BIToolResponseType.oData)
                {
                    ReportDefinition definition = new ReportDefinition(
                        Global.Core,
                        fileName,
                        Global.HierarchyFilters[fileName]
                    );

                    ODataRenderer renderer = new ODataRenderer(
                        definition,
                        context.Request.Params["Query"]
                    );

                    context.Response.ContentType = "text/xml";
                    context.Response.Write(renderer.Render());
                }
                else
                {
                    /*string xml = File.ReadAllText(fileName);

                    int index = xml.IndexOf("<Variables Position=\"Left\">");

                    xml = xml.Replace("<Variables Position=\"Left\">", "<Dimensions>");

                    index = xml.IndexOf("</Variables>", index);

                    xml = xml.Remove(index, 12);
                    xml = xml.Insert(index, "</Dimensions>");


                    xml = xml.Replace("<Variables Position=\"Top\">", "<Measures>");

                    index = xml.IndexOf("</Variables>");

                    xml = xml.Remove(index, 12);
                    xml = xml.Insert(index, "</Measures>");

                    string tempFileName = Path.GetTempFileName() + ".xml";

                    //File.WriteAllText(tempFileName, xml);

                    XmlDocument document = new XmlDocument();
                    document.LoadXml(xml);

                    foreach (string container in new string[] {
                        "Dimensions",
                        "Measures"
                    })
                    {
                        XmlNode xmlNodeContainer = document.DocumentElement.SelectSingleNode("Dimensions");

                        MoveNestedNodes(xmlNodeContainer, xmlNodeContainer);
                    }

                    document.Save(tempFileName);

                    LinkBiDefinition definition = new LinkBiDefinition(
                        Global.Core,
                        tempFileName,
                        Global.HierarchyFilters.Default
                    );

                    LinkBi1.Classes.Interfaces.JSON json = new LinkBi1.Classes.Interfaces.JSON(Global.Core, definition);
                    context.Response.Write(json.Render());

                    context.Response.ContentType = "application/json";

                    File.Delete(tempFileName);*/
                    ReportDefinition definition = new ReportDefinition(
                        Global.Core,
                        fileName,
                        Global.HierarchyFilters[fileName]
                    );

                    ReportDefinitionRendererJSON renderer = new ReportDefinitionRendererJSON(
                        definition
                    );

                    context.Response.ContentType = "text/plain";
                    context.Response.Write(renderer.Render());
                }
            }
        }


        private void SignificanceTest(HttpContext context)
        {
            int level = 95;

            if (context.Request.Params["Level"] != null)
                level = int.Parse(context.Request.Params["Level"]);

            double value = double.Parse(context.Request.Params["Value"]);
            double baseValue = double.Parse(context.Request.Params["BaseValue"]);
            double value2 = double.Parse(context.Request.Params["Value2"]);
            double baseValue2 = double.Parse(context.Request.Params["BaseValue2"]);

            // TEST:

            // 95% = 1.96
            // 90% = 1.645
            double zScoreLevel = 0.0;

            switch (level)
            {
                case 95:
                    zScoreLevel = 1.96;
                    break;

                case 90:
                    zScoreLevel = 1.645;
                    break;
            }

            double avgPerc = ((value * baseValue) + (value2 * baseValue2)) / (baseValue + baseValue2);
            double zScore = (value - value2) / Math.Sqrt(avgPerc * (100 - avgPerc) * (1 / baseValue) + (1 / baseValue2));

            if (zScore > zScoreLevel || zScore < (zScoreLevel * -1))
                context.Response.Write("True");
            else
                context.Response.Write("False");

            return;

            // Create a new report calculator.
            ReportCalculator calculator = new ReportCalculator(null, Global.Core, context.Session);

            // Check if the values are significantly different to each other.
            context.Response.Write(calculator.IsSigDiff(
                level,
                value * 100 / baseValue,
                baseValue,
                value2 * 100 / baseValue2,
                baseValue2
            ));
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


        public string[] RenderTable(string fileName, string file)
        {
            // Create a new xml document that
            // contains the result data.
            XmlDocument document = new XmlDocument();

            StringBuilder sbReturnData = new StringBuilder();

            StringBuilder sbFieldNames = new StringBuilder();
            StringBuilder sbFieldTypes = new StringBuilder();

            document.Load(fileName);

            XmlDocument document1 = new XmlDocument();
            document1.Load(file);

            XmlNodeList xmlNodesDimensions1 = document1.DocumentElement.SelectNodes("Dimensions/*");
            foreach (XmlNode xmlNodeDimension in xmlNodesDimensions1)
            {
                if (sbFieldNames.ToString() == "")
                {
                    sbFieldNames.Append("[");
                    sbFieldTypes.Append("[");
                    sbReturnData.Append("[");
                }
                if (xmlNodeDimension.Attributes["Label2057"].Value.Length < 150)
                    sbFieldNames.Append("\"" + xmlNodeDimension.Attributes["Label2057"].Value + "\",");
                else
                    sbFieldNames.Append("\"" + xmlNodeDimension.Attributes["Name"].Value + "\",");

                sbFieldTypes.Append("\"string\",");
            }
            XmlNodeList xmlNodesMeasures = document1.DocumentElement.SelectNodes("Measures/*");

            XmlNodeList xmlDimensions = document.DocumentElement.SelectNodes("Variable/*");
            sbReturnData.Append("[");
            foreach (XmlNode xmlVariable in xmlDimensions)
            {
                RenderTableDimension(
        xmlVariable,
        sbFieldNames,
        sbFieldTypes,
        sbReturnData,
        xmlNodesMeasures.Count,
        false,
        xmlVariable
    );

            }


            sbFieldNames.Append("\"variablename\",");
            sbFieldTypes.Append("\"string\",");

            sbFieldNames.Append("\"responses\",");
            sbFieldTypes.Append("\"string\",");
            if (document.DocumentElement.GetElementsByTagName("TaxonomyCategory").Item(document.DocumentElement.GetElementsByTagName("TaxonomyCategory").Count - 1).Attributes["UBase"] != null)
            {
                sbFieldNames.Append("\"unweighted base\",");
                sbFieldTypes.Append("\"float\",");
            }
            sbFieldNames.Append("\"base\",");
            sbFieldTypes.Append("\"float\",");

            sbFieldNames.Append("\"value\",");
            sbFieldTypes.Append("\"float\",");

            sbFieldNames.Append("\"percentage\",");
            sbFieldTypes.Append("\"float\",");

            sbFieldNames = sbFieldNames.Remove(sbFieldNames.Length - 1, 1);
            sbFieldTypes = sbFieldTypes.Remove(sbFieldTypes.Length - 1, 1);

            sbFieldNames.Append("]");
            sbFieldTypes.Append("]");

            sbReturnData = sbReturnData.Remove(sbReturnData.Length - 2, 2);
            sbReturnData.Append("]");


            return new string[]
           {
                sbFieldNames.ToString(),
                sbFieldTypes.ToString(),
                sbReturnData.ToString()
           };
        }
        List<string> repeatData = new List<string>();

        public void RenderTableDimension(XmlNode xmlNodeDimension, StringBuilder sbFieldNames, StringBuilder sbFieldTypes, StringBuilder sbReturnData, int measure, bool isFieldNameRepeat = false, XmlNode categoryNode = null)
        {

            XmlNodeList xmlNodesDimensions = xmlNodeDimension.SelectNodes("Variable/*");

            foreach (XmlNode x in xmlNodesDimensions)
            {
                if (x.InnerXml != "" && x.Attributes["Label2057"] != null)
                { repeatData.Clear(); repeatData.Add(x.Attributes["Label2057"].Value); }
                if (!isFieldNameRepeat)
                    sbReturnData.Append("\"" + categoryNode.Attributes["Label2057"].Value + "\",");

                if (x.InnerXml == "" && x.ParentNode.Attributes["Label2057"] != null)
                    sbReturnData.Append("\"" + x.ParentNode.Attributes["Label2057"].Value + "\",");

                sbReturnData.Append("\"" + x.Attributes["Label2057"].Value + "\",");


                RenderTableDimension(
                    x,
                    sbFieldNames,
                    sbFieldTypes,
                    sbReturnData,
                    measure,
                    true,
                    categoryNode
                );
            }

            if (xmlNodesDimensions.Count == 0)
            {

                if (xmlNodeDimension.Attributes["UBase"] != null)
                {
                    sbReturnData.Append(xmlNodeDimension.Attributes["UBase"].Value + ",");
                }
                if (xmlNodeDimension.Attributes["Base"] != null && xmlNodeDimension.Attributes["Value"] != null && xmlNodeDimension.Attributes["Percentage"] != null)
                {

                    sbReturnData.Append(xmlNodeDimension.Attributes["Base"].Value + ",");

                    sbReturnData.Append(xmlNodeDimension.Attributes["Value"].Value + ",");
                    double percentage = double.Parse(xmlNodeDimension.Attributes["Percentage"].Value);
                    if (double.IsNaN(percentage))
                        percentage = 0;
                    sbReturnData.Append(percentage);
                    sbReturnData.Append("]");
                    sbReturnData.Append(",");
                    sbReturnData.Append("[");
                }

                if (xmlNodeDimension.NextSibling != null)
                {
                    if (measure > 1)
                        sbReturnData.Append("\"" + categoryNode.Attributes["Label2057"].Value + "\",");
                    foreach (var item in repeatData)
                    {
                        sbReturnData.Append("\"" + item + "\",");
                    }
                }
                else
                {
                    repeatData.Clear();
                }

            }
        }

        #endregion
    }

    public class Global
    {
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
    }

    public delegate void ExternalMethod(HttpContext context, XmlDocument xmlDocument);

    public enum LinkBiExternalResponseType
    {
        XML,
        JSON,
        CSV,
        EXCEL,
        TABLE,
        PowerBI
    }

    public enum BIToolResponseType
    {
        PowerBI,
        Tableau,
        JSON,
        oData
    }

    public enum ErrorResponse
    {
        Exception,
        JSON
    }

    public delegate void Meth(HttpContext context);
}
