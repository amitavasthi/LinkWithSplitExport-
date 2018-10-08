using Crosstables.Classes.HierarchyClasses;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;
using WebUtilities.Controls;

namespace LinkOnline.Pages.TaxonomyManager
{
    public partial class WorkflowDefinition : WebUtilities.BasePage
    {
        #region Properties

        TreeView treeView;

        public string FileName
        {
            get
            {
                return Path.Combine(
                    Request.PhysicalApplicationPath,
                    "App_Data",
                    "ReportingWorkflows",
                    Global.Core.ClientName + ".xml"
                );
            }
        }

        #endregion


        #region Constructor

        public WorkflowDefinition()
        {

        }

        #endregion


        #region Methods

        private void BindHierarchies()
        {
            // Get all hierarchies on root level.
            List<object[]> hierarchies = Global.Core.Hierarchies.GetValues(
                new string[] { "Id", "Name" },
                new string[] { "IdHierarchy" },
                new object[] { null }
            );

            treeView = new TreeView("tvHierarchies");

            // Run through all hierarchies on root level.
            foreach (object[] hierarchy in hierarchies)
            {
                TreeViewNode node = RenderHierarchyTreeViewNode(
                    treeView,
                    hierarchy,
                    ""
                );
                node.Attributes.Add("id", "tnHierarchy" + hierarchy[0]);

                treeView.Nodes.Add(node);

                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "LoadHierarchyVariables" + hierarchy[0],
                    "LoadWorkflow(document.getElementById('cphContent__tvnHierarchy" + hierarchy[0] + "'), '" + hierarchy[0] + "', 'Workflow/Default');",
                    true
                );
            }

            pnlHierarchies.Controls.Add(treeView);
        }

        private TreeViewNode RenderHierarchyTreeViewNode(TreeView treeView, object[] hierarchy, string path)
        {
            path += "/" + (string)hierarchy[1];

            // Create a new tree view node for
            // the hierarchy on root level.
            TreeViewNode node = new TreeViewNode(treeView, "tvnHierarchy" + hierarchy[0].ToString(), "");
            node.CssClass = "BackgroundColor5";
            node.Label = string.Format(
                "<div id=\"lblHierarchyName{1}\">{0}</div>",
                (string)hierarchy[1],
                hierarchy[0]
            );

            node.Attributes.Add(
                "Path",
                path
            );
            //for root (default) hierarchy selected.
            if (hierarchy[0].ToString() == "00000000-0000-0000-0000-000000000000")
                node.OnClientClick = string.Format(
               "LoadWorkflow(this, '{0}', '{1}');",
               hierarchy[0],
               HttpUtility.UrlEncode("Workflow/Default")
           );
            else
                node.OnClientClick = string.Format(
                "LoadWorkflow(this, '{0}', '{1}');",
                hierarchy[0],
                HttpUtility.UrlEncode("Workflow/HierarchyWorkflow[@IdHierarchy=\"" + hierarchy[0] + "\"]")
            );

            // Get all hierarchies of the workgroups where the user
            // is assigned to where the hierarchy is the parent.
            List<object[]> childHierarchies = Global.Core.Hierarchies.ExecuteReader(string.Format(
                "SELECT Id, Name FROM [Hierarchies] WHERE IdHierarchy='{1}' AND Id IN (SELECT IdHierarchy FROM WorkgroupHierarchies " +
                "WHERE IdWorkgroup IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser='{0}')) ORDER BY Name",
                Global.IdUser.Value,
                hierarchy[0]
            ));

            // Run through all available child hierarchies of the hierarchy.
            foreach (object[] childHierarchy in childHierarchies)
            {
                node.AddChild(RenderHierarchyTreeViewNode(
                    treeView,
                    childHierarchy,
                    path
                ));
            }

            /*TreeViewNode nodeAdd = new TreeViewNode(treeView, "add_hierarchy" + hierarchy[0], "");
            nodeAdd.Label = "<img src=\"/Images/Icons/Add2.png\" />";

            node.AddChild(nodeAdd);*/

            return node;
        }


