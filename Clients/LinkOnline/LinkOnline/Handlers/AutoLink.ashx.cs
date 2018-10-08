using DatabaseCore.BaseClasses;
using DatabaseCore.Items;
using DataCore.Classes;
using LinkBi1.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Xml;
using VariableSelector1.Classes;
using WebUtilities;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for AutoLink
    /// </summary>
    public class AutoLink : BaseHandler
    {
        public DefinitionObject Variable { get; set; }

        #region Constructor

        public AutoLink()
        {
            base.Methods.Add("GetTaxonomyCategories", GetTaxonomyCategories);
            base.Methods.Add("GetCategoryMappings", GetCategoryMappings);
            base.Methods.Add("GetCategoryLinks", GetCategoryLinks);
            base.Methods.Add("GetCategories", GetCategories);
            base.Methods.Add("UnLink", UnLink);
            base.Methods.Add("Link", Link);
            base.Methods.Add("LinkNumericOrMulti", LinkNumericOrMulti);
            base.Methods.Add("BindStudyVariables", BindStudyVariables);
            base.Methods.Add("AddNotes", AddNotes);
            base.Methods.Add("ClearNotes", ClearNotes);
            base.Methods.Add("GetChartDetails", GetChartDetails);
            base.Methods.Add("UnlinkVariables", UnlinkVariables);
        }

        #endregion
        #region Web Methods


        /// <summary>
        /// Get all the studies of the client.
        /// </summary>
        /// <param name="idHierarchy"></param>
        /// <param name="variableName"></param>
        /// <param name="existingVariables"></param>     

        private void BindStudyVariables(HttpContext context)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<Root></Root>");

            string variableName = context.Request.Params["searchstring"];
            Guid[] existingVariables = null;
            if (!String.IsNullOrEmpty(context.Request.Params["existingVariables"]))
            {
                existingVariables = context.Request.Params["existingVariables"].
                    Split(',').Select(x => Guid.Parse(x)).ToArray();
            }

            Dictionary<Guid, WebUtilities.Controls.Panel> studySections = new Dictionary<Guid, WebUtilities.Controls.Panel>();

            //if (!String.IsNullOrEmpty(variableName))
            {
                List<object[]> variables = Global.Core.VariableLabels.ExecuteReader(
                    "SELECT Top 100 VariableLabels.IdVariable, Variables.IdStudy, Variables.Name, Variables.Type, VariableLabels.Label " +
                    "FROM VariableLabels INNER JOIN Variables ON Variables.Id=VariableLabels.IdVariable WHERE VariableLabels.Label like {0} " +
                    "AND Variables.Id NOT IN(SELECT IDVARIABLE FROM VARIABLELINKS)",
                    new object[] { "%" + variableName.Replace(" ", "").Trim() + "%" }
                );

                List<object[]> linkedVariables = Global.Core.VariableLinks.ExecuteReader("SELECT TOP 100 VariableLabels.IdVariable, Variables.IdStudy, Variables.Name, Variables.Type, VariableLabels.Label " +
                            "FROM VariableLabels INNER JOIN Variables ON Variables.Id=VariableLabels.IdVariable INNER JOIN VariableLinks ON Variables.Id=VariableLinks.IdVariable WHERE VariableLabels.Label like {0}",
                              new object[] { "%" + variableName.Replace(" ", "").Trim() + "%" });

                Guid idHierarchy = Guid.Parse(context.Request.Params["idHierarchy"]);


                WebUtilities.Controls.Panel pnlContainer = new WebUtilities.Controls.Panel();

                // Run through all unlinked variables
                foreach (object[] variable in variables)
                {
                    Guid idVariable = (Guid)variable[0];
                    Guid idStudy = (Guid)variable[1];

                    if (existingVariables != null)
                    {
                        if (existingVariables.Contains(idVariable))
                            continue;
                    }

                    if (!studySections.ContainsKey(idStudy))
                    {
                        WebUtilities.Controls.Panel pnlTaxonomyChapterVariables = new WebUtilities.Controls.Panel();
                        pnlTaxonomyChapterVariables.CssClass = "TaxonomyChapterVariables PnlVariableSelectors";
                        pnlTaxonomyChapterVariables.Attributes.Add("id", "TaxonomyChapterVariables" + idStudy);

                        studySections.Add(idStudy, pnlTaxonomyChapterVariables);
                    }

                    DefinitionObject definitionObject = new DefinitionObject(Global.Core, "", "");
                    definitionObject.Values.Add("Id", idVariable);
                    definitionObject.Values.Add("Name", (string)variable[2]);
                    definitionObject.Values.Add("Type", (int)variable[3]);
                    definitionObject.Values.Add("Label" + "2057", (string)variable[4]);

                    //VariableSelector1.Classes.VariableSelector variableSelector = new VariableSelector1.Classes.VariableSelector(
                    //    2057,
                    //    definitionObject
                    //);

                    VariableSelector1.Classes.LinkingVariableSelector variableSelector = new VariableSelector1.Classes.LinkingVariableSelector(
                        2057,
                        definitionObject
                    );

                    variableSelector.DisableVariableOptions = true;
                    variableSelector.AlreadyLinkedVariable = false;

                    variableSelector.Render();
                    studySections[idStudy].Controls.Add(variableSelector);
                }
                // Run through all linked variables
                foreach (object[] linkedvariable in linkedVariables)
                {
                    Guid idVariable = (Guid)linkedvariable[0];
                    Guid idStudy = (Guid)linkedvariable[1];

                    if (existingVariables != null)
                    {
                        if (existingVariables.Contains(idVariable))
                            continue;
                    }

                    if (!studySections.ContainsKey(idStudy))
                    {
                        WebUtilities.Controls.Panel pnlTaxonomyChapterVariables = new WebUtilities.Controls.Panel();
                        pnlTaxonomyChapterVariables.CssClass = "TaxonomyChapterVariables PnlVariableSelectors";
                        pnlTaxonomyChapterVariables.Attributes.Add("id", "TaxonomyChapterVariables" + idStudy);

                        studySections.Add(idStudy, pnlTaxonomyChapterVariables);
                    }

                    DefinitionObject definitionObject = new DefinitionObject(Global.Core, "", "");
                    definitionObject.Values.Add("Id", idVariable);
                    definitionObject.Values.Add("Name", (string)linkedvariable[2]);
                    definitionObject.Values.Add("Type", (int)linkedvariable[3]);
                    definitionObject.Values.Add("Label" + "2057", (string)linkedvariable[4]);

                    VariableSelector1.Classes.LinkingVariableSelector variableSelector = new VariableSelector1.Classes.LinkingVariableSelector(
                        2057,
                        definitionObject
                    );

                    variableSelector.DisableVariableOptions = true;
                    variableSelector.AlreadyLinkedVariable = true;

                    variableSelector.Render();
                    studySections[idStudy].Controls.Add(variableSelector);
                }


                foreach (Guid idStudy in studySections.Keys)
                {
                    string studyName = (string)Global.Core.Studies.GetValue("Name", "Id", idStudy);

                    WebUtilities.Controls.Panel pnlTaxonomyChapter = new WebUtilities.Controls.Panel();
                    pnlTaxonomyChapter.CssClass = "TaxonomyChapterTitle";
                    pnlTaxonomyChapter.Attributes.Add("name", studyName);

                    HtmlGenericControl lblTaxonomyChapterTitle = new HtmlGenericControl("div");
                    lblTaxonomyChapterTitle.Attributes.Add("class", "TaxonomyChapterTitleLabel");
                    lblTaxonomyChapterTitle.InnerText = studyName;

                    lblTaxonomyChapterTitle.Attributes.Add("onclick", string.Format(
                        "HideTaxonomyChapter(this, '{0}');",
                        idStudy
                    ));

                    pnlTaxonomyChapter.Controls.Add(lblTaxonomyChapterTitle);

                    pnlTaxonomyChapter.Controls.Add(studySections[idStudy]);

                    pnlContainer.Controls.Add(pnlTaxonomyChapter);
                }

                context.Response.Write(pnlContainer.ToHtml());
            }
        }




        public void GetTaxonomyCategories(HttpContext context)
        {
            Guid idTaxonomyVariable = Guid.Parse(context.Request.Params["selectdTaxonomyVariableId"]);

            List<object[]> taxonomyCategories = Global.Core.TaxonomyCategories.ExecuteReader(
             "SELECT TC.Id,TCL.Label FROM TaxonomyCategories(nolock) TC INNER JOIN TaxonomyCategoryLabels (nolock) TCL ON (TC.Id=TCL.IdTaxonomyCategory)" +
             "WHERE TC.IdTaxonomyVariable = {0}", new object[] { idTaxonomyVariable });

            ArrayList taxonomyCategoriesList = new ArrayList();

            foreach (var item in taxonomyCategories)
            {
                dynamic taxonomyvariable = new { id = item[0], name = item[1] };
                taxonomyCategoriesList.Add(taxonomyvariable);
            }

            string jSon = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(taxonomyCategoriesList);

            context.Response.Write(jSon);
        }

        public void GetCategoryMappings(HttpContext context)
        {
            Guid idTaxonomyVariable = Guid.Parse(context.Request.Params["selectdTaxonomyVariableId"]);

            //List<object[]> categoryLinks = Global.Core.CategoryLinks.ExecuteReader(
            //    "SELECT IdCategory,IdTaxonomyCategory FROM CategoryLinks (nolock) WHERE IdTaxonomyVariable='" + idTaxonomyVariable + "'");
            List<object[]> categoryLinks = Global.Core.CategoryLinks.ExecuteReader(
              "SELECT IdCategory,IdTaxonomyCategory FROM CategoryLinks (nolock) WHERE IdTaxonomyVariable={0}", new object[] { idTaxonomyVariable });

            ArrayList categoriesList = new ArrayList();

            foreach (var item in categoryLinks)
            {
                dynamic variable = new { IdCategory = item[0], IdTaxonomyCategory = item[1] };
                categoriesList.Add(variable);
            }

            string jSon = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(categoriesList);
            context.Response.Write(jSon);
        }


        public void GetCategoryLinks(HttpContext context)
        {
            Guid idVariables = Guid.Parse(context.Request.Params["idVariable"]);
            string idTaxonomyVariables = context.Request.Params["selectdTaxonomyVariableId"];

            List<object[]> categoryLinks = Global.Core.CategoryLinks.ExecuteReader("SELECT IdCategory,IdTaxonomyCategory,Notes FROM [dbo].[CategoryLinks] WHERE IdTaxonomyVariable={0} AND IdVariable={1}",
                new object[] { idTaxonomyVariables, idVariables });

            ArrayList categoriesLinkList = new ArrayList();

            foreach (var item in categoryLinks)
            {
                dynamic variable = new { IdVariable = idVariables, IdCategory = item[0], IdTaxonomyCategory = item[1], notes = item[2] };
                categoriesLinkList.Add(variable);
            }

            string jSon = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(categoriesLinkList);
            context.Response.Write(jSon);
        }

        public void GetCategories(HttpContext context)
        {
            Guid idVariable = Guid.Parse(context.Request.Params["idVariable"]);
            Guid idStudy = Guid.Parse(context.Request.Params["idStudy"]);
            string idTaxonomyVariables = context.Request.Params["selectdTaxonomyVariableId"];
            ArrayList categoriesList = new ArrayList();

            //int categoryLinkCount;
            List<object[]> categories = Global.Core.Categories.ExecuteReader("select C.Id,CL.Label FROM Categories (nolock) C INNER JOIN CategoryLabels (nolock) CL ON C.Id=CL.IdCategory WHERE C.Name != 'SystemMissing'  AND CL.Label != 'empty' AND C.IdVariable= {0} order by CL.Label",
                 new object[] { idVariable });

            foreach (var item in categories)
            {
                List<object[]> categoryLinks = Global.Core.CategoryLinks.ExecuteReader("Select IdCategory,IdTaxonomyCategory,Notes FROM [dbo].[CategoryLinks] WHERE IdTaxonomyVariable={0} AND IdVariable={1} AND IdCategory={2}",
                    new object[] { idTaxonomyVariables, idVariable, item[0] });

                if (categoryLinks.Count > 0)
                {
                    foreach (var categoryLinkItem in categoryLinks)
                    {
                        if (categoryLinkItem[2] != null)
                        {
                            dynamic variable = new { id = categoryLinkItem[0], name = item[1], idStudy, idVariable, exist = true, IdTaxonomyCategory = categoryLinkItem[1], savedNotes = categoryLinkItem[2] };
                            categoriesList.Add(variable);
                        }
                        else
                        {
                            dynamic variable = new { id = categoryLinkItem[0], name = item[1], idStudy, idVariable, exist = true, IdTaxonomyCategory = categoryLinkItem[1], savedNotes = "" };
                            categoriesList.Add(variable);
                        }
                    }
                }
                else
                {
                    dynamic variable = new { id = item[0], name = item[1], idStudy, idVariable, exist = false, idTaxonomyVariables, IdTaxonomyCategory = "", savedNotes = "" };
                    categoriesList.Add(variable);
                }
                // dynamic variable = new { id = item[0], name = item[1], idStudy, idVariable };

            }

            string jSon = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(categoriesList);
            context.Response.Write(jSon);
        }


        public void GetCategoriesWithExistingLinks(HttpContext context)
        {
            Guid idVariable = Guid.Parse(context.Request.Params["idVariable"]);
            Guid idStudy = Guid.Parse(context.Request.Params["idStudy"]);

            List<object[]> categories = Global.Core.Categories.ExecuteReader("select C.Id,CL.Label FROM Categories (nolock) C INNER JOIN CategoryLabels (nolock) CL ON (C.Id=CL.IdCategory) WHERE C.Name != 'SystemMissing'  AND CL.Label != 'empty' AND C.IdVariable= {0}",
                  new object[] { idVariable });
            ArrayList categoriesList = new ArrayList();

            foreach (var item in categories)
            {
                dynamic variable = new { id = item[0], name = item[1], idStudy, idVariable };
                categoriesList.Add(variable);
            }

            string jSon = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(categoriesList);
            context.Response.Write(jSon);
        }



        /// <summary>
        /// For Unlinking a Variable
        /// </summary>
        /// <param name="IdTaxonomyCategory"></param>
        /// <param name="IdCategory"></param>
        /// <param name="IdVariable"></param>
        /// <param name="IdTaxonomyVariable"></param>
        /// <returns></returns>
        public void UnLink(HttpContext context)
        {
            Guid IdTaxonomyCategory = Guid.Parse(context.Request.Params["IdTaxonomyCategory"]);
            Guid IdCategory = Guid.Parse(context.Request.Params["IdCategory"]);
            Guid IdVariable = Guid.Parse(context.Request.Params["IdVariable"]);
            Guid IdTaxonomyVariable = Guid.Parse(context.Request.Params["IdTaxonomyVariable"]);
            if (!String.IsNullOrEmpty(context.Request.Params["Text"]))
            {
                string taxonomyCategoryText = context.Request.Params["Text"].Trim();
            }
            //List<object[]> categoryLinks = Global.Core.CategoryLinks.ExecuteReader("SELECT IdVariable,IdCategory,IdTaxonomyCategory,IdTaxonomyVariable FROM [dbo].[CategoryLinks]" +
            //                                "WHERE IdVariable={0} AND IdCategory={1} AND IdTaxonomyCategory ={2} AND IdTaxonomyVariable ={3}",
            //                                new object[] { IdVariable, IdCategory, IdTaxonomyCategory, IdTaxonomyVariable });

            List<object[]> categoryLinks = Global.Core.CategoryLinks.ExecuteReader("SELECT IdVariable,IdCategory,IdTaxonomyCategory,IdTaxonomyVariable FROM [dbo].[CategoryLinks]" +
                                           "WHERE IdVariable={0} AND IdCategory={1} AND IdTaxonomyCategory ={2} AND IdTaxonomyVariable ={3}",
                                           new object[] { IdVariable, IdCategory, IdTaxonomyCategory, IdTaxonomyVariable });

            if (categoryLinks.Count() > 0)
            {
                List<object[]> mCnt = Global.Core.CategoryLinks.ExecuteReader("SELECT IdVariable,IdCategory,IdTaxonomyCategory,IdTaxonomyVariable FROM [dbo].[CategoryLinks]" +
                                     "WHERE IdVariable={0} AND IdCategory ={1} AND IdTaxonomyCategory ={2} AND IdTaxonomyVariable ={3}",
                                     new object[] { IdVariable, IdCategory, IdTaxonomyCategory, IdTaxonomyVariable });

                if (mCnt.Count() > 1)
                {
                    string SQL = "DELETE FROM [CategoryLinks] WHERE IdVariable ='" + Guid.Parse(categoryLinks[0][0].ToString()) + "'AND IdCategory ='" + Guid.Parse(categoryLinks[0][1].ToString()) + "'" +
                                "AND IdTaxonomyVariable ='" + Guid.Parse(categoryLinks[0][3].ToString()) + "'AND IdTaxonomyCategory NOT IN('" + IdTaxonomyCategory + "')";

                    Global.Core.CategoryLinks.ExecuteQuery(SQL);
                }
                else
                {
                    // Delete all the category links of the study's variables.
                    string SQL = "DELETE FROM [CategoryLinks] WHERE IdVariable ='" + Guid.Parse(categoryLinks[0][0].ToString()) + "'AND IdCategory ='" + Guid.Parse(categoryLinks[0][1].ToString()) + "'" +
                        "AND IdTaxonomyCategory ='" + Guid.Parse(categoryLinks[0][2].ToString()) + "'AND IdTaxonomyVariable ='" + Guid.Parse(categoryLinks[0][3].ToString()) + "'";
                    Global.Core.CategoryLinks.ExecuteQuery(SQL);
                }
            }
            else if (categoryLinks.Count() == 0)
            {
                List<object[]> multiLink = Global.Core.CategoryLinks.ExecuteReader("SELECT Id, IdVariable,IdCategory,IdTaxonomyCategory,IdTaxonomyVariable FROM [dbo].[CategoryLinks]" +
                                                                                    "WHERE IdVariable= {0} AND IdTaxonomyCategory ={1} AND IdTaxonomyVariable ={2}",
                                                                                     new object[] { IdVariable, IdTaxonomyCategory, IdTaxonomyVariable });
                if (multiLink.Count() > 0)
                {
                    string SQL = "DELETE FROM [CategoryLinks] WHERE IdVariable ='" + IdVariable + "'" +
                                  "AND IdTaxonomyCategory ='" + IdTaxonomyCategory + "'AND IdTaxonomyVariable ='" + IdTaxonomyVariable + "'";
                    Global.Core.CategoryLinks.ExecuteQuery(SQL);
                    // Global.Core.CategoryLinks.Delete(multiLink[0].Id);
                }
            }

            List<object[]> allCategoryLinks = Global.Core.CategoryLinks.ExecuteReader("SELECT IdVariable,IdTaxonomyVariable FROM [dbo].[CategoryLinks]" +
               "WHERE IdVariable={0} AND IdTaxonomyVariable ={1}", new object[] { IdVariable, IdTaxonomyVariable });

            if (allCategoryLinks.Count() == 0)
            {
                // Delete all the variable links of the study's variables.
                Global.Core.VariableLinks.ExecuteQuery("DELETE FROM [VariableLinks] WHERE IdVariable='" + IdVariable + "'" + "AND IdTaxonomyVariable ='" + IdTaxonomyVariable + "'");
            }
            context.Response.Write("success");
        }

        /// <summary>
        /// For Linking Multi and Numeric Variables
        /// </summary>
        /// <param name="context"></param>
        public void LinkNumericOrMulti(HttpContext context)
        {
            Guid IdVariable = Guid.Parse(context.Request.Params["IdVariable"]);
            Guid IdTaxonomyVariable = Guid.Parse(context.Request.Params["IdTaxonomyVariable"]);

            VariableLink variableLink = new VariableLink(Global.Core.VariableLinks);

            List<object[]> variableLinks = Global.Core.VariableLinks.GetValues(
                     new string[] { "Id" },
                     new string[] { "IdVariable", "IdTaxonomyVariable" },
                     new object[] { IdVariable,IdTaxonomyVariable
                     });

            variableLink.IdVariable = IdVariable;
            variableLink.IdTaxonomyVariable = IdTaxonomyVariable;
            variableLink.CreationDate = DateTime.Now;

            if (variableLinks.Count() == 0)
            {
                variableLink.Insert();
            }

            context.Response.Write("success");
        }

        /// <summary>
        /// For Unlinking all the variable links
        /// </summary>      
        /// <param name="IdVariable"></param>
        /// <returns></returns>
        public void UnlinkVariables(HttpContext context)
        {

            Guid IdVariable = Guid.Parse(context.Request.Params["IdVariable"]);
            Guid IdTaxonomyVariable = Guid.Parse(context.Request.Params["IdTaxonomyVariable"]);

            VariableLink variableLink = new VariableLink(Global.Core.VariableLinks);

            List<object[]> variableLinks = Global.Core.VariableLinks.GetValues(
                     new string[] { "Id" },
                     new string[] { "IdVariable", "IdTaxonomyVariable" },
                     new object[] { IdVariable,IdTaxonomyVariable
                     });

            if (variableLinks.Count > 0)
            {
                List<object[]> categoryLinks = Global.Core.CategoryLinks.ExecuteReader("SELECT IdVariable,IdCategory,IdTaxonomyCategory,IdTaxonomyVariable FROM [dbo].[CategoryLinks]" +
                                           "WHERE IdVariable={0} AND IdTaxonomyVariable ={1}",
                                           new object[] { IdVariable, IdTaxonomyVariable });

                if (categoryLinks.Count > 0)
                {
                    Global.Core.CategoryLinks.ExecuteQuery("DELETE FROM [CategoryLinks] WHERE IdVariable='" + IdVariable + "'" + "AND IdTaxonomyVariable ='" + IdTaxonomyVariable + "'");
                }

                Global.Core.VariableLinks.ExecuteQuery("DELETE FROM [VariableLinks] WHERE IdVariable='" + IdVariable + "'" + "AND IdTaxonomyVariable ='" + IdTaxonomyVariable + "'");

            }

            context.Response.Write("success");
        }


        /// <summary>
        /// For Add Notes for Category Links
        /// </summary>
        /// <param name="context"></param>
        public void AddNotes(HttpContext context)
        {
            Guid IdCategory = Guid.Parse(context.Request.Params["IdCategory"]);
            Guid IdVariable = Guid.Parse(context.Request.Params["IdVariable"]);
            Guid IdTaxonomyCategory = Guid.Parse(context.Request.Params["IdTaxonomyCategory"]);
            string notes = context.Request.Params["Notes"];

            if (notes != null)
            {
                notes = notes.Replace("'", "''");
            }

            List<object[]> categoryLinks = Global.Core.CategoryLinks.GetValues(
                     new string[] { "Id" },
                     new string[] { "IdVariable", "IdCategory", "IdTaxonomyCategory" },
                     new object[] { IdVariable, IdCategory, IdTaxonomyCategory });

            if (categoryLinks.Count() > 0)
            {
                Global.Core.CategoryLinks.ExecuteQuery("UPDATE CATEGORYLINKS SET NOTES='" + notes + "'" + "WHERE IDVARIABLE='" + IdVariable + "'" + " AND IDCATEGORY='" + IdCategory + "'" + "AND IDTAXONOMYCATEGORY='" + IdTaxonomyCategory + "'");
            }

        }
        /// <summary>
        /// For Clearing the Notes from Category Links
        /// </summary>
        /// <param name="context"></param>
        public void ClearNotes(HttpContext context)
        {
            Guid IdCategory = Guid.Parse(context.Request.Params["IdCategory"]);
            Guid IdVariable = Guid.Parse(context.Request.Params["IdVariable"]);
            Guid IdTaxonomyCategory = Guid.Parse(context.Request.Params["IdTaxonomyCategory"]);
            string notes = context.Request.Params["Notes"];

            List<object[]> categoryLinks = Global.Core.CategoryLinks.GetValues(
                     new string[] { "Id" },
                     new string[] { "IdVariable", "IdCategory", "IdTaxonomyCategory" },
                     new object[] { IdVariable, IdCategory, IdTaxonomyCategory });

            if (categoryLinks.Count() > 0)
            {
                Global.Core.CategoryLinks.ExecuteQuery("UPDATE CATEGORYLINKS SET NOTES='" + DBNull.Value + "'" + "WHERE IDVARIABLE='" + IdVariable + "'" + " AND IDCATEGORY='" + IdCategory + "'" + "AND IDTAXONOMYCATEGORY='" + IdTaxonomyCategory + "'");

            }

        }
        /// <summary>
        /// For Linking Variables and Categories
        /// </summary>
        /// <param name="context"></param>
        public void Link(HttpContext context)
        {
            Guid IdTaxonomyCategory = Guid.Parse(context.Request.Params["IdTaxonomyCategory"]);
            Guid IdCategory = Guid.Parse(context.Request.Params["IdCategory"]);
            Guid IdVariable = Guid.Parse(context.Request.Params["IdVariable"]);
            Guid IdTaxonomyVariable = Guid.Parse(context.Request.Params["IdTaxonomyVariable"]);

            VariableLink variableLink = new VariableLink(Global.Core.VariableLinks);

            List<object[]> variableLinks = Global.Core.VariableLinks.GetValues(
                     new string[] { "Id" },
                     new string[] { "IdVariable", "IdTaxonomyVariable" },
                     new object[] { IdVariable,IdTaxonomyVariable
                     });

            ArrayList categoriesLinkList = new ArrayList();

            variableLink.IdVariable = IdVariable;
            variableLink.IdTaxonomyVariable = IdTaxonomyVariable;
            variableLink.CreationDate = DateTime.Now;

            if (variableLinks.Count() == 0)
            {
                variableLink.Insert();
            }

            CategoryLink categoryLink = new CategoryLink(Global.Core.CategoryLinks);
            categoryLink.IdVariable = IdVariable;
            categoryLink.IdCategory = IdCategory;
            categoryLink.IdTaxonomyCategory = IdTaxonomyCategory;
            categoryLink.IdTaxonomyVariable = IdTaxonomyVariable;
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


            foreach (var item in categoryLinks)
            {
                dynamic variable = new { IdVariable = IdVariable, IdCategory = IdCategory, IdTaxonomyCategory = IdTaxonomyCategory, IdTaxonomyVariable = IdTaxonomyVariable };
                categoriesLinkList.Add(variable);
            }

            string jSon = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(categoriesLinkList);
            context.Response.Write(jSon);

            //context.Response.Write("success");
        }

        public void GetChartDetails(HttpContext context)
        {
            ArrayList chartList = new ArrayList();

            int linked = Global.Core.VariableLinks.GetCount();
            int variables = Global.Core.Variables.GetCount();
            int linkedC = Global.Core.CategoryLinks.GetCount();
            int variablesC = Global.Core.Categories.GetCount();
            int unLinkedV = 0;
            if ((variables - linked) > 0)
            {
                unLinkedV = variables - linked;
            }
            int unLinkedCategories = 0;
            if ((variablesC - linkedC) > 0)
            {
                unLinkedCategories = variablesC - linkedC;
            }            
            dynamic variablelinked = new { dimension = "linked variables", value = linked, label = "linked variables", color = "#1f77b4" };
            chartList.Add(variablelinked);

            dynamic variableUnlinked = new { dimension = "unlinked variables", value = unLinkedV, label = "unlinked variables", color = "#aec7e8" };
            chartList.Add(variableUnlinked);

            dynamic categoryLinked = new { dimension = "linked categories", value = linkedC, label = "linked categories", color = "#ff7f0e" };
            chartList.Add(categoryLinked);

            dynamic categoryUnlinked = new { dimension = "unlinked categories", value = unLinkedCategories, label = "unlinked categories", color = "#ffbb78" };
            chartList.Add(categoryUnlinked);

            string jSon = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(chartList);
            context.Response.Write(jSon);


        }

        #endregion

    }
}