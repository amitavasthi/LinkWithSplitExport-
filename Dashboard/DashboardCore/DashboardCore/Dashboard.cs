using ApplicationUtilities.Classes;
using DashboardCore.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace DashboardCore
{
    public class Dashboard
    {
        #region Properties

        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets the name of the dashboard.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the database core.
        /// </summary>
        public DatabaseCore.Core Core { get; set; }

        /// <summary>
        /// Gets or sets the full path to
        /// the dashboard definition file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that
        /// holds the dashboard definition.
        /// </summary>
        public XmlDocument Document { get; set; }

        /// <summary>
        /// Gets or sets the document element dashboard node.
        /// </summary>
        public DashboardNode DocumentElement { get; set; }

        public DashboardCache Cache { get; set; }

        public DashboardSettings Settings { get; set; }

        public Dictionary<string, DashboardNodeDimensionSelector> DimensionSelectors { get; set; }

        public int IdCounter { get; set; }

        public Dictionary<DashboardPropertyType, string> Properties { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the dashboard object.
        /// </summary>
        /// <param name="fileName">
        /// The full path to the dashboard definition file.
        /// </param>
        public Dashboard(string fileName, DatabaseCore.Core core)
        {
            this.FileName = fileName;
            this.Core = core;
            this.Document = new XmlDocument();
            this.Document.Load(fileName);

            this.Identity = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(fileName)).Name;

            this.Settings = new DashboardSettings(this);

            this.InitCache();
        }

        #endregion


        #region Methods

        private void InitCache()
        {
            this.Cache = new DashboardCache();

            this.Cache.Variables = this.Core.TaxonomyVariables.ExecuteReaderDict<string>(
                "SELECT Name, Id FROM TaxonomyVariables",
                new object[] { }
            );

            this.Cache.Categories = this.Core.TaxonomyCategories.ExecuteReaderDict<Guid, string>(
                "SELECT IdTaxonomyVariable, Name, Id FROM TaxonomyCategories ORDER BY [Order]",
                 new object[] { }
            );
            this.Cache.Categories2 = this.Core.TaxonomyCategories.ExecuteReaderDict<Guid>(
                "SELECT Id, Name FROM TaxonomyCategories ORDER BY [Order]",
                 new object[] { }
            );

            this.Cache.CategoryLabels = this.Core.TaxonomyCategoryLabels.ExecuteReaderDict<Guid>(
                "SELECT IdTaxonomyCategory, Label FROM TaxonomyCategoryLabels",
                new object[] { }
            );

            this.Cache.CategoryLabels2 = this.Core.TaxonomyCategoryLabels.ExecuteReaderDict<Guid, string>(
                "SELECT IdTaxonomyVariable, Label, IdTaxonomyCategory FROM TaxonomyCategoryLabels RIGHT JOIN TaxonomyCategories ON TaxonomyCategories.Id=TaxonomyCategoryLabels.IdTaxonomyCategory",
                new object[] { }
            );

            this.Cache.HasValue = new Dictionary<Guid, DashboardCore.DashboardCacheHasValue>();
        }

        public void Parse()
        {
            this.DimensionSelectors = new Dictionary<string, Classes.DashboardNodeDimensionSelector>();
            this.Name = this.Document.DocumentElement.Attributes["Name"].Value;

            // Create document element dashboard node.
            this.DocumentElement = ParseDashboardNode(
                this.Document.DocumentElement.SelectSingleNode("Definition")
            );

            this.InitProperties();

            // Parse all child dashboard nodes.
            this.DocumentElement.ParseChildren();

            // Execute the pre parsing methods.
            this.DocumentElement.PreParse();

            // Parse the document element dashboard node.
            this.DocumentElement.Parse();
        }

        private void InitProperties()
        {
            this.Properties = new Dictionary<Classes.DashboardPropertyType, string>();

            foreach (DashboardPropertyType property in Enum.GetValues(typeof(DashboardPropertyType)))
            {
                this.Properties.Add(property, "");
            }

            this.Properties[DashboardPropertyType.Title] = this.Name;
        }

        public string Render(bool bodyOnly)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Check if the dashboard has been parsed.
            if (this.DocumentElement == null)
                throw new Exception("Unable to render non-parsed dashboard.");

            // Create a new string builder that
            // holds the result html string.
            StringBuilder result = new StringBuilder();

            StringBuilder body = new StringBuilder();

            // Run through all child dashboard nodes.
            foreach (DashboardNode children in this.DocumentElement.Children)
            {
                children.Render(
                    body,
                    new DashboardRenderContext(),
                    null,
                    null
                );
            }

            if (!bodyOnly)
            {
                result.Append("<html><head>");

                if (File.Exists(Path.Combine(Path.GetDirectoryName(this.FileName), "favicon.png")))
                {
                    result.Append(string.Format(
                        "<link rel=\"icon\" type=\"image/png\" href=\"/Fileadmin/Dashboards/{0}/{1}/favicon.png\">",
                        this.Core.ClientName,
                        this.Identity
                    ));
                }
                else
                {
                    result.Append("<link rel=\"icon\" type=\"image/png\" href=\"favicon.png\">");
                }
                result.Append("<title>");
                result.Append(this.Properties[DashboardPropertyType.Title]);
                result.Append("</title>");

                foreach (string stylesheet in this.Settings.Includes)
                {
                    result.Append(stylesheet);
                }

                result.Append("</head><body>");
            }

            result.Append(body.ToString());

            // Render the document element dashboard node.
            //this.DocumentElement.Render(result, new DashboardRenderContext());

            if (!bodyOnly)
            {
                result.Append("</body>");
                result.Append("</html>");
            }

            if (this.Settings.DivTables)
            {
                result = result.Replace("<table", "<div Element=\"Table\"").Replace("</table>", "</div>");
                result = result.Replace("<tr", "<div Element=\"TableRow\"").Replace("</tr>", "</div>");
                result = result.Replace("<td", "<div Element=\"TableCell\"").Replace("</td>", "</div>");
            }

            stopwatch.Stop();

            this.Document.DocumentElement.AddAttribute(
                "ElapsedRendering",
                stopwatch.ElapsedMilliseconds
            );

            return result.ToString();
        }

        public string RenderDataUpdate()
        {
            // Create a new string builder that
            // holds the result JSON string.
            StringBuilder result = new StringBuilder();

            result.Append("[");

            // Run through all child dashboard nodes.
            foreach (DashboardNode children in this.DocumentElement.Children)
            {
                children.RenderDataUpdate(
                    result,
                    new DashboardRenderContext(),
                    null,
                    null,
                    ""
                );
            }

            result.RemoveLastComma();

            result.Append("]");

            return result.ToString();
        }

        internal DashboardNode ParseDashboardNode(XmlNode xmlNode, DashboardNode parent = null)
        {
            DashboardNode result;

            switch (xmlNode.Name)
            {
                case "Measure":
                    result = new DashboardNodeMeasure(this, xmlNode, parent);
                    break;
                case "MeasureLabel":
                    result = new DashboardNodeMeasureLabel(this, xmlNode, parent);
                    break;
                case "Filter":
                    result = new DashboardNodeFilter(this, xmlNode, parent);
                    break;
                case "Dimension":
                    result = new DashboardNodeDimension(this, xmlNode, parent);
                    break;
                case "DimensionLabel":
                    result = new DashboardNodeDimensionLabel(this, xmlNode, parent);
                    break;
                case "DimensionSelector":
                    result = new DashboardNodeDimensionSelector(this, xmlNode, parent);
                    break;
                case "Eval":
                    result = new DashboardNodeEval(this, xmlNode, parent);
                    break;
                case "DateTime":
                    result = new DashboardNodeDateTime(this, xmlNode, parent);
                    break;
                case "DashboardProperty":
                    result = new DashboardNodeProperty(this, xmlNode, parent);
                    break;
                default:
                    result = new DashboardNodeHtml(this, xmlNode, parent);
                    break;
            }

            return result;
        }

        #endregion
    }

    public class DashboardCache
    {
        public Dictionary<string, List<object[]>> Variables { get; set; }

        public Dictionary<Guid, Dictionary<string, List<object[]>>> Categories { get; set; }

        public Dictionary<Guid, List<object[]>> Categories2 { get; set; }

        public Dictionary<Guid, List<object[]>> CategoryLabels { get; set; }

        public Dictionary<Guid, Dictionary<string, List<object[]>>> CategoryLabels2 { get; set; }

        public Dictionary<Guid, DashboardCacheHasValue> HasValue { get; set; }
    }

    public class DashboardCacheHasValue
    {
        public bool HasValue { get; set; }

        public DataCore.Classes.Data Filter { get; set; }

        public Dictionary<Guid, DashboardCacheHasValue> Values { get; set; }

        public DashboardCacheHasValue()
        {
            this.Values = new Dictionary<Guid, DashboardCore.DashboardCacheHasValue>();
        }
    }
}
