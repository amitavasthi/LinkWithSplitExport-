using ApplicationUtilities.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkingHelper2.Classes
{
    public class Study
    {
        #region Properties

        public string Source { get; set; }

        public string Name { get; set; }

        public Dictionary<string, TaxonomyVariable> Variables { get; set; }

        public Dictionary<string, Dictionary<string, List<string>>> VariableLinks { get; set; }

        public Dictionary<string, Dictionary<string, List<string>>> CategoryLinks { get; set; }

        public bool Linked { get; set; }

        #endregion


        #region Constructor

        public Study()
        {
            this.Linked = false;
            this.Variables = new Dictionary<string, TaxonomyVariable>();
            this.VariableLinks = new Dictionary<string, Dictionary<string, List<string>>>();
            this.CategoryLinks = new Dictionary<string, Dictionary<string, List<string>>>();
        }

        #endregion


        #region Methods

        public void Parse(string fileName, string augmentFileName)
        {
            this.Linked = true;
            ParseMetadata(fileName);
            ParseAugment(augmentFileName);
        }

        public void ParseMetadata(string fileName)
        {
            this.Source = fileName;
            this.Name = new FileInfo(fileName).Name.Replace(new FileInfo(fileName).Extension, "");

            MDMLib.Document document = OpenMetadata(fileName, MDMLib.openConstants.oREAD);

            // Run through all variables of the mdm lib document.
            foreach (MDMLib.Variable _variable in document.Variables)
            {
                // Check if the variable is a field.
                if ((_variable is MDMLib.IMDMField) == false)
                    continue;

                if (!_variable.HasCaseData)
                    continue;

                MDMLib.IMDMField field = (MDMLib.IMDMField)_variable;

                if (this.Variables.ContainsKey(field.FullName))
                    continue;

                this.Variables.Add(field.FullName.Trim(), new TaxonomyVariable(
                    field.FullName.Trim(),
                    field.Label.Trim().ToLower(),
                    this
                ));


                // Run through all categories of the variable.
                foreach (MDMLib.IElement _category in _variable.Categories)
                {
                    if (this.Variables[field.FullName.Trim()].Categories.ContainsKey(_category.Name.Trim()))
                        continue;

                    this.Variables[field.FullName.Trim()].Categories.Add(_category.Name.Trim(), new TaxonomyCategory(
                        this.Variables[field.FullName.Trim()],
                        _category.Name.Trim(),
                        _category.Label.Trim().ToLower()
                    ));

                    if (_category.Factor != null)
                        this.Variables[field.FullName.Trim()].Categories[_category.Name.Trim()].Value = _category.Factor;
                }
            }
        }

        private void ParseAugment(string fileName)
        {
            ExcelReader reader = new ExcelReader(fileName);

            reader.ActiveSheet = reader.Workbook.Worksheets[0];

            string taxonomyVariableName;
            string variableName;
            string taxonomyCategoryName;
            string categoryName;

            while (reader.Read())
            {
                taxonomyVariableName = reader[0].Trim();
                variableName = reader[1].Trim();
                taxonomyCategoryName = reader[2].Trim();
                categoryName = reader[3].Trim();

                if (!this.Variables.ContainsKey(variableName))
                    continue;

                this.Variables[variableName].Linked = true;
                this.Variables[variableName].LinkedTaxonomyVariable = taxonomyVariableName;

                if (!this.VariableLinks.ContainsKey(this.Variables[variableName].Label))
                {
                    this.VariableLinks.Add(this.Variables[variableName].Label, new Dictionary<string, List<string>>());
                    this.VariableLinks[this.Variables[variableName].Label].Add(variableName, new List<string>());
                }

                if (!this.VariableLinks[this.Variables[variableName].Label].ContainsKey(variableName))
                    continue;

                if (!this.VariableLinks[this.Variables[variableName].Label][variableName].Contains(taxonomyVariableName))
                    this.VariableLinks[this.Variables[variableName].Label][variableName].Add(taxonomyVariableName);

                if (string.IsNullOrEmpty(categoryName))
                    continue;

                if (!this.CategoryLinks.ContainsKey(variableName))
                    this.CategoryLinks.Add(variableName, new Dictionary<string, List<string>>());

                if (!this.CategoryLinks[variableName].ContainsKey(categoryName))
                    this.CategoryLinks[variableName].Add(categoryName, new List<string>());

                if (!this.CategoryLinks[variableName][categoryName].Contains(taxonomyCategoryName))
                    this.CategoryLinks[variableName][categoryName].Add(taxonomyCategoryName);
            }
        }

        /// <summary>
        /// Opens a dimensions file.
        /// </summary>
        /// <param name="fileName">The full path to the file to open.</param>
        /// <param name="openMode">The open mode which should be used.</param>
        /// <returns></returns>
        public MDMLib.Document OpenMetadata(string fileName, MDMLib.openConstants openMode)
        {
            // Get the file information of the file to open.
            FileInfo fInfo = new FileInfo(fileName);

            // Get the file's file extension.
            string fileExtension = fInfo.Extension.ToLower();

            //Opens MDD or MDSC, depending if the dsc is empty (MDD) or if there is a mdsc name.
            MRDSCReg.Components dscs = null;

            try
            {
                dscs = new MRDSCReg.Components();
            }
            catch { }

            MRDSCReg.Component dsc = null;

            MDMLib.Document document = new MDMLib.Document();

            // Check if the file is a .mdd file.
            if (fileExtension.Length > 0 & fileExtension.ToLower() != ".mdd")
            {
                // Run through all dimensions components in the components list.
                foreach (MRDSCReg.Component testDsc in dscs)
                {
                    string dscFileExt = testDsc.Metadata.
                        FileMasks.String.ToLower();

                    if (dscFileExt.Length != 0)
                    {
                        if (dscFileExt.Split('|')[1].Split(';')[0] == "*" + fileExtension.ToLower())
                        {
                            dsc = testDsc;
                        }
                    }
                }

                if (dsc != null)
                {
                    // Open the file.
                    document = (MDMLib.Document)dsc.Metadata.Open(
                        fileName,
                        "",
                        MRDSCReg.openConstants.oREAD
                    );
                }
            }
            else
            {
                // Open the document as .mdd file.
                document.Open(fileName, "", openMode);
            }
            return document;
        }

        #endregion
    }
}
