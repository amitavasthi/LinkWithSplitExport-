using Crosstables.Classes.HierarchyClasses;
using DataCore.Classes;
using LinkBi1.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using VariableSelector1.Classes;
using WebUtilities;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für VariableSelector
    /// </summary>
    public class VariableSelector : BaseHandler
    {
        #region Constructor

        public VariableSelector()
            : base()
        {

            this.Methods.Add("CombineScales", CombineScales);

            this.Methods.Add("InsertMeanScore", InsertMeanScore);
            this.Methods.Add("InsertStandardDeviation", InsertStandardDeviation);
            this.Methods.Add("InsertStandardError", InsertStandardError);
            this.Methods.Add("InsertSampleVariance", InsertSampleVariance);

            this.Methods.Add("UpdateScoreGroupName", UpdateScoreGroupName);
            this.Methods.Add("UpdateScoreLabel", UpdateScoreLabel);

            this.Methods.Add("GetScore", GetScore);
            this.Methods.Add("GetScores", GetScores);
            this.Methods.Add("ReorderScale", ReorderScale);

            this.Methods.Add("DeleteScoreGroup", DeleteScoreGroup);
            this.Methods.Add("DeleteScore", DeleteScore);
            this.Methods.Add("RemoveScoreFromGroup", RemoveScoreFromGroup);

            this.Methods.Add("HideScore", HideScore);
            this.Methods.Add("ShowScore", ShowScore);

            this.Methods.Add("SetScoreFactor", SetScoreFactor);
            this.Methods.Add("SetScoreName", SetScoreName);

            this.Methods.Add("SetEquation", SetEquation);
            this.Methods.Add("GetEquation", GetEquation);
            this.Methods.Add("ValidateEquation", ValidateEquation);

            this.Methods.Add("DeleteVariable", DeleteVariable);
        }

        #endregion


        #region Web Methods

        private void CombineScales(HttpContext context)
        {
            string xPath = context.Request.Params["XPath"];

            string name = context.Request.Params["Name"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];
            string source2 = context.Request.Params["Source2"];

            string path = context.Request.Params["Path"];
            string path2 = context.Request.Params["Path2"];

            // Set the default for the language to english GB.
            int idLanguage = 2057;

            // Check if a specific language is defined.
            if (context.Request.Params["IdLanguage"] != null)
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);

            DefinitionObject score = new DefinitionObject(Global.Core, source, path);
            DefinitionObject score2 = new DefinitionObject(Global.Core, source2, path2);

            if (string.IsNullOrEmpty(path2))
                score2 = null;

            string resultPath = score.Combine(score2);

            //context.Response.Write(idScoreGroup.ToString());

            string result = RenderScoreToJson(new DefinitionObject(
                Global.Core,
                source,
                path
            ), idLanguage);

            context.Response.Write(result);
        }

        private void InsertMeanScore(HttpContext context)
        {
            string xPath = context.Request.Params["Path"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            XmlDocument document = new XmlDocument();
            document.Load(source);

            XmlNode xmlNodeVariable = document.SelectSingleNode(xPath).ParentNode;

            object idStudy = null;

            if (!bool.Parse(xmlNodeVariable.Attributes["IsTaxonomy"].Value))
            {
                idStudy = Global.Core.Variables.GetValue(
                    "IdStudy",
                     "Id",
                     Guid.Parse(xmlNodeVariable.Attributes["Id"].Value)
                );
            }

            DefinitionObject variable = new DefinitionObject(Global.Core, source, xmlNodeVariable);

            string meanEquation = RenderMeanEquation(
                source,
                variable,
                xmlNodeVariable,
                idStudy
            );

            xmlNodeVariable.InnerXml += (string.Format(
                "<ScoreGroup Id=\"{0}\" Name=\"\" ShowInChart=\"False\" Order=\"{2}\" Value=\"0\" Color=\"444444\" Label2057=\"Mean\" Equation=\"{1}\" HasValues=\"True\" SignificantLetter=\"F\"></ScoreGroup> ",
                Guid.NewGuid(),
                meanEquation,
                xmlNodeVariable.ChildNodes.Count
            ));

            document.Save(source);
        }

        private void InsertStandardDeviation(HttpContext context)
        {
            string xPath = context.Request.Params["Path"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            XmlDocument document = new XmlDocument();
            document.Load(source);

            XmlNode xmlNodeVariable = document.SelectSingleNode(xPath).ParentNode;

            object idStudy = null;

            if (!bool.Parse(xmlNodeVariable.Attributes["IsTaxonomy"].Value))
            {
                idStudy = Global.Core.Variables.GetValue(
                    "IdStudy",
                     "Id",
                     Guid.Parse(xmlNodeVariable.Attributes["Id"].Value)
                );
            }

            DefinitionObject variable = new DefinitionObject(Global.Core, source, xmlNodeVariable);

            string equation = RenderStdDevEquation(
                source,
                variable,
                xmlNodeVariable,
                idStudy
            );

            xmlNodeVariable.InnerXml += (string.Format(
                "<ScoreGroup Id=\"{0}\" ShowInChart=\"False\" Name=\"\" Order=\"{2}\" Value=\"0\" Color=\"444444\" Label2057=\"Standard deviation\" Equation=\"{1}\" HasValues=\"True\" SignificantLetter=\"F\"></ScoreGroup> ",
                Guid.NewGuid(),
                equation.ToString(),
                xmlNodeVariable.ChildNodes.Count
            ));

            document.Save(source);
        }

        private void InsertStandardError(HttpContext context)
        {
            string xPath = context.Request.Params["Path"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            XmlDocument document = new XmlDocument();
            document.Load(source);

            XmlNode xmlNodeVariable = document.SelectSingleNode(xPath).ParentNode;

            object idStudy = null;

            if (!bool.Parse(xmlNodeVariable.Attributes["IsTaxonomy"].Value))
            {
                idStudy = Global.Core.Variables.GetValue(
                    "IdStudy",
                     "Id",
                     Guid.Parse(xmlNodeVariable.Attributes["Id"].Value)
                );
            }

            DefinitionObject variable = new DefinitionObject(Global.Core, source, xmlNodeVariable);

            string equation = RenderStandardErrorEquation(
                source,
                variable,
                xmlNodeVariable,
                idStudy
            );

            xmlNodeVariable.InnerXml += (string.Format(
                "<ScoreGroup Id=\"{0}\" Name=\"\" ShowInChart=\"False\" Order=\"{2}\" Value=\"0\" Color=\"444444\" Label2057=\"Standard error\" Equation=\"{1}\" HasValues=\"True\" SignificantLetter=\"F\"></ScoreGroup> ",
                Guid.NewGuid(),
                equation.ToString(),
                xmlNodeVariable.ChildNodes.Count
            ));

            document.Save(source);
        }

        private void InsertSampleVariance(HttpContext context)
        {
            string xPath = context.Request.Params["Path"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            XmlDocument document = new XmlDocument();
            document.Load(source);

            XmlNode xmlNodeVariable = document.SelectSingleNode(xPath).ParentNode;

            object idStudy = null;

            if (!bool.Parse(xmlNodeVariable.Attributes["IsTaxonomy"].Value))
            {
                idStudy = Global.Core.Variables.GetValue(
                    "IdStudy",
                     "Id",
                     Guid.Parse(xmlNodeVariable.Attributes["Id"].Value)
                );
            }

            DefinitionObject variable = new DefinitionObject(Global.Core, source, xmlNodeVariable);

            string equation = RenderSampleVarianceEquation(
                source,
                variable,
                xmlNodeVariable,
                idStudy
            );

            xmlNodeVariable.InnerXml += (string.Format(
                "<ScoreGroup Id=\"{0}\" Name=\"\" ShowInChart=\"False\" Order=\"{2}\" Value=\"0\" Color=\"444444\" Label2057=\"Sample variance\" Equation=\"{1}\" HasValues=\"True\" SignificantLetter=\"F\"></ScoreGroup> ",
                Guid.NewGuid(),
                equation.ToString(),
                xmlNodeVariable.ChildNodes.Count
            ));

            document.Save(source);
        }

        private void ReorderScale(HttpContext context)
        {
            string xPath = context.Request.Params["XPath"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            DefinitionObject score = new DefinitionObject(Global.Core, source, xPath);

            int order = int.Parse(context.Request.Params["Order"]);

            int oldOrder = int.Parse(score.GetValue("Order").ToString());

            score.SetValue("Order", order);
            score.Save();

            DefinitionObject[] scores = score.GetParent().GetChilds();
            foreach (DefinitionObject _score in scores)
            {
                int _order = int.Parse(_score.GetValue("Order").ToString());

                if (_order < order)
                    continue;

                if (_order >= oldOrder)
                    continue;

                if (_score.GetValue("Id") == score.GetValue("Id"))
                    continue;

                _score.SetValue(
                    "Order",
                    (_order + 1)
                );

                _score.Save();
            }

            ReOrderScores(scores.OrderBy(x => int.Parse(x.GetValue("Order").ToString())).ToArray());
        }


        private void UpdateScoreGroupName(HttpContext context)
        {
            string path = context.Request.Params["Path"];

            string name = context.Request.Params["Value"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            // Set the default for the language to english GB.
            int idLanguage = 2057;

            // Check if a specific language is defined.
            if (context.Request.Params["IdLanguage"] != null)
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);

            DefinitionObject score = new DefinitionObject(Global.Core, source, path);
            score.SetLabel(idLanguage, name);

            score.Save();
        }

        private void UpdateScoreLabel(HttpContext context)
        {
            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            string xPath = context.Request.Params["XPath"];

            string label = context.Request.Params["Value"];

            DefinitionObject item = new DefinitionObject(Global.Core, source, xPath);

            // Set the default for the language to english GB.
            int idLanguage = 2057;

            // Check if a specific language is defined.
            if (context.Request.Params["IdLanguage"] != null)
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);

            item.SetLabel(idLanguage, label);

            item.Save();
        }


        private void GetScore(HttpContext context)
        {
            string xPath = context.Request.Params["Path"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            DefinitionObject item = new DefinitionObject(Global.Core, source, xPath);

            // Set the default for the language to english GB.
            int idLanguage = 2057;

            // Check if a specific language is defined.
            if (context.Request.Params["IdLanguage"] != null)
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);

            context.Response.Write(RenderScoreToJson(item, idLanguage));
        }

        private void GetScores(HttpContext context)
        {
            string xPath = context.Request.Params["XPath"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            DefinitionObject variable = new DefinitionObject(
                Global.Core,
                source,
                xPath
            );

            StringBuilder result = new StringBuilder();
            result.Append("{ \"Scores\" : [");

            // Set the default for the language to english GB.
            int idLanguage = 2057;

            // Check if a specific language is defined.
            if (context.Request.Params["IdLanguage"] != null)
                idLanguage = int.Parse(context.Request.Params["IdLanguage"]);

            DefinitionObject[] scores = variable.GetChilds();

            HierarchyFilter hierarchyFilter;
            
            if (context.Request.UrlReferrer.ToString().EndsWith("LinkBi.aspx"))
            {
                hierarchyFilter = Global.HierarchyFilters[(string)HttpContext.Current.Session["LinkBiDefinition"]];
            }
            else if(HttpContext.Current.Session["ReportDefinition"]!=null)
            {
                hierarchyFilter = Global.HierarchyFilters[(string)HttpContext.Current.Session["ReportDefinition"]];
            }
            else 
            {
                string fileName = System.IO.Path.Combine(
                   HttpContext.Current.Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "ExportDefinitions",
                   Global.User.Id + ".xml"
               );
                hierarchyFilter = Global.HierarchyFilters[fileName];
            }

            if (!hierarchyFilter.IsLoaded)
                hierarchyFilter.Load();

            bool isTaxonomy=false;

            if (context.Request.UrlReferrer.ToString().EndsWith("Exports.aspx")|| context.Request.UrlReferrer.ToString().EndsWith("TaxonomyManager.aspx"))
            {
                if (variable.GetValue("Id").ToString() != "")
                    isTaxonomy = true;
            }
            else
                isTaxonomy = bool.Parse((string)variable.GetValue("IsTaxonomy"));

            foreach (DefinitionObject score in scores)
            {
                object persistant = score.GetValue("Persistent", false, false);

                if (isTaxonomy && persistant != null)
                {
                    if (!hierarchyFilter.TaxonomyCategories.ContainsKey(Guid.Parse(score.GetValue("Id").ToString())))
                        continue;
                }

                result.Append(RenderScoreToJson(score, idLanguage));
                result.Append(",");
            }

            if (scores.Length > 0)
                result = result.Remove(result.Length - 1, 1);

            result.Append("]}");

            context.Response.Write(result.ToString());
        }



        private void DeleteScoreGroup(HttpContext context)
        {
            string xPath = context.Request.Params["XPath"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            DefinitionObject scoreGroup = new DefinitionObject(Global.Core, source, xPath);

            if (scoreGroup.StorageType == DatabaseCore.StorageMethodType.Database)
            {
                // Get all categories of the score group.
                List<object[]> scoreGroupCategories = Global.Core.TaxonomyCategoryLinks.GetValues(
                    new string[] { "Id" },
                    new string[] { "IdScoreGroup" },
                    new object[] { scoreGroup.GetValue("Id") }
                );

                // Run through all categories of the scrore group.
                foreach (object[] scoreGroupCategory in scoreGroupCategories)
                {
                    // Remove the category from the score group.
                    Global.Core.TaxonomyCategoryLinks.Delete((Guid)scoreGroupCategory[0]);
                }
            }

            scoreGroup.Delete();
        }

        private void DeleteScore(HttpContext context)
        {
            string xPath = context.Request.Params["XPath"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            DefinitionObject obj = new DefinitionObject(Global.Core, source, xPath);
            obj.Delete();
        }

        private void RemoveScoreFromGroup(HttpContext context)
        {
            string path = context.Request.Params["Path"];
            string groupPath = context.Request.Params["GroupPath"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];
            string groupSource = context.Request.Params["GroupSource"];

            DefinitionObject score = new DefinitionObject(Global.Core, source, path);
            DefinitionObject group = new DefinitionObject(Global.Core, groupSource, groupPath);

            if (score.StorageType == DatabaseCore.StorageMethodType.Database)
            {
                // Delete the score group link.
                Global.Core.TaxonomyCategoryLinks.Delete((Guid)Global.Core.TaxonomyCategoryLinks.GetValue(
                    "Id",
                    new string[] { "IdScoreGroup", "IdTaxonomyCategory" },
                    new object[] { group.GetValue("Id"), score.GetValue("Id") }
                ));
            }
            else
            {
                score.Delete();
            }
        }


        private void HideScore(HttpContext context)
        {
            string xPath = context.Request.Params["XPath"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            DefinitionObject item = new DefinitionObject(Global.Core, source, xPath);
            item.SetValue("Enabled", false);

            item.Save();
        }

        private void ShowScore(HttpContext context)
        {
            string xPath = context.Request.Params["XPath"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            DefinitionObject item = new DefinitionObject(Global.Core, source, xPath);
            item.SetValue("Enabled", true);

            item.Save();
        }


        private void SetScoreFactor(HttpContext context)
        {
            string xPath = context.Request.Params["Path"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            double value = 0.0;

            double.TryParse(context.Request.Params["Value"], out value);

            DefinitionObject item = new DefinitionObject(Global.Core, source, xPath);
            item.SetValue("Value", value);

            item.Save();
        }

        private void SetScoreName(HttpContext context)
        {
            string xPath = context.Request.Params["Path"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            string value = context.Request.Params["Value"];

            DefinitionObject item = new DefinitionObject(Global.Core, source, xPath);
            item.SetValue("Name", value);

            item.Save();
        }


        private void SetEquation(HttpContext context)
        {
            string path = context.Request.Params["Path"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            string equation = context.Request.Params["Equation"];

            DefinitionObject item = new DefinitionObject(Global.Core, source, path);
            item.SetValue("Equation", equation);

            item.Save();
        }

        private void GetEquation(HttpContext context)
        {
            string path = context.Request.Params["Path"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            DefinitionObject item = new DefinitionObject(Global.Core, source, path);

            context.Response.Write(HttpUtility.HtmlDecode((string)item.GetValue("Equation")));
        }

        private void ValidateEquation(HttpContext context)
        {
            List<EquationValidationError> errors = new List<EquationValidationError>();

            DataCore.Classes.StorageMethods.Database storageMethod =
                new DataCore.Classes.StorageMethods.Database(Global.Core, null);

            // Create a equation by the equation string
            // from the http request's parameters.
            Equation equation = new Equation(
                Global.Core,
                context.Request.Params["Equation"],
                storageMethod.WeightMissingValue
            );

            // Validate the equation.
            errors.AddRange(equation.Validate());

            // Perform a security check on the equation.
            errors.AddRange(equation.SecurityCheck(
                new Data(),
                storageMethod,
                null
            ));

            errors.AddRange(equation.SyntaxCheck(
                new Data(),
                storageMethod,
                null
            ));

            // Create a new string builder that contains the result JSON script.
            StringBuilder result = new StringBuilder();

            // Open the array that contains the rendered error messages.
            result.Append("[");

            // Run through all found validation errors.
            foreach (EquationValidationError error in errors)
            {
                result.Append("\"");

                // Render the error message to the result JSON script.
                result.Append(string.Format(
                    error.ToString().Trim()
                ));

                result.Append("\",");
            }

            if (errors.Count > 0)
                result = result.Remove(result.Length - 1, 1);

            // Open the array that contains the rendered error messages.
            result.Append("]");

            // Write the contents of the result
            // string builder to the http response.
            context.Response.Write(result.ToString());
        }


        private void DeleteVariable(HttpContext context)
        {
            string path = context.Request.Params["Path"];

            // Get the source string from the http request's parameters.
            string source = context.Request.Params["Source"];

            DefinitionObject item = new DefinitionObject(Global.Core, source, path);
            item.Delete();

            if (source.Contains("ReportDefinitions"))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(source);

                XmlNode xmlNodeResults = xmlDocument.DocumentElement.SelectSingleNode("Results");

                if (xmlNodeResults != null)
                {
                    xmlNodeResults.ParentNode.RemoveChild(xmlNodeResults);

                    xmlDocument.Save(source);
                }
            }
        }

        #endregion


        #region Methods

        private string RenderScoreToJson(DefinitionObject score, int idLanguage)
        {
            StringBuilder result = new StringBuilder();

            if (score.IsScoreGroup())
            {
                object _enabled = score.GetValue("Enabled");

                result.Append("{");
                result.Append(string.Format(
                    " \"Type\": \"ScoreGroup\", \"Id\": \"{0}\", \"Name\": \"{1}\", \"Order\": \"{2}\", \"Path\": \"{3}\", \"Source\": \"{4}\", \"Value\": \"{5}\", \"Enabled\": {6}, \"Color\":\"{7}\", \"Scores\": [",
                    score.GetValue("Id"),
                    HttpUtility.HtmlEncode(score.GetLabel(idLanguage)),
                    score.GetValue("Order"),
                    score.Path.Replace("\"", "\\\""),
                    score.Source,
                    score.GetValue("Value"),
                    _enabled != null ? _enabled.ToString().ToLower() : true.ToString().ToLower(),
                    score.GetValue("Color")
                ));

                /*foreach (XmlNode xmlNodeScore in xmlNode.ChildNodes)
                {
                    result.Append(RenderScoreToJson(xmlNodeScore, idLanguage));
                    result.Append(",");
                }*/
                DefinitionObject[] childScores = score.GetChilds();
                foreach (DefinitionObject childScore in childScores)
                {
                    result.Append(RenderScoreToJson(childScore, idLanguage));
                    result.Append(",");
                }

                if (childScores.Length > 0)
                    result = result.Remove(result.Length - 1, 1);

                result.Append("] }");
            }
            else
            {

                object _enabled = score.GetValue("Enabled");

                result.Append("{");
                result.Append(string.Format(
                    " \"Type\": \"{0}\", \"Id\": \"{1}\", \"Label\": \"{2}\", \"Order\": \"{3}\", \"Path\": \"{4}\", \"Enabled\": {5}, \"Source\": \"{6}\", \"Name\": \"{7}\", \"Value\": \"{8}\" ",
                    score.TypeName,
                    score.GetValue("Id"),
                    HttpUtility.HtmlEncode(score.GetLabel(idLanguage)),
                    score.GetValue("Order"),
                    score.Path.Replace("\"", "\\\""),
                    _enabled != null ? _enabled.ToString().ToLower() : true.ToString().ToLower(),
                    score.Source,
                    HttpUtility.HtmlEncode(score.GetValue("Name")),
                    score.GetValue("Value")
                ));
                result.Append("}");
            }

            return result.ToString();
        }

        private void ReOrderScores(DefinitionObject[] scores)
        {
            int order = 0;

            foreach (DefinitionObject score in scores)
            {
                score.SetValue("Order", order);

                order++;
            }
        }


        private string RenderMeanEquation(
            string source,
            DefinitionObject variable,
            XmlNode xmlNodeVariable,
            object idStudy
        )
        {
            StringBuilder meanEquation = new StringBuilder();

            // Run through all category ids of the variable.
            foreach (XmlNode xmlNodeCategory in xmlNodeVariable.ChildNodes)
            {
                if (xmlNodeCategory.Name == "ScoreGroup")
                    continue;

                DefinitionObject category = new DefinitionObject(Global.Core, source, xmlNodeCategory);

                meanEquation.Append(string.Format(
                    "([{3}{0}.{1}] * {2} / [{3}{0}]) + ",
                    variable.GetValue("Name"),
                    category.GetValue("Name"),
                    category.GetValue("Value"),
                    idStudy != null ? "/" + idStudy + "\\" : ""
                ));
            }

            if (xmlNodeVariable.ChildNodes.Count != 0)
                meanEquation = meanEquation.Remove(meanEquation.Length - 3, 3);

            return meanEquation.ToString();
        }

        private string RenderStdDevEquation(
            string source,
            DefinitionObject variable,
            XmlNode xmlNodeVariable,
            object idStudy
        )
        {
            StringBuilder meanEquation = new StringBuilder();

            // Run through all category ids of the variable.
            foreach (XmlNode xmlNodeCategory in xmlNodeVariable.ChildNodes)
            {
                if (xmlNodeCategory.Name == "ScoreGroup")
                    continue;

                DefinitionObject category = new DefinitionObject(Global.Core, source, xmlNodeCategory);

                meanEquation.Append(string.Format(
                    "([{3}{0}.{1}] * {2}) + ",
                    variable.GetValue("Name"),
                    category.GetValue("Name"),
                    category.GetValue("Value"),
                    idStudy != null ? "/" + idStudy + "\\" : ""
                ));
            }

            if (xmlNodeVariable.ChildNodes.Count != 0)
                meanEquation = meanEquation.Remove(meanEquation.Length - 3, 3);

            StringBuilder equation = new StringBuilder();

            equation.Append("double var_mean = Math.Pow(");
            equation.Append(meanEquation.ToString());
            equation.Append(string.Format(
                ", 2) / [{1}{0}];",
                variable.GetValue("Name"),
                idStudy != null ? "/" + idStudy + "\\" : ""
            ));

            equation.Append(Environment.NewLine);
            equation.Append("double var_result = ");

            // Run through all category ids of the variable.
            foreach (XmlNode xmlNodeCategory in xmlNodeVariable.ChildNodes)
            {
                if (xmlNodeCategory.Name == "ScoreGroup")
                    continue;

                DefinitionObject category = new DefinitionObject(Global.Core, source, xmlNodeCategory);

                equation.Append(string.Format(
                    "([{3}{0}.{1}] * Math.Pow({3}{2}, 2)) + ",
                    variable.GetValue("Name"),
                    category.GetValue("Name"),
                    category.GetValue("Value"),
                    idStudy != null ? "/" + idStudy + "\\" : ""
                ));
            }

            if (xmlNodeVariable.ChildNodes.Count != 0)
                equation = equation.Remove(equation.Length - 3, 3);

            equation.Append(" - var_mean");

            equation.Append(";");
            equation.Append(Environment.NewLine);
            equation.Append(Environment.NewLine);

            equation.Append(string.Format(
                "var_result = var_result / ([{1}{0}] - 1);",
                variable.GetValue("Name"),
                idStudy != null ? "/" + idStudy + "\\" : ""
            ));

            equation.Append(Environment.NewLine);
            equation.Append("return Math.Pow(var_result, 0.5);");

            return equation.ToString();
        }

        private string RenderStandardErrorEquation(
            string source,
            DefinitionObject variable,
            XmlNode xmlNodeVariable,
            object idStudy
        )
        {
            StringBuilder result = new StringBuilder();

            result.Append(RenderStdDevEquation(
                source,
                variable,
                xmlNodeVariable,
                idStudy
            ).Replace("return", "var_result = "));

            result.Append(Environment.NewLine);
            result.Append(Environment.NewLine);

            result.Append(string.Format(
                "return var_result / Math.Pow([{1}{0}] - 1, 0.5);",
                variable.GetValue("Name"),
                idStudy != null ? "/" + idStudy + "\\" : ""
            ));

            return result.ToString();
        }

        private string RenderSampleVarianceEquation(
            string source,
            DefinitionObject variable,
            XmlNode xmlNodeVariable,
            object idStudy
        )
        {
            StringBuilder result = new StringBuilder();

            result.Append(RenderStdDevEquation(
                source,
                variable,
                xmlNodeVariable,
                idStudy
            ).Replace("return", "var_result = "));

            result.Append(Environment.NewLine);
            result.Append(Environment.NewLine);

            result.Append(string.Format(
                "return Math.Sqrt(var_result / Math.Pow([{1}{0}] - 1, 0.5));",
                variable.GetValue("Name"),
                idStudy != null ? "/" + idStudy + "\\" : ""
            ));

            return result.ToString();
        }

        #endregion
    }
}