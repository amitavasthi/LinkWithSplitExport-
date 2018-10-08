using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using WebUtilities;
using WebUtilities.Controls;
using DatabaseCore.Items;
using System.Web.UI.HtmlControls;
using System.Configuration;
using LinkOnline.Classes;
using System.IO;
using ApplicationUtilities.Classes;
using VariableSelector1.Classes;
using System.Globalization;
using System.Text;

namespace LinkOnline.Pages.TaxonomyManager
{
    public partial class TaxonomyManager : BasePage
    {
        #region Properties

        TreeView treeView;

        #endregion


        #region Constructor

        public TaxonomyManager()
            : base(true, true)
        {

        }

        #endregion


        #region Methods

        private void BindHierarchies()
        {
            // Get all hierarchies on root level.
            List<object[]> hierarchies = Global.Core.Hierarchies.GetValues(
                new string[] { "Id", "Name" },
                new string[] { "IdHierarchy" },
                new object[] { null }
            );

            treeView = new TreeView("tvHierarchies");

            // Run through all hierarchies on root level.
            foreach (object[] hierarchy in hierarchies)
            {
                TreeViewNode node = RenderHierarchyTreeViewNode(
                    treeView,
                    hierarchy,
                    ""
                );
                node.Attributes.Add("id", "tnHierarchy" + hierarchy[0]);

                treeView.Nodes.Add(node);

                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "LoadHierarchyVariables" + hierarchy[0],
                    "LoadVariables(document.getElementById('cphContent__tvnHierarchy" + hierarchy[0] + "'), '" + hierarchy[0] + "');",
                    true
                );
            }

            pnlHierarchies.Controls.Add(treeView);
        }

        private TreeViewNode RenderHierarchyTreeViewNode(
            TreeView treeView, 
            object[] hierarchy, 
            string path
        )
        {
            path += "/" + (string)hierarchy[1];

            // Create a new tree view node for
            // the hierarchy on root level.
            TreeViewNode node = new TreeViewNode(treeView, "tvnHierarchy" + hierarchy[0].ToString(), "");
            node.CssClass = "BackgroundColor5";
            node.Label = (string)hierarchy[1];

            node.Attributes.Add(
                "Path",
                path
            );
            node.OnClientClick = string.Format(
                "LoadVariables(this, '{0}');",
                hierarchy[0]
            );

            // Get all hierarchies of the workgroups where the user
            // is assigned to where the hierarchy is the parent.
            List<object[]> childHierarchies = Global.Core.Hierarchies.ExecuteReader(string.Format(
                "SELECT Id, Name FROM [Hierarchies] WHERE IdHierarchy='{1}' AND Id IN (SELECT IdHierarchy FROM WorkgroupHierarchies " +
                "WHERE IdWorkgroup IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser='{0}'))",
                Global.IdUser.Value,
                hierarchy[0]
            ));

            // Run through all available child hierarchies of the hierarchy.
            foreach (object[] childHierarchy in childHierarchies)
            {
                node.AddChild(RenderHierarchyTreeViewNode(
                    treeView,
                    childHierarchy,
                    path
                ));
            }

            return node;
        }


        private void BindLanguages()
        {
            // Get all available languages.
            CultureInfo[] languages = Global.GetCultures();

            // Run through all languages.
            foreach (CultureInfo language in languages)
            {
                // Create a new list item for the language.
                System.Web.UI.WebControls.ListItem lItem = new System.Web.UI.WebControls.ListItem();
                lItem.Text = language.DisplayName;
                lItem.Value = language.LCID.ToString();

                ddlUploadLanguage.Items.Add(lItem);
            }

            ddlUploadLanguage.SelectedValue = "2057";
        }

        private void BindVariables(Guid idHierarchy)
        {
            // Get all taxonomy chapters of the client.
            List<object[]> taxonomyChapters = Global.Core.TaxonomyChapters.GetValues(
                new string[] { "Id" },
                new string[] { },
                new object[] { },
                "Name"
            );

            Panel pnlContainer = new Panel();

            List<object[]> taxonomyVariables;

            // Run through all taxonomy chapters.
            foreach (object[] taxonomyChapter in taxonomyChapters)
            {
                Panel pnlTaxonomyChapter = new Panel();
                pnlTaxonomyChapter.CssClass = "TaxonomyChapterTitle";

                HtmlGenericControl lblTaxonomyChapterTitle = new HtmlGenericControl("div");
                lblTaxonomyChapterTitle.Attributes.Add("class", "TaxonomyChapterTitleLabel");
                lblTaxonomyChapterTitle.InnerText = (string)Global.Core.TaxonomyChapterLabels.GetValue(
                    "Label",
                    new string[] { "IdTaxonomyChapter", "IdLanguage" },
                    new object[] { taxonomyChapter[0], 2057 }
                );

                lblTaxonomyChapterTitle.Attributes.Add("onclick", string.Format(
                    "HideTaxonomyChapter(this, '{0}');",
                    taxonomyChapter[0]
                ));

                pnlTaxonomyChapter.Controls.Add(lblTaxonomyChapterTitle);

                Panel pnlTaxonomyChapterVariables = new Panel();
                pnlTaxonomyChapterVariables.CssClass = "TaxonomyChapterVariables PnlVariableSelectors";
                pnlTaxonomyChapterVariables.Attributes.Add("id", "TaxonomyChapterVariables" + taxonomyChapter[0]);

                // Get all taxonomy variables of the chapter.
                /*taxonomyVariables = Global.Core.TaxonomyVariables.GetValues(
                    new string[] { "Id" },
                    new string[] { "IdTaxonomyChapter" },
                    new object[] { (Guid)taxonomyChapter[0] }
                );*/
                /*Commented for Sorting functionality by Label*/
                //taxonomyVariables = Global.Core.TaxonomyVariables.ExecuteReader(
                //    "SELECT Id FROM TaxonomyVariables WHERE IdTaxonomyChapter={0} AND Id IN " +
                //    "(SELECT IdTaxonomyVariable FROM TaxonomyVariableHierarchies WHERE IdHierarchy={1}) ORDER BY Name",
                //    new object[] { taxonomyChapter[0], idHierarchy }
                //);

                taxonomyVariables = Global.Core.TaxonomyVariables.ExecuteReader(
                    "SELECT TV.ID FROM TaxonomyVariables TV INNER JOIN TaxonomyVariableLabels TVL ON TVL.IdTaxonomyVariable = TV.ID " +
                    "WHERE TV.IdTaxonomyChapter = {0} AND TV.Id IN" +
                    "(SELECT IdTaxonomyVariable FROM TaxonomyVariableHierarchies WHERE IdHierarchy = {1}) ORDER BY TVL.Label",
                    new object[] { taxonomyChapter[0], idHierarchy }
                );

                if (taxonomyVariables.Count == 0)
                    continue;

                // Run through all taxonomy variables.
                foreach (object[] taxonomyVariable in taxonomyVariables)
                {
                    VariableSelector1.Classes.VariableSelector variableSelector = new VariableSelector1.Classes.VariableSelector(
                        2057,
                        "TaxonomyVariables",
                        "Id=" + taxonomyVariable[0].ToString()
                    );
                    //variableSelector.EnableCombine = false;

                    variableSelector.Render();
                    pnlTaxonomyChapterVariables.Controls.Add(variableSelector);
                }

                pnlTaxonomyChapter.Controls.Add(pnlTaxonomyChapterVariables);

                pnlContainer.Controls.Add(pnlTaxonomyChapter);
            }

            // Get all taxonomy variables without a chapter.
            taxonomyVariables = Global.Core.TaxonomyVariables.GetValues(
                new string[] { "Id" },
                new string[] { "IdTaxonomyChapter" },
                new object[] { DBNull.Value }
            );

            // Run through all taxonomy variables.
            foreach (object[] taxonomyVariable in taxonomyVariables)
            {
                VariableSelector1.Classes.VariableSelector variableSelector = new VariableSelector1.Classes.VariableSelector(
                    2057,
                    "TaxonomyVariables",
                    "Id=" + taxonomyVariable[0].ToString()
                );
                //variableSelector.EnableCombine = false;
                variableSelector.Variable.Values.Add("Id", taxonomyVariable[0]);

                variableSelector.Render();
                pnlContainer.Controls.Add(variableSelector);
            }

            Response.Write(pnlContainer.ToHtml());
        }



