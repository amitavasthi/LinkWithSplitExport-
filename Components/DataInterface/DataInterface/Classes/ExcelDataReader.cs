using ApplicationUtilities.Classes;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataInterface.Classes
{
    public class ExcelDataReader : BaseClasses.BaseReader
    {
        #region Properties

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
        public Dictionary<Guid, Dictionary<string, Guid>> Categories { get; set; }

        #endregion


        #region Constructor

        public ExcelDataReader(string fileName, DatabaseCore.Core core, Study study, string respondentVariable, string createResponsesFile, int idLanguage)
            : base(fileName, core, study, createResponsesFile, idLanguage)
        {
            this.RespondentVariable = respondentVariable;
            this.Respondents = new Dictionary<string, Guid>();
            this.Variables = new Dictionary<string, Variable>();
            this.Categories = new Dictionary<Guid, Dictionary<string, Guid>>();
        }

        #endregion


        #region Methods

        public override Variable[] Read()
        {
            ExcelReader reader = new ExcelReader(this.FileName);

            // Create a new list of variables for the result.
            List<Variable> result = new List<Variable>();

            base.Status = BaseClasses.DataImportStatus.Step4;
            base.Progress = 0;

            Dictionary<string, Dictionary<string, int>> variables = new Dictionary<string, Dictionary<string, int>>();

            int columnCount = 0;
            while (true)
            {
                if (string.IsNullOrEmpty(reader[columnCount]))
                    break;

                columnCount++;
            }

            int respondentVariableIndex = 0;

            int lineCount = 0;

            while (reader.Read())
            {
                for (int i = 0; i < columnCount; i++)
                {
                    string variableName = reader[0, i];

                    if (variableName == this.RespondentVariable)
                        respondentVariableIndex = i;

                    if (string.IsNullOrEmpty(reader[0]))
                        break;

                    string category = reader[i];

                    if (string.IsNullOrEmpty(category))
                        continue;

                    if (!variables.ContainsKey(variableName))
                        variables.Add(variableName, new Dictionary<string, int>());

                    if (variables[variableName].ContainsKey(category))
                        variables[variableName][category]++;
                    else
                        variables[variableName].Add(category, 1);
                }

                lineCount++;
            }

            base.Status = BaseClasses.DataImportStatus.Step4;
            base.Progress = 0;

            int variableOrder = 0;
            foreach (string variableName in variables.Keys)
            {
                VariableType variableType = VariableType.Single;

                /*if (variableName.Contains("LiNK_Var_Type_"))
                {
                    variableType = (VariableType)Enum.Parse(
                        typeof(VariableType),
                        variableName.Split(
                    );
                }*/

                //int typeCheck = variables[variableName].Values.Count(x => x > 1);
                int test = variables[variableName].Keys.Count * 10 / 100;
                int typeCheck = variables[variableName].Values.Count(x => x > test);

                if (typeCheck == 0)
                    variableType = VariableType.Text;

                bool isNumeric = true;

                foreach (string category in variables[variableName].Keys)
                {
                    double value;
                    if (double.TryParse(category, out value) == false && category != "")
                    {
                        isNumeric = false;
                        break;
                    }
                    if (category.Contains(" "))
                    {
                        variableType = VariableType.Text;
                        break;
                    }
                }

                if (isNumeric)
                    variableType = VariableType.Numeric;

                // Create a new variable.
                Variable variable = new Variable(base.Core.Variables);

                // Set the variable's project id.
                variable.IdStudy = base.Study.Id;

                // Set the variable's values.
                variable.Name = variableName.Replace(" ", "");
                variable.Type = variableType;
                variable.Order = variableOrder++;

                // Lock the insert action.
                lock (this)
                {
                    // Insert the new variable into the database.
                    variable.Insert();
                }

                // Create a new variable label.
                VariableLabel variableLabel = new VariableLabel(this.Core.VariableLabels);
                variableLabel.Label = variableName;
                variableLabel.IdLanguage = 2057;
                variableLabel.IdVariable = variable.Id;

                // Lock the insert action.
                lock (this)
                {
                    // Insert the new variable label into the database.
                    variableLabel.Insert();
                }

                this.Categories.Add(variable.Id, new Dictionary<string, Guid>());

                if (variable.Type != VariableType.Numeric && variable.Type != VariableType.Text)
                {
                    int categoryOrder = 0;

                    // Run through all parsed categories of the variable.
                    foreach (string categoryNames in variables[variableName].Keys)
                    {
                        foreach (string categoryName in categoryNames.Split(','))
                        {
                            if (this.Categories[variable.Id].ContainsKey(categoryName.Replace(" ", "_")))
                                continue;

                            // Create a new category.
                            Category category = new Category(base.Core.Categories);

                            // Set the category's variable id.
                            category.IdVariable = variable.Id;

                            // Set the category's values.
                            category.Name = categoryName.Replace(" ", "_");

                            //category.Factor = (categoryOrder + 1);
                            category.Value = (categoryOrder + 1);
                            category.Order = categoryOrder++;

                            // Lock the insert action.
                            lock (this)
                            {
                                // Insert the new variable into the database.
                                category.Insert();
                            }

                            // Create a new category label.
                            CategoryLabel categoryLabel = new CategoryLabel(this.Core.CategoryLabels);
                            categoryLabel.Label = categoryName;
                            categoryLabel.IdLanguage = 2057;
                            categoryLabel.IdCategory = category.Id;

                            // Lock the insert action.
                            lock (this)
                            {
                                // Insert the new category label into the database.
                                categoryLabel.Insert();
                            }

                            this.Categories[variable.Id].Add(category.Name, category.Id);
                        }
                    }
                }

                this.Variables.Add(variable.Name, variable);

                // Calculate the current reading progress.
                base.Progress = (int)(variableOrder * 100 / variables.Count);
            }

            base.Progress = 100;

            base.Status = BaseClasses.DataImportStatus.Step5;
            base.Progress = 0;

            StringBuilder bulkInsertBuilder = new StringBuilder();

            int insertSteps = lineCount / 100;

            if (insertSteps == 0)
                insertSteps = 1;

            reader.Position = 0;
            while (reader.Read())
            {
                if (string.IsNullOrEmpty(reader[respondentVariableIndex]))
                    break;

                string respondentId = reader[respondentVariableIndex];

                Respondent respondent = new Respondent(this.Core.Respondents);
                respondent.OriginalRespondentID = respondentId;
                respondent.IdStudy = this.Study.Id;

                //respondent.Insert();
                bulkInsertBuilder.Append(respondent.RenderInsertQuery());
                bulkInsertBuilder.Append(Environment.NewLine);

                this.Respondents.Add(respondentId, respondent.Id);

                if (reader.Position % insertSteps == 0)
                {
                    this.Core.Respondents.ExecuteQuery(bulkInsertBuilder.ToString());
                    bulkInsertBuilder = new StringBuilder();

                    base.Progress = reader.Position * 100 / lineCount;
                }
            }

            if (bulkInsertBuilder.Length > 0)
                this.Core.Respondents.ExecuteQuery(bulkInsertBuilder.ToString());

            base.ResponseInsertStarted = DateTime.Now;
            base.Status = BaseClasses.DataImportStatus.Step6;
            base.Progress = 0;

            reader.Position = 0;

            List<Guid> createdResponsesTables = new List<Guid>();

            Dictionary<string, StringBuilder> responseBulkInsertBuilder = new Dictionary<string, StringBuilder>();

            while (reader.Read())
            {
                for (int i = 0; i < columnCount; i++)
                {
                    string variableName = reader[0, i].Replace(" ", "");

                    if (!this.Variables.ContainsKey(variableName))
                        continue;

                    if (!responseBulkInsertBuilder.ContainsKey(variableName))
                        responseBulkInsertBuilder.Add(variableName, new StringBuilder());

                    if (!createdResponsesTables.Contains(this.Variables[variableName].Id))
                    {
                        // Create the responses database table for the variable.
                        CreateResponsesTable(this.Variables[variableName]);

                        createdResponsesTables.Add(this.Variables[variableName].Id);
                    }

                    if (string.IsNullOrEmpty(reader[0]))
                        break;

                    string[] values = new string[] { reader[i].Replace(" ", "_") };

                    if (this.Variables[variableName].Type == VariableType.Single ||
                        this.Variables[variableName].Type == VariableType.Multi)
                    {
                        values = values[0].Split(',');
                    }

                    foreach (string value in values)
                    {

                        Response response = new Response(this.Core.Responses[this.Variables[variableName].Id]);
                        response.IdRespondent = this.Respondents[reader[respondentVariableIndex]];
                        response.IdStudy = this.Study.Id;

                        switch (this.Variables[variableName].Type)
                        {
                            case VariableType.Text:
                                response.TextAnswer = value;
                                break;
                            case VariableType.Single:
                            case VariableType.Multi:

                                if (!this.Categories[this.Variables[variableName].Id].ContainsKey(value))
                                    continue;

                                response.IdCategory = this.Categories[this.Variables[variableName].Id][value];

                                break;
                            case VariableType.Numeric:
                                decimal numericValue;

                                if (decimal.TryParse(value, out numericValue))
                                {
                                    response.NumericAnswer = numericValue;
                                }
                                break;
                        }

                        //response.Insert();
                        responseBulkInsertBuilder[variableName].Append(response.RenderInsertQuery());
                        responseBulkInsertBuilder[variableName].Append(Environment.NewLine);

                    }
                }

                //base.Progress = reader.Position * 100 / lineCount;
                if (reader.Position % insertSteps == 0)
                {
                    foreach (string variableName in responseBulkInsertBuilder.Keys.ToList())
                    {
                        if (responseBulkInsertBuilder[variableName].Length > 0)
                            this.Core.Responses[this.Variables[variableName].Id].ExecuteQuery(
                                responseBulkInsertBuilder[variableName].ToString()
                            );

                        responseBulkInsertBuilder[variableName] = new StringBuilder();
                    }

                    if (lineCount != 0)
                        base.Progress = reader.Position * 100 / lineCount;
                }
            }

            foreach (string variableName in responseBulkInsertBuilder.Keys.ToList())
            {
                if (responseBulkInsertBuilder[variableName].Length == 0)
                    continue;

                this.Core.Responses[this.Variables[variableName].Id].ExecuteQuery(
                    responseBulkInsertBuilder[variableName].ToString()
                );

                responseBulkInsertBuilder[variableName] = new StringBuilder();
            }

            CreateStudyLinkVariable(reader, respondentVariableIndex);

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

        private void CreateStudyLinkVariable(ExcelReader reader, int respondentVariableIndex)
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

            this.Categories.Add(variable.Id, new Dictionary<string, Guid>());

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

            this.Categories[variable.Id].Add(category.Name, category.Id);
            this.Variables.Add(variable.Name, variable);

            CreateResponsesTable(variable);

            reader.Position = 0;

            StringBuilder bulkInsertBuilder = new StringBuilder();

            int insertSteps = this.Respondents.Count / 100;

            if (insertSteps == 0)
                insertSteps = 1;

            while (reader.Read())
            {
                if (string.IsNullOrEmpty(reader[0]))
                    break;

                Response response = new Response(this.Core.Responses[this.Variables[variable.Name].Id]);
                response.IdRespondent = this.Respondents[reader[respondentVariableIndex]];
                response.IdStudy = this.Study.Id;

                response.IdCategory = category.Id;

                bulkInsertBuilder.Append(response.RenderInsertQuery());
                bulkInsertBuilder.Append(Environment.NewLine);

                if (reader.Position % insertSteps == 0)
                {
                    this.Core.Respondents.ExecuteQuery(bulkInsertBuilder.ToString());
                    bulkInsertBuilder = new StringBuilder();
                }
            }

            if (bulkInsertBuilder.Length > 0)
                this.Core.Respondents.ExecuteQuery(bulkInsertBuilder.ToString());
        }

        public override List<string> Validate()
        {
            base.Status = BaseClasses.DataImportStatus.Step3;

            List<string> result = new List<string>();

            ExcelReader reader = null;

            try
            {
                reader = new ExcelReader(this.FileName);
            }
            catch (Exception e)
            {
                result.Add(e.Message);
            }

            if (reader != null)
            {
                bool found = false;
                int i = 0;

                while (true)
                {
                    if (reader[i] == this.RespondentVariable)
                    {
                        found = true;
                        break;
                    }

                    if (string.IsNullOrEmpty(reader[i]))
                        break;

                    i++;
                }

                if (!found)
                {
                    result.Add(string.Format(
                        "The defined respondent variable '{0}' wasn't found in the data file.",
                        this.RespondentVariable
                    ));
                }
            }

            return result;
        }

        #endregion
    }
}
