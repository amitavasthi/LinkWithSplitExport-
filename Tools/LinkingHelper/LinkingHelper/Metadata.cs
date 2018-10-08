using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkingHelper
{
    public class Metadata
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the metadata file.
        /// </summary>
        public string FileName { get; set; }

        public Dictionary<string, Variable> Variables { get; set; }

        public Dictionary<string, Dictionary<string, string>> Categories { get; set; }

        public Dictionary<string, Dictionary<string, double>> CategoryFactors { get; set; }

        #endregion


        #region Constructor

        public Metadata(string fileName)
        {
            this.FileName = fileName;

            this.Variables = new Dictionary<string, Variable>();
            this.Categories = new Dictionary<string, Dictionary<string, string>>();
            this.CategoryFactors = new Dictionary<string, Dictionary<string, double>>();

            if (this.FileName != null)
                this.Read();
        }

        #endregion


        #region Methods

        private void Read()
        {
            MDMLib.Document document = OpenMetadata(this.FileName, MDMLib.openConstants.oREAD);

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

                this.Variables.Add(field.FullName, new Variable(
                    field.FullName,
                    field.Label
                ));
                this.Categories.Add(field.FullName, new Dictionary<string, string>());
                this.CategoryFactors.Add(field.FullName, new Dictionary<string, double>());


                // Run through all categories of the variable.
                foreach (MDMLib.IElement _category in _variable.Categories)
                {
                    if (this.Categories[field.FullName].ContainsKey(_category.Name))
                        continue;

                    double factor;

                    double.TryParse(_category.Factor, out factor);

                    this.Categories[field.FullName].Add(_category.Name, _category.Label);
                    this.CategoryFactors[field.FullName].Add(_category.Name, factor);
                }
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
