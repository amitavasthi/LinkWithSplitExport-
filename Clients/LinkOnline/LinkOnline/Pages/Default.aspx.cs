using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ApplicationUtilities.Classes;
using HtmlAgilityPack;
using LinkOnline.Classes;
using DatabaseCore.Items;
using System.Xml;
using System.IO;
using LinkBi1.Controls;
using System.Text;
using System.Web.Hosting;
using System.Globalization;
using Crosstables.Classes.ReportDefinitionClasses;
using LinkBi1.Classes;
using Homescreen1;

namespace LinkOnline.Pages
{
    public partial class Default : WebUtilities.BasePage
    {
        #region Properties

        #endregion


        #region Constructor

        public Default()
        {
            this.Load += Default_Load;
        }

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        protected void Default_Load(object sender, EventArgs e)
        {
            /*Response.Clear();
            foreach (string key in HttpContext.Current.Session.Keys)
            {
                try
                {
                    byte[] bytes = HttpContext.Current.Session[key].ToByteArray();
                    Response.Write(string.Format(
                        "{0}: Before: {1}, After: {2} <br />",
                        key,
                        HttpContext.Current.Session[key].ToString(),
                        HttpContext.Current.Session[key].GetType().FromByteArray(bytes)
                    ));
                }
                catch (Exception ex)
                {
                    //Response.Write(ex.Message + "<br />\n");
                    Response.Write(string.Format(
                        "FAILED: {0} TYPE: {1}<br />",
                        key,
                        HttpContext.Current.Session[key].GetType().Name
                    ));
                }
            }
            Response.End();*/
            if (Request.Params["Method"] != null)
            {
                Response.Clear();

                switch (Request.Params["Method"])
                {
                    case "GotoLibrary":
                        GotoLibrary();
                        break;
                }
                Response.End();
                return;
            }
            // Build the path to the user's home screen definition file.
            string fileName = Path.Combine(
    Request.PhysicalApplicationPath,
    "Fileadmin",
    "HomeDefinitions",
    Global.Core.ClientName,
    Global.IdUser.Value + ".xml"
);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            // Check if the user has a homescreen defined.
            if (!File.Exists(fileName))
            {
                // Build the path to the client's home screen definition file.
                string fileNameClient = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "HomeDefinitions",
                    Global.Core.ClientName + ".xml"
                );

                // Check if the client has a homescreen defined.
                if (!File.Exists(fileNameClient))
                {
                    // Build the full path to the default homescreen definition.
                    string fileNameDefault = Path.Combine(
                        Request.PhysicalApplicationPath,
                        "App_Data",
                        "Homescreen.xml"
                    );

                    File.Copy(
                        fileNameDefault,
                        fileNameClient
                    );
                }

                File.Copy(
                    fileNameClient,
                    fileName
                );
            }

            Homescreen homescreen = new Homescreen(fileName);

            //homescreen.ContentWidth = base.ContentWidth - 40;
            //homescreen.ContentHeight = base.ContentHeight - 103;
            homescreen.ContentWidth = "ContentWidth - 50";
            homescreen.ContentHeight = "ContentHeight - 55";

            homescreen.BaseContentWidth = base.ContentWidth - 50;
            homescreen.BaseContentHeight = base.ContentHeight - 55;

            homescreen.Parse();

            pnlHomeContainer.Controls.Add(homescreen);
        }

        private void GotoLibrary()
        {
            if (Request.Params["Value"] != null)
            {
                string path = Path.Combine(Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "SavedReports",
                    Global.Core.ClientName,
                    Request.Params["Value"].ToString().ToLower());
                if (Directory.Exists(path))
                {
                    HttpContext.Current.Session["LinkCloudSelectedDirectory"] = path;
                }
            }
        }

        #endregion
    }
}