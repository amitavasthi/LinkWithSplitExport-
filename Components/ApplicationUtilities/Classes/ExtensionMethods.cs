using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace System
{
    public static class CustomExtensionMethods
    {
        /// <summary>
        /// Converts any object into a byte array.
        /// </summary>
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts any object into a byte array.
        /// </summary>
        public static object FromByteArray(this Type type, byte[] bytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }

        /// <summary>
        /// Converts a XmlNodeList into an xml node array.
        /// </summary>
        public static XmlNode[] ToArray(this XmlNodeList xmlNodes)
        {
            XmlNode[] result = new XmlNode[xmlNodes.Count];

            for (int i = 0; i < xmlNodes.Count; i++)
            {
                result[i] = xmlNodes[i];
            }

            return result;
        }

        /// <summary>
        /// Creates a cloned list.
        /// </summary>
        public static List<T> Clone<T>(this List<T> list)
        {
            List<T> result = new List<T>();

            for (int i = 0; i < list.Count; i++)
            {
                result.Add(list[i]);
            }

            return result;
        }

        /// <summary>
        /// Removes the last comma from a string builder.
        /// </summary>
        public static void RemoveLastComma(this StringBuilder stringBuilder)
        {
            if (stringBuilder.Length == 0)
                return;

            if (stringBuilder[stringBuilder.Length - 1] == ',')
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }

        /// <summary>
        /// Add a xml attribute to a existing xml node.
        /// </summary>
        /// <param name="xmlNode">The xml node to add the xml attribute to.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public static void AddAttribute(this XmlNode xmlNode, string name, object value)
        {
            if (xmlNode == null)
                return;

            if (value == null)
                value = "";

            if (xmlNode.Attributes[name] != null)
            {
                xmlNode.Attributes[name].Value = value.ToString();

                return;
            }

            // Create a new xml attribute.
            XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute(name);

            // Set the xml attribute's value.
            xmlAttribute.Value = value.ToString();

            // Add the xml attribute to the xml node's xml attribute collection.
            xmlNode.Attributes.Append(xmlAttribute);
        }

        public static int IndexOf(this XmlNodeList items, XmlNode item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == item)
                    return i;
            }

            return -1;
        }

        public static int RoundUp(this double value)
        {
            double decimals = value - (int)value;

            if (decimals > 0.0)
                return (int)value + 1;
            else
                return (int)value;
        }

        /// <summary>
        /// Orders a xml node list by an attribute's value.
        /// </summary>
        /// <param name="xmlNode">The xml node list to order.</param>
        /// <param name="name">The name of the ordering attribute.</param>
        public static List<XmlNode> OrderByNumeric(this XmlNodeList xmlNodeList, string name)
        {
            List<XmlNode> result = new List<XmlNode>();

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                result.Add(xmlNode);
            }

            return result.OrderBy(x => x.Attributes[name] != null ? int.Parse(x.Attributes[name].Value) : int.MaxValue).ToList();
        }

        /// <summary>
        /// Orders a xml node list by an attribute's value.
        /// </summary>
        /// <param name="xmlNode">The xml node list to order.</param>
        /// <param name="name">The name of the ordering attribute.</param>
        public static List<XmlNode> OrderByDate(this XmlNodeList xmlNodeList, string name)
        {
            List<XmlNode> result = new List<XmlNode>();

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                result.Add(xmlNode);
            }

            return result.OrderByDescending(x => x.Attributes[name] != null ? DateTime.Parse(x.Attributes[name].Value) : new DateTime()).ToList();
        }

        /// <summary>
        /// Orders a xml node list by an attribute's value.
        /// </summary>
        /// <param name="xmlNode">The xml node list to order.</param>
        /// <param name="name">The name of the ordering attribute.</param>
        public static List<XmlNode> OrderByBool(this XmlNodeList xmlNodeList, string name)
        {
            List<XmlNode> result = new List<XmlNode>();

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                result.Add(xmlNode);
            }

            return result.OrderByDescending(x => x.Attributes[name] != null ? bool.Parse(x.Attributes[name].Value) : false).ToList();
        }

        /// <summary>
        /// Gets the xPath of a xml node.
        /// </summary>
        /// <param name="xmlNode">The xml node to get the xPath of.</param>
        /// <returns></returns>
        public static string GetXPath(this XmlNode xmlNode, bool forceAttribute = false, bool renderAttribute = true)
        {
            string result = xmlNode.Name;

            if (renderAttribute && xmlNode.Attributes != null)
            {
                if (xmlNode.Attributes["Id"] != null)
                {
                    result += string.Format(
                        "[@Id=\"{0}\"]",
                        xmlNode.Attributes["Id"].Value
                    );
                }
                else if (xmlNode.Attributes["Name"] != null)
                {
                    result += string.Format(
                        "[@Name=\"{0}\"]",
                        xmlNode.Attributes["Name"].Value
                    );
                }
                else if (xmlNode.Attributes.Count > 0 && forceAttribute)
                {
                    result += string.Format(
                        "[@{0}=\"{1}\"]",
                        xmlNode.Attributes[0].Name,
                        xmlNode.Attributes[0].Value
                    );
                }
            }

            if (xmlNode.ParentNode != null && xmlNode.ParentNode.Attributes != null)
            {
                result = xmlNode.ParentNode.GetXPath(forceAttribute, renderAttribute) + "/" + result;
            }

            return result;
        }

        /// <summary>
        /// Renders the xml node into json script.
        /// </summary>
        /// <param name="xmlNode">The xml node.</param>
        /// <returns></returns>
        public static string ToJson(this XmlNode xmlNode)
        {
            StringBuilder result = new StringBuilder();

            if (xmlNode.Attributes.Count == 0 && xmlNode.ChildNodes.Count == 0)
            {
                result.Append(string.Format(
                    " \"{0}\": ",
                    xmlNode.Name
                ));

                result.Append(" [ ] ");
            }
            else if (xmlNode.ChildNodes.Count < 2)
            {
                result.Append(" { ");

                // Run through all attributes of the xml node.
                foreach (XmlAttribute attribute in xmlNode.Attributes)
                {
                    result.Append(string.Format(
                        "\"{0}\": \"{1}\",",
                        attribute.Name,
                        attribute.Value.Replace("\"", "\\\"").Replace("\n", "").Replace("\r", "").Replace("\t", "")
                    ));
                }

                if (xmlNode.ChildNodes.Count > 0)
                {
                    if (xmlNode.Attributes.Count > 0 && xmlNode.ChildNodes[0].ChildNodes.Count == 1)
                    {
                        result.Append(string.Format(
                            " \"{0}\": [ ",
                            xmlNode.ChildNodes[0].Name
                        ));

                        result.Append(xmlNode.ChildNodes[0].ChildNodes[0].ToJson() + "");

                        if (xmlNode.Attributes.Count > 0 && xmlNode.ChildNodes[0].ChildNodes.Count == 1)
                            result.Append(" ] ");
                    }
                    else
                    {
                        // Run through all child nodes of the xml node.
                        foreach (XmlNode xmlNodeChild in xmlNode.ChildNodes)
                        {
                            // Render the child node as json and add it to the result string.
                            result.Append(xmlNodeChild.ToJson() + "");
                        }
                    }
                }

                if (xmlNode.Attributes.Count > 0 && xmlNode.ChildNodes.Count == 0)
                    result = result.Remove(result.Length - 1, 1);


                result.Append(" } ");
            }
            else
            {
                result.Append(" { ");

                result.Append(string.Format(
                    " \"{0}\": ",
                    xmlNode.Name
                ));

                result.Append(" [ ");

                // Run through all child nodes of the xml node.
                foreach (XmlNode xmlNodeChild in xmlNode.ChildNodes)
                {
                    // Render the child node as json and add it to the result string.
                    result.Append(xmlNodeChild.ToJson() + ",");
                }

                result = result.Remove(result.Length - 1, 1);

                result.Append(" ] ");

                result.Append(" } ");
            }

            // Return the contents of the result string builder.
            return result.ToString();
        }


        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            GregorianCalendar _gc = new GregorianCalendar();
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }
}