        private void RenderWorkflowDefinition()
        {
            // Parse the id of the hierarchy
            // from the http request's parameters.
            Guid idHierarchy = Guid.Parse(Request.Params["IdHierarchy"]);

            // Create a new xml document that contains
            // the client's workflow definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the client's workflow
            // definition file into the xml document.
            document.Load(this.FileName);

            XmlNode xmlNode;

            // Check if the hierarchy is the root level hierarchy.
            if (Global.Core.Hierarchies.GetValue("IdHierarchy", "Id", idHierarchy) == null)
            {
                xmlNode = document.DocumentElement.SelectSingleNode("Default");

                if (xmlNode == null)
                {
                    xmlNode = document.CreateElement("Default");

                    xmlNode.InnerXml = document.DocumentElement.InnerXml;

                    document.DocumentElement.InnerXml = "";

                    document.DocumentElement.AppendChild(xmlNode);

                    document.Save(this.FileName);
                }

                RenderWorkflowDefinition(
                    idHierarchy,
                    xmlNode
                );

                return;
            }

            // Select the xml node that defines the hierarchy dependent workflow.
            xmlNode = document.DocumentElement.SelectSingleNode(string.Format(
                "HierarchyWorkflow[@IdHierarchy=\"{0}\"]",
                idHierarchy
            ));

            // Check if the hierarchy has a workflow defined.
            if (xmlNode == null)
            {
                Response.Write(string.Format(
                    "<div class=\"HierarchyWorkflowNotDefinedMessage Color1\">{0}</div>",
                    string.Format(
                        Global.LanguageManager.GetText("HierarchyWorkflowNotDefined"),
                        Global.Core.Hierarchies.GetValue("Name", "Id", idHierarchy)
                    )
                ));

                Response.Write("<div class=\"HierarchyWorkflowInherited\">");

                RenderWorkflowDefinition(idHierarchy, GetInheritedWorkflowDefinition(
                    document,
                    idHierarchy
                ), false);

                object IdHierarchy = Global.Core.Hierarchies.GetValue(
                        "IdHierarchy",
                        "Id",
                        idHierarchy
                    );
                if (IdHierarchy.ToString() != ("00000000-0000-0000-0000-000000000000") && IdHierarchy != null)
                {
                    xmlNode = GetInheritedWorkflowDefinition(
                    document,
                    idHierarchy
                );
                    if (xmlNode.Name != "Default")
                    {
                        xmlNode = document.DocumentElement.SelectSingleNode("Default");
                        if (xmlNode != null)
                            RenderWorkflowDefinition(
                            idHierarchy,
                            xmlNode
                        );
                    }
                }
                else if (IdHierarchy != null)
                {
                    xmlNode = document.DocumentElement.SelectSingleNode(string.Format(
                                  "HierarchyWorkflow[@IdHierarchy=\"{0}\"]",
                                  idHierarchy
                              ));
                    if (xmlNode != null)
                        RenderWorkflowDefinition(
                        idHierarchy,
                        xmlNode
                    );
                }
                Response.Write("</div>");

                return;
            }

            document.Save(this.FileName);

            RenderWorkflowDefinition(
                idHierarchy,
                xmlNode
            );
            XmlNodeList nodeList;
            nodeList = document.DocumentElement.SelectNodes(string.Format("HierarchyWorkflow[@IdHierarchy!=\"{0}\"]", idHierarchy));

            if (xmlNode.Name != "")
            {
                Response.Write("<div class=\"HierarchyWorkflowInherited\">");

                foreach (XmlNode xmlNodeSelection in nodeList)
                {
                    object IdHierarchy = Global.Core.Hierarchies.GetValue(
                        "Id",
                        "IdHierarchy",
                        xmlNodeSelection.Attributes["IdHierarchy"].Value
                    );
                    if (IdHierarchy != null)
                    {
                        RenderWorkflowDefinition(
                           Guid.Parse(xmlNodeSelection.Attributes["IdHierarchy"].Value),
                           xmlNodeSelection,
                           false
                           );
                    }
                }
                xmlNode = document.DocumentElement.SelectSingleNode("Default");
                if (xmlNode != null)
                    RenderWorkflowDefinition(
                    idHierarchy,
                    xmlNode
                );

            }

        }

