using Crosstables.Classes.ReportDefinitionClasses;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace BenchmarkTest1
{
    public class BenchmarkTest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the number of
        /// simultaneously aggregations to test.
        /// </summary>
        public int Tests { get; set; }

        /// <summary>
        /// Gets or sets the path to the report definition file which
        /// is used to do the benchmark aggregation tests.
        /// </summary>
        public string ReportDefinitionFileName { get; set; }

        /// <summary>
        /// Gets or sets the current aggregating report's sessions.
        /// </summary>
        public List<HttpSessionState> Sessions { get; set; }

        /// <summary>
        /// Gets if the benchmark test is finished.
        /// </summary>
        public bool Finished
        {
            get
            {
                foreach (HttpSessionState session in this.Sessions)
                {
                    if (session["DataAggregationProgress"] == null || int.Parse(session["DataAggregationProgress"].ToString()) != 100)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the average aggregation time using the active
        /// aggregation sessions and their aggregation time. 
        /// </summary>
        public long AverageAggregationTime
        {
            get
            {
                long total = 0;
                int count = 0;

                foreach (HttpSessionState session in this.Sessions)
                {
                    if (session["DataAggregationTime"] == null)
                        continue;

                    TimeSpan aggregationTime = (TimeSpan)session["DataAggregationTime"];

                    total += (long)aggregationTime.TotalMilliseconds;
                    count++;
                }

                if (count != 0)
                    return total / count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets or sets how many steps the benchmark tests should do.
        /// </summary>
        public int Steps { get; set; }

        /// <summary>
        /// Gets or sets what is the current performing step.
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Gets or sets how many aggregations the
        /// benchmark tests should perform in total.
        /// </summary>
        public int TotalTests { get; set; }

        public DatabaseCore.Core Core { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the BenchmarkTest class.
        /// </summary>
        /// <param name="total">Defines how many aggregations the benchmark test should perform in total.</param>
        /// <param name="steps">Defines how many steps the benchmark test should do.</param>
        /// <param name="step">Defines what is the current performing step.</param>
        /// <param name="reportDefinitionFileName">Defines the full path to the report definition file which is used to perform the benchmark tests.</param>
        /// <param name="core">The instance of the database connection object which is used to perform the benchmark tests.</param>
        public BenchmarkTest(int total, int steps, int step, string reportDefinitionFileName, DatabaseCore.Core core)
        {
            this.Sessions = new List<HttpSessionState>();
            this.Steps = steps;
            this.Step = step;
            this.TotalTests = total;

            this.Tests = (total / steps) * step;
            this.ReportDefinitionFileName = reportDefinitionFileName;
            this.Core = core;
        }

        #endregion


        #region Methods

        public void StartTest()
        {
            for (int i = 0; i < this.Tests; i++)
            {
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(Aggregate);

                // Start the data aggregation.
                Thread thread = new Thread(Aggregate);

                thread.Start(HttpContext.Current);
            }
        }

        private void Aggregate(object currentHttpContext)
        {
            HttpContext httpContext = FakeHttpContext(
                new Dictionary<string, object>(),
                ((HttpContext)currentHttpContext).Request.Url.ToString()
            );

            this.Sessions.Add(httpContext.Session);

            ReportDefinition reportDefinition = new ReportDefinition(
                this.Core,
                this.ReportDefinitionFileName,
                new Crosstables.Classes.HierarchyClasses.HierarchyFilter(null)
            );

            ReportCalculator calculator = new ReportCalculator(
                reportDefinition,
                this.Core,
                httpContext.Session
            );

            //try
            //{
                calculator.Aggregate("1.0.0.0", false);
            /*}
            catch {
                httpContext.Session["DataAggregationProgress"] = 100;
            }*/
        }

        private HttpContext FakeHttpContext(Dictionary<string, object> sessionVariables, string path)
        {
            var httpRequest = new HttpRequest(string.Empty, path, string.Empty);
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponce);
            httpContext.User = new GenericPrincipal(new GenericIdentity("username"), new string[0]);
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("username"), new string[0]);

            var sessionContainer = new HttpSessionStateContainer(
              "id",
              new SessionStateItemCollection(),
              new HttpStaticObjectsCollection(),
              10,
              true,
              HttpCookieMode.AutoDetect,
              SessionStateMode.InProc,
              false
            );

            foreach (var var in sessionVariables)
            {
                sessionContainer.Add(var.Key, var.Value);
            }

            SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);
            return httpContext;
        }

        #endregion
    }
}