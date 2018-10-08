using Crosstables.Classes.HierarchyClasses;
using DatabaseCore.Items;
using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebUtilities.Controls;

namespace Crosstables.Classes.WorkflowClasses
{
    public class Workflow : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the report definition
        /// where the workflow is part of.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the full path to
        /// the workflow definition file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the used database core.
        /// </summary>
        public DatabaseCore.Core Core { get; set; }

        public HierarchyFilter HierarchyFilter { get; set; }

        /// <summary>
        /// Gets or sets the xml node that
        /// contains the workflow definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets a collection
        /// of the workflow's selections.
        /// </summary>
        public Dictionary<string, WorkflowSelection> Selections { get; set; }

        /// <summary>
        /// Gets or sets if the workflow is editable.
        /// </summary>
        public bool Editable { get; set; }

        public string Service { get; set; }

        #endregion


        #region Constructor

        public Workflow(
            DatabaseCore.Core core, 
            string source, 
            XmlNode xmlNode, 
            string service,
            HierarchyFilter hierarchyFilter
        )
        {
            this.HierarchyFilter = hierarchyFilter;
            this.Source = source;
            this.Editable = false;
            this.Selections = new Dictionary<string, WorkflowSelection>();
            this.Service = service;
            this.Core = core;

            this.XmlNode = xmlNode;

            // Set the on load event of the web control.
            this.Load += Workflow_Load;

            Parse();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Gets the respondents that apply on a list of categories.
        /// </summary>
        /// <param name="categories">The categories to filter on.</param>
        /// <param name="respondents">A previous applied filter, to filter the categories on.</param>
        /// <returns></returns>
        private Data FilterCategories(
            DataCore.Classes.StorageMethods.Database storageMethod,
            bool aggregateNonQAData,
            List<KeyValuePair<Guid, Guid>> categories,
            bool isTaxonomy,
            Data respondents = null
        )
        {
            foreach (KeyValuePair<Guid, Guid> filterCategory in categories)
            {
                respondents = storageMethod.GetRespondents(
                    filterCategory.Key,
                    filterCategory.Value,
                    isTaxonomy,
                    this.Core.CaseDataLocation,
                    respondents,
                    null,
                    null
                );
            }

            return respondents;
        }

        public Data GetWorkflowFilter(
            DataCore.Classes.StorageMethods.Database storageMethod,
            bool aggregateNonQAData
        )
        {
            Data result = null;

            foreach (WorkflowSelection workflowSelection in this.Selections.Values)
            {
                Data workflowFilter = new Data();

                foreach (WorkflowSelectionSelector workflowSelectionVariable in workflowSelection.SelectionVariables.Values)
                {
                    switch (workflowSelectionVariable.GetType().Name)
                    {
                        case "WorkflowSelectionHierarchy":
                                continue;
                        case "WorkflowSelectionVariable":
                            if (workflowSelectionVariable.Selector.SelectedItems.Count == 0)
                                continue;

                            //Guid idVariable = (Guid)this.Core.Categories.GetValue("IdVariable", "Id", idCategory);
                            Guid idVariable = ((WorkflowSelectionVariable)workflowSelectionVariable).IdVariable;
                            bool isTaxonomy = ((WorkflowSelectionVariable)workflowSelectionVariable).IsTaxonomy;

                            foreach (Guid idCategory in workflowSelectionVariable.Selector.SelectedItems)
                            {
                                // Check if the category is a score group.
                                bool isScoreGroup = (bool)this.Core.TaxonomyCategories.GetValue(
                                    "IsScoreGroup",
                                    "Id",
                                    idCategory
                                );

                                if (isScoreGroup)
                                {
                                    // Get all the categories of the score group.
                                    List<object[]> scoreGroupCategories = this.Core.TaxonomyCategoryLinks.GetValues(
                                        new string[] { "IdTaxonomyCategory" },
                                        new string[] { "IdScoreGroup" },
                                        new object[] { idCategory }
                                    );

                                    // Run through all categories of the score group.
                                    foreach (object[] scoreGroupCategory in scoreGroupCategories)
                                    {
                                        List<KeyValuePair<Guid, Guid>> categories = new List<KeyValuePair<Guid, Guid>>();
                                        categories.Add(new KeyValuePair<Guid, Guid>((Guid)scoreGroupCategory[0], idVariable));

                                        Data categoryRespondents = FilterCategories(
                                            storageMethod,
                                            aggregateNonQAData,
                                            categories,
                                            isTaxonomy,
                                            result
                                        );

                                        // Run though all respondents of the weighting selection variable.
                                        foreach (KeyValuePair<Guid, double[]> respondent in categoryRespondents.Responses)
                                        {
                                            if (workflowFilter.Responses.ContainsKey(respondent.Key))
                                                continue;

                                            workflowFilter.Responses.Add(respondent.Key, respondent.Value);
                                        }
                                    }
                                }
                                else
                                {
                                    List<KeyValuePair<Guid, Guid>> categories = new List<KeyValuePair<Guid, Guid>>();
                                    categories.Add(new KeyValuePair<Guid, Guid>(idCategory, idVariable));

                                    Data categoryRespondents = FilterCategories(
                                        storageMethod,
                                        aggregateNonQAData,
                                        categories,
                                        isTaxonomy,
                                        result
                                    );

                                    // Run though all respondents of the weighting selection variable.
                                    foreach (KeyValuePair<Guid, double[]> respondent in categoryRespondents.Responses)
                                    {
                                        if (workflowFilter.Responses.ContainsKey(respondent.Key))
                                            continue;

                                        workflowFilter.Responses.Add(respondent.Key, respondent.Value);
                                    }
                                }
                            }
                            break;
                        case "WorkflowSelectionProject":
                            if (workflowSelectionVariable.Selector.SelectedItems.Count == 0)
                                continue;

                            foreach (Guid idStudy in workflowSelectionVariable.Selector.SelectedItems)
                            {
                                List<object[]> respondents = this.Core.Respondents.GetValues(
                                    new string[] { "Id" },
                                    new string[] { "IdStudy" },
                                    new object[] { idStudy }
                                );

                                // Run though all respondents of the selected project.
                                foreach (object[] respondent in respondents)
                                {
                                    Guid idRespondent = (Guid)respondent[0];

                                    if (workflowFilter.Responses.ContainsKey(idRespondent))
                                        continue;

                                    workflowFilter.Responses.Add(idRespondent, new double[0]);
                                }
                            }
                            break;
                    }

                    result = workflowFilter;
                }
            }

            return result;
        }


        private void Parse()
        {
            // Run through all workflow selection xml nodes.
            foreach (XmlNode xmlNode in this.XmlNode.ChildNodes)
            {
                WorkflowSelection selection = new WorkflowSelection(this, xmlNode);
                if (!this.Selections.ContainsKey(selection.Name))
                    this.Selections.Add(selection.Name, selection);
            }
        }

        #endregion


        #region Event Handlers

        protected void Workflow_Load(object sender, EventArgs e)
        {
            this.CssClass = "Workflow Color1";

            double width = 100.0;

            if (this.XmlNode.ChildNodes.Count != 0)
                width /= this.XmlNode.ChildNodes.Count;

            bool hasSelectedItems = false;

            // Run through all workflow selection xml nodes.
            foreach (WorkflowSelection selection in this.Selections.Values)
            {
                selection.Style.Add("width", width.ToString(new System.Globalization.CultureInfo("en-GB")) + "%");

                foreach (WorkflowSelectionSelector variableSelection in selection.SelectionVariables.Values)
                {
                    if (variableSelection.Selector.SelectedItems.Count > 0)
                        hasSelectedItems = true;
                }

                base.Controls.Add(selection);
            }
        }

        #endregion
    }
}