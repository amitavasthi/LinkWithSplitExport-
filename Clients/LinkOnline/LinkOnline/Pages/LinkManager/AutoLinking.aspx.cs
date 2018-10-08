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


namespace LinkOnline.Pages.LinkManager
{
    public partial class AutoLinking : BasePage
    {
        #region Properties

        TreeView treeView;

        #endregion

        #region Constructor

        public AutoLinking()
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

                selectedHierarchyId.Value = hierarchy[0].ToString().Trim();

                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "LoadHierarchyVariables" + hierarchy[0],
                    "LoadVariables(document.getElementById('cphContent__tvnHierarchy" + hierarchy[0] + "'), '" + hierarchy[0] + "');",
                    true
                );
            }

            pnlHierarchies.Controls.Add(treeView);
        }

        private TreeViewNode RenderHierarchyTreeViewNode(TreeView treeView, object[] hierarchy, string path)
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

        /// <summary>
        /// This method is used for get all the Taxonomy Variables.
        /// </summary>
        /// <param name="idHierarchy"></param>

        private void BindTaxonomyVariables(Guid idHierarchy)
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
                //taxonomyVariables = Global.Core.TaxonomyVariables.ExecuteReader(
                //    "SELECT Id FROM TaxonomyVariables (nolock) WHERE IdTaxonomyChapter={0} AND Id IN " +
                //    "(SELECT IdTaxonomyVariable FROM TaxonomyVariableHierarchies (nolock) WHERE IdHierarchy={1})",
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
                variableSelector.Settings.EnableDelete = false;
                variableSelector.Settings.EnableRename = false;
                variableSelector.DisableVariableOptions = true;
                variableSelector.Settings.FullScreen = false;

                variableSelector.Variable.Values.Add("Id", taxonomyVariable[0]);

                variableSelector.Render();
                pnlContainer.Controls.Add(variableSelector);
            }

            Response.Write(pnlContainer.ToHtml());
        }
        /// <summary>
        /// This method is used for searching the Taxonomy Variable.
        /// </summary>
        /// <param name="idHierarchy as Guid"></param>
        /// <param name="variableName as string"></param>
        private void BindTaxonomyVariablesBySearch(Guid idHierarchy, string variableName)
        {
            // Get all taxonomy chapters of the client.
            List<object[]> taxonomyChapters = Global.Core.TaxonomyChapters.GetValues(
                new string[] { "Id" },
                new string[] { },
                new object[] { }
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

                taxonomyVariables = Global.Core.TaxonomyVariables.ExecuteReader(
                    "SELECT Id FROM TaxonomyVariables(nolock) WHERE IdTaxonomyChapter={0} AND Name LIKE {1} AND Id IN " +
                    "(SELECT IdTaxonomyVariable FROM TaxonomyVariableHierarchies (nolock) WHERE IdHierarchy={2})",
                    new object[] { taxonomyChapter[0], variableName.Replace(" ", "").Trim() + "%", idHierarchy }
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
                    variableSelector.Settings.EnableDelete = false;
                    variableSelector.Settings.EnableRename = false;
                    variableSelector.Settings.FullScreen = false;
                    variableSelector.DisableVariableOptions = true;

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

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["Method"] == "GetTaxonomyVariables")
            {
                Response.Clear();
                BindTaxonomyVariables(Guid.Parse(Request.Params["IdHierarchy"]));
                Response.End();

                return;
            }
            else if (Request.Params["Method"] == "GetTaxonomyVariablesSearch")
            {
                Response.Clear();
                string searchstring = "";
                if (!String.IsNullOrEmpty(Request.Params["variableName"]))
                {
                    searchstring = Request.Params["variableName"];
                }

                BindTaxonomyVariablesBySearch(Guid.Parse(Request.Params["IdHierarchy"]), searchstring);
                Response.End();

                return;
            }            

            BindHierarchies();
            boxVariableSearch.Visible = true;
        }

        #endregion
    }

}