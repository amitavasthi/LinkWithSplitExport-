using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class ODataRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the report
        /// definition to render.
        /// </summary>
        public ReportDefinition Definition { get; set; }

        /// <summary>
        /// Gets or sets the query to process.
        /// </summary>
        public string Query { get; set; }

        public string RequestString { get; set; }

        #endregion


        #region Constructor

        public ODataRenderer(ReportDefinition definition, string query)
        {
            this.Definition = definition;
            this.Query = query;

            this.RequestString = HttpContext.Current.Request.Url.ToString().Replace(
                "Query=" + this.Query,
                ""
            );
        }

        #endregion


        #region Methods

        public string Render()
        {
            return System.IO.File.ReadAllText("C:\\Temp\\oData.xml").Replace(
                "https://blueoceanmi.sharepoint.com/_vti_bin/listdata.svc/",
                HttpContext.Current.Request.Url.ToString()
            );

            // Create a new string builder that
            // holds the result JSON string.
            StringBuilder result = new StringBuilder();

            // Open the root element.
            result.Append("{");

            // Write the current odata context
            // to the result JSON string.
            result.Append(string.Format(
                "\"@odata.context\": \"{0}\",",
                HttpContext.Current.Request.Url.ToString()
            ));

            // Open the values array.
            result.Append("\"value\": [");

            // Get all values for the requested query.
            List<Dictionary<string, object>> values = GetValues();

            // Run through all values for the requested query.
            foreach (Dictionary<string, object> value in values)
            {
                result.Append("{");

                // Run through all properties of the value.
                foreach (string name in value.Keys)
                {
                    result.Append(string.Format(
                        "\"{0}\": ",
                        name
                    ));

                    switch (value[name].GetType().Name)
                    {
                        case "Int16":
                        case "Int32":
                        case "Int64":
                        case "Double":
                        case "Float":

                            result.Append(value[name].ToString());

                            break;
                        default:

                            result.Append(string.Format(
                                "\"{0}\"",
                                value[name]
                            ));

                            break;
                    }

                    result.Append(",");
                }

                if (value.Count != 0)
                    result = result.Remove(result.Length - 1, 1);

                result.Append("},");
            }

            if (values.Count != 0)
                result = result.Remove(result.Length - 1, 1);

            // Close the values array.
            result.Append("]");

            // Close the root element.
            result.Append("}");

            return result.ToString();
        }

        private List<Dictionary<string, object>> GetValues()
        {

            // Write the request query for
            // the value to the JSON string.
            /*result.Append(string.Format(
                "\"@odata.id\": \"{0}\",",
                this.RequestString + "&Query="
            ));*/

            if (string.IsNullOrEmpty(this.Query))
            {
                return GetVariables();
            }
            else if (this.Query.StartsWith("Variable_"))
            {
                return GetCategories(this.Definition.ResolvePath<ReportDefinitionVariable>(this.Query.Split('_')[1]));
            }

            return new List<Dictionary<string, object>>();
        }

        private List<Dictionary<string, object>> GetVariables()
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            foreach (ReportDefinitionVariable variable in this.Definition.LeftVariables)
            {
                Dictionary<string, object> value = new Dictionary<string, object>();

                value.Add("name", variable.XmlNode.Attributes["Name"].Value);
                value.Add("kind", "EntitySet");
                value.Add("url", this.RequestString + "&Query=Variable_" + variable.XmlNode.GetXPath().Replace("\"", "'"));

                result.Add(value);
            }
            foreach (ReportDefinitionVariable variable in this.Definition.TopVariables)
            {
                Dictionary<string, object> value = new Dictionary<string, object>();

                value.Add("name", variable.XmlNode.Attributes["Name"].Value);
                value.Add("kind", "EntitySet");
                value.Add("url", this.RequestString + "&Query=Variable_" + variable.XmlNode.GetXPath());

                result.Add(value);
            }

            return result;
        }

        private List<Dictionary<string, object>> GetCategories(ReportDefinitionVariable variable)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            foreach (ReportDefinitionScore score in variable.Scores)
            {
                if (score.Hidden)
                    continue;

                Dictionary<string, object> value = new Dictionary<string, object>();

                value.Add("name", score.XmlNode.Attributes["Name"].Value);
                value.Add("kind", "EntitySet");
                value.Add("url", this.RequestString + "&Query=Category_" + score.Identity);

                result.Add(value);
            }

            return result;
        }

        #endregion
    }
}
