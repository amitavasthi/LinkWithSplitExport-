using LinkOnline.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for EquationWizard
    /// </summary>
    public class EquationWizards : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public EquationWizards()
        {
            base.Methods.Add("GetEquationWizards", GetEquationWizards);
            base.Methods.Add("GetEquationWizard", GetEquationWizard);
        }

        #endregion


        #region Methods

        private void LoadEquationWizards(string directoryName, Dictionary<string, List<EquationWizard>> result)
        {
            if (!Directory.Exists(directoryName))
                return;

            EquationWizard equationWizard;
            // Run through all defined equation wizards.
            foreach (string fileName in Directory.GetFiles(directoryName))
            {
                equationWizard = new EquationWizard(fileName);

                if (!result.ContainsKey(equationWizard.Properties.Section))
                    result.Add(equationWizard.Properties.Section, new List<EquationWizard>());

                result[equationWizard.Properties.Section].Add(equationWizard);
            }
        }

        #endregion


        #region Web Methods

        private void GetEquationWizards(HttpContext context)
        {
            // Create a new string builder that
            // contains the result JSON string.
            StringBuilder result = new StringBuilder();
            result.Append("{ \"Sections\": [");

            // Build the full path to the
            // global equation wizards.
            string directoryName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "EquationWizards"
            );

            Dictionary<string, List<EquationWizard>> equationWizards = new Dictionary<string, List<EquationWizard>>();

            // Load all global equatio wizards.
            LoadEquationWizards(directoryName, equationWizards);

            // Build the full path to the
            // client specific equation wizards.
            directoryName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "EquationWizards",
                Global.Core.ClientName
            );

            // Load the client specific equation wizards.
            LoadEquationWizards(directoryName, equationWizards);

            foreach (string section in equationWizards.Keys)
            {
                result.Append("{");

                result.Append(string.Format(
                    "\"{0}\": \"{1}\",",
                    "Name",
                    section
                ));

                result.Append("\"Wizards\": [");

                foreach (EquationWizard equationWizard in equationWizards[section])
                {
                    result.Append("{");

                    result.Append(string.Format(
                        "\"{0}\": \"{1}\",",
                        "Section",
                        equationWizard.Properties.Section
                    ));

                    result.Append(string.Format(
                        "\"{0}\": \"{1}\",",
                        "Name",
                        equationWizard.Properties.Name
                    ));

                    result.Append(string.Format(
                        "\"{0}\": \"{1}\",",
                        "Description",
                        equationWizard.Properties.Description
                    ));

                    result.Append(string.Format(
                        "\"{0}\": \"{1}\",",
                        "Source",
                        equationWizard.FileName.Replace("\\", "/")
                    ));

                    result.RemoveLastComma();
                    result.Append("},");
                }

                result.RemoveLastComma();
                result.Append("]},");
            }

            result.RemoveLastComma();

            result.Append("]}");

            context.Response.Write(result.ToString());
        }

        private void GetEquationWizard(HttpContext context)
        {
            // Get the full path to the equation wizard definition
            // file from the http request's parameters.
            string fileName = context.Request.Params["Source"];

            // Create a new equation wizard by
            // the equation wizard definition file.
            EquationWizard equationWizard = new EquationWizard(fileName);

            // Parse the full equation wizard definition.
            equationWizard.ParseFull();

            // Create a new string builder that
            // contains the result JSON string.
            StringBuilder result = new StringBuilder();

            result.Append("{ \"PlaceHolders\": [");

            // Run through all place holders of the equation wizard.
            foreach (string name in equationWizard.PlaceHolders.Keys)
            {
                // Render the place holder details to the result JSON string.
                result.Append(base.ToJson(new string[] {
                    "Name",
                    "Type"
                }, new object[]
                {
                    Global.LanguageManager.GetText("EquationWizard"+ 
                    equationWizard.PlaceHolders[name].Name),
                    equationWizard.PlaceHolders[name].Type
                }));

                result.Append(",");
            }

            result.RemoveLastComma();
            result.Append("]}");

            context.Response.Write(result.ToString());
            result.Clear();
        }

        #endregion
    }
}