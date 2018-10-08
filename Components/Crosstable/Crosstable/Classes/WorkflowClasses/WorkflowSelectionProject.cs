using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebUtilities.Controls;

namespace Crosstables.Classes.WorkflowClasses
{
    /// <summary>
    /// This will turn into the base class.
    /// </summary>
    public class WorkflowSelectionProject : WorkflowSelectionSelector
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the study of
        /// the workflow selection project.
        /// </summary>
        public Guid IdStudy
        {
            get
            {
                return Guid.Parse(base.XmlNode.Attributes["IdVariable"].Value);
            }
            set
            {
                base.XmlNode.Attributes["IdVariable"].Value = value.ToString();
            }
        }

        #endregion


        #region Constructor

        public WorkflowSelectionProject(WorkflowSelection owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;

            this.Load += WorkflowSelectionProject_Load;

            Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            // Create a new workflow selector for the variable.
            this.Selector = new WorkflowSelector(this);

            // Run through all studies of the client.
            foreach (Study study in this.Owner.Owner.Core.Studies.Get())
            {
                // Create a new workflow selector item for the category.
                WorkflowSelectorItem selectorItem = new WorkflowSelectorItem(this.Selector);
                selectorItem.Id = study.Id;
                selectorItem.Text = study.Name;

                // Add the workflow selector item to the workflow selector's items.
                this.Selector.Items.Add(selectorItem);
            }

            // Run through all selected category xml nodes.
            foreach (XmlNode xmlNodeSelected in this.XmlNode.ChildNodes)
            {
                Guid idStudy = Guid.Parse(
                    xmlNodeSelected.Attributes["Id"].Value
                );

                this.Selector.SelectedItems.Add(idStudy);
            }
        }

        #endregion


        #region Event Handlers

        protected void WorkflowSelectionProject_Load(object sender, EventArgs e)
        {
            this.CssClass = "WorkflowSelectionProject";

            base.Controls.Add(this.Selector);
        }

        #endregion
    }
}
