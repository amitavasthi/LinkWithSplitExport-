using DatabaseCore.BaseClasses.StorageMethods;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.BaseClasses
{
    public abstract class BaseCollection
    {

        /// <summary>
        /// Gets or sets the name of the database table.
        /// </summary>
        public string TableName { get; set; }

        public static Dictionary<string, Dictionary<string, object>> Cache { get; set; }

        /// <summary>
        /// Gets or sets the owning core of the collection.
        /// </summary>
        public BaseCore Owner { get; set; }

        public abstract object GetValue(string field, string[] names, object[] values);
        public abstract object GetValue(string field, string name, object value);
        public abstract object GetValue(string field, string path);
        public abstract List<object[]> GetValues(string[] fields, string[] names, object[] values, string orderField = null);

        public abstract void SetValue(string path, string field, object value);

        public abstract void Delete(string path);

        public abstract BaseItem[] GetItems(string[] names, object[] values, string orderField = null);

        public abstract void Update(BaseItem item);
        public abstract void Insert(BaseItem item);
    }

    public class BaseCollection<T> : BaseCollection
    {
        #region Properties

        /// <summary>
        /// Gets or sets a list of queue items for a bulk insert.
        /// </summary>
        private List<BaseItem<T>> Queue { get; set; }


        /// <summary>
        /// Gets or sets the provider of the database.
        /// </summary>
        public string DatabaseProvider { get; set; }

        /// <summary>
        /// Gets or sets the connection string to the database.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the storage method of the data.
        /// </summary>
        public StorageMethodType StorageMethod { get; set; }

        public bool DoCache { get; set; }

        #endregion


        #region Constructor

        public BaseCollection(BaseCore owner, string tableName, bool cache,
            StorageMethodType storageMethod = StorageMethodType.Database)
        {
            this.DoCache = cache;
            this.Owner = owner;
            this.TableName = PrepareTableName(tableName);
            this.Queue = new List<BaseItem<T>>();

            this.StorageMethod = storageMethod;
            this.DatabaseProvider = owner.DatabaseProvider;
            this.ConnectionString = owner.ConnectionString;

            if (!this.Owner.Tables.ContainsKey(tableName))
                this.Owner.Tables.Add(tableName, this);

            if (Cache == null)
                Cache = new Dictionary<string, Dictionary<string, object>>();
        }

        public BaseCollection(
            BaseCore owner,
            string tableName,
            string databaseProvider,
            string connectionString,
            bool cache,
            StorageMethodType storageMethod = StorageMethodType.Database
        )
            : this(owner, tableName, cache, storageMethod)
        {
            this.DatabaseProvider = databaseProvider;
            this.ConnectionString = connectionString;
        }

        #endregion


        #region Methods

        private StorageMethod<T> GetStorageMethod()
        {
            StorageMethod<T> result = null;

            switch (this.StorageMethod)
            {
                case StorageMethodType.Database:

                    result = new StorageMethods.Database<T>(
                        this,
                        this.TableName,
                        this.DatabaseProvider,
                        this.ConnectionString
                    );

                    break;
                case StorageMethodType.Xml:

                    result = new StorageMethods.Xml<T>(this, Path.Combine(
                        this.Owner.FileStorageRoot,
                        this.TableName + ".xml"
                    ));

                    break;
            }

            return result;
        }


        /// <summary>
        /// Prepares a table name for the use in database commands.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string PrepareTableName(string tableName)
        {
            string result = "";

            if (!tableName.Contains("."))
                tableName = "dbo." + tableName;

            string[] parts = tableName.Split('.');

            foreach (string part in parts)
            {
                result += "[" + part + "].";
            }

            if (result.Length > 0)
                result = result.Remove(result.Length - 1, 1);

            return result;
        }



        public T[] Get()
        {
            // Forward call.
            return Get(
                new string[0],
                new object[0]
            );
        }

        public override BaseItem[] GetItems(string[] names, object[] values, string orderField = null)
        {
            List<BaseItem> result = new List<BaseItem>();

            T[] items = Get(names, values);

            if (orderField != null)
            {
                items = items.OrderBy(x => ((BaseItem<T>)((object)x)).GetValue<int>("Order")).ToArray();
            }

            foreach (T item in items)
            {
                result.Add((BaseItem)((object)item));
            }

            return result.ToArray();
        }

        public T[] Get(string name, object value)
        {
            // Forward call.
            return Get(
                new string[] { name },
                new object[] { value }
            );
        }

        /// <summary>
        /// Gets base items.
        /// </summary>
        /// <param name="names">The select field names.</param>
        /// <param name="values">The select field values.</param>
        public T[] Get(string[] names, object[] values)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.Get(
                names,
                values
            );
        }

        public T[] Search(string searchField, string searchExpression, int maxResults = -1, string orderField = null)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.Search(
                searchField,
                searchExpression,
                new string[0],
                new object[0],
                maxResults,
                orderField
            );
        }

        public List<object[]> SearchValues(string field, string searchField, string searchExpression, int maxResults = -1)
        {
            // Forward call.
            return SearchValues(
                new string[] { field },
                searchField,
                searchExpression,
                maxResults
            );
        }

        public List<object[]> SearchValues(string[] fields, string searchField, string searchExpression, int maxResults = -1, string orderField = null)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.SearchValues(
                fields,
                searchField,
                searchExpression,
                new string[0],
                new object[0],
                maxResults,
                orderField
            );
        }

        public T[] Search(string searchField, string searchExpression, string[] names, object[] values)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.Search(
                searchField,
                searchExpression,
                names,
                values
            );
        }

        public int Count(string name, object value)
        {
            // Forward call.
            return Count(
                new string[] { name },
                new object[] { value }
            );
        }

        /// <summary>
        /// Gets base items.
        /// </summary>
        /// <param name="names">The select field names.</param>
        /// <param name="values">The select field values.</param>
        public int Count(string[] names, object[] values)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.Count(
                names,
                values
            );
        }

        public T GetSingle(Guid id)
        {
            T[] result = Get(
                new string[] { "Id" },
                new object[] { id }
            );

            if (result.Length == 0)
                return default(T);

            return result[0];
        }

        public T GetSingle(string name, object value)
        {
            T[] result = Get(
                new string[] { name },
                new object[] { value }
            );

            if (result.Length == 0)
                return default(T);

            return result[0];
        }

        public T GetSingle(string[] names, object[] values)
        {
            T[] result = Get(
                names,
                values
            );

            if (result.Length == 0)
                return default(T);

            return result[0];
        }

        public override object GetValue(string field, string name, object value)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.GetValue(
                field,
                new string[] { name },
                new object[] { value }
            );
        }

        public override object GetValue(string field, string path)
        {
            string[] attributes = path.Split('&');

            List<string> names = new List<string>();
            List<object> values = new List<object>();

            foreach (string attribute in attributes)
            {
                names.Add(attribute.Split('=')[0]);
                values.Add(attribute.Split('=')[1]);
            }

            // Forward call.
            return GetValue(
                field,
                names.ToArray(),
                values.ToArray()
            );
        }

        public override void Delete(string path)
        {
            string[] attributes = path.Split('&');

            List<string> names = new List<string>();
            List<object> values = new List<object>();

            foreach (string attribute in attributes)
            {
                names.Add(attribute.Split('=')[0]);
                values.Add(attribute.Split('=')[1]);
            }

            BaseItem<T> item = (BaseItem<T>)((object)GetSingle(
                names.ToArray(),
                values.ToArray()
            ));

            if (item != null)
                Delete(item);
        }

        public override object GetValue(string field, string[] names, object[] values)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.GetValue(
                field,
                names,
                values
            );
        }

        public List<object[]> GetValues(string[] fields)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.GetValues(
                fields,
                new string[0],
                new object[0],
                null
            );
        }

        public int GetCount()
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.GetCount(
                new string[0],
                new object[0]
            );
        }

        public int GetCount(string name, object value)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.GetCount(
                new string[] { name },
                new object[] { value }
            );
        }

        public int GetCount(string[] names, object[] values)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.GetCount(
                names,
                values
            );
        }

        public override List<object[]> GetValues(string[] fields, string[] names, object[] values, string orderField = null)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.GetValues(
                fields,
                names,
                values,
                orderField
            );
        }
        public Dictionary<Guid, List<object[]>> GetValuesDict(string[] fields, string[] names, object[] values)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.GetValuesDict(
                fields,
                names,
                values
            );
        }

        public List<object[]> GetValuesLike(string[] fields, string[] names, object[] values, string orderField = null)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.GetValuesLike(
                fields,
                names,
                values,
                orderField
            );
        }

        public List<object[]> GetValuesLikeWithoutSign(string[] fields, string[] names, object[] values, string orderField = null)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.GetValuesLikeWithoutSign(
                fields,
                names,
                values,
                orderField
            );
        }



        public override void SetValue(string path, string field, object value)
        {
            string[] attributes = path.Split('&');

            List<string> names = new List<string>();
            List<object> values = new List<object>();

            foreach (string attribute in attributes)
            {
                names.Add(attribute.Split('=')[0]);
                values.Add(attribute.Split('=')[1]);
            }

            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            storageMethod.SetValue(
                field,
                value,
                names.ToArray(),
                values.ToArray()
            );

            lock (Cache)
            {
                Cache.Clear();
            }
        }

        /// <summary>
        /// Updates the values of an item in the database.
        /// </summary>
        /// <param name="item">The item to update.</param>
        public override void Update(BaseItem item)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            storageMethod.Update(
                (BaseItem<T>)item
            );

            lock (Cache)
            {
                Cache.Clear();
            }
        }

        /// <summary>
        /// Inserts a base item into the database.
        /// </summary>
        /// <param name="baseItem">The item to add into the database.</param>
        public override void Insert(BaseItem item)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            storageMethod.Insert(
                (BaseItem<T>)item
            );

            lock (Cache)
            {
                Cache.Clear();
            }
            /*
            // Log the insert action of the element.
            Log<T>(
                item.Id,
                LogType.Insert
            );*/
        }

        /// <summary>
        /// Inserts base items into the database.
        /// </summary>
        /// <param name="baseItem">The items to add into the database.</param>
        public void Insert(BaseItem<T>[] items)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            storageMethod.Insert(
                items
            );

            lock (Cache)
            {
                Cache.Clear();
            }
            /*
            // Run through all elements.
            foreach (BaseItem<T> item in items)
            {
                // Log the insert action of the element.
                Log<T>(
                    item.Id,
                    LogType.Insert
                );
            }*/
        }

        /// <summary>
        /// Inserts base items into the database.
        /// </summary>
        /// <param name="baseItem">The items to add into the database.</param>
        /// <param name="fields">The fields to insert.</param>
        public void Insert(BaseItem<T>[] items, string[] fields)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            storageMethod.Insert(
                items,
                fields
            );

            lock (Cache)
            {
                Cache.Clear();
            }
        }

        /// <summary>
        /// Deletes a base item.
        /// </summary>
        /// <param name="item">The base item to delete.</param>
        public void Delete(BaseItem<T> item)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            storageMethod.Delete(
                item
            );

            // Log the delete action of the element.
            Log<T>(
                item.Id,
                LogType.Delete
            );

            lock (Cache)
            {
                Cache.Clear();
            }
        }

        /// <summary>
        /// Deletes a base item.
        /// </summary>
        /// <param name="item">The id of the base item to delete.</param>
        public void Delete(Guid id)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            storageMethod.Delete(
                id
            );

            // Log the delete action of the element.
            Log<T>(
                id,
                LogType.Delete
            );

            lock (Cache)
            {
                Cache.Clear();
            }
        }

        public void ExecuteQueue()
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

            DbTransaction transaction = null;

            // Run through all queue items.
            foreach (BaseItem<T> item in this.Queue)
            {
                foreach (KeyValuePair<string, object> field in item.Values)
                {
                    // Create a new database parameter.
                    DbParameter parameter = command.CreateParameter();

                    // Set the parameter's name.
                    parameter.ParameterName = field.Key + item.Id.
                        ToString().Replace("-", "");

                    // Set the parameter's value.
                    parameter.Value = field.Value;

                    // Add the parameter to the command's parameters.
                    command.Parameters.Add(parameter);
                }

                // Set the command's command text.
                command.CommandText += string.Format(
                    "INSERT INTO {0} ({1}) VALUES ({2});",
                    this.TableName,
                    string.Join(",", item.Values.Keys),
                    string.Join(",", item.Values.Keys.Select(
                        x => "@" + x + item.Id.ToString().Replace("-", ""))
                    )
                );

                if (command.Parameters.Count > 1700)
                {
                    connection.Open();

                    // Begin a new database transaction.
                    transaction = connection.BeginTransaction();

                    // Set the command's transaction.
                    command.Transaction = transaction;

                    // Execute the command on the database.
                    command.ExecuteNonQuery();

                    transaction.Commit();

                    connection.Close();

                    command = factory.CreateCommand();

                    command.Connection = connection;
                }
            }

            // Open the database connection.
            connection.Open();

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

                lock (Cache)
                {
                    Cache.Clear();
                }
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

        public void ExecuteQuery(string query)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            storageMethod.ExecuteQuery(
                query
            );

            lock (Cache)
            {
                Cache.Clear();
            }
        }

        public List<object[]> ExecuteReader(string query, params Type[] types)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.ExecuteReader(
                query,
                types
            );
        }

        public List<object[]> ExecuteReader(string query, object[] parameters)
        {
            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            return storageMethod.ExecuteReader(
                query,
                parameters
            );
        }

        public Dictionary<T2, List<object[]>> ExecuteReaderDict<T2>(string query, object[] parameters)
        {
            lock (Cache)
            {
                if(Cache.ContainsKey(this.Owner.ConnectionString) && 
                    Cache[this.Owner.ConnectionString].ContainsKey(query + string.Join("", parameters)))
                    return (Dictionary<T2, List<object[]>>)Cache[this.Owner.ConnectionString][query + string.Join("", parameters)];
            }

            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            Dictionary<T2, List<object[]>> result = storageMethod.ExecuteReaderDict<T2>(
                query,
                parameters
            );

            lock (Cache)
            {
                if (!Cache.ContainsKey(this.Owner.ConnectionString))
                    Cache.Add(this.Owner.ConnectionString, new Dictionary<string, object>());

                if (!Cache[this.Owner.ConnectionString].ContainsKey(query + string.Join("", parameters)))
                    Cache[this.Owner.ConnectionString].Add(query + string.Join("", parameters), result);
            }

            return (Dictionary<T2, List<object[]>>)Cache[this.Owner.ConnectionString][query + string.Join("", parameters)];
        }

        public Dictionary<T2, Dictionary<T3, List<object[]>>> ExecuteReaderDict<T2, T3>(string query, object[] parameters)
        {
            lock (Cache)
            {
                if (Cache.ContainsKey(this.Owner.ConnectionString) &&
                    Cache[this.Owner.ConnectionString].ContainsKey("_" + query + string.Join("", parameters)))
                    return (Dictionary<T2, Dictionary<T3, List<object[]>>>)Cache[this.Owner.ConnectionString]["_" + query + string.Join("", parameters)];
            }

            // Get the storage method.
            StorageMethod<T> storageMethod = GetStorageMethod();

            // Forward call.
            Dictionary<T2, Dictionary<T3, List<object[]>>> result = storageMethod.ExecuteReaderDict<T2, T3>(
                query,
                parameters
            );

            lock (Cache)
            {
                if (!Cache.ContainsKey(this.Owner.ConnectionString))
                    Cache.Add(this.Owner.ConnectionString, new Dictionary<string, object>());

                if (!Cache[this.Owner.ConnectionString].ContainsKey("_" + query + string.Join("", parameters)))
                    Cache[this.Owner.ConnectionString].Add("_" + query + string.Join("", parameters), result);
            }

            return (Dictionary<T2, Dictionary<T3, List<object[]>>>)Cache[this.Owner.ConnectionString]["_" + query + string.Join("", parameters)];
        }


        private void Log<T>(Guid idElement, string property, object oldValue, object newValue)
        {
            return;

            // Format the log change xml node with the values.
            string xml = string.Format(
                "<Change Timestamp=\"{0}\" Action=\"Change\" Property=\"{1}\" OldValue=\"{2}\" NewValue=\"{3}\" Id=\"{4}\" />",
                DateTime.Now.ToString(),
                property,
                oldValue,
                newValue,
                idElement
            );

            string fileName = Path.Combine(
                this.Owner.LogDirectory,
                DateTime.Now.ToString("yyyyMMdd")
            );

            // Check if the log directoy exists.
            if (!Directory.Exists(fileName))
                Directory.CreateDirectory(fileName);

            fileName = Path.Combine(
                fileName,
                typeof(T).Name + ".xml"
            );

            File.AppendAllText(
                fileName,
                xml
            );
        }

        /// <summary>
        /// Logs the action of an element.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="idElement">The id of the element.</param>
        private void Log<T>(Guid idElement, LogType type)
        {
            return;

            // Format the log change xml node with the values.
            string xml = string.Format(
                "<Change Timestamp=\"{0}\" Action=\"{2}\" Id=\"{1}\" />",
                DateTime.Now.ToString(),
                idElement,
                type
            );

            string fileName = Path.Combine(
                this.Owner.LogDirectory,
                DateTime.Now.ToString("yyyyMMdd")
            );

            // Check if the log directoy exists.
            if (!Directory.Exists(fileName))
                Directory.CreateDirectory(fileName);

            fileName = Path.Combine(
                fileName,
                typeof(T).Name + ".xml"
            );

            File.AppendAllText(
                fileName,
                xml
            );
        }

        #endregion
    }

    public enum LogType
    {
        Delete, Change, Insert
    }
}
