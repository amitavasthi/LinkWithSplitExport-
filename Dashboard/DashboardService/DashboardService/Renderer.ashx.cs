using DashboardCore;
using DashboardCore.Exporter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace DashboardService
{
    /// <summary>
    /// Summary description for Renderer
    /// </summary>
    public class Renderer : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string fileName = Path.Combine(
                ConfigurationManager.AppSettings["DashboardRoot"],
                context.Request.Params["Dashboard"],
                "Definition.xml"
            );

            // Create a new database core for the session.
            DatabaseCore.Core core = new DatabaseCore.Core(
                ConfigurationManager.AppSettings["DatabaseProvider"],
                string.Format(ConfigurationManager.AppSettings["ConnectionString"], "iheartmedia"),
                ConfigurationManager.AppSettings["DatabaseProviderUserManagement"],
                string.Format(ConfigurationManager.AppSettings["ConnectionString"], "iheartmedia"),
                new string[0]
            );

            Dashboard dashboard = new Dashboard(fileName, core);

            dashboard.Parse();

            if (context.Request.Params["Export"] == "True")
            {
                /*context.Response.ContentType = "application/vnd.ms-excel;charset=utf-8";
                context.Response.AddHeader("Content-Disposition", "attachment;filename = ExcelFile.xls");
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.Write(dashboard.Render());
                context.Response.End();
                return;*/

                //DashboardExporter exporter = new DashboardExporterExcel(dashboard);
                DashboardExporter exporter = new DashboardExporterPdf(dashboard);

                string test = exporter.Export(HttpUtility.UrlDecode(context.Request.Params["Html"]), Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "Temp",
                    Guid.NewGuid() + exporter.Extension
                ));

                WriteFileToResponse(
                    test,
                    dashboard.Document.DocumentElement.Attributes["Name"].Value + exporter.Extension,
                    exporter.MimeType,
                    true
                );
                return;
            }

            StringBuilder result = new StringBuilder();
            

            context.Response.Write(dashboard.Render(false));
            context.Response.ContentType = "text/html";

            result.Clear();
        }

        /// <summary>
        /// Writes a file to the response of a request.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file that is to be written.
        /// </param>
        /// <param name="displayName">
        /// The name that is to be displayed for the file.
        /// </param>
        public void WriteFileToResponse(string fileName, string displayName, string mimeType, bool deleteFile)
        {
            // Local variables.
            HttpContext context = null;


            // Replace the spaces in the 
            // file name with underscores.
            displayName = displayName.Replace(" ", "_");


            // Read the context.
            context = HttpContext.Current;

            // Configure the response and transfer the file.
            context.Response.Buffer = true;
            context.Response.Clear();
            context.Response.AppendHeader("content-disposition",
                string.Format(CultureInfo.InvariantCulture,
                "attachment; filename={0}", displayName));
            context.Response.ContentType = mimeType;

            byte[] buffer = File.ReadAllBytes(fileName);

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);

            if (deleteFile)
                File.Delete(fileName);

            context.Response.RedirectLocation = context.Request.Url.ToString();

            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}