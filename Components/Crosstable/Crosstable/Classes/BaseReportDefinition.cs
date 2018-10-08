using Crosstables.Classes.HierarchyClasses;
using Crosstables.Classes.ReportDefinitionClasses;
using Crosstables.Classes.ReportDefinitionClasses.Collections;
using Crosstables.Classes.WorkflowClasses;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Crosstables.Classes
{
    public abstract class BaseReportDefinition
    {
        #region Properties

        public HierarchyFilter HierarchyFilter { get; set; }

        /// <summary>
        /// Gets or sets the used database core.
        /// </summary>
        public DatabaseCore.Core Core { get; set; }

        /// <summary>
        /// Gets or sets the full path to
        /// the report definition xml file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that
        /// contains the report definition.
        /// </summary>
        public XmlDocument XmlDocument { get; set; }

        /// <summary>
        /// Gets or sets a collection
        /// of filter categories.
        /// </summary>
        public List<FilterCategoryOperator> FilterCategories { get; set; }

        /// <summary>
        /// Gets or sets a collection of
        /// the weighting variables.
        /// </summary>
        public WeightingFilterCollection WeightingFilters { get; set; }

        /// <summary>
        /// Gets or sets the display
        /// settings for the report.
        /// </summary>
        public BaseReportSettings Settings { get; set; }

        public Workflow Workflow { get; set; }

        /// <summary>
        /// Gets or sets a collection of
        /// the selected hierarchy ids.
        /// </summary>
        public List<Guid> IdHierarchies { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of a report definition.
        /// </summary>
        /// <param name="core">The used database core.</param>
        protected BaseReportDefinition(DatabaseCore.Core core, HierarchyFilter hierarchyFilter)
        {
            this.HierarchyFilter = hierarchyFilter;
            this.Core = core;
            this.IdHierarchies = new List<Guid>();
            this.FilterCategories = new List<FilterCategoryOperator>();
        }

        /// <summary>
        /// Creates a new instance of a report definition.
        /// </summary>
        /// <param name="core">The used database core.</param>
        /// <param name="fileName">The full path to the report definition xml file.</param>
        public BaseReportDefinition(DatabaseCore.Core core, string fileName, HierarchyFilter hierarchyFilter)
            : this(core, hierarchyFilter)
        {
            this.FileName = fileName;

            // Create a new xml document.
            this.XmlDocument = new XmlDocument();

            // Load the contents of the report definition
            // xml file into the xml document.
            this.XmlDocument.Load(this.FileName);

            // Parse the report definition xml file.
            ParseBase();
        }

        /// <summary>
        /// Creates a new instance of a report definition.
        /// </summary>
        /// <param name="core">The used database core.</param>
        public BaseReportDefinition(DatabaseCore.Core core, XmlDocument document, HierarchyFilter hierarchyFilter)
            : this(core, hierarchyFilter)
        {
            this.FileName = "";

            // Create a new xml document.
            this.XmlDocument = document;

            // Parse the report definition xml file.
            ParseBase();
        }

        #endregion


        #region Methods

        public void ParseBase()
        {
            this.FilterCategories = new List<FilterCategoryOperator>();
            this.IdHierarchies = new List<Guid>();

            // Select all xml nodes that define a hierarchy filter.
            XmlNodeList xmlNodesHierarchy = this.XmlDocument.DocumentElement.SelectNodes("HierarchyFilter/Hierarchy");

            // Run through all hierarchy filter definition xml nodes.
            foreach (XmlNode xmlNodeHierarchy in xmlNodesHierarchy)
            {
                this.IdHierarchies.Add(Guid.Parse(
                    xmlNodeHierarchy.Attributes["Id"].Value
                ));
            }

            if (System.Web.HttpContext.Current != null)
            {
                CheckWorkflow(System.IO.Path.Combine(
                    System.Web.HttpContext.Current.Request.PhysicalApplicationPath,
                    "App_Data",
                    "ReportingWorkflows",
                    this.Core.ClientName + ".xml"
                ));
            }

            // Select the workflow definition xml node.
            XmlNode xmlNodeWorkflow = this.XmlDocument.DocumentElement.SelectSingleNode("Workflow");

            if (xmlNodeWorkflow != null)
            {
                // Create a new workflow by the xml node.
                this.Workflow = new Workflow(
                    this.Core,
                    this.FileName,
                    xmlNodeWorkflow,
                    "/Handlers/GlobalHandler.ashx",
                    this.HierarchyFilter
                );
            }

            // Select all weighting variable xml nodes.
            XmlNode xmlNodeWeightingVariables = this.XmlDocument.DocumentElement.
                SelectSingleNode("WeightingVariables");

            if (xmlNodeWeightingVariables != null)
            {
                this.WeightingFilters = new WeightingFilterCollection(this, this.Core, xmlNodeWeightingVariables);
            }

            // Select all filter category xml nodes.
            XmlNode xmlNodesFilterCategories = this.XmlDocument.DocumentElement.SelectSingleNode("Filters");

            if (xmlNodesFilterCategories != null)
            {
                // Run through all filter category xml nodes.
                foreach (XmlNode xmlNodeFilterCategory in xmlNodesFilterCategories.ChildNodes)
                {
                    this.FilterCategories.Add(
                        new FilterCategoryOperator(this.WeightingFilters, xmlNodeFilterCategory, 1, this.FileName)
                    );
                }
            }

            this.Parse();
        }

        /// <summary>
        /// Checks if the report's workflow definition is up to date.
        /// </summary>
        public void CheckWorkflow(string fileName)
        {
            XmlNode xmlNodeWorkflow = this.XmlDocument.DocumentElement.SelectSingleNode("Workflow");

            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.WriteAllText(
                    fileName,
                    "<Workflow></Workflow>"
                );
            }

            XmlNode result = GetWorkflowNode(fileName);

            bool takeSelection = false;

            if (xmlNodeWorkflow != null)
            {
                if (xmlNodeWorkflow.InnerXml != null && xmlNodeWorkflow.InnerXml.Trim() != "")
                {
                    // Check if the result and selected hierarchy is the default workflow.
                    if (result == null && xmlNodeWorkflow.Attributes["IdHierarchy"] == null)
                        takeSelection = true;

                    // Check if a hierarchy specific hierarchy is selected.                   
                        if (result != null && xmlNodeWorkflow.Attributes["IdHierarchy"] != null && result.Attributes["IdHierarchy"] != null)
                        {
                        if (xmlNodeWorkflow.Attributes["IdHierarchy"].Value == result.Attributes["IdHierarchy"].Value)
                            takeSelection = true;
                    }
                }

                if (result.Name == "Default" && xmlNodeWorkflow.Attributes["IdHierarchy"] == null)
                    takeSelection = true;

                if (takeSelection)
                {
                    MigrateWorkflowSelection(result, xmlNodeWorkflow);
                }
                if (result.Name != "Default")
                {
                    if (this.IdHierarchies.Count > 0)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(fileName);
                      
                        foreach (XmlNode item in doc.DocumentElement.SelectNodes("HierarchyWorkflow"))
                        {
                            foreach (object IdHierarchy in this.IdHierarchies)
                            {
                                // foreach (XmlNode xmlNodeSelection in item.ChildNodes)
                                {
                                    if (item.Attributes["IdHierarchy"] != null && item.Attributes["IdHierarchy"].Value == IdHierarchy.ToString())
                                    {
                                        foreach (XmlNode item1 in item.SelectNodes("Selection"))
                                        {
                                            XmlNode node = xmlNodeWorkflow.SelectSingleNode(string.Format("Selection[@Id=\"{0}\"]", item1.Attributes["Id"].Value));
                                            if (node == null)
                                                xmlNodeWorkflow.InnerXml += item.InnerXml;
                                        }

                                    }
                                }
                            }
                        }

                        XmlElement root = doc.DocumentElement;
                        List<object> selectedVariables = new List<object>();
                        List<object> selectedHierarchy = new List<object>();
                        foreach (object IdHierarchy in this.IdHierarchies)
                        {
                            foreach (XmlNode item in root.SelectNodes("HierarchyWorkflow"))
                            {
                                if (selectedHierarchy.Contains(item.Attributes["IdHierarchy"].Value))
                                    continue;

                                if (IdHierarchy.ToString() == item.Attributes["IdHierarchy"].Value)
                                {
                                    selectedHierarchy.Add(item.Attributes["IdHierarchy"].Value);
                                    foreach (XmlNode item1 in item.SelectNodes("Selection"))
                                    {
                                        selectedVariables.Remove(item1.Attributes["Id"].Value);
                                    }
                                    break;
                                }
                                if (IdHierarchy.ToString() != item.Attributes["IdHierarchy"].Value)
                                {
                                    foreach (XmlNode item1 in item.SelectNodes("Selection"))
                                    {
                                        selectedVariables.Add(item1.Attributes["Id"].Value);
                                    }
                                }
                            }
                        }
                        foreach (object item in selectedVariables)
                        {
                            XmlNode node = xmlNodeWorkflow.SelectSingleNode(string.Format("Selection[@Id=\"{0}\"]", item));
                            if (node != null)
                                node.ParentNode.RemoveChild(node);
                        }
                        XmlNode xmlNodeDefault = doc.DocumentElement.SelectSingleNode("Default");
                        if (xmlNodeDefault != null)
                        {
                            foreach (XmlNode xmlNodeSelection in xmlNodeDefault.ChildNodes)
                            {
                                XmlNode xmlNode = xmlNodeWorkflow.SelectSingleNode(string.Format("Selection[@Id=\"{0}\"]", xmlNodeSelection.Attributes["Id"].Value));
                                if (xmlNode == null)
                                {
                                    xmlNodeWorkflow.InnerXml += xmlNodeSelection.OuterXml;
                                }
                            }
                        }
                    }
                }
                else
                    xmlNodeWorkflow.InnerXml = result.InnerXml;
                //if (result.Attributes["IdHierarchy"] != null)
                //{
                //    if (xmlNodeWorkflow.Attributes["IdHierarchy"] == null)
                //        xmlNodeWorkflow.AddAttribute("IdHierarchy", result.Attributes["IdHierarchy"].Value);
                //    else
                //        xmlNodeWorkflow.Attributes["IdHierarchy"].Value = result.Attributes["IdHierarchy"].Value;
                //}
                //else if (xmlNodeWorkflow.Attributes["IdHierarchy"] != null)
                //    xmlNodeWorkflow.Attributes.Remove(xmlNodeWorkflow.Attributes["IdHierarchy"]);
            }
        }

        public void MigrateWorkflowSelection(XmlNode result, XmlNode xmlNodeWorkflow)
        {
            foreach (XmlNode xmlNodeSelection in result.ChildNodes)
            {
                foreach (XmlNode xmlNodeFilter in xmlNodeSelection.ChildNodes)
                {
                    XmlNode xmlNodeSelected = xmlNodeWorkflow.SelectSingleNode(string.Format(
                        "Selection[@Id=\"{0}\"]/VariableFilter[@VariableName=\"{1}\"]",
                        xmlNodeSelection.Attributes["Id"].Value,
                        xmlNodeFilter.Attributes["VariableName"].Value
                    ));

                    if (xmlNodeSelected == null)
                        continue;

                    xmlNodeFilter.InnerXml += xmlNodeSelected.InnerXml;
                }
            }
        }

        public XmlNode GetWorkflowNode(string fileName)
        {
            XmlDocument workflowDocument = new XmlDocument();
            workflowDocument.Load(fileName);

            // Select all xml nodes that define a hierarchy specific workflow.
            XmlNodeList xmlNodesHierarchyWorkflows = workflowDocument.
                DocumentElement.SelectNodes("HierarchyWorkflow");

            XmlNode result = null;

            if (xmlNodesHierarchyWorkflows.Count == 0 && workflowDocument.DocumentElement.SelectSingleNode("Default") == null)
            {
                result = workflowDocument.DocumentElement;
                //return;
            }
            else if (xmlNodesHierarchyWorkflows.Count == 0 && workflowDocument.DocumentElement.SelectSingleNode("Default") != null)
            {
                result = workflowDocument.DocumentElement.SelectSingleNode("Default");
            }
            else
            {
                // Run through all xml nodes that define a hierarchy specific workflow.
                foreach (XmlNode xmlNodeHierarchyWorkflow in xmlNodesHierarchyWorkflows)
                {
                    // Parse the id of the hierarchy from
                    // the xml node's attribute values.
                    Guid idHierarchy = Guid.Parse(
                        xmlNodeHierarchyWorkflow.Attributes["IdHierarchy"].Value
                    );

                    // Check if the hierarchy of the
                    // workflow is selected in the report.
                    if (this.IdHierarchies.Contains(idHierarchy))
                    {
                        result = xmlNodeHierarchyWorkflow;
                    }
                }
            }

            if (result == null)
                result = workflowDocument.DocumentElement.SelectSingleNode("Default");

            return result;
        }

        public abstract void ClearData();
        public abstract void Parse();


        public virtual void Save()
        {
            if (!string.IsNullOrEmpty(this.FileName))
            {
                try
                {
                    this.XmlDocument.Save(this.FileName);
                }
                catch
                {

                }
            }
        }


        public Data GetHierarchyFilter(Data filter = null)
        {
            Data result = null;

            if (this.IdHierarchies.Count == 0)
                return filter;

            // Get all the respondents of the assigned studies of the hierarchy.
            List<object[]> respondents = this.Core.Respondents.ExecuteReader(string.Format(
                "SELECT DISTINCT Id FROM Respondents WHERE IdStudy IN (SELECT Id FROM Studies WHERE IdHierarchy IN ({0}) OR IdHierarchy IN (SELECT Id FROM Hierarchies WHERE IdHierarchy IS NULL))",
                string.Join(",", this.IdHierarchies.Select(x => "'" + x + "'")
            )));

            if (respondents.Count != 0)
                result = new Data();

            // Run through all respondents.
            foreach (object[] idRespondent in respondents)
            {
                if (filter != null && filter.Responses.ContainsKey((Guid)idRespondent[0]) == false)
                    continue;

                result.Responses.Add((Guid)idRespondent[0], new double[0]);
            }

            return result;
        }

        #endregion
    }
}
