using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using WebUtilities.Controls;

namespace WebUtilities.Classes.Controls.GridClasses
{
    public class GridRow : BaseControl
    {
        #region Properties

        public WebUtilities.Controls.Grid Owner { get; set; }

        public List<GridRowItem> Items { get; set; }

        public object Identity { get; set; }

        #endregion


        #region Constructor

        public GridRow(WebUtilities.Controls.Grid owner, object identity)
            : base("div")
        {
            this.Owner = owner;
            this.Identity = identity;

            this.CssClass = "GridRow";

            this.Items = new List<GridRowItem>();

            this.Load += GridRow_Load;
        }

        public GridRow(WebUtilities.Controls.Grid owner, object identity, params string[] items)
            : this(owner, identity)
        {
            foreach (string item in items)
            {
                this.Items.Add(new GridRowItem(this, item));
            }
        }

        #endregion


        #region Methods

        public void Build()
        {
            int i = 0;
            foreach (GridRowItem item in this.Items)
            {
                item.Index = i;

                this.Controls.Add(item);

                string columnName = "";


                if (this.Owner.GridHeadline.Items.Count > i)
                {
                    columnName = this.Owner.GridHeadline.Items[i].ColumnName;
                }

                item.ColumnName = columnName;

                if (i != this.Items.Count - 1)
                {
                    GridColumnResizer resizer = new GridColumnResizer("div", this.Owner);
                    resizer.ColumnName = columnName;

                    this.Controls.Add(resizer);

                    if (Page == null)
                        resizer.Build();
                }
                else
                {
                    item.Style.Add("border-right", "none");
                }

                i++;

                if (Page == null)
                    item.Build();
            }

            this.Attributes.Add("onclick", "SelectRow('" +
                this.Owner.ID + "', '" +
                this.Identity + "', " +
                this.Owner.AutoPostBack.ToString().ToLower() + ", " +
                "this, '" +
                this.Owner.ClientID + "'" +
                (this.Owner.DependingGrid != null ? ",'" + this.Owner.DependingGrid.ClientID + "'" : "") +
                ")"
            );

            this.Attributes.Add("ondblclick", "GridDoubleClick('" + this.Owner.ID + "');");

            if (this.Owner.SelectedItem != null &&
                this.Owner.SelectedItem.ToString() == this.Identity.ToString())
            {
                this.CssClass = "GridRow_Active";
            }
            else
            {
                this.CssClass = "GridRow";
            }

            this.Controls.Add(new LiteralControl("<div class=\"GridClear\"></div>"));
        }

        #endregion


        #region Event Handlers

        protected void GridRow_Load(object sender, EventArgs e)
        {
            Build();
        }

        #endregion
    }
}
