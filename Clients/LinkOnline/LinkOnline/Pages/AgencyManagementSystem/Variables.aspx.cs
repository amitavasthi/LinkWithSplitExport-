using ApplicationUtilities.Classes;
using DatabaseCore.Items;
using DataCore.Classes;
using DataCore.Classes.StorageMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VariableSelector1.Classes;

namespace LinkOnline.Pages.AgencyManagementSystem
{
    public partial class Variables : WebUtilities.BasePage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the study
        /// to display the variables of.
        /// </summary>
        public Guid IdStudy { get; set; }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void BindVariables()
        {
            // Get all the variables of the study.
            List<object[]> variables = Global.Core.Variables.GetValues(
                new string[] { "Id" },
                new string[] { "IdStudy" },
                new object[] { this.IdStudy }
            );

            // Run through all variables of the study.
            foreach (object[] variable in variables)
            {
                VariableSelector variableSelector = new VariableSelector(
                    2057,
                    "Variables",
                    "Id=" + variable[0],
                    true
                );
                variableSelector.Settings.EnableCategorize = true;

                pnlVariables.Controls.Add(variableSelector);
            }
        }

        private void CreateCategorizeTextVariableAssignment()
        {
            Guid idVariable = Guid.Parse(this.Request.Params["Id"]);

            // Create a new excel writer to write the
            // result assignment excel file.
            ExcelWriter writer = new ExcelWriter();

            // Write the headline of the assignment file.
            writer.Write(0, "TextResponse");
            writer.Write(1, "CategoryName");
            writer.Write(2, "CategoryLabel");

            //writer.ActiveSheet.WindowInfo.FreezePanes = true;
            //writer.ActiveSheet.Cells[0, 0, 0, 0].Select();

            writer.ActiveSheet.Cells[0, 0].ColumnWidth = 40;
            writer.ActiveSheet.Cells[0, 1].ColumnWidth = 40;
            writer.ActiveSheet.Cells[0, 2].ColumnWidth = 40;

            writer.NewLine();

            // Create a new storage method to get the text
            // responses of the text variable to categorize.
            Database storageMethod = new Database(
                Global.Core,
                null
            );

            List<string> categories = storageMethod.GetTextAnswers(
                idVariable,
                false
            );

            // Run through all categories of the study.
            foreach (string category in categories)
            {
                // Write the text response into the first
                // column of the assignment file.
                writer.Write(0, "'" + category);

                // Write the name of the study category into
                // the second column of the assignment file.
                writer.Write(1, "'" + PrepareCategoryName(category));

                // Write the label of the study category into
                // the third column of the assignment file.
                writer.Write(2, "'" + HttpUtility.HtmlEncode(category));

                writer.NewLine();
            }

            writer.ActiveSheet.Cells[0, 0].EntireColumn.Locked = false;
            writer.ActiveSheet.Cells[0, 1].EntireColumn.Locked = false;
            writer.ActiveSheet.Cells[0, 2].EntireColumn.Locked = false;

            writer.ActiveSheet.Cells[0, 0, 0, 0].EntireColumn.Interior.Color = SpreadsheetGear.Color.FromArgb(255, 255, 204);
            writer.ActiveSheet.Cells[0, 0, 0, 0].EntireColumn.Borders.Color = SpreadsheetGear.Color.FromArgb(178, 178, 178);
            writer.ActiveSheet.Cells[0, 0, 0, 0].EntireColumn.Borders.LineStyle = SpreadsheetGear.LineStyle.Continous;

            writer.ActiveSheet.ProtectContents = true;

            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                "Exports",
                Guid.NewGuid() + ".xlsx"
            );

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            // Save the assignment file to the temp file.
            writer.Save(fileName);

