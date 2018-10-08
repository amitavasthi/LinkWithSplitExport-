using ApplicationUtilities;
using DatabaseCore.BaseClasses;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataInterface.Classes
{
    public class SpssReader : BaseClasses.BaseReader
    {
        #region Properties

        private OleDbConnection connection = null;

        /// <summary>
        /// Gets the data source of the reader's file.
        /// </summary>
        public string VDataSource
        {
            get
            {
                FileInfo fInfo = new FileInfo(base.FileName);

                switch (fInfo.Extension.ToLower())
                {
                    case ".sav":
                        return "mrSavDsc";
                        break;
                    case ".pkd":
                        return "mrQvDsc";
                        break;
                    case ".ddf":
                        return "mrDataFileDsc";
                        break;
                }

                return "";
            }
        }
        public string MrInitMDSC
        {
            get
            {
                FileInfo fInfo = new FileInfo(base.FileName);

                switch (fInfo.Extension.ToLower())
                {
                    case ".sav":
                        return "mrSavDsc";
                        break;
                    case ".pkd":
                        return "mrQvDsc";
                        break;
                    case ".ddf":
                        return "";
                        break;
                }

                return "";
            }
        }



        /// <summary>
        /// Gets or sets the dictionary which contains
        /// the case data of the current import.
        /// </summary>
        public Dictionary<string, TemporaryDataHolder> Data { get; set; }

        /// <summary>
        /// Gets or sets the respondents of the current import.
        /// </summary>
        public string[] RespondentIds { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of the import's respondents
        /// by the respondent variable value.
        /// </summary>
        private Dictionary<string, Guid> Respondents { get; set; }

        /// <summary>
        /// Gets or sets a dictionary that stores all variables by name.
        /// </summary>
        public Dictionary<string, Variable> Variables { get; set; }

        /// <summary>
        /// Gets or sets a dictionary that stores all variable's categories.
        /// </summary>
        public Dictionary<Guid, Dictionary<int, Guid>> Categories { get; set; }

        public string[] RotationVariables { get; set; }


        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the sav reader.
        /// </summary>
        /// <param name="fileName">The name of the file to read.</param>
        /// <param name="core">The database core of the instance.</param>
        /// <param name="project">The project of the reader's import.</param>
        /// <param name="respondentVariable">The name of the respondent variable.</param>
        /// <param name="createResponsesFile">
        /// The file name of the template database script
        /// which is used to create the responses table.
        /// </param>
        public SpssReader(string fileName, DatabaseCore.Core core, Study project,
            string respondentVariable, string createResponsesFile, int idLanguage)
            : base(fileName, core, project, createResponsesFile, idLanguage)
        {
            this.RespondentVariable = respondentVariable;
            this.Variables = new Dictionary<string, Variable>();
        }

        #endregion


        #region Methods

        MDMLib.Document document;
        /// <summary>
        /// Reads the variables from the file.
        /// </summary>
        public override Variable[] Read()
        {
            if (this.RotationVariables == null)
                this.RotationVariables = new string[0];

            // Create a new list of variables for the result.
            List<Variable> result = new List<Variable>();

            this.Categories = new Dictionary<Guid, Dictionary<int, Guid>>();

            // Open the document.
            /*document = OpenMetadata(
                this.FileName,
                MDMLib.openConstants.oREAD
            );*/

            // Load the case data from the import's source file.
            //InitCaseData();

            double i = 0.0;
            double variableCount = document.Variables.Count;
            int variableOrder = 0;

            base.Status = BaseClasses.DataImportStatus.Step4;
            base.Progress = 0;

            // Run through all variables of the mdm lib document.
            foreach (MDMLib.Variable _variable in document.Variables)
            {
                /*if (_variable.DataType != MDMLib.DataTypeConstants.mtLong &&
                    _variable.DataType != MDMLib.DataTypeConstants.mtDouble)
                    continue;*/

                // Check if the variable is a field.
                if ((_variable is MDMLib.IMDMField) == false)
                    continue;

                if (!_variable.HasCaseData)
                    continue;

                MDMLib.IMDMField field = (MDMLib.IMDMField)_variable;

                MDMLib.VariableInstance instance = document.get_VariableInstance(
                    field.FullName,
                    "",
                    false
                );

                // Create a new variable.
                Variable variable = new Variable(base.Core.Variables);

                // Set the variable's project id.
                variable.IdStudy = base.Study.Id;

                string variableName = field.FullName;

                // Set the variable's values.
                variable.Name = field.FullName;
                //variable.Type = (VariableType);
                //variable.ScaleType = (int)field.Scale;
                switch ((_variable.DataType))
                {
                    case MDMLib.DataTypeConstants.mtBoolean:
                    case MDMLib.DataTypeConstants.mtCategorical:
                        variable.Type = VariableType.Single;
                        break;
                    case MDMLib.DataTypeConstants.mtLong:
                    case MDMLib.DataTypeConstants.mtDouble:
                        variable.Type = VariableType.Numeric;
                        break;
                    case MDMLib.DataTypeConstants.mtText:
                    case MDMLib.DataTypeConstants.mtDate:
                        variable.Type = VariableType.Text;
                        break;
                    case MDMLib.DataTypeConstants.mtLevel:
                        break;
                    case MDMLib.DataTypeConstants.mtNone:
                        break;
                    case MDMLib.DataTypeConstants.mtObject:
                        break;
                }

                if (variable.Type == VariableType.Single && _variable.EffectiveMaxValue > 1)
                    variable.Type = VariableType.Multi;

                if (variable.Name == null)
                    variable.Name = Guid.NewGuid().ToString();

                variable.Order = variableOrder++;

                // Lock the insert action.
                lock (this)
                {
                    // Insert the new variable into the database.
                    variable.Insert();
                }

                // Run through all languages of the document.
                foreach (MDMLib.Language language in document.Languages)
                {
                    // Set the current language.
                    document.Languages.Current = language.Name;

                    // Get the language code.
                    string languageCode = language.XMLName;

                    CultureInfo cultureInfo = new CultureInfo(languageCode);

                    // Create a new variable label.
                    VariableLabel variableLabel = new VariableLabel(this.Core.VariableLabels);
                    variableLabel.Label = field.Label;
                    variableLabel.ReportLabel = field.Label;
                    //variableLabel.IdLanguage = cultureInfo.LCID;
                    variableLabel.IdLanguage = this.IdLanguage;
                    variableLabel.IdVariable = variable.Id;

                    // Lock the insert action.
                    lock (this)
                    {
                        // Insert the new variable label into the database.
                        variableLabel.Insert();
                    }
                }

                this.Categories.Add(variable.Id, new Dictionary<int, Guid>());

                this.Variables.Add(variable.Name, variable);

                // Check if the variable is text and a rotation variable.
                if (variable.Type == VariableType.Text && this.RotationVariables.Contains(variable.Name))
                {
                    // Create a new text coder for the variable.
                    TextCoder coder = new TextCoder(variable, this.Respondents, TextCodingType.Multi);

                    // Code the variable's responses as categories.
                    coder.CodeResponses(
                        this.Data[this.RespondentVariable].Get(),
                        this.Data[variable.Name].Get()
                    );

                    // Get all coded categories of the variable.
                    List<object[]> categories = this.Core.Categories.GetValues(
                        new string[] { "Id", "Name" },
                        new string[] { "IdVariable" },
                        new object[] { variable.Id }
                    );

                    // Run through all coded categories of the variable.
                    foreach (object[] category in categories)
                    {
                        // Run through all languages of the document.
                        foreach (MDMLib.Language language in document.Languages)
                        {
                            // Set the current language.
                            document.Languages.Current = language.Name;

                            // Get the language code.
                            string languageCode = language.XMLName;

                            CultureInfo cultureInfo = new CultureInfo(languageCode);

                            // Create a new category label.
                            CategoryLabel categoryLabel = new CategoryLabel(this.Core.CategoryLabels);
                            categoryLabel.Label = (string)category[1];
                            //categoryLabel.IdLanguage = cultureInfo.LCID;
                            categoryLabel.IdLanguage = this.IdLanguage;
                            categoryLabel.IdCategory = (Guid)category[0];

                            // Lock the insert action.
                            lock (this)
                            {
                                // Insert the new category label into the database.
                                categoryLabel.Insert();
                            }
                        }
                    }
                }
                else
                {
                    int valueCounter = 1;
                    int categoryOrder = 0;
                    // Run through all categories of the variable.
                    foreach (MDMLib.IElement _category in _variable.Categories)
                    {
                        // Create a new category.
                        Category category = new Category(base.Core.Categories);

                        // Set the category's variable id.
                        category.IdVariable = variable.Id;

                        // Set the category's values.
                        category.Name = _category.Name;

                        int id = document.CategoryMap.get_NameToValue(category.Name);

                        if (_category.Factor != null)
                            category.Factor = (float)_category.Factor;

                        category.Value = valueCounter++;
                        category.Order = categoryOrder++;

                        // Lock the insert action.
                        lock (this)
                        {
                            // Insert the new category into the database.
                            category.Insert();
                        }

                        // Run through all languages of the document.
                        foreach (MDMLib.Language language in document.Languages)
                        {
                            // Set the current language.
                            document.Languages.Current = language.Name;

                            // Get the language code.
                            string languageCode = language.XMLName;

                            CultureInfo cultureInfo = new CultureInfo(languageCode);

                            // Create a new category label.
                            CategoryLabel categoryLabel = new CategoryLabel(this.Core.CategoryLabels);
                            categoryLabel.Label = _category.Label;
                            //categoryLabel.IdLanguage = cultureInfo.LCID;
                            categoryLabel.IdLanguage = this.IdLanguage;
                            categoryLabel.IdCategory = category.Id;

                            // Lock the insert action.
                            lock (this)
                            {
                                // Insert the new category label into the database.
                                categoryLabel.Insert();
                            }
                        }

                        string categoryName = category.Name.ToLower();

                        // Remove all special characters.
                        //categoryName = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.UTF8.GetBytes(categoryName)).Replace("?", "");

                        this.Categories[variable.Id].Add(id, category.Id);
                    }

                    // Create a new category for the system missings.
                    Category smCategory = new Category(base.Core.Categories);

                    // Set the category's variable id.
                    smCategory.IdVariable = variable.Id;

                    // Set the category's values.
                    smCategory.Name = "SystemMissing";
                    smCategory.SetValue("Enabled", false);
                    smCategory.SetValue("ExcludeBase", true);

                    smCategory.Order = categoryOrder++;

                    // Lock the insert action.
                    lock (this)
                    {
                        // Insert the new category into the database.
                        smCategory.Insert();
                    }

                    // Create a new category label for the system missing category.
                    CategoryLabel smCategoryLabel = new CategoryLabel(base.Core.CategoryLabels);
                    smCategoryLabel.Label = "system missing";
                    smCategoryLabel.IdLanguage = this.IdLanguage;
                    smCategoryLabel.IdCategory = smCategory.Id;

                    // Lock the insert action.
                    lock (this)
                    {
                        // Insert the new category label into the database.
                        smCategoryLabel.Insert();
                    }

                    this.Categories[variable.Id].Add(-1, smCategory.Id);
                }

                // Calculate the current reading progress.
                base.Progress = (int)(i++ * 100 / variableCount);
            }

            base.Progress = 100;

            InitCaseData();

            // Open the database connection.
            dataConnection.Open();

            base.Status = BaseClasses.DataImportStatus.Step5;
            base.Progress = 0;

            // Insert the respondents.
            InsertRespondents();

            base.ResponseInsertStarted = DateTime.Now;
            base.Status = BaseClasses.DataImportStatus.Step6;
            base.Progress = 0;

            i = 0;

            bool asynchEnabled = true;

            //List<string> inserts = new List<string>();

            if (this.Core.CaseDataLocation == ApplicationUtilities.Classes.CaseDataLocation.File)
            {
                i = InsertResponsesFile();

                foreach (Variable variable in this.Variables.Values)
                {
                    if (variable.Type != VariableType.Text)
                        continue;

                    // Create the responses database table for the variable.
                    CreateResponsesTable(variable);

                    // Insert the variable's responses.
                    InsertResponsesSql(variable);

                    base.Progress = ((i++ * 100) / (double)this.Variables.Count);
                }
            }
            else
            {
                foreach (Variable variable in this.Variables.Values)
                {
                    // Create the responses database table for the variable.
                    CreateResponsesTable(variable);

                    // Insert the variable's responses.
                    InsertResponsesSql(variable);

                    base.Progress = ((i++ * 100) / (double)this.Variables.Count);
                }
            }

            /*i = 0;
            foreach (Variable variable in this.Variables.Values)
            {
                string fileName = Path.Combine(tempInsertDirectoryName, variable.Id + ".txt");

                if (!File.Exists(fileName))
                    continue;

                string insert = File.ReadAllText(fileName);

                if (insert.Length == 0)
                    continue;

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    this.Core.Respondents.ExecuteQuery(insert);
                }));

                if (tasks.Count == 10)
                {
                    Task.WaitAll(tasks.ToArray());

                    tasks.Clear();
                }

                base.Progress = 50 + ((i++ * 50) / (double)this.Variables.Count);
            }*/
            i = 0;
            /*foreach (Variable variable in this.Variables.Values)
            {
                string fileName = Path.Combine(tempInsertDirectoryName, variable.Id + ".txt");

                if (!File.Exists(fileName))
                    continue;

                string insert = File.ReadAllText(fileName);

                if (insert.Length == 0)
                    continue;
                
                this.Core.Respondents.ExecuteQuery(insert);

                base.Progress = 50 + ((i++ * 50) / (double)this.Variables.Count);
            }*/


            //Task.WaitAll(tasks.ToArray());

            //Directory.Delete(tempInsertDirectoryName, true);

            dataConnection.Close();

            CreateStudyLinkVariable();

            //ImportCaseData();

            base.Progress = 100;

            // Check if the vData connection exists.
            if (connection != null)
            {
                // Close the vData connection.
                connection.Close();

                // Dispose the vData connection.
                connection.Dispose();
            }

            this.Data.Clear();
            this.Categories.Clear();
            this.Respondents.Clear();

            // Run the garbage collector.
            System.GC.Collect();

            // Check if a mail to the creator of the study should be sended.
            if (this.SendMail)
            {
                // Create a new mail configuration and load the
                // configuration values from the web.config file.
                MailConfiguration mailConfiguration = new MailConfiguration(true);

                // Create a new mail by the mail configuration.
                Mail mail = new Mail(mailConfiguration, this.Core.ClientName);

                // Set the full path to the mail's template file.
                mail.TemplatePath = Path.Combine(
                    this.ApplicationPath,
                    "App_Data",
                    "MailTemplates",
                    this.Language,
                    "UploadFinished.html"
                );

                User user = this.Core.Users.GetSingle(this.Study.IdUser);

                // Add the placeholder value for the user's first name.
                mail.Placeholders.Add("FirstName", user.FirstName);

                // Add the placeholder value for the user's last name.
                mail.Placeholders.Add("LastName", user.LastName);

                // Add the placeholder value for the study's name.
                mail.Placeholders.Add("STUDY", this.Study.Name);

                //mail.Placeholders.Add("imagepath", "http://" + Request.Url.Host.ToString() + "/Images/Logos/link.png");

                try
                {
                    // Send the mail.
                    mail.Send(user.Mail);
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                string logFileName = Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "Log.txt"
                );

                /*File.WriteAllText(logFileName, string.Format(
                    "File access: {0}{2}Database inserts:{1}",
                    swFileAccess.Elapsed.ToString(),
                    swDatabaseInserts.Elapsed.ToString(),
                    Environment.NewLine
                ));*/
                File.WriteAllText(logFileName, string.Format(
                    "{0}",
                    (DateTime.Now - base.StartTime).ToString() +
                    Environment.NewLine
                ));
            }
            catch { }

            // Return the result list of variables.
            return result.ToArray();
        }

        private void CreateResponsesTable(Variable variable)
        {
            // Check if the script template file exists.
            if (!File.Exists(base.CreateResponsesTemplateFile))
                throw new Exception("Create responses script template does not exist.");

            // Read the script from the script template file.
            string script = File.ReadAllText(base.CreateResponsesTemplateFile);

            // Format the script with the variable's id.
            script = string.Format(
                script,
                variable.Id
            );

            // Lock the create table action.
            lock (this)
            {
                // Execute the script on the database.
                this.Core.Responses[variable.Id].ExecuteQuery(script);
            }
        }

        private void CreateStudyLinkVariable()
        {
            // Create a new variable.
            Variable variable = new Variable(base.Core.Variables);

            // Set the variable's project id.
            variable.IdStudy = base.Study.Id;

            // Set the variable's values.
            variable.Name = "StudyLink";
            variable.Type = VariableType.Single;
            variable.Order = 0;

            // Lock the insert action.
            lock (this)
            {
                // Insert the new variable into the database.
                variable.Insert();
            }

            // Create a new variable label.
            VariableLabel variableLabel = new VariableLabel(this.Core.VariableLabels);
            variableLabel.Label = "StudyLink";
            variableLabel.IdLanguage = 2057;
            variableLabel.IdVariable = variable.Id;

            // Lock the insert action.
            lock (this)
            {
                // Insert the new variable label into the database.
                variableLabel.Insert();
            }

            // Create a new category.
            Category category = new Category(base.Core.Categories);

            // Set the category's variable id.
            category.IdVariable = variable.Id;

            // Set the category's values.
            category.Name = "StudyLink";

            //category.Factor = (categoryOrder + 1);
            category.Value = 0;
            category.Order = 0;

            // Lock the insert action.
            lock (this)
            {
                // Insert the new variable into the database.
                category.Insert();
            }

            // Create a new category label.
            CategoryLabel categoryLabel = new CategoryLabel(this.Core.CategoryLabels);
            categoryLabel.Label = "StudyLink";
            categoryLabel.IdLanguage = 2057;
            categoryLabel.IdCategory = category.Id;

            // Lock the insert action.
            lock (this)
            {
                // Insert the new category label into the database.
                categoryLabel.Insert();
            }

            CreateResponsesTable(variable);

            StringBuilder bulkInsertBuilder = new StringBuilder();

            int insertSteps = this.Respondents.Count / 100;

            // Run through all respondents of the study.
            foreach (Guid idRespondent in this.Respondents.Values)
            {
                Response response = new Response(this.Core.Responses[variable.Id]);
                response.IdRespondent = idRespondent;
                response.IdStudy = this.Study.Id;

                response.IdCategory = category.Id;

                bulkInsertBuilder.Append(response.RenderInsertQuery());
                bulkInsertBuilder.Append(Environment.NewLine);
            }

            this.Core.Respondents.ExecuteQuery(bulkInsertBuilder.ToString());
        }

        public override List<string> Validate()
        {
            base.Status = BaseClasses.DataImportStatus.Step3;

            string connectionString = "Provider=mrOleDB.Provider.2;Persist Security Info=False;User ID=;Data Source={1};Location={2};Extended Properties=;Initial Catalog={0};Mode=ReadWrite;MR Init MDSC={3};MR Init MDSC Access=2;MR Init MDM Version=;MR Init MDM Language=;MR Init MDM Context=;MR Init MDM Label Type=;MR Init MDM Access=0;MR Init MDM DataSource Use=0;MR Init MDM Version Variable=False;MR Init Category Names=1;MR Init Category Values=0;MR Init Allow Dirty=False;MR Init Validation=True;MR Init Input Locale=0;MR Init Output Locale=0;MR Init Project=;MR Init Custom=;MR Init MDM Document=;MR Init Overwrite=0;MR Init Native Schema=False;MR Init Merge Data Elements=False";

            if (ConfigurationManager.AppSettings["ConnectionStringVData"] != null)
                connectionString = ConfigurationManager.AppSettings["ConnectionStringVData"];

            OleDbCommand command = null;

            List<string> result = new List<string>();
            OleDbConnection connection = null;

            try
            {
                connectionString = string.Format(
                    connectionString,
                    this.FileName.Replace(".ddf", ".mdd"),
                    this.VDataSource,
                    this.FileName,
                    this.MrInitMDSC
                );

                // Create a new database connection.
                connection = new OleDbConnection(connectionString);

                // Open the database connection.
                connection.Open();

                // Create a new data adapter for selecting the import's data.
                command = new OleDbCommand(
                    string.Format(
                        "SELECT {0} FROM vData",
                        this.RespondentVariable
                    ),
                    connection
                );
            }
            catch
            {
                result.Add(string.Format(
                    "An error occurred while accessing the case data. {0}",
                    connectionString
                ));
            }

            try
            {
                if (command != null)
                    command.ExecuteNonQuery();
            }
            catch
            {
                result.Add(string.Format(
                    "The defined respondent variable '{0}' wasn't found in the data file.",
                    this.RespondentVariable
                ));
            }

            try
            {
                if (connection != null)
                    connection.Close();

                connection.Dispose();
            }
            catch { }

            try
            {
                document = OpenMetadata(this.FileName.Replace(".ddf", ".mdd"), MDMLib.openConstants.oREAD);
            }
            catch (Exception e)
            {
                result.Add(e.Message);
            }

            return result;
        }

        /// <summary>
        /// Opens a dimensions file.
        /// </summary>
        /// <param name="fileName">The full path to the file to open.</param>
        /// <param name="openMode">The open mode which should be used.</param>
        /// <returns></returns>
        public MDMLib.Document OpenMetadata(string fileName, MDMLib.openConstants openMode)
        {
            // Get the file information of the file to open.
            FileInfo fInfo = new FileInfo(fileName);

            // Get the file's file extension.
            string fileExtension = fInfo.Extension.ToLower();

            //Opens MDD or MDSC, depending if the dsc is empty (MDD) or if there is a mdsc name.
            MRDSCReg.Components dscs = null;

            try
            {
                dscs = new MRDSCReg.Components();
            }
            catch { }

            MRDSCReg.Component dsc = null;

            MDMLib.Document document = new MDMLib.Document();

            // Check if the file is a .mdd file.
            if (fileExtension.Length > 0 & fileExtension.ToLower() != ".mdd")
            {
                // Run through all dimensions components in the components list.
                foreach (MRDSCReg.Component testDsc in dscs)
                {
                    string dscFileExt = testDsc.Metadata.
                        FileMasks.String.ToLower();

                    if (dscFileExt.Length != 0)
                    {
                        if (dscFileExt.Split('|')[1].Split(';')[0] == "*" + fileExtension.ToLower())
                        {
                            dsc = testDsc;
                        }
                    }
                }

                if (dsc != null)
                {
                    // Open the file.
                    document = (MDMLib.Document)dsc.Metadata.Open(
                        fileName,
                        "",
                        MRDSCReg.openConstants.oREAD
                    );
                }
            }
            else
            {
                // Open the document as .mdd file.
                document.Open(fileName, "", openMode);
            }
            return document;
        }


        /// <summary>
        /// Reads the values from the respondent variable from vData.
        /// </summary>
        private void InsertRespondents()
        {
            // Create a new dictionary which stores the
            // respondents by the respondent variable value.
            this.Respondents = new Dictionary<string, Guid>();

            //if (!this.Data.ContainsKey(this.RespondentVariable))
            //    return;

            OleDbCommand command = new OleDbCommand(
                string.Format("SELECT {0} FROM vData", this.RespondentVariable),
                dataConnection
            );

            this.Data = new Dictionary<string, TemporaryDataHolder>();

            OleDbDataReader reader = command.ExecuteReader();

            List<string> identifiers = new List<string>();

            while (reader.Read())
            {
                string identifier = reader.GetValue(0).ToString();

                // Check if the respondent already exists.
                if (identifiers.Contains(identifier))
                    continue;

                identifiers.Add(identifier);
            }

            StringBuilder bulkInsertBuilder = new StringBuilder();

            int insertSteps = identifiers.Count / 100;
            int i = 0;

            if (insertSteps <= 0)
                insertSteps = 1;

            foreach (string identifier in identifiers)
            {
                // Create a new respondent.
                Respondent respondent = new Respondent(this.Core.Respondents);
                respondent.IdStudy = this.Study.Id;
                respondent.OriginalRespondentID = identifier;

                // Lock the insert action.
                /*lock (this)
                {
                    respondent.Insert();
                }*/
                bulkInsertBuilder.Append(respondent.RenderInsertQuery());
                bulkInsertBuilder.Append(Environment.NewLine);

                this.Respondents.Add(identifier, respondent.Id);

                //base.Progress = (int)((i++ * 100) / identifiers.Count);
                if (i % insertSteps == 0)
                {
                    this.Core.Respondents.ExecuteQuery(bulkInsertBuilder.ToString());
                    bulkInsertBuilder = new StringBuilder();

                    base.Progress = (int)((i++ * 100) / identifiers.Count);
                }
            }

            if (bulkInsertBuilder.Length > 0)
                this.Core.Respondents.ExecuteQuery(bulkInsertBuilder.ToString());
        }

        OleDbConnection dataConnection = null;
        /// <summary>
        /// Loads the case data from the import into the memory.
        /// </summary>
        private void InitCaseData()
        {
            string connectionString = "Provider=mrOleDB.Provider.2;Persist Security Info=False;User ID=;Data Source={1};Location={0};Extended Properties=;Initial Catalog={0};Mode=ReadWrite;MR Init MDSC={1};MR Init MDSC Access=2;MR Init MDM Version=;MR Init MDM Language=;MR Init MDM Context=;MR Init MDM Label Type=;MR Init MDM Access=0;MR Init MDM DataSource Use=0;MR Init MDM Version Variable=False;MR Init Category Names=1;MR Init Category Values=0;MR Init Allow Dirty=False;MR Init Validation=True;MR Init Input Locale=0;MR Init Output Locale=0;MR Init Project=;MR Init Custom=;MR Init MDM Document=;MR Init Overwrite=0;MR Init Native Schema=False;MR Init Merge Data Elements=False";

            if (ConfigurationManager.AppSettings["ConnectionStringVData"] != null)
                connectionString = ConfigurationManager.AppSettings["ConnectionStringVData"];

            // Create a new database connection.
            dataConnection = new OleDbConnection(string.Format(
                connectionString,
                this.FileName.Replace(".ddf", ".mdd"),
                this.VDataSource,
                this.FileName,
                this.MrInitMDSC
            ));
        }

        /// <summary>
        /// Reads the values from vData and inserts the responses into the database.
        /// </summary>
        /// <param name="variable">The variable to create responses for.</param>
        private void InsertResponses(Variable variable)
        {
            if (variable.Type == VariableType.Numeric || variable.Type == VariableType.Text)
            {
                InsertResponsesSql(variable);
                return;
            }

            Stopwatch stopwatchSql = new Stopwatch();
            Stopwatch stopwatchKDF = new Stopwatch();

            stopwatchSql.Start();
            InsertResponsesSql(variable);
            stopwatchSql.Stop();

            stopwatchKDF.Start();
            InsertResponsesFile(variable);
            stopwatchKDF.Stop();

            string test = "";
            /*
            switch (variable.Type)
            {
                case VariableType.Text:
                case VariableType.Numeric:
                    InsertResponsesSql(variable);
                    break;
                case VariableType.Single:
                case VariableType.Multi:
                    InsertResponsesFile(variable);
                    break;
            }*/
        }

        private void InsertResponsesSql(Variable variable)
        {
            MDMLib.DataTypeConstants variableType = (MDMLib.DataTypeConstants)variable.Type;

            Dictionary<Guid, List<BaseItem<Response>>> queue = new Dictionary<Guid, List<BaseItem<Response>>>();

            List<string> fields = new List<string>();
            fields.Add("Id");
            fields.Add("IdRespondent");

            switch (variable.Type)
            {
                case VariableType.Text:
                    fields.Add("TextAnswer");
                    break;
                case VariableType.Single:
                case VariableType.Multi:
                    break;
                case VariableType.Numeric:
                    fields.Add("NumericAnswer");

                    break;
                default:
                    break;
            }

            string variableNameSelect = variable.Name;

            if (!variableNameSelect.Contains("["))
                variableNameSelect = "[" + variableNameSelect + "]";

            string respondentVariableNameSelect = this.RespondentVariable;

            if (!respondentVariableNameSelect.Contains("["))
                respondentVariableNameSelect = "[" + respondentVariableNameSelect + "]";

            OleDbCommand command = new OleDbCommand(
                string.Format("SELECT {0}, {1} FROM vData", respondentVariableNameSelect, variableNameSelect),
                dataConnection
            );

            //this.Data = new Dictionary<string, TemporaryDataHolder>();

            //swFileAccess.Start();
            OleDbDataReader reader = command.ExecuteReader();
            //swFileAccess.Stop();

            int insertSteps = this.Respondents.Count / 100;
            if (insertSteps <= 0)
                insertSteps = 1;
            int i = 0;

            StringBuilder bulkInsertBuilder = new StringBuilder();

            int c = 0;
            while (reader.Read())
            {
                //swFileAccess.Start();

                string respondentId = reader.GetValue(0).ToString();
                object _value = reader.GetValue(1);

                if (!this.Respondents.ContainsKey(respondentId))
                    continue;

                //swFileAccess.Stop();

                string _values = "";

                _values = _value.ToString();

                if (_values.StartsWith("{") && _values.EndsWith("}"))
                {
                    // Remove the brakets from the value.
                    _values = _values.Remove(0, 1);
                    _values = _values.Remove(_values.Length - 1, 1);
                }

                _values = _values.ToLower();

                if (variable.Type == VariableType.Multi || variable.Type == VariableType.Single)
                    _values = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Default.GetBytes(_values));

                // Get the respondent of the response.
                Guid idRespondent = this.Respondents[respondentId];

                string[] values = new string[] { _values };

                if (variable.Type == VariableType.Multi || variable.Type == VariableType.Single)
                    values = _values.Split(',');

                foreach (string value in values)
                {
                    // Create a new response.
                    Response response = new Response(this.Core.Responses[variable.Id]);
                    response.IdStudy = this.Study.Id;
                    response.IdRespondent = idRespondent;

                    Guid key = new Guid();

                    switch (variable.Type)
                    {
                        /*case MDMLib.DataTypeConstants.mtBoolean:
                            break;*/
                        case VariableType.Single:
                        case VariableType.Multi:

                            int id;

                            if (!int.TryParse(value, out id))
                                id = -1;

                            if (!this.Categories[variable.Id].ContainsKey(id))
                                continue;

                            response.IdCategory = this.Categories[variable.Id][id];

                            key = response.IdCategory;

                            break;
                        case VariableType.Numeric:

                            decimal numericValue;

                            if (_value.GetType() == typeof(double))
                            {
                                numericValue = Convert.ToDecimal((double)_value);
                            }

                            else if (!decimal.TryParse(value, out numericValue))
                                continue;

                            key = variable.Id;

                            // Set the response's numeric answer.
                            response.NumericAnswer = numericValue;

                            break;
                        case VariableType.Text:

                            // Set the response's text answer.
                            response.TextAnswer = value;

                            key = variable.Id;

                            break;
                        default:

                            // Set the response's text answer.
                            response.TextAnswer = value;

                            key = variable.Id;
                            break;
                    }

                    /*if (variable.Type == VariableType.Text)
                    {
                        //swDatabaseInserts.Start();
                        response.Insert();
                        //swDatabaseInserts.Stop();
                    }
                    else*/
                    {
                        bulkInsertBuilder.Append(response.RenderInsertQuery());
                    }

                    if (i++ % insertSteps == 0)
                    {
                        this.Core.Respondents.ExecuteQuery(bulkInsertBuilder.ToString());
                        bulkInsertBuilder = new StringBuilder();
                    }
                }
            }

            if (bulkInsertBuilder.Length > 0)
            {
                this.Core.Responses[variable.Id].ExecuteQuery(bulkInsertBuilder.ToString());
                bulkInsertBuilder.Clear();
            }
        }

        private void InsertResponsesFile(Variable variable)
        {
            MDMLib.DataTypeConstants variableType = (MDMLib.DataTypeConstants)variable.Type;

            Dictionary<Guid, List<BaseItem<Response>>> queue = new Dictionary<Guid, List<BaseItem<Response>>>();

            List<string> fields = new List<string>();
            fields.Add("Id");
            fields.Add("IdRespondent");

            switch (variable.Type)
            {
                case VariableType.Text:
                    fields.Add("TextAnswer");
                    break;
                case VariableType.Single:
                case VariableType.Multi:
                    break;
                case VariableType.Numeric:
                    fields.Add("NumericAnswer");

                    break;
                default:
                    break;
            }

            string variableNameSelect = variable.Name;

            if (!variableNameSelect.Contains("["))
                variableNameSelect = "[" + variableNameSelect + "]";

            string respondentVariableNameSelect = this.RespondentVariable;

            if (!respondentVariableNameSelect.Contains("["))
                respondentVariableNameSelect = "[" + respondentVariableNameSelect + "]";

            OleDbCommand command = new OleDbCommand(
                string.Format("SELECT {0}, {1} FROM vData", respondentVariableNameSelect, variableNameSelect),
                dataConnection
            );

            //this.Data = new Dictionary<string, TemporaryDataHolder>();

            //swFileAccess.Start();
            OleDbDataReader reader = command.ExecuteReader();
            //swFileAccess.Stop();

            int insertSteps = this.Respondents.Count / 100;
            if (insertSteps <= 0)
                insertSteps = 1;
            int i = 0;

            List<byte> result = new List<byte>();

            int c = 0;
            while (reader.Read())
            {
                //swFileAccess.Start();

                string respondentId = reader.GetValue(0).ToString();
                object _value = reader.GetValue(1);

                if (!this.Respondents.ContainsKey(respondentId))
                    continue;

                //swFileAccess.Stop();

                string _values = "";

                _values = _value.ToString();

                if (_values.StartsWith("{") && _values.EndsWith("}"))
                {
                    // Remove the brakets from the value.
                    _values = _values.Remove(0, 1);
                    _values = _values.Remove(_values.Length - 1, 1);
                }

                _values = _values.ToLower();

                if (variable.Type == VariableType.Multi || variable.Type == VariableType.Single)
                    _values = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Default.GetBytes(_values));

                // Get the respondent of the response.
                Guid idRespondent = this.Respondents[respondentId];

                string[] values = new string[] { _values };

                if (variable.Type == VariableType.Multi || variable.Type == VariableType.Single)
                    values = _values.Split(',');

                foreach (string value in values)
                {
                    int id;

                    if (!int.TryParse(value, out id))
                        id = -1;

                    if (!this.Categories[variable.Id].ContainsKey(id))
                        continue;

                    result.AddRange(idRespondent.ToByteArray());
                    result.AddRange(this.Categories[variable.Id][id].ToByteArray());
                }
            }

            string path = Path.Combine(
                ConfigurationManager.AppSettings["CaseDataPath"],
                this.Core.ClientName,
                variable.Id + ".kdf"
            );

            File.WriteAllBytes(path, result.ToArray());

            result.Clear();
        }
        private int InsertResponsesFile()
        {
            List<string> variables = new List<string>();

            Dictionary<string, List<byte>> result = new Dictionary<string, List<byte>>();

            string respondentVariableNameSelect = this.RespondentVariable;
            result.Add(respondentVariableNameSelect, new List<byte>());

            if (!respondentVariableNameSelect.Contains("["))
                respondentVariableNameSelect = "[" + respondentVariableNameSelect + "]";

            variables.Add(respondentVariableNameSelect);

            Dictionary<string, int> fieldLength = new Dictionary<string, int>();
            foreach (string variable in this.Variables.Keys)
            {
                if (this.Variables[variable].Type == VariableType.Text)
                    continue;

                    string variableNameSelect = this.Variables[variable].Name;

                if (this.Variables[variable].Type == VariableType.Text)
                    fieldLength.Add(variable, 0);

                if (!result.ContainsKey(variableNameSelect))
                    result.Add(variableNameSelect, new List<byte>());

                if (!variableNameSelect.Contains("["))
                    variableNameSelect = "[" + variableNameSelect + "]";

                if (variableNameSelect == respondentVariableNameSelect)
                    continue;

                variables.Add(variableNameSelect);
            }

            OleDbCommand command = new OleDbCommand(
                string.Format("SELECT {0} FROM vData", string.Join(",", variables)),
                dataConnection
            );

            //this.Data = new Dictionary<string, TemporaryDataHolder>();

            //swFileAccess.Start();
            OleDbDataReader reader = command.ExecuteReader();

            int c = 0;
            while (reader.Read())
            {
                string respondentId = reader.GetValue(0).ToString();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object _value = reader.GetValue(i);

                    if (!this.Respondents.ContainsKey(respondentId))
                        continue;

                    if (!this.Variables.ContainsKey(reader.GetName(i)))
                        continue;

                    Variable variable = this.Variables[reader.GetName(i)];

                    string _values = "";

                    _values = _value.ToString();

                    if (_values.StartsWith("{") && _values.EndsWith("}"))
                    {
                        // Remove the brakets from the value.
                        _values = _values.Remove(0, 1);
                        _values = _values.Remove(_values.Length - 1, 1);
                    }

                    _values = _values.ToLower();

                    if (variable.Type == VariableType.Multi || variable.Type == VariableType.Single)
                        _values = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Default.GetBytes(_values));

                    // Get the respondent of the response.
                    Guid idRespondent = this.Respondents[respondentId];

                    string[] values = new string[] { _values };

                    if (variable.Type == VariableType.Multi || variable.Type == VariableType.Single)
                        values = _values.Split(',');

                    

                    foreach (string value in values)
                    {
                        switch (variable.Type)
                        {
                            /*case MDMLib.DataTypeConstants.mtBoolean:
                                break;*/
                            case VariableType.Single:
                            case VariableType.Multi:


                                int id;

                                if (!int.TryParse(value, out id))
                                    id = -1;

                                if (!this.Categories[variable.Id].ContainsKey(id))
                                    continue;

                                result[variable.Name].AddRange(idRespondent.ToByteArray());
                                result[variable.Name].AddRange(this.Categories[variable.Id][id].ToByteArray());

                                break;
                            case VariableType.Numeric:

                                decimal numericValue;

                                if (_value.GetType() == typeof(double))
                                {
                                    numericValue = Convert.ToDecimal((double)_value);
                                }

                                else if (!decimal.TryParse(value, out numericValue))
                                    continue;

                                result[variable.Name].AddRange(idRespondent.ToByteArray());
                                // Set the response's numeric answer.
                                result[variable.Name].AddRange(BitConverter.GetBytes((double)numericValue));

                                break;
                            case VariableType.Text:

                                byte[] stringBuffer = System.Text.Encoding.UTF8.GetBytes(value);

                                if (stringBuffer.Length > fieldLength[variable.Name])
                                    fieldLength[variable.Name] = stringBuffer.Length;

                                result[variable.Name].AddRange(idRespondent.ToByteArray());
                                result[variable.Name].AddRange(stringBuffer);

                                break;
                        }
                    }

                    c++;
                }

                base.Progress = (c * 100) / ((double)variables.Count * this.Respondents.Count);
            }

            foreach (string variable in result.Keys)
            {
                string path = Path.Combine(
                    ConfigurationManager.AppSettings["CaseDataPath"],
                    this.Core.ClientName,
                    this.Variables[variable].Id + ".kdf"
                );

                /*if (fieldLength.ContainsKey(variable))
                {
                    result[variable].InsertRange(0, BitConverter.GetBytes(fieldLength[variable]));
                }*/

                File.WriteAllBytes(path, result[variable].ToArray());

                result[variable].Clear();
            }

            result.Clear();

            return variables.Count;
        }

        #endregion
    }

    public class TemporaryDataHolder
    {
        #region Properties

        /// <summary>
        /// Gets or sets where the data should be stored.
        /// </summary>
        public TemporaryDataStorage DataStorage { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the file name where the data is stored.
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// Gets or sets a collection which holds the data in the memory.
        /// </summary>
        public List<string> Data { get; set; }

        #endregion


        #region Constructor

        public TemporaryDataHolder(string field, TemporaryDataStorage dataStorage)
        {
            this.Field = field;
            this.DataStorage = dataStorage;

            //this.FileName = Path.GetTempFileName();
            this.Data = new List<string>();
        }

        #endregion


        #region Methods

        public void Dispose()
        {
            if (this.FileName != null && File.Exists(this.FileName))
                File.Delete(this.FileName);

            this.Data.Clear();
        }


        public void Add(string item)
        {
            switch (this.DataStorage)
            {
                case TemporaryDataStorage.Memory:

                    this.Data.Add(item);

                    break;
                case TemporaryDataStorage.File:

                    File.AppendAllText(
                        this.FileName,
                        item + Environment.NewLine
                    );

                    break;
            }
        }

        public string[] Get()
        {
            List<string> items = new List<string>();

            if (File.Exists(this.FileName))
            {
                string content = File.ReadAllText(this.FileName);

                items = content.Split('\n').ToList();
            }

            items.AddRange(this.Data);

            return items.ToArray();
        }

        #endregion
    }

    public enum TemporaryDataStorage
    {
        Memory,
        File
    }
}
