using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using WebUtilities.Controls;

namespace WebUtilities.Classes.Controls.GridClasses
{
    public class GridHeadline : BaseControl
    {
        #region Properties

        public List<GridHeadlineItem> Items { get; set; }

        public Grid Owner { get; set; }

        #endregion


        #region Constructor

        public GridHeadline(Grid owner)
            : base("div")
        {
            this.Owner = owner;
            this.Items = new List<GridHeadlineItem>();

            this.Load += GridHeadline_Load;
        }

        public GridHeadline(Grid owner, params string[] items)
            : this(owner)
        {
            int i = 0;
            foreach (string item in items)
            {
                this.Items.Add(new GridHeadlineItem(this, i++,
                    item,
                    new GridHeadlineItemWidth(100 / items.Length))
                );
            }
        }

        public GridHeadline(Grid owner, string[] items, bool languageLabel)
            : this(owner)
        {
            int i = 0;
            foreach (string item in items)
            {
                this.Items.Add(new GridHeadlineItem(this, i++,
                    item,
                    new GridHeadlineItemWidth(100 / items.Length),
                    languageLabel)
                );
            }
        }

        #endregion


        #region Methods

        public void Build()
        {
            this.CssClass = "GridHeadline BackgroundColor1";

            int i = 0;
            foreach (GridHeadlineItem item in this.Items)
            {
                this.Controls.Add(item);

                if (i != this.Items.Count - 1)
                {
                    GridColumnResizer resizer = new GridColumnResizer("div", this.Owner);
                    resizer.ColumnName = item.ColumnName;

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

            this.Controls.Add(new LiteralControl("<div class=\"GridClear\"></div>"));
        }

        #endregion


        #region Event Handlers

        protected void GridHeadline_Load(object sender, EventArgs e)
        {
            Build();
        }

        #endregion
    }
}
