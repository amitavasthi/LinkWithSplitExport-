using Crosstables.Classes.ReportDefinitionClasses.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DataCore.Classes.StorageMethods;
using DatabaseCore.Items;
using ApplicationUtilities.Classes;

namespace DataCore.Classes
{
    public class Equation
    {
        #region Properties

        public CaseDataLocation CaseDataLocation { get; set; }

        public EquationAssembly Assembly { get; set; }

        public DatabaseCore.Core Core { get; set; }

        /// <summary>
        /// Gets or sets the equation string.
        /// </summary>
        public string EquationString { get; set; }

        /// <summary>
        /// Gets or sets a dictionary which contains the
        /// values for the equation's place holders.
        /// </summary>
        public Dictionary<string, EquationPlaceHolder> Values { get; set; }

        public int WeightMissingValue { get; set; }

        #endregion


        #region Constructor

        public Equation(DatabaseCore.Core core, string equation, int weightMissingValue)
        {
            this.WeightMissingValue = weightMissingValue;
            this.CaseDataLocation = core.CaseDataLocation;

            if (equation.Contains("return ") == false &&
                equation.Contains("double ") == false &&
                equation.Contains("int ") == false &&
                equation.Contains(" long") == false)
            {
                equation = "return " + equation + ";";
            }

            this.Core = core;
            this.EquationString = equation.Replace("\n", " ").Replace("\r", " ");
            this.Values = new Dictionary<string, EquationPlaceHolder>();

            this.ParsePlaceHolders();
            this.Validate();

            if (SecurityCheck(new Data(), new Database(core, null, 1), null).Count > 0)
            {
                //throw new Exception("InsecureEquationDetected");
                Log("<![CDATA[[InsecureEquationDetected]: " + this.EquationString + "]]>");
                return;
            }

            EquationEvaluator evaluator = new EquationEvaluator(this);

            try
            {
                this.Assembly = evaluator.Compile();
            }
            catch (Exception ex)
            {
                Log(string.Format(
                    "<Error><![CDATA[{0}]]></Error><Equation><![CDATA[{1}]]></Equation>",
                    ex.ToString(),
                    this.EquationString
                ));
            }
        }

        #endregion


        #region Methods

        private void ParsePlaceHolders()
        {
            string[] parts = this.EquationString.Split('[');

            foreach (string part in parts)
            {
                if (!part.Contains("]"))
                    continue;

                string placeHolder = part.Split(']')[0];

                int index;
                if (placeHolder == "" || placeHolder.StartsWith("var_") || int.TryParse(placeHolder, out index))
                {
                    continue;
                }

                EquationPlaceHolder equationPlaceHolder = new EquationPlaceHolder(
                    this,
                    placeHolder
                );

                if (!this.Values.ContainsKey(placeHolder))
                    this.Values.Add(placeHolder, equationPlaceHolder);
            }
        }
        public static List<EquationPlaceHolder> ParsePlaceHolders(string equationString)
        {
            List<EquationPlaceHolder> result = new List<EquationPlaceHolder>();

            string[] parts = equationString.Split('[');

            foreach (string part in parts)
            {
                if (!part.Contains("]"))
                    continue;

                string placeHolder = part.Split(']')[0];

                int index;
                if (placeHolder == "" || placeHolder.StartsWith("var_") || int.TryParse(placeHolder, out index))
                {
                    continue;
                }

                EquationPlaceHolder equationPlaceHolder = new EquationPlaceHolder(
                    null,
                    placeHolder
                );

                result.Add(equationPlaceHolder);
            }

            return result;
        }

        public List<EquationValidationError> Validate()
        {
            List<EquationValidationError> result = new List<EquationValidationError>();

            foreach (EquationPlaceHolder placeHolder in this.Values.Values)
            {
                if (!placeHolder.Validate())
                {
                    result.Add(new EquationValidationError(
                        EquationValidationErrorType.IncorrectPlaceHolder,
                        placeHolder.PlaceHolder
                    ));
                }
            }

            return result;
        }

