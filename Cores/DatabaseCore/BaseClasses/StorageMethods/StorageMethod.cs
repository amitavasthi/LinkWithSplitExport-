using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.BaseClasses.StorageMethods
{
    public abstract class StorageMethod<T>
    {
        #region Properties

        public BaseCollection<T> Collection { get; set; }

        #endregion


        #region Constructor

        public StorageMethod(BaseCollection<T> collection)
        {
            this.Collection = collection;
        }

        #endregion


        #region Abstract Method Declarations

        public abstract void Insert(BaseItem<T> item);

        public abstract void Insert(BaseItem<T>[] items);

        public abstract void Insert(BaseItem<T>[] items, string[] fields);

        public abstract void Update(BaseItem<T> item);

        public abstract void Delete(BaseItem<T> item);

        public abstract void Delete(Guid id);

        public abstract int GetCount(string[] names, object[] values);

        public abstract T[] Get(string[] names, object[] values);

        public abstract object GetValue(string field, string[] names, object[] values);

        public abstract List<object[]> GetValues(string[] fields, string[] names, object[] values, string orderField = null);
        public abstract Dictionary<Guid, List<object[]>> GetValuesDict(string[] fields, string[] names, object[] values);

        public abstract List<object[]> GetValuesLike(string[] fields, string[] names, object[] values, string orderField = null);

        public abstract List<object[]> GetValuesLikeWithoutSign(string[] fields, string[] names, object[] values, string orderField = null);

        public abstract T[] Search(string searchField, string searchExpression, string[] names, object[] values, int maxResults = -1, string orderField = null);

        public abstract List<object[]> SearchValues(string[] fields, string searchField, string searchExpression, string[] names, object[] values, int maxResults = -1, string orderField = null);

        public abstract int Count(string[] names, object[] values);

        public abstract void SetValue(string field, object value, string[] names, object[] values);


        public abstract void ExecuteQuery(string query);

        public abstract List<object[]> ExecuteReader(string query, params Type[] types);

        public abstract List<object[]> ExecuteReader(string query, params object[] parameters);

        public abstract Dictionary<T2, List<object[]>> ExecuteReaderDict<T2>(string query, params object[] parameters);
        public abstract Dictionary<T2, Dictionary<T3, List<object[]>>> ExecuteReaderDict<T2, T3>(string query, params object[] parameters);

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        #endregion
    }
}
