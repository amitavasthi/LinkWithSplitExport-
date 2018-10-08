using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCore.Items;
using System.Xml;
using System.IO;

namespace DataInterface.Classes
{
    public class LdfReader : BaseClasses.BaseReader
    {
        #region Properties

        public XmlDocument Document { get; private set; }

        #endregion


        #region Constructor

        public LdfReader(string fileName, DatabaseCore.Core core, Study project,
            string createResponsesFile, int idLanguage)
            : base(fileName, core, project, createResponsesFile, idLanguage)
        {

        }

        #endregion


        #region Methods

        public override Variable[] Read()
        {
            if (this.Document == null)
                throw new Exception("File not validated yet.");

            Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
            Dictionary<string, Dictionary<string, Guid>> categories = new Dictionary<string, Dictionary<string, Guid>>();

            base.Status = BaseClasses.DataImportStatus.Step4;
            base.Progress = 0;

            XmlNodeList xmlNodes = this.Document.DocumentElement.SelectNodes("Variables/Variable");

            Variable variable;
            VariableLabel variableLabel;
            Category category;
            CategoryLabel categoryLabel;
            int i = 0;
            StringBuilder query = new StringBuilder();
            foreach (XmlNode xmlNode in xmlNodes)
            {
                variable = new Variable(base.Core.Variables);
                variable.Name = xmlNode.Attributes["Name"].Value;
                variable.IdStudy = base.Study.Id;
                variable.Type = (VariableType)Enum.Parse(
                    typeof(VariableType),
                    xmlNode.Attributes["Type"].Value
                );

                variable.Insert();

                variableLabel = new VariableLabel(base.Core.VariableLabels);
                variableLabel.IdVariable = variable.Id;
                variableLabel.Label = xmlNode.SelectSingleNode("Label").InnerText;
                variableLabel.IdLanguage = base.IdLanguage;

                variableLabel.Insert();

                if (!variables.ContainsKey(variable.Name))
                {
                    variables.Add(variable.Name, variable);
                    categories.Add(variable.Name, new Dictionary<string, Guid>());
                }

                // Create the responses database table for the variable.
                CreateResponsesTable(variable);

                query.Clear();

                foreach (XmlNode xmlNodeCategory in xmlNode.ChildNodes)
                {
                    if (xmlNodeCategory.Name != "Category")
                        continue;

                    category = new Category(base.Core.Categories);
                    category.IdVariable = variable.Id;
                    category.Name = xmlNodeCategory.Attributes["Name"].Value;

                    query.Append(category.RenderInsertQuery());
                    //category.Insert();

                    if (!categories[variable.Name].ContainsKey(category.Name))
                    {
                        categories[variable.Name].Add(category.Name, category.Id);
                    }

                    categoryLabel = new CategoryLabel(base.Core.CategoryLabels);
                    categoryLabel.IdCategory = category.Id;
                    categoryLabel.Label = xmlNodeCategory.InnerText;
                    categoryLabel.IdLanguage = base.IdLanguage;

                    //categoryLabel.Insert();
                    query.Append(categoryLabel.RenderInsertQuery());

                    if (query.Length >= 200000)
                    {
                        base.Core.Categories.ExecuteQuery(query.ToString());
                        query.Clear();
                    }
                }

                if (query.Length > 0)
                    base.Core.Categories.ExecuteQuery(query.ToString());

                // Calculate the current reading progress.
                base.Progress = (int)(i++ * 100 / xmlNodes.Count);
            }

            base.Progress = 100;

            base.Status = BaseClasses.DataImportStatus.Step5;
            base.Progress = 0;

            Dictionary<string, Guid> respondents = new Dictionary<string, Guid>();
            query.Clear();
            xmlNodes = this.Document.DocumentElement.SelectNodes("Respondents/Respondent");

            foreach (XmlNode xmlNode in xmlNodes)
            {
                Respondent respondent = new Respondent(base.Core.Respondents);
                respondent.IdStudy = base.Study.Id;
                respondent.OriginalRespondentID = xmlNode.Attributes["Id"].Value;

                query.Append(respondent.RenderInsertQuery());

                if (!respondents.ContainsKey(respondent.OriginalRespondentID))
                    respondents.Add(respondent.OriginalRespondentID, respondent.Id);

                if (query.Length >= 200000)
                {
                    base.Core.Respondents.ExecuteQuery(query.ToString());
                    query.Clear();
                }
            }

            if (query.Length > 0)
                base.Core.Respondents.ExecuteQuery(query.ToString());

            base.Status = BaseClasses.DataImportStatus.Step6;
            base.Progress = 0;

            query.Clear();

            xmlNodes = this.Document.DocumentElement.SelectNodes("Responses/Response");

            i = 0;
            Variable v;
            foreach (XmlNode xmlNode in xmlNodes)
            {
                v = variables[xmlNode.Attributes["Variable"].Value];

                if (v.Type == VariableType.Numeric)
                {
                    query.Append(string.Format(
                        "INSERT INTO [resp].[Var_{0}] (Id, IdRespondent, IdStudy, NumericAnswer) VALUES (NEWID(), '{1}', '{2}', '{3}')" + Environment.NewLine,
                        v.Id,
                        respondents[xmlNode.Attributes["Respondent"].Value],
                        base.Study.Id,
                        double.Parse(xmlNode.Attributes["Category"].Value)
                    ));
                }
                else if(v.Type == VariableType.Text)
                {
                    query.Append(string.Format(
                        "INSERT INTO [resp].[Var_{0}] (Id, IdRespondent, IdStudy, TextAnswer) VALUES (NEWID(), '{1}', '{2}', '{3}')" + Environment.NewLine,
                        v.Id,
                        respondents[xmlNode.Attributes["Respondent"].Value],
                        base.Study.Id,
                        xmlNode.Attributes["Category"].Value.Replace("'", "''")
                    ));
                }
                else
                {
                    query.Append(string.Format(
                        "INSERT INTO [resp].[Var_{0}] (Id, IdRespondent, IdStudy, IdCategory) VALUES (NEWID(), '{1}', '{2}', '{3}')" + Environment.NewLine,
                        v.Id,
                        respondents[xmlNode.Attributes["Respondent"].Value],
                        base.Study.Id,
                        categories[xmlNode.Attributes["Variable"].Value][xmlNode.Attributes["Category"].Value]
                    ));
                }

                if (xmlNodes.Count <= 100 || i % (xmlNodes.Count / 100) == 0)
                {
                    base.Core.Respondents.ExecuteQuery(query.ToString());
                    query.Clear();

                    base.Progress = ((i++ * 100) / (double)xmlNodes.Count);
                }
            }

            if (query.Length != 0)
            {
                base.Core.Respondents.ExecuteQuery(query.ToString());
                query.Clear();
            }

            base.Progress = 100;

            return variables.Values.ToArray();
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

        public override List<string> Validate()
        {
            List<string> result = new List<string>();

            try
            {
                this.Document = new XmlDocument();
                this.Document.Load(base.FileName);
            }
            catch (Exception ex)
            {
                result.Add("The file was not identified as a valid LiNK Data File (ldf). " + ex.Message);
            }

            return result;
        }

        #endregion


        #region Event Handlers

        #endregion
    }
}
