using Homescreen1.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUtilities.Classes.Controls.GridClasses;
using WebUtilities.Controls;

namespace LinkOnline.Pages.ClientManager
{
    public partial class DashboardItems : WebUtilities.BasePage
    {
        #region Properties

        Grid grid;

        #endregion


        #region Constructor

        public DashboardItems()
        {

        }

        #endregion


        #region Methods

        private void LoadGrid()
        {
            // Create a new grid that displays all
            // dashboard items of the client.
            grid = new Grid();
            grid.ID = "gridDashboardItems";
            grid.MaxHeight = base.ContentHeight - 200;

            GridHeadline headline = new GridHeadline(grid);

            headline.Items.Add(new GridHeadlineItem(headline, 0, Global.LanguageManager.GetText("DashboardItemTitle"), new GridHeadlineItemWidth(15)));
            headline.Items.Add(new GridHeadlineItem(headline, 1, Global.LanguageManager.GetText("DashboardItemSource"), new GridHeadlineItemWidth(50)));
            headline.Items.Add(new GridHeadlineItem(headline, 2, Global.LanguageManager.GetText("DashboardItemCreator"), new GridHeadlineItemWidth(15)));
            headline.Items.Add(new GridHeadlineItem(headline, 3, Global.LanguageManager.GetText("DashboardItemCreationDate"), new GridHeadlineItemWidth(20)));

            grid.GridHeadline = headline;

            // Build the full path to the directory where
            // the client's dashboard items are stored.
            string directoryName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "DashboardItems",
                Global.Core.ClientName
            );

            // Check if the client has any dashboard items defined.
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            // Run through all dashboard items of the client.
            foreach (string fileName in Directory.GetFiles(directoryName))
            {
                // Create a new dashboard item by
                // the dashboard item definition file.
                DashboardItem info = new DashboardItem(fileName);

                // Create a new grid row for the dashboard item.
                GridRow row = new GridRow(grid, info.Id);

                row.Items.Add(new GridRowItem(row, info.Name, true));
                row.Items.Add(new GridRowItem(row, HttpUtility.UrlDecode(info.Source), true));
                row.Items.Add(new GridRowItem(row, Global.GetNiceUsername(info.IdUser), true));
                row.Items.Add(new GridRowItem(row, info.CreationDate.ToFormattedString(), true));

                grid.Rows.Add(row);
            }

            pnlGrid.Controls.Add(grid);
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadGrid();

            boxDashboardItem.Visible = false;
            cbDelete.Visible = false;
        }


        protected void btnAdd_Click(object sender, EventArgs e)
        {
            txtDashboardItemTitle.Text = "";
            txtDashboardItemSource.Text = "";

            btnDashboardItemAdd.Visible = true;
            btnDashboardItemSave.Visible = false;

            txtDashboardItemTitle.Button = "btnDashboardItemAdd";
            txtDashboardItemSource.Button = "btnDashboardItemAdd";

            boxDashboardItem.Title = "AddDashboardItem";
            boxDashboardItem.Visible = true;
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (grid.SelectedItem == null)
                return;

            // Build the full path to the selected
            // dashboard item's definition file.
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "DashboardItems",
                Global.Core.ClientName,
                (Guid)grid.SelectedItem + ".xml"
            );

            if (!File.Exists(fileName))
                return;

            DashboardItem info = new DashboardItem(fileName);

            txtDashboardItemTitle.Text = info.Name;
            txtDashboardItemSource.Text = HttpUtility.UrlDecode(info.Source);

            txtDashboardItemTitle.Button = "btnDashboardItemSave";
            txtDashboardItemSource.Button = "btnDashboardItemSave";

            btnDashboardItemAdd.Visible = false;
            btnDashboardItemSave.Visible = true;

            boxDashboardItem.Title = "EditDashboardItem";
            boxDashboardItem.Visible = true;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (grid.SelectedItem == null)
                return;

            // Build the full path to the selected
            // dashboard item's definition file.
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "DashboardItems",
                Global.Core.ClientName,
                (Guid)grid.SelectedItem + ".xml"
            );

            if (!File.Exists(fileName))
                return;

            cbDelete.ID = "cbDelete";

            DashboardItem info = new DashboardItem(fileName);

            cbDelete.Title = Global.LanguageManager.GetText("DeleteDashboardItemTitle");

            cbDelete.Text = string.Format(
                Global.LanguageManager.GetText("DeleteDashboardItem"),
                info.Name
            );

            cbDelete.Confirm = delegate()
            {
                File.Delete(fileName);
            };

            cbDelete.Visible = true;
        }


        protected void btnDashboardItemAdd_Click(object sender, EventArgs e)
        {
            // Build the full path to the new
            // dashboard item's definition file.
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "DashboardItems",
                Global.Core.ClientName,
                Guid.NewGuid() + ".xml"
            );

            File.WriteAllText(fileName, string.Format(
                "<DashboardItem Name=\"{0}\" Source=\"{1}\" IdUser=\"{2}\" CreationDate=\"{3}\"><LatestUses></LatestUses></DashboardItem>",
                txtDashboardItemTitle.Text,
                HttpUtility.UrlEncode(txtDashboardItemSource.Text),
                Global.IdUser.Value.ToString(),
                DateTime.Now.ToString()
            ));

            Response.Redirect(Request.Url.ToString());
        }

        protected void btnDashboardItemSave_Click(object sender, EventArgs e)
        {
            // Build the full path to the selected
            // dashboard item's definition file.
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "DashboardItems",
                Global.Core.ClientName,
                (Guid)grid.SelectedItem + ".xml"
            );

            DashboardItem info = new DashboardItem(fileName);

            info.Name = txtDashboardItemTitle.Text;
            info.Source = HttpUtility.UrlEncode(txtDashboardItemSource.Text);

            info.Save();

            Response.Redirect(Request.Url.ToString());
        }

        #endregion
    }
}