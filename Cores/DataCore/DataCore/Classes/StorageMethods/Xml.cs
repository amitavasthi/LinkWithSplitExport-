using Crosstables.Classes.ReportDefinitionClasses.Collections;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataCore.Classes.StorageMethods
{
    public class Xml
    {
        #region Properties

        public string FileStorageFolder { get; set; }

        #endregion


        #region Constructor

        public Xml(string fileStorageFolder)
        {
            this.FileStorageFolder = fileStorageFolder;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Checks if there is data for a category.
        /// </summary>
        /// <param name="idCategory">The id of the category to check data for.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public bool HasData(Guid idCategory, Data filter = null)
        {
            // Load the report file.
            XmlDocument xmlDocument = LoadReportFile(idCategory);

            // Build the xPath selection.
            string xPath = "Response";

            // Select the response xml nodes.
            XmlNodeList xmlNodes = xmlDocument.DocumentElement.SelectNodes(xPath);

            // Create a new collection containing the result respondent ids.
            Dictionary<Guid, double[]> result = new Dictionary<Guid, double[]>();

            // Run through all response xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                // Get the response's respondent id.
                Guid idRespondent = Guid.Parse(
                    xmlNode.Attributes["IdRespondent"].Value
                );

                if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                    continue;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates the values for a variable.
        /// </summary>
        /// <param name="variable">The variable to get responses from.</param>
        /// <param name="idRespondents">The respondent filter.</param>
        /// <returns></returns>
        public Dictionary<Guid, double> GetValues(Variable variable, Guid[] idRespondents)
        {
            Dictionary<Guid, double> categoryValues = new Dictionary<Guid, double>();

            // Get all categories of the variable.
            Category[] categories = variable.Categories;

            // Run through all categories of the variable.
            foreach (Category category in categories)
                categoryValues.Add(category.Id, 0.0);

            // Load the report file.
            XmlDocument xmlDocument = LoadReportFile(variable.Id);

            // Create a new string builder for building the xPath selection string.
            StringBuilder xPathBuilder = new StringBuilder();

            // Append the report xml node selection to the xPath.
            xPathBuilder.Append("Report");

            // Check if there is a filter defined.
            if (idRespondents.Length > 0)
                xPathBuilder.Append("[");

            // Run through all filtered respondents.
            for (int i = 0; i < idRespondents.Length; i++)
            {
                // Add the respondent selection to the xPath.
                xPathBuilder.Append(string.Format(
                    "@IdRespondent=\"{0}\"",
                    idRespondents[i]
                ));

                if (i < (idRespondents.Length - 1))
                    xPathBuilder.Append(" or ");
            }

            // Check if there is a filter defined.
            if (idRespondents.Length > 0)
                xPathBuilder.Append("]");

            // Select the response xml nodes.
            XmlNodeList xmlNodes = xmlDocument.DocumentElement.SelectNodes(xPathBuilder.ToString());

            // Run through all xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                // Parse the id of the response's category
                Guid idCategory = Guid.Parse(
                    xmlNode.Attributes["IdCategory"].Value
                );

                // ToDo: Parse the weight of the response's respondent.
                double weight = 1.0;

                // Add the weight value to the category's value.
                categoryValues[idCategory] += weight;
            }

            return categoryValues;
        }

        /// <summary>
        /// Calculates the values for a category.
        /// </summary>
        /// <param name="variable">The category to get the value from.</param>
        /// <param name="idRespondents">The respondent filter.</param>
        /// <returns></returns>
        public double GetValue(Category category, Guid[] idRespondents, bool weighted = true)
        {
            // Load the report file.
            XmlDocument xmlDocument = LoadReportFile(category.Id);

            double result = 0.0;

            // Create a new string builder for building the xPath selection string.
            StringBuilder xPathBuilder = new StringBuilder();

            // Append the report xml node selection to the xPath.
            xPathBuilder.Append("Response");

            xPathBuilder.Append("[");
            // Add the respondent selection to the xPath.
            xPathBuilder.Append("@IdRespondent=\"{0}\"");

            xPathBuilder.Append("]");

            // Run through all filtered respondents.
            for (int i = 0; i < idRespondents.Length; i++)
            {
                /*
                // Add the category selection to the xPath.
                xPathBuilder.Append(string.Format(
                    "@IdCategory=\"{0}\"",
                    category.Id
                ));

                xPathBuilder.Append(" and ");
                */

                XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(string.Format(
                    xPathBuilder.ToString(),
                    idRespondents[i]
                ));

                if (xmlNode == null)
                    continue;

                // Check if the data should be weighted.
                if (!weighted)
                {
                    result += 1;

                    continue;
                }

                // ToDo: Parse the weight of the response's respondent.
                double weight = 1.0;

                // Return the weight value.
                result += weight;
            }

            return result;
        }

        /// <summary>
        /// Gets the respondents of a category.
        /// </summary>
        /// <param name="idCategory">The id of the category to get the respondents for.</param>
        public Dictionary<Guid, double[]> GetRespondents(Guid idCategory, Dictionary<Guid, double[]> filter = null, WeightingFilterCollection weightingFilters = null)
        {
            throw new NotImplementedException();
            /*
            // Load the report file.
            XmlDocument xmlDocument = LoadReportFile(idCategory);

            // Build the xPath selection.
            string path = "Response";

            // Create a new collection containing the result respondent ids.
            Dictionary<Guid, double[]> result = new Dictionary<Guid, double[]>();

            // Select the response xml nodes.
            XmlNodeList xmlNodes = xmlDocument.DocumentElement.SelectNodes(path);

            // Run through all response xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                // Get the response's respondent id.
                Guid idRespondent = Guid.Parse(
                    xmlNode.Attributes["IdRespondent"].Value
                );

                if (filter != null && filter.ContainsKey(idRespondent) == false)
                    continue;

                string weightingVariable = null;

                if (weightingFilters != null)
                {
                    // Run through all weighting filters.
                    foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                    {
                        // Check if the weighting filter applies to the respondent.
                        if (weightingFilter.Respondents.Contains(idRespondent))
                        {
                            weightingVariable = weightingFilter.WeightingVariable;

                            break;
                        }
                    }

                    if (weightingVariable == null && weightingFilters.DefaultWeighting != null)
                        weightingVariable = weightingFilters.DefaultWeighting;
                }

                // Check if the respondent was already added to the result.
                if (!result.ContainsKey(idRespondent))
                    result.Add(idRespondent, new double[1]);

                if (weightingVariable != null)
                {
                    double weight = 1.0;

                    if (xmlNode.Attributes[weightingVariable] != null)
                        double.TryParse(xmlNode.Attributes[weightingVariable].Value, out weight);

                    result[idRespondent][0] += weight;
                }
                else
                {
                    result[idRespondent][0] += 1.0;
                }
            }

            // Return the result list of respondent ids.
            return result;*/
        }

        /// <summary>
        /// Gets the respondents of a numeric variable.
        /// </summary>
        /// <param name="idVariable">The id of the numeric variable to get the respondents for.</param>
        public Dictionary<Guid, double[]> GetRespondentsNumeric(Guid idVariable, Dictionary<Guid, double[]> filter = null)
        {
            // Load the report file.
            XmlDocument xmlDocument = LoadReportFile(idVariable);

            // Build the xPath selection.
            string xPath = "Response";

            // Select the response xml nodes.
            XmlNodeList xmlNodes = xmlDocument.DocumentElement.SelectNodes(xPath);

            // Create a new collection containing the result respondent ids.
            Dictionary<Guid, double[]> result = new Dictionary<Guid, double[]>();

            // Run through all response xml nodes.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                // Get the response's respondent id.
                Guid idRespondent = Guid.Parse(
                    xmlNode.Attributes["IdRespondent"].Value
                );

                if (filter != null && filter.ContainsKey(idRespondent) == false)
                    continue;

                // Check if the respondent was already added to the result.
                if (!result.ContainsKey(idRespondent))
                    result.Add(idRespondent, new double[1]);

                double numericValue = 0.0;

                // Parse the response's numeric answer.
                double.TryParse(
                    xmlNode.Attributes["NumericAnswer"].Value,
                    out numericValue
                );

                result[idRespondent][0] += numericValue;
            }

            // Return the result list of respondent ids.
            return result;
        }

        private XmlDocument LoadReportFile(Guid idVariable)
        {
            // For test only:
            string fileName = Path.Combine(
                this.FileStorageFolder,
                "[resp].[Var_" + idVariable + "].xml"
            );

            // Create a new xml document 
            XmlDocument xmlDocument = new XmlDocument();

            StringBuilder xmlString = new StringBuilder();
            xmlString.Append("<Responses>");

            if (File.Exists(fileName))
                xmlString.Append(File.ReadAllText(fileName));

            xmlString.Append("</Responses>");

            xmlDocument.LoadXml(xmlString.ToString());

            return xmlDocument;
        }

        #endregion
    }
}
