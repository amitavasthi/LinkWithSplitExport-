using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace DatabaseCore.BaseClasses
{
    public abstract class BaseItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base item's owning collection.
        /// </summary>
        public BaseCollection Owner { get; set; }

        public abstract Guid Id { get; set; }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        public abstract void Insert();

        public abstract void Save();

        public abstract void SetValue(string name, object value);

        #endregion


        #region Event Handlers

        #endregion
    }

    public class BaseItem<T> : BaseItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets all values of the base item.
        /// </summary>
        public Dictionary<string, object> Values { get; set; }


        /// <summary>
        /// Gets or sets the id of the base item.
        /// </summary>
        public override Guid Id
        {
            get
            {
                return GetValue<Guid>("Id");
            }
            set
            {
                SetValue("Id", value);
            }
        }

        #endregion


        #region Constructor

        public BaseItem()
        {
            this.Values = new Dictionary<string, object>();
        }

        public BaseItem(BaseCollection<T> collection)
            : this()
        {
            this.Owner = collection;

            // Generate a new id for the base item.
            this.Id = Guid.NewGuid();
        }

        public BaseItem(BaseCollection<T> collection, DbDataReader reader = null)
            : this(collection)
        {
            // Check if a reader is set.
            if (reader != null)
            {
                // Read the values from the data reader.
                ReadValues(reader);
            }
        }

        public BaseItem(BaseCollection<T> collection, XmlNode xmlNode)
            : this(collection)
        {
            // Check if a reader is set.
            if (xmlNode != null)
            {
                // Read the values from the data reader.
                ReadValues(xmlNode);
            }
        }

        #endregion


        #region Methods

        private T Cast<T>(object value)
        {
            if (typeof(T).Name == "Guid" && value.GetType().Name == "String")
                return (T)((object)Guid.Parse(value.ToString()));

            return (T)value;
        }


        public void Dispose()
        {
            this.Owner = null;
            this.Values.Clear();
        }


        /// <summary>
        /// Reads values from a database reader into the base item's values.
        /// </summary>
        /// <param name="reader">The database reader to read the values from.</param>
        private void ReadValues(DbDataReader reader)
        {
            // Run for the reader's field count.
            for (int i = 0; i < reader.FieldCount; i++)
            {
                object value = reader.GetValue(i);

                if (value.GetType() == typeof(DBNull))
                    value = null;

                string name = reader.GetName(i);

                if (!this.Values.ContainsKey(name))
                    this.Values.Add(name, null);

                // Add the field with value to the
                // base item's values dictionary.
                this.Values[name] = value;
            }
        }

        /// <summary>
        /// Reads values from a xml node into the base item's values.
        /// </summary>
        /// <param name="xmlNode">The xml node to read the values from.</param>
        private void ReadValues(XmlNode xmlNode)
        {
            // Run through all xml attributes of the xml node.
            foreach (XmlAttribute attribute in xmlNode.Attributes)
            {
                Guid value;

                if (!this.Values.ContainsKey(attribute.Name))
                    this.Values.Add(attribute.Name, null);

                if (Guid.TryParse(attribute.Value, out value))
                {
                    this.Values[attribute.Name] = value;
                }
                else
                {
                    this.Values[attribute.Name] = attribute.Value;
                }
            }
        }


        /// <summary>
        /// Gets the value of an field.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="name">The name of the field.</param>
        public T GetValue<T>(string name)
        {
            // Check if the field exists.
            if (!this.Values.ContainsKey(name))
                return default(T);

            // Return the value of the field.
            return this.Cast<T>(this.Values[name]);
        }

        /// <summary>
        /// Gets the value of an field.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        public virtual object GetValue(string name)
        {
            // Check if the field exists.
            if (!this.Values.ContainsKey(name))
                return null;

            // Return the value of the field.
            return this.Values[name];
        }

        /// <summary>
        /// Sets a field's value of the base item.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        public override void SetValue(string name, object value)
        {
            if (value != null)
            {
                Type t = value.GetType();

                if (t.BaseType != null && t.BaseType.Name == "Enum")
                    value = (int)value;
            }

            // Check if the field exists.
            if (!this.Values.ContainsKey(name))
                this.Values.Add(name, null);

            // Set the field's value.
            this.Values[name] = value;
        }


        public override void Save()
        {
            // Forward call.
            this.Owner.Update(this);
        }

        public override void Insert()
        {
            this.Owner.Insert(this);
        }


        public XmlNode ToXml(XmlDocument document)
        {
            // Create a new xml node with the type of T as tag name.
            XmlNode result = document.CreateElement(typeof(T).Name);

            // Run through all fields of the item.
            foreach (KeyValuePair<string, object> field in this.Values)
            {
                // Create a new xml attribute for the field.
                XmlAttribute attribute = document.CreateAttribute(field.Key);

                // Set the attribute's value to the field's value.
                attribute.Value = field.Value.ToString();

                // Add the field's xml attribute to
                // the result xml node's attributes.
                result.Attributes.Append(attribute);
            }

            // Return the result xml node.
            return result;
        }

        public string ToXml()
        {
            // Create a new xml node with the type of T as tag name.
            StringBuilder result = new StringBuilder();

            result.Append(string.Format(
                "<{0}",
                typeof(T).Name
            ));

            // Create a new string builder to build the inner
            // xml which contains the field declarations.
            StringBuilder innerXml = new StringBuilder();

            // Run through all fields of the item.
            foreach (KeyValuePair<string, object> field in this.Values)
            {
                result.Append(string.Format(
                    " {0}=\"{1}\"",
                    field.Key,
                    field.Value
                ));
                /*
                innerXml.Append(string.Format(
                    "<Field Name=\"{0}\" Type=\"{1}\">{2}</Field>",
                    field.Key,
                    field.Value.GetType().Name,
                    field.Value
                ));
                */
            }

            result.Append(string.Format(
                ">{1}</{0}>",
                typeof(T).Name,
                innerXml.ToString()
            ));

            // Return the result xml node.
            return result.ToString();
        }

        public string ToXml(string[] fields)
        {
            // Create a new xml node with the type of T as tag name.
            StringBuilder result = new StringBuilder();

            result.Append(string.Format(
                "<{0}",
                typeof(T).Name
            ));

            // Create a new string builder to build the inner
            // xml which contains the field declarations.
            StringBuilder innerXml = new StringBuilder();

            // Run through all fields to insert.
            foreach (string field in fields)
            {
                result.Append(string.Format(
                    " {0}=\"{1}\"",
                    field,
                    this.Values[field]
                ));
            }

            result.Append(string.Format(
                ">{1}</{0}>",
                typeof(T).Name,
                innerXml.ToString()
            ));

            // Return the result xml node.
            return result.ToString();
        }

        public string ToJson()
        {
            StringBuilder jsonBuilder = new StringBuilder();

            jsonBuilder.Append("{");

            // Run through all properties of the element.
            foreach (KeyValuePair<string, object> property in this.Values)
            {
                jsonBuilder.Append(string.Format(
                    "\"{0}\": \"{1}\",",
                    property.Key,
                    property.Value
                ));
            }

            string result = jsonBuilder.ToString();

            if (result.Length > 0)
                result = result.Remove(result.Length - 1, 1);

            result += "}";

            return result;
        }

        public string ToJson(string[] names, object[] values)
        {
            StringBuilder jsonBuilder = new StringBuilder();

            jsonBuilder.Append("{");

            // Run through all properties of the element.
            foreach (KeyValuePair<string, object> property in this.Values)
            {
                jsonBuilder.Append(string.Format(
                    "\"{0}\": \"{1}\",",
                    property.Key,
                    HttpUtility.HtmlEncode(property.Value)
                ));
            }

            for (int i = 0; i < names.Length; i++)
            {
                jsonBuilder.Append(string.Format(
                    "\"{0}\": \"{1}\",",
                    names[i],
                    HttpUtility.HtmlEncode(values[i])
                ));
            }

            string result = jsonBuilder.ToString();

            if (result.Length > 0)
                result = result.Remove(result.Length - 1, 1);

            result += "}";

            return result;
        }

        public Type GetPropertyType(string name)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name == name)
                {
                    return property.PropertyType;
                }
            }

            return null;
        }

        public string RenderInsertQuery()
        {
            StringBuilder result = new StringBuilder();

            result.Append(string.Format(
                "INSERT INTO {0} ({1}) VALUES (",
                base.Owner.TableName,
                string.Join(",", this.Values.Keys.Select(x => "[" + x + "]"))
            ));

            foreach (object value in this.Values.Values)
            {
                //for japanese insert
                string TextAnswer = this.Values.FirstOrDefault(x => x.Value == value).Key;
                if (TextAnswer == "TextAnswer")
                {
                    result.Append("N'");
                    result.Append(System.Security.SecurityElement.Escape(value.ToString()));
                    result.Append("'");
                }
                else
                    switch (value.GetType().Name)
                    {
                        case "Int32":
                        case "Int64":
                            result.Append(value.ToString());
                            break;
                        case "Decimal":
                            result.Append(((decimal)value).ToString(new CultureInfo(2057)));
                            break;
                        case "Double":
                            result.Append(((double)value).ToString(new CultureInfo(2057)));
                            break;
                        case "DateTime":
                            result.Append("'");
                            result.Append(((DateTime)value).ToString("yyyy/MM/dd HH:mm"));
                            result.Append("'");
                            break;
                        default:
                            result.Append("'");
                            result.Append(System.Security.SecurityElement.Escape(value.ToString()));
                            result.Append("'");
                            break;
                    }

                result.Append(",");
            }

            if (this.Values.Values.Count > 0)
                result = result.Remove(result.Length - 1, 1);

            result.Append(");");

            return result.ToString();
        }

        public string RenderSaveQuery()
        {
            StringBuilder result = new StringBuilder();

            result.Append(string.Format(
                "UPDATE {0} SET ",
                base.Owner.TableName
            ));

            foreach (string key in this.Values.Keys)
            {
                result.Append(string.Format(
                    "[{0}]=",
                    key
                ));

                if (this.Values[key] == null)
                {
                    result.Append("NULL");
                }
                else
                {
                    switch (this.Values[key].GetType().Name)
                    {
                        case "Int32":
                        case "Int64":
                            result.Append(this.Values[key].ToString());
                            break;
                        case "Decimal":
                            result.Append(((decimal)this.Values[key]).ToString(new CultureInfo(2057)));
                            break;
                        case "Double":
                            result.Append(((double)this.Values[key]).ToString(new CultureInfo(2057)));
                            break;
                        case "DateTime":
                            result.Append("'");
                            result.Append(((DateTime)this.Values[key]).ToString("yyyy/MM/dd HH:mm"));
                            result.Append("'");
                            break;
                        default:
                            result.Append("'");
                            result.Append(System.Security.SecurityElement.Escape(this.Values[key].ToString()));
                            result.Append("'");
                            break;
                    }
                }

                result.Append(",");
            }

            result.RemoveLastComma();

            result.Append(string.Format(
                " WHERE Id='{0}';",
                this.Id
            ));

            return result.ToString();
        }

        #endregion
    }
}
