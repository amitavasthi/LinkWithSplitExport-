using DatabaseCore.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WebUtilities.Controls;

namespace LinkOnline.Pages.LinkManager
{
    public partial class LinkingModule : WebUtilities.BasePage
    {
        #region Properties

        WebUtilities.Controls.TreeView treeView;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

            BindHierarchies();
            var ctrlName = Request.Params[Page.postEventSourceID];
            var args = Request.Params[Page.postEventArgumentID];

            if (Request.Params["Method"] == "GetTaxonomyVariables")
            {
                Response.Clear();
                string searchstring = "";
                if (!String.IsNullOrEmpty(Request.Params["searchstring"]))
                {
                    searchstring = Request.Params["searchstring"];
                }

                BindTaxonomyVariables(Guid.Parse(Request.Params["IdHierarchy"]), searchstring);
                Response.End();

                return;
            }
            if (Request.Params["Method"] == "GetVariables")
            {

                Response.Clear();
                string searchstring = "";
                if (!String.IsNullOrEmpty(Request.Params["searchstring"]))
                {
                    searchstring = Request.Params["searchstring"];
                }

                BindVariables(Guid.Parse(Request.Params["IdHierarchy"]), searchstring, Request.Params["existingVariables"].ToString());
                Response.End();

                return;

            }

        }

