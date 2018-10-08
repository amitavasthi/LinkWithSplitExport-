using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoberInsert
{
    class Program
    {
        static string ConnectionStringOld = "";
        static string ConnectionStringNew = "";

        static void Main(string[] args)
        {
            List<Dictionary<string, object>> variables = GetFromDb("SELECT * FROM Variables", ConnectionStringOld);

            foreach (Dictionary<string, object> variable in variables)
            {
                List<Dictionary<string, object>> test = GetFromDb("SELECT * FROM Variables WHERE Name='" + variable["Name"] + "'", ConnectionStringNew);

                if (test.Count == 0)
                {
                    string chapterName = (string)ExecuteScalar("SELECT Name FROM Chapters WHERE Id=" + variable["IdChapter"], ConnectionStringOld);
                    int newChapterId = (int)ExecuteScalar("SELECT Id FROM Chapters WHERE Name='" + chapterName + "'", ConnectionStringNew);

                    string rangeExpression = "NULL";
                    string formula = "NULL";

                    if (variable["RangeExpression"] != DBNull.Value)
                        rangeExpression = (string)variable["RangeExpression"];

                    if (variable["Formula"] != DBNull.Value)
                        rangeExpression = (string)variable["Formula"];

                    ExecuteOnNewDb(string.Format(
                        "INSERT INTO Variables (Name, IdChapter, [Type], RangeExpression, Formula, ClearText, AdditionalInfo, ReportFilter, ReportVariable, ScaleType, ChapterOrder, VariableOrderInChapter) "+
                        "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')",
                        variable["Name"],
                        newChapterId,
                        variable["Type"],
                        rangeExpression,
                        formula,
                        variable["ClearText"],
                        variable["AdditionalInfo"],
                        variable["ReportFilter"],
                        variable["ReportVariable"],
                        variable["ScaleType"],
                        variable["ChapterOrder"],
                        variable["VariableOrderInChapter"]
                    ));
                }

                List<Dictionary<string, object>> responses = GetFromDb("SELECT * FROM resp.Var_" + variable["Id"], ConnectionStringOld);

                int idNewVariable = (int)ExecuteScalar("SELECT Id FROM Variables WHERE Name='" + variable["Name"] + "'", ConnectionStringNew);

                foreach (Dictionary<string, object> response in responses)
                {
                    string categoryName = (string)ExecuteScalar("SELECT Name FROM Categories WHERE Id=" + response["IdCategory"], ConnectionStringOld);

                    int idNewCategory = (int)ExecuteScalar("SELECT Id FROM Categories WHERE IdVariable=" + idNewVariable + " AND Name='" + categoryName + "'", ConnectionStringNew);

                    List<Dictionary<string, object>> oldRespondent = GetFromDb("SELECT OriginalRespondentID, Origin FROM Respondents WHERE Id=" + response["IdRespondent"], ConnectionStringOld);

                    int idNewRespondent = (int)ExecuteScalar("SELECT Id FROM Respondents WHERE OriginalRespondentID='" + oldRespondent[0]["OriginalRespondentID"] + "' AND Origin='" + oldRespondent[0]["Origin"] + "'", ConnectionStringNew);
                    int idProject = (int)response["IdProject"] - 27;
                    int idHLevelSlice = (int)response["IdHLevelSlice"] - 105;
                    int idVariableLink = (int)response["IdVariableLink"];

                    string numericAnswer = "NULL";
                    string textAnswer = "NULL";

                    if (response["NumericAnswer"] != DBNull.Value)
                        numericAnswer = "'" + response["NumericAnswer"].ToString() + "'";

                    if (response["TextAnswer"] != DBNull.Value)
                        numericAnswer = "'" + response["TextAnswer"].ToString() + "'";

                    ExecuteOnNewDb(string.Format(
                        "INSERT INTO resp.Var_" + idNewVariable + " (IdRespondent, IdProject, IdHLevelSlice, IdVariableLink, IdCategory, NumericAnswer, TextAnswer) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
                        idNewRespondent,
                        idProject,
                        idHLevelSlice,
                        numericAnswer,
                        textAnswer
                    ));
                }
            }
        }


        private static List<Dictionary<string, object>> GetFromDb(string commandText, string connectionString)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            // Create a new database connection.
            SqlConnection connection = new SqlConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = connectionString;

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

        private static object ExecuteScalar(string commandText, string connectionString)
        {
            // Create a new database connection.
            SqlConnection connection = new SqlConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = connectionString;

            // Create a new database command.
            SqlCommand command = new SqlCommand();

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = commandText;

            connection.Open();

            object result = command.ExecuteScalar();

            connection.Close();

            command.Dispose();
            connection.Dispose();

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

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = commandText;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();

            command.Dispose();
            connection.Dispose();
        }
    }
}
