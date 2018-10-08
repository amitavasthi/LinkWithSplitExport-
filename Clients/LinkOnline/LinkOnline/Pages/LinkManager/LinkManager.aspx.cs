using ApplicationUtilities.Classes;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.LinkManager
{
    public partial class LinkManager : WebUtilities.BasePage
    {
        #region Properties

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void ClearCache()
        {
            DataCore.Classes.StorageMethods.Database database = new DataCore.Classes.StorageMethods.Database(
                Global.Core,
                null
            );

            database.ClearCaseDataCache();

            Global.Core.ClearCache();
        }

        private void BindStudies()
        {
            if (this.IsPostBack)
                return;

            // Run through all studies.
            foreach (Study study in Global.Core.Studies.Get().OrderBy(studies => studies.Name))
            {
                // Create a new list item for the study.
                ListItem lItem = new ListItem();
                lItem.Text = study.Name;
                lItem.Value = study.Id.ToString();

                ddlUploadAugmentStudy.Items.Add(lItem);
                ddlDownloadAugmentStudy.Items.Add(lItem);
                ddlDeleteLinkingStudy.Items.Add(lItem);
                ddlUnlinkedVariables.Items.Add(lItem);
            }
        }

        private void BindStatus()
        {
            int linked = Global.Core.VariableLinks.GetCount();
            int variables = Global.Core.Variables.GetCount();

            lblLinkedVariableCount.Text = linked.ToString();
            if ((variables - linked) > 0)
            {
                lblUnlinkedVariableCount.Text = (variables - linked).ToString();
            }
            else
            {
                lblUnlinkedVariableCount.Text = "0";
            }

            int linkedC = Global.Core.CategoryLinks.GetCount();
            int variablesC = Global.Core.Categories.GetCount();

            lblLinkedCategoryCount.Text = linkedC.ToString();
            if ((variablesC - linkedC) > 0)
            {
                lblUnlinkedCategoryCount.Text = (variablesC - linkedC).ToString();
            }
            else
            {
                lblUnlinkedCategoryCount.Text = "0";
            }
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["msg"] != null)
                {
                    if (Request.QueryString["msg"].Trim() == "1")
                    {
                        base.ShowMessage(Global.LanguageManager.GetText("ExcelOnly"), WebUtilities.MessageType.Error);
                    }
                }
            }
            BindStudies();
            BindStatus();
            regularExpressionValidator1.ErrorMessage = Global.LanguageManager.GetText("ExcelOnly");

            boxUploadAugment.Visible = false;
            boxDownloadAugment.Visible = false;
            boxLinkReport.Visible = false;
            boxDeleteLinking.Visible = false;
        }

        protected void btnUploadAugmentConfirm_Click(object sender, EventArgs e)
        {
            StringBuilder errorLogBuilder = new StringBuilder();
            List<int> errorLines = new List<int>();

            Dictionary<Guid, object> addedidVariable = new Dictionary<Guid, object>();

            if (!fuUploadAugmentFile.HasFile)
                return;

            if ((Path.GetExtension(fuUploadAugmentFile.FileName) == ".xls") || (Path.GetExtension(fuUploadAugmentFile.FileName) == ".xlsx"))
            {
                Guid idStudy = Guid.Parse(
                    ddlUploadAugmentStudy.SelectedValue
                );

                // Get a temp file path to store the excel file.
                string fileName = Path.GetTempFileName() + fuUploadAugmentFile.FileName;

                // Save the file uploader's excel file to the temp file path.
                fuUploadAugmentFile.SaveAs(fileName);

                // Create a new excel reader by the temporary saved file.
                ExcelReader reader = new ExcelReader(fileName);

                Dictionary<string, List<object[]>> taxonomyVariables = Global.Core.TaxonomyVariables.ExecuteReaderDict<string>(
                    "SELECT LOWER(Name), Id FROM TaxonomyVariables",
                    new object[] { }
                );
                Dictionary<string, List<object[]>> variables = Global.Core.TaxonomyVariables.ExecuteReaderDict<string>(
                    "SELECT LOWER(Name), Id FROM Variables WHERE IdStudy={0}",
                    new object[] { idStudy }
                );
                Dictionary<Guid, List<object[]>> variableLinks = Global.Core.TaxonomyVariables.ExecuteReaderDict<Guid>(
                    "SELECT IdVariable, IdTaxonomyVariable, Id FROM VariableLinks WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy={0})",
                    new object[] { idStudy }
                );
                Dictionary<Guid, Dictionary<string, List<object[]>>> taxonomyCategories = Global.Core.TaxonomyCategories.ExecuteReaderDict<Guid, string>(
                    "SELECT IdTaxonomyVariable, LOWER(Name), Id FROM [TaxonomyCategories]",
                    new object[] { }
                );
                Dictionary<Guid, Dictionary<string, List<object[]>>> categories = Global.Core.TaxonomyCategories.ExecuteReaderDict<Guid, string>(
                    "SELECT IdVariable, LOWER(Name), Id FROM [Categories] WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy={0})",
                    new object[] { idStudy }
                );
                Dictionary<Guid, List<object[]>> categoryLinks = Global.Core.TaxonomyVariables.ExecuteReaderDict<Guid>(
                    "SELECT IdCategory, IdTaxonomyCategory, Id FROM CategoryLinks WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy={0})",
                    new object[] { idStudy }
                );

                StringBuilder queryBuilder = new StringBuilder();

                Guid idTaxonomyVariable;
                Guid idVariable;
                Guid idTaxonomyCategory;
                Guid idCategory;
                while (reader.Read())
                {
                    /*object _taxonomyVariable = Global.Core.TaxonomyVariables.GetValue("Id", "Name", reader[0]);

                    if (_taxonomyVariable == null)
                    {
                        errorLogBuilder.Append(string.Format(
                            "Taxonomy variable '{0}' wasn't found." + Environment.NewLine,
                            reader[0]
                        ));

                        errorLines.Add(reader.Position);
                        continue;
                    }

                    Guid idTaxonomyVariable = (Guid)_taxonomyVariable;*/
                    if (!taxonomyVariables.ContainsKey(reader[0].ToLower()))
                    {
                        errorLogBuilder.Append(string.Format(
                            "Taxonomy variable '{0}' wasn't found." + Environment.NewLine,
                            reader[0]
                        ));

                        errorLines.Add(reader.Position);
                        continue;
                    }
                    idTaxonomyVariable = (Guid)taxonomyVariables[reader[0].ToLower()][0][1];

                    /*object _idVariable = Global.Core.Variables.GetValue(
                        "Id",
                        new string[] { "IdStudy", "Name" },
                        new object[] { idStudy, reader[1] }
                    );

                    if (_idVariable == null)
                    {
                        errorLogBuilder.Append(string.Format(
                            "Study variable '{0}' wasn't found." + Environment.NewLine,
                            reader[1]
                        ));

                        errorLines.Add(reader.Position);
                        continue;
                    }

                    Guid idVariable = (Guid)_idVariable;*/
                    if (!variables.ContainsKey(reader[1].ToLower()))
                    {
                        errorLogBuilder.Append(string.Format(
                            "Study variable '{0}' wasn't found." + Environment.NewLine,
                            reader[1]
                        ));

                        errorLines.Add(reader.Position);
                        continue;
                    }
                    idVariable = (Guid)variables[reader[1].ToLower()][0][1];

                    /*VariableLink variableLink = Global.Core.VariableLinks.GetSingle(
                        new string[] { "IdVariable" },
                        new object[] { idVariable }
                    );

                    if (variableLink == null || (ddlUploadAugmentOverwriteExistingLinks.Checked == false && variableLink.IdTaxonomyVariable != idTaxonomyVariable))
                    {
                        variableLink = new VariableLink(Global.Core.VariableLinks);
                        variableLink.IdTaxonomyVariable = idTaxonomyVariable;
                        variableLink.IdVariable = idVariable;
                        variableLink.CreationDate = DateTime.Now;

                        variableLink.Insert();
                    }
                    else
                    {
                        variableLink.IdTaxonomyVariable = idTaxonomyVariable;
                        variableLink.IdVariable = idVariable;

                        variableLink.Save();
                    }*/



                    if (variableLinks.ContainsKey(idVariable) == false || (ddlUploadAugmentOverwriteExistingLinks.Checked == false && ((Guid)variableLinks[idVariable][0][2]) != idTaxonomyVariable))
                    {
                        if (!addedidVariable.ContainsKey(idVariable))
                        {
                            addedidVariable.Add(idVariable, null);
                            queryBuilder.Append(string.Format(
                                "INSERT INTO [VariableLinks] (Id, IdVariable, IdTaxonomyVariable, CreationDate) VALUES (NEWID(), '{0}', '{1}', '{2}')",
                                idVariable,
                                idTaxonomyVariable,
                                DateTime.Now.ToString("yyyy/MM/dd HH:mm")
                            ));
                        }
                    }
                    else
                    {
                        queryBuilder.Append(string.Format(
                            "UPDATE [VariableLinks] SET IdTaxonomyVariable='{1}' WHERE Id='{0}';",
                            variableLinks[idVariable][0][2],
                            idTaxonomyVariable
                        ));
                    }

                    /*object _taxonomyCategory = Global.Core.TaxonomyCategories.GetValue(
                        "Id",
                        new string[] { "IdTaxonomyVariable", "Name" },
                        new object[] { idTaxonomyVariable, reader[2] }
                    );

                    if (_taxonomyCategory == null)
                    {
                        errorLogBuilder.Append(string.Format(
                            "Taxonomy category '{0}' wasn't found." + Environment.NewLine,
                            reader[2]
                        ));

                        errorLines.Add(reader.Position);
                        continue;
                    }

                    Guid idTaxonomyCategory = (Guid)_taxonomyCategory;*/
                    if (reader[2] == "")
                        continue;

                    if (taxonomyCategories.ContainsKey(idTaxonomyVariable) == false ||
                        taxonomyCategories[idTaxonomyVariable].ContainsKey(reader[2].ToLower()) == false)
                    {
                        errorLogBuilder.Append(string.Format(
                            "Taxonomy category '{0}' wasn't found." + Environment.NewLine,
                            reader[2]
                        ));

                        errorLines.Add(reader.Position);
                        continue;
                    }
                    idTaxonomyCategory = (Guid)taxonomyCategories[idTaxonomyVariable][reader[2].ToLower()][0][2];

                    /*object _idCategory = Global.Core.Categories.GetValue(
                        "Id",
                        new string[] { "IdVariable", "Name" },
                        new object[] { idVariable, reader[3] }
                    );

                    if (_idCategory == null)
                    {
                        errorLogBuilder.Append(string.Format(
                            "Study category '{0}' wasn't found." + Environment.NewLine,
                            reader[3]
                        ));

                        errorLines.Add(reader.Position);
                        continue;
                    }

                    Guid idCategory = (Guid)_idCategory;*/
                    if (categories.ContainsKey(idVariable) == false ||
                        categories[idVariable].ContainsKey(reader[3].ToLower()) == false)
                    {
                        errorLogBuilder.Append(string.Format(
                            "Study category '{0}' wasn't found." + Environment.NewLine,
                            reader[3]
                        ));

                        errorLines.Add(reader.Position);
                        continue;
                    }
                    idCategory = (Guid)categories[idVariable][reader[3].ToLower()][0][2];

                    /*CategoryLink categoryLink = Global.Core.CategoryLinks.GetSingle(
                        "IdCategory",
                        idCategory
                    );

                    if (categoryLink == null || (ddlUploadAugmentOverwriteExistingLinks.Checked == false && categoryLink.IdTaxonomyCategory != idTaxonomyCategory))
                    {
                        // Create a new category link object.
                        categoryLink = new CategoryLink(Global.Core.CategoryLinks);

                        categoryLink.IdTaxonomyCategory = idTaxonomyCategory;
                        categoryLink.IdCategory = idCategory;
                        categoryLink.IdVariable = variableLink.IdVariable;
                        categoryLink.IdTaxonomyVariable = variableLink.IdTaxonomyVariable;
                        categoryLink.CreationDate = DateTime.Now;

                        categoryLink.Insert();
                    }
                    else
                    {
                        categoryLink.IdTaxonomyCategory = idTaxonomyCategory;
                        categoryLink.IdCategory = idCategory;
                        categoryLink.IdVariable = variableLink.IdVariable;
                        categoryLink.IdTaxonomyVariable = variableLink.IdTaxonomyVariable;

                        categoryLink.Save();
                    }*/
                    if (categoryLinks.ContainsKey(idCategory) == false || (ddlUploadAugmentOverwriteExistingLinks.Checked == false && ((Guid)categoryLinks[idCategory][0][1]) != idTaxonomyCategory))
                    {
                        queryBuilder.Append(string.Format(
                            "INSERT INTO [CategoryLinks] (Id, IdCategory, IdTaxonomyCategory, CreationDate, IdVariable, IdTaxonomyVariable) " +
                            "VALUES (NEWID(), '{0}', '{1}', '{2}', '{3}', '{4}')",
                            idCategory,
                            idTaxonomyCategory,
                            DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                            idVariable,
                            idTaxonomyVariable
                        ));
                    }
                    else
                    {
                        queryBuilder.Append(string.Format(
                            "UPDATE [CategoryLinks] SET IdTaxonomyCategory='{0}', IdVariable='{1}', IdTaxonomyVariable='{2}' WHERE Id='{3}'",
                            idTaxonomyCategory,
                            idVariable,
                            idTaxonomyVariable,
                            categoryLinks[idCategory][0][2]
                        ));
                    }
                }

                if (queryBuilder.Length != 0)
                    Global.Core.CategoryLinks.ExecuteQuery(queryBuilder.ToString());

                // Delete the temporary file.
                File.Delete(fileName);

                if (errorLines.Count > 0)
                {
                    string errorLogDirectory = Path.Combine(
                        Request.PhysicalApplicationPath,
                        "Fileadmin",
                        "Temp",
                        "Logs"
                    );

                    if (!Directory.Exists(errorLogDirectory))
                        Directory.CreateDirectory(errorLogDirectory);

                    string errorLogFileName = Guid.NewGuid() + ".txt";

                    File.WriteAllText(Path.Combine(
                        errorLogDirectory,
                        errorLogFileName
                    ), errorLogBuilder.ToString());

                    string errorLogFileUrl = Request.Url.ToString().Replace("Pages/LinkManager/LinkManager.aspx", "") + "Fileadmin/Temp/Logs/" + errorLogFileName;

                    errorLogBuilder.Clear();

                    string augmentFileName = Guid.NewGuid() + ".xls";

                    string augmentFileUrl = Request.Url.ToString().Replace("Pages/LinkManager/LinkManager.aspx", "") + "Fileadmin/Temp/Logs/" + augmentFileName;

                    ExcelWriter writer = new ExcelWriter();

                    foreach (int position in errorLines)
                    {
                        writer.Write(
                            0,
                            reader[position, 0]
                        );

                        writer.Write(
                            1,
                            reader[position, 1]
                        );

                        writer.Write(
                            2,
                            reader[position, 2]
                        );

                        writer.Write(
                            3,
                            reader[position, 3]
                        );

                        writer.NewLine();
                    }

                    writer.Save(Path.Combine(
                        Request.PhysicalApplicationPath,
                        "Fileadmin",
                        "Temp",
                        "Logs",
                        augmentFileName
                    ));

                    lblLinkReportErrorCountValue.Text = errorLines.Count.ToString();
                    lblLinkReportErrorLogFileValue.Text = "<a target=\"_blank\" href=\"" + errorLogFileUrl + "\">" + errorLogFileName + "</a>";
                    lblLinkReportAugmentValue.Text = "<a target=\"_blank\" href=\"" + augmentFileUrl + "\">" + augmentFileName + "</a>";

                    boxUploadAugment.Visible = false;
                    boxLinkReport.Visible = true;
                }
                else
                {
                    Global.ClientManager.IncreaseCaseDataVersion(Global.Core.ClientName);
                    Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(Global.Core.ClientName).CaseDataVersion;

                    ClearCache();

                    Response.Redirect(
                        Request.Url.ToString()
                    );
                }
            }
            else
            {
                Global.ClientManager.IncreaseCaseDataVersion(Global.Core.ClientName);
                Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(Global.Core.ClientName).CaseDataVersion;

                ClearCache();
                Response.Redirect(Request.Url.ToString() + "?msg=1", false);
            }
            Global.ClientManager.IncreaseCaseDataVersion(Global.Core.ClientName);
            Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(Global.Core.ClientName).CaseDataVersion;

            ClearCache();
        }

        protected void btnUploadAugment_Click(object sender, EventArgs e)
        {
            boxUploadAugment.Visible = true;
        }


        protected void btnDownloadAugmentConfirm_Click(object sender, EventArgs e)
        {
            if (ddlDownloadAugmentStudy.SelectedValue != null)
            {
                Guid idStudy = Guid.Parse(
                    ddlDownloadAugmentStudy.SelectedValue
                );

                List<object[]> getDetails = Global.Core.Categories.ExecuteReader(
                    "SELECT TaxonomyVariables.Name,Variables.Name,TaxonomyCategories.Name,Categories.Name " +
                        "FROM Variables " +
                        "INNER JOIN Categories ON Categories.IdVariable = Variables.Id " +
                        "INNER JOIN VariableLinks ON Variables.Id = VariableLinks.IdVariable " +
                        "INNER JOIN TaxonomyVariables ON TaxonomyVariables.Id = VariableLinks.idTaxonomyVariable " +
                        "INNER JOIN CategoryLinks ON Categories.Id = CategoryLinks.IdCategory " +
                        "INNER JOIN TaxonomyCategories ON TaxonomyCategories.Id = CategoryLinks.IdTaxonomyCategory " +
                        "WHERE Variables.IdStudy={0}",
                    new object[] { idStudy }
                );
                if (getDetails.Count > 0)
                {

                    // Get a temp file path to store the excel file.
                    //string fileName = Path.GetTempFileName() + fuUploadAugmentFile.FileName;

                    string fileName = Path.Combine(
                        Request.PhysicalApplicationPath,
                        "Fileadmin",
                        "Temp",
                        "Exports",
                        HttpContext.Current.Session.SessionID,
                        Global.Core.Studies.GetValue("Name", "Id", idStudy) + ".xlsx"
                    );

                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    }

                    // Save the file uploader's excel file to the temp file path.
                    fuUploadAugmentFile.SaveAs(fileName);

                    //// Run through all variables of the study.
                    //foreach (object[] variable in Global.Core.Variables.GetValues(
                    //    new string[] { "Id", "Name" },
                    //    new string[] { "IdStudy" },
                    //    new object[] { idStudy }
                    //))
                    //{
                    //    object _idTaxonomyVariable = Global.Core.VariableLinks.GetValue("IdTaxonomyVariable", "IdVariable", variable[0]);

                    //    if (_idTaxonomyVariable == null)
                    //        continue;

                    //    Guid idTaxonomyVariable = (Guid)_idTaxonomyVariable;

                    //    string taxonomyVariableName = (string)Global.Core.TaxonomyVariables.GetValue("Name", "Id", idTaxonomyVariable);

                    //    // Run through all categories of the variable.
                    //    foreach (object[] category in Global.Core.Categories.GetValues(
                    //        new string[] { "Id", "Name" },
                    //        new string[] { "IdVariable" },
                    //        new object[] { variable[0] }
                    //    ))
                    //    {
                    //        object _idTaxonomyCategory = Global.Core.CategoryLinks.GetValue("IdTaxonomyCategory", "IdCategory", category[0]);

                    //        if (_idTaxonomyCategory == null)
                    //            continue;

                    //        Guid idTaxonomyCategory = (Guid)_idTaxonomyCategory;

                    //        string taxonomyCategoryName = (string)Global.Core.TaxonomyCategories.GetValue("Name", "Id", idTaxonomyCategory);

                    //        writer.Write(0, taxonomyVariableName);
                    //        writer.Write(1, (string)variable[1]);
                    //        writer.Write(2, taxonomyCategoryName);
                    //        writer.Write(3, (string)category[1]);

                    //        writer.NewLine();
                    //    }
                    //}

                    // Create a new excel reader by the temporary saved file.
                    ExcelWriter writer = new ExcelWriter();
                    writer.Write(0, "TaxonomyVariable Name");
                    writer.Write(1, "Variable Name");
                    writer.Write(2, "TaxonomyCategory Name");
                    writer.Write(3, "Category Name");

                    writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 3].Interior.Color = SpreadsheetGear.Color.FromArgb(54, 94, 146);
                    writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 3].Font.Color = SpreadsheetGear.Color.FromArgb(255, 255, 255);

                    writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 1].EntireColumn.ColumnWidth = 50;
                    writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 1].EntireColumn.WrapText = true;

                    writer.ActiveSheet.WindowInfo.FreezePanes = true;
                    writer.ActiveSheet.Cells[0, 0, 0, 3].Select();

                    writer.NewLine();

                    foreach (object[] detail in getDetails)
                    {
                        writer.Write(0, (string)detail[0]);
                        writer.Write(1, (string)detail[1]);
                        writer.Write(2, (string)detail[2]);
                        writer.Write(3, (string)detail[3]);
                        writer.NewLine();
                    }

                    writer.Save(fileName);

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "DownloadAction", string.Format(
                      "window.open('/Fileadmin/Temp/Exports/{0}/{1}');",
                      HttpContext.Current.Session.SessionID,
                      Global.Core.Studies.GetValue("Name", "Id", idStudy) + ".xlsx"
                  ), true);

                    boxDownLoadUnlinked.Visible = false;

                    //base.WriteFileToResponse(fileName, ddlDownloadAugmentStudy.SelectedItem.Text + ".xls", "application/msexcel", true);
                }
                else
                {
                    boxDownLoadUnlinked.Visible = false;
                    base.ShowMessage(string.Format(Global.LanguageManager.GetText("NoLinkedVariables"),
                        Global.LanguageManager.GetText("NoLinkedVariables")),
                        WebUtilities.MessageType.Warning);

                }
            }
            ClearCache();

            // base.WriteFileToResponse(fileName, "Test.xls", "application/msexcel", true);
        }

        protected void btnDownloadAugment_Click(object sender, EventArgs e)
        {
            boxDownloadAugment.Visible = true;
        }


        protected void btnReset_Click(object sender, EventArgs e)
        {
            cbReset.Visible = true;

            cbReset.Text = string.Format(
                Global.LanguageManager.GetText("ResetLinkingConfirmMessage")
            );

            cbReset.Confirm = delegate ()
            {
                Global.Core.CategoryLinks.ExecuteQuery("DELETE FROM [CategoryLinks];");
                Global.Core.VariableLinks.ExecuteQuery("DELETE FROM [VariableLinks];");

                cbReset.Visible = false;

                ClearCache();

                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.ToString());
            };
        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            boxDeleteLinking.Visible = true;
        }

        protected void btnDeleteLinkingConfirm_Click(object sender, EventArgs e)
        {
            Guid idStudy;

            // Get the id of the selected study to delete.
            if (!Guid.TryParse(ddlDeleteLinkingStudy.SelectedValue, out idStudy))
                return;

            // Delete all the category links of the study's variables.
            Global.Core.CategoryLinks.ExecuteQuery("DELETE FROM [CategoryLinks] WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='" + idStudy + "');");

            // Delete all the variable links of the study's variables.
            Global.Core.VariableLinks.ExecuteQuery("DELETE FROM [VariableLinks] WHERE IdVariable IN (SELECT Id FROM Variables WHERE IdStudy='" + idStudy + "');");

            ClearCache();

            // Self redirect to display updated values.
            Response.Redirect(Request.Url.ToString());
        }

        //protected void btnManualLinking_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect("ManualLinkManager.aspx");
        //}

        #endregion

        //protected void btnAutoLinkIng_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect("LinkingModule.aspx");
        //}

        protected void btnAutoLink_Click(object sender, EventArgs e)
        {
            Response.Redirect("AutoLinking.aspx");
        }

        protected void btnDownloadUnLinkedVariables_Click(object sender, EventArgs e)
        {
            boxDownLoadUnlinked.Visible = true;
        }
        protected void btnDownloadUnLinkedVariablesConfirm_Click(object sender, EventArgs e)
        {

            if (ddlDownloadAugmentStudy.SelectedValue != null)
            {
                Guid idStudy = Guid.Parse(
                    ddlUnlinkedVariables.SelectedValue
                );

                // Get a temp file path to store the excel file.
                //string fileName = Path.GetTempFileName() + fuUploadAugmentFile.FileName;
                string fileName = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "Temp",
                    "Exports",
                    HttpContext.Current.Session.SessionID,
                    Global.Core.Studies.GetValue("Name", "Id", idStudy) + "_unlinked.xlsx"
                );

                if (File.Exists(fileName))
                    File.Delete(fileName);

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                // Save the file uploader's excel file to the temp file path.
                fuUploadAugmentFile.SaveAs(fileName);

                // Create a new excel reader by the temporary saved file.
                ExcelWriter writer = new ExcelWriter();

                writer.Write(0, "Variable Label");
                writer.Write(1, "Variable Name");
                writer.Write(2, "Category Label");
                writer.Write(3, "Category Name");

                writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 3].Interior.Color = SpreadsheetGear.Color.FromArgb(54, 94, 146);
                writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 3].Font.Color = SpreadsheetGear.Color.FromArgb(255, 255, 255);

                writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 1].EntireColumn.ColumnWidth = 50;
                writer.ActiveSheet.Cells[writer.Position, 0, writer.Position, 1].EntireColumn.WrapText = true;

                writer.ActiveSheet.WindowInfo.FreezePanes = true;
                writer.ActiveSheet.Cells[0, 0, 0, 3].Select();

                writer.NewLine();

                List<object[]> c = Global.Core.Categories.ExecuteReader(
                        "SELECT Variables.Name, VariableLabels.Label, Categories.Name, CategoryLabels.Label FROM Categories " +
                        "INNER JOIN Categorylabels ON Categories.Id=CategoryLabels.IdCategory " +
                        "INNER JOIN Variables ON Categories.IdVariable=Variables.Id " +
                        "INNER JOIN VariableLabels ON Variables.Id=VariableLabels.IdVariable " +
                        "WHERE Variables.IdStudy={0}",
                        new object[] { idStudy }
                    );

                foreach (object[] category in c)
                {
                    writer.Write(0, (string)category[0]);
                    writer.Write(1, (string)category[1]);
                    writer.Write(2, (string)category[2]);
                    writer.Write(3, (string)category[3]);
                    writer.NewLine();
                }

                /*foreach (object[] variable in Global.Core.Variables.ExecuteReader("SELECT V.Id,V.Name,VL.IdVariable,VL.Label FROM Variables  V INNER JOIN VariableLabels VL ON VL.IdVariable = V.id WHERE V.Id NOT IN (SELECT  IdVariable FROM VariableLinks) AND IdStudy={0}",
                     new object[] { idStudy }))
                {
                    List<object[]> categories = Global.Core.Categories.ExecuteReader(
                        "SELECT Categories.Id, Categories.Name, CategoryLabels.Label FROM "+
                        "Categories INNER JOIN Categorylabels ON Categories.Id=CategoryLabels.IdCategory "+
                        "WHERE IdVariable={0}",
                        new object[] { variable[0] }
                    );

                    // Run through all categories of the variable.
                    foreach (object[] category in categories)
                    {
                        if ((string)category[1] == "SystemMissing")
                            continue;

                        if (String.IsNullOrEmpty((string)category[2]))
                            continue;

                        writer.Write(0, (string)variable[3]);
                        writer.Write(1, (string)variable[1]);
                        writer.Write(2, (string)category[2]);
                        writer.Write(3, (string)category[1]);
                        writer.NewLine();
                    }
                }*/

                writer.Save(fileName);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "DownloadAction", string.Format(
                    "window.open('/Fileadmin/Temp/Exports/{0}/{1}');",
                    HttpContext.Current.Session.SessionID,
                    Global.Core.Studies.GetValue("Name", "Id", idStudy) + "_unlinked.xlsx"
                ), true);

                boxDownLoadUnlinked.Visible = false;

                //base.WriteFileToResponse(fileName, ddlDownloadAugmentStudy.SelectedItem + "_Unlinked.xls", "application/msexcel", true);
            }

        }

        protected void btnDownloadAugmentCancel_Click(object sender, EventArgs e)
        {
            boxDownloadAugment.Visible = false;
        }
        protected void btnDeleteLinkingCancel_Click(object sender, EventArgs e)
        {
            boxDeleteLinking.Visible = false;
        }
        protected void btnDownloadUnLinkedVariablesCancel_Click(object sender, EventArgs e)
        {
            boxDownLoadUnlinked.Visible = false;
        }
        protected void btnUploadAugmentCancel_Click(object sender, EventArgs e)
        {
            boxUploadAugment.Visible = false;
        }
    }
}