        private void BindVariables(Guid idHierarchy, string variableName,string existingVariables)
        {
            // Get all taxonomy chapters of the client.
            List<object[]> studies = Global.Core.Studies.GetValues(
                new string[] { "Id", "Name" },
                new string[] { },
                new object[] { }
            );

            WebUtilities.Controls.Panel pnlContainer = new WebUtilities.Controls.Panel();

            List<object[]> Variables;

            // Run through all taxonomy chapters.
            foreach (object[] study in studies)
            {
                WebUtilities.Controls.Panel pnlTaxonomyChapter = new WebUtilities.Controls.Panel();
                pnlTaxonomyChapter.CssClass = "TaxonomyChapterTitle";

                HtmlGenericControl lblTaxonomyChapterTitle = new HtmlGenericControl("div");
                lblTaxonomyChapterTitle.Attributes.Add("class", "TaxonomyChapterTitleLabel");
                lblTaxonomyChapterTitle.InnerText = study[1].ToString();

                lblTaxonomyChapterTitle.Attributes.Add("onclick", string.Format(
                    "HideTaxonomyChapter(this, '{0}');",
                    study[0]
                ));

                pnlTaxonomyChapter.Controls.Add(lblTaxonomyChapterTitle);

                WebUtilities.Controls.Panel pnlTaxonomyChapterVariables = new WebUtilities.Controls.Panel();
                pnlTaxonomyChapterVariables.CssClass = "TaxonomyChapterVariables PnlVariableSelectors";
                pnlTaxonomyChapterVariables.Attributes.Add("id", "TaxonomyChapterVariables" + study[0]);

                string SQLQuery = "SELECT Id FROM Variables WHERE IdStudy='" + study[0] + "' AND Name LIKE '%" + variableName.Trim() + "%' ";

                if (!string.IsNullOrEmpty(existingVariables))
                {
                    SQLQuery = SQLQuery + " AND Id NOT IN ('" + existingVariables + "') ";
                }

                Variables = Global.Core.TaxonomyVariables.ExecuteReader(SQLQuery);

                if (Variables.Count == 0)
                    continue;

                // Run through all taxonomy variables.
                foreach (object[] taxonomyVariable in Variables)
                {
                    VariableSelector1.Classes.VariableSelector variableSelector = new VariableSelector1.Classes.VariableSelector(
                        2057,
                        "Variables",
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
            Variables = Global.Core.Variables.GetValues(
                new string[] { "Id" },
                new string[] { "IdStudy" },
                new object[] { DBNull.Value }
            );

            // Run through all taxonomy variables.
            foreach (object[] taxonomyVariable in Variables)
            {
                VariableSelector1.Classes.VariableSelector variableSelector = new VariableSelector1.Classes.VariableSelector(
                    2057,
                    "Variables",
                    "Id=" + taxonomyVariable[0].ToString()
                );
                //variableSelector.EnableCombine = false;
                variableSelector.Variable.Values.Add("Id", taxonomyVariable[0]);

                variableSelector.Render();
                pnlContainer.Controls.Add(variableSelector);
            }

            Response.Write(pnlContainer.ToHtml());
        }

        private void BindTaxonomyVariables(Guid idHierarchy, string variableName)
        {
            // Get all taxonomy chapters of the client.
            List<object[]> taxonomyChapters = Global.Core.TaxonomyChapters.GetValues(
                new string[] { "Id" },
                new string[] { },
                new object[] { }
            );

            WebUtilities.Controls.Panel pnlContainer = new WebUtilities.Controls.Panel();

            List<object[]> taxonomyVariables;

            // Run through all taxonomy chapters.
            foreach (object[] taxonomyChapter in taxonomyChapters)
            {
                WebUtilities.Controls.Panel pnlTaxonomyChapter = new WebUtilities.Controls.Panel();
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

                WebUtilities.Controls.Panel pnlTaxonomyChapterVariables = new WebUtilities.Controls.Panel();
                pnlTaxonomyChapterVariables.CssClass = "TaxonomyChapterVariables PnlVariableSelectors";
                pnlTaxonomyChapterVariables.Attributes.Add("id", "TaxonomyChapterVariables" + taxonomyChapter[0]);


                taxonomyVariables = Global.Core.TaxonomyVariables.ExecuteReader(
                    "SELECT Id FROM TaxonomyVariables WHERE IdTaxonomyChapter='" + taxonomyChapter[0] + "' AND Name LIKE '%" + variableName.Trim() + "%' AND Id IN " +
                    "(SELECT IdTaxonomyVariable FROM TaxonomyVariableHierarchies WHERE IdHierarchy='" + idHierarchy + "')  "
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

        private void BindHierarchies()
        {
            // Get all hierarchies on root level.
            List<object[]> hierarchies = Global.Core.Hierarchies.GetValues(
                new string[] { "Id", "Name" },
                new string[] { "IdHierarchy" },
                new object[] { null }
            );

            treeView = new WebUtilities.Controls.TreeView("tvHierarchies");

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

        private TreeViewNode RenderHierarchyTreeViewNode(WebUtilities.Controls.TreeView treeView, object[] hierarchy, string path)
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


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetTaxonomyCategories(string IdTaxonomyVariable)
        {
            List<object[]> taxonomyCategories = Global.Core.TaxonomyVariables.ExecuteReader("SELECT TC.Id,TCL.Label FROM TaxonomyCategories TC INNER JOIN TaxonomyCategoryLabels TCL ON (TC.Id=TCL.IdTaxonomyCategory) WHERE TC.IdTaxonomyVariable = '" + IdTaxonomyVariable.Trim() + "'");
            ArrayList taxonomyCategoriesList = new ArrayList();

            foreach (var item in taxonomyCategories)
            {
                dynamic taxonomyvariable = new { id = item[0], name = item[1] };
                taxonomyCategoriesList.Add(taxonomyvariable);
            }

            string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(taxonomyCategoriesList);
            return json;

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetCategories(string idVariable)
        {

            List<object[]> categories = Global.Core.TaxonomyVariables.ExecuteReader("select C.Id,CL.Label FROM Categories C INNER JOIN CategoryLabels CL ON (C.Id=CL.IdCategory) WHERE C.Name != 'SystemMissing'  AND CL.Label != 'empty' AND C.IdVariable= '" + idVariable.Trim() + "'");
            ArrayList categoriesList = new ArrayList();

            foreach (var item in categories)
            {
                dynamic variable = new { id = item[0], name = item[1] };
                categoriesList.Add(variable);
            }

            string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(categoriesList);
            return json;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getCategoryMappings(string idTaxonomyVariables)
        {
            List<object[]> categoryLinks = Global.Core.CategoryLinks.ExecuteReader("SELECT IdCategory,IdTaxonomyCategory FROM CategoryLinks WHERE IdTaxonomyVariable='" + idTaxonomyVariables + "'");
            ArrayList categoriesList = new ArrayList();

            foreach (var item in categoryLinks)
            {
                dynamic variable = new { IdCategory = item[0], IdTaxonomyCategory = item[1] };
                categoriesList.Add(variable);
            }

            string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(categoriesList);
            return json;
         
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getCategoryLinks(string idVariables,string idTaxonomyVariables)
        {
            
            List<object[]> categoryLinks = Global.Core.CategoryLinks.ExecuteReader("SELECT IdCategory,IdTaxonomyCategory FROM [dbo].[CategoryLinks] WHERE IdTaxonomyVariable='" + idTaxonomyVariables + "' AND IdVariable='" + idVariables + "' ");
            ArrayList categoriesLinkList = new ArrayList();

            foreach (var item in categoryLinks)
            {
                dynamic variable = new { IdVariable = idVariables, IdCategory = item[0], IdTaxonomyCategory = item[1] };
                categoriesLinkList.Add(variable);
            }

            string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(categoriesLinkList);
            return json;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string Link(string IdTaxonomyCategory, string IdCategory,string IdVariable,string IdTaxonomyVariable)
        {
            VariableLink variableLink = new VariableLink(Global.Core.VariableLinks);

            List<object[]> variableLinks = Global.Core.VariableLinks.GetValues(
                     new string[] { "Id" },
                     new string[] { "IdVariable", "IdTaxonomyVariable" },
                     new object[] { IdVariable,IdTaxonomyVariable
                     });

            variableLink.IdVariable = new Guid(IdVariable);
            variableLink.IdTaxonomyVariable = new Guid(IdTaxonomyVariable);
            variableLink.CreationDate = DateTime.Now;

            if (variableLinks.Count() == 0)
            {
                variableLink.Insert();
            }

            CategoryLink categoryLink = new CategoryLink(Global.Core.CategoryLinks);
            categoryLink.IdCategory = new Guid(IdCategory);
            categoryLink.IdTaxonomyCategory = new Guid(IdTaxonomyCategory);
            categoryLink.IdVariable = new Guid(IdVariable);
            categoryLink.IdTaxonomyVariable = new Guid(IdTaxonomyVariable);
            categoryLink.CreationDate = DateTime.Now;

            List<object[]> categoryLinks = Global.Core.CategoryLinks.GetValues(
                new string[] { "Id" },
                new string[] {
                                "IdCategory", 
                                "IdTaxonomyCategory", 
                                "IdVariable", 
                                "IdTaxonomyVariable" 
                }, 
                new object[] { 
                             IdCategory, 
                             IdTaxonomyCategory,
                             IdVariable,
                             IdTaxonomyVariable
                            });

            if (categoryLinks.Count() == 0)
            {
                categoryLink.Insert();
            }

            return "sucess";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UnLink(string IdTaxonomyCategory, string IdCategory, string IdVariable, string IdTaxonomyVariable)
        {

            List<object[]> categoryLinks = Global.Core.CategoryLinks.GetValues(
                new string[] { "Id" },
                new string[] {
                                "IdCategory", 
                                "IdTaxonomyCategory", 
                                "IdVariable", 
                                "IdTaxonomyVariable" 
                },
                new object[] { 
                             IdCategory, 
                             IdTaxonomyCategory,
                             IdVariable,
                             IdTaxonomyVariable
                            });

            if (categoryLinks.Count() == 0)
            {
                Global.Core.VariableLinks.Delete(new Guid((categoryLinks[0])[0].ToString()));
            }

            return "success";
        }


        
    }
}