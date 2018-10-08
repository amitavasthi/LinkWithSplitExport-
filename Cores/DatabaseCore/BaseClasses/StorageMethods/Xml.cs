using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DatabaseCore.BaseClasses.StorageMethods
{
    public class Xml<T> : StorageMethod<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the file name of the xml file.
        /// </summary>
        public string FileName { get; set; }

        #endregion


        #region Constructor

        public Xml(BaseCollection<T> collection, string fileName)
            : base(collection)
        {
            this.FileName = fileName;

            InitFile();
        }

        #endregion


        #region Methods

        private string BuildPath(string[] names, object[] values)
        {
            // Create a new string builder to build the selection xPath.
            StringBuilder xPath = new StringBuilder();

            // Append the name of the xml node to the xPath's string builder.
            xPath.Append(typeof(T).Name);

            // Run for all selection fields.
            for (int i = 0; i < names.Length; i++)
            {
                // Add the xPath selection of the selection
                // to the xPath's string builder.
                xPath.Append(string.Format(
                    "[@{0}='{1}']",
                    names[i],
                    values[i].ToString()
                ));
            }

            return xPath.ToString();
        }


        private T CreateInstance(XmlNode source)
        {
            // Create a new base item by the data source.
            T result = (T)Activator.CreateInstance(
                typeof(T),
                this.Collection,
                source
            );

            return result;
        }


        public override void Insert(BaseItem<T> item)
        {
            /*
            // Create a new xml document.
            XmlDocument document = new XmlDocument();

            // Load the content of the xml file into the xml document.
            document.Load(this.FileName);

            // Render the item to a xml node.
            XmlNode xmlNode = item.ToXml(document);

            // Add the item's xml node to the document.
            document.DocumentElement.AppendChild(xmlNode);

            // Save the document.
            document.Save(this.FileName);
            */

            File.AppendAllText(
              this.FileName,
              item.ToXml()
            );
        }

        public override void Insert(BaseItem<T>[] items)
        {
            // Create a new string builder containing the xml string.
            StringBuilder xmlBuilder = new StringBuilder();

            // Run through all items.
            foreach (BaseItem<T> item in items)
            {
                // Add the item as xml to the string builder.
                xmlBuilder.Append(item.ToXml());
            }

            File.AppendAllText(
              this.FileName,
              xmlBuilder.ToString()
            );
        }

        public override void Insert(BaseItem<T>[] items, string[] fields)
        {
            // Create a new string builder containing the xml string.
            StringBuilder xmlBuilder = new StringBuilder();

            // Run through all items.
            foreach (BaseItem<T> item in items)
            {
                // Add the item as xml to the string builder.
                xmlBuilder.Append(item.ToXml(fields));
            }

            File.AppendAllText(
              this.FileName,
              xmlBuilder.ToString()
            );
        }

        public override void Update(BaseItem<T> item)
        {
            throw new NotImplementedException();
        }

        public override void Delete(BaseItem<T> item)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public override int GetCount(string[] names, object[] values)
        {
            throw new NotImplementedException();
        }

        public override T[] Get(string[] names, object[] values)
        {
            // Create a new list of items containing the result.
            List<T> result = new List<T>();

            string xPath = BuildPath(names, values);

            // Create a new xml document.
            XmlDocument document = new XmlDocument();

            StringBuilder xmlString = new StringBuilder();
            xmlString.Append("<Items>");
            xmlString.Append(File.ReadAllText(this.FileName));
            xmlString.Append("</Items>");

            // Load the content of the xml
            // file into the xml document.
            //document.Load(this.FileName);
            document.LoadXml(xmlString.ToString());

            // Select all xml nodes with the builded xPath.
            XmlNodeList xmlNodes = document.DocumentElement.
                SelectNodes(xPath.ToString());

            // Run through all selected xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                // Create a new instance of T using
                // the xml node as source.
                T item = CreateInstance(xmlNode);

                // Add the item to the result collection.
                result.Add(item);
            }

            // Return the result list as an array.
            return result.ToArray();
        }

        public override object GetValue(string field, string[] names, object[] values)
        {
            string xPath = BuildPath(names, values);

            // Create a new xml document.
            XmlDocument document = new XmlDocument();

            StringBuilder xmlString = new StringBuilder();
            xmlString.Append("<Items>");
            xmlString.Append(File.ReadAllText(this.FileName));
            xmlString.Append("</Items>");

            // Load the content of the xml
            // file into the xml document.
            //document.Load(this.FileName);
            document.LoadXml(xmlString.ToString());

            // Select all xml nodes with the builded xPath.
            XmlNode xmlNode = document.DocumentElement.
                SelectSingleNode(xPath.ToString());

            if (xmlNode == null)
                return null;

            if (xmlNode.Attributes[field] == null)
                return null;

            return xmlNode.Attributes[field].Value;
        }

        public override List<object[]> GetValues(string[] fields, string[] names, object[] values, string orderField = null)
        {
            string xPath = BuildPath(names, values);

            // Create a new xml document.
            XmlDocument document = new XmlDocument();

            StringBuilder xmlString = new StringBuilder();
            xmlString.Append("<Items>");
            xmlString.Append(File.ReadAllText(this.FileName));
            xmlString.Append("</Items>");

            // Load the content of the xml
            // file into the xml document.
            //document.Load(this.FileName);
            document.LoadXml(xmlString.ToString());

            // Select all xml nodes with the builded xPath.
            XmlNodeList xmlNodes = document.DocumentElement.
                SelectNodes(xPath.ToString());

            List<object[]> result = new List<object[]>();

            foreach (XmlNode xmlNode in xmlNodes)
            {
                object[] item = new object[fields.Length];

                for (int i = 0; i < fields.Length; i++)
                {
                    if (xmlNode.Attributes[fields[i]] == null)
                        item[i] = null;
                    else
                        item[i] = xmlNode.Attributes[fields[i]].Value;
                }

                result.Add(item);
            }

            return result;
        }

        public override Dictionary<Guid, List<object[]>> GetValuesDict(string[] fields, string[] names, object[] values)
        {
            throw new NotImplementedException();
        }

        public override List<object[]> GetValuesLike(string[] fields, string[] names, object[] values, string orderField = null)
        {
            throw new NotImplementedException();
        }

        public override List<object[]> GetValuesLikeWithoutSign(string[] fields, string[] names, object[] values, string orderField = null)
        {
            throw new NotImplementedException();
        }

        public override T[] Search(string searchField, string searchExpression, string[] names, object[] values, int maxResults = -1, string orderField = null)
        {
            throw new NotImplementedException();
        }

        public override List<object[]> SearchValues(string[] fields, string searchField, string searchExpression, string[] names, object[] values, int maxResults = -1, string orderField = null)
        {
            throw new NotImplementedException();
        }

        public override int Count(string[] names, object[] values)
        {
            // Create a new list of items containing the result.
            List<T> result = new List<T>();

            // Create a new string builder to build the selection xPath.
            StringBuilder xPath = new StringBuilder();

            // Append the name of the xml node to the xPath's string builder.
            xPath.Append(typeof(T).Name);

            // Run for all selection fields.
            for (int i = 0; i < names.Length; i++)
            {
                // Add the xPath selection of the selection
                // to the xPath's string builder.
                xPath.Append(string.Format(
                    "[@{0}='{1}']",
                    names[i],
                    values[i].ToString()
                ));
            }

            // Create a new xml document.
            XmlDocument document = new XmlDocument();

            StringBuilder xmlString = new StringBuilder();
            xmlString.Append("<Items>");
            xmlString.Append(File.ReadAllText(this.FileName));
            xmlString.Append("</Items>");

            // Load the content of the xml
            // file into the xml document.
            //document.Load(this.FileName);
            document.LoadXml(xmlString.ToString());

            // Select all xml nodes with the builded xPath.
            XmlNodeList xmlNodes = document.DocumentElement.
                SelectNodes(xPath.ToString());

            return xmlNodes.Count;
        }

        public override void SetValue(string field, object value, string[] names, object[] values)
        {
            throw new NotImplementedException();
        }

        private void InitFile()
        {
            // Check if the xml file exists.
            if (File.Exists(this.FileName))
                return;

            // Create a new string builder to build
            // the base structure of the xml file.
            StringBuilder xmlBuilder = new StringBuilder();

            // Render the document element xml node.
            xmlBuilder.Append(string.Format(
                ""
            ));

            // Write the content's of the
            // string builder to the file.
            File.WriteAllText(
                this.FileName,
                xmlBuilder.ToString()
            );
        }


        public override void ExecuteQuery(string query)
        {
            throw new NotImplementedException();
        }

        public override List<object[]> ExecuteReader(string query, params Type[] types)
        {
            throw new NotImplementedException();
        }

        public override List<object[]> ExecuteReader(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<T2, List<object[]>> ExecuteReaderDict<T2>(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<T2, Dictionary<T3, List<object[]>>> ExecuteReaderDict<T2, T3>(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
