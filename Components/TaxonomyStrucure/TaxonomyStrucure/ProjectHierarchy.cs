using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectHierarchy1
{
    public class ProjectHierarchy
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the
        /// project hierarchy definition file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that contains
        /// the project hierarchy definitions.
        /// </summary>
        public XmlDocument XmlDocument { get; set; }

        /// <summary>
        /// Gets or sets the project information
        /// of the project hierarchy.
        /// </summary>
        public ProjectHierarchyFieldSet ProjectInformation { get; set; }

        /// <summary>
        /// Gets or sets the study information
        /// of the project hierarchy.
        /// </summary>
        public ProjectHierarchyFieldSet StudyInformation { get; set; }

        /// <summary>
        /// Gets or sets the typology information
        /// of the project hierarchy.
        /// </summary>
        public ProjectHierarchyFieldSet TypologyInformation { get; set; }

        /// <summary>
        /// Gets or sets the name of the project hierarchy.
        /// </summary>
        public string Name
        {
            get
            {
                return this.XmlDocument.DocumentElement.Attributes["Name"].Value;
            }
            set
            {
                this.XmlDocument.DocumentElement.Attributes["Name"].Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the origin project hierarchy template.
        /// </summary>
        public Guid Origin
        {
            get
            {
                return Guid.Parse(this.XmlDocument.DocumentElement.Attributes["Origin"].Value);
            }
            set
            {
                if (this.XmlDocument.DocumentElement.Attributes["Origin"] == null)
                {
                    this.XmlDocument.DocumentElement.AddAttribute("Origin", value.ToString());
                }
                else
                {
                    this.XmlDocument.DocumentElement.Attributes["Origin"].Value = value.ToString();
                }
            }
        }

        #endregion


        #region Constructor

        public ProjectHierarchy(string fileName)
        {
            this.FileName = fileName;

            Init();
        }

        #endregion


        #region Methods

        public void Save()
        {
            this.XmlDocument.Save(this.FileName);
        }


        private void Init()
        {
            // Create a new xml document.
            this.XmlDocument = new XmlDocument();

            // Load the contents of the project hierarchy
            // definition file into the xml document.
            this.XmlDocument.Load(this.FileName);

            ParseProjectInformation();
            ParseStudyInformation();
            ParseTypologyInformation();
        }

        private void ParseProjectInformation()
        {
            // Select the project information definition xml
            // nodes of the project hierarchy's xml document.
            XmlNode xmlNode = this.XmlDocument.DocumentElement.SelectSingleNode("ProjectInformation");

            // Create a new study information by the xml node.
            this.ProjectInformation = new ProjectHierarchyFieldSet(
                this,
                xmlNode
            );
        }

        private void ParseStudyInformation()
        {
            // Select the information definition xml node
            // of the project hierarchy's xml document.
            XmlNode xmlNode = this.XmlDocument.DocumentElement.SelectSingleNode("StudyInformation");

            // Create a new study information by the xml node.
            this.StudyInformation = new ProjectHierarchyFieldSet(
                this,
                xmlNode
            );
        }

        private void ParseTypologyInformation()
        {
            // Select the typology information definition
            // xml node of the project hierarchy's xml document.
            XmlNode xmlNode = this.XmlDocument.DocumentElement.SelectSingleNode("TypologyInformation");

            // Create a new typology information by the xml node.
            this.TypologyInformation = new ProjectHierarchyFieldSet(
                this,
                xmlNode
            );
        }


        public bool IsValid()
        {
            // Check if the project information is valid.
            if (!this.ProjectInformation.IsValid())
                return false;

            // Check if the study information is valid.
            if (!this.StudyInformation.IsValid())
                return false;

            // Check if the typology information is valid.
            if (!this.TypologyInformation.IsValid())
                return false;

            return true;
        }

        #endregion
    }
}
