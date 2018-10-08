using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using WebUtilities;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class ReportDefinitionRenderer : WebUtilities.BaseControl
    {
        #region Properties

        public ReportDefinition Definition { get; set; }

        public LanguageManager LanguageManager { get; set; }

        public Language Language { get; set; }

        public int CellHeight { get; set; }
        public int CellWidth { get; set; }

        public int ContentWidth
        {
            get
            {
                if (HttpContext.Current.Session["ContentWidth"] == null)
                    HttpContext.Current.Session["ContentWidth"] = 0;

                return (int)HttpContext.Current.Session["ContentWidth"];
            }
        }

        public int ContentHeight
        {
            get
            {
                if (HttpContext.Current.Session["ContentHeight"] == null)
                    HttpContext.Current.Session["ContentHeight"] = 0;

                return (int)HttpContext.Current.Session["ContentHeight"];
            }
        }

        public Dictionary<string, XmlNode> DefinitionXmlNodeMapping { get; set; }

        public bool IsExport { get; set; }

        #endregion


        #region Constructor

        public ReportDefinitionRenderer(ReportDefinition definition, bool isExport = false)
        {
            this.DefinitionXmlNodeMapping = new Dictionary<string, XmlNode>();
            this.Definition = definition;
            this.LanguageManager = (LanguageManager)HttpContext.Current.Session["LanguageManager"];
            this.Language = (Language)HttpContext.Current.Session["Language"];
            this.IsExport = isExport;
        }

        #endregion


        #region Methods

        public string Render(
            int cellWidth,
            int cellHeight,
            ReportDefinitionRenderConfiguration renderConfiguration = null
        )
        {
            this.CellWidth = cellWidth;
            this.CellHeight = cellHeight;

            if (renderConfiguration == null)
            {
                renderConfiguration = new ReportDefinitionRenderConfiguration();

                renderConfiguration.RenderSigDiff = this.Definition.Settings.SignificanceTest;
                renderConfiguration.RenderValue = this.Definition.Settings.ShowValues;
                renderConfiguration.RenderPercentage = this.Definition.Settings.ShowPercentage;
            }

            if (this.Definition.Settings.BaseType == BaseType.TotalBase)
                renderConfiguration.BasePrefix = "Total";

            if (this.Definition.TopVariables.Count == 0 || this.Definition.TopVariables[0].IsFake)
            {
                renderConfiguration.RenderSigDiff = false;
            }
            if (this.Definition.LeftVariables.Count == 0 || this.Definition.LeftVariables[0].IsFake)
            {
                if (!renderConfiguration.PowerBIExport)
                {
                    renderConfiguration.RenderValue = false;
                    renderConfiguration.RenderPercentage = false;
                    renderConfiguration.RenderSigDiff = false;
                }
            }

            StringBuilder result = new StringBuilder();

            if (renderConfiguration.PowerBIExport)
            {
                result.Append("<h1>");

                if (this.Definition.XmlDocument.DocumentElement.Attributes["Name"] != null)
                    result.Append(this.Definition.XmlDocument.DocumentElement.Attributes["Name"].Value);
                else
                    result.Append("Table");

                result.Append("</h1>");
            }

            result.Append("<table class=\"RendererV2\" cellspacing=\"0\" cellpadding=\"0\">");

            if (renderConfiguration.PowerBIExport)
            {
                result.Append("<thead><tr>");

                if (this.Definition.XmlDocument.DocumentElement.SelectNodes("Variables[@Position=\"Left\"]/Variable")[0].Attributes["IsFake"] != null &&
                    bool.Parse(this.Definition.XmlDocument.DocumentElement.SelectNodes("Variables[@Position=\"Left\"]/Variable")[0].Attributes["IsFake"].Value) == true)
                {
                    string topLabel = null;
                    topLabel = this.Definition.XmlDocument.DocumentElement.SelectNodes("Variables[@Position=\"Top\"]/Variable")[0].Attributes["Label2057"].Value;

                    result.Append(string.Format(
                        "<th>{0}</th>",
                       topLabel
                    ));
                }

                RenderLeftVariableLabels(
                    result,
                    this.Definition.XmlDocument.DocumentElement.SelectNodes("Variables[@Position=\"Left\"]/Variable")
                );


                foreach (XmlNode xmlNodeTopVariable in this.Definition.XmlDocument.DocumentElement.SelectNodes("Variables[@Position=\"Top\"]/Variable"))
                {
                    RenderTopVariableLabels(xmlNodeTopVariable, result);
                }

                result.Append("</tr></thead>");
                result.Append("<tbody>");
            }

            XmlNodeList xmlNodesLeftVariables = this.Definition.XmlDocument.SelectNodes("Report/Results/Variable");

            foreach (XmlNode xmlNodeLeftVariable in xmlNodesLeftVariables)
            {
                RenderLeftVariable(
                    xmlNodeLeftVariable,
                    true,
                    result,
                    renderConfiguration,
                    new List<string>()
                );
            }

            if (renderConfiguration.PowerBIExport)
                result.Append("</tbody>");

            result.Append(@"</table>
                            ");

            return result.ToString();
        }

        public void RenderLeftVariable(
            XmlNode xmlNodeLeftVariable,
            bool root,
            StringBuilder result,
            ReportDefinitionRenderConfiguration renderConfiguration,
            List<string> nestedLeftCategoryLabels
        )
        {
            XmlNodeList xmlNodesLeftCategories = xmlNodeLeftVariable.SelectNodes("*[not(self::Variable)]");

            Dictionary<Guid, Dictionary<Guid, List<string>>> renderedCategoryRows =
                new Dictionary<Guid, Dictionary<Guid, List<string>>>();

            //TaskCollection tasks = new TaskCollection();

            XmlNodeList xmlNodesNestedLeftVariables = xmlNodeLeftVariable.SelectNodes("*/Variable[@Position=\"Left\"]");

            ReportDefinitionRenderConfiguration leftVariableRenderConfiguration = renderConfiguration.Clone();

            if (xmlNodeLeftVariable.SelectSingleNode("StdDev") != null)
            {
                leftVariableRenderConfiguration.RenderSigDiff = false;
                leftVariableRenderConfiguration.RenderPercentage = false;

                if (leftVariableRenderConfiguration.RenderValue == false)
                    leftVariableRenderConfiguration.RenderValue = true;
            }

            bool isTaxonomy = bool.Parse(GetPropertyFromDefinition(xmlNodeLeftVariable, "IsTaxonomy", false.ToString()));

            if (xmlNodesNestedLeftVariables.Count == 0)
            {
                foreach (XmlNode xmlNodeLeftCategory in xmlNodesLeftCategories)
                {
                    string isFake = GetPropertyFromDefinition(xmlNodeLeftCategory, "IsFake");

                    if (isFake != null && bool.Parse(isFake) == true)
                        continue;

                    string enabled = GetPropertyFromDefinition(xmlNodeLeftCategory, "Enabled");
                    if (enabled != null &&
                        bool.Parse(enabled) == false)
                        continue;

                    if (isTaxonomy && GetPropertyFromDefinition(xmlNodeLeftCategory, "Persistent") == "True")
                    {
                        if (!this.Definition.HierarchyFilter.TaxonomyCategories.
                            ContainsKey(Guid.Parse(xmlNodeLeftCategory.Attributes["Id"].Value)))
                            continue;
                    }

                    if (this.Definition.Settings.HideEmptyRowsAndColumns)
                    {
                        string hasValues = GetPropertyFromDefinition(xmlNodeLeftCategory, "HasValues");
                        if (hasValues != null &&
                            bool.Parse(hasValues) == false)
                            continue;
                    }

                    ReportDefinitionRenderConfiguration leftCategoryRenderConfiguration = leftVariableRenderConfiguration.Clone();

                    if (xmlNodeLeftCategory.Name == "MeanScore")
                    {
                        if (this.Definition.Settings.SignificanceTest)
                            leftCategoryRenderConfiguration.RenderSigDiff = true;

                        leftCategoryRenderConfiguration.RenderPercentage = false;
                    }

                    //tasks.Add(() =>
                    RenderLeftCategory(
                        xmlNodeLeftCategory,
                        renderedCategoryRows,
                        leftCategoryRenderConfiguration
                    );
                    //);
                }

                //tasks.WaitAll();

                StringBuilder trValues = new StringBuilder();
                StringBuilder trPercentage = new StringBuilder();
                StringBuilder trSigDiff = new StringBuilder();

                if (this.Definition.Settings.DisplayUnweightedBase && renderConfiguration.PowerBIExport == false && xmlNodesLeftCategories.Count != 0)
                {
                    Dictionary<Guid, Dictionary<Guid, List<string>>> renderedBaseRows =
                        new Dictionary<Guid, Dictionary<Guid, List<string>>>();

                    Guid idScore = Guid.Parse(xmlNodesLeftCategories[0].Attributes["Id"].Value);

                    RenderLeftCategory(
                        xmlNodesLeftCategories[0],
                        renderedBaseRows,
                        new ReportDefinitionRenderConfiguration(true, false, false, "Count", "Count", "class=\"TableCellBase TableCellTopBase\"", true)
                        {
                            BasePrefix = this.Definition.Settings.BaseType == BaseType.TotalBase ? "Total" : ""
                        }
                    );

                    trValues.Append("<tr>");

                    foreach (XmlNode xmlNodeTopVariable in xmlNodesLeftCategories[0].SelectNodes("Variable[@IsNestedBase=\"True\"]"))
                    {
                        Guid idTopVariable = Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value);

                        trValues.Append(renderedBaseRows[idScore][idTopVariable][0]);
                    }
                    foreach (XmlNode xmlNodeTopVariable in xmlNodesLeftCategories[0].SelectNodes("Variable[not(@IsNestedBase=\"True\")]"))
                    {
                        Guid idTopVariable = Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value);

                        trValues.Append(renderedBaseRows[idScore][idTopVariable][0]);
                    }

                    trValues.Append("</tr>");
                }

                if (this.Definition.Settings.DisplayEffectiveBase && renderConfiguration.PowerBIExport == false && xmlNodesLeftCategories.Count != 0)
                {
                    Dictionary<Guid, Dictionary<Guid, List<string>>> renderedBaseRows =
                        new Dictionary<Guid, Dictionary<Guid, List<string>>>();

                    Guid idScore = Guid.Parse(xmlNodesLeftCategories[0].Attributes["Id"].Value);

                    RenderLeftCategory(
                        xmlNodesLeftCategories[0],
                        renderedBaseRows,
                        new ReportDefinitionRenderConfiguration(true, false, false, "EffectiveBase", "EffectiveBase", "class=\"TableCellBase TableCellTopBase\"", true)
                        {
                            BasePrefix = this.Definition.Settings.BaseType == BaseType.TotalBase ? "Total" : ""
                        }
                    );

                    trValues.Append("<tr>");

                    foreach (XmlNode xmlNodeTopVariable in xmlNodesLeftCategories[0].SelectNodes("Variable[@IsNestedBase=\"True\"]"))
                    {
                        Guid idTopVariable = Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value);

                        trValues.Append(renderedBaseRows[idScore][idTopVariable][0]);
                    }
                    foreach (XmlNode xmlNodeTopVariable in xmlNodesLeftCategories[0].SelectNodes("Variable[not(@IsNestedBase=\"True\")]"))
                    {
                        Guid idTopVariable = Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value);

                        trValues.Append(renderedBaseRows[idScore][idTopVariable][0]);
                    }

                    trValues.Append("</tr>");
                }

                if (xmlNodesLeftCategories.Count != 0 && renderConfiguration.PowerBIExport == false)
                {
                    /*Fix for Bug 404 in leantesting.com*/

                    foreach (XmlNode xmlNodeLeftCategory in xmlNodesLeftCategories)
                    {
                        /*string enabled = GetPropertyFromDefinition(xmlNodeLeftCategory, "Enabled");

                            if (enabled != null &&
                                bool.Parse(enabled) == false)
                                continue;*/

                        Guid idScore = Guid.Parse(xmlNodeLeftCategory.Attributes["Id"].Value);

                        if (isTaxonomy && GetPropertyFromDefinition(xmlNodeLeftCategory, "Persistent") == "True")
                        {
                            if (!this.Definition.HierarchyFilter.TaxonomyCategories.
                                ContainsKey(idScore))
                                continue;
                        }

                        Dictionary<Guid, Dictionary<Guid, List<string>>> renderedBaseRows =
                            new Dictionary<Guid, Dictionary<Guid, List<string>>>();

                        RenderLeftCategory(
                            xmlNodeLeftCategory,
                            renderedBaseRows,
                            new ReportDefinitionRenderConfiguration(
                                true,
                                false,
                                false,
                                "Base",
                                "Base",
                                "class=\"TableCellBase TableCellTopBase\"",
                                true
                            )
                            {
                                BasePrefix = this.Definition.Settings.BaseType == BaseType.TotalBase ? "Total" : ""
                            }
                        );

                        trValues.Append("<tr>");

                        foreach (XmlNode xmlNodeTopVariable in xmlNodeLeftCategory.SelectNodes("Variable[@IsNestedBase=\"True\"]"))
                        {
                            Guid idTopVariable = Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value);

                            trValues.Append(renderedBaseRows[idScore][idTopVariable][0]);
                        }

                        foreach (XmlNode xmlNodeTopVariable in xmlNodeLeftCategory.SelectNodes("Variable[not(@IsNestedBase=\"True\")]"))
                        {
                            Guid idTopVariable = Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value);

                            trValues.Append(renderedBaseRows[idScore][idTopVariable][0]);
                        }

                        trValues.Append("</tr>");
                        break;
                    }

                    /*End for Bug 404 in leantesting.com*/

                    //Dictionary<Guid, Dictionary<Guid, List<string>>> renderedBaseRows =
                    //    new Dictionary<Guid, Dictionary<Guid, List<string>>>();

                    //Guid idScore = Guid.Parse(xmlNodesLeftCategories[0].Attributes["Id"].Value);

                    //RenderLeftCategory(
                    //    xmlNodesLeftCategories[0],
                    //    renderedBaseRows,
                    //    new ReportDefinitionRenderConfiguration(
                    //        true,
                    //        false,
                    //        false,
                    //        "Base",
                    //        "Base",
                    //        "class=\"TableCellBase TableCellTopBase\"",
                    //        true
                    //    )
                    //);

                    //trValues.Append("<tr>");

                    //foreach (XmlNode xmlNodeTopVariable in xmlNodesLeftCategories[0].SelectNodes("Variable[@IsNestedBase=\"True\"]"))
                    //{
                    //    Guid idTopVariable = Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value);

                    //    trValues.Append(renderedBaseRows[idScore][idTopVariable][0]);
                    //}

                    //foreach (XmlNode xmlNodeTopVariable in xmlNodesLeftCategories[0].SelectNodes("Variable[not(@IsNestedBase=\"True\")]"))
                    //{
                    //    Guid idTopVariable = Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value);

                    //    trValues.Append(renderedBaseRows[idScore][idTopVariable][0]);
                    //}

                    //trValues.Append("</tr>");
                }

                result.Append(trValues.ToString());
                result.Append(trPercentage.ToString());
                result.Append(trSigDiff.ToString());

                foreach (XmlNode xmlNodeLeftCategory in xmlNodesLeftCategories)
                {
                    string isFake = GetPropertyFromDefinition(xmlNodeLeftCategory, "IsFake");

                    if (isFake != null && bool.Parse(isFake) == true)
                        continue;

                    if (renderConfiguration.RenderMeanScore == false && xmlNodeLeftCategory.Name == "MeanScore")
                        continue;

                    string enabled = GetPropertyFromDefinition(xmlNodeLeftCategory, "Enabled");
                    if (enabled != null &&
                        bool.Parse(enabled) == false)
                        continue;

                    if (isTaxonomy && GetPropertyFromDefinition(xmlNodeLeftCategory, "Persistent") == "True")
                    {
                        if (!this.Definition.HierarchyFilter.TaxonomyCategories.
                            ContainsKey(Guid.Parse(xmlNodeLeftCategory.Attributes["Id"].Value)))
                            continue;
                    }

                    if (this.Definition.Settings.HideEmptyRowsAndColumns)
                    {
                        string hasValues = GetPropertyFromDefinition(xmlNodeLeftCategory, "HasValues");
                        if (hasValues != null &&
                            bool.Parse(hasValues) == false)
                            continue;
                    }

                    ReportDefinitionRenderConfiguration leftCategoryRenderConfiguration = leftVariableRenderConfiguration.Clone();

                    if (xmlNodeLeftCategory.Name == "MeanScore")
                    {
                        if (this.Definition.Settings.SignificanceTest)
                            leftCategoryRenderConfiguration.RenderSigDiff = true;

                        leftCategoryRenderConfiguration.RenderPercentage = false;
                    }

                    trValues = new StringBuilder();
                    trPercentage = new StringBuilder();
                    trSigDiff = new StringBuilder();

                    Guid idScore = Guid.Parse(xmlNodeLeftCategory.Attributes["Id"].Value);

                    if (!renderedCategoryRows.ContainsKey(idScore))
                    {
                        RenderLeftCategory(
                            xmlNodeLeftCategory,
                            renderedCategoryRows,
                            leftCategoryRenderConfiguration
                        );
                    }

                    //if (root)
                    {
                        trValues.Append("<tr>");
                        trPercentage.Append("<tr>");
                        trSigDiff.Append("<tr>");
                    }

                    if (renderConfiguration.PowerBIExport)
                    {
                        foreach (string cat in nestedLeftCategoryLabels)
                        {
                            if (renderConfiguration.RenderValue)
                            {
                                trValues.Append(string.Format(
                                    "<th>{0}</th>",
                                    cat
                                ));
                            }
                            else
                            {
                                trPercentage.Append(string.Format(
                                    "<th>{0}</th>",
                                    cat
                                ));
                            }
                        }
                    }

                    foreach (XmlNode xmlNodeTopVariable in xmlNodeLeftCategory.SelectNodes("Variable[@IsNestedBase=\"True\"]"))
                    {
                        Guid idTopVariable = Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value);

                        if (!renderedCategoryRows[idScore].ContainsKey(idTopVariable))
                        {
                            renderedCategoryRows.Remove(idScore);

                            RenderLeftCategory(
                                xmlNodeLeftCategory,
                                renderedCategoryRows,
                                leftCategoryRenderConfiguration
                            );
                        }

                        trValues.Append(renderedCategoryRows[idScore][idTopVariable][0]);
                        trPercentage.Append(renderedCategoryRows[idScore][idTopVariable][1]);
                        trSigDiff.Append(renderedCategoryRows[idScore][idTopVariable][2]);
                    }
                    foreach (XmlNode xmlNodeTopVariable in xmlNodeLeftCategory.SelectNodes("Variable[not(@IsNestedBase=\"True\")]"))
                    {
                        Guid idTopVariable = Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value);

                        if (!renderedCategoryRows[idScore].ContainsKey(idTopVariable))
                        {
                            renderedCategoryRows.Remove(idScore);

                            RenderLeftCategory(
                                xmlNodeLeftCategory,
                                renderedCategoryRows,
                                leftCategoryRenderConfiguration
                            );
                        }

                        trValues.Append(renderedCategoryRows[idScore][idTopVariable][0]);
                        trPercentage.Append(renderedCategoryRows[idScore][idTopVariable][1]);
                        trSigDiff.Append(renderedCategoryRows[idScore][idTopVariable][2]);
                    }

                    //if (root)
                    {
                        trValues.Append("</tr>");
                        trPercentage.Append("</tr>");
                        trSigDiff.Append("</tr>");
                    }

                    result.Append(trValues.ToString());
                    result.Append(trPercentage.ToString());
                    result.Append(trSigDiff.ToString());
                }
            }
            else
            {
                XmlNodeList xmlNodesNestedLeftVariablesBase;

                xmlNodesNestedLeftVariablesBase = xmlNodeLeftVariable.SelectNodes("Variable[@IsNestedBase=\"True\" and @Position=\"Left\"]");

                if (renderConfiguration.PowerBIExport == false)
                {
                    foreach (XmlNode xmlNodeNestedLeftVariableBase in xmlNodesNestedLeftVariablesBase)
                    {
                        RenderLeftVariable(
                            xmlNodeNestedLeftVariableBase,
                            false,
                            result,
                            renderConfiguration,
                            new List<string>()
                        );
                    }
                }

                foreach (XmlNode xmlNodeLeftCategory in xmlNodesLeftCategories)
                {
                    if (renderConfiguration.RenderMeanScore == false && xmlNodeLeftCategory.Name == "MeanScore")
                        continue;

                    string enabled = GetPropertyFromDefinition(xmlNodeLeftCategory, "Enabled");
                    if (enabled != null &&
                        bool.Parse(enabled) == false)
                        continue;

                    if (isTaxonomy && GetPropertyFromDefinition(xmlNodeLeftCategory, "Persistent") == "True")
                    {
                        if (!this.Definition.HierarchyFilter.TaxonomyCategories.
                            ContainsKey(Guid.Parse(xmlNodeLeftCategory.Attributes["Id"].Value)))
                            continue;
                    }

                    /*if (renderConfiguration.PowerBIExport)
                    {
                        result.Append(string.Format(
                            "<th>{0}</th>",
                            GetPropertyFromDefinition(xmlNodeLeftCategory, "Label2057")
                        ));
                    }*/

                    ReportDefinitionRenderConfiguration leftCategoryRenderConfiguration = renderConfiguration.Clone();

                    if (xmlNodeLeftCategory.Name == "MeanScore")
                    {
                        leftCategoryRenderConfiguration.RenderSigDiff = true;
                        leftCategoryRenderConfiguration.RenderPercentage = false;
                    }

                    List<string> _nestedLeftCategoryLabels = nestedLeftCategoryLabels.Clone();
                    _nestedLeftCategoryLabels.Add(GetPropertyFromDefinition(xmlNodeLeftCategory, "Label2057"));

                    xmlNodesNestedLeftVariables = xmlNodeLeftCategory.SelectNodes("Variable[@Position=\"Left\"]");
                    foreach (XmlNode xmlNodeNestedLeftVariable in xmlNodesNestedLeftVariables)
                    {
                        RenderLeftVariable(
                            xmlNodeNestedLeftVariable,
                            false,
                            result,
                            leftCategoryRenderConfiguration,
                            _nestedLeftCategoryLabels
                        );
                    }
                }
            }
        }

        public void RenderLeftCategory(
            XmlNode xmlNode,
            Dictionary<Guid, Dictionary<Guid, List<string>>> renderedCategoryRows,
            ReportDefinitionRenderConfiguration renderConfiguration
        )
        {
            StringBuilder trValues;
            StringBuilder trPercentage;
            StringBuilder trSigDiff;

            if (xmlNode.Attributes["RenderPercentage"] != null && bool.Parse(xmlNode.Attributes["RenderPercentage"].Value) == false)
                renderConfiguration.RenderPercentage = false;

            XmlNodeList xmlNodesTopVariables = xmlNode.SelectNodes("Variable[@Position=\"Top\"]");

            Guid idLeftScore = Guid.Parse(xmlNode.Attributes["Id"].Value);

            Dictionary<Guid, List<string>> renderedTopCategoryValues =
                new Dictionary<Guid, List<string>>();

            int i = 0;
            foreach (XmlNode xmlNodeTopVariable in xmlNodesTopVariables)
            {
                trValues = new StringBuilder();
                trPercentage = new StringBuilder();
                trSigDiff = new StringBuilder();

                ReportDefinitionRenderConfiguration topVariableRenderConfiguration = renderConfiguration.Clone();

                if (xmlNode.Name == "Mean" ||
                    xmlNode.Name == "Max" ||
                    xmlNode.Name == "Min" ||
                    xmlNode.Name == "StdDev")
                {
                    topVariableRenderConfiguration.IsBase = true;
                }

                if (i++ == 0 && topVariableRenderConfiguration.PowerBIExport)
                {
                    if (renderConfiguration.RenderValue)
                    {
                        if (!String.IsNullOrEmpty(GetPropertyFromDefinition(xmlNode, "Label2057")))
                        {
                            trValues.Append(string.Format(
                                "<th>{0}</th>",
                                GetPropertyFromDefinition(xmlNode, "Label2057")
                            ));
                        }
                        else
                        {
                            trValues.Append(string.Format(
                                "<th>{0}</th>",
                                 "Value"
                            ));
                        }
                    }
                    else
                    {
                        trPercentage.Append(string.Format(
                            "<th>{0}</th>",
                            GetPropertyFromDefinition(xmlNode, "Label2057")
                        ));
                    }
                }


                RenderTopVariable(
                    xmlNodeTopVariable,
                    trValues,
                    trPercentage,
                    trSigDiff,
                    topVariableRenderConfiguration,
                    Guid.Parse(xmlNode.Attributes["Id"].Value)
                );

                renderedTopCategoryValues.Add(Guid.Parse(xmlNodeTopVariable.Attributes["Id"].Value), (new string[]
                {
                    trValues.ToString(),
                    trPercentage.ToString(),
                    trSigDiff.ToString()
                }).ToList());
            }

            // Needed cause of asynch tasks......
            try
            {
                renderedCategoryRows.Add(idLeftScore, renderedTopCategoryValues);
            }
            catch { }
        }

        public void RenderTopVariable(
            XmlNode xmlNodeTopVariable,
            StringBuilder trValues,
            StringBuilder trPercentage,
            StringBuilder trSigDiff,
            ReportDefinitionRenderConfiguration renderConfiguration,
            Guid idLeftScore
        )
        {

            Dictionary<Guid, List<string>> renderedCategoryRows = new Dictionary<Guid, List<string>>();

            XmlNodeList xmlNodesTopCategories = xmlNodeTopVariable.SelectNodes("*[not(self::Variable)] ");
            Guid sigDiffContext = Guid.NewGuid();
            string sigDiffContextForTotal = sigDiffContext.ToString();

            if (this.Definition.Settings.SignificanceTestType != 4)
                sigDiffContextForTotal = "";

            if (xmlNodeTopVariable.SelectSingleNode("*/Variable[@Position=\"Top\"]") == null)
            {
                bool isNumeric = xmlNodeTopVariable.SelectSingleNode("Mean") != null;

                int rowSpan = 0;

                if (isNumeric && renderConfiguration.IsBase == false)
                {
                    if (this.Definition.Settings.ShowValues)
                        rowSpan++;
                    if (this.Definition.Settings.ShowPercentage)
                        rowSpan++;
                    if (this.Definition.Settings.SignificanceTest)
                        rowSpan++;
                }


                double value;
                double _base = 0.0;
                double percentage;

                //if (this.Definition.Settings.DisplayUnweightedBase &&
                //    renderConfiguration.PowerBIExport == false)
                //{
                //    value = 0.0;

                //    if (renderConfiguration.CssClass != null && renderConfiguration.CssClass.Contains("TableCellTopBase"))
                //    {
                //        //unweighted base value showing based on base type.
                //        if (this.Definition.Settings.BaseType == BaseType.AnsweringBase)
                //        {
                //            if (xmlNodeTopVariable.Attributes["VariableCount"] != null)
                //                value = double.Parse(xmlNodeTopVariable.Attributes["VariableCount"].Value);
                //        }
                //        else
                //        {
                //            if (xmlNodeTopVariable.Attributes["TotalVariableCount"] != null)
                //                value = double.Parse(xmlNodeTopVariable.Attributes["TotalVariableCount"].Value);
                //        }
                //    }
                //    else
                //    {
                //        if (xmlNodeTopVariable.Attributes["Count"] != null)
                //        {
                //            value = double.Parse(xmlNodeTopVariable.Attributes["Count"].Value);
                //        }
                //    }

                //    if (xmlNodeTopVariable.Attributes["VariableCount"] != null)
                //        _base = double.Parse(xmlNodeTopVariable.Attributes["VariableCount"].Value);

                //    if (this.Definition.Settings.BaseType == BaseType.TotalBase && xmlNodeTopVariable.Attributes["TotalVariableCount"] != null)
                //        _base = double.Parse(xmlNodeTopVariable.Attributes["TotalVariableCount"].Value);

                //    percentage = (value * 100 / _base);

                //    if (renderConfiguration.RenderValue)
                //    {
                //        trValues.Append(string.Format(
                //            "<td {2} style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\">",
                //            rowSpan > 1 ? (CellHeight * rowSpan) : CellHeight,
                //            CellWidth,
                //            rowSpan > 1 ? "rowspan=\"" + rowSpan + "\"" : ""
                //        ));

                //        if (double.IsNaN(value) || double.IsInfinity(value))
                //            value = 0;


                //        trValues.Append(Math.Round(value, this.Definition.Settings.DecimalPlaces) != 0 ? value.ToFormattedString(
                //          this.LanguageManager,
                //          this.Language,
                //          this.Definition.Settings.DecimalPlaces
                //      ) : "-");

                //        trValues.Append("</td>");
                //    }
                //    else if (isNumeric)
                //    {
                //        trPercentage.Append(string.Format(
                //            "<td {2} style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\">",
                //            rowSpan > 1 ? (CellHeight * rowSpan) : CellHeight,
                //            CellWidth,
                //            rowSpan > 1 ? "rowspan=\"" + rowSpan + "\"" : ""
                //        ));

                //        if (double.IsNaN(value) || double.IsInfinity(value))
                //            value = 0;

                //        trPercentage.Append(Math.Round(value, this.Definition.Settings.DecimalPlaces) != 0 ? value.ToFormattedString(
                //        this.LanguageManager,
                //        this.Language,
                //        this.Definition.Settings.DecimalPlaces
                //    ) : "-");

                //        trPercentage.Append("</td>");
                //    }

                //    if (!isNumeric)
                //    {
                //        if (renderConfiguration.RenderPercentage)
                //        {
                //            trPercentage.Append(string.Format(
                //                "<td style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\">",
                //                CellHeight,
                //                CellWidth
                //            ));

                //            if (this.IsExport)
                //            {
                //                    trPercentage.Append(Math.Round(percentage, 9) != 0 && double.IsNaN(percentage) == false ? percentage.ToFormattedString(
                //                      this.LanguageManager,
                //                      this.Language,
                //                      9
                //                  ) + " %" : "");
                //            }
                //            else
                //            {
                //                trPercentage.Append(Math.Round(percentage, this.Definition.Settings.DecimalPlaces) != 0 && double.IsNaN(percentage) == false ? percentage.ToFormattedString(
                //                  this.LanguageManager,
                //                  this.Language,
                //                  this.Definition.Settings.DecimalPlaces
                //              ) + " %" : "");
                //            }
                //            trPercentage.Append("</td>");
                //        }

                //        if (renderConfiguration.RenderSigDiff)
                //        {
                //            trSigDiff.Append(string.Format(
                //                "<td style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\">",
                //                CellHeight,
                //                CellWidth
                //            ));
                //            if (xmlNodeTopVariable.Attributes["SigDiff"] != null)
                //            {
                //                trSigDiff.Append(xmlNodeTopVariable.Attributes["SigDiff"].Value);
                //            }                            
                //            trSigDiff.Append("</td>");
                //        }
                //    }
                //}

                //  if (this.Definition.Settings.DisplayEffectiveBase &&
                //renderConfiguration.PowerBIExport == false)
                //  {
                //      value = 0.0;

                //      if (renderConfiguration.CssClass != null && renderConfiguration.CssClass.Contains("TableCellTopBase"))
                //      {
                //          if (renderConfiguration.BaseAttribute== "Count")
                //          {
                //              //unweighted base value showing based on base type.
                //              if (this.Definition.Settings.BaseType == BaseType.AnsweringBase)
                //              {
                //                  if (xmlNodeTopVariable.Attributes["VariableCount"] != null)
                //                      value = double.Parse(xmlNodeTopVariable.Attributes["VariableCount"].Value);
                //              }
                //              else
                //              {
                //                  if (xmlNodeTopVariable.Attributes["TotalVariableCount"] != null)
                //                      value = double.Parse(xmlNodeTopVariable.Attributes["TotalVariableCount"].Value);
                //              }

                //          }
                //          else
                //          {
                //              //unweighted base value showing based on base type.
                //              if (this.Definition.Settings.BaseType == BaseType.AnsweringBase)
                //              {
                //                  if (xmlNodeTopVariable.Attributes["VariableEffectiveBase"] != null)
                //                      value = double.Parse(xmlNodeTopVariable.Attributes["VariableEffectiveBase"].Value);
                //              }
                //              else
                //              {
                //                  if (xmlNodeTopVariable.Attributes["TotalVariableEffectiveBase"] != null)
                //                      value = double.Parse(xmlNodeTopVariable.Attributes["TotalVariableEffectiveBase"].Value);
                //              }
                //          }

                //      }
                //      else
                //      {
                //          if (xmlNodeTopVariable.Attributes["EffectiveBase"] != null)
                //          {
                //              value = double.Parse(xmlNodeTopVariable.Attributes["EffectiveBase"].Value);
                //          }
                //      }

                //          if (xmlNodeTopVariable.Attributes["VariableEffectiveBase"] != null)
                //              _base = double.Parse(xmlNodeTopVariable.Attributes["VariableEffectiveBase"].Value);

                //          if (this.Definition.Settings.BaseType == BaseType.TotalBase && xmlNodeTopVariable.Attributes["TotalVariableEffectiveBase"] != null)
                //              _base = double.Parse(xmlNodeTopVariable.Attributes["TotalVariableEffectiveBase"].Value);

                //      percentage = (value * 100 / _base);

                //      if (renderConfiguration.RenderValue)
                //      {
                //          trValues.Append(string.Format(
                //              "<td {2} style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\">",
                //              rowSpan > 1 ? (CellHeight * rowSpan) : CellHeight,
                //              CellWidth,
                //              rowSpan > 1 ? "rowspan=\"" + rowSpan + "\"" : ""
                //          ));
                //          if (double.IsNaN(value) || double.IsInfinity(value))
                //              value = 0;
                //          trValues.Append(Math.Round(value, this.Definition.Settings.DecimalPlaces) != 0 ? value.ToFormattedString(
                //            this.LanguageManager,
                //            this.Language,
                //            this.Definition.Settings.DecimalPlaces
                //        ) : "-");

                //          trValues.Append("</td>");
                //      }
                //      else if (isNumeric)
                //      {
                //          trPercentage.Append(string.Format(
                //              "<td {2} style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\">",
                //              rowSpan > 1 ? (CellHeight * rowSpan) : CellHeight,
                //              CellWidth,
                //              rowSpan > 1 ? "rowspan=\"" + rowSpan + "\"" : ""
                //          ));
                //          if (double.IsNaN(value) || double.IsInfinity(value))
                //              value = 0;
                //          trPercentage.Append(Math.Round(value, this.Definition.Settings.DecimalPlaces) != 0 ? value.ToFormattedString(
                //          this.LanguageManager,
                //          this.Language,
                //          this.Definition.Settings.DecimalPlaces
                //      ) : "-");

                //          trPercentage.Append("</td>");
                //      }

                //      if (!isNumeric)
                //      {
                //          if (renderConfiguration.RenderPercentage)
                //          {
                //              trPercentage.Append(string.Format(
                //                  "<td style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\">",
                //                  CellHeight,
                //                  CellWidth
                //              ));

                //              if (this.IsExport)
                //              {
                //                  trPercentage.Append(Math.Round(percentage, 9) != 0 && double.IsNaN(percentage) == false ? percentage.ToFormattedString(
                //                    this.LanguageManager,
                //                    this.Language,
                //                    9
                //                ) + " %" : "");
                //              }
                //              else
                //              {
                //                  trPercentage.Append(Math.Round(percentage, this.Definition.Settings.DecimalPlaces) != 0 && double.IsNaN(percentage) == false ? percentage.ToFormattedString(
                //                    this.LanguageManager,
                //                    this.Language,
                //                    this.Definition.Settings.DecimalPlaces
                //                ) + " %" : "");
                //              }
                //              trPercentage.Append("</td>");
                //          }

                //          if (renderConfiguration.RenderSigDiff)
                //          {
                //              trSigDiff.Append(string.Format(
                //                  "<td style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\">",
                //                  CellHeight,
                //                  CellWidth
                //              ));
                //              if (xmlNodeTopVariable.Attributes["SigDiff"] != null)
                //              {
                //                  trSigDiff.Append(xmlNodeTopVariable.Attributes["SigDiff"].Value);
                //              }
                //              trSigDiff.Append("</td>");
                //          }
                //      }
                //  }

                value = 0.0;

                if (renderConfiguration.PowerBIExport == false || GetPropertyFromDefinition(xmlNodeTopVariable, "IsFake") == "True")
                {
                    if (renderConfiguration.CssClass != null && renderConfiguration.CssClass.Contains("TableCellTopBase"))
                    {
                        if (xmlNodeTopVariable.Attributes[renderConfiguration.BasePrefix + "Variable" + renderConfiguration.BaseAttribute] != null)
                        {
                            value = double.Parse(xmlNodeTopVariable.Attributes[renderConfiguration.BasePrefix + "Variable" + renderConfiguration.BaseAttribute].Value);
                        }
                    }
                    else
                    {
                        if (xmlNodeTopVariable.Attributes["Base"] != null)
                        {
                            value = double.Parse(xmlNodeTopVariable.Attributes["Base"].Value);
                        }
                    }

                    if (xmlNodeTopVariable.Attributes["VariableBase"] != null)
                        _base = double.Parse(xmlNodeTopVariable.Attributes["VariableBase"].Value);

                    if (this.Definition.Settings.BaseType == BaseType.TotalBase && xmlNodeTopVariable.Attributes["TotalVariableBase"] != null)
                        _base = double.Parse(xmlNodeTopVariable.Attributes["TotalVariableBase"].Value);
                    percentage = (value * 100 / _base);

                    if (renderConfiguration.RenderValue)
                    {
                        trValues.Append(string.Format(
                            "<td {2} style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\" SigDiffLetter=\"{3}\" SigDiffContext=\"{4}\" >",
                            CellHeight,
                            CellWidth,
                            rowSpan > 1 ? "rowspan=\"" + rowSpan + "\"" : "",
                            "A",
                           sigDiffContextForTotal
                        ));
                        if (double.IsNaN(value) || double.IsInfinity(value))
                            value = 0;

                        trValues.Append(Math.Round(value, this.Definition.Settings.DecimalPlaces) != 0 ? value.ToFormattedString(
                          this.LanguageManager,
                          this.Language,
                          this.Definition.Settings.DecimalPlaces
                      ) : "-");

                        trValues.Append("</td>");
                    }
                    else if (isNumeric)
                    {
                        trPercentage.Append(string.Format(
                            "<td {2} style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\">",
                            CellHeight,
                            CellWidth,
                            rowSpan > 1 ? "rowspan=\"" + rowSpan + "\"" : ""
                        ));
                        if (double.IsNaN(value) || double.IsInfinity(value))
                            value = 0;

                        trPercentage.Append(Math.Round(value, this.Definition.Settings.DecimalPlaces) != 0 ? value.ToFormattedString(
                      this.LanguageManager,
                      this.Language,
                      this.Definition.Settings.DecimalPlaces
                  ) : "-");

                        trPercentage.Append("</td>");
                    }

                    if (!isNumeric)
                    {
                        if (renderConfiguration.RenderPercentage)
                        {
                            trPercentage.Append(string.Format(
                                "<td style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\" SigDiffLetter=\"{2}\" SigDiffContext=\"{3}\">",
                                CellHeight,
                                CellWidth,
                                "A",
                               sigDiffContextForTotal
                            ));

                            if (this.IsExport)
                            {
                                trPercentage.Append(Math.Round(percentage, 9) != 0 && double.IsNaN(percentage) == false ? percentage.ToFormattedString(
                                  this.LanguageManager,
                                  this.Language,
                                  9
                              ) + " %" : "");
                            }
                            else
                            {
                                trPercentage.Append(Math.Round(percentage, this.Definition.Settings.DecimalPlaces) != 0 && double.IsNaN(percentage) == false ? percentage.ToFormattedString(
                                 this.LanguageManager,
                                 this.Language,
                                 this.Definition.Settings.DecimalPlaces
                             ) + " %" : "");
                            }
                            trPercentage.Append("</td>");
                        }

                        if (renderConfiguration.RenderSigDiff)
                        {
                            trSigDiff.Append(string.Format(
                                "<td style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{1}px;max-width:{1}px;min-width:{1}px;\" class=\"TableCellBase TableCellLeftBase\" SigDiffLetter=\"{2}\" SigDiffContext=\"{3}\" onclick=\"ShowSigDiff(this, '#00FF00');\">",
                                CellHeight,
                                CellWidth,
                                "A",
                               sigDiffContextForTotal

                            ));
                            if (xmlNodeTopVariable.Attributes["SigDiff"] != null)
                            {
                                trSigDiff.Append(xmlNodeTopVariable.Attributes["SigDiff"].Value);
                            }
                            trSigDiff.Append("</td>");
                        }
                    }
                }
            }

            XmlNodeList xmlNodesNestedBase = xmlNodeTopVariable.SelectNodes("Variable[@IsNestedBase=\"True\" and @Position=\"Top\"]");

            if (xmlNodesNestedBase.Count != 0)
            {
                foreach (XmlNode xmlNodeNestedBaseVariable in xmlNodesNestedBase)
                {
                    RenderTopVariable(
                        xmlNodeNestedBaseVariable,
                        trValues,
                        trPercentage,
                        trSigDiff,
                        renderConfiguration,
                        idLeftScore
                    );
                }
            }

            // Guid sigDiffContext = Guid.NewGuid();

            if (GetPropertyFromDefinition(xmlNodeTopVariable, "IsFake") == "True")
                return;

            bool isTaxonomy = bool.Parse(GetPropertyFromDefinition(xmlNodeTopVariable, "IsTaxonomy"));

            foreach (XmlNode xmlNodeTopCategory in xmlNodesTopCategories)
            {
                string enabled = GetPropertyFromDefinition(xmlNodeTopCategory, "Enabled");
                if (enabled != null && bool.Parse(enabled) == false)
                    continue;

                if (isTaxonomy && GetPropertyFromDefinition(xmlNodeTopCategory, "Persistent") == "True")
                {
                    if (!this.Definition.HierarchyFilter.TaxonomyCategories.
                        ContainsKey(Guid.Parse(xmlNodeTopCategory.Attributes["Id"].Value)))
                        continue;
                }


                if (this.Definition.Settings.HideEmptyRowsAndColumns && renderConfiguration.PowerBIExport == false)
                {
                    string hasValues = GetPropertyFromDefinition(xmlNodeTopCategory, "HasValues");
                    if (hasValues != null &&
                        bool.Parse(hasValues) == false)
                        continue;
                }

                Guid idScore = Guid.Parse(xmlNodeTopCategory.Attributes["Id"].Value);

                ReportDefinitionRenderConfiguration topVariableRenderConfiguration = renderConfiguration.Clone();

                if (xmlNodeTopCategory.Name == "Mean" ||
                    xmlNodeTopCategory.Name == "Max" ||
                    xmlNodeTopCategory.Name == "Min" ||
                    xmlNodeTopCategory.Name == "StdDev")
                {
                    topVariableRenderConfiguration.RenderSigDiff = false;
                    topVariableRenderConfiguration.RenderPercentage = false;
                }

                if (!renderedCategoryRows.ContainsKey(idScore))
                {
                    RenderTopCategory(
                        xmlNodeTopCategory,
                        renderedCategoryRows,
                        topVariableRenderConfiguration,
                        sigDiffContext,
                        idLeftScore
                    );
                }

                trValues.Append(renderedCategoryRows[idScore][0]);
                trPercentage.Append(renderedCategoryRows[idScore][1]);
                trSigDiff.Append(renderedCategoryRows[idScore][2]);
            }
        }

        public void RenderTopCategory(
            XmlNode xmlNode,
            Dictionary<Guid, List<string>> renderedCategoryRows,
            ReportDefinitionRenderConfiguration renderConfiguration,
            Guid sigDiffContext,
            Guid idLeftScore
        )
        {

            StringBuilder trValues = new StringBuilder();
            StringBuilder trPercentage = new StringBuilder();
            StringBuilder trSigDiff = new StringBuilder();

            XmlNodeList xmlNodesNested = xmlNode.SelectNodes("Variable[not(@IsNestedBase=\"True\") and @Position=\"Top\"]");

            if (xmlNodesNested.Count != 0)
            {
                foreach (XmlNode xmlNodeNestedVariable in xmlNodesNested)
                {
                    RenderTopVariable(
                        xmlNodeNestedVariable,
                        trValues,
                        trPercentage,
                        trSigDiff,
                        renderConfiguration,
                        idLeftScore
                    );
                }

                renderedCategoryRows.Add(Guid.Parse(xmlNode.Attributes["Id"].Value), (new string[]
                {
                    trValues.ToString(),
                    trPercentage.ToString(),
                    trSigDiff.ToString()
                }).ToList());

                return;
            }

            double value = 0.0;
            double _base = 0.0;
            string sigDiffLetter = "";

            if (xmlNode.Attributes[renderConfiguration.ValueAttribute] != null)
                value = double.Parse(xmlNode.Attributes[renderConfiguration.ValueAttribute].Value);

            if (xmlNode.Attributes[renderConfiguration.BaseAttribute] != null)
                _base = double.Parse(xmlNode.Attributes[renderConfiguration.BaseAttribute].Value);

            if (this.Definition.Settings.PercentageBase == PercentageBase.Column)
            {
                if (xmlNode.Attributes[renderConfiguration.BasePrefix + renderConfiguration.BaseAttribute] != null)
                    _base = double.Parse(xmlNode.Attributes[renderConfiguration.BasePrefix + renderConfiguration.BaseAttribute].Value);

                if (renderConfiguration.CssClass != null && renderConfiguration.CssClass.Contains("TableCellTopBase"))
                {
                    value = _base;
                }
            }
            else
            {
                if (xmlNode.ParentNode.Attributes[renderConfiguration.BasePrefix + renderConfiguration.BaseAttribute] != null)
                    _base = double.Parse(xmlNode.ParentNode.Attributes[renderConfiguration.BasePrefix + renderConfiguration.BaseAttribute].Value);
            }

            if (xmlNode.Attributes["SigDiffLetter"] != null)
                sigDiffLetter = xmlNode.Attributes["SigDiffLetter"].Value;

            double percentage = value * 100 / _base;

            string exportAttributes = string.Format(
                " IdLeftScore=\"{0}\" IdCategory=\"{1}\"",
                idLeftScore,
                xmlNode.Attributes["Id"].Value
            );

            string lowBaseWarning = "";

            double thresholdWeight = 0.0;
            if (this.Definition.Settings.BaseType == Crosstables.Classes.BaseType.TotalBase)
            {
                if (this.Definition.Settings.LowBaseConsider == 1 && xmlNode.Attributes["TotalCount"] != null)
                    thresholdWeight = double.Parse(xmlNode.Attributes["TotalCount"].Value);
                else if (this.Definition.Settings.LowBaseConsider == 2 && xmlNode.Attributes["TotalBase"] != null)
                    thresholdWeight = double.Parse(xmlNode.Attributes["TotalBase"].Value);
                else if (this.Definition.Settings.LowBaseConsider == 3 && xmlNode.Attributes["TotalEffectiveBase"] != null)
                    thresholdWeight = double.Parse(xmlNode.Attributes["TotalEffectiveBase"].Value);
            }
            else
            {
                if (this.Definition.Settings.LowBaseConsider == 1 && xmlNode.Attributes["Count"] != null)
                    thresholdWeight = double.Parse(xmlNode.Attributes["Count"].Value);
                else if (this.Definition.Settings.LowBaseConsider == 2 && xmlNode.Attributes["Base"] != null)
                    thresholdWeight = double.Parse(xmlNode.Attributes["Base"].Value);
                else if (this.Definition.Settings.LowBaseConsider == 3 && xmlNode.Attributes["EffectiveBase"] != null)
                    thresholdWeight = double.Parse(xmlNode.Attributes["EffectiveBase"].Value);
            }

            if (this.Definition.Settings.LowBase != 0 && Math.Round(thresholdWeight, 4) <= this.Definition.Settings.LowBase && thresholdWeight != 0)
            {
                lowBaseWarning = "TableCellLowBase";

                if (renderConfiguration.CssClass == null)
                    renderConfiguration.CssClass = "class=\"" + lowBaseWarning + "\"";
                else
                    renderConfiguration.CssClass = renderConfiguration.CssClass.Replace("class=\"", "class=\"" + lowBaseWarning + " ");
            }

            if (renderConfiguration.CssClass != null && renderConfiguration.CssClass.Contains("TableCellTopBase"))
                exportAttributes = "";

            int rowSpan = 0;

            if (renderConfiguration.IsBase == false && (xmlNode.Name == "Mean" ||
                xmlNode.Name == "Max" ||
                xmlNode.Name == "Min" ||
                xmlNode.Name == "StdDev"))
            {
                if (this.Definition.Settings.ShowValues)
                    rowSpan++;
                if (this.Definition.Settings.ShowPercentage)
                    rowSpan++;
                if (this.Definition.Settings.SignificanceTest)
                    rowSpan++;

                if (!renderConfiguration.RenderValue)
                    renderConfiguration.RenderPercentage = true;
            }

            if (renderConfiguration.RenderValue)
            {
                trValues.Append(string.Format(
                   //"<td SigDiffLetter=\"{3}\" {6} SigDiffContext=\"{4}\" style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{5}px;max-width:{5}px;min-width:{5}px;\" {1} {2}>",
                   (renderConfiguration.CssClass != null) ? (renderConfiguration.CssClass.Contains("TableCellLowBase") ? "<td onmouseover=\"ShowToolTip(this,'{7}'+' '+document.getElementById('cphContent_txtLeftPanelSettingsLowBase').value,false,'Bottom');\" SigDiffLetter=\"{3}\" {6} SigDiffContext=\"{4}\" style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{5}px;max-width:{5}px;min-width:{5}px;\" {1} {2}>"
                   : "<td SigDiffLetter=\"{3}\" {6} SigDiffContext=\"{4}\" style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{5}px;max-width:{5}px;min-width:{5}px;\" {1} {2}>")
                   : "<td SigDiffLetter=\"{3}\" {6} SigDiffContext=\"{4}\" style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{5}px;max-width:{5}px;min-width:{5}px;\" {1} {2}>",
                    rowSpan > 1 ? (CellHeight * rowSpan) : CellHeight,
                    renderConfiguration.CssClass,
                    rowSpan > 1 ? "rowspan=\"" + rowSpan + "\"" : "",
                    sigDiffLetter,
                    sigDiffContext,
                    CellWidth,
                    renderConfiguration.RenderPercentage ? "" : exportAttributes,
                    base.LanguageManager.GetText("LowBaseText")
                ));
                if (double.IsNaN(value) || double.IsInfinity(value))
                    value = 0;

                trValues.Append(Math.Round(value, this.Definition.Settings.DecimalPlaces) != 0 ? value.ToFormattedString(
               this.LanguageManager,
               this.Language,
               this.Definition.Settings.DecimalPlaces
           ) : "-");

                trValues.Append("</td>");
            }

            if (renderConfiguration.RenderPercentage)
            {
                trPercentage.Append(string.Format(
                    (renderConfiguration.CssClass != null) ? (renderConfiguration.CssClass.Contains("TableCellLowBase") ? "<td onmouseover=\"ShowToolTip(this,'{7}'+' '+document.getElementById('cphContent_txtLeftPanelSettingsLowBase').value,false,'Bottom');\" SigDiffLetter=\"{1}\" class=\"{6}\" {4} SigDiffContext=\"{2}\" style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{3}px;max-width:{3}px;min-width:{3}px;\" {5}>"
                   : "<td SigDiffLetter=\"{1}\" class=\"{6}\" {4} SigDiffContext=\"{2}\" style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{3}px;max-width:{3}px;min-width:{3}px;\" {5}>")
                   : "<td SigDiffLetter=\"{1}\" class=\"{6}\" {4} SigDiffContext=\"{2}\" style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{3}px;max-width:{3}px;min-width:{3}px;\" {5}>",
                    rowSpan > 1 ? (CellHeight * rowSpan) : CellHeight,
                    sigDiffLetter,
                    sigDiffContext,
                    CellWidth,
                    exportAttributes,
                    rowSpan > 1 ? "rowspan=\"" + rowSpan + "\"" : "",
                    lowBaseWarning,
                    base.LanguageManager.GetText("LowBaseText")
                ));

                if ((xmlNode.Name == "Mean" ||
                    xmlNode.Name == "Max" ||
                    xmlNode.Name == "Min" ||
                    xmlNode.Name == "StdDev"))
                {
                    if (double.IsNaN(value) || double.IsInfinity(value))
                        value = 0;
                    trPercentage.Append(Math.Round(value, this.Definition.Settings.DecimalPlaces) != 0 ? value.ToFormattedString(
                    this.LanguageManager,
                    this.Language,
                    this.Definition.Settings.DecimalPlaces
                ) : "-");

                }
                else
                {
                    if (this.IsExport)
                    {
                        trPercentage.Append(Math.Round(percentage, 9) != 0 && double.IsNaN(percentage) == false ? percentage.ToFormattedString(
                          this.LanguageManager,
                          this.Language,
                          9
                      ) + " %" : "");
                    }
                    else
                    {
                        trPercentage.Append(Math.Round(percentage, this.Definition.Settings.DecimalPlaces) != 0 && double.IsNaN(percentage) == false ? percentage.ToFormattedString(
                          this.LanguageManager,
                          this.Language,
                          this.Definition.Settings.DecimalPlaces
                      ) + " %" : "");
                    }
                }

                trPercentage.Append("</td>");
            }

            if (renderConfiguration.RenderSigDiff)
            {
                trSigDiff.Append(string.Format(
                    "<td class=\"TableCellSigDiff {4}\" style=\"height:{0}px;max-height:{0}px;min-height:{0}px;width:{3}px;max-width:{3}px;min-width:{3}px;\" SigDiffLetter=\"{1}\" SigDiffContext=\"{2}\" onclick=\"ShowSigDiff(this, '#00FF00');\">",
                    CellHeight,
                    sigDiffLetter,
                    sigDiffContext,
                    CellWidth,
                    lowBaseWarning
                ));

                if (xmlNode.Attributes["SigDiff"] != null)
                    trSigDiff.Append(xmlNode.Attributes["SigDiff"].Value);

                trSigDiff.Append("</td>");
            }

            renderedCategoryRows.Add(Guid.Parse(xmlNode.Attributes["Id"].Value), (new string[]
            {
                trValues.ToString(),
                trPercentage.ToString(),
                trSigDiff.ToString()
            }).ToList());

        }

        public void RenderLeftVariableLabels(
            StringBuilder trValues,
            XmlNodeList xmlNodesLeftVariables
        )
        {
            if (xmlNodesLeftVariables.Count == 0)
                return;

            if (!(xmlNodesLeftVariables[0].Attributes["IsFake"] != null && bool.Parse(xmlNodesLeftVariables[0].Attributes["IsFake"].Value) == true))
            {
                string label = string.Join(" &amp; ", xmlNodesLeftVariables.ToArray().
                    Select(x => x.Attributes["Label2057"].Value));

                trValues.Append(string.Format(
                    "<th>{0}</th>",
                    label
                ));

                RenderLeftVariableLabels(
                    trValues,
                    xmlNodesLeftVariables[0].ParentNode.SelectNodes("Variable/Variable")
                );
            }
        }

        public void RenderTopVariableLabels(
            XmlNode xmlNodeTopVariable,
            StringBuilder trValues
        )
        {
            bool isTaxonomy = true;

            if (xmlNodeTopVariable.Attributes["IsTaxonomy"] != null)
                bool.Parse(xmlNodeTopVariable.Attributes["IsTaxonomy"].Value);

            if (xmlNodeTopVariable.Attributes["IsFake"] != null && bool.Parse(xmlNodeTopVariable.Attributes["IsFake"].Value) == true)
            {
                trValues.Append(string.Format(
                    "<th>{0}</th>",
                    "Value"
                ));
            }
            else
            {
                // Run through all category xml nodes of the top variable.
                foreach (XmlNode xmlNodeTopCategory in xmlNodeTopVariable.ChildNodes.OrderByNumeric("Order"))
                {
                    string label = null;

                    if (xmlNodeTopCategory.Attributes["Enabled"] != null && bool.Parse(xmlNodeTopCategory.Attributes["Enabled"].Value) == false)
                        continue;

                    if (isTaxonomy && xmlNodeTopCategory.Attributes["Persistent"] != null &&
                        xmlNodeTopCategory.Attributes["Persistent"].Value == "True")
                    {
                        if (!this.Definition.HierarchyFilter.TaxonomyCategories.
                            ContainsKey(Guid.Parse(xmlNodeTopCategory.Attributes["Id"].Value)))
                            continue;
                    }

                    if (xmlNodeTopCategory.Attributes["Label2057"] != null)
                    {
                        label = xmlNodeTopCategory.Attributes["Label2057"].Value;
                    }
                    else
                    {
                        foreach (XmlAttribute attribute in xmlNodeTopCategory.Attributes)
                        {
                            if (attribute.Name.StartsWith("Label"))
                            {
                                label = attribute.Value;
                                break;
                            }
                        }

                        if (label == null)
                        {
                            if (xmlNodeTopCategory.Attributes["Name"] != null)
                                label = xmlNodeTopCategory.Attributes["Name"].Value;
                            else
                                label = "";
                        }
                    }

                    trValues.Append(string.Format(
                        "<th>{0}</th>",
                        "&nbsp;" + label
                    ));
                }
            }
        }

        private void CalculateCellDimensions()
        {
            int totalTopCategoryCount = 0;
            int totalLeftCategoryCount = 0;

            int maxTopNestedLevel = 0;
            int maxLeftNestedLevel = 0;

            foreach (ReportDefinitionVariable topVariable in this.Definition.TopVariables)
            {
                totalTopCategoryCount += GetTotalCategoryCount(topVariable);

                if (topVariable.NestedLevels > maxTopNestedLevel)
                    maxTopNestedLevel = topVariable.NestedLevels;
            }

            foreach (ReportDefinitionVariable leftVariable in this.Definition.LeftVariables)
            {
                totalLeftCategoryCount += GetTotalCategoryCount(leftVariable, true);

                if (leftVariable.NestedLevels > maxLeftNestedLevel)
                    maxLeftNestedLevel = leftVariable.NestedLevels;
            }

            //cellWidth = this.ReportDefinition.Settings.TableWidth;
            //cellHeight = this.ReportDefinition.Settings.TableHeight;
            CellWidth = this.ContentWidth - 40;
            CellHeight = this.ContentHeight - 175;

            int topHeadlineHeight = (103 + ((maxTopNestedLevel - 1) * 101) + (this.Definition.SignificanceTest ? 20 : 0));
            int leftHeadlineWidth = maxLeftNestedLevel * 120;

            // Left headline.
            CellWidth -= leftHeadlineWidth;

            // Top headline.
            //cellHeight -= (maxTopNestedLevel - 1) * (this.ReportDefinition.SignificanceTest ? 123 : 102);
            CellHeight -= topHeadlineHeight;

            int scrollBarThickness = 8;

            if (HttpContext.Current != null &&
                (HttpContext.Current.Request.Browser.Browser.Contains("InternetExplorer") ||
                HttpContext.Current.Request.Browser.Browser.Contains("IE")))
            {
                scrollBarThickness = 16;
            }

            // Scroll bar on right hand side.
            CellWidth -= scrollBarThickness;

            int maxWidth = CellWidth + 5;
            int maxHeight = CellHeight + scrollBarThickness;

            // Border on the right
            CellWidth -= 10;
            CellHeight -= 5;

            // Devide the cell width by the ammount of columns to
            // get the width for each individual column.
            if (totalTopCategoryCount != 0)
                CellWidth /= totalTopCategoryCount;

            // Devide the cell height by the ammount of rows to
            // get the height for each individual row.
            if (totalLeftCategoryCount != 0)
                CellHeight /= totalLeftCategoryCount;

            // Check if the calculated cell width is smaller than
            // the maximum width. If yes a scroll bar appears.
            if (CellWidth < this.Definition.Settings.MinWidth)
                CellWidth = this.Definition.Settings.MinWidth;

            // Check if the calculated cell height is smaller than
            // the maximum height. If yes a scroll bar appears.
            if (CellHeight < this.Definition.Settings.MinHeight)
                CellHeight = this.Definition.Settings.MinHeight;

            CellWidth -= 1;
            CellHeight -= 1;
        }


        private int GetTotalCategoryCount(ReportDefinitionVariable variable, bool timesRowSpan = false)
        {
            int result = 0;

            int baseValueCells = 1;

            if (this.Definition.Settings.DisplayUnweightedBase)
                baseValueCells = 2;

            if (this.Definition.Settings.DisplayEffectiveBase)
                baseValueCells = 2;

            if (variable.NestedVariables.Count == 0)
            {
                int count = 0;

                if (variable.VariableType == VariableType.Numeric)
                    count += 4;
                else
                {
                    count += variable.ScoresCount;

                    if (timesRowSpan)
                    {
                        int rows = 0;
                        if (this.Definition.SignificanceTest)
                            rows += 1;

                        if (this.Definition.Settings.ShowValues)
                            rows += 1;

                        if (this.Definition.Settings.ShowPercentage)
                            rows += 1;

                        /*if (this.ReportDefinition.SignificanceTest)
                            count *= 3;
                        else
                            count *= 2;*/
                        count *= rows;
                    }

                    if (timesRowSpan && variable.Scale && this.Definition.SignificanceTest)
                    {
                        if (variable.ParentVariable == null)
                            count += 2;
                        else
                            count += 1;
                    }
                }

                count += baseValueCells;

                result += count;
            }
            else
            {
                int count = variable.ScoresCount + 1;

                /*if (timesRowSpan && variable.Scale && this.ReportDefinition.SignificanceTest)
                    count++;*/

                for (int i = 0; i < count; i++)
                {
                    // Run through all nested variables.
                    foreach (ReportDefinitionVariable nestedVariable in variable.NestedVariables)
                    {
                        result += GetTotalCategoryCount(nestedVariable, timesRowSpan);
                    }
                }

                if (timesRowSpan && variable.Scale && this.Definition.SignificanceTest)
                {
                    // Run through all nested variables.
                    foreach (ReportDefinitionVariable nestedVariable in variable.NestedVariables)
                    {
                        result += GetTotalCategoryCount(nestedVariable, false);
                    }
                }
            }

            return result;
        }


        private string GetPropertyFromDefinition(XmlNode xmlNode, string property, string defaultReturn = null)
        {
            if (!this.DefinitionXmlNodeMapping.ContainsKey(xmlNode.Attributes["Id"].Value))
            {
                string xPath = "//";

                if (xmlNode.Name != "Variable")
                {
                    if (xmlNode.ParentNode.Name == "Variable")
                    {
                        xPath += string.Format(
                            "{0}[@Id=\"{1}\" and @Position=\"{2}\"]/",
                            xmlNode.ParentNode.Name,
                            xmlNode.ParentNode.Attributes["Id"].Value,
                            xmlNode.ParentNode.Attributes["Position"].Value
                        );
                    }
                    else
                    {
                        xPath += string.Format(
                            "{0}[@Id=\"{1}\" and @Position=\"{2}\"]/",
                            xmlNode.ParentNode.ParentNode.Name,
                            xmlNode.ParentNode.ParentNode.Attributes["Id"].Value,
                            xmlNode.ParentNode.ParentNode.Attributes["Position"].Value
                        );
                    }
                }

                xPath += string.Format(
                    "{0}[@Id=\"{1}\"]",
                    xmlNode.Name,
                    xmlNode.Attributes["Id"].Value
                );

                // Needed cause of asynch tasks......
                try
                {
                    if (!this.DefinitionXmlNodeMapping.ContainsKey(xmlNode.Attributes["Id"].Value))
                        this.DefinitionXmlNodeMapping.Add(xmlNode.Attributes["Id"].Value, this.Definition.XmlDocument.SelectSingleNode(xPath));
                }
                catch
                { }
            }

            XmlNode xmlNodeDefinition = this.DefinitionXmlNodeMapping[xmlNode.Attributes["Id"].Value];

            if (xmlNodeDefinition == null || xmlNodeDefinition.Attributes[property] == null)
                return defaultReturn;

            return xmlNodeDefinition.Attributes[property].Value;
        }

        #endregion
    }

    public class ReportDefinitionRenderConfiguration
    {
        #region Properties

        public bool IsBase { get; set; }

        public bool RenderValue { get; set; }
        public bool RenderPercentage { get; set; }
        public bool RenderSigDiff { get; set; }
        public bool RenderMeanScore { get; set; }
        public bool RenderLeftBase { get; set; }
        public bool PowerBIExport { get; set; }

        public string ValueAttribute { get; set; }
        public string BaseAttribute { get; set; }
        public string BasePrefix { get; set; }

        public int ValueParents { get; set; }
        public int BaseParents { get; set; }

        public string CssClass { get; set; }

        #endregion


        #region Constructor

        public ReportDefinitionRenderConfiguration()
        {
            this.RenderValue = true;
            this.RenderPercentage = true;
            this.RenderSigDiff = true;
            this.RenderMeanScore = true;
            this.ValueAttribute = "Value";
            this.BaseAttribute = "Base";
            this.BasePrefix = "";
        }

        public ReportDefinitionRenderConfiguration(
            bool renderValue,
            bool renderPercentage,
            bool renderSigDiff,
            bool renderMeanScore
        )
            : this()
        {
            this.RenderValue = renderValue;
            this.RenderPercentage = renderPercentage;
            this.RenderSigDiff = renderSigDiff;
            this.RenderMeanScore = renderMeanScore;
        }

        public ReportDefinitionRenderConfiguration(
            bool renderValue,
            bool renderPercentage,
            bool renderSigDiff,
            string valueAttribute,
            string baseAttribute
        )
            : this()
        {
            this.RenderValue = renderValue;
            this.RenderPercentage = renderPercentage;
            this.RenderSigDiff = renderSigDiff;
            this.ValueAttribute = valueAttribute;
            this.BaseAttribute = baseAttribute;
        }

        public ReportDefinitionRenderConfiguration(
            bool renderValue,
            bool renderPercentage,
            bool renderSigDiff,
            string valueAttribute,
            string baseAttribute,
            string cssClass,
            bool isBase
        )
        {
            this.RenderValue = renderValue;
            this.RenderPercentage = renderPercentage;
            this.RenderSigDiff = renderSigDiff;

            this.ValueAttribute = valueAttribute;
            this.BaseAttribute = baseAttribute;
            this.BasePrefix = "";

            this.CssClass = cssClass;

            this.IsBase = true;
        }

        #endregion


        #region Methods

        public ReportDefinitionRenderConfiguration Clone()
        {
            ReportDefinitionRenderConfiguration result = new ReportDefinitionRenderConfiguration();

            result.IsBase = this.IsBase;

            result.RenderValue = this.RenderValue;
            result.RenderPercentage = this.RenderPercentage;
            result.RenderSigDiff = this.RenderSigDiff;
            result.RenderLeftBase = this.RenderLeftBase;
            result.PowerBIExport = this.PowerBIExport;
            result.RenderMeanScore = this.RenderMeanScore;

            result.ValueAttribute = this.ValueAttribute;
            result.BaseAttribute = this.BaseAttribute;
            result.BasePrefix = this.BasePrefix;

            result.ValueParents = this.ValueParents;
            result.BaseParents = this.BaseParents;

            result.CssClass = this.CssClass;

            return result;
        }

        #endregion
    }
}