        private string PrepareLabel(string label)
        {
            string result = label;

            result = result.Replace("↵", "");
            result = result.Replace("\n", "");
            result = result.Replace("\r", "");
            result = result.Replace("\t", "");
            result = result.Trim();

            return result;
        }


        private Guid[] GetHierarchies(string path)
        {
            List<Guid> result = new List<Guid>();
            //Guid? result = null;

            string[] hierarchyPaths = path.Split(',');

            foreach (string hierarchyPath in hierarchyPaths)
            {
                string[] parts = hierarchyPath.Split('/');

                Guid? idHierarchy = null;
                for (int i = 0; i < parts.Length; i++)
                {
                    Hierarchy hierarchy = Global.Core.Hierarchies.GetSingle(
                        new string[] { "Name", "IdHierarchy" },
                        new object[] { parts[i], (idHierarchy.HasValue ? (object)idHierarchy.Value : null) }
                    );

                    if (hierarchy == null)
                    {
                        hierarchy = new Hierarchy(Global.Core.Hierarchies);

                        hierarchy.Name = parts[i];
                        hierarchy.IdHierarchy = idHierarchy;
                        hierarchy.SetValue("CreationDate", DateTime.Now);

                        hierarchy.Insert();

                        WorkgroupHierarchy workgroupHierarchy = new WorkgroupHierarchy(Global.Core.WorkgroupHierarchies);
                        workgroupHierarchy.IdHierarchy = hierarchy.Id;
                        workgroupHierarchy.IdWorkgroup = (Guid)Global.Core.UserWorkgroups.GetValue(
                            "IdWorkgroup",
                            "IdUser",
                            Global.IdUser.Value
                        );

                        workgroupHierarchy.Insert();
                    }

                    idHierarchy = hierarchy.Id;
                }

                if (idHierarchy.HasValue)
                    result.Add(idHierarchy.Value);
            }

            return result.ToArray();
        }

