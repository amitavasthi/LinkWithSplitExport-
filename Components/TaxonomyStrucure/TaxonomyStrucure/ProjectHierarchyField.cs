using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebUtilities.Controls;

namespace ProjectHierarchy1
{
    public class ProjectHierarchyField
    {
        #region Properties

        /// <summary>
        /// Gets or sets the taxonomy strucure
        /// of which the field is part of.
        /// </summary>
        public ProjectHierarchy TaxonomyStructure { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains
        /// the field definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        public ProjectHierarchyFieldType Type { get; set; }

        /// <summary>
        /// Gets or sets the id of the field.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the labels of the field
        /// seperated by the language id.
        /// </summary>
        public Dictionary<int, string> Labels { get; set; }

        /// <summary>
        /// Gets or sets the field's values.
        /// </summary>
        public Dictionary<Guid, ProjectHierarchyFieldValue> Values { get; set; }

        /// <summary>
        /// Gets or sets the study values of the field value.
        /// </summary>
        public Dictionary<int, ProjectHierarchyStudyValue> StudyValues { get; set; }

        #endregion


        #region Constructor

        public ProjectHierarchyField(ProjectHierarchy taxonomyStrucure, XmlNode xmlNode)
        {
            this.TaxonomyStructure = taxonomyStrucure;
            this.XmlNode = xmlNode;

            Init();
        }

        #endregion


        #region Methods

        private void Init()
        {
            // Parse the field's id.
            this.Id = Guid.Parse(this.XmlNode.Attributes["Id"].Value);

            // Parse the field's type.
            this.Type = (ProjectHierarchyFieldType)Enum.Parse(
                typeof(ProjectHierarchyFieldType),
                this.XmlNode.Attributes["Type"].Value
            );

            // Parse the field's labels.
            ParseLabels();

            // Parse the field's values.
            ParseValues();

            // Parse the value's study values.
            ParseStudyValues();
        }

        private void ParseLabels()
        {
            // Create a new dicitionary that contains the
            // value's labels seperated by the language id.
            this.Labels = new Dictionary<int, string>();

            // Run through all label xml nodes of the field's child nodes.
            foreach (XmlNode xmlNodeLabel in this.XmlNode.SelectNodes("Label"))
            {
                // Parse the language id of the label.
                int idLanguage = int.Parse(xmlNodeLabel.Attributes["IdLanguage"].Value);

                // Get the label text.
                string label = xmlNodeLabel.InnerXml;

                // Add the label to the value's labels.
                this.Labels.Add(idLanguage, label);
            }
        }

        private void ParseValues()
        {
            this.Values = new Dictionary<Guid, ProjectHierarchyFieldValue>();

            // Select all value definition xml
            // nodes of the field's xml node.
            XmlNodeList xmlNodes = this.XmlNode.SelectNodes("Value");

            // Run through all value definition xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                // Create a new value by the xml node.
                ProjectHierarchyFieldValue value = new ProjectHierarchyFieldValue(
                    this,
                    xmlNode
                );

                // Add the value to the field's values.
                this.Values.Add(
                    value.Id,
                    value
                );
            }
        }

        private void ParseStudyValues()
        {
            // Select all study value xml nodes.
            XmlNodeList xmlNodes = this.XmlNode.SelectNodes("StudyValue");

            // Create a new dictionary that contains the study
            // values of the field value sperated by the index.
            this.StudyValues = new Dictionary<int, ProjectHierarchyStudyValue>();

            // Run through all study value xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                // Create a new study value by the xml node.
                ProjectHierarchyStudyValue value = new ProjectHierarchyStudyValue(
                    this,
                    xmlNode
                );

                this.StudyValues.Add(
                    value.Index,
                    value
                );
            }
        }

        public void AddStudyValue(string value)
        {
            this.XmlNode.InnerXml += string.Format(
                "<StudyValue Index=\"{0}\">{1}</StudyValue>",
                value
            );

            ParseStudyValues();
        }


