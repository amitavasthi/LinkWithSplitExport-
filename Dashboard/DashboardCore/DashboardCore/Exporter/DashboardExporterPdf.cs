using ApplicationUtilities.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace DashboardCore.Exporter
{
    public class DashboardExporterPdf : DashboardExporter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the color scheme split by css class names.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> ColorScheme { get; set; }

        #endregion


        #region Constructor

        public DashboardExporterPdf(Dashboard dashboard)
            : base(dashboard)
        {
            base.Extension = ".pdf";
            base.MimeType = "application/pdf";
        }

        #endregion


        #region Methods

        public override string Export(string html, string fileName)
        {
            string temp = Path.GetTempFileName() + ".html";
            File.WriteAllText(temp, html);

            Run(temp, fileName);

            File.Delete(temp);

            return fileName;
        }

        public void Run(string input, string output)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = string.Format(
                "\"C:\\Program Files\\wkhtmltopdf\\bin\\wkhtmltopdf.exe\" -O Landscape \"{0}\" \"{1}\"",
                input,
                output
            );
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            //string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
        }


        #endregion
    }
}
