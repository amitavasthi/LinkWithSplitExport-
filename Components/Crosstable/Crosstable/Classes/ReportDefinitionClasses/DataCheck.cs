using DatabaseCore.Items;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class DataCheck
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the report definition
        /// file which contains the filter definitions.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the full path to the file which
        /// contains the data check's values.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that
        /// contains the data check values.
        /// </summary>
        public XmlDocument XmlDocument { get; set; }

        /// <summary>
        /// Gets or sets the report calculator that is
        /// used to check if variables has data.
        /// </summary>
        public ReportCalculator Calculator { get; set; }

        /// <summary>
        /// Gets or sets the respondents of the current filter
        /// selection of the report definition.
        /// </summary>
        public Data Filter
        {
            get
            {
                if (HttpContext.Current.Session["DataCheckFilter" + this.Source] == null)
                    this.Filter = this.InitFilter();

                return (Data)HttpContext.Current.Session["DataCheckFilter" + this.Source];
            }
            set
            {
                HttpContext.Current.Session["DataCheckFilter" + this.Source] = value;
            }
        }

        public bool AggregateNonQAData { get; set; }

        public Dictionary<Guid, bool> Values { get; set; }

        #endregion


        #region Constructor

        public DataCheck(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            FileInfo fInfo = new FileInfo(fileName);

            this.Source = fInfo.FullName;
            this.FileName = fInfo.FullName.Replace("\\Fileadmin\\", "\\Fileadmin\\DataChecks\\");

            fInfo = new FileInfo(this.FileName);

            if (!Directory.Exists(fInfo.Directory.FullName))
                Directory.CreateDirectory(fInfo.Directory.FullName);

            this.XmlDocument = new XmlDocument();

            if (!File.Exists(this.FileName))
                File.WriteAllText(this.FileName, "<DataChecks></DataChecks>");

            this.XmlDocument.Load(this.FileName);

            // Create a new report calculator to check if the variable has data.
            this.Calculator = new ReportCalculator(
                null,
                (DatabaseCore.Core)HttpContext.Current.Session["Core"],
                null
            );

            this.Values = new Dictionary<Guid, bool>();

            this.LoadValues();
        }

        #endregion


        #region Methods

        public Data InitFilter()
        {
            Data result = null;

            if (!File.Exists(this.Source))
                return result;

            // Create a new xml document that contains the report
            // definition which contains the filter definitions.
            XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(this.Source);

            // Select the xml node that contains the workflow definition.
            XmlNode xmlNodeWorkflow = xmlDocument.DocumentElement.SelectSingleNode("Workflow");

            // Select the xml node that contains the filter definition.
            XmlNode xmlNodeFilter = xmlDocument.DocumentElement.SelectSingleNode("Filters/Operator");

            bool aggregateNonQAData = false;

            XmlNode xmlNodeSetting = xmlDocument.DocumentElement.SelectSingleNode("Settings/Setting[@Name=\"AggregateNonQAData\"]");

            if (xmlNodeSetting != null)
            {
                bool.TryParse(xmlNodeSetting.InnerXml, out aggregateNonQAData);
            }

            this.AggregateNonQAData = aggregateNonQAData;

            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Calculator.Core,
                this.Calculator
            );

            // Check if a workflow is defined.
            if (xmlNodeWorkflow != null)
            {
                WorkflowClasses.Workflow workflow = new WorkflowClasses.Workflow(
                    this.Calculator.Core,
                    this.Source,
                    xmlNodeWorkflow,
                    "",
                    new HierarchyClasses.HierarchyFilter(null)
                );

                try
                {
                    result = workflow.GetWorkflowFilter(storageMethod, aggregateNonQAData);
                }
                catch { }
            }

            // Check if a filter is defined.
            if (xmlNodeFilter != null)
            {
                FilterCategoryOperator filterOperator = new FilterCategoryOperator(
                    null,
                    xmlNodeFilter,
                    0,
                    this.Source
                );

                try
                {
                    result = filterOperator.GetRespondents(
                        storageMethod,
                        result
                    );
                }
                catch { }
            }

            return result;
        }


        public void Clear()
        {
            // Check if the data check file exists.
            if (File.Exists(this.FileName))
            {
                // Delete the data check file which contains the entries.
                File.Delete(this.FileName);
            }

            this.Filter = null;
        }

        public bool HasData(Variable variable)
        {
            // Forward call.
            return HasData(variable.Id);
        }

        public bool HasData(Guid idVariable)
        {
            if (!File.Exists(this.Source))
                return true;

            if (!this.Values.ContainsKey(idVariable))
            {
                bool result = false;

                // Check if the variable has data.
                if (this.Calculator.HasData(idVariable, true, this.Filter))
                    result = true;

                this.Values.Add(idVariable, result);
            }

            return this.Values[idVariable];
        }

        public bool HasData(Guid idVariable, Data filter)
        {
            if (!File.Exists(this.Source))
                return true;

            if (!this.Values.ContainsKey(idVariable))
            {
                bool result = false;

                // Check if the variable has data.
                if (this.Calculator.HasData(idVariable, true, filter))
                    result = true;

                this.Values.Add(idVariable, result);

                return result;
            }
            else
            {
                return this.Values[idVariable];
            }
        }

        private void LoadValues()
        {
            foreach (XmlNode xmlNode in this.XmlDocument.DocumentElement.ChildNodes)
            {
                this.Values.Add(
                    Guid.Parse(xmlNode.Attributes["IdVariable"].Value),
                    bool.Parse(xmlNode.Attributes["HasData"].Value)
                );
            }
        }


        public void Save()
        {
            if (this.XmlDocument != null)
                this.XmlDocument.Save(this.FileName);
        }

        #endregion
    }
}