        private void CreateTaxonomyByExcel(string source, int language)
        {
            ExcelReader reader = new ExcelReader(source);

            int order = 0;
            // Read all the taxonomy variables.
            while (reader.Read())
            {
                // FOR TEST ONLY
                if (reader[3] == "")
                    continue;

                Guid[] hierarchies = GetHierarchies(reader[5]);

                TaxonomyChapter taxonomyChapter;

                taxonomyChapter = Global.Core.TaxonomyChapters.GetSingle(
                     new string[] { "Name" },
                     new object[] { reader[0] }
                );

                if (taxonomyChapter == null)
                {
                    taxonomyChapter = new TaxonomyChapter(
                        Global.Core.TaxonomyChapters
                    );

                    taxonomyChapter.Name = reader[0];
                    taxonomyChapter.SetValue("CreationDate", DateTime.Now);

                    taxonomyChapter.Insert();
                }

                TaxonomyChapterLabel taxonomyChapterLabel = Global.Core.TaxonomyChapterLabels.GetSingle(
                    new string[] { "IdTaxonomyChapter", "IdLanguage" },
                    new object[] { taxonomyChapter.Id, language }
                );

                if (taxonomyChapterLabel == null)
                {
                    taxonomyChapterLabel = new TaxonomyChapterLabel(
                        Global.Core.TaxonomyChapterLabels
                    );

                    taxonomyChapterLabel.IdTaxonomyChapter = taxonomyChapter.Id;
                    taxonomyChapterLabel.IdLanguage = language;
                    taxonomyChapterLabel.SetValue("CreationDate", DateTime.Now);
                    taxonomyChapterLabel.Label = reader[0];

                    taxonomyChapterLabel.Insert();
                }
                else
                {
                    taxonomyChapterLabel.Label = reader[0];

                    taxonomyChapterLabel.Save();
                }

                //TaxonomyVariable taxonomyVariable = new TaxonomyVariable(Global.Core.TaxonomyVariables);
                TaxonomyVariable taxonomyVariable = Global.Core.TaxonomyVariables.GetSingle("Name", reader[2]);

                VariableType variableType = VariableType.Single;

                switch (reader[1].ToUpper())
                {
                    case "NUMBER":
                        variableType = VariableType.Numeric;
                        break;
                    case "MULTI":
                        variableType = VariableType.Multi;
                        break;
                    case "TEXT":
                        variableType = VariableType.Text;
                        break;
                }

                if (taxonomyVariable == null)
                {
                    taxonomyVariable = new TaxonomyVariable(
                        Global.Core.TaxonomyVariables
                    );

                    taxonomyVariable.Name = reader[2];
                    taxonomyVariable.SetValue("CreationDate", DateTime.Now);
                    taxonomyVariable.Type = variableType;

                    taxonomyVariable.Insert();
                }

                taxonomyVariable.IdTaxonomyChapter = taxonomyChapter.Id;
                taxonomyVariable.Order = order++;
                taxonomyVariable.Type = variableType;
                taxonomyVariable.Save();

                Global.Core.TaxonomyVariableHierarchies.ExecuteQuery(string.Format(
                    "DELETE FROM TaxonomyVariableHierarchies WHERE IdTaxonomyVariable='{0}'",
                    taxonomyVariable.Id
                ));

                foreach (Guid idHierarchy in hierarchies)
                {
                    TaxonomyVariableHierarchy taxonomyVariableHierarchy = new TaxonomyVariableHierarchy(Global.Core.TaxonomyVariableHierarchies);
                    taxonomyVariableHierarchy.IdHierarchy = idHierarchy;
                    taxonomyVariableHierarchy.IdTaxonomyVariable = taxonomyVariable.Id;

                    taxonomyVariableHierarchy.Insert();
                }

                // Get the additional parameters for the variable.
                string[] additionalParameters = reader[4].Split(',');

                taxonomyVariable.Scale = false;
                taxonomyVariable.Weight = false;

                // Run through all additional parameters for the variable.
                foreach (string additionalParameter in additionalParameters)
                {
                    if (additionalParameter.Trim() == "")
                        continue;

                    TaxonomyVariableAdditionalParameter parameter;

                    if (Enum.TryParse(
                        additionalParameter,
                        true,
                        out parameter
                    ))
                    {
                        switch (parameter)
                        {
                            case TaxonomyVariableAdditionalParameter.Scale:
                                taxonomyVariable.Scale = true;

                                break;
                            case TaxonomyVariableAdditionalParameter.Weight:
                                taxonomyVariable.Weight = true;

                                break;
                        }
                    }
                }

                taxonomyVariable.Save();

                //TaxonomyVariableLabel taxonomyVariableLabel = new
                //    TaxonomyVariableLabel(Global.Core.TaxonomyVariableLabels);
                TaxonomyVariableLabel taxonomyVariableLabel = Global.Core.TaxonomyVariableLabels.GetSingle(
                    new string[] { "IdTaxonomyVariable", "IdLanguage" },
                    new object[] { taxonomyVariable.Id, language }
                );

                if (taxonomyVariableLabel == null)
                {
                    taxonomyVariableLabel = new TaxonomyVariableLabel(Global.Core.TaxonomyVariableLabels);

                    taxonomyVariableLabel.IdTaxonomyVariable = taxonomyVariable.Id;
                    taxonomyVariableLabel.SetValue("CreationDate", DateTime.Now);
                    taxonomyVariableLabel.IdLanguage = language;
                    taxonomyVariableLabel.Label = PrepareLabel(reader[3]);

                    taxonomyVariableLabel.Insert();
                }
                else
                {
                    taxonomyVariableLabel.Label = PrepareLabel(reader[3]);

                    taxonomyVariableLabel.Save();
                }
            }

            // Set the reader's active worksheet to the second worksheet.
            reader.ActiveSheet = reader.Workbook.Worksheets[1];

            // Reset the reader's position.
            reader.Position = 0;

            Dictionary<object, int> categoryOrder = new Dictionary<object, int>();

            // Read all the taxonomy categories.
            while (reader.Read())
            {
                // FOR TEST ONLY
                if (reader[3] == "")
                    continue;

                object idTaxonomyVariable = Global.Core.TaxonomyVariables.GetValue(
                    "Id",
                    "Name",
                    reader[0]
                );

                if (idTaxonomyVariable == null)
                    continue;

                if (!categoryOrder.ContainsKey(idTaxonomyVariable))
                    categoryOrder.Add(idTaxonomyVariable, 0);

                order = Global.Core.TaxonomyCategories.GetCount(
                    "IdTaxonomyVariable",
                    idTaxonomyVariable
                );

                //TaxonomyCategory taxonomyCategory = new TaxonomyCategory(Global.Core.TaxonomyCategories);
                TaxonomyCategory taxonomyCategory = Global.Core.TaxonomyCategories.GetSingle(
                    new string[] { "IdTaxonomyVariable", "Name" },
                    new object[] { idTaxonomyVariable, PrepareLabel(reader[2]) }
                );

                if (taxonomyCategory == null)
                {
                    taxonomyCategory = new TaxonomyCategory(Global.Core.TaxonomyCategories);

                    taxonomyCategory.SetValue("CreationDate", DateTime.Now);
                    taxonomyCategory.IdTaxonomyVariable = (Guid)idTaxonomyVariable;
                    taxonomyCategory.Name = PrepareLabel(reader[2]);

                    taxonomyCategory.Insert();
                }

                // Get the additional parameters for the variable.
                string[] additionalParameters = reader[4].Split(',');

                taxonomyCategory.Enabled = true;

                Guid[] hierarchies = GetHierarchies(reader[5]);

                Global.Core.TaxonomyVariableHierarchies.ExecuteQuery(string.Format(
                    "DELETE FROM TaxonomyCategoryHierarchies WHERE IdTaxonomyCategory='{0}'",
                    taxonomyCategory.Id
                ));

                foreach (Guid idHierarchy in hierarchies)
                {
                    TaxonomyCategoryHierarchy categoryHierarchy = Global.Core.TaxonomyCategoryHierarchies.GetSingle(
                        new string[] { "IdTaxonomyCategory", "IdHierarchy" },
                        new object[] { taxonomyCategory.Id, idHierarchy }
                    );

                    if (categoryHierarchy != null)
                        continue;

                    categoryHierarchy = new TaxonomyCategoryHierarchy(Global.Core.TaxonomyCategoryHierarchies);
                    categoryHierarchy.IdTaxonomyCategory = taxonomyCategory.Id;
                    categoryHierarchy.IdHierarchy = idHierarchy;

                    categoryHierarchy.Insert();
                }

                // Run through all additional parameters for the variable.
                foreach (string additionalParameter in additionalParameters)
                {
                    if (additionalParameter.Trim() == "")
                        continue;

                    TaxonomyVariableAdditionalParameter parameter;

                    if (Enum.TryParse(
                        additionalParameter,
                        true,
                        out parameter
                    ))
                    {
                        switch (parameter)
                        {
                            case TaxonomyVariableAdditionalParameter.Hidden:
                                taxonomyCategory.Enabled = false;

                                break;
                            case TaxonomyVariableAdditionalParameter.DELETE:

                                Global.Core.TaxonomyVariableHierarchies.ExecuteQuery(string.Format(
                                    "DELETE FROM TaxonomyCategoryHierarchies WHERE IdTaxonomyCategory='{0}'",
                                    taxonomyCategory.Id
                                ));

                                Global.Core.TaxonomyCategoryLabels.ExecuteQuery(string.Format(
                                    "DELETE FROM TaxonomyCategoryLabels WHERE IdTaxonomyCategory='{0}'",
                                    taxonomyCategory.Id
                                ));

                                Global.Core.CategoryLinks.ExecuteQuery(string.Format(
                                    "DELETE FROM CategoryLinks WHERE IdTaxonomyCategory='{0}'",
                                    taxonomyCategory.Id
                                ));

                                Global.Core.CategoryLinks.ExecuteQuery(string.Format(
                                    "DELETE FROM TaxonomyCategories WHERE Id='{0}'",
                                    taxonomyCategory.Id
                                ));

                                taxonomyCategory = null;

                                break;
                        }
                    }
                }

                if (taxonomyCategory == null)
                    continue;

                taxonomyCategory.Save();

                //int value = 0;
                //int.TryParse(reader[1], out value);

                double value = 0;
                double.TryParse(reader[1], out value);

                taxonomyCategory.SetValue("Value", value);
                taxonomyCategory.Order = categoryOrder[idTaxonomyVariable]++;

                taxonomyCategory.Save();

                //TaxonomyCategoryLabel taxonomyCategoryLabel = new TaxonomyCategoryLabel(Global.Core.TaxonomyCategoryLabels);
                TaxonomyCategoryLabel taxonomyCategoryLabel = Global.Core.TaxonomyCategoryLabels.GetSingle(
                    new string[] { "IdTaxonomyCategory", "IdLanguage" },
                    new object[] { taxonomyCategory.Id, language }
                );

                if (taxonomyCategoryLabel == null)
                {
                    taxonomyCategoryLabel = new TaxonomyCategoryLabel(Global.Core.TaxonomyCategoryLabels);

                    taxonomyCategoryLabel.SetValue("CreationDate", DateTime.Now);
                    taxonomyCategoryLabel.IdLanguage = language;
                    taxonomyCategoryLabel.IdTaxonomyCategory = taxonomyCategory.Id;
                    taxonomyCategoryLabel.Label = PrepareLabel(reader[3]);

                    taxonomyCategoryLabel.Insert();
                }
                else
                {
                    taxonomyCategoryLabel.Label = PrepareLabel(reader[3]);

                    taxonomyCategoryLabel.Save();
                }
            }
        }

