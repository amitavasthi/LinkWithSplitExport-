using Crosstables.Classes;
using Crosstables.Classes.WorkflowClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace LinkOnline.Classes
{
    public class ExportDefinition
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to
        /// the export definition's file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that
        /// contains the export definition.
        /// </summary>
        public XmlDocument Document { get; set; }

        public Workflow Workflow { get; set; }

        /// <summary>
        /// Gets or sets a list of the
        /// selected variables for the export.
        /// </summary>
        public List<Guid> SelectedVariables { get; set; }

        #endregion


        #region Constructor

        public ExportDefinition(string fileName)
        {
            this.FileName = fileName;

            if (!File.Exists(fileName))
                this.Create();

            this.Parse();
        }

        #endregion


        #region Methods

        private void Create()
        {
            if (!Directory.Exists(new FileInfo(this.FileName).DirectoryName))
                Directory.CreateDirectory(new FileInfo(this.FileName).DirectoryName);

            // Build the full path to the
            // export definition template file.
            string template = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "ExportDefinition.xml"
            );

            File.Copy(
                template,
                this.FileName
            );

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(this.FileName);

            /*xmlDocument.DocumentElement.InnerXml += File.ReadAllText(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "ReportingWorkflows",
                Global.Core.ClientName + ".xml"
            ));*/

            xmlDocument.DocumentElement.InnerXml += "<Workflow>" + CreateWorkflow() + "</Workflow>";

            xmlDocument.Save(this.FileName);
        }

        private string CreateWorkflow()
        {
            XmlDocument document = new XmlDocument();

            document.Load(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "ReportingWorkflows",
                Global.Core.ClientName + ".xml"
            ));

            if (document.SelectSingleNode("//Default") != null)
            {
                return document.SelectSingleNode("//Default").InnerXml;
            }
            else { return ""; }
        }

        private void Parse()
        {
            this.SelectedVariables = new List<Guid>();
            this.Document = new XmlDocument();
            this.Document.Load(this.FileName);

            this.Workflow = new Workflow(
                Global.Core,
                this.FileName,
                this.Document.SelectSingleNode("Export/Workflow"),
                "Exports.aspx",
                new Crosstables.Classes.HierarchyClasses.HierarchyFilter(null)
            );

            // Get all xml nodes that define a selected variable.
            XmlNodeList xmlNodes = this.Document.SelectNodes("Export/Variables/Variable");

            foreach (XmlNode xmlNode in xmlNodes)
            {
                this.SelectedVariables.Add(Guid.Parse(
                    xmlNode.Attributes["Id"].Value
                ));
            }
        }

        public void SelectVariable(Guid idVariable)
        {
            if (this.SelectedVariables.Contains(idVariable))
            {
                XmlNode xmlNode = this.Document.SelectSingleNode(string.Format(
                    "Export/Variables/Variable[@Id=\"{0}\"]",
                    idVariable
                ));

                if (xmlNode != null)
                    xmlNode.ParentNode.RemoveChild(xmlNode);

                this.SelectedVariables.Remove(idVariable);
            }
            else
            {
                XmlNode xmlNode = this.Document.SelectSingleNode("Export/Variables");

                xmlNode.InnerXml += string.Format(
                    "<Variable Id=\"{0}\" />",
                    idVariable
                );

                this.SelectedVariables.Add(idVariable);
            }
        }

        public void Save()
        {
            this.Document.Save(this.FileName);
        }

        #endregion
    }
}