using Crosstables.Classes.WorkflowClasses;
using LinkBi1.Classes;
using LinkBi1.Classes.Interfaces;
using LinkBi1.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using System.Xml.XPath;
using VariableSelector1.Classes;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für LinkBi
    /// </summary>
    public class LinkBi : IHttpHandler, IRequiresSessionState
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

        #endregion


        #region Constructor

        public LinkBi()
        {
            this.Methods = new Dictionary<string, Meth>();

            this.Methods.Add("RemoveFilter", RemoveFilter);
            this.Methods.Add("RemoveMeasure", RemoveMeasure);
            this.Methods.Add("GetProgress", GetProgress);

            this.Methods.Add("GetSelectedWorkflowItems", GetSelectedWorkflowItems);
            this.Methods.Add("SelectWorkflowSelectorItem", SelectWorkflowSelectorItem);

            this.Methods.Add("SwitchLinkBiDefinition", SwitchLinkBiDefinition);
            this.Methods.Add("ClearLinkBiDefinition", ClearLinkBiDefinition);
            this.Methods.Add("Save", Save);

            this.Methods.Add("UpdateLinkBiSetting", UpdateLinkBiSetting);
            this.Methods.Add("UpdateLinkBiWeightingVariable", UpdateLinkBiWeightingVariable);

            this.Methods.Add("ToggleLinkBiSelectorCategory", ToggleLinkBiSelectorCategory);


            this.Methods.Add("DeleteSavedReport", DeleteSavedReport);
            this.Methods.Add("LoadLinkBiDefinitionProperties", LoadLinkBiDefinitionProperties);
            this.Methods.Add("UpdateLinkBiSavedReportName", UpdateLinkBiSavedReportName);
            this.Methods.Add("ChangeServerConnectionType", ChangeServerConnectionType);
            this.Methods.Add("AddServerConnection", AddServerConnection);
            this.Methods.Add("DeleteServerConnection", DeleteServerConnection);
            this.Methods.Add("UpdateServerConnectionProperty", UpdateServerConnectionProperty);

            this.Methods.Add("DeployLinkBiReport", DeployLinkBiReport);
            this.Methods.Add("CheckLinkBiServerConnection", CheckLinkBiServerConnection);
            this.Methods.Add("IsOutdated", IsOutdated);

            this.Methods.Add("SetActiveLinkBiDefinition", SetActiveLinkBiDefinition);

            this.Methods.Add("Test", Test);

            this.Methods.Add("BuildAPIRequestString", BuildAPIRequestString);
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


        private void RemoveFilter(HttpContext context)
        {
            Guid idVariable = Guid.Parse(
                context.Request.Params["Id"]
            );

            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            LinkBiDefinition definition = new LinkBiDefinition(Global.Core, fileName, Global.HierarchyFilters[fileName]);

            definition.Properties.LatestUpdate = DateTime.Now;

            definition.RemoveDimension(idVariable);
        }

        private void RemoveMeasure(HttpContext context)
        {
            Guid idVariable = Guid.Parse(
                context.Request.Params["Id"]
            );

            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            LinkBiDefinition definition = new LinkBiDefinition(Global.Core, fileName, Global.HierarchyFilters[fileName]);

            definition.Properties.LatestUpdate = DateTime.Now;

            definition.RemoveMeasure(idVariable);
        }

        private void GetProgress(HttpContext context)
        {
            if (HttpContext.Current.Session["CurrentLinkBiCreation"] == null)
            {
                context.Response.Write("-1");
            }
            else
            {
                LinkBiInterface linkBiInterface = (LinkBiInterface)HttpContext.Current.Session["CurrentLinkBiCreation"];

                context.Response.Write(linkBiInterface.Progress);
            }

            context.Response.ContentType = "text/plain";
        }


        private void GetSelectedWorkflowItems(HttpContext context)
        {
            string workflowSelection = context.Request.Params["WorkflowSelection"];
            string workflowSelectionSelector = context.Request.Params["WorkflowSelectionSelector"];

            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            LinkBiDefinition reportDefinition = new LinkBiDefinition(Global.Core, fileName, Global.HierarchyFilters[fileName]);

            List<Guid> selectedItems = reportDefinition.Workflow.Selections[workflowSelection].SelectionVariables[workflowSelectionSelector].Selector.SelectedItems;

            context.Response.Write(string.Join(",", selectedItems.ToArray()));
        }

        private void SelectWorkflowSelectorItem(HttpContext context)
        {
            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            string workflowSelection = context.Request.Params["WorkflowSelection"];
            string workflowSelectionVariable = context.Request.Params["WorkflowSelectionVariable"];
            string action = context.Request.Params["Action"];

            Guid idItem = Guid.Parse(
                context.Request.Params["IdItem"]
            );

            LinkBiDefinition reportDefinition = new LinkBiDefinition(Global.Core, fileName, Global.HierarchyFilters[fileName]);

            bool isDefault = reportDefinition.Workflow.Selections[workflowSelection].SelectionVariables[workflowSelectionVariable].IsDefaultSelection;

            if (action == "Select")
            {
                reportDefinition.Workflow.Selections[workflowSelection].SelectionVariables[workflowSelectionVariable].Select(idItem);
            }
            else if (action == "DeSelect")
            {
                reportDefinition.Workflow.Selections[workflowSelection].SelectionVariables[workflowSelectionVariable].DeSelect(idItem);
            }

            bool hasSelection = false;

            foreach (WorkflowSelectionSelector selectionVariable in reportDefinition.Workflow.Selections[workflowSelection].SelectionVariables.Values)
            {
                if (selectionVariable.Selector.SelectedItems.Count > 0)
                {
                    hasSelection = true;
                    break;
                }
            }

            string result = "BackgroundColor8";

            if (hasSelection)
                result = "BackgroundColor1";

            context.Response.Write(result);
            context.Response.ContentType = "text/plain";

            reportDefinition.Save();
        }


        private void SwitchLinkBiDefinition(HttpContext context)
        {
            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            XmlNode xmlNodeDimensions = document.DocumentElement.SelectSingleNode("Dimensions");
            XmlNode xmlNodeMeasures = document.DocumentElement.SelectSingleNode("Measures");

            string xmlStringDimensions = xmlNodeDimensions.InnerXml;
            xmlNodeDimensions.InnerXml = xmlNodeMeasures.InnerXml;
            xmlNodeMeasures.InnerXml = xmlStringDimensions;

            document.Save(fileName);
        }

        private void ClearLinkBiDefinition(HttpContext context)
        {
            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            if (File.Exists(fileName))
                File.Delete(fileName);
        }


        private void UpdateLinkBiSetting(HttpContext context)
        {
            // Get the name of the setting to update
            // from the http request's parameters.
            string name = context.Request.Params["Name"];

            // Get the new value for the setting
            // from the http request's parameters.
            string value = context.Request.Params["Value"];

            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            LinkBiDefinition linkBiDefinition = new LinkBiDefinition(Global.Core, fileName, Global.HierarchyFilters[fileName]);

            if (linkBiDefinition.Settings.Values.ContainsKey(name))
                linkBiDefinition.Settings.Values[name] = value;
            else
                linkBiDefinition.Settings.Values.Add(name, value);

            linkBiDefinition.Properties.LatestUpdate = DateTime.Now;

            linkBiDefinition.Save();
        }

        private void UpdateLinkBiWeightingVariable(HttpContext context)
        {
            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            LinkBiDefinition linkBiDefinition = new LinkBiDefinition(Global.Core, fileName, Global.HierarchyFilters[fileName]);

            Guid idVariable;

            if (Guid.TryParse(context.Request.Params["IdVariable"], out idVariable))
            {
                linkBiDefinition.WeightingFilters.DefaultWeighting = idVariable;
            }
            else
            {
                linkBiDefinition.WeightingFilters.DefaultWeighting = null;
            }

            linkBiDefinition.Save();
        }


        private void ToggleLinkBiSelectorCategory(HttpContext context)
        {
            // Parse the id of the category to toggle.
            Guid idCategory = Guid.Parse(
                context.Request.Params["IdCategory"]
            );

            // Get the xPath to the LinkBi filter item's xml node.
            string xPath = context.Request.Params["XPath"];

            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            LinkBiDefinition linkBiDefinition = new LinkBiDefinition(Global.Core, fileName, Global.HierarchyFilters[fileName]);

            XmlNode xmlNode = linkBiDefinition.XmlDocument.SelectSingleNode(xPath);

            if (xmlNode == null)
                return;

            XmlNode xmlNodeCategory = xmlNode.SelectSingleNode(string.Format(
                "*[@Id=\"{0}\"]",
                idCategory
            ));

            string result = "";

            if (xmlNodeCategory == null)
            {
                xmlNode.InnerXml += string.Format(
                    "<{0} Id=\"{1}\"></{0}>",
                    "TaxonomyCategory",
                    idCategory
                );

                result = "BackgroundColor2";
            }
            else
            {
                xmlNode.RemoveChild(xmlNodeCategory);

                result = "Color1";
            }

            linkBiDefinition.Save();

            context.Response.Write(result);
            context.Response.ContentType = "text/plain";
        }


        private void DeleteSavedReport(HttpContext context)
        {
            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Check if the saved report exists.
            if (File.Exists(fileName))
            {
                // Delete the saved report.
                File.Delete(fileName);
            }
        }

        private void LoadLinkBiDefinitionProperties(HttpContext context)
        {
            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Set the LinkBi definition file to the currently selected LinkBi definition.
            HttpContext.Current.Session["LinkBiSelectedReport"] = fileName;

            // Create a new LinkBi definition by the file.
            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Create a new properties control by the LinkBi definition.
            LinkBiDefinitionPropertiesControl propertiesControl = new LinkBiDefinitionPropertiesControl(definition);

            // Render the properties control.
            propertiesControl.Render();

            // Write the properties control as html string to the http response.
            context.Response.Write(propertiesControl.ToHtml());

            // Set the content type of the http response to plain text.
            context.Response.ContentType = "text/plain";
        }

        private void UpdateLinkBiSavedReportName(HttpContext context)
        {
            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Create a new LinkBi definition by the file.
            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Change the name of the saved report.
            definition.Properties.Name = context.Request.Params["Value"];

            // Save the LinkBi definition.
            definition.Save();
        }

        private void ChangeServerConnectionType(HttpContext context)
        {
            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Get the xPath to the server connection xml
            // node from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Create a new LinkBi definition by the file.
            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Select the server connection type xml node.
            XmlNode xmlNodeServerConnectionType = definition.XmlDocument.SelectSingleNode(xPath + "/Type");

            // Check if the server connection exists.
            if (xmlNodeServerConnectionType == null)
                return;

            // Set the new server connection type.
            xmlNodeServerConnectionType.InnerXml = context.Request.Params["Value"];

            // Save the LinkBi definition.
            definition.Save();
        }

        private void AddServerConnection(HttpContext context)
        {
            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Create a new LinkBi definition by the file.
            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Select the xml node that contains the server connection definitions.
            XmlNode xmlNodeServerConnections = definition.XmlDocument.DocumentElement.
                SelectSingleNode("Properties/ServerConnections");

            // Check if the xml node exists.
            if (xmlNodeServerConnections == null)
                return;

            // Add a new server connection xml node.
            xmlNodeServerConnections.InnerXml += string.Format(
                "<ServerConnection Id=\"{0}\"></ServerConnection>",
                Guid.NewGuid()
            );

            // Save the LinkBi definition.
            definition.Save();
        }

        private void DeleteServerConnection(HttpContext context)
        {
            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Get the xPath to the server connection xml
            // node from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Create a new LinkBi definition by the file.
            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Select the server connection xml node.
            XmlNode xmlNodeServerConnection = definition.XmlDocument.SelectSingleNode(xPath);

            // Check if the server connection exists.
            if (xmlNodeServerConnection == null)
                return;

            // Delete the server connection xml node.
            xmlNodeServerConnection.ParentNode.RemoveChild(xmlNodeServerConnection);

            // Save the LinkBi definition.
            definition.Save();
        }

        private void UpdateServerConnectionProperty(HttpContext context)
        {
            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Get the xPath to the server connection's xml
            // node from the http request's parameters.
            string xPath = context.Request.Params["XPath"];

            // Create a new LinkBi definition by the file.
            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Select the xml node that contains the server connection definitions.
            XmlNode xmlNodeServerConnection = definition.XmlDocument.SelectSingleNode(xPath);

            // Get the name of the field to update
            // from the http request's parameters.
            string name = context.Request.Params["Name"];

            // Get the value of the field to update
            // from the http request's parameters.
            string value = context.Request.Params["Value"];

            // Select the xml node that contains the field definition.
            XmlNode xmlNodeField = xmlNodeServerConnection.SelectSingleNode(name);

            // Check if the property exists.
            if (xmlNodeField == null)
            {
                xmlNodeServerConnection.InnerXml += string.Format(
                    "<{0}>{1}</{0}>",
                    name,
                    value
                );
            }
            else
            {
                xmlNodeField.InnerXml = value;
            }

            // Save the LinkBi definition.
            definition.Save();
        }


        private void DeployLinkBiReport(HttpContext context)
        {
            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Create a new LinkBi definition by the file.
            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            Guid? idServerConnection = null;

            // Check if the report should be deployed to a certain server connection.
            if (context.Request.Params["IdServerConnection"] != null)
                idServerConnection = Guid.Parse(context.Request.Params["IdServerConnection"]);

            bool success = false;

            if (idServerConnection.HasValue)
            {
                success = definition.Deploy(idServerConnection.Value);
            }
            else
            {
                // Deploy the LinkBi definition to all server connections.
                success = definition.Deploy();
            }

            definition.Save();

            if (idServerConnection.HasValue)
            {
                context.Response.Write(idServerConnection.Value.ToString() + "|" + success.ToString().ToLower());
            }
            else
            {
                context.Response.Write(success.ToString().ToLower());
            }
        }

        private void CheckLinkBiServerConnection(HttpContext context)
        {
            bool result = false;

            // Get the id of the server connection to
            // check from the http request's parameters.
            Guid idServerConnection = Guid.Parse(
                context.Request.Params["IdServerConnection"]
            );

            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Create a new LinkBi definition by the file.
            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            if (definition.Properties.ServerConnections.ContainsKey(idServerConnection))
            {
                result = definition.Properties.ServerConnections[idServerConnection].IsValid();
            }

            context.Response.Write(result.ToString().ToLower());
        }

        private void IsOutdated(HttpContext context)
        {
            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            // Create a new LinkBi definition by the file.
            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            context.Response.Write((definition.IsUpToDate() == false).ToString().ToLower());
        }


        private void SetActiveLinkBiDefinition(HttpContext context)
        {
            // Get the full path to the LinkBi definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["FileName"];

            HttpContext.Current.Session["LinkBiDefinition"] = fileName;

            LinkBiDefinitionInfo info = new LinkBiDefinitionInfo(fileName);

            if (info.LatestUses.ContainsKey(Global.IdUser.Value))
                info.LatestUses[Global.IdUser.Value] = DateTime.Now;
            else
                info.LatestUses.Add(Global.IdUser.Value, DateTime.Now);

            info.Save();
        }


        private void Test(HttpContext context)
        {
        }


        private void Save(HttpContext context)
        {
            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            // Check if the report exists.
            if (!File.Exists(fileName))
                return;

            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            if (string.IsNullOrEmpty(definition.Properties.Name) == false)
            {
                return;
            }

            // Build the full path for the saved report file.
            string fileNameDestination = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedLinkBiDefinitions",
                Global.Core.ClientName,
                Guid.NewGuid() + ".xml"
            );

            FileInfo fInfo = new FileInfo(fileNameDestination);

            if (!Directory.Exists(fInfo.Directory.FullName))
                Directory.CreateDirectory(fInfo.Directory.FullName);

            // Copy the file to the saved definitions directory.
            File.Copy(
                fileName,
                fileNameDestination
            );

            // Set the currently selected saved report to the new report.
            HttpContext.Current.Session["LinkBiSelectedReport"] = fileNameDestination;

            LinkBiDefinition definition2 = new global::LinkBi1.Classes.LinkBiDefinition(
                Global.Core, 
                fileNameDestination,
                Global.HierarchyFilters[fileName]
            );

            definition2.Properties.Name = string.Format(
                Global.LanguageManager.GetText("NewLinkBiSavedReport"),
                Global.User.FirstName,
                Global.User.LastName
            );

            definition2.Properties.LatestUpdate = DateTime.Now;

            definition2.Save();
        }


        private void BuildAPIRequestString(HttpContext context)
        {
            // Get the full path to the current report's definition file.
            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            // Check if the report exists.
            if (!File.Exists(fileName))
                return;

            LinkBiDefinition definition = new LinkBiDefinition(
                Global.Core,
                fileName,
                Global.HierarchyFilters[fileName]
            );

            // Create a new string builder to build
            // the result http request url.
            StringBuilder result = new StringBuilder();

            result.Append(context.Request.Url.ToString().
                Replace(context.Request.Url.Query, "").
                Replace("LinkBi.ashx", "LinkBiExternal.ashx"));

            result.Append("?Method=ProcessReport");

            // Run through all dimensions of the LinkBi definitions.
            for (int i = 0; i < definition.Dimensions.Count; i++)
            {
                result.Append(string.Format(
                    "&Dimension{0}={1}",
                    i + 1,
                    definition.Dimensions[i].Name
                ));
            }

            // Run through all measures of the LinkBi definitions.
            for (int i = 0; i < definition.Measures.Count; i++)
            {
                result.Append(string.Format(
                    "&Measure{0}={1}",
                    i + 1,
                    definition.Measures[i].Name
                ));
            }

            result.Append("&Filter=");

            foreach (string workflowSelection in definition.Workflow.Selections.Keys)
            {
                foreach (string workflowSelectionVariable in definition.Workflow
                    .Selections[workflowSelection].SelectionVariables.Keys)
                {
                    foreach (Guid idCategory in definition.Workflow.Selections[workflowSelection]
                        .SelectionVariables[workflowSelectionVariable].Selector.SelectedItems)
                    {
                        result.Append(idCategory.ToString());
                        result.Append(",");
                    }
                }
            }

            result.Append("&ResponseType=TABLE");

            result.Append(string.Format(
                "&Username={0}&Password=",
                Global.User.Name
            ));

            context.Response.Write(result.ToString());

            result.Clear();
        }

        #endregion
    }
}