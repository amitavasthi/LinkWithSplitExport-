using ApplicationUtilities;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace LinkOnline.Pages.LinkManager
{
    public partial class ManualLinkManager : WebUtilities.BasePage
    {

        public bool ShowAllVariables { get; set; }
        public StringBuilder intialJavascriptVariablesSB = new StringBuilder();

        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                csStudyCategories.Limit = 500;
                csStudyCategories.SearchMode = WebUtilities.Controls.CategorySearchMode.Study;


                csTaxonomyCategories.SearchMode = WebUtilities.Controls.CategorySearchMode.Taxonomy;
                csTaxonomyCategories.SelectionType = WebUtilities.Controls.CategorySearchSelectionType.Multi;

                string SQLQuery = "";

                List<object[]> unlinkedcategories = new List<object[]>();

                //if (this.ShowAllVariables != true)
                //{
                //    SQLQuery = String.Format("SELECT c.Id From Categories C INNER JOIN CategoryLabels CL ON (C.Id=CL.IdCategory) WHERE C.Name != 'SystemMissing' AND C.Id NOT IN (SELECT IdCategory FROM CategoryLinks)");
                //}
                //else
                //{
                    SQLQuery = String.Format("SELECT c.Id From Categories C INNER JOIN CategoryLabels CL ON (C.Id=CL.IdCategory) WHERE C.Name != 'SystemMissing' ");
                //}

                unlinkedcategories = Global.Core.TaxonomyVariableLabels.ExecuteReader(SQLQuery);

                Dictionary<Guid, object> categories = new Dictionary<Guid, object>();

                // Run through all unlinked categories of the database select result.
                foreach (object[] linkedCategory in unlinkedcategories)
                {
                    categories.Add((Guid)linkedCategory[0], null);
                }

                // Define the delegate method that the category search
                // uses to check if a category should be displayed.
                csStudyCategories.CheckDisplayMethod = delegate(Guid idCategory, bool isTaxonomy)
                {
                    // Check if the category is a non-linked category.
                    return categories.ContainsKey(idCategory);
                };

            }

        }

        #endregion

        #region INITIAL DATA POPULATION


        #endregion

        #region WEB METHODS

        /// <summary>
        /// Web method to create the link between variables and taxonmy variables also categories and taxonomy categories
        /// </summary>
        /// <param name="IdsStudyCategories">comma seperated values of  categories ids to be linked</param>
        /// <param name="IdsTaxonomyCategories">comma seperated values of  taxonomy categories ids to be linked</param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string Link(string IdsStudyCategories, string IdsTaxonomyCategories)
        {

            string[] IdsStudyCategoriesArray = SplitCommaSeperatedValues(IdsStudyCategories);
            string[] IdsTaxonomyCategoriesArray = SplitCommaSeperatedValues(IdsTaxonomyCategories);

            LinkCategories(IdsStudyCategoriesArray, IdsTaxonomyCategoriesArray);

            return "success";
        }


        /// <summary>
        /// Web method to create the un link between variables and taxonmy variables also categories and taxonomy categories
        /// </summary>
        /// <param name="IdsStudyVariables">comma seperated values of variable ids to be un linked</param>
        /// <param name="IdsTaxonomyVariables">comma seperated values of  taxonomy variable ids to be un linked</param>
        /// <param name="IdsStudyCategories">comma seperated values of  categories ids to be un linked</param>
        /// <param name="IdsTaxonomyCategories">comma seperated values of  taxonomy categories ids to be un linked</param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UnLink(string IdsStudyVariables, string IdsTaxonomyVariables, string IdsStudyCategories, string IdsTaxonomyCategories)
        {
            UnlinkVariables(IdsStudyVariables, IdsTaxonomyVariables);
            UnlinkChildCategories(IdsStudyVariables, IdsTaxonomyVariables);
            UnlinkCategories(IdsStudyCategories, IdsTaxonomyCategories);
            return "success";
        }

        #endregion

        #region UN LINK FUNCTIONALITY METHODS

        /// <summary>
        /// segregated only unlink funtionality of variables
        /// </summary>
        /// <param name="IdsStudyVariables">comma seperated values of variable ids to be un linked</param>
        /// <param name="IdsTaxonomyVariables">comma seperated values of  taxonomy variable ids to be un linked</param>
        /// <returns></returns>
        public static string UnlinkVariables(string IdsStudyVariables, string IdsTaxonomyVariables)
        {
            string[] ArrIdsStudyVariables = SplitCommaSeperatedValues(IdsStudyVariables);
            string[] ArrIdsTaxonomyVariables = SplitCommaSeperatedValues(IdsTaxonomyVariables);

            for (int i = 0; i < ArrIdsStudyVariables.Count(); i++)
            {
                List<object[]> VariableLinks = Global.Core.VariableLinks.GetValues(new string[] { "Id" }, new string[] { "IdVariable", "IdTaxonomyVariable" }, new object[] { ArrIdsStudyVariables[i], ArrIdsTaxonomyVariables[0] });
                for (int j = 0; j < VariableLinks.Count(); j++)
                {
                    Global.Core.VariableLinks.Delete(new Guid((VariableLinks[j])[0].ToString()));
                }
            }

            return "sucess";
        }

        /// <summary>
        /// deletes the relationship between the categories and taxonomy variables of the unlinked variables and taxonomy variables 
        /// </summary>
        /// <param name="IdsStudyVariables"></param>
        /// <param name="IdsTaxonomyVariables"></param>
        public static void UnlinkChildCategories(string IdsStudyVariables, string IdsTaxonomyVariables)
        {
            string[] ArrIdsStudyVariables = SplitCommaSeperatedValues(IdsStudyVariables);
            string[] ArrIdsTaxonomyVariables = SplitCommaSeperatedValues(IdsTaxonomyVariables);

            for (int i = 0; i < ArrIdsStudyVariables.Count(); i++)
            {
                List<object[]> categoryLinks = Global.Core.CategoryLinks.GetValues(new string[] { "Id" }, new string[] { "IdVariable", "IdTaxonomyVariable" }, new object[] { ArrIdsStudyVariables[i], ArrIdsTaxonomyVariables[0] });
                for (int j = 0; j < categoryLinks.Count(); j++)
                {
                    Global.Core.CategoryLinks.Delete(new Guid((categoryLinks[j])[0].ToString()));
                }
            }
        }


        /// <summary>
        /// segregated only unlink funtionality of categories
        /// </summary>
        /// <param name="IdsStudyCategories">comma seperated values of  categories ids to be un linked</param>
        /// <param name="IdsTaxonomyCategories">comma seperated values of  taxonomy categories ids to be un linked</param>
        /// <returns></returns>
        public static string UnlinkCategories(string IdsStudyCategories, string IdsTaxonomyCategories)
        {
            string[] IdsStudyCategoriesArray = SplitCommaSeperatedValues(IdsStudyCategories);
            string[] IdsTaxonomyCategoriesArray = SplitCommaSeperatedValues(IdsTaxonomyCategories);

            TaxonomyCategory taxonomyCategory = Global.Core.TaxonomyCategories.GetSingle(new Guid(IdsTaxonomyCategoriesArray[0]));

            for (int i = 0; i < IdsStudyCategoriesArray.Count(); i++)
            {
                Category category = Global.Core.Categories.GetSingle(new Guid(IdsStudyCategoriesArray[i]));
                List<object[]> categoryLinks = Global.Core.CategoryLinks.GetValues(new string[] { "Id" }, new string[] { "IdCategory", "IdTaxonomyCategory", "IdVariable", "IdTaxonomyVariable" }, new object[] { IdsStudyCategoriesArray[i], IdsTaxonomyCategoriesArray[0], category.IdVariable, taxonomyCategory.IdTaxonomyVariable });
                for (int j = 0; j < categoryLinks.Count(); j++)
                {
                    Global.Core.CategoryLinks.Delete(new Guid((categoryLinks[j])[0].ToString()));
                }
            }

            return "success";
        }

        #endregion

        #region LINK FUNCTIONALITY METHODS

        /// <summary>
        /// segregated only link funtionality of variables
        /// </summary>
        /// <param name="IdsStudyVariables">comma seperated values of variable ids to be un linked</param>
        /// <param name="IdsTaxonomyVariables">comma seperated values of  taxonomy variable ids to be un linked</param>
        /// <returns></returns>
        private static void LinkVariables(string[] ArrIdsStudyVariables, string[] ArrIdsTaxonomyVariables)
        {
            for (int i = 0; i < ArrIdsStudyVariables.Count(); i++)
            {

                List<object[]> variableLinks = Global.Core.VariableLinks.GetValues(new string[] { "Id" }, new string[] { "IdVariable", "IdTaxonomyVariable" }, new object[] { ArrIdsStudyVariables[i], ArrIdsTaxonomyVariables[0] });

                if (variableLinks.Count() == 0)
                {
                    VariableLink newVariableLink = new VariableLink(Global.Core.VariableLinks);

                    newVariableLink.IdVariable = new Guid(ArrIdsStudyVariables[i]);
                    newVariableLink.IdTaxonomyVariable = new Guid(ArrIdsTaxonomyVariables[0]);
                    newVariableLink.CreationDate = DateTime.Now;

                    newVariableLink.Insert();
                }
            }
        }

        /// <summary>
        /// segregated only link funtionality of categories
        /// </summary>
        /// <param name="IdsStudyVariables">comma seperated values of variable ids to be un linked</param>
        /// <param name="IdsTaxonomyVariables">comma seperated values of  taxonomy variable ids to be un linked</param>
        /// <returns></returns>
        private static void LinkCategories(string[] ArrIdsStudyCategories, string[] ArrIdsTaxonomyCategories)
        {
            TaxonomyCategory taxonomyCategory = Global.Core.TaxonomyCategories.GetSingle(new Guid(ArrIdsTaxonomyCategories[0]));
            for (int i = 0; i < ArrIdsStudyCategories.Count(); i++)
            {

                Category category = Global.Core.Categories.GetSingle(new Guid(ArrIdsStudyCategories[i]));

                foreach (var taxonomyCategories in ArrIdsTaxonomyCategories)
                {

                    List<object[]> categoryLinks = Global.Core.CategoryLinks.GetValues(new string[] { "Id" }, new string[] { "IdCategory", "IdTaxonomyCategory", "IdVariable", "IdTaxonomyVariable" }, new object[] { ArrIdsStudyCategories[i], taxonomyCategories, category.IdVariable, taxonomyCategory.IdTaxonomyVariable });

                    if (categoryLinks.Count() == 0)
                    {
                        CategoryLink categoryLink = new CategoryLink(Global.Core.CategoryLinks);
                        categoryLink.IdCategory = new Guid(ArrIdsStudyCategories[i]);
                        categoryLink.IdTaxonomyCategory = new Guid(taxonomyCategories);
                        categoryLink.IdVariable = category.IdVariable;
                        categoryLink.IdTaxonomyVariable = taxonomyCategory.IdTaxonomyVariable;
                        categoryLink.CreationDate = DateTime.Now;
                        categoryLink.Insert();
                    }
                }

            }
        }

        #endregion

        #region UTILITIES FUNCTIONS


        /// <summary>
        /// converts the comma seperated values to array of string
        /// </summary>
        /// <param name="commaSeperatedString">string needs to be comma seperated</param>
        /// <returns>comma seperated array values</returns>
        private static string[] SplitCommaSeperatedValues(string commaSeperatedString)
        {
            string[] values = commaSeperatedString.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim();
            }
            return values;
        }


        #endregion




    }
}
