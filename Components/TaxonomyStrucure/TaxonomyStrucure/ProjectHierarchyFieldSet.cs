using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectHierarchy1
{
    public class ProjectHierarchyFieldSet
    {
        #region Properties

        /// <summary>
        /// Gets or sets the project hierachy of which
        /// the field set is part of.
        /// </summary>
        public ProjectHierarchy ProjectHierachy { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains
        /// the field set definitions.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the id of the field set.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the field set's fields.
        /// </summary>
        public Dictionary<Guid, ProjectHierarchyField> Fields { get; set; }

        public int StudyValuesCount 
        {
            get
            {
                int result = 1;

                // Run through all fields of the field set.
                foreach (ProjectHierarchyField field in this.Fields.Values)
                {
                    if (field.StudyValues.Count > result)
                        result = field.StudyValues.Count;
                }

                return result;
            }
        }

        #endregion


        #region Constructor

        public ProjectHierarchyFieldSet(ProjectHierarchy taxonomyStrucure, XmlNode xmlNode)
        {
            this.ProjectHierachy = taxonomyStrucure;
            this.XmlNode = xmlNode;

            Init();
        }

        #endregion


        #region Methods

        private void Init()
        {
            // Parse the field set's id.
            this.Id = Guid.Parse(this.XmlNode.Attributes["Id"].Value);

            // Parse the field set's fields.
            ParseFields();
        }

        private void ParseFields()
        {
            this.Fields = new Dictionary<Guid, ProjectHierarchyField>();

            // Get all field defintion xml nodes of
            // the field set's xml node.
            XmlNodeList xmlNodes = this.XmlNode.SelectNodes("Field");

            // Run through all field definition xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                // Create a new field by the xml node.
                ProjectHierarchyField field = new ProjectHierarchyField(
                    this.ProjectHierachy, 
                    xmlNode
                );

                // Add the field to the field set's fields.
                this.Fields.Add(
                    field.Id,
                    field
                );
            }
        }


        public bool IsValid()
        {
            // Run through all fields of the field set.
            foreach (ProjectHierarchyField field in this.Fields.Values)
            {
                // Check if the field is valid.
                if (!field.IsValid(this.StudyValuesCount))
                    return false;
            }

            return true;
        }

        #endregion
    }
}
