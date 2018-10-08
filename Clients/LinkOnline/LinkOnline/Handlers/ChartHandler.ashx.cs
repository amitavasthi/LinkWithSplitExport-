using Crosstables.Classes.Charts.ChartDataRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für ChartHandler
    /// </summary>
    public class ChartHandler : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public ChartHandler()
        {
            base.Methods.Add("GetData", GetData);
        }

        #endregion


        #region Methods

        #endregion


        #region Web Methods

        private void GetData(HttpContext context)
        {
            // Get the full path to the report definition
            // file from the http request's parameters.
            string source = context.Request.Params["Source"];

            // Get the xPath to the xml node that contains the chart's
            // dimensions from the http request's parameters.
            string pathDimensions = context.Request.Params["PathDimensions"];

            // Create a new string builder that
            // contains the result json string.
            StringBuilder result = new StringBuilder();

            string renderer = context.Request.Params["Renderer"];

            ChartDataRenderer dataRenderer = null;

            switch (renderer)
            {
                case "MultiSeries":
                    // Get the xPath to the xml node that contains the chart's
                    // values from the http request's parameters.
                    string pathMeasures = context.Request.Params["PathMeasures"];

                    // Create a new multi series renderer.
                    dataRenderer = new ChartDataRendererMultiSeries(
                        source,
                        pathDimensions,
                        pathMeasures
                    );
                    break;
                case "SingleSeries":
                    // Create a new single series renderer.
                    dataRenderer = new ChartDataRendererSingleSeries(
                        source,
                        pathDimensions
                    );
                    break;
                case "Words":
                    Guid idVariable = Guid.Parse(
                        context.Request.Params["IdVariable"]
                    );

                    // Create a new words renderer.
                    dataRenderer = new ChartDataRendererWords(
                        idVariable,
                        source,
                        Global.Core
                    );
                    break;
            }

            if (dataRenderer == null)
                return;

            dataRenderer.Core = Global.Core;
            dataRenderer.IdLanguage = 2057;

            dataRenderer.Render(
                result,
                Global.HierarchyFilters[source]
            );

            if (dataRenderer.Document != null)
                dataRenderer.Document.Save(source);

            context.Response.Write(result.ToString());
        }

        #endregion
    }
}