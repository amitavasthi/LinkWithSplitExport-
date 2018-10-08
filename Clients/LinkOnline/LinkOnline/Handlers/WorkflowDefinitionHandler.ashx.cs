using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für WorkflowDefinitionHandler
    /// </summary>
    public class WorkflowDefinitionHandler : WebUtilities.BaseHandler
    {
        
        #region Constructor

        public WorkflowDefinitionHandler()
            : base()
        {
            base.Methods.Add("SelectVariable", SelectVariable);
            base.Methods.Add("RenameWorkflowSelector", RenameWorkflowSelector);
            base.Methods.Add("DeleteWorkflowSelector", DeleteWorkflowSelector);

            base.Methods.Add("SelectWorkflowSelectorItem", EmptyMethod);
            base.Methods.Add("GetSelectedWorkflowItems", EmptyMethod);
        }

        #endregion


        #region Web Methods

        private void SelectVariable(HttpContext context)
        {
            // Get the id of the variable to select
            // from the http request's parameters.
            Guid idVariable = Guid.Parse(
                context.Request.Params["IdVariable"]
            );

            // Get the full path to the current workflow definition file.
            string fileName = context.Request.Params["Source"];

            // Create a new xml document for the workflow definition.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the workflow definition file into the xml document.
            xmlDocument.Load(fileName);

            // Select the xml node that contains the workflow selector definition.
            XmlNode xmlNode;

            xmlNode = xmlDocument.SelectSingleNode(context.Request.Params["Path"]);

            string variableName = (string)Global.Core.TaxonomyVariables.GetValue(
                "Name",
                "Id",
                idVariable
            );

            // Check if a existing selector should be replaced.
            if (xmlNode.Name == "Selection")
            {
                XmlNode xmlNodeVariable = xmlNode.SelectSingleNode("VariableFilter");

                if (xmlNodeVariable == null)
                {
                    xmlNode.InnerXml = string.Format(
                        "<VariableFilter Name=\"select {0}\" VariableName=\"{1}\" IsTaxonomy=\"True\" Mode=\"Multi\"></VariableFilter>",
                        xmlNode.Attributes["Name"].Value,
                        variableName
                    );
                }
                else
                {
                    xmlNodeVariable.Attributes["VariableName"].Value = variableName;
                }
            }
            else
            {
                xmlNode.InnerXml += string.Format(
                    "<Selection Name=\"{0}\"><VariableFilter Name=\"select {0}\" VariableName=\"{0}\" IsTaxonomy=\"True\" Mode=\"Multi\"></VariableFilter></Selection>",
                    variableName
                );
            }

            // Save the report definition.
            xmlDocument.Save(fileName);
        }

        private void RenameWorkflowSelector(HttpContext context)
        {
            // Get the full path to the current workflow definition file.
            string fileName = context.Request.Params["Source"];

            // Create a new xml document for the workflow definition.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the workflow definition file into the xml document.
            xmlDocument.Load(fileName);

            // Select the xml node that contains the workflow selector definition.
            XmlNode xmlNode = xmlDocument.SelectSingleNode(context.Request.Params["Path"]);

            if (xmlNode == null)
                return;

            string name = context.Request.Params["Name"];

            xmlNode.Attributes["Name"].Value = name;

            XmlNode xmlNodeVariable = xmlNode.SelectSingleNode("VariableFilter");

            if (xmlNodeVariable == null)
                return;

            xmlNodeVariable.Attributes["Name"].Value = "select " + name;

            xmlDocument.Save(fileName);
        }

        private void DeleteWorkflowSelector(HttpContext context)
        {
            // Get the full path to the current workflow definition file.
            string fileName = context.Request.Params["Source"];

            // Create a new xml document for the workflow definition.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the contents of the workflow definition file into the xml document.
            xmlDocument.Load(fileName);

            // Select the xml node that contains the workflow selector definition.
            XmlNode xmlNode = xmlDocument.SelectSingleNode(context.Request.Params["Path"]);

            if (xmlNode == null)
                return;

            xmlNode.ParentNode.RemoveChild(xmlNode);

            xmlDocument.Save(fileName);
        }


        private void EmptyMethod(HttpContext context) { }

        #endregion


        #region Methods

        #endregion
    }
}