using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Merge
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string[]> tables = new List<string[]>();
            tables.Add(new string[] { "dbo", "Studies" });
            tables.Add(new string[] { "dbo", "Variables" });
            tables.Add(new string[] { "dbo", "VariableLabels" });
            tables.Add(new string[] { "dbo", "Categories" });
            tables.Add(new string[] { "dbo", "CategoryLabels" });
            tables.Add(new string[] { "dbo", "Respondents" });
            tables.Add(new string[] { "dbo", "Hierarchies" });
            tables.Add(new string[] { "dbo", "TaxonomyChapters" });
            tables.Add(new string[] { "dbo", "TaxonomyChapterLabels" });
            tables.Add(new string[] { "dbo", "TaxonomyVariables" });
            tables.Add(new string[] { "dbo", "TaxonomyVariableLabels" });
            tables.Add(new string[] { "dbo", "TaxonomyCategories" });
            tables.Add(new string[] { "dbo", "TaxonomyCategoryLabels" });
            tables.Add(new string[] { "dbo", "TaxonomyCategoryLinks" });
            tables.Add(new string[] { "dbo", "VariableLinks" });
            tables.Add(new string[] { "dbo", "CategoryLinks" });
            tables.Add(new string[] { "dbo", "TaxonomyVariableHierarchies" });
            tables.Add(new string[] { "dbo", "TaxonomyCategoryHierarchies" });

            List<Dictionary<string, object>> responseTables = Select(
                ConfigurationManager.AppSettings["SourceDatabaseConnectionString"],
                "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA='resp' ORDER BY TABLE_NAME"
            );

            foreach (string[] table in tables)
            {
                bool result = CopyTable(
                    table[0] + ".[" + table[1] + "]"
                );

                if (result)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(table[0] + ".[" + table[1] + "]" + " merged successfully.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(table[0] + ".[" + table[1] + "]" + " failed.");
                }
            }

            foreach (Dictionary<string, object> table in responseTables)
            {
                bool result = true;

                try {
                    CreateResponsesTable(
                        ConfigurationManager.AppSettings["DestinationDatabaseConnectionString"],
                        (string)table.ElementAt(0).Value
                    );

                    result = CopyTable(
                        "resp" + ".[" + table.ElementAt(0).Value + "]"
                    );
                }
                catch
                {
                    result = false;
                }

                if (result)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("resp" + ".[" + table.ElementAt(0).Value + "]" + " merged successfully.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("resp" + ".[" + table.ElementAt(0).Value + "]" + " failed.");
                }
            }
        }

        static bool CopyTable(string tableName)
        {
            string query = "";

            if (!tableName.StartsWith("resp."))
            {
                List<Dictionary<string, object>> destinationColumns = Select(
                    ConfigurationManager.AppSettings["DestinationDatabaseConnectionString"],
                    string.Format("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{0}'", tableName.Split('.')[1].Replace("[", "").Replace("]", ""))
                );
                List<string> sourceColumns = Select(
                    ConfigurationManager.AppSettings["SourceDatabaseConnectionString"],
                    string.Format("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{0}'", tableName.Split('.')[1].Replace("[", "").Replace("]", ""))
                ).Select(x=>(string)x.ElementAt(0).Value).ToList();

                List<string> columns = new List<string>();

                foreach (string column in destinationColumns.Select(x=>(string)x.ElementAt(0).Value))
                {
                    if (sourceColumns.Contains(column))
                        columns.Add("[" + column + "]");
                }

                query = string.Format(
                    "SELECT {1} FROM {0}",
                    tableName,
                    string.Join(",", columns)
                );
            }
            else
            {
                query = string.Format("SELECT * FROM {0}", tableName);
            }

            List<Dictionary<string, object>> sourceData = Select(
                ConfigurationManager.AppSettings["SourceDatabaseConnectionString"],
                query
            );

            StringBuilder buildInsertBuilder = new StringBuilder();

            foreach (Dictionary<string, object> item in sourceData)
            {
                foreach (string column in item.Keys.ToArray())
                {
                    if (item[column] == DBNull.Value)
                    {
                        item[column] = null;
                    }
                    else if (item[column].GetType() == typeof(DateTime))
                    {
                        item[column] = ((DateTime)item[column]).ToString(new CultureInfo("en-US"));
                    }
                }

                buildInsertBuilder.Append(string.Format(
                    "INSERT INTO {0} ({1}) VALUES ({2});" + Environment.NewLine,
                    tableName,
                    string.Join(",", item.Keys.Select(x => "[" + x + "]")),
                    string.Join(",", item.Values.Select(x => x != null ? ("'" + x.ToString().Replace("'", "''") + "'") : "NULL"))
                ));
            }

            if (buildInsertBuilder.Length == 0)
                return true;

            return ExecuteQuery(
                ConfigurationManager.AppSettings["DestinationDatabaseConnectionString"],
                buildInsertBuilder.ToString(),
                tableName
            );
        }

        static List<Dictionary<string, object>> Select(string connectionString, string query)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            SqlConnection sqlConnection = new SqlConnection(connectionString);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            sqlConnection.Open();

            Dictionary<string, object> item;
            try
            {
                SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    item = new Dictionary<string, object>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        item.Add(
                            reader.GetName(i),
                            reader.GetValue(i)
                        );
                    }

                    result.Add(item);
                }
            }
            catch
            {

            }
            finally
            {
                sqlCommand.Dispose();
                sqlConnection.Close();

                sqlConnection.Dispose();
            }

            return result;
        }

        static bool ExecuteQuery(string connectionString, string query, string tableName)
        {
            bool result = true;

            SqlConnection sqlConnection = new SqlConnection(connectionString);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            sqlCommand.CommandTimeout = 1200;

            sqlConnection.Open();

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "Logs",
                    tableName + ".txt"
                ), ex.ToString() + "\n\n");

                result = false;
            }
            finally
            {
                sqlCommand.Dispose();
                sqlConnection.Close();

                sqlConnection.Dispose();
            }

            return result;
        }

        static bool CreateResponsesTable(string connectionString, string tableName)
        {
            string query = File.ReadAllText(Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                "CreateResponses.sql"
            ));

            query = string.Format(
                query,
                tableName
            );

            return ExecuteQuery(
                connectionString,
                query,
                tableName
            );
        }
    }
}