        private void RenderWorkflowDefinition(Guid idHierarchy, XmlNode xmlNode, bool editable = true)
        {
            // Create a new string builder to
            // render the result html string.
            StringBuilder result = new StringBuilder();

            // Run through all selections of the workflow definition.
            foreach (XmlNode xmlNodeSelection in xmlNode.SelectNodes("*"))
            {
                if (xmlNodeSelection.Attributes["Id"] == null)
                    xmlNodeSelection.AddAttribute("Id", Guid.NewGuid());

                // Run through all filter selections of the selection.
                foreach (XmlNode xmlNodeFilterSelection in xmlNodeSelection.SelectNodes("*"))
                {
                    // Get the id of the variable of the filter selection.
                    object idVariable = Global.Core.TaxonomyVariables.GetValue(
                        "Id",
                        "Name",
                        xmlNodeFilterSelection.Attributes["VariableName"].Value
                    );

                    // Check if the defined variable exists.
                    if (idVariable == null)
                    {
                        // Remove the invalid filter selection
                        // xml node from the selection xml node.
                        xmlNodeSelection.RemoveChild(xmlNodeFilterSelection);

                        continue;
                    }

                    result.Append(string.Format(
                        "<div class=\"WorkflowFilterSelection BackgroundColor1\" " +
                        "oncontextmenu=\"WorkflowFilterSelection_ContextMenu(this, '{0}');return false;\">",
                        HttpUtility.UrlEncode(xmlNodeFilterSelection.GetXPath())
                    ));

                    result.Append("<div class=\"WorkflowFilterSelectionTitle\"");

                    if (editable)
                    {
                        result.Append(string.Format(
                            " onclick=\"EditWorkflowFilterSelectionTitle(this, '{0}');\"",
                            HttpUtility.UrlEncode(xmlNodeSelection.GetXPath())
                        ));
                    }

                    string filterName = xmlNodeSelection.SelectSingleNode("VariableFilter").Attributes[0].Value;

                    if (filterName.Trim() == "") { filterName = xmlNodeSelection.Attributes["Name"].Value; }

                    result.Append(string.Format(
                        ">{0}</div>",
                    // xmlNodeSelection.SelectSingleNode("VariableFilter").Attributes[0].Value
                    filterName
                    ));

                    result.Append("<div class=\"WorkflowFilterSelectionCategories\">");

                    // Get all categories of the variable.
                    List<object[]> taxonomyCategories = Global.Core.TaxonomyCategories.ExecuteReader(
                        "SELECT (SELECT TOP 1 Label FROM TaxonomyCategoryLabels WHERE IdTaxonomyCategory=" +
                        "TaxonomyCategories.Id) FROM TaxonomyCategories WHERE IdTaxonomyVariable={0}",
                        new object[] { idVariable }
                    );

                    // Run through all categories of the variable.
                    foreach (object[] taxonomyCategory in taxonomyCategories)
                    {
                        result.Append("<div class=\"WorkflowFilterSelectionCategory\">");
                        result.Append(taxonomyCategory[0]);
                        result.Append("</div>");
                    }

                    result.Append("</div>");
                    result.Append("</div>");
                }

                // Check if the selection has valid filter selections.
                if (xmlNodeSelection.ChildNodes.Count == 0)
                {
                    xmlNode.RemoveChild(xmlNodeSelection);
                }
            }
            if (Guid.Parse(Request.Params["IdHierarchy"]) == idHierarchy && xmlNode.Name != "Default")
                if (editable)
                {
                    // Render the add button open the variable selector.
                    result.Append("<div class=\"ButtonWorkflowFilterSelectionAdd\">" +
                        "<img src=\"/Images/Icons/Add5.png\" onclick=\"InitDragBox('boxVariableSearchControl');" +
                        "SearchVariables();idVariableSearchSelectedItem=undefined;\" /></div>");
                }
                else
                {
                    // Render the add button open the variable selector.
                    result.Append(string.Format(
                        "<div class=\"ButtonWorkflowAdd\"><img src=\"/Images/Icons/Add5.png\"" +
                        " onclick=\"ButtonWorkflowAdd_Click();\" /><br />{0}</div>",
                        string.Format(
                            Global.LanguageManager.GetText("ButtonWorkflowAdd"),
                            Global.Core.Hierarchies.GetValue("Name", "Id", idHierarchy)
                        )
                    ));
                }

            if (xmlNode.Name == "Default" && idHierarchy == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                // Render the add button open the variable selector.
                result.Append("<div class=\"ButtonWorkflowFilterSelectionAdd\">" +
                    "<img src=\"/Images/Icons/Add5.png\" onclick=\"InitDragBox('boxVariableSearchControl');" +
                    "SearchVariables();idVariableSearchSelectedItem=undefined;\" /></div>");
            }
            else if(xmlNode.Name == "Default" && !editable)
            {
                // Render the add button open the variable selector.
                result.Append(string.Format(
                    "<div class=\"ButtonWorkflowAdd\"><img src=\"/Images/Icons/Add5.png\"" +
                    " onclick=\"ButtonWorkflowAdd_Click();\" /><br />{0}</div>",
                    string.Format(
                        Global.LanguageManager.GetText("ButtonWorkflowAdd"),
                        Global.Core.Hierarchies.GetValue("Name", "Id", idHierarchy)
                    )
                ));
            }

            // Write the contents of the result
            // string builder to the http response.
            Response.Write(result.ToString());

            xmlNode.OwnerDocument.Save(this.FileName);

            // Clear the contents of the result string builder.
            result.Clear();
        }

