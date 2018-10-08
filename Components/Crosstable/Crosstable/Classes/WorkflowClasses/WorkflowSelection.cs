using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Xml;
using WebUtilities.Controls;

namespace Crosstables.Classes.WorkflowClasses
{
    public class WorkflowSelection : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning workflow.
        /// </summary>
        public Workflow Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node which contains the selection definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the name of the workflow selection.
        /// </summary>
        public string Name
        {
            get
            {
                return this.XmlNode.Attributes["Name"].Value;
            }
            set
            {
                this.XmlNode.Attributes["Name"].Value = value;
            }
        }

        public Dictionary<string, WorkflowSelectionSelector> SelectionVariables { get; set; }

        public Panel PnlSelector { get; set; }

        #endregion


        #region Constructor

        public WorkflowSelection(Workflow owner, XmlNode xmlNode)
        {
            this.SelectionVariables = new Dictionary<string, WorkflowSelectionSelector>();
            this.Owner = owner;
            this.XmlNode = xmlNode;

            this.Load += WorkflowSelection_Load;

            Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            // Run through all child nodes of the selection.
            foreach (XmlNode xmlNode in this.XmlNode.ChildNodes)
            {
                WorkflowSelectionSelector selector = null;

                switch (xmlNode.Name)
                {
                    case "HierarchyFilter":
                        selector = new WorkflowSelectionHierarchy(this, xmlNode); ;
                        break;

                    case "VariableFilter":
                        selector = new WorkflowSelectionVariable(this, xmlNode); ;
                        break;

                    case "ProjectFilter":
                        selector = new WorkflowSelectionProject(this, xmlNode);
                        break;
                }

                if (selector != null)
                    this.SelectionVariables.Add(selector.Name, selector);
            }
        }

        #endregion


        #region Event Handlers

        protected void WorkflowSelection_Load(object sender, EventArgs e)
        {
            this.CssClass = "WorkflowSelection BorderColor1";

            this.PnlSelector = new Panel();
            this.PnlSelector.ID = "pnlSelector" + this.Name;
            this.PnlSelector.CssClass = "WorkflowSelectionSelector";

            HtmlGenericControl lblSelector = new HtmlGenericControl("div");
            lblSelector.Attributes.Add("class", "WorkflowSelectionSelectorLabel");
            lblSelector.Attributes.Add("onclick", "SelectAllWorkflowItems(this.parentNode);");


            string filterName = "";
            int i = 0;
            foreach (var item in this.SelectionVariables.Keys)
            {
                if (i == 0)
                {
                    filterName = item;
                }
                i++;
            }

            if (filterName.Trim() == "") { filterName = base.LanguageManager.GetText(this.Name); }

            lblSelector.InnerHtml = filterName;

            Panel pnlSelection = new Panel();
            pnlSelection.ID = "pnlSelection" + this.Name;

            // Run through all child xml nodes of the selection definition xml node.
            foreach (WorkflowSelectionSelector variableSelection in this.SelectionVariables.Values)
            {
                pnlSelection.Controls.Add(variableSelection);
            }

            if (this.Owner.Editable)
            {
                this.PnlSelector.Attributes.Add("DropArea", "True");
                this.PnlSelector.Attributes.Add("DropAreaMessage", "DropAreaMessageReplace");
                this.PnlSelector.Attributes.Add("Source", this.Owner.FileName.Replace("\\", "/"));
                this.PnlSelector.Attributes.Add("Path", this.XmlNode.GetXPath(true));

                this.PnlSelector.Attributes.Add("oncontextmenu", string.Format(
                    "LoadContextMenuWorkflowSelector(this, '{0}', '{1}');return false;",
                    this.Owner.FileName.Replace("\\", "/"),
                    this.XmlNode.GetXPath(true)
                ));
            }

            this.PnlSelector.Controls.Add(lblSelector);
            this.PnlSelector.Controls.Add(pnlSelection);

            base.Controls.Add(this.PnlSelector);
        }

        #endregion
    }
}
