using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ApplicationUtilities
{
    public class UsageLogger
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database provider of the usage database.
        /// </summary>
        public string DatabaseProvider { get; set; }

        /// <summary>
        /// Gets or sets the connection string to the usage database.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        public DatabaseCore.Items.User Respondent { get; set; }

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        public string ClientName { get; set; }
        #endregion


        #region Constructor

        public UsageLogger(string clientName, DatabaseCore.Items.User respondent)
        {
            this.DatabaseProvider = ConfigurationManager.AppSettings["DatabaseProviderUsageLog"];
            this.ConnectionString = ConfigurationManager.AppSettings["ConnectionStringUsageLog"];

            this.ClientName = clientName;
            this.Respondent = respondent;
        }

        #endregion


        #region Methods

        public void Log(UsageLogVariable logVariable, string categoryName)
        {
            try
            {
                Guid idVariable = GetVariable(logVariable.ToString());
                Guid idCategory = GetCategory(idVariable, categoryName);
                Guid idRespondent = GetRespondent();

                ExecuteReader(
                    "INSERT INTO resp.[Var_" + idVariable + "] (Id, IdRespondent, IdStudy, IdCategory) " +
                    "VALUES (NEWID(), {0}, '00000000-0000-0000-0000-000000000000', {1})",
                    idRespondent,
                    idCategory
                );
            }
            catch { }
        }

        public void InitLog(string browser)
        {
            try
            {
                SelectSingle(
                  "INSERT INTO UsageLog ([Id],[IdUser],[UserName],[EmailId],[SessionId],[Login],[Browser],[Client],[Role]) " +
                  "VALUES (NEWID(), {0}, {1}, {2},{3},GETDATE(),{4},{5},{6})",
                  this.Respondent.Id,
                  this.Respondent.Name,
                  this.Respondent.Mail,
                  HttpContext.Current.Session.SessionID,
                  browser,
                  this.ClientName,
                  this.Respondent.Role.Name
              );
            }
            catch { }
        }

        public void UsageLogDetails(string file)
        {
            try
            {
                object[] usageLogId = SelectSingle(
               "SELECT Id FROM UsageLog WHERE SessionId={0} ORDER BY Login",
             HttpContext.Current.Session.SessionID
           );

                if (usageLogId.Length > 0)
                    SelectSingle(
                     "INSERT INTO UsageLogDetails ([Id],[IdUsage],[AccessedPage]) " +
                     "VALUES (NEWID(), {0}, {1})",
                     usageLogId[0],
                     file
                 );
            }
            catch { }
        }

        public void Logout()
        {
            try
            {
                object[] usageLogId = SelectSingle(
             "SELECT Id FROM UsageLog WHERE SessionId={0} ORDER BY Login",
           HttpContext.Current.Session.SessionID
         );
                if (usageLogId.Length > 0)
                    SelectSingle(
                  "UPDATE UsageLog SET Logout=GETDATE() WHERE Id={0}",
                usageLogId[0]
              );
            }
            catch { }
        }
        private Guid GetVariable(string variableName)
        {
            object[] variable = SelectSingle(
                "SELECT Id FROM Variables WHERE Name={0}",
                variableName.ToString()
            );

            if (variable == null)
            {
                Guid idVariable = Guid.NewGuid();

                ExecuteReader(
                    "INSERT INTO [Variables] (Id, IdStudy, Name, Type) VALUES ({0}, '00000000-0000-0000-0000-000000000000', {1}, 4);",
                    idVariable,
                    variableName.ToString()
                );
                ExecuteReader(
                    "INSERT INTO [VariableLabels] (Id, IdVariable, IdLanguage, Label) VALUES (NEWID(), {0}, 2057, {1});",
                    idVariable,
                    variableName.ToString()
                );

                string fileName = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "App_Data",
                    "DataStorage",
                    "CreateResponses.sql"
                );

                ExecuteReader(File.ReadAllText(fileName).Replace("{0}", idVariable.ToString()));

                variable = new object[] { idVariable };
            }

            return (Guid)variable[0];
        }
        private Guid GetCategory(Guid idVariable, string categoryName)
        {
            object[] category = SelectSingle(
                "SELECT Id FROM Categories WHERE IdVariable={0} AND Name={1}",
                idVariable,
                categoryName
            );

            if (category == null)
            {
                Guid idCategory = Guid.NewGuid();

                ExecuteReader(
                    "INSERT INTO [Categories] (Id, IdVariable, Name) VALUES ({0}, {1}, {2});",
                    idCategory,
                    idVariable,
                    categoryName
                );
                ExecuteReader(
                    "INSERT INTO [CategoryLabels] (Id, IdCategory, IdLanguage, Label) VALUES (NEWID(), {0}, 2057, {1});",
                    idCategory,
                    categoryName
                );

                category = new object[] { idCategory };
            }

            return (Guid)category[0];
        }
        private Guid GetRespondent()
        {
            object[] respondent = SelectSingle(
                "SELECT Id FROM Respondents WHERE OriginalRespondentID={0}",
                 this.ClientName + this.Respondent.Id
            );

            if (respondent == null)
            {
                Guid idRespondent = Guid.NewGuid();

                ExecuteReader(
                    "INSERT INTO [Respondents] (Id, IdStudy, OriginalRespondentID) VALUES ({0}, '00000000-0000-0000-0000-000000000000', {1});",
                    idRespondent,
                    this.ClientName + this.Respondent.Id
                );

                respondent = new object[] { idRespondent };

                Guid idVariableClient = GetVariable("Clients");
                Guid idCategoryClient = GetCategory(idVariableClient, this.ClientName);

                ExecuteReader(
                    "INSERT INTO resp.[Var_" + idVariableClient + "] (Id, IdRespondent, IdStudy, IdCategory) " +
                    "VALUES (NEWID(), {0}, '00000000-0000-0000-0000-000000000000', {1})",
                    idRespondent,
                    idCategoryClient
                );

                Guid idVariableUsers = GetVariable("Users");
                Guid idCategoryUsers = GetCategory(idVariableUsers, this.Respondent.Name);

                ExecuteReader(
                    "INSERT INTO resp.[Var_" + idVariableUsers + "] (Id, IdRespondent, IdStudy, IdCategory) " +
                    "VALUES (NEWID(), {0}, '00000000-0000-0000-0000-000000000000', {1})",
                    idRespondent,
                    idCategoryUsers
                );

                string groupName = (string)((DatabaseCore.Core)this.Respondent.Owner.Owner).Roles.GetValue("Name", "Id",
                    (Guid)((DatabaseCore.Core)this.Respondent.Owner.Owner).UserRoles.GetValue("IdRole", "IdUser", this.Respondent.Id)
                );

                Guid idVariableRoles = GetVariable("Roles");
                Guid idCategoryRoles = GetCategory(idVariableRoles, groupName);

                ExecuteReader(
                    "INSERT INTO resp.[Var_" + idVariableRoles + "] (Id, IdRespondent, IdStudy, IdCategory) " +
                    "VALUES (NEWID(), {0}, '00000000-0000-0000-0000-000000000000', {1})",
                    idRespondent,
                    idCategoryRoles
                );
            }

            return (Guid)respondent[0];
        }

        public object[] SelectSingle(string query, params object[] parameters)
        {
            List<object[]> entries = ExecuteReader(query, parameters);

            if (entries.Count == 0)
                return null;

            return entries.First();
        }

        public List<object[]> ExecuteReader(string query, params object[] parameters)
        {
            // Get a new database provider factory.
            DbProviderFactory factory = DbProviderFactories.
                GetFactory(this.DatabaseProvider);

            // Create a new database connection.
            DbConnection connection = factory.CreateConnection();

            // Set the database connection's connection string.
            connection.ConnectionString = this.ConnectionString;

            // Create a new database command.
            DbCommand command = factory.CreateCommand();

            // Set the command's connection.
            command.Connection = connection;

            command.CommandText = query;

            for (int i = 0; i < parameters.Length; i++)
            {
                command.CommandText = command.CommandText.Replace("{" + i + "}", "@parameter" + i);

                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = "parameter" + i;
                parameter.Value = parameters[i];

                command.Parameters.Add(parameter);
            }

            connection.Open();

            DbDataReader reader = null;

            List<object[]> result = new List<object[]>();

            try
            {
                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    object[] value = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        value[i] = reader.GetValue(i);

                        if (value[i] == DBNull.Value)
                            value[i] = null;
                    }

                    result.Add(value);
                }
            }
            catch { }
            finally
            {
                connection.Close();

                command.Dispose();
                connection.Dispose();

                if (reader != null)
                    reader.Dispose();
            }

            return result;
        }

        #endregion
    }

    public enum UsageLogVariable
    {
        AccessedPage,
        Login,
        Studio,
        VariableUsed,
        VariableUsedSide,
        Browser,
        EmailId
    }
}
