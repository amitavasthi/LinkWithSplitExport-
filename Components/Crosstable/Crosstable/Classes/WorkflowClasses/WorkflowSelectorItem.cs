using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstables.Classes.WorkflowClasses
{
    public class WorkflowSelectorItem : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning workflow selector.
        /// </summary>
        public WorkflowSelector Owner { get; set; }

        /// <summary>
        /// Gets or sets the unique id of the selector item.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the displaying text of the selector item.
        /// </summary>
        public string Text { get; set; }

        #endregion


        #region Constructor

        public WorkflowSelectorItem(WorkflowSelector owner)
        {
            this.Owner = owner;
            this.Load += WorkflowSelectorItem_Load;
        }

        public WorkflowSelectorItem(WorkflowSelector owner, Guid id, string text)
            : this(owner)
        {
            this.Id = id;
            this.Text = text;
        }

        #endregion


        #region Event Handlers

        protected void WorkflowSelectorItem_Load(object sender, EventArgs e)
        {
            this.CssClass = "WorkflowSelectorItem";

            if (this.Owner.SelectedItems.Contains(this.Id))
            {
                this.CssClass += " WorkflowSelectorItem_Active BackgroundColor1";

                this.Attributes.Add("State", "Selected");
            }
            else
            {
                this.CssClass += " BackgroundColor5H";

                this.Attributes.Add("State", "none");
            }

            this.Attributes.Add(
                "IdItem",
                this.Id.ToString()
            );

            this.Attributes.Add(
                "onclick",
                string.Format(
                    "SelectWorkflowItem(this, '{4}', '{0}', '{1}','{2}', '{3}',this.getAttribute('IdItem'));",
                    this.Owner.Owner.Owner.PnlSelector.ClientID,
                    this.Owner.Owner.Owner.Name,
                    this.Owner.Owner.Name,
                    this.Owner.Owner.Owner.Owner.Service,
                    this.Owner.Owner.Owner.Owner.Source.Replace("\\", "/")
                )
            );

            System.Web.UI.WebControls.Label lblText = new System.Web.UI.WebControls.Label();
            lblText.Text = this.Text;

            base.Controls.Add(lblText);
        }

        #endregion
    }
}
