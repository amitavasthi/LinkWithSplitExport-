using ApplicationUtilities.Classes;
using Crosstables.Classes.ReportDefinitionClasses;
using DatabaseCore.Items;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataAnalyser1.Classes
{
    public class DataAnalyser
    {
        #region Properties

        /// <summary>
        /// Gets or sets which study's data should be analized.
        /// </summary>
        public Guid IdStudy { get; set; }

        public DatabaseCore.Core Core { get; set; }

        #endregion


        #region Constructor

        public DataAnalyser(DatabaseCore.Core core, Guid idStudy)
        {
            this.Core = core;
            this.IdStudy = idStudy;
        }

        #endregion


        #region Methods

        public byte[] ExportValues()
        {
            string fileName = Path.GetTempFileName() + ".xlsx";

            // Create a new excel writer to build the result document.
            ExcelWriter writer = new ExcelWriter(fileName);

            // Get the study's base value.
            double studyBase = this.Core.Respondents.GetCount("IdStudy", this.IdStudy);

            // Render the document's headline.
            writer.Write(0, "Variable");
            //writer.Write(0, "Category");
            writer.Write(1, "Variable Type");
            writer.Write(2, "% Complete");
            writer.Write(3, "Valid records");

            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 3].Interior.Color = SpreadsheetGear.Color.FromArgb(54, 94, 146);
            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 3].Font.Color = SpreadsheetGear.Color.FromArgb(255, 255, 255);

            writer.ActiveSheet.Cells[writer.Position, 0].EntireColumn.ColumnWidth = 50;
            writer.ActiveSheet.Cells[writer.Position, 0].EntireColumn.WrapText = true;

            writer.ActiveSheet.Cells[writer.Position, 1, writer.Position, 3].EntireColumn.ColumnWidth = 12;

            writer.ActiveSheet.WindowInfo.FreezePanes = true;
            writer.ActiveSheet.Cells[0, 0, 0, 4].Select();

            writer.NewLine();

            // Get all variables of the study.
            List<object[]> variables = this.Core.Variables.GetValues(
                new string[] { "Id", "Name", "Type" },
                new string[] { "IdStudy" },
                new object[] { this.IdStudy }
            );

            Dictionary<string, double> variableBases = new Dictionary<string, double>();

            // Run through all variables of the study.
            foreach (object[] variable in variables)
            {
                object idSystemMissingCategory = this.Core.Categories.GetValue(
                    "Id",
                    new string[] { "IdVariable", "Name" },
                    new object[] { variable[0], "SystemMissing" }
                );
                double baseValue;

                if (idSystemMissingCategory != null)
                {
                    if (this.Core.CaseDataLocation == CaseDataLocation.Sql || (VariableType)variable[2] == VariableType.Text)
                    {
                        baseValue = (int)this.Core.Responses[(Guid)variable[0]].ExecuteReader(string.Format(
                            "SELECT Count(*) FROM resp.[Var_{0}] WHERE IdCategory <> '{1}' OR IdCategory IS NULL",
                            variable[0],
                            idSystemMissingCategory
                        ), typeof(int))[0][0];
                    }
                    else
                    {
                        baseValue = 0;

                        CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                        string commandText = "";

                        switch ((VariableType)variable[2])
                        {
                            case VariableType.Numeric:
                                commandText = string.Format(
                                    "SELECT IdRespondent, NumericAnswer FROM [resp].[Var_{0}]",
                                    variable[0]
                                );
                                break;
                            case VariableType.Text:
                                commandText = string.Format(
                                    "SELECT IdRespondent, TextAnswer FROM [resp].[Var_{0}]",
                                    variable[0]
                                );
                                break;
                            case VariableType.Single:
                            case VariableType.Multi:
                                commandText = string.Format(
                                    "SELECT IdRespondent, IdCategory FROM [resp].[Var_{0}]",
                                    variable[0]
                                );
                                break;
                        }

                        List<object[]> responses = dataLink.Select2(
                            commandText,
                            this.Core.ClientName
                        );

                        if ((VariableType)variable[2] != VariableType.Single && (VariableType)variable[2] != VariableType.Multi)
                        {
                            baseValue = responses.Count;
                        }
                        else
                        {
                            foreach (object[] response in responses)
                            {
                                if ((Guid)response[1] == (Guid)idSystemMissingCategory)
                                    continue;

                                baseValue++;
                            }
                        }
                    }
                }
                else
                {
                    if (this.Core.CaseDataLocation == CaseDataLocation.Sql)
                    {
                        baseValue = this.Core.Responses[(Guid)variable[0]].GetCount();
                    }
                    else
                    {
                        CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                        CaseDataCore1.CaseDataType dataType = CaseDataCore1.CaseDataType.Categorical;

                        if ((VariableType)variable[2] == VariableType.Single)
                            dataType = CaseDataCore1.CaseDataType.Numeric;
                        else if ((VariableType)variable[2] == VariableType.Text)
                            dataType = CaseDataCore1.CaseDataType.Text;

                        baseValue = dataLink.Count(
                            (Guid)variable[0],
                            dataType,
                            this.Core.ClientName
                        );
                    }
                }

                writer.Write(0, (string)variable[1]);
                writer.Write(1, ((VariableType)variable[2]).ToString());
                writer.Write(2, Math.Round(baseValue * 100 / studyBase, 0).ToString() + "%");
                writer.Write(3, baseValue.ToString());

                if (baseValue == 0)
                    writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 3].Interior.Color = SpreadsheetGear.Color.FromArgb(255, 0, 0);

                writer.NewLine();

                variableBases.Add((string)variable[1], baseValue);
            }

            writer.NewSheet("Categories");

            writer.Write(0, "Variable");
            writer.Write(1, "Category");
            writer.Write(2, "% Complete");
            writer.Write(3, "Base");
            writer.Write(4, "Valid records");

            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 4].Interior.Color = SpreadsheetGear.Color.FromArgb(54, 94, 146);
            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 4].Font.Color = SpreadsheetGear.Color.FromArgb(255, 255, 255);

            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 1].EntireColumn.ColumnWidth = 50;
            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 1].EntireColumn.WrapText = true;

            writer.ActiveSheet.Cells[writer.Position, 2, writer.Position, 4].EntireColumn.ColumnWidth = 12;

            writer.ActiveSheet.WindowInfo.FreezePanes = true;
            writer.ActiveSheet.Cells[0, 0, 0, 4].Select();

            writer.NewLine();

            // Run through all variables of the study.
            foreach (object[] variable in variables)
            {
                VariableType variableType = (VariableType)variable[2];

                // Check if the variable is a categorical variable.
                if (variableType != VariableType.Single && variableType != VariableType.Multi)
                    continue;

                // Get all categories of the variable.
                List<object[]> categories = this.Core.Categories.GetValues(
                    new string[] { "Id", "Name" },
                    new string[] { "IdVariable" },
                    new object[] { variable[0] }
                );

                List<object[]> variableData = new List<object[]>();

                if (this.Core.CaseDataLocation == CaseDataLocation.File)
                {
                    CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();
                    variableData = dataLink.Select2(string.Format(
                        "SELECT IdRespondent, IdCategory FROM [resp].[Var_{0}]",
                        variable[0]
                    ), this.Core.ClientName);
                }

                // Run through all categories of the variable.
                foreach (object[] category in categories)
                {
                    // Check if the category is the system missing category.
                    if ((string)category[1] == "SystemMissing")
                        continue;

                    double baseValue = 0;

                    if (this.Core.CaseDataLocation == CaseDataLocation.Sql)
                    {
                        baseValue = this.Core.Responses[(Guid)variable[0]].GetCount("IdCategory", category[0]);
                    }
                    else
                    {
                        foreach (object[] response in variableData)
                        {
                            if ((Guid)response[1] == (Guid)category[0])
                                baseValue++;
                        }
                    }

                    writer.Write(0, (string)variable[1]);
                    writer.Write(1, (string)category[1]);
                    writer.Write(2, Math.Round(baseValue * 100 / variableBases[(string)variable[1]], 0).ToString() + "%");
                    writer.Write(3, variableBases[(string)variable[1]].ToString());
                    writer.Write(4, baseValue.ToString());

                    if (baseValue == 0)
                        writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 4].Interior.Color = SpreadsheetGear.Color.FromArgb(255, 0, 0);

                    writer.NewLine();
                }
            }

            return writer.Save();
        }


        /// <summary>
        /// Checks if all of the study's respondent have a weight.
        /// </summary>
        /// <param name="idVariable">The weighting variable.</param>
        public List<DataAnalyserLog> CheckWeights(Variable variable)
        {
            // Create a list that stores all result data analyser log messages.
            List<DataAnalyserLog> result = new List<DataAnalyserLog>();

            List<object[]> respondentsMissing;
            List<object[]> respondentsZero;

            if (this.Core.CaseDataLocation == CaseDataLocation.Sql)
            {
                respondentsMissing = this.Core.Respondents.ExecuteReader(string.Format(
                    "SELECT (SELECT OriginalRespondentID FROM Respondents WHERE Respondents.Id=IdRespondent), IdRespondent FROM resp.[Var_{0}] WHERE IdRespondent NOT IN (SELECT Id FROM Respondents WHERE IdStudy='{1}')",
                    variable.Id,
                    this.IdStudy
                ));

                // Get all respondents that have a response in the weight variable, but the value is zero.
                respondentsZero = this.Core.Respondents.ExecuteReader(string.Format(
                    "SELECT (SELECT OriginalRespondentID FROM Respondents WHERE Respondents.Id=IdRespondent), IdRespondent FROM resp.[Var_{0}] WHERE [NumericAnswer]=0",
                    variable.Id,
                    this.IdStudy
                ));
            }
            else
            {
                CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                List<object[]> _respondents = this.Core.Respondents.GetValues(
                    new string[] { "OriginalRespondentID", "Id" },
                    new string[] { "IdStudy" },
                    new object[] { this.IdStudy }
                );

                Dictionary<Guid, List<object>> responses = dataLink.Select(
                    string.Format("SELECT IdRespondent, NumericAnswer FROM [resp].[Var_{0}]", variable.Id),
                    this.Core.ClientName
                );

                respondentsMissing = new List<object[]>();
                respondentsZero = new List<object[]>();

                foreach (object[] respondent in _respondents)
                {
                    if (responses.ContainsKey((Guid)respondent[1]))
                    {
                        if ((double)(responses[(Guid)respondent[1]])[0] == 0.0)
                        {
                            respondentsZero.Add(respondent);
                        }

                        continue;
                    }

                    respondentsMissing.Add(respondent);
                }
            }

            // Run through all respondents without a response in the weight variable.
            foreach (object[] respondent in respondentsMissing)
            {
                DataAnalyserLog log = new DataAnalyserLog();
                log.Message = string.Format(
                    "The respondent with the id '{0}' hasn't got a response in the weighting variable '{1}'",
                    respondent[0],
                    variable.Name
                );

                result.Add(log);
            }

            // Run through all respondents that have a response
            // in the weight variable, but the value is zero.
            foreach (object[] respondent in respondentsZero)
            {
                DataAnalyserLog log = new DataAnalyserLog();
                log.Message = string.Format(
                    "The respondent with the id '{0}' has a weight value of zero in the weighting variable '{1}'",
                    respondent[0],
                    variable.Name
                );

                result.Add(log);
            }

            return result;
        }

        public byte[] CheckTaxonomizedData()
        {
            Data2 filter = new Data2(0);

            // Get all respondents of the study.
            List<object[]> respondents = this.Core.Respondents.GetValues(
                new string[] { "Id" },
                new string[] { "IdStudy" },
                new object[] { this.IdStudy }
            );

            // Run through all respondents of the study.
            foreach (object[] respondent in respondents)
            {
                filter.Responses.Add((Guid)respondent[0], new double[] { 0 });
            }

            string fileName = Path.GetTempFileName() + ".xlsx";

            // Create a new excel writer that builds the result document.
            ExcelWriter writer = new ExcelWriter();

            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                 this.Core,
                null
            );

            // Get all variable links of the study.
            List<object[]> variableLinks = this.Core.VariableLinks.ExecuteReader(string.Format(
                "SELECT IdVariable, IdTaxonomyVariable FROM VariableLinks WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='{0}')",
                this.IdStudy
            ), typeof(Guid), typeof(Guid));

            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, double[]>>>> categoryValues = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, double[]>>>>();
            Dictionary<string, Dictionary<string, double>> unlinkedCategories = new Dictionary<string, Dictionary<string, double>>();

            writer.Write(0, "Variable");
            writer.Write(1, "Taxonomy variable");
            writer.Write(2, "Variable value");
            writer.Write(3, "Taxonomy variable value");
            writer.Write(4, "Status");

            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 4].Interior.Color = SpreadsheetGear.Color.FromArgb(54, 94, 146);
            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 4].Font.Color = SpreadsheetGear.Color.FromArgb(255, 255, 255);

            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 1].EntireColumn.ColumnWidth = 50;
            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 1].EntireColumn.WrapText = true;

            writer.ActiveSheet.WindowInfo.FreezePanes = true;
            writer.ActiveSheet.Cells[0, 0, 0, 4].Select();

            writer.NewLine();

            int i = 0;
            // Run through all variable links of the study.
            foreach (object[] variableLink in variableLinks)
            {
                Variable variable = this.Core.Variables.GetSingle(
                    "Id",
                    variableLink[0]
                );
                TaxonomyVariable taxonomyVariable = this.Core.TaxonomyVariables.GetSingle(
                    "Id",
                    variableLink[1]
                );

                if (!categoryValues.ContainsKey(variable.Name))
                    categoryValues.Add(variable.Name, new Dictionary<string, Dictionary<string, Dictionary<string, double[]>>>());

                if (!categoryValues[variable.Name].ContainsKey(taxonomyVariable.Name))
                    categoryValues[variable.Name].Add(taxonomyVariable.Name, new Dictionary<string, Dictionary<string, double[]>>());

                XmlDocument test2 = new XmlDocument();
                test2.LoadXml("<Report><Settings><Setting Name=\"AggregateNonQAData\">True</Setting></Settings></Report>");

                ReportDefinition test = new ReportDefinition(
                    this.Core,
                    test2,
                    new Crosstables.Classes.HierarchyClasses.HierarchyFilter(null)
                );
                test.HierarchyFilter.Load();

                // Create a fake report definition variable for the study variable.
                ReportDefinitionVariable reportDefinitionVariable = new ReportDefinitionVariable(test, variable, null);

                // Create a fake report definition variable for the taxonomy variable.
                ReportDefinitionVariable reportDefinitionTaxonomyVariable = new ReportDefinitionVariable(test, taxonomyVariable, null);

                Data variableData = storageMethod.GetRespondents(reportDefinitionVariable);
                Data taxonomyVariableData = storageMethod.GetRespondents(
                    reportDefinitionTaxonomyVariable,
                    filter
                );

                writer.Write(0, variable.Name);
                writer.Write(1, taxonomyVariable.Name);
                writer.Write(2, variableData.Value.ToString());
                writer.Write(3, taxonomyVariableData.Value.ToString());

                if (variableData.Value != taxonomyVariableData.Value)
                {
                    writer.Write(4, "FAILED");
                    writer.ActiveSheet.Cells[writer.Position, 4].Interior.Color = SpreadsheetGear.Drawing.Color.FromArgb(255, 0, 0);
                }
                else
                {
                    writer.Write(4, "PASSED");
                    writer.ActiveSheet.Cells[writer.Position, 4].Interior.Color = SpreadsheetGear.Drawing.Color.FromArgb(0, 255, 0);
                }

                writer.NewLine();

                // Get all category links of the variable.
                /*List<object[]> categoryLinks = this.Core.CategoryLinks.ExecuteReader(string.Format(
                    "SELECT IdCategory, IdTaxonomyCategory FROM CategoryLinks WHERE IdVariable='{0}'",
                    variableLink[0]
                ));*/

                // Get all categories of the variable.
                List<object[]> categories = this.Core.Categories.GetValues(
                    new string[] { "Id", "Name" },
                    new string[] { "IdVariable" },
                    new object[] { variableLink[0] }
                );

                // Run through all category links of the variable.
                foreach (object[] category in categories)
                {
                    /*if ((string)category[1] == "SystemMissing")
                        continue;*/

                    // Get the category links for the category.
                    List<object[]> categoryLinks = this.Core.CategoryLinks.ExecuteReader(string.Format(
                        "SELECT IdCategory, IdTaxonomyCategory FROM CategoryLinks WHERE IdCategory='{0}'",
                        category[0]
                    ));

                    // Create a fake report definition category for the study category.
                    ReportDefinitionCategory reportDefinitionCategory = new ReportDefinitionCategory(reportDefinitionVariable, reportDefinitionVariable.XmlNode.SelectSingleNode(string.Format(
                        "*[@Id=\"{0}\"]",
                        category[0]
                    )));

                    Data categoryData = storageMethod.GetRespondents(
                        reportDefinitionCategory,
                        reportDefinitionVariable
                    );

                    if (categoryLinks.Count == 0)
                    {
                        if (!unlinkedCategories.ContainsKey(variable.Name))
                            unlinkedCategories.Add(variable.Name, new Dictionary<string, double>());

                        if (!unlinkedCategories[variable.Name].ContainsKey((string)category[1]))
                            unlinkedCategories[variable.Name].Add((string)category[1], categoryData.Value);

                        continue;
                    }

                    object[] categoryLink = categoryLinks.First();

                    XmlNode xmlNode = reportDefinitionTaxonomyVariable.XmlNode.SelectSingleNode(string.Format(
                        "*[@Id=\"{0}\"]",
                        categoryLink[1]
                    ));

                    if (xmlNode == null)
                        continue;

                    // Create a fake report definition category for the study category.
                    ReportDefinitionTaxonomyCategory reportDefinitionTaxonomyCategory = new ReportDefinitionTaxonomyCategory(reportDefinitionTaxonomyVariable, xmlNode);

                    if (!categoryValues[variable.Name][taxonomyVariable.Name].ContainsKey(reportDefinitionCategory.Name))
                        categoryValues[variable.Name][taxonomyVariable.Name].Add(reportDefinitionCategory.Name, new Dictionary<string, double[]>());

                    if (!categoryValues[variable.Name][taxonomyVariable.Name][reportDefinitionCategory.Name].ContainsKey(reportDefinitionTaxonomyCategory.Name))
                        categoryValues[variable.Name][taxonomyVariable.Name][reportDefinitionCategory.Name].Add(reportDefinitionTaxonomyCategory.Name, new double[2]);

                    Data taxonomyCategoryData = storageMethod.GetRespondents(reportDefinitionTaxonomyCategory, reportDefinitionTaxonomyVariable, filter);

                    categoryValues[variable.Name][taxonomyVariable.Name][reportDefinitionCategory.Name][reportDefinitionTaxonomyCategory.Name][0] = categoryData.Value;
                    categoryValues[variable.Name][taxonomyVariable.Name][reportDefinitionCategory.Name][reportDefinitionTaxonomyCategory.Name][1] = taxonomyCategoryData.Value;
                }

                i++;
            }

            writer.NewSheet("Categories");

            writer.Write(0, "Variable");
            writer.Write(1, "Taxonomy variable");
            writer.Write(2, "Category");
            writer.Write(3, "Taxonomy category");
            writer.Write(4, "Category value");
            writer.Write(5, "Taxonomy category value");
            writer.Write(6, "Status");

            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 6].Interior.Color = SpreadsheetGear.Color.FromArgb(54, 94, 146);
            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 6].Font.Color = SpreadsheetGear.Color.FromArgb(255, 255, 255);

            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 4].EntireColumn.ColumnWidth = 25;
            writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 4].EntireColumn.WrapText = true;

            writer.ActiveSheet.Cells[writer.Position, 6].EntireColumn.ColumnWidth = 12;

            writer.ActiveSheet.WindowInfo.FreezePanes = true;
            writer.ActiveSheet.Cells[0, 0, 0, 6].Select();

            writer.NewLine();

            // Run through all variables of the study.
            foreach (string variableName in categoryValues.Keys)
            {
                // Run through all taxonomy variables that are linked to the study variable.
                foreach (string taxonomyVariableName in categoryValues[variableName].Keys)
                {
                    // Run through all categories of the variable.
                    foreach (string categoryName in categoryValues[variableName][taxonomyVariableName].Keys)
                    {
                        // Run through all taxonomy categories that are linked to the study category.
                        foreach (string taxonomyCategoryName in categoryValues[variableName][taxonomyVariableName][categoryName].Keys)
                        {
                            double categoryData = categoryValues[variableName][taxonomyVariableName][categoryName][taxonomyCategoryName][0];
                            double taxonomyCategoryData = categoryValues[variableName][taxonomyVariableName][categoryName][taxonomyCategoryName][1];

                            writer.Write(0, variableName);
                            writer.Write(1, taxonomyVariableName);
                            writer.Write(2, categoryName);
                            writer.Write(3, taxonomyCategoryName);
                            writer.Write(4, categoryData.ToString());
                            writer.Write(5, taxonomyCategoryData.ToString());

                            if (categoryData != taxonomyCategoryData)
                            {
                                writer.Write(6, "FAILED");
                                writer.ActiveSheet.Cells[writer.Position, 6].Interior.Color = SpreadsheetGear.Drawing.Color.FromArgb(255, 0, 0);
                            }
                            else
                            {
                                writer.Write(6, "PASSED");
                                writer.ActiveSheet.Cells[writer.Position, 6].Interior.Color = SpreadsheetGear.Drawing.Color.FromArgb(0, 255, 0);
                            }

                            writer.NewLine();
                        }
                    }

                    if (unlinkedCategories.ContainsKey(variableName))
                    {
                        // Run through all unlinked categories of the variable.
                        foreach (string categoryName in unlinkedCategories[variableName].Keys)
                        {
                            writer.Write(0, variableName);
                            writer.Write(1, "");
                            writer.Write(2, categoryName);
                            writer.Write(3, "");
                            writer.Write(4, unlinkedCategories[variableName][categoryName].ToString());
                            writer.Write(5, "");

                            writer.Write(6, "NON-LINKED");
                            writer.ActiveSheet.Cells[writer.Position, 6].Interior.Color = SpreadsheetGear.Drawing.Color.FromArgb(255, 192, 0);

                            writer.NewLine();
                        }
                    }
                }
            }

            return writer.Save();
        }

        #endregion
    }
}
