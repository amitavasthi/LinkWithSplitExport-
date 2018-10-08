using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using ProjectHierarchy1;
using System.Text;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für StudyTaxonomyStructure
    /// </summary>
    public class StudyTaxonomyStructure : IHttpHandler, IRequiresSessionState
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
        /// Gets or sets the id of the currently selected study.
        /// </summary>
        public Guid IdSelectedStudy
        {
            get
            {
                return (Guid)HttpContext.Current.Session["ProjectHierachy_IdSelectedStudy"];
            }
            set
            {
                HttpContext.Current.Session["ProjectHierachy_IdSelectedStudy"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the currently selected language
        /// </summary>
        public int IdLanguage { get { return 2057; } }

        #endregion


        #region Constructor

        public StudyTaxonomyStructure()
        {
            this.Methods = new Dictionary<string, Meth>();

            this.Methods.Add("SetProjectHierachy", SetProjectHierachy);
            this.Methods.Add("SelectHierarchy", SelectHierarchy);

            this.Methods.Add("SetStudyValue", SetStudyValue);
            this.Methods.Add("LoadCategories", LoadCategories);
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

        private void SetProjectHierachy(HttpContext context)
        {
            // Get the id of the study from the http request's parameters.
            this.IdSelectedStudy = Guid.Parse(
                context.Request.Params["IdStudy"]
            );
        }

        private void SelectHierarchy(HttpContext context)
        {
            // Get the full path to the hierarchy
            // file from the http request's parameters.
            string fileNameTemplate = context.Request.Params["FileName"];

            // Build the path to the study's hierarchy file.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "StudyTaxonomyStructures",
                Global.Core.ClientName,
                this.IdSelectedStudy + ".xml"
            );

            // Copy the template file to the study hierarchy file.
            File.Copy(
                fileNameTemplate,
                fileName
            );

            ProjectHierarchy hierarchy = new ProjectHierarchy(fileName);

            hierarchy.Origin = Guid.Parse(
                (new FileInfo(fileNameTemplate)).Name.Replace(".xml", "")
            );

            hierarchy.Save();
        }


        private void SetStudyValue(HttpContext context)
        {
            // Build the path to the study's taxonomy structure file.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "StudyTaxonomyStructures",
                Global.Core.ClientName,
                this.IdSelectedStudy + ".xml"
            );

            ProjectHierarchy structure = new ProjectHierarchy(fileName);

            // Get the xPath to the value xml node
            // from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Select the value xml node.
            XmlNode xmlNode = structure.XmlDocument.SelectSingleNode(xPath);

            // Check if the field value exists.
            if (xmlNode == null)
                return;

            // Get the index of the study value from
            // the http request's parameters.
            int index = int.Parse(context.Request.Params["Index"]);

            // Get the value for the study value
            // from the http request's parameters.
            string value = context.Request.Params["Value"];

            // Select the study value xml node.
            XmlNode xmlNodeStudyValue = xmlNode.SelectSingleNode(string.Format(
                "StudyValue[@Index=\"{0}\"]",
                index
            ));

            // Check if a study value with that index exists.
            if (xmlNodeStudyValue == null)
            {
                xmlNode.InnerXml += string.Format(
                    "<StudyValue Index=\"{0}\">{1}</StudyValue>",
                    index,
                    value
                );
            }
            else
            {
                xmlNodeStudyValue.InnerXml = value;
            }

            // Save the taxonomy structure.
            structure.Save();
        }

        private void LoadCategories(HttpContext context)
        {
            // Get the id of the variables to get the categories
            // from the http request's parameters.
            Guid idVariable = Guid.Parse(
                context.Request.Params["IdVariable"]
            );

            // Get all categories of the variable.
            List<object[]> categories = Global.Core.Categories.GetValues(
                new string[] { "Id" },
                new string[] { "IdVariable" },
                new object[] { idVariable }
            );

            // Create a new string builder which holds the result html string.
            StringBuilder result = new StringBuilder();

            result.Append("<option value=\"\"></option>");

            // Run through all categories of the variable.
            foreach (object[] category in categories)
            {
                // Get the label of the category in
                // the currently selected language.
                string label = (string)Global.Core.CategoryLabels.GetValue(
                    "Label",
                    new string[] { "IdCategory", "IdLanguage" },
                    new object[] { (Guid)category[0], this.IdLanguage }
                );

                result.Append(string.Format(
                    "<option value=\"{0}\">{1}</option>",
                    ((Guid)category[0]).ToString(),
                    label
                ));
            }

            // Write the contents of the result's
            // string builder to the http response.
            context.Response.Write(result.ToString());

            // Set the content type of the
            // http response to plain text.
            context.Response.ContentType = "text/plain";
        }

        #endregion
    }
}