        private XmlNode GetInheritedWorkflowDefinition(XmlDocument document, Guid idHierarchy)
        {
            XmlNode result = document.DocumentElement.SelectSingleNode(string.Format(
                "HierarchyWorkflow[@IdHierarchy=\"{0}\"]",
                idHierarchy
            ));

            if (result != null)
            {
                return result;
            }
            else
            {
                object idParentHierarchy = Global.Core.Hierarchies.GetValue(
                    "IdHierarchy",
                    "Id",
                    idHierarchy
                );

                if (idParentHierarchy == null)
                {
                    return document.DocumentElement.SelectSingleNode("Default");
                }

                return GetInheritedWorkflowDefinition(document, (Guid)idParentHierarchy);
            }
        }


        private void SetWorkflowFilterSelectionTitle()
        {
            // Get the path to the workflow selection filter
            // xml node from the http request's parameters.
            string path = Request.Params["Path"];

            // Get the new name for the workflow selection
            // filter from the http request's parameters.
            string value = Request.Params["Value"];

            // Create a new xml document that contains
            // the client's workflow definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the client's workflow
            // definition file into the xml document.
            document.Load(this.FileName);

            // Select the xml node that defines
            // the workflow selection filter.
            XmlNode xmlNode = document.SelectSingleNode(path);

            // Check if the workflow selection filter exists.
            if (xmlNode == null)
                return;

            // Set the new name of the workflow selection filter.
            xmlNode.Attributes["Name"].Value = value;

            // Save the changes of the workflow definition
            // to the workflow definition xml file.
            document.Save(this.FileName);
        }


        private void SearchVariables()
        {
            // Get the search expression from
            // the http request's parameters.
            string expression = Request.Params["Expression"];

            List<object[]> taxonomyVariableLabels = Global.Core.TaxonomyVariableLabels.ExecuteReader(
                "SELECT DISTINCT IdTaxonomyVariable, Label FROM TaxonomyVariableLabels WHERE (Label like {0} OR Label like {1} OR Label like {2} OR Label like {3}) ORDER BY Label",
                new object[] { expression, expression + "%", "%" + expression, "%" + expression + "%" }
            );

            // Create a new string builder to render the result html string.
            StringBuilder result = new StringBuilder();

            // Run through all variable labels found.
            foreach (object[] taxonomyVariableLabel in taxonomyVariableLabels)
            {
                Guid idVariable = (Guid)taxonomyVariableLabel[0];

                switch ((VariableType)((int)Global.Core.TaxonomyVariables.GetValue("Type", "Id", idVariable)))
                {
                    case VariableType.Numeric:
                    case VariableType.Text:
                        continue;
                        break;
                }

                result.Append(string.Format(
                    "<table IdVariable=\"{0}\" class=\"VariableSearchResultItem VariableSelector BackgroundColor1\"" +
                    " cellspacing=\"0\" cellpadding=\"0\" onclick=\"SelectVariableSelectorItem(this);\"><tr>",
                    idVariable
                ));

                result.Append(string.Format(
                    "<td style=\"width:60px;min-width:60px;max-width:60px;padding:0px;\">" +
                    "<img style=\"width:60px\" src=\"/Images/Icons/VariableSelector/{0}.png\" /></td>",
                    (VariableType)((int)Global.Core.TaxonomyVariables.GetValue("Type", "Id", idVariable))
                ));

                result.Append(string.Format(
                    "<td class=\"VariableSelectorVariableLabel\" style=\"width:100%;\">{0}</td>",
                    taxonomyVariableLabel[1]
                ));

                result.Append("</tr></table>");
            }

            Response.Write(result.ToString());
        }