        private void CreateTaxonomyByExcel(string source, int language, Guid idHierarchy)
        {
            ExcelReader reader = new ExcelReader(source);

            StringBuilder insertBuilder1 = new StringBuilder();
            StringBuilder insertBuilder2 = new StringBuilder();
            StringBuilder insertBuilder3 = new StringBuilder();
            StringBuilder insertBuilder4 = new StringBuilder();

            Dictionary<Guid, List<object[]>> taxonomyVariableHierarchies = Global.Core.TaxonomyVariableHierarchies.ExecuteReaderDict<Guid>(
                "SELECT IdTaxonomyVariable, Id FROM [TaxonomyVariableHierarchies]",
                new object[] { }
            );
            Dictionary<Guid, List<object[]>> taxonomyCategoryHierarchies = Global.Core.TaxonomyVariableHierarchies.ExecuteReaderDict<Guid>(
                "SELECT IdTaxonomyCategory, Id FROM [TaxonomyCategoryHierarchies]",
                new object[] { }
            );
            Dictionary<Guid, List<object[]>> taxonomyVariableLabels = Global.Core.TaxonomyVariableHierarchies.ExecuteReaderDict<Guid>(
                "SELECT IdTaxonomyVariable, Id FROM [TaxonomyVariableLabels] WHERE IdLanguage={0}",
                new object[] { language }
            );
            Dictionary<Guid, List<object[]>> taxonomyCategoryLabels = Global.Core.TaxonomyVariableHierarchies.ExecuteReaderDict<Guid>(
                "SELECT IdTaxonomyCategory, Id FROM [TaxonomyCategoryLabels] WHERE IdLanguage={0}",
                new object[] { language }
            );

            int order = 0;
            // Read all the taxonomy variables.
            while (reader.Read())
            {
                // FOR TEST ONLY
                if (reader[3] == "")
                    continue;

                TaxonomyChapter taxonomyChapter;

                taxonomyChapter = Global.Core.TaxonomyChapters.GetSingle(
                     new string[] { "Name" },
                     new object[] { reader[0] }
                );

                if (taxonomyChapter == null)
                {
                    taxonomyChapter = new TaxonomyChapter(
                        Global.Core.TaxonomyChapters
                    );

                    taxonomyChapter.Name = reader[0];
                    taxonomyChapter.SetValue("CreationDate", DateTime.Now);

                    taxonomyChapter.Insert();
                    //insertBuilder.Append(;
                }

                TaxonomyChapterLabel taxonomyChapterLabel = Global.Core.TaxonomyChapterLabels.GetSingle(
                    new string[] { "IdTaxonomyChapter", "IdLanguage" },
                    new object[] { taxonomyChapter.Id, language }
                );

                if (taxonomyChapterLabel == null)
                {
                    taxonomyChapterLabel = new TaxonomyChapterLabel(
                        Global.Core.TaxonomyChapterLabels
                    );

                    taxonomyChapterLabel.IdTaxonomyChapter = taxonomyChapter.Id;
                    taxonomyChapterLabel.IdLanguage = language;
                    taxonomyChapterLabel.SetValue("CreationDate", DateTime.Now);
                    taxonomyChapterLabel.Label = reader[0];

                    taxonomyChapterLabel.Insert();
                    //insertBuilder.Append(taxonomyChapterLabel.RenderInsertQuery());
                }
                else
                {
                    taxonomyChapterLabel.Label = reader[0];

                    taxonomyChapterLabel.Save();
                    //insertBuilder.Append(taxonomyChapterLabel.RenderSaveQuery());
                }

                //TaxonomyVariable taxonomyVariable = new TaxonomyVariable(Global.Core.TaxonomyVariables);
                TaxonomyVariable taxonomyVariable = Global.Core.TaxonomyVariables.GetSingle("Name", reader[2]);

                VariableType variableType = VariableType.Single;

                switch (reader[1].ToUpper())
                {
                    case "NUMBER":
                        variableType = VariableType.Numeric;
                        break;
                    case "MULTI":
                        variableType = VariableType.Multi;
                        break;
                    case "TEXT":
                        variableType = VariableType.Text;
                        break;
                }

                if (taxonomyVariable == null)
                {
                    taxonomyVariable = new TaxonomyVariable(
                        Global.Core.TaxonomyVariables
                    );

                    taxonomyVariable.Name = reader[2];
                    taxonomyVariable.SetValue("CreationDate", DateTime.Now);
                    taxonomyVariable.Type = variableType;

                    //taxonomyVariable.Insert();
                    insertBuilder1.Append(taxonomyVariable.RenderInsertQuery());
                }

                taxonomyVariable.IdTaxonomyChapter = taxonomyChapter.Id;
                taxonomyVariable.Order = order++;
                taxonomyVariable.Type = variableType;
                //taxonomyVariable.Save();
                //insertBuilder1.Append(taxonomyVariable.RenderSaveQuery());

                /*TaxonomyVariableHierarchy taxonomyVariableHierarchy = Global.Core.TaxonomyVariableHierarchies.GetSingle(
                    "IdTaxonomyVariable",
                    taxonomyVariable.Id
                );*/

                //if (taxonomyVariableHierarchy == null)
                //{
                //    taxonomyVariableHierarchy = new TaxonomyVariableHierarchy(Global.Core.TaxonomyVariableHierarchies);
                //    taxonomyVariableHierarchy.IdHierarchy = idHierarchy;
                //    taxonomyVariableHierarchy.IdTaxonomyVariable = taxonomyVariable.Id;

                //    //taxonomyVariableHierarchy.Insert();
                //    insertBuilder2.Append(taxonomyVariableHierarchy.RenderInsertQuery());
                //}
                //else
                //{
                //    taxonomyVariableHierarchy.IdHierarchy = idHierarchy;
                //    //taxonomyVariableHierarchy.Save();
                //    insertBuilder2.Append(taxonomyVariableHierarchy.RenderSaveQuery());
                //}
                if (!taxonomyVariableHierarchies.ContainsKey(taxonomyVariable.Id))
                {
                    TaxonomyVariableHierarchy taxonomyVariableHierarchy = new TaxonomyVariableHierarchy(Global.Core.TaxonomyVariableHierarchies);
                    taxonomyVariableHierarchy.IdHierarchy = idHierarchy;
                    taxonomyVariableHierarchy.IdTaxonomyVariable = taxonomyVariable.Id;

                    insertBuilder2.Append(taxonomyVariableHierarchy.RenderInsertQuery());
                }
                else
                {
                    insertBuilder2.Append(string.Format(
                        "UPDATE [TaxonomyVariableHierarchies] SET IdHierarchy='{0}' WHERE IdTaxonomyVariable='{1}';",
                        idHierarchy,
                        taxonomyVariable.Id
                    ));
                }

                // Get the additional parameters for the variable.
                string[] additionalParameters = reader[4].Split(',');

                taxonomyVariable.Scale = false;
                taxonomyVariable.Weight = false;

                // Run through all additional parameters for the variable.
                foreach (string additionalParameter in additionalParameters)
                {
                    if (additionalParameter.Trim() == "")
                        continue;

                    TaxonomyVariableAdditionalParameter parameter;

                    if (Enum.TryParse(
                        additionalParameter,
                        true,
                        out parameter
                    ))
                    {
                        switch (parameter)
                        {
                            case TaxonomyVariableAdditionalParameter.Scale:
                                taxonomyVariable.Scale = true;

                                break;
                            case TaxonomyVariableAdditionalParameter.Weight:
                                taxonomyVariable.Weight = true;

                                break;
                        }
                    }
                }

                //taxonomyVariable.Save();
                insertBuilder2.Append(taxonomyVariable.RenderSaveQuery());

                //TaxonomyVariableLabel taxonomyVariableLabel = Global.Core.TaxonomyVariableLabels.GetSingle(
                //    new string[] { "IdTaxonomyVariable", "IdLanguage" },
                //    new object[] { taxonomyVariable.Id, language }
                //);

                //if (taxonomyVariableLabel == null)
                //{
                //    taxonomyVariableLabel = new TaxonomyVariableLabel(Global.Core.TaxonomyVariableLabels);

                //    taxonomyVariableLabel.IdTaxonomyVariable = taxonomyVariable.Id;
                //    taxonomyVariableLabel.SetValue("CreationDate", DateTime.Now);
                //    taxonomyVariableLabel.IdLanguage = language;
                //    taxonomyVariableLabel.Label = PrepareLabel(reader[3]);

                //    //taxonomyVariableLabel.Insert();
                //    insertBuilder2.Append(taxonomyVariableLabel.RenderInsertQuery());
                //}
                //else
                //{
                //    taxonomyVariableLabel.Label = PrepareLabel(reader[3]);

                //    taxonomyVariableLabel.Save();
                //    insertBuilder2.Append(taxonomyVariableLabel.RenderSaveQuery());
                //}
                if (!taxonomyVariableLabels.ContainsKey(taxonomyVariable.Id))
                {
                    TaxonomyVariableLabel taxonomyVariableLabel = new TaxonomyVariableLabel(Global.Core.TaxonomyVariableLabels);

                    taxonomyVariableLabel.IdTaxonomyVariable = taxonomyVariable.Id;
                    taxonomyVariableLabel.SetValue("CreationDate", DateTime.Now);
                    taxonomyVariableLabel.IdLanguage = language;
                    taxonomyVariableLabel.Label = PrepareLabel(reader[3]);

                    insertBuilder2.Append(taxonomyVariableLabel.RenderInsertQuery());
                }
                else
                {
                    insertBuilder2.Append(string.Format(
                        "UPDATE [TaxonomyVariableLabels] SET Label='{0}' WHERE Id='{1}';",
                        System.Security.SecurityElement.Escape(PrepareLabel(reader[3])),
                        taxonomyVariableLabels[taxonomyVariable.Id][0][1]
                    ));
                }
            }

            Global.Core.TaxonomyVariables.ExecuteQuery(insertBuilder1.ToString());
            Global.Core.TaxonomyVariables.ExecuteQuery(insertBuilder2.ToString());

            insertBuilder1.Clear();
            insertBuilder2.Clear();

            Dictionary<string, List<object[]>> taxonomyVariables = Global.Core.TaxonomyVariableHierarchies.ExecuteReaderDict<string>(
                "SELECT Name, Id FROM [TaxonomyVariables]",
                new object[] { }
            );

            Dictionary<Guid, List<object[]>> taxonomyCategoryOrders = Global.Core.TaxonomyVariableHierarchies.ExecuteReaderDict<Guid>(
                "SELECT IdTaxonomyVariable, Count(*) FROM [TaxonomyCategories] GROUP BY IdTaxonomyVariable",
                new object[] { }
            );

            // Set the reader's active worksheet to the second worksheet.
            reader.ActiveSheet = reader.Workbook.Worksheets[1];

            // Reset the reader's position.
            reader.Position = 0;

            Dictionary<object, int> categoryOrder = new Dictionary<object, int>();

            // Read all the taxonomy categories.
            while (reader.Read())
            {
                // FOR TEST ONLY
                if (reader[3] == "")
                    continue;

                /*object idTaxonomyVariable = Global.Core.TaxonomyVariables.GetValue(
                    "Id",
                    "Name",
                    reader[0]
                );*/

                if (!taxonomyVariables.ContainsKey(reader[0]))
                    continue;

                object idTaxonomyVariable = taxonomyVariables[reader[0]][0][1];

                if (!categoryOrder.ContainsKey(idTaxonomyVariable))
                    categoryOrder.Add(idTaxonomyVariable, 0);

                /*order = Global.Core.TaxonomyCategories.GetCount(
                    "IdTaxonomyVariable",
                    idTaxonomyVariable
                );*/
                if (taxonomyCategoryOrders.ContainsKey((Guid)idTaxonomyVariable))
                    order = (int)taxonomyCategoryOrders[(Guid)idTaxonomyVariable][0][1];
                else
                    order = 0;

                //TaxonomyCategory taxonomyCategory = new TaxonomyCategory(Global.Core.TaxonomyCategories);
                TaxonomyCategory taxonomyCategory = Global.Core.TaxonomyCategories.GetSingle(
                    new string[] { "IdTaxonomyVariable", "Name" },
                    new object[] { idTaxonomyVariable, PrepareLabel(reader[2]) }
                );

                if (taxonomyCategory == null)
                {
                    taxonomyCategory = new TaxonomyCategory(Global.Core.TaxonomyCategories);

                    taxonomyCategory.SetValue("CreationDate", DateTime.Now);
                    taxonomyCategory.IdTaxonomyVariable = (Guid)idTaxonomyVariable;
                    taxonomyCategory.Name = PrepareLabel(reader[2]);

                    //taxonomyCategory.Insert();
                    insertBuilder2.Append(taxonomyCategory.RenderInsertQuery());
                }

                // Get the additional parameters for the variable.
                string[] additionalParameters = reader[4].Split(',');

                taxonomyCategory.Enabled = true;

                //TaxonomyCategoryHierarchy taxonomyCategoryHierarchy = Global.Core.TaxonomyCategoryHierarchies.GetSingle(
                //    "IdTaxonomyCategory",
                //    taxonomyCategory.Id
                //);

                //if (taxonomyCategoryHierarchy == null)
                //{
                //    taxonomyCategoryHierarchy = new TaxonomyCategoryHierarchy(Global.Core.TaxonomyCategoryHierarchies);
                //    taxonomyCategoryHierarchy.IdHierarchy = idHierarchy;
                //    taxonomyCategoryHierarchy.IdTaxonomyCategory = taxonomyCategory.Id;

                //    //taxonomyCategoryHierarchy.Insert();
                //    insertBuilder3.Append(taxonomyCategoryHierarchy.RenderInsertQuery());
                //}
                //else
                //{
                //    taxonomyCategoryHierarchy.IdHierarchy = idHierarchy;
                //    //taxonomyCategoryHierarchy.Save();
                //    insertBuilder3.Append(taxonomyCategoryHierarchy.RenderSaveQuery());
                //}
                if (!taxonomyCategoryHierarchies.ContainsKey(taxonomyCategory.Id))
                {
                    TaxonomyCategoryHierarchy taxonomyCategoryHierarchy = new TaxonomyCategoryHierarchy(Global.Core.TaxonomyCategoryHierarchies);
                    taxonomyCategoryHierarchy.IdHierarchy = idHierarchy;
                    taxonomyCategoryHierarchy.IdTaxonomyCategory = taxonomyCategory.Id;

                    insertBuilder4.Append(taxonomyCategoryHierarchy.RenderInsertQuery());
                }
                else
                {
                    insertBuilder2.Append(string.Format(
                        "UPDATE [TaxonomyCategoryHierarchies] SET IdHierarchy='{0}' WHERE IdTaxonomyCategory='{1}';",
                        idHierarchy,
                        taxonomyCategory.Id
                    ));
                }

                // Run through all additional parameters for the variable.
                foreach (string additionalParameter in additionalParameters)
                {
                    if (additionalParameter.Trim() == "")
                        continue;

                    TaxonomyVariableAdditionalParameter parameter;

                    if (Enum.TryParse(
                        additionalParameter,
                        true,
                        out parameter
                    ))
                    {
                        switch (parameter)
                        {
                            case TaxonomyVariableAdditionalParameter.Hidden:
                                taxonomyCategory.Enabled = false;

                                break;
                            case TaxonomyVariableAdditionalParameter.DELETE:

                                Global.Core.TaxonomyCategoryLabels.ExecuteQuery(string.Format(
                                    "DELETE FROM TaxonomyCategoryLabels WHERE IdTaxonomyCategory='{0}'",
                                    taxonomyCategory.Id
                                ));

                                Global.Core.CategoryLinks.ExecuteQuery(string.Format(
                                    "DELETE FROM CategoryLinks WHERE IdTaxonomyCategory='{0}'",
                                    taxonomyCategory.Id
                                ));

                                Global.Core.CategoryLinks.ExecuteQuery(string.Format(
                                    "DELETE FROM TaxonomyCategories WHERE Id='{0}'",
                                    taxonomyCategory.Id
                                ));

                                taxonomyCategory = null;

                                break;
                        }
                    }
                }

                if (taxonomyCategory == null)
                    continue;

                //int value = 0;
                //int.TryParse(reader[1], out value);

                double value = 0;
                double.TryParse(reader[1], out value);

                taxonomyCategory.SetValue("Value", value);
                taxonomyCategory.Order = categoryOrder[idTaxonomyVariable]++;

                //taxonomyCategory.Save();
                insertBuilder3.Append(taxonomyCategory.RenderSaveQuery());

                //TaxonomyCategoryLabel taxonomyCategoryLabel = Global.Core.TaxonomyCategoryLabels.GetSingle(
                //    new string[] { "IdTaxonomyCategory", "IdLanguage" },
                //    new object[] { taxonomyCategory.Id, language }
                //);

                //if (taxonomyCategoryLabel == null)
                //{
                //    taxonomyCategoryLabel = new TaxonomyCategoryLabel(Global.Core.TaxonomyCategoryLabels);

                //    taxonomyCategoryLabel.SetValue("CreationDate", DateTime.Now);
                //    taxonomyCategoryLabel.IdLanguage = language;
                //    taxonomyCategoryLabel.IdTaxonomyCategory = taxonomyCategory.Id;
                //    taxonomyCategoryLabel.Label = PrepareLabel(reader[3]);

                //    //taxonomyCategoryLabel.Insert();
                //    insertBuilder3.Append(taxonomyCategoryLabel.RenderInsertQuery());
                //}
                //else
                //{
                //    taxonomyCategoryLabel.Label = PrepareLabel(reader[3]);

                //    //taxonomyCategoryLabel.Save();
                //    insertBuilder3.Append(taxonomyCategoryLabel.RenderSaveQuery());
                //}
                if (!taxonomyCategoryLabels.ContainsKey(taxonomyCategory.Id))
                {
                    TaxonomyCategoryLabel taxonomyCategoryLabel = new TaxonomyCategoryLabel(Global.Core.TaxonomyCategoryLabels);

                    taxonomyCategoryLabel.SetValue("CreationDate", DateTime.Now);
                    taxonomyCategoryLabel.IdLanguage = language;
                    taxonomyCategoryLabel.IdTaxonomyCategory = taxonomyCategory.Id;
                    taxonomyCategoryLabel.Label = PrepareLabel(reader[3]);
                    
                    insertBuilder3.Append(taxonomyCategoryLabel.RenderInsertQuery());
                }
                else
                {
                    insertBuilder2.Append(string.Format(
                        "UPDATE [TaxonomyCategoryLabels] SET Label='{0}' WHERE Id='{1}';",
                        System.Security.SecurityElement.Escape(PrepareLabel(reader[3])),
                        (Guid)taxonomyCategoryLabels[taxonomyCategory.Id][0][1]
                    ));
                }
            }


            Global.Core.TaxonomyVariables.ExecuteQuery(insertBuilder1.ToString());
            Global.Core.TaxonomyVariables.ExecuteQuery(insertBuilder2.ToString());
            Global.Core.TaxonomyVariables.ExecuteQuery(insertBuilder3.ToString());
            Global.Core.TaxonomyVariables.ExecuteQuery(insertBuilder4.ToString());

            insertBuilder1.Clear();
            insertBuilder2.Clear();
            insertBuilder3.Clear();
            insertBuilder4.Clear();
        }


