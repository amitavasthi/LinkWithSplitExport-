using LinkBi1.Classes;
using LinkBi1.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtilities.Controls;

namespace LinkBi1.Controls
{
    public class LinkBiServerConnectionControl : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the server connection to display values for.
        /// </summary>
        public LinkBiServerConnection ServerConnection { get; set; }

        #endregion


        #region Constructor

        public LinkBiServerConnectionControl(LinkBiServerConnection serverConnection)
        {
            this.ServerConnection = serverConnection;

            this.Load += LinkBiServerConnectionControl_Load;
        }

        #endregion


        #region Methods

        public void Render()
        {
            base.CssClass = "LinkBiServerConnection";
            this.Attributes.Add("id", "LinkBiServerConnection" + this.ServerConnection.Identity);

            Table table = new Table();

            TableRow tableRowValid = new TableRow();
            TableCell tableCellValid = new TableCell();
            tableCellValid.Attributes.Add("id", "LinkBiServerConnectionValid" + this.ServerConnection.Identity);
            tableCellValid.ColumnSpan = 2;

            tableRowValid.Cells.Add(tableCellValid);
            table.Rows.Add(tableRowValid);

            TableRow tableRowInterfaceType = new TableRow();

            TableCell tableCellInterfaceTypeTitle = new TableCell();
            TableCell tableCellInterfaceTypeValue = new TableCell();

            tableCellInterfaceTypeTitle.Text = base.LanguageManager.GetText("LinkBiInterfaceType");

            DropDownList ddlInterfaceType = new DropDownList();
            ddlInterfaceType.BindEnum(typeof(LinkBiInterfaceType));
            ddlInterfaceType.SelectedValue = this.ServerConnection.InterfaceType.ToString();

            ddlInterfaceType.Attributes.Add("onchange", string.Format(
                "UpdateServerConnectionProperty('{0}' ,'{1}', '{2}', this.value);",
                this.ServerConnection.Owner.FileName.Replace("\\", "/"),
                this.ServerConnection.XmlNode.GetXPath(),
                "InterfaceType"
            ));

            tableCellInterfaceTypeValue.Controls.Add(ddlInterfaceType);

            tableRowInterfaceType.Cells.Add(tableCellInterfaceTypeTitle);
            tableRowInterfaceType.Cells.Add(tableCellInterfaceTypeValue);

            table.Rows.Add(tableRowInterfaceType);

            TableRow tableRowType = new TableRow();

            TableCell tableCellTypeTitle = new TableCell();
            TableCell tableCellTypeValue = new TableCell();

            tableCellTypeTitle.Text = base.LanguageManager.GetText("LinkBiServerConnectionType");

            DropDownList ddlType = new DropDownList();
            ddlType.BindEnum(typeof(LinkBiServerConnectionType));
            ddlType.SelectedValue = this.ServerConnection.Type.ToString();

            ddlType.Attributes.Add("onchange", string.Format(
                "ChangeServerConnectionType(this, '{0}', '{1}')",
                this.ServerConnection.Owner.FileName.Replace("\\", "/"),
                this.ServerConnection.XmlNode.GetXPath()
            ));

            tableCellTypeValue.Controls.Add(ddlType);

            tableRowType.Cells.Add(tableCellTypeTitle);
            tableRowType.Cells.Add(tableCellTypeValue);

            table.Rows.Add(tableRowType);

            TableRow[] tableRowsConnectionProperties = this.ServerConnection.Render();

            foreach (TableRow tableRowConnectionProperties in tableRowsConnectionProperties)
            {
                table.Rows.Add(tableRowConnectionProperties);
            }

            base.Controls.Add(table);


            base.Attributes.Add("oncontextmenu", string.Format(
                "DeleteServerConnection(this, '{0}', '{1}');return false;",
                this.ServerConnection.Owner.FileName.Replace("\\", "/"),
                this.ServerConnection.XmlNode.GetXPath()
            ));

            /*base.Attributes.Add(
                "onmouseout",
                ""
            );*/
        }

        #endregion


        #region Event Handlers

        protected void LinkBiServerConnectionControl_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }
}
