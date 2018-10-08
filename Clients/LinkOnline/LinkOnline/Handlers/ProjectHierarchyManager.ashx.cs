using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using ProjectHierarchy1;
using WebUtilities.Controls;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für ProjectHierarchyManager
    /// </summary>
    public class ProjectHierarchyManager : IHttpHandler, IRequiresSessionState
    {
        #region Properties

        /// <summary>
        /// Gets or sets the available methods of the generic handler.
        /// </summary>
        public Dictionary<string, Meth> Methods { get; set; }

        /// <summary>
        /// Gets if the generic handler is re useable.
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected taxonomy strucure.
        /// </summary>
        public string SelectedTaxonomyStrucure
        {
            get
            {
                if (HttpContext.Current.Session["TaxonomyStructureManagerSelectedTaxonomyStructure"] == null)
                    return null;

                return (string)HttpContext.Current.Session["TaxonomyStructureManagerSelectedTaxonomyStructure"];
            }
        }

        /// <summary>
        /// Gets or sets the currently selected language.
        /// </summary>
        public int IdLanguage { get { return 2057; } }

        #endregion


        #region Constructor

        public ProjectHierarchyManager()
        {
            this.Methods = new Dictionary<string, Meth>();

            this.Methods.Add("GetFields", GetFields);
            this.Methods.Add("UpdateField", UpdateField);
            this.Methods.Add("DeleteField", DeleteField);
            this.Methods.Add("DeleteFieldValue", DeleteFieldValue);
            this.Methods.Add("SetFieldLabel", SetFieldLabel);
            this.Methods.Add("SetFieldValueLabel", SetFieldValueLabel);
            this.Methods.Add("SetFieldType", SetFieldType);
            this.Methods.Add("AddFieldValue", AddFieldValue);
            this.Methods.Add("AddField", AddField);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public void ProcessRequest(HttpContext context)
        {
            // Check if the current session has an authenticated user.
            if (HttpContext.Current.Session["User"] == null)
                throw new Exception("Not authenticated.");

            // Get the requested method name from the http request.
            string method = context.Request.Params["Method"];

            // Check if the requested method exists.
            if (!this.Methods.ContainsKey(method))
                throw new NotImplementedException();

            // Invoke the requested method.
            this.Methods[method].Invoke(context);
        }

        #endregion


        #region Web Methods

        private void GetFields(HttpContext context)
        {
            if (this.SelectedTaxonomyStrucure == null)
                return;

            ProjectHierarchy taxonomyStructure = new ProjectHierarchy(this.SelectedTaxonomyStrucure);

            string xPath = context.Request.Params["XPath"];

            XmlNode xmlNode = taxonomyStructure.XmlDocument.SelectSingleNode(xPath);

            if (xmlNode == null)
                return;

            StringBuilder result = new StringBuilder();

            // Run through all field xml nodes of the xml node.
            foreach (XmlNode xmlNodeField in xmlNode.SelectNodes("Field"))
            {
                ProjectHierarchyField field = new ProjectHierarchyField(taxonomyStructure, xmlNodeField);

                Panel pnlField = field.Render(this.IdLanguage);

                result.Append(pnlField.ToHtml());
                result.Append("<br />");
            }

            context.Response.Write(result.ToString());

            // Set the http response's content type to plain text.
            context.Response.ContentType = "text/plain";
        }

        private void UpdateField(HttpContext context)
        {
            if (this.SelectedTaxonomyStrucure == null)
                return;

            ProjectHierarchy taxonomyStructure = new ProjectHierarchy(this.SelectedTaxonomyStrucure);

            // Parse the xPath of the field's xml node to
            // get from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Select the field's xml node.
            XmlNode xmlNode = taxonomyStructure.XmlDocument.SelectSingleNode(xPath);

            // Check if the field exists.
            if (xmlNode == null)
                return;

            ProjectHierarchyField field = new ProjectHierarchyField(taxonomyStructure, xmlNode);

            Panel pnlField = field.Render(this.IdLanguage);

            // Write the field's panel as html to the http response.
            context.Response.Write(pnlField.ToHtml());

            // Set the http response's content type to plain text.
            context.Response.ContentType = "text/plain";
        }

        private void DeleteField(HttpContext context)
        {
            if (this.SelectedTaxonomyStrucure == null)
                return;

            ProjectHierarchy taxonomyStructure = new ProjectHierarchy(this.SelectedTaxonomyStrucure);

            // Get the xPath of the field's xml node
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Select the field's xml node.
            XmlNode xmlNode = taxonomyStructure.XmlDocument.SelectSingleNode(xPath);

            // Check if the field exists.
            if (xmlNode == null)
                return;

            // Delete the field.
            xmlNode.ParentNode.RemoveChild(xmlNode);

            // Save the taxonomy structure.
            taxonomyStructure.Save();
        }

        private void DeleteFieldValue(HttpContext context)
        {
            if (this.SelectedTaxonomyStrucure == null)
                return;

            ProjectHierarchy taxonomyStructure = new ProjectHierarchy(this.SelectedTaxonomyStrucure);

            // Get the xPath of the field value's xml node
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Select the field value's xml node.
            XmlNode xmlNode = taxonomyStructure.XmlDocument.SelectSingleNode(xPath);

            // Check if the field value exists.
            if (xmlNode == null)
                return;

            // Delete the field value.
            xmlNode.ParentNode.RemoveChild(xmlNode);

            // Save the taxonomy structure.
            taxonomyStructure.Save();
        }

        private void SetFieldLabel(HttpContext context)
        {
            if (this.SelectedTaxonomyStrucure == null)
                return;

            ProjectHierarchy taxonomyStructure = new ProjectHierarchy(this.SelectedTaxonomyStrucure);

            // Get the xPath of the field's xml node
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Get the new label for the field
            // from the http request's parameters.
            string label = context.Request.Params["Value"];

            // Select the field's xml node.
            XmlNode xmlNode = taxonomyStructure.XmlDocument.SelectSingleNode(xPath);

            // Check if the field exists.
            if (xmlNode == null)
                return;

            // Select the label xml node.
            XmlNode xmlNodeLabel = xmlNode.SelectSingleNode(string.Format(
                "Label[@IdLanguage=\"{0}\"]",
                this.IdLanguage
            ));

            // Check if a label in that language exists.
            if (xmlNodeLabel == null)
            {
                // Add a new label in that language.
                xmlNode.InnerXml += string.Format(
                    "<Label IdLanguage=\"{0}\">{1}</Label>",
                    this.IdLanguage,
                    label
                );
            }
            else
            {
                // Set the new label.
                xmlNodeLabel.InnerXml = label;
            }

            // Save the taxonomy structure.
            taxonomyStructure.Save();
        }

        private void SetFieldValueLabel(HttpContext context)
        {
            if (this.SelectedTaxonomyStrucure == null)
                return;

            ProjectHierarchy taxonomyStructure = new ProjectHierarchy(this.SelectedTaxonomyStrucure);

            // Get the xPath of the field value's xml node
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Get the new label for the field value
            // from the http request's parameters.
            string label = context.Request.Params["Value"];

            // Select the field value's xml node.
            XmlNode xmlNode = taxonomyStructure.XmlDocument.SelectSingleNode(xPath);

            // Check if the field value exists.
            if (xmlNode == null)
                return;

            // Select the label xml node.
            XmlNode xmlNodeLabel = xmlNode.SelectSingleNode(string.Format(
                "Label[@IdLanguage=\"{0}\"]",
                this.IdLanguage
            ));

            // Check if a label in that language exists.
            if (xmlNodeLabel == null)
            {
                // Add a new label in that language.
                xmlNode.InnerXml += string.Format(
                    "<Label IdLanguage=\"{0}\">{1}</Label>",
                    this.IdLanguage,
                    label
                );
            }
            else
            {
                // Set the new label.
                xmlNodeLabel.InnerXml = label;
            }

            // Save the taxonomy structure.
            taxonomyStructure.Save();
        }

        private void SetFieldType(HttpContext context)
        {
            if (this.SelectedTaxonomyStrucure == null)
                return;

            ProjectHierarchy taxonomyStructure = new ProjectHierarchy(this.SelectedTaxonomyStrucure);

            // Get the xPath of the field's xml node
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Get the new type for the field
            // from the http request's parameters.
            ProjectHierarchyFieldType type = (ProjectHierarchyFieldType)Enum.Parse(
                typeof(ProjectHierarchyFieldType),
                context.Request.Params["Value"]
            );

            // Select the field's xml node.
            XmlNode xmlNode = taxonomyStructure.XmlDocument.SelectSingleNode(xPath);

            // Check if the field exists.
            if (xmlNode == null)
                return;

            // Set new type of the field.
            xmlNode.Attributes["Type"].Value = type.ToString();

            // Save the taxonomy structure.
            taxonomyStructure.Save();
        }

        private void AddFieldValue(HttpContext context)
        {
            if (this.SelectedTaxonomyStrucure == null)
                return;

            ProjectHierarchy taxonomyStructure = new ProjectHierarchy(this.SelectedTaxonomyStrucure);

            // Get the xPath of the field's xml node
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Select the field's xml node.
            XmlNode xmlNode = taxonomyStructure.XmlDocument.SelectSingleNode(xPath);

            // Check if the field exists.
            if (xmlNode == null)
                return;

            // Add a new value xml node to the field's xml node.
            xmlNode.InnerXml += string.Format(
                "<Value Id=\"{0}\"><Label IdLanguage=\"{1}\">{2}</Label></Value>",
                Guid.NewGuid(),
                this.IdLanguage,
                Global.LanguageManager.GetText("NewTaxonomyFieldValue")
            );

            // Save the taxonomy structure.
            taxonomyStructure.Save();
        }

        private void AddField(HttpContext context)
        {
            if (this.SelectedTaxonomyStrucure == null)
                return;

            ProjectHierarchy taxonomyStructure = new ProjectHierarchy(this.SelectedTaxonomyStrucure);

            string xPath = context.Request.Params["XPath"];

            XmlNode xmlNode = taxonomyStructure.XmlDocument.SelectSingleNode(xPath);

            if (xmlNode == null)
                return;

            xmlNode.InnerXml += string.Format(
                "<Field Id=\"{0}\" Type=\"Single\"><Label IdLanguage=\"{1}\"></Label></Field>",
                Guid.NewGuid(),
                this.IdLanguage
            );

            taxonomyStructure.Save();
        }

        #endregion
    }
}