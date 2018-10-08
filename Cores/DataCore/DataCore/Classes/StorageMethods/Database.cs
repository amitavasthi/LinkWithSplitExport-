using ApplicationUtilities.Classes;
using Crosstables.Classes.ReportDefinitionClasses;
using Crosstables.Classes.ReportDefinitionClasses.Collections;
using DatabaseCore.Items;
using LinkBi1.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataCore.Classes.StorageMethods
{
    public class Database
    {
        #region Properties

        public int WeightMissingValue { get; set; }

        public ReportCalculator Owner { get; set; }

        public DatabaseCore.Core Core { get; set; }

        public static Dictionary<string, CaseDataCache> Cache { get; set; }

        #endregion


        #region Constructor

        public Database(DatabaseCore.Core core, ReportCalculator owner, int weightMissingValue = 1)
        {
            this.WeightMissingValue = weightMissingValue;
            this.Core = core;
            this.Owner = owner;

            if (Cache == null)
                Cache = new Dictionary<string, CaseDataCache>();

            lock (Cache)
            {
                if (!Cache.ContainsKey(core.ClientName))
                    Cache.Add(core.ClientName, new CaseDataCache());
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Checks if there is data for a category.
        /// </summary>
        /// <param name="idCategory">The id of the category to check data for.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public bool HasData(Guid idVariable, bool isTaxonomy)
        {
            List<string> commandTexts = new List<string>();

            if (!isTaxonomy)
            {
                commandTexts.Add(string.Format(
                    "SELECT Count(*) FROM [resp].[Var_{0}]",
                    idVariable
                ));
            }
            else
            {
                List<object[]> variableLinks;

                variableLinks = this.Core.VariableLinks.GetValues(
                    new string[] { "IdVariable" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { idVariable }
                );

                // Run through all linked variable ids.
                foreach (object[] variableLink in variableLinks)
                {
                    commandTexts.Add(string.Format(
                        "SELECT Count(*) FROM [resp].[Var_{0}]",
                        variableLink[0]
                    ));
                }
            }


            foreach (string commandText in commandTexts)
            {
                int count = (int)this.Core.Respondents.ExecuteReader(commandText, typeof(int))[0][0];

                if (count > 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if there is data for a category.
        /// </summary>
        /// <param name="idCategory">The id of the category to check data for.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public bool HasData(Guid idVariable, bool isTaxonomy, Data filter)
        {
            if (filter == null)
            {
                return HasData(idVariable, isTaxonomy);
            }

            bool result = false;

            List<string> commandTexts = new List<string>();

            if (!isTaxonomy)
            {
                commandTexts.Add(string.Format(
                    "SELECT [IdRespondent] FROM [resp].[Var_{0}]",
                    idVariable
                ));
            }
            else
            {
                List<object[]> variableLinks;

                variableLinks = this.Core.VariableLinks.GetValues(
                    new string[] { "IdVariable" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { idVariable }
                );

                // Run through all linked variable ids.
                foreach (object[] variableLink in variableLinks)
                {
                    commandTexts.Add(string.Format(
                        "SELECT [IdRespondent] FROM [resp].[Var_{0}]",
                        variableLink[0]
                    ));
                }
            }

            foreach (string commandText in commandTexts)
            {
                List<object[]> values = this.Core.Respondents.ExecuteReader(commandText, typeof(Guid));

                foreach (object[] value in values)
                {
                    Guid idRespondent = (Guid)value[0];

                    if (filter.Responses.ContainsKey(idRespondent))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if there is data for a category.
        /// </summary>
        /// <param name="idCategory">The id of the category to check data for.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public bool HasData(Guid idVariable, Guid idCategory, Data filter = null)
        {
            SqlConnection sqlConnection = new SqlConnection(this.Core.ConnectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "SELECT Count(*) FROM [resp].[Var_" + idVariable + "] WHERE [IdCategory]='" + idCategory + "'",
                sqlConnection
            );

            sqlConnection.Open();

            int count = 0;

            try
            {
                count = (int)sqlCommand.ExecuteScalar();
            }
            finally
            {
                sqlConnection.Close();
                sqlCommand.Dispose();
                sqlConnection.Dispose();
            }

            return count > 0;
        }

        /// <summary>
        /// Gets the respondents of a category.
        /// </summary>
        /// <param name="idVariable">The id of the variable to get the respondents for.</param>
        public Data GetRespondents(
            ReportDefinitionVariable variable,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null,
            Equation equation = null
        )
        {
            Data result = new Data();

            if (variable.IsFake)
                return filter;

            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            if (equation != null)
            {
                //Equation equation = new Equation(this.Core, equationString);

                /*if (equation.Validate().Count > 0)
                    return new Data();*/

                double value = 0.0;

                string[] values = equation.Calculate(
                    this.Owner != null ? this.Owner.BaseFilter : null,
                    filter,
                    this,
                    weightingFilters
                );

                double.TryParse(values[0], out value);

                if (double.IsNaN(value))
                    value = 0;

                result.Base = filter.Base;
                result.UnweightedValue = filter.Responses.Count;
                result.Value = value;

                double.TryParse(values[1], out value);

                if (double.IsNaN(value))
                    value = 0;

                //result.UnweightedValue = (int)value;

                return result;
            }

            List<string> commandTexts = new List<string>();

            if (variable.VariableType == VariableType.Numeric)
            {
                InitDataNumeric(
                    variable.IdVariable,
                    variable.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }
            else if (variable.VariableType == VariableType.Text)
            {
                InitDataText(
                    variable.IdVariable,
                    variable.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }
            else
            {
                InitData(
                    variable.IdVariable,
                    variable.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }
            double sqrWiegteBase = 0;
            double categoryEffectivebase = 0;
            if (variable.VariableType == VariableType.Numeric)
            {
                return GetRespondentsNumeric(
                    variable,
                    this.Core.CaseDataLocation,
                    filter,
                    weightingFilters
                );
            }
            else if (variable.VariableType == VariableType.Text)
            {
                //foreach (Guid idRespondent in variable.Data.Respondents.Keys)
                //foreach (Guid idRespondent in variable.Data.VariableRespondents.Keys)
                //ProcessRespondents(variable.Data.VariableRespondents, delegate (Guid idRespondent)
                List<Guid> respondents = Cache[this.Core.ClientName].GetRespondents(variable.IdVariable);

                foreach (Guid idRespondent in respondents)
                {
                    if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                        continue;

                    double weight = 1.0;

                    if (weightingFilters != null)
                    {
                        weight = this.WeightMissingValue;
                        bool useDefault = true;

                        // Run through all weighting filters.
                        foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                        {
                            // Check if the weighting filter applies to the respondent.
                            if (weightingFilter.Respondents.ContainsKey(idRespondent))
                            {
                                weight = weightingFilter.Respondents[idRespondent];

                                useDefault = false;

                                break;
                            }
                        }

                        if (useDefault && weightingFilters.DefaultWeighting != null)
                        {
                            if (weightingFilters.Respondents.ContainsKey(idRespondent))
                            {
                                weight = weightingFilters.Respondents[idRespondent];
                            }
                        }
                    }

                    if (!result.Responses.ContainsKey(idRespondent))
                    {
                        result.Responses.Add(idRespondent, new double[1]);

                        result.Base += weight;
                    }

                    //weight *= variable.Data.VariableRespondents[idRespondent];

                    result.Value += weight;
                    result.Responses[idRespondent][0] += weight;
                }
            }
            else
            {
                //foreach (Guid idCategory in variable.Data.CategoryData.Keys)
                List<Guid> categories = Cache[this.Core.ClientName].GetCategories(
                    variable.IdVariable
                );
                foreach (Guid idCategory in categories)
                {
                    List<Guid> respondents = Cache[this.Core.ClientName].GetRespondents(variable.IdVariable, idCategory);
                    foreach (Guid idRespondent in respondents)
                    //ProcessRespondents(variable.Data[idCategory], delegate (Guid idRespondent)
                    {
                        if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                            continue;

                        double weight = 1.0;

                        if (weightingFilters != null)
                        {
                            weight = this.WeightMissingValue;
                            bool useDefault = true;

                            // Run through all weighting filters.
                            foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                            {
                                // Check if the weighting filter applies to the respondent.
                                if (weightingFilter.Respondents.ContainsKey(idRespondent))
                                {
                                    weight = weightingFilter.Respondents[idRespondent];

                                    useDefault = false;

                                    break;
                                }
                            }

                            if (useDefault && weightingFilters.DefaultWeighting != null)
                            {
                                if (weightingFilters.Respondents.ContainsKey(idRespondent))
                                {
                                    weight = weightingFilters.Respondents[idRespondent];
                                }
                            }
                        }

                        if (!result.Responses.ContainsKey(idRespondent))
                        {
                            result.Responses.Add(idRespondent, new double[1]);

                            result.Base += weight;
                            sqrWiegteBase += Math.Pow(weight, 2);
                            categoryEffectivebase += weight;

                        }

                        //weight *= variable.Data[idCategory][idRespondent];                      
                        result.Value += weight;
                        result.Responses[idRespondent][0] += weight;
                    }
                    //to avoid NaN check below condition
                    if (sqrWiegteBase != 0)
                        result.EffectiveValue += Math.Pow(categoryEffectivebase, 2) / sqrWiegteBase;
                    sqrWiegteBase = 0;
                    categoryEffectivebase = 0;
                }


            }

            result.UnweightedValue = result.Responses.Count;
            if (result.EffectiveValue == 0)
            {
                result.EffectiveValue = result.Responses.Count;
            }

            return result;
        }

        /// <summary>
        /// Gets the respondents of a category.
        /// </summary>
        /// <param name="idCategory">The id of the category to get the respondents for.</param>
        public Data GetRespondents(
            ReportDefinitionScore score,
            ReportDefinitionVariable variable,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null,
            Equation equation = null
        )
        {
            Data result = new Data();

            if (equation == null && score.Equation != null)
                return filter;

            if (equation != null)
            {
                //Equation equation = new Equation(this.Core, equationString);

                /*if (equation.Validate().Count > 0)
                    return new Data();*/

                double value = 0.0;

                string[] equationValues = equation.Calculate(
                    this.Owner != null ? this.Owner.BaseFilter : null,
                    filter,
                    this,
                    weightingFilters
                );

                double.TryParse(equationValues[0], out value);

                if (double.IsNaN(value))
                    value = 0;

                if (filter == null)
                {
                    filter = GetRespondents(variable, null, weightingFilters);
                }

                // ToDo: Calculate unweighted value.
                result.Base = filter.Base;
                result.UnweightedValue = filter.Responses.Count;
                result.Value = value;

                double.TryParse(equationValues[1], out value);

                if (double.IsNaN(value))
                    value = 0;

                //result.UnweightedValue = value;

                return result;
            }

            if (variable.VariableType == VariableType.Text)
            {
                return GetRespondentsText(
                    score,
                    variable.IdVariable,
                    variable.IsTaxonomy,
                    filter,
                    weightingFilters
                );
            }

            if (variable.IsFake)
                return filter;

            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            Dictionary<Guid, int> values;

            if (score.GetType() == typeof(ReportDefinitionScoreGroup))
            {
                values = new Dictionary<Guid, int>();

                ReportDefinitionScoreGroup scoreGroup = (ReportDefinitionScoreGroup)score;

                foreach (ReportDefinitionScore scoreGroupScore in scoreGroup.Scores)
                {
                    if (!Cache[this.Core.ClientName].HasData(variable.IdVariable, scoreGroupScore.Identity))
                        continue;

                    List<Guid> respondents = Cache[this.Core.ClientName].GetRespondents(variable.IdVariable, scoreGroupScore.Identity);
                    foreach (Guid idRespondent in respondents)
                    {
                        if (!values.ContainsKey(idRespondent))
                            values.Add(idRespondent, 0);

                        values[idRespondent]++;
                    }
                }
            }
            else
            {
                if (!Cache[this.Core.ClientName].HasData(variable.IdVariable, score.Identity))
                    return new Data();

                values = Cache[this.Core.ClientName].GetResponses(variable.IdVariable, score.Identity);
            }

            double var_result = 0.0;
            double var_result2 = 0.0;

            foreach (Guid idRespondent in values.Keys)
            //ProcessRespondents(values, delegate (Guid idRespondent)
            {
                if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                    continue;

                double weight = 1.0;

                if (weightingFilters != null)
                {
                    weight = this.WeightMissingValue;
                    bool useDefault = true;

                    // Run through all weighting filters.
                    foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                    {
                        // Check if the weighting filter applies to the respondent.
                        if (weightingFilter.Respondents.ContainsKey(idRespondent))
                        {
                            weight = weightingFilter.Respondents[idRespondent];

                            useDefault = false;

                            break;
                        }
                    }

                    if (useDefault && weightingFilters.DefaultWeighting != null)
                    {
                        if (weightingFilters.Respondents.ContainsKey(idRespondent))
                            weight = weightingFilters.Respondents[idRespondent];
                    }
                }

                if (!result.Responses.ContainsKey(idRespondent))
                {
                    result.Responses.Add(idRespondent, new double[1]);

                    result.Base += weight;
                }

                //weight *= values[idRespondent];

                result.Value += weight;
                result.Responses[idRespondent][0] += weight;

                var_result += Math.Pow(weight, 2);
                var_result2 += weight;
            }

            result.EffectiveBase = Math.Pow(var_result2, 2) / var_result;

            result.UnweightedValue = result.Responses.Count;

            return result;
        }

        public void Test(
            int startIndex,
            int steps,
            Guid[] respondents,
            Data filter,
            WeightingFilterCollection weightingFilters,
            Data result
        )
        {
            int index = startIndex;

            while (index < respondents.Length)
            {
                Guid idRespondent = respondents[index];
                index += steps;

                if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                    continue;

                double weight = 1.0;

                if (weightingFilters != null)
                {
                    weight = this.WeightMissingValue;
                    bool useDefault = true;

                    // Run through all weighting filters.
                    foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                    {
                        // Check if the weighting filter applies to the respondent.
                        if (weightingFilter.Respondents.ContainsKey(idRespondent))
                        {
                            weight = weightingFilter.Respondents[idRespondent];

                            useDefault = false;

                            break;
                        }
                    }

                    if (useDefault && weightingFilters.DefaultWeighting != null)
                    {
                        if (weightingFilters.Respondents.ContainsKey(idRespondent))
                            weight = weightingFilters.Respondents[idRespondent];
                    }
                }

                if (!result.Responses.ContainsKey(idRespondent))
                {
                    result.Responses.Add(idRespondent, new double[1]);

                    result.Base += weight;
                }

                result.Value += weight;
                result.Responses[idRespondent][0] += weight;
            }
        }

        /// <summary>
        /// Gets the respondents of a category.
        /// </summary>
        /// <param name="idCategory">The id of the category to get the respondents for.</param>
        public Data GetRespondents(
            Guid idCategory,
            Guid idVariable,
            bool isTaxonomy,
            CaseDataLocation caseDataLocation,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null,
            Equation equation = null
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            Data result = new Data();

            if (equation != null)
            {
                //Equation equation = new Equation(this.Core, equationString);

                /*if (equation.Validate().Count > 0)
                    return new Data();*/

                double value = 0.0;

                string[] values = equation.Calculate(
                    this.Owner != null ? this.Owner.BaseFilter : null,
                    filter,
                    this,
                    weightingFilters
                );

                double.TryParse(values[0], out value);

                if (double.IsNaN(value))
                    value = 0;

                result.Base = filter.Base;
                result.UnweightedValue = filter.Responses.Count;
                result.Value = value;

                double.TryParse(values[1], out value);

                if (double.IsNaN(value))
                    value = 0;

                //result.UnweightedValue = (int)value;

                return result;
            }

            if (!Cache[this.Core.ClientName].HasData(idVariable, idCategory))
            {
                InitData(idVariable, isTaxonomy, caseDataLocation);
            }

            //foreach (Guid idRespondent in Cache[this.Core.ClientName].CategoryData[idVariable][idCategory].Keys)
            List<Guid> respondents = Cache[this.Core.ClientName].GetRespondents(idVariable, idCategory);
            double sqrWiegteBase = 0;
            foreach (Guid idRespondent in respondents)
            //ProcessRespondents(Cache[this.Core.ClientName].CategoryData[idVariable][idCategory], delegate (Guid idRespondent)
            {
                if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                    continue;

                double weight = 1.0;

                if (weightingFilters != null)
                {
                    weight = this.WeightMissingValue;
                    bool useDefault = true;

                    // Run through all weighting filters.
                    foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                    {
                        // Check if the weighting filter applies to the respondent.
                        if (weightingFilter.Respondents.ContainsKey(idRespondent))
                        {
                            weight = weightingFilter.Respondents[idRespondent];

                            useDefault = false;

                            break;
                        }
                    }

                    if (useDefault && weightingFilters.DefaultWeighting != null)
                    {
                        if (weightingFilters.Respondents.ContainsKey(idRespondent))
                            weight = weightingFilters.Respondents[idRespondent];
                    }
                }

                if (!result.Responses.ContainsKey(idRespondent))
                {
                    result.Responses.Add(idRespondent, new double[1]);

                    result.Base += weight;

                }

                //weight *= Cache[this.Core.ClientName].CategoryData[idVariable][idCategory][idRespondent];
                sqrWiegteBase += weight * weight;
                result.Value += weight;
                result.Responses[idRespondent][0] += weight;
            }

            result.UnweightedValue = result.Responses.Count;
            if (sqrWiegteBase == 0)
            {
                result.EffectiveValue = result.Responses.Count;
            }
            else
            {
                result.EffectiveValue = (result.Base * result.Base) / sqrWiegteBase;
            }
            sqrWiegteBase = 0;

            return result;
        }

        /// <summary>
        /// Gets the respondents of a category.
        /// </summary>
        /// <param name="idCategory">The id of the category to get the respondents for.</param>
        public Data GetRespondents(
            LinkBiDefinitionDimensionScore score,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null,
            Equation equation = null
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            Data result = new Data();

            if (equation != null)
            {
                /*Equation equation = new Equation(this.Core, equationString);

                if (equation.Validate().Count > 0)
                    return new Data();*/

                double value = 0.0;

                string[] values = equation.Calculate(
                    this.Owner != null ? this.Owner.BaseFilter : null,
                    filter,
                    this,
                    weightingFilters
                );

                double.TryParse(values[0], out value);

                if (double.IsNaN(value))
                    value = 0;

                result.Base = filter.Base;
                result.UnweightedValue = filter.Responses.Count;
                result.Value = value;

                double.TryParse(values[1], out value);

                if (double.IsNaN(value))
                    value = 0;

                //result.UnweightedValue = (int)value;

                return result;
            }

            if (score.Owner.VariableType == VariableType.Numeric)
            {
                InitDataNumeric(
                    score.Owner.Identity,
                    score.Owner.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }
            else if (score.Owner.VariableType == VariableType.Text)
            {
                /*InitDataText(
                    score.Owner.Identity,
                    score.Owner.IsTaxonomy,
                    this.Core.CaseDataLocation
                );*/
                return GetRespondentsText(
                    score.XmlNode.Attributes["Text"].Value,
                    score.Owner.Identity,
                    score.Owner.IsTaxonomy,
                    filter,
                    weightingFilters
                );
            }
            else
            {
                InitData(
                    score.Owner.Identity,
                    score.Owner.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }

            if (!Cache[this.Core.ClientName].HasData(score.Owner.Identity, score.Identity))
                return new Data();
            double sqrWiegteBase = 0;
            List<Guid> respondents = Cache[this.Core.ClientName].GetRespondents(score.Owner.Identity, score.Identity);
            foreach (Guid idRespondent in respondents)
            //ProcessRespondents(score.Owner.Data.Cache[this.Core.ClientName].CategoryData[score.Identity], delegate (Guid idRespondent)
            {
                if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                    continue;

                double weight = 1.0;

                if (weightingFilters != null)
                {
                    weight = this.WeightMissingValue;
                    bool useDefault = true;

                    // Run through all weighting filters.
                    foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                    {
                        // Check if the weighting filter applies to the respondent.
                        if (weightingFilter.Respondents.ContainsKey(idRespondent))
                        {
                            weight = weightingFilter.Respondents[idRespondent];

                            useDefault = false;

                            break;
                        }
                    }

                    if (useDefault && weightingFilters.DefaultWeighting != null)
                    {
                        if (weightingFilters.Respondents.ContainsKey(idRespondent))
                            weight = weightingFilters.Respondents[idRespondent];
                    }
                }

                if (!result.Responses.ContainsKey(idRespondent))
                {
                    result.Responses.Add(idRespondent, new double[1]);

                    result.Base += weight;

                }

                //weight *= score.Owner.Data.Cache[this.Core.ClientName].CategoryData[score.Identity][idRespondent];
                sqrWiegteBase += weight * weight;
                result.Value += weight;
                result.Responses[idRespondent][0] += weight;
            }

            result.UnweightedValue = result.Responses.Count;

            if (sqrWiegteBase == 0)
            {
                result.EffectiveValue = result.Responses.Count;
            }
            else
            {
                result.EffectiveValue = (result.Base * result.Base) / sqrWiegteBase;
            }
            sqrWiegteBase = 0;

            return result;
        }

        /// <summary>
        /// Gets the respondents of a variable.
        /// </summary>
        /// <param name="idVariable">The id of the variable to get the respondents for.</param>
        public Data GetRespondents(
            Guid idVariable,
            bool isTaxonomy,
            CaseDataLocation caseDataLocation,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            Data result = new Data();

            if (!Cache[this.Core.ClientName].HasData(idVariable))
            {
                InitData(idVariable, isTaxonomy, caseDataLocation);
            }
            double sqrWiegteBase = 0;
            List<Guid> respondents = Cache[this.Core.ClientName].GetRespondents(idVariable);
            foreach (Guid idRespondent in respondents)
            //ProcessRespondents(Cache[this.Core.ClientName].VariableData[idVariable], delegate (Guid idRespondent)
            {
                if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                    continue;

                double weight = 1.0;

                if (weightingFilters != null)
                {
                    weight = this.WeightMissingValue;
                    bool useDefault = true;

                    // Run through all weighting filters.
                    foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                    {
                        // Check if the weighting filter applies to the respondent.
                        if (weightingFilter.Respondents.ContainsKey(idRespondent))
                        {
                            weight = weightingFilter.Respondents[idRespondent];

                            useDefault = false;

                            break;
                        }
                    }

                    if (useDefault && weightingFilters.DefaultWeighting != null)
                    {
                        if (weightingFilters.Respondents.ContainsKey(idRespondent))
                            weight = weightingFilters.Respondents[idRespondent];
                    }
                }

                if (!result.Responses.ContainsKey(idRespondent))
                {
                    result.Responses.Add(idRespondent, new double[1]);

                    result.Base += weight;

                }

                //weight *= Cache[this.Core.ClientName].VariableData[idVariable][idRespondent];
                sqrWiegteBase += weight * weight;
                result.Value += weight;
                result.Responses[idRespondent][0] += weight;
            }

            result.UnweightedValue = result.Responses.Count;

            if (sqrWiegteBase == 0)
            {
                result.EffectiveValue = result.Responses.Count;
            }
            else
            {
                result.EffectiveValue = (result.Base * result.Base) / sqrWiegteBase;
            }
            sqrWiegteBase = 0;

            return result;
        }

        public void ClearCaseDataCache()
        {
            lock (Cache)
            {
                Cache[this.Core.ClientName].Clear();
            }
        }

        public void InitData(
            Guid idVariable,
            bool isTaxonomy,
            CaseDataLocation caseDataLocation
        )
        {
            lock (Cache)
            {
                if (Cache[this.Core.ClientName].AccessTimes.ContainsKey(idVariable))
                    Cache[this.Core.ClientName].AccessTimes[idVariable] = DateTime.Now;
                else
                    Cache[this.Core.ClientName].AccessTimes.Add(idVariable, DateTime.Now);

                if (Cache[this.Core.ClientName].HasData(idVariable))
                    return;
            }

            long size = 0;

            Dictionary<Guid, object[]> variableData = new Dictionary<Guid, object[]>();
            Dictionary<Guid, Dictionary<Guid, int>> categoryData = new Dictionary<Guid, Dictionary<Guid, int>>();

            if (!isTaxonomy)
            {
                List<object[]> values;

                string commandText = string.Format(
                    "SELECT [IdRespondent], [IdCategory] FROM [resp].[Var_{0}] WHERE IdCategory IN ({1})",
                    idVariable,
                    string.Join(",", this.Core.Categories.GetValues(
                        new string[] { "Id" },
                        new string[] { "IdVariable", "ExcludeBase" },
                        new object[] { idVariable, false }
                    ).Select(x => "'" + x[0] + "'")
                ));

                if (!commandText.EndsWith("WHERE IdCategory IN ()"))
                {
                    if (caseDataLocation == CaseDataLocation.Sql)
                    {
                        values = this.Core.Respondents.ExecuteReader(commandText, typeof(Guid));
                    }
                    else
                    {
                        CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                        // Konstantin's database system.
                        values = dataLink.Select2(
                            commandText,
                            this.Core.ClientName
                        );
                    }
                }
                else
                {
                    values = new List<object[]>();
                }

                foreach (object[] value in values)
                {
                    if (!variableData.ContainsKey((Guid)value[0]))
                        variableData.Add((Guid)value[0], new object[0]);

                    //variableData[(Guid)value[0]]++;

                    if (!categoryData.ContainsKey((Guid)value[1]))
                    {
                        categoryData.Add((Guid)value[1], new Dictionary<Guid, int>());
                    }

                    if (!categoryData[(Guid)value[1]].ContainsKey((Guid)value[0]))
                        categoryData[(Guid)value[1]].Add((Guid)value[0], 0);

                    categoryData[(Guid)value[1]][(Guid)value[0]]++;
                }

                size += values.Count * 2;
            }
            else
            {
                List<object[]> linkedVariables;

                linkedVariables = this.Core.CategoryLinks.ExecuteReader(
                    "SELECT DISTINCT IdVariable, QA FROM [CategoryLinks] WHERE [IdTaxonomyVariable]={0}",
                    new object[] { idVariable }
                );

                Dictionary<Guid, List<string>> variables = new Dictionary<Guid, List<string>>();

                Dictionary<Guid, Guid> categoryMapping = new Dictionary<Guid, Guid>();

                List<object[]> categories = this.Core.CategoryLinks.GetValues(
                    new string[] { "IdCategory", "IdTaxonomyCategory" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { idVariable }
                );

                foreach (object[] category in categories)
                {
                    if (categoryMapping.ContainsKey((Guid)category[0]))
                        continue;

                    categoryMapping.Add((Guid)category[0], (Guid)category[1]);
                }

                List<object[]> values = new List<object[]>();

                foreach (object[] link in linkedVariables)
                {
                    /*if (!this.Owner.Definition.HierarchyFilter.Variables.ContainsKey((Guid)link[0]))
                        continue;*/

                    string commandText = string.Format(
                        "SELECT [IdRespondent], [IdCategory] FROM [resp].[Var_{0}]",
                        link[0]
                    );

                    if (caseDataLocation == CaseDataLocation.Sql)
                    {
                        values.AddRange(this.Core.Respondents.ExecuteReader(commandText, typeof(Guid)));
                    }
                    else
                    {
                        CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                        values.AddRange(dataLink.Select2(
                            commandText,
                            this.Core.ClientName
                        ));
                    }
                }

                foreach (object[] value in values)
                {
                    if (value[1] == null)
                        continue;

                    if (!categoryMapping.ContainsKey((Guid)value[1]))
                        continue;

                    if (!variableData.ContainsKey((Guid)value[0]))
                        variableData.Add((Guid)value[0], new object[0]);

                    //variableData[(Guid)value[0]]++;

                    Guid idTaxonomyCategory = categoryMapping[(Guid)value[1]];

                    if (!categoryData.ContainsKey(idTaxonomyCategory))
                    {
                        categoryData.Add(idTaxonomyCategory, new Dictionary<Guid, int>());
                    }

                    if (!categoryData[idTaxonomyCategory].ContainsKey((Guid)value[0]))
                        categoryData[idTaxonomyCategory].Add((Guid)value[0], 0);

                    categoryData[idTaxonomyCategory][(Guid)value[0]]++;
                }

                size += values.Count * 2;
            }

            lock (Cache)
            {
                Cache[this.Core.ClientName].Add(idVariable, variableData);

                Cache[this.Core.ClientName].Add(idVariable, categoryData);

                Cache[this.Core.ClientName].Size += size;

                if (Cache[this.Core.ClientName].AccessTimes.ContainsKey(idVariable))
                    Cache[this.Core.ClientName].AccessTimes[idVariable] = DateTime.Now;
                else
                    Cache[this.Core.ClientName].AccessTimes.Add(idVariable, DateTime.Now);
            }
        }

        public void InitDataNumeric(
            Guid idVariable,
            bool isTaxonomy,
            CaseDataLocation caseDataLocation
        )
        {
            lock (Cache)
            {
                if (Cache[this.Core.ClientName].AccessTimes.ContainsKey(idVariable))
                    Cache[this.Core.ClientName].AccessTimes[idVariable] = DateTime.Now;
                else
                    Cache[this.Core.ClientName].AccessTimes.Add(idVariable, DateTime.Now);

                if (Cache[this.Core.ClientName].HasData(idVariable))
                    return;
            }

            Dictionary<Guid, object[]> variableData = new Dictionary<Guid, object[]>();
            Dictionary<Guid, Dictionary<Guid, int>> categoryData = new Dictionary<Guid, Dictionary<Guid, int>>();

            long size = 0;

            if (!isTaxonomy)
            {
                List<object[]> values;

                string commandText = string.Format(
                    "SELECT [IdRespondent], [NumericAnswer] FROM [resp].[Var_{0}]",
                    idVariable
                );

                if (caseDataLocation == CaseDataLocation.Sql)
                {
                    values = this.Core.Respondents.ExecuteReader(commandText, typeof(Guid));
                }
                else
                {
                    CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                    // Konstantin's database system.
                    values = dataLink.Select2(
                        commandText,
                        this.Core.ClientName
                    );
                }

                foreach (object[] value in values)
                {
                    if (!variableData.ContainsKey((Guid)value[0]))
                        variableData.Add((Guid)value[0], new object[2]);

                    //variableData[(Guid)value[0]]++;
                    variableData[(Guid)value[0]][0] = value[1];

                    if (value.Length > 2)
                        variableData[(Guid)value[0]][1] = value[2];
                }

                size += values.Count * 2;
            }
            else
            {
                List<object[]> linkedVariables;

                linkedVariables = this.Core.CategoryLinks.ExecuteReader(
                    "SELECT DISTINCT IdVariable, QA FROM [VariableLinks] WHERE [IdTaxonomyVariable]={0}",
                    new object[] { idVariable }
                );

                Dictionary<Guid, List<string>> variables = new Dictionary<Guid, List<string>>();


                List<object[]> values = new List<object[]>();

                foreach (object[] link in linkedVariables)
                {
                    string commandText = string.Format(
                        "SELECT [IdRespondent], [NumericAnswer] FROM [resp].[Var_{0}]",
                        link[0]
                    );

                    if (caseDataLocation == CaseDataLocation.Sql)
                    {
                        values.AddRange(this.Core.Respondents.ExecuteReader(commandText, typeof(Guid)));
                    }
                    else
                    {
                        CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                        values.AddRange(dataLink.Select2(
                            commandText,
                            this.Core.ClientName
                        ));
                    }
                }

                foreach (object[] value in values)
                {
                    if (!variableData.ContainsKey((Guid)value[0]))
                        variableData.Add((Guid)value[0], new object[2]);

                    variableData[(Guid)value[0]][0] = value[1];
                }
                size += values.Count;
            }

            lock (Cache)
            {
                Cache[this.Core.ClientName].Add(idVariable, variableData);

                if (Cache[this.Core.ClientName].AccessTimes.ContainsKey(idVariable))
                    Cache[this.Core.ClientName].AccessTimes[idVariable] = DateTime.Now;
                else
                    Cache[this.Core.ClientName].AccessTimes.Add(idVariable, DateTime.Now);
            }
        }

        public void InitDataText(
            Guid idVariable,
            bool isTaxonomy,
            CaseDataLocation caseDataLocation
        )

        {
            lock (Cache)
            {
                if (Cache[this.Core.ClientName].AccessTimes.ContainsKey(idVariable))
                    Cache[this.Core.ClientName].AccessTimes[idVariable] = DateTime.Now;
                else
                    Cache[this.Core.ClientName].AccessTimes.Add(idVariable, DateTime.Now);

                if (Cache[this.Core.ClientName].HasData(idVariable))
                    return;
            }

            Dictionary<Guid, object[]> variableData = new Dictionary<Guid, object[]>();
            Dictionary<Guid, Dictionary<Guid, int>> categoryData = new Dictionary<Guid, Dictionary<Guid, int>>();

            long size = 0;

            if (!isTaxonomy)
            {
                List<object[]> values = new List<object[]>();

                string commandText = string.Format(
                    "SELECT [IdRespondent], [TextAnswer] FROM [resp].[Var_{0}]",
                    idVariable
                );

                if (caseDataLocation == CaseDataLocation.Sql)
                {
                    values = this.Core.Respondents.ExecuteReader(commandText, typeof(Guid));
                }
                else
                {
                    CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                    // Konstantin's database system.
                    values = dataLink.Select2(
                        commandText,
                        this.Core.ClientName
                    );
                }

                foreach (object[] value in values)
                {
                    if (!variableData.ContainsKey((Guid)value[0]))
                        variableData.Add((Guid)value[0], new object[2]);

                    //variableData[(Guid)value[0]]++;
                    //variableData[(Guid)value[0]][0] = value[1];
                    variableData[(Guid)value[0]][1] = value[1];
                }

                size += values.Count * 2;
            }
            else
            {
                List<object[]> linkedVariables;

                linkedVariables = this.Core.CategoryLinks.ExecuteReader(
                    "SELECT DISTINCT IdVariable, QA FROM [VariableLinks] WHERE [IdTaxonomyVariable]={0}",
                    new object[] { idVariable }
                );

                Dictionary<Guid, List<string>> variables = new Dictionary<Guid, List<string>>();


                List<object[]> values = new List<object[]>();

                foreach (object[] link in linkedVariables)
                {
                    string commandText = string.Format(
                        "SELECT [IdRespondent], [TextAnswer] FROM [resp].[Var_{0}]",
                        link[0]
                    );

                    if (caseDataLocation == CaseDataLocation.Sql)
                    {
                        values.AddRange(this.Core.Respondents.ExecuteReader(commandText, typeof(Guid)));

                    }
                    else
                    {
                        CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                        values.AddRange(dataLink.Select2(
                            commandText,
                            this.Core.ClientName
                        ));
                    }
                }

                foreach (object[] value in values)
                {
                    if (!variableData.ContainsKey((Guid)value[0]))
                        variableData.Add((Guid)value[0], new object[2]);

                    variableData[(Guid)value[0]][1] = value[1];
                }

                size += values.Count;
            }

            lock (Cache)
            {
                Cache[this.Core.ClientName].Add(idVariable, variableData);

                if (Cache[this.Core.ClientName].AccessTimes.ContainsKey(idVariable))
                    Cache[this.Core.ClientName].AccessTimes[idVariable] = DateTime.Now;
                else
                    Cache[this.Core.ClientName].AccessTimes.Add(idVariable, DateTime.Now);
            }
        }

        /// <summary>
        /// Gets the respondents of a variable.
        /// </summary>
        /// <param name="idVariable">The id of the variable to get the respondents for.</param>
        public Data GetRespondents(
            LinkBiDefinitionDimension variable,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            Data result = new Data();

            if (variable.VariableType == VariableType.Numeric)
            {
                InitDataNumeric(
                    variable.Identity,
                    variable.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }
            else if (variable.VariableType == VariableType.Text)
            {
                InitDataText(
                    variable.Identity,
                    variable.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }
            else
            {
                InitData(
                    variable.Identity,
                    variable.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }
            double sqrWiegteBase = 0;
            double categoryEffectivebase = 0;
            //List<object[]> values = this.Core.Respondents.ExecuteReader(commandText, typeof(Guid));
            //foreach (Guid idCategory in variable.Data.CategoryData.Keys)
            List<Guid> categories = Cache[this.Core.ClientName].GetCategories(variable.Identity);
            //foreach (Guid idCategory in Cache[this.Core.ClientName].CategoryData[variable.Identity].Keys)
            foreach (Guid idCategory in categories)
            {
                List<Guid> respondents = Cache[this.Core.ClientName].GetRespondents(variable.Identity, idCategory);
                //foreach (Guid idRespondent in Cache[this.Core.ClientName].CategoryData[variable.Identity][idCategory].Keys)
                foreach (Guid idRespondent in respondents)
                //ProcessRespondents(variable.Data.Cache[this.Core.ClientName].CategoryData[idCategory], delegate (Guid idRespondent)
                {
                    if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                        continue;

                    double weight = 1.0;

                    if (weightingFilters != null)
                    {
                        weight = this.WeightMissingValue;
                        bool useDefault = true;

                        // Run through all weighting filters.
                        foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                        {
                            // Check if the weighting filter applies to the respondent.
                            if (weightingFilter.Respondents.ContainsKey(idRespondent))
                            {
                                weight = weightingFilter.Respondents[idRespondent];

                                useDefault = false;

                                break;
                            }
                        }

                        if (useDefault && weightingFilters.DefaultWeighting != null)
                        {
                            if (weightingFilters.Respondents.ContainsKey(idRespondent))
                                weight = weightingFilters.Respondents[idRespondent];
                        }
                    }

                    if (!result.Responses.ContainsKey(idRespondent))
                    {
                        result.Responses.Add(idRespondent, new double[1]);

                        result.Base += weight;
                        sqrWiegteBase += Math.Pow(weight, 2);
                        categoryEffectivebase += weight;
                    }

                    //weight *= variable.Data.Cache[this.Core.ClientName].CategoryData[idCategory][idRespondent];
                    sqrWiegteBase += weight * weight;
                    result.Value += weight;
                    result.Responses[idRespondent][0] += weight;
                }

                result.EffectiveValue += Math.Pow(categoryEffectivebase, 2) / sqrWiegteBase;
                sqrWiegteBase = 0;
                categoryEffectivebase = 0;
            }

            result.UnweightedValue = result.Responses.Count;
            if (result.EffectiveValue == 0)
            {
                result.EffectiveValue = result.Responses.Count;
            }
            return result;
        }

        /// <summary>
        /// Gets the respondents of a numeric variable.
        /// </summary>
        /// <param name="idVariable">The id of the numeric variable to get the respondents for.</param>
        public NumericData GetRespondentsNumeric(
            ReportDefinitionVariable variable,
            CaseDataLocation caseDataLocation,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null
        )
        {
            NumericData result;

            if (filter != null && filter.Responses.Count == 0)
            {
                result = new NumericData();
                result.MinValue = 0;
                return result;
            }

            result = new NumericData();

            if (variable.VariableType == VariableType.Numeric)
            {
                InitDataNumeric(
                    variable.IdVariable,
                    variable.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }
            else if (variable.VariableType == VariableType.Text)
            {
                InitDataText(
                    variable.IdVariable,
                    variable.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }
            else
            {
                InitData(
                    variable.IdVariable,
                    variable.IsTaxonomy,
                    this.Core.CaseDataLocation
                );
            }

            //foreach (Guid idRespondent in variable.Data.Respondents.Keys)
            //List<Guid> respondents = Cache[this.Core.ClientName].GetRespondents(variable.IdVariable, this.AggregateNonQAData);
            Dictionary<Guid, object[]> responses = Cache[this.Core.ClientName].GetResponses(
                variable.IdVariable
            );
            double sqrWiegteBase = 0;
            foreach (Guid idRespondent in responses.Keys)
            {
                if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                    continue;

                double weight = 1.0;

                if (weightingFilters != null)
                {
                    weight = this.WeightMissingValue;
                    bool useDefault = true;

                    // Run through all weighting filters.
                    foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                    {
                        // Check if the weighting filter applies to the respondent.
                        if (weightingFilter.Respondents.ContainsKey(idRespondent))
                        {
                            weight = weightingFilter.Respondents[idRespondent];

                            useDefault = false;

                            break;
                        }
                    }

                    if (useDefault && weightingFilters.DefaultWeighting != null)
                    {
                        if (weightingFilters.Respondents.ContainsKey(idRespondent))
                            weight = weightingFilters.Respondents[idRespondent];
                    }
                }

                if (!result.Responses.ContainsKey(idRespondent))
                {
                    result.Responses.Add(idRespondent, new double[2]);

                    result.Base += weight;

                }

                double value = 0.0;
                //object _value = v[1];
                //object _value = variable.Data.Respondents[idRespondent].Value;
                //object _value = Cache[this.Core.ClientName].VariableData[variable.IdVariable][idRespondent][0];
                object _value = responses[idRespondent][0];

                if (_value != DBNull.Value && _value != null)
                    value = (double)_value;

                result.Value += weight;
                sqrWiegteBase += weight * weight;
                result.MeanValue += weight * value;
                result.UMeanValue += value;

                if (result.MaxValue < value)
                    result.MaxValue = value;

                if (result.MinValue > value)
                    result.MinValue = value;

                result.Responses[idRespondent][0] += value;
                result.Responses[idRespondent][1] = weight;
            }

            double test = Math.Pow(result.MeanValue, 2);

            if (result.Base != 0)
                test /= result.Base;

            result.MeanValue /= result.Base;
            result.UMeanValue /= result.Responses.Count;

            double variance = 0.0;
            double uVariance = 0.0;

            // Now calculate the variance
            // Run through all respondents of the result.
            foreach (Guid idRespondent in result.Responses.Keys)
            {
                //variance += Math.Pow((result.Responses[idRespondent][0] * result.Responses[idRespondent][1]) - result.MeanValue, 2);
                //variance += Math.Pow((result.Responses[idRespondent][0]) - result.MeanValue, 2);
                variance += (result.Responses[idRespondent][1] * Math.Pow(result.Responses[idRespondent][0], 2));
                uVariance += Math.Pow((result.Responses[idRespondent][0]) - result.UMeanValue, 2);
            }

            variance -= test;

            if (result.Base != 0)
                variance /= result.Base - 1;

            if (result.Responses.Count != 0)
                uVariance /= result.Responses.Count;

            result.StdDev = Math.Sqrt(variance);
            result.UStdDev = Math.Sqrt(uVariance);

            if (result.MinValue == double.MaxValue)
                result.MinValue = 0;

            if (double.IsNaN(result.MeanValue))
                result.MeanValue = 0;

            if (double.IsNaN(result.UMeanValue))
                result.UMeanValue = 0;

            if (double.IsNaN(result.StdDev))
                result.StdDev = 0;

            if (double.IsNaN(result.UStdDev))
                result.UStdDev = 0;

            result.UnweightedValue = result.Responses.Count;
            if (sqrWiegteBase == 0)
            {
                result.EffectiveValue = result.Responses.Count;
            }
            else
            {
                result.EffectiveValue = (result.Base * result.Base) / sqrWiegteBase;
            }
            sqrWiegteBase = 0;

            return result;
        }

        /// <summary>
        /// Gets the respondents of a numeric variable.
        /// </summary>
        /// <param name="idVariable">The id of the numeric variable to get the respondents for.</param>
        public Data GetRespondentsNumeric(
            Guid idVariable,
            bool isTaxonomy,
            CaseDataLocation caseDataLocation,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            Data result = new Data();

            if (!Cache[this.Core.ClientName].HasData(idVariable))
                InitDataNumeric(idVariable, isTaxonomy, caseDataLocation);
            double sqrWiegteBase = 0;
            //List<Guid> respondents = Cache[this.Core.ClientName].GetRespondents(idVariable, this.AggregateNonQAData);
            Dictionary<Guid, object[]> responses = Cache[this.Core.ClientName].GetResponses(idVariable);
            foreach (Guid idRespondent in responses.Keys)
            {
                if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                    continue;

                double weight = 1.0;

                if (weightingFilters != null)
                {
                    weight = this.WeightMissingValue;
                    bool useDefault = true;

                    // Run through all weighting filters.
                    foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                    {
                        // Check if the weighting filter applies to the respondent.
                        if (weightingFilter.Respondents.ContainsKey(idRespondent))
                        {
                            weight = weightingFilter.Respondents[idRespondent];

                            useDefault = false;

                            break;
                        }
                    }

                    if (useDefault && weightingFilters.DefaultWeighting != null)
                    {
                        if (weightingFilters.Respondents.ContainsKey(idRespondent))
                            weight = weightingFilters.Respondents[idRespondent];
                    }
                }

                if (!result.Responses.ContainsKey(idRespondent))
                {
                    result.Responses.Add(idRespondent, new double[2]);

                    result.Base += weight;

                }

                double value = 0.0;
                //object _value = v[1];
                //object _value = Cache[this.Core.ClientName].VariableData[idVariable][idRespondent][0];
                object _value = responses[idRespondent][0];

                /*if (_value != DBNull.Value)
                    value = double.Parse(_value.ToString());*/
                if (_value != DBNull.Value && _value != null)
                    value = (double)_value;

                result.Responses[idRespondent][0] += value;
                result.Responses[idRespondent][1] += weight;
                sqrWiegteBase += weight * weight;
                result.Value += value * weight;
            }

            result.UnweightedValue = result.Responses.Count;
            if (sqrWiegteBase == 0)
            {
                result.EffectiveValue = result.Responses.Count;
            }
            else
            {
                result.EffectiveValue = (result.Base * result.Base) / sqrWiegteBase;
            }
            sqrWiegteBase = 0;

            return result;
        }
        /*public Data GetRespondentsNumeric(
            Guid idVariable,
            bool isTaxonomy,
            CaseDataLocation caseDataLocation,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            List<string> commandTexts = new List<string>();

            if (!isTaxonomy)
            {
                commandTexts.Add("SELECT [IdRespondent], [NumericAnswer] FROM [resp].[Var_" + idVariable + "]");
            }
            else
            {
                List<object[]> variableLinks;

                if (this.AggregateNonQAData)
                {
                    variableLinks = this.Core.VariableLinks.GetValues(
                        new string[] { "IdVariable" },
                        new string[] { "IdTaxonomyVariable" },
                        new object[] { idVariable }
                    );
                }
                else
                {
                    variableLinks = this.Core.VariableLinks.GetValues(
                        new string[] { "IdVariable" },
                        new string[] { "IdTaxonomyVariable", "QA" },
                        new object[] { idVariable, 1 }
                    );
                }

                // Run through all linked variable ids.
                foreach (object[] variableLink in variableLinks)
                {
                    commandTexts.Add("SELECT [IdRespondent], [NumericAnswer] FROM [resp].[Var_" + (Guid)variableLink[0] + "]");
                }
            }

            Data result = new Data();

            foreach (string commandText in commandTexts)
            {
                List<object[]> values;

                if (caseDataLocation == CaseDataLocation.File)
                {
                    CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();

                    values = dataLink.Select2(
                        commandText,
                        this.Core.ClientName
                    );
                }
                else
                {
                    values = this.Core.Respondents.ExecuteReader(commandText, typeof(Guid), typeof(double));
                }

                foreach (object[] v in values)
                {
                    Guid idRespondent = (Guid)v[0];

                    if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                        continue;

                    double weight = 1.0;

                    if (weightingFilters != null)
                    {
                        bool useDefault = true;

                        // Run through all weighting filters.
                        foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                        {
                            // Check if the weighting filter applies to the respondent.
                            if (weightingFilter.Respondents.ContainsKey(idRespondent))
                            {
                                weight = weightingFilter.Respondents[idRespondent];

                                useDefault = false;

                                break;
                            }
                        }

                        if (useDefault && weightingFilters.DefaultWeighting != null)
                        {
                            if (weightingFilters.Respondents.ContainsKey(idRespondent))
                                weight = weightingFilters.Respondents[idRespondent];
                        }
                    }

                    if (!result.Responses.ContainsKey(idRespondent))
                    {
                        result.Responses.Add(idRespondent, new double[2]);

                        result.Base += weight;
                    }

                    double value = 0.0;
                    object _value = v[1];
                    
                    if (_value != DBNull.Value && _value != null)
                        value = (double)_value;

                    result.Responses[idRespondent][0] += value;
                    result.Responses[idRespondent][1] += weight;

                    result.Value += value * weight;
                }
            }

            result.UnweightedValue = result.Responses.Count;

            return result;
        }*/


        /// <summary>
        /// Gets the respondents of a text category.
        /// </summary>
        /// <param name="idCategory">The id of the category to get the respondents for.</param>
        public Data GetRespondentsText(
            ReportDefinitionScore score,
            Guid idVariable,
            bool isTaxonomy,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            Data result = new Data();

            List<string> categories = new List<string>();

            if (score.XmlNode.Attributes["Text"] != null)
            {
                categories.Add(score.XmlNode.Attributes["Text"].Value);
            }
            else
            {
                if (score is ReportDefinitionScoreGroup)
                {
                    // Run through all scores of the score group.
                    foreach (ReportDefinitionScore _score in (score as ReportDefinitionScoreGroup).Scores)
                    {
                        if (_score.XmlNode.Attributes["Text"] != null)
                            categories.Add(_score.XmlNode.Attributes["Text"].Value);
                    }
                }
                else
                {
                    return new Data();
                }
            }

            List<KeyValuePair<Guid, string>> commandTexts = new List<KeyValuePair<Guid, string>>();

            foreach (string category in categories)
            {
                if (!isTaxonomy)
                {
                    commandTexts.Add(new KeyValuePair<Guid, string>(
                        idVariable,
                        category
                    ));
                }
                else
                {
                    List<object[]> variableLinks;

                    variableLinks = this.Core.VariableLinks.GetValues(
                        new string[] { "IdVariable" },
                        new string[] { "IdTaxonomyVariable" },
                        new object[] { idVariable }
                    );

                    // Run through all linked variable ids.
                    foreach (object[] variableLink in variableLinks)
                    {
                        /*commandTexts.Add(string.Format(
                            "SELECT [IdRespondent] FROM [resp].[Var_{0}] WHERE [TextAnswer] like '%{1}%'",
                            variableLink[0],
                            category
                        ));*/

                        commandTexts.Add(new KeyValuePair<Guid, string>(
                            (Guid)variableLink[0],
                            category
                        ));
                    }
                }
            }
            double sqrWiegteBase = 0;
            foreach (KeyValuePair<Guid, string> commandText in commandTexts)
            {
                List<object[]> values = new List<object[]>();
                try
                {
                    if (commandText.Value.Trim() == "")
                    {
                        values = this.Core.Responses[commandText.Key].GetValues(
                            new string[] { "IdRespondent" },
                            new string[] { "TextAnswer" },
                            new object[] { commandText.Value }
                        );
                    }
                    else
                    {
                        values = this.Core.Responses[commandText.Key].GetValuesLikeWithoutSign(
                            new string[] { "IdRespondent" },
                            new string[] { "TextAnswer" },
                            new object[] { commandText.Value }
                        );
                    }
                }
                catch { }

                foreach (object[] value in values)
                {
                    Guid idRespondent = (Guid)value[0];

                    if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                        continue;

                    double weight = 1.0;

                    if (weightingFilters != null)
                    {
                        weight = this.WeightMissingValue;
                        bool useDefault = true;

                        // Run through all weighting filters.
                        foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                        {
                            // Check if the weighting filter applies to the respondent.
                            if (weightingFilter.Respondents.ContainsKey(idRespondent))
                            {
                                weight = weightingFilter.Respondents[idRespondent];

                                useDefault = false;

                                break;
                            }
                        }

                        if (useDefault && weightingFilters.DefaultWeighting != null)
                        {
                            if (weightingFilters.Respondents.ContainsKey(idRespondent))
                                weight = weightingFilters.Respondents[idRespondent];
                        }
                    }

                    if (!result.Responses.ContainsKey(idRespondent))
                    {
                        result.Responses.Add(idRespondent, new double[1]);

                        result.Base += weight;

                    }
                    sqrWiegteBase += weight * weight;
                    result.Value += weight;
                    result.Responses[idRespondent][0] += weight;
                }
            }

            result.UnweightedValue = result.Responses.Count;
            if (sqrWiegteBase == 0)
            {
                result.EffectiveValue = result.Responses.Count;
            }
            else
            {
                result.EffectiveValue = (result.Base * result.Base) / sqrWiegteBase;
            }
            sqrWiegteBase = 0;


            return result;
        }

        /// <summary>
        /// Gets the respondents of a text category.
        /// </summary>
        /// <param name="idCategory">The id of the category to get the respondents for.</param>
        public Data GetRespondentsText(
            string category,
            Guid idVariable,
            bool isTaxonomy,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null,
            bool like = true
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            Data result = new Data();

            List<KeyValuePair<Guid, string>> commandTexts = new List<KeyValuePair<Guid, string>>();

            if (!isTaxonomy)
            {
                commandTexts.Add(new KeyValuePair<Guid, string>(
                    idVariable,
                    category
                ));
            }
            else
            {
                List<object[]> variableLinks;

                variableLinks = this.Core.VariableLinks.GetValues(
                    new string[] { "IdVariable" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { idVariable }
                );

                // Run through all linked variable ids.
                foreach (object[] variableLink in variableLinks)
                {
                    /*commandTexts.Add(string.Format(
                        "SELECT [IdRespondent] FROM [resp].[Var_{0}] WHERE [TextAnswer] like '%{1}%'",
                        variableLink[0],
                        category
                    ));*/

                    commandTexts.Add(new KeyValuePair<Guid, string>(
                        (Guid)variableLink[0],
                        category
                    ));
                }
            }
            double sqrWiegteBase = 0;
            foreach (KeyValuePair<Guid, string> commandText in commandTexts)
            {
                List<object[]> values = new List<object[]>();
                try
                {
                    //values = this.Core.Respondents.ExecuteReader(commandText, typeof(Guid));
                    if (like && commandText.Value.Trim() != "")
                    {
                        values = this.Core.Responses[commandText.Key].GetValuesLike(
                            new string[] { "IdRespondent" },
                            new string[] { "TextAnswer" },
                            new object[] { commandText.Value }
                        );
                    }
                    else
                    {
                        values = this.Core.Responses[commandText.Key].GetValues(
                            new string[] { "IdRespondent" },
                            new string[] { "TextAnswer" },
                            new object[] { commandText.Value }
                        );
                    }
                }
                catch { }

                foreach (object[] value in values)
                {
                    Guid idRespondent = (Guid)value[0];

                    if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                        continue;

                    double weight = 1.0;

                    if (weightingFilters != null)
                    {
                        weight = this.WeightMissingValue;
                        bool useDefault = true;

                        // Run through all weighting filters.
                        foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                        {
                            // Check if the weighting filter applies to the respondent.
                            if (weightingFilter.Respondents.ContainsKey(idRespondent))
                            {
                                weight = weightingFilter.Respondents[idRespondent];

                                useDefault = false;

                                break;
                            }
                        }

                        if (useDefault && weightingFilters.DefaultWeighting != null)
                        {
                            if (weightingFilters.Respondents.ContainsKey(idRespondent))
                                weight = weightingFilters.Respondents[idRespondent];
                        }
                    }

                    if (!result.Responses.ContainsKey(idRespondent))
                    {
                        result.Responses.Add(idRespondent, new double[1]);

                        result.Base += weight;

                    }
                    sqrWiegteBase += weight * weight;
                    result.Value += weight;
                    result.Responses[idRespondent][0] += weight;
                }
            }

            result.UnweightedValue = result.Responses.Count;
            if (sqrWiegteBase == 0)
            {
                result.EffectiveValue = result.Responses.Count;
            }
            else
            {
                result.EffectiveValue = (result.Base * result.Base) / sqrWiegteBase;
            }
            sqrWiegteBase = 0;
            return result;
        }

        /// <summary>
        /// Gets the respondents of a text category.
        /// </summary>
        /// <param name="idCategory">The id of the category to get the respondents for.</param>
        public Data GetRespondentsText(
            string category,
            Guid idVariable,
            bool isTaxonomy,
            Guid[] idVariables,
            Data filter = null,
            WeightingFilterCollection weightingFilters = null,
            bool like = true
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new Data();

            Data result = new Data();

            List<KeyValuePair<Guid, string>> commandTexts = new List<KeyValuePair<Guid, string>>();

            if (!isTaxonomy)
            {
                commandTexts.Add(new KeyValuePair<Guid, string>(
                    idVariable,
                    category
                ));
            }
            else
            {
                List<object[]> variableLinks;

                /*if (this.AggregateNonQAData)
                {
                    variableLinks = this.Core.VariableLinks.GetValues(
                        new string[] { "IdVariable" },
                        new string[] { "IdTaxonomyVariable" },
                        new object[] { idVariable }
                    );
                }
                else
                {
                    variableLinks = this.Core.VariableLinks.GetValues(
                        new string[] { "IdVariable" },
                        new string[] { "IdTaxonomyVariable", "QA" },
                        new object[] { idVariable, 1 }
                    );
                }*/

                // Run through all linked variable ids.
                foreach (Guid variableLink in idVariables)
                {
                    /*commandTexts.Add(string.Format(
                        "SELECT [IdRespondent] FROM [resp].[Var_{0}] WHERE [TextAnswer] like '%{1}%'",
                        variableLink[0],
                        category
                    ));*/

                    commandTexts.Add(new KeyValuePair<Guid, string>(
                        variableLink,
                        category
                    ));
                }
            }
            double sqrWiegteBase = 0;
            foreach (KeyValuePair<Guid, string> commandText in commandTexts)
            {
                List<object[]> values = new List<object[]>();
                try
                {
                    //values = this.Core.Respondents.ExecuteReader(commandText, typeof(Guid));
                    if (like && commandText.Value.Trim() != "")
                    {
                        values = this.Core.Responses[commandText.Key].GetValuesLike(
                            new string[] { "IdRespondent" },
                            new string[] { "TextAnswer" },
                            new object[] { commandText.Value }
                        );
                    }
                    else
                    {
                        values = this.Core.Responses[commandText.Key].GetValues(
                            new string[] { "IdRespondent" },
                            new string[] { "TextAnswer" },
                            new object[] { commandText.Value }
                        );
                    }
                }
                catch { }

                foreach (object[] value in values)
                {
                    Guid idRespondent = (Guid)value[0];

                    if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                        continue;

                    double weight = 1.0;

                    if (weightingFilters != null)
                    {
                        weight = this.WeightMissingValue;
                        bool useDefault = true;

                        // Run through all weighting filters.
                        foreach (WeightingFilter weightingFilter in weightingFilters.ToArray())
                        {
                            // Check if the weighting filter applies to the respondent.
                            if (weightingFilter.Respondents.ContainsKey(idRespondent))
                            {
                                weight = weightingFilter.Respondents[idRespondent];

                                useDefault = false;

                                break;
                            }
                        }

                        if (useDefault && weightingFilters.DefaultWeighting != null)
                        {
                            if (weightingFilters.Respondents.ContainsKey(idRespondent))
                                weight = weightingFilters.Respondents[idRespondent];
                        }
                    }

                    if (!result.Responses.ContainsKey(idRespondent))
                    {
                        result.Responses.Add(idRespondent, new double[1]);

                        result.Base += weight;

                    }
                    sqrWiegteBase += weight * weight;
                    result.Value += weight;
                    result.Responses[idRespondent][0] += weight;
                }
            }

            result.UnweightedValue = result.Responses.Count;
            if (sqrWiegteBase == 0)
            {
                result.EffectiveValue = result.Responses.Count;
            }
            else
            {
                result.EffectiveValue = (result.Base * result.Base) / sqrWiegteBase;
            }
            sqrWiegteBase = 0;

            return result;
        }

        /// <summary>
        /// Gets the respondents of a text category.
        /// </summary>
        /// <param name="idCategory">The id of the category to get the respondents for.</param>
        public List<string> GetTextAnswers(
            Guid idVariable,
            bool isTaxonomy,
            Data filter = null,
            bool split = true
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new List<string>();

            Dictionary<string, int> result = new Dictionary<string, int>();

            List<string> commandTexts = new List<string>();

            if (!isTaxonomy)
            {
                commandTexts.Add(string.Format(
                    "SELECT [IdRespondent], [TextAnswer] FROM [resp].[Var_{0}]",
                    idVariable
                ));
            }
            else
            {
                List<object[]> variableLinks;

                variableLinks = this.Core.VariableLinks.GetValues(
                    new string[] { "IdVariable" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { idVariable }
                );

                // Run through all linked variable ids.
                foreach (object[] variableLink in variableLinks)
                {
                    commandTexts.Add(string.Format(
                        "SELECT [IdRespondent], [TextAnswer] FROM [resp].[Var_{0}]",
                        variableLink[0]
                    ));
                }
            }

            foreach (string commandText in commandTexts)
            {
                List<object[]> values;

                if (true/*this.Core.CaseDataLocation == CaseDataLocation.Sql*/)
                {
                    values = this.Core.Respondents.ExecuteReader(commandText, typeof(Guid));
                }
                else
                {
                    CaseDataCore1.CaseDataLink dataLink = new CaseDataCore1.CaseDataLink();
                    values = dataLink.Select2(commandText, this.Core.ClientName);
                }

                foreach (object[] value in values)
                {
                    Guid idRespondent = (Guid)value[0];

                    if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                        continue;

                    if (!split)
                    {
                        string word = (string)value[1];

                        if (word == null)
                            word = "";

                        if (!result.ContainsKey(word))
                            result.Add(word, 1);
                        else
                            result[word]++;
                    }
                    else
                    {
                        if (value[1] == null)
                            value[1] = "";

                        string[] words = ((string)value[1]).Split('_');

                        foreach (string word in words)
                        {
                            if (!result.ContainsKey(word))
                                result.Add(word, 1);
                            else
                                result[word]++;
                        }
                    }
                }
            }

            return result.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        }

        /// <summary>
        /// Gets the respondents of a text category.
        /// </summary>
        /// <param name="idCategory">The id of the category to get the respondents for.</param>
        public Dictionary<Guid, List<string>> LoadTextAnswers(
            Guid idVariable,
            bool isTaxonomy,
            Data filter = null,
            bool split = true
        )
        {
            if (filter != null && filter.Responses.Count == 0)
                return new Dictionary<Guid, List<string>>();

            Dictionary<Guid, List<string>> result = new Dictionary<Guid, List<string>>();

            List<string> commandTexts = new List<string>();

            if (!isTaxonomy)
            {
                commandTexts.Add(string.Format(
                    "SELECT [IdRespondent], [TextAnswer] FROM [resp].[Var_{0}]",
                    idVariable
                ));
            }
            else
            {
                List<object[]> variableLinks;

                variableLinks = this.Core.VariableLinks.GetValues(
                    new string[] { "IdVariable" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { idVariable }
                );

                // Run through all linked variable ids.
                foreach (object[] variableLink in variableLinks)
                {
                    commandTexts.Add(string.Format(
                        "SELECT [IdRespondent], [TextAnswer] FROM [resp].[Var_{0}]",
                        variableLink[0]
                    ));
                }
            }

            foreach (string commandText in commandTexts)
            {
                List<object[]> values = this.Core.Respondents.ExecuteReader(commandText, typeof(Guid));

                foreach (object[] value in values)
                {
                    Guid idRespondent = (Guid)value[0];

                    if (filter != null && filter.Responses.ContainsKey(idRespondent) == false)
                        continue;

                    if (!result.ContainsKey(idRespondent))
                        result.Add(idRespondent, new List<string>());

                    if (!split)
                    {
                        result[idRespondent].Add((string)value[1]);
                    }
                    else
                    {
                        string[] words = ((string)value[1]).Split('_');

                        foreach (string word in words)
                        {
                            result[idRespondent].Add(word);
                        }
                    }
                }
            }

            return result;
        }


        private void ProcessRespondents(
            Dictionary<Guid, int> respondents,
            ProcessRespondentMethod method
        )
        {
            foreach (Guid idRespondent in respondents.Keys)
            {
                method(idRespondent);
            }
        }

        #endregion
    }

    public class CaseDataCache
    {
        #region Properties

        public long Size { get; set; }

        private Dictionary<Guid, Dictionary<Guid, object[]>> VariableData { get; set; }

        private Dictionary<Guid, Dictionary<Guid, Dictionary<Guid, int>>> CategoryData { get; set; }

        public Dictionary<Guid, DateTime> AccessTimes { get; set; }

        public System.Timers.Timer Timer { get; set; }

        #endregion


        #region Constructor

        public CaseDataCache()
        {
            this.VariableData = new Dictionary<Guid, Dictionary<Guid, object[]>>();
            this.CategoryData = new Dictionary<Guid, Dictionary<Guid, Dictionary<Guid, int>>>();
            this.AccessTimes = new Dictionary<Guid, DateTime>();

            this.Timer = new System.Timers.Timer(10000);
            this.Timer.Elapsed += Timer_Elapsed;
            this.Timer.Start();
        }

        #endregion


        #region Methods

        public List<Guid> GetRespondents(Guid idVariable)
        {
            return this.VariableData[idVariable].Keys.ToList();
        }

        public List<Guid> GetRespondents(Guid idVariable, Guid idCategory)
        {
            if (this.CategoryData.ContainsKey(idVariable) == false ||
                this.CategoryData[idVariable].ContainsKey(idCategory) == false)
                return new List<Guid>();

            return this.CategoryData[idVariable][idCategory].Keys.ToList();
        }

        public List<Guid> GetCategories(Guid idVariable)
        {
            List<Guid> result = new List<Guid>();

            if (this.CategoryData.ContainsKey(idVariable))
                result = this.CategoryData[idVariable].Keys.ToList();

            return result;
        }

        public Dictionary<Guid, object[]> GetResponses(Guid idVariable)
        {
            if (!this.VariableData.ContainsKey(idVariable))
                return new Dictionary<Guid, object[]>();

            return this.VariableData[idVariable];
        }

        public Dictionary<Guid, int> GetResponses(
            Guid idVariable,
            Guid idCategory
        )
        {
            if (this.CategoryData.ContainsKey(idVariable) == false ||
                this.CategoryData[idVariable].ContainsKey(idCategory) == false)
                return new Dictionary<Guid, int>();

            return this.CategoryData[idVariable][idCategory];
        }

        public bool HasData(Guid idVariable)
        {
            return this.VariableData.ContainsKey(idVariable);
        }
        public bool HasData(Guid idVariable, Guid idCategory)
        {
            if (this.CategoryData.ContainsKey(idVariable) &&
                this.CategoryData[idVariable].ContainsKey(idCategory))
                return true;

            return false;
        }

        public void Add(
            Guid idVariable,
            Dictionary<Guid, object[]> variableData
        )
        {
            if (!this.VariableData.ContainsKey(idVariable))
                this.VariableData.Add(idVariable, variableData);
        }

        public void Add(
            Guid idVariable,
            Dictionary<Guid, Dictionary<Guid, int>> categoryData
        )
        {
            if (!this.CategoryData.ContainsKey(idVariable))
                this.CategoryData.Add(idVariable, categoryData);
        }

        public void Clear()
        {
            this.AccessTimes.Clear();
            this.VariableData.Clear();
            this.CategoryData.Clear();
        }

        public long GetSize()
        {
            long result = 0;

            foreach (Guid idVariable in VariableData.Keys)
            {
                result += VariableData[idVariable].Count * 16;

                if (!CategoryData.ContainsKey(idVariable))
                    continue;

                foreach (Guid idCategory in CategoryData[idVariable].Keys)
                {
                    result += CategoryData[idVariable][idCategory].Count * 16;
                }
            }

            return result;
        }

        #endregion


        #region Event Handlers

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (Guid idVariable in this.AccessTimes.Keys.ToArray())
            {
                if ((DateTime.Now - this.AccessTimes[idVariable]).TotalMinutes >= 15.0)
                {
                    this.VariableData.Remove(idVariable);
                    this.CategoryData.Remove(idVariable);
                    this.AccessTimes.Remove(idVariable);
                }
            }
        }

        #endregion
    }

    public delegate void ProcessRespondentMethod(Guid idRespondent);

    public static class EquationData
    {
        public static Dictionary<string, Dictionary<Guid, object>> Data;
    }
}
