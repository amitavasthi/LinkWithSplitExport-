using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für QAHandler
    /// </summary>
    public class QAHandler : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public QAHandler()
        {
            base.Methods.Add("SignOff", SignOff);
        }

        #endregion


        #region Web Methods

        private void SignOff(HttpContext context)
        {
            // Get the source to the item to sign off
            // from the http request's parameters.
            string source = context.Request.Params["Source"];

            // Parse the identity of the item to sign off
            // from the http request's parameters.
            Guid identity = Guid.Parse(context.Request.Params["Identity"]);

            // Parse the id of the study to sign off
            // from the http request's parameters.
            Guid idStudy = Guid.Parse(context.Request.Params["IdStudy"]);

            List<object[]> categories = new List<object[]>();
            List<object[]> variables = new List<object[]>();

            StringBuilder builkUpdateBuilder = new StringBuilder();

            // Switch on the item's source.
            switch (source)
            {
                case "Studies":

                    // Get all category links of the study.
                    /*categories = Global.Core.CategoryLinks.ExecuteReader(string.Format(
                        "SELECT Id FROM [CategoryLinks] WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{0}')",
                        identity
                    ));

                    // Get all variable links of the study.
                    variables = Global.Core.VariableLinks.ExecuteReader(string.Format(
                        "SELECT Id FROM [VariableLinks] WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{0}')",
                        identity
                    ));*/
                    builkUpdateBuilder.Append(string.Format(
                        "UPDATE [CategoryLinks] SET QA=1 WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{0}');",
                        identity
                    ));
                    builkUpdateBuilder.Append(string.Format(
                        "UPDATE [VariableLinks] SET QA=1 WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{0}');",
                        identity
                    ));

                    break;
                case "TaxonomyVariables":

                    // Get all category links of the taxonomy variable.
                    /*categories = Global.Core.CategoryLinks.ExecuteReader(string.Format(
                        "SELECT Id FROM [CategoryLinks] WHERE IdTaxonomyVariable='{0}' AND IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{1}')",
                        identity,
                        idStudy
                    ));

                    // Get all variable links of the taxonomy variable.
                    variables = Global.Core.VariableLinks.ExecuteReader(string.Format(
                        "SELECT Id FROM [VariableLinks] WHERE IdTaxonomyVariable='{0}' AND IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{1}')",
                        identity,
                        idStudy
                    ));*/
                    builkUpdateBuilder.Append(string.Format(
                        "UPDATE [CategoryLinks] SET QA=1 WHERE IdTaxonomyVariable='{0}' AND IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{1}');",
                        identity,
                        idStudy
                    ));
                    builkUpdateBuilder.Append(string.Format(
                        "UPDATE [VariableLinks] SET QA=1 WHERE IdTaxonomyVariable='{0}' AND IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{1}');",
                        identity,
                        idStudy
                    ));

                    break;
                case "TaxonomyCategories":

                    // Get all category links of the taxonomy category.
                    /*categories = Global.Core.CategoryLinks.ExecuteReader(string.Format(
                        "SELECT Id FROM [CategoryLinks] WHERE IdTaxonomyCategory='{0}' AND IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{1}')",
                        identity,
                        idStudy
                    ));*/

                    builkUpdateBuilder.Append(string.Format(
                        "UPDATE [CategoryLinks] SET QA=1 WHERE IdTaxonomyCategory='{0}' AND IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{1}');",
                        identity,
                        idStudy
                    ));

                    break;
                case "Categories":

                    // Get all category links of the taxonomy category.
                    /*categories = Global.Core.CategoryLinks.GetValues(
                        new string[] { "Id" },
                        new string[] { "IdCategory" },
                        new object[] { identity }
                    );*/
                    builkUpdateBuilder.Append(string.Format(
                        "UPDATE [CategoryLinks] SET QA=1 WHERE IdCategory='{1}';",
                        identity
                    ));

                    break;
            }

            // Run through all categories.
            /*foreach (object[] category in categories)
            {
                // Set the QA flag of the category link to 1.
                Global.Core.CategoryLinks.SetValue(
                    "Id=" + category[0],
                    "QA",
                    1
                );
            }

            // Run through all variables.
            foreach (object[] variable in variables)
            {
                // Set the QA flag of the variable link to 1.
                Global.Core.VariableLinks.SetValue(
                    "Id=" + variable[0],
                    "QA",
                    1
                );
            }*/

            Global.Core.CategoryLinks.ExecuteQuery(builkUpdateBuilder.ToString());

            // Log the sign off action.
            LogSignOff(
                source,
                identity,
                idStudy
            );
        }

        #endregion


        #region Methods

        private void LogSignOff(string source, Guid identity, Guid idStudy)
        {
            // Create a new qa log entry.
            QALog qaLog = new QALog(Global.Core.QALogs);
            qaLog.IdStudy = idStudy;
            qaLog.Source = source;
            qaLog.Identity = identity;
            qaLog.IdUser = Global.IdUser.Value;
            qaLog.CreationDate = DateTime.Now;

            qaLog.Insert();
        }

        #endregion
    }
}