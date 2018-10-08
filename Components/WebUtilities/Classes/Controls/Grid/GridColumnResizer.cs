using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtilities.Controls;

namespace WebUtilities.Classes.Controls.GridClasses
{
    public class GridColumnResizer : BaseControl
    {
        #region Properties

        public string ColumnName { get; set; }

        public Grid Owner { get; set; }

        #endregion


        #region Constructor

        public GridColumnResizer(string tag, Grid owner)
            : base(tag)
        {
            this.Load += GridColumnResizer_Load;
            this.Owner = owner;
        }

        #endregion


        #region Methods

        public void Build()
        {
            this.CssClass = "GridColumnResizer";
            this.Attributes.Add("ColumnName", this.ColumnName);
        }

        #endregion


        #region Event Handlers

        protected void GridColumnResizer_Load(object sender, EventArgs e)
        {
            Build();

            // Resize is disabled.
            //this.Attributes.Add("onmousedown", "ActivateResize('"+ this.Owner.ClientID +"','"+ this.ColumnName +"');");
        }

        #endregion
    }
}
