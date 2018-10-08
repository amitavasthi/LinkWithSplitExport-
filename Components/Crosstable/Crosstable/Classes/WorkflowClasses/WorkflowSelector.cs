using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstables.Classes.WorkflowClasses
{
    public class WorkflowSelector : WebUtilities.BaseControl
    {
        #region Properties

        public WorkflowSelectionSelector Owner { get; set; }

        /// <summary>
        /// Gets or sets a collection of items.
        /// </summary>
        public List<WorkflowSelectorItem> Items { get; set; }

        /// <summary>
        /// Gets or sets a collection of the selected items.
        /// </summary>
        public List<Guid> SelectedItems { get; set; }

        #endregion


        #region Constructor

        public WorkflowSelector(WorkflowSelectionSelector owner)
        {
            this.Owner = owner;
            this.Items = new List<WorkflowSelectorItem>();
            this.SelectedItems = new List<Guid>();

            this.Load += WorkflowSelector_Load;
        }

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        protected void WorkflowSelector_Load(object sender, EventArgs e)
        {
            this.CssClass = "WorkflowSelector";

            this.Style.Add("height", (300 / this.Owner.Owner.SelectionVariables.Count) + "px");

            // Run through all items of the selector.
            foreach (WorkflowSelectorItem item in this.Items)
            {
                base.Controls.Add(item);
            }
        }

        #endregion
    }
}
