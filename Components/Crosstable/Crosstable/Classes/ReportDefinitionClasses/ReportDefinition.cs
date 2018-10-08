using Crosstables.Classes.HierarchyClasses;
using Crosstables.Classes.ReportDefinitionClasses.Collections;
using Crosstables.Classes.WorkflowClasses;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class ReportDefinition : BaseReportDefinition
    {
        #region Properties

        /// <summary>
        /// Gets or sets a collection of the
        /// left variables of the report.
        /// </summary>
        public List<ReportDefinitionVariable> LeftVariables { get; set; }

        /// <summary>
        /// Gets or sets a collection of the
        /// top variables of the report.
        /// </summary>
        public List<ReportDefinitionVariable> TopVariables { get; set; }

        /// <summary>
        /// Gets or sets the data request date of the report.
        /// </summary>
        public DateTime DataRequestDate
        {
            get
            {
                return DateTime.Parse(
                    this.XmlDocument.DocumentElement.Attributes["DataRequest"].Value
                );
            }
            set
            {
                if (this.XmlDocument.DocumentElement.Attributes["DataRequest"] == null)
                    this.XmlDocument.DocumentElement.AddAttribute("DataRequest", value.ToString());
                else
                    this.XmlDocument.DocumentElement.Attributes["DataRequest"].Value = value.ToString();
            }
        }

        public TimeSpan DataCalculationTime
        {
            get
            {
                return TimeSpan.Parse(
                    this.XmlDocument.DocumentElement.Attributes["DataCalculationTime"].Value
                );
            }
            set
            {
                if (this.XmlDocument.DocumentElement.Attributes["DataCalculationTime"] == null)
                    this.XmlDocument.DocumentElement.AddAttribute("DataCalculationTime", value.ToString());
                else
                    this.XmlDocument.DocumentElement.Attributes["DataCalculationTime"].Value = value.ToString();
            }
        }
        public TimeSpan DataPreperationTime
        {
            get
            {
                return TimeSpan.Parse(
                    this.XmlDocument.DocumentElement.Attributes["DataPreperationTime"].Value
                );
            }
            set
            {
                if (this.XmlDocument.DocumentElement.Attributes["DataPreperationTime"] == null)
                    this.XmlDocument.DocumentElement.AddAttribute("DataPreperationTime", value.ToString());
                else
                    this.XmlDocument.DocumentElement.Attributes["DataPreperationTime"].Value = value.ToString();
            }
        }
        public TimeSpan DataRenderTime
        {
            get
            {
                return TimeSpan.Parse(
                    this.XmlDocument.DocumentElement.Attributes["DataRenderTime"].Value
                );
            }
            set
            {
                if (this.XmlDocument.DocumentElement.Attributes["DataRenderTime"] == null)
                    this.XmlDocument.DocumentElement.AddAttribute("DataRenderTime", value.ToString());
                else
                    this.XmlDocument.DocumentElement.Attributes["DataRenderTime"].Value = value.ToString();
            }
        }
        public int CaseDataVersion
        {
            get
            {
                if (this.XmlDocument.DocumentElement.Attributes["CaseDataVersion"] == null)
                    this.CaseDataVersion = 0;

                return int.Parse(
                    this.XmlDocument.DocumentElement.Attributes["CaseDataVersion"].Value
                );
            }
            set
            {
                if (this.XmlDocument.DocumentElement.Attributes["CaseDataVersion"] == null)
                    this.XmlDocument.DocumentElement.AddAttribute("CaseDataVersion", value.ToString());
                else
                    this.XmlDocument.DocumentElement.Attributes["CaseDataVersion"].Value = value.ToString();
            }
        }

        public DataCheck DataCheck
        {
            get
            {
                return new DataCheck(this.FileName);
            }
        }


        public bool SignificanceTest
        {
            get
            {
                if (this.TopVariables.Count > 0 && this.TopVariables[0].IsFake)
                    return false;              
                return this.Settings.SignificanceTest;
            }
        }


        public bool HasData
        {
            get
            {
                if (this.LeftVariables.Count == 0 || this.TopVariables.Count == 0)
                    return true;

                XmlNode xmlNodeResults = this.XmlDocument.DocumentElement.SelectSingleNode("Results");

                if (xmlNodeResults == null)
                    return false;

                return true;
            }
        }

        public CrosstableSettings Settings
        {
            get
            {
                return (CrosstableSettings)base.Settings;
            }
            set
            {
                base.Settings = value;
            }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of a report definition.
        /// </summary>
        /// <param name="core">The used database core.</param>
        private ReportDefinition(DatabaseCore.Core core, HierarchyFilter hierarchyFilter)
            : base(core, hierarchyFilter)
        { }

        /// <summary>
        /// Creates a new instance of a report definition.
        /// </summary>
        /// <param name="core">The used database core.</param>
        /// <param name="fileName">The full path to the report definition xml file.</param>
        public ReportDefinition(DatabaseCore.Core core, string fileName, HierarchyFilter hierarchyFilter)
            : base(core, fileName, hierarchyFilter)
        { }

        /// <summary>
        /// Creates a new instance of a report definition.
        /// </summary>
        /// <param name="core">The used database core.</param>
        public ReportDefinition(DatabaseCore.Core core, XmlDocument document, HierarchyFilter hierarchyFilter)
            : base(core, document, hierarchyFilter)
        { }

        public ReportDefinition(
            DatabaseCore.Core core, 
            string fileName, 
            string fileNameWorkflow, 
            string fileNameWeighting, 
            HierarchyFilter hierarchyFilter, 
            string userDefaultSettings = null
        )
            : this(core, hierarchyFilter)
        {
            this.FileName = fileName;

            XmlNode xmlNodeWeighting = null;

            if (fileNameWeighting != null)
            {
                XmlDocument xmlDocumentWeighting = new XmlDocument();
                xmlDocumentWeighting.Load(fileNameWeighting);

                xmlNodeWeighting = xmlDocumentWeighting.DocumentElement;
            }

            // Create a new report definition xml file.
            Create(fileNameWorkflow, xmlNodeWeighting, userDefaultSettings);

            //this.Workflow.SelectAll();

            // Save the report definition.
            Save();
        }

        #endregion


        #region Methods

        public Crosstables.Classes.ReportDefinitionClasses.ReportDefinitionVariable ResolvePath<ReportDefinitionVariable>(string path)
        {
            Crosstables.Classes.ReportDefinitionClasses.ReportDefinitionVariable result = null;

            result = ResolvePath(this.LeftVariables, path);

            if (result == null)
                result = ResolvePath(this.TopVariables, path);

            return result;
        }

        private ReportDefinitionVariable ResolvePath(List<ReportDefinitionVariable> collection, string path)
        {
            foreach (ReportDefinitionVariable variable in collection)
            {
                if (variable.XmlNode.GetXPath() == path)
                    return variable;

                ReportDefinitionVariable nested = ResolvePath(variable.NestedVariables, path);

                if (nested != null)
                    return nested;
            }

            return null;
        }

        public override void Parse()
        {
            this.LeftVariables = new List<ReportDefinitionVariable>();
            this.TopVariables = new List<ReportDefinitionVariable>();

            // Select all left variable xml nodes.
            XmlNodeList xmlNodesLeftVariables = this.XmlDocument.DocumentElement.
                SelectNodes("Variables[@Position=\"Left\"]/Variable");

            // Select all top variable xml nodes.
            XmlNodeList xmlNodesTopVariables = this.XmlDocument.DocumentElement.
                SelectNodes("Variables[@Position=\"Top\"]/Variable");

            // Run through all left variable xml nodes.
            foreach (XmlNode xmlNodeLeftVariable in xmlNodesLeftVariables)
            {
                // Create a new report definition variable by the left variable xml node.
                ReportDefinitionVariable variable = new ReportDefinitionVariable(this, xmlNodeLeftVariable);

                // Check if the variable still exists.
                if (variable.IsFake == false && variable.IsTaxonomy == false && this.Core.Variables.GetSingle(variable.IdVariable) == null)
                {
                    // Remove the variable's xml node from the report definition.
                    xmlNodeLeftVariable.ParentNode.RemoveChild(xmlNodeLeftVariable);

                    // Continue with the next item.
                    continue;
                }

                // Add the report definition variable to the left variables collection.
                this.LeftVariables.Add(variable);

                string position = variable.Position;
            }

            // Run through all top variable xml nodes.
            foreach (XmlNode xmlNodeTopVariable in xmlNodesTopVariables)
            {
                // Create a new report definition variable by the top variable xml node.
                ReportDefinitionVariable variable = new ReportDefinitionVariable(this, xmlNodeTopVariable);

                // Check if the variable still exists.
                if (variable.IsFake == false && variable.IsTaxonomy == false && this.Core.Variables.GetSingle(variable.IdVariable) == null)
                {
                    // Remove the variable's xml node from the report definition.
                    xmlNodeTopVariable.ParentNode.RemoveChild(xmlNodeTopVariable);

                    // Continue with the next item.
                    continue;
                }

                // Add the report definition variable to the top variables collection.
                this.TopVariables.Add(variable);

                string position = variable.Position;
            }

            // Select the settings xml node.
            XmlNode xmlNodeSettings = this.XmlDocument.DocumentElement.SelectSingleNode("Settings");

            if (xmlNodeSettings != null)
                this.Settings = new CrosstableSettings(this, xmlNodeSettings);
        }

        private void Create(string fileNameWorkflowDefinition, XmlNode xmlNodeWeighting = null, string userDefaultSettings = null)
        {
            // Build the path to the report definition template xml file.
            string fileNameTemplate = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "ReportDefinition.xml"
            );

            // Copy the template file to the report definition's file path.
            File.Copy(
                fileNameTemplate,
                this.FileName
            );

            // Create a new xml document.
            this.XmlDocument = new System.Xml.XmlDocument();

            // Load the contents of the report definition's
            // xml file into the xml document.
            this.XmlDocument.Load(this.FileName);

            if(userDefaultSettings != null)
            {
                this.XmlDocument.SelectSingleNode("Report/Settings").InnerXml = userDefaultSettings;
            }

            // Set the report definition's workflow structure
            // to the used workflow structure.
            //xmlNodeWorkflow.InnerXml = workflowDefinition.InnerXml;
            base.CheckWorkflow(fileNameWorkflowDefinition);

            if (xmlNodeWeighting != null)
            {
                XmlNode xmlNodeW = this.XmlDocument.DocumentElement.SelectSingleNode("WeightingVariables");

                xmlNodeW.InnerXml = xmlNodeWeighting.InnerXml;

                foreach (XmlAttribute attribute in xmlNodeWeighting.Attributes)
                {
                    if (xmlNodeW[attribute.Name] == null)
                        xmlNodeW.AddAttribute(attribute.Name, attribute.Value);
                    else
                        xmlNodeW[attribute.Name].Value = attribute.Value;
                }
            }

            this.XmlDocument.Save(this.FileName);

            // Parse the report definition xml file.
            //Parse();
            base.ParseBase();

            this.Settings.AutoUpdate = false;

            TaxonomyVariable[] weightingVariables = this.Core.TaxonomyVariables.Get("Weight", true).OrderBy(x => x.Order).ToArray();

            if (weightingVariables.Length > 0)
            {
                this.WeightingFilters.DefaultWeighting = weightingVariables[0].Id;
            }

            this.Save();
        }

        /// <summary>
        /// Clears the whole report definition.
        /// </summary>
        public void Clear()
        {
            // Delete the report definition file.
            File.Delete(this.FileName);

            /*string fileNameWorkflow = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "ReportingWorkflows",
                this.Core.ClientName + ".xml"
            );

            string fileNameWeighting = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "WeightingDefaults",
                this.Core.ClientName + ".xml"
            );

            if (!File.Exists(fileNameWeighting))
                fileNameWeighting = null;

            ReportDefinition reportDefinition = new ReportDefinition(
                this.Core,
                this.FileName,
                fileNameWorkflow,
                fileNameWeighting,
                this.HierarchyFilter
            );

            reportDefinition.XmlDocument.DocumentElement.SetAttribute("Name", "new report");

            reportDefinition.Save();*/
        }

        /// <summary>
        /// Clears the aggregated results of the report definition.
        /// </summary>
        public override void ClearData()
        {
            this.DataCheck.Clear();

            XmlNode xmlNodeResults = this.XmlDocument.DocumentElement.SelectSingleNode("Results");

            if (xmlNodeResults == null)
                return;

            xmlNodeResults.ParentNode.RemoveChild(xmlNodeResults);

            this.Save();
        }


        public void ValidateChanges()
        {
            if (this.LeftVariables.Count == 1 && this.LeftVariables[0].IsFake &&
                this.TopVariables.Count == 1 && this.TopVariables[0].IsFake)
            {
                this.LeftVariables[0].XmlNode.ParentNode.RemoveChild(this.LeftVariables[0].XmlNode);
                this.TopVariables[0].XmlNode.ParentNode.RemoveChild(this.TopVariables[0].XmlNode);

                this.Save();
                this.Parse();
            }
        }


        public void AddFilterCategory(Guid idCategory)
        {
            /*this.FilterCategories.Add(idCategory);

            XmlNode xmlNode = this.XmlDocument.CreateElement("Category");

            XmlAttribute xmlAttribute = this.XmlDocument.CreateAttribute("Id");
            xmlAttribute.Value = idCategory.ToString();

            xmlNode.Attributes.Append(xmlAttribute);

            XmlNode xmlNodeFilters = this.XmlDocument.DocumentElement.SelectSingleNode("Filters");

            xmlNodeFilters.AppendChild(xmlNode);*/
        }

        public void RemoveFilterCategory(Guid idCategory)
        {
            /*this.FilterCategories.Remove(idCategory);

            XmlNode xmlNode = this.XmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Filters/Category[@Id=\"{0}\"]",
                idCategory
            ));

            xmlNode.ParentNode.RemoveChild(xmlNode);*/
        }


        public void RemoveVariable(string path)
        {
            XmlNode xmlNodeVariable = this.XmlDocument.SelectSingleNode(path);

            if (xmlNodeVariable == null)
                return;

            // Remove the variable child xml node.
            xmlNodeVariable.ParentNode.RemoveChild(xmlNodeVariable);
        }


        public DataCore.Classes.Data GetFilter()
        {
            DataCore.Classes.ReportCalculator calculator = new DataCore.Classes.ReportCalculator(
                this,
                this.Core,
                HttpContext.Current.Session
            );

            return calculator.GetFilter();
        }

        public bool CheckForNewCategories()
        {
            bool result = false;

            foreach (ReportDefinitionVariable variable in this.LeftVariables)
            {
                if (variable.CheckForNewCategories())
                    result = true;
            }

            foreach (ReportDefinitionVariable variable in this.TopVariables)
            {
                if (variable.CheckForNewCategories())
                    result = true;
            }

            return result;
        }

        #endregion
    }
}
