using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Crosstables.Classes.Charts
{
    public class Chart : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the display type of the chart.
        /// </summary>
        public DisplayType DisplayType { get; set; }

        /// <summary>
        /// Gets or sets the full path to the report definition file.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the title of the chart.
        /// </summary>
        public string Title { get; set; }

        public Guid? IdVariable { get; set; }

        public int DecimalPlaces { get; set; }

        #endregion


        #region Constructor

        public Chart()
        {
        }

        #endregion


        #region Methods

        public void Render()
        {
            string xPath = string.Format(
                "Report/Results/Variable"
            );

            string chart = File.ReadAllText(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Charts",
                this.DisplayType + ".html"
            ));

            chart = chart.Replace("###SOURCE###", this.Source.Replace("\\", "/"));
            chart = chart.Replace("###PATH###", xPath);
            chart = chart.Replace("###TITLE###", HttpUtility.HtmlEncode(this.Title));
            chart = chart.Replace("###DECIMALPLACES###", this.DecimalPlaces.ToString());

            if (this.IdVariable.HasValue)
                chart = chart.Replace("###IdVariable###", this.IdVariable.Value.ToString());

            Guid idChartCache = Guid.NewGuid();

            // Build the path to the temp chart cache file.
            string fileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                "ChartCache",
                HttpContext.Current.Session.SessionID,
                idChartCache + ".html"
            );

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, chart);

            this.Attributes.Add("id", "chartFrame" + idChartCache);
            this.Attributes.Add("frameborder", "0");

            this.Style.Add("width", "100%");
            this.Style.Add("height", "100%");

            /*this.Attributes.Add("src", string.Format(
                "/Fileadmin/Temp/ChartCache/{0}.html",
                idChartCache
            ));*/

            base.Controls.Add(new LiteralControl(
                "<iframe id=\"chartFrame" + idChartCache + "\" frameborder=\"0\" style=\"width:100%;height:100%\" "+
                "onload=\"InitChart(this, '/Fileadmin/Temp/ChartCache/"+ 
                HttpContext.Current.Session.SessionID +"/" + idChartCache + ".html');\"></iframe>"
            ));
        }

        #endregion
    }
}
