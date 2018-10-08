using Crosstables.Classes.ReportDefinitionClasses;
using DatabaseCore.Items;
using LinkBi1.Classes;
using LinkBi1.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkOnline.Pages.LinkBi
{
    public partial class LinkBi : WebUtilities.BasePage
    {
        #region Properties

        public LinkBiDefinition LinkBiDefinition { get; set; }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void BindFilters()
        {
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "InitFilterView",
                "loadFunctions.push(function() {UpdateFilterView('" + this.LinkBiDefinition.FileName.Replace("\\", "/") + "')});",
                true
            );

            /*// Run through all applied filter categories.
            foreach (FilterCategoryOperator filterCategoryOperator in this.LinkBiDefinition.FilterCategories)
            {
                //filterCategoryOperator.OnChange += filterCategoryOperator_OnChange;

                pnlFilterCategories.Controls.Add(
                    filterCategoryOperator
                );
            }*/
        }

        private void BindSettings()
        {

            if (string.IsNullOrEmpty(this.LinkBiDefinition.Properties.Name) == false)
            {
                lblDefinitionName.Text = this.LinkBiDefinition.Properties.Name;
                //pnlRightPanelConnectPowerBI.Visible = true;

                if (Global.User.HasPermission(Global.PermissionCore.Permissions["PowerBi"]))
                {
                    pnlRightPanelConnectPowerBI.Visible = true;
                }
                else
                {
                    pnlRightPanelConnectPowerBI.Visible = false;
                }

                pnlRightPanelConnectPowerBI.Attributes.Add("onclick", string.Format(
                    "ConnectPowerBI('{0}', 'LinkBi');",
                    new FileInfo(this.LinkBiDefinition.FileName).Name.Replace(".xml", "")
                ));
            }
            else
            {
                pnlRightPanelConnectPowerBI.Visible = false;
                btnBack.Visible = false;
                lblDefinitionName.Text = Global.LanguageManager.GetText("NewLinkBiReport");
            }

            chkLeftPanelSettingsExportPercentage.Checked = false;

            if (this.LinkBiDefinition.Settings.ExportPercentage)
            {
                chkLeftPanelSettingsExportPercentage.Checked = true;
            }

            chkLeftPanelSettingsDisplayUnweightedBase.Checked = false;

            if (this.LinkBiDefinition.Settings.DisplayUnweightedBase)
            {
                chkLeftPanelSettingsDisplayUnweightedBase.Checked = true;
            }

            // Get all available translations of the taxonomy
            List<object[]> languages = Global.Core.TaxonomyVariableLabels.
                ExecuteReader("SELECT DISTINCT IdLanguage FROM TaxonomyVariableLabels");

            // Run through all available translations.
            foreach (object[] language in languages)
            {
                CultureInfo cultureInfo = new CultureInfo((int)language[0]);

                ListItem lItem = new ListItem();
                lItem.Text = cultureInfo.DisplayName;
                lItem.Value = cultureInfo.LCID.ToString();

                ddlLeftPanelSettingsMetadataLanguage.Items.Add(lItem);
            }

            ddlLeftPanelSettingsMetadataLanguage.SelectedValue = this.LinkBiDefinition.Settings.IdLanguage.ToString();

            VariableSearch.Source = this.LinkBiDefinition.FileName.Replace("\\", "/");
            VariableSearch.IdLanguage = this.LinkBiDefinition.Settings.IdLanguage;
            VariableSearch.DataCheckClientId = chkLeftPanelSettingsDataCheckEnabled.ClientID;
        }


        private void InitToolsMenu()
        {
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "LinkBiSettingsMenu.xml"
            );

            WebUtilities.NavigationMenu navReporterTools = new WebUtilities.NavigationMenu(
                "navLinkBiTools",
                fileName
            );

            pnlLinkBiTools.Controls.Add(navReporterTools);
        }

        private void InitLinkBiDefinition()
        {
            if (HttpContext.Current.Session["LinkBiDefinition"] == null || File.Exists(HttpContext.Current.Session["LinkBiDefinition"].ToString()) == false)
            {
                string fileName = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "LinkBiDefinitions",
                    Global.Core.ClientName,
                    Global.User.Id + ".xml"
                );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                // Check if the LinkBi definition exists.
                if (!File.Exists(fileName))
                {
                    File.Copy(Path.Combine(
                        Request.PhysicalApplicationPath,
                        "App_Data",
                        "LinkBiDefinition.xml"
                    ), fileName);

                    InitWorkflow(fileName);
                }

                HttpContext.Current.Session["LinkBiDefinition"] = fileName;
            }


            HierarchySelector.FileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "HierarchySelectors",
                Global.Core.ClientName + ".xml"
            );

            HierarchySelector.Source = HttpContext.Current.Session["LinkBiDefinition"].ToString();
            EquationDefinition.Source = HierarchySelector.Source;
            csFilterDefinition.Source = HierarchySelector.Source;

            this.LinkBiDefinition = new LinkBiDefinition(
                Global.Core,
                HttpContext.Current.Session["LinkBiDefinition"].ToString(),
                Global.HierarchyFilters[HttpContext.Current.Session["LinkBiDefinition"].ToString()]
            );

            if (HierarchySelector.Exists)
            {
                HierarchySelector.Parse();

                int optionCount = 0;

                foreach (Classes.Controls.HierarchySelectorSection section in HierarchySelector.Sections.Values)
                {
                    optionCount += section.OptionCount;
                }

                if (optionCount <= 1)
                {
                    pnlRightPanelHierarchy.Visible = false;
                }
                else
                {
                    pnlRightPanelHierarchy.Attributes.Add("onclick", string.Format(
                        "InitDragBox('boxHierarchySelectorControl');LoadHierarchySelectedItems('{0}');",
                        HierarchySelector.Source
                    ));

                    if (this.LinkBiDefinition.XmlDocument.DocumentElement.SelectSingleNode("HierarchyFilter") == null)
                    {
                        Page.ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "ShowHierarchySelector",
                            string.Format("InitDragBox('boxHierarchySelectorControl');LoadHierarchySelectedItems('{0}');", HierarchySelector.Source),
                            true
                        );
                    }
                }
            }
            else
            {
                pnlRightPanelHierarchy.Visible = false;
            }



            if (this.LinkBiDefinition.Dimensions.Count > 0 &&
                this.LinkBiDefinition.Measures.Count > 0)
            {
                pnlGoButton.CssClass = "GoButton2 GreenBackground2I";
                pnlGoButton.Attributes.Add("onclick", "window.location='SelectInterface.aspx';");
            }
            else
            {
                pnlGoButton.CssClass = "GoButton2";
            }
        }

        private void InitWorkflow(string fileName)
        {
            /*string fileNameWorkflow = Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "ReportingWorkflows",
                Global.Core.ClientName + ".xml"
            );

            XmlDocument xmlDocumentWorkflow = new XmlDocument();
            xmlDocumentWorkflow.Load(fileNameWorkflow);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);

            XmlNode xmlNodeWorkflow = xmlDocument.DocumentElement.SelectSingleNode("Workflow");
            
            xmlNodeWorkflow.InnerXml = xmlDocumentWorkflow.DocumentElement.InnerXml;*/

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);

            string fileNameWeighting = Path.Combine(
                Request.PhysicalApplicationPath,
                "App_Data",
                "WeightingDefaults",
                Global.Core.ClientName + ".xml"
            );

            if (File.Exists(fileNameWeighting))
            {
                XmlDocument xmlDocumentWeighting = new XmlDocument();
                xmlDocumentWeighting.Load(fileNameWeighting);

                XmlNode xmlNodeWeighting = xmlDocument.DocumentElement.SelectSingleNode("WeightingVariables");

                xmlNodeWeighting.InnerXml = xmlDocumentWeighting.DocumentElement.InnerXml;
            }

            xmlDocument.Save(fileName);
        }


        private void RenderLinkBiDefinition()
        {
            Table table = new Table();
            table.Attributes.Add("id", "TableLinkBiDefinition");
            table.CssClass = "TableLinkBiDefinition BackgroundColor7";
            table.CellPadding = 0;
            table.CellSpacing = 0;

            pnlLinkBiDefinition.Controls.Add(table);

            TableRow tableRowHeadline = new TableRow();
            tableRowHeadline.CssClass = "TableRowHeadline Color1";

            TableCell tableCellIcons = new TableCell();
            TableCell tableCellHeadlineDimensions = new TableCell();
            TableCell tableCellHeadlineMeasures = new TableCell();

            tableCellIcons.CssClass = "TableCellHeadlineIcons";
            tableCellHeadlineDimensions.CssClass = "TableCellHeadlineDimensions";
            tableCellHeadlineMeasures.CssClass = "TableCellHeadlineMeasures";

            tableCellIcons.Controls.Add(RenderTopLeftButtons(100));
            tableCellHeadlineDimensions.Text = Global.LanguageManager.GetText("Dimensions");
            tableCellHeadlineMeasures.Text = Global.LanguageManager.GetText("Measures");

            tableCellHeadlineDimensions.Attributes.Add("onclick", string.Format(
                "InitDragBox('boxVariableSearchControl');SearchVariables('{0}', '{1}');",
                this.LinkBiDefinition.FileName.Replace("\\", "/"),
                this.LinkBiDefinition.Settings.IdLanguage
            ));

            tableCellHeadlineMeasures.Attributes.Add("onclick", string.Format(
                "InitDragBox('boxVariableSearchControl');SearchVariables('{0}', '{1}');",
                this.LinkBiDefinition.FileName.Replace("\\", "/"),
                this.LinkBiDefinition.Settings.IdLanguage
            ));

            tableRowHeadline.Cells.Add(tableCellIcons);
            tableRowHeadline.Cells.Add(tableCellHeadlineDimensions);
            tableRowHeadline.Cells.Add(tableCellHeadlineMeasures);

            table.Rows.Add(tableRowHeadline);

            TableRow tableRow = new TableRow();
            tableRow.VerticalAlign = VerticalAlign.Top;

            table.Rows.Add(tableRow);

            TableCell tableCellDimensions = new TableCell();
            TableCell tableCellMeasures = new TableCell();

            tableCellDimensions.ColumnSpan = 2;

            tableCellDimensions.CssClass = "TableCellDimension PnlVariableSelectors";
            tableCellMeasures.CssClass = "TableCellMeasure PnlVariableSelectors";

            tableRow.Cells.Add(tableCellDimensions);
            tableRow.Cells.Add(tableCellMeasures);

            Panel pnlDimensions = new Panel();
            Panel pnlMeasures = new Panel();

            pnlDimensions.CssClass = "PanelDimensions";
            pnlMeasures.CssClass = "PanelMeasures";

            pnlDimensions.Attributes.Add("DropArea", "True");
            pnlMeasures.Attributes.Add("DropArea", "True");

            pnlDimensions.Attributes.Add("DropAreaTypeRestriction", string.Join(",", new string[] {
                VariableType.Single.ToString(),
                VariableType.Multi.ToString()
            }));

            pnlDimensions.Attributes.Add("Source", this.LinkBiDefinition.FileName.Replace("\\", "/"));
            pnlMeasures.Attributes.Add("Source", this.LinkBiDefinition.FileName.Replace("\\", "/"));

            pnlDimensions.Attributes.Add("Path", "Report/Dimensions");
            pnlMeasures.Attributes.Add("Path", "Report/Measures");

            tableCellDimensions.Controls.Add(pnlDimensions);
            tableCellMeasures.Controls.Add(pnlMeasures);

            // Run through all dimensions of the definition.
            foreach (LinkBiDefinitionDimension dimension in this.LinkBiDefinition.Dimensions)
            {
                VariableSelector1.Classes.VariableSelector ddlVariable = new VariableSelector1.Classes.VariableSelector(
                    this.LinkBiDefinition.Settings.IdLanguage, new VariableSelector1.Classes.DefinitionObject(
                    Global.Core,
                    this.LinkBiDefinition.FileName,
                    dimension.XmlNode
                ), false);
                ddlVariable.ID = "ddlDimension" + dimension.Identity;

                pnlDimensions.Controls.Add(ddlVariable);
            }

            int i = 0;
            // Run through all measures of the definition.
            foreach (LinkBiDefinitionDimension measure in this.LinkBiDefinition.Measures)
            {
                VariableSelector1.Classes.VariableSelector ddlVariable = new VariableSelector1.Classes.VariableSelector(
                    this.LinkBiDefinition.Settings.IdLanguage, new VariableSelector1.Classes.DefinitionObject(
                    Global.Core,
                    this.LinkBiDefinition.FileName,
                    measure.XmlNode
                ), false);
                ddlVariable.ID = "ddlMeasure" + measure.Identity;

                pnlMeasures.Controls.Add(ddlVariable);


                i++;
            }

            if (this.LinkBiDefinition.Workflow.Selections.Count > 0)
                pnlWorkflow.Controls.Add(this.LinkBiDefinition.Workflow);
            else
                pnlWorkflowContainer.Visible = false;
        }

        private Table RenderTopLeftButtons(int topHeadlineHeight)
        {
            Table table = new Table();
            table.CssClass = "TableTopLeftButtons";
            table.Style.Add("height", (topHeadlineHeight - 1) + "px");
            table.CellPadding = 0;
            table.CellSpacing = 0;

            TableRow tableRow1 = new TableRow();
            TableRow tableRow2 = new TableRow();

            TableCell tableCellSelectVariable = new TableCell();
            TableCell tableCellWeighting = new TableCell();
            TableCell tableCellFilter = new TableCell();
            TableCell tableCellSwitch = new TableCell();

            tableCellSelectVariable.Attributes.Add("onmouseover", string.Format(
                "ShowToolTip(this, '{0}', false, 'Bottom');",
                Global.LanguageManager.GetText("ToolTipSelectVariable")
            ));

            tableCellWeighting.Attributes.Add("onmouseover", string.Format(
                "ShowToolTip(this, '{0}', false, 'Bottom');",
                Global.LanguageManager.GetText("ToolTipWeighting")
            ));

            tableCellSwitch.Attributes.Add("onmouseover", string.Format(
                "ShowToolTip(this, '{0}', false, 'Bottom');",
                Global.LanguageManager.GetText("ToolTipSwitchLinkBi")
            ));

            tableCellFilter.ID = "pnlFilter";

            tableCellWeighting.ID = "pnlWeighting";

            tableCellFilter.Attributes.Add("onmouseover", string.Format(
                "ShowToolTip(this, '{0}', false, 'Bottom');OverFilter();",
                Global.LanguageManager.GetText("ToolTipFilter")
            ));

            tableCellSelectVariable.CssClass = "BackgroundColor7H1";
            tableCellWeighting.CssClass = "BackgroundColor7H1";
            tableCellFilter.CssClass = "BackgroundColor7H1";
            tableCellSwitch.CssClass = "BackgroundColor7H1";

            tableCellSelectVariable.Style.Add("background-image", "url('/Images/Icons/SelectVariable.png')");
            tableCellWeighting.Style.Add("background-image", "url('/Images/Icons/Weighting.png')");
            tableCellFilter.Style.Add("background-image", "url('/Images/Icons/Filter.png')");
            tableCellSwitch.Style.Add("background-image", "url('/Images/Icons/Switch.png')");

            if (this.LinkBiDefinition.FilterCategories.Count > 0 && this.LinkBiDefinition.FilterCategories[0].FiltersApplied)
            {
                tableCellFilter.Style["background-image"] = "url('/Images/Icons/Filter_Active.png')";
                tableCellFilter.CssClass = "GreenBackground3";
            }

            if (this.LinkBiDefinition.WeightingFilters.DefaultWeighting.HasValue || this.LinkBiDefinition.WeightingFilters.Length > 0)
            {
                tableCellWeighting.Style["background-image"] = "url('/Images/Icons/Weighting_Active.png')";
                tableCellWeighting.CssClass = "GreenBackground3";
            }

            tableCellSelectVariable.Style.Add("background-size", "50px");
            tableCellWeighting.Style.Add("background-size", "50px");
            tableCellFilter.Style.Add("background-size", "50px");
            tableCellSwitch.Style.Add("background-size", "50px");

            tableCellSelectVariable.Attributes.Add("onclick", "document.getElementById('variableSearchResults').style.height=(window.innerHeight - 500) + 'px';InitDragBox('boxVariableSearchControl');window.setTimeout(function() { SearchVariables('" + this.LinkBiDefinition.FileName.Replace("\\", "/") + "', '" + this.LinkBiDefinition.Settings.IdLanguage + "', " + true.ToString().ToLower() + "); }, 500);document.getElementById('cphContent_VariableSearch_txtVariableSearch').focus();");
            tableCellWeighting.Attributes.Add("onclick", "ShowDefaultWeightingSelector(this);");
            tableCellFilter.Attributes.Add("onclick", "InitDragBox('boxFilterDefinitionControl');");
            tableCellSwitch.Attributes.Add("onclick", "SwitchLinkBiDefinition();");

            tableRow1.Cells.Add(tableCellSelectVariable);
            tableRow1.Cells.Add(tableCellWeighting);

            tableRow2.Cells.Add(tableCellFilter);
            tableRow2.Cells.Add(tableCellSwitch);

            table.Rows.Add(tableRow1);
            table.Rows.Add(tableRow2);

            return table;
        }

        private void BindWeightingDefinition()
        {
            ddlDefaultWeighting.Items.Add(new ListItem()
            {
                Text = Global.LanguageManager.GetText("None"),
                Value = (new Guid()).ToString()
            });

            // Get all as weighting variable defined variables.
            List<object[]> weightingVariables = Global.Core.TaxonomyVariables.GetValues(
                new string[] { "Id" },
                new string[] { "Weight" },
                new object[] { true }
            );

            // Run through all weighting variables.
            foreach (object[] weightingVariable in weightingVariables)
            {
                // Create a new list item for the weighting variable.
                ddlDefaultWeighting.Items.Add(new ListItem()
                {
                    Value = weightingVariable[0].ToString(),
                    Text = (string)Global.Core.TaxonomyVariableLabels.GetValue(
                        "Label",
                        new string[] { "IdTaxonomyVariable", "IdLanguage" },
                        new object[] { weightingVariable[0], this.LinkBiDefinition.Settings.IdLanguage }
                    )
                });
            }

            if (this.LinkBiDefinition.WeightingFilters.DefaultWeighting.HasValue)
                ddlDefaultWeighting.SelectedValue = this.LinkBiDefinition.WeightingFilters.DefaultWeighting.Value.ToString();

            ddlDefaultWeighting.Attributes.Add(
                "onchange",
                "UpdateLinkBiWeightingVariable(this.value)"
            );
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            InitToolsMenu();
            InitLinkBiDefinition();
            BindFilters();
            BindSettings();
            RenderLinkBiDefinition();

            if (!this.IsPostBack)
            {
                BindWeightingDefinition();
            }

            if (this.LinkBiDefinition.Dimensions.Count > 0 && this.LinkBiDefinition.Measures.Count > 0)
                pnlGoButton.Visible = true;

            boxFilterDefinition.Visible = true;
            boxFilterSearch.Visible = true;
            boxSettings.Visible = true;
        }


        protected void lnkLeftPanelSectionSave_Click(object sender, EventArgs e)
        {

        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            string fileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "LinkBiDefinitions",
                Global.Core.ClientName,
                Global.User.Id + ".xml"
            );

            HttpContext.Current.Session["LinkBiDefinition"] = fileName;

            // Redirect to the saved reports page.
            Response.Redirect("SavedDefinitions.aspx");
        }

        #endregion
    }
}