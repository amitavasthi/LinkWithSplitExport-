using ApplicationUtilities;
using DatabaseCore;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInterface.Classes
{
    public class SpssWriter : BaseClasses.BaseWriter
    {
        #region Properties

        #endregion


        #region Constructor

        public SpssWriter(CultureInfo language, Core core)
            : base(core, language)
        { }

        #endregion


        #region Methods

        /// <summary>
        /// Exports a project to spss.
        /// </summary>
        /// <param name="variables">The variables to export.</param>
        public override string Export(TaxonomyVariable[] variables)

        {
            // Get a temp file name for the document.
            string fileName = Path.GetTempFileName() + ".sav";

            // Create a new spss data document.
            Spss.SpssDataDocument document = Spss.SpssDataDocument.Create(
                fileName
            );

            int i = 0;

            Dictionary<Guid, Spss.SpssVariable> spssVariables = new Dictionary<Guid, Spss.SpssVariable>();

            // Create a new string variable for the respondent ids.
            Spss.SpssVariable respondentVariable = new Spss.SpssStringVariable();

            respondentVariable.ColumnWidth = 36;

            // Set the respondent variable's name.
            respondentVariable.Name = "LiNK_Respondent_ID";

            // Set the respondent variable's label.
            respondentVariable.Label = "LiNK Respondent ID";

            // Add the respondent variable to the document's variables.
            document.Variables.Add(respondentVariable);

            // Run through all variables of the project.
            foreach (TaxonomyVariable variable in variables)
            {
                // Get the variable's type.
                MDMLib.DataTypeConstants variableType = (MDMLib.DataTypeConstants)variable.Type;

                Spss.SpssVariable var = null;

                // Switch on the variable's type.
                switch (variableType)
                {
                    case MDMLib.DataTypeConstants.mtDouble:
                    case MDMLib.DataTypeConstants.mtLong:

                        // Create a new numeric variable.
                        var = new Spss.SpssNumericVariable();

                        // Set the variable's name.
                        var.Name = variable.Name;

                        // Get the variable label for the export's
                        // language and set the spss variable's label.
                        //var.Label = variable.GetLabelText(base.Language.LCID);
                        var.Label = (string)this.Core.TaxonomyVariableLabels.GetValue(
                            "Label",
                            new string[] { "IdTaxonomyVariable", "IdLanguage" },
                            new object[] { variable.Id, base.Language.LCID }
                        );

                        break;
                    case MDMLib.DataTypeConstants.mtCategorical:

                        // Create a new variable.
                        var = new Spss.SpssNumericVariable();

                        // Set the variable's name.
                        var.Name = variable.Name;

                        // Get the variable label for the export's
                        // language and set the spss variable's label.
                        var.Label = (string)this.Core.TaxonomyVariableLabels.GetValue(
                            "Label",
                            new string[] { "IdTaxonomyVariable", "IdLanguage" },
                            new object[] { variable.Id, base.Language.LCID }
                        );

                        Spss.SpssNumericVariable v = var as Spss.SpssNumericVariable;

                        List<object[]> categories = this.Core.TaxonomyCategories.GetValues(
                            new string[] { "Id", "Value" },
                            new string[] { "IdTaxonomyVariable" },
                            new object[] { variable.Id }
                        );

                        int fCount = 0;
                        // Run through all categories of the variable.
                        foreach (object[] category in categories)
                        {
                            // Add the category as value label
                            // to the spss numeric variable.
                            v.ValueLabels.Add(
                                (int)category[1],
                                (string)this.Core.TaxonomyCategoryLabels.GetValue(
                                    "Label",
                                    new string[] { "IdTaxonomyCategory", "IdLanguage" },
                                    new object[] { category[0], base.Language.LCID }
                                )
                            );
                        }

                        break;
                    case MDMLib.DataTypeConstants.mtBoolean:
                        break;
                    case MDMLib.DataTypeConstants.mtDate:
                        break;
                    case MDMLib.DataTypeConstants.mtText:

                        // Create a new string variable.
                        var = new Spss.SpssStringVariable();
                        var.ColumnWidth = 4000;

                        // Set the variable's name.
                        var.Name = variable.Name;

                        // Get the variable label for the export's
                        // language and set the spss variable's label.
                        var.Label = (string)this.Core.TaxonomyVariableLabels.GetValue(
                            "Label",
                            new string[] { "IdTaxonomyVariable", "IdLanguage" },
                            new object[] { variable.Id, base.Language.LCID }
                        );

                        break;
                    default:
                        break;
                }

                // Check if the variable is set.
                if (var == null)
                    continue;

                spssVariables.Add(variable.Id, var);

                // Add the variable to the document's variables.
                document.Variables.Add(var);

                // Calculate the progress of the metadata export.
                base.MetadataProgress = i * 100 / variables.Length;
            }

            // Commit the header.
            document.CommitDictionary();

            base.MetadataProgress = 100;

            i = 0;

            Dictionary<Guid, Dictionary<Guid, Guid>> taxonomyCategoryLinks = new Dictionary<Guid, Dictionary<Guid, Guid>>();

            // Run through all variables.
            foreach (TaxonomyVariable variable in variables)
            {
                taxonomyCategoryLinks.Add(variable.Id, new Dictionary<Guid, Guid>());

                if (variable.Type == VariableType.Multi || variable.Type == VariableType.Single)
                {
                    // Get all category links of the taxonomy variable.
                    List<object[]> categoryLinks = this.Core.CategoryLinks.GetValues(
                        new string[] { "IdCategory", "IdTaxonomyCategory" },
                        new string[] { "IdTaxonomyVariable" },
                        new object[] { variable.Id }
                    );

                    // Run through all category links of the variable.
                    foreach (object[] categoryLink in categoryLinks)
                    {
                        bool enabled = (bool)this.Core.TaxonomyCategories.GetValue(
                            "Enabled",
                            new string[] { "Id" },
                            new object[] { categoryLink[1] }
                        );

                        if (!enabled)
                            continue;

                        if (!taxonomyCategoryLinks[variable.Id].ContainsKey((Guid)categoryLink[0]))
                            taxonomyCategoryLinks[variable.Id].Add((Guid)categoryLink[0], (Guid)categoryLink[1]);
                    }
                }
            }

            Dictionary<Guid, Dictionary<Guid, List<object[]>>> responses = new Dictionary<Guid, Dictionary<Guid, List<object[]>>>();

            // Run through all respondent ids.
            foreach (Guid idRespondent in this.Respondents)
            {
                // Get the respondent by the id.
                /*Respondent respondent = this.Core.Respondents.
                    GetSingle(idRespondent);*/

                Spss.SpssCase spssCase = document.Cases[-1];

                // Set the response value.
                spssCase.SetDBValue(
                    "LiNK_Respondent_ID",
                    idRespondent.ToString()
                );

                TaskCollection tasks = new TaskCollection();

                // Run through all variables.
                foreach (TaxonomyVariable variable in variables)
                {
                    if (!responses.ContainsKey(variable.Id))
                    {
                        StringBuilder commandText = new StringBuilder();

                        // Get all links of the taxonomy variable.
                        List<object[]> variableLinks = this.Core.VariableLinks.GetValues(
                            new string[] { "IdVariable" },
                            new string[] { "IdTaxonomyVariable" },
                            new object[] { variable.Id }
                        );

                        foreach (object[] variableLink in variableLinks)
                        {
                            commandText.Append(string.Format(
                                "SELECT IdRespondent, IdCategory, NumericAnswer, "+
                                "TextAnswer FROM [resp].[Var_{0}] UNION ALL ",
                                variableLink[0]
                            ));
                        }

                        if (variableLinks.Count != 0)
                            commandText = commandText.Remove(commandText.Length - 11, 11);

                        responses.Add(variable.Id, this.Core.TaxonomyVariables.ExecuteReaderDict<Guid>(
                            commandText.ToString(),
                            new object[] {}
                        ));
                    }

                    tasks.Add(() => WriteVariableResponses(
                        variable,
                        idRespondent,
                        taxonomyCategoryLinks,
                        spssVariables,
                        spssCase,
                        responses[variable.Id]
                    ));
                }

                tasks.WaitAll();

                // Commit the spss case.
                spssCase.Commit();

                // Calculate the progress of the case data export.
                base.CaseDataProgress = i++ * 100 / this.Respondents.Count;
            }

            base.CaseDataProgress = 100;

            // Close the spss document.
            document.Close();

            return fileName;
        }

        private void WriteVariableResponses(
            TaxonomyVariable variable,
            Guid idRespondent,
            Dictionary<Guid, Dictionary<Guid, Guid>> taxonomyCategoryLinks,
            Dictionary<Guid, Spss.SpssVariable> spssVariables,
            Spss.SpssCase spssCase,
            Dictionary<Guid, List<object[]>> responses
        )
        {
            if (!responses.ContainsKey(idRespondent))
                return;

            MDMLib.DataTypeConstants variableType = (MDMLib.DataTypeConstants)variable.Type;

            // Run through all variable links of the taxonomy variable.
            //foreach (object[] variableLink in variableLinks)
            {
                // Get all responses for the respondent and variable.
                /*List<object[]> responses = this.Core.Responses[(Guid)variableLink[0]].GetValues(
                    new string[] { "IdCategory", "NumericAnswer", "TextAnswer" },
                    new string[] { "IdRespondent" },
                    new object[] { idRespondent }
                );*/

                // Run through all responses of the respondent.
                foreach (object[] response in responses[idRespondent])
                {
                    object value = null;

                    // Switch on the variable type.
                    switch (variableType)
                    {
                        case MDMLib.DataTypeConstants.mtBoolean:
                            break;
                        case MDMLib.DataTypeConstants.mtCategorical:

                            if (!taxonomyCategoryLinks[variable.Id].ContainsKey((Guid)response[1]))
                                continue;

                            /*value = this.Core.Categories.GetValue(
                                "Value",
                                "Id",
                                response[1]
                            );*/
                            value = this.Core.TaxonomyCategories.GetValue(
                                "Value",
                                "Id",
                                taxonomyCategoryLinks[variable.Id][(Guid)response[1]]
                            );

                            break;
                        case MDMLib.DataTypeConstants.mtDate:
                            break;
                        case MDMLib.DataTypeConstants.mtDouble:
                        case MDMLib.DataTypeConstants.mtLong:

                            if (response[2] != null)
                                value = (double)response[2];

                            break;
                        case MDMLib.DataTypeConstants.mtText:

                            value = (string)response[3];

                            if (((string)value).Length > spssVariables[variable.Id].ColumnWidth)
                                spssVariables[variable.Id].ColumnWidth = ((string)value).Length;

                            break;
                    }

                    // Check if a value was set.
                    if (value != null)
                    {
                        // Set the response value.
                        spssCase.SetDBValue(
                            variable.Name,
                            value
                        );
                    }
                }
            }
        }

        #endregion
    }
}
