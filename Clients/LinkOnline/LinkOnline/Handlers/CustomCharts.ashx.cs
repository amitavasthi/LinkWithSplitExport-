using Homescreen1;
using LinkOnline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LinkOnline.Handlers
{
    public class CustomCharts : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public CustomCharts()
        {
            base.Methods.Add("RenderContent", RenderContent);
            base.Methods.Add("Init", Init);
        }

        #endregion


        #region Methods

        #endregion


        #region Event Handlers

        /// <summary>
        /// Renders the content of a custom chart.
        /// </summary>
        /// <param name="context">The current http context.</param>
        private void RenderContent(HttpContext context)
        {
            // Get the id of the custom chart to render.
            Guid idCustomChart = Guid.Parse(context.Request.Params["Id"]);

            // Build the full path to the custom chart definition file.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "CustomCharts",
                Global.Core.ClientName,
                idCustomChart + ".xml"
            );

            Homescreen homescreen = new Homescreen(fileName);

            //homescreen.ContentWidth = int.Parse(context.Request.Params["ContentWidth"]);
            //homescreen.ContentHeight = int.Parse(context.Request.Params["ContentHeight"]);
            homescreen.ContentWidth = "ContentWidth";
            homescreen.ContentHeight = "ContentHeight";

            homescreen.Parse();

            homescreen.Render();

            string result = File.ReadAllText(Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "CustomCharts",
                "CustomChart.html"
            )).Replace("###CONTENT###", homescreen.ToHtml());

            context.Response.ContentType = "text/html";
            context.Response.Write(result);
        }

        /// <summary>
        /// Initializes a custom chart render.
        /// </summary>
        /// <param name="context">The current http context.</param>
        private void Init(HttpContext context)
        {
            // Get the id of the custom chart to render.
            Guid idCustomChart = Guid.Parse(context.Request.Params["Id"]);

            string result = File.ReadAllText(Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "CustomCharts",
                "CustomChartInit.html"
            )).Replace("###IdCustomChart###", idCustomChart.ToString());

            context.Response.ContentType = "text/html";
            context.Response.Write(result);
        }

        #endregion
    }
}