            this.Response.Clear();
            this.Response.Write(string.Format(
                "/Fileadmin/Temp/Exports/{0}",
                new FileInfo(fileName).Name
            ));
            this.Response.Flush();
            this.Response.End();
        }

        private void UploadTextVariableAssignmentFile()
        {
            int idLanguage = 2057;
            Guid idVariable = Guid.Parse(this.Request.Params["Id"]);
            string variableName = this.Request.Params["VariableName"];

            FileInfo fInfo = new FileInfo(Request.Files[0].FileName);

            string fileName = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid() + fInfo.Extension
            );

            Request.Files[0].SaveAs(fileName);

            Variable variable = new Variable(Global.Core.Variables);
            variable.IdStudy = (Guid)Global.Core.Variables.GetValue("IdStudy", "Id", idVariable);
            variable.Name = variableName;
            variable.Type = VariableType.Single;

            variable.Insert();

            VariableLabel variableLabel = new VariableLabel(Global.Core.VariableLabels);
            variableLabel.IdVariable = variable.Id;
            variableLabel.IdLanguage = idLanguage;
            variableLabel.Label = (string)Global.Core.VariableLabels.GetValue(
                "Label", 
                new string[] { "IdVariable", "IdLanguage" },
                new object[] { idVariable, idLanguage }
            );
            variableLabel.ReportLabel = variableLabel.Label;

            variableLabel.Insert();

            // Read the script from the script template file.
            string script = File.ReadAllText(Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "DataStorage",
                "CreateResponses.sql"
            ));

            // Format the script with the variable's id.
            script = string.Format(
                script,
                variable.Id
            );

            // Lock the create table action.
            lock (this)
            {
                // Execute the script on the database.
                this.Core.Responses[variable.Id].ExecuteQuery(script);
            }

            ExcelReader reader = new ExcelReader(fileName);

            Database storageMethod = new Database(
                Global.Core,
                null
            );

            reader.Position = 0;

            int categoryOrder = 0;
            while (reader.Read())
            {
                string textAnswer = reader[0];
                string categoryName = reader[1];

                Data respondents = storageMethod.GetRespondentsText(
                    textAnswer,
                    idVariable,
                    false,
                    null,
                    null,
                    false
                );

                object idCategory = Global.Core.Categories.GetValue(
                    "Id",
                    new string[] { "IdVariable", "Name" },
                    new object[] { variable.Id, categoryName }
                );

                if (idCategory == null)
                {
                    Category category = new Category(Global.Core.Categories);
                    category.IdVariable = variable.Id;
                    category.Name = categoryName;
                    category.Order = categoryOrder++;

                    category.Insert();

                    idCategory = category.Id;

                    CategoryLabel categoryLabel = new CategoryLabel(Global.Core.CategoryLabels);
                    categoryLabel.IdCategory = category.Id;
                    categoryLabel.IdLanguage = idLanguage;
                    categoryLabel.Label = reader[2];

                    categoryLabel.Insert();
                }

                // Run through all respondents of the text answer.
                foreach (Guid idRespondent in respondents.Responses.Keys)
                {
                    Response response = new Response(Global.Core.Responses[variable.Id]);
                    response.IdCategory = (Guid)idCategory;
                    response.IdRespondent = idRespondent;
                    response.IdStudy = variable.IdStudy;
                    
                    // Lock the insert action.
                    lock (this)
                    {
                        response.Insert();
                    }
                }
            }

            Global.HierarchyFilters = new Crosstables.Classes.HierarchyClasses.HierarchyFilterCollection();
        }

        private string PrepareCategoryName(string name)
        {
            name = name.Replace(" ", "_");
            name = name.Replace("&", "_");
            name = name.Replace("<", "_");
            name = name.Replace(">", "_");
            name = name.Replace(",", "_");
            name = name.Replace("-", "_");

            return name;
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if a method is defined.
            if (this.Request.Params["Method"] != null)
            {
                switch (this.Request.Params["Method"])
                {
                    case "CreateCategorizeTextVariableAssignment":
                        CreateCategorizeTextVariableAssignment();
                        break;
                    case "UploadTextVariableAssignmentFile":
                        UploadTextVariableAssignmentFile();
                        break;
                }

                return;
            }

            // Check if a study id is defined.
            if (this.Request.Params["IdStudy"] == null)
                Response.Redirect("Overview.aspx");

            // Parse the id of the study to display the variables of.
            this.IdStudy = Guid.Parse(this.Request.Params["IdStudy"]);

            lblPageTitle.Text = string.Format(
                Global.LanguageManager.GetText("AgencyManagementSystemVariablesTitle"),
                Global.Core.Studies.GetValue("Name", "Id", this.IdStudy)
            );

            // Load the study's variables.
            this.BindVariables();

            boxCategorizeTextVariable.Visible = true;
        }

        #endregion
    }
}