        public List<EquationValidationError> SecurityCheck(
            Data filter,
            StorageMethods.Database storageMethod,
            WeightingFilterCollection weights
        )
        {
            //return new List<EquationValidationError>();

            List<EquationValidationError> result = new List<EquationValidationError>();

            string equation = this.Render(false);

            string fileName = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "")),
                "Resources",
                "EquationValidatorConfiguration.xml"
            );

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            List<string> validKeys = new List<string>();

            foreach (XmlNode xmlNode in document.DocumentElement.ChildNodes)
            {
                validKeys.Add(xmlNode.InnerText);
            }

            // To remove the string values:
            int index = 0;

            while (index != -1)
            {
                if (index + 1 >= equation.Length)
                    break;

                index = equation.IndexOf('"', index + 1);

                if (index == -1)
                    break;

                int index2 = equation.IndexOf('"', index + 1);

                if (index2 == -1)
                    break;

                equation = equation.Remove(index + 1, (index2 - 1) - index);

                index += 2;
            }

            string[] parts = equation.Split(new string[]{
                " ",
                ",",
                "(",
                ")",
                "+",
                "-",
                "~",
                "*",
                "/",
                "==",
                "<",
                "<=",
                ">",
                ">=",
                "?",
                ":",
                ";",
                "=",
                "{",
                "}",
                "\t"
            }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                if (part.StartsWith("\"") && part.EndsWith("\"") && part.Count(x => x == '"') == 2)
                    continue;

                if (part.StartsWith("[") && part.EndsWith("]"))
                {
                    int v;

                    if (int.TryParse(part.Substring(1, part.Length - 2), out v))
                        continue;
                }

                double value;

                if (double.TryParse(part, out value))
                    continue;

                if (part.StartsWith("var_"))
                    continue;

                if (!validKeys.Contains(part))
                {
                    result.Add(new EquationValidationError(
                        EquationValidationErrorType.MethodNotSupported,
                        part
                    ));
                }
            }

            return result;
        }

        public List<EquationValidationError> SyntaxCheck(Data filter, Database storageMethod, WeightingFilterCollection weights)
        {
            try
            {
                EquationEvaluator evaluator = new EquationEvaluator(this);

                evaluator.Compile();
                /*string value = evaluator.Compile(
                    filter,
                    storageMethod,
                    weights
                );*/

                return new List<EquationValidationError>();
            }
            catch (Exception ex)
            {
                List<EquationValidationError> result = new List<EquationValidationError>();

                result.Add(new EquationValidationError(EquationValidationErrorType.Syntax, ex.Message));

                return result;
            }
        }

        public string[] Calculate(
            Data basefilter,
            Data filter,
            StorageMethods.Database storageMethod,
            WeightingFilterCollection weights
        )
        {
            if (this.Assembly == null)
                return new string[] { "0", "0" };

            try
            {
                //return (decimal)new System.Data.DataTable().Compute(equation, null);
                //EquationEvaluator evaluator = new EquationEvaluator(this);

                List<object> argumentsWeighted = new List<object>();
                List<object> argumentsUnweighted = new List<object>();

                foreach (string key in this.Values.Keys)
                {
                    if (this.Values[key].Type == EquationPlaceHolderType.Value)
                    {
                        Data data = this.Values[key].GetValue(basefilter, filter, storageMethod, weights);

                        argumentsWeighted.Add(data.Value);
                        argumentsUnweighted.Add(data.UnweightedValue);
                    }
                    else
                    {
                        Data data = this.Values[key].GetValue(basefilter, filter, storageMethod, weights);

                        argumentsWeighted.Add(data.Responses);
                        argumentsUnweighted.Add(data.Responses);

                        //argumentsWeighted.Add(data.Responses.Select(x => x.Value[0]).ToArray());
                        //argumentsUnweighted.Add(data.Responses.Select(x => 1.0).ToArray());
                    }
                }

                return new string[] {
                    this.Assembly.Evaluate(argumentsWeighted.ToArray()),
                    this.Assembly.Evaluate(argumentsUnweighted.ToArray())
                };
            }
            catch (Exception ex)
            {
                Log(string.Format(
                    "<Error><![CDATA[{0}]]></Error><Equation><![CDATA[{1}]]></Equation>",
                    ex.ToString(),
                    this.EquationString
                ));

                return new string[] { "0", "0" };
            }
        }

        public string Render(bool useData)
        {
            string equation = this.EquationString;

            // Run through all place holders of the equation.
            foreach (string key in this.Values.Keys)
            {
                if (this.Values[key].Type == EquationPlaceHolderType.ValuesArray)
                {
                    equation = equation.Replace(
                        "[" + key + "]",
                        useData ? "_" + this.Values[key].Identity.ToString().Replace("-", "") : "new Dictionary<Guid, double[]>()"
                    );
                    // ToDo: 
                    continue;
                }

                /*string value = "";

                if(useData == false)
                {
                    value = "0";
                }
                else if(this.Values[key].Type == EquationPlaceHolderType.Value)
                {
                    value = this.Values[key].GetValue(filter, storageMethod, weights).Value.ToString(new CultureInfo(2057));
                }
                else
                {
                    Data data = this.Values[key].GetValue(filter, storageMethod, weights);

                    value = string.Join(",", data.Responses.Select(x => x.Value[0]));
                }

                // Replace the place holder with it's value.
                equation = equation.Replace(
                    "[" + key + "]",
                    value
                );*/

                // Replace the place holder with the method argument.
                equation = equation.Replace(
                    "[" + key + "]",
                    useData ? "_" + this.Values[key].Identity.ToString().Replace("-", "") : "0"
                );
            }

            return equation;
        }

        private void Log(string message)
        {
            return;
            try
            {
                string fileName = Path.Combine(
                    new DirectoryInfo(Path.GetDirectoryName(System.Reflection.
                    Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", ""))).Parent.FullName,
                    "Logs",
                    DateTime.Today.ToString("yyyyMMdd")
                );

                if (!Directory.Exists(fileName))
                    Directory.CreateDirectory(fileName);

                fileName = Path.Combine(
                    fileName,
                    "EquationLog.xml"
                );

                if (!File.Exists(fileName))
                {
                    File.WriteAllText(
                        fileName,
                        "<Logs></Logs>"
                    );
                }

                XmlDocument document = new XmlDocument();
                document.Load(fileName);

                document.DocumentElement.InnerXml += string.Format(
                    "<Log Timestamp=\"{0}\">{1}</Log>",
                    DateTime.Now,
                    message
                );

                document.Save(fileName);
            }
            catch { }
        }

        #endregion
    }

    public class EquationPlaceHolder
    {
        #region Properties

        public bool Valid { get; set; }

        public bool Unfiltered { get; set; }

        public bool Unfiltered2 { get; set; }

        public Guid Identity { get; set; }

        public EquationPlaceHolderType Type { get; set; }

        public string PlaceHolder { get; set; }

        public Equation Equation { get; set; }

        public string Study { get; set; }
        public string Variable { get; set; }
        public string Category { get; set; }
        public string WeightingVariable { get; set; }

        public bool IsTaxonomy { get; set; }

        public Guid? IdStudy { get; set; }
        public Guid IdVariable { get; set; }
        public Guid? IdCategory { get; set; }
        public Guid? IdWeightingVariable { get; set; }
        public EquationPlaceHolderFilter Filter { get; set; }
        public VariableType VariableType { get; set; }

        #endregion


        #region Constructor

        public EquationPlaceHolder(Equation equation, string placeHolder)
        {
            this.Identity = Guid.NewGuid();

            if (placeHolder.StartsWith("###") && placeHolder.EndsWith("###"))
            {
                this.Type = EquationPlaceHolderType.ValuesArray;

                placeHolder = placeHolder.Substring(3, placeHolder.Length - 6);
            }

            if (placeHolder.StartsWith("**"))
            {
                placeHolder = placeHolder.Remove(0, 2);
                this.Unfiltered2 = true;
            }
            else if (placeHolder.StartsWith("*"))
            {
                placeHolder = placeHolder.Remove(0, 1);
                this.Unfiltered = true;
            }

            this.PlaceHolder = placeHolder;
            this.Equation = equation;
            this.Category = null;
            this.WeightingVariable = null;
            this.IsTaxonomy = true;

            if (placeHolder.StartsWith("/"))
            {
                this.IsTaxonomy = false;
                this.Study = placeHolder.Split('/')[1].Split('\\')[0];
                placeHolder = placeHolder.Replace("/" + this.Study + "\\", "");
            }

            if (placeHolder.Contains("$"))
            {
                this.WeightingVariable = placeHolder.Split('$')[1];
                placeHolder = placeHolder.Replace("$" + this.WeightingVariable + "$", "");
            }

            if (placeHolder.Contains("!"))
            {
                string filterStr = placeHolder.Split('!')[1];
                placeHolder = placeHolder.Replace("!" + filterStr + "!", "");

                this.Filter = new EquationPlaceHolderFilter(this, filterStr);
            }

            this.Variable = placeHolder.Split('.')[0];

            if (placeHolder.Contains("."))
            {
                this.Category = placeHolder.Split('.')[1].Split('$')[0];
            }
        }

        #endregion


        #region Methods

        public bool Validate()
        {
            this.Valid = true;

            if (!this.IsTaxonomy)
            {
                Guid idStudy;

                if (!Guid.TryParse(this.Study, out idStudy))
                {
                    this.Valid = false;
                    return false;
                }

                this.IdStudy = idStudy;
            }

            object idVariable;

            if (this.IsTaxonomy)
            {
                Dictionary<string, List<object[]>> variables = this.Equation.Core.TaxonomyVariables.ExecuteReaderDict<string>(
                    "SELECT Name, Id FROM TaxonomyVariables",
                    new object[] {}
                );

                if (variables.ContainsKey(this.Variable))
                    idVariable = variables[this.Variable][0][1];
                else
                    idVariable = null;

                /*idVariable = this.Equation.Core.TaxonomyVariables.GetValue(
                    "Id",
                    "Name",
                    this.Variable
                );*/
            }
            else
            {
                idVariable = this.Equation.Core.Variables.GetValue(
                    "Id",
                    new string[] { "IdStudy", "Name" },
                    new object[] { this.IdStudy.Value, this.Variable }
                );
            }

            //if (idVariable == null)
            //    return false;
            if (idVariable == null)
            {
                this.Valid = false;
                return true;
            }

            this.IdVariable = (Guid)idVariable;

            if (this.IsTaxonomy)
            {
                Dictionary<Guid, List<object[]>> variableTypes = 
                    this.Equation.Core.TaxonomyVariables.ExecuteReaderDict<Guid>(
                    "SELECT [Id], [Type] FROM TaxonomyVariables",
                    new object[] {}
                );

                if (variableTypes.ContainsKey(this.IdVariable))
                    this.VariableType = (VariableType)variableTypes[this.IdVariable][0][1];

                /*this.VariableType = (VariableType)this.Equation.Core.TaxonomyVariables.GetValue(
                    "Type",
                    "Id",
                    this.IdVariable
                );*/
            }
            else
            {
                this.VariableType = (VariableType)this.Equation.Core.Variables.GetValue(
                    "Type",
                    "Id",
                    this.IdVariable
                );
            }

            if (this.Category != null && this.VariableType != VariableType.Numeric)
            {
                object idCategory;

                if (this.IsTaxonomy)
                {
                    Dictionary<Guid, Dictionary<string, List<object[]>>> categories = this.Equation.Core.TaxonomyCategories.ExecuteReaderDict<Guid, string>(
                        "SELECT IdTaxonomyVariable, Name, Id FROM TaxonomyCategories",
                        new object[] { }
                    );

                    if (categories.ContainsKey(this.IdVariable) && categories[this.IdVariable].ContainsKey(this.Category))
                        idCategory = categories[this.IdVariable][this.Category][0][2];
                    else
                        idCategory = null;

                    /*idCategory = this.Equation.Core.TaxonomyCategories.GetValue(
                        "Id",
                        new string[] { "IdTaxonomyVariable", "Name" },
                        new object[] { this.IdVariable, this.Category }
                    );*/
                }
                else
                {
                    idCategory = this.Equation.Core.Categories.GetValue(
                        "Id",
                        new string[] { "IdVariable", "Name" },
                        new object[] { this.IdVariable, this.Category }
                    );
                }

                if (idCategory == null)
                {
                    this.Valid = false;
                    return false;
                }

                this.IdCategory = (Guid)idCategory;
            }

            if (this.WeightingVariable != null && this.WeightingVariable != "NONE")
            {
                object idWeightingVariable;

                if (this.IsTaxonomy)
                {
                    idWeightingVariable = this.Equation.Core.TaxonomyVariables.GetValue(
                        "Id",
                        "Name",
                        this.WeightingVariable
                    );
                }
                else
                {
                    idWeightingVariable = this.Equation.Core.Variables.GetValue(
                        "Id",
                        new string[] { "IdStudy", "Name" },
                        new object[] { this.IdStudy.Value, this.WeightingVariable }
                    );
                }

                if (idWeightingVariable == null)
                {
                    this.Valid = false;
                    return false;
                }

                this.IdWeightingVariable = (Guid)idWeightingVariable;
            }

            /*if (this.Filter != null)
            {
                object idFilterVariable;

                if (this.IsTaxonomy)
                {
                    idFilterVariable = this.Equation.Core.TaxonomyVariables.GetValue(
                        "Id",
                        "Name",
                        this.Filter.Split('.')[0]
                    );
                }
                else
                {
                    idFilterVariable = this.Equation.Core.Variables.GetValue(
                        "Id",
                        new string[] { "IdStudy", "Name" },
                        new object[] { this.IdStudy.Value, this.Filter.Split('.')[0] }
                    );
                }

                if (idFilterVariable == null)
                    return false;

                this.IdFilterVariable = (Guid)idFilterVariable;

                if (this.Filter.Contains("."))
                {
                    object idFilterCategory;

                    if (this.IsTaxonomy)
                    {
                        idFilterCategory = this.Equation.Core.TaxonomyCategories.GetValue(
                            "Id",
                            new string[] { "IdTaxonomyVariable", "Name" },
                            new object[] { idFilterVariable, this.Filter.Split('.')[1] }
                        );
                    }
                    else
                    {
                        idFilterCategory = this.Equation.Core.Categories.GetValue(
                            "Id",
                            new string[] { "IdVariable", "Name" },
                            new object[] { idFilterVariable, this.Filter.Split('.')[1] }
                        );
                    }

                    if (idFilterCategory == null)
                        return false;

                    this.IdFilterCategory = (Guid)idFilterCategory;
                }
            }*/

            return true;
        }

        public Data GetValue(
            Data basefilter,
            Data filter,
            StorageMethods.Database storageMethod,
            WeightingFilterCollection weights
        )
        {
            if (!this.Valid)
                return new Data();

            Data result;

            if (this.IdWeightingVariable.HasValue)
            {
                XmlDocument document = new XmlDocument();

                XmlNode xmlNode = document.CreateElement("WeightingVariables");
                xmlNode.AddAttribute("DefaultWeighting", this.IdWeightingVariable.Value.ToString());
                xmlNode.AddAttribute("IsTaxonomy", this.IsTaxonomy);

                weights = new WeightingFilterCollection(null, this.Equation.Core, xmlNode);

                weights.LoadRespondents(filter);
            }
            else if (this.WeightingVariable == "NONE")
            {
                weights = null;
            }

            if (this.Unfiltered)
                filter = null;

            if (this.Unfiltered2)
                filter = basefilter;

            if (this.Filter != null)
                filter = this.Filter.Evalute(filter, filter);

            /*if (this.IdFilterVariable.HasValue)
            {
                if (this.IdFilterCategory.HasValue)
                {
                    filter = storageMethod.GetRespondents(
                        this.IdFilterCategory.Value,
                        this.IdFilterVariable.Value,
                        this.IsTaxonomy,
                        this.Equation.CaseDataLocation,
                        filter,
                        null,
                        null
                    );
                }
                else
                {
                    filter = storageMethod.GetRespondents(
                        this.IdFilterVariable.Value,
                        this.IsTaxonomy,
                        this.Equation.CaseDataLocation,
                        filter,
                        null
                    );
                }
            }*/

            if (this.IdCategory.HasValue)
            {
                result = storageMethod.GetRespondents(
                    this.IdCategory.Value,
                    this.IdVariable,
                    this.IsTaxonomy,
                    this.Equation.Core.CaseDataLocation,
                    filter,
                    weights
                );
            }
            else
            {
                if (this.VariableType != VariableType.Numeric)
                {
                    result = storageMethod.GetRespondents(
                        this.IdVariable,
                        this.IsTaxonomy,
                        this.Equation.Core.CaseDataLocation,
                        filter,
                        weights
                    );
                }
                else
                {
                    result = storageMethod.GetRespondentsNumeric(
                        this.IdVariable,
                        this.IsTaxonomy,
                        this.Equation.CaseDataLocation,
                        filter,
                        weights
                    );

                    if (this.Category == "Mean")
                        result.Value = result.Value / result.Base;
                }
            }

            return result;
        }

        #endregion
    }

    public class EquationValidationError
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type of the
        /// equation validation error.
        /// </summary>
        public EquationValidationErrorType Type { get; set; }

        /// <summary>
        /// Gets or sets the item where the
        /// equation validation error refers to.
        /// </summary>
        public string Item { get; set; }

        #endregion


        #region Constructor

        public EquationValidationError()
        {

        }

        public EquationValidationError(EquationValidationErrorType type, string item)
        {
            this.Type = type;
            this.Item = item;
        }

        #endregion


        #region Methods

        public override string ToString()
        {
            return string.Format(
                ((WebUtilities.LanguageManager)System.Web.HttpContext.Current.Session["LanguageManager"]).GetText("EquationValidationError" + this.Type),
                this.Item
            );
        }

        #endregion
    }

    public enum EquationValidationErrorType
    {
        Syntax,
        IncorrectPlaceHolder,
        MethodNotSupported
    }

    public enum EquationPlaceHolderType
    {
        Value,
        ValuesArray
    }
}