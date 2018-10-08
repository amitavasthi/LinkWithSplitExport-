using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.LinkManager
{
    public partial class Test : WebUtilities.BasePage
    {
        #region Properties

        #endregion


        #region Constructor

        public Test()
            : base(true, true)
        { }

        #endregion


        #region Methods

        private void LoadStudyVariables()
        {
            List<object[]> variables = Global.Core.Variables.ExecuteReader(string.Format(
                "SELECT TOP 500 Id, Name, (SELECT Label FROM VariableLabels WHERE VariableLabels.IdVariable=Variables.Id) FROM Variables WHERE Id IN (SELECT DISTINCT IdVariable FROM Categories WHERE Name <> 'SystemMissing' AND Id IN (SELECT IdCategory FROM CategoryLinks)) AND Id IN (SELECT IdVariable FROM VariableLabels WHERE Label like '%{0}%')",
                Request.Params["Expression"]
            ));

            StringBuilder result = new StringBuilder();
            /*result.Append("[");

            foreach (object[] variable in variables)
            {
                result.Append(string.Format(
                    "\"Id\": \"{0}\", \"Name\": \"{1}\", \"Label\": \"{2}\"",
                    variable[0],
                    variable[1],
                    variable[2]
                ));
            }

            if (variables.Count != 0)
                result = result.Remove(result.Length - 1, 1);

            result.Append("]");*/

            foreach (object[] variable in variables)
            {
                result.Append(string.Format(
                    "<table><tr><td><input type=\"checkbox\" /></td><td>{0}</td></tr></table>",
                    variable[2]
                ));
            }

            HttpContext.Current.Response.Write(result.ToString());
            //pnlStudyVariables.Controls.Add(new LiteralControl(result.ToString()));
        }

        private void LoadTaxonomyVariables()
        {
            List<object[]> variables = Global.Core.Variables.ExecuteReader(string.Format(
                "SELECT TOP 500 Id, Name, (SELECT Label FROM TaxonomyVariableLabels WHERE TaxonomyVariableLabels.IdTaxonomyVariable=TaxonomyVariables.Id) FROM TaxonomyVariables WHERE Id IN (SELECT IdTaxonomyVariable FROM TaxonomyVariableLabels WHERE Label like '%{0}%')",
                Request.Params["Expression"]
            ));

            StringBuilder result = new StringBuilder();
            /*result.Append("[");

            foreach (object[] variable in variables)
            {
                result.Append(string.Format(
                    "\"Id\": \"{0}\", \"Name\": \"{1}\", \"Label\": \"{2}\"",
                    variable[0],
                    variable[1],
                    variable[2]
                ));
            }

            if (variables.Count != 0)
                result = result.Remove(result.Length - 1, 1);

            result.Append("]");*/

            foreach (object[] variable in variables)
            {
                result.Append(string.Format(
                    "<table><tr><td><input type=\"checkbox\" /></td><td>{0}</td></tr></table>",
                    variable[2]
                ));
            }

            //pnlTaxonomyVariables.Controls.Add(new LiteralControl(result.ToString()));
            HttpContext.Current.Response.Write(result.ToString());
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            /*if (Request.Params["Method"] == "LoadStudyVariables")
            {
                HttpContext.Current.Response.Clear();
                LoadStudyVariables();
                HttpContext.Current.Response.End();
            }
            else if (Request.Params["Method"] == "LoadTaxonomyVariables")
            {
                HttpContext.Current.Response.Clear();
                LoadTaxonomyVariables();
                HttpContext.Current.Response.End();
            }*/


            csStudyCategories.Limit = 500;
            csTaxonomyCategories.Limit = 500;

            csStudyCategories.SearchMode = WebUtilities.Controls.CategorySearchMode.Study;
            csTaxonomyCategories.SearchMode = WebUtilities.Controls.CategorySearchMode.Taxonomy;

            // Load the ids of all unlinked categories in the database.    
            List<object[]> unlinkedCategories = Global.Core.Categories.ExecuteReader(
                "SELECT Id FROM Categories WHERE Id NOT IN (SELECT IdCategory FROM CategoryLinks) AND Name <> 'SystemMissing'"
            );

            // Store the ids of the unlinked categories, for
            // performance reasons, in a dictionary rather than a list.
            Dictionary<Guid, object> categories = new Dictionary<Guid, object>();

            // Run through all unlinked categories of the database select result.
            foreach (object[] linkedCategory in unlinkedCategories)
            {
                categories.Add((Guid)linkedCategory[0], null);
            }

            // Define the delegate method that the category search
            // uses to check if a category should be displayed.
            csStudyCategories.CheckDisplayMethod = delegate (Guid idCategory, bool isTaxonomy)
            {
                // Check if the category is a non-linked category.
                return categories.ContainsKey(idCategory);
            };
        }

        #endregion
    }
}