using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using WebUtilities.Controls;

namespace Crosstables.Classes.WorkflowClasses
{
    /// <summary>
    /// This will turn into the base class.
    /// </summary>
    public class WorkflowSelectionHierarchy : WorkflowSelectionSelector
    {
        #region Properties

        #endregion


        #region Constructor

        public WorkflowSelectionHierarchy(WorkflowSelection owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;

            this.Load += WorkflowSelectionHierarchy_Load;

            Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            // Create a new workflow selector for the variable.
            this.Selector = new WorkflowSelector(this);

            if (HttpContext.Current == null)
                return;

            // Get all the hierarchies on root level.
            List<object[]> hierarchies = this.Owner.Core.Hierarchies.GetValues(
                new string[] { "Id", "Name" },
                new string[] { "IdHierarchy" },
                new object[] { null }
            );

            // Run through all available hierarchies.
            foreach (object[] hierarchy in hierarchies)
            {
                //RenderHierarchyItem(hierarchy, 0);
                // Get all hierarchies of the workgroups where the user
                // is assigned to where the hierarchy is the parent.
                List<object[]> childHierarchies = base.Core.Hierarchies.ExecuteReader(string.Format(
                    "SELECT Id, Name FROM [Hierarchies] WHERE IdHierarchy='{1}' AND Id IN (SELECT IdHierarchy FROM WorkgroupHierarchies " +
                    "WHERE IdWorkgroup IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser='{0}'))",
                    (Guid)HttpContext.Current.Session["User"],
                    hierarchy[0]
                ));

                // Run through all child hierarchy items of the hierarchy.
                foreach (object[] childHierarchy in childHierarchies)
                {
                    RenderHierarchyItem(childHierarchy, 0);
                }
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

        private void RenderHierarchyItem(object[] hierarchy, int level)
        {
            WorkflowSelectorItem item = new WorkflowSelectorItem(this.Selector);
            item.Id = (Guid)hierarchy[0];
            item.Text = (string)hierarchy[1];
            item.Style.Add("margin-left", (level * 10) + "px");

            this.Selector.Items.Add(item);

            // Get all hierarchies of the workgroups where the user
            // is assigned to where the hierarchy is the parent.
            List<object[]> childHierarchies = base.Core.Hierarchies.ExecuteReader(string.Format(
                "SELECT Id, Name FROM [Hierarchies] WHERE IdHierarchy='{1}' AND Id IN (SELECT IdHierarchy FROM WorkgroupHierarchies " +
                "WHERE IdWorkgroup IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser='{0}'))",
                (Guid)HttpContext.Current.Session["User"],
                hierarchy[0]
            ));

            // Run through all child hierarchy items of the hierarchy.
            foreach (object[] childHierarchy in childHierarchies)
            {
                RenderHierarchyItem(childHierarchy, level + 1);
            }
        }

        #endregion


        #region Event Handlers

        protected void WorkflowSelectionHierarchy_Load(object sender, EventArgs e)
        {
            this.CssClass = "WorkflowSelectionHierarchy";

            base.Controls.Add(this.Selector);
        }

        #endregion
    }
}
