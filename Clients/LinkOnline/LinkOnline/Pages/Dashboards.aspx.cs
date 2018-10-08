using ApplicationUtilities;
using DatabaseCore.BaseClasses;
using DatabaseCore.Items;
using Homescreen1.Classes;
using HtmlAgilityPack;
using LinkOnline.Classes;
using PermissionCore.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using WebUtilities.Controls;

namespace LinkOnline.Pages
{
    public partial class Dashboards : WebUtilities.BasePage
    {
        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.Params["IdDashboardItem"] == null)
                Response.Redirect("/Pages/Default.aspx");

            // Build the full path to the dashboard item's definition file.
            string fileName = Path.Combine(
                this.Request.PhysicalApplicationPath,
                "Fileadmin",
                "DashboardItems",
                Global.Core.ClientName,
                this.Request.Params["IdDashboardItem"] + ".xml"
            );

            if (!File.Exists(fileName))
                Response.Redirect("/Pages/Default.aspx");

            DashboardItem dashboardItem = new DashboardItem(fileName);

            if (dashboardItem.LatestUses.ContainsKey(Global.IdUser.Value))
                dashboardItem.LatestUses[Global.IdUser.Value] = DateTime.Now;
            else
                dashboardItem.LatestUses.Add(Global.IdUser.Value, DateTime.Now);

            dashboardItem.Save();

            lblDefinitionName.Text = dashboardItem.Name;

            //frame.Attributes["src"] = HttpUtility.UrlDecode(dashboardItem.Source);
            Page.ClientScript.RegisterStartupScript(
                this.GetType(), 
                "LoadDashboard",
                "loadFunctions.push(function() { document.getElementById('cphContent_frame').src = '" + HttpUtility.UrlDecode(dashboardItem.Source) + "'; });", 
                true
            );
        }

        #endregion
    }
}