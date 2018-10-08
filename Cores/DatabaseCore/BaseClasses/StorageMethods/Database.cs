using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.BaseClasses.StorageMethods
{
    class Database<T> : StorageMethod<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the provider of the database.
        /// </summary>
        public string DatabaseProvider { get; set; }

        /// <summary>
        /// Gets or sets the connection string to the database.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the database table.
        /// </summary>
        public string TableName { get; set; }

        #endregion


        #region Constructor

        public Database(
            BaseCollection<T> collection,
            string tableName,
            string databaseProvider,
            string connectionString
        )
            : base(collection)
        {
            this.TableName = tableName;
            this.DatabaseProvider = databaseProvider;
            this.ConnectionString = connectionString;
        }

        #endregion


        #region Methods

        private DbCommand PrepareQuery(DbCommand query)
        {
            if (this.DatabaseProvider == "Npgsql")
            {
                string replacement = "";

                replacement = "\"";

                query.CommandText = query.CommandText.Replace("[", replacement);
                query.CommandText = query.CommandText.Replace("]", replacement);

                if (query.CommandText.StartsWith("SELECT TOP "))
                {
                    int limit = int.Parse(query.CommandText.Remove(0, 11).Split(' ')[0]);

                    query.CommandText = query.CommandText.Replace("SELECT TOP " + limit, "SELECT ");

                    query.CommandText += " LIMIT " + limit;
                }

                foreach (DbParameter parameter in query.Parameters)
                {
                    string value = "";

                    switch (parameter.Value.GetType().Name)
                    {
                        case "Guid":
                            value = "'{" + parameter.Value.ToString().ToUpper() + "}' ";
                            break;
                        case "DBNull":
                            value = "NULL ";
                            break;
                        case "Double":
                        case "Decimal":
                            value = "'" + parameter.Value.ToString().Replace(",", ".") + "' ";
                            break;
                        default:
                            value = "'" + parameter.Value.ToString().Replace("'", "''") + "' ";
                            break;
                    }

                    query.CommandText = query.CommandText.Replace("@" + parameter.ParameterName + " ", value);
                }

                query.Parameters.Clear();
            }

            return query;
        }


        private T CreateInstance(DbDataReader source)
        {
            // Create a new base item by the data source.
            T result = (T)Activator.CreateInstance(
                typeof(T),
                this.Collection,
                source
            );

            return result;
        }

        /// <summary>
        /// Inserts a base item into the database.
        /// </summary>
        /// <param name="baseItem">The item to add into the database.</param>
        public override void Insert(BaseItem<T> item)
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

            // Set the command's command text.
            command.CommandText = string.Format(
                "INSERT INTO {0} ({1}) VALUES ({2});",
                this.TableName,
                string.Join(",", item.Values.Keys.Select(x => "[" + x + "]")),
                string.Join(",", item.Values.Keys.Select(x => "@" + x + " "))
            );

            foreach (KeyValuePair<string, object> field in item.Values)
            {
                if (field.Value == null || field.Value == DBNull.Value)
                {
                    command.CommandText = command.CommandText.Replace("@" + field.Key, "NULL");
                    continue;
                }

                // Create a new database parameter.
                DbParameter parameter = command.CreateParameter();

                // Set the parameter's name.
                parameter.ParameterName = field.Key;

                // Set the parameter's value.
                parameter.Value = field.Value;

                // Add the parameter to the command's parameters.
                command.Parameters.Add(parameter);
            }

            DbTransaction transaction = null;

            // Open the database connection.
            connection.Open();

            command = PrepareQuery(command);

            try
            {
                // Begin a new database transaction.
                transaction = connection.BeginTransaction();

                // Set the command's transaction.
                command.Transaction = transaction;

                // Execute the command on the database.
                command.ExecuteNonQuery();

                SynchLog(command);

                transaction.Commit();

                connection.Close();
            }
            catch (Exception ex)
            {
                // Check if the transaction is set.
                if (transaction != null)
                {
                    // Rollback the transaction's changes.
                    transaction.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();

                connection.Dispose();
                command.Dispose();
                transaction.Dispose();
            }
        }

        public override void Insert(BaseItem<T>[] items)
        {
            foreach (BaseItem<T> item in items)
            {
                Insert(item);
            }
        }

        public override void Insert(BaseItem<T>[] items, string[] fields)
        {
            foreach (BaseItem<T> item in items)
            {
                Insert(item);
            }
        }

        public override void Update(BaseItem<T> item)
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

            // Set the command's command text.
            command.CommandText = string.Format(
                "UPDATE {0} SET ",
                this.TableName
            );

            // Run through all fields.
            foreach (KeyValuePair<string, object> value in item.Values)
            {
                if (value.Value == null)
                    continue;

                // Check if the field is the identity.
                if (value.Key == "Id")
                    continue;

                command.CommandText += string.Format(
                    "[{0}] = @{0} , ",
                    value.Key
                );

                // Create a new database parameter for the field.
                DbParameter parameter = command.CreateParameter();

                parameter.ParameterName = value.Key;
                parameter.Value = value.Value;

                command.Parameters.Add(parameter);
            }

            if (item.Values.Count > 0)
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - 2, 2);

            command.CommandText += string.Format(
                " WHERE [Id]='{0}'",
                item.Id
            );

            DbTransaction transaction = null;

            // Open the database connection.
            connection.Open();

            command = PrepareQuery(command);

            try
            {
                // Begin a new database transaction.
                transaction = connection.BeginTransaction();

                // Set the command's transaction.
                command.Transaction = transaction;

                // Execute the command on the database.
                command.ExecuteNonQuery();

                SynchLog(command);

                transaction.Commit();

                connection.Close();
            }
            catch (Exception ex)
            {
                // Check if the transaction is set.
                if (transaction != null)
                {
                    // Rollback the transaction's changes.
                    transaction.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();

                connection.Dispose();
                command.Dispose();
                transaction.Dispose();
            }
        }

        public override void Delete(BaseItem<T> item)
        {
            // Forward call.
            Delete(item.Id);
        }

        public override void Delete(Guid id)
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

            // Set the command's command text.
            command.CommandText = string.Format(
                "DELETE FROM {0} Where Id=@Id ",
                this.TableName
            );

            // Create a new database parameter for the id.
            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = "Id";
            parameter.Value = id;

            command.Parameters.Add(parameter);

            DbTransaction transaction = null;

            // Open the database connection.
            connection.Open();

            command = PrepareQuery(command);

            try
            {
                // Begin a new database transaction.
                transaction = connection.BeginTransaction();

                // Set the command's transaction.
                command.Transaction = transaction;

                // Execute the command on the database.
                command.ExecuteNonQuery();

                SynchLog(command);

                transaction.Commit();

                connection.Close();
            }
            catch (Exception ex)
            {
                // Check if the transaction is set.
                if (transaction != null)
                {
                    // Rollback the transaction's changes.
                    transaction.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();

                connection.Dispose();
                command.Dispose();
                transaction.Dispose();
            }
        }

        public override int GetCount(string[] names, object[] values)
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

            command.CommandText = string.Format(
                "SELECT Count(*) FROM {0} ",
                this.TableName
            );

            // Check if there are any fields to select.
            if (names.Length > 0)
            {
                command.CommandText += "WHERE ";

                // Run for the field name's length.
                for (int i = 0; i < names.Length; i++)
                {
                    // Add the field select to the command text.
                    command.CommandText += string.Format(
                        "[{0}] = @{0} AND ",
                        names[i]
                    );

                    // Create a new database parameter
                    // for the field select.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = names[i];

                    // Set the parameter's value.
                    parameter.Value = values[i];

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Remove the last " AND " from the command's command text.
                command.CommandText = command.CommandText.
                    Remove(command.CommandText.Length - 4, 4);
            }

            connection.Open();

            int result = 0;

            try
            {
                command = PrepareQuery(command);

                result = (int)command.ExecuteScalar();
            }
            finally
            {
                connection.Close();

                command.Dispose();
                connection.Dispose();
            }

            return result;
        }

        public override T[] Get(string[] names, object[] values)
        {
            List<T> result = new List<T>();

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

            command.CommandText = string.Format(
                "SELECT * FROM {0} ",
                this.TableName
            );

            // Check if there are any fields to select.
            if (names.Length > 0)
            {
                command.CommandText += "WHERE ";

                // Run for the field name's length.
                for (int i = 0; i < names.Length; i++)
                {
                    if (values[i] == null)
                    {
                        command.CommandText += string.Format(
                            "[{0}] IS NULL AND ",
                            names[i]
                        );

                        continue;
                    }

                    // Add the field select to the command text.
                    command.CommandText += string.Format(
                        "[{0}] = @{0} AND ",
                        names[i]
                    );

                    // Create a new database parameter
                    // for the field select.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = names[i];

                    // Set the parameter's value.
                    parameter.Value = values[i];

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Remove the last " AND " from the command's command text.
                command.CommandText = command.CommandText.
                    Remove(command.CommandText.Length - 4, 4);
            }

            connection.Open();

            DbDataReader reader = null;

            try
            {
                command = PrepareQuery(command);

                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    // Create a new instance of t by the database reader.
                    T item = CreateInstance(reader);

                    result.Add(item);
                }
            }
            finally
            {
                connection.Close();

                command.Dispose();
                connection.Dispose();

                if (reader != null)
                    reader.Dispose();
            }

            return result.ToArray();
        }

        public override object GetValue(string field, string[] names, object[] values)
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

            command.CommandText = string.Format(
                "SELECT [{0}] FROM {1} ",
                field,
                this.TableName
            );

            // Check if there are any fields to select.
            if (names.Length > 0)
            {
                command.CommandText += "WHERE ";

                // Run for the field name's length.
                for (int i = 0; i < names.Length; i++)
                {
                    if (values[i] == null)
                    {
                        // Add the field select to the command text.
                        command.CommandText += string.Format(
                            "[{0}] IS NULL AND ",
                            names[i]
                        );

                        continue;
                    }

                    // Add the field select to the command text.
                    command.CommandText += string.Format(
                        "[{0}] = @{0} AND ",
                        names[i]
                    );

                    // Create a new database parameter
                    // for the field select.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = names[i];

                    // Set the parameter's value.
                    parameter.Value = values[i];

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Remove the last " AND " from the command's command text.
                command.CommandText = command.CommandText.
                    Remove(command.CommandText.Length - 4, 4);
            }

            connection.Open();

            object result = null;
            DbDataReader reader = null;

            try
            {
                command = PrepareQuery(command);

                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    result = reader.GetValue(0);
                }

                if (result == DBNull.Value)
                    result = null;
            }
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

        public override List<object[]> GetValues(string[] fields, string[] names, object[] values, string orderField = null)
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

            command.CommandText = string.Format(
                "SELECT {0} FROM {1} ",
                string.Join(",", fields.Select(x => "[" + x + "]")),
                this.TableName
            );

            // Check if there are any fields to select.
            if (names.Length > 0)
            {
                command.CommandText += "WHERE ";

                // Run for the field name's length.
                for (int i = 0; i < names.Length; i++)
                {
                    if (values[i] == null)
                    {
                        // Add the field select to the command text.
                        command.CommandText += string.Format(
                            "[{0}] IS NULL AND ",
                            names[i]
                        );

                        continue;
                    }

                    // Add the field select to the command text.
                    command.CommandText += string.Format(
                        "[{0}] = @{0} AND ",
                        names[i]
                    );

                    // Create a new database parameter
                    // for the field select.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = names[i];

                    // Set the parameter's value.
                    parameter.Value = values[i];

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Remove the last " AND " from the command's command text.
                command.CommandText = command.CommandText.
                    Remove(command.CommandText.Length - 4, 4);
            }

            if (orderField != null)
            {
                command.CommandText += string.Format(
                    " ORDER BY [{0}]",
                    orderField
                );
            }

            connection.Open();


            DbDataReader reader = null;
            List<object[]> result = new List<object[]>();

            try
            {
                command = PrepareQuery(command);

                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    object[] item = new object[fields.Length];

                    for (int i = 0; i < fields.Length; i++)
                    {
                        item[i] = reader.GetValue(i);

                        if (item[i] == DBNull.Value)
                            item[i] = null;
                    }

                    result.Add(item);
                }
            }
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

        public override Dictionary<Guid, List<object[]>> GetValuesDict(string[] fields, string[] names, object[] values)
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

            command.CommandText = string.Format(
                "SELECT {0} FROM {1} ",
                string.Join(",", fields.Select(x => "[" + x + "]")),
                this.TableName
            );

            // Check if there are any fields to select.
            if (names.Length > 0)
            {
                command.CommandText += "WHERE ";

                // Run for the field name's length.
                for (int i = 0; i < names.Length; i++)
                {
                    if (values[i] == null)
                    {
                        // Add the field select to the command text.
                        command.CommandText += string.Format(
                            "[{0}] IS NULL AND ",
                            names[i]
                        );

                        continue;
                    }

                    // Add the field select to the command text.
                    command.CommandText += string.Format(
                        "[{0}] = @{0} AND ",
                        names[i]
                    );

                    // Create a new database parameter
                    // for the field select.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = names[i];

                    // Set the parameter's value.
                    parameter.Value = values[i];

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Remove the last " AND " from the command's command text.
                command.CommandText = command.CommandText.
                    Remove(command.CommandText.Length - 4, 4);
            }

            connection.Open();


            DbDataReader reader = null;
            Dictionary<Guid, List<object[]>> result = new Dictionary<Guid, List<object[]>>();

            try
            {
                command = PrepareQuery(command);

                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    object[] item = new object[fields.Length];

                    for (int i = 0; i < fields.Length; i++)
                    {
                        item[i] = reader.GetValue(i);

                        if (item[i] == DBNull.Value)
                            item[i] = null;
                    }

                    if (!result.ContainsKey((Guid)item[0]))
                        result.Add((Guid)item[0], new List<object[]>());

                    result[(Guid)item[0]].Add(item);
                }
            }
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

        public override List<object[]> GetValuesLike(string[] fields, string[] names, object[] values, string orderField = null)
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

            command.CommandText = string.Format(
                "SELECT {0} FROM {1} ",
                string.Join(",", fields.Select(x => "[" + x + "]")),
                this.TableName
            );

            // Check if there are any fields to select.
            if (names.Length > 0)
            {
                command.CommandText += "WHERE ";

                // Run for the field name's length.
                for (int i = 0; i < names.Length; i++)
                {
                    // For test only:
                    if (values[i].GetType() == typeof(string) && ((string)values[i]).Length >= 4000)
                    {
                        // Add the field select to the command text.
                        command.CommandText += string.Format(
                            "[{0}] like '%{1}%' AND ",
                            names[i],
                            System.Security.SecurityElement.Escape(values[i].ToString())
                        );

                        continue;
                    }

                    // Add the field select to the command text.
                    command.CommandText += string.Format(
                        "[{0}] like @{0} AND ",
                        names[i]
                    );

                    // Create a new database parameter
                    // for the field select.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = names[i];

                    // Set the parameter's value.
                    parameter.Value = "%" + values[i] + "%";

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Remove the last " AND " from the command's command text.
                command.CommandText = command.CommandText.
                    Remove(command.CommandText.Length - 4, 4);
            }

            if (orderField != null)
            {
                command.CommandText += string.Format(
                    " ORDER BY [{0}]",
                    orderField
                );
            }

            connection.Open();


            DbDataReader reader = null;
            List<object[]> result = new List<object[]>();

            try
            {
                command = PrepareQuery(command);

                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    object[] item = new object[fields.Length];

                    for (int i = 0; i < fields.Length; i++)
                    {
                        item[i] = reader.GetValue(i);

                        if (item[i] == DBNull.Value)
                            item[i] = null;
                    }

                    result.Add(item);
                }
            }
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

        public override List<object[]> GetValuesLikeWithoutSign(string[] fields, string[] names, object[] values, string orderField = null)
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

            command.CommandText = string.Format(
                "SELECT {0} FROM {1} ",
                string.Join(",", fields.Select(x => "[" + x + "]")),
                this.TableName
            );

            // Check if there are any fields to select.
            if (names.Length > 0)
            {
                command.CommandText += "WHERE ";

                // Run for the field name's length.
                for (int i = 0; i < names.Length; i++)
                {
                    // For test only:
                    if (values[i].GetType() == typeof(string) && ((string)values[i]).Length >= 4000)
                    {
                        // Add the field select to the command text.
                        command.CommandText += string.Format(
                            "[{0}] like '{1}' AND ",
                            names[i],
                            System.Security.SecurityElement.Escape(values[i].ToString())
                        );

                        continue;
                    }

                    // Add the field select to the command text.
                    command.CommandText += string.Format(
                        "[{0}] like @{0} AND ",
                        names[i]
                    );

                    // Create a new database parameter
                    // for the field select.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = names[i];

                    // Set the parameter's value.
                    parameter.Value = "" + values[i] + "";

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Remove the last " AND " from the command's command text.
                command.CommandText = command.CommandText.
                    Remove(command.CommandText.Length - 4, 4);
            }

            //japanese collate to get case sensitive match text
            command.CommandText += " COLLATE Japanese_CS_AS_KS_WS ";

            if (orderField != null)
            {
                command.CommandText += string.Format(
                    " ORDER BY [{0}]",
                    orderField
                );
            }

            connection.Open();


            DbDataReader reader = null;
            List<object[]> result = new List<object[]>();

            try
            {
                command = PrepareQuery(command);

                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    object[] item = new object[fields.Length];

                    for (int i = 0; i < fields.Length; i++)
                    {
                        item[i] = reader.GetValue(i);

                        if (item[i] == DBNull.Value)
                            item[i] = null;
                    }

                    result.Add(item);
                }
            }
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

        public override T[] Search(string searchField, string searchExpression, string[] names, object[] values, int maxResults = -1, string orderField = null)
        {
            List<T> result = new List<T>();

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

            command.CommandText = string.Format(
                "SELECT {1} FROM {0} WHERE ",
                this.TableName,
                maxResults == -1 ? "*" : "TOP " + maxResults + " *"
            );

            // Add the search expression to the command text.
            command.CommandText += string.Format(
                "{0} like @{0} ",
                searchField
            );

            // Create a new database parameter for the search field.
            DbParameter searchParameter = command.CreateParameter();
            searchParameter.ParameterName = searchField;
            searchParameter.Value = searchExpression;

            command.Parameters.Add(searchParameter);

            // Check if there are any fields to select.
            if (names.Length > 0)
            {
                command.CommandText += "AND ";

                // Run for the field name's length.
                for (int i = 0; i < names.Length; i++)
                {
                    // Add the field select to the command text.
                    command.CommandText += string.Format(
                        "[{0}] = @{0} AND ",
                        names[i]
                    );

                    // Create a new database parameter
                    // for the field select.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = names[i];

                    // Set the parameter's value.
                    parameter.Value = values[i];

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Remove the last " AND " from the command's command text.
                command.CommandText = command.CommandText.
                    Remove(command.CommandText.Length - 4, 4);
            }

            if (orderField != null)
            {
                command.CommandText += string.Format(
                    " ORDER BY {0}",
                    orderField
                );
            }

            connection.Open();

            DbDataReader reader = null;

            try
            {
                command = PrepareQuery(command);

                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    // Create a new instance of t by the database reader.
                    T item = CreateInstance(reader);

                    result.Add(item);
                }
            }
            finally
            {
                connection.Close();

                command.Dispose();
                connection.Dispose();

                if (reader != null)
                    reader.Dispose();
            }

            return result.ToArray();
        }

        public override List<object[]> SearchValues(string[] fields, string searchField, string searchExpression, string[] names, object[] values, int maxResults = -1, string orderField = null)
        {
            List<object[]> result = new List<object[]>();

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

            command.CommandText = string.Format(
                "SELECT {1} FROM {0} WHERE ",
                this.TableName,
                maxResults == -1 ? string.Join(", ", fields.Select(x => "[" + x + "]")) : "TOP " + maxResults + " " + string.Join(", ", fields.Select(x => "[" + x + "]"))
            );

            // Add the search expression to the command text.
            command.CommandText += string.Format(
                "[{0}] like @{0} ",
                searchField
            );

            // Create a new database parameter for the search field.
            DbParameter searchParameter = command.CreateParameter();
            searchParameter.ParameterName = searchField;
            searchParameter.Value = searchExpression;

            command.Parameters.Add(searchParameter);

            // Check if there are any fields to select.
            if (names.Length > 0)
            {
                command.CommandText += "AND ";

                // Run for the field name's length.
                for (int i = 0; i < names.Length; i++)
                {
                    // Add the field select to the command text.
                    command.CommandText += string.Format(
                        "[{0}] = @{0} AND ",
                        names[i]
                    );

                    // Create a new database parameter
                    // for the field select.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = names[i];

                    // Set the parameter's value.
                    parameter.Value = values[i];

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Remove the last " AND " from the command's command text.
                command.CommandText = command.CommandText.
                    Remove(command.CommandText.Length - 4, 4);
            }

            if (orderField != null)
            {
                command.CommandText += string.Format(
                    " ORDER BY [{0}]",
                    orderField
                );
            }

            connection.Open();
            DbDataReader reader = null;

            try
            {
                command = PrepareQuery(command);

                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    object[] item = new object[fields.Length];

                    for (int i = 0; i < fields.Length; i++)
                    {
                        item[i] = reader.GetValue(i);
                    }

                    result.Add(item);
                }
            }
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

        public override int Count(string[] names, object[] values)
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

            command.CommandText = string.Format(
                "SELECT Count(*) FROM {0} ",
                this.TableName
            );

            // Check if there are any fields to select.
            if (names.Length > 0)
            {
                command.CommandText += "WHERE ";

                // Run for the field name's length.
                for (int i = 0; i < names.Length; i++)
                {
                    // Add the field select to the command text.
                    command.CommandText += string.Format(
                        "[{0}] = @{0} AND ",
                        names[i]
                    );

                    // Create a new database parameter
                    // for the field select.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = names[i];

                    // Set the parameter's value.
                    parameter.Value = values[i];

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Remove the last " AND " from the command's command text.
                command.CommandText = command.CommandText.
                    Remove(command.CommandText.Length - 4, 4);
            }

            connection.Open();

            int result = 0;

            try
            {
                command = PrepareQuery(command);

                result = (int)command.ExecuteScalar();
            }
            finally
            {
                connection.Close();

                command.Dispose();
                connection.Dispose();
            }

            return result;
        }

        public override void SetValue(string field, object value, string[] names, object[] values)
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

            // Set the command's command text.
            command.CommandText = string.Format(
                "UPDATE {0} SET [{1}]=@{1}",
                this.TableName,
                field
            );

            // Create a new database parameter for the field.
            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = field;
            parameter.Value = value;

            command.Parameters.Add(parameter);

            command.CommandText += " WHERE ";

            for (int i = 0; i < names.Length; i++)
            {
                command.CommandText += string.Format(
                    "[{0}] = @{0} AND ",
                    names[i]
                );

                // Create a new database parameter for the field.
                parameter = command.CreateParameter();

                parameter.ParameterName = names[i];
                parameter.Value = values[i];

                command.Parameters.Add(parameter);
            }

            if (names.Length > 0)
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - 4, 4);

            DbTransaction transaction = null;

            // Open the database connection.
            connection.Open();

            command = PrepareQuery(command);

            try
            {
                // Begin a new database transaction.
                transaction = connection.BeginTransaction();

                // Set the command's transaction.
                command.Transaction = transaction;

                // Execute the command on the database.
                command.ExecuteNonQuery();

                transaction.Commit();

                connection.Close();
            }
            catch (Exception ex)
            {
                // Check if the transaction is set.
                if (transaction != null)
                {
                    // Rollback the transaction's changes.
                    transaction.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();

                connection.Dispose();
                command.Dispose();
                transaction.Dispose();
            }
        }


        public override void ExecuteQuery(string query)
        {
            if (query == null || query.Trim() == "")
                return;

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

            // Set the command's command text.
            command.CommandText = query;

            DbTransaction transaction = null;

            // Open the database connection.
            connection.Open();

            try
            {
                // Begin a new database transaction.
                transaction = connection.BeginTransaction();

                // Set the command's transaction.
                command.Transaction = transaction;

                command.CommandTimeout = 1200;

                // Execute the command on the database.
                command.ExecuteNonQuery();

                SynchLog(command);

                transaction.Commit();

                connection.Close();
            }
            catch (Exception ex)
            {
                try
                {
                    // Check if the transaction is set.
                    if (transaction != null)
                    {
                        // Rollback the transaction's changes.
                        transaction.Rollback();
                    }
                }
                catch { }

                throw ex;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();

                connection.Dispose();
                command.Dispose();
                transaction.Dispose();
            }
        }

        public override List<object[]> ExecuteReader(string query, params Type[] types)
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

            connection.Open();

            DbDataReader reader = null;

            List<object[]> result = new List<object[]>();

            try
            {
                command = PrepareQuery(command);

                // Execute a database reader on the command.
                reader = command.ExecuteReader();

                // Run for each row of the command's select result.
                while (reader.Read())
                {
                    object[] value = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (types.Length <= i)
                        {
                            value[i] = reader.GetValue(i);

                            if (value[i] == DBNull.Value)
                                value[i] = null;

                            continue;
                        }
                        object v = reader.GetValue(i);

                        if (v == DBNull.Value)
                        {
                            value[i] = null;
                        }
                        else
                        {
                            switch (types[i].Name)
                            {
                                case "Guid":
                                    value[i] = (Guid)v;
                                    break;
                                case "Double":
                                    value[i] = (double)v;
                                    break;
                                default:
                                    value[i] = v;
                                    break;
                            }
                        }
                    }

                    result.Add(value);
                }
            }
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

        public override List<object[]> ExecuteReader(string query, params object[] parameters)
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
                command = PrepareQuery(command);

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

        public override Dictionary<T2, List<object[]>> ExecuteReaderDict<T2>(string query, params object[] parameters)
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

            Dictionary<T2, List<object[]>> result = new Dictionary<T2, List<object[]>>();

            try
            {
                command = PrepareQuery(command);

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

                    if (!result.ContainsKey((T2)value[0]))
                        result.Add((T2)value[0], new List<object[]>());

                    result[(T2)value[0]].Add(value);
                }
            }
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
        public override Dictionary<T2, Dictionary<T3, List<object[]>>> ExecuteReaderDict<T2, T3>(string query, params object[] parameters)
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

            Dictionary<T2, Dictionary<T3, List<object[]>>> result =
                new Dictionary<T2, Dictionary<T3, List<object[]>>>();

            try
            {
                command = PrepareQuery(command);

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

                    if (!result.ContainsKey((T2)value[0]))
                        result.Add((T2)value[0], new Dictionary<T3, List<object[]>>());

                    if (!result[(T2)value[0]].ContainsKey((T3)value[1]))
                        result[(T2)value[0]].Add((T3)value[1], new List<object[]>());

                    result[(T2)value[0]][(T3)value[1]].Add(value);
                }
            }
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

        private void SynchLog(DbCommand command)
        {
            if (this.Collection.Owner.SynchServers.Length == 0)
                return;

            string[] names = new string[command.Parameters.Count];
            object[] values = new object[command.Parameters.Count];

            for (int i = 0; i < names.Length; i++)
            {
                names[i] = command.Parameters[i].ParameterName;
                values[i] = command.Parameters[i].Value;
            }

            SynchLog(
                command.CommandText,
                names,
                values
            );
        }

        private void SynchLog(string commandText, string[] names, object[] values)
        {
            if (this.Collection.Owner.SynchServers.Length == 0)
                return;

            Guid identity = Guid.NewGuid();

            StringBuilder result = new StringBuilder();

            result.Append("<DatabaseAction>");

            result.Append(string.Format(
                "<Identity>{0}></Identity>",
                identity
            ));

            result.Append(string.Format(
                "<Database>{0}</Database>",
                this.ConnectionString.Split(
                    new string[] { "Initial Catalog=" },
                    StringSplitOptions.None
                )[1].Split(';')[0]
            ));

            result.Append(string.Format(
                "<Query><![CDATA[{0}]]></Query>",
                commandText
            ));

            result.Append("<Parameters>");

            for (int i = 0; i < names.Length; i++)
            {
                result.Append(string.Format(
                    "<Parameter Name=\"{0}\" Type=\"{2}\"><![CDATA[{1}]]></Parameter>",
                    names[i],
                    values[i],
                    values[i].GetType().Name
                ));
            }

            result.Append("</Parameters>");

            result.Append("</DatabaseAction>");

            foreach (string synchServer in this.Collection.Owner.SynchServers)
            {
                string path = System.IO.Path.Combine(
                    System.Configuration.ConfigurationManager.AppSettings["SynchQueuePath"],
                    "DATABASE",
                    synchServer
                );

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                path = System.IO.Path.Combine(
                    path,
                    identity.ToString() + ".xml"
                );

                System.IO.File.WriteAllText(
                    path,
                    result.ToString()
                );

                /*System.IO.FileInfo fInfo = new System.IO.FileInfo(path);

                System.Security.AccessControl.FileSecurity fileSecurity = fInfo.GetAccessControl();
                fileSecurity.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule("everyone", System.Security.AccessControl.FileSystemRights.FullControl,
                                                     System.Security.AccessControl.InheritanceFlags.ObjectInherit | System.Security.AccessControl.InheritanceFlags.ContainerInherit,
                                                     System.Security.AccessControl.PropagationFlags.NoPropagateInherit, System.Security.AccessControl.AccessControlType.Allow));

                fInfo.SetAccessControl(fileSecurity);*/
            }

            result.Clear();
        }
    }
}
