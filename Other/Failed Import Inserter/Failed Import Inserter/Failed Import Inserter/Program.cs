using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Failed_Import_Inserter
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            string directoryName = ConfigurationManager.AppSettings["DirectoryPath"];

            string[] files = Directory.GetFiles(directoryName);

            string query;

            int i = 0;
            foreach (string file in files)
            {
                FileInfo fInfo = new FileInfo(file);

                Guid idVariable = Guid.Parse(fInfo.Name.Replace(".txt", ""));

                Console.WriteLine(string.Format(
                    "Processing variable {0} ({1}/{2})",
                    idVariable,
                    i++,
                    files.Length
                ));

                ExecuteQuery(connectionString, string.Format(
                    "DELETE FROM [resp].[Var_{0}];",
                    idVariable
                ));

                query = File.ReadAllText(file);

                if (query.Trim() == "")
                    continue;

                ExecuteQuery(connectionString, query);
            }

            Console.WriteLine("Done");

            Console.ReadLine();
        }

        static void ExecuteQuery(string connectionString, string query)
        {
            // Get a new database provider factory.
            DbProviderFactory factory = DbProviderFactories.
                GetFactory("System.Data.SqlClient");

            // Create a new database connection.
            DbConnection connection = factory.CreateConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = connectionString;

            // Create a new database command.
            DbCommand command = factory.CreateCommand();

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = query;

            connection.Open();

            try
            {
                // Execute a database reader on the command.
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();

                command.Dispose();
                connection.Dispose();
            }
        }
    }
}
