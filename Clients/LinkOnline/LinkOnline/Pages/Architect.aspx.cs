using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages
{
    public partial class Architect : WebUtilities.BasePage
    {
        #region Properties

        /// <summary>
        /// Gets or sets a dictionary of the
        /// available methods of this page.
        /// </summary>
        public Dictionary<string, Method> Methods { get; set; }

        #endregion


        #region Constructor

        public Architect()
        {
            this.Methods = new Dictionary<string, Method>();

            this.Methods.Add("LoadSuggestions", LoadSuggestions);
        }

        #endregion


        #region Methods

        private void LoadSuggestions()
        {
            Test();
            return;

            // Get all variables that aren't linked.
            List<object[]> unlinked = Global.Core.Variables.ExecuteReader(
                "SELECT Variables.Id, Variables.Name, VariableLabels.Label, Variables.IdStudy FROM Variables" +
                "INNER JOIN VariableLabels ON VariableLabels.IdVariable=Variables.Id WHERE " +
                "Variables.Id NOT IN (SELECT IdVariable FROM VariableLinks)"
            );

            // Run through all variables that aren't linked.
            foreach (object[] variable in unlinked)
            {

            }
        }

        private void Test()
        {
            // Get all taxonomy variables.
            Dictionary<Guid, List<object[]>> taxonomyVariables = Global.Core.Variables.ExecuteReaderDict<Guid>(
                "SELECT TaxonomyVariables.Id, TaxonomyVariables.Name, " +
                "TaxonomyVariableLabels.Label FROM TaxonomyVariables " +
                "INNER JOIN TaxonomyVariableLabels ON " +
                "TaxonomyVariableLabels.IdTaxonomyVariable=TaxonomyVariables.Id",
                new object[] { }
            );

            // Select all variable links.
            Dictionary<string, List<object[]>> variableLinks = Global.Core.Variables.ExecuteReaderDict<string>(
                "SELECT Variables.Name, VariableLinks.IdTaxonomyVariable, VariableLinks.IdVariable FROM VariableLinks " +
                "INNER JOIN Variables ON VariableLinks.IdVariable=Variables.Id",
                new object[] { }
            );

            // Select all study variables.
            Dictionary<Guid, List<object[]>> studyVariables = Global.Core.Variables.ExecuteReaderDict<Guid>(
                "SELECT Variables.Id, Variables.Name, VariableLabels.Label FROM Variables " +
                "INNER JOIN VariableLabels ON VariableLabels.IdVariable=Variables.Id ",
                //"WHERE Variables.Id IN (SELECT IdVariable FROM VariableLinks)",
                new object[] { }
            );

            // Select all unlinked variables.
            Dictionary<Guid, List<object[]>> unlinkedVariables = Global.Core.Variables.ExecuteReaderDict<Guid>(
                "SELECT Variables.Id, Variables.Name, VariableLabels.Label FROM Variables " +
                "INNER JOIN VariableLabels ON VariableLabels.IdVariable=Variables.Id " +
                "WHERE Variables.Id NOT IN (SELECT IdVariable FROM VariableLinks)",
                new object[] { }
            );

            Dictionary<Guid, List<Suggestion>> suggestions = new Dictionary<Guid, List<Suggestion>>();

            foreach (Guid idUnlinkedVariable in unlinkedVariables.Keys)
            {
                if (!variableLinks.ContainsKey((string)unlinkedVariables[idUnlinkedVariable][0][1]))
                    continue;

                Guid idTaxonomyVariable = (Guid)variableLinks[(string)unlinkedVariables[idUnlinkedVariable][0][1]][0][1];
                bool taxonomyVariableSame = true;

                foreach (object[] variableLink in variableLinks[(string)unlinkedVariables[idUnlinkedVariable][0][1]])
                {
                    if (idTaxonomyVariable != (Guid)variableLink[1])
                    {
                        taxonomyVariableSame = false;
                        break;
                    }
                }

                if (!taxonomyVariableSame)
                    continue;

                Suggestion suggestion = new Suggestion();

                if (!suggestions.ContainsKey(idTaxonomyVariable))
                    suggestions.Add(idTaxonomyVariable, new List<Suggestion>());

                suggestions[idTaxonomyVariable].Add(suggestion);
            }

            foreach (Guid idTaxonomyVariable in suggestions.Keys)
            {
                Response.Write("<div class=\"Variable BackgroundColor1\">");

                Response.Write(string.Format(
                    "<div style=\"float:right;\">{0} suggestions</div>",
                    suggestions[idTaxonomyVariable].Count
                ));

                Response.Write(string.Format(
                    "<div class=\"VariableLabel\">{0}</div>",
                    (string)taxonomyVariables[idTaxonomyVariable][0][2]
                ));

                Response.Write("</div>");
            }
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            this.EnableViewState = false;

            if (Request.Params["Method"] == null)
                return;

            string method = Request.Params["Method"];

            if (!this.Methods.ContainsKey(method))
                throw new NotImplementedException();

            Response.Clear();

            this.Methods[method]();

            Response.End();
        }

        #endregion
    }

    public delegate void Method();

    public class Suggestion
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the
        /// taxonomy variable for the link.
        /// </summary>
        public Guid IdTaxonomyVariable { get; set; }

        /// <summary>
        /// Gets or sets the id of the
        /// study variable for the link.
        /// </summary>
        public Guid IdVariable { get; set; }

        #endregion


        #region Constructor

        public Suggestion()
        {

        }

        #endregion


        #region Methods

        #endregion
    }
}