        private void ResetHierarchyTaxonomy()
        {
            // Parse the id of the hierarchy from
            // the http request's parameters.
            Guid idHierarchy = Guid.Parse(
                Request.Params["IdHierarchy"]
            );

            // Delete all category links.
            Global.Core.Categories.ExecuteQuery(string.Format(
                "DELETE FROM CategoryLinks WHERE IdTaxonomyVariable IN (SELECT Id FROM TaxonomyVariables WHERE IdTaxonomyChapter IN (SELECT Id FROM TaxonomyChapters WHERE IdHierarchy='{0}'))",
                idHierarchy
            ));

            // Delete all variable links.
            Global.Core.Categories.ExecuteQuery(string.Format(
                "DELETE FROM VariableLinks WHERE IdTaxonomyVariable IN (SELECT Id FROM TaxonomyVariables WHERE IdTaxonomyChapter IN (SELECT Id FROM TaxonomyChapters WHERE IdHierarchy='{0}'))",
                idHierarchy
            ));

            // Delete all taxonomy category hierarchies.
            Global.Core.Categories.ExecuteQuery(string.Format(
                "DELETE FROM TaxonomyCategoryHierarchies WHERE IdTaxonomyCategory IN (SELECT Id FROM TaxonomyCategories WHERE IdTaxonomyVariable IN (SELECT Id FROM TaxonomyVariables WHERE IdTaxonomyChapter IN (SELECT Id FROM TaxonomyChapters WHERE IdHierarchy='{0}')))",
                idHierarchy
            ));

            // Delete all taxonomy category labels.
            Global.Core.Categories.ExecuteQuery(string.Format(
                "DELETE FROM TaxonomyCategoryLabels WHERE IdTaxonomyCategory IN (SELECT Id FROM TaxonomyCategories WHERE IdTaxonomyVariable IN (SELECT Id FROM TaxonomyVariables WHERE IdTaxonomyChapter IN (SELECT Id FROM TaxonomyChapters WHERE IdHierarchy='{0}')))",
                idHierarchy
            ));

            // Delete all taxonomy categories.
            Global.Core.Categories.ExecuteQuery(string.Format(
                "DELETE FROM TaxonomyCategories WHERE IdTaxonomyVariable IN (SELECT Id FROM TaxonomyVariables WHERE IdTaxonomyChapter IN (SELECT Id FROM TaxonomyChapters WHERE IdHierarchy='{0}'))",
                idHierarchy
            ));

            // Delete all taxonomy variable hierarchies.
            Global.Core.Categories.ExecuteQuery(string.Format(
                "DELETE FROM TaxonomyVariableHierarchies WHERE IdTaxonomyVariable IN (SELECT Id FROM TaxonomyVariables WHERE IdTaxonomyChapter IN (SELECT Id FROM TaxonomyChapters WHERE IdHierarchy='{0}'))",
                idHierarchy
            ));

            // Delete all taxonomy variable labels.
            Global.Core.Categories.ExecuteQuery(string.Format(
                "DELETE FROM TaxonomyVariableLabels WHERE IdTaxonomyVariable IN (SELECT Id FROM TaxonomyVariables WHERE IdTaxonomyChapter IN (SELECT Id FROM TaxonomyChapters WHERE IdHierarchy='{0}'))",
                idHierarchy
            ));

            // Delete all taxonomy variables.
            Global.Core.Categories.ExecuteQuery(string.Format(
                "DELETE FROM TaxonomyVariables WHERE IdTaxonomyChapter IN (SELECT Id FROM TaxonomyChapters WHERE IdHierarchy='{0}')",
                idHierarchy
            ));

            // Delete all taxonomy chapters.
            Global.Core.Categories.ExecuteQuery(string.Format(
                "DELETE FROM TaxonomyChapterLabels WHERE IdTaxonomyChapter IN (SELECT Id FROM TaxonomyChapters WHERE IdHierarchy='{0}')",
                idHierarchy
            ));

            // Delete all taxonomy chapters.
            Global.Core.Categories.ExecuteQuery(string.Format(
                "DELETE FROM TaxonomyChapters WHERE IdHierarchy='{0}'",
                idHierarchy
            ));
        }

