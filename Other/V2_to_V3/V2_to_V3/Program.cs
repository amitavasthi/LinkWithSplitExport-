using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace V2_to_V3
{
    class Program
    {
        static string ConnectionStringOld = "";
        static string ConnectionStringNew = "";

        static Dictionary<Guid, double[]> respondentsWeights;

        static void Main(string[] args)
        {
            ConnectionStringOld = ConfigurationManager.AppSettings["ConnectionStringV2"];
            ConnectionStringNew = ConfigurationManager.AppSettings["ConnectionStringV3"];

            Process();
        }

        static Dictionary<int, Guid> newStudyIds;
        static void Process()
        {
            newStudyIds = new Dictionary<int, Guid>();

            List<Dictionary<string, object>> studies = GetFromOldDb("SELECT * FROM Projects");

            // Run through all studies from the v2 database.
            foreach (Dictionary<string, object> study in studies)
            {
                Guid idNewStudy = Guid.NewGuid();

                ExecuteOnNewDb(string.Format(
                    "INSERT INTO Studies (Id, Name, Description, CreationDate, IdUser, IdHierarchy) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{4}')",
                    idNewStudy,
                    study["Name"],
                    study["Description"],
                    DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                    "00000000-0000-0000-0000-000000000000"
                ));

                newStudyIds.Add(
                    (int)study["Id"],
                    idNewStudy
                );
            }

            Dictionary<long, Guid> newRespondentIds = new Dictionary<long, Guid>();
            respondentsWeights = new Dictionary<Guid, double[]>();

            List<Dictionary<string, object>> respondents = GetFromOldDb("SELECT * FROM Respondents");

            StringBuilder bulkInsertBuilder = new StringBuilder();
            int count = 0;

            foreach (Dictionary<string, object> respondent in respondents)
            {
                Guid newRespondentId = Guid.NewGuid();

                newRespondentIds.Add(
                    long.Parse(respondent["Id"].ToString()),
                    newRespondentId
                );

                double weight1 = 1.0;
                double weight2 = 1.0;

                double.TryParse(respondent["Weight1"].ToString(), out weight1);
                double.TryParse(respondent["Weight2"].ToString(), out weight2);

                bulkInsertBuilder.Append(string.Format(
                    "INSERT INTO Respondents (Id, IdStudy, OriginalRespondentId) VALUES ('{0}', '{1}', '{2}');" + Environment.NewLine,
                    newRespondentId,
                    newStudyIds[(int)respondent["IdProject"]],
                    respondent["OriginalRespondentID"]
                ));

                if (count++ == 40000)
                {
                    count = 0;

                    ExecuteOnNewDb(bulkInsertBuilder.ToString());
                    bulkInsertBuilder.Clear();

                    Console.WriteLine(newRespondentIds.Count + " respondents inserted.");
                }

                respondentsWeights.Add(
                    newRespondentId,
                    new double[] {
                        weight1,
                        weight2
                    }
                );
            }

            if (bulkInsertBuilder.Length > 0)
                ExecuteOnNewDb(bulkInsertBuilder.ToString());

            bulkInsertBuilder.Clear();

            Guid idVariableWeight1 = Guid.NewGuid();
            Guid idVariableWeight2 = Guid.NewGuid();

            CreateWeightingVariable(idVariableWeight1, "Weight1", 0);
            CreateWeightingVariable(idVariableWeight2, "Weight2", 1);

            List<Dictionary<string, object>> taxonomyChapters = GetFromOldDb("SELECT * FROM Chapters");

            Dictionary<long, Guid> taxonomyVariableIds = new Dictionary<long, Guid>();
            Dictionary<int, Guid> taxonomyCategoryIds = new Dictionary<int, Guid>();
            Dictionary<long, int> taxonomyVariableTypes = new Dictionary<long, int>();

            int variableOrder = 0;
            foreach (Dictionary<string, object> taxonomyChapter in taxonomyChapters)
            {
                long oldChapterId = long.Parse(taxonomyChapter["Id"].ToString());
                Guid newChapterId = Guid.NewGuid();

                ExecuteOnNewDb(string.Format(
                    "INSERT INTO TaxonomyChapters (Id, IdHierarchy, Name, CreationDate) VALUES ('{0}', '{1}', '{2}', '{3}')",
                    newChapterId,
                    "00000000-0000-0000-0000-000000000000",
                    taxonomyChapter["Name"],
                    DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                    newChapterId
                ));

                List<Dictionary<string, object>> taxonomyVariables = GetFromOldDb("SELECT * FROM Variables WHERE IdChapter=" + taxonomyChapter["Id"]);

                count = 0;
                foreach (Dictionary<string, object> taxonomyVariable in taxonomyVariables)
                {
                    Console.WriteLine(string.Format(
                        "Processing taxonomy variable in chapter '{0}': {1}/{2}",
                        taxonomyChapter["Name"],
                        count++,
                        taxonomyVariables.Count
                    ));

                    long oldVariableId = long.Parse(taxonomyVariable["Id"].ToString());
                    Guid newVariableId = Guid.NewGuid();

                    int variableType = 0;

                    switch ((byte)taxonomyVariable["IdVariableType"])
                    {
                        case 1:
                            variableType = 3;
                            break;
                        case 2:
                            variableType = 6;
                            break;
                        case 4:
                            variableType = 4;
                            break;
                    }

                    ExecuteOnNewDb(string.Format(
                        "INSERT INTO TaxonomyVariables (Id, [Type], Name, CreationDate, IdTaxonomyChapter) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                        newVariableId,
                        variableType,
                        taxonomyVariable["Name"],
                        DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                        newChapterId
                    ));

                    List<Dictionary<string, object>> variableLabels = GetFromOldDb("SELECT * FROM VariableLabels WHERE IdVariable=" + oldVariableId);

                    foreach (Dictionary<string, object> variableLabel in variableLabels)
                    {
                        ExecuteOnNewDb(string.Format(
                            "INSERT INTO TaxonomyVariableLabels (Id, IdTaxonomyVariable, IdLanguage, Label, CreationDate) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                            Guid.NewGuid(),
                            newVariableId,
                            1031,
                            variableLabel["Label"].ToString().Replace("'", "''"),
                            DateTime.Now.ToString("yyyy/MM/dd HH:mm")
                        ));
                    }

                    taxonomyVariableIds.Add(oldVariableId, newVariableId);
                    taxonomyVariableTypes.Add(oldVariableId, variableType);

                    List<Dictionary<string, object>> categories = GetFromOldDb("SELECT * FROM Categories WHERE IdVariable=" + oldVariableId);

                    int categoryOrder = 0;
                    foreach (Dictionary<string, object> category in categories)
                    {
                        Guid newCategoryId = Guid.NewGuid();

                        ExecuteOnNewDb(string.Format(
                            "INSERT INTO TaxonomyCategories (Id, IdTaxonomyVariable, Name, [Order], CreationDate, Value) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                            newCategoryId,
                            newVariableId,
                            HttpUtility.HtmlEncode(category["Name"]),
                            categoryOrder++,
                            DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                            category["Factor"]
                        ));

                        taxonomyCategoryIds.Add((int)category["Id"], newCategoryId);

                        List<Dictionary<string, object>> categoryLabels = GetFromOldDb("SELECT * FROM CategoryLabels WHERE IdCategory=" + category["Id"]);

                        foreach (Dictionary<string, object> categoryLabel in categoryLabels)
                        {
                            ExecuteOnNewDb(string.Format(
                                "INSERT INTO TaxonomyCategoryLabels (Id, IdTaxonomyCategory, IdLanguage, Label, CreationDate) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                                Guid.NewGuid(),
                                newCategoryId,
                                1031,
                                categoryLabel["Label"].ToString().Replace("'", "''"),
                                DateTime.Now.ToString("yyyy/MM/dd HH:mm")
                            ));
                        }
                    }
                }
            }

            List<Dictionary<string, object>> variables = GetFromOldDb("SELECT * FROM OriginalVariables");

            int variablesLength = variables.Count;
            int variableCount = 0;

            variableOrder = 0;
            foreach (Dictionary<string, object> variable in variables)
            {
                long oldVariableId = long.Parse(variable["Id"].ToString());
                Guid newVariableId = Guid.NewGuid();

                ExecuteOnNewDb("CREATE TABLE [resp].[Var_" + newVariableId + "](" +
                    "[Id] [uniqueidentifier] NOT NULL," +
                    "[IdRespondent] [uniqueidentifier] NOT NULL," +
                    "[IdStudy] [uniqueidentifier] NOT NULL," +
                    "[IdCategory] [uniqueidentifier] NULL," +
                    "[NumericAnswer] [float] NULL," +
                    "[TextAnswer] [nvarchar](4000) NULL " +
                    "PRIMARY KEY CLUSTERED " +
                    "(" +
                        "[Id] ASC" +
                    ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                    ") ON [PRIMARY]"
                );

                variable["Id"] = newVariableId;
                //variable.Add("IdStudy", new Guid());

                ExecuteOnNewDb(string.Format(
                    "INSERT INTO Variables (Id, IdStudy, Name, [Type], [Order]) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                    newVariableId,
                    newStudyIds[(int)variable["IdProject"]],
                    variable["Name"],
                    4,
                    variableOrder++
                ));

                int idVariable = 0;
                bool linkedVariable = false;

                ExecuteOnNewDb(string.Format(
                    "INSERT INTO VariableLabels (Id, IdVariable, IdLanguage, Label, ReportLabel) VALUES ('{0}', '{1}', '{2}', '{4}', '{5}')",
                    Guid.NewGuid(),
                    newVariableId,
                    1031,
                    new Guid(),
                    variable["Name"],
                    variable["Name"]
                ));

                List<Dictionary<string, object>> categories = GetFromOldDb("SELECT * FROM OriginalCategories WHERE IdOriginalVariable=" + oldVariableId);

                int categoryOrder = 0;
                foreach (Dictionary<string, object> category in categories)
                {
                    int idCategory = 0;
                    Guid newCategoryId = Guid.NewGuid();

                    ExecuteOnNewDb(string.Format(
                        "INSERT INTO Categories (Id, IdVariable, Name, ClearText, Value, [Order]) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                        newCategoryId,
                        newVariableId,
                        category["Name"],
                        category["Name"].ToString().Replace("'", "''"),
                        0,
                        categoryOrder++
                    ));

                    List<Dictionary<string, object>> categoryLinks = GetFromOldDb("SELECT * FROM VariableLinks WHERE IdOriginalCategory=" + category["Id"]);

                    if (categoryLinks.Count > 0)
                    {
                        ExecuteOnNewDb(string.Format(
                            "INSERT INTO CategoryLinks (Id, IdCategory, IdTaxonomyCategory, CreationDate) VALUES ('{0}', '{1}', '{2}', '{3}')",
                            Guid.NewGuid(),
                            newCategoryId,
                            taxonomyCategoryIds[(int)categoryLinks[0]["IdCategory"]],
                            DateTime.Now.ToString("yyyy/MM/dd HH:mm")
                        ));

                        idCategory = (int)categoryLinks[0]["IdCategory"];
                    }

                    if (!linkedVariable)
                    {
                        List<Dictionary<string, object>> variableLinks = GetFromOldDb("SELECT IdVariable FROM Categories WHERE Id=(SELECT TOP 1 IdCategory FROM VariableLinks WHERE IdOriginalCategory=" + category["Id"] + ")");

                        ExecuteOnNewDb(string.Format(
                            "INSERT INTO VariableLinks (Id, IdVariable, IdTaxonomyVariable, CreationDate) VALUES ('{0}', '{1}', '{2}', '{3}')",
                            Guid.NewGuid(),
                            newVariableId,
                            taxonomyVariableIds[(int)variableLinks[0]["IdVariable"]],
                            DateTime.Now.ToString("yyyy/MM/dd HH:mm")
                        ));

                        idVariable = (int)variableLinks[0]["IdVariable"];

                        linkedVariable = true;
                    }

                    List<Dictionary<string, object>> categoryLabels = GetFromOldDb("SELECT * FROM CategoryLabels WHERE IdCategory=" + category["Id"]);

                    ExecuteOnNewDb(string.Format(
                        "INSERT INTO CategoryLabels (Id, IdCategory, IdLanguage, Label) VALUES ('{0}', '{1}', '{2}', '{3}')",
                        Guid.NewGuid(),
                        newCategoryId,
                        1031,
                        category["Name"]
                    ));

                    List<Dictionary<string, object>> responses = GetFromOldDb("SELECT * FROM resp.Var_" + idVariable + " WHERE IdProject=" + variable["IdProject"] + " AND IdCategory=" + idCategory);

                    //StringBuilder xmlBuilder = new StringBuilder();
                    long oldRespondentId;
                    Guid newRespondentId;

                    bulkInsertBuilder.Clear();

                    foreach (Dictionary<string, object> response in responses)
                    {
                        oldRespondentId = long.Parse(response["IdRespondent"].ToString());
                        newRespondentId = newRespondentIds[oldRespondentId];

                        /*xmlBuilder.Append(string.Format(
                            "<Response IdRespondent=\"{0}\" Weight1=\"{1}\" Weight2=\"{2}\"></Response>",
                            newRespondentId,
                            respondentsWeights[newRespondentId][0],
                            respondentsWeights[newRespondentId][1]
                        ));*/

                        string numericAnswer = "NULL";

                        if (response["NumericAnswer"] != DBNull.Value)
                            numericAnswer = "'" + response["NumericAnswer"].ToString() + "'";

                        bulkInsertBuilder.Append(string.Format(
                            "INSERT INTO [resp].[Var_{0}] (Id, IdRespondent, IdStudy, IdCategory, NumericAnswer, TextAnswer) VALUES ('{1}', '{2}', '{3}', '{4}', {5}, NULL)",
                            newVariableId,
                            Guid.NewGuid(),
                            newRespondentId,
                            newStudyIds[(int)response["IdProject"]],
                            newCategoryId,
                            numericAnswer,
                            response["TextAnswer"]
                        ));
                    }

                    if (bulkInsertBuilder.Length > 0)
                        ExecuteOnNewDb(bulkInsertBuilder.ToString());

                    /*File.WriteAllText(Path.Combine(
                        ResponsesFolder,
                        "[resp].[Var_"+ newCategoryId +"].xml"
                    ), xmlBuilder.ToString());

                    xmlBuilder.Clear();*/
                }
                variableCount++;

                Console.WriteLine("Variable " + variableCount + " from " + variablesLength + " processed.");
            }
        }

        static void CreateWeightingVariable(Guid id, string name, int index)
        {
            ExecuteOnNewDb(string.Format(
                "INSERT INTO TaxonomyVariables (Id, [Type], Name, CreationDate) VALUES ('{0}', '{1}', '{2}', '{3}')",
                id,
                6,
                name,
                DateTime.Now.ToString("yyyy/MM/dd HH:mm")
            ));

            ExecuteOnNewDb(string.Format(
                "INSERT INTO TaxonomyVariableLabels (Id, IdTaxonomyVariable, IdLanguage, Label, CreationDate) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                Guid.NewGuid(),
                id,
                1031,
                name,
                DateTime.Now.ToString("yyyy/MM/dd HH:mm")
            ));

            foreach (Guid idStudy in newStudyIds.Values)
            {
                Guid idVariable = Guid.NewGuid();

                ExecuteOnNewDb(string.Format(
                    "INSERT INTO Variables (Id, IdStudy, Name, [Type], [Order]) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                    idVariable,
                    idStudy,
                    name,
                    4,
                    0
                ));

                ExecuteOnNewDb("CREATE TABLE [resp].[Var_" + idVariable + "](" +
                    "[Id] [uniqueidentifier] NOT NULL," +
                    "[IdRespondent] [uniqueidentifier] NOT NULL," +
                    "[IdStudy] [uniqueidentifier] NOT NULL," +
                    "[IdCategory] [uniqueidentifier] NULL," +
                    "[NumericAnswer] [float] NULL," +
                    "[TextAnswer] [nvarchar](4000) NULL " +
                    "PRIMARY KEY CLUSTERED " +
                    "(" +
                        "[Id] ASC" +
                    ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                    ") ON [PRIMARY]"
                );

                ExecuteOnNewDb(string.Format(
                    "INSERT INTO VariableLabels (Id, IdVariable, IdLanguage, Label, ReportLabel) VALUES ('{0}', '{1}', '{2}', '{4}', '{5}')",
                    Guid.NewGuid(),
                    idVariable,
                    1031,
                    new Guid(),
                    name,
                    name
                ));

                ExecuteOnNewDb(string.Format(
                    "INSERT INTO VariableLinks (Id, IdVariable, IdTaxonomyVariable, CreationDate) VALUES ('{0}', '{1}', '{2}', '{3}')",
                    Guid.NewGuid(),
                    idVariable,
                    id,
                    DateTime.Now.ToString("yyyy/MM/dd HH:mm")
                ));

                List<Dictionary<string, object>> respondents = GetFromNewDb("SELECT Id FROM Respondents WHERE IdStudy='" + idStudy + "'");
                StringBuilder builkInsertBuilder = new StringBuilder();


                foreach (Dictionary<string, object> respondent in respondents)
                {
                    builkInsertBuilder.Append(string.Format(
                        "INSERT INTO [resp].[Var_{0}] (Id, IdRespondent, IdStudy, IdCategory, NumericAnswer, TextAnswer) VALUES ('{1}', '{2}', '{3}', NULL, '{4}', NULL)",
                        idVariable,
                        Guid.NewGuid(),
                        (Guid)respondent["Id"],
                        idStudy,
                        respondentsWeights[(Guid)respondent["Id"]][index].ToString(new CultureInfo(2057))
                    ));
                }

                if (builkInsertBuilder.Length > 0)
                    ExecuteOnNewDb(builkInsertBuilder.ToString());

                builkInsertBuilder.Clear();
            }
        }


        private static List<Dictionary<string, object>> GetFromOldDb(string commandText)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            // Create a new database connection.
            SqlConnection connection = new SqlConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = ConnectionStringOld;

            // Create a new database command.
            SqlCommand command = new SqlCommand();

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = commandText;

            connection.Open();

            // Execute a database reader on the command.
            SqlDataReader reader = command.ExecuteReader();

            // Run for each row of the command's select result.
            while (reader.Read())
            {
                Dictionary<string, object> values = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    values.Add(
                        reader.GetName(i),
                        reader.GetValue(i)
                    );
                }

                result.Add(values);
            }

            connection.Close();

            command.Dispose();
            connection.Dispose();
            reader.Dispose();

            return result;
        }

        private static List<Dictionary<string, object>> GetFromNewDb(string commandText)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            // Create a new database connection.
            SqlConnection connection = new SqlConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = ConnectionStringNew;

            // Create a new database command.
            SqlCommand command = new SqlCommand();
            command.CommandTimeout = 3600;

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = commandText;

            connection.Open();

            // Execute a database reader on the command.
            SqlDataReader reader = command.ExecuteReader();

            // Run for each row of the command's select result.
            while (reader.Read())
            {
                Dictionary<string, object> values = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    values.Add(
                        reader.GetName(i),
                        reader.GetValue(i)
                    );
                }

                result.Add(values);
            }

            connection.Close();

            command.Dispose();
            connection.Dispose();
            reader.Dispose();

            return result;
        }

        private static void ExecuteOnNewDb(string commandText)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            // Create a new database connection.
            SqlConnection connection = new SqlConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = ConnectionStringNew;

            // Create a new database command.
            SqlCommand command = new SqlCommand();
            command.CommandTimeout = 3600;

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = commandText;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();

            command.Dispose();
            connection.Dispose();
        }

        private static void InsertOnNewDb(Dictionary<string, object> obj, string tableName)
        {
            string commandText = "INSERT INTO {0} ({1}) VALUES ({2});";

            string names = string.Join(",", obj.Keys);
            string values = "";

            foreach (object value in obj.Values)
            {
                values += "'" + value + "',";
            }

            values = values.Remove(values.Length - 1, 1);

            commandText = string.Format(
                commandText,
                tableName,
                names,
                values
            );

            ExecuteOnNewDb(commandText);
        }
    }
}
