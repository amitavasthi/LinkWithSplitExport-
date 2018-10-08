using ApplicationUtilities.Classes;
using DatabaseCore.BaseClasses;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LinkOnline.Classes
{
    public class MetadataExport
    {
        #region Properties

        /// <summary>
        /// Gets or sets which columns
        /// for the variables to render.
        /// </summary>
        public MetadataVariableColumn[] VariableColumns { get; set; }

        /// <summary>
        /// Gets or sets which columns
        /// for the categories to render.
        /// </summary>
        public MetadataCategoryColumn[] CategoryColumns { get; set; }

        private int ColumnIndex { get; set; }

        private Dictionary<string, object[]> ValueCache { get; set; }

        private Dictionary<Guid, List<object[]>> CategoryHierarchies { get; set; }

        private Dictionary<Guid, List<object[]>> Hierarchies { get; set; }

        public int IdLanguage { get; set; }

        #endregion


        #region Constructor

        public MetadataExport()
        {
            this.IdLanguage = 2057;
            this.ValueCache = new Dictionary<string, object[]>();
            this.CategoryHierarchies = new Dictionary<Guid, List<object[]>>();
            this.Hierarchies = new Dictionary<Guid, List<object[]>>();

            this.VariableColumns = new MetadataVariableColumn[]
            {
                MetadataVariableColumn.Chapter,
                MetadataVariableColumn.Type,
                MetadataVariableColumn.Name,
                MetadataVariableColumn.Label,
                MetadataVariableColumn.Additional,
                MetadataVariableColumn.Hierarchy
            };

            this.CategoryColumns = new MetadataCategoryColumn[]
            {
                MetadataCategoryColumn.VariableName,
                MetadataCategoryColumn.Value,
                MetadataCategoryColumn.Name,
                MetadataCategoryColumn.Label,
                MetadataCategoryColumn.Additional,
                MetadataCategoryColumn.Hierarchy
            };
        }

        public MetadataExport(
            MetadataVariableColumn[] variableColumns,
            MetadataCategoryColumn[] categoryColumns
        ) : this()
        {
            this.VariableColumns = variableColumns;
            this.CategoryColumns = categoryColumns;
        }

        #endregion


        #region Methods

        public string Render()
        {
            return Render(Global.Core.TaxonomyVariables.GetValues(
                new string[] { "Id", "Name", "IdTaxonomyChapter", "Type", "Scale" },
                new string[] { },
                new object[] { },
                "Name"
            ));
        }

        public string Render(List<object[]> variables)
        {
            this.CategoryHierarchies = Global.Core.TaxonomyCategoryHierarchies.GetValuesDict(
                new string[] { "IdTaxonomyCategory", "IdHierarchy" },
                new string[] { },
                new object[] { }
            );
            this.Hierarchies = Global.Core.Hierarchies.GetValuesDict(
                new string[] { "Id", "Name", "IdHierarchy" },
                new string[] { },
                new object[] { }
            );

            // Build the full path to the session's
            // temporary file storage.
            string fileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Temp",
                HttpContext.Current.Session.SessionID,
                Guid.NewGuid() + ".xlsx"
            );

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            // Create a new excel writer.
            ExcelWriter writer = new ExcelWriter();

            writer.ActiveSheet.Name = "Variables";

            // Run through all variable columns to render.
            foreach (MetadataVariableColumn variableColumn in this.VariableColumns)
            {
                // Render the headline of the variable column.
                writer.Write(this.ColumnIndex++, Global.LanguageManager.
                    GetText("MetadataVariableColumn" + variableColumn));
            }

            writer.ActiveSheet.Cells[0, 0, 0, this.VariableColumns.Length - 1].Interior.Color = SpreadsheetGear.Color.FromArgb(108, 174, 224);
            writer.ActiveSheet.Cells[0, 0, 0, this.VariableColumns.Length - 1].Font.Color = SpreadsheetGear.Color.FromArgb(255, 255, 255);

            // Reset the column index.
            this.ColumnIndex = 0;
            writer.NewLine();

            // Run through all variables to render.
            foreach (object[] variable in variables)
            {
                // Run through all variable columns to render.
                foreach (MetadataVariableColumn variableColumn in this.VariableColumns)
                {
                    // Render the column value of the variable.
                    RenderColumn(writer, variable, variableColumn);
                }

                // Reset the column index.
                this.ColumnIndex = 0;
                writer.NewLine();
            }

            for (int i = 0; i < this.VariableColumns.Length; i++)
                writer.ActiveSheet.Cells[0, i].EntireColumn.AutoFit();

            for (int i = 0; i < this.VariableColumns.Length; i++)
            {
                if (this.VariableColumns[i] != MetadataVariableColumn.Label)
                    continue;

                writer.ActiveSheet.Cells[0, i].EntireColumn.WrapText = true;
                writer.ActiveSheet.Cells[0, i].EntireColumn.ColumnWidth = 100;
            }

            writer.NewSheet("Categories");

            // Run through all variable columns to render.
            foreach (MetadataCategoryColumn categoryColumn in this.CategoryColumns)
            {
                // Render the headline of the variable column.
                writer.Write(this.ColumnIndex++, Global.LanguageManager.
                    GetText("MetadataCategoryColumn" + categoryColumn));
            }

            writer.ActiveSheet.Cells[0, 0, 0, this.CategoryColumns.Length - 1].
                Interior.Color = SpreadsheetGear.Color.FromArgb(108, 174, 224);
            writer.ActiveSheet.Cells[0, 0, 0, this.CategoryColumns.Length - 1].
                Font.Color = SpreadsheetGear.Color.FromArgb(255, 255, 255);

            // Reset the column index.
            this.ColumnIndex = 0;
            writer.NewLine();

            List<object[]> categories;

            // Run through all variables to render.
            foreach (object[] variable in variables)
            {
                // Get all categories of the variable.
                /*categories = Global.Core.TaxonomyCategories.GetValues(
                    new string[] { "Id", "IdTaxonomyVariable", "Name", "Value", "Enabled" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { variable[0] },
                    "Value"
                );*/
                categories = Global.Core.TaxonomyCategories.ExecuteReader(
                    "SELECT [Id], [IdTaxonomyVariable], [Name], [Value], [Enabled], " +
                    "(SELECT Label FROM TaxonomyCategoryLabels WHERE [IdTaxonomyCategory]=TaxonomyCategories.Id " +
                    "AND IdLanguage={1}) FROM TaxonomyCategories " +
                    "WHERE [IdTaxonomyVariable]={0} ORDER BY [Value];",
                    new object[] { variable[0], this.IdLanguage }
                );

                // Run through all categories of the variable.
                foreach (object[] category in categories)
                {
                    // Run through all category columns to render.
                    foreach (MetadataCategoryColumn categoryColumn in this.CategoryColumns)
                    {
                        // Render the column value of the category.
                        RenderColumn(writer, category, categoryColumn);
                    }

                    // Reset the column index.
                    this.ColumnIndex = 0;
                    writer.NewLine();
                }
            }

            for (int i = 0; i < this.CategoryColumns.Length; i++)
                writer.ActiveSheet.Cells[0, i].EntireColumn.AutoFit();

            for (int i = 0; i < this.CategoryColumns.Length; i++)
            {
                if (this.CategoryColumns[i] != MetadataCategoryColumn.Label &&
                    this.CategoryColumns[i] != MetadataCategoryColumn.VariableLabel)
                    continue;

                writer.ActiveSheet.Cells[0, i].EntireColumn.WrapText = true;
                writer.ActiveSheet.Cells[0, i].EntireColumn.ColumnWidth = 100;
            }

            // Save the contents of the excel writer to the temp file.
            writer.Save(fileName);

            // Return the full path to the result excel
            // sheet in the session's temporary file storage.
            return fileName;
        }


        private void RenderColumn(ExcelWriter writer, object[] variable, MetadataVariableColumn column)
        {
            string value = string.Empty;

            switch (column)
            {
                case MetadataVariableColumn.Chapter:
                    if(variable[2] != null)
                    value = GetValues(Global.Core.TaxonomyChapterLabels, new string[] {
                        "Label"
                    }, new string[] {
                        "IdLanguage",
                        "IdTaxonomyChapter"
                    }, new object[] {
                        this.IdLanguage,
                        variable[2]
                    })[0].ToString();
                    break;
                case MetadataVariableColumn.Name:
                    value = (string)variable[1];
                    break;
                case MetadataVariableColumn.Type:
                    value = ((VariableType)variable[3]).ToString();
                    break;
                case MetadataVariableColumn.Label:
                    value = GetValue<string>(Global.Core.TaxonomyVariableLabels, new string[] {
                        "Label"
                    }, new string[] {
                        "IdLanguage",
                        "IdTaxonomyVariable"
                    }, new object[] {
                        this.IdLanguage,
                        variable[0]
                    });
                    break;
                case MetadataVariableColumn.Additional:
                    value = ((bool)variable[4]) ? "SCALE" : "";
                    break;
                case MetadataVariableColumn.Hierarchy:
                    List<string> hierarchyStrings = new List<string>();

                    List<object[]> hierarchies = Global.Core.TaxonomyVariableHierarchies.GetValues(
                        new string[] { "IdHierarchy" },
                        new string[] { "IdTaxonomyVariable" },
                        new object[] { variable[0] }
                    );

                    foreach (object[] hierarchy in hierarchies)
                    {
                        hierarchyStrings.Add(BuildHierarchyPath((Guid)hierarchy[0]));
                    }

                    value = string.Join(",", hierarchyStrings.ToArray());
                    break;
            }

            if (value == null)
                value = "";

            writer.Write(this.ColumnIndex++, value);
        }

        private void RenderColumn(ExcelWriter writer, object[] category, MetadataCategoryColumn column)
        {
            string value = string.Empty;

            switch (column)
            {
                case MetadataCategoryColumn.VariableName:
                    value = (string)GetValues(Global.Core.TaxonomyVariables, new string[]
                    {
                        "Name"
                    }, new string[] {
                        "Id"
                    }, new object[] {
                        category[1]
                    })[0];
                    break;
                case MetadataCategoryColumn.VariableLabel:
                    value = (string)GetValues(Global.Core.TaxonomyVariableLabels, new string[]
                    {
                        "Label"
                    }, new string[] {
                        "IdLanguage",
                        "IdTaxonomyVariable"
                    }, new object[] {
                        this.IdLanguage,
                        category[1]
                    })[0];
                    break;
                case MetadataCategoryColumn.Value:
                    value = category[3].ToString();
                    break;
                case MetadataCategoryColumn.Name:
                    value = (string)category[2];
                    break;
                case MetadataCategoryColumn.Label:
                    /*value = (string)GetValues(Global.Core.TaxonomyCategoryLabels, new string[]
                    {
                        "Label"
                    }, new string[] {
                        "IdLanguage",
                        "IdTaxonomyCategory"
                    }, new object[] {
                        this.IdLanguage,
                        category[0]
                    })[0];*/
                    value = (string)category[5];
                    break;
                case MetadataCategoryColumn.Additional:
                    value = ((bool)category[4]) ? "" : "HIDDEN";
                    break;
                case MetadataCategoryColumn.Hierarchy:
                    List<string> hierarchyStrings = new List<string>();

                    if (this.CategoryHierarchies.ContainsKey((Guid)category[0]))
                    {
                        foreach (object[] hierarchy in this.CategoryHierarchies[(Guid)category[0]])
                        {
                            hierarchyStrings.Add(BuildHierarchyPath((Guid)hierarchy[1]));
                        }
                    }

                    value = string.Join(",", hierarchyStrings.ToArray());
                    break;
            }

            writer.Write(this.ColumnIndex++, value);
        }

        private T GetValue<T>(BaseCollection collection, string[] fields, string[] names, object[] values)
        {
            object[] result = GetValues(
                collection,
                fields,
                names,
                values
            );

            if (result.Length != 0)
                return (T)result[0];
            else
                return default(T);
        }

        private object[] GetValues(BaseCollection collection, string[] fields, string[] names, object[] values)
        {
            string key = string.Join("", fields) + string.Join("", names) + string.Join("", values);

            if (this.ValueCache.ContainsKey(key))
                return this.ValueCache[key];

            List<object[]> result = collection.GetValues(fields, names, values);

            if (result.Count != 0)
                this.ValueCache.Add(key, result[0]);
            else
                this.ValueCache.Add(key, new object[0]);

            return this.ValueCache[key];
        }

        private string BuildHierarchyPath(Guid idHierarchy)
        {
            /*object[] hierarchy = GetValues(
                Global.Core.Hierarchies,
                new string[] { "Name", "IdHierarchy" },
                new string[] { "Id" },
                new object[] { idHierarchy }
            );*/
            if (!this.Hierarchies.ContainsKey(idHierarchy))
                return "";

            object[] hierarchy = this.Hierarchies[idHierarchy][0];

            if (hierarchy[2] != null)
                return BuildHierarchyPath((Guid)hierarchy[2]) + "/" + (string)hierarchy[1];
            else
                return (string)hierarchy[1];
        }

        #endregion
    }

    public enum MetadataVariableColumn
    {
        Chapter,
        Name,
        Type,
        Label,
        Additional,
        Hierarchy
    }

    public enum MetadataCategoryColumn
    {
        VariableName,
        VariableLabel,
        Value,
        Name,
        Label,
        Additional,
        Hierarchy
    }
}