        private void AddWorkflowFilterSelection()
        {
            // Create a new xml document that contains
            // the client's workflow definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the client's workflow
            // definition file into the xml document.
            document.Load(this.FileName);

            // Parse the id of the variable to add as a workflow
            // filter from the http request's parameters.
            Guid idVariable = Guid.Parse(
                Request.Params["IdVariable"]
            );

            // Get the path to the workflow selector
            // from the http request's parameters.
            string path = Request.Params["Path"];

            // Select the xml node that defines the workflow selector.
            XmlNode xmlNode = document.SelectSingleNode(path);

            // Check if the workflow selector exists.
            if (xmlNode == null)
                return;

            xmlNode.InnerXml += string.Format(
                "<Selection Name=\"{0}\" Id=\"{1}\">" +
                "<VariableFilter Name=\"\" VariableName=\"{0}\" IsTaxonomy=\"True\" Mode=\"Multi\"></VariableFilter>" +
                "</Selection>",
                (string)Global.Core.TaxonomyVariables.GetValue("Name", "Id", idVariable),
                Guid.NewGuid()
            );

            // Save the changes made to the workflow definition file.
            document.Save(this.FileName);
        }

        private void DeleteWorkflowFilterSelection()
        {
            // Create a new xml document that contains
            // the client's workflow definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the client's workflow
            // definition file into the xml document.
            document.Load(this.FileName);

            // Get the path to the workflow selection filter
            // from the http request's parameters.
            string path = Request.Params["Path"];

            // Select the xml node that defines
            // the workflow selection filter.
            XmlNode xmlNode = document.SelectSingleNode(path);

            // Check if the workflow selection filter exists.
            if (xmlNode == null)
                return;

            xmlNode.ParentNode.RemoveChild(xmlNode);

            document.Save(this.FileName);
        }

        private void DeleteWorkflow()
        {
            // Create a new xml document that contains
            // the client's workflow definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the client's workflow
            // definition file into the xml document.
            document.Load(this.FileName);

            // Get the path to the workflow selection filter
            // from the http request's parameters.
            string path = Request.Params["Path"];

            // Select the xml node that defines
            // the workflow selection filter.
            XmlNode xmlNode = document.SelectSingleNode(path);

            // Check if the workflow selection filter exists.
            if (xmlNode == null)
                return;

            xmlNode.ParentNode.RemoveChild(xmlNode);

            document.Save(this.FileName);
        }

        private void AddWorkflow()
        {
            // Create a new xml document that contains
            // the client's workflow definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the client's workflow
            // definition file into the xml document.
            document.Load(this.FileName);

            // Parse the id of the hierarchy from
            // the http request's parameters.
            Guid idHierarchy = Guid.Parse(
                Request.Params["IdHierarchy"]
            );

            document.DocumentElement.InnerXml += string.Format(
                "<HierarchyWorkflow IdHierarchy=\"{0}\"></HierarchyWorkflow>",
                idHierarchy
            );

            document.Save(this.FileName);
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["Method"] != null)
            {
                switch (Request.Params["Method"])
                {
                    case "RenderWorkflowDefinition":
                        Response.Clear();
                        RenderWorkflowDefinition();
                        Response.End();
                        break;
                    case "SetWorkflowFilterSelectionTitle":
                        Response.Clear();
                        SetWorkflowFilterSelectionTitle();
                        Response.End();
                        break;
                    case "SearchVariables":
                        Response.Clear();
                        SearchVariables();
                        Response.End();
                        break;
                    case "AddWorkflowFilterSelection":
                        Response.Clear();
                        AddWorkflowFilterSelection();
                        Response.End();
                        break;
                    case "DeleteWorkflowFilterSelection":
                        Response.Clear();
                        DeleteWorkflowFilterSelection();
                        Response.End();
                        break;
                    case "DeleteWorkflow":
                        Response.Clear();
                        DeleteWorkflow();
                        Response.End();
                        break;
                    case "AddWorkflow":
                        Response.Clear();
                        AddWorkflow();
                        Response.End();
                        break;
                }

                return;
            }

            boxVariableSearch.Visible = true;

            BindHierarchies();
        }

        #endregion
    }
}