        public bool IsValid(int count)
        {
            int studyValues = 0;

            // Run through all study values of the field.
            foreach (ProjectHierarchyStudyValue studyValue in this.StudyValues.Values)
            {
                if (studyValue.Value.Trim() != "")
                    studyValues++;
            }

            if (studyValues < count)
                return false;

            return true;
        }


        public Panel Render(int idLanguage)
        {
            Panel pnlField = new Panel();
            pnlField.ID = "pnlField" + this.Id;
            pnlField.CssClass = "TaxonomyStructureField BackgroundColor1";
            pnlField.Style.Add("opacity", "0.0");

            pnlField.Attributes.Add("oncontextmenu", string.Format(
                "DeleteField(this, '{0}');return false;",
                this.XmlNode.GetXPath()
            ));

            Table table = new Table();

            TableRow tableRow = new TableRow();

            TableCell tableCellLabel = new TableCell();
            TableCell tableCellType = new TableCell();

            tableCellLabel.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
            tableCellType.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;

            TextBox txtFieldLabel = new TextBox();
            txtFieldLabel.CssClass = "BackgroundColor1";
            txtFieldLabel.Text = this.Labels[idLanguage];

            txtFieldLabel.Attributes.Add(
                "onchange",
                "SetFieldLabel('" + this.XmlNode.GetXPath() + "', this.value)"
            );

            DropDownList ddlType = new DropDownList();
            ddlType.BindEnum(typeof(ProjectHierarchyFieldType));
            ddlType.SelectedValue = this.Type.ToString();

            ddlType.Attributes.Add(
                "onchange",
                "SetFieldType('" + this.Id + "', '" + this.XmlNode.GetXPath() + "',this.value)"
            );

            tableCellLabel.Controls.Add(txtFieldLabel);
            tableCellType.Controls.Add(ddlType);

            tableRow.Cells.Add(tableCellLabel);
            tableRow.Cells.Add(tableCellType);

            table.Rows.Add(tableRow);

            if (this.Type == ProjectHierarchyFieldType.Single || this.Type == ProjectHierarchyFieldType.Multi)
            {
                tableCellLabel.RowSpan = this.Values.Count + 2;
                tableCellType.RowSpan = tableCellLabel.RowSpan;

                // Run through all values of the field.
                foreach (ProjectHierarchyFieldValue value in this.Values.Values)
                {
                    TableRow tableRowValue = new TableRow();

                    TableCell tableCellValue = new TableCell();

                    tableCellValue.Attributes.Add("oncontextmenu", string.Format(
                        "DeleteFieldValue(this, '{0}');return false;",
                        value.XmlNode.GetXPath()
                    ));

                    TextBox txtValueLabel = new TextBox();
                    txtValueLabel.CssClass = "BackgroundColor1";

                    txtValueLabel.Attributes.Add(
                        "onchange",
                        "SetFieldValueLabel('" + value.XmlNode.GetXPath() + "', this.value)"
                    );

                    txtValueLabel.Text = value.Labels[idLanguage];

                    tableCellValue.Controls.Add(txtValueLabel);

                    tableRowValue.Cells.Add(tableCellValue);

                    table.Rows.Add(tableRowValue);
                }

                TableRow tableRowNewFieldValue = new TableRow();
                TableCell tableCellNewFieldValue = new TableCell();

                Image imgNewFieldValue = new Image();
                imgNewFieldValue.ImageUrl = "/Images/Icons/Add2.png";
                imgNewFieldValue.Style.Add("cursor", "pointer");
                imgNewFieldValue.Attributes.Add("onclick", string.Format(
                    "AddFieldValue('{0}', '{1}');",
                    this.Id,
                    this.XmlNode.GetXPath()
                ));

                tableCellNewFieldValue.Controls.Add(imgNewFieldValue);

                tableRowNewFieldValue.Cells.Add(tableCellNewFieldValue);

                table.Rows.Add(tableRowNewFieldValue);
            }

            pnlField.Controls.Add(table);

            return pnlField;
        }

        #endregion
    }

    public enum ProjectHierarchyFieldType
    {
        Single,
        Multi,
        Language,
        Date,
        Text,
        Numeric,
        CategorySelector
    }
}