        private string BuildHierarchyPath(Guid idHierarchy)
        {
            object[] hierarchy = Global.Core.Hierarchies.GetValues(
                new string[] { "Name", "IdHierarchy" },
                new string[] { "Id" },
                new object[] { idHierarchy }
            )[0];

            if (hierarchy[1] != null)
                return BuildHierarchyPath((Guid)hierarchy[1]) + "/" + (string)hierarchy[0];
            else
                return (string)hierarchy[0];
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["Method"] == "GetVariables")
            {
                Response.Clear();
                BindVariables(Guid.Parse(Request.Params["IdHierarchy"]));
                Response.End();

                return;
            }
            else if (Request.Params["Method"] == "ResetHierarchyTaxonomy")
            {
                Response.Clear();
                ResetHierarchyTaxonomy();
                Response.End();

                return;
            }

            btnUpload.Value = Global.LanguageManager.GetText("UploadTaxonomy");
            boxUpload.Visible = true;

            BindHierarchies();
            //BindVariables();

            if (!this.IsPostBack)
                BindLanguages();
        }


        protected void btnDownload_Click(object sender, EventArgs e)
        {
            MetadataExport export = new MetadataExport();
            
            string fileName = export.Render();

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "DownloadTaxonomy",
                "window.open('/Fileadmin/Temp/" +
                HttpContext.Current.Session.SessionID + "/" +
                new FileInfo(fileName).Name + "');",
                true
            );
            return;

            //ExcelWriter writer = new ExcelWriter();

            //writer.Write(0, "Chapter");
            //writer.Write(1, "Variable type");
            //writer.Write(2, "Variable name");
            //writer.Write(3, "Variable label");
            //writer.Write(4, "Additional (SCALE, ...)");
            //writer.Write(5, "Hierarchy");

            //writer.NewLine();

            //// Get all taxonomy variables.
            //List<object[]> taxonomyVariables = Global.Core.TaxonomyVariables.GetValues(
            //    new string[] { "Id", "Name", "IdTaxonomyChapter", "Type", "Scale" },
            //    new string[] { },
            //    new object[] { }
            //);

            //// Run through all taxonomy variables.
            //foreach (object[] taxonomyVariable in taxonomyVariables)
            //{
            //    if (!Global.HierarchyFilters.Default.TaxonomyVariables.ContainsKey((Guid)taxonomyVariable[0]))
            //        continue;

            //    writer.Write(0, (string)Global.Core.TaxonomyChapterLabels.GetValue(
            //        "Label",
            //        "IdTaxonomyChapter",
            //        taxonomyVariable[2]
            //    ));

            //    writer.Write(1, ((VariableType)taxonomyVariable[3]).ToString());
            //    writer.Write(2, (string)taxonomyVariable[1]);
            //    writer.Write(3, (string)Global.Core.TaxonomyVariableLabels.GetValue(
            //        "Label",
            //        "IdTaxonomyVariable",
            //        taxonomyVariable[0]
            //    ));

            //    List<string> hierarchyStrings = new List<string>();

            //    List<object[]> hierarchies = Global.Core.TaxonomyVariableHierarchies.GetValues(
            //        new string[] { "IdHierarchy" },
            //        new string[] { "IdTaxonomyVariable" },
            //        new object[] { taxonomyVariable[0] }
            //    );

            //    foreach (object[] hierarchy in hierarchies)
            //    {
            //        hierarchyStrings.Add(BuildHierarchyPath((Guid)hierarchy[0]));
            //    }

            //    writer.Write(4, ((bool)taxonomyVariable[4]) ? "SCALE" : "");
            //    writer.Write(5, string.Join(",", hierarchyStrings.ToArray()));

            //    writer.NewLine();
            //}

            //writer.NewSheet("Categories");

            //writer.Write(0, "Variable");
            //writer.Write(1, "Value");
            //writer.Write(2, "Category name");
            //writer.Write(3, "Category label");
            //writer.Write(4, "Additional (HIDDEN, ...)");
            //writer.Write(5, "Hierarchy");

            //writer.NewLine();


            //// Run through all taxonomy variables.
            //foreach (object[] taxonomyVariable in taxonomyVariables)
            //{
            //    if (!Global.HierarchyFilters.Default.TaxonomyVariables.ContainsKey((Guid)taxonomyVariable[0]))
            //        continue;

            //    List<object[]> taxonomyCategories = Global.Core.TaxonomyCategories.GetValues(
            //        new string[] { "Id", "Name", "Value", "Enabled" },
            //        new string[] { "IdTaxonomyVariable" },
            //        new object[] { taxonomyVariable[0] }
            //    );

            //    foreach (object[] taxonomyCategory in taxonomyCategories)
            //    {
            //        if (!Global.HierarchyFilters.Default.TaxonomyCategories.ContainsKey((Guid)taxonomyCategory[0]))
            //            continue;

            //        writer.Write(0, (string)taxonomyVariable[1]);
            //        writer.Write(1, ((int)taxonomyCategory[2]).ToString());
            //        writer.Write(2, (string)taxonomyCategory[1]);

            //        writer.Write(3, (string)Global.Core.TaxonomyCategoryLabels.GetValue(
            //            "Label",
            //            "IdTaxonomyCategory",
            //            taxonomyCategory[0]
            //        ));

            //        writer.Write(4, ((bool)taxonomyCategory[3]) ? "" : "HIDDEN");

            //        List<string> hierarchyStrings = new List<string>();

            //        List<object[]> hierarchies = Global.Core.TaxonomyCategoryHierarchies.GetValues(
            //            new string[] { "IdHierarchy" },
            //            new string[] { "IdTaxonomyCategory" },
            //            new object[] { taxonomyCategory[0] }
            //        );

            //        foreach (object[] hierarchy in hierarchies)
            //        {
            //            hierarchyStrings.Add(BuildHierarchyPath((Guid)hierarchy[0]));
            //        }

            //        writer.Write(5, string.Join(",", hierarchyStrings.ToArray()));

            //        writer.NewLine();
            //    }
            //}

            //string fileName = Path.Combine(
            //    Request.PhysicalApplicationPath,
            //    "Fileadmin",
            //    "Temp",
            //    "Exports",
            //    HttpContext.Current.Session.SessionID,
            //    "Taxonomy.xlsx"
            //);

            //if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            //    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            //if (File.Exists(fileName))
            //    File.Delete(fileName);

            //writer.Save(fileName);

            //Page.ClientScript.RegisterStartupScript(
            //    this.GetType(),
            //    "DownloadTaxonomy",
            //    "window.open('/Fileadmin/Temp/Exports/" + HttpContext.Current.Session.SessionID + "/Taxonomy.xlsx');",
            //    true
            //);
        }

        protected void btnUploadConfirm_Click(object sender, EventArgs e)
        {
            if (!fuUploadFile.HasFile)
                return;

            // Get a temp file path to store the excel file.
            string fileName = Path.GetTempFileName() + fuUploadFile.FileName;

            // Save the file uploader's excel file to the temp file path.
            fuUploadFile.SaveAs(fileName);

            // Get the selected upload language.
            int language = int.Parse(ddlUploadLanguage.SelectedValue);

            if (chkUploadHierarchySpecificUpload.Checked)
            {
                Guid idHierarchy = Guid.Parse(
                    Request.Params["hdfUploadHierarchy"]
                );

                // Import the taxonomy for the selected
                // hierarchy using the uploaded excel file.
                CreateTaxonomyByExcel(fileName, language, idHierarchy);
            }
            else
            {
                // Import the taxonomy across all hierarchies.
                CreateTaxonomyByExcel(fileName, language);
            }

            Global.ClientManager.IncreaseCaseDataVersion(Global.Core.ClientName);
            Global.Core.CaseDataVersion = Global.ClientManager.GetSingle(Global.Core.ClientName).CaseDataVersion;

            // Delete the temporary file.
            File.Delete(fileName);

            Response.Redirect(
                Request.Url.ToString()
            );
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            cbReset.Visible = true;

            cbReset.Text = string.Format(
                Global.LanguageManager.GetText("ResetTaxonomyConfirmMessage")
            );

            cbReset.Confirm = delegate ()
            {
                // Get all user ids of the client's database.
                List<object[]> users = Global.Core.Users.GetValues(
                    new string[] { "Id" },
                    new string[] { },
                    new object[] { }
                );

                string directoryName;

                // Run through all user ids
                foreach (object[] idUser in users)
                {
                    directoryName = Path.Combine(
                        HttpContext.Current.Request.PhysicalApplicationPath,
                        "Fileadmin",
                        "SavedReports",
                        Global.Core.ClientName,
                        idUser[0].ToString()
                    );

                    // Check if the user has any saved reports.
                    if (Directory.Exists(directoryName))
                    {
                        // Delete the user's saved reports.
                        Directory.Delete(directoryName, true);
                    }
                }

                // Run through all user ids
                foreach (object[] idUser in users)
                {
                    directoryName = Path.Combine(
                        HttpContext.Current.Request.PhysicalApplicationPath,
                        "Fileadmin",
                        "ReportDefinitions",
                        Global.Core.ClientName,
                        idUser[0].ToString()
                    );

                    // Check if the user has any saved reports.
                    if (Directory.Exists(directoryName))
                    {
                        // Delete the user's report.
                        Directory.Delete(directoryName, true);
                    }
                }

                // Run through all user ids
                foreach (object[] idUser in users)
                {
                    directoryName = Path.Combine(
                        HttpContext.Current.Request.PhysicalApplicationPath,
                        "Fileadmin",
                        "LinkBiDefinitions",
                        Global.Core.ClientName,
                        idUser[0].ToString()
                    );

                    // Check if the user has any saved reports.
                    if (Directory.Exists(directoryName))
                    {
                        // Delete the user's report.
                        Directory.Delete(directoryName, true);
                    }
                }

                directoryName = Path.Combine(
                    HttpContext.Current.Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "SavedLinkBiDefinitions",
                    Global.Core.ClientName
                );

                if (Directory.Exists(directoryName))
                {
                    Directory.Delete(directoryName, true);
                }

                Global.Core.TaxonomyCategoryLabels.ExecuteQuery("DELETE FROM [CategoryLinks];");
                Global.Core.TaxonomyCategoryLabels.ExecuteQuery("DELETE FROM [VariableLinks];");

                Global.Core.TaxonomyVariableLabels.ExecuteQuery("DELETE FROM [TaxonomyVariableHierarchies];");

                Global.Core.TaxonomyCategoryLabels.ExecuteQuery("DELETE FROM [TaxonomyCategoryLabels];");
                Global.Core.TaxonomyCategories.ExecuteQuery("DELETE FROM [TaxonomyCategories];");
                Global.Core.TaxonomyVariableLabels.ExecuteQuery("DELETE FROM [TaxonomyVariableLabels];");
                Global.Core.TaxonomyVariables.ExecuteQuery("DELETE FROM [TaxonomyVariables];");
                Global.Core.TaxonomyChapterLabels.ExecuteQuery("DELETE FROM [TaxonomyChapterLabels];");
                Global.Core.TaxonomyChapters.ExecuteQuery("DELETE FROM [TaxonomyChapters];");

                cbReset.Visible = false;

                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.ToString());
            };
        }

        #endregion
    }

    public enum TaxonomyVariableAdditionalParameter
    {
        Scale,
        Weight,
        Hidden,
        DELETE
    }

    public enum TaxonomyUploadAction
    {
        Create,
